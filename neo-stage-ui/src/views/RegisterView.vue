<template>
  <div class="ultra-register-root">
    <!-- Iconographie Premium -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css">

    <!-- Fond Immersif Multicouche -->
    <div class="aura-background">
      <div class="aura-sphere sphere-amber"></div>
      <div class="aura-sphere sphere-blue"></div>
      <div class="aura-sphere sphere-rose"></div>
      <div class="grainy-texture"></div>
    </div>

    <div class="master-container">
      
      <!-- GAUCHE : L'UNIVERS DU COMPAGNON IA -->
      <div class="side-visual">
        <div class="bot-universe">
          <div class="bot-platform">
            <!-- Robot Master SVG Ultra-Détail -->
            <svg class="master-bot" viewBox="0 0 200 200" xmlns="http://www.w3.org/2000/svg">
              <defs>
                <linearGradient id="screenGrad" x1="0%" y1="0%" x2="0%" y2="100%">
                  <stop offset="0%" style="stop-color:#1e293b;stop-opacity:1" />
                  <stop offset="100%" style="stop-color:#0f172a;stop-opacity:1" />
                </linearGradient>
              </defs>
              <!-- Ondes de signal -->
              <circle cx="100" cy="40" r="10" fill="none" stroke="#fbbf24" stroke-width="1.5" class="signal-ping" />
              <circle cx="100" cy="40" r="22" fill="none" stroke="#fbbf24" stroke-width="1" class="signal-ping delayed" />
              
              <!-- Ombre dynamique -->
              <ellipse cx="100" cy="185" rx="40" ry="10" fill="rgba(15, 23, 42, 0.06)" class="bot-shadow" />
              
              <!-- Corps Perle Blanche -->
              <rect x="55" y="55" width="90" height="95" rx="42" fill="#ffffff" stroke="#f1f5f9" stroke-width="1"/>
              
              <!-- Écran Noir Profond -->
              <rect x="65" y="75" width="70" height="45" rx="18" fill="url(#screenGrad)" class="bot-screen"/>
              
              <!-- Yeux LED Ambre -->
              <circle cx="85" cy="97" r="4.5" fill="#fbbf24" class="led-eye" />
              <circle cx="115" cy="97" r="4.5" fill="#fbbf24" class="led-eye" />
              
              <!-- Antenne -->
              <line x1="100" y1="40" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
              <circle cx="100" cy="40" r="6" fill="#fbbf24" class="antenna-bulb" />
            </svg>
          </div>

          <div class="speech-glass-luxury">
            <div class="bot-header">
              <span class="online-status"></span>
              <strong>EvaluaBot</strong>
              <small class="bot-badge">IA ACTIVE</small>
            </div>
            <p>"Prêt à digitaliser votre organisation ? Laissez-moi configurer votre accès sécurisé !"</p>
          </div>
        </div>
      </div>

      <!-- DROITE : LE PORTAIL D'ADHÉSION -->
      <div class="side-form">
        <div class="bento-card-luxury">
          <div class="form-header">
            <h1 class="logo-shimmer">Evalua<span>Tech</span></h1>
            <div class="header-line"></div>
            <p class="form-subtitle">Initialisation du compte professionnel</p>
          </div>

          <!-- ÉCRAN DE RÉUSSITE -->
          <div v-if="requestSent" class="success-screen animate__animated animate__fadeInUp">
            <div class="success-icon-orbit">
              <i class="fa-solid fa-check"></i>
            </div>
            <h2>Demande Envoyée</h2>
            <p>Nous analysons vos données. Un email de confirmation vous sera envoyé très prochainement.</p>
            <button @click="goToLogin" class="btn-return-classic">Retour à l'accueil</button>
          </div>

          <!-- FORMULAIRE -->
          <form v-else @submit.prevent="handleRegister" class="premium-form">
            <div v-if="errorMessage" class="error-pill">
              <i class="fa-solid fa-circle-exclamation"></i> {{ errorMessage }}
            </div>

            <div class="grid-inputs">
              <div class="field">
                <label>Organisation</label>
                <div class="input-glow-box">
                  <i class="fa-solid fa-building"></i>
                  <input v-model="form.nomEntreprise" type="text" placeholder="Nom de l'entreprise" required>
                </div>
              </div>

              <div class="field">
                <label>Responsable</label>
                <div class="input-glow-box">
                  <i class="fa-solid fa-user-tie"></i>
                  <input v-model="form.nomResponsable" type="text" placeholder="Prénom Nom" required>
                </div>
              </div>
            </div>

            <div class="field">
              <label>Email Professionnel</label>
              <div class="input-glow-box">
                <i class="fa-solid fa-envelope"></i>
                <input v-model="form.emailResponsable" type="email" placeholder="contact@domaine.pro" required>
              </div>
            </div>

            <div class="field">
              <label>Matricule Fiscale <small class="opt-tag">(Optionnel)</small></label>
              <div class="input-glow-box">
                <i class="fa-solid fa-id-card"></i>
                <input v-model="form.matriculeFiscale" type="text" placeholder="Référence légale">
              </div>
            </div>

            <button type="submit" class="btn-amber-gold" :disabled="isLoading">
              <div class="btn-label" v-if="!isLoading">
                <span>DÉPLOYER L'ACCÈS</span>
                <i class="fa-solid fa-arrow-right"></i>
              </div>
              <div v-else class="loader-dots"><span></span><span></span><span></span></div>
              <div class="btn-glass-reflect"></div>
            </button>

            <div class="footer-auth">
              Déjà membre ? <router-link to="/login"><b>Identifiez-vous</b></router-link>
            </div>
          </form>
        </div>
      </div>

    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const isLoading = ref(false);
