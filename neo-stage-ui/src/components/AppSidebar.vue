<template>
  <div>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <button @click="toggleSidebar" class="mobile-toggle-btn d-lg-none">
      <i :class="isSidebarActive ? 'fa-solid fa-xmark' : 'fa-solid fa-bars-staggered'"></i>
    </button>

   

      <!-- HEADER -->
      <div class="sidebar-header">
        <div class="brand-row">
          <div class="brand-hex-icon">
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
              <defs>
                <linearGradient id="hex-grad" x1="0%" y1="0%" x2="100%" y2="100%">
                  <stop offset="0%" stop-color="#f5c842"/>
                  <stop offset="100%" stop-color="#f0a012"/>
                </linearGradient>
              </defs>
              <path d="M50 10 L85 30 L85 70 L50 90 L15 70 L15 30 Z" stroke="url(#hex-grad)" stroke-width="6" fill="none"/>
              <path d="M50 25 L72 38 L72 62 L50 75 L28 62 L28 38 Z" fill="url(#hex-grad)"/>
              <circle cx="50" cy="50" r="8" fill="white"/>
            </svg>
          </div>
          <div>
            <div class="brand-name">Evalua<span>Tech</span></div>
            <div class="brand-tagline">Smart Evaluation</div>
          </div>
        </div>
        <div class="role-pill">
          <span class="status-dot"></span>
          {{ roleLabel }}
        </div>
      </div>

      <!-- PROFIL -->
      <div class="profile-section">
        <div class="profile-card">
          <div class="avatar">{{ authStore.user?.name?.charAt(0)?.toUpperCase() || 'U' }}</div>
          <div class="profile-info">
            <span class="profile-name">{{ authStore.user?.name || 'Utilisateur' }}</span>
            <span class="profile-status">
              <span class="online-dot"></span>
              Membre actif
            </span>
          </div>
          <div class="profile-chevron"><i class="fa-solid fa-chevron-right"></i></div>
        </div>
      </div>

      <!-- NAVIGATION SCROLLABLE -->
      <div class="nav-body">

        <!-- Dashboard commun -->
        <router-link to="/dashboard" class="nav-item nav-dashboard">
          <div class="nav-icon-wrap"><i class="fa-solid fa-house-user"></i></div>
          <span>Tableau de bord</span>
          <i class="fa-solid fa-chevron-right nav-arrow"></i>
        </router-link>

        <!-- ESPACE ENTREPRISE -->
        <div v-if="userRole !== 'SuperAdmin' && userRole !== 'Candidat'" class="nav-group">

          <span class="group-label">Général</span>
          <router-link to="/dashboard" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-gauge-high"></i></div>
            <span>Vue d'ensemble</span>
          </router-link>

          <template v-if="authStore.hasPermission('view_can') || authStore.hasPermission('inv_can')">
            <span class="group-label">Recrutement & Candidats</span>
            <router-link v-if="authStore.hasPermission('view_can')" to="/candidates-list" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-users-viewfinder"></i></div>
              <span>Liste des candidats</span>
            </router-link>
            <router-link v-if="authStore.hasPermission('inv_can')" to="/invite" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-paper-plane"></i></div>
              <span>Invitations</span>
            </router-link>
            <router-link v-if="authStore.hasPermission('view_can')" to="/groups" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-object-group"></i></div>
              <span>Groupes</span>
            </router-link>
          </template>

          <template v-if="authStore.hasPermission('view_tests') || authStore.hasPermission('inv_can') || authStore.hasPermission('edit_bank')">
            <span class="group-label">Évaluations & Correction</span>
            <router-link v-if="authStore.hasPermission('view_tests') || authStore.hasPermission('inv_can')" to="/campaigns" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-rectangle-list"></i></div>
              <span>Campagnes (Tests)</span>
              <span class="nav-badge">{{ campaignCount }}</span>
            </router-link>
            <router-link v-if="authStore.hasPermission('edit_bank')" to="/questions" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-database"></i></div>
              <span>Banque de questions</span>
            </router-link>
            <router-link v-if="authStore.hasPermission('edit_bank')" to="/ai-generator" class="nav-item nav-ai">
              <div class="nav-icon-wrap"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
              <span>Générateur IA</span>
              <span class="nav-badge-ai">NEW</span>
            </router-link>
          </template>

          <template v-if="authStore.hasPermission('view_tests')">
            <span class="group-label">Analytique</span>
            <router-link to="/analyse-comportementale" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-microchip"></i></div>
              <span>Smart Analysis</span>
            </router-link>
            <router-link to="/stats" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-chart-pie"></i></div>
              <span>Statistiques</span>
            </router-link>
          </template>

          <template v-if="authStore.hasPermission('view_staff') || authStore.hasPermission('view_rol') || userRole === 'AdminEntreprise'">
            <span class="group-label">Organisation</span>
            <router-link v-if="authStore.hasPermission('view_staff')" to="/staff-members" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-user-gear"></i></div>
              <span>Staff & Employés</span>
            </router-link>
            <router-link v-if="authStore.hasPermission('view_rol')" to="/roles" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-shield-halved"></i></div>
              <span>Rôles & Permissions</span>
            </router-link>
            <router-link v-if="userRole === 'AdminEntreprise'" to="/settings" class="nav-item">
              <div class="nav-icon-wrap"><i class="fa-solid fa-sliders"></i></div>
              <span>Paramètres org</span>
            </router-link>
          </template>

        </div>

        <!-- Super Admin -->
        <div v-if="userRole === 'SuperAdmin'" class="nav-group">
          <span class="group-label">Master Console</span>
          <router-link to="/super-admin" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-server"></i></div>
            <span>Voir organisations</span>
          </router-link>
          <router-link to="/platform-users" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-users-gear"></i></div>
            <span>Gérer utilisateurs</span>
          </router-link>
          <router-link to="/platform-analytics" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-globe"></i></div>
            <span>Analytique globale</span>
          </router-link>
        </div>

        <!-- Candidat -->
        <div v-if="userRole === 'Candidat'" class="nav-group">
          <span class="group-label">Mon parcours</span>
          <router-link to="/my-tests" class="nav-item nav-success">
            <div class="nav-icon-wrap"><i class="fa-solid fa-play"></i></div>
            <span>Passer un test</span>
          </router-link>
          <router-link to="/results" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-award"></i></div>
            <span>Mes résultats</span>
          </router-link>
          <router-link to="/history" class="nav-item">
            <div class="nav-icon-wrap"><i class="fa-solid fa-clock-rotate-left"></i></div>
            <span>Historique</span>
          </router-link>
        </div>

        <!-- Scroll bottom spacer -->
        <div class="scroll-spacer"></div>
      </div>


        <button @click="logout" class="logout-btn">
          <i class="fa-solid fa-power-off"></i>
          <span>Déconnexion</span>
        </button>
      </div>
  

    <div v-if="isSidebarActive" class="sidebar-overlay" @click="toggleSidebar"></div>

