<template>
  <div class="admin-layout d-flex min-vh-100">
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

    <AppSidebar />

    <div class="main-content flex-grow-1 p-4 p-lg-5 position-relative">
      <AppNavbar />

      <!-- HEADER -->
      <header class="d-flex flex-column flex-xl-row justify-content-between align-items-xl-center mb-5 gap-4 animate__animated animate__fadeIn">
        <div class="brand-section">
          <div class="status-badge"><span class="dot"></span> SYSTÈME DE GESTION D'ACTIFS</div>
          <h2 class="brand-title">Evalua<span>Tech</span> <small class="ms-2 vault-text">Question Bank</small></h2>
          <p class="brand-subtitle text-uppercase">Référentiel centralisé des évaluations techniques</p>
        </div>
        <div class="header-actions d-flex flex-wrap gap-3 align-items-center">
          <div class="search-cyber-box">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher un actif...">
          </div>
          <button class="btn-manage-cat" @click="showCatManager = true" title="Gérer les catégories">
            <i class="fa-solid fa-folder-tree me-2"></i> CATÉGORIES
          </button>
          <button class="btn-primary-gradient" @click="openModal()">
            <i class="fa-solid fa-plus-circle me-2"></i> NOUVELLE QUESTION
          </button>
        </div>
      </header>

      <!-- KPI -->
      <div class="row g-4 mb-5 animate__animated animate__fadeInUp">
        <div class="col-xl-3 col-md-6" v-for="stat in quickStats" :key="stat.label">
          <div class="kpi-glass-card">
            <div class="icon-wrap" :style="{ color: stat.color, background: stat.bg }">
              <i :class="stat.icon"></i>
            </div>
            <div class="data-wrap">
              <span class="value">{{ stat.value }}</span>
              <span class="label">{{ stat.label }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- FILTRES -->
      <div class="filter-cyber-bar mb-4 animate__animated animate__fadeIn">
        <div class="d-flex align-items-center flex-wrap gap-3">
          <div class="filter-label"><i class="fa-solid fa-filter me-2"></i> FILTRER :</div>
          <div class="filter-chips">
            <button @click="activeFilter = -1" :class="['chip', { active: activeFilter === -1 }]">Tous les formats</button>
            <button
              v-for="t in typeDefinitions" :key="t.val"
              @click="activeFilter = t.val"
              :class="['chip', { active: activeFilter === t.val }]">
              <i :class="t.icon" class="me-1"></i> {{ t.label }}
            </button>
          </div>
          <div class="ms-auto border-start ps-3">
            <select v-model="selectedCat" class="cyber-select-minimal">
              <option value="All">Toutes les Catégories</option>
              <option v-for="cat in categoriesList" :key="cat" :value="cat">{{ cat }}</option>
            </select>
          </div>
        </div>
      </div>

      <!-- GRILLE QUESTIONS -->
      <!-- FIX #1: v-if/v-else-if/v-else properly chained so grid never shows while loading/empty -->
      <div class="row g-4 position-relative">
        <div v-if="loading" class="col-12 text-center py-5">
          <div class="cyber-spinner"></div>
          <p class="mt-3 fw-bold text-muted letter-spacing-1">SYNCHRONISATION DU COFFRE...</p>
        </div>
        <div v-else-if="filteredQuestions.length === 0" class="col-12 text-center py-5 opacity-50">
          <i class="fa-solid fa-box-open fa-3x mb-3"></i>
          <p class="fw-bold">Aucun actif trouvé pour cette sélection.</p>
        </div>
        <!-- FIX #2: transition-group IS the row (no double-row wrapper) -->
        <transition-group v-else name="stagger" tag="div" class="row g-4 m-0 p-0">
          <div v-for="q in filteredQuestions" :key="q.id" class="col-xl-4 col-md-6">
            <div class="asset-card-cyber">
              <div class="card-header-actions">
                <span class="badge-cat">{{ q.categorie || '—' }}</span>
                <div class="d-flex gap-2">
                  <button @click="openModal(q)" class="btn-card-action" title="Éditer"><i class="fa-solid fa-pen"></i></button>
                  <button @click="handleDelete(q.id)" class="btn-card-action text-danger" title="Supprimer"><i class="fa-solid fa-trash"></i></button>
                </div>
              </div>
              <div class="card-main">
                <h5 class="question-text">{{ q.texte }}</h5>
                <div class="type-badge-sm">
                  <i :class="getTypeInfo(q.type).icon"></i> {{ getTypeInfo(q.type).label }}
                </div>
              </div>
              <div class="card-footer-stats">
                <div class="poids-group">
                  <div class="d-flex justify-content-between mb-1">
                    <span class="poids-label">Complexité</span>
                    <span class="poids-val">{{ q.poids }}/5</span>
                  </div>
                  <div class="cyber-progress-container">
                    <div class="fill" :style="{ width: (q.poids * 20) + '%', background: getWeightColor(q.poids) }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </transition-group>
      </div>

      <!-- MODALE CATÉGORIES -->
      <transition name="modal-fade">
        <div v-if="showCatManager" class="modal-cyber-overlay" @click.self="showCatManager = false">
          <div class="modal-cyber-card cat-manager-modal animate__animated animate__zoomIn">
            <div class="modal-cyber-header">
              <h4 class="m-0">GÉRER LES <span>CATÉGORIES</span></h4>
              <button @click="showCatManager = false" class="btn-close-cyber">&times;</button>
            </div>
            <div class="p-4">
              <div class="d-flex gap-2 mb-4">
                <input v-model="newCatName" @keyup.enter="addCategory" type="text" class="cyber-field-simple" placeholder="Nom de la catégorie (ex: DevOps)...">
                <button class="btn-primary-gradient" @click="addCategory">AJOUTER</button>
              </div>
              <div class="cat-list-container custom-scrollbar">
                <div v-for="cat in categoriesList" :key="cat" class="cat-pill-item">
                  <span><i class="fa-solid fa-hashtag me-2 opacity-50"></i>{{ cat }}</span>
                  <button @click="removeCategory(cat)" class="btn-del-mini">&times;</button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition>

      <!-- MODALE QUESTION -->
      <transition name="modal-fade">
        <div v-if="showModal" class="modal-cyber-overlay" @click.self="showModal = false">
          <div class="modal-cyber-card master-config-modal animate__animated animate__zoomIn">
            <div class="modal-cyber-header">
              <div class="title-wrap">
                <h4 class="m-0">{{ isEdit ? 'ÉDITION' : 'CRÉATION' }} D'<span>ACTIF</span></h4>
                <p class="text-muted small mb-0">Configuration de la logique d'évaluation technique</p>
              </div>
              <button @click="showModal = false" class="btn-close-cyber">&times;</button>
            </div>

            <div class="modal-cyber-body custom-scrollbar">
              <div class="row g-4">

                <!-- TYPE -->
                <div class="col-12">
                  <label class="label-cyber">TYPE DE COMPOSANT</label>
                  <div class="type-visual-selector">
                    <div
                      v-for="t in typeDefinitions" :key="t.val"
                      class="type-v-card" :class="{ active: form.type === t.val }"
                      @click="handleTypeChange(t.val)">
                      <i :class="t.icon"></i>
                      <span>{{ t.label }}</span>
                    </div>
                  </div>
                </div>

                <!-- TEXTE -->
                <div class="col-12">
                  <label class="label-cyber">ÉNONCÉ DE LA QUESTION *</label>
                  <textarea v-model="form.texte" class="cyber-input-area" rows="3" placeholder="Saisir la problématique technique..."></textarea>
                  <span v-if="validationErrors.texte" class="field-error-msg">{{ validationErrors.texte }}</span>
                </div>

                <!-- CATÉGORIE + POIDS -->
                <div class="col-md-6">
                  <label class="label-cyber">CATÉGORIE / DOMAINE</label>
                  <select v-model="form.categorie" class="cyber-select-field">
                    <option value="">— Choisir —</option>
                    <option v-for="cat in categoriesList" :key="cat" :value="cat">{{ cat }}</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="label-cyber">NIVEAU DE COMPLEXITÉ : <span class="text-amber fw-bold">{{ form.poids }} PTS</span></label>
                  <input type="range" min="1" max="5" step="1" v-model.number="form.poids" class="cyber-range">
                  <div class="d-flex justify-content-between small text-muted px-1 mt-1">
                    <span>Junior</span><span>Senior</span>
                  </div>
                </div>

                <!-- RÉPONSES QCM / CHOIX MULTIPLE / VRAI-FAUX -->
                <div class="col-12 responses-config-panel" v-if="[0, 1, 2].includes(form.type)">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <label class="label-cyber m-0">VALEURS DE RÉPONSE ET CORRECTION</label>
                    <button v-if="form.type !== 2" @click="addResponse" class="btn-add-cyber">
                      <i class="fa-solid fa-plus me-1"></i> AJOUTER OPTION
                    </button>
                  </div>

                  <div class="responses-list">
                    <div v-for="(rep, index) in form.reponses" :key="index" class="response-cyber-row animate__animated animate__fadeIn">
                      <div class="check-container" :title="rep.estCorrecte ? 'Bonne réponse' : 'Marquer comme correcte'">
                        <!--
                          FIX #3: Correct radio/checkbox binding.
                          - Radio (type 0 & 2): bind to correctRadioIndex — single shared model.
                          - Checkbox (type 1): bind directly to rep.estCorrecte.
                        -->
                        <input
                          v-if="form.type === 1"
                          type="checkbox"
                          v-model="rep.estCorrecte"
                          class="cyber-check-input">
                        <input
                          v-else
                          type="radio"
                          :name="`correct-radio-${form.id || 'new'}`"
                          :value="index"
                          v-model="correctRadioIndex"
                          class="cyber-check-input">
                      </div>
                      <input type="text" v-model="rep.texte" class="cyber-field-simple flex-grow-1" placeholder="Texte de la réponse..." :readonly="form.type === 2">
                      <button v-if="form.type < 2" @click="removeResponse(index)" class="btn-del-cyber"><i class="fa-solid fa-trash-can"></i></button>
                    </div>
                  </div>
                  <span v-if="validationErrors.reponses" class="field-error-msg">{{ validationErrors.reponses }}</span>
                  <div class="alert-cyber mt-3">
                    <i class="fa-solid fa-circle-info me-2"></i>
                    Cochez les cercles/carrés à gauche pour définir la ou les bonnes réponses.
                  </div>
                </div>

                <!-- ZONE IA -->
                <div class="col-12" v-if="[4, 5, 6].includes(form.type)">
                  <label class="label-cyber">CRITÈRES DE VALIDATION (CLÉ IA)</label>
                  <textarea v-model="form.bonneReponse" class="cyber-input-area" rows="3" placeholder="Décrivez les mots-clés ou le comportement attendu pour l'analyse IA..."></textarea>
                </div>
              </div>
            </div>

            <div class="modal-cyber-footer">
              <button @click="showModal = false" class="btn-glass-cancel">ANNULER</button>
              <button @click="save" :disabled="isSaving" class="btn-primary-gradient px-5 shadow">
                <span v-if="isSaving" class="spinner-border spinner-border-sm me-2"></span>
                <i v-else class="fa-solid fa-shield-halved me-2"></i>
                {{ isSaving ? 'SAUVEGARDE...' : "ENREGISTRER L'ACTIF" }}
              </button>
            </div>
          </div>
        </div>
      </transition>

      <!-- TOAST -->
      <transition name="toast-slide">
        <div v-if="toast.active" class="enigma-toast" :class="toast.type">
          <div class="t-ico"><i :class="toast.icon"></i></div>
          <div class="t-body"><strong>SYSTÈME</strong><p class="m-0 small">{{ toast.message }}</p></div>
        </div>
      </transition>

      <!-- CONFIRM DIALOG -->
      <transition name="modal-fade">
        <div v-if="confirmDialog.show" class="modal-cyber-overlay" style="z-index:10000" @click.self="confirmDialog.show = false">
          <div class="confirm-modal animate__animated animate__zoomIn animate__faster">
            <div class="confirm-icon mb-3"><i :class="confirmDialog.icon" class="fa-2x text-danger"></i></div>
            <h5 class="fw-bold mb-2">{{ confirmDialog.title }}</h5>
            <p class="text-muted small mb-4">{{ confirmDialog.message }}</p>
            <div class="d-flex gap-3 justify-content-center">
              <button @click="confirmDialog.show = false" class="btn-glass-cancel">ANNULER</button>
              <button @click="runConfirm" class="btn-confirm-danger">CONFIRMER</button>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import axios from 'axios';

const API_QUESTIONS = 'http://localhost:5172/api/Questions';

/* ─── TYPE DEFINITIONS ────────────────────────────────────────── */
const typeDefinitions = [
  { val: 0, label: 'Choix Unique',    icon: 'fa-solid fa-circle-dot' },
  { val: 1, label: 'Choix Multiple',  icon: 'fa-solid fa-square-check' },
  { val: 2, label: 'Vrai / Faux',     icon: 'fa-solid fa-toggle-on' },
  { val: 4, label: 'Audit Texte',     icon: 'fa-solid fa-robot' },
  { val: 5, label: 'Code Source',     icon: 'fa-solid fa-code' },
  { val: 6, label: 'Fichier / Projet',icon: 'fa-solid fa-folder-open' },
];

/* ─── STATE ───────────────────────────────────────────────────── */
const questions      = ref([]);
const loading        = ref(true);
const isSaving       = ref(false);
const showModal      = ref(false);
const showCatManager = ref(false);
const isEdit         = ref(false);
const searchQuery    = ref('');
const activeFilter   = ref(-1);
const selectedCat    = ref('All');
const newCatName     = ref('');
const categoriesList = ref(['Backend', 'Frontend', 'Cyber-Sécurité', 'Architecte UML', 'DevOps']);
const validationErrors = reactive({ texte: '', reponses: '' });

const toast = reactive({ active: false, message: '', type: '', icon: '' });
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _resolve: null });

