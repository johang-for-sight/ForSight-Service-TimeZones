FROM mcr.microsoft.com/dotnet/sdk:6.0-focal as build
EXPOSE 80
EXPOSE 443

WORKDIR /app

COPY ForSight.TimeZonesService.API/*.csproj ./ForSight.TimeZonesService.API/
COPY ForSight.TimeZonesService.Data/*.csproj ./ForSight.TimeZonesService.Data/
COPY ForSight.TimeZonesService.Handlers/*.csproj ./ForSight.TimeZonesService.Handlers/
COPY ForSight.TimeZonesService.Core/*.csproj ./ForSight.TimeZonesService.Core/

ARG PRIVATE_NUGET_SOURCE_URL
ARG PRIVATE_NUGET_PASSWD
RUN dotnet nuget add source $PRIVATE_NUGET_SOURCE_URL --name ForthCom --username forthcomadmin --password $PRIVATE_NUGET_PASSWD --store-password-in-clear-text
RUN dotnet restore "ForSight.TimeZonesService.API/ForSight.TimeZonesService.API.csproj"

COPY ForSight.TimeZonesService.API/. ./ForSight.TimeZonesService.API/
COPY ForSight.TimeZonesService.Data/. ./ForSight.TimeZonesService.Data/
COPY ForSight.TimeZonesService.Handlers/. ./ForSight.TimeZonesService.Handlers/
COPY ForSight.TimeZonesService.Core/. ./ForSight.TimeZonesService.Core/

RUN dotnet publish "ForSight.TimeZonesService.API/ForSight.TimeZonesService.API.csproj" -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /app

ARG BUILD_DEV_CERT
COPY DockerDummyFile.txt mkcert-v144-linux-arm64* /usr/local/bin/
COPY DockerDummyFile.txt rootCA\*.pem /root/.local/share/mkcert/
RUN if [ "$BUILD_DEV_CERT" = "True" ] ; then \
    chmod +x /usr/local/bin/mkcert-v144-linux-arm64 \
    && mkcert-v144-linux-arm64 -install \
    && rm /root/.local/share/mkcert/rootCA*.pem \
    && rm -rf /usr/local/bin/mkcert-v144-linux-arm64* ; \
    fi

RUN rm /usr/local/bin/DockerDummyFile.txt \
    && rm /root/.local/share/mkcert/DockerDummyFile.txt

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ForSight.TimeZonesService.API.dll"]
