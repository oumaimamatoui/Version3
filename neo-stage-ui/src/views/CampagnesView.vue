<template>
  <div class="admin-layout">
    <div class="background-mesh"></div>
    <AppSidebar />
    
    <div class="main-content flex-grow-1">
      <AppNavbar />

      <div class="content-wrapper p-4 p-lg-5">
        <!-- HEADER PROFESSIONNEL -->
        <header class="d-flex justify-content-between align-items-end mb-5 animate__animated animate__fadeIn">
          <div>
            <div class="breadcrumb-pro mb-2">
              <span class="root">Administration</span>
              <i class="fa-solid fa-chevron-right mx-2 separator"></i>
              <span class="current">Sessions d'Évaluation</span>
            </div>
            <h2 class="premium-title">Gestion des <span class="gradient-text">Campagnes</span></h2>
          </div>
          <div class="d-flex gap-3">
            <button class="btn-refresh-pro" @click="fetchData" :disabled="loading">
              <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
            </button>
            <button class="btn-action-primary shadow-premium" @click="openAddModal">
              <i class="fa-solid fa-plus me-2"></i> Nouvelle Campagne
            </button>
          </div>
        </header>

        <!-- KPI STATS (Glassmorphism Style) -->
        <div class="row g-4 mb-5 animate__animated animate__fadeInUp">
          <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
            <div class="stat-card-premium">
              <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div class="stat-details">
                <div class="stat-value">{{ stat.value }}</div>
                <div class="stat-label">{{ stat.label }}</div>
              </div>
              <div class="stat-decoration"></div>
            </div>
          </div>
        </div>

        <!-- SECTION : ONGLETS DE FILTRAGE -->
        <div class="tabs-container mb-4 animate__animated animate__fadeIn">
          <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border fit-content">
            <button 
              v-for="tab in filterTabs" 
              :key="tab.label"
              class="nav-tab-btn-modern"
              :class="{ active: activeTab === tab.value }"
              @click="activeTab = tab.value"
            >
              {{ tab.label }}
              <span class="tab-count">{{ tab.count }}</span>
            </button>
          </div>
        </div>

        <!-- LISTE DES CAMPAGNES FILTRÉES -->
        <div class="row g-4">
          <div v-if="loading" class="col-12 text-center py-5">
            <div class="loader-container">
              <div class="spinner-pro-premium"></div>
              <p class="mt-3 text-muted fw-600 ls-1">SYNCHRONISATION EN COURS</p>
            </div>
          </div>

          <div v-else-if="filteredCampaigns.length === 0" class="col-12 text-center py-5">
             <div class="empty-state-card shadow-sm">
                <div class="empty-icon-box">
                  <i class="fa-solid fa-folder-open fa-2x"></i>
                </div>
                <h5>Aucune campagne trouvée</h5>
                <p>Commencez par créer une nouvelle session d'évaluation pour vos candidats.</p>
             </div>
          </div>

          <div v-else v-for="c in filteredCampaigns" :key="c.id" class="col-xl-4 col-md-6 animate__animated animate__fadeInUp">
            <div class="campaign-card-modern">
              <div class="card-header-modern">
                <span class="status-badge" :class="'status-' + c.statut">
                  <span class="status-dot"></span>
                  {{ getStatusLabel(c.statut) }}
                </span>
                <div class="dropdown">
                  <button class="btn-options-round" data-bs-toggle="dropdown">
                    <i class="fa-solid fa-ellipsis-vertical"></i>
                  </button>
                  <ul class="dropdown-menu dropdown-menu-end border-0 shadow-premium p-2 rounded-4">
                    <li><button class="dropdown-item rounded-3" @click="openEditModal(c)"><i class="fa-solid fa-pen-to-square me-2 text-primary"></i>Modifier</button></li>
                    <li><hr class="dropdown-divider opacity-50"></li>
                    <li><button class="dropdown-item rounded-3 text-danger" @click="handleDelete(c.id)"><i class="fa-solid fa-trash-can me-2"></i>Supprimer</button></li>
                  </ul>
                </div>
              </div>

              <h5 class="campaign-title-modern">{{ c.nom }}</h5>
              
              <div class="test-attachment-box">
                <div class="icon-file"><i class="fa-solid fa-file-signature"></i></div>
                <div class="flex-grow-1">
                  <span class="text-overline">Questionnaire</span>
                  <p class="text-truncate-1">{{ getQuestionnaireName(c.questionnaireId) }}</p>
                </div>
              </div>

              <div class="timeline-box">
                <div class="d-flex justify-content-between mb-2">
                  <span class="date-label"><i class="fa-regular fa-calendar me-1"></i> {{ formatDate(c.dateDebut) }}</span>
                  <span class="date-label text-end"><i class="fa-solid fa-flag-checkered me-1"></i> {{ formatDate(c.dateFin) }}</span>
                </div>
                <div class="progress-premium">
                  <div class="progress-bar-glow" :style="{ width: calculateTimeProgress(c.dateDebut, c.dateFin) + '%' }"></div>
                </div>
              </div>

              <div class="card-footer-modern">
                <div class="meta-item"><i class="fa-regular fa-clock me-2"></i>{{ c.dureeMinutes }} min</div>
                <div class="meta-item"><i class="fa-solid fa-user-group me-2"></i>{{ c.maxCandidats }} max</div>
              </div>
            </div>
          </div>
        </div>

        <!-- MODALE DE CONFIGURATION -->
        <transition name="modal-fade">
          <div v-if="showModal" class="modal-overlay-blur" @click.self="showModal = false">
            <div class="modal-card-glass animate__animated animate__zoomIn animate__faster">
              <div class="modal-header-premium">
                <div class="header-icon-box">
                  <i class="fa-solid fa-sliders"></i>
                </div>
                <div class="ms-3">
                  <h4 class="mb-0 fw-800">{{ isEditing ? 'Modifier la session' : 'Nouvelle session' }}</h4>
                  <p class="mb-0 text-muted small">Paramétrez les critères d'évaluation</p>
                </div>
                <button class="btn-close-custom ms-auto" @click="showModal = false">&times;</button>
              </div>

              <div class="modal-body-premium">
                <div class="row g-4">
                  <div class="col-12">
                    <label class="form-label-premium">Nom de la Campagne</label>
                    <input v-model="form.nom" class="form-input-premium" placeholder="ex: Recrutement Tech Lead 2024">
                  </div>

                  <div class="col-12">
                    <label class="form-label-premium">Test de référence</label>
                    <div class="select-wrapper">
                      <select v-model="form.questionnaireId" class="form-select-premium">
                        <option value="" disabled>Sélectionner un questionnaire...</option>
                        <option v-for="q in questionnairesList" :key="q.id" :value="q.id">
                          {{ q.titre }} (Niveau {{ q.difficulte }})
                        </option>
                      </select>
                    </div>
                  </div>

                  <div class="col-md-6">
                    <label class="form-label-premium">Date de début</label>
                    <input type="datetime-local" v-model="form.dateDebut" class="form-input-premium">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label-premium">Date de clôture</label>
                    <input type="datetime-local" v-model="form.dateFin" class="form-input-premium">
                  </div>

                  <div class="col-md-4">
                    <label class="form-label-premium">Durée (min)</label>
                    <input type="number" v-model.number="form.dureeMinutes" class="form-input-premium">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label-premium">Passage (%)</label>
                    <input type="number" v-model.number="form.scorePassage" class="form-input-premium">
                  </div>
                  <div class="col-md-4">
                    <label class="form-label-premium">Places max</label>
                    <input type="number" v-model.number="form.maxCandidats" class="form-input-premium">
                  </div>
                </div>
              </div>

              <div class="modal-footer-premium">
                <button class="btn-cancel-link" @click="showModal = false">Ignorer les changements</button>
                <button class="btn-action-primary shadow-premium px-4" @click="saveCampaign" :disabled="submitting || !form.questionnaireId">
                  <span v-if="submitting" class="spinner-border spinner-border-sm me-2"></span>
                  <i v-else class="fa-solid fa-check-circle me-2"></i>
                  {{ isEditing ? 'Mettre à jour' : 'Lancer la session' }}
                </button>
              </div>
            </div>
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import axios from 'axios';

