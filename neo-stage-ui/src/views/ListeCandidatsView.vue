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

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- ═══ HEADER ═══ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">{{ t('candidatListe.breadcrumb') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ t('candidatListe.titleHighlight') }}</span>
              </div>
              <h2 class="premium-title">
                {{ t('candidatListe.title') }}
                <span class="gradient-text">{{ t('candidatListe.titleHighlight') }}</span>
              </h2>
              <p class="brand-subtitle">{{ t('candidatListe.subtitle') }}</p>
            </div>
            <div class="d-flex gap-3 flex-wrap">
              <button @click="$router.push('/invite')" class="btn-outline-pro">
                <i class="fa-solid fa-user-plus me-2"></i>{{ t('candidatListe.inviteUnique') }}
              </button>
              <button @click="$router.push('/groups')" class="btn-enigma-primary shadow-premium">
                <div class="btn-content">
                  <i class="fa-solid fa-users-rectangle me-2"></i>{{ t('candidatListe.inviteGroup') }}
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- ═══ KPI CARDS ═══ -->
          <div class="row g-4 mb-5">
            <div class="col-md-4" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
                <div class="stat-trend ms-auto trend-up">
                  <i class="fa-solid fa-arrow-trend-up"></i>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══ FILTER BAR ═══ -->
          <div class="enigma-card p-4 mb-4">
            <div class="row g-3 align-items-center">
              <div class="col-md-5">
                <div class="search-inline-box">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  <input
                    v-model="search"
                    type="text"
                    :placeholder="t('candidatListe.searchPlaceholder')"
                    class="search-inline-input"
                  >
                  <button v-if="search" @click="search = ''" class="btn-clear-search">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>
              </div>
              <div class="col-md-4">
                <div class="select-pro-wrap">
                  <i class="fa-solid fa-filter select-pro-icon"></i>
                  <select v-model="selectedFilter" class="enigma-field select-pro">
                    <option value="">{{ t('candidatListe.allCampaigns') }}</option>
                    <option v-for="camp in campaigns" :key="camp.id" :value="camp.nom || camp.titre">
                      {{ camp.nom || camp.titre }}
                    </option>
                  </select>
                </div>
              </div>
              <div class="col-md-3 d-flex justify-content-end gap-2 align-items-center">
                <span class="results-count">{{ filteredCandidates.length }} {{ t('candidatListe.results') }}</span>
                <button @click="fetchCandidates" class="btn-refresh-pro" :title="t('candidatListe.refreshTitle')">
                  <i class="fa-solid fa-rotate"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- ═══ LOADING ═══ -->
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-pro-premium"></div>
            <p class="fw-700 text-muted mt-3" style="font-size:0.85rem">{{ t('candidatListe.loadingText') }}</p>
          </div>

          <!-- ═══ EMPTY STATE ═══ -->
          <div v-else-if="filteredCandidates.length === 0" class="empty-state-pro py-5 text-center enigma-card">
            <i class="fa-solid fa-users-slash fa-3x text-muted mb-3"></i>
            <h5 class="fw-800">{{ t('candidatListe.emptyTitle') }}</h5>
            <p class="text-muted small">{{ t('candidatListe.emptySubtitle') }}</p>
            <button @click="$router.push('/invite')" class="btn-enigma-primary mt-3">
              <div class="btn-content">
                <i class="fa-solid fa-user-plus me-2"></i>{{ t('candidatListe.inviteBtn') }}
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>

          <!-- ═══ TABLE ═══ -->
          <div v-else class="enigma-card p-0 overflow-hidden">

            <!-- LIST HEADER -->
            <div class="list-header-row d-flex align-items-center px-4 py-3">
              <span style="width:260px" class="list-col-label">{{ t('candidatListe.colCandidate') }}</span>
              <span class="flex-grow-1 list-col-label">{{ t('candidatListe.colEmail') }}</span>
              <span style="width:180px" class="list-col-label">{{ t('candidatListe.colGroup') }}</span>
              <span style="width:150px" class="list-col-label text-center">{{ t('candidatListe.colStatus') }}</span>
              <span style="width:120px" class="list-col-label text-end pe-2">{{ t('candidatListe.colActions') }}</span>
            </div>

            <!-- ROWS -->
            <div
              v-for="(c, index) in filteredCandidates"
              :key="c.id"
              class="list-row-item d-flex align-items-center px-4 py-3"
              :style="{ animationDelay: index * 0.04 + 's' }"
            >
              <!-- Candidat -->
              <div style="width:260px" class="d-flex align-items-center gap-3">
                <div class="avatar-initials" :style="getAvatarStyle(c.name)">
                  {{ c.name ? c.name.charAt(0).toUpperCase() : '?' }}
                </div>
                <div>
                  <div class="candidate-name">{{ c.name || t('candidatListe.noName') }}</div>
                  <div class="candidate-meta">{{ t('candidatListe.candidateId') }}{{ c.id ? c.id.toString().split('-')[0] : 'N/A' }}</div>
                </div>
              </div>

              <!-- Email -->
              <div class="flex-grow-1">
                <span class="email-text">{{ c.email }}</span>
              </div>

              <!-- Groupe -->
              <div style="width:180px">
                <span class="group-tag">
                  <i class="fa-solid fa-tag me-1"></i>{{ c.group || t('candidatListe.noGroup') }}
                </span>
              </div>

              <!-- Statut -->
              <div style="width:150px" class="text-center">
                <span class="status-badge" :class="getStatusClass(c.status)">
                  <span class="status-dot"></span>
                  {{ getStatusLabel(c.status) }}
                </span>
              </div>

              <!-- Actions -->
              <div style="width:120px" class="d-flex gap-2 justify-content-end">
                <button @click="goToDetails(c.id)" class="btn-icon-sm btn-view-cand" :title="t('candidatListe.viewProfile')">
                  <i class="fa-solid fa-eye"></i>
                </button>
                <button @click="confirmDeleteCandidate(c)" class="btn-icon-sm btn-delete-cand" :title="t('candidatListe.actionDelete') || 'Supprimer'">
                  <i class="fa-solid fa-trash-can"></i>
                </button>
              </div>
            </div>

            <!-- TABLE FOOTER -->
            <div class="table-footer-bar px-4 py-3 d-flex justify-content-between align-items-center">
              <span class="footer-text">
                {{ t('candidatListe.showing') }}
                <strong>{{ filteredCandidates.length }}</strong>
                {{ t('candidatListe.outOf') }}
                <strong>{{ candidates.length }}</strong>
                {{ t('candidatListe.candidatesSuffix') }}
              </span>
              <div class="d-flex gap-2">
                <button class="btn-page" disabled><i class="fa-solid fa-chevron-left"></i></button>
                <button class="btn-page active">1</button>
                <button class="btn-page" disabled><i class="fa-solid fa-chevron-right"></i></button>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- TOAST NOTIFICATIONS -->
    <transition name="toast-anim">
      <div v-if="toast.active" class="custom-toast animate__animated animate__fadeIn" :class="'toast-' + toast.type">
        <i :class="toast.icon" class="fs-5"></i>
        <span>{{ toast.message }}</span>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const { t } = useI18n();
