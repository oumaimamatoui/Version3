<template>
  <div class="pricing-wrapper"><link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>
    <div class="tech-grid-subtle"></div>

   
    <nav class="pricing-nav p-4 d-flex justify-content-between align-items-center">
      <router-link to="/" class="logo-link text-decoration-none">
        <div class="logo-box">E</div>
        <h3 class="brand-title">Evalua<span>Tech</span></h3>
      </router-link>
      <router-link to="/connexion" class="btn-back-cyber">
        <i class="fa-solid fa-arrow-left me-2"></i> Retour au portail
      </router-link>
    </nav>

    <div class="container py-5 position-relative">
      
  
      <div class="pricing-robot-mascot d-none d-xl-block">
        <div class="robot-float">
          <svg class="modern-robot" viewBox="0 0 200 200" width="140">
            <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#eab308" stroke-width="2"/>
            <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b" />
            <circle cx="85" cy="81" r="5" fill="#eab308"><animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" /></circle>
            <circle cx="115" cy="81" r="5" fill="#eab308"><animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite" /></circle>
            <path d="M85 110 Q100 118 115 110" stroke="#eab308" stroke-width="2" fill="none" />
          </svg>
          <div class="robot-bubble">Choisissez le plan IA adapté !</div>
        </div>
      </div>

    
      <div class="text-center mb-5 animate__animated animate__fadeIn">
        <span class="badge-premium-ia mb-3">Tarification SaaS & IA</span>
        <h1 class="display-4 fw-800 text-navy">Investissez dans votre <span class="text-amber">Capital Humain</span></h1>
        <p class="text-muted mx-auto" style="max-width: 650px; font-size: 1.1rem; line-height: 1.6;">
          Avec <strong>EvaluaTech</strong>, accédez à l’ensemble de la plateforme gratuitement, y compris les fonctionnalités IA avancées. 
          La version gratuite fonctionne avec des <strong>limites temporaires intelligentes</strong>.
          Passez à <strong>EvaluaTech Go</strong> pour une utilisation continue et sans interruption.
        </p>
      </div>

      <div class="row g-4 justify-content-center align-items-stretch">
        
        <!-- PLAN STARTER (FREE) -->
        <div class="col-lg-4 col-md-6">
          <div class="pricing-card glass-card h-100 animate__animated animate__fadeInLeft">
            <div class="card-header-tech">
              <div class="plan-badge starter-badge">Starter</div>
              <h5 class="fw-bold mt-3">Gratuit</h5>
              <div class="price-box">
                <span class="amount">0</span>
                <span class="currency">DT</span>
                <span class="period">/mois</span>
              </div>
              <p class="desc">Accès complet à la plateforme</p>
            </div>
            <div class="card-body-tech flex-grow-1">
              <ul class="feature-list">
                <li><i class="fa-solid fa-sparkles text-amber"></i> Fonctionnalités IA incluses</li>
                <li><i class="fa-solid fa-check-double text-success"></i> Banque de tests complète</li>
                <li><i class="fa-solid fa-check-double text-success"></i> Rapports de performance</li>
                <li class="premium-highlight"><i class="fa-solid fa-hourglass-half text-amber"></i> Limites temporaires intelligentes</li>
                <li class="premium-highlight"><i class="fa-solid fa-arrows-rotate text-amber"></i> Réactivation automatique</li>
              </ul>
            </div>
            <router-link to="/register" class="btn-outline-starter w-100">Commencer gratuitement</router-link>
          </div>
        </div>

        <!-- PLAN BUSINESS IA (PAID) -->
        <div class="col-lg-4 col-md-6">
          <div class="pricing-card glass-card pro-featured h-100 animate__animated animate__fadeInUp">
            <div class="popular-ribbon"><i class="fa-solid fa-bolt me-1"></i> Utilisation Continue</div>
            <div class="card-header-tech">
              <div class="plan-badge business-badge">EvaluaTech Go</div>
              <h5 class="fw-bold text-amber mt-3">Elite Premium</h5>
              <div class="price-box text-amber">
                <span class="amount">99</span>
                <span class="currency">DT</span>
                <span class="period">/mois</span>
              </div>
              <p class="desc">Utilisation sans interruption</p>
            </div>
            <div class="card-body-tech flex-grow-1">
              <ul class="feature-list">
                <li><i class="fa-solid fa-circle-check text-amber"></i> <strong>Tout le plan Starter</strong></li>
                <li><i class="fa-solid fa-infinity text-amber"></i> Générations illimitées</li>
                <li><i class="fa-solid fa-shield-heart text-amber"></i> Proctoring anti-triche Pro</li>
                <li><i class="fa-solid fa-chart-line text-amber"></i> Analyses RH prédictives</li>
                <li><i class="fa-solid fa-star text-amber"></i> <strong>Zéro interruption pendant 30j</strong></li>
              </ul>
            </div>
            <button 
              @click="handleSubscription('EvaluaTech Go', 99)" 
              class="btn-amber-premium w-100 shadow-amber border-0"
            >
              Passer à EvaluaTech Go
            </button>
          </div>
        </div>

        <!-- PLAN ENTERPRISE (CUSTOM) -->
        <div class="col-lg-4 col-md-6">
          <div class="pricing-card glass-card h-100 animate__animated animate__fadeInRight">
            <div class="card-header-tech">
              <div class="plan-badge enterprise-badge">Enterprise</div>
              <h5 class="fw-bold mt-3">Sur mesure</h5>
              <div class="price-box mt-2">
                <span class="amount-text">Custom</span>
              </div>
              <p class="desc">Pour grandes organisations</p>
            </div>
            <div class="card-body-tech flex-grow-1">
              <ul class="feature-list">
                <li><i class="fa-solid fa-server text-navy"></i> Multi-entreprises</li>
                <li><i class="fa-solid fa-code text-navy"></i> API personnalisée</li>
                <li><i class="fa-solid fa-headset text-navy"></i> Support dédié</li>
                <li><i class="fa-solid fa-shield-halved text-navy"></i> Infrastructure privée</li>
              </ul>
            </div>
            <button class="btn-navy-premium w-100">Contacter l’expert</button>
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { loadStripe } from '@stripe/stripe-js';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';

