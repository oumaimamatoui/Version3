<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    
    <div class="content flex-grow-1 bg-light-soft">
      <AppNavbar />

      <main class="p-4 animate-fade-in">
        <!-- 1. HEADER DYNAMIQUE -->
        <div class="d-flex justify-content-between align-items-center mb-4">
          <div>
            <h2 class="fw-800 text-navy m-0">Évaluations</h2>
            <p class="text-muted small">Gérez vos campagnes d'évaluation et suivez la progression des candidats.</p>
          </div>
          <button @click="showCreateModal = true" class="btn btn-create-assessment shadow-sm">
            <i class="fa-solid fa-plus me-2"></i> Créer une évaluation
          </button>
        </div>

        <!-- 2. CARTES DE STATISTIQUES (KPI) -->
        <div class="row g-3 mb-4">
          <div class="col-md-3" v-for="stat in kpis" :key="stat.label">
            <div class="glass-card p-3 d-flex align-items-center gap-3 border-0 shadow-sm">
              <div class="icon-box-lg" :style="{ color: stat.color, background: stat.bg }">
                <i :class="stat.icon"></i>
              </div>
              <div>
                <div class="fw-800 fs-4 text-navy">{{ stat.value }}</div>
                <div class="tiny text-muted uppercase fw-bold ls-1">{{ stat.label }}</div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. ONGLETS DE FILTRAGE -->
        <div class="tabs-container mb-4">
          <button 
            v-for="tab in ['Toutes', 'Actives', 'Planifiées', 'Terminées']" 
            :key="tab"
            @click="activeTab = tab"
            :class="['tab-btn', { active: activeTab === tab }]"
          >
            {{ tab }} <span class="tab-badge" v-if="tab === 'Toutes'">12</span>
          </button>
        </div>

        <!-- 4. GRILLE D'ÉVALUATIONS -->
        <div class="row g-4">
          <div class="col-xl-4 col-md-6" v-for="assessment in filteredAssessments" :key="assessment.id">
            <div class="assessment-card shadow-sm border-0 h-100">
              <div class="p-4">
                <!-- Status & Menu -->
                <div class="d-flex justify-content-between align-items-start mb-3">
                  <div class="icon-vessel" :style="{ background: assessment.iconBg }">
                    <i :class="assessment.icon"></i>
                  </div>
                  <div class="d-flex align-items-center gap-2">
                    <span :class="['badge-status', assessment.status.toLowerCase()]">
                      <span class="dot"></span> {{ assessment.status }}
                    </span>
                    <i class="fa-solid fa-ellipsis-vertical text-muted cursor-pointer"></i>
                  </div>
                </div>

                <h5 class="fw-bold text-navy mb-2">{{ assessment.title }}</h5>
                <p class="text-muted small mb-4 line-clamp">{{ assessment.description }}</p>

                <!-- Meta Info -->
                <div class="d-flex gap-3 mb-4">
                  <span class="meta-item"><i class="fa-regular fa-file-lines me-1"></i> {{ assessment.questions }} questions</span>
                  <span class="meta-item"><i class="fa-regular fa-clock me-1"></i> {{ assessment.duration }} min</span>
                </div>

                <!-- Progression -->
                <div class="progress-section mb-4">
                  <div class="d-flex justify-content-between tiny fw-bold mb-1">
                    <span>Complétion</span>
                    <span>{{ assessment.progress }}%</span>
                  </div>
                  <div class="progress-track">
                    <div class="progress-bar-fill" :style="{ width: assessment.progress + '%', background: assessment.color }"></div>
                  </div>
                </div>

                <!-- Footer Card -->
                <div class="d-flex justify-content-between align-items-center mt-3 pt-3 border-top">
                  <div class="avatar-group">
                    <img v-for="i in 3" :key="i" :src="`https://i.pravatar.cc/100?u=${assessment.id + i}`" class="group-avatar">
                    <div class="group-avatar-more">+{{ assessment.candidates - 3 }}</div>
                  </div>
                  <button class="btn-view-details">Voir détails</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>

      <!-- MODALE DE CRÉATION D'ÉVALUATION -->
      <Transition name="fade">
        <div v-if="showCreateModal" class="modal-overlay" @click.self="showCreateModal = false">
          <div class="modal-card animate-zoom-in">
            <div class="modal-header border-bottom p-4">
              <h5 class="fw-800 text-navy m-0">Créer une nouvelle évaluation</h5>
              <button class="btn-close" @click="showCreateModal = false"></button>
            </div>
            <div class="modal-body p-4">
              <div class="row g-3">
                <div class="col-12">
                  <label class="form-label fw-bold small">Titre de l'évaluation</label>
                  <input type="text" v-model="newCampagne.nom" class="form-control-pro" placeholder="ex: Développeur FullStack Senior" required>
                </div>
                <div class="col-12">
                  <label class="form-label fw-bold small">Description</label>
                  <textarea v-model="newCampagne.description" class="form-control-pro" rows="3" placeholder="Objectifs du test..." required></textarea>
                </div>
                <div class="col-12">
                  <label class="form-label fw-bold small">Questionnaire (Banque de questions)</label>
                  <select v-model="newCampagne.questionnaireId" class="form-control-pro" required>
                    <option value="" disabled>-- Sélectionnez un questionnaire --</option>
                    <option v-for="q in questionnaires" :key="q.id" :value="q.id">{{ q.nom }} ({{ q.questions?.length || 0 }} questions)</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label fw-bold small">Durée (minutes)</label>
                  <input type="number" v-model="newCampagne.dureeMinutes" class="form-control-pro" value="60">
                </div>
                <div class="col-md-6">
                  <label class="form-label fw-bold small">Max Candidats</label>
                  <input type="number" v-model="newCampagne.maxCandidats" class="form-control-pro" value="100">
                </div>
              </div>
            </div>
            <div class="modal-footer p-4 border-top">
              <button class="btn btn-light-pro me-2" @click="showCreateModal = false">Annuler</button>
              <button class="btn btn-primary-pro" @click="createCampagne" :disabled="isSaving || !newCampagne.questionnaireId">
                <span v-if="isSaving" class="spinner-border spinner-border-sm me-2"></span>
                Créer la campagne
              </button>
            </div>
          </div>
        </div>
      </Transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const authStore = useAuthStore();
