#!/bin/bash

DOCKER_REPOSITORY="$2"

if [ ! -z "$DOCKER_REPOSITORY" ]; then
    DOCKER_REPOSITORY="$DOCKER_REPOSITORY/"
fi

DOCKER_BUILD_TAG="${1:-latest}"
DOCKER_IMAGE_NAME="${DOCKER_REPOSITORY}guistack:${DOCKER_BUILD_TAG}"

docker build -t "$DOCKER_IMAGE_NAME" . && echo "Docker image built successfully: $DOCKER_IMAGE_NAME"
