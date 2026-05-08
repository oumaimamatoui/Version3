import { createI18n } from 'vue-i18n';

const messages = {
  FR: {
    search: 'Rechercher… ',
    toggleTheme: 'Changer de thème',
    notifications: {
      title: 'Notifications',
      new: '{count} nouvelles',
      empty: 'Aucune notification',
      markRead: 'Tout marquer comme lu'
    },
    // --- ROLES ---
    roles: {
      SuperAdmin: 'Super Administrateur',
      AdminEntreprise: 'Administrateur Organisation',
      Evaluateur: 'Évaluateur Expert',
      Candidat: 'Candidat',
      Recruteur: 'RH / Recruteur',
      User: 'Utilisateur',
      Master: 'Master',
      Organisation: 'Organisation'
    },
    // --- PROFIL (Vue Simple) ---
    profile: {
      myProfile: 'Mon profil',
      settings: 'Paramètres',
      logout: 'Déconnexion',
      online: 'En ligne',
      changePhoto: 'Changer',
      share: 'Partager',
      edit: 'Editer',
      generalInfo: 'Informations Générales',
      email: 'Email Professionnel',
      joinedSince: 'Inscrit depuis',
      bio: 'Description / Bio',
      organization: 'Organisation',
      belongsTo: "Vous appartenez à l'entreprise :",
      uploadSuccess: 'Photo de profil mise à jour !',
      uploadError: "Échec de l'upload de l'image.",
      loading: 'Chargement...',
      placeholder: '...',
      avatarAlt: 'Photo de profil'
    },
    // --- SIDEBAR ---
    sidebar: {
      subBrand: 'Plateforme d\'évaluation intelligente',
      search: 'Recherche rapide…',
      dashboard: 'Tableau de bord',
      groups: {
        general: 'Général',
        recruitment: 'Recrutement',
        evaluations: 'Évaluations',
        analytics: 'Analytique',
        organization: 'Organisation',
        master: 'Console Maître',
        journey: 'Mon parcours'
      },
      links: {
        overview: 'Vue d\'ensemble',
        candidates: 'Candidats',
        invitations: 'Invitations',
        groups: 'Groupes',
        campaigns: 'Campagnes',
        bank: 'Banque questions',
        ai: 'Générateur IA',
        smart: 'Smart Analysis',
        stats: 'Statistiques',
        staff: 'Staff & Employés',
        roles: 'Rôles & Droits',
        settings: 'Paramètres',
        organizations: 'Organisations',
        manageUsers: 'Gérer utilisateurs',
        globalStats: 'Analytique globale',
        takeTest: 'Passer un test',
        results: 'Mes résultats',
        history: 'Historique'
      },
      stats: {
        active: '{count} actives',
        sync: 'Sync OK'
      }
    },
    // --- DASHBOARD ---
    dashboard: {
      terminal: { latency: 'LATENCE', system: 'SYSTEM_OS' },
      hero: {
        welcome: 'Ravi de vous revoir,',
        aiStatus: 'MOTEUR COGNITIF ACTIF',
        analyst: 'AURA_ANALYST v6.0',
        sync: 'SYNCHRONISÉ',
        loading: 'Calcul des flux...',
        defaultInsight: 'Analyse des talents terminée. 3 profils hautement compatibles détectés ce matin.'
      },
      kpis: {
        talents: 'TALENTS_TOTAL',
        success: 'INDEX_SUCCÈS',
        sessions: 'SESSIONS_ACTIVES',
        auraScore: 'SCORE_AURA_CORE'
      },
      accelerators: {
        title: 'IA_ACCÉLÉRATEURS',
        cvScan: { title: 'Neural CV Scan', desc: 'Matching instantané entre CV et fiches de poste via IA.' },
        testGen: { title: 'Test Generator AI', desc: 'Créez des tests techniques sur-mesure en quelques secondes.' },
        radar: { title: 'Personality Radar', desc: 'Visualisez les soft skills dominantes de vos candidats.' }
      },
      matrix: { growth: 'Matrice_Croissance_Talents', logs: 'Logs_Activité' },
      modals: {
        run: 'LANCER', create: 'CRÉER LE TEST', scan: 'LANCER LE SCAN', radar: 'VOIR LE RADAR',
        upload: 'Déposez le PDF du candidat', analyzing: 'ANALYSE EN COURS...', generating: 'GÉNÉRATION IA...',
        jobTitle: 'Poste ciblé', matchFound: 'Matching trouvé', strengths: 'Points Forts',
        newScan: 'Nouveau scan', radarTitle: 'Radar Cognitif', radarDesc: 'Analyse basée sur les patterns de réponse IA du candidat.'
      }
    },
    // --- CONFIGURATION / SETTINGS ---
    settings: {
      terminal: 'PARAMÈTRES DU TERMINAL',
      title: 'Configuration',
      titleSpan: 'Système',
      subtitle: 'Espace {role} — Gestion de compte NeoEvaluation',
      tabs: {
        profile: 'Profil',
        security: 'Sécurité',
        branding: 'Branding',
        integrations: 'Intégrations'
      },
      sections: {
        personalInfo: 'Informations Personnelles',
        security: 'Sécurité & Chiffrement',
        branding: 'Identité Visuelle Entreprise',
        integrations: 'Connexions Externes'
      },
      labels: {
        firstName: 'Prénom',
        lastName: 'Nom de famille',
        email: 'Email de liaison',
        bio: 'Ma Description / Bio',
        bioPlaceholder: 'Parlez-nous de vous...',
        currentPassword: 'Mot de passe actuel',
        newPassword: 'Nouveau mot de passe',
        confirmPassword: 'Confirmer le mot de passe',
        companyName: 'Raison sociale (Entreprise)',
        signatureColor: 'Couleur Signature',
        googleTitle: 'Google Gmail API',
        googleDesc: 'Envoyez vos emails via votre propre compte professionnel.',
        connectedAs: 'Connecté en tant que'
      },
      actions: {
        connect: 'CONNECTER',
        disconnect: 'DÉCONNECTER',
        cancel: 'ANNULER LES MODIFICATIONS',
        save: 'SAUVEGARDER',
        syncing: 'SYNCHRONISATION...',
        loadingCore: 'Connexion au Neural Core...'
      },
      alerts: {
        syncSuccess: 'Paramètres synchronisés avec succès !',
        passMismatch: 'Les mots de passe ne correspondent pas.',
        disconnectConfirm: "Êtes-vous sûr de vouloir déconnecter ce compte Gmail ? Vous ne pourrez plus envoyer d'emails système tant qu'il ne sera pas reconnecté.",
        disconnectSuccess: 'Compte déconnecté avec succès.',
        authError: "Impossible de joindre le service d'authentification Google."
      }
    }
  },

  EN: {
    search: 'Search…  ⌘K',
    toggleTheme: 'Toggle theme',
    notifications: {
      title: 'Notifications',
      new: '{count} new',
      empty: 'No notifications',
      markRead: 'Mark all as read'
    },
    // --- ROLES ---
    roles: {
      SuperAdmin: 'Super Admin',
      AdminEntreprise: 'Organization Administrator',
      Evaluateur: 'Expert Evaluator',
      Candidat: 'Candidate',
      Recruteur: 'HR / Recruiter',
      User: 'User',
      Master: 'Master',
      Organisation: 'Organization'
    },
    // --- PROFILE ---
    profile: {
      myProfile: 'My Profile',
      settings: 'Settings',
      logout: 'Logout',
      online: 'Online',
      changePhoto: 'Change',
      share: 'Share',
      edit: 'Edit',
      generalInfo: 'General Information',
      email: 'Professional Email',
      joinedSince: 'Joined since',
      bio: 'Description / Bio',
      organization: 'Organization',
      belongsTo: 'You belong to the company:',
      uploadSuccess: 'Profile picture updated!',
      uploadError: 'Failed to upload image.',
      loading: 'Loading...',
      placeholder: '...',
      avatarAlt: 'Profile picture'
    },
    // --- SIDEBAR ---
    sidebar: {
      subBrand: 'Smart Evaluation Platform',
      search: 'Quick search…',
      dashboard: 'Dashboard',
      groups: {
        general: 'General',
        recruitment: 'Recruitment',
        evaluations: 'Evaluations',
        analytics: 'Analytics',
        organization: 'Organization',
        master: 'Master Console',
        journey: 'My Journey'
      },
      links: {
        overview: 'Overview',
        candidates: 'Candidates',
        invitations: 'Invitations',
        groups: 'Groups',
        campaigns: 'Campaigns',
        bank: 'Question Bank',
        ai: 'AI Generator',
        smart: 'Smart Analysis',
        stats: 'Statistics',
        staff: 'Staff & Employees',
        roles: 'Roles & Rights',
        settings: 'Settings',
        organizations: 'Organizations',
        manageUsers: 'Manage Users',
        globalStats: 'Global Analytics',
        takeTest: 'Take a test',
        results: 'My Results',
        history: 'History'
      },
      stats: {
        active: '{count} active',
        sync: 'Sync OK'
      }
    },
    // --- DASHBOARD ---
    dashboard: {
      terminal: { latency: 'LATENCY', system: 'SYSTEM_OS' },
      hero: {
        welcome: 'Welcome back,',
        aiStatus: 'COGNITIVE ENGINE ACTIVE',
        analyst: 'AURA_ANALYST v6.0',
        sync: 'SYNCHRONIZED',
        loading: 'Calculating flows...',
        defaultInsight: 'Talent analysis complete. 3 highly compatible profiles detected this morning.'
      },
      kpis: {
        talents: 'TOTAL_TALENTS',
        success: 'SUCCESS_INDEX',
        sessions: 'ACTIVE_SESSIONS',
        auraScore: 'AURA_CORE_SCORE'
      },
      accelerators: {
        title: 'AI_ACCELERATORS',
        cvScan: { title: 'Neural CV Scan', desc: 'Instant matching between CVs and job descriptions via AI.' },
        testGen: { title: 'AI Test Generator', desc: 'Create custom technical tests in seconds.' },
        radar: { title: 'Personality Radar', desc: 'Visualize the dominant soft skills of your candidates.' }
      },
      matrix: { growth: 'Talent_Growth_Matrix', logs: 'Activity_Logs' },
      modals: {
        run: 'RUN', create: 'CREATE TEST', scan: 'START SCAN', radar: 'VIEW RADAR',
        upload: 'Drop candidate PDF', analyzing: 'ANALYSIS IN PROGRESS...', generating: 'AI GENERATING...',
        jobTitle: 'Targeted position', matchFound: 'Match found', strengths: 'Strengths',
        newScan: 'New scan', radarTitle: 'Cognitive Radar', radarDesc: 'Analysis based on candidate AI response patterns.'
      }
    },
    // --- CONFIGURATION / SETTINGS ---
    settings: {
      terminal: 'TERMINAL SETTINGS',
      title: 'System',
      titleSpan: 'Configuration',
      subtitle: '{role} Space — NeoEvaluation Account Management',
      tabs: {
        profile: 'Profile',
        security: 'Security',
        branding: 'Branding',
        integrations: 'Integrations'
      },
      sections: {
        personalInfo: 'Personal Information',
        security: 'Security & Encryption',
        branding: 'Company Visual Identity',
        integrations: 'External Connections'
      },
      labels: {
        firstName: 'First Name',
        lastName: 'Last Name',
        email: 'Link Email',
        bio: 'My Description / Bio',
        bioPlaceholder: 'Tell us about yourself...',
        currentPassword: 'Current Password',
        newPassword: 'New Password',
        confirmPassword: 'Confirm Password',
        companyName: 'Company Name',
        signatureColor: 'Signature Color',
        googleTitle: 'Google Gmail API',
        googleDesc: 'Send emails via your own professional account.',
        connectedAs: 'Connected as'
      },
      actions: {
        connect: 'CONNECT',
        disconnect: 'DISCONNECT',
        cancel: 'CANCEL CHANGES',
        save: 'SAVE CHANGES',
        syncing: 'SYNCHRONIZING...',
        loadingCore: 'Connecting to Neural Core...'
      },
      alerts: {
        syncSuccess: 'Settings synchronized successfully!',
        passMismatch: 'Passwords do not match.',
        disconnectConfirm: 'Are you sure you want to disconnect this Gmail account? You will not be able to send system emails until reconnected.',
        disconnectSuccess: 'Account disconnected successfully.',
        authError: 'Unable to reach Google authentication service.'
      }
    }
  }
};

const i18n = createI18n({
  legacy: false,
  locale: localStorage.getItem('lang') || 'FR',
  fallbackLocale: 'FR',
  messages,
});

export default i18n;