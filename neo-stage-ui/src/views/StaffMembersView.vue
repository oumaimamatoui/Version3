<template>
  <div class="enigma-staff-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- ══════════════════════════════════
         BACKGROUND
    ══════════════════════════════════ -->
    <div class="cyber-engine-bg">
      <div class="glow-orb orb-amber"  :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-indigo" :style="orbStyle(0.02)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="p-4 p-lg-5">

          <!-- ══════════════════════════════════
               HEADER PREMIUM
          ══════════════════════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">{{ $t('sidebar.groups.organization') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ $t('staff.terminalTitle') }}</span>
              </div>
              <h2 class="premium-title">
                {{ $t('staff.titlePrefix') }}
                <span class="gradient-text">{{ $t('staff.titleHighlight') }}</span>
              </h2>
              <p class="header-sub mt-1">
                {{ $t('staff.subtitle') }}
                <span class="fw-900 text-dark">
                  {{ authStore.user?.entrepriseNom || $t('staff.defaultCompany') }}
                </span>
              </p>
            </div>

            <div class="d-flex gap-3 flex-wrap align-items-center">
              <div class="system-live-badge">
                <span class="pulse-dot"></span>
                {{ $t('invite.networkOk') }}
              </div>

              <div class="view-toggle-cluster">
                <button
                  :class="['btn-view-toggle', { active: viewMode === 'table' }]"
                  @click="viewMode = 'table'"
                  :title="$t('staff.viewTable')"
                >
                  <i class="fa-solid fa-table-list"></i>
                </button>
                <button
                  :class="['btn-view-toggle', { active: viewMode === 'grid' }]"
                  @click="viewMode = 'grid'"
                  :title="$t('staff.viewGrid')"
                >
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
              </div>

              <button
                @click="loadData"
                class="btn-refresh-pro"
                :title="$t('refresh')"
                :disabled="loading"
              >
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>

              <div class="counter-badge-pro">
                <i class="fa-solid fa-users me-2"></i>
                <span class="fw-900">{{ filteredStaff.length }}</span>
                <span class="badge-label ms-1">{{ $t('staff.membersLabel') }}</span>
              </div>
            </div>
          </header>

          <!-- ══════════════════════════════════
               KPI STRIP
          ══════════════════════════════════ -->
          <div class="row g-3 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details ms-3">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- ══════════════════════════════════
               FILTRES PANEL
          ══════════════════════════════════ -->
          <div class="enigma-card p-3 mb-4 d-flex align-items-center gap-3 flex-wrap">
            <div class="search-inline-box flex-grow-1">
              <i class="fa-solid fa-magnifying-glass"></i>
              <input
                type="text"
                v-model="searchQuery"
                class="search-inline-input"
                :placeholder="$t('staff.searchPlaceholder')"
              />
              <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>

            <div class="filter-select-wrap">
              <i class="fa-solid fa-filter filter-icon"></i>
              <select v-model="selectedRoleFilter" class="filter-select-pro">
                <option value="">{{ $t('staff.allRoles') }}</option>
                <option v-for="role in filteredRolesList" :key="role.id" :value="role.nom">
                  {{ role.nom }}
                </option>
              </select>
            </div>

            <div class="sort-select-wrap">
              <select v-model="sortBy" class="filter-select-pro">
                <option value="name">{{ $t('staff.sortName') }}</option>
                <option value="date">{{ $t('staff.sortDate') }}</option>
                <option value="role">{{ $t('staff.sortRole') }}</option>
              </select>
            </div>
          </div>

          <!-- ══════════════════════════════════
               CHARGEMENT
          ══════════════════════════════════ -->
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="mt-3 text-muted small fw-700">{{ $t('staff.syncing') }}</p>
          </div>

          <!-- ══════════════════════════════════
               VUE TABLEAU
          ══════════════════════════════════ -->
          <div v-else-if="viewMode === 'table'" class="animate__animated animate__fadeIn">
            <div class="enigma-card overflow-hidden">
              <!-- Table header bar -->
              <div class="table-header-elite">
                <div class="d-flex align-items-center gap-2">
                  <div class="table-header-icon">
                    <i class="fa-solid fa-users"></i>
                  </div>
                  <span>{{ $t('staff.directoryTitle') }}</span>
                </div>
                <div class="table-badge-count">
                  {{ paginatedStaff.length }} / {{ filteredStaff.length }}
                </div>
              </div>

              <div class="table-responsive">
                <table class="table elite-table mb-0">
                  <thead>
                    <tr>
                      <th>{{ $t('staff.colMember') }}</th>
                      <th>{{ $t('staff.colRole') }}</th>
                      <th>{{ $t('staff.colEmail') }}</th>
                      <th>{{ $t('staff.colDate') }}</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="member in paginatedStaff" :key="member.id">
                      <!-- Membre -->
                      <td>
                        <div class="d-flex align-items-center gap-3">
                          <div class="staff-avatar">
                            <img
                              v-if="member.photoUrl"
                              :src="getPhotoUrl(member.photoUrl)"
                              class="avatar-img-fit"
                              :alt="$t('profile.avatarAlt')"
                            />
                            <span v-else>
                              {{ member.prenom?.charAt(0) }}{{ member.nomFamille?.charAt(0) }}
                            </span>
                          </div>
                          <div>
                            <div class="fw-800 text-dark">
                              {{ member.prenom }} {{ member.nomFamille }}
                            </div>
                            <div class="member-status-dot">
                              <span class="status-dot-green"></span>
                              <span>{{ $t('staff.activeMember') }}</span>
                            </div>
                          </div>
                        </div>
                      </td>

                      <!-- Rôle -->
                      <td>
                        <span class="role-badge" :class="getRoleClass(member.roleNom)">
                          <i class="fa-solid fa-shield-halved me-1"></i>
                          {{ member.roleNom }}
                        </span>
                      </td>

                      <!-- Email -->
                      <td>
                        <div class="email-cell-text">{{ member.email }}</div>
                      </td>

                      <!-- Date -->
                      <td>
                        <div class="date-cell">{{ formatDate(member.creeLe) }}</div>
                      </td>

                      <!-- Actions -->
                      <td class="text-end">
                        <div class="d-flex justify-content-end gap-2" v-if="isAdmin">
                          <button
                            @click="openEditModal(member)"
                            class="btn-icon-sm"
                            :title="$t('edit')"
                          >
                            <i class="fa-solid fa-pen-to-square"></i>
                          </button>
                          <button
                            @click="deleteMemberConfirm(member)"
                            class="btn-icon-sm danger"
                            :title="$t('delete')"
                          >
                            <i class="fa-solid fa-trash-can"></i>
                          </button>
                        </div>
                        <span v-else class="consultation-badge">
                          {{ $t('staff.readOnly') }}
                        </span>
                      </td>
                    </tr>

                    <!-- Empty state -->
                    <tr v-if="filteredStaff.length === 0">
                      <td colspan="5" class="empty-state-row">
                        <div class="empty-icon"><i class="fa-solid fa-user-slash"></i></div>
                        <p>{{ $t('staff.emptyMessage') }}</p>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- PAGINATION TABLE -->
              <div v-if="totalPages > 1" class="pagination-bar">
                <div class="pagination-info">
                  <span>{{ $t('staff.paginationShowing') }}</span>
                  <strong>
                    {{ currentPage * perPage + 1 }}–{{ Math.min((currentPage + 1) * perPage, filteredStaff.length) }}
                  </strong>
                  <span>{{ $t('staff.paginationOf') }}</span>
                  <strong>{{ filteredStaff.length }}</strong>
                  <span>{{ $t('staff.paginationMembers') }}</span>
                </div>

                <div class="pagination-controls">
                  <button
                    class="pg-btn"
                    @click="currentPage = 0"
                    :disabled="currentPage === 0"
                    :title="$t('staff.paginationFirst')"
                  >
                    <i class="fa-solid fa-angles-left"></i>
                  </button>
                  <button
                    class="pg-btn"
                    @click="currentPage--"
                    :disabled="currentPage === 0"
                    :title="$t('staff.paginationPrev')"
                  >
                    <i class="fa-solid fa-angle-left"></i>
                  </button>
                  <div class="pg-numbers">
                    <button
                      v-for="p in pagesRange"
                      :key="p"
                      :class="['pg-num', { active: p === currentPage, ellipsis: p === '...' }]"
                      @click="p !== '...' && (currentPage = p)"
                      :disabled="p === '...'"
                    >
                      {{ p === '...' ? '…' : p + 1 }}
                    </button>
                  </div>
                  <button
                    class="pg-btn"
                    @click="currentPage++"
                    :disabled="currentPage >= totalPages - 1"
                    :title="$t('staff.paginationNext')"
                  >
                    <i class="fa-solid fa-angle-right"></i>
                  </button>
                  <button
                    class="pg-btn"
                    @click="currentPage = totalPages - 1"
                    :disabled="currentPage >= totalPages - 1"
                    :title="$t('staff.paginationLast')"
                  >
                    <i class="fa-solid fa-angles-right"></i>
                  </button>
                </div>

                <div class="pagination-size">
                  <span class="pg-size-label">{{ $t('staff.paginationRows') }}</span>
                  <select v-model="perPage" @change="currentPage = 0" class="pg-size-select">
                    <option :value="5">5</option>
                    <option :value="10">10</option>
                    <option :value="20">20</option>
                    <option :value="50">50</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

          <!-- ══════════════════════════════════
               VUE GRILLE
          ══════════════════════════════════ -->
          <div v-else-if="viewMode === 'grid'" class="animate__animated animate__fadeIn">
            <div class="row g-4">
              <!-- Empty state grille -->
              <div v-if="filteredStaff.length === 0" class="col-12">
                <div class="empty-state-grid">
                  <div class="empty-icon"><i class="fa-solid fa-user-slash"></i></div>
                  <p>{{ $t('staff.emptyMessageShort') }}</p>
                </div>
              </div>

              <!-- Cartes membres -->
              <div
                v-else
                v-for="member in paginatedStaff"
                :key="member.id"
                class="col-xl-3 col-lg-4 col-md-6"
              >
                <div class="staff-grid-card">
                  <div class="grid-card-top">
                    <span class="role-badge" :class="getRoleClass(member.roleNom)">
                      <i class="fa-solid fa-shield-halved me-1"></i>{{ member.roleNom }}
                    </span>
                    <div class="d-flex gap-1" v-if="isAdmin">
                      <button
                        @click="openEditModal(member)"
                        class="btn-icon-sm sm"
                        :title="$t('edit')"
                      >
                        <i class="fa-solid fa-pen-to-square"></i>
                      </button>
                      <button
                        @click="deleteMemberConfirm(member)"
                        class="btn-icon-sm danger sm"
                        :title="$t('delete')"
                      >
                        <i class="fa-solid fa-trash-can"></i>
                      </button>
                    </div>
                  </div>

                  <div class="grid-avatar-wrap">
                    <div class="staff-avatar large">
                      <img
                        v-if="member.photoUrl"
                        :src="getPhotoUrl(member.photoUrl)"
                        class="avatar-img-fit"
                        :alt="$t('profile.avatarAlt')"
                      />
                      <span v-else>
                        {{ member.prenom?.charAt(0) }}{{ member.nomFamille?.charAt(0) }}
                      </span>
                    </div>
                    <div class="grid-status-ring"></div>
                  </div>

                  <div class="grid-card-body">
                    <h6 class="fw-900 mb-1">{{ member.prenom }} {{ member.nomFamille }}</h6>
                    <div class="grid-email">{{ member.email }}</div>
                    <div class="grid-date">
                      <i class="fa-regular fa-calendar me-1"></i>{{ formatDate(member.creeLe) }}
                    </div>
                  </div>

                  <div class="member-status-dot justify-content-center mt-2">
                    <span class="status-dot-green"></span>
                    <span>{{ $t('staff.activeMember') }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- PAGINATION GRILLE -->
            <div v-if="totalPages > 1" class="enigma-card mt-4 overflow-hidden">
              <div class="pagination-bar border-0">
                <div class="pagination-info">
                  <span>{{ $t('staff.paginationShowing') }}</span>
                  <strong>
                    {{ currentPage * perPage + 1 }}–{{ Math.min((currentPage + 1) * perPage, filteredStaff.length) }}
                  </strong>
                  <span>{{ $t('staff.paginationOf') }}</span>
                  <strong>{{ filteredStaff.length }}</strong>
                  <span>{{ $t('staff.paginationMembers') }}</span>
                </div>
                <div class="pagination-controls">
                  <button class="pg-btn" @click="currentPage = 0" :disabled="currentPage === 0">
                    <i class="fa-solid fa-angles-left"></i>
                  </button>
                  <button class="pg-btn" @click="currentPage--" :disabled="currentPage === 0">
                    <i class="fa-solid fa-angle-left"></i>
                  </button>
                  <div class="pg-numbers">
                    <button
                      v-for="p in pagesRange"
                      :key="p"
                      :class="['pg-num', { active: p === currentPage, ellipsis: p === '...' }]"
                      @click="p !== '...' && (currentPage = p)"
                      :disabled="p === '...'"
                    >
                      {{ p === '...' ? '…' : p + 1 }}
                    </button>
                  </div>
                  <button class="pg-btn" @click="currentPage++" :disabled="currentPage >= totalPages - 1">
                    <i class="fa-solid fa-angle-right"></i>
                  </button>
                  <button class="pg-btn" @click="currentPage = totalPages - 1" :disabled="currentPage >= totalPages - 1">
                    <i class="fa-solid fa-angles-right"></i>
                  </button>
                </div>
                <div class="pagination-size">
                  <span class="pg-size-label">{{ $t('staff.paginationRows') }}</span>
                  <select v-model="perPage" @change="currentPage = 0" class="pg-size-select">
                    <option :value="8">8</option>
                    <option :value="12">12</option>
                    <option :value="24">24</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- ══════════════════════════════════
         MODALE ÉDITION
    ══════════════════════════════════ -->
    <transition name="modal-quantum">
      <div
        v-if="showEditModal"
        class="quantum-vault-overlay"
        @click.self="showEditModal = false"
      >
        <div class="enigma-modal animate__animated animate__zoomIn animate__faster">
          <div class="modal-header-elite">
            <div class="d-flex align-items-center gap-3">
              <div class="modal-header-icon">
                <i class="fa-solid fa-pen-to-square"></i>
              </div>
              <div>
                <div class="fw-900" style="font-size:14px">{{ $t('staff.modal.editTitle') }}</div>
                <div style="font-size:11px;opacity:.5;font-weight:700;letter-spacing:1px">
                  {{ $t('staff.modal.protocol') }}
                </div>
              </div>
            </div>
            <button @click="showEditModal = false" class="btn-close-elite">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="p-4">
            <div class="row g-3 mb-3">
              <div class="col-6">
                <div class="enigma-input-wrap">
                  <label>{{ $t('settings.labels.firstName') }}</label>
                  <input
                    v-model="editForm.prenom"
                    class="enigma-field"
                    :placeholder="$t('settings.labels.firstName')"
                  />
                </div>
              </div>
              <div class="col-6">
                <div class="enigma-input-wrap">
                  <label>{{ $t('settings.labels.lastName') }}</label>
                  <input
                    v-model="editForm.nomFamille"
                    class="enigma-field"
                    :placeholder="$t('settings.labels.lastName')"
                  />
                </div>
              </div>
            </div>

            <div class="enigma-input-wrap mb-3">
              <label>{{ $t('email') }}</label>
              <input v-model="editForm.email" class="enigma-field" readonly />
              <p class="field-hint-readonly mt-1">{{ $t('staff.modal.emailReadonly') }}</p>
            </div>

            <div class="enigma-input-wrap mb-4">
              <label>{{ $t('role') }}</label>
              <div class="theme-select-wrapper">
                <i class="fa-solid fa-shield-halved theme-select-icon"></i>
                <select v-model="editForm.role" class="enigma-field theme-select">
                  <option v-for="r in filteredRolesList" :key="r.id" :value="r.nom">
                    {{ r.nom }}
                  </option>
                </select>
              </div>
              <p class="field-hint-info mt-1">{{ $t('staff.modal.roleHint') }}</p>
            </div>

            <div v-if="editError" class="error-alert mb-3">
              <i class="fa-solid fa-circle-exclamation me-2"></i>{{ editError }}
            </div>

            <div class="d-flex gap-3 justify-content-end">
              <button @click="showEditModal = false" class="btn-qv-cancel">
                {{ $t('cancel') }}
              </button>
              <button @click="handleSave" class="btn-enigma-primary" :disabled="saving">
                <div class="btn-glow"></div>
                <div class="btn-content">
                  <span v-if="!saving">{{ $t('save') }}</span>
                  <div v-else class="btn-dots-loader">
                    <span></span><span></span><span></span>
                  </div>
                </div>
              </button>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- ══════════════════════════════════
         CONFIRM DIALOG
    ══════════════════════════════════ -->
    <transition name="modal-quantum">
      <div
        v-if="confirmDialog.show"
        class="quantum-vault-overlay"
        style="z-index:9999"
        @click.self="confirmDialog.show = false"
      >
        <div class="confirm-modal animate__animated animate__zoomIn animate__faster">
          <div class="confirm-icon mb-3">
            <i :class="confirmDialog.icon" class="fa-2x text-danger"></i>
          </div>
          <h5 class="fw-900 mb-2">{{ confirmDialog.title }}</h5>
          <p class="text-muted small mb-4">{{ confirmDialog.message }}</p>
          <div class="d-flex gap-3 justify-content-center">
            <button @click="confirmDialog.show = false" class="btn-qv-cancel">
              {{ $t('cancel') }}
            </button>
            <button @click="runConfirm" class="btn-confirm-danger">
              {{ $t('confirm') }}
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- ══════════════════════════════════
         TOAST
    ══════════════════════════════════ -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>{{ $t('dashboard.toast.systemMessage') }}</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useAuthStore } from '@/stores/auth';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar  from '@/components/AppNavbar.vue';

