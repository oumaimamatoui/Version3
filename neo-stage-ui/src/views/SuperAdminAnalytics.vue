<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1 p-4">
      <AppNavbar />
      
      <main class="mt-4 animate-fade-in">
        <!-- HEADER WITH CONTROLS -->
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3 mb-4">
          <div>
            <h2 class="fw-800 text-navy mb-1">
              Analytics <span class="text-primary font-gradient">Global</span>
            </h2>
            <p class="text-muted tiny mb-0">Statistiques en temps réel et performance financière de la plateforme.</p>
          </div>
          
          <div class="d-flex align-items-center gap-2">
            <!-- Period Selector -->
            <div class="dropdown">
              <button class="btn btn-glass dropdown-toggle tiny fw-bold px-3 py-2" type="button" data-bs-toggle="dropdown">
                <i class="fa-solid fa-calendar-days me-2 text-primary"></i> {{ activePeriodLabel }}
              </button>
              <ul class="dropdown-menu dropdown-menu-end shadow border-0 glass-dropdown">
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('month')">7 Derniers Jours</a></li>
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('quarter')">30 Derniers Jours</a></li>
                <li><a class="dropdown-item tiny fw-semibold" href="#" @click.prevent="setPeriod('year')">Cette Année (12 mois)</a></li>
              </ul>
            </div>

            <!-- Export Button -->
            <button class="btn btn-primary btn-glow tiny fw-bold px-3 py-2" @click="simulateExport" :disabled="exporting">
              <i v-if="exporting" class="fa-solid fa-circle-notch fa-spin me-2"></i>
              <i v-else class="fa-solid fa-download me-2"></i> Exporter
            </button>

            <!-- Refresh Button -->
            <button class="btn btn-glass btn-icon" @click="refreshData" :class="{ 'fa-spin-active': loading }">
              <i class="fa-solid fa-rotate"></i>
            </button>
          </div>
        </div>

        <!-- LOADING OVERLAY -->
        <div v-if="loading" class="d-flex align-items-center justify-content-center py-5 my-5">
          <div class="spinner-loader"></div>
        </div>

        <div v-else class="animate-fade-in">
          <!-- MASTER KPI CARDS -->
          <div class="row g-4 mb-4">
            <div class="col-md-3 col-sm-6" v-for="(stat, i) in currentStats" :key="stat.label">
              <div class="kpi-card p-4 shadow-sm relative overflow-hidden transition-all">
                <div class="card-glow" :style="{ background: stat.glowColor }"></div>
                
                <div class="d-flex justify-content-between align-items-start mb-3">
                  <div class="tiny fw-800 text-muted uppercase tracking-wider">{{ stat.label }}</div>
                  <div class="kpi-icon" :style="{ background: stat.bg, color: stat.color }">
                    <i :class="stat.icon"></i>
                  </div>
                </div>

                <div class="h2 fw-800 text-navy tracking-tight mb-2">{{ stat.val }}</div>
                
                <div class="d-flex align-items-center gap-1.5 tiny">
                  <span class="fw-bold px-2 py-0.5 rounded-full" :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
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
              <div class="glass-card p-4 h-100">
                <div class="d-flex justify-content-between align-items-center mb-4">
                  <div>
                    <h6 class="fw-800 text-navy mb-0">Évolution des Revenus (€)</h6>
                    <p class="text-muted super-tiny mb-0">Courbe de croissance récurrente mensuelle.</p>
                  </div>
                  <div class="d-flex align-items-center gap-3">
                    <div class="d-flex align-items-center gap-1.5 tiny text-muted">
                      <span class="legend-dot" style="background: #8b5cf6;"></span> Revenus récurrents
                    </div>
                  </div>
                </div>

                <!-- Custom Line Chart SVG -->
                <div class="chart-container relative" style="height: 220px;">
                  <svg viewBox="0 0 500 240" class="w-100 h-100 overflow-visible" preserveAspectRatio="none">
                    <!-- Y-Axis Grid Lines -->
                    <line x1="40" y1="40" x2="480" y2="40" stroke="var(--border-color)" stroke-dasharray="4" />
                    <line x1="40" y1="90" x2="480" y2="90" stroke="var(--border-color)" stroke-dasharray="4" />
                    <line x1="40" y1="140" x2="480" y2="140" stroke="var(--border-color)" stroke-dasharray="4" />
                    <line x1="40" y1="190" x2="480" y2="190" stroke="var(--border-color)" />

                    <!-- Y-Axis Labels -->
                    <text x="10" y="45" font-size="9" fill="var(--text-light)" font-weight="bold">15k</text>
                    <text x="10" y="95" font-size="9" fill="var(--text-light)" font-weight="bold">10k</text>
                    <text x="10" y="145" font-size="9" fill="var(--text-light)" font-weight="bold">5k</text>
                    <text x="10" y="195" font-size="9" fill="var(--text-light)" font-weight="bold">0</text>

                    <!-- Area under the Curve -->
                    <path :d="curveAreaPath" fill="url(#area-grad)" class="area-transition" />

                    <!-- Curve Line -->
                    <path :d="curveLinePath" fill="none" stroke="url(#line-grad)" stroke-width="3.5" stroke-linecap="round" class="line-transition" />

                    <!-- Grid Definitions -->
                    <defs>
                      <linearGradient id="area-grad" x1="0" y1="0" x2="0" y2="1">
                        <stop offset="0%" stop-color="#8b5cf6" stop-opacity="0.35" />
                        <stop offset="100%" stop-color="#8b5cf6" stop-opacity="0.0" />
                      </linearGradient>
                      <linearGradient id="line-grad" x1="0" y1="0" x2="1" y2="0">
                        <stop offset="0%" stop-color="#3b82f6" />
                        <stop offset="50%" stop-color="#8b5cf6" />
                        <stop offset="100%" stop-color="#ec4899" />
                      </linearGradient>
                    </defs>

                    <!-- Interactive Data Points -->
                    <g v-for="(pt, idx) in currentCurvePoints" :key="idx" 
                       @mouseenter="hoveredPoint = idx" 
                       @mouseleave="hoveredPoint = null"
                       class="cursor-pointer">
                      <circle :cx="pt.x" :cy="pt.y" r="5" fill="var(--bg-card)" stroke="#8b5cf6" stroke-width="3" class="transition-all" />
                      <circle :cx="pt.x" :cy="pt.y" r="12" fill="transparent" />
                    </g>
                  </svg>

                  <!-- Tooltip Float -->
                  <div v-if="hoveredPoint !== null" 
                       class="chart-tooltip p-2 text-center absolute shadow"
                       :style="{ 
                         left: (currentCurvePoints[hoveredPoint].x / 5) + '%', 
                         top: (currentCurvePoints[hoveredPoint].y - 50) + 'px'
                       }">
                    <div class="super-tiny text-muted fw-bold">{{ monthsLabels[hoveredPoint] }}</div>
                    <div class="tiny fw-800 text-navy">{{ currentLineData[hoveredPoint] }} €</div>
                  </div>
                </div>

                <div class="d-flex justify-content-between px-4 mt-3 tiny fw-bold text-muted">
                  <span v-for="m in monthsLabels" :key="m">{{ m }}</span>
                </div>
              </div>
            </div>

            <!-- Bar Chart: Subscription Distribution -->
            <div class="col-lg-4">
              <div class="glass-card p-4 h-100">
                <div class="mb-4">
                  <h6 class="fw-800 text-navy mb-0">Répartition des abonnements</h6>
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
                        <stop offset="0%" stop-color="#9ca3af" />
                        <stop offset="100%" stop-color="#6b7280" />
                      </linearGradient>
                    </defs>

                    <!-- Horizontal Grid Lines -->
                    <line x1="30" y1="40" x2="280" y2="40" stroke="var(--border-color)" stroke-dasharray="2" />
                    <line x1="30" y1="100" x2="280" y2="100" stroke="var(--border-color)" stroke-dasharray="2" />
                    <line x1="30" y1="160" x2="280" y2="160" stroke="var(--border-color)" />

                    <!-- Bars with responsive dynamic animations -->
                    <!-- STARTUP -->
                    <rect x="25" :y="160 - (120 * currentBarScale.startup)" width="30" :height="120 * currentBarScale.startup" rx="4" fill="url(#grad-startup)" class="chart-bar" @mouseenter="hoveredBar = 'startup'" @mouseleave="hoveredBar = null" />
                    <!-- BUSINESS -->
                    <rect x="90" :y="160 - (120 * currentBarScale.business)" width="30" :height="120 * currentBarScale.business" rx="4" fill="url(#grad-business)" class="chart-bar" @mouseenter="hoveredBar = 'business'" @mouseleave="hoveredBar = null" />
                    <!-- ENTERPRISE -->
                    <rect x="155" :y="160 - (120 * currentBarScale.enterprise)" width="30" :height="120 * currentBarScale.enterprise" rx="4" fill="url(#grad-enterprise)" class="chart-bar" @mouseenter="hoveredBar = 'enterprise'" @mouseleave="hoveredBar = null" />
                    <!-- GRATUIT -->
                    <rect x="220" :y="160 - (120 * currentBarScale.gratuit)" width="30" :height="120 * currentBarScale.gratuit" rx="4" fill="url(#grad-gratuit)" class="chart-bar" @mouseenter="hoveredBar = 'gratuit'" @mouseleave="hoveredBar = null" />
                  </svg>

                  <!-- Tooltip Float for Bars -->
                  <div v-if="hoveredBar !== null" 
                       class="chart-tooltip p-2 text-center absolute shadow"
                       :style="getBarTooltipStyle(hoveredBar)">
                     <div class="super-tiny text-muted fw-bold uppercase">{{ hoveredBar }}</div>
                     <div class="tiny fw-800 text-navy">{{ getBarStats(hoveredBar).pct }}%</div>
                     <div class="super-tiny text-muted">{{ getBarStats(hoveredBar).count }} orgs</div>
                  </div>
                </div>

                <div class="d-flex justify-content-around mt-2 tiny fw-800 text-muted flex-wrap gap-1">
                  <span class="badge-tag tag-primary">STARTUP</span>
                  <span class="badge-tag tag-amber">BUSINESS</span>
                  <span class="badge-tag tag-emerald">ENTERPRISE</span>
                  <span class="badge-tag tag-secondary" style="background-color: #6b7280; color: #fff;">GRATUIT</span>
                </div>
              </div>
            </div>
          </div>

          <!-- DETAILED DATA TABLE -->
          <div class="glass-card p-4">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
              <div>
                <h6 class="fw-800 text-navy mb-1">Dernières Activités Financières</h6>
                <p class="text-muted super-tiny mb-0">Historique des transactions et abonnements récemment validés.</p>
              </div>
              <button class="btn btn-glass tiny fw-bold px-3 py-1.5" @click="router.push('/super-admin')">
                <i class="fa-solid fa-list me-2"></i> Gérer Organisations
              </button>
            </div>

            <div class="table-responsive">
              <table class="table custom-table align-middle">
                <thead>
                  <tr class="text-muted tiny uppercase font-weight-bold">
                    <th scope="col">Organisation</th>
                    <th scope="col">Formule</th>
                    <th scope="col">Moyen de paiement</th>
                    <th scope="col">Date d'activation</th>
                    <th scope="col">Facturation</th>
                    <th scope="col" class="text-center">Statut</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="org in recentActivities" :key="org.id" class="hover-row">
                    <td>
                      <div class="d-flex align-items-center gap-2.5">
                        <div class="avatar-circle font-weight-bold shadow-sm" :style="{ background: org.color + '15', color: org.color }">
                          {{ org.name[0] }}
                        </div>
                        <div>
                          <div class="fw-bold text-navy tiny">{{ org.name }}</div>
                          <div class="super-tiny text-muted">ID: {{ org.code }}</div>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span class="badge-tag" :class="getPlanTagClass(org.plan)">
                        {{ org.plan }}
                      </span>
                    </td>
                    <td>
                      <div class="d-flex align-items-center gap-2 tiny">
                        <i :class="org.paymentIcon + ' text-muted'"></i>
                        <span class="fw-semibold text-muted">{{ org.paymentMethod }}</span>
                      </div>
                    </td>
                    <td class="tiny fw-bold text-navy">{{ org.date }}</td>
                    <td class="tiny fw-800 text-navy">{{ org.price }} €<span class="text-muted super-tiny fw-semibold">/mois</span></td>
                    <td class="text-center">
                      <span class="status-pill status-success tiny">Actif</span>
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
    { label: 'Organisations', val: '0', trend: 12, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 8, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 25, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 5, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
  ],
  quarter: [
    { label: 'Organisations', val: '0', trend: 18, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 15, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 32, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 12, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
  ],
  year: [
    { label: 'Organisations', val: '0', trend: 45, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs', val: '0', trend: 38, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
    { label: 'Tests Passés', val: '0', trend: 78, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements', val: '0 €', trend: 28, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
  ]
});

const currentStats = computed(() => {
  return statsByPeriod.value[activePeriod.value];
});

const curveLineDataByPeriod = ref({
  month: [4200, 6800, 5100, 9400, 8200, 14200],
  quarter: [7200, 9100, 11400, 15300, 13800, 21500],
  year: [18000, 24000, 29000, 38000, 42000, 54200]
});

const currentLineData = computed(() => {
  return curveLineDataByPeriod.value[activePeriod.value];
});

const monthsLabels = ref(['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun']);

const currentCurvePoints = computed(() => {
  const data = currentLineData.value;
  const maxVal = Math.max(...data, 1000) * 1.15;
  const points = [];
  for (let i = 0; i < data.length; i++) {
    const x = 40 + (440 / (data.length - 1)) * i;
    const ratio = data[i] / maxVal;
    const y = 190 - (150 * ratio);
    points.push({ x, y });
  }
  return points;
});

const curveLinePath = computed(() => {
  const pts = currentCurvePoints.value;
  if (pts.length < 2) return '';
  let d = `M ${pts[0].x} ${pts[0].y}`;
  for (let i = 0; i < pts.length - 1; i++) {
    const cpX1 = pts[i].x + (pts[i+1].x - pts[i].x) / 2;
    const cpY1 = pts[i].y;
    const cpX2 = pts[i].x + (pts[i+1].x - pts[i].x) / 2;
    const cpY2 = pts[i+1].y;
    d += ` C ${cpX1} ${cpY1}, ${cpX2} ${cpY2}, ${pts[i+1].x} ${pts[i+1].y}`;
  }
  return d;
});

const curveAreaPath = computed(() => {
  const pts = currentCurvePoints.value;
  if (pts.length < 2) return '';
  let d = curveLinePath.value;
  d += ` L ${pts[pts.length - 1].x} 190 L ${pts[0].x} 190 Z`;
  return d;
});

const barScaleByPeriod = ref({
  month: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 },
  quarter: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 },
  year: { startup: 0.0, business: 0.0, enterprise: 0.0, gratuit: 0.0 }
});