</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

const router          = useRouter();
const authStore       = useAuthStore();
const userRole        = computed(() => authStore.role);
const isSidebarActive = ref(false);
const campaignCount   = ref(0);

const roleLabel = computed(() => {
  const map = {
    SuperAdmin:      'SuperAdmin',
    AdminEntreprise: 'Administrateur',
    Recruteur:       'RH / Recruteur',
    Evaluateur:      'Évaluateur',
    Candidat:        'Candidat',
  };
  return map[userRole.value] || 'Utilisateur';
});

const fetchCounts = async () => {
  try {
    const res = await api.get('/Campagnes');
    campaignCount.value = res.data.length;
  } catch (err) {
    console.error('Erreur sidebar counts:', err);
  }
};

const toggleSidebar = () => { isSidebarActive.value = !isSidebarActive.value; };

const changeRole = (e) => {
  authStore.setUser(authStore.user, e.target.value, authStore.token);
  router.push('/dashboard').then(() => window.location.reload());
};

const logout = () => {
  authStore.logout();
  router.push('/login');
};

onMounted(fetchCounts);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@500;600&display=swap');

/* ═══════════════════════════════════════════════════════════
   MOBILE TOGGLE
═══════════════════════════════════════════════════════════ */
.mobile-toggle-btn {
  position: fixed;
  top: 14px;
  left: 14px;
  z-index: 2000;
  width: 40px;
  height: 40px;
  border-radius: 11px;
  background: #ffffff;
  border: 1px solid #e8e7e1;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #2a2a4a;
  font-size: 15px;
  transition: background 0.2s, box-shadow 0.2s;
}

.mobile-toggle-btn:hover {
  background: #f4f3ef;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.1);
}

/* ═══════════════════════════════════════════════════════════
   SIDEBAR SHELL
═══════════════════════════════════════════════════════════ */
.sidebar-pro {
  width: 264px;
  min-width: 264px;
  height: 100vh;
  position: sticky;
  top: 0;
  display: flex;
  flex-direction: column;
  background: #ffffff;
  border-right: 1px solid #eceae3;
  font-family: 'Inter', sans-serif;
  transition: transform 0.38s cubic-bezier(0.16, 1, 0.3, 1);
  z-index: 1000;
  overflow: hidden;
}

