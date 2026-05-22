<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1 p-4">
      <AppNavbar />
      
      <main class="mt-4 animate-fade-in">
        <!-- HEADER WITH CONTROLS -->
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
          <div>
            <div class="breadcrumb-pro mb-2">
              <span class="root">Administration</span>
              <i class="fa-solid fa-chevron-right mx-2 separator"></i>
              <span class="current">Analytics Global</span>
            </div>
            <h2 class="premium-title mb-1">
              Analytics <span class="gradient-text">Global</span>
            </h2>
            <p class="text-muted tiny mb-0">Statistiques en temps réel et performance financière de la plateforme.</p>
          </div>
          
          <div class="d-flex align-items-center gap-2">
            <!-- Period Selector -->
            <div class="dropdown">
              <button class="btn-glass-pro dropdown-toggle tiny fw-bold px-3 py-2" type="button" data-bs-toggle="dropdown">
                <i class="fa-solid fa-calendar-days me-2 text-amber"></i> {{ activePeriodLabel }}
              </button>
              <ul class="dropdown-menu dropdown-menu-end shadow border-0 glass-dropdown-pro">
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('month')">
                  <i class="fa-solid fa-clock me-2 text-amber"></i>7 Derniers Jours
                </a></li>
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('quarter')">
                  <i class="fa-solid fa-calendar me-2 text-amber"></i>30 Derniers Jours
                </a></li>
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('year')">
                  <i class="fa-solid fa-chart-line me-2 text-amber"></i>Cette Année (12 mois)
                </a></li>
              </ul>
            </div>

            <!-- Export Button -->
            <button class="btn-enigma-primary-sm" @click="simulateExport" :disabled="exporting">
              <div class="btn-content-sm">
                <i v-if="exporting" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                <i v-else class="fa-solid fa-download me-2"></i> Exporter
              </div>
              <div class="btn-glow-sm"></div>
            </button>

            <!-- Refresh Button -->
            <button class="btn-refresh-pro" @click="refreshData" :class="{ 'fa-spin-active': loading }">
              <i class="fa-solid fa-rotate"></i>
            </button>
          </div>
        </div>

        <!-- LOADING OVERLAY -->
        <div v-if="loading" class="d-flex align-items-center justify-content-center py-5 my-5">
          <div class="spinner-pro-premium"></div>
        </div>

        <div v-else class="animate-fade-in">
          <!-- MASTER KPI CARDS -->
          <div class="row g-4 mb-4">
            <div class="col-md-3 col-sm-6" v-for="(stat, i) in currentStats" :key="stat.label">
              <div class="stat-card-premium p-4 relative overflow-hidden">
                <div class="stat-glow-orb" :style="{ background: stat.glowColor }"></div>
                
                <div class="d-flex justify-content-between align-items-start mb-3">
                  <div class="tiny fw-900 text-muted uppercase tracking-wider stat-label-top">{{ stat.label }}</div>
                  <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                    <i :class="stat.icon"></i>
                  </div>
                </div>

                <div class="stat-value mb-2">{{ stat.val }}</div>
                
                <div class="d-flex align-items-center gap-1 tiny">
                  <span class="trend-badge fw-bold px-2 py-1 rounded-pill" :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
                    <i :class="stat.trend >= 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                    {{ Math.abs(stat.trend) }}%
                  </span>
                  <span class="text-muted fw-semibold">vs mois dernier</span>
                </div>
              </div>
            </div>
          </div>

          <!-- CHARTS SECTION -->
          <div class="row g-4 mb-4">
            <!-- Line Chart: Revenue Evolution -->
            <div class="col-lg-8">
              <div class="enigma-card p-4 h-100">
                <div class="d-flex justify-content-between align-items-center mb-4">
                  <div>
                    <h6 class="fw-900 text-navy mb-0">Évolution des Revenus <span class="text-muted fw-600">(€)</span></h6>
                    <p class="text-muted super-tiny mb-0">Courbe de croissance récurrente mensuelle.</p>
                  </div>
                  <div class="d-flex align-items-center gap-2 tiny text-muted">
                    <span class="legend-dot-sm" style="background: #8b5cf6;"></span>
                    <span class="fw-700">Revenus récurrents</span>
                  </div>
                </div>

                <!-- Custom Line Chart SVG -->
                <div class="chart-container relative" style="height: 220px;">
                  <svg viewBox="0 0 500 240" class="w-100 h-100 overflow-visible" preserveAspectRatio="none">
                    <!-- Y-Axis Grid Lines -->
                    <line x1="40" y1="40" x2="480" y2="40" stroke="var(--border-chart)" stroke-dasharray="4" />
                    <line x1="40" y1="90" x2="480" y2="90" stroke="var(--border-chart)" stroke-dasharray="4" />
                    <line x1="40" y1="140" x2="480" y2="140" stroke="var(--border-chart)" stroke-dasharray="4" />
                    <line x1="40" y1="190" x2="480" y2="190" stroke="var(--border-chart)" />

                    <!-- Y-Axis Labels -->
                    <text x="10" y="45" font-size="9" fill="var(--text-chart)" font-weight="bold">15k</text>
                    <text x="10" y="95" font-size="9" fill="var(--text-chart)" font-weight="bold">10k</text>
                    <text x="10" y="145" font-size="9" fill="var(--text-chart)" font-weight="bold">5k</text>
                    <text x="10" y="195" font-size="9" fill="var(--text-chart)" font-weight="bold">0</text>

                    <!-- Area under the Curve -->
                    <path :d="curveAreaPath" fill="url(#area-grad)" class="area-transition" />

                    <!-- Curve Line -->
                    <path :d="curveLinePath" fill="none" stroke="url(#line-grad)" stroke-width="3.5" stroke-linecap="round" class="line-transition" />

                    <!-- Grid Definitions -->
                    <defs>
                      <linearGradient id="area-grad" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#f59e0b" stop-opacity="0.3" />
                        <stop offset="100%" stop-color="#f59e0b" stop-opacity="0.0" />
                      </linearGradient>
                      <linearGradient id="line-grad" x1="0" y1="0" x2="1" y2="0">
                        <stop offset="0%" stop-color="#3b82f6" />
                        <stop offset="50%" stop-color="#f59e0b" />
                        <stop offset="100%" stop-color="#fb923c" />
                      </linearGradient>
                    </defs>

                    <!-- Interactive Data Points -->
                    <g v-for="(pt, idx) in currentCurvePoints" :key="idx" 
                       @mouseenter="hoveredPoint = idx" 
                       @mouseleave="hoveredPoint = null"
                       class="cursor-pointer">
                      <circle :cx="pt.x" :cy="pt.y" r="5" fill="white" stroke="#f59e0b" stroke-width="3" class="transition-all chart-point" />
                      <circle :cx="pt.x" :cy="pt.y" r="14" fill="transparent" />
                    </g>
                  </svg>

                  <!-- Tooltip Float -->
                  <div v-if="hoveredPoint !== null" 
                       class="chart-tooltip-pro p-2 text-center absolute shadow"
                       :style="{ 
                         left: (currentCurvePoints[hoveredPoint].x / 5) + '%', 
                         top: (currentCurvePoints[hoveredPoint].y - 55) + 'px'
                       }">
                    <div class="super-tiny text-muted fw-800">{{ monthsLabels[hoveredPoint] }}</div>
                    <div class="tiny fw-900" style="color:#f59e0b;">{{ currentLineData[hoveredPoint] }} €</div>
                  </div>
                </div>

                <div class="d-flex justify-content-between px-4 mt-3 tiny fw-800 text-muted">
                  <span v-for="m in monthsLabels" :key="m">{{ m }}</span>
                </div>
              </div>
            </div>

            <!-- Bar Chart: Subscription Distribution -->
            <div class="col-lg-4">
              <div class="enigma-card p-4 h-100">
                <div class="mb-4">
                  <h6 class="fw-900 text-navy mb-0">Répartition des abonnements</h6>
                  <p class="text-muted super-tiny mb-0">Pourcentage et volume d'organisations par offre active.</p>
                </div>

                <!-- Custom Bar Chart SVG -->
                <div class="chart-container relative d-flex align-items-center" style="height: 220px;">
                  <svg viewBox="0 0 300 200" class="w-100 h-100 overflow-visible">
                    <defs>
                      <linearGradient id="grad-startup" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#3b82f6" />
                        <stop offset="100%" stop-color="#2563eb" />
                      </linearGradient>
                      <linearGradient id="grad-business" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#f59e0b" />
                        <stop offset="100%" stop-color="#d97706" />
                      </linearGradient>
                      <linearGradient id="grad-enterprise" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#10b981" />
                        <stop offset="100%" stop-color="#059669" />
                      </linearGradient>
                      <linearGradient id="grad-gratuit" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#94a3b8" />
                        <stop offset="100%" stop-color="#64748b" />
                      </linearGradient>
                    </defs>

                    <!-- Horizontal Grid Lines -->
                    <line x1="30" y1="40" x2="280" y2="40" stroke="var(--border-chart)" stroke-dasharray="2" />
                    <line x1="30" y1="100" x2="280" y2="100" stroke="var(--border-chart)" stroke-dasharray="2" />
                    <line x1="30" y1="160" x2="280" y2="160" stroke="var(--border-chart)" />

                    <!-- STARTUP -->
                    <rect x="25" :y="160 - (120 * currentBarScale.startup)" width="30" :height="120 * currentBarScale.startup" rx="6" fill="url(#grad-startup)" class="chart-bar" @mouseenter="hoveredBar = 'startup'" @mouseleave="hoveredBar = null" />
                    <!-- BUSINESS -->
                    <rect x="90" :y="160 - (120 * currentBarScale.business)" width="30" :height="120 * currentBarScale.business" rx="6" fill="url(#grad-business)" class="chart-bar" @mouseenter="hoveredBar = 'business'" @mouseleave="hoveredBar = null" />
                    <!-- ENTERPRISE -->
                    <rect x="155" :y="160 - (120 * currentBarScale.enterprise)" width="30" :height="120 * currentBarScale.enterprise" rx="6" fill="url(#grad-enterprise)" class="chart-bar" @mouseenter="hoveredBar = 'enterprise'" @mouseleave="hoveredBar = null" />
                    <!-- GRATUIT -->
                    <rect x="220" :y="160 - (120 * currentBarScale.gratuit)" width="30" :height="120 * currentBarScale.gratuit" rx="6" fill="url(#grad-gratuit)" class="chart-bar" @mouseenter="hoveredBar = 'gratuit'" @mouseleave="hoveredBar = null" />
                  </svg>

                  <!-- Tooltip Float for Bars -->
                  <div v-if="hoveredBar !== null" 
                       class="chart-tooltip-pro p-2 text-center absolute shadow"
                       :style="getBarTooltipStyle(hoveredBar)">
                     <div class="super-tiny text-muted fw-800 uppercase">{{ hoveredBar }}</div>
                     <div class="tiny fw-900" style="color:#f59e0b;">{{ getBarStats(hoveredBar).pct }}%</div>
                     <div class="super-tiny text-muted">{{ getBarStats(hoveredBar).count }} orgs</div>
                  </div>
                </div>

                <div class="d-flex justify-content-around mt-2 flex-wrap gap-1">
                  <span class="plan-tag tag-blue">STARTUP</span>
                  <span class="plan-tag tag-amber">BUSINESS</span>
                  <span class="plan-tag tag-emerald">ENTERPRISE</span>
                  <span class="plan-tag tag-slate">GRATUIT</span>
                </div>
              </div>
            </div>
          </div>

          <!-- DETAILED DATA TABLE -->
          <div class="enigma-card p-4">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
              <div>
                <h6 class="fw-900 text-navy mb-1">Dernières Activités Financières</h6>
                <p class="text-muted super-tiny mb-0">Historique des transactions et abonnements récemment validés.</p>
              </div>
              <button class="btn-outline-pro tiny fw-bold px-3 py-2" @click="router.push('/super-admin')">
                <i class="fa-solid fa-list me-2"></i> Gérer Organisations
              </button>
            </div>

            <div class="table-responsive">
              <table class="table custom-table-pro align-middle">
                <thead>
                  <tr>
                    <th scope="col">Organisation</th>
                    <th scope="col">Formule</th>
                    <th scope="col">Moyen de paiement</th>
                    <th scope="col">Date d'activation</th>
                    <th scope="col">Facturation</th>
                    <th scope="col" class="text-center">Statut</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="org in recentActivities" :key="org.id" class="table-row-pro">
                    <td>
                      <div class="d-flex align-items-center gap-2">
                        <div class="org-avatar fw-bold" :style="{ background: org.color + '18', color: org.color }">
                          {{ org.name[0] }}
                        </div>
                        <div>
                          <div class="fw-800 text-navy tiny">{{ org.name }}</div>
                          <div class="super-tiny text-muted">ID: {{ org.code }}</div>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span class="plan-tag" :class="getPlanTagClass(org.plan)">
                        {{ org.plan }}
                      </span>
                    </td>
                    <td>
                      <div class="d-flex align-items-center gap-2 tiny">
                        <i :class="org.paymentIcon + ' text-muted'"></i>
                        <span class="fw-700 text-muted">{{ org.paymentMethod }}</span>
                      </div>
                    </td>
                    <td class="tiny fw-800 text-navy">{{ org.date }}</td>
                    <td>
                      <span class="tiny fw-900 text-navy">{{ org.price }} €</span>
                      <span class="text-muted super-tiny fw-600">/mois</span>
                    </td>
                    <td class="text-center">
                      <span class="status-pill-pro status-active tiny">
                        <span class="status-dot-pro"></span> Actif
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useToast } from 'vue-toastification';
import { superAdminApi } from '@/services/api';

