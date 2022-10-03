# GuiStack
A web-based GUI for your AWS or LocalStack environment.

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

Optional:
- Visual Studio 2022
