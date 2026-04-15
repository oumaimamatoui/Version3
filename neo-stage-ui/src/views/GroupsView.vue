<template>
  <div class="d-flex admin-layout">
    <AppSidebar />
    
    <div class="main-content flex-grow-1">
      <AppNavbar />

      <main class="p-4 p-lg-5 animate-in">
        
        <!-- SECTION : ENTÊTE PROFESSIONNELLE -->
        <header class="d-flex justify-content-between align-items-end mb-5">
          <div>
            <h2 class="brand-title">Gestion des <span>Invitations</span></h2>
            <p class="brand-subtitle">DÉPLOYEZ VOS ÉVALUATIONS AUX GROUPES DE TALENTS</p>
          </div>
          <div class="stats-mini d-none d-md-flex">
            <div class="stat-item">
              <span class="val">{{ emailList.length }}</span>
              <span class="lab">En attente</span>
            </div>
          </div>
        </header>

        <!-- NOTIFICATION TOAST -->
        <transition name="slide-fade">
          <div v-if="statusMsg" :class="['custom-toast', statusType]">
            <i class="fa-solid" :class="statusType === 'success' ? 'fa-circle-check' : 'fa-circle-exclamation'"></i>
            <span>{{ statusMsg }}</span>
          </div>
        </transition>

        <div class="row g-4">
          <!-- COLONNE PRINCIPALE (FORMULAIRE) -->
          <div class="col-lg-8">
            
            <!-- ÉTAPE 1 : SÉLECTION DU GROUPE -->
            <div class="glass-card mb-4">
              <div class="card-step-num">01</div>
              <div class="card-content-wrap">
                <h6 class="section-title">Assignation à une Campagne</h6>
                <p class="section-desc">Choisissez le flux d'évaluation auquel ces candidats seront rattachés.</p>
                
                <div class="d-flex gap-3 mt-3">
                  <div class="flex-grow-1 position-relative">
                    <i class="fa-solid fa-folder-tree icon-overlay"></i>
                    <select v-model="selectedCampagneId" class="modern-select ps-5">
                      <option value="">— Sélectionner une campagne active —</option>
                      <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom || c.nom }}</option>
                    </select>
                  </div>
                  <button class="btn-outline-gold" @click="router.push('/campaigns')">
                    <i class="fa-solid fa-plus"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- ÉTAPE 2 : MÉTHODE D'IMPORTATION -->
            <div class="glass-card">
              <div class="card-step-num">02</div>
              <div class="card-content-wrap w-100">
                <h6 class="section-title">Méthode d'importation</h6>
                
                <div class="modern-tabs mb-4">
                   <button @click="activeTab = 'unique'" :class="{ active: activeTab === 'unique' }">E-mail Unique</button>
                   <button @click="activeTab = 'multiple'" :class="{ active: activeTab === 'multiple' }">Bulk Import</button>
                   <button @click="activeTab = 'csv'" :class="{ active: activeTab === 'csv' }">Fichier CSV</button>
                </div>

                <div class="tab-body p-2">
                    <!-- UNIQUE -->
                    <div v-if="activeTab === 'unique'" class="fade-in">
                      <div class="d-flex gap-2">
                        <input type="email" v-model="currentEmail" @keyup.enter="addEmail" placeholder="talent@exemple.com" class="modern-input">
                        <button @click="addEmail" class="btn-primary-dark">Ajouter</button>
                      </div>
                    </div>

                    <!-- BULK -->
                    <div v-if="activeTab === 'multiple'" class="fade-in">
                      <textarea v-model="bulkEmails" class="modern-input" rows="4" placeholder="Séparez les emails par des virgules ou des lignes..."></textarea>
                      <button @click="processBulkEmails" class="btn-primary-dark mt-2 w-100">Analyser & Injecter</button>
                    </div>

                    <!-- CSV -->
                    <div v-if="activeTab === 'csv'" class="fade-in text-center">
                      <div class="csv-dropzone" @click="$refs.fileInput.click()">
                        <i class="fa-solid fa-cloud-arrow-up fa-2x mb-2 text-warning"></i>
                        <p class="m-0 fw-bold">Cliquez pour importer votre CSV</p>
                        <span class="text-muted small">Format: email@test.com</span>
                        <input type="file" class="d-none" ref="fileInput" accept=".csv" @change="handleFileUpload">
                      </div>
                    </div>
                </div>

                <!-- PREVIEW LIST (PILLS) -->
                <div v-if="emailList.length > 0" class="mt-5 border-top pt-4">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <span class="fw-800 small text-muted uppercase">Candidats identifiés ({{ emailList.length }})</span>
                    <button @click="emailList = []" class="btn-text-danger">Effacer tout</button>
                  </div>
                  <div class="pill-container">
                    <span v-for="(mail, idx) in emailList" :key="idx" class="modern-pill">
                      {{ mail }} <i @click="removeEmail(idx)" class="fa-solid fa-xmark ms-2"></i>
                    </span>
                  </div>
                </div>
              </div>
            </div>

            <!-- BOUTON FINAL -->
            <div class="text-end mt-5">
              <button @click="deployInvitations" :disabled="isLoading || !selectedCampagneId || emailList.length === 0" class="btn-launch shadow-lg">
                <span v-if="isLoading"><i class="fa-solid fa-spinner fa-spin me-2"></i> TRAITEMENT...</span>
                <span v-else><i class="fa-solid fa-paper-plane me-2"></i> EXPÉDIER LES INVITATIONS</span>
              </button>
            </div>
          </div>

          <!-- SIDEBAR INFO (GUIDELINES) -->
          <div class="col-lg-4">
            <div class="info-panel p-4">
              <h6 class="fw-800 mb-4"><i class="fa-solid fa-circle-info me-2 text-warning"></i> Guidelines</h6>
              <div class="guide-item mb-4">
                <div class="guide-icon"><i class="fa-solid fa-shield-halved"></i></div>
                <p>Les invitations sont sécurisées par un token unique valide pour une seule session.</p>
              </div>
              <div class="guide-item mb-4">
                <div class="guide-icon"><i class="fa-solid fa-envelope-open-text"></i></div>
                <p>Un e-mail de bienvenue avec les consignes sera envoyé automatiquement.</p>
              </div>
              <div class="guide-item">
                <div class="guide-icon"><i class="fa-solid fa-chart-pie"></i></div>
                <p>Vous pourrez suivre le taux de complétion dans l'onglet "Campagnes".</p>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import { useRouter } from 'vue-router';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const router = useRouter();
