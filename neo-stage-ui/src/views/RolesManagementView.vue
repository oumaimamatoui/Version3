<template>
  <div class="elite-roles-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="hero-bg-grid"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column">

      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">

        <div class="p-4 p-lg-5 animate__animated animate__fadeIn">

          <!-- ══════════════════════════════════════
               SECTION 1 — EN-TÊTE
          ══════════════════════════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">{{ t('rolesView.breadcrumb.admin') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ t('rolesView.breadcrumb.security') }}</span>
              </div>
              <h2 class="premium-title">
                {{ t('rolesView.header.title') }}
                <span class="gradient-text">{{ t('rolesView.header.titleHighlight') }}</span>
              </h2>
              <p class="subtitle">{{ t('rolesView.header.subtitle') }}</p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <button class="btn-refresh-pro" @click="fetchData" :disabled="loading" :title="t('rolesView.actions.sync')">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <button @click="openAddModal" class="btn-enigma-primary shadow-premium">
                <div class="btn-content">
                  <i class="fa-solid fa-shield-plus me-2"></i>{{ t('rolesView.actions.deployRole') }}
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- ══════════════════════════════════════
               SECTION 2 — KPI STATS
          ══════════════════════════════════════ -->
          <div class="row g-4 mb-5">
            <div class="col-md-4" v-for="stat in computedStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label.toUpperCase() }}</div>
                </div>
                <div v-if="stat.trend" class="stat-trend ms-auto" :class="stat.trend > 0 ? 'trend-up' : 'trend-down'">
                  <i :class="stat.trend > 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ Math.abs(stat.trend) }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ══════════════════════════════════════
               SECTION 3 — RECHERCHE
          ══════════════════════════════════════ -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="search-inline-box flex-grow-1" style="max-width:480px">
              <i class="fa-solid fa-magnifying-glass"></i>
              <input
                type="text"
                v-model="searchQuery"
                class="search-inline-input"
                :placeholder="t('rolesView.search.placeholder')"
              />
              <transition name="fade">
                <span v-if="searchQuery" class="search-badge">{{ filteredRoles.length }}</span>
              </transition>
              <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>
            <div class="tabs-wrapper d-flex gap-2 p-1 rounded-4">
              <button
                v-for="tab in filterTabs" :key="tab.value"
                class="nav-tab-btn-modern"
                :class="{ active: activeTab === tab.value }"
                @click="activeTab = tab.value"
              >
                {{ tab.label }} <span class="tab-count">{{ tab.count }}</span>
              </button>
            </div>
          </div>

          <!-- ══════════════════════════════════════
               SECTION 4 — LOADER
          ══════════════════════════════════════ -->
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="loading-text mt-3">{{ t('rolesView.loading.credentials') }}</p>
          </div>

          <!-- ══════════════════════════════════════
               SECTION 5 — GRILLE DES RÔLES
          ══════════════════════════════════════ -->
          <div v-else class="row g-4">

            <div v-if="filteredRoles.length === 0" class="col-12">
              <div class="empty-state-pro py-5 text-center">
                <i class="fa-solid fa-shield-halved fa-3x mb-3 empty-icon"></i>
                <h5 class="fw-800">{{ t('rolesView.empty.title') }}</h5>
                <p class="empty-sub">{{ t('rolesView.empty.subtitle') }}</p>
                <button class="btn-enigma-primary mt-3" @click="openAddModal">
                  <div class="btn-content">
                    <i class="fa-solid fa-plus me-2"></i>{{ t('rolesView.empty.cta') }}
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>
            </div>

            <div
              v-else
              class="col-xl-4 col-md-6 animate__animated animate__fadeInUp"
              v-for="(role, idx) in filteredRoles"
              :key="role.id"
              :style="{ animationDelay: idx * 0.06 + 's' }"
            >
              <div class="role-card-modern">

                <!-- Header -->
                <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                  <div class="role-badge-wrapper">
                    <span class="access-level-badge" :class="getAccessClass(role)">
                      <span class="status-dot"></span>
                      {{ getAccessLabel(role) }}
                    </span>
                  </div>
                  <div class="dropdown">
                    <button class="btn-options-round" data-bs-toggle="dropdown">
                      <i class="fa-solid fa-ellipsis-vertical"></i>
                    </button>
                    <ul class="dropdown-menu border-0 shadow-premium p-2 rounded-4 dropdown-menu-themed">
                      <li>
                        <button class="dropdown-item rounded-3" @click="openEditModal(role)">
                          <i class="fa-solid fa-pen-to-square me-2 text-amber"></i>{{ t('rolesView.dropdown.edit') }}
                        </button>
                      </li>
                      <li><hr class="dropdown-divider"></li>
                      <li>
                        <button class="dropdown-item rounded-3 text-danger" @click="confirmDelete(role.id)">
                          <i class="fa-solid fa-trash-can me-2"></i>{{ t('rolesView.dropdown.revoke') }}
                        </button>
                      </li>
                    </ul>
                  </div>
                </div>

                <!-- Icône + Nom -->
                <div class="d-flex align-items-center gap-3 mb-3">
                  <div class="role-squircle">
                    <i class="fa-solid fa-fingerprint"></i>
                  </div>
                  <div class="min-width-0">
                    <h5 class="role-name mb-1">{{ role.nom }}</h5>
                    <p class="role-desc m-0 text-truncate">{{ role.description || t('rolesView.card.defaultDesc') }}</p>
                  </div>
                </div>

                <!-- Barre permissions -->
                <div class="perm-progress-box mb-3">
                  <div class="d-flex justify-content-between mb-1">
                    <span class="micro-label">{{ t('rolesView.card.rightsCoverage') }}</span>
                    <span class="micro-label text-amber">{{ role.permissions?.length || 0 }} / {{ totalPerms }}</span>
                  </div>
                  <div class="progress-slim">
                    <div
                      class="progress-fill"
                      :style="{
                        width: Math.round(((role.permissions?.length || 0) / totalPerms) * 100) + '%',
                        background: getProgressColor(Math.round(((role.permissions?.length || 0) / totalPerms) * 100))
                      }"
                    ></div>
                  </div>
                </div>

                <!-- Permissions tags -->
                <div class="perm-tags mb-4">
                  <span
                    v-for="p in role.permissions?.slice(0, 3)"
                    :key="p"
                    class="perm-pill"
                  >
                    {{ p.replace('_', ' ').toUpperCase() }}
                  </span>
                  <span v-if="role.permissions?.length > 3" class="perm-pill more">
                    +{{ role.permissions.length - 3 }}
                  </span>
                  <span v-if="!role.permissions?.length" class="perm-pill empty">
                    {{ t('rolesView.card.noPermission') }}
                  </span>
                </div>

                <!-- Footer -->
                <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3">
                  <span class="members-chip">
                    <i class="fa-solid fa-user-shield me-2"></i>{{ role.nombreMembres || 0 }} {{ t('rolesView.card.members') }}
                  </span>
                  <button @click="openEditModal(role)" class="btn-edit-link">
                    {{ t('rolesView.card.configure') }} <i class="fa-solid fa-arrow-right-long ms-1"></i>
                  </button>
                </div>

              </div>
            </div>

          </div>
        </div>
      </main>
    </div>

    <!-- ══════════════════════════════════════
         MODALE — DÉPLOYER / MODIFIER UN RÔLE
    ══════════════════════════════════════ -->
    <Transition name="modal-quantum">
      <div v-if="showModal" class="quantum-vault-overlay" @click.self="closeModal">
        <div class="role-modal-window animate__animated animate__zoomIn animate__faster">

          <!-- Tête -->
          <header class="modal-head-v2">
            <div class="d-flex align-items-center gap-4">
              <div class="modal-brand-icon">
                <i class="fa-solid fa-shield-halved"></i>
              </div>
              <div>
                <h3 class="modal-title-v2">
                  {{ isEditing ? t('rolesView.modal.titleEdit') : t('rolesView.modal.titleCreate') }}
                </h3>
                <p class="modal-sub-v2">{{ t('rolesView.modal.protocol') }}</p>
              </div>
            </div>
            <button @click="closeModal" class="btn-close-modal">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </header>
          <div class="header-accent-line"></div>

          <!-- Corps -->
          <div class="modal-body-scroll custom-scrollbar">

            <!-- 01 Paramètres -->
            <div class="form-section-card mb-4">
              <div class="section-badge mb-4">
                <span>01</span> {{ t('rolesView.modal.section1') }}
              </div>
              <div class="row g-4">
                <div class="col-md-6">
                  <div class="enigma-input-wrap">
                    <label>{{ t('rolesView.modal.roleName') }}</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-id-badge"></i>
                      <input
                        v-model="form.nom"
                        type="text"
                        class="enigma-field"
                        :placeholder="t('rolesView.modal.roleNamePlaceholder')"
                      />
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="enigma-input-wrap">
                    <label>{{ t('rolesView.modal.roleTemplate') }}</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-layer-group"></i>
                      <select v-model="selectedTemplate" @change="applyTemplate" class="enigma-field" :disabled="isEditing">
                        <option value="">{{ t('rolesView.modal.templateCustom') }}</option>
                        <option value="manager">{{ t('rolesView.modal.templateManager') }}</option>
                        <option value="hr">{{ t('rolesView.modal.templateHr') }}</option>
                        <option value="evaluator">{{ t('rolesView.modal.templateEvaluator') }}</option>
                        <option value="viewer">{{ t('rolesView.modal.templateViewer') }}</option>
                      </select>
                    </div>
                  </div>
                </div>
                <div class="col-12" v-if="!isEditing">
                  <div class="enigma-input-wrap">
                    <label>{{ t('rolesView.modal.inviteLabel') }}</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-envelope-open-text"></i>
                      <input
                        v-model="form.email"
                        type="email"
                        class="enigma-field"
                        :placeholder="t('rolesView.modal.invitePlaceholder')"
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- 02 Matrice droits -->
            <div class="form-section-card">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="section-badge m-0">
                  <span>02</span> {{ t('rolesView.modal.section2') }}
                </div>
                <div class="perm-count-pill">
                  <i class="fa-solid fa-lock-open me-1"></i>
                  {{ form.permissions.length }} / {{ totalPerms }} {{ t('rolesView.modal.active') }}
                </div>
              </div>

              <div v-for="group in localizedPermissionGroups" :key="group.title" class="perm-group mb-3">
                <div class="group-label">
                  <span class="group-dot"></span>{{ group.title }}
                </div>
                <div class="perm-grid">
                  <div
                    v-for="item in group.items"
                    :key="item.id"
                    @click="togglePermission(item.id)"
                    class="perm-node"
                    :class="{ active: form.permissions.includes(item.id) }"
                  >
                    <div class="node-checkbox">
                      <i v-if="form.permissions.includes(item.id)" class="fa-solid fa-check"></i>
                    </div>
                    <div>
                      <span class="node-name">{{ item.label }}</span>
                      <span class="node-desc">{{ item.desc }}</span>
                    </div>
                  </div>
                </div>
              </div>

            </div>

          </div>

          <!-- Pied -->
          <footer class="modal-foot-v2">
            <button @click="closeModal" class="btn-qv-cancel">{{ t('rolesView.modal.cancel') }}</button>
            <button @click="saveRole" class="btn-enigma-primary px-5" :disabled="saving">
              <div class="btn-content" v-if="!saving">
                <i class="fa-solid fa-unlock-keyhole me-2"></i>
                {{ isEditing ? t('rolesView.modal.update') : t('rolesView.modal.deploy') }}
              </div>
              <div v-else class="btn-content">
                <span class="spinner-border spinner-border-sm me-2"></span>{{ t('rolesView.modal.deploying') }}
              </div>
              <div class="btn-glow"></div>
            </button>
          </footer>

        </div>
      </div>
    </Transition>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>{{ t('rolesView.toast.systemMessage') }}</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

