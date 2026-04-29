<template>
  <div class="admin-body d-flex">
    <AppSidebar />

    <div class="content flex-grow-1 position-relative">
      <div class="background-overlay"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-indigo"></div>
      <div class="glow-orb orb-rose"></div>
      <div class="tech-grid-subtle"></div>

      <AppNavbar />

      <!-- ÉCRAN DE CHARGEMENT IA -->
      <div v-if="isLoading" class="loader-overlay">
        <div class="tech-loader-container">
          <div class="tech-spinner"></div>
          <div class="loader-text">SYNCHRONISATION NEURALE EN COURS...</div>
          <div class="loader-sub">L'IA analyse les patterns comportementaux</div>
        </div>
      </div>

      <main v-else class="p-4 main-viewport animate-fade-in">

        <!-- ── HEADER DYNAMIQUE ── -->
        <div class="page-header">
          <div>
            <nav class="breadcrumb-cyber mb-2">
              <span class="bc-root">INTELLIGENCE</span>
              <span class="bc-sep">/</span>
              <span class="bc-current">RAPPORT_CANDIDAT_{{ analysis.id || '404' }}</span>
            </nav>
            <h2 class="main-title m-0">Analyse <span class="title-accent">IA</span></h2>
            <p class="page-sub">Type de profil : <strong>{{ analysis.profile_type }}</strong> — Généré le {{ today }}</p>
          </div>
          <div class="header-right">
            <div class="system-status">
              <span class="status-dot pulse"></span>
              <span class="status-text">Moteur sémantique actif</span>
            </div>
            <div class="score-pill">
              <span class="score-pill-label">NEURAL INDEX</span>
              <span class="score-pill-value">{{ analysis.global_score }}<span class="score-unit">/100</span></span>
            </div>
          </div>
        </div>

        <div class="row g-4 mb-4">
          <!-- ── GAUGE DYNAMIQUE ── -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 text-center h-100 gauge-card">
              <div class="card-inner-glow amber"></div>
              <h6 class="label-heading mb-3">Neural Reliability Score</h6>

              <div class="neural-gauge-wrapper mx-auto mb-3">
                <div class="deco-ring ring-1"></div>
                <div class="deco-ring ring-2"></div>
                <svg viewBox="0 0 120 120" class="neural-svg">
                  <circle class="gauge-bg" cx="60" cy="60" r="50"/>
                  <circle class="gauge-arc" cx="60" cy="60" r="50"
                    :style="{ strokeDasharray: (analysis.global_score * 3.14) + ', 314' }"/>
                </svg>
                <div class="gauge-content">
                  <div class="gauge-value-wrap">
                    <span class="gauge-value">{{ analysis.global_score }}</span>
                    <span class="gauge-percent">%</span>
                  </div>
                  <span class="gauge-label">FIABILITÉ</span>
                  <div class="gauge-tier">{{ analysis.neural_tier }}</div>
                </div>
              </div>

              <div class="badge-profile mb-3">
                <span class="badge-dot"></span> {{ analysis.profile_type }}
              </div>

              <div class="gauge-mini-stats">
                <div class="mini-stat">
                  <span class="mini-val">{{ analysis.traits.length }}</span>
                  <span class="mini-lbl">Vecteurs</span>
                </div>
                <div class="mini-stat-div"></div>
                <div class="mini-stat">
                  <span class="mini-val">TOP {{ 100 - analysis.global_score }}%</span>
                  <span class="mini-lbl">Percentile</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ── RADAR CHART DYNAMIQUE ── -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 h-100">
              <div class="card-inner-glow indigo"></div>
              <h6 class="label-heading mb-3">Carte Cognitive</h6>
              <div class="radar-container">
                <svg viewBox="0 0 300 280" class="radar-svg">
                  <polygon v-for="n in 4" :key="n" :points="getRadarRing(n * 25)" fill="none" stroke="rgba(148,163,184,0.1)"/>
                  <line v-for="(axis, i) in radarAxes" :key="i" :x1="cx" :y1="cy" :x2="axis.x" :y2="axis.y" stroke="rgba(148,163,184,0.2)"/>
                  <polygon :points="radarDataPoints" fill="rgba(79,70,229,0.2)" stroke="#4f46e5" stroke-width="2"/>
                  <text v-for="(axis, i) in radarAxes" :key="'l'+i" :x="axis.lx" :y="axis.ly" text-anchor="middle" class="radar-label">{{ axis.label }}</text>
                </svg>
              </div>
            </div>
          </div>

          <!-- ── TRAITS DYNAMIQUES ── -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 h-100">
              <h6 class="label-heading mb-3">Dimensions Analysées</h6>
              <div class="traits-list">
                <div v-for="trait in analysis.traits" :key="trait.name" class="trait-row">
                  <div class="trait-row-left">
                    <div class="trait-icon-wrap" :style="{ background: trait.color + '22', color: trait.color }">
                      <i :class="trait.icon"></i>
                    </div>
                    <div class="trait-info">
                      <span class="trait-name">{{ trait.name }}</span>
                      <span class="trait-level" :style="{ color: trait.color }">{{ getLevel(trait.val) }}</span>
                    </div>
                  </div>
                  <div class="trait-row-right">
                    <span class="score-val">{{ trait.val }}%</span>
                    <div class="progress-cyber"><div class="progress-fill" :style="{ width: trait.val + '%', background: trait.color }"></div></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- ── TERMINAL DYNAMIQUE ── -->
        <div class="row g-4">
          <div class="col-lg-12">
            <div class="ai-insight-terminal p-4">
              <div class="terminal-titlebar mb-3">
                <span class="terminal-label">LOGS_SYSTÈME_IA // ANALYSE_TEMPS_RÉEL</span>
              </div>
              <div class="terminal-body">
                <div class="t-line" v-for="(line, i) in analysis.terminal_insights" :key="i">
                  <span class="t-time">[{{ line.time }}]</span>
                  <span class="t-prompt" :class="line.type">›</span>
                  <span class="t-text" v-html="line.text"></span>
                </div>
                <div class="t-cursor-line">
                  <span class="t-prompt green">›</span>
                  <span class="t-cursor">█</span>
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
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

