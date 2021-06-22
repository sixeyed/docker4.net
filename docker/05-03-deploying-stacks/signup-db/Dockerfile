# LogMonitor - https://github.com/microsoft/windows-container-tools/tree/master/LogMonitor
FROM mcr.microsoft.com/windows/nanoserver:1809 AS logmonitor
ARG LOGMONITOR_VERSION="v1.1"
ADD https://github.com/microsoft/windows-container-tools/releases/download/${LOGMONITOR_VERSION}/LogMonitor.exe .

# ServiceMonitor - https://github.com/microsoft/IIS.ServiceMonitor
FROM mcr.microsoft.com/windows/nanoserver:1809 AS servicemonitor
ARG SERVICEMONITOR_VERSION="2.0.1.10"
ADD https://dotnetbinaries.blob.core.windows.net/servicemonitor/${SERVICEMONITOR_VERSION}/ServiceMonitor.exe .

# sql server - the base image is built for Windows Server 2019
FROM docker4dotnet/sql-server:2017-ltsc2019

ENV SA_PASSWORD="docker4.net!" \
    DATA_FOLDER="C:\\data"

COPY --from=logmonitor /LogMonitor.exe /LogMonitor.exe
COPY --from=servicemonitor /ServiceMonitor.exe /ServiceMonitor.exe

COPY docker/04-07-persistent-databases/signup-db/start.ps1 /
COPY docker/04-07-persistent-databases/signup-db/LogMonitorConfig.json /LogMonitor/
