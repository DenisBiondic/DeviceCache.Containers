FROM microsoft/dotnet:1.1.2-sdk as build
ADD DeviceCache.Simulator.csproj /sources/DeviceCache.Simulator.csproj
WORKDIR /sources
RUN dotnet restore
COPY . /sources
RUN dotnet publish -c release --output publish/DeviceCache.Simulator

FROM microsoft/dotnet:1.1-runtime
WORKDIR /app
COPY --from=build sources/publish/DeviceCache.Simulator /app
ENTRYPOINT ["dotnet", "DeviceCache.Simulator.dll"]