const router = useRouter();
const toast = useToast();

const loading = ref(false);
const exporting = ref(false);
const activePeriod = ref('month');
const hoveredPoint = ref(null);
const hoveredBar = ref(null);

const activePeriodLabel = computed(() => {
  if (activePeriod.value === 'month') return '7 Derniers Jours';
  if (activePeriod.value === 'quarter') return '30 Derniers Jours';
  return 'Cette Année (12 mois)';
});

const statsData = ref(null);
const recentActivities = ref([]);

const statsByPeriod = ref({
  month: [
    { label: 'Organisations', val: '0', trend: 12, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 8, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 25, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 5, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
  ],
  quarter: [
    { label: 'Organisations', val: '0', trend: 18, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 15, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 32, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 12, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
  ],
  year: [
    { label: 'Organisations', val: '0', trend: 45, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 38, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 78, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 28, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
  ]
});

const currentStats = computed(() => statsByPeriod.value[activePeriod.value]);

const curveLineDataByPeriod = ref({
  month: [4200, 6800, 5100, 9400, 8200, 14200],
  quarter: [7200, 9100, 11400, 15300, 13800, 21500],
  year: [18000, 24000, 29000, 38000, 42000, 54200]
});

const currentLineData = computed(() => curveLineDataByPeriod.value[activePeriod.value]);
const monthsLabels = ref(['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun']);

const currentCurvePoints = computed(() => {
  const data = currentLineData.value;
  const maxVal = Math.max(...data, 1000) * 1.15;
  return data.map((val, i) => ({
    x: 40 + (440 / (data.length - 1)) * i,
    y: 190 - (150 * (val / maxVal))
  }));
});

const curveLinePath = computed(() => {
  const pts = currentCurvePoints.value;
  if (pts.length < 2) return '';
  let d = `M ${pts[0].x} ${pts[0].y}`;
  for (let i = 0; i < pts.length - 1; i++) {
    const cpX = pts[i].x + (pts[i+1].x - pts[i].x) / 2;
    d += ` C ${cpX} ${pts[i].y}, ${cpX} ${pts[i+1].y}, ${pts[i+1].x} ${pts[i+1].y}`;
  }
  return d;
});

const curveAreaPath = computed(() => {
  const pts = currentCurvePoints.value;
  if (pts.length < 2) return '';
  return curveLinePath.value + ` L ${pts[pts.length - 1].x} 190 L ${pts[0].x} 190 Z`;
});

const barScaleByPeriod = ref({
  month: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 },
  quarter: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 },
  year: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 }
});

