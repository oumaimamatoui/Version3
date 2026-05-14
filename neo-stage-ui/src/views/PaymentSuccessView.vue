<template>
  <div class="payment-status-wrapper">
    <div class="glass-card text-center p-5 animate__animated animate__zoomIn">
      <div class="success-icon mb-4">
        <i class="fa-solid fa-circle-check"></i>
      </div>
      <h1 class="display-5 fw-800 text-navy mb-3">Paiement Réussi !</h1>
      <p class="text-muted mb-4">
        Merci pour votre confiance. Votre abonnement <strong>Premium</strong> est maintenant actif.
        Vous pouvez commencer à utiliser toutes les fonctionnalités IA.
      </p>
      <div class="action-buttons d-flex flex-column gap-3">
        <router-link :to="authStore.isAuthenticated ? '/dashboard' : '/login'" class="btn-amber-premium px-5">
          {{ authStore.isAuthenticated ? 'Aller au Tableau de Bord' : 'Se connecter pour commencer' }}
        </router-link>
        
        <a v-if="invoiceUrl" :href="invoiceUrl" target="_blank" class="btn-outline-navy animate__animated animate__fadeIn">
          <i class="fa-solid fa-file-pdf me-2"></i> Télécharger ma facture
        </a>
        <div v-else-if="isLoadingInvoice" class="invoice-loading">
          <i class="fa-solid fa-circle-notch fa-spin me-2"></i> Préparation de votre facture...
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
const route = useRoute();
const planName = ref('Premium');
const invoiceUrl = ref(null);
const isLoadingInvoice = ref(true);
const sessionId = route.query.session_id;

onMounted(async () => {
  if (sessionId) {
    try {
      console.log("Confirmation de la session:", sessionId);
      // 1. Confirmer la session manuellement pour mettre à jour le plan en DB
      await api.post(`/Payments/confirm-session?sessionId=${encodeURIComponent(sessionId)}`, null, {
        headers: { Authorization: '' }
      });
      
      console.log("Récupération de la facture...");
      // 2. Récupérer l'URL de la facture (Anonymous call)
      const res = await api.get(`/Payments/session-invoice/${sessionId}`, {
        headers: { Authorization: '' } 
      });
      console.log("Facture reçue:", res.data);
      invoiceUrl.value = res.data.hostedUrl || res.data.invoiceUrl;
    } catch (err) {
      console.error("DÉTAIL ERREUR PAIEMENT:", err.response?.data || err.message);
    } finally {
      isLoadingInvoice.value = false;
    }
  }
});
</script>

<style scoped>
.payment-status-wrapper {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f8fafc;
  padding: 20px;
}

.glass-card {
  max-width: 500px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 30px;
  border: 1px solid white;
  box-shadow: 0 20px 50px rgba(0,0,0,0.05);
}

.success-icon {
  font-size: 80px;
  color: #10b981;
}

.text-navy { color: #0f172a; }
.fw-800 { font-weight: 800; }

.btn-amber-premium {
  padding: 14px 28px;
  background: #eab308;
  color: white;
  border-radius: 15px;
  font-weight: 700;
  text-decoration: none;
  display: inline-block;
  transition: 0.3s;
}

.btn-amber-premium:hover {
  background: #0f172a;
  transform: translateY(-3px);
}

.btn-outline-navy {
  padding: 12px 24px;
  background: transparent;
  border: 2px solid #0f172a;
  color: #0f172a;
  border-radius: 15px;
  font-weight: 700;
  text-decoration: none;
  display: inline-block;
  transition: 0.3s;
  cursor: pointer;
}

.btn-outline-navy:hover {
  background: #0f172a;
  color: white;
}

.invoice-loading {
  font-size: 13px;
  color: #64748b;
  font-weight: 600;
  padding: 12px;
  border: 1px dashed #cbd5e1;
  border-radius: 15px;
}
</style>