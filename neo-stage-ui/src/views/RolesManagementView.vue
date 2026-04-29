<template>
  <div class="elite-roles-root">
    <!-- Fond identique au Login -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport flex-grow-1">
      <AppNavbar />

      <div class="container-fluid p-4 p-lg-5 content-overlay animate__animated animate__fadeIn">

        <!-- ==========================================
             SECTION 1 : EN-TÊTE
             ========================================== -->
        <header class="page-header mb-5">
          <div class="header-info">
            <nav class="elite-breadcrumb mb-2">
              ADMINISTRATION <span class="sep">/</span> SÉCURITÉ_ACCÈS
            </nav>
            <h1 class="main-title">Rôles & <span class="text-amber-gradient">Privilèges</span></h1>
            <p class="subtitle">Gestion des accréditations et protocoles d'accès sécurisés.</p>
          </div>
          <div class="header-actions d-flex gap-3">
            <button @click="fetchData" class="btn-secondary-elite" title="Actualiser">
              <i class="fa-solid fa-sync me-2"></i>SYNCHRONISER
            </button>
            <button @click="openAddModal" class="btn-sunburst">
              <div class="btn-label">
                <i class="fa-solid fa-shield-plus me-2"></i>DÉPLOYER UN RÔLE
              </div>
              <div class="shine-sweep"></div>
            </button>
          </div>
        </header>

        <!-- ==========================================
             SECTION 2 : STATISTIQUES
             ========================================== -->
        <div class="row g-4 mb-5">
          <div class="col-md-4" v-for="stat in stats" :key="stat.label">
            <div class="stat-bento-card">
              <div class="stat-icon-wrap" :style="{ background: stat.bg, color: stat.color }">
                <i :class="stat.icon"></i>
              </div>
              <div class="stat-body">
                <span class="stat-label">{{ stat.label.toUpperCase() }}</span>
                <span class="stat-value">{{ stat.value }}</span>
              </div>
              <div class="stat-glow" :style="{ background: stat.bg }"></div>
            </div>
          </div>
        </div>

        <!-- ==========================================
             SECTION 3 : RECHERCHE
             ========================================== -->
        <div class="search-vessel mb-5">
          <div class="search-icon-pill">
            <i class="fa-solid fa-magnifying-glass"></i>
          </div>
          <input
            type="text"
            v-model="searchQuery"
            class="search-input"
            placeholder="Rechercher un rôle..."
          />
          <transition name="fade">
            <span v-if="searchQuery" class="search-badge">{{ filteredRoles.length }}</span>
          </transition>
        </div>

        <!-- ==========================================
             SECTION 4 : LISTE DES RÔLES
             ========================================== -->
        <div v-if="loading" class="loading-zone text-center py-5">
          <div class="bot-loader-wrap">
            <svg viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg" class="loader-bot">
              <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping" />
              <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
              <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
              <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
              <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
              <line x1="100" y1="40" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
              <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-core" />
            </svg>
          </div>
          <p class="loading-text mt-3">CHARGEMENT DES ACCRÉDITATIONS...</p>
        </div>

        <div v-else class="row g-4">
          <div
            class="col-xl-4 col-md-6"
            v-for="(role, idx) in filteredRoles"
            :key="role.id"
            :style="{ animationDelay: idx * 0.08 + 's' }"
          >
            <div class="role-card animate__animated animate__fadeInUp">
              <div class="card-shine"></div>

              <div class="card-top d-flex justify-content-between align-items-start">
                <div class="role-squircle">
                  <i class="fa-solid fa-fingerprint"></i>
                </div>
                <div class="dropdown">
                  <button class="btn-dots" data-bs-toggle="dropdown">
                    <i class="fa-solid fa-ellipsis-vertical"></i>
                  </button>
                  <ul class="dropdown-menu dropdown-premium border-0">
                    <li>
                      <a @click="openEditModal(role)" class="dropdown-item">
                        <i class="fa-solid fa-pen-nib me-2 text-amber"></i>Modifier
                      </a>
                    </li>
                    <li><hr class="dropdown-divider my-1"></li>
                    <li>
                      <a @click="confirmDelete(role.id)" class="dropdown-item text-danger">
                        <i class="fa-solid fa-trash-can me-2"></i>Révoquer
                      </a>
                    </li>
                  </ul>
                </div>
              </div>

              <div class="card-body-content mt-4">
                <h3 class="role-name">{{ role.nom }}</h3>
                <p class="role-desc text-truncate">{{ role.description || 'Accès sécurisé défini.' }}</p>
              </div>

              <div class="perm-tags mt-4">
                <span
                  v-for="p in role.permissions?.slice(0, 3)"
                  :key="p"
                  class="perm-pill"
                >{{ p.replace('_', ' ').toUpperCase() }}</span>
                <span v-if="role.permissions?.length > 3" class="perm-pill more">
                  +{{ role.permissions.length - 3 }}
                </span>
              </div>

              <footer class="card-footer-row mt-4 pt-3">
                <span class="members-chip">
                  <i class="fa-solid fa-user-shield me-2"></i>{{ role.nombreMembres }} Membres
                </span>
                <button @click="openEditModal(role)" class="btn-edit-link">
                  MODIFIER <i class="fa-solid fa-arrow-right-long ms-1"></i>
                </button>
              </footer>
            </div>
          </div>
        </div>

      </div>
    </div>

    <!-- ==========================================
         SECTION 5 : MODALE
         ========================================== -->
    <Transition name="fade-slide" mode="out-in">
      <div v-if="showModal" class="modal-overlay" @click.self="closeModal">
        <div class="modal-bento animate__animated animate__fadeInUp">

          <header class="modal-head">
            <div class="modal-brand">
              <div class="modal-brand-icon">
                <i class="fa-solid fa-shield-halved"></i>
              </div>
              <div>
                <h3 class="modal-title">{{ isEditing ? 'MISE À JOUR' : 'NOUVELLE ACCRÉDITATION' }}</h3>
                <p class="modal-sub">PROTOCOLE DE SÉCURITÉ V2.0</p>
              </div>
            </div>
            <button @click="closeModal" class="btn-close-modal">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </header>

          <div class="header-line-modal"></div>

          <div class="modal-body-scroll">

            <!-- 01 Paramètres -->
            <div class="form-section mb-4">
              <div class="section-badge mb-4"><span>01</span> Paramètres de base</div>
              <div class="row g-4">
                <div class="col-md-6">
                  <label class="field-label">NOM DU RÔLE</label>
                  <div class="input-glow-wrapper">
                    <i class="fa-solid fa-id-badge"></i>
                    <input v-model="form.nom" type="text" class="field-input" placeholder="ex: Lead Développeur" />
                  </div>
                </div>
                <div class="col-md-6">
                  <label class="field-label">MODÈLE DE RÔLE</label>
                  <div class="input-glow-wrapper">
                    <i class="fa-solid fa-layer-group"></i>
                    <select v-model="selectedTemplate" @change="applyTemplate" class="field-input" :disabled="isEditing">
                      <option value="">Rôle personnalisé</option>
                      <option value="manager">Manager - Accès complet</option>
                      <option value="hr">RH - Recrutement & Staff</option>
                      <option value="evaluator">Évaluateur (Correcteur technique)</option>
                      <option value="viewer">Lecteur - Lecture seule</option>
                    </select>
                  </div>
                </div>
                <div class="col-12" v-if="!isEditing">
                  <label class="field-label">INVITER UN RESPONSABLE (Optionnel)</label>
                  <div class="input-glow-wrapper">
                    <i class="fa-solid fa-envelope-open-text"></i>
                    <input v-model="form.email" type="email" class="field-input" placeholder="responsable@societe.pro" />
                  </div>
                </div>
              </div>
            </div>

            <!-- 02 Matrice droits -->
            <div class="form-section">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="section-badge m-0"><span>02</span> MATRICE DES DROITS</div>
                <div class="perm-count-pill">{{ form.permissions.length }} / {{ totalPerms }} actifs</div>
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

          <footer class="modal-foot">
            <button @click="closeModal" class="btn-secondary-elite">ANNULER</button>
            <button @click="saveRole" class="btn-sunburst px-5" :disabled="saving">
              <div class="btn-label" v-if="!saving">
                <i class="fa-solid fa-unlock-keyhole me-2"></i>
                {{ isEditing ? 'METTRE À JOUR' : 'DÉPLOYER LE RÔLE' }}
              </div>
              <div v-else class="btn-loader">
                <span></span><span></span><span></span>
              </div>
              <div class="shine-sweep"></div>
            </button>
          </footer>

        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const roles = ref([]);
