<template>
  <div class="elite-admin-root">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <!-- Fond Immersif Elite -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-teal"></div>
      <div class="mesh-grain"></div>
      <div class="grid-overlay"></div>
    </div>

    <div class="d-flex admin-layout position-relative" style="z-index: 10;">
      <AppSidebar class="luxury-sidebar" />

      <div class="main-content flex-grow-1">
        <AppNavbar class="luxury-navbar" />

        <main class="p-4 p-lg-5">

          <!-- HEADER -->
          <header class="station-header">
            <div class="d-flex align-items-center gap-4">
              <div class="station-icon">
                <i class="fa-solid fa-satellite-dish"></i>
              </div>
              <div>
                <p class="station-tagline">GESTION DES ACCÈS</p>
                <h2 class="station-title">Studio de <span>Déploiement</span></h2>
              </div>
            </div>
            <div class="header-right d-flex align-items-center gap-3">
              <div class="system-badge">
                <span class="pulse-dot"></span> RÉSEAU OPTIMAL
              </div>
              <div class="counter-badge">
                <i class="fa-solid fa-paper-plane me-2"></i>
                <span>{{ recentInvites.length }}</span> Envois
              </div>
            </div>
          </header>

          <div class="row g-4 mt-1">

            <!-- COLONNE PRINCIPALE -->
            <div class="col-lg-8">

              <!-- CARTE FORMULAIRE -->
              <div class="bento-studio-card">
                <!-- Robot flottant -->
                <div class="bot-deployment-anchor">
                  <div class="robot-float">
                    <svg class="master-bot-deployer" viewBox="0 0 200 200">
                      <circle cx="100" cy="30" r="8" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping"/>
                      <rect x="50" y="55" width="100" height="95" rx="44" fill="white" stroke="#f1f5f9" stroke-width="1.5"/>
                      <rect x="62" y="75" width="76" height="44" rx="20" fill="#0f172a"/>
                      <circle cx="84" cy="97" r="5" fill="#fbbf24" class="led-blink"/>
                      <circle cx="116" cy="97" r="5" fill="#fbbf24" class="led-blink"/>
                      <path d="M86 118 Q100 126 114 118" stroke="#fbbf24" stroke-width="2" fill="none" stroke-linecap="round"/>
                      <line x1="100" y1="30" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
                      <circle cx="100" cy="30" r="6" fill="#fbbf24" class="antenna-core"/>
                      <!-- antennes latérales -->
                      <line x1="50" y1="80" x2="30" y2="70" stroke="#f1f5f9" stroke-width="3" stroke-linecap="round"/>
                      <circle cx="28" cy="69" r="5" fill="#fbbf24" opacity="0.6"/>
                      <line x1="150" y1="80" x2="170" y2="70" stroke="#f1f5f9" stroke-width="3" stroke-linecap="round"/>
                      <circle cx="172" cy="69" r="5" fill="#fbbf24" opacity="0.6"/>
                    </svg>
                  </div>
                </div>

                <div class="card-inner-content">
                  <!-- Steps indicator -->
                  <div class="steps-track mb-5">
                    <div class="step-item step-done">
                      <div class="step-bubble"><i class="fa-solid fa-check"></i></div>
                      <span>Campagne</span>
                    </div>
                    <div class="step-line"></div>
                    <div class="step-item step-active">
                      <div class="step-bubble">2</div>
                      <span>Talent</span>
                    </div>
                    <div class="step-line"></div>
                    <div class="step-item">
                      <div class="step-bubble">3</div>
                      <span>Envoi</span>
                    </div>
                  </div>

                  <h4 class="form-section-title">
                    <i class="fa-solid fa-sliders me-2 text-amber"></i>
                    Configuration de l'accès
                  </h4>

                  <!-- Campagne -->
                  <div class="field-luxury mb-4">
                    <label><i class="fa-solid fa-circle me-2 text-amber-xs"></i>Flux d'Évaluation Cible</label>
                    <div class="input-glow-wrapper">
                      <i class="fa-solid fa-rocket icon-left"></i>
                      <select v-model="form.campagneId" class="luxury-input ps-5" :disabled="loadingCampagnes">
                        <option value="">{{ loadingCampagnes ? 'Synchronisation...' : 'Sélectionner une campagne' }}</option>
                        <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                      </select>
                      <div class="input-border-glow"></div>
                    </div>
                    <p class="field-hint" v-if="form.campagneId">
                      <i class="fa-solid fa-circle-check me-1"></i> Campagne sélectionnée
                    </p>
                  </div>

                  <!-- Email -->
                  <div class="field-luxury mb-5">
                    <label><i class="fa-solid fa-circle me-2 text-amber-xs"></i>Identifiant E-mail du Talent</label>
                    <div class="input-glow-wrapper">
                      <i class="fa-solid fa-at icon-left"></i>
                      <input
                        type="email"
                        v-model="currentEmail"
                        placeholder="talent@entreprise.com"
                        class="luxury-input ps-5"
                        @keyup.enter="sendSingleInvitation"
                      />
                      <div class="input-border-glow"></div>
                    </div>
                  </div>

                  <!-- Footer du formulaire -->
                  <div class="form-footer">
                    <div class="security-cluster">
                      <div class="security-icon">
                        <i class="fa-solid fa-shield-halved"></i>
                      </div>
                      <div>
                        <div class="security-label">Protocole SHA-256</div>
                        <div class="security-sub">Lien chiffré · Validité 72h</div>
                      </div>
                    </div>

                    <button
                      @click="sendSingleInvitation"
                      :disabled="isLoading || !form.campagneId || !currentEmail"
                      class="btn-sunburst"
                    >
                      <template v-if="!isLoading">
                        <i class="fa-solid fa-paper-plane"></i>
                        <span>DÉPLOYER</span>
                      </template>
                      <div v-else class="btn-dots-loader">
                        <span></span><span></span><span></span>
                      </div>
                    </button>
                  </div>
                </div>
              </div>

              <!-- TABLEAU LOGS -->
              <div class="bento-table-card mt-4">
                <div class="table-header-elite">
                  <div class="d-flex align-items-center gap-2">
                    <div class="table-header-icon"><i class="fa-solid fa-clock-rotate-left"></i></div>
                    <span>LOGS DE TRANSMISSION RÉCENTS</span>
                  </div>
                  <div class="table-badge-count">{{ recentInvites.length }} envois</div>
                </div>
                <div class="table-responsive">
                  <table class="table elite-table mb-0">
                    <thead>
                      <tr>
                        <th>DESTINATAIRE</th>
                        <th>CAMPAGNE</th>
                        <th>STATUT</th>
                        <th class="text-end">HEURE</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(item, index) in recentInvites" :key="index" class="table-row-anim">
                        <td>
                          <div class="email-cell">
                            <div class="email-avatar">{{ item.email.charAt(0).toUpperCase() }}</div>
                            <span class="fw-bold">{{ item.email }}</span>
                          </div>
                        </td>
                        <td class="text-muted small">{{ item.campagne || '—' }}</td>
                        <td><span class="status-pill-elite"><i class="fa-solid fa-check me-1"></i>EXPÉDIÉ</span></td>
                        <td class="text-end text-muted small">{{ item.date }}</td>
                      </tr>
                      <tr v-if="recentInvites.length === 0">
                        <td colspan="4" class="empty-state-row">
                          <div class="empty-icon"><i class="fa-solid fa-satellite-dish"></i></div>
                          <p>Aucun signal envoyé pour le moment.</p>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- SIDEBAR DROITE -->
            <div class="col-lg-4">

              <!-- Info Card -->
              <div class="bento-info-card">
                <div class="info-card-top">
                  <div class="info-icon-wrap">
                    <i class="fa-solid fa-wand-magic-sparkles"></i>
                  </div>
                  <h5>Guide de Flux</h5>
                </div>
                <p class="info-desc">Chaque invitation génère un environnement sécurisé unique. Le système Proctoring surveille les anomalies comportementales en temps réel.</p>

                <div class="luxury-divider"></div>

                <div class="info-features">
                  <div class="info-feature-item">
                    <div class="feat-dot"></div>
                    <div>
                      <div class="feat-label">Chiffrement</div>
                      <div class="feat-value">SHA-256 · TLS 1.3</div>
                    </div>
                  </div>
                  <div class="info-feature-item">
                    <div class="feat-dot"></div>
                    <div>
                      <div class="feat-label">Validité du lien</div>
                      <div class="feat-value">72 heures</div>
                    </div>
                  </div>
                  <div class="info-feature-item">
                    <div class="feat-dot"></div>
                    <div>
                      <div class="feat-label">Proctoring</div>
                      <div class="feat-value">Actif · Temps réel</div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Statistiques rapides -->
              <div class="bento-stats-card mt-4">
                <div class="stats-title">ACTIVITÉ DU JOUR</div>
                <div class="stats-grid">
                  <div class="stat-box">
                    <div class="stat-num">{{ recentInvites.length }}</div>
                    <div class="stat-lbl">Invitations</div>
                  </div>
                  <div class="stat-box stat-box-amber">
                    <div class="stat-num">{{ campagnes.length }}</div>
                    <div class="stat-lbl">Campagnes</div>
                  </div>
                  <div class="stat-box stat-full">
                    <i class="fa-solid fa-shield-halved me-2"></i>
                    <span>Système sécurisé · En ligne</span>
                  </div>
                </div>
              </div>

              <!-- Email preview -->
              <div class="bento-preview-card mt-4">
                <div class="preview-header">
                  <div class="preview-dots">
                    <span class="pd red"></span>
                    <span class="pd yellow"></span>
                    <span class="pd green"></span>
                  </div>
                  <span class="preview-label">Aperçu e-mail</span>
                </div>
                <div class="preview-mockup-elite">
                  <div class="preview-logo-row">
                    <div class="prev-logo-box">E</div>
                    <span class="prev-brand">EvaluaTech</span>
                  </div>
                  <div class="mock-line" style="width:70%"></div>
                  <div class="mock-line" style="width:90%"></div>
                  <div class="mock-line" style="width:55%"></div>
                  <div class="mock-btn-preview">
                    <i class="fa-solid fa-arrow-right me-1"></i> Accéder à l'évaluation
                  </div>
                  <div class="mock-line" style="width:40%; margin-top:12px; opacity:0.5"></div>
                </div>
              </div>

            </div>
          </div>
        </main>
      </div>
    </div>

    <!-- TOAST LUXURY -->
    <Transition name="toast-slide">
      <div v-if="statusMsg" :class="['luxury-toast', statusType]">
        <div class="toast-icon">
          <i :class="['fa-solid', statusType === 'success' ? 'fa-circle-check' : 'fa-circle-exclamation']"></i>
        </div>
        <div class="toast-body">
          <div class="toast-title">{{ statusType === 'success' ? 'Transmission réussie' : 'Échec de transmission' }}</div>
          <div class="toast-msg">{{ statusMsg }}</div>
        </div>
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
  } finally {
    loadingCampagnes.value = false;
  }
});

