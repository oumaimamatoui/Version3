<template>
  <div class="elite-details-root">
    <!-- Fond Immersif Elite -->
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

        <!-- ÉTAT : SUCCÈS -->
        <div v-else-if="candidate" class="animate__animated animate__fadeIn">
          
          <!-- Header Navigation Premium -->
          <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-5 gap-3">
             <button @click="$router.back()" class="btn-back-elite">
               <i class="fa-solid fa-arrow-left-long"></i> <span>RETOUR AU RÉPERTOIRE</span>
             </button>
             <div class="elite-breadcrumb">
               <span class="text-muted">CANDIDATS</span>
               <i class="fa-solid fa-chevron-right mx-2 separator"></i>
               <span class="fw-bold">{{ candidate.fullName.toUpperCase() }}</span>
             </div>
          </div>

          <div class="row g-4">
            <!-- COLONNE GAUCHE : IDENTITY BOX -->
            <div class="col-lg-4">
              <div class="bento-card identity-bento h-100">
                <div class="profile-header text-center">
                  <div class="avatar-master-wrapper">
                    <div class="avatar-squircle">
                      {{ candidate.fullName.split(' ').map(n => n[0]).join('').toUpperCase() }}
                    </div>
                    <div class="status-online-dot"></div>
                  </div>
                  <h2 class="candidate-name">{{ candidate.fullName }}</h2>
                  <p class="candidate-subtitle">ARCHITECTE SOLUTIONS / {{ candidate.campaignName }}</p>
                </div>

                <div class="exam-performance-hub my-4">
                  <div class="score-circle-container">
                    <svg viewBox="0 0 100 100" class="performance-svg">
                      <circle cx="50" cy="50" r="45" class="bg"></circle>
                      <circle cx="50" cy="50" r="45" class="fg" :style="scoreStyle"></circle>
                    </svg>
                    <div class="score-text">
                      <span class="number">{{ candidate.scoreGlobal }}</span>
                      <span class="percent">%</span>
                    </div>
                  </div>
                  <p class="perf-label">RÉSULTAT DE CERTIFICATION</p>
                </div>

                <div class="info-list">
                  <div class="info-item">
                    <div class="i-icon"><i class="fa-solid fa-envelope"></i></div>
                    <div class="i-data">
                      <label>Email Professionnel</label>
                      <span>{{ candidate.email }}</span>
                    </div>
                  </div>
                  <div class="info-item">
                    <div class="i-icon" :class="candidate.integrityScore < 80 ? 'warn' : 'safe'">
                      <i class="fa-solid fa-shield-halved"></i>
                    </div>
                    <div class="i-data">
                      <label>Indice d'Intégrité</label>
                      <span class="fw-bold">{{ candidate.integrityScore }}% - Fiabilité Haute</span>
                    </div>
                  </div>
                  <div class="info-item">
                    <div class="i-icon"><i class="fa-solid fa-stopwatch"></i></div>
                    <div class="i-data">
                      <label>Temps de Réalisation</label>
                      <span>{{ candidate.timeTaken || '24m 15s' }}</span>
                    </div>
                  </div>
                </div>

                <div class="global-tag">
                   GLOBAL SCORE: {{ candidate.scoreGlobal }}%
                </div>
              </div>
            </div>

            <!-- COLONNE DROITE : INSIGHTS & AUDIT -->
            <div class="col-lg-8">
              
              <!-- SECTION IA VERDICT (Dark Premium) -->
              <div class="bento-card ai-insight-card mb-4">
                <div class="ai-header-elite">
                   <div class="ai-bot-visual">
                      <div class="scanner"></div>
                      <i class="fa-solid fa-robot"></i>
                   </div>
                   <div class="ai-title-wrap">
                      <h5>RAPPORT DÉCISIONNEL IA</h5>
                      <p>Analyse prédictive basée sur les patterns de réponse.</p>
                   </div>
                </div>
                <div class="ai-content-box">
                  <p class="verdict-text">{{ candidate.iaVerdict }}</p>
                  <button @click="openAIConsult" class="btn-consult-ia">
                    <i class="fa-solid fa-wand-magic-sparkles me-2"></i> CONSULTER CONSEILS IA (External)
                    <div class="btn-shine"></div>
                  </button>
                </div>
              </div>

              <!-- SECTION AUDIT (White Premium) -->
              <div class="bento-card audit-log-card">
                <div class="d-flex justify-content-between align-items-center mb-4">
                   <h6 class="section-title-elite">AUDIT DÉTAILLÉ DES RÉPONSES</h6>
                   <div class="stat-pill-small">{{ candidate.detailedCorrection?.length }} QUESTIONS ANALYSÉES</div>
                </div>

                <div class="audit-list-wrapper">
                   <div v-for="(item, idx) in candidate.detailedCorrection" :key="idx" 
                        class="audit-entry" :class="item.isCorrect ? 'valid' : 'invalid'">
                      <div class="entry-indicator">
                        <i class="fa-solid" :class="item.isCorrect ? 'fa-check-circle' : 'fa-circle-xmark'"></i>
                      </div>
                      <div class="entry-main">
                        <div class="entry-header">
                           <span class="q-number">QUESTION #{{ idx + 1 }}</span>
                           <span class="q-difficulty">NIVEAU TECHNIQUE: AVANCÉ</span>
                        </div>
                        <p class="q-enonce">{{ item.enonce }}</p>
                        <div class="q-comparison">
                           <div class="ans-box user">
                              <label>RÉPONSE DU CANDIDAT</label>
                              <p>{{ item.userAnswer || '---' }}</p>
                           </div>
                           <div class="ans-box target" v-if="!item.isCorrect">
                              <label>RÉPONSE ATTENDUE</label>
                              <p>{{ item.correctAnswer }}</p>
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
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import axios from 'axios';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const route = useRoute();
const candidate = ref(null);
const loading = ref(true);