/* ─── I18N ────────────────────────────────────────────────── */
const { t } = useI18n();

/* ─── STORES ──────────────────────────────────────────────── */
const authStore = useAuthStore();

/* ─── STATE ───────────────────────────────────────────────── */
const staff              = ref([]);
const rolesList          = ref([]);
const loading            = ref(true);
const saving             = ref(false);
const searchQuery        = ref('');
const selectedRoleFilter = ref('');
const sortBy             = ref('name');
const viewMode           = ref('table');
const showEditModal      = ref(false);
const editError          = ref(null);
const mousePos           = reactive({ x: 0, y: 0 });

/* ─── PAGINATION ──────────────────────────────────────────── */
const currentPage = ref(0);
const perPage     = ref(10);

/* ─── DIALOGS / TOAST ─────────────────────────────────────── */
const globalToast   = reactive({ active: false, message: '', type: '', icon: '' });
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _cb: null });

const editForm = reactive({
  id: null,
  prenom: '',
  nomFamille: '',
  email: '',
  role: '',
});

/* ─── COMPUTED ────────────────────────────────────────────── */
const isAdmin = computed(() =>
  (authStore.role || '').toLowerCase().includes('admin')
);

const filteredRolesList = computed(() =>
  rolesList.value.filter(role => {
    const notSuper    = role.nom !== 'SuperAdmin';
    const sameCompany =
      authStore.role === 'SuperAdmin' ||
      role.entrepriseId === authStore.user?.entrepriseId;
    return notSuper && sameCompany;
  })
);

