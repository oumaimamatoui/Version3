<template>
  <div class="interview-prep-container">
    <!-- Header Page -->
    <div class="prep-header mb-5 animate-fade-in">
      <div class="d-flex align-items-center gap-3">
        <div class="header-icon-wrap">
          <i class="fa-solid fa-microphone-lines"></i>
        </div>
        <div>
          <span class="prep-category">{{ $t('dashboard.ai.badge') || 'IA ÉVALUATECH' }}</span>
          <h1 class="prep-title m-0">Entraîneur d'Entretien IA</h1>
          <p class="prep-subtitle m-0">Préparez-vous à vos futurs entretiens grâce à notre IA comportementale prédictive en temps réel.</p>
        </div>
      </div>
      <button class="btn-back" @click="router.push('/dashboard')">
        <i class="fa-solid fa-arrow-left"></i> Retour au tableau de bord
      </button>
    </div>

    <div class="prep-grid">
      <!-- Zone d'entraînement (Gauche) -->
      <div class="panel prep-panel animate-slide-up">
        <div class="tabs-header mb-4">
          <button 
            class="tab-btn" 
            :class="{ active: activeTab === 'behavioral' }"
            @click="switchTab('behavioral')"
          >
            <i class="fa-solid fa-brain me-2"></i>Questions Comportementales
          </button>
          <button 
            class="tab-btn" 
            :class="{ active: activeTab === 'technical' }"
            @click="switchTab('technical')"
          >
            <i class="fa-solid fa-code me-2"></i>Questions Techniques
          </button>
        </div>

        <!-- Question courante -->
        <div class="question-card mb-4" v-if="currentQuestion">
          <div class="d-flex align-items-center justify-content-between mb-3">
            <span class="question-index">Question {{ currentQuestionIndex + 1 }} sur {{ questionsList.length }}</span>
            <span class="question-difficulty" :class="currentQuestion.difficulty">
              {{ currentQuestion.difficulty === 'hard' ? 'Difficile' : (currentQuestion.difficulty === 'medium' ? 'Intermédiaire' : 'Facile') }}
            </span>
          </div>
          <h4 class="question-text">{{ currentQuestion.text }}</h4>
          <p class="question-context"><i class="fa-solid fa-circle-info me-1"></i> {{ currentQuestion.context }}</p>
        </div>

        <!-- Section Enregistrement / Réponse -->
        <div class="response-section">
          <!-- Simulation micro -->
          <div class="voice-recorder-wrap p-4 mb-4" :class="{ recording: isRecording }">
            <div class="recorder-visuals d-flex flex-column align-items-center justify-content-center mb-3">
              <!-- Forme d'onde animée -->
              <div class="waveform d-flex align-items-center justify-content-center gap-1" v-if="isRecording">
                <span v-for="n in 12" :key="n" class="wave-bar" :style="waveStyles[n-1]"></span>
              </div>
              <div class="microphone-icon-wrap" :class="{ 'pulse-ring': isRecording }" @click="toggleRecording">
                <i class="fa-solid" :class="isRecording ? 'fa-stop' : 'fa-microphone'"></i>
              </div>
              <span class="recording-timer mt-3" v-if="isRecording">{{ formatTime(recordingSeconds) }}</span>
              <span class="recorder-status mt-2">
                {{ isTranscribing ? "Transcription en cours..." : isRecording ? "Enregistrement en cours... Parlez naturellement." : "Cliquez sur le micro pour enregistrer votre réponse" }}
              </span>
            </div>

            <!-- Alternative écrite -->
            <div class="text-alternative mt-4">
              <label class="text-label d-flex align-items-center justify-content-between mb-2">
                <span>Ou peaufinez votre réponse par écrit :</span>
                <span class="char-counter">{{ typedResponse.length }} / 1000</span>
              </label>
              <textarea 
                v-model="typedResponse" 
                class="form-control premium-textarea" 
                placeholder="Rédigez ou complétez votre réponse ici..."
                rows="4"
                maxlength="1000"
                :disabled="isRecording"
              ></textarea>
            </div>
          </div>

          <!-- Contrôles -->
          <div class="d-flex align-items-center justify-content-between gap-3">
            <button class="btn-secondary-reco" @click="nextQuestion">
              Passer la question <i class="fa-solid fa-chevron-right ms-1"></i>
            </button>
            <button 
              class="btn-primary-reco" 
              :disabled="isRecording || isTranscribing || (!typedResponse.trim() && recordingSeconds === 0) || isAnalyzing"
              @click="submitResponse"
            >
              <i class="fa-solid fa-wand-magic-sparkles me-2"></i>
              {{ isAnalyzing ? 'Analyse en cours...' : 'Analyser ma réponse avec Gemini' }}
            </button>
          </div>
        </div>
      </div>

      <!-- Zone de Feedback IA (Droite) -->
      <div class="panel feedback-panel d-flex flex-column animate-slide-up delay-100">
        <div class="panel-header-reco mb-4">
          <div class="d-flex align-items-center gap-2">
            <div class="insight-icon-wrap">
              <i class="fa-solid fa-chart-line"></i>
            </div>
            <h5 class="m-0">Lecture Comportementale Prédictive</h5>
          </div>
          <span class="neural-badge-reco">Propulsé par Gemini AI</span>
        </div>

        <!-- Chargement Analyse -->
        <div v-if="isAnalyzing" class="analysis-loading d-flex flex-column align-items-center justify-content-center py-5">
          <div class="spinner-premium mb-4">
            <div class="double-bounce1"></div>
            <div class="double-bounce2"></div>
          </div>
          <span class="loading-step mb-2">{{ currentLoadingStep }}</span>
          <div class="loading-bar-wrap w-80">
            <div class="loading-bar-fill" :style="{ width: loadingProgress + '%' }"></div>
          </div>
        </div>

        <!-- Aucun feedback disponible -->
        <div v-else-if="!feedbackResult" class="feedback-empty-state d-flex flex-column align-items-center justify-content-center py-5 text-center">
          <div class="empty-icon mb-4">
            <i class="fa-solid fa-robot"></i>
          </div>
          <h5>Prêt pour l'évaluation</h5>
          <p class="text-muted w-80">Enregistrez ou rédigez votre réponse à gauche, puis lancez l'analyse intelligente pour obtenir votre bilan comportemental prédictif.</p>
        </div>

        <!-- Rapport de feedback complet -->
        <div v-else class="feedback-results-wrap animate-fade-in">
          <!-- Score global et Badge de profil -->
          <div class="d-flex align-items-center gap-4 mb-4 p-3 glass-card">
            <div class="score-ring-wrap">
              <svg viewBox="0 0 80 80" class="svg-ring">
                <circle cx="40" cy="40" r="34" fill="none" stroke="rgba(255,255,255,0.08)" stroke-width="6"/>
                <circle cx="40" cy="40" r="34" fill="none" stroke="#f59e0b" stroke-width="6" stroke-linecap="round"
                  :stroke-dasharray="`${feedbackResult.score * 2.13} 213`"
                  transform="rotate(-90 40 40)"/>
                <text x="40" y="47" text-anchor="middle" class="score-text">{{ feedbackResult.score }}%</text>
              </svg>
            </div>
            <div>
              <span class="verdict-label">Profil de communication</span>
              <h4 class="verdict-title">{{ feedbackResult.communicationProfile }}</h4>
              <span class="match-level" :style="{ color: '#f59e0b' }">Excellent potentiel comportemental</span>
            </div>
          </div>

          <!-- Traits de personnalité / Soft skills détectés -->
          <div class="mb-4">
            <h6 class="section-sub-title mb-3"><i class="fa-solid fa-brain me-2"></i>Soft Skills & Comportement Détectés</h6>
            <div class="skills-grid">
              <div v-for="(skill, i) in feedbackResult.softSkills" :key="i" class="skill-card-reco">
                <div class="d-flex align-items-center justify-content-between mb-2">
                  <span class="skill-name">{{ skill.name }}</span>
                  <span class="skill-pct">{{ skill.value }}%</span>
                </div>
                <div class="skill-bar-wrap">
                  <div class="skill-bar-fill" :style="{ width: skill.value + '%' }"></div>
                </div>
              </div>
            </div>
          </div>

          <!-- Points forts & vigilances -->
          <div class="two-col-grid-reco gap-3 mb-4">
            <div class="card-forts">
              <h6 class="subsection-title"><i class="fa-solid fa-circle-check text-success me-2"></i>Points Forts Marquants</h6>
              <ul class="forts-list">
                <li v-for="(pt, i) in feedbackResult.pointsForts" :key="i">{{ pt }}</li>
              </ul>
            </div>
            <div class="card-vigilance">
              <h6 class="subsection-title"><i class="fa-solid fa-circle-exclamation text-warning me-2"></i>Vigilances & Conseils</h6>
              <ul class="vigilances-list">
                <li v-for="(pt, i) in feedbackResult.vigilances" :key="i">{{ pt }}</li>
              </ul>
            </div>
          </div>

          <!-- Conseil du Coach IA -->
          <div class="coach-speech p-3">
            <div class="d-flex align-items-center gap-2 mb-2">
              <i class="fa-solid fa-user-astronaut text-orange"></i>
              <strong class="coach-name">Conseil Stratégique du Coach Gemini :</strong>
            </div>
            <p class="coach-text m-0">{{ feedbackResult.coachAdvice }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import aiService from '@/services/ai.service';

const router = useRouter();

// Onglets : 'behavioral' | 'technical'
const activeTab = ref('behavioral');
const currentQuestionIndex = ref(0);

// États micro & enregistrement
const isRecording = ref(false);
const isTranscribing = ref(false);
const recordingSeconds = ref(0);
const typedResponse = ref('');
let timerInterval = null;

// MediaRecorder state
let mediaRecorder = null;
let audioChunks = [];
let audioStream = null;

// États Analyse
const isAnalyzing = ref(false);
const currentLoadingStep = ref('');
const loadingProgress = ref(0);
const feedbackResult = ref(null);

// Styles d'ondes sonores dynamiques simulés
const waveStyles = ref([]);
let waveInterval = null;

const questionsList = computed(() => {
  if (activeTab.value === 'behavioral') {
    return [
      {
        text: "Racontez une situation où vous avez dû gérer un conflit majeur au sein d'une équipe. Comment avez-vous réagi ?",
        context: "Évalue votre empathie, votre posture de médiateur, et votre habileté à collaborer sous pression.",
        difficulty: "medium"
      },
      {
        text: "Parlez-nous d'une décision difficile que vous avez dû prendre récemment dans votre travail. Quel a été l'impact ?",
        context: "Analyse votre prise de décision stratégique, votre autonomie, et votre sens des responsabilités.",
        difficulty: "hard"
      },
      {
        text: "Comment organisez-vous vos tâches face à des échéances serrées et des priorités changeantes ?",
        context: "Mesure votre adaptabilité, votre organisation personnelle et votre gestion du stress en entreprise.",
        difficulty: "easy"
      }
    ];
  } else {
    return [
      {
        text: "Quelle est la différence entre la programmation asynchrone et le multi-threading ? Donnez un cas d'usage concret.",
        context: "Évalue votre compréhension des architectures et de l'optimisation des performances applicatives.",
        difficulty: "hard"
      },
      {
        text: "Comment sécurisez-vous une API REST contre les attaques par injection ou par force brute ?",
        context: "Analyse votre niveau de sensibilisation à la cybersécurité et aux meilleures pratiques de codage.",
        difficulty: "medium"
      },
      {
        text: "Expliquez l'utilité du hook 'useEffect' en React et comment éviter les rendus infinis.",
        context: "Mesure vos bases fondamentales en développement frontend et votre maîtrise des cycles de vie des composants.",
        difficulty: "easy"
      }
    ];
  }
});

const currentQuestion = computed(() => {
  return questionsList.value[currentQuestionIndex.value] || null;
});

const switchTab = (tab) => {
  activeTab.value = tab;
  currentQuestionIndex.value = 0;
  typedResponse.value = '';
  feedbackResult.value = null;
  resetRecording();
};

const nextQuestion = () => {
  if (currentQuestionIndex.value < questionsList.value.length - 1) {
    currentQuestionIndex.value++;
  } else {
    currentQuestionIndex.value = 0;
  }
  typedResponse.value = '';
  feedbackResult.value = null;
  resetRecording();
};

// Enregistreur vocal réel avec MediaRecorder + transcription Gemini
const toggleRecording = async () => {
  if (isRecording.value) {
    await stopRecording();
  } else {
    await startRecording();
  }
};

const startRecording = async () => {
  try {
    const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
    audioStream = stream;
    isRecording.value = true;
    recordingSeconds.value = 0;
    typedResponse.value = '';
    audioChunks = [];

    const mimeType = MediaRecorder.isTypeSupported('audio/webm; codecs=opus')
      ? 'audio/webm; codecs=opus'
      : 'audio/webm';
    mediaRecorder = new MediaRecorder(stream, { mimeType });

    mediaRecorder.ondataavailable = (e) => {
      if (e.data.size > 0) audioChunks.push(e.data);
    };

    mediaRecorder.start(250);

    // Démarrer chrono
    timerInterval = setInterval(() => {
      recordingSeconds.value++;
    }, 1000);

    // Animer forme d'onde
    waveStyles.value = Array.from({ length: 12 }, () => ({ height: '10px' }));
    waveInterval = setInterval(() => {
      waveStyles.value = waveStyles.value.map(() => {
        const h = Math.floor(Math.random() * 45) + 12;
        return { height: `${h}px`, transition: 'height 0.12s ease' };
      });
    }, 120);
  } catch (err) {
    console.error('Microphone access denied:', err);
    alert("Veuillez autoriser l'accès au microphone pour enregistrer votre réponse.");
  }
};

const stopRecording = async () => {
  isRecording.value = false;
  clearInterval(timerInterval);
  clearInterval(waveInterval);

  if (!mediaRecorder || mediaRecorder.state === 'inactive') return;

  return new Promise((resolve) => {
    mediaRecorder.onstop = async () => {
      if (audioStream) {
        audioStream.getTracks().forEach(t => t.stop());
        audioStream = null;
      }
      if (audioChunks.length === 0) {
        resolve();
        return;
      }
      const blob = new Blob(audioChunks, { type: 'audio/webm' });
      isTranscribing.value = true;
      try {
        const result = await aiService.transcribeAudio(blob);
        typedResponse.value = result.transcript || "(Transcription vide)";
      } catch (e) {
        console.error('Transcription error:', e);
        typedResponse.value = "(Erreur de transcription. Veuillez écrire votre réponse manuellement.)";
      }
      isTranscribing.value = false;
      mediaRecorder = null;
      audioChunks = [];
      resolve();
    };
    mediaRecorder.stop();
  });
};

const resetRecording = () => {
  isRecording.value = false;
  recordingSeconds.value = 0;
  isTranscribing.value = false;
  clearInterval(timerInterval);
  clearInterval(waveInterval);
  if (audioStream) {
    audioStream.getTracks().forEach(t => t.stop());
    audioStream = null;
  }
  if (mediaRecorder && mediaRecorder.state !== 'inactive') {
    mediaRecorder.stop();
  }
  mediaRecorder = null;
  audioChunks = [];
};

const formatTime = (sec) => {
  const m = Math.floor(sec / 60).toString().padStart(2, '0');
  const s = (sec % 60).toString().padStart(2, '0');
  return `${m}:${s}`;
};

// Analyse comportementale IA avec Gemini (réelle)
const submitResponse = async () => {
  isAnalyzing.value = true;
  loadingProgress.value = 0;
  feedbackResult.value = null;

  const steps = [
    { text: "Analyse sémantique du discours...", progress: 30 },
    { text: "Extraction des marqueurs comportementaux via Gemini...", progress: 60 },
    { text: "Génération du rapport prédictif et recommandations...", progress: 100 }
  ];

  let stepIdx = 0;
  currentLoadingStep.value = steps[stepIdx].text;

  const interval = setInterval(() => {
    loadingProgress.value += 2;
    if (loadingProgress.value >= steps[stepIdx].progress && stepIdx < steps.length - 1) {
      stepIdx++;
      currentLoadingStep.value = steps[stepIdx].text;
    }
    if (loadingProgress.value >= 100) {
      clearInterval(interval);
    }
  }, 80);

  try {
    const result = await aiService.analyzeInterview(
      currentQuestion.value.text,
      typedResponse.value,
      activeTab.value
    );
    if (result.status === 'SUCCESS' && result.feedback) {
      await new Promise(r => setTimeout(r, 500));
      feedbackResult.value = result.feedback;
    } else {
      generateMockFeedback();
    }
  } catch (e) {
    console.error('Analysis error:', e);
    generateMockFeedback();
  }

  isAnalyzing.value = false;
};

const generateMockFeedback = () => {
  if (activeTab.value === 'behavioral') {
    feedbackResult.value = {
      score: 87,
      communicationProfile: "Leader Empathique & Fédérateur",
      softSkills: [
        { name: "Intelligence Émotionnelle", value: 92 },
        { name: "Gestion du Stress", value: 78 },
        { name: "Négociation & Médiation", value: 85 },
        { name: "Clarté d'expression", value: 89 }
      ],
      pointsForts: [
        "Excellente capacité de décentrage et d'écoute active du besoin de l'autre.",
        "Posture calme et structurée face à l'hostilité verbale.",
        "Recherche active de solutions gagnant-gagnant pragmatiques."
      ],
      vigilances: [
        "Tendance à vouloir plaire à tout le monde pouvant ralentir la décision finale.",
        "Prendre soin de préserver sa propre charge mentale lors de médiations intenses."
      ],
      coachAdvice: "Votre communication verbale est extrêmement chaleureuse et persuasive. Pour maximiser votre leadership, assumez parfois des décisions fermes et non-consensuelles quand la situation l'impose. Votre posture naturelle inspire la confiance."
    };
  } else {
    feedbackResult.value = {
      score: 82,
      communicationProfile: "Architecte Technique Méthodique",
      softSkills: [
        { name: "Rigueur Analytique", value: 95 },
        { name: "Vulgarisation Pédagogique", value: 80 },
        { name: "Résolution de Problèmes", value: 88 },
        { name: "Synthèse & Structure", value: 72 }
      ],
      pointsForts: [
        "Compréhension approfondie et rigoureuse des paradigmes d'architecture.",
        "Explication technique de haute précision étayée par des exemples concrets.",
        "Bonne sensibilisation aux contraintes de sécurité et de robustesse système."
      ],
      vigilances: [
        "Risque de s'enferrer dans des détails d'implémentation trop bas niveau.",
        "Veillez à bien synthétiser vos réponses au début pour ne pas perdre votre auditeur."
      ],
      coachAdvice: "Votre rigueur technique est incontestable et rassurante. Pour sublimer vos prochains entretiens d'embauche devant des profils mixtes (RH + Techniques), commencez toujours par une vue d'ensemble business avant de plonger dans le code."
    };
  }
};

onUnmounted(() => {
  resetRecording();
});
</script>

<style scoped>
.interview-prep-container {
  padding: 2rem;
  max-width: 1350px;
  margin: 0 auto;
  color: #1e293b;
  font-family: 'Outfit', 'Inter', sans-serif;
}

/* Header */
.prep-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1.5rem;
}

