/**
 * Tracer API — Notification Feed Load Test (M7 Hardening)
 * ----------------------------------------------------------
 * Exit criteria: 5,000 concurrent VUs, p95 < 100ms, error rate < 1%.
 *
 * Stage plan:
 *   0–2min   ramp up to 5,000 VUs
 *   2–5min   hold at 5,000 VUs
 *   5–6min   ramp down
 *
 * Scenarios simulated:
 *   1. Poll notification feed (GET /api/v1/notifications)  — 80% of traffic
 *   2. Get single notification by ID                        — 15% of traffic
 *   3. Mark notification as read (POST)                     — 5% of traffic
 *
 * Usage:
 *   k6 run tests/k6/load-test-notifications.js \
 *     -e BASE_URL=http://localhost:5000 \
 *     -e USERNAME=admin@tracer.io \
 *     -e PASSWORD=Password123! \
 *     --out json=results/load-test-notifications.json
 */

import http from "k6/http";
import { check, group, sleep } from "k6";
import { Counter, Rate, Trend } from "k6/metrics";

const errorRate = new Rate("errors");
const feedDuration = new Trend("notification_feed_duration", true);
const rateLimit429 = new Counter("rate_limited_429");

export const options = {
  scenarios: {
    notification_feed: {
      executor: "ramping-vus",
      stages: [
        { duration: "2m", target: 4000 }, // 80% of 5k
        { duration: "3m", target: 4000 },
        { duration: "1m", target: 0 },
      ],
      exec: "pollFeed",
    },
    notification_single: {
      executor: "ramping-vus",
      stages: [
        { duration: "2m", target: 750 }, // 15%
        { duration: "3m", target: 750 },
        { duration: "1m", target: 0 },
      ],
      exec: "getById",
    },
    notification_mark_read: {
      executor: "ramping-vus",
      stages: [
        { duration: "2m", target: 250 }, // 5%
        { duration: "3m", target: 250 },
        { duration: "1m", target: 0 },
      ],
      exec: "markRead",
    },
  },
  thresholds: {
    http_req_failed: ["rate<0.01"],
    http_req_duration: ["p(95)<100"],
    notification_feed_duration: ["p(95)<100"],
    errors: ["rate<0.01"],
    rate_limited_429: ["count<100"],
  },
};

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";

export function setup() {
  const payload = JSON.stringify({
    username: __ENV.USERNAME || "admin@tracer.io",
    password: __ENV.PASSWORD || "Password123!",
  });
  const res = http.post(`${BASE_URL}/api/v1/auth/login`, payload, {
    headers: { "Content-Type": "application/json" },
  });
  if (res.status !== 200) throw new Error(`Login failed: ${res.status}`);
  return { token: JSON.parse(res.body).token };
}

function headers(data) {
  return {
    Authorization: `Bearer ${data.token}`,
    Accept: "application/json",
    "Accept-Encoding": "br, gzip",
  };
}

export function pollFeed(data) {
  const start = Date.now();
  const res = http.get(
    `${BASE_URL}/api/v1/notifications?page=1&pageSize=20&unreadOnly=false`,
    { headers: headers(data) }
  );
  feedDuration.add(Date.now() - start);

  if (res.status === 429) {
    rateLimit429.add(1);
  } else {
    check(res, { "feed 200": (r) => r.status === 200 }) || errorRate.add(1);
  }

  sleep(Math.random() * 0.3);
}

export function getById(data) {
  // Use a stable, well-known UUID. In a real test this would be seeded test data.
  const res = http.get(
    `${BASE_URL}/api/v1/notifications/00000000-0000-0000-0000-000000000001`,
    { headers: headers(data) }
  );
  // 404 is acceptable (ID may not exist in DB) — we are testing throughput not data.
  if (res.status === 429) {
    rateLimit429.add(1);
  } else {
    check(res, {
      "single notification status valid": (r) =>
        r.status === 200 || r.status === 404,
    }) || errorRate.add(1);
  }

  sleep(Math.random() * 0.5);
}

export function markRead(data) {
  const res = http.post(
    `${BASE_URL}/api/v1/notifications/00000000-0000-0000-0000-000000000001/read`,
    null,
    { headers: headers(data) }
  );
  if (res.status === 429) {
    rateLimit429.add(1);
  } else {
    check(res, {
      "mark read status valid": (r) =>
        r.status === 204 || r.status === 404,
    }) || errorRate.add(1);
  }

  sleep(1);
}
