<template>
  <div class="stat-master-root" @mousemove="handleParallax">

    <!-- ══════════════════════════════════════
         BACKGROUND
    ══════════════════════════════════════ -->
    <div class="cyber-engine-bg">
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="glow-orb orb-purple" :style="orbStyle(0.025)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column">
      <AppNavbar />

      <!-- ══════════════════════════════════════
           LOADER
      ══════════════════════════════════════ -->
      <div v-if="loading" class="loader-viewport">
        <div class="spinner-pro-premium"></div>
        <p class="loading-text mt-3">SYNCHRONISATION DES SYSTÈMES ANALYTIQUES...</p>
      </div>

      <!-- ══════════════════════════════════════
           CONTENU PRINCIPAL
      ══════════════════════════════════════ -->
      <main v-else id="capture-zone" class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="p-4 p-lg-5 animate__animated animate__fadeIn">

          <!-- SECTION 1 — EN-TÊTE -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Système</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Analytique Temps Réel</span>
              </div>
              <h2 class="premium-title">Tableau de <span class="gradient-text">Bord Global</span></h2>
              <p class="subtitle">Surveillance intelligente des performances et des certifications.</p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap" data-html2canvas-ignore="true">
              <button @click="fetchStats" class="btn-refresh-pro" :disabled="loading" title="Actualiser">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <button @click="downloadPDF" class="btn-enigma-primary shadow-premium" :disabled="downloading">
                <div class="btn-content">
                  <i class="fa-solid me-2" :class="downloading ? 'fa-spinner fa-spin' : 'fa-file-pdf'"></i>
                  {{ downloading ? 'GÉNÉRATION...' : 'EXPORTER RAPPORT' }}
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- SECTION 2 — KPI CARDS -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="kpi in kpiArray" :key="kpi.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: kpi.bg, color: kpi.color }">
                  <i :class="kpi.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ kpi.value }}</div>
                  <div class="stat-label">{{ kpi.label }}</div>
                </div>
                <div v-if="kpi.trend" class="stat-trend ms-auto" :class="kpi.trend > 0 ? 'trend-up' : 'trend-down'">
                  <i :class="kpi.trend > 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ Math.abs(kpi.trend) }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 3 — CHART + LEADERBOARD -->
          <div class="row g-4 mb-5">

            <!-- GRAPHIQUE -->
            <div class="col-lg-8">
              <div class="enigma-card p-4 p-xl-5 h-100">
                <div class="d-flex justify-content-between align-items-center mb-5 flex-wrap gap-3">
                  <div>
                    <h6 class="card-section-title m-0">Performance par Campagne</h6>
                    <p class="card-section-sub m-0">Moyenne des scores en pourcentage</p>
                  </div>
                  <div class="d-flex align-items-center gap-2">
                    <span class="legend-dot dot-amber"></span>
                    <span class="small text-muted fw-700">Score moyen</span>
                    <div class="badge-tech ms-3">TEMPS RÉEL</div>
                  </div>
                </div>

                <div v-if="stats.chart.length" class="chart-pro-viewport">
                  <div v-for="item in stats.chart" :key="item.name" class="bar-column-group">
                    <div class="bar-score-floating">{{ item.score }}%</div>
                    <div class="bar-track-pro">
                      <div class="bar-fill-pro" :style="{ height: item.score + '%', background: getBarColor(item.score) }"></div>
                    </div>
                    <div class="bar-label-pro">{{ item.name }}</div>
                  </div>
                </div>
                <div v-else class="empty-state-pro py-5 text-center">
                  <i class="fa-solid fa-chart-simple fa-2x text-muted mb-3"></i>
                  <p class="text-muted small fw-700">Données insuffisantes pour l'affichage graphique.</p>
                </div>
              </div>
            </div>

            <!-- LEADERBOARD -->
            <div class="col-lg-4">
              <div class="enigma-card p-4 p-xl-5 h-100">
                <div class="d-flex justify-content-between align-items-center mb-4">
                  <h6 class="card-section-title m-0">Top Candidatures</h6>
                  <div class="badge-tech">CLASSEMENT</div>
                </div>

                <div v-if="stats.leaders.length" class="leaders-stack">
                  <div v-for="(user, i) in stats.leaders" :key="i" class="leader-row-modern">
                    <div class="rank-node" :class="'rank-' + (i + 1)">{{ i + 1 }}</div>
                    <div class="avatar-node">{{ user.name.charAt(0) }}</div>
                    <div class="flex-grow-1 ms-3 min-width-0">
                      <div class="leader-name">{{ user.name }}</div>
                      <div class="leader-test text-truncate">{{ user.test }}</div>
                    </div>
                    <div class="leader-score">{{ user.score }}%</div>
                  </div>
                </div>
                <div v-else class="empty-state-pro py-4 text-center">
                  <i class="fa-solid fa-trophy fa-2x text-muted mb-3"></i>
                  <p class="text-muted small fw-700">Aucun leader identifié.</p>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 4 — ANALYTICS OVERVIEW -->
          <div class="row g-4 mb-5">
            <div class="col-lg-8">
              <div class="enigma-card p-4 p-xl-5">
                <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
                  <h6 class="card-section-title m-0">Activité des Sessions (7j)</h6>
                  <div class="d-flex gap-3 align-items-center">
                    <span class="legend-dot dot-amber"></span><span class="small text-muted fw-700">Déploiements</span>
                    <span class="legend-dot dot-indigo ms-2"></span><span class="small text-muted fw-700">Candidats</span>
                  </div>
                </div>
                <div class="bar-chart-v2">
                  <div v-for="(bar, i) in activityData" :key="i" class="bar-col">
                    <div class="bar-wrap">
                      <div class="bar-fill bar-amber" :style="{ height: bar.deploy + '%' }"></div>
                      <div class="bar-fill bar-indigo" :style="{ height: bar.cand + '%' }"></div>
                    </div>
                    <span class="bar-label-sm">{{ bar.label }}</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-lg-4">
              <div class="enigma-card p-4 p-xl-5 h-100">
                <h6 class="card-section-title mb-4">Distribution Statuts</h6>
                <div class="donut-chart-container">
                  <svg viewBox="0 0 120 120" width="110" style="flex-shrink:0">
                    <circle v-for="(seg, i) in donutSegments" :key="i"
                      cx="60" cy="60" r="45"
                      :stroke="seg.color" stroke-width="18" fill="none"
                      :stroke-dasharray="`${seg.dash} ${283 - seg.dash}`"
                      :stroke-dashoffset="seg.offset"
                      style="transition: stroke-dasharray 0.6s ease"/>
                    <text x="60" y="64" text-anchor="middle" class="donut-center-text">{{ stats.chart.length }}</text>
                    <text x="60" y="75" text-anchor="middle" class="donut-sub-text">Sessions</text>
                  </svg>
                  <div class="donut-legend">
                    <div v-for="seg in donutSegments" :key="seg.label" class="donut-legend-item">
                      <span class="legend-dot-sm" :style="{ background: seg.color }"></span>
                      <span class="small fw-700">{{ seg.label }}</span>
                      <span class="ms-auto fw-900 small">{{ seg.count }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- SECTION 5 — TABLE RÉSULTATS -->
          <div class="enigma-card overflow-hidden">
            <div class="table-head-row">
              <div>
                <h6 class="card-section-title m-0">Dernières Évaluations Certifiées</h6>
                <p class="card-section-sub m-0">Résultats triés par date décroissante</p>
              </div>
              <button class="btn-outline-pro" @click="$router.push('/candidates')">
                TOUS LES CANDIDATS <i class="fa-solid fa-arrow-right-long ms-2"></i>
              </button>
            </div>

            <div class="table-responsive">
              <table class="table-pro m-0">
                <thead>
                  <tr>
                    <th>CANDIDAT</th>
                    <th>CAMPAGNE / TEST</th>
                    <th>DATE</th>
                    <th>SCORE</th>
                    <th>INTÉGRITÉ</th>
                    <th class="text-center">ACTION</th>
                  </tr>
                </thead>
                <tbody v-if="stats.recentResults.length">
                  <tr v-for="res in stats.recentResults" :key="res.id">
                    <td>
                      <div class="d-flex align-items-center gap-3">
                        <div class="table-avatar">{{ res.candidateName.charAt(0) }}</div>
                        <span class="fw-800 text-dark-lux">{{ res.candidateName }}</span>
                      </div>
                    </td>
                    <td><span class="text-muted fw-600 small">{{ res.testName }}</span></td>
                    <td><span class="text-mono">{{ res.date }}</span></td>
                    <td>
                      <div class="score-badge" :class="getScoreClass(res.score)">{{ res.score }}%</div>
                    </td>
                    <td>
                      <div class="integrity-badge" :class="res.integrity < 80 ? 'risk' : 'safe'">
                        <i class="fa-solid fa-shield-halved me-1"></i>{{ res.integrity }}%
                      </div>
                    </td>
                    <td class="text-center">
                      <button class="btn-action-view" @click="$router.push(`/details-candidat/${res.candidateId}`)" title="Voir le détail">
                        <i class="fa-solid fa-eye"></i>
                      </button>
                    </td>
                  </tr>
                </tbody>
                <tbody v-else>
                  <tr>
                    <td colspan="6">
                      <div class="py-5 text-center">
                        <i class="fa-solid fa-inbox fa-2x text-muted mb-3"></i>
                        <p class="text-muted small fw-700 m-0">Aucun résultat récent à afficher.</p>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed, reactive } from 'vue';
