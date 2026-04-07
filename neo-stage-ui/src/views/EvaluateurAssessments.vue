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
                <input type="text" class="form-control-pro" placeholder="ex: Développeur FullStack Senior">
              </div>
              <div class="col-12">
                <label class="form-label fw-bold small">Description</label>
                <textarea class="form-control-pro" rows="3" placeholder="Objectifs du test..."></textarea>
              </div>
              <div class="col-md-6">
                <label class="form-label fw-bold small">Durée (minutes)</label>
                <input type="number" class="form-control-pro" value="60">
              </div>
              <div class="col-md-6">
                <label class="form-label fw-bold small">Nombre de questions</label>
                <input type="number" class="form-control-pro" value="20">
              </div>
            </div>
          </div>
          <div class="modal-footer p-4 border-top">
            <button class="btn btn-light-pro me-2" @click="showCreateModal = false">Annuler</button>
            <button class="btn btn-primary-pro" @click="showCreateModal = false">Créer la campagne</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const activeTab = ref('Toutes');
const showCreateModal = ref(false);

const kpis = [
  { label: 'Total Évaluations', value: '12', icon: 'fa-solid fa-clipboard-list', color: '#EAB308', bg: '#FEFCE8' },
  { label: 'Actives Now', value: '5', icon: 'fa-solid fa-play', color: '#22C55E', bg: '#F0FDF4' },
  { label: 'Total Candidats', value: '284', icon: 'fa-solid fa-users', color: '#3B82F6', bg: '#EFF6FF' },
  { label: 'Terminées', value: '1,234', icon: 'fa-solid fa-circle-check', color: '#06B6D4', bg: '#ECFEFF' },
];

const assessments = ref([
  { id: 1, title: 'Senior Full Stack Developer', description: 'Évaluation complète couvrant React, Node.js, bases de données et conception système.', questions: 45, duration: 90, progress: 68, status: 'Actives', color: '#3B82F6', icon: 'fa-solid fa-code', iconBg: '#FEF9C3' },
  { id: 2, title: 'Database Administrator', description: 'SQL, NoSQL, optimisation de performance et évaluation de l\'architecture.', questions: 30, duration: 60, progress: 42, status: 'Actives', color: '#10B981', icon: 'fa-solid fa-database', iconBg: '#E0F2FE' },
  { id: 3, title: 'DevOps Engineer Assessment', description: 'CI/CD, conteneurisation, services cloud et automatisation d\'infrastructure.', questions: 35, duration: 75, progress: 0, status: 'Planifiées', color: '#94A3B8', icon: 'fa-solid fa-server', iconBg: '#FEF3C7' },
]);

const filteredAssessments = computed(() => {
  if (activeTab.value === 'Toutes') return assessments.value;
  return assessments.value.filter(a => a.status === activeTab.value);
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