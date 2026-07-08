/**
 * Tracer API — Asset Grid Load Test (M7 Hardening)
 * ---------------------------------------------------
 * Exit criteria: 5,000 concurrent VUs, p95 < 100ms, error rate < 1%.
 *
 * Stage plan:
 *   0–2min   ramp up to 5,000 VUs
 *   2–5min   hold at 5,000 VUs  (steady-state measurement window)
 *   5–6min   ramp down
 *
 * Usage:
 *   k6 run tests/k6/load-test-assets.js \
 *     -e BASE_URL=http://localhost:5000 \
 *     -e USERNAME=admin@tracer.io \
 *     -e PASSWORD=Password123! \
 *     --out json=results/load-test-assets.json
 */

import http from "k6/http";
import { check, group, sleep } from "k6";
import { Counter, Rate, Trend } from "k6/metrics";

const errorRate = new Rate("errors");
const assetDuration = new Trend("asset_get_duration", true);
const rateLimit429 = new Counter("rate_limited_429");

export const options = {
  stages: [
    { duration: "2m", target: 5000 },  // ramp up
    { duration: "3m", target: 5000 },  // hold
    { duration: "1m", target: 0 },     // ramp down
  ],
  thresholds: {
    http_req_failed: ["rate<0.01"],          // <1% non-2xx/3xx
    http_req_duration: ["p(95)<100"],        // M7 exit criteria
    asset_get_duration: ["p(95)<100"],       // specific to asset grid
    errors: ["rate<0.01"],
    rate_limited_429: ["count<50"],          // allow a small burst of 429s during ramp
  },
};

const BASE_URL = __ENV.BASE_URL || "http://localhost:5000";

// Shared token cache — one login per VU init phase.
let cachedToken = null;

export function setup() {
  // Obtain a token once and share across VUs.
  const payload = JSON.stringify({
    email: __ENV.USERNAME || "admin@tracer.io",
    password: __ENV.PASSWORD || "Admin123!",
  });
  const res = http.post(`${BASE_URL}/api/v1/auth/login`, payload, {
    headers: { "Content-Type": "application/json" },
  });
  if (res.status !== 200) throw new Error(`Login failed: ${res.status} ${res.body}`);
  return { token: JSON.parse(res.body).token };
}

export default function (data) {
  const headers = {
    Authorization: `Bearer ${data.token}`,
    Accept: "application/json, application/problem+json",
    "Accept-Encoding": "br, gzip",
  };

  group("GET /api/v1/assets", () => {
    const page = Math.floor(Math.random() * 10) + 1; // simulate browsing multiple pages
    const start = Date.now();
    const res = http.get(`${BASE_URL}/api/v1/assets?page=${page}&pageSize=50`, { headers });
    assetDuration.add(Date.now() - start);

    if (res.status === 429) {
      rateLimit429.add(1);
    } else {
      check(res, {
        "assets status 200": (r) => r.status === 200,
        "assets has content-type json": (r) =>
          r.headers["Content-Type"]?.includes("application/json"),
        "assets body non-empty": (r) => r.body.length > 0,
      }) || errorRate.add(1);
    }
  });

  sleep(Math.random() * 0.5); // 0–500ms think time between requests
}
