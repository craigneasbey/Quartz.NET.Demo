# Quartz.NET.Demo

## Reference
https://www.quartz-scheduler.net/

## Requirements
.NET Core 3.1 SDK

## Development

### Setup
dotnet new web
dotnet add ./Quartz.NET.Demo.csproj package Quartz.AspNetCore -v 3.3.3 -s https://api.nuget.org/v3/index.json
dotnet add ./Quartz.NET.Demo.csproj package Quartz.Extensions.DependencyInjection -v 3.3.3 -s https://api.nuget.org/v3/index.json

### Build
dotnet build

### Run
dotnet run