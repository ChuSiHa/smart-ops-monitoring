#!/usr/bin/env bash
set -euo pipefail

API="http://localhost:5143"
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZGQ4ZWEwZC0xMTY5LTQ2MDQtOGY0MS01NDBhNzdlMTcyMTEiLCJlbWFpbCI6ImV0aGFuLWRldkBnbWFpbC5jb20iLCJqdGkiOiI3NjM3ODhkYi00Y2Q3LTRlNmQtYTNhNy02MDc5MzJjZTQ3NDkiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZXRoYW4tZGV2QGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6Ik9wZXJhdG9yIiwiZXhwIjoxNzc2MjMwMzg4LCJpc3MiOiJTbWFydE9wc01vbml0b3JpbmciLCJhdWQiOiJTbWFydE9wc01vbml0b3JpbmdDbGllbnRzIn0.K0EpfqZG5k_MC57-Qoz5aVecTnjJS1BkqXHYjDngRcU"
HOST_ID="1bb8a3cf-f5fa-4684-86e0-21b76c8e37f5"

SLEEP_SECONDS=3

while true; do
  # random cpu/mem giống PowerShell Get-Random
  cpu=$(( (RANDOM % 90) + 5 ))   # 5..94
  mem=$(( (RANDOM % 80) + 10 ))  # 10..89

  # cpu_usage
  curl -sS -X POST "$API/api/metrics/ingest" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"hostId\":\"$HOST_ID\",\"metricType\":\"cpu_usage\",\"value\":$cpu,\"unit\":\"percent\",\"labels\":{\"source\":\"mock\"}}" \
    >/dev/null

  # mem_usage
  curl -sS -X POST "$API/api/metrics/ingest" \
    -H "Authorization: Bearer $TOKEN" \
    -H "Content-Type: application/json" \
    -d "{\"hostId\":\"$HOST_ID\",\"metricType\":\"mem_usage\",\"value\":$mem,\"unit\":\"percent\",\"labels\":{\"source\":\"mock\"}}" \
    >/dev/null

  sleep "$SLEEP_SECONDS"
done