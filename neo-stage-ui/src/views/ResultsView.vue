<template>
  <div class="elite-results-root">
    <!-- FOND DYNAMIQUE LUXE -->
    <div class="luxury-bg-wrapper">
      <div class="glow-sphere sphere-primary"></div>
      <div class="glow-sphere sphere-secondary"></div>
      <div class="grain-overlay"></div>
    </div>

    <div class="layout-bridge d-flex">
      <AppSidebar />
      
      <main class="main-viewport flex-grow-1">
        <AppNavbar />

        <!-- ÉTAT DE CHARGEMENT CYBER -->
        <div v-if="isLoading" class="neural-loader">
          <div class="loader-visual"></div>
          <h2 class="loader-title">DÉCRYPTAGE DU PROFIL...</h2>
          <p class="text-muted small">Analyse des vecteurs de compétences en cours</p>
        </div>

        <!-- CONTENU PRINCIPAL -->
        <div v-else class="content-container animate__animated animate__fadeIn p-4">
          
          <!-- HEADER DE PERFORMANCE -->
          <header class="performance-header mb-5">
            <div class="row align-items-end">
              <div class="col-md-8">
                <nav class="cyber-breadcrumb mb-2">
                  <span class="path">DASHBOARD</span>
                  <span class="sep">/</span>
                  <span class="path current">ANALYSE COGNITIVE</span>
                </nav>
                <h1 class="main-title-tech">Diagnostic <span class="text-gradient-amber">Expert</span></h1>
                <p class="session-info">
                  <span class="badge-session">SESSION : {{ sessionId }}</span>
                  <span class="campaign-name ms-3 text-uppercase fw-bold">{{ campaignName }}</span>
                </p>
              </div>
              <div class="col-md-4 text-md-end">
                <button @click="printPage" class="btn-export-premium">
                  <i class="fa-solid fa-file-pdf me-2"></i> GÉNÉRER LE RAPPORT
                </button>
              </div>
            </div>
          </header>

          <div class="row g-4">
            <!-- BENTO 1 : SCORE RADIAL -->
            <div class="col-lg-4">
              <div class="glass-bento score-bento">
                <h6 class="bento-title">SCORE D'APTITUDE</h6>
                <div class="gauge-wrapper">
                  <svg viewBox="0 0 100 100" class="gauge-svg">
                    <circle class="track" cx="50" cy="50" r="42"></circle>
                    <circle class="fill" :class="statusColor" cx="50" cy="50" r="42" :style="gaugeStyle"></circle>
                  </svg>
                  <div class="gauge-content">
                    <span class="score-num" :class="statusText">{{ globalScore }}</span>
                    <span class="score-perc">%</span>
                  </div>
                </div>
                <div class="score-status-pill" :class="statusColor">
                   <i class="fa-solid" :class="globalScore >= 70 ? 'fa-medal' : 'fa-circle-info'"></i>
                   {{ globalScore >= 70 ? 'CANDIDAT CERTIFIABLE' : 'BESOIN DE FORMATION' }}
                </div>
              </div>
            </div>

            <!-- BENTO 2 : MATRICE RADAR -->
            <div class="col-lg-8">
              <div class="glass-bento chart-bento">
                <div class="d-flex justify-content-between mb-4">
                  <h6 class="bento-title">MATRICE DES COMPÉTENCES DÉTAILLÉES</h6>
                  <span class="live-tag"><span class="dot-pulse"></span> SYSTÈME D'ÉVALUATION IA</span>
                </div>
                <div class="chart-container-premium">
                  <canvas id="radarChart"></canvas>
                </div>
              </div>
            </div>

            <!-- BENTO 3 : CONSULTANT IA GÉNÉRATIF -->
            <div class="col-12">
              <div class="ai-master-bento shadow-lg">
                <div class="ai-scan-line"></div>
                
                <div v-if="isAiLoading" class="ai-processing-state p-5 text-center">
                  <div class="spinner-border text-warning mb-3"></div>
                  <p class="text-white font-mono">L'IA ANALYSE VOS RÉPONSES ET GÉNÈRE VOS CONSEILS...</p>
                </div>

                <div v-else class="row g-0">
                  <!-- Colonne Analyse -->
                  <div class="col-lg-7 p-5 ai-main-column">
                    <div class="ai-header mb-4 d-flex align-items-center gap-3">
                      <div class="ai-avatar-glow"><i class="fa-solid fa-robot"></i></div>
                      <h4 class="m-0 text-white fw-bold">Synthèse de votre Consultant IA</h4>
                    </div>
                    
                    <div class="ai-bubble">
                      <p class="ai-text-block">"{{ aiInsights.synthese }}"</p>
                    </div>
                    
                    <div class="row g-3 mt-4">
                      <div class="col-md-6">
                        <div class="insight-box force">
                          <div class="label text-success"><i class="fa-solid fa-circle-check"></i> VOS POINTS FORTS</div>
                          <ul class="point-list">
                            <li v-for="f in aiInsights.forces" :key="f">{{ f }}</li>
                          </ul>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="insight-box faiblesse">
                          <div class="label text-warning"><i class="fa-solid fa-lightbulb"></i> AXES DE CONSEILS</div>
                          <ul class="point-list">
                            <li v-for="a in aiInsights.faibmisses" :key="a">{{ a }}</li>
                          </ul>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- Colonne Roadmap -->
                  <div class="col-lg-5 p-5 roadmap-column">
                    <h6 class="bento-title text-white-50 mb-4">ROADMAP DE PROGRESSION</h6>
                    <div class="roadmap-stepper">
                      <div class="roadmap-step">
                        <div class="step-line"></div>
                        <div class="step-icon-hex"><i class="fa-solid fa-crosshairs"></i></div>
                        <div class="step-content">
                          <label>Objectif immédiat</label>
                          <p>{{ aiInsights.roadmap?.objectif }}</p>
                        </div>
                      </div>
                      <div class="roadmap-step mt-5">
                        <div class="step-icon-hex gold"><i class="fa-solid fa-award"></i></div>
                        <div class="step-content">
                          <label>Spécialisation conseillée</label>
                          <p>{{ aiInsights.roadmap?.certification }}</p>
                        </div>
                      </div>
                    </div>
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
import { ref, onMounted, nextTick, computed } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import api from '@/services/api';
import Chart from 'chart.js/auto';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const route = useRoute();
const isLoading = ref(true);
const isAiLoading = ref(true);
const globalScore = ref(0);
const campaignName = ref('');
const themes = ref([]);
const aiInsights = ref({ synthese: "", forces: [], faibmisses: [], roadmap: {} });

