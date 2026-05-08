<template>
  <div class="elite-details-root d-flex overflow-hidden">

    <!-- ── FOND ── -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <!-- ══ LOADER ══ -->
      <div v-if="isLoading" class="loader-portal">
        <div class="robot-ring"></div>
        <p class="loading-text mt-3">DÉCRYPTAGE DU PROFIL CANDIDAT...</p>
      </div>

      <!-- ══ ERREUR ══ -->
      <div v-else-if="errorMsg" class="loader-portal">
        <div class="error-icon-box mb-4">
          <i class="fa-solid fa-triangle-exclamation fa-2x" style="color:#f43f5e;"></i>
        </div>
        <p class="loading-text" style="color:#f43f5e;">{{ errorMsg }}</p>
        <button @click="$router.back()" class="btn-back-elite mt-4">
          <i class="fa-solid fa-arrow-left-long me-2"></i> RETOUR
        </button>
      </div>

      <!-- ══ CONTENU ══ -->
      <main v-else class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="p-4 p-lg-5 animate__animated animate__fadeIn">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root" @click="$router.push('/dashboard')" style="cursor:pointer">Accueil</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="root" @click="$router.push('/analyse-comportementale')" style="cursor:pointer">
                  Analyses IA
                </span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ candidat.fullName }}</span>
              </div>
              <h2 class="premium-title">
                Rapport <span class="gradient-text">Candidat</span>
              </h2>
              <p class="text-muted-sm mt-1">
                <span class="session-id-badge me-2">
                  ID : {{ String(candidateId).substring(0,13).toUpperCase() }}
                </span>
                <span class="fw-800" style="font-size:0.75rem;text-transform:uppercase;">
                  {{ candidat.campaignName }}
                </span>
              </p>
            </div>
            <div class="d-flex gap-3">
              <button class="btn-refresh-pro" @click="$router.back()" title="Retour">
                <i class="fa-solid fa-arrow-left-long"></i>
              </button>
              <button class="btn-enigma-primary" @click="printPage">
                <div class="btn-content">
                  <i class="fa-solid fa-file-pdf me-2"></i>GÉNÉRER LE RAPPORT
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- KPI -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value" :style="{ color: stat.color }">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <div class="row g-4 mb-4">

            <!-- SCORE RADIAL + IDENTITÉ -->
            <div class="col-xl-4">
              <div class="enigma-card p-5 h-100 d-flex flex-column align-items-center text-center">

                <!-- Avatar -->
                <div class="avatar-squircle mb-3">
                  {{ initials(candidat.fullName) }}
                </div>
                <h3 class="fw-900 mb-0" style="font-size:1.3rem;">{{ candidat.fullName }}</h3>
                <p class="text-muted-sm small mb-4">{{ candidat.email }}</p>

                <!-- Ring -->
                <div class="score-ring-container mb-3">
                  <svg viewBox="0 0 140 140" width="180" height="180">
                    <circle cx="70" cy="70" r="56" class="ring-bg"/>
                    <circle
                      cx="70" cy="70" r="56"
                      class="ring-fill"
                      :stroke="isPassed ? '#10b981' : '#f43f5e'"
                      :style="ringStyle"
                    />
                    <text x="70" y="65" text-anchor="middle" class="ring-pct-text">
                      {{ globalScore }}%
                    </text>
                    <text x="70" y="82" text-anchor="middle" class="ring-sub-text">
                      {{ scorePoints }} pts
                    </text>
                  </svg>
                </div>

                <!-- Status pill -->
                <div class="result-status-pill mb-2" :class="isPassed ? 'pill-pass' : 'pill-fail'">
                  <i :class="isPassed ? 'fa-solid fa-medal me-2' : 'fa-solid fa-rotate me-2'"></i>
                  {{ isPassed ? 'CANDIDAT CERTIFIABLE' : 'BESOIN DE FORMATION' }}
                </div>
                <span class="text-muted-sm" style="font-size:0.72rem;font-weight:700;">
                  Seuil requis : {{ examMeta.scoreReussite }}%
                </span>

                <!-- Répartition -->
                <div class="repartition-row mt-4 w-100">
                  <div class="rep-item rep-correct">
                    <div class="rep-icon"><i class="fa-solid fa-check"></i></div>
                    <span class="rep-val">{{ correctCount }}</span>
                    <span class="rep-lbl">Correctes</span>
                  </div>
                  <div class="rep-item rep-incorrect">
                    <div class="rep-icon"><i class="fa-solid fa-xmark"></i></div>
                    <span class="rep-val">{{ incorrectCount }}</span>
                    <span class="rep-lbl">Incorrectes</span>
                  </div>
                  <div class="rep-item rep-skipped">
                    <div class="rep-icon"><i class="fa-solid fa-minus"></i></div>
                    <span class="rep-val">{{ skippedCount }}</span>
                    <span class="rep-lbl">Ignorées</span>
                  </div>
                </div>

                <!-- Infos candidat -->
                <div class="info-list w-100 mt-4 text-start">
                  <div class="info-item">
                    <div class="i-icon"><i class="fa-solid fa-stopwatch"></i></div>
                    <div class="i-data">
                      <label>Temps de réalisation</label>
                      <span>{{ candidat.timeTaken || 'N/A' }}</span>
                    </div>
                  </div>
                  <div class="info-item">
                    <div class="i-icon" :class="integrityScore >= 80 ? 'safe' : 'warn'">
                      <i class="fa-solid fa-shield-halved"></i>
                    </div>
                    <div class="i-data">
                      <label>Intégrité Anti-Cheat</label>
                      <span class="fw-800">
                        {{ integrityScore }}% —
                        {{ integrityScore >= 80 ? 'Fiable' : 'À surveiller' }}
                      </span>
                    </div>
                  </div>
                  <div class="info-item">
                    <div class="i-icon"><i class="fa-solid fa-calendar-check"></i></div>
                    <div class="i-data">
                      <label>Date de passation</label>
                      <span>{{ formatDate(candidat.passedAt) }}</span>
                    </div>
                  </div>
                </div>

                <div class="global-tag mt-4 w-100">
                  GLOBAL SCORE : {{ globalScore }}% — TIER {{ tierLabel }}
                </div>
              </div>
            </div>

            <!-- MÉTRIQUES + THÈMES -->
            <div class="col-xl-8">
              <div class="enigma-card p-5 h-100">
                <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
                  <h6 class="fw-900 m-0 text-muted-sm" style="font-size:0.65rem;letter-spacing:2px;">
                    MÉTRIQUES DE SESSION
                  </h6>
                  <span class="live-tag">
                    <span class="dot-pulse me-2"></span>SYSTÈME D'ÉVALUATION IA
                  </span>
                </div>

                <div v-for="m in metricsData" :key="m.label" class="metric-item mb-4">
                  <div class="d-flex justify-content-between mb-2">
                    <span class="small fw-700">
                      <i :class="m.icon + ' me-2'" :style="{ color: m.color }"></i>{{ m.label }}
                    </span>
                    <strong class="small">{{ m.display }}</strong>
                  </div>
                  <div class="metric-bar">
                    <div class="mbar-fill" :style="{ width: m.pct + '%', background: m.color }"></div>
                  </div>
                </div>

                <!-- Anti-cheat -->
                <div class="anticheat-result-box mt-3"
                  :class="integrityScore >= 70 ? 'ac-result-ok' : 'ac-result-warn'">
                  <i class="fa-solid fa-shield-halved fa-lg me-3"></i>
                  <div>
                    <strong class="d-block" style="font-size:0.82rem;">Anti-Cheat v2.0</strong>
                    <span style="font-size:0.72rem;">
                      {{ infractions }} infraction(s) — Intégrité : {{ integrityScore }}%
                    </span>
                  </div>
                </div>

                <!-- Analyse par thème -->
                <div v-if="themeBreakdown.length > 0" class="mt-4">
                  <h6 class="fw-900 mb-3 text-muted-sm" style="font-size:0.62rem;letter-spacing:2px;">
                    ANALYSE PAR THÈME
                  </h6>
                  <div v-for="th in themeBreakdown" :key="th.name" class="mb-3">
                    <div class="d-flex justify-content-between mb-1">
                      <span class="small fw-800">{{ th.name }}</span>
                      <span class="small fw-800" :class="th.pct >= 70 ? 'text-success' : 'text-danger'">
                        {{ th.correct }}/{{ th.total }} · {{ th.pct }}%
                      </span>
                    </div>
                    <div class="progress-slim">
                      <div class="progress-fill"
                        :style="{ width: th.pct + '%', background: th.pct >= 70 ? '#10b981' : '#f43f5e' }">
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- IA COACH BENTO -->
          <div class="ai-master-bento mb-4">
            <div v-if="isAiLoading" class="p-5 text-center">
              <div class="robot-ring mx-auto mb-3"></div>
              <p class="fw-800 text-white" style="font-size:0.7rem;letter-spacing:2px;">
                L'IA ANALYSE LES RÉPONSES DU CANDIDAT...
              </p>
            </div>
            <div v-else class="row g-0">
              <div class="col-lg-7 p-5 ai-main-column">
                <div class="d-flex align-items-center gap-3 mb-4">
                  <div class="ai-avatar-glow"><i class="fa-solid fa-robot"></i></div>
                  <h4 class="m-0 text-white fw-800">Synthèse Consultant IA</h4>
                </div>
                <div class="ai-bubble mb-4">
                  <p class="ai-text-block m-0">"{{ aiInsights.synthese }}"</p>
                </div>
                <div class="row g-3">
                  <div class="col-md-6">
                    <div class="insight-box force-box p-4">
                      <div class="insight-label text-success mb-2">
                        <i class="fa-solid fa-circle-check me-2"></i>POINTS FORTS
                      </div>
                      <ul class="point-list m-0 ps-0">
                        <li v-for="f in aiInsights.forces" :key="f">{{ f }}</li>
                      </ul>
                    </div>
                  </div>
                  <div class="col-md-6">
                    <div class="insight-box warn-box p-4">
                      <div class="insight-label mb-2" style="color:#f59e0b;">
                        <i class="fa-solid fa-lightbulb me-2"></i>AXES DE PROGRESSION
                      </div>
                      <ul class="point-list m-0 ps-0">
                        <li v-for="a in aiInsights.axes" :key="a">{{ a }}</li>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-lg-5 p-5 roadmap-column">
                <h6 class="fw-900 mb-4 text-white"
                  style="font-size:0.62rem;letter-spacing:2px;opacity:0.5;">
                  ROADMAP DE PROGRESSION
                </h6>
                <div class="roadmap-step mb-5">
                  <div class="step-icon-hex me-4">
                    <i class="fa-solid fa-crosshairs"></i>
                  </div>
                  <div>
                    <label class="d-block text-muted mb-1"
                      style="font-size:0.6rem;font-weight:900;letter-spacing:1px;">
                      OBJECTIF IMMÉDIAT
                    </label>
                    <p class="text-white fw-700 m-0" style="font-size:0.95rem;">
                      {{ aiInsights.roadmap.objectif }}
                    </p>
                  </div>
                </div>
                <div class="roadmap-step">
                  <div class="step-icon-hex gold me-4">
                    <i class="fa-solid fa-award"></i>
                  </div>
                  <div>
                    <label class="d-block text-muted mb-1"
                      style="font-size:0.6rem;font-weight:900;letter-spacing:1px;">
                      DÉCISION RH CONSEILLÉE
                    </label>
                    <p class="text-white fw-700 m-0" style="font-size:0.95rem;">
                      {{ aiInsights.roadmap.certification }}
                    </p>
                  </div>
                </div>
                <div class="result-tags mt-5">
                  <span v-for="tag in resultTags" :key="tag" class="result-tag">{{ tag }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- CORRECTION DÉTAILLÉE -->
          <div class="enigma-card p-5">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
              <h6 class="fw-900 m-0 text-muted-sm" style="font-size:0.65rem;letter-spacing:2px;">
                CORRECTION DÉTAILLÉE
              </h6>
              <div class="d-flex gap-2 flex-wrap">
                <button v-for="f in filterDefs" :key="f.val"
                  class="rf-btn" :class="{ 'rf-active': reviewFilter === f.val }"
                  @click="reviewFilter = f.val">
                  <span class="rf-dot" :class="'rfd-' + f.val"></span>
                  {{ f.label }} ({{ f.count }})
                </button>
              </div>
            </div>

            <!-- Pas de données -->
            <div v-if="detailedCorrection.length === 0" class="text-center py-5">
              <i class="fa-solid fa-inbox fa-3x mb-3" style="color:#e2e8f0;"></i>
              <p class="text-muted-sm fw-700">Aucune correction disponible pour ce candidat.</p>
            </div>

            <div v-else class="d-flex flex-column gap-3">
              <div v-for="(item, idx) in filteredCorrection" :key="idx"
                class="correction-card"
                :class="item.isCorrect ? 'cc-correct' : (item.userAnswer ? 'cc-incorrect' : 'cc-skipped')">

                <div class="cc-header d-flex justify-content-between align-items-center px-4 py-3">
                  <div class="d-flex align-items-center gap-2 flex-wrap">
                    <span class="cc-num">QUESTION {{ item.originalIndex + 1 }}</span>
                    <span v-if="item.theme" class="cc-theme">{{ item.theme }}</span>
                    <span v-if="item.points" class="cc-pts">{{ item.points }} pts</span>
                  </div>
                  <span class="cc-status"
                    :class="item.isCorrect ? 'cc-s-correct' : (item.userAnswer ? 'cc-s-incorrect' : 'cc-s-skipped')">
                    <i :class="item.isCorrect
                      ? 'fa-solid fa-check me-1'
                      : (item.userAnswer ? 'fa-solid fa-xmark me-1' : 'fa-solid fa-minus me-1')">
                    </i>
                    {{ item.isCorrect ? 'CORRECT' : (item.userAnswer ? 'INCORRECT' : 'IGNORÉ') }}
                  </span>
                </div>

                <div class="px-4 py-3">
                  <h5 class="fw-800 mb-3" style="font-size:1rem;">
                    {{ item.enonce || item.question || item.questionText }}
                  </h5>

                  <!-- OPTIONS QCU/QCM -->
                  <div v-if="item.options && item.options.length > 0" class="d-flex flex-column gap-2 mb-3">
                    <div v-for="(opt, oi) in item.options" :key="oi"
                      class="cc-opt d-flex align-items-center gap-3 p-3"
                      :class="{
                        'cco-correct':      item.correctIndexes?.includes(oi),
                        'cco-user':         item.userIndexes?.includes(oi) && !item.correctIndexes?.includes(oi),
                        'cco-user-correct': item.userIndexes?.includes(oi) && item.correctIndexes?.includes(oi)
                      }">
                      <div class="cco-letter">{{ String.fromCharCode(65 + oi) }}</div>
                      <div class="flex-grow-1 fw-700" style="font-size:0.88rem;">{{ opt }}</div>
                      <i v-if="item.correctIndexes?.includes(oi)" class="fa-solid fa-check"
                        style="color:#10b981;"></i>
                      <i v-else-if="item.userIndexes?.includes(oi)" class="fa-solid fa-xmark"
                        style="color:#f43f5e;"></i>
                    </div>
                  </div>

                  <!-- TEXTE LIBRE -->
                  <div v-else class="row g-3 mb-3">
                    <div class="col-md-6">
                      <div class="cctc-block p-3" :class="item.isCorrect ? 'cctc-ok' : 'cctc-user'">
                        <label class="d-block mb-2"
                          style="font-size:0.55rem;font-weight:900;letter-spacing:1.5px;">
                          RÉPONSE DU CANDIDAT
                        </label>
                        <p class="m-0 fw-700" style="font-size:0.85rem;">
                          {{ item.userAnswer || 'AUCUNE RÉPONSE' }}
                        </p>
                      </div>
                    </div>
                    <div v-if="!item.isCorrect" class="col-md-6">
                      <div class="cctc-block cctc-ok p-3">
                        <label class="d-block mb-2"
                          style="font-size:0.55rem;font-weight:900;letter-spacing:1.5px;">
                          RÉPONSE CORRECTE
                        </label>
                        <p class="m-0 fw-700" style="font-size:0.85rem;">
                          {{ item.correctAnswer || item.bonneReponse }}
                        </p>
                      </div>
                    </div>
                  </div>

                  <!-- EXPLICATION -->
                  <div v-if="item.explication || item.explanation" class="cc-explication p-3">
                    <div class="d-flex align-items-center mb-2"
                      style="font-size:0.62rem;font-weight:900;letter-spacing:1px;">
                      <i class="fa-solid fa-lightbulb me-2" style="color:#f59e0b;"></i>EXPLICATION
                    </div>
                    <p class="m-0" style="font-size:0.85rem;font-weight:600;line-height:1.6;">
                      {{ item.explication || item.explanation }}
                    </p>
                  </div>
                </div>
              </div>

              <div v-if="filteredCorrection.length === 0" class="text-center py-5 text-muted-sm">
                <i class="fa-solid fa-check-double fa-2x mb-3 text-success"></i>
                <p class="fw-700 small">Aucune question dans cette catégorie.</p>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar  from '../components/AppNavbar.vue';

