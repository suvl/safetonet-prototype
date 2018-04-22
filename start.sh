#!/bin/bash

dotnet SafeToNet.Prototype.Api.dll &
serve ClientApp/build/ &

nginx -g 'daemon off;'