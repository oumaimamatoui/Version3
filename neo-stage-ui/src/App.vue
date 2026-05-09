<template>
  <div id="app-container" :class="['app-root', isDark ? 'theme-dark' : 'theme-light']" :dir="isRTL ? 'rtl' : 'ltr'">
    <router-view v-slot="{ Component }">
      <transition name="page-fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>

    <!-- ============================================================ -->
    <!-- NEOBOT v6.0 — ASSISTANT IA UNIVERSEL                        -->
    <!-- ============================================================ -->
    <div class="chatbot-wrapper">
      <transition name="bubble-bounce">
        <div @click="toggleChat" class="chat-bubble" :class="{ 'active': isChatOpen }">
          <div class="bubble-ring"></div>
          <i v-if="!isChatOpen" class="fa fa-comments"></i>
          <i v-else class="fa fa-times"></i>
          <span v-if="unreadCount > 0 && !isChatOpen" class="unread-badge">{{ unreadCount }}</span>
        </div>
      </transition>

      <transition name="chat-slide">
        <div v-if="isChatOpen" class="chat-window">

          <div class="chat-header">
            <div class="header-left">
              <div class="bot-avatar">
                <i class="fa fa-robot"></i>
                <span class="avatar-pulse"></span>
              </div>
              <div class="header-info">
                <h6>NeoBot Assistant</h6>
                <div class="status-row">
                  <span class="dot-online"></span>
                  <small>{{ t('chatbot.online') }}</small>
                </div>
              </div>
            </div>
            <div class="header-actions">
              <button @click="toggleTheme" class="btn-action" :title="isDark ? t('theme.light') : t('theme.dark')">
                <i class="fa" :class="isDark ? 'fa-sun' : 'fa-moon'"></i>
              </button>
              <button @click="cycleLang" class="btn-action btn-lang" :title="t('lang.switch')">
                {{ langFlags[currentLang] }}
              </button>
              <button @click="clearChat" class="btn-action" :title="t('chatbot.clear')">
                <i class="fa fa-trash-alt"></i>
              </button>
              <button @click="toggleChat" class="btn-action btn-close-chat">
                <i class="fa fa-times"></i>
              </button>
            </div>
          </div>

          <div v-if="isChatLoading" class="loading-bar">
            <div class="loading-bar-fill"></div>
          </div>

          <div class="chat-body" ref="chatScroll">
            <div v-if="chatMessages.length <= 1 && startSuggestions.length > 0" class="start-suggestions">
              <p class="suggestions-label">{{ t('chatbot.suggestLabel') }}</p>
              <div class="suggestion-chips">
                <button v-for="(s, i) in startSuggestions" :key="i" @click="sendSuggestion(s)" class="chip">
                  {{ s }}
                </button>
              </div>
            </div>

            <div v-for="(msg, index) in chatMessages" :key="index" :class="['chat-msg', msg.role]">
              <div v-if="msg.role === 'ai'" class="msg-avatar">
                <i class="fa fa-robot"></i>
              </div>
              <div class="msg-bubble">
                <div class="msg-content" v-html="formatMessage(msg.text)"></div>
                <span v-if="msg.source && msg.source !== 'gemini'" class="source-badge">
                  {{ msg.source === 'cache' ? '⚡ Cache' : msg.source === 'intent' ? '🧠 Local' : '🤖 IA' }}
                </span>
                <div v-if="msg.role === 'ai'" class="msg-actions">
                  <button @click="speak(msg.text)" class="msg-action-btn" :title="t('chatbot.speak')">
                    <i class="fa fa-volume-up"></i>
                  </button>
                  <button @click="copyText(msg.text)" class="msg-action-btn" :title="t('chatbot.copy')">
                    <i class="fa fa-copy"></i>
                  </button>
                </div>
                <div v-if="msg.suggestions && msg.suggestions.length > 0" class="follow-suggestions">
                  <button v-for="(s, si) in msg.suggestions" :key="si" @click="sendSuggestion(s)" class="follow-chip">
                    {{ s }}
                  </button>
                </div>
                <span class="msg-time">{{ msg.time }}</span>
              </div>
            </div>

            <div v-if="isChatLoading" class="chat-msg ai typing-indicator-wrap">
              <div class="msg-avatar"><i class="fa fa-robot"></i></div>
              <div class="msg-bubble">
                <div class="msg-content typing-dots">
                  <span></span><span></span><span></span>
                </div>
              </div>
            </div>
          </div>

          <div class="chat-footer">
            <div v-if="isListening" class="voice-active-bar">
              <i class="fa fa-microphone"></i>
              <span>{{ t('chatbot.listening') }}</span>
              <div class="voice-waves">
                <span v-for="i in 5" :key="i"></span>
              </div>
            </div>
            <div class="input-row">
              <button @click="toggleVoiceRecognition" class="btn-mic" :class="{ 'mic-active': isListening }" :title="t('chatbot.voice')">
                <i class="fa" :class="isListening ? 'fa-microphone-slash' : 'fa-microphone'"></i>
              </button>
              <div class="input-wrapper">
                <input
                  v-model="chatInput"
                  @keyup.enter="handleChat(false)"
                  type="text"
                  class="chat-input"
                  :placeholder="t('chatbot.placeholder')"
                  :disabled="isChatLoading"
                  ref="chatInputRef"
                />
                <span v-if="chatInput.length > 0" class="char-count">{{ chatInput.length }}</span>
              </div>
              <button @click="handleChat(false)" class="btn-send" :disabled="!chatInput.trim() || isChatLoading">
                <i class="fa fa-paper-plane"></i>
              </button>
            </div>
            <div class="footer-note">
              <i class="fa fa-shield-alt"></i>
              NeoStage · {{ t('chatbot.poweredBy') }}
            </div>
          </div>

        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { watch, onMounted, onUnmounted, ref, computed, nextTick, provide } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useNotificationStore } from '@/stores/notification';

const route          = useRoute();
const authStore      = useAuthStore();
const notifStore     = useNotificationStore();

// ══════════════════════════════════════════════════════════
// SIGNALR — CONNEXION NOTIFICATIONS TEMPS RÉEL
// Se connecte dès que l'utilisateur est authentifié,
// se déconnecte au logout ou à la destruction du composant.
// ══════════════════════════════════════════════════════════

onMounted(async () => {
  applyPageTheme();
  applyLangToDocument();

  // Connexion SignalR si déjà authentifié au montage
  if (authStore.isAuthenticated) {
    await notifStore.connect();
  }
});

// Reconnexion automatique après login / déconnexion après logout
watch(
  () => authStore.isAuthenticated,
  async (isAuth) => {
    if (isAuth) {
      await notifStore.connect();
    } else {
      await notifStore.disconnect();
    }
  }
);

// Nettoyage propre à la destruction du composant racine
onUnmounted(async () => {
  await notifStore.disconnect();
});

// ══════════════════════════════════════════════════════════
// I18N — SYSTÈME DE TRADUCTIONS GLOBAL (3 LANGUES)
// ══════════════════════════════════════════════════════════

