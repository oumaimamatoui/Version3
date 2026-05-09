// ═══════════════════════════════════════════════════════════════════
//  i18n.js  —  NeoEvaluation Multilingual Core
//  Super Admin configure available languages globally.
//  Each user picks from the enabled set.
// ═══════════════════════════════════════════════════════════════════

import { createI18n } from 'vue-i18n';

// ─── LANGUAGE CONFIG KEYS (localStorage) ───────────────────────────
export const LANG_CONFIG_KEY      = 'neoeval_lang_config';   // Super Admin settings
export const USER_LANG_KEY        = 'lang';                   // per-user choice

// ─── DEFAULT SUPER-ADMIN CONFIG ─────────────────────────────────────
// Stored in localStorage under LANG_CONFIG_KEY (synced with backend in real app)
export const DEFAULT_LANG_CONFIG = {
  available: ['FR', 'EN', 'AR'],   // which codes are active
  default:   'FR',                  // platform default
};

// ─── HELPERS ────────────────────────────────────────────────────────

/** Read Super Admin language config (localStorage fallback) */
export function getLangConfig() {
  try {
    const raw = localStorage.getItem(LANG_CONFIG_KEY);
    return raw ? JSON.parse(raw) : { ...DEFAULT_LANG_CONFIG };
  } catch {
    return { ...DEFAULT_LANG_CONFIG };
  }
}

/** Save Super Admin language config (localStorage + optional API call) */
export function saveLangConfig(config) {
  localStorage.setItem(LANG_CONFIG_KEY, JSON.stringify(config));
}

/** Return the user's preferred locale, constrained to available languages */
export function resolveUserLocale() {
  const config   = getLangConfig();
  const saved    = localStorage.getItem(USER_LANG_KEY);
  if (saved && config.available.includes(saved)) return saved;
  if (config.available.includes(config.default)) return config.default;
  return config.available[0] || 'FR';
}

/** Set a user locale (only if it is in the available list) */
export function setUserLocale(code) {
  const config = getLangConfig();
  if (!config.available.includes(code)) return false;
  localStorage.setItem(USER_LANG_KEY, code);
  return true;
}

/** All supported locale codes with metadata */
export const ALL_LOCALES = [
  { code: 'FR', label: 'Français',  flag: '🇫🇷', dir: 'ltr', nativeName: 'Français' },
  { code: 'EN', label: 'English',   flag: '🇬🇧', dir: 'ltr', nativeName: 'English'  },
  { code: 'AR', label: 'العربية',    flag: '🇹🇳', dir: 'rtl', nativeName: 'العربية'  },
];

// ─── TRANSLATIONS ────────────────────────────────────────────────────

