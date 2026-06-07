<template>
  <div class="admin-body d-flex" :data-theme="isDark ? 'dark' : 'light'">

    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-indigo"></div>
    <div class="tech-grid-subtle"></div>

    <AppSidebar />

    <div class="content flex-grow-1 position-relative">
      <AppNavbar />

      <!-- THEME TOGGLE -->
      <button class="theme-toggle-btn" @click="toggleTheme" :title="isDark ? 'Mode Clair' : 'Mode Sombre'">
        <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
      </button>

      <transition name="fade">
        <div v-if="isLoading" class="loader-overlay">
          <div class="tech-loader-container">
            <div class="tech-spinner"></div>
            <div class="loader-text">SYNCHRONISATION NEURALE...</div>
            <div class="loader-sub">Analyse des patterns comportementaux</div>
          </div>
        </div>
      </transition>

      <main v-if="!isLoading" class="p-4 main-viewport animate-fade-in">

        <!-- HEADER -->
        <div class="page-header mb-4">
          <div class="d-flex justify-content-between align-items-end w-100 flex-wrap gap-3">
            <div>
              <nav class="breadcrumb-cyber mb-2">
                <span class="bc-root">INTELLIGENCE</span>
                <span class="bc-sep">/</span>
                <span class="bc-current">ANALYSES_COMPORTEMENTALES</span>
              </nav>
              <h2 class="main-title m-0">Analyses <span class="title-accent">IA</span></h2>
              <p class="page-sub">Résultats comportementaux · <strong>{{ today }}</strong></p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <div class="system-status">
                <span class="status-dot pulse"></span>
                <span class="status-text">NEURAL ENGINE : ACTIF</span>
              </div>
              <button @click="fetchAnalyses" class="btn-cyber-refresh" :class="{ spinning: isRefreshing }">
                <i class="fa-solid fa-rotate"></i>
              </button>
            </div>
          </div>
        </div>

        <!-- KPI CARDS -->
        <div class="row g-4 mb-4">
          <div class="col-xl-3 col-md-6" v-for="s in kpiStats" :key="s.label">
            <div class="stat-card-premium">
              <div class="stat-icon-wrapper" :style="{ background: s.bg, color: s.color }">
                <i :class="s.icon"></i>
              </div>
              <div class="stat-details ms-3">
                <div class="stat-value">{{ s.val }}</div>
                <div class="stat-label">{{ s.label }}</div>
              </div>
              <span class="trend-badge ms-auto" :class="s.trendUp ? 'up' : 'down'">
                <i :class="s.trendUp ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                {{ s.trend }}
              </span>
            </div>
          </div>
        </div>

        <!-- FILTER BAR -->
        <div class="glass-surface filter-bar-glass mb-4">
          <div class="filter-bar-inner">
            <div class="search-cyber-box">
              <i class="fa-solid fa-magnifying-glass"></i>
              <input v-model="searchQuery" type="text" placeholder="Rechercher candidat, profil, score...">
              <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>
            <div class="filter-cyber-controls">
              <select v-model="profileFilter" class="cyber-select">
                <option value="">Tous les profils</option>
                <option value="Analytique">Analytique</option>
                <option value="Créatif">Créatif</option>
                <option value="Leader">Leader</option>
                <option value="Opérationnel">Opérationnel</option>
              </select>
              <select v-model="tierFilter" class="cyber-select">
                <option value="">Tous les tiers</option>
                <option value="Élite">Élite</option>
                <option value="Senior">Senior</option>
                <option value="Junior">Junior</option>
              </select>
              <select v-model="sortBy" class="cyber-select">
                <option value="score_desc">Score ↓</option>
                <option value="score_asc">Score ↑</option>
                <option value="date_desc">Date ↓</option>
                <option value="name_asc">Nom A-Z</option>
              </select>
            </div>
          </div>
        </div>

        <!-- ROW : Gauge + Radar + Traits (top candidat) -->
        <div v-if="topAnalysis" class="row g-4 mb-4">
          <!-- GAUGE -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 text-center h-100 gauge-card">
              <h6 class="label-heading mb-3">Indice de Fiabilité — Top Candidat</h6>
              <div class="neural-gauge-wrapper mx-auto mb-3">
                <div class="deco-ring ring-1"></div>
                <div class="deco-ring ring-2"></div>
                <svg viewBox="0 0 120 120" class="neural-svg">
                  <circle class="gauge-bg" cx="60" cy="60" r="50"/>
                  <circle class="gauge-arc" cx="60" cy="60" r="50"
                    :style="{ strokeDasharray: (topAnalysis.global_score * 3.14) + ', 314' }"/>
                </svg>
                <div class="gauge-content">
                  <div class="gauge-value-wrap">
                    <span class="gauge-value">{{ topAnalysis.global_score }}</span>
                    <span class="gauge-percent">%</span>
                  </div>
                  <span class="gauge-label">NEURAL INDEX</span>
                  <span class="gauge-tier">{{ topAnalysis.neural_tier }}</span>
                </div>
              </div>
              <div class="badge-profile">
                <span class="badge-dot"></span> {{ topAnalysis.nom }} — {{ topAnalysis.profile_type }}
              </div>
            </div>
          </div>

          <!-- RADAR -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 h-100">
              <h6 class="label-heading mb-3">Carte Cognitive</h6>
              <div class="radar-container">
                <svg viewBox="0 0 300 280" class="radar-svg">
                  <polygon v-for="n in 4" :key="n" :points="getRadarRing(n * 25, topAnalysis.radar_data)" fill="none" stroke="rgba(148,163,184,0.15)"/>
                  <line v-for="(axis, i) in getRadarAxes(topAnalysis.radar_data)" :key="i"
                    :x1="150" :y1="140" :x2="axis.x" :y2="axis.y" stroke="rgba(148,163,184,0.2)"/>
                  <polygon :points="getRadarDataPoints(topAnalysis.radar_data)"
                    fill="rgba(79,70,229,0.2)" stroke="#4f46e5" stroke-width="2"/>
                  <text v-for="(axis, i) in getRadarAxes(topAnalysis.radar_data)" :key="'l'+i"
                    :x="axis.lx" :y="axis.ly" text-anchor="middle" class="radar-label">{{ axis.label }}</text>
                </svg>
              </div>
            </div>
          </div>

          <!-- TRAITS -->
          <div class="col-lg-4">
            <div class="glass-surface p-4 h-100">
              <h6 class="label-heading mb-3">Dimensions Clés</h6>
              <div class="traits-list">
                <div v-for="trait in topAnalysis.traits" :key="trait.name" class="trait-row">
                  <div class="trait-row-left">
                    <div class="trait-icon-wrap" :style="{ color: trait.color, background: trait.color + '20' }">
                      <i :class="trait.icon"></i>
                    </div>
                    <span class="trait-name">{{ trait.name }}</span>
                  </div>
                  <div class="trait-row-right">
                    <span class="score-val" :style="{ color: trait.color }">{{ trait.val }}<small>%</small></span>
                    <div class="progress-wrap">
                      <div class="progress-cyber">
                        <div class="progress-fill" :style="{ width: trait.val + '%', background: trait.color }">
                          <div class="progress-shine"></div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- ═══════════════════════════════════════════════════════
             RECYCLE VIEW — Liste virtualisée des analyses
        ═══════════════════════════════════════════════════════ -->
        <div class="glass-surface p-0 overflow-hidden mb-4">
          <div class="card-inner-glow amber"></div>

          <div class="table-header-cyber d-flex justify-content-between align-items-center p-4">
            <div class="d-flex align-items-center gap-3">
              <div class="table-icon-box">
                <i class="fa-solid fa-brain"></i>
              </div>
              <div>
                <h6 class="label-heading m-0">Registre des Analyses Comportementales</h6>
                <p class="m-0" style="font-size:10px;color:#94a3b8;margin-top:2px!important">
                  Vue recyclée · {{ filteredAnalyses.length }} entrées · Scroll virtuel
                </p>
              </div>
            </div>
            <div class="d-flex gap-2 align-items-center">
              <span class="vectors-badge">RECYCLE VIEW</span>
              <input v-model="recycleSearch" type="text" placeholder="Filtrer..." class="recycle-search-input">
            </div>
          </div>

          <!-- Header colonnes -->
          <div class="recycle-header-row d-flex align-items-center px-4 py-2">
            <span style="width:36px" class="list-col-label">#</span>
            <span style="width:42px"></span>
            <span class="flex-grow-1 list-col-label">CANDIDAT / PROFIL</span>
            <span style="width:140px" class="list-col-label d-none d-lg-block">DIMENSIONS</span>
            <span style="width:100px" class="list-col-label text-center">SCORE</span>
            <span style="width:90px"  class="list-col-label text-center">TIER</span>
            <span style="width:100px" class="list-col-label text-center">DATE</span>
            <span style="width:90px"  class="list-col-label text-end pe-2">ACTIONS</span>
          </div>

          <!-- Recycle viewport -->
          <div class="recycle-viewport" ref="recycleViewport" @scroll="onRecycleScroll">
            <div :style="{ height: paddingTop + 'px' }"></div>

            <div v-for="(item, idx) in visibleAnalyses" :key="item.id"
              class="recycle-row"
              :class="{ 'row-selected': selectedAnalysisId === item.id }"
              @click="selectAnalysis(item)"
              :style="{ animationDelay: (idx % 8) * 0.03 + 's' }">

              <div class="recycle-rank" style="width:36px;flex-shrink:0">
                <span class="rank-num">#{{ item._rank }}</span>
              </div>

              <div class="recycle-avatar" :style="{ background: item._color + '22', color: item._color }">
                {{ item.nom?.[0] || '?' }}
              </div>

              <div class="recycle-main flex-grow-1">
                <div class="recycle-name">{{ item.nom }}</div>
                <div class="recycle-sub">{{ item.profile_type }}</div>
              </div>

              <div class="recycle-dims d-none d-lg-flex" style="width:140px;flex-shrink:0;gap:4px;flex-wrap:wrap">
                <span v-for="tr in (item.traits || []).slice(0,3)" :key="tr.name"
                  class="dim-chip" :style="{ color: tr.color, background: tr.color + '18' }">
                  {{ tr.val }}%
                </span>
              </div>

              <div style="width:100px;flex-shrink:0;text-align:center">
                <div class="score-ring-mini" :style="{ '--pct': item.global_score, '--col': item._color }">
                  <span class="score-ring-val">{{ item.global_score }}</span>
                </div>
              </div>

              <div style="width:90px;flex-shrink:0;text-align:center">
                <span class="tier-badge" :class="'tier-' + (item.neural_tier || '').toLowerCase()">
                  {{ item.neural_tier || '—' }}
                </span>
              </div>

              <div style="width:100px;flex-shrink:0;text-align:center">
                <span class="date-chip">{{ formatDate(item.date) }}</span>
              </div>

              <div style="width:90px;flex-shrink:0" class="d-flex justify-content-end gap-1 pe-2">
                <button class="btn-icon-sm" title="Voir rapport" @click.stop="viewDetail(item)">
                  <i class="fa-solid fa-eye"></i>
                </button>
                <button class="btn-icon-sm" title="Rapport PDF" @click.stop="exportReport(item)">
                  <i class="fa-solid fa-file-pdf"></i>
                </button>
                <button class="btn-icon-sm danger" title="Supprimer" @click.stop="deleteAnalysis(item.id)">
                  <i class="fa-solid fa-trash-can"></i>
                </button>
              </div>
            </div>

            <div :style="{ height: paddingBottom + 'px' }"></div>

            <div v-if="isLoadingMore" class="recycle-loader">
              <div class="tech-spinner" style="width:30px;height:30px;border-width:3px"></div>
              <span style="font-size:10px;color:#94a3b8;font-family:'JetBrains Mono',monospace">CHARGEMENT...</span>
            </div>

            <div v-if="!isLoadingMore && filteredAnalyses.length > 0 && scrolledToEnd" class="recycle-end">
              <i class="fa-solid fa-check-circle text-success me-2"></i>
              <span>{{ filteredAnalyses.length }} analyses chargées · Fin de liste</span>
            </div>

            <div v-if="filteredAnalyses.length === 0" class="recycle-empty">
              <i class="fa-solid fa-inbox fa-2x text-muted mb-3 d-block"></i>
              <p class="text-muted fw-700" style="font-size:13px">Aucune analyse trouvée</p>
            </div>
          </div>

          <!-- Footer -->
          <div class="recycle-footer d-flex align-items-center justify-content-between p-3 border-top">
            <div class="d-flex gap-4">
              <div class="recycle-stat"><span class="rv">{{ filteredAnalyses.length }}</span><span class="rl">Total</span></div>
              <div class="recycle-stat"><span class="rv text-success">{{ analyses.filter(a=>a.global_score>=80).length }}</span><span class="rl">Élite</span></div>
              <div class="recycle-stat"><span class="rv text-amber">{{ Math.round(avgScore) }}</span><span class="rl">Score Moy.</span></div>
            </div>
            <div style="font-size:10px;color:#94a3b8;font-family:'JetBrains Mono',monospace" class="d-flex align-items-center gap-2">
              <i class="fa-solid fa-recycle text-amber"></i>
              VIRTUAL SCROLL · {{ visibleAnalyses.length }} rows rendered
            </div>
          </div>
        </div>

        <!-- DETAIL PANEL (si analyse sélectionnée) -->
        <transition name="slide-up">
          <div v-if="selectedAnalysis" class="glass-surface p-4 mb-4 detail-panel">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h6 class="label-heading m-0">RAPPORT DÉTAILLÉ — {{ selectedAnalysis.nom }}</h6>
              <button @click="selectedAnalysis = null; selectedAnalysisId = null" class="btn-icon-sm">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>
            <div class="row g-4">
              <div class="col-lg-8">
                <div class="ai-insight-terminal p-4">
                  <div class="terminal-header-mini mb-2">LOGS_ANALYSE // {{ selectedAnalysis.id }}</div>
                  <div class="terminal-body-mini">
                    <div class="t-line" v-for="(line, i) in selectedAnalysis.terminal_insights" :key="i">
                      <span class="t-time">[{{ line.time }}]</span>
                      <span class="t-prompt" :class="line.type">›</span>
                      <span class="t-text" v-html="line.text"></span>
                    </div>
                    <div class="t-cursor-line"><span class="t-prompt green">›</span><span class="t-cursor">█</span></div>
                  </div>
                </div>
              </div>
              <div class="col-lg-4">
                <div class="detail-radar">
                  <h6 class="label-heading mb-3">Carte Cognitive</h6>
                  <svg viewBox="0 0 280 260" width="100%">
                    <polygon v-for="n in 4" :key="n" :points="getRadarRing(n * 25, selectedAnalysis.radar_data)" fill="none" stroke="rgba(148,163,184,0.15)"/>
                    <line v-for="(axis, i) in getRadarAxes(selectedAnalysis.radar_data)" :key="i"
                      :x1="140" :y1="130" :x2="axis.x2" :y2="axis.y2" stroke="rgba(148,163,184,0.2)"/>
                    <polygon :points="getRadarDataPointsSmall(selectedAnalysis.radar_data)"
                      fill="rgba(245,158,11,0.2)" stroke="#f59e0b" stroke-width="2"/>
                    <text v-for="(axis, i) in getRadarAxes(selectedAnalysis.radar_data)" :key="'l'+i"
                      :x="axis.lx2" :y="axis.ly2" text-anchor="middle" class="radar-label">{{ axis.label }}</text>
                  </svg>
                </div>
              </div>
            </div>
          </div>
        </transition>

        <!-- TERMINAL GLOBAL -->
        <div class="ai-insight-terminal p-0 position-relative overflow-hidden">
          <div class="terminal-noise"></div>
          <div class="terminal-corner tl"></div>
          <div class="terminal-corner tr"></div>
          <div class="terminal-corner bl"></div>
          <div class="terminal-corner br"></div>
          <div class="scanner-line"></div>
          <div class="terminal-titlebar">
            <div class="t-dots">
              <div class="terminal-dot red"></div>
              <div class="terminal-dot amber"></div>
              <div class="terminal-dot green"></div>
            </div>
            <span class="terminal-label">NEURAL_CORE // ANALYSE_COMPORTEMENTALE_REALTIME — v3.0</span>
            <div class="t-status-online"><span class="t-online-dot"></span> ONLINE</div>
          </div>
          <div class="terminal-body p-4">
            <div v-for="(line, i) in terminalLogs" :key="i" class="t-line-anim" :style="{ animationDelay: i * 0.05 + 's' }">
              <span class="t-time">{{ line.time }}</span>
              <span class="t-prompt" :class="line.type">›</span>
              <span class="t-text" v-html="line.text"></span>
            </div>
            <div class="t-cursor-line mt-1">
              <span class="t-prompt green">›</span>
              <span class="t-cursor">█</span>
            </div>
          </div>
        </div>

      </main>
    </div>

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
import { ref, reactive, computed, onMounted, onUnmounted, watch, inject } from 'vue';
import axios from 'axios';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const isDark      = inject('isDark', ref(false));
const toggleTheme = inject('toggleTheme', () => {});
const isLoading   = ref(true);
const isRefreshing = ref(false);
const today       = new Date().toLocaleDateString('fr-FR', { day: '2-digit', month: 'long', year: 'numeric' });

