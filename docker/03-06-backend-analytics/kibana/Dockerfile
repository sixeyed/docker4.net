# escape=`
FROM mcr.microsoft.com/windows/nanoserver:1809 AS download
ARG KIBANA_VERSION="6.8.12"

RUN curl -o kibana.zip https://artifacts.elastic.co/downloads/kibana/kibana-oss-%KIBANA_VERSION%-windows-x86_64.zip
RUN md C:\kibana-%KIBANA_VERSION%-windows-x86_64 && `
    tar -xzf kibana.zip

WORKDIR /kibana-${KIBANA_VERSION}-windows-x86_64
RUN del /Q node\node.exe

# kibana - 6.8.12 requires node@ v10.21.0
FROM mcr.microsoft.com/windows/servercore:ltsc2019 as node
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

ARG NODE_VERSION="10.21.0"

RUN Write-Host "Downloading Node version: $env:NODE_VERSION"; `
    Invoke-WebRequest -OutFile node.zip "https://nodejs.org/dist/v$($env:NODE_VERSION)/node-v$($env:NODE_VERSION)-win-x64.zip"; `
    Expand-Archive node.zip -DestinationPath C:\ ; `
    Rename-Item -Path "C:\node-v$($env:NODE_VERSION)-win-x64" -NewName C:\nodejs

# kibana
FROM mcr.microsoft.com/windows/nanoserver:1809
ARG KIBANA_VERSION="6.8.12"

COPY --from=node /nodejs /nodejs

USER ContainerAdministrator
RUN setx /M PATH "%PATH%;C:\nodejs"

EXPOSE 5601
ENV KIBANA_HOME="/usr/share/kibana" 

WORKDIR /usr/share/kibana
COPY --from=download /kibana-${KIBANA_VERSION}-windows-x86_64/ .
COPY docker/03-06-backend-analytics/kibana/kibana.bat bin/
COPY docker/03-06-backend-analytics/kibana/kibana.yml config/

CMD ["bin\\kibana.bat"]