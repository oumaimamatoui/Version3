<template>
  <div class="elite-roles-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
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
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Sécurité & Accès</span>
              </div>
              <h2 class="premium-title">Rôles &amp; <span class="gradient-text">Privilèges</span></h2>
              <p class="subtitle">Gestion des accréditations et protocoles d'accès sécurisés.</p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <button class="btn-refresh-pro" @click="fetchData" :disabled="loading" title="Synchroniser">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <button @click="openAddModal" class="btn-enigma-primary shadow-premium">
                <div class="btn-content">
                  <i class="fa-solid fa-shield-plus me-2"></i>DÉPLOYER UN RÔLE
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- ══════════════════════════════════════
               SECTION 2 — KPI STATS
          ══════════════════════════════════════ -->
          <div class="row g-4 mb-5">
            <div class="col-md-4" v-for="stat in stats" :key="stat.label">
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
                placeholder="Rechercher un rôle, une permission..."
              />
              <transition name="fade">
                <span v-if="searchQuery" class="search-badge">{{ filteredRoles.length }}</span>
              </transition>
              <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                <i class="fa-solid fa-xmark"></i>
              </button>
            </div>
            <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border">
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
            <p class="loading-text mt-3">CHARGEMENT DES ACCRÉDITATIONS...</p>
          </div>

          <!-- ══════════════════════════════════════
               SECTION 5 — GRILLE DES RÔLES
          ══════════════════════════════════════ -->
          <div v-else class="row g-4">

            <div v-if="filteredRoles.length === 0" class="col-12">
              <div class="empty-state-pro py-5 text-center">
                <i class="fa-solid fa-shield-halved fa-3x text-muted mb-3"></i>
                <h5 class="fw-800">Aucun rôle trouvé</h5>
                <p class="text-muted">Créez votre premier rôle ou modifiez les filtres.</p>
                <button class="btn-enigma-primary mt-3" @click="openAddModal">
                  <div class="btn-content"><i class="fa-solid fa-plus me-2"></i>Déployer un rôle</div>
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
                    <ul class="dropdown-menu border-0 shadow-premium p-2 rounded-4">
                      <li>
                        <button class="dropdown-item rounded-3" @click="openEditModal(role)">
                          <i class="fa-solid fa-pen-to-square me-2 text-amber"></i>Modifier
                        </button>
                      </li>
                      <li><hr class="dropdown-divider"></li>
                      <li>
                        <button class="dropdown-item rounded-3 text-danger" @click="confirmDelete(role.id)">
                          <i class="fa-solid fa-trash-can me-2"></i>Révoquer
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
                    <p class="role-desc m-0 text-truncate">{{ role.description || 'Accès sécurisé défini.' }}</p>
                  </div>
                </div>

                <!-- Barre permissions -->
                <div class="perm-progress-box mb-3">
                  <div class="d-flex justify-content-between mb-1">
                    <span class="micro-label">COUVERTURE DES DROITS</span>
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
                    Aucune permission
                  </span>
                </div>

                <!-- Footer -->
                <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3 border-top border-light">
                  <span class="members-chip">
                    <i class="fa-solid fa-user-shield me-2"></i>{{ role.nombreMembres || 0 }} membre(s)
                  </span>
                  <button @click="openEditModal(role)" class="btn-edit-link">
                    CONFIGURER <i class="fa-solid fa-arrow-right-long ms-1"></i>
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
                <h3 class="modal-title-v2">{{ isEditing ? 'MISE À JOUR DU RÔLE' : 'NOUVELLE ACCRÉDITATION' }}</h3>
                <p class="modal-sub-v2">PROTOCOLE DE SÉCURITÉ V2.0</p>
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
              <div class="section-badge mb-4"><span>01</span> Paramètres de base</div>
              <div class="row g-4">
                <div class="col-md-6">
                  <div class="enigma-input-wrap">
                    <label>NOM DU RÔLE</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-id-badge"></i>
                      <input v-model="form.nom" type="text" class="enigma-field" placeholder="ex: Lead Développeur" />
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="enigma-input-wrap">
                    <label>MODÈLE DE RÔLE</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-layer-group"></i>
                      <select v-model="selectedTemplate" @change="applyTemplate" class="enigma-field" :disabled="isEditing">
                        <option value="">Rôle personnalisé</option>
                        <option value="manager">Manager — Accès complet</option>
                        <option value="hr">RH — Recrutement & Staff</option>
                        <option value="evaluator">Évaluateur (Correcteur technique)</option>
                        <option value="viewer">Lecteur — Lecture seule</option>
                      </select>
                    </div>
                  </div>
                </div>
                <div class="col-12" v-if="!isEditing">
                  <div class="enigma-input-wrap">
                    <label>INVITER UN RESPONSABLE (Optionnel)</label>
                    <div class="input-icon-wrap">
                      <i class="fa-solid fa-envelope-open-text"></i>
                      <input v-model="form.email" type="email" class="enigma-field" placeholder="responsable@societe.pro" />
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- 02 Matrice droits -->
            <div class="form-section-card">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="section-badge m-0"><span>02</span> MATRICE DES DROITS</div>
                <div class="perm-count-pill">
                  <i class="fa-solid fa-lock-open me-1"></i>
                  {{ form.permissions.length }} / {{ totalPerms }} actifs
                </div>
              </div>

              <div v-for="group in permissionGroups" :key="group.title" class="perm-group mb-3">
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
            <button @click="closeModal" class="btn-qv-cancel">ANNULER</button>
            <button @click="saveRole" class="btn-enigma-primary px-5" :disabled="saving">
              <div class="btn-content" v-if="!saving">
                <i class="fa-solid fa-unlock-keyhole me-2"></i>
                {{ isEditing ? 'METTRE À JOUR' : 'DÉPLOYER LE RÔLE' }}
              </div>
              <div v-else class="btn-content">
                <span class="spinner-border spinner-border-sm me-2"></span>Déploiement...
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
          <strong>SYSTEM MESSAGE</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

