<template>
  <div class="admin-layout">
    <!-- COUCHES DÉCORATIVES (Identiques au Login pour la cohérence) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <AppSidebar />
    
    <div class="main-viewport animate__animated animate__fadeIn">
      <AppNavbar />

      <div class="container-fluid px-4 pt-3">
        
        <!-- 1. TERMINAL STATUS BAR (Style Apple/Linear) -->
        <div class="terminal-status-bar mb-4">
          <div class="d-flex justify-content-between align-items-center px-3">
            <div class="breadcrumb-cyber">
              <span class="root">EVALUATECH</span>
              <span class="sep">//</span>
              <span class="current">{{ roleLabel }}</span>
            </div>
            <div class="system-tag">
              <span class="pulse-dot"></span>
              <span class="tag-text">LOGGED_AS: {{ authStore.user?.name.toUpperCase() }}</span>
            </div>
          </div>
        </div>

        <!-- 2. HERO SECTION : INTERFACE IA CENTRALISÉE -->
        <div class="hero-cyber-card mb-5">
          <div class="row align-items-center">
            <div class="col-lg-8 p-5">
              <div class="badge-amber-glow mb-3">
                <i class="fa-solid fa-shield-halved me-2"></i> SESSION SÉCURISÉE : {{ roleContext }}
              </div>
              <h1 class="display-title-cyber">
                Ravi de vous revoir, <br>
                <span class="text-amber">{{ authStore.user?.name || 'Utilisateur' }}</span>
              </h1>
              <div class="ai-insight-box mt-4">
                <div class="ai-box-icon">
                  <i class="fa-solid fa-microchip"></i>
                </div>
                <p class="ai-insight-text">
                  <strong>SYSTÈME IA :</strong> {{ aiInsight }}
                </p>
              </div>
              <div class="hero-action-group mt-4">
                <button v-if="userRole === 'Candidat'" @click="$router.push('/exam-lobby')" class="btn-cyber-primary">
                  Lancer l'évaluation <i class="fa-solid fa-rocket ms-2"></i>
                </button>
                <button v-if="userRole === 'AdminEntreprise'" @click="$router.push('/invite')" class="btn-cyber-primary">
                  Inviter des Talents <i class="fa-solid fa-user-plus ms-2"></i>
                </button>
                <button class="btn-cyber-outline">Archives de bord</button>
              </div>
            </div>
            
            <!-- Robot identique au Login -->
            <div class="col-lg-4 text-center d-none d-lg-block">
              <div class="ai-robot-dashboard">
                <div class="robot-float">
                  <svg class="modern-robot" viewBox="0 0 200 200" width="220">
                    <defs>
                      <linearGradient id="botGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                        <stop offset="0%" style="stop-color:#ffffff" />
                        <stop offset="100%" style="stop-color:#f1f5f9" />
                      </linearGradient>
                    </defs>
                    <rect x="55" y="50" width="90" height="85" rx="28" fill="url(#botGrad)" stroke="#e2e8f0" stroke-width="2"/>
                    <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
                    <g class="eyes-anim">
                      <circle cx="85" cy="81" r="5" fill="#eab308" />
                      <circle cx="115" cy="81" r="5" fill="#eab308" />
                    </g>
                    <path d="M85 110 Q100 118 115 110" stroke="#cbd5e1" stroke-width="2" fill="none" stroke-linecap="round" />
                  </svg>
                </div>
                <div class="robot-glow-base"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 3. KPI BENTO GRID -->
        <div class="row g-4 mb-5">
          <div class="col-md-6 col-xl-3" v-for="stat in dynamicStats" :key="stat.label">
            <div class="kpi-cyber-card">
              <div class="d-flex justify-content-between align-items-start">
                <div class="kpi-icon-box" :style="{ color: stat.color, backgroundColor: stat.bg }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="kpi-trend" :class="stat.trendUp ? 'up' : 'down'">
                  {{ stat.trend }} <i :class="stat.trendUp ? 'fa-solid fa-caret-up' : 'fa-solid fa-caret-down'"></i>
                </div>
              </div>
              <div class="mt-4">
                <h2 class="kpi-value">{{ stat.value }}</h2>
                <span class="kpi-label">{{ stat.label }}</span>
              </div>
              <div class="kpi-progress mt-3">
                <div class="kpi-progress-bar" :style="{ width: '70%', background: stat.color }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 4. ANALYTICS AREA -->
        <div class="row g-4 pb-5">
          <div class="col-lg-8">
            <div class="glass-panel">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <h5 class="panel-title">COURBE DE PERFORMANCE GLOBAL</h5>
                <div class="chart-filter">
                  <button class="active">LIVE</button>
                  <button>7J</button>
                </div>
              </div>
              <div class="chart-height">
                <canvas id="mainActivityChart"></canvas>
              </div>
            </div>
          </div>
          <div class="col-lg-4">
            <div class="glass-panel">
              <h5 class="panel-title mb-4">FLUX D'ACTIVITÉS</h5>
              <div class="cyber-timeline">
                <div class="timeline-entry" v-for="act in recentActs" :key="act.id">
                  <div class="entry-dot" :style="{ background: act.color }"></div>
                  <div class="entry-content">
                    <p class="entry-title">{{ act.title }}</p>
                    <span class="entry-info">{{ act.time }} • {{ act.user }}</span>
                  </div>
                </div>
              </div>
              <button class="btn-view-all mt-4">ACCÉDER AUX LOGS <i class="fa-solid fa-arrow-right ms-2"></i></button>
            </div>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
