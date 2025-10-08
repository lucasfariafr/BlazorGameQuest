# BlazorGameQuest

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) installé
- Un terminal ou PowerShell

## Lancer le projet

### 1. Backend (GameService)

```bash
cd GameServices
dotnet build
dotnet run
```
Une fois lancé, vous pouvez ouvrir Swagger pour explorer l’API via : http://localhost:5154/index.html

### 2. Frontend (BlazorGame.Client)

```bash
cd BlazorGame.Client
dotnet build
dotnet run
```
Enfin, lancer l'application via : http://localhost:5133/

## Notes

- Assurez-vous que GameService est lancé avant de démarrer le frontend.