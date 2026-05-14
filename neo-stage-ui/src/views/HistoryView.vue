<template>
  <div class="enigma-master-root d-flex overflow-hidden" :data-theme="theme">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-blue"></div>
      <div class="quantum-grid"></div>
    </div>

    <!-- SIDEBAR -->
    <AppSidebar />

    <!-- MAIN -->
    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar @toggle-theme="toggleTheme" :theme="theme" />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root" @click="$router.push('/dashboard')" style="cursor:pointer">{{ t('sidebar.dashboard') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ t('history.breadcrumb') }}</span>
              </div>
              <div class="d-flex align-items-center gap-2 mb-1">
                <div class="live-indicator"></div>
                <span class="top-label">{{ t('history.subtitle') }}</span>
              </div>
              <h2 class="premium-title">{{ t('history.title').split(' ')[0] }} <span class="gradient-text">{{ t('history.title').split(' ').slice(1).join(' ') }}</span></h2>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <button class="btn-refresh-pro" @click="fetchHistory" :disabled="loading" :title="t('refresh')">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <div class="view-toggle-cluster">
                <button
                  class="btn-view-toggle"
                  :class="{ active: viewMode === 'list' }"
                  @click="viewMode = 'list'"
                  :title="t('view')"
                ><i class="fa-solid fa-list-ul"></i></button>
                <button
                  class="btn-view-toggle"
                  :class="{ active: viewMode === 'grid' }"
                  @click="viewMode = 'grid'"
                  :title="t('view')"
                ><i class="fa-solid fa-table-cells-large"></i></button>
                <button
                  class="btn-view-toggle"
                  :class="{ active: themeLocal === 'light' }"
                  @click="themeLocal = 'light'"
                  :title="t('theme.light')"
                ><i class="fa-solid fa-sun"></i></button>
                <button
                  class="btn-view-toggle"
                  :class="{ active: themeLocal === 'dark' }"
                  @click="themeLocal = 'dark'"
                  :title="t('theme.dark')"
                ><i class="fa-solid fa-moon"></i></button>
              </div>
              <div class="search-inline-box">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" v-model="searchQuery" :placeholder="t('search')" class="search-inline-input">
                <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search"><i class="fa-solid fa-xmark"></i></button>
              </div>
            </div>
          </header>

          <!-- KPI ROW -->
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
                <div v-if="stat.trend !== undefined" class="stat-trend ms-auto" :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
                  <i :class="stat.trend >= 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ Math.abs(stat.trend) }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- FILTRES TABS -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="tabs-container">
              <div class="d-flex gap-2 p-1 bg-white-tab rounded-4 shadow-sm border-tab">
                <button v-for="tab in filterTabs" :key="tab.val"
                  class="nav-tab-btn-modern" :class="{ active: activeTab === tab.val }"
                  @click="activeTab = tab.val">
                  {{ tab.label }} <span class="tab-count">{{ tab.count }}</span>
                </button>
              </div>
            </div>
            <div class="d-flex gap-2 align-items-center">
              <select v-model="sortBy" class="sort-select-pro">
                <option value="date">{{ t('history.sort.date') }}</option>
                <option value="score">{{ t('history.sort.score') }}</option>
                <option value="name">{{ t('history.sort.name') }}</option>
              </select>
            </div>
          </div>

          <!-- LOADER -->
          <div v-if="loading" class="loader-portal">
            <div class="robot-ring mb-3"></div>
            <span class="loading-text">{{ t('history.loading') }}</span>
          </div>

          <!-- ÉTAT VIDE -->
          <div v-else-if="filteredHistory.length === 0 && !searchQuery" class="bento-empty-card text-center py-5">
            <div class="empty-visual mb-4">
              <i class="fa-solid fa-layer-group"></i>
            </div>
            <h3 class="fw-800 mb-2">{{ t('history.empty.title') }}</h3>
            <p class="text-muted-pro">{{ t('history.empty.desc') }}</p>
            <button @click="$router.push('/dashboard')" class="btn-enigma-primary mt-4">
              <div class="btn-content"><i class="fa-solid fa-rocket me-2"></i>{{ t('history.empty.start') }}</div>
              <div class="btn-glow"></div>
            </button>
          </div>

          <!-- RÉSULTATS VIDE (filtre) -->
          <div v-else-if="filteredHistory.length === 0" class="text-center py-5 text-muted-pro">
            <i class="fa-solid fa-magnifying-glass fa-2x mb-3"></i>
            <p class="fw-700">{{ t('history.noResults', { query: searchQuery }) }}</p>
          </div>

          <!-- ══ VUE LISTE ══ -->
          <div v-else-if="viewMode === 'list'" class="d-flex flex-column gap-4">
            <div
              v-for="test in filteredHistory"
              :key="test.id"
              class="history-item-card animate__animated animate__fadeInUp"
            >
              <div class="card-accent" :class="test.score >= (test.scoreReussite || 70) ? 'bg-success-accent' : 'bg-danger-accent'"></div>

              <div class="row align-items-center g-0">
                <!-- Score -->
                <div class="col-md-2 p-4 text-center border-end-glass">
                  <div class="score-display mx-auto">
                    <svg class="score-circle" viewBox="0 0 36 36">
                      <path class="circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                      <path class="circle-fill"
                        :style="`stroke-dasharray: ${test.score}, 100`"
                        :class="test.score >= (test.scoreReussite || 70) ? 'stroke-success' : 'stroke-danger'"
                        d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                    </svg>
                    <div class="score-text">
                      <span class="num">{{ test.score }}</span><span class="per">%</span>
                    </div>
                  </div>
                  <div class="mt-2">
                    <span class="result-pill-sm" :class="test.score >= (test.scoreReussite || 70) ? 'pill-pass' : 'pill-fail'">
                      {{ test.score >= (test.scoreReussite || 70) ? t('history.card.passed') : t('history.card.failed') }}
                    </span>
                  </div>
                </div>

                <!-- Infos -->
                <div class="col-md-7 px-md-5 py-4">
                  <div class="d-flex align-items-center gap-3 mb-2 flex-wrap">
                    <span class="badge-glass">{{ test.statut || t('history.card.ended') }}</span>
                    <span class="date-text"><i class="fa-regular fa-calendar me-1"></i>{{ formatDate(test.date) }}</span>
                    <span v-if="test.infractions !== undefined" class="integrity-badge">
                      <i class="fa-solid fa-shield-halved me-1"></i>
                      {{ t('history.card.integrity', { pct: Math.max(0, 100 - (test.infractions || 0) * 10) }) }}
                    </span>
                  </div>
                  <h3 class="test-name mb-2">{{ test.titreExamen || test.titre }}</h3>
                  <div class="meta-info d-flex gap-3 flex-wrap">
                    <span v-if="test.nombreQuestions">
                      <i class="fa-solid fa-list-check me-1" style="color:#6366f1"></i>
                      {{ test.nombreQuestions }} questions
                    </span>
                    <span v-if="test.dureeMinutes">
                      <i class="fa-solid fa-clock me-1" style="color:#f59e0b"></i>
                      {{ test.dureeMinutes }} min
                    </span>
                    <span>
                      <i class="fa-solid fa-trophy me-1" style="color:#f59e0b"></i>
                      {{ t('results.score.label') }} :
                      <strong :class="test.score >= (test.scoreReussite || 70) ? 'text-success' : 'text-danger'">
                        {{ test.score >= (test.scoreReussite || 70) ? t('history.card.passed') : t('history.card.failed') }}
                      </strong>
                    </span>
                  </div>

                  <div v-if="test.theme" class="mt-3">
                    <div class="d-flex align-items-center gap-2">
                      <span class="theme-tag"><i class="fa-solid fa-tag me-1"></i>{{ test.theme }}</span>
                    </div>
                  </div>

                  <div class="mt-3">
                    <div class="metric-bar">
                      <div class="mbar-fill" :style="{
                        width: test.score + '%',
                        background: test.score >= (test.scoreReussite || 70) ? '#10b981' : '#f43f5e'
                      }"></div>
                    </div>
                  </div>
                </div>

                <!-- Action -->
                <div class="col-md-3 p-4 text-end">
                  <router-link :to="`/results/${test.id}`" class="btn-elite-action">
                    <span>{{ t('history.card.report') }}</span>
                    <div class="icon-box">
                      <i class="fa-solid fa-arrow-right-long"></i>
                    </div>
                  </router-link>
                  <div v-if="test.scoreReussite" class="mt-2 text-center">
                    <span class="small text-muted-pro" style="font-size:0.65rem;">{{ t('history.card.threshold') }} {{ test.scoreReussite }}%</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ══ VUE GRILLE ══ -->
          <div v-else class="row g-5">
            <div v-for="test in filteredHistory" :key="test.id" class="col-xl-4 col-md-6 animate__animated animate__fadeInUp">
              <div class="campaign-card-modern h-100">
                <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                  <span class="result-pill-sm" :class="test.score >= (test.scoreReussite || 70) ? 'pill-pass' : 'pill-fail'">
                    <span class="status-dot"></span>
                    {{ test.score >= (test.scoreReussite || 70) ? t('history.card.passed') : t('history.card.failed') }}
                  </span>
                  <span class="date-text">{{ formatDate(test.date) }}</span>
                </div>

                <h5 class="campaign-title-modern fw-800 mb-3">{{ test.titreExamen || test.titre }}</h5>

                <!-- Score ring compact -->
                <div class="d-flex align-items-center gap-4 mb-3">
                  <div class="score-display-sm">
                    <svg class="score-circle" viewBox="0 0 36 36">
                      <path class="circle-bg" d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                      <path class="circle-fill"
                        :style="`stroke-dasharray: ${test.score}, 100`"
                        :class="test.score >= (test.scoreReussite || 70) ? 'stroke-success' : 'stroke-danger'"
                        d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" />
                    </svg>
                    <div class="score-text">
                      <span class="num">{{ test.score }}</span><span class="per">%</span>
                    </div>
                  </div>
                  <div class="flex-grow-1">
                    <div v-if="test.theme" class="mb-2">
                      <span class="theme-tag"><i class="fa-solid fa-tag me-1"></i>{{ test.theme }}</span>
                    </div>
                    <div v-if="test.nombreQuestions" class="small text-muted-pro fw-700">
                      <i class="fa-solid fa-list-check me-1" style="color:#6366f1"></i>{{ test.nombreQuestions }} questions
                    </div>
                    <div v-if="test.infractions !== undefined" class="small text-muted-pro fw-700 mt-1">
                      <i class="fa-solid fa-shield-halved me-1" style="color:#10b981"></i>
                      {{ t('history.card.integrity', { pct: Math.max(0, 100 - (test.infractions || 0) * 10) }) }}
                    </div>
                  </div>
                </div>

                <div class="progress-slim mb-3">
                  <div class="progress-fill" :style="{
                    width: test.score + '%',
                    background: test.score >= (test.scoreReussite || 70) ? '#10b981' : '#f43f5e'
                  }"></div>
                </div>

                <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3 border-top border-light-pro">
                  <span class="badge-glass">{{ test.statut || t('history.card.ended') }}</span>
                  <router-link :to="`/results/${test.id}`" class="btn-elite-action btn-elite-sm">
                    <span>{{ t('history.card.shortReport') }}</span>
                    <div class="icon-box icon-box-sm">
                      <i class="fa-solid fa-arrow-right-long"></i>
                    </div>
                  </router-link>
                </div>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="toast.active" class="enigma-toast" :class="toast.type">
        <div class="t-ico"><i :class="toast.icon"></i></div>
        <div class="t-body"><strong>SYSTEM</strong><p class="m-0 small">{{ toast.message }}</p></div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t } = useI18n();
