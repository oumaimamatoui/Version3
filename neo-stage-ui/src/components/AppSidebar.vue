<template>
  <div>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <button @click="toggleSidebar" class="mobile-toggle-btn d-lg-none">
      <i :class="isSidebarActive ? 'fa-solid fa-xmark' : 'fa-solid fa-bars-staggered'"></i>
    </button>

    <nav :class="['sidebar-pro', isSidebarActive ? 'active' : '']">

      <!-- HEADER -->
      <div class="sidebar-header">
        <div class="brand-row">
          <svg class="brand-logo" viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg">
            <defs>
              <linearGradient id="hex-grad" x1="0%" y1="0%" x2="100%" y2="100%">
                <stop offset="0%" stop-color="#f59e0b"/>
                <stop offset="100%" stop-color="#d97706"/>
              </linearGradient>
            </defs>
            <path d="M50 10 L85 30 L85 70 L50 90 L15 70 L15 30 Z" stroke="url(#hex-grad)" stroke-width="6" fill="none"/>
            <path d="M50 25 L72 38 L72 62 L50 75 L28 62 L28 38 Z" fill="url(#hex-grad)"/>
            <circle cx="50" cy="50" r="8" fill="white"/>
          </svg>
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
          <div class="avatar">{{ authStore.user?.name?.charAt(0) || 'U' }}</div>
          <div class="profile-info">
            <span class="profile-name">{{ authStore.user?.name || 'Utilisateur' }}</span>
            <span class="profile-status">Membre actif</span>
          </div>
          <div class="profile-chevron"><i class="fa-solid fa-chevron-right"></i></div>
        </div>
      </div>

      <!-- NAVIGATION -->
      <div class="nav-body">

        <!-- Dashboard commun -->
        <router-link to="/dashboard" class="nav-item nav-dashboard">
          <div class="nav-icon"><i class="fa-solid fa-house-user"></i></div>
          <span>Tableau de bord</span>
          <i class="fa-solid fa-chevron-right nav-arrow"></i>
        </router-link>

        <!-- Admin Entreprise / Recruteur -->
        <div v-if="userRole === 'AdminEntreprise' || userRole === 'Recruteur'" class="nav-group">

          <span class="group-label">Gestion système</span>
          <router-link to="/dashboard" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-gauge-high"></i></div>
            <span>Vue d'ensemble</span>
          </router-link>

          <span class="group-label">Ressources humaines</span>
          <router-link to="/candidates-list" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-users-viewfinder"></i></div>
            <span>Candidats</span>
          </router-link>
          <router-link to="/invite" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-paper-plane"></i></div>
            <span>Invitations</span>
          </router-link>
          <router-link to="/groups" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-object-group"></i></div>
            <span>Groupes</span>
          </router-link>

          <span class="group-label">Évaluations</span>
          <router-link to="/campaigns" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-rectangle-list"></i></div>
            <span>Campagnes</span>
            <span class="nav-badge">{{ campaignCount }}</span>
          </router-link>

          <span class="group-label">Intelligence artificielle</span>
          <router-link to="/ai-generator" class="nav-item nav-ai">
            <div class="nav-icon"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
            <span>Générer des questions</span>
          </router-link>
          <router-link to="/questions" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-database"></i></div>
            <span>Banque de données</span>
          </router-link>

          <span class="group-label">Organisation</span>
          <router-link to="/staff-members" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-user-gear"></i></div>
            <span>Membres du personnel</span>
          </router-link>
          <router-link to="/roles" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-shield-halved"></i></div>
            <span>Rôles & Permissions</span>
          </router-link>
          <router-link to="/settings" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-sliders"></i></div>
            <span>Paramètres org</span>
          </router-link>
        </div>

        <!-- Évaluateur -->
        <div v-if="userRole === 'Evaluateur'" class="nav-group">
          <span class="group-label">Outils d'évaluation</span>
          <router-link to="/questions" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-database"></i></div>
            <span>Banque de questions</span>
          </router-link>
          <router-link to="/campaigns" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-clipboard-check"></i></div>
            <span>Évaluations en cours</span>
          </router-link>

          <span class="group-label">IA & Analytics</span>
          <router-link to="/ai-generator" class="nav-item nav-ai">
            <div class="nav-icon"><i class="fa-solid fa-brain"></i></div>
            <span>IA Assistant</span>
          </router-link>
          <router-link to="/analyse-comportementale" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-microchip"></i></div>
            <span>Smart Analysis</span>
          </router-link>
          <router-link to="/stats" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-chart-pie"></i></div>
            <span>Statistiques</span>
          </router-link>
        </div>

        <!-- Super Admin -->
        <div v-if="userRole === 'SuperAdmin'" class="nav-group">
          <span class="group-label">Master Console</span>
          <router-link to="/super-admin" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-server"></i></div>
            <span>Voir organisations</span>
          </router-link>
          <router-link to="/platform-users" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-users-gear"></i></div>
            <span>Gérer utilisateurs</span>
          </router-link>
          <router-link to="/platform-analytics" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-globe"></i></div>
            <span>Analytique globale</span>
          </router-link>
        </div>

        <!-- Candidat -->
        <div v-if="userRole === 'Candidat'" class="nav-group">
          <span class="group-label">Mon parcours</span>
          <router-link to="/exam-lobby" class="nav-item nav-success">
            <div class="nav-icon"><i class="fa-solid fa-play"></i></div>
            <span>Passer un test</span>
          </router-link>
          <router-link to="/results" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-award"></i></div>
            <span>Mes résultats</span>
          </router-link>
          <router-link to="/history" class="nav-item">
            <div class="nav-icon"><i class="fa-solid fa-clock-rotate-left"></i></div>
            <span>Historique</span>
          </router-link>
        </div>

      </div>

      <!-- FOOTER -->
      <div class="sidebar-footer">
        <div class="env-card">
          <label class="env-label">
            <i class="fa-solid fa-flask-vial"></i>
            Environnement test
          </label>
          <div class="select-wrapper">
            <select @change="changeRole($event)">
              <option value="Candidat"          :selected="userRole === 'Candidat'">Candidat</option>
              <option value="Evaluateur"        :selected="userRole === 'Evaluateur'">Évaluateur</option>
              <option value="Recruteur"         :selected="userRole === 'Recruteur'">Recruteur</option>
              <option value="AdminEntreprise"   :selected="userRole === 'AdminEntreprise'">Admin Entreprise</option>
            </select>
            <i class="fa-solid fa-chevron-up select-icon"></i>
          </div>
        </div>
        <button @click="logout" class="logout-btn">
          <i class="fa-solid fa-power-off"></i>
          <span>Déconnexion</span>
        </button>
      </div>
    </nav>

    <div v-if="isSidebarActive" class="sidebar-overlay" @click="toggleSidebar"></div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';

