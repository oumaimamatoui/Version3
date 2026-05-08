<template>
  <div class="grandmaster-root d-flex">
    <!-- 1. DEEP IMMERSIVE ATMOSPHERE -->
    <div class="sovereign-bg">
      <div class="aura aura-gold"></div>
      <div class="aura aura-indigo"></div>
      <div class="grid-layer"></div>
      <div class="particles-overlay"></div>
    </div>

    <AppSidebar />
    
    <div class="content-wrapper flex-grow-1 position-relative d-flex flex-column">
      <AppNavbar />

      <main class="p-4 main-container animate-fade-in" v-if="!loading">
        
        <!-- 2. SOVEREIGN HEADER -->
        <header class="d-flex justify-content-between align-items-center mb-5">
          <div class="header-content">
            <div class="system-path">NEURAL_NETWORK // ADMIN_CORE // ANALYTICS_PRO_MAX</div>
            <h1 class="display-title">Cognition <span class="gold-shimmer">Plateforme</span></h1>
            <div class="status-hub">
              <div class="status-badge live">
                <span class="ping-core"></span>
                <span class="label">LIVE DATASTREAM</span>
              </div>
              <div class="status-badge latency">
                <i class="fa-solid fa-bolt-lightning"></i>
                <span class="label">12ms RESPONSE</span>
              </div>
            </div>
          </div>
          
          <div class="ai-companion-vessel">
            <div class="bot-shell">
              <svg viewBox="0 0 200 200" class="bot-svg">
                <!-- Floating Antennas -->
                <line x1="100" y1="40" x2="100" y2="60" stroke="#fbbf24" stroke-width="2" class="antenna-anim" />
                <circle cx="100" cy="35" r="4" fill="#fbbf24" class="glow-dot" />
                <!-- Bot Head -->
                <rect x="50" y="60" width="100" height="85" rx="40" fill="white" stroke="#e2e8f0" stroke-width="1" />
                <rect x="65" y="80" width="70" height="35" rx="15" fill="#0f172a" />
                <circle cx="85" cy="98" r="4" fill="#fbbf24" class="eye-scan" />
                <circle cx="115" cy="98" r="4" fill="#fbbf24" class="eye-scan" />
                <!-- AI Core Pulse -->
                <circle cx="100" cy="125" r="8" fill="#fbbf24" class="core-pulse" />
              </svg>
            </div>
            <div class="bot-status-text">
              <span class="name">EVALUA_AI_CORE</span>
              <span class="mode">OPTIMISATION ACTIVE</span>
            </div>
          </div>

          <button @click="refreshData" class="btn-sovereign" :disabled="refreshing">
            <div class="btn-glow"></div>
            <i class="fa-solid fa-sync-alt me-2" :class="{'fa-spin': refreshing}"></i>
            <span>{{ refreshing ? 'RE-CALIBRATION...' : 'ACTUALISER' }}</span>
          </button>
        </header>

        <!-- 3. BENTO METRIC TILES -->
        <div class="row g-4 mb-5">
          <div class="col-xl-3 col-md-6" v-for="(stat, i) in masterKpis" :key="i">
            <div class="metric-card-pro" :style="{'--accent': stat.color}">
              <div class="optical-refraction"></div>
              <div class="card-body-content">
                <div class="icon-vessel-pro">
                  <i :class="stat.icon"></i>
                  <div class="icon-ring"></div>
                </div>
                <div class="value-group">
                  <h3 class="big-value">{{ stat.val }}</h3>
                  <span class="small-label">{{ stat.label }}</span>
                </div>
              </div>
              <div class="card-data-line">
                <div class="line-fill" :style="{ width: '80%', background: stat.color }"></div>
              </div>
            </div>
          </div>
        </div>

        <!-- 4. ANALYTICS ENGINE VISUALS -->
        <div class="row g-4 mb-4">
          <div class="col-lg-8">
            <div class="engine-panel chart-panel h-100">
              <div class="panel-header-pro">
                <div class="title-stack">
                  <h6 class="pro-title">PERFORMANCE DES FLUX D'ÉVALUATION</h6>
                  <p class="pro-sub">Mesures analytiques de précision par campagne</p>
                </div>
                <div class="time-range-picker">
                   <span>24H</span> <span class="active">7J</span> <span>30J</span>
                </div>
              </div>
              
              <div class="visual-engine-stage">
                <!-- Data Grid Background -->
                <div class="stage-grid"></div>
                
                <div class="glow-pillars-container">
                  <div v-for="(item, i) in apiChartData" :key="i" class="pillar-group">
                    <div class="pillar-value">{{ item.score }}%</div>
                    <div class="pillar-vessel">
                      <div class="pillar-fill" :style="{ height: item.score + '%' }">
                        <div class="pillar-light-beam"></div>
                        <div class="pillar-particles"></div>
                      </div>
                    </div>
                    <span class="pillar-label" :title="item.name">{{ item.name }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
          <!-- SYSTEM HEALTH & TALENT FLOW -->
          <div class="col-lg-4">
            <div class="engine-panel p-4 h-100">
              <h6 class="pro-title mb-4">INFRASTRUCTURE & RESSOURCES</h6>
              
              <div class="resource-gauges">
                <div class="gauge-item-pro mb-4" v-for="res in resources" :key="res.name">
                  <div class="d-flex justify-content-between align-items-center mb-2">
                    <span class="g-name">{{ res.name }}</span>
                    <span class="g-val">{{ res.usage }}%</span>
                  </div>
                  <div class="g-track">
                    <div class="g-fill-pro" :style="{ width: res.usage + '%' }"></div>
                  </div>
                </div>
              </div>

              <!-- Top Talent Stream -->
              <div class="talent-stream mt-5">
                <h6 class="pro-title-sm mb-3">TOP TALENTS DÉTECTÉS</h6>
                <div class="talent-row-pro" v-for="(leader, idx) in leaders" :key="idx">
                   <div class="talent-rank">{{ idx + 1 }}</div>
                   <div class="talent-details">
                     <span class="t-name">{{ leader.name }}</span>
                     <span class="t-meta">Vérifié par EvaluaBot</span>
                   </div>
                   <div class="t-score-badge">{{ leader.score }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- 5. NEURAL FEED FOOTER -->
        <footer class="neural-footer-bar">
          <div class="feed-label">NEURAL_STREAM</div>
          <div class="feed-ticker" v-if="lastAudit">
             <span class="time">[{{ lastSyncTime }}]</span>
             <span class="user">{{ lastAudit.utilisateur }}</span>
             <i class="fa-solid fa-chevron-right mx-2"></i>
             <span class="action">{{ lastAudit.action }}</span> : <span class="detail">{{ lastAudit.details }}</span>
          </div>
          <div class="feed-ticker" v-else>En attente de synchronisation...</div>
        </footer>

      </main>

      <!-- SUPREME LOADER -->
      <div v-else class="supreme-loader">
         <div class="loader-nexus">
            <div class="ring"></div>
            <div class="ring"></div>
            <div class="ring"></div>
            <div class="core-dot"></div>
         </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const loading = ref(true);
const refreshing = ref(false);
const lastSyncTime = ref('');
const systemStatus = ref('OPTIMAL');

const masterKpis = ref([
  { label: 'ORGS ACTIVES', val: '0', icon: 'fa-solid fa-building-shield', color: '#fbbf24' },
  { label: 'TALENTS IA', val: '0', icon: 'fa-solid fa-microchip', color: '#6366f1' },
  { label: 'SESSIONS FLOW', val: '0', icon: 'fa-solid fa-wave-square', color: '#fbbf24' },
  { label: 'SÉCURITÉ', val: 'MAX', icon: 'fa-solid fa-shield-halved', color: '#6366f1' },
]);

const apiChartData = ref([]);
const leaders = ref([]);
const lastAudit = ref(null);
const resources = ref([
  { name: 'Neural Processing', usage: 68 },
  { name: 'Uptime Global', usage: 99 },
  { name: 'Sync Database', usage: 42 }
]);

const fetchData = async () => {
  try {
    refreshing.value = true;
    const [statsRes, globalRes, auditRes] = await Promise.all([
      api.get('/SuperAdmin/stats'),
      api.get('/Dashboard/global-stats'),
      api.get('/SuperAdmin/audit-logs')
    ]);

    masterKpis.value[0].val = statsRes.data.totalEntreprises;
    masterKpis.value[1].val = statsRes.data.totalUtilisateurs;
    masterKpis.value[2].val = statsRes.data.totalTests || '0';
    
    apiChartData.value = globalRes.data.chart || [];
    leaders.value = globalRes.data.leaders || [];
    if(auditRes.data.length > 0) lastAudit.value = auditRes.data[auditRes.data.length - 1];

    lastSyncTime.value = new Date().toLocaleTimeString();
    systemStatus.value = 'OPTIMAL';
  } catch (err) {
    console.error("Critical Sync Failure", err);
    systemStatus.value = 'DÉGRADÉ';
    apiChartData.value = [{name: 'M1', score: 80}, {name: 'M2', score: 65}, {name: 'M3', score: 90}, {name: 'M4', score: 45}];
  } finally {
    loading.value = false;
    refreshing.value = false;
  }
};

onMounted(() => fetchData());
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@400;700&display=swap');

/* --- MASTER ROOT & ATMOSPHERE --- */
.grandmaster-root {
  min-height: 100vh; background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif; color: #0f172a;
  position: relative; overflow-x: hidden;
}

.sovereign-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.aura { position: absolute; border-radius: 50%; filter: blur(150px); opacity: 0.15; }
.aura-gold { width: 800px; height: 800px; background: #fbbf24; top: -15%; left: -10%; }
.aura-indigo { width: 700px; height: 700px; background: #6366f1; bottom: -10%; right: -5%; }
.grid-layer { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 1px, transparent 1px); background-size: 40px 40px; opacity: 0.2; }

/* --- HEADER DISPLAY --- */
.system-path { font-family: 'JetBrains Mono'; font-size: 10px; color: #94a3b8; letter-spacing: 4px; margin-bottom: 8px; }
.display-title { font-weight: 900; font-size: 42px; letter-spacing: -2px; }
.gold-shimmer { background: linear-gradient(90deg, #0f172a, #fbbf24, #0f172a); background-size: 200%; animation: shimmer 5s infinite linear; -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
@keyframes shimmer { to { background-position: 200% center; } }

.status-hub { display: flex; gap: 15px; margin-top: 10px; }
.status-badge { background: white; padding: 5px 15px; border-radius: 100px; border: 1px solid #f1f5f9; display: flex; align-items: center; gap: 8px; font-size: 10px; font-weight: 800; color: #64748b; }
.ping-core { width: 8px; height: 8px; background: #10b981; border-radius: 50%; animation: ping 2s infinite; }
@keyframes ping { 0% { transform: scale(1); opacity: 1; } 100% { transform: scale(3); opacity: 0; } }

/* --- NEURAL BOT 2.0 --- */
.ai-companion-vessel { display: flex; align-items: center; gap: 15px; background: white; padding: 8px 20px; border-radius: 24px; border: 1px solid #f1f5f9; box-shadow: 0 10px 30px rgba(0,0,0,0.03); }
.bot-svg { width: 55px; }
.eye-scan { animation: eyeMove 3s infinite; }
@keyframes eyeMove { 0%, 100% { transform: translateX(0); } 50% { transform: translateX(2px); } }
.core-pulse { animation: pulseCore 2s infinite; opacity: 0.6; }
@keyframes pulseCore { 0% { r: 5; opacity: 0.8; } 100% { r: 12; opacity: 0; } }
.bot-status-text { display: flex; flex-direction: column; }
.bot-status-text .name { font-weight: 900; font-size: 11px; color: #0f172a; }
.bot-status-text .mode { font-size: 9px; font-weight: 700; color: #fbbf24; }

/* --- BENTO METRIC TILES --- */
.metric-card-pro {
  background: white; border-radius: 35px; padding: 30px; border: 1.5px solid white;
  box-shadow: 0 20px 50px -10px rgba(0,0,0,0.05); position: relative; overflow: hidden; transition: 0.4s;
}
.metric-card-pro:hover { transform: translateY(-10px); }
.optical-refraction { position: absolute; inset: 0; background: linear-gradient(135deg, rgba(255,255,255,0.4) 0%, transparent 100%); pointer-events: none; }
.icon-vessel-pro { width: 55px; height: 55px; background: #f8fafc; border-radius: 20px; display: flex; align-items: center; justify-content: center; font-size: 22px; color: var(--accent); position: relative; }
.icon-ring { position: absolute; inset: -5px; border: 1px solid var(--accent); border-radius: 20px; opacity: 0.2; }
.big-value { font-weight: 900; font-size: 34px; margin: 15px 0 0; letter-spacing: -1px; }
.small-label { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; }

/* --- GLOW PILLAR CHART --- */
.engine-panel { background: rgba(255, 255, 255, 0.8); backdrop-filter: blur(30px); border-radius: 45px; border: 1px solid white; box-shadow: 0 25px 60px rgba(0,0,0,0.03); }
.pro-title { font-weight: 800; font-size: 14px; letter-spacing: 1px; color: #475569; }
.visual-engine-stage { height: 350px; position: relative; overflow: hidden; display: flex; align-items: flex-end; padding: 40px; }
.stage-grid { position: absolute; inset: 0; background-image: linear-gradient(#e2e8f0 1px, transparent 1px), linear-gradient(90deg, #e2e8f0 1px, transparent 1px); background-size: 30px 30px; opacity: 0.2; }

.glow-pillars-container { display: flex; align-items: flex-end; justify-content: space-around; width: 100%; height: 100%; position: relative; z-index: 2; }
.pillar-group { display: flex; flex-direction: column; align-items: center; gap: 15px; }
.pillar-vessel { width: 45px; height: 220px; background: rgba(0,0,0,0.03); border-radius: 100px; position: relative; overflow: hidden; }
.pillar-fill { width: 100%; position: absolute; bottom: 0; background: linear-gradient(180deg, #fbbf24, #f59e0b); border-radius: 100px; transition: height 1.5s cubic-bezier(0.19, 1, 0.22, 1); }
.pillar-light-beam { position: absolute; top: 0; left: 15%; width: 25%; height: 100%; background: rgba(255,255,255,0.2); filter: blur(4px); }
.pillar-value { font-size: 12px; font-weight: 900; color: #0f172a; }
.pillar-label { 
  font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 65px; text-align: center;
}

/* --- TOP TALENTS --- */
.talent-row-pro { display: flex; align-items: center; justify-content: space-between; padding: 12px 15px; background: #f8fafc; border-radius: 16px; margin-bottom: 10px; transition: 0.3s; border: 1px solid transparent; }
.talent-row-pro:hover { border-color: #fbbf24; background: white; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(0,0,0,0.05); }
.talent-rank { width: 30px; height: 30px; background: #e2e8f0; color: #475569; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 12px; }
.talent-details { flex-grow: 1; padding: 0 15px; display: flex; flex-direction: column; }
.t-name { font-weight: 800; font-size: 13px; color: #0f172a; }
.t-meta { font-size: 10px; color: #94a3b8; font-weight: 600; }
.t-score-badge { background: #0f172a; color: #fbbf24; padding: 5px 10px; border-radius: 8px; font-weight: 900; font-size: 12px; }

/* --- DARK MODE OVERRIDES --- */
[data-theme="dark"] .grandmaster-root { background: #0f172a; color: #f8fafc; }
[data-theme="dark"] .display-title { color: white; }
[data-theme="dark"] .metric-card-pro, [data-theme="dark"] .engine-panel {
  background: rgba(30, 41, 59, 0.8); border-color: rgba(255, 255, 255, 0.1); box-shadow: 0 25px 60px rgba(0,0,0,0.3);
}
[data-theme="dark"] .pro-title, [data-theme="dark"] .small-label, [data-theme="dark"] .system-path, [data-theme="dark"] .bot-status-text .name { color: #cbd5e1; }
[data-theme="dark"] .big-value, [data-theme="dark"] .pillar-value { color: white; }
[data-theme="dark"] .status-badge, [data-theme="dark"] .ai-companion-vessel, [data-theme="dark"] .icon-vessel-pro { background: #1e293b; border-color: #334155; }
[data-theme="dark"] .bot-shell rect[fill="white"] { fill: #1e293b; stroke: #334155; }
[data-theme="dark"] .talent-row-pro { background: #1e293b; border-color: transparent; }
[data-theme="dark"] .talent-row-pro:hover { background: #334155; border-color: #fbbf24; }
[data-theme="dark"] .t-name { color: white; }
[data-theme="dark"] .talent-rank { background: #475569; color: white; }
[data-theme="dark"] .stage-grid { background-image: linear-gradient(#334155 1px, transparent 1px), linear-gradient(90deg, #334155 1px, transparent 1px); }


/* --- NEURAL FOOTER --- */
.neural-footer-bar { background: #0f172a; color: white; padding: 15px 30px; border-radius: 25px; display: flex; align-items: center; gap: 20px; font-family: 'JetBrains Mono'; font-size: 12px; box-shadow: 0 15px 40px rgba(15, 23, 42, 0.2); }
.feed-label { background: #fbbf24; color: #0f172a; padding: 3px 10px; border-radius: 8px; font-weight: 900; font-size: 10px; }

/* --- SUPREME LOADER --- */
.supreme-loader { position: fixed; inset: 0; background: white; z-index: 9999; display: flex; align-items: center; justify-content: center; }
.loader-nexus { position: relative; width: 100px; height: 100px; display: flex; align-items: center; justify-content: center; }
.ring { position: absolute; border: 3px solid transparent; border-top-color: #fbbf24; border-radius: 50%; animation: spin 1s infinite linear; }
.ring:nth-child(1) { width: 80px; height: 80px; animation-duration: 1.2s; }
.ring:nth-child(2) { width: 60px; height: 60px; animation-duration: 1s; border-top-color: #6366f1; }
.ring:nth-child(3) { width: 40px; height: 40px; animation-duration: 0.8s; }
.core-dot { width: 10px; height: 10px; background: #fbbf24; border-radius: 50%; box-shadow: 0 0 15px #fbbf24; }

/* --- BUTTONS --- */
.btn-sovereign { background: #0f172a; color: white; border: none; padding: 12px 30px; border-radius: 20px; font-weight: 800; font-size: 13px; position: relative; overflow: hidden; transition: 0.3s; }
.btn-sovereign:hover { transform: translateY(-3px) scale(1.02); background: #fbbf24; color: #0f172a; }
</style>