#!/bin/bash

PROJECT=FilConv/FilConv.csproj
VERSION=$(cat $PROJECT | grep '<AssemblyVersion>' | sed -E 's/^.*>(.*?)<.*/\1/')

pub() {
  dotnet publish $PROJECT \
    --configuration Release \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -p:PublishSingleFile=true \
    "$@"
}

echo "AssemblyVersion $VERSION"

pub --runtime linux-x64 --self-contained --output FilConv-linux-$VERSION
pub --runtime linux-x64 --no-self-contained --output FilConv-linux-net8-$VERSION
pub --runtime win-x64 --self-contained --output FilConv-win-$VERSION
pub --runtime win-x64 --no-self-contained --output FilConv-win-net8-$VERSION

for d in FilConv-*-$VERSION; do
    7za a -tzip $d.zip $d
done
