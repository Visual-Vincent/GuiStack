#!/bin/bash

if [ -z "$1" ]; then
    echo "No runtime identifier specified"
    exit 1
fi

# To ensure Git Bash/MinGW doesn't try to interpret and convert slashes into Windows paths
export MSYS_NO_PATHCONV=1

dotnet publish GuiStack.sln /consoleloggerparameters:NoSummary /property:GenerateFullPaths=true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true --self-contained false -c Release -r "$1"
