<template>
  <div class="admin-layout d-flex min-vh-100">

    <!-- ARRIÈRE-PLAN DYNAMIQUE -->
    <div class="bg-orchestrator">
      <div class="hero-bg-grid"></div>
      <div class="hero-orb hero-orb-1"></div>
      <div class="hero-orb hero-orb-2"></div>
      <div class="hero-orb hero-orb-3"></div>
    </div>

    <AppSidebar class="luxury-sidebar" />

    <div class="main-content flex-grow-1 p-4 p-lg-5 position-relative">
      <AppNavbar />

      <!-- HEADER -->
      <header class="header-main animate__animated animate__fadeInDown">
        <div class="brand-section">
          <div class="status-badge-premium">
            <span class="pulse-dot"></span>
            <span class="badge-text">DATABASE-SYNCHRONIZED</span>
          </div>
          <h2 class="brand-title">Evalua<span>Tech</span> <small class="vault-text">Vault V2.0</small></h2>
          <p class="brand-subtitle">RÉFÉRENTIEL CENTRALISÉ DES ÉVALUATIONS TECHNIQUES</p>
        </div>

        <div class="header-actions">
          <div class="search-cyber-container">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher un actif technique...">
            <div class="search-kbd">/</div>
          </div>
          <div class="btn-group-premium">
            <button class="btn-ai-magic shadow-cyan" @click="showAIModal = true">
              <i class="fa-solid fa-wand-magic-sparkles"></i> <span>GÉRER PAR IA</span>
            </button>
            <button class="btn-glass-secondary" @click="showCatManager = true">
              <i class="fa-solid fa-folder-tree"></i> <span>CATÉGORIES</span>
            </button>
            <button class="btn-amber-primary shadow-amber" @click="openModal()">
              <i class="fa-solid fa-plus-circle"></i> <span>NOUVEL ACTIF</span>
            </button>
          </div>
        </div>
      </header>

      <!-- KPI SECTION -->
      <div class="row g-4 mb-5 animate__animated animate__fadeInUp">
        <div class="col-xl-3 col-md-6" v-for="(stat, idx) in quickStats" :key="idx">
          <div class="kpi-card-modern" :style="{ '--accent': stat.color }">
            <div class="kpi-icon-box" :style="{ background: stat.bg, color: stat.color }">
              <i :class="stat.icon"></i>
            </div>
            <div class="kpi-info">
              <div class="kpi-val">{{ stat.value }}</div>
              <div class="kpi-lab">{{ stat.label }}</div>
            </div>
            <div class="kpi-deco-line"></div>
          </div>
        </div>
      </div>

      <!-- FILTRES AVANCÉS -->
      <div class="filter-bar-pro animate__animated animate__fadeIn">
        <div class="filter-left">
          <span class="filter-label"><i class="fa-solid fa-sliders"></i> FORMAT :</span>
          <div class="chips-wrapper">
            <button @click="activeFilter = -1" :class="['premium-chip', { active: activeFilter === -1 }]">TOUS</button>
            <button
              v-for="t in typeDefinitions" :key="t.val"
              @click="activeFilter = t.val"
              :class="['premium-chip', { active: activeFilter === t.val }]">
              <i :class="t.icon"></i> {{ t.label.toUpperCase() }}
            </button>
          </div>
        </div>
        <div class="filter-right">
          <div class="select-premium-box">
            <i class="fa-solid fa-tags"></i>
            <select v-model="selectedCat" class="select-clean">
              <option value="All">TOUTES LES CATÉGORIES</option>
              <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom.toUpperCase() }}</option>
            </select>
          </div>
        </div>
      </div>

      <!-- GRILLE D'ACTIFS -->
      <div class="row g-4 mt-2">
        <div v-if="loading" class="col-12 text-center py-5">
          <div class="tech-loader"></div>
          <div class="loading-label mt-3">INTERROGATION DU COFFRE...</div>
        </div>

        <div v-else-if="filteredQuestions.length === 0" class="col-12 empty-state-box animate__animated animate__zoomIn">
          <div class="empty-icon"><i class="fa-solid fa-database"></i></div>
          <h3>AUCUN RÉSULTAT</h3>
          <p>Désolé, nous n'avons trouvé aucun actif correspondant à vos filtres.</p>
        </div>

        <transition-group v-else name="stagger-list" tag="div" class="row g-4 m-0 p-0">
          <div v-for="(q, i) in filteredQuestions" :key="q.id" class="col-xl-4 col-md-6" :style="{ '--order': i }">
            <div class="asset-card-pro">
              <div class="card-glow-effect"></div>
              <div class="card-header-pro">
                <span class="cat-pill"><i class="fa-solid fa-folder-open"></i> {{ q.theme }}</span>
                <div class="card-actions">
                  <button @click="openModal(q)" class="action-btn edit" title="Modifier"><i class="fa-solid fa-pen-to-square"></i></button>
                  <button @click="handleDelete(q.id)" class="action-btn del" title="Supprimer"><i class="fa-solid fa-trash"></i></button>
                </div>
              </div>
              <div class="card-body-pro">
                <h5 class="question-title">{{ q.enonce }}</h5>
                <div class="type-badge-premium">
                  <div class="type-icon"><i :class="getTypeInfo(q.type).icon"></i></div>
                  <span>{{ getTypeInfo(q.type).label }}</span>
                </div>
              </div>
              <div class="card-footer-pro">
                <div class="complexity-stat">
                  <div class="comp-label">NIVEAU TECHNIQUE <span>{{ q.points }}/5</span></div>
                  <div class="comp-bar-bg">
                    <div class="comp-bar-fill" :style="{ width: (q.points * 20) + '%', background: getWeightColor(q.points) }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </transition-group>
      </div>

      <!-- MODALE GÉNÉRATEUR IA -->
      <transition name="modal-premium">
        <div v-if="showAIModal" class="modal-overlay-p" @click.self="showAIModal = false">
          <div class="modal-card-p wide-modal animate__animated animate__zoomIn">
            <div class="modal-header-p">
              <div class="header-icon-box cyan"><i class="fa-solid fa-brain-circuit"></i></div>
              <h3>Génération <span>Automatique</span> IA</h3>
              <button @click="showAIModal = false" class="btn-close-modal">&times;</button>
            </div>
            <div class="modal-body-p custom-scrollbar">
              <div class="row g-4">
                <div class="col-12">
                  <label class="p-input-label">FORMAT DE L'ACTIF À GÉNÉRER</label>
                  <div class="p-type-grid">
                    <div v-for="t in typeDefinitions" :key="t.val"
                         class="p-type-item" :class="{ active: aiForm.type === t.val }"
                         @click="aiForm.type = t.val">
                      <i :class="t.icon"></i>
                      <span>{{ t.label }}</span>
                    </div>
                  </div>
                </div>

                <div class="col-md-6">
                  <label class="p-input-label">CATÉGORIE CIBLE</label>
                  <select v-model="aiForm.theme" class="p-select" @change="aiForm.sousTheme = ''">
                    <option value="">-- Choisir catégorie --</option>
                    <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                  </select>
                </div>

                <div class="col-md-6">
                  <label class="p-input-label">SOUS-THÈME</label>
                  <select v-model="aiForm.sousTheme" class="p-select" :disabled="!aiForm.theme">
                    <option value="">-- Choisir sous-thème --</option>
                    <option v-for="sub in aiDynamicSubCategories" :key="sub.id" :value="sub.nom">{{ sub.nom }}</option>
                  </select>
                </div>

                <div class="col-md-6">
                  <label class="p-input-label">QUANTITÉ DE QUESTIONS</label>
                  <input v-model.number="aiForm.n" type="number" min="1" max="20" class="p-input-clean">
                </div>
                <div class="col-md-6">
                  <label class="p-input-label">LANGUE DES ÉNONCÉS</label>
                  <select v-model="aiForm.langue" class="p-select">
                    <option value="fr">Français (FR)</option>
                    <option value="en">English (EN)</option>
                  </select>
                </div>

                <div class="col-12" v-if="isAILoading">
                  <div class="tech-loader"></div>
                  <p class="text-center font-monospace mt-3" style="color: #0891b2; font-size: 12px; font-weight: 700;">
                    MOTEUR IA EN ACTION : GÉNÉRATION DU CONTENU TECHNIQUE...
                  </p>
                </div>
              </div>
            </div>
            <div class="modal-footer-p">
              <button @click="showAIModal = false" class="btn-p-cancel">ANNULER</button>
              <button @click="handleAIGeneration" :disabled="isAILoading || !aiForm.theme || !aiForm.sousTheme" class="btn-p-save btn-cyan-magic">
                <i v-if="isAILoading" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                LANCER LE MOTEUR IA
              </button>
            </div>
          </div>
        </div>
      </transition>

      <!-- MODALE GESTIONNAIRE CATÉGORIES -->
      <transition name="modal-premium">
        <div v-if="showCatManager" class="modal-overlay-p" @click.self="showCatManager = false">
          <div class="modal-card-p animate__animated animate__fadeInUp">
            <div class="modal-header-p">
              <div class="header-icon-box"><i class="fa-solid fa-sitemap"></i></div>
              <h3>Gérer les <span>Structures</span></h3>
              <button @click="showCatManager = false" class="btn-close-modal">&times;</button>
            </div>
            <div class="modal-body-p custom-scrollbar">
              <div class="input-add-group mb-5">
                <input v-model="newCatName" @keyup.enter="addCategory" placeholder="Nom de la nouvelle catégorie...">
                <button @click="addCategory"><i class="fa-solid fa-plus me-1"></i> CRÉER</button>
              </div>
              <div class="row g-4">
                <div v-for="cat in categoriesList" :key="cat.id" class="col-md-6">
                  <div class="struct-card-p">
                    <div class="struct-head-p">
                      <strong>{{ cat.nom }}</strong>
                      <button @click="removeCategory(cat.id)" class="btn-del-mini"><i class="fa-solid fa-xmark"></i></button>
                    </div>
                    <div class="struct-sub-list">
                      <div v-for="sub in cat.sousCategories" :key="sub.id" class="sub-item-p">
                        <span>{{ sub.nom }}</span>
                        <i class="fa-solid fa-trash-can" @click="removeSubCategory(sub.id)"></i>
                      </div>
                    </div>
                    <div class="struct-add-box">
                      <input v-model="subCatInputs[cat.id]" @keyup.enter="handleSubAdd(cat.id)" placeholder="Ajouter sous-thème...">
                      <i class="fa-solid fa-plus-circle" @click="handleSubAdd(cat.id)"></i>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </transition>

      <!-- MODALE ÉDITION/CRÉATION MANUELLE -->
      <transition name="modal-premium">
        <div v-if="showModal" class="modal-overlay-p" @click.self="showModal = false">
          <div class="modal-card-p wide-modal animate__animated animate__zoomIn">
            <div class="modal-header-p">
              <div class="header-icon-box amber"><i class="fa-solid fa-sliders"></i></div>
              <h3>{{ isEdit ? 'Configuration' : 'Nouveau' }} <span>Composant</span></h3>
              <button @click="showModal = false" class="btn-close-modal">&times;</button>
            </div>
            <div class="modal-body-p custom-scrollbar">
              <div class="row g-4">
                <div class="col-12">
                  <label class="p-input-label">FORMAT DE L'ACTIF</label>
                  <div class="p-type-grid">
                    <div v-for="t in typeDefinitions" :key="t.val"
                         class="p-type-item" :class="{ active: form.type === t.val }"
                         @click="handleTypeChange(t.val)">
                      <i :class="t.icon"></i>
                      <span>{{ t.label }}</span>
                    </div>
                  </div>
                </div>

                <div class="col-12">
                  <label class="p-input-label">ÉNONCÉ TECHNIQUE *</label>
                  <textarea v-model="form.enonce" class="p-textarea" rows="3" placeholder="Saisir la problématique technique..."></textarea>
                </div>

                <div class="col-md-6">
                  <label class="p-input-label">CATÉGORIE</label>
                  <select v-model="form.theme" class="p-select">
                    <option value="">Sélectionner...</option>
                    <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                  </select>
                </div>

                <div class="col-md-6">
                  <label class="p-input-label">SOUS-THÈME</label>
                  <select v-model="form.sousTheme" class="p-select" :disabled="!form.theme">
                    <option value="">Aucun</option>
                    <option v-for="sub in dynamicSubCategories" :key="sub.id" :value="sub.nom">{{ sub.nom }}</option>
                  </select>
                </div>

                <div class="col-12">
                  <div class="p-range-box">
                    <label class="p-input-label d-flex justify-content-between">NIVEAU DE COMPLEXITÉ <span>{{ form.points }} PTS</span></label>
                    <input type="range" min="1" max="5" step="1" v-model.number="form.points" class="p-range">
                  </div>
                </div>

                <!-- QCM OPTIONS -->
                <div class="col-12" v-if="[0, 1, 2].includes(form.type)">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <label class="p-input-label m-0">PARAMÈTRES DE RÉPONSE</label>
                    <button v-if="form.type !== 2" @click="addResponse" class="btn-p-add-opt"><i class="fa-solid fa-plus me-1"></i> OPTION</button>
                  </div>
                  <div class="p-options-list">
                    <div v-for="(rep, index) in form.reponses" :key="index" class="p-option-row">
                      <div class="p-check-wrap">
                        <input v-if="form.type === 1" type="checkbox" v-model="rep.estCorrecte">
                        <input v-else type="radio" :name="`radio-${form.id}`" :value="index" v-model="correctRadioIndex">
                      </div>
                      <input v-model="rep.texte" class="p-input-clean" placeholder="Valeur de la réponse...">
                      <button v-if="form.type !== 2" @click="removeResponse(index)" class="btn-p-del-opt"><i class="fa-solid fa-trash-can"></i></button>
                    </div>
                  </div>
                </div>

                <!-- CODE EDITOR MODE -->
                <div class="col-12" v-if="[4, 5, 6].includes(form.type)">
                  <label class="p-input-label">{{ form.type === 5 ? 'CODE SOURCE RÉFÉRENT' : 'SOLUTION ATTENDUE' }}</label>
                  <div class="p-editor-container">
                    <div class="editor-bar">
                      <span><i class="fa-solid fa-terminal me-2"></i> SYSTEM_OUTPUT.SH</span>
                      <i class="fa-solid fa-expand-arrows-alt"></i>
                    </div>
                    <textarea v-model="form.bonneReponse" class="p-editor-area font-monospace" :rows="form.type === 5 ? 12 : 6" :placeholder="getPlaceholderForType(form.type)"></textarea>
                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer-p">
              <button @click="showModal = false" class="btn-p-cancel">FERMER</button>
              <button @click="save" :disabled="isSaving" class="btn-p-save">
                <i v-if="isSaving" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                {{ isSaving ? 'SYNCHRONISATION...' : "SAUVEGARDER L'ACTIF" }}
              </button>
            </div>
          </div>
        </div>
      </transition>

      <!-- TOAST -->
      <transition name="toast-slide">
        <div v-if="toast.active" class="p-toast" :class="toast.type">
          <div class="toast-ico-box"><i :class="toast.icon"></i></div>
          <div class="toast-body">
            <strong>MESSAGE SYSTÈME</strong>
            <p>{{ toast.message }}</p>
          </div>
        </div>
      </transition>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import api from '@/services/api';
