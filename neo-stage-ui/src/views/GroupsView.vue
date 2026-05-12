<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar" @scroll="handleScroll">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- ═══ HEADER ═══ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">{{ t('groupeInvite.breadcrumb') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ t('groupeInvite.titleHighlight') }}</span>
              </div>
              <h2 class="premium-title">
                {{ t('groupeInvite.title') }}
                <span class="gradient-text">{{ t('groupeInvite.titleHighlight') }}</span>
              </h2>
              <p class="page-subtitle">{{ t('groupeInvite.subtitle') }}</p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <button class="btn-theme-toggle" @click="toggleTheme" :title="isDark ? t('theme.light') : t('theme.dark')">
                <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
              </button>
              <div class="stats-card-mini">
                <div class="stat-icon-amber">
                  <i class="fa-solid fa-users-viewfinder"></i>
                </div>
                <div class="text-start">
                  <div class="stat-val">{{ emailList.length }}</div>
                  <div class="stat-label">{{ t('groupeInvite.pendingCandidates') }}</div>
                </div>
              </div>
            </div>
          </header>

          <!-- ═══ TOAST ═══ -->
          <transition name="toast-slide">
            <div v-if="statusMsg" :class="['enigma-toast', statusType === 'success' ? 't-success' : 't-error']">
              <div class="t-ico">
                <i class="fa-solid" :class="statusType === 'success' ? 'fa-check' : 'fa-exclamation-triangle'"></i>
              </div>
              <div class="t-body">
                <strong>{{ statusType === 'success' ? t('groupeInvite.toastSuccess') : t('groupeInvite.toastWarning') }}</strong>
                <p class="m-0 small">{{ statusMsg }}</p>
              </div>
            </div>
          </transition>

          <div class="row g-5">
            <!-- ═══ COLONNE PRINCIPALE ═══ -->
            <div class="col-lg-8">

              <!-- ÉTAPE 1 : CAMPAGNE -->
              <section class="enigma-card p-5 mb-4">
                <div class="pane-header-v2 mb-5">
                  <div class="icon-box-v2 amber"><i class="fa-solid fa-layer-group"></i></div>
                  <div>
                    <h4 class="fw-900 m-0">{{ t('groupeInvite.campaignSection') }}</h4>
                    <p class="text-muted m-0 small">{{ t('groupeInvite.campaignSubtitle') }}</p>
                  </div>
                  <span class="section-tag-mini ms-auto">{{ t('required') }}</span>
                </div>
                <div class="enigma-input-wrap">
                  <label>{{ t('groupeInvite.campaignLabel') }}</label>
                  <div class="theme-select-wrapper d-flex gap-3">
                    <div class="position-relative flex-grow-1">
                      <i class="fa-solid fa-layer-group theme-select-icon"></i>
                      <select v-model="selectedCampagneId" class="enigma-field theme-select">
                        <option value="">{{ t('groupeInvite.campaignPlaceholder') }}</option>
                        <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                      </select>
                    </div>
                    <button class="btn-enigma-primary" @click="router.push('/campaigns')" :title="t('groupeInvite.createCampaign')" style="padding:0 20px;border-radius:16px;">
                      <div class="btn-content"><i class="fa-solid fa-plus"></i></div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>
                </div>
              </section>

              <!-- ÉTAPE 2 : IMPORTATION -->
              <section class="enigma-card p-5 mb-4">
                <div class="pane-header-v2 mb-5">
                  <div class="icon-box-v2 amber"><i class="fa-solid fa-user-plus"></i></div>
                  <div>
                    <h4 class="fw-900 m-0">{{ t('groupeInvite.importSection') }}</h4>
                    <p class="text-muted m-0 small">{{ t('groupeInvite.importSubtitle') }}</p>
                  </div>
                </div>

                <!-- TABS -->
                <div class="premium-tabs mb-4">
                  <button @click="activeTab = 'unique'" :class="{ active: activeTab === 'unique' }">
                    <i class="fa-solid fa-user-plus me-2"></i>{{ t('groupeInvite.tabUnique') }}
                  </button>
                  <button @click="activeTab = 'multiple'" :class="{ active: activeTab === 'multiple' }">
                    <i class="fa-solid fa-list-check me-2"></i>{{ t('groupeInvite.tabBulk') }}
                  </button>
                  <button @click="activeTab = 'csv'" :class="{ active: activeTab === 'csv' }">
                    <i class="fa-solid fa-file-csv me-2"></i>{{ t('groupeInvite.tabCsv') }}
                  </button>
                </div>

                <!-- UNIQUE -->
                <div v-if="activeTab === 'unique'" class="fade-in-quick">
                  <div class="enigma-input-wrap">
                    <label>{{ t('groupeInvite.emailLabel') }}</label>
                    <div class="d-flex gap-3">
                      <input
                        type="email"
                        v-model="currentEmail"
                        @keyup.enter="addEmail"
                        :placeholder="t('groupeInvite.emailPlaceholder')"
                        class="enigma-field"
                        style="flex:1"
                      >
                      <button @click="addEmail" class="btn-enigma-primary" style="padding:0 28px;border-radius:16px;">
                        <div class="btn-content">{{ t('groupeInvite.addBtn') }}</div>
                        <div class="btn-glow"></div>
                      </button>
                    </div>
                  </div>
                </div>

                <!-- BULK -->
                <div v-if="activeTab === 'multiple'" class="fade-in-quick">
                  <div class="enigma-input-wrap">
                    <label>{{ t('groupeInvite.bulkLabel') }}</label>
                    <textarea
                      v-model="bulkEmails"
                      class="enigma-field"
                      rows="4"
                      :placeholder="t('groupeInvite.bulkPlaceholder')"
                    ></textarea>
                  </div>
                  <button @click="processBulkEmails" class="btn-enigma-primary mt-3 w-100" style="border-radius:16px;padding:14px;">
                    <div class="btn-content justify-content-center">
                      <i class="fa-solid fa-bolt me-2"></i>{{ t('groupeInvite.bulkProcess') }}
                    </div>
                    <div class="btn-glow"></div>
                  </button>
                </div>

                <!-- CSV -->
                <div v-if="activeTab === 'csv'" class="fade-in-quick">
                  <div class="csv-upload-zone" @click="$refs.fileInput.click()">
                    <div class="upload-icon-wrap">
                      <i class="fa-solid fa-cloud-arrow-up"></i>
                    </div>
                    <h5 class="fw-800 mt-3 mb-1">{{ t('groupeInvite.csvTitle') }}</h5>
                    <p class="text-muted small m-0">{{ t('groupeInvite.csvSubtitle') }}</p>
                    <input type="file" class="d-none" ref="fileInput" accept=".csv" @change="handleFileUpload">
                  </div>
                </div>

                <!-- LISTE EMAILS -->
                <div v-if="emailList.length > 0" class="mt-5 fade-in">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <h6 class="fw-900 m-0">
                      <i class="fa-solid fa-clipboard-list text-amber me-2"></i>
                      {{ t('groupeInvite.queueTitle') }} ({{ emailList.length }})
                    </h6>
                    <button @click="emailList = []; currentPage = 1" class="btn-clear-all">{{ t('reset') }}</button>
                  </div>
                  <div class="pills-grid">
                    <span v-for="(mail, idx) in paginatedEmails" :key="idx" class="premium-pill">
                      <span class="pill-text">{{ mail }}</span>
                      <i @click="removeEmail(emailList.indexOf(mail))" class="fa-solid fa-xmark pill-close"></i>
                    </span>
                  </div>

                  <div v-if="totalPages > 1" class="pagination-bar mt-4">
                    <button class="page-btn" @click="currentPage = 1" :disabled="currentPage === 1"><i class="fa-solid fa-angles-left"></i></button>
                    <button class="page-btn" @click="currentPage--" :disabled="currentPage === 1"><i class="fa-solid fa-angle-left"></i></button>
                    <template v-for="page in visiblePages" :key="page">
                      <span v-if="page === '...'" class="page-ellipsis">…</span>
                      <button v-else class="page-btn" :class="{ active: currentPage === page }" @click="currentPage = page">{{ page }}</button>
                    </template>
                    <button class="page-btn" @click="currentPage++" :disabled="currentPage === totalPages"><i class="fa-solid fa-angle-right"></i></button>
                    <button class="page-btn" @click="currentPage = totalPages" :disabled="currentPage === totalPages"><i class="fa-solid fa-angles-right"></i></button>
                    <span class="page-info">Page {{ currentPage }} / {{ totalPages }} <span class="text-muted ms-2">({{ emailList.length }} emails)</span></span>
                  </div>
                </div>
              </section>

              <!-- BOUTON DEPLOY -->
              <div class="action-footer mt-2 text-end">
                <button
                  @click="deployInvitations"
                  :disabled="isLoading || !selectedCampagneId || emailList.length === 0"
                  class="btn-enigma-primary shadow-premium"
                  style="padding:18px 45px;border-radius:18px;font-size:16px;"
                >
                  <div class="btn-content">
                    <span v-if="isLoading" class="d-flex align-items-center gap-2">
                      <div class="spinner-border spinner-border-sm" role="status"></div>
                      {{ t('groupeInvite.deploying') }}
                    </span>
                    <span v-else class="d-flex align-items-center gap-2">
                      {{ t('groupeInvite.deployBtn') }} <i class="fa-solid fa-paper-plane ms-2"></i>
                    </span>
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>
            </div>

            <!-- ═══ SIDEBAR HUD ═══ -->
            <div class="col-lg-4">
              <aside :class="['sticky-info', { 'is-scrolled': isScrolled }]">
                <div class="analytics-hub-glass">
                  <div class="hub-header-v2 mb-4">
                    <span class="hub-label">{{ t('groupeInvite.protocol') }}</span>
                    <h4 class="hub-title-v2">{{ t('groupeInvite.securedDeploy') }}</h4>
                    <div class="hub-status-box cl-success mt-2">
                      <span class="pulse-dot"></span> {{ t('groupeInvite.systemActive') }}
                    </div>
                  </div>

                  <div class="kpi-bento-grid mb-4">
                    <div class="bento-item">
                      <span class="v">{{ emailList.length }}</span>
                      <span class="l">{{ t('groupeInvite.emailsKpi') }}</span>
                    </div>
                    <div class="bento-item highlight">
                      <span class="v">{{ selectedCampagneId ? '1' : '0' }}</span>
                      <span class="l">{{ t('groupeInvite.campaignKpi') }}</span>
                    </div>
                    <div class="bento-item">
                      <span class="v">{{ campagnes.length }}</span>
                      <span class="l">{{ t('groupeInvite.availableKpi') }}</span>
                    </div>
                    <div class="bento-item">
                      <span class="v">{{ isLoading ? '...' : '✓' }}</span>
                      <span class="l">{{ t('groupeInvite.statusKpi') }}</span>
                    </div>
                  </div>

                  <div class="readiness-checklist">
                    <div class="checklist-label mb-2">{{ t('groupeInvite.checklistTitle') }}</div>
                    <div class="check-item" :class="{ passed: !!selectedCampagneId }">
                      <i :class="selectedCampagneId ? 'fa-solid fa-check-circle text-success' : 'fa-regular fa-circle text-muted'"></i>
                      <span>{{ t('groupeInvite.checkCampaign') }}</span>
                    </div>
                    <div class="check-item" :class="{ passed: emailList.length > 0 }">
                      <i :class="emailList.length > 0 ? 'fa-solid fa-check-circle text-success' : 'fa-regular fa-circle text-muted'"></i>
                      <span>{{ t('groupeInvite.checkEmail') }}</span>
                    </div>
                    <div class="check-item" :class="{ passed: emailList.length >= 5 }">
                      <i :class="emailList.length >= 5 ? 'fa-solid fa-check-circle text-success' : 'fa-regular fa-circle text-muted'"></i>
                      <span>{{ t('groupeInvite.checkMinFive') }}</span>
                    </div>
                    <div class="check-item passed">
                      <i class="fa-solid fa-check-circle text-success"></i>
                      <span>{{ t('groupeInvite.checkEncrypted') }}</span>
                    </div>
                  </div>
                </div>

                <!-- GUIDE -->
                <div class="enigma-card p-4 mt-4">
                  <div class="hub-label mb-3">{{ t('groupeInvite.secureProtocol') }}</div>
                  <div class="guide-item">
                    <div class="guide-dot"></div>
                    <p class="small m-0" v-html="t('groupeInvite.guideUnique')"></p>
                  </div>
                  <div class="guide-item mt-3">
                    <div class="guide-dot"></div>
                    <p class="small m-0" v-html="t('groupeInvite.guideProctor')"></p>
                  </div>
                  <div class="guide-item mt-3">
                    <div class="guide-dot"></div>
                    <p class="small m-0" v-html="t('groupeInvite.guideDashboard')"></p>
                  </div>
                  <div class="alert-premium mt-4">
                    <i class="fa-solid fa-circle-info text-amber"></i>
                    <span class="small ms-2">{{ t('groupeInvite.alertValidity') }}</span>
                  </div>
                </div>

                <!-- TIP -->
                <div class="tip-card mt-4">
                  <div class="tip-icon"><i class="fa-regular fa-lightbulb"></i></div>
                  <div class="tip-content">
                    <h6 class="fw-800 mb-1" style="font-size:0.85rem;color:#854d0e;">{{ t('groupeInvite.tipTitle') }}</h6>
                    <p class="m-0 small" style="color:#a16207;">{{ t('groupeInvite.tipContent') }}</p>
                  </div>
                </div>
              </aside>
            </div>
          </div>

          <!-- ═══ HISTORIQUE ═══ -->
          <section class="enigma-card p-5 mt-5">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
              <div class="pane-header-v2">
                <div class="icon-box-v2 amber"><i class="fa-solid fa-history"></i></div>
                <div>
                  <h4 class="fw-900 m-0">{{ t('groupeInvite.historyTitle') }}</h4>
                  <p class="text-muted m-0 small">{{ filteredHistory.length }} {{ t('groupeInvite.historyFound') }}</p>
                </div>
              </div>
              <div class="d-flex gap-2 flex-wrap">
                <div class="search-inline-box">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  <input type="text" v-model="historySearch" :placeholder="t('groupeInvite.historySearchPlaceholder')" class="search-inline-input">
                  <button v-if="historySearch" @click="historySearch = ''" class="btn-clear-search"><i class="fa-solid fa-xmark"></i></button>
                </div>
                <select v-model="historyStatusFilter" class="enigma-field" style="width:auto;padding:10px 16px;">
                  <option value="">{{ t('groupeInvite.historyFilterAll') }}</option>
                  <option value="pending">{{ t('groupeInvite.statusPending') }}</option>
                  <option value="sent">{{ t('groupeInvite.statusSent') }}</option>
                  <option value="opened">{{ t('groupeInvite.statusOpened') }}</option>
                  <option value="completed">{{ t('groupeInvite.statusCompleted') }}</option>
                </select>
              </div>
            </div>

            <div class="table-responsive">
              <table class="history-table w-100">
                <thead>
                  <tr>
                    <th>{{ t('groupeInvite.colEmail') }}</th>
                    <th>{{ t('groupeInvite.colCampaign') }}</th>
                    <th>{{ t('groupeInvite.colDate') }}</th>
                    <th>{{ t('groupeInvite.colStatus') }}</th>
                    <th class="text-center">{{ t('groupeInvite.colActions') }}</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="paginatedHistory.length === 0">
                    <td colspan="5" class="text-center py-5 text-muted">
                      <i class="fa-solid fa-inbox fa-2x mb-3 d-block"></i>
                      {{ t('groupeInvite.historyEmpty') }}
                    </td>
                  </tr>
                  <tr v-for="(inv, i) in paginatedHistory" :key="i">
                    <td>
                      <div class="d-flex align-items-center gap-3">
                        <div class="avatar-mini">{{ inv.email[0].toUpperCase() }}</div>
                        <span class="fw-700 small">{{ inv.email }}</span>
                      </div>
                    </td>
                    <td><span class="small text-muted fw-600">{{ inv.campagne }}</span></td>
                    <td><span class="small text-muted">{{ inv.date }}</span></td>
                    <td>
                      <span class="status-badge" :class="'status-inv-' + inv.statut">
                        <span class="status-dot"></span>
                        {{ getStatusLabel(inv.statut) }}
                      </span>
                    </td>
                    <td class="text-center">
                      <button class="btn-icon-sm me-1" :title="t('groupeInvite.historyResend')" @click="resendInvitation(inv)">
                        <i class="fa-solid fa-rotate-right"></i>
                      </button>
                      <button class="btn-icon-sm danger" :title="t('groupeInvite.historyDelete')" @click="deleteInvitation(i)">
                        <i class="fa-solid fa-trash-can"></i>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div v-if="totalHistoryPages > 1" class="pagination-bar mt-4">
              <button class="page-btn" @click="historyPage = 1" :disabled="historyPage === 1"><i class="fa-solid fa-angles-left"></i></button>
              <button class="page-btn" @click="historyPage--" :disabled="historyPage === 1"><i class="fa-solid fa-angle-left"></i></button>
              <template v-for="page in visibleHistoryPages" :key="'h' + page">
                <span v-if="page === '...'" class="page-ellipsis">…</span>
                <button v-else class="page-btn" :class="{ active: historyPage === page }" @click="historyPage = page">{{ page }}</button>
              </template>
              <button class="page-btn" @click="historyPage++" :disabled="historyPage === totalHistoryPages"><i class="fa-solid fa-angle-right"></i></button>
              <button class="page-btn" @click="historyPage = totalHistoryPages" :disabled="historyPage === totalHistoryPages"><i class="fa-solid fa-angles-right"></i></button>
              <span class="page-info">Page {{ historyPage }} / {{ totalHistoryPages }} <span class="text-muted ms-2">({{ filteredHistory.length }} {{ t('actions') }})</span></span>
            </div>
          </section>

        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import { useRouter } from 'vue-router';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const { t } = useI18n();
