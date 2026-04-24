<template>
  <div class="d-flex admin-layout">
    <AppSidebar />
    <div class="main-content flex-grow-1">
      <AppNavbar />
      <main class="p-4 p-lg-5 animate-in">
        <header class="d-flex justify-content-between align-items-center mb-5">
          <div>
            <h2 class="brand-title">Hub d'<span>Invitations</span></h2>
            <p class="brand-subtitle">STUDIO DE DÉPLOIEMENT EVALUATECH</p>
          </div>
          <div class="status-badge">
            <span class="pulse-dot"></span> Système Opérationnel
          </div>
        </header>

        <div class="row g-4">
          <div class="col-lg-8">
            <div class="cyber-glass-card p-4 p-lg-5 shadow-lg">
              <!-- ETAPE 1 -->
              <div class="step-header mb-4">
                <span class="step-num">01</span>
                <div>
                  <h5 class="fw-800 m-0">Configuration de la Campagne</h5>
                  <p class="small text-muted">Liez ce candidat à un flux d'évaluation actif.</p>
                </div>
              </div>

              <div class="group-select-wrapper mb-5">
                <label class="input-label">Campagne Cible</label>
                <div class="position-relative">
                  <!-- Correction icône : fa-rocket -->
                  <i class="fa-solid fa-rocket icon-left-gold"></i>
                  <select v-model="form.campagneId" class="cyber-select ps-5" :disabled="loadingCampagnes">
                    <option value="">{{ loadingCampagnes ? 'Chargement...' : '— Sélectionner une campagne —' }}</option>
                    <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                  </select>
                </div>
              </div>

              <!-- ETAPE 2 -->
              <div class="step-header mb-4">
                <span class="step-num">02</span>
                <div>
                  <h5 class="fw-800 m-0">Identification du Talent</h5>
                  <p class="small text-muted">Saisissez l'adresse e-mail professionnelle du candidat.</p>
                </div>
              </div>

              <div class="input-wrapper mb-5">
                <label class="input-label">Adresse E-mail</label>
                <div class="position-relative">
                  <i class="fa-solid fa-at icon-left-gold"></i>
                  <input 
                    type="email" 
                    v-model="currentEmail" 
                    placeholder="talent@entreprise.com" 
                    class="cyber-input ps-5"
                    @keyup.enter="sendSingleInvitation"
                  >
                </div>
              </div>

              <div class="action-footer text-end pt-4 border-top">
                <button 
                  @click="sendSingleInvitation" 
                  :disabled="isLoading || !form.campagneId || !currentEmail" 
                  class="btn-primary-gradient-neo px-5 py-3"
                >
                  <span v-if="isLoading"><i class="fa-solid fa-circle-notch fa-spin me-2"></i>SYNCHRONISATION...</span>
                  <span v-else><i class="fa-solid fa-paper-plane me-2"></i> DÉPLOYER L'INVITATION</span>
                </button>
              </div>
            </div>

            <!-- ACTIVITÉ RÉCENTE -->
            <div class="mt-5 animate-up">
              <h6 class="fw-800 mb-3 text-slate-500"><i class="fa-solid fa-history me-2"></i>INVITATIONS RÉCENTES</h6>
              <div class="activity-card p-0 overflow-hidden shadow-sm">
                <table class="table table-hover align-middle mb-0">
                  <thead>
                    <tr>
                      <th>Candidat</th>
                      <th>Statut</th>
                      <th>Date</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, index) in recentInvites" :key="index">
                      <td class="fw-600">{{ item.email }}</td>
                      <td><span class="badge-status-success">Envoyé</span></td>
                      <td class="text-muted small">{{ item.date }}</td>
                    </tr>
                    <tr v-if="recentInvites.length === 0">
                      <td colspan="3" class="text-center py-4 text-muted small italic">Aucune activité récente détectée.</td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <!-- SIDEBAR -->
          <div class="col-lg-4">
            <div class="info-card mb-4">
              <div class="info-icon"><i class="fa-solid fa-lightbulb"></i></div>
              <h6 class="fw-800">Comment ça marche ?</h6>
              <ul class="info-list">
                <li>Le candidat reçoit un e-mail avec un lien d'activation.</li>
                <li>L'accès à l'évaluation est instantané après activation.</li>
                <li>Le suivi se fait via votre dashboard de recrutement.</li>
              </ul>
            </div>
            <div class="preview-card-mockup">
              <div class="mockup-header">Aperçu de l'email</div>
              <div class="mockup-body">
                <div class="skeleton-line sm"></div>
                <div class="skeleton-line lg"></div>
                <div class="mockup-btn" style="background:#0f172a; color:#f59e0b; border:1px solid #f59e0b">Démarrer le test</div>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- TOAST -->
    <transition name="slide-fade">
      <div v-if="statusMsg" :class="['toast-notification', statusType]">
        <i class="fa-solid" :class="statusType === 'success' ? 'fa-check-circle' : 'fa-circle-xmark'"></i>
        <div class="ms-3">{{ statusMsg }}</div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const BASE_URL = '/Invitations';
const campagnes = ref([]);
const currentEmail = ref('');
const isLoading = ref(false);
const loadingCampagnes = ref(false);
const statusMsg = ref('');
const statusType = ref('success');
const recentInvites = ref([]);

const form = reactive({ campagneId: '' });

