# DP-01: the API image. Built from the solution folder: docker build -t tasks-api .

# DP-01-R1: build with the SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project files first, so restore is cached until a dependency changes.
# DP-01-R2: Task.Tests is not part of the image.
COPY Task.Core/Task.Core.csproj Task.Core/
COPY Task.Data/Task.Data.csproj Task.Data/
COPY Task.Service/Task.Service.csproj Task.Service/
COPY Task.api/Task.api.csproj Task.api/
RUN dotnet restore Task.api/Task.api.csproj

COPY Task.Core/ Task.Core/
COPY Task.Data/ Task.Data/
COPY Task.Service/ Task.Service/
COPY Task.api/ Task.api/
RUN dotnet publish Task.api/Task.api.csproj -c Release -o /app --no-restore

# DP-01-R1: run with the ASP.NET runtime only
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# DP-01-R4: 8080 unless PORT is set (Render)
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "Task.api.dll"]