const router       = useRouter();
const authStore    = useAuthStore();
const userRole     = computed(() => authStore.role);
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
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap');

/* ── STRUCTURE ── */
.sidebar-pro {
  width: 260px;
  min-width: 260px;
  height: 100vh;
  position: sticky;
  top: 0;
  display: flex;
  flex-direction: column;
  background: #ffffff;
  border-right: 1px solid #f1f5f9;
  font-family: 'Inter', sans-serif;
  transition: transform 0.35s cubic-bezier(0.16, 1, 0.3, 1);
  z-index: 1000;
}

[data-theme="dark"] .sidebar-pro {
  background: #0f172a;
  border-right-color: rgba(255,255,255,0.06);
}

/* ── HEADER ── */
.sidebar-header {
  padding: 22px 18px 14px;
  border-bottom: 1px solid #f1f5f9;
}

[data-theme="dark"] .sidebar-header { border-bottom-color: rgba(255,255,255,0.06); }

.brand-row { display: flex; align-items: center; gap: 10px; margin-bottom: 10px; }

.brand-logo {
  width: 34px;
  height: 34px;
  flex-shrink: 0;
}

.brand-name {
  font-size: 17px;
  font-weight: 700;
  color: #0f172a;
  letter-spacing: -0.4px;
  line-height: 1.1;
}
.brand-name span { color: #d97706; }
[data-theme="dark"] .brand-name { color: #f8fafc; }

.brand-tagline {
  font-size: 10px;
  font-weight: 600;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  margin-top: 2px;
}

.role-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 20px;
  padding: 3px 10px;
  font-size: 10px;
  font-weight: 600;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.6px;
}
[data-theme="dark"] .role-pill {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.08);
  color: #94a3b8;
}