const activeTab = ref('Toutes');
const showCreateModal = ref(false);
const isSaving = ref(false);

const API_BASE = 'http://localhost:5172/api/Campagnes';

// Formulaire nouvelle campagne
const newCampagne = ref({
  nom: '',
  description: '',
  questionnaireId: '',
  dureeMinutes: 60,
  maxCandidats: 100,
  statut: 1, // 1 = Active
  dateDebut: new Date().toISOString(),
  dateFin: new Date(new Date().setMonth(new Date().getMonth() + 1)).toISOString()
});

const kpis = ref([
  { label: 'Total Évaluations', value: '0', icon: 'fa-solid fa-clipboard-list', color: '#EAB308', bg: '#FEFCE8' },
  { label: 'Actives Now', value: '0', icon: 'fa-solid fa-play', color: '#22C55E', bg: '#F0FDF4' },
  { label: 'Total Candidats', value: '0', icon: 'fa-solid fa-users', color: '#3B82F6', bg: '#EFF6FF' },
  { label: 'Total Tests', value: '0', icon: 'fa-solid fa-database', color: '#06B6D4', bg: '#ECFEFF' },
]);

const assessments = ref([]);
const questionnaires = ref([]);

// Configuration Axios avec Sécurité (Token JWT)
const getAuthHeaders = () => {
    return { headers: { Authorization: `Bearer ${authStore.token}` } };
};

// Récupérer les questionnaires pour le dropdown
const fetchQuestionnaires = async () => {
    try {
        const res = await axios.get('http://localhost:5172/api/Questionnaire', getAuthHeaders());
        questionnaires.value = res.data;
    } catch (e) {
        console.error("Erreur chargement questionnaires:", e);
    }
};

// Récupérer les KPIs
const fetchStats = async () => {
    try {
        const response = await axios.get(`${API_BASE}/stats`, getAuthHeaders());
        const data = response.data;
        kpis.value[0].value = data.totalCampaigns.toString();
        kpis.value[1].value = data.activeCampaigns.toString();
        kpis.value[2].value = data.totalCapacity.toString();
        kpis.value[3].value = data.totalTests.toString();
    } catch (error) {
        console.error('Erreur KPIs:', error);
    }
};

// Récupérer la liste des campagnes
const fetchAssessments = async () => {
    try {
        const response = await axios.get(API_BASE, getAuthHeaders());
        assessments.value = response.data.map(c => {
            // Mapping du statut backend (0=Planifié, 1=Active, 2=Terminée)
            let statusText = 'Actives';
            let color = '#3B82F6';
            if (c.statut === 0) { statusText = 'Planifiées'; color = '#94A3B8'; }
            if (c.statut === 2) { statusText = 'Terminées'; color = '#10B981'; }

            return {
                id: c.id,
                title: c.nom,
                description: c.description || 'Description non disponible',
                questions: 20, // Valeur par défaut pour l'instant
                duration: c.dureeMinutes || 60,
                progress: 0,
                status: statusText,
                color: color,
                candidates: c.maxCandidats,
                icon: 'fa-solid fa-code', 
                iconBg: '#FEF9C3'
            };
        });
    } catch (error) {
        console.error('Erreur Campagnes:', error);
    }
};

