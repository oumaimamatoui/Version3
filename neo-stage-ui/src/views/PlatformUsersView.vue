<template>
  <div class="d-flex admin-body">
    <!-- FIX ICONES : Importation forcée de FontAwesome 6 -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <AppSidebar />
    
    <div class="content flex-grow-1 position-relative">
      <!-- COUCHES DÉCORATIVES PROFESSIONNELLES -->
      <div class="background-overlay"></div>
      <div class="tech-grid-subtle"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-indigo"></div>

      <AppNavbar />

      <main class="p-4 main-viewport animate-fade-in">
        
        <!-- 1. HEADER SECTION -->
        <div class="d-flex flex-wrap justify-content-between align-items-center mb-5 gap-3">
          <div class="header-main">
            <nav class="breadcrumb-pro mb-1">ADMINISTRATION / PLATEFORME / <span class="active">COMPTES</span></nav>
            <h2 class="display-title-v2">Utilisateurs <span>Système</span></h2>
            <p class="text-muted-pro small m-0">Gérez l'ensemble des accès et les privilèges des organisations.</p>
          </div>
          <div class="d-flex gap-3">
            <button @click="fetchData" class="btn-outline-tech-sm"><i class="fa-solid fa-rotate me-2"></i>Actualiser</button>
            <button @click="showInviteModal = true" class="btn-primary-premium">
              <i class="fa-solid fa-plus-circle me-2"></i> Ajouter un Utilisateur
            </button>
          </div>
        </div>

        <!-- 2. KPI BENTO GRID (Nouveau) -->
        <div class="row g-3 mb-5">
          <div class="col-md-4" v-for="stat in platformStats" :key="stat.label">
            <div class="kpi-card-v2 shadow-sm">
              <div class="d-flex justify-content-between align-items-start">
                <div class="icon-vessel-v2" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="trend-pro up">{{ stat.trend }}</div>
              </div>
              <div class="mt-4">
                <div class="h2 fw-800 text-navy m-0">{{ stat.val }}</div>
                <div class="tiny-label uppercase ls-1">{{ stat.label }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. BARRE DE RECHERCHE & FILTRES -->
        <div class="toolbar-pro p-2 mb-4 shadow-sm">
          <div class="search-pro-wrapper flex-grow-1">
            <i class="fa-solid fa-magnifying-glass ms-3"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher par nom, e-mail, organisation..." class="input-pro-search">
          </div>
          <div class="filter-pills-group d-none d-md-flex px-3 border-start">
             <select v-model="selectedRole" class="form-select border-0 bg-transparent small fw-bold text-muted">
               <option>Tous les rôles</option>
               <option>SuperAdmin</option>
               <option>AdminEntreprise</option>
               <option>Evaluateur</option>
               <option>Candidat</option>
             </select>
          </div>
        </div>

        <!-- 4. TABLE DES UTILISATEURS -->
        <div class="glass-card border-0 shadow-pro overflow-hidden">
          <table class="table align-middle m-0 table-hover">
            <thead>
              <tr class="bg-light tiny uppercase fw-bold text-muted border-bottom">
                <th class="ps-4 py-3">Utilisateur</th>
                <th>Organisation</th>
                <th>Rôle</th>
                <th>Dernière connexion</th>
                <th>État</th>
                <th class="text-end pe-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="u in filteredUsers" :key="u.id" class="user-row-pro">
                <td class="ps-4">
                  <div class="d-flex align-items-center gap-3">
                    <div class="avatar-sm-premium bg-navy text-white fw-bold">
                      {{ u.name?.[0] || '?' }}
                      <span v-if="u.lastLogin === 'En ligne'" class="online-indicator-dot"></span>
                    </div>
                    <div>
                      <div class="fw-bold text-navy">{{ u.name }}</div>
                      <div class="tiny text-muted">{{ u.email }}</div>
                    </div>
                  </div>
                </td>
                <td>
                  <span class="org-badge shadow-sm"><i class="fa-solid fa-building me-1 opacity-50"></i> {{ u.org }}</span>
                </td>
                <td>
                  <span :class="['role-chip', getRoleClass(u.role)]">{{ u.role }}</span>
                </td>
                <td>
                  <div class="d-flex align-items-center gap-2">
                    <i class="fa-regular fa-clock text-muted tiny"></i>
                    <span class="small text-secondary fw-500">{{ u.lastLogin }}</span>
                  </div>
                </td>
                <td>
                  <div class="form-check form-switch custom-switch">
                    <input class="form-check-input" type="checkbox" :checked="u.isActive" @change="handleToggleStatus(u)" style="cursor: pointer;">
                  </div>
                </td>
                <td class="text-end pe-4">
                  <div class="d-flex justify-content-end gap-2">
                    <button class="btn-icon-mini edit"><i class="fa-solid fa-pen-to-square"></i></button>
                    <button @click="handleDelete(u)" class="btn-icon-mini delete"><i class="fa-solid fa-trash-can"></i></button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
          <div class="p-3 bg-light-soft border-top d-flex justify-content-between align-items-center">
            <span class="tiny text-muted fw-bold uppercase">Affichage de {{ users.length }} utilisateurs actifs</span>
            <div class="pagination-dots">
              <span class="dot active"></span><span class="dot"></span><span class="dot"></span>
            </div>
          </div>
        </div>

        <!-- MODAL INVITATION ADMIN -->
        <div v-if="showInviteModal" class="custom-modal-overlay">
          <div class="glass-card p-4 modal-box shadow-lg animate-fade-in">
            <h4 class="fw-800 text-navy mb-4">Inviter un <span>Administrateur</span></h4>
            <div class="mb-3">
              <label class="tiny-label uppercase mb-2">Nom Complet</label>
              <input v-model="inviteData.name" type="text" class="form-control-pro" placeholder="Ex: Jean Dupont">
            </div>
            <div class="mb-4">
              <label class="tiny-label uppercase mb-2">Email Professionnel</label>
              <input v-model="inviteData.email" type="email" class="form-control-pro" placeholder="admin@entreprise.com">
            </div>
            <div class="d-flex gap-2 justify-content-end">
              <button @click="showInviteModal = false" class="btn btn-outline-secondary btn-pro">Annuler</button>
              <button @click="handleInvite" class="btn-primary-premium py-2 px-4">Envoyer l'invitation</button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import { superAdminApi } from '@/services/api';
import { useToast } from 'vue-toastification';

const toast = useToast();
const loading = ref(false);
const searchQuery = ref('');
const selectedRole = ref('Tous les rôles');
const showInviteModal = ref(false);

const inviteData = reactive({ name: '', email: '' });

const platformStats = ref([
  { label: 'Utilisateurs Totaux', val: '0', icon: 'fa-solid fa-users', bg: 'rgba(59, 130, 246, 0.1)', color: '#3b82f6', trend: 'Live', key: 'totalUtilisateurs' },
  { label: 'Organisations Actives', val: '0', icon: 'fa-solid fa-building-circle-check', bg: 'rgba(16, 185, 129, 0.1)', color: '#10b981', trend: 'Live', key: 'totalEntreprises' },
  { label: 'Sessions en cours', val: '0', icon: 'fa-solid fa-bolt-lightning', bg: 'rgba(234, 179, 8, 0.1)', color: '#f59e0b', trend: 'Live', key: 'sessionsIARecentes' },
]);

const users = ref([]);

const fetchData = async () => {
    loading.value = true;
    try {
        const [resStats, resUsers] = await Promise.all([
            superAdminApi.getStats(),
            superAdminApi.getPlatformUsers()
        ]);
        
        // Update Stats
        platformStats.value.forEach(s => {
            s.val = resStats.data[s.key] || 0;
        });

        users.value = resUsers.data;
    } catch (e) {
        toast.error("Erreur de chargement des données.");
    } finally {
        loading.value = false;
    }
};

const filteredUsers = computed(() => {
    return users.value.filter(u => {
        const matchesSearch = 
            u.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
            u.email.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
            u.org.toLowerCase().includes(searchQuery.value.toLowerCase());
        
        const matchesRole = selectedRole.value === 'Tous les rôles' || u.role === selectedRole.value;
        
        return matchesSearch && matchesRole;
    });
});

const handleDelete = async (user) => {
    if(confirm(`Voulez-vous vraiment supprimer l'utilisateur ${user.name} ? Cette action est irréversible.`)) {
        try {
            await superAdminApi.deleteUser(user.id);
            toast.success("Utilisateur supprimé avec succès.");
            fetchData(); // Plus sûr de recharger tout pour les stats
        } catch (e) { toast.error("Erreur lors de la suppression."); }
    }
};

const handleToggleStatus = async (user) => {
    try {
        await superAdminApi.toggleUserStatus(user.id);
        user.isActive = !user.isActive; // Update local state for immediate feedback
        toast.success(`Statut de ${user.name} mis à jour.`);
    } catch (e) {
        toast.error("Erreur lors du changement de statut.");
        // Optionnel: Recharger si erreur pour recouvrer l'état réel
        fetchData();
    }
};

const handleInvite = async () => {
    if(!inviteData.email || !inviteData.name) return toast.warning("Champs requis.");
    try {
        await superAdminApi.inviteAdmin(inviteData);
        toast.success(`Invitation envoyée à ${inviteData.email}`);
        showInviteModal.value = false;
        inviteData.name = ''; inviteData.email = '';
    } catch (e) { toast.error("Erreur d'invitation."); }
};

const getRoleClass = (role) => {
  const map = {
    'SuperAdmin': 'role-super',
    'AdminEntreprise': 'role-admin',
    'Evaluateur': 'role-eval',
    'Candidat': 'role-cand'
  };
  return map[role] || '';
};

onMounted(fetchData);
</script>
<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@400;700&display=swap');

/* --- MASTER ROOT & LUXURY BACKGROUND --- */
.admin-body {
  min-height: 100vh;
  background: #fdfdfd;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  position: relative;
  overflow-x: hidden;
}

/* طبقات الخلفية الفاخرة */
.background-overlay { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.tech-grid-subtle { position: fixed; inset: 0; background-image: radial-gradient(#e2e8f0 0.5px, transparent 0.5px); background-size: 30px 30px; opacity: 0.3; z-index: 1; }
.glow-orb { position: fixed; border-radius: 50%; filter: blur(140px); opacity: 0.15; z-index: 1; }
.orb-amber { width: 700px; height: 700px; background: #fbbf24; top: -10%; left: -10%; }
.orb-indigo { width: 600px; height: 600px; background: #6366f1; bottom: -10%; right: -5%; }

.main-viewport { position: relative; z-index: 10; }

/* --- HEADER SECTION --- */
.breadcrumb-pro { font-family: 'JetBrains Mono'; font-size: 10px; color: #94a3b8; letter-spacing: 3px; }
.breadcrumb-pro .active { color: #fbbf24; font-weight: 700; }
.display-title-v2 { font-weight: 900; font-size: 36px; letter-spacing: -2px; }
.display-title-v2 span { color: #fbbf24; }

/* --- KPI BENTO CARDS --- */
.kpi-card-v2 {
  background: white; border-radius: 30px; padding: 30px;
  border: 1px solid white; box-shadow: 0 20px 50px -15px rgba(0,0,0,0.05);
  position: relative; overflow: hidden; transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.kpi-card-v2:hover { transform: translateY(-10px); box-shadow: 0 30px 60px -10px rgba(0,0,0,0.08); }
.icon-vessel-v2 {
  width: 55px; height: 55px; border-radius: 18px; 
  display: flex; align-items: center; justify-content: center; font-size: 1.3rem;
  box-shadow: inset 0 0 10px rgba(255,255,255,0.5);
}
.trend-pro { font-size: 10px; font-weight: 900; padding: 5px 12px; border-radius: 100px; letter-spacing: 1px; }
.fw-800 { font-weight: 900; font-size: 32px; letter-spacing: -1px; }
.tiny-label { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; }

/* --- TOOLBAR (Command Center Style) --- */
.toolbar-pro {
  background: rgba(255, 255, 255, 0.7); backdrop-filter: blur(20px);
  border-radius: 25px; border: 1px solid white;
  padding: 10px 25px !important; box-shadow: 0 15px 35px rgba(0,0,0,0.03);
}
.input-pro-search {
  border: none; background: transparent; padding: 12px; font-size: 14px;
  font-weight: 600; width: 100%; outline: none; color: #0f172a;
}
.search-pro-wrapper i { color: #fbbf24; font-size: 18px; }

/* --- ELITE TABLE --- */
.glass-card {
  background: rgba(255, 255, 255, 0.8); backdrop-filter: blur(25px);
  border-radius: 40px; border: 1px solid white; overflow: hidden;
  box-shadow: 0 30px 70px -20px rgba(0,0,0,0.05);
}

.table thead tr { background: rgba(248, 250, 252, 0.5); border-bottom: 2px solid #f1f5f9; }
.table th { padding: 20px 15px; color: #94a3b8; font-family: 'JetBrains Mono'; letter-spacing: 1px; }
.user-row-pro { transition: 0.3s; border-bottom: 1px solid #f8fafc; }
.user-row-pro:hover { background: rgba(251, 191, 36, 0.02); }

.avatar-sm-premium {
  width: 44px; height: 44px; border-radius: 15px; background: #0f172a;
  display: flex; align-items: center; justify-content: center;
  box-shadow: 0 8px 15px rgba(15, 23, 42, 0.1);
}
.online-indicator-dot {
  width: 12px; height: 12px; border: 2.5px solid white;
  background: #10b981; animation: pulse 2s infinite;
}
@keyframes pulse { 0% { transform: scale(1); opacity: 1; } 100% { transform: scale(1.5); opacity: 0; } }

.org-badge {
  background: white; border: 1.5px solid #f1f5f9; padding: 7px 14px;
  border-radius: 12px; font-size: 11px; font-weight: 800; color: #64748b;
  box-shadow: 0 4px 10px rgba(0,0,0,0.02);
}

/* --- ROLE CHIPS --- */
.role-chip { padding: 6px 14px; border-radius: 10px; font-size: 10px; font-weight: 900; letter-spacing: 0.5px; }
.role-super { background: #f5f3ff; color: #7c3aed; border: 1px solid #ddd6fe; }
.role-admin { background: #fffbeb; color: #b45309; border: 1px solid #fef3c7; }
.role-eval { background: #eff6ff; color: #1d4ed8; border: 1px solid #dbeafe; }
.role-cand { background: #f0fdf4; color: #15803d; border: 1px solid #dcfce7; }

/* --- SWITCH & ACTIONS --- */
.custom-switch .form-check-input {
  width: 45px; height: 22px; background-color: #e2e8f0; border: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='-4 -4 8 8'%3e%3ccircle r='3' fill='white'/%3e%3c/svg%3e");
}
.custom-switch .form-check-input:checked { background-color: #fbbf24; }

.btn-icon-mini {
  width: 36px; height: 36px; border-radius: 12px; border: 1.5px solid #f1f5f9;
  background: white; color: #94a3b8; transition: 0.3s;
}
.btn-icon-mini:hover.edit { color: #fbbf24; border-color: #fbbf24; transform: rotate(15deg); }
.btn-icon-mini:hover.delete { color: #ef4444; border-color: #ef4444; transform: scale(1.1); }

/* --- BUTTONS --- */
.btn-primary-premium {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; border: none; padding: 14px 28px; border-radius: 18px;
  font-weight: 800; font-size: 13px; box-shadow: 0 10px 25px rgba(251, 191, 36, 0.3);
  transition: 0.3s cubic-bezier(0.23, 1, 0.32, 1);
}
.btn-primary-premium:hover { transform: translateY(-4px) scale(1.02); box-shadow: 0 15px 35px rgba(251, 191, 36, 0.45); }

.btn-outline-tech-sm {
  background: white; border: 1.5px solid #f1f5f9; color: #64748b;
  padding: 12px 24px; border-radius: 16px; font-size: 12px; font-weight: 800;
}

/* --- MODAL ELITE --- */
.custom-modal-overlay {
  background: rgba(15, 23, 42, 0.6); backdrop-filter: blur(15px);
}
.modal-box {
  border-radius: 40px; border: 1px solid white; padding: 40px !important;
  box-shadow: 0 50px 100px rgba(0,0,0,0.1);
}
.form-control-pro {
  background: #f8fafc; border: 1.5px solid #f1f5f9; border-radius: 15px;
  padding: 15px; font-weight: 600; transition: 0.3s;
}
.form-control-pro:focus { border-color: #fbbf24; background: white; box-shadow: 0 10px 20px rgba(251, 191, 36, 0.05); }

/* --- UTILS --- */
.pagination-dots .dot { width: 8px; height: 8px; background: #e2e8f0; border-radius: 50%; margin: 0 4px; transition: 0.3s; }
.pagination-dots .dot.active { background: #fbbf24; width: 25px; border-radius: 10px; }
</style>