const router = useRouter();
const API_URL = '/Invitations';

/* ─── STATE ─────────────────────────────────────── */
const activeTab          = ref('unique');
const campagnes          = ref([]);
const selectedCampagneId = ref('');
const emailList          = ref([]);
const currentEmail       = ref('');
const bulkEmails         = ref('');
const isLoading          = ref(false);
const statusMsg          = ref('');
const statusType         = ref('success');
const fileInput          = ref(null);
const isScrolled         = ref(false);
const isDark             = ref(false);
const mousePos           = { x: 0, y: 0 };

/* ─── PAGINATION EMAILS ─────────────────────────── */
const currentPage  = ref(1);
const itemsPerPage = ref(20);

const totalPages = computed(() => Math.ceil(emailList.value.length / itemsPerPage.value));
const paginatedEmails = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  return emailList.value.slice(start, start + itemsPerPage.value);
});
const visiblePages = computed(() => buildPageRange(currentPage.value, totalPages.value));

/* ─── HISTORIQUE ────────────────────────────────── */
const historySearch       = ref('');
const historyStatusFilter = ref('');
const historyPage         = ref(1);
const histPerPage         = 8;
const invitationHistory   = ref([]);

const filteredHistory = computed(() => {
  let list = [...invitationHistory.value];
  if (historySearch.value)
    list = list.filter(i => i.email.toLowerCase().includes(historySearch.value.toLowerCase()));
  if (historyStatusFilter.value)
    list = list.filter(i => i.statut === historyStatusFilter.value);
  return list;
});
const totalHistoryPages  = computed(() => Math.ceil(filteredHistory.value.length / histPerPage));
const paginatedHistory   = computed(() => {
  const start = (historyPage.value - 1) * histPerPage;
  return filteredHistory.value.slice(start, start + histPerPage);
});
const visibleHistoryPages = computed(() => buildPageRange(historyPage.value, totalHistoryPages.value));

