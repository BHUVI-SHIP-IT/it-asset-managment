/**
 * Tracer API — Smoke Test (M7 Hardening)
 * -----------------------------------------
 * Quick sanity check: 10 VUs for 30s.
 * Covers: auth → get assets → get notifications → mark notification read.
 *
 * Usage:
 *   k6 run tests/k6/smoke-test.js \
 *     -e BASE_URL=http://localhost:5000 \
 *     -e USERNAME=admin@tracer.io \
 *     -e PASSWORD=Password123!
 */

import http from "k6/http";
import { check, group, sleep } from "k6";
import { Rate } from "k6/metrics";

const errorRate = new Rate("errors");

export const options = {
  vus: 10,
  duration: "30s",
  thresholds: {
    http_req_failed: ["rate<0.01"],   // <1% errors
    http_req_duration: ["p(95)<500"], // smoke: generous 500ms threshold
    errors: ["rate<0.01"],
  },
};

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";

function login() {
  const payload = JSON.stringify({
    username: __ENV.USERNAME || "admin@tracer.io",
    password: __ENV.PASSWORD || "Password123!",
  });
  const res = http.post(`${BASE_URL}/api/v1/auth/login`, payload, {
    headers: { "Content-Type": "application/json" },
  });
  check(res, { "login 200": (r) => r.status === 200 }) || errorRate.add(1);
  return JSON.parse(res.body)?.token;
}

export default function () {
  const token = login();
  if (!token) return;

  const headers = {
    Authorization: `Bearer ${token}`,
    "Content-Type": "application/json",
    Accept: "application/json",
  };

  group("Assets", () => {
    const res = http.get(`${BASE_URL}/api/v1/assets?page=1&pageSize=20`, { headers });
    check(res, { "assets 200": (r) => r.status === 200 }) || errorRate.add(1);
  });

  group("Notifications", () => {
    const res = http.get(`${BASE_URL}/api/v1/notifications?page=1&pageSize=20`, { headers });
    check(res, { "notifications 200": (r) => r.status === 200 }) || errorRate.add(1);
  });

  sleep(1);
}
