<template>
  <div class="d-flex admin-body">
    <!-- Sidebar -->
    <AppSidebar />
    
    <!-- Grille technique discrète pour le style light -->
    <div class="tech-grid-bg-light"></div>

    <div class="content flex-grow-1 p-lg-5 p-4 position-relative">
      <AppNavbar />

      <main class="mt-4 animate-fade-in">
        <!-- HEADER SECTION -->
        <div class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <nav class="breadcrumb-tiny mb-2">SYSTEM / ANALYTICS / <span class="active">EXPORT_CENTER</span></nav>
            <h2 class="fw-black text-navy display-5">Hub d'Extraction <span class="text-indigo">Automatisé</span></h2>
            <p class="text-muted-pro ls-tight">Préparez vos rapports de performance basés sur le moteur IA EvaluaTech.</p>
          </div>
          <div class="sync-indicator">
            <span class="pulse-dot"></span>
            <span class="live-text text-success">DONNÉES SYNCHRONISÉES</span>
          </div>
        </div>

        <!-- KPI SUMMARY -->
        <div class="row g-4 mb-5">
          <div class="col-md-4" v-for="kpi in kpiData" :key="kpi.title">
            <div :class="['analytics-card-pro', kpi.borderClass]">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                  <span class="card-label-pro uppercase">{{ kpi.title }}</span>
                  <div class="card-value-pro text-navy">{{ kpi.value }}</div>
                </div>
                <div :class="['card-icon-pro', kpi.iconBg]">
                  <i :class="kpi.icon"></i>
                </div>
              </div>
              <div class="card-footer-pro mt-3">
                <span :class="kpi.trendClass"><i :class="kpi.trendIcon"></i> {{ kpi.trend }}</span>
                <span class="text-muted ms-2">vs mois dernier</span>
              </div>
            </div>
          </div>
        </div>

        <!-- EXPORT ENGINE BOXES -->
        <div class="row g-4 mb-5">
          <!-- EXCEL ENGINE -->
          <div class="col-lg-6">
            <div class="premium-panel h-100 export-engine-card">
              <div class="d-flex align-items-center gap-3 mb-4">
                <div class="engine-icon bg-success-light text-success">
                  <i class="fa-solid fa-file-excel"></i>
                </div>
                <div>
                  <h4 class="text-navy fw-bold m-0">Matrice Brute Excel</h4>
                  <span class="tiny-status text-success">FORMAT: .XLSX (MODERNE)</span>
                </div>
              </div>
              
              <div class="preview-mockup-light mb-4">
                <div class="mock-line-light" v-for="i in 3" :key="i" :style="{width: (100 - (i*15)) + '%'}"></div>
              </div>

              <p class="small text-muted mb-4">Inclut les scores pondérés, les temps de réponse millimétrés et l'index de confiance IA par candidat.</p>
              
              <div class="export-options mb-4">
                <div class="form-check form-switch custom-switch">
                  <input class="form-check-input" type="checkbox" checked id="incMetadata">
                  <label class="form-check-label text-muted small fw-bold" for="incMetadata">Inclure métadonnées techniques</label>
                </div>
              </div>

              <button class="btn btn-indigo-glow w-100 py-3 d-flex align-items-center justify-content-center gap-2">
                <i class="fa-solid fa-cloud-arrow-down"></i> GÉNÉRER LA MATRICE XLSX
              </button>
            </div>
          </div>

          <!-- PDF REPORTING -->
          <div class="col-lg-6">
            <div class="premium-panel h-100 export-engine-card">
              <div class="d-flex align-items-center gap-3 mb-4">
                <div class="engine-icon bg-warning-light text-warning">
                  <i class="fa-solid fa-file-pdf"></i>
                </div>
                <div>
                  <h4 class="text-navy fw-bold m-0">Executive Summary PDF</h4>
                  <span class="tiny-status text-warning">IA VISUAL REPORT</span>
                </div>
              </div>

              <div class="chart-preview-box-light mb-4">
                <canvas id="skillChartMini" height="80"></canvas>
              </div>

              <p class="small text-muted mb-4">Rapport graphique haute définition incluant le radar des compétences, les benchmarks sectoriels et les prédictions IA.</p>

              <button class="btn btn-outline-navy w-100 py-3 d-flex align-items-center justify-content-center gap-2">
                <i class="fa-solid fa-wand-magic-sparkles"></i> COMPILER LE BILAN VISUEL
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import Chart from 'chart.js/auto';

const kpiData = ref([
  { title: 'SUCCÈS ANALYSES', value: '1,248', icon: 'fa-solid fa-chart-pie', iconBg: 'bg-info-light', borderClass: 'b-info-light', trend: '+12%', trendClass: 'text-success', trendIcon: 'fa-solid fa-caret-up' },
  { title: 'LATENCE MOYENNE', value: '1.2s', icon: 'fa-solid fa-bolt', iconBg: 'bg-warning-light', borderClass: 'b-warning-light', trend: '-0.3s', trendClass: 'text-success', trendIcon: 'fa-solid fa-caret-down' },
  { title: 'VOLUME DATA', value: '840 GB', icon: 'fa-solid fa-database', iconBg: 'bg-success-light', borderClass: 'b-success-light', trend: 'Système Stable', trendClass: 'text-indigo', trendIcon: 'fa-solid fa-shield-halved' }
]);

