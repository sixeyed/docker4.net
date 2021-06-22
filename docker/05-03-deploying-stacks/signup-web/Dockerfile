# escape=`
FROM mcr.microsoft.com/windows/nanoserver:1809 AS logmonitor
ARG LOGMONITOR_VERSION="v1.1"
ADD https://github.com/microsoft/windows-container-tools/releases/download/${LOGMONITOR_VERSION}/LogMonitor.exe .

FROM mcr.microsoft.com/windows/nanoserver:1809 AS servicemonitor
ARG SERVICEMONITOR_VERSION="2.0.1.10"
ADD https://dotnetbinaries.blob.core.windows.net/servicemonitor/${SERVICEMONITOR_VERSION}/ServiceMonitor.exe .

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS builder

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
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop';"]

ENV APP_ROOT=C:\web-app `
    LOGS_ROOT=C:\logs
    
WORKDIR ${LOGS_ROOT}
WORKDIR ${APP_ROOT}
RUN Import-Module WebAdministration; `
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value LocalSystem; `
    New-WebApplication -Name 'app' -Site 'Default Web Site' -PhysicalPath $env:APP_ROOT

HEALTHCHECK --start-period=30s --interval=30s `
 CMD powershell -command `
    try { `
     $response = Invoke-WebRequest http://localhost/app/SignUp -UseBasicParsing; `
     if ($response.StatusCode -eq 200) { return 0} `
     else {return 1}; `
    } catch { return 1 }

ENTRYPOINT ["powershell", "/start.ps1"]

COPY --from=logmonitor /LogMonitor.exe /LogMonitor.exe
COPY --from=servicemonitor /ServiceMonitor.exe /ServiceMonitor.exe
COPY --from=builder C:\out\_PublishedWebsites\SignUp.Web ${APP_ROOT} 

COPY docker\05-03-deploying-stacks\signup-web\start.ps1 C:\
COPY docker\05-03-deploying-stacks\signup-web\Web.config ${APP_ROOT}
COPY docker\05-03-deploying-stacks\signup-web\LogMonitorConfig.json C:\LogMonitor\

COPY docker\05-03-deploying-stacks\signup-web\config\connectionStrings.config ${APP_ROOT}\config\connectionStrings.config.default
COPY docker\05-03-deploying-stacks\signup-web\config\log4net.config ${APP_ROOT}\config\log4net.config.default