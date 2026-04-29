<template>
  <nav class="topbar" role="banner">
    <div class="topbar-inner">

      <!-- ── GAUCHE : RECHERCHE ── -->
      <div class="search-zone">
        <div class="search-wrap">
          <Search :size="14" class="search-ico" />
          <input
            ref="searchInput"
            type="text"
            :placeholder="t('search')"
            class="search-field"
            @focus="searchFocused = true"
            @blur="searchFocused = false"
          />
          <kbd class="search-kbd">⌘K</kbd>
        </div>
      </div>

      <!-- ── DROITE : ACTIONS ── -->
      <div class="topbar-actions">

        <!-- THÈME -->
        <button class="action-btn" @click="toggleTheme" :title="t('toggleTheme')">
          <Transition name="icon-swap" mode="out-in">
            <Sun v-if="isDark" :size="16" key="sun" />
            <Moon v-else :size="16" key="moon" />
          </Transition>
        </button>

        <!-- LANGUE -->
        <div class="dropdown" ref="langDropdown">
          <button class="action-btn lang-btn" @click="toggleDropdown('lang')">
            <Globe :size="16" />
            <span class="lang-label">{{ currentLang }}</span>
            <ChevronDown :size="11" class="caret" />
          </button>
          <Transition name="dropdown-fade">
            <div v-if="openDropdown === 'lang'" class="drop-panel lang-panel">
              <button
                v-for="l in langs"
                :key="l.code"
                class="drop-item"
                :class="{ active: currentLang === l.code }"
                @click="setLang(l.code)"
              >
                <span class="flag">{{ l.flag }}</span>
                <span>{{ l.label }}</span>
                <Check v-if="currentLang === l.code" :size="12" class="check-ico" />
              </button>
            </div>
          </Transition>
        </div>

        <!-- NOTIFICATIONS -->
        <div class="dropdown" ref="notifDropdown">
          <button
            class="action-btn notif-btn"
            @click="toggleDropdown('notif'); notifStore.markAllAsRead()"
          >
            <Bell :size="16" />
            <span v-if="notifStore.unreadCount > 0" class="notif-dot">
              {{ notifStore.unreadCount > 9 ? '9+' : notifStore.unreadCount }}
            </span>
          </button>
          <Transition name="dropdown-fade">
            <div v-if="openDropdown === 'notif'" class="drop-panel notif-panel">
              <div class="panel-head">
                <span class="panel-title">Notifications</span>
                <span class="count-chip">{{ notifStore.unreadCount }} nouvelles</span>
              </div>
              <div class="notif-scroll">
                <div
                  v-for="n in notifStore.notifications"
                  :key="n.id"
                  class="notif-row"
                >
                  <div class="notif-icon-wrap">
                    <Bell :size="12" />
                  </div>
                  <div class="notif-body">
                    <p class="notif-text">{{ n.text }}</p>
                    <span class="notif-time">{{ n.time }}</span>
                  </div>
                </div>
                <div v-if="!notifStore.notifications?.length" class="notif-empty">
                  <BellOff :size="28" class="empty-ico" />
                  <span>Aucune notification</span>
                </div>
              </div>
              <div class="panel-foot">
                <button class="mark-read-btn">Tout marquer comme lu</button>
              </div>
            </div>
          </Transition>
        </div>

        <!-- SÉPARATEUR -->
        <span class="sep"></span>

        <!-- PROFIL -->
        <div class="dropdown" ref="profileDropdown">
          <button class="profile-trigger" @click="toggleDropdown('profile')">
            <div class="avatar-wrap">
              <img :src="userPhotoUrl" :alt="authStore.user?.name" class="avatar-img" />
              <span class="status-dot"></span>
            </div>
            <div class="user-info">
              <span class="user-name">{{ authStore.user?.name || 'Utilisateur' }}</span>
              <span class="user-role">{{ roleDisplay }}</span>
            </div>
            <ChevronDown :size="12" class="caret" :class="{ rotated: openDropdown === 'profile' }" />
          </button>
          <Transition name="dropdown-fade">
            <div v-if="openDropdown === 'profile'" class="drop-panel profile-panel">
              <div class="profile-head">
                <img :src="userPhotoUrl" :alt="authStore.user?.name" class="ph-avatar" />
                <div>
                  <div class="ph-name">{{ authStore.user?.name || 'Utilisateur' }}</div>
                  <div class="ph-role">{{ roleDisplay }}</div>
                </div>
              </div>
              <div class="drop-divider"></div>
              <router-link to="/profile" class="drop-item" @click="closeDropdown">
                <UserCircle :size="14" class="di-icon" /><span>Mon profil</span>
              </router-link>
              <router-link to="/settings" class="drop-item" @click="closeDropdown">
                <Settings :size="14" class="di-icon" /><span>Paramètres</span>
              </router-link>
              <div class="drop-divider"></div>
              <button class="drop-item danger" @click="logout">
                <LogOut :size="14" class="di-icon" /><span>Déconnexion</span>
              </button>
            </div>
          </Transition>
        </div>

      </div>
    </div>

    <!-- Click outside overlay -->
    <div v-if="openDropdown" class="click-outside" @click="closeDropdown"></div>
  </nav>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import { useNotificationStore } from '@/stores/notification';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

