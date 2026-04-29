<template>
  <div class="admin-body d-flex">

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <AppSidebar />

    <div class="content flex-grow-1">
      <AppNavbar />

      <div class="main-container p-4 pt-3">

        <!-- HEADER -->
        <header class="dashboard-header d-flex justify-content-between align-items-end mb-4">
          <div>
            <nav aria-label="breadcrumb">
              <ol class="breadcrumb mb-1">
                <li class="breadcrumb-item"><a href="#">Admin</a></li>
                <li class="breadcrumb-item active">Dashboard</li>
              </ol>
            </nav>
            <h2 class="page-title m-0">Master Control Panel</h2>
            <p class="text-secondary small m-0">Supervision en temps réel de l'écosystème NeoEvaluation</p>
          </div>

          <div class="header-actions d-flex gap-3 align-items-center">
            <div class="status-pill d-none d-lg-flex">
              <span class="pulse-dot"></span>
              <span class="status-text">Serveurs IA : <span class="text-success fw-bold">Optimum</span></span>
            </div>
            <button @click="showOrgModal = true" class="btn-premium secondary">
              <i class="fa-solid fa-plus-circle"></i>
              <span>Enregistrer une Entreprise</span>
            </button>
          </div>
        </header>

        <!-- STAT CARDS -->
        <div class="row g-4 mb-4">
          <div class="col-xl-3 col-md-6" v-for="s in masterStats" :key="s.label">
            <div class="card-premium stat-item">
              <div class="d-flex justify-content-between align-items-start mb-3">
                <div class="stat-icon-wrapper" :style="{ backgroundColor: s.bg, color: s.color }">
                  <i :class="s.icon"></i>
                </div>
                <span class="trend-badge" :class="s.trendUp ? 'up' : 'down'">
                  <i :class="s.trendUp ? 'fa-solid fa-arrow-up' : 'fa-solid fa-arrow-down'"></i>
                  {{ s.trend }}
                </span>
              </div>
              <div class="stat-content">
                <h3 class="stat-number">{{ s.val }}</h3>
                <p class="stat-label text-uppercase">{{ s.label }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- FILTER BAR -->
        <div class="card-premium filter-bar mb-4">
          <div class="search-box">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher une entreprise, un email, un ID...">
          </div>
          <div class="filter-controls">
            <select class="form-select-premium">
              <option>Statut : Tous</option>
              <option>Actif</option>
              <option>Inactif</option>
            </select>
            <select class="form-select-premium">
              <option>Période : 30 jours</option>
              <option>7 jours</option>
              <option>90 jours</option>
            </select>
            <button class="btn-refresh" @click="fetchData">
              <i class="fa-solid fa-rotate"></i>
            </button>
          </div>
        </div>

        <!-- MAIN GRID -->
        <div class="row g-4 mb-5">

          <!-- DEMANDES TABLE -->
          <div class="col-lg-8">
            <div class="card-premium h-100 p-0 overflow-hidden">
              <div class="card-header-premium p-4 d-flex justify-content-between align-items-center">
                <h6 class="m-0 fw-bold">
                  <i class="fa-solid fa-id-card-clip text-indigo me-2"></i>Demandes d'adhésion
                </h6>
                <span class="badge-count-pro">{{ filteredPendingRequests.length }} dossiers</span>
              </div>

              <div class="table-responsive">
                <table class="table table-custom m-0">
                  <thead>
                    <tr>
                      <th class="ps-4">Entreprise</th>
                      <th>Contact</th>
                      <th>Date</th>
                      <th class="text-end pe-4">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="req in filteredPendingRequests" :key="req.id" class="align-middle">
                      <td class="ps-4">
                        <div class="d-flex align-items-center">
                          <div class="avatar-letter">{{ req.nomEntreprise?.[0] || '?' }}</div>
                          <div class="ms-3">
                            <div class="fw-bold text-dark small">{{ req.nomEntreprise }}</div>
                            <div class="tiny text-muted">Matricule: {{ req.matriculeFiscale || 'Non renseigné' }}</div>
                          </div>
                        </div>
                      </td>
                      <td>
                        <div class="small fw-semibold">{{ req.nomResponsable }}</div>
                        <div class="tiny text-indigo">{{ req.emailResponsable }}</div>
                      </td>
                      <td class="tiny text-secondary">{{ formatDate(req.creeLe) }}</td>
                      <td class="text-end pe-4">
                        <div class="d-flex justify-content-end gap-2">
                          <button @click="handleApprove(req.id)" class="btn-circle approve" title="Approuver">
                            <i class="fa-solid fa-check"></i>
                          </button>
                          <button @click="handleReject(req.id)" class="btn-circle reject" title="Refuser">
                            <i class="fa-solid fa-xmark"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                    <tr v-if="filteredPendingRequests.length === 0">
                      <td colspan="4" class="text-center py-5">
                        <img src="https://cdn-icons-png.flaticon.com/512/7486/7486744.png" width="60" class="opacity-25 mb-3 d-block mx-auto">
                        <p class="text-muted small">Aucun dossier trouvé</p>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- RIGHT COLUMN -->
          <div class="col-lg-4">
            <div class="d-flex flex-column gap-4 h-100">

              <!-- AI PERFORMANCE -->
              <div class="card-premium p-4">
                <h6 class="fw-bold mb-4">
                  <i class="fa-solid fa-microchip text-indigo me-2"></i>Performance IA
                </h6>
                <div class="chart-container mb-4">
                  <canvas id="masterChart"></canvas>
                  <div class="chart-center-text">
                    <span class="number">82%</span>
                    <span class="label">Charge</span>
                  </div>
                </div>

                <ul class="list-unstyled mb-0">
                  <li class="d-flex justify-content-between align-items-center mb-3">
                    <div class="d-flex align-items-center gap-2">
                      <span class="dot-sm bg-indigo"></span>
                      <span class="small text-secondary">Tokens LLM</span>
                    </div>
                    <span class="small fw-bold">1.2M / 1.5M</span>
                  </li>
                  <li class="d-flex justify-content-between align-items-center mb-3">
                    <div class="d-flex align-items-center gap-2">
                      <span class="dot-sm bg-success"></span>
                      <span class="small text-secondary">Temps de réponse</span>
                    </div>
                    <span class="small fw-bold">240ms</span>
                  </li>
                  <li class="d-flex justify-content-between align-items-center">
                    <div class="d-flex align-items-center gap-2">
                      <span class="dot-sm bg-orange"></span>
                      <span class="small text-secondary">Requêtes actives</span>
                    </div>
                    <span class="small fw-bold">318 / s</span>
                  </li>
                </ul>
              </div>

              <!-- MODULE USAGE -->
              <div class="card-premium p-4">
                <h6 class="fw-bold mb-4">
                  <i class="fa-solid fa-chart-bar text-indigo me-2"></i>Utilisation par module
                </h6>
                <div v-for="mod in moduleUsage" :key="mod.name" class="mb-3">
                  <div class="d-flex justify-content-between mb-1">
                    <span class="small text-secondary">{{ mod.name }}</span>
                    <span class="small fw-bold">{{ mod.pct }}%</span>
                  </div>
                  <div class="prog-bar-bg">
                    <div class="prog-bar" :style="{ width: mod.pct + '%', background: mod.color }"></div>
                  </div>
                </div>
              </div>

            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- CREATE ORG MODAL -->
    <Transition name="slide-up">
      <div v-if="showOrgModal" class="modal-backdrop-custom" @click.self="showOrgModal = false">
        <div class="modal-content-pro modal-wide">
          <div class="modal-header-pro border-bottom-0 pb-0 d-flex justify-content-between align-items-center">
            <h5 class="fw-bold m-0 text-dark">Créer une organisation</h5>
            <button @click="showOrgModal = false" class="btn-close-modal">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <form @submit.prevent="handleCreateOrg" class="modal-body-pro pt-2">

            <!-- SECTION 1: ORGANISATION -->
            <div class="section-title-mod mb-3 mt-2">
              <i class="fa-solid fa-briefcase text-warning me-2"></i>
              <span class="fw-bold">Informations sur l'organisation</span>
            </div>

            <div class="mb-3">
              <label class="form-label-mod">Nom de l'organisation <span class="text-danger">*</span></label>
              <input type="text" v-model="newOrg.name" class="form-control-mod" placeholder="Entrez le nom de l'organisation" required>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-mod">Domaine</label>
                <input type="text" v-model="newOrg.domain" class="form-control-mod" placeholder="entreprise.com">
              </div>
              <div class="col-md-6">
                <label class="form-label-mod">Industrie</label>
                <select v-model="newOrg.industry" class="form-select-mod">
                  <option value="" disabled selected>Sélectionnez une industrie</option>
                  <option>Technologie</option>
                  <option>Finance</option>
                  <option>Santé</option>
                  <option>Éducation</option>
                </select>
              </div>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-mod">Site web</label>
                <input type="url" v-model="newOrg.website" class="form-control-mod" placeholder="https://example.com">
              </div>
              <div class="col-md-6">
                <label class="form-label-mod">Ville</label>
                <input type="text" v-model="newOrg.city" class="form-control-mod" placeholder="Ville">
              </div>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-mod">Pays</label>
                <input type="text" v-model="newOrg.country" class="form-control-mod" placeholder="Pays">
              </div>
              <div class="col-md-6">
                <label class="form-label-mod">Code postal</label>
                <input type="text" v-model="newOrg.zipCode" class="form-control-mod" placeholder="12345">
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label-mod">Adresse</label>
              <input type="text" v-model="newOrg.address" class="form-control-mod" placeholder="Adresse de la rue">
            </div>

            <div class="mb-3">
              <label class="form-label-mod">Description</label>
              <textarea v-model="newOrg.description" class="form-control-mod" rows="2" placeholder="Brève description de l'organisation"></textarea>
            </div>

            <!-- SECTION 2: ADMIN -->
            <div class="section-title-mod mb-3 mt-4">
              <i class="fa-solid fa-circle-user text-info me-2"></i>
              <span class="fw-bold">Informations sur l'administrateur</span>
            </div>

            <div class="alert-info-mod mb-3 d-flex align-items-center">
              <i class="fa-solid fa-circle-info me-2"></i>
              <span>Un mot de passe sécurisé sera automatiquement généré et envoyé à l'administrateur par e-mail.</span>
            </div>

            <div class="row g-3 mb-3">
              <div class="col-md-6">
                <label class="form-label-mod">Prénom <span class="text-danger">*</span></label>
                <input type="text" v-model="newOrg.adminFirstName" class="form-control-mod" placeholder="Prénom de l'administrateur" required>
              </div>
              <div class="col-md-6">
                <label class="form-label-mod">Nom <span class="text-danger">*</span></label>
                <input type="text" v-model="newOrg.adminLastName" class="form-control-mod" placeholder="Nom de l'administrateur" required>
              </div>
            </div>

            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <label class="form-label-mod">Adresse e-mail <span class="text-danger">*</span></label>
                <input type="email" v-model="newOrg.adminEmail" class="form-control-mod" placeholder="Adresse e-mail de l'admin" required>
              </div>
              <div class="col-md-6">
                <label class="form-label-mod">Numéro de téléphone</label>
                <div class="d-flex gap-2">
                  <select class="form-select-mod" style="width:auto;min-width:100px">
                    <option>🇹🇳 +216</option>
                  </select>
                  <input type="tel" v-model="newOrg.adminPhone" class="form-control-mod" placeholder="(555) 123-4567">
                </div>
              </div>
            </div>

            <div class="modal-footer-mod border-top pt-3 d-flex justify-content-end gap-2">
              <button type="button" @click="showOrgModal = false" class="btn-cancel-mod">Annuler</button>
              <button type="submit" class="btn-submit-mod">
                <i class="fa-solid fa-check me-2"></i>Créer une organisation
              </button>
            </div>

          </form>
        </div>
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';
import Chart from 'chart.js/auto';
import { superAdminApi } from '@/services/api';
import { useToast } from 'vue-toastification';

const toast = useToast();
const showOrgModal = ref(false);
const searchQuery = ref('');

// --- STATS ---
const statsData = reactive({
  totalEntreprises: 0,
  totalUtilisateurs: 0,
  demandesEnAttente: 0,
  sessionsIARecentes: 0
});

const masterStats = ref([
  { label: 'Entreprises',  val: '0', icon: 'fa-solid fa-building',             bg: '#eef2ff', color: '#4f46e5', trend: '12%', trendUp: true,  key: 'totalEntreprises'   },
  { label: 'Sessions IA',  val: '0', icon: 'fa-solid fa-wand-magic-sparkles',  bg: '#fff7ed', color: '#f97316', trend: '24%', trendUp: true,  key: 'sessionsIARecentes' },
  { label: 'Utilisateurs', val: '0', icon: 'fa-solid fa-users',                bg: '#ecfdf5', color: '#10b981', trend: '5%',  trendUp: true,  key: 'totalUtilisateurs'  },
  { label: 'En attente',   val: '0', icon: 'fa-solid fa-shield-virus',         bg: '#fef2f2', color: '#ef4444', trend: '0%',  trendUp: false, key: 'demandesEnAttente'  }
]);

const moduleUsage = ref([
  { name: 'Évaluations',  pct: 74, color: '#6366f1' },
  { name: 'Analyses CV',  pct: 58, color: '#f97316' },
  { name: 'Entretiens IA',pct: 41, color: '#10b981' },
  { name: 'Rapports',     pct: 29, color: '#8b5cf6' }
]);

// --- FORM ---
const newOrg = reactive({
  name: '', domain: '', industry: '', website: '',
  city: '', country: '', zipCode: '', address: '',
  description: '', adminFirstName: '', adminLastName: '',
  adminEmail: '', adminPhone: ''
});

const pendingRequests = ref([]);

const filteredPendingRequests = computed(() => {
  const q = searchQuery.value.toLowerCase().trim();
  if (!q) return pendingRequests.value;
  return pendingRequests.value.filter(r =>
    r.nomEntreprise?.toLowerCase().includes(q) ||
    r.emailResponsable?.toLowerCase().includes(q) ||
    r.nomResponsable?.toLowerCase().includes(q) ||
    r.matriculeFiscale?.toLowerCase().includes(q)
  );
});

// --- API ---
const fetchData = async () => {
  try {
    const [resStats, resPending] = await Promise.all([
      superAdminApi.getStats(),
      superAdminApi.getPendingRequests()
    ]);
    Object.assign(statsData, resStats.data);
    masterStats.value.forEach(s => { s.val = statsData[s.key] || 0; });
    pendingRequests.value = resPending.data;
  } catch (err) {
    console.error('Erreur Backend:', err);
  }
};

const handleApprove = async (id) => {
  toast.info('Validation du compte...');
  try {
    await superAdminApi.approveRequest(id);
    toast.success('Entreprise activée avec succès !');
    fetchData();
  } catch { toast.error("Erreur lors de l'activation."); }
};

const handleReject = async (id) => {
  if (!confirm('Voulez-vous vraiment refuser cette demande ?')) return;
  try {
    await superAdminApi.rejectRequest(id);
    toast.success('Demande refusée.');
    fetchData();
  } catch { toast.error('Erreur.'); }
};

const handleCreateOrg = async () => {
  try {
    await superAdminApi.createOrg(newOrg);
    toast.success(`L'organisation ${newOrg.name} a été créée.`);
    showOrgModal.value = false;
    Object.keys(newOrg).forEach(k => newOrg[k] = '');
    fetchData();
  } catch { toast.error('Erreur lors de la création.'); }
};

const formatDate = (date) =>
  date ? new Date(date).toLocaleDateString('fr-FR', { day: 'numeric', month: 'short' }) : 'N/A';

onMounted(async () => {
  await fetchData();
  const ctx = document.getElementById('masterChart');
  if (ctx) {
    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['Libre', 'Occupé'],
        datasets: [{
          data: [18, 82],
          backgroundColor: ['#f1f5f9', '#6366f1'],
          borderWidth: 0,
          borderRadius: 5
        }]
      },
      options: {
        cutout: '80%',
        plugins: { legend: { display: false }, tooltip: { enabled: false } }
      }
    });
  }
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&family=Plus+Jakarta+Sans:wght@600;700;800&display=swap');