const TRANSLATIONS = {
  fr: {
    'theme.dark'  : 'Mode Sombre',
    'theme.light' : 'Mode Clair',
    'lang.switch' : 'Changer de langue',
    'chatbot.online'     : 'En ligne · Répond en temps réel',
    'chatbot.placeholder': 'Posez votre question...',
    'chatbot.welcome'    : '👋 Bonjour ! Je suis **NeoBot**, votre assistant IA NeoStage.\nJe peux vous aider à créer des tests, analyser des CVs, générer des rapports et bien plus.',
    'chatbot.error'      : '⚠️ Serveur indisponible. Veuillez réessayer.',
    'chatbot.suggestLabel': 'Questions fréquentes :',
    'chatbot.listening'  : 'Je vous écoute...',
    'chatbot.voice'      : 'Commande vocale',
    'chatbot.copy'       : 'Copier',
    'chatbot.speak'      : 'Lire à voix haute',
    'chatbot.clear'      : 'Effacer la conversation',
    'chatbot.poweredBy'  : 'Propulsé par Gemini IA',
    'chatbot.copied'     : '✅ Copié !',
    'nav.dashboard'  : 'Tableau de bord',
    'nav.tests'      : 'Tests',
    'nav.candidates' : 'Candidats',
    'nav.reports'    : 'Rapports',
    'nav.settings'   : 'Paramètres',
    'nav.logout'     : 'Déconnexion',
    'nav.profile'    : 'Mon profil',
    'common.save'    : 'Enregistrer',
    'common.cancel'  : 'Annuler',
    'common.delete'  : 'Supprimer',
    'common.edit'    : 'Modifier',
    'common.create'  : 'Créer',
    'common.search'  : 'Rechercher',
    'common.loading' : 'Chargement...',
    'common.noData'  : 'Aucune donnée disponible',
    'common.confirm' : 'Confirmer',
    'common.back'    : 'Retour',
    'common.next'    : 'Suivant',
    'common.close'   : 'Fermer',
    'common.yes'     : 'Oui',
    'common.no'      : 'Non',
    'common.all'     : 'Tous',
    'common.status'  : 'Statut',
    'common.date'    : 'Date',
    'common.actions' : 'Actions',
    'common.name'    : 'Nom',
    'common.email'   : 'E-mail',
    'common.role'    : 'Rôle',
    'common.export'  : 'Exporter',
    'common.import'  : 'Importer',
    'common.filter'  : 'Filtrer',
    'common.reset'   : 'Réinitialiser',
    'common.view'    : 'Voir',
    'common.download': 'Télécharger',
    'auth.login'          : 'Connexion',
    'auth.register'       : "S'inscrire",
    'auth.email'          : 'Adresse e-mail',
    'auth.password'       : 'Mot de passe',
    'auth.confirmPassword': 'Confirmer le mot de passe',
    'auth.forgotPassword' : 'Mot de passe oublié ?',
    'auth.noAccount'      : 'Pas encore de compte ?',
    'auth.hasAccount'     : 'Déjà un compte ?',
    'auth.logout'         : 'Se déconnecter',
    'auth.rememberMe'     : 'Se souvenir de moi',
    'dashboard.welcome' : 'Bienvenue',
    'dashboard.stats'   : 'Statistiques',
    'dashboard.recent'  : 'Activité récente',
    'dashboard.overview': "Vue d'ensemble",
    'tests.title'     : 'Gestion des tests',
    'tests.create'    : 'Créer un test',
    'tests.aiCreate'  : 'Créer avec IA',
    'tests.duration'  : 'Durée',
    'tests.questions' : 'Questions',
    'tests.publish'   : 'Publier',
    'tests.draft'     : 'Brouillon',
    'tests.archived'  : 'Archivé',
    'tests.score'     : 'Score minimum',
    'candidates.title'            : 'Candidats',
    'candidates.invite'           : 'Inviter',
    'candidates.score'            : 'Score',
    'candidates.status.pending'   : 'En attente',
    'candidates.status.completed' : 'Terminé',
    'candidates.status.inProgress': 'En cours',
    'reports.title'    : 'Rapports',
    'reports.generate' : 'Générer un rapport',
    'reports.export'   : 'Exporter PDF',
    'settings.title'         : 'Paramètres',
    'settings.theme'         : 'Apparence',
    'settings.language'      : 'Langue',
    'settings.account'       : 'Compte',
    'settings.security'      : 'Sécurité',
    'settings.notifications' : 'Notifications',
  },

  en: {
    'theme.dark'  : 'Dark Mode',
    'theme.light' : 'Light Mode',
    'lang.switch' : 'Switch language',
    'chatbot.online'     : 'Online · Real-time responses',
    'chatbot.placeholder': 'Ask your question...',
    'chatbot.welcome'    : "👋 Hello! I'm **NeoBot**, your NeoStage AI assistant.\nI can help you create tests, analyze CVs, generate reports and more.",
    'chatbot.error'      : '⚠️ Server unavailable. Please try again.',
    'chatbot.suggestLabel': 'Frequent questions:',
    'chatbot.listening'  : 'Listening...',
    'chatbot.voice'      : 'Voice command',
    'chatbot.copy'       : 'Copy',
    'chatbot.speak'      : 'Read aloud',
    'chatbot.clear'      : 'Clear conversation',
    'chatbot.poweredBy'  : 'Powered by Gemini AI',
    'chatbot.copied'     : '✅ Copied!',
    'nav.dashboard'  : 'Dashboard',
    'nav.tests'      : 'Tests',
    'nav.candidates' : 'Candidates',
    'nav.reports'    : 'Reports',
    'nav.settings'   : 'Settings',
    'nav.logout'     : 'Logout',
    'nav.profile'    : 'My profile',
    'common.save'    : 'Save',
    'common.cancel'  : 'Cancel',
    'common.delete'  : 'Delete',
    'common.edit'    : 'Edit',
    'common.create'  : 'Create',
    'common.search'  : 'Search',
    'common.loading' : 'Loading...',
    'common.noData'  : 'No data available',
    'common.confirm' : 'Confirm',
    'common.back'    : 'Back',
    'common.next'    : 'Next',
    'common.close'   : 'Close',
    'common.yes'     : 'Yes',
    'common.no'      : 'No',
    'common.all'     : 'All',
    'common.status'  : 'Status',
    'common.date'    : 'Date',
    'common.actions' : 'Actions',
    'common.name'    : 'Name',
    'common.email'   : 'Email',
    'common.role'    : 'Role',
    'common.export'  : 'Export',
    'common.import'  : 'Import',
    'common.filter'  : 'Filter',
    'common.reset'   : 'Reset',
    'common.view'    : 'View',
    'common.download': 'Download',
    'auth.login'          : 'Login',
    'auth.register'       : 'Sign up',
    'auth.email'          : 'Email address',
    'auth.password'       : 'Password',
    'auth.confirmPassword': 'Confirm password',
    'auth.forgotPassword' : 'Forgot password?',
    'auth.noAccount'      : "Don't have an account?",
    'auth.hasAccount'     : 'Already have an account?',
    'auth.logout'         : 'Sign out',
    'auth.rememberMe'     : 'Remember me',
    'dashboard.welcome' : 'Welcome',
    'dashboard.stats'   : 'Statistics',
    'dashboard.recent'  : 'Recent activity',
    'dashboard.overview': 'Overview',
    'tests.title'     : 'Test management',
    'tests.create'    : 'Create test',
    'tests.aiCreate'  : 'Create with AI',
    'tests.duration'  : 'Duration',
    'tests.questions' : 'Questions',
    'tests.publish'   : 'Publish',
    'tests.draft'     : 'Draft',
    'tests.archived'  : 'Archived',
    'tests.score'     : 'Min. score',
    'candidates.title'            : 'Candidates',
    'candidates.invite'           : 'Invite',
    'candidates.score'            : 'Score',
    'candidates.status.pending'   : 'Pending',
    'candidates.status.completed' : 'Completed',
    'candidates.status.inProgress': 'In progress',
    'reports.title'    : 'Reports',
    'reports.generate' : 'Generate report',
    'reports.export'   : 'Export PDF',
    'settings.title'         : 'Settings',
    'settings.theme'         : 'Appearance',
    'settings.language'      : 'Language',
    'settings.account'       : 'Account',
    'settings.security'      : 'Security',
    'settings.notifications' : 'Notifications',
  },

  ar: {
    'theme.dark'  : 'الوضع الداكن',
    'theme.light' : 'الوضع الفاتح',
    'lang.switch' : 'تغيير اللغة',
    'chatbot.online'     : 'متصل · ردود فورية',
    'chatbot.placeholder': 'اطرح سؤالك...',
    'chatbot.welcome'    : '👋 مرحباً! أنا **NeoBot**، مساعدك الذكي في NeoStage.\nيمكنني مساعدتك في إنشاء الاختبارات وتحليل السير الذاتية وإنشاء التقارير والمزيد.',
    'chatbot.error'      : '⚠️ الخادم غير متاح. حاول مجدداً.',
    'chatbot.suggestLabel': 'أسئلة شائعة:',
    'chatbot.listening'  : 'أستمع إليك...',
    'chatbot.voice'      : 'أمر صوتي',
    'chatbot.copy'       : 'نسخ',
    'chatbot.speak'      : 'قراءة بصوت عالٍ',
    'chatbot.clear'      : 'مسح المحادثة',
    'chatbot.poweredBy'  : 'مدعوم من Gemini AI',
    'chatbot.copied'     : '✅ تم النسخ!',
    'nav.dashboard'  : 'لوحة التحكم',
    'nav.tests'      : 'الاختبارات',
    'nav.candidates' : 'المرشحون',
    'nav.reports'    : 'التقارير',
    'nav.settings'   : 'الإعدادات',
    'nav.logout'     : 'تسجيل الخروج',
    'nav.profile'    : 'ملفي الشخصي',
    'common.save'    : 'حفظ',
    'common.cancel'  : 'إلغاء',
    'common.delete'  : 'حذف',
    'common.edit'    : 'تعديل',
    'common.create'  : 'إنشاء',
    'common.search'  : 'بحث',
    'common.loading' : 'جارٍ التحميل...',
    'common.noData'  : 'لا توجد بيانات متاحة',
    'common.confirm' : 'تأكيد',
    'common.back'    : 'رجوع',
    'common.next'    : 'التالي',
    'common.close'   : 'إغلاق',
    'common.yes'     : 'نعم',
    'common.no'      : 'لا',
    'common.all'     : 'الكل',
    'common.status'  : 'الحالة',
    'common.date'    : 'التاريخ',
    'common.actions' : 'الإجراءات',
    'common.name'    : 'الاسم',
    'common.email'   : 'البريد الإلكتروني',
    'common.role'    : 'الدور',
    'common.export'  : 'تصدير',
    'common.import'  : 'استيراد',
    'common.filter'  : 'تصفية',
    'common.reset'   : 'إعادة تعيين',
    'common.view'    : 'عرض',
    'common.download': 'تنزيل',
    'auth.login'          : 'تسجيل الدخول',
    'auth.register'       : 'إنشاء حساب',
    'auth.email'          : 'عنوان البريد الإلكتروني',
    'auth.password'       : 'كلمة المرور',
    'auth.confirmPassword': 'تأكيد كلمة المرور',
    'auth.forgotPassword' : 'نسيت كلمة المرور؟',
    'auth.noAccount'      : 'ليس لديك حساب؟',
    'auth.hasAccount'     : 'هل لديك حساب بالفعل؟',
    'auth.logout'         : 'تسجيل الخروج',
    'auth.rememberMe'     : 'تذكرني',
    'dashboard.welcome' : 'مرحباً',
    'dashboard.stats'   : 'الإحصائيات',
    'dashboard.recent'  : 'النشاط الأخير',
    'dashboard.overview': 'نظرة عامة',
    'tests.title'     : 'إدارة الاختبارات',
    'tests.create'    : 'إنشاء اختبار',
    'tests.aiCreate'  : 'إنشاء بالذكاء الاصطناعي',
    'tests.duration'  : 'المدة',
    'tests.questions' : 'الأسئلة',
    'tests.publish'   : 'نشر',
    'tests.draft'     : 'مسودة',
    'tests.archived'  : 'مؤرشف',
    'tests.score'     : 'الدرجة الدنيا',
    'candidates.title'            : 'المرشحون',
    'candidates.invite'           : 'دعوة',
    'candidates.score'            : 'الدرجة',
    'candidates.status.pending'   : 'في الانتظار',
    'candidates.status.completed' : 'مكتمل',
    'candidates.status.inProgress': 'جارٍ',
    'reports.title'    : 'التقارير',
    'reports.generate' : 'إنشاء تقرير',
    'reports.export'   : 'تصدير PDF',
    'settings.title'         : 'الإعدادات',
    'settings.theme'         : 'المظهر',
    'settings.language'      : 'اللغة',
    'settings.account'       : 'الحساب',
    'settings.security'      : 'الأمان',
    'settings.notifications' : 'الإشعارات',
  }
};

