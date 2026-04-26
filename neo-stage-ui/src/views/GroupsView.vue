<template>
  <div class="admin-layout d-flex">
    <AppSidebar />
    
    <div class="main-content flex-grow-1">
      <AppNavbar />

      <main class="content-wrapper p-4 p-lg-5 animate-in">
        
        <!-- HEADER : STYLE SAAS PREMIUM -->
        <header class="dashboard-header mb-5">
          <div class="row align-items-center">
            <div class="col-md-8">
              <div class="d-flex align-items-center gap-3 mb-2">
                <span class="header-badge">Administration</span>
                <div class="breadcrumb-dot"></div>
                <span class="text-slate-400 fw-medium">Recrutement</span>
              </div>
              <h1 class="page-title">Gestion des <span class="text-amber">Invitations</span></h1>
              <p class="page-subtitle">Déployez vos campagnes d'évaluation et suivez l'engagement des talents.</p>
            </div>
            <div class="col-md-4 text-md-end d-none d-md-block">
              <div class="stats-card-mini">
                <div class="stat-icon-amber">
                  <i class="fa-solid fa-users-viewfinder"></i>
                </div>
                <div class="text-start">
                  <div class="stat-val">{{ emailList.length }}</div>
                  <div class="stat-label">Candidats en attente</div>
                </div>
              </div>
            </div>
          </div>
        </header>

        <!-- NOTIFICATION TOAST AMÉLIORÉ -->
        <transition name="toast-slide">
          <div v-if="statusMsg" :class="['custom-toast-v2', statusType]">
            <div class="toast-content">
              <div class="toast-icon">
                <i class="fa-solid" :class="statusType === 'success' ? 'fa-check' : 'fa-exclamation-triangle'"></i>
              </div>
              <div class="toast-text">
                <div class="toast-title">{{ statusType === 'success' ? 'Succès' : 'Attention' }}</div>
                <div class="toast-desc">{{ statusMsg }}</div>
              </div>
            </div>
            <div class="toast-progress"></div>
          </div>
        </transition>

        <div class="row g-4">
          <!-- COLONNE PRINCIPALE -->
          <div class="col-lg-8">
            
            <!-- ÉTAPE 1 : CAMPAGNE -->
            <section class="step-card mb-4">
              <div class="step-indicator">
                <span class="step-num">01</span>
                <div class="step-line"></div>
              </div>
              
              <div class="step-body w-100">
                <div class="d-flex justify-content-between align-items-start mb-4">
                  <div>
                    <h3 class="step-title">Assignation à une Campagne</h3>
                    <p class="text-muted small">Sélectionnez le flux d'évaluation pour ce groupe.</p>
                  </div>
                  <span class="section-tag-mini">Requis</span>
                </div>
                
                <div class="custom-select-wrapper">
                  <i class="fa-solid fa-layer-group select-icon"></i>
                  <select v-model="selectedCampagneId" class="premium-select">
                    <option value="">Sélectionner une campagne active...</option>
                    <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                  </select>
                  <button class="add-campagne-btn" @click="router.push('/campaigns')" title="Créer une campagne">
                    <i class="fa-solid fa-plus"></i>
                  </button>
                </div>
              </div>
            </section>

            <!-- ÉTAPE 2 : IMPORTATION -->
            <section class="step-card">
              <div class="step-indicator">
                <span class="step-num">02</span>
              </div>
              
              <div class="step-body w-100">
                <h3 class="step-title mb-4">Méthode d'importation des candidats</h3>
                
                <div class="premium-tabs">
                   <button @click="activeTab = 'unique'" :class="{ active: activeTab === 'unique' }">
                    <i class="fa-solid fa-user-plus me-2"></i>Unique
                   </button>
                   <button @click="activeTab = 'multiple'" :class="{ active: activeTab === 'multiple' }">
                    <i class="fa-solid fa-list-check me-2"></i>Bulk Import
                   </button>
                   <button @click="activeTab = 'csv'" :class="{ active: activeTab === 'csv' }">
                    <i class="fa-solid fa-file-csv me-2"></i>Fichier CSV
                   </button>
                </div>

                <div class="tab-content-area mt-4">
                    <!-- UNIQUE -->
                    <div v-if="activeTab === 'unique'" class="fade-in-quick">
                      <div class="input-group-premium">
                        <input type="email" v-model="currentEmail" @keyup.enter="addEmail" placeholder="nom@entreprise.com" class="premium-input">
                        <button @click="addEmail" class="btn-amber-sm">Ajouter</button>
                      </div>
                    </div>

                    <!-- BULK -->
                    <div v-if="activeTab === 'multiple'" class="fade-in-quick">
                      <textarea v-model="bulkEmails" class="premium-input" rows="4" placeholder="Collez ici vos emails (séparés par virgule, espace ou ligne)..."></textarea>
                      <button @click="processBulkEmails" class="btn-slate-sm mt-3 w-100">Analyser & Injecter</button>
                    </div>

                    <!-- CSV -->
                    <div v-if="activeTab === 'csv'" class="fade-in-quick">
                      <div class="csv-upload-zone" @click="$refs.fileInput.click()">
                        <div class="upload-icon-wrap">
                           <i class="fa-solid fa-cloud-arrow-up"></i>
                        </div>
                        <h5>Glissez votre fichier CSV ici</h5>
                        <p>Format accepté : .csv contenant une colonne d'emails</p>
                        <input type="file" class="d-none" ref="fileInput" accept=".csv" @change="handleFileUpload">
                      </div>
                    </div>
                </div>

                <!-- LISTE DE PRÉVISUALISATION -->
                <div v-if="emailList.length > 0" class="preview-section mt-5 fade-in">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <h6 class="preview-count">
                      <i class="fa-solid fa-clipboard-list text-amber me-2"></i>
                      File d'attente ({{ emailList.length }})
                    </h6>
                    <button @click="emailList = []" class="btn-clear-all">Effacer tout</button>
                  </div>
                  <div class="pills-grid">
                    <span v-for="(mail, idx) in emailList" :key="idx" class="premium-pill">
                      <span class="pill-text">{{ mail }}</span>
                      <i @click="removeEmail(idx)" class="fa-solid fa-xmark pill-close"></i>
                    </span>
                  </div>
                </div>
              </div>
            </section>

            <!-- BOUTON D'ACTION -->
            <div class="action-footer mt-5 text-end">
              <button @click="deployInvitations" :disabled="isLoading || !selectedCampagneId || emailList.length === 0" class="btn-amber-lg shadow-amber">
                <span v-if="isLoading" class="d-flex align-items-center gap-2">
                  <div class="spinner-border spinner-border-sm" role="status"></div>
                  ENVOI EN COURS...
                </span>
                <span v-else class="d-flex align-items-center gap-2">
                  EXPÉDIER LES INVITATIONS <i class="fa-solid fa-paper-plane"></i>
                </span>
              </button>
            </div>
          </div>

          <!-- SIDEBAR INFO -->
          <div class="col-lg-4">
            <aside class="sticky-info">
              <div class="info-card">
                <div class="info-header">
                  <i class="fa-solid fa-shield-halved text-amber"></i>
                  <h6>Protocole Sécurisé</h6>
                </div>
                <div class="info-body">
                  <div class="guide-item">
                    <div class="guide-dot"></div>
                    <p>Chaque candidat reçoit un <strong>lien unique</strong> crypté.</p>
                  </div>
                  <div class="guide-item">
                    <div class="guide-dot"></div>
                    <p>Le système de <strong>proctoring IA</strong> sera activé par défaut.</p>
                  </div>
                  <div class="guide-item">
                    <div class="guide-dot"></div>
                    <p>Les résultats seront agrégés dans votre tableau de bord.</p>
                  </div>
                </div>
                <div class="info-footer mt-4">
                  <div class="alert-premium">
                    <i class="fa-solid fa-circle-info"></i>
                    <span>N'oubliez pas de vérifier les dates de validité de votre campagne.</span>
                  </div>
                </div>
              </div>

              <!-- MINI TIP -->
              <div class="tip-card mt-4">
                <div class="tip-icon"><i class="fa-regular fa-lightbulb"></i></div>
                <div class="tip-content">
                  <h6>Astuce Pro</h6>
                  <p>L'import CSV est recommandé pour les listes de plus de 50 candidats.</p>
                </div>
              </div>
            </aside>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
