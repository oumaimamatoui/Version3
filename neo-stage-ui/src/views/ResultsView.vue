<template>
  <div class="results-root d-flex overflow-hidden">

    <!-- ══ BACKGROUND ══ -->
    <div class="cyber-engine-bg">
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-blue"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">

        <!-- ══ LOADER ══ -->
        <div v-if="loading" class="full-loader">
          <div class="spinner-pro-premium"></div>
          <p class="loader-label">CHARGEMENT DU RAPPORT…</p>
        </div>

        <!-- ══ ERREUR ══ -->
        <div v-else-if="error" class="full-loader">
          <div class="error-icon"><i class="fa-solid fa-triangle-exclamation fa-3x text-danger"></i></div>
          <p class="loader-label mt-3 text-danger">{{ error }}</p>
          <button class="btn-enigma-primary mt-4" @click="$router.push('/historique')">
            <div class="btn-content"><i class="fa-solid fa-arrow-left me-2"></i>Retour à l'historique</div>
            <div class="btn-glow"></div>
          </button>
        </div>

        <!-- ══ CONTENU ══ -->
        <div v-else class="dashboard-view p-4 p-lg-5">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root" @click="$router.push('/dashboard')">Accueil</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="root" @click="$router.push('/historique')">Historique</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Rapport de Session</span>
              </div>
              <h2 class="premium-title">Diagnostic &amp; <span class="gradient-text">Résultats</span></h2>
              <div class="d-flex flex-wrap gap-2 mt-2">
                <span class="meta-badge">
                  <i class="fa-solid fa-fingerprint me-1"></i>
                  SESSION : {{ shortId }}
                </span>
                <span v-if="campagneName" class="meta-badge">
                  <i class="fa-solid fa-building me-1"></i>
                  {{ campagneName }}
                </span>
              </div>
            </div>

            <div class="d-flex align-items-center gap-3 flex-wrap">
              <button class="btn-refresh-pro" @click="fetchResults" title="Rafraîchir">
                <i class="fa-solid fa-rotate"></i>
              </button>
              <button class="btn-enigma-primary shadow-premium" @click="generatePDF">
                <div class="btn-content"><i class="fa-solid fa-file-pdf me-2"></i>GÉNÉRER LE RAPPORT</div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- KPI CARDS -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="kpi in kpiCards" :key="kpi.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: kpi.bg, color: kpi.color }">
                  <i :class="kpi.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value" :style="{ color: kpi.color }">{{ kpi.value }}</div>
                  <div class="stat-label">{{ kpi.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- BENTO LAYOUT -->
          <div class="bento-layout mb-5">

            <!-- ── SCORE CIRCLE ── -->
            <div class="enigma-card p-5 score-card">
              <div class="card-label mb-4">SCORE D'APTITUDE</div>

              <div class="score-ring-outer mx-auto">
                <svg viewBox="0 0 200 200" width="200" height="200">
                  <circle cx="100" cy="100" r="80" class="ring-track"/>
                  <circle cx="100" cy="100" r="80" class="ring-progress" :style="ringProgressStyle"/>
                </svg>
                <div class="ring-center">
                  <div class="ring-pct" :class="isPassed ? 'text-success' : 'text-danger-pro'">
                    {{ result.pourcentage }}%
                  </div>
                  <div class="ring-pts">{{ displayScore }} pts</div>
                </div>
              </div>

              <div class="d-flex justify-content-center mt-4">
                <span class="status-badge" :class="isPassed ? 'status-1' : 'status-2'">
                  <span class="status-dot"></span>
                  {{ isPassed ? 'ADMIS' : 'ÉCHEC' }}
                </span>
              </div>
              <p class="seuil-note text-center mt-2">Seuil requis : {{ SCORE_REUSSITE }}%</p>

              <div class="rep-row mt-4">
                <div class="rep-item rep-ok">
                  <i class="fa-solid fa-check"></i>
                  <span class="rep-val">{{ correctCount }}</span>
                  <span class="rep-lbl">Correctes</span>
                </div>
                <div class="rep-item rep-ko">
                  <i class="fa-solid fa-xmark"></i>
                  <span class="rep-val">{{ incorrectCount }}</span>
                  <span class="rep-lbl">Incorrectes</span>
                </div>
                <div class="rep-item rep-skip">
                  <i class="fa-solid fa-minus"></i>
                  <span class="rep-val">{{ skippedCount }}</span>
                  <span class="rep-lbl">Ignorées</span>
                </div>
              </div>
            </div>

            <!-- ── MÉTRIQUES ── -->
            <div class="enigma-card p-5 metrics-card">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="card-label">MÉTRIQUES DE SESSION</div>
                <div class="ai-badge">
                  <span class="ai-dot"></span>
                  SYSTÈME D'ÉVALUATION IA
                </div>
              </div>

              <div class="metrics-list mb-4">
                <div class="metric-row" v-for="m in metricRows" :key="m.label">
                  <div class="d-flex justify-content-between align-items-center mb-2">
                    <span class="metric-icon-label">
                      <i :class="m.icon" :style="{ color: m.color }"></i>
                      {{ m.label }}
                    </span>
                    <strong class="metric-val">{{ m.value }}</strong>
                  </div>
                  <div class="metric-track">
                    <div class="metric-fill" :style="{ width: m.pct + '%', background: m.fill }"></div>
                  </div>
                </div>
              </div>

              <div class="anticheat-box" :class="integrityScore >= 70 ? 'ac-ok' : 'ac-warn'">
                <i class="fa-solid fa-shield-halved me-2"></i>
                <div>
                  <strong>Anti-Cheat v2.0</strong>
                  <span>{{ result.infractions }} infraction(s) — Intégrité : {{ integrityScore }}%</span>
                </div>
              </div>

              <div class="d-flex flex-wrap gap-2 mt-3 mb-3">
                <span v-for="tag in resultTags" :key="tag" class="pin-badge">{{ tag }}</span>
              </div>

              <button class="btn-voir-correction w-100" @click="toggleCorrection">
                <i class="fa-solid fa-magnifying-glass me-2"></i>
                {{ showCorrection ? 'MASQUER LA CORRECTION' : 'VOIR LA CORRECTION' }}
              </button>
            </div>

            <!-- ── COACH IA ── -->
            <div class="enigma-card p-5 coach-card">
              <div class="ia-coach-terminal">
                <div class="robot-glow-container"><i class="fa-solid fa-robot text-white"></i></div>
                <div class="coach-text-v8">
                  <h6>Coach EvaluaIA</h6>
                  <p class="m-0 small">{{ coachMessage }}</p>
                </div>
              </div>
              <div class="d-flex flex-wrap gap-2 mt-4">
                <span v-for="tag in resultTags" :key="tag" class="pin-badge">{{ tag }}</span>
              </div>
            </div>

          </div>

          <!-- ── THÈMES ── -->
          <div v-if="themeBreakdown.length" class="enigma-card p-5 mb-5">
            <div class="card-label mb-4">ANALYSE PAR THÈME</div>
            <div class="theme-list">
              <div v-for="th in themeBreakdown" :key="th.name" class="theme-row">
                <div class="d-flex justify-content-between mb-2">
                  <span class="theme-name fw-800 small">{{ th.name }}</span>
                  <span class="small fw-800" :class="th.pct >= 70 ? 'text-success' : 'text-danger-pro'">
                    {{ th.correct }}/{{ th.total }} · {{ th.pct }}%
                  </span>
                </div>
                <div class="metric-track">
                  <div class="metric-fill"
                    :style="{ width: th.pct + '%', background: th.pct >= 70 ? '#10b981' : '#f43f5e' }">
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ══ CORRECTION DÉTAILLÉE ══ -->
          <transition name="slide-down">
            <div v-if="showCorrection && normalizedCorrection.length" class="correction-section">
              <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
                <div class="card-label">CORRECTION DÉTAILLÉE</div>
                <div class="d-flex gap-2 flex-wrap">
                  <button
                    v-for="f in corrFilters" :key="f.val"
                    class="btn-icon-sm corr-filter-btn"
                    :class="{ active: corrFilter === f.val }"
                    @click="corrFilter = f.val"
                    style="height:auto;padding:6px 14px;font-size:0.72rem;font-weight:800;"
                  >
                    <span class="cf-dot" :class="'cfd-' + f.val"></span>
                    {{ f.label }} ({{ f.count }})
                  </button>
                </div>
              </div>

              <div class="corr-list">
                <div
                  v-for="(item, idx) in filteredCorrection" :key="idx"
                  class="enigma-card mb-4"
                  :class="item.isCorrect ? 'cc-ok' : (item.userAnswer ? 'cc-ko' : 'cc-skip')"
                  style="overflow:hidden;"
                >
                  <!-- TOP BAR -->
                  <div class="cc-top d-flex justify-content-between align-items-center px-4 py-3">
                    <div class="d-flex align-items-center gap-2">
                      <span class="card-label">QUESTION {{ item.originalIndex + 1 }}</span>
                      <span v-if="item.theme" class="pin-badge">{{ item.theme }}</span>
                      <span class="slot-badge">{{ item.points }} pts</span>
                    </div>
                    <span class="status-badge"
                      :class="item.isCorrect ? 'status-1' : (item.userAnswer ? 'status-2' : 'status-skip')">
                      <span class="status-dot"></span>
                      {{ item.isCorrect ? 'CORRECT' : (item.userAnswer ? 'INCORRECT' : 'IGNORÉ') }}
                    </span>
                  </div>

                  <div class="px-4 pb-4 pt-3">
                    <h5 class="fw-800 mb-4" style="font-size:0.95rem;">{{ item.enonce }}</h5>

                    <!-- Options QCU/QCM -->
                    <div v-if="item.options && item.options.length" class="cc-options">
                      <div
                        v-for="(opt, oi) in item.options" :key="oi"
                        class="cc-opt"
                        :class="{
                          'cco-correct': item.correctIndexes.includes(oi),
                          'cco-user':    item.userIndexes.includes(oi) && !item.correctIndexes.includes(oi),
                          'cco-user-ok': item.userIndexes.includes(oi) && item.correctIndexes.includes(oi)
                        }"
                      >
                        <div class="cco-letter">{{ String.fromCharCode(65 + oi) }}</div>
                        <div class="cco-text flex-grow-1">{{ opt }}</div>
                        <i v-if="item.correctIndexes.includes(oi)" class="fa-solid fa-check" style="color:#10b981"></i>
                        <i v-else-if="item.userIndexes.includes(oi)" class="fa-solid fa-xmark" style="color:#f43f5e"></i>
                      </div>
                    </div>

                    <!-- Texte libre -->
                    <div v-else class="row g-3">
                      <div class="col-md-6">
                        <div class="preview-field-box p-3 rounded-3"
                          :style="item.isCorrect ? 'background:#f0fdf4;border:1.5px solid #6ee7b7;' : 'background:#fff1f2;border:1.5px solid #fca5a5;'">
                          <span class="card-label d-block mb-2">VOTRE RÉPONSE</span>
                          <p class="fw-800 small m-0">{{ item.userAnswer || 'AUCUNE RÉPONSE' }}</p>
                        </div>
                      </div>
                      <div v-if="!item.isCorrect" class="col-md-6">
                        <div class="preview-field-box p-3 rounded-3"
                          style="background:#f0fdf4;border:1.5px solid #6ee7b7;">
                          <span class="card-label d-block mb-2">RÉPONSE CORRECTE</span>
                          <p class="fw-800 small m-0">{{ item.correctAnswer }}</p>
                        </div>
                      </div>
                    </div>

                    <div v-if="item.explication" class="cc-explication mt-3">
                      <i class="fa-solid fa-lightbulb me-2 text-amber"></i>
                      <span>{{ item.explication }}</span>
                    </div>
                  </div>
                </div>

                <div v-if="filteredCorrection.length === 0" class="empty-state-pro py-5 text-center">
                  <i class="fa-solid fa-check-double fa-2x text-success mb-3"></i>
                  <p class="fw-800">Aucune question dans cette catégorie.</p>
                </div>
              </div>
            </div>
          </transition>

        </div>
      </main>
    </div>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="toast.active" class="enigma-toast" :class="'t-' + toast.type">
        <div class="t-ico"><i :class="toast.icon"></i></div>
        <div class="t-body"><strong>SYSTÈME</strong><p class="m-0 small">{{ toast.message }}</p></div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar   from '../components/AppNavbar.vue';

// ────────────────────────────────────────────────────────────────
//  BACKEND : GET /api/Examen/results/{evaluationId}
//  Réponse exacte du contrôleur C# :
//  {
//    Pourcentage        : int        ← Math.Round(eval.ScorePourcentage)
//    ScoreTotal         : float      ← eval.ScoreTotal
//    DetailedCorrection : [
//      { Enonce, UserAnswer, CorrectAnswer, IsCorrect,
//        Options:string[], Theme, Points:float, Explication }
//    ]
//    Infractions        : int        ← 0 (hardcodé pour l'instant)
//  }
//
//  ABSENTS dans /results (récupérés ailleurs ou constants) :
//    ScoreReussite  → SCORE_REUSSITE = 70  (identique au backend)
//    CampagneNom    → appel secondaire GET /Examen/historique
// ────────────────────────────────────────────────────────────────

const SCORE_REUSSITE = 70;

const route  = useRoute();
const router = useRouter();

const loading        = ref(true);
const error          = ref('');
const showCorrection = ref(false);
const corrFilter     = ref('all');
const campagneName   = ref('');

const result = ref({
  pourcentage:        0,
  scoreTotal:         0,
  infractions:        0,
  detailedCorrection: [],
});

const toast = reactive({ active: false, message: '', type: 'success', icon: '' });
let _toastTimer = null;

const evaluationId = computed(() => route.params.id);

const shortId = computed(() => {
  const id = String(evaluationId.value || '');
  return (id.length > 12 ? id.slice(0, 12) : id).toUpperCase() + (id.length > 12 ? '…' : '');
});

const displayScore = computed(() => {
  const s = result.value.scoreTotal;
  return s % 1 === 0 ? s : parseFloat(s.toFixed(1));
});

/* ─── FETCH ─────────────────────────────────────────────────── */
const fetchResults = async () => {
  loading.value = true;
  error.value   = '';

  try {
    const res = await api.get(`/Examen/results/${evaluationId.value}`);
    const raw = res.data;

    result.value = {
      pourcentage:        raw.pourcentage        ?? raw.Pourcentage        ?? 0,
      scoreTotal:         raw.scoreTotal         ?? raw.ScoreTotal         ?? 0,
      infractions:        raw.infractions        ?? raw.Infractions        ?? 0,
      detailedCorrection: raw.detailedCorrection ?? raw.DetailedCorrection ?? [],
    };

    try {
      const hist  = await api.get('/Examen/historique');
      const entry = (hist.data || []).find(h =>
        String(h.id ?? h.Id) === String(evaluationId.value)
      );
      if (entry) campagneName.value = entry.titreExamen ?? entry.TitreExamen ?? '';
    } catch { /* non critique */ }

  } catch (err) {
    const status = err?.response?.status;
    if (status === 404) {
      error.value = 'Session introuvable. Vérifiez l\'identifiant.';
    } else if (status === 401 || status === 403) {
      error.value = 'Accès non autorisé. Veuillez vous reconnecter.';
    } else {
      error.value = err?.response?.data?.message
                 ?? err?.response?.data
                 ?? 'Erreur lors du chargement du rapport.';
    }
  } finally {
    loading.value = false;
  }
};

/* ─── NORMALISATION detailedCorrection ──────────────────────── */
const normalizedCorrection = computed(() =>
  (result.value.detailedCorrection || []).map((raw_item, idx) => {
    const options   = raw_item.options        ?? raw_item.Options        ?? [];
    const userRaw   = String(raw_item.userAnswer    ?? raw_item.UserAnswer    ?? '').trim();
    const corrRaw   = String(raw_item.correctAnswer ?? raw_item.CorrectAnswer ?? '').trim();
    const isCorrect = raw_item.isCorrect      ?? raw_item.IsCorrect      ?? false;

    let userIndexes = [];
    if (options.length > 0 && userRaw !== '') {
      const parts   = userRaw.split(';').map(s => s.trim()).filter(Boolean);
      const allNums = parts.length > 0 && parts.every(p => !isNaN(Number(p)));
      if (allNums) {
        userIndexes = parts.map(Number).filter(n => n >= 0 && n < options.length);
      }
    }

    let correctIndexes = [];
    if (options.length > 0 && corrRaw !== '') {
      const clean = corrRaw.replace(/^la bonne réponse était\s*:\s*/i, '').trim();
      const parts = clean.split('|').map(s => s.trim().toLowerCase()).filter(Boolean);
      correctIndexes = options
        .map((opt, i) => parts.includes(String(opt).trim().toLowerCase()) ? i : -1)
        .filter(i => i !== -1);
    }

    let userAnswerDisplay = userRaw;
    if (options.length > 0 && userIndexes.length > 0) {
      userAnswerDisplay = userIndexes.map(i => options[i]).join(', ');
    }

    const correctAnswerDisplay = corrRaw.replace(/^la bonne réponse était\s*:\s*/i, '').trim();

    return {
      enonce:          raw_item.enonce       ?? raw_item.Enonce       ?? '',
      userAnswer:      userAnswerDisplay,
      correctAnswer:   correctAnswerDisplay,
      isCorrect,
      options,
      userIndexes,
      correctIndexes,
      theme:           raw_item.theme        ?? raw_item.Theme        ?? 'Général',
      points:          raw_item.points       ?? raw_item.Points       ?? 1,
      explication:     raw_item.explication  ?? raw_item.Explication  ?? '',
      originalIndex:   idx,
    };
  })
);

/* ─── COMPUTED ─────────────────────────────────────────────── */
const isPassed       = computed(() => result.value.pourcentage >= SCORE_REUSSITE);
const integrityScore = computed(() => Math.max(0, 100 - (result.value.infractions || 0) * 10));
const correctCount   = computed(() => normalizedCorrection.value.filter(q => q.isCorrect).length);
const incorrectCount = computed(() => normalizedCorrection.value.filter(q => !q.isCorrect && q.userAnswer).length);
const skippedCount   = computed(() => normalizedCorrection.value.filter(q => !q.userAnswer).length);

const ringProgressStyle = computed(() => {
  const circ = 2 * Math.PI * 80;
  const fill = (result.value.pourcentage / 100) * circ;
  return {
    strokeDasharray:  `${fill} ${circ}`,
    stroke:           isPassed.value ? '#10b981' : '#f43f5e',
    strokeDashoffset: circ * 0.25,
    transition:       'stroke-dasharray 1.5s ease-out',
  };
});

const kpiCards = computed(() => [
  { label: 'SCORE GLOBAL',   value: result.value.pourcentage + '%',   icon: 'fa-solid fa-star',           color: '#f59e0b', bg: '#fffbeb' },
  { label: 'INTÉGRITÉ',      value: integrityScore.value + '%',        icon: 'fa-solid fa-shield-halved',  color: '#10b981', bg: '#ecfdf5' },
  { label: 'QUESTIONS',      value: normalizedCorrection.value.length, icon: 'fa-solid fa-list-check',     color: '#6366f1', bg: '#eef2ff' },
  { label: 'POINTS OBTENUS', value: displayScore.value,                icon: 'fa-solid fa-trophy',         color: '#f59e0b', bg: '#fffbeb' },
]);

const metricRows = computed(() => {
  const total = Math.max(normalizedCorrection.value.length, 1);
  return [
    {
      label: 'Score obtenu',
      value: result.value.pourcentage + '%',
      pct:   result.value.pourcentage,
      icon:  'fa-solid fa-star',
      color: '#f59e0b',
      fill:  'linear-gradient(90deg,#f59e0b,#fbbf24)',
    },
    {
      label: 'Intégrité Anti-Cheat',
      value: integrityScore.value + '%',
      pct:   integrityScore.value,
      icon:  'fa-solid fa-shield-halved',
      color: '#6366f1',
      fill:  'linear-gradient(90deg,#6366f1,#818cf8)',
    },
    {
      label: 'Réponses correctes',
      value: `${correctCount.value} / ${normalizedCorrection.value.length}`,
      pct:   (correctCount.value / total) * 100,
      icon:  'fa-solid fa-circle-check',
      color: '#10b981',
      fill:  'linear-gradient(90deg,#10b981,#34d399)',
    },
  ];
});

const themeBreakdown = computed(() => {
  const map = {};
  normalizedCorrection.value.forEach(q => {
    const t = q.theme || 'Général';
    if (!map[t]) map[t] = { name: t, total: 0, correct: 0 };
    map[t].total++;
    if (q.isCorrect) map[t].correct++;
  });
  return Object.values(map).map(t => ({
    ...t,
    pct: Math.round((t.correct / t.total) * 100),
  }));
});

const coachMessage = computed(() => {
  const p = result.value.pourcentage;
  if (p >= 90) return "Excellente performance ! Maîtrise experte démontrée sur l'ensemble des modules. Votre profil est hautement qualifié.";
  if (p >= 70) return "Bonne performance — session validée avec succès. Quelques axes d'amélioration identifiés sur les questions avancées.";
  if (p >= 50) return "Performance intermédiaire. Des lacunes détectées sur certains modules. Une révision ciblée est recommandée.";
  return "Performance insuffisante. Une préparation approfondie sur les fondamentaux est nécessaire avant de retenter.";
});

const resultTags = computed(() => {
  const tags = [];
  if (integrityScore.value === 100)   tags.push('✦ Intégrité parfaite');
  if (result.value.pourcentage >= 90) tags.push('✦ Expert certifié');
  else if (isPassed.value)            tags.push('✦ Standard validé');
  if (skippedCount.value === 0)       tags.push('✦ 0 question ignorée');
  if (correctCount.value === normalizedCorrection.value.length && correctCount.value > 0)
    tags.push('✦ Score parfait');
  return tags;
});

const corrFilters = computed(() => [
  { val: 'all',       label: 'Toutes',      count: normalizedCorrection.value.length },
  { val: 'correct',   label: 'Correctes',   count: correctCount.value },
  { val: 'incorrect', label: 'Incorrectes', count: incorrectCount.value },
  { val: 'skipped',   label: 'Ignorées',    count: skippedCount.value },
]);

const filteredCorrection = computed(() => {
  const list = normalizedCorrection.value;
  if (corrFilter.value === 'correct')   return list.filter(q => q.isCorrect);
  if (corrFilter.value === 'incorrect') return list.filter(q => !q.isCorrect && q.userAnswer);
  if (corrFilter.value === 'skipped')   return list.filter(q => !q.userAnswer);
  return list;
});

/* ─── HELPERS ────────────────────────────────────────────────── */
const toggleCorrection = () => {
  showCorrection.value = !showCorrection.value;
  if (showCorrection.value)
    setTimeout(() => document.querySelector('.correction-section')?.scrollIntoView({ behavior: 'smooth' }), 120);
};

const generatePDF = () => {
  showToast('Préparation du rapport PDF…', 'success', 'fa-solid fa-file-pdf');
  setTimeout(() => window.print(), 600);
};

let _toastTimerFn = null;
const showToast = (message, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimerFn);
  Object.assign(toast, { message, type, icon, active: true });
  _toastTimerFn = setTimeout(() => { toast.active = false; }, 4500);
};

