<template>
  <div class="quiz-master-bg">
    <!-- Header: Timers -->
    <div v-if="!quizFinished && currentQuestion && !loading" class="quiz-header shadow-sm">
      <div class="container d-flex justify-content-between align-items-center py-3">
        <div class="brand-logo">Evalua<span>Tech</span></div>
        <div class="timers-container d-flex gap-4">
          <div class="global-timer" :class="{'text-danger': globalTimeLeft < 60}">
            <i class="fa-solid fa-hourglass-half"></i> Global : {{ formatTime(globalTimeLeft) }}
          </div>
          <div v-if="questionTimeLeft !== null" class="question-timer" :class="{'text-danger': questionTimeLeft <= 10}">
            <i class="fa-regular fa-clock"></i> Question : {{ formatTime(questionTimeLeft) }}
          </div>
        </div>
      </div>
    </div>

    <div v-if="loading" class="loading-screen d-flex flex-column justify-content-center align-items-center">
      <div class="spinner-border text-warning" style="width: 3rem; height: 3rem;" role="status"></div>
      <p class="mt-3 fw-bold text-muted">Initialisation de l'environnement de test...</p>
    </div>

    <div v-else-if="!quizFinished && currentQuestion" class="container py-5">
      <!-- Progress -->
      <div class="progress-container mb-5 mx-auto" style="max-width: 800px;">
        <div class="d-flex justify-content-between mb-2">
          <span class="progress-text fw-bold text-muted">Question {{ currentIndex + 1 }} / {{ questions.length }}</span>
          <span class="progress-percent fw-bold text-warning">{{ Math.round(((currentIndex + 1) / questions.length) * 100) }}%</span>
        </div>
        <div class="progress" style="height: 10px; border-radius: 10px; background-color: #e2e8f0;">
          <div class="progress-bar bg-warning" :style="{width: ((currentIndex + 1) / questions.length * 100) + '%'}"></div>
        </div>
      </div>

      <!-- Question Card -->
      <div class="question-card bg-white p-4 p-md-5 rounded-4 shadow-sm mx-auto" style="max-width: 800px;">
        <h3 class="question-title fw-bold text-slate-800 mb-5 lh-base">{{ currentQuestion.enonce }}</h3>

        <div class="options-container">
          <!-- QCU / Vrai/Faux -->
          <template v-if="currentQuestion.type === 'QCU' || currentQuestion.type === 'VRAI_FAUX'">
            <label 
              v-for="(opt, idx) in currentQuestion.options" 
              :key="idx"
              class="option-row d-flex align-items-center p-3 mb-3 rounded-3 border"
              :class="{ 'selected': selectedSingle === opt }"
            >
              <input type="radio" :value="opt" v-model="selectedSingle" class="d-none">
              <div class="option-indicator me-3 d-flex align-items-center justify-content-center">
                <i v-if="selectedSingle === opt" class="fa-solid fa-circle-dot text-warning"></i>
                <i v-else class="fa-regular fa-circle text-muted"></i>
              </div>
              <span class="option-text fw-semibold text-slate-700">
                <span class="text-muted me-2">{{ String.fromCharCode(65 + idx) }} -</span> {{ opt }}
              </span>
            </label>
          </template>

          <!-- QCM -->
          <template v-else-if="currentQuestion.type === 'QCM'">
            <label 
              v-for="(opt, idx) in currentQuestion.options" 
              :key="idx"
              class="option-row d-flex align-items-center p-3 mb-3 rounded-3 border"
              :class="{ 'selected': selectedMultiple.includes(opt) }"
            >
              <input type="checkbox" :value="opt" v-model="selectedMultiple" class="d-none">
              <div class="option-indicator me-3 d-flex align-items-center justify-content-center">
                <i v-if="selectedMultiple.includes(opt)" class="fa-solid fa-square-check text-warning"></i>
                <i v-else class="fa-regular fa-square text-muted"></i>
              </div>
              <span class="option-text fw-semibold text-slate-700">
                <span class="text-muted me-2">{{ String.fromCharCode(65 + idx) }} -</span> {{ opt }}
              </span>
            </label>
          </template>
        </div>

        <div class="action-footer mt-5 d-flex justify-content-end">
          <button @click="nextQuestion" class="btn btn-warning px-5 py-3 fw-bold rounded-3 shadow-sm next-btn" :disabled="!hasAnswered && questionTimeLeft > 0">
            {{ isLastQuestion ? 'TERMINER' : 'QUESTION SUIVANTE' }} <i class="fa-solid fa-arrow-right ms-2"></i>
          </button>
        </div>
      </div>
    </div>

    <!-- Écran de Fin -->
    <div v-else-if="quizFinished" class="container text-center py-5 d-flex flex-column justify-content-center" style="min-height: 80vh;">
      <div class="bg-white p-5 rounded-4 shadow-sm mx-auto animate__animated animate__fadeInUp" style="max-width: 600px;">
        <div class="success-icon mb-4"><i class="fa-solid fa-shield-check text-warning" style="font-size: 70px;"></i></div>
        <h2 class="fw-bold mb-3 text-slate-800">Évaluation Terminée</h2>
        <p class="text-muted mb-5 fs-5">Vos réponses ont été enregistrées avec succès et transmises pour correction.</p>
        <router-link to="/results" class="btn btn-dark px-5 py-3 fw-bold rounded-pill shadow-sm">
          VOIR MES RÉSULTATS <i class="fa-solid fa-award ms-2"></i>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const router = useRouter();

const loading = ref(true);
const quizFinished = ref(false);
const evaluationId = ref(null);
const questions = ref([]);
const currentIndex = ref(0);

