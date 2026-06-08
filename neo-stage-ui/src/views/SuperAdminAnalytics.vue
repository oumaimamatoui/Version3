SuperAdminAnalytics.vue:<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- ═══ BACKGROUND ENGINE ═══ -->
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

          <!-- ═══ HEADER ═══ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Analytics Global</span>
              </div>
              <h2 class="premium-title mb-1">
                Analytics <span class="gradient-text">Global</span>
              </h2>
              <p class="text-muted super-tiny mb-0 fw-700">Statistiques en temps réel et performance financière de la plateforme.</p>
            </div>

            <div class="d-flex align-items-center gap-3 flex-wrap">
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

              <!-- Export -->
              <button class="btn-enigma-primary shadow-premium" @click="simulateExport" :disabled="exporting">
                <div class="btn-content">
                  <i v-if="exporting" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                  <i v-else class="fa-solid fa-download me-2"></i> Exporter
                </div>
                <div class="btn-glow"></div>
              </button>

              <!-- Refresh -->
              <button class="btn-refresh-pro" @click="refreshData" :class="{ 'fa-spin-active': loading }" title="Rafraîchir">
                <i class="fa-solid fa-rotate"></i>
              </button>
            </div>
          </header>

          <!-- ═══ LOADING ═══ -->
          <div v-if="loading" class="d-flex align-items-center justify-content-center py-5 my-5">
            <div class="spinner-pro-premium"></div>
          </div>

          <div v-else class="animate__animated animate__fadeIn">

            <!-- ═══ KPI CARDS ═══ -->
            <div class="row g-4 mb-5">
              <div class="col-xl-3 col-md-6" v-for="(stat, i) in currentStats" :key="stat.label">
                <div class="stat-card-premium">
                  <div class="stat-glow-orb" :style="{ background: stat.glowColor }"></div>
                  <div class="d-flex justify-content-between align-items-start mb-4">
                    <div class="tiny fw-900 text-muted uppercase tracking-wider stat-label-top">{{ stat.label }}</div>
                    <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                      <i :class="stat.icon"></i>
                    </div>
                  </div>
                  <div class="stat-value mb-3">{{ stat.val }}</div>
                  <div class="d-flex align-items-center gap-2 tiny">
                    <span class="trend-badge fw-bold px-2 py-1 rounded-pill" :class="stat.trend >= 0 ? 'trend-up' : 'trend-down'">
                      <i :class="stat.trend >= 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                      {{ Math.abs(stat.trend) }}%
                    </span>
                    <span class="text-muted fw-600">vs mois dernier</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- ═══ CHARTS SECTION ═══ -->
            <div class="row g-4 mb-5">

              <!-- Line Chart -->
              <div class="col-lg-8">
                <div class="enigma-card p-5 h-100">
                  <div class="d-flex justify-content-between align-items-center mb-5 flex-wrap gap-2">
                    <div>
                      <h6 class="fw-900 text-navy mb-1">Évolution des Revenus <span class="text-muted fw-600">(€)</span></h6>
                      <p class="text-muted super-tiny mb-0 fw-700">Courbe de croissance récurrente mensuelle.</p>
                    </div>
                    <div class="d-flex align-items-center gap-2 tiny text-muted">
                      <span class="legend-dot-sm" style="background: linear-gradient(135deg, #3b82f6, #f59e0b);"></span>
                      <span class="fw-700">Revenus récurrents</span>
                    </div>
                  </div>

                  <div class="chart-container relative" style="height: 220px;">
                    <svg viewBox="0 0 500 240" class="w-100 h-100 overflow-visible" preserveAspectRatio="none">
                      <defs>
                        <linearGradient id="area-grad-v2" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#f59e0b" stop-opacity="0.25" />
                          <stop offset="100%" stop-color="#f59e0b" stop-opacity="0.0" />
                        </linearGradient>
                        <linearGradient id="line-grad-v2" x1="0" y1="0" x2="1" y2="0">
                          <stop offset="0%" stop-color="#3b82f6" />
                          <stop offset="50%" stop-color="#f59e0b" />
                          <stop offset="100%" stop-color="#fb923c" />
                        </linearGradient>
                        <filter id="glow-filter">
                          <feGaussianBlur stdDeviation="2" result="coloredBlur"/>
                          <feMerge><feMergeNode in="coloredBlur"/><feMergeNode in="SourceGraphic"/></feMerge>
                        </filter>
                      </defs>

                      <!-- Grid Lines -->
                      <line x1="40" y1="40"  x2="480" y2="40"  stroke="var(--border-chart)" stroke-dasharray="4,4" />
                      <line x1="40" y1="90"  x2="480" y2="90"  stroke="var(--border-chart)" stroke-dasharray="4,4" />
                      <line x1="40" y1="140" x2="480" y2="140" stroke="var(--border-chart)" stroke-dasharray="4,4" />
                      <line x1="40" y1="190" x2="480" y2="190" stroke="var(--border-chart)" />

                      <!-- Y Labels -->
                      <text x="10" y="45"  font-size="9" fill="var(--text-chart)" font-weight="800">15k</text>
                      <text x="10" y="95"  font-size="9" fill="var(--text-chart)" font-weight="800">10k</text>
                      <text x="10" y="145" font-size="9" fill="var(--text-chart)" font-weight="800">5k</text>
                      <text x="10" y="195" font-size="9" fill="var(--text-chart)" font-weight="800">0</text>

                      <!-- Area -->
                      <path :d="curveAreaPath" fill="url(#area-grad-v2)" class="area-transition" />

                      <!-- Glow Line (thicker, blurred) -->
                      <path :d="curveLinePath" fill="none" stroke="url(#line-grad-v2)" stroke-width="6" stroke-linecap="round" class="line-transition" opacity="0.3" filter="url(#glow-filter)" />

                      <!-- Main Line -->
                      <path :d="curveLinePath" fill="none" stroke="url(#line-grad-v2)" stroke-width="3" stroke-linecap="round" class="line-transition" />

                      <!-- Data Points -->
                      <g v-for="(pt, idx) in currentCurvePoints" :key="idx"
                         @mouseenter="hoveredPoint = idx"
                         @mouseleave="hoveredPoint = null"
                         class="cursor-pointer">
                        <circle :cx="pt.x" :cy="pt.y" r="16" fill="transparent" />
                        <circle :cx="pt.x" :cy="pt.y" :r="hoveredPoint === idx ? 8 : 5"
                          fill="white" stroke="#f59e0b" :stroke-width="hoveredPoint === idx ? 3.5 : 2.5"
                          class="chart-point"
                          :filter="hoveredPoint === idx ? 'url(#glow-filter)' : ''" />
                      </g>
                    </svg>

                    <!-- Tooltip -->
                    <transition name="tooltip-fade">
                      <div v-if="hoveredPoint !== null"
                           class="chart-tooltip-pro p-2 text-center"
                           :style="{ left: (currentCurvePoints[hoveredPoint].x / 5) + '%', top: (currentCurvePoints[hoveredPoint].y - 62) + 'px' }">
                        <div class="super-tiny text-muted fw-800">{{ monthsLabels[hoveredPoint] }}</div>
                        <div class="tiny fw-900 text-amber">{{ currentLineData[hoveredPoint].toLocaleString() }} €</div>
                      </div>
                    </transition>
                  </div>

                  <div class="d-flex justify-content-between px-1 mt-3">
                    <span v-for="m in monthsLabels" :key="m" class="super-tiny fw-800 text-muted">{{ m }}</span>
                  </div>
                </div>
              </div>

              <!-- Bar Chart -->
              <div class="col-lg-4">
                <div class="enigma-card p-5 h-100">
                  <div class="mb-5">
                    <h6 class="fw-900 text-navy mb-1">Répartition des abonnements</h6>
                    <p class="text-muted super-tiny mb-0 fw-700">Volume d'organisations par offre active.</p>
                  </div>

                  <div class="chart-container relative" style="height: 200px;">
                    <svg viewBox="0 0 300 180" class="w-100 h-100 overflow-visible">
                      <defs>
                        <linearGradient id="grad-startup" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#818cf8" /><stop offset="100%" stop-color="#6366f1" />
                        </linearGradient>
                        <linearGradient id="grad-business" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#fbbf24" /><stop offset="100%" stop-color="#f59e0b" />
                        </linearGradient>
                        <linearGradient id="grad-enterprise" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#34d399" /><stop offset="100%" stop-color="#10b981" />
                        </linearGradient>
                        <linearGradient id="grad-gratuit" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="0%" stop-color="#94a3b8" /><stop offset="100%" stop-color="#64748b" />
                        </linearGradient>
                        <filter id="bar-shadow">
                          <feDropShadow dx="0" dy="4" stdDeviation="4" flood-opacity="0.15"/>
                        </filter>
                      </defs>

                      <line x1="30" y1="20"  x2="270" y2="20"  stroke="var(--border-chart)" stroke-dasharray="3,3" />
                      <line x1="30" y1="70"  x2="270" y2="70"  stroke="var(--border-chart)" stroke-dasharray="3,3" />
                      <line x1="30" y1="140" x2="270" y2="140" stroke="var(--border-chart)" />

                      <!-- STARTUP -->
                      <rect x="20"  :y="140 - (120 * currentBarScale.startup)"   width="32" :height="120 * currentBarScale.startup"   rx="8" fill="url(#grad-startup)"   class="chart-bar" filter="url(#bar-shadow)" @mouseenter="hoveredBar='startup'"   @mouseleave="hoveredBar=null"/>
                      <!-- BUSINESS -->
                      <rect x="85"  :y="140 - (120 * currentBarScale.business)"  width="32" :height="120 * currentBarScale.business"  rx="8" fill="url(#grad-business)"  class="chart-bar" filter="url(#bar-shadow)" @mouseenter="hoveredBar='business'"  @mouseleave="hoveredBar=null"/>
                      <!-- ENTERPRISE -->
                      <rect x="150" :y="140 - (120 * currentBarScale.enterprise)" width="32" :height="120 * currentBarScale.enterprise" rx="8" fill="url(#grad-enterprise)" class="chart-bar" filter="url(#bar-shadow)" @mouseenter="hoveredBar='enterprise'" @mouseleave="hoveredBar=null"/>
                      <!-- GRATUIT -->
                      <rect x="215" :y="140 - (120 * currentBarScale.gratuit)"   width="32" :height="120 * currentBarScale.gratuit"   rx="8" fill="url(#grad-gratuit)"   class="chart-bar" filter="url(#bar-shadow)" @mouseenter="hoveredBar='gratuit'"   @mouseleave="hoveredBar=null"/>
                    </svg>

                    <!-- Bar Tooltip -->
                    <transition name="tooltip-fade">
                      <div v-if="hoveredBar !== null"
                           class="chart-tooltip-pro p-2 text-center"
                           :style="getBarTooltipStyle(hoveredBar)">
                        <div class="super-tiny text-muted fw-800 uppercase">{{ hoveredBar }}</div>
                        <div class="tiny fw-900 text-amber">{{ getBarStats(hoveredBar).pct }}%</div>
                        <div class="super-tiny text-muted">{{ getBarStats(hoveredBar).count }} orgs</div>
                      </div>
                    </transition>
                  </div>

                  <div class="d-flex justify-content-around mt-3 flex-wrap gap-2">
                    <span class="plan-tag tag-indigo">STARTUP</span>
                    <span class="plan-tag tag-amber">BUSINESS</span>
                    <span class="plan-tag tag-emerald">ENTERPRISE</span>
                    <span class="plan-tag tag-slate">GRATUIT</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- ═══ FINANCIAL TABLE ═══ -->
            <div class="enigma-card p-5">
              <div class="d-flex justify-content-between align-items-center mb-5 flex-wrap gap-3">
                <div>
                  <h6 class="fw-900 text-navy mb-1">Dernières Activités Financières</h6>
                  <p class="text-muted super-tiny mb-0 fw-700">Historique des transactions et abonnements récemment validés.</p>
                </div>
                <button class="btn-outline-pro tiny fw-bold px-3 py-2" @click="router.push('/super-admin')">
                  <i class="fa-solid fa-list me-2"></i> Gérer Organisations
                </button>
              </div>

              <div class="table-responsive">
                <table class="table custom-table-pro align-middle mb-0">
                  <thead>
                    <tr>
                      <th>Organisation</th>
                      <th>Formule</th>
                      <th>Moyen de paiement</th>
                      <th>Date d'activation</th>
                      <th>Facturation</th>
                      <th class="text-center">Statut</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="org in recentActivities" :key="org.id" class="table-row-pro">
                      <td>
                        <div class="d-flex align-items-center gap-3">
                          <div class="org-avatar fw-900" :style="{ background: org.color + '18', color: org.color }">
                            {{ org.name[0] }}
                          </div>
                          <div>
                            <div class="fw-800 text-navy tiny">{{ org.name }}</div>
                            <div class="super-tiny text-muted">ID: {{ org.code }}</div>
                          </div>
                        </div>
                      </td>
                      <td>
                        <span class="plan-tag" :class="getPlanTagClass(org.plan)">{{ org.plan }}</span>
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
                        <span class="status-badge status-1 tiny">
                          <span class="status-dot"></span> Actif
                        </span>
                      </td>
                    </tr>
                    <tr v-if="recentActivities.length === 0">
                      <td colspan="6" class="text-center py-5 text-muted">
                        <i class="fa-solid fa-inbox fa-2x mb-3 d-block"></i>
                        <span class="small fw-700">Aucune transaction récente</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>

          </div><!-- /v-else -->
        </div>
      </main>
    </div>

    <!-- ═══ TOAST ═══ -->
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
import { ref, computed, reactive, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { superAdminApi } from '@/services/api';

const router = useRouter();

const loading   = ref(false);
const exporting = ref(false);
const activePeriod = ref('month');
const hoveredPoint = ref(null);
const hoveredBar   = ref(null);
const statsData    = ref(null);
const recentActivities = ref([]);
const mousePos = reactive({ x: 0, y: 0 });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });

