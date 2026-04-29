<template>
  <div class="skeleton-container mb-4">
    <div class="d-flex align-items-start gap-3">
      <!-- 1. CERCLE (Icône/Avatar) -->
      <div class="skeleton-item circle shadow-sm"></div>
      
      <div class="flex-grow-1">
        <!-- 2. TITRE -->
        <div class="skeleton-item title mb-3 w-50"></div>
        
        <!-- 3. LIGNES DE TEXTE -->
        <div class="skeleton-item text mb-2 w-100"></div>
        <div class="skeleton-item text mb-2 w-100"></div>
        <div class="skeleton-item text w-75"></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* --- VARIABLES DE THEME --- */
.skeleton-container {
  --skeleton-bg: #f1f5f9;
  --skeleton-shimmer: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0) 0%,
    rgba(255, 255, 255, 0.6) 50%,
    rgba(255, 255, 255, 0) 100%
  );
}

/* Version Mode Sombre (Dark Mode) */
:global([data-theme="dark"]) .skeleton-container {
  --skeleton-bg: #1e293b;
  --skeleton-shimmer: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0) 0%,
    rgba(255, 255, 255, 0.05) 50%,
    rgba(255, 255, 255, 0) 100%
  );
}

/* --- BASE DES ITEMS --- */
.skeleton-item {
  background-color: var(--skeleton-bg);
  position: relative;
  overflow: hidden;
  border-radius: 12px;
}

/* --- EFFET SHIMMER (BALAYAGE) --- */
.skeleton-item::after {
  content: "";
  position: absolute;
  inset: 0;
  transform: translateX(-100%);
  background: var(--skeleton-shimmer);
  animation: shimmer 2s infinite cubic-bezier(0.4, 0, 0.2, 1);
}

/* --- DIMENSIONS --- */
.circle {
  width: 48px;
  height: 48px;
  border-radius: 14px;
  flex-shrink: 0;
}

.title {
  height: 20px;
  border-radius: 8px;
}

.text {
  height: 12px;
  border-radius: 6px;
}

/* --- ANIMATION --- */
@keyframes shimmer {
  100% {
    transform: translateX(100%);
  }
}

/* Optionnel : Légère pulsation de l'échelle pour un effet plus "vivant" */
.skeleton-container {
  animation: pulse 2s infinite ease-in-out;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.85; }
}
</style>