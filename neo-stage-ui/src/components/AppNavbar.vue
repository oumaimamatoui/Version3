<template>
  <nav class="navbar navbar-expand bg-glass sticky-top px-4 py-0 shadow-sm border-bottom">
    <div class="container-fluid p-0">
      
      <!-- 1. BARRE DE RECHERCHE SMART (Accent Ambre) -->
      <div class="d-none d-md-flex align-items-center flex-grow-1 me-4">
        <div class="search-box">
          <i class="fa-solid fa-magnifying-glass search-icon"></i>
          <input type="text" :placeholder="t('search')" class="search-input" ref="searchInput">
          <span class="search-shortcut">⌘K</span>
        </div>
      </div>

      <div class="ms-auto d-flex align-items-center gap-2">
        
        <!-- 2. SWITCHER DE THÈME -->
        <button class="nav-btn-icon theme-toggle" @click="toggleTheme" :title="t('toggleTheme')">
          <Transition name="scale" mode="out-in">
            <i v-if="isDark" class="fa-solid fa-sun text-amber" key="sun"></i>
            <i v-else class="fa-solid fa-moon text-navy" key="moon"></i>
          </Transition>
        </button>

        <!-- 3. SÉLECTEUR DE LANGUE -->
        <div class="nav-item dropdown">
          <button class="nav-btn-icon dropdown-toggle no-caret" data-bs-toggle="dropdown">
            <i class="fa-solid fa-globe"></i>
            <span class="lang-text ms-2 d-none d-sm-inline">{{ currentLang }}</span>
          </button>
          <div class="dropdown-menu dropdown-menu-end shadow-premium lang-dropdown animate-slide-in">
            <button class="dropdown-item d-flex align-items-center" @click="setLang('FR')">
              <span class="flag">🇫🇷</span> Français 
              <i v-if="currentLang === 'FR'" class="fa-solid fa-check ms-auto tiny-check text-amber"></i>
            </button>
            <button class="dropdown-item d-flex align-items-center" @click="setLang('EN')">
              <span class="flag">🇺🇸</span> English 
              <i v-if="currentLang === 'EN'" class="fa-solid fa-check ms-auto tiny-check text-amber"></i>
            </button>
          </div>
        </div>

        <!-- 4. DROPDOWN NOTIFICATIONS (Accent Ambre) -->
        <div class="nav-item dropdown px-2">
          <a href="#" class="nav-link dropdown-toggle no-caret position-relative bell-container" 
             data-bs-toggle="dropdown" 
             @click="notifStore.markAllAsRead()">
            <i class="fa-regular fa-bell fs-5 text-navy"></i>
            <span v-if="notifStore.unreadCount > 0" class="notif-badge">
              {{ notifStore.unreadCount }}
            </span>
          </a>
          <div class="dropdown-menu dropdown-menu-end shadow-premium notif-dropdown animate-slide-in">
            <div class="p-3 border-bottom d-flex justify-content-between align-items-center bg-light-soft">
              <h6 class="m-0 fw-bold text-navy">Notifications</h6>
              <span class="badge bg-amber-soft text-amber">{{ notifStore.unreadCount }} nouvelles</span>
            </div>
            <div class="notif-list custom-scrollbar">
              <a v-for="n in notifStore.notifications" :key="n.id" href="#" class="dropdown-item notif-item">
                <p class="mb-1 small fw-bold text-navy">{{ n.text }}</p>
                <small class="text-muted">{{ n.time }}</small>
              </a>
            </div>
            <div class="p-2 border-top text-center text-muted tiny">
              NeoEvaluation Platform
            </div>
          </div>
        </div>
        
        <!-- 5. PROFIL UTILISATEUR (Navy & Amber) -->
        <div class="nav-item dropdown ps-2 border-start border-light-2">
          <a href="#" class="nav-link dropdown-toggle d-flex align-items-center gap-2 user-link" data-bs-toggle="dropdown">
            <div class="avatar-container">
              <img class="avatar-img" :src="userPhotoUrl" alt="Profile">
              <span class="status-indicator"></span>
            </div>
            <div class="d-none d-lg-block text-start">
              <span class="user-name">{{ authStore.user?.name || 'Utilisateur' }}</span>
              <span class="user-role">{{ roleDisplay }}</span>
            </div>
          </a>
          <div class="dropdown-menu dropdown-menu-end shadow-premium profile-dropdown animate-slide-in">
            <router-link to="/profile" class="dropdown-item"><i class="fa-solid fa-circle-user me-2 text-amber"></i>Mon Profil</router-link>
            <router-link to="/settings" class="dropdown-item"><i class="fa-solid fa-user-gear me-2 text-amber"></i>Paramètres</router-link>
            <div class="dropdown-divider"></div>
            <router-link to="/login" class="dropdown-item text-danger fw-bold"><i class="fa-solid fa-power-off me-2"></i>Déconnexion</router-link>
          </div>
        </div>

      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useNotificationStore } from '@/stores/notification';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

