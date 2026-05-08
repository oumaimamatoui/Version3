<template>
  <div class="d-flex admin-body">
    <AppSidebar />
    <div class="content flex-grow-1 p-4">
      <AppNavbar />
      <main class="mt-4 animate-fade-in">
        <h2 class="fw-800 text-navy mb-4">Analytics <span class="text-primary">Global</span></h2>

        <div class="row g-4 mb-4">
          <!-- Stats de croissance -->
          <div class="col-md-3" v-for="stat in masterStats" :key="stat.label">
            <div class="glass-card p-4 shadow-sm border-0">
              <div class="tiny fw-bold text-muted uppercase">{{ stat.label }}</div>
              <div class="h3 fw-800 text-navy">{{ stat.val }}</div>
              <div class="tiny text-success fw-bold"><i class="fa fa-arrow-up"></i> {{ stat.trend }}%</div>
            </div>
          </div>
        </div>

        <div class="glass-card p-4">
          <h6 class="fw-bold mb-4">Répartition des abonnements entreprises</h6>
          <div class="chart-mockup d-flex align-items-end gap-3" style="height: 200px;">
             <div class="bg-primary rounded-top w-100" style="height: 40%"></div>
             <div class="bg-amber rounded-top w-100" style="height: 85%"></div>
             <div class="bg-navy rounded-top w-100" style="height: 60%"></div>
          </div>
          <div class="d-flex justify-content-between mt-3 tiny fw-bold text-muted">
            <span>STARTUP</span><span>BUSINESS</span><span>ENTERPRISE</span>
          </div>
        </div>
      </main>
    </div>
  </div>
</template>

<script setup>
const masterStats = [
  { label: 'Organisations', val: '42', trend: 12 },
  { label: 'Utilisateurs', val: '1.2k', trend: 8 },
  { label: 'Tests Passés', val: '8.4k', trend: 25 },
  { label: 'Abonnements', val: '14.2k€', trend: 5 }
];
</script>