<template>
  <div>
    <!-- Inclusion des icônes FontAwesome -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    
    <button @click="toggleSidebar" class="mobile-toggle-btn d-lg-none shadow-sm">
      <i :class="isSidebarActive ? 'fa-solid fa-xmark' : 'fa-solid fa-bars-staggered'"></i>
    </button>

    <nav :class="['sidebar-tech shadow-sm', isSidebarActive ? 'active' : '']">
      
      <!-- 1. SECTION LOGO -->
      <div class="sidebar-logo-container">
        <div class="brand-box">
          <div class="logo-icon-wrap"><i class="fa-solid fa-lightbulb"></i></div>
          <div class="logo-text">Evalua<span>Tech</span></div>
        </div>
        <div class="role-pill">
          <span class="status-dot"></span> {{ roleLabel }}
        </div>
      </div>

      <!-- 2. PROFIL UTILISATEUR -->
      <div class="user-profile-section px-3 mb-4">
        <div class="user-glass-pill shadow-sm">
          <div class="avatar-box">
            <div class="avatar-content shadow-sm">{{ authStore.user?.name?.charAt(0) || 'U' }}</div>
          </div>
          <div class="user-meta">
            <span class="user-name">{{ authStore.user?.name || 'Utilisateur' }}</span>
            <span class="user-status">En ligne</span>
          </div>
        </div>
      </div>
      
      <!-- 3. NAVIGATION (DYNAMIQUE PAR RÔLE) -->
      <div class="navbar-nav w-100 px-3 flex-grow-1 custom-scrollbar">
        
        <!-- DASHBOARD COMMUN -->
        <router-link to="/dashboard" class="nav-item nav-link-tech main-nav-special mb-3">
          <div class="icon-shell shadow-sm"><i class="fa-solid fa-house"></i></div>
          <span>Tableau de bord</span>
        </router-link>

        <!-- ========================================================== -->
        <!-- SECTION ADMIN ENTREPRISE (STRUCTURE MISE À JOUR SELON IMAGE) -->
        <!-- ========================================================== -->
        <div v-if="userRole === 'AdminEntreprise'" class="nav-group">
          
          <label class="group-header">MENU PRINCIPAL</label>
          <router-link to="/dashboard" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-table-cells-large"></i></div>
            <span>Tableau de bord</span>
          </router-link>

          <label class="group-header">CANDIDATS</label>
          <router-link to="/candidates-list" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-users"></i></div>
            <span>Candidats</span>
          </router-link>
          <router-link to="/invite" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-user-plus"></i></div>
            <span>Invitations</span>
          </router-link>
          <router-link to="/groups" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-layer-group"></i></div>
            <span>Groupes</span>
          </router-link>

          <label class="group-header">ÉVALUATIONS</label>
          <router-link to="/campaigns" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-regular fa-clipboard"></i></div>
            <span>Toutes les évaluations</span>
            <span class="badge-yellow-image ms-auto">12</span>
          </router-link>

          <label class="group-header">QUESTIONS</label>
          <router-link to="/ai-generator" class="nav-item nav-link-tech highlight-yellow-image">
            <div class="icon-shell"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
            <span>Générer des Questions</span>
          </router-link>
          <router-link to="/questions" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-book-open"></i></div>
            <span>Banque de questions</span>
          </router-link>
          <router-link to="/questions" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-regular fa-square-plus"></i></div>
            <span>Créer une question</span>
          </router-link>

          <label class="group-header">ÉQUIPE</label>
          <!-- ✅ ROUTER MIS À JOUR ICI -->
          <router-link to="/staff-members" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-user-gear"></i></div>
            <span>Membres du personnel</span>
          </router-link>
          <router-link to="/roles" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-shield-halved"></i></div>
            <span>Rôles & Permissions</span>
          </router-link>
          <router-link to="/settings" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-building"></i></div>
            <span>Paramètres de l'organisation</span>
          </router-link>
        </div>

        <!-- ============================================= -->
        <!-- SECTION ÉVALUATEUR / FORMATEUR (ORIGINAL)    -->
        <!-- ============================================= -->
        <div v-if="userRole === 'Formateur'" class="nav-group">
          <label class="group-header">OUTILS ÉVALUATION</label>
          <router-link to="/questions" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-database"></i></div>
            <span>Banque de questions</span>
            <span class="badge-custom-blue ms-auto">248</span>
          </router-link>
          <router-link to="/campaigns" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-regular fa-clipboard"></i></div>
            <span>Toutes les évaluations</span>
            <span class="badge-custom-blue ms-auto">12</span>
          </router-link>
          <router-link to="/test-builder" class="nav-item nav-link-tech">
            <div class="icon-shell"><i class="fa-solid fa-file-circle-plus"></i></div>
            <span>Test Builder</span>
          </router-link>

          <label class="group-header mt-4">INTELLIGENCE ARTIFICIELLE</label>
          <router-link to="/ai-generator" class="nav-item nav-link-tech">
            <div class="icon-shell ai-purple"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
            <span>AI Generator</span>
          </router-link>
          <router-link to="/analyse-comportementale" class="nav-item nav-link-tech">
            <div class="icon-shell ai-blue"><i class="fa-solid fa-microchip"></i></div>
            <span>Smart Analysis</span>
          </router-link>

          <label class="group-header mt-4">SESSIONS & PERFORMANCE</label>
          <router-link to="/sessions" class="nav-item nav-link-tech">
            <div class="icon-shell warning"><i class="fa-solid fa-calendar-check"></i></div>
            <span>Gérer les Sessions</span>
            <span class="badge-custom-blue ms-auto">5</span>
          </router-link>
          <router-link to="/stats" class="nav-item nav-link-tech">
            <div class="icon-shell success"><i class="fa-solid fa-chart-line"></i></div>
            <span>Statistiques</span>
          </router-link>

          <label class="group-header mt-4">CONFIGURATION</label>
          <router-link to="/settings" class="nav-item nav-link-tech">
            <div class="icon-shell info"><i class="fa-solid fa-gears"></i></div>
            <span>Paramètres</span>
          </router-link>
        </div>

        <!-- ============================================= -->
        <!-- SECTION SUPER ADMIN (ORIGINAL)                -->
        <!-- ============================================= -->
        <div v-if="userRole === 'SuperAdmin'" class="nav-group">
          <label class="group-header">MASTER ADMIN</label>
          <router-link to="/super-admin" class="nav-item nav-link-tech">
            <div class="icon-shell warning shadow-sm"><i class="fa-solid fa-building-circle-check"></i></div>
            <span>Voir Organisations</span>
          </router-link>
          <!-- GESTION UTILISATEURS -->
          <router-link to="/platform-users" class="nav-item nav-link-tech">
            <div class="icon-shell primary shadow-sm"><i class="fa-solid fa-users-gear"></i></div>
            <span>Gérer Utilisateurs</span>
          </router-link>
    
          <router-link to="/platform-analytics" class="nav-item nav-link-tech">
            <div class="icon-shell success shadow-sm"><i class="fa-solid fa-chart-line"></i></div>
            <span>Analytique</span>
          </router-link>
        </div>

        <!-- ============================================= -->
        <!-- SECTION CANDIDAT (ORIGINAL)                   -->
        <!-- ============================================= -->
        <div v-if="userRole === 'Candidat'" class="nav-group">
          <label class="group-header">MON PARCOURS</label>
          <router-link to="/exam-lobby" class="nav-item nav-link-tech">
            <div class="icon-shell success"><i class="fa-solid fa-pen-to-square"></i></div>
            <span>Passer Test</span>
          </router-link>
          <router-link to="/results" class="nav-item nav-link-tech">
            <div class="icon-shell primary"><i class="fa-solid fa-square-poll-vertical"></i></div>
            <span>Voir Résultats</span>
          </router-link>
          <router-link to="/history" class="nav-item nav-link-tech">
            <div class="icon-shell secondary"><i class="fa-solid fa-clock-rotate-left"></i></div>
            <span>Historique</span>
          </router-link>

          <label class="group-header mt-4">COMPTE</label>
          <router-link to="/settings" class="nav-item nav-link-tech">
            <div class="icon-shell info"><i class="fa-solid fa-gears"></i></div>
            <span>Paramètres</span>
          </router-link>
        </div>

      </div>

      <!-- 4. FOOTER -->
      <div class="sidebar-footer shadow-sm">
        <!-- MODE TEST (PFE) -->
        <div class="demo-switcher mb-3">
          <div class="demo-label">RÔLES PFE (MODE TEST)</div>
          <div class="custom-select-wrapper">
            <select @change="changeRole($event)" class="role-select-custom">
              <option value="Candidat" :selected="userRole === 'Candidat'">Candidat</option>
              <option value="Formateur" :selected="userRole === 'Formateur'">Évaluateur</option>
              <option value="AdminEntreprise" :selected="userRole === 'AdminEntreprise'">Admin Entreprise</option>
              <option value="SuperAdmin" :selected="userRole === 'SuperAdmin'">Super Admin</option>
            </select>
          </div>
        </div>
        <div @click="logout" class="logout-link-action px-3 mt-2"><i class="fa-solid fa-power-off me-2"></i>DÉCONNEXION</div>
      </div>
    </nav>
    <div v-if="isSidebarActive" class="sidebar-overlay d-lg-none" @click="toggleSidebar"></div>
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

