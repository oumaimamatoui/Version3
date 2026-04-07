<template>
  <div class="admin-dashboard-container">
    <!-- COUCHES D'ARRIÈRE-PLAN DÉCORATIVES (Cohérence avec le style Login) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>
    <div class="tech-grid-subtle"></div>

    <div class="d-flex position-relative z-index-10">
      <AppSidebar />

      <div class="content flex-grow-1">
        <AppNavbar />

        <div class="p-4 pt-0 main-viewport animate__animated animate__fadeIn">
          
          <!-- HEADER AVEC ASSISTANT IA -->
          <div class="d-flex justify-content-between align-items-end mb-5">
            <div class="header-content">
              <div class="badge-tech mb-2">
                <span class="pulse-dot"></span> ANALYSE NEURALE EN COURS
              </div>
              <h1 class="display-title">Performance <span>IA</span></h1>
              <p class="brand-subtitle">RAPPORT DÉTAILLÉ : FULLSTACK .NET & VUE.JS</p>
            </div>
            
            <div class="d-flex gap-3 align-items-center">
              <!-- Robot Mini (Rappel visuel) -->
              <div class="mini-bot-wrapper d-none d-xl-block">
                <svg viewBox="0 0 200 200" width="60" class="mini-robot">
                  <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#e2e8f0" stroke-width="2"/>
                  <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
                  <circle cx="85" cy="81" r="5" fill="#eab308" />
                  <circle cx="115" cy="81" r="5" fill="#eab308" />
                </svg>
              </div>
              <button @click="exportRapport" class="btn-primary-gradient px-4" :disabled="isExporting">
                <span v-if="isExporting" class="spinner-border spinner-border-sm me-2"></span>
                <i v-else class="fa fa-file-pdf me-2"></i>
                {{ isExporting ? 'COMPILATION...' : 'EXPORTER PDF' }}
              </button>
            </div>
          </div>

          <div class="row g-4">
            <!-- SCORE GLOBAL (Style Glassmorphism) -->
            <div class="col-lg-4">
              <div class="glass-surface p-4 h-100 text-center d-flex flex-column justify-content-center border-amber-soft">
                <h6 class="label-heading mb-4">GLOBAL SCORE INDEX</h6>
                <div class="score-neural-circle mx-auto mb-4">
                  <div class="inner-glow"></div>
                  <span class="score-value">82</span>
                  <span class="score-percent">%</span>
                </div>
                <div class="status-pill-success mb-3">
                  <i class="fa fa-medal me-2"></i> NIVEAU : AVANCÉ
                </div>
                <p class="text-slate-500 small">Positionnement : <span class="text-indigo fw-bold">Top 15%</span> du pool candidat.</p>
              </div>
            </div>

            <!-- RADAR CHART -->
            <div class="col-lg-8">
              <div class="glass-surface p-4 h-100">
                <div class="d-flex justify-content-between mb-4">
                  <h6 class="label-heading"><i class="fa fa-project-diagram me-2 text-indigo"></i>MATRIX DE COMPÉTENCES</h6>
                  <span class="text-slate-400 small">Précision IA : 99.8%</span>
                </div>
                <div class="chart-container">
                  <canvas id="radarChart"></canvas>
                </div>
              </div>
            </div>

            <!-- RECOMMANDATIONS IA (Style Dark Tech Indigo) -->
            <div class="col-lg-12">
              <div class="ai-insight-box">
                <div class="cyber-scan-line"></div>
                <div class="row position-relative">
                  <div class="col-md-7 border-end-cyber">
                    <div class="d-flex align-items-center mb-3">
                      <div class="ai-pulse-icon me-3">
                        <i class="fa fa-robot"></i>
                      </div>
                      <h5 class="m-0 fw-bold text-white tracking-tight">INSIGHTS GÉNÉRATIFS</h5>
                    </div>
                    <p class="small text-white-50 italic mb-4">"Analyse comportementale : Gestion des ressources optimale, architecture propre."</p>
                    <div class="d-flex flex-column gap-3">
                      <div class="point-item">
                        <i class="fa fa-bolt text-amber"></i>
                        <span><strong>Force :</strong> Maîtrise avancée du cycle de vie Vue.js.</span>
                      </div>
                      <div class="point-item">
                        <i class="fa fa-microchip text-indigo-light"></i>
                        <span><strong>Axe :</strong> Optimisation des requêtes LINQ (.NET).</span>
                      </div>
                    </div>
                  </div>
                  <div class="col-md-5 ps-md-5 mt-4 mt-md-0">
                    <h6 class="label-heading text-white-50 mb-3">NEURAL ROADMAP</h6>
                    <div class="roadmap-grid">
                      <div class="roadmap-card">
                        <span class="small d-block text-slate-400">Objectif</span>
                        <strong>Senior .NET</strong>
                      </div>
                      <div class="roadmap-card gold-border">
                        <span class="small d-block text-slate-400">Certification</span>
                        <strong>SaaS Security</strong>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- TABLEAU BREAKDOWN (Cyber Style) -->
            <div class="col-12 pb-5">
              <div class="glass-surface p-0 overflow-hidden">
                <div class="p-4 border-bottom border-light">
                  <h6 class="label-heading m-0">DÉTAILS TECHNIQUES PAR MODULE</h6>
                </div>
                <div class="table-responsive">
                  <table class="table cyber-table align-middle m-0">
                    <thead>
                      <tr>
                        <th>THÉMATIQUE</th>
                        <th style="width: 30%">SCORE NEURAL</th>
                        <th>PRÉCISION</th>
                        <th>TEMPS</th>
                        <th>ÉTAT</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="t in themes" :key="t.name">
                        <td class="fw-bold text-slate-700">{{ t.name }}</td>
                        <td>
                          <div class="cyber-progress">
                            <div class="progress-fill" :style="{width: t.score + '%'}"></div>
                          </div>
                        </td>
                        <td><span class="text-indigo fw-800">{{ t.score }}%</span></td>
                        <td class="text-slate-400 font-mono">{{ t.time }}s</td>
                        <td>
                          <span :class="t.score >= 70 ? 'st-tag-success' : 'st-tag-warning'">
                            {{ t.score >= 70 ? 'STABLE' : 'À OPTIMISER' }}
                          </span>
                        </td>
                      </tr>
                    </tbody>
                  </table>
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
import { ref, onMounted } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import Chart from 'chart.js/auto';

