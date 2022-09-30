# GuiStack
A web-based GUI for your AWS or LocalStack environment.

## System requirements
- ASP.NET Core Runtime for .NET 6.0 or newer ([Download](https://dotnet.microsoft.com/en-us/download/dotnet))
- Any operating system supported by the above runtime

## Configuration
GuiStack can be configured by setting various environment variables before starting the application. Most of them are provided in the `run.sh` or `run.bat` file included within the release.

| Variable name           | Default value (in `run.sh`) |
| ----------------------- | --------------------------- |
| AWS_ACCESS_KEY_ID       | localstack                  |
| AWS_SECRET_ACCESS_KEY   | localstack                  |
| AWS_REGION              | eu-central-1                |
| AWS_S3_ENDPOINT_URL     | http://localhost:4566       |
| AWS_SQS_ENDPOINT_URL    | http://localhost:4566       |
| AWS_S3_FORCE_PATH_STYLE | true                        |

## Building
Requires:
- .NET 6.0 SDK or newer ([Download](https://dotnet.microsoft.com/en-us/download/dotnet))

Optional:
- Visual Studio 2022