onMounted(fetchResults);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');

*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

/* ── ROOT ── */
.results-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* ── BACKGROUND ── */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.12; border-radius: 50%;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }

/* ── LAYOUT ── */
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }
.dashboard-view { max-width: 1440px; margin: 0 auto; }

/* ── LOADER ── */
.full-loader {
  display: flex; flex-direction: column; align-items: center; justify-content: center;
  min-height: calc(100vh - 64px); gap: 16px;
}
.loader-label {
  font-size: 0.72rem; font-weight: 800; letter-spacing: 0.3em; color: #94a3b8;
}
.spinner-pro-premium {
  width: 50px; height: 50px;
  border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b;
  border-radius: 50%; animation: spin 1s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── HEADER ── */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
.meta-badge {
  background: white; border: 1px solid #eef2f6; border-radius: 10px;
  padding: 5px 14px; font-size: 0.7rem; font-weight: 800; color: #64748b;
}
.btn-refresh-pro {
  width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}
.btn-refresh-pro:hover { background: #f8fafc; border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg); }

/* ── KPI CARDS ── */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6; transition: 0.2s;
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center; font-size: 1.4rem; flex-shrink: 0;
}
.stat-details { margin-left: 16px; }
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }

/* ── BENTO LAYOUT ── */
.bento-layout {
  display: grid;
  grid-template-columns: 340px 1fr 300px;
  gap: 20px;
}