const messages = {

  // ══════════════════════════════════════════════════════════════
  //  FRENCH
  // ══════════════════════════════════════════════════════════════
  FR: {
    search: 'Rechercher… ',
    toggleTheme: 'Changer de thème',
    notifications: {
      title: 'Notifications',
      new: '{count} nouvelles',
      empty: 'Aucune notification',
      markRead: 'Tout marquer comme lu',
    },
    // --- ROLES ---
    roles: {
      SuperAdmin:       'Super Administrateur',
      AdminEntreprise:  'Administrateur Organisation',
      Evaluateur:       'Évaluateur Expert',
      Candidat:         'Candidat',
      Recruteur:        'RH / Recruteur',
      User:             'Utilisateur',
      Master:           'Master',
      Organisation:     'Organisation',
    },
    // --- PROFILE ---
    profile: {
      myProfile:     'Mon profil',
      settings:      'Paramètres',
      logout:        'Déconnexion',
      online:        'En ligne',
      changePhoto:   'Changer',
      share:         'Partager',
      edit:          'Éditer',
      generalInfo:   'Informations Générales',
      email:         'Email Professionnel',
      joinedSince:   'Inscrit depuis',
      bio:           'Description / Bio',
      organization:  'Organisation',
      belongsTo:     "Vous appartenez à l'entreprise :",
      uploadSuccess: 'Photo de profil mise à jour !',
      uploadError:   "Échec de l'upload de l'image.",
      loading:       'Chargement...',
      placeholder:   '...',
      avatarAlt:     'Photo de profil',
    },
    // --- SIDEBAR ---
    sidebar: {
      subBrand: "Plateforme d'évaluation intelligente",
      search:   'Recherche rapide…',
      dashboard:'Tableau de bord',
      groups: {
        general:      'Général',
        recruitment:  'Recrutement',
        evaluations:  'Évaluations',
        analytics:    'Analytique',
        organization: 'Organisation',
        master:       'Console Maître',
        journey:      'Mon parcours',
      },
      links: {
        overview:      "Vue d'ensemble",
        candidates:    'Candidats',
        invitations:   'Invitations',
        groups:        'Groupes',
        campaigns:     'Campagnes',
        bank:          'Banque questions',
        ai:            'Générateur IA',
        smart:         'Smart Analysis',
        stats:         'Statistiques',
        staff:         'Staff & Employés',
        roles:         'Rôles & Droits',
        settings:      'Paramètres',
        organizations: 'Organisations',
        manageUsers:   'Gérer utilisateurs',
        globalStats:   'Analytique globale',
        takeTest:      'Passer un test',
        results:       'Mes résultats',
        history:       'Historique',
      },
      stats: {
        active: '{count} actives',
        sync:   'Sync OK',
      },
    },
    // --- DASHBOARD ---
    dashboard: {
      terminal: { latency: 'LATENCE', system: 'SYSTEM_OS' },
      hero: {
        welcome:        'Ravi de vous revoir,',
        aiStatus:       'MOTEUR COGNITIF ACTIF',
        analyst:        'AURA_ANALYST v6.0',
        sync:           'SYNCHRONISÉ',
        loading:        'Calcul des flux...',
        defaultInsight: 'Analyse des talents terminée. 3 profils hautement compatibles détectés ce matin.',
      },
      kpis: {
        talents:   'TALENTS_TOTAL',
        success:   'INDEX_SUCCÈS',
        sessions:  'SESSIONS_ACTIVES',
        auraScore: 'SCORE_AURA_CORE',
      },
      accelerators: {
        title: 'IA_ACCÉLÉRATEURS',
        cvScan:  { title: 'Neural CV Scan',   desc: 'Matching instantané entre CV et fiches de poste via IA.' },
        testGen: { title: 'Test Generator AI', desc: 'Créez des tests techniques sur-mesure en quelques secondes.' },
        radar:   { title: 'Personality Radar', desc: 'Visualisez les soft skills dominantes de vos candidats.' },
      },
      matrix: { growth: 'Matrice_Croissance_Talents', logs: 'Logs_Activité' },
      modals: {
        run: 'LANCER', create: 'CRÉER LE TEST', scan: 'LANCER LE SCAN', radar: 'VOIR LE RADAR',
        upload: 'Déposez le PDF du candidat', analyzing: 'ANALYSE EN COURS...', generating: 'GÉNÉRATION IA...',
        jobTitle: 'Poste ciblé', matchFound: 'Matching trouvé', strengths: 'Points Forts',
        newScan: 'Nouveau scan', radarTitle: 'Radar Cognitif',
        radarDesc: "Analyse basée sur les patterns de réponse IA du candidat.",
      },
    },
    // --- SETTINGS ---
    settings: {
      terminal:  'PARAMÈTRES DU TERMINAL',
      title:     'Configuration',
      titleSpan: 'Système',
      subtitle:  'Espace {role} — Gestion de compte NeoEvaluation',
      tabs: {
        profile:      'Profil',
        security:     'Sécurité',
        branding:     'Branding',
        integrations: 'Intégrations',
      },
      sections: {
        personalInfo: 'Informations Personnelles',
        security:     'Sécurité & Chiffrement',
        branding:     'Identité Visuelle Entreprise',
        integrations: 'Connexions Externes',
      },
      labels: {
        firstName:       'Prénom',
        lastName:        'Nom de famille',
        email:           'Email de liaison',
        bio:             'Ma Description / Bio',
        bioPlaceholder:  'Parlez-nous de vous...',
        currentPassword: 'Mot de passe actuel',
        newPassword:     'Nouveau mot de passe',
        confirmPassword: 'Confirmer le mot de passe',
        companyName:     'Raison sociale (Entreprise)',
        signatureColor:  'Couleur Signature',
        googleTitle:     'Google Gmail API',
        googleDesc:      "Envoyez vos emails via votre propre compte professionnel.",
        connectedAs:     'Connecté en tant que',
      },
      actions: {
        connect:     'CONNECTER',
        disconnect:  'DÉCONNECTER',
        cancel:      'ANNULER LES MODIFICATIONS',
        save:        'SAUVEGARDER',
        syncing:     'SYNCHRONISATION...',
        loadingCore: 'Connexion au Neural Core...',
      },
      alerts: {
        syncSuccess:        'Paramètres synchronisés avec succès !',
        passMismatch:       'Les mots de passe ne correspondent pas.',
        disconnectConfirm:  "Êtes-vous sûr de vouloir déconnecter ce compte Gmail ?",
        disconnectSuccess:  'Compte déconnecté avec succès.',
        authError:          "Impossible de joindre le service d'authentification Google.",
      },
    },
    // --- LANGUAGE MANAGER (Super Admin) ---
    langManager: {
      title:           'Gestion Multilingue',
      subtitle:        'Configurez les langues disponibles sur la plateforme',
      availableLangs:  'Langues disponibles',
      defaultLang:     'Langue par défaut',
      enabledBadge:    'Activée',
      disabledBadge:   'Désactivée',
      toggleEnable:    'Activer',
      toggleDisable:   'Désactiver',
      setDefault:      'Définir par défaut',
      isDefault:       'Défaut',
      save:            'SAUVEGARDER LA CONFIG',
      saving:          'SAUVEGARDE...',
      saveSuccess:     'Configuration multilingue sauvegardée !',
      saveError:       'Erreur lors de la sauvegarde.',
      atLeastOne:      'Au moins une langue doit rester activée.',
      previewTitle:    'Aperçu utilisateur',
      previewDesc:     "Les utilisateurs verront uniquement les langues activées ci-dessus.",
      dirLabel:        'Sens',
      ltr:             'Gauche → Droite',
      rtl:             'Droite ← Gauche',
      langCode:        'Code',
      nativeName:      'Nom natif',
      usersCount:      '{n} utilisateurs',
    },
  },

  // ══════════════════════════════════════════════════════════════
  //  ENGLISH
  // ══════════════════════════════════════════════════════════════
  EN: {
    search: 'Search…  ⌘K',
    toggleTheme: 'Toggle theme',
    notifications: {
      title:    'Notifications',
      new:      '{count} new',
      empty:    'No notifications',
      markRead: 'Mark all as read',
    },
    roles: {
      SuperAdmin:      'Super Admin',
      AdminEntreprise: 'Organization Administrator',
      Evaluateur:      'Expert Evaluator',
      Candidat:        'Candidate',
      Recruteur:       'HR / Recruiter',
      User:            'User',
      Master:          'Master',
      Organisation:    'Organization',
    },
    profile: {
      myProfile:     'My Profile',
      settings:      'Settings',
      logout:        'Logout',
      online:        'Online',
      changePhoto:   'Change',
      share:         'Share',
      edit:          'Edit',
      generalInfo:   'General Information',
      email:         'Professional Email',
      joinedSince:   'Joined since',
      bio:           'Description / Bio',
      organization:  'Organization',
      belongsTo:     'You belong to the company:',
      uploadSuccess: 'Profile picture updated!',
      uploadError:   'Failed to upload image.',
      loading:       'Loading...',
      placeholder:   '...',
      avatarAlt:     'Profile picture',
    },
    sidebar: {
      subBrand:  'Smart Evaluation Platform',
      search:    'Quick search…',
      dashboard: 'Dashboard',
      groups: {
        general:      'General',
        recruitment:  'Recruitment',
        evaluations:  'Evaluations',
        analytics:    'Analytics',
        organization: 'Organization',
        master:       'Master Console',
        journey:      'My Journey',
      },
      links: {
        overview:      'Overview',
        candidates:    'Candidates',
        invitations:   'Invitations',
        groups:        'Groups',
        campaigns:     'Campaigns',
        bank:          'Question Bank',
        ai:            'AI Generator',
        smart:         'Smart Analysis',
        stats:         'Statistics',
        staff:         'Staff & Employees',
        roles:         'Roles & Rights',
        settings:      'Settings',
        organizations: 'Organizations',
        manageUsers:   'Manage Users',
        globalStats:   'Global Analytics',
        takeTest:      'Take a test',
        results:       'My Results',
        history:       'History',
      },
      stats: {
        active: '{count} active',
        sync:   'Sync OK',
      },
    },
    dashboard: {
      terminal: { latency: 'LATENCY', system: 'SYSTEM_OS' },
      hero: {
        welcome:        'Welcome back,',
        aiStatus:       'COGNITIVE ENGINE ACTIVE',
        analyst:        'AURA_ANALYST v6.0',
        sync:           'SYNCHRONIZED',
        loading:        'Calculating flows...',
        defaultInsight: 'Talent analysis complete. 3 highly compatible profiles detected this morning.',
      },
      kpis: {
        talents:   'TOTAL_TALENTS',
        success:   'SUCCESS_INDEX',
        sessions:  'ACTIVE_SESSIONS',
        auraScore: 'AURA_CORE_SCORE',
      },
      accelerators: {
        title: 'AI_ACCELERATORS',
        cvScan:  { title: 'Neural CV Scan',    desc: 'Instant matching between CVs and job descriptions via AI.' },
        testGen: { title: 'AI Test Generator', desc: 'Create custom technical tests in seconds.' },
        radar:   { title: 'Personality Radar', desc: 'Visualize the dominant soft skills of your candidates.' },
      },
      matrix: { growth: 'Talent_Growth_Matrix', logs: 'Activity_Logs' },
      modals: {
        run: 'RUN', create: 'CREATE TEST', scan: 'START SCAN', radar: 'VIEW RADAR',
        upload: 'Drop candidate PDF', analyzing: 'ANALYSIS IN PROGRESS...', generating: 'AI GENERATING...',
        jobTitle: 'Targeted position', matchFound: 'Match found', strengths: 'Strengths',
        newScan: 'New scan', radarTitle: 'Cognitive Radar',
        radarDesc: 'Analysis based on candidate AI response patterns.',
      },
    },
    settings: {
      terminal:  'TERMINAL SETTINGS',
      title:     'System',
      titleSpan: 'Configuration',
      subtitle:  '{role} Space — NeoEvaluation Account Management',
      tabs: {
        profile:      'Profile',
        security:     'Security',
        branding:     'Branding',
        integrations: 'Integrations',
      },
      sections: {
        personalInfo: 'Personal Information',
        security:     'Security & Encryption',
        branding:     'Company Visual Identity',
        integrations: 'External Connections',
      },
      labels: {
        firstName:       'First Name',
        lastName:        'Last Name',
        email:           'Link Email',
        bio:             'My Description / Bio',
        bioPlaceholder:  'Tell us about yourself...',
        currentPassword: 'Current Password',
        newPassword:     'New Password',
        confirmPassword: 'Confirm Password',
        companyName:     'Company Name',
        signatureColor:  'Signature Color',
        googleTitle:     'Google Gmail API',
        googleDesc:      'Send emails via your own professional account.',
        connectedAs:     'Connected as',
      },
      actions: {
        connect:     'CONNECT',
        disconnect:  'DISCONNECT',
        cancel:      'CANCEL CHANGES',
        save:        'SAVE CHANGES',
        syncing:     'SYNCHRONIZING...',
        loadingCore: 'Connecting to Neural Core...',
      },
      alerts: {
        syncSuccess:       'Settings synchronized successfully!',
        passMismatch:      'Passwords do not match.',
        disconnectConfirm: 'Are you sure you want to disconnect this Gmail account?',
        disconnectSuccess: 'Account disconnected successfully.',
        authError:         'Unable to reach Google authentication service.',
      },
    },
    langManager: {
      title:          'Language Management',
      subtitle:       'Configure available platform languages',
      availableLangs: 'Available Languages',
      defaultLang:    'Default Language',
      enabledBadge:   'Enabled',
      disabledBadge:  'Disabled',
      toggleEnable:   'Enable',
      toggleDisable:  'Disable',
      setDefault:     'Set as default',
      isDefault:      'Default',
      save:           'SAVE CONFIG',
      saving:         'SAVING...',
      saveSuccess:    'Multilingual configuration saved!',
      saveError:      'Error saving configuration.',
      atLeastOne:     'At least one language must remain enabled.',
      previewTitle:   'User Preview',
      previewDesc:    'Users will only see the languages enabled above.',
      dirLabel:       'Direction',
      ltr:            'Left → Right',
      rtl:            'Right ← Left',
      langCode:       'Code',
      nativeName:     'Native name',
      usersCount:     '{n} users',
    },
  },

  // ══════════════════════════════════════════════════════════════
  //  ARABIC
  // ══════════════════════════════════════════════════════════════
  AR: {
    search: 'بحث…',
    toggleTheme: 'تبديل المظهر',
    notifications: {
      title:    'الإشعارات',
      new:      '{count} جديدة',
      empty:    'لا توجد إشعارات',
      markRead: 'تحديد الكل كمقروء',
    },
    roles: {
      SuperAdmin:      'المشرف العام',
      AdminEntreprise: 'مدير المنظمة',
      Evaluateur:      'مقيّم خبير',
      Candidat:        'مرشح',
      Recruteur:       'موارد بشرية / مجنّد',
      User:            'مستخدم',
      Master:          'ماستر',
      Organisation:    'منظمة',
    },
    profile: {
      myProfile:     'ملفي الشخصي',
      settings:      'الإعدادات',
      logout:        'تسجيل الخروج',
      online:        'متصل',
      changePhoto:   'تغيير',
      share:         'مشاركة',
      edit:          'تعديل',
      generalInfo:   'معلومات عامة',
      email:         'البريد المهني',
      joinedSince:   'عضو منذ',
      bio:           'الوصف / النبذة',
      organization:  'المنظمة',
      belongsTo:     'أنت تنتمي إلى الشركة:',
      uploadSuccess: 'تم تحديث صورة الملف الشخصي!',
      uploadError:   'فشل تحميل الصورة.',
      loading:       'جارٍ التحميل...',
      placeholder:   '...',
      avatarAlt:     'صورة الملف الشخصي',
    },
    sidebar: {
      subBrand:  'منصة التقييم الذكي',
      search:    'بحث سريع…',
      dashboard: 'لوحة التحكم',
      groups: {
        general:      'عام',
        recruitment:  'التوظيف',
        evaluations:  'التقييمات',
        analytics:    'التحليلات',
        organization: 'المنظمة',
        master:       'وحدة التحكم الرئيسية',
        journey:      'مساري',
      },
      links: {
        overview:      'نظرة عامة',
        candidates:    'المرشحون',
        invitations:   'الدعوات',
        groups:        'المجموعات',
        campaigns:     'الحملات',
        bank:          'بنك الأسئلة',
        ai:            'مولّد الذكاء الاصطناعي',
        smart:         'التحليل الذكي',
        stats:         'الإحصائيات',
        staff:         'الموظفون والطاقم',
        roles:         'الأدوار والصلاحيات',
        settings:      'الإعدادات',
        organizations: 'المنظمات',
        manageUsers:   'إدارة المستخدمين',
        globalStats:   'التحليلات الشاملة',
        takeTest:      'إجراء اختبار',
        results:       'نتائجي',
        history:       'السجل',
      },
      stats: {
        active: '{count} نشطة',
        sync:   'مزامنة ناجحة',
      },
    },
    dashboard: {
      terminal: { latency: 'زمن الاستجابة', system: 'نظام_التشغيل' },
      hero: {
        welcome:        'مرحباً بعودتك،',
        aiStatus:       'المحرك المعرفي نشط',
        analyst:        'AURA_ANALYST v6.0',
        sync:           'متزامن',
        loading:        'جارٍ حساب التدفقات...',
        defaultInsight: 'اكتمل تحليل المواهب. تم اكتشاف 3 ملفات شخصية متوافقة للغاية هذا الصباح.',
      },
      kpis: {
        talents:   'إجمالي_المواهب',
        success:   'مؤشر_النجاح',
        sessions:  'الجلسات_النشطة',
        auraScore: 'نقاط_أورا_الأساسية',
      },
      accelerators: {
        title: 'مسرّعات_الذكاء_الاصطناعي',
        cvScan:  { title: 'مسح السيرة الذاتية العصبي', desc: 'مطابقة فورية بين السير الذاتية وأوصاف الوظائف عبر الذكاء الاصطناعي.' },
        testGen: { title: 'مولّد الاختبارات الذكي',    desc: 'أنشئ اختبارات تقنية مخصصة في ثوانٍ.' },
        radar:   { title: 'رادار الشخصية',              desc: 'تصور المهارات الشخصية السائدة لمرشحيك.' },
      },
      matrix: { growth: 'مصفوفة_نمو_المواهب', logs: 'سجلات_النشاط' },
      modals: {
        run: 'تشغيل', create: 'إنشاء الاختبار', scan: 'بدء المسح', radar: 'عرض الرادار',
        upload: 'أفلت ملف PDF للمرشح', analyzing: 'جارٍ التحليل...', generating: 'الذكاء الاصطناعي يولّد...',
        jobTitle: 'المنصب المستهدف', matchFound: 'تم العثور على تطابق', strengths: 'نقاط القوة',
        newScan: 'مسح جديد', radarTitle: 'الرادار المعرفي',
        radarDesc: 'تحليل مبني على أنماط استجابة الذكاء الاصطناعي للمرشح.',
      },
    },
    settings: {
      terminal:  'إعدادات الطرفية',
      title:     'التكوين',
      titleSpan: 'النظام',
      subtitle:  'فضاء {role} — إدارة حساب NeoEvaluation',
      tabs: {
        profile:      'الملف الشخصي',
        security:     'الأمان',
        branding:     'الهوية البصرية',
        integrations: 'التكاملات',
      },
      sections: {
        personalInfo: 'المعلومات الشخصية',
        security:     'الأمان والتشفير',
        branding:     'الهوية البصرية للشركة',
        integrations: 'الاتصالات الخارجية',
      },
      labels: {
        firstName:       'الاسم الأول',
        lastName:        'اسم العائلة',
        email:           'البريد الإلكتروني',
        bio:             'وصفي / نبذتي',
        bioPlaceholder:  'أخبرنا عن نفسك...',
        currentPassword: 'كلمة المرور الحالية',
        newPassword:     'كلمة المرور الجديدة',
        confirmPassword: 'تأكيد كلمة المرور',
        companyName:     'اسم الشركة',
        signatureColor:  'لون التوقيع',
        googleTitle:     'Google Gmail API',
        googleDesc:      'أرسل رسائل البريد الإلكتروني عبر حسابك المهني.',
        connectedAs:     'متصل بوصفك',
      },
      actions: {
        connect:     'اتصال',
        disconnect:  'قطع الاتصال',
        cancel:      'إلغاء التغييرات',
        save:        'حفظ',
        syncing:     'جارٍ المزامنة...',
        loadingCore: 'جارٍ الاتصال بالنواة العصبية...',
      },
      alerts: {
        syncSuccess:       'تمت مزامنة الإعدادات بنجاح!',
        passMismatch:      'كلمتا المرور غير متطابقتين.',
        disconnectConfirm: 'هل أنت متأكد من رغبتك في قطع الاتصال بهذا الحساب؟',
        disconnectSuccess: 'تم قطع الاتصال بالحساب بنجاح.',
        authError:         'تعذّر الوصول إلى خدمة مصادقة Google.',
      },
    },
    langManager: {
      title:          'إدارة اللغات',
      subtitle:       'تكوين اللغات المتاحة على المنصة',
      availableLangs: 'اللغات المتاحة',
      defaultLang:    'اللغة الافتراضية',
      enabledBadge:   'مفعّلة',
      disabledBadge:  'معطّلة',
      toggleEnable:   'تفعيل',
      toggleDisable:  'تعطيل',
      setDefault:     'تعيين كافتراضي',
      isDefault:      'افتراضي',
      save:           'حفظ الإعدادات',
      saving:         'جارٍ الحفظ...',
      saveSuccess:    'تم حفظ إعدادات اللغة بنجاح!',
      saveError:      'خطأ أثناء الحفظ.',
      atLeastOne:     'يجب أن تظل لغة واحدة على الأقل مفعّلة.',
      previewTitle:   'معاينة المستخدم',
      previewDesc:    'سيرى المستخدمون فقط اللغات المفعّلة أعلاه.',
      dirLabel:       'الاتجاه',
      ltr:            'يسار ← يمين',
      rtl:            'يمين → يسار',
      langCode:       'الرمز',
      nativeName:     'الاسم المحلي',
      usersCount:     '{n} مستخدم',
    },
  },
};

// ─── CREATE & EXPORT I18N INSTANCE ──────────────────────────────────
const i18n = createI18n({
  legacy:         false,
  locale:         resolveUserLocale(),
  fallbackLocale: 'FR',
  messages,
});

export default i18n;