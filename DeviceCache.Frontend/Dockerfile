FROM microsoft/aspnetcore-build:1.1.2 as build
WORKDIR /sources
COPY . /sources
RUN dotnet restore
RUN dotnet publish -c release --output publish/DeviceCache.Frontend

FROM microsoft/aspnetcore:1.1.2
WORKDIR /app
EXPOSE 80
COPY --from=build sources/publish/DeviceCache.Frontend /app
ENTRYPOINT ["dotnet", "DeviceCache.Frontend.dll"]