<template>
  <div class="admin-layout">
    <div class="background-overlay"></div>
    <div class="tech-grid-subtle"></div>
    <div class="glow-orb orb-amber"></div>
    
    <AppSidebar />
    
    <div class="main-viewport animate__animated animate__fadeIn">
      <AppNavbar />

      <div class="container-fluid px-lg-5 pt-4">
        <div class="terminal-status-bar mb-4">
          <div class="d-flex justify-content-between align-items-center px-4">
            <div class="breadcrumb-cyber">
              <span class="root">EXAMEN</span>
              <span class="sep">/</span>
              <span class="current">MES TESTS DISPONIBLES</span>
            </div>
          </div>
        </div>

        <div class="d-flex justify-content-between align-items-end mb-4">
          <div>
            <h1 class="display-title-cyber mb-1">Passer un <span class="text-gradient-amber">test</span></h1>
            <p class="text-muted">Sélectionnez une évaluation pour commencer votre session.</p>
          </div>
          <div class="test-stats-group">
            <div class="test-count-badge active">
              <span class="count">{{ activeTests.length }}</span>
              <span class="label">Disponibles</span>
            </div>
            <div class="test-count-badge expired">
              <span class="count">{{ expiredTests.length }}</span>
              <span class="label">Terminés/Expirés</span>
            </div>
          </div>
        </div>        <div v-if="loading" class="text-center py-5">
          <div class="loader-ripple"><div></div><div></div></div>
          <p class="mt-3 text-muted fw-semibold">Chargement de vos tests...</p>
        </div>

        <div v-else>
          <!-- SECTION 1: TESTS ACTIFS -->
          <div class="mb-5">
            <h4 class="section-subtitle mb-4"><i class="fa-solid fa-bolt text-warning me-2"></i> TESTS ACTIFS & À VENIR</h4>
            <div class="row g-4">
              <div v-if="activeTests.length === 0" class="col-12">
                <div class="empty-state-panel py-4">
                  <p class="text-muted m-0">Aucun test actif pour le moment.</p>
                </div>
              </div>

              <div v-for="c in activeTests" :key="c.id" class="col-md-6 col-xl-4">
                <div class="campaign-card-glass" :class="{ 'upcoming': new Date(c.dateDebut) > new Date() }">
                  <div class="card-header-tech">
                    <span class="category-tag">DISPONIBLE</span>
                    <div class="duration-tag"><i class="fa-regular fa-clock me-1"></i> {{ c.dureeMinutes }}min</div>
                  </div>
                  
                  <div class="p-4 pt-3">
                    <h5 class="campaign-title">{{ c.nom }}</h5>
                    <p class="campaign-description">{{ c.description || 'Test de compétences techniques.' }}</p>
                    
                    <div class="campaign-info-grid">
                      <div class="info-item">
                        <span class="label">Échéance</span>
                        <span class="value">{{ formatDate(c.dateFin) }}</span>
                      </div>
                    </div>

                    <button 
                      @click="startExam(c)" 
                      class="btn-primary-cyber" 
                      :disabled="new Date(c.dateDebut) > new Date()"
                    >
                      <span v-if="new Date(c.dateDebut) > new Date()">
                        Ouvre le {{ formatDate(c.dateDebut) }}
                      </span>
                      <span v-else>
                        Démarrer maintenant <i class="fa-solid fa-play ms-2"></i>
                      </span>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 2: TESTS EXPIRÉS -->
          <div>
            <h4 class="section-subtitle mb-4"><i class="fa-solid fa-calendar-xmark text-danger me-2"></i> TESTS EXPIRÉS</h4>
            <div class="row g-4">
              <div v-if="expiredTests.length === 0" class="col-12">
                <div class="empty-state-panel py-4">
                  <p class="text-muted m-0">Aucun test expiré.</p>
                </div>
              </div>

              <div v-for="c in expiredTests" :key="c.id" class="col-md-6 col-xl-4 opacity-75">
                <div class="campaign-card-glass expired">
                  <div class="card-header-tech">
                    <span class="category-tag bg-slate-200 text-slate-500">EXPIRÉ</span>
                    <div class="duration-tag">{{ c.dureeMinutes }}min</div>
                  </div>
                  
                  <div class="p-4 pt-3">
                    <h5 class="campaign-title">{{ c.nom }}</h5>
                    <div class="expired-overlay">TEMPS ÉCOULÉ</div>
                    <div class="campaign-info-grid">
                      <div class="info-item">
                        <span class="label">Fermé le</span>
                        <span class="value text-danger">{{ formatDate(c.dateFin) }}</span>
                      </div>
                    </div>
                    <button class="btn-disabled-cyber" disabled>SESSION FERMÉE</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const router = useRouter();
const campaigns = ref([]);
const loading = ref(true);

const fetchMyTests = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Campagnes');
    campaigns.value = res.data;
  } catch (err) {
    console.error("Erreur:", err);
  } finally {
    loading.value = false;
  }
};

const activeTests = computed(() => {
  return campaigns.value.filter(c => new Date(c.dateFin) >= new Date());
});

const expiredTests = computed(() => {
  return campaigns.value.filter(c => new Date(c.dateFin) < new Date());
});

