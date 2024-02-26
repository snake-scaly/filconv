#!/bin/bash

PROJECT=FilConvWpf/FilConvWpf.csproj
VERSION=$(cat FilConvWpf/Properties/AssemblyInfo.cs | sed 's@//.*@@' | grep '\<AssemblyVersion\>' | sed -E 's/^.*\("(.*?)"\).*/\1/')
COMMFLAGS="--configuration Release -p:DebugType=None -p:DebugSymbols=false"

echo "AssemblyVersion $VERSION"

dotnet restore $PROJECT
dotnet build $COMMFLAGS $PROJECT --no-restore
dotnet publish $COMMFLAGS $PROJECT --no-build --output FilConv-$VERSION