// ─── ROUTE ────────────────────────────────────────────────────────
const route  = useRoute();
const router = useRouter();

// ─── STATE ────────────────────────────────────────────────────────
const isLoading   = ref(true);
const isAiLoading = ref(true);
const errorMsg    = ref('');
const reviewFilter = ref('all');

// Données candidat
const candidat = ref({
  fullName:     '',
  email:        '',
  campaignName: '',
  timeTaken:    null,
  passedAt:     null,
});

// Données examen
const globalScore        = ref(0);
const scorePoints        = ref(0);
const infractions        = ref(0);
const detailedCorrection = ref([]);
const examMeta           = ref({ scoreReussite: 70 });

// IA
const aiInsights = ref({
  synthese: '',
  forces:   [],
  axes:     [],
  roadmap:  { objectif: '', certification: '' },
});

// ─── COMPUTED ──────────────────────────────────────────────────────
const candidateId = computed(() => route.params.id || '');

const isPassed = computed(() =>
  globalScore.value >= (examMeta.value?.scoreReussite || 70)
);

const tierLabel = computed(() => {
  const s = globalScore.value;
  return s >= 85 ? 'ÉLITE' : s >= 70 ? 'STANDARD' : 'BASIQUE';
});

const integrityScore = computed(() =>
  Math.max(0, 100 - (infractions.value || 0) * 10)
);

