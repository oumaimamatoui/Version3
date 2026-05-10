<template>
  <div class="payment-status-wrapper">
    <div class="glass-card text-center p-5 animate__animated animate__zoomIn">
      <div class="success-icon mb-4">
        <i class="fa-solid fa-circle-check"></i>
      </div>
      <h1 class="display-5 fw-800 text-navy mb-3">Paiement Réussi !</h1>
      <p class="text-muted mb-4">
        Merci pour votre confiance. Votre abonnement <strong>{{ planName }}</strong> est maintenant actif.
        Vous pouvez commencer à utiliser toutes les fonctionnalités IA.
      </p>
      <div class="action-buttons">
        <router-link to="/dashboard" class="btn-amber-premium px-5">
          Aller au Tableau de Bord
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/services/api';

const route = useRoute();
const planName = ref('Business IA');

onMounted(async () => {
  const sessionId = route.query.session_id;
  if (sessionId) {
    try {
      await api.get(`/Payments/confirm-session?session_id=${sessionId}`);
      console.log('Plan mis à jour !');
    } catch (err) {
      console.error('Erreur de confirmation:', err);
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
</style>