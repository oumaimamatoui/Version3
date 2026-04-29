<template>
  <div class="admin-body d-flex">
    <!-- 1. SIDEBAR -->
    <AppSidebar />

    <div class="content flex-grow-1 position-relative">
      <!-- COUCHES DÉCORATIVES -->
      <div class="background-overlay"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-indigo"></div>
      <div class="tech-grid-subtle"></div>

      <!-- 2. NAVBAR -->
      <AppNavbar />

      <main class="p-4 main-viewport animate-fade-in">
        <!-- HEADER ANALYTIQUE -->
        <div class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <nav class="breadcrumb-cyber mb-2">INTELLIGENCE / ANALYSE_COMPORTEMENTALE_V4</nav>
            <h2 class="fw-800 text-navy m-0">Analyse <span>IA</span></h2>
          </div>
          <div class="system-status">
            <span class="status-dot pulse"></span>
            <span class="status-text tiny fw-800 uppercase">Moteur sémantique actif</span>
          </div>
        </div>

        <div class="row g-4">
          <!-- 3. NEURAL INDEX (GAUGE) -->
          <div class="col-lg-4">
            <div class="glass-surface p-5 text-center h-100 border-amber-soft shadow-lg">
              <h6 class="label-heading mb-4">Neural Index Score</h6>
              <div class="neural-gauge-wrapper mx-auto mb-4">
                <svg viewBox="0 0 100 100" class="neural-svg">
                  <circle class="base" cx="50" cy="50" r="45"></circle>
                  <circle class="progress" cx="50" cy="50" r="45" :style="{ strokeDasharray: (88 * 2.83) + ', 283' }"></circle>
                </svg>
                <div class="gauge-content">
                  <span class="value">88</span>
                  <span class="label">SOFT SKILLS</span>
                </div>
              </div>
              <div class="badge-profile mb-3">PROFIL : INNOVATEUR</div>
              <p class="small text-slate-500 italic px-2">
                "Capacité d'adaptation critique et gestion du stress optimisée détectée lors des phases de résolution."
              </p>
            </div>
          </div>

          <!-- 4. TRAITS DE PERSONNALITÉ (CARDS) -->
          <div class="col-lg-8">
            <div class="glass-surface p-4 h-100 shadow-sm">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <h6 class="label-heading m-0"><i class="fa-solid fa-microchip me-2 text-indigo"></i>Dimensions Cognitives</h6>
                <span class="tiny text-muted fw-bold">BASÉ SUR 12 VECTEURS</span>
              </div>
              
              <div class="row g-3">
                <div v-for="trait in traits" :key="trait.name" class="col-md-6">
                  <div class="trait-card-premium p-3">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="trait.icon" class="text-indigo"></i>
                        <span class="small fw-800 text-navy uppercase ls-1">{{ trait.name }}</span>
                      </div>
                      <span class="score-val fw-800">{{ trait.val }}%</span>
                    </div>
                    <div class="progress-cyber">
                      <div class="progress-fill" :style="{ width: trait.val + '%' }"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- 5. INTERPRÉTATION IA (STYLE SCANNER) -->
          <div class="col-12">
            <div class="ai-insight-terminal p-5 shadow-2xl position-relative overflow-hidden">
              <div class="scanner-line"></div>
              <div class="row align-items-center">
                <div class="col-lg-1 text-center d-none d-lg-block">
                  <i class="fa-solid fa-brain-circuit fa-3x text-amber opacity-50"></i>
                </div>
                <div class="col-lg-11">
                  <h5 class="fw-800 text-white mb-2 uppercase tiny ls-1">Verdict du Moteur Neural EvaluaTech</h5>
                  <p class="m-0 text-white-50 font-mono small leading-relaxed">
                    > SCAN_COMPLETE: Le candidat manifeste une prise de décision analytique (12.4s de latence moyenne). <br>
                    > NO_HESITATION_DETECTED: Stabilité confirmée sur les segments complexes. <br>
                    > RECOMMENDATION: Aptitude élevée pour l'architecture logicielle et le leadership technique.
                  </p>
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
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const traits = [
  { name: 'Rigueur', val: 95, icon: 'fa-solid fa-bullseye' },
  { name: 'Adaptabilité', val: 82, icon: 'fa-solid fa-shuffle' },
  { name: 'Communication', val: 70, icon: 'fa-solid fa-comments' },
  { name: 'Esprit Critique', val: 88, icon: 'fa-solid fa-magnifying-glass-chart' }
];
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&family=JetBrains+Mono:wght@400;700&display=swap');

/* --- BASES & DÉCORATIONS (Identiques à vos autres vues premium) --- */
.admin-body { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; overflow: hidden; }
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.15; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }
.main-viewport { position: relative; z-index: 10; }

.fw-800 { font-weight: 800; }
.ls-1 { letter-spacing: 1px; }
.tiny { font-size: 10px; }
.uppercase { text-transform: uppercase; }
.font-mono { font-family: 'JetBrains Mono', monospace; }

/* --- NEURAL GAUGE --- */
.neural-gauge-wrapper { position: relative; width: 180px; height: 180px; }
.neural-svg { transform: rotate(-90deg); }
.neural-svg circle { fill: none; stroke-width: 8; stroke-linecap: round; }
.neural-svg circle.base { stroke: #f1f5f9; }
.neural-svg circle.progress { stroke: #eab308; transition: stroke-dasharray 1s ease; filter: drop-shadow(0 0 8px rgba(234, 179, 8, 0.3)); }

.gauge-content { position: absolute; inset: 0; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.gauge-content .value { font-size: 3rem; font-weight: 800; color: #0f172a; line-height: 1; }
.gauge-content .label { font-size: 10px; font-weight: 800; color: #94a3b8; margin-top: 5px; }

.badge-profile { background: #0f172a; color: #eab308; padding: 6px 16px; border-radius: 100px; font-size: 11px; font-weight: 800; display: inline-block; }

/* --- TRAIT CARDS --- */
.glass-surface { background: rgba(255, 255, 255, 0.75); backdrop-filter: blur(10px); border: 1px solid rgba(226, 232, 240, 0.8); border-radius: 28px; }
.trait-card-premium { background: white; border-radius: 18px; border: 1px solid #f1f5f9; transition: 0.3s; }
.trait-card-premium:hover { transform: translateY(-3px); border-color: #eab308; }
.progress-cyber { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; background: #4f46e5; border-radius: 10px; box-shadow: 0 0 10px rgba(79, 70, 229, 0.4); }

/* --- TERMINAL INSIGHT --- */
.ai-insight-box { background: #0f172a; border-radius: 24px; color: white; }
.ai-insight-terminal { 
  background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 100%); 
  border-radius: 32px; 
  border: 1px solid rgba(255,255,255,0.05); 
}
.scanner-line {
  position: absolute; top: 0; left: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, #eab308, transparent);
  animation: scan 4s linear infinite;
  opacity: 0.5;
}

@keyframes scan { 0% { top: 0; } 100% { top: 100%; } }

/* --- UTILS --- */
.breadcrumb-cyber { font-family: monospace; font-size: 10px; color: #94a3b8; letter-spacing: 2px; }
.status-dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; display: inline-block; margin-right: 8px; }
.pulse { animation: statusPulse 2s infinite; }
@keyframes statusPulse { 0% { box-shadow: 0 0 0 0 rgba(16, 185, 106, 0.4); } 70% { box-shadow: 0 0 0 8px rgba(16, 185, 106, 0); } 100% { box-shadow: 0 0 0 0 rgba(16, 185, 106, 0); } }
</style>