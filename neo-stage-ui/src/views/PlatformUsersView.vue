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
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

/* --- LAYOUT CORE --- */
.admin-body { background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; min-height: 100vh; color: #0f172a; }
.main-viewport { position: relative; z-index: 10; }

/* DÉCORS */
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#e2e8f0 1px, transparent 1px); background-size: 40px 40px; opacity: 0.5; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.1; z-index: 1; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 400px; height: 400px; background: #4f46e5; bottom: -5%; left: -5%; }

/* --- TYPOGRAPHY --- */
.display-title-v2 { font-weight: 800; font-size: 32px; letter-spacing: -1.5px; margin: 0; }
.display-title-v2 span { color: #eab308; }
.breadcrumb-pro { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; }
.breadcrumb-pro .active { color: #0f172a; }

/* --- KPI CARDS --- */
.kpi-card-v2 { background: #fff; border-radius: 20px; padding: 24px; border: 1px solid #f1f5f9; transition: 0.3s; }
.icon-vessel-v2 { width: 44px; height: 44px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 1.1rem; }
.trend-pro { font-size: 10px; font-weight: 800; color: #10b981; background: #ecfdf5; padding: 4px 8px; border-radius: 100px; }

/* --- TOOLBAR --- */
.toolbar-pro { background: #fff; border-radius: 100px; border: 1px solid #f1f5f9; display: flex; align-items: center; }
.input-pro-search { border: none; background: transparent; padding: 12px; font-size: 14px; outline: none; width: 100%; color: #0f172a; }
.search-pro-wrapper { display: flex; align-items: center; color: #94a3b8; }

/* --- TABLE STYLES --- */
.user-row-pro { transition: 0.2s; }
.user-row-pro:hover { background: #fcfcfd; }
.avatar-sm-premium { 
  width: 40px; height: 40px; border-radius: 12px; position: relative;
  display: flex; align-items: center; justify-content: center; font-size: 14px;
}
.online-indicator-dot {
  position: absolute; bottom: -2px; right: -2px; width: 10px; height: 10px; 
  background: #10b981; border: 2px solid white; border-radius: 50%;
}

.org-badge {
  background: white; border: 1px solid #f1f5f9; padding: 6px 12px; 
  border-radius: 8px; font-size: 12px; font-weight: 700; color: #64748b;
}

/* --- ROLE CHIPS --- */
.role-chip { padding: 4px 12px; border-radius: 6px; font-size: 11px; font-weight: 800; text-transform: uppercase; }
.role-super { background: #f5f3ff; color: #7c3aed; border: 1px solid #ddd6fe; }
.role-admin { background: #fffbeb; color: #b45309; border: 1px solid #fef3c7; }
.role-eval { background: #eff6ff; color: #1d4ed8; border: 1px solid #dbeafe; }
.role-cand { background: #f0fdf4; color: #15803d; border: 1px solid #dcfce7; }

/* --- ACTIONS --- */
.btn-icon-mini { width: 32px; height: 32px; border-radius: 8px; border: none; background: #f8fafc; color: #94a3b8; transition: 0.2s; }
.btn-icon-mini:hover.edit { color: #3b82f6; background: #eff6ff; }
.btn-icon-mini:hover.delete { color: #ef4444; background: #fff1f2; }

/* --- BUTTONS --- */
.btn-primary-premium {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  color: #fff; border: none; padding: 12px 24px; border-radius: 14px;
  font-weight: 800; font-size: 13px; transition: 0.3s;
}
.btn-primary-premium:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(15, 23, 42, 0.2); }

.btn-outline-tech-sm {
  background: white; border: 1px solid #e2e8f0; color: #64748b;
  padding: 10px 20px; border-radius: 12px; font-size: 12px; font-weight: 700; transition: 0.2s;
}

/* --- UTILS --- */
.bg-light-soft { background: #fcfcfd; }
.tiny-label { font-size: 9px; font-weight: 800; color: #94a3b8; }
.fw-500 { font-weight: 500; }
.uppercase { text-transform: uppercase; }
.pagination-dots .dot { width: 6px; height: 6px; background: #e2e8f0; display: inline-block; border-radius: 50%; margin: 0 3px; }
.pagination-dots .dot.active { background: #eab308; width: 15px; border-radius: 10px; }

.animate-fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

/* --- MODAL CUSTOM --- */
.custom-modal-overlay {
  position: fixed; inset: 0; background: rgba(15, 23, 42, 0.7);
  backdrop-filter: blur(8px); z-index: 9999;
  display: flex; align-items: center; justify-content: center;
}
.modal-box { width: 100%; max-width: 450px; border-radius: 24px; background: white; }
.form-control-pro {
  width: 100%; padding: 12px 16px; border-radius: 12px;
  border: 1px solid #e2e8f0; font-size: 14px; outline: none; transition: 0.2s;
}
.form-control-pro:focus { border-color: #3b82f6; box-shadow: 0 0 0 4px rgba(59, 130, 246, 0.1); }
.btn-pro { border-radius: 12px; font-weight: 700; font-size: 13px; }
</style>