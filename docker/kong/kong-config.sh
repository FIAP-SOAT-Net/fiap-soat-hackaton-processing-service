#!/bin/bash
set -e

KONG_ADMIN_URL="${KONG_ADMIN_URL:-http://kong:8001}"
MAX_RETRIES=60
RETRY_COUNT=0

echo "Waiting for Kong Admin API to be ready at $KONG_ADMIN_URL..."
until curl -s "$KONG_ADMIN_URL/status" > /dev/null 2>&1 || [ $RETRY_COUNT -eq $MAX_RETRIES ]; do
  echo "Kong not ready, retrying... (attempt $((RETRY_COUNT + 1))/$MAX_RETRIES)"
  sleep 2
  RETRY_COUNT=$((RETRY_COUNT + 1))
done

if [ $RETRY_COUNT -eq $MAX_RETRIES ]; then
  echo "Kong Admin API did not become ready in time"
  exit 1
fi

echo "Kong Admin API is ready!"

# Check if service already exists
SERVICE_EXISTS=$(curl -s "$KONG_ADMIN_URL/services/file-service" > /dev/null 2>&1 && echo "true" || echo "false")

if [ "$SERVICE_EXISTS" = "false" ]; then
  echo "Creating file-service..."
  curl -s -X POST "$KONG_ADMIN_URL/services" \
    -H "Content-Type: application/json" \
    -d '{
      "name": "file-service",
      "url": "http://file-service-api:8080"
    }'
  echo ""

  echo "Creating file-service route..."
  curl -s -X POST "$KONG_ADMIN_URL/services/file-service/routes" \
    -H "Content-Type: application/json" \
    -d '{
      "paths": ["/api/files"],
      "strip_path": true,
      "methods": ["GET", "POST", "PUT", "PATCH", "DELETE"]
    }'
  echo ""
fi

PROCESSING_EXISTS=$(curl -s "$KONG_ADMIN_URL/services/processing-service" > /dev/null 2>&1 && echo "true" || echo "false")

if [ "$PROCESSING_EXISTS" = "false" ]; then
  echo "Creating processing-service..."
  curl -s -X POST "$KONG_ADMIN_URL/services" \
    -H "Content-Type: application/json" \
    -d '{
      "name": "processing-service",
      "url": "http://processing-service-api:8080"
    }'
  echo ""

  echo "Creating processing-service route..."
  curl -s -X POST "$KONG_ADMIN_URL/services/processing-service/routes" \
    -H "Content-Type: application/json" \
    -d '{
      "paths": ["/api/processing"],
      "strip_path": true,
      "methods": ["GET", "POST", "PUT", "PATCH", "DELETE"]
    }'
  echo ""
else
  echo "Services already configured!"
fi

echo "Kong setup completed!"