import {
  Search, Sun, Moon, Globe, Bell, BellOff, ChevronDown,
  Check, UserCircle, Settings, LogOut,
} from 'lucide-vue-next';

const router     = useRouter();
const notifStore = useNotificationStore();
const authStore  = useAuthStore();

const searchInput   = ref(null);
const searchFocused = ref(false);
const openDropdown  = ref(null);
const isDark        = ref(false);
const currentLang   = ref(localStorage.getItem('lang') || 'FR');

const langs = [
  { code: 'FR', label: 'Français', flag: '🇫🇷' },
  { code: 'EN', label: 'English',  flag: '🇺🇸' },
];

const roleDisplay = computed(() => ({
  SuperAdmin:      'SuperAdmin',
  AdminEntreprise: 'Administrateur',
  Evaluateur:      'Évaluateur',
  Candidat:        'Candidat',
  Recruteur:       'RH / Recruteur',
}[authStore.role] || authStore.role || 'Utilisateur'));

const userPhotoUrl = computed(() => {
  if (authStore.user?.photoUrl) return `http://localhost:5172${authStore.user.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${encodeURIComponent(authStore.user?.name || 'User')}&background=0f172a&color=fff&bold=true`;
});

// ── Dropdowns ──
const toggleDropdown = (name) => {
  openDropdown.value = openDropdown.value === name ? null : name;
};
const closeDropdown = () => { openDropdown.value = null; };

// ── Thème ──
const getThemeKey = () => authStore.user?.id ? `theme_${authStore.user.id}` : 'theme_guest';

const toggleTheme = async () => {
  isDark.value = !isDark.value;
  const theme = isDark.value ? 'dark' : 'light';
  
  // App.vue gérera le DOM via le watch sur localStorage ou authStore
  localStorage.setItem(getThemeKey(), theme);
  
  if (authStore.user) {
    try {
      await api.post('/Settings/theme', JSON.stringify(theme), {
        headers: { 'Content-Type': 'application/json' },
      });
      authStore.user.themePreference = theme;
      // Mettre à jour l'authStore pour déclencher le watch dans App.vue
      localStorage.setItem('user', JSON.stringify(authStore.user));
    } catch (err) {
      console.error('Erreur sauvegarde thème:', err);
    }
  }
};

// ── Langue ──
const setLang = (lang) => {
  currentLang.value = lang;
  localStorage.setItem('lang', lang);
  closeDropdown();
};

const t = (key) => ({
  FR: { search: 'Rechercher…  ⌘K', toggleTheme: 'Changer de thème' },
  EN: { search: 'Search…  ⌘K',     toggleTheme: 'Toggle theme' },
}[currentLang.value]?.[key] || key);

