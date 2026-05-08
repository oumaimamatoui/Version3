<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- ═══════════════════ HEADER ═══════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Plateforme</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Tableau de Bord</span>
              </div>
              <h2 class="premium-title">Mes <span class="gradient-text">Évaluations</span></h2>
              <p class="subtitle-pro mt-1">Propulsez votre carrière avec nos tests techniques.</p>
            </div>

            <!-- STATS PILL -->
            <div class="stats-bento-pill">
              <div class="stat-item text-center">
                <div class="stat-value">{{ activeTests.length }}</div>
                <div class="stat-label">DISPONIBLES</div>
              </div>
              <div class="stat-divider"></div>
              <div class="stat-item text-center">
                <div class="stat-value muted">{{ expiredTests.length }}</div>
                <div class="stat-label">ARCHIVÉS</div>
              </div>
              <div class="stat-divider"></div>
              <div class="stat-item text-center">
                <div class="stat-value indigo">{{ upcomingTests.length }}</div>
                <div class="stat-label">EN ATTENTE</div>
              </div>
            </div>
          </header>

          <!-- ═══════════════════ SEARCH & CONTROLS ═══════════════════ -->
          <div class="d-flex align-items-center gap-3 mb-5 flex-wrap">
            <div class="search-inline-box flex-grow-1" style="max-width:400px">
              <i class="fa-solid fa-magnifying-glass"></i>
              <input
                type="text"
                v-model="searchQuery"
                placeholder="Rechercher un test..."
                class="search-inline-input"
              >
              <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>
            <div class="view-toggle-cluster">
              <button
                :class="['btn-view-toggle', { active: viewMode === 'grid' }]"
                @click="viewMode = 'grid'"
                title="Vue grille"
              >
                <i class="fa-solid fa-table-cells-large"></i>
              </button>
              <button
                :class="['btn-view-toggle', { active: viewMode === 'list' }]"
                @click="viewMode = 'list'"
                title="Vue liste"
              >
                <i class="fa-solid fa-list-ul"></i>
              </button>
            </div>
            <button class="btn-refresh-pro" @click="fetchMyTests" :disabled="loading" title="Rafraîchir">
              <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
            </button>
          </div>

          <!-- ═══════════════════ LOADER ═══════════════════ -->
          <div v-if="loading" class="loader-portal-pro">
            <div class="spinner-pro-premium"></div>
            <span class="loading-text-pro">Analyse de vos sessions...</span>
          </div>

          <div v-else>

            <!-- ═══════════════════ SESSIONS ACTIVES ═══════════════════ -->
            <section class="mb-5">
              <div class="mb-4">
                <div class="section-tag-premium">
                  <span class="pulse-dot-amber"></span>
                  SESSIONS ACTIVES
                  <span class="count-pill">{{ filteredActive.length }}</span>
                </div>
              </div>

              <!-- EMPTY -->
              <div v-if="filteredActive.length === 0" class="empty-state-pro py-5 text-center">
                <i class="fa-solid fa-wind fa-3x text-muted mb-3 d-block"></i>
                <h5 class="fw-800">
                  {{ searchQuery ? 'Aucun test trouvé...' : 'C\'est le calme plat ici...' }}
                </h5>
                <p class="text-muted">
                  {{ searchQuery ? 'Modifiez votre recherche.' : 'Aucun test en attente pour le moment.' }}
                </p>
              </div>

              <!-- ── GRID VIEW ── -->
              <div v-else-if="viewMode === 'grid'" class="row g-4">
                <div
                  v-for="(c, index) in filteredActive"
                  :key="c.id"
                  class="col-md-6 col-xl-4 animate__animated animate__fadeInUp"
                  :style="{ animationDelay: `${index * 0.08}s` }"
                >
                  <div
                    class="campaign-card-modern"
                    :class="{
                      'is-upcoming': isUpcoming(c.dateDebut),
                      'card-clickable': !isUpcoming(c.dateDebut)
                    }"
                    @click="!isUpcoming(c.dateDebut) && openPreview(c)"
                  >
                    <!-- CARD TOP ROW -->
                    <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                      <span
                        class="status-badge"
                        :class="isUpcoming(c.dateDebut) ? 'status-waiting' : 'status-open'"
                      >
                        <span class="status-dot"></span>
                        {{ isUpcoming(c.dateDebut) ? 'EN ATTENTE' : 'OUVERT' }}
                      </span>
                      <!-- backend: dureeMinutes -->
                      <div class="duration-tag">
                        <i class="fa-regular fa-hourglass-half me-1"></i>
                        {{ c.dureeMinutes }} min
                      </div>
                    </div>

                    <!-- TITLE & DESCRIPTION  ← backend: nom, description -->
                    <h5 class="campaign-title-modern fw-800 mb-2">{{ c.nom }}</h5>
                    <p class="test-description-pro">
                      {{ c.description || 'Validation des compétences techniques avancées.' }}
                    </p>

                    <!-- DATE LIMITE BOX  ← backend: dateFin, nbCandidats -->
                    <div class="test-attachment-box mt-3 mb-3 p-3 rounded-4 d-flex align-items-center gap-3">
                      <div class="icon-file text-amber">
                        <i class="fa-solid fa-calendar-xmark fa-lg"></i>
                      </div>
                      <div class="flex-grow-1 overflow-hidden">
                        <span class="text-overline d-block">DATE LIMITE</span>
                        <p class="m-0 fw-bold small">{{ formatDate(c.dateFin) }}</p>
                      </div>
                      <div v-if="c.nbCandidats != null" class="questions-count-box">
                        <span class="q-count">{{ c.nbCandidats }}</span>
                        <span class="q-label">CND.</span>
                      </div>
                    </div>

                    <!-- PROGRESS BAR (time elapsed) -->
                    <div class="progress-slim mb-3">
                      <div
                        class="progress-fill"
                        :style="{
                          width: isUpcoming(c.dateDebut) ? '5%' : getTimeProgress(c) + '%',
                          background: isUpcoming(c.dateDebut) ? '#6366f1' : getProgressColor(getTimeProgress(c))
                        }"
                      ></div>
                    </div>

                    <!-- TIME ROW -->
                    <div class="d-flex justify-content-between align-items-center mb-3 small text-muted">
                      <span v-if="!isUpcoming(c.dateDebut)">
                        <i class="fa-solid fa-clock me-1"></i>
                        Ferme {{ getRelativeTime(c.dateFin) }}
                      </span>
                      <span v-else>
                        <i class="fa-solid fa-calendar-day me-1"></i>
                        Ouvre {{ formatDate(c.dateDebut) }}
                      </span>
                    </div>

                    <!-- ACTION BUTTON  ← routing uses: candidatureId || id -->
                    <button
                      @click.stop="startExam(c)"
                      class="btn-enigma-primary w-100 mt-1"
                      :disabled="isUpcoming(c.dateDebut)"
                    >
                      <div class="btn-content justify-content-center">
                        <template v-if="isUpcoming(c.dateDebut)">
                          <i class="fa-solid fa-lock me-2"></i>
                          OUVRE DANS {{ getCountdown(c.dateDebut) }}
                        </template>
                        <template v-else>
                          <i class="fa-solid fa-bolt me-2"></i>
                          COMMENCER LE TEST
                          <i class="fa-solid fa-arrow-right ms-2"></i>
                        </template>
                      </div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>
                </div>
              </div>

              <!-- ── LIST VIEW ── -->
              <div v-else class="list-view-pro animate__animated animate__fadeIn">
                <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
                  <span style="width:150px" class="list-col-label">STATUT</span>
                  <span class="flex-grow-1 list-col-label">NOM</span>
                  <span style="width:160px" class="list-col-label">DATE LIMITE</span>
                  <span style="width:90px"  class="list-col-label text-center">DURÉE</span>
                  <span style="width:80px"  class="list-col-label text-center">ACTION</span>
                </div>
                <div
                  v-for="c in filteredActive"
                  :key="c.id"
                  class="list-row-item d-flex align-items-center px-4 py-3 mb-2"
                >
                  <div style="width:150px">
                    <span class="status-badge" :class="isUpcoming(c.dateDebut) ? 'status-waiting' : 'status-open'">
                      <span class="status-dot"></span>
                      {{ isUpcoming(c.dateDebut) ? 'EN ATTENTE' : 'OUVERT' }}
                    </span>
                  </div>
                  <div class="flex-grow-1 pe-3">
                    <div class="fw-800 small">{{ c.nom }}</div>
                    <div class="text-muted" style="font-size:0.72rem">
                      Ferme {{ getRelativeTime(c.dateFin) }}
                    </div>
                  </div>
                  <div style="width:160px" class="small text-muted">{{ formatDate(c.dateFin) }}</div>
                  <div style="width:90px" class="text-center">
                    <span class="slot-badge">{{ c.dureeMinutes }} min</span>
                  </div>
                  <div style="width:80px" class="d-flex justify-content-center">
                    <button
                      @click="startExam(c)"
                      class="btn-enigma-primary"
                      style="padding:9px 14px;font-size:0.75rem;border-radius:12px;"
                      :disabled="isUpcoming(c.dateDebut)"
                    >
                      <div class="btn-content">
                        <i :class="isUpcoming(c.dateDebut) ? 'fa-solid fa-lock' : 'fa-solid fa-play'"></i>
                      </div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>
                </div>
              </div>
            </section>

            <!-- ═══════════════════ HISTORIQUE ═══════════════════ -->
            <section v-if="filteredExpired.length > 0">
              <div class="mb-4">
                <div class="section-tag-muted">
                  HISTORIQUE DES SESSIONS
                  <span class="count-pill-muted">{{ filteredExpired.length }}</span>
                </div>
              </div>

              <!-- GRID VIEW -->
              <div v-if="viewMode === 'grid'" class="row g-4">
                <div v-for="c in filteredExpired" :key="c.id" class="col-md-6 col-xl-4">
                  <div class="campaign-card-modern expired">
                    <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                      <span class="status-badge status-done">
                        <span class="status-dot"></span> TERMINÉ
                      </span>
                      <div class="duration-tag">
                        <i class="fa-regular fa-hourglass-half me-1"></i>
                        {{ c.dureeMinutes }} min
                      </div>
                    </div>
                    <h5 class="campaign-title-modern fw-800">{{ c.nom }}</h5>
                    <p v-if="c.description" class="test-description-pro">{{ c.description }}</p>
                    <div class="test-attachment-box mt-3 mb-3 p-3 rounded-4 d-flex align-items-center gap-3">
                      <div style="color:#94a3b8"><i class="fa-solid fa-calendar-check fa-lg"></i></div>
                      <div class="flex-grow-1 overflow-hidden">
                        <span class="text-overline d-block">Clôturé le</span>
                        <p class="m-0 fw-bold small">{{ formatDate(c.dateFin) }}</p>
                      </div>
                    </div>
                    <div class="progress-slim">
                      <div class="progress-fill" style="width:100%;background:#e2e8f0"></div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- LIST VIEW EXPIRED -->
              <div v-else class="list-view-pro animate__animated animate__fadeIn">
                <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
                  <span style="width:150px" class="list-col-label">STATUT</span>
                  <span class="flex-grow-1 list-col-label">NOM</span>
                  <span style="width:160px" class="list-col-label">CLÔTURÉ LE</span>
                  <span style="width:90px"  class="list-col-label text-center">DURÉE</span>
                </div>
                <div
                  v-for="c in filteredExpired"
                  :key="c.id"
                  class="list-row-item d-flex align-items-center px-4 py-3 mb-2 expired"
                >
                  <div style="width:150px">
                    <span class="status-badge status-done">
                      <span class="status-dot"></span> TERMINÉ
                    </span>
                  </div>
                  <div class="flex-grow-1 pe-3">
                    <div class="fw-800 small">{{ c.nom }}</div>
                  </div>
                  <div style="width:160px" class="small text-muted">{{ formatDate(c.dateFin) }}</div>
                  <div style="width:90px" class="text-center">
                    <span class="slot-badge-muted">{{ c.dureeMinutes }} min</span>
                  </div>
                </div>
              </div>
            </section>

          </div><!-- /v-else (loading) -->
        </div>
      </main>
    </div>

    <!-- ═══════════════════════════════════════════════════
         MODALE PREVIEW TEST
    ═══════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="previewModal.show" class="quantum-vault-overlay" @click.self="closePreview">
        <div class="test-preview-modal animate__animated animate__zoomIn animate__faster">

          <!-- HEADER -->
          <div class="preview-modal-header d-flex align-items-start justify-content-between mb-4">
            <div>
              <span class="status-badge status-open mb-2" style="display:inline-flex">
                <span class="status-dot"></span> OUVERT
              </span>
              <!-- backend: nom -->
              <h4 class="fw-900 mt-2 mb-1">{{ previewModal.data?.nom }}</h4>
              <p class="text-muted small mb-0">
                {{ previewModal.data?.description || 'Validation des compétences techniques.' }}
              </p>
            </div>
            <button @click="closePreview" class="btn-icon-sm flex-shrink-0 ms-3">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <!-- INFO GRID — only fields returned by backend -->
          <div class="row g-3 mb-4">
            <div class="col-6">
              <div class="preview-info-box">
                <div class="pib-icon text-amber"><i class="fa-regular fa-clock fa-lg"></i></div>
                <div>
                  <div class="text-overline">DURÉE</div>
                  <!-- backend: dureeMinutes -->
                  <div class="fw-800">{{ previewModal.data?.dureeMinutes }} min</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <div class="preview-info-box">
                <div class="pib-icon" style="color:#6366f1"><i class="fa-solid fa-users fa-lg"></i></div>
                <div>
                  <div class="text-overline">CANDIDATS</div>
                  <!-- backend: nbCandidats -->
                  <div class="fw-800">{{ previewModal.data?.nbCandidats ?? '—' }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <div class="preview-info-box">
                <div class="pib-icon" style="color:#10b981"><i class="fa-solid fa-user-group fa-lg"></i></div>
                <div>
                  <div class="text-overline">MAX PLACES</div>
                  <!-- backend: maxCandidats -->
                  <div class="fw-800">{{ previewModal.data?.maxCandidats ?? '—' }}</div>
                </div>
              </div>
            </div>
            <div class="col-6">
              <div class="preview-info-box">
                <div class="pib-icon" style="color:#f43f5e"><i class="fa-solid fa-calendar-xmark fa-lg"></i></div>
                <div>
                  <div class="text-overline">CLÔTURE</div>
                  <!-- backend: dateFin -->
                  <div class="fw-800">{{ formatDate(previewModal.data?.dateFin) }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- CHECKLIST PRÉ-TEST -->
          <div class="pre-exam-checklist mb-4">
            <div class="checklist-label mb-3">AVANT DE COMMENCER</div>
            <div class="check-item passed">
              <i class="fa-solid fa-check-circle text-success"></i>
              <span>Assurez-vous d'être dans un environnement calme</span>
            </div>
            <div class="check-item passed">
              <i class="fa-solid fa-check-circle text-success"></i>
              <span>Connexion internet stable recommandée</span>
            </div>
            <div class="check-item passed">
              <i class="fa-solid fa-check-circle text-success"></i>
              <span>Le timer démarre dès que vous accédez au test</span>
            </div>
            <div class="check-item">
              <i class="fa-regular fa-circle text-muted"></i>
              <span>Surveillance anti-triche activée</span>
            </div>
          </div>

          <!-- CTA -->
          <div class="d-flex gap-3">
            <button @click="closePreview" class="btn-qv-cancel flex-grow-1">ANNULER</button>
            <button
              @click="startExam(previewModal.data); closePreview()"
              class="btn-enigma-primary flex-grow-1"
            >
              <div class="btn-content justify-content-center">
                <i class="fa-solid fa-bolt me-2"></i>
                DÉMARRER MAINTENANT
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>SYSTEM MESSAGE</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, reactive } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const router      = useRouter();
const campaigns   = ref([]);
const loading     = ref(true);
const now         = ref(new Date());
const searchQuery = ref('');
const viewMode    = ref('grid');
let   timer       = null;

const mousePos     = reactive({ x: 0, y: 0 });
const previewModal = reactive({ show: false, data: null });
const globalToast  = reactive({ active: false, message: '', type: '', icon: '' });

/* ─────────────────────────────────────────────────────────────
   DATA FETCHING
   Backend (CampagnesController.GetCampagnes) returns per item:
     id             : Guid
     nom            : string
     description    : string?
     statut         : int   (0=Planifié, 1=Active, 2=Terminée)
     dateDebut      : DateTime
     dateFin        : DateTime
     dureeMinutes   : int
     creeLe         : DateTime
     maxCandidats   : int?
     questionnaireId: Guid
     nbCandidats    : int   (count of candidatures)
     candidatureId  : Guid  (current user's candidature id, or empty Guid)
────────────────────────────────────────────────────────────── */
const fetchMyTests = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Campagnes');
    campaigns.value = res.data;
  } catch (err) {
    console.error('Erreur API:', err);
    showPulseToast('Impossible de charger les tests.', 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    loading.value = false;
  }
};