const currentBarScale = computed(() => {
  return barScaleByPeriod.value[activePeriod.value];
});

function getBarTooltipStyle(bar) {
  let leftOffset = '50%';
  if (bar === 'startup') leftOffset = '18%';
  if (bar === 'business') leftOffset = '38%';
  if (bar === 'enterprise') leftOffset = '58%';
  if (bar === 'gratuit') leftOffset = '78%';
  return {
    left: leftOffset,
    top: '40px',
    transform: 'translateX(-50%)',
    zIndex: 10
  };
}

const planStats = ref({
  month: {
    startup: { pct: 0, count: 0 },
    business: { pct: 0, count: 0 },
    enterprise: { pct: 0, count: 0 },
    gratuit: { pct: 0, count: 0 }
  },
  quarter: {
    startup: { pct: 0, count: 0 },
    business: { pct: 0, count: 0 },
    enterprise: { pct: 0, count: 0 },
    gratuit: { pct: 0, count: 0 }
  },
  year: {
    startup: { pct: 0, count: 0 },
    business: { pct: 0, count: 0 },
    enterprise: { pct: 0, count: 0 },
    gratuit: { pct: 0, count: 0 }
  }
});

function getBarStats(bar) {
  return planStats.value[activePeriod.value][bar];
}

function getPlanTagClass(plan) {
  if (plan === 'ENTERPRISE') return 'tag-emerald';
  if (plan === 'BUSINESS') return 'tag-amber';
  return 'tag-primary';
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

    // Load active period real values
    statsByPeriod.value.month = [
      { label: 'Organisations', val: totalEnt7.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers7), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT7), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev7.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
    ];

    statsByPeriod.value.quarter = [
      { label: 'Organisations', val: totalEnt30.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers30), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT30), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev30.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
    ];

    statsByPeriod.value.year = [
      { label: 'Organisations', val: totalEnt.toString(), trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99, 102, 241, 0.15)', icon: 'fa-solid fa-building' },
      { label: 'Utilisateurs', val: formatNumber(totalUsers), trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217, 119, 6, 0.15)', icon: 'fa-solid fa-users' },
      { label: 'Tests Passés', val: formatNumber(totalT), trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5, 150, 105, 0.15)', icon: 'fa-solid fa-wand-magic-sparkles' },
      { label: 'Abonnements', val: `${totalRev.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220, 38, 38, 0.15)', icon: 'fa-solid fa-coins' }
    ];

    // Real active plan distribution counts
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

    // Load recent activities
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

    // Dynamic line/area curve from database registration history!
    if (statsData.value.monthlyRevenues && statsData.value.monthlyRevenues.length > 0) {
      curveLineDataByPeriod.value.month = statsData.value.monthlyRevenues;
      curveLineDataByPeriod.value.quarter = statsData.value.monthlyRevenues;
      curveLineDataByPeriod.value.year = statsData.value.monthlyRevenues;
    } else {
      curveLineDataByPeriod.value.month = [0, 0, 0, 0, 0, 0];
      curveLineDataByPeriod.value.quarter = [0, 0, 0, 0, 0, 0];
      curveLineDataByPeriod.value.year = [0, 0, 0, 0, 0, 0];
    }

    // Dynamically calculate month names labels for the last 6 months
    const monthNames = ['Jan', 'Fév', 'Mar', 'Avr', 'Mai', 'Jun', 'Jui', 'Aoû', 'Sep', 'Oct', 'Nov', 'Déc'];
    const labels = [];
    const currentM = new Date().getMonth();
    for (let i = 5; i >= 0; i--) {
      let mIdx = (currentM - i + 12) % 12;
      labels.push(monthNames[mIdx]);
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
    
    // Generate beautiful real report content
    const dateStr = new Date().toLocaleDateString('fr-FR', {
      year: 'numeric', month: 'long', day: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
    
    const reportHtml = `<!DOCTYPE html>
<html>
<head>
  <meta charset="utf-8">
  <title>Rapport Financier - EvaluaTech</title>
  <style>
    body { font-family: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #0f172a; padding: 40px; line-height: 1.6; background-color: #f8fafc; }
    .container { max-width: 900px; margin: 0 auto; background: #ffffff; padding: 40px; border-radius: 12px; box-shadow: 0 4px 6px -1px rgb(0 0 0 / 0.1); border: 1px solid #e2e8f0; }
    .header { border-bottom: 2px solid #3b82f6; padding-bottom: 20px; margin-bottom: 30px; display: flex; justify-content: space-between; align-items: center; }
    .title { font-size: 28px; font-weight: 800; color: #1e3a8a; margin: 0; }
    .subtitle { font-size: 13px; color: #64748b; margin-top: 5px; }
    .logo { font-size: 24px; font-weight: 900; color: #3b82f6; }
    .section-title { font-size: 16px; font-weight: 800; color: #1e293b; margin-top: 35px; margin-bottom: 15px; border-left: 4px solid #3b82f6; padding-left: 12px; text-transform: uppercase; letter-spacing: 0.05em; }
    .grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 15px; margin-bottom: 30px; }
    .card { background: #f1f5f9; border: 1px solid #e2e8f0; padding: 18px; border-radius: 8px; text-align: center; }
    .card-label { font-size: 11px; color: #64748b; font-weight: 700; text-transform: uppercase; margin-bottom: 6px; }
    .card-value { font-size: 20px; font-weight: 800; color: #1e3a8a; }
    table { width: 100%; border-collapse: collapse; margin-top: 15px; }
    th { background: #f8fafc; text-align: left; padding: 12px; font-size: 11px; font-weight: 800; color: #475569; border-bottom: 2px solid #e2e8f0; text-transform: uppercase; }
    td { padding: 14px 12px; font-size: 13px; border-bottom: 1px solid #e2e8f0; }
    .badge { padding: 4px 8px; font-size: 10px; font-weight: 800; border-radius: 4px; display: inline-block; text-transform: uppercase; }
    .badge-startup { background: rgba(59, 130, 246, 0.12); color: #2563eb; }
    .badge-business { background: rgba(245, 158, 11, 0.12); color: #d97706; }
    .badge-enterprise { background: rgba(16, 185, 129, 0.12); color: #059669; }
    .badge-gratuit { background: #6b7280; color: #ffffff; }
    .footer { margin-top: 60px; font-size: 11px; color: #94a3b8; text-align: center; border-top: 1px solid #e2e8f0; padding-top: 20px; }
  </style>
</head>
<body>
  <div class="container">
    <div class="header">
      <div>
        <div class="title">Rapport Financier Global</div>
        <div class="subtitle">Généré le ${dateStr} par le Super Administrateur d'EvaluaTech</div>
      </div>
      <div class="logo">Evalua<span style="color:#0f172a;">Tech</span></div>
    </div>
    
    <div class="section-title">Métriques Clés (Période : ${activePeriodLabel.value})</div>
    <div class="grid">
      ${currentStats.value.map(s => `
        <div class="card">
          <div class="card-label">${s.label}</div>
          <div class="card-value">${s.val}</div>
        </div>
      `).join('')}
    </div>
    
    <div class="section-title">Dernières Activités & Transactions</div>
    <table>
      <thead>
        <tr>
          <th>Organisation</th>
          <th>Formule</th>
          <th>Moyen de paiement</th>
          <th>Date d'activation</th>
          <th>Facturation</th>
        </tr>
      </thead>
      <tbody>
        ${recentActivities.value.length === 0 ? `
          <tr>
            <td colspan="5" style="text-align: center; color: #94a3b8; padding: 20px;">Aucune transaction récente enregistrée en base de données.</td>
          </tr>
        ` : recentActivities.value.map(org => `
          <tr>
            <td><strong>${org.name}</strong><br><small style="color: #64748b;">${org.code}</small></td>
            <td><span class="badge badge-${org.plan.toLowerCase()}">${org.plan}</span></td>
            <td>${org.paymentMethod}</td>
            <td>${org.date}</td>
            <td><strong>${org.price} €/mois</strong></td>
          </tr>
        `).join('')}
      </tbody>
    </table>
    
    <div class="footer">
      EvaluaTech SaaS Platform &copy; ${new Date().getFullYear()} - Document Confidentiel Réservé à l'Administration.
    </div>
  </div>
</body>
</html>`;
    
    // Create blob and trigger a real file download!
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
/* GENERAL STYLES */
.admin-body {
  min-height: 100vh;
  background-color: var(--bg-page);
}

.font-gradient {
  background: linear-gradient(135deg, #3b82f6 0%, #8b5cf6 50%, #ec4899 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}

/* BUTTONS */
.btn-glass {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  color: var(--text-main);
  backdrop-filter: blur(10px);
  transition: var(--transition);
}
.btn-glass:hover {
  background: var(--bg-hover);
  border-color: var(--text-light);
}

.btn-icon {
  width: 38px;
  height: 38px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-sm);
}

.btn-glow:hover {
  box-shadow: 0 0 15px rgba(245, 158, 11, 0.4);
  transform: translateY(-1px);
}

.fa-spin-active i {
  animation: spin 0.8s linear infinite;
}
@keyframes spin {
  100% { transform: rotate(360deg); }
}

/* LOADER */
.spinner-loader {
  width: 50px;
  height: 50px;
  border: 4px solid var(--border-color);
  border-top-color: var(--primary);
  border-radius: 50%;
  animation: spin 1s ease-in-out infinite;
}

/* KPI CARDS */
.kpi-card {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  z-index: 1;
}
.kpi-card:hover {
  transform: translateY(-4px);
  box-shadow: var(--shadow-md);
  border-color: var(--primary-light);
}

.card-glow {
  position: absolute;
  top: -30px;
  right: -30px;
  width: 90px;
  height: 90px;
  border-radius: 50%;
  filter: blur(40px);
  z-index: -1;
  pointer-events: none;
}

.kpi-icon {
  width: 44px;
  height: 44px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: var(--radius-md);
  font-size: 1.15rem;
  box-shadow: var(--shadow-xs);
}

/* TREND BADGES */
.trend-up {
  background: rgba(16, 185, 129, 0.12);
  color: #059669;
}
.trend-down {
  background: rgba(239, 68, 68, 0.12);
  color: #dc2626;
}

/* GLASS CARD & TRANSITIONS */
.glass-card {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
}

/* SVG Line chart styles */
.area-transition {
  transition: d 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}
.line-transition {
  transition: d 0.4s cubic-bezier(0.4, 0, 0.2, 1);
}

.chart-point {
  transition: r 0.2s ease, stroke-width 0.2s ease;
}
.chart-point:hover {
  r: 8px;
  stroke-width: 4px;
  filter: drop-shadow(0 0 6px #8b5cf6);
}

/* SVG Bar chart styles */
.chart-bar {
  cursor: pointer;
  transition: height 0.4s cubic-bezier(0.4, 0, 0.2, 1), y 0.4s cubic-bezier(0.4, 0, 0.2, 1), opacity 0.2s ease;
}
.chart-bar:hover {
  opacity: 0.9;
  filter: brightness(1.1) drop-shadow(0 4px 8px rgba(0,0,0,0.15));
}

/* TOOLTIPS */
.chart-tooltip {
  background: rgba(15, 23, 42, 0.95);
  border: 1px solid rgba(255, 255, 255, 0.1);
  color: #ffffff;
  border-radius: var(--radius-sm);
  z-index: 100;
  pointer-events: none;
  backdrop-filter: blur(4px);
  min-width: 80px;
  transition: left 0.15s ease-out, top 0.15s ease-out;
}
.chart-tooltip::after {
  content: '';
  position: absolute;
  bottom: -5px;
  left: 50%;
  transform: translateX(-50%);
  border-width: 5px 5px 0;
  border-style: solid;
  border-color: rgba(15, 23, 42, 0.95) transparent;
  display: block;
  width: 0;
}

/* TABLE STYLING */
.custom-table {
  margin-bottom: 0;
}
.custom-table th {
  border-bottom: 1.5px solid var(--border-color);
  padding: 12px 16px;
  font-size: 0.72rem;
  letter-spacing: 0.05em;
}
.custom-table td {
  padding: 14px 16px;
  border-bottom: 1px solid var(--border-color);
}

.hover-row {
  transition: background 0.15s ease;
}
.hover-row:hover {
  background-color: var(--bg-hover) !important;
}

/* AVATARS & PINS */
.avatar-circle {
  width: 38px;
  height: 38px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.95rem;
}

.badge-tag {
  font-size: 0.68rem;
  font-weight: 800;
  padding: 4px 8px;
  border-radius: var(--radius-xs);
  display: inline-block;
  letter-spacing: 0.02em;
}

.tag-primary { background: rgba(59, 130, 246, 0.12); color: #2563eb; }
.tag-amber { background: rgba(245, 158, 11, 0.12); color: #d97706; }
.tag-emerald { background: rgba(16, 185, 129, 0.12); color: #059669; }

.status-pill {
  padding: 3px 10px;
  border-radius: var(--radius-full);
  font-weight: 700;
  font-size: 0.65rem;
  display: inline-block;
}
.status-success {
  background: rgba(16, 185, 129, 0.12);
  color: #059669;
}

.legend-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
}

/* ANIMATION UTILS */
.animate-fade-in {
  animation: fadeIn 0.4s cubic-bezier(0.4, 0, 0.2, 1) forwards;
}
@keyframes fadeIn {
  from { opacity: 0; transform: translateY(8px); }
  to { opacity: 1; transform: translateY(0); }
}

.glass-dropdown {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  backdrop-filter: blur(15px);
}
.glass-dropdown .dropdown-item {
  color: var(--text-main);
  transition: var(--transition-fast);
}
.glass-dropdown .dropdown-item:hover {
  background: var(--bg-hover);
}
</style>