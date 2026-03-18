FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire repo
COPY . .

# Restore using solution
WORKDIR /src
RUN dotnet restore

# Publish Task API
WORKDIR /src/Services/TaskService/Task.API
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskService.API.dll"]