const router     = useRouter();
const candidates = ref([]);
const campaigns  = ref([]);
const search     = ref('');
const selectedFilter = ref('');
const loading    = ref(false);
const mousePos   = reactive({ x: 0, y: 0 });

const toast = reactive({ active: false, message: '', type: 'success', icon: '' });
let toastTimer = null;

const showToast = (message, type = 'success', icon = 'fa-solid fa-circle-check') => {
  clearTimeout(toastTimer);
  Object.assign(toast, { message, type, icon, active: true });
  toastTimer = setTimeout(() => { toast.active = false; }, 4000);
};

const confirmDeleteCandidate = async (candidate) => {
  const isConfirmed = confirm(`${t('candidatListe.confirmDelete') || 'Voulez-vous vraiment supprimer ce candidat et toutes ses données associées ?'} (${candidate.name || candidate.email})`);
  if (!isConfirmed) return;

  showToast(t('candidatListe.deleting') || 'Suppression en cours...', 'info', 'fa-solid fa-spinner fa-spin');
  try {
    await api.delete(`/Candidates/${candidate.id}`);
    showToast(t('candidatListe.deleteSuccess') || 'Candidat supprimé avec succès.', 'success', 'fa-solid fa-trash-can');
    fetchCandidates();
  } catch (err) {
    console.error('Erreur lors de la suppression du candidat:', err);
    showToast(err.response?.data?.message || 'Erreur lors de la suppression.', 'error', 'fa-solid fa-circle-exclamation');
  }
};