const stats = ref([]);
const loading = ref(true);
const saving = ref(false);
const showModal = ref(false);
const isEditing = ref(false);
const searchQuery = ref('');
const selectedTemplate = ref('');

const form = reactive({ id: null, nom: '', email: '', description: '', permissions: [] });

const permissionGroups = [
  {
    title: 'CANDIDATS',
    items: [
      { id: 'view_can', label: 'Accès Liste', desc: 'Lecture des profils' },
      { id: 'inv_can', label: 'Invitations', desc: 'Envoi des tests' }
    ]
  },
  {
    title: 'ÉVALUATIONS & CONTENU',
    items: [
      { id: 'view_tests', label: 'Audit Tests', desc: 'Lecture des tests' },
      { id: 'grade_tests', label: 'Évaluation', desc: 'Attribution des scores' },
      { id: 'edit_bank', label: 'Banque de questions', desc: 'Gérer les questions' }
    ]
  },
  {
    title: 'ADMINISTRATION',
    items: [
      { id: 'add_rol', label: 'Gérer Rôles', desc: 'Création de rôles' },
      { id: 'add_staff', label: 'Gérer Staff', desc: 'Nouveaux membres' }
    ]
  }
];

const totalPerms = computed(() => permissionGroups.reduce((a, g) => a + g.items.length, 0));

