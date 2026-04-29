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
  --primary: #f59e0b; /* Amber */
  --secondary: #0f172a; /* Navy */
  --bg-light: #e3e8ef; /* Slightly darker gray for better contrast with white inputs */
  --text-muted: #64748b;
  --glass: rgba(255, 255, 255, 0.85);
  --shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05);
}

[data-theme="dark"] {
  --secondary: #f1f5f9; /* White-ish text */
  --bg-light: #080c14;  /* Deep Navy Background */
  --text-muted: #94a3b8;
  --glass: rgba(15, 23, 42, 0.85); /* Dark Glass */
  --shadow: 0 10px 30px -5px rgba(0, 0, 0, 0.5);
  
  /* Global Overrides for components across all views */
  --card-bg: #111827;
  --table-header: #1f2937;
  --input-bg: #1f2937;
  --border-color: rgba(255, 255, 255, 0.05);
}

[data-theme="dark"] .stat-card-premium,
[data-theme="dark"] .glass-panel,
[data-theme="dark"] .table-container-premium,
[data-theme="dark"] .modal-box-pro,
[data-theme="dark"] .premium-card,
[data-theme="dark"] .hero-cyber-card,
[data-theme="dark"] .glass-card,
[data-theme="dark"] .cyber-glass-card,
[data-theme="dark"] .activity-card,
[data-theme="dark"] .preview-card-mockup,
[data-theme="dark"] .analytics-card-pro,
[data-theme="dark"] .campaign-card-modern,
[data-theme="dark"] .enigma-card,
[data-theme="dark"] .list-row-item,
[data-theme="dark"] .info-card,
[data-theme="dark"] .kpi-glass-card,
[data-theme="dark"] .asset-card-cyber,
[data-theme="dark"] .filter-cyber-bar,
[data-theme="dark"] .modal-cyber-card,
[data-theme="dark"] .type-v-card,
[data-theme="dark"] .card-premium,
[data-theme="dark"] .modal-content-pro,
[data-theme="dark"] .kpi-card-v2,
[data-theme="dark"] .toolbar-pro,
[data-theme="dark"] .stat-card,
[data-theme="dark"] .premium-card {
  background: var(--card-bg) !important;
  border-color: var(--border-color) !important;
  color: var(--secondary) !important;
}

[data-theme="dark"] .step-header h5, [data-theme="dark"] .step-header h6 {
  color: #ffffff !important;
}

