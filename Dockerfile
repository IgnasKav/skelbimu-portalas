FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./API/API.csproj"
RUN dotnet publish "./API/API.csproj" -c release -o /app --no-restore

#Serve
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./
COPY ./API/images/ ./images/
EXPOSE 5001
ENTRYPOINT ["dotnet", "API.dll"]
