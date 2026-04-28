<template>
  <div class="d-flex admin-body">
    
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <AppSidebar />

    <div class="content flex-grow-1">
      <AppNavbar />
      
      <div class="main-container p-4 pt-2">
    
        <header class="dashboard-header d-flex justify-content-between align-items-end mb-4">
          <div>
            <nav aria-label="breadcrumb">
              <ol class="breadcrumb mb-1">
                <li class="breadcrumb-item"><a href="#">Admin</a></li>
                <li class="breadcrumb-item active">Dashboard</li>
              </ol>
            </nav>
            <h2 class="page-title m-0">Master Control Panel</h2>
            <p class="text-secondary small m-0">Supervision en temps réel de l'écosystème NeoEvaluation</p>
          </div>
          
          <div class="header-actions d-flex gap-3 align-items-center">
             <div class="status-pill d-none d-lg-flex">
               <span class="pulse-dot"></span>
               <span class="status-text">Serveurs IA : <span class="text-success fw-bold">Optimum</span></span>
             </div>
             
             <button @click="showOrgModal = true" class="btn-premium secondary">
                <i class="fa-solid fa-plus-circle"></i>
                <span>Enregistrer une Entreprise</span>
             </button>
          </div>
        </header>

     
        <div class="row g-4 mb-4">
          <div class="col-xl-3 col-md-6" v-for="s in masterStats" :key="s.label">
            <div class="card-premium stat-item">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div class="stat-icon-wrapper" :style="{ backgroundColor: s.bg, color: s.color }">
                  <i :class="s.icon"></i>
                </div>
                <span class="trend-badge" :class="s.trendUp ? 'up' : 'down'">
                  <i :class="s.trendUp ? 'fa-solid fa-arrow-up' : 'fa-solid fa-arrow-down'"></i>
                  {{ s.trend }}
                </span>
              </div>
              <div class="stat-content">
                <h3 class="stat-number">{{ s.val }}</h3>
                <p class="stat-label text-uppercase">{{ s.label }}</p>
              </div>
            </div>
          </div>
        </div>

        <div class="card-premium filter-bar mb-4">
          <div class="search-box">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher une entreprise, un email, un ID...">
          </div>
          <div class="filter-controls">
            <select class="form-select-premium"><option>Statut : Tous</option></select>
            <select class="form-select-premium"><option>Période : 30 jours</option></select>
            <button class="btn-refresh"><i class="fa-solid fa-rotate"></i></button>
          </div>
        </div>

        <div class="row g-4 mb-5">
     
          <div class="col-lg-8">
            <div class="card-premium h-100 p-0 overflow-hidden">
              <div class="card-header-premium p-4 d-flex justify-content-between align-items-center">
                <h6 class="m-0 fw-bold"><i class="fa-solid fa-id-card-clip text-indigo me-2"></i>Demandes d'adhésion</h6>
                <span class="badge-count-pro">{{ filteredPendingRequests.length }} dossiers</span>
              </div>
              
              <div class="table-responsive">
                <table class="table table-custom m-0">
                  <thead>
                    <tr>
                      <th class="ps-4">ENTREPRISE</th>
                      <th>CONTACT</th>
                      <th>DATE</th>
                      <th class="text-end pe-4">ACTIONS</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="req in filteredPendingRequests" :key="req.id" class="align-middle">
                      <td class="ps-4">
                        <div class="d-flex align-items-center">
                          <div class="avatar-letter">{{ req.nomEntreprise?.[0] || '?' }}</div>
                          <div class="ms-3">
                            <div class="fw-bold text-dark">{{ req.nomEntreprise }}</div>
                            <div class="tiny text-muted">Matricule: {{ req.matriculeFiscale || 'Non renseigné' }}</div>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div class="small fw-semibold">{{ req.nomResponsable }}</div>
                        <div class="tiny text-indigo">{{ req.emailResponsable }}</div>
                      </td>
                      <td class="tiny text-secondary">{{ formatDate(req.creeLe) }}</td>
                      <td class="text-end pe-4">
                        <div class="d-flex justify-content-end gap-2">
                          <button @click="handleApprove(req.id)" class="btn-circle approve" title="Approuver"><i class="fa-solid fa-check"></i></button>
                          <button @click="handleReject(req.id)" class="btn-circle reject" title="Refuser"><i class="fa-solid fa-xmark"></i></button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="filteredPendingRequests.length === 0">
                      <td colspan="4" class="text-center py-5">
                        <img src="https://cdn-icons-png.flaticon.com/512/7486/7486744.png" width="60" class="opacity-25 mb-3">
                        <p class="text-muted small">Aucun dossier trouvé</p>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

      
          <div class="col-lg-4">
            <div class="card-premium h-100 p-4">
              <h6 class="fw-bold mb-4"><i class="fa-solid fa-microchip text-indigo me-2"></i>Performance IA</h6>
              <div class="chart-container mb-4">
                <canvas id="masterChart"></canvas>
                <div class="chart-center-text">
                  <span class="number">82%</span>
                  <span class="label">Charge</span>
                </div>
              </div>
              
              <ul class="list-unstyled mb-0">
                <li class="d-flex justify-content-between align-items-center mb-3">
                  <div class="d-flex align-items-center gap-2">
                    <span class="dot-sm bg-indigo"></span>
                    <span class="small text-secondary">Tokens LLM</span>
                  </div>
                  <span class="small fw-bold">1.2M / 1.5M</span>
                </li>
                <li class="d-flex justify-content-between align-items-center">
                  <div class="d-flex align-items-center gap-2">
                    <span class="dot-sm bg-success"></span>
                    <span class="small text-secondary">Temps de réponse</span>
                  </div>
                  <span class="small fw-bold">240ms</span>
                </li>
              </ul>
            </div>
          </div>
        </div>

      
      </div>
    </div>

  
    <Transition name="slide-up">
      <div v-if="showOrgModal" class="modal-backdrop-custom" @click.self="showOrgModal = false">
        <div class="modal-content-pro modal-wide">
          <div class="modal-header-pro border-bottom-0 pb-0">
            <h5 class="fw-bold m-0 text-dark">Créer une organisation</h5>
            <button @click="showOrgModal = false" class="btn-close-modal"><i class="fa-solid fa-xmark"></i></button>
          </div>
          
          <form @submit.prevent="handleCreateOrg" class="modal-body-pro pt-2">
            <!-- SECTION 1 : ORGANISATION -->
            <div class="section-title-mod mb-3 mt-2">
               <i class="fa-solid fa-briefcase text-warning me-2"></i>
               <span class="fw-bold">Informations sur l'organisation</span>
            </div>

            <div class="mb-3">
              <label class="form-label-photo">Nom de l'organisation <span class="text-danger">*</span></label>
              <input type="text" v-model="newOrg.name" class="form-control-photo" placeholder="Entrez le nom de l'organisation" required>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-photo">Domaine</label>
                <input type="text" v-model="newOrg.domain" class="form-control-photo" placeholder="entreprise.com">
              </div>
              <div class="col-md-6">
                <label class="form-label-photo">Industrie</label>
                <select v-model="newOrg.industry" class="form-select-photo">
                    <option value="" disabled selected>Sélectionnez une industrie</option>
                    <option>Technologie</option>
                    <option>Finance</option>
                    <option>Santé</option>
                    <option>Éducation</option>
                </select>
              </div>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-photo">Site web</label>
                <input type="url" v-model="newOrg.website" class="form-control-photo" placeholder="https://example.com">
              </div>
              <div class="col-md-6">
                <label class="form-label-photo">Ville</label>
                <input type="text" v-model="newOrg.city" class="form-control-photo" placeholder="Ville">
              </div>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-photo">Pays</label>
                <input type="text" v-model="newOrg.country" class="form-control-photo" placeholder="Pays">
              </div>
              <div class="col-md-6">
                <label class="form-label-photo">Code postal</label>
                <input type="text" v-model="newOrg.zipCode" class="form-control-photo" placeholder="12345">
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label-photo">Adresse</label>
              <input type="text" v-model="newOrg.address" class="form-control-photo" placeholder="Adresse de la rue">
            </div>

            <div class="mb-3">
              <label class="form-label-photo">Description</label>
              <textarea v-model="newOrg.description" class="form-control-photo" rows="2" placeholder="Brève description de l'organisation"></textarea>
            </div>

            <!-- SECTION 2 : ADMINISTRATEUR -->
            <div class="section-title-mod mb-3 mt-4">
               <i class="fa-solid fa-circle-user text-info me-2"></i>
               <span class="fw-bold">Informations sur l'administrateur</span>
            </div>

            <div class="alert alert-info-photo mb-3 d-flex align-items-center">
              <i class="fa-solid fa-circle-info me-2"></i>
              <span>Remarque: Un mot de passe sécurisé sera automatiquement généré et envoyé à l'administrateur par e-mail.</span>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-photo">Prénom <span class="text-danger">*</span></label>
                <input type="text" v-model="newOrg.adminFirstName" class="form-control-photo" placeholder="Prénom de l'administrateur" required>
              </div>
              <div class="col-md-6">
                <label class="form-label-photo">Nom <span class="text-danger">*</span></label>
                <input type="text" v-model="newOrg.adminLastName" class="form-control-photo" placeholder="Nom de l'administrateur" required>
              </div>
            </div>

            <div class="row g-3 mb-4">
                <div class="col-md-6">
                    <label class="form-label-photo">Adresse e-mail <span class="text-danger">*</span></label>
                    <input type="email" v-model="newOrg.adminEmail" class="form-control-photo" placeholder="Adresse e-mail de l'admin" required>
                </div>
                <div class="col-md-6">
                    <label class="form-label-photo">Numéro de téléphone</label>
                    <div class="d-flex gap-2">
                        <select class="form-select-photo w-auto"><option>🇹🇳 +216</option></select>
                        <input type="tel" v-model="newOrg.adminPhone" class="form-control-photo" placeholder="(555) 123-4567">
                    </div>
                </div>
            </div>
            
            <div class="modal-footer-photo border-top pt-3 d-flex justify-content-end gap-2">
                <button type="button" @click="showOrgModal = false" class="btn-annuler-photo">Annuler</button>
                <button type="submit" class="btn-creer-photo">Créer une organisation</button>
            </div>
          </form>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';