import axios from 'axios';

/* --- CONFIGURATIONS TYPES --- */
const typeDefinitions = [
  { val: 0, label: 'Choix Unique',    icon: 'fa-solid fa-circle-dot' },
  { val: 1, label: 'Choix Multiple',  icon: 'fa-solid fa-square-check' },
  { val: 2, label: 'Vrai / Faux',     icon: 'fa-solid fa-toggle-on' },
  { val: 4, label: 'Audit Texte',     icon: 'fa-solid fa-robot' },
  { val: 5, label: 'Code Source',     icon: 'fa-solid fa-code' },
  { val: 6, label: 'Projet',          icon: 'fa-solid fa-folder-open' },
];

/* --- STATES --- */
const questions = ref([]);
const categoriesList = ref([]);
const loading = ref(true);
const isSaving = ref(false);
const showModal = ref(false);
const showCatManager = ref(false);
const isEdit = ref(false);
const searchQuery = ref('');
const activeFilter = ref(-1);
const selectedCat = ref('All');
const newCatName = ref('');
const subCatInputs = reactive({});
const toast = reactive({ active: false, message: '', type: '', icon: '' });

const showAIModal = ref(false);
const isAILoading = ref(false);
const aiForm = reactive({
  theme: '',
  sousTheme: '',
  n: 5,
  langue: 'fr',
  type: 0
});