const fetchCampagnes = async () => {
  loadingCampagnes.value = true;
  try {
    const res = await api.get(`${BASE_URL}/campagnes`);
    campagnes.value = res.data;
  } catch (e) {
    showToast("Impossible de charger les campagnes.", "error");
  } finally {
    loadingCampagnes.value = false;
  }
};

onMounted(fetchCampagnes);

const sendSingleInvitation = async () => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(currentEmail.value)) {
    showToast("Format d'e-mail invalide.", "error");
    return;
  }

  isLoading.value = true;
  try {
    await api.post(`${BASE_URL}/invite-candidates`, {
      campagneId: form.campagneId,
      emails: [currentEmail.value.toLowerCase()]
    });
    
    showToast("Le candidat a été invité avec succès.", "success");
    
    recentInvites.value.unshift({
      email: currentEmail.value,
      date: new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' })
    });

    currentEmail.value = '';
  } catch (error) {
    const errorMsg = error.response?.data || "Échec de l'envoi de l'invitation.";
    showToast(errorMsg, "error");
  } finally {
    isLoading.value = false;
  }
};

const showToast = (msg, type) => {
  statusMsg.value = msg;
  statusType.value = type;
  setTimeout(() => statusMsg.value = '', 4000);
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;800&display=swap');

.admin-layout { background: #f8fafc; min-height: 100vh; font-family: 'Plus Jakarta Sans', sans-serif; }

/* HEADER */
.brand-title { font-weight: 800; font-size: 2.5rem; letter-spacing: -1px; color: #0f172a; }
.brand-title span { color: #f59e0b; }
.brand-subtitle { font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 3px; }

/* CYBER CARD */
.cyber-glass-card {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 30px;
  border: 1px solid #ffffff;
  box-shadow: 0 20px 50px rgba(0,0,0,0.05);
}

.step-num {
  width: 40px; height: 40px; background: #0f172a; color: #f59e0b;
  border-radius: 12px; display: flex; align-items: center; justify-content: center;
  font-weight: 800; margin-right: 15px; font-size: 18px;
}
.step-header { display: flex; align-items: center; }

/* INPUTS */
.input-label { font-size: 10px; font-weight: 800; color: #64748b; text-transform: uppercase; letter-spacing: 1.5px; margin-bottom: 10px; display: block; }
.cyber-select, .cyber-input {
  width: 100%; padding: 16px; border-radius: 18px; border: 2px solid #f1f5f9;
  background: #f8fafc; font-weight: 700; transition: 0.3s;
}
.cyber-select:focus, .cyber-input:focus { border-color: #f59e0b; background: white; outline: none; box-shadow: 0 0 0 5px rgba(245, 158, 11, 0.1); }
.icon-left-gold { position: absolute; left: 18px; top: 50%; transform: translateY(-50%); color: #f59e0b; font-size: 18px; }

/* BUTTONS */
.btn-primary-gradient-neo {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
  color: white; border: none; border-radius: 20px; font-weight: 800;
  transition: 0.4s; letter-spacing: 1px;
}
.btn-primary-gradient-neo:hover:not(:disabled) { background: #f59e0b; color: #0f172a; transform: translateY(-3px); box-shadow: 0 15px 30px rgba(245, 158, 11, 0.3); }

/* ACTIVITY CARD */
.activity-card { background: white; border-radius: 20px; border: 1px solid #f1f5f9; }
.badge-status-success { background: #ecfdf5; color: #10b981; padding: 5px 12px; border-radius: 8px; font-size: 11px; font-weight: 800; }

/* SIDEBAR INFO */
.info-card { background: #0f172a; color: white; padding: 25px; border-radius: 25px; position: relative; overflow: hidden; }
.info-icon { position: absolute; right: -10px; top: -10px; font-size: 80px; opacity: 0.1; color: #f59e0b; }
.info-list { padding-left: 15px; margin-top: 15px; font-size: 13px; color: #94a3b8; }

.preview-card-mockup { background: #e2e8f0; border-radius: 25px; padding: 20px; border: 2px dashed #cbd5e1; }
.mockup-header { font-size: 10px; font-weight: 800; color: #94a3b8; text-transform: uppercase; margin-bottom: 15px; }
.skeleton-line { height: 8px; background: #cbd5e1; border-radius: 4px; margin-bottom: 8px; }
.skeleton-line.sm { width: 40%; } .skeleton-line.lg { width: 90%; } .skeleton-line.md { width: 70%; }
.mockup-btn { background: white; color: #94a3b8; font-size: 10px; font-weight: 800; text-align: center; padding: 10px; border-radius: 10px; margin-top: 15px; }

/* TOAST */
.toast-notification {
  position: fixed; bottom: 30px; right: 30px; padding: 20px 30px; border-radius: 20px;
  display: flex; align-items: center; color: white; font-weight: 700; z-index: 9999; box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.toast-notification.success { background: #10b981; }
.toast-notification.error { background: #ef4444; }

/* ANIMATIONS */
.animate-in { animation: fadeInRight 0.8s ease-out; }
@keyframes fadeInRight { from { opacity: 0; transform: translateX(30px); } to { opacity: 1; transform: translateX(0); } }

.pulse-dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; display: inline-block; margin-right: 8px; animation: pulse 2s infinite; }
@keyframes pulse { 0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); } 70% { transform: scale(1); box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); } 100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); } }
</style>