// ── Drapeaux & voix ──
const langFlags  = { fr: '🇫🇷', en: '🇬🇧', ar: '🇸🇦' };
const langVoice  = { fr: 'fr-FR', en: 'en-US', ar: 'ar-SA' };
const langLocale = { fr: 'fr-FR', en: 'en-US', ar: 'ar-SA' };

// ── État réactif langue ──
const currentLang = ref(localStorage.getItem('app_lang') || 'fr');
const isRTL       = computed(() => currentLang.value === 'ar');

const t = (key) =>
  TRANSLATIONS[currentLang.value]?.[key]
  ?? TRANSLATIONS['fr']?.[key]
  ?? key;

const cycleLang = () => {
  const langs   = ['fr', 'en', 'ar'];
  currentLang.value = langs[(langs.indexOf(currentLang.value) + 1) % langs.length];
  localStorage.setItem('app_lang', currentLang.value);
  applyLangToDocument();
  loadStartSuggestions();
  if (chatMessages.value.length === 1 && chatMessages.value[0].role === 'ai') {
    chatMessages.value[0].text = t('chatbot.welcome');
  }
};

const applyLangToDocument = () => {
  document.documentElement.setAttribute('lang', currentLang.value);
  document.documentElement.setAttribute('dir', isRTL.value ? 'rtl' : 'ltr');
};

