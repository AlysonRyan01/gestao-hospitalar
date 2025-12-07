FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dev
WORKDIR /app

COPY ["src/gestao-hospitalar.Api/gestao-hospitalar.Api.csproj", "src/gestao-hospitalar.Api/"]
COPY ["src/gestao-hospitalar.Infrastructure/gestao-hospitalar.Infrastructure.csproj", "src/gestao-hospitalar.Infrastructure/"]
COPY ["src/gestao-hospitalar.Domain/gestao-hospitalar.Domain.csproj", "src/gestao-hospitalar.Domain/"]
COPY ["src/gestao-hospitalar.Shared/gestao-hospitalar.Shared.csproj", "src/gestao-hospitalar.Shared/"]
COPY ["src/gestao-hospitalar.Application/gestao-hospitalar.Application.csproj", "src/gestao-hospitalar.Application/"]

RUN dotnet restore "src/gestao-hospitalar.Api/gestao-hospitalar.Api.csproj"

COPY . .

WORKDIR "/app/src/gestao-hospitalar.Api"

RUN dotnet build "gestao-hospitalar.Api.csproj" -c Debug

EXPOSE 8080

ENTRYPOINT ["dotnet", "run", "--urls", "http://0.0.0.0:8080"]