// ── Logout ──
const logout = () => {
  authStore.logout();
  window.location.href = '/login';
  setTimeout(() => {
    window.location.reload();
  }, 100);
};

// ── Shortcut ⌘K ──
const handleKeyDown = (e) => {
  if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
    e.preventDefault();
    searchInput.value?.focus();
  }
  if (e.key === 'Escape') closeDropdown();
};

onMounted(() => {
  const saved = authStore.user?.themePreference || localStorage.getItem(getThemeKey()) || 'light';
  isDark.value = saved === 'dark';
  window.addEventListener('keydown', handleKeyDown);
});

onUnmounted(() => window.removeEventListener('keydown', handleKeyDown));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700&display=swap');

/* ════════════════════════════════
   ROOT
════════════════════════════════ */
.topbar {
  position: sticky;
  top: 0;
  z-index: 1050;
  height: 64px;
  background: rgba(255, 255, 255, 0.88);
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid #edf0f4;
  font-family: 'DM Sans', sans-serif;
}

[data-theme="dark"] .topbar {
  background: rgba(13, 17, 23, 0.88);
  border-bottom-color: rgba(255, 255, 255, 0.06);
}

.topbar-inner {
  height: 100%;
  max-width: 100%;
  padding: 0 24px;
  display: flex;
  align-items: center;
  gap: 16px;
}

/* ════════════════════════════════
   RECHERCHE
════════════════════════════════ */
.search-zone {
  flex: 1;
  max-width: 420px;
}

.search-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.search-ico {
  position: absolute;
  left: 13px;
  color: #9ca3af;
  pointer-events: none;
  flex-shrink: 0;
}

.search-field {
  width: 100%;
  height: 38px;
  background: #f6f8fa;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 0 40px 0 38px;
  font-size: 13px;
  font-weight: 500;
  color: #0d1117;
  font-family: 'DM Sans', sans-serif;
  transition: border-color 0.2s, box-shadow 0.2s, background 0.2s;
  outline: none;
}

.search-field::placeholder { color: #9ca3af; }

.search-field:focus {
  background: #fff;
  border-color: #d97706;
  box-shadow: 0 0 0 3px rgba(217, 119, 6, 0.12);
}

[data-theme="dark"] .search-field {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.08);
  color: #f0f6fc;
}
[data-theme="dark"] .search-field:focus {
  background: rgba(255, 255, 255, 0.08);
  border-color: #d97706;
}

.search-kbd {
  position: absolute;
  right: 10px;
  background: #edf0f4;
  color: #6b7280;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 5px;
  font-family: 'DM Sans', sans-serif;
  pointer-events: none;
}
[data-theme="dark"] .search-kbd { background: rgba(255,255,255,0.08); color: #6b7280; }

/* ════════════════════════════════
   ACTIONS
════════════════════════════════ */
.topbar-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  margin-left: auto;
}

.action-btn {
  width: 38px;
  height: 38px;
  border-radius: 9px;
  border: 1px solid transparent;
  background: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #57606a;
  transition: background 0.15s, color 0.15s, border-color 0.15s;
  position: relative;
  gap: 5px;
}

.action-btn:hover {
  background: #f6f8fa;
  color: #0d1117;
  border-color: #e2e8f0;
}

