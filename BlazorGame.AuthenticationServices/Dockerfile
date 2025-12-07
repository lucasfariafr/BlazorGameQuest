FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ../BlazorGame.AuthenticationServices/BlazorGame.AuthenticationServices.csproj ./BlazorGame.AuthenticationServices/
RUN dotnet restore ./BlazorGame.AuthenticationServices

COPY ./BlazorGame.AuthenticationServices/ ./BlazorGame.AuthenticationServices/
RUN dotnet publish ./BlazorGame.AuthenticationServices -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

EXPOSE 5051

ENTRYPOINT ["dotnet", "BlazorGame.AuthenticationServices.dll"]
