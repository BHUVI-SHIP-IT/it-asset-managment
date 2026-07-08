# Tracer — Disaster Recovery Runbook

> **M7 Hardening Artefact** | Audience: DevOps on-call engineers and DBAs
>
> This runbook documents the procedures for recovering the Tracer IT Asset Management
> system from a catastrophic failure. It must be reviewed and practised every quarter.

---

## 1. Service Overview

| Component | Technology | Criticality |
|-----------|-----------|-------------|
| API | ASP.NET Core 9 / Kestrel | P0 |
| Database | SQL Server 2022 (Azure SQL or on-prem) | P0 |
| Cache | Redis | P1 (degraded mode possible) |
| Background Jobs | Hangfire (SQL Server backed) | P1 |

---

## 2. Recovery Objectives

| Metric | Target |
|--------|--------|
| **RPO** (Recovery Point Objective) | ≤ 1 hour — max data loss acceptable |
| **RTO** (Recovery Time Objective) | ≤ 4 hours — max time to restore service |

---

## 3. Backup Strategy

### 3.1 SQL Server

| Backup Type | Frequency | Retention |
|-------------|-----------|-----------|
| Full database backup | Daily 02:00 UTC | 30 days |
| Differential backup | Every 4 hours | 7 days |
| Transaction log backup | Every 15 minutes | 72 hours |

**Verify backup is healthy (run daily via monitoring):**

```sql
-- Last successful backup per database
SELECT database_name, backup_finish_date, type, backup_size / 1024 / 1024 AS size_mb
FROM msdb.dbo.backupset
WHERE database_name = 'TracerDb'
ORDER BY backup_finish_date DESC;
```

### 3.2 Redis

Redis is used as a distributed cache only. **All data is ephemeral — no backup required.**
On failure, the API automatically falls through to the database.

---

## 4. Failure Scenarios & Procedures

### 4.1 API Server Failure

**Symptoms:** Health check `GET /health/live` returns non-200 or times out.

**Steps:**

1. **Check container / process status:**
   ```bash
   docker ps -a | grep tracer-api
   # or for systemd:
   systemctl status tracer-api
   ```

2. **Inspect logs (last 200 lines):**
   ```bash
   docker logs --tail=200 tracer-api
   # or:
   journalctl -u tracer-api -n 200
   ```

3. **Restart the API:**
   ```bash
   docker restart tracer-api
   # or:
   systemctl restart tracer-api
   ```

4. **Verify health:**
   ```bash
   curl -sf http://localhost:5000/health/live && echo "OK"
   curl -sf http://localhost:5000/health/ready && echo "Ready"
   ```

5. **If restart fails**, escalate to deployment team and initiate blue/green swap (§4.4).

---

### 4.2 Database Failure — Point-in-Time Restore

**Symptoms:** `GET /health/ready` returns `Unhealthy` for `sql-server` tag. API returns 500s.

**Step 1 — Identify the last known-good point in time:**

```sql
-- Find last clean checkpoint before the failure
SELECT TOP 10 database_name, backup_finish_date, type
FROM msdb.dbo.backupset
WHERE database_name = 'TracerDb'
ORDER BY backup_finish_date DESC;
```

**Step 2 — Restore to a test instance first (mandatory — never restore directly to production):**

```sql
-- Full restore with NORECOVERY (more log backups will be applied)
RESTORE DATABASE TracerDb_Restore
  FROM DISK = N'\\backup-share\TracerDb_FULL_20260708.bak'
  WITH NORECOVERY, REPLACE,
  MOVE 'TracerDb' TO 'D:\Data\TracerDb.mdf',
  MOVE 'TracerDb_log' TO 'D:\Logs\TracerDb_ldf';

-- Apply log backups up to the desired point in time
RESTORE LOG TracerDb_Restore
  FROM DISK = N'\\backup-share\TracerDb_LOG_20260708_0230.bak'
  WITH NORECOVERY, STOPAT = '2026-07-08T02:29:59';

-- Bring online
RESTORE DATABASE TracerDb_Restore WITH RECOVERY;
```

**Step 3 — Smoke test the restored instance** (run unit tests against restored DB connection string).

**Step 4 — Apply EF Core migrations if needed:**
```bash
dotnet ef database update \
  -p src/Tracer.Persistence \
  -s src/Tracer.Api \
  --connection "Server=RESTORED_HOST;Database=TracerDb_Restore;..."
```

**Step 5 — Swap connection string and restart API.**

---

### 4.3 Redis Cache Failure

**Symptoms:** Redis health check fails. API may be slower but continues to function.

**Steps:**

1. Restart Redis:
   ```bash
   docker restart tracer-redis
   # or:
   systemctl restart redis
   ```

2. Cache will warm automatically as requests come in. No manual action required.

3. If Redis is permanently unavailable, remove the Redis connection string from
   `appsettings.Production.json` and restart the API. **The API runs fully without Redis**
   (output caching degrades to in-memory; distributed session not used).

---

### 4.4 Blue/Green Deployment Swap (RTO ≤ 4 hours)

> Use this when the current production instance is unrecoverable and a new build is needed.

```bash
# 1. Tag the last known-good image
docker tag tracer-api:latest tracer-api:last-good

# 2. Deploy the rollback image to the green slot
docker run -d --name tracer-api-green \
  --env-file /etc/tracer/production.env \
  -p 5001:5000 \
  tracer-api:last-good

# 3. Verify green is healthy
curl -sf http://localhost:5001/health/ready && echo "Green healthy"

# 4. Swap the load balancer (nginx upstream or cloud ALB rule)
#    Point traffic from :5000 (blue) to :5001 (green).

# 5. Stop the blue slot
docker stop tracer-api && docker rm tracer-api

# 6. Promote green to primary port
docker rename tracer-api-green tracer-api
```

---

## 5. DR Drill Procedure

Run this drill quarterly. It must pass before the M8 go-live gate.

### Drill Steps

1. **Announce maintenance window** (minimum 15-minute window in staging).
2. **Simulate API failure:** `docker stop tracer-api`
3. **Verify monitoring alerts fire** within 2 minutes.
4. **Restore from most recent backup** in staging using §4.2 steps.
5. **Run smoke test:**
   ```bash
   k6 run tests/k6/smoke-test.js \
     -e BASE_URL=http://staging.tracer.internal \
     -e USERNAME=drtest@tracer.io \
     -e PASSWORD=$DR_TEST_PASSWORD
   ```
6. **Record actual RTO/RPO:**
   - RTO achieved = Time from failure announcement to smoke test pass
   - RPO achieved = Age of oldest restored record vs failure timestamp

### Pass Criteria

| Metric | Threshold |
|--------|-----------|
| RTO | ≤ 4 hours |
| RPO | ≤ 1 hour |
| Smoke test error rate | < 1% |
| All health checks | `Healthy` |

---

## 6. Health Check Endpoints Reference

| Endpoint | Purpose | Expected Response |
|----------|---------|-------------------|
| `GET /health/live` | Liveness — is process running? | `200 Healthy` |
| `GET /health/ready` | Readiness — can it serve traffic? (SQL, Redis) | `200 Healthy` |

---

## 7. Escalation Contacts

> Update this table with real contacts before going to production.

| Role | Contact | Availability |
|------|---------|--------------|
| On-call DevOps | devops-oncall@tracer.io | 24/7 PagerDuty |
| DBA | dba@tracer.io | Business hours + on-call |
| Engineering Lead | engineering-lead@tracer.io | Business hours |

---

*Last updated: 2026-07-08 | Owner: Platform Engineering | Review cycle: Quarterly*
