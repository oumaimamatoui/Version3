<template>
  <div class="admin-layout d-flex min-vh-100">
    <!-- DÉCORATION D'ARRIÈRE-PLAN DYNAMIQUE -->
    <div class="bg-orchestrator">
      <div class="hero-bg-grid"></div>
      <div class="hero-orb hero-orb-1"></div>
      <div class="hero-orb hero-orb-2"></div>
      <div class="tech-grid-overlay"></div>
    </div>

    <AppSidebar />

    <div class="main-content flex-grow-1 p-4 p-lg-5 position-relative">
      <AppNavbar />

      <!-- HEADER PREMIUM -->
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
            <button class="btn-glass-secondary" @click="showCatManager = true">
              <i class="fa-solid fa-folder-tree"></i> <span>CATÉGORIES</span>
            </button>
            <button class="btn-amber-primary shadow-amber" @click="openModal()">
              <i class="fa-solid fa-plus-circle"></i> <span>NOUVEL ACTIF</span>
            </button>
          </div>
        </div>
      </header>

      <!-- KPI SECTION (STYLISÉE) -->
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
          <div class="loading-label">INTERROGATION DU COFFRE...</div>
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

      <!-- MODALE ÉDITION/CRÉATION -->
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
                {{ isSaving ? 'SYNCHRONISATION...' : 'SAUVEGARDER L\'ACTIF' }}
              </button>
            </div>
          </div>
        </div>
      </transition>

      <!-- NOTIFICATIONS -->
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

/* --- LOGIQUE JS (Identique à la précédente pour ne rien casser) --- */
const typeDefinitions = [
  { val: 0, label: 'Choix Unique',    icon: 'fa-solid fa-circle-dot' },
  { val: 1, label: 'Choix Multiple',  icon: 'fa-solid fa-square-check' },
  { val: 2, label: 'Vrai / Faux',     icon: 'fa-solid fa-toggle-on' },
  { val: 4, label: 'Audit Texte',     icon: 'fa-solid fa-robot' },
  { val: 5, label: 'Code Source',     icon: 'fa-solid fa-code' },
  { val: 6, label: 'Projet',          icon: 'fa-solid fa-folder-open' },
];

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

const form = reactive({
  id: '', enonce: '', type: 0, points: 1, theme: '', sousTheme: '', reponses: [], bonneReponse: ''
});

const dynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === form.theme);
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
  { label: 'Total Actifs', value: questions.value.length, icon: 'fa-solid fa-database', color: '#0f172a', bg: 'rgba(15,23,42,0.05)' },
  { label: 'Filières', value: categoriesList.value.length, icon: 'fa-solid fa-sitemap', color: '#3b82f6', bg: 'rgba(59,130,246,0.1)' },
  { label: 'Niveau Expert', value: questions.value.filter(x => x.points >= 4).length, icon: 'fa-solid fa-bolt-lightning', color: '#ef4444', bg: 'rgba(239,68,68,0.1)' },
  { label: 'Sous-Thèmes', value: categoriesList.value.reduce((acc, c) => acc + (c.sousCategories?.length || 0), 0), icon: 'fa-solid fa-tags', color: '#eab308', bg: 'rgba(234,179,8,0.1)' },
]);

const fetchData = async () => {
  loading.value = true;
  try {
    const [resQ, resC] = await Promise.all([api.get('/Questions'), api.get('/Categories')]);
    questions.value = resQ.data;
    categoriesList.value = resC.data;
  } catch (e) { showToast('ERREUR RESEAU SERVEUR', 'error'); } 
  finally { loading.value = false; }
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
  if(!confirm("Supprimer cette catégorie ?")) return;
  try {
    await api.delete(`/Categories/${id}`);
    categoriesList.value = categoriesList.value.filter(c => c.id !== id);
    showToast('CATÉGORIE RETIRÉE', 'warn');
  } catch (e) { showToast('ERREUR SUPPRESSION', 'error'); }
};