import Chart from 'chart.js/auto';
import { superAdminApi } from '@/services/api';
import { useToast } from 'vue-toastification';

const toast = useToast();
const showOrgModal = ref(false);
const searchQuery = ref('');

// --- DATA ---
const statsData = reactive({
    totalEntreprises: 0,
    totalUtilisateurs: 0,
    demandesEnAttente: 0,
    sessionsIARecentes: 0
});

const masterStats = ref([
  { label: 'Entreprises', val: '0', icon: 'fa-solid fa-building', bg: '#eef2ff', color: '#4f46e5', trend: '12%', trendUp: true, key: 'totalEntreprises' },
  { label: 'Sessions IA', val: '0', icon: 'fa-solid fa-wand-magic-sparkles', bg: '#fff7ed', color: '#f97316', trend: '24%', trendUp: true, key: 'sessionsIARecentes' },
  { label: 'Utilisateurs', val: '0', icon: 'fa-solid fa-users', bg: '#ecfdf5', color: '#10b981', trend: '5%', trendUp: true, key: 'totalUtilisateurs' },
  { label: 'En attente', val: '0', icon: 'fa-solid fa-shield-virus', bg: '#fef2f2', color: '#ef4444', trend: '0%', trendUp: false, key: 'demandesEnAttente' }
]);

