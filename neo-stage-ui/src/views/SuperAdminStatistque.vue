<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- ═══════════════════════════════════════════
               HEADER
          ═══════════════════════════════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Super Admin</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Statistiques</span>
              </div>
              <h2 class="premium-title">
                Performance des Flux d'Évaluation
              </h2>
              <p class="text-muted small fw-700 mt-1 mb-0" style="font-size:0.78rem;">
                Scores moyens par campagne
              </p>
              <div class="d-flex gap-3 mt-3 flex-wrap">
                <div class="status-pill live">
                  <span class="pill-dot"></span>
                  <span>{{ t('statistiquesView.liveDatastream') }}</span>
                </div>
                <div class="status-pill latency">
                  <i class="fa-solid fa-bolt-lightning me-1"></i>
                  <span>{{ responseTime }}{{ t('statistiquesView.responseMs') }}</span>
                </div>
                <div class="status-pill" :class="systemStatus === t('statistiquesView.status.optimal') ? 'status-ok' : 'status-warn'">
                  <i class="fa-solid fa-shield-halved me-1"></i>
                  <span>{{ systemStatus }}</span>
                </div>
              </div>
            </div>

            <div class="d-flex gap-3 align-items-center flex-wrap">
              <div class="ai-companion-box">
                <div class="ai-robot-terminal">
                  <svg viewBox="0 0 60 60" fill="none" width="38">
                    <rect x="12" y="10" width="36" height="34" rx="11" fill="white" opacity=".96"/>
                    <rect x="16" y="18" width="28" height="12" rx="6" fill="#0f172a"/>
                    <circle cx="22" cy="24" r="3.5" fill="#f59e0b">
                      <animate attributeName="opacity" values="1;0.15;1" dur="3s" repeatCount="indefinite"/>
                    </circle>
                    <circle cx="38" cy="24" r="3.5" fill="#f59e0b">
                      <animate attributeName="opacity" values="1;0.15;1" dur="3s" begin="0.4s" repeatCount="indefinite"/>
                    </circle>
                    <circle cx="30" cy="36" r="4" fill="#f59e0b" opacity="0.5">
                      <animate attributeName="r" values="3;6;3" dur="2s" repeatCount="indefinite"/>
                      <animate attributeName="opacity" values="0.5;0;0.5" dur="2s" repeatCount="indefinite"/>
                    </circle>
                  </svg>
                </div>
                <div>
                  <div class="ai-name">{{ t('statistiquesView.aiName') }}</div>
                  <div class="ai-mode">{{ t('statistiquesView.aiMode') }}</div>
                </div>
              </div>

              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'overview' }]"
                  @click="viewMode = 'overview'"
                  :title="t('statistiquesView.views.overview')">
                  <i class="fa-solid fa-chart-pie"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'analytics' }]"
                  @click="viewMode = 'analytics'"
                  :title="t('statistiquesView.views.analytics')">
                  <i class="fa-solid fa-chart-simple"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'system' }]"
                  @click="viewMode = 'system'"
                  :title="t('statistiquesView.views.system')">
                  <i class="fa-solid fa-server"></i>
                </button>
              </div>

              <button class="btn-refresh-pro" @click="refreshData" :disabled="refreshing"
                :title="t('statistiquesView.refresh')">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': refreshing }"></i>
              </button>
            </div>
          </header>

          <!-- ═══════════════════════════════════════════
               KPI STATS
          ═══════════════════════════════════════════ -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in masterKpis" :key="stat.labelKey">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">
                    <span v-if="!loading">{{ stat.value }}</span>
                    <span v-else class="skeleton-val"></span>
                  </div>
                  <div class="stat-label">{{ t(stat.labelKey) }}</div>
                </div>
                <div v-if="stat.trend !== undefined" class="stat-trend ms-auto"
                  :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
                  <i :class="stat.trend >= 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ Math.abs(stat.trend) }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════
               OVERVIEW VIEW
          ═══════════════════════════════════════════ -->
          <div v-if="viewMode === 'overview'" class="animate__animated animate__fadeIn">

            <!-- CHART + TALENTS -->
            <div class="row g-4 mb-4">
              <div class="col-lg-8">
                <div class="enigma-card p-4 h-100">
                  <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
                    <div>
                      <h6 class="fw-800 m-0">{{ t('statistiquesView.overview.chartTitle') }}</h6>
                      <p class="text-muted small m-0">{{ t('statistiquesView.overview.chartSubtitle') }}</p>
                    </div>
                    <div class="time-range-picker">
                      <button :class="['trp-btn', { active: timeRange === '24h' }]" @click="timeRange = '24h'">
                        {{ t('statistiquesView.overview.ranges.h24') }}
                      </button>
                      <button :class="['trp-btn', { active: timeRange === '7j' }]"  @click="timeRange = '7j'">
                        {{ t('statistiquesView.overview.ranges.d7') }}
                      </button>
                      <button :class="['trp-btn', { active: timeRange === '30j' }]" @click="timeRange = '30j'">
                        {{ t('statistiquesView.overview.ranges.d30') }}
                      </button>
                    </div>
                  </div>

                  <div class="chart-stage">
                    <div class="stage-grid-bg"></div>
                    <div v-if="loading" class="chart-loader"><div class="spinner-pro-premium"></div></div>
                    <div v-else-if="campagnes.length === 0" class="chart-empty text-center text-muted py-5">
                      <i class="fa-solid fa-chart-bar fa-2x mb-2"></i>
                      <p class="small fw-700">{{ t('statistiquesView.overview.noData') }}</p>
                    </div>
                    <div v-else class="glow-pillars-container">
                      <div v-for="(item, i) in campagnes" :key="i" class="pillar-group">
                        <div class="pillar-value">{{ Math.round(item.scoreMoyen) }}%</div>
                        <div class="pillar-vessel">
                          <div class="pillar-fill"
                            :style="{ height: item.scoreMoyen + '%', background: getPillarColor(item.scoreMoyen) }">
                            <div class="pillar-light-beam"></div>
                          </div>
                        </div>
                        <span class="pillar-label" :title="item.nom">{{ item.nom }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="col-lg-4">
                <div class="enigma-card p-4 mb-4">
                  <h6 class="fw-800 mb-4">{{ t('statistiquesView.overview.infra') }}</h6>
                  <div class="resource-gauges">
                    <div class="gauge-item-pro mb-4" v-for="res in resources" :key="res.nameKey">
                      <div class="d-flex justify-content-between align-items-center mb-2">
                        <span class="gauge-name">{{ t(res.nameKey) }}</span>
                        <span class="gauge-val" :style="{ color: getGaugeColor(res.usage) }">{{ res.usage }}%</span>
                      </div>
                      <div class="g-track">
                        <div class="g-fill-pro"
                          :style="{ width: res.usage + '%', background: getGaugeColor(res.usage) }"></div>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-3">
                    <i class="fa-solid fa-trophy me-2 text-amber"></i>
                    {{ t('statistiquesView.overview.topTalents') }}
                  </h6>
                  <div v-if="loading">
                    <div v-for="i in 3" :key="i" class="skeleton-talent mb-2"></div>
                  </div>
                  <div v-else-if="talentsDetectes.length === 0"
                    class="text-center text-muted py-5 small">
                    <i class="fa-solid fa-user-slash fa-2x mb-3 opacity-50 d-block"></i>
                    {{ t('statistiquesView.overview.noTalents') }}
                  </div>
                  <div v-else class="talent-row-pro"
                    v-for="(talent, idx) in talentsDetectes.slice(0, 5)" :key="idx">
                    <div class="talent-rank"
                      :class="idx === 0 ? 'rank-gold' : idx === 1 ? 'rank-silver' : 'rank-bronze'">
                      {{ idx + 1 }}
                    </div>
                    <div class="talent-details">
                      <span class="t-name">{{ talent.nomComplet }}</span>
                      <span class="t-meta">{{ talent.campagne }}</span>
                    </div>
                    <div class="t-score-badge">{{ Math.round(talent.score) }}%</div>
                  </div>
                </div>
              </div>
            </div>

            <!-- DONUT + ACTIVITY -->
            <div class="row g-4 mb-4">
              <div class="col-lg-4">
                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-4">{{ t('statistiquesView.overview.donutTitle') }}</h6>
                  <div class="donut-chart-container">
                    <svg viewBox="0 0 120 120" width="130">
                      <circle v-for="(seg, i) in orgDonutSegments" :key="i"
                        cx="60" cy="60" r="45"
                        :stroke="seg.color" stroke-width="20" fill="none"
                        :stroke-dasharray="`${seg.dash} ${283 - seg.dash}`"
                        :stroke-dashoffset="seg.offset"
                        style="transition: stroke-dasharray 0.8s ease"/>
                      <text x="60" y="58" text-anchor="middle" class="donut-center-text">
                        {{ statsData.totalEntreprises || 0 }}
                      </text>
                      <text x="60" y="72" text-anchor="middle" class="donut-sub-text">
                        {{ t('statistiquesView.overview.donutOrgs') }}
                      </text>
                    </svg>
                    <div class="donut-legend">
                      <div v-for="seg in orgDonutSegments" :key="seg.labelKey" class="donut-legend-item">
                        <span class="legend-dot-sm" :style="{ background: seg.color }"></span>
                        <span class="small">{{ t(seg.labelKey) }}</span>
                        <span class="ms-auto fw-800 small">{{ seg.count }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="col-lg-8">
                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-4">{{ t('statistiquesView.overview.activityTitle') }}</h6>
                  <div class="bar-chart-v2">
                    <div v-for="(bar, i) in weekActivityData" :key="i" class="bar-col">
                      <div class="bar-wrap">
                        <div class="bar-fill bar-amber" :style="{ height: bar.sessions + '%' }"></div>
                        <div class="bar-fill bar-indigo" :style="{ height: bar.users + '%' }"></div>
                      </div>
                      <span class="bar-label">{{ bar.label }}</span>
                    </div>
                  </div>
                  <div class="d-flex gap-3 mt-3 justify-content-center">
                    <div class="d-flex align-items-center gap-2">
                      <span class="legend-dot dot-amber"></span>
                      <span class="small text-muted fw-700">{{ t('statistiquesView.overview.legendSessions') }}</span>
                    </div>
                    <div class="d-flex align-items-center gap-2">
                      <span class="legend-dot dot-indigo"></span>
                      <span class="small text-muted fw-700">{{ t('statistiquesView.overview.legendUsers') }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- NEURAL FOOTER -->
            <div class="neural-footer-bar">
              <div class="feed-label">{{ t('statistiquesView.neural.stream') }}</div>
              <div class="feed-ticker" v-if="lastAudit">
                <span class="feed-time">[{{ lastSyncTime }}]</span>
                <span class="feed-user">{{ lastAudit.utilisateur }}</span>
                <i class="fa-solid fa-chevron-right mx-2 opacity-50"></i>
                <span class="feed-action">{{ lastAudit.action }}</span>
                <span class="mx-1 opacity-50">:</span>
                <span class="feed-detail">{{ lastAudit.details }}</span>
              </div>
              <div class="feed-ticker" v-else>{{ t('statistiquesView.neural.waiting') }}</div>
              <div class="feed-time-badge">{{ lastSyncTime || '--:--' }}</div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════
               ANALYTICS VIEW
          ═══════════════════════════════════════════ -->
          <div v-if="viewMode === 'analytics'" class="animate__animated animate__fadeIn">
            <div class="row g-4 mb-4">
              <div class="col-12">
                <div class="enigma-card p-4">
                  <div class="d-flex justify-content-between align-items-center mb-4">
                    <h6 class="fw-800 m-0">{{ t('statistiquesView.analytics.chartTitle') }}</h6>
                    <div class="d-flex gap-2 align-items-center">
                      <span class="legend-dot dot-amber"></span>
                      <span class="small text-muted">{{ t('statistiquesView.overview.legendSessions') }}</span>
                      <span class="legend-dot dot-indigo ms-3"></span>
                      <span class="small text-muted">{{ t('statistiquesView.overview.legendUsers') }}</span>
                      <span class="legend-dot dot-green ms-3"></span>
                      <span class="small text-muted">{{ t('statistiquesView.analytics.legendScore') }}</span>
                    </div>
                  </div>
                  <div class="bar-chart-v2 extended">
                    <div v-for="(bar, i) in analyticsChartData" :key="i" class="bar-col">
                      <div class="bar-wrap tri">
                        <div class="bar-fill bar-amber"  :style="{ height: bar.sessions + '%' }"></div>
                        <div class="bar-fill bar-indigo" :style="{ height: bar.users + '%' }"></div>
                        <div class="bar-fill bar-green"  :style="{ height: bar.score + '%' }"></div>
                      </div>
                      <span class="bar-label">{{ bar.label }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="row g-4">
              <div class="col-lg-6">
                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-4">{{ t('statistiquesView.analytics.metricsTitle') }}</h6>
                  <div class="metrics-table">
                    <div class="metric-row" v-for="m in detailedMetrics" :key="m.labelKey">
                      <div class="metric-icon-box" :style="{ background: m.bg, color: m.color }">
                        <i :class="m.icon"></i>
                      </div>
                      <div class="metric-info">
                        <span class="metric-name">{{ t(m.labelKey) }}</span>
                        <div class="metric-progress-thin">
                          <div :style="{ width: m.pct + '%', background: m.color }"></div>
                        </div>
                      </div>
                      <div class="metric-val fw-800">{{ m.value }}</div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-lg-6">
                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-4">{{ t('statistiquesView.analytics.logsTitle') }}</h6>
                  <div class="activity-timeline" v-if="auditLogs.length > 0">
                    <div class="timeline-item" v-for="log in auditLogs.slice(0,5)" :key="log.id">
                      <div class="tl-dot" :style="{ background: getAuditColor(log.action) }"></div>
                      <div class="tl-content">
                        <strong class="small">{{ log.utilisateur }}</strong>
                        <p class="text-muted m-0" style="font-size:0.72rem">
                          {{ log.action }} — {{ log.details }}
                        </p>
                      </div>
                      <span class="tl-time">{{ formatTime(log.timestamp) }}</span>
                    </div>
                  </div>
                  <div v-else-if="loading" class="text-center py-4">
                    <div class="spinner-pro-premium"></div>
                  </div>
                  <div v-else class="text-center text-muted py-4 small">
                    {{ t('statistiquesView.analytics.noLogs') }}
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════
               SYSTEM VIEW
          ═══════════════════════════════════════════ -->
          <div v-if="viewMode === 'system'" class="animate__animated animate__fadeIn">
            <div class="row g-4 mb-4">
              <div class="col-12">
                <div class="system-health-banner p-4"
                  :class="systemStatus === t('statistiquesView.status.optimal') ? 'banner-ok' : 'banner-warn'">
                  <div class="d-flex align-items-center gap-4">
                    <div class="health-icon-ring">
                      <i class="fa-solid fa-shield-halved fa-2x"
                        :class="systemStatus === t('statistiquesView.status.optimal') ? 'text-success' : 'text-amber'">
                      </i>
                    </div>
                    <div>
                      <h5 class="fw-900 m-0">
                        {{ t('statistiquesView.system.statusTitle') }} {{ systemStatus }}
                      </h5>
                      <p class="m-0 text-muted small">
                        {{ t('statistiquesView.system.lastCheck') }} {{ lastSyncTime }}
                      </p>
                    </div>
                    <div class="ms-auto d-flex gap-4">
                      <div class="sys-stat-box text-center">
                        <div class="sys-stat-val fw-900">{{ responseTime }}ms</div>
                        <div class="sys-stat-label">{{ t('statistiquesView.system.latency') }}</div>
                      </div>
                      <div class="sys-stat-box text-center">
                        <div class="sys-stat-val fw-900">{{ systemUptime }}%</div>
                        <div class="sys-stat-label">{{ t('statistiquesView.system.uptime') }}</div>
                      </div>
                      <div class="sys-stat-box text-center">
                        <div class="sys-stat-val fw-900">v6.5</div>
                        <div class="sys-stat-label">{{ t('statistiquesView.system.version') }}</div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <div class="row g-4">
              <div class="col-lg-12">
                <div class="enigma-card p-4">
                  <h6 class="fw-800 mb-4">
                    <i class="fa-solid fa-server me-2 text-amber"></i>
                    {{ t('statistiquesView.system.serverTitle') }}
                  </h6>
                  <div class="gauge-item-pro mb-4" v-for="res in extendedResources" :key="res.nameKey">
                    <div class="d-flex justify-content-between align-items-center mb-2">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="res.icon + ' text-muted'" style="font-size:0.75rem"></i>
                        <span class="gauge-name">{{ t(res.nameKey) }}</span>
                      </div>
                      <span class="gauge-val fw-900" :style="{ color: getGaugeColor(res.usage) }">
                        {{ res.usage }}%
                      </span>
                    </div>
                    <div class="g-track">
                      <div class="g-fill-pro"
                        :style="{ width: res.usage + '%', background: getGaugeColor(res.usage) }"></div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- AUDIT TABLE -->
              <div class="col-12">
                <div class="enigma-card p-4">
                  <div class="d-flex justify-content-between align-items-center mb-4">
                    <h6 class="fw-800 m-0">
                      <i class="fa-solid fa-list-check me-2 text-amber"></i>
                      {{ t('statistiquesView.system.auditTitle') }}
                    </h6>
                    <div class="search-inline-box" style="width:240px">
                      <i class="fa-solid fa-magnifying-glass"></i>
                      <input type="text" v-model="auditSearch"
                        :placeholder="t('statistiquesView.system.auditSearch')"
                        class="search-inline-input">
                    </div>
                  </div>
                  <div v-if="loading" class="text-center py-4">
                    <div class="spinner-pro-premium"></div>
                  </div>
                  <div v-else>
                    <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
                      <span style="width:160px" class="list-col-label">
                        {{ t('statistiquesView.system.cols.user') }}
                      </span>
                      <span class="flex-grow-1 list-col-label">
                        {{ t('statistiquesView.system.cols.action') }}
                      </span>
                      <span style="width:200px" class="list-col-label">
                        {{ t('statistiquesView.system.cols.details') }}
                      </span>
                      <span style="width:120px" class="list-col-label text-center">
                        {{ t('statistiquesView.system.cols.timestamp') }}
                      </span>
                    </div>
                    <div v-if="filteredAuditLogs.length === 0"
                      class="text-center text-muted py-4 small">
                      {{ t('statistiquesView.system.noLogs') }}
                    </div>
                    <div v-else v-for="log in filteredAuditLogs" :key="log.id"
                      class="list-row-item d-flex align-items-center px-4 py-3 mb-2">
                      <div style="width:160px">
                        <div class="fw-800 small">{{ log.utilisateur }}</div>
                      </div>
                      <div class="flex-grow-1">
                        <span class="audit-action-badge" :class="getAuditBadgeClass(log.action)">
                          {{ log.action }}
                        </span>
                      </div>
                      <div style="width:200px" class="small text-muted text-truncate">{{ log.details }}</div>
                      <div style="width:120px" class="text-center small text-muted">
                        {{ formatTime(log.timestamp) }}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
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
          <strong>{{ t('dashboard.toast.systemMessage') }}</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t } = useI18n();

const loading       = ref(true);
const refreshing    = ref(false);
const viewMode      = ref('overview');
const timeRange     = ref('7j');
const lastSyncTime  = ref('');
const systemStatus   = ref('');
const responseTime   = ref(12);
const systemUptime   = ref('99.8');
const auditSearch    = ref('');

const statsData      = ref({});
const leaders        = ref([]);
const auditLogs      = ref([]);
const mousePos       = reactive({ x: 0, y: 0 });
const globalToast    = reactive({ active: false, message: '', type: '', icon: '' });
const lastAudit      = ref(null);

const campagnes      = ref([]);
const talentsDetectes = ref([]);

const weekActivityData = ref([]);

const analyticsChartData = ref([]);

const masterKpis = ref([
  { labelKey: 'statistiquesView.kpis.orgsActives', value: '—', icon: 'fa-solid fa-building-shield',
    color: '#f59e0b', bg: '#fffbeb', trend: 12 },
  { labelKey: 'statistiquesView.kpis.talentsAi',   value: '—', icon: 'fa-solid fa-microchip',
    color: '#6366f1', bg: '#eef2ff', trend: 8 },
  { labelKey: 'statistiquesView.kpis.sessionsFlow', value: '—', icon: 'fa-solid fa-wave-square',
    color: '#10b981', bg: '#ecfdf5', trend: -2 },
  { labelKey: 'statistiquesView.kpis.securite',     value: 'MAX', icon: 'fa-solid fa-shield-halved',
    color: '#f43f5e', bg: '#fff1f2', trend: 5 },
]);

const resources = ref([
  { nameKey: 'statistiquesView.system.resources.cpu',     usage: 0 },
  { nameKey: 'statistiquesView.system.resources.ram',     usage: 0 },
  { nameKey: 'statistiquesView.system.resources.disk',    usage: 0 },
  { nameKey: 'statistiquesView.system.resources.uptime',  usage: 0 },
]);

const extendedResources = ref([
  { nameKey: 'statistiquesView.system.resources.cpu',     usage: 0,  icon: 'fa-solid fa-microchip' },
  { nameKey: 'statistiquesView.system.resources.ram',     usage: 0,  icon: 'fa-solid fa-memory' },
  { nameKey: 'statistiquesView.system.resources.disk',    usage: 0,  icon: 'fa-solid fa-hard-drive' },
  { nameKey: 'statistiquesView.system.resources.uptime',  usage: 0,  icon: 'fa-solid fa-server' },
]);

const detailedMetrics = computed(() => [
  { labelKey: 'statistiquesView.analytics.metrics.tauxReussite', value: statsData.value.tauxReussite || '—',
    pct: statsData.value.tauxReussite || 74, icon: 'fa-solid fa-chart-line', color: '#10b981', bg: '#ecfdf5' },
  { labelKey: 'statistiquesView.analytics.metrics.candidatsActifs', value: statsData.value.candidatsActifs || '—',
    pct: Math.min((statsData.value.candidatsActifs / 500) * 100, 100) || 60, icon: 'fa-solid fa-user-tie', color: '#6366f1', bg: '#eef2ff' },
  { labelKey: 'statistiquesView.analytics.metrics.totalQuestions', value: statsData.value.totalQuestions || '—',
    pct: Math.min((statsData.value.totalQuestions / 1000) * 100, 100) || 45, icon: 'fa-solid fa-database', color: '#f59e0b', bg: '#fffbeb' },
  { labelKey: 'statistiquesView.analytics.metrics.sessionsTermin', value: statsData.value.sessionsTermin || '—',
    pct: statsData.value.sessionsTermin || 82, icon: 'fa-solid fa-flag-checkered', color: '#f43f5e', bg: '#fff1f2' },
]);

const orgDonutSegments = computed(() => {
  const total = statsData.value.totalEntreprises || 4;
  const circ  = 283;
  const data  = [
    { labelKey: 'statistiquesView.overview.donutActive',   count: statsData.value.activeCount || 0,   color: '#f59e0b' },
    { labelKey: 'statistiquesView.overview.donutInactive', count: statsData.value.inactiveCount || 0,  color: '#6366f1' },
    { labelKey: 'statistiquesView.overview.donutPending',  count: statsData.value.demandesEnAttente || 0, color: '#10b981' },
  ];
  let cumulative = circ / 4;
  return data.map(d => {
    const dash   = (d.count / total) * circ;
    const offset = cumulative;
    cumulative  -= dash;
    return { ...d, dash, offset };
  });
});

const filteredAuditLogs = computed(() => {
  if (!auditSearch.value) return auditLogs.value;
  const q = auditSearch.value.toLowerCase();
  return auditLogs.value.filter(l =>
    (l.utilisateur || '').toLowerCase().includes(q) ||
    (l.action || '').toLowerCase().includes(q) ||
    (l.details || '').toLowerCase().includes(q)
  );
});

const fetchData = async () => {
  loading.value = true;
  const start = Date.now();
  try {
    const [statsRes, perfRes, auditRes, healthRes, activityRes, monthlyRes] = await Promise.allSettled([
      api.get('/SuperAdmin/stats'),
      api.get('/SuperAdmin/campaign-performance', { params: { period: timeRange.value } }),
      api.get('/SuperAdmin/audit-logs'),
      api.get('/SuperAdmin/system-health'),
      api.get('/SuperAdmin/recent-activity'),
      api.get('/SuperAdmin/monthly-eval-stats'),
    ]);

    if (statsRes.status === 'fulfilled') {
      statsData.value = statsRes.value.data;
      masterKpis.value[0].value = statsRes.value.data.activeCount || '0';
      masterKpis.value[1].value = statsRes.value.data.totalUtilisateurs || '0';
      masterKpis.value[2].value = statsRes.value.data.totalTests || '0';
    }

    if (perfRes.status === 'fulfilled') {
      campagnes.value = perfRes.value.data.campagnes || [];
      talentsDetectes.value = perfRes.value.data.talentsDetectes || [];
    } else {
      campagnes.value = [];
      talentsDetectes.value = [];
    }

    if (monthlyRes.status === 'fulfilled') {
      analyticsChartData.value = monthlyRes.value.data || [];
    }

    if (auditRes.status === 'fulfilled') {
      auditLogs.value = auditRes.value.data || [];
      if (auditLogs.value.length > 0)
        lastAudit.value = auditLogs.value[auditLogs.value.length - 1];
    }

    if (healthRes.status === 'fulfilled') {
      const h = healthRes.value.data;
      systemUptime.value = h.uptime;
      resources.value = [
        { nameKey: 'statistiquesView.system.resources.cpu',     usage: h.cpu },
        { nameKey: 'statistiquesView.system.resources.ram',     usage: h.ram },
        { nameKey: 'statistiquesView.system.resources.disk',    usage: h.disk },
        { nameKey: 'statistiquesView.system.resources.uptime',  usage: h.uptime },
      ];
      extendedResources.value = [
        { nameKey: 'statistiquesView.system.resources.cpu',     usage: h.cpu,  icon: 'fa-solid fa-microchip' },
        { nameKey: 'statistiquesView.system.resources.ram',     usage: h.ram,  icon: 'fa-solid fa-memory' },
        { nameKey: 'statistiquesView.system.resources.disk',    usage: h.disk, icon: 'fa-solid fa-hard-drive' },
        { nameKey: 'statistiquesView.system.resources.uptime',  usage: h.uptime, icon: 'fa-solid fa-server' },
      ];
    }

    if (activityRes.status === 'fulfilled') {
      weekActivityData.value = activityRes.value.data || [];
    }

    responseTime.value = Date.now() - start;
    systemStatus.value = t('statistiquesView.status.optimal');

    if (statsRes.status === 'rejected' && perfRes.status === 'rejected') {
      systemStatus.value = t('statistiquesView.status.degraded');
      showPulseToast(t('statistiquesView.toast.offlineMode'), 'warn', 'fa-solid fa-plug-circle-xmark');
    }
  } catch (err) {
    console.error('Critical Sync Failure', err);
    systemStatus.value = t('statistiquesView.status.degraded');
    showPulseToast('Erreur de chargement des données', 'error', 'fa-solid fa-circle-exclamation');
  } finally {
    loading.value    = false;
    refreshing.value = false;
    lastSyncTime.value = new Date().toLocaleTimeString('fr-FR', {
      hour: '2-digit', minute: '2-digit', second: '2-digit',
    });
  }
};

const refreshData = async () => {
  refreshing.value = true;
  await fetchData();
  showPulseToast(t('statistiquesView.toast.refreshed'), 'success', 'fa-solid fa-rotate');
};

watch(timeRange, () => {
  fetchData();
});

const getPillarColor     = (s) => s >= 80 ? '#10b981' : s >= 60 ? '#f59e0b' : '#f43f5e';
const getGaugeColor      = (p) => p >= 90 ? '#f43f5e' : p >= 70 ? '#f59e0b' : '#10b981';
const getAuditColor      = (a) => ({ LOGIN:'#10b981', CREATE:'#6366f1', DEPLOY:'#f59e0b', DELETE:'#f43f5e', EXPORT:'#06b6d4' }[a] || '#94a3b8');
const getAuditBadgeClass = (a) => ({ LOGIN:'audit-login', CREATE:'audit-create', DEPLOY:'audit-deploy', DELETE:'audit-delete', EXPORT:'audit-export' }[a] || 'audit-default');
const formatTime         = (ts) => ts ? new Date(ts).toLocaleTimeString('fr-FR', { hour:'2-digit', minute:'2-digit' }) : '—';

let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(() => {
  systemStatus.value = t('statistiquesView.status.optimal');
  fetchData();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');

/* ════════════════════════════════════════
   BASE
════════════════════════════════════════ */
.enigma-master-root { min-height: 100vh; background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; color: #0f172a; }
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 1px, transparent 1px); background-size: 40px 40px; opacity: 0.2; }
.glow-orb { position: absolute; width: 600px; height: 600px; filter: blur(120px); opacity: 0.15; border-radius: 50%; transition: transform 0.3s ease-out; }
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* HEADER */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; color: #0f172a; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* STATUS PILLS */
.status-pill { display: inline-flex; align-items: center; gap: 6px; background: white; border: 1px solid #eef2f6; border-radius: 100px; padding: 4px 12px; font-size: 0.65rem; font-weight: 800; color: #64748b; }
.status-pill.live .pill-dot { width: 7px; height: 7px; background: #10b981; border-radius: 50%; animation: liveping 2s infinite; }
.status-pill.latency { color: #6366f1; border-color: #eef2ff; background: #f5f3ff; }
.status-pill.status-ok   { color: #10b981; border-color: #d1fae5; background: #f0fdf4; }
.status-pill.status-warn { color: #f59e0b; border-color: #fde68a; background: #fffbeb; }
@keyframes liveping { 0%,100%{opacity:1}50%{opacity:0.3} }

/* AI COMPANION */
.ai-companion-box { display: flex; align-items: center; gap: 12px; background: white; border: 1.5px solid #eef2f6; border-radius: 18px; padding: 10px 16px; }
.ai-robot-terminal { width: 46px; height: 46px; background: #0f172a; border-radius: 14px; display: flex; align-items: center; justify-content: center; }
.ai-name { font-weight: 900; font-size: 0.72rem; color: #0f172a; letter-spacing: 0.5px; }
.ai-mode { font-size: 0.6rem; font-weight: 800; color: #f59e0b; }

/* CONTROLS */
.view-toggle-cluster { display: flex; background: white; border: 1.5px solid #e2e8f0; border-radius: 16px; padding: 4px; gap: 4px; }
.btn-view-toggle { width: 38px; height: 38px; border-radius: 12px; border: none; background: transparent; color: #94a3b8; transition: 0.3s; display: flex; align-items: center; justify-content: center; cursor: pointer; }
.btn-view-toggle:hover { background: #f8fafc; color: #0f172a; }
.btn-view-toggle.active { background: #0f172a; color: #f59e0b; }
.btn-refresh-pro { width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0; border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s; display: flex; align-items: center; justify-content: center; }
.btn-refresh-pro:hover:not(:disabled) { background: #f8fafc; border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg) scale(1.1); }

/* STAT CARDS */
.stat-card-premium { background: white; border-radius: 24px; padding: 24px; display: flex; align-items: center; border: 1px solid #eef2f6; transition: 0.2s; }
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper { width: 56px; height: 56px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.4rem; flex-shrink: 0; }
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up   { color: #10b981; }
.trend-down { color: #f43f5e; }
.skeleton-val { display: inline-block; width: 60px; height: 28px; background: linear-gradient(90deg,#f1f5f9 25%,#e2e8f0 50%,#f1f5f9 75%); background-size: 200% 100%; border-radius: 8px; animation: shimmerSkel 1.2s infinite; }
@keyframes shimmerSkel { to { background-position: -200% 0; } }

/* ENIGMA CARD */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; }

/* PILLAR CHART */
.chart-stage { height: 300px; position: relative; overflow: hidden; display: flex; align-items: flex-end; padding: 30px 20px 20px; }
.stage-grid-bg { position: absolute; inset: 0; background-image: linear-gradient(#eef2f6 1px,transparent 1px),linear-gradient(90deg,#eef2f6 1px,transparent 1px); background-size: 30px 30px; opacity: 0.5; }
.chart-loader  { position: absolute; inset: 0; display: flex; align-items: center; justify-content: center; }
.chart-empty   { flex:1; display:flex; flex-direction:column; align-items:center; justify-content:center; position:relative; z-index:2; }
.glow-pillars-container { display: flex; align-items: flex-end; justify-content: space-around; width: 100%; height: 100%; position: relative; z-index: 2; }
.pillar-group  { display: flex; flex-direction: column; align-items: center; gap: 10px; }
.pillar-value  { font-size: 0.7rem; font-weight: 900; color: #0f172a; }
.pillar-vessel { width: 42px; height: 200px; background: rgba(0,0,0,0.03); border-radius: 100px; position: relative; overflow: hidden; }
.pillar-fill   { width: 100%; position: absolute; bottom: 0; border-radius: 100px; transition: height 1.5s cubic-bezier(0.19,1,0.22,1); }
.pillar-light-beam { position: absolute; top: 0; left: 15%; width: 25%; height: 100%; background: rgba(255,255,255,0.25); filter: blur(3px); }
.pillar-label  { font-size: 0.6rem; font-weight: 800; color: #94a3b8; text-transform: uppercase; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 65px; text-align: center; }

/* TIME RANGE PICKER */
.time-range-picker { display: flex; background: #f8fafc; border-radius: 12px; padding: 4px; gap: 2px; border: 1px solid #eef2f6; }
.trp-btn { padding: 5px 14px; border-radius: 9px; border: none; background: transparent; font-size: 0.72rem; font-weight: 800; color: #94a3b8; cursor: pointer; transition: 0.2s; font-family: inherit; }
.trp-btn.active { background: #0f172a; color: white; }

/* RESOURCE GAUGES */
.gauge-name { font-size: 0.75rem; font-weight: 700; color: #475569; }
.gauge-val  { font-size: 0.75rem; font-weight: 800; }
.g-track    { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.g-fill-pro { height: 100%; border-radius: 10px; transition: width 0.8s ease; }

/* TALENT STREAM */
.talent-row-pro { display: flex; align-items: center; gap: 12px; padding: 10px 14px; background: #f8fafc; border-radius: 14px; margin-bottom: 8px; transition: 0.2s; border: 1px solid transparent; }
.talent-row-pro:hover { border-color: #f59e0b; background: white; transform: translateX(3px); }
.talent-rank { width: 28px; height: 28px; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 0.75rem; flex-shrink: 0; }
.rank-gold   { background: #fffbeb; color: #f59e0b; }
.rank-silver { background: #f8fafc; color: #64748b; }
.rank-bronze { background: #fff7f2; color: #ea580c; }
.talent-details { flex: 1; display: flex; flex-direction: column; }
.t-name { font-weight: 800; font-size: 0.82rem; color: #0f172a; }
.t-meta { font-size: 0.62rem; color: #94a3b8; font-weight: 600; }
.t-score-badge { background: #0f172a; color: #f59e0b; padding: 4px 10px; border-radius: 8px; font-weight: 900; font-size: 0.72rem; }
.skeleton-talent { height: 46px; background: linear-gradient(90deg,#f1f5f9 25%,#e2e8f0 50%,#f1f5f9 75%); background-size: 200% 100%; border-radius: 14px; animation: shimmerSkel 1.2s infinite; }

/* DONUT */
.donut-chart-container { display: flex; align-items: center; gap: 20px; }
.donut-legend          { display: flex; flex-direction: column; gap: 10px; flex: 1; }
.donut-legend-item     { display: flex; align-items: center; gap: 8px; }
.legend-dot-sm  { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.donut-center-text { font-size: 22px; font-weight: 900; fill: #0f172a; }
.donut-sub-text    { font-size: 8px; fill: #94a3b8; font-weight: 700; }

/* BAR CHART */
.bar-chart-v2 { display: flex; align-items: flex-end; gap: 8px; height: 140px; }
.bar-chart-v2.extended { height: 180px; gap: 6px; }
.bar-col  { display: flex; flex-direction: column; align-items: center; gap: 6px; flex: 1; }
.bar-wrap { display: flex; gap: 3px; align-items: flex-end; height: 100%; width: 100%; justify-content: center; }
.bar-wrap.tri { gap: 2px; }
.bar-fill { width: 12px; border-radius: 6px 6px 0 0; transition: height 0.8s ease; min-height: 4px; cursor: pointer; }
.bar-fill:hover { opacity: 0.8; }
.bar-amber  { background: #f59e0b; }
.bar-indigo { background: #6366f1; }
.bar-green  { background: #10b981; }
.bar-label  { font-size: 0.6rem; font-weight: 800; color: #94a3b8; }
.legend-dot { width: 8px; height: 8px; border-radius: 50%; display: inline-block; }
.dot-amber  { background: #f59e0b; }
.dot-indigo { background: #6366f1; }
.dot-green  { background: #10b981; }

/* METRICS TABLE */
.metrics-table { display: flex; flex-direction: column; gap: 14px; }
.metric-row    { display: flex; align-items: center; gap: 14px; }
.metric-icon-box { width: 40px; height: 40px; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 1rem; flex-shrink: 0; }
.metric-info   { flex: 1; }
.metric-name   { font-size: 0.78rem; font-weight: 700; color: #475569; display: block; margin-bottom: 6px; }
.metric-progress-thin { height: 5px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.metric-progress-thin div { height: 100%; border-radius: 10px; transition: width 0.8s ease; }
.metric-val  { font-size: 1rem; font-weight: 900; color: #0f172a; flex-shrink: 0; min-width: 48px; text-align: right; }

/* TIMELINE */
.activity-timeline { display: flex; flex-direction: column; }
.timeline-item { display: flex; align-items: flex-start; gap: 14px; padding: 12px 0; border-bottom: 1px solid #f1f5f9; }
.tl-dot    { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; margin-top: 5px; }
.tl-content{ flex: 1; }
.tl-time   { font-size: 0.62rem; color: #94a3b8; font-weight: 700; white-space: nowrap; }

/* SYSTEM VIEW */
.system-health-banner { border-radius: 28px; border: 2px solid; }
.banner-ok   { background: #f0fdf4; border-color: #bbf7d0; }
.banner-warn { background: #fffbeb; border-color: #fde68a; }
.health-icon-ring { width: 60px; height: 60px; background: white; border-radius: 18px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.sys-stat-box   { background: white; border-radius: 14px; padding: 12px 16px; border: 1px solid #eef2f6; }
.sys-stat-val   { font-size: 1.1rem; font-weight: 900; color: #0f172a; }
.sys-stat-label { font-size: 0.55rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; text-transform: uppercase; }

/* AUDIT BADGES */
.audit-action-badge { font-size: 0.62rem; font-weight: 900; padding: 3px 10px; border-radius: 8px; text-transform: uppercase; }
.audit-login  { background: #ecfdf5; color: #10b981; }
.audit-create { background: #eef2ff; color: #6366f1; }
.audit-deploy { background: #fffbeb; color: #f59e0b; }
.audit-delete { background: #fff1f2; color: #f43f5e; }
.audit-export { background: #f0fdfa; color: #0d9488; }
.audit-default{ background: #f1f5f9; color: #64748b; }

/* LIST */
.list-header-row { background: #f8fafc; border-radius: 14px; }
.list-col-label  { font-size: 0.6rem; font-weight: 900; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
.list-row-item   { background: white; border-radius: 16px; border: 1px solid #eef2f6; transition: 0.2s; }
.list-row-item:hover { border-color: #f59e0b; }
.search-inline-box   { display: flex; align-items: center; background: white; border: 1.5px solid #eef2f6; border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8; }
.search-inline-input { border: none; outline: none; background: transparent; padding: 10px 0; font-weight: 700; font-size: 0.82rem; flex: 1; font-family: inherit; }

/* NEURAL FOOTER */
.neural-footer-bar { background: #0f172a; color: white; padding: 14px 28px; border-radius: 22px; display: flex; align-items: center; gap: 20px; font-size: 0.72rem; margin-top: 8px; }
.feed-label { background: #f59e0b; color: #0f172a; padding: 3px 10px; border-radius: 8px; font-weight: 900; font-size: 0.6rem; flex-shrink: 0; }
.feed-ticker { flex: 1; display: flex; align-items: center; gap: 6px; flex-wrap: wrap; }
.feed-time   { color: #64748b; font-weight: 700; }
.feed-user   { color: #f59e0b; font-weight: 800; }
.feed-action { color: white; font-weight: 700; }
.feed-detail { color: #94a3b8; font-weight: 600; }
.feed-time-badge { background: rgba(255,255,255,0.08); padding: 4px 12px; border-radius: 8px; font-size: 0.65rem; font-weight: 800; color: #94a3b8; flex-shrink: 0; }

/* MISC */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #eef2f6; border-radius: 10px; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.text-amber  { color: #f59e0b !important; }
.text-success{ color: #10b981 !important; }
.opacity-50  { opacity: 0.5; }

/* SPINNER */
.spinner-pro-premium { width: 46px; height: 46px; border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; margin: auto; }
@keyframes spin { to { transform: rotate(360deg); } }

/* TOAST */
.enigma-toast { position: fixed; bottom: 30px; right: 30px; background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px; display: flex; align-items: center; gap: 15px; z-index: 3000; border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.2); }
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from{transform:translateX(120%);opacity:0}to{transform:translateX(0);opacity:1} }

/* ════════════════════════════════════════════
   DARK MODE
════════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .stat-card-premium { background: rgba(22,27,34,0.7); border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }
[data-theme="dark"] .stat-label { color: #8b949e; }
[data-theme="dark"] .status-pill { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .status-pill.latency { background: rgba(99,102,241,0.1); color: #a5b4fc; border-color: rgba(99,102,241,0.2); }
[data-theme="dark"] .status-pill.status-ok { background: rgba(16,185,129,0.1); color: #34d399; border-color: rgba(16,185,129,0.2); }
[data-theme="dark"] .status-pill.status-warn { background: rgba(245,158,11,0.1); color: #fbbf24; border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .ai-companion-box { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .ai-name { color: #f0f6fc; }
[data-theme="dark"] .view-toggle-cluster { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .btn-view-toggle { color: #8b949e; }
[data-theme="dark"] .btn-view-toggle:hover { background: rgba(255,255,255,0.05); color: #f0f6fc; }
[data-theme="dark"] .btn-view-toggle.active { background: #0d1117; color: #f59e0b; }
[data-theme="dark"] .btn-refresh-pro { background: #161b22; border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .btn-refresh-pro:hover:not(:disabled) { border-color: #f59e0b; color: #f59e0b; background: rgba(245,158,11,0.08); }
[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .enigma-card h6 { color: #f0f6fc; }
[data-theme="dark"] .pillar-value { color: #f0f6fc; }
[data-theme="dark"] .stage-grid-bg { background-image: linear-gradient(#21262d 1px,transparent 1px),linear-gradient(90deg,#21262d 1px,transparent 1px); opacity: 0.8; }
[data-theme="dark"] .pillar-vessel { background: rgba(255,255,255,0.04); }
[data-theme="dark"] .time-range-picker { background: #0d1117; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .trp-btn { color: #8b949e; }
[data-theme="dark"] .trp-btn.active { background: #f59e0b; color: #0d1117; }
[data-theme="dark"] .g-track { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .gauge-name { color: #8b949e; }
[data-theme="dark"] .talent-row-pro { background: #0d1117; border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .talent-row-pro:hover { background: rgba(255,255,255,0.03); border-color: #d97706; }
[data-theme="dark"] .t-name { color: #f0f6fc; }
[data-theme="dark"] .t-score-badge { background: #f59e0b; color: #0d1117; }
[data-theme="dark"] .rank-gold   { background: rgba(245,158,11,0.15); color: #fbbf24; }
[data-theme="dark"] .rank-silver { background: rgba(255,255,255,0.06); color: #94a3b8; }
[data-theme="dark"] .rank-bronze { background: rgba(234,88,12,0.12); color: #fb923c; }
[data-theme="dark"] .donut-center-text { fill: #f0f6fc; }
[data-theme="dark"] .donut-sub-text    { fill: #8b949e; }
[data-theme="dark"] .metric-val  { color: #f0f6fc; }
[data-theme="dark"] .metric-name { color: #8b949e; }
[data-theme="dark"] .metric-progress-thin { background: rgba(255,255,255,0.06); }
[data-theme="dark"] .timeline-item { border-bottom-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .tl-content strong { color: #f0f6fc; }
[data-theme="dark"] .system-health-banner.banner-ok   { background: rgba(16,185,129,0.08); border-color: rgba(16,185,129,0.25); }
[data-theme="dark"] .system-health-banner.banner-warn { background: rgba(245,158,11,0.08); border-color: rgba(245,158,11,0.25); }
[data-theme="dark"] .health-icon-ring { background: rgba(255,255,255,0.05); }
[data-theme="dark"] .sys-stat-box { background: #0d1117; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .sys-stat-val { color: #f0f6fc; }
[data-theme="dark"] .list-header-row { background: rgba(255,255,255,0.03); }
[data-theme="dark"] .list-col-label  { color: #8b949e; }
[data-theme="dark"] .list-row-item   { background: #161b22; border-color: rgba(255,255,255,0.06); color: #f0f6fc; }
[data-theme="dark"] .list-row-item:hover { border-color: #d97706; background: rgba(255,255,255,0.02); }
[data-theme="dark"] .search-inline-box   { background: #0d1117; border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; background: transparent; }
[data-theme="dark"] .neural-footer-bar { background: #161b22; box-shadow: 0 4px 20px rgba(0,0,0,0.3); }
[data-theme="dark"] .feed-time  { color: #8b949e; }
[data-theme="dark"] .feed-time-badge { background: rgba(255,255,255,0.06); color: #8b949e; }
[data-theme="dark"] .bar-label  { color: #8b949e; }
[data-theme="dark"] .skeleton-val,
[data-theme="dark"] .skeleton-talent { background: linear-gradient(90deg,#21262d 25%,#30363d 50%,#21262d 75%); background-size: 200% 100%; }

.animate__animated { animation-fill-mode: both; }
.animate__fadeIn { animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from{opacity:0;transform:translateY(8px)}to{opacity:1;transform:none} }
</style>
