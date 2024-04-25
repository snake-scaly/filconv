#!/bin/bash

PROJECT=FilConv/FilConv.csproj
VERSION=$(cat $PROJECT | grep '<AssemblyVersion>' | sed -E 's/^.*>(.*?)<.*/\1/')

pub() {
  dotnet publish $PROJECT \
    --configuration Release \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    "$@"
}

echo "AssemblyVersion $VERSION"

pub --runtime linux-x64 --output FilConv-linux-$VERSION
pub --runtime win-x64 --output FilConv-win-$VERSION
