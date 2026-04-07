<template>
  <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
  
  <div class="premium-secure-viewport" :class="{ 'lock-shield-active': securityBreach }" @contextmenu.prevent @copy.prevent @paste.prevent>
    
    <!-- 1. ÉCRAN DE CHARGEMENT -->
    <div v-if="loading" class="centering-layout">
        <div class="text-center animate__animated animate__fadeIn">
            <i class="fa-solid fa-circle-notch fa-spin fa-4x text-amber-500 mb-4"></i>
            <h2 class="text-slate-700">Initialisation de la session sécurisée...</h2>
        </div>
    </div>

    <!-- 2. ÉCRAN D'ERREUR (ID MANQUANT OU INVALIDE) -->
    <div v-else-if="errorMsg" class="centering-layout">
        <div class="card-glass-setup text-center animate__animated animate__headShake">
            <i class="fa-solid fa-triangle-exclamation fa-5x text-danger mb-4"></i>
            <h1 class="title-display">Accès <span>Interdit</span></h1>
            <p class="subtitle-display">{{ errorMsg }}</p>
            <button @click="router.push('/dashboard')" class="btn-img btn-amber mt-5 w-100">RETOUR AU DASHBOARD</button>
        </div>
    </div>

    <!-- 3. INTERFACE D'EXAMEN -->
    <template v-else>
        <!-- Sidebar Guardian IA -->
        <aside class="proctor-aside" v-if="examStarted && !quizFinished">
            <div class="biometric-hud">
                <div class="scanner-bar"></div>
                <i class="fa-solid fa-robot fa-3x mb-2 text-slate-800"></i>
                <div class="ai-label">NEO_GUARDIAN_V1</div>
            </div>
            <div class="log-stream-terminal mt-4">
                <div class="log-box">
                    <div v-for="(log, i) in securityLogs.slice(-6)" :key="i" class="log-entry">
                        <span class="ts">[{{ log.time }}]</span> {{ log.msg }}
                    </div>
                </div>
            </div>
            <div class="aside-footer mt-auto">
                <div class="integrity-report">
                    <div class="d-flex justify-content-between mb-1">
                        <label>INTÉGRITÉ</label>
                        <span :class="integrity < 70 ? 'danger' : 'safe'">{{ integrity }}%</span>
                    </div>
                    <div class="i-track"><div class="i-fill" :style="{ width: integrity + '%' }"></div></div>
                </div>
            </div>
        </aside>

        <main class="main-terminal">
            <header class="top-meta-nav" v-if="examStarted && !quizFinished">
                <div class="brand-name">NEO<span>EVAL</span></div>
                <div class="timer-box-pro" :class="{ 'critical': secondsLeft < 60 }">
                    <i class="fa-solid fa-clock-rotate-left me-2"></i> {{ formatTime(secondsLeft) }}
                </div>
            </header>

            <!-- Écran d'accueil (Setup) -->
            <div v-if="!examStarted && !quizFinished" class="centering-layout">
                <div class="card-glass-setup animate__animated animate__zoomIn">
                    <div class="icon-secure-ring mb-4"><i class="fa-solid fa-user-lock"></i></div>
                    <h1 class="title-display">Portail <span>Examen</span></h1>
                    <p class="subtitle-display">Campagne : <strong>{{ campaignName }}</strong></p>
                    <button @click="initiateSession" class="btn-img btn-amber px-5 mt-5">DÉVERROUILLER L'ESPACE &rarr;</button>
                </div>
            </div>

            <!-- Questionnaire Actif -->
            <div v-if="examStarted && !quizFinished" class="centering-layout">
                <div class="exam-unit" v-if="questions.length > 0">
                    <div class="pro-stepper mb-4">
                        QUESTION <span>{{ currentIndex + 1 }}</span> SUR {{ questions.length }}
                        <div class="s-track"><div class="s-fill" :style="{ width: ((currentIndex + 1) / questions.length * 100) + '%' }"></div></div>
                    </div>

                    <div class="surface-card-q animate__animated animate__fadeInUp">
                        <h2 class="h2-q">{{ questions[currentIndex]?.texte }}</h2>
                        <div class="options-pro-grid mt-5">
                            <div v-for="(option, idx) in questions[currentIndex]?.options" :key="idx"
                                 @click="pickOption(idx)" class="cyber-option-btn"
                                 :class="{ 'is-selected': answers[currentIndex] === idx }">
                                <div class="alpha-idx">{{ String.fromCharCode(65 + idx) }}</div>
                                <div class="alpha-label">{{ option }}</div>
                                <i v-if="answers[currentIndex] === idx" class="fa-solid fa-circle-check status-icon"></i>
                            </div>
                        </div>
                        <div class="footer-navigation mt-5 pt-4">
                            <button @click="currentIndex--" class="btn-img btn-gray" :disabled="currentIndex === 0">RETOUR</button>
                            <button @click="nextStep" class="btn-img btn-amber" :disabled="answers[currentIndex] === null">
                                {{ currentIndex === questions.length - 1 ? 'TERMINER' : 'SUIVANT &rarr;' }}
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Écran Résultat Final -->
            <div v-else-if="quizFinished" class="centering-layout">
                <div class="card-glass-setup result-portal animate__animated animate__fadeInDown">
                    <h1 class="title-display">Session <span>Terminée</span></h1>
                    <div class="visual-score my-5">
                        <div class="val-score">{{ finalScore }}<span>/20</span></div>
                        <div class="tag-credibility success">INTÉGRITÉ SESSION : {{ integrity }}%</div>
                    </div>
                    <button @click="router.push('/dashboard')" class="btn-img btn-gray mt-4">QUITTER LA SESSION</button>
                </div>
            </div>
        </main>
    </template>

    <!-- Modal Blocage Sécurité -->
    <div v-if="securityBreach && examStarted" class="lockdown-curtain">
         <div class="card-shield animate__animated animate__shakeX">
            <i class="fa-solid fa-shield-virus fa-4x text-danger mb-4"></i>
            <h2 class="text-dark">SÉCURITÉ RÉSEAU INTERROMPUE</h2>
            <p class="text-muted">Plein écran requis ou perte de focus détectée.</p>
            <button @click="restoreConnection" class="btn-img btn-amber w-100 mt-4">RESTAURER LA SESSION &rarr;</button>
         </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const API_BASE = "http://localhost:5172/api/Examen";