/* ─────────────────────────────────────────────────────────────
   COMPUTED
   "active"  = dateFin >= now  (visible / open window)
   "expired" = dateFin <  now  (archived)
   "upcoming"= dateDebut > now (open window not yet started)
────────────────────────────────────────────────────────────── */
const activeTests   = computed(() => campaigns.value.filter(c => new Date(c.dateFin) >= now.value));
const expiredTests  = computed(() => campaigns.value.filter(c => new Date(c.dateFin) <  now.value));
const upcomingTests = computed(() => campaigns.value.filter(c => new Date(c.dateDebut) > now.value));

const filteredActive = computed(() => {
  const q = searchQuery.value.toLowerCase();
  if (!q) return activeTests.value;
  return activeTests.value.filter(c =>
    (c.nom         || '').toLowerCase().includes(q) ||
    (c.description || '').toLowerCase().includes(q)
  );
});

const filteredExpired = computed(() => {
  const q = searchQuery.value.toLowerCase();
  if (!q) return expiredTests.value;
  return expiredTests.value.filter(c => (c.nom || '').toLowerCase().includes(q));
});

/* ─────────────────────────────────────────────────────────────
   NAVIGATION
   candidatureId (Guid) is the correct exam-lobby param when set
   (backend sets it to the candidature id for the current user).
   Empty Guid "00000000-…" → fall back to campagne id.
────────────────────────────────────────────────────────────── */
const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';
const startExam = (campaign) => {
  const targetId =
    (campaign.candidatureId && campaign.candidatureId !== EMPTY_GUID)
      ? campaign.candidatureId
      : campaign.id;
  router.push(`/exam-lobby/${targetId}`);
};

