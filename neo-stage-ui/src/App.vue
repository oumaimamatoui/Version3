<template>
  <div id="app-container" :class="['app-root', isDark ? 'theme-dark' : 'theme-light']">
    <router-view v-slot="{ Component }">
      <transition name="page-fade" mode="out-in">
        <component :is="Component" :key="route.path" />
      </transition>
    </router-view>

    <!-- NEOBOT — ASSISTANT IA UNIVERSEL (Extracted Component) -->
    <NeoBot v-if="authStore.isAuthenticated && route.meta?.requiresAuth" />
  </div>
</template>

<script setup>
import { watch, onMounted, onUnmounted, ref, computed, provide } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import { useNotificationStore } from '@/stores/notification';
import NeoBot from '@/components/NeoBot.vue';

const route      = useRoute();
const authStore  = useAuthStore();
const notifStore = useNotificationStore();

// ══════════════════════════════════════════════════════════
// SIGNALR
// ══════════════════════════════════════════════════════════
onMounted(async () => {
  applyPageTheme();
  applyLangToDocument();
  if (authStore.isAuthenticated) {
    await notifStore.connect();
  }
});

watch(
  () => authStore.isAuthenticated,
  async (isAuth) => {
    if (isAuth) await notifStore.connect();
    else await notifStore.disconnect();
  }
);

onUnmounted(async () => {
  await notifStore.disconnect();
});

// ══════════════════════════════════════════════════════════
// I18N
// ══════════════════════════════════════════════════════════
import fr from '@/locales/fr';
import en from '@/locales/en';
import ar from '@/locales/ar';

const TRANSLATIONS = { fr, en, ar };

const currentLang = ref(localStorage.getItem('app_lang') || 'fr');

// isRTL utilisé pour les composants enfants si nécessaire
const isRTL = computed(() => currentLang.value === 'ar');

const t = (key) =>
  TRANSLATIONS[currentLang.value]?.[key]
  ?? TRANSLATIONS['fr']?.[key]
  ?? key;

const cycleLang = () => {
  const langs = ['fr', 'en', 'ar'];
  currentLang.value = langs[(langs.indexOf(currentLang.value) + 1) % langs.length];
  localStorage.setItem('app_lang', currentLang.value);
  applyLangToDocument();
};

const applyLangToDocument = () => {
  // Met à jour l'attribut lang sur <html> pour l'accessibilité
  document.documentElement.setAttribute('lang', currentLang.value);
};

// ══════════════════════════════════════════════════════════
// THÈME
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
provide('cycleLang',    cycleLang);

// ══════════════════════════════════════════════════════════
// WATCHERS & LISTENERS
// ══════════════════════════════════════════════════════════
watch(() => route.path, applyPageTheme);

watch(() => authStore.user?.themePreference, (val) => {
  if (val) { isDark.value = val === 'dark'; applyTheme(isDark.value); }
});

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
/* ─── STYLES GLOBAUX & RESET ─── */
html, body {
  margin: 0;
  padding: 0;
  width: 100%;
  min-height: 100vh;
  font-family: 'Plus Jakarta Sans', sans-serif;
  overflow-x: hidden;
  background-color: #f8fafc;
}

#app, #app-container, .app-root {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

/* ─── DESIGN TOKENS GLOBAUX ─── */
:root {
  --surface: #ffffff;
  --surface2: #f8fafc;
  --bdr: #e2e8f0;
  --text: #1e293b;
  --text2: #475569;
  --text3: #94a3b8;
  --amber: #f59e0b;
}
[data-theme="dark"] :root {
  --surface: #161b22;
  --surface2: #1c2128;
  --bdr: rgba(255,255,255,0.08);
  --text: #f0f6fc;
  --text2: #8b949e;
  --text3: #6e7681;
  --amber: #f59e0b;
}

/* Thème sombre global pour le body */
html[data-theme="dark"] body, 
body.dark {
  background-color: #0d1117;
  color: #f0f6fc;
}

/* Custom scrollbar globale */
::-webkit-scrollbar { width: 8px; height: 8px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }
html[data-theme="dark"] ::-webkit-scrollbar-thumb { background: #30363d; }
::-webkit-scrollbar-thumb:hover { background: #94a3b8; }
</style>