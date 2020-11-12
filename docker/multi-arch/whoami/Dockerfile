ARG SDK_TAG=3.1
ARG ASPNET_TAG=3.1
FROM mcr.microsoft.com/dotnet/core/sdk:$SDK_TAG as builder

WORKDIR /src
COPY src/whoami.csproj .
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out whoami.csproj

# app image
FROM mcr.microsoft.com/dotnet/core/aspnet:$ASPNET_TAG

EXPOSE 80

WORKDIR /app
ENTRYPOINT ["dotnet", "whoami.dll"]

COPY --from=builder /out/ .