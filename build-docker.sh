#!/bin/bash

DOCKER_BUILD_TAG="${1:-latest}"
DOCKER_IMAGE_NAME=guistack:${DOCKER_BUILD_TAG}

docker build -t $DOCKER_IMAGE_NAME . && echo "Docker image built successfully: $DOCKER_IMAGE_NAME"
