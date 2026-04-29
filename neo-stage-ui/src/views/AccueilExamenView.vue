<template>
  <div 
    class="certification-engine-root" 
    @contextmenu.prevent 
    @copy.prevent 
    @cut.prevent 
    @paste.prevent 
    @selectstart.prevent
  >
    <!-- 1. LUXURY BACKGROUND (Identité Visuelle Cohérente) -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
      <div class="tech-grid-overlay"></div>
    </div>

    <!-- 2. SÉCURITÉ : OVERLAY D'ALERTE TRICHE -->
    <Transition name="scale">
      <div v-if="showSecurityOverlay" class="security-lock-overlay">
        <div class="lock-card">
          <div class="lock-icon">
            <i class="fa-solid fa-triangle-exclamation"></i>
          </div>
          <h2>VIOLATION DE PROTOCOLE</h2>
          <p>
            Toute tentative de sortie du mode plein écran ou de changement d'onglet est enregistrée. 
            Votre indice d'intégrité a été impacté.
          </p>
          <button @click="resumeExam" class="btn-resume">
            REVENIR À L'EXAMEN ({{ securityTimer }}s)
          </button>
        </div>
      </div>
    </Transition>

    <!-- 3. LOADER SYSTÈME (Initialisation) -->
    <div v-if="loading" class="full-viewport-center">
      <div class="elite-loader-container">
        <div class="loader-rings">
          <div class="ring"></div>
          <div class="ring"></div>
          <div class="ring"></div>
        </div>
        <div class="loader-content">
          <h3 class="loading-title">CHARGEMENT DU PORTAIL</h3>
          <p class="loading-subtitle">SÉCURISATION DE LA LIAISON AES-256...</p>
          <div class="progress-nano">
            <div class="progress-nano-fill"></div>
          </div>
        </div>
      </div>
    </div>

    <!-- 4. ÉCRAN DES RÉSULTATS (BENTO STYLE LUXE) -->
    <div v-else-if="status === 'results'" class="full-viewport-center p-4">
      <div class="bento-results-container animate__animated animate__zoomIn">
        <div class="bento-grid">
          
          <!-- Bloc Score Principal -->
          <div class="bento-card score-card">
            <div class="card-glass-shine"></div>
            <span class="label">RÉSULTAT FINAL</span>
            <div class="score-circle-wrap">
              <svg viewBox="0 0 100 100" class="score-svg">
                <circle cx="50" cy="50" r="45" class="bg-ring"></circle>
                <circle cx="50" cy="50" r="45" class="fill-ring" :style="scoreProgressStyle"></circle>
              </svg>
              <div class="score-data">
                <span class="val">{{ resultsData.pourcentage }}<small>%</small></span>
              </div>
            </div>
            <div class="status-pill" :class="resultsData.pourcentage >= 70 ? 'pass' : 'fail'">
              <i class="fa-solid" :class="resultsData.pourcentage >= 70 ? 'fa-check-double' : 'fa-xmark'"></i>
              {{ resultsData.pourcentage >= 70 ? 'CERTIFICATION VALIDÉE' : 'ÉCHEC' }}
            </div>
          </div>

          <!-- Bloc Stats Détails -->
          <div class="bento-card stats-card">
            <h4 class="card-title">MÉTRIQUES CLÉS</h4>
            <div class="stat-items-stack">
              <div class="s-item">
                <div class="s-info">
                  <span class="s-label">Score Total</span>
                  <span class="s-val">{{ resultsData.scoreTotal }} Points</span>
                </div>
                <div class="s-bar"><div class="s-fill amber" :style="{width: resultsData.pourcentage + '%'}"></div></div>
              </div>
              <div class="s-item">
                <div class="s-info">
                  <span class="s-label">Intégrité</span>
                  <span class="s-val" :class="{danger: cheatingAttempts > 0}">{{ integrityScore }}% Secure</span>
                </div>
                <div class="s-bar"><div class="s-fill blue" :style="{width: integrityScore + '%'}"></div></div>
              </div>
              <div class="s-item">
                <div class="s-info">
                  <span class="s-label">Temps Final</span>
                  <span class="s-val">{{ usedTime }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Bloc Actions IA & Correction -->
          <div class="bento-card actions-card">
            <div class="ai-banner">
              <i class="fa-solid fa-sparkles"></i>
              <span>Conseils IA Disponibles</span>
            </div>
            <p class="action-desc">Analysez vos erreurs et recevez des recommandations personnalisées basées sur votre performance.</p>
            <div class="buttons-group">
              <button @click="startReview" class="btn-action outline">
                <i class="fa-solid fa-file-signature"></i> VOIR LA CORRECTION
              </button>
              <button @click="goToAIRoutes" class="btn-action primary">
                <i class="fa-solid fa-robot"></i> CONSEILS IA & ANALYSE
                <div class="shine"></div>
              </button>
            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- 5. MODE CORRECTION (REVIEW) -->
    <div v-else-if="status === 'review'" class="review-portal animate__animated animate__fadeIn">
      <nav class="review-navbar">
        <div class="brand">Evalua<span>Tech</span> <small>REVUE_TECHNIQUE</small></div>
        <button @click="status = 'results'" class="btn-close-review">
          <i class="fa-solid fa-xmark me-2"></i> FERMER LA RÉVISION
        </button>
      </nav>

      <div class="review-scroll-area">
        <div class="review-container">
          <div v-for="(item, idx) in resultsData.detailedCorrection" :key="idx" 
               class="correction-node" :class="item.isCorrect ? 'correct' : 'incorrect'">
            <div class="node-header">
              <span class="node-q">QUESTION {{ idx + 1 }}</span>
              <span class="node-status-label">{{ item.isCorrect ? 'VALIDE' : 'ERREUR' }}</span>
            </div>
            <div class="node-body">
              <h3 class="node-enonce">{{ item.enonce }}</h3>
              <div class="node-comparison">
                <div class="ans-block user">
                  <label>VOTRE RÉPONSE</label>
                  <div class="ans-content">{{ item.userAnswer || 'NON RÉPONDU' }}</div>
                </div>
                <div v-if="!item.isCorrect" class="ans-block valid">
                  <label>RÉPONSE ATTENDUE</label>
                  <div class="ans-content">{{ item.correctAnswer }}</div>
                </div>
              </div>
              <div v-if="item.feedbackIA" class="ai-feedback-box">
                <i class="fa-solid fa-lightbulb"></i>
                <p>{{ item.feedbackIA }}</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 6. INTERFACE D'EXAMEN LIVE (STYLE INTERNATIONAL) -->
    <template v-else>
      <header class="exam-header">
        <div class="header-left">
          <div class="brand-box">
            <span class="brand-main">Evalua<span>Tech</span></span>
            <span class="badge-live">SECURE_LIVE</span>
          </div>
        </div>

        <div class="header-center">
          <div class="timer-widget" :class="{ 'timer-critical': timeLeft < 300 }">
            <i class="fa-solid fa-hourglass-half"></i>
            <span class="timer-val">{{ formatTime(timeLeft) }}</span>
          </div>
        </div>

        <div class="header-right">
          <div class="integrity-monitor">
            <i class="fa-solid fa-shield-check"></i>
            <span>{{ integrityScore }}% INTEGRITY</span>
          </div>
          <button @click="triggerFinish" class="btn-submit-exam">
            SOUMETTRE <i class="fa-solid fa-paper-plane ms-1"></i>
          </button>
        </div>
      </header>

      <div class="exam-main-container">
        <!-- Sidebar Navigation -->
        <aside class="exam-sidebar-nav">
          <div class="nav-scroll">
            <div class="nav-section-title">CARTE DES QUESTIONS</div>
            <div class="questions-grid">
              <div v-for="(q, idx) in questions" :key="idx" 
                   @click="currentIndex = idx"
                   class="q-item-node" 
                   :class="{ active: currentIndex === idx, filled: userAnswers[idx].valeur }">
                {{ idx + 1 }}
              </div>
            </div>
          </div>
          <div class="sidebar-info">
            <div class="info-row">
              <span class="dot gold"></span> <span>Question actuelle</span>
            </div>
            <div class="info-row">
              <span class="dot blue"></span> <span>Réponse enregistrée</span>
            </div>
          </div>
        </aside>

        <!-- Question Display Area -->
        <section class="question-display-portal">
          <div class="question-card-wrapper animate__animated animate__fadeInRight">
            <div class="q-meta">
              <span class="q-index">SEGMENT_{{ currentIndex + 1 }}</span>
              <div class="q-type-pill">{{ currentQ?.type || 'STANDARD' }}</div>
            </div>

            <div class="q-content-body" v-if="currentQ">
              <h2 class="q-text-heading">{{ currentQ.enonce }}</h2>
              
              <!-- ANSWER INTERFACES -->
              <div class="answer-interaction-zone">
                <!-- QCU / QCM -->
                <div v-if="['QCU', 'QCM', 0, 1].includes(currentQ.type)" class="options-layout">
                  <div v-for="(opt, i) in currentQ.options" :key="i" 
                       @click="handleSelect(i)"
                       class="elite-option-card" :class="{ selected: isSelected(i) }">
                    <div class="opt-indicator">{{ String.fromCharCode(65 + i) }}</div>
                    <div class="opt-label-text">{{ opt }}</div>
                    <div class="opt-status-icon"><i class="fa-solid fa-check"></i></div>
                  </div>
                </div>

                <!-- TEXTAREA / LIBRE -->
                <div v-else class="elite-textarea-container">
                  <div class="editor-header">
                    <i class="fa-solid fa-code me-2"></i> ÉDITEUR DE RÉPONSE TECHNIQUE
                  </div>
                  <textarea 
                    v-model="userAnswers[currentIndex].valeur" 
                    @blur="saveResponse"
                    placeholder="Saisissez votre analyse détaillée..."
                  ></textarea>
                </div>
              </div>
            </div>

            <footer class="q-footer-nav">
              <button @click="currentIndex--" :disabled="currentIndex === 0" class="btn-step">
                <i class="fa-solid fa-arrow-left me-2"></i> PRÉCÉDENT
              </button>
              <div class="step-indicator">PAGE {{ currentIndex + 1 }} / {{ questions.length }}</div>
              <button v-if="currentIndex < questions.length - 1" @click="currentIndex++" class="btn-step next">
                SUIVANT <i class="fa-solid fa-arrow-right ms-2"></i>
              </button>
              <button v-else @click="triggerFinish" class="btn-step finish">
                FINALISER <i class="fa-solid fa-check-double ms-2"></i>
              </button>
            </footer>
          </div>
        </section>
      </div>
    </template>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();

/* -------------------------------------------------------------------------- */
/*                                   STATES                                   */
/* -------------------------------------------------------------------------- */
const loading = ref(true);
const status = ref('testing'); // testing, results, review
const questions = ref([]);
const currentIndex = ref(0);
const userAnswers = ref([]);
const timeLeft = ref(3600);
const evalId = ref(null);
const resultsData = ref({ scoreTotal: 0, pourcentage: 0, detailedCorrection: [] });

// Sécurité
const cheatingAttempts = ref(0);
const showSecurityOverlay = ref(false);
const securityTimer = ref(5);
let secInterval = null;

/* -------------------------------------------------------------------------- */
/*                                 COMPUTED                                   */
/* -------------------------------------------------------------------------- */
const currentQ = computed(() => questions.value[currentIndex.value]);
const integrityScore = computed(() => Math.max(0, 100 - (cheatingAttempts.value * 10)));
const usedTime = computed(() => formatTime(3600 - timeLeft.value));

const scoreProgressStyle = computed(() => {
  const dash = (resultsData.value.pourcentage / 100) * 283;
  return { strokeDasharray: `${dash} 283` };
});

/* -------------------------------------------------------------------------- */
/*                                LOGIQUE SÉCURITÉ                            */
/* -------------------------------------------------------------------------- */
const enableSecurity = () => {
  // 1. Bloquer clic droit et raccourcis
  window.addEventListener('keydown', handleKeyBlock);
  // 2. Détecter changement d'onglet / sortie fenêtre
  document.addEventListener('visibilitychange', handleVisibility);
  window.addEventListener('blur', handleWindowBlur);
  // 3. Forcer plein écran (optionnel mais recommandé)
  document.documentElement.requestFullscreen().catch(() => {});
};

const handleKeyBlock = (e) => {
  // F12, Ctrl+Shift+I, Ctrl+U, Ctrl+C, Ctrl+V
  if (e.keyCode === 123 || (e.ctrlKey && e.shiftKey && e.keyCode === 73) || (e.ctrlKey && e.keyCode === 85) || (e.ctrlKey && (e.keyCode === 67 || e.keyCode === 86))) {
    e.preventDefault();
    cheatingAttempts.value++;
    return false;
  }
};

const handleVisibility = () => {
  if (document.hidden && status.value === 'testing') {
    cheatingAttempts.value++;
    triggerSecurityWarning();
  }
};

const handleWindowBlur = () => {
  if (status.value === 'testing') {
    cheatingAttempts.value++;
    triggerSecurityWarning();
  }
};

const triggerSecurityWarning = () => {
  showSecurityOverlay.value = true;
  securityTimer.value = 5;
  clearInterval(secInterval);
  secInterval = setInterval(() => {
    securityTimer.value--;
    if (securityTimer.value <= 0) clearInterval(secInterval);
  }, 1000);
};

const resumeExam = () => {
  if (securityTimer.value <= 0) {
    showSecurityOverlay.value = false;
    document.documentElement.requestFullscreen().catch(() => {});
  }
};

/* -------------------------------------------------------------------------- */
/*                                LOGIQUE MÉTIER                             */
/* -------------------------------------------------------------------------- */
const loadExamData = async () => {
  try {
    const res = await api.get(`/Examen/setup/${route.params.id}`);
    questions.value = res.data.questions;
    evalId.value = res.data.evaluationId;
    userAnswers.value = questions.value.map(q => ({ questionId: q.id, valeur: '' }));
    
    // Simuler un chargement pro
    setTimeout(() => {
      loading.value = false;
      enableSecurity();
      startTimer();
    }, 2000);
  } catch (err) {
    console.error("Erreur chargement examen", err);
  }
};

const startTimer = () => {
  const clock = setInterval(() => {
    if (status.value === 'testing' && !loading.value) {
      if (timeLeft.value > 0) timeLeft.value--;
      else {
        clearInterval(clock);
        autoFinish();
      }
    }
  }, 1000);
};

const handleSelect = (idx) => {
  let ans = userAnswers.value[currentIndex.value];
  if (currentQ.value.type === 'QCM' || currentQ.value.type === 1) {
    let list = ans.valeur ? ans.valeur.split(';') : [];
    list.includes(idx.toString()) ? list = list.filter(x => x !== idx.toString()) : list.push(idx.toString());
    ans.valeur = list.sort().join(';');
  } else {
    ans.valeur = idx.toString();
  }
  saveResponse();
};

const isSelected = (idx) => userAnswers.value[currentIndex.value]?.valeur.split(';').includes(idx.toString());

const saveResponse = async () => {
  try {
    await api.post('/Examen/save-response', {
      evaluationId: evalId.value,
      questionId: currentQ.value.id,
      valeur: userAnswers.value[currentIndex.value].valeur
    });
  } catch (err) { console.error("Erreur auto-save"); }
};

const triggerFinish = () => {
  if (confirm("Souhaitez-vous finaliser votre examen ? Cette action est irréversible.")) {
    finishExam();
  }
};

const finishExam = async () => {
  loading.value = true;
  try {
    const termRes = await api.post(`/Examen/terminer/${evalId.value}`);
    const detailRes = await api.get(`/Examen/results/${evalId.value}`);
    
    resultsData.value = {
      scoreTotal: termRes.data.score,
      pourcentage: termRes.data.pourcentage,
      detailedCorrection: detailRes.data.detailedCorrection
    };
    
    status.value = 'results';
    if (document.fullscreenElement) document.exitFullscreen();
  } catch (err) {
    alert("Erreur lors de la soumission. Vérifiez votre connexion.");
  } finally {
    loading.value = false;
  }
};

const autoFinish = () => {
  finishExam();
};

const startReview = () => {
  status.value = 'review';
};

const goToAIRoutes = () => {
  // Redirection vers votre lien externe
  window.location.href = "http://localhost:5173/results";
};

const formatTime = (s) => {
  const m = Math.floor(s / 60);
  const sec = s % 60;
  return `${m.toString().padStart(2, '0')}:${sec.toString().padStart(2, '0')}`;
};

/* -------------------------------------------------------------------------- */
/*                               LIFECYCLE                                    */
/* -------------------------------------------------------------------------- */
onMounted(() => {
  loadExamData();
});

onUnmounted(() => {
  window.removeEventListener('keydown', handleKeyBlock);
  document.removeEventListener('visibilitychange', handleVisibility);
  window.removeEventListener('blur', handleWindowBlur);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@500;700&display=swap');

/* --- ROOT & CORE --- */
.certification-engine-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background-color: #f8fafc;
  color: #1e293b;
  height: 100vh;
  width: 100vw;
  overflow: hidden;
  position: relative;
  user-select: none;
}

/* --- LUXURY BACKGROUND --- */
.luxury-bg { position: fixed; inset: 0; z-index: 0; overflow: hidden; background: #fdfdfd; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.25; }
.sphere-amber { width: 700px; height: 700px; background: #fbbf24; top: -200px; right: -100px; }
.sphere-blue { width: 600px; height: 600px; background: #60a5fa; bottom: -150px; left: -100px; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 30%; left: 20%; opacity: 0.1; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.02; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }
.tech-grid-overlay { position: absolute; inset: 0; background-image: radial-gradient(#e2e8f0 1px, transparent 1px); background-size: 40px 40px; opacity: 0.3; }

/* --- SECURITY OVERLAY --- */
.security-lock-overlay {
  position: fixed; inset: 0; z-index: 9999; background: rgba(15, 23, 42, 0.9);
  backdrop-filter: blur(15px); display: flex; align-items: center; justify-content: center;
}
.lock-card {
  background: white; padding: 60px; border-radius: 40px; text-align: center; max-width: 500px;
  box-shadow: 0 40px 100px rgba(0,0,0,0.2); transform: scale(1);
}
.lock-icon { width: 80px; height: 80px; background: #fee2e2; color: #ef4444; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 2rem; margin: 0 auto 30px; }
.lock-card h2 { font-weight: 800; color: #0f172a; margin-bottom: 15px; }
.btn-resume { background: #fbbf24; border: none; padding: 18px 40px; border-radius: 20px; font-weight: 800; cursor: pointer; transition: 0.3s; }
.btn-resume:disabled { opacity: 0.5; cursor: not-allowed; }

/* --- LOADER --- */
.full-viewport-center { height: 100vh; display: flex; align-items: center; justify-content: center; position: relative; z-index: 10; }
.elite-loader-container { text-align: center; }
.loader-rings { position: relative; width: 100px; height: 100px; margin: 0 auto 30px; }
.ring { position: absolute; inset: 0; border: 4px solid transparent; border-top-color: #fbbf24; border-radius: 50%; animation: spin 1.5s linear infinite; }
.ring:nth-child(2) { inset: 10px; border-top-color: #60a5fa; animation-duration: 2s; }
.ring:nth-child(3) { inset: 20px; border-top-color: #f59e0b; animation-duration: 1s; }
@keyframes spin { to { transform: rotate(360deg); } }
.loading-title { font-weight: 800; letter-spacing: 2px; color: #0f172a; }
.loading-subtitle { font-size: 0.7rem; color: #94a3b8; font-weight: 700; margin-top: 5px; }

/* --- EXAM HEADER --- */
.exam-header {
  height: 80px; background: white; border-bottom: 1px solid #e2e8f0; position: relative; z-index: 100;
  display: flex; align-items: center; justify-content: space-between; padding: 0 40px;
}
.brand-box { display: flex; align-items: center; gap: 15px; }
.brand-main { font-weight: 900; font-size: 1.5rem; color: #0f172a; }
.brand-main span { color: #fbbf24; }
.badge-live { background: #ecfdf5; color: #10b981; font-size: 9px; font-weight: 800; padding: 4px 10px; border-radius: 6px; }

.timer-widget {
  background: #0f172a; color: white; padding: 10px 25px; border-radius: 15px;
  display: flex; align-items: center; gap: 12px; font-family: 'JetBrains Mono', monospace;
  font-size: 1.4rem; font-weight: 700; transition: 0.3s;
}
.timer-critical { background: #ef4444; animation: pulse-red 1s infinite; }
@keyframes pulse-red { 0%, 100% { transform: scale(1); } 50% { transform: scale(1.05); } }

.btn-submit-exam {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; border: none; padding: 12px 24px; border-radius: 12px; font-weight: 800; cursor: pointer;
  box-shadow: 0 10px 20px rgba(251, 191, 36, 0.2);
}

/* --- MAIN LAYOUT --- */
.exam-main-container { display: flex; height: calc(100vh - 80px); position: relative; z-index: 10; }

.exam-sidebar-nav {
  width: 320px; background: rgba(255,255,255,0.7); backdrop-filter: blur(20px);
  border-right: 1px solid #e2e8f0; padding: 30px; display: flex; flex-direction: column;
}
.nav-section-title { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; margin-bottom: 25px; }
.questions-grid { display: grid; grid-template-columns: repeat(5, 1fr); gap: 12px; }
.q-item-node {
  height: 42px; border-radius: 12px; background: white; border: 1.5px solid #f1f5f9;
  display: flex; align-items: center; justify-content: center; font-weight: 700; cursor: pointer; transition: 0.3s;
}
.q-item-node.active { background: #0f172a; color: white; border-color: #0f172a; transform: translateY(-3px); }
.q-item-node.filled { border-color: #fbbf24; color: #fbbf24; }

.sidebar-info { margin-top: auto; padding-top: 30px; }
.info-row { display: flex; align-items: center; gap: 10px; font-size: 11px; font-weight: 700; color: #64748b; margin-bottom: 10px; }
.dot { width: 10px; height: 10px; border-radius: 50%; }
.dot.gold { background: #fbbf24; }
.dot.blue { border: 2px solid #fbbf24; }

/* --- QUESTION AREA --- */
.question-display-portal { flex: 1; padding: 60px; overflow-y: auto; display: flex; justify-content: center; }
.question-card-wrapper { width: 100%; max-width: 900px; }
.q-meta { display: flex; justify-content: space-between; align-items: center; margin-bottom: 30px; }
.q-index { font-family: 'JetBrains Mono'; font-weight: 800; color: #fbbf24; }
.q-type-pill { background: #f1f5f9; padding: 6px 15px; border-radius: 50px; font-size: 10px; font-weight: 800; color: #64748b; }

.q-text-heading { font-size: 2.2rem; font-weight: 800; color: #0f172a; line-height: 1.3; margin-bottom: 45px; }

/* OPTIONS */
.options-layout { display: flex; flex-direction: column; gap: 18px; }
.elite-option-card {
  background: white; border: 2px solid #f1f5f9; border-radius: 24px; padding: 22px 30px;
  display: flex; align-items: center; gap: 25px; cursor: pointer; transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.elite-option-card:hover { border-color: #fbbf24; transform: scale(1.01); }
.elite-option-card.selected { border-color: #fbbf24; background: #fffbeb; box-shadow: 0 15px 30px rgba(251, 191, 36, 0.08); }

.opt-indicator {
  width: 40px; height: 40px; background: #f8fafc; border-radius: 12px;
  display: flex; align-items: center; justify-content: center; font-weight: 800; color: #64748b;
}
.selected .opt-indicator { background: #fbbf24; color: #0f172a; }
.opt-label-text { font-size: 1.1rem; font-weight: 600; color: #334155; flex: 1; }
.opt-status-icon { color: #fbbf24; opacity: 0; transition: 0.3s; }
.selected .opt-status-icon { opacity: 1; }

/* TEXTAREA */
.elite-textarea-container { border: 2px solid #f1f5f9; border-radius: 25px; background: white; overflow: hidden; }
.editor-header { padding: 15px 25px; background: #f8fafc; font-size: 11px; font-weight: 800; color: #94a3b8; border-bottom: 1px solid #f1f5f9; }
.elite-textarea-container textarea {
  width: 100%; height: 350px; padding: 30px; border: none; outline: none; font-size: 1.1rem; line-height: 1.6;
}

/* FOOTER NAV */
.q-footer-nav { margin-top: 60px; display: flex; justify-content: space-between; align-items: center; border-top: 1px solid #e2e8f0; padding-top: 40px; }
.btn-step {
  padding: 15px 35px; border-radius: 18px; border: 2px solid #e2e8f0; background: white;
  font-weight: 700; color: #64748b; cursor: pointer; transition: 0.3s;
}
.btn-step.next { background: #0f172a; color: white; border-color: #0f172a; }
.btn-step.finish { background: #10b981; color: white; border-color: #10b981; box-shadow: 0 10px 20px rgba(16, 185, 129, 0.2); }

/* --- BENTO RESULTS --- */
.bento-results-container { width: 100%; max-width: 1000px; }
.bento-grid { display: grid; grid-template-columns: 1.3fr 1fr; gap: 30px; }
.bento-card {
  background: rgba(255, 255, 255, 0.85); backdrop-filter: blur(30px);
  border: 1px solid white; border-radius: 45px; padding: 50px; position: relative; overflow: hidden;
  box-shadow: 0 30px 60px rgba(0,0,0,0.05);
}

.score-card { grid-row: span 2; display: flex; flex-direction: column; align-items: center; justify-content: center; text-align: center; }
.score-circle-wrap { position: relative; width: 220px; height: 220px; margin: 30px 0; }
.score-svg { transform: rotate(-90deg); width: 100%; height: 100%; }
.bg-ring { fill: none; stroke: #f1f5f9; stroke-width: 8; }
.fill-ring { fill: none; stroke: #fbbf24; stroke-width: 8; stroke-linecap: round; transition: stroke-dasharray 1s ease-out; }
.score-data { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; }
.score-data .val { font-size: 5rem; font-weight: 900; color: #0f172a; }
.score-data .val small { font-size: 1.5rem; color: #fbbf24; }

.status-pill { padding: 12px 30px; border-radius: 100px; font-weight: 800; font-size: 0.9rem; }
.status-pill.pass { background: #ecfdf5; color: #10b981; }
.status-pill.fail { background: #fff1f2; color: #ef4444; }

.stat-items-stack { display: flex; flex-direction: column; gap: 25px; margin-top: 30px; }
.s-info { display: flex; justify-content: space-between; margin-bottom: 8px; font-weight: 700; }
.s-label { font-size: 0.8rem; color: #94a3b8; }
.s-bar { height: 8px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.s-fill { height: 100%; border-radius: 10px; }
.s-fill.amber { background: #fbbf24; }
.s-fill.blue { background: #60a5fa; }

.actions-card { display: flex; flex-direction: column; gap: 20px; }
.ai-banner {
  background: #fdf2f8; color: #db2777; padding: 10px 20px; border-radius: 12px;
  display: inline-flex; align-items: center; gap: 10px; font-weight: 800; font-size: 0.75rem;
}
.buttons-group { display: flex; flex-direction: column; gap: 15px; margin-top: 10px; }
.btn-action { padding: 20px; border-radius: 20px; font-weight: 800; cursor: pointer; transition: 0.3s; border: none; }
.btn-action.outline { background: white; border: 2px solid #f1f5f9; color: #475569; }
.btn-action.primary {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  color: white; box-shadow: 0 15px 30px rgba(15, 23, 42, 0.2); position: relative; overflow: hidden;
}

/* --- REVIEW PORTAL --- */
.review-portal { height: 100vh; display: flex; flex-direction: column; background: #fdfdfd; position: relative; z-index: 1000; }
.review-navbar { height: 80px; padding: 0 50px; background: white; border-bottom: 1px solid #e2e8f0; display: flex; align-items: center; justify-content: space-between; }
.review-scroll-area { flex: 1; overflow-y: auto; padding: 50px; display: flex; justify-content: center; }
.review-container { width: 100%; max-width: 900px; }

.correction-node {
  background: white; border-radius: 30px; padding: 40px; margin-bottom: 30px;
  border-left: 10px solid; box-shadow: 0 10px 30px rgba(0,0,0,0.02);
}
.correction-node.correct { border-color: #10b981; }
.correction-node.incorrect { border-color: #ef4444; }

.node-header { display: flex; justify-content: space-between; margin-bottom: 20px; }
.node-q { font-size: 11px; font-weight: 800; color: #94a3b8; }
.node-status-label { font-weight: 800; font-size: 0.75rem; }
.correct .node-status-label { color: #10b981; }
.incorrect .node-status-label { color: #ef4444; }

.node-enonce { font-size: 1.4rem; font-weight: 800; color: #0f172a; margin-bottom: 30px; }
.node-comparison { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.ans-block label { font-size: 9px; font-weight: 800; color: #cbd5e1; display: block; margin-bottom: 10px; }
.ans-content { padding: 20px; border-radius: 15px; background: #f8fafc; font-weight: 600; font-size: 0.95rem; }
.valid .ans-content { background: #ecfdf5; color: #059669; }

.ai-feedback-box {
  margin-top: 30px; padding: 20px; background: #fffbeb; border-radius: 15px;
  display: flex; gap: 15px; align-items: flex-start; border: 1px solid #fef3c7;
}
.ai-feedback-box i { color: #fbbf24; margin-top: 4px; }
.ai-feedback-box p { font-size: 0.9rem; color: #92400e; font-weight: 600; margin: 0; }

/* --- TRANSITIONS & ANIMATIONS --- */
.scale-enter-active, .scale-leave-active { transition: all 0.4s ease; }
.scale-enter-from, .scale-leave-to { opacity: 0; transform: scale(0.9); }

@keyframes shine { 0% { left: -100%; } 100% { left: 200%; } }
.primary .shine {
  position: absolute; top: 0; left: -100%; width: 50%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
  animation: shine 3s infinite;
}

/* Scrollbar Custom */
::-webkit-scrollbar { width: 8px; }
::-webkit-scrollbar-track { background: #f1f5f9; }
::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }
::-webkit-scrollbar-thumb:hover { background: #fbbf24; }
</style>