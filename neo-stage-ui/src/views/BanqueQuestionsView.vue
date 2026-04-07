<template>
  <div class="admin-layout d-flex">
    <div class="background-overlay"></div>
    <AppSidebar />

    <div class="main-content flex-grow-1 p-4">
      <AppNavbar />

      <!-- HEADER PROFESSIONNEL -->
      <header class="d-flex justify-content-between align-items-center mb-5 animate__animated animate__fadeIn">
        <div>
          <h2 class="brand-title-main">Banque d'<span>Actifs</span></h2>
          <p class="brand-subtitle"><i class="fa-solid fa- microchip me-1"></i> LOGIQUE UML : RÉFÉRENTIEL DES QUESTIONS IA</p>
        </div>
        <div class="d-flex gap-3">
          <div class="search-container-pro">
            <i class="fa-solid fa-magnifying-glass"></i>
            <input v-model="searchQuery" type="text" placeholder="Rechercher par concept, catégorie...">
          </div>
          <button class="btn-primary-pro" @click="openModal()">
            <i class="fa-solid fa-plus-circle me-2"></i> Créer une Question
          </button>
        </div>
      </header>

      <!-- BARRE DE STATS (KPI) -->
      <div class="row g-4 mb-5 animate__animated animate__fadeInUp">
        <div class="col-md-3" v-for="stat in quickStats" :key="stat.label">
          <div class="mini-stat-card">
            <div class="icon-box" :style="{ background: stat.bg, color: stat.color }">
              <i :class="stat.icon"></i>
            </div>
            <div>
              <div class="val">{{ stat.value }}</div>
              <div class="lbl">{{ stat.label }}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- FILTRES DE SEGMENTATION -->
      <div class="filter-strip mb-4">
        <div class="filter-label">Filtrer par Format :</div>
        <div class="chips-group">
          <button v-for="(t, i) in ['Tous', 'QCM', 'Texte', 'Code', 'Fichier']" :key="i" 
            @click="activeFilter = (i === 0 ? null : i-1)"
            :class="['chip-item', { active: activeFilter === (i === 0 ? null : i-1) }]">
            {{ t }}
          </button>
        </div>
      </div>

      <!-- GRILLE DES ACTIFS -->
      <div class="row g-4">
        <div v-if="loading" class="text-center py-5">
           <div class="spinner-pro"></div>
           <p class="mt-3 text-muted fw-bold">Synchronisation UML...</p>
        </div>

        <div v-for="q in filteredQuestions" :key="q.id" class="col-xl-4 col-md-6 animate__animated animate__zoomIn">
          <div class="asset-card-premium">
            <div class="card-top">
              <span class="category-tag">{{ q.categorie || 'SANS CATÉGORIE' }}</span>
              <div class="card-actions">
                <button @click="openModal(q)" class="btn-action-round"><i class="fa-solid fa-pen-nib"></i></button>
                <button @click="handleDelete(q.id)" class="btn-action-round del"><i class="fa-solid fa-trash-can"></i></button>
              </div>
            </div>

            <h5 class="question-text-pro">{{ q.texte }}</h5>

            <div class="difficulty-indicator my-4">
              <div class="d-flex justify-content-between mb-2">
                <span class="small-label">Niveau de Difficulté</span>
                <span class="difficulty-val" :style="{ color: getWeightColor(q.poids) }">{{ getDifficultyLabel(q.poids) }}</span>
              </div>
              <div class="custom-progress">
                <div class="progress-fill" :style="{ width: (q.poids * 20) + '%', background: getWeightColor(q.poids) }"></div>
              </div>
            </div>

            <div class="card-footer-pro pt-3 mt-auto">
              <div class="meta-tag">
                <i :class="getTypeIcon(q.type)"></i> {{ getTypeName(q.type) }}
              </div>
              <div class="parent-link" v-if="getQuestionnaireName(q.questionnaireId)">
                <i class="fa-solid fa-link me-1"></i> {{ getQuestionnaireName(q.questionnaireId) }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- MODAL PROFESSIONNEL -->
      <transition name="fade">
        <div v-if="showModal" class="modal-pro-overlay">
          <div class="modal-pro-card animate__animated animate__fadeInUp">
            <div class="modal-pro-header">
              <h4 class="m-0 fw-800"><i class="fa-solid fa-file-circle-plus text-warning me-2"></i> {{ isEdit ? 'Éditer' : 'Configurer' }} l'Actif</h4>
              <button @click="showModal = false" class="close-btn">&times;</button>
            </div>
            
            <div class="modal-pro-body p-4">
              <div class="row g-4">
                <div class="col-12">
                  <label class="pro-label">Énoncé de la question *</label>
                  <textarea v-model="form.texte" class="pro-input" rows="3" placeholder="Saisir la problématique technique..."></textarea>
                </div>

                <div class="col-md-6">
                  <label class="pro-label">Questionnaire Parent (Logique UML)</label>
                  <select v-model="form.questionnaireId" class="pro-input">
                    <option value="" disabled>-- Sélectionner le test cible --</option>
                    <option v-for="quest in questionnaires" :key="quest.id" :value="quest.id">{{ quest.titre }}</option>
                  </select>
                </div>

                <div class="col-md-6">
                  <label class="pro-label">Format de Question</label>
                  <select v-model.number="form.type" class="pro-input">
                    <option :value="0">QCM</option>
                    <option :value="1">Texte Libre</option>
                    <option :value="2">Code / Algorithme</option>
                    <option :value="3">Téléversement</option>
                  </select>
                </div>

                <div class="col-md-4">
                  <label class="pro-label">Poids (Poids UML : 1-5)</label>
                  <input type="number" step="0.5" v-model.number="form.poids" class="pro-input">
                </div>
                <div class="col-md-4">
                  <label class="pro-label">Catégorie</label>
                  <input v-model="form.categorie" class="pro-input" placeholder="Ex: Backend">
                </div>
                <div class="col-md-4">
                  <label class="pro-label">Clé de Correction (IA)</label>
                  <input v-model="form.bonneReponse" class="pro-input" placeholder="Réponse attendue">
                </div>
              </div>
            </div>

            <div class="modal-pro-footer p-4 border-top text-end">
              <button @click="showModal = false" class="btn-cancel-pro me-3">Annuler</button>
              <button @click="save" class="btn-primary-pro px-5 shadow" :disabled="!form.questionnaireId">
                <i class="fa-solid fa-save me-2"></i> Enregistrer dans le Système
              </button>
            </div>
          </div>
        </div>
      </transition>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted, computed } from 'vue';
