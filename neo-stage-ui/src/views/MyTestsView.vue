<template>
  <div class="admin-layout elite-dashboard-root">
    <!-- Fond dynamique complexe -->
    <div class="luxury-bg">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="mesh-grain"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport">
      <AppNavbar />

      <main class="container-fluid px-xl-5 pt-4 content-container">
        <!-- Header Section -->
        <header class="content-header animate__animated animate__fadeIn">
          <div class="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-4 mb-5">
            <div class="header-text-group">
              <nav class="elite-breadcrumb mb-2">
                <span class="root">PLATEFORME</span>
                <i class="fa-solid fa-chevron-right mx-2"></i>
                <span class="current">TABLEAU DE BORD</span>
              </nav>
              <h1 class="main-title">
                Mes <span class="text-amber-gradient">Évaluations</span>
              </h1>
              <p class="subtitle">Propulsez votre carrière avec nos tests techniques.</p>
            </div>

            <!-- Stats Bento Pill -->
            <div class="stats-bento-container">
              <div class="stat-item">
                <div class="stat-value">{{ activeTests.length }}</div>
                <div class="stat-label">DISPONIBLES</div>
              </div>
              <div class="stat-divider"></div>
              <div class="stat-item">
                <div class="stat-value text-muted-stat">{{ expiredTests.length }}</div>
                <div class="stat-label">ARCHIVÉS</div>
              </div>
            </div>
          </div>
        </header>

        <!-- Loader -->
        <div v-if="loading" class="loader-portal">
          <div class="robot-orbit-container">
            <div class="robot-ring"></div>
            <div class="bot-glow"></div>
            <svg class="mini-bot" viewBox="0 0 200 200">
              <rect x="55" y="55" width="90" height="90" rx="42" fill="white" stroke="#f1f5f9" stroke-width="1"/>
              <rect x="65" y="75" width="70" height="42" rx="18" fill="#0f172a"/>
              <circle cx="85" cy="95" r="4.5" fill="#fbbf24" class="led-blink"/>
              <circle cx="115" cy="95" r="4.5" fill="#fbbf24" class="led-blink"/>
              <rect x="80" y="105" width="40" height="4" rx="2" fill="#1e293b"/>
            </svg>
          </div>
          <span class="loading-text">Analyse de vos sessions...</span>
        </div>

        <div v-else class="content-body">
          <!-- SECTION 1 : SESSIONS ACTIVES -->
          <section class="test-section mb-5">
            <div class="section-header mb-4">
              <div class="section-tag-premium">
                <span class="pulse-amber"></span>
                SESSIONS ACTIVES
                <span class="count-pill">{{ activeTests.length }}</span>
              </div>
            </div>

            <div class="row g-4">
              <!-- Empty State -->
              <div v-if="activeTests.length === 0" class="col-12">
                <div class="bento-empty-card">
                  <div class="empty-icon-wrap">
                    <i class="fa-solid fa-wind"></i>
                  </div>
                  <p class="empty-title">C'est le calme plat ici...</p>
                  <p class="empty-sub">Aucun test en attente pour le moment.</p>
                </div>
              </div>

              <!-- Test Card -->
              <div
                v-for="(c, index) in activeTests"
                :key="c.id"
                class="col-md-6 col-xl-4 animate__animated animate__fadeInUp"
                :style="{ animationDelay: `${index * 0.15}s` }"
              >
                <div class="card-bento-premium" :class="{ 'is-upcoming': isUpcoming(c.dateDebut) }">
                  <div class="glass-reflection"></div>
                  
                  <div class="card-top-info">
                    <span class="badge-status" :class="isUpcoming(c.dateDebut) ? 'waiting' : 'open'">
                      <span class="dot"></span>
                      {{ isUpcoming(c.dateDebut) ? 'EN ATTENTE' : 'OUVERT' }}
                    </span>
                    <div class="duration-tag">
                      <i class="fa-regular fa-hourglass-half me-1"></i> {{ c.dureeMinutes }} min
                    </div>
                  </div>

                  <div class="card-main-content">
                    <h3 class="test-title">{{ c.nom }}</h3>
                    <p class="test-description">{{ c.description || 'Validation des compétences techniques avancées.' }}</p>

                    <div class="expiry-box">
                      <div class="expiry-label">DATE LIMITE</div>
                      <div class="expiry-date">{{ formatDate(c.dateFin) }}</div>
                    </div>
                  </div>

                  <div class="card-footer-action">
                    <button
                      @click="startExam(c)"
                      class="btn-premium-action"
                      :disabled="isUpcoming(c.dateDebut)"
                    >
                      <template v-if="isUpcoming(c.dateDebut)">
                        <span class="timer-label">OUVRE DANS</span>
                        <span class="timer-value">{{ getCountdown(c.dateDebut) }}</span>
                      </template>
                      <template v-else>
                        COMMENCER LE TEST
                        <i class="fa-solid fa-arrow-right ms-2"></i>
                      </template>
                      <div class="btn-shine"></div>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </section>

          <!-- SECTION 2 : HISTORIQUE -->
          <section class="test-section archives-section" v-if="expiredTests.length > 0">
            <div class="section-header mb-4">
              <div class="section-tag-muted">
                HISTORIQUE DES SESSIONS
                <span class="count-pill-muted">{{ expiredTests.length }}</span>
              </div>
            </div>
            <div class="row g-4">
              <div v-for="c in expiredTests" :key="c.id" class="col-md-6 col-xl-4">
                <div class="card-bento-premium expired">
                  <div class="d-flex justify-content-between align-items-center mb-3">
                    <h3 class="test-title-small">{{ c.nom }}</h3>
                    <span class="badge-expired">TERMINÉ</span>
                  </div>
                  <div class="expiry-box muted">
                    <div class="expiry-label">Clôturé le</div>
                    <div class="expiry-date">{{ formatDate(c.dateFin) }}</div>
                  </div>
                </div>
              </div>
            </div>
          </section>
        </div>
      </main>
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