// (Logique identique à votre script original, seul le design change)
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
  } else { showStatus("Format d'email invalide.", "error"); }
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
    showStatus("Une erreur est survenue lors de l'envoi.", "error");
  } finally { isLoading.value = false; }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* --- LAYOUT BASE --- */
.admin-layout {
  background-color: #f8fafc;
  min-height: 100vh;
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
}

/* --- HEADER --- */
.page-title { font-weight: 800; font-size: 2.5rem; letter-spacing: -1.5px; margin-bottom: 8px; }
.text-amber { color: #eab308 !important; }
.page-subtitle { color: #64748b; font-size: 1.05rem; }
.header-badge {
  background: #fefce8; color: #a16207; font-size: 11px; font-weight: 800;
  text-transform: uppercase; padding: 4px 12px; border-radius: 100px; letter-spacing: 1px;
}
.breadcrumb-dot { width: 5px; height: 5px; background: #cbd5e1; border-radius: 50%; }

/* --- MINI STATS --- */
.stats-card-mini {
  background: white; border-radius: 20px; padding: 16px 24px;
  display: inline-flex; align-items: center; gap: 16px;
  box-shadow: 0 10px 25px rgba(0,0,0,0.03); border: 1px solid #f1f5f9;
}
.stat-icon-amber {
  width: 48px; height: 48px; background: #fefce8; color: #eab308;
  border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px;
}
.stat-val { font-size: 24px; font-weight: 800; line-height: 1; }
.stat-label { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }

/* --- STEP CARDS --- */
.step-card {
  background: white; border-radius: 24px; padding: 32px;
  display: flex; gap: 24px; border: 1px solid #f1f5f9;
  transition: all 0.3s ease;
}
.step-card:hover { border-color: #e2e8f0; box-shadow: 0 20px 40px rgba(0,0,0,0.02); }

.step-indicator { display: flex; flex-direction: column; align-items: center; gap: 12px; }
.step-num {
  width: 44px; height: 44px; background: #0f172a; color: #eab308;
  border-radius: 12px; display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 18px; position: relative; z-index: 2;
}
.step-line { width: 2px; flex-grow: 1; background: #f1f5f9; }
.step-title { font-weight: 800; font-size: 1.25rem; color: #0f172a; margin: 0; }

.section-tag-mini {
  font-size: 10px; font-weight: 800; color: #64748b; background: #f1f5f9;
  padding: 4px 10px; border-radius: 6px; text-transform: uppercase;
}

/* --- INPUTS & SELECTS --- */
.custom-select-wrapper { position: relative; display: flex; gap: 12px; }
.select-icon { position: absolute; left: 18px; top: 50%; transform: translateY(-50%); color: #eab308; pointer-events: none; }
.premium-select, .premium-input {
  width: 100%; padding: 15px 15px 15px 50px; border-radius: 16px; border: 2px solid #f1f5f9;
  background: #f8fafc; font-weight: 600; font-size: 15px; outline: none; transition: 0.3s;
}
.premium-input { padding-left: 20px; }
.premium-select:focus, .premium-input:focus { border-color: #eab308; background: white; box-shadow: 0 0 0 4px rgba(234,179,8,0.1); }

.add-campagne-btn {
  background: #0f172a; color: white; border: none; width: 56px; border-radius: 16px;
  transition: 0.3s; font-size: 18px;
}
.add-campagne-btn:hover { background: #eab308; color: #0f172a; }

/* --- TABS --- */
.premium-tabs { display: flex; background: #f1f5f9; border-radius: 14px; padding: 6px; gap: 6px; }
.premium-tabs button {
  flex: 1; border: none; padding: 12px; border-radius: 10px;
  font-weight: 700; font-size: 14px; color: #64748b; background: transparent; transition: 0.3s;
}
.premium-tabs button.active { background: white; color: #0f172a; box-shadow: 0 4px 15px rgba(0,0,0,0.05); }

/* --- BUTTONS --- */
.btn-amber-sm { background: #eab308; color: white; border: none; border-radius: 12px; font-weight: 800; padding: 0 25px; transition: 0.3s; }
.btn-amber-sm:hover { background: #ca8a04; transform: translateY(-2px); }

.btn-slate-sm { background: #0f172a; color: white; border: none; border-radius: 12px; font-weight: 800; padding: 12px; transition: 0.3s; }
.btn-slate-sm:hover { background: #1e293b; }

.btn-amber-lg {
  background: #eab308; color: white; border: none; padding: 18px 45px; border-radius: 18px;
  font-weight: 800; font-size: 16px; transition: 0.4s;
}
.btn-amber-lg:hover:not(:disabled) { transform: translateY(-4px); box-shadow: 0 15px 30px rgba(234,179,8,0.3); }

/* --- CSV ZONE --- */
.csv-upload-zone {
  border: 2px dashed #e2e8f0; padding: 40px; border-radius: 20px;
  background: #f8fafc; cursor: pointer; text-align: center; transition: 0.3s;
}
.csv-upload-zone:hover { border-color: #eab308; background: #fefce8; }
.upload-icon-wrap {
  width: 60px; height: 60px; background: white; color: #eab308;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  margin: 0 auto 15px; font-size: 24px; box-shadow: 0 10px 20px rgba(0,0,0,0.05);
}

/* --- PILLS --- */
.pills-grid { display: flex; flex-wrap: wrap; gap: 10px; }
.premium-pill {
  background: #0f172a; color: white; padding: 8px 16px; border-radius: 100px;
  font-size: 13px; font-weight: 600; display: flex; align-items: center; gap: 10px;
  animation: scaleIn 0.3s ease;
}
.pill-close { cursor: pointer; color: #eab308; transition: 0.2s; }
.pill-close:hover { transform: scale(1.2); color: white; }
.btn-clear-all { border: none; background: transparent; color: #ef4444; font-size: 12px; font-weight: 700; text-transform: uppercase; }

/* --- SIDEBAR INFO --- */
.sticky-info { position: sticky; top: 20px; }
.info-card { background: #0f172a; color: white; border-radius: 28px; padding: 32px; }
.info-header { display: flex; align-items: center; gap: 12px; margin-bottom: 24px; }
.info-header h6 { margin: 0; font-weight: 800; font-size: 17px; }

.guide-item { display: flex; gap: 12px; margin-bottom: 16px; }
.guide-dot { width: 6px; height: 6px; background: #eab308; border-radius: 50%; margin-top: 8px; flex-shrink: 0; }
.guide-item p { font-size: 14px; color: #94a3b8; line-height: 1.5; margin: 0; }

.alert-premium {
  background: rgba(255,255,255,0.05); border-radius: 16px; padding: 15px;
  display: flex; gap: 12px; font-size: 12px; color: #94a3b8; line-height: 1.4;
}
.alert-premium i { color: #eab308; font-size: 16px; }

.tip-card {
  background: #fefce8; border: 1px solid #fde68a; border-radius: 20px; padding: 20px;
  display: flex; gap: 15px;
}
.tip-icon { font-size: 24px; color: #eab308; }
.tip-content h6 { font-weight: 800; font-size: 14px; margin-bottom: 4px; color: #854d0e; }
.tip-content p { font-size: 13px; color: #a16207; margin: 0; }

/* --- TOAST V2 --- */
.custom-toast-v2 {
  position: fixed; top: 30px; right: 30px; background: white; border-radius: 18px;
  padding: 20px; z-index: 9999; min-width: 320px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.15);
  overflow: hidden; border: 1px solid #f1f5f9;
}
.toast-content { display: flex; gap: 15px; }
.toast-icon {
  width: 40px; height: 40px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center; font-size: 18px;
}
.success .toast-icon { background: #dcfce7; color: #16a34a; }
.error .toast-icon { background: #fee2e2; color: #ef4444; }
.toast-title { font-weight: 800; font-size: 15px; }
.toast-desc { font-size: 13px; color: #64748b; }
.toast-progress {
  position: absolute; bottom: 0; left: 0; height: 4px; width: 100%;
  background: #eab308; animation: progress 4s linear;
}

@keyframes progress { from { width: 100%; } to { width: 0%; } }
@keyframes scaleIn { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }

/* --- ANIMATIONS --- */
.animate-in { animation: fadeInUp 0.6s ease-out; }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }

.fade-in-quick { animation: fadeIn 0.4s ease; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }

.toast-slide-enter-active, .toast-slide-leave-active { transition: all 0.4s ease; }
.toast-slide-enter-from { transform: translateX(100px); opacity: 0; }
.toast-slide-leave-to { transform: translateX(100px); opacity: 0; }
</style>