const notifStore = useNotificationStore();
const authStore = useAuthStore();
const searchInput = ref(null);

const roleDisplay = computed(() => {
  const map = { 
    'SuperAdmin': 'SuperAdmin', 
    'AdminEntreprise': 'Administrateur', 
    'Evaluateur': 'Évaluateur', 
    'Candidat': 'Candidat' 
  };
  return map[authStore.role] || authStore.role || 'Utilisateur';
});

const userPhotoUrl = computed(() => {
  if (authStore.user?.photoUrl) {
    return `http://localhost:5172${authStore.user.photoUrl}`;
  }
  return `https://ui-avatars.com/api/?name=${authStore.user?.name || 'User'}&background=0f172a&color=fff`;
});

// --- THÈME ---
const getThemeKey = () => authStore.user?.id ? `theme_${authStore.user.id}` : null;
const isDark = ref(false);

const applyTheme = (theme) => {
  document.documentElement.setAttribute('data-theme', theme);
  document.documentElement.classList.toggle('dark-mode', theme === 'dark');
};

const toggleTheme = async () => {
  isDark.value = !isDark.value;
  const theme = isDark.value ? 'dark' : 'light';
  applyTheme(theme);
  
  const themeKey = getThemeKey();
  if (themeKey) {
    localStorage.setItem(themeKey, theme);
  }

  // Sauvegarde en DB si connecté
  if (authStore.user) {
    try {
      await api.post('/Settings/theme', JSON.stringify(theme), {
        headers: { 'Content-Type': 'application/json' }
      });
      // Mettre à jour l'objet user dans le store
      authStore.user.themePreference = theme;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    } catch (err) {
      console.error("Erreur sauvegarde thème:", err);
    }
  }
};

// --- LANGUE ---
const currentLang = ref(localStorage.getItem('lang') || 'FR');
const setLang = (lang) => {
  currentLang.value = lang;
  localStorage.setItem('lang', lang);
};

const t = (key) => {
  const translations = {
    FR: { search: 'Rechercher (⌘K)...', toggleTheme: 'Changer de thème' },
    EN: { search: 'Search (⌘K)...', toggleTheme: 'Toggle Theme' }
  };
  return translations[currentLang.value][key];
};

// --- SHORTCUT ---
const handleKeyDown = (e) => {
  if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
    e.preventDefault();
    searchInput.value?.focus();
  }
};

onMounted(() => {
  // Priorité : Store (DB) > LocalStorage > Default (light)
  const dbTheme = authStore.user?.themePreference;
  const localTheme = localStorage.getItem(getThemeKey());
  const savedTheme = dbTheme || localTheme || 'light';
  
  isDark.value = savedTheme === 'dark';
  applyTheme(savedTheme);
  window.addEventListener('keydown', handleKeyDown);
});

onUnmounted(() => window.removeEventListener('keydown', handleKeyDown));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* --- COLORS --- */
:root {
  --amber: #eab308;
  --navy: #0f172a;
  --bg-glass: rgba(255, 255, 255, 0.85);
  --border: #f1f5f9;
}