const activePeriodLabel = computed(() => ({
  month: '7 Derniers Jours',
  quarter: '30 Derniers Jours',
  year: 'Cette Année (12 mois)'
}[activePeriod.value]));

// ─── KPI DATA ──────────────────────────────────────────────────
const statsByPeriod = ref({
  month: [
    { label: 'Organisations', val: '—', trend: 12, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99,102,241,0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs',  val: '—', trend: 8,  bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217,119,6,0.15)',  icon: 'fa-solid fa-users' },
    { label: 'Tests Passés',  val: '—', trend: 25, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5,150,105,0.15)',  icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements',   val: '— €', trend: 5, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220,38,38,0.15)',  icon: 'fa-solid fa-coins' }
  ],
  quarter: [
    { label: 'Organisations', val: '—', trend: 18, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99,102,241,0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs',  val: '—', trend: 15, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217,119,6,0.15)',  icon: 'fa-solid fa-users' },
    { label: 'Tests Passés',  val: '—', trend: 32, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5,150,105,0.15)',  icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements',   val: '— €', trend: 12, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220,38,38,0.15)', icon: 'fa-solid fa-coins' }
  ],
  year: [
    { label: 'Organisations', val: '—', trend: 45, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99,102,241,0.15)', icon: 'fa-solid fa-building' },
    { label: 'Utilisateurs',  val: '—', trend: 38, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217,119,6,0.15)',  icon: 'fa-solid fa-users' },
    { label: 'Tests Passés',  val: '—', trend: 78, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5,150,105,0.15)',  icon: 'fa-solid fa-wand-magic-sparkles' },
    { label: 'Abonnements',   val: '— €', trend: 28, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220,38,38,0.15)', icon: 'fa-solid fa-coins' }
  ]
});
const currentStats = computed(() => statsByPeriod.value[activePeriod.value]);

