# BlazorGameQuest 

## Développeur 
- Lucas FARIA
- El Hadj SYLLA

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
Une fois lancé, vous pouvez ouvrir Swagger pour explorer l’API via : 
http://localhost:5154/index.html

### 2. Frontend (BlazorGame.Client)

```bash
cd BlazorGame.Client
dotnet build
dotnet run
```
Enfin, lancer l'application via : 
http://localhost:5133/

## Notes

- Assurez-vous que GameService est lancé avant de démarrer le frontend.

## Tests Unitaires à venir 

### 1. Vérifier si Joueur change de Salle quand Salle Reussit
### 2. Vérifier si Joueur ouvre Coffre alors réponse
### 3. Vérifier si lorsque Derniere Salle, Fin du Jeu
### 4. Vérifier si Joueur a Perdu lorsque Plus de Point de Vie
### 5. Vérifier si Salles non Nulles
### 6. Vérifier si Première Salle est vraiment la Première Salle
### 7. Vérifier si Point de Vie Perdu sont réellement Perdu
### 8. Vérifier si Potion Soigne Point de Vie


