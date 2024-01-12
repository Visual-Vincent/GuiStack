# GuiStack
[![GitHub Downloads](https://img.shields.io/github/downloads/Visual-Vincent/GuiStack/total)](https://github.com/Visual-Vincent/GuiStack/releases)
[![Docker Pulls](https://img.shields.io/docker/pulls/visualvincent/guistack)](https://hub.docker.com/r/visualvincent/guistack)
[![License](https://img.shields.io/github/license/Visual-Vincent/GuiStack?color=green)](/LICENSE.txt)

A web-based GUI for your AWS or LocalStack environment.

## Features

Contributions and feature requests are welcome!

- **AWS S3**
  - Create, view and delete buckets
  - View, delete, upload and download objects

- **AWS SQS**
  - Create, view and delete queues
  - Send, receive and peek messages
  - Import Protobuf definitions and send Protobuf-formatted messages

- **AWS SNS**
  - Create, view and delete topics
  - Create and delete subscriptions

Also see the [checklist of upcoming features](https://github.com/Visual-Vincent/GuiStack/issues/1).

## Download

Download precompiled executables from [Releases](https://github.com/Visual-Vincent/GuiStack/releases), or pull the latest [Docker image](https://hub.docker.com/r/visualvincent/guistack).

## System requirements
- ASP.NET Core Runtime for .NET 6.0 or newer ([Download](https://dotnet.microsoft.com/en-us/download/dotnet))
- Any operating system supported by the above runtime

## Running
### On Windows
To run GuiStack, execute the `run.bat` script included within the release.

### On Unix-like operating systems (Linux, macOS)
Prior to running GuiStack, you have to give the GuiStack executable and the accompanying `run.sh` script _execute_ permissions:

```sh
$ chmod +x ./GuiStack ./run.sh
```

Then you should be able to execute `run.sh` to start GuiStack.

### Through Docker
Download the `scripts/default-run-docker.sh` script from the repository and execute it to start the Docker container.

## Configuration
GuiStack can be configured by setting various environment variables before starting the application. Most of them are provided in the `run.sh` or `run.bat` file included within the release (or in `default-run-docker.sh`, if using the Docker image).

| Variable name           | Default value (in `run.sh`) |
| ----------------------- | --------------------------- |
| AWS_ACCESS_KEY_ID       | localstack                  |
| AWS_SECRET_ACCESS_KEY   | localstack                  |
| AWS_REGION              | eu-central-1                |
| AWS_S3_ENDPOINT_URL     | http://localhost:4566       |
| AWS_SNS_ENDPOINT_URL    | http://localhost:4566       |
| AWS_SQS_ENDPOINT_URL    | http://localhost:4566       |
| AWS_S3_FORCE_PATH_STYLE | true                        |

### Listening ports (standalone release)
By default, GuiStack can be accessed via https://localhost:5001. To configure which port GuiStack should listen to, edit the accompanying `appsettings.json` file and add a property called `"Urls"`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Urls": "https://localhost:5005"
}
```

### Listening ports (Docker)
In `default-run-docker.sh`, the Docker container is configured to map the internal listening port (port 80) to http://localhost:5000. To change the port, modify the following line in `default-run-docker.sh`:

```sh
docker run -p 5000:80 \
```

## Building
Requires:
- .NET 6.0 SDK or newer ([Download](https://dotnet.microsoft.com/en-us/download/dotnet))
- Git Bash (if building on Windows) ([Download](https://gitforwindows.org/))

Optional:
- Visual Studio 2022

To build GuiStack, simply execute one of the `build-*.sh` scripts for the platform you wish to build for, located in the root of this repository.