// ══════════════════════════════════════════════════════════
// THÈME DARK / LIGHT
// ══════════════════════════════════════════════════════════

const isDark = ref(localStorage.getItem('app_theme') === 'dark');

const applyTheme = (dark) => {
  const root = document.documentElement;
  root.setAttribute('data-theme', dark ? 'dark' : 'light');
  root.classList.toggle('dark', dark);
  root.classList.toggle('light', !dark);
  root.style.setProperty('color-scheme', dark ? 'dark' : 'light');
  localStorage.setItem('app_theme', dark ? 'dark' : 'light');
};

const toggleTheme = () => {
  isDark.value = !isDark.value;
  applyTheme(isDark.value);
};

const applyPageTheme = () => {
  const publicPages = ['/', '/login', '/register', '/pricing'];
  const isPublic    = publicPages.includes(route.path);
  if (isPublic) {
    isDark.value = false;
    applyTheme(false);
  } else {
    const saved  = localStorage.getItem('app_theme') === 'dark';
    isDark.value = saved;
    applyTheme(saved);
  }
};

// ══════════════════════════════════════════════════════════
// PROVIDE GLOBAL
// ══════════════════════════════════════════════════════════
provide('isDark',       isDark);
provide('toggleTheme',  toggleTheme);
provide('currentLang',  currentLang);
provide('t',            t);
provide('isRTL',        isRTL);

// ══════════════════════════════════════════════════════════
// CHATBOT — ÉTAT & LOGIQUE
// ══════════════════════════════════════════════════════════

const isChatOpen     = ref(false);
const chatInput      = ref('');
const isChatLoading  = ref(false);
const isListening    = ref(false);
const chatScroll     = ref(null);
const chatInputRef   = ref(null);
const chatMessages   = ref([]);
const unreadCount    = ref(0);
const startSuggestions = ref([]);
const sessionId      = ref(`session_${Date.now()}`);

const API_BASE = import.meta.env.VITE_API_BASE || 'http://127.0.0.1:8000';

const DEFAULT_SUGGESTIONS = {
  fr: ['Créer un test IA', 'Analyser un CV', 'Voir mes résultats', 'Générer un rapport'],
  en: ['Create an AI test', 'Analyze a CV', 'View my results', 'Generate a report'],
  ar: ['إنشاء اختبار ذكاء اصطناعي', 'تحليل سيرة ذاتية', 'عرض نتائجي', 'إنشاء تقرير'],
};

const formatMessage = (text) => {
  if (!text) return '';
  return text
    .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
    .replace(/\n/g, '<br>')
    .replace(/^(\d+\.\s)/gm, '<br>$1');
};

const now = () =>
  new Date().toLocaleTimeString(langLocale[currentLang.value] || 'fr-FR', {
    hour: '2-digit', minute: '2-digit'
  });

const scrollToBottom = async () => {
  await nextTick();
  chatScroll.value?.scrollTo({ top: chatScroll.value.scrollHeight, behavior: 'smooth' });
};

const loadStartSuggestions = async () => {
  try {
    const role = authStore.user?.role || 'Recruteur';
    const res  = await fetch(`${API_BASE}/ia/chat/suggestions?role=${role}&lang=${currentLang.value}`);
    const data = await res.json();
    startSuggestions.value = data.suggestions || DEFAULT_SUGGESTIONS[currentLang.value];
  } catch {
    startSuggestions.value = DEFAULT_SUGGESTIONS[currentLang.value] || DEFAULT_SUGGESTIONS.fr;
  }
};

const sendSuggestion = (text) => {
  chatInput.value = text;
  handleChat(false);
};

const toggleChat = async () => {
  isChatOpen.value = !isChatOpen.value;
  if (isChatOpen.value) {
    unreadCount.value = 0;
    if (chatMessages.value.length === 0) {
      chatMessages.value.push({ role: 'ai', text: t('chatbot.welcome'), time: now(), suggestions: [] });
      await loadStartSuggestions();
    }
    await nextTick();
    chatInputRef.value?.focus();
    await scrollToBottom();
  }
};

const handleChat = async (isVocal = false) => {
  if (!chatInput.value.trim() || isChatLoading.value) return;
  const userText = chatInput.value.trim();
  chatMessages.value.push({ role: 'user', text: userText, time: now() });
  chatInput.value    = '';
  isChatLoading.value = true;
  await scrollToBottom();
  try {
    const fd = new FormData();
    fd.append('message',    userText);
    fd.append('role',       authStore.user?.role || 'Recruteur');
    fd.append('lang',       'auto');
    fd.append('session_id', sessionId.value);
    const response = await fetch(`${API_BASE}/ia/chat`, { method: 'POST', body: fd });
    const data     = await response.json();
    const reply    = data.response || data.reply || t('chatbot.error');
    chatMessages.value.push({
      role: 'ai', text: reply, time: now(),
      suggestions: data.suggestions || [],
      source: data.source || ''
    });
    if (isVocal) speak(reply);
    if (!isChatOpen.value) unreadCount.value++;
  } catch {
    chatMessages.value.push({ role: 'ai', text: t('chatbot.error'), time: now(), suggestions: [] });
  } finally {
    isChatLoading.value = false;
    await scrollToBottom();
  }
};

const clearChat = async () => {
  try {
    const fd = new FormData();
    fd.append('session_id', sessionId.value);
    await fetch(`${API_BASE}/ia/chat/reset`, { method: 'POST', body: fd });
  } catch {}
  chatMessages.value = [];
  sessionId.value    = `session_${Date.now()}`;
  chatMessages.value.push({ role: 'ai', text: t('chatbot.welcome'), time: now(), suggestions: [] });
  await loadStartSuggestions();
};

const speak = (text) => {
  window.speechSynthesis.cancel();
  const clean = text.replace(/<[^>]*>/g, '').replace(/\*\*/g, '');
  const msg   = new SpeechSynthesisUtterance(clean);
  msg.lang    = langVoice[currentLang.value] || 'fr-FR';
  msg.rate    = 0.95;
  window.speechSynthesis.speak(msg);
};

const copyText = (text) => {
  const clean = text.replace(/<[^>]*>/g, '').replace(/\*\*/g, '');
  navigator.clipboard.writeText(clean).catch(() => {});
};

const toggleVoiceRecognition = () => {
  const SR = window.SpeechRecognition || window.webkitSpeechRecognition;
  if (!SR) { alert('Votre navigateur ne supporte pas la reconnaissance vocale.'); return; }
  if (isListening.value) { isListening.value = false; return; }
  const recognition  = new SR();
  recognition.lang   = langVoice[currentLang.value] || 'fr-FR';
  recognition.onstart  = () => { isListening.value = true; };
  recognition.onend    = () => { isListening.value = false; };
  recognition.onerror  = () => { isListening.value = false; };
  recognition.onresult = (e) => {
    chatInput.value = e.results[0][0].transcript;
    handleChat(true);
  };
  recognition.start();
};

// ══════════════════════════════════════════════════════════
// WATCHERS THÈME & LANGUE
// ══════════════════════════════════════════════════════════