/* ─── HELPERS ───────────────────────────────────── */
function buildPageRange(current, total) {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
  const pages = [];
  if (current <= 4)           pages.push(1, 2, 3, 4, 5, '...', total);
  else if (current >= total - 3) pages.push(1, '...', total - 4, total - 3, total - 2, total - 1, total);
  else                        pages.push(1, '...', current - 1, current, current + 1, '...', total);
  return pages;
}

/* ─── EMAIL ACTIONS ─────────────────────────────── */
const addEmail = () => {
  const mail = currentEmail.value.trim().toLowerCase();
  if (mail && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(mail)) {
    if (!emailList.value.includes(mail)) {
      emailList.value.push(mail);
      currentPage.value = totalPages.value;
    }
    currentEmail.value = '';
  } else {
    showStatus(t('groupeInvite.invalidEmail'), 'error');
  }
};

const processBulkEmails = () => {
  const extracted = bulkEmails.value.split(/[\s,\n;]+/).map(e => e.trim().toLowerCase());
  let added = 0;
  extracted.forEach(e => {
    if (e && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(e) && !emailList.value.includes(e)) {
      emailList.value.push(e);
      added++;
    }
  });
  bulkEmails.value = '';
  if (added > 0) showStatus(t('groupeInvite.bulkAdded', { n: added }), 'success');
};

const handleFileUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;
  const reader = new FileReader();
  reader.onload = (e) => {
    const found = e.target.result.match(/[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g);
    if (found) {
      let added = 0;
      found.forEach(m => {
        if (!emailList.value.includes(m.toLowerCase())) { emailList.value.push(m.toLowerCase()); added++; }
      });
      showStatus(t('groupeInvite.csvImported', { n: added }), 'success');
    }
  };
  reader.readAsText(file);
};

const removeEmail = (index) => {
  emailList.value.splice(index, 1);
  if (currentPage.value > totalPages.value && totalPages.value > 0) currentPage.value = totalPages.value;
};

const resendInvitation = (inv) => showStatus(t('groupeInvite.historyResendMsg', { email: inv.email }), 'success');
const deleteInvitation  = (i)   => { invitationHistory.value.splice(i, 1); };

const showStatus = (msg, type) => {
  statusMsg.value  = msg;
  statusType.value = type;
  setTimeout(() => statusMsg.value = '', 4000);
};

const getStatusLabel = (s) => ({
  pending:   t('groupeInvite.statusPending'),
  sent:      t('groupeInvite.statusSent'),
  opened:    t('groupeInvite.statusOpened'),
  completed: t('groupeInvite.statusCompleted'),
}[s] ?? s);