const authStore = useAuthStore();
const router = useRouter();
const isRedirecting = ref(false);

const handleSubscription = async (planName, price) => {
  if (authStore.isAuthenticated) {
    // ── CAS 1 : DÉJÀ CONNECTÉ (UPGRADE) ──
    try {
      isRedirecting.value = true;
      Swal.fire({
        title: 'Préparation du paiement...',
        html: 'Nous vous redirigeons vers Stripe en toute sécurité.',
        allowOutsideClick: false,
        didOpen: () => Swal.showLoading()
      });

      const response = await api.post('/Payments/create-checkout-session', {
        planName: planName,
        price: price
      });

      if (response.data && response.data.url) {
        window.location.href = response.data.url;
      } else {
        throw new Error('URL de redirection Stripe introuvable');
      }
    } catch (err) {
      console.error('Erreur Stripe :', err);
      Swal.fire('Erreur', 'Impossible d\'initier le paiement. Veuillez réessayer.', 'error');
    } finally {
      isRedirecting.value = false;
    }
  } else {
    // ── CAS 2 : NOUVEL UTILISATEUR ──
    localStorage.setItem('pending_plan', JSON.stringify({ name: planName, price: price }));
    router.push({ 
      path: '/register', 
      query: { plan: planName } 
    });
  }
};
</script>

<style scoped>
/* Styles existants conservés */
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800&display=swap');

.pricing-wrapper {
  background: #f8fafc;
  min-height: 100vh;
  font-family: 'Plus Jakarta Sans', sans-serif;
  position: relative;
  overflow: hidden;
}


