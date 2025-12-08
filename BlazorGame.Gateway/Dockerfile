FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ../BlazorGame.Gateway/BlazorGame.Gateway.csproj ./BlazorGame.Gateway/
RUN dotnet restore ./BlazorGame.Gateway

COPY ./BlazorGame.Gateway/ ./BlazorGame.Gateway/
RUN dotnet publish ./BlazorGame.Gateway -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "BlazorGame.Gateway.dll"]