/* ─────────────────────────────────────────────────────────────
   MODAL
────────────────────────────────────────────────────────────── */
const openPreview  = (c) => { previewModal.data = c; previewModal.show = true; };
const closePreview = ()  => { previewModal.show = false; previewModal.data = null; };

/* ─────────────────────────────────────────────────────────────
   DATE & TIME HELPERS
────────────────────────────────────────────────────────────── */
// dateDebut in the future → test not yet open
const isUpcoming = (dateDebut) => new Date(dateDebut) > now.value;

// "07 juin 2026, 01:28"
const formatDate = (d) => {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('fr-FR', {
    day: '2-digit', month: 'short', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  });
};

// "dans 29j" / "dans 3h" / "dans 12min"
const getRelativeTime = (dateStr) => {
  const diff = new Date(dateStr) - now.value;
  if (diff <= 0) return 'maintenant';
  const d = Math.floor(diff / 86400000);
  const h = Math.floor(diff / 3600000);
  const m = Math.floor(diff / 60000);
  if (d > 0)  return `dans ${d}j`;
  if (h > 0)  return `dans ${h}h`;
  if (m > 0)  return `dans ${m}min`;
  return 'bientôt';
};

// Live countdown "HH:MM:SS" shown on locked (upcoming) cards
const getCountdown = (dateDebut) => {
  const diff = new Date(dateDebut) - now.value;
  if (diff <= 0) return '00:00:00';
  const h = Math.floor(diff / 3600000).toString().padStart(2, '0');
  const m = Math.floor((diff % 3600000) / 60000).toString().padStart(2, '0');
  const s = Math.floor((diff % 60000)  / 1000).toString().padStart(2, '0');
  return `${h}:${m}:${s}`;
};