// ─── CURVE DATA ────────────────────────────────────────────────
const curveLineDataByPeriod = ref({
  month:   [4200, 6800, 5100, 9400, 8200, 14200],
  quarter: [7200, 9100, 11400, 15300, 13800, 21500],
  year:    [18000, 24000, 29000, 38000, 42000, 54200]
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
  return curveLinePath.value + ` L ${pts[pts.length-1].x} 190 L ${pts[0].x} 190 Z`;
});

// ─── BAR DATA ──────────────────────────────────────────────────
const barScaleByPeriod = ref({
  month:   { startup: 0.60, business: 0.85, enterprise: 0.40, gratuit: 0.30 },
  quarter: { startup: 0.55, business: 0.90, enterprise: 0.50, gratuit: 0.25 },
  year:    { startup: 0.50, business: 0.95, enterprise: 0.65, gratuit: 0.20 }
});
const currentBarScale = computed(() => barScaleByPeriod.value[activePeriod.value]);

const planStats = ref({
  month:   { startup: { pct: 28, count: 14 }, business: { pct: 40, count: 20 }, enterprise: { pct: 19, count: 10 }, gratuit: { pct: 13, count: 7 } },
  quarter: { startup: { pct: 25, count: 18 }, business: { pct: 42, count: 30 }, enterprise: { pct: 24, count: 17 }, gratuit: { pct: 9,  count: 6 } },
  year:    { startup: { pct: 22, count: 45 }, business: { pct: 44, count: 90 }, enterprise: { pct: 31, count: 62 }, gratuit: { pct: 3,  count: 6 } }
});
function getBarStats(bar) { return planStats.value[activePeriod.value][bar]; }
function getBarTooltipStyle(bar) {
  const leftMap = { startup: '13%', business: '37%', enterprise: '60%', gratuit: '83%' };
  return { left: leftMap[bar] || '50%', top: '10px', transform: 'translateX(-50%)', zIndex: 10 };
}