const sendSingleInvitation = async () => {
  if (!form.campagneId || !currentEmail.value) return;
  isLoading.value = true;
  try {
    await api.post('/Invitations/invite-candidates', {
      campagneId: form.campagneId,
      emails: [currentEmail.value.toLowerCase()]
    });
    statusMsg.value = "Signal d'invitation transmis avec succès.";
    statusType.value = 'success';
    const campagneName = campagnes.value.find(c => c.id === form.campagneId)?.nom || '';
    recentInvites.value.unshift({
      email: currentEmail.value,
      campagne: campagneName,
      date: new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' })
    });
    currentEmail.value = '';
    setTimeout(() => statusMsg.value = '', 4000);
  } catch (e) {
    statusMsg.value = 'Échec de la transmission du signal.';
    statusType.value = 'error';
    setTimeout(() => statusMsg.value = '', 4000);
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&display=swap');

/* ===== BASE ===== */
.elite-admin-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background: #f8fafc;
  min-height: 100vh;
  position: relative;
  overflow-x: hidden;
}

/* ===== FOND LUXURY ===== */
.luxury-bg { position: fixed; inset: 0; z-index: 1; pointer-events: none; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(130px); }
.sphere-amber { width: 700px; height: 700px; background: #fbbf24; top: -15%; right: -15%; opacity: 0.1; }
.sphere-blue  { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; left: -10%; opacity: 0.08; }
.sphere-teal  { width: 300px; height: 300px; background: #2dd4bf; top: 50%; left: 40%; opacity: 0.06; }
.mesh-grain {
  position: absolute; inset: 0; opacity: 0.025;
  background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e");
}
.grid-overlay {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(148,163,184,0.06) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148,163,184,0.06) 1px, transparent 1px);
  background-size: 50px 50px;
}

/* ===== STATION HEADER ===== */
.station-header {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: 36px; flex-wrap: wrap; gap: 16px;
}
.station-icon {
  width: 64px; height: 64px; background: #0f172a; color: #fbbf24;
  display: flex; align-items: center; justify-content: center;
  border-radius: 22px; font-size: 26px;
  box-shadow: 0 12px 30px rgba(15,23,42,0.15), inset 0 1px 0 rgba(255,255,255,0.1);
}
.station-tagline { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2.5px; margin: 0 0 4px; }
.station-title { font-weight: 900; font-size: 1.9rem; color: #0f172a; margin: 0; letter-spacing: -1.5px; line-height: 1.1; }
.station-title span { color: #fbbf24; }

.header-right { flex-wrap: wrap; }
.system-badge {
  background: white; border: 1px solid #f1f5f9; padding: 10px 18px;
  border-radius: 50px; font-size: 11px; font-weight: 800; color: #10b981;
  box-shadow: 0 4px 12px rgba(0,0,0,0.04); display: flex; align-items: center;
}
.counter-badge {
  background: #0f172a; color: #fbbf24; padding: 10px 18px;
  border-radius: 50px; font-size: 11px; font-weight: 800;
  display: flex; align-items: center;
}
.pulse-dot {
  width: 8px; height: 8px; background: #10b981; border-radius: 50%;
  display: inline-block; margin-right: 8px;
  animation: pulse-anim 2s infinite;
}
@keyframes pulse-anim {
  0% { box-shadow: 0 0 0 0 rgba(16,185,129,0.7); }
  70% { box-shadow: 0 0 0 8px rgba(16,185,129,0); }
  100% { box-shadow: 0 0 0 0 rgba(16,185,129,0); }
}

/* ===== BENTO STUDIO CARD ===== */
.bento-studio-card {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(24px);
  border-radius: 36px;
  padding: 52px 44px 40px;
  border: 1px solid rgba(255,255,255,0.9);
  box-shadow: 0 24px 60px rgba(0,0,0,0.05), 0 1px 0 rgba(255,255,255,0.8) inset;
  position: relative;
  overflow: visible;
}

/* ===== ROBOT ===== */
.bot-deployment-anchor { position: absolute; top: -80px; right: 44px; }
.robot-float { animation: floatBot 4s ease-in-out infinite; }
.master-bot-deployer { width: 150px; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.1)); }
@keyframes floatBot { 0%,100% { transform: translateY(0) rotate(-1deg); } 50% { transform: translateY(-12px) rotate(1deg); } }
.signal-ping { animation: ping-anim 3s ease-out infinite; transform-origin: center; }
@keyframes ping-anim { 0% { r: 6; opacity: 0.9; } 100% { r: 44; opacity: 0; } }
.led-blink { animation: blink-anim 4s ease-in-out infinite; }
@keyframes blink-anim { 0%,88%,100% { transform: scaleY(1); } 94% { transform: scaleY(0.1); } }

/* ===== STEPS TRACK ===== */
.steps-track { display: flex; align-items: center; gap: 0; }
.step-item { display: flex; flex-direction: column; align-items: center; gap: 6px; }
.step-bubble {
  width: 36px; height: 36px; border-radius: 12px;
  background: #f1f5f9; color: #94a3b8;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 14px; transition: 0.3s;
}
.step-item span { font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.5px; }
.step-done .step-bubble { background: #dcfce7; color: #16a34a; }
.step-active .step-bubble { background: #fbbf24; color: #0f172a; box-shadow: 0 6px 16px rgba(251,191,36,0.35); }
.step-active span { color: #0f172a; }
.step-line { flex: 1; height: 2px; background: #f1f5f9; margin: 0 8px; margin-bottom: 22px; }

/* ===== FORM ===== */
.form-section-title {
  font-weight: 800; font-size: 1.15rem; color: #0f172a; margin-bottom: 28px;
}
.text-amber { color: #fbbf24; }
.text-amber-xs { color: #fbbf24; font-size: 8px; }

.field-luxury label {
  display: block; font-size: 11px; font-weight: 800; color: #475569;
  margin-bottom: 10px; text-transform: uppercase; letter-spacing: 0.8px;
}
.input-glow-wrapper { position: relative; }
.icon-left { position: absolute; left: 18px; top: 50%; transform: translateY(-50%); color: #fbbf24; font-size: 15px; z-index: 2; }
.luxury-input {
  width: 100%; padding: 16px 20px; border-radius: 18px;
  border: 1.5px solid #f1f5f9;
  background: #f8fafc; font-size: 14px; font-weight: 600;
  color: #0f172a; transition: all 0.3s; appearance: none;
}
.luxury-input:focus {
  outline: none; border-color: #fbbf24; background: white;
  box-shadow: 0 0 0 4px rgba(251,191,36,0.1), 0 8px 24px rgba(251,191,36,0.08);
}
.luxury-input::placeholder { color: #cbd5e1; }
.field-hint { font-size: 11px; color: #10b981; font-weight: 700; margin: 8px 0 0 4px; }

/* ===== FORM FOOTER ===== */
.form-footer {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 28px; border-top: 1px solid #f1f5f9; gap: 16px; flex-wrap: wrap;
}
.security-cluster { display: flex; align-items: center; gap: 14px; }
.security-icon {
  width: 42px; height: 42px; background: #f8fafc; border: 1px solid #f1f5f9;
  border-radius: 14px; display: flex; align-items: center; justify-content: center;
  color: #fbbf24; font-size: 16px;
}
.security-label { font-size: 13px; font-weight: 800; color: #0f172a; }
.security-sub { font-size: 11px; color: #94a3b8; margin-top: 2px; }

/* ===== SUNBURST BUTTON ===== */
.btn-sunburst {
  display: flex; align-items: center; gap: 10px;
  padding: 16px 32px; border-radius: 16px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; font-size: 13px; letter-spacing: 0.5px;
  cursor: pointer; transition: all 0.3s;
  box-shadow: 0 10px 24px rgba(251,191,36,0.3);
}
.btn-sunburst:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 18px 36px rgba(251,191,36,0.4);
}
.btn-sunburst:disabled { opacity: 0.5; cursor: not-allowed; }
.btn-dots-loader { display: flex; gap: 5px; align-items: center; padding: 0 8px; }
.btn-dots-loader span {
  width: 6px; height: 6px; background: #0f172a; border-radius: 50%;
  animation: dots 1.2s ease-in-out infinite;
}
.btn-dots-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-dots-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes dots { 0%,80%,100% { transform: scale(0.6); opacity: 0.4; } 40% { transform: scale(1); opacity: 1; } }

/* ===== TABLE BENTO ===== */
.bento-table-card {
  background: white; border-radius: 30px; overflow: hidden;
  border: 1px solid #f1f5f9;
  box-shadow: 0 8px 30px rgba(0,0,0,0.03);
}
.table-header-elite {
  display: flex; justify-content: space-between; align-items: center;
  background: #f8fafc; padding: 18px 24px;
  border-bottom: 1px solid #f1f5f9;
  font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px;
}
.table-header-icon {
  width: 28px; height: 28px; background: #0f172a; color: #fbbf24;
  border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 12px;
}
.table-badge-count {
  background: #fef3c7; color: #b45309; padding: 4px 12px;
  border-radius: 100px; font-size: 11px; font-weight: 800;
}
.elite-table thead th {
  border: none; font-size: 10px; font-weight: 800; color: #cbd5e1;
  letter-spacing: 1px; padding: 14px 24px; text-transform: uppercase;
  background: transparent;
}
.elite-table td { padding: 16px 24px; border-top: 1px solid #f8fafc; font-size: 13px; vertical-align: middle; }
.elite-table tbody tr { transition: background 0.2s; }
.elite-table tbody tr:hover { background: #fafafa; }
.email-cell { display: flex; align-items: center; gap: 10px; }
.email-avatar {
  width: 32px; height: 32px; background: #0f172a; color: #fbbf24;
  border-radius: 10px; display: flex; align-items: center; justify-content: center;
  font-size: 13px; font-weight: 800; flex-shrink: 0;
}
.status-pill-elite {
  background: #fef3c7; color: #b45309;
  padding: 5px 12px; border-radius: 8px;
  font-weight: 800; font-size: 10px; letter-spacing: 0.5px;
}
.empty-state-row { text-align: center; padding: 48px 0 !important; }
.empty-icon { font-size: 32px; color: #e2e8f0; margin-bottom: 12px; }
.empty-state-row p { color: #94a3b8; font-size: 13px; margin: 0; }

/* ===== INFO CARD ===== */
.bento-info-card {
  background: #0f172a; border-radius: 28px; padding: 32px;
  position: relative; overflow: hidden;
  box-shadow: 0 16px 40px rgba(15,23,42,0.2);
}
.bento-info-card::before {
  content: ''; position: absolute; top: -60px; right: -60px;
  width: 200px; height: 200px; background: #fbbf24; opacity: 0.06;
  border-radius: 50%;
}
.info-card-top { display: flex; align-items: center; gap: 14px; margin-bottom: 16px; }
.info-icon-wrap {
  width: 44px; height: 44px; background: rgba(251,191,36,0.15); color: #fbbf24;
  border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 18px;
  flex-shrink: 0;
}
.bento-info-card h5 { color: white; font-weight: 800; margin: 0; font-size: 17px; }
.info-desc { color: #94a3b8; font-size: 13px; line-height: 1.7; margin-bottom: 0; }
.luxury-divider { height: 1px; background: rgba(255,255,255,0.08); margin: 22px 0; }
.info-features { display: flex; flex-direction: column; gap: 16px; }
.info-feature-item { display: flex; align-items: flex-start; gap: 14px; }
.feat-dot { width: 8px; height: 8px; background: #fbbf24; border-radius: 50%; margin-top: 4px; flex-shrink: 0; }
.feat-label { font-size: 10px; font-weight: 800; color: #64748b; text-transform: uppercase; letter-spacing: 0.5px; }
.feat-value { font-size: 13px; font-weight: 700; color: #e2e8f0; margin-top: 2px; }

/* ===== STATS CARD ===== */
.bento-stats-card {
  background: white; border-radius: 24px; padding: 24px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 8px 24px rgba(0,0,0,0.03);
}
.stats-title { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 2px; margin-bottom: 16px; }
.stats-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.stat-box {
  background: #f8fafc; border-radius: 16px; padding: 16px 12px;
  text-align: center; border: 1px solid #f1f5f9;
}
.stat-box-amber { background: #fffbeb; border-color: #fef3c7; }
.stat-num { font-size: 28px; font-weight: 900; color: #0f172a; letter-spacing: -1px; }
.stat-lbl { font-size: 10px; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 2px; }
.stat-full {
  grid-column: 1/-1; background: #0f172a; color: #fbbf24;
  border-radius: 14px; padding: 12px 16px; text-align: center;
  font-size: 11px; font-weight: 800; display: flex; align-items: center; justify-content: center;
}

/* ===== PREVIEW CARD ===== */
.bento-preview-card {
  background: white; border-radius: 24px; overflow: hidden;
  border: 1px solid #f1f5f9;
  box-shadow: 0 8px 24px rgba(0,0,0,0.03);
}
.preview-header {
  background: #f8fafc; padding: 12px 16px; border-bottom: 1px solid #f1f5f9;
  display: flex; align-items: center; gap: 10px;
}
.preview-dots { display: flex; gap: 6px; }
.pd { width: 10px; height: 10px; border-radius: 50%; }
.pd.red { background: #ff5f56; } .pd.yellow { background: #ffbd2e; } .pd.green { background: #27c93f; }
.preview-label { font-size: 11px; font-weight: 700; color: #94a3b8; }
.preview-mockup-elite { padding: 20px; }
.preview-logo-row { display: flex; align-items: center; gap: 8px; margin-bottom: 16px; }
.prev-logo-box {
  width: 28px; height: 28px; background: #fbbf24; color: #0f172a;
  border-radius: 8px; display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 13px;
}
.prev-brand { font-size: 13px; font-weight: 800; color: #0f172a; }
.mock-line { height: 6px; background: #e2e8f0; border-radius: 4px; margin-bottom: 10px; }
.mock-btn-preview {
  background: #0f172a; color: #fbbf24;
  text-align: center; font-size: 11px; font-weight: 800;
  padding: 12px 16px; border-radius: 12px; margin-top: 16px;
}

/* ===== TOAST ===== */
.luxury-toast {
  position: fixed; bottom: 32px; right: 32px;
  display: flex; align-items: center; gap: 16px;
  padding: 18px 24px; border-radius: 22px;
  background: white; z-index: 9999;
  box-shadow: 0 24px 60px rgba(0,0,0,0.12), 0 1px 0 rgba(255,255,255,0.8) inset;
  border: 1px solid #f1f5f9; min-width: 320px;
}
.luxury-toast.success { border-left: 4px solid #10b981; }
.luxury-toast.error   { border-left: 4px solid #ef4444; }
.toast-icon {
  width: 40px; height: 40px; border-radius: 12px;
  display: flex; align-items: center; justify-content: center; font-size: 18px;
  flex-shrink: 0;
}
.success .toast-icon { background: #dcfce7; color: #16a34a; }
.error .toast-icon   { background: #fee2e2; color: #ef4444; }
.toast-title { font-size: 13px; font-weight: 800; color: #0f172a; }
.toast-msg   { font-size: 12px; color: #64748b; margin-top: 2px; }

/* ===== TRANSITIONS ===== */
.toast-slide-enter-active, .toast-slide-leave-active { transition: all 0.4s cubic-bezier(0.34,1.56,0.64,1); }
.toast-slide-enter-from, .toast-slide-leave-to { opacity: 0; transform: translateY(20px) scale(0.95); }

/* ===== RESPONSIVE ===== */
@media (max-width: 991px) {
  .bento-studio-card { padding: 48px 24px 32px; }
  .station-header { flex-direction: column; align-items: flex-start; }
  .bot-deployment-anchor { top: -60px; right: 16px; }
  .master-bot-deployer { width: 110px; }
  .form-footer { flex-direction: column; align-items: flex-start; }
}
@media (max-width: 576px) {
  .luxury-toast { left: 16px; right: 16px; bottom: 16px; min-width: unset; }
}
</style>