

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
COPY adminleftypro /app
WORKDIR /app
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Admin.LeftyPro.dll"]