const form = reactive({
  id: '', enonce: '', type: 0, points: 1, theme: '', sousTheme: '', reponses: [], bonneReponse: ''
});

/* --- COMPUTED --- */
const dynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === form.theme);
  return cat ? cat.sousCategories : [];
});

const aiDynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === aiForm.theme);
  return cat ? cat.sousCategories : [];
});

const correctRadioIndex = computed({
  get: () => form.reponses.findIndex(r => r.estCorrecte),
  set: (idx) => form.reponses.forEach((r, i) => { r.estCorrecte = (i === idx); }),
});

const filteredQuestions = computed(() =>
  questions.value.filter(q => {
    const mSearch = q.enonce?.toLowerCase().includes(searchQuery.value.toLowerCase());
    const mType   = activeFilter.value === -1 || q.type === activeFilter.value;
    const mCat    = selectedCat.value === 'All' || q.theme === selectedCat.value;
    return mSearch && mType && mCat;
  })
);

const quickStats = computed(() => [
  { label: 'Total Actifs',  value: questions.value.length, icon: 'fa-solid fa-database',      color: '#0f172a', bg: 'rgba(15,23,42,0.05)'   },
  { label: 'Filières',      value: categoriesList.value.length, icon: 'fa-solid fa-sitemap',  color: '#3b82f6', bg: 'rgba(59,130,246,0.1)'  },
  { label: 'Niveau Expert', value: questions.value.filter(x => x.points >= 4).length, icon: 'fa-solid fa-bolt-lightning', color: '#ef4444', bg: 'rgba(239,68,68,0.1)' },
  { label: 'Sous-Thèmes',  value: categoriesList.value.reduce((acc, c) => acc + (c.sousCategories?.length || 0), 0), icon: 'fa-solid fa-tags', color: '#eab308', bg: 'rgba(234,179,8,0.1)' },
]);

/* --- METHODS --- */
const fetchData = async () => {
  loading.value = true;
  try {
    const [resQ, resC] = await Promise.all([api.get('/Questions'), api.get('/Categories')]);
    questions.value = resQ.data;
    categoriesList.value = resC.data;
  } catch (e) { showToast('ERREUR RESEAU SERVEUR', 'error'); }
  finally { loading.value = false; }
};

