<template>
  <div class="payment-status-wrapper">
    <div class="glass-card text-center p-5 animate__animated animate__headShake">
      <div class="cancel-icon mb-4">
        <i class="fa-solid fa-circle-xmark"></i>
      </div>
      <h1 class="display-5 fw-800 text-navy mb-3">Paiement Annulé</h1>
      <p class="text-muted mb-4">
        Le processus de paiement a été interrompu. Aucun montant n'a été débité.
        Si vous avez rencontré un problème, n'hésitez pas à nous contacter.
      </p>
      <div class="action-buttons d-flex gap-3 justify-content-center">
        <router-link to="/pricing" class="btn-outline-amber px-4">
          Réessayer
        </router-link>
        <router-link to="/dashboard" class="btn-navy-premium px-4">
          Retour au Dashboard
        </router-link>
      </div>
    </div>
  </div>
</template>

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

.cancel-icon {
  font-size: 80px;
  color: #ef4444;
}

.text-navy { color: #0f172a; }
.fw-800 { font-weight: 800; }

.btn-navy-premium {
  padding: 14px 24px;
  background: #0f172a;
  color: white;
  border-radius: 15px;
  font-weight: 700;
  text-decoration: none;
  transition: 0.3s;
}

.btn-outline-amber {
  padding: 14px 24px;
  border: 2px solid #eab308;
  color: #eab308;
  border-radius: 15px;
  font-weight: 700;
  text-decoration: none;
  transition: 0.3s;
}

.btn-outline-amber:hover {
  background: #fffbeb;
}
</style>