const analyses            = ref([]);
const selectedAnalysis    = ref(null);
const selectedAnalysisId  = ref(null);
const searchQuery         = ref('');
const recycleSearch       = ref('');
const profileFilter       = ref('');
const tierFilter          = ref('');
const sortBy              = ref('score_desc');

// ─── RECYCLE VIEW ────────────────────────────────────────
const ROW_HEIGHT      = 68;
const BUFFER          = 5;
const recycleViewport = ref(null);
const scrollTop       = ref(0);
const viewportHeight  = ref(480);
const isLoadingMore   = ref(false);
const scrolledToEnd   = ref(false);

const COLORS = ['#f59e0b', '#4f46e5', '#10b981', '#f43f5e', '#06b6d4', '#a855f7', '#ec4899'];

const filteredAnalyses = computed(() => {
  let list = analyses.value;
  const q  = (searchQuery.value + ' ' + recycleSearch.value).trim().toLowerCase();
  if (q) list = list.filter(a =>
    a.nom?.toLowerCase().includes(q) ||
    a.profile_type?.toLowerCase().includes(q) ||
    String(a.global_score).includes(q)
  );
  if (profileFilter.value) list = list.filter(a => a.profile_type?.includes(profileFilter.value));
  if (tierFilter.value)    list = list.filter(a => a.neural_tier === tierFilter.value);
  list = [...list].sort((a, b) => {
    if (sortBy.value === 'score_desc') return b.global_score - a.global_score;
    if (sortBy.value === 'score_asc')  return a.global_score - b.global_score;
    if (sortBy.value === 'name_asc')   return (a.nom || '').localeCompare(b.nom || '');
    if (sortBy.value === 'date_desc')  return new Date(b.date || 0) - new Date(a.date || 0);
    return 0;
  });
  return list.map((a, i) => ({
    ...a,
    _rank:  i + 1,
    _color: COLORS[i % COLORS.length],
  }));
});