const ringStyle = computed(() => {
  const circ = 2 * Math.PI * 56;
  return {
    strokeDasharray:  `${(globalScore.value / 100) * circ} ${circ}`,
    strokeDashoffset: circ * 0.25,
    transition:       'stroke-dasharray 1.5s ease-out',
  };
});

const correctCount   = computed(() => detailedCorrection.value.filter(q => q.isCorrect).length);
const incorrectCount = computed(() => detailedCorrection.value.filter(q => !q.isCorrect && q.userAnswer).length);
const skippedCount   = computed(() => detailedCorrection.value.filter(q => !q.userAnswer).length);

const kpiStats = computed(() => [
  {
    label: 'Score global',
    value: globalScore.value + '%',
    icon:  'fa-solid fa-star',
    color: isPassed.value ? '#10b981' : '#f43f5e',
    bg:    isPassed.value ? 'rgba(16,185,129,0.1)' : 'rgba(244,63,94,0.1)',
  },
  {
    label: 'Intégrité',
    value: integrityScore.value + '%',
    icon:  'fa-solid fa-shield-halved',
    color: integrityScore.value >= 70 ? '#10b981' : '#f43f5e',
    bg:    integrityScore.value >= 70 ? 'rgba(16,185,129,0.1)' : 'rgba(244,63,94,0.1)',
  },
  {
    label: 'Questions',
    value: detailedCorrection.value.length,
    icon:  'fa-solid fa-list-check',
    color: '#6366f1',
    bg:    'rgba(99,102,241,0.1)',
  },
  {
    label: 'Points obtenus',
    value: scorePoints.value,
    icon:  'fa-solid fa-trophy',
    color: '#f59e0b',
    bg:    'rgba(245,158,11,0.1)',
  },
]);

