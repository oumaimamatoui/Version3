<template>
  <div class="admin-layout elite-dashboard-root">
    <!-- Fond Immersif Elite (Identique au Login) -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>
    
    <AppSidebar />
    
    <div class="main-viewport">
      <AppNavbar />

      <div class="container-fluid px-xl-5 pt-4 content-container">
        <!-- Header Section style Bento -->
        <header class="content-header animate__animated animate__fadeIn">
          <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-4 mb-5">
            <div>
              <nav class="elite-breadcrumb mb-2">
                <span class="root">PLATEFORME</span>
                <i class="fa-solid fa-chevron-right mx-2"></i>
                <span class="current">TABLEAU DE BORD</span>
              </nav>
              <h1 class="main-title">
                Mes <span class="text-amber-gradient">Évaluations</span>
              </h1>
              <p class="subtitle">Prêt pour votre prochaine étape professionnelle ?</p>
            </div>

            <div class="stats-bento-container">
              <div class="stat-item">
                <div class="stat-value">{{ activeTests.length }}</div>
                <div class="stat-label">DISPONIBLES</div>
              </div>
              <div class="stat-divider"></div>
              <div class="stat-item">
                <div class="stat-value text-muted">{{ expiredTests.length }}</div>
                <div class="stat-label">ARCHIVÉS</div>
              </div>
            </div>
          </div>
        </header>

        <!-- Loader State avec le Robot -->
        <div v-if="loading" class="loader-portal">
          <div class="robot-mini-float">
             <svg class="mini-bot" viewBox="0 0 200 200">
                <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
                <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a" />
                <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
                <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink" />
             </svg>
          </div>
          <span class="loading-text">Synchronisation de vos accès...</span>
        </div>

        <div v-else class="content-body">
          <!-- SECTION 1: TESTS ACTIFS -->
          <section class="test-section mb-5">
            <div class="section-tag mb-4">
              <span class="pulse-amber"></span>
              SESSIONS ACTIVES
            </div>

            <div class="row g-4">
              <!-- Empty State -->
              <div v-if="activeTests.length === 0" class="col-12">
                <div class="bento-empty-card">
                  <i class="fa-solid fa- inbox mb-3"></i>
                  <p>Aucune session n'est programmée pour le moment.</p>
                </div>
              </div>

              <!-- Test Cards -->
              <div 
                v-for="(c, index) in activeTests" 
                :key="c.id" 
                class="col-md-6 col-xl-4 animate__animated animate__fadeInUp"
                :style="{ animationDelay: `${index * 0.1}s` }"
              >
                <div class="card-bento-elite" :class="{ 'is-upcoming': isUpcoming(c.dateDebut) }">
                  <div class="card-top-info">
                    <span class="status-badge" v-if="!isUpcoming(c.dateDebut)">
                      <i class="fa-solid fa-circle-play me-1"></i> OUVERT
                    </span>
                    <span class="status-badge-waiting" v-else>
                      <i class="fa-regular fa-clock me-1"></i> ATTENTE
                    </span>
                    <div class="time-pill">
                      <i class="fa-solid fa-stopwatch me-2"></i>{{ c.dureeMinutes }} min
                    </div>
                  </div>
                  
                  <div class="card-main-content">
                    <h3 class="test-title">{{ c.nom }}</h3>
                    <p class="test-summary">{{ c.description || 'Évaluation technique de haut niveau pour valider vos compétences.' }}</p>
                    
                    <div class="meta-bento-row">
                      <div class="meta-box">
                        <label>VALIDE JUSQU'AU</label>
                        <span>{{ formatDate(c.dateFin) }}</span>
                      </div>
                    </div>
                  </div>

                  <div class="card-action-area">
                    <button 
                      @click="startExam(c)" 
                      class="btn-sunburst-elite" 
                      :disabled="isUpcoming(c.dateDebut)"
                    >
                      <div class="btn-label" v-if="isUpcoming(c.dateDebut)">
                        OUVERTURE : <span class="mono-timer">{{ getCountdown(c.dateDebut) }}</span>
                      </div>
                      <div class="btn-label" v-else>
                        DÉMARRER LA SESSION <i class="fa-solid fa-arrow-right-long ms-2"></i>
                      </div>
                      <div class="shine-sweep"></div>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </section>

          <!-- SECTION 2: ARCHIVES -->
          <section class="test-section archives-section">
            <h2 class="section-tag mb-4">HISTORIQUE RÉCENT</h2>
            <div class="row g-4">
              <div v-for="c in expiredTests" :key="c.id" class="col-md-6 col-xl-4">
                <div class="card-bento-elite expired">
                  <div class="card-main-content">
                    <div class="d-flex justify-content-between">
                       <h3 class="test-title">{{ c.nom }}</h3>
                       <span class="badge-expired">TERMINÉ</span>
                    </div>
                    <div class="meta-box mt-3">
                      <label>Session fermée le</label>
                      <span>{{ formatDate(c.dateFin) }}</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/api';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const router = useRouter();