const branding = ref({
  companyName: 'NeoEvaluation',
  color: '#ffa000',
  logoUrl: null
});

const fetchBranding = async () => {
  try {
    const res = await api.get('/Settings/branding');
    branding.value = res.data;
  } catch (e) { /* Fallback */ }
};

const roleLabel = computed(() => {
  const roles = { 'SuperAdmin': 'SuperAdmin', 'AdminEntreprise': 'Administrateur', 'Formateur': 'Évaluateur', 'Candidat': 'Candidat' };
  return roles[userRole.value] || 'Utilisateur';
});

const toggleSidebar = () => { isSidebarActive.value = !isSidebarActive.value; };
const changeRole = (e) => { 
  authStore.setUser(authStore.user, e.target.value, authStore.token); 
  router.push('/dashboard').then(() => window.location.reload()); 
};
const logout = () => { authStore.logout(); router.push('/login'); };

onMounted(fetchBranding);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800;900&display=swap');

.sidebar-tech {
  min-width: 280px; max-width: 280px;
  background: #ffffff; height: 100vh;
  position: sticky; top: 0;
  border-right: 1px solid #f1f5f9;
  display: flex; flex-direction: column;
  transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  font-family: 'Plus Jakarta Sans', sans-serif;
  z-index: 1000;
}