const topAnalysis = computed(() =>
  filteredAnalyses.value.length ? filteredAnalyses.value[0] : null
);

const avgScore = computed(() => {
  if (!analyses.value.length) return 0;
  return analyses.value.reduce((s, a) => s + (a.global_score || 0), 0) / analyses.value.length;
});

const kpiStats = computed(() => [
  { label: 'Analyses',   val: analyses.value.length,                            icon: 'fa-solid fa-brain',      color: '#4f46e5', bg: '#eef2ff', trend: '+8%',  trendUp: true  },
  { label: 'Score Moy.', val: Math.round(avgScore.value) + '%',                  icon: 'fa-solid fa-chart-line', color: '#f59e0b', bg: '#fffbeb', trend: '+3%',  trendUp: true  },
  { label: 'Élite',      val: analyses.value.filter(a => a.global_score >= 80).length, icon: 'fa-solid fa-star', color: '#10b981', bg: '#ecfdf5', trend: '+12%', trendUp: true  },
  { label: 'Profils',    val: new Set(analyses.value.map(a => a.profile_type)).size,   icon: 'fa-solid fa-users', color: '#f43f5e', bg: '#fef2f2', trend: '0',    trendUp: false },
]);

// Virtual scroll
const startIdx = computed(() => Math.max(0, Math.floor(scrollTop.value / ROW_HEIGHT) - BUFFER));
const endIdx   = computed(() =>
  Math.min(filteredAnalyses.value.length, Math.ceil((scrollTop.value + viewportHeight.value) / ROW_HEIGHT) + BUFFER)
);
const visibleAnalyses = computed(() => filteredAnalyses.value.slice(startIdx.value, endIdx.value));
const paddingTop      = computed(() => startIdx.value * ROW_HEIGHT);
const paddingBottom   = computed(() =>
  Math.max(0, (filteredAnalyses.value.length - endIdx.value) * ROW_HEIGHT)
);