/* ═══════════════════════════════════════════════════════════
   HEADER
═══════════════════════════════════════════════════════════ */
.sidebar-header {
  padding: 20px 18px 14px;
  border-bottom: 1px solid #f2f1eb;
  flex-shrink: 0;
}

.brand-row {
  display: flex;
  align-items: center;
  gap: 11px;
  margin-bottom: 12px;
}

.brand-hex-icon {
  width: 38px;
  height: 38px;
  flex-shrink: 0;
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  border-radius: 11px;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 6px;
  border: 1px solid rgba(245, 200, 66, 0.3);
}

.brand-hex-icon svg {
  width: 100%;
  height: 100%;
}

.brand-name {
  font-size: 16px;
  font-weight: 800;
  color: #0f0e17;
  letter-spacing: -0.5px;
  line-height: 1.15;
}

.brand-name span {
  color: #f0a012;
}

.brand-tagline {
  font-size: 9.5px;
  font-weight: 700;
  color: #b0b0c0;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-top: 2px;
}

/* Role pill */
.role-pill {
  display: inline-flex;
  align-items: center;
  gap: 7px;
  background: #f7f6f0;
  border: 1px solid #e8e7e1;
  border-radius: 999px;
  padding: 4px 12px;
  font-size: 10px;
  font-weight: 700;
  color: #6a6a8a;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.status-dot {
  width: 6px;
  height: 6px;
  background: #22c55e;
  border-radius: 50%;
  flex-shrink: 0;
  box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.4);
  animation: statusPulse 2.5s infinite;
}

@keyframes statusPulse {
  0%   { box-shadow: 0 0 0 0 rgba(34, 197, 94, 0.45); }
  60%  { box-shadow: 0 0 0 5px rgba(34, 197, 94, 0); }
  100% { box-shadow: 0 0 0 0 rgba(34, 197, 94, 0); }
}

/* ═══════════════════════════════════════════════════════════
   PROFILE CARD
═══════════════════════════════════════════════════════════ */
.profile-section {
  padding: 10px 14px 6px;
  flex-shrink: 0;
}