// Computed pour le style du score
const sessionId = computed(() => route.params.id?.substring(0, 13).toUpperCase() || 'SESSION-ID');
const gaugeStyle = computed(() => ({ strokeDashoffset: 264 - (264 * globalScore.value) / 100 }));
const statusColor = computed(() => globalScore.value >= 70 ? 'status-elite' : 'status-standard');
const statusText = computed(() => globalScore.value >= 70 ? 'text-success' : 'text-warning');

const printPage = () => window.print();

const fetchData = async () => {
  const id = route.params.id;
  if (!id) { loadDemo(); return; }

  try {
    const res = await api.get(`/Examen/results/${id}`);
    globalScore.value = Math.round(res.data.scorePourcentage);
    themes.value = res.data.themes;
    campaignName.value = res.data.campaignName;
    isLoading.value = false;

    await nextTick();
    initChart();

    // APPEL IA (FastAPI Python)
    const fd = new FormData();
    fd.append('global_score', globalScore.value);
    fd.append('themes_json', JSON.stringify(themes.value));
    
    const aiRes = await axios.post('http://127.0.0.1:8000/ia/performance-report', fd);
    aiInsights.value = aiRes.data;
  } catch (e) {
    console.error("Erreur IA ou API:", e);
    generateLocalAnalysis(); // Si l'IA Python échoue, on génère une analyse locale
  } finally {
    isAiLoading.value = false;
  }
};

// Analyse de secours si le serveur Python est éteint
const generateLocalAnalysis = () => {
  const bestTheme = [...themes.value].sort((a,b) => b.score - a.score)[0];
  const worstTheme = [...themes.value].sort((a,b) => a.score - b.score)[0];

  aiInsights.value = {
    synthese: `Basé sur vos résultats de ${globalScore.value}%, votre profil montre une dominance en ${bestTheme?.name}. Cependant, des axes d'amélioration sont identifiés en ${worstTheme?.name}.`,
    forces: [`Expertise en ${bestTheme?.name}`, "Rigueur de raisonnement"],
    faibmisses: [`Approfondir ${worstTheme?.name}`, "Optimisation des temps de réponse"],
    roadmap: { 
      objectif: `Renforcer vos acquis en ${worstTheme?.name}`, 
      certification: globalScore.value > 80 ? "Architecte Solutions Senior" : "Développeur Fullstack Confirmé"
    }
  };
};