/* ── Navigation ── */
const goToDetails = (id) => {
  if (!id) return;
  router.push({ name: 'details-candidat', params: { id } });
};

/* ── KPI — labels traduits ── */
const kpiStats = computed(() => [
  {
    label: t('candidatListe.kpiTotal'),
    value: candidates.value.length,
    icon:  'fa-solid fa-users',
    bg:    '#fffbeb',
    color: '#f59e0b',
  },
  {
    label: t('candidatListe.kpiActive'),
    value: candidates.value.filter(c => c.status !== 'terminé').length,
    icon:  'fa-solid fa-paper-plane',
    bg:    '#eef2ff',
    color: '#6366f1',
  },
  {
    label: t('candidatListe.kpiGroups'),
    value: campaigns.value.length,
    icon:  'fa-solid fa-layer-group',
    bg:    '#ecfdf5',
    color: '#10b981',
  },
]);

/* ── API ── */
const fetchCandidates = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Candidates');
    const allCandidates = Array.isArray(res.data) ? res.data : [];

    // ── FILTRAGE : exclure les admins / responsables / entreprises ──
    // On exclut tout utilisateur dont le rôle indique admin/responsable/entreprise
    // OU dont le nom commence par des mots-clés typiques d'un compte admin
    const adminKeywords = ['responsable', 'admin', 'administrateur', 'manager', 'rh ', 'drh', 'entreprise'];
    const adminRoles    = ['admin', 'administrator', 'manager', 'responsable', 'hr', 'recruiter', 'entreprise'];

    candidates.value = allCandidates.filter(c => {
      const name  = (c.name  || c.fullName || c.nom  || '').toLowerCase();
      const role  = (c.role  || c.Role     || c.type || c.userType || '').toLowerCase();
      const email = (c.email || c.Email    || '').toLowerCase();

      // Exclure si le rôle est explicitement non-candidat
      if (adminRoles.some(r => role.includes(r))) return false;

      // Exclure si le nom contient un mot-clé admin
      if (adminKeywords.some(kw => name.startsWith(kw) || name.includes(' ' + kw))) return false;

      // Exclure si isAdmin / isEnterprise flag est true
      if (c.isAdmin || c.IsAdmin || c.isEntreprise || c.IsEntreprise) return false;

      return true;
    });

  } catch (e) {
    console.error('Erreur chargement candidats:', e);
    candidates.value = [];
  } finally {
    loading.value = false;
  }
};

const fetchCampaigns = async () => {
  try {
    const res = await api.get('/Invitations/campagnes');
    campaigns.value = Array.isArray(res.data) ? res.data : [];
  } catch (e) {
    console.error('Erreur chargement campagnes:', e);
  }
};

onMounted(() => {
  fetchCandidates();
  fetchCampaigns();
});

/* ── Filters ── */
const filteredCandidates = computed(() =>
  candidates.value.filter(c => {
    const q = search.value.toLowerCase();
    const matchesSearch =
      c.name?.toLowerCase().includes(q) ||
      c.email?.toLowerCase().includes(q) ||
      c.group?.toLowerCase().includes(q);
    const matchesGroup = selectedFilter.value === '' || c.group === selectedFilter.value;
    return matchesSearch && matchesGroup;
  })
);

/* ── Status — traduits via i18n ── */
const getStatusClass = (status) => {
  if (!status) return 'status-0';
  const s = status.toLowerCase();
  if (s === 'terminé' || s === 'termine') return 'status-2';
  if (s === 'postulé' || s === 'postule') return 'status-applied';
  if (s === 'en cours') return 'status-progress';
  return 'status-0';
};

const getStatusLabel = (status) => {
  if (!status) return t('candidatListe.statusInvited');
  const s = status.toLowerCase();
  if (s === 'terminé' || s === 'termine') return t('candidatListe.statusTerminated');
  if (s === 'postulé' || s === 'postule') return t('candidatListe.statusApplied');
  if (s === 'en cours') return t('candidatListe.statusInProgress');
  return t('candidatListe.statusInvited');
};

