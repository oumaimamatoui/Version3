<template>
  <div class="d-flex admin-layout">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <AppSidebar />

    <div class="content-right flex-grow-1">
      <AppNavbar />

      <main class="p-4 pt-2">
        <div class="ai-generator-container animate-fade-in">
          
          <!-- HEADER DYNAMIQUE -->
          <div class="page-header mb-4 d-flex justify-content-between align-items-end">
            <div>
              <span class="badge-ai-powered">
                <i class="fa-solid fa-sparkles me-1"></i> AI Powered
              </span>
              <h2 class="fw-800 text-navy mt-2">AI Question Generator</h2>
              <p class="text-muted small mb-0">Transformez vos documents techniques en évaluations intelligentes.</p>
            </div>
            <div class="header-actions">
               <button class="btn-outline-tech-sm"><i class="fa-solid fa-history me-2"></i>Historique</button>
            </div>
          </div>

          <div class="row g-4">
            <!-- PANNEAU GAUCHE : CONFIGURATION -->
            <div class="col-lg-5">
              
              <!-- 1. Source Documents -->
              <div class="glass-card p-4 mb-4 shadow-pro border-0">
                <div class="section-title-pro mb-3">
                  <div class="icon-circle bg-navy text-white"><i class="fa-solid fa-file-arrow-up"></i></div>
                  <h6 class="fw-bold m-0 text-navy">Source Documents</h6>
                </div>

                <!-- ZONE DE DROP CLIQUABLE -->
                <div class="upload-zone-premium text-center p-4 mb-3" @click="triggerUpload">
                  <i class="fa-solid fa-cloud-arrow-up main-cloud mb-2"></i>
                  <p class="fw-800 mb-0 small">Déposer vos fichiers ici</p>
                  <p class="text-muted tiny">PDF, DOCX (Max 10MB)</p>
                  <!-- INPUT CACHÉ -->
                  <input 
                    type="file" 
                    ref="fileRef" 
                    hidden 
                    multiple 
                    accept=".pdf,.docx,.doc" 
                    @change="handleFile"
                  >
                </div>

                <div class="file-stack">
                  <TransitionGroup name="list">
                    <div v-for="(file, index) in files" :key="file.name" class="file-pill-pro">
                      <i :class="getFileIcon(file.name)" class="me-2"></i>
                      <span class="file-name-text text-truncate" style="max-width: 200px;">{{ file.name }}</span>
                      <button class="btn-remove-sm" @click="removeFile(index)">&times;</button>
                    </div>
                  </TransitionGroup>
                </div>
              </div>

              <!-- 2. Generation Settings -->
              <div class="glass-card p-4 shadow-pro border-0">
                <div class="section-title-pro mb-4">
                  <div class="icon-circle bg-amber text-white"><i class="fa-solid fa-sliders"></i></div>
                  <h6 class="fw-bold m-0 text-navy">Generation Settings</h6>
                </div>

                <!-- Number of Questions -->
                <div class="setting-block mb-4">
                  <div class="d-flex justify-content-between mb-2">
                    <label class="form-label-pro">Nombre de questions</label>
                    <span class="badge-value">{{ settings.count }}</span>
                  </div>
                  <input type="range" v-model="settings.count" min="5" max="50" class="custom-range-yellow">
                </div>

                <!-- Theme / Category -->
                <div class="setting-block mb-4">
                  <label class="form-label-pro">Thématique / Catégorie</label>
                  <div class="select-wrapper">
                    <select v-model="settings.theme" class="form-select-premium">
                      <option>Auto-detect from content</option>
                      <option>Programming</option>
                      <option>Soft Skills</option>
                    </select>
                  </div>
                </div>

                <!-- Difficulty -->
                <div class="setting-block mb-4">
                  <label class="form-label-pro">Distribution de la difficulté</label>
                  <div class="difficulty-pills">
                    <button v-for="d in ['Easy', 'Medium', 'Hard']" 
                            :key="d" 
                            @click="settings.difficulty = d"
                            :class="['pill-btn', { active: settings.difficulty === d }]">
                      {{ d }}
                    </button>
                  </div>
                </div>

                <!-- Question Types -->
                <div class="setting-block mb-4">
                  <label class="form-label-pro">Types de questions</label>
                  <div class="type-grid">
                    <div v-for="t in qTypes" :key="t.id" 
                         :class="['type-card-mini', { active: settings.type === t.id }]" 
                         @click="settings.type = t.id">
                      <i :class="t.icon"></i>
                      <span>{{ t.label }}</span>
                    </div>
                  </div>
                </div>

                <button class="btn-generate-premium w-100" @click="startGeneration" :disabled="isGenerating || files.length === 0">
                  <span v-if="!isGenerating">
                    <i class="fa-solid fa-wand-magic-sparkles me-2"></i>Générer par IA
                  </span>
                  <span v-else>
                    <i class="fa-solid fa-circle-notch fa-spin me-2"></i>Analyse en cours...
                  </span>
                </button>
              </div>
            </div>

            <!-- PANNEAU DROIT : QUESTIONS GÉNÉRÉES -->
            <div class="col-lg-7">
              <div class="glass-card p-4 h-100 shadow-pro border-0">
                <div class="d-flex justify-content-between align-items-center mb-4">
                  <h6 class="fw-bold m-0 text-navy"><i class="fa-solid fa-list-check me-2"></i>Questions Générées</h6>
                  <div class="d-flex gap-2">
                    <button class="btn-outline-tech-sm" :disabled="mockQuestions.length === 0">Tout sélectionner</button>
                    <button class="btn-navy-sm" :disabled="mockQuestions.length === 0">Enregistrer</button>
                  </div>
                </div>

                <div class="generated-list-container custom-scrollbar">
                  <div v-if="isGenerating" class="skeleton-loader p-5 text-center">
                    <div class="spinner-border text-warning mb-3"></div>
                    <p class="text-muted italic">L'IA parcourt vos documents...</p>
                  </div>

                  <TransitionGroup name="list" v-else>
                    <div v-for="q in mockQuestions" :key="q.id" class="q-card-premium mb-3">
                      <div class="d-flex justify-content-between">
                        <div class="d-flex gap-2">
                          <span :class="['badge-diff-v2', q.difficulty.toLowerCase()]">{{ q.difficulty }}</span>
                          <span class="badge-type-v2">{{ q.type }}</span>
                        </div>
                        <div class="q-actions-mini">
                          <button @click="openEditModal(q)"><i class="fa-solid fa-pen"></i></button>
                          <button @click="deleteQuestion(q.id)"><i class="fa-solid fa-trash"></i></button>
                        </div>
                      </div>
                      
                      <h6 class="q-text-v2 fw-bold my-3 text-navy">{{ q.text }}</h6>
                      
                      <div class="options-stack">
                        <div v-for="(opt, idx) in q.options" :key="idx" 
                             :class="['opt-item-v2', { 'correct': q.correctIndex === idx }]">
                          <div class="opt-letter">{{ String.fromCharCode(65 + idx) }}</div>
                          <div class="opt-label">{{ opt }}</div>
                          <i v-if="q.correctIndex === idx" class="fa-solid fa-check-circle ms-auto"></i>
                        </div>
                      </div>
                    </div>
                  </TransitionGroup>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