// API LOGIC
const API_BASE = "http://localhost:5172/api";
const campaigns = ref([]);
const questionnairesList = ref([]);
const loading = ref(true);
const showModal = ref(false);
const isEditing = ref(false);
const submitting = ref(false);
const activeTab = ref('all');

const statusLabels = ["Brouillon", "Active", "Fermée", "Archivée"];

const form = reactive({
  id: null, nom: '', dateDebut: '', dateFin: '', maxCandidats: 100, 
  dureeMinutes: 60, scorePassage: 70, statut: 1, questionnaireId: '',
  entrepriseId: "00000000-0000-0000-0000-000000000000"
});

// LOGIQUE DES ONGLETS DE FILTRAGE
const filterTabs = computed(() => [
  { label: 'Tout', value: 'all', count: campaigns.value.length },
  { label: 'Actives', value: 1, count: campaigns.value.filter(c => c.statut === 1).length },
  { label: 'Planifiées', value: 0, count: campaigns.value.filter(c => c.statut === 0).length },
  { label: 'Terminées', value: 2, count: campaigns.value.filter(c => c.statut === 2).length }
]);

const filteredCampaigns = computed(() => {
  if (activeTab.value === 'all') return campaigns.value;
  return campaigns.value.filter(c => c.statut === activeTab.value);
});