/* ── Avatar ── */
const avatarPalette = [
  { bg: '#fffbeb', color: '#d97706' },
  { bg: '#eef2ff', color: '#4f46e5' },
  { bg: '#ecfdf5', color: '#059669' },
  { bg: '#fdf2f8', color: '#db2777' },
  { bg: '#f5f3ff', color: '#7c3aed' },
  { bg: '#fff7ed', color: '#ea580c' },
];
const getAvatarStyle = (name) => {
  if (!name) return { background: '#f1f5f9', color: '#94a3b8' };
  const idx = name.charCodeAt(0) % avatarPalette.length;
  return { background: avatarPalette[idx].bg, color: avatarPalette[idx].color };
};

/* ── Parallax ── */
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};
</script>


<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');

/* ════════════════════════════════════════
   BASE
════════════════════════════════════════ */
.enigma-master-root {
  min-height: 100vh;
  background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* BACKGROUND */
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
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }

.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ════════════════════════════════════════
   HEADER
════════════════════════════════════════ */
.premium-title {
  font-weight: 800;
  font-size: 2.2rem;
  letter-spacing: -1px;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
}
.brand-subtitle {
  font-size: 0.6rem;
  font-weight: 800;
  color: #94a3b8;
  letter-spacing: 2px;
  margin-top: 4px;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ════════════════════════════════════════
   BUTTONS
════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer;
  font-family: inherit;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2; display: flex; align-items: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

.btn-outline-pro {
  background: white; color: #0f172a;
  border: 1.5px solid #eef2f6;
  padding: 12px 22px; border-radius: 16px;
  font-weight: 800; font-size: 0.85rem;
  cursor: pointer; transition: 0.2s; font-family: inherit;
}
.btn-outline-pro:hover { border-color: #f59e0b; color: #f59e0b; background: #fffbeb; }

.btn-refresh-pro {
  width: 44px; height: 44px;
  background: white; border: 1.5px solid #e2e8f0;
  border-radius: 14px; color: #64748b;
  cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover { background: #f8fafc; border-color: #f59e0b; color: #f59e0b; transform: rotate(180deg) scale(1.1); }

.btn-icon-sm {
  width: 34px; height: 34px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  color: #64748b; cursor: pointer; transition: 0.2s;
  font-size: 0.8rem; display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }
.btn-view-cand { background: #fffbeb; color: #d97706; border-color: #fde68a; }
.btn-view-cand:hover { background: #0f172a; color: #f59e0b; border-color: #0f172a; }

/* ════════════════════════════════════════
   STAT CARDS
════════════════════════════════════════ */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center;
  border: 1px solid #eef2f6; transition: 0.2s;
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.stat-details { margin-left: 16px; }
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; font-size: 0.7rem; font-weight: 800; }
.trend-up { color: #10b981; }

/* ════════════════════════════════════════
   ENIGMA CARD
════════════════════════════════════════ */
.enigma-card {
  background: white;
  border-radius: 32px;
  border: 1px solid #eef2f6;
}

/* ════════════════════════════════════════
   FILTER BAR
════════════════════════════════════════ */
.search-inline-box {
  display: flex; align-items: center;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 16px; padding: 0 14px; gap: 10px; color: #94a3b8;
  transition: border-color 0.2s;
}
.search-inline-box:focus-within { border-color: #f59e0b; background: white; }
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 12px 0; font-weight: 700; font-size: 0.85rem;
  flex: 1; font-family: inherit; color: #0f172a;
}
.search-inline-input::placeholder { color: #94a3b8; font-weight: 600; }
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }
.btn-clear-search:hover { color: #0f172a; }

.select-pro-wrap { position: relative; }
.select-pro-icon {
  position: absolute; left: 14px; top: 50%;
  transform: translateY(-50%); color: #94a3b8;
  font-size: 0.75rem; pointer-events: none; z-index: 1;
}
.enigma-field {
  width: 100%; padding: 12px 14px; background: #f8fafc;
  border: 1.5px solid #eef2f6; border-radius: 16px;
  font-weight: 700; outline: none; font-family: inherit;
  transition: border-color 0.2s; font-size: 0.85rem; color: #0f172a;
}
.enigma-field:focus { border-color: #f59e0b; background: white; }
.select-pro { padding-left: 38px !important; appearance: none; cursor: pointer; }

.results-count { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }

/* ════════════════════════════════════════
   LIST
════════════════════════════════════════ */
.list-header-row {
  background: #f8fafc;
  border-bottom: 1px solid #eef2f6;
}
.list-col-label {
  font-size: 0.6rem; font-weight: 900;
  color: #94a3b8; text-transform: uppercase; letter-spacing: 1px;
}

.list-row-item {
  border-bottom: 1px solid #f1f5f9;
  transition: background 0.18s;
  animation: slideInRow 0.35s ease-out both;
}
@keyframes slideInRow {
  from { opacity: 0; transform: translateX(-8px); }
  to   { opacity: 1; transform: translateX(0); }
}
.list-row-item:hover { background: #fffcf5; }
.list-row-item:last-child { border-bottom: none; }

/* ── Avatar ── */
.avatar-initials {
  width: 40px; height: 40px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 0.95rem; flex-shrink: 0;
}

/* ── Candidate info ── */
.candidate-name { font-weight: 700; color: #0f172a; font-size: 0.88rem; }
.candidate-meta { font-size: 0.65rem; color: #94a3b8; font-weight: 600; margin-top: 2px; }
.email-text     { color: #475569; font-weight: 600; font-size: 0.82rem; }

/* ── Group tag ── */
.group-tag {
  background: #f1f5f9; color: #475569;
  padding: 4px 12px; border-radius: 10px;
  font-size: 0.68rem; font-weight: 700;
  border: 1px solid #e2e8f0; white-space: nowrap;
  display: inline-flex; align-items: center;
}

/* ── Status badges ── */
.status-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 0.62rem; font-weight: 800;
  text-transform: uppercase; display: inline-flex; align-items: center;
}
.status-dot {
  width: 6px; height: 6px; border-radius: 50%;
  background: currentColor; margin-right: 6px;
  animation: blink 2s infinite;
}
@keyframes blink { 0%,100% { opacity:1; } 50% { opacity:0.3; } }

.status-0        { background: #eef2ff; color: #6366f1; }
.status-2        { background: #ecfdf5; color: #10b981; }
.status-applied  { background: #fffbeb; color: #d97706; }
.status-progress { background: #fff7ed; color: #f97316; }

/* ── TABLE FOOTER ── */
.table-footer-bar {
  background: #fafbfc;
  border-top: 1px solid #f1f5f9;
  border-radius: 0 0 32px 32px;
}
.footer-text { font-size: 0.72rem; color: #94a3b8; font-weight: 600; }
.btn-page {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid #e2e8f0; background: white;
  color: #475569; font-size: 0.72rem; font-weight: 700;
  cursor: pointer; display: flex; align-items: center; justify-content: center;
  transition: 0.2s;
}
.btn-page.active  { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
.btn-page:disabled { opacity: 0.4; cursor: default; }
.btn-page:not(:disabled):not(.active):hover { background: #f8fafc; color: #0f172a; }

/* ── EMPTY STATE ── */
.empty-state-pro { border: 2px dashed #e2e8f0; }

/* ── SPINNER ── */
.spinner-pro-premium {
  width: 48px; height: 48px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto;
}
@keyframes spin { to { transform: rotate(360deg); } }

/* ── SCROLLBAR ── */
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #eef2f6; border-radius: 10px; }

/* ── MISC ── */
.fw-700 { font-weight: 700 !important; }
.fw-800 { font-weight: 800 !important; }

/* ── ANIMATE COMPAT ── */
.animate__animated { animation-fill-mode: both; }
.animate__fadeIn { animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: none; } }

/* ════════════════════════════════════════
   DARK MODE
════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine      { background: #0d1117; }
[data-theme="dark"] .premium-title      { color: #f0f6fc; }
[data-theme="dark"] .brand-subtitle     { color: #8b949e; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

[data-theme="dark"] .stat-card-premium { background: rgba(22,27,34,0.7); border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .stat-value        { color: #f0f6fc; }
[data-theme="dark"] .stat-label        { color: #8b949e; }

[data-theme="dark"] .btn-outline-pro {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.1); color: #f0f6fc;
}
[data-theme="dark"] .btn-outline-pro:hover { border-color: #d97706; color: #f59e0b; background: rgba(245,158,11,0.08); }
[data-theme="dark"] .btn-refresh-pro {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.08); color: #8b949e;
}
[data-theme="dark"] .btn-refresh-pro:hover { border-color: #f59e0b; color: #f59e0b; background: rgba(245,158,11,0.08); }
[data-theme="dark"] .btn-icon-sm {
  background: rgba(255,255,255,0.04);
  border-color: rgba(255,255,255,0.08); color: #8b949e;
}
[data-theme="dark"] .btn-icon-sm:hover { background: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .btn-view-cand { background: rgba(245,158,11,0.1); color: #fbbf24; border-color: rgba(245,158,11,0.2); }
[data-theme="dark"] .btn-view-cand:hover { background: #f59e0b; color: #0d1117; border-color: #f59e0b; }

[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }

[data-theme="dark"] .search-inline-box {
  background: rgba(255,255,255,0.04);
  border-color: rgba(255,255,255,0.08);
}
[data-theme="dark"] .search-inline-box:focus-within { border-color: #d97706; background: rgba(255,255,255,0.06); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }
[data-theme="dark"] .enigma-field {
  background: rgba(255,255,255,0.04);
  border-color: rgba(255,255,255,0.08); color: #f0f6fc;
}
[data-theme="dark"] .enigma-field:focus { border-color: #d97706; background: rgba(255,255,255,0.06); }

[data-theme="dark"] .list-header-row { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .list-col-label   { color: #8b949e; }
[data-theme="dark"] .list-row-item    { border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .list-row-item:hover { background: rgba(245,158,11,0.04); }

[data-theme="dark"] .candidate-name { color: #f0f6fc; }
[data-theme="dark"] .candidate-meta { color: #8b949e; }
[data-theme="dark"] .email-text     { color: #8b949e; }

[data-theme="dark"] .group-tag { background: rgba(255,255,255,0.05); color: #8b949e; border-color: rgba(255,255,255,0.08); }

[data-theme="dark"] .status-0        { background: rgba(99,102,241,0.12);  color: #a5b4fc; }
[data-theme="dark"] .status-2        { background: rgba(16,185,129,0.12);  color: #34d399; }
[data-theme="dark"] .status-applied  { background: rgba(217,119,6,0.12);   color: #fbbf24; }
[data-theme="dark"] .status-progress { background: rgba(249,115,22,0.12);  color: #fb923c; }

[data-theme="dark"] .table-footer-bar { background: rgba(255,255,255,0.02); border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .footer-text      { color: #8b949e; }
[data-theme="dark"] .btn-page {
  background: rgba(255,255,255,0.04);
  border-color: rgba(255,255,255,0.08); color: #8b949e;
}
[data-theme="dark"] .btn-page.active { background: #f59e0b; color: #0d1117; border-color: #f59e0b; }
[data-theme="dark"] .btn-page:not(:disabled):not(.active):hover { background: rgba(255,255,255,0.08); color: #f0f6fc; }

[data-theme="dark"] .empty-state-pro { border-color: rgba(255,255,255,0.08); }

/* DELETE BUTTON ACTIONS */
.btn-delete-cand {
  background: #fff5f5;
  color: #ef4444;
  border-color: #fee2e2;
}
.btn-delete-cand:hover {
  background: #ef4444;
  color: white;
  border-color: #ef4444;
}

[data-theme="dark"] .btn-delete-cand {
  background: rgba(239, 68, 68, 0.1);
  color: #ff6b6b;
  border-color: rgba(239, 68, 68, 0.2);
}
[data-theme="dark"] .btn-delete-cand:hover {
  background: #ef4444;
  color: white;
  border-color: #ef4444;
}

/* TOAST */
.custom-toast {
  position: fixed;
  bottom: 30px;
  right: 30px;
  padding: 16px 24px;
  border-radius: 16px;
  color: white;
  font-weight: 700;
  font-size: 0.85rem;
  display: flex;
  align-items: center;
  gap: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.15);
  z-index: 9999;
}
.toast-success { background: #10b981; }
.toast-error   { background: #f43f5e; }
.toast-info    { background: #3b82f6; }

.toast-anim-enter-active,
.toast-anim-leave-active { transition: all 0.3s ease; }
.toast-anim-enter-from,
.toast-anim-leave-to { opacity: 0; transform: translateY(15px); }
</style>