/* ─── FORM ────────────────────────────────────────────────────── */
const form = reactive({
  id: '', texte: '', type: 0, poids: 1,
  categorie: '', reponses: [], bonneReponse: '',
  questionnaireId: null,
});

/*
  FIX #3 (continued): correctRadioIndex is the single v-model for all radio buttons.
  It syncs bidirectionally with form.reponses[i].estCorrecte.
*/
const correctRadioIndex = computed({
  get: () => form.reponses.findIndex(r => r.estCorrecte),
  set: (idx) => form.reponses.forEach((r, i) => { r.estCorrecte = (i === idx); }),
});

/* ─── DATA FETCHING ───────────────────────────────────────────── */
const fetchData = async () => {
  loading.value = true;
  try {
    const res = await axios.get(API_QUESTIONS);
    questions.value = res.data;
  } catch (e) {
    showToast('Erreur de connexion à l\'API.', 'error', 'fa-solid fa-plug-circle-xmark');
  } finally {
    loading.value = false;
  }
};

/* ─── TYPE CHANGE ─────────────────────────────────────────────── */
const handleTypeChange = (newType) => {
  form.type = newType;
  if (newType === 2) {
    form.reponses = [
      { texte: 'Vrai', estCorrecte: true  },
      { texte: 'Faux', estCorrecte: false },
    ];
  } else if ([0, 1].includes(newType)) {
    if (form.reponses.length < 2) {
      form.reponses = [
        { texte: '', estCorrecte: true  },
        { texte: '', estCorrecte: false },
      ];
    }
  } else {
    form.reponses = [];
  }
};

