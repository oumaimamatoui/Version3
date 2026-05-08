<template>
  <div class="support-container">
    <!-- COUCHES D'ARRIÈRE-PLAN LIGHT TECH -->
    <div class="neural-bg">
      <div class="glow-orb orb-1"></div>
      <div class="glow-orb orb-2"></div>
      <div class="cyber-grid"></div>
    </div>

    <div class="d-flex position-relative z-index-10">
      <AppSidebar />

      <div class="content flex-grow-1">
        <AppNavbar />

        <div class="main-viewport p-4 pt-0">
          
          <!-- HEADER SECTION (LIGHT VERSION) -->
          <div class="support-header text-center mb-5 animate-in">
            <nav class="breadcrumb-cyber mb-2">SYSTEM / NETWORK / ASSISTANCE_HUB</nav>
            <h1 class="glitch-title">Support <span>Cognitif</span></h1>
            <p class="text-slate-500">Initialisation du protocole d'aide. Comment pouvons-nous optimiser votre session ?</p>
            
            <!-- Barre de recherche HUD -->
            <div class="search-vessel mx-auto mt-4 shadow-sm">
              <i class="fa-solid fa-magnifying-glass ms-3 text-amber"></i>
              <input type="text" placeholder="Rechercher une solution dans le registre..." class="cyber-input">
              <kbd class="d-none d-md-block me-3">CMD + K</kbd>
            </div>
          </div>

          <!-- GRILLE DE TOPICS (Matrix Light) -->
          <div class="row g-4 animate-staggered">
            <div class="col-lg-4 col-md-6" v-for="(topic, index) in faq" :key="topic.title" :style="{'--delay': index * 0.1 + 's'}">
              <div class="cyber-faq-card glass-surface">
                <div class="card-glow"></div>
                <div class="vessel-content">
                  <div class="icon-vessel mb-4">
                    <div class="pulse-aura"></div>
                    <i :class="topic.icon"></i>
                  </div>
                  <h5 class="fw-bold text-navy mb-2">{{ topic.title }}</h5>
                  <p class="small text-slate-500 mb-4">{{ topic.desc }}</p>
                  <button class="btn-outline-tech w-100">
                    ACCÉDER AU REGISTRE <i class="fa-solid fa-chevron-right ms-2"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- AI NEURAL CHAT BANNER (Contrast Premium) -->
          <div class="mt-5 pt-4 animate-in-up">
            <div class="neural-chat-banner">
              <div class="scanner-line"></div>
              <div class="row align-items-center position-relative">
                <div class="col-lg-2 text-center mb-4 mb-lg-0">
                  <div class="ai-vessel">
                    <div class="orbit"></div>
                    <svg viewBox="0 0 200 200" width="80" class="floating-bot">
                      <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#eab308" stroke-width="2"/>
                      <rect x="65" y="65" width="70" height="32" rx="16" fill="#0f172a" />
                      <circle cx="85" cy="81" r="5" fill="#eab308" />
                      <circle cx="115" cy="81" r="5" fill="#eab308" />
                    </svg>
                  </div>
                </div>
                <div class="col-lg-6 text-center text-lg-start">
                  <div class="status-badge mb-2">
                    <span class="ping-dot"></span> MOTEUR IA OPÉRATIONNEL
                  </div>
                  <h3 class="fw-black text-white m-0">BESOIN D'UNE RÉPONSE IMMÉDIATE ?</h3>
                  <p class="text-white-50 mt-2 mb-0 italic">"Notre IA de support analyse votre environnement pour une assistance technique 24/7."</p>
                </div>
                <div class="col-lg-4 text-center text-lg-end mt-4 mt-lg-0">
                  <button class="btn-primary-gradient px-5 py-3">
                    TERMINAL CHAT <i class="fa-solid fa-terminal ms-2"></i>
                  </button>
                </div>
              </div>
            </div>
          </div>

          <!-- FOOTER STATUS -->
          <div class="mt-5 text-center pb-4">
            <div class="status-indicator-pill">
               <span class="dot-online"></span> ALL_SYSTEMS_OPERATIONAL [v4.2]
            </div>
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
  { icon: 'fa-solid fa-shield-halved text-indigo', title: 'SÉCURITÉ & PRIVACY', desc: 'Protocoles de cryptage et conformité RGPD pour vos données.' },
  { icon: 'fa-solid fa-wand-magic-sparkles text-amber', title: 'INTELLIGENCE ARTIFICIELLE', desc: 'Guide avancé pour le paramétrage des modèles de questions IA.' },
  { icon: 'fa-solid fa-credit-card text-success', title: 'FACTURATION & PLANS', desc: 'Gestion comptable, suivi des tokens et abonnements SaaS.' },
  { icon: 'fa-solid fa-users-gear text-primary', title: 'GESTION DES ÉQUIPES', desc: 'Administration des accès personnels et rôles évaluateurs.' },
  { icon: 'fa-solid fa-file-code text-secondary', title: 'API & INTÉGRATION', desc: 'Connectez EvaluaTech à vos outils RH via nos Webhooks.' },
  { icon: 'fa-solid fa-graduation-cap text-warning', title: 'ACADÉMIE IA', desc: 'Formations pour devenir un expert certifié en recrutement IA.' }
];
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&family=JetBrains+Mono:wght@500;800&display=swap');