// % of the campaign window already elapsed
const getTimeProgress = (c) => {
  const total   = new Date(c.dateFin) - new Date(c.dateDebut);
  const elapsed = now.value - new Date(c.dateDebut);
  if (total <= 0) return 100;
  return Math.min(100, Math.max(0, Math.round((elapsed / total) * 100)));
};

// green → amber → red encodes urgency
const getProgressColor = (p) => {
  if (p >= 80) return '#f43f5e';
  if (p >= 50) return '#f59e0b';
  return '#10b981';
};

/* ─── TOAST ── */
let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

/* ─── PARALLAX ── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

/* ─── KEYBOARD ── */
const handleKeyboard = (e) => { if (e.key === 'Escape') closePreview(); };

/* ─── LIFECYCLE ── */
onMounted(() => {
  fetchMyTests();
  timer = setInterval(() => (now.value = new Date()), 1000);
  document.addEventListener('keydown', handleKeyboard);
});
onUnmounted(() => {
  clearInterval(timer);
  document.removeEventListener('keydown', handleKeyboard);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');

*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

/* ══ ROOT ═════════════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* ══ BACKGROUND ═══════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.15; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }

/* ══ LAYOUT ═══════════════════════════════════════════════ */
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }
.dashboard-view { max-width: 1440px; margin: 0 auto; }

/* ══ HEADER ═══════════════════════════════════════════════ */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle-pro { color: #64748b; font-size: 1rem; font-weight: 600; margin: 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root  { cursor: pointer; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator  { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current    { color: #0f172a; font-weight: 800; }

/* ══ STATS PILL ═══════════════════════════════════════════ */
.stats-bento-pill {
  background: white; border: 1px solid #eef2f6;
  padding: 1.2rem 2rem; border-radius: 2rem;
  display: flex; align-items: center;
  box-shadow: 0 4px 6px rgba(0,0,0,0.02);
}
.stat-value        { font-size: 2rem; font-weight: 800; color: #0f172a; line-height: 1; }
.stat-value.muted  { color: #94a3b8; }
.stat-value.indigo { color: #6366f1; }
.stat-label  { font-size: 0.6rem; font-weight: 700; color: #94a3b8; margin-top: 4px; letter-spacing: 0.05em; }
.stat-divider { width: 1px; height: 40px; background: #e2e8f0; margin: 0 1.5rem; }

/* ══ SEARCH & CONTROLS ════════════════════════════════════ */
.search-inline-box {
  display: flex; align-items: center;
  background: white; border: 1.5px solid #eef2f6;
  border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8;
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 11px 0; font-weight: 700; font-size: 0.85rem;
  flex: 1; font-family: inherit; color: #0f172a;
}
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }

.view-toggle-cluster {
  display: flex; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 16px; padding: 4px; gap: 4px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.02);
}
.btn-view-toggle {
  width: 38px; height: 38px; border-radius: 12px; border: none;
  background: transparent; color: #94a3b8; transition: 0.3s;
  display: flex; align-items: center; justify-content: center; cursor: pointer;
}
.btn-view-toggle:hover { background: #f8fafc; color: #0f172a; }
.btn-view-toggle.active { background: #0f172a; color: #f59e0b; box-shadow: 0 4px 12px rgba(15,23,42,0.2); }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) { border-color: #f59e0b; color: #f59e0b; }

/* ══ SECTION TAGS ═════════════════════════════════════════ */
.section-tag-premium {
  display: inline-flex; align-items: center; gap: 12px;
  font-size: 0.7rem; font-weight: 800; letter-spacing: 0.1em;
  color: #0f172a; background: white; padding: 8px 16px;
  border-radius: 100px; border: 1px solid #eef2f6;
  box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}
.section-tag-muted {
  display: inline-flex; align-items: center; gap: 10px;
  font-size: 0.7rem; font-weight: 800; letter-spacing: 0.1em;
  color: #94a3b8; background: #f8fafc; padding: 8px 16px;
  border-radius: 100px; border: 1px solid #eef2f6;
}
.count-pill       { background: #0f172a; color: #fbbf24; padding: 2px 8px; border-radius: 6px; font-size: 0.65rem; }
.count-pill-muted { background: #e2e8f0; color: #94a3b8;  padding: 2px 8px; border-radius: 6px; font-size: 0.65rem; }
.pulse-dot-amber  { width: 7px; height: 7px; background: #f59e0b; border-radius: 50%; animation: pulse 2s infinite; flex-shrink: 0; }
@keyframes pulse { 0%,100% { opacity:1; } 50% { opacity:0.3; } }

/* ══ CARDS ════════════════════════════════════════════════ */
.campaign-card-modern {
  background: white; border-radius: 30px; padding: 28px;
  border: 1px solid #eef2f6;
  transition: 0.3s cubic-bezier(0.4,0,0.2,1);
  height: 100%; cursor: default;
}
.campaign-card-modern.card-clickable { cursor: pointer; }
.campaign-card-modern.card-clickable:hover {
  transform: translateY(-10px);
  border-color: #f59e0b;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08);
}
.campaign-card-modern.expired       { opacity: 0.65; }
.campaign-card-modern.expired:hover { opacity: 0.9; transform: translateY(-4px); }
.campaign-card-modern.is-upcoming   { border-left: 4px solid #6366f1; }

.card-header-modern    { display: flex; justify-content: space-between; align-items: flex-start; }
.campaign-title-modern { font-size: 1rem; color: #0f172a; }
.test-description-pro  { color: #64748b; font-size: 0.88rem; font-weight: 600; line-height: 1.5; margin: 0; }
.duration-tag          { font-size: 0.75rem; font-weight: 600; color: #94a3b8; }

/* ══ STATUS BADGES ════════════════════════════════════════ */
.status-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 0.65rem; font-weight: 800; text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-open    { background: #ecfdf5; color: #10b981; }
.status-done    { background: #f1f5f9; color: #64748b; }
.status-waiting { background: #eef2ff; color: #6366f1; }
.status-dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; margin-right: 6px; }

/* ══ ATTACHMENT BOX ═══════════════════════════════════════ */
.test-attachment-box { background: #f8fafc; border: 1px solid #f1f5f9; }
.text-overline { font-size: 0.6rem; font-weight: 900; opacity: 0.5; text-transform: uppercase; letter-spacing: 1px; }
.text-amber { color: #f59e0b; }
.icon-file  { flex-shrink: 0; }

/* ══ NB CANDIDATS BOX ═════════════════════════════════════ */
.questions-count-box {
  display: flex; flex-direction: column; align-items: center;
  background: #fffbeb; border-radius: 10px; padding: 4px 10px; flex-shrink: 0;
}
.q-count { font-size: 1rem; font-weight: 900; color: #f59e0b; line-height: 1; }
.q-label { font-size: 0.55rem; font-weight: 900; color: #fbbf24; }

/* ══ PROGRESS BAR ═════════════════════════════════════════ */
.progress-slim { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: width 0.6s ease; }

/* ══ BUTTONS ══════════════════════════════════════════════ */
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
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px; border: 1.5px solid #eef2f6;
  background: white; color: #64748b; cursor: pointer; transition: 0.2s;
  font-size: 0.75rem; display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }

.btn-qv-cancel {
  background: #f1f5f9; color: #64748b; border: none;
  padding: 14px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
}
.btn-qv-cancel:hover { background: #e2e8f0; }

/* ══ LIST VIEW ════════════════════════════════════════════ */
.list-header-row { background: #f8fafc; border-radius: 14px; }
.list-col-label  { font-size: 0.6rem; font-weight: 900; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
.list-row-item   { background: white; border-radius: 16px; border: 1px solid #eef2f6; transition: 0.2s; }
.list-row-item:hover { border-color: #f59e0b; }
.list-row-item.expired       { opacity: 0.65; }
.list-row-item.expired:hover { opacity: 0.9; }
.slot-badge       { background: #fffbeb; color: #f59e0b; font-weight: 800; font-size: 0.75rem; padding: 3px 10px; border-radius: 8px; }
.slot-badge-muted { background: #f1f5f9; color: #94a3b8;  font-weight: 700; font-size: 0.75rem; padding: 3px 10px; border-radius: 8px; }

/* ══ EMPTY STATE ══════════════════════════════════════════ */
.empty-state-pro { background: white; border-radius: 30px; padding: 40px; border: 1px dashed #e2e8f0; }

/* ══ LOADER ═══════════════════════════════════════════════ */
.loader-portal-pro {
  display: flex; flex-direction: column; align-items: center;
  justify-content: center; min-height: 300px; gap: 20px;
}
.spinner-pro-premium {
  width: 50px; height: 50px;
  border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b;
  border-radius: 50%; animation: spin 1s linear infinite;
}
.loading-text-pro { font-weight: 700; color: #64748b; letter-spacing: 0.05em; font-size: 0.8rem; }
@keyframes spin { to { transform: rotate(360deg); } }

/* ══ MODAL ════════════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; background: rgba(15,23,42,0.6);
  backdrop-filter: blur(10px); z-index: 2000;
  display: flex; align-items: center; justify-content: center;
}
.test-preview-modal {
  width: 520px; max-width: 96vw;
  background: white; border-radius: 40px; padding: 40px;
  box-shadow: 0 40px 100px rgba(0,0,0,0.2);
  max-height: 90vh; overflow-y: auto;
}
.preview-modal-header { border-bottom: 1px solid #eef2f6; padding-bottom: 20px; }
.preview-info-box {
  background: #f8fafc; border: 1px solid #eef2f6;
  border-radius: 16px; padding: 16px;
  display: flex; align-items: center; gap: 14px;
}
.pib-icon { font-size: 1.2rem; flex-shrink: 0; }

/* ══ PRE-EXAM CHECKLIST ═══════════════════════════════════ */
.pre-exam-checklist { background: #f8fafc; border-radius: 20px; padding: 20px 24px; }
.checklist-label { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; text-transform: uppercase; }
.check-item {
  display: flex; align-items: center; gap: 10px;
  font-size: 0.8rem; font-weight: 600; padding: 7px 0;
  opacity: 0.4; transition: 0.3s;
}
.check-item.passed { opacity: 1; }

/* ══ TOAST ════════════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 20px 30px;
  border-radius: 20px; display: flex; align-items: center; gap: 15px;
  z-index: 3000; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* ══ TRANSITIONS ══════════════════════════════════════════ */
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.92); } to { opacity: 1; transform: scale(1); } }

/* ══ SCROLLBAR ════════════════════════════════════════════ */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

/* ══ MISC ═════════════════════════════════════════════════ */
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.text-amber   { color: #f59e0b !important; }
.text-success { color: #10b981 !important; }
.text-danger  { color: #f43f5e !important; }

/* ══════════════════════════════════════════════════════════
   DARK MODE
══════════════════════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root  { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine      { background: #0d1117; }
[data-theme="dark"] .premium-title      { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .subtitle-pro       { color: #8b949e; }

[data-theme="dark"] .stats-bento-pill   { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .stat-value         { color: #f0f6fc; }

[data-theme="dark"] .search-inline-box   { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] .view-toggle-cluster { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .btn-view-toggle     { color: #8b949e; }
[data-theme="dark"] .btn-view-toggle.active { background: #f0f6fc; color: #0d1117; }
[data-theme="dark"] .btn-refresh-pro     { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }

[data-theme="dark"] .section-tag-premium { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .section-tag-muted   { background: #0d1117; border-color: rgba(255,255,255,0.05); color: #8b949e; }
[data-theme="dark"] .count-pill          { background: #f0f6fc; color: #0d1117; }

[data-theme="dark"] .campaign-card-modern                    { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .campaign-card-modern.card-clickable:hover { border-color: #d97706; box-shadow: 0 20px 40px rgba(0,0,0,0.5); }
[data-theme="dark"] .campaign-card-modern.is-upcoming        { border-left-color: #818cf8; }
[data-theme="dark"] .campaign-title-modern                   { color: #f0f6fc; }
[data-theme="dark"] .test-description-pro                    { color: #8b949e; }

[data-theme="dark"] .test-attachment-box  { background: rgba(255,255,255,0.04) !important; border-color: rgba(255,255,255,0.06) !important; }
[data-theme="dark"] .test-attachment-box p { color: #f0f6fc; }

[data-theme="dark"] .progress-slim { background: rgba(255,255,255,0.06); }

[data-theme="dark"] .list-row-item       { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .list-row-item:hover { border-color: #d97706; }

[data-theme="dark"] .empty-state-pro    { background: #161b22; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .empty-state-pro h5 { color: #f0f6fc; }

[data-theme="dark"] .test-preview-modal         { background: #161b22; }
[data-theme="dark"] .preview-modal-header       { border-bottom-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .test-preview-modal h4      { color: #f0f6fc; }
[data-theme="dark"] .preview-info-box           { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .preview-info-box .fw-800   { color: #f0f6fc; }
[data-theme="dark"] .pre-exam-checklist         { background: rgba(255,255,255,0.04); }

[data-theme="dark"] .btn-enigma-primary:hover .btn-content { color: #0d1117; }
[data-theme="dark"] .btn-icon-sm   { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .btn-icon-sm:hover { background: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-qv-cancel { background: rgba(255,255,255,0.06); color: #8b949e; }

[data-theme="dark"] .quantum-grid {
  background-image: radial-gradient(rgba(255,255,255,0.04) 1px, transparent 1px);
}
[data-theme="dark"] .custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(255,255,255,0.08); }
</style>