const router = useRouter();

/* ─── THEME ─────────────────────────────────────────────────────── */
const themeLocal = ref(localStorage.getItem('app-theme') || 'light');
const theme = computed(() => themeLocal.value);
const toggleTheme = () => {
  themeLocal.value = themeLocal.value === 'light' ? 'dark' : 'light';
  localStorage.setItem('app-theme', themeLocal.value);
};

/* ─── ÉTAT ─────────────────────────────────────────────────────── */
const historyData  = ref([]);
const loading      = ref(true);
const searchQuery  = ref('');
const sortBy       = ref('date');
const activeTab    = ref('all');
const viewMode     = ref('list');
const toast        = reactive({ active: false, message: '', type: '', icon: '' });
let _toastTimer = null;

/* ─── FETCH ─────────────────────────────────────────────────────── */
const fetchHistory = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Examen/historique');
    historyData.value = res.data;
  } catch (err) {
    console.error('Erreur historique:', err);
    // DEMO FALLBACK
    historyData.value = [
      {
        id: '1', titreExamen: 'Frontend Senior Audit — Vue.js', score: 85, date: new Date().toISOString(),
        statut: 'Terminé', scoreReussite: 70, theme: 'Frontend Architect', nombreQuestions: 20, dureeMinutes: 60, infractions: 0
      },
      {
        id: '2', titreExamen: 'Backend Node.js Architecture', score: 62, date: new Date(Date.now() - 86400000).toISOString(),
        statut: 'Terminé', scoreReussite: 70, theme: 'Backend Specialist', nombreQuestions: 15, dureeMinutes: 45, infractions: 1
      },
      {
        id: '3', titreExamen: 'DevOps & CI/CD Pipeline', score: 91, date: new Date(Date.now() - 172800000).toISOString(),
        statut: 'Terminé', scoreReussite: 70, theme: 'DevOps', nombreQuestions: 25, dureeMinutes: 90, infractions: 0
      },
      {
        id: '4', titreExamen: 'Data Engineering — SQL Avancé', score: 54, date: new Date(Date.now() - 259200000).toISOString(),
        statut: 'Terminé', scoreReussite: 70, theme: 'Data Engineering', nombreQuestions: 18, dureeMinutes: 60, infractions: 2
      },
    ];
    showToast(t('history.demo'), 'warn', 'fa-solid fa-plug-circle-xmark');
  } finally {
    loading.value = false;
  }
};

