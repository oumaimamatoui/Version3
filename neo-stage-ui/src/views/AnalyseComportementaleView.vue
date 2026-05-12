<template>
  <div class="ac-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="ac-bg" aria-hidden="true">
      <div class="ac-orb ac-orb-amber" :style="orbStyle(0.04)"></div>
      <div class="ac-orb ac-orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="ac-orb ac-orb-purple":style="orbStyle(0.025)"></div>
      <div class="ac-grid-dots"></div>
    </div>

    <AppSidebar />

    <div class="ac-main flex-grow-1 d-flex flex-column">
      <AppNavbar />

      <!-- LOADER -->
      <div v-if="loading" class="ac-loader-viewport">
        <div class="ac-spinner-ring">
          <div></div><div></div><div></div><div></div>
        </div>
        <p class="ac-loading-text mt-3">{{ t('analyseComportementale.loading') }}</p>
      </div>

      <!-- MAIN CONTENT -->
      <main v-else class="ac-canvas flex-grow-1 overflow-auto">
        <div class="ac-inner p-4 p-lg-5">

          <!-- HEADER -->
          <header class="ac-page-header mb-5">
            <div class="ac-header-left">
              <div class="ac-breadcrumb mb-2">
                <span class="root">{{ t('analyseComportementale.breadcrumb') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 sep"></i>
                <span class="current">{{ t('analyseComportementale.breadcrumbCurrent') }}</span>
              </div>
              <h2 class="ac-page-title">
                {{ t('analyseComportementale.pageTitle') }}
                <span class="ac-title-accent">{{ t('analyseComportementale.pageTitleAccent') }}</span>
              </h2>
              <p class="ac-subtitle">
                {{ t('analyseComportementale.profilesAnalyzed', analyses.length) }}
                · {{ t('analyseComportementale.avgScore') }}
                <strong class="ac-avg-val">{{ avgScore !== null ? avgScore + '%' : t('analyseComportementale.noScore') }}</strong>
              </p>
            </div>

            <div class="ac-header-right">
              <button class="ac-btn-icon" @click="initOrchestrator" :disabled="loading" :title="t('analyseComportementale.refresh')">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <div class="ac-view-toggle">
                <button :class="['ac-vtoggle', { active: viewMode === 'grid' }]" @click="viewMode = 'grid'" :title="t('analyseComportementale.viewGrid')">
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
                <button :class="['ac-vtoggle', { active: viewMode === 'list' }]" @click="viewMode = 'list'" :title="t('analyseComportementale.viewList')">
                  <i class="fa-solid fa-list-ul"></i>
                </button>
              </div>
            </div>
          </header>

          <!-- KPI CARDS -->
          <div class="ac-kpi-grid mb-5">
            <div class="ac-kpi-card" v-for="stat in kpiStats" :key="stat.label">
              <div class="ac-kpi-icon-wrap" :style="{ background: stat.bg }">
                <i :class="stat.icon" :style="{ color: stat.color }"></i>
              </div>
              <div class="ac-kpi-body">
                <div class="ac-kpi-value">{{ stat.value }}</div>
                <div class="ac-kpi-label">{{ stat.label }}</div>
              </div>
              <div class="ac-kpi-trend" :style="{ '--bar-color': stat.color, '--bar-h': stat.bar + '%' }">
                <div class="ac-kpi-bar"></div>
              </div>
            </div>
          </div>

          <!-- FILTERS -->
          <div class="ac-filters-row mb-4">
            <div class="ac-tabs-wrap">
              <button
                v-for="tab in filterTabs" :key="tab.value"
                :class="['ac-tab', { active: activeTab === tab.value }]"
                @click="activeTab = tab.value"
              >
                {{ tab.label }}
                <span class="ac-tab-count">{{ tab.count }}</span>
              </button>
            </div>

            <div class="ac-filters-right">
              <select class="ac-select" v-model="sortBy">
                <option value="score_desc">{{ t('analyseComportementale.sort.scoreDesc') }}</option>
                <option value="score_asc">{{ t('analyseComportementale.sort.scoreAsc') }}</option>
                <option value="name">{{ t('analyseComportementale.sort.name') }}</option>
                <option value="date_desc">{{ t('analyseComportementale.sort.dateDesc') }}</option>
              </select>
              <div class="ac-search-box">
                <i class="fa-solid fa-magnifying-glass ac-search-icon"></i>
                <input
                  type="text"
                  v-model="searchQuery"
                  :placeholder="t('analyseComportementale.search')"
                  class="ac-search-input"
                />
                <button v-if="searchQuery" class="ac-search-clear" @click="searchQuery = ''">
                  <i class="fa-solid fa-xmark"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- EMPTY STATE -->
          <div v-if="filteredAnalyses.length === 0" class="ac-empty-state">
            <div class="ac-empty-icon">
              <i class="fa-solid fa-user-slash"></i>
            </div>
            <h5>{{ t('analyseComportementale.emptyTitle') }}</h5>
            <p>{{ t('analyseComportementale.emptySubtitle') }}</p>
          </div>

          <!-- GRID VIEW -->
          <div v-else-if="viewMode === 'grid'" class="ac-grid">
            <div
              v-for="(a, idx) in filteredAnalyses"
              :key="a.id"
              class="ac-profile-card"
              :style="{ animationDelay: (idx * 0.045) + 's' }"
              @click="goToDetail(a)"
            >
              <!-- Shimmer hover layer -->
              <div class="ac-card-shimmer" aria-hidden="true"></div>

              <div class="ac-card-top">
                <div class="ac-avatar" :style="{ background: avatarBg(a.candidat_nom) }">
                  {{ initials(a.candidat_nom) }}
                </div>
                <div class="ac-card-badges">
                  <span class="ac-tier-badge" :class="'tier-' + tierKey(a.tier_raw)">
                    <span class="ac-tier-dot"></span>
                    {{ t(`analyseComportementale.tiers.${tierKey(a.tier_raw)}`) }}
                  </span>
                  <button class="ac-delete-btn" @click.stop="handleDelete(a.id)" :title="t('analyseComportementale.delete')">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>

              <div class="ac-card-body">
                <h5 class="ac-candidate-name">{{ a.candidat_nom }}</h5>
                <p class="ac-candidate-profile">{{ a.profile_type_translated }}</p>
              </div>

              <div class="ac-card-score-area">
                <div class="ac-score-row">
                  <span class="ac-score-label">{{ t('analyseComportementale.scoreLabel') }}</span>
                  <span class="ac-score-value mono" :style="{ color: scoreColor(a.global_score) }">
                    {{ a.global_score > 0 ? a.global_score + '%' : 'N/A' }}
                  </span>
                </div>
                <div class="ac-progress-track">
                  <div
                    class="ac-progress-fill"
                    :style="{ width: a.global_score + '%', background: scoreColor(a.global_score) }"
                  ></div>
                </div>
              </div>

              <div class="ac-card-footer">
                <span class="ac-foot-item">
                  <i class="fa-regular fa-calendar"></i>
                  {{ formatDate(a.created_at) }}
                </span>
                <span class="ac-foot-item">
                  <i class="fa-solid fa-file-lines"></i>
                  {{ t('analyseComportementale.tests', a.tests_count) }}
                </span>
              </div>

              <div class="ac-card-cta">
                <i class="fa-solid fa-arrow-right"></i>
                {{ t('analyseComportementale.viewProfile') }}
              </div>
            </div>
          </div>

          <!-- LIST VIEW -->
          <div v-else class="ac-list-container">
            <div class="ac-list-head">
              <div class="col-name">{{ t('analyseComportementale.colCandidate') }}</div>
              <div class="col-profile">{{ t('analyseComportementale.colProfile') }}</div>
              <div class="col-tier">{{ t('analyseComportementale.colTier') }}</div>
              <div class="col-score">{{ t('analyseComportementale.colScore') }}</div>
              <div class="col-date">{{ t('analyseComportementale.colDate') }}</div>
              <div class="col-actions"></div>
            </div>

            <div
              v-for="(a, idx) in filteredAnalyses"
              :key="a.id"
              class="ac-list-row"
              :style="{ animationDelay: (idx * 0.03) + 's' }"
              @click="goToDetail(a)"
            >
              <div class="col-name">
                <div class="ac-list-avatar" :style="{ background: avatarBg(a.candidat_nom) }">
                  {{ initials(a.candidat_nom) }}
                </div>
                <span class="ac-list-name">{{ a.candidat_nom }}</span>
              </div>
              <div class="col-profile">
                <span class="ac-list-profile">{{ a.profile_type_translated }}</span>
              </div>
              <div class="col-tier">
                <span class="ac-tier-badge" :class="'tier-' + tierKey(a.tier_raw)">
                  <span class="ac-tier-dot"></span>
                  {{ t(`analyseComportementale.tiers.${tierKey(a.tier_raw)}`) }}
                </span>
              </div>
              <div class="col-score">
                <div class="ac-list-score-wrap">
                  <div class="ac-list-progress">
                    <div
                      class="ac-list-progress-fill"
                      :style="{ width: a.global_score + '%', background: scoreColor(a.global_score) }"
                    ></div>
                  </div>
                  <span class="ac-list-score-val mono" :style="{ color: scoreColor(a.global_score) }">
                    {{ a.global_score > 0 ? a.global_score + '%' : '—' }}
                  </span>
                </div>
              </div>
              <div class="col-date">
                <span class="ac-foot-item">{{ formatDate(a.created_at) }}</span>
              </div>
              <div class="col-actions">
                <button class="ac-action-btn" @click.stop="goToDetail(a)" :title="t('analyseComportementale.view')">
                  <i class="fa-solid fa-eye"></i>
                </button>
                <button class="ac-action-btn danger" @click.stop="handleDelete(a.id)" :title="t('analyseComportementale.delete')">
                  <i class="fa-solid fa-trash-can"></i>
                </button>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- TOAST -->
    <transition name="ac-toast-anim">
      <div v-if="toast.active" class="ac-toast" :class="'ac-toast-' + toast.type">
        <i :class="toast.icon"></i>
        <span>{{ toast.message }}</span>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar  from '../components/AppNavbar.vue';

const { t }       = useI18n();
const router      = useRouter();
const loading     = ref(true);
const viewMode    = ref('grid');
const searchQuery = ref('');
const activeTab   = ref('all');
const sortBy      = ref('score_desc');
const analyses    = ref([]);
const mouse       = reactive({ x: 0, y: 0 });
const toast       = reactive({ active: false, message: '', type: 'info', icon: '' });

const initOrchestrator = async () => {
  loading.value = true;
  try {
    const { data: candidats } = await api.get('/Candidates');
    if (!Array.isArray(candidats) || candidats.length === 0) { analyses.value = []; return; }
    const results = await Promise.allSettled(candidats.map(enrichCandidat));
    analyses.value = results.filter(r => r.status === 'fulfilled').map(r => r.value);
  } catch (err) {
    showToast(t('analyseComportementale.apiError'), 'error');
  } finally {
    loading.value = false;
  }
};

const enrichCandidat = async (c) => {
  const id = c.id || c.Id;
  let tests = [];
  try {
    const { data } = await api.get(`/Examen/resultats/${id}`);
    if (data) tests = Array.isArray(data) ? data : [data];
  } catch (_) {}

  let globalScore = 0;
  if (tests.length > 0) {
    const scores = tests.map(t => Number(t.scoreGlobal ?? t.scoreTotal ?? 0)).filter(s => s > 0);
    if (scores.length > 0) globalScore = Math.round(scores.reduce((a, b) => a + b, 0) / scores.length);
  } else {
    globalScore = Number(c.global_score ?? 0);
  }

  const profileTypeRaw = c.group || c.poste || detectProfileKey(globalScore, tests);
  return {
    id,
    candidat_nom:             c.name || c.nom || 'Candidat',
    tier_raw:                 globalScore >= 85 ? 'elite' : globalScore >= 70 ? 'standard' : 'basique',
    profile_type_translated:  t(`analyseComportementale.profiles.${profileTypeRaw}`, profileTypeRaw),
    global_score:             globalScore,
    created_at:               c.created_at || new Date().toISOString(),
    tests_count:              tests.length,
  };
};

const detectProfileKey = (score, tests) => {
  if (tests.length === 0) return 'toEvaluate';
  return score >= 85 ? 'expert' : score >= 70 ? 'confirmed' : 'junior';
};

const handleDelete = async (id) => {
  if (!confirm(t('analyseComportementale.deleteConfirm'))) return;
  try {
    await api.delete(`/Candidates/${id}`);
    analyses.value = analyses.value.filter(a => a.id !== id);
    showToast(t('analyseComportementale.deleteSuccess'), 'success');
  } catch (_) {
    showToast(t('analyseComportementale.apiError'), 'error');
  }
};

const goToDetail = (a) => router.push({ path: `/analyse-comportementale/${a.id}` });

const filteredAnalyses = computed(() => {
  let list = [...analyses.value];
  if (activeTab.value === 'noeval') list = list.filter(a => a.global_score === 0);
  else if (activeTab.value !== 'all') list = list.filter(a => a.tier_raw === activeTab.value);
  const q = searchQuery.value.toLowerCase().trim();
  if (q) list = list.filter(a => a.candidat_nom.toLowerCase().includes(q));
  if (sortBy.value === 'score_desc') list.sort((a, b) => b.global_score - a.global_score);
  if (sortBy.value === 'score_asc')  list.sort((a, b) => a.global_score - b.global_score);
  if (sortBy.value === 'name')       list.sort((a, b) => a.candidat_nom.localeCompare(b.candidat_nom));
  if (sortBy.value === 'date_desc')  list.sort((a, b) => new Date(b.created_at) - new Date(a.created_at));
  return list;
});

const filterTabs = computed(() => [
  { label: t('analyseComportementale.tabs.all'),      value: 'all',      count: analyses.value.length },
  { label: t('analyseComportementale.tabs.elite'),    value: 'elite',    count: analyses.value.filter(a => a.tier_raw === 'elite').length },
  { label: t('analyseComportementale.tabs.standard'), value: 'standard', count: analyses.value.filter(a => a.tier_raw === 'standard').length },
  { label: t('analyseComportementale.tabs.basique'),  value: 'basique',  count: analyses.value.filter(a => a.tier_raw === 'basique').length },
  { label: t('analyseComportementale.tabs.noeval'),   value: 'noeval',   count: analyses.value.filter(a => a.global_score === 0).length },
]);

const avgScore = computed(() => {
  const avecScore = analyses.value.filter(a => a.global_score > 0);
  return avecScore.length ? Math.round(avecScore.reduce((s, a) => s + a.global_score, 0) / avecScore.length) : null;
});

const kpiStats = computed(() => [
  { label: t('analyseComportementale.kpi.totalProfiles'), value: analyses.value.length,                                    icon: 'fa-solid fa-users',          color: '#f59e0b', bg: '#fffbeb', bar: 70 },
  { label: t('analyseComportementale.kpi.avgScore'),      value: avgScore.value ? avgScore.value + '%' : '—',              icon: 'fa-solid fa-chart-line',     color: '#3b82f6', bg: '#eff6ff', bar: avgScore.value || 0 },
  { label: t('analyseComportementale.kpi.tierElite'),     value: analyses.value.filter(a => a.tier_raw === 'elite').length, icon: 'fa-solid fa-crown',          color: '#8b5cf6', bg: '#f5f3ff', bar: 40 },
  { label: t('analyseComportementale.kpi.noEval'),        value: analyses.value.filter(a => a.global_score === 0).length,  icon: 'fa-solid fa-hourglass-half', color: '#f43f5e', bg: '#fff1f2', bar: 20 },
]);

const tierKey    = (v) => v;
const scoreColor = (s) => (s >= 85 ? '#10b981' : s >= 70 ? '#f59e0b' : '#f43f5e');
const initials   = (n) => n.split(' ').map(p => p[0]).join('').toUpperCase().slice(0, 2);
const formatDate = (d) => new Date(d).toLocaleDateString(undefined, { day: '2-digit', month: 'short' });

const avatarBg = (name) => {
  const palettes = [
    'rgba(245,158,11,0.12)',
    'rgba(99,102,241,0.12)',
    'rgba(16,185,129,0.12)',
    'rgba(244,63,94,0.10)',
    'rgba(59,130,246,0.12)',
  ];
  return palettes[name.length % palettes.length];
};

const orbStyle       = (f) => ({ transform: `translate(${mouse.x * f * 10}px, ${mouse.y * f * 10}px)` });
const handleParallax = (e) => {
  mouse.x = (e.clientX - window.innerWidth / 2) / 30;
  mouse.y = (e.clientY - window.innerHeight / 2) / 30;
};

const showToast = (message, type) => {
  const icons = { success: 'fa-solid fa-circle-check', error: 'fa-solid fa-circle-xmark', warn: 'fa-solid fa-triangle-exclamation' };
  Object.assign(toast, { active: true, message, type, icon: icons[type] || icons.success });
  setTimeout(() => (toast.active = false), 3000);
};

onMounted(initOrchestrator);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;0,900;1,700&family=JetBrains+Mono:wght@600;800&display=swap');

/* ══════════════════════════════════════
   ROOT & TOKENS  (cohérent Home.vue)
══════════════════════════════════════ */
.ac-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: #f8fafc;
  color: #0f172a;
  display: flex;
  position: relative;
  overflow-x: hidden;
}
[data-theme="dark"] .ac-root { background: #0d1117; color: #f0f6fc; }

/* ══ BACKGROUND (identique Home/Tarification) ══ */
.ac-bg {
  position: fixed; inset: 0; z-index: 0;
  pointer-events: none; overflow: hidden;
}
.ac-grid-dots {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(234,179,8,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(234,179,8,0.04) 1px, transparent 1px);
  background-size: 60px 60px;
}
[data-theme="dark"] .ac-grid-dots {
  background-image:
    linear-gradient(rgba(255,255,255,0.025) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.025) 1px, transparent 1px);
}
.ac-orb {
  position: absolute; border-radius: 50%;
  filter: blur(120px); opacity: 0.14;
  transition: transform 0.35s ease-out;
}
.ac-orb-amber  { width: 650px; height: 650px; background: #f59e0b; top: -200px; right: -180px; }
.ac-orb-blue   { width: 500px; height: 500px; background: #6366f1; bottom: -200px; left: -120px; }
.ac-orb-purple { width: 380px; height: 380px; background: #8b5cf6; top: 40%; left: 30%; opacity: 0.07; }

/* ══ LAYOUT ══ */
.ac-main   { z-index: 5; min-width: 0; }
.ac-canvas { height: calc(100vh - 64px); }
.ac-inner  { max-width: 1400px; margin: 0 auto; }

/* ══ LOADER ══ */
.ac-loader-viewport {
  flex: 1; display: flex; flex-direction: column;
  align-items: center; justify-content: center; gap: 20px; z-index: 5;
}
/* Spinner identique Home.vue preloader */
.ac-spinner-ring {
  display: inline-block; position: relative; width: 58px; height: 58px;
}
.ac-spinner-ring div {
  box-sizing: border-box; display: block; position: absolute;
  width: 46px; height: 46px; margin: 6px;
  border: 4px solid transparent;
  border-top-color: #eab308;
  border-radius: 50%;
  animation: ac-ring-spin 1.2s cubic-bezier(0.5,0,0.5,1) infinite;
}
.ac-spinner-ring div:nth-child(1) { animation-delay: -0.45s; }
.ac-spinner-ring div:nth-child(2) { animation-delay: -0.30s; border-top-color: rgba(234,179,8,0.5); }
.ac-spinner-ring div:nth-child(3) { animation-delay: -0.15s; border-top-color: rgba(234,179,8,0.25); }
@keyframes ac-ring-spin { 0%{transform:rotate(0)} 100%{transform:rotate(360deg)} }

.ac-loading-text {
  font-size: 11px; font-weight: 800; color: #94a3b8;
  letter-spacing: 2.5px; text-transform: uppercase;
}

/* ══ PAGE HEADER ══ */
.ac-page-header {
  display: flex; justify-content: space-between;
  align-items: flex-end; gap: 16px; flex-wrap: wrap;
}
.ac-header-right { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }

/* Breadcrumb */
.ac-breadcrumb { font-size: 0.72rem; font-weight: 700; color: #94a3b8; display: flex; align-items: center; }
.ac-breadcrumb .root { transition: color 0.2s; }
.ac-breadcrumb .root:hover { color: #eab308; cursor: pointer; }
.ac-breadcrumb .sep { font-size: 0.5rem; opacity: 0.4; }
.ac-breadcrumb .current { color: #0f172a; font-weight: 800; }
[data-theme="dark"] .ac-breadcrumb .current { color: #f0f6fc; }

/* Title — style identique Home.vue hero-title */
.ac-page-title {
  font-size: 2.3rem; font-weight: 800;
  letter-spacing: -1.5px; line-height: 1.1;
  color: #0f172a; margin: 0;
}
[data-theme="dark"] .ac-page-title { color: #f0f6fc; }

/* Accent amber italic — identique hero-highlight */
.ac-title-accent {
  color: #eab308;
  font-style: italic;
}

.ac-subtitle {
  color: #64748b; font-size: 14px; margin-top: 8px; margin-bottom: 0;
}
.ac-avg-val { color: #0f172a; font-weight: 800; }
[data-theme="dark"] .ac-avg-val { color: #f0f6fc; }

/* ══ ICON BUTTON (refresh) ══ */
.ac-btn-icon {
  width: 44px; height: 44px;
  background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 15px; transition: all 0.3s;
}
.ac-btn-icon:hover:not(:disabled) {
  border-color: #eab308; color: #eab308;
  transform: rotate(180deg) scale(1.08);
  box-shadow: 0 4px 16px rgba(234,179,8,0.2);
}
.ac-btn-icon:disabled { opacity: 0.4; cursor: not-allowed; }
[data-theme="dark"] .ac-btn-icon {
  background: #161b22; border-color: rgba(255,255,255,0.08); color: #8b949e;
}

/* ══ VIEW TOGGLE ══ */
.ac-view-toggle {
  display: flex; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  padding: 4px; gap: 4px;
}
[data-theme="dark"] .ac-view-toggle {
  background: #161b22; border-color: rgba(255,255,255,0.08);
}
.ac-vtoggle {
  width: 38px; height: 38px; border-radius: 10px;
  border: none; background: transparent;
  color: #94a3b8; cursor: pointer; font-size: 14px;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.ac-vtoggle.active {
  background: #eab308; color: #0f172a;
  box-shadow: 0 2px 8px rgba(234,179,8,0.3);
}
[data-theme="dark"] .ac-vtoggle.active { background: #eab308; color: #0f172a; }

/* ══ KPI CARDS — style identique pricing-card ══ */
.ac-kpi-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 18px;
}
.ac-kpi-card {
  background: white;
  border: 1px solid #e8edf5;
  border-radius: 24px; padding: 22px 24px;
  display: flex; align-items: center; gap: 16px;
  transition: transform 0.3s cubic-bezier(0.175,0.885,0.32,1.275), box-shadow 0.3s;
  box-shadow: 0 2px 10px rgba(0,0,0,0.03);
  position: relative; overflow: hidden;
}
.ac-kpi-card::before {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(135deg, rgba(234,179,8,0.03) 0%, transparent 60%);
  opacity: 0; transition: opacity 0.3s;
}
.ac-kpi-card:hover { transform: translateY(-6px); box-shadow: 0 20px 50px rgba(0,0,0,0.08); }
.ac-kpi-card:hover::before { opacity: 1; }
[data-theme="dark"] .ac-kpi-card {
  background: #161b22; border-color: rgba(255,255,255,0.07);
}
[data-theme="dark"] .ac-kpi-card:hover { box-shadow: 0 20px 50px rgba(0,0,0,0.4); }

.ac-kpi-icon-wrap {
  width: 56px; height: 56px; border-radius: 18px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.2rem;
}
.ac-kpi-body { flex: 1; min-width: 0; }
.ac-kpi-value {
  font-family: 'JetBrains Mono', monospace;
  font-size: 1.75rem; font-weight: 800;
  color: #0f172a; line-height: 1; letter-spacing: -1px;
}
[data-theme="dark"] .ac-kpi-value { color: #f0f6fc; }
.ac-kpi-label {
  font-size: 0.62rem; font-weight: 800; color: #94a3b8;
  text-transform: uppercase; letter-spacing: 1.2px; margin-top: 6px;
}

/* Mini trend bar */
.ac-kpi-trend {
  width: 34px; height: 34px; border-radius: 10px; flex-shrink: 0;
  background: color-mix(in srgb, var(--bar-color) 10%, transparent);
  display: flex; align-items: flex-end; justify-content: center;
  padding: 6px;
}
.ac-kpi-bar {
  width: 10px; border-radius: 4px; min-height: 4px;
  height: var(--bar-h);
  background: var(--bar-color);
  transition: height 1s ease;
}

/* ══ FILTERS ROW ══ */
.ac-filters-row {
  display: flex; justify-content: space-between;
  align-items: center; gap: 12px; flex-wrap: wrap;
}
.ac-filters-right { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }

/* Tabs — style section-tag de Home.vue */
.ac-tabs-wrap {
  display: flex; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 18px;
  padding: 4px; gap: 4px; flex-wrap: wrap;
}
[data-theme="dark"] .ac-tabs-wrap {
  background: #161b22; border-color: rgba(255,255,255,0.08);
}
.ac-tab {
  padding: 8px 16px; border-radius: 13px; border: none;
  background: transparent; color: #64748b;
  font-family: inherit; font-weight: 700; font-size: 0.8rem;
  cursor: pointer; transition: all 0.2s; white-space: nowrap;
}
.ac-tab:hover:not(.active) { color: #0f172a; background: #fefce8; }
.ac-tab.active {
  background: #0f172a; color: white;
  box-shadow: 0 2px 8px rgba(15,23,42,0.15);
}
[data-theme="dark"] .ac-tab.active { background: #eab308; color: #0f172a; }
[data-theme="dark"] .ac-tab:hover:not(.active) { color: #f0f6fc; background: rgba(255,255,255,0.06); }

.ac-tab-count {
  display: inline-block;
  background: #f1f5f9; color: #94a3b8;
  border-radius: 6px; padding: 1px 7px;
  font-size: 0.65rem; margin-left: 5px; font-weight: 800;
}
.ac-tab.active .ac-tab-count {
  background: rgba(255,255,255,0.15); color: white;
}
[data-theme="dark"] .ac-tab.active .ac-tab-count {
  background: rgba(0,0,0,0.15); color: #0f172a;
}

/* Select */
.ac-select {
  height: 44px; padding: 0 16px; border-radius: 14px;
  border: 1.5px solid #e2e8f0; background: white; color: #0f172a;
  font-family: inherit; font-size: 0.83rem; font-weight: 600;
  cursor: pointer; outline: none; transition: border-color 0.2s;
}
.ac-select:focus { border-color: #eab308; box-shadow: 0 0 0 3px rgba(234,179,8,0.1); }
[data-theme="dark"] .ac-select {
  background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc;
}

/* Search box */
.ac-search-box {
  display: flex; align-items: center; gap: 8px;
  background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; padding: 0 14px; height: 44px;
  transition: border-color 0.2s, box-shadow 0.2s;
}
.ac-search-box:focus-within {
  border-color: #eab308;
  box-shadow: 0 0 0 3px rgba(234,179,8,0.1);
}
[data-theme="dark"] .ac-search-box {
  background: #161b22; border-color: rgba(255,255,255,0.08);
}
.ac-search-icon { color: #94a3b8; font-size: 13px; flex-shrink: 0; }
.ac-search-input {
  border: none; outline: none; background: transparent;
  color: #0f172a; font-family: inherit; font-size: 0.85rem; font-weight: 600;
  width: 200px;
}
[data-theme="dark"] .ac-search-input { color: #f0f6fc; }
.ac-search-input::placeholder { color: #cbd5e1; font-weight: 400; }
.ac-search-clear {
  border: none; background: none; cursor: pointer; padding: 0;
  color: #cbd5e1; font-size: 12px; transition: color 0.2s;
  display: flex; align-items: center;
}
.ac-search-clear:hover { color: #f43f5e; }

/* ══ EMPTY STATE ══ */
.ac-empty-state {
  text-align: center; padding: 80px 20px; color: #94a3b8;
}
.ac-empty-icon {
  width: 76px; height: 76px; border-radius: 22px;
  background: #f8fafc; border: 1px solid #e8edf5;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.7rem; color: #cbd5e1; margin: 0 auto 20px;
}
.ac-empty-state h5 {
  font-size: 1.1rem; font-weight: 800; color: #0f172a; margin-bottom: 8px;
}
[data-theme="dark"] .ac-empty-state h5 { color: #f0f6fc; }
.ac-empty-state p { font-size: 14px; color: #94a3b8; }

/* ══ GRID LAYOUT ══ */
.ac-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(290px, 1fr));
  gap: 20px;
}

/* ══ PROFILE CARD — identique pricing-card style ══ */
.ac-profile-card {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(20px);
  border: 1px solid #e8edf5;
  border-radius: 28px; padding: 26px;
  cursor: pointer; position: relative; overflow: hidden;
  display: flex; flex-direction: column;
  transition: transform 0.3s cubic-bezier(0.175,0.885,0.32,1.275), box-shadow 0.3s, border-color 0.3s;
  box-shadow: 0 2px 10px rgba(0,0,0,0.03);
  animation: ac-card-in 0.45s ease both;
}
[data-theme="dark"] .ac-profile-card {
  background: rgba(22,27,34,0.92); border-color: rgba(255,255,255,0.07);
}
@keyframes ac-card-in {
  from { opacity: 0; transform: translateY(22px); }
  to   { opacity: 1; transform: translateY(0); }
}
.ac-profile-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 24px 60px rgba(0,0,0,0.09);
  border-color: #eab308;
}
[data-theme="dark"] .ac-profile-card:hover {
  box-shadow: 0 24px 60px rgba(0,0,0,0.45);
}
.ac-profile-card:hover .ac-card-cta    { opacity: 1; transform: translateY(0); }
.ac-profile-card:hover .ac-delete-btn  { opacity: 1; }
.ac-profile-card:hover .ac-card-shimmer { opacity: 1; }

/* Shimmer layer */
.ac-card-shimmer {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, rgba(234,179,8,0.04) 0%, transparent 60%);
  opacity: 0; transition: opacity 0.35s; pointer-events: none;
  border-radius: inherit;
}

/* Card top */
.ac-card-top {
  display: flex; justify-content: space-between;
  align-items: flex-start; margin-bottom: 18px;
}
.ac-avatar {
  width: 52px; height: 52px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.88rem; font-weight: 900; color: #0f172a;
  letter-spacing: 0.5px; flex-shrink: 0;
  border: 1.5px solid rgba(0,0,0,0.05);
}
[data-theme="dark"] .ac-avatar { color: #f0f6fc; border-color: rgba(255,255,255,0.1); }

.ac-card-badges { display: flex; align-items: center; gap: 8px; }

/* Tier badge — identique plan-badge */
.ac-tier-badge {
  font-size: 0.6rem; font-weight: 800; padding: 4px 10px;
  border-radius: 8px; text-transform: uppercase; letter-spacing: 0.8px;
  display: inline-flex; align-items: center; gap: 5px; white-space: nowrap;
}
.ac-tier-dot { width: 5px; height: 5px; border-radius: 50%; background: currentColor; flex-shrink: 0; }
.tier-elite    { background: #ecfdf5; color: #059669; }
.tier-standard { background: #eef2ff; color: #6366f1; }
.tier-basique  { background: #f8fafc; color: #64748b; border: 1px solid #e2e8f0; }
[data-theme="dark"] .tier-elite    { background: rgba(16,185,129,0.15); color: #34d399; }
[data-theme="dark"] .tier-standard { background: rgba(99,102,241,0.15); color: #818cf8; }
[data-theme="dark"] .tier-basique  { background: rgba(255,255,255,0.06); color: #8b949e; border-color: rgba(255,255,255,0.08); }

/* Delete button */
.ac-delete-btn {
  width: 30px; height: 30px; border-radius: 9px;
  border: 1px solid #eef2f6; background: white;
  color: #94a3b8; cursor: pointer; font-size: 0.72rem;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s; opacity: 0;
}
[data-theme="dark"] .ac-delete-btn {
  background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08);
}
.ac-delete-btn:hover { background: #fff1f2; color: #e11d48; border-color: #fecdd3; transform: scale(1.1); }
[data-theme="dark"] .ac-delete-btn:hover {
  background: rgba(244,63,94,0.15); color: #f43f5e; border-color: rgba(244,63,94,0.3);
}

/* Card body */
.ac-card-body { margin-bottom: 20px; flex: 1; }
.ac-candidate-name {
  font-size: 1rem; font-weight: 800; color: #0f172a; margin: 0 0 5px;
}
[data-theme="dark"] .ac-candidate-name { color: #f0f6fc; }
.ac-candidate-profile {
  font-size: 0.78rem; color: #94a3b8; margin: 0;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

/* Score area */
.ac-card-score-area { margin-bottom: 18px; }
.ac-score-row {
  display: flex; justify-content: space-between;
  align-items: center; margin-bottom: 8px;
}
.ac-score-label {
  font-size: 0.67rem; font-weight: 800; color: #94a3b8;
  text-transform: uppercase; letter-spacing: 1.2px;
}
.ac-score-value { font-size: 0.95rem; font-weight: 800; }
.mono { font-family: 'JetBrains Mono', monospace; }

.ac-progress-track {
  height: 7px; background: #f1f5f9; border-radius: 100px; overflow: hidden;
}
[data-theme="dark"] .ac-progress-track { background: rgba(255,255,255,0.08); }
.ac-progress-fill {
  height: 100%; border-radius: 100px;
  transition: width 1.2s cubic-bezier(0.4,0,0.2,1);
}

/* Card footer */
.ac-card-footer {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 16px; border-top: 1px solid #f1f5f9;
}
[data-theme="dark"] .ac-card-footer { border-color: rgba(255,255,255,0.06); }
.ac-foot-item {
  font-size: 0.7rem; color: #94a3b8; font-weight: 600;
  display: flex; align-items: center; gap: 5px;
}

/* CTA overlay — identique btn-plan-amber style */
.ac-card-cta {
  position: absolute; bottom: 0; left: 0; right: 0;
  height: 48px; background: #0f172a; color: #fbbf24;
  font-weight: 800; font-size: 0.8rem;
  display: flex; align-items: center; justify-content: center; gap: 8px;
  border-radius: 0 0 27px 27px;
  opacity: 0; transform: translateY(6px);
  transition: all 0.25s cubic-bezier(0.4,0,0.2,1);
}
[data-theme="dark"] .ac-card-cta { background: #eab308; color: #0f172a; }

/* ══ LIST VIEW ══ */
.ac-list-container { display: flex; flex-direction: column; gap: 8px; }

.ac-list-head {
  display: grid;
  grid-template-columns: 2fr 2fr 1.2fr 2fr 1fr 90px;
  gap: 12px; padding: 12px 22px;
  background: #f8fafc; border-radius: 16px;
  border: 1px solid #e8edf5;
  font-size: 0.66rem; font-weight: 900; color: #94a3b8;
  text-transform: uppercase; letter-spacing: 1.5px;
}
[data-theme="dark"] .ac-list-head {
  background: rgba(22,27,34,0.9); border-color: rgba(255,255,255,0.07);
}

.ac-list-row {
  display: grid;
  grid-template-columns: 2fr 2fr 1.2fr 2fr 1fr 90px;
  gap: 12px; padding: 16px 22px;
  background: white; border: 1px solid #e8edf5;
  border-radius: 20px; cursor: pointer; align-items: center;
  transition: all 0.22s cubic-bezier(0.4,0,0.2,1);
  box-shadow: 0 1px 6px rgba(0,0,0,0.02);
  animation: ac-card-in 0.38s ease both;
}
[data-theme="dark"] .ac-list-row {
  background: rgba(22,27,34,0.9); border-color: rgba(255,255,255,0.07);
}
.ac-list-row:hover {
  border-color: #eab308; transform: translateX(5px);
  box-shadow: 0 8px 28px rgba(0,0,0,0.06);
}
[data-theme="dark"] .ac-list-row:hover { box-shadow: 0 8px 28px rgba(0,0,0,0.35); }

/* List columns */
.col-name    { display: flex; align-items: center; gap: 11px; min-width: 0; }
.col-profile { display: flex; align-items: center; min-width: 0; }
.col-tier    { display: flex; align-items: center; }
.col-score   { display: flex; align-items: center; }
.col-date    { display: flex; align-items: center; }
.col-actions { display: flex; align-items: center; gap: 6px; justify-content: flex-end; }

.ac-list-avatar {
  width: 36px; height: 36px; border-radius: 12px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.72rem; font-weight: 900; color: #0f172a;
  border: 1.5px solid rgba(0,0,0,0.05);
}
[data-theme="dark"] .ac-list-avatar { color: #f0f6fc; border-color: rgba(255,255,255,0.08); }
.ac-list-name {
  font-weight: 800; font-size: 0.88rem; color: #0f172a;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}
[data-theme="dark"] .ac-list-name { color: #f0f6fc; }
.ac-list-profile {
  font-size: 0.78rem; color: #94a3b8;
  white-space: nowrap; overflow: hidden; text-overflow: ellipsis;
}

.ac-list-score-wrap { display: flex; align-items: center; gap: 10px; width: 100%; }
.ac-list-progress {
  flex: 1; height: 6px; background: #f1f5f9; border-radius: 100px; overflow: hidden;
}
[data-theme="dark"] .ac-list-progress { background: rgba(255,255,255,0.08); }
.ac-list-progress-fill {
  height: 100%; border-radius: 100px; transition: width 1s ease;
}
.ac-list-score-val {
  font-size: 0.82rem; font-weight: 800; min-width: 40px; text-align: right;
}

/* Action buttons */
.ac-action-btn {
  width: 38px; height: 38px; border-radius: 12px;
  border: 1px solid #e8edf5; background: white;
  color: #64748b; cursor: pointer; font-size: 13px;
  display: inline-flex; align-items: center; justify-content: center;
  transition: all 0.22s;
}
[data-theme="dark"] .ac-action-btn {
  background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e;
}
.ac-action-btn:hover {
  background: #0f172a; color: #fbbf24;
  border-color: #0f172a; transform: scale(1.08);
}
[data-theme="dark"] .ac-action-btn:hover {
  background: #eab308; color: #0f172a; border-color: #eab308;
}
.ac-action-btn.danger:hover {
  background: #fff1f2; color: #e11d48; border-color: #fecdd3; transform: scale(1.08);
}
[data-theme="dark"] .ac-action-btn.danger:hover {
  background: rgba(244,63,94,0.15); color: #f43f5e; border-color: rgba(244,63,94,0.3);
}

/* ══ TOAST ══ */
.ac-toast {
  position: fixed; bottom: 28px; right: 28px; z-index: 99999;
  display: flex; align-items: center; gap: 10px;
  padding: 14px 22px; border-radius: 18px;
  background: #0f172a; color: white;
  font-weight: 700; font-size: 0.85rem;
  box-shadow: 0 20px 60px rgba(0,0,0,0.25);
  border: 1px solid rgba(255,255,255,0.07);
}
[data-theme="dark"] .ac-toast { background: #21262d; }
.ac-toast-error   { border-left: 4px solid #f43f5e; }
.ac-toast-warn    { border-left: 4px solid #eab308; }
.ac-toast-success { border-left: 4px solid #10b981; }
.ac-toast-info    { border-left: 4px solid #3b82f6; }

.ac-toast-anim-enter-active { animation: ac-toast-in 0.35s cubic-bezier(0.175,0.885,0.32,1.275); }
.ac-toast-anim-leave-active { animation: ac-toast-in 0.25s ease reverse; }
@keyframes ac-toast-in {
  from { transform: translateX(110%); opacity: 0; }
  to   { transform: translateX(0);    opacity: 1; }
}

/* ══ RESPONSIVE ══ */
@media (max-width: 1200px) {
  .ac-kpi-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 900px) {
  .ac-list-head, .ac-list-row { grid-template-columns: 1fr 1fr 90px; }
  .col-profile, .col-date { display: none; }
  .ac-kpi-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 600px) {
  .ac-filters-row  { flex-direction: column; align-items: stretch; }
  .ac-filters-right { flex-direction: column; }
  .ac-search-box   { width: 100%; }
  .ac-search-input { width: 100%; flex: 1; }
  .ac-grid         { grid-template-columns: 1fr; }
  .ac-page-title   { font-size: 1.7rem; }
  .ac-kpi-grid     { grid-template-columns: 1fr 1fr; }
}
@media (max-width: 420px) {
  .ac-kpi-grid { grid-template-columns: 1fr; }
}
</style>