#!/usr/bin/env bats

URL="https://${DEFAULT_INGRESS_URL}/"

@test "${URL} (metrics endpoint) is reachable" {
    run curl -s -o /dev/null -w "%{http_code}" ${URL}-/metrics
    [ "$status" -eq 0 ]
    [ "$output" -eq 200 ]
}