// States
const loading = ref(true);
const errorMsg = ref(null);
const examStarted = ref(false);
const quizFinished = ref(false);
const evaluationId = ref(null);
const questions = ref([]);
const answers = ref([]);
const currentIndex = ref(0);
const secondsLeft = ref(0);
const campaignName = ref("");
const integrity = ref(100);
const isFocused = ref(true);
const isFullscreen = ref(false);
const securityLogs = ref([]);
const finalScore = ref(0);

const securityBreach = computed(() => (!isFocused.value || !isFullscreen.value) && examStarted.value && !quizFinished.value);

const loadExamData = async () => {
    const cid = route.query.id;
    // Vérification de l'ID avant l'appel API (Empêche l'erreur undefined)
    if (!cid || cid === 'undefined') {
        errorMsg.value = "ID de candidature manquant dans l'URL. Veuillez utiliser le lien d'invitation.";
        loading.value = false;
        return;
    }

    try {
        const res = await axios.get(`${API_BASE}/setup/${cid}`);
        evaluationId.value = res.data.evaluationId;
        questions.value = res.data.questions;
        campaignName.value = res.data.campagneNom;
        secondsLeft.value = res.data.tempsLimite;
        answers.value = new Array(questions.value.length).fill(null);
        loading.value = false;
        addLog("Système synchronisé.");
    } catch (err) {
        console.error(err);
        errorMsg.value = "Impossible de charger l'examen. ID invalide ou serveur hors ligne.";
        loading.value = false;
    }
};

const initiateSession = () => {
    const el = document.documentElement;
    if (el.requestFullscreen) {
        el.requestFullscreen().then(() => {
            examStarted.value = true;
            isFullscreen.value = true;
            setupSecurityListeners();
            addLog("Environnement isolé.");
        });
    }
};

const setupSecurityListeners = () => {
    window.onblur = () => { if(examStarted.value) { isFocused.value = false; handleViolation("FOCUS_LOST"); } };
    window.onfocus = () => isFocused.value = true;
    document.onfullscreenchange = () => isFullscreen.value = !!document.fullscreenElement;
};

const handleViolation = (reason) => {
    integrity.value = Math.max(0, integrity.value - 20);
    addLog(`ALERTE: ${reason}`);
    syncProgress();
};

const pickOption = (idx) => {
    answers.value[currentIndex.value] = idx;
    syncProgress();
};

const syncProgress = async () => {
    if (!evaluationId.value) return;
    try {
        await axios.post(`${API_BASE}/sync`, {
            evaluationId: evaluationId.value,
            currentIndex: currentIndex.value,
            reponsesChoisies: answers.value.map(a => a !== null ? a.toString() : ""),
            integrite: integrity.value
        });
    } catch (e) { console.error("Sync Error"); }
};