/* ─── DEPLOY ────────────────────────────────────── */
const deployInvitations = async () => {
  isLoading.value = true;
  try {
    await api.post(`${API_URL}/invite-candidates`, {
      campagneId: selectedCampagneId.value,
      emails: emailList.value,
    });
    const campagneName = campagnes.value.find(c => c.id === selectedCampagneId.value)?.nom || 'Campagne';
    emailList.value.forEach(email => {
      invitationHistory.value.unshift({
        email,
        campagne: campagneName,
        date: new Date().toLocaleDateString(),
        statut: 'sent',
      });
    });
    showStatus(t('groupeInvite.deploySuccess', { n: emailList.value.length }), 'success');
    emailList.value = [];
    selectedCampagneId.value = '';
    currentPage.value = 1;
  } catch {
    showStatus(t('groupeInvite.deployError'), 'error');
  } finally {
    isLoading.value = false;
  }
};

/* ─── THEME ─────────────────────────────────────── */
const toggleTheme = () => {
  isDark.value = !isDark.value;
  document.documentElement.setAttribute('data-theme', isDark.value ? 'dark' : 'light');
};

/* ─── PARALLAX ──────────────────────────────────── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => { mousePos.x = (e.clientX - window.innerWidth / 2) / 20; mousePos.y = (e.clientY - window.innerHeight / 2) / 20; };
const handleScroll   = (e) => { isScrolled.value = e.target.scrollTop > 50; };

/* ─── LIFECYCLE ─────────────────────────────────── */
onMounted(async () => {
  try {
    const res = await api.get(`${API_URL}/campagnes`);
    campagnes.value = res.data;
  } catch {
    campagnes.value = [
      { id: '1', nom: 'Frontend Senior Audit' },
      { id: '2', nom: 'Backend Node.js Test' },
      { id: '3', nom: 'SQL Data Engineer' },
    ];
  }
});
</script>


<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800;900&display=swap');

