FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ../BlazorGame.SharedModels/BlazorGame.SharedModels.csproj ./BlazorGame.SharedModels/
RUN dotnet restore ./BlazorGame.SharedModels

COPY ../BlazorGame.GameService/BlazorGame.GameService.csproj ./BlazorGame.GameService/
RUN dotnet restore ./BlazorGame.GameService

COPY ./BlazorGame.SharedModels/ ./BlazorGame.SharedModels/
COPY ./BlazorGame.GameService/ ./BlazorGame.GameService/
RUN dotnet publish ./BlazorGame.GameService -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5203

ENTRYPOINT ["dotnet", "BlazorGame.GameService.dll"]
