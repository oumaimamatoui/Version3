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
            <button @click="openInviteModal" class="btn-primary-gradient shadow-premium">
              <i class="fa-solid fa-user-plus me-2"></i> Ajouter un membre
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
                    <!-- Avatar dynamique : Image ou Initiales -->
                    <div class="avatar-member-orange">
                      <img v-if="member.photoUrl" :src="getPhotoUrl(member.photoUrl)" class="avatar-img-fit" alt="P">
                      <span v-else>{{ member.prenom?.charAt(0) }}{{ member.nomFamille?.charAt(0) }}</span>
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
                  <div class="d-flex justify-content-end gap-2" v-if="authStore.user?.role === 'AdminEntreprise' || authStore.user?.role === 'SuperAdmin'">
                    <button @click="openEditModal(member)" class="btn-circle-action" title="Modifier">
                      <i class="fa-solid fa-pen-to-square"></i>
                    </button>
                    <button @click="deleteMember(member.id)" class="btn-circle-action text-danger" title="Supprimer">
                      <i class="fa-solid fa-trash"></i>
                    </button>
                  </div>
                  <span v-else class="tiny text-slate-400">Consultation seule</span>
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
 
     <!-- MODALE D'INVITATION / ÉDITION (DESIGN PREMIUM) -->
     <div v-if="showInviteModal" class="modal-overlay-tech">
       <div class="modal-box-pro animate__animated animate__zoomIn">
         <div class="modal-header-tech">
           <h4 class="m-0 fw-800">{{ isEditMode ? 'Modifier' : 'Inviter' }} un Collaborateur</h4>
           <button @click="showInviteModal = false" class="btn-close-tech"><i class="fa-solid fa-xmark"></i></button>
         </div>
         
         <form @submit.prevent="handleSave" class="modal-body-tech">
           <div class="row g-3">
             <div class="col-md-6">
               <label class="input-label-tech">Prénom</label>
               <input v-model="inviteForm.prenom" type="text" class="input-pro" placeholder="Prénom" required />
             </div>
             <div class="col-md-6">
               <label class="input-label-tech">Nom de famille</label>
               <input v-model="inviteForm.nomFamille" type="text" class="input-pro" placeholder="Nom" required />
             </div>
             <div class="col-12">
               <label class="input-label-tech">Adresse Email Professionnelle</label>
               <input v-model="inviteForm.email" type="email" class="input-pro" placeholder="exemple@entrprise.com" required :readonly="isEditMode" />
             </div>
             <div class="col-12">
               <label class="input-label-tech">Droit d'accès (Rôle)</label>
               <select v-model="inviteForm.role" class="select-pro" required>
                 <option value="" disabled selected>Sélectionner un rôle...</option>
                 <option value="Evaluateur">Évaluateur (Correcteur technique)</option>
                 <option value="Recruteur">Recruteur (Gestion RH)</option>
                 <option value="AdminEntreprise">Admin Entreprise (Plein pouvoir)</option>
               </select>
               <p class="tiny text-slate-400 mt-2"><i class="fa-solid fa-info-circle me-1"></i> Ce rôle définit les permissions du membre selon le diagramme système.</p>
             </div>
           </div>

           <div v-if="inviteError" class="alert-error-tech mt-3">
             <i class="fa-solid fa-triangle-exclamation me-2"></i> {{ inviteError }}
           </div>

           <div class="d-flex justify-content-end gap-3 mt-5">
             <button type="button" @click="showInviteModal = false" class="btn-secondary-pro">Annuler</button>
             <button type="submit" class="btn-primary-gradient px-4" :disabled="inviting">
               <span v-if="inviting"><i class="fa-solid fa-spinner fa-spin me-2"></i>Traitement...</span>
               <span v-else>{{ isEditMode ? 'Enregistrer les modifications' : "Envoyer l'invitation" }}</span>
             </button>
           </div>
         </form>
       </div>
     </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, reactive } from 'vue';
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

// LOGIQUE MODALE STAFF (INVITATION & ÉDITION)
const showInviteModal = ref(false);
const isEditMode = ref(false);
const currentEditId = ref(null);
const inviting = ref(false);
const inviteError = ref(null);

const inviteForm = reactive({
  prenom: '',
  nomFamille: '',
  email: '',
  role: '',
  entrepriseId: authStore.user?.entrepriseId
});

const openInviteModal = () => {
  isEditMode.value = false;
  currentEditId.value = null;
  inviteForm.prenom = '';
  inviteForm.nomFamille = '';
  inviteForm.email = '';
  inviteForm.role = '';
  inviteError.value = null;
  showInviteModal.value = true;
};

const openEditModal = (member) => {
  isEditMode.value = true;
  currentEditId.value = member.id;
  inviteForm.prenom = member.prenom;
  inviteForm.nomFamille = member.nomFamille;
  inviteForm.email = member.email;
  inviteForm.role = member.roleNom;
  inviteError.value = null;
  showInviteModal.value = true;
};

// ... CHARGEMENT DES DONNÉES ...
const loadData = async () => {
  loading.value = true;
  try {
    const [resStaff, resRoles] = await Promise.all([
      axios.get(`${API_BASE}/Staff`),
      axios.get(`${API_BASE}/Roles`)
    ]);
    staff.value = resStaff.data;
    rolesList.value = resRoles.data;
    console.log("[DEBUG] Membres reçus du backend:", staff.value);
  } catch (err) {
    console.error("Erreur de chargement :", err);
  } finally {
    loading.value = false;
  }
};

