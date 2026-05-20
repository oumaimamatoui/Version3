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
        <p class="loading-text mt-3">CHARGEMENT DU PROFIL CANDIDAT...</p>
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
      <main v-else class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar" ref="mainScrollRef">
        <div class="p-4 p-lg-5 animate__fadeIn">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root" @click="$router.push('/dashboard')" style="cursor:pointer">Accueil</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="root" @click="$router.push('/analyse-comportementale')" style="cursor:pointer">Analyses IA</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ candidat.fullName }}</span>
              </div>
              <h2 class="premium-title">
                Rapport <span class="gradient-text">Candidat</span>
              </h2>
              <p class="text-muted-sm mt-1">
                <span class="session-id-badge me-2">
                  ID : {{ String(candidateId).substring(0, 13).toUpperCase() }}
                </span>
                <span class="fw-800" style="font-size:0.75rem;text-transform:uppercase;">
                  {{ historiqueList.length }} session(s) enregistrée(s)
                </span>
              </p>
            </div>

            <!-- ✅ BOUTONS HEADER : Retour / Correction / Rapport PDF -->
            <div class="d-flex gap-3 flex-wrap">
              <button class="btn-refresh-pro" @click="$router.back()" title="Retour">
                <i class="fa-solid fa-arrow-left-long"></i>
              </button>

              <!-- ✅ Bouton Correction : révèle la section et scrolle vers elle -->
              <button
                class="btn-correction-scroll"
                :class="{ 'btn-correction-active': correctionVisible }"
                @click="toggleCorrection"
                title="Afficher / masquer la correction détaillée"
              >
                <div class="btn-content">
                  <i class="fa-solid fa-list-check me-2"></i>
                  {{ correctionVisible ? 'MASQUER LA CORRECTION' : 'VOIR LA CORRECTION' }}
                </div>
                <div class="btn-glow"></div>
              </button>

              <button class="btn-enigma-primary" @click="printPage">
                <div class="btn-content">
                  <i class="fa-solid fa-file-pdf me-2"></i>GÉNÉRER LE RAPPORT
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- ══ HISTORIQUE DES SESSIONS ══ -->
          <div v-if="historiqueList.length > 1" class="enigma-card p-4 mb-5">
            <h6 class="fw-900 mb-3 text-muted-sm" style="font-size:0.62rem;letter-spacing:2px;">
              TOUTES LES SESSIONS ({{ historiqueList.length }})
            </h6>
            <div class="d-flex gap-2 flex-wrap">
              <button
                v-for="(h, idx) in historiqueList"
                :key="h.id"
                class="session-btn"
                :class="{ 'session-btn-active': selectedEvalId === h.id }"
                @click="loadSession(h.id)"
              >
                <span class="session-btn-num">Session {{ idx + 1 }}</span>
                <span
                  class="session-btn-score"
                  :class="h.score >= (h.scoreReussite || 70) ? 'score-pass' : 'score-fail'"
                >
                  {{ h.score }}%
                </span>
                <span class="session-btn-date">{{ formatDateShort(h.date) }}</span>
              </button>
            </div>
          </div>

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

                <div class="avatar-squircle mb-3">
                  {{ initials(candidat.fullName) }}
                </div>
                <h3 class="fw-900 mb-0" style="font-size:1.3rem;">{{ candidat.fullName }}</h3>
                <p class="text-muted-sm small mb-4">{{ candidat.email }}</p>

                <!-- Ring SVG -->
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
                      <label>Session sélectionnée</label>
                      <span>{{ currentSessionTitle || 'N/A' }}</span>
                    </div>
                  </div>
                  <div class="info-item">
                    <div class="i-icon" :class="integrityScore >= 80 ? 'safe' : 'warn'">
                      <i class="fa-solid fa-shield-halved"></i>
                    </div>
                    <div class="i-data">
                      <label>Intégrité Anti-Cheat</label>
                      <span class="fw-800">
                        {{ integrityScore }}% — {{ integrityScore >= 80 ? 'Fiable' : 'À surveiller' }}
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
                <div
                  class="anticheat-result-box mt-3"
                  :class="integrityScore >= 70 ? 'ac-result-ok' : 'ac-result-warn'"
                >
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
                      <span
                        class="small fw-800"
                        :class="th.pct >= 70 ? 'text-success' : 'text-danger'"
                      >
                        {{ th.correct }}/{{ th.total }} · {{ th.pct }}%
                      </span>
                    </div>
                    <div class="progress-slim">
                      <div
                        class="progress-fill"
                        :style="{ width: th.pct + '%', background: th.pct >= 70 ? '#10b981' : '#f43f5e' }"
                      ></div>
                    </div>
                  </div>
                </div>

                <div v-else-if="!isSessionLoading" class="mt-4 text-center py-3">
                  <p class="text-muted-sm small fw-700">
                    <i class="fa-solid fa-chart-pie me-2"></i>Aucune donnée de thème disponible.
                  </p>
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
                <h6
                  class="fw-900 mb-4 text-white"
                  style="font-size:0.62rem;letter-spacing:2px;opacity:0.5;"
                >
                  ROADMAP DE PROGRESSION
                </h6>
                <div class="roadmap-step mb-5">
                  <div class="step-icon-hex me-4">
                    <i class="fa-solid fa-crosshairs"></i>
                  </div>
                  <div>
                    <label
                      class="d-block text-muted mb-1"
                      style="font-size:0.6rem;font-weight:900;letter-spacing:1px;"
                    >
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
                    <label
                      class="d-block text-muted mb-1"
                      style="font-size:0.6rem;font-weight:900;letter-spacing:1px;"
                    >
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

          <!-- ══════════════════════════════════════════
               ✅ SECTION CORRECTION — CACHÉE PAR DÉFAUT
               Affichée seulement après clic sur "VOIR LA CORRECTION"
          ══════════════════════════════════════════ -->
          <Transition name="correction-reveal">
            <div
              v-if="correctionVisible"
              class="enigma-card p-5"
              ref="correctionSectionRef"
            >
              <!-- En-tête correction -->
              <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
                <div class="d-flex align-items-center gap-3">
                  <div class="correction-title-icon">
                    <i class="fa-solid fa-list-check"></i>
                  </div>
                  <div>
                    <h6 class="fw-900 m-0 correction-section-title">CORRECTION DÉTAILLÉE</h6>
                    <p
                      v-if="currentSessionTitle"
                      class="m-0 mt-1"
                      style="font-size:0.72rem;font-weight:700;color:#94a3b8;"
                    >
                      {{ currentSessionTitle }}
                    </p>
                  </div>
                </div>
                <div class="d-flex gap-2 flex-wrap align-items-center">
                  <!-- Filtres -->
                  <button
                    v-for="f in filterDefs"
                    :key="f.val"
                    class="rf-btn"
                    :class="{ 'rf-active': reviewFilter === f.val }"
                    @click="reviewFilter = f.val"
                  >
                    <span class="rf-dot" :class="'rfd-' + f.val"></span>
                    {{ f.label }} ({{ f.count }})
                  </button>
                  <!-- Bouton fermer -->
                  <button class="btn-close-correction ms-2" @click="correctionVisible = false" title="Masquer la correction">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>
              </div>

              <!-- Loader correction -->
              <div v-if="isSessionLoading" class="text-center py-5">
                <div class="robot-ring mx-auto mb-3"></div>
                <p class="fw-800 text-muted" style="font-size:0.72rem;letter-spacing:1px;">
                  CHARGEMENT DE LA CORRECTION...
                </p>
              </div>

              <!-- Pas de données -->
              <div v-else-if="detailedCorrection.length === 0" class="text-center py-5">
                <i class="fa-solid fa-inbox fa-3x mb-3" style="color:#e2e8f0;"></i>
                <p class="text-muted-sm fw-700">Aucune correction disponible pour ce candidat.</p>
              </div>

              <!-- ✅ Liste des corrections -->
              <div v-else class="d-flex flex-column gap-3">
                <div
                  v-for="(item, idx) in filteredCorrection"
                  :key="idx"
                  class="correction-card"
                  :class="item.isCorrect
                    ? 'cc-correct'
                    : (item.userAnswer ? 'cc-incorrect' : 'cc-skipped')"
                >
                  <div class="cc-header d-flex justify-content-between align-items-center px-4 py-3">
                    <div class="d-flex align-items-center gap-2 flex-wrap">
                      <span class="cc-num">QUESTION {{ item.originalIndex + 1 }}</span>
                      <span v-if="item.theme" class="cc-theme">{{ item.theme }}</span>
                      <span v-if="item.points" class="cc-pts">{{ item.points }} pts</span>
                    </div>
                    <span
                      class="cc-status"
                      :class="item.isCorrect
                        ? 'cc-s-correct'
                        : (item.userAnswer ? 'cc-s-incorrect' : 'cc-s-skipped')"
                    >
                      <i
                        :class="item.isCorrect
                          ? 'fa-solid fa-check me-1'
                          : (item.userAnswer ? 'fa-solid fa-xmark me-1' : 'fa-solid fa-minus me-1')"
                      ></i>
                      {{ item.isCorrect ? 'CORRECT' : (item.userAnswer ? 'INCORRECT' : 'IGNORÉ') }}
                    </span>
                  </div>

                  <div class="px-4 py-3">
                    <h5 class="fw-800 mb-3" style="font-size:1rem;">
                      {{ item.enonce }}
                    </h5>

                    <!-- OPTIONS QCU / QCM -->
                    <div
                      v-if="item.options && item.options.length > 0"
                      class="d-flex flex-column gap-2 mb-3"
                    >
                      <div
                        v-for="(opt, oi) in item.options"
                        :key="oi"
                        class="cc-opt d-flex align-items-center gap-3 p-3"
                        :class="{
                          'cco-correct':      isOptionCorrect(item, oi),
                          'cco-user':         isOptionUser(item, oi) && !isOptionCorrect(item, oi),
                          'cco-user-correct': isOptionUser(item, oi) &&  isOptionCorrect(item, oi),
                        }"
                      >
                        <div class="cco-letter">{{ String.fromCharCode(65 + oi) }}</div>
                        <div class="flex-grow-1 fw-700" style="font-size:0.88rem;">{{ opt }}</div>
                        <i
                          v-if="isOptionCorrect(item, oi)"
                          class="fa-solid fa-check"
                          style="color:#10b981;"
                        ></i>
                        <i
                          v-else-if="isOptionUser(item, oi)"
                          class="fa-solid fa-xmark"
                          style="color:#f43f5e;"
                        ></i>
                      </div>
                    </div>

                    <!-- TEXTE LIBRE -->
                    <div v-else class="row g-3 mb-3">
                      <div class="col-md-6">
                        <div
                          class="cctc-block p-3"
                          :class="item.isCorrect ? 'cctc-ok' : 'cctc-user'"
                        >
                          <label
                            class="d-block mb-2"
                            style="font-size:0.55rem;font-weight:900;letter-spacing:1.5px;"
                          >
                            RÉPONSE DU CANDIDAT
                          </label>
                          <p class="m-0 fw-700" style="font-size:0.85rem;">
                            {{ item.userAnswer || 'AUCUNE RÉPONSE' }}
                          </p>
                        </div>
                      </div>
                      <div v-if="!item.isCorrect" class="col-md-6">
                        <div class="cctc-block cctc-ok p-3">
                          <label
                            class="d-block mb-2"
                            style="font-size:0.55rem;font-weight:900;letter-spacing:1.5px;"
                          >
                            RÉPONSE CORRECTE
                          </label>
                          <p class="m-0 fw-700" style="font-size:0.85rem;">
                            {{ item.correctAnswer }}
                          </p>
                        </div>
                      </div>
                    </div>

                    <!-- EXPLICATION -->
                    <div v-if="item.explication" class="cc-explication p-3">
                      <div
                        class="d-flex align-items-center mb-2"
                        style="font-size:0.62rem;font-weight:900;letter-spacing:1px;"
                      >
                        <i class="fa-solid fa-lightbulb me-2" style="color:#f59e0b;"></i>EXPLICATION
                      </div>
                      <p class="m-0" style="font-size:0.85rem;font-weight:600;line-height:1.6;">
                        {{ item.explication }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Filtre vide -->
                <div
                  v-if="filteredCorrection.length === 0"
                  class="text-center py-5 text-muted-sm"
                >
                  <i class="fa-solid fa-check-double fa-2x mb-3 text-success"></i>
                  <p class="fw-700 small">Aucune question dans cette catégorie.</p>
                </div>
              </div>
            </div>
          </Transition>

          <!-- ✅ Invite à afficher la correction (visible quand elle est masquée) -->
          <div v-if="!correctionVisible && !isLoading" class="correction-invite-card enigma-card p-4 text-center">
            <div class="d-flex align-items-center justify-content-center gap-3 flex-wrap">
              <div class="correction-invite-icon">
                <i class="fa-solid fa-lock"></i>
              </div>
              <div class="text-start">
                <p class="fw-800 m-0" style="font-size:0.9rem;">Correction détaillée disponible</p>
                <p class="text-muted-sm m-0" style="font-size:0.72rem;">
                  {{ detailedCorrection.length > 0 ? detailedCorrection.length + ' question(s) à revoir' : 'Chargement en cours…' }}
                </p>
              </div>
              <button class="btn-enigma-primary ms-auto" @click="toggleCorrection">
                <div class="btn-content">
                  <i class="fa-solid fa-list-check me-2"></i>AFFICHER LA CORRECTION
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </div>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, nextTick } from 'vue';
import { useRoute, useRouter }      from 'vue-router';
import api        from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar  from '../components/AppNavbar.vue';