// --- ÉTATS ---
const isLoading = ref(true);
const analysis = ref(null);
const today = new Date().toLocaleDateString('fr-FR', { day: '2-digit', month: 'long', year: 'numeric' });

// --- RADAR CONFIG ---
const cx = 150, cy = 140, maxR = 95;

// --- APPEL IA ---
const fetchAnalysis = async () => {
  isLoading.value = true;
  try {
    const fd = new FormData();
    // Ces données viendraient normalement de votre sélection de candidat
    fd.append('nom', 'Candidat Alpha');
    fd.append('scores_techniques', 'Frontend: 92, Backend: 70, Algorithmes: 85, SQL: 40');

    const response = await axios.post('http://127.0.0.1:8000/ia/analyze-candidate', fd);
    analysis.value = response.data;
  } catch (error) {
    console.error("Erreur IA:", error);
  } finally {
    isLoading.value = false;
  }
};

onMounted(fetchAnalysis);

// --- LOGIQUE RADAR ---
const radarAxes = computed(() => {
  if (!analysis.value) return [];
  return analysis.value.radar_data.map((d, i) => {
    const angle = (i * 2 * Math.PI / analysis.value.radar_data.length) - Math.PI / 2;
    return {
      x: cx + maxR * Math.cos(angle),
      y: cy + maxR * Math.sin(angle),
      lx: cx + (maxR + 25) * Math.cos(angle),
      ly: cy + (maxR + 25) * Math.sin(angle),
      label: d.label
    };
  });
});

const radarDataPoints = computed(() => {
  if (!analysis.value) return "";
  return analysis.value.radar_data.map((d, i) => {
    const angle = (i * 2 * Math.PI / analysis.value.radar_data.length) - Math.PI / 2;
    const r = maxR * d.val / 100;
    return `${cx + r * Math.cos(angle)},${cy + r * Math.sin(angle)}`;
  }).join(' ');
});

const getRadarRing = (radiusPct) => {
  if (!analysis.value) return "";
  const r = maxR * radiusPct / 100;
  return analysis.value.radar_data.map((_, i) => {
    const angle = (i * 2 * Math.PI / analysis.value.radar_data.length) - Math.PI / 2;
    return `${cx + r * Math.cos(angle)},${cy + r * Math.sin(angle)}`;
  }).join(' ');
};

const getLevel = (v) => v > 85 ? 'EXPERT' : v > 60 ? 'AVANCÉ' : 'DÉBUTANT';

