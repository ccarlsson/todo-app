FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY backend/TodoApp.slnx ./
COPY backend/src ./src
COPY backend/tests ./tests

RUN dotnet restore ./TodoApp.slnx
RUN dotnet publish ./src/API/TodoApp.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoApp.Api.dll"]