// ── ROUTE ────────────────────────────────────────────────────────
const route  = useRoute();
const router = useRouter();

// ── REFS DOM ─────────────────────────────────────────────────────
const mainScrollRef        = ref(null);
const correctionSectionRef = ref(null);

// ── STATE ────────────────────────────────────────────────────────
const isLoading        = ref(true);
const isSessionLoading = ref(false);
const isAiLoading      = ref(true);
const errorMsg         = ref('');
const reviewFilter     = ref('all');

// ✅ Correction masquée par défaut
const correctionVisible = ref(false);

const historiqueList      = ref([]);
const selectedEvalId      = ref(null);
const currentSessionTitle = ref('');

const candidat = ref({
  fullName:     '',
  email:        '',
  campaignName: '',
  timeTaken:    null,
  passedAt:     null,
});

const globalScore        = ref(0);
const scorePoints        = ref(0);
const infractions        = ref(0);
const detailedCorrection = ref([]);
const examMeta           = ref({ scoreReussite: 70 });

const aiInsights = ref({
  synthese: '',
  forces:   [],
  axes:     [],
  roadmap:  { objectif: '', certification: '' },
});

// ── TOGGLE CORRECTION ─────────────────────────────────────────────
/**
 * ✅ Affiche ou masque la section correction.
 * Si on l'affiche, on scrolle automatiquement vers elle.
 */