const fetchData = async () => {
  loading.value = true;
  try {
    const [rolesRes, statsRes] = await Promise.all([api.get('/Roles'), api.get('/Roles/stats')]);
    roles.value = rolesRes.data;
    stats.value = rolesRes.data ? statsRes.data : [];
  } catch (err) { console.error(err); } finally { loading.value = false; }
};

const togglePermission = (id) => {
  const i = form.permissions.indexOf(id);
  i > -1 ? form.permissions.splice(i, 1) : form.permissions.push(id);
};

const applyTemplate = () => {
  if (!selectedTemplate.value) { form.permissions = []; return; }
  const mapping = {
    manager: ['view_can', 'inv_can', 'view_tests', 'grade_tests', 'edit_bank', 'add_rol', 'add_staff'],
    hr: ['view_can', 'inv_can', 'add_staff'],
    evaluator: ['view_tests', 'grade_tests', 'edit_bank'],
    viewer: ['view_can', 'view_tests']
  };
  form.permissions = [...(mapping[selectedTemplate.value] || [])];
};

const saveRole = async () => {
  if (!form.nom) return alert("Nom de rôle requis.");
  saving.value = true;
  try {
    if (isEditing.value) await api.put(`/Roles/${form.id}`, form);
    else await api.post('/Roles', form);
    await fetchData();
    closeModal();
  } catch (e) {
    alert("ERREUR : " + (e.response?.data?.message || "Échec de l'opération"));
  } finally { saving.value = false; }
};

const confirmDelete = async (id) => {
  if (confirm("Révoquer ce rôle ?")) {
    try { await api.delete(`/Roles/${id}`); fetchData(); } catch (e) { alert("Le rôle est utilisé."); }
  }
};

const openAddModal = () => { isEditing.value = false; resetForm(); showModal.value = true; };
const openEditModal = (r) => { isEditing.value = true; Object.assign(form, JSON.parse(JSON.stringify(r))); showModal.value = true; };
const closeModal = () => showModal.value = false;
const resetForm = () => { Object.assign(form, { id: null, nom: '', email: '', description: '', permissions: [] }); selectedTemplate.value = ''; };

const filteredRoles = computed(() =>
  roles.value.filter(r => r.nom !== 'SuperAdmin' && r.nom.toLowerCase().includes(searchQuery.value.toLowerCase()))
);

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&display=swap');

/* =============================================
   BASE — identique au Login
   ============================================= */