watch(() => route.path, applyPageTheme);

watch(() => authStore.user?.themePreference, (val) => {
  if (val) { isDark.value = val === 'dark'; applyTheme(isDark.value); }
});

// Synchroniser thème et langue entre onglets
window.addEventListener('storage', (e) => {
  if (e.key === 'app_theme') {
    isDark.value = e.newValue === 'dark';
    applyTheme(isDark.value);
  }
  if (e.key === 'app_lang') {
    currentLang.value = e.newValue || 'fr';
    applyLangToDocument();
  }
});
</script>

<style>
/* ════════════════════════════════════════════════════════════
   VARIABLES CSS GLOBALES
   ════════════════════════════════════════════════════════════ */

:root,
:root[data-theme="light"],
.theme-light {
  --bg-page:          #f1f5f9;
  --bg-card:          #ffffff;
  --bg-sidebar:       #ffffff;
  --bg-input:         #f8fafc;
  --bg-hover:         #f1f5f9;
  --bg-overlay:       rgba(15, 23, 42, 0.5);
  --text-main:        #1e293b;
  --text-muted:       #64748b;
  --text-light:       #94a3b8;
  --text-inverse:     #ffffff;
  --border-color:     #e2e8f0;
  --border-focus:     rgba(245, 158, 11, 0.5);
  --primary:          #f59e0b;
  --primary-dark:     #d97706;
  --primary-light:    rgba(245, 158, 11, 0.12);
  --secondary:        #0f172a;
  --accent:           #3b82f6;
  --success:          #10b981;
  --success-bg:       rgba(16, 185, 129, 0.1);
  --warning:          #f59e0b;
  --warning-bg:       rgba(245, 158, 11, 0.1);
  --danger:           #ef4444;
  --danger-bg:        rgba(239, 68, 68, 0.1);
  --info:             #3b82f6;
  --info-bg:          rgba(59, 130, 246, 0.1);
  --shadow-xs:        0 1px 3px rgba(0,0,0,0.04);
  --shadow-sm:        0 2px 8px rgba(0,0,0,0.06);
  --shadow-md:        0 8px 30px rgba(0,0,0,0.10);
  --shadow-lg:        0 20px 60px rgba(0,0,0,0.15);
  --shadow-primary:   0 4px 14px rgba(245, 158, 11, 0.35);
  --radius-xs:        4px;
  --radius-sm:        8px;
  --radius-md:        12px;
  --radius-lg:        16px;
  --radius-xl:        24px;
  --radius-full:      9999px;
  --transition-fast:  all 0.15s ease;
  --transition:       all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
  --transition-slow:  all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  color-scheme: light;
}

:root[data-theme="dark"],
.theme-dark {
  --bg-page:          #0f172a;
  --bg-card:          #1e293b;
  --bg-sidebar:       #111827;
  --bg-input:         #0f172a;
  --bg-hover:         #243044;
  --bg-overlay:       rgba(0, 0, 0, 0.7);
  --text-main:        #f1f5f9;
  --text-muted:       #94a3b8;
  --text-light:       #64748b;
  --text-inverse:     #0f172a;
  --border-color:     #334155;
  --border-focus:     rgba(245, 158, 11, 0.6);
  --primary:          #f59e0b;
  --primary-dark:     #d97706;
  --primary-light:    rgba(245, 158, 11, 0.15);
  --secondary:        #1e293b;
  --accent:           #60a5fa;
  --success:          #34d399;
  --success-bg:       rgba(52, 211, 153, 0.12);
  --warning:          #fbbf24;
  --warning-bg:       rgba(251, 191, 36, 0.12);
  --danger:           #f87171;
  --danger-bg:        rgba(248, 113, 113, 0.12);
  --info:             #60a5fa;
  --info-bg:          rgba(96, 165, 250, 0.12);
  --shadow-xs:        0 1px 3px rgba(0,0,0,0.2);
  --shadow-sm:        0 2px 8px rgba(0,0,0,0.3);
  --shadow-md:        0 8px 30px rgba(0,0,0,0.4);
  --shadow-lg:        0 20px 60px rgba(0,0,0,0.5);
  --shadow-primary:   0 4px 14px rgba(245, 158, 11, 0.25);
  color-scheme: dark;
}

/* ════════════════════════════════════════════════════════════
   RESET & BASE
   ════════════════════════════════════════════════════════════ */

*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

html { transition: color-scheme 0.3s ease; }

body {
  background-color:       var(--bg-page) !important;
  color:                  var(--text-main) !important;
  font-family:            'Segoe UI', system-ui, -apple-system, sans-serif;
  font-size:              15px;
  line-height:            1.6;
  min-height:             100vh;
  -webkit-font-smoothing: antialiased;
  transition:             background-color 0.35s ease, color 0.35s ease;
}

#app-container, .app-root {
  background-color: var(--bg-page);
  color:            var(--text-main);
  min-height:       100vh;
  transition:       background-color 0.35s ease, color 0.35s ease;
}

/* ════════════════════════════════════════════════════════════
   SUPPORT RTL — Arabe
   ════════════════════════════════════════════════════════════ */

[dir="rtl"] .chatbot-wrapper { right: auto; left: 28px; }
[dir="rtl"] .chat-window     { right: auto; left: 0; }
[dir="rtl"] .chat-msg.user   { flex-direction: row; }
[dir="rtl"] .chat-msg.ai     { flex-direction: row-reverse; }
[dir="rtl"] .msg-bubble      { align-items: flex-end; }
[dir="rtl"] .header-actions  { flex-direction: row-reverse; }

/* ════════════════════════════════════════════════════════════
   CARDS & SURFACES
   ════════════════════════════════════════════════════════════ */

.card,
.panel-luxe,
.luxury-bento-card,
.modal-content,
.modal-dialog .modal-content,
.offcanvas,
.offcanvas-body,
.dropdown-menu,
.list-group-item,
.accordion-item,
.accordion-button,
.popover,
.tooltip-inner {
  background-color: var(--bg-card) !important;
  color:            var(--text-main) !important;
  border-color:     var(--border-color) !important;
  transition:       background-color 0.3s ease, color 0.3s ease, border-color 0.3s ease;
}

.accordion-button:not(.collapsed) {
  background-color: var(--bg-hover) !important;
  color:            var(--primary) !important;
  box-shadow:       inset 0 -1px 0 var(--border-color) !important;
}
[data-theme="dark"] .accordion-button::after {
  filter: invert(1) brightness(1.5);
}

/* ════════════════════════════════════════════════════════════
   SIDEBAR & NAVBAR
   ════════════════════════════════════════════════════════════ */

.sidebar, .sidebar-wrapper, .app-sidebar,
.navbar, .nav-container, .app-navbar,
.bg-white, [class*="bg-white"] {
  background-color: var(--bg-card) !important;
  color:            var(--text-main) !important;
  border-color:     var(--border-color) !important;
  transition:       background-color 0.3s ease, border-color 0.3s ease;
}

/* ════════════════════════════════════════════════════════════
   TYPOGRAPHIE
   ════════════════════════════════════════════════════════════ */

