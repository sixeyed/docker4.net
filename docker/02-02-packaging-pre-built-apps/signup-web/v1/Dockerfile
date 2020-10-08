FROM docker4dotnet/aspnet:4.8
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

COPY SignUp-1.0.msi /
RUN Start-Process msiexec.exe -ArgumentList '/i', 'C:\SignUp-1.0.msi', '/quiet', '/norestart' -NoNewWindow -Wait