const toggleCorrection = async () => {
  correctionVisible.value = !correctionVisible.value;
  if (correctionVisible.value) {
    await nextTick();
    scrollToCorrection();
  }
};

// ── COMPUTED ─────────────────────────────────────────────────────
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
const incorrectCount = computed(() => detailedCorrection.value.filter(q => !q.isCorrect && !!q.userAnswer).length);
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
    label: 'Sessions totales',
    value: historiqueList.value.length,
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
  if (reviewFilter.value === 'incorrect') return list.filter(q => !q.isCorrect && !!q.userAnswer);
  if (reviewFilter.value === 'skipped')   return list.filter(q => !q.userAnswer);
  return list;
});

// ── SCROLL VERS CORRECTION ────────────────────────────────────────
const scrollToCorrection = () => {
  if (!correctionSectionRef.value || !mainScrollRef.value) return;
  const top = correctionSectionRef.value.offsetTop - 24;
  mainScrollRef.value.scrollTo({ top, behavior: 'smooth' });
};

// ── OPTIONS HELPERS ───────────────────────────────────────────────
const isOptionCorrect = (item, optionIndex) => {
  if (!item.options?.length) return false;
  if (Array.isArray(item.correctIndexes)) {
    return item.correctIndexes.includes(optionIndex);
  }
  const raw = String(item.correctAnswer ?? '').trim();
  if (!raw) return false;
  const byIndex = raw.split(/[;|,]/).map(s => s.trim()).filter(Boolean);
  if (byIndex.every(p => /^\d+$/.test(p))) {
    return byIndex.map(Number).includes(optionIndex);
  }
  const letters = byIndex.map(s => s.toUpperCase());
  const letter  = String.fromCharCode(65 + optionIndex);
  if (letters.some(l => /^[A-Z]$/.test(l))) {
    return letters.includes(letter);
  }
  const optText = (item.options[optionIndex] ?? '').trim().toLowerCase();
  return byIndex.some(p => p.toLowerCase() === optText);
};

