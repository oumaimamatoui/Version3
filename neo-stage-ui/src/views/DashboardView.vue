<template>
  <div class="aura-dashboard-final">
    <!-- Iconographie Pro -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <!-- Fond Immersif Signature (Optimisé pour la profondeur) -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
      <div class="vignette-overlay"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport">
      <AppNavbar />

      <div class="container-fluid px-xl-5 pt-4 content-z">
        
        <!-- 1. NAVIGATION TERMINAL (Look Precision Tech) -->
        <header class="terminal-container mb-4 animate__animated animate__fadeInDown">
          <div class="d-flex justify-content-between align-items-center px-4 w-100">
            <div class="terminal-path">
              <span class="path-prefix">SYSTEM_OS</span>
              <span class="path-sep"></span>
              <span class="path-current">{{ authStore.role?.toUpperCase() }}</span>
            </div>
            <div class="terminal-metrics d-none d-md-flex align-items-center">
              <div class="metric-pill">
                <span class="live-indicator"></span>
                <span class="metric-label">LATENCY:</span>
                <span class="metric-value">24MS</span>
              </div>
              <div class="ms-3 user-badge-premium">
                <i class="fa-solid fa-circle-user me-2"></i>
                {{ authStore.user?.name.split(' ')[0] }}
              </div>
            </div>
          </div>
        </header>

        <!-- 2. HERO BENTO : AI INTELLIGENCE (Look Signature) -->
        <div class="hero-signature-card mb-5 animate__animated animate__fadeIn">
          <div class="card-glass-reflection"></div>
          <div class="scanner-sweep"></div>
          <div class="row align-items-center g-0">
            <div class="col-lg-8 p-5 hero-text-content">
              <div class="premium-tag mb-3">
                <i class="fa-solid fa-wand-magic-sparkles me-2"></i> COGNITIVE ENGINE ACTIVE
              </div>
              <h1 class="hero-display-title">
                Ravi de vous revoir, <br>
                <span class="text-gradient-gold">{{ authStore.user?.name || 'Expert' }}</span>
              </h1>
              <p class="hero-lead">Analyse systémique terminée. Votre périmètre technique est à jour.</p>
              
              <div class="ia-insight-module mt-4">
                <div class="ia-orb-container">
                  <div class="ia-orb-ring"></div>
                  <div class="ia-orb-core"></div>
                  <i class="fa-solid fa-brain"></i>
                </div>
                <div class="ia-data-wrap">
                  <div class="ia-header-row">
                    <span class="ia-title">AURA_ANALYST v5.0</span>
                    <span class="ia-status">SYNCHRONIZED</span>
                  </div>
                  <p v-if="loading" class="shimmer-text"></p>
                  <p v-else class="ia-message">{{ dashboardData.insight || 'Prêt pour l\'extraction des données...' }}</p>
                </div>
              </div>
            </div>

            <!-- ROBOT STATION LUXE -->
            <div class="col-lg-4 d-none d-lg-flex justify-content-center">
              <div class="bot-chamber">
                <div class="chamber-rings">
                  <div class="ring r1"></div>
                  <div class="ring r2"></div>
                  <div class="ring r3"></div>
                </div>
                <svg class="bot-svg-elite" viewBox="0 0 200 200">
                  <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping" />
                  <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="0.5"/>
                  <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
                  <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                  <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                </svg>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. KPI GRID (High-Contrast Bento) -->
        <div class="row g-4 mb-5">
          <div v-for="(stat, i) in kpiCards" :key="i" class="col-xl-3 col-md-6">
            <div class="luxury-bento-card" :style="{'--accent-color': stat.color}">
              <div class="card-glow"></div>
              <div class="d-flex justify-content-between align-items-start">
                <div class="bento-icon-wrapper" :style="{background: stat.bg, color: stat.color}">
                  <i :class="stat.icon"></i>
                </div>
                <div class="bento-trend-tag">
                  <i class="fa-solid fa-arrow-trend-up"></i> +{{ 10 + i }}%
                </div>
              </div>
              <div class="bento-body mt-4">
                <h2 class="bento-value">{{ loading ? '---' : (stat.value || '0') }}</h2>
                <span class="bento-label">{{ stat.label }}</span>
              </div>
              <div class="bento-footer-progress">
                <div class="track"></div>
                <div class="fill" :style="{width: '65%', background: stat.color}"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 4. PERFORMANCE & LOGS -->
        <div class="row g-4 pb-5">
          <div class="col-lg-8">
            <div class="panel-luxe p-4">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <h5 class="fw-black text-slate-800 m-0">Performance_Matrix</h5>
                <div class="panel-pill-tabs">
                  <button class="tab-btn active">REALTIME</button>
                  <button class="tab-btn">HISTORY</button>
                </div>
              </div>
              <div class="chart-container-luxe">
                <canvas id="auraMainChart"></canvas>
              </div>
            </div>
          </div>
          
          <div class="col-lg-4">
            <div class="panel-luxe p-4 h-100">
              <h5 class="fw-black text-slate-800 mb-4">Activity_Logs</h5>
              <div class="activity-feed-luxe">
                <div v-for="act in dashboardData.recentActivities" :key="act.id" class="activity-row-pill">
                  <div class="row-indicator" :style="{background: act.color}"></div>
                  <div class="row-content">
                    <span class="user-name">{{ act.user }}</span>
                    <span class="action-desc">{{ act.action }}</span>
                    <div class="campagne-chip" :style="{color: act.color, borderColor: act.color}">
                      {{ act.campagne }}
                    </div>
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
import { ref, onMounted, computed, nextTick } from 'vue';
import { useAuthStore } from '@/stores/auth';
import Chart from 'chart.js/auto';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const authStore = useAuthStore();
const loading = ref(true);
const dashboardData = ref({ kpis: {}, recentActivities: [], chartData: [], insight: "" });

