# BlazorGameQuest

BlazorGameQuest est un jeu d’aventure basé sur un système de donjons et de combats.

## Développeurs
- Lucas Faria
- El Hadj Sylla

## Prérequis

Avant de lancer le projet, assurez-vous d’avoir installé les éléments suivants :

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Un terminal ou PowerShell

## Choix d'architecture

L'application suit une architecture en microservices avec une séparation des responsabilités :

| Microservice / Projet                 | Rôle principal                                                                 |
| ------------------------------------- | ------------------------------------------------------------------------------ |
| **BlazorGame.Client (Frontend)**      | Application Blazor WebAssembly pour l'interface utilisateur                    |
| **BlazorGame.GameService (Backend)**  | API REST gérant la logique métier du jeu, les sessions, combats et persistance |
| **BlazorGame.AuthenticationServices** | Service dédié à l'authentification des utilisateurs et de l'admin              |
| **BlazorGame.SharedModels**           | Bibliothèque partagée contenant modèles, DTOs et constantes communes           |
| **BlazorGame.Tests**                  | Projet de tests unitaires                                                      |

**Technologies principales :**

- Entity Framework Core avec base de données en mémoire pour la persistance
- CORS configuré pour la communication entre le client et l'API
- Swagger pour la documentation de l'API

### Pourquoi cette architecture ?

Cette architecture sépare clairement le frontend et le backend :

- Le client Blazor gère uniquement l’interface utilisateur.
- Le backend centralise toute la logique du jeu.
- L’authentification est isolée pour sécuriser les rôles et les accès.

## Pages

| Page               | Route                          | Description                                                           |
| ------------------ | ------------------------------ | --------------------------------------------------------------------- |
| **HomePage**       | `/home`                        | Démarrage d’une nouvelle aventure ou reprise d’une partie sauvegardée |
| **LoginPage**      | `/login`                       | Page de connexion avec authentification utilisateur                   |
| **AdventurePage**  | `/new-adventure`               | Configuration d'une nouvelle partie (difficulté et nombre de salles)  |
| **RoomPage**       | `/new-adventure/room/{roomId}` | Salle actuelle, actions disponibles, état du joueur, combats, coffres |
| **SavedGamesPage** | `/saved-games`                 | Liste des parties sauvegardées avec options de reprise ou suppression |
| **GameOverPage**   | `/game-over`                   | Page affichée en cas de défaite avec le score final                   |
| **VictoryPage**    | `/victory`                     | Page affichée lorsque le donjon est complété                          |
| **ScoresPage**     | `/scores`                      | Tableau des scores de tous les joueurs                                |
| **HistoryPage**    | `/personal-history`            | Historique personnel des parties du joueur connecté                   |
| **RulesGame**      | `/rules`                       | Présentation des règles et mécaniques du jeu                          |

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

## Notes

- Assurez-vous que GameService est lancé avant de démarrer le frontend.
- Si un port est déjà utilisé, vous pouvez le modifier dans le fichier launchSettings.json.