const isOptionUser = (item, optionIndex) => {
  if (!item.options?.length) return false;
  if (Array.isArray(item.userIndexes)) {
    return item.userIndexes.includes(optionIndex);
  }
  const raw = String(item.userAnswer ?? '').trim();
  if (!raw) return false;
  const byIndex = raw.split(/[;|,]/).map(s => s.trim()).filter(Boolean);
  if (byIndex.every(p => /^\d+$/.test(p))) {
    return byIndex.map(Number).includes(optionIndex);
  }
  const letters = byIndex.map(s => s.toUpperCase());
  const letter  = String.fromCharCode(65 + optionIndex);
  if (letters.some(l => /^[A-Z]$/.test(l))) {
    return letters.includes(letter);
  }
  const optText = (item.options[optionIndex] ?? '').trim().toLowerCase();
  return byIndex.some(p => p.toLowerCase() === optText);
};

// ── FETCH PRINCIPAL ───────────────────────────────────────────────
onMounted(async () => {
  const id = route.params.id;
  if (!id) {
    errorMsg.value  = 'ID candidat manquant.';
    isLoading.value = false;
    return;
  }
  try {
    await Promise.all([
      fetchCandidatInfo(id),
      fetchHistorique(id),
    ]);
    if (historiqueList.value.length > 0) {
      await loadSession(historiqueList.value[0].id);
    } else {
      isSessionLoading.value = false;
      isAiLoading.value      = false;
      globalScore.value      = 0;
      scorePoints.value      = 0;
      infractions.value      = 0;
      detailedCorrection.value = [];
      aiInsights.value = {
        synthese: "Aucune session d'examen enregistrée pour ce candidat pour le moment. Dès qu'un test est soumis, l'IA générera automatiquement le rapport complet.",
        forces: ["Aucun test soumis"],
        axes: ["En attente de passation"],
        roadmap: {
          objectif: "Inviter le candidat à finaliser son test",
          certification: "En attente de résultats"
        }
      };
    }
  } catch (err) {
    console.error('DetailsCandidat mount error:', err);
    errorMsg.value = 'Impossible de charger les données de ce candidat.';
  } finally {
    isLoading.value = false;
  }
});

// ── FETCH INFOS CANDIDAT ──────────────────────────────────────────
const fetchCandidatInfo = async (id) => {
  try {
    const { data } = await api.get(`/Candidates/${id}`);
    if (data) { applyCandidatData(data); return; }
  } catch (_) {}
  try {
    const { data: list } = await api.get('/Candidates');
    const found = (Array.isArray(list) ? list : []).find(
      x => String(x.id ?? x.Id) === String(id)
    );
    if (found) applyCandidatData(found);
  } catch (_) {}
};

const applyCandidatData = (d) => {
  candidat.value = {
    fullName:     d.name      ?? d.fullName ?? d.nom     ?? d.Nom      ?? 'Candidat',
    email:        d.email     ?? d.Email    ?? '—',
    campaignName: d.group     ?? d.campaignName ?? d.poste ?? 'N/A',
    timeTaken:    d.timeTaken ?? d.duree    ?? null,
    passedAt:     d.passedAt  ?? d.completedAt ?? d.createdAt ?? null,
  };
};

// ── FETCH HISTORIQUE ──────────────────────────────────────────────
const fetchHistorique = async (candidateId) => {
  try {
    const { data } = await api.get(`/Examen/historique-candidat/${candidateId}`);
    if (Array.isArray(data) && data.length > 0) {
      historiqueList.value = normalizeHistorique(data, candidateId);
      return;
    }
  } catch (_) {}
  try {
    const { data } = await api.get(`/Examen/resultats/${candidateId}`);
    const arr = Array.isArray(data) ? data : (data ? [data] : []);
    if (arr.length > 0) {
      historiqueList.value = normalizeHistorique(arr, candidateId);
      return;
    }
  } catch (_) {}
  try {
    const { data } = await api.get('/Examen/historique');
    const filtered = (Array.isArray(data) ? data : []).filter(r =>
      String(r.candidatId ?? r.CandidatId ?? r.userId ?? r.UserId ?? r.id ?? '') === String(candidateId)
    );
    if (filtered.length > 0) {
      historiqueList.value = normalizeHistorique(filtered, candidateId);
      return;
    }
  } catch (_) {}
  try {
    const { data } = await api.get(`/Examen/candidate-report/${candidateId}`);
    const arr = data?.history ?? data?.sessions ?? (Array.isArray(data) ? data : []);
    if (arr.length > 0) {
      historiqueList.value = normalizeHistorique(arr, candidateId);
    }
  } catch (_) {}
};