const isExporting = ref(false);
const themes = [
  { name: 'Architecture Vue.js 3', score: 92, time: 45 },
  { name: 'Logique .NET 10', score: 65, time: 120 },
  { name: 'SQL & Entity Framework', score: 78, time: 85 },
  { name: 'Sécurité SaaS', score: 85, time: 30 }
];

const exportRapport = () => {
  isExporting.value = true;
  setTimeout(() => {
    isExporting.value = false;
    alert("Téléchargement du rapport de performance IA...");
  }, 2000);
};

onMounted(() => {
  const ctx = document.getElementById('radarChart');
  new Chart(ctx, {
    type: 'radar',
    data: {
      labels: ['Vue.js', '.NET', 'SQL', 'IA Concepts', 'Logique', 'SaaS'],
      datasets: [{
        data: [92, 65, 78, 85, 90, 85],
        fill: true,
        backgroundColor: 'rgba(79, 70, 229, 0.15)',
        borderColor: '#4f46e5',
        borderWidth: 2,
        pointBackgroundColor: '#eab308',
        pointBorderColor: '#fff',
        pointRadius: 4
      }]
    },
    options: {
      maintainAspectRatio: false,
      scales: {
        r: {
          angleLines: { color: 'rgba(148, 163, 184, 0.2)' },
          grid: { color: 'rgba(148, 163, 184, 0.2)' },
          pointLabels: { font: { family: 'Plus Jakarta Sans', size: 11, weight: '700' }, color: '#64748b' },
          ticks: { display: false }
        }
      },
      plugins: { legend: { display: false } }
    }
  });
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&display=swap');

.admin-dashboard-container {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background-color: #f8fafc;
  position: relative;
  overflow-x: hidden;
}

/* --- BACKGROUND ELEMENTS (Same as Login) --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.15; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }

.z-index-10 { z-index: 10; }

/* --- TYPOGRAPHY & TITLES --- */
.display-title { font-weight: 800; color: #0f172a; font-size: 42px; margin: 0; letter-spacing: -2px; }
.display-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 2px; }
.label-heading { font-size: 10px; font-weight: 800; letter-spacing: 1.5px; color: #64748b; text-transform: uppercase; }

/* --- COMPONENTS --- */
.glass-surface {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 24px;
  box-shadow: 0 10px 30px -10px rgba(0,0,0,0.04);
}

.border-amber-soft { border-top: 4px solid #eab308; }

.score-neural-circle {
  width: 150px; height: 150px;
  border-radius: 50%;
  background: white;
  border: 12px solid #f1f5f9;
  border-top-color: #eab308;
  display: flex; align-items: center; justify-content: center;
  position: relative;
  box-shadow: 0 15px 35px rgba(234, 179, 8, 0.1);
}

.score-value { font-size: 3.5rem; font-weight: 800; color: #0f172a; }
.score-percent { font-size: 1.2rem; font-weight: 700; color: #eab308; margin-top: 15px; }

/* --- AI INSIGHT BOX --- */
.ai-insight-box {
  background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 100%);
  border-radius: 24px;
  padding: 40px;
  position: relative;
  overflow: hidden;
  box-shadow: 0 20px 40px rgba(15, 23, 42, 0.2);
}

.cyber-scan-line {
  position: absolute; top: 0; left: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, #eab308, transparent);
  animation: scanLine 4s linear infinite;
}

@keyframes scanLine { 0% { top: 0; } 100% { top: 100%; } }

.ai-pulse-icon {
  width: 50px; height: 50px; background: rgba(234, 179, 8, 0.2);
  color: #eab308; border-radius: 12px; display: flex;
  align-items: center; justify-content: center; font-size: 24px;
  animation: pulseAmber 2s infinite;
}

@keyframes pulseAmber { 0% { box-shadow: 0 0 0 0 rgba(234, 179, 8, 0.4); } 70% { box-shadow: 0 0 0 10px rgba(234, 179, 8, 0); } 100% { box-shadow: 0 0 0 0 rgba(234, 179, 8, 0); } }

.point-item { display: flex; align-items: flex-start; gap: 12px; color: white; font-size: 14px; }
.roadmap-card { background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); padding: 15px; border-radius: 16px; margin-bottom: 10px; }
.gold-border { border-color: rgba(234, 179, 8, 0.3); }

/* --- TABLE --- */
.cyber-table thead th { background: #f8fafc; border: none; color: #94a3b8; padding: 15px 25px; font-size: 10px; font-weight: 800; }
.cyber-table tbody td { padding: 20px 25px; border-bottom: 1px solid #f1f5f9; font-size: 14px; }

.cyber-progress { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; background: linear-gradient(90deg, #4f46e5, #818cf8); border-radius: 10px; }

/* --- TAGS & BUTTONS --- */
.btn-primary-gradient {
  padding: 12px 25px; 
  background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 12px;
  font-weight: 800; font-size: 12px; letter-spacing: 0.5px;
  box-shadow: 0 8px 15px rgba(234, 179, 8, 0.2);
  transition: 0.3s;
}

.btn-primary-gradient:hover { transform: translateY(-3px); box-shadow: 0 12px 20px rgba(234, 179, 8, 0.3); }

.st-tag-success { background: #dcfce7; color: #15803d; padding: 4px 12px; border-radius: 8px; font-size: 10px; font-weight: 800; }
.st-tag-warning { background: #fffbeb; color: #b45309; padding: 4px 12px; border-radius: 8px; font-size: 10px; font-weight: 800; }

.status-pill-success { display: inline-flex; align-items: center; background: #f0fdf4; border: 1px solid #bbf7d0; color: #16a34a; padding: 6px 16px; border-radius: 100px; font-size: 12px; font-weight: 700; }

/* --- MINI BOT ANIMATION --- */
.mini-robot { animation: float 3s ease-in-out infinite; }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-8px); } }

.badge-tech {
  display: inline-flex; align-items: center; gap: 8px; background: white;
  padding: 4px 12px; border-radius: 100px; font-size: 9px; font-weight: 800;
  border: 1px solid #e2e8f0; color: #64748b;
}
.pulse-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; animation: blink 1s infinite; }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }
</style>