// SAUVEGARDE (INVITE ou UPDATE)
const handleSave = async () => {
  inviting.value = true;
  inviteError.value = null;
  try {
    inviteForm.entrepriseId = authStore.user?.entrepriseId;
    
    if (isEditMode.value) {
      // MISE À JOUR (PUT) : On utilise maintenant le Staff endpoint
      await axios.put(`${API_BASE}/Staff/${currentEditId.value}`, {
        prenom: inviteForm.prenom,
        nom: inviteForm.nomFamille, // On envoie nom car c'est ce qu'attend le DTO/Modèle
        roleNom: inviteForm.role
      });
      alert("Membre mis à jour avec succès !");
    } else {
      // INVITATION (POST)
      await axios.post(`${API_BASE}/Invitations/invite-staff`, inviteForm);
      alert("L'invitation a été envoyée avec succès !");
    }
    
    showInviteModal.value = false;
    loadData();
  } catch (err) {
    inviteError.value = err.response?.data?.message || "Erreur lors de l'enregistrement.";
  } finally {
    inviting.value = false;
  }
};

// Fonction helper pour l'URL de la photo
const getPhotoUrl = (url) => {
  if (!url) return null;
  if (url.startsWith('http')) return url;
  // On préfixe par l'URL du backend (Port 5172)
  return `${API_BASE.replace('/api', '')}/${url.replace(/\\/g, '/')}`;
};

// 2. LOGIQUE DE FILTRAGE : SUPPRIMER SUPERADMIN ET FILTRER PAR ENTREPRISE
const filteredStaff = computed(() => {
  return staff.value.filter(member => {
    // A. SUPPRIMER LE SUPER ADMIN : Jamais affiché dans cette liste
    if (member.roleNom === 'SuperAdmin') return false;

    // B. FILTRER PAR ENTREPRISE (Multi-tenancy)
    // NOTE: Désactivé temporairement pour diagnostic car 4 membres existent en DB
    if (authStore.user?.role !== 'SuperAdmin') {
       // On laisse tout passer pour l'instant pour que vous puissiez les voir
       // if (member.entrepriseId && member.entrepriseId !== authStore.user?.entrepriseId) return false;
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
.avatar-member-orange { width: 44px; height: 44px; background: #f59e0b; color: white; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 14px; box-shadow: 0 4px 10px rgba(245, 158, 11, 0.2); overflow: hidden; }
.avatar-img-fit { width: 100%; height: 100%; object-fit: cover; }
.badge-role-tag { background: #f0f9ff; color: #0369a1; padding: 6px 12px; border-radius: 100px; font-size: 11px; font-weight: 700; border: 1px solid #bae6fd; }

.btn-circle-action { 
  width: 38px; height: 38px; border-radius: 12px; 
  border: 1.5px solid #e2e8f0; background: #f1f5f9; 
  color: #64748b; transition: 0.3s; 
}
.btn-circle-action:hover { border-color: #f59e0b; color: #f59e0b; background: white; }

.btn-primary-gradient { background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%); color: white; border: none; padding: 12px 25px; border-radius: 14px; font-weight: 800; cursor: pointer; }
.btn-secondary-pro { background: #f1f5f9; border: 1.5px solid #e2e8f0; color: #475569; padding: 12px 25px; border-radius: 14px; font-weight: 800; cursor: pointer; transition: 0.3s; }
.btn-secondary-pro:hover { border-color: #f59e0b; color: #f59e0b; background: white; }

.view-toggle-cluster { 
  background: #f1f5f9; border-radius: 12px; 
  padding: 4px; border: 1.5px solid #e2e8f0; 
  display: flex; gap: 4px; 
}
.btn-view-toggle { 
  width: 36px; height: 36px; border-radius: 10px; 
  border: none; background: transparent; 
  color: #94a3b8; transition: 0.3s; 
  display: flex; align-items: center; justify-content: center; 
}
.btn-view-toggle.active { 
  background: #0f172a; color: #f59e0b; 
  box-shadow: 0 4px 10px rgba(0,0,0,0.2); 
}

/* NOUVEAU STYLE MODALE */
.modal-overlay-tech { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.6); backdrop-filter: blur(8px); z-index: 3000; display: flex; align-items: center; justify-content: center; }
.modal-box-pro { background: white; width: 100%; max-width: 550px; border-radius: 28px; box-shadow: 0 40px 100px -20px rgba(0,0,0,0.25); border: 1px solid rgba(255,255,255,0.3); overflow: hidden; }
.modal-header-tech { padding: 25px 30px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; background: #fafafa; }
.btn-close-tech { background: #f1f5f9; border: none; width: 32px; height: 32px; border-radius: 10px; color: #64748b; transition: 0.2s; }
.btn-close-tech:hover { background: #fee2e2; color: #ef4444; }
.modal-body-tech { padding: 30px; }
.input-label-tech { font-size: 11px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-bottom: 8px; display: block; }
.input-pro, .select-pro { width: 100%; padding: 12px 16px; border-radius: 12px; border: 1.5px solid #f1f5f9; background: #f8fafc; font-weight: 600; outline: none; transition: 0.2s; }
.input-pro:focus, .select-pro:focus { border-color: #f59e0b; background: white; box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.1); }
.alert-error-tech { background: #fff1f2; border: 1px solid #fecaca; color: #e11d48; padding: 12px; border-radius: 12px; font-size: 13px; font-weight: 600; }
</style>