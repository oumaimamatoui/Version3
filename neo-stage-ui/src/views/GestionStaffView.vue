<template>
  <div class="p-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="fw-bold">Gestion du Staff</h2>
      <button class="btn btn-primary rounded-pill px-4">+ Ajouter Staff</button>
    </div>
    
    <div class="row g-4">
      <div v-for="member in staff" :key="member.id" class="col-md-4">
        <div class="card border-0 shadow-sm p-3 rounded-4">
          <div class="d-flex align-items-center gap-3">
            <div class="avatar bg-light p-3 rounded-circle"><i class="fa-solid fa-user-tie fa-2x text-primary"></i></div>
            <div>
              <h6 class="mb-0 fw-bold">{{ member.name }}</h6>
              <small class="text-muted">{{ member.role }}</small>
            </div>
          </div>
          <div class="mt-3 border-top pt-2 d-flex justify-content-between">
            <button class="btn btn-sm btn-outline-danger border-0">Supprimer</button>
            <button class="btn btn-sm btn-light">Modifier</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
const staff = [
  { id: 1, name: "Ahmed Manager", role: "Manager RH" },
  { id: 2, name: "Dr. Slimane", role: "Professeur" },
  { id: 3, name: "Karim Evaluateur", role: "Évaluateur Tech" }
];
</script>