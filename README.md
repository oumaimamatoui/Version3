# 🎓 Manuel d'Apprentissage : Projet EvaluaTech

Bienvenue dans ce guide ! Ce document est conçu comme un **cours**, pour t'aider à comprendre comment fonctionne chaque pièce du puzzle de ton application.

---

## 🏗️ 1. Le Concept : C'est quoi un "SaaS Multi-Tenant" ?

Imagine un immense immeuble (Ton Application).
- Chaque entreprise qui s'inscrit loue un bureau (Un "Tenant").
- Elles partagent le même ascenseur et les mêmes murs (Le code Backend), mais personne ne peut entrer dans le bureau des autres (**L'isolation des données**).

### 💡 Exemple Concret :
Si **Entreprise A** crée une campagne de test, l'**Entreprise B** ne verra JAMAIS cette campagne. C'est géré automatiquement par le fichier `AppDbContext.cs` qui ajoute un filtre invisible sur chaque requête SQL.

---

## 🔗 2. Le Dialogue entre le Frontend et le Backend

Le Frontend (**Vue.js**) est comme une télécommande. Le Backend (**C#**) est comme le moteur de la télé.

1.  **Frontend** : "Hé, je veux la liste des candidats !" (`api.get('/Candidates')`)
2.  **Backend** : Il vérifie ton badge (**Le JWT Token**) pour savoir qui tu es.
3.  **Backend** : Il cherche en base de données UNIQUEMENT les candidats de ton entreprise.
4.  **Frontend** : Il reçoit la liste et l'affiche joliment.

---

## 🛠️ 3. Explication des Fichiers et Fonctions (Pas à Pas)

### 📂 Backend (Le Cerveau)

#### `InvitationsController.cs` (La porte d'entrée des nouveaux)
C'est ici qu'on gère les invitations.
- **Fonction `InviteStaff`** : 
    - *Code* : `_context.Utilisateurs.Add(personnel); await _context.SaveChangesAsync();`
    - *Pourquoi ?* On crée d'abord l'utilisateur en base, puis on génère son lien d'activation unique.

#### `GmailApiService.cs` (Le facteur intelligent)
C'est le module qui envoie les emails. On utilise une **stratégie de fallback**.
- **Le Code $(\textit{Simplifié})$** :
    ```csharp
    if (entreprise.HasGmailConfig) {
        // Envoi avec le compte Pro de l'entreprise
        await SendUsingGmailApi(entreprise, ...);
    } else {
        // Envoi avec le compte Master de la plateforme
        await SendUsingSmtp(masterConfig, ...);
    }
    ```

#### `AppDbContext.cs` (Le Gardien du Multi-Tenant)
C'est ici que la magie de la séparation des entreprises se passe.
- **Le Code** :
    ```csharp
    // Dit à EF Core d'ajouter "WHERE EntrepriseId = X" à chaque requête
    modelBuilder.Entity<Campagne>().HasQueryFilter(c => c.EntrepriseId == _tenantId);
    ```

---

### 📂 Frontend (Le Visage)

#### `api.js` (Le transporteur sécurisé)
Il s'assure que chaque appel au serveur possède ton "badge" d'accès.
- **Le Code** :
    ```javascript
    api.interceptors.request.use(config => {
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`; // Injection du token
      }
      return config;
    });
    ```

#### `auth.js` (Le Store Pinia)
Il garde en mémoire qui est connecté.
- **Exemple** : La fonction `login()` enregistre le token et le rôle pour que toute l'app sache si tu es un Admin ou un Recruteur.

---

## 🔐 4. La Sécurité : Le Jeton JWT

Quand tu te connectes (Login), le serveur ne te donne pas juste un "OK". Il te donne un **JWT (JSON Web Token)**. C'est comme un pass VIP qui contient :
- Ton **ID** d'utilisateur.
- Ton **Rôle** (Admin, Recruteur, Evaluateur).
- Ton **EntrepriseId**.

**Exemple d'utilisation dans le code** : 
`v-if="authStore.role === 'AdminEntreprise'"` (Pour n'afficher un bouton qu'aux admins).

---

## 📚 6. L'Encyclopédie des Modules (Le Code en Détail)

### 🤖 Module IA : Génération & Analyse
- **AIGeneratorView.vue** : Envoie un sujet (ex: "Java") au backend. Le backend interroge un modèle d'IA qui renvoie un JSON de questions. 
    - *Le truc en plus* : On utilise des "prompts" cachés pour s'assurer que les questions ont toujours 4 options et une seule bonne réponse.
- **AnalyseComportementaleView.vue** : Utilise les événements du navigateur (changement d'onglet, sortie de souris) pour détecter si un candidat triche. Chaque incident augmente un "Score de Risque".

### 📊 Module Exam : Le Calcul des Scores
- **ExamenController.cs** :
    - Quand un candidat soumet un test, le serveur compare chaque `ReponseId` choisie avec la `BonneReponseId` stockée en base de données.
    - **Calcul** : `PointObtenus / PointsTotaux * 100`. Le résultat est enregistré immédiatement dans la table `Candidature`.

### 🛡️ Module Sécurité : Comment le serveur sait qui tu es ?
- **TenantService.cs** : C'est le cerveau de la sécurité.
    - Il regarde le "Header" de la requête HTTP.
    - Il décode le JWT et récupère le `EntrepriseId`.
    - **Pourquoi ?** Pour que même si un hacker essaie de demander les données d'une autre entreprise, le serveur réponde "Non, ce n'est pas ton ID".

---

## 🏗️ 7. Structure des Dossiers (L'Arbre du Projet)

### 📂 Backend
- **Controllers** : Les ports d'entrée (Les "guichets").
- **Models** : La forme des données (Les "moules").
- **Migrations** : L'historique des changements de la base de données.
- **appsettings.json** : Les secrets et configurations (Clés API, Connection String).

### 📂 Frontend
- **views** : Les pages entières.
- **components** : Les petits morceaux réutilisables (Boutons, Sidebar).
- **assets** : Les images et styles CSS globaux.
- **stores** : La mémoire à court terme de l'application (Pinia).

---

**Félicitations ! Tu as maintenant une compréhension à 100% de la structure de ton projet.** 🌟📚
