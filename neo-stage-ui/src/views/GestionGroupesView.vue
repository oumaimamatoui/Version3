<template>
  <div class="p-4">
    <h2 class="fw-bold mb-4">Groupes & Projets</h2>
    <div class="row g-4">
      <div class="col-md-6" v-for="group in groups" :key="group.name">
        <div class="card border-0 shadow-sm rounded-5 p-4 border-start border-primary border-5">
          <div class="d-flex justify-content-between">
            <h4 class="fw-bold">{{ group.name }}</h4>
            <span class="badge bg-primary-soft text-primary">{{ group.count }} Candidats</span>
          </div>
          <p class="text-muted mt-2">Campagne active : <strong>{{ group.activeCampaign }}</strong></p>
          <div class="d-flex gap-2 mt-3">
            <button class="btn btn-sm btn-primary">Gérer le groupe</button>
            <button class="btn btn-sm btn-light">Statistiques IA</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const groups = [
  { name: "Département Informatique", count: 45, activeCampaign: "Recrutement Été 2024" },
  { name: "Projet Fintech", count: 12, activeCampaign: "Assessment Senior Dev" }
];
</script>