const API_URL = '/Invitations';

const activeTab = ref('unique');
const campagnes = ref([]);
const selectedCampagneId = ref('');
const emailList = ref([]);
const currentEmail = ref('');
const bulkEmails = ref('');
const isLoading = ref(false);
const statusMsg = ref('');
const statusType = ref('success');
const fileInput = ref(null);

onMounted(async () => {
  try {
    const res = await api.get(`${API_URL}/campagnes`);
    campagnes.value = res.data;
  } catch (e) { console.error(e); }
});

const addEmail = () => {
  const mail = currentEmail.value.trim().toLowerCase();
  if (mail && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(mail)) {
    if (!emailList.value.includes(mail)) emailList.value.push(mail);
    currentEmail.value = '';
    statusMsg.value = '';
  } else {
    showStatus("Format d'email invalide.", "error");
  }
};

const processBulkEmails = () => {
  const extracted = bulkEmails.value.split(/[\s,\n,;]+/).map(e => e.trim().toLowerCase());
  let added = 0;
  extracted.forEach(e => {
    if (e && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(e) && !emailList.value.includes(e)) {
      emailList.value.push(e);
      added++;
    }
  });
  bulkEmails.value = '';
  if (added > 0) showStatus(`${added} emails ajoutés.`, "success");
};

const handleFileUpload = (event) => {
  const file = event.target.files[0];
  if (!file) return;
  const reader = new FileReader();
  reader.onload = (e) => {
    const found = e.target.result.match(/[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}/g);
    if (found) {
      found.forEach(m => {
        if (!emailList.value.includes(m.toLowerCase())) emailList.value.push(m.toLowerCase());
      });
      showStatus("Importation CSV réussie.", "success");
    }
  };
  reader.readAsText(file);
};

const removeEmail = (index) => emailList.value.splice(index, 1);

const showStatus = (msg, type) => {
  statusMsg.value = msg;
  statusType.value = type;
  setTimeout(() => statusMsg.value = '', 4000);
};