const onRecycleScroll = (e) => {
  scrollTop.value = e.target.scrollTop;
  const { scrollHeight, scrollTop: st, clientHeight } = e.target;
  if (scrollHeight - st - clientHeight < 80 && !isLoadingMore.value && !scrolledToEnd.value) {
    loadMore();
  }
};

const loadMore = async () => {
  if (isLoadingMore.value || scrolledToEnd.value) return;
  isLoadingMore.value = true;
  await new Promise(r => setTimeout(r, 700));
  const extras = generateMockAnalyses(10, analyses.value.length);
  analyses.value.push(...extras);
  isLoadingMore.value = false;
  if (analyses.value.length >= 60) scrolledToEnd.value = true;
};

// ─── RADAR UTILS ─────────────────────────────────────────
const CX = 150, CY = 140, MAX_R = 90;
const CX2 = 140, CY2 = 130, MAX_R2 = 80;

const getRadarAxes = (data) => {
  if (!data?.length) return [];
  return data.map((d, i) => {
    const angle = (i * 2 * Math.PI / data.length) - Math.PI / 2;
    return {
      x:   CX + MAX_R * Math.cos(angle),
      y:   CY + MAX_R * Math.sin(angle),
      lx:  CX + (MAX_R + 28) * Math.cos(angle),
      ly:  CY + (MAX_R + 28) * Math.sin(angle),
      x2:  CX2 + MAX_R2 * Math.cos(angle),
      y2:  CY2 + MAX_R2 * Math.sin(angle),
      lx2: CX2 + (MAX_R2 + 24) * Math.cos(angle),
      ly2: CY2 + (MAX_R2 + 24) * Math.sin(angle),
      label: d.label,
    };
  });
};

const getRadarRing = (pct, data) => {
  if (!data?.length) return '';
  const r = MAX_R * pct / 100;
  return data.map((_, i) => {
    const angle = (i * 2 * Math.PI / data.length) - Math.PI / 2;
    return `${CX + r * Math.cos(angle)},${CY + r * Math.sin(angle)}`;
  }).join(' ');
};

const getRadarDataPoints = (data) => {
  if (!data?.length) return '';
  return data.map((d, i) => {
    const angle = (i * 2 * Math.PI / data.length) - Math.PI / 2;
    const r = MAX_R * d.val / 100;
    return `${CX + r * Math.cos(angle)},${CY + r * Math.sin(angle)}`;
  }).join(' ');
};

const getRadarDataPointsSmall = (data) => {
  if (!data?.length) return '';
  return data.map((d, i) => {
    const angle = (i * 2 * Math.PI / data.length) - Math.PI / 2;
    const r = MAX_R2 * d.val / 100;
    return `${CX2 + r * Math.cos(angle)},${CY2 + r * Math.sin(angle)}`;
  }).join(' ');
};

// ─── TERMINAL ────────────────────────────────────────────
const terminalLogs = ref([
  { time: '00:00:01', type: 'green', text: 'Moteur Neural initialisé <span class="t-ok">[ OK ]</span>' },
  { time: '00:00:03', type: 'blue',  text: 'Connexion API Analyse <span class="t-hi">établie</span>' },
  { time: '00:00:05', type: 'amber', text: 'Chargement des profils comportementaux...' },
  { time: '00:00:07', type: 'green', text: 'RecycleView Virtual Scroll <span class="t-ok">[ READY ]</span>' },
  { time: '00:00:09', type: 'green', text: 'Dashboard opérationnel <span class="t-ok">[ DEPLOYED ]</span>' },
]);

const addTerminalLog = (type, text) => {
  const time = new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  terminalLogs.value.push({ time, type, text });
  if (terminalLogs.value.length > 20) terminalLogs.value.shift();
};

// ─── TOAST ───────────────────────────────────────────────
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });
let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

// ─── ACTIONS ─────────────────────────────────────────────
const selectAnalysis = (item) => {
  if (selectedAnalysisId.value === item.id) {
    selectedAnalysis.value   = null;
    selectedAnalysisId.value = null;
  } else {
    selectedAnalysis.value   = item;
    selectedAnalysisId.value = item.id;
  }
};

const viewDetail = (item) => {
  selectedAnalysis.value   = item;
  selectedAnalysisId.value = item.id;
  showPulseToast(`Rapport : ${item.nom}`, 'success', 'fa-solid fa-eye');
};

const exportReport = (item) => {
  showPulseToast(`Export PDF : ${item.nom}`, 'warn', 'fa-solid fa-file-pdf');
};

const deleteAnalysis = (id) => {
  if (!confirm('Supprimer cette analyse ?')) return;
  analyses.value = analyses.value.filter(a => a.id !== id);
  if (selectedAnalysisId.value === id) {
    selectedAnalysis.value   = null;
    selectedAnalysisId.value = null;
  }
  showPulseToast('Analyse supprimée.', 'warn', 'fa-solid fa-trash-can');
};

const formatDate = (d) =>
  d ? new Date(d).toLocaleDateString('fr-FR', { day: 'numeric', month: 'short' }) : '—';

// ─── DATA ─────────────────────────────────────────────────
const PROFILES = ['Profil Analytique', 'Profil Créatif', 'Profil Leader', 'Profil Opérationnel'];
const TIERS    = ['Élite', 'Senior', 'Junior'];
const NAMES    = ['Alice Durand', 'Mohamed Ben Ali', 'Sophie Martin', 'Karim Boulard', 'Laura Chen',
                  'Rami Mansour', 'Léa Petit', 'Omar Saidi', 'Clara Dupont', 'Yassin Hajem'];
const TRAIT_SETS = [
  [{ name: 'Logique', val: 88, color: '#4f46e5', icon: 'fa-solid fa-brain' }, { name: 'Adaptabilité', val: 75, color: '#f59e0b', icon: 'fa-solid fa-bolt' }, { name: 'Communication', val: 80, color: '#10b981', icon: 'fa-solid fa-comments' }],
  [{ name: 'Créativité', val: 92, color: '#a855f7', icon: 'fa-solid fa-palette' }, { name: 'Leadership', val: 70, color: '#f43f5e', icon: 'fa-solid fa-crown' }, { name: 'Rigueur', val: 85, color: '#06b6d4', icon: 'fa-solid fa-shield' }],
];