const metricsData = computed(() => [
  {
    label:   'Score obtenu',
    display: globalScore.value + '%',
    pct:     globalScore.value,
    icon:    'fa-solid fa-star',
    color:   '#f59e0b',
  },
  {
    label:   'Intégrité Anti-Cheat',
    display: integrityScore.value + '%',
    pct:     integrityScore.value,
    icon:    'fa-solid fa-shield-halved',
    color:   '#6366f1',
  },
  {
    label:   'Réponses correctes',
    display: `${correctCount.value} / ${detailedCorrection.value.length}`,
    pct:     detailedCorrection.value.length
      ? Math.round((correctCount.value / detailedCorrection.value.length) * 100)
      : 0,
    icon:    'fa-solid fa-check-circle',
    color:   '#10b981',
  },
]);

const themeBreakdown = computed(() => {
  const map = {};
  detailedCorrection.value.forEach(q => {
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

const resultTags = computed(() => {
  const tags = [];
  if (integrityScore.value === 100) tags.push('Intégrité parfaite');
  if (globalScore.value >= 90)      tags.push('Expert certifié');
  else if (globalScore.value >= 70) tags.push('Standard validé');
  if (correctCount.value === detailedCorrection.value.length && correctCount.value > 0)
    tags.push('Score parfait');
  if (skippedCount.value === 0 && detailedCorrection.value.length > 0)
    tags.push('Aucune question ignorée');
  return tags;
});

const filterDefs = computed(() => [
  { val: 'all',       label: 'Toutes',      count: detailedCorrection.value.length },
  { val: 'correct',   label: 'Correctes',   count: correctCount.value },
  { val: 'incorrect', label: 'Incorrectes', count: incorrectCount.value },
  { val: 'skipped',   label: 'Ignorées',    count: skippedCount.value },
]);

const filteredCorrection = computed(() => {
  const list = detailedCorrection.value.map((item, i) => ({ ...item, originalIndex: i }));
  if (reviewFilter.value === 'correct')   return list.filter(q => q.isCorrect);
  if (reviewFilter.value === 'incorrect') return list.filter(q => !q.isCorrect && q.userAnswer);
  if (reviewFilter.value === 'skipped')   return list.filter(q => !q.userAnswer);
  return list;
});

// ─── FETCH ────────────────────────────────────────────────────────
// Stratégie :
//   1. Récupérer les données du candidat via /Candidates/:id
//   2. Trouver le dernier résultat d'examen via plusieurs endpoints
//   3. Parser la correction détaillée exactement comme ResultsView.vue
// ─────────────────────────────────────────────────────────────────
onMounted(async () => {
  const id = route.params.id;
  if (!id) { errorMsg.value = 'ID candidat manquant.'; isLoading.value = false; return; }

  try {
    // ── 1. Candidat ────────────────────────────────────────────────
    let c = null;
    try {
      const { data } = await api.get(`/Candidates/${id}`);
      c = data;
    } catch {
      try {
        const { data: list } = await api.get('/Candidates');
        c = list.find(x => String(x.id || x.Id) === String(id));
      } catch (_) {}
    }

    if (c) {
      candidat.value = {
        fullName:     c.name     || c.fullName     || c.nom  || 'Candidat',
        email:        c.email    || c.Email         || '—',
        campaignName: c.group    || c.campaignName  || c.poste || 'N/A',
        timeTaken:    c.timeTaken|| c.duree          || null,
        passedAt:     c.passedAt || c.completedAt   || c.createdAt || null,
      };
    }

    // ── 2. Résultats — même endpoints que ResultsView.vue ──────────
    // On tente les mêmes routes que ResultsView pour rester cohérent
    let raw = null;
    const endpointsToTry = [
      `/Examen/results/${id}`,
      `/Examen/resultats/${id}`,
      `/Examen/historique/${id}`,
      `/Examen/candidate-report/${id}`,
      `/Results/${id}`,
    ];

    for (const ep of endpointsToTry) {
      try {
        const { data } = await api.get(ep);
        if (data) { raw = data; break; }
      } catch (_) {}
    }

    // ── 3. Fallback : dernier résultat via l'historique global ─────
    if (!raw) {
      try {
        const { data: hist } = await api.get('/Examen/historique');
        const candidatResults = (Array.isArray(hist) ? hist : [])
          .filter(r =>
            String(r.candidatId || r.CandidatId || r.userId || '') === String(id)
          )
          .sort((a, b) =>
            new Date(b.createdAt || b.date || 0) - new Date(a.createdAt || a.date || 0)
          );
        if (candidatResults.length > 0) raw = candidatResults[0];
      } catch (_) {}
    }

    // ── 4. Parser raw → même logique que ResultsView.vue ──────────
    if (raw) {
      globalScore.value  = Math.round(raw.pourcentage || raw.scorePourcentage || raw.scoreGlobal || 0);
      scorePoints.value  = raw.scoreTotal || raw.scorePoints || 0;
      infractions.value  = raw.infractions || 0;

      if (raw.campaignName || raw.nomCampagne) {
        candidat.value.campaignName = raw.campaignName || raw.nomCampagne;
      }
      if (raw.timeTaken || raw.duree) {
        candidat.value.timeTaken = raw.timeTaken || raw.duree;
      }

      examMeta.value = {
        scoreReussite: raw.scoreReussite || 70,
        titre:         raw.titre || '',
      };

      const questions = raw.questions || [];
      const rawCorrection = raw.detailedCorrection || raw.corrections || raw.details || [];

      detailedCorrection.value = rawCorrection.map((item, idx) => {
        const q        = questions[idx];
        const choix    = q?.choix || item.options || [];
        const userRaw  = item.userAnswer  || item.reponse        || '';
        const corrRaw  = item.correctAnswer || item.bonneReponse || '';

        let userIndexes = [];
        const parts = userRaw.split(';').map(s => s.trim()).filter(Boolean);
        if (parts.every(p => !isNaN(p)) && parts.length > 0)
          userIndexes = parts.map(Number).filter(n => n >= 0 && n < choix.length);

        let correctIndexes = [];
        if (choix.length > 0 && corrRaw) {
          const corrTexts = corrRaw.split('|').map(s => s.trim().toLowerCase());
          correctIndexes = choix
            .map((c2, i) => corrTexts.includes(c2.trim().toLowerCase()) ? i : -1)
            .filter(i => i !== -1);
        }

        return {
          ...item,
          enonce:         item.enonce    || item.question     || item.questionText || 'N/A',
          userAnswer:     userRaw,
          correctAnswer:  corrRaw,
          isCorrect:      Boolean(item.isCorrect ?? item.estCorrect ?? item.correct ?? false),
          options:        choix,
          userIndexes,
          correctIndexes,
          theme:          item.theme       || q?.theme       || 'Général',
          points:         item.points      || q?.points      || 1,
          explication:    item.explication || q?.explication || item.explanation || '',
        };
      });
    }

  } catch (err) {
    console.error('CandidateDetails fetch error:', err);
    errorMsg.value = 'Impossible de charger les données de ce candidat.';
  } finally {
    isLoading.value = false;
    generateAiInsights();
  }
});

// ─── GÉNÉRATION IA LOCALE ─────────────────────────────────────────
const generateAiInsights = () => {
  const best  = [...themeBreakdown.value].sort((a, b) => b.pct - a.pct)[0];
  const worst = [...themeBreakdown.value].sort((a, b) => a.pct - b.pct)[0];
  const pct   = globalScore.value;

  aiInsights.value = {
    synthese: pct >= 70
      ? `Avec un score de ${pct}%, ${candidat.value.fullName} démontre une maîtrise solide${best ? ` en ${best.name}` : ''}. Profil certifiable pour ce niveau.`
      : `Avec ${pct}%, des axes d'amélioration sont identifiés${worst ? ` notamment en ${worst.name}` : ''}. Une formation ciblée est recommandée avant intégration.`,
    forces: [
      best ? `Expertise en ${best.name} (${best.pct}%)` : 'Rigueur de raisonnement',
      correctCount.value > 0 ? `${correctCount.value} réponse(s) correcte(s)` : 'Engagement complet',
    ],
    axes: [
      worst && worst.pct < 70 ? `Approfondir ${worst.name} (${worst.pct}%)` : 'Optimiser les temps de réponse',
      skippedCount.value > 0  ? `Répondre aux ${skippedCount.value} questions ignorées` : 'Maintenir la précision',
    ],
    roadmap: {
      objectif:      worst ? `Renforcer ${worst.name} avec des exercices pratiques` : 'Consolider les acquis actuels',
      certification: pct >= 85
        ? 'Intégration immédiate recommandée — Profil Senior'
        : pct >= 70
          ? 'Intégration avec accompagnement — Profil Confirmé'
          : 'Plan de formation 30j avant reconsidération',
    },
  };
  isAiLoading.value = false;
};

// ─── HELPERS ──────────────────────────────────────────────────────
const initials   = (name) => (name || '?').split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'long', year: 'numeric' }) : '—';
const printPage  = () => window.print();
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800;900&display=swap');

/* ── ROOT ── */
.elite-details-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* ── BACKGROUND ── */
.luxury-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.15; }
.sphere-amber { width: 600px; height: 600px; background: #f59e0b; top: -200px; right: -100px; }
.sphere-blue  { width: 500px; height: 500px; background: #6366f1; bottom: -100px; left: -100px; }

/* ── LAYOUT ── */
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ── LOADER / ERROR ── */
.loader-portal {
  min-height: 100vh; display: flex; flex-direction: column;
  align-items: center; justify-content: center; gap: 16px;
}
.robot-ring {
  width: 56px; height: 56px;
  border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b;
  border-radius: 50%; animation: spin 1s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }
.loading-text { font-weight: 800; color: #94a3b8; font-size: 0.7rem; letter-spacing: 0.4em; }
.error-icon-box {
  width: 80px; height: 80px; border-radius: 24px;
  background: #fff1f2; display: flex; align-items: center; justify-content: center;
}

/* ── HEADER ── */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
.session-id-badge {
  background: #f1f5f9; color: #64748b;
  padding: 4px 12px; border-radius: 8px; font-size: 0.7rem; font-weight: 700;
}
.text-muted-sm { color: #94a3b8; }

/* ── BUTTONS ── */
.btn-back-elite {
  background: white; border: 1.5px solid #e2e8f0; padding: 11px 22px;
  border-radius: 18px; font-weight: 800; font-size: 11px; color: #475569;
  display: inline-flex; align-items: center; cursor: pointer; transition: 0.3s;
}
.btn-back-elite:hover { background: #0f172a; color: white; }
.btn-refresh-pro {
  width: 44px; height: 44px; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover { border-color: #f59e0b; color: #f59e0b; }
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
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }

/* ── KPI ── */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6;
  transition: 0.2s; box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0; margin-right: 16px;
}
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }

/* ── CARD ── */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; }

/* ── AVATAR ── */
.avatar-squircle {
  width: 100px; height: 100px; border-radius: 32px;
  background: linear-gradient(135deg, #0f172a, #1e293b);
  display: flex; align-items: center; justify-content: center;
  font-size: 34px; font-weight: 900; color: #fbbf24;
  box-shadow: 0 12px 30px rgba(0,0,0,0.12);
}

/* ── SCORE RING ── */
.ring-bg   { fill: none; stroke: #eef2f6; stroke-width: 10; }
.ring-fill { fill: none; stroke-width: 10; stroke-linecap: round; }
.ring-pct-text { font-size: 22px; font-weight: 900; fill: #0f172a; }
.ring-sub-text { font-size: 9px; fill: #94a3b8; font-weight: 700; }

.result-status-pill {
  padding: 10px 28px; border-radius: 14px;
  font-size: 0.72rem; font-weight: 900; display: inline-flex; align-items: center;
}
.pill-pass { background: rgba(16,185,129,0.12); color: #10b981; }
.pill-fail { background: rgba(244,63,94,0.12);  color: #f43f5e; }

/* ── RÉPARTITION ── */
.repartition-row  { display: flex; gap: 12px; }
.rep-item         { flex: 1; border-radius: 16px; padding: 14px; text-align: center; display: flex; flex-direction: column; align-items: center; gap: 4px; }
.rep-correct      { background: #ecfdf5; }
.rep-incorrect    { background: #fff1f2; }
.rep-skipped      { background: #f8fafc; border: 1px solid #eef2f6; }
.rep-correct .rep-icon  { color: #10b981; font-size: 1rem; }
.rep-incorrect .rep-icon { color: #f43f5e; font-size: 1rem; }
.rep-skipped .rep-icon   { color: #94a3b8; font-size: 1rem; }
.rep-val { font-size: 1.4rem; font-weight: 900; color: #0f172a; line-height: 1; }
.rep-lbl { font-size: 0.58rem; font-weight: 900; color: #94a3b8; letter-spacing: 0.5px; }

/* ── INFO LIST ── */
.info-list { }
.info-item { display: flex; gap: 14px; align-items: flex-start; margin-bottom: 18px; }
.i-icon {
  width: 40px; height: 40px; flex-shrink: 0; border-radius: 12px;
  background: #f8fafc; display: flex; align-items: center; justify-content: center;
  color: #94a3b8; font-size: 13px;
}
.i-icon.safe { color: #10b981; background: #f0fdf4; }
.i-icon.warn { color: #f43f5e; background: #fff1f2; }
.i-data label { display: block; font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-bottom: 3px; }
.i-data span  { font-size: 13px; font-weight: 700; color: #1e293b; }

.global-tag {
  background: #0f172a; color: white; padding: 14px;
  border-radius: 18px; font-weight: 900; font-size: 13px; text-align: center;
}

/* ── METRICS ── */
.metric-bar  { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.mbar-fill   { height: 100%; border-radius: 10px; transition: width 1.2s ease; }
.live-tag    { display: inline-flex; align-items: center; font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; }
.dot-pulse   { width: 6px; height: 6px; background: #f59e0b; border-radius: 50%; display: inline-block; animation: pulse 2s infinite; }
@keyframes pulse { 0%,100%{opacity:1;}50%{opacity:0.3;} }

.anticheat-result-box {
  display: flex; align-items: center; gap: 16px;
  border-radius: 14px; padding: 16px 18px; font-size: 0.8rem;
}
.ac-result-ok   { background: #ecfdf5; color: #10b981; border: 1px solid #6ee7b7; }
.ac-result-warn { background: #fff1f2; color: #f43f5e; border: 1px solid #fca5a5; }

.progress-slim  { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill  { height: 100%; border-radius: 10px; transition: width 0.8s ease; }

/* ── IA BENTO ── */
.ai-master-bento  { background: #0f172a; border-radius: 32px; overflow: hidden; }
.ai-main-column   { border-right: 1px solid rgba(255,255,255,0.06); }
.ai-avatar-glow {
  width: 55px; height: 55px; background: rgba(245,158,11,0.15); color: #f59e0b;
  border-radius: 18px; display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.ai-bubble  { background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 18px; padding: 20px; }
.ai-text-block { font-size: 1rem; color: #f1f5f9; line-height: 1.6; font-style: italic; }
.insight-box  { border-radius: 18px; }
.force-box    { background: rgba(16,185,129,0.08);  border: 1px solid rgba(16,185,129,0.2); }
.warn-box     { background: rgba(245,158,11,0.08);  border: 1px solid rgba(245,158,11,0.2); }
.insight-label { font-size: 0.6rem; font-weight: 900; letter-spacing: 1.5px; }
.point-list   { list-style: none; color: #cbd5e1; font-size: 0.88rem; }
.point-list li::before { content: "›"; color: #f59e0b; font-weight: 800; margin-right: 8px; }
.roadmap-column { background: rgba(255,255,255,0.02); }
.roadmap-step   { display: flex; align-items: flex-start; }
.step-icon-hex  {
  width: 46px; height: 46px; flex-shrink: 0;
  background: rgba(245,158,11,0.1); border: 1.5px solid rgba(245,158,11,0.4);
  border-radius: 14px; display: flex; align-items: center; justify-content: center; color: #f59e0b;
}
.step-icon-hex.gold { background: #f59e0b; color: #0f172a; border-color: #f59e0b; }
.result-tags { display: flex; flex-wrap: wrap; gap: 8px; }
.result-tag  { background: rgba(245,158,11,0.15); color: #f59e0b; font-size: 0.65rem; font-weight: 900; padding: 5px 12px; border-radius: 8px; }

/* ── CORRECTION ── */
.correction-card  { background: white; border-radius: 22px; border: 2px solid #eef2f6; overflow: hidden; }
.cc-correct   { border-color: #6ee7b7; }
.cc-incorrect { border-color: #fca5a5; }
.cc-skipped   { border-color: #fde68a; }
.cc-header    { background: #f8fafc; border-bottom: 1px solid #eef2f6; }
.cc-num   { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; }
.cc-theme { background: #fffbeb; color: #f59e0b; font-size: 0.6rem; font-weight: 900; padding: 3px 8px; border-radius: 6px; }
.cc-pts   { background: #f1f5f9; color: #64748b; font-size: 0.6rem; font-weight: 900; padding: 3px 8px; border-radius: 6px; }
.cc-status { font-size: 0.62rem; font-weight: 900; padding: 5px 12px; border-radius: 8px; display: flex; align-items: center; }
.cc-s-correct   { background: #ecfdf5; color: #10b981; }
.cc-s-incorrect { background: #fff1f2; color: #f43f5e; }
.cc-s-skipped   { background: #fffbeb; color: #f59e0b; }

.cc-opt     { border-radius: 14px; border: 1.5px solid #eef2f6; background: #f8fafc; }
.cco-correct      { border-color: #6ee7b7; background: #f0fdf4; }
.cco-user         { border-color: #fca5a5; background: #fff1f2; }
.cco-user-correct { border-color: #6ee7b7; background: #ecfdf5; }
.cco-letter {
  width: 32px; height: 32px; min-width: 32px; border-radius: 10px;
  background: white; border: 1.5px solid #eef2f6;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 0.72rem; color: #94a3b8;
}
.cco-correct .cco-letter { background: #10b981; border-color: #10b981; color: white; }
.cco-user    .cco-letter { background: #f43f5e; border-color: #f43f5e; color: white; }

.cctc-block { border-radius: 14px; border: 1.5px solid #eef2f6; }
.cctc-ok    { background: #f0fdf4; border-color: #6ee7b7; }
.cctc-user  { background: #fff1f2; border-color: #fca5a5; }
.cc-explication { background: #fffbeb; border: 1px solid #fde68a; border-radius: 14px; }
.cc-explication p { color: #78350f; }

/* ── FILTRES ── */
.rf-btn {
  display: flex; align-items: center; gap: 8px; padding: 7px 16px;
  border-radius: 10px; border: 1.5px solid #eef2f6; background: white;
  font-size: 0.72rem; font-weight: 800; cursor: pointer; font-family: inherit;
  transition: 0.2s; color: #64748b;
}
.rf-btn:hover   { border-color: #0f172a; color: #0f172a; }
.rf-btn.rf-active { background: #0f172a; color: white; border-color: #0f172a; }
.rf-dot        { width: 7px; height: 7px; border-radius: 50%; display: inline-block; }
.rfd-all       { background: #94a3b8; }
.rfd-correct   { background: #10b981; }
.rfd-incorrect { background: #f43f5e; }
.rfd-skipped   { background: #f59e0b; }

/* ── UTILS ── */
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.text-success { color: #10b981 !important; }
.text-danger  { color: #f43f5e !important; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

@media (max-width: 768px) {
  .premium-title { font-size: 1.6rem; }
  .repartition-row { flex-direction: column; gap: 8px; }
  .ai-main-column { border-right: none; border-bottom: 1px solid rgba(255,255,255,0.06); }
}
</style>