// Updated reactive object to match photo fields
const newOrg = reactive({
    name: '', 
    domain: '',
    industry: '',
    website: '',
    city: '',
    country: '',
    zipCode: '',
    address: '',
    description: '',
    adminFirstName: '', 
    adminLastName: '', 
    adminEmail: '',
    adminPhone: ''
});

const pendingRequests = ref([]);

const filteredPendingRequests = computed(() => {
  const query = searchQuery.value.toLowerCase().trim();
  if (!query) return pendingRequests.value;
  return pendingRequests.value.filter(req => 
    (req.nomEntreprise?.toLowerCase().includes(query)) ||
    (req.emailResponsable?.toLowerCase().includes(query)) ||
    (req.nomResponsable?.toLowerCase().includes(query)) ||
    (req.matriculeFiscale?.toLowerCase().includes(query))
  );
});

const fetchData = async () => {
  try {
    const [resStats, resPending] = await Promise.all([
        superAdminApi.getStats(),
        superAdminApi.getPendingRequests()
    ]);
    
    // Stats Update
    Object.assign(statsData, resStats.data);
    masterStats.value.forEach(s => {
        s.val = statsData[s.key] || 0;
    });

    pendingRequests.value = resPending.data;
  } catch (error) {
    console.error("Erreur Backend:", error);
  }
};