function generateMockAnalyses(count = 20, offset = 0) {
  return Array.from({ length: count }, (_, i) => {
    const score = Math.floor(Math.random() * 40 + 55);
    const tier  = score >= 85 ? 'Élite' : score >= 70 ? 'Senior' : 'Junior';
    const tIdx  = (offset + i) % TRAIT_SETS.length;
    return {
      id:              `ana-${offset + i + 1}`,
      nom:             NAMES[(offset + i) % NAMES.length] + (offset + i >= NAMES.length ? ` ${Math.floor((offset + i) / NAMES.length) + 1}` : ''),
      global_score:    score,
      profile_type:    PROFILES[(offset + i) % PROFILES.length],
      neural_tier:     tier,
      date:            new Date(Date.now() - Math.random() * 30 * 86400000).toISOString(),
      traits:          TRAIT_SETS[tIdx].map(t => ({ ...t, val: Math.floor(Math.random() * 30 + 60) })),
      radar_data:      [
        { label: 'TECH',  val: Math.floor(Math.random() * 40 + 55) },
        { label: 'SOFT',  val: Math.floor(Math.random() * 40 + 55) },
        { label: 'LOGIC', val: Math.floor(Math.random() * 40 + 55) },
        { label: 'SPEED', val: Math.floor(Math.random() * 40 + 55) },
        { label: 'SQL',   val: Math.floor(Math.random() * 40 + 55) },
      ],
      terminal_insights: [
        { time: '0ms',  type: 'green', text: `Analyse démarrée pour <span class="t-hi">${NAMES[(offset + i) % NAMES.length]}</span>` },
        { time: '120ms', type: 'blue', text: `Score global : <span class="t-amber">${score}%</span> — Tier : <span class="t-ok">${tier}</span>` },
        { time: '250ms', type: 'green', text: `Rapport finalisé <span class="t-ok">[ OK ]</span>` },
      ],
    };
  });
}

const fetchAnalyses = async () => {
  isRefreshing.value = true;
  try {
    const fd = new FormData();
    fd.append('nom', 'Candidat');
    fd.append('scores_techniques', 'Logique: 85, Tech: 70, Soft: 80');
    const response = await axios.post(`${import.meta.env.VITE_AI_URL || 'http://localhost:5600'}/ia/analyze-candidate`, fd);
    if (Array.isArray(response.data)) {
      analyses.value = response.data;
    } else if (response.data) {
      analyses.value = [response.data];
    }
    addTerminalLog('green', `${analyses.value.length} analyses chargées depuis l'API <span class="t-ok">[ OK ]</span>`);
  } catch {
    addTerminalLog('amber', 'Mode local activé — API indisponible');
    if (analyses.value.length === 0) {
      analyses.value = generateMockAnalyses(25);
    }
  } finally {
    isRefreshing.value = false;
    setTimeout(() => { isLoading.value = false; }, 800);
  }
};

watch(recycleViewport, (el) => {
  if (el) viewportHeight.value = el.clientHeight;
});