/* ─── BASE ─────────────────────────────────── */
.admin-body {
  background: #f1f5f9;
  font-family: 'Inter', sans-serif;
  color: #1e293b;
  min-height: 100vh;
}

.page-title {
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-weight: 800;
  font-size: 22px;
  letter-spacing: -0.03em;
  color: #0f172a;
}

/* ─── CARD ─────────────────────────────────── */
.card-premium {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 16px;
  box-shadow: 0 4px 24px -4px rgba(0,0,0,0.06), 0 1px 4px -1px rgba(0,0,0,0.03);
  transition: box-shadow 0.25s ease;
}
.card-premium:hover {
  box-shadow: 0 8px 32px -6px rgba(0,0,0,0.1), 0 2px 8px -2px rgba(0,0,0,0.04);
}
.card-header-premium {
  background: #fafbfc;
  border-bottom: 1px solid #e2e8f0;
  border-radius: 16px 16px 0 0;
}

/* ─── BREADCRUMB ───────────────────────────── */
.breadcrumb {
  font-size: 12px;
}
.breadcrumb-item a {
  color: #64748b;
  text-decoration: none;
}
.breadcrumb-item a:hover { color: #4f46e5; }
.breadcrumb-item.active { color: #94a3b8; }

/* ─── STATUS PILL ──────────────────────────── */
.status-pill {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  padding: 8px 16px;
  border-radius: 100px;
  font-size: 13px;
  display: flex;
  align-items: center;
  gap: 10px;
}
.pulse-dot {
  width: 8px;
  height: 8px;
  background: #10b981;
  border-radius: 50%;
  animation: pulse 2s infinite;
}
@keyframes pulse {
  0%   { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16,185,129,0.7); }
  70%  { transform: scale(1);    box-shadow: 0 0 0 8px rgba(16,185,129,0); }
  100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16,185,129,0); }
}