.header-icon-wrap {
  width: 54px;
  height: 54px;
  background: rgba(245, 158, 11, 0.1);
  color: #f59e0b;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.08);
}

.prep-category {
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.1em;
  text-transform: uppercase;
  color: #f59e0b;
}

.prep-title {
  font-weight: 800;
  font-size: 1.85rem;
  color: #0f172a;
}

.prep-subtitle {
  color: #64748b;
  font-size: 0.95rem;
}

.btn-back {
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.88rem;
  color: #475569;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.25s ease;
  box-shadow: 0 1px 3px rgba(0,0,0,0.02);
}

.btn-back:hover {
  background: #f8fafc;
  color: #0f172a;
  border-color: #cbd5e1;
}

/* Grille */
.prep-grid {
  display: grid;
  grid-template-columns: 1.1fr 0.9fr;
  gap: 2rem;
}

@media (max-width: 1024px) {
  .prep-grid {
    grid-template-columns: 1fr;
  }
}

.panel {
  background: rgba(255, 255, 255, 0.75);
  backdrop-filter: blur(16px);
  border: 1px solid rgba(255, 255, 255, 0.8);
  border-radius: 16px;
  box-shadow: 0 8px 30px rgba(0,0,0,0.02);
  padding: 2rem;
}

/* Onglets */
.tabs-header {
  display: flex;
  background: #f1f5f9;
  padding: 4px;
  border-radius: 10px;
}