import axios from 'axios';

// SERVICES API
const API_QUESTIONS = "http://localhost:5172/api/Questions";
const API_QUESTIONNAIRES = "http://localhost:5172/api/Questionnaires";

const questions = ref([]);
const questionnaires = ref([]);
const loading = ref(true);
const showModal = ref(false);
const isEdit = ref(false);
const searchQuery = ref('');
const activeFilter = ref(null);

const form = reactive({ 
  id: '', texte: '', type: 0, poids: 1.0, categorie: '', bonneReponse: '', 
  questionnaireId: '' 
});

// INITIALISATION
const fetchData = async () => {
  loading.value = true;
  try {
    const [qRes, questRes] = await Promise.all([
      axios.get(API_QUESTIONS),
      axios.get(API_QUESTIONNAIRES)
    ]);
    questions.value = qRes.data;
    questionnaires.value = questRes.data;
  } catch (e) { console.error("Erreur de synchronisation API"); }
  finally { loading.value = false; }
};

// LOGIQUE DE FILTRAGE
const filteredQuestions = computed(() => {
  return questions.value.filter(q => {
    const matchSearch = q.texte.toLowerCase().includes(searchQuery.value.toLowerCase()) || 
                        q.categorie?.toLowerCase().includes(searchQuery.value.toLowerCase());
    const matchType = activeFilter.value === null || q.type === activeFilter.value;
    return matchSearch && matchType;
  });
});

const quickStats = computed(() => [
  { label: 'Total Actifs', value: questions.value.length, icon: 'fa-solid fa-box-archive', color: '#6366f1', bg: '#eef2ff' },
  { label: 'Difficulté Moy.', value: (questions.value.reduce((a, b) => a + b.poids, 0) / (questions.value.length || 1)).toFixed(1), icon: 'fa-solid fa-gauge-high', color: '#f59e0b', bg: '#fffbeb' },
  { label: 'QCM Déployés', value: questions.value.filter(x => x.type === 0).length, icon: 'fa-solid fa-list-check', color: '#10b981', bg: '#ecfdf5' },
  { label: 'Tests Liés', value: questionnaires.value.length, icon: 'fa-solid fa-link', color: '#ef4444', bg: '#fef2f2' },
]);

// HELPERS VISUELS
const getTypeName = (val) => ["QCM", "Texte", "Code", "Fichier"][val] || "Inconnu";
const getTypeIcon = (val) => ["fa-solid fa-list-ul", "fa-solid fa-align-left", "fa-solid fa-terminal", "fa-solid fa-file-arrow-up"][val];
const getQuestionnaireName = (id) => questionnaires.value.find(x => x.id === id)?.titre;
const getWeightColor = (p) => p >= 4 ? '#ef4444' : p >= 2.5 ? '#f59e0b' : '#10b981';
const getDifficultyLabel = (p) => p >= 4 ? 'SENIOR' : p >= 2.5 ? 'INTERMÉDIAIRE' : 'JUNIOR';

const openModal = (q = null) => {
  isEdit.value = !!q;
  if (q) Object.assign(form, q);
  else Object.assign(form, { id: '00000000-0000-0000-0000-000000000000', texte: '', type: 0, poids: 1.0, categorie: '', bonneReponse: '', questionnaireId: questionnaires.value[0]?.id || '' });
  showModal.value = true;
};

const save = async () => {
  try {
    if (isEdit.value) await axios.put(`${API_QUESTIONS}/${form.id}`, form);
    else await axios.post(API_QUESTIONS, form);
    showModal.value = false; fetchData();
  } catch (e) { alert("Erreur d'intégrité UML."); }
};

