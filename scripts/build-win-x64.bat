@echo off

dotnet publish GuiStack.sln /property:GenerateFullPaths=true /consoleloggerparameters:NoSummary /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained false -c Release -r win-x64