const normalizeHistorique = (arr, candidateId) =>
  arr
    .map(h => ({
      id:         h.id         ?? h.Id         ?? h.evalId    ?? h.EvalId    ?? h.resultId ?? h.ResultId,
      evalId:     h.evalId     ?? h.EvalId     ?? h.id        ?? h.Id,
      examenId:   h.examenId   ?? h.ExamenId   ?? h.testId    ?? h.TestId    ?? null,
      resultId:   h.resultId   ?? h.ResultId   ?? h.id        ?? h.Id,
      candidatId: candidateId,
      titreExamen:   h.titreExamen ?? h.TitreExamen ?? h.titre     ?? h.Titre    ?? h.nomExamen ?? 'Examen',
      date:          h.date        ?? h.Date        ?? h.createdAt ?? h.passedAt ?? null,
      score:         Math.round(Number(h.score ?? h.Score ?? h.pourcentage ?? h.Pourcentage ?? h.scoreGlobal ?? 0)),
      scoreReussite: Number(h.scoreReussite ?? h.ScoreReussite ?? 70),
      infractions:   Number(h.infractions   ?? h.Infractions   ?? 0),
      _raw: h,
    }))
    .sort((a, b) => new Date(b.date ?? 0) - new Date(a.date ?? 0));

// ── LOAD SESSION ──────────────────────────────────────────────────
const loadSession = async (sessionId) => {
  if (!sessionId) return;

  isSessionLoading.value   = true;
  isAiLoading.value        = true;
  selectedEvalId.value     = sessionId;
  reviewFilter.value       = 'all';
  detailedCorrection.value = [];
  // ✅ Masquer la correction lors du changement de session
  correctionVisible.value  = false;

  const sessionInfo = historiqueList.value.find(h => h.id === sessionId);

  if (sessionInfo) {
    currentSessionTitle.value = `${sessionInfo.titreExamen} — ${formatDate(sessionInfo.date)}`;
    candidat.value.passedAt   = sessionInfo.date;
    infractions.value         = sessionInfo.infractions ?? 0;
  }

  if (sessionInfo?._raw) {
    const embedded = extractEmbeddedCorrection(sessionInfo._raw);
    if (embedded) {
      parseSessionData(embedded, sessionInfo);
      isSessionLoading.value = false;
      generateAiInsights();
      return;
    }
  }

  const idsToTry = [
    ...new Set([
      sessionInfo?.evalId,
      sessionInfo?.resultId,
      sessionInfo?.id,
      sessionInfo?.examenId,
      sessionInfo?.candidatId,
    ].filter(Boolean).map(String))
  ];

  const cId = route.params.id;
  const attempts = [];
  for (const id of idsToTry) {
    attempts.push(
      `/Examen/results/${id}`,
      `/Examen/resultats/${id}`,
      `/Examen/correction/${id}`,
      `/Examen/result/${id}`,
      `/Examen/details/${id}`,
    );
  }
  if (sessionInfo?.examenId) {
    attempts.push(`/Examen/results/candidat/${cId}/examen/${sessionInfo.examenId}`);
    attempts.push(`/Examen/correction/candidat/${cId}/test/${sessionInfo.examenId}`);
  }

  let raw = null;
  for (const ep of attempts) {
    try {
      const { data } = await api.get(ep);
      if (data && hasCorrection(data)) { raw = data; break; }
    } catch (_) {}
  }

  if (raw) {
    parseSessionData(raw, sessionInfo);
  } else {
    globalScore.value        = sessionInfo?.score ?? 0;
    scorePoints.value        = 0;
    detailedCorrection.value = [];
    examMeta.value           = { scoreReussite: sessionInfo?.scoreReussite ?? 70 };
  }

  isSessionLoading.value = false;
  generateAiInsights();
};

const hasCorrection = (data) => {
  if (!data || typeof data !== 'object') return false;
  const correction =
    data.detailedCorrection ?? data.DetailedCorrection ??
    data.corrections        ?? data.Corrections        ??
    data.details            ?? data.Details            ??
    data.questions          ?? data.Questions          ??
    null;
  return Array.isArray(correction) && correction.length > 0;
};

const extractEmbeddedCorrection = (raw) => {
  if (!raw || typeof raw !== 'object') return null;
  if (hasCorrection(raw)) return raw;
  if (raw.result && hasCorrection(raw.result)) return raw.result;
  if (raw.data   && hasCorrection(raw.data))   return raw.data;
  return null;
};

