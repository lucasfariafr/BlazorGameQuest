# 🛡️ Fonctionnalités d'Administration - Bladebound

## Vue d'ensemble

Le système d'administration de Bladebound propose deux interfaces complémentaires pour gérer et visualiser les données du jeu.

---

## 📊 Dashboard Admin V2 (Recommandé)

### Accès
- **URL**: `/admin/dashboard`
- **Menu**: Cliquer sur "Administration" dans le menu de navigation

### Fonctionnalités principales

#### 1. Vue d'ensemble
Tableau de bord visuel avec:
- **Cartes statistiques** : Résumé des joueurs, sessions, monstres et scores
- **Graphique de répartition** : Distribution des sessions par statut (en cours, terminées, sauvegardées, abandonnées)
- **Activité récente** : Flux des 5 dernières sessions avec icônes de statut
- **Top 5 joueurs** : Mini-classement avec médailles (🥇🥈🥉)
- **Statistiques joueurs** : Joueurs vivants/morts, santé et force moyennes

#### 2. Gestion des joueurs
- Redirection vers le tableau de bord complet
- Export CSV et JSON des données joueurs

#### 3. Classement général
- Affichage du leaderboard complet (top 20)
- Médailles pour les 3 premiers
- Statistiques détaillées : parties jouées, victoires, défaites, scores
- Export CSV du classement

#### 4. Sessions de jeu
- Grille visuelle des sessions avec cartes colorées
- Codes couleur par statut :
  - 🟦 Bleu : En cours
  - 🟩 Vert : Victoire
  - 🟥 Rouge : Défaite
  - 🟨 Jaune : Sauvegardée
  - ⚫ Gris : Abandonnée

### Design moderne
- Gradient de fond élégant
- Cartes avec ombres et effets de survol
- Navigation par onglets intuitive
- Design responsive (mobile, tablette, desktop)
- Animations fluides

---

## 🔧 Dashboard Admin Complet

### Accès
- **URL**: `/admin`
- Accessible depuis le Dashboard V2 (onglet "Joueurs")

### Fonctionnalités avancées

#### Gestion des joueurs
- Liste complète avec toutes les informations
- Actions disponibles :
  - ✅ Activer/Désactiver un joueur
  - 📊 Voir les statistiques détaillées
  - 🗑️ Supprimer (désactiver) un joueur
- Modal de statistiques complètes par joueur

#### Exports
- Export CSV des joueurs
- Export JSON des joueurs
- Export CSV du classement

---

## 🎨 Composants réutilisables

### StatsCard
Composant moderne pour afficher des statistiques :
```razor
<StatsCard
    Icon="👥"
    Value="@playerCount"
    Label="Joueurs Totaux"
    SubText="@activePlayers actifs"
    CssClass="primary"
    OnClick="@(() => NavigateToPlayers())" />
```

**Variantes de couleurs** : `primary`, `success`, `danger`, `warning`, `info`

---

## 🔌 API Backend

### Endpoints disponibles

#### Statistiques
- `GET /api/Admin/overview` - Vue d'ensemble complète
- `GET /api/Admin/statistics` - Statistiques détaillées

#### Joueurs
- `GET /api/Admin/players` - Liste des joueurs
- `GET /api/Admin/players/{id}/stats` - Stats d'un joueur
- `PATCH /api/Admin/players/{id}/toggle-active` - Activer/désactiver
- `DELETE /api/Admin/players/{id}` - Supprimer (désactiver)

#### Exports
- `GET /api/Admin/players/export/csv` - Export CSV joueurs
- `GET /api/Admin/players/export/json` - Export JSON joueurs
- `GET /api/Admin/leaderboard/export/csv` - Export CSV classement

#### Classement & Sessions
- `GET /api/Admin/leaderboard?top=50` - Classement
- `GET /api/Admin/sessions` - Liste des sessions

---

## 🚀 Utilisation

### Démarrage
1. Lancez le serveur API : `dotnet run --project BlazorGame.GameService`
2. Lancez le client Blazor : `dotnet run --project BlazorGame.Client`
3. Accédez au dashboard : `http://localhost:PORT/admin/dashboard`

### Navigation
- **Menu principal** : Cliquez sur "Administration"
- **Entre dashboards** : Utilisez les boutons de navigation dans les interfaces
- **Rafraîchissement** : Bouton "Actualiser" dans l'en-tête

---

## 📱 Responsive Design

Les interfaces s'adaptent automatiquement :
- **Desktop** (>992px) : Grille complète, toutes les colonnes visibles
- **Tablette** (768px-992px) : Grille réduite, colonnes adaptées
- **Mobile** (<768px) : Vue en pile, navigation verticale

---

## 🎯 Points techniques

### Technologies utilisées
- **Blazor WebAssembly** : Frontend interactif
- **ASP.NET Core** : API RESTful
- **CSS Grid/Flexbox** : Layout responsive
- **Bootstrap 5** : Framework CSS de base

### Performance
- Chargement parallèle des données avec `Task.WhenAll`
- Mise en cache des résultats côté client
- Actualisation à la demande uniquement

### Sécurité
- Tous les endpoints admin nécessitent une authentification (à implémenter)
- Validation des données côté serveur
- Gestion des erreurs avec messages utilisateur

---

## 📝 Notes de développement

### Structure des fichiers
```
BlazorGame.Client/
├── Pages/
│   ├── AdminDashboard.razor        # Dashboard complet
│   └── AdminDashboardV2.razor      # Dashboard moderne (recommandé)
├── Components/
│   └── Admin/
│       ├── StatsCard.razor         # Composant de carte stats
│       └── StatsCard.razor.css     # Styles du composant
└── wwwroot/
    └── css/
        └── admin.css               # Styles globaux admin

BlazorGame.GameService/
└── Controllers/
    └── AdminController.cs          # API d'administration
```

### Constantes du jeu
Les constantes sont définies dans `GameConstants.cs`:
- Vie maximum : 100 HP
- Cœurs maximum : 3
- Dégâts d'armes : Épée (13), Arc (7), Bâton (9)
- Potions : Récupération (+20 HP), Force (+10)

---

## 🐛 Dépannage

### Le dashboard ne charge pas
1. Vérifiez que l'API tourne sur le bon port
2. Vérifiez `API_BASE_URL` dans AdminDashboardV2.razor (ligne 456)
3. Consultez la console du navigateur pour les erreurs

### Les exports ne fonctionnent pas
- Vérifiez que le navigateur autorise les pop-ups
- L'API doit être accessible depuis le client

### Problèmes de style
- Vérifiez que `admin.css` est chargé dans `index.html`
- Videz le cache du navigateur (Ctrl+F5)
- Vérifiez que le CSS isolé compile correctement

---

## 🔮 Améliorations futures possibles

- [ ] Authentification et autorisation admin
- [ ] Filtres et recherche dans les tableaux
- [ ] Pagination pour grandes quantités de données
- [ ] Graphiques interactifs (Charts.js, ApexCharts)
- [ ] WebSockets pour mises à jour en temps réel
- [ ] Dashboard de monitoring système
- [ ] Logs d'actions admin (audit trail)
- [ ] Gestion des dungeons et monstres
- [ ] Éditeur de contenu de jeu

---

## 📧 Support

Pour toute question ou problème, consultez :
- README.md principal du projet
- Documentation API (Swagger) : `http://localhost:PORT/swagger`
- Issues GitHub du projet

---

**Version** : 4.0 (Dashboard moderne)
**Dernière mise à jour** : Décembre 2025