h1, h2, h3, h4, h5, h6 { color: var(--text-main) !important; transition: color 0.3s ease; }
p, span, label, li, td, th, small, strong, div { color: inherit; }

.text-muted, .text-gray-500, .text-slate-500 { color: var(--text-muted) !important; }
.text-primary { color: var(--primary) !important; }
.text-success { color: var(--success) !important; }
.text-danger  { color: var(--danger) !important; }
.text-warning { color: var(--warning) !important; }
.text-info    { color: var(--info) !important; }

/* ════════════════════════════════════════════════════════════
   NAVIGATION
   ════════════════════════════════════════════════════════════ */

.nav-link { color: var(--text-muted) !important; transition: color 0.2s ease; }
.nav-link.active, .nav-link:hover { color: var(--primary) !important; }

.nav-tabs { border-color: var(--border-color) !important; }
.nav-tabs .nav-link { color: var(--text-muted) !important; border-color: transparent !important; }
.nav-tabs .nav-link.active {
  background-color: var(--bg-card) !important;
  border-color:     var(--border-color) var(--border-color) var(--bg-card) !important;
  color:            var(--primary) !important;
}

/* ════════════════════════════════════════════════════════════
   INPUTS & FORMULAIRES
   ════════════════════════════════════════════════════════════ */

input, select, textarea,
.form-control, .form-select, .input-group-text {
  background-color: var(--bg-input) !important;
  color:            var(--text-main) !important;
  border-color:     var(--border-color) !important;
  transition:       background-color 0.3s ease, color 0.3s ease, border-color 0.3s ease;
}

input::placeholder, textarea::placeholder { color: var(--text-light) !important; }

input:focus, select:focus, textarea:focus,
.form-control:focus, .form-select:focus {
  background-color: var(--bg-input) !important;
  color:            var(--text-main) !important;
  border-color:     var(--primary) !important;
  box-shadow:       0 0 0 3px var(--primary-light) !important;
  outline:          none;
}

.form-label, .form-check-label { color: var(--text-main) !important; }

.form-check-input {
  background-color: var(--bg-input) !important;
  border-color:     var(--border-color) !important;
}
.form-check-input:checked {
  background-color: var(--primary) !important;
  border-color:     var(--primary) !important;
}
.form-switch .form-check-input:checked { background-color: var(--primary) !important; }

/* ════════════════════════════════════════════════════════════
   TABLES
   ════════════════════════════════════════════════════════════ */

.table { color: var(--text-main) !important; border-color: var(--border-color) !important; }
.table thead th {
  border-color:     var(--border-color) !important;
  background-color: var(--bg-card) !important;
  color:            var(--text-muted) !important;
  font-weight:      600;
  font-size:        0.78rem;
  text-transform:   uppercase;
  letter-spacing:   0.05em;
}
.table tbody tr { border-color: var(--border-color) !important; background-color: transparent !important; }
.table tbody tr:hover { background-color: var(--bg-hover) !important; }
.table-striped > tbody > tr:nth-of-type(odd) { background-color: var(--bg-hover) !important; }

/* ════════════════════════════════════════════════════════════
   MODALS
   ════════════════════════════════════════════════════════════ */

.modal-backdrop     { background-color: var(--bg-overlay) !important; }
.modal-header, .modal-footer { border-color: var(--border-color) !important; }
.modal-title        { color: var(--text-main) !important; }
[data-theme="dark"] .btn-close { filter: invert(1) grayscale(100%) brightness(200%); }

/* ════════════════════════════════════════════════════════════
   DROPDOWNS
   ════════════════════════════════════════════════════════════ */

.dropdown-item { color: var(--text-main) !important; }
.dropdown-item:hover, .dropdown-item:focus {
  background-color: var(--bg-hover) !important;
  color: var(--text-main) !important;
}
.dropdown-divider { border-color: var(--border-color) !important; }

/* ════════════════════════════════════════════════════════════
   BADGES
   ════════════════════════════════════════════════════════════ */

