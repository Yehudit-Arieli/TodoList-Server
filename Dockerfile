FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TodoApi.csproj", "."]
RUN dotnet restore "./TodoApi.csproj"
COPY . .
RUN dotnet publish "./TodoApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TodoApi.dll"]