const requestSent = ref(false);
const errorMessage = ref("");

const form = reactive({
  nomEntreprise: '',
  nomResponsable: '',
  emailResponsable: '',
  matriculeFiscale: ''
});

const goToLogin = () => router.push('/login');

const handleRegister = async () => {
  errorMessage.value = "";
  isLoading.value = true;
  try {
    await new Promise(r => setTimeout(r, 1500));
    await axios.post('http://localhost:5172/api/Registration', form);
    requestSent.value = true;
  } catch (err) {
    errorMessage.value = err.response?.data || "Une erreur technique est survenue.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800&display=swap');

.ultra-register-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #fdfdfd;
  position: relative;
  overflow: hidden;
  padding: 20px;
}

/* --- BACKGROUND DESIGN --- */
.aura-background { position: absolute; inset: 0; z-index: 1; }
.aura-sphere { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.25; animation: orbitMotion 30s infinite linear; }
.sphere-amber { width: 600px; height: 600px; background: #fbbf24; top: -10%; left: -5%; }
.sphere-blue { width: 500px; height: 500px; background: #60a5fa; bottom: -10%; right: -5%; }
.sphere-rose { width: 300px; height: 300px; background: #f472b6; top: 30%; right: 20%; opacity: 0.1; }
.grainy-texture { position: absolute; inset: 0; opacity: 0.02; background-image: url("data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3e%3cfilter id='n'%3e%3cfeTurbulence type='fractalNoise' baseFrequency='0.65' numOctaves='3'/%3e%3c/filter%3e%3crect width='100%25' height='100%25' filter='url(%23n)'/%3e%3c/svg%3e"); }

@keyframes orbitMotion { 
  0% { transform: translate(0, 0); }
  50% { transform: translate(30px, 30px); }
  100% { transform: translate(0, 0); }
}

/* --- MASTER CONTAINER --- */
.master-container { display: flex; width: 100%; max-width: 1100px; z-index: 10; align-items: center; gap: 60px; }

/* --- CÔTÉ ROBOT --- */
.side-visual { flex: 1; display: flex; flex-direction: column; align-items: center; }
.bot-platform { animation: floatBot 4s ease-in-out infinite; }
.master-bot { width: 100%; max-width: 280px; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.06)); }
.bot-shadow { animation: shadowEase 4s ease-in-out infinite; transform-origin: center; }

.signal-ping { animation: pingCircle 3s infinite; transform-origin: center; opacity: 0; }
.delayed { animation-delay: 1.5s; }
@keyframes pingCircle { 0% { r: 5; opacity: 0.8; stroke-width: 2; } 100% { r: 40; opacity: 0; stroke-width: 0.5; } }

.speech-glass-luxury {
  background: rgba(255, 255, 255, 0.8); backdrop-filter: blur(20px);
  padding: 24px; border-radius: 28px; border: 1px solid rgba(255,255,255,1);
  box-shadow: 0 15px 35px rgba(15, 23, 42, 0.04); margin-top: 30px; max-width: 380px;
}
.bot-header { display: flex; align-items: center; gap: 10px; margin-bottom: 10px; }
.online-status { width: 8px; height: 8px; background: #10b981; border-radius: 50%; box-shadow: 0 0 8px #10b981; }
.bot-badge { font-size: 10px; font-weight: 800; color: #64748b; background: #f1f5f9; padding: 2px 8px; border-radius: 6px; }
.speech-glass-luxury p { font-size: 14px; color: #475569; line-height: 1.5; margin: 0; font-weight: 500; }

/* --- CARTE BENTO FORM --- */
.side-form { flex: 1.2; }
.bento-card-luxury {
  background: rgba(255, 255, 255, 0.98); backdrop-filter: blur(40px);
  border: 1px solid #ffffff; padding: 50px; border-radius: 45px;
  box-shadow: 0 40px 100px -20px rgba(0,0,0,0.05);
}

.logo-shimmer { font-size: 36px; font-weight: 900; color: #0f172a; margin: 0; letter-spacing: -2px; }
.logo-shimmer span { color: #fbbf24; }
.header-line { width: 40px; height: 5px; background: #fbbf24; border-radius: 5px; margin: 15px 0; }
.form-subtitle { color: #94a3b8; font-size: 13px; font-weight: 600; margin-bottom: 30px; }

/* --- INPUTS --- */
.grid-inputs { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.field { margin-bottom: 20px; }
.field label { display: block; font-size: 11px; font-weight: 800; color: #475569; text-transform: uppercase; margin-bottom: 10px; letter-spacing: 0.5px; }
.opt-tag { color: #cbd5e1; font-weight: 400; text-transform: none; }

.input-glow-box { position: relative; display: flex; align-items: center; }
.input-glow-box i { position: absolute; left: 18px; color: #fbbf24; font-size: 16px; transition: 0.3s; }
.input-glow-box input {
  width: 100%; padding: 16px 20px 16px 52px; border-radius: 18px; border: 1.5px solid #f1f5f9;
  background: #f8fafc; transition: all 0.3s ease; font-size: 14px; color: #1e293b;
}
.input-glow-box input:focus {
  outline: none; border-color: #fbbf24; background: #ffffff;
  box-shadow: 0 8px 20px rgba(251, 191, 36, 0.08); transform: translateY(-2px);
}

/* --- BOUTON --- */
.btn-amber-gold {
  width: 100%; padding: 18px; border-radius: 20px; border: none;
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: #0f172a; font-weight: 800; cursor: pointer; position: relative;
  overflow: hidden; transition: 0.4s; margin-top: 10px;
  box-shadow: 0 12px 30px rgba(251, 191, 36, 0.3);
}
.btn-amber-gold:hover { transform: translateY(-3px); box-shadow: 0 18px 40px rgba(251, 191, 36, 0.4); }

.btn-glass-reflect {
  position: absolute; top: 0; left: -100%; width: 40%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  animation: reflection 4s infinite linear;
}
@keyframes reflection { 0% { left: -120%; } 30% { left: 150%; } 100% { left: 150%; } }

.btn-label { display: flex; justify-content: center; align-items: center; gap: 10px; }

/* --- SUCCESS SCREEN --- */
.success-screen { text-align: center; padding: 20px 0; }
.success-icon-orbit { width: 70px; height: 70px; background: #fef3c7; color: #fbbf24; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 30px; margin: 0 auto 20px; }
.btn-return-classic { margin-top: 20px; background: #0f172a; color: white; padding: 12px 25px; border-radius: 12px; border: none; cursor: pointer; font-weight: 700; }

/* --- FOOTER --- */
.footer-auth { margin-top: 30px; text-align: center; color: #94a3b8; font-size: 14px; }
.footer-auth b { color: #0f172a; border-bottom: 2px solid #fbbf24; }

/* --- ANIMATIONS --- */
@keyframes floatBot { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-15px); } }
@keyframes shadowEase { 0%, 100% { transform: scale(1); opacity: 0.06; } 50% { transform: scale(0.9); opacity: 0.03; } }

@media (max-width: 992px) {
  .side-visual { display: none; }
  .master-container { justify-content: center; }
  .side-form { width: 100%; max-width: 550px; }
}

.error-pill { background: #fff1f2; color: #e11d48; padding: 12px; border-radius: 14px; margin-bottom: 25px; font-size: 13px; font-weight: 700; border: 1px solid #ffe4e6; text-align: center; }

.loader-dots span { width: 8px; height: 8px; background: #0f172a; border-radius: 50%; display: inline-block; animation: dotAnim 0.6s infinite alternate; margin: 0 3px; }
.loader-dots span:nth-child(2) { animation-delay: 0.2s; }
.loader-dots span:nth-child(3) { animation-delay: 0.4s; }
@keyframes dotAnim { from { transform: translateY(0); opacity: 1; } to { transform: translateY(-5px); opacity: 0.3; } }
</style>