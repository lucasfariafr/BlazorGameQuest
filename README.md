# BlazorGameQuest

## Développeurs
- Lucas Faria
- El Hadj Sylla

## Prérequis

Avant de lancer le projet, assurez-vous d’avoir installé les éléments suivants :

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Un terminal ou PowerShell

## Lancer le projet

### 1. Lancer le service principal (API)

Dans un premier terminal, exécutez :

```bash
cd GameServices
dotnet build
dotnet run
```
Une fois le service lancé, ouvrez Swagger pour explorer l’API : http://localhost:5154/index.html

### 2. Lancer le client Blazor
Dans un second terminal, exécutez :

```bash
cd BlazorGame.Client
dotnet build
dotnet run
```
Ensuite, ouvrez l’application à l’adresse suivante : http://localhost:5133/

## Tests Unitaires à venir 

### Logique du Joueur
1. **Changement de salle**  
   Vérifier que le joueur change bien de salle lorsque la salle actuelle est réussie.

2. **Ouverture du coffre**  
   Vérifier que le joueur obtient la récompense lorsqu’il ouvre un coffre.

3. **Perte de points de vie**  
   Vérifier que les points de vie perdus sont effectivement déduits du total.

4. **Utilisation de potion**  
   Vérifier qu’une potion restaure correctement les points de vie du joueur.

5. **Mort du joueur**  
   Vérifier que le joueur est déclaré perdant lorsqu’il n’a plus de points de vie.

### Logique du Jeu et des Salles
6. **Validité des salles**  
   Vérifier que toutes les salles du jeu sont bien non nulles.

7. **Première salle correcte**  
   Vérifier que la première salle définie est bien celle attendue.

8. **Fin du jeu** 
   Vérifier que lorsque le joueur atteint la dernière salle, la fin du jeu est déclenchée.

## Notes
- Assurez-vous que GameService est lancé avant de démarrer le frontend.
- Si un port est déjà utilisé, vous pouvez le modifier dans le fichier launchSettings.json.