/* ═══════════════════════════════════════════
   BASE LAYOUT
═══════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  transition: background 0.3s, color 0.3s;
}

/* ═══════════════════════════════════════════
   BACKGROUND
═══════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.glow-orb { position: absolute; width: 600px; height: 600px; filter: blur(120px); opacity: 0.15; border-radius: 50%; transition: transform 0.3s ease-out; }
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ═══════════════════════════════════════════
   HEADER
═══════════════════════════════════════════ */
.premium-title { font-weight: 900; font-size: 2.2rem; letter-spacing: -1px; margin: 0; }
.gradient-text { background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.page-subtitle { color: #64748b; font-size: 1rem; margin: 6px 0 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ═══════════════════════════════════════════
   THEME TOGGLE
═══════════════════════════════════════════ */
.btn-theme-toggle {
  width: 44px; height: 44px; border-radius: 14px;
  border: 1.5px solid #e2e8f0; background: white;
  cursor: pointer; color: #64748b; font-size: 1rem;
  display: flex; align-items: center; justify-content: center;
  transition: 0.3s;
}
.btn-theme-toggle:hover { background: #0f172a; color: #f59e0b; border-color: #0f172a; }

/* ═══════════════════════════════════════════
   STATS MINI
═══════════════════════════════════════════ */
.stats-card-mini {
  background: white; border-radius: 20px; padding: 14px 22px;
  display: inline-flex; align-items: center; gap: 14px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.04);
  border: 1px solid #f1f5f9;
}
.stat-icon-amber {
  width: 46px; height: 46px; background: #fefce8; color: #eab308;
  border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 18px;
}
.stat-val { font-size: 22px; font-weight: 900; line-height: 1; color: #0f172a; }
.stat-label { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 3px; }

/* ═══════════════════════════════════════════
   CARDS / FORMS
═══════════════════════════════════════════ */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; transition: border-color 0.2s; }
.enigma-card:hover { border-color: #e2e8f0; }

.pane-header-v2 { display: flex; align-items: center; gap: 18px; }
.icon-box-v2 { width: 52px; height: 52px; border-radius: 18px; display: flex; align-items: center; justify-content: center; font-size: 1.3rem; flex-shrink: 0; }
.icon-box-v2.amber { background: #fffbeb; color: #f59e0b; }

.enigma-input-wrap label {
  font-size: 0.6rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 1px; margin-bottom: 8px; display: block;
}
.enigma-field {
  width: 100%; padding: 14px 20px; background: #f8fafc;
  border: 2px solid #eef2f6; border-radius: 16px;
  font-weight: 700; outline: none;
  font-family: 'Plus Jakarta Sans', sans-serif;
  transition: border-color 0.2s, background 0.2s;
  font-size: 0.9rem; color: #0f172a;
}
.enigma-field:focus { border-color: #f59e0b; background: white; }

.theme-select-wrapper { position: relative; }
.theme-select-icon {
  position: absolute; left: 16px; top: 50%;
  transform: translateY(-50%); color: #f59e0b;
  font-size: 0.75rem; z-index: 2; pointer-events: none;
}
.theme-select { padding-left: 40px !important; appearance: none; -webkit-appearance: none; cursor: pointer; }

.section-tag-mini {
  font-size: 10px; font-weight: 800; color: #64748b; background: #f1f5f9;
  padding: 4px 10px; border-radius: 6px; text-transform: uppercase; white-space: nowrap;
}

/* ═══════════════════════════════════════════
   TABS
═══════════════════════════════════════════ */
.premium-tabs { display: flex; background: #f1f5f9; border-radius: 14px; padding: 6px; gap: 6px; }
.premium-tabs button {
  flex: 1; border: none; padding: 12px 10px; border-radius: 10px;
  font-weight: 700; font-size: 0.82rem; color: #64748b;
  background: transparent; transition: 0.3s; cursor: pointer;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.premium-tabs button.active {
  background: white; color: #0f172a;
  box-shadow: 0 4px 15px rgba(0,0,0,0.05);
}

/* ═══════════════════════════════════════════
   CSV ZONE
═══════════════════════════════════════════ */
.csv-upload-zone {
  border: 2px dashed #e2e8f0; padding: 40px 20px;
  border-radius: 24px; background: #f8fafc;
  cursor: pointer; text-align: center; transition: 0.3s;
}
.csv-upload-zone:hover { border-color: #eab308; background: #fefce8; }
.upload-icon-wrap {
  width: 64px; height: 64px; background: white; color: #eab308;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  margin: 0 auto; font-size: 24px;
  box-shadow: 0 10px 20px rgba(0,0,0,0.06);
}

/* ═══════════════════════════════════════════
   PILLS
═══════════════════════════════════════════ */
.pills-grid { display: flex; flex-wrap: wrap; gap: 10px; }
.premium-pill {
  background: #0f172a; color: white; padding: 8px 16px;
  border-radius: 100px; font-size: 13px; font-weight: 600;
  display: flex; align-items: center; gap: 10px;
  animation: scaleIn 0.3s ease;
}
.pill-close { cursor: pointer; color: #f59e0b; transition: 0.2s; font-size: 12px; }
.pill-close:hover { transform: scale(1.2); color: white; }
.btn-clear-all {
  border: none; background: transparent; color: #ef4444;
  font-size: 12px; font-weight: 700; text-transform: uppercase; cursor: pointer;
}

/* ═══════════════════════════════════════════
   PAGINATION
═══════════════════════════════════════════ */
.pagination-bar {
  display: flex; align-items: center; gap: 6px; flex-wrap: wrap;
}
.page-btn {
  min-width: 36px; height: 36px; padding: 0 10px;
  border-radius: 10px; border: 1.5px solid #e2e8f0;
  background: white; color: #64748b; font-weight: 700;
  font-size: 0.82rem; cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.page-btn:hover:not(:disabled) { border-color: #f59e0b; color: #f59e0b; background: #fffbeb; }
.page-btn.active { background: #0f172a; color: white; border-color: #0f172a; }
.page-btn:disabled { opacity: 0.35; cursor: not-allowed; }
.page-ellipsis { color: #94a3b8; font-weight: 700; font-size: 0.85rem; padding: 0 6px; }
.page-info { font-size: 0.75rem; font-weight: 700; color: #64748b; margin-left: 8px; white-space: nowrap; }

/* ═══════════════════════════════════════════
   SIDEBAR / HUD
═══════════════════════════════════════════ */
.sticky-info { position: sticky; top: 20px; transition: top 0.3s; }
.sticky-info.is-scrolled { top: 90px; }

.analytics-hub-glass { background: #0f172a; color: white; border-radius: 32px; padding: 32px; }
.hub-label { font-size: 0.55rem; font-weight: 900; opacity: 0.4; letter-spacing: 2px; text-transform: uppercase; }
.hub-title-v2 { font-size: 1.15rem; font-weight: 900; margin-top: 8px; margin-bottom: 4px; }
.hub-status-box { font-size: 0.6rem; font-weight: 800; padding: 6px 12px; border-radius: 10px; display: inline-flex; align-items: center; gap: 8px; }
.cl-success { background: rgba(16,185,129,0.1); color: #10b981; }
.pulse-dot { width: 6px; height: 6px; background: currentColor; border-radius: 50%; animation: pulse 2s infinite; }
@keyframes pulse { 0%,100% { opacity: 1; } 50% { opacity: 0.3; } }

.kpi-bento-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.bento-item { background: rgba(255,255,255,0.05); padding: 16px; border-radius: 16px; text-align: center; }
.bento-item.highlight { background: rgba(245,158,11,0.15); }
.bento-item .v { font-size: 1.7rem; font-weight: 900; display: block; line-height: 1; color: white; }
.bento-item .l { font-size: 0.55rem; opacity: 0.5; text-transform: uppercase; letter-spacing: 1px; margin-top: 4px; display: block; }

.readiness-checklist { background: rgba(255,255,255,0.05); border-radius: 14px; padding: 16px; }
.checklist-label { font-size: 0.55rem; font-weight: 900; opacity: 0.4; letter-spacing: 1px; text-transform: uppercase; }
.check-item { display: flex; align-items: center; gap: 10px; font-size: 0.75rem; padding: 6px 0; opacity: 0.4; transition: 0.3s; color: white; }
.check-item.passed { opacity: 1; }

.guide-item { display: flex; align-items: flex-start; gap: 10px; }
.guide-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; margin-top: 7px; flex-shrink: 0; }

.alert-premium {
  background: #f8fafc; border-radius: 14px; padding: 14px;
  display: flex; align-items: flex-start; gap: 10px;
  border: 1px solid #eef2f6;
}

.tip-card {
  background: #fefce8; border: 1px solid #fde68a; border-radius: 20px;
  padding: 20px; display: flex; gap: 14px; align-items: flex-start;
}
.tip-icon { font-size: 22px; color: #eab308; flex-shrink: 0; }

/* ═══════════════════════════════════════════
   HISTORY TABLE
═══════════════════════════════════════════ */
.history-table { border-collapse: separate; border-spacing: 0 8px; }
.history-table thead th {
  font-size: 0.6rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 1px; text-transform: uppercase;
  padding: 10px 16px; border: none;
}
.history-table tbody tr {
  background: white; border-radius: 16px;
  transition: 0.2s;
}
.history-table tbody tr:hover td { background: #fafbfc; }
.history-table tbody td {
  padding: 14px 16px; border-top: 1px solid #eef2f6;
  border-bottom: 1px solid #eef2f6;
  transition: background 0.2s;
}
.history-table tbody td:first-child { border-left: 1px solid #eef2f6; border-radius: 16px 0 0 16px; }
.history-table tbody td:last-child  { border-right: 1px solid #eef2f6; border-radius: 0 16px 16px 0; }

.avatar-mini {
  width: 34px; height: 34px; border-radius: 10px;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  color: #0f172a; font-weight: 900; font-size: 0.9rem;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}

.status-badge {
  padding: 4px 12px; border-radius: 10px; font-size: 0.62rem;
  font-weight: 800; text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-dot { width: 5px; height: 5px; border-radius: 50%; background: currentColor; margin-right: 6px; }
.status-inv-pending   { background: #f0f9ff; color: #6366f1; }
.status-inv-sent      { background: #fffbeb; color: #d97706; }
.status-inv-opened    { background: #eff6ff; color: #3b82f6; }
.status-inv-completed { background: #ecfdf5; color: #10b981; }

/* ═══════════════════════════════════════════
   BUTTONS
═══════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; justify-content: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white; color: #64748b;
  cursor: pointer; transition: 0.2s; font-size: 0.75rem;
  display: inline-flex; align-items: center; justify-content: center;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }
.btn-icon-sm.danger:hover { background: #fff1f2; color: #f43f5e; border-color: #f43f5e; }

.search-inline-box {
  display: flex; align-items: center; background: white;
  border: 1.5px solid #eef2f6; border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8;
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 10px 0; font-weight: 700; font-size: 0.85rem;
  width: 180px; font-family: 'Plus Jakarta Sans', sans-serif;
}
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }

.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

/* ═══════════════════════════════════════════
   TOAST
═══════════════════════════════════════════ */
.enigma-toast {
  position: fixed; top: 30px; right: 30px; background: #0f172a; color: white;
  padding: 18px 28px; border-radius: 20px; display: flex; align-items: center; gap: 14px;
  z-index: 3000; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
  min-width: 300px;
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-ico { font-size: 1.1rem; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* ═══════════════════════════════════════════
   ANIMATIONS
═══════════════════════════════════════════ */
@keyframes scaleIn { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }
.fade-in-quick { animation: fadeInQ 0.3s ease; }
@keyframes fadeInQ { from { opacity: 0; } to { opacity: 1; } }
.text-amber  { color: #f59e0b !important; }
.text-muted  { color: #64748b !important; }
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }

/* ═══════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine { background: #0d1117; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .page-subtitle { color: #8b949e; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

[data-theme="dark"] .stats-card-mini { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .stat-val { color: #f0f6fc; }
[data-theme="dark"] .btn-theme-toggle { background: #161b22; border-color: rgba(255,255,255,0.08); color: #f0f6fc; }

[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .enigma-field {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.1);
  color: #f0f6fc;
}
[data-theme="dark"] .enigma-field:focus { border-color: #d97706; background: rgba(255,255,255,0.08); }
[data-theme="dark"] select.enigma-field option { background: #161b22; color: #f0f6fc; }

[data-theme="dark"] .premium-tabs { background: rgba(255,255,255,0.05); }
[data-theme="dark"] .premium-tabs button { color: #8b949e; }
[data-theme="dark"] .premium-tabs button.active { background: #0d1117; color: #f0f6fc; }

[data-theme="dark"] .csv-upload-zone { border-color: rgba(255,255,255,0.1); background: rgba(255,255,255,0.03); }
[data-theme="dark"] .csv-upload-zone h5 { color: #f0f6fc; }

[data-theme="dark"] .premium-pill { background: rgba(255,255,255,0.1); }

[data-theme="dark"] .alert-premium { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .tip-card { background: rgba(245,158,11,0.08); border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .tip-content h6 { color: #fbbf24; }
[data-theme="dark"] .tip-content p  { color: #d97706; }

[data-theme="dark"] .history-table thead th { color: #8b949e; }
[data-theme="dark"] .history-table tbody tr { background: #161b22; }
[data-theme="dark"] .history-table tbody tr:hover td { background: rgba(255,255,255,0.03); }
[data-theme="dark"] .history-table tbody td { border-color: rgba(255,255,255,0.06); color: #f0f6fc; }

[data-theme="dark"] .search-inline-box { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] select.enigma-field { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }

[data-theme="dark"] .page-btn { background: #161b22; border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .page-btn:hover:not(:disabled) { border-color: #d97706; color: #f59e0b; background: rgba(245,158,11,0.1); }
[data-theme="dark"] .page-btn.active { background: #f59e0b; color: #0f172a; border-color: #f59e0b; }
[data-theme="dark"] .page-info { color: #8b949e; }
[data-theme="dark"] .page-ellipsis { color: #8b949e; }

[data-theme="dark"] .btn-icon-sm { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .btn-icon-sm:hover { background: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-icon-sm.danger:hover { background: rgba(244,63,94,0.1); color: #f43f5e; border-color: #f43f5e; }

[data-theme="dark"] .section-tag-mini { background: rgba(255,255,255,0.08); color: #8b949e; }
[data-theme="dark"] .text-muted { color: #8b949e !important; }
[data-theme="dark"] .guide-item p { color: #8b949e; }

[data-theme="dark"] .btn-clear-all { color: #f87171; }
[data-theme="dark"] .breadcrumb-pro { color: #8b949e; }

[data-theme="dark"] .enigma-toast { background: #161b22; }
</style>