const addResponse    = () => form.reponses.push({ texte: '', estCorrecte: false });
const removeResponse = (i) => {
  form.reponses.splice(i, 1);
  // Ensure at least one correct answer remains for radio types
  if ((form.type === 0) && !form.reponses.some(r => r.estCorrecte) && form.reponses.length) {
    form.reponses[0].estCorrecte = true;
  }
};

/* ─── CATEGORIES ──────────────────────────────────────────────── */
const addCategory = () => {
  const name = newCatName.value.trim();
  if (name && !categoriesList.value.includes(name)) {
    categoriesList.value.push(name);
    newCatName.value = '';
  }
};
const removeCategory = (cat) => {
  categoriesList.value = categoriesList.value.filter(c => c !== cat);
};

/* ─── MODAL / CRUD ────────────────────────────────────────────── */
const openModal = (q = null) => {
  validationErrors.texte    = '';
  validationErrors.reponses = '';
  isEdit.value = !!q;

  if (q) {
    Object.assign(form, JSON.parse(JSON.stringify(q)));
    // Ensure reponses exists
    if (!form.reponses) form.reponses = [];
  } else {
    Object.assign(form, {
      id: '00000000-0000-0000-0000-000000000000',
      texte: '', type: 0, poids: 1,
      categorie: categoriesList.value[0] || '',
      reponses: [
        { texte: '', estCorrecte: true  },
        { texte: '', estCorrecte: false },
      ],
      bonneReponse: '',
      questionnaireId: null,
    });
  }
  showModal.value = true;
};

