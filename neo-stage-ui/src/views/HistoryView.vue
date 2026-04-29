<template>
  <div class="admin-layout elite-dashboard-root">
    <!-- Fond High-Tech Sublimé -->
    <div class="luxury-bg">
      <div class="tech-grid"></div>
      <div class="aura-sphere sphere-indigo"></div>
      <div class="aura-sphere sphere-amber"></div>
      <div class="grain-overlay"></div>
    </div>

    <AppSidebar />
    
    <div class="main-viewport">
      <AppNavbar />
      
      <main class="container-fluid px-xl-5 pt-5">
        <!-- Header Style "Executive" -->
        <header class="mb-5 animate__animated animate__fadeIn">
          <div class="d-flex align-items-center gap-2 mb-3">
            <div class="live-indicator"></div>
            <span class="top-label">PORTAL ANALYTICS</span>
          </div>
          <h1 class="main-title">Mes <span class="text-amber-gradient">Performances</span></h1>
          <p class="subtitle">Analyse statistique de vos compétences et historique des sessions.</p>
        </header>

        <!-- Loader Futuriste -->
        <div v-if="loading" class="loader-portal">
          <div class="scanner-line"></div>
          <div class="robot-ring"></div>
          <span class="loading-text">CHARGEMENT DES DONNÉES</span>
        </div>

        <div v-else class="content-body">
          <!-- State Vide (Design Épuré) -->
          <div v-if="historyData.length === 0" class="bento-empty-card">
            <div class="empty-visual">
              <i class="fa-solid fa-layer-group"></i>
              <div class="pulse-ring"></div>
            </div>
            <h3>Aucun historique</h3>
            <p class="text-muted">Votre parcours de certification commence ici.</p>
            <router-link to="/dashboard" class="btn-premium-action mt-4">
              DÉMARRER MAINTENANT
            </router-link>
          </div>

          <!-- Liste des Tests (Style Elite) -->
          <div v-else class="row g-4">
            <div v-for="test in historyData" :key="test.evaluationId" class="col-12">
              <div class="history-item-card">
                <!-- Accent décoratif sur le côté -->
                <div class="card-accent" :class="test.score >= 50 ? 'bg-success' : 'bg-danger'"></div>
                
                <div class="row align-items-center g-0">
                  <!-- Section Score : Design "Data-Viz" -->
                  <div class="col-md-2 p-4 text-center border-end-glass">
                    <div class="score-display">
                      <svg class="score-circle" viewBox="0 0 36 36">
                        <path class="circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                        <path class="circle-fill" :style="`stroke-dasharray: ${test.score}, 100`" :class="test.score >= 50 ? 'stroke-success' : 'stroke-danger'" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                      </svg>
                      <div class="score-text">
                        <span class="num">{{ test.score }}</span><span class="per">%</span>
                      </div>
                    </div>
                  </div>
                  
                  <!-- Section Infos : Typographie travaillée -->
                  <div class="col-md-7 px-md-5 py-4">
                    <div class="d-flex align-items-center gap-3 mb-2">
                      <span class="badge-glass">SESSION TERMINÉE</span>
                      <span class="date-text">{{ formatDate(test.dateFin) }}</span>
                    </div>
                    <h3 class="test-name">{{ test.nom }}</h3>
                    <div class="meta-info">
                      <i class="fa-solid fa-shield-halved me-2"></i> Évaluation technique certifiée
                    </div>
                  </div>

                  <!-- Section Action : Bouton Premium -->
                  <div class="col-md-3 p-4 text-end">
                    <router-link :to="`/results/${test.evaluationId}`" class="btn-elite-action">
                      <span>VOIR RAPPORT</span>
                      <div class="icon-box">
                        <i class="fa-solid fa-arrow-right-long"></i>
                      </div>
                    </router-link>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const historyData = ref([]);
const loading = ref(true);

const fetchHistory = async () => {
  try {
    const res = await api.get('/Examen/mon-historique');
    historyData.value = res.data;
  } catch (err) {
    console.error("Erreur historique:", err);
  } finally {
    loading.value = false;
  }
};

const formatDate = (d) => new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', year: 'numeric' });

onMounted(fetchHistory);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@600;800&display=swap');

/* CONFIGURATION GÉNÉRALE */
.admin-layout { 
  display: flex; 
  min-height: 100vh; 
  background-color: #f8fafc; 
  font-family: 'Plus Jakarta Sans', sans-serif; 
}
.main-viewport { flex: 1; position: relative; z-index: 1; }