</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&family=JetBrains+Mono:wght@500&display=swap');

.admin-body { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; }

/* --- LOADER STYLE --- */
.loader-overlay { position: fixed; inset: 0; background: #fff; z-index: 9999; display: flex; align-items: center; justify-content: center; }
.tech-loader-container { text-align: center; }
.tech-spinner { width: 60px; height: 60px; border: 4px solid #f1f5f9; border-top: 4px solid #4f46e5; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px; }
.loader-text { font-weight: 800; color: #0f172a; letter-spacing: 2px; font-size: 14px; }
@keyframes spin { to { transform: rotate(360deg); } }

/* --- BACKGROUND --- */
.background-overlay { position: fixed; inset: 0; background: linear-gradient(160deg, #f9fafb 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: fixed; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.1; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }

/* --- GAUGE & CHARTS --- */
.glass-surface { background: rgba(255,255,255,0.8); backdrop-filter: blur(20px); border: 1px solid #fff; border-radius: 28px; box-shadow: 0 10px 30px rgba(0,0,0,0.02); }
.neural-gauge-wrapper { position: relative; width: 180px; height: 180px; }
.neural-svg { transform: rotate(-90deg); width: 100%; height: 100%; }
.gauge-bg { fill: none; stroke: #f1f5f9; stroke-width: 8; }
.gauge-arc { fill: none; stroke: #f59e0b; stroke-width: 8; stroke-linecap: round; transition: 1.5s ease-out; }
.gauge-content { position: absolute; inset: 0; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.gauge-value { font-size: 40px; font-weight: 900; color: #0f172a; }

/* --- TRAITS --- */
.trait-row { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; background: #f8fafc; padding: 10px; border-radius: 12px; }
.trait-icon-wrap { width: 35px; height: 35px; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.progress-cyber { height: 6px; background: #e2e8f0; border-radius: 10px; width: 100px; margin-top: 5px; }
.progress-fill { height: 100%; border-radius: 10px; transition: 1s; }

/* --- TERMINAL --- */
.ai-insight-terminal { background: #0f172a; border-radius: 20px; color: #fff; font-family: 'JetBrains Mono', monospace; }
.t-line { font-size: 13px; margin-bottom: 5px; display: flex; gap: 10px; }
.t-time { color: rgba(255,255,255,0.3); }
.t-prompt.green { color: #22c55e; }
.t-prompt.amber { color: #f59e0b; }
.t-text { color: rgba(255,255,255,0.7); }

.animate-fade-in { animation: fadeIn 0.8s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
</style>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800;900&family=JetBrains+Mono:wght@400;500;700&display=swap');

/* ─── ROOT ─────────────────────────────────────────── */
.admin-body {
  min-height: 100vh;
  background-color: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  overflow-x: hidden;
}
.content { position: relative; }

/* ─── BG ───────────────────────────────────────────── */
.background-overlay {
  position: fixed; inset: 0; z-index: 0;
  background:
    radial-gradient(ellipse 70% 50% at 75% 0%, rgba(251,191,36,0.07) 0%, transparent 60%),
    radial-gradient(ellipse 60% 60% at 5% 100%, rgba(79,70,229,0.07) 0%, transparent 60%),
    linear-gradient(160deg, #f9fafb 0%, #f1f5f9 60%, #eef0f7 100%);
}
.tech-grid-subtle {
  position: fixed; inset: 0; z-index: 1;
  background-image:
    linear-gradient(rgba(148,163,184,0.055) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148,163,184,0.055) 1px, transparent 1px);
  background-size: 48px 48px;
}
.glow-orb { position: fixed; border-radius: 50%; filter: blur(130px); z-index: 1; pointer-events: none; }
.orb-amber  { width:500px;height:500px;background:radial-gradient(circle,#fde68a,#fbbf24 50%,transparent);top:-12%;right:-6%;opacity:.14;animation:orb-drift 18s ease-in-out infinite; }
.orb-indigo { width:550px;height:550px;background:radial-gradient(circle,#c7d2fe,#4f46e5 50%,transparent);bottom:-12%;left:-7%;opacity:.09;animation:orb-drift 24s ease-in-out infinite reverse; }
.orb-rose   { width:300px;height:300px;background:radial-gradient(circle,#fecdd3,#fda4af 50%,transparent);top:40%;left:30%;opacity:.05;animation:orb-drift 20s ease-in-out infinite 5s; }
@keyframes orb-drift {
  0%,100%{transform:translateY(0) translateX(0) scale(1)}
  33%{transform:translateY(-30px) translateX(15px) scale(1.04)}
  66%{transform:translateY(15px) translateX(-10px) scale(.97)}
}
.main-viewport { position: relative; z-index: 10; }

/* ─── ENTRY ANIMATIONS ─────────────────────────────── */
.animate-entry {
  animation: entry-up 0.55s cubic-bezier(0.165, 0.84, 0.44, 1) both;
  animation-delay: var(--delay, 0s);
}
@keyframes entry-up {
  from { opacity: 0; transform: translateY(24px); }
  to   { opacity: 1; transform: translateY(0); }
}

/* ─── HEADER ───────────────────────────────────────── */
.page-header {
  display: flex; justify-content: space-between; align-items: flex-start;
  margin-bottom: 2rem; gap: 20px; flex-wrap: wrap;
}
.breadcrumb-cyber {
  font-family: 'JetBrains Mono', monospace;
  font-size: 10px; color: #94a3b8; letter-spacing: 2px;
  display: flex; align-items: center; gap: 8px;
}
.bc-root { color: #cbd5e1; }
.bc-sep  { color: #e2e8f0; }
.bc-current { color: #64748b; }
.main-title {
  font-size: clamp(1.8rem, 3vw, 2.6rem);
  font-weight: 900; color: #0f172a;
  letter-spacing: -1.5px; line-height: 1;
}
.title-accent {
  background: linear-gradient(135deg, #4f46e5, #7c3aed, #a855f7);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text;
}
.page-sub { font-size: 12px; color: #94a3b8; font-weight: 500; margin-top: 6px; }

.header-right { display: flex; flex-direction: column; align-items: flex-end; gap: 10px; }

.system-status {
  display: flex; align-items: center;
  background: rgba(255,255,255,0.75);
  backdrop-filter: blur(12px);
  border: 1px solid rgba(255,255,255,0.9);
  border-radius: 100px; padding: 7px 16px;
  box-shadow: 0 4px 16px rgba(0,0,0,0.03);
}
.status-dot {
  width: 7px; height: 7px; background: #10b981;
  border-radius: 50%; display: inline-block; margin-right: 8px;
}
.pulse { animation: statusPulse 2s infinite; }
@keyframes statusPulse {
  0%{box-shadow:0 0 0 0 rgba(16,185,129,.6)}
  70%{box-shadow:0 0 0 8px rgba(16,185,129,0)}
  100%{box-shadow:0 0 0 0 rgba(16,185,129,0)}
}
.status-text { font-size: 10px; font-weight: 800; color: #475569; letter-spacing: .8px; text-transform: uppercase; }

.score-pill {
  background: linear-gradient(135deg, #0f172a, #1e293b);
  border-radius: 20px; padding: 10px 20px;
  display: flex; flex-direction: column; align-items: center;
  box-shadow: 0 8px 24px rgba(15,23,42,.15);
}
.score-pill-label { font-size: 9px; font-weight: 800; color: #64748b; letter-spacing: 1.5px; text-transform: uppercase; }
.score-pill-value { font-size: 1.5rem; font-weight: 900; color: #fbbf24; letter-spacing: -1px; line-height: 1.1; }
.score-unit { font-size: 0.9rem; color: rgba(251,191,36,.5); }

/* ─── GLASS SURFACE ────────────────────────────────── */
.glass-surface {
  background: rgba(255,255,255,.8);
  backdrop-filter: blur(20px); -webkit-backdrop-filter: blur(20px);
  border: 1px solid rgba(255,255,255,.93);
  border-radius: 28px; position: relative; overflow: hidden;
  box-shadow: 0 10px 40px rgba(0,0,0,.03), inset 0 1px 0 rgba(255,255,255,.9);
}
.card-inner-glow { position: absolute; top: -50px; right: -50px; width: 200px; height: 200px; border-radius: 50%; pointer-events: none; }
.card-inner-glow.amber  { background: radial-gradient(circle, rgba(251,191,36,.14) 0%, transparent 65%); }
.card-inner-glow.indigo { background: radial-gradient(circle, rgba(79,70,229,.12) 0%, transparent 65%); }
.card-inner-glow.rose   { background: radial-gradient(circle, rgba(253,164,175,.15) 0%, transparent 65%); }
.label-heading { font-size: 11px; font-weight: 800; color: #64748b; letter-spacing: 1.5px; text-transform: uppercase; }
.vectors-badge {
  font-size: 9px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; text-transform: uppercase;
  background: #f8fafc; border: 1px solid #e2e8f0; padding: 4px 10px; border-radius: 7px;
}

/* ─── GAUGE ────────────────────────────────────────── */
.neural-gauge-wrapper {
  position: relative; width: 200px; height: 200px;
}

/* Decorative rings */
.deco-ring {
  position: absolute; border-radius: 50%;
  border: 1px solid rgba(251,191,36,.1);
}
.ring-1 { inset: -8px;  animation: ring-spin 20s linear infinite; }
.ring-2 { inset: -18px; border-style: dashed; animation: ring-spin 30s linear infinite reverse; opacity: .5; }
.ring-3 { inset: -30px; border-color: rgba(79,70,229,.07); animation: ring-spin 40s linear infinite; }
@keyframes ring-spin { to { transform: rotate(360deg); } }

.neural-svg { transform: rotate(-90deg); width: 100%; height: 100%; }

.neural-svg circle { fill: none; stroke-linecap: round; }
.gauge-bg       { stroke: #f1f5f9; stroke-width: 8; }
.gauge-glow-ring { stroke: rgba(251,191,36,.15); stroke-width: 14; }
.gauge-arc      {
  stroke: #f59e0b; stroke-width: 8;
  transition: stroke-dasharray 1.4s cubic-bezier(0.165, 0.84, 0.44, 1);
  filter: drop-shadow(0 0 8px rgba(245,158,11,.6));
}
.gauge-cap {
  fill: #fbbf24; stroke: white; stroke-width: 2;
  filter: drop-shadow(0 0 4px rgba(251,191,36,.8));
}

.gauge-content {
  position: absolute; inset: 0;
  display: flex; flex-direction: column;
  align-items: center; justify-content: center; gap: 2px;
}
.gauge-value-wrap { display: flex; align-items: flex-start; gap: 2px; }
.gauge-value { font-size: 3.4rem; font-weight: 900; color: #0f172a; line-height: 1; letter-spacing: -3px; }
.gauge-percent { font-size: 1.2rem; font-weight: 700; color: #f59e0b; margin-top: 8px; }
.gauge-label { font-size: 9px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; text-transform: uppercase; }
.gauge-tier {
  font-size: 9px; font-weight: 800; letter-spacing: 1.5px;
  background: linear-gradient(90deg, #fbbf24, #f59e0b);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text;
}

/* Orbiting dots */
.orbit-dot {
  position: absolute; top: 2px; left: 50%;
  width: 9px; height: 9px; margin-left: -4.5px;
  background: #fbbf24; border-radius: 50%;
  box-shadow: 0 0 12px rgba(251,191,36,.8);
  transform-origin: 0 98px;
  animation: orbit 4s linear infinite;
}
.orbit-dot-2 {
  width: 5px; height: 5px; margin-left: -2.5px;
  background: #a78bfa;
  box-shadow: 0 0 8px rgba(167,139,250,.8);
  transform-origin: 0 98px;
  animation: orbit 7s linear infinite reverse;
  top: 4px;
}
@keyframes orbit { to { transform: rotate(360deg); } }

.badge-profile {
  background: #0f172a; color: #fbbf24;
  padding: 7px 20px; border-radius: 100px;
  font-size: 10.5px; font-weight: 800;
  display: inline-flex; align-items: center; gap: 7px;
  letter-spacing: .8px;
  box-shadow: 0 6px 16px rgba(15,23,42,.15);
}
.badge-dot { width: 6px; height: 6px; background: #fbbf24; border-radius: 50%; animation: statusPulse 2s infinite; }

.gauge-mini-stats {
  display: flex; align-items: center; justify-content: center;
  background: #f8fafc; border-radius: 16px; padding: 10px 14px; margin-top: 12px;
  gap: 0;
}
.mini-stat { text-align: center; flex: 1; }
.mini-val { display: block; font-size: 14px; font-weight: 800; color: #0f172a; letter-spacing: -.5px; }
.mini-lbl { display: block; font-size: 9px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: .8px; margin-top: 2px; }
.mini-stat-div { width: 1px; height: 28px; background: #e2e8f0; margin: 0 10px; }

.gauge-quote {
  font-size: 11.5px; font-style: italic; color: #64748b; line-height: 1.6;
  border-left: 2px solid rgba(245,158,11,.3); padding-left: 12px; text-align: left;
}

/* ─── RADAR ────────────────────────────────────────── */
.radar-container { display: flex; justify-content: center; }
.radar-svg { width: 100%; max-width: 300px; }
.radar-label {
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 10px; font-weight: 800; fill: #64748b;
  text-transform: uppercase; letter-spacing: .8px;
}
.radar-val {
  font-family: 'JetBrains Mono', monospace;
  font-size: 9px; font-weight: 700; fill: #4f46e5;
}

/* ─── TRAIT ROWS ───────────────────────────────────── */
.traits-list { display: flex; flex-direction: column; gap: 12px; }

.trait-row {
  display: flex; justify-content: space-between; align-items: center;
  background: rgba(255,255,255,.7); border: 1px solid #f1f5f9;
  border-radius: 16px; padding: 12px 14px;
  transition: all .3s cubic-bezier(.165,.84,.44,1);
  animation: entry-up .45s both;
}
.trait-row:hover {
  transform: translateX(4px);
  border-color: rgba(79,70,229,.2);
  box-shadow: 0 6px 20px rgba(79,70,229,.06);
}
.trait-row-left  { display: flex; align-items: center; gap: 10px; }
.trait-row-right { display: flex; flex-direction: column; align-items: flex-end; gap: 6px; min-width: 80px; }

.trait-icon-wrap {
  width: 32px; height: 32px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center; font-size: 13px; flex-shrink: 0;
}
.trait-info { display: flex; flex-direction: column; gap: 2px; }
.trait-name  { font-size: 12px; font-weight: 800; color: #0f172a; letter-spacing: .3px; }
.trait-level { font-size: 9px; font-weight: 800; letter-spacing: 1px; text-transform: uppercase; }

.score-val { font-family: 'JetBrains Mono', monospace; font-size: 16px; font-weight: 700; }
.score-val small { font-size: 11px; opacity: .7; }

.progress-wrap { width: 80px; }
.progress-cyber { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill {
  height: 100%; border-radius: 10px; position: relative;
  transition: width 1.2s cubic-bezier(.165,.84,.44,1);
}
.progress-shine {
  position: absolute; top: 0; right: 0; width: 12px; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,.7));
}

/* ─── TERMINAL ─────────────────────────────────────── */
.ai-insight-terminal {
  background: linear-gradient(135deg, #080f1e 0%, #0f172a 40%, #130a2a 80%, #0f172a 100%);
  border-radius: 28px;
  border: 1px solid rgba(255,255,255,.06);
  box-shadow: 0 32px 64px rgba(15,23,42,.25), inset 0 1px 0 rgba(255,255,255,.04);
}
.terminal-corner {
  position: absolute; width: 12px; height: 12px;
  border-color: rgba(79,70,229,.3); border-style: solid;
}
.terminal-corner.tl { top: 12px; left: 12px; border-width: 1px 0 0 1px; }
.terminal-corner.tr { top: 12px; right: 12px; border-width: 1px 1px 0 0; }
.terminal-corner.bl { bottom: 12px; left: 12px; border-width: 0 0 1px 1px; }
.terminal-corner.br { bottom: 12px; right: 12px; border-width: 0 1px 1px 0; }

.terminal-noise {
  position: absolute; inset: 0; border-radius: 28px;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e");
  opacity: .018; pointer-events: none;
}
.scanner-line {
  position: absolute; top: 0; left: 0; width: 100%; height: 1px;
  background: linear-gradient(90deg, transparent 0%, rgba(234,179,8,.5) 40%, rgba(234,179,8,.8) 50%, rgba(234,179,8,.5) 60%, transparent 100%);
  animation: scan 6s linear infinite;
}
@keyframes scan {
  0%{top:0%;opacity:0}5%{opacity:1}95%{opacity:.3}100%{top:100%;opacity:0}
}

.terminal-titlebar {
  display: flex; align-items: center; gap: 0;
  padding: 16px 20px 12px;
  border-bottom: 1px solid rgba(255,255,255,.05);
}
.t-dots { display: flex; gap: 5px; margin-right: 14px; }
.terminal-dot { width: 10px; height: 10px; border-radius: 50%; }
.terminal-dot.red   { background: #ef4444; }
.terminal-dot.amber { background: #f59e0b; }
.terminal-dot.green { background: #22c55e; }
.terminal-label {
  flex: 1;
  font-family: 'JetBrains Mono', monospace;
  font-size: 10px; color: rgba(255,255,255,.2); letter-spacing: 1px;
}
.t-status-online {
  display: flex; align-items: center; gap: 5px;
  font-size: 9px; font-weight: 800; color: #22c55e; letter-spacing: 1px;
}
.t-online-dot { width: 5px; height: 5px; background: #22c55e; border-radius: 50%; animation: statusPulse 2s infinite; }

.terminal-body { padding: 18px 20px 20px; display: flex; flex-direction: column; gap: 4px; }

.t-line-anim {
  display: flex; align-items: baseline; gap: 8px;
  animation: entry-up .4s both;
}
.t-time {
  font-family: 'JetBrains Mono', monospace;
  font-size: 10px; color: rgba(255,255,255,.18);
  min-width: 68px; flex-shrink: 0;
}
.t-prompt { font-family: 'JetBrains Mono', monospace; font-size: 13px; font-weight: 700; flex-shrink: 0; }
.t-prompt.green { color: #22c55e; }
.t-prompt.blue  { color: #60a5fa; }
.t-prompt.amber { color: #f59e0b; }
.t-text {
  font-family: 'JetBrains Mono', monospace;
  font-size: 11.5px; color: rgba(255,255,255,.45); line-height: 1.8;
}
.t-hi    { color: rgba(255,255,255,.85); font-weight: 600; }
.t-ok    { color: #22c55e; font-weight: 700; }
.t-amber { color: #fbbf24; font-weight: 600; }

.t-cursor-line { display: flex; align-items: center; gap: 8px; margin-top: 4px; }
.t-cursor {
  font-family: 'JetBrains Mono', monospace;
  font-size: 14px; color: #4f46e5;
  animation: blink-cursor 1s step-end infinite;
}
@keyframes blink-cursor { 0%,100%{opacity:1} 50%{opacity:0} }

/* ─── TIMELINE ─────────────────────────────────────── */
.timeline { display: flex; flex-direction: column; }
.timeline-item {
  display: flex; gap: 14px;
  animation: entry-up .45s both;
}
.tl-dot-col { display: flex; flex-direction: column; align-items: center; flex-shrink: 0; padding-top: 3px; }
.tl-dot { width: 12px; height: 12px; border-radius: 50%; flex-shrink: 0; }
.tl-line { flex: 1; width: 1px; background: linear-gradient(to bottom, rgba(148,163,184,.3), rgba(148,163,184,.05)); margin: 6px 0; min-height: 30px; }
.tl-content { flex: 1; padding-bottom: 22px; }
.tl-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 3px; }
.tl-title { font-size: 13px; font-weight: 800; color: #0f172a; }
.tl-badge { font-size: 11px; font-weight: 800; padding: 3px 10px; border-radius: 8px; font-family: 'JetBrains Mono', monospace; }
.tl-date  { font-size: 10px; color: #94a3b8; font-weight: 600; display: block; margin-bottom: 5px; letter-spacing: .3px; }
.tl-desc  { font-size: 12px; color: #64748b; line-height: 1.55; margin: 0; }

/* ─── SCROLLBAR ────────────────────────────────────── */
::-webkit-scrollbar { width: 6px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
::-webkit-scrollbar-thumb:hover { background: #fbbf24; }

/* ─── RESPONSIVE ───────────────────────────────────── */
@media (max-width: 768px) {
  .glass-surface { border-radius: 20px; }
  .ai-insight-terminal { border-radius: 20px; }
  .neural-gauge-wrapper { width: 160px; height: 160px; }
  .gauge-value { font-size: 2.6rem; }
  .page-header { flex-direction: column; align-items: flex-start; }
  .header-right { flex-direction: row; align-items: center; flex-wrap: wrap; }
}
</style>