/* ─── COMPUTED ──────────────────────────────────────────────────── */
const passedSessions = computed(() =>
  historyData.value.filter(item => item.score >= (item.scoreReussite || 70))
);
const failedSessions = computed(() =>
  historyData.value.filter(item => item.score < (item.scoreReussite || 70))
);
const avgScore = computed(() => {
  if (!historyData.value.length) return 0;
  return Math.round(historyData.value.reduce((a, b) => a + b.score, 0) / historyData.value.length);
});
const bestScore = computed(() => {
  if (!historyData.value.length) return 0;
  return Math.max(...historyData.value.map(item => item.score));
});

const kpiStats = computed(() => [
  { label: t('history.kpi.total'),    value: historyData.value.length,     icon: 'fa-solid fa-layer-group',  color: '#6366f1', bg: 'rgba(99,102,241,0.1)',  trend: 12 },
  { label: t('history.kpi.passed'),   value: passedSessions.value.length,  icon: 'fa-solid fa-medal',        color: '#10b981', bg: 'rgba(16,185,129,0.1)',  trend: 5  },
  { label: t('history.kpi.avgScore'), value: avgScore.value + '%',         icon: 'fa-solid fa-chart-line',   color: '#f59e0b', bg: 'rgba(245,158,11,0.1)',  trend: 8  },
  { label: t('history.kpi.best'),     value: bestScore.value + '%',        icon: 'fa-solid fa-trophy',       color: '#f43f5e', bg: 'rgba(244,63,94,0.1)'             },
]);

