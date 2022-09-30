#!/bin/bash

echo "Building GuiStack for Windows..."
./build-windows.sh

echo "Building GuiStack for Linux..."
./build-linux.sh

echo "Building GuiStack for OS X..."
./build-osx.sh