/* LUXURY BG & TECH GRID */
.luxury-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden; background: #ffffff; }
.tech-grid { 
  position: absolute; inset: 0; 
  background-image: radial-gradient(#e2e8f0 1.2px, transparent 1.2px); 
  background-size: 40px 40px; 
  opacity: 0.4; 
}
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.18; }
.sphere-indigo { width: 800px; height: 800px; background: #6366f1; top: -15%; right: -10%; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; bottom: -10%; left: -5%; }
.grain-overlay { position: absolute; inset: 0; opacity: 0.03; background-image: url("https://grainy-gradients.vercel.app/noise.svg"); }

/* HEADER ELEMENTS */
.top-label { font-size: 0.7rem; font-weight: 800; color: #64748b; letter-spacing: 0.2em; }
.live-indicator { width: 8px; height: 8px; background: #fbbf24; border-radius: 50%; box-shadow: 0 0 10px #fbbf24; animation: pulse 2s infinite; }
.main-title { font-size: 3.5rem; font-weight: 800; color: #0f172a; letter-spacing: -0.04em; }
.text-amber-gradient { background: linear-gradient(135deg, #f59e0b, #d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.subtitle { color: #64748b; font-size: 1.1rem; font-weight: 500; }

/* CARDS DESIGN (ELITE) */
.history-item-card {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(226, 232, 240, 0.7);
  border-radius: 2rem;
  position: relative;
  overflow: hidden;
  transition: all 0.5s cubic-bezier(0.19, 1, 0.22, 1);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -1px rgba(0, 0, 0, 0.03);
}
.history-item-card:hover {
  transform: translateX(12px) scale(1.01);
  background: #ffffff;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.08);
  border-color: #fbbf24;
}

.card-accent { position: absolute; left: 0; top: 0; bottom: 0; width: 6px; }
.border-end-glass { border-right: 1px solid rgba(0,0,0,0.05); }

/* SCORE DISPLAY DATA-VIZ */
.score-display { position: relative; width: 80px; height: 80px; margin: 0 auto; }
.score-circle { transform: rotate(-90deg); }
.circle-bg { fill: none; stroke: #f1f5f9; stroke-width: 3.5; }
.circle-fill { fill: none; stroke-width: 3.5; stroke-linecap: round; transition: stroke-dasharray 1s ease; }
.stroke-success { stroke: #10b981; filter: drop-shadow(0 0 5px rgba(16, 185, 129, 0.3)); }
.stroke-danger { stroke: #ef4444; }
.score-text { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; font-family: 'JetBrains Mono'; flex-direction: row; }
.score-text .num { font-size: 1.5rem; font-weight: 800; color: #0f172a; }
.score-text .per { font-size: 0.7rem; color: #94a3b8; font-weight: 700; margin-left: 1px; }

/* CONTENT DETAILS */
.test-name { font-size: 1.5rem; font-weight: 800; color: #0f172a; letter-spacing: -0.02em; }
.badge-glass { background: #f1f5f9; color: #475569; font-size: 0.65rem; font-weight: 800; padding: 4px 10px; border-radius: 8px; letter-spacing: 0.05em; }
.date-text { font-size: 0.85rem; color: #94a3b8; font-weight: 600; }
.meta-info { font-size: 0.8rem; color: #64748b; font-weight: 500; }

/* ELITE ACTION BUTTON */
.btn-elite-action {
  display: inline-flex; align-items: center; gap: 15px; text-decoration: none;
  background: #0f172a; padding: 8px 8px 8px 24px; border-radius: 100px;
  color: white; font-weight: 700; font-size: 0.75rem; transition: all 0.3s ease;
}
.icon-box {
  width: 40px; height: 40px; background: rgba(255,255,255,0.1); border-radius: 50%;
  display: flex; align-items: center; justify-content: center; transition: 0.3s;
}
.btn-elite-action:hover { background: #fbbf24; color: #0f172a; transform: scale(1.05); }
.btn-elite-action:hover .icon-box { background: #0f172a; color: white; transform: rotate(-45deg); }

/* LOADER & ANIMATIONS */
.loader-portal { display: flex; flex-direction: column; align-items: center; padding: 120px 0; position: relative; }
.robot-ring { width: 60px; height: 60px; border: 3px solid #f1f5f9; border-top-color: #fbbf24; border-radius: 50%; animation: spin 1s infinite linear; }
.loading-text { margin-top: 20px; font-weight: 800; color: #94a3b8; font-size: 0.7rem; letter-spacing: 0.4em; }

@keyframes spin { to { transform: rotate(360deg); } }
@keyframes pulse { 0% { transform: scale(1); opacity: 1; } 50% { transform: scale(1.5); opacity: 0.5; } 100% { transform: scale(1); opacity: 1; } }

@media (max-width: 768px) {
  .border-end-glass { border-right: none; border-bottom: 1px solid rgba(0,0,0,0.05); }
  .history-item-card { margin-left: 0 !important; text-align: center; }
  .btn-elite-action { width: 100%; justify-content: space-between; }
}
</style>