import axios from 'axios';
import jsPDF from 'jspdf';
import html2canvas from 'html2canvas';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

/* ─── STATE ────────────────────────────────────────────────── */
const loading    = ref(true);
const downloading = ref(false);
const mousePos   = reactive({ x: 0, y: 0 });
const stats      = ref({ kpis: {}, chart: [], leaders: [], recentResults: [] });

const activityData = ref(
  ['L','M','M','J','V','S','D'].map(d => ({
    label: d,
    deploy: Math.floor(Math.random() * 80 + 20),
    cand:   Math.floor(Math.random() * 60 + 10),
  }))
);

/* ─── KPI ──────────────────────────────────────────────────── */
const kpiArray = computed(() => [
  { label: 'Certifications',  value: stats.value.kpis.totalTests  || 0,           icon: 'fa-solid fa-certificate',  color: '#f59e0b', bg: '#fffbeb', trend:  8 },
  { label: 'Moyenne Globale', value: (stats.value.kpis.moyenne    || 0) + '%',     icon: 'fa-solid fa-chart-line',   color: '#3b82f6', bg: '#eff6ff', trend:  3 },
  { label: 'Analyses IA',     value: stats.value.kpis.iaProcessed || 0,            icon: 'fa-solid fa-robot',        color: '#8b5cf6', bg: '#f5f3ff', trend:  12 },
  { label: 'Taux d\'Échec',   value: (stats.value.kpis.tauxEchec  || 0) + '%',     icon: 'fa-solid fa-user-xmark',   color: '#f43f5e', bg: '#fff1f2', trend: -5 },
]);