/* ─── i18n ─────────────────────────────────────────────────── */
const { t } = useI18n();

/* ─── STATE ────────────────────────────────────────────────── */
const roles         = ref([]);
const rawStats      = ref([]);
const loading       = ref(true);
const saving        = ref(false);
const showModal     = ref(false);
const isEditing     = ref(false);
const searchQuery   = ref('');
const activeTab     = ref('all');
const selectedTemplate = ref('');
const mousePos      = reactive({ x: 0, y: 0 });
const globalToast   = reactive({ active: false, message: '', type: '', icon: '' });
let _toastTimer     = null;

const form = reactive({ id: null, nom: '', email: '', description: '', permissions: [] });

/* ─── PERMISSIONS (IDs only — labels come from i18n) ──────── */
const permissionDefs = [
  {
    groupKey: 'candidates',
    items: [
      { id: 'view_can',    labelKey: 'view_can',    descKey: 'view_can'    },
      { id: 'inv_can',     labelKey: 'inv_can',     descKey: 'inv_can'     },
    ]
  },
  {
    groupKey: 'evaluations',
    items: [
      { id: 'view_tests',  labelKey: 'view_tests',  descKey: 'view_tests'  },
      { id: 'grade_tests', labelKey: 'grade_tests', descKey: 'grade_tests' },
      { id: 'edit_bank',   labelKey: 'edit_bank',   descKey: 'edit_bank'   },
    ]
  },
  {
    groupKey: 'admin',
    items: [
      { id: 'add_rol',     labelKey: 'add_rol',     descKey: 'add_rol'     },
      { id: 'add_staff',   labelKey: 'add_staff',   descKey: 'add_staff'   },
    ]
  }
];

