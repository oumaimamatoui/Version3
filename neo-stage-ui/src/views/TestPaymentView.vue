<template>
  <div class="p-6">
    <h1>Test de Paiement</h1>
    <button @click="handlePayment" class="btn btn-primary">Payer avec Stripe</button>
  </div>
</template>

<script setup>
import { loadStripe } from '@stripe/stripe-js';

const handlePayment = async () => {
  // Votre logique Stripe ici
  console.log("Initialisation du paiement...");
};
</script>