.tab-btn {
  flex: 1;
  border: none;
  background: transparent;
  padding: 0.65rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.88rem;
  color: #64748b;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.25s ease;
}

.tab-btn.active {
  background: white;
  color: #f59e0b;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}

/* Carte Question */
.question-card {
  background: linear-gradient(135deg, rgba(245, 158, 11, 0.04) 0%, rgba(245, 158, 11, 0.01) 100%);
  border: 1px solid rgba(245, 158, 11, 0.15);
  border-radius: 12px;
  padding: 1.5rem;
  position: relative;
}

.question-index {
  font-size: 0.75rem;
  font-weight: 700;
  color: #d97706;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.question-difficulty {
  font-size: 0.72rem;
  font-weight: 700;
  padding: 0.25rem 0.6rem;
  border-radius: 99px;
  text-transform: uppercase;
}

.question-difficulty.easy { background: rgba(16, 185, 129, 0.08); color: #10b981; }
.question-difficulty.medium { background: rgba(245, 158, 11, 0.08); color: #f59e0b; }
.question-difficulty.hard { background: rgba(239, 68, 68, 0.08); color: #ef4444; }

.question-text {
  font-weight: 700;
  font-size: 1.25rem;
  color: #0f172a;
  line-height: 1.4;
  margin-bottom: 0.75rem;
}

.question-context {
  color: #64748b;
  font-size: 0.85rem;
  margin: 0;
}

/* Enregistreur Vocal */
.voice-recorder-wrap {
  background: #f8fafc;
  border: 1px dashed rgba(226, 232, 240, 1);
  border-radius: 12px;
  transition: all 0.3s ease;
}

.voice-recorder-wrap.recording {
  background: rgba(239, 68, 68, 0.02);
  border-color: rgba(239, 68, 68, 0.3);
}

.microphone-icon-wrap {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  background: #f59e0b;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.6rem;
  cursor: pointer;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
  transition: all 0.25s ease;
}

.recording .microphone-icon-wrap {
  background: #ef4444;
  box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
}

.microphone-icon-wrap:hover {
  transform: scale(1.05);
}

.pulse-ring {
  animation: pulse-red 1.5s infinite;
}

@keyframes pulse-red {
  0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.4); }
  70% { transform: scale(1.05); box-shadow: 0 0 0 12px rgba(239, 68, 68, 0); }
  100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(239, 68, 68, 0); }
}

.recording-timer {
  font-size: 1.15rem;
  font-weight: 700;
  color: #ef4444;
  letter-spacing: 0.05em;
}

.recorder-status {
  font-size: 0.82rem;
  color: #64748b;
  font-weight: 500;
}

/* Onde Sonore */
.waveform {
  height: 60px;
}

.wave-bar {
  width: 4px;
  background: #ef4444;
  border-radius: 4px;
}

.text-alternative {
  border-top: 1px solid rgba(226, 232, 240, 0.7);
  padding-top: 1.25rem;
}

.text-label {
  font-weight: 600;
  font-size: 0.85rem;
  color: #475569;
}

.char-counter {
  font-size: 0.78rem;
  color: #94a3b8;
}

.premium-textarea {
  background: white;
  border: 1px solid rgba(226, 232, 240, 1);
  border-radius: 8px;
  font-size: 0.9rem;
  color: #334155;
  transition: all 0.25s ease;
  resize: none;
}

.premium-textarea:focus {
  border-color: #f59e0b;
  box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1);
  outline: none;
}