// RÉFÉRENCES
const isGenerating = ref(false);
const fileRef = ref(null);
const files = ref([]); // Liste vide par défaut pour l'utilisateur

const settings = reactive({ 
  count: 12, 
  theme: 'Auto-detect from content', 
  difficulty: 'Medium', 
  type: 'mcq' 
});

const qTypes = [
  { id: 'mcq', label: 'Multi Choice', icon: 'fa-solid fa-layer-group' },
  { id: 'scq', label: 'Single Choice', icon: 'fa-solid fa-circle-dot' },
  { id: 'tf', label: 'True / False', icon: 'fa-solid fa-equals' }
];

const mockQuestions = ref([
  {
    id: 1, difficulty: 'Hard', type: 'MCQ',
    text: 'Expliquez le concept de clôture (closure) en JavaScript et son impact sur la mémoire.',
    options: ['Une fonction sans portée', 'Une fonction liée à son environnement lexical', 'Une variable globale', 'Une erreur de syntaxe'],
    correctIndex: 1
  }
]);

// LOGIQUE D'UPLOAD
const triggerUpload = () => {
  fileRef.value.click(); // Ouvre la boîte de dialogue du fichier
};

const handleFile = (event) => {
  const selectedFiles = Array.from(event.target.files);
  
  selectedFiles.forEach(file => {
    // Vérification basique (extension et taille 10Mo)
    const isValidSize = file.size <= 10 * 1024 * 1024;
    const isAlreadyAdded = files.value.some(f => f.name === file.name);

    if (isValidSize && !isAlreadyAdded) {
      files.value.push({
        name: file.name,
        size: file.size,
        rawFile: file // On garde le fichier brut si besoin de l'envoyer au serveur
      });
    } else if (!isValidSize) {
      alert(`Le fichier ${file.name} est trop lourd (> 10MB)`);
    }
  });

  // Reset de l'input pour permettre de uploader le même fichier si supprimé
  event.target.value = '';
};

const removeFile = (i) => {
  files.value.splice(i, 1);
};

// HELPERS
const getFileIcon = (n) => {
  if (n.endsWith('.pdf')) return 'fa-solid fa-file-pdf text-danger';
  if (n.endsWith('.docx') || n.endsWith('.doc')) return 'fa-solid fa-file-word text-primary';
  return 'fa-solid fa-file text-muted';
};

const startGeneration = () => {
  if (files.value.length === 0) return;
  isGenerating.value = true;
  setTimeout(() => { isGenerating.value = false; }, 3000);
};