/* ─── DONUT ────────────────────────────────────────────────── */
const donutSegments = computed(() => {
  const colors  = ['#f59e0b', '#6366f1', '#10b981'];
  const labels  = ['Planifiées', 'Actives', 'Terminées'];
  const total   = Math.max(stats.value.chart.length, 1);
  const circ    = 283;
  const grouped = { 0: Math.ceil(total * 0.3), 1: Math.ceil(total * 0.5), 2: Math.ceil(total * 0.2) };
  let cumulative = 0;
  return Object.entries(grouped).map(([s, count], i) => {
    const dash   = (count / total) * circ;
    const offset = circ / 4 - cumulative;
    cumulative  += dash;
    return { label: labels[s], count, color: colors[i % colors.length], dash, offset };
  });
});

/* ─── HELPERS ──────────────────────────────────────────────── */
const getBarColor   = (s) => s >= 75 ? '#10b981' : s >= 50 ? '#f59e0b' : '#f43f5e';
const getScoreClass = (s) => s >= 75 ? 'score-high' : s >= 50 ? 'score-mid' : 'score-low';

/* ─── API ──────────────────────────────────────────────────── */
const fetchStats = async () => {
  loading.value = true;
  try {
    const token = localStorage.getItem('token');
    const res = await axios.get('http://localhost:5172/api/Dashboard/global-stats', {
      headers: { Authorization: `Bearer ${token}` }
    });
    stats.value = res.data;
  } catch (err) {
    console.error('Dashboard API Error:', err);
  } finally {
    setTimeout(() => { loading.value = false; }, 600);
  }
};

