<template>
  <div class="register-container">
    <!-- COUCHES D'ARRIÈRE-PLAN -->
    <div class="background-overlay"></div>
    <div class="glow-orb orb-amber"></div>
    <div class="glow-orb orb-blue"></div>

    <div class="register-box">
      <!-- Header -->
      <div class="text-center mb-4">
        <h3 class="brand-title">Evalua<span>Tech</span></h3>
        <p class="brand-subtitle">Demande d'adhésion organisationnelle</p>
      </div>

      <!-- 1. MESSAGE DE SUCCÈS -->
      <div v-if="requestSent" class="success-screen animate__animated animate__zoomIn">
        <div class="success-icon">
          <i class="fa-solid fa-clock-rotate-left"></i>
        </div>
        <h4 class="fw-800">Dossier en Attente</h4>
        <p class="text-muted-pro">Votre demande a été transmise. Veuillez patienter pendant que le <b>Super Admin</b> valide votre accès. Vous recevrez une confirmation par e-mail.</p>
        <button @click="goToLogin" class="btn-primary-premium mt-3">Retour à la connexion</button>
      </div>

      <!-- 2. FORMULAIRE -->
      <form v-else @submit.prevent="handleRegister">
        <div v-if="errorMessage" class="error-alert">{{ errorMessage }}</div>

        <div class="form-group">
          <label>Nom de l'entreprise</label>
          <input v-model="form.nomEntreprise" type="text" placeholder="Ex: Ma Société" required>
        </div>
        
        <div class="form-group">
          <label>Responsable Principal</label>
          <input v-model="form.nomResponsable" type="text" placeholder="Prénom et Nom" required>
        </div>

        <div class="form-group">
          <label>Email Professionnel</label>
          <input v-model="form.emailResponsable" type="email" placeholder="email@societe.com" required>
        </div>

        <div class="form-group">
          <label>Matricule Fiscale (Optionnel)</label>
          <input v-model="form.matriculeFiscale" type="text" placeholder="Numéro fiscal">
        </div>

        <button type="submit" class="btn-amber" :disabled="isLoading">
          {{ isLoading ? "Envoi en cours..." : "DEMANDER MON ACCÈS" }}
        </button>

        <div class="footer-links">
          <router-link to="/login">Déjà un compte ? <b>Se connecter</b></router-link>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();

// Déclaration obligatoire de TOUTES les variables utilisées dans le template
const isLoading = ref(false);
const requestSent = ref(false);
const errorMessage = ref("");

const form = reactive({
  nomEntreprise: '',
  nomResponsable: '',
  emailResponsable: '',
  matriculeFiscale: ''
});

const goToLogin = () => {
  router.push('/login');
};

const handleRegister = async () => {
  errorMessage.value = "";
  
  // Custom Email Validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailRegex.test(form.emailResponsable)) {
    errorMessage.value = "Veuillez entrer une adresse e-mail valide (ex: moi@entreprise.com).";
    return;
  }

  isLoading.value = true;
  try {
    const response = await axios.post('http://localhost:5172/api/Registration', {
      nomEntreprise: form.nomEntreprise,
      nomResponsable: form.nomResponsable,
      emailResponsable: form.emailResponsable,
      matriculeFiscale: form.matriculeFiscale
    });
    requestSent.value = true;
  } catch (err) {
    console.error(err);
    errorMessage.value = err.response?.data || "Erreur de connexion au serveur.";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
/* STYLE SIMPLIFIÉ POUR ÉVITER LES ERREURS DE RENDU */
.register-container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #f1f5f9;
  position: relative;
}
.background-overlay { position: absolute; inset: 0; background: radial-gradient(circle, #ffffff, #e2e8f0); z-index: 1; }
.glow-orb { position: absolute; border-radius: 50%; filter: blur(80px); z-index: 2; opacity: 0.4; }
.orb-amber { width: 300px; height: 300px; background: #fbbf24; top: 10%; left: 10%; }
.orb-blue { width: 300px; height: 300px; background: #60a5fa; bottom: 10%; right: 10%; }

.register-box {
  width: 100%;
  max-width: 450px;
  background: white;
  padding: 40px;
  border-radius: 24px;
  box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1);
  z-index: 10;
}

.brand-title { font-weight: 800; font-size: 28px; text-align: center; margin: 0; }
.brand-title span { color: #eab308; }
.brand-subtitle { text-align: center; color: #64748b; font-size: 14px; margin-bottom: 30px; }

.form-group { margin-bottom: 15px; }
.form-group label { display: block; font-size: 12px; font-weight: bold; margin-bottom: 5px; color: #475569; }
.form-group input { width: 100%; padding: 12px; border: 1px solid #e2e8f0; border-radius: 10px; outline: none; }

.btn-amber {
  width: 100%;
  padding: 15px;
  background: #eab308;
  color: white;
  border: none;
  border-radius: 12px;
  font-weight: bold;
  cursor: pointer;
  margin-top: 10px;
}
.btn-amber:disabled { opacity: 0.6; }

.footer-links { text-align: center; margin-top: 20px; font-size: 14px; }
.footer-links a { color: #64748b; text-decoration: none; }
.error-alert { background: #fef2f2; color: #dc2626; padding: 10px; border-radius: 8px; margin-bottom: 15px; font-size: 13px; text-align: center; }
.success-screen { text-align: center; }
.success-icon { font-size: 50px; color: #10b981; }
</style>