onMounted(() => {
  const ctx = document.getElementById('skillChartMini').getContext('2d');
  new Chart(ctx, {
    type: 'bar',
    data: {
      labels: ['L', 'S', 'T', 'M'],
      datasets: [{
        data: [65, 80, 45, 70],
        backgroundColor: ['#6366f1', '#10b981', '#f59e0b', '#3b82f6'],
        borderRadius: 4,
      }]
    },
    options: {
      responsive: true, maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: { x: { display: false }, y: { display: false } }
    }
  });
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800;900&display=swap');

/* --- THEME LIGHT PREMIUM --- */
.admin-body {
  background-color: #ffffff; /* Fond Blanc */
  min-height: 100vh;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #1e293b;
  overflow-x: hidden;
}

.tech-grid-bg-light {
  position: fixed; inset: 0;
  background-image: radial-gradient(circle at 2px 2px, #f1f5f9 1.5px, transparent 0);
  background-size: 32px 32px;
  pointer-events: none;
  z-index: 0;
}

/* TYPO & COLORS */
.text-navy { color: #0f172a !important; }
.text-indigo { color: #6366f1 !important; }
.text-muted-pro { color: #64748b; font-weight: 500; }
.fw-black { font-weight: 900; }

/* BREADCRUMB */
.breadcrumb-tiny {
  font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px;
}
.breadcrumb-tiny .active { color: #0f172a; }

/* INDICATORS */
.sync-indicator {
  display: flex; align-items: center; gap: 10px;
  background: #f8fafc; border: 1px solid #e2e8f0;
  padding: 8px 16px; border-radius: 12px;
}
.live-text { font-size: 9px; font-weight: 800; letter-spacing: 0.5px; }

/* KPI CARDS (LIGHT MODE) */
.analytics-card-pro {
  background: #ffffff;
  padding: 1.5rem; border-radius: 20px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.03);
  transition: transform 0.3s;
}
.analytics-card-pro:hover { transform: translateY(-5px); border-color: #6366f1; }

.card-label-pro { font-size: 10px; color: #94a3b8; font-weight: 800; }
.card-value-pro { font-size: 1.6rem; font-weight: 900; margin-top: 5px; letter-spacing: -1px; }

/* PANELS */
.premium-panel {
  background: #ffffff;
  border: 1px solid #f1f5f9;
  border-radius: 24px; padding: 2rem;
  box-shadow: 0 10px 40px -10px rgba(0, 0, 0, 0.05);
}

.engine-icon {
  width: 52px; height: 52px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center; font-size: 1.5rem;
}

/* PREVIEWS (MOCKUPS) */
.preview-mockup-light {
  background: #f8fafc; height: 90px;
  border-radius: 16px; padding: 25px;
  display: flex; flex-direction: column; gap: 10px;
}
.mock-line-light { height: 7px; background: #cbd5e1; border-radius: 10px; opacity: 0.5; }

.chart-preview-box-light {
  background: #f8fafc; padding: 15px; border-radius: 16px; height: 90px;
}

/* BOUTONS LIGHT */
.btn-indigo-glow {
  background: #6366f1; color: white; border: none; font-weight: 800; border-radius: 14px;
  box-shadow: 0 10px 25px rgba(99, 102, 241, 0.2); transition: 0.3s;
}
.btn-indigo-glow:hover {
  transform: translateY(-2px); box-shadow: 0 15px 30px rgba(99, 102, 241, 0.3);
  filter: brightness(1.1);
}

.btn-outline-navy {
  border: 2px solid #0f172a; background: transparent;
  color: #0f172a; border-radius: 14px; font-weight: 800; transition: 0.3s;
}
.btn-outline-navy:hover {
  background: #0f172a; color: #ffffff;
}

/* STATUS COLORS LIGHT */
.bg-info-light { background: #f0f9ff; color: #0ea5e9; }
.bg-warning-light { background: #fffbeb; color: #f59e0b; }
.bg-success-light { background: #f0fdf4; color: #10b981; }

.b-info-light { border-left: 5px solid #0ea5e9; }
.b-warning-light { border-left: 5px solid #f59e0b; }
.b-success-light { border-left: 5px solid #10b981; }

/* UTILS */
.tiny-status { font-size: 10px; font-weight: 900; letter-spacing: 0.5px; }
.pulse-dot { width: 8px; height: 8px; border-radius: 50%; background: #10b981; animation: pulse 2s infinite; }
@keyframes pulse {
  0% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.4); }
  70% { box-shadow: 0 0 0 8px rgba(16, 185, 129, 0); }
  100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
}

.animate-fade-in { animation: fadeIn 0.6s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(15px); } to { opacity: 1; transform: translateY(0); } }

.custom-switch .form-check-input:checked {
    background-color: #6366f1; border-color: #6366f1;
}
</style>