const kpiCards = computed(() => [
  { label: 'TALENTS_TOTAL', value: dashboardData.value.kpis.talentsActifs, icon: 'fa-solid fa-user-group', color: '#fbbf24', bg: '#fef3c7' },
  { label: 'SUCCESS_INDEX', value: dashboardData.value.kpis.tauxReussite, icon: 'fa-solid fa-circle-check', color: '#10b981', bg: '#ecfdf5' },
  { label: 'ACTIVE_SESSIONS', value: dashboardData.value.kpis.testsEnCours, icon: 'fa-solid fa-bolt-lightning', color: '#3b82f6', bg: '#eff6ff' },
  { label: 'AURA_CORE_SCORE', value: dashboardData.value.kpis.iaScore, icon: 'fa-solid fa-brain', color: '#8b5cf6', bg: '#f5f3ff' },
]);

const initChart = (data) => {
  const canvas = document.getElementById('auraMainChart');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  
  const grad = ctx.createLinearGradient(0, 0, 0, 350);
  grad.addColorStop(0, 'rgba(251, 191, 36, 0.2)');
  grad.addColorStop(1, 'rgba(251, 191, 36, 0)');

  new Chart(ctx, {
    type: 'line',
    data: {
      labels: data.map(d => d.day),
      datasets: [{
        data: data.map(d => d.count),
        borderColor: '#fbbf24',
        borderWidth: 4,
        tension: 0.45,
        fill: true,
        backgroundColor: grad,
        pointRadius: 0,
        pointHoverRadius: 8,
        pointHoverBackgroundColor: '#fbbf24',
        pointHoverBorderColor: '#fff',
        pointHoverBorderWidth: 3,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        x: { grid: { display: false }, ticks: { color: '#94a3b8', font: { weight: '700', size: 10 } } },
        y: { display: false }
      }
    }
  });
};

onMounted(async () => {
  try {
    const res = await api.get('/Analytics/overview');
    dashboardData.value = res.data;
    loading.value = false;
    await nextTick();
    if (res.data.chartData) initChart(res.data.chartData);
  } catch (err) {
    console.error("Critical Dashboard Failure", err);
    loading.value = false;
  }
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@400;700&display=swap');

/* --- LAYOUT --- */
.aura-dashboard-final {
  min-height: 100vh; background: #fdfdfd;
  font-family: 'Plus Jakarta Sans', sans-serif;
  display: flex; position: relative; overflow-x: hidden;
}
.main-viewport { flex: 1; position: relative; z-index: 10; }
.content-z { position: relative; z-index: 20; }

/* --- LUXURY BACKGROUND --- */
.luxury-bg { position: fixed; inset: 0; z-index: 0; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.15; animation: orbit 25s infinite linear; }
.sphere-amber { width: 700px; height: 700px; background: #fbbf24; top: -15%; left: -10%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 35%; right: 15%; opacity: 0.1; }
.vignette-overlay { position: absolute; inset: 0; background: radial-gradient(circle, transparent 50%, rgba(255,255,255,0.8) 100%); }
.mesh-grain { position: absolute; inset: 0; opacity: 0.02; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

/* --- TERMINAL HEADER --- */
.terminal-container {
  background: rgba(255, 255, 255, 0.7); backdrop-filter: blur(20px);
  border-radius: 20px; border: 1px solid rgba(255,255,255,0.8);
  padding: 12px 0; box-shadow: 0 4px 20px rgba(0,0,0,0.02);
}
.terminal-path { font-family: 'JetBrains Mono', monospace; font-size: 11px; font-weight: 700; color: #94a3b8; display: flex; align-items: center; }
.path-sep { width: 20px; height: 1px; background: #e2e8f0; margin: 0 12px; }
.path-current { color: #0f172a; }

.metric-pill { background: #f1f5f9; padding: 6px 14px; border-radius: 100px; font-size: 9px; font-weight: 800; display: flex; align-items: center; gap: 8px; color: #64748b; }
.live-indicator { width: 6px; height: 6px; background: #10b981; border-radius: 50%; box-shadow: 0 0 10px #10b981; }
.user-badge-premium { background: #0f172a; color: white; padding: 6px 16px; border-radius: 100px; font-size: 10px; font-weight: 700; }

/* --- HERO SIGNATURE --- */
.hero-signature-card {
  background: white; border-radius: 40px; border: 1px solid #f1f5f9;
  box-shadow: 0 40px 80px rgba(0,0,0,0.03); position: relative; overflow: hidden;
}
.scanner-sweep {
  position: absolute; top: 0; left: -100%; width: 40%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(251, 191, 36, 0.05), transparent);
  animation: sweep 4s linear infinite;
}
.hero-display-title { font-weight: 800; font-size: 3rem; letter-spacing: -2px; line-height: 1.1; color: #0f172a; }
.text-gradient-gold { background: linear-gradient(135deg, #fbbf24, #f59e0b); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.premium-tag { display: inline-flex; align-items: center; background: #fef3c7; color: #fbbf24; padding: 6px 16px; border-radius: 100px; font-size: 10px; font-weight: 800; }

/* --- IA MODULE --- */
.ia-insight-module {
  background: #f8fafc; border-radius: 28px; padding: 25px;
  display: flex; align-items: center; gap: 24px; max-width: 600px;
  border: 1px solid #f1f5f9; box-shadow: inset 0 2px 4px rgba(0,0,0,0.02);
}
.ia-orb-container {
  width: 54px; height: 54px; background: #fbbf24; color: #0f172a;
  border-radius: 18px; display: flex; align-items: center; justify-content: center;
  font-size: 24px; position: relative; box-shadow: 0 10px 20px rgba(251,191,36,0.2);
}
.ia-orb-ring { position: absolute; inset: -6px; border: 2px solid rgba(251,191,36,0.2); border-radius: 22px; animation: rotate 10s linear infinite; }
.ia-header-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 5px; }
.ia-title { font-size: 10px; font-weight: 800; color: #fbbf24; letter-spacing: 1px; }
.ia-status { font-size: 8px; font-weight: 800; background: #ecfdf5; color: #10b981; padding: 2px 8px; border-radius: 4px; }
.ia-message { font-size: 14px; font-weight: 500; color: #475569; margin: 0; line-height: 1.5; }

/* --- CHAMBER & BOT --- */
.bot-chamber { position: relative; width: 250px; height: 250px; }
.chamber-rings .ring { position: absolute; border: 1px solid rgba(251, 191, 36, 0.1); border-radius: 50%; top: 50%; left: 50%; transform: translate(-50%, -50%); }
.ring.r1 { width: 200px; height: 200px; animation: rotate 20s linear infinite; border-style: dashed; }
.ring.r2 { width: 260px; height: 260px; animation: rotate 30s linear infinite reverse; }
.bot-svg-elite { width: 100%; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.06)); animation: float 5s infinite ease-in-out; }

/* --- BENTO CARDS --- */
.luxury-bento-card {
  background: white; border-radius: 35px; padding: 30px;
  border: 1px solid #f1f5f9; transition: all 0.5s cubic-bezier(0.2, 1, 0.3, 1);
  position: relative; overflow: hidden;
}
.luxury-bento-card:hover { transform: translateY(-12px); border-color: var(--accent-color); box-shadow: 0 30px 60px rgba(0,0,0,0.04); }
.card-glow { position: absolute; top: -50%; left: -50%; width: 200%; height: 200%; background: radial-gradient(circle, var(--accent-color) 0%, transparent 70%); opacity: 0; transition: 0.5s; pointer-events: none; filter: blur(60px); }
.luxury-bento-card:hover .card-glow { opacity: 0.03; }

.bento-icon-wrapper { width: 56px; height: 56px; border-radius: 20px; display: flex; align-items: center; justify-content: center; font-size: 24px; }
.bento-trend-tag { background: #ecfdf5; color: #10b981; padding: 4px 10px; border-radius: 8px; font-size: 10px; font-weight: 800; }
.bento-value { font-size: 2.2rem; font-weight: 800; color: #0f172a; letter-spacing: -2px; }
.bento-label { font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }

/* --- PANELS --- */
.panel-luxe { background: white; border-radius: 40px; border: 1px solid #f1f5f9; box-shadow: 0 10px 30px rgba(0,0,0,0.02); }
.panel-pill-tabs { background: #f8fafc; padding: 4px; border-radius: 100px; display: flex; gap: 5px; }
.tab-btn { border: none; background: transparent; padding: 6px 18px; border-radius: 100px; font-size: 10px; font-weight: 800; color: #94a3b8; transition: 0.3s; }
.tab-btn.active { background: #0f172a; color: white; }

/* --- ACTIVITY ROW --- */
.activity-row-pill { display: flex; gap: 18px; padding: 16px; border-radius: 24px; margin-bottom: 8px; transition: 0.3s; }
.activity-row-pill:hover { background: #f8fafc; transform: translateX(5px); }
.row-indicator { width: 4px; height: 35px; border-radius: 10px; flex-shrink: 0; }
.user-name { display: block; font-weight: 800; font-size: 14px; color: #0f172a; }
.action-desc { display: block; font-size: 12px; color: #64748b; margin-top: 1px; }
.campagne-chip { font-size: 9px; font-weight: 800; border: 1px solid; padding: 2px 10px; border-radius: 6px; margin-top: 6px; display: inline-block; text-transform: uppercase; }

/* --- ANIMATIONS --- */
@keyframes orbit { from { transform: rotate(0) translate(20px) rotate(0); } to { transform: rotate(360deg) translate(20px) rotate(-360deg); } }
@keyframes sweep { 0% { left: -100%; } 100% { left: 200%; } }
@keyframes rotate { from { transform: translate(-50%, -50%) rotate(0deg); } to { transform: translate(-50%, -50%) rotate(360deg); } }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
@keyframes pulse { 0% { box-shadow: 0 0 0 0 rgba(251, 191, 36, 0.7); } 70% { box-shadow: 0 0 0 10px rgba(251, 191, 36, 0); } }
</style>