const startExam = (campaign) => {
  const targetId = campaign.candidatureId || campaign.id;
  router.push(`/exam-lobby/${targetId}`);
};

const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'long', year: 'numeric' }) : 'N/A';

onMounted(fetchMyTests);
</script>

<style scoped>
.admin-layout { min-height: 100vh; background-color: #f1f5f9; position: relative; overflow-x: hidden; display: flex; }
.main-viewport { flex-grow: 1; z-index: 5; position: relative; padding-bottom: 50px; }
.background-overlay { position: absolute; inset: 0; background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 0.8px, transparent 0.8px); background-size: 32px 32px; opacity: 0.3; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.12; width: 500px; height: 500px; background: #fbbf24; top: -10%; right: -5%; }

.terminal-status-bar { background: rgba(255, 255, 255, 0.6); backdrop-filter: blur(12px); border: 1px solid rgba(255, 255, 255, 0.8); border-radius: 20px; padding: 12px 0; }
.breadcrumb-cyber { font-size: 11px; font-weight: 800; letter-spacing: 1px; color: #64748b; }

.display-title-cyber { font-weight: 800; color: #0f172a; font-size: 32px; letter-spacing: -1px; }
.text-gradient-amber { background: linear-gradient(135deg, #fbbf24, #d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }

.section-subtitle { font-size: 14px; font-weight: 800; color: #475569; letter-spacing: 1px; border-left: 4px solid #fbbf24; padding-left: 15px; }

.test-stats-group { display: flex; gap: 15px; }
.test-count-badge { border-radius: 20px; padding: 10px 25px; display: flex; align-items: center; gap: 12px; min-width: 150px; }
.test-count-badge.active { background: #0f172a; color: white; border: 1px solid #1e293b; box-shadow: 0 4px 15px rgba(15, 23, 42, 0.15); }
.test-count-badge.expired { background: #f1f5f9; color: #64748b; border: 1px solid #e2e8f0; }

.test-count-badge .count { font-size: 24px; font-weight: 900; line-height: 1; }
.test-count-badge.active .count { color: #fbbf24; }
.test-count-badge .label { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: 1px; opacity: 0.8; }

.campaign-card-glass { background: rgba(255, 255, 255, 0.9); border: 1px solid white; border-radius: 30px; box-shadow: 0 10px 30px rgba(0,0,0,0.03); transition: 0.3s; height: 100%; display: flex; flex-direction: column; position: relative; }
.campaign-card-glass:hover:not(.expired) { transform: translateY(-8px); border-color: #fbbf24; }
.campaign-card-glass.upcoming { border-left: 6px solid #6366f1; }
.campaign-card-glass.expired { background: #f8fafc; border: 1px solid #e2e8f0; }

.expired-overlay { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%) rotate(-15deg); font-size: 24px; font-weight: 900; color: rgba(239, 68, 68, 0.1); pointer-events: none; }

.card-header-tech { padding: 20px 25px; display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #f1f5f9; }
.category-tag { font-size: 9px; font-weight: 900; background: #ecfdf5; color: #10b981; padding: 5px 12px; border-radius: 10px; }
.category-tag.bg-slate-200 { background: #e2e8f0; color: #64748b; }

.campaign-title { font-weight: 800; font-size: 18px; margin-bottom: 10px; color: #1e293b; }
.campaign-description { font-size: 13px; color: #64748b; line-height: 1.6; margin-bottom: 20px; flex-grow: 1; }

.campaign-info-grid { display: grid; grid-template-columns: 1fr; gap: 10px; margin-bottom: 25px; background: rgba(248, 250, 252, 0.8); padding: 15px; border-radius: 20px; }
.info-item .label { display: block; font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-bottom: 4px; }
.info-item .value { font-size: 14px; font-weight: 800; color: #1e293b; }

.btn-primary-cyber { width: 100%; background: #0f172a; color: white; border: none; padding: 15px; border-radius: 18px; font-weight: 700; font-size: 14px; transition: 0.3s; }
.btn-primary-cyber:hover { background: #1e293b; transform: scale(1.02); }
.btn-disabled-cyber { width: 100%; background: #cbd5e1; color: #64748b; border: none; padding: 15px; border-radius: 18px; font-weight: 700; font-size: 12px; cursor: not-allowed; }

.empty-state-panel { text-align: center; background: rgba(255,255,255,0.3); border: 1px dashed #cbd5e1; border-radius: 20px; }

.loader-ripple { display: inline-block; position: relative; width: 80px; height: 80px; }
.loader-ripple div { position: absolute; border: 4px solid #fbbf24; opacity: 1; border-radius: 50%; animation: loader-ripple 1s cubic-bezier(0, 0.2, 0.8, 1) infinite; }
.loader-ripple div:nth-child(2) { animation-delay: -0.5s; }
@keyframes loader-ripple { 0% { top: 36px; left: 36px; width: 0; height: 0; opacity: 0; } 4.9% { top: 36px; left: 36px; width: 0; height: 0; opacity: 0; } 5% { top: 36px; left: 36px; width: 0; height: 0; opacity: 1; } 100% { top: 0px; left: 0px; width: 72px; height: 72px; opacity: 0; } }
</style>