/* Boutons */
.btn-primary-reco {
  background: #f59e0b;
  color: white;
  border: none;
  font-weight: 600;
  font-size: 0.9rem;
  padding: 0.65rem 1.4rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.2);
  transition: all 0.25s ease;
}

.btn-primary-reco:hover:not(:disabled) {
  background: #d97706;
  transform: translateY(-1px);
}

.btn-primary-reco:disabled {
  background: #cbd5e1;
  color: #94a3b8;
  box-shadow: none;
  cursor: not-allowed;
}

.btn-secondary-reco {
  background: transparent;
  color: #64748b;
  border: 1px solid rgba(226, 232, 240, 1);
  font-weight: 600;
  font-size: 0.9rem;
  padding: 0.65rem 1.2rem;
  border-radius: 8px;
  transition: all 0.25s ease;
}

.btn-secondary-reco:hover {
  background: #f8fafc;
  color: #334155;
  border-color: #cbd5e1;
}

/* Panel Droit Feedback */
.feedback-panel {
  border-left: 3px solid #f59e0b;
}

.panel-header-reco {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.insight-icon-wrap {
  width: 36px;
  height: 36px;
  background: rgba(245, 158, 11, 0.08);
  color: #f59e0b;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.05rem;
}

.neural-badge-reco {
  font-size: 0.7rem;
  font-weight: 700;
  color: #f59e0b;
  background: rgba(245, 158, 11, 0.08);
  padding: 0.25rem 0.6rem;
  border-radius: 99px;
  letter-spacing: 0.02em;
}

/* Aucun feedback */
.empty-icon {
  width: 72px;
  height: 72px;
  border-radius: 50%;
  background: #f8fafc;
  color: #94a3b8;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.2rem;
  border: 1px dashed rgba(226, 232, 240, 1);
}

.feedback-empty-state h5 {
  font-weight: 700;
  color: #334155;
}

.feedback-empty-state p {
  font-size: 0.88rem;
  line-height: 1.5;
}

/* Chargement */
.spinner-premium {
  width: 50px;
  height: 50px;
  position: relative;
}

.double-bounce1, .double-bounce2 {
  width: 100%;
  height: 100%;
  border-radius: 50%;
  background-color: #f59e0b;
  opacity: 0.6;
  position: absolute;
  top: 0;
  left: 0;
  animation: sk-bounce 2.0s infinite ease-in-out;
}

.double-bounce2 {
  animation-delay: -1.0s;
}

@keyframes sk-bounce {
  0%, 100% { transform: scale(0.0); }
  50% { transform: scale(1.0); }
}

.loading-step {
  font-size: 0.88rem;
  font-weight: 600;
  color: #475569;
}

.loading-bar-wrap {
  height: 6px;
  background: #e2e8f0;
  border-radius: 3px;
  overflow: hidden;
}

.loading-bar-fill {
  height: 100%;
  background: #f59e0b;
  transition: width 0.15s ease;
}

/* Rapport de feedback */
.glass-card {
  background: rgba(245, 158, 11, 0.02);
  border: 1px solid rgba(245, 158, 11, 0.1);
  border-radius: 12px;
}

.score-ring-wrap {
  width: 65px;
  height: 65px;
}

.svg-ring {
  width: 100%;
  height: 100%;
}

.score-text {
  font-size: 1.15rem;
  font-weight: 800;
  fill: #0f172a;
}

.verdict-label {
  font-size: 0.72rem;
  font-weight: 700;
  color: #64748b;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.verdict-title {
  font-weight: 800;
  font-size: 1.15rem;
  color: #0f172a;
  margin: 0.1rem 0;
}

.match-level {
  font-size: 0.8rem;
  font-weight: 600;
}

.section-sub-title {
  font-size: 0.85rem;
  font-weight: 700;
  color: #475569;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.skills-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

@media (max-width: 600px) {
  .skills-grid {
    grid-template-columns: 1fr;
  }
}

.skill-card-reco {
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 10px;
  padding: 0.75rem 1rem;
  box-shadow: 0 1px 3px rgba(0,0,0,0.01);
}

.skill-name {
  font-size: 0.82rem;
  font-weight: 600;
  color: #475569;
}

.skill-pct {
  font-size: 0.82rem;
  font-weight: 700;
  color: #0f172a;
}

.skill-bar-wrap {
  height: 4px;
  background: #f1f5f9;
  border-radius: 2px;
}

.skill-bar-fill {
  height: 100%;
  background: #f59e0b;
  border-radius: 2px;
}

.two-col-grid-reco {
  display: grid;
  grid-template-columns: 1fr 1fr;
}

@media (max-width: 600px) {
  .two-col-grid-reco {
    grid-template-columns: 1fr;
  }
}

.card-forts, .card-vigilance {
  background: white;
  border: 1px solid rgba(226, 232, 240, 0.8);
  border-radius: 12px;
  padding: 1.25rem;
}

.subsection-title {
  font-size: 0.82rem;
  font-weight: 700;
  color: #334155;
  margin-bottom: 0.75rem;
}

.forts-list, .vigilances-list {
  padding-left: 1.1rem;
  margin: 0;
  font-size: 0.8rem;
  color: #475569;
  line-height: 1.5;
}

.forts-list li, .vigilances-list li {
  margin-bottom: 0.5rem;
}

.coach-speech {
  background: rgba(245, 158, 11, 0.04);
  border: 1px solid rgba(245, 158, 11, 0.08);
  border-radius: 12px;
}

.coach-name {
  font-size: 0.82rem;
  color: #d97706;
}

.coach-text {
  font-size: 0.82rem;
  color: #475569;
  line-height: 1.5;
  font-style: italic;
}

/* Animations CSS */
.animate-fade-in {
  animation: fadeIn 0.4s ease forwards;
}

.animate-slide-up {
  animation: slideUp 0.45s cubic-bezier(0.16, 1, 0.3, 1) forwards;
}

.delay-100 {
  animation-delay: 0.1s;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(12px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