const currentBarScale = computed(() => barScaleByPeriod.value[activePeriod.value]);

function getBarTooltipStyle(bar) {
  const leftMap = { startup: '18%', business: '38%', enterprise: '58%', gratuit: '78%' };
  return { left: leftMap[bar] || '50%', top: '10px', transform: 'translateX(-50%)', zIndex: 10 };
}

const planStats = ref({
  month: { startup: { pct: 0, count: 0 }, business: { pct: 0, count: 0 }, enterprise: { pct: 0, count: 0 }, gratuit: { pct: 0, count: 0 } },
  quarter: { startup: { pct: 0, count: 0 }, business: { pct: 0, count: 0 }, enterprise: { pct: 0, count: 0 }, gratuit: { pct: 0, count: 0 } },
  year: { startup: { pct: 0, count: 0 }, business: { pct: 0, count: 0 }, enterprise: { pct: 0, count: 0 }, gratuit: { pct: 0, count: 0 } }
});

function getBarStats(bar) { return planStats.value[activePeriod.value][bar]; }

function getPlanTagClass(plan) {
  if (plan === 'ENTERPRISE') return 'tag-emerald';
  if (plan === 'BUSINESS') return 'tag-amber';
  if (plan === 'GRATUIT') return 'tag-slate';
  return 'tag-blue';
}

