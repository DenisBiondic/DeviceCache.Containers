FROM microsoft/aspnetcore-build:2.0.2 as build
COPY DeviceCache.Frontend.csproj /sources/DeviceCache.Frontend.csproj
WORKDIR /sources
RUN dotnet restore
COPY . /sources
RUN dotnet publish -c release --output publish/DeviceCache.Frontend

FROM microsoft/aspnetcore:2.0.0
WORKDIR /app
EXPOSE 80
COPY --from=build sources/publish/DeviceCache.Frontend /app
ENTRYPOINT ["dotnet", "DeviceCache.Frontend.dll"]