/* Build localized permission groups reactively */
const localizedPermissionGroups = computed(() =>
  permissionDefs.map(group => ({
    title: t(`rolesView.permGroups.${group.groupKey}`),
    items: group.items.map(item => ({
      id:    item.id,
      label: t(`rolesView.perms.${item.labelKey}.label`),
      desc:  t(`rolesView.perms.${item.descKey}.desc`),
    }))
  }))
);

const totalPerms = computed(() =>
  permissionDefs.reduce((a, g) => a + g.items.length, 0)
);

/* ─── COMPUTED STATS (i18n labels) ────────────────────────── */
const computedStats = computed(() => [
  {
    label: t('rolesView.stats.activeRoles'),
    value: roles.value.length,
    icon: 'fa-solid fa-shield-halved',
    color: '#eab308', bg: 'rgba(234,179,8,0.15)', trend: 5
  },
  {
    label: t('rolesView.stats.managedMembers'),
    value: roles.value.reduce((a, r) => a + (r.nombreMembres || 0), 0),
    icon: 'fa-solid fa-users',
    color: '#6366f1', bg: 'rgba(99,102,241,0.15)', trend: 8
  },
  {
    label: t('rolesView.stats.permissions'),
    value: totalPerms.value,
    icon: 'fa-solid fa-lock',
    color: '#10b981', bg: 'rgba(16,185,129,0.15)', trend: 0
  },
]);

/* ─── TABS (i18n labels) ───────────────────────────────────── */
const filterTabs = computed(() => [
  {
    label: t('rolesView.tabs.all'),
    value: 'all',
    count: filteredBySearch.value.length
  },
  {
    label: t('rolesView.tabs.full'),
    value: 'full',
    count: filteredBySearch.value.filter(r => (r.permissions?.length || 0) >= totalPerms.value).length
  },
  {
    label: t('rolesView.tabs.limited'),
    value: 'limited',
    count: filteredBySearch.value.filter(r => (r.permissions?.length || 0) < totalPerms.value).length
  },
]);

const filteredBySearch = computed(() =>
  roles.value.filter(r =>
    r.nom !== 'SuperAdmin' &&
    r.nom.toLowerCase().includes(searchQuery.value.toLowerCase())
  )
);

const filteredRoles = computed(() => {
  let list = filteredBySearch.value;
  if (activeTab.value === 'full')    list = list.filter(r => (r.permissions?.length || 0) >= totalPerms.value);
  if (activeTab.value === 'limited') list = list.filter(r => (r.permissions?.length || 0) < totalPerms.value);
  return list;
});

/* ─── HELPERS ──────────────────────────────────────────────── */
const getProgressColor = (p) => p >= 80 ? '#10b981' : p >= 40 ? '#eab308' : '#6366f1';

const getAccessLabel = (role) => {
  const n = role.permissions?.length || 0;
  if (n >= totalPerms.value) return t('rolesView.accessLevel.full');
  if (n >= 4)                return t('rolesView.accessLevel.high');
  if (n >= 1)                return t('rolesView.accessLevel.low');
  return t('rolesView.accessLevel.none');
};

const getAccessClass = (role) => {
  const n = role.permissions?.length || 0;
  if (n >= totalPerms.value) return 'access-full';
  if (n >= 4)                return 'access-high';
  if (n >= 1)                return 'access-low';
  return 'access-none';
};