const handleApprove = async (id) => {
  toast.info("Validation du compte...");
  try {
    await superAdminApi.approveRequest(id);
    toast.success("Entreprise activée avec succès !");
    fetchData();
  } catch (e) { toast.error("Erreur lors de l'activation."); }
};

const handleReject = async (id) => {
  if(confirm("Voulez-vous vraiment refuser cette demande ?")) {
    try {
        await superAdminApi.rejectRequest(id);
        toast.success("Demande refusée.");
        fetchData();
    } catch(e) { toast.error("Erreur."); }
  }
};

const handleCreateOrg = async () => {
    try {
        await superAdminApi.createOrg(newOrg);
        toast.success(`L'organisation ${newOrg.name} a été créée.`);
        showOrgModal.value = false;
        // Reset form
        Object.keys(newOrg).forEach(key => newOrg[key] = '');
        fetchData();
    } catch(e) { toast.error("Erreur lors de la création."); }
};

const formatDate = (date) => date ? new Date(date).toLocaleDateString('fr-FR', { day: 'numeric', month: 'short' }) : 'N/A';

onMounted(async () => {
  await fetchData();
  const ctx = document.getElementById('masterChart');
  if (ctx) {
    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['Libre', 'Occupé'],
        datasets: [{
          data: [18, 82],
          backgroundColor: ['#f1f5f9', '#4f46e5'],
          borderWidth: 0,
          borderRadius: 5
        }]
      },
      options: { cutout: '80%', plugins: { legend: { display: false } } }
    });
  }
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Plus+Jakarta+Sans:wght@600;700;800&display=swap');