const handleAIGeneration = async () => {
  if (!aiForm.theme || !aiForm.sousTheme) {
    showToast("Veuillez choisir une catégorie et un sous-thème", "error");
    return;
  }
  isAILoading.value = true;
  const token = localStorage.getItem('token');
  try {
    const fd = new FormData();
    fd.append('theme', aiForm.theme);
    fd.append('sousTheme', aiForm.sousTheme);
    fd.append('type', aiForm.type);
    fd.append('n', aiForm.n);
    fd.append('langue', aiForm.langue);

    const response = await axios.post('http://127.0.0.1:8000/ia/generate-ultra', fd);
    const aiQs = response.data.questions;

    for (const q of aiQs) {
      const payload = {
        enonce: q.question,
        type: aiForm.type,
        points: 2,
        theme: aiForm.theme,
        sousTheme: aiForm.sousTheme,
        choix: q.options,
        bonneReponse: q.options ? q.options[q.answer] : q.answer_text
      };
      await api.post('/Questions', payload, { headers: { Authorization: `Bearer ${token}` } });
    }

    showAIModal.value = false;
    await fetchData();
    showToast("Génération synchronisée !", "success");
  } catch (e) {
    showToast("Erreur Moteur IA", "error");
  } finally {
    isAILoading.value = false;
  }
};

const addCategory = async () => {
  if (!newCatName.value.trim()) return;
  try {
    const res = await api.post('/Categories', { nom: newCatName.value });
    categoriesList.value.push(res.data);
    newCatName.value = '';
    showToast('CATÉGORIE AJOUTÉE', 'success');
  } catch (e) { showToast('ÉCHEC CRÉATION', 'error'); }
};

const handleSubAdd = async (catId) => {
  const val = subCatInputs[catId];
  if (val && val.trim()) {
    try {
      const res = await api.post(`/Categories/${catId}/sub`, { nom: val.trim() });
      const cat = categoriesList.value.find(c => c.id === catId);
      cat.sousCategories.push(res.data);
      subCatInputs[catId] = '';
      showToast('SOUS-THÈME AJOUTÉ', 'success');
    } catch (e) { showToast('ÉCHEC AJOUT', 'error'); }
  }
};

const removeCategory = async (id) => {
  if (!confirm("Supprimer cette catégorie ?")) return;
  try {
    await api.delete(`/Categories/${id}`);
    categoriesList.value = categoriesList.value.filter(c => c.id !== id);
    showToast('CATÉGORIE RETIRÉE', 'warn');
  } catch (e) { showToast('ERREUR SUPPRESSION', 'error'); }
};

const removeSubCategory = async (subId) => {
  if (!confirm("Supprimer ce sous-thème ?")) return;
  try {
    await api.delete(`/Categories/sub/${subId}`);
    fetchData();
    showToast('SOUS-THÈME RETIRÉ', 'warn');
  } catch (e) { showToast('ERREUR SUPPRESSION', 'error'); }
};

const handleTypeChange = (newType) => {
  form.type = newType;
  if (newType === 2) {
    form.reponses = [{ texte: 'Vrai', estCorrecte: true }, { texte: 'Faux', estCorrecte: false }];
  } else if ([0, 1].includes(newType)) {
    if (form.reponses.length < 2) form.reponses = [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }];
  } else {
    form.reponses = [];
  }
};

const getPlaceholderForType = (type) => {
  if (type === 4) return "Exemple: Expliquer le cycle de vie...";
  if (type === 5) return "// Solution code...";
  return "Réponse attendue...";
};

const addResponse = () => form.reponses.push({ texte: '', estCorrecte: false });
const removeResponse = (i) => form.reponses.splice(i, 1);

const openModal = (q = null) => {
  isEdit.value = !!q;
  if (q) {
    const clone = JSON.parse(JSON.stringify(q));
    Object.assign(form, clone);
    const rawOptions = q.choix || q.options || [];
    form.reponses = rawOptions.map(opt => ({
      texte: opt,
      estCorrecte: (q.bonneReponse || "").split('|').includes(opt)
    }));
  } else {
    Object.assign(form, {
      id: '', enonce: '', type: 0, points: 1, theme: '', sousTheme: '', bonneReponse: '',
      reponses: [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }]
    });
  }
  showModal.value = true;
};

const save = async () => {
  if (!form.enonce.trim()) return;
  isSaving.value = true;
  try {
    let finalBonneReponse = '';
    if (form.type === 0 || form.type === 2) finalBonneReponse = form.reponses[correctRadioIndex.value]?.texte || '';
    else if (form.type === 1) finalBonneReponse = form.reponses.filter(r => r.estCorrecte).map(r => r.texte).join('|');
    else finalBonneReponse = form.bonneReponse;

    const validChoices = form.reponses.map(r => r.texte).filter(t => t && t.trim() !== '');

    const payload = {
      enonce: form.enonce, type: form.type, points: form.points || 1,
      theme: form.theme, sousTheme: form.sousTheme, choix: validChoices, bonneReponse: finalBonneReponse
    };

    if (isEdit.value) await api.put(`/Questions/${form.id}`, payload);
    else await api.post('/Questions', payload);

    showModal.value = false;
    showToast('ACTIF ENREGISTRÉ', 'success');
    await fetchData();
  } catch (e) { showToast('ERREUR SAUVEGARDE', 'error'); }
  finally { isSaving.value = false; }
};

const handleDelete = async (id) => {
  if (!confirm("Supprimer ?")) return;
  try {
    await api.delete(`/Questions/${id}`);
    showToast('ACTIF SUPPRIMÉ', 'warn');
    await fetchData();
  } catch (e) { showToast('ERREUR RESEAU', 'error'); }
};

const getTypeInfo = (val) => typeDefinitions.find(t => t.val === val) || typeDefinitions[0];
const getWeightColor = (p) => p >= 4 ? '#ef4444' : p >= 2.5 ? '#f59e0b' : '#10b981';

const showToast = (message, type = 'success') => {
  toast.message = message;
  toast.type = `t-${type}`;
  toast.icon = type === 'success' ? 'fa-solid fa-circle-check' : type === 'warn' ? 'fa-solid fa-triangle-exclamation' : 'fa-solid fa-circle-xmark';
  toast.active = true;
  setTimeout(() => { toast.active = false; }, 3000);
};

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;600&display=swap');

/* ═══════════════════════════════════════════
   BASE LAYOUT
═══════════════════════════════════════════ */
.admin-layout {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background-color: #f8fafc;
  position: relative;
  overflow-x: hidden;
  color: #1e293b;
}
.main-content { z-index: 10; position: relative; }

