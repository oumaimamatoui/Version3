<template>
  <div>
    <!-- Inclusion des icônes FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    
    <button @click="toggleSidebar" class="mobile-toggle-btn d-lg-none">
      <i :class="isSidebarActive ? 'fa-solid fa-xmark' : 'fa-solid fa-bars-staggered'"></i>
    </button>

    <nav :class="['sidebar-tech', isSidebarActive ? 'active' : '']">
      
      <!-- 1. SECTION LOGO (DESIGN PREMIUM REVISITÉ) -->
      <div class="sidebar-header">
        <div class="brand-wrapper">
          <div class="logo-box-premium">
            <!-- Nouveau Logo Géométrique Abstrait -->
            <svg viewBox="0 0 100 100" fill="none" xmlns="http://www.w3.org/2000/svg" class="brand-svg">
              <defs>
                <linearGradient id="brand-grad" x1="0%" y1="0%" x2="100%" y2="100%">
                  <stop offset="0%" stop-color="#f59e0b" />
                  <stop offset="100%" stop-color="#d97706" />
                </linearGradient>
              </defs>
              <path d="M50 10 L85 30 L85 70 L50 90 L15 70 L15 30 Z" stroke="url(#brand-grad)" stroke-width="6" fill="none"/>
              <path d="M50 25 L72 38 L72 62 L50 75 L28 62 L28 38 Z" fill="url(#brand-grad)"/>
              <circle cx="50" cy="50" r="8" fill="white" />
            </svg>
          </div>
          <div class="brand-info">
            <div class="brand-text">Evalua<span>Tech</span></div>
            <div class="brand-subtitle">Smart Evaluation</div>
          </div>
        </div>
        <div class="role-badge-premium">
          <span class="pulse-dot"></span> {{ roleLabel }}
        </div>
      </div>

      <!-- 2. PROFIL UTILISATEUR (DESIGN DARK GLASS) -->
      <div class="profile-section px-3">
        <div class="user-glass-card">
          <div class="avatar-wrapper">
            <div class="avatar-gradient">{{ authStore.user?.name?.charAt(0) || 'U' }}</div>
            <div class="status-indicator"></div>
          </div>
          <div class="user-meta">
            <span class="user-name-text">{{ authStore.user?.name || 'Utilisateur' }}</span>
            <span class="user-status-text">Membre Actif</span>
          </div>
          <div class="user-action-icon"><i class="fa-solid fa-chevron-right"></i></div>
        </div>
      </div>
      
      <!-- 3. NAVIGATION (EFFETS DE SURVOL MODERNE) -->
      <div class="navbar-nav w-100 px-3 flex-grow-1 custom-scrollbar">
        
        <!-- DASHBOARD COMMUN -->
        <router-link to="/dashboard" class="nav-link-tech dashboard-main mb-4">
          <div class="icon-shell"><i class="fa-solid fa-house-user"></i></div>
          <span>Tableau de bord</span>
          <i class="fa-solid fa-chevron-right arrow-indicator"></i>
        </router-link>

        <!-- SECTION ADMIN ENTREPRISE / RECRUTEUR -->
        <div v-if="userRole === 'AdminEntreprise' || userRole === 'Recruteur'" class="nav-group">
          <label class="group-label">Gestion Système</label>
          <router-link to="/dashboard" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-gauge-high"></i></div>
            <span>Vue d'ensemble</span>
          </router-link>

          <label class="group-label">Ressources Humaines</label>
          <router-link to="/candidates-list" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-users-viewfinder"></i></div>
            <span>Candidats</span>
          </router-link>
          <router-link to="/invite" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-paper-plane"></i></div>
            <span>Invitations</span>
          </router-link>
          <router-link to="/groups" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-object-group"></i></div>
            <span>Groupes</span>
          </router-link>

          <label class="group-label">Évaluations</label>
          <router-link to="/campaigns" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-rectangle-list"></i></div>
            <span>Campagnes</span>
            <span class="nav-badge-count">{{ campaignCount }}</span>
          </router-link>

          <label class="group-label">Intelligence Artificielle</label>
          <router-link to="/ai-generator" class="nav-link-tech ai-highlight">
            <div class="icon-shell"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
            <span>Générer des Questions</span>
          </router-link>
          <router-link to="/questions" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-database"></i></div>
            <span>Banque de données</span>
          </router-link>

          <label class="group-label">Organisation</label>
          <router-link to="/staff-members" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-user-gear"></i></div>
            <span>Membres du personnel</span>
          </router-link>
          <router-link to="/settings" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-sliders"></i></div>
            <span>Paramètres Org</span>
          </router-link>
        </div>

        <!-- SECTION ÉVALUATEUR -->
        <div v-if="userRole === 'Formateur'" class="nav-group">
          <label class="group-label">Expertise</label>
          <router-link to="/questions" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-layer-group"></i></div>
            <span>Banque de questions</span>
          </router-link>
          <router-link to="/campaigns" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-clipboard-check"></i></div>
            <span>Évaluations en cours</span>
          </router-link>

          <label class="group-label">IA & Analytics</label>
          <router-link to="/ai-generator" class="nav-link-tech ai-special-gradient">
            <div class="icon-shell"><i class="fa-solid fa-brain"></i></div>
            <span>IA Assistant</span>
          </router-link>
          <router-link to="/analyse-comportementale" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-microchip"></i></div>
            <span>Smart Analysis</span>
          </router-link>
          <router-link to="/stats" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-chart-pie"></i></div>
            <span>Statistiques</span>
          </router-link>
        </div>

        <!-- SECTION SUPER ADMIN -->
        <div v-if="userRole === 'SuperAdmin'" class="nav-group">
          <label class="group-label">Master Console</label>
          <router-link to="/super-admin" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-server"></i></div>
            <span>Voir Organisations</span>
          </router-link>
          <router-link to="/platform-users" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-users-gear"></i></div>
            <span>Gérer Utilisateurs</span>
          </router-link>
          <router-link to="/platform-analytics" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-globe"></i></div>
            <span>Analytique globale</span>
          </router-link>
        </div>

        <!-- SECTION CANDIDAT -->
        <div v-if="userRole === 'Candidat'" class="nav-group">
          <label class="group-label">Mon Parcours</label>
          <router-link to="/exam-lobby" class="nav-link-tech success-mode">
            <div class="icon-shell"><i class="fa-solid fa-play"></i></div>
            <span>Passer Test</span>
          </router-link>
          <router-link to="/results" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-award"></i></div>
            <span>Mes Résultats</span>
          </router-link>
          <router-link to="/history" class="nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-clock-rotate-left"></i></div>
            <span>Historique</span>
          </router-link>
        </div>

      </div>

      <!-- 4. FOOTER (SIMULATEUR & DÉCONNEXION) -->
      <div class="sidebar-footer">
        <div class="simulator-card shadow-sm mb-3">
          <label><i class="fa-solid fa-flask-vial"></i> Environnement Test</label>
          <div class="select-ui-wrapper">
            <select @change="changeRole($event)">
              <option value="Candidat" :selected="userRole === 'Candidat'">Candidat</option>
              <option value="Evaluateur" :selected="userRole === 'Evaluateur'">Évaluateur</option>
              <option value="Recruteur" :selected="userRole === 'Recruteur'">Recruteur</option>
              <option value="AdminEntreprise" :selected="userRole === 'AdminEntreprise'">Admin Entreprise</option>
            </select>
            <i class="fa-solid fa-chevron-up"></i>
          </div>
        </div>
        <button @click="logout" class="premium-logout-btn">
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