/* ─── STATE ────────────────────────────────────────────────── */
const roles         = ref([]);
const stats         = ref([]);
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

/* ─── PERMISSIONS ──────────────────────────────────────────── */
const permissionGroups = [
  {
    title: 'CANDIDATS',
    items: [
      { id: 'view_can',  label: 'Accès Liste',  desc: 'Lecture des profils' },
      { id: 'inv_can',   label: 'Invitations',  desc: 'Envoi des tests' }
    ]
  },
  {
    title: 'ÉVALUATIONS & CONTENU',
    items: [
      { id: 'view_tests', label: 'Audit Tests',       desc: 'Lecture des tests' },
      { id: 'grade_tests',label: 'Évaluation',        desc: 'Attribution des scores' },
      { id: 'edit_bank',  label: 'Banque de questions',desc: 'Gérer les questions' }
    ]
  },
  {
    title: 'ADMINISTRATION',
    items: [
      { id: 'add_rol',   label: 'Gérer Rôles', desc: 'Création de rôles' },
      { id: 'add_staff', label: 'Gérer Staff',  desc: 'Nouveaux membres' }
    ]
  }
];

const totalPerms = computed(() =>
  permissionGroups.reduce((a, g) => a + g.items.length, 0)
);

/* ─── TABS ─────────────────────────────────────────────────── */
const filterTabs = computed(() => [
  { label: 'Tous',        value: 'all',    count: filteredBySearch.value.length },
  { label: 'Plein accès', value: 'full',   count: filteredBySearch.value.filter(r => (r.permissions?.length || 0) >= totalPerms.value).length },
  { label: 'Restreints',  value: 'limited',count: filteredBySearch.value.filter(r => (r.permissions?.length || 0) < totalPerms.value).length },
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
const getProgressColor = (p) => p >= 80 ? '#10b981' : p >= 40 ? '#f59e0b' : '#6366f1';

const getAccessLabel = (role) => {
  const n = role.permissions?.length || 0;
  if (n >= totalPerms.value) return 'Accès total';
  if (n >= 4)                return 'Accès étendu';
  if (n >= 1)                return 'Accès restreint';
  return 'Aucun accès';
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
    const [rolesRes, statsRes] = await Promise.all([
      api.get('/Roles'),
      api.get('/Roles/stats')
    ]);
    roles.value = rolesRes.data;
    stats.value = statsRes.data ? statsRes.data : [
      { label: 'Rôles actifs',   value: rolesRes.data.length,                              icon: 'fa-solid fa-shield-halved', color: '#f59e0b', bg: '#fffbeb', trend: 5  },
      { label: 'Membres gérés',  value: rolesRes.data.reduce((a, r) => a + (r.nombreMembres || 0), 0), icon: 'fa-solid fa-users',         color: '#6366f1', bg: '#eef2ff', trend: 8  },
      { label: 'Permissions',    value: totalPerms.value,                                  icon: 'fa-solid fa-lock',          color: '#10b981', bg: '#ecfdf5', trend: 0  },
    ];
  } catch (err) {
    console.error(err);
    showPulseToast('Erreur de chargement', 'error', 'fa-solid fa-triangle-exclamation');
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
  if (!form.nom) return showPulseToast('Nom de rôle requis.', 'warn', 'fa-solid fa-triangle-exclamation');
  saving.value = true;
  try {
    if (isEditing.value) await api.put(`/Roles/${form.id}`, form);
    else                 await api.post('/Roles', form);
    showPulseToast(isEditing.value ? 'Rôle mis à jour.' : 'Rôle déployé.', 'success', 'fa-solid fa-shield-check');
    await fetchData();
    closeModal();
  } catch (e) {
    showPulseToast('Échec : ' + (e.response?.data?.message || 'Erreur serveur'), 'error', 'fa-solid fa-circle-xmark');
  } finally { saving.value = false; }
};

const confirmDelete = async (id) => {
  if (confirm('Révoquer ce rôle définitivement ?')) {
    try {
      await api.delete(`/Roles/${id}`);
      showPulseToast('Rôle révoqué.', 'warn', 'fa-solid fa-trash-can');
      fetchData();
    } catch {
      showPulseToast('Ce rôle est en cours d\'utilisation.', 'error', 'fa-solid fa-circle-xmark');
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

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&display=swap');

/* ══════════════════════════════════
   BASE
══════════════════════════════════ */
.elite-roles-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: #f8fafc;
  color: #0f172a;
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
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.2;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.15; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }

/* ══════════════════════════════════
   LAYOUT
══════════════════════════════════ */
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ══════════════════════════════════
   EN-TÊTE
══════════════════════════════════ */
.premium-title {
  font-weight: 900; font-size: 2.2rem; letter-spacing: -1.5px; margin: 0;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle { color: #64748b; font-size: 14px; margin-top: 6px; margin-bottom: 0; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; transition: color 0.2s; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* ══════════════════════════════════
   BOUTONS
══════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  font-size: 13px; position: relative; overflow: hidden;
  cursor: pointer; font-family: inherit; transition: transform 0.2s;
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: opacity 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2; display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }

.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

.btn-refresh-pro {
  width: 44px; height: 44px; background: white;
  border: 1.5px solid #e2e8f0; border-radius: 14px;
  color: #64748b; cursor: pointer; transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
}
.btn-refresh-pro:hover:not(:disabled) {
  background: #f8fafc; border-color: #f59e0b; color: #f59e0b;
  transform: rotate(180deg) scale(1.1);
}

/* ══════════════════════════════════
   KPI STATS
══════════════════════════════════ */
.stat-card-premium {
  background: white; border-radius: 24px; padding: 24px;
  display: flex; align-items: center; border: 1px solid #eef2f6;
  transition: 0.3s cubic-bezier(0.4,0,0.2,1);
}
.stat-card-premium:hover { transform: translateY(-4px); box-shadow: 0 12px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper {
  width: 56px; height: 56px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.4rem; flex-shrink: 0;
}
.stat-details  { margin-left: 16px; }
.stat-value    { font-size: 1.7rem; font-weight: 900; line-height: 1; color: #0f172a; }
.stat-label    { font-size: 0.65rem; font-weight: 800; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; margin-top: 4px; }
.stat-trend    { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up      { color: #10b981; }
.trend-down    { color: #f43f5e; }

/* ══════════════════════════════════
   RECHERCHE & TABS
══════════════════════════════════ */
.search-inline-box {
  display: flex; align-items: center; background: white;
  border: 1.5px solid #eef2f6; border-radius: 14px;
  padding: 0 14px; gap: 10px; color: #94a3b8;
  transition: border-color 0.2s, box-shadow 0.2s;
}
.search-inline-box:focus-within {
  border-color: #f59e0b;
  box-shadow: 0 0 0 4px rgba(251,191,36,0.1);
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 12px 0; font-weight: 700; font-size: 0.85rem;
  flex: 1; color: #0f172a; font-family: inherit;
}
.search-inline-input::placeholder { color: #cbd5e1; }
.search-badge {
  background: #fbbf24; color: #0f172a; font-size: 11px;
  font-weight: 900; padding: 3px 10px; border-radius: 99px;
}
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; font-size: 13px; }

.nav-tab-btn-modern {
  padding: 8px 16px; border-radius: 12px; border: none;
  background: transparent; font-weight: 800; font-size: 0.78rem;
  color: #94a3b8; cursor: pointer; transition: 0.2s;
  font-family: inherit;
}
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.tab-count {
  background: rgba(255,255,255,0.2); padding: 2px 7px;
  border-radius: 8px; font-size: 0.65rem; margin-left: 6px;
}
.nav-tab-btn-modern:not(.active) .tab-count { background: #f1f5f9; color: #64748b; }

/* ══════════════════════════════════
   LOADER
══════════════════════════════════ */
.spinner-pro-premium {
  width: 50px; height: 50px; border: 4px solid #f1f5f9;
  border-top: 4px solid #f59e0b; border-radius: 50%;
  animation: spin 1s linear infinite; margin: 40px auto;
}
@keyframes spin { to { transform: rotate(360deg); } }
.loading-text { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }

/* ══════════════════════════════════
   CARTES RÔLES
══════════════════════════════════ */
.role-card-modern {
  background: white; border-radius: 30px; padding: 28px;
  border: 1px solid #eef2f6; height: 100%;
  transition: 0.3s cubic-bezier(0.4,0,0.2,1); cursor: default;
  box-shadow: 0 2px 8px rgba(0,0,0,0.02);
}
.role-card-modern:hover {
  transform: translateY(-8px); border-color: #f59e0b;
  box-shadow: 0 25px 50px -12px rgba(251,191,36,0.12), 0 8px 24px rgba(0,0,0,0.06);
}

.access-level-badge {
  padding: 5px 12px; border-radius: 10px;
  font-size: 0.65rem; font-weight: 800;
  display: inline-flex; align-items: center; gap: 6px;
}
.access-full  { background: #ecfdf5; color: #059669; }
.access-high  { background: #fffbeb; color: #d97706; }
.access-low   { background: #eff6ff; color: #3b82f6; }
.access-none  { background: #f1f5f9; color: #64748b; }
.status-dot   { width: 6px; height: 6px; border-radius: 50%; background: currentColor; }

.btn-options-round {
  width: 34px; height: 34px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  color: #94a3b8; cursor: pointer; transition: 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.btn-options-round:hover { background: #0f172a; color: #f59e0b; border-color: #0f172a; }

.role-squircle {
  width: 52px; height: 52px; background: #0f172a; color: #fbbf24;
  border-radius: 17px; display: flex; align-items: center;
  justify-content: center; font-size: 1.2rem; flex-shrink: 0;
}
.role-name { font-size: 1.1rem; font-weight: 900; color: #0f172a; letter-spacing: -0.3px; }
.role-desc { color: #64748b; font-size: 13px; max-width: 180px; }
.min-width-0 { min-width: 0; }

/* Progress */
.perm-progress-box { background: #f8fafc; border-radius: 14px; padding: 14px 16px; }
.progress-slim  { height: 5px; background: #e2e8f0; border-radius: 10px; overflow: hidden; }
.progress-fill  { height: 100%; border-radius: 10px; transition: width 0.6s ease; }
.micro-label    { font-size: 9px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; text-transform: uppercase; }
.micro-label.text-amber { color: #f59e0b; }

/* Tags */
.perm-tags  { display: flex; flex-wrap: wrap; gap: 6px; }
.perm-pill  {
  font-size: 9px; font-weight: 800; padding: 5px 12px;
  background: #f8fafc; border: 1px solid #e2e8f0;
  border-radius: 99px; color: #64748b; white-space: nowrap;
}
.perm-pill.more  { background: #fffbeb; border-color: #fde68a; color: #d97706; }
.perm-pill.empty { background: #f1f5f9; border-color: #e2e8f0; color: #94a3b8; font-style: italic; }

/* Footer carte */
.card-footer-modern { display: flex; justify-content: space-between; align-items: center; }
.members-chip {
  font-size: 12px; font-weight: 600; color: #64748b;
  background: #f8fafc; padding: 6px 14px;
  border-radius: 99px; border: 1px solid #f1f5f9;
}
.btn-edit-link {
  background: none; border: none; font-size: 10px; font-weight: 900;
  color: #f59e0b; cursor: pointer; letter-spacing: 0.8px;
  transition: color 0.2s; padding: 0;
}
.btn-edit-link:hover { color: #d97706; }

/* Empty state */
.empty-state-pro {
  background: white; border-radius: 30px;
  padding: 40px; border: 1px dashed #e2e8f0;
}

/* Dropdown */
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }
.dropdown-menu { border-radius: 20px !important; }
.dropdown-item { font-weight: 700; font-size: 13.5px; }
.text-amber { color: #f59e0b !important; }

/* ══════════════════════════════════
   MODALE
══════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; z-index: 2000;
  background: rgba(15,23,42,0.65); backdrop-filter: blur(16px);
  display: flex; align-items: center; justify-content: center; padding: 20px;
}
.role-modal-window {
  background: white; border-radius: 40px;
  width: 100%; max-width: 860px; max-height: 90vh;
  display: flex; flex-direction: column; overflow: hidden;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.15);
  border: 1px solid rgba(255,255,255,0.9);
}

/* Head modale */
.modal-head-v2 {
  padding: 28px 36px; display: flex;
  justify-content: space-between; align-items: center;
  background: #fafafa; flex-shrink: 0;
}
.modal-brand-icon {
  width: 50px; height: 50px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 16px; display: flex; align-items: center;
  justify-content: center; color: #0f172a; font-size: 1.2rem;
  box-shadow: 0 8px 20px rgba(251,191,36,0.3); flex-shrink: 0;
}
.modal-title-v2 { font-size: 1.05rem; font-weight: 900; color: #0f172a; margin: 0; letter-spacing: 0.5px; }
.modal-sub-v2   { font-size: 9.5px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; margin: 4px 0 0; }
.header-accent-line { height: 4px; background: linear-gradient(90deg, #fbbf24, #f59e0b); flex-shrink: 0; }

.btn-close-modal {
  width: 40px; height: 40px; background: #f8fafc;
  border: 1px solid #f1f5f9; border-radius: 14px;
  color: #94a3b8; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 14px; transition: all 0.3s;
}
.btn-close-modal:hover { background: #fee2e2; color: #e11d48; border-color: #fecaca; }

/* Body */
.modal-body-scroll {
  padding: 28px 36px; overflow-y: auto; flex: 1;
  background: #fcfdfe;
}
.modal-body-scroll::-webkit-scrollbar { width: 5px; }
.modal-body-scroll::-webkit-scrollbar-track { background: transparent; }
.modal-body-scroll::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 99px; }

/* Sections formulaire */
.form-section-card {
  background: white; border-radius: 28px;
  border: 1px solid #f1f5f9; padding: 28px;
}
.section-badge {
  display: inline-flex; align-items: center; gap: 12px;
  font-size: 11px; font-weight: 800; color: #0f172a;
  letter-spacing: 1px; text-transform: uppercase;
}
.section-badge span {
  width: 28px; height: 28px; background: #0f172a; color: white;
  border-radius: 9px; display: flex; align-items: center;
  justify-content: center; font-size: 12px; font-weight: 800; flex-shrink: 0;
}

/* Inputs */
.enigma-input-wrap label {
  font-size: 10px; font-weight: 800; color: #94a3b8;
  letter-spacing: 1px; text-transform: uppercase;
  margin-bottom: 10px; display: block;
}
.input-icon-wrap { position: relative; display: flex; align-items: center; }
.input-icon-wrap i {
  position: absolute; left: 18px; color: #fbbf24;
  font-size: 14px; pointer-events: none;
}
.enigma-field {
  width: 100%; padding: 16px 20px 16px 50px;
  border-radius: 18px; border: 1.5px solid #f1f5f9;
  background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif;
  font-size: 14px; font-weight: 600; color: #0f172a;
  outline: none; appearance: none; -webkit-appearance: none;
  transition: all 0.35s cubic-bezier(0.175,0.885,0.32,1.275);
}
.enigma-field:focus {
  border-color: #fbbf24; background: white;
  box-shadow: 0 10px 24px rgba(251,191,36,0.1);
  transform: translateY(-2px);
}
.enigma-field:disabled { opacity: 0.45; cursor: not-allowed; }
.enigma-field::placeholder { color: #cbd5e1; }

/* Permissions */
.perm-count-pill {
  background: #fffbeb; color: #d97706;
  border: 1px solid #fde68a; font-size: 11px; font-weight: 800;
  padding: 5px 14px; border-radius: 99px;
}
.perm-group {
  background: #f8fafc; border-radius: 20px;
  padding: 18px; border: 1px solid #f1f5f9;
}
.group-label {
  font-size: 9.5px; font-weight: 800; color: #94a3b8;
  letter-spacing: 2.5px; margin-bottom: 12px;
  display: flex; align-items: center; gap: 8px;
}
.group-dot { width: 6px; height: 6px; border-radius: 50%; background: #fbbf24; flex-shrink: 0; }

.perm-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 10px;
}
.perm-node {
  padding: 16px; border-radius: 16px; border: 1.5px solid #f1f5f9;
  background: white; display: flex; gap: 14px; cursor: pointer;
  user-select: none; align-items: flex-start;
  transition: all 0.3s cubic-bezier(0.175,0.885,0.32,1.275);
}
.perm-node:hover { border-color: #e2e8f0; transform: translateY(-2px); box-shadow: 0 6px 16px rgba(0,0,0,0.04); }
.perm-node.active { border-color: #fbbf24; background: #fffbeb; box-shadow: 0 8px 24px rgba(251,191,36,0.14); }
.node-checkbox {
  width: 22px; height: 22px; border: 2px solid #cbd5e1;
  border-radius: 8px; display: flex; align-items: center;
  justify-content: center; color: white; font-size: 11px;
  flex-shrink: 0; transition: all 0.2s;
}
.active .node-checkbox { background: #fbbf24; border-color: #fbbf24; }
.node-name { display: block; font-weight: 800; font-size: 13px; color: #0f172a; line-height: 1.3; }
.node-desc { display: block; font-size: 11px; color: #94a3b8; margin-top: 2px; }

/* Footer modale */
.modal-foot-v2 {
  padding: 20px 36px; border-top: 1px solid #f1f5f9;
  display: flex; justify-content: flex-end; gap: 14px;
  background: #fafafa; flex-shrink: 0;
}
.btn-qv-cancel {
  background: #f1f5f9; color: #64748b; border: none;
  padding: 13px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit; font-size: 13px;
  transition: background 0.2s;
}
.btn-qv-cancel:hover { background: #e2e8f0; }

/* ══════════════════════════════════
   TOAST
══════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 18px 28px;
  border-radius: 20px; display: flex; align-items: center;
  gap: 15px; z-index: 9999; border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.t-ico { font-size: 1.1rem; }
.t-body strong { font-size: 0.65rem; letter-spacing: 1.5px; opacity: 0.6; display: block; margin-bottom: 2px; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* ══════════════════════════════════
   TRANSITIONS
══════════════════════════════════ */
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.93); } to { opacity: 1; transform: scale(1); } }

.fade-enter-active, .fade-leave-active { transition: opacity 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* ══════════════════════════════════
   DARK MODE
══════════════════════════════════ */
[data-theme="dark"] .elite-roles-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .canvas-engine { background: #0d1117; }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }
[data-theme="dark"] .subtitle { color: #8b949e; }

[data-theme="dark"] .stat-card-premium { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }

[data-theme="dark"] .bg-white { background: #161b22 !important; border-color: rgba(255,255,255,0.08) !important; }
[data-theme="dark"] .nav-tab-btn-modern { color: #8b949e; }
[data-theme="dark"] .nav-tab-btn-modern.active { background: #f0f6fc; color: #0d1117; }

[data-theme="dark"] .search-inline-box { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }

[data-theme="dark"] .role-card-modern { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .role-card-modern:hover { border-color: #d97706; }
[data-theme="dark"] .role-name { color: #f0f6fc; }
[data-theme="dark"] .role-desc { color: #8b949e; }
[data-theme="dark"] .perm-progress-box { background: rgba(255,255,255,0.04); }
[data-theme="dark"] .progress-slim { background: rgba(255,255,255,0.08); }
[data-theme="dark"] .perm-pill { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .members-chip { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }

[data-theme="dark"] .role-modal-window { background: #0d1117; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .modal-head-v2 { background: #161b22; }
[data-theme="dark"] .modal-title-v2 { color: #f0f6fc; }
[data-theme="dark"] .modal-body-scroll { background: #0d1117; }
[data-theme="dark"] .form-section-card { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .enigma-field { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .enigma-field:focus { background: rgba(255,255,255,0.08); border-color: #fbbf24; }
[data-theme="dark"] .perm-group { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .perm-node { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .perm-node.active { background: rgba(251,191,36,0.1); border-color: #fbbf24; }
[data-theme="dark"] .node-name { color: #f0f6fc; }
[data-theme="dark"] .modal-foot-v2 { background: #161b22; border-color: rgba(255,255,255,0.06); }

[data-theme="dark"] .dropdown-menu { background: #161b22 !important; border-color: rgba(255,255,255,0.08) !important; }
[data-theme="dark"] .dropdown-item { color: #f0f6fc; }
[data-theme="dark"] .dropdown-item:hover { background: rgba(255,255,255,0.06); }

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