const removeSubCategory = async (subId) => {
  if(!confirm("Supprimer ce sous-thème ?")) return;
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
  } else if ([0,1].includes(newType)) {
    if(form.reponses.length < 2) form.reponses = [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }];
  } else {
    form.reponses = [];
  }
};

const getPlaceholderForType = (type) => {
  if (type === 4) return "Exemple: Expliquer le cycle de vie d'un composant...";
  if (type === 5) return "// Solution de référence...\nfunction validate() {\n  return true;\n}";
  if (type === 6) return "Livrables projet attendus...";
  return "Réponse attendue...";
};

const addResponse = () => form.reponses.push({ texte: '', estCorrecte: false });
const removeResponse = (i) => form.reponses.splice(i, 1);

const openModal = (q = null) => {
  isEdit.value = !!q;
  if (q) {
    const clone = JSON.parse(JSON.stringify(q));
    Object.assign(form, clone);
    const rawOptions = q.choix || q.Choix || q.options || [];
    if ([0, 1, 2].includes(q.type) && Array.isArray(rawOptions) && rawOptions.length > 0) {
      form.reponses = rawOptions.map(opt => ({
        texte: opt,
        estCorrecte: (q.bonneReponse || "").split('|').includes(opt)
      }));
    }
  } else {
    Object.assign(form, {
      id: '00000000-0000-0000-0000-000000000000',
      enonce: '', type: 0, points: 1, theme: '', sousTheme: '', bonneReponse: '',
      reponses: [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }]
    });
  }
  showModal.value = true;
};