/* Headers */
.group-header { 
  font-size: 10.5px; 
  font-weight: 800; 
  color: #a3aed0; 
  letter-spacing: 1px; 
  margin: 25px 0 10px 15px; 
  display: block; 
  text-transform: uppercase;
}

/* Nav Links */
.nav-link-tech {
  color: #6e7a9e; font-weight: 600; font-size: 14px; 
  padding: 11px 16px; border-radius: 14px; margin-bottom: 3px;
  display: flex; align-items: center; transition: 0.2s; text-decoration: none;
  cursor: pointer;
}
.nav-link-tech:hover { background: #f8fafc; color: #1e293b; transform: translateX(3px); }
.nav-link-tech.router-link-active:not(.highlight-yellow-image) { 
    background: #f8fafc; 
    color: #ffa000 !important; 
}

/* --- DESIGN SPÉCIFIQUE IMAGE ADMIN --- */

/* Mise en évidence jaune de l'image */
.highlight-yellow-image {
  background-color: #fff8e1 !important;
  color: #b45309 !important;
  font-weight: 700;
}
.highlight-yellow-image .icon-shell { color: #b45309 !important; }

/* Badge jaune de l'image (le "12") */
.badge-yellow-image {
  background: #fef3c7;
  color: #d97706;
  font-size: 11px;
  font-weight: 800;
  padding: 2px 8px;
  border-radius: 10px;
}

/* Style dashboard principal noir */
.main-nav-special { background: #0f172a; color: white !important; }

/* Styles standards existants */
.badge-custom-blue { background: #f0f7ff; color: #3b82f6; font-size: 10px; font-weight: 800; padding: 2px 10px; border-radius: 12px; }
.icon-shell { width: 30px; font-size: 16px; margin-right: 12px; display: flex; justify-content: center; }
.ai-purple { color: #8b5cf6; } .ai-blue { color: #0ea5e9; }
.warning { color: #ffa000; } .success { color: #10b981; } .primary { color: #6366f1; } .secondary { color: #6e7a9e; } .info { color: #06b6d4; }

.sidebar-logo-container { padding: 35px 25px 20px; }
.brand-box { display: flex; align-items: center; gap: 12px; }
.logo-icon-wrap { color: #ffa000; font-size: 26px; }
.logo-text { font-size: 23px; font-weight: 900; letter-spacing: -0.5px; }
.logo-text span { color: #ffa000; }
.role-pill { display: flex; align-items: center; gap: 6px; font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-top: 5px; }
.status-dot { height: 6px; width: 6px; background: #22c55e; border-radius: 50%; display: inline-block; }

.user-glass-pill { background: white; border: 1px solid #f1f5f9; padding: 14px; border-radius: 20px; margin: 0 6px; display: flex; align-items: center; gap: 14px; }
.avatar-content { width: 42px; height: 42px; background: #f8fafc; color: #0f172a; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-weight: 800; border: 1px solid #f1f5f9; }

.sidebar-footer { padding: 20px; border-top: 1px solid #f1f5f9; margin-top: auto; background: #fafbfc; }
.demo-switcher { padding: 10px; background: white; border-radius: 12px; border: 1px solid #f1f5f9; }
.role-select-custom { width: 100%; border: none; background: transparent; font-weight: 800; font-size: 13px; outline: none; cursor: pointer; }
.logout-link-action { color: #ef4444; font-size: 12px; font-weight: 800; cursor: pointer; text-transform: uppercase; }

.custom-scrollbar { overflow-y: auto; scrollbar-width: none; }
.custom-scrollbar::-webkit-scrollbar { width: 0; }
.mobile-toggle-btn { position: fixed; top: 15px; left: 15px; z-index: 2000; width: 44px; height: 44px; border-radius: 12px; background: white; border: 1px solid #f1f5f9; color: #0f172a; display: flex; align-items: center; justify-content: center; }
.sidebar-overlay { position: fixed; inset: 0; background: rgba(0,0,0,0.1); backdrop-filter: blur(2px); z-index: 999; }

</style>