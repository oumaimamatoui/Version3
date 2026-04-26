<template>
  <div class="elite-admin-root">
    <!-- Iconographie Pro -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <!-- Fond Immersif Elite (Cohérence Totale avec le Login) -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="mesh-grain"></div>
    </div>

    <div class="d-flex admin-layout position-relative" style="z-index: 10;">
      <AppSidebar class="luxury-sidebar" />
      
      <div class="main-content flex-grow-1">
        <AppNavbar class="luxury-navbar" />
        
        <main class="p-4 p-lg-5">
          <!-- HEADER DE LA STATION -->
          <header class="station-header animate__animated animate__fadeIn">
            <div class="d-flex align-items-center gap-4">
              <div class="station-icon">
                <i class="fa-solid fa-satellite-dish"></i>
              </div>
              <div>
                <h2 class="station-title">Studio de <span>Déploiement</span></h2>
                <p class="station-tagline">EXPÉDITION DES PROTOCOLES D'ÉVALUATION</p>
              </div>
            </div>
            <div class="system-badge">
              <span class="pulse-dot"></span> ÉTAT DU RÉSEAU : OPTIMAL
            </div>
          </header>

          <div class="row g-4 mt-2">
            <!-- CARTE BENTO PRINCIPALE -->
            <div class="col-lg-8">
              <div class="bento-studio-card animate__animated animate__fadeInUp">
                
                <!-- ROBOT DISPATCHER -->
                <div class="bot-deployment-anchor">
                  <div class="robot-float">
                    <svg class="master-bot-deployer" viewBox="0 0 200 200">
                      <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping" />
                      <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
                      <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
                      <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                      <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                      <line x1="100" y1="40" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
                      <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-core" />
                    </svg>
                  </div>
                </div>

                <div class="card-inner-content">
                  <h4 class="form-section-title">Configuration de l'accès</h4>
                  
                  <!-- ÉTAPE 1 -->
                  <div class="field-luxury mb-4">
                    <label>Flux d'Évaluation Cible</label>
                    <div class="input-glow-wrapper">
                      <i class="fa-solid fa-rocket"></i>
                      <select v-model="form.campagneId" class="luxury-input ps-5" :disabled="loadingCampagnes">
                        <option value="">{{ loadingCampagnes ? 'Synchronisation...' : 'Sélectionner une campagne' }}</option>
                        <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                      </select>
                    </div>
                  </div>

                  <!-- ÉTAPE 2 -->
                  <div class="field-luxury mb-5">
                    <label>Identifiant E-mail du Talent</label>
                    <div class="input-glow-wrapper">
                      <i class="fa-solid fa-at"></i>
                      <input 
                        type="email" 
                        v-model="currentEmail" 
                        placeholder="nom@talent-pro.com" 
                        class="luxury-input ps-5"
                        @keyup.enter="sendSingleInvitation"
                      >
                    </div>
                  </div>

                  <div class="d-flex justify-content-between align-items-center pt-4 border-top border-slate-100">
                    <div class="security-info">
                      <i class="fa-solid fa-shield-halved me-2"></i> Protocole de sécurité activé
                    </div>
                    <button 
                      @click="sendSingleInvitation" 
                      :disabled="isLoading || !form.campagneId || !currentEmail" 
                      class="btn-sunburst-sm"
                    >
                      <div class="btn-label" v-if="!isLoading">
                        <span>DÉPLOYER L'INVITATION</span>
                        <i class="fa-solid fa-paper-plane"></i>
                      </div>
                      <div v-else class="btn-loader-white"><span></span><span></span><span></span></div>
                    </button>
                  </div>
                </div>
              </div>

              <!-- TABLEAU D'ACTIVITÉ BENTO -->
              <div class="bento-table-card mt-4 shadow-sm">
                <div class="table-header">
                  <i class="fa-solid fa-clock-rotate-left"></i> LOGS DE TRANSMISSION RÉCENTS
                </div>
                <div class="table-responsive">
                  <table class="table elite-table mb-0">
                    <thead>
                      <tr>
                        <th>DESTINATAIRE</th>
                        <th>STATUT</th>
                        <th class="text-end">HEURE</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(item, index) in recentInvites" :key="index">
                        <td class="fw-bold">{{ item.email }}</td>
                        <td><span class="status-pill-elite">EXPÉDIÉ</span></td>
                        <td class="text-end text-muted">{{ item.date }}</td>
                      </tr>
                      <tr v-if="recentInvites.length === 0">
                        <td colspan="3" class="text-center py-4 text-muted small">Aucun signal envoyé.</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- SIDEBAR D'INFORMATIONS ÉLITE -->
            <div class="col-lg-4">
              <div class="bento-info-card">
                <div class="info-icon-glow"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
                <h5>Guide de Flux</h5>
                <p>Chaque invitation génère un environnement sécurisé unique. Le système Proctoring surveillera les anomalies comportementales en temps réel.</p>
                <div class="luxury-divider"></div>
                <div class="d-flex align-items-center gap-2 mb-2">
                  <div class="mini-dot"></div>
                  <small>Lien crypté SHA-256</small>
                </div>
                <div class="d-flex align-items-center gap-2">
                  <div class="mini-dot"></div>
                  <small>Validité : 72 heures</small>
                </div>
              </div>

              <div class="bento-preview-card mt-4">
                <div class="preview-mockup-elite">
                  <div class="mock-line-sm"></div>
                  <div class="mock-line-lg"></div>
                  <div class="mock-btn">Accès à l'espace</div>
                </div>
                <p class="text-center mt-3 text-muted small">Visualisation de l'e-mail sortant</p>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>

    <!-- TOAST LUXURY -->
    <Transition name="fade-slide">
      <div v-if="statusMsg" :class="['luxury-toast', statusType]">
        <i :class="['fa-solid me-2', statusType === 'success' ? 'fa-circle-check' : 'fa-circle-exclamation']"></i>
        {{ statusMsg }}
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar from '@/components/AppNavbar.vue';