/* ═══════════════════════════════════════════
   BACKGROUND ORCHESTRATOR
═══════════════════════════════════════════ */
.bg-orchestrator {
  position: fixed; inset: 0; z-index: 0; pointer-events: none;
}
.hero-bg-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(148,163,184,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148,163,184,0.04) 1px, transparent 1px);
  background-size: 48px 48px;
}
.hero-orb {
  position: absolute; border-radius: 50%;
  filter: blur(110px); opacity: 0.12;
  animation: orb-drift 12s ease-in-out infinite alternate;
}
.hero-orb-1 { width: 560px; height: 560px; background: #fbbf24; top: -8%; right: 3%; animation-delay: 0s; }
.hero-orb-2 { width: 420px; height: 420px; background: #3b82f6; bottom: -8%; left: 2%; animation-delay: -4s; }
.hero-orb-3 { width: 300px; height: 300px; background: #a78bfa; top: 40%; left: 45%; opacity: 0.07; animation-delay: -8s; }

@keyframes orb-drift {
  from { transform: translate(0, 0) scale(1); }
  to   { transform: translate(20px, 30px) scale(1.05); }
}

/* ═══════════════════════════════════════════
   PREMIUM HEADER
═══════════════════════════════════════════ */
.header-main {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 3rem;
  flex-wrap: wrap;
  gap: 20px;
}
.brand-title {
  font-weight: 800;
  font-size: 30px;
  letter-spacing: -1px;
  color: #0f172a;
  line-height: 1.1;
  margin: 0;
}
.brand-title span { color: #eab308; }
.vault-text {
  font-weight: 500;
  color: #94a3b8;
  border-left: 1.5px solid #e2e8f0;
  padding-left: 14px;
  margin-left: 6px;
  font-size: 15px;
  vertical-align: middle;
}
.brand-subtitle {
  color: #94a3b8;
  font-size: 10.5px;
  font-weight: 800;
  letter-spacing: 1.2px;
  margin-top: 4px;
}

.status-badge-premium {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  padding: 5px 14px;
  border-radius: 50px;
  display: inline-flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 10px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.04);
}
.badge-text { font-size: 10px; font-weight: 800; color: #64748b; letter-spacing: 0.8px; }
.pulse-dot {
  width: 7px; height: 7px;
  background: #22c55e;
  border-radius: 50%;
  box-shadow: 0 0 8px #22c55e;
  animation: pulse-glow 2s infinite;
}
@keyframes pulse-glow {
  0%,100% { opacity: 1; transform: scale(1); box-shadow: 0 0 6px #22c55e; }
  50%      { opacity: 0.5; transform: scale(1.3); box-shadow: 0 0 14px #22c55e; }
}

/* ═══════════════════════════════════════════
   HEADER ACTIONS
═══════════════════════════════════════════ */
.header-actions {
  display: flex;
  align-items: center;
  gap: 14px;
  flex-wrap: wrap;
}
.btn-group-premium { display: flex; gap: 10px; align-items: center; }

.search-cyber-container {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 10px 18px;
  display: flex;
  align-items: center;
  gap: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.03);
  transition: all 0.25s;
}
.search-cyber-container:focus-within {
  border-color: #0f172a;
  box-shadow: 0 0 0 3px rgba(15,23,42,0.08);
}
.search-cyber-container i { color: #94a3b8; font-size: 13px; }
.search-cyber-container input {
  border: none; background: none;
  font-size: 13px; outline: none;
  width: 210px; font-weight: 600;
  color: #1e293b;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.search-kbd {
  background: #f1f5f9;
  padding: 2px 8px;
  border-radius: 5px;
  font-size: 10px;
  font-weight: 800;
  color: #94a3b8;
}

/* Buttons */
.btn-ai-magic {
  background: linear-gradient(135deg, #0f172a 0%, #0c4a6e 100%);
  color: #fff;
  border: none;
  padding: 11px 22px;
  border-radius: 13px;
  font-weight: 700;
  font-size: 12px;
  display: flex; align-items: center; gap: 9px;
  transition: all 0.25s;
  cursor: pointer;
  letter-spacing: 0.3px;
}
.btn-ai-magic:hover {
  transform: translateY(-2px);
  box-shadow: 0 10px 28px rgba(8,145,178,0.35);
  background: linear-gradient(135deg, #0f172a 0%, #0891b2 100%);
}
.shadow-cyan { box-shadow: 0 4px 14px rgba(8,145,178,0.2); }

.btn-glass-secondary {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  color: #1e293b;
  padding: 11px 22px;
  border-radius: 13px;
  font-weight: 700;
  font-size: 12px;
  display: flex; align-items: center; gap: 9px;
  transition: all 0.25s;
  cursor: pointer;
}
.btn-glass-secondary:hover {
  border-color: #0f172a;
  transform: translateY(-2px);
  box-shadow: 0 6px 18px rgba(0,0,0,0.06);
}

.btn-amber-primary {
  background: #0f172a;
  color: #fff;
  border: none;
  padding: 11px 26px;
  border-radius: 13px;
  font-weight: 700;
  font-size: 12px;
  display: flex; align-items: center; gap: 9px;
  transition: all 0.25s;
  cursor: pointer;
  letter-spacing: 0.3px;
}
.btn-amber-primary:hover {
  background: #eab308;
  color: #0f172a;
  transform: translateY(-2px);
  box-shadow: 0 10px 28px rgba(234,179,8,0.3);
}
.shadow-amber { box-shadow: 0 4px 14px rgba(15,23,42,0.15); }

/* ═══════════════════════════════════════════
   KPI CARDS
═══════════════════════════════════════════ */
.kpi-card-modern {
  background: rgba(255,255,255,0.75);
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border: 1px solid rgba(255,255,255,0.9);
  padding: 22px 24px;
  border-radius: 22px;
  display: flex;
  align-items: center;
  gap: 18px;
  box-shadow: 0 8px 24px rgba(0,0,0,0.04);
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
  overflow: hidden;
}
.kpi-card-modern::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0;
  height: 2px;
  background: var(--accent);
  opacity: 0;
  transition: opacity 0.3s;
}
.kpi-card-modern:hover { transform: translateY(-5px); border-color: var(--accent); }
.kpi-card-modern:hover::before { opacity: 1; }

.kpi-icon-box {
  width: 52px; height: 52px;
  border-radius: 15px;
  display: flex; align-items: center; justify-content: center;
  font-size: 20px;
  transition: transform 0.3s;
  flex-shrink: 0;
}
.kpi-card-modern:hover .kpi-icon-box { transform: scale(1.1) rotate(-5deg); }
.kpi-val {
  font-size: 27px; font-weight: 800;
  line-height: 1; color: #0f172a;
  letter-spacing: -0.03em;
}
.kpi-lab {
  font-size: 10.5px; font-weight: 700;
  color: #94a3b8; margin-top: 5px;
  text-transform: uppercase; letter-spacing: 0.5px;
}
.kpi-deco-line {
  position: absolute; bottom: 0; left: 0;
  width: 100%; height: 3px;
  background: var(--accent); opacity: 0.08;
}

/* ═══════════════════════════════════════════
   FILTER BAR
═══════════════════════════════════════════ */
.filter-bar-pro {
  background: #ffffff;
  border: 1px solid #e8ecf0;
  border-radius: 18px;
  padding: 11px 22px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-shadow: 0 4px 16px rgba(0,0,0,0.025);
  flex-wrap: wrap;
  gap: 12px;
}
.filter-left { display: flex; align-items: center; gap: 14px; flex-wrap: wrap; }
.filter-label {
  font-size: 10.5px; font-weight: 800;
  color: #94a3b8; letter-spacing: 0.5px;
  white-space: nowrap;
}
.chips-wrapper { display: flex; gap: 7px; flex-wrap: wrap; }
.premium-chip {
  background: #f1f5f9;
  border: 1px solid transparent;
  color: #64748b;
  padding: 6px 16px;
  border-radius: 10px;
  font-size: 10.5px;
  font-weight: 700;
  transition: all 0.2s;
  cursor: pointer;
  display: flex; align-items: center; gap: 6px;
  letter-spacing: 0.2px;
}
.premium-chip:hover { background: #e2e8f0; color: #334155; }
.premium-chip.active {
  background: #0f172a;
  color: #ffffff;
  border-color: #0f172a;
  box-shadow: 0 4px 14px rgba(15,23,42,0.15);
}

.select-premium-box {
  display: flex; align-items: center; gap: 9px;
  background: #f8fafc;
  padding: 7px 14px;
  border-radius: 11px;
  border: 1px solid #e8ecf0;
  transition: border-color 0.2s;
}
.select-premium-box:focus-within { border-color: #0f172a; }
.select-premium-box i { color: #94a3b8; font-size: 12px; }
.select-clean {
  border: none; background: none;
  font-weight: 800; font-size: 10.5px;
  color: #0f172a; outline: none;
  cursor: pointer;
  font-family: 'Plus Jakarta Sans', sans-serif;
  letter-spacing: 0.3px;
}

/* ═══════════════════════════════════════════
   ASSET CARDS
═══════════════════════════════════════════ */
.asset-card-pro {
  background: #ffffff;
  border: 1px solid #edf2f7;
  border-radius: 26px;
  padding: 26px;
  height: 100%;
  display: flex; flex-direction: column;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0,0,0,0.025);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  animation: card-appear 0.5s ease backwards;
  animation-delay: calc(var(--order) * 0.04s);
}
@keyframes card-appear {
  from { opacity: 0; transform: translateY(24px) scale(0.98); }
  to   { opacity: 1; transform: translateY(0) scale(1); }
}
.asset-card-pro::after {
  content: '';
  position: absolute;
  inset: 0;
  border-radius: 26px;
  border: 2px solid transparent;
  transition: border-color 0.3s;
  pointer-events: none;
}
.asset-card-pro:hover {
  transform: translateY(-8px);
  box-shadow: 0 24px 50px rgba(0,0,0,0.07);
}
.asset-card-pro:hover::after { border-color: #eab30850; }

.card-glow-effect {
  position: absolute;
  top: -40px; right: -40px;
  width: 120px; height: 120px;
  background: radial-gradient(circle, rgba(234,179,8,0.08) 0%, transparent 70%);
  pointer-events: none;
}

.card-header-pro {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}
.cat-pill {
  background: #f8fafc;
  color: #64748b;
  font-size: 10px;
  font-weight: 800;
  padding: 5px 12px;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  letter-spacing: 0.2px;
  max-width: 180px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
.card-actions { display: flex; gap: 6px; }
.action-btn {
  width: 33px; height: 33px;
  border-radius: 9px;
  border: 1px solid #f1f5f9;
  background: #fafbfc;
  color: #cbd5e1;
  transition: all 0.2s;
  cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 13px;
}
.action-btn:hover { border-color: #0f172a; color: #0f172a; transform: scale(1.1); }
.action-btn.del:hover { border-color: #ef4444; color: #ef4444; }

.card-body-pro { flex-grow: 1; }
.question-title {
  font-size: 17px;
  font-weight: 700;
  color: #0f172a;
  margin-bottom: 18px;
  line-height: 1.55;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.type-badge-premium {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  color: #64748b;
  font-size: 11px;
  font-weight: 700;
  background: #f8fafc;
  padding: 5px 14px;
  border-radius: 10px;
  border: 1px solid #e2e8f0;
}
.type-icon { font-size: 13px; color: #94a3b8; }

.card-footer-pro { margin-top: 20px; padding-top: 18px; border-top: 1px solid #f1f5f9; }
.comp-label {
  font-size: 9.5px; font-weight: 800;
  color: #cbd5e1;
  display: flex; justify-content: space-between;
  margin-bottom: 8px;
  letter-spacing: 0.5px;
  text-transform: uppercase;
}
.comp-bar-bg {
  height: 5px; background: #f1f5f9;
  border-radius: 10px; overflow: hidden;
}
.comp-bar-fill {
  height: 100%; border-radius: 10px;
  transition: width 1s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}

/* ═══════════════════════════════════════════
   EMPTY & LOADING STATES
═══════════════════════════════════════════ */
.empty-state-box {
  text-align: center;
  padding: 80px 20px;
  color: #94a3b8;
}
.empty-icon {
  font-size: 52px;
  margin-bottom: 20px;
  opacity: 0.15;
}
.empty-state-box h3 {
  font-size: 16px; font-weight: 800;
  letter-spacing: 1px; margin-bottom: 8px;
  color: #cbd5e1;
}
.empty-state-box p { font-size: 13px; }

.tech-loader {
  width: 46px; height: 46px;
  border: 3px solid #f1f5f9;
  border-top-color: #eab308;
  border-radius: 50%;
  animation: spin 0.9s linear infinite;
  margin: 0 auto;
}
@keyframes spin { to { transform: rotate(360deg); } }
.loading-label {
  font-size: 11px; font-weight: 900;
  color: #94a3b8; letter-spacing: 2px;
}

/* ═══════════════════════════════════════════
   MODALS
═══════════════════════════════════════════ */
.modal-overlay-p {
  position: fixed; inset: 0;
  background: rgba(15,23,42,0.45);
  backdrop-filter: blur(10px);
  -webkit-backdrop-filter: blur(10px);
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}
.modal-card-p {
  background: #ffffff;
  border-radius: 32px;
  width: 100%;
  max-width: 650px;
  box-shadow: 0 40px 80px rgba(0,0,0,0.18), 0 10px 30px rgba(0,0,0,0.08);
  overflow: hidden;
  border: 1px solid rgba(255,255,255,0.8);
}
.modal-card-p.wide-modal { max-width: 940px; }

.modal-header-p {
  padding: 28px 40px;
  border-bottom: 1px solid #f1f5f9;
  display: flex;
  align-items: center;
  gap: 18px;
}
.modal-header-p h3 {
  font-size: 22px; font-weight: 800;
  margin: 0; color: #0f172a; flex-grow: 1;
}
.modal-header-p h3 span { color: #eab308; }
.header-icon-box {
  width: 46px; height: 46px;
  background: #0f172a;
  color: #fff;
  border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 18px;
  flex-shrink: 0;
}
.header-icon-box.amber { background: #eab308; color: #0f172a; }
.header-icon-box.cyan  { background: #0891b2; color: #fff; box-shadow: 0 0 18px rgba(8,145,178,0.3); }

.btn-close-modal {
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  color: #94a3b8;
  font-size: 22px;
  width: 38px; height: 38px;
  border-radius: 10px;
  cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.15s;
  line-height: 1;
  flex-shrink: 0;
}
.btn-close-modal:hover { background: #fee2e2; color: #ef4444; border-color: #fca5a5; }

.modal-body-p {
  padding: 36px 40px;
  max-height: 72vh;
  overflow-y: auto;
}
.modal-footer-p {
  padding: 22px 40px;
  background: #fafbfc;
  border-top: 1px solid #f1f5f9;
  display: flex;
  justify-content: flex-end;
  gap: 14px;
}

/* ═══════════════════════════════════════════
   FORM ELEMENTS (MODAL)
═══════════════════════════════════════════ */
.p-input-label {
  font-size: 10.5px;
  font-weight: 800;
  color: #94a3b8;
  margin-bottom: 10px;
  display: block;
  letter-spacing: 0.6px;
  text-transform: uppercase;
}
.p-textarea,
.p-input-clean,
.p-select {
  width: 100%;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 14px;
  padding: 13px 18px;
  color: #0f172a;
  font-weight: 600;
  font-size: 14px;
  outline: none;
  transition: all 0.2s;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.p-textarea:focus,
.p-input-clean:focus,
.p-select:focus {
  border-color: #0f172a;
  background: #fff;
  box-shadow: 0 0 0 3px rgba(15,23,42,0.07);
}
.p-textarea { resize: vertical; }

.p-range-box { }
.p-range {
  width: 100%;
  margin-top: 6px;
  height: 5px;
  appearance: none;
  background: linear-gradient(to right, #eab308 0%, #eab308 calc(var(--pct, 20%) * 1%), #e2e8f0 calc(var(--pct, 20%) * 1%));
  border-radius: 5px;
  cursor: pointer;
  outline: none;
}
.p-range::-webkit-slider-thumb {
  appearance: none;
  width: 18px; height: 18px;
  border-radius: 50%;
  background: #0f172a;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(0,0,0,0.2);
  border: 2px solid #fff;
}

/* Type grid */
.p-type-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(130px, 1fr));
  gap: 12px;
  margin-bottom: 8px;
}
.p-type-item {
  background: #f8fafc;
  border: 1.5px solid #e8ecf0;
  border-radius: 16px;
  padding: 18px 12px;
  text-align: center;
  cursor: pointer;
  transition: all 0.2s;
  display: flex; flex-direction: column;
  align-items: center; gap: 10px;
}
.p-type-item i { font-size: 18px; color: #94a3b8; transition: color 0.2s; }
.p-type-item span { font-size: 11px; font-weight: 700; color: #64748b; }
.p-type-item:hover { border-color: #cbd5e1; background: #fff; }
.p-type-item.active {
  border-color: #0f172a;
  background: #0f172a;
}
.p-type-item.active i { color: #eab308; }
.p-type-item.active span { color: #fff; }

/* Options list */
.p-options-list { display: flex; flex-direction: column; gap: 10px; }
.p-option-row {
  display: flex;
  align-items: center;
  gap: 12px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 10px 14px;
  transition: border-color 0.2s;
}
.p-option-row:focus-within { border-color: #0f172a; background: #fff; }
.p-check-wrap input { width: 16px; height: 16px; cursor: pointer; accent-color: #0f172a; }
.p-option-row .p-input-clean {
  flex-grow: 1; border: none; background: none;
  padding: 4px 0; border-radius: 0;
  font-size: 13px;
}
.p-option-row .p-input-clean:focus { box-shadow: none; }
.btn-p-add-opt {
  background: #f1f5f9;
  border: 1px solid #e2e8f0;
  color: #475569;
  padding: 7px 16px;
  border-radius: 10px;
  font-size: 12px;
  font-weight: 700;
  cursor: pointer;
  transition: all 0.2s;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.btn-p-add-opt:hover { background: #0f172a; color: #fff; border-color: #0f172a; }
.btn-p-del-opt {
  background: none; border: none;
  color: #cbd5e1; cursor: pointer;
  font-size: 14px; transition: color 0.2s;
  padding: 0 2px;
}
.btn-p-del-opt:hover { color: #ef4444; }

/* Code editor */
.p-editor-container {
  border-radius: 16px;
  overflow: hidden;
  border: 1px solid #e2e8f0;
}
.editor-bar {
  background: #1e293b;
  padding: 10px 18px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 11px;
  font-weight: 700;
  color: #64748b;
  font-family: 'JetBrains Mono', monospace;
}
.editor-bar i { color: #475569; }
.p-editor-area {
  width: 100%;
  background: #0f172a;
  border: none;
  border-radius: 0;
  padding: 18px 22px;
  color: #a5f3fc;
  font-family: 'JetBrains Mono', monospace;
  font-size: 13px;
  resize: vertical;
  outline: none;
}

/* Modal footer buttons */
.btn-p-cancel {
  background: #ffffff;
  color: #475569;
  border: 1px solid #d1d5db;
  padding: 12px 26px;
  border-radius: 13px;
  font-weight: 700;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
  font-family: 'Plus Jakarta Sans', sans-serif;
  letter-spacing: 0.3px;
}
.btn-p-cancel:hover { background: #f8fafc; border-color: #94a3b8; }
.btn-p-save {
  background: #0f172a;
  color: #ffffff;
  border: none;
  padding: 12px 32px;
  border-radius: 13px;
  font-weight: 800;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.25s;
  font-family: 'Plus Jakarta Sans', sans-serif;
  display: flex; align-items: center;
  letter-spacing: 0.3px;
}
.btn-p-save:hover {
  background: #eab308;
  color: #0f172a;
  transform: translateY(-2px);
  box-shadow: 0 8px 22px rgba(234,179,8,0.3);
}
.btn-p-save:disabled { opacity: 0.6; cursor: not-allowed; transform: none; }
.btn-cyan-magic { background: #0891b2 !important; color: #fff !important; }
.btn-cyan-magic:hover { background: #0e7490 !important; }

/* ═══════════════════════════════════════════
   CATEGORY MANAGER
═══════════════════════════════════════════ */
.input-add-group {
  display: flex; gap: 10px;
}
.input-add-group input {
  flex-grow: 1;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 12px 18px;
  font-size: 13px;
  font-weight: 600;
  color: #0f172a;
  outline: none;
  transition: border-color 0.2s;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.input-add-group input:focus { border-color: #0f172a; }
.input-add-group button {
  background: #0f172a;
  color: #fff;
  border: none;
  padding: 12px 22px;
  border-radius: 12px;
  font-weight: 700;
  font-size: 12px;
  cursor: pointer;
  transition: all 0.2s;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.input-add-group button:hover { background: #eab308; color: #0f172a; }

.struct-card-p {
  background: #f8fafc;
  border: 1px solid #e8ecf0;
  border-radius: 18px;
  padding: 18px;
  transition: border-color 0.2s;
}
.struct-card-p:hover { border-color: #cbd5e1; }
.struct-head-p {
  display: flex; justify-content: space-between;
  align-items: center; margin-bottom: 14px;
}
.struct-head-p strong { font-size: 13px; font-weight: 800; color: #0f172a; }
.btn-del-mini {
  width: 26px; height: 26px;
  background: #fef2f2; border: 1px solid #fca5a5;
  color: #ef4444; border-radius: 7px;
  cursor: pointer; transition: all 0.15s;
  display: flex; align-items: center; justify-content: center;
  font-size: 12px;
}
.btn-del-mini:hover { background: #ef4444; color: #fff; }
.sub-item-p {
  display: flex; justify-content: space-between;
  align-items: center;
  background: #fff; padding: 7px 13px;
  border-radius: 9px; margin-bottom: 7px;
  font-size: 12px; font-weight: 600;
  border: 1px solid #f1f5f9;
}
.sub-item-p i { color: #ef4444; cursor: pointer; opacity: 0.4; transition: opacity 0.2s; font-size: 12px; }
.sub-item-p i:hover { opacity: 1; }
.struct-add-box {
  display: flex; align-items: center; gap: 10px;
  margin-top: 12px; padding-top: 12px;
  border-top: 1px dashed #e2e8f0;
}
.struct-add-box input {
  flex-grow: 1;
  border: 1px solid #e2e8f0;
  border-radius: 9px;
  padding: 7px 12px;
  font-size: 12px;
  font-weight: 600;
  outline: none;
  background: #fff;
  font-family: 'Plus Jakarta Sans', sans-serif;
}
.struct-add-box i { font-size: 20px; color: #0891b2; cursor: pointer; transition: color 0.2s; }
.struct-add-box i:hover { color: #0e7490; }

/* ═══════════════════════════════════════════
   TOAST NOTIFICATIONS
═══════════════════════════════════════════ */
.p-toast {
  position: fixed; bottom: 36px; right: 36px;
  background: #ffffff;
  border-left: 4px solid #0f172a;
  padding: 18px 24px;
  border-radius: 18px;
  display: flex; align-items: center; gap: 18px;
  z-index: 9999;
  box-shadow: 0 16px 48px rgba(0,0,0,0.12);
  min-width: 280px;
}
.p-toast.t-success { border-color: #22c55e; }
.p-toast.t-error   { border-color: #ef4444; }
.p-toast.t-warn    { border-color: #f59e0b; }
.toast-ico-box {
  font-size: 20px;
  color: #22c55e;
}
.p-toast.t-error .toast-ico-box { color: #ef4444; }
.p-toast.t-warn .toast-ico-box  { color: #f59e0b; }
.toast-body strong {
  font-size: 10px; font-weight: 900;
  letter-spacing: 0.8px; color: #94a3b8;
  display: block; margin-bottom: 3px;
}
.toast-body p { font-size: 13px; font-weight: 600; color: #0f172a; margin: 0; }

/* ═══════════════════════════════════════════
   TRANSITIONS
═══════════════════════════════════════════ */
.modal-premium-enter-active,
.modal-premium-leave-active { transition: all 0.28s cubic-bezier(0.4, 0, 0.2, 1); }
.modal-premium-enter-from,
.modal-premium-leave-to { opacity: 0; }
.modal-premium-enter-from .modal-card-p { transform: scale(0.96) translateY(10px); }

.toast-slide-enter-active,
.toast-slide-leave-active { transition: all 0.3s ease; }
.toast-slide-enter-from { opacity: 0; transform: translateY(14px) translateX(10px); }
.toast-slide-leave-to   { opacity: 0; transform: translateX(20px); }

.stagger-list-enter-active { transition: all 0.4s ease; transition-delay: calc(var(--order) * 0.04s); }
.stagger-list-enter-from   { opacity: 0; transform: translateY(20px); }
.stagger-list-leave-to     { opacity: 0; }

/* ═══════════════════════════════════════════
   SCROLLBAR
═══════════════════════════════════════════ */
.custom-scrollbar::-webkit-scrollbar { width: 5px; }
.custom-scrollbar::-webkit-scrollbar-track { background: #f8fafc; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: #cbd5e1; }

/* ═══════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════ */
[data-theme="dark"] .admin-layout     { background-color: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .asset-card-pro  { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .modal-card-p    { background: #161b22; }
[data-theme="dark"] .filter-bar-pro  { background: #161b22; border-color: rgba(255,255,255,0.07); }
[data-theme="dark"] .kpi-card-modern { background: rgba(22,27,34,0.8); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .brand-title,
[data-theme="dark"] .question-title,
[data-theme="dark"] .kpi-val         { color: #f0f6fc; }
[data-theme="dark"] .p-textarea,
[data-theme="dark"] .p-input-clean,
[data-theme="dark"] .p-select        { background: rgba(255,255,255,0.05); color: #f0f6fc; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .search-cyber-container { background: #161b22; border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .p-toast         { background: #161b22; }
</style>