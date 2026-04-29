<template>
  <div class="d-flex admin-layout">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <AppSidebar />

    <div class="content-right flex-grow-1">
      <AppNavbar />

      <main class="p-4 pt-2">
        <div class="ai-generator-container animate-fade-in">
          
          <!-- HEADER -->
          <div class="page-header mb-4 d-flex justify-content-between align-items-end">
            <div>
              <span class="badge-ai-powered">
                <i class="fa-solid fa-sparkles me-1"></i> AI Powered 2.0
              </span>
              <h2 class="fw-800 text-navy mt-2">AI Pro Generator</h2>
              <p class="text-muted small mb-0">Analysez vos CV ou fiches de poste pour générer des tests sur-mesure.</p>
            </div>
          </div>

          <div class="row g-4">
            <!-- CONFIGURATION (GAUCHE) -->
            <div class="col-lg-5">
              
              <!-- 1. Source Documents -->
              <div class="glass-card p-4 mb-4 shadow-pro border-0">
                <div class="section-title-pro mb-3">
                  <div class="icon-circle bg-navy text-white"><i class="fa-solid fa-file-arrow-up"></i></div>
                  <h6 class="fw-bold m-0 text-navy">Document de Contexte</h6>
                </div>

                <div class="upload-zone-premium text-center p-4 mb-3" @click="triggerUpload">
                  <i class="fa-solid fa-cloud-arrow-up main-cloud mb-2"></i>
                  <p class="fw-800 mb-0 small">Cliquez pour uploader</p>
                  <p class="text-muted tiny">CV ou Fiche de poste (PDF, DOCX)</p>
                  <input type="file" ref="fileRef" hidden @change="handleFile" accept=".pdf,.docx">
                </div>

                <div class="file-stack" v-if="files.length">
                  <div v-for="(file, index) in files" :key="index" class="file-pill-pro">
                    <i :class="getFileIcon(file.name)" class="me-2"></i>
                    <span class="file-name-text text-truncate">{{ file.name }}</span>
                    <button class="btn-remove-sm" @click="removeFile">&times;</button>
                  </div>
                </div>
              </div>

              <!-- 2. Generation Settings -->
              <div class="glass-card p-4 shadow-pro border-0">
                <div class="section-title-pro mb-4">
                  <div class="icon-circle bg-amber text-white"><i class="fa-solid fa-sliders"></i></div>
                  <h6 class="fw-bold m-0 text-navy">Paramètres IA</h6>
                </div>

                <div class="setting-block mb-4">
                  <div class="d-flex justify-content-between mb-2">
                    <label class="form-label-pro">Nombre de questions</label>
                    <span class="badge-value">{{ settings.count }}</span>
                  </div>
                  <input type="range" v-model="settings.count" min="5" max="20" class="custom-range-yellow">
                </div>

                <div class="setting-block mb-4">
                  <label class="form-label-pro">Expertise ciblée</label>
                  <input v-model="settings.theme" class="form-control-premium" placeholder="Ex: Développeur Python, Manager...">
                </div>

                <div class="setting-block mb-4">
                  <label class="form-label-pro">Difficulté</label>
                  <div class="difficulty-pills">
                    <button v-for="d in ['Easy', 'Medium', 'Hard']" :key="d" 
                            @click="settings.difficulty = d"
                            :class="['pill-btn', { active: settings.difficulty === d }]">
                      {{ d }}
                    </button>
                  </div>
                </div>

                <button class="btn-generate-premium w-100" @click="startGeneration" :disabled="isGenerating">
                  <span v-if="!isGenerating">
                    <i class="fa-solid fa-wand-magic-sparkles me-2"></i>GÉNÉRER AVEC GEMINI
                  </span>
                  <span v-else>
                    <i class="fa-solid fa-circle-notch fa-spin me-2"></i>ANALYSE EN COURS...
                  </span>
                </button>
              </div>
            </div>

            <!-- QUESTIONS GÉNÉRÉES (DROITE) -->
            <div class="col-lg-7">
              <div class="glass-card p-4 h-100 shadow-pro border-0">
                <div class="d-flex justify-content-between align-items-center mb-4">
                  <h6 class="fw-bold m-0 text-navy"><i class="fa-solid fa-list-check me-2"></i>Questions Extraites</h6>
                  <button class="btn-navy-sm" @click="saveAllQuestions" :disabled="generatedQuestions.length === 0 || isSaving">
                    <i v-if="isSaving" class="fa-solid fa-circle-notch fa-spin me-1"></i>
                    {{ isSaving ? 'Sauvegarde...' : 'Tout Enregistrer' }}
                  </button>
                </div>

                <div class="generated-list-container custom-scrollbar">
                  <div v-if="generatedQuestions.length === 0 && !isGenerating" class="text-center p-5">
                    <i class="fa-solid fa-robot fa-3x text-light mb-3"></i>
                    <p class="text-muted">Aucune question générée pour le moment.</p>
                  </div>

                  <div v-for="(q, idx) in generatedQuestions" :key="idx" class="q-card-premium mb-3">
                    <div class="d-flex justify-content-between">
                       <span class="badge-type-v2">IA GENERATED</span>
                       <button class="btn-remove-mini" @click="generatedQuestions.splice(idx, 1)">&times;</button>
                    </div>
                    <h6 class="q-text-v2 fw-bold my-3 text-navy">{{ q.question }}</h6>
                    <div class="options-stack">
                      <div v-for="(opt, oIdx) in q.options" :key="oIdx" 
                           :class="['opt-item-v2', { 'correct': q.answer === oIdx }]">
                        <div class="opt-letter">{{ String.fromCharCode(65 + oIdx) }}</div>
                        <div class="opt-label">{{ opt }}</div>
                        <i v-if="q.answer === oIdx" class="fa-solid fa-check-circle ms-auto"></i>
                      </div>
                    </div>
                  </div>
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
import axios from 'axios';
import api from '@/services/api'; // Votre service axios pointant vers le Port 3000

