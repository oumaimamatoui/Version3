<template>
  <div class="error-container">
    <div class="text-center animate__animated animate__zoomIn">
      <h1 class="error-code">404</h1>
      <div class="error-illustration mb-4">
        <i class="fa fa-robot fa-4x text-indigo"></i>
      </div>
      <h2 class="fw-bold text-slate-800">Oops! Liaison IA interrompue</h2>
      <p class="text-muted mb-4">La ressource que vous recherchez n'existe pas ou a été déplacée par l'IA.</p>
      <router-link to="/dashboard" class="btn btn-indigo-pro shadow-lg">
        Retourner au Dashboard
      </router-link>
    </div>
  </div>
</template>

<style scoped>
.error-container { height: 100vh; display: flex; align-items: center; justify-content: center; background: #f8fafc; }
.error-code { font-size: 150px; font-weight: 900; color: #e2e8f0; line-height: 1; letter-spacing: -10px; margin-bottom: -40px; }
.btn-indigo-pro { background: #4f46e5; color: white; padding: 15px 40px; border-radius: 50px; font-weight: 700; text-decoration: none; transition: 0.3s; }
.btn-indigo-pro:hover { transform: scale(1.05); box-shadow: 0 10px 25px rgba(79, 70, 229, 0.3); }
</style>