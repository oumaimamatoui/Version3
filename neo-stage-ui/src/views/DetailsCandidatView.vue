<template>
  <div class="elite-details-root">
    <!-- Fond Immersif Elite (Identique au Login) -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>

    <AppSidebar />

    <div class="main-content flex-grow-1">
      <AppNavbar />

      <div class="container-fluid p-4 p-lg-5 content-overlay">
        
        <!-- ÉTAT : CHARGEMENT -->
        <div v-if="loading" class="state-portal">
          <div class="loader-orbit">
             <div class="dot"></div>
             <i class="fa-solid fa-microchip ai-icon-spin"></i>
          </div>
          <p class="loading-text">INTERROGATION DES SYSTÈMES IA...</p>
        </div>

        <!-- ÉTAT : ERREUR -->
        <div v-else-if="error" class="state-portal animate__animated animate__fadeIn">
          <div class="error-glass-card">
            <i class="fa-solid fa-triangle-exclamation text-amber mb-4"></i>
            <h2 class="fw-900">Candidat Introuvable</h2>
            <p>Le protocole d'accès a échoué ou le profil est inexistant.</p>
            <button @click="$router.push('/candidates')" class="btn-sunburst mt-4">
              <i class="fa-solid fa-arrow-rotate-left me-2"></i>RETOUR AU RÉPERTOIRE
            </button>
          </div>
        </div>

        <!-- ÉTAT : SUCCÈS -->
        <div v-else-if="candidate" class="animate__animated animate__fadeInUp">
          
          <!-- Header Navigation -->
          <div class="d-flex justify-content-between align-items-center mb-5">
             <button @click="$router.back()" class="btn-back-elite">
               <i class="fa-solid fa-chevron-left"></i> <span>DASHBOARD</span>
             </button>
             <div class="elite-badge-status">
               <span class="status-indicator"></span> FICHE CANDIDAT CERTIFIÉE
             </div>
          </div>

          <div class="row g-4">
            <!-- COLONNE GAUCHE : IDENTITÉ BENTO -->
            <div class="col-lg-4">
              <div class="bento-card identity-bento text-center">
                <div class="avatar-master-wrapper mb-4">
                  <div class="avatar-squircle-elite shadow-lg">
                    {{ candidate.fullName.split(' ').map(n => n[0]).join('').toUpperCase() }}
                  </div>
                  <div class="premium-score-tag">
                    <label>GLOBAL</label>
                    <span class="val">{{ candidate.scoreGlobal }}%</span>
                  </div>
                </div>

                <div class="header-info">
                  <h2 class="candidate-title">{{ candidate.fullName }}</h2>
                  <span class="campaign-pill">{{ candidate.campaignName }}</span>
                </div>

                <div class="luxury-divider"></div>

                <div class="data-grid text-start">
                  <div class="data-item">
                    <label>COORDONNÉES</label>
                    <p><i class="fa-solid fa-at me-2 text-amber"></i>{{ candidate.email }}</p>
                  </div>
                  <div class="data-item">
                    <label>SIGNATURE NUMÉRIQUE</label>
                    <p class="text-mono">#{{ candidate.id.split('-')[0].toUpperCase() }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- COLONNE DROITE : ANALYSE & SKILLS -->
            <div class="col-lg-8">
              
              <!-- CARTE IA AVEC ROBOT COMPAGNON -->
              <div class="bento-card ai-analysis-bento mb-4">
                <div class="ai-header">
                  <!-- Intégration du Robot du Login -->
                  <div class="mini-bot">
                    <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
                      <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
                      <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
                      <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                      <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                      <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-core" />
                    </svg>
                  </div>
                  <h5 class="m-0 ms-3">RAPPORT D'ANALYSE PRÉDICTIF</h5>
                </div>
                <p class="ai-verdict-text">{{ candidate.iaVerdict }}</p>
              </div>

              <!-- CARTE SKILLS ÉLITE -->
              <div class="bento-card skills-bento">
                <div class="d-flex justify-content-between align-items-center mb-4">
                    <h6 class="section-title">MATRICE DES COMPÉTENCES</h6>
                    <i class="fa-solid fa-layer-group text-amber"></i>
                </div>

                <div v-for="(s, index) in candidate.skills" :key="s.name" 
                     class="skill-row-elite" :style="{ '--delay': (index * 0.15) + 's' }">
                  <div class="d-flex justify-content-between align-items-end mb-2">
                    <span class="skill-label">{{ s.name }}</span>
                    <span class="skill-value">{{ s.val }}%</span>
                  </div>
                  <div class="elite-progress-track">
                    <div class="elite-progress-fill" :style="{ width: s.val + '%' }">
                      <div class="glow-effect"></div>
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
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const route = useRoute();
const candidate = ref(null);
const loading = ref(true);
const error = ref(false);