const filteredStaff = computed(() => {
  let list = staff.value.filter(member => {
    if (member.roleNom === 'SuperAdmin') return false;
    const search      = searchQuery.value.toLowerCase();
    const matchSearch =
      `${member.prenom} ${member.nomFamille}`.toLowerCase().includes(search) ||
      member.email.toLowerCase().includes(search);
    const matchRole   =
      selectedRoleFilter.value === '' || member.roleNom === selectedRoleFilter.value;
    return matchSearch && matchRole;
  });

  list.sort((a, b) => {
    if (sortBy.value === 'name')
      return `${a.prenom} ${a.nomFamille}`.localeCompare(`${b.prenom} ${b.nomFamille}`);
    if (sortBy.value === 'date')
      return new Date(b.creeLe || 0) - new Date(a.creeLe || 0);
    if (sortBy.value === 'role')
      return (a.roleNom || '').localeCompare(b.roleNom || '');
    return 0;
  });

  return list;
});

const totalPages = computed(() =>
  Math.ceil(filteredStaff.value.length / perPage.value)
);

const paginatedStaff = computed(() => {
  const start = currentPage.value * perPage.value;
  return filteredStaff.value.slice(start, start + perPage.value);
});

const pagesRange = computed(() => {
  const total   = totalPages.value;
  const current = currentPage.value;
  if (total <= 7) return Array.from({ length: total }, (_, i) => i);
  const pages = [];
  if (current > 2)         { pages.push(0); if (current > 3) pages.push('...'); }
  for (let i = Math.max(0, current - 2); i <= Math.min(total - 1, current + 2); i++)
    pages.push(i);
  if (current < total - 3) { if (current < total - 4) pages.push('...'); pages.push(total - 1); }
  return pages;
});