const parseSessionData = (data, sessionInfo) => {
  globalScore.value = Math.round(
    Number(
      data.pourcentage      ?? data.Pourcentage      ??
      data.scorePourcentage ?? data.ScorePourcentage ??
      data.scoreGlobal      ?? data.ScoreGlobal      ??
      sessionInfo?.score    ?? 0
    )
  );
  scorePoints.value = Number(data.scoreTotal ?? data.ScoreTotal ?? data.points ?? 0);
  infractions.value = Number(data.infractions ?? data.Infractions ?? infractions.value ?? 0);
  examMeta.value    = {
    scoreReussite: Number(data.scoreReussite ?? data.ScoreReussite ?? sessionInfo?.scoreReussite ?? 70),
  };

  const rawItems =
    data.detailedCorrection ?? data.DetailedCorrection ??
    data.corrections        ?? data.Corrections        ??
    data.details            ?? data.Details            ??
    data.questions          ?? data.Questions          ??
    [];

  detailedCorrection.value = rawItems.map(item => {
    const options = normalizeOptions(item);
    const enonce  =
      item.enonce        ?? item.Enonce        ??
      item.question      ?? item.Question      ??
      item.questionText  ?? item.QuestionText  ??
      item.libelle       ?? item.Libelle       ?? 'N/A';

    const userAnswer =
      item.userAnswer    ?? item.UserAnswer    ??
      item.reponse       ?? item.Reponse       ??
      item.candidateAnswer ?? item.CandidateAnswer ?? '';

    const correctAnswer =
      item.correctAnswer ?? item.CorrectAnswer ??
      item.bonneReponse  ?? item.BonneReponse  ??
      item.expectedAnswer ?? item.ExpectedAnswer ?? '';

    const isCorrect = Boolean(
      item.isCorrect   ?? item.IsCorrect   ??
      item.estCorrect  ?? item.EstCorrect  ??
      item.correct     ?? item.Correct     ?? false
    );

    const explication =
      item.explication ?? item.Explication ??
      item.explanation ?? item.Explanation ?? '';

    const correctIndexes = resolveIndexes(correctAnswer, options);
    const userIndexes    = resolveIndexes(userAnswer, options);

    return {
      enonce, userAnswer, correctAnswer, isCorrect, options,
      correctIndexes, userIndexes,
      theme:      item.theme  ?? item.Theme  ?? item.categorie ?? 'Général',
      points:     Number(item.points ?? item.Points ?? 1),
      explication,
    };
  });
};

const normalizeOptions = (item) => {
  const raw = item.options ?? item.Options ?? item.choix ?? item.Choix ?? item.answers ?? item.Answers ?? [];
  if (Array.isArray(raw)) {
    return raw.map(o =>
      typeof o === 'string' ? o :
      (o.text ?? o.Text ?? o.label ?? o.Label ?? o.valeur ?? o.Valeur ?? String(o))
    );
  }
  if (raw && typeof raw === 'object') return Object.values(raw).map(String);
  return [];
};

const resolveIndexes = (rawAnswer, options) => {
  const str = String(rawAnswer ?? '').trim();
  if (!str || !options.length) return [];
  const parts = str.split(/[;|,]/).map(s => s.trim()).filter(Boolean);
  if (parts.every(p => /^\d+$/.test(p))) {
    return parts.map(Number).filter(n => n >= 0 && n < options.length);
  }
  if (parts.every(p => /^[A-Za-z]$/.test(p))) {
    return parts.map(l => l.toUpperCase().charCodeAt(0) - 65).filter(n => n >= 0 && n < options.length);
  }
  return parts.map(p => options.findIndex(o => o.trim().toLowerCase() === p.toLowerCase())).filter(n => n >= 0);
};

// ── IA LOCALE ─────────────────────────────────────────────────────
const generateAiInsights = () => {
  const best  = [...themeBreakdown.value].sort((a, b) => b.pct - a.pct)[0];
  const worst = [...themeBreakdown.value].sort((a, b) => a.pct - b.pct)[0];
  const pct   = globalScore.value;

  aiInsights.value = {
    synthese: pct >= 70
      ? `Avec un score de ${pct}%, ${candidat.value.fullName} démontre une maîtrise solide${best ? ` en ${best.name}` : ''}. Profil certifiable pour ce niveau.`
      : `Avec ${pct}%, des axes d'amélioration sont identifiés${worst ? ` notamment en ${worst.name}` : ''}. Une formation ciblée est recommandée avant intégration.`,
    forces: [
      best && best.pct >= 70
        ? `Expertise validée en ${best.name} (${best.pct}%)`
        : 'Rigueur dans l\'approche des exercices',
      correctCount.value > 0
        ? `${correctCount.value} réponse(s) correcte(s) sur ${detailedCorrection.value.length}`
        : 'Engagement complet dans l\'évaluation',
    ],
    axes: [
      worst && worst.pct < 70
        ? `Approfondir ${worst.name} (${worst.pct}%)`
        : 'Optimiser les temps de réponse sous pression',
      skippedCount.value > 0
        ? `Traiter les ${skippedCount.value} question(s) ignorée(s)`
        : 'Maintenir la précision en condition réelle',
    ],
    roadmap: {
      objectif: worst
        ? `Renforcer ${worst.name} avec des exercices pratiques ciblés`
        : 'Consolider et approfondir les acquis actuels',
      certification: pct >= 85
        ? 'Intégration immédiate recommandée — Profil Senior'
        : pct >= 70
          ? 'Intégration avec accompagnement — Profil Confirmé'
          : 'Plan de formation 30 jours avant reconsidération',
    },
  };

  isAiLoading.value = false;
};

// ── HELPERS ───────────────────────────────────────────────────────
const initials = (name) =>
  (name || '?').split(' ').map(n => n[0] ?? '').join('').toUpperCase().slice(0, 2);

const formatDate = (d) =>
  d
    ? new Date(d).toLocaleDateString('fr-FR', {
        day: '2-digit', month: 'long', year: 'numeric',
      })
    : '—';

const formatDateShort = (d) =>
  d
    ? new Date(d).toLocaleDateString('fr-FR', {
        day: '2-digit', month: '2-digit', year: '2-digit',
      })
    : '—';