[data-theme="dark"] .cyber-select, [data-theme="dark"] .cyber-input {
  background: var(--input-bg) !important;
  color: white !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .cyber-select option {
  background: var(--card-bg) !important;
  color: white !important;
}

[data-theme="dark"] .candidate-row:hover {
  background-color: rgba(255, 255, 255, 0.05) !important;
}

[data-theme="dark"] .avatar-initials {
  background: rgba(245, 158, 11, 0.1) !important;
  color: #f59e0b !important;
}

[data-theme="dark"] .group-tag {
  background: rgba(255, 255, 255, 0.05) !important;
  color: #f1f5f9 !important;
}

[data-theme="dark"] .filter-input, [data-theme="dark"] .filter-select {
  background: var(--input-bg) !important;
  color: white !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .btn-action-outline {
  background: transparent !important;
  color: white !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .text-navy {
  color: #ffffff !important;
}

[data-theme="dark"] .cyber-input-area, 
[data-theme="dark"] .cyber-field-simple, 
[data-theme="dark"] .cyber-select-field,
[data-theme="dark"] .cyber-select-minimal,
[data-theme="dark"] .search-cyber-box,
[data-theme="dark"] .chip,
[data-theme="dark"] .form-control-photo,
[data-theme="dark"] .form-select-photo,
[data-theme="dark"] .search-box input,
[data-theme="dark"] .input-pro-search,
[data-theme="dark"] .form-control-pro {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] .org-badge, [data-theme="dark"] .btn-icon-mini, [data-theme="dark"] .btn-outline-tech-sm {
  background: rgba(255,255,255,0.05) !important;
  color: white !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .bg-light {
  background: var(--table-header) !important;
}

[data-theme="dark"] .stat-label, 
[data-theme="dark"] .tiny-label, 
[data-theme="dark"] .stat-label-v2,
[data-theme="dark"] .text-secondary,
[data-theme="dark"] .text-muted-pro,
[data-theme="dark"] .meta-item,
[data-theme="dark"] .text-muted,
[data-theme="dark"] .tiny {
  color: #94a3b8 !important; /* Lighter gray for readability */
}

[data-theme="dark"] .stat-number, 
[data-theme="dark"] .stat-value,
[data-theme="dark"] .display-title-v2,
[data-theme="dark"] .page-title,
[data-theme="dark"] .fw-bold,
[data-theme="dark"] th {
  color: #ffffff !important;
}

[data-theme="dark"] .text-indigo {
  color: #818cf8 !important; /* Lighter indigo for dark mode */
}

[data-theme="dark"] .section-title, [data-theme="dark"] .card-content-wrap h6 {
  color: #ffffff !important;
}

[data-theme="dark"] .modern-tabs, [data-theme="dark"] .difficulty-pills {
  background: rgba(255, 255, 255, 0.05) !important;
}

[data-theme="dark"] .modern-tabs button.active, [data-theme="dark"] .pill-btn.active {
  background: var(--secondary) !important;
  color: var(--bg-light) !important;
}

[data-theme="dark"] .modern-select, [data-theme="dark"] .modern-input, [data-theme="dark"] .form-select-premium, [data-theme="dark"] .input-pro-search, [data-theme="dark"] .form-control-pro, [data-theme="dark"] .cyber-select, [data-theme="dark"] .cyber-input {
  background: var(--input-bg) !important;
  color: white !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .csv-dropzone, [data-theme="dark"] .upload-zone-premium {
  background: rgba(255, 255, 255, 0.02) !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .type-card-mini, [data-theme="dark"] .q-card-premium {
  background: var(--card-bg) !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .type-card-mini.active {
  background: rgba(245, 158, 11, 0.1) !important;
  border-color: #f59e0b !important;
}

[data-theme="dark"] .opt-item-v2, [data-theme="dark"] .file-pill-pro {
  background: rgba(255, 255, 255, 0.03) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] .opt-item-v2.correct {
  background: rgba(16, 185, 129, 0.1) !important;
  border-color: #10b981 !important;
  color: #10b981 !important;
}

[data-theme="dark"] .text-navy, [data-theme="dark"] .q-text-v2, [data-theme="dark"] .display-title-v2, [data-theme="dark"] .csv-dropzone p, [data-theme="dark"] .upload-zone-premium p {
  color: #ffffff !important;
}

[data-theme="dark"] .main-cloud, [data-theme="dark"] .q-actions-mini button, [data-theme="dark"] .btn-remove-sm {
  color: #94a3b8 !important;
}

[data-theme="dark"] .cyber-input-area, 
[data-theme="dark"] .cyber-field-simple, 
[data-theme="dark"] .cyber-select-field,
[data-theme="dark"] .cyber-select-minimal,
[data-theme="dark"] .search-cyber-box,
[data-theme="dark"] .chip,
[data-theme="dark"] .form-control-photo,
[data-theme="dark"] .form-select-photo,
[data-theme="dark"] .search-box input {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] .modal-header-pro, [data-theme="dark"] .section-title-mod {
  border-color: var(--border-color) !important;
  color: white !important;
}

[data-theme="dark"] .page-title, [data-theme="dark"] .stat-number, [data-theme="dark"] .chart-center-text .number {
  color: #ffffff !important;
}

[data-theme="dark"] .cyber-input-area, 
[data-theme="dark"] .cyber-field-simple, 
[data-theme="dark"] .cyber-select-field,
[data-theme="dark"] .cyber-select-minimal,
[data-theme="dark"] .search-cyber-box,
[data-theme="dark"] .chip {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] .chip.active {
  background: #eab308 !important;
  color: #0f172a !important;
}

[data-theme="dark"] .modal-cyber-footer {
  background: var(--table-header) !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .cat-pill-item, [data-theme="dark"] .type-badge-sm {
  background: rgba(255,255,255,0.05) !important;
  color: #f1f5f9 !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .nav-tab-btn-modern, [data-theme="dark"] .btn-refresh-pro {
  background: rgba(255,255,255,0.05) !important;
  color: var(--secondary) !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .nav-tab-btn-modern.active {
  background: var(--secondary) !important;
  color: var(--bg-light) !important;
}

[data-theme="dark"] .search-inline-box, 
[data-theme="dark"] .sort-select-pro, 
[data-theme="dark"] .tabs-container > div {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
}

[data-theme="dark"] .search-inline-input {
  background: transparent !important;
  color: white !important;
}

[data-theme="dark"] .table-pro th,
[data-theme="dark"] .modal-header-tech {
  background: var(--table-header) !important;
  color: var(--secondary) !important;
}

[data-theme="dark"] .cyber-select, 
[data-theme="dark"] .cyber-input, 
[data-theme="dark"] .input-pro, 
[data-theme="dark"] .select-pro, 
[data-theme="dark"] .clean-input {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] .brand-title,
[data-theme="dark"] .brand-title span {
  color: #ffffff !important;
}

[data-theme="dark"] .brand-subtitle {
  color: #94a3b8 !important;
}

[data-theme="dark"] .mockup-body, 
[data-theme="dark"] .mockup-btn {
  background: rgba(255,255,255,0.05) !important;
  color: #94a3b8 !important;
}

[data-theme="dark"] .text-slate-900,
[data-theme="dark"] .text-slate-800,
[data-theme="dark"] .text-slate-700,
[data-theme="dark"] .text-slate-600,
[data-theme="dark"] .text-slate-500,
[data-theme="dark"] h1, [data-theme="dark"] h2, [data-theme="dark"] h3, [data-theme="dark"] h4, [data-theme="dark"] h5,
[data-theme="dark"] .table-pro td,
[data-theme="dark"] .user-name-text {
  color: #f1f5f9 !important;
}

[data-theme="dark"] .text-muted, [data-theme="dark"] .tiny, [data-theme="dark"] .text-slate-400 {
  color: #94a3b8 !important;
}

[data-theme="dark"] .input-pro, [data-theme="dark"] .select-pro, [data-theme="dark"] .clean-input {
  background: var(--input-bg) !important;
  border-color: var(--border-color) !important;
  color: #ffffff !important;
}

[data-theme="dark"] body, 
[data-theme="dark"] .admin-body, 
[data-theme="dark"] .main-viewport, 
[data-theme="dark"] .cyber-engine-bg {
  background-color: var(--bg-light) !important;
  background-image: none !important;
}

[data-theme="dark"] .quantum-grid {
  background-image: linear-gradient(rgba(255,255,255,0.03) 1px, transparent 1px), 
                    linear-gradient(90deg, rgba(255,255,255,0.03) 1px, transparent 1px) !important;
  opacity: 0.5;
}

[data-theme="dark"] .glow-orb {
  opacity: 0.2 !important;
}

body { font-family: 'Plus Jakarta Sans', sans-serif; background-color: var(--bg-light); color: var(--secondary); }
.admin-layout { min-height: 100vh; display: flex; }


/* Global Components */
.brand-title { font-weight: 800; font-size: 2rem; color: var(--secondary); }
.brand-title span { color: var(--primary); }

.premium-card {
  background: var(--glass); backdrop-filter: blur(12px);
  border: 1px solid white; border-radius: 24px;
  box-shadow: var(--shadow); padding: 1.5rem; transition: 0.3s ease;
}
.premium-card:hover { transform: translateY(-5px); box-shadow: 0 20px 40px rgba(0,0,0,0.08); }

.btn-primary-gold {
  background: linear-gradient(135deg, var(--primary) 0%, #fbbf24 100%);
  color: white; border: none; border-radius: 14px; font-weight: 700;
  padding: 12px 28px; transition: 0.3s; box-shadow: 0 8px 15px rgba(245, 158, 11, 0.2);
}
.btn-primary-gold:hover { transform: scale(1.05); }

.clean-input {
  width: 100%; padding: 14px 18px; border-radius: 14px;
  border: 1.5px solid var(--primary); background: white; outline: none; transition: 0.2s;
  color: var(--secondary);
}
.clean-input:focus { border-color: var(--primary); box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.1); }

/* Unified Input Borders across platform */
input[type="text"], input[type="password"], input[type="email"], input[type="number"], input[type="tel"], input[type="url"],
select, textarea {
  border-color: #f59e0b !important;
}

/* Status Pills */
.status-pill { padding: 6px 14px; border-radius: 10px; font-size: 11px; font-weight: 800; text-transform: uppercase; }
.active-pill { background: #ecfdf5; color: #10b981; }
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
.fw-800 { font-weight: 800; }
.glass-card { background: white; border: 1px solid #f1f5f9; border-radius: 20px; transition: 0.3s; }
.glass-card:hover { transform: translateY(-5px); box-shadow: 0 10px 30px rgba(0,0,0,0.05); }
.icon-box-small { width: 40px; height: 40px; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.bg-indigo { background-color: #0f172a !important; }
.text-amber { color: #eab308 !important; }
.btn-amber { background-color: #eab308; color: #0f172a; border: none; }
.bg-success-soft { background-color: #dcfce7; }

</style>