const campaigns = ref([]);
const loading = ref(true);
const now = ref(new Date());
let timer = null;

const fetchMyTests = async () => {
  try {
    const res = await api.get('/Campagnes');
    campaigns.value = res.data;
  } catch (err) {
    console.error("Erreur API:", err);
  } finally {
    loading.value = false;
  }
};

const activeTests = computed(() => campaigns.value.filter(c => new Date(c.dateFin) >= now.value));
const expiredTests = computed(() => campaigns.value.filter(c => new Date(c.dateFin) < now.value));
const isUpcoming = (date) => new Date(date) > now.value;

const startExam = (campaign) => {
  const targetId = campaign.candidatureId || campaign.id;
  router.push(`/exam-lobby/${targetId}`);
};

const formatDate = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }) : '---';

const getCountdown = (dateDebut) => {
  const diff = new Date(dateDebut) - now.value;
  if (diff <= 0) return "00:00:00";
  const h = Math.floor(diff / 3600000).toString().padStart(2, '0');
  const m = Math.floor((diff % 3600000) / 60000).toString().padStart(2, '0');
  const s = Math.floor((diff % 60000) / 1000).toString().padStart(2, '0');
  return `${h}:${m}:${s}`;
};

onMounted(() => {
  fetchMyTests();
  timer = setInterval(() => now.value = new Date(), 1000);
});

onUnmounted(() => clearInterval(timer));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&family=JetBrains+Mono:wght@500;700&display=swap');

/* --- FOND ELITE (IDENTIQUE LOGIN) --- */
.elite-dashboard-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  background-color: #fdfdfd;
  min-height: 100vh;
  position: relative;
  overflow-x: hidden;
}

