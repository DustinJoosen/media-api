FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln ./

COPY Media.Core/Media.Core.csproj Media.Core/
COPY Media.Infrastructure/Media.Infrastructure.csproj Media.Infrastructure/
COPY Media.Persistence/Media.Persistence.csproj Media.Persistence/
COPY Media.Abstractions/Media.Abstractions.csproj Media.Abstractions/
COPY Media.Presentation/Media.Presentation.csproj Media.Presentation/

RUN dotnet restore Media.Presentation/Media.Presentation.csproj
COPY . .

RUN dotnet publish Media.Presentation/Media.Presentation.csproj \
    -c Release \
    -o /app/publish \
    -p:NoWarn=1591

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:9001
EXPOSE 9001

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Media.Presentation.dll"]