// --- STATE ---
const isGenerating = ref(false);
const isSaving = ref(false);
const fileRef = ref(null);
const files = ref([]);
const generatedQuestions = ref([]);

const settings = reactive({ 
  count: 10, 
  theme: '', 
  difficulty: 'Medium'
});

// --- UPLOAD LOGIC ---
const triggerUpload = () => fileRef.value.click();

const handleFile = (e) => {
  const file = e.target.files[0];
  if (file) {
    files.value = [{ name: file.name, raw: file }];
  }
};

const removeFile = () => files.value = [];
const getFileIcon = (n) => n.endsWith('.pdf') ? 'fa-solid fa-file-pdf text-danger' : 'fa-solid fa-file-word text-primary';

// --- ACTION 1 : APPEL SERVEUR PYTHON (Génération) ---
const startGeneration = async () => {
  if (!settings.theme) {
     alert("Veuillez saisir une expertise ciblée.");
     return;
  }

  isGenerating.value = true;
  generatedQuestions.value = [];

  try {
    const fd = new FormData();
    if (files.value.length > 0) {
      fd.append('file', files.value[0].raw);
    }
    fd.append('nombre', settings.count);
    fd.append('themetique', settings.theme);
    fd.append('difficulte', settings.difficulty);
    fd.append('langue', 'fr');

    const response = await axios.post('http://127.0.0.1:8000/ia/generate-pro', fd);
    
    if (response.data.status === "IA_SUCCESS") {
      generatedQuestions.value = response.data.questions;
    }
  } catch (err) {
    if (err.response && err.response.status === 429) {
      alert("⚠️ Quota dépassé. Google limite les requêtes gratuites. Attendez 1 minute.");
    } else {
      console.error("Erreur IA:", err);
      alert("Erreur lors de la génération. Vérifiez le serveur Python (Port 8000).");
    }
  } finally {
    isGenerating.value = false;
  }
};

