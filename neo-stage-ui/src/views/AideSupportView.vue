<template>
  <div class="admin-dashboard-container">
    <!-- COUCHES D'ARRIÈRE-PLAN (Cohérence visuelle) -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>
    <div class="tech-grid-subtle"></div>

    <div class="d-flex position-relative z-index-10">
      <AppSidebar />

      <div class="content flex-grow-1">
        <AppNavbar />

        <div class="p-4 pt-0 main-viewport animate__animated animate__fadeIn">
          
          <!-- HEADER SECTION -->
          <div class="text-center mb-5 mt-4">
            <div class="badge-tech mb-2">
              <span class="pulse-dot"></span> HUB DE SUPPORT PRIORITAIRE
            </div>
            <h1 class="display-title">Centre <span>d'Assistance</span></h1>
            <p class="brand-subtitle text-uppercase">Comment pouvons-nous optimiser votre expérience aujourd'hui ?</p>
          </div>

          <!-- FAQ GRID (Style Matrix) -->
          <div class="row g-4">
            <div class="col-md-4" v-for="topic in faq" :key="topic.title">
              <div class="glass-surface p-4 h-100 text-center hover-tech-card">
                <div class="icon-pulse-amber mx-auto mb-4">
                  <i :class="topic.icon"></i>
                </div>
                <h6 class="label-heading mb-2 text-dark">{{ topic.title }}</h6>
                <p class="small text-slate-500 mb-4">{{ topic.desc }}</p>
                <button class="btn-outline-tech-sm w-100">
                  CONSULTER LE GUIDE <i class="fa-solid fa-chevron-right ms-2"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- IA SUPPORT CTA (Style Deep Navy Insight) -->
          <div class="mt-5">
            <div class="ai-support-box p-5 rounded-5 overflow-hidden position-relative">
              <div class="cyber-scan-line"></div>
              <div class="row align-items-center position-relative">
                <div class="col-lg-2 text-center mb-4 mb-lg-0">
                  <div class="bot-wrapper">
                    <svg viewBox="0 0 200 200" width="100" class="floating-bot">
                      <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#eab308" stroke-width="2"/>
                      <rect x="65" y="65" width="70" height="32" rx="16" fill="#0f172a" />
                      <circle cx="85" cy="81" r="5" fill="#eab308" />
                      <circle cx="115" cy="81" r="5" fill="#eab308" />
                    </svg>
                  </div>
                </div>
                <div class="col-lg-6 text-center text-lg-start">
                  <h4 class="fw-800 text-white mb-2">BESOIN D'UNE RÉPONSE IMMÉDIATE ?</h4>
                  <p class="text-white-50 m-0 italic">"Notre IA de support analyse votre environnement en temps réel pour une assistance technique 24/7."</p>
                </div>
                <div class="col-lg-4 text-center text-lg-end mt-4 mt-lg-0">
                  <button class="btn-primary-gradient px-5 py-3">
                    OUVRIR LE TERMINAL CHAT <i class="fa-solid fa-terminal ms-2"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- FOOTER STATUS -->
          <div class="text-center mt-5 pb-4">
             <span class="status-indicator-pill">
               <span class="dot-online"></span> TOUS LES SYSTÈMES SONT OPÉRATIONNELS
             </span>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const faq = [
  { icon: 'fa-solid fa-shield-halved', title: 'SÉCURITÉ & PRIVACY', desc: 'Protocoles de cryptage et conformité RGPD pour vos données candidat.' },
  { icon: 'fa-solid fa-wand-magic-sparkles', title: 'IA GÉNÉRATIVE', desc: 'Guide avancé pour le paramétrage des modèles de questions IA.' },
  { icon: 'fa-solid fa-credit-card', title: 'GESTION COMPTABLE', desc: 'Suivi de consommation de tokens IA, abonnements et facturation.' }
];
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

/* --- BACKGROUND DECORATIONS --- */
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px); background-size: 45px 45px; z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.15; }
.orb-amber { width: 400px; height: 400px; background: #fbbf24; top: -10%; right: -5%; }
.orb-indigo { width: 500px; height: 500px; background: #4f46e5; bottom: -10%; left: -5%; }
.z-index-10 { z-index: 10; }

/* --- TYPOGRAPHY --- */
.display-title { font-weight: 800; color: #0f172a; font-size: 42px; margin: 0; letter-spacing: -2px; }
.display-title span { color: #eab308; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 700; letter-spacing: 2px; }
.label-heading { font-size: 13px; font-weight: 800; letter-spacing: 0.5px; text-transform: uppercase; }

/* --- FAQ CARDS --- */
.glass-surface {
  background: rgba(255, 255, 255, 0.75);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 24px;
  box-shadow: 0 10px 30px -10px rgba(0,0,0,0.04);
  transition: all 0.3s ease;
}
.hover-tech-card:hover {
  transform: translateY(-10px);
  border-color: #eab308;
  background: white;
}

.icon-pulse-amber {
  width: 65px; height: 65px; background: #fffbeb; color: #eab308;
  border-radius: 20px; display: flex; align-items: center; justify-content: center;
  font-size: 26px; border: 1px solid #fef3c7;
}

.btn-outline-tech-sm {
  background: transparent; border: 2px solid #f1f5f9; color: #64748b;
  font-size: 10px; font-weight: 800; padding: 12px; border-radius: 12px;
  transition: 0.3s;
}
.hover-tech-card:hover .btn-outline-tech-sm {
  border-color: #0f172a; color: #0f172a;
}

/* --- AI SUPPORT CTA --- */
.ai-support-box {
  background: linear-gradient(135deg, #0f172a 0%, #1e1b4b 100%);
  border-radius: 32px;
  box-shadow: 0 25px 50px -12px rgba(15, 23, 42, 0.3);
}

.cyber-scan-line {
  position: absolute; top: 0; left: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, #eab308, transparent);
  animation: scanLine 4s linear infinite;
}
@keyframes scanLine { 0% { top: 0; } 100% { top: 100%; } }

.floating-bot { animation: float 3s ease-in-out infinite; }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-10px); } }

/* --- BUTTONS --- */
.btn-primary-gradient {
  background: linear-gradient(135deg, #eab308 0%, #facc15 100%);
  color: #fff; border: none; border-radius: 16px;
  font-weight: 800; font-size: 14px;
  box-shadow: 0 10px 20px -5px rgba(234, 179, 8, 0.3);
  transition: 0.3s;
}
.btn-primary-gradient:hover { transform: scale(1.05); box-shadow: 0 15px 30px rgba(234, 179, 8, 0.4); }

/* --- STATUS PILL --- */
.status-indicator-pill {
  background: #f1f5f9; padding: 8px 20px; border-radius: 100px;
  font-size: 10px; font-weight: 800; color: #64748b; border: 1px solid #e2e8f0;
}
.dot-online {
  width: 8px; height: 8px; background: #10b981; border-radius: 50%;
  display: inline-block; margin-right: 8px;
}

.badge-tech {
  display: inline-flex; align-items: center; gap: 8px; background: white;
  padding: 4px 12px; border-radius: 100px; font-size: 9px; font-weight: 800;
  border: 1px solid #e2e8f0; color: #64748b;
}
.pulse-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; animation: blink 1s infinite; }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }

.italic { font-style: italic; }
.fw-800 { font-weight: 800; }
</style>