const router = useRouter();
const authStore = useAuthStore();
const userRole = computed(() => authStore.role);
const isSidebarActive = ref(false);
const campaignCount = ref(0);

const branding = ref({ companyName: 'EvaluaTech', color: '#f59e0b' });

const roleLabel = computed(() => {
  const roles = { 'SuperAdmin': 'SuperAdmin', 'AdminEntreprise': 'Administrateur', 'Recruteur': 'RH / Recruteur', 'Evaluateur': 'Évaluateur', 'Candidat': 'Candidat' };
  return roles[userRole.value] || 'Utilisateur';
});

const fetchCounts = async () => {
  try {
    const res = await api.get('/Campagnes');
    campaignCount.value = res.data.length;
  } catch (err) {
    console.error("Erreur sidebar counts:", err);
  }
};

const toggleSidebar = () => { isSidebarActive.value = !isSidebarActive.value; };
const changeRole = (e) => { 
  authStore.setUser(authStore.user, e.target.value, authStore.token); 
  router.push('/dashboard').then(() => window.location.reload()); 
};
const logout = () => { authStore.logout(); router.push('/login'); };

onMounted(() => {
  fetchCounts();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* --- STRUCTURE PRINCIPALE --- */
.sidebar-tech {
  min-width: 290px; width: 290px;
  background: var(--bg-sidebar, #ffffff); height: 100vh;
  position: sticky; top: 0;
  border-right: 1px solid var(--border-sidebar, #f1f5f9);
  display: flex; flex-direction: column;
  transition: all 0.4s cubic-bezier(0.16, 1, 0.3, 1);
  font-family: 'Plus Jakarta Sans', sans-serif;
  z-index: 1000;
}

[data-theme="dark"] .sidebar-tech {
  --bg-sidebar: #0f172a;
  --border-sidebar: rgba(255, 255, 255, 0.05);
}

/* --- HEADER & LOGO --- */
.sidebar-header { padding: 32px 24px 20px; }
.brand-wrapper { display: flex; align-items: center; gap: 14px; margin-bottom: 12px; }

.logo-box-premium {
  width: 44px; height: 44px;
  flex-shrink: 0;
}
.brand-svg { width: 100%; height: 100%; filter: drop-shadow(0 4px 6px rgba(245, 158, 11, 0.2)); }

.brand-text { font-size: 22px; font-weight: 800; color: var(--navy-text, #0f172a); letter-spacing: -0.5px; line-height: 1; }
.brand-text span { color: #f59e0b; }
.brand-subtitle { font-size: 10px; color: #94a3b8; font-weight: 700; text-transform: uppercase; margin-top: 4px; letter-spacing: 1px; }

[data-theme="dark"] .brand-text { --navy-text: #ffffff; }

.role-badge-premium { 
  background: #f8fafc; color: #64748b; font-size: 10px; font-weight: 700; 
  padding: 4px 12px; border-radius: 50px; text-transform: uppercase;
  display: inline-flex; align-items: center; gap: 8px; border: 1px solid #f1f5f9;
}
.pulse-dot { width: 6px; height: 6px; background: #10b981; border-radius: 50%; animation: pulse-ring 2s infinite; }

/* --- CARTE PROFIL PREMIUM --- */
.user-glass-card {
  background: #0f172a; padding: 14px; border-radius: 18px; 
  display: flex; align-items: center; gap: 12px; position: relative;
  box-shadow: 0 12px 24px -6px rgba(15, 23, 42, 0.2);
}
.avatar-gradient {
  width: 42px; height: 42px; background: linear-gradient(135deg, #f59e0b, #fbbf24);
  border-radius: 12px; display: flex; align-items: center; justify-content: center;
  color: white; font-weight: 800; font-size: 16px; border: 1px solid rgba(255,255,255,0.1);
}
.avatar-wrapper { position: relative; }
.status-indicator { position: absolute; bottom: -2px; right: -2px; width: 10px; height: 10px; background: #10b981; border: 2px solid #0f172a; border-radius: 50%; }
.user-name-text { display: block; color: white; font-weight: 700; font-size: 14px; }
.user-status-text { font-size: 11px; color: #94a3b8; font-weight: 500; }
.user-action-icon { margin-left: auto; color: #334155; font-size: 14px; }

/* --- NAVIGATION --- */
.group-label {
  font-size: 11px; font-weight: 800; color: #cbd5e1;
  text-transform: uppercase; letter-spacing: 1.2px;
  padding: 24px 12px 10px; display: block;
}
.nav-link-tech {
  display: flex; align-items: center; padding: 11px 16px;
  border-radius: 12px; color: var(--nav-text, #475569); font-weight: 600; font-size: 14px;
  text-decoration: none; transition: all 0.25s ease; margin-bottom: 4px;
}
.nav-link-tech:hover { background: var(--nav-hover-bg, #f8fafc); color: var(--nav-hover-text, #0f172a); transform: translateX(5px); }
.nav-link-tech.router-link-active { background: #fffbeb; color: #b45309; font-weight: 700; box-shadow: 0 4px 12px -4px rgba(245,158,11,0.15); }

[data-theme="dark"] .nav-link-tech { --nav-text: #94a3b8; }
[data-theme="dark"] .nav-link-tech:hover { --nav-hover-bg: rgba(255, 255, 255, 0.05); --nav-hover-text: #ffffff; }
[data-theme="dark"] .nav-link-tech.router-link-active { background: rgba(245, 158, 11, 0.1); color: #f59e0b; }

.icon-shell { width: 30px; font-size: 17px; display: flex; align-items: center; color: inherit; }

/* --- SPECIALS --- */
.dashboard-main { background: #f1f5f9; color: #0f172a; }
.ai-highlight { background: #fdf2ff; color: #7e22ce; border: 1px solid #f3e8ff; }
.ai-special-gradient { background: linear-gradient(to right, #faf5ff, #ffffff); border: 1px solid #f3e8ff; color: #7e22ce; }
.success-mode { background: #f0fdf4; color: #15803d; border: 1px solid #dcfce7; }
.nav-badge-count { margin-left: auto; background: #fbbf24; color: #92400e; font-size: 10px; font-weight: 800; padding: 1px 7px; border-radius: 20px; }

/* --- FOOTER --- */
.sidebar-footer { padding: 20px 20px 24px; border-top: 1px solid var(--border-sidebar, #f1f5f9); margin-top: auto; }
.simulator-card { background: var(--footer-card, #f8fafc); padding: 12px; border-radius: 14px; border: 1px solid var(--border-sidebar, #edf2f7); }
[data-theme="dark"] .simulator-card { --footer-card: rgba(255,255,255,0.03); }
.simulator-card label { font-size: 9px; font-weight: 800; color: #94a3b8; text-transform: uppercase; display: block; margin-bottom: 6px; }

.select-ui-wrapper { position: relative; display: flex; align-items: center; }
.select-ui-wrapper select {
  width: 100%; border: none; background: transparent; font-size: 13px; font-weight: 700;
  color: #1e293b; appearance: none; cursor: pointer; outline: none;
}
.select-ui-wrapper i { position: absolute; right: 0; font-size: 10px; color: #f59e0b; pointer-events: none; }

.premium-logout-btn {
  width: 100%; background: #fff1f2; border: 1px solid #fee2e2; padding: 12px;
  border-radius: 12px; color: #e11d48; font-weight: 700; font-size: 13px;
  display: flex; align-items: center; justify-content: center; gap: 10px;
  cursor: pointer; transition: 0.3s; margin-top: 12px;
}
.premium-logout-btn:hover { background: #ffe4e6; transform: translateY(-2px); box-shadow: 0 4px 12px rgba(225, 29, 72, 0.1); }

/* --- UTILS --- */
.custom-scrollbar {
  overflow-y: auto;
  overflow-x: hidden;
}

@keyframes pulse-ring {
  0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.4); }
  70% { box-shadow: 0 0 0 6px rgba(16, 185, 129, 0); }
  100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}

.mobile-toggle-btn {
  position: fixed; top: 15px; left: 15px; z-index: 2000;
  width: 40px; height: 40px; border-radius: 10px;
  background: white; border: 1px solid #f1f5f9; box-shadow: 0 4px 12px rgba(0,0,0,0.08);
}
</style>