.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle at center, transparent 0%, #f1f5f9 100%); z-index: 0; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(100px); z-index: 1; opacity: 0.2; }
.orb-amber { width: 500px; height: 500px; background: #fbbf24; top: -10%; left: -10%; }
.orb-blue { width: 400px; height: 400px; background: #60a5fa; bottom: -5%; right: -5%; }
.tech-grid-subtle {
  position: absolute; inset: 0;
  background-image: linear-gradient(rgba(148, 163, 184, 0.08) 1px, transparent 1px),
                    linear-gradient(90deg, rgba(148, 163, 184, 0.08) 1px, transparent 1px);
  background-size: 45px 45px; z-index: 1;
}

.logo-box {
  width: 38px; height: 38px; background: #0f172a; color: white;
  border-radius: 10px; display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 20px;
}
.logo-link { display: flex; align-items: center; gap: 12px; }
.brand-title { font-weight: 800; color: #0f172a; margin: 0; }
.brand-title span { color: #eab308; }

.btn-back-cyber {
  padding: 10px 20px; background: white; border: 1px solid #e2e8f0;
  border-radius: 12px; font-weight: 700; color: #64748b; text-decoration: none;
  transition: 0.3s;
}
.btn-back-cyber:hover { border-color: #eab308; color: #0f172a; transform: translateX(-5px); }


.pricing-robot-mascot { position: absolute; top: 150px; right: 5%; z-index: 5; }
.robot-float { animation: floatRobot 4s ease-in-out infinite; text-align: center; }
@keyframes floatRobot { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
.robot-bubble {
  background: white; padding: 10px 20px; border-radius: 20px;
  font-size: 13px; font-weight: 700; color: #0f172a; margin-top: 10px;
  box-shadow: 0 10px 20px rgba(0,0,0,0.05); border: 1px solid #fef3c7;
}


.pricing-card {
  padding: 50px 35px; border-radius: 35px; transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  display: flex; flex-direction: column; position: relative; z-index: 2;
}
.glass-card {
  background: rgba(255, 255, 255, 0.85); backdrop-filter: blur(20px);
  border: 1px solid rgba(255, 255, 255, 1);
  box-shadow: 0 25px 60px -15px rgba(0, 0, 0, 0.05);
}
.pricing-card:hover { transform: translateY(-12px); box-shadow: 0 40px 80px -20px rgba(15, 23, 42, 0.1); }

.pro-featured {
  border: 2px solid #eab308 !important; background: rgba(255, 255, 255, 0.95);
  transform: scale(1.05); z-index: 10;
}
.pro-featured:hover { transform: scale(1.05) translateY(-12px); }

.popular-ribbon {
  position: absolute; top: 15px; right: 15px;
  background: #eab308; color: #0f172a; padding: 6px 18px;
  border-radius: 100px; font-size: 11px; font-weight: 800; text-transform: uppercase;
}


.badge-premium-ia {
  background: #fffbeb; color: #b45309; padding: 8px 18px;
  border-radius: 100px; font-size: 12px; font-weight: 800; border: 1px solid #fef3c7;
}
.text-navy { color: #0f172a; }
.text-amber { color: #eab308 !important; }
.fw-800 { font-weight: 800; letter-spacing: -1.5px; }

.price-box { margin: 25px 0; }
.amount { font-size: 3.5rem; font-weight: 900; letter-spacing: -2px; }
.currency { font-size: 1.5rem; font-weight: 700; vertical-align: super; }
.period { font-size: 1rem; color: #94a3b8; font-weight: 600; }
.amount-text { font-size: 2rem; font-weight: 900; }
.desc { font-size: 14px; font-weight: 600; color: #64748b; }

.feature-list { list-style: none; padding: 0; margin-bottom: 40px; }
.feature-list li { margin-bottom: 15px; font-size: 14px; font-weight: 600; color: #334155; display: flex; align-items: center; gap: 12px; }
.feature-list li i { font-size: 16px; color: #10b981; }
.feature-list li.disabled { opacity: 0.4; }
.feature-list li.disabled i { color: #94a3b8; }


.btn-amber-premium {
  padding: 16px; background: #eab308; color: #0f172a; border: none;
  border-radius: 18px; font-weight: 800; text-decoration: none; transition: 0.3s;
  text-align: center; font-size: 15px;
}
.btn-amber-premium:hover { background: #0f172a; color: #fff; transform: scale(1.02); }

.btn-outline-starter {
  padding: 16px; background: transparent; color: #64748b; border: 2px solid #e2e8f0;
  border-radius: 18px; font-weight: 800; text-decoration: none; transition: 0.3s;
  text-align: center;
}
.btn-outline-starter:hover { border-color: #0f172a; color: #0f172a; background: #f8fafc; }

.plan-badge {
  display: inline-block; padding: 4px 12px; border-radius: 8px; font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px;
}
.starter-badge { background: #f1f5f9; color: #64748b; }
.business-badge { background: #fffbeb; color: #b45309; }
.enterprise-badge { background: #e0f2fe; color: #0369a1; }

.premium-highlight {
  background: rgba(251, 191, 36, 0.05);
  padding: 8px 12px;
  border-radius: 12px;
  margin-left: -12px;
  margin-right: -12px;
}

.btn-navy-premium {
  padding: 16px; background: #0f172a; color: #fff; border: none;
  border-radius: 18px; font-weight: 800; transition: 0.3s;
}
.btn-navy-premium:hover { background: #eab308; color: #0f172a; transform: scale(1.02); }

.shadow-amber { box-shadow: 0 15px 30px -5px rgba(234, 179, 8, 0.4); }

@media (max-width: 991px) {
  .pro-featured { transform: scale(1); margin: 20px 0; }
  .pro-featured:hover { transform: translateY(-12px); }
}
</style>