.status-dot {
  width: 5px;
  height: 5px;
  background: #10b981;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

/* ── PROFIL ── */
.profile-section { padding: 10px 14px; }

.profile-card {
  background: #0f172a;
  border-radius: 10px;
  padding: 11px 13px;
  display: flex;
  align-items: center;
  gap: 10px;
}

.avatar {
  width: 34px;
  height: 34px;
  border-radius: 8px;
  background: linear-gradient(135deg, #f59e0b, #d97706);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 14px;
  font-weight: 700;
  color: white;
  flex-shrink: 0;
  position: relative;
}

.avatar::after {
  content: '';
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 9px;
  height: 9px;
  background: #10b981;
  border: 2px solid #0f172a;
  border-radius: 50%;
}

.profile-name  { display: block; color: #f8fafc; font-size: 13px; font-weight: 600; }
.profile-status { font-size: 10px; color: #64748b; font-weight: 500; }
.profile-chevron { margin-left: auto; color: #334155; font-size: 11px; }

/* ── NAV BODY ── */
.nav-body {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  padding: 8px 12px;
}

.nav-body::-webkit-scrollbar { width: 3px; }
.nav-body::-webkit-scrollbar-track { background: transparent; }
.nav-body::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 3px; }
[data-theme="dark"] .nav-body::-webkit-scrollbar-thumb { background: rgba(255,255,255,0.1); }

.group-label {
  display: block;
  font-size: 9.5px;
  font-weight: 700;
  color: #cbd5e1;
  text-transform: uppercase;
  letter-spacing: 1px;
  padding: 14px 6px 5px;
}
[data-theme="dark"] .group-label { color: #475569; }

.nav-item {
  display: flex;
  align-items: center;
  gap: 9px;
  padding: 8px 10px;
  border-radius: 8px;
  font-size: 13px;
  font-weight: 500;
  color: #475569;
  text-decoration: none;
  margin-bottom: 1px;
  transition: background 0.15s ease, color 0.15s ease, transform 0.15s ease;
}

.nav-item:hover {
  background: #f8fafc;
  color: #0f172a;
  transform: translateX(3px);
}

.nav-item.router-link-active {
  background: #fffbeb;
  color: #b45309;
  font-weight: 600;
}

[data-theme="dark"] .nav-item { color: #94a3b8; }
[data-theme="dark"] .nav-item:hover { background: rgba(255,255,255,0.05); color: #f1f5f9; }
[data-theme="dark"] .nav-item.router-link-active { background: rgba(245,158,11,0.1); color: #fbbf24; }

.nav-icon {
  width: 26px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  flex-shrink: 0;
  color: inherit;
}

.nav-arrow { margin-left: auto; font-size: 10px; opacity: 0.35; }

/* Variantes */
.nav-dashboard {
  background: #f1f5f9;
  color: #0f172a;
  font-weight: 600;
  margin-bottom: 8px;
}
[data-theme="dark"] .nav-dashboard { background: rgba(255,255,255,0.07); color: #f8fafc; }

.nav-ai {
  background: #faf5ff;
  color: #7c3aed;
  border: 1px solid #ede9fe;
}
[data-theme="dark"] .nav-ai { background: rgba(139,92,246,0.1); border-color: rgba(139,92,246,0.2); color: #a78bfa; }

.nav-success {
  background: #f0fdf4;
  color: #15803d;
  border: 1px solid #dcfce7;
}
[data-theme="dark"] .nav-success { background: rgba(34,197,94,0.08); border-color: rgba(34,197,94,0.15); color: #4ade80; }

.nav-badge {
  margin-left: auto;
  background: #fde68a;
  color: #92400e;
  font-size: 10px;
  font-weight: 700;
  padding: 1px 7px;
  border-radius: 10px;
}

/* ── FOOTER ── */
.sidebar-footer {
  padding: 12px 14px 18px;
  border-top: 1px solid #f1f5f9;
}
[data-theme="dark"] .sidebar-footer { border-top-color: rgba(255,255,255,0.06); }

.env-card {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 9px 11px;
  margin-bottom: 10px;
}
[data-theme="dark"] .env-card { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.07); }

.env-label {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 9px;
  font-weight: 700;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.8px;
  margin-bottom: 5px;
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
  font-size: 12px;
  font-weight: 600;
  color: #1e293b;
  appearance: none;
  cursor: pointer;
  outline: none;
  padding-right: 16px;
}
[data-theme="dark"] .select-wrapper select { color: #e2e8f0; }

.select-icon {
  position: absolute;
  right: 0;
  font-size: 9px;
  color: #d97706;
  pointer-events: none;
}

.logout-btn {
  width: 100%;
  background: #fff1f2;
  border: 1px solid #fee2e2;
  border-radius: 8px;
  padding: 9px;
  color: #e11d48;
  font-size: 12px;
  font-weight: 600;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
  transition: background 0.2s ease, transform 0.2s ease;
}
.logout-btn:hover { background: #ffe4e6; transform: translateY(-1px); }

/* ── MOBILE ── */
.mobile-toggle-btn {
  position: fixed;
  top: 14px;
  left: 14px;
  z-index: 2000;
  width: 38px;
  height: 38px;
  border-radius: 9px;
  background: white;
  border: 1px solid #e2e8f0;
  box-shadow: 0 2px 8px rgba(0,0,0,0.07);
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #334155;
  font-size: 15px;
}
[data-theme="dark"] .mobile-toggle-btn { background: #1e293b; border-color: rgba(255,255,255,0.1); color: #94a3b8; }

@media (max-width: 991px) {
  .sidebar-pro {
    position: fixed;
    left: 0;
    transform: translateX(-100%);
  }
  .sidebar-pro.active {
    transform: translateX(0);
    box-shadow: 4px 0 24px rgba(0,0,0,0.1);
  }
}

.sidebar-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.4);
  z-index: 999;
  backdrop-filter: blur(2px);
}

/* ── ANIMATIONS ── */
@keyframes pulse {
  0%   { box-shadow: 0 0 0 0 rgba(16,185,129,0.4); }
  70%  { box-shadow: 0 0 0 5px rgba(16,185,129,0); }
  100% { box-shadow: 0 0 0 0 rgba(16,185,129,0); }
}
</style>