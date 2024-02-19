@echo off

set AWS_ACCESS_KEY_ID=localstack
set AWS_SECRET_ACCESS_KEY=localstack
set AWS_REGION=eu-central-1
set AWS_DYNAMODB_ENDPOINT_URL=http://localhost:4566
set AWS_S3_ENDPOINT_URL=http://localhost:4566
set AWS_SNS_ENDPOINT_URL=http://localhost:4566
set AWS_SQS_ENDPOINT_URL=http://localhost:4566
set AWS_S3_FORCE_PATH_STYLE=true

.\GuiStack.exe
