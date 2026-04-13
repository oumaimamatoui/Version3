<template>
  <div id="app">
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </div>
</template>

<style>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

:root {
  --primary: #f59e0b; /* Amber */
  --secondary: #0f172a; /* Navy */
  --bg-light: #e3e8ef; /* Slightly darker gray for better contrast with white inputs */
  --text-muted: #64748b;
  --glass: rgba(255, 255, 255, 0.85);
  --shadow: 0 10px 25px -5px rgba(0, 0, 0, 0.05);
}

body { font-family: 'Plus Jakarta Sans', sans-serif; background-color: var(--bg-light); color: var(--secondary); }
.admin-layout { min-height: 100vh; display: flex; }
.main-content { flex-grow: 1; padding: 2rem; background: var(--bg-light); }

/* Global Components */
.brand-title { font-weight: 800; font-size: 2rem; color: var(--secondary); }
.brand-title span { color: var(--primary); }

.premium-card {
  background: var(--glass); backdrop-filter: blur(12px);
  border: 1px solid white; border-radius: 24px;
  box-shadow: var(--shadow); padding: 1.5rem; transition: 0.3s ease;
}
.premium-card:hover { transform: translateY(-5px); box-shadow: 0 20px 40px rgba(0,0,0,0.08); }

.btn-primary-gold {
  background: linear-gradient(135deg, var(--primary) 0%, #fbbf24 100%);
  color: white; border: none; border-radius: 14px; font-weight: 700;
  padding: 12px 28px; transition: 0.3s; box-shadow: 0 8px 15px rgba(245, 158, 11, 0.2);
}
.btn-primary-gold:hover { transform: scale(1.05); }

.clean-input {
  width: 100%; padding: 14px 18px; border-radius: 14px;
  border: 1.5px solid #e2e8f0; background: white; outline: none; transition: 0.2s;
}
.clean-input:focus { border-color: var(--primary); box-shadow: 0 0 0 4px rgba(245, 158, 11, 0.1); }

/* Status Pills */
.status-pill { padding: 6px 14px; border-radius: 10px; font-size: 11px; font-weight: 800; text-transform: uppercase; }
.active-pill { background: #ecfdf5; color: #10b981; }
.fade-enter-active, .fade-leave-active { transition: opacity 0.2s ease; }
.fade-enter-from, .fade-leave-to { opacity: 0; }
.fw-800 { font-weight: 800; }
.glass-card { background: white; border: 1px solid #f1f5f9; border-radius: 20px; transition: 0.3s; }
.glass-card:hover { transform: translateY(-5px); box-shadow: 0 10px 30px rgba(0,0,0,0.05); }
.icon-box-small { width: 40px; height: 40px; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.bg-indigo { background-color: #0f172a !important; }
.text-amber { color: #eab308 !important; }
.btn-amber { background-color: #eab308; color: #0f172a; border: none; }
.bg-success-soft { background-color: #dcfce7; }
#app {
  zoom: 0.85; /* Chrome & Safari */
  -moz-transform: scale(0.85); /* Firefox */
  -moz-transform-origin: 0 0;
}

</style>