const nextStep = () => {
    if (currentIndex.value < questions.value.length - 1) {
        currentIndex.value++;
    } else {
        finishExam();
    }
};

const finishExam = async () => {
    try {
        const res = await axios.post(`${API_BASE}/terminer/${evaluationId.value}`);
        finalScore.value = res.data.score;
        quizFinished.value = true;
        if (document.exitFullscreen) document.exitFullscreen();
    } catch (e) { alert("Erreur lors de la clôture."); }
};

const restoreConnection = () => {
    document.documentElement.requestFullscreen();
    isFocused.value = true;
};

const addLog = (m) => securityLogs.value.push({ time: new Date().toLocaleTimeString(), msg: m });
const formatTime = (s) => `${Math.floor(s/60)}:${(s%60).toString().padStart(2, '0')}`;

onMounted(loadExamData);
let timer = setInterval(() => { if(examStarted.value && secondsLeft.value > 0) secondsLeft.value--; }, 1000);
onUnmounted(() => clearInterval(timer));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&family=JetBrains+Mono&display=swap');

.premium-secure-viewport {
  font-family: 'Plus Jakarta Sans', sans-serif;
  height: 100vh; background: #f8fafc; display: flex; overflow: hidden; position: relative; user-select: none;
}

/* SIDEBAR */
.proctor-aside {
  width: 280px; background: rgba(255,255,255,0.8); backdrop-filter: blur(20px);
  border-right: 1.5px solid #e2e8f0; z-index: 100; display: flex; flex-direction: column; padding: 25px;
}
.biometric-hud { background: #fff; padding: 25px; border-radius: 20px; border: 1.5px solid #f1f5f9; position: relative; text-align: center;}
.scanner-bar { position: absolute; top: 0; left: 0; width: 100%; height: 2px; background: #eab308; animation: scanning 3s infinite linear; }
@keyframes scanning { 0%, 100% {top: 15%} 50% {top: 85%} }
.log-box { height: 180px; background: #0f172a; border-radius: 12px; padding: 12px; font-family: 'JetBrains Mono'; color: #22c55e; font-size: 10px; overflow: hidden;}

/* MAIN AREA */
.main-terminal { flex: 1; z-index: 10; display: flex; flex-direction: column; overflow-y: auto; }
.centering-layout { flex: 1; display: flex; align-items: center; justify-content: center; padding: 30px; }

/* CARDS */
.card-glass-setup { background: #fff; padding: 50px; border-radius: 35px; box-shadow: 0 20px 50px rgba(0,0,0,0.05); text-align: center; max-width: 500px;}
.surface-card-q { background: #fff; padding: 50px; border-radius: 35px; box-shadow: 0 20px 50px rgba(0,0,0,0.05); width: 100%; max-width: 800px;}

/* ELEMENTS */
.cyber-option-btn {
    display: flex; align-items: center; padding: 18px 25px; background: #f8fafc; border: 1.5px solid #f1f5f9;
    border-radius: 18px; margin-bottom: 12px; cursor: pointer; transition: 0.2s;
}
.cyber-option-btn.is-selected { background: #0f172a; color: #fff; border-color: #0f172a;}
.alpha-idx { width: 35px; height: 35px; background: #fff; color: #0f172a; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-weight: 800; margin-right: 15px; }

.btn-img { padding: 14px 28px; border-radius: 14px; font-weight: 700; border: none; cursor: pointer; transition: 0.3s; }
.btn-amber { background: #ffb800; color: #fff; }
.btn-gray { background: #e2e8f0; color: #475569; }

.timer-box-pro { background: #0f172a; color: #fff; font-family: 'JetBrains Mono'; padding: 8px 18px; border-radius: 10px; font-size: 1.3rem; font-weight: 800;}
.val-score { font-size: 80px; font-weight: 900; color: #0f172a; } .val-score span { font-size: 30px; color: #94a3b8;}

.lockdown-curtain { position: fixed; inset: 0; background: rgba(255,255,255,0.96); z-index: 9999; display: flex; align-items: center; justify-content: center; backdrop-filter: blur(10px);}
.card-shield { background: #fff; padding: 60px; border-radius: 40px; text-align: center; width: 450px;}

.i-track { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; margin-top: 5px;}
.i-fill { height: 100%; background: #0f172a; transition: 0.5s; }
.safe { color: #10b981;} .danger { color: #ef4444;}
</style>