const kpiStats = computed(() => [
  {
    label: t('staff.kpi.total'),
    value: filteredStaff.value.length,
    icon:  'fa-solid fa-users',
    color: '#6366f1',
    bg:    '#eef2ff',
  },
  {
    label: t('staff.kpi.activeRoles'),
    value: filteredRolesList.value.length,
    icon:  'fa-solid fa-id-badge',
    color: '#f59e0b',
    bg:    '#fffbeb',
  },
  {
    label: t('staff.kpi.admins'),
    value: staff.value.filter(m => m.roleNom?.toLowerCase().includes('admin')).length,
    icon:  'fa-solid fa-shield-halved',
    color: '#10b981',
    bg:    '#ecfdf5',
  },
  {
    label: t('staff.kpi.evaluators'),
    value: staff.value.filter(m => m.roleNom === 'Evaluateur').length,
    icon:  'fa-solid fa-user-check',
    color: '#f43f5e',
    bg:    '#fff1f2',
  },
]);

/* ─── WATCHERS ────────────────────────────────────────────── */
watch([searchQuery, selectedRoleFilter, sortBy], () => {
  currentPage.value = 0;
});

/* ─── DATA ────────────────────────────────────────────────── */
const loadData = async () => {
  loading.value = true;
  try {
    const [resStaff, resRoles] = await Promise.all([
      api.get('/Staff'),
      api.get('/Roles'),
    ]);
    staff.value     = resStaff.data;
    rolesList.value = resRoles.data;
  } catch {
    showToast(
      t('staff.toast.loadError'),
      't-error',
      'fa-solid fa-circle-exclamation'
    );
  } finally {
    loading.value = false;
  }
};