async function loadRealStats() {
  loading.value = true;
  try {
    const res = await superAdminApi.getStats();
    statsData.value = res.data;

    const totalEnt = statsData.value.totalEntreprises || 0;
    const totalUsers = statsData.value.totalUtilisateurs || 0;
    const totalT = statsData.value.totalTests || 0;
    const totalRev = statsData.value.totalRevenus || 0;
    const totalEnt7 = statsData.value.totalEntreprises7Days || 0;
    const totalUsers7 = statsData.value.totalUtilisateurs7Days || 0;
    const totalT7 = statsData.value.totalTests7Days || 0;
    const totalRev7 = statsData.value.totalRevenus7Days || 0;
    const totalEnt30 = statsData.value.totalEntreprises30Days || 0;
    const totalUsers30 = statsData.value.totalUtilisateurs30Days || 0;
    const totalT30 = statsData.value.totalTests30Days || 0;
    const totalRev30 = statsData.value.totalRevenus30Days || 0;

    statsByPeriod.value.month = [
      { label: 'Organisations', val: totalEnt7.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers7), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT7), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev7.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
    ];
    statsByPeriod.value.quarter = [
      { label: 'Organisations', val: totalEnt30.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers30), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT30), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev30.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
    ];
    statsByPeriod.value.year = [
      { label: 'Organisations', val: totalEnt.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.12)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.12)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.12)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.12)', icon: 'fa-solid fa-coins' }
    ];

    const stCount = statsData.value.startupCount || 0;
    const bsCount = statsData.value.businessCount || 0;
    const epCount = statsData.value.enterpriseCount || 0;
    const grCount = statsData.value.gratuitCount || 0;
    const totalPlans = (stCount + bsCount + epCount + grCount) || 1;

    const actualStats = {
      startup: { pct: Math.round((stCount / totalPlans) * 100), count: stCount },
      business: { pct: Math.round((bsCount / totalPlans) * 100), count: bsCount },
      enterprise: { pct: Math.round((epCount / totalPlans) * 100), count: epCount },
      gratuit: { pct: Math.round((grCount / totalPlans) * 100), count: grCount }
    };
    planStats.value.month = actualStats;
    planStats.value.quarter = actualStats;
    planStats.value.year = actualStats;

    const actualScales = {
      startup: stCount / totalPlans,
      business: bsCount / totalPlans,
      enterprise: epCount / totalPlans,
      gratuit: grCount / totalPlans
    };
    barScaleByPeriod.value.month = actualScales;
    barScaleByPeriod.value.quarter = actualScales;
    barScaleByPeriod.value.year = actualScales;

    if (statsData.value.recentTransactions && statsData.value.recentTransactions.length > 0) {
      recentActivities.value = statsData.value.recentTransactions.map((t, idx) => ({
        id: t.id || idx,
        name: t.name || 'Inconnue',
        code: `ORG-${(t.name || 'INC').substring(0, 3).toUpperCase()}`,
        plan: t.plan || 'GRATUIT',
        paymentMethod: t.plan.toLowerCase() === 'gratuit' ? 'Aucun' : 'Stripe / Bank',
        paymentIcon: t.plan.toLowerCase() === 'gratuit' ? 'fa-solid fa-slash' : 'fa-brands fa-stripe',
        date: t.date || 'Récemment',
        price: t.price || '0',
        color: t.color || '#6366f1'
      }));
    } else {
      recentActivities.value = [];
    }

    if (statsData.value.monthlyRevenues && statsData.value.monthlyRevenues.length > 0) {
      curveLineDataByPeriod.value.month = statsData.value.monthlyRevenues;
      curveLineDataByPeriod.value.quarter = statsData.value.monthlyRevenues;
      curveLineDataByPeriod.value.year = statsData.value.monthlyRevenues;
    } else {
      curveLineDataByPeriod.value.month = [0, 0, 0, 0, 0, 0];
      curveLineDataByPeriod.value.quarter = [0, 0, 0, 0, 0, 0];
      curveLineDataByPeriod.value.year = [0, 0, 0, 0, 0, 0];
    }

    const monthNames = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jui', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc'];
    const labels = [];
    const currentM = new Date().getMonth();
    for (let i = 5; i >= 0; i--) {
      labels.push(monthNames[(currentM - i + 12) % 12]);
    }
    monthsLabels.value = labels;

  } catch (err) {
    console.error("Error loading stats", err);
    toast.error("Erreur lors du chargement des statistiques réelles.");
  } finally {
    loading.value = false;
  }
}