/* Logique identique à ta version précédente pour la gestion des rôles */
import { computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import Chart from 'chart.js/auto';

const authStore = useAuthStore();
const userRole = computed(() => authStore.role);

const roleLabel = computed(() => {
  const map = { 'SuperAdmin': 'ADMINISTRATEUR MAÎTRE', 'AdminEntreprise': 'ESPACE CORPORATE', 'Evaluateur': 'ESPACE ÉVALUATEUR', 'Candidat': 'PORTAIL CANDIDAT' };
  return map[userRole.value] || 'TABLEAU DE BORD';
});

const roleContext = computed(() => {
  if (userRole.value === 'SuperAdmin') return 'CORE SYSTEM INFRA';
  if (userRole.value === 'AdminEntreprise') return 'CORP METRICS';
  if (userRole.value === 'Evaluateur') return 'EVAL_METRICS';
  return 'EVAL_PROCESS';
});

const aiInsight = computed(() => {
  if (userRole.value === 'Candidat') return "VOTRE PROFIL EST OPTIMISÉ À 94% POUR L'EXAMEN ACTUEL.";
  return "ANALYSE COMPLÉTÉE : 12 NOUVELLES INSCRIPTIONS DÉTECTÉES CE MATIN.";
});

const dynamicStats = computed(() => {
  const stats = {
    AdminEntreprise: [
      { label: 'TALENTS', value: '12', icon: 'fa-solid fa-users-viewfinder', bg: '#eff6ff', color: '#3b82f6', trend: '+4', trendUp: true },
      { label: 'ÉVALUATIONS', value: '284', icon: 'fa-solid fa-file-signature', bg: '#fffbeb', color: '#f59e0b', trend: '+15%', trendUp: true },
      { label: 'TAUX SUCCÈS', value: '78%', icon: 'fa-solid fa-chart-pie', bg: '#f5f3ff', color: '#8b5cf6', trend: '+2%', trendUp: true },
      { label: 'ALERTES IA', value: '02', icon: 'fa-solid fa-bolt-lightning', bg: '#fef2f2', color: '#ef4444', trend: 'BAS', trendUp: false }
    ],
    Evaluateur: [
      { label: 'SESSIONS', value: '05', icon: 'fa-solid fa-calendar-check', bg: '#fffbeb', color: '#f59e0b', trend: '+1', trendUp: true },
      { label: 'QUESTIONS', value: '248', icon: 'fa-solid fa-database', bg: '#eff6ff', color: '#3b82f6', trend: '+12', trendUp: true },
      { label: 'À CORRIGER', value: '03', icon: 'fa-solid fa-pen-nib', bg: '#fef2f2', color: '#ef4444', trend: 'Urgent', trendUp: false },
      { label: 'SCORE MOY.', value: '14.5', icon: 'fa-solid fa-star', bg: '#f5f3ff', color: '#8b5cf6', trend: '+0.5', trendUp: true }
    ],
    Candidat: [
        { label: 'À PASSER', value: '01', icon: 'fa-solid fa-hourglass-half', bg: '#fffbeb', color: '#f59e0b', trend: 'Urg.', trendUp: false },
        { label: 'TERMINÉS', value: '04', icon: 'fa-solid fa-check-double', bg: '#ecfdf5', color: '#10b981', trend: '+1', trendUp: true },
        { label: 'SCORE MOY.', value: '82%', icon: 'fa-solid fa-brain', bg: '#f5f3ff', color: '#8b5cf6', trend: '+5%', trendUp: true },
        { label: 'RANG', value: '#12', icon: 'fa-solid fa-trophy', bg: '#eff6ff', color: '#3b82f6', trend: '+2', trendUp: true }
    ]
  };
  return stats[userRole.value] || stats.AdminEntreprise;
});

const recentActs = [
  { id: 1, title: 'Évaluation Cloud finie', time: '5 min', user: 'Admin', color: '#10b981' },
  { id: 2, title: 'Nouvelle règle de sécurité', time: '1h', user: 'System', color: '#8b5cf6' },
  { id: 3, title: 'Échec authentification', time: '2h', user: 'Anonyme', color: '#ef4444' }
];

onMounted(() => {
  const ctx = document.getElementById('mainActivityChart').getContext('2d');
  const gradient = ctx.createLinearGradient(0, 0, 0, 400);
  gradient.addColorStop(0, 'rgba(234, 179, 8, 0.3)');
  gradient.addColorStop(1, 'rgba(234, 179, 8, 0)');

  new Chart(ctx, {
    type: 'line',
    data: {
      labels: ['LUN', 'MAR', 'MER', 'JEU', 'VEN', 'SAM', 'DIM'],
      datasets: [{
        label: 'Activité',
        data: [40, 60, 45, 90, 75, 110, 95],
        borderColor: '#eab308',
        backgroundColor: gradient,
        fill: true,
        tension: 0.4,
        borderWidth: 4,
        pointRadius: 6,
        pointBackgroundColor: '#fff',
        pointBorderColor: '#eab308',
        pointBorderWidth: 2
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        x: { grid: { display: false }, ticks: { color: '#94a3b8', font: { weight: '700' } } },
        y: { grid: { color: 'rgba(226, 232, 240, 0.5)' }, ticks: { color: '#94a3b8' } }
      }
    }
  });
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

/* --- LAYOUT CONFIG (Coherent with Login) --- */
.admin-layout {
  min-height: 100vh;
  background-color: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  display: flex;
  position: relative;
  overflow-x: hidden;
}

.main-viewport {
  flex-grow: 1;
  position: relative;
  z-index: 5;
  padding-left: 20px;
}

/* --- DECO LAYERS (Copied from Login) --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: radial-gradient(#e2e8f0 1.5px, transparent 1.5px); background-size: 40px 40px; opacity: 0.4; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(130px); opacity: 0.15; z-index: 1; }
.orb-amber { width: 600px; height: 600px; background: #fbbf24; top: -100px; right: -100px; }
.orb-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -100px; left: -100px; }

/* --- TERMINAL STATUS BAR --- */
.terminal-status-bar {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  border: 1px solid rgba(255,255,255,0.5);
  padding: 10px 0;
  box-shadow: 0 4px 15px rgba(0,0,0,0.02);
}
.breadcrumb-cyber .root { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }
.breadcrumb-cyber .sep { color: #eab308; margin: 0 10px; font-weight: 800; }
.breadcrumb-cyber .current { font-size: 11px; font-weight: 800; color: #0f172a; letter-spacing: 1px; }

.system-tag { display: flex; align-items: center; gap: 8px; background: #0f172a; padding: 6px 15px; border-radius: 100px; }
.tag-text { color: white; font-size: 9px; font-weight: 800; letter-spacing: 0.5px; }
.pulse-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; animation: blink 2s infinite; }

/* --- HERO CARD --- */
.hero-cyber-card {
  background: rgba(255, 255, 255, 0.9);
  border: 1px solid #fff;
  border-radius: 40px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.04);
  backdrop-filter: blur(20px);
  position: relative;
  overflow: hidden;
}
.display-title-cyber { font-weight: 800; font-size: 46px; letter-spacing: -2px; color: #0f172a; line-height: 1.1; }
.text-amber { color: #eab308; }
.badge-amber-glow {
  display: inline-block; padding: 8px 18px; background: #0f172a; color: #fff;
  border-radius: 100px; font-size: 11px; font-weight: 800; letter-spacing: 1px;
}
.ai-insight-box {
  background: #f8fafc; border-left: 5px solid #eab308; padding: 20px; border-radius: 20px;
  display: flex; align-items: center; gap: 20px;
}
.ai-box-icon { font-size: 24px; color: #eab308; }
.ai-insight-text { margin: 0; font-size: 14px; color: #475569; font-weight: 500; }

/* --- ROBOT DANS LE DASHBOARD --- */
.ai-robot-dashboard { position: relative; }
.robot-float { animation: float 4s ease-in-out infinite; }
.eyes-anim { animation: blinkEyes 4s infinite; }
.robot-glow-base {
  width: 120px; height: 15px; background: #eab308; opacity: 0.2;
  filter: blur(15px); border-radius: 50%; margin: 0 auto;
}

/* --- BENTO KPI --- */
.kpi-cyber-card {
  background: white; border-radius: 30px; padding: 25px; border: 1px solid #f1f5f9;
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.kpi-cyber-card:hover { transform: translateY(-10px); box-shadow: 0 20px 30px rgba(0,0,0,0.05); border-color: #eab308; }
.kpi-icon-box { width: 50px; height: 50px; border-radius: 18px; display: flex; align-items: center; justify-content: center; font-size: 22px; }
.kpi-value { font-size: 32px; font-weight: 800; color: #0f172a; margin: 10px 0 0; }
.kpi-label { font-size: 11px; font-weight: 800; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
.kpi-trend { font-size: 10px; font-weight: 800; padding: 4px 10px; border-radius: 100px; }
.kpi-trend.up { background: #ecfdf5; color: #10b981; }
.kpi-trend.down { background: #fef2f2; color: #ef4444; }
.kpi-progress { height: 6px; background: #f1f5f9; border-radius: 10px; }
.kpi-progress-bar { height: 100%; border-radius: 10px; }

/* --- GLASS PANELS --- */
.glass-panel {
  background: white; border: 1px solid #f1f5f9; border-radius: 35px; padding: 30px; height: 100%;
}
.panel-title { font-size: 12px; font-weight: 800; letter-spacing: 1px; color: #0f172a; }

/* --- TIMELINE --- */
.cyber-timeline { display: flex; flex-direction: column; gap: 20px; }
.timeline-entry { display: flex; gap: 15px; align-items: flex-start; }
.entry-dot { width: 8px; height: 8px; border-radius: 50%; margin-top: 6px; }
.entry-title { font-size: 13px; font-weight: 700; color: #1e293b; margin: 0; }
.entry-info { font-size: 11px; color: #94a3b8; }

/* --- BUTTONS --- */
.btn-cyber-primary {
  background: #eab308; color: #0f172a; border: none; padding: 14px 28px;
  border-radius: 18px; font-weight: 800; font-size: 14px; transition: 0.3s;
}
.btn-cyber-primary:hover { box-shadow: 0 10px 20px rgba(234, 179, 8, 0.3); transform: translateY(-2px); }
.btn-cyber-outline {
  background: transparent; border: 2px solid #0f172a; color: #0f172a; padding: 14px 28px;
  border-radius: 18px; font-weight: 800; margin-left: 12px;
}
.btn-view-all { width: 100%; background: #f8fafc; border: 1px solid #e2e8f0; padding: 12px; border-radius: 15px; font-weight: 700; font-size: 11px; color: #64748b; }

/* --- ANIMATIONS --- */
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-20px); } }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }
@keyframes blinkEyes { 0%, 45%, 55%, 100% { transform: scaleY(1); } 50% { transform: scaleY(0.1); } }
</style>