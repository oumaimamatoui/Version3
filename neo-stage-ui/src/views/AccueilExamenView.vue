<template>
  <div class="premium-secure-viewport">
    <!-- COUCHES DÉCORATIVES -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>

    <!-- 1. ÉTAT DE CHARGEMENT -->
    <div v-if="loading" class="centering-lzzayout">
        <div class="loader-container text-center">
            <div class="spinner-pro-premium"></div>
            <h2 class="mt-4 fw-800">CHARGEMENT DE LA SESSION...</h2>
            <p class="text-muted">Sécurisation de la connexion en cours</p>
        </div>
    </div>

    <!-- 2. ÉTAT D'ERREUR (Accès Refusé / Candidature introuvable) -->
    <div v-else-if="errorMsg" class="centering-layout">
        <div class="card-glass-setup text-center animate__animated animate__fadeIn">
            <div class="icon-error-container mb-4">
                <i class="fa-solid fa-shield-halved fa-4x text-danger"></i>
            </div>
            <h1 class="title-display">Accès <span>Refusé</span></h1>
            <p class="error-text mt-3">{{ errorMsg }}</p>
            <div class="divider my-4"></div>
            <button @click="router.push('/dashboard')" class="btn-img btn-dark w-100">
                <i class="fa-solid fa-arrow-left me-2"></i> RETOUR AU TERMINAL
            </button>
        </div>
    </div>

    <!-- 3. ÉCRAN D'ACCUEIL DU TEST -->
    <template v-else>
        <main class="main-terminal">
            <div v-if="!examStarted" class="centering-layout">
                <div class="card-glass-setup animate__animated animate__zoomIn">
                    <div class="badge-tech mb-3">EXAMEN OFFICIEL</div>
                    <h1 class="title-display">Session <span>{{ campaignName }}</span></h1>
                    <p class="text-muted mt-3">
                        Cette évaluation est chronométrée. Assurez-vous d'être dans un environnement calme.
                    </p>
                    
                    <div class="info-summary my-4">
                        <div class="info-pill"><i class="fa-solid fa-list-check me-2"></i> {{ questions.length }} Questions</div>
                        <div class="info-pill"><i class="fa-solid fa-clock me-2"></i> Temps limité</div>
                    </div>

                    <button @click="initiateSession" class="btn-img btn-amber px-5 mt-4 w-100">
                        DÉMARRER LE TEST <i class="fa-solid fa-play ms-2"></i>
                    </button>
                </div>
            </div>
            
            <!-- 4. INTERFACE DU QUIZ (Une fois démarré) -->
            <div v-else class="centering-layout">
                 <div class="surface-card-q animate__animated animate__fadeInUp">
                    <div class="quiz-header d-flex justify-content-between align-items-center mb-5">
                        <span class="question-count">Question {{ currentIndex + 1 }} sur {{ questions.length }}</span>
                        <div class="timer-box"><i class="fa-regular fa-clock me-2"></i> {{ formatTime }}</div>
                    </div>

                    <h2 class="question-title fw-800">{{ questions[currentIndex]?.enonce }}</h2>
                    
                    <div class="options-container mt-5">
                        <div 
                            v-for="(opt, idx) in questions[currentIndex]?.options" 
                            :key="idx" 
                            class="option-card"
                            :class="{ 'selected': selectedOption === idx }"
                            @click="selectOption(idx)"
                        >
                            <div class="option-marker">{{ String.fromCharCode(65 + idx) }}</div>
                            <div class="option-text">{{ opt }}</div>
                        </div>
                    </div>

                    <div class="quiz-footer mt-5 d-flex justify-content-between">
                        <button @click="router.push('/dashboard')" class="btn-img btn-gray">QUITTER</button>
                        <button @click="nextQuestion" class="btn-img btn-dark">SUIVANT <i class="fa-solid fa-chevron-right ms-2"></i></button>
                    </div>
                 </div>
            </div>
        </main>
    </template>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
// IMPORTANT : Utilisation de l'instance API configurée avec le Token
import api from '@/services/api'; 

const route = useRoute();
const router = useRouter();

// --- ÉTATS ---
const loading = ref(true);
const errorMsg = ref(null);
const examStarted = ref(false);
const campaignName = ref("");
const questions = ref([]);
const currentIndex = ref(0);
const evaluationId = ref(null);
const duration = ref(20); // Valeur par défaut
const timeLeft = ref(0);
const timerInterval = ref(null);
const selectedOption = ref(null); // Option sélectionnée par le candidat

const selectOption = (idx) => {
    selectedOption.value = idx;
};

// --- TIMER LOGIC ---
const startTimer = () => {
    timeLeft.value = duration.value * 60;
    timerInterval.value = setInterval(() => {
        if (timeLeft.value > 0) {
            timeLeft.value--;
        } else {
            clearInterval(timerInterval.value);
            finishExam();
        }
    }, 1000);
};

const formatTime = computed(() => {
    const mins = Math.floor(timeLeft.value / 60);
    const secs = timeLeft.value % 60;
    return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
});

const loadData = async () => {
    const cid = route.params.id; 
    if (!cid || cid === 'undefined') {
        errorMsg.value = "Identifiant de session manquant.";
        loading.value = false;
        return;
    }

    try {
        const res = await api.get(`/Examen/setup/${cid}`);
        evaluationId.value = res.data.evaluationId;
        questions.value = (res.data.questions || []).map(q => ({
            ...q,
            options: q.options || q.Options || q.choix || q.Choix || []
        }));
        campaignName.value = res.data.campagneNom || "Évaluation";
        duration.value = Math.floor(res.data.tempsLimite / 60) || 20; // Utiliser tempsLimite du DTO
    } catch (err) {
        errorMsg.value = err.response?.data?.message || "Erreur de chargement.";
    } finally {
        loading.value = false;
    }
};