const fetchCandidateDetails = async () => {
  loading.value = true;
  try {
    const token = localStorage.getItem('token');
    const res = await axios.get(`http://localhost:5172/api/Candidates/${route.params.id}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    candidate.value = res.data;
  } catch (err) {
    error.value = true;
  } finally {
    setTimeout(() => { loading.value = false; }, 600);
  }
};

onMounted(fetchCandidateDetails);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

.elite-details-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh; display: flex; position: relative;
  background: #fdfdfd; overflow-x: hidden;
}

/* --- FOND IMMERSIF (Repris du Login) --- */
.luxury-bg { position: fixed; inset: 0; z-index: 0; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.2; animation: orbit 30s infinite linear; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; left: -10%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 35%; right: 15%; opacity: 0.1; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.03; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

@keyframes orbit { from { transform: rotate(0) translate(50px) rotate(0); } to { transform: rotate(360deg) translate(50px) rotate(-360deg); } }

.content-overlay { position: relative; z-index: 2; }
.text-amber { color: #fbbf24 !important; }

/* --- BENTO CARDS --- */
.bento-card {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(20px);
  border-radius: 40px; padding: 40px;
  border: 1px solid rgba(255,255,255,0.8);
  box-shadow: 0 30px 60px -12px rgba(0,0,0,0.05);
}

/* --- IDENTITY COLUMN --- */
.avatar-master-wrapper { position: relative; display: inline-block; }
.avatar-squircle-elite {
  width: 140px; height: 140px; border-radius: 45px;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
  display: flex; align-items: center; justify-content: center;
  font-size: 42px; font-weight: 800; color: #0f172a;
  border: 4px solid white; box-shadow: 0 20px 40px rgba(0,0,0,0.05);
}
.premium-score-tag {
  position: absolute; bottom: -10px; right: -15px;
  background: #0f172a; color: white; padding: 8px 15px; border-radius: 18px;
  display: flex; flex-direction: column; line-height: 1; border: 3px solid white;
}
.premium-score-tag label { font-size: 8px; font-weight: 800; color: #fbbf24; margin-bottom: 2px; }
.premium-score-tag .val { font-size: 18px; font-weight: 900; }

.candidate-title { font-weight: 900; color: #0f172a; margin: 20px 0 10px; font-size: 28px; }
.campaign-pill {
  background: #fef3c7; color: #d97706; padding: 6px 18px; border-radius: 50px;
  font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px;
}

.luxury-divider { width: 50px; height: 4px; background: #fbbf24; margin: 30px auto; border-radius: 10px; }
.data-item label { display: block; font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; margin-bottom: 8px; }
.data-item p { font-weight: 700; color: #1e293b; font-size: 15px; }

/* --- AI BENTO --- */
.ai-analysis-bento { background: #0f172a; border-color: #1e293b; color: white; }
.ai-header { display: flex; align-items: center; margin-bottom: 25px; }
.mini-bot { width: 60px; height: 60px; filter: drop-shadow(0 0 10px rgba(251, 191, 36, 0.3)); }
.ai-header h5 { font-weight: 800; letter-spacing: 1.5px; font-size: 13px; color: #fbbf24; }
.ai-verdict-text { font-size: 16px; line-height: 1.8; color: #cbd5e1; font-weight: 400; }

/* --- SKILLS --- */
.skill-row-elite { margin-bottom: 25px; opacity: 0; animation: fadeInUp 0.5s forwards; animation-delay: var(--delay); }
.skill-label { font-weight: 800; color: #0f172a; font-size: 13px; }
.skill-value { font-weight: 900; color: #fbbf24; }
.elite-progress-track { height: 12px; background: #f1f5f9; border-radius: 20px; overflow: hidden; position: relative; }
.elite-progress-fill {
  height: 100%; background: linear-gradient(90deg, #fbbf24, #f59e0b);
  border-radius: 20px; transition: width 1.5s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.glow-effect { position: absolute; inset: 0; background: linear-gradient(90deg, transparent, rgba(255,255,255,0.3), transparent); animation: sweep 3s infinite; }

/* --- BUTTONS --- */
.btn-back-elite {
  background: white; border: 1px solid #f1f5f9; padding: 12px 25px; border-radius: 20px;
  font-weight: 800; font-size: 12px; color: #475569; display: flex; align-items: center; gap: 15px; transition: 0.3s;
}
.btn-back-elite:hover { background: #0f172a; color: white; transform: translateX(-5px); box-shadow: 0 15px 30px rgba(0,0,0,0.1); }

.btn-sunburst {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; border: none; padding: 15px 35px; border-radius: 20px;
  font-weight: 800; font-size: 13px; transition: 0.4s; box-shadow: 0 10px 25px rgba(251, 191, 36, 0.3);
}

.elite-badge-status {
  background: white; border: 1px solid #d1fae5; color: #059669; padding: 10px 25px;
  border-radius: 50px; font-weight: 800; font-size: 11px; display: flex; align-items: center; gap: 12px;
}
.status-indicator { width: 8px; height: 8px; background: #10b981; border-radius: 50%; animation: pulse 2s infinite; }

/* --- ANIMATIONS & UTILS --- */
@keyframes sweep { 0% { transform: translateX(-100%); } 100% { transform: translateX(100%); } }
@keyframes pulse { 0% { transform: scale(1); opacity: 1; } 50% { transform: scale(1.5); opacity: 0.4; } 100% { transform: scale(1); opacity: 1; } }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }

.loading-text { font-weight: 800; color: #94a3b8; font-size: 12px; letter-spacing: 2px; margin-top: 25px; }
.state-portal { min-height: 60vh; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.ai-icon-spin { font-size: 40px; color: #fbbf24; animation: spin 2s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }

.text-mono { font-family: 'JetBrains Mono', monospace; font-size: 13px !important; color: #94a3b8 !important; }
</style>