/* ─── EDIT MODAL ──────────────────────────────────────────── */
const openEditModal = (member) => {
  editForm.id         = member.id;
  editForm.prenom     = member.prenom;
  editForm.nomFamille = member.nomFamille;
  editForm.email      = member.email;
  editForm.role       = member.roleNom;
  editError.value     = null;
  showEditModal.value = true;
};

const handleSave = async () => {
  saving.value    = true;
  editError.value = null;
  try {
    await api.put(`/Staff/${editForm.id}`, {
      prenom:  editForm.prenom,
      nom:     editForm.nomFamille,
      roleNom: editForm.role,
    });
    showEditModal.value = false;
    showToast(t('staff.toast.updated'), 't-success', 'fa-solid fa-circle-check');
    loadData();
  } catch (err) {
    editError.value = err.response?.data?.message || t('staff.toast.updateError');
  } finally {
    saving.value = false;
  }
};

/* ─── DELETE ──────────────────────────────────────────────── */
const deleteMemberConfirm = (member) => {
  showConfirmDialog(
    t('staff.confirm.deleteTitle'),
    t('staff.confirm.deleteMessage', { name: `${member.prenom} ${member.nomFamille}` }),
    'fa-solid fa-trash-can',
    async () => {
      try {
        await api.delete(`/Staff/${member.id}`);
        showToast(t('staff.toast.deleted'), 't-warn', 'fa-solid fa-trash-can');
        loadData();
      } catch {
        showToast(t('staff.toast.deleteError'), 't-error', 'fa-solid fa-circle-exclamation');
      }
    }
  );
};

/* ─── UTILS ───────────────────────────────────────────────── */
const getPhotoUrl = (url) => {
  if (!url) return null;
  if (url.startsWith('http')) return url;
  return `${import.meta.env.VITE_BASE_URL || 'http://localhost:5172'}/${url.replace(/\\/g, '/')}`;
};

const formatDate = (d) => {
  if (!d) return '—';
  const localeMap  = { FR: 'fr-FR', EN: 'en-GB', AR: 'ar-TN' };
  const currentLang = localStorage.getItem('lang') || 'FR';
  return new Date(d).toLocaleDateString(localeMap[currentLang] || 'fr-FR', {
    day: '2-digit', month: 'short', year: 'numeric',
  });
};

const getRoleClass = (role) => {
  if (!role) return 'role-default';
  const r = role.toLowerCase();
  if (r.includes('admin'))   return 'role-admin';
  if (r.includes('evaluat')) return 'role-eval';
  if (r.includes('recrut'))  return 'role-recrut';
  return 'role-default';
};

const showConfirmDialog = (title, message, icon, cb) => {
  Object.assign(confirmDialog, { title, message, icon, _cb: cb, show: true });
};

const runConfirm = () => {
  confirmDialog.show = false;
  if (confirmDialog._cb) confirmDialog._cb();
};

