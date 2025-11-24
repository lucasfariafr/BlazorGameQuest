# BlazorGameQuest

## Développeurs
- Lucas Faria
- El Hadj Sylla

## Prérequis

Avant de lancer le projet, assurez-vous d’avoir installé les éléments suivants :

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Un terminal ou PowerShell

## Choix d'architecture

L'application suit une architecture en microservices avec une séparation des responsabilités :

- **BlazorGame.Client** : Application Blazor WebAssembly pour l'interface utilisateur
- **BlazorGame.GameService** : API REST (ASP.NET Core) gérant la logique métier du jeu, les sessions, les combats et la persistance
- **BlazorGame.AuthenticationServices** : Service dédié à l'authentification des utilisateurs et de l'admin
- **BlazorGame.SharedModels** : Bibliothèque partagée contenant les modèles, DTOs et constantes communes
- **BlazorGame.Tests** : Projet de tests unitaires

**Technologies principales** :
- Entity Framework Core avec base de données en mémoire pour la persistance
- Injection de dépendances pour la gestion des services
- CORS configuré pour la communication entre le client et l'API
- Swagger pour la documentation de l'API

## Pages

- **HomePage** (`/home`) : Page d'accueil permettant de démarrer une nouvelle aventure ou de reprendre une partie sauvegardée
- **LoginPage** (`/login`) : Page de connexion avec authentification utilisateur
- **AdventurePage** (`/new-adventure`) : Configuration d'une nouvelle partie (choix de la difficulté et du nombre de salles)
- **RoomPage** (`/new-adventure/room/{roomId}`) : Page principale du jeu affichant la salle actuelle, les actions disponibles, l'état du joueur et gérant les interactions (combats, coffres, événements)
- **SavedGamesPage** (`/saved-games`) : Liste des parties sauvegardées avec possibilité de reprendre ou supprimer une partie
- **GameOverPage** (`/game-over`) : Page affichée en cas de défaite avec le score final
- **VictoryPage** (`/victory`) : Page de victoire affichée lorsque le donjon est complété
- **ScoresPage** (`/scores`) : Tableau des scores de tous les joueurs
- **HistoryPage** (`/personal-history`) : Historique personnel des parties du joueur connecté
- **RulesGame** (`/rules`) : Page présentant les règles du jeu et les mécaniques

## Lancer le projet

### 1. Lancer le service principal (API)

Dans un premier terminal, exécutez :

```bash
cd GameServices
dotnet build
dotnet run
```
Une fois le service lancé, ouvrez Swagger pour explorer l’API : http://localhost:5203/index.html

### 2. Lancer le client Blazor
Dans un second terminal, exécutez :

```bash
cd BlazorGame.Client
dotnet build
dotnet run
```
Ensuite, ouvrez l’application à l’adresse suivante : http://localhost:5000/

### 3. Lancer le test de couverture
Dans un terminal, exécutez :

```bash
cd BlazorGame.Tests
dotnet dotnet test --collect:"XPlat Code Coverage"
```

## Notes
- Assurez-vous que GameService est lancé avant de démarrer le frontend.
- Si un port est déjà utilisé, vous pouvez le modifier dans le fichier launchSettings.json.
