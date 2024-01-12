#!/bin/bash

export AWS_ACCESS_KEY_ID=localstack
export AWS_SECRET_ACCESS_KEY=localstack
export AWS_REGION=eu-central-1
export AWS_S3_ENDPOINT_URL=http://localhost:4566
export AWS_SNS_ENDPOINT_URL=http://localhost:4566
export AWS_SQS_ENDPOINT_URL=http://localhost:4566
export AWS_S3_FORCE_PATH_STYLE=true

./GuiStack