/* ─── API ──────────────────────────────────────────────────── */
const fetchData = async () => {
  loading.value = true;
  try {
    const [rolesRes] = await Promise.all([
      api.get('/Roles'),
    ]);
    roles.value = rolesRes.data;
  } catch (err) {
    console.error(err);
    showPulseToast(t('rolesView.toast.loadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    loading.value = false;
  }
};

const togglePermission = (id) => {
  const i = form.permissions.indexOf(id);
  i > -1 ? form.permissions.splice(i, 1) : form.permissions.push(id);
};

const applyTemplate = () => {
  if (!selectedTemplate.value) { form.permissions = []; return; }
  const mapping = {
    manager:   ['view_can','inv_can','view_tests','grade_tests','edit_bank','add_rol','add_staff'],
    hr:        ['view_can','inv_can','add_staff'],
    evaluator: ['view_tests','grade_tests','edit_bank'],
    viewer:    ['view_can','view_tests']
  };
  form.permissions = [...(mapping[selectedTemplate.value] || [])];
};

const saveRole = async () => {
  if (!form.nom) return showPulseToast(t('rolesView.toast.nameRequired'), 'warn', 'fa-solid fa-triangle-exclamation');
  saving.value = true;

  const templateMap = {
    manager: 'AdminEntreprise',
    hr: 'Recruteur',
    evaluator: 'Evaluateur',
    viewer: 'Lecteur',
  };
  const payload = {
    ...form,
    modeleRole: templateMap[selectedTemplate.value] || 'Personnalise'
  };

  try {
    if (isEditing.value) await api.put(`/Roles/${form.id}`, payload);
    else                 await api.post('/Roles', payload);
    showPulseToast(
      isEditing.value ? t('rolesView.toast.updated') : t('rolesView.toast.deployed'),
      'success', 'fa-solid fa-shield-check'
    );
    await fetchData();
    closeModal();
  } catch (e) {
    showPulseToast(
      t('rolesView.toast.saveError') + (e.response?.data?.message || t('rolesView.toast.serverError')),
      'error', 'fa-solid fa-circle-xmark'
    );
  } finally { saving.value = false; }
};

const confirmDelete = async (id) => {
  if (confirm(t('rolesView.confirm.revoke'))) {
    try {
      await api.delete(`/Roles/${id}`);
      showPulseToast(t('rolesView.toast.revoked'), 'warn', 'fa-solid fa-trash-can');
      fetchData();
    } catch {
      showPulseToast(t('rolesView.toast.roleInUse'), 'error', 'fa-solid fa-circle-xmark');
    }
  }
};

/* ─── MODAL ────────────────────────────────────────────────── */
const openAddModal  = () => { isEditing.value = false; resetForm(); showModal.value = true; };
const openEditModal = (r) => { isEditing.value = true; Object.assign(form, JSON.parse(JSON.stringify(r))); showModal.value = true; };
const closeModal    = () => { showModal.value = false; };
const resetForm     = () => {
  Object.assign(form, { id: null, nom: '', email: '', description: '', permissions: [] });
  selectedTemplate.value = '';
};

/* ─── TOAST ────────────────────────────────────────────────── */
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

/* ─── PARALLAX ─────────────────────────────────────────────── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(fetchData);
</script>

<!-- ═══════════════════════════════════════════════════════
     STYLE GLOBAL (non-scoped) — variables CSS light/dark
═══════════════════════════════════════════════════════ -->
<style>
/* ── LIGHT (défaut) ───────────────────────────────────── */
.elite-roles-root {
  --roles-bg-page:            #f8fafc;
  --roles-bg-card:            #ffffff;
  --roles-bg-input:           #f8fafc;
  --roles-bg-section:         #f8fafc;
  --roles-bg-modal:           #ffffff;
  --roles-bg-modal-head:      #fafafa;
  --roles-bg-modal-foot:      #fafafa;
  --roles-bg-perm-group:      #f8fafc;
  --roles-bg-perm-node:       #ffffff;
  --roles-bg-perm-node-active:#fffbeb;
  --roles-bg-tabs:            #ffffff;
  --roles-bg-progress:        #e2e8f0;
  --roles-bg-members:         #f8fafc;

  --roles-border-color:       #eef2f6;
  --roles-border-card:        #eef2f6;
  --roles-border-input:       #f1f5f9;
  --roles-border-modal:       rgba(255,255,255,0.9);
  --roles-border-perm:        #f1f5f9;
  --roles-border-tab:         #e2e8f0;
  --roles-border-footer:      #f1f5f9;

  --roles-text-primary:       #0f172a;
  --roles-text-secondary:     #64748b;
  --roles-text-muted:         #94a3b8;
  --roles-text-heading:       #0f172a;

  --roles-amber:              #eab308;
  --roles-amber-light:        #fbbf24;
  --roles-amber-bg:           #fefce8;
  --roles-amber-border:       #fde68a;

  --roles-dropdown-bg:        #ffffff;
  --roles-dropdown-item:      #0f172a;
  --roles-dropdown-hover:     #f8fafc;
  --roles-divider:            #f1f5f9;

  --roles-shadow-card:        0 2px 8px rgba(0,0,0,0.04);
  --roles-shadow-card-hover:  0 25px 50px -12px rgba(234,179,8,0.14), 0 8px 24px rgba(0,0,0,0.06);
  --roles-shadow-premium:     0 20px 60px rgba(0,0,0,0.10);

  --roles-orb-amber-opacity:  0.12;
  --roles-orb-blue-opacity:   0.10;
  --roles-grid-opacity:       0.18;

  --roles-btn-cancel-bg:      #f1f5f9;
  --roles-btn-cancel-color:   #64748b;
  --roles-btn-cancel-hover:   #e2e8f0;

  --roles-empty-icon-color:   #94a3b8;
}

/* ── DARK ─────────────────────────────────────────────── */
[data-theme="dark"] .elite-roles-root {
  --roles-bg-page:            #0d1117;
  --roles-bg-card:            #161b22;
  --roles-bg-input:           rgba(255,255,255,0.05);
  --roles-bg-section:         #0d1117;
  --roles-bg-modal:           #0d1117;
  --roles-bg-modal-head:      #161b22;
  --roles-bg-modal-foot:      #161b22;
  --roles-bg-perm-group:      rgba(255,255,255,0.03);
  --roles-bg-perm-node:       rgba(255,255,255,0.03);
  --roles-bg-perm-node-active:rgba(234,179,8,0.10);
  --roles-bg-tabs:            rgba(255,255,255,0.04);
  --roles-bg-progress:        rgba(255,255,255,0.08);
  --roles-bg-members:         rgba(255,255,255,0.05);

  --roles-border-color:       rgba(255,255,255,0.06);
  --roles-border-card:        rgba(255,255,255,0.07);
  --roles-border-input:       rgba(255,255,255,0.10);
  --roles-border-modal:       rgba(255,255,255,0.07);
  --roles-border-perm:        rgba(255,255,255,0.07);
  --roles-border-tab:         rgba(255,255,255,0.06);
  --roles-border-footer:      rgba(255,255,255,0.06);

  --roles-text-primary:       #f0f6fc;
  --roles-text-secondary:     #8b949e;
  --roles-text-muted:         #6e7681;
  --roles-text-heading:       #f0f6fc;

  --roles-dropdown-bg:        #1c2330;
  --roles-dropdown-item:      #c9d1d9;
  --roles-dropdown-hover:     rgba(255,255,255,0.06);
  --roles-divider:            rgba(255,255,255,0.08);

  --roles-shadow-card:        0 2px 12px rgba(0,0,0,0.25);
  --roles-shadow-card-hover:  0 25px 50px -12px rgba(234,179,8,0.18), 0 8px 24px rgba(0,0,0,0.3);
  --roles-shadow-premium:     0 20px 60px rgba(0,0,0,0.4);

  --roles-orb-amber-opacity:  0.15;
  --roles-orb-blue-opacity:   0.12;
  --roles-grid-opacity:       0.06;

  --roles-btn-cancel-bg:      rgba(255,255,255,0.06);
  --roles-btn-cancel-color:   #8b949e;
  --roles-btn-cancel-hover:   rgba(255,255,255,0.10);

  --roles-empty-icon-color:   #6e7681;
}
</style>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&display=swap');

/* ══════════════════════════════════
   BASE
══════════════════════════════════ */
.elite-roles-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: var(--roles-bg-page);
  color: var(--roles-text-primary);
  display: flex;
  position: relative;
  overflow-x: hidden;
}

/* ══════════════════════════════════
   BACKGROUND
══════════════════════════════════ */
.cyber-engine-bg {
  position: fixed; inset: 0; z-index: 0; pointer-events: none;
}
.hero-bg-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(234,179,8,0.05) 1px, transparent 1px),
    linear-gradient(90deg, rgba(234,179,8,0.05) 1px, transparent 1px);
  background-size: 60px 60px;
  opacity: var(--roles-grid-opacity);
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #eab308; top: -200px; right: -100px; opacity: var(--roles-orb-amber-opacity); }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; opacity: var(--roles-orb-blue-opacity); }