// --- SÉCURITÉ (ALERTES SUPPRIMÉES) ---
const warnCount = ref(0);

const handleVisibilityChange = () => {
    // Logique désactivée à la demande de l'utilisateur
};

const enterFullScreen = () => {
    const el = document.documentElement;
    if (el.requestFullscreen) el.requestFullscreen();
    else if (el.webkitRequestFullscreen) el.webkitRequestFullscreen();
};

const initiateSession = () => {
    examStarted.value = true;
    enterFullScreen();
    startTimer();
    // Activer la surveillance
    document.addEventListener('visibilitychange', handleVisibilityChange);
};

const nextQuestion = async () => {
    // Sauvegarde auto du progrès
    try {
        await api.post('/Examen/sync', { evaluationId: evaluationId.value });
    } catch (e) { console.error("Sync failed"); }

    selectedOption.value = null;
    if (currentIndex.value < questions.value.length - 1) {
        currentIndex.value++;
    } else {
        finishExam();
    }
};

const finishExam = async () => {
    clearInterval(timerInterval.value);
    document.removeEventListener('visibilitychange', handleVisibilityChange);
    if (document.fullscreenElement) document.exitFullscreen();
    
    try {
        await api.post(`/Examen/terminer/${evaluationId.value}`);
    } catch (e) { console.error("Finish failed"); }

    router.push(`/results/${route.params.id}`);
};

onMounted(loadData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.premium-secure-viewport { height: 100vh; background: #f8fafc; overflow: hidden; position: relative; font-family: 'Plus Jakarta Sans', sans-serif; }
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #fff 0%, #f1f5f9 100%); z-index: 1; }

.glow-orb { position: absolute; border-radius: 50%; filter: blur(130px); opacity: 0.15; z-index: 2; }
.orb-amber { width: 600px; height: 600px; background: #fbbf24; top: -100px; right: -100px; }
.orb-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -100px; left: -100px; }

.centering-layout { height: 100%; display: flex; align-items: center; justify-content: center; z-index: 10; position: relative; padding: 20px; }

.card-glass-setup { 
    background: rgba(255, 255, 255, 0.9); 
    backdrop-filter: blur(10px);
    border-radius: 35px; 
    box-shadow: 0 30px 60px rgba(0,0,0,0.08); 
    padding: 50px; 
    max-width: 550px;
    width: 100%;
}

.surface-card-q { 
    background: white; 
    border-radius: 35px; 
    box-shadow: 0 30px 60px rgba(0,0,0,0.05); 
    padding: 50px; 
    width: 100%;
    max-width: 900px;
}

.title-display { font-weight: 800; font-size: 2.5rem; color: #0f172a; }
.title-display span { color: #eab308; }

.error-text { font-weight: 600; color: #64748b; font-size: 1.1rem; }

.badge-tech { display: inline-block; padding: 6px 15px; background: #f1f5f9; border-radius: 10px; font-weight: 800; font-size: 11px; color: #475569; letter-spacing: 1px; }

.info-summary { display: flex; gap: 15px; justify-content: center; }
.info-pill { background: #f8fafc; padding: 10px 20px; border-radius: 15px; font-weight: 700; color: #1e293b; border: 1px solid #e2e8f0; }

/* BOUTONS */
.btn-img { 
    padding: 18px 30px; 
    border-radius: 20px; 
    border: none; 
    font-weight: 800; 
    cursor: pointer; 
    transition: all 0.3s ease;
    display: flex;
    align-items: center;
    justify-content: center;
}
.btn-amber { background: #eab308; color: #0f172a; }
.btn-amber:hover { background: #ca8a04; transform: translateY(-2px); }
.btn-dark { background: #0f172a; color: white; }
.btn-dark:hover { background: #1e293b; transform: translateY(-2px); }
.btn-gray { background: #f1f5f9; color: #475569; }

/* SPINNER */
.spinner-pro-premium {
    width: 60px; height: 60px;
    border: 5px solid #f1f5f9;
    border-top: 5px solid #eab308;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin: 0 auto;
}
@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }

.timer-box { background: #0f172a; color: white; padding: 10px 20px; border-radius: 15px; font-weight: 800; }
.question-title { color: #0f172a; line-height: 1.4; font-size: 2rem; }
.option-card { 
    padding: 20px 25px; 
    background: #f8fafc; 
    border: 2.5px solid #f1f5f9; 
    border-radius: 20px; 
    margin-bottom: 15px; 
    font-weight: 700; 
    cursor: pointer; 
    transition: 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275); 
    display: flex;
    align-items: center;
    gap: 15px;
}
.option-card:hover { border-color: #eab308; background: #fff; transform: translateX(10px); }
.option-card.selected { border-color: #eab308; background: #fffbeb; box-shadow: 0 10px 25px rgba(234, 179, 8, 0.1); }

.option-marker { 
    width: 35px; height: 35px; background: #0f172a; color: white; 
    border-radius: 10px; display: flex; align-items: center; justify-content: center; 
    font-weight: 800; font-size: 14px; 
}
.option-card.selected .option-marker { background: #eab308; color: #0f172a; }

.divider { height: 1px; background: #e2e8f0; }
</style>