/* ── ENIGMA CARD (même style que Campagnes) ── */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; }

.card-label {
  font-size: 0.6rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 0.2em; text-transform: uppercase;
}

/* ── SCORE CARD ── */
.score-card { display: flex; flex-direction: column; }
.score-ring-outer { position: relative; width: 200px; height: 200px; }
.ring-track    { fill: none; stroke: #f1f5f9; stroke-width: 12; }
.ring-progress { fill: none; stroke-width: 12; stroke-linecap: round; transform: rotate(-90deg); transform-origin: 100px 100px; }
.ring-center {
  position: absolute; inset: 0;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
}
.ring-pct  { font-size: 2.2rem; font-weight: 900; line-height: 1; }
.ring-pts  { font-size: 0.8rem; color: #94a3b8; font-weight: 700; margin-top: 4px; }
.text-success     { color: #10b981 !important; }
.text-danger-pro  { color: #f43f5e !important; }
.seuil-note       { font-size: 0.68rem; color: #94a3b8; font-weight: 700; }

/* REP ROW */
.rep-row { display: flex; gap: 10px; width: 100%; }
.rep-item {
  flex: 1; border-radius: 16px; padding: 14px 8px;
  display: flex; flex-direction: column; align-items: center; gap: 3px;
}
.rep-ok   { background: #ecfdf5; }
.rep-ko   { background: #fff1f2; }
.rep-skip { background: #fffbeb; }
.rep-ok   i { color: #10b981; font-size: 0.9rem; }
.rep-ko   i { color: #f43f5e; font-size: 0.9rem; }
.rep-skip i { color: #f59e0b; font-size: 0.9rem; }
.rep-val { font-size: 1.4rem; font-weight: 900; color: #0f172a; line-height: 1; }
.rep-lbl { font-size: 0.58rem; font-weight: 800; color: #94a3b8; letter-spacing: 0.5px; }

/* STATUS BADGES */
.status-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 0.65rem; font-weight: 800; text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-0    { background: #f0f9ff; color: #6366f1; }
.status-1    { background: #ecfdf5; color: #10b981; }
.status-2    { background: #fff1f2; color: #f43f5e; }
.status-skip { background: #fffbeb; color: #f59e0b; }
.status-dot  { width: 6px; height: 6px; border-radius: 50%; background: currentColor; margin-right: 6px; }

/* PIN BADGE */
.pin-badge {
  font-size: 0.6rem; font-weight: 800; color: #f59e0b;
  background: #fffbeb; padding: 3px 10px; border-radius: 8px;
  border: 1px solid rgba(245,158,11,0.2); display: inline-block;
}

/* SLOT BADGE */
.slot-badge {
  background: #f1f5f9; color: #64748b;
  font-size: 0.6rem; font-weight: 900; padding: 3px 8px; border-radius: 6px;
}

/* ── METRICS CARD ── */
.metrics-card { display: flex; flex-direction: column; }
.ai-badge {
  display: flex; align-items: center; gap: 7px;
  font-size: 0.6rem; font-weight: 800; color: #f59e0b;
  background: rgba(245,158,11,0.1); padding: 5px 12px; border-radius: 8px;
}
.ai-dot {
  width: 6px; height: 6px; background: #f59e0b; border-radius: 50%;
  animation: pulse 2s infinite;
}
@keyframes pulse { 0%,100% { opacity:1; } 50% { opacity:0.3; } }
.metrics-list { display: flex; flex-direction: column; gap: 16px; }
.metric-icon-label {
  display: flex; align-items: center; gap: 8px;
  font-size: 0.82rem; font-weight: 700; color: #64748b;
}
.metric-val { font-size: 0.82rem; font-weight: 800; color: #0f172a; }
.metric-track { height: 7px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.metric-fill  { height: 100%; border-radius: 10px; transition: width 1.2s cubic-bezier(0.4,0,0.2,1); }

/* ANTICHEAT BOX */
.anticheat-box {
  display: flex; align-items: center; gap: 12px;
  border-radius: 14px; padding: 12px 16px; font-size: 0.78rem; margin-top: 4px;
}
.anticheat-box div { display: flex; flex-direction: column; gap: 2px; }
.anticheat-box strong { font-size: 0.75rem; font-weight: 900; display: block; }
.anticheat-box span  { font-size: 0.68rem; font-weight: 600; }
.ac-ok   { background: #ecfdf5; color: #10b981; border: 1px solid #6ee7b7; }
.ac-warn { background: #fff1f2; color: #f43f5e; border: 1px solid #fca5a5; }

/* VOR CORRECTION BUTTON */
.btn-voir-correction {
  padding: 13px; border-radius: 14px;
  background: transparent; border: 2px solid #0f172a; color: #0f172a;
  font-weight: 900; cursor: pointer; font-family: inherit; font-size: 0.8rem;
  display: flex; align-items: center; justify-content: center; transition: 0.2s;
}
.btn-voir-correction:hover { background: #0f172a; color: white; }

/* ── COACH CARD ── */
.coach-card {}
.ia-coach-terminal {
  background: #0f172a; border-radius: 20px; padding: 20px;
  display: flex; gap: 16px; align-items: flex-start;
}
.robot-glow-container {
  width: 44px; height: 44px; border-radius: 14px;
  background: rgba(245,158,11,0.2); display: flex; align-items: center;
  justify-content: center; font-size: 1.1rem; flex-shrink: 0;
}
.coach-text-v8 h6  { color: white; font-weight: 800; margin-bottom: 4px; font-size: 0.85rem; }
.coach-text-v8 p   { color: #94a3b8; font-size: 0.75rem; }

/* ── THEMES ── */
.theme-list { display: flex; flex-direction: column; gap: 16px; }
.theme-name { color: #0f172a; }

/* ── CORRECTION ── */
.correction-section { margin-top: 0; }
.corr-filter-btn {
  display: flex; align-items: center; gap: 7px;
  background: white; border: 1.5px solid #e2e8f0; color: #64748b;
  border-radius: 10px; cursor: pointer; font-family: inherit;
  transition: 0.2s; width: auto;
}
.corr-filter-btn:hover { border-color: #f59e0b; color: #f59e0b; }
.corr-filter-btn.active { background: #0f172a; color: white; border-color: #0f172a; }
.cf-dot { width: 7px; height: 7px; border-radius: 50%; flex-shrink: 0; }
.cfd-all       { background: #94a3b8; }
.cfd-correct   { background: #10b981; }
.cfd-incorrect { background: #f43f5e; }
.cfd-skipped   { background: #f59e0b; }

/* CORR CARD BORDER COLORS */
.cc-ok   { border-color: #6ee7b7 !important; }
.cc-ko   { border-color: #fca5a5 !important; }
.cc-skip { border-color: #fde68a !important; }

.cc-top {
  background: #f8fafc; border-bottom: 1px solid #eef2f6;
}

/* OPTIONS */
.cc-options { display: flex; flex-direction: column; gap: 9px; }
.cc-opt {
  display: flex; align-items: center; gap: 12px;
  padding: 11px 16px; border-radius: 13px;
  border: 1.5px solid #eef2f6; background: #f8fafc;
}
.cc-opt.cco-correct { border-color: #6ee7b7; background: #f0fdf4; }
.cc-opt.cco-user    { border-color: #fca5a5; background: #fff1f2; }
.cc-opt.cco-user-ok { border-color: #6ee7b7; background: #ecfdf5; }
.cco-letter {
  width: 28px; height: 28px; min-width: 28px; border-radius: 8px;
  background: white; border: 1.5px solid #eef2f6;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 0.7rem; color: #94a3b8; flex-shrink: 0;
}
.cco-correct .cco-letter { background: #10b981; border-color: #10b981; color: white; }
.cco-user    .cco-letter { background: #f43f5e; border-color: #f43f5e; color: white; }
.cco-text { font-weight: 700; font-size: 0.85rem; color: #0f172a; }

/* EXPLICATION */
.cc-explication {
  background: #fffbeb; border: 1px solid #fde68a; border-radius: 14px;
  padding: 14px 18px; font-size: 0.83rem; color: #78350f; font-weight: 600;
  display: flex; align-items: flex-start; gap: 8px; line-height: 1.6;
}
.text-amber { color: #f59e0b !important; }

/* EMPTY STATE */
.empty-state-pro {
  background: white; border-radius: 30px; padding: 40px;
  border: 1px dashed #e2e8f0;
}

/* ── BUTTONS ── */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer; font-family: inherit;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2; display: flex; align-items: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white; color: #64748b;
  cursor: pointer; transition: 0.2s; font-size: 0.75rem;
  display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }

/* ── TOAST ── */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px;
  display: flex; align-items: center; gap: 15px; z-index: 3000;
  border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.enigma-toast .t-body strong { font-size: 0.58rem; letter-spacing: 1px; opacity: 0.5; display: block; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* ── TRANSITIONS ── */
.slide-down-enter-active { animation: slideDown 0.4s ease-out; }
.slide-down-leave-active { animation: slideDown 0.3s ease-in reverse; }
@keyframes slideDown { from { opacity:0; transform:translateY(-16px); } to { opacity:1; transform:translateY(0); } }

/* ── SCROLLBAR ── */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

/* ── RESPONSIVE ── */
@media (max-width: 1200px) {
  .bento-layout { grid-template-columns: 1fr 1fr; }
  .coach-card   { grid-column: span 2; }
}
@media (max-width: 768px) {
  .premium-title { font-size: 1.6rem; }
  .bento-layout  { grid-template-columns: 1fr; }
  .coach-card    { grid-column: unset; }
}

/* ── PRINT ── */
@media print {
  .cyber-engine-bg, .btn-enigma-primary, .btn-refresh-pro, .btn-voir-correction { display: none !important; }
  .enigma-card { box-shadow: none !important; border: 1px solid #ddd !important; }
  .canvas-engine { height: auto !important; overflow: visible !important; }
}

/* ══════════════════════════════════════════════════
   DARK MODE OVERRIDES (identique à Campagnes.vue)
══════════════════════════════════════════════════ */
[data-theme="dark"] .results-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

[data-theme="dark"] .stat-card-premium { background: rgba(22,27,34,0.7); border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }

[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .rep-val { color: #f0f6fc; }
[data-theme="dark"] .theme-name { color: #e6edf3; }

[data-theme="dark"] .metric-icon-label { color: #8b949e; }
[data-theme="dark"] .metric-val { color: #f0f6fc; }
[data-theme="dark"] .metric-track { background: rgba(255,255,255,0.06); }

[data-theme="dark"] .cc-top { background: rgba(255,255,255,0.03); border-bottom-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .cc-opt { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .cco-text { color: #e6edf3; }
[data-theme="dark"] .cc-question { color: #e6edf3; }
[data-theme="dark"] .enigma-card h5 { color: #e6edf3; }

[data-theme="dark"] .meta-badge { background: rgba(255,255,255,0.07); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .slot-badge { background: rgba(255,255,255,0.07); color: #8b949e; }

[data-theme="dark"] .corr-filter-btn { background: #161b22; border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .corr-filter-btn.active { background: #0d1117; color: #f0f6fc; border-color: #f0f6fc; }

[data-theme="dark"] .btn-voir-correction { border-color: rgba(255,255,255,0.2); color: #e6edf3; }
[data-theme="dark"] .btn-voir-correction:hover { background: #e6edf3; color: #0d1117; }

[data-theme="dark"] .btn-refresh-pro { background: #161b22; border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .btn-refresh-pro:hover { border-color: #f59e0b; color: #f59e0b; }

[data-theme="dark"] .custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(255,255,255,0.08); }
[data-theme="dark"] .empty-state-pro { background: #161b22; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .empty-state-pro p { color: #8b949e; }

[data-theme="dark"] .quantum-grid { background-image: radial-gradient(rgba(255,255,255,0.04) 1px, transparent 1px); }
</style>