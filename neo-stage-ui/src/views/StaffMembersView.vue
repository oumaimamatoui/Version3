<template>
  <div class="d-flex admin-body">
    <!-- BARRE LATÉRALE -->
    <AppSidebar />
    
    <div class="content flex-grow-1 p-4">
      <!-- BARRE DE NAVIGATION (Profil utilisateur) -->
      <AppNavbar />

      <div class="main-viewport mt-4 animate__animated animate__fadeIn">
        
        <!-- EN-TÊTE -->
        <div class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <nav aria-label="breadcrumb" class="mb-2">
              <span class="tiny fw-800 text-amber uppercase ls-1">Ressources Humaines</span>
            </nav>
            <h2 class="fw-800 m-0 text-slate-900">Membres du Personnel</h2>
            <p class="text-slate-500 small mt-1">
              Gestion des collaborateurs pour l'entreprise : 
              <span class="fw-bold text-slate-800">{{ authStore.user?.entrepriseNom || 'Ma Société' }}</span>
            </p>
          </div>
          <div class="d-flex gap-3">
            <button @click="loadData" class="btn-secondary-pro">
              <i class="fa-solid fa-arrows-rotate me-2"></i> Actualiser
            </button>
            <button @click="$router.push('/roles')" class="btn-primary-gradient shadow-premium">
              <i class="fa-solid fa-user-shield me-2"></i> Gérer les rôles
            </button>
          </div>
        </div>

        <!-- STATISTIQUES FILTRÉES -->
        <div class="row g-4 mb-5">
          <div class="col-md-4">
            <div class="stat-card-premium border-warning border-start border-4">
              <div class="d-flex align-items-center">
                <div class="icon-stat-box bg-soft-blue text-primary">
                  <i class="fa-solid fa-users"></i>
                </div>
                <div class="ms-3">
                  <div class="tiny text-slate-400 uppercase fw-800 ls-1">Total Personnel</div>
                  <div class="fw-800 fs-3 text-slate-800">{{ filteredStaff.length }}</div>
                </div>
              </div>
            </div>
          </div>
          <div class="col-md-4">
            <div class="stat-card-premium">
              <div class="d-flex align-items-center">
                <div class="icon-stat-box bg-soft-amber text-amber">
                  <i class="fa-solid fa-id-badge"></i>
                </div>
                <div class="ms-3">
                  <div class="tiny text-slate-400 uppercase fw-800 ls-1">Rôles Actifs</div>
                  <div class="fw-800 fs-3 text-slate-800">{{ filteredRolesList.length }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- FILTRES -->
        <div class="glass-panel p-3 mb-4 d-flex align-items-center gap-3">
          <div class="search-vessel flex-grow-1 pe-4 border-end">
            <i class="fa-solid fa-magnifying-glass text-slate-400"></i>
            <input type="text" v-model="searchQuery" class="input-search-pro ms-2" placeholder="Rechercher par nom ou email...">
          </div>
          <div class="filter-vessel ps-2">
            <select v-model="selectedRoleFilter" class="select-minimal">
              <option value="">Tous les rôles</option>
              <!-- On n'affiche pas SuperAdmin dans le menu déroulant -->
              <option v-for="role in filteredRolesList" :key="role.id" :value="role.nom">
                {{ role.nom }}
              </option>
            </select>
          </div>
        </div>

        <!-- ÉTAT DE CHARGEMENT -->
        <div v-if="loading" class="text-center py-5">
           <div class="spinner-border text-warning" role="status"></div>
           <p class="mt-3 text-slate-400 small fw-bold">Filtrage des données entreprise...</p>
        </div>

        <!-- TABLEAU DES COLLABORATEURS -->
        <div v-else class="table-container-premium shadow-sm">
          <table class="table-pro">
            <thead>
              <tr>
                <th>Collaborateur</th>
                <th>Rôle d'entreprise</th>
                <th>Contact Email</th>
                <th>Date d'ajout</th>
                <th class="text-end">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="member in filteredStaff" :key="member.id" class="table-row-hover">
                <td>
                  <div class="d-flex align-items-center">
                    <!-- Avatar orange comme sur votre capture -->
                    <div class="avatar-member-orange">
                      {{ member.prenom?.charAt(0) }}{{ member.nomFamille?.charAt(0) }}
                    </div>
                    <div class="ms-3">
                      <div class="fw-800 text-slate-800">{{ member.prenom }} {{ member.nomFamille }}</div>
                      <div class="tiny text-success fw-bold uppercase ls-1">● Membre Actif</div>
                    </div>
                  </div>
                </td>
                <td>
                  <span class="badge-role-tag">
                    <i class="fa-solid fa-shield-halved me-1"></i> {{ member.roleNom }}
                  </span>
                </td>
                <td>
                  <div class="small text-slate-600 fw-600">{{ member.email }}</div>
                </td>
                <td>
                  <div class="tiny text-slate-400 fw-800 uppercase">{{ formatDate(member.creeLe) }}</div>
                </td>
                <td class="text-end">
                  <div class="d-flex justify-content-end gap-2">
                    <button class="btn-circle-action"><i class="fa-solid fa-pen-to-square"></i></button>
                    <button @click="deleteMember(member.id)" class="btn-circle-action text-danger"><i class="fa-solid fa-trash"></i></button>
                  </div>
                </td>
              </tr>
              <!-- SI LA LISTE EST VIDE -->
              <tr v-if="filteredStaff.length === 0">
                <td colspan="5" class="text-center py-5">
                  <i class="fa-solid fa-user-slash fa-2x text-slate-200 mb-3"></i>
                  <p class="text-slate-400 mb-0">Aucun membre trouvé pour votre entreprise.</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';
import { useAuthStore } from '@/stores/auth'; // Votre store Pinia pour l'utilisateur

const authStore = useAuthStore();
const API_BASE = 'http://localhost:5172/api';

const staff = ref([]);
const rolesList = ref([]);
const loading = ref(true);
const searchQuery = ref('');
const selectedRoleFilter = ref('');

// 1. CHARGEMENT DES DONNÉES
const loadData = async () => {
  loading.value = true;
  try {
    const [resStaff, resRoles] = await Promise.all([
      axios.get(`${API_BASE}/Staff`),
      axios.get(`${API_BASE}/Roles`)
    ]);
    staff.value = resStaff.data;
    rolesList.value = resRoles.data;
  } catch (err) {
    console.error("Erreur de chargement :", err);
  } finally {
    loading.value = false;
  }
};

// 2. LOGIQUE DE FILTRAGE : SUPPRIMER SUPERADMIN ET FILTRER PAR ENTREPRISE
const filteredStaff = computed(() => {
  return staff.value.filter(member => {
    // A. SUPPRIMER LE SUPER ADMIN : Jamais affiché dans cette liste
    if (member.roleNom === 'SuperAdmin') return false;

    // B. FILTRER PAR ENTREPRISE (Multi-tenancy)
    // On ne montre que les membres qui ont le même entrepriseId que l'admin connecté
    // (Sauf si l'admin connecté est lui-même SuperAdmin, il voit tout)
    if (authStore.user?.role !== 'SuperAdmin') {
       if (member.entrepriseId !== authStore.user?.entrepriseId) return false;
    }

    // C. RECHERCHE TEXTUELLE
    const search = searchQuery.value.toLowerCase();
    const matchesSearch = 
      `${member.prenom} ${member.nomFamille}`.toLowerCase().includes(search) ||
      member.email.toLowerCase().includes(search);

    // D. FILTRE PAR RÔLE
    const matchesRole = selectedRoleFilter.value === '' || member.roleNom === selectedRoleFilter.value;

    return matchesSearch && matchesRole;
  });
});

// FILTRER LA LISTE DES RÔLES (Pour le menu déroulant)
const filteredRolesList = computed(() => {
  return rolesList.value.filter(role => {
    // On cache SuperAdmin et on ne montre que les rôles de l'entreprise
    const notSuper = role.nom !== 'SuperAdmin';
    const sameCompany = authStore.user?.role === 'SuperAdmin' || role.entrepriseId === authStore.user?.entrepriseId;
    return notSuper && sameCompany;
  });
});

// 3. SUPPRESSION
const deleteMember = async (id) => {
  if (confirm("Supprimer ce collaborateur ?")) {
    try {
      await axios.delete(`${API_BASE}/Staff/${id}`);
      loadData();
    } catch (err) {
      alert("Erreur lors de la suppression.");
    }
  }
};

const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', year: 'numeric' }) : '-';

onMounted(loadData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.admin-body { background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; min-height: 100vh; color: #1e293b; }
.ls-1 { letter-spacing: 1px; }
.tiny { font-size: 10px; }
.uppercase { text-transform: uppercase; }
.fw-800 { font-weight: 800; }

/* CARTES STATS */
.stat-card-premium { background: white; padding: 24px; border-radius: 20px; border: 1px solid #f1f5f9; box-shadow: 0 4px 15px rgba(0,0,0,0.02); }
.icon-stat-box { width: 52px; height: 52px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 22px; }
.bg-soft-blue { background: #eff6ff; color: #3b82f6; }
.bg-soft-amber { background: #fffbeb; color: #f59e0b; }

/* FILTRES */
.glass-panel { background: white; border-radius: 18px; border: 1px solid #e2e8f0; }
.input-search-pro { border: none; outline: none; width: 100%; font-weight: 600; color: #475569; background: transparent; }
.select-minimal { border: none; outline: none; background: transparent; font-weight: 800; color: #64748b; font-size: 12px; text-transform: uppercase; cursor: pointer; }

/* TABLEAU */
.table-container-premium { background: white; border-radius: 24px; overflow: hidden; border: 1px solid #f1f5f9; }
.table-pro { width: 100%; border-collapse: collapse; }
.table-pro th { background: #f8fafc; padding: 18px 24px; text-align: left; font-size: 11px; font-weight: 800; color: #64748b; text-transform: uppercase; border-bottom: 1px solid #f1f5f9; }
.table-pro td { padding: 20px 24px; border-bottom: 1px solid #f8fafc; vertical-align: middle; }

/* AVATAR ORANGE (Comme sur la capture) */
.avatar-member-orange { width: 44px; height: 44px; background: #f59e0b; color: white; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 14px; box-shadow: 0 4px 10px rgba(245, 158, 11, 0.2); }
.badge-role-tag { background: #f0f9ff; color: #0369a1; padding: 6px 12px; border-radius: 100px; font-size: 11px; font-weight: 700; border: 1px solid #bae6fd; }

.btn-circle-action { width: 34px; height: 34px; border-radius: 10px; border: 1px solid #e2e8f0; background: white; color: #64748b; transition: 0.2s; }
.btn-circle-action:hover { background: #f8fafc; color: #0f172a; }

.btn-primary-gradient { background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: white; border: none; padding: 12px 25px; border-radius: 14px; font-weight: 800; }
.btn-secondary-pro { background: white; border: 1.5px solid #e2e8f0; color: #475569; padding: 12px 25px; border-radius: 14px; font-weight: 800; }
</style>