function formatNumber(num) {
  if (num >= 1000) return `${(num / 1000).toFixed(1)}k`;
  return num.toString();
}

function setPeriod(period) {
  activePeriod.value = period;
  loading.value = true;
  setTimeout(() => {
    loading.value = false;
    toast.success("Statistiques actualisées pour la période demandée.");
  }, 400);
}

function refreshData() {
  loadRealStats();
  toast.success("Toutes les métriques réelles ont été rafraîchies.");
}

function simulateExport() {
  exporting.value = true;
  setTimeout(() => {
    exporting.value = false;
    const dateStr = new Date().toLocaleDateString('fr-FR', { year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit' });
    const reportHtml = `<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>Rapport Financier - EvaluaTech</title>
  <style>
    body { font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #0f172a; padding: 40px; line-height: 1.6; background-color: #f8fafc; }
    .container { max-width: 900px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1); border: 1px solid #e2e8f0; }
    .header { border-bottom: 2px solid #f59e0b; padding-bottom: 20px; margin-bottom: 30px; display: flex; justify-content: space-between; align-items: center; }
    .title { font-size: 28px; font-weight: 800; color: #1e3a8a; margin: 0; }
    .subtitle { font-size: 13px; color: #64748b; margin-top: 5px; }
    .logo { font-size: 24px; font-weight: 900; color: #f59e0b; }
    .section-title { font-size: 16px; font-weight: 800; color: #1e293b; margin-top: 35px; margin-bottom: 15px; border-left: 4px solid #f59e0b; padding-left: 12px; text-transform: uppercase; letter-spacing: 0.05em; }
    .grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 15px; margin-bottom: 30px; }
    .card { background: #f1f5f9; border: 1px solid #e2e8f0; padding: 18px; border-radius: 8px; text-align: center; }
    .card-label { font-size: 11px; color: #64748b; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; }
    .card-value { font-size: 20px; font-weight: 800; color: #1e3a8a; }
    table { width: 100%; border-collapse: collapse; margin-top: 15px; }
    th { background: #f8fafc; text-align: left; padding: 12px; font-size: 11px; font-weight: 800; color: #475569; border-bottom: 2px solid #e2e8f0; text-transform: uppercase; }
    td { padding: 14px 12px; font-size: 13px; border-bottom: 1px solid #e2e8f0; }
    .footer { margin-top: 60px; font-size: 11px; color: #94a3b8; text-align: center; border-top: 1px solid #e2e8f0; padding-top: 20px; }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <div><div class="title">Rapport Financier Global</div><div class="subtitle">Généré le ${dateStr}</div></div>
      <div class="logo">EvaluaTech</div>
    </div>
    <div class="section-title">Métriques Clés (Période : ${activePeriodLabel.value})</div>
    <div class="grid">
      ${currentStats.value.map(s => `<div class="card"><div class="card-label">${s.label}</div><div class="card-value">${s.val}</div></div>`).join('')}
    </div>
    <div class="footer">EvaluaTech SaaS Platform &copy; ${new Date().getFullYear()} — Document Confidentiel.</div>
  </div>
</body>
</html>`;
    const blob = new Blob([reportHtml], { type: 'text/html' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `rapport_financier_${activePeriod.value}_${new Date().toISOString().slice(0,10)}.html`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
    toast.success("Rapport exporté et téléchargé avec succès !");
  }, 1000);
}

onMounted(() => {
  loadRealStats();
});
</script>

<style scoped>
/* ═══════════════════════════════════════════════════════
   CSS VARIABLES
═══════════════════════════════════════════════════════ */
:root {
  --border-chart: #eef2f6;
  --text-chart: #94a3b8;
}

/* ═══════════════════════════════════════════════════════
   LAYOUT
═══════════════════════════════════════════════════════ */
.admin-body {
  min-height: 100vh;
  background-color: var(--bg-page, #f8fafc);
}

/* ═══════════════════════════════════════════════════════
   BREADCRUMB & HEADER
═══════════════════════════════════════════════════════ */
.breadcrumb-pro {
  font-size: 0.72rem;
  font-weight: 700;
  color: #94a3b8;
}
.breadcrumb-pro .root {
  cursor: pointer;
  transition: color 0.2s;
}
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

.premium-title {
  font-weight: 900;
  font-size: 2rem;
  letter-spacing: -1px;
  color: #0f172a;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fb923c 50%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* ═══════════════════════════════════════════════════════
   BUTTONS
═══════════════════════════════════════════════════════ */
.btn-glass-pro {
  background: white;
  border: 1.5px solid #eef2f6;
  color: #0f172a;
  border-radius: 14px;
  font-weight: 700;
  font-size: 0.78rem;
  transition: all 0.25s;
  cursor: pointer;
}
.btn-glass-pro:hover {
  border-color: #f59e0b;
  background: #fffbeb;
  transform: translateY(-1px);
}

.btn-enigma-primary-sm {
  background: #0f172a;
  color: white;
  border: none;
  padding: 10px 20px;
  border-radius: 14px;
  font-weight: 800;
  font-size: 0.78rem;
  position: relative;
  overflow: hidden;
  cursor: pointer;
  font-family: inherit;
  transition: all 0.3s;
}
.btn-content-sm {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: center;
}
.btn-glow-sm {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fb923c, #fbbf24);
  opacity: 0;
  transition: opacity 0.3s;
  z-index: 1;
}
.btn-enigma-primary-sm:hover .btn-glow-sm { opacity: 1; }
.btn-enigma-primary-sm:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 30px rgba(245, 158, 11, 0.3);
}
.btn-enigma-primary-sm:hover .btn-content-sm { color: #0f172a; }
.btn-enigma-primary-sm:disabled { opacity: 0.4; cursor: not-allowed; transform: none !important; }

.btn-refresh-pro {
  width: 40px;
  height: 40px;
  background: white;
  border: 1.5px solid #eef2f6;
  border-radius: 14px;
  color: #64748b;
  cursor: pointer;
  transition: all 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}
.btn-refresh-pro:hover {
  background: #fffbeb;
  border-color: #f59e0b;
  color: #f59e0b;
  transform: rotate(180deg) scale(1.05);
}
.btn-refresh-pro:hover i { animation: none; }
.fa-spin-active i { animation: spin 0.8s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }

.btn-outline-pro {
  background: white;
  color: #0f172a;
  border: 1.5px solid #eef2f6;
  padding: 9px 16px;
  border-radius: 14px;
  font-weight: 800;
  font-size: 0.78rem;
  cursor: pointer;
  transition: all 0.2s;
  font-family: inherit;
}
.btn-outline-pro:hover {
  border-color: #0f172a;
  transform: translateY(-1px);
}

/* ═══════════════════════════════════════════════════════
   DROPDOWN
═══════════════════════════════════════════════════════ */
.glass-dropdown-pro {
  background: white;
  border: 1.5px solid #eef2f6 !important;
  border-radius: 18px !important;
  padding: 8px !important;
  min-width: 200px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1) !important;
}
.glass-dropdown-pro .dropdown-item {
  border-radius: 12px;
  padding: 10px 14px;
  font-size: 0.78rem;
  font-weight: 700;
  color: #0f172a;
  transition: all 0.2s;
}
.glass-dropdown-pro .dropdown-item:hover {
  background: #fffbeb;
  color: #f59e0b;
}
.text-amber { color: #f59e0b !important; }

/* ═══════════════════════════════════════════════════════
   SPINNER
═══════════════════════════════════════════════════════ */
.spinner-pro-premium {
  width: 50px;
  height: 50px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 0.8s linear infinite;
}

/* ═══════════════════════════════════════════════════════
   KPI STAT CARDS — matching Campagne.vue premium style
═══════════════════════════════════════════════════════ */
.stat-card-premium {
  background: white;
  border-radius: 28px;
  border: 1px solid #eef2f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.04);
  transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
  overflow: hidden;
}
.stat-card-premium::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 3px;
  background: linear-gradient(90deg, transparent, #f59e0b, transparent);
  opacity: 0;
  transition: opacity 0.3s;
}
.stat-card-premium:hover::before { opacity: 1; }
.stat-card-premium:hover {
  transform: translateY(-6px);
  box-shadow: 0 24px 48px rgba(0,0,0,0.08);
  border-color: rgba(245,158,11,0.2);
}

.stat-glow-orb {
  position: absolute;
  top: -40px;
  right: -40px;
  width: 100px;
  height: 100px;
  border-radius: 50%;
  filter: blur(50px);
  z-index: 0;
  pointer-events: none;
}

.stat-label-top {
  position: relative;
  z-index: 1;
  font-size: 0.58rem;
  letter-spacing: 0.08em;
  text-transform: uppercase;
}

.stat-icon-wrapper {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 16px;
  font-size: 1.1rem;
  flex-shrink: 0;
  position: relative;
  z-index: 1;
  box-shadow: 0 4px 8px rgba(0,0,0,0.06);
  transition: transform 0.3s;
}
.stat-card-premium:hover .stat-icon-wrapper {
  transform: scale(1.1) rotate(-5deg);
}

.stat-value {
  font-size: 1.9rem;
  font-weight: 900;
  color: #0f172a;
  letter-spacing: -1px;
  line-height: 1;
  position: relative;
  z-index: 1;
}

.trend-badge {
  font-size: 0.62rem;
  display: inline-flex;
  align-items: center;
  gap: 3px;
}
.trend-up { background: rgba(16, 185, 129, 0.1); color: #059669; }
.trend-down { background: rgba(239, 68, 68, 0.1); color: #dc2626; }

/* ═══════════════════════════════════════════════════════
   GLASS CARDS — matching Campagne.vue enigma-card
═══════════════════════════════════════════════════════ */
.enigma-card {
  background: white;
  border: 1px solid #eef2f6;
  border-radius: 30px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.04);
  transition: box-shadow 0.3s;
}
.text-navy { color: #0f172a; }

/* ═══════════════════════════════════════════════════════
   CHARTS
═══════════════════════════════════════════════════════ */
.chart-container { position: relative; }
.area-transition { transition: d 0.4s cubic-bezier(0.4, 0, 0.2, 1); }
.line-transition { transition: d 0.4s cubic-bezier(0.4, 0, 0.2, 1); }
.chart-point {
  transition: r 0.2s ease, stroke-width 0.2s ease;
  cursor: pointer;
}
.chart-point:hover {
  r: 8px;
  filter: drop-shadow(0 0 8px rgba(245, 158, 11, 0.6));
}
.chart-bar {
  cursor: pointer;
  transition: opacity 0.2s ease, filter 0.2s ease;
}
.chart-bar:hover {
  opacity: 0.85;
  filter: brightness(1.1) drop-shadow(0 6px 10px rgba(0,0,0,0.15));
}

.chart-tooltip-pro {
  position: absolute;
  background: #0f172a;
  border: 1px solid rgba(255,255,255,0.08);
  color: white;
  border-radius: 12px;
  z-index: 100;
  pointer-events: none;
  min-width: 80px;
  font-size: 0.7rem;
  box-shadow: 0 8px 20px rgba(0,0,0,0.3);
  transition: opacity 0.15s ease-out;
}
.chart-tooltip-pro::after {
  content: '';
  position: absolute;
  bottom: -5px;
  left: 50%;
  transform: translateX(-50%);
  border-width: 5px 5px 0;
  border-style: solid;
  border-color: #0f172a transparent;
  width: 0;
  display: block;
}

.legend-dot-sm {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
  flex-shrink: 0;
}

/* ═══════════════════════════════════════════════════════
   PLAN TAGS
═══════════════════════════════════════════════════════ */
.plan-tag {
  font-size: 0.62rem;
  font-weight: 900;
  padding: 4px 10px;
  border-radius: 8px;
  display: inline-block;
  letter-spacing: 0.03em;
  text-transform: uppercase;
}
.tag-blue     { background: rgba(59, 130, 246, 0.1);  color: #2563eb; }
.tag-amber    { background: rgba(245, 158, 11, 0.1);  color: #d97706; }
.tag-emerald  { background: rgba(16, 185, 129, 0.1);  color: #059669; }
.tag-slate    { background: rgba(100, 116, 139, 0.1); color: #475569; }

/* ═══════════════════════════════════════════════════════
   TABLE
═══════════════════════════════════════════════════════ */
.custom-table-pro {
  margin-bottom: 0;
}
.custom-table-pro thead tr th {
  border-bottom: 2px solid #eef2f6;
  padding: 10px 16px;
  font-size: 0.62rem;
  font-weight: 900;
  letter-spacing: 0.07em;
  text-transform: uppercase;
  color: #94a3b8;
  background: transparent;
}
.custom-table-pro tbody td {
  padding: 14px 16px;
  border-bottom: 1px solid #f1f5f9;
  vertical-align: middle;
}
.table-row-pro {
  transition: background 0.15s ease;
}
.table-row-pro:hover td {
  background-color: #fffbeb !important;
}
.table-row-pro:last-child td {
  border-bottom: none;
}

.org-avatar {
  width: 40px;
  height: 40px;
  border-radius: 14px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.95rem;
  font-weight: 900;
  flex-shrink: 0;
  transition: transform 0.2s;
}
.table-row-pro:hover .org-avatar {
  transform: scale(1.08);
}

.status-pill-pro {
  padding: 5px 12px;
  border-radius: 10px;
  font-weight: 800;
  font-size: 0.62rem;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}
.status-active {
  background: rgba(16, 185, 129, 0.1);
  color: #059669;
  border: 1px solid rgba(16, 185, 129, 0.15);
  animation: statusPulse 3s ease-in-out infinite;
}
@keyframes statusPulse {
  0%, 100% { box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); }
  50% { box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.08); }
}
.status-dot-pro {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: currentColor;
  animation: blink 1.5s ease-in-out infinite;
}
@keyframes blink {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.3; }
}

/* ═══════════════════════════════════════════════════════
   ANIMATION
═══════════════════════════════════════════════════════ */
.animate-fade-in {
  animation: fadeInUp 0.4s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}
@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ═══════════════════════════════════════════════════════
   UTILITY
═══════════════════════════════════════════════════════ */
.tiny       { font-size: 0.78rem; }
.super-tiny { font-size: 0.65rem; }
.uppercase  { text-transform: uppercase; }
.fw-600 { font-weight: 600 !important; }
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.relative { position: relative; }
.overflow-hidden { overflow: hidden; }
.w-100 { width: 100%; }
.h-100 { height: 100%; }

/* ═══════════════════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════════════════ */
[data-theme="dark"] .admin-body { background: #0d1117; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .stat-card-premium { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }
[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .text-navy { color: #f0f6fc; }
[data-theme="dark"] .btn-glass-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-glass-pro:hover { background: rgba(245,158,11,0.1); border-color: #f59e0b; }
[data-theme="dark"] .btn-refresh-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .btn-refresh-pro:hover { background: rgba(245,158,11,0.1); border-color: #f59e0b; color: #f59e0b; }
[data-theme="dark"] .btn-outline-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-outline-pro:hover { border-color: #f59e0b; background: rgba(245,158,11,0.1); }
[data-theme="dark"] .glass-dropdown-pro { background: #161b22 !important; border-color: rgba(255,255,255,0.1) !important; }
[data-theme="dark"] .glass-dropdown-pro .dropdown-item { color: #f0f6fc; }
[data-theme="dark"] .glass-dropdown-pro .dropdown-item:hover { background: rgba(245,158,11,0.1); color: #f59e0b; }
[data-theme="dark"] .custom-table-pro thead tr th { color: #8b949e; border-bottom-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .custom-table-pro tbody td { border-bottom-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .table-row-pro:hover td { background-color: rgba(245,158,11,0.05) !important; }
[data-theme="dark"] .plan-tag.tag-blue    { background: rgba(59,130,246,0.15);  color: #93c5fd; }
[data-theme="dark"] .plan-tag.tag-amber   { background: rgba(245,158,11,0.15);  color: #fcd34d; }
[data-theme="dark"] .plan-tag.tag-emerald { background: rgba(16,185,129,0.15);  color: #6ee7b7; }
[data-theme="dark"] .plan-tag.tag-slate   { background: rgba(100,116,139,0.15); color: #94a3b8; }
[data-theme="dark"] .spinner-pro-premium { border-color: rgba(255,255,255,0.08); border-top-color: #f59e0b; }
[data-theme="dark"] :root { --border-chart: rgba(255,255,255,0.07); --text-chart: #8b949e; }
</style>