/* --- THEME CORE LIGHT --- */
.support-container {
  --navy: #0f172a;
  --bg-light: #ffffff;
  --bg-subtle: #f8fafc;
  --amber: #f59e0b;
  --amber-glow: rgba(245, 158, 11, 0.1);
  --glass: rgba(255, 255, 255, 0.8);
  
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background-color: var(--bg-light);
  color: var(--navy);
  overflow-x: hidden;
  position: relative;
}

/* --- ARRIÈRE-PLAN --- */
.neural-bg { position: absolute; inset: 0; z-index: 0; overflow: hidden; background: var(--bg-subtle); }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.15; animation: drift 15s infinite alternate; }
.orb-1 { width: 500px; height: 500px; background: var(--amber); top: -10%; right: -5%; }
.orb-2 { width: 400px; height: 400px; background: #6366f1; bottom: -5%; left: -5%; }
.cyber-grid { 
  position: absolute; inset: 0; 
  background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px);
  background-size: 50px 50px;
}

/* --- HEADER --- */
.glitch-title { font-size: 42px; font-weight: 800; letter-spacing: -2px; color: var(--navy); }
.glitch-title span { color: var(--amber); }
.breadcrumb-cyber { font-family: 'JetBrains Mono'; font-size: 10px; color: #94a3b8; letter-spacing: 2px; }

.search-vessel {
  max-width: 600px; background: white; border: 1px solid #e2e8f0;
  border-radius: 100px; display: flex; align-items: center; padding: 5px;
}
.cyber-input {
  flex: 1; background: transparent; border: none; padding: 12px 15px; color: var(--navy); outline: none; font-size: 14px;
}
kbd { background: #f1f5f9; color: #94a3b8; font-size: 10px; padding: 5px 10px; border-radius: 6px; border: 1px solid #e2e8f0; }

/* --- FAQ CARDS (LIGHT GLASS) --- */
.glass-surface {
  background: var(--glass); backdrop-filter: blur(15px);
  border: 1px solid #eef2f7; border-radius: 30px;
  box-shadow: 0 10px 25px -5px rgba(0,0,0,0.02);
}
.cyber-faq-card {
  height: 100%; padding: 35px; position: relative; overflow: hidden;
  transition: all 0.4s ease;
}
.cyber-faq-card:hover { transform: translateY(-8px); border-color: var(--amber); background: white; }

.icon-vessel {
  width: 60px; height: 60px; background: #f8fafc; border-radius: 18px;
  display: flex; align-items: center; justify-content: center; font-size: 24px;
  border: 1px solid #f1f5f9;
}

/* --- AI BANNER (STAYS DARK FOR PREMIUM CONTRAST) --- */
.neural-chat-banner {
  padding: 45px; border-radius: 40px; position: relative; overflow: hidden;
  background: linear-gradient(135deg, #0f172a, #020617);
  box-shadow: 0 25px 50px -12px rgba(15, 23, 42, 0.2);
}
.scanner-line {
  position: absolute; left: 0; top: 0; width: 100%; height: 2px;
  background: linear-gradient(90deg, transparent, var(--amber), transparent);
  animation: scan 3s linear infinite;
}

.floating-bot { animation: float 3s ease-in-out infinite; }

.status-badge {
  display: inline-flex; align-items: center; gap: 8px; font-size: 10px; font-weight: 800;
  color: var(--amber); background: rgba(245, 158, 11, 0.1); padding: 5px 12px; border-radius: 100px;
}
.ping-dot { width: 6px; height: 6px; background: #10b981; border-radius: 50%; animation: blink 1.5s infinite; }

/* --- BUTTONS --- */
.btn-primary-gradient {
  background: linear-gradient(135deg, #eab308, #facc15); color: white; border: none; border-radius: 14px;
  font-weight: 800; font-size: 13px; transition: 0.3s;
}
.btn-primary-gradient:hover { transform: scale(1.05); box-shadow: 0 10px 20px rgba(234, 179, 8, 0.3); }

.btn-outline-tech {
  background: transparent; border: 2px solid #f1f5f9; color: #64748b;
  padding: 10px; border-radius: 12px; font-size: 10px; font-weight: 800; transition: 0.3s;
}
.cyber-faq-card:hover .btn-outline-tech { border-color: var(--navy); color: var(--navy); }

/* --- STATUS PILL --- */
.status-indicator-pill {
  background: #f1f5f9; padding: 8px 20px; border-radius: 100px;
  font-size: 10px; font-weight: 800; color: #94a3b8; border: 1px solid #e2e8f0;
}
.dot-online { width: 8px; height: 8px; background: #10b981; border-radius: 50%; display: inline-block; margin-right: 8px; }

/* --- ANIMATIONS --- */
@keyframes scan { 0% { top: 0; } 100% { top: 100%; } }
@keyframes drift { from { transform: translate(0,0); } to { transform: translate(3%, 3%); } }
@keyframes blink { 50% { opacity: 0.4; } }
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-10px); } }

.z-index-10 { z-index: 10; }
.italic { font-style: italic; }
</style>