/* ══════════════════════════════════
   LAYOUT
══════════════════════════════════ */
.main-orchestrator { z-index: 5; }
.canvas-engine {
  height: calc(100vh - 64px);
  background: var(--roles-bg-page);
  transition: background 0.3s;
}

/* ══════════════════════════════════
   HEADER + BREADCRUMB
══════════════════════════════════ */
.premium-title {
  font-weight: 900; font-size: 2.2rem; letter-spacing: -1.5px; margin: 0;
  color: var(--roles-text-heading);
}
.gradient-text {
  background: linear-gradient(135deg, #eab308 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle { color: var(--roles-text-secondary); font-size: 14px; margin-top: 6px; margin-bottom: 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: var(--roles-text-muted); display: flex; align-items: center; }
.breadcrumb-pro .root { cursor: pointer; transition: color 0.2s; }
.breadcrumb-pro .root:hover { color: var(--roles-amber); }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: var(--roles-text-primary); font-weight: 800; }

/* ══════════════════════════════════
   BOUTONS
══════════════════════════════════ */
.btn-enigma-primary {
  background: var(--roles-text-heading); color: #f0f6fc; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  font-size: 13px; position: relative; overflow: hidden;
  cursor: pointer; font-family: inherit; transition: transform 0.2s;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #eab308, #fbbf24);
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2; display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }
[data-theme="dark"] .elite-roles-root .btn-enigma-primary,
[data-theme="dark"] .elite-roles-root .nav-tab-btn-modern.active { background: #0d1117; }
.shadow-premium { box-shadow: var(--roles-shadow-premium) !important; }

.btn-refresh-pro {
  width: 44px; height: 44px;
  background: var(--roles-bg-card);
  border: 1.5px solid var(--roles-border-card); border-radius: 14px;
  color: var(--roles-text-muted); cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) {
  background: var(--roles-bg-section); border-color: var(--roles-amber); color: var(--roles-amber);
  transform: rotate(180deg) scale(1.1);
}

/* ══════════════════════════════════
   KPI STATS — style home.vue
══════════════════════════════════ */
.stat-card-premium {
  background: var(--roles-bg-card);
  border-radius: 24px; padding: 24px;
  display: flex; align-items: center;
  border: 1px solid var(--roles-border-card);
  box-shadow: var(--roles-shadow-card);
  transition: 0.3s cubic-bezier(0.4,0,0.2,1);
  position: relative; overflow: hidden;
}
/* subtle accent line top, like home.vue's featured cards */
.stat-card-premium::before {
  content: ''; position: absolute; top: 0; left: 0; right: 0; height: 3px;
  background: linear-gradient(90deg, var(--roles-amber), transparent);
  opacity: 0; transition: opacity 0.3s;
}
.stat-card-premium:hover::before { opacity: 1; }
.stat-card-premium:hover {
  transform: translateY(-6px);
  box-shadow: var(--roles-shadow-card-hover);
  border-color: var(--roles-amber-border);
}
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.stat-details  { margin-left: 16px; }
.stat-value    { font-size: 1.7rem; font-weight: 900; line-height: 1; color: var(--roles-text-primary); }
.stat-label    { font-size: 0.65rem; font-weight: 800; color: var(--roles-text-muted); text-transform: uppercase; letter-spacing: 1px; margin-top: 4px; }
.stat-trend    { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up      { color: #10b981; }
.trend-down    { color: #f43f5e; }

/* ══════════════════════════════════
   RECHERCHE & TABS
══════════════════════════════════ */
.search-inline-box {
  display: flex; align-items: center;
  background: var(--roles-bg-card);
  border: 1.5px solid var(--roles-border-card); border-radius: 14px;
  padding: 0 14px; gap: 10px; color: var(--roles-text-muted);
  transition: border-color 0.2s, box-shadow 0.2s;
}
.search-inline-box:focus-within {
  border-color: var(--roles-amber);
  box-shadow: 0 0 0 4px rgba(234,179,8,0.12);
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 12px 0; font-weight: 700; font-size: 0.85rem;
  flex: 1; color: var(--roles-text-primary); font-family: inherit;
}
.search-inline-input::placeholder { color: var(--roles-text-muted); }
.search-badge {
  background: var(--roles-amber); color: #0f172a; font-size: 11px;
  font-weight: 900; padding: 3px 10px; border-radius: 99px;
}
.btn-clear-search {
  border: none; background: transparent; color: var(--roles-text-muted);
  padding: 0; cursor: pointer; font-size: 13px;
}

.tabs-wrapper {
  background: var(--roles-bg-tabs);
  border: 1px solid var(--roles-border-tab);
  box-shadow: var(--roles-shadow-card);
}
.nav-tab-btn-modern {
  padding: 8px 16px; border-radius: 12px; border: none;
  background: transparent; font-weight: 800; font-size: 0.78rem;
  color: var(--roles-text-muted); cursor: pointer; transition: 0.2s;
  font-family: inherit;
}
.nav-tab-btn-modern.active { background: var(--roles-text-heading); color: #f0f6fc; }
.tab-count {
  background: rgba(255,255,255,0.2); padding: 2px 7px;
  border-radius: 8px; font-size: 0.65rem; margin-left: 6px;
}
.nav-tab-btn-modern:not(.active) .tab-count {
  background: var(--roles-border-card); color: var(--roles-text-secondary);
}

/* ══════════════════════════════════
   LOADER
══════════════════════════════════ */
.spinner-pro-premium {
  width: 50px; height: 50px; border: 4px solid var(--roles-border-card);
  border-top: 4px solid var(--roles-amber); border-radius: 50%;
  animation: spin 1s linear infinite; margin: 40px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }
.loading-text { font-size: 11px; font-weight: 800; color: var(--roles-text-muted); letter-spacing: 2px; }

/* ══════════════════════════════════
   CARTES RÔLES — améliorées comme home.vue
══════════════════════════════════ */
.role-card-modern {
  background: var(--roles-bg-card); border-radius: 30px; padding: 28px;
  border: 1px solid var(--roles-border-card); height: 100%;
  transition: 0.35s cubic-bezier(0.4,0,0.2,1); cursor: default;
  box-shadow: var(--roles-shadow-card);
  position: relative; overflow: hidden;
}
/* gold top accent like home.vue service-card-large featured */
.role-card-modern::after {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(135deg, var(--roles-amber-bg) 0%, transparent 60%);
  opacity: 0; transition: opacity 0.35s; z-index: 0; pointer-events: none;
}
.role-card-modern:hover::after { opacity: 1; }
.role-card-modern > * { position: relative; z-index: 1; }
.role-card-modern:hover {
  transform: translateY(-8px);
  border-color: var(--roles-amber);
  box-shadow: var(--roles-shadow-card-hover);
}

.access-level-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 0.65rem; font-weight: 800;
  display: inline-flex; align-items: center; gap: 6px;
}
.access-full  { background: rgba(16,185,129,0.15); color: #10b981; }
.access-high  { background: rgba(234,179,8,0.15);  color: #d97706; }
.access-low   { background: rgba(99,102,241,0.15); color: #818cf8; }
.access-none  { background: var(--roles-bg-section); color: var(--roles-text-muted); }
.status-dot   { width: 6px; height: 6px; border-radius: 50%; background: currentColor; animation: pulse-dot 2s ease-in-out infinite; }
@keyframes pulse-dot { 0%,100%{transform:scale(1);opacity:1} 50%{transform:scale(1.5);opacity:0.7} }

.btn-options-round {
  width: 34px; height: 34px; border-radius: 10px;
  border: 1.5px solid var(--roles-border-card); background: var(--roles-bg-card);
  color: var(--roles-text-muted); cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.btn-options-round:hover {
  background: var(--roles-text-heading); color: var(--roles-amber);
  border-color: var(--roles-text-heading);
}

.role-squircle {
  width: 52px; height: 52px;
  background: var(--roles-text-heading); color: var(--roles-amber);
  border-radius: 17px; display: flex; align-items: center;
  justify-content: center; font-size: 1.2rem; flex-shrink: 0;
  transition: 0.3s;
}
.role-card-modern:hover .role-squircle {
  background: var(--roles-amber); color: #0f172a;
  box-shadow: 0 8px 20px rgba(234,179,8,0.3);
}
.role-name { font-size: 1.1rem; font-weight: 900; color: var(--roles-text-primary); letter-spacing: -0.3px; }
.role-desc { color: var(--roles-text-secondary); font-size: 13px; max-width: 180px; }
.min-width-0 { min-width: 0; }

/* Progress */
.perm-progress-box {
  background: var(--roles-bg-section); border-radius: 14px; padding: 14px 16px;
  border: 1px solid var(--roles-border-card);
}
.progress-slim  { height: 5px; background: var(--roles-bg-progress); border-radius: 10px; overflow: hidden; }
.progress-fill  { height: 100%; border-radius: 10px; transition: width 0.6s ease; }
.micro-label    { font-size: 9px; font-weight: 800; color: var(--roles-text-muted); letter-spacing: 1.5px; text-transform: uppercase; }
.micro-label.text-amber { color: var(--roles-amber) !important; }

/* Tags */
.perm-tags  { display: flex; flex-wrap: wrap; gap: 6px; }
.perm-pill  {
  font-size: 9px; font-weight: 800; padding: 5px 12px;
  background: var(--roles-bg-section); border: 1px solid var(--roles-border-perm);
  border-radius: 99px; color: var(--roles-text-secondary); white-space: nowrap;
  transition: 0.2s;
}
.perm-pill:hover { border-color: var(--roles-amber); color: var(--roles-amber); }
.perm-pill.more  { background: rgba(234,179,8,0.10); border-color: rgba(234,179,8,0.25); color: #d97706; }
.perm-pill.empty { background: var(--roles-bg-section); border-color: var(--roles-border-perm); color: var(--roles-text-muted); font-style: italic; }

/* Footer carte */
.card-footer-modern {
  border-top: 1px solid var(--roles-border-footer);
  display: flex; justify-content: space-between; align-items: center;
}
.members-chip {
  font-size: 12px; font-weight: 600; color: var(--roles-text-secondary);
  background: var(--roles-bg-members); padding: 6px 14px;
  border-radius: 99px; border: 1px solid var(--roles-border-card);
}
.btn-edit-link {
  background: none; border: none; font-size: 10px; font-weight: 900;
  color: var(--roles-amber); cursor: pointer; letter-spacing: 0.8px;
  transition: color 0.2s; padding: 0;
}
.btn-edit-link:hover { color: #d97706; }

/* Empty state */
.empty-state-pro {
  background: var(--roles-bg-card); border-radius: 30px;
  padding: 40px; border: 1px dashed var(--roles-border-card);
}
.empty-icon { color: var(--roles-empty-icon-color); }
.empty-sub  { color: var(--roles-text-secondary); }

/* Dropdown */
.dropdown-menu-themed {
  background: var(--roles-dropdown-bg) !important;
  border-color: var(--roles-border-card) !important;
}
.dropdown-menu-themed .dropdown-item {
  font-weight: 700; font-size: 13.5px; color: var(--roles-dropdown-item);
}
.dropdown-menu-themed .dropdown-item:hover { background: var(--roles-dropdown-hover); }
.dropdown-menu-themed .dropdown-divider { border-color: var(--roles-divider); }
.text-amber { color: var(--roles-amber) !important; }

/* ══════════════════════════════════
   MODALE — style amélioré
══════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; z-index: 2000;
  background: rgba(0,0,0,0.65); backdrop-filter: blur(16px);
  display: flex; align-items: center; justify-content: center; padding: 20px;
}
.role-modal-window {
  background: var(--roles-bg-modal);
  border-radius: 40px;
  width: 100%; max-width: 860px; max-height: 90vh;
  display: flex; flex-direction: column; overflow: hidden;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.3),
              0 0 0 1px var(--roles-border-modal);
}

/* Head modale */
.modal-head-v2 {
  padding: 28px 36px; display: flex;
  justify-content: space-between; align-items: center;
  background: var(--roles-bg-modal-head); flex-shrink: 0;
}
.modal-brand-icon {
  width: 50px; height: 50px;
  background: linear-gradient(135deg, #fbbf24, #eab308);
  border-radius: 16px; display: flex; align-items: center;
  justify-content: center; color: #0f172a; font-size: 1.2rem;
  box-shadow: 0 8px 20px rgba(234,179,8,0.3); flex-shrink: 0;
}
.modal-title-v2 { font-size: 1.05rem; font-weight: 900; color: var(--roles-text-primary); margin: 0; letter-spacing: 0.5px; }
.modal-sub-v2   { font-size: 9.5px; font-weight: 800; color: var(--roles-text-muted); letter-spacing: 1.5px; margin: 4px 0 0; }
.header-accent-line { height: 4px; background: linear-gradient(90deg, #fbbf24, #eab308, transparent); flex-shrink: 0; }

.btn-close-modal {
  width: 40px; height: 40px; background: var(--roles-bg-section);
  border: 1px solid var(--roles-border-input); border-radius: 14px;
  color: var(--roles-text-muted); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 14px; transition: all 0.3s;
}
.btn-close-modal:hover { background: rgba(239,68,68,0.12); color: #f43f5e; border-color: rgba(239,68,68,0.3); }

/* Body */
.modal-body-scroll {
  padding: 28px 36px; overflow-y: auto; flex: 1;
  background: var(--roles-bg-section);
}

/* Sections formulaire */
.form-section-card {
  background: var(--roles-bg-modal);
  border-radius: 28px; border: 1px solid var(--roles-border-perm); padding: 28px;
}
.section-badge {
  display: inline-flex; align-items: center; gap: 12px;
  font-size: 11px; font-weight: 800; color: var(--roles-text-primary);
  letter-spacing: 1px; text-transform: uppercase;
}
.section-badge span {
  width: 28px; height: 28px; background: var(--roles-text-heading); color: #f0f6fc;
  border-radius: 9px; display: flex; align-items: center;
  justify-content: center; font-size: 12px; font-weight: 800; flex-shrink: 0;
}

/* Inputs */
.enigma-input-wrap label {
  font-size: 10px; font-weight: 800; color: var(--roles-text-muted);
  letter-spacing: 1px; text-transform: uppercase;
  margin-bottom: 10px; display: block;
}
.input-icon-wrap { position: relative; display: flex; align-items: center; }
.input-icon-wrap i {
  position: absolute; left: 18px; color: var(--roles-amber);
  font-size: 14px; pointer-events: none;
}
.enigma-field {
  width: 100%; padding: 16px 20px 16px 50px;
  border-radius: 18px; border: 1.5px solid var(--roles-border-input);
  background: var(--roles-bg-input);
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 14px; font-weight: 600; color: var(--roles-text-primary);
  outline: none; appearance: none; -webkit-appearance: none;
  transition: all 0.35s cubic-bezier(0.175,0.885,0.32,1.275);
}
.enigma-field:focus {
  border-color: var(--roles-amber); background: var(--roles-bg-modal);
  box-shadow: 0 10px 24px rgba(234,179,8,0.12);
  transform: translateY(-2px);
}
.enigma-field:disabled { opacity: 0.45; cursor: not-allowed; }
.enigma-field::placeholder { color: var(--roles-text-muted); }
.enigma-field option { background: var(--roles-bg-modal); color: var(--roles-text-primary); }

/* Permissions matrix */
.perm-count-pill {
  background: rgba(234,179,8,0.12); color: #d97706;
  border: 1px solid rgba(234,179,8,0.25); font-size: 11px; font-weight: 800;
  padding: 5px 14px; border-radius: 99px;
}
.perm-group {
  background: var(--roles-bg-perm-group); border-radius: 20px;
  padding: 18px; border: 1px solid var(--roles-border-perm);
}
.group-label {
  font-size: 9.5px; font-weight: 800; color: var(--roles-text-muted);
  letter-spacing: 2.5px; margin-bottom: 12px;
  display: flex; align-items: center; gap: 8px;
}
.group-dot {
  width: 6px; height: 6px; border-radius: 50%;
  background: var(--roles-amber); flex-shrink: 0;
}

.perm-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 10px;
}
.perm-node {
  padding: 16px; border-radius: 16px; border: 1.5px solid var(--roles-border-perm);
  background: var(--roles-bg-perm-node); display: flex; gap: 14px; cursor: pointer;
  user-select: none; align-items: flex-start;
  transition: all 0.3s cubic-bezier(0.175,0.885,0.32,1.275);
}
.perm-node:hover {
  border-color: var(--roles-border-card); transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(0,0,0,0.08);
}
.perm-node.active {
  border-color: var(--roles-amber);
  background: var(--roles-bg-perm-node-active);
  box-shadow: 0 8px 24px rgba(234,179,8,0.14);
}
.node-checkbox {
  width: 22px; height: 22px; border: 2px solid var(--roles-text-muted);
  border-radius: 8px; display: flex; align-items: center;
  justify-content: center; color: #0f172a; font-size: 11px;
  flex-shrink: 0; transition: all 0.2s;
}
.active .node-checkbox { background: var(--roles-amber); border-color: var(--roles-amber); }
.node-name { display: block; font-weight: 800; font-size: 13px; color: var(--roles-text-primary); line-height: 1.3; }
.node-desc { display: block; font-size: 11px; color: var(--roles-text-muted); margin-top: 2px; }

/* Footer modale */
.modal-foot-v2 {
  padding: 20px 36px; border-top: 1px solid var(--roles-border-footer);
  display: flex; justify-content: flex-end; gap: 14px;
  background: var(--roles-bg-modal-foot); flex-shrink: 0;
}
.btn-qv-cancel {
  background: var(--roles-btn-cancel-bg); color: var(--roles-btn-cancel-color); border: none;
  padding: 13px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit; font-size: 13px;
  transition: background 0.2s;
}
.btn-qv-cancel:hover { background: var(--roles-btn-cancel-hover); }

/* ══════════════════════════════════
   TOAST
══════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: var(--roles-bg-card); color: var(--roles-text-primary); padding: 18px 28px;
  border-radius: 20px; display: flex; align-items: center;
  gap: 15px; z-index: 9999; border-left: 5px solid var(--roles-amber);
  box-shadow: 0 20px 40px rgba(0,0,0,0.25);
  border: 1px solid var(--roles-border-card);
}
.t-success { border-left-color: #10b981 !important; }
.t-error   { border-left-color: #f43f5e !important; }
.t-warn    { border-left-color: #eab308 !important; }
.t-ico { font-size: 1.1rem; color: var(--roles-text-secondary); }
.t-body strong {
  font-size: 0.65rem; letter-spacing: 1.5px; opacity: 0.6;
  display: block; margin-bottom: 2px; color: var(--roles-text-muted);
}

/* ══════════════════════════════════
   TRANSITIONS
══════════════════════════════════ */
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.93); } to { opacity: 1; transform: scale(1); } }

.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

.fade-enter-active, .fade-leave-active { transition: opacity 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* ══════════════════════════════════
   SCROLLBAR
══════════════════════════════════ */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: var(--roles-border-card); border-radius: 99px; }

/* ══════════════════════════════════
   RESPONSIVE
══════════════════════════════════ */
@media (max-width: 768px) {
  .premium-title { font-size: 1.7rem; }
  .role-modal-window { border-radius: 30px; max-height: 95vh; }
  .modal-head-v2, .modal-body-scroll, .modal-foot-v2 { padding: 20px; }
  .perm-grid { grid-template-columns: 1fr; }
  .form-section-card { padding: 18px; }
}
</style>