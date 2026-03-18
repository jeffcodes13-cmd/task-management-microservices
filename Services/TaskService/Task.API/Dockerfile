FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire repo
COPY . .

# Restore using solution
RUN dotnet restore Services/TaskService/TaskService.sln

# Publish Task API (IMPORTANT PATH)
RUN dotnet publish Services/TaskService/Task.API/TaskService.API.csproj -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskService.API.dll"]