const filterTabs = computed(() => [
  { label: t('history.tabs.all'),  val: 'all',  count: historyData.value.length },
  { label: t('history.tabs.pass'), val: 'pass', count: passedSessions.value.length },
  { label: t('history.tabs.fail'), val: 'fail', count: failedSessions.value.length },
]);

const filteredHistory = computed(() => {
  let list = [...historyData.value];

  if (activeTab.value === 'pass')
    list = list.filter(item => item.score >= (item.scoreReussite || 70));
  else if (activeTab.value === 'fail')
    list = list.filter(item => item.score < (item.scoreReussite || 70));

  if (searchQuery.value)
    list = list.filter(item =>
      (item.titreExamen || item.titre || '').toLowerCase().includes(searchQuery.value.toLowerCase()) ||
      (item.theme || '').toLowerCase().includes(searchQuery.value.toLowerCase())
    );

  if (sortBy.value === 'score')
    list.sort((a, b) => b.score - a.score);
  else if (sortBy.value === 'name')
    list.sort((a, b) => (a.titreExamen || '').localeCompare(b.titreExamen || ''));
  else
    list.sort((a, b) => new Date(b.date) - new Date(a.date));

  return list;
});

/* ─── HELPERS ────────────────────────────────────────────────── */
const formatDate = (d) => {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', year: 'numeric' });
};

const showToast = (message, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(toast, { message, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { toast.active = false; }, 4500);
};

onMounted(fetchHistory);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800;900&family=JetBrains+Mono:wght@600;800&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css');

/* ─── ROOT ──────────────────────────────────────────────── */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  transition: background 0.3s, color 0.3s;
}