.admin-body { background: #f1f5f9; font-family: 'Inter', sans-serif; color: #1e293b; min-height: 100vh; }
.card-premium { background: white; border: 1px solid #e2e8f0; border-radius: 16px; box-shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05), 0 8px 10px -6px rgba(0, 0, 0, 0.01); transition: all 0.3s ease; }
.page-title { font-family: 'Plus Jakarta Sans', sans-serif; font-weight: 800; letter-spacing: -0.03em; color: #0f172a; }

.modal-backdrop-custom { 
  position: fixed; inset: 0; background: rgba(0, 0, 0, 0.4); 
  backdrop-filter: blur(4px); z-index: 9999; 
  display: flex; align-items: flex-start; justify-content: center; 
  padding: 40px 10px; overflow-y: auto;
}

.modal-content-pro.modal-wide { 
  background: white; width: 95%; max-width: 850px; 
  border-radius: 12px; padding: 30px; 
  box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
}

.section-title-mod { 
    font-size: 15px; color: #1e293b; border-bottom: 1px solid #f1f5f9; 
    padding-bottom: 10px; display: flex; align-items: center;
}

.form-label-photo { font-size: 13px; font-weight: 600; color: #334155; margin-bottom: 6px; display: block; }

.form-control-photo, .form-select-photo { 
    width: 100%; padding: 11px 15px; border-radius: 8px; 
    border: 1px solid #e2e8f0; font-size: 14px; color: #1e293b; 
    background: #f8fafc; outline: none; transition: all 0.2s;
}

.form-control-photo:focus, .form-select-photo:focus { border-color: #f59e0b; background: white; box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1); }

.alert-info-photo { 
    background-color: #f0f9ff; color: #0369a1; border: 1px solid #bae6fd; 
    padding: 12px 16px; border-radius: 8px; font-size: 13px; font-weight: 500;
}

.btn-annuler-photo { background: #fff; color: #475569; border: 1px solid #d1d5db; padding: 10px 24px; border-radius: 8px; font-weight: 600; font-size: 14px; }
.btn-creer-photo { background: #fbbf24; color: #000; border: none; padding: 10px 28px; border-radius: 8px; font-weight: 700; font-size: 14px; transition: 0.2s; }
.btn-creer-photo:hover { background: #f59e0b; }

.btn-close-modal { border: none; background: transparent; color: #94a3b8; font-size: 20px; cursor: pointer; transition: 0.2s; }
.btn-close-modal:hover { color: #ef4444; }

/* Existing styles below */
.status-pill { background: white; border: 1px solid #e2e8f0; padding: 8px 16px; border-radius: 100px; font-size: 13px; display: flex; align-items: center; gap: 10px; }
.pulse-dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; animation: pulse 2s infinite; }
@keyframes pulse {
  0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); }
  70% { transform: scale(1); box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}
.btn-premium { padding: 10px 20px; border-radius: 12px; font-weight: 600; font-size: 14px; border: none; display: flex; align-items: center; gap: 8px; transition: 0.2s; }
.btn-premium.secondary { background: #0f172a; color: white; }
.stat-item { padding: 24px; }
.stat-icon-wrapper { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 20px; }
.stat-number { font-weight: 800; font-size: 28px; margin-bottom: 4px; letter-spacing: -0.02em; }
.stat-label { font-size: 11px; font-weight: 700; color: #64748b; letter-spacing: 0.05em; }
.trend-badge { padding: 4px 8px; border-radius: 6px; font-size: 11px; font-weight: 700; }
.trend-badge.up { background: #ecfdf5; color: #059669; }
.filter-bar { padding: 12px 20px; display: flex; justify-content: space-between; align-items: center; }
.search-box { position: relative; flex-grow: 1; max-width: 400px; }
.search-box input { width: 100%; padding: 10px 15px 10px 42px; border-radius: 10px; border: 1px solid #e2e8f0; background: #f8fafc; font-size: 14px; outline: none; transition: 0.2s; }
.search-box input:focus { border-color: #f59e0b; background: white; box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1); }
.avatar-letter { width: 38px; height: 38px; background: #eef2ff; color: #4f46e5; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-weight: 800; border: 1px solid #e0e7ff; }
.btn-circle { width: 32px; height: 32px; border-radius: 8px; border: none; display: inline-flex; align-items: center; justify-content: center; transition: 0.2s; }
.btn-circle.approve { background: #ecfdf5; color: #10b981; }
.btn-circle.reject { background: #fef2f2; color: #ef4444; }
.chart-container { position: relative; height: 200px; }
.chart-center-text { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); text-align: center; pointer-events: none; }
.chart-center-text .number { font-size: 24px; font-weight: 800; display: block; color: #0f172a; }
.chart-center-text .label { font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; }
.tiny { font-size: 11px; }
.slide-up-enter-active, .slide-up-leave-active { transition: all 0.3s ease-out; }
.slide-up-enter-from { opacity: 0; transform: translateY(20px); }
</style>