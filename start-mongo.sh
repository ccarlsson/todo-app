#!/usr/bin/env bash
set -euo pipefail

CONTAINER_NAME="todo-mongo"
IMAGE="mongo:7.0"
PORT="27017"
VOLUME="todo-mongo-data"

if docker ps -a --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
  if docker ps --format '{{.Names}}' | grep -q "^${CONTAINER_NAME}$"; then
    echo "${CONTAINER_NAME} is already running."
  else
    echo "Starting existing container ${CONTAINER_NAME}..."
    docker start "${CONTAINER_NAME}" > /dev/null
  fi
else
  echo "Creating and starting ${CONTAINER_NAME}..."
  docker run -d \
    --name "${CONTAINER_NAME}" \
    -p "${PORT}:27017" \
    -v "${VOLUME}:/data/db" \
    "${IMAGE}" > /dev/null
fi

echo "MongoDB is available on mongodb://localhost:${PORT}"