const fetchData = async () => {
  loading.value = true;
  try {
    const [cRes, qRes] = await Promise.all([
      axios.get(`${API_BASE}/Campagnes`),
      axios.get(`${API_BASE}/Questionnaires`)
    ]);
    campaigns.value = cRes.data;
    questionnairesList.value = qRes.data;
  } catch (e) { 
    console.error("Error fetching data", e); 
  } finally { 
    loading.value = false; 
  }
};

const getQuestionnaireName = (id) => questionnairesList.value.find(q => q.id === id)?.titre || "Inconnu";
const formatDate = (date) => date ? new Date(date).toLocaleDateString('fr-FR', { day: 'numeric', month: 'short' }) : '∞';

const calculateTimeProgress = (start, end) => {
  if (!start || !end) return 0;
  const now = new Date(), s = new Date(start), e = new Date(end);
  if (now < s) return 0; if (now > e) return 100;
  return Math.round(((now - s) / (e - s)) * 100);
};

const openAddModal = () => {
  isEditing.value = false;
  Object.assign(form, { id: null, nom: '', dateDebut: '', dateFin: '', maxCandidats: 100, questionnaireId: '', statut: 1 });
  showModal.value = true;
};

const openEditModal = (c) => {
  isEditing.value = true;
  Object.assign(form, { ...c, dateDebut: c.dateDebut?.substring(0, 16), dateFin: c.dateFin?.substring(0, 16) });
  showModal.value = true;
};

const saveCampaign = async () => {
  submitting.value = true;
  try {
    if (isEditing.value) await axios.put(`${API_BASE}/Campagnes/${form.id}`, form);
    else await axios.post(`${API_BASE}/Campagnes`, form);
    showModal.value = false; 
    fetchData();
  } catch (e) { 
    alert("Erreur serveur."); 
  } finally { 
    submitting.value = false; 
  }
};

const handleDelete = async (id) => {
  if(confirm("Confirmer la suppression ?")) {
    await axios.delete(`${API_BASE}/Campagnes/${id}`); 
    fetchData();
  }
};

const getStatusLabel = (s) => statusLabels[s] || "DRAFT";

const kpiStats = computed(() => [
  { label: 'Campagnes Total', value: campaigns.value.length, icon: 'fa-solid fa-layer-group', color: '#f59e0b', bg: '#fffbeb' },
  { label: 'En cours', value: campaigns.value.filter(c => c.statut === 1).length, icon: 'fa-solid fa-bolt-lightning', color: '#10b981', bg: '#ecfdf5' },
  { label: 'Tests Liés', value: questionnairesList.value.length, icon: 'fa-solid fa-file-shield', color: '#6366f1', bg: '#eef2ff' },
  { label: 'Capacité Totale', value: campaigns.value.reduce((acc, c) => acc + c.maxCandidats, 0), icon: 'fa-solid fa-user-group', color: '#f43f5e', bg: '#fff1f2' }
]);

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* FOND ET BASE */
.admin-layout { 
  font-family: 'Plus Jakarta Sans', sans-serif; 
  background-color: #fcfdfe; 
  min-height: 100vh; 
  color: #1e293b;
}