// ─── LOAD REAL DATA ────────────────────────────────────────────
async function loadRealStats() {
  loading.value = true;
  try {
    const res = await superAdminApi.getStats();
    const d   = res.data;

    const updatePeriod = (period, ent, users, tests, rev) => {
      statsByPeriod.value[period] = [
        { label: 'Organisations', val: ent.toString(),            trend: 0, bg: '#e0e7ff', color: '#6366f1', glowColor: 'rgba(99,102,241,0.15)', icon: 'fa-solid fa-building' },
        { label: 'Utilisateurs',  val: formatNumber(users),       trend: 0, bg: '#fef3c7', color: '#d97706', glowColor: 'rgba(217,119,6,0.15)',  icon: 'fa-solid fa-users' },
        { label: 'Tests Passés',  val: formatNumber(tests),       trend: 0, bg: '#d1fae5', color: '#059669', glowColor: 'rgba(5,150,105,0.15)',  icon: 'fa-solid fa-wand-magic-sparkles' },
        { label: 'Abonnements',   val: `${rev.toLocaleString()} €`, trend: 0, bg: '#fee2e2', color: '#dc2626', glowColor: 'rgba(220,38,38,0.15)', icon: 'fa-solid fa-coins' }
      ];
    };

    updatePeriod('month',   d.totalEntreprises7Days  || 0, d.totalUtilisateurs7Days  || 0, d.totalTests7Days  || 0, d.totalRevenus7Days  || 0);
    updatePeriod('quarter', d.totalEntreprises30Days || 0, d.totalUtilisateurs30Days || 0, d.totalTests30Days || 0, d.totalRevenus30Days || 0);
    updatePeriod('year',    d.totalEntreprises       || 0, d.totalUtilisateurs       || 0, d.totalTests       || 0, d.totalRevenus       || 0);

    const st = d.startupCount || 0, bs = d.businessCount || 0, ep = d.enterpriseCount || 0, gr = d.gratuitCount || 0;
    const tp = (st + bs + ep + gr) || 1;
    const scales = { startup: st/tp, business: bs/tp, enterprise: ep/tp, gratuit: gr/tp };
    barScaleByPeriod.value.month = barScaleByPeriod.value.quarter = barScaleByPeriod.value.year = scales;

    const pStats = {
      startup:    { pct: Math.round((st/tp)*100), count: st },
      business:   { pct: Math.round((bs/tp)*100), count: bs },
      enterprise: { pct: Math.round((ep/tp)*100), count: ep },
      gratuit:    { pct: Math.round((gr/tp)*100), count: gr }
    };
    planStats.value.month = planStats.value.quarter = planStats.value.year = pStats;

    if (d.recentTransactions?.length) {
      recentActivities.value = d.recentTransactions.map((t, i) => ({
        id: t.id || i,
        name: t.name || 'Inconnue',
        code: `ORG-${(t.name || 'INC').substring(0, 3).toUpperCase()}`,
        plan: t.plan || 'GRATUIT',
        paymentMethod: t.plan?.toLowerCase() === 'gratuit' ? 'Aucun' : 'Stripe / Bank',
        paymentIcon:   t.plan?.toLowerCase() === 'gratuit' ? 'fa-solid fa-slash' : 'fa-brands fa-stripe',
        date:  t.date  || 'Récemment',
        price: t.price || '0',
        color: t.color || '#6366f1'
      }));
    }

    if (d.monthlyRevenues?.length) {
      curveLineDataByPeriod.value.month   = d.monthlyRevenues;
      curveLineDataByPeriod.value.quarter = d.monthlyRevenues;
      curveLineDataByPeriod.value.year    = d.monthlyRevenues;
    }

    const mNames = ['Jan','Fév','Mar','Avr','Mai','Jun','Jui','Aoû','Sep','Oct','Nov','Déc'];
    const labels = [], cm = new Date().getMonth();
    for (let i = 5; i >= 0; i--) labels.push(mNames[(cm - i + 12) % 12]);
    monthsLabels.value = labels;

  } catch (err) {
    console.error('Stats load error', err);
    showToast('Erreur lors du chargement des statistiques.', 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    loading.value = false;
  }
}

function formatNumber(n) {
  if (n >= 1000000) return `${(n/1000000).toFixed(1)}M`;
  if (n >= 1000)    return `${(n/1000).toFixed(1)}k`;
  return (n || 0).toString();
}

function setPeriod(p) {
  activePeriod.value = p;
  loading.value = true;
  setTimeout(() => {
    loading.value = false;
    showToast('Statistiques actualisées.', 'success', 'fa-solid fa-check');
  }, 400);
}

function refreshData() {
  loadRealStats();
  showToast('Métriques rafraîchies.', 'success', 'fa-solid fa-rotate');
}

function getPlanTagClass(plan) {
  if (plan === 'ENTERPRISE') return 'tag-emerald';
  if (plan === 'BUSINESS')   return 'tag-amber';
  if (plan === 'GRATUIT')    return 'tag-slate';
  return 'tag-indigo';
}

// ─── EXPORT ────────────────────────────────────────────────────
function simulateExport() {
  exporting.value = true;
  setTimeout(() => {
    exporting.value = false;
    const dateStr  = new Date().toLocaleDateString('fr-FR', { year: 'numeric', month: 'long', day: 'numeric' });
    const content  = `<!DOCTYPE html><html><head><meta charset="utf-8"><title>Rapport — EvaluaTech</title>
<style>body{font-family:system-ui,sans-serif;color:#0f172a;padding:40px;background:#f8fafc;}
.wrap{max-width:900px;margin:0 auto;background:#fff;padding:40px;border-radius:12px;box-shadow:0 4px 24px rgba(0,0,0,.07);}
.h1{font-size:24px;font-weight:900;color:#1e3a8a;}.sub{font-size:12px;color:#64748b;margin-top:4px;}
.grid{display:grid;grid-template-columns:repeat(4,1fr);gap:12px;margin:24px 0;}
.card{background:#f1f5f9;border-radius:10px;padding:16px;text-align:center;}
.card-lbl{font-size:10px;color:#64748b;font-weight:800;text-transform:uppercase;margin-bottom:6px;}
.card-val{font-size:22px;font-weight:900;color:#1e3a8a;}
.foot{margin-top:40px;font-size:10px;color:#94a3b8;text-align:center;border-top:1px solid #e2e8f0;padding-top:16px;}
</style></head><body><div class="wrap">
<div class="h1">Rapport Financier Global</div><div class="sub">Généré le ${dateStr}</div>
<div class="grid">${currentStats.value.map(s=>`<div class="card"><div class="card-lbl">${s.label}</div><div class="card-val">${s.val}</div></div>`).join('')}</div>
<div class="foot">EvaluaTech SaaS Platform &copy; ${new Date().getFullYear()} — Confidentiel</div>
</div></body></html>`;
    const blob = new Blob([content], { type: 'text/html' });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href = url; a.download = `rapport_${activePeriod.value}_${new Date().toISOString().slice(0,10)}.html`;
    document.body.appendChild(a); a.click(); document.body.removeChild(a);
    URL.revokeObjectURL(url);
    showToast('Rapport exporté avec succès !', 'success', 'fa-solid fa-download');
  }, 1000);
}

// ─── PARALLAX ──────────────────────────────────────────────────
const orbStyle      = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => { mousePos.x = (e.clientX - window.innerWidth/2)/20; mousePos.y = (e.clientY - window.innerHeight/2)/20; };

// ─── TOAST ─────────────────────────────────────────────────────
let _tt = null;
function showToast(msg, type = 'success', icon = 'fa-solid fa-check') {
  clearTimeout(_tt);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _tt = setTimeout(() => { globalToast.active = false; }, 4000);
}

onMounted(loadRealStats);
</script>

<style scoped>
/* ═══════════════════════════════════════════════════
   VARIABLES
═══════════════════════════════════════════════════ */
:root {
  --border-chart: #eef2f6;
  --text-chart: #94a3b8;
}

/* ═══════════════════════════════════════════════════
   ROOT / BACKGROUND
═══════════════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image:
    radial-gradient(circle, #e2e8f0 1.5px, transparent 1.5px),
    linear-gradient(rgba(245,158,11,0.03) 1px, transparent 1px),
    linear-gradient(90deg, rgba(99,102,241,0.03) 1px, transparent 1px);
  background-size: 40px 40px, 80px 80px, 80px 80px;
  opacity: 0.4;
  animation: gridDrift 60s linear infinite;
}
@keyframes gridDrift {
  0%   { background-position: 0 0, 0 0, 0 0; }
  100% { background-position: 40px 40px, 80px 80px, 80px 80px; }
}
.glow-orb { position: absolute; width: 700px; height: 700px; filter: blur(140px); opacity: 0.12; border-radius: 50%; transition: transform 0.3s ease-out; }
.orb-amber { background: radial-gradient(circle, #fbbf24, #f59e0b); top: -250px; right: -150px; }
.orb-blue  { background: radial-gradient(circle, #818cf8, #6366f1); bottom: -250px; left: -150px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }
.dashboard-view {}

/* ═══════════════════════════════════════════════════
   HEADER
═══════════════════════════════════════════════════ */
.premium-title {
  font-weight: 900;
  font-size: 2.2rem;
  letter-spacing: -1px;
  color: #0f172a;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fb923c 50%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; transition: color 0.2s; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ═══════════════════════════════════════════════════
   BUTTONS
═══════════════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none; padding: 12px 24px;
  border-radius: 18px; font-weight: 800; font-size: 0.82rem;
  position: relative; overflow: hidden; cursor: pointer; font-family: inherit; transition: all 0.3s;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fb923c, #fbbf24);
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary:hover { transform: translateY(-2px); box-shadow: 0 12px 30px rgba(245,158,11,0.3); }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; transform: none !important; }

.btn-glass-pro {
  background: white; border: 1.5px solid #eef2f6; color: #0f172a;
  border-radius: 14px; font-weight: 700; font-size: 0.78rem; transition: all 0.25s; cursor: pointer;
}
.btn-glass-pro:hover { border-color: #f59e0b; background: #fffbeb; transform: translateY(-1px); }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white; border: 1.5px solid #eef2f6;
  border-radius: 14px; color: #64748b; cursor: pointer; transition: all 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover { background: #fffbeb; border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg) scale(1.05); }
.fa-spin-active i { animation: spin 0.8s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }

.btn-outline-pro {
  background: white; color: #0f172a; border: 1.5px solid #eef2f6;
  padding: 10px 18px; border-radius: 14px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; transition: all 0.2s; font-family: inherit;
}
.btn-outline-pro:hover { border-color: #0f172a; transform: translateY(-1px); }

.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

/* ═══════════════════════════════════════════════════
   DROPDOWN
═══════════════════════════════════════════════════ */
.glass-dropdown-pro {
  background: white !important; border: 1.5px solid #eef2f6 !important;
  border-radius: 18px !important; padding: 8px !important; min-width: 210px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1) !important;
}
.glass-dropdown-pro .dropdown-item {
  border-radius: 12px; padding: 10px 14px; font-size: 0.78rem;
  font-weight: 700; color: #0f172a; transition: all 0.2s;
}
.glass-dropdown-pro .dropdown-item:hover { background: #fffbeb; color: #f59e0b; }
.text-amber { color: #f59e0b !important; }

/* ═══════════════════════════════════════════════════
   SPINNER
═══════════════════════════════════════════════════ */
.spinner-pro-premium {
  width: 52px; height: 52px;
  border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b;
  border-radius: 50%; animation: spin 0.8s linear infinite;
}

/* ═══════════════════════════════════════════════════
   KPI STAT CARDS
═══════════════════════════════════════════════════ */
.stat-card-premium {
  background: white; border-radius: 30px; padding: 28px;
  border: 1px solid #eef2f6;
  box-shadow: 0 1px 3px rgba(0,0,0,0.04);
  transition: all 0.35s cubic-bezier(0.4,0,0.2,1);
  position: relative; overflow: hidden;
}
.stat-card-premium::before {
  content: '';
  position: absolute; top: 0; left: 0; right: 0; height: 3px;
  background: linear-gradient(90deg, transparent, #f59e0b, transparent);
  opacity: 0; transition: opacity 0.3s;
}
.stat-card-premium:hover::before { opacity: 1; }
.stat-card-premium:hover {
  transform: translateY(-6px);
  box-shadow: 0 24px 50px rgba(0,0,0,0.08);
  border-color: rgba(245,158,11,0.2);
}
.stat-glow-orb {
  position: absolute; top: -40px; right: -40px;
  width: 120px; height: 120px; border-radius: 50%;
  filter: blur(50px); z-index: 0; pointer-events: none;
}
.stat-label-top { font-size: 0.6rem; letter-spacing: 0.08em; position: relative; z-index: 1; }
.stat-icon-wrapper {
  width: 52px; height: 52px; border-radius: 18px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.2rem; flex-shrink: 0; position: relative; z-index: 1;
  box-shadow: 0 4px 10px rgba(0,0,0,0.07); transition: transform 0.3s;
}
.stat-card-premium:hover .stat-icon-wrapper { transform: scale(1.1) rotate(-5deg); }
.stat-value {
  font-size: 2rem; font-weight: 900; color: #0f172a;
  letter-spacing: -1.5px; line-height: 1; position: relative; z-index: 1;
}
.trend-badge { font-size: 0.62rem; display: inline-flex; align-items: center; gap: 4px; }
.trend-up   { background: rgba(16,185,129,0.1); color: #059669; }
.trend-down { background: rgba(239,68,68,0.1);  color: #dc2626; }

/* ═══════════════════════════════════════════════════
   CARDS
═══════════════════════════════════════════════════ */
.enigma-card {
  background: white; border: 1px solid #eef2f6;
  border-radius: 32px; box-shadow: 0 1px 4px rgba(0,0,0,0.04);
  transition: box-shadow 0.3s;
}
.text-navy { color: #0f172a; }

/* ═══════════════════════════════════════════════════
   CHARTS
═══════════════════════════════════════════════════ */
.chart-container { position: relative; }
.area-transition { transition: d 0.5s cubic-bezier(0.4,0,0.2,1); }
.line-transition { transition: d 0.5s cubic-bezier(0.4,0,0.2,1); }
.chart-point { transition: r 0.2s ease; cursor: pointer; }
.chart-bar {
  cursor: pointer; transition: opacity 0.2s, filter 0.2s;
}
.chart-bar:hover { opacity: 0.8; filter: brightness(1.08) drop-shadow(0 8px 12px rgba(0,0,0,0.18)); }

.chart-tooltip-pro {
  position: absolute; background: #0f172a; color: white;
  border-radius: 14px; z-index: 100; pointer-events: none; min-width: 88px;
  box-shadow: 0 10px 24px rgba(0,0,0,0.25);
  border: 1px solid rgba(255,255,255,0.06);
}
.chart-tooltip-pro::after {
  content: '';
  position: absolute; bottom: -5px; left: 50%; transform: translateX(-50%);
  border-width: 5px 5px 0; border-style: solid;
  border-color: #0f172a transparent; width: 0; display: block;
}
.tooltip-fade-enter-active { transition: opacity 0.15s ease, transform 0.15s ease; }
.tooltip-fade-leave-active { transition: opacity 0.1s ease; }
.tooltip-fade-enter-from   { opacity: 0; transform: translateY(4px); }
.tooltip-fade-leave-to     { opacity: 0; }

.legend-dot-sm { width: 10px; height: 10px; border-radius: 50%; display: inline-block; flex-shrink: 0; }

/* ═══════════════════════════════════════════════════
   PLAN TAGS
═══════════════════════════════════════════════════ */
.plan-tag {
  font-size: 0.6rem; font-weight: 900; padding: 4px 10px;
  border-radius: 8px; display: inline-block;
  letter-spacing: 0.03em; text-transform: uppercase;
}
.tag-indigo  { background: rgba(99,102,241,0.1);  color: #6366f1; }
.tag-amber   { background: rgba(245,158,11,0.1);  color: #d97706; }
.tag-emerald { background: rgba(16,185,129,0.1);  color: #059669; }
.tag-slate   { background: rgba(100,116,139,0.1); color: #475569; }

/* ═══════════════════════════════════════════════════
   TABLE
═══════════════════════════════════════════════════ */
.custom-table-pro thead tr th {
  border-bottom: 2px solid #eef2f6; padding: 10px 18px;
  font-size: 0.6rem; font-weight: 900; letter-spacing: 0.07em;
  text-transform: uppercase; color: #94a3b8; background: transparent;
}
.custom-table-pro tbody td { padding: 16px 18px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; }
.table-row-pro { transition: background 0.15s; }
.table-row-pro:hover td { background-color: #fffbeb !important; }
.table-row-pro:last-child td { border-bottom: none; }

.org-avatar {
  width: 42px; height: 42px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1rem; flex-shrink: 0; transition: transform 0.2s;
}
.table-row-pro:hover .org-avatar { transform: scale(1.08); }

/* Status badge reused from Campagne style */
.status-badge {
  padding: 5px 13px; border-radius: 11px; font-size: 0.62rem;
  font-weight: 800; text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-1 {
  background: #ecfdf5; color: #10b981; border: 1px solid rgba(16,185,129,0.15);
  animation: statusPulse 3s ease-in-out infinite;
}
@keyframes statusPulse { 0%,100% { box-shadow: 0 0 0 0 rgba(16,185,129,0); } 50% { box-shadow: 0 0 0 4px rgba(16,185,129,0.1); } }
.status-dot {
  width: 6px; height: 6px; border-radius: 50%; background: currentColor; margin-right: 7px;
  animation: blink 1.5s ease-in-out infinite;
}
@keyframes blink { 0%,100% { opacity: 1; } 50% { opacity: 0.3; } }

/* ═══════════════════════════════════════════════════
   ANIMATION
═══════════════════════════════════════════════════ */
.animate__animated.animate__fadeIn { animation: fadeIn 0.4s ease forwards; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: translateY(0); } }

/* ═══════════════════════════════════════════════════
   TOAST
═══════════════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: linear-gradient(135deg, #0f172a, #1e293b);
  color: white; padding: 20px 28px; border-radius: 20px;
  display: flex; align-items: center; gap: 14px; z-index: 3000;
  border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.25);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s cubic-bezier(0.4,0,0.2,1); }
.toast-slide-leave-active { animation: slideIn 0.3s cubic-bezier(0.4,0,0.2,1) reverse; }
@keyframes slideIn { from { transform: translateX(120%) scale(0.9); opacity: 0; } to { transform: translateX(0) scale(1); opacity: 1; } }

/* ═══════════════════════════════════════════════════
   SCROLLBAR
═══════════════════════════════════════════════════ */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #f59e0b; }

/* ═══════════════════════════════════════════════════
   UTILS
═══════════════════════════════════════════════════ */
.tiny       { font-size: 0.78rem; }
.super-tiny { font-size: 0.65rem; }
.uppercase  { text-transform: uppercase; }
.tracking-wider { letter-spacing: 0.05em; }
.fw-600 { font-weight: 600 !important; }
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.relative { position: relative; }
.w-100 { width: 100%; }
.h-100 { height: 100%; }
.overflow-visible { overflow: visible; }
.cursor-pointer { cursor: pointer; }

/* ═══════════════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .stat-card-premium { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }
[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .text-navy { color: #f0f6fc; }
[data-theme="dark"] .btn-glass-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-glass-pro:hover { background: rgba(245,158,11,0.1); border-color: #f59e0b; }
[data-theme="dark"] .btn-refresh-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .btn-outline-pro { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .glass-dropdown-pro { background: #161b22 !important; border-color: rgba(255,255,255,0.1) !important; }
[data-theme="dark"] .glass-dropdown-pro .dropdown-item { color: #f0f6fc; }
[data-theme="dark"] .glass-dropdown-pro .dropdown-item:hover { background: rgba(245,158,11,0.1); color: #f59e0b; }
[data-theme="dark"] .custom-table-pro thead tr th { color: #8b949e; border-bottom-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .custom-table-pro tbody td { border-bottom-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .table-row-pro:hover td { background-color: rgba(245,158,11,0.05) !important; }
[data-theme="dark"] .spinner-pro-premium { border-color: rgba(255,255,255,0.08); border-top-color: #f59e0b; }
[data-theme="dark"] :root { --border-chart: rgba(255,255,255,0.07); --text-chart: #8b949e; }
</style>