const scoreStyle = computed(() => {
  if (!candidate.value) return {};
  const dash = (candidate.value.scoreGlobal / 100) * 283;
  return { strokeDasharray: `${dash} 283` };
});

const fetchCandidateDetails = async () => {
  try {
    const token = localStorage.getItem('token');
    const res = await axios.get(`http://localhost:5172/api/Candidates/${route.params.id}`, {
        headers: { Authorization: `Bearer ${token}` }
    });
    candidate.value = res.data;
  } catch (err) {
    console.error(err);
  } finally {
    setTimeout(() => { loading.value = false; }, 800);
  }
};

const openAIConsult = () => { window.location.href = "http://localhost:5173/results"; };

onMounted(fetchCandidateDetails);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@500;700&display=swap');

/* --- STRUCTURE --- */
.elite-details-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background: #f8fafc; min-height: 100vh; display: flex; position: relative;
}

.luxury-bg { position: fixed; inset: 0; z-index: 0; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.2; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -100px; right: -50px; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -50px; left: -50px; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.03; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

.content-overlay { position: relative; z-index: 10; }

/* --- BENTO CARDS --- */
.bento-card {
  background: rgba(255, 255, 255, 0.85);
  backdrop-filter: blur(25px);
  border-radius: 40px; padding: 40px;
  border: 1px solid white;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.04);
}