const deleteQuestion = (id) => { 
  mockQuestions.value = mockQuestions.value.filter(q => q.id !== id); 
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

.admin-layout { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; }
.text-navy { color: #0f172a; }
.bg-navy { background-color: #0f172a; }
.bg-amber { background-color: #facc15; }

/* --- GLASS SURFACES --- */
.glass-card { background: white; border-radius: 24px; border: 1px solid #f1f5f9; }
.shadow-pro { box-shadow: 0 10px 30px -5px rgba(0,0,0,0.03); }

/* --- UPLOAD ZONE --- */
.upload-zone-premium {
  border: 2px dashed #cbd5e1; border-radius: 20px; cursor: pointer; transition: 0.3s;
  background: #fcfdfe;
}
.upload-zone-premium:hover { border-color: #facc15; background: #fffbeb; }
.main-cloud { font-size: 32px; color: #94a3b8; }

.file-pill-pro {
  background: #f8fafc; padding: 10px 15px; border-radius: 12px;
  display: flex; align-items: center; margin-bottom: 8px; font-size: 13px; font-weight: 600;
}
.btn-remove-sm { border: none; background: none; font-size: 18px; color: #94a3b8; margin-left: auto; }

/* --- SETTINGS STYLING --- */
.icon-circle { width: 30px; height: 30px; border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 14px; display: inline-flex; vertical-align: middle; margin-right: 10px; }

.custom-range-yellow { -webkit-appearance: none; width: 100%; height: 6px; background: #f1f5f9; border-radius: 5px; outline: none; }
.custom-range-yellow::-webkit-slider-thumb { -webkit-appearance: none; width: 18px; height: 18px; background: #facc15; border-radius: 50%; cursor: pointer; border: 3px solid #fff; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }

.form-select-premium { width: 100%; padding: 12px; border: 1.5px solid #e2e8f0; border-radius: 14px; font-size: 14px; outline: none; transition: 0.2s; }
.form-select-premium:focus { border-color: #facc15; }

.difficulty-pills { display: flex; gap: 8px; background: #f1f5f9; padding: 5px; border-radius: 14px; }
.pill-btn { flex: 1; border: none; background: none; padding: 8px; border-radius: 10px; font-size: 12px; font-weight: 700; color: #64748b; transition: 0.3s; }
.pill-btn.active { background: white; color: #0f172a; shadow: 0 4px 10px rgba(0,0,0,0.05); }

.type-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; }
.type-card-mini { padding: 15px 5px; border: 1.5px solid #f1f5f9; border-radius: 16px; text-align: center; cursor: pointer; transition: 0.2s; }
.type-card-mini i { display: block; margin-bottom: 8px; font-size: 18px; color: #94a3b8; }
.type-card-mini span { font-size: 10px; font-weight: 800; color: #64748b; text-transform: uppercase; }
.type-card-mini.active { border-color: #facc15; background: #fffbeb; }
.type-card-mini.active i, .type-card-mini.active span { color: #ca8a04; }

/* --- GENERATED CARDS --- */
.q-card-premium { background: #fff; border: 1.5px solid #f1f5f9; border-radius: 20px; padding: 20px; transition: 0.3s; }
.q-card-premium:hover { border-color: #facc15; transform: translateY(-2px); }

.badge-diff-v2 { font-size: 9px; font-weight: 800; padding: 4px 10px; border-radius: 8px; text-transform: uppercase; }
.badge-diff-v2.hard { background: #fef2f2; color: #ef4444; }
.badge-diff-v2.medium { background: #fffbeb; color: #d97706; }
.badge-type-v2 { font-size: 9px; font-weight: 800; background: #f1f5f9; color: #64748b; padding: 4px 10px; border-radius: 8px; }

.opt-item-v2 { display: flex; align-items: center; gap: 12px; padding: 12px 15px; border-radius: 14px; background: #f8fafc; margin-bottom: 8px; font-size: 14px; border: 1px solid transparent; }
.opt-item-v2.correct { background: #ecfdf5; border-color: #10b981; color: #065f46; font-weight: 600; }
.opt-letter { width: 24px; height: 24px; border-radius: 50%; border: 1.5px solid #cbd5e1; display: flex; align-items: center; justify-content: center; font-size: 11px; font-weight: 700; color: #64748b; }
.correct .opt-letter { border-color: #10b981; color: #10b981; }

/* --- BUTTONS --- */
.btn-generate-premium { background: #facc15; color: white; border: none; padding: 15px; border-radius: 16px; font-weight: 800; font-size: 15px; transition: 0.3s; box-shadow: 0 10px 20px -5px rgba(250, 204, 21, 0.4); }
.btn-generate-premium:hover:not(:disabled) { background: #eab308; transform: scale(1.02); }
.btn-navy-sm { background: #0f172a; color: white; border: none; padding: 6px 15px; border-radius: 10px; font-size: 12px; font-weight: 700; }
.btn-outline-tech-sm { background: white; border: 1.5px solid #e2e8f0; color: #64748b; padding: 6px 15px; border-radius: 10px; font-size: 12px; font-weight: 700; }

.q-actions-mini button { border: none; background: none; color: #cbd5e1; font-size: 13px; margin-left: 10px; transition: 0.2s; }
.q-actions-mini button:hover { color: #0f172a; }

.custom-scrollbar { max-height: 70vh; overflow-y: auto; padding-right: 10px; }
.custom-scrollbar::-webkit-scrollbar { width: 4px; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }

.animate-fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
</style>