let _toastTimer = null;
const showToast = (msg, type = 't-success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

/* ─── PARALLAX ────────────────────────────────────────────── */
const orbStyle       = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`,
});
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(loadData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800;900&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css');

/* ═══════════════════════ BASE ═══════════════════════ */
.enigma-staff-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* ═══════════════════════ BACKGROUND ═══════════════════════ */
.cyber-engine-bg {
  position: fixed; inset: 0; z-index: 0; pointer-events: none;
}
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.18;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.12; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber  { background: #f59e0b; top: -200px; right: -100px; }
.orb-indigo { background: #6366f1; bottom: -200px; left: -100px; }

.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ═══════════════════════ HEADER ═══════════════════════ */
.premium-title {
  font-weight: 900; font-size: 2.2rem;
  letter-spacing: -1px; margin: 0; line-height: 1.1;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.header-sub { font-size: 13px; color: #64748b; font-weight: 600; margin: 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root     { cursor: default; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current  { color: #0f172a; font-weight: 800; }

.system-live-badge {
  background: white; border: 1px solid #f1f5f9; padding: 10px 18px;
  border-radius: 50px; font-size: 11px; font-weight: 800; color: #10b981;
  display: flex; align-items: center; box-shadow: 0 4px 12px rgba(0,0,0,0.04);
}
.pulse-dot {
  width: 8px; height: 8px; background: #10b981; border-radius: 50%;
  display: inline-block; margin-right: 8px;
  animation: pulse-anim 2s infinite;
}
@keyframes pulse-anim {
  0%   { box-shadow: 0 0 0 0 rgba(16,185,129,0.7); }
  70%  { box-shadow: 0 0 0 8px rgba(16,185,129,0); }
  100% { box-shadow: 0 0 0 0 rgba(16,185,129,0); }
}
.counter-badge-pro {
  background: #0f172a; color: #f59e0b; padding: 10px 18px;
  border-radius: 50px; font-size: 11px; font-weight: 800;
  display: flex; align-items: center; gap: 4px;
}
.badge-label { font-size: 9px; font-weight: 700; opacity: 0.6; letter-spacing: 1px; }

/* VIEW TOGGLE */
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
.btn-view-toggle:hover         { background: #f8fafc; color: #0f172a; }
.btn-view-toggle.active        {
  background: #0f172a; color: #f59e0b;
  box-shadow: 0 4px 12px rgba(15,23,42,0.2);
}

.btn-refresh-pro {
  width: 44px; height: 44px; background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center; font-size: 15px;
}
.btn-refresh-pro:hover:not(:disabled) {
  background: #f8fafc; border-color: #f59e0b; color: #f59e0b;
}
.btn-refresh-pro:disabled { opacity: 0.5; cursor: not-allowed; }

/* ═══════════════════════ KPI CARDS ═══════════════════════ */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 20px;
  display: flex; align-items: center; border: 1px solid #eef2f6;
  transition: 0.2s; box-shadow: 0 2px 8px rgba(0,0,0,0.02);
}
.stat-card-premium:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 30px rgba(0,0,0,0.06);
}
.stat-icon-wrapper {
  width: 52px; height: 52px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.3rem; flex-shrink: 0;
}
.stat-value {
  font-size: 1.7rem; font-weight: 900; line-height: 1; color: #0f172a;
}
.stat-label {
  font-size: 0.68rem; font-weight: 700; color: #94a3b8;
  text-transform: uppercase; margin-top: 4px; letter-spacing: 0.5px;
}

/* ═══════════════════════ FILTER PANEL ═══════════════════════ */
.enigma-card {
  background: white; border-radius: 24px;
  border: 1px solid #eef2f6; box-shadow: 0 2px 8px rgba(0,0,0,0.02);
}
.search-inline-box {
  display: flex; align-items: center; gap: 10px;
  padding: 10px 16px; color: #94a3b8; min-width: 0;
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  font-weight: 700; font-size: 0.88rem; width: 100%;
  font-family: inherit; color: #0f172a;
}
.search-inline-input::placeholder { color: #cbd5e1; }
.btn-clear-search {
  border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer;
}

.filter-select-wrap, .sort-select-wrap { position: relative; flex-shrink: 0; }
.filter-icon {
  position: absolute; left: 12px; top: 50%; transform: translateY(-50%);
  font-size: 12px; color: #94a3b8; pointer-events: none;
}
.filter-select-pro {
  padding: 10px 16px 10px 32px; border: 1.5px solid #eef2f6; border-radius: 14px;
  background: #f8fafc; font-size: 12px; font-weight: 800; color: #0f172a;
  cursor: pointer; outline: none; font-family: inherit; appearance: none;
}
.sort-select-wrap .filter-select-pro { padding-left: 16px; }
.filter-select-pro:focus { border-color: #f59e0b; }

/* ═══════════════════════ TABLE ═══════════════════════ */
.table-header-elite {
  display: flex; justify-content: space-between; align-items: center;
  background: #f8fafc; padding: 18px 24px; border-bottom: 1px solid #f1f5f9;
  font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px;
}
.table-header-icon {
  width: 28px; height: 28px; background: #0f172a; color: #f59e0b;
  border-radius: 8px; display: flex; align-items: center; justify-content: center;
  font-size: 12px;
}
.table-badge-count {
  background: #fffbeb; color: #b45309;
  padding: 4px 12px; border-radius: 100px;
  font-size: 11px; font-weight: 800;
}

.elite-table { width: 100%; border-collapse: collapse; }
.elite-table thead th {
  border: none; font-size: 10px; font-weight: 800; color: #94a3b8;
  letter-spacing: 1.5px; padding: 14px 24px;
  text-transform: uppercase; background: transparent;
}
.elite-table td {
  padding: 18px 24px; border-top: 1px solid #f8fafc; vertical-align: middle;
}
.elite-table tbody tr           { transition: background 0.2s; }
.elite-table tbody tr:hover     { background: #fafafa; }

/* AVATAR */
.staff-avatar {
  width: 44px; height: 44px;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  color: #0f172a; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 14px; flex-shrink: 0; overflow: hidden;
  box-shadow: 0 4px 10px rgba(245,158,11,0.2);
}
.staff-avatar.large {
  width: 72px; height: 72px; font-size: 22px; border-radius: 22px;
}
.avatar-img-fit { width: 100%; height: 100%; object-fit: cover; }

.member-status-dot {
  display: flex; align-items: center; gap: 5px;
  font-size: 10px; font-weight: 800; color: #10b981;
  text-transform: uppercase; letter-spacing: 0.5px; margin-top: 3px;
}
.status-dot-green {
  width: 6px; height: 6px; background: #10b981; border-radius: 50%;
}

/* ROLE BADGES */
.role-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 11px; font-weight: 800; letter-spacing: 0.3px;
  display: inline-flex; align-items: center;
}
.role-admin   { background: #fef3c7; color: #b45309; border: 1px solid #fde68a; }
.role-eval    { background: #ecfdf5; color: #065f46; border: 1px solid #a7f3d0; }
.role-recrut  { background: #eff6ff; color: #1d4ed8; border: 1px solid #bfdbfe; }
.role-default { background: #f1f5f9; color: #475569; border: 1px solid #e2e8f0; }

.email-cell-text { font-size: 13px; font-weight: 600; color: #475569; }
.date-cell {
  font-size: 11px; font-weight: 800; color: #94a3b8;
  text-transform: uppercase; letter-spacing: 0.5px;
}
.consultation-badge {
  font-size: 10px; font-weight: 700; color: #94a3b8; font-style: italic;
}

.btn-icon-sm {
  width: 34px; height: 34px; border-radius: 10px; border: 1.5px solid #eef2f6;
  background: white; color: #64748b; cursor: pointer; font-size: 13px;
  display: flex; align-items: center; justify-content: center; transition: 0.2s;
}
.btn-icon-sm.sm           { width: 28px; height: 28px; font-size: 11px; border-radius: 8px; }
.btn-icon-sm:hover        { background: #f8fafc; color: #0f172a; }
.btn-icon-sm.danger:hover { background: #fff1f2; color: #f43f5e; border-color: #f43f5e; }

.empty-state-row { text-align: center; padding: 60px 0 !important; }
.empty-icon      { font-size: 36px; color: #e2e8f0; margin-bottom: 12px; }
.empty-state-row p { color: #94a3b8; font-size: 14px; margin: 0; }

/* ═══════════════════════ GRID VIEW ═══════════════════════ */
.staff-grid-card {
  background: white; border-radius: 28px; padding: 24px;
  border: 1px solid #eef2f6; transition: 0.3s;
  box-shadow: 0 2px 8px rgba(0,0,0,0.02);
  display: flex; flex-direction: column; gap: 10px;
}
.staff-grid-card:hover {
  transform: translateY(-6px);
  border-color: #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.06);
}
.grid-card-top {
  display: flex; justify-content: space-between; align-items: center;
}
.grid-avatar-wrap {
  position: relative; display: flex; justify-content: center; padding: 8px 0;
}
.grid-status-ring {
  position: absolute; bottom: 8px; right: calc(50% - 44px);
  width: 14px; height: 14px; border-radius: 50%;
  background: #10b981; border: 2px solid white;
}
.grid-card-body { text-align: center; }
.grid-card-body h6 { font-size: 15px; color: #0f172a; }
.grid-email { font-size: 12px; color: #94a3b8; font-weight: 600; margin: 2px 0; word-break: break-all; }
.grid-date  { font-size: 11px; color: #cbd5e1; font-weight: 700; margin-top: 4px; }

.empty-state-grid {
  background: white; border-radius: 32px; padding: 80px 40px;
  text-align: center; border: 2px dashed #e2e8f0;
  display: flex; flex-direction: column; align-items: center;
}
.empty-state-grid .empty-icon { font-size: 40px; color: #e2e8f0; margin-bottom: 16px; }
.empty-state-grid p { color: #94a3b8; }

/* ═══════════════════════ MODAL ═══════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; background: rgba(15,23,42,0.65);
  backdrop-filter: blur(10px); z-index: 2000;
  display: flex; align-items: center; justify-content: center;
}
.enigma-modal {
  background: white; border-radius: 32px; width: 560px; max-width: 95vw;
  box-shadow: 0 40px 100px rgba(0,0,0,0.2); overflow: hidden;
}
.modal-header-elite {
  padding: 24px 28px; border-bottom: 1px solid #eef2f6;
  background: #fafbfc;
  display: flex; justify-content: space-between; align-items: center;
}
.modal-header-icon {
  width: 44px; height: 44px; background: #fffbeb; color: #f59e0b;
  border-radius: 14px; display: flex; align-items: center; justify-content: center;
  font-size: 18px;
}
.btn-close-elite {
  width: 34px; height: 34px; border-radius: 10px; border: 1.5px solid #eef2f6;
  background: white; color: #64748b; cursor: pointer;
  display: flex; align-items: center; justify-content: center; transition: 0.2s;
}
.btn-close-elite:hover { background: #fee2e2; color: #f43f5e; border-color: #fecaca; }

.enigma-input-wrap label {
  display: block; font-size: 10px; font-weight: 900; color: #94a3b8;
  margin-bottom: 8px; text-transform: uppercase; letter-spacing: 0.8px;
}
.enigma-field {
  width: 100%; padding: 13px 18px; border-radius: 14px;
  border: 2px solid #f1f5f9; background: #f8fafc;
  font-size: 14px; font-weight: 600; color: #0f172a;
  transition: all 0.3s; appearance: none; font-family: inherit; outline: none;
}
.enigma-field:focus {
  border-color: #f59e0b; background: white;
  box-shadow: 0 0 0 4px rgba(251,191,36,0.1);
}
.enigma-field[readonly] { background: #f1f5f9; color: #94a3b8; cursor: not-allowed; }

.theme-select-wrapper { position: relative; }
.theme-select-icon {
  position: absolute; left: 16px; top: 50%; transform: translateY(-50%);
  color: #f59e0b; font-size: 14px; z-index: 2; pointer-events: none;
}
.theme-select { padding-left: 44px !important; cursor: pointer; }

.field-hint-readonly { font-size: 11px; color: #94a3b8; font-weight: 700; margin: 6px 0 0 2px; }
.field-hint-info     { font-size: 11px; color: #64748b; font-weight: 600; }

.error-alert {
  background: #fff1f2; border: 1px solid #fecaca; color: #e11d48;
  padding: 12px 16px; border-radius: 12px; font-size: 13px; font-weight: 600;
}

/* BUTTONS */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 13px 24px; border-radius: 16px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer;
  font-family: inherit; transition: 0.3s;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow    { opacity: 1; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2;
  display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-dots-loader { display: flex; gap: 5px; align-items: center; padding: 0 4px; }
.btn-dots-loader span {
  width: 6px; height: 6px; background: white; border-radius: 50%;
  animation: dots 1.2s ease-in-out infinite;
}
.btn-dots-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-dots-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes dots {
  0%,80%,100% { transform: scale(0.6); opacity: 0.4; }
  40%          { transform: scale(1);   opacity: 1;   }
}

.btn-qv-cancel {
  background: #f1f5f9; color: #64748b; border: none;
  padding: 12px 24px; border-radius: 14px;
  font-weight: 800; cursor: pointer; font-family: inherit;
}
.btn-qv-cancel:hover { background: #e2e8f0; }

/* CONFIRM MODAL */
.confirm-modal {
  background: white; border-radius: 32px; padding: 40px;
  width: 420px; max-width: 95vw; text-align: center;
  box-shadow: 0 40px 80px rgba(0,0,0,0.15);
}
.confirm-icon { display: flex; justify-content: center; }
.btn-confirm-danger {
  background: #f43f5e; color: white; border: none;
  padding: 12px 24px; border-radius: 14px;
  font-weight: 800; cursor: pointer; font-family: inherit;
}
.btn-confirm-danger:hover { background: #e11d48; }

/* ═══════════════════════ PAGINATION ═══════════════════════ */
.pagination-bar {
  display: flex; align-items: center; justify-content: space-between;
  padding: 16px 24px; border-top: 1px solid #f1f5f9;
  flex-wrap: wrap; gap: 12px; background: #fafbfc;
}
.pagination-info {
  display: flex; align-items: center; gap: 6px;
  font-size: 12px; color: #64748b; font-weight: 600;
}
.pagination-info strong { color: #0f172a; font-weight: 900; }
.pagination-controls { display: flex; align-items: center; gap: 4px; }

.pg-btn {
  width: 34px; height: 34px; border-radius: 10px; border: 1.5px solid #e2e8f0;
  background: white; color: #64748b; cursor: pointer; font-size: 12px;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s; font-family: inherit;
}
.pg-btn:hover:not(:disabled) { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
.pg-btn:disabled              { opacity: 0.35; cursor: not-allowed; }

.pg-numbers { display: flex; align-items: center; gap: 3px; margin: 0 4px; }
.pg-num {
  min-width: 34px; height: 34px; padding: 0 6px;
  border-radius: 10px; border: 1.5px solid transparent;
  background: transparent; color: #64748b;
  cursor: pointer; font-size: 12px; font-weight: 800;
  display: flex; align-items: center; justify-content: center;
  transition: 0.2s; font-family: inherit;
}
.pg-num:hover:not(:disabled):not(.ellipsis) {
  background: #f1f5f9; border-color: #e2e8f0; color: #0f172a;
}
.pg-num.active {
  background: #0f172a; color: #f59e0b; border-color: #0f172a;
  box-shadow: 0 4px 12px rgba(15,23,42,0.18);
}
.pg-num.ellipsis { cursor: default; opacity: 0.5; }

.pagination-size { display: flex; align-items: center; gap: 8px; }
.pg-size-label   { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 0.5px; }
.pg-size-select  {
  padding: 6px 10px; border-radius: 10px; border: 1.5px solid #e2e8f0;
  background: white; font-size: 12px; font-weight: 800; color: #0f172a;
  cursor: pointer; outline: none; font-family: inherit;
}
.pg-size-select:focus { border-color: #f59e0b; }

/* ═══════════════════════ SPINNER ═══════════════════════ */
.spinner-pro-premium {
  width: 50px; height: 50px; border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b; border-radius: 50%;
  animation: spin 1s linear infinite; margin: 40px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ═══════════════════════ TOAST ═══════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 20px 28px; border-radius: 20px;
  display: flex; align-items: center; gap: 15px; z-index: 3000;
  border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.2);
  min-width: 300px;
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.t-ico     { font-size: 1.3rem; }
.t-body strong { font-size: 0.65rem; letter-spacing: 1.5px; opacity: 0.5; display: block; }
.t-body p      { color: #94a3b8; font-size: 0.82rem; }

/* ═══════════════════════ TRANSITIONS ═══════════════════════ */
.toast-slide-enter-active  { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn {
  from { transform: translateX(120%); opacity: 0; }
  to   { transform: translateX(0);    opacity: 1; }
}
.modal-quantum-enter-active { animation: zoomInM 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomInM 0.2s ease-in reverse; }
@keyframes zoomInM {
  from { opacity: 0; transform: scale(0.92); }
  to   { opacity: 1; transform: scale(1); }
}

/* ═══════════════════════ UTILITIES ═══════════════════════ */
.fw-700   { font-weight: 700 !important; }
.fw-800   { font-weight: 800 !important; }
.fw-900   { font-weight: 900 !important; }
.text-dark   { color: #0f172a !important; }
.text-muted  { color: #64748b !important; }
.text-danger { color: #f43f5e !important; }

/* ═══════════════════════ RESPONSIVE ═══════════════════════ */
@media (max-width: 991px) {
  .premium-title    { font-size: 1.7rem; }
  .pagination-bar   { justify-content: center; }
}
@media (max-width: 576px) {
  .enigma-toast     { left: 16px; right: 16px; bottom: 16px; min-width: unset; }
  .premium-title    { font-size: 1.4rem; }
  .pagination-info,
  .pagination-size  { display: none; }
}

/* ═══════════════════════ DARK MODE ═══════════════════════ */
[data-theme="dark"] .enigma-staff-root   { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title,
[data-theme="dark"] .stat-value          { color: #f0f6fc; }
[data-theme="dark"] .enigma-card,
[data-theme="dark"] .stat-card-premium,
[data-theme="dark"] .staff-grid-card     { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .table-header-elite,
[data-theme="dark"] .pagination-bar      { background: rgba(255,255,255,0.02); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .elite-table thead th { color: #64748b; }
[data-theme="dark"] .elite-table td      { border-top-color: rgba(255,255,255,0.04); }
[data-theme="dark"] .elite-table tbody tr:hover { background: rgba(255,255,255,0.03); }
[data-theme="dark"] .enigma-field        { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .enigma-field:focus  { border-color: #f59e0b; background: rgba(255,255,255,0.08); }
[data-theme="dark"] .enigma-field[readonly] { background: rgba(255,255,255,0.03); color: #64748b; }
[data-theme="dark"] .enigma-modal,
[data-theme="dark"] .confirm-modal       { background: #161b22; }
[data-theme="dark"] .enigma-modal h5,
[data-theme="dark"] .confirm-modal h5    { color: #f0f6fc; }
[data-theme="dark"] .modal-header-elite  { background: rgba(255,255,255,0.02); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .pg-btn              { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .pg-btn:hover:not(:disabled) { background: #f59e0b; border-color: #f59e0b; color: #0f172a; }
[data-theme="dark"] .pg-num              { color: #64748b; }
[data-theme="dark"] .pg-num:hover:not(:disabled):not(.ellipsis) { background: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .pg-num.active       { background: #f59e0b; color: #0f172a; border-color: #f59e0b; }
[data-theme="dark"] .pg-size-select      { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .filter-select-pro   { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-icon-sm         { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .grid-card-body h6   { color: #f0f6fc; }
[data-theme="dark"] .system-live-badge   { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .btn-refresh-pro     { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .view-toggle-cluster { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .btn-qv-cancel       { background: rgba(255,255,255,0.08); color: #94a3b8; }
[data-theme="dark"] .email-cell-text     { color: #8b949e; }
[data-theme="dark"] .date-cell           { color: #64748b; }
[data-theme="dark"] .empty-state-grid    { background: rgba(255,255,255,0.02); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .table-badge-count   { background: rgba(245,158,11,0.1); color: #f59e0b; }
</style>