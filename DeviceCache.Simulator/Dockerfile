FROM microsoft/dotnet:2.0.0-sdk as build
ADD DeviceCache.Simulator.csproj /sources/DeviceCache.Simulator.csproj
WORKDIR /sources
RUN dotnet restore
COPY . /sources
RUN dotnet publish -c release --output publish/DeviceCache.Simulator

FROM microsoft/dotnet:2.0.0-runtime
WORKDIR /app
COPY --from=build sources/publish/DeviceCache.Simulator /app
ENTRYPOINT ["dotnet", "DeviceCache.Simulator.dll"]