const deployInvitations = async () => {
  isLoading.value = true;
  try {
    await api.post(`${API_URL}/invite-candidates`, {
      campagneId: selectedCampagneId.value,
      emails: emailList.value
    });
    showStatus("Les invitations ont été expédiées !", "success");
    emailList.value = [];
    selectedCampagneId.value = '';
  } catch (error) {
    showStatus("Une erreur est survenue.", "error");
  } finally { isLoading.value = false; }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;800&display=swap');

.admin-layout { background: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }

/* HEADER */
.brand-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; color: #0f172a; }
.brand-title span { color: #f59e0b; }
.brand-subtitle { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }

.stat-item { background: white; padding: 10px 20px; border-radius: 15px; border: 1px solid #f1f5f9; text-align: center; }
.stat-item .val { display: block; font-size: 20px; font-weight: 800; color: #f59e0b; }
.stat-item .lab { font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; }

/* CARDS */
.glass-card {
  background: white; border-radius: 25px; padding: 30px; 
  display: flex; gap: 20px; border: 1px solid #f1f5f9;
  position: relative; transition: 0.3s;
}
.glass-card:hover { transform: translateY(-5px); box-shadow: 0 20px 40px rgba(0,0,0,0.04); }
.card-step-num {
  width: 45px; height: 45px; background: #0f172a; color: #f59e0b;
  border-radius: 12px; display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 18px; flex-shrink: 0;
}
.section-title { font-weight: 800; font-size: 18px; color: #1e293b; margin: 0; }
.section-desc { font-size: 13px; color: #64748b; margin-top: 5px; }

/* INPUTS & SELECTS */
.modern-select, .modern-input {
  width: 100%; padding: 15px; border-radius: 15px; border: 2.5px solid #f1f5f9;
  background: #f8fafc; font-weight: 700; outline: none; transition: 0.3s;
}
.modern-select:focus, .modern-input:focus { border-color: #f59e0b; background: white; }
.icon-overlay { position: absolute; left: 18px; top: 50%; transform: translateY(-50%); color: #f59e0b; }

/* TABS */
.modern-tabs { display: flex; background: #f1f5f9; border-radius: 12px; padding: 5px; }
.modern-tabs button {
  flex: 1; border: none; padding: 10px; border-radius: 8px;
  font-weight: 800; font-size: 12px; color: #94a3b8; background: transparent; transition: 0.3s;
}
.modern-tabs button.active { background: white; color: #0f172a; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }

/* CSV ZONE */
.csv-dropzone {
  border: 2px dashed #e2e8f0; padding: 30px; border-radius: 20px;
  background: #fafafa; cursor: pointer; transition: 0.3s;
}
.csv-dropzone:hover { border-color: #f59e0b; background: #fffcf5; }

/* PILLS */
.pill-container { display: flex; flex-wrap: wrap; gap: 8px; }
.modern-pill {
  background: #0f172a; color: white; padding: 6px 15px; border-radius: 50px;
  font-size: 12px; font-weight: 700; display: flex; align-items: center;
}
.modern-pill i { cursor: pointer; color: #f59e0b; }

/* BUTTONS */
.btn-primary-dark { background: #0f172a; color: white; border: none; border-radius: 12px; font-weight: 800; padding: 10px 25px; }
.btn-launch {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  color: white; border: none; padding: 18px 40px; border-radius: 20px;
  font-weight: 800; font-size: 16px; transition: 0.4s;
}
.btn-launch:hover:not(:disabled) { background: #f59e0b; color: #0f172a; transform: scale(1.05); }
.btn-outline-gold { border: 2px solid #f1f5f9; background: white; color: #f59e0b; padding: 0 15px; border-radius: 15px; font-size: 20px; }

/* INFO PANEL */
.info-panel { background: #0f172a; color: white; border-radius: 30px; min-height: 400px; }
.guide-item { display: flex; gap: 15px; align-items: flex-start; }
.guide-icon { color: #f59e0b; font-size: 20px; }
.guide-item p { font-size: 13px; color: #94a3b8; line-height: 1.6; }

/* TOAST */
.custom-toast {
  position: fixed; top: 30px; right: 30px; padding: 15px 30px; border-radius: 15px;
  z-index: 9999; color: white; font-weight: 800; display: flex; align-items: center; gap: 15px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.1);
}
.custom-toast.success { background: #10b981; }
.custom-toast.error { background: #ef4444; }

.animate-in { animation: fadeInUp 0.8s ease-out; }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
</style>