// Créer une nouvelle campagne
const createCampagne = async () => {
    if (!newCampagne.value.nom || !newCampagne.value.description) {
        alert("Veuillez remplir le nom et la description.");
        return;
    }

    isSaving.value = true;
    try {
        await axios.post(API_BASE, newCampagne.value, getAuthHeaders());
        showCreateModal.value = false;
        
        // Reset form
        newCampagne.value.nom = '';
        newCampagne.value.description = '';
        
        // Refresh data
        await fetchAssessments();
        await fetchStats();
    } catch (error) {
        console.error('Erreur Création:', error);
        alert(error.response?.data?.message || "Erreur lors de la création.");
    } finally {
        isSaving.value = false;
    }
};

const filteredAssessments = computed(() => {
  if (activeTab.value === 'Toutes') return assessments.value;
  return assessments.value.filter(a => a.status === activeTab.value);
});

// Chargement initial
onMounted(() => {
    fetchStats();
    fetchAssessments();
    fetchQuestionnaires();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.admin-body { background-color: #F8FAFC; font-family: 'Plus Jakarta Sans', sans-serif; }
.text-navy { color: #0F172A; }
.bg-light-soft { background-color: #FCFCFD; }

/* --- BUTTONS --- */
.btn-create-assessment {
  background-color: #FACC15; color: #0F172A; font-weight: 700; border: none;
  padding: 10px 20px; border-radius: 12px; transition: 0.3s;
}
.btn-create-assessment:hover { background-color: #EAB308; transform: translateY(-2px); }

/* --- KPI CARDS --- */
.glass-card { background: white; border-radius: 20px; border: 1px solid #F1F5F9; }
.icon-box-lg { width: 48px; height: 48px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px; }

/* --- TABS --- */
.tab-btn {
  border: none; background: transparent; padding: 8px 16px; font-weight: 700;
  color: #64748B; font-size: 14px; border-bottom: 2px solid transparent; transition: 0.3s;
}
.tab-btn.active { color: #0F172A; border-bottom: 2px solid #FACC15; }
.tab-badge { background: #FEF9C3; color: #854D0E; padding: 2px 8px; border-radius: 6px; font-size: 12px; margin-left: 5px; }

/* --- ASSESSMENT CARDS --- */
.assessment-card { background: white; border-radius: 24px; transition: 0.3s; border: 1px solid #F1F5F9 !important; }
.assessment-card:hover { transform: translateY(-5px); box-shadow: 0 15px 30px rgba(0,0,0,0.05) !important; }
.icon-vessel { width: 42px; height: 42px; border-radius: 12px; display: flex; align-items: center; justify-content: center; color: #EAB308; font-size: 18px; }

.badge-status { padding: 4px 12px; border-radius: 100px; font-size: 10px; font-weight: 800; text-transform: uppercase; display: flex; align-items: center; gap: 6px; }
.badge-status.actives { background: #DCFCE7; color: #166534; }
.badge-status.planifiées { background: #FEF3C7; color: #92400E; }
.dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; }

.meta-item { color: #64748B; font-size: 12px; font-weight: 600; }
.progress-track { height: 8px; background: #F1F5F9; border-radius: 10px; overflow: hidden; }
.progress-bar-fill { height: 100%; border-radius: 10px; transition: 1s ease; }

.btn-view-details { background: transparent; border: none; color: #64748B; font-weight: 700; font-size: 13px; }
.avatar-group { display: flex; align-items: center; }
.group-avatar { width: 28px; height: 28px; border-radius: 50%; border: 2px solid white; margin-right: -10px; }
.group-avatar-more { width: 28px; height: 28px; border-radius: 50%; background: #F1F5F9; font-size: 9px; font-weight: 800; display: flex; align-items: center; justify-content: center; }

/* --- MODAL --- */
.modal-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.4); backdrop-filter: blur(4px); z-index: 2000; display: flex; align-items: center; justify-content: center; }
.modal-card { background: white; width: 100%; max-width: 550px; border-radius: 28px; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.25); }
.form-control-pro { background: #F8FAFC; border: 1px solid #E2E8F0; border-radius: 12px; padding: 12px; font-size: 14px; width: 100%; outline: none; }
.btn-primary-pro { background: #0F172A; color: white; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 700; }

.line-clamp { display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.tiny { font-size: 10px; }
.uppercase { text-transform: uppercase; }
.ls-1 { letter-spacing: 0.5px; }
</style>