// --- ACTION 2 : SAUVEGARDE DB (Enregistrement) ---
const saveAllQuestions = async () => {
  isSaving.value = true;
  const token = localStorage.getItem('token');
  
  try {
    for (const q of generatedQuestions.value) {
      const payload = {
        enonce: q.question,
        type: 0, // Format QCM
        points: settings.difficulty === 'Hard' ? 4 : (settings.difficulty === 'Medium' ? 2 : 1),
        theme: settings.theme.toUpperCase(),
        sousTheme: 'Recrutement Pro (IA)',
        choix: q.options,
        bonneReponse: q.options[q.answer]
      };

      // Utilisation du endpoint /Questions pour json-server
      await api.post('/Questions', payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
    }
    alert("🚀 Succès ! Toutes les questions ont été ajoutées à la banque.");
    generatedQuestions.value = [];
  } catch (err) {
    console.error("Erreur Enregistrement:", err);
    alert("Erreur lors de l'enregistrement. Vérifiez que json-server (Port 3000) est lancé.");
  } finally {
    isSaving.value = false;
  }
};
</script>

<style scoped>
/* Styles identiques à votre version pour garder le design Premium */
.badge-ai-powered { background: linear-gradient(90deg, #0f172a, #0891b2); color: white; padding: 5px 12px; border-radius: 50px; font-size: 10px; font-weight: 800; text-transform: uppercase; }
.admin-layout { min-height: 100vh; background-color: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; }
.text-navy { color: #0f172a; }
.bg-navy { background-color: #0f172a; }
.bg-amber { background-color: #facc15; }
.glass-card { background: white; border-radius: 24px; border: 1px solid #f1f5f9; }
.shadow-pro { box-shadow: 0 10px 30px -5px rgba(0,0,0,0.03); }
.upload-zone-premium { border: 2px dashed #cbd5e1; border-radius: 20px; cursor: pointer; transition: 0.3s; background: #fcfdfe; }
.upload-zone-premium:hover { border-color: #facc15; background: #fffbeb; }
.main-cloud { font-size: 32px; color: #94a3b8; }
.file-pill-pro { background: #f8fafc; padding: 10px 15px; border-radius: 12px; display: flex; align-items: center; margin-bottom: 8px; font-size: 13px; font-weight: 600; }
.btn-remove-sm { border: none; background: none; font-size: 18px; color: #94a3b8; margin-left: auto; cursor: pointer; }
.custom-range-yellow { -webkit-appearance: none; width: 100%; height: 6px; background: #f1f5f9; border-radius: 5px; outline: none; }
.custom-range-yellow::-webkit-slider-thumb { -webkit-appearance: none; width: 18px; height: 18px; background: #facc15; border-radius: 50%; cursor: pointer; border: 3px solid #fff; }
.form-control-premium { width: 100%; padding: 12px; border: 1.5px solid #e2e8f0; border-radius: 14px; font-size: 14px; outline: none; }
.difficulty-pills { display: flex; gap: 8px; background: #f1f5f9; padding: 5px; border-radius: 14px; }
.pill-btn { flex: 1; border: none; background: none; padding: 8px; border-radius: 10px; font-size: 12px; font-weight: 700; color: #64748b; transition: 0.3s; cursor: pointer; }
.pill-btn.active { background: white; color: #0f172a; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.btn-generate-premium { background: #facc15; color: white; border: none; padding: 15px; border-radius: 16px; font-weight: 800; font-size: 15px; transition: 0.3s; cursor: pointer; }
.btn-generate-premium:hover:not(:disabled) { background: #eab308; transform: scale(1.01); }
.q-card-premium { background: #fff; border: 1.5px solid #f1f5f9; border-radius: 20px; padding: 20px; transition: 0.3s; }
.opt-item-v2 { display: flex; align-items: center; gap: 12px; padding: 12px 15px; border-radius: 14px; background: #f8fafc; margin-bottom: 8px; font-size: 14px; border: 1px solid transparent; }
.opt-item-v2.correct { background: #ecfdf5; border-color: #10b981; color: #065f46; font-weight: 600; }
.opt-letter { width: 24px; height: 24px; border-radius: 50%; border: 1.5px solid #cbd5e1; display: flex; align-items: center; justify-content: center; font-size: 11px; font-weight: 700; }
.btn-navy-sm { background: #0f172a; color: white; border: none; padding: 8px 20px; border-radius: 10px; font-size: 12px; font-weight: 700; cursor: pointer; }
.custom-scrollbar { max-height: 65vh; overflow-y: auto; padding-right: 5px; }
.animate-fade-in { animation: fadeIn 0.4s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
</style>
<style scoped>
/* (نفس الستايل الخاص بك مع إضافة بعض اللمسات) */
.badge-ai-powered { background: linear-gradient(90deg, #0f172a, #0891b2); color: white; padding: 5px 12px; border-radius: 50px; font-size: 10px; font-weight: 800; text-transform: uppercase; }
.form-control-premium { width: 100%; padding: 12px; border: 1.5px solid #e2e8f0; border-radius: 14px; font-size: 14px; outline: none; }
.btn-remove-mini { border: none; background: none; color: #cbd5e1; font-size: 20px; cursor: pointer; }
.btn-remove-mini:hover { color: #ef4444; }
.generated-list-container { max-height: 65vh; overflow-y: auto; padding-right: 10px; }
/* ... بقية الـ CSS كما أرسلته أنت ... */
</style>

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


.animate-fade-in { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
</style>