/* ─── PDF ──────────────────────────────────────────────────── */
const downloadPDF = async () => {
  downloading.value = true;
  try {
    const element = document.getElementById('capture-zone');
    const canvas  = await html2canvas(element, { scale: 2, useCORS: true, backgroundColor: '#f8fafc' });
    const imgData = canvas.toDataURL('image/png');
    const pdf     = new jsPDF('p', 'mm', 'a4');
    const pdfW    = pdf.internal.pageSize.getWidth();
    pdf.addImage(imgData, 'PNG', 0, 0, pdfW, (canvas.height * pdfW) / canvas.width);
    pdf.save(`EvaluaTech_Rapport_${new Date().getTime()}.pdf`);
  } catch (err) {
    console.error('PDF Error:', err);
  } finally {
    downloading.value = false;
  }
};

/* ─── PARALLAX ─────────────────────────────────────────────── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(fetchStats);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&family=JetBrains+Mono:wght@600;800&display=swap');

/* ══════════════════════════════════
   BASE
══════════════════════════════════ */
.stat-master-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: #f8fafc;
  color: #0f172a;
  display: flex;
  position: relative;
  overflow-x: hidden;
}

/* ══════════════════════════════════
   BACKGROUND
══════════════════════════════════ */
.cyber-engine-bg {
  position: fixed; inset: 0; z-index: 0; pointer-events: none;
}
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.18;
}
.glow-orb {
  position: absolute; filter: blur(130px); opacity: 0.13;
  border-radius: 50%; transition: transform 0.3s ease-out;
}
.orb-amber  { width: 700px; height: 700px; background: #f59e0b; top: -200px; right: -150px; }
.orb-blue   { width: 500px; height: 500px; background: #6366f1; bottom: -200px; left: -100px; }
.orb-purple { width: 400px; height: 400px; background: #8b5cf6; top: 40%; left: 30%; opacity: 0.06; }

/* ══════════════════════════════════
   LAYOUT
══════════════════════════════════ */
.main-orchestrator { z-index: 5; }
.canvas-engine     { height: calc(100vh - 64px); }

/* ══════════════════════════════════
   EN-TÊTE
══════════════════════════════════ */
.premium-title {
  font-weight: 900; font-size: 2.2rem; letter-spacing: -1.5px; margin: 0;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle { color: #64748b; font-size: 14px; margin-top: 6px; margin-bottom: 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root:hover { color: #f59e0b; cursor: pointer; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ══════════════════════════════════
   BOUTONS
══════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  font-size: 13px; position: relative; overflow: hidden;
  cursor: pointer; font-family: inherit; transition: transform 0.2s;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2; display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.45; cursor: not-allowed; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) {
  background: #f8fafc; border-color: #f59e0b; color: #f59e0b;
  transform: rotate(180deg) scale(1.1);
}

.btn-outline-pro {
  background: white; color: #0f172a; border: 1.5px solid #eef2f6;
  padding: 10px 20px; border-radius: 14px; font-weight: 800;
  font-size: 0.78rem; cursor: pointer; transition: 0.2s; font-family: inherit;
  letter-spacing: 0.5px;
}
.btn-outline-pro:hover { border-color: #0f172a; }

/* ══════════════════════════════════
   LOADER
══════════════════════════════════ */
.loader-viewport {
  flex: 1; display: flex; flex-direction: column;
  align-items: center; justify-content: center; gap: 20px; z-index: 5;
}
.spinner-pro-premium {
  width: 54px; height: 54px; border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b; border-radius: 50%;
  animation: spin 1s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.loading-text { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }

/* ══════════════════════════════════
   KPI CARDS
══════════════════════════════════ */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6;
  transition: 0.3s cubic-bezier(0.4,0,0.2,1);
  box-shadow: 0 2px 8px rgba(0,0,0,0.02);
}
.stat-card-premium:hover { transform: translateY(-5px); box-shadow: 0 16px 40px rgba(0,0,0,0.07); }
.stat-icon-wrapper {
  width: 60px; height: 60px; border-radius: 18px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.stat-details  { margin-left: 16px; }
.stat-value    { font-family: 'JetBrains Mono', monospace; font-size: 1.9rem; font-weight: 800; line-height: 1; color: #0f172a; }
.stat-label    { font-size: 0.65rem; font-weight: 800; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; margin-top: 5px; }
.stat-trend    { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up      { color: #10b981; }
.trend-down    { color: #f43f5e; }

/* ══════════════════════════════════
   ENIGMA CARDS
══════════════════════════════════ */
.enigma-card {
  background: white; border-radius: 32px;
  border: 1px solid #eef2f6;
  box-shadow: 0 2px 8px rgba(0,0,0,0.02);
  transition: 0.3s;
}
.enigma-card:hover { box-shadow: 0 12px 32px rgba(0,0,0,0.05); }

.card-section-title {
  font-size: 1rem; font-weight: 900; color: #0f172a; letter-spacing: -0.3px;
}
.card-section-sub { font-size: 0.72rem; color: #94a3b8; font-weight: 600; margin-top: 3px; }

.badge-tech {
  background: #f8fafc; border: 1px solid #e2e8f0;
  padding: 5px 12px; border-radius: 10px;
  font-size: 9px; font-weight: 900; color: #64748b; letter-spacing: 1.5px;
}

/* ══════════════════════════════════
   GRAPHIQUE PRINCIPAL
══════════════════════════════════ */
.chart-pro-viewport {
  height: 280px; display: flex; align-items: flex-end;
  gap: 24px; padding: 10px 0;
}
.bar-column-group { flex: 1; display: flex; flex-direction: column; align-items: center; }
.bar-track-pro {
  width: 44px; height: 200px; background: #f8fafc;
  border-radius: 100px; position: relative; overflow: hidden;
  border: 1px solid #f1f5f9;
}
.bar-fill-pro {
  width: 100%; position: absolute; bottom: 0;
  border-radius: 100px;
  transition: height 1.2s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.bar-score-floating {
  font-family: 'JetBrains Mono', monospace; font-weight: 800;
  font-size: 11px; margin-bottom: 10px; color: #0f172a;
}
.bar-label-pro {
  font-size: 9.5px; font-weight: 800; color: #94a3b8;
  margin-top: 12px; text-transform: uppercase; letter-spacing: 0.5px;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
  max-width: 60px; text-align: center;
}
.legend-dot   { width: 8px; height: 8px; border-radius: 50%; display: inline-block; }
.dot-amber    { background: #f59e0b; }
.dot-indigo   { background: #6366f1; }

/* ══════════════════════════════════
   LEADERBOARD
══════════════════════════════════ */
.leaders-stack { display: flex; flex-direction: column; gap: 12px; }
.leader-row-modern {
  display: flex; align-items: center; padding: 16px 18px;
  background: #f8fafc; border-radius: 20px;
  border: 1.5px solid #f1f5f9; transition: 0.25s;
}
.leader-row-modern:hover { border-color: #fbbf24; background: #fffdf7; transform: translateX(4px); }
.rank-node    { width: 28px; font-weight: 900; font-size: 1.1rem; color: #cbd5e1; flex-shrink: 0; }
.rank-1       { color: #f59e0b; }
.rank-2       { color: #94a3b8; }
.rank-3       { color: #d97706; }
.avatar-node  {
  width: 42px; height: 42px; background: #0f172a; color: #fbbf24;
  border-radius: 14px; display: flex; align-items: center;
  justify-content: center; font-weight: 900; font-size: 1rem; flex-shrink: 0;
}
.leader-name { font-size: 0.88rem; font-weight: 800; color: #0f172a; }
.leader-test { font-size: 0.7rem; color: #94a3b8; font-weight: 600; }
.leader-score {
  font-family: 'JetBrains Mono', monospace; font-weight: 800;
  color: #f59e0b; font-size: 1.1rem; flex-shrink: 0;
}
.min-width-0 { min-width: 0; }

/* ══════════════════════════════════
   ANALYTICS MINI-CHART
══════════════════════════════════ */
.bar-chart-v2 { display: flex; align-items: flex-end; gap: 8px; height: 120px; }
.bar-col  { display: flex; flex-direction: column; align-items: center; gap: 4px; flex: 1; }
.bar-wrap { display: flex; gap: 3px; align-items: flex-end; height: 100%; width: 100%; justify-content: center; }
.bar-fill { width: 10px; border-radius: 6px 6px 0 0; transition: height 0.8s ease; min-height: 4px; }
.bar-amber  { background: #f59e0b; }
.bar-indigo { background: #6366f1; }
.bar-label-sm { font-size: 0.6rem; font-weight: 800; color: #94a3b8; }

/* ══════════════════════════════════
   DONUT
══════════════════════════════════ */
.donut-chart-container { display: flex; align-items: center; gap: 20px; }
.donut-legend { display: flex; flex-direction: column; gap: 10px; flex: 1; }
.donut-legend-item { display: flex; align-items: center; gap: 8px; }
.legend-dot-sm { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.donut-center-text { font-size: 22px; font-weight: 900; fill: #0f172a; font-family: 'JetBrains Mono', monospace; }
.donut-sub-text { font-size: 8px; fill: #94a3b8; font-weight: 700; }

/* ══════════════════════════════════
   TABLE
══════════════════════════════════ */
.table-head-row {
  padding: 28px 32px;
  display: flex; justify-content: space-between;
  align-items: center; flex-wrap: wrap; gap: 16px;
  border-bottom: 1px solid #eef2f6;
}
.table-pro { width: 100%; border-collapse: collapse; }
.table-pro thead th {
  background: #f8fafc; color: #94a3b8;
  font-size: 10px; font-weight: 900; padding: 16px 32px;
  border: none; letter-spacing: 1.5px; white-space: nowrap;
}
.table-pro tbody td {
  padding: 20px 32px; vertical-align: middle;
  border-bottom: 1px solid #f8fafc;
}
.table-pro tbody tr { transition: background 0.15s; }
.table-pro tbody tr:hover td { background: #fafbfc; }
.table-pro tbody tr:last-child td { border-bottom: none; }

.table-avatar {
  width: 36px; height: 36px; border-radius: 12px;
  background: #eef2f6; color: #64748b;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 13px; flex-shrink: 0;
}
.text-dark-lux { color: #0f172a; font-weight: 800; }
.text-mono     { font-family: 'JetBrains Mono', monospace; font-size: 12px; font-weight: 600; color: #94a3b8; }

.score-badge {
  display: inline-block; font-family: 'JetBrains Mono', monospace;
  font-weight: 800; font-size: 13px;
  padding: 5px 12px; border-radius: 10px;
}
.score-high { background: #ecfdf5; color: #059669; }
.score-mid  { background: #fffbeb; color: #d97706; }
.score-low  { background: #fff1f2; color: #e11d48; }

.integrity-badge { font-size: 11px; font-weight: 800; display: inline-flex; align-items: center; }
.integrity-badge.safe { color: #10b981; }
.integrity-badge.risk { color: #f43f5e; }

.btn-action-view {
  width: 38px; height: 38px; border-radius: 12px;
  border: 1.5px solid #eef2f6; background: white;
  color: #64748b; cursor: pointer; transition: 0.25s;
  display: inline-flex; align-items: center; justify-content: center;
  font-size: 13px;
}
.btn-action-view:hover { background: #0f172a; color: #fbbf24; border-color: #0f172a; transform: rotate(12deg) scale(1.08); }

/* Empty states */
.empty-state-pro {
  background: #f8fafc; border-radius: 20px;
}

/* ══════════════════════════════════
   DARK MODE
══════════════════════════════════ */
[data-theme="dark"] .stat-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine    { background: #0d1117; }
[data-theme="dark"] .premium-title, [data-theme="dark"] .card-section-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

[data-theme="dark"] .stat-card-premium { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .stat-value  { color: #f0f6fc; }

[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.06); }

[data-theme="dark"] .bar-track-pro { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .bar-score-floating { color: #f0f6fc; }

[data-theme="dark"] .leader-row-modern { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .leader-row-modern:hover { border-color: #d97706; background: rgba(251,191,36,0.05); }
[data-theme="dark"] .leader-name { color: #f0f6fc; }

[data-theme="dark"] .donut-center-text { fill: #f0f6fc; }

[data-theme="dark"] .table-head-row  { border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .table-pro thead th { background: #161b22; }
[data-theme="dark"] .table-pro tbody td { border-color: rgba(255,255,255,0.04); }
[data-theme="dark"] .table-pro tbody tr:hover td { background: rgba(255,255,255,0.02); }
[data-theme="dark"] .text-dark-lux { color: #f0f6fc; }
[data-theme="dark"] .table-avatar  { background: rgba(255,255,255,0.08); color: #94a3b8; }
[data-theme="dark"] .btn-action-view { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }

[data-theme="dark"] .badge-tech { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }

/* ══════════════════════════════════
   RESPONSIVE
══════════════════════════════════ */
@media (max-width: 768px) {
  .premium-title { font-size: 1.7rem; }
  .chart-pro-viewport { gap: 12px; }
  .bar-track-pro { width: 32px; height: 160px; }
  .table-pro thead th, .table-pro tbody td { padding: 14px 16px; }
  .table-head-row { padding: 20px; }
}
</style>