.elite-roles-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background: #fdfdfd;
  min-height: 100vh;
  position: relative;
  overflow-x: hidden;
  display: flex;
}

/* =============================================
   FOND LUXE — copié du Login
   ============================================= */
.luxury-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }

.aura-sphere {
  position: absolute; border-radius: 50%;
  filter: blur(140px); opacity: 0.22;
  animation: orbit 25s infinite linear;
}
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; left: -10%; }
.sphere-blue  { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose  { width: 400px; height: 400px; background: #fda4af; top: 35%; right: 15%; opacity: 0.08; }

.mesh-grain {
  position: absolute; inset: 0; opacity: 0.03;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e");
}

@keyframes orbit {
  from { transform: rotate(0) translate(40px) rotate(0); }
  to   { transform: rotate(360deg) translate(40px) rotate(-360deg); }
}

/* =============================================
   LAYOUT
   ============================================= */
.main-viewport   { flex: 1; min-width: 0; }
.content-overlay { position: relative; z-index: 10; }

/* =============================================
   EN-TÊTE
   ============================================= */
.page-header { display: flex; flex-direction: column; gap: 20px; }
@media (min-width: 768px) {
  .page-header { flex-direction: row; justify-content: space-between; align-items: flex-end; }
}

.elite-breadcrumb { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }
.elite-breadcrumb .sep { opacity: 0.4; margin: 0 6px; }

.main-title {
  font-size: clamp(1.8rem, 4vw, 2.8rem);
  font-weight: 900; color: #0f172a; letter-spacing: -2px; margin: 0;
}
.text-amber-gradient {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text;
}
.subtitle { color: #64748b; font-size: 14px; font-weight: 400; margin-top: 6px; margin-bottom: 0; }

/* =============================================
   BOUTONS — identique au Login
   ============================================= */
.btn-sunburst {
  padding: 16px 28px; border-radius: 22px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; cursor: pointer;
  position: relative; overflow: hidden;
  transition: all 0.4s; box-shadow: 0 15px 35px rgba(251,191,36,0.28);
  font-family: 'Plus Jakarta Sans', sans-serif; font-size: 13px;
}
.btn-sunburst:hover { transform: translateY(-4px); box-shadow: 0 20px 45px rgba(251,191,36,0.45); }
.btn-sunburst:disabled { opacity: 0.6; cursor: not-allowed; transform: none; }

.btn-label { display: flex; align-items: center; justify-content: center; gap: 8px; }

.shine-sweep {
  position: absolute; top: 0; left: -120%; width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.38), transparent);
  animation: sweep 4s infinite;
}
@keyframes sweep { 0% { left: -120%; } 35% { left: 150%; } 100% { left: 150%; } }

.btn-secondary-elite {
  background: white; border: 1.5px solid #f1f5f9;
  padding: 14px 24px; border-radius: 20px;
  font-family: 'Plus Jakarta Sans', sans-serif;
  font-weight: 800; font-size: 13px; color: #475569; cursor: pointer;
  transition: all 0.3s;
}
.btn-secondary-elite:hover { background: #f8fafc; border-color: #e2e8f0; transform: translateY(-1px); }

/* =============================================
   STAT CARDS
   ============================================= */
.stat-bento-card {
  background: rgba(255,255,255,0.95); backdrop-filter: blur(20px);
  border-radius: 30px; padding: 28px; border: 1px solid rgba(255,255,255,0.9);
  box-shadow: 0 10px 40px rgba(0,0,0,0.04);
  display: flex; align-items: center; gap: 20px;
  position: relative; overflow: hidden;
  transition: transform 0.4s cubic-bezier(0.175,0.885,0.32,1.275), box-shadow 0.4s;
}
.stat-bento-card:hover { transform: translateY(-6px); box-shadow: 0 20px 50px rgba(0,0,0,0.08); }

.stat-icon-wrap {
  width: 56px; height: 56px; border-radius: 18px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.3rem; flex-shrink: 0; z-index: 1;
}
.stat-body    { display: flex; flex-direction: column; gap: 4px; z-index: 1; }
.stat-label   { font-size: 9px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }
.stat-value   { font-size: 2rem; font-weight: 900; color: #0f172a; line-height: 1; }
.stat-glow    { position: absolute; right: -30px; top: -30px; width: 120px; height: 120px; border-radius: 50%; opacity: 0.06; filter: blur(20px); }

/* =============================================
   RECHERCHE
   ============================================= */
.search-vessel {
  background: rgba(255,255,255,0.95); backdrop-filter: blur(20px);
  border-radius: 24px; border: 1.5px solid rgba(255,255,255,0.9);
  box-shadow: 0 10px 30px rgba(0,0,0,0.04);
  display: flex; align-items: center; gap: 12px; padding: 10px 16px;
  transition: border-color 0.3s, box-shadow 0.3s;
}
.search-vessel:focus-within {
  border-color: #fbbf24;
  box-shadow: 0 12px 30px rgba(251,191,36,0.12);
}
.search-icon-pill {
  width: 36px; height: 36px; background: #fef3c7; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  color: #fbbf24; font-size: 13px; flex-shrink: 0;
}
.search-input {
  flex: 1; border: none; background: transparent; outline: none;
  font-family: 'Plus Jakarta Sans', sans-serif; font-size: 15px; font-weight: 600; color: #0f172a;
}
.search-input::placeholder { color: #cbd5e1; }
.search-badge { background: #fbbf24; color: #0f172a; font-size: 11px; font-weight: 800; padding: 3px 10px; border-radius: 99px; }

/* =============================================
   LOADER ROBOT — identique au Login
   ============================================= */
.bot-loader-wrap { display: flex; justify-content: center; }
.loader-bot { width: 100px; animation: floatBot 4s ease-in-out infinite; }
@keyframes floatBot { 0%,100% { transform: translateY(0); } 50% { transform: translateY(-12px); } }
.signal-ping { animation: ping 3s infinite; transform-origin: center; }
@keyframes ping { 0% { r: 5; opacity: 0.8; } 100% { r: 40; opacity: 0; } }
.led-blink { animation: blink 4s infinite; transform-origin: center; }
@keyframes blink { 0%,90%,100% { transform: scaleY(1); } 95% { transform: scaleY(0.1); } }
.loading-text { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }

/* =============================================
   CARTES RÔLES
   ============================================= */
.role-card {
  background: rgba(255,255,255,0.95); backdrop-filter: blur(20px);
  border: 1px solid rgba(255,255,255,0.9); border-radius: 40px;
  padding: 32px; height: 100%; position: relative; overflow: hidden;
  box-shadow: 0 10px 40px rgba(0,0,0,0.03);
  transition: transform 0.4s cubic-bezier(0.175,0.885,0.32,1.275), border-color 0.3s, box-shadow 0.4s;
}
.role-card:hover {
  transform: translateY(-10px);
  border-color: #fbbf24;
  box-shadow: 0 25px 60px rgba(251,191,36,0.12), 0 10px 30px rgba(0,0,0,0.05);
}
.card-shine {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, rgba(255,255,255,0.6) 0%, transparent 60%);
  pointer-events: none; border-radius: inherit;
}

.role-squircle {
  width: 50px; height: 50px; background: #0f172a; color: #fbbf24;
  border-radius: 16px; display: flex; align-items: center; justify-content: center;
  font-size: 1.2rem; flex-shrink: 0;
}

.btn-dots {
  width: 36px; height: 36px; background: #f8fafc;
  border: 1px solid #f1f5f9; border-radius: 12px; color: #94a3b8; cursor: pointer;
  display: flex; align-items: center; justify-content: center; transition: all 0.3s;
}
.btn-dots:hover { background: #0f172a; color: #fbbf24; border-color: #0f172a; }

.dropdown-premium {
  border-radius: 20px !important; padding: 8px !important;
  box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important;
  border: 1px solid #f1f5f9 !important; min-width: 170px;
}
.dropdown-premium .dropdown-item {
  border-radius: 12px; font-weight: 600; font-size: 13.5px;
  padding: 10px 14px !important; transition: background 0.15s;
}
.dropdown-premium .dropdown-item:hover { background: #f8fafc; }
.text-amber { color: #fbbf24 !important; }

.role-name  { font-size: 1.4rem; font-weight: 900; color: #0f172a; letter-spacing: -0.5px; margin-bottom: 6px; }
.role-desc  { color: #64748b; font-size: 13.5px; margin-bottom: 0; }

.perm-tags  { display: flex; flex-wrap: wrap; gap: 6px; }
.perm-pill  { font-size: 9px; font-weight: 800; padding: 5px 12px; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 99px; color: #64748b; white-space: nowrap; }
.perm-pill.more { background: #fef3c7; border-color: #fde68a; color: #d97706; }

.card-footer-row { border-top: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
.members-chip    { font-size: 12px; font-weight: 600; color: #64748b; background: #f8fafc; padding: 6px 14px; border-radius: 99px; border: 1px solid #f1f5f9; }
.btn-edit-link   { background: none; border: none; font-size: 11px; font-weight: 800; color: #fbbf24; cursor: pointer; letter-spacing: 0.5px; transition: color 0.2s; padding: 0; }
.btn-edit-link:hover { color: #d97706; }

/* =============================================
   MODALE — style carte Login
   ============================================= */
.modal-overlay {
  position: fixed; inset: 0; z-index: 3000;
  background: rgba(15,23,42,0.65); backdrop-filter: blur(16px);
  display: flex; align-items: center; justify-content: center; padding: 20px;
}

.modal-bento {
  background: rgba(255,255,255,0.99); backdrop-filter: blur(40px);
  border-radius: 50px; width: 100%; max-width: 880px; max-height: 90vh;
  display: flex; flex-direction: column;
  border: 1px solid white;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.08);
  overflow: hidden;
}

.modal-head {
  padding: 28px 40px; display: flex; justify-content: space-between; align-items: center;
  background: #fafafa; flex-shrink: 0;
}
.modal-brand        { display: flex; align-items: center; gap: 16px; }
.modal-brand-icon   {
  width: 48px; height: 48px;
  background: linear-gradient(135deg, #fbbf24, #f59e0b);
  border-radius: 16px; display: flex; align-items: center; justify-content: center;
  color: #0f172a; font-size: 1.2rem;
  box-shadow: 0 8px 20px rgba(251,191,36,0.3); flex-shrink: 0;
}
.modal-title   { font-size: 1.1rem; font-weight: 900; color: #0f172a; letter-spacing: 0.5px; margin: 0; }
.modal-sub     { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; margin: 4px 0 0; }

.btn-close-modal {
  width: 40px; height: 40px; background: #f8fafc; border: 1px solid #f1f5f9;
  border-radius: 14px; color: #94a3b8; cursor: pointer;
  display: flex; align-items: center; justify-content: center; font-size: 14px;
  transition: all 0.3s;
}
.btn-close-modal:hover { background: #fee2e2; color: #e11d48; border-color: #fecaca; }

.header-line-modal { height: 4px; background: linear-gradient(90deg, #fbbf24, #f59e0b); flex-shrink: 0; }

.modal-body-scroll { padding: 32px 40px; overflow-y: auto; flex: 1; background: #fcfdfe; }
.modal-body-scroll::-webkit-scrollbar { width: 5px; }
.modal-body-scroll::-webkit-scrollbar-track { background: transparent; }
.modal-body-scroll::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 99px; }

/* =============================================
   FORMULAIRE — inputs identiques Login
   ============================================= */
.form-section { background: white; border-radius: 30px; border: 1px solid #f1f5f9; padding: 28px; }

.section-badge {
  display: flex; align-items: center; gap: 12px;
  font-size: 12px; font-weight: 800; color: #0f172a; letter-spacing: 1px; text-transform: uppercase;
}
.section-badge span {
  width: 28px; height: 28px; background: #0f172a; color: white;
  border-radius: 9px; display: flex; align-items: center; justify-content: center;
  font-size: 12px; font-weight: 800; flex-shrink: 0;
}

.field-label { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; text-transform: uppercase; margin-bottom: 10px; display: block; }

.input-glow-wrapper { position: relative; display: flex; align-items: center; }
.input-glow-wrapper i { position: absolute; left: 18px; color: #fbbf24; font-size: 15px; pointer-events: none; transition: 0.3s; }

.field-input {
  width: 100%; padding: 17px 20px 17px 52px;
  border-radius: 20px; border: 1.5px solid #f1f5f9; background: #f8fafc;
  font-family: 'Plus Jakarta Sans', sans-serif; font-size: 14px; font-weight: 600; color: #0f172a;
  outline: none; appearance: none; -webkit-appearance: none;
  transition: all 0.4s cubic-bezier(0.175,0.885,0.32,1.275);
}
.field-input:focus {
  border-color: #fbbf24; background: white;
  box-shadow: 0 12px 25px rgba(251,191,36,0.1); transform: translateY(-2px);
}
.field-input:disabled { opacity: 0.5; cursor: not-allowed; }
.field-input::placeholder { color: #cbd5e1; }

/* =============================================
   PERMISSIONS
   ============================================= */
.perm-count-pill { background: #fef3c7; color: #d97706; border: 1px solid #fde68a; font-size: 11px; font-weight: 800; padding: 5px 14px; border-radius: 99px; }

.perm-group { background: #f8fafc; border-radius: 20px; padding: 18px; border: 1px solid #f1f5f9; }

.group-label { font-size: 9.5px; font-weight: 800; color: #94a3b8; letter-spacing: 2.5px; margin-bottom: 12px; display: flex; align-items: center; gap: 8px; }
.group-dot   { width: 6px; height: 6px; border-radius: 50%; background: #fbbf24; flex-shrink: 0; }

.perm-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(230px, 1fr)); gap: 10px; }

.perm-node {
  padding: 16px; border-radius: 16px; border: 1.5px solid #f1f5f9;
  background: white; display: flex; gap: 14px; cursor: pointer; user-select: none;
  align-items: flex-start;
  transition: all 0.3s cubic-bezier(0.175,0.885,0.32,1.275);
}
.perm-node:hover { border-color: #e2e8f0; transform: translateY(-2px); box-shadow: 0 8px 20px rgba(0,0,0,0.05); }
.perm-node.active { border-color: #fbbf24; background: #fffbeb; box-shadow: 0 8px 25px rgba(251,191,36,0.15); }

.node-checkbox {
  width: 22px; height: 22px; border: 2px solid #cbd5e1; border-radius: 8px;
  display: flex; align-items: center; justify-content: center;
  color: white; font-size: 11px; flex-shrink: 0; transition: all 0.2s;
}
.active .node-checkbox { background: #fbbf24; border-color: #fbbf24; }

.node-name { display: block; font-weight: 800; font-size: 13px; color: #0f172a; line-height: 1.3; }
.node-desc { display: block; font-size: 11px; color: #94a3b8; margin-top: 2px; }

/* =============================================
   FOOTER MODALE
   ============================================= */
.modal-foot { padding: 22px 40px; border-top: 1px solid #f1f5f9; display: flex; justify-content: flex-end; gap: 14px; background: #fafafa; flex-shrink: 0; }

/* =============================================
   LOADER BOUTON — identique Login
   ============================================= */
.btn-loader { display: flex; gap: 6px; justify-content: center; align-items: center; }
.btn-loader span { width: 8px; height: 8px; background: #0f172a; border-radius: 50%; animation: loaderPulse 0.6s infinite alternate; }
.btn-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes loaderPulse { from { transform: scale(1); opacity: 1; } to { transform: scale(1.5); opacity: 0.3; } }

/* =============================================
   TRANSITIONS
   ============================================= */
.fade-slide-enter-active, .fade-slide-leave-active { transition: all 0.35s cubic-bezier(0.4,0,0.2,1); }
.fade-slide-enter-from { opacity: 0; transform: translateY(20px) scale(0.97); }
.fade-slide-leave-to   { opacity: 0; transform: translateY(-10px) scale(0.97); }

.fade-enter-active, .fade-leave-active { transition: opacity 0.2s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* =============================================
   RESPONSIVE
   ============================================= */
@media (max-width: 768px) {
  .main-title { font-size: 1.8rem; }
  .modal-bento { border-radius: 30px; max-height: 95vh; }
  .modal-head, .modal-body-scroll, .modal-foot { padding: 20px; }
  .perm-grid { grid-template-columns: 1fr; }
  .form-section { padding: 20px; }
}
</style>