/* ─── BUTTON ───────────────────────────────── */
.btn-premium {
  padding: 10px 20px;
  border-radius: 12px;
  font-weight: 600;
  font-size: 14px;
  border: none;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  transition: all 0.2s;
}
.btn-premium.secondary {
  background: #0f172a;
  color: #ffffff;
}
.btn-premium.secondary:hover {
  background: #1e293b;
  transform: translateY(-1px);
  box-shadow: 0 6px 20px -4px rgba(15,23,42,0.4);
}

/* ─── STAT CARDS ───────────────────────────── */
.stat-item { padding: 22px 24px; }

.stat-icon-wrapper {
  width: 46px;
  height: 46px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 19px;
}
.stat-number {
  font-weight: 800;
  font-size: 28px;
  margin-bottom: 3px;
  letter-spacing: -0.025em;
  color: #0f172a;
}
.stat-label {
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
  letter-spacing: 0.06em;
  margin: 0;
}
.trend-badge {
  padding: 4px 9px;
  border-radius: 7px;
  font-size: 11px;
  font-weight: 700;
  display: flex;
  align-items: center;
  gap: 4px;
}
.trend-badge.up   { background: #ecfdf5; color: #059669; }
.trend-badge.down { background: #fef2f2; color: #dc2626; }

/* ─── FILTER BAR ───────────────────────────── */
.filter-bar {
  padding: 10px 18px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-radius: 12px;
}
.filter-controls { display: flex; gap: 8px; align-items: center; }

.search-box { position: relative; flex-grow: 1; max-width: 400px; }
.search-box i {
  position: absolute;
  left: 13px;
  top: 50%;
  transform: translateY(-50%);
  color: #94a3b8;
  font-size: 13px;
}
.search-box input {
  width: 100%;
  padding: 10px 14px 10px 38px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  background: #f8fafc;
  font-size: 13px;
  color: #1e293b;
  outline: none;
  transition: all 0.2s;
  font-family: 'Inter', sans-serif;
}
.search-box input:focus {
  border-color: #6366f1;
  background: #ffffff;
  box-shadow: 0 0 0 3px rgba(99,102,241,0.1);
}

.form-select-premium {
  padding: 8px 14px;
  border-radius: 9px;
  border: 1px solid #e2e8f0;
  background: #f8fafc;
  font-size: 13px;
  color: #475569;
  outline: none;
  font-family: 'Inter', sans-serif;
  cursor: pointer;
}
.form-select-premium:focus {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99,102,241,0.1);
}

.btn-refresh {
  width: 36px;
  height: 36px;
  border-radius: 9px;
  border: 1px solid #e2e8f0;
  background: #f8fafc;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  color: #64748b;
  transition: all 0.2s;
}
.btn-refresh:hover {
  background: #f1f5f9;
  border-color: #cbd5e1;
  color: #334155;
}
.btn-refresh i { font-size: 13px; }

/* ─── TABLE ─────────────────────────────────── */
.table-custom { width: 100%; border-collapse: collapse; }
.table-custom thead th {
  padding: 11px 16px;
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.07em;
  color: #94a3b8;
  background: #fafbfc;
  border-bottom: 1px solid #f1f5f9;
}
.table-custom tbody tr {
  border-bottom: 1px solid #f8fafc;
  transition: background 0.12s;
}
.table-custom tbody tr:last-child { border-bottom: none; }
.table-custom tbody tr:hover { background: #f8fafc; }
.table-custom td { padding: 14px 16px; vertical-align: middle; }

.avatar-letter {
  width: 36px;
  height: 36px;
  min-width: 36px;
  background: #eef2ff;
  color: #4338ca;
  border-radius: 9px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 800;
  font-size: 13px;
  border: 1px solid #e0e7ff;
}

.text-indigo { color: #6366f1; }
.tiny        { font-size: 11px; }

.badge-count-pro {
  background: #eef2ff;
  color: #4338ca;
  font-size: 12px;
  font-weight: 600;
  padding: 4px 12px;
  border-radius: 7px;
}

/* ─── ACTION BUTTONS ───────────────────────── */
.btn-circle {
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.15s;
  font-size: 12px;
}
.btn-circle.approve { background: #ecfdf5; color: #10b981; }
.btn-circle.approve:hover { background: #d1fae5; transform: scale(1.08); }
.btn-circle.reject  { background: #fef2f2; color: #ef4444; }
.btn-circle.reject:hover  { background: #fee2e2; transform: scale(1.08); }

/* ─── CHART ─────────────────────────────────── */
.chart-container {
  position: relative;
  height: 190px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.chart-center-text {
  position: absolute;
  text-align: center;
  pointer-events: none;
}
.chart-center-text .number {
  font-size: 24px;
  font-weight: 800;
  display: block;
  color: #0f172a;
}
.chart-center-text .label {
  font-size: 10px;
  font-weight: 700;
  color: #94a3b8;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

/* ─── DOT INDICATORS ───────────────────────── */
.dot-sm {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  display: inline-block;
  flex-shrink: 0;
}
.bg-indigo  { background: #6366f1; }
.bg-orange  { background: #f97316; }

/* ─── PROGRESS BARS ─────────────────────────── */
.prog-bar-bg {
  height: 5px;
  background: #f1f5f9;
  border-radius: 3px;
  overflow: hidden;
}
.prog-bar {
  height: 5px;
  border-radius: 3px;
  transition: width 0.6s ease;
}

/* ─── MODAL ─────────────────────────────────── */
.modal-backdrop-custom {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  backdrop-filter: blur(5px);
  z-index: 9999;
  display: flex;
  align-items: flex-start;
  justify-content: center;
  padding: 40px 12px;
  overflow-y: auto;
}
.modal-content-pro.modal-wide {
  background: #ffffff;
  width: 95%;
  max-width: 860px;
  border-radius: 16px;
  padding: 30px;
  box-shadow: 0 24px 60px -8px rgba(0,0,0,0.25), 0 8px 24px -4px rgba(0,0,0,0.1);
  animation: modal-in 0.25s ease;
}
@keyframes modal-in {
  from { opacity: 0; transform: translateY(16px) scale(0.99); }
  to   { opacity: 1; transform: translateY(0)   scale(1); }
}
.btn-close-modal {
  border: none;
  background: #f1f5f9;
  color: #64748b;
  font-size: 16px;
  width: 34px;
  height: 34px;
  border-radius: 8px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.15s;
}
.btn-close-modal:hover { background: #fee2e2; color: #ef4444; }

/* ─── MODAL FORM ─────────────────────────────── */
.section-title-mod {
  font-size: 14px;
  color: #1e293b;
  border-bottom: 1px solid #f1f5f9;
  padding-bottom: 10px;
  display: flex;
  align-items: center;
}
.form-label-mod {
  font-size: 13px;
  font-weight: 600;
  color: #334155;
  margin-bottom: 6px;
  display: block;
}
.form-control-mod,
.form-select-mod {
  width: 100%;
  padding: 10px 14px;
  border-radius: 9px;
  border: 1px solid #e2e8f0;
  font-size: 13px;
  color: #1e293b;
  background: #f8fafc;
  outline: none;
  transition: all 0.2s;
  font-family: 'Inter', sans-serif;
}
.form-control-mod:focus,
.form-select-mod:focus {
  border-color: #f59e0b;
  background: #ffffff;
  box-shadow: 0 0 0 3px rgba(245,158,11,0.12);
}
textarea.form-control-mod { resize: vertical; }

.alert-info-mod {
  background: #f0f9ff;
  color: #0369a1;
  border: 1px solid #bae6fd;
  border-radius: 9px;
  padding: 11px 14px;
  font-size: 12.5px;
  font-weight: 500;
}
.modal-footer-mod { margin-top: 8px; }

.btn-cancel-mod {
  background: #ffffff;
  color: #475569;
  border: 1px solid #d1d5db;
  padding: 10px 22px;
  border-radius: 9px;
  font-weight: 600;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.15s;
  font-family: 'Inter', sans-serif;
}
.btn-cancel-mod:hover { background: #f8fafc; border-color: #94a3b8; }

.btn-submit-mod {
  background: #fbbf24;
  color: #000000;
  border: none;
  padding: 10px 26px;
  border-radius: 9px;
  font-weight: 700;
  font-size: 13px;
  cursor: pointer;
  transition: all 0.2s;
  font-family: 'Inter', sans-serif;
  display: flex;
  align-items: center;
}
.btn-submit-mod:hover {
  background: #f59e0b;
  transform: translateY(-1px);
  box-shadow: 0 6px 18px -3px rgba(245,158,11,0.45);
}

/* ─── TRANSITIONS ───────────────────────────── */
.slide-up-enter-active,
.slide-up-leave-active { transition: all 0.28s ease; }
.slide-up-enter-from   { opacity: 0; transform: translateY(18px); }
.slide-up-leave-to     { opacity: 0; transform: translateY(12px); }
</style>