.luxury-bg { position: fixed; inset: 0; z-index: 0; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.2; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; right: -5%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; left: -5%; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 30%; left: 20%; opacity: 0.05; }
.mesh-grain { position: absolute; inset: 0; opacity: 0.02; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

.main-viewport { position: relative; z-index: 1; }

/* --- TYPOGRAPHIE & HEADER --- */
.elite-breadcrumb {
  font-size: 11px; font-weight: 800; color: #94a3b8; letter-spacing: 1.5px;
}
.elite-breadcrumb .current { color: #fbbf24; }

.main-title { font-size: 2.5rem; font-weight: 900; color: #0f172a; letter-spacing: -1.5px; }
.text-amber-gradient {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle { color: #64748b; font-size: 1rem; font-weight: 500; }

/* --- STATS BENTO --- */
.stats-bento-container {
  background: white; border: 1px solid white; border-radius: 25px;
  display: flex; padding: 15px 30px; align-items: center;
  box-shadow: 0 15px 35px rgba(0,0,0,0.03);
}
.stat-item { text-align: center; }
.stat-value { font-size: 1.8rem; font-weight: 800; color: #0f172a; line-height: 1; }
.stat-label { font-size: 10px; font-weight: 800; color: #94a3b8; letter-spacing: 1px; margin-top: 5px; }
.stat-divider { width: 1px; height: 30px; background: #f1f5f9; margin: 0 25px; }

/* --- CARTES BENTO --- */
.card-bento-elite {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(20px);
  border: 1px solid white;
  border-radius: 35px;
  padding: 30px;
  height: 100%;
  display: flex;
  flex-direction: column;
  transition: all 0.4s cubic-bezier(0.165, 0.84, 0.44, 1);
  box-shadow: 0 10px 30px rgba(0,0,0,0.02);
}

.card-bento-elite:hover {
  transform: translateY(-8px);
  background: white;
  box-shadow: 0 30px 60px rgba(0,0,0,0.06);
  border-color: #fbbf24;
}

.card-top-info { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; }

.status-badge {
  background: #ecfdf5; color: #10b981; font-size: 10px; font-weight: 800;
  padding: 6px 14px; border-radius: 12px;
}
.status-badge-waiting {
  background: #fffbeb; color: #d97706; font-size: 10px; font-weight: 800;
  padding: 6px 14px; border-radius: 12px;
}

.time-pill { font-size: 12px; font-weight: 700; color: #94a3b8; }

.test-title { font-size: 1.4rem; font-weight: 800; color: #0f172a; margin-bottom: 12px; }
.test-summary { font-size: 0.95rem; color: #64748b; line-height: 1.6; margin-bottom: 20px; flex-grow: 1; }

.meta-box label { display: block; font-size: 10px; font-weight: 800; color: #cbd5e1; margin-bottom: 4px; text-transform: uppercase; }
.meta-box span { font-family: 'JetBrains Mono', monospace; font-size: 13px; font-weight: 700; color: #475569; }

/* --- BOUTON ELITE (STYLE LOGIN) --- */
.card-action-area { margin-top: 25px; }

.btn-sunburst-elite {
  width: 100%; padding: 16px; border-radius: 20px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; font-size: 0.85rem; cursor: pointer;
  position: relative; overflow: hidden; transition: 0.4s;
  box-shadow: 0 10px 20px rgba(251, 191, 36, 0.2);
}

.btn-sunburst-elite:hover:not(:disabled) {
  transform: translateY(-3px);
  box-shadow: 0 15px 30px rgba(251, 191, 36, 0.3);
}

.btn-sunburst-elite:disabled {
  background: #f1f5f9; color: #94a3b8; cursor: not-allowed; box-shadow: none;
}

.shine-sweep {
  position: absolute; top: 0; left: -100%; width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  animation: sweep 4s infinite;
}
@keyframes sweep { 0% { left: -120%; } 30% { left: 150%; } 100% { left: 150%; } }

/* --- SECTION TAG & DECOR --- */
.section-tag {
  display: inline-flex; align-items: center; gap: 10px;
  font-size: 11px; font-weight: 800; color: #0f172a; letter-spacing: 1px;
}
.pulse-amber {
  width: 8px; height: 8px; background: #fbbf24; border-radius: 50%;
  box-shadow: 0 0 0 rgba(251, 191, 36, 0.4); animation: pulse-ring 2s infinite;
}
@keyframes pulse-ring {
  0% { box-shadow: 0 0 0 0 rgba(251, 191, 36, 0.7); }
  70% { box-shadow: 0 0 0 10px rgba(251, 191, 36, 0); }
  100% { box-shadow: 0 0 0 0 rgba(251, 191, 36, 0); }
}

/* --- LOADER --- */
.loader-portal {
  display: flex; flex-direction: column; align-items: center; padding: 100px 0;
}
.mini-bot { width: 100px; height: 100px; filter: drop-shadow(0 10px 20px rgba(0,0,0,0.05)); }
.loading-text { margin-top: 20px; font-weight: 700; color: #94a3b8; font-size: 0.9rem; }
.led-blink { animation: blink 2s infinite; transform-origin: center; }
@keyframes blink { 0%, 100% { opacity: 1; } 50% { opacity: 0.3; } }

/* --- EMPTY STATE --- */
.bento-empty-card {
  padding: 60px; text-align: center; background: rgba(255,255,255,0.4);
  border: 2px dashed #e2e8f0; border-radius: 40px; color: #94a3b8;
}
.bento-empty-card i { font-size: 3rem; opacity: 0.3; }

/* --- ARCHIVES --- */
.card-bento-elite.expired { opacity: 0.7; background: rgba(248, 250, 252, 0.6); }
.badge-expired { font-size: 9px; font-weight: 800; color: #94a3b8; border: 1px solid #e2e8f0; padding: 4px 10px; border-radius: 8px; }

.mono-timer { font-family: 'JetBrains Mono', monospace; color: #0f172a; }

/* Custom Scrollbar */
::-webkit-scrollbar { width: 8px; }
::-webkit-scrollbar-track { background: transparent; }
::-webkit-scrollbar-thumb { background: #e2e8f0; border-radius: 10px; }
::-webkit-scrollbar-thumb:hover { background: #fbbf24; }
</style>