.badge { font-weight: 600; }
.badge-subtle-amber { background: rgba(251,191,36,0.15) !important; color: #d97706 !important; }
.badge-subtle-green { background: var(--success-bg) !important;     color: var(--success) !important; }
.badge-subtle-red   { background: var(--danger-bg) !important;       color: var(--danger) !important; }
.badge-subtle-blue  { background: var(--info-bg) !important;         color: var(--info) !important; }
.badge-subtle-gray  { background: var(--bg-hover) !important;        color: var(--text-muted) !important; }
[data-theme="dark"] .badge-subtle-amber { background: rgba(251,191,36,0.2) !important; color: #fbbf24 !important; }

/* ════════════════════════════════════════════════════════════
   BOUTONS BOOTSTRAP OVERRIDE
   ════════════════════════════════════════════════════════════ */

.btn-light {
  background-color: var(--bg-hover) !important;
  color:            var(--text-main) !important;
  border-color:     var(--border-color) !important;
}
.btn-outline-secondary {
  color:        var(--text-muted) !important;
  border-color: var(--border-color) !important;
}
.btn-outline-secondary:hover {
  background-color: var(--bg-hover) !important;
  color:            var(--text-main) !important;
}

/* ════════════════════════════════════════════════════════════
   ALERTS
   ════════════════════════════════════════════════════════════ */

.alert         { border-color: var(--border-color) !important; }
.alert-info    { background: var(--info-bg) !important;    color: var(--info) !important; }
.alert-success { background: var(--success-bg) !important; color: var(--success) !important; }
.alert-warning { background: var(--warning-bg) !important; color: var(--warning) !important; }
.alert-danger  { background: var(--danger-bg) !important;  color: var(--danger) !important; }

/* ════════════════════════════════════════════════════════════
   PROGRESS
   ════════════════════════════════════════════════════════════ */

.progress { background-color: var(--bg-hover) !important; border-radius: var(--radius-full) !important; }

/* ════════════════════════════════════════════════════════════
   PAGINATION
   ════════════════════════════════════════════════════════════ */

.page-link {
  background-color: var(--bg-card) !important;
  border-color:     var(--border-color) !important;
  color:            var(--text-main) !important;
}
.page-link:hover              { background-color: var(--bg-hover) !important; }
.page-item.active .page-link  { background-color: var(--primary) !important; border-color: var(--primary) !important; color: #fff !important; }
.page-item.disabled .page-link { opacity: 0.45; }

/* ════════════════════════════════════════════════════════════
   UTILITAIRES GLOBAUX
   ════════════════════════════════════════════════════════════ */

hr, .divider           { border-color: var(--border-color) !important; opacity: 1; }
.bg-success-soft       { background-color: var(--success-bg) !important; }
.bg-danger-soft        { background-color: var(--danger-bg) !important; }
.bg-warning-soft       { background-color: var(--warning-bg) !important; }
.bg-info-soft          { background-color: var(--info-bg) !important; }
.bg-card               { background-color: var(--bg-card) !important; }
.bg-page               { background-color: var(--bg-page) !important; }
.border-theme          { border-color: var(--border-color) !important; }
.text-theme-muted      { color: var(--text-muted) !important; }
.text-theme-light      { color: var(--text-light) !important; }
.shadow-theme-sm       { box-shadow: var(--shadow-sm) !important; }
.shadow-theme-md       { box-shadow: var(--shadow-md) !important; }
.shadow-theme-lg       { box-shadow: var(--shadow-lg) !important; }

::selection  { background: var(--primary-light); color: var(--text-main); }
:focus-visible { outline: 2px solid var(--primary); outline-offset: 2px; }

/* ════════════════════════════════════════════════════════════
   SCROLLBARS GLOBALES
   ════════════════════════════════════════════════════════════ */

* { scrollbar-width: thin; scrollbar-color: var(--border-color) transparent; }
*::-webkit-scrollbar        { width: 6px; height: 6px; }
*::-webkit-scrollbar-track  { background: transparent; }
*::-webkit-scrollbar-thumb  { background: var(--border-color); border-radius: 6px; }
*::-webkit-scrollbar-thumb:hover { background: var(--text-light); }

/* ════════════════════════════════════════════════════════════
   TRANSITION DE PAGE
   ════════════════════════════════════════════════════════════ */

.page-fade-enter-active, .page-fade-leave-active { transition: opacity 0.28s ease, transform 0.28s ease; }
.page-fade-enter-from, .page-fade-leave-to       { opacity: 0; transform: translateY(6px); }

/* ════════════════════════════════════════════════════════════
   CHATBOT — WRAPPER & BULLE
   ════════════════════════════════════════════════════════════ */

.chatbot-wrapper {
  position: fixed; bottom: 28px; right: 28px;
  z-index: 99999;
  display: flex; flex-direction: column; align-items: flex-end;
}

.chat-bubble {
  width: 62px; height: 62px;
  background: var(--primary); color: #fff;
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; font-size: 1.35rem;
  box-shadow: var(--shadow-primary);
  transition: var(--transition); position: relative;
}
.chat-bubble:hover { transform: scale(1.08); }
.chat-bubble.active { background: var(--danger); box-shadow: 0 8px 28px rgba(239,68,68,0.45); }

.bubble-ring {
  position: absolute; width: 100%; height: 100%;
  border-radius: 50%; border: 2px solid var(--primary);
  animation: ring-pulse 2.5s ease-out infinite; pointer-events: none;
}
.chat-bubble.active .bubble-ring { border-color: var(--danger); animation: none; }

.unread-badge {
  position: absolute; top: -4px; right: -4px;
  background: var(--danger); color: #fff;
  border-radius: 50%; width: 20px; height: 20px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.7rem; font-weight: 700; border: 2px solid var(--bg-page);
}

/* ════════════════════════════════════════════════════════════
   CHATBOT — FENÊTRE
   ════════════════════════════════════════════════════════════ */

.chat-window {
  position: absolute; bottom: 80px; right: 0;
  width: 370px; max-height: 585px;
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  display: flex; flex-direction: column;
  border: 1px solid var(--border-color);
  overflow: hidden; box-shadow: var(--shadow-lg);
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

.chat-header {
  background: linear-gradient(135deg, #0f172a 0%, #1e3a5f 100%);
  padding: 14px 16px;
  display: flex; align-items: center; justify-content: space-between;
  flex-shrink: 0;
}

.header-left { display: flex; align-items: center; gap: 10px; }

.bot-avatar {
  width: 40px; height: 40px; border-radius: 12px;
  background: rgba(245,158,11,0.2); border: 1.5px solid rgba(245,158,11,0.4);
  display: flex; align-items: center; justify-content: center;
  color: var(--primary); font-size: 1.1rem; position: relative;
}
.avatar-pulse {
  position: absolute; top: -3px; right: -3px;
  width: 10px; height: 10px;
  background: #10b981; border-radius: 50%;
  border: 2px solid #0f172a;
  animation: dot-pulse 2s ease infinite;
}
.header-info h6    { margin: 0; color: #fff; font-size: 0.9rem; font-weight: 700; }
.status-row        { display: flex; align-items: center; gap: 5px; }
.status-row small  { color: #6ee7b7; font-size: 0.7rem; }
.dot-online        { width: 7px; height: 7px; background: #10b981; border-radius: 50%; animation: dot-pulse 2s ease infinite; }

.header-actions { display: flex; align-items: center; gap: 6px; }
.btn-action {
  width: 30px; height: 30px; border-radius: 8px;
  border: 1px solid rgba(255,255,255,0.15); background: rgba(255,255,255,0.08);
  color: rgba(255,255,255,0.8); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.75rem; transition: var(--transition);
}
.btn-action:hover     { background: rgba(255,255,255,0.18); color: #fff; }
.btn-lang             { font-size: 1rem; }
.btn-close-chat:hover { background: rgba(239,68,68,0.35) !important; }

.loading-bar      { height: 2px; background: var(--border-color); flex-shrink: 0; overflow: hidden; }
.loading-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--primary), #f97316, var(--primary));
  background-size: 200% 100%;
  animation: loading-slide 1.2s linear infinite;
}

.chat-body {
  flex: 1; overflow-y: auto; padding: 16px 14px;
  background: var(--bg-page);
  display: flex; flex-direction: column; gap: 8px;
  transition: background-color 0.3s ease;
}

.start-suggestions  { margin-bottom: 6px; }
.suggestions-label  { font-size: 0.72rem; color: var(--text-muted); margin-bottom: 8px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; }
.suggestion-chips   { display: flex; flex-wrap: wrap; gap: 6px; }

.chip {
  padding: 5px 11px; border-radius: 20px;
  border: 1.5px solid var(--primary); background: transparent; color: var(--primary);
  font-size: 0.75rem; font-weight: 600; cursor: pointer; transition: var(--transition);
}
.chip:hover { background: var(--primary); color: #fff; transform: translateY(-1px); }

.chat-msg      { display: flex; gap: 8px; animation: msgFadeIn 0.3s ease; }
.chat-msg.user { flex-direction: row-reverse; }

.msg-avatar {
  width: 30px; height: 30px; border-radius: 9px;
  background: var(--primary-light); border: 1px solid rgba(245,158,11,0.25);
  display: flex; align-items: center; justify-content: center;
  color: var(--primary); font-size: 0.75rem; flex-shrink: 0; align-self: flex-end;
}

.msg-bubble  { display: flex; flex-direction: column; gap: 4px; max-width: 80%; }

.msg-content { padding: 9px 13px; border-radius: 14px; font-size: 0.85rem; line-height: 1.55; word-break: break-word; }
.chat-msg.ai .msg-content {
  background: var(--bg-card); color: var(--text-main);
  border-radius: 4px 14px 14px 14px; border: 1px solid var(--border-color); box-shadow: var(--shadow-sm);
}
.chat-msg.user .msg-content {
  background: linear-gradient(135deg, var(--primary), var(--primary-dark));
  color: #fff; border-radius: 14px 4px 14px 14px; box-shadow: var(--shadow-primary);
}

.source-badge {
  font-size: 0.65rem; color: var(--text-muted);
  padding: 1px 6px; background: var(--bg-page);
  border-radius: 6px; border: 1px solid var(--border-color); align-self: flex-start;
}

.msg-actions { display: flex; gap: 4px; opacity: 0; transition: opacity 0.2s; }
.chat-msg:hover .msg-actions { opacity: 1; }
.msg-action-btn {
  width: 22px; height: 22px; border-radius: 6px;
  border: 1px solid var(--border-color); background: var(--bg-card);
  color: var(--text-muted); font-size: 0.65rem; cursor: pointer;
  display: flex; align-items: center; justify-content: center; transition: var(--transition);
}
.msg-action-btn:hover { background: var(--primary); color: #fff; border-color: var(--primary); }

.follow-suggestions { display: flex; flex-wrap: wrap; gap: 5px; margin-top: 3px; }
.follow-chip {
  padding: 3px 9px; border-radius: 14px;
  border: 1px solid var(--border-color); background: var(--bg-page);
  color: var(--text-muted); font-size: 0.72rem; cursor: pointer; transition: var(--transition);
}
.follow-chip:hover { border-color: var(--primary); color: var(--primary); }

.msg-time { font-size: 0.62rem; color: var(--text-light); align-self: flex-end; }

.typing-dots { display: flex; align-items: center; gap: 4px; padding: 4px 2px !important; }
.typing-dots span { width: 7px; height: 7px; background: var(--text-muted); border-radius: 50%; animation: typing-bounce 1.4s ease infinite; }
.typing-dots span:nth-child(2) { animation-delay: 0.2s; }
.typing-dots span:nth-child(3) { animation-delay: 0.4s; }

.chat-footer {
  border-top: 1px solid var(--border-color); padding: 10px 12px 8px;
  background: var(--bg-card); flex-shrink: 0;
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

.voice-active-bar {
  display: flex; align-items: center; gap: 8px;
  padding: 6px 10px; margin-bottom: 8px;
  background: var(--danger-bg); border: 1px solid rgba(239,68,68,0.2);
  border-radius: 8px; color: var(--danger); font-size: 0.78rem; font-weight: 600;
}
.voice-waves { display: flex; align-items: center; gap: 2px; margin-left: auto; }
.voice-waves span { width: 3px; border-radius: 3px; background: var(--danger); animation: voice-wave 0.7s ease infinite; }
.voice-waves span:nth-child(1) { height: 6px; }
.voice-waves span:nth-child(2) { height: 12px; animation-delay: 0.1s; }
.voice-waves span:nth-child(3) { height: 18px; animation-delay: 0.2s; }
.voice-waves span:nth-child(4) { height: 12px; animation-delay: 0.3s; }
.voice-waves span:nth-child(5) { height: 6px;  animation-delay: 0.4s; }

.input-row     { display: flex; align-items: center; gap: 7px; }
.input-wrapper { flex: 1; position: relative; }

.chat-input {
  width: 100%; padding: 9px 34px 9px 13px; border-radius: 12px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-input) !important; color: var(--text-main) !important;
  font-size: 0.85rem; outline: none; transition: var(--transition);
}
.chat-input:focus        { border-color: var(--primary); box-shadow: 0 0 0 3px var(--primary-light); }
.chat-input::placeholder { color: var(--text-light) !important; }
.chat-input:disabled     { opacity: 0.6; cursor: not-allowed; }
.char-count { position: absolute; right: 10px; top: 50%; transform: translateY(-50%); font-size: 0.65rem; color: var(--text-light); }

.btn-mic {
  width: 38px; height: 38px; border-radius: 10px;
  border: 1.5px solid var(--border-color); background: var(--bg-input);
  color: var(--text-muted); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; transition: var(--transition); flex-shrink: 0;
}
.btn-mic:hover { border-color: var(--primary); color: var(--primary); }
.mic-active    { background: var(--danger) !important; border-color: var(--danger) !important; color: #fff !important; animation: pulse-red 1.5s infinite; }

.btn-send {
  width: 38px; height: 38px; border-radius: 10px;
  border: none; background: var(--primary); color: #fff; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; transition: var(--transition); flex-shrink: 0;
  box-shadow: var(--shadow-primary);
}
.btn-send:hover:not(:disabled) { background: var(--primary-dark); transform: scale(1.05); }
.btn-send:disabled             { opacity: 0.45; cursor: not-allowed; transform: none; }

.footer-note {
  margin-top: 7px; text-align: center; font-size: 0.64rem; color: var(--text-light);
  display: flex; align-items: center; justify-content: center; gap: 4px;
}

/* ════════════════════════════════════════════════════════════
   TRANSITIONS CHATBOT
   ════════════════════════════════════════════════════════════ */

.chat-slide-enter-active    { animation: chatSlideIn 0.32s cubic-bezier(0.34,1.56,0.64,1); }
.chat-slide-leave-active    { animation: chatSlideOut 0.22s ease-in forwards; }
.bubble-bounce-enter-active { animation: bubbleIn 0.4s cubic-bezier(0.34,1.56,0.64,1); }
.bubble-bounce-leave-active { animation: bubbleOut 0.2s ease forwards; }

/* ════════════════════════════════════════════════════════════
   KEYFRAMES
   ════════════════════════════════════════════════════════════ */

@keyframes ring-pulse    { 0% { transform: scale(1); opacity: 0.6; } 100% { transform: scale(1.7); opacity: 0; } }
@keyframes dot-pulse     { 0%, 100% { opacity: 1; } 50% { opacity: 0.4; } }
@keyframes loading-slide { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
@keyframes msgFadeIn     { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: translateY(0); } }
@keyframes typing-bounce { 0%, 60%, 100% { transform: translateY(0); } 30% { transform: translateY(-5px); } }
@keyframes pulse-red     { 0% { box-shadow: 0 0 0 0 rgba(239,68,68,0.4); } 100% { box-shadow: 0 0 0 10px rgba(239,68,68,0); } }
@keyframes voice-wave    { 0%, 100% { transform: scaleY(0.5); } 50% { transform: scaleY(1); } }
@keyframes chatSlideIn   { from { opacity: 0; transform: translateY(20px) scale(0.95); } to { opacity: 1; transform: translateY(0) scale(1); } }
@keyframes chatSlideOut  { from { opacity: 1; transform: translateY(0) scale(1); } to { opacity: 0; transform: translateY(16px) scale(0.95); } }
@keyframes bubbleIn      { from { transform: scale(0); opacity: 0; } to { transform: scale(1); opacity: 1; } }
@keyframes bubbleOut     { from { transform: scale(1); opacity: 1; } to { transform: scale(0); opacity: 0; } }

/* ════════════════════════════════════════════════════════════
   RESPONSIVE — MOBILE FIRST
   ════════════════════════════════════════════════════════════ */

@media (max-width: 768px) {
  .chat-window {
    width: calc(100vw - 20px); right: -14px;
    height: 78vh; bottom: 78px; border-radius: var(--radius-lg);
  }
  .chatbot-wrapper { bottom: 16px; right: 16px; }
}

@media (max-width: 480px) {
  .chat-window {
    width: 100vw; right: -14px;
    height: 85vh; bottom: 70px;
    border-radius: var(--radius-lg) var(--radius-lg) 0 0;
  }
  .chatbot-wrapper { bottom: 14px; right: 14px; }
  .chat-bubble     { width: 54px; height: 54px; font-size: 1.1rem; }
}
</style>