const save = async () => {
  if(!form.enonce.trim()) return;
  const validChoices = form.reponses.map(r => r.texte).filter(t => t && t.trim() !== '');
  if ([0, 1, 2].includes(form.type) && validChoices.length < 2) {
    alert("Veuillez saisir au moins 2 options.");
    return;
  }

  isSaving.value = true;
  try {
    let finalBonneReponse = '';
    if (form.type === 0 || form.type === 2) {
      finalBonneReponse = form.reponses[correctRadioIndex.value]?.texte || '';
    } else if (form.type === 1) {
      finalBonneReponse = form.reponses.filter(r => r.estCorrecte).map(r => r.texte).join('|');
    } else {
      finalBonneReponse = form.bonneReponse;
    }

    const payload = {
      enonce: form.enonce,
      type: form.type,
      points: form.points || 1,
      theme: form.theme,
      sousTheme: form.sousTheme,
      choix: [0, 1].includes(form.type) ? validChoices : [],
      bonneReponse: finalBonneReponse
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
  if(!confirm("Supprimer cet actif ?")) return;
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
  toast.icon = type === 'success' ? 'fa-solid fa-circle-check' : 'fa-solid fa-triangle-exclamation';
  toast.active = true;
  setTimeout(() => { toast.active = false; }, 3000);
};

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;600&display=swap');

/* ----- GLOBAL LAYOUT ----- */
.admin-layout { 
  font-family: 'Plus Jakarta Sans', sans-serif; 
  background-color: #f8fafc; 
  position: relative; 
  overflow-x: hidden;
  color: #1e293b;
}

/* ----- ENHANCED BACKGROUND ----- */
.bg-orchestrator { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.hero-bg-grid { position: absolute; inset: 0; background-image: linear-gradient(rgba(148,163,184,0.05) 1px, transparent 1px), linear-gradient(90deg, rgba(148,163,184,0.05) 1px, transparent 1px); background-size: 50px 50px; }
.hero-orb { position: absolute; border-radius: 50%; filter: blur(100px); opacity: 0.15; }
.hero-orb-1 { width: 500px; height: 500px; background: #fbbf24; top: -5%; right: 5%; }
.hero-orb-2 { width: 400px; height: 400px; background: #3b82f6; bottom: -5%; left: 5%; }
.main-content { z-index: 10; position: relative; }

/* ----- PREMIUM HEADER ----- */
.header-main { display: flex; justify-content: space-between; align-items: center; margin-bottom: 3.5rem; flex-wrap: wrap; gap: 20px; }
.brand-title { font-weight: 800; font-size: 32px; letter-spacing: -1px; color: #0f172a; }
.brand-title span { color: #eab308; }
.vault-text { font-weight: 500; color: #94a3b8; border-left: 1.5px solid #e2e8f0; padding-left: 15px; font-size: 16px; }
.brand-subtitle { color: #64748b; font-size: 11px; font-weight: 800; letter-spacing: 1px; }

.status-badge-premium { background: #fff; border: 1px solid #e2e8f0; padding: 6px 14px; border-radius: 50px; display: flex; align-items: center; gap: 8px; margin-bottom: 12px; width: fit-content; box-shadow: 0 4px 10px rgba(0,0,0,0.03); }
.pulse-dot { width: 7px; height: 7px; background: #22c55e; border-radius: 50%; box-shadow: 0 0 10px #22c55e; animation: pulse 2s infinite; }
@keyframes pulse { 0% { opacity: 1; transform: scale(1); } 50% { opacity: 0.4; transform: scale(1.2); } 100% { opacity: 1; transform: scale(1); } }

/* ----- SEARCH & BUTTONS ----- */
.search-cyber-container { background: #fff; border: 1px solid #e2e8f0; border-radius: 16px; padding: 10px 20px; display: flex; align-items: center; gap: 12px; box-shadow: 0 2px 10px rgba(0,0,0,0.02); transition: 0.3s; }
.search-cyber-container:focus-within { border-color: #0f172a; box-shadow: 0 10px 25px rgba(0,0,0,0.05); }
.search-cyber-container input { border: none; font-size: 14px; outline: none; width: 220px; font-weight: 600; color: #1e293b; }
.search-kbd { background: #f1f5f9; padding: 2px 8px; border-radius: 6px; font-size: 10px; font-weight: 800; color: #94a3b8; }

.btn-amber-primary { background: #0f172a; color: #fff; border: none; padding: 12px 28px; border-radius: 14px; font-weight: 700; font-size: 13px; display: flex; align-items: center; gap: 10px; transition: 0.3s; }
.btn-amber-primary:hover { background: #eab308; color: #0f172a; transform: translateY(-2px); }
.btn-glass-secondary { background: #fff; border: 1px solid #e2e8f0; color: #1e293b; padding: 12px 24px; border-radius: 14px; font-weight: 700; font-size: 13px; transition: 0.3s; }
.btn-glass-secondary:hover { border-color: #0f172a; transform: translateY(-2px); }

/* ----- KPI CARDS MODERN ----- */
.kpi-card-modern {
  background: rgba(255, 255, 255, 0.7);
  backdrop-filter: blur(10px);
  border: 1px solid #fff;
  padding: 24px;
  border-radius: 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.04);
  transition: 0.3s;
  position: relative;
  overflow: hidden;
}
.kpi-card-modern:hover { transform: translateY(-5px); border-color: var(--accent); }
.kpi-icon-box { width: 54px; height: 54px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 22px; transition: 0.3s; }
.kpi-val { font-size: 28px; font-weight: 800; line-height: 1; color: #0f172a; }
.kpi-lab { font-size: 11px; font-weight: 700; color: #94a3b8; margin-top: 4px; text-transform: uppercase; }
.kpi-deco-line { position: absolute; bottom: 0; left: 0; width: 100%; height: 3px; background: var(--accent); opacity: 0.1; }

/* ----- FILTER BAR ----- */
.filter-bar-pro { background: #fff; border: 1px solid #e2e8f0; border-radius: 20px; padding: 12px 25px; display: flex; align-items: center; justify-content: space-between; box-shadow: 0 4px 15px rgba(0,0,0,0.02); }
.filter-label { font-size: 11px; font-weight: 800; color: #94a3b8; margin-right: 15px; }
.chips-wrapper { display: flex; gap: 8px; }
.premium-chip { background: #f1f5f9; border: 1px solid transparent; color: #64748b; padding: 7px 18px; border-radius: 12px; font-size: 11px; font-weight: 700; transition: 0.3s; cursor: pointer; }
.premium-chip:hover { background: #e2e8f0; color: #0f172a; }
.premium-chip.active { background: #0f172a; color: #fff; box-shadow: 0 5px 15px rgba(0,0,0,0.1); }
.select-premium-box { display: flex; align-items: center; gap: 10px; background: #f8fafc; padding: 7px 15px; border-radius: 12px; border: 1px solid #eef2f6; }
.select-clean { border: none; background: none; font-weight: 800; font-size: 11px; color: #0f172a; outline: none; cursor: pointer; }

/* ----- ASSET CARDS PREMIUM ----- */
.asset-card-pro {
  background: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 28px;
  padding: 28px;
  height: 100%;
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: hidden;
  box-shadow: 0 4px 20px rgba(0,0,0,0.02);
  transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  animation: fadeInUp 0.6s ease backwards;
  animation-delay: calc(var(--order) * 0.05s);
}
.asset-card-pro:hover { transform: translateY(-10px); border-color: #eab308; box-shadow: 0 20px 40px rgba(0,0,0,0.05); }

.card-header-pro { display: flex; justify-content: space-between; align-items: center; margin-bottom: 22px; }
.cat-pill { background: #f8fafc; color: #64748b; font-size: 10px; font-weight: 800; padding: 6px 14px; border-radius: 10px; border: 1px solid #e2e8f0; }
.action-btn { width: 34px; height: 34px; border-radius: 10px; border: 1px solid #f1f5f9; background: #fff; color: #cbd5e1; transition: 0.3s; margin-left: 6px; }
.action-btn:hover { border-color: #0f172a; color: #0f172a; transform: scale(1.1); }
.action-btn.del:hover { color: #ef4444; border-color: #ef4444; }

.question-title { font-size: 18px; font-weight: 700; color: #0f172a; margin-bottom: 22px; line-height: 1.5; flex-grow: 1; }
.type-badge-premium { display: flex; align-items: center; gap: 10px; color: #94a3b8; font-size: 12px; font-weight: 700; background: #f8fafc; padding: 6px 16px; border-radius: 12px; width: fit-content; }
.type-icon { color: #0f172a; }

.complexity-stat .comp-label { font-size: 10px; font-weight: 800; color: #cbd5e1; display: flex; justify-content: space-between; margin-bottom: 8px; }
.comp-bar-bg { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.comp-bar-fill { height: 100%; border-radius: 10px; transition: 1s cubic-bezier(0.175, 0.885, 0.32, 1.275); }

/* ----- MODALS PREMIUM ----- */
.modal-overlay-p { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.4); backdrop-filter: blur(8px); z-index: 1000; display: flex; align-items: center; justify-content: center; padding: 20px; }
.modal-card-p { background: #fff; border-radius: 35px; width: 100%; max-width: 650px; box-shadow: 0 50px 100px rgba(0,0,0,0.15); overflow: hidden; border: 1px solid #fff; }
.modal-card-p.wide-modal { max-width: 950px; }

.modal-header-p { padding: 30px 45px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
.header-icon-box { width: 48px; height: 48px; background: #0f172a; color: #fff; border-radius: 15px; display: flex; align-items: center; justify-content: center; font-size: 20px; }
.header-icon-box.amber { background: #eab308; }
.modal-header-p h3 { font-size: 24px; font-weight: 800; margin: 0; color: #0f172a; }
.modal-header-p span { color: #eab308; }
.btn-close-modal { background: none; border: none; color: #cbd5e1; font-size: 35px; cursor: pointer; }

.modal-body-p { padding: 45px; max-height: 75vh; overflow-y: auto; }

/* STUDIO FORM ELEMENTS */
.p-input-label { font-size: 11px; font-weight: 800; color: #94a3b8; margin-bottom: 12px; display: block; letter-spacing: 0.5px; }
.p-textarea, .p-input-clean, .p-select {
  width: 100%; background: #f8fafc; border: 1px solid #e2e8f0;
  border-radius: 18px; padding: 16px 22px; color: #0f172a;
  font-weight: 600; outline: none; transition: 0.3s;
}
.p-textarea:focus, .p-input-clean:focus { border-color: #eab308; background: #fff; }

.p-type-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(140px, 1fr)); gap: 15px; margin-bottom: 30px; }
.p-type-item { background: #f8fafc; border: 2px solid #f1f5f9; border-radius: 20px; padding: 20px; text-align: center; cursor: pointer; transition: 0.3s; }
.p-type-item i { font-size: 24px; color: #cbd5e1; margin-bottom: 10px; display: block; }
.p-type-item span { font-size: 11px; font-weight: 800; color: #94a3b8; }
.p-type-item.active { border-color: #0f172a; background: #fff; }
.p-type-item.active i, .p-type-item.active span { color: #0f172a; }

.p-editor-container { border-radius: 20px; overflow: hidden; border: 2px solid #e2e8f0; }
.editor-bar { background: #f8fafc; padding: 12px 25px; border-bottom: 1px solid #e2e8f0; font-family: 'JetBrains Mono', monospace; font-size: 11px; color: #94a3b8; display: flex; justify-content: space-between; }
.p-editor-area { width: 100%; background: #fafafa; border: none; padding: 25px; color: #0f172a; font-family: 'JetBrains Mono', monospace; font-size: 14px; outline: none; line-height: 1.6; }

.modal-footer-p { padding: 30px 45px; background: #f8fafc; display: flex; justify-content: flex-end; gap: 20px; }
.btn-p-cancel { background: none; border: none; color: #94a3b8; font-weight: 800; cursor: pointer; }
.btn-p-save { background: #0f172a; color: #fff; border: none; padding: 16px 40px; border-radius: 18px; font-weight: 800; transition: 0.3s; }
.btn-p-save:hover { background: #eab308; color: #0f172a; transform: translateY(-2px); }

/* ----- CATEGORY MANAGER CARDS ----- */
.struct-card-p { background: #f8fafc; border-radius: 24px; border: 1.5px solid #eef2f6; padding: 22px; height: 100%; transition: 0.3s; }
.struct-head-p { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eef2f6; padding-bottom: 12px; margin-bottom: 15px; }
.struct-sub-list { min-height: 50px; display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 15px; }
.sub-pill-item { background: #fff; border: 1px solid #e2e8f0; padding: 6px 14px; border-radius: 50px; font-size: 11px; font-weight: 700; display: flex; align-items: center; gap: 8px; }
.sub-pill-item i { color: #ef4444; cursor: pointer; }
.struct-add-box { position: relative; }
.struct-add-box input { width: 100%; padding: 10px 40px 10px 15px; border-radius: 12px; border: 1px solid #e2e8f0; font-size: 12px; outline: none; }
.struct-add-box i { position: absolute; right: 15px; top: 50%; transform: translateY(-50%); color: #eab308; cursor: pointer; font-size: 18px; }

/* ----- TOASTS ----- */
.p-toast { position: fixed; bottom: 40px; right: 40px; background: #fff; border-left: 5px solid #0f172a; padding: 20px 30px; border-radius: 20px; display: flex; align-items: center; gap: 20px; z-index: 9999; box-shadow: 0 15px 50px rgba(0,0,0,0.1); }
.toast-ico-box { width: 42px; height: 42px; background: #f8fafc; border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 18px; }
.p-toast.t-success { border-color: #22c55e; }
.p-toast.t-error { border-color: #ef4444; }

/* ----- ANIMATIONS & LOADERS ----- */
@keyframes fadeInUp { from { opacity: 0; transform: translateY(30px); } to { opacity: 1; transform: translateY(0); } }
.tech-loader { width: 50px; height: 50px; border: 4px solid #f1f5f9; border-top-color: #eab308; border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px; }
@keyframes spin { to { transform: rotate(360deg); } }
.loading-label { font-size: 12px; font-weight: 900; color: #94a3b8; letter-spacing: 2px; }

.font-monospace { font-family: 'JetBrains Mono', monospace !important; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
</style>