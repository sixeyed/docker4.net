FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

WORKDIR /src
COPY src/SignUp.Entities/SignUp.Entities.csproj ./SignUp.Entities/
COPY src/SignUp.Api.ReferenceData/SignUp.Api.ReferenceData.csproj ./SignUp.Api.ReferenceData/

WORKDIR /src/SignUp.Api.ReferenceData
RUN dotnet restore

COPY src /src
RUN dotnet publish -c Release -o /out SignUp.Api.ReferenceData.csproj

# app image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

EXPOSE 80
WORKDIR /app
ENTRYPOINT ["dotnet", "/app/SignUp.Api.ReferenceData.dll"]

COPY --from=builder /out/ .
COPY ./docker/02-05-packaging-dotnet-apps/reference-data-api/appsettings.json .