FROM microsoft/dotnet:1.1.2-sdk as build
ADD DeviceCache.Processor.csproj /sources/DeviceCache.Processor.csproj
WORKDIR /sources
RUN dotnet restore
COPY . /sources
RUN dotnet publish -c release --output publish/DeviceCache.Processor

FROM microsoft/dotnet:1.1-runtime
WORKDIR /app
COPY --from=build sources/publish/DeviceCache.Processor /app
ENTRYPOINT ["dotnet", "DeviceCache.Processor.dll"]