.background-mesh {
  position: fixed;
  inset: 0;
  z-index: -1;
  background-color: #fcfdfe;
  background-image: 
    radial-gradient(at 0% 0%, rgba(245, 158, 11, 0.05) 0px, transparent 50%),
    radial-gradient(at 100% 0%, rgba(99, 102, 241, 0.05) 0px, transparent 50%);
}

.fit-content { width: fit-content; }

/* TEXTES GRADIENTS */
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  font-weight: 800;
}

/* TITRES */
.premium-title { font-weight: 800; color: #0f172a; font-size: 2.2rem; letter-spacing: -1px; }
.breadcrumb-pro { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 1.5px; }
.breadcrumb-pro .current { color: #f59e0b; }

/* KPI CARDS */
.stat-card-premium {
  background: white;
  border-radius: 24px;
  padding: 24px;
  display: flex;
  align-items: center;
  position: relative;
  overflow: hidden;
  border: 1px solid rgba(226, 232, 240, 0.8);
  box-shadow: 0 10px 30px -10px rgba(0, 0, 0, 0.04);
  transition: transform 0.3s ease;
}
.stat-card-premium:hover { transform: translateY(-5px); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; z-index: 1;
}
.stat-details { margin-left: 20px; z-index: 1; }
.stat-value { font-size: 1.75rem; font-weight: 800; color: #0f172a; line-height: 1.2; }
.stat-label { font-size: 0.75rem; font-weight: 600; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; }

/* TABS MODERNES */
.nav-tab-btn-modern {
  background: transparent; border: none;
  padding: 10px 20px; border-radius: 12px;
  font-size: 0.9rem; font-weight: 700; color: #64748b;
  display: flex; align-items: center; gap: 10px;
  transition: all 0.25s cubic-bezier(0.4, 0, 0.2, 1);
}
.nav-tab-btn-modern.active {
  background: #0f172a; color: white;
  box-shadow: 0 10px 20px -5px rgba(15, 23, 42, 0.3);
}
.tab-count {
  background: rgba(148, 163, 184, 0.15);
  padding: 2px 8px; border-radius: 8px; font-size: 0.75rem;
}
.active .tab-count { background: rgba(255, 255, 255, 0.2); }

/* CAMPAIGN CARDS */
.campaign-card-modern {
  background: white; border-radius: 30px; padding: 28px;
  border: 1px solid #eef2f6; height: 100%;
  display: flex; flex-direction: column;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.02), 0 2px 4px -1px rgba(0, 0, 0, 0.01);
}
.campaign-card-modern:hover {
  transform: translateY(-10px) scale(1.02);
  border-color: #f59e0b;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.08);
}

.card-header-modern { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; }
.status-badge {
  display: inline-flex; align-items: center;
  padding: 6px 14px; border-radius: 12px;
  font-size: 0.7rem; font-weight: 800; text-transform: uppercase; letter-spacing: 0.5px;
}
.status-dot { width: 6px; height: 6px; border-radius: 50%; margin-right: 8px; }

.status-1 { background: #ecfdf5; color: #059669; }
.status-1 .status-dot { background: #059669; box-shadow: 0 0 10px #059669; }
.status-2 { background: #fff1f2; color: #e11d48; }
.status-2 .status-dot { background: #e11d48; }

.campaign-title-modern { font-weight: 800; color: #0f172a; font-size: 1.25rem; margin-bottom: 20px; line-height: 1.4; }

.test-attachment-box {
  background: #f8fafc; border-radius: 20px; padding: 16px;
  display: flex; align-items: center; gap: 15px; margin-bottom: 24px;
}
.icon-file {
  width: 40px; height: 40px; background: white;
  border-radius: 12px; display: flex; align-items: center;
  justify-content: center; color: #f59e0b; font-size: 1.2rem;
  box-shadow: 0 4px 10px rgba(0,0,0,0.05);
}
.text-overline { font-size: 0.65rem; text-transform: uppercase; font-weight: 800; color: #94a3b8; display: block; }
.text-truncate-1 { margin: 0; font-size: 0.9rem; font-weight: 700; color: #334155; }

.timeline-box .date-label { font-size: 0.75rem; font-weight: 600; color: #64748b; }
.progress-premium { height: 8px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-bar-glow {
  height: 100%; background: linear-gradient(90deg, #f59e0b, #fbbf24);
  border-radius: 10px; position: relative;
}

.card-footer-modern {
  margin-top: auto; padding-top: 24px; border-top: 1px solid #f1f5f9;
  display: flex; gap: 15px;
}
.meta-item {
  font-size: 0.8rem; font-weight: 700; color: #64748b;
  background: #f8fafc; padding: 6px 12px; border-radius: 10px;
}

/* MODAL DESIGN */
.modal-overlay-blur {
  position: fixed; inset: 0; background: rgba(15, 23, 42, 0.4);
  backdrop-filter: blur(8px); z-index: 9999;
  display: flex; align-items: center; justify-content: center; padding: 20px;
}
.modal-card-glass {
  background: rgba(255, 255, 255, 0.95); width: 100%; max-width: 750px;
  border-radius: 32px; overflow: hidden;
  box-shadow: 0 50px 100px -20px rgba(0,0,0,0.25);
  border: 1px solid white;
}
.modal-header-premium {
  padding: 30px; border-bottom: 1px solid #f1f5f9;
  display: flex; align-items: center;
}
.header-icon-box {
  width: 48px; height: 48px; background: #fff7ed;
  color: #f59e0b; border-radius: 14px;
  display: flex; align-items: center; justify-content: center; font-size: 1.2rem;
}
.modal-body-premium { padding: 30px; max-height: 70vh; overflow-y: auto; }

.form-label-premium { font-size: 0.75rem; font-weight: 800; color: #475569; text-transform: uppercase; margin-bottom: 10px; display: block; }
.form-input-premium, .form-select-premium {
  width: 100%; background: #f8fafc; border: 2px solid #f1f5f9;
  padding: 14px 18px; border-radius: 16px; font-weight: 600;
  transition: all 0.2s;
}
.form-input-premium:focus {
  border-color: #f59e0b; background: white; outline: none;
  box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.1);
}

.modal-footer-premium {
  padding: 24px 30px; background: #f8fafc;
  display: flex; justify-content: space-between; align-items: center;
}

/* BOUTONS */
.btn-action-primary {
  background: #0f172a; color: white; border: none;
  padding: 12px 28px; border-radius: 14px;
  font-weight: 700; transition: 0.3s;
}
.btn-action-primary:hover:not(:disabled) { background: #f59e0b; transform: translateY(-2px); }

.btn-refresh-pro {
  width: 48px; height: 48px; border-radius: 14px;
  border: 1px solid #e2e8f0; background: white;
  color: #64748b; transition: 0.2s;
}
.btn-refresh-pro:hover { color: #f59e0b; border-color: #f59e0b; background: #fffbeb; }

.shadow-premium { box-shadow: 0 10px 20px -5px rgba(0, 0, 0, 0.1); }

/* ANIMATIONS & LOADER */
.spinner-pro-premium {
  width: 40px; height: 40px; border: 3px solid #f1f5f9;
  border-top-color: #f59e0b; border-radius: 50%;
  animation: spin 0.8s cubic-bezier(0.4, 0, 0.2, 1) infinite;
  margin: auto;
}
@keyframes spin { to { transform: rotate(360deg); } }

.ls-1 { letter-spacing: 2px; font-size: 0.7rem; }

/* Custom Scrollbar */
::-webkit-scrollbar { width: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
::-webkit-scrollbar-thumb:hover { background: #cbd5e1; }

/* Dropdown styling */
.btn-options-round {
  background: none; border: none; color: #94a3b8;
  width: 36px; height: 36px; border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s;
}
.btn-options-round:hover { background: #f1f5f9; color: #0f172a; }

/* Empty state design */
.empty-state-card {
  background: white; border-radius: 24px; padding: 40px;
  max-width: 400px; margin: auto; border: 1px dashed #cbd5e1;
}
.empty-icon-box {
  width: 64px; height: 64px; background: #f8fafc; color: #94a3b8;
  border-radius: 20px; display: flex; align-items: center; justify-content: center;
  margin: 0 auto 20px auto;
}
.empty-state-card h5 { font-weight: 800; color: #334155; }
.empty-state-card p { font-size: 0.9rem; color: #64748b; margin-bottom: 0; }
</style>