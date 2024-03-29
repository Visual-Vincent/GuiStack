#!/bin/bash

export AWS_ACCESS_KEY_ID=localstack
export AWS_SECRET_ACCESS_KEY=localstack
export AWS_REGION=eu-central-1
export AWS_DYNAMODB_ENDPOINT_URL=http://host.docker.internal:4566
export AWS_S3_ENDPOINT_URL=http://host.docker.internal:4566
export AWS_SNS_ENDPOINT_URL=http://host.docker.internal:4566
export AWS_SQS_ENDPOINT_URL=http://host.docker.internal:4566
export AWS_S3_FORCE_PATH_STYLE=true

docker run -p 5000:80 \
    -e AWS_ACCESS_KEY_ID \
    -e AWS_SECRET_ACCESS_KEY \
    -e AWS_REGION \
    -e AWS_DYNAMODB_ENDPOINT_URL \
    -e AWS_S3_ENDPOINT_URL \
    -e AWS_SNS_ENDPOINT_URL \
    -e AWS_SQS_ENDPOINT_URL \
    -e AWS_S3_FORCE_PATH_STYLE \
  --add-host=host.docker.internal:host-gateway \
  --name guistack -d visualvincent/guistack:latest