const initChart = () => {
  const ctx = document.getElementById('radarChart');
  if (!ctx) return;
  new Chart(ctx, {
    type: 'radar',
    data: {
      labels: themes.value.map(t => t.name),
      datasets: [{
        label: 'Votre Score',
        data: themes.value.map(t => t.score),
        backgroundColor: 'rgba(251, 191, 36, 0.2)',
        borderColor: '#fbbf24',
        borderWidth: 3,
        pointBackgroundColor: '#fff',
        pointRadius: 5,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        r: { min: 0, max: 100, ticks: { display: false }, pointLabels: { font: { size: 13, weight: 'bold' } } }
      },
      plugins: { legend: { display: false } }
    }
  });
};

const loadDemo = () => {
  isLoading.value = false;
  globalScore.value = 85;
  campaignName.value = "Campagne de Test Démo";
  themes.value = [{name:'Logique', score:90}, {name:'SQL', score:70}, {name:'C#', score:85}, {name:'Architecture', score:60}];
  setTimeout(() => { initChart(); isAiLoading.value = false; generateLocalAnalysis(); }, 500);
};

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&family=JetBrains+Mono&display=swap');

.elite-results-root { min-height: 100vh; background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; }
.main-viewport { flex: 1; min-width: 0; position: relative; z-index: 10; }

/* --- BACKGROUND --- */
.luxury-bg-wrapper { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.glow-sphere { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.1; }
.sphere-primary { width: 600px; height: 600px; background: #fbbf24; top: -10%; right: -5%; }
.sphere-secondary { width: 500px; height: 500px; background: #6366f1; bottom: -10%; left: -5%; }

/* --- BENTO CARDS --- */
.glass-bento { background: white; border: 1px solid #e2e8f0; border-radius: 2.5rem; padding: 2.5rem; height: 100%; box-shadow: 0 10px 40px rgba(0,0,0,0.03); }
.bento-title { font-size: 0.7rem; font-weight: 800; color: #94a3b8; letter-spacing: 0.15em; text-transform: uppercase; }

/* --- GAUGE --- */
.gauge-wrapper { position: relative; width: 220px; height: 220px; margin: 20px auto; }
.gauge-svg { transform: rotate(-90deg); width: 100%; height: 100%; }
.gauge-svg circle { fill: none; stroke-width: 9; stroke-linecap: round; }
.track { stroke: #f1f5f9; }
.fill { stroke: #fbbf24; stroke-dasharray: 264; transition: 2s cubic-bezier(0.4, 0, 0.2, 1); }
.status-elite { stroke: #10b981; }
.status-standard { stroke: #fbbf24; }

.gauge-content { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; flex-direction: column; }
.score-num { font-size: 4rem; font-weight: 900; letter-spacing: -2px; }

/* --- AI BOX --- */
.ai-master-bento { background: #0f172a; border-radius: 2.5rem; overflow: hidden; position: relative; }
.ai-scan-line { position: absolute; top: 0; left: 0; width: 100%; height: 4px; background: linear-gradient(90deg, transparent, #fbbf24, transparent); animation: scan 4s linear infinite; }
@keyframes scan { 0% { top: 0; } 100% { top: 100%; } }

.ai-avatar-glow { width: 55px; height: 55px; background: rgba(251,191,36,0.2); color: #fbbf24; border-radius: 15px; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; box-shadow: 0 0 20px rgba(251,191,36,0.2); }
.ai-bubble { background: rgba(255,255,255,0.05); padding: 1.5rem; border-radius: 1.5rem; margin-top: 1rem; border: 1px solid rgba(255,255,255,0.1); }
.ai-text-block { font-size: 1.15rem; color: #f1f5f9; line-height: 1.6; font-style: italic; }

.insight-box { background: rgba(255,255,255,0.03); padding: 1.5rem; border-radius: 1.5rem; border: 1px solid rgba(255,255,255,0.05); height: 100%; }
.point-list { list-style: none; padding: 0; margin-top: 1rem; color: #cbd5e1; font-size: 0.95rem; }
.point-list li::before { content: "•"; color: #fbbf24; font-weight: bold; margin-right: 10px; }

/* --- ROADMAP --- */
.roadmap-column { background: rgba(255,255,255,0.02); border-left: 1px solid rgba(255,255,255,0.05); }
.roadmap-step { display: flex; gap: 20px; position: relative; }
.step-icon-hex { width: 45px; height: 45px; background: rgba(251,191,36,0.1); border: 1px solid #fbbf24; border-radius: 12px; display: flex; align-items: center; justify-content: center; color: #fbbf24; z-index: 2; }
.step-icon-hex.gold { background: #fbbf24; color: #0f172a; box-shadow: 0 0 15px rgba(251,191,36,0.4); }
.step-content label { display: block; font-size: 0.65rem; color: #94a3b8; font-weight: 800; text-transform: uppercase; }
.step-content p { color: white; font-weight: 700; font-size: 1.1rem; margin: 0; }

/* --- UI ELEMENTS --- */
.text-gradient-amber { background: linear-gradient(135deg, #fbbf24, #d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.badge-session { background: #f1f5f9; color: #64748b; padding: 5px 12px; border-radius: 8px; font-family: 'JetBrains Mono'; font-size: 0.7rem; font-weight: 700; }
.btn-export-premium { background: #0f172a; color: white; border: none; padding: 12px 25px; border-radius: 14px; font-weight: 800; transition: 0.3s; }
.btn-export-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 20px rgba(0,0,0,0.15); }

.dot-pulse { width: 8px; height: 8px; background: #fbbf24; border-radius: 50%; display: inline-block; margin-right: 8px; animation: pulse 1.5s infinite; }
@keyframes pulse { 0% { opacity: 1; } 50% { opacity: 0.3; } 100% { opacity: 1; } }

.neural-loader { height: 80vh; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.loader-visual { width: 70px; height: 70px; border: 5px solid #f1f5f9; border-top-color: #fbbf24; border-radius: 50%; animation: spin 1s infinite linear; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>