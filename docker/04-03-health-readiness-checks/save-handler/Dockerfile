# escape=`

# container utilities
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as utilities

WORKDIR /src
COPY src/Utilities.MessageQueueCheck/Utilities.MessageQueueCheck.csproj ./Utilities.MessageQueueCheck/

WORKDIR /src/Utilities.MessageQueueCheck
RUN dotnet restore

COPY src /src
RUN dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -c Release -o /out Utilities.MessageQueueCheck.csproj

# app build
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
FROM mcr.microsoft.com/dotnet/framework/runtime:4.8

ENV APP_ROOT=C:\save-prospect-handler

WORKDIR ${APP_ROOT}
CMD .\Utilities.MessageQueueCheck.exe && .\SignUp.MessageHandlers.SaveProspect.exe

COPY --from=utilities C:\out\ .
COPY --from=builder C:\out\save-prospect\SaveProspectHandler .
COPY docker\04-03-health-readiness-checks\save-handler\App.config ${APP_ROOT}\SignUp.MessageHandlers.SaveProspect.exe.config
COPY docker\04-03-health-readiness-checks\save-handler\config\*.config ${APP_ROOT}\config\