.profile-card {
  background: linear-gradient(135deg, #0f0e17 0%, #1e1d2e 100%);
  border-radius: 14px;
  padding: 12px 14px;
  display: flex;
  align-items: center;
  gap: 11px;
  cursor: pointer;
  transition: opacity 0.2s;
  border: 1px solid rgba(255, 255, 255, 0.04);
}

.profile-card:hover {
  opacity: 0.92;
}

.avatar {
  width: 36px;
  height: 36px;
  border-radius: 10px;
  background: linear-gradient(135deg, #f5c842, #f0a012);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 800;
  color: #1a1207;
  flex-shrink: 0;
  position: relative;
  letter-spacing: 0;
}

.profile-info {
  display: flex;
  flex-direction: column;
  gap: 3px;
  min-width: 0;
}

.profile-name {
  font-size: 13px;
  font-weight: 700;
  color: #f5f4f0;
  line-height: 1.2;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.profile-status {
  font-size: 10px;
  color: #6a6a9a;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 5px;
}

.online-dot {
  width: 5px;
  height: 5px;
  background: #22c55e;
  border-radius: 50%;
  flex-shrink: 0;
  display: inline-block;
}

.profile-chevron {
  margin-left: auto;
  color: #3a3a5a;
  font-size: 10px;
  flex-shrink: 0;
}

/* ═══════════════════════════════════════════════════════════
   NAV BODY — SCROLLABLE
═══════════════════════════════════════════════════════════ */
.nav-body {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 8px 12px 0;
  /* custom scrollbar */
  scrollbar-width: thin;
  scrollbar-color: #e2e0d8 transparent;
}

.nav-body::-webkit-scrollbar {
  width: 4px;
}

.nav-body::-webkit-scrollbar-track {
  background: transparent;
}

.nav-body::-webkit-scrollbar-thumb {
  background: #dddbd2;
  border-radius: 4px;
}

.nav-body::-webkit-scrollbar-thumb:hover {
  background: #c8c6be;
}

/* Fade-out mask at bottom of scroll area */
.nav-body::after {
  content: '';
  display: block;
  height: 24px;
  background: transparent;
}

/* Group label */
.group-label {
  display: block;
  font-size: 9.5px;
  font-weight: 700;
  color: #c0bfb8;
  text-transform: uppercase;
  letter-spacing: 0.12em;
  padding: 16px 8px 6px;
  user-select: none;
}

/* ── NAV ITEM ── */
.nav-item {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 9px 11px;
  border-radius: 10px;
  font-size: 13px;
  font-weight: 500;
  color: #5a5a7a;
  text-decoration: none;
  margin-bottom: 2px;
  transition:
    background 0.18s ease,
    color 0.18s ease,
    transform 0.18s ease;
  position: relative;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.nav-item:hover {
  background: #f4f3ef;
  color: #0f0e17;
  transform: translateX(2px);
}

/* Active state via Vue Router */
.nav-item.router-link-active,
.nav-item.router-link-exact-active {
  background: #fffbea;
  color: #b07010;
  font-weight: 700;
}

/* Left accent bar on active */
.nav-item.router-link-active::before {
  content: '';
  position: absolute;
  left: 0;
  top: 25%;
  bottom: 25%;
  width: 3px;
  background: linear-gradient(180deg, #f5c842, #f0a012);
  border-radius: 0 3px 3px 0;
}

/* Icon wrap */
.nav-icon-wrap {
  width: 28px;
  height: 28px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 8px;
  font-size: 12px;
  flex-shrink: 0;
  color: inherit;
  background: rgba(0, 0, 0, 0.03);
  transition: background 0.18s;
}

.nav-item:hover .nav-icon-wrap,
.nav-item.router-link-active .nav-icon-wrap {
  background: rgba(245, 200, 66, 0.12);
  color: #c08010;
}

/* Arrow */
.nav-arrow {
  margin-left: auto;
  font-size: 9px;
  opacity: 0.25;
  flex-shrink: 0;
}

/* ── DASHBOARD ITEM (special) ── */
.nav-dashboard {
  background: #f4f3ef;
  color: #0f0e17;
  font-weight: 600;
  margin-bottom: 10px;
  border: 1px solid #eceae3;
}

.nav-dashboard .nav-icon-wrap {
  background: #0f0e17;
  color: #f5c842;
}

.nav-dashboard:hover {
  background: #ede9e0;
  border-color: #dddad2;
}

/* ── AI GENERATOR ITEM ── */
.nav-ai {
  background: linear-gradient(135deg, #faf5ff, #f5f0ff);
  color: #7c3aed;
  border: 1px solid #ede9fe;
}

.nav-ai .nav-icon-wrap {
  background: rgba(124, 58, 237, 0.1);
  color: #7c3aed;
}

.nav-ai:hover {
  background: linear-gradient(135deg, #f3ebff, #ede4ff);
  color: #6d28d9;
  transform: translateX(2px);
}

/* ── SUCCESS (Candidat - Passer un test) ── */
.nav-success {
  background: linear-gradient(135deg, #f0fdf4, #e8fdf0);
  color: #15803d;
  border: 1px solid #dcfce7;
}

.nav-success .nav-icon-wrap {
  background: rgba(21, 128, 61, 0.1);
  color: #15803d;
}

.nav-success:hover {
  background: linear-gradient(135deg, #dcfce7, #d0fae5);
  color: #166534;
  transform: translateX(2px);
}

/* ── BADGES ── */
.nav-badge {
  margin-left: auto;
  background: #fde68a;
  color: #92400e;
  font-size: 10px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 999px;
  flex-shrink: 0;
  font-family: 'JetBrains Mono', monospace;
  letter-spacing: 0;
}

.nav-badge-ai {
  margin-left: auto;
  background: linear-gradient(135deg, #7c3aed, #6d28d9);
  color: #ffffff;
  font-size: 8.5px;
  font-weight: 800;
  padding: 2px 7px;
  border-radius: 999px;
  flex-shrink: 0;
  letter-spacing: 0.06em;
  text-transform: uppercase;
}

/* Scroll spacer */
.scroll-spacer {
  height: 16px;
}

/* ═══════════════════════════════════════════════════════════
   FOOTER
═══════════════════════════════════════════════════════════ */
.sidebar-footer {
  padding: 12px 14px 18px;
  border-top: 1px solid #f2f1eb;
  flex-shrink: 0;
  background: #ffffff;
}

/* Env card */
.env-card {
  background: #f7f6f0;
  border: 1px solid #eceae3;
  border-radius: 11px;
  padding: 10px 13px;
  margin-bottom: 10px;
}

.env-label {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 9px;
  font-weight: 700;
  color: #b0b0c0;
  text-transform: uppercase;
  letter-spacing: 0.1em;
  margin-bottom: 6px;
}

.env-label i {
  color: #f0a012;
  font-size: 10px;
}

.select-wrapper {
  position: relative;
  display: flex;
  align-items: center;
}

.select-wrapper select {
  width: 100%;
  border: none;
  background: transparent;
  font-size: 12.5px;
  font-weight: 700;
  color: #0f0e17;
  appearance: none;
  -webkit-appearance: none;
  cursor: pointer;
  outline: none;
  padding-right: 18px;
  font-family: 'Inter', sans-serif;
}

.select-wrapper select:focus {
  color: #f0a012;
}

.select-icon {
  position: absolute;
  right: 0;
  font-size: 9px;
  color: #f0a012;
  pointer-events: none;
}

/* Logout button */
.logout-btn {
  width: 100%;
  background: rgba(224, 80, 80, 0.05);
  border: 1px solid rgba(224, 80, 80, 0.2);
  border-radius: 10px;
  padding: 10px;
  color: #e05050;
  font-size: 12.5px;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
  transition: background 0.2s, border-color 0.2s, transform 0.15s;
  font-family: 'Inter', sans-serif;
  letter-spacing: 0.02em;
}

.logout-btn:hover {
  background: rgba(224, 80, 80, 0.1);
  border-color: rgba(224, 80, 80, 0.35);
  transform: translateY(-1px);
}

.logout-btn:active {
  transform: translateY(0);
}

/* ═══════════════════════════════════════════════════════════
   MOBILE / RESPONSIVE
═══════════════════════════════════════════════════════════ */
@media (max-width: 991px) {
  .sidebar-pro {
    position: fixed;
    left: 0;
    top: 0;
    transform: translateX(-100%);
    box-shadow: none;
  }

  .sidebar-pro.active {
    transform: translateX(0);
    box-shadow: 8px 0 32px rgba(0, 0, 0, 0.12);
  }
}

/* Overlay */
.sidebar-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 14, 23, 0.45);
  z-index: 999;
  backdrop-filter: blur(3px);
  -webkit-backdrop-filter: blur(3px);
  animation: overlayIn 0.25s ease;
}

@keyframes overlayIn {
  from { opacity: 0; }
  to   { opacity: 1; }
}

/* ═══════════════════════════════════════════════════════════
   DARK MODE  [data-theme="dark"]
═══════════════════════════════════════════════════════════ */
[data-theme="dark"] .sidebar-pro {
  background: #0f0e17;
  border-right-color: rgba(255, 255, 255, 0.06);
}

[data-theme="dark"] .sidebar-header {
  border-bottom-color: rgba(255, 255, 255, 0.06);
}

[data-theme="dark"] .brand-name {
  color: #f5f4f0;
}

[data-theme="dark"] .role-pill {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.08);
  color: #8a8aaa;
}

[data-theme="dark"] .group-label {
  color: #3a3a5a;
}

[data-theme="dark"] .nav-item {
  color: #8a8aaa;
}

[data-theme="dark"] .nav-item:hover {
  background: rgba(255, 255, 255, 0.05);
  color: #f5f4f0;
}

[data-theme="dark"] .nav-item.router-link-active {
  background: rgba(245, 200, 66, 0.08);
  color: #f5c842;
}

[data-theme="dark"] .nav-item:hover .nav-icon-wrap,
[data-theme="dark"] .nav-item.router-link-active .nav-icon-wrap {
  background: rgba(245, 200, 66, 0.12);
}

[data-theme="dark"] .nav-dashboard {
  background: rgba(255, 255, 255, 0.06);
  color: #f5f4f0;
  border-color: rgba(255, 255, 255, 0.08);
}

[data-theme="dark"] .nav-dashboard .nav-icon-wrap {
  background: rgba(245, 200, 66, 0.15);
}

[data-theme="dark"] .nav-ai {
  background: rgba(124, 58, 237, 0.1);
  border-color: rgba(124, 58, 237, 0.2);
  color: #a78bfa;
}

[data-theme="dark"] .nav-success {
  background: rgba(34, 197, 94, 0.07);
  border-color: rgba(34, 197, 94, 0.15);
  color: #4ade80;
}

[data-theme="dark"] .nav-body::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1);
}

[data-theme="dark"] .sidebar-footer {
  background: #0f0e17;
  border-top-color: rgba(255, 255, 255, 0.06);
}

[data-theme="dark"] .env-card {
  background: rgba(255, 255, 255, 0.03);
  border-color: rgba(255, 255, 255, 0.07);
}

[data-theme="dark"] .select-wrapper select {
  color: #e8e7e0;
}

[data-theme="dark"] .logout-btn {
  background: rgba(224, 80, 80, 0.07);
  border-color: rgba(224, 80, 80, 0.2);
}

[data-theme="dark"] .logout-btn:hover {
  background: rgba(224, 80, 80, 0.14);
}

[data-theme="dark"] .mobile-toggle-btn {
  background: #1e1d2e;
  border-color: rgba(255, 255, 255, 0.1);
  color: #8a8aaa;
}
</style>