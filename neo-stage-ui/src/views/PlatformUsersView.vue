<template>
  <div class="enigma-master-root d-flex overflow-hidden" :data-theme="currentTheme">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber"></div>
      <div class="glow-orb orb-blue"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- ═══════════════════════════════════════════════════
               HEADER
          ═══════════════════════════════════════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">{{ t('rolesView.breadcrumb.admin') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ t('platformUsers.breadcrumbCurrent') }}</span>
              </div>
              <h2 class="premium-title">
                {{ t('platformUsers.titlePrefix') }}
                <span class="gradient-text">{{ t('platformUsers.titleHighlight') }}</span>
              </h2>
              <p class="text-muted-pro small m-0">{{ t('platformUsers.subtitle') }}</p>
            </div>
            <div class="d-flex gap-3 flex-wrap align-items-center">
              <!-- Theme Toggle -->
              <button class="btn-theme-toggle" @click="toggleTheme"
                :title="currentTheme === 'dark' ? t('theme.light') : t('theme.dark')">
                <i :class="currentTheme === 'dark' ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
              </button>
              <!-- Lang Toggle -->
              <div class="lang-cluster">
                <button
                  v-for="loc in availableLocales"
                  :key="loc.code"
                  :class="['btn-lang', { active: locale === loc.code }]"
                  @click="switchLocale(loc.code)"
                  :title="loc.nativeName"
                >
                  {{ loc.flag }}
                </button>
              </div>
              <!-- View Toggle -->
              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'table' }]"
                  @click="viewMode = 'table'" :title="t('staff.viewTable')">
                  <i class="fa-solid fa-table-list"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'grid' }]"
                  @click="viewMode = 'grid'" :title="t('staff.viewGrid')">
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'analytics' }]"
                  @click="viewMode = 'analytics'" :title="t('platformUsers.viewAnalytics')">
                  <i class="fa-solid fa-chart-simple"></i>
                </button>
              </div>
              <button @click="fetchData" class="btn-refresh-pro" :disabled="loading" :title="t('refresh')">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
            </div>
          </header>

          <!-- ═══════════════════════════════════════════════════
               KPI STATS
          ═══════════════════════════════════════════════════ -->
          <div class="row g-4 mb-5">
            <div class="col-xl-4 col-md-6" v-for="stat in platformStats" :key="stat.labelKey">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details ms-3">
                  <div class="stat-value">{{ stat.val }}</div>
                  <div class="stat-label">{{ t(stat.labelKey) }}</div>
                </div>
                <div v-if="stat.trend" class="stat-trend ms-auto">
                  <i class="fa-solid fa-bolt-lightning" style="font-size:0.6rem"></i>
                  <span>{{ t('platformUsers.trendLive') }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════════════
               ANALYTICS VIEW
          ═══════════════════════════════════════════════════ -->
          <div v-if="viewMode === 'analytics'" class="analytics-overview-panel mb-5 animate__animated animate__fadeIn">
            <div class="row g-4">
              <div class="col-lg-8">
                <div class="analytics-card-pro p-4">
                  <div class="d-flex justify-content-between align-items-center mb-4">
                    <h6 class="fw-800 m-0">{{ t('platformUsers.roleDistribution') }}</h6>
                    <div class="d-flex gap-2 align-items-center">
                      <span class="legend-dot dot-amber"></span>
                      <span class="small text-muted">{{ t('platformUsers.activeLabel') }}</span>
                      <span class="legend-dot dot-indigo ms-3"></span>
                      <span class="small text-muted">{{ t('platformUsers.inactiveLabel') }}</span>
                    </div>
                  </div>
                  <div class="bar-chart-v2">
                    <div v-for="(bar, i) in roleBarData" :key="i" class="bar-col">
                      <div class="bar-wrap">
                        <div class="bar-fill bar-amber" :style="{ height: bar.active + '%' }"></div>
                        <div class="bar-fill bar-indigo" :style="{ height: bar.inactive + '%' }"></div>
                      </div>
                      <span class="bar-label">{{ bar.label }}</span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-lg-4">
                <div class="analytics-card-pro p-4 h-100">
                  <h6 class="fw-800 mb-4">{{ t('platformUsers.globalStatus') }}</h6>
                  <div class="donut-chart-container">
                    <svg viewBox="0 0 120 120" width="120">
                      <circle v-for="(seg, i) in donutSegments" :key="i"
                        cx="60" cy="60" r="45"
                        :stroke="seg.color" stroke-width="20" fill="none"
                        :stroke-dasharray="`${seg.dash} ${283 - seg.dash}`"
                        :stroke-dashoffset="seg.offset"
                        style="transition: stroke-dasharray 0.6s ease"/>
                      <text x="60" y="64" text-anchor="middle" class="donut-center-text">{{ users.length }}</text>
                      <text x="60" y="75" text-anchor="middle" class="donut-sub-text">{{ t('platformUsers.total') }}</text>
                    </svg>
                    <div class="donut-legend">
                      <div v-for="seg in donutSegments" :key="seg.labelKey" class="donut-legend-item">
                        <span class="legend-dot-sm" :style="{ background: seg.color }"></span>
                        <span class="small">{{ t(seg.labelKey) }}</span>
                        <span class="ms-auto fw-800 small">{{ seg.count }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════════════
               TOOLBAR
          ═══════════════════════════════════════════════════ -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border tabs-wrap">
              <button v-for="tab in filterTabs" :key="tab.value"
                class="nav-tab-btn-modern" :class="{ active: activeTab === tab.value }"
                @click="activeTab = tab.value">
                {{ tab.label }} <span class="tab-count">{{ tab.count }}</span>
              </button>
            </div>
            <div class="d-flex gap-2">
              <div class="search-inline-box">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" v-model="searchQuery"
                  :placeholder="t('search')"
                  class="search-inline-input">
                <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                  <i class="fa-solid fa-xmark"></i>
                </button>
              </div>
              <select v-model="selectedRole" class="sort-select-pro">
                <option>{{ t('platformUsers.allRoles') }}</option>
                <option>SuperAdmin</option>
                <option>AdminEntreprise</option>
                <option>Evaluateur</option>
                <option>Candidat</option>
              </select>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════════════
               GRID VIEW
          ═══════════════════════════════════════════════════ -->
          <div v-if="viewMode === 'grid'" class="row g-4">
            <div v-if="loading" class="col-12 text-center py-5">
              <div class="spinner-pro-premium"></div>
            </div>
            <div v-else-if="filteredUsers.length === 0" class="col-12">
              <div class="empty-state-pro py-5 text-center">
                <i class="fa-solid fa-users fa-3x text-muted mb-3"></i>
                <h5 class="fw-800">{{ t('platformUsers.emptyTitle') }}</h5>
                <p class="text-muted">{{ t('platformUsers.emptySubtitle') }}</p>
              </div>
            </div>
            <div v-else v-for="u in filteredUsers" :key="u.id"
              class="col-xl-4 col-md-6 animate__animated animate__fadeInUp">
              <div class="user-card-modern">
                <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                  <span class="status-badge-user" :class="u.isActive ? 'status-active' : 'status-inactive'">
                    <span class="status-dot"></span>
                    {{ u.isActive ? t('platformUsers.active') : t('platformUsers.inactive') }}
                  </span>
                  <div class="dropdown">
                    <button class="btn-options-round" data-bs-toggle="dropdown" @click.stop>
                      <i class="fa-solid fa-ellipsis-vertical"></i>
                    </button>
                    <ul class="dropdown-menu border-0 shadow-premium p-2 rounded-4">
                      <li>
                        <button class="dropdown-item rounded-3" @click="handleToggleStatus(u)">
                          <i class="fa-solid fa-toggle-on me-2"></i>{{ t('platformUsers.changeStatus') }}
                        </button>
                      </li>
                      <li><hr class="dropdown-divider"></li>
                      <li>
                        <button class="dropdown-item rounded-3 text-danger" @click="handleDelete(u)">
                          <i class="fa-solid fa-trash-can me-2"></i>{{ t('delete') }}
                        </button>
                      </li>
                    </ul>
                  </div>
                </div>

                <div class="d-flex align-items-center gap-3 mb-3">
                  <div class="avatar-card-lg">
                    {{ u.name?.[0] || '?' }}
                    <span v-if="u.lastLogin === 'En ligne'" class="online-dot-card"></span>
                  </div>
                  <div class="overflow-hidden">
                    <h5 class="user-title-card fw-800 text-truncate mb-0">{{ u.name }}</h5>
                    <div class="text-muted small text-truncate">{{ u.email }}</div>
                  </div>
                </div>

                <div class="d-flex align-items-center gap-2 mb-3">
                  <span class="org-badge">
                    <i class="fa-solid fa-building me-1 opacity-50"></i>{{ u.org }}
                  </span>
                </div>

                <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3 border-top border-light">
                  <span :class="['role-chip', getRoleClass(u.role)]">{{ u.role }}</span>
                  <div class="d-flex align-items-center gap-2">
                    <div class="form-check form-switch custom-switch m-0">
                      <input class="form-check-input" type="checkbox"
                        :checked="u.isActive" @change="handleToggleStatus(u)" style="cursor:pointer">
                    </div>
                    <button @click="handleDelete(u)" class="btn-icon-sm danger">
                      <i class="fa-solid fa-trash-can"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════════════
               TABLE VIEW
          ═══════════════════════════════════════════════════ -->
          <div v-if="viewMode === 'table'" class="animate__animated animate__fadeIn">
            <div v-if="loading" class="text-center py-5">
              <div class="spinner-pro-premium"></div>
            </div>
            <template v-else>
              <!-- List Header -->
              <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
                <span style="flex:2"      class="list-col-label">{{ t('platformUsers.colUser') }}</span>
                <span style="flex:1.5"    class="list-col-label">{{ t('platformUsers.colOrg') }}</span>
                <span style="width:130px" class="list-col-label">{{ t('role') }}</span>
                <span style="width:140px" class="list-col-label">{{ t('platformUsers.colLastLogin') }}</span>
                <span style="width:80px"  class="list-col-label text-center">{{ t('status') }}</span>
                <span style="width:60px"  class="list-col-label text-center">{{ t('actions') }}</span>
              </div>
              <div v-if="filteredUsers.length === 0" class="text-center py-4 text-muted">
                {{ t('platformUsers.emptyTitle') }}
              </div>
              <div v-for="u in filteredUsers" :key="u.id"
                class="list-row-item d-flex align-items-center px-4 py-3 mb-2">
                <div style="flex:2" class="d-flex align-items-center gap-3">
                  <div class="avatar-sm-list">
                    {{ u.name?.[0] || '?' }}
                    <span v-if="u.lastLogin === 'En ligne'" class="online-dot-list"></span>
                  </div>
                  <div>
                    <div class="fw-800 small">{{ u.name }}</div>
                    <div class="text-muted" style="font-size:0.7rem">{{ u.email }}</div>
                  </div>
                </div>
                <div style="flex:1.5">
                  <span class="org-badge">
                    <i class="fa-solid fa-building me-1 opacity-50"></i>{{ u.org }}
                  </span>
                </div>
                <div style="width:130px">
                  <span :class="['role-chip', getRoleClass(u.role)]">{{ u.role }}</span>
                </div>
                <div style="width:140px" class="small text-muted d-flex align-items-center gap-1">
                  <i class="fa-regular fa-clock" style="font-size:0.65rem"></i> {{ u.lastLogin }}
                </div>
                <div style="width:80px" class="text-center">
                  <div class="form-check form-switch custom-switch d-flex justify-content-center m-0">
                    <input class="form-check-input" type="checkbox"
                      :checked="u.isActive" @change="handleToggleStatus(u)" style="cursor:pointer">
                  </div>
                </div>
                <div style="width:60px" class="d-flex gap-1 justify-content-center">
                  <button @click="handleDelete(u)" class="btn-icon-sm danger" :title="t('delete')">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>
              <!-- Footer -->
              <div class="table-footer-bar mt-3 d-flex justify-content-between align-items-center px-2">
                <span class="tiny text-muted fw-bold uppercase">
                  {{ t('platformUsers.showing', { shown: filteredUsers.length, total: users.length }) }}
                </span>
                <div class="pagination-dots">
                  <span class="dot active"></span>
                  <span class="dot"></span>
                  <span class="dot"></span>
                </div>
              </div>
            </template>
          </div>

        </div>
      </main>
    </div>

    <!-- CONFIRM DIALOG -->
    <transition name="modal-quantum">
      <div v-if="confirmDialog.show" class="quantum-vault-overlay"
        @click.self="confirmDialog.show = false">
        <div class="confirm-modal animate__animated animate__zoomIn animate__faster">
          <div class="confirm-icon mb-3">
            <i :class="confirmDialog.icon" class="fa-2x text-danger"></i>
          </div>
          <h5 class="fw-900 mb-2">{{ confirmDialog.title }}</h5>
          <p class="text-muted small mb-4">{{ confirmDialog.message }}</p>
          <div class="d-flex gap-3 justify-content-center">
            <button @click="confirmDialog.show = false" class="btn-qv-cancel">
              {{ t('cancel').toUpperCase() }}
            </button>
            <button @click="runConfirmDialog" class="btn-confirm-danger">
              {{ t('confirm').toUpperCase() }}
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
          <strong>{{ t('dashboard.toast.systemMessage') }}</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import { superAdminApi } from '@/services/api';
import { ALL_LOCALES, setUserLocale } from '@/i18n';

// ── i18n ──────────────────────────────────────────────────────────────────
const { t, locale } = useI18n();

const availableLocales = ALL_LOCALES;

const switchLocale = (code) => {
  if (setUserLocale(code)) {
    locale.value = code;
    document.documentElement.setAttribute(
      'dir',
      ALL_LOCALES.find(l => l.code === code)?.dir || 'ltr'
    );
  }
};

// ── Theme ─────────────────────────────────────────────────────────────────
import { inject } from 'vue';
const isDark = inject('isDark', ref(false));
const globalToggleTheme = inject('toggleTheme', () => {});
const currentTheme = computed(() => isDark.value ? 'dark' : 'light');

const toggleTheme = () => {
  globalToggleTheme();
};

// ── State ─────────────────────────────────────────────────────────────────
const loading      = ref(false);
const searchQuery  = ref('');
const selectedRole = ref('');
const viewMode     = ref('table');
const activeTab    = ref('all');

// ── KPI Stats ─────────────────────────────────────────────────────────────
const platformStats = ref([
  {
    labelKey: 'dashboard.kpis.users',
    val: '0',
    icon: 'fa-solid fa-users',
    bg: 'rgba(59,130,246,0.1)',
    color: '#3b82f6',
    trend: true,
    key: 'totalUtilisateurs',
  },
  {
    labelKey: 'dashboard.kpis.companies',
    val: '0',
    icon: 'fa-solid fa-building-circle-check',
    bg: 'rgba(16,185,129,0.1)',
    color: '#10b981',
    trend: true,
    key: 'totalEntreprises',
  },
  {
    labelKey: 'dashboard.kpis.sessions',
    val: '0',
    icon: 'fa-solid fa-bolt-lightning',
    bg: 'rgba(245,158,11,0.1)',
    color: '#f59e0b',
    trend: true,
    key: 'sessionsIARecentes',
  },
]);

const users = ref([]);

// ── Fetch ─────────────────────────────────────────────────────────────────
const fetchData = async () => {
  loading.value = true;
  try {
    const [resStats, resUsers] = await Promise.all([
      superAdminApi.getStats(),
      superAdminApi.getPlatformUsers(),
    ]);
    platformStats.value.forEach(s => { s.val = resStats.data[s.key] || 0; });
    users.value = resUsers.data;
  } catch {
    showPulseToast(
      t('staff.toast.loadError'),
      'error',
      'fa-solid fa-triangle-exclamation'
    );
  } finally {
    loading.value = false;
  }
};

// ── Filter Tabs ───────────────────────────────────────────────────────────
const filterTabs = computed(() => [
  { label: t('all'),                       value: 'all',      count: users.value.length },
  { label: t('platformUsers.activeLabel'), value: 'active',   count: users.value.filter(u => u.isActive).length },
  { label: t('platformUsers.inactiveLabel'),value: 'inactive', count: users.value.filter(u => !u.isActive).length },
]);

// ── Filtered Users ────────────────────────────────────────────────────────
const filteredUsers = computed(() => {
  const allRolesLabel = t('platformUsers.allRoles');
  return users.value.filter(u => {
    const q = searchQuery.value.toLowerCase();
    const matchSearch =
      u.name?.toLowerCase().includes(q) ||
      u.email?.toLowerCase().includes(q) ||
      u.org?.toLowerCase().includes(q);
    const matchRole =
      selectedRole.value === allRolesLabel || !selectedRole.value || u.role === selectedRole.value;
    const matchTab =
      activeTab.value === 'all' ||
      (activeTab.value === 'active'   && u.isActive) ||
      (activeTab.value === 'inactive' && !u.isActive);
    return matchSearch && matchRole && matchTab;
  });
});

// ── Donut Chart ───────────────────────────────────────────────────────────
const donutSegments = computed(() => {
  const active   = users.value.filter(u => u.isActive).length;
  const inactive = users.value.length - active;
  const total    = users.value.length || 1;
  const circ     = 283;
  return [
    {
      labelKey: 'platformUsers.activeLabel',
      count: active,
      color: '#10b981',
      dash: (active / total) * circ,
      offset: circ / 4,
    },
    {
      labelKey: 'platformUsers.inactiveLabel',
      count: inactive,
      color: '#f43f5e',
      dash: (inactive / total) * circ,
      offset: circ / 4 - (active / total) * circ,
    },
  ];
});

// ── Bar Chart ─────────────────────────────────────────────────────────────
const roleBarData = computed(() => {
  const roles = ['SuperAdmin', 'AdminEntreprise', 'Evaluateur', 'Candidat'];
  const max   = users.value.length || 1;
  return roles.map(role => {
    const active   = users.value.filter(u => u.role === role && u.isActive).length;
    const inactive = users.value.filter(u => u.role === role && !u.isActive).length;
    return {
      label:    role === 'AdminEntreprise' ? 'Admin' : role,
      active:   Math.round((active / max) * 100),
      inactive: Math.round((inactive / max) * 100),
    };
  });
});

// ── Actions ───────────────────────────────────────────────────────────────
const handleDelete = (user) => {
  showConfirmDialog(
    `${t('platformUsers.deleteConfirmTitle')} ${user.name} ?`,
    t('platformUsers.deleteConfirmMsg'),
    'fa-solid fa-trash-can',
    async () => {
      try {
        await superAdminApi.deleteUser(user.id);
        showPulseToast(t('staff.toast.deleted'), 'success', 'fa-solid fa-check');
        fetchData();
      } catch {
        showPulseToast(t('staff.toast.deleteError'), 'error', 'fa-solid fa-xmark');
      }
    }
  );
};

const handleToggleStatus = async (user) => {
  try {
    await superAdminApi.toggleUserStatus(user.id);
    user.isActive = !user.isActive;
    showPulseToast(
      `${t('platformUsers.statusUpdated')} ${user.name}`,
      'success',
      'fa-solid fa-check'
    );
  } catch {
    showPulseToast(t('platformUsers.statusError'), 'error', 'fa-solid fa-xmark');
    fetchData();
  }
};

const getRoleClass = (role) => ({
  SuperAdmin:      'role-super',
  AdminEntreprise: 'role-admin',
  Evaluateur:      'role-eval',
  Candidat:        'role-cand',
}[role] || '');

// ── Confirm Dialog ────────────────────────────────────────────────────────
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _cb: null });
const showConfirmDialog = (title, message, icon, cb) =>
  Object.assign(confirmDialog, { title, message, icon, _cb: cb, show: true });
