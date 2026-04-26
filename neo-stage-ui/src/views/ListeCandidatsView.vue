<template>
  <div class="d-flex admin-layout">
    <AppSidebar />

    <div class="main-content flex-grow-1">
      <AppNavbar />

      <main class="p-4 p-lg-5 animate-in">

        <!-- HEADER -->
        <header class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <p class="brand-eyebrow">TABLEAU DE BORD · RECRUTEMENT</p>
            <h2 class="brand-title">Gestion des <span>Candidats</span></h2>
            <p class="brand-subtitle">RÉPERTOIRE GLOBAL ET SUIVI DES TALENTS</p>
          </div>
          <div class="d-flex gap-3">
            <button @click="$router.push('/invite')" class="btn-action-outline">
              <i class="fa-solid fa-user-plus me-2"></i> Invite Unique
            </button>
            <button @click="$router.push('/groups')" class="btn-action-gold">
              <i class="fa-solid fa-users-rectangle me-2"></i> Invite par Groupe
            </button>
          </div>
        </header>

        <!-- KPI CARDS -->
        <div class="row g-4 mb-5">
          <div class="col-md-4" v-for="stat in kpiStats" :key="stat.label">
            <div class="stat-card shadow-sm">
              <div class="stat-icon" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div class="ms-3">
                <div class="stat-value">{{ stat.value }}</div>
                <div class="stat-label">{{ stat.label }}</div>
              </div>
              <div class="stat-trend ms-auto">
                <i class="fa-solid fa-arrow-trend-up" :style="{ color: stat.color }"></i>
              </div>
            </div>
          </div>
        </div>

        <!-- FILTERS BAR -->
        <div class="filter-bar premium-card mb-4 shadow-sm">
          <div class="row g-3 align-items-center">
            <div class="col-md-5">
              <div class="search-input-group">
                <i class="fa-solid fa-magnifying-glass search-icon"></i>
                <input v-model="search" type="text" placeholder="Rechercher un candidat, email, groupe..." class="filter-input">
              </div>
            </div>
            <div class="col-md-4">
              <div class="select-wrapper">
                <i class="fa-solid fa-filter select-icon"></i>
                <select v-model="selectedFilter" class="filter-select">
                  <option value="">Toutes les Campagnes</option>
                  <option v-for="camp in campaigns" :key="camp.id" :value="camp.nom || camp.titre">
                    {{ camp.nom || camp.titre }}
                  </option>
                </select>
              </div>
            </div>
            <div class="col-md-3 d-flex justify-content-end gap-2 align-items-center">
              <span class="results-count">{{ filteredCandidates.length }} résultat(s)</span>
              <button @click="fetchCandidates" class="btn-refresh" title="Actualiser">
                <i class="fa-solid fa-rotate"></i> Actualiser
              </button>
            </div>
          </div>
        </div>

        <!-- LOADING STATE -->
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-pulse mx-auto mb-3"></div>
          <p class="fw-700 text-slate-400">Chargement des candidats...</p>
        </div>

        <!-- EMPTY STATE -->
        <div v-else-if="filteredCandidates.length === 0" class="empty-state premium-card text-center py-5">
          <div class="empty-icon mb-3"><i class="fa-solid fa-users-slash"></i></div>
          <h5 class="fw-800 text-navy">Aucun candidat trouvé</h5>
          <p class="text-muted small">Essayez de modifier vos filtres ou d'inviter de nouveaux candidats.</p>
        </div>

        <!-- CANDIDATES TABLE -->
        <div v-else class="premium-card p-0 overflow-hidden shadow-lg border-0">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-head">
                <tr>
                  <th class="ps-4 py-3">Candidat</th>
                  <th>E-mail</th>
                  <th>Campagne / Groupe</th>
                  <th>Statut d'Invitation</th>
                  <th class="text-end pe-4">Actions</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(c, index) in filteredCandidates" :key="c.id" class="candidate-row" :style="{ animationDelay: index * 0.05 + 's' }">
                  <td class="ps-4">
                    <div class="d-flex align-items-center gap-3">
                      <div class="avatar-initials" :style="getAvatarStyle(c.name)">
                        {{ c.name ? c.name.charAt(0).toUpperCase() : '?' }}
                      </div>
                      <div>
                        <div class="candidate-name">{{ c.name || 'Nom non renseigné' }}</div>
                        <div class="candidate-meta">Candidat #{{ c.id ? c.id.split('-')[0] : 'N/A' }}</div>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span class="email-text">{{ c.email }}</span>
                  </td>
                  <td>
                    <span class="group-tag">
                      <i class="fa-solid fa-tag me-1"></i> {{ c.group || 'Aucun groupe' }}
                    </span>
                  </td>
                  <td>
                    <span class="status-pill" :class="getStatusClass(c.status)">
                      <span class="status-dot"></span>
                      {{ getStatusLabel(c.status) }}
                    </span>
                  </td>
                  <td class="text-end pe-4">
                    <div class="d-flex gap-2 justify-content-end">
                      <button
                        @click="goToDetails(c.id)"
                        class="btn-action-icon btn-view"
                        title="Voir le profil"
                      >
                        <i class="fa-solid fa-eye"></i>
                        <span class="btn-label">Profil</span>
                      </button>
                      <button
                        class="btn-action-icon btn-more"
                        title="Plus d'options"
                      >
                        <i class="fa-solid fa-ellipsis-vertical"></i>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- TABLE FOOTER -->
          <div class="table-footer px-4 py-3 d-flex justify-content-between align-items-center">
            <span class="footer-text">Affichage de <strong>{{ filteredCandidates.length }}</strong> sur <strong>{{ candidates.length }}</strong> candidats</span>
            <div class="d-flex gap-2">
              <button class="btn-page" disabled><i class="fa-solid fa-chevron-left"></i></button>
              <button class="btn-page active">1</button>
              <button class="btn-page" disabled><i class="fa-solid fa-chevron-right"></i></button>
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
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const router = useRouter();
const candidates = ref([]);
const campaigns = ref([]);
const search = ref('');
const selectedFilter = ref('');
const loading = ref(false);