const handleDelete = async (id) => {
  if (confirm("Supprimer définitivement cet actif du référentiel ?")) {
    await axios.delete(`${API_QUESTIONS}/${id}`);
    fetchData();
  }
};

onMounted(fetchData);
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.admin-layout { background: #fcfcfd; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }
.brand-title-main { font-weight: 800; font-size: 32px; color: #1e293b; }
.brand-title-main span { color: #f59e0b; }
.brand-subtitle { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px; text-transform: uppercase; }

/* HEADER & SEARCH */
.search-container-pro { position: relative; width: 350px; }
.search-container-pro i { position: absolute; left: 18px; top: 50%; transform: translateY(-50%); color: #cbd5e1; }
.search-container-pro input { width: 100%; border: 1.5px solid #e2e8f0; border-radius: 16px; padding: 12px 12px 12px 50px; font-weight: 600; outline: none; transition: 0.3s; background: white; }
.search-container-pro input:focus { border-color: #f59e0b; box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.08); }

.btn-primary-pro { background: #f59e0b; color: white; border: none; padding: 12px 28px; border-radius: 16px; font-weight: 800; transition: 0.3s; }
.btn-primary-pro:hover:not(:disabled) { background: #d97706; transform: translateY(-3px); box-shadow: 0 10px 20px rgba(217, 119, 6, 0.2); }

/* MINI STATS */
.mini-stat-card { background: white; border-radius: 24px; padding: 22px; display: flex; align-items: center; border: 1px solid #f1f5f9; }
.mini-stat-card .icon-box { width: 50px; height: 50px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px; margin-right: 15px; }
.mini-stat-card .val { font-size: 24px; font-weight: 900; color: #1e293b; line-height: 1; }
.mini-stat-card .lbl { font-size: 11px; font-weight: 800; color: #94a3b8; margin-top: 5px; text-transform: uppercase; }

/* FILTERS */
.filter-strip { display: flex; align-items: center; gap: 20px; }
.filter-label { font-size: 13px; font-weight: 800; color: #475569; }
.chips-group { display: flex; gap: 10px; }
.chip-item { border: none; background: white; border: 1px solid #e2e8f0; padding: 8px 20px; border-radius: 12px; font-size: 12px; font-weight: 800; color: #64748b; transition: 0.3s; }
.chip-item.active { background: #0f172a; color: white; border-color: #0f172a; }

/* ASSET CARD PREMIUM */
.asset-card-premium { background: white; border-radius: 32px; padding: 30px; border: 1px solid #f1f5f9; transition: 0.4s; height: 100%; display: flex; flex-direction: column; }
.asset-card-premium:hover { border-color: #f59e0b; transform: translateY(-10px); box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.05); }
.category-tag { font-size: 10px; font-weight: 900; color: #f59e0b; background: #fff9e6; padding: 6px 14px; border-radius: 10px; text-transform: uppercase; }
.question-text-pro { font-size: 18px; font-weight: 800; color: #1e293b; line-height: 1.5; margin-top: 20px; }

.difficulty-indicator .difficulty-val { font-size: 11px; font-weight: 900; }
.custom-progress { height: 7px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: 1.5s cubic-bezier(0.4, 0, 0.2, 1); }

.card-footer-pro { display: flex; justify-content: space-between; align-items: center; }
.meta-tag { font-size: 12px; font-weight: 800; color: #64748b; }
.parent-link { font-size: 12px; font-weight: 800; color: #f59e0b; }

.btn-action-round { width: 38px; height: 38px; border-radius: 12px; border: none; background: #f8fafc; color: #cbd5e1; transition: 0.3s; margin-left: 8px; }
.btn-action-round:hover { background: #0f172a; color: #f59e0b; }
.btn-action-round.del:hover { background: #ef4444; color: white; }

/* MODAL PRO */
.modal-pro-overlay { position: fixed; inset: 0; background: rgba(15, 23, 42, 0.4); backdrop-filter: blur(10px); z-index: 5000; display: flex; align-items: center; justify-content: center; padding: 20px; }
.modal-pro-card { background: white; width: 100%; max-width: 750px; border-radius: 35px; overflow: hidden; box-shadow: 0 30px 60px rgba(0,0,0,0.1); }
.modal-pro-header { padding: 30px 40px; border-bottom: 1px solid #f1f5f9; display: flex; justify-content: space-between; align-items: center; }
.pro-label { font-size: 11px; font-weight: 900; color: #94a3b8; text-transform: uppercase; margin-bottom: 8px; display: block; letter-spacing: 0.5px; }
.pro-input { width: 100%; background: #f8fafc; border: 1.5px solid #e2e8f0; padding: 14px 20px; border-radius: 16px; font-weight: 600; outline: none; transition: 0.3s; }
.pro-input:focus { border-color: #f59e0b; background: white; }
.btn-cancel-pro { background: transparent; border: none; font-weight: 800; color: #94a3b8; }
.close-btn { background: none; border: none; font-size: 28px; color: #cbd5e1; }

.spinner-pro { width: 40px; height: 40px; border: 4px solid #f1f5f9; border-top-color: #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; margin: auto; }
@keyframes spin { to { transform: rotate(360deg); } }
</style>