const campagnes = ref([]);
const currentEmail = ref('');
const isLoading = ref(false);
const loadingCampagnes = ref(false);
const statusMsg = ref('');
const statusType = ref('success');
const recentInvites = ref([]);
const form = reactive({ campagneId: '' });

onMounted(async () => {
  loadingCampagnes.value = true;
  try {
    const res = await api.get('/Invitations/campagnes');
    campagnes.value = res.data;
  } finally { loadingCampagnes.value = false; }
});

const sendSingleInvitation = async () => {
  isLoading.value = true;
  try {
    await api.post('/Invitations/invite-candidates', {
      campagneId: form.campagneId,
      emails: [currentEmail.value.toLowerCase()]
    });
    statusMsg.value = "Signal d'invitation transmis avec succès.";
    statusType.value = "success";
    recentInvites.value.unshift({ email: currentEmail.value, date: new Date().toLocaleTimeString() });
    currentEmail.value = '';
    setTimeout(() => statusMsg.value = '', 4000);
  } catch (e) {
    statusMsg.value = "Échec de la transmission du signal.";
    statusType.value = "error";
  } finally { isLoading.value = false; }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

.elite-admin-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background: #fdfdfd; min-height: 100vh; position: relative; overflow-x: hidden;
}

/* FOND LUXURY (Identique au Login) */
.luxury-bg { position: fixed; inset: 0; z-index: 1; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.15; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; right: -10%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; left: -5%; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.02; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

/* STATION HEADER */
.station-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 40px; }
.station-icon { width: 60px; height: 60px; background: #0f172a; color: #fbbf24; display: flex; align-items: center; justify-content: center; border-radius: 20px; font-size: 24px; box-shadow: 0 10px 20px rgba(0,0,0,0.1); }
.station-title { font-weight: 900; font-size: 2rem; color: #0f172a; margin: 0; letter-spacing: -1px; }
.station-title span { color: #fbbf24; }
.station-tagline { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; }
.system-badge { background: #fff; border: 1px solid #f1f5f9; padding: 10px 20px; border-radius: 50px; font-size: 11px; font-weight: 800; color: #64748b; }
.pulse-dot { width: 8px; height: 8px; background: #10b981; border-radius: 50%; display: inline-block; margin-right: 8px; animation: pulse 2s infinite; }

/* BENTO CARDS */
.bento-studio-card {
  background: rgba(255, 255, 255, 0.9); backdrop-filter: blur(20px);
  border-radius: 40px; padding: 60px 40px 40px; border: 1px solid white;
  box-shadow: 0 30px 60px rgba(0,0,0,0.04); position: relative;
}

/* ROBOT DEPLOYER (Style Login) */
.bot-deployment-anchor { position: absolute; top: -75px; right: 40px; }
.robot-float { animation: floatBot 4s ease-in-out infinite; }
.master-bot-deployer { width: 140px; filter: drop-shadow(0 15px 30px rgba(0,0,0,0.08)); }
@keyframes floatBot { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-10px); } }
.signal-ping { animation: ping 3s infinite; transform-origin: center; }
@keyframes ping { 0% { r: 5; opacity: 0.8; } 100% { r: 40; opacity: 0; } }
.led-blink { animation: blink 4s infinite; transform-origin: center; }
@keyframes blink { 0%, 90%, 100% { transform: scaleY(1); } 95% { transform: scaleY(0.1); } }

.form-section-title { font-weight: 800; font-size: 1.2rem; color: #0f172a; margin-bottom: 30px; }

/* LUXURY INPUTS */
.field-luxury label { display: block; font-size: 11px; font-weight: 800; color: #475569; margin-bottom: 10px; text-transform: uppercase; letter-spacing: 0.5px; }
.input-glow-wrapper { position: relative; display: flex; align-items: center; }
.input-glow-wrapper i { position: absolute; left: 18px; color: #fbbf24; font-size: 16px; }
.luxury-input {
  width: 100%; padding: 15px 20px; border-radius: 18px; border: 1.5px solid #f1f5f9;
  background: #f8fafc; font-size: 14px; font-weight: 600; transition: all 0.3s;
}
.luxury-input:focus { outline: none; border-color: #fbbf24; background: white; box-shadow: 0 10px 20px rgba(251, 191, 36, 0.08); }

/* SUNBURST BUTTON */
.btn-sunburst-sm {
  padding: 14px 28px; border-radius: 15px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; cursor: pointer; transition: 0.3s;
  box-shadow: 0 10px 20px rgba(251, 191, 36, 0.2); font-size: 13px;
}
.btn-sunburst-sm:hover:not(:disabled) { transform: translateY(-3px); box-shadow: 0 15px 30px rgba(251, 191, 36, 0.3); }
.btn-label { display: flex; align-items: center; gap: 10px; }

/* TABLES BENTO */
.bento-table-card { background: white; border-radius: 30px; overflow: hidden; border: 1px solid #f1f5f9; }
.table-header { background: #f8fafc; padding: 20px; font-weight: 800; font-size: 11px; color: #94a3b8; letter-spacing: 1px; }
.elite-table thead th { border: none; font-size: 10px; color: #cbd5e1; padding: 15px 20px; }
.elite-table td { padding: 18px 20px; border-top: 1px solid #f8fafc; font-size: 13px; }
.status-pill-elite { background: #fef3c7; color: #fbbf24; padding: 4px 12px; border-radius: 6px; font-weight: 800; font-size: 10px; }

/* INFO SIDEBAR */
.bento-info-card { background: #0f172a; color: white; border-radius: 30px; padding: 30px; position: relative; overflow: hidden; }
.info-icon-glow { position: absolute; right: -20px; bottom: -20px; font-size: 100px; color: #fbbf24; opacity: 0.1; }
.mini-dot { width: 6px; height: 6px; background: #fbbf24; border-radius: 50%; }
.luxury-divider { height: 1px; background: rgba(255,255,255,0.1); margin: 20px 0; }

.preview-mockup-elite { background: #f1f5f9; border-radius: 20px; padding: 20px; border: 2px dashed #cbd5e1; }
.mock-line-sm { height: 6px; width: 40%; background: #cbd5e1; border-radius: 3px; margin-bottom: 10px; }
.mock-line-lg { height: 6px; width: 90%; background: #cbd5e1; border-radius: 3px; margin-bottom: 20px; }
.mock-btn { background: #0f172a; color: #fbbf24; text-align: center; font-size: 10px; font-weight: 800; padding: 10px; border-radius: 10px; }

/* TOAST LUXURY */
.luxury-toast {
  position: fixed; bottom: 30px; right: 30px; padding: 18px 30px; border-radius: 20px;
  background: #0f172a; color: white; font-weight: 700; z-index: 9999;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2); border-left: 5px solid #fbbf24;
}

@keyframes pulse { 0% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0.7); } 70% { transform: scale(1); box-shadow: 0 0 0 10px rgba(16, 185, 129, 0); } 100% { transform: scale(0.95); box-shadow: 0 0 0 0 rgba(16, 185, 129, 0); } }
</style>