[data-theme="dark"] .action-btn { color: #8b949e; }
[data-theme="dark"] .action-btn:hover { background: rgba(255,255,255,0.07); color: #f0f6fc; border-color: rgba(255,255,255,0.08); }

/* Langue */
.lang-btn {
  width: auto;
  padding: 0 10px;
  gap: 6px;
}
.lang-label {
  font-size: 12px;
  font-weight: 700;
  letter-spacing: 0.5px;
}
.caret {
  color: #9ca3af;
  transition: transform 0.2s;
  flex-shrink: 0;
}
.caret.rotated { transform: rotate(180deg); }

/* Notif dot */
.notif-btn { position: relative; }
.notif-dot {
  position: absolute;
  top: 5px;
  right: 5px;
  min-width: 16px;
  height: 16px;
  padding: 0 4px;
  background: #d97706;
  color: white;
  font-size: 9px;
  font-weight: 800;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid white;
  line-height: 1;
}
[data-theme="dark"] .notif-dot { border-color: #0d1117; }

/* Séparateur */
.sep {
  width: 1px;
  height: 24px;
  background: #e2e8f0;
  margin: 0 6px;
  flex-shrink: 0;
}
[data-theme="dark"] .sep { background: rgba(255,255,255,0.08); }

/* ════════════════════════════════
   PROFIL TRIGGER
════════════════════════════════ */
.profile-trigger {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 4px 10px 4px 4px;
  border-radius: 10px;
  border: 1px solid transparent;
  background: transparent;
  cursor: pointer;
  transition: background 0.15s, border-color 0.15s;
}

.profile-trigger:hover {
  background: #f6f8fa;
  border-color: #e2e8f0;
}
[data-theme="dark"] .profile-trigger:hover {
  background: rgba(255,255,255,0.06);
  border-color: rgba(255,255,255,0.08);
}

.avatar-wrap {
  position: relative;
  flex-shrink: 0;
}

.avatar-img {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #e2e8f0;
  display: block;
}
[data-theme="dark"] .avatar-img { border-color: rgba(255,255,255,0.1); }

.status-dot {
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 9px;
  height: 9px;
  background: #10b981;
  border: 2px solid white;
  border-radius: 50%;
}
[data-theme="dark"] .status-dot { border-color: #0d1117; }

.user-info {
  display: none;
  flex-direction: column;
  text-align: left;
}

@media (min-width: 1024px) {
  .user-info { display: flex; }
}

.user-name {
  font-size: 13px;
  font-weight: 600;
  color: #0d1117;
  line-height: 1.2;
  white-space: nowrap;
}
[data-theme="dark"] .user-name { color: #f0f6fc; }

.user-role {
  font-size: 10px;
  font-weight: 700;
  color: #d97706;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* ════════════════════════════════
   DROPDOWN PANELS
════════════════════════════════ */
.dropdown { position: relative; }

.drop-panel {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  background: #ffffff;
  border: 1px solid #edf0f4;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(13, 17, 23, 0.1), 0 2px 8px rgba(13, 17, 23, 0.06);
  z-index: 1100;
  min-width: 180px;
  overflow: hidden;
}

[data-theme="dark"] .drop-panel {
  background: #161b22;
  border-color: rgba(255, 255, 255, 0.08);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
}

/* Langue */
.lang-panel {
  min-width: 160px;
  padding: 6px;
}

/* Notif */
.notif-panel { width: 340px; padding: 0; }

.panel-head {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px 12px;
  border-bottom: 1px solid #edf0f4;
}
[data-theme="dark"] .panel-head { border-bottom-color: rgba(255,255,255,0.06); }

.panel-title {
  font-size: 14px;
  font-weight: 700;
  color: #0d1117;
}
[data-theme="dark"] .panel-title { color: #f0f6fc; }

.count-chip {
  background: #fef3c7;
  color: #92400e;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 20px;
}
[data-theme="dark"] .count-chip { background: rgba(245,158,11,0.15); color: #fbbf24; }

.notif-scroll {
  max-height: 300px;
  overflow-y: auto;
}
.notif-scroll::-webkit-scrollbar { width: 3px; }
.notif-scroll::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 3px; }

.notif-row {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  padding: 12px 16px;
  border-bottom: 1px solid #f6f8fa;
  transition: background 0.15s;
  cursor: pointer;
}
.notif-row:hover { background: #fafafa; }
[data-theme="dark"] .notif-row { border-bottom-color: rgba(255,255,255,0.04); }
[data-theme="dark"] .notif-row:hover { background: rgba(255,255,255,0.03); }

.notif-icon-wrap {
  width: 28px;
  height: 28px;
  border-radius: 7px;
  background: #fef3c7;
  color: #d97706;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}
[data-theme="dark"] .notif-icon-wrap { background: rgba(245,158,11,0.12); }

.notif-text {
  font-size: 12px;
  font-weight: 600;
  color: #0d1117;
  margin: 0 0 3px;
  line-height: 1.4;
}
[data-theme="dark"] .notif-text { color: #e2e8f0; }

.notif-time {
  font-size: 11px;
  color: #9ca3af;
}

.notif-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 32px 16px;
  color: #9ca3af;
  font-size: 13px;
  font-weight: 500;
}
.empty-ico { opacity: 0.4; }

.panel-foot {
  padding: 10px 16px;
  border-top: 1px solid #edf0f4;
  text-align: center;
}
[data-theme="dark"] .panel-foot { border-top-color: rgba(255,255,255,0.06); }

.mark-read-btn {
  background: none;
  border: none;
  font-size: 12px;
  font-weight: 600;
  color: #d97706;
  cursor: pointer;
  font-family: 'DM Sans', sans-serif;
}
.mark-read-btn:hover { color: #b45309; }

/* Profil panel */
.profile-panel {
  min-width: 220px;
  padding: 6px;
  right: 0;
}

.profile-head {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 10px 12px 12px;
  background: #f6f8fa;
  border-radius: 8px;
  margin-bottom: 4px;
}
[data-theme="dark"] .profile-head { background: rgba(255,255,255,0.04); }

.ph-avatar {
  width: 36px;
  height: 36px;
  border-radius: 8px;
  object-fit: cover;
  border: 1px solid #e2e8f0;
}

.ph-name {
  font-size: 13px;
  font-weight: 700;
  color: #0d1117;
  line-height: 1.2;
}
[data-theme="dark"] .ph-name { color: #f0f6fc; }

.ph-role {
  font-size: 10px;
  font-weight: 700;
  color: #d97706;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

/* Drop items */
.drop-item {
  display: flex;
  align-items: center;
  gap: 9px;
  padding: 9px 12px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #374151;
  text-decoration: none;
  cursor: pointer;
  background: none;
  border: none;
  width: 100%;
  text-align: left;
  font-family: 'DM Sans', sans-serif;
  transition: background 0.13s, color 0.13s;
}
.drop-item:hover { background: #f6f8fa; color: #0d1117; }
[data-theme="dark"] .drop-item { color: #8b949e; }
[data-theme="dark"] .drop-item:hover { background: rgba(255,255,255,0.06); color: #f0f6fc; }

.drop-item.active { color: #d97706; font-weight: 600; }
.drop-item.danger { color: #e11d48; }
.drop-item.danger:hover { background: #fff1f2; color: #be123c; }
[data-theme="dark"] .drop-item.danger:hover { background: rgba(225,29,72,0.08); }

.di-icon { color: #d97706; flex-shrink: 0; }
.drop-item.danger .di-icon { color: inherit; }

.check-ico { margin-left: auto; color: #d97706; }
.flag { font-size: 16px; }

.drop-divider {
  height: 1px;
  background: #edf0f4;
  margin: 4px 0;
}
[data-theme="dark"] .drop-divider { background: rgba(255,255,255,0.06); }

/* ════════════════════════════════
   OVERLAY CLICK OUTSIDE
════════════════════════════════ */
.click-outside {
  position: fixed;
  inset: 0;
  z-index: 1099;
}

/* ════════════════════════════════
   TRANSITIONS
════════════════════════════════ */
.dropdown-fade-enter-active { transition: opacity 0.15s ease, transform 0.15s ease; }
.dropdown-fade-leave-active { transition: opacity 0.1s ease, transform 0.1s ease; }
.dropdown-fade-enter-from  { opacity: 0; transform: translateY(-6px); }
.dropdown-fade-leave-to    { opacity: 0; transform: translateY(-4px); }

.icon-swap-enter-active, .icon-swap-leave-active { transition: all 0.2s ease; }
.icon-swap-enter-from, .icon-swap-leave-to { opacity: 0; transform: scale(0.6) rotate(30deg); }
</style>