const runConfirmDialog = () => { confirmDialog.show = false; confirmDialog._cb?.(); };

// ── Toast ─────────────────────────────────────────────────────────────────
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });
let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

// ── Init ──────────────────────────────────────────────────────────────────
onMounted(() => {
  selectedRole.value = t('platformUsers.allRoles');
  const storedLocale = locale.value;
  const localeInfo   = ALL_LOCALES.find(l => l.code === storedLocale);
  if (localeInfo?.dir) {
    document.documentElement.setAttribute('dir', localeInfo.dir);
  }
  fetchData();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css');

/* ══════════════════════════════════════════════
   ROOT & BACKGROUND
══════════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.15; border-radius: 50%;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ══════════════════════════════════════════════
   HEADER
══════════════════════════════════════════════ */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
.text-muted-pro { color: #94a3b8; }

/* ── Lang Cluster ─────────────────────────────────────────────────────── */
.lang-cluster {
  display: flex; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; padding: 4px; gap: 3px;
}
.btn-lang {
  width: 34px; height: 34px; border-radius: 10px; border: none;
  background: transparent; font-size: 1.1rem; cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.btn-lang:hover  { background: #f8fafc; }
.btn-lang.active { background: #0f172a; }

.btn-theme-toggle {
  width: 42px; height: 42px; border-radius: 14px;
  border: 1.5px solid #eef2f6; background: white;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center; font-size: 1rem;
}
.btn-theme-toggle:hover { background: #fffbeb; color: #f59e0b; border-color: #f59e0b; }

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
.btn-view-toggle:hover  { background: #f8fafc; color: #0f172a; }
.btn-view-toggle.active { background: #0f172a; color: #f59e0b; box-shadow: 0 4px 12px rgba(15,23,42,0.2); }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) {
  background: #f8fafc; border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg);
}

/* ══════════════════════════════════════════════
   STATS
══════════════════════════════════════════════ */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6; transition: 0.2s;
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.stat-value  { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label  { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }
.stat-trend  {
  display: flex; align-items: center; gap: 4px;
  font-size: 0.65rem; font-weight: 800; color: #f59e0b;
  background: #fffbeb; padding: 4px 10px; border-radius: 8px;
}

/* ══════════════════════════════════════════════
   ANALYTICS
══════════════════════════════════════════════ */
.analytics-card-pro { background: white; border-radius: 24px; border: 1px solid #eef2f6; }
.bar-chart-v2 { display: flex; align-items: flex-end; gap: 8px; height: 120px; }
.bar-col { display: flex; flex-direction: column; align-items: center; gap: 4px; flex: 1; }
.bar-wrap { display: flex; gap: 3px; align-items: flex-end; height: 100%; width: 100%; justify-content: center; }
.bar-fill { width: 12px; border-radius: 6px 6px 0 0; transition: height 0.8s ease; min-height: 4px; }
.bar-amber  { background: #f59e0b; }
.bar-indigo { background: #6366f1; }
.bar-label  { font-size: 0.6rem; font-weight: 800; color: #94a3b8; }
.legend-dot { width: 8px; height: 8px; border-radius: 50%; display: inline-block; }
.dot-amber  { background: #f59e0b; }
.dot-indigo { background: #6366f1; }
.donut-chart-container { display: flex; align-items: center; gap: 20px; }
.donut-legend { display: flex; flex-direction: column; gap: 8px; flex: 1; }
.donut-legend-item { display: flex; align-items: center; gap: 8px; }
.legend-dot-sm { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.donut-center-text { font-size: 22px; font-weight: 900; fill: #0f172a; }
.donut-sub-text    { font-size: 8px; fill: #94a3b8; font-weight: 700; }

/* ══════════════════════════════════════════════
   TOOLBAR
══════════════════════════════════════════════ */
.tabs-wrap { border: 1px solid #eef2f6 !important; }
.nav-tab-btn-modern {
  padding: 8px 18px; border-radius: 12px; border: none;
  background: transparent; font-weight: 800; font-size: 0.8rem;
  color: #94a3b8; cursor: pointer; transition: 0.2s;
}
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.tab-count {
  background: rgba(255,255,255,0.2); padding: 2px 7px;
  border-radius: 8px; font-size: 0.65rem; margin-left: 6px;
}
.nav-tab-btn-modern:not(.active) .tab-count { background: #f1f5f9; color: #64748b; }

.search-inline-box {
  display: flex; align-items: center; background: white;
  border: 1.5px solid #eef2f6; border-radius: 14px;
  padding: 0 14px; gap: 10px; color: #94a3b8;
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 10px 0; font-weight: 700; font-size: 0.85rem;
  width: 180px; font-family: inherit;
}
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }
.sort-select-pro {
  border: 1.5px solid #eef2f6; border-radius: 14px; padding: 10px 14px;
  font-weight: 700; font-size: 0.8rem; background: white;
  outline: none; cursor: pointer; font-family: inherit;
}

/* ══════════════════════════════════════════════
   GRID — USER CARDS
══════════════════════════════════════════════ */
.user-card-modern {
  background: white; border-radius: 30px; padding: 28px;
  border: 1px solid #eef2f6; transition: 0.3s cubic-bezier(0.4,0,0.2,1);
  cursor: default; height: 100%;
}
.user-card-modern:hover {
  transform: translateY(-8px); border-color: #f59e0b;
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08);
}
.user-title-card { font-size: 1rem; color: #0f172a; }

.avatar-card-lg {
  width: 52px; height: 52px; border-radius: 16px; background: #0f172a;
  color: #fbbf24; font-weight: 900; font-size: 1.2rem;
  display: flex; align-items: center; justify-content: center;
  box-shadow: 0 8px 15px rgba(15,23,42,0.1); flex-shrink: 0; position: relative;
}
.online-dot-card {
  position: absolute; bottom: -2px; right: -2px;
  width: 14px; height: 14px; background: #10b981;
  border: 2.5px solid white; border-radius: 50%;
  animation: pulse-dot 2s infinite;
}
@keyframes pulse-dot {
  0%,100% { transform: scale(1); opacity: 1; }
  50%      { transform: scale(1.2); opacity: 0.7; }
}

.status-badge-user {
  padding: 5px 12px; border-radius: 10px; font-size: 0.65rem;
  font-weight: 800; text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-active   { background: #ecfdf5; color: #10b981; }
.status-inactive { background: #f1f5f9; color: #64748b; }
.status-dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; margin-right: 6px; }

.btn-options-round {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  cursor: pointer; font-size: 0.8rem; color: #94a3b8;
}
.org-badge {
  background: white; border: 1.5px solid #f1f5f9; padding: 5px 12px;
  border-radius: 10px; font-size: 0.7rem; font-weight: 800; color: #64748b;
  box-shadow: 0 2px 6px rgba(0,0,0,0.02);
}

/* ══════════════════════════════════════════════
   TABLE VIEW
══════════════════════════════════════════════ */
.list-header-row { background: #f8fafc; border-radius: 14px; }
.list-col-label  { font-size: 0.6rem; font-weight: 900; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
.list-row-item {
  background: white; border-radius: 16px; border: 1px solid #eef2f6; transition: 0.2s;
}
.list-row-item:hover { border-color: #f59e0b; transform: translateX(3px); }

.avatar-sm-list {
  width: 38px; height: 38px; border-radius: 12px; background: #0f172a;
  color: #fbbf24; font-weight: 900; font-size: 0.9rem;
  display: flex; align-items: center; justify-content: center;
  flex-shrink: 0; position: relative;
}
.online-dot-list {
  position: absolute; bottom: -1px; right: -1px;
  width: 10px; height: 10px; background: #10b981;
  border: 2px solid white; border-radius: 50%;
}
.table-footer-bar { padding: 8px 0; }

/* ══════════════════════════════════════════════
   ROLE CHIPS
══════════════════════════════════════════════ */
.role-chip { padding: 5px 12px; border-radius: 10px; font-size: 0.65rem; font-weight: 900; letter-spacing: 0.5px; }
.role-super { background: #f5f3ff; color: #7c3aed; border: 1px solid #ddd6fe; }
.role-admin { background: #fffbeb; color: #b45309; border: 1px solid #fef3c7; }
.role-eval  { background: #eff6ff; color: #1d4ed8; border: 1px solid #dbeafe; }
.role-cand  { background: #f0fdf4; color: #15803d; border: 1px solid #dcfce7; }

/* ══════════════════════════════════════════════
   SWITCH & ACTIONS
══════════════════════════════════════════════ */
.custom-switch .form-check-input {
  width: 42px; height: 22px; background-color: #e2e8f0; border: none;
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='-4 -4 8 8'%3e%3ccircle r='3' fill='white'/%3e%3c/svg%3e");
}
.custom-switch .form-check-input:checked { background-color: #f59e0b; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px; border: 1.5px solid #eef2f6;
  background: white; color: #64748b; cursor: pointer; transition: 0.2s; font-size: 0.75rem;
  display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm.danger:hover { background: #fff1f2; color: #f43f5e; border-color: #f43f5e; }

/* ══════════════════════════════════════════════
   MISC
══════════════════════════════════════════════ */
.empty-state-pro { background: white; border-radius: 30px; padding: 40px; border: 1px dashed #e2e8f0; }
.shadow-premium  { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }
.pagination-dots .dot { width: 8px; height: 8px; background: #e2e8f0; border-radius: 50%; margin: 0 4px; display: inline-block; }
.pagination-dots .dot.active { background: #f59e0b; width: 25px; border-radius: 10px; }
.spinner-pro-premium {
  width: 50px; height: 50px; border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b; border-radius: 50%;
  animation: spin 1s linear infinite; margin: 40px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }

/* ══════════════════════════════════════════════
   MODALS
══════════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; background: rgba(15,23,42,0.6);
  backdrop-filter: blur(10px); z-index: 2000;
  display: flex; align-items: center; justify-content: center;
}
.confirm-modal {
  background: white; border-radius: 32px; padding: 40px;
  width: 420px; max-width: 95vw; text-align: center;
  box-shadow: 0 40px 80px rgba(0,0,0,0.15);
}
.btn-qv-cancel {
  background: #f1f5f9; color: #64748b; border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
}
.btn-confirm-danger {
  background: #f43f5e; color: white; border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
}
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active  { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn {
  from { opacity: 0; transform: scale(0.92); }
  to   { opacity: 1; transform: scale(1); }
}

/* ══════════════════════════════════════════════
   TOAST
══════════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px; background: #0f172a; color: white;
  padding: 18px 28px; border-radius: 18px; display: flex; align-items: center;
  gap: 14px; z-index: 3000; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn {
  from { transform: translateX(120%); opacity: 0; }
  to   { transform: translateX(0);    opacity: 1; }
}

/* ══════════════════════════════════════════════
   DARK MODE
══════════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root   { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title        { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .text-muted-pro       { color: #8b949e; }
[data-theme="dark"] .stat-card-premium    { background: #161b22; border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .stat-value           { color: #f0f6fc; }
[data-theme="dark"] .analytics-card-pro  { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .analytics-card-pro h6 { color: #f0f6fc; }
[data-theme="dark"] .donut-center-text   { fill: #f0f6fc; }
[data-theme="dark"] .tabs-wrap           { background: #161b22 !important; border-color: rgba(255,255,255,0.08) !important; }
[data-theme="dark"] .nav-tab-btn-modern  { color: #8b949e; }
[data-theme="dark"] .nav-tab-btn-modern.active { background: #f59e0b; color: #0d1117; }
[data-theme="dark"] .search-inline-box,
[data-theme="dark"] .sort-select-pro     { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] .user-card-modern    { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .user-card-modern:hover { border-color: #f59e0b; }
[data-theme="dark"] .user-title-card     { color: #f0f6fc; }
[data-theme="dark"] .list-header-row     { background: #0d1117; }
[data-theme="dark"] .list-row-item       { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .list-row-item:hover { background: rgba(255,255,255,0.03); border-color: #f59e0b; }
[data-theme="dark"] .org-badge           { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .btn-options-round   { background: #0d1117; border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .btn-theme-toggle,
[data-theme="dark"] .btn-refresh-pro,
[data-theme="dark"] .btn-icon-sm         { background: #161b22; border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .lang-cluster        { background: #161b22; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .btn-lang:hover      { background: rgba(255,255,255,0.05); }
[data-theme="dark"] .btn-lang.active     { background: #f59e0b; }
[data-theme="dark"] .view-toggle-cluster { background: #161b22; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .btn-view-toggle:hover  { background: rgba(255,255,255,0.05); color: #f0f6fc; }
[data-theme="dark"] .btn-view-toggle.active { background: #f59e0b; color: #0d1117; }
[data-theme="dark"] .dropdown-menu       { background: #1e2a3a !important; border: 1px solid rgba(255,255,255,0.08) !important; }
[data-theme="dark"] .dropdown-item       { color: #f0f6fc; }
[data-theme="dark"] .dropdown-item:hover { background: rgba(255,255,255,0.08); }
[data-theme="dark"] .dropdown-divider    { border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .confirm-modal       { background: #161b22; color: #f0f6fc; }
[data-theme="dark"] .btn-qv-cancel       { background: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .custom-switch .form-check-input         { background-color: #475569; }
[data-theme="dark"] .custom-switch .form-check-input:checked { background-color: #f59e0b; }

/* ══════════════════════════════════════════════
   RTL SUPPORT
══════════════════════════════════════════════ */
[dir="rtl"] .enigma-toast          { right: auto; left: 30px; border-left: none; border-right: 5px solid #f59e0b; }
[dir="rtl"] .t-success             { border-right-color: #10b981; }
[dir="rtl"] .t-error               { border-right-color: #f43f5e; }
[dir="rtl"] .list-row-item:hover   { transform: translateX(-3px); }
[dir="rtl"] .breadcrumb-pro .separator { transform: scaleX(-1); display: inline-block; }
[dir="rtl"] .stat-details          { margin-right: 12px; margin-left: 0; }
[dir="rtl"] .online-dot-card       { right: auto; left: -2px; }
[dir="rtl"] .online-dot-list       { right: auto; left: -1px; }
</style>