const printPage = () => window.print();
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
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; display: flex; align-items: center; flex-wrap: wrap; gap: 4px; }
.breadcrumb-pro .root { cursor: pointer; transition: color 0.2s; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
.session-id-badge {
  background: #f1f5f9; color: #64748b;
  padding: 4px 12px; border-radius: 8px; font-size: 0.7rem; font-weight: 700;
}
.text-muted-sm { color: #94a3b8; }

/* ══════════════════════════════════════════════
   ✅ BOUTON CORRECTION — états normal / actif
══════════════════════════════════════════════ */
.btn-correction-scroll {
  background: white; color: #6366f1; border: 1.5px solid #c7d2fe;
  padding: 14px 22px; border-radius: 18px; font-weight: 800; font-size: 0.78rem;
  position: relative; overflow: hidden; cursor: pointer; font-family: inherit;
  transition: box-shadow 0.3s, border-color 0.3s, background 0.3s; letter-spacing: 0.5px;
}
.btn-correction-scroll .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #6366f1, #818cf8);
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-correction-scroll:hover .btn-glow { opacity: 1; }
.btn-correction-scroll .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-correction-scroll:hover .btn-content { color: white; }
.btn-correction-scroll:hover { border-color: #6366f1; box-shadow: 0 8px 24px rgba(99,102,241,0.25); }

/* État actif : correction visible */
.btn-correction-active {
  background: #6366f1 !important;
  color: white !important;
  border-color: #6366f1 !important;
  box-shadow: 0 8px 24px rgba(99,102,241,0.3) !important;
}
.btn-correction-active .btn-content { color: white !important; }

/* ✅ Bouton fermer dans l'en-tête de correction */
.btn-close-correction {
  width: 36px; height: 36px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  color: #64748b; cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.9rem;
}
.btn-close-correction:hover { background: #fff1f2; color: #f43f5e; border-color: #fca5a5; }

/* ✅ Invite correction (quand masquée) */
.correction-invite-card {
  border: 2px dashed #c7d2fe !important;
  background: linear-gradient(135deg, rgba(99,102,241,0.03), rgba(129,140,248,0.03)) !important;
}
.correction-invite-icon {
  width: 48px; height: 48px; border-radius: 14px;
  background: #eef2ff; color: #6366f1;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.1rem; flex-shrink: 0;
}

/* ══════════════════════════════════════════════
   ✅ ANIMATION D'APPARITION DE LA CORRECTION
══════════════════════════════════════════════ */
.correction-reveal-enter-active {
  animation: correctionIn 0.45s cubic-bezier(0.16, 1, 0.3, 1);
}
.correction-reveal-leave-active {
  animation: correctionOut 0.3s ease-in;
}
@keyframes correctionIn {
  from { opacity: 0; transform: translateY(20px) scale(0.98); }
  to   { opacity: 1; transform: translateY(0) scale(1); }
}
@keyframes correctionOut {
  from { opacity: 1; transform: translateY(0); }
  to   { opacity: 0; transform: translateY(10px); }
}

/* ── SESSION SWITCHER ── */
.session-btn {
  display: flex; flex-direction: column; align-items: flex-start; gap: 2px;
  padding: 10px 16px; border-radius: 14px; border: 1.5px solid #eef2f6;
  background: white; cursor: pointer; transition: all 0.2s; font-family: inherit; min-width: 110px;
}
.session-btn:hover:not(.session-btn-active) { border-color: #f59e0b; background: #fffbeb; }
.session-btn-active { border-color: #0f172a; background: #0f172a; }
.session-btn-active .session-btn-num,
.session-btn-active .session-btn-date { color: #64748b !important; }
.session-btn-num   { font-size: 0.6rem; font-weight: 900; letter-spacing: 1px; color: #94a3b8; }
.session-btn-score { font-size: 1rem; font-weight: 900; color: #0f172a; line-height: 1.2; }
.session-btn-active .session-btn-score { color: #fbbf24 !important; }
.session-btn-date  { font-size: 0.6rem; font-weight: 700; color: #94a3b8; }
.score-pass { color: #10b981 !important; }
.score-fail { color: #f43f5e !important; }

/* ── BUTTONS ── */
.btn-back-elite {
  background: white; border: 1.5px solid #e2e8f0; padding: 11px 22px;
  border-radius: 18px; font-weight: 800; font-size: 11px; color: #475569;
  display: inline-flex; align-items: center; cursor: pointer; transition: 0.3s;
}
.btn-back-elite:hover { background: #0f172a; color: white; border-color: #0f172a; }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center; font-size: 15px;
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
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }

/* ── KPI ── */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6;
  transition: transform 0.2s, box-shadow 0.2s;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
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
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; box-shadow: 0 2px 12px rgba(0,0,0,0.03); }

/* ── AVATAR ── */
.avatar-squircle {
  width: 100px; height: 100px; border-radius: 32px;
  background: linear-gradient(135deg, #0f172a, #1e293b);
  display: flex; align-items: center; justify-content: center;
  font-size: 34px; font-weight: 900; color: #fbbf24;
  box-shadow: 0 12px 30px rgba(0,0,0,0.12);
}

/* ── SCORE RING ── */
.ring-bg  { fill: none; stroke: #eef2f6; stroke-width: 10; }
.ring-fill { fill: none; stroke-width: 10; stroke-linecap: round; }
.ring-pct-text { font-size: 22px; font-weight: 900; fill: #0f172a; }
.ring-sub-text { font-size: 9px;  fill: #94a3b8;  font-weight: 700; }

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
.rep-correct  .rep-icon { color: #10b981; font-size: 1rem; }
.rep-incorrect .rep-icon { color: #f43f5e; font-size: 1rem; }
.rep-skipped  .rep-icon  { color: #94a3b8; font-size: 1rem; }
.rep-val { font-size: 1.4rem; font-weight: 900; color: #0f172a; line-height: 1; }
.rep-lbl { font-size: 0.58rem; font-weight: 900; color: #94a3b8; letter-spacing: 0.5px; }

/* ── INFO LIST ── */
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
.metric-bar { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.mbar-fill  { height: 100%; border-radius: 10px; transition: width 1.2s ease; }
.live-tag   { display: inline-flex; align-items: center; font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; }
.dot-pulse  { width: 6px; height: 6px; background: #f59e0b; border-radius: 50%; display: inline-block; animation: dotpulse 2s infinite; }
@keyframes dotpulse { 0%,100%{opacity:1;}50%{opacity:0.3;} }

.anticheat-result-box {
  display: flex; align-items: center; gap: 16px;
  border-radius: 14px; padding: 16px 18px; font-size: 0.8rem;
}
.ac-result-ok   { background: #ecfdf5; color: #10b981; border: 1px solid #6ee7b7; }
.ac-result-warn { background: #fff1f2; color: #f43f5e; border: 1px solid #fca5a5; }

.progress-slim { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: width 0.8s ease; }

/* ── IA BENTO ── */
.ai-master-bento { background: #0f172a; border-radius: 32px; overflow: hidden; }
.ai-main-column  { border-right: 1px solid rgba(255,255,255,0.06); }
.ai-avatar-glow  {
  width: 55px; height: 55px; background: rgba(245,158,11,0.15); color: #f59e0b;
  border-radius: 18px; display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.ai-bubble     { background: rgba(255,255,255,0.05); border: 1px solid rgba(255,255,255,0.1); border-radius: 18px; padding: 20px; }
.ai-text-block { font-size: 1rem; color: #f1f5f9; line-height: 1.6; font-style: italic; }
.insight-box   { border-radius: 18px; }
.force-box     { background: rgba(16,185,129,0.08); border: 1px solid rgba(16,185,129,0.2); }
.warn-box      { background: rgba(245,158,11,0.08); border: 1px solid rgba(245,158,11,0.2); }
.insight-label { font-size: 0.6rem; font-weight: 900; letter-spacing: 1.5px; }
.point-list    { list-style: none; color: #cbd5e1; font-size: 0.88rem; }
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

/* ── CORRECTION TITLE ── */
.correction-title-icon {
  width: 44px; height: 44px; border-radius: 14px;
  background: #eef2ff; color: #6366f1;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.1rem; flex-shrink: 0;
}
.correction-section-title { font-size: 0.75rem; letter-spacing: 2px; color: #0f172a; text-transform: uppercase; }

/* ── CORRECTION CARDS ── */
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

.cc-opt           { border-radius: 14px; border: 1.5px solid #eef2f6; background: #f8fafc; }
.cco-correct      { border-color: #6ee7b7; background: #f0fdf4; }
.cco-user         { border-color: #fca5a5; background: #fff1f2; }
.cco-user-correct { border-color: #6ee7b7; background: #ecfdf5; }
.cco-letter {
  width: 32px; height: 32px; min-width: 32px; border-radius: 10px;
  background: white; border: 1.5px solid #eef2f6;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 0.72rem; color: #94a3b8;
}
.cco-correct .cco-letter,
.cco-user-correct .cco-letter { background: #10b981; border-color: #10b981; color: white; }
.cco-user    .cco-letter      { background: #f43f5e; border-color: #f43f5e; color: white; }

.cctc-block { border-radius: 14px; border: 1.5px solid #eef2f6; }
.cctc-ok    { background: #f0fdf4; border-color: #6ee7b7; }
.cctc-user  { background: #fff1f2; border-color: #fca5a5; }
.cc-explication { background: #fffbeb; border: 1px solid #fde68a; border-radius: 14px; }
.cc-explication p { color: #78350f; }

/* ── FILTRES CORRECTION ── */
.rf-btn {
  display: flex; align-items: center; gap: 8px; padding: 7px 16px;
  border-radius: 10px; border: 1.5px solid #eef2f6; background: white;
  font-size: 0.72rem; font-weight: 800; cursor: pointer; font-family: inherit;
  transition: all 0.2s; color: #64748b;
}
.rf-btn:hover     { border-color: #0f172a; color: #0f172a; }
.rf-btn.rf-active { background: #0f172a; color: white; border-color: #0f172a; }
.rf-dot        { width: 7px; height: 7px; border-radius: 50%; display: inline-block; flex-shrink: 0; }
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

.custom-scrollbar::-webkit-scrollbar       { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

.animate__fadeIn {
  animation: fadeIn 0.45s ease both;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to   { opacity: 1; transform: none; }
}

@media (max-width: 768px) {
  .premium-title        { font-size: 1.6rem; }
  .repartition-row      { flex-direction: column; gap: 8px; }
  .ai-main-column       { border-right: none; border-bottom: 1px solid rgba(255,255,255,0.06); }
  .btn-correction-scroll { font-size: 0.7rem; padding: 11px 16px; }
  .btn-enigma-primary   { font-size: 0.78rem; padding: 11px 18px; }
}
</style>