/* --- IDENTITY BOX --- */
.avatar-squircle {
  width: 130px; height: 130px; border-radius: 45px; margin: 0 auto;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  display: flex; align-items: center; justify-content: center;
  font-size: 42px; font-weight: 800; color: #fbbf24;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}
.candidate-name { font-weight: 900; font-size: 26px; color: #0f172a; margin-top: 20px; margin-bottom: 5px; }
.candidate-subtitle { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; }

/* Performance Hub */
.score-circle-container { position: relative; width: 140px; height: 140px; margin: 0 auto; }
.performance-svg { transform: rotate(-90deg); }
.performance-svg circle { fill: none; stroke-width: 8; }
.performance-svg .bg { stroke: #f1f5f9; }
.performance-svg .fg { stroke: #fbbf24; stroke-linecap: round; transition: 1.5s cubic-bezier(0.4, 0, 0.2, 1); }
.score-text { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; flex-direction: row; }
.score-text .number { font-size: 32px; font-weight: 900; color: #0f172a; }
.score-text .percent { font-size: 14px; font-weight: 800; color: #fbbf24; margin-left: 2px; }
.perf-label { font-size: 10px; font-weight: 800; color: #94a3b8; margin-top: 15px; text-align: center; }

/* Info List */
.info-list { margin-top: 35px; text-align: left; }
.info-item { display: flex; gap: 20px; margin-bottom: 25px; align-items: center; }
.i-icon { width: 45px; height: 45px; background: #f8fafc; border-radius: 15px; display: flex; align-items: center; justify-content: center; color: #94a3b8; }
.i-icon.safe { color: #10b981; background: #f0fdf4; }
.i-icon.warn { color: #ef4444; background: #fef2f2; }
.i-data label { display: block; font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-bottom: 2px; }
.i-data span { font-size: 14px; font-weight: 700; color: #1e293b; }

.global-tag {
  background: #0f172a; color: white; padding: 15px; border-radius: 20px;
  font-weight: 900; font-size: 14px; text-align: center; margin-top: 30px;
}

/* --- AI INSIGHT CARD (Dark) --- */
.ai-insight-card { background: #0f172a; border-color: #1e293b; color: white; }
.ai-header-elite { display: flex; align-items: center; gap: 20px; margin-bottom: 30px; }
.ai-bot-visual {
  width: 60px; height: 60px; background: rgba(251, 191, 36, 0.1); color: #fbbf24;
  border-radius: 20px; display: flex; align-items: center; justify-content: center; font-size: 24px;
  position: relative; overflow: hidden; border: 1px solid rgba(251,191,36,0.2);
}
.scanner { position: absolute; width: 100%; height: 2px; background: #fbbf24; top: 0; animation: scan 2s infinite linear; }
@keyframes scan { 0% { top: 0%; } 100% { top: 100%; } }

.ai-title-wrap h5 { font-weight: 900; color: #fbbf24; margin: 0; font-size: 16px; letter-spacing: 1px; }
.ai-title-wrap p { font-size: 11px; color: #94a3b8; margin: 0; font-weight: 700; }
.verdict-text { font-size: 17px; line-height: 1.8; color: #cbd5e1; margin-bottom: 30px; }

.btn-consult-ia {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; border: none; padding: 16px 32px; border-radius: 20px;
  font-weight: 800; font-size: 13px; cursor: pointer; position: relative; overflow: hidden; transition: 0.3s;
}
.btn-consult-ia:hover { transform: translateY(-3px); box-shadow: 0 15px 30px rgba(251, 191, 36, 0.3); }

/* --- AUDIT LOG --- */
.section-title-elite { font-weight: 900; font-size: 15px; color: #0f172a; letter-spacing: 1px; }
.stat-pill-small { font-size: 9px; font-weight: 800; background: #f1f5f9; color: #64748b; padding: 6px 15px; border-radius: 50px; }
.audit-list-wrapper { max-height: 550px; overflow-y: auto; padding-right: 15px; }

.audit-entry {
  background: white; border-radius: 30px; padding: 30px; margin-bottom: 20px;
  display: flex; gap: 25px; border: 1.5px solid #f1f5f9; transition: 0.3s;
}
.audit-entry:hover { transform: scale(1.01); box-shadow: 0 15px 30px rgba(0,0,0,0.02); }
.audit-entry.valid { border-left: 8px solid #10b981; }
.audit-entry.invalid { border-left: 8px solid #ef4444; }

.entry-indicator { font-size: 24px; }
.valid .entry-indicator { color: #10b981; }
.invalid .entry-indicator { color: #ef4444; }

.q-number { font-family: 'JetBrains Mono'; font-weight: 800; color: #94a3b8; font-size: 11px; }
.q-difficulty { font-size: 9px; font-weight: 800; background: #f8fafc; color: #475569; padding: 4px 10px; border-radius: 8px; margin-left: 15px; }
.q-enonce { font-weight: 800; font-size: 16px; color: #1e293b; margin: 15px 0 25px; line-height: 1.4; }

.q-comparison { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.ans-box label { display: block; font-size: 9px; font-weight: 800; color: #cbd5e1; margin-bottom: 8px; }
.ans-box p { font-size: 13px; font-weight: 700; color: #475569; margin: 0; padding: 12px; background: #f8fafc; border-radius: 12px; }
.ans-box.target p { color: #10b981; background: #f0fdf4; }

/* --- NAV & BUTTONS --- */
.btn-back-elite {
  background: white; border: 1.5px solid #f1f5f9; padding: 12px 24px; border-radius: 20px;
  font-weight: 800; font-size: 12px; color: #475569; display: flex; align-items: center; gap: 15px; transition: 0.3s;
}
.btn-back-elite:hover { background: #0f172a; color: white; transform: translateX(-5px); }

.elite-breadcrumb { font-size: 12px; font-weight: 700; }
.separator { font-size: 10px; color: #cbd5e1; }

/* Custom Scrollbar */
.audit-list-wrapper::-webkit-scrollbar { width: 6px; }
.audit-list-wrapper::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
</style>