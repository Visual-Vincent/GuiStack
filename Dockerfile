FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

COPY *.* .
COPY GuiStack/*.csproj ./GuiStack/
RUN dotnet restore

COPY GuiStack/. ./GuiStack/
WORKDIR /source/GuiStack
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "GuiStack.dll"]
