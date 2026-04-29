<template>
  <div id="app">
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </div>
</template>

<script setup>
import { watch, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import { useAuthStore } from '@/stores/auth';

const route = useRoute();
const authStore = useAuthStore();

const applyThemeForCurrentRoute = () => {
  const publicPages = ['/', '/login', '/register', '/pricing', '/activate-role', '/forgot-password', '/reset-password', '/definir-mot-de-passe'];
  
  if (publicPages.includes(route.path) || !route.meta || !route.meta.requiresAuth || !authStore.user) {
    // Force light mode on public pages or if not logged in
    document.documentElement.setAttribute('data-theme', 'light');
    document.documentElement.classList.remove('dark-mode');
  } else {
    // Apply user-specific preference from DB or specific localStorage key
    const dbTheme = authStore.user.themePreference;
    const themeKey = `theme_${authStore.user.id}`;
    const localTheme = localStorage.getItem(themeKey);
    const savedTheme = dbTheme || localTheme || 'light';
    
    document.documentElement.setAttribute('data-theme', savedTheme);
    document.documentElement.classList.toggle('dark-mode', savedTheme === 'dark');
  }
};

onMounted(() => {
  applyThemeForCurrentRoute();
});

watch(
  () => route.path,
  () => {
    applyThemeForCurrentRoute();
  }
);

// Watch for user changes (login/logout) to update theme immediately
watch(
  () => authStore.user?.id,
  () => {
    applyThemeForCurrentRoute();
  }
);
</script>

<style>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

:root {
  color-scheme: light;
  --primary: #f59e0b;
  --secondary: #0f172a;
  --bg-main: #e3e8ef;
  --card-bg: rgba(255, 255, 255, 0.85);
  --text-main: #0f172a;
  --text-muted: #64748b;
  --border: rgba(0, 0, 0, 0.05);
  --shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05);
  --input-bg: #ffffff;
}

[data-theme="dark"] {
  color-scheme: dark;
  --secondary: #f1f5f9;
  --bg-main: #080c14;
  --card-bg: #111827;
  --text-main: #f1f5f9;
  --text-muted: #94a3b8;
  --border: rgba(255, 255, 255, 0.05);
  --shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.5);
  --input-bg: #1f2937;
}

body { 
  font-family: 'Plus Jakarta Sans', sans-serif; 
  background-color: var(--bg-main); 
  color: var(--text-main);
  transition: background-color 0.3s ease, color 0.3s ease;
}

/* Optimisation globale des cartes et panneaux */
.premium-card, .glass-panel, .stat-card, .glass-card, .cyber-glass-card, 
.table-container-premium, .modal-box-pro, .hero-cyber-card, .activity-card,
.analytics-card-pro, .campaign-card-modern, .enigma-card, .list-row-item,
.info-card, .kpi-glass-card, .asset-card-cyber, .filter-cyber-bar,
.modal-cyber-card, .type-v-card, .card-premium, .modal-content-pro,
.kpi-card-v2, .toolbar-pro {
  background: var(--card-bg) !important;
  border: 1px solid var(--border) !important;
  color: var(--text-main) !important;
  backdrop-filter: blur(12px);
  transition: transform 0.3s ease, background-color 0.3s ease, border-color 0.3s ease;
}

.premium-card:hover { transform: translateY(-5px); }

/* Optimisation des formulaires */
.clean-input, .cyber-input, .cyber-select, .filter-input, .filter-select,
.form-control-pro, .search-box input, .input-pro-search {
  background: var(--input-bg) !important;
  color: var(--text-main) !important;
  border: 1.5px solid var(--border) !important;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.clean-input:focus { border-color: var(--primary) !important; box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.1); }

/* Textes et Labels */
.text-muted, .tiny, .stat-label, .tiny-label, .meta-item {
  color: var(--text-muted) !important;
  transition: color 0.3s ease;
}

h1, h2, h3, h4, h5, h6, .fw-bold, th, .page-title, .stat-number {
  color: var(--text-main) !important;
  transition: color 0.3s ease;
}

/* Boutons */
.btn-primary-gold {
  background: linear-gradient(135deg, var(--primary) 0%, #fbbf24 100%);
  color: white; border: none; border-radius: 14px; font-weight: 700;
  padding: 12px 28px; transition: 0.3s; box-shadow: 0 8px 15px rgba(245, 158, 11, 0.2);
}
.btn-primary-gold:hover { transform: scale(1.05); }

/* Utility Classes */
.admin-layout { min-height: 100vh; display: flex; }
.main-content { flex-grow: 1; padding: 2rem; background: var(--bg-main); }
.status-pill { padding: 6px 14px; border-radius: 10px; font-size: 11px; font-weight: 800; text-transform: uppercase; }
.active-pill { background: #ecfdf5; color: #10b981; }
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

</style>