const validate = () => {
  let ok = true;
  validationErrors.texte    = '';
  validationErrors.reponses = '';

  if (!form.texte.trim()) {
    validationErrors.texte = 'L\'énoncé est obligatoire.';
    ok = false;
  }
  if ([0, 1].includes(form.type)) {
    const filled = form.reponses.filter(r => r.texte.trim()).length;
    if (filled < 2) {
      validationErrors.reponses = 'Veuillez renseigner au moins 2 options de réponse.';
      ok = false;
    }
    if (!form.reponses.some(r => r.estCorrecte)) {
      validationErrors.reponses = 'Veuillez désigner au moins une bonne réponse.';
      ok = false;
    }
  }
  return ok;
};

const save = async () => {
  if (!validate()) return;
  isSaving.value = true;
  try {
    if (isEdit.value) {
      await axios.put(`${API_QUESTIONS}/${form.id}`, form);
    } else {
      await axios.post(API_QUESTIONS, form);
    }
    showModal.value = false;
    showToast('Actif enregistré avec succès.', 'success', 'fa-solid fa-check-circle');
    await fetchData();
  } catch (e) {
    showToast('Erreur lors de la sauvegarde.', 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    isSaving.value = false;
  }
};

/* FIX #4 (QuestionsBank): handleDelete now gives error feedback */
const handleDelete = (id) => {
  confirmDialog.title   = 'Supprimer cet actif ?';
  confirmDialog.message = 'Cette action est irréversible. La question sera définitivement supprimée de la banque.';
  confirmDialog.icon    = 'fa-solid fa-trash-can';
  confirmDialog.show    = true;
  confirmDialog._resolve = async () => {
    try {
      await axios.delete(`${API_QUESTIONS}/${id}`);
      showToast('Actif supprimé.', 'warn', 'fa-solid fa-trash-can');
      await fetchData();
    } catch {
      showToast('Erreur lors de la suppression.', 'error', 'fa-solid fa-triangle-exclamation');
    }
  };
};

const runConfirm = async () => {
  confirmDialog.show = false;
  if (typeof confirmDialog._resolve === 'function') {
    await confirmDialog._resolve();
    confirmDialog._resolve = null;
  }
};

/* ─── COMPUTED ────────────────────────────────────────────────── */
const filteredQuestions = computed(() =>
  questions.value.filter(q => {
    const matchSearch = q.texte?.toLowerCase().includes(searchQuery.value.toLowerCase());
    const matchType   = activeFilter.value === -1 || q.type === activeFilter.value;
    const matchCat    = selectedCat.value === 'All' || q.categorie === selectedCat.value;
    return matchSearch && matchType && matchCat;
  })
);

const quickStats = computed(() => [
  { label: 'Actifs Totaux',   value: questions.value.length,                                          icon: 'fa-solid fa-vault',       color: '#eab308', bg: 'rgba(234,179,8,0.1)'   },
  { label: 'Types de Question', value: new Set(questions.value.map(x => x.type)).size,                icon: 'fa-solid fa-shapes',      color: '#60a5fa', bg: 'rgba(96,165,250,0.1)'  },
  { label: 'Catégories',      value: categoriesList.value.length,                                     icon: 'fa-solid fa-folder-tree', color: '#c084fc', bg: 'rgba(192,132,252,0.1)' },
  { label: 'Haut Niveau (4+)', value: questions.value.filter(x => x.poids >= 4).length,              icon: 'fa-solid fa-fire',        color: '#f87171', bg: 'rgba(248,113,113,0.1)' },
]);

/* ─── HELPERS ─────────────────────────────────────────────────── */
const getTypeInfo    = (val) => typeDefinitions.find(t => t.val === val) || typeDefinitions[0];
const getWeightColor = (p)   => p >= 4 ? '#f87171' : p >= 2.5 ? '#fbbf24' : '#4ade80';

let _toastTimer = null;
const showToast = (message, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  toast.message = message;
  toast.type    = `t-${type}`;
  toast.icon    = icon;
  toast.active  = true;
  _toastTimer   = setTimeout(() => { toast.active = false; }, 4000);
};

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* GLOBAL */
.admin-layout { font-family: 'Plus Jakarta Sans', sans-serif; background-color: #f8fafc; position: relative; overflow-x: hidden; }
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at 30% 30%, #ffffff 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(120px); z-index: 1; opacity: 0.12; }
.orb-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; right: -5%; }
.orb-blue  { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; left: -5%; }
.tech-grid-subtle { position: absolute; inset: 0; background-image: linear-gradient(rgba(148,163,184,0.08) 1px, transparent 1px), linear-gradient(90deg, rgba(148,163,184,0.08) 1px, transparent 1px); background-size: 40px 40px; z-index: 1; }
.main-content { z-index: 5; position: relative; }

/* HEADER */
.brand-title { font-weight: 800; color: #0f172a; font-size: 32px; letter-spacing: -1px; }
.brand-title span { color: #eab308; }
.vault-text { font-weight: 700; color: #94a3b8; border-left: 2px solid #e2e8f0; padding-left: 10px; font-size: 16px; }
.brand-subtitle { color: #94a3b8; font-size: 11px; font-weight: 800; letter-spacing: 1.2px; }
.status-badge { font-size: 9px; font-weight: 800; background: white; padding: 6px 14px; border-radius: 20px; border: 1px solid #e2e8f0; width: fit-content; margin-bottom: 10px; }
.status-badge .dot { height: 7px; width: 7px; background: #22c55e; border-radius: 50%; display: inline-block; margin-right: 6px; box-shadow: 0 0 10px rgba(34,197,94,0.5); }

/* SEARCH */
.search-cyber-box { position: relative; background: white; border: 1px solid #e2e8f0; border-radius: 16px; width: 300px; box-shadow: 0 4px 6px rgba(0,0,0,0.02); }
.search-cyber-box i { position: absolute; left: 15px; top: 50%; transform: translateY(-50%); color: #cbd5e1; }
.search-cyber-box input { width: 100%; border: none; padding: 12px 15px 12px 45px; outline: none; background: transparent; font-weight: 600; font-size: 14px; }

/* BUTTONS */
.btn-manage-cat { background: white; border: 1px solid #e2e8f0; padding: 12px 20px; border-radius: 16px; font-weight: 800; color: #64748b; font-size: 12px; transition: 0.3s; cursor: pointer; }
.btn-manage-cat:hover { border-color: #eab308; color: #eab308; transform: translateY(-2px); }
.btn-primary-gradient { background: linear-gradient(135deg, #eab308 0%, #facc15 100%); color: #0f172a; border: none; padding: 13px 25px; border-radius: 16px; font-weight: 800; font-size: 13px; transition: 0.3s; cursor: pointer; }
.btn-primary-gradient:hover { transform: translateY(-2px); box-shadow: 0 10px 25px rgba(234,179,8,0.3); }
.btn-primary-gradient:disabled { opacity: 0.6; cursor: not-allowed; transform: none; }

/* KPI */
.kpi-glass-card { background: rgba(255,255,255,0.85); backdrop-filter: blur(10px); border: 1px solid white; border-radius: 26px; padding: 25px; display: flex; align-items: center; gap: 20px; box-shadow: 0 10px 30px rgba(0,0,0,0.03); }
.icon-wrap { width: 55px; height: 55px; border-radius: 18px; display: flex; align-items: center; justify-content: center; font-size: 22px; }
.data-wrap .value { font-size: 28px; font-weight: 900; color: #0f172a; line-height: 1; display: block; }
.data-wrap .label { font-size: 11px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-top: 5px; }

/* FILTERS */
.filter-cyber-bar { background: rgba(255,255,255,0.7); backdrop-filter: blur(10px); border: 1px solid white; padding: 15px 25px; border-radius: 20px; }
.filter-label { font-size: 11px; font-weight: 900; color: #94a3b8; }
.chip { background: white; border: 1px solid #e2e8f0; padding: 8px 18px; border-radius: 14px; font-size: 12px; font-weight: 700; color: #64748b; transition: 0.3s; margin-right: 8px; cursor: pointer; }
.chip.active { background: #0f172a; color: white; border-color: #0f172a; }
.cyber-select-minimal { background: transparent; border: none; font-weight: 800; font-size: 12px; color: #0f172a; outline: none; }

/* ASSET CARDS */
.asset-card-cyber { background: rgba(255,255,255,0.9); border: 1px solid white; border-radius: 30px; padding: 28px; height: 100%; transition: 0.4s; display: flex; flex-direction: column; box-shadow: 0 4px 20px rgba(0,0,0,0.02); }
.asset-card-cyber:hover { transform: translateY(-8px); border-color: #eab308; box-shadow: 0 20px 40px rgba(234,179,8,0.1); }
.badge-cat { font-size: 9px; font-weight: 900; background: #fffbeb; color: #eab308; padding: 6px 14px; border-radius: 12px; text-transform: uppercase; }
.btn-card-action { width: 34px; height: 34px; background: white; border: 1px solid #f1f5f9; border-radius: 10px; font-size: 12px; transition: 0.3s; color: #94a3b8; cursor: pointer; }
.btn-card-action:hover { border-color: #eab308; color: #eab308; }
.card-main { flex-grow: 1; }
.question-text { font-size: 17px; font-weight: 700; color: #1e293b; margin: 22px 0; line-height: 1.5; min-height: 50px; }
.type-badge-sm { font-size: 11px; font-weight: 700; color: #94a3b8; background: #f8fafc; padding: 6px 12px; border-radius: 10px; width: fit-content; }
.cyber-progress-container { height: 7px; background: #f1f5f9; border-radius: 10px; overflow: hidden; margin-top: 8px; }
.cyber-progress-container .fill { height: 100%; border-radius: 10px; transition: 1s ease; }
.poids-label { font-size: 10px; font-weight: 800; color: #cbd5e1; text-transform: uppercase; }
.poids-val { font-size: 11px; font-weight: 900; }

/* MODAL */
.modal-cyber-overlay { position: fixed; inset: 0; background: rgba(15,23,42,0.45); backdrop-filter: blur(12px); z-index: 9999; display: flex; align-items: center; justify-content: center; padding: 20px; }
.modal-cyber-card { background: white; border-radius: 35px; border: 1px solid white; box-shadow: 0 40px 100px rgba(0,0,0,0.15); overflow: hidden; width: 100%; }
.modal-cyber-header { padding: 30px 40px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
.modal-cyber-header h4 { font-weight: 800; font-size: 20px; letter-spacing: -0.5px; }
.modal-cyber-header h4 span { color: #eab308; }
.btn-close-cyber { background: none; border: none; font-size: 32px; color: #cbd5e1; font-weight: 300; cursor: pointer; line-height: 1; }
.cat-manager-modal { max-width: 500px; }
.cat-list-container { max-height: 300px; overflow-y: auto; display: flex; flex-wrap: wrap; gap: 8px; }
.cat-pill-item { background: #f8fafc; border: 1px solid #e2e8f0; padding: 8px 16px; border-radius: 12px; font-size: 13px; font-weight: 700; display: flex; align-items: center; gap: 10px; }
.btn-del-mini { background: none; border: none; color: #f87171; font-size: 18px; line-height: 1; cursor: pointer; }
.master-config-modal { max-width: 850px; }
.modal-cyber-body { padding: 40px; max-height: 75vh; overflow-y: auto; }
.label-cyber { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; margin-bottom: 12px; display: block; }
.type-visual-selector { display: grid; grid-template-columns: repeat(auto-fill, minmax(130px, 1fr)); gap: 12px; margin-bottom: 25px; }
.type-v-card { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 18px; padding: 15px; text-align: center; cursor: pointer; transition: 0.3s; }
.type-v-card i { font-size: 22px; margin-bottom: 10px; display: block; color: #94a3b8; }
.type-v-card span { font-size: 11px; font-weight: 800; text-transform: uppercase; color: #64748b; }
.type-v-card.active { border-color: #eab308; background: #fffbeb; }
.type-v-card.active i, .type-v-card.active span { color: #eab308; }
.cyber-input-area, .cyber-field-simple, .cyber-select-field { width: 100%; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 16px; padding: 14px 20px; font-weight: 600; font-size: 14px; outline: none; transition: 0.3s; font-family: inherit; }
.cyber-input-area:focus, .cyber-field-simple:focus, .cyber-select-field:focus { border-color: #eab308; background: white; }
.response-cyber-row { display: flex; align-items: center; gap: 15px; margin-bottom: 12px; }
.cyber-check-input { width: 22px; height: 22px; accent-color: #eab308; cursor: pointer; flex-shrink: 0; }
.btn-add-cyber { background: #0f172a; color: white; border: none; padding: 8px 18px; border-radius: 12px; font-weight: 800; font-size: 11px; transition: 0.3s; cursor: pointer; }
.btn-del-cyber { background: none; border: none; color: #f87171; font-size: 20px; cursor: pointer; }
.alert-cyber { background: #f0f9ff; border: 1px solid #e0f2fe; color: #0369a1; padding: 12px 18px; border-radius: 14px; font-size: 12px; font-weight: 700; }
.modal-cyber-footer { padding: 30px 40px; border-top: 1px solid #f1f5f9; display: flex; justify-content: flex-end; gap: 15px; background: #fafafa; }
.btn-glass-cancel { background: #f1f5f9; border: none; padding: 12px 25px; border-radius: 15px; font-weight: 800; color: #64748b; cursor: pointer; }
.cyber-range { accent-color: #eab308; width: 100%; }
.field-error-msg { font-size: 11px; color: #f87171; font-weight: 700; margin-top: 4px; display: block; }

/* CONFIRM */
.confirm-modal { background: white; border-radius: 32px; padding: 40px; width: 420px; max-width: 95vw; text-align: center; box-shadow: 0 40px 80px rgba(0,0,0,0.15); }
.btn-confirm-danger { background: #f43f5e; color: white; border: none; padding: 12px 24px; border-radius: 14px; font-weight: 800; cursor: pointer; }

/* TOAST */
.enigma-toast { position: fixed; bottom: 30px; right: 30px; background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px; display: flex; align-items: center; gap: 15px; z-index: 10001; border-left: 5px solid #eab308; }
.t-success { border-left-color: #22c55e; }
.t-error   { border-left-color: #f87171; }
.t-warn    { border-left-color: #eab308; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* SPINNER */
.cyber-spinner { width: 50px; height: 50px; border: 4px solid #f1f5f9; border-top: 4px solid #eab308; border-radius: 50%; animation: spin 1s linear infinite; margin: auto; }
@keyframes spin { to { transform: rotate(360deg); } }



/* STAGGER */
.stagger-enter-active { animation: fadeInUp 0.5s ease forwards; }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }

/* MODAL TRANSITION */
.modal-fade-enter-active, .modal-fade-leave-active { transition: opacity 0.3s; }
.modal-fade-enter-from, .modal-fade-leave-to { opacity: 0; }

.text-amber { color: #eab308; }
</style>