/* ─── DARK MODE ─────────────────────────────────────────── */
[data-theme="dark"].enigma-master-root,
[data-theme="dark"] .enigma-master-root {
  background: #0d1117;
  color: #f0f6fc;
}
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .gradient-text { background: linear-gradient(135deg, #f59e0b, #fbbf24); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
[data-theme="dark"] .stat-card-premium { background: rgba(22,27,34,0.8); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }
[data-theme="dark"] .stat-label { color: #8b949e; }
[data-theme="dark"] .text-muted-pro { color: #8b949e !important; }
[data-theme="dark"] .bg-white-tab { background: #161b22 !important; }
[data-theme="dark"] .border-tab { border-color: rgba(255,255,255,0.08) !important; }
[data-theme="dark"] .nav-tab-btn-modern { color: #8b949e; }
[data-theme="dark"] .nav-tab-btn-modern.active { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .nav-tab-btn-modern:not(.active) .tab-count { background: rgba(255,255,255,0.06); color: #8b949e; }
[data-theme="dark"] .view-toggle-cluster { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .btn-view-toggle { color: #8b949e; }
[data-theme="dark"] .btn-view-toggle:hover { background: rgba(255,255,255,0.05); color: #f0f6fc; }
[data-theme="dark"] .btn-view-toggle.active { background: #0d1117; color: #f59e0b; }
[data-theme="dark"] .btn-refresh-pro { background: #161b22; border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .search-inline-box { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] .sort-select-pro { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .history-item-card { background: rgba(22,27,34,0.8); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .history-item-card:hover { border-color: #f59e0b; background: #161b22; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.5); }
[data-theme="dark"] .test-name { color: #f0f6fc; }
[data-theme="dark"] .badge-glass { background: rgba(255,255,255,0.06); color: #8b949e; }
[data-theme="dark"] .date-text { color: #8b949e; }
[data-theme="dark"] .meta-info { color: #8b949e; }
[data-theme="dark"] .border-end-glass { border-color: rgba(255,255,255,0.06) !important; }
[data-theme="dark"] .campaign-card-modern { background: rgba(22,27,34,0.8); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .campaign-card-modern:hover { border-color: #f59e0b; background: #161b22; }
[data-theme="dark"] .campaign-title-modern { color: #f0f6fc; }
[data-theme="dark"] .border-light-pro { border-color: rgba(255,255,255,0.06) !important; }
[data-theme="dark"] .circle-bg { stroke: rgba(255,255,255,0.08); }
[data-theme="dark"] .metric-bar { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .progress-slim { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .bento-empty-card { background: rgba(22,27,34,0.5); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .empty-visual { background: rgba(245,158,11,0.1); color: #f59e0b; }
[data-theme="dark"] h3 { color: #f0f6fc; }
[data-theme="dark"] .integrity-badge { background: rgba(16,185,129,0.1); color: #10b981; }

/* BACKGROUND */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 1px, transparent 1px); background-size: 40px 40px; opacity: 0.18; }
[data-theme="dark"] .quantum-grid { opacity: 0.07; }
.glow-orb { position: absolute; width: 600px; height: 600px; filter: blur(120px); opacity: 0.12; border-radius: 50%; }
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* HEADER */
.premium-title { font-size: 3.5rem; font-weight: 800; color: #0f172a; letter-spacing: -0.04em; }
.gradient-text { background: linear-gradient(135deg, #f59e0b, #d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.top-label { font-size: 0.7rem; font-weight: 800; color: #64748b; letter-spacing: 0.2em; }
.live-indicator { width: 8px; height: 8px; background: #fbbf24; border-radius: 50%; box-shadow: 0 0 10px #fbbf24; animation: pulse 2s infinite; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
.text-muted-pro { color: #94a3b8; }

/* SEARCH */
.search-inline-box { display: flex; align-items: center; background: white; border: 1.5px solid #eef2f6; border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8; }
.search-inline-input { border: none; outline: none; background: transparent; padding: 10px 0; font-weight: 700; font-size: 0.85rem; width: 200px; font-family: inherit; color: #0f172a; }
[data-theme="dark"] .search-inline-input::placeholder { color: #8b949e; }
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }

/* VIEW TOGGLE */
.view-toggle-cluster { display: flex; background: white; border: 1.5px solid #e2e8f0; border-radius: 16px; padding: 4px; gap: 4px; }
.btn-view-toggle { width: 38px; height: 38px; border-radius: 12px; border: none; background: transparent; color: #94a3b8; transition: 0.3s; display: flex; align-items: center; justify-content: center; cursor: pointer; }
.btn-view-toggle:hover { background: #f8fafc; color: #0f172a; }
.btn-view-toggle.active { background: #0f172a; color: #f59e0b; box-shadow: 0 4px 12px rgba(15,23,42,0.2); }

.btn-refresh-pro { width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0; border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s; display: flex; align-items: center; justify-content: center; }
.btn-refresh-pro:hover:not(:disabled) { border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg); }

/* SORT */
.sort-select-pro { border: 1.5px solid #eef2f6; border-radius: 14px; padding: 10px 14px; font-weight: 700; font-size: 0.8rem; background: white; outline: none; cursor: pointer; font-family: inherit; }

/* KPI */
.stat-card-premium { background: white; border-radius: 24px; padding: 24px; display: flex; align-items: center; border: 1px solid #eef2f6; transition: 0.2s; }
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper { width: 56px; height: 56px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.4rem; flex-shrink: 0; margin-right: 16px; }
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up { color: #10b981; } .trend-down { color: #f43f5e; }

/* TABS */
.bg-white-tab { background: white; }
.border-tab { border: 1px solid #e2e8f0; }
.nav-tab-btn-modern { padding: 8px 18px; border-radius: 12px; border: none; background: transparent; font-weight: 800; font-size: 0.8rem; color: #94a3b8; cursor: pointer; transition: 0.2s; font-family: inherit; }
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.tab-count { background: #f1f5f9; color: #64748b; padding: 2px 7px; border-radius: 8px; font-size: 0.65rem; margin-left: 6px; }
.nav-tab-btn-modern.active .tab-count { background: rgba(255,255,255,0.15); color: rgba(255,255,255,0.8); }

/* LOADER */
.loader-portal { display: flex; flex-direction: column; align-items: center; padding: 80px 0; }
.robot-ring { width: 56px; height: 56px; border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; }
[data-theme="dark"] .robot-ring { border-color: rgba(255,255,255,0.06); border-top-color: #f59e0b; }
.loading-text { font-weight: 800; color: #94a3b8; font-size: 0.7rem; letter-spacing: 0.4em; }
@keyframes spin { to { transform: rotate(360deg); } }
@keyframes pulse { 0%,100% { transform: scale(1); opacity: 1; } 50% { transform: scale(1.5); opacity: 0.5; } }

/* EMPTY */
.bento-empty-card { background: white; border-radius: 32px; padding: 60px; border: 1px dashed #e2e8f0; }
.empty-visual { width: 80px; height: 80px; background: #fffbeb; color: #f59e0b; border-radius: 24px; display: flex; align-items: center; justify-content: center; font-size: 2rem; margin: 0 auto; }

/* HISTORY ITEM (LIST VIEW) */
.history-item-card {
  background: rgba(255,255,255,0.85);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(226,232,240,0.8);
  border-radius: 2rem;
  position: relative;
  overflow: hidden;
  transition: all 0.5s cubic-bezier(0.19,1,0.22,1);
  box-shadow: 0 4px 6px -1px rgba(0,0,0,0.04);
}
.history-item-card:hover {
  transform: translateX(12px) scale(1.008);
  background: white;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08);
  border-color: #fbbf24;
}
.card-accent { position: absolute; left: 0; top: 0; bottom: 0; width: 6px; }
.bg-success-accent { background: linear-gradient(180deg, #10b981, #34d399); }
.bg-danger-accent  { background: linear-gradient(180deg, #f43f5e, #fb7185); }
.border-end-glass { border-right: 1px solid rgba(0,0,0,0.06); }
.score-display { position: relative; width: 80px; height: 80px; margin: 0 auto; }
.score-circle { transform: rotate(-90deg); }
.circle-bg   { fill: none; stroke: #f1f5f9; stroke-width: 3.5; }
.circle-fill { fill: none; stroke-width: 3.5; stroke-linecap: round; transition: stroke-dasharray 1s ease; }
.stroke-success { stroke: #10b981; filter: drop-shadow(0 0 5px rgba(16,185,129,0.3)); }
.stroke-danger  { stroke: #f43f5e; }
.score-text { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; font-family: 'JetBrains Mono', monospace; }
.score-text .num { font-size: 1.5rem; font-weight: 800; color: #0f172a; }
[data-theme="dark"] .score-text .num { color: #f0f6fc; }
.score-text .per { font-size: 0.7rem; color: #94a3b8; font-weight: 700; margin-left: 1px; }
.test-name { font-size: 1.5rem; font-weight: 800; color: #0f172a; letter-spacing: -0.02em; }
.badge-glass { background: #f1f5f9; color: #475569; font-size: 0.65rem; font-weight: 800; padding: 4px 10px; border-radius: 8px; letter-spacing: 0.05em; }
.date-text { font-size: 0.85rem; color: #94a3b8; font-weight: 600; }
.meta-info { font-size: 0.8rem; color: #64748b; font-weight: 600; }
.result-pill-sm { padding: 5px 12px; border-radius: 10px; font-size: 0.65rem; font-weight: 900; display: inline-flex; align-items: center; gap: 6px; }
.pill-pass { background: #ecfdf5; color: #10b981; }
.pill-fail { background: #fff1f2; color: #f43f5e; }
.status-dot { width: 5px; height: 5px; border-radius: 50%; background: currentColor; }
.theme-tag { background: #fffbeb; color: #f59e0b; font-size: 0.65rem; font-weight: 800; padding: 3px 10px; border-radius: 8px; }
.integrity-badge { background: #ecfdf5; color: #10b981; font-size: 0.65rem; font-weight: 800; padding: 4px 10px; border-radius: 8px; }
.metric-bar  { height: 5px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.mbar-fill   { height: 100%; border-radius: 10px; transition: width 1s ease; }
.progress-slim { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: width 0.6s ease; }

/* GRID VIEW - CAMPAIGN CARD */
/* NOTE : espacement entre cartes géré par Bootstrap g-5 (gap: 3rem) */
.campaign-card-modern {
  background: white;
  border-radius: 30px;
  padding: 28px;
  border: 1px solid #eef2f6;
  transition: 0.3s cubic-bezier(0.4,0,0.2,1);
  cursor: default;
}
.campaign-card-modern:hover {
  transform: translateY(-10px);
  border-color: #f59e0b;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08);
}
.campaign-title-modern { font-size: 1rem; color: #0f172a; }
.border-light-pro { border-color: #f1f5f9; }
.score-display-sm { position: relative; width: 60px; height: 60px; flex-shrink: 0; }
.score-display-sm .score-text .num { font-size: 1.1rem; }

/* ELITE BUTTON */
.btn-elite-action {
  display: inline-flex; align-items: center; gap: 15px; text-decoration: none;
  background: #0f172a; padding: 8px 8px 8px 24px; border-radius: 100px;
  color: white; font-weight: 700; font-size: 0.75rem; transition: all 0.3s ease;
}
.icon-box {
  width: 40px; height: 40px; background: rgba(255,255,255,0.1); border-radius: 50%;
  display: flex; align-items: center; justify-content: center; transition: 0.3s;
}
.btn-elite-action:hover { background: #fbbf24; color: #0f172a; transform: scale(1.05); }
.btn-elite-action:hover .icon-box { background: #0f172a; color: white; transform: rotate(-45deg); }
.btn-elite-sm { padding: 5px 5px 5px 14px; font-size: 0.65rem; gap: 8px; }
.icon-box-sm { width: 28px; height: 28px; }

/* BTN ENIGMA */
.btn-enigma-primary { background: #0f172a; color: white; border: none; padding: 14px 28px; border-radius: 18px; font-weight: 800; position: relative; overflow: hidden; cursor: pointer; font-family: inherit; }
.btn-enigma-primary .btn-glow { position: absolute; inset: 0; background: linear-gradient(135deg, #f59e0b, #fbbf24); opacity: 0; transition: 0.3s; z-index: 1; }
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }

/* TOAST */
.enigma-toast { position: fixed; bottom: 30px; right: 30px; background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px; display: flex; align-items: center; gap: 15px; z-index: 3000; border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.2); }
.t-success { border-left-color: #10b981; } .t-error { border-left-color: #f43f5e; } .t-warn { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* MISC */
.text-success { color: #10b981 !important; }
.text-danger  { color: #f43f5e !important; }
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
[data-theme="dark"] .custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(255,255,255,0.08); }

@media (max-width: 768px) {
  .premium-title { font-size: 2.2rem; }
  .border-end-glass { border-right: none; border-bottom: 1px solid rgba(0,0,0,0.05); }
  .history-item-card:hover { transform: none; }
  .btn-elite-action { width: 100%; justify-content: space-between; }
  .search-inline-input { width: 120px; }
}
</style>