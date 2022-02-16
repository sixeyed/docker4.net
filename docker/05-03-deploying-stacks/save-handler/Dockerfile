# escape=`

# container utilities
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1809 as utilities

WORKDIR /src
COPY src/Utilities.MessageQueueCheck/Utilities.MessageQueueCheck.csproj ./Utilities.MessageQueueCheck/

WORKDIR /src/Utilities.MessageQueueCheck
RUN dotnet restore

COPY src /src
RUN dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -c Release -o /out Utilities.MessageQueueCheck.csproj

FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS builder

WORKDIR C:\src
COPY src\SignUp.sln .
COPY src\SignUp.Core\SignUp.Core.csproj .\SignUp.Core\
COPY src\SignUp.Entities\SignUp.Entities.csproj .\SignUp.Entities\
COPY src\SignUp.Messaging\SignUp.Messaging.csproj .\SignUp.Messaging\
COPY src\SignUp.Model\SignUp.Model.csproj .\SignUp.Model\
COPY src\SignUp.Model\packages.config .\SignUp.Model\
COPY src\SignUp.MessageHandlers.SaveProspect\SignUp.MessageHandlers.SaveProspect.csproj .\SignUp.MessageHandlers.SaveProspect\
COPY src\SignUp.MessageHandlers.SaveProspect\packages.config .\SignUp.MessageHandlers.SaveProspect\
RUN nuget restore .\SignUp.sln

COPY src C:\src
RUN msbuild SignUp.MessageHandlers.SaveProspect\SignUp.MessageHandlers.SaveProspect.csproj /p:OutputPath=c:\out\save-prospect\SaveProspectHandler

# app image
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8-windowsservercore-ltsc2019

ENV APP_ROOT=C:\save-prospect-handler

WORKDIR ${APP_ROOT}
CMD ["powershell", "/start.ps1"]

COPY --from=utilities C:\out\ .
COPY --from=builder C:\out\save-prospect\SaveProspectHandler .

COPY docker\05-03-deploying-stacks\save-handler\start.ps1 C:\
COPY docker\05-03-deploying-stacks\save-handler\App.config ${APP_ROOT}\SignUp.MessageHandlers.SaveProspect.exe.config
COPY docker\05-03-deploying-stacks\save-handler\config\connectionStrings.config ${APP_ROOT}\config\connectionStrings.config.default