// Réponses
const selectedSingle = ref(null);
const selectedMultiple = ref([]);

// Timers
const globalTimeLeft = ref(0);
const questionTimeLeft = ref(null);
let globalTimerInterval = null;
let questionTimerInterval = null;

const currentQuestion = computed(() => questions.value[currentIndex.value] || null);
const isLastQuestion = computed(() => currentIndex.value >= questions.value.length - 1);
const hasAnswered = computed(() => {
  if (!currentQuestion.value) return false;
  if (currentQuestion.value.type === 'QCM') return selectedMultiple.value.length > 0;
  return selectedSingle.value !== null;
});

const formatTime = (seconds) => {
  if (seconds == null || isNaN(seconds)) return "00:00";
  const m = Math.floor(seconds / 60).toString().padStart(2, '0');
  const s = (seconds % 60).toString().padStart(2, '0');
  return `${m}:${s}`;
};

const loadExam = async () => {
  try {
    const res = await api.get(`/Examen/setup/${route.params.id}`);
    const setup = res.data;
    
    evaluationId.value = setup.evaluationId;
    questions.value = setup.questions;
    currentIndex.value = setup.currentIndex || 0;
    globalTimeLeft.value = setup.tempsLimite || 3600;

    if (currentIndex.value >= questions.value.length) {
      quizFinished.value = true;
    } else {
      startTimers();
    }
  } catch (err) {
    alert(err.response?.data?.message || err.response?.data || "Erreur de chargement de l'examen.");
    router.push('/dashboard');
  } finally {
    loading.value = false;
  }
};

const startTimers = () => {
  clearInterval(globalTimerInterval);
  globalTimerInterval = setInterval(() => {
    if (globalTimeLeft.value > 0) {
      globalTimeLeft.value--;
    } else {
      forceFinishExam();
    }
  }, 1000);

  resetQuestionTimer();
};

const resetQuestionTimer = () => {
  clearInterval(questionTimerInterval);
  if (currentQuestion.value && currentQuestion.value.dureeSecondes) {
    questionTimeLeft.value = currentQuestion.value.dureeSecondes;
    questionTimerInterval = setInterval(() => {
      if (questionTimeLeft.value > 0) {
        questionTimeLeft.value--;
      } else {
        // Temps écoulé pour cette question -> On passe à la suivante automatiquement
        nextQuestion();
      }
    }, 1000);
  } else {
    questionTimeLeft.value = null;
  }
};

const saveResponse = async () => {
  if (!currentQuestion.value) return;
  
  let valeur = "";
  if (currentQuestion.value.type === 'QCM') {
    valeur = selectedMultiple.value.sort().join(';'); // On trie et on sépare par point-virgule
  } else {
    valeur = selectedSingle.value || "";
  }

  const payload = {
    evaluationId: evaluationId.value,
    questionId: currentQuestion.value.id,
    valeur: valeur,
    tempsSecondes: currentQuestion.value.dureeSecondes ? (currentQuestion.value.dureeSecondes - questionTimeLeft.value) : 0
  };

  try {
    await api.post('/Examen/save-response', payload);
  } catch (err) {
    console.error("Erreur sauvegarde réponse:", err);
  }
};

const nextQuestion = async () => {
  await saveResponse();

  if (isLastQuestion.value) {
    await finishExam();
  } else {
    currentIndex.value++;
    selectedSingle.value = null;
    selectedMultiple.value = [];
    resetQuestionTimer();
    
    // Sync progress with backend
    api.post('/Examen/sync', { evaluationId: evaluationId.value, currentIndex: currentIndex.value }).catch(()=>{});
  }
};

const finishExam = async () => {
  clearInterval(globalTimerInterval);
  clearInterval(questionTimerInterval);
  try {
    await api.post(`/Examen/terminer/${evaluationId.value}`);
    quizFinished.value = true;
  } catch (err) {
    console.error("Erreur clôture examen", err);
    quizFinished.value = true; // Afficher l'écran de fin quand même
  }
};

const forceFinishExam = async () => {
  alert("Le temps global est écoulé ! L'examen est terminé.");
  await finishExam();
};

onMounted(() => {
  loadExam();
});

onUnmounted(() => {
  clearInterval(globalTimerInterval);
  clearInterval(questionTimerInterval);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.quiz-master-bg {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  display: flex;
  flex-direction: column;
}

.loading-screen {
  flex: 1;
}

.quiz-header {
  background: white;
  border-bottom: 1px solid #f1f5f9;
}

.brand-logo {
  font-size: 22px;
  font-weight: 900;
  color: #0f172a;
}
.brand-logo span {
  color: #fbbf24;
}

.global-timer, .question-timer {
  font-size: 16px;
  font-weight: 700;
  color: #475569;
  background: #f1f5f9;
  padding: 8px 16px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.question-timer {
  background: #fffbeb;
  color: #b45309;
}

.option-row {
  cursor: pointer;
  transition: all 0.2s ease;
  background: #fdfdfd;
  border: 2px solid #f1f5f9 !important;
}

.option-row:hover {
  border-color: #fcd34d !important;
  background: #fffbeb;
  transform: translateY(-2px);
}

.option-row.selected {
  border-color: #fbbf24 !important;
  background: #fffbeb;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.15);
}

.option-indicator i {
  font-size: 20px;
}

.next-btn {
  transition: all 0.3s;
}

.next-btn:hover:not(:disabled) {
  transform: translateX(5px);
}

.text-slate-800 { color: #1e293b; }
.text-slate-700 { color: #334155; }
</style>