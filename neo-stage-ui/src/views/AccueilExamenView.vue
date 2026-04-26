<template>
  <div class="premium-secure-viewport" @contextmenu.prevent>
    <div class="bg-mesh"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>

    <!-- ANTICHEAT OVERLAY -->
    <transition name="fade">
      <div v-if="anticheatWarning" class="anticheat-overlay">
        <div class="anticheat-box">
          <i class="fa-solid fa-shield-exclamation fa-3x text-danger mb-3"></i>
          <h3 class="fw-900">Violation Détectée</h3>
          <p class="text-muted mt-2">Vous avez quitté la fenêtre d'examen. Cet incident a été enregistré.</p>
          <button @click="dismissAnticheat" class="btn-img btn-dark mt-4 w-100">Reprendre le Test</button>
        </div>
      </div>
    </transition>

    <!-- 1. CHARGEMENT -->
    <div v-if="loading" class="centering-layout">
      <div class="text-center">
        <div class="spinner-pro-premium"></div>
        <p class="loading-step mt-4">Initialisation du Terminal...</p>
      </div>
    </div>

    <!-- 2. INTERFACE PRINCIPALE -->
    <template v-else>

      <!-- ═══ ÉCRAN D'ACCUEIL (LOBBY) ═══ -->
      <div v-if="!examStarted" class="centering-layout">
        <div class="card-glass-setup animate__animated animate__fadeIn">
          <div class="badge-tech mb-3">
            <span class="live-dot"></span> TERMINAL D'ÉVALUATION SÉCURISÉ
          </div>
          <h1 class="title-display mt-4">{{ campaignData.nom }}</h1>
          
          <div v-if="isDemoMode" class="demo-badge mb-3">MODE DÉMONSTRATION (Bypass SQL)</div>

          <div class="info-grid my-5">
            <div class="info-pill">
              <div class="pill-icon"><i class="fa-solid fa-layer-group"></i></div>
              <div><div class="pill-val">{{ questions.length }}</div><div class="pill-lab">Questions</div></div>
            </div>
            <div class="info-pill">
              <div class="pill-icon amber"><i class="fa-solid fa-hourglass-half"></i></div>
              <div><div class="pill-val">30 min</div><div class="pill-lab">Durée</div></div>
            </div>
          </div>

          <button @click="initiateSession" class="btn-img btn-amber w-100 btn-xl">
            <i class="fa-solid fa-bolt me-2"></i>OUVRIR LA SESSION
          </button>
        </div>
      </div>

      <!-- ═══ INTERFACE DU TEST ═══ -->
      <div v-else class="exam-layout animate__animated animate__fadeIn">
        
        <!-- TOP BAR -->
        <div class="top-bar">
          <div class="top-bar-inner">
            <div class="campaign-tag fw-800"><i class="fa-solid fa-microchip me-2 text-amber"></i>{{ campaignData.nom }}</div>
            <div class="progress-track"><div class="progress-fill" :style="{ width: progressPct + '%' }"></div></div>
            <div class="timer-global">{{ formatGlobalTime }}</div>
          </div>
        </div>

        <!-- ZONE DE QUESTION -->
        <div class="exam-main-area">
          <div class="question-card">
            <div class="q-meta-row mb-4">
              <div class="q-number-badge">Question {{ currentIndex + 1 }}</div>
              <div class="q-points-badge ms-2">{{ currentQ.points }} PTS</div>
            </div>

            <h2 class="q-enonce-text mb-5">{{ currentQ.enonce }}</h2>

            <!-- OPTIONS TYPE 0 (CHOIX UNIQUE) -->
            <div v-if="currentQ.type === 0" class="options-grid">
              <div v-for="(opt, i) in currentQ.reponses" :key="i" 
                   class="option-card" :class="{ selected: userAnswers[currentIndex].selectedIndex === i }"
                   @click="selectOption(i)">
                <div class="option-marker">{{ String.fromCharCode(65 + i) }}</div>
                <div class="option-text">{{ opt }}</div>
              </div>
            </div>

            <!-- TYPE VRAI / FAUX -->
            <div v-else-if="currentQ.type === 2" class="vf-grid">
               <div class="vf-card vf-true" :class="{ selected: userAnswers[currentIndex].boolAnswer === true }" @click="selectBool(true)">VRAI</div>
               <div class="vf-card vf-false" :class="{ selected: userAnswers[currentIndex].boolAnswer === false }" @click="selectBool(false)">FAUX</div>
            </div>

            <!-- TYPE CODE -->
            <div v-else-if="currentQ.type === 5" class="code-answer-zone">
               <textarea v-model="userAnswers[currentIndex].textAnswer" class="code-answer-field" rows="10" placeholder="// Tapez votre code ici..."></textarea>
            </div>

            <!-- NAVIGATION FOOTER -->
            <div class="q-footer mt-5 pt-4 border-top">
              <button @click="prevQuestion" :disabled="currentIndex === 0" class="btn-img btn-outline-nav">Précédent</button>
              <button @click="nextQuestion" class="btn-img btn-amber px-5">
                {{ currentIndex === questions.length - 1 ? 'TERMINER' : 'SUIVANT' }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();

/* ─── ÉTATS ─────────────────────────────────────────────── */
const loading = ref(true);
const examStarted = ref(false);
const isDemoMode = ref(false);
const anticheatWarning = ref(false);

const campaignData = ref({ nom: 'Chargement...' });
const questions = ref([]);
const currentIndex = ref(0);
const userAnswers = ref([]);
const evaluationId = ref(null);
const globalTimeLeft = ref(1800);
let timerInterval = null;

/* ─── COMPUTED ───────────────────────────────────────────── */
const currentQ = computed(() => questions.value[currentIndex.value] || {});
const progressPct = computed(() => (currentIndex.value / questions.length) * 100);
const formatGlobalTime = computed(() => {
  const m = Math.floor(globalTimeLeft.value / 60);
  const s = globalTimeLeft.value % 60;
  return `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
});

/* ─── CHARGEMENT AVEC BYPASS ────────────────────────────── */
const loadData = async () => {
  const cid = route.params.id;
  try {
    const res = await api.get(`/Examen/setup/${cid}`);
    campaignData.value = { nom: res.data.campagneNom };
    questions.value = res.data.questions;
    evaluationId.value = res.data.evaluationId;
  } catch (err) {
    // 🔥 BYPASS AUTOMATIQUE SI ERREUR BASE DE DONNÉES
    console.warn("⚠️ Base de données inaccessible ou ID inconnu. Passage en mode Démo.");
    isDemoMode.value = true;
    campaignData.value = { nom: "Architecture Senior Frontend (Demo)" };
    evaluationId.value = "demo-session";
    
    questions.value = [
      { id: 1, enonce: "Lequel de ces hooks est utilisé pour gérer les effets de bord dans Vue 3 ?", type: 0, points: 2, reponses: ["onMounted", "useEffect", "startLifecycle", "setupInit"] },
      { id: 2, enonce: "Vue.js utilise un DOM virtuel pour optimiser les performances.", type: 2, points: 1, reponses: ["Vrai", "Faux"] },
      { id: 3, enonce: "Écrivez une fonction JavaScript qui retourne la somme de deux nombres.", type: 5, points: 5 }
    ];
  } finally {
    userAnswers.value = questions.value.map(() => ({ selectedIndex: null, boolAnswer: null, textAnswer: '' }));
    loading.value = false;
  }
};

/* ─── LOGIQUE DE SESSION ────────────────────────────────── */
const initiateSession = () => {
  examStarted.value = true;
  timerInterval = setInterval(() => {
    if (globalTimeLeft.value > 0) globalTimeLeft.value--;
    else finishExam();
  }, 1000);
  
  document.addEventListener('visibilitychange', () => {
    if (document.hidden) anticheatWarning.value = true;
  });
};

const selectOption = (idx) => userAnswers.value[currentIndex.value].selectedIndex = idx;
const selectBool = (val) => userAnswers.value[currentIndex.value].boolAnswer = val;

const nextQuestion = () => {
  if (currentIndex.value < questions.value.length - 1) currentIndex.value++;
  else finishExam();
};

const prevQuestion = () => { if (currentIndex.value > 0) currentIndex.value--; };

const finishExam = async () => {
  clearInterval(timerInterval);
  alert("Examen terminé ! Calcul du score...");
  router.push('/dashboard');
};

const dismissAnticheat = () => anticheatWarning.value = false;

onMounted(loadData);
onUnmounted(() => clearInterval(timerInterval));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800;900&display=swap');

.premium-secure-viewport { min-height: 100vh; background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; position: relative; overflow: hidden; }
.bg-mesh { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 1px, transparent 1px); background-size: 35px 35px; opacity: 0.15; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.12; }
.orb-amber { width: 50vw; height: 50vw; background: #fbbf24; top: -10%; right: -10%; }
.orb-blue { width: 40vw; height: 40vw; background: #6366f1; bottom: -10%; left: -10%; }

.centering-layout { height: 100vh; display: flex; align-items: center; justify-content: center; z-index: 10; position: relative; }
.card-glass-setup { background: rgba(255, 255, 255, 0.95); backdrop-filter: blur(20px); border-radius: 40px; padding: 60px; max-width: 600px; width: 100%; border: 1px solid #eef2f6; box-shadow: 0 40px 100px rgba(0,0,0,0.1); text-align: center; }

.title-display { font-weight: 900; font-size: 2.2rem; line-height: 1.1; }
.demo-badge { display: inline-block; padding: 6px 15px; background: #fff1f2; color: #f43f5e; border-radius: 10px; font-weight: 800; font-size: 0.7rem; }

.exam-layout { height: 100vh; display: flex; flex-direction: column; z-index: 10; position: relative; }
.top-bar { height: 70px; background: white; border-bottom: 1px solid #eef2f6; display: flex; align-items: center; padding: 0 40px; }
.top-bar-inner { display: flex; align-items: center; width: 100%; gap: 20px; }
.progress-track { flex: 1; height: 8px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; background: #f59e0b; transition: 0.3s; }
.timer-global { background: #0f172a; color: white; padding: 8px 18px; border-radius: 12px; font-weight: 900; }

.exam-main-area { flex: 1; display: flex; align-items: center; justify-content: center; padding: 40px; }
.question-card { background: white; border-radius: 35px; padding: 50px; max-width: 850px; width: 100%; box-shadow: 0 20px 60px rgba(0,0,0,0.05); }

.q-enonce-text { font-size: 1.6rem; font-weight: 800; color: #0f172a; }
.options-grid { display: grid; gap: 12px; }
.option-card { padding: 18px 25px; border: 2px solid #f1f5f9; border-radius: 20px; cursor: pointer; display: flex; align-items: center; gap: 15px; font-weight: 700; transition: 0.2s; }
.option-card:hover { border-color: #fbbf24; background: #fffbeb; }
.option-card.selected { border-color: #f59e0b; background: #fffbeb; color: #f59e0b; }
.option-marker { width: 35px; height: 35px; background: #f1f5f9; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-size: 0.9rem; }
.option-card.selected .option-marker { background: #f59e0b; color: white; }

.vf-grid { display: flex; gap: 20px; }
.vf-card { flex: 1; padding: 40px; border-radius: 20px; border: 2px solid #f1f5f9; text-align: center; font-weight: 900; cursor: pointer; transition: 0.2s; }
.vf-true.selected { border-color: #10b981; color: #10b981; background: #f0fdf4; }
.vf-false.selected { border-color: #f43f5e; color: #f43f5e; background: #fef2f2; }

.code-answer-field { width: 100%; background: #0f172a; color: #10b981; border: none; border-radius: 20px; padding: 25px; font-family: monospace; outline: none; }

.btn-img { padding: 14px 28px; border-radius: 18px; border: none; font-weight: 800; cursor: pointer; transition: 0.2s; font-size: 0.9rem; }
.btn-amber { background: #f59e0b; color: white; }
.btn-amber:hover { transform: translateY(-3px); box-shadow: 0 10px 20px rgba(245, 158, 11, 0.3); }
.btn-dark { background: #0f172a; color: white; }
.btn-outline-nav { background: white; border: 1.5px solid #eef2f6; color: #64748b; }

.anticheat-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.9); backdrop-filter: blur(10px); z-index: 1000; display: flex; align-items: center; justify-content: center; }
.anticheat-box { background: white; border-radius: 30px; padding: 50px; text-align: center; max-width: 400px; }

.spinner-pro-premium { width: 50px; height: 50px; border: 5px solid #f1f5f9; border-top-color: #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>