// ✅ FIX: Navigation correcte vers la page détails
const goToDetails = (id) => {
  if (!id) return;
  router.push({ name: 'details-candidat', params: { id } });
};

const kpiStats = computed(() => [
  {
    label: 'Total Candidats',
    value: candidates.value.length,
    icon: 'fa-solid fa-users',
    bg: '#fff9e6',
    color: '#f59e0b'
  },
  {
    label: 'Invitations Actives',
    value: candidates.value.filter(c => c.status !== 'terminé').length,
    icon: 'fa-solid fa-paper-plane',
    bg: '#eef2ff',
    color: '#6366f1'
  },
  {
    label: 'Groupes Engagés',
    value: campaigns.value.length,
    icon: 'fa-solid fa-layer-group',
    bg: '#f0fff4',
    color: '#10b981'
  }
]);

const fetchCandidates = async () => {
  loading.value = true;
  try {
    const res = await api.get('/Candidates');
    candidates.value = Array.isArray(res.data) ? res.data : [];
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

const filteredCandidates = computed(() => {
  return candidates.value.filter(c => {
    const q = search.value.toLowerCase();
    const matchesSearch =
      c.name?.toLowerCase().includes(q) ||
      c.email?.toLowerCase().includes(q) ||
      c.group?.toLowerCase().includes(q);
    const matchesGroup =
      selectedFilter.value === '' || c.group === selectedFilter.value;
    return matchesSearch && matchesGroup;
  });
});

const getStatusClass = (status) => {
  if (!status) return 'status-invited';
  const s = status.toLowerCase();
  if (s === 'terminé' || s === 'termine') return 'status-done';
  if (s === 'postulé' || s === 'postule') return 'status-applied';
  if (s === 'en cours') return 'status-progress';
  return 'status-invited';
};

const getStatusLabel = (status) => {
  if (!status) return 'Invité';
  return status;
};

// Couleurs d'avatar dynamiques selon la première lettre
const avatarColors = [
  { bg: '#fef3c7', color: '#d97706' },
  { bg: '#dbeafe', color: '#2563eb' },
  { bg: '#d1fae5', color: '#059669' },
  { bg: '#fce7f3', color: '#db2777' },
  { bg: '#ede9fe', color: '#7c3aed' },
  { bg: '#ffedd5', color: '#ea580c' },
];

const getAvatarStyle = (name) => {
  if (!name) return { background: '#f1f5f9', color: '#94a3b8' };
  const idx = name.charCodeAt(0) % avatarColors.length;
  return { background: avatarColors[idx].bg, color: avatarColors[idx].color };
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=DM+Sans:wght@400;500;600;700;800&family=Sora:wght@700;800&display=swap');

/* ────────── BASE ────────── */
.admin-layout {
  background: #f0f2f8;
  min-height: 100vh;
  font-family: 'DM Sans', sans-serif;
}

.main-content { overflow-x: hidden; }

/* ────────── HEADER ────────── */
.brand-eyebrow {
  font-size: 10px;
  font-weight: 700;
  color: #f59e0b;
  letter-spacing: 3px;
  margin-bottom: 4px;
}
.brand-title {
  font-family: 'Sora', sans-serif;
  font-weight: 800;
  font-size: 2.4rem;
  letter-spacing: -1.5px;
  color: #0f172a;
  margin: 0;
  line-height: 1.1;
}
.brand-title span { color: #f59e0b; }
.brand-subtitle {
  font-size: 9px;
  font-weight: 700;
  color: #94a3b8;
  letter-spacing: 3px;
  margin-top: 4px;
}

/* ────────── BUTTONS ────────── */
.btn-action-gold {
  background: linear-gradient(135deg, #f59e0b 0%, #f97316 100%);
  color: #fff;
  border: none;
  padding: 12px 24px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 13px;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  box-shadow: 0 4px 15px rgba(245, 158, 11, 0.3);
}
.btn-action-gold:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 25px rgba(245, 158, 11, 0.4);
}
.btn-action-outline {
  background: white;
  color: #0f172a;
  border: 2px solid #e2e8f0;
  padding: 10px 22px;
  border-radius: 14px;
  font-weight: 700;
  font-size: 13px;
  transition: all 0.25s ease;
}
.btn-action-outline:hover {
  border-color: #f59e0b;
  color: #f59e0b;
  background: #fff9e6;
}

/* ────────── KPI CARDS ────────── */
.stat-card {
  background: white;
  padding: 22px 24px;
  border-radius: 20px;
  display: flex;
  align-items: center;
  border: 1px solid #e8ecf4;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}
.stat-card::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 3px;
  background: linear-gradient(90deg, #f59e0b, #f97316);
  opacity: 0;
  transition: opacity 0.3s;
}
.stat-card:hover { transform: translateY(-4px); box-shadow: 0 20px 40px rgba(0,0,0,0.08) !important; }
.stat-card:hover::before { opacity: 1; }
.stat-icon {
  width: 52px; height: 52px;
  border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 20px;
  flex-shrink: 0;
}
.stat-value { font-size: 28px; font-weight: 800; color: #0f172a; line-height: 1; }
.stat-label { font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; margin-top: 3px; }
.stat-trend { font-size: 18px; opacity: 0.4; }

/* ────────── FILTER BAR ────────── */
.premium-card {
  background: white;
  border-radius: 20px;
  padding: 20px;
  border: 1px solid #e8ecf4;
}
.filter-bar { padding: 18px 24px; }

.search-input-group { position: relative; }
.search-icon {
  position: absolute; left: 14px; top: 50%;
  transform: translateY(-50%);
  color: #94a3b8; font-size: 13px;
  pointer-events: none;
}
.filter-input {
  width: 100%;
  padding: 11px 15px 11px 40px;
  border-radius: 12px;
  border: 1.5px solid #e2e8f0;
  background: #f8fafc;
  font-weight: 600;
  font-size: 13px;
  color: #0f172a;
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
  font-family: 'DM Sans', sans-serif;
}
.filter-input:focus { border-color: #f59e0b; box-shadow: 0 0 0 3px rgba(245, 158, 11, 0.1); }
.filter-input::placeholder { color: #94a3b8; font-weight: 500; }

.select-wrapper { position: relative; }
.select-icon {
  position: absolute; left: 14px; top: 50%;
  transform: translateY(-50%);
  color: #94a3b8; font-size: 13px;
  pointer-events: none; z-index: 1;
}
.filter-select {
  width: 100%;
  padding: 11px 15px 11px 38px;
  border-radius: 12px;
  border: 1.5px solid #e2e8f0;
  background: #f8fafc;
  font-weight: 600;
  font-size: 13px;
  color: #0f172a;
  outline: none;
  appearance: none;
  cursor: pointer;
  transition: border-color 0.2s;
  font-family: 'DM Sans', sans-serif;
}
.filter-select:focus { border-color: #f59e0b; }

.results-count { font-size: 12px; font-weight: 700; color: #94a3b8; }
.btn-refresh {
  background: #f8fafc;
  border: 1.5px solid #e2e8f0;
  color: #475569;
  font-weight: 700;
  font-size: 12px;
  padding: 9px 16px;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-refresh:hover { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
.btn-refresh:hover i { animation: spin 0.6s ease; }
@keyframes spin { to { transform: rotate(360deg); } }

/* ────────── TABLE ────────── */
.table-head { background: #0f172a; }
.table-head th {
  color: #94a3b8;
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 1.5px;
  border: none;
  padding: 14px 12px;
}
.table-head th:first-child { color: white; }

.candidate-row {
  border-bottom: 1px solid #f1f5f9;
  animation: slideInRow 0.4s ease-out both;
  transition: background 0.2s;
}
@keyframes slideInRow {
  from { opacity: 0; transform: translateX(-10px); }
  to   { opacity: 1; transform: translateX(0); }
}
.candidate-row:hover { background-color: #fffcf5 !important; }
.candidate-row td { padding: 16px 12px; border: none; vertical-align: middle; }

.avatar-initials {
  width: 40px; height: 40px;
  border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 15px;
  flex-shrink: 0;
}
.candidate-name { font-weight: 700; color: #0f172a; font-size: 14px; }
.candidate-meta { font-size: 11px; color: #94a3b8; font-weight: 600; margin-top: 2px; }

.email-text { color: #475569; font-weight: 600; font-size: 13px; }

.group-tag {
  background: #f1f5f9;
  color: #475569;
  padding: 5px 12px;
  border-radius: 8px;
  font-size: 11px;
  font-weight: 700;
  white-space: nowrap;
  border: 1px solid #e2e8f0;
}

/* STATUS PILLS */
.status-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 10px;
  font-weight: 800;
  padding: 5px 13px;
  border-radius: 20px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}
.status-dot {
  width: 6px; height: 6px;
  border-radius: 50%;
  background: currentColor;
  animation: blink 2s infinite;
}
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }

.status-invited  { background: #eef2ff; color: #6366f1; }
.status-done     { background: #ecfdf5; color: #10b981; }
.status-applied  { background: #fef3c7; color: #d97706; }
.status-progress { background: #fff7ed; color: #f97316; }

/* ACTION BUTTONS */
.btn-action-icon {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 7px 14px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  border: 1.5px solid transparent;
  transition: all 0.2s ease;
  font-family: 'DM Sans', sans-serif;
}
.btn-view {
  background: #fff9e6;
  color: #d97706;
  border-color: #fde68a;
}
.btn-view:hover {
  background: #0f172a;
  color: #f59e0b;
  border-color: #0f172a;
  transform: scale(1.05);
}
.btn-more {
  background: #f8fafc;
  color: #94a3b8;
  border-color: #e2e8f0;
  padding: 7px 10px;
}
.btn-more:hover { background: #f1f5f9; color: #475569; }
.btn-label { font-size: 11px; }

/* TABLE FOOTER */
.table-footer {
  border-top: 1px solid #f1f5f9;
  background: #fafbfc;
}
.footer-text { font-size: 12px; color: #94a3b8; font-weight: 600; }
.btn-page {
  width: 32px; height: 32px;
  border-radius: 8px;
  border: 1.5px solid #e2e8f0;
  background: white;
  color: #475569;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s;
}
.btn-page.active { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
.btn-page:disabled { opacity: 0.4; cursor: default; }

/* EMPTY STATE */
.empty-state { border: 2px dashed #e2e8f0; }
.empty-icon { font-size: 48px; color: #d1d5db; }

/* SPINNER */
.spinner-pulse {
  width: 44px; height: 44px;
  border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
}

/* ANIMATE IN */
.animate-in { animation: fadeInRight 0.5s ease-out; }
@keyframes fadeInRight {
  from { opacity: 0; transform: translateX(16px); }
  to   { opacity: 1; transform: translateX(0); }
}
</style>