onMounted(() => {
  fetchAnalyses();
  if (recycleViewport.value) viewportHeight.value = recycleViewport.value.clientHeight;
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800;900&family=JetBrains+Mono:wght@400;500;700&display=swap');

/* ─── VARIABLES DARK / LIGHT ─────────────────────────── */
.admin-body {
  --bg-primary:    #f8fafc;
  --bg-surface:    rgba(255,255,255,0.8);
  --bg-surface-solid: #ffffff;
  --bg-input:      #f8fafc;
  --text-primary:  #0f172a;
  --text-secondary:#64748b;
  --text-muted:    #94a3b8;
  --border-color:  rgba(255,255,255,0.93);
  --border-subtle: #eef2f6;
  --row-hover:     rgba(255,251,235,0.6);
  --terminal-bg:   linear-gradient(135deg,#080f1e 0%,#0f172a 40%,#130a2a 80%,#0f172a 100%);
}
.admin-body[data-theme="dark"] {
  --bg-primary:    #0d1117;
  --bg-surface:    rgba(22,27,34,0.9);
  --bg-surface-solid: #161b22;
  --bg-input:      rgba(255,255,255,0.05);
  --text-primary:  #f0f6fc;
  --text-secondary:#8b949e;
  --text-muted:    #6e7681;
  --border-color:  rgba(255,255,255,0.08);
  --border-subtle: rgba(255,255,255,0.06);
  --row-hover:     rgba(245,158,11,0.07);
  --terminal-bg:   linear-gradient(135deg,#010409 0%,#0d1117 50%,#010409 100%);
}

/* ─── ROOT ───────────────────────────────────────────── */
.admin-body {
  min-height: 100vh;
  background-color: var(--bg-primary);
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: var(--text-primary);
  overflow-x: hidden;
  transition: background-color 0.3s, color 0.3s;
}
.content { position: relative; }

/* ─── THEME TOGGLE ───────────────────────────────────── */
.theme-toggle-btn {
  position: fixed; top: 80px; right: 20px; z-index: 200;
  width: 42px; height: 42px; border-radius: 14px;
  background: var(--bg-surface-solid); border: 1px solid var(--border-subtle);
  color: var(--text-secondary); cursor: pointer; font-size: 16px;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s; backdrop-filter: blur(12px);
  box-shadow: 0 4px 16px rgba(0,0,0,0.08);
}
.theme-toggle-btn:hover { color: #f59e0b; border-color: #f59e0b; }

/* ─── BG ─────────────────────────────────────────────── */
.background-overlay {
  position: fixed; inset: 0; z-index: 0;
  background:
    radial-gradient(ellipse 70% 50% at 75% 0%, rgba(251,191,36,0.07) 0%, transparent 60%),
    radial-gradient(ellipse 60% 60% at 5% 100%, rgba(79,70,229,0.07) 0%, transparent 60%),
    linear-gradient(160deg, #f9fafb 0%, #f1f5f9 60%, #eef0f7 100%);
  transition: 0.3s;
}
.admin-body[data-theme="dark"] .background-overlay {
  background:
    radial-gradient(ellipse 70% 50% at 75% 0%, rgba(245,158,11,0.04) 0%, transparent 60%),
    radial-gradient(ellipse 60% 60% at 5% 100%, rgba(79,70,229,0.04) 0%, transparent 60%),
    linear-gradient(160deg, #0d1117 0%, #010409 100%);
}
.tech-grid-subtle {
  position: fixed; inset: 0; z-index: 1;
  background-image:
    linear-gradient(rgba(148,163,184,0.055) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148,163,184,0.055) 1px, transparent 1px);
  background-size: 48px 48px;
}
.glow-orb { position: fixed; border-radius: 50%; filter: blur(130px); z-index: 1; pointer-events: none; }
.orb-amber  { width:500px;height:500px;background:radial-gradient(circle,#fde68a,#fbbf24 50%,transparent);top:-12%;right:-6%;opacity:.14;animation:orb-drift 18s ease-in-out infinite; }
.orb-indigo { width:550px;height:550px;background:radial-gradient(circle,#c7d2fe,#4f46e5 50%,transparent);bottom:-12%;left:-7%;opacity:.09;animation:orb-drift 24s ease-in-out infinite reverse; }
@keyframes orb-drift {
  0%,100%{transform:translateY(0) translateX(0) scale(1)}
  33%{transform:translateY(-30px) translateX(15px) scale(1.04)}
  66%{transform:translateY(15px) translateX(-10px) scale(.97)}
}
.main-viewport { position: relative; z-index: 10; }

/* ─── LOADER ─────────────────────────────────────────── */
.loader-overlay {
  position: fixed; inset: 0; background: var(--bg-primary); z-index: 9999;
  display: flex; flex-direction: column; align-items: center; justify-content: center;
}
.tech-loader-container { text-align: center; }
.tech-spinner { width:60px;height:60px;border:4px solid var(--border-subtle);border-top:4px solid #4f46e5;border-radius:50%;animation:spin .9s linear infinite;margin:0 auto 20px; }
.loader-text { font-weight:800;color:var(--text-primary);letter-spacing:2px;font-size:14px;font-family:'JetBrains Mono',monospace; }
.loader-sub  { font-size:11px;color:var(--text-muted);margin-top:6px;font-family:'JetBrains Mono',monospace; }
@keyframes spin { to { transform: rotate(360deg); } }
.fade-leave-active { transition: opacity 0.3s; }
.fade-leave-to { opacity: 0; }
.animate-fade-in { animation: fadeIn 0.6s ease-out forwards; }
@keyframes fadeIn { from {opacity:0;transform:translateY(15px);} to {opacity:1;transform:translateY(0);} }

/* ─── HEADER ─────────────────────────────────────────── */
.breadcrumb-cyber {
  font-family:'JetBrains Mono',monospace;font-size:10px;color:var(--text-muted);
  letter-spacing:2px;display:flex;align-items:center;gap:8px;
}
.bc-root { color: var(--text-muted); } .bc-sep { color: var(--border-subtle); }
.bc-current { color: var(--text-secondary); }
.main-title { font-size:clamp(1.8rem,3vw,2.6rem);font-weight:900;color:var(--text-primary);letter-spacing:-1.5px;line-height:1; }
.title-accent {
  background:linear-gradient(135deg,#4f46e5,#7c3aed,#a855f7);
  -webkit-background-clip:text;-webkit-text-fill-color:transparent;background-clip:text;
}
.page-sub { font-size:12px;color:var(--text-muted);font-weight:500;margin-top:6px; }
.system-status {
  display:flex;align-items:center;background:var(--bg-surface);backdrop-filter:blur(12px);
  border:1px solid var(--border-color);border-radius:100px;padding:7px 16px;
}
.status-dot { width:7px;height:7px;background:#10b981;border-radius:50%;display:inline-block;margin-right:8px; }
.pulse { animation: statusPulse 2s infinite; }
@keyframes statusPulse {
  0%{box-shadow:0 0 0 0 rgba(16,185,129,.6)} 70%{box-shadow:0 0 0 8px rgba(16,185,129,0)} 100%{box-shadow:0 0 0 0 rgba(16,185,129,0)}
}
.status-text { font-size:10px;font-weight:800;color:var(--text-secondary);letter-spacing:.8px;text-transform:uppercase; }

/* ─── KPI CARDS ──────────────────────────────────────── */
.stat-card-premium {
  background:var(--bg-surface-solid);border-radius:24px;padding:24px;
  display:flex;align-items:center;border:1px solid var(--border-subtle);
  transition:.2s; box-shadow:0 2px 8px rgba(0,0,0,0.04);
}
.stat-card-premium:hover { transform:translateY(-3px);box-shadow:0 10px 30px rgba(0,0,0,0.08); }
.stat-icon-wrapper { width:56px;height:56px;border-radius:16px;display:flex;align-items:center;justify-content:center;font-size:1.4rem;flex-shrink:0; }
.stat-value { font-size:1.6rem;font-weight:800;line-height:1;color:var(--text-primary); }
.stat-label { font-size:.7rem;font-weight:700;color:var(--text-muted);text-transform:uppercase;margin-top:4px; }
.trend-badge { padding:4px 10px;border-radius:8px;font-size:.65rem;font-weight:800;display:flex;align-items:center;gap:4px; }
.trend-badge.up   { background:#ecfdf5;color:#10b981; }
.trend-badge.down { background:#fef2f2;color:#f43f5e; }

/* ─── GLASS SURFACE ──────────────────────────────────── */
.glass-surface {
  background:var(--bg-surface);backdrop-filter:blur(20px);
  border:1px solid var(--border-color);border-radius:28px;position:relative;overflow:hidden;
  box-shadow:0 10px 40px rgba(0,0,0,.03);
}
.card-inner-glow { position:absolute;top:-50px;right:-50px;width:200px;height:200px;border-radius:50%;pointer-events:none; }
.card-inner-glow.amber  { background:radial-gradient(circle,rgba(251,191,36,.14) 0%,transparent 65%); }
.card-inner-glow.indigo { background:radial-gradient(circle,rgba(79,70,229,.12) 0%,transparent 65%); }
.label-heading { font-size:11px;font-weight:800;color:var(--text-muted);letter-spacing:1.5px;text-transform:uppercase; }
.vectors-badge { font-size:9px;font-weight:800;color:var(--text-muted);letter-spacing:1.5px;text-transform:uppercase;background:var(--bg-input);border:1px solid var(--border-subtle);padding:4px 10px;border-radius:7px; }

/* ─── FILTER BAR ─────────────────────────────────────── */
.filter-bar-glass { border-radius:20px!important; }
.filter-bar-inner { display:flex;justify-content:space-between;align-items:center;padding:12px 20px;gap:16px;flex-wrap:wrap; }
.search-cyber-box { position:relative;flex-grow:1;max-width:420px;display:flex;align-items:center; }
.search-cyber-box i { position:absolute;left:14px;color:var(--text-muted);font-size:12px; }
.search-cyber-box input {
  width:100%;padding:11px 40px 11px 38px;background:var(--bg-input);border:1.5px solid var(--border-subtle);
  border-radius:14px;font-family:'Plus Jakarta Sans',sans-serif;font-size:13px;font-weight:600;
  color:var(--text-primary);outline:none;transition:.2s;
}
.search-cyber-box input:focus { border-color:#f59e0b;background:var(--bg-surface-solid);box-shadow:0 0 0 3px rgba(245,158,11,.1); }
.btn-clear-search { position:absolute;right:10px;border:none;background:transparent;color:var(--text-muted);cursor:pointer;font-size:12px; }
.filter-cyber-controls { display:flex;gap:8px;align-items:center; }
.cyber-select {
  padding:9px 14px;border-radius:12px;border:1.5px solid var(--border-subtle);
  background:var(--bg-input);font-size:12px;font-weight:700;color:var(--text-primary);
  font-family:'Plus Jakarta Sans',sans-serif;outline:none;cursor:pointer;
}
.cyber-select:focus { border-color:#f59e0b; }
.btn-cyber-refresh {
  width:38px;height:38px;border-radius:12px;border:1.5px solid var(--border-subtle);
  background:var(--bg-surface-solid);display:flex;align-items:center;justify-content:center;
  cursor:pointer;color:var(--text-secondary);transition:.2s;font-size:13px;
}
.btn-cyber-refresh:hover { background:#0f172a;color:#f59e0b;border-color:#0f172a; }
.btn-cyber-refresh.spinning i { animation:spin .8s linear infinite; }

/* ─── GAUGE ──────────────────────────────────────────── */
.neural-gauge-wrapper { position:relative;width:180px;height:180px; }
.deco-ring { position:absolute;border-radius:50%;border:1px solid rgba(251,191,36,.1); }
.ring-1 { inset:-8px;animation:ring-spin 20s linear infinite; }
.ring-2 { inset:-18px;border-style:dashed;animation:ring-spin 30s linear infinite reverse;opacity:.5; }
@keyframes ring-spin { to { transform:rotate(360deg); } }
.neural-svg { transform:rotate(-90deg);width:100%;height:100%; }
.gauge-bg  { fill:none;stroke:var(--border-subtle);stroke-width:8; }
.gauge-arc { fill:none;stroke:#f59e0b;stroke-width:8;stroke-linecap:round;transition:1.4s cubic-bezier(.165,.84,.44,1);filter:drop-shadow(0 0 8px rgba(245,158,11,.6)); }
.gauge-content { position:absolute;inset:0;display:flex;flex-direction:column;align-items:center;justify-content:center;gap:2px; }
.gauge-value-wrap { display:flex;align-items:flex-start;gap:2px; }
.gauge-value  { font-size:3rem;font-weight:900;color:var(--text-primary);line-height:1;letter-spacing:-3px; }
.gauge-percent { font-size:1.1rem;font-weight:700;color:#f59e0b;margin-top:6px; }
.gauge-label  { font-size:9px;font-weight:800;color:var(--text-muted);letter-spacing:2px;text-transform:uppercase; }
.gauge-tier   { font-size:9px;font-weight:800;color:#f59e0b;letter-spacing:1.5px; }
.badge-profile {
  background:#0f172a;color:#fbbf24;padding:7px 20px;border-radius:100px;
  font-size:10.5px;font-weight:800;display:inline-flex;align-items:center;gap:7px;letter-spacing:.8px;
  box-shadow:0 6px 16px rgba(15,23,42,.15);
}
.badge-dot { width:6px;height:6px;background:#fbbf24;border-radius:50%;animation:statusPulse 2s infinite; }

/* ─── RADAR ──────────────────────────────────────────── */
.radar-container { display:flex;justify-content:center; }
.radar-svg { width:100%;max-width:300px; }
.radar-label { font-family:'Plus Jakarta Sans',sans-serif;font-size:10px;font-weight:800;fill:var(--text-muted);text-transform:uppercase;letter-spacing:.8px; }

/* ─── TRAITS ─────────────────────────────────────────── */
.traits-list { display:flex;flex-direction:column;gap:12px; }
.trait-row {
  display:flex;justify-content:space-between;align-items:center;
  background:var(--bg-input);border:1px solid var(--border-subtle);
  border-radius:16px;padding:12px 14px;transition:.3s cubic-bezier(.165,.84,.44,1);
}
.trait-row:hover { transform:translateX(4px);border-color:rgba(79,70,229,.2);box-shadow:0 6px 20px rgba(79,70,229,.06); }
.trait-row-left  { display:flex;align-items:center;gap:10px; }
.trait-row-right { display:flex;flex-direction:column;align-items:flex-end;gap:6px;min-width:80px; }
.trait-icon-wrap { width:32px;height:32px;border-radius:10px;display:flex;align-items:center;justify-content:center;font-size:13px;flex-shrink:0; }
.trait-name  { font-size:12px;font-weight:800;color:var(--text-primary);letter-spacing:.3px; }
.score-val   { font-family:'JetBrains Mono',monospace;font-size:16px;font-weight:700; }
.score-val small { font-size:11px;opacity:.7; }
.progress-wrap { width:80px; }
.progress-cyber { height:4px;background:var(--border-subtle);border-radius:10px;overflow:hidden; }
.progress-fill  { height:100%;border-radius:10px;position:relative;transition:width 1.2s cubic-bezier(.165,.84,.44,1); }
.progress-shine { position:absolute;top:0;right:0;width:12px;height:100%;background:linear-gradient(90deg,transparent,rgba(255,255,255,.7)); }

/* ─── RECYCLE VIEW ───────────────────────────────────── */
.recycle-search-input {
  padding:8px 14px;border-radius:12px;border:1.5px solid var(--border-subtle);
  background:var(--bg-input);font-size:12px;font-weight:700;
  font-family:'Plus Jakarta Sans',sans-serif;outline:none;color:var(--text-primary);
  width:180px;transition:.2s;
}
.recycle-search-input:focus { border-color:#f59e0b;background:var(--bg-surface-solid); }
.recycle-header-row {
  background:var(--bg-input);border-bottom:1px solid var(--border-subtle);
}
.list-col-label { font-size:.6rem;font-weight:900;color:var(--text-muted);text-transform:uppercase;letter-spacing:1px; }
.recycle-viewport {
  height:440px;overflow-y:auto;position:relative;scroll-behavior:smooth;
}
.recycle-viewport::-webkit-scrollbar { width:6px; }
.recycle-viewport::-webkit-scrollbar-track { background:transparent; }
.recycle-viewport::-webkit-scrollbar-thumb { background:var(--border-subtle);border-radius:10px; }
.recycle-viewport::-webkit-scrollbar-thumb:hover { background:#fbbf24; }
.recycle-row {
  display:flex;align-items:center;gap:12px;padding:0 24px;height:68px;
  border-bottom:1px solid var(--border-subtle);
  transition:background .15s,transform .15s;
  animation:entry-up .35s both;cursor:pointer;
}
.recycle-row:hover { background:var(--row-hover);transform:translateX(3px); }
.recycle-row.row-selected { background:rgba(79,70,229,.08);border-left:3px solid #4f46e5; }
@keyframes entry-up { from{opacity:0;transform:translateY(12px);}to{opacity:1;transform:translateY(0);} }
.rank-num { font-family:'JetBrains Mono',monospace;font-size:11px;font-weight:800;color:var(--text-muted); }
.recycle-avatar {
  width:38px;height:38px;min-width:38px;border-radius:12px;flex-shrink:0;
  display:flex;align-items:center;justify-content:center;
  font-weight:900;font-size:15px;font-family:'JetBrains Mono',monospace;
}
.recycle-main { min-width:0; }
.recycle-name { font-size:13px;font-weight:800;color:var(--text-primary);white-space:nowrap;overflow:hidden;text-overflow:ellipsis; }
.recycle-sub  { font-size:10px;color:var(--text-muted);font-weight:600;margin-top:2px; }
.dim-chip { font-size:9px;font-weight:800;padding:2px 7px;border-radius:6px; }
.score-ring-mini {
  --pct: 75; --col: #f59e0b;
  width:42px;height:42px;border-radius:50%;display:inline-flex;align-items:center;justify-content:center;
  background:conic-gradient(var(--col) calc(var(--pct)*1%),var(--border-subtle) 0);
  box-shadow:inset 0 0 0 6px var(--bg-surface-solid);
}
.score-ring-val { font-family:'JetBrains Mono',monospace;font-size:11px;font-weight:900;color:var(--text-primary); }
.tier-badge { padding:4px 10px;border-radius:8px;font-size:10px;font-weight:800; }
.tier-élite  { background:#fffbeb;color:#f59e0b; }
.tier-senior { background:#ecfdf5;color:#10b981; }
.tier-junior { background:#f1f5f9;color:#64748b; }
.date-chip { background:var(--bg-input);border:1px solid var(--border-subtle);color:var(--text-muted);font-size:10px;font-weight:800;padding:3px 10px;border-radius:8px;font-family:'JetBrains Mono',monospace; }
.recycle-loader { display:flex;flex-direction:column;align-items:center;padding:20px;gap:8px; }
.recycle-end { text-align:center;padding:16px;font-size:11px;font-weight:700;color:var(--text-muted);font-family:'JetBrains Mono',monospace; }
.recycle-empty { text-align:center;padding:40px;color:var(--text-muted); }
.recycle-footer { background:var(--bg-input);border-radius:0 0 28px 28px;border-color:var(--border-subtle)!important; }
.recycle-stat { display:flex;flex-direction:column;align-items:center; }
.recycle-stat .rv { font-size:14px;font-weight:900;color:var(--text-primary);line-height:1; }
.recycle-stat .rl { font-size:9px;font-weight:700;color:var(--text-muted);text-transform:uppercase;letter-spacing:.8px;margin-top:2px; }
.text-success { color:#10b981!important; }
.text-amber   { color:#f59e0b!important; }
.btn-icon-sm  { width:30px;height:30px;border-radius:9px;border:1.5px solid var(--border-subtle);background:var(--bg-surface-solid);color:var(--text-secondary);cursor:pointer;transition:.2s;font-size:11px;display:flex;align-items:center;justify-content:center; }
.btn-icon-sm:hover { background:var(--bg-input);color:var(--text-primary);border-color:var(--text-muted); }
.btn-icon-sm.danger:hover { background:#fff1f2;color:#f43f5e;border-color:#f43f5e; }

/* ─── TABLE HEADER ───────────────────────────────────── */
.table-header-cyber { border-bottom:1px solid var(--border-subtle); }
.table-icon-box { width:40px;height:40px;border-radius:13px;background:#fffbeb;color:#f59e0b;display:flex;align-items:center;justify-content:center;font-size:1rem;flex-shrink:0; }

/* ─── DETAIL PANEL ───────────────────────────────────── */
.detail-panel { border-color:rgba(79,70,229,.2)!important; }
.slide-up-enter-active { animation:slideUp .35s ease-out; }
.slide-up-leave-active { animation:slideUp .25s ease-in reverse; }
@keyframes slideUp { from{opacity:0;transform:translateY(20px);}to{opacity:1;transform:translateY(0);} }
.detail-radar h6 { color:var(--text-muted); }

/* ─── TERMINAL ───────────────────────────────────────── */
.ai-insight-terminal {
  background: var(--terminal-bg);
  border-radius:28px;border:1px solid rgba(255,255,255,.06);
  box-shadow:0 32px 64px rgba(15,23,42,.25),inset 0 1px 0 rgba(255,255,255,.04);
}
.terminal-header-mini { font-size:10px;opacity:.3;letter-spacing:1px;color:white;font-family:'JetBrains Mono',monospace; }
.terminal-body-mini,.terminal-body { display:flex;flex-direction:column;gap:4px; }
.terminal-corner { position:absolute;width:12px;height:12px;border-color:rgba(79,70,229,.3);border-style:solid; }
.terminal-corner.tl { top:12px;left:12px;border-width:1px 0 0 1px; }
.terminal-corner.tr { top:12px;right:12px;border-width:1px 1px 0 0; }
.terminal-corner.bl { bottom:12px;left:12px;border-width:0 0 1px 1px; }
.terminal-corner.br { bottom:12px;right:12px;border-width:0 1px 1px 0; }
.terminal-noise { position:absolute;inset:0;border-radius:28px;opacity:.018;pointer-events:none;background-image:url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }
.scanner-line { position:absolute;top:0;left:0;width:100%;height:1px;background:linear-gradient(90deg,transparent 0%,rgba(234,179,8,.5) 40%,rgba(234,179,8,.8) 50%,rgba(234,179,8,.5) 60%,transparent 100%);animation:scan 6s linear infinite; }
@keyframes scan { 0%{top:0%;opacity:0}5%{opacity:1}95%{opacity:.3}100%{top:100%;opacity:0} }
.terminal-titlebar { display:flex;align-items:center;padding:16px 20px 12px;border-bottom:1px solid rgba(255,255,255,.05); }
.t-dots { display:flex;gap:5px;margin-right:14px; }
.terminal-dot { width:10px;height:10px;border-radius:50%; }
.terminal-dot.red{background:#ef4444;}.terminal-dot.amber{background:#f59e0b;}.terminal-dot.green{background:#22c55e;}
.terminal-label { flex:1;font-family:'JetBrains Mono',monospace;font-size:10px;color:rgba(255,255,255,.2);letter-spacing:1px; }
.t-status-online { display:flex;align-items:center;gap:5px;font-size:9px;font-weight:800;color:#22c55e;letter-spacing:1px;font-family:'JetBrains Mono',monospace; }
.t-online-dot { width:5px;height:5px;background:#22c55e;border-radius:50%;animation:statusPulse 2s infinite; }
.t-line,.t-line-anim { display:flex;align-items:baseline;gap:8px;animation:entry-up .4s both; }
.t-time   { font-family:'JetBrains Mono',monospace;font-size:10px;color:rgba(255,255,255,.18);min-width:68px;flex-shrink:0; }
.t-prompt { font-family:'JetBrains Mono',monospace;font-size:13px;font-weight:700;flex-shrink:0; }
.t-prompt.green{color:#22c55e;}.t-prompt.blue{color:#60a5fa;}.t-prompt.amber{color:#f59e0b;}
.t-text   { font-family:'JetBrains Mono',monospace;font-size:11.5px;color:rgba(255,255,255,.45);line-height:1.8; }
.t-hi     { color:rgba(255,255,255,.85);font-weight:600; }
.t-ok     { color:#22c55e;font-weight:700; }
.t-amber  { color:#fbbf24;font-weight:600; }
.t-cursor-line { display:flex;align-items:center;gap:8px;margin-top:4px; }
.t-cursor { font-family:'JetBrains Mono',monospace;font-size:14px;color:#4f46e5;animation:blink-cursor 1s step-end infinite; }
@keyframes blink-cursor { 0%,100%{opacity:1}50%{opacity:0} }

/* ─── TOAST ──────────────────────────────────────────── */
.enigma-toast { position:fixed;bottom:30px;right:30px;background:#0f172a;color:white;padding:20px 28px;border-radius:20px;display:flex;align-items:center;gap:14px;z-index:3000;border-left:5px solid #f59e0b;box-shadow:0 20px 40px rgba(0,0,0,.2); }
.t-success{border-left-color:#10b981;}.t-error{border-left-color:#f43f5e;}.t-warn{border-left-color:#f59e0b;}
.t-ico { font-size:18px; }
.t-body strong { font-size:10px;font-weight:900;letter-spacing:1px;color:rgba(255,255,255,.5); }
.t-body .small { font-size:12px;color:rgba(255,255,255,.85);margin-top:2px; }
.toast-slide-enter-active { animation:slideIn .4s ease-out; }
.toast-slide-leave-active  { animation:slideIn .3s ease-in reverse; }
@keyframes slideIn { from{transform:translateX(120%);opacity:0;}to{transform:translateX(0);opacity:1;} }

/* ─── RESPONSIVE ─────────────────────────────────────── */
@media (max-width:768px) {
  .glass-surface { border-radius:20px; }
  .main-title { font-size:1.8rem; }
  .neural-gauge-wrapper { width:150px;height:150px; }
}
</style>