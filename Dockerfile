FROM mcr.microsoft.com/dotnet/sdk:10.0 AS restore
WORKDIR /app

COPY Directory.Build.props .

COPY src/CompanyName.ProjectName.Domain/CompanyName.ProjectName.Domain.csproj \
     src/CompanyName.ProjectName.Domain/
COPY src/CompanyName.ProjectName.Application/CompanyName.ProjectName.Application.csproj \
     src/CompanyName.ProjectName.Application/
COPY src/CompanyName.ProjectName.Infrastructure/CompanyName.ProjectName.Infrastructure.csproj \
     src/CompanyName.ProjectName.Infrastructure/
COPY src/CompanyName.ProjectName.Api/CompanyName.ProjectName.Api.csproj \
     src/CompanyName.ProjectName.Api/

RUN dotnet restore src/CompanyName.ProjectName.Api/CompanyName.ProjectName.Api.csproj

FROM restore AS publish
COPY src/ src/
RUN dotnet publish src/CompanyName.ProjectName.Api/CompanyName.ProjectName.Api.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "CompanyName.ProjectName.Api.dll"]