:host-context([data-theme="dark"]), 
[data-theme="dark"] .navbar {
  --bg-glass: rgba(15, 23, 42, 0.85);
  --border: rgba(255, 255, 255, 0.1);
  --navy: #f1f5f9;
}

/* --- NAVBAR --- */
.navbar {
  height: 75px;
  background: var(--bg-glass) !important;
  backdrop-filter: blur(12px);
  font-family: 'Plus Jakarta Sans', sans-serif;
  z-index: 1050;
}

/* --- SEARCH --- */
.search-box { position: relative; width: 100%; max-width: 400px; }
.search-icon { position: absolute; left: 15px; top: 50%; transform: translateY(-50%); color: #94a3b8; font-size: 14px; }
.search-input {
  width: 100%; background: #f8fafc; border: 1px solid #e2e8f0;
  padding: 12px 12px 12px 45px; border-radius: 14px; font-size: 14px;
  transition: all 0.3s;
}
.search-input:focus { 
  outline: none; border-color: var(--amber); background: #fff; 
  box-shadow: 0 0 0 4px rgba(234, 179, 8, 0.1); 
}
.search-shortcut { position: absolute; right: 12px; top: 50%; transform: translateY(-50%); font-size: 10px; font-weight: 800; background: #e2e8f0; color: #64748b; padding: 2px 6px; border-radius: 6px; }

/* --- ICONS & BUTTONS --- */
.nav-btn-icon {
  width: 42px; height: 42px; border-radius: 12px; border: none; background: transparent;
  display: flex; align-items: center; justify-content: center; color: var(--navy); transition: 0.2s;
}
.nav-btn-icon:hover { background: rgba(234, 179, 8, 0.1); color: var(--amber); }

.text-amber { color: var(--amber) !important; }
.text-navy { color: var(--navy) !important; }

/* --- DROPDOWNS --- */
.shadow-premium { box-shadow: 0 20px 50px rgba(15, 23, 42, 0.1); }
.dropdown-menu { border: 1px solid var(--border); border-radius: 18px; padding: 10px; margin-top: 15px; background: var(--bg-glass); backdrop-filter: blur(20px); }
.dropdown-item { border-radius: 10px; padding: 10px 15px; font-size: 14px; font-weight: 600; color: var(--navy); transition: 0.2s; }
.dropdown-item:hover { background: #fefce8; color: var(--amber); transform: translateX(3px); }

/* --- NOTIFICATIONS --- */
.notif-badge {
  position: absolute; top: 5px; right: 5px; background: var(--amber); color: #fff;
  width: 18px; height: 18px; border-radius: 50%; font-size: 10px; font-weight: 800;
  display: flex; align-items: center; justify-content: center; border: 2px solid #fff;
}
.bg-amber-soft { background-color: #fefce8; }
.notif-dropdown { width: 340px; }
.notif-list { max-height: 320px; overflow-y: auto; }

/* --- USER --- */
.avatar-container { position: relative; }
.avatar-img { width: 42px; height: 42px; border-radius: 14px; border: 2px solid #fff; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.status-indicator { position: absolute; bottom: -2px; right: -2px; width: 12px; height: 12px; background: #22c55e; border: 2px solid #fff; border-radius: 50%; }
.user-name { display: block; font-size: 14px; font-weight: 800; color: var(--navy); line-height: 1.2; }
.user-role { font-size: 11px; font-weight: 700; color: var(--amber); text-transform: uppercase; letter-spacing: 0.5px; }

/* --- ANIMATIONS --- */
.animate-slide-in { animation: slideIn 0.3s ease-out; }
@keyframes slideIn { from { transform: translateY(10px); opacity: 0; } to { transform: translateY(0); opacity: 1; } }
.scale-enter-active, .scale-leave-active { transition: all 0.2s ease; }
.scale-enter-from, .scale-leave-to { transform: scale(0); opacity: 0; }

.no-caret::after { display: none; }
.btn-link-sm { text-decoration: none; font-size: 13px; }
</style>