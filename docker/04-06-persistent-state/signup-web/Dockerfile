# escape=`
FROM mcr.microsoft.com/windows/nanoserver:1809 AS logmonitor
ARG LOGMONITOR_VERSION="v1.1"
ADD https://github.com/microsoft/windows-container-tools/releases/download/${LOGMONITOR_VERSION}/LogMonitor.exe .

FROM mcr.microsoft.com/windows/nanoserver:1809 AS servicemonitor
ARG SERVICEMONITOR_VERSION="2.0.1.10"
ADD https://dotnetbinaries.blob.core.windows.net/servicemonitor/${SERVICEMONITOR_VERSION}/ServiceMonitor.exe .

# app
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS builder

WORKDIR C:\src
COPY src\SignUp.Web.sln .
COPY src\SignUp.Core\SignUp.Core.csproj .\SignUp.Core\
COPY src\SignUp.Entities\SignUp.Entities.csproj .\SignUp.Entities\
COPY src\SignUp.Messaging\SignUp.Messaging.csproj .\SignUp.Messaging\
COPY src\SignUp.Model\SignUp.Model.csproj .\SignUp.Model\
COPY src\SignUp.Web\SignUp.Web.csproj .\SignUp.Web\
COPY src\SignUp.Model\packages.config .\SignUp.Model\
COPY src\SignUp.Web\packages.config .\SignUp.Web\
RUN nuget restore SignUp.Web.sln

COPY src C:\src
RUN msbuild SignUp.Web\SignUp.Web.csproj /p:OutputPath=c:\out /p:Configuration=Release

# app image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]

ENV APP_ROOT=C:\web-app `
    LOGS_ROOT=C:\logs
    
ENTRYPOINT C:\LogMonitor.exe C:\ServiceMonitor.exe w3svc

WORKDIR ${LOGS_ROOT}
WORKDIR ${APP_ROOT}
RUN New-WebApplication -Name 'app' -Site 'Default Web Site' -PhysicalPath $env:APP_ROOT

HEALTHCHECK --interval=5s `
 CMD powershell -command `
    try { `
     $response = Invoke-WebRequest http://localhost/app/SignUp -UseBasicParsing; `
     if ($response.StatusCode -eq 200) { return 0} `
     else {return 1}; `
    } catch { return 1 }

VOLUME C:\logs

COPY --from=logmonitor /LogMonitor.exe /LogMonitor.exe
COPY --from=servicemonitor /ServiceMonitor.exe /ServiceMonitor.exe
COPY --from=builder C:\out\_PublishedWebsites\SignUp.Web ${APP_ROOT} 

COPY docker\04-06-persistent-state\signup-web\Web.config ${APP_ROOT}
COPY docker\04-06-persistent-state\signup-web\config\*.config ${APP_ROOT}\config\
COPY docker\04-06-persistent-state\signup-web\LogMonitorConfig.json C:\LogMonitor\