const formatDate = (d) =>
  d ? new Date(d).toLocaleDateString('fr-FR', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }) : '---';

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
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&family=JetBrains+Mono:wght@600&display=swap');

/* ─── BASE LAYOUT ─── */
.admin-layout {
  display: flex;
  min-height: 100vh;
  background-color: #fafafa;
  font-family: 'Plus Jakarta Sans', sans-serif;
}

.main-viewport {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
  position: relative;
}

/* ─── LUXURY BACKGROUND ─── */
.luxury-bg { position: fixed; inset: 0; z-index: 0; overflow: hidden; pointer-events: none; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(140px); opacity: 0.15; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; right: -5%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; left: -5%; }
.sphere-rose { width: 400px; height: 400px; background: #fda4af; top: 40%; left: 15%; opacity: 0.08; }

/* ─── HEADER & TITLES ─── */
.main-title {
  font-size: 3rem; font-weight: 800; color: #0f172a;
  letter-spacing: -0.04em; line-height: 1;
}
.text-amber-gradient {
  background: linear-gradient(135deg, #f59e0b, #d97706);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.subtitle { color: #64748b; font-size: 1.1rem; font-weight: 500; }

.elite-breadcrumb {
  font-size: 0.65rem; font-weight: 800; color: #94a3b8; letter-spacing: 0.15em;
}
.elite-breadcrumb .current { color: #f59e0b; }

/* ─── STATS PILL ─── */
.stats-bento-container {
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(12px);
  padding: 1.2rem 2.5rem;
  border-radius: 2rem;
  display: flex; align-items: center;
  border: 1px solid rgba(255, 255, 255, 1);
  box-shadow: 0 10px 30px rgba(0,0,0,0.03);
}
.stat-value { font-size: 2.2rem; font-weight: 800; color: #0f172a; line-height: 1; }
.stat-label { font-size: 0.6rem; font-weight: 700; color: #94a3b8; margin-top: 4px; }
.stat-divider { width: 1px; height: 40px; background: #e2e8f0; margin: 0 2rem; }

/* ─── SECTION TAGS ─── */
.section-tag-premium {
  display: inline-flex; align-items: center; gap: 12px;
  font-size: 0.7rem; font-weight: 800; letter-spacing: 0.1em;
  color: #0f172a; background: white; padding: 8px 16px;
  border-radius: 100px; box-shadow: 0 4px 12px rgba(0,0,0,0.03);
}
.count-pill { background: #0f172a; color: #fbbf24; padding: 2px 8px; border-radius: 6px; font-size: 0.65rem; }

/* ─── CARDS PREMIUM ─── */
.card-bento-premium {
  background: rgba(255, 255, 255, 0.6);
  backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 0.8);
  border-radius: 2.5rem;
  padding: 2.2rem;
  height: 100%;
  display: flex; flex-direction: column;
  position: relative; overflow: hidden;
  transition: all 0.5s cubic-bezier(0.19, 1, 0.22, 1);
  box-shadow: 0 4px 24px rgba(0,0,0,0.02);
}

.card-bento-premium:hover {
  transform: translateY(-12px);
  background: rgba(255, 255, 255, 0.95);
  box-shadow: 0 30px 60px rgba(0,0,0,0.08);
  border-color: #fcd34d;
}

.glass-reflection {
  position: absolute; top: -50%; left: -50%; width: 200%; height: 200%;
  background: radial-gradient(circle, rgba(251,191,36,0.05) 0%, transparent 70%);
  opacity: 0; transition: opacity 0.5s; pointer-events: none;
}
.card-bento-premium:hover .glass-reflection { opacity: 1; }

.badge-status {
  padding: 6px 14px; border-radius: 100px; font-size: 0.65rem; font-weight: 800;
  display: inline-flex; align-items: center; gap: 6px;
}
.badge-status.open { background: #dcfce7; color: #15803d; }
.badge-status.waiting { background: #fef3c7; color: #b45309; }
.dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; }
.duration-tag { font-size: 0.75rem; font-weight: 600; color: #94a3b8; }

.test-title { font-size: 1.5rem; font-weight: 800; color: #0f172a; margin: 1.5rem 0 0.5rem; }
.test-description { color: #64748b; font-size: 0.95rem; line-height: 1.6; margin-bottom: 2rem; flex: 1; }

.expiry-box { background: #f8fafc; padding: 1rem 1.5rem; border-radius: 1.2rem; }
.expiry-label { font-size: 0.6rem; font-weight: 700; color: #94a3b8; margin-bottom: 4px; }
.expiry-date { font-family: 'JetBrains Mono', monospace; color: #334155; font-size: 0.85rem; font-weight: 600; }

/* ─── BUTTON ACTION ─── */
.btn-premium-action {
  width: 100%; padding: 1.2rem; border-radius: 1.2rem; border: none;
  background: #0f172a; color: white; font-weight: 700; font-size: 0.85rem;
  position: relative; overflow: hidden; transition: 0.3s;
  margin-top: 1.5rem; cursor: pointer;
}
.btn-premium-action:hover:not(:disabled) { background: #1e293b; transform: scale(1.02); }
.btn-premium-action:disabled { background: #f1f5f9; color: #94a3b8; cursor: not-allowed; }

.timer-label { font-size: 0.6rem; opacity: 0.7; display: block; }
.timer-value { font-family: 'JetBrains Mono'; font-size: 1.1rem; }

/* ─── LOADER PORTAL ─── */
.loader-portal { display: flex; flex-direction: column; align-items: center; justify-content: center; min-height: 400px; }
.robot-orbit-container { position: relative; width: 100px; height: 100px; }
.robot-ring { border: 3px solid transparent; border-top-color: #fbbf24; border-radius: 50%; width: 100%; height: 100%; animation: spin 1s linear infinite; }
.bot-glow { position: absolute; inset: 0; background: radial-gradient(circle, rgba(251,191,36,0.2), transparent); filter: blur(10px); }
.loading-text { margin-top: 2rem; font-weight: 700; color: #64748b; letter-spacing: 0.05em; }

@keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }

/* ─── RESPONSIVE ─── */
@media (max-width: 992px) {
  .main-title { font-size: 2.2rem; }
  .card-bento-premium { padding: 1.5rem; }
}
</style>