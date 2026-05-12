<template>
  <div class="pricing-wrapper">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css">

    <!-- Background layers identiques à Home.vue -->
    <div class="hero-bg-grid"></div>
    <div class="hero-orb hero-orb-1"></div>
    <div class="hero-orb hero-orb-2"></div>

    <!-- NAVBAR identique à Home.vue -->
    <header class="header-nav sticky-top">
      <div class="container">
        <nav class="main-navigation">
          <router-link to="/" class="brand text-decoration-none">
            <div class="brand-logo-wrap">
              <svg width="42" height="42" viewBox="0 0 42 42" fill="none">
                <rect width="42" height="42" rx="12" fill="#0f172a"/>
                <rect x="8" y="10" width="16" height="3" rx="1.5" fill="#eab308"/>
                <rect x="8" y="16" width="11" height="3" rx="1.5" fill="#eab308" opacity="0.7"/>
                <rect x="8" y="22" width="14" height="3" rx="1.5" fill="#eab308" opacity="0.5"/>
                <circle cx="30" cy="30" r="7" fill="#eab308"/>
                <path d="M27 30l2 2 4-4" stroke="#0f172a" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </div>
            <span class="brand-name">Evalua<span class="text-amber">Tech</span></span>
          </router-link>

          <ul class="nav-links d-none d-lg-flex">
            <li><router-link to="/">Accueil</router-link></li>
            <li><a href="/#services">Services</a></li>
            <li><a href="/#process">Processus</a></li>
            <li><a href="/#about">À propos</a></li>
            <li><router-link to="/pricing" class="active">Tarifs</router-link></li>
          </ul>

          <div class="nav-auth">
            <router-link to="/connexion" class="btn-text d-none d-sm-inline">Connexion</router-link>
            <router-link to="/register" class="btn-amber-pill shadow-amber">
              Essai Gratuit
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="ms-1"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
            </router-link>
          </div>
        </nav>
      </div>
    </header>

    <!-- HERO SECTION TARIFICATION -->
    <section class="pricing-hero">
      <div class="container position-relative">

        <!-- Robot mascot flottant -->
        <div class="pricing-robot-mascot d-none d-xl-block">
          <div class="robot-float">
            <svg class="modern-robot" viewBox="0 0 200 200" width="130">
              <rect x="55" y="50" width="90" height="85" rx="28" fill="#fff" stroke="#eab308" stroke-width="2"/>
              <rect x="65" y="65" width="70" height="32" rx="16" fill="#1e293b"/>
              <circle cx="85" cy="81" r="5" fill="#eab308">
                <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite"/>
              </circle>
              <circle cx="115" cy="81" r="5" fill="#eab308">
                <animate attributeName="opacity" values="1;0.3;1" dur="2s" repeatCount="indefinite"/>
              </circle>
              <path d="M85 110 Q100 118 115 110" stroke="#eab308" stroke-width="2" fill="none"/>
              <rect x="45" y="70" width="10" height="30" rx="5" fill="#eab308" opacity="0.6"/>
              <rect x="145" y="70" width="10" height="30" rx="5" fill="#eab308" opacity="0.6"/>
              <rect x="75" y="135" width="15" height="25" rx="6" fill="#eab308" opacity="0.7"/>
              <rect x="110" y="135" width="15" height="25" rx="6" fill="#eab308" opacity="0.7"/>
            </svg>
            <div class="robot-bubble">Choisissez le plan IA adapté !</div>
          </div>
        </div>

        <div class="text-center pricing-hero-text">
          <div class="hero-badge mb-3">
            <span class="badge-dot"></span>
            Tarification SaaS & IA
          </div>
          <p class="hero-eyebrow">Plans adaptés à chaque organisation</p>
          <h1 class="hero-title">
            Investissez dans votre<br>
            <span class="hero-highlight">Capital Humain</span>
          </h1>
          <p class="hero-desc mx-auto" style="max-width:660px;">
            Accédez à l'ensemble de la plateforme gratuitement, y compris les fonctionnalités IA avancées.
            La version gratuite fonctionne avec des <strong>limites temporaires intelligentes</strong>.
            Passez à <strong>EvaluaTech Go</strong> pour une utilisation continue sans interruption.
          </p>
        </div>

        <!-- Toggle billing period (décoratif) -->
        <div class="billing-toggle">
          <span class="billing-label active">Mensuel</span>
          <div class="toggle-switch">
            <div class="toggle-thumb"></div>
          </div>
          <span class="billing-label">Annuel</span>
          <span class="billing-save">-20%</span>
        </div>
      </div>
    </section>

    <!-- CARDS SECTION -->
    <section class="pricing-cards-section">
      <div class="container">
        <div class="pricing-grid">

          <!-- PLAN STARTER (FREE) -->
          <div class="pricing-card animate-up" style="animation-delay:0s">
            <div class="card-top">
              <div class="plan-icon plan-icon-slate">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><path d="M20 7H4a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2V9a2 2 0 0 0-2-2z"/><path d="M16 21V5a2 2 0 0 0-2-2h-4a2 2 0 0 0-2 2v16"/></svg>
              </div>
              <div class="plan-badge starter-badge">Starter</div>
            </div>
            <h3 class="plan-name">Gratuit</h3>
            <p class="plan-tagline">Accès complet à la plateforme</p>
            <div class="price-display">
              <span class="price-amount">0</span>
              <div class="price-meta">
                <span class="price-currency">DT</span>
                <span class="price-period">/mois</span>
              </div>
            </div>
            <div class="price-note">Gratuit pour toujours</div>

            <div class="features-divider"></div>

            <ul class="features-list">
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-sparkles"></i>
                </div>
                <span>Fonctionnalités IA incluses</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-green">
                  <i class="fa-solid fa-check-double"></i>
                </div>
                <span>Banque de tests complète</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-green">
                  <i class="fa-solid fa-chart-bar"></i>
                </div>
                <span>Rapports de performance</span>
              </li>
              <li class="feature-item feature-highlight">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-hourglass-half"></i>
                </div>
                <span>Limites temporaires intelligentes</span>
              </li>
              <li class="feature-item feature-highlight">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-arrows-rotate"></i>
                </div>
                <span>Réactivation automatique</span>
              </li>
            </ul>

            <router-link to="/register" class="btn-plan btn-plan-outline">
              Commencer gratuitement
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
            </router-link>
          </div>

          <!-- PLAN BUSINESS IA (FEATURED) -->
          <div class="pricing-card pricing-card-featured animate-up" style="animation-delay:0.1s">
            <div class="featured-ribbon">
              <i class="fa-solid fa-bolt me-1"></i> Utilisation Continue
            </div>
            <div class="card-top">
              <div class="plan-icon plan-icon-amber">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/></svg>
              </div>
              <div class="plan-badge business-badge">EvaluaTech Go</div>
            </div>
            <h3 class="plan-name plan-name-amber">Elite Premium</h3>
            <p class="plan-tagline">Utilisation sans interruption</p>
            <div class="price-display">
              <span class="price-amount price-amount-amber">99</span>
              <div class="price-meta">
                <span class="price-currency price-currency-amber">DT</span>
                <span class="price-period">/mois</span>
              </div>
            </div>
            <div class="price-note price-note-amber">Facturé mensuellement · sans engagement</div>

            <div class="features-divider features-divider-amber"></div>

            <ul class="features-list">
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-circle-check"></i>
                </div>
                <span><strong>Tout le plan Starter</strong></span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-infinity"></i>
                </div>
                <span>Générations IA illimitées</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-shield-heart"></i>
                </div>
                <span>Proctoring anti-triche Pro</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-chart-line"></i>
                </div>
                <span>Analyses RH prédictives</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-amber">
                  <i class="fa-solid fa-star"></i>
                </div>
                <span><strong>Zéro interruption pendant 30j</strong></span>
              </li>
            </ul>

            <button
              @click="handleSubscription('EvaluaTech Go', 99)"
              class="btn-plan btn-plan-amber shadow-amber"
            >
              Passer à EvaluaTech Go
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
            </button>

            <p class="plan-guarantee">
              <i class="fa-solid fa-lock me-1"></i>
              Paiement 100% sécurisé via Stripe
            </p>
          </div>

          <!-- PLAN ENTERPRISE -->
          <div class="pricing-card animate-up" style="animation-delay:0.2s">
            <div class="card-top">
              <div class="plan-icon plan-icon-navy">
                <svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>
              </div>
              <div class="plan-badge enterprise-badge">Enterprise</div>
            </div>
            <h3 class="plan-name">Sur mesure</h3>
            <p class="plan-tagline">Pour grandes organisations</p>
            <div class="price-display">
              <span class="price-custom">Custom</span>
            </div>
            <div class="price-note">Devis personnalisé gratuit</div>

            <div class="features-divider"></div>

            <ul class="features-list">
              <li class="feature-item">
                <div class="feature-icon feature-icon-navy">
                  <i class="fa-solid fa-server"></i>
                </div>
                <span>Multi-entreprises & multi-sites</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-navy">
                  <i class="fa-solid fa-code"></i>
                </div>
                <span>API personnalisée & webhooks</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-navy">
                  <i class="fa-solid fa-headset"></i>
                </div>
                <span>Support dédié 24/7</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-navy">
                  <i class="fa-solid fa-shield-halved"></i>
                </div>
                <span>Infrastructure privée on-premise</span>
              </li>
              <li class="feature-item">
                <div class="feature-icon feature-icon-navy">
                  <i class="fa-solid fa-file-contract"></i>
                </div>
                <span>SLA garanti & onboarding dédié</span>
              </li>
            </ul>

            <button class="btn-plan btn-plan-navy">
              Contacter un expert
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
            </button>
          </div>

        </div>
      </div>
    </section>

    <!-- FEATURE COMPARISON TABLE -->
    <section class="comparison-section">
      <div class="container">
        <div class="section-heading text-center mb-5">
          <span class="section-tag">Comparaison détaillée</span>
          <h2 class="section-title mt-3">Tout ce qui est inclus</h2>
          <div class="h-line"></div>
        </div>

        <div class="comparison-table-wrap">
          <table class="comparison-table">
            <thead>
              <tr>
                <th class="feature-col">Fonctionnalité</th>
                <th class="text-center">Starter</th>
                <th class="text-center plan-col-featured">EvaluaTech Go</th>
                <th class="text-center">Enterprise</th>
              </tr>
            </thead>
            <tbody>
              <tr class="table-group-row">
                <td colspan="4">IA & Génération de tests</td>
              </tr>
              <tr v-for="row in comparisonRows.ia" :key="row.label">
                <td>{{ row.label }}</td>
                <td class="text-center"><span v-html="renderCell(row.starter)"></span></td>
                <td class="text-center plan-col-featured"><span v-html="renderCell(row.go)"></span></td>
                <td class="text-center"><span v-html="renderCell(row.enterprise)"></span></td>
              </tr>
              <tr class="table-group-row">
                <td colspan="4">Sécurité & Proctoring</td>
              </tr>
              <tr v-for="row in comparisonRows.security" :key="row.label">
                <td>{{ row.label }}</td>
                <td class="text-center"><span v-html="renderCell(row.starter)"></span></td>
                <td class="text-center plan-col-featured"><span v-html="renderCell(row.go)"></span></td>
                <td class="text-center"><span v-html="renderCell(row.enterprise)"></span></td>
              </tr>
              <tr class="table-group-row">
                <td colspan="4">Support & Intégrations</td>
              </tr>
              <tr v-for="row in comparisonRows.support" :key="row.label">
                <td>{{ row.label }}</td>
                <td class="text-center"><span v-html="renderCell(row.starter)"></span></td>
                <td class="text-center plan-col-featured"><span v-html="renderCell(row.go)"></span></td>
                <td class="text-center"><span v-html="renderCell(row.enterprise)"></span></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </section>

    <!-- TESTIMONIALS SECTION (identique style Home.vue) -->
    <section class="testimonials-section">
      <div class="container">
        <div class="section-heading text-center mb-5">
          <span class="section-tag">Ils nous font confiance</span>
          <h2 class="section-title mt-3">Ce qu'ils disent de nous</h2>
          <div class="h-line"></div>
        </div>
        <div class="row g-4">
          <div class="col-lg-4" v-for="(t, i) in testimonials" :key="i">
            <div class="testimonial-card h-100" :class="{ 'testimonial-featured': i === 1 }">
              <div class="d-flex text-amber mb-3 gap-1">
                <svg v-for="s in 5" :key="s" width="16" height="16" viewBox="0 0 24 24" fill="currentColor"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
              </div>
              <div class="quote-mark">"</div>
              <p class="fst-italic text-muted mt-2">{{ t.quote }}</p>
              <div class="d-flex align-items-center mt-4">
                <div class="avatar-placeholder me-3" :style="{ background: t.color }">{{ t.name.charAt(0) }}</div>
                <div>
                  <h6 class="fw-bold m-0">{{ t.name }}</h6>
                  <small class="text-muted">{{ t.role }}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- FAQ SECTION -->
    <section class="faq-section">
      <div class="container">
        <div class="section-heading text-center mb-5">
          <span class="section-tag">Questions fréquentes</span>
          <h2 class="section-title mt-3">Vous avez des questions ?</h2>
          <div class="h-line"></div>
        </div>
        <div class="faq-grid">
          <div class="faq-item" v-for="(faq, i) in faqs" :key="i" @click="toggleFaq(i)" :class="{ open: openFaq === i }">
            <div class="faq-question">
              <span>{{ faq.q }}</span>
              <div class="faq-arrow">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M6 9l6 6 6-6"/></svg>
              </div>
            </div>
            <div class="faq-answer" :style="{ maxHeight: openFaq === i ? '200px' : '0' }">
              <p>{{ faq.a }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- CTA FINAL (identique Home.vue) -->
    <section class="final-cta py-5">
      <div class="container">
        <div class="cta-box text-center position-relative overflow-hidden">
          <div class="cta-particles">
            <div class="cta-p" v-for="n in 12" :key="n"></div>
          </div>
          <div class="position-relative" style="z-index:10">
            <span class="section-tag light mb-3 d-inline-block">Prêt à commencer ?</span>
            <h2 class="fw-bold text-white mb-3 fs-1">Identifiez vos futurs talents<br>avec l'IA</h2>
            <p class="text-white-50 mb-4 mx-auto" style="max-width:500px">Rejoignez plus de 500 entreprises qui utilisent EvaluaTech pour fiabiliser leurs recrutements techniques.</p>
            <div class="d-flex gap-3 justify-content-center flex-wrap">
              <router-link to="/register" class="btn btn-light btn-lg rounded-pill px-5 fw-bold" style="color:#0f172a">
                Démarrer l'essai gratuit
              </router-link>
              <a href="/#about" class="btn btn-outline-light btn-lg rounded-pill px-5 fw-bold">En savoir plus</a>
            </div>
          </div>
          <div class="cta-circle-1"></div>
          <div class="cta-circle-2"></div>
          <div class="cta-circle-3"></div>
        </div>
      </div>
    </section>

    <!-- FOOTER identique Home.vue -->
    <footer class="footer-dark">
      <div class="container py-5">
        <div class="row gy-4">
          <div class="col-lg-4">
            <div class="d-flex align-items-center gap-2 mb-3">
              <svg width="40" height="40" viewBox="0 0 42 42" fill="none">
                <rect width="42" height="42" rx="12" fill="#eab308"/>
                <rect x="8" y="10" width="16" height="3" rx="1.5" fill="#0f172a"/>
                <rect x="8" y="16" width="11" height="3" rx="1.5" fill="#0f172a" opacity="0.7"/>
                <rect x="8" y="22" width="14" height="3" rx="1.5" fill="#0f172a" opacity="0.5"/>
                <circle cx="30" cy="30" r="7" fill="#0f172a"/>
                <path d="M27 30l2 2 4-4" stroke="#eab308" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
              <h3 class="text-white fw-bold mb-0">Evalua<span class="text-amber">Tech</span></h3>
            </div>
            <p class="text-slate-400 small">Innovation Éducative & Professionnelle. Projet SaaS assisté par l'IA pour moderniser l'évaluation.</p>
            <div class="footer-socials mt-4">
              <a href="#" class="social-icon">in</a>
              <a href="#" class="social-icon">tw</a>
              <a href="#" class="social-icon">gh</a>
            </div>
          </div>
          <div class="col-lg-2 offset-lg-1">
            <h5 class="text-white fw-bold mb-4">Navigation</h5>
            <ul class="list-unstyled footer-links">
              <li><router-link to="/">Accueil</router-link></li>
              <li><a href="/#services">Services</a></li>
              <li><a href="/#process">Processus</a></li>
              <li><a href="/#about">À propos</a></li>
            </ul>
          </div>
          <div class="col-lg-2">
            <h5 class="text-white fw-bold mb-4">Produit</h5>
            <ul class="list-unstyled footer-links">
              <li><router-link to="/pricing">Tarifs</router-link></li>
              <li><a href="#">Documentation</a></li>
              <li><a href="#">API</a></li>
              <li><a href="#">Changelog</a></li>
            </ul>
          </div>
          <div class="col-lg-3">
            <h5 class="text-white fw-bold mb-4">Contact</h5>
            <p class="text-slate-400 small">contact@evaluatech.tn</p>
            <p class="text-slate-400 small">Plateforme SaaS - PFE 2026</p>
            <div class="footer-newsletter mt-3">
              <input type="email" placeholder="Votre email..." class="newsletter-input">
              <button class="newsletter-btn">→</button>
            </div>
          </div>
        </div>
        <div class="footer-bottom mt-5 pt-4 border-top border-slate-700 d-flex flex-wrap justify-content-between align-items-center">
          <p class="text-slate-500 small mb-0">© 2026 EvaluaTech IA. Tous droits réservés.</p>
          <div class="d-flex gap-4">
            <a href="#" class="text-slate-500 small text-decoration-none">Confidentialité</a>
            <a href="#" class="text-slate-500 small text-decoration-none">CGU</a>
          </div>
        </div>
      </div>
    </footer>

  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';

const authStore = useAuthStore();
const router = useRouter();
const isRedirecting = ref(false);
const openFaq = ref(null);

const toggleFaq = (i) => {
  openFaq.value = openFaq.value === i ? null : i;
};

const renderCell = (val) => {
  if (val === true) return '<span class="check-yes"><i class="fa-solid fa-check"></i></span>';
  if (val === false) return '<span class="check-no"><i class="fa-solid fa-minus"></i></span>';
  return `<span class="check-text">${val}</span>`;
};

const comparisonRows = {
  ia: [
    { label: 'Génération de questions IA', starter: 'Limitée', go: 'Illimitée', enterprise: 'Illimitée' },
    { label: 'Banque de tests complète', starter: true, go: true, enterprise: true },
    { label: 'Tests adaptatifs IA', starter: false, go: true, enterprise: true },
    { label: 'Analyses prédictives RH', starter: false, go: true, enterprise: true },
    { label: 'Rapports PDF automatiques', starter: true, go: true, enterprise: true },
  ],
  security: [
    { label: 'Proctoring de base', starter: true, go: true, enterprise: true },
    { label: 'Proctoring IA avancé', starter: false, go: true, enterprise: true },
    { label: 'Détection comportementale', starter: false, go: true, enterprise: true },
    { label: 'Infrastructure privée', starter: false, go: false, enterprise: true },
  ],
  support: [
    { label: 'Support email', starter: true, go: true, enterprise: true },
    { label: 'Support prioritaire', starter: false, go: true, enterprise: true },
    { label: 'Support dédié 24/7', starter: false, go: false, enterprise: true },
    { label: 'API & Webhooks', starter: false, go: 'Limité', enterprise: 'Complet' },
    { label: 'SLA garanti', starter: false, go: false, enterprise: true },
    { label: 'Onboarding personnalisé', starter: false, go: false, enterprise: true },
  ],
};

const testimonials = [
  {
    quote: "Depuis qu'on utilise EvaluaTech, le temps de screening technique a été divisé par trois. L'IA génère des questions qu'on n'aurait pas imaginées.",
    name: "Ahmed B.", role: "CTO @ TechMakers", color: "#6366f1"
  },
  {
    quote: "La fonction de Proctoring nous rassure sur l'authenticité des résultats, surtout pour le recrutement de développeurs en full remote.",
    name: "Sarah Y.", role: "HR Manager @ Innovate", color: "#eab308"
  },
  {
    quote: "Les rapports détaillés permettent de comprendre non seulement si le code fonctionne, mais aussi la logique derrière. Un outil indispensable.",
    name: "Karim D.", role: "Lead Developer", color: "#14b8a6"
  },
];

const faqs = [
  {
    q: "La version gratuite est-elle vraiment gratuite ?",
    a: "Oui, complètement. Vous accédez à toutes les fonctionnalités IA sans carte bancaire. Les limites temporaires intelligentes se réinitialisent automatiquement selon votre usage."
  },
  {
    q: "Que se passe-t-il à la fin de mes 30 jours EvaluaTech Go ?",
    a: "Votre compte passe automatiquement au plan Starter gratuit. Aucun prélèvement automatique sans votre accord. Vous gardez accès à toutes vos données."
  },
  {
    q: "Le paiement est-il sécurisé ?",
    a: "Absolument. Nous utilisons Stripe, la référence mondiale du paiement en ligne. Vos données bancaires ne transitent jamais par nos serveurs."
  },
  {
    q: "Puis-je annuler à tout moment ?",
    a: "Oui, sans engagement ni pénalité. Annulez depuis votre tableau de bord en un clic, et votre accès premium reste actif jusqu'à la fin de la période payée."
  },
  {
    q: "L'offre Enterprise est-elle adaptée aux PME ?",
    a: "L'Enterprise est conçu pour les organisations avec des besoins spécifiques : volume élevé, infrastructure privée, API dédiée ou contrat SLA. Contactez-nous pour un devis."
  },
  {
    q: "Proposez-vous des réductions pour les établissements éducatifs ?",
    a: "Oui ! Nous offrons des tarifs spéciaux pour les universités, écoles et centres de formation. Contactez-nous avec votre justificatif institutionnel."
  },
];

const handleSubscription = async (planName, price) => {
  if (authStore.isAuthenticated) {
    try {
      isRedirecting.value = true;
      Swal.fire({
        title: 'Préparation du paiement...',
        html: 'Nous vous redirigeons vers Stripe en toute sécurité.',
        allowOutsideClick: false,
        didOpen: () => Swal.showLoading()
      });
      const response = await api.post('/Payments/create-checkout-session', {
        planName,
        price
      });
      if (response.data && response.data.url) {
        window.location.href = response.data.url;
      } else {
        throw new Error('URL de redirection Stripe introuvable');
      }
    } catch (err) {
      console.error('Erreur Stripe :', err);
      Swal.fire('Erreur', 'Impossible d\'initier le paiement. Veuillez réessayer.', 'error');
    } finally {
      isRedirecting.value = false;
    }
  } else {
    localStorage.setItem('pending_plan', JSON.stringify({ name: planName, price }));
    router.push({ path: '/register', query: { plan: planName } });
  }
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;1,700&display=swap');

/* ===== BASE ===== */
*, *::before, *::after { box-sizing: border-box; }
.pricing-wrapper {
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  background: #ffffff;
  overflow-x: hidden;
  position: relative;
}

/* ===== TOKENS (identiques Home.vue) ===== */
.text-amber { color: #eab308 !important; }
.text-slate-400 { color: #94a3b8; }
.text-slate-500 { color: #64748b; }
.shadow-amber { box-shadow: 0 8px 24px rgba(234,179,8,0.28) !important; }

/* ===== BACKGROUND (identique Home.vue) ===== */
.hero-bg-grid {
  position: fixed; inset: 0; pointer-events: none; z-index: 0;
  background-image:
    linear-gradient(rgba(234,179,8,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(234,179,8,0.04) 1px, transparent 1px);
  background-size: 60px 60px;
}
.hero-orb {
  position: fixed; border-radius: 50%;
  filter: blur(80px); pointer-events: none; z-index: 0;
}
.hero-orb-1 { width: 600px; height: 600px; background: rgba(234,179,8,0.10); top: -200px; right: -150px; animation: orb-float 8s ease-in-out infinite; }
.hero-orb-2 { width: 400px; height: 400px; background: rgba(234,179,8,0.06); bottom: 10%; left: -100px; animation: orb-float 10s ease-in-out infinite reverse; }
@keyframes orb-float { 0%,100%{transform:translate(0,0)} 50%{transform:translate(30px,-30px)} }

/* ===== NAVBAR (identique Home.vue) ===== */
.header-nav {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(241,245,249,0.9);
  z-index: 1100;
  box-shadow: 0 4px 30px rgba(0,0,0,0.05);
}
.main-navigation { height: 72px; display: flex; align-items: center; justify-content: space-between; }
.brand { text-decoration: none; display: flex; align-items: center; gap: 10px; }
.brand-name { font-size: 21px; font-weight: 800; color: #0f172a; letter-spacing: -0.5px; }
.nav-links { list-style: none; margin: 0; padding: 0; display: flex; gap: 8px; }
.nav-links a, .nav-links .router-link-active {
  text-decoration: none; color: #64748b; font-weight: 600; font-size: 14px;
  padding: 8px 14px; border-radius: 10px; transition: 0.2s; display: block;
}
.nav-links a:hover, .nav-links a.active { color: #0f172a; background: #fefce8; }
.btn-text { text-decoration: none; color: #64748b; font-weight: 600; font-size: 14px; margin-right: 4px; transition: 0.2s; }
.btn-text:hover { color: #eab308; }
.btn-amber-pill {
  background: #eab308; color: #0f172a; padding: 10px 22px; border-radius: 50px;
  text-decoration: none; font-weight: 800; font-size: 14px;
  display: inline-flex; align-items: center; transition: 0.3s;
}
.btn-amber-pill:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(234,179,8,0.4); color: #0f172a; }

/* ===== PRICING HERO ===== */
.pricing-hero {
  padding: 120px 0 60px;
  position: relative; z-index: 2;
  text-align: center;
}
.pricing-hero-text { position: relative; z-index: 3; }

/* Robot mascot */
.pricing-robot-mascot { position: absolute; top: 80px; right: 3%; z-index: 5; }
.robot-float { animation: floatRobot 4s ease-in-out infinite; text-align: center; }
@keyframes floatRobot { 0%,100%{transform:translateY(0)} 50%{transform:translateY(-14px)} }
.robot-bubble {
  background: white; padding: 10px 18px; border-radius: 20px;
  font-size: 13px; font-weight: 700; color: #0f172a; margin-top: 10px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.07); border: 1px solid #fef3c7;
}

/* Hero badge & text (identiques Home.vue) */
.hero-badge {
  display: inline-flex; align-items: center; gap: 8px;
  background: #fefce8; color: #92400e;
  padding: 7px 16px; border-radius: 100px; font-weight: 700; font-size: 12px;
  border: 1px solid #fde68a; text-transform: uppercase; letter-spacing: 0.8px;
}
.badge-dot { width: 7px; height: 7px; background: #eab308; border-radius: 50%; animation: pulse-dot 2s ease-in-out infinite; }
@keyframes pulse-dot { 0%,100%{transform:scale(1);opacity:1} 50%{transform:scale(1.5);opacity:0.7} }
.hero-eyebrow {
  font-size: 12px; font-weight: 700; color: #eab308;
  text-transform: uppercase; letter-spacing: 2px; margin-top: 16px; margin-bottom: 14px;
}
.hero-title {
  font-size: 58px; font-weight: 800; letter-spacing: -3px; line-height: 1.08;
  color: #0f172a; margin-bottom: 22px;
}
.hero-highlight { color: #eab308; font-style: italic; }
.hero-desc { font-size: 16px; color: #475569; line-height: 1.75; margin-bottom: 40px; }

/* Billing toggle */
.billing-toggle {
  display: inline-flex; align-items: center; gap: 12px;
  background: white; border: 1px solid #e2e8f0; border-radius: 100px;
  padding: 8px 20px; margin-top: 10px; position: relative; z-index: 3;
}
.billing-label { font-size: 14px; font-weight: 700; color: #94a3b8; transition: 0.2s; }
.billing-label.active { color: #0f172a; }
.toggle-switch {
  width: 44px; height: 24px; background: #f1f5f9; border-radius: 100px;
  position: relative; cursor: pointer;
}
.toggle-thumb {
  width: 18px; height: 18px; background: #eab308; border-radius: 50%;
  position: absolute; top: 3px; left: 3px; transition: 0.3s;
}
.billing-save {
  background: #fef9c3; color: #a16207; border: 1px solid #fde68a;
  padding: 2px 10px; border-radius: 100px; font-size: 11px; font-weight: 800;
}

/* ===== PRICING GRID ===== */
.pricing-cards-section {
  padding: 20px 0 80px;
  position: relative; z-index: 2;
}
.pricing-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 24px;
  align-items: start;
}

/* ===== PRICING CARD ===== */
.pricing-card {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(20px);
  border: 1px solid #e8edf5;
  border-radius: 28px;
  padding: 36px 32px 32px;
  display: flex; flex-direction: column;
  position: relative;
  transition: transform 0.35s cubic-bezier(0.175,0.885,0.32,1.275), box-shadow 0.35s;
  box-shadow: 0 4px 24px rgba(15,23,42,0.05);
}
.pricing-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 24px 60px rgba(15,23,42,0.1);
}

/* Featured card */
.pricing-card-featured {
  border: 2px solid #eab308;
  background: white;
  transform: translateY(-12px);
  box-shadow: 0 20px 60px rgba(234,179,8,0.18);
  z-index: 3;
}
.pricing-card-featured:hover {
  transform: translateY(-20px);
  box-shadow: 0 32px 80px rgba(234,179,8,0.22);
}

/* Featured ribbon */
.featured-ribbon {
  position: absolute; top: -14px; left: 50%; transform: translateX(-50%);
  background: #eab308; color: #0f172a; padding: 6px 20px;
  border-radius: 100px; font-size: 11px; font-weight: 800;
  text-transform: uppercase; letter-spacing: 0.8px; white-space: nowrap;
}

/* Card top */
.card-top { display: flex; align-items: center; gap: 12px; margin-bottom: 20px; }
.plan-icon {
  width: 48px; height: 48px; border-radius: 14px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center;
}
.plan-icon-slate { background: #f1f5f9; color: #64748b; }
.plan-icon-amber { background: #fef9c3; color: #a16207; }
.plan-icon-navy { background: #e0f2fe; color: #0369a1; }

/* Plan badges */
.plan-badge {
  display: inline-block; padding: 4px 12px; border-radius: 8px;
  font-size: 11px; font-weight: 800; text-transform: uppercase; letter-spacing: 1px;
}
.starter-badge { background: #f1f5f9; color: #64748b; }
.business-badge { background: #fffbeb; color: #b45309; }
.enterprise-badge { background: #e0f2fe; color: #0369a1; }

/* Plan names */
.plan-name { font-size: 28px; font-weight: 800; color: #0f172a; margin: 0 0 4px; letter-spacing: -0.5px; }
.plan-name-amber { color: #a16207; }
.plan-tagline { font-size: 13px; color: #94a3b8; font-weight: 600; margin: 0 0 20px; }

/* Price display */
.price-display { display: flex; align-items: baseline; gap: 4px; margin-bottom: 8px; }
.price-amount { font-size: 58px; font-weight: 900; color: #0f172a; line-height: 1; letter-spacing: -3px; }
.price-amount-amber { color: #eab308; }
.price-meta { display: flex; flex-direction: column; gap: 2px; padding-bottom: 6px; }
.price-currency { font-size: 20px; font-weight: 800; color: #0f172a; }
.price-currency-amber { color: #eab308; }
.price-period { font-size: 13px; color: #94a3b8; font-weight: 600; }
.price-custom { font-size: 40px; font-weight: 900; color: #0f172a; letter-spacing: -1px; }
.price-note { font-size: 12px; color: #94a3b8; font-weight: 600; margin-bottom: 24px; }
.price-note-amber { color: #b45309; }

/* Divider */
.features-divider { height: 1px; background: #f1f5f9; margin-bottom: 24px; }
.features-divider-amber { background: linear-gradient(90deg, #fde68a, transparent); }

/* Features list */
.features-list { list-style: none; padding: 0; margin: 0 0 28px; flex: 1; }
.feature-item {
  display: flex; align-items: center; gap: 12px;
  padding: 9px 0; font-size: 14px; font-weight: 600; color: #334155;
  border-bottom: 1px solid #f8fafc;
}
.feature-item:last-child { border-bottom: none; }
.feature-highlight {
  background: rgba(251,191,36,0.05); margin: 0 -12px;
  padding: 9px 12px; border-radius: 10px; border-bottom: none;
}
.feature-icon {
  width: 28px; height: 28px; border-radius: 8px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center; font-size: 12px;
}
.feature-icon-amber { background: #fef9c3; color: #a16207; }
.feature-icon-green { background: #dcfce7; color: #16a34a; }
.feature-icon-navy { background: #e0f2fe; color: #0369a1; }

/* Buttons */
.btn-plan {
  width: 100%; padding: 15px 24px; border-radius: 14px;
  font-weight: 800; font-size: 15px; transition: all 0.3s;
  display: flex; align-items: center; justify-content: center; gap: 8px;
  border: none; cursor: pointer; text-decoration: none;
}
.btn-plan-outline {
  background: transparent; border: 2px solid #e2e8f0; color: #64748b;
}
.btn-plan-outline:hover { border-color: #0f172a; color: #0f172a; background: #f8fafc; }
.btn-plan-amber {
  background: #eab308; color: #0f172a;
}
.btn-plan-amber:hover { background: #0f172a; color: white; transform: scale(1.02); }
.btn-plan-navy {
  background: #0f172a; color: white;
}
.btn-plan-navy:hover { background: #eab308; color: #0f172a; transform: scale(1.02); }

.plan-guarantee {
  text-align: center; font-size: 12px; color: #94a3b8; font-weight: 600;
  margin: 12px 0 0;
}

/* ===== ANIMATE UP ===== */
@keyframes animateUp {
  from { opacity: 0; transform: translateY(40px); }
  to { opacity: 1; transform: translateY(0); }
}
.animate-up { animation: animateUp 0.7s ease forwards; }

/* ===== COMPARISON TABLE ===== */
.comparison-section { padding: 80px 0; background: #f8fafc; position: relative; z-index: 2; }
.comparison-table-wrap { overflow-x: auto; border-radius: 20px; border: 1px solid #e8edf5; background: white; }
.comparison-table { width: 100%; border-collapse: collapse; font-size: 14px; }
.comparison-table th {
  padding: 18px 24px; font-weight: 800; font-size: 13px;
  color: #64748b; text-transform: uppercase; letter-spacing: 0.8px;
  border-bottom: 2px solid #f1f5f9; background: #f8fafc;
}
.comparison-table .feature-col { text-align: left; width: 35%; }
.comparison-table td { padding: 14px 24px; border-bottom: 1px solid #f8fafc; color: #334155; font-weight: 600; }
.comparison-table tr:last-child td { border-bottom: none; }
.comparison-table tr:hover td { background: #fefce8; }
.plan-col-featured {
  background: rgba(234,179,8,0.04);
  border-left: 2px solid rgba(234,179,8,0.3);
  border-right: 2px solid rgba(234,179,8,0.3);
}
.table-group-row td {
  background: #f1f5f9; font-weight: 800; font-size: 12px;
  text-transform: uppercase; letter-spacing: 1px; color: #64748b;
  padding: 10px 24px;
}
.check-yes { color: #16a34a; font-size: 16px; }
.check-no { color: #d1d5db; font-size: 14px; }
.check-text { color: #b45309; font-size: 13px; font-weight: 700; background: #fffbeb; padding: 2px 10px; border-radius: 8px; border: 1px solid #fde68a; }

/* ===== TESTIMONIALS (identique Home.vue) ===== */
.testimonials-section { padding: 80px 0; background: white; position: relative; z-index: 2; }
.testimonial-card {
  padding: 36px 30px; background: #f8fafc; border: 1px solid #f1f5f9;
  border-radius: 24px; transition: 0.3s; position: relative; overflow: hidden;
}
.testimonial-featured { border-color: #eab308; background: #fffbeb; }
.testimonial-featured::before { content: ''; position: absolute; top: 0; left: 0; right: 0; height: 4px; background: #eab308; }
.testimonial-card:hover { box-shadow: 0 20px 40px rgba(0,0,0,0.06); transform: translateY(-4px); }
.quote-mark { font-size: 72px; font-weight: 800; color: #fde68a; line-height: 0.5; height: 30px; overflow: visible; }
.avatar-placeholder { width: 48px; height: 48px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 18px; color: white; }
.text-amber { color: #eab308 !important; }

/* ===== FAQ ===== */
.faq-section { padding: 80px 0; background: #f8fafc; position: relative; z-index: 2; }
.faq-grid { max-width: 820px; margin: 0 auto; display: flex; flex-direction: column; gap: 12px; }
.faq-item {
  background: white; border: 1px solid #e8edf5; border-radius: 16px;
  overflow: hidden; transition: 0.3s; cursor: pointer;
}
.faq-item:hover { border-color: #fde68a; box-shadow: 0 8px 24px rgba(0,0,0,0.05); }
.faq-item.open { border-color: #eab308; }
.faq-question {
  display: flex; align-items: center; justify-content: space-between;
  padding: 20px 24px; font-weight: 700; font-size: 15px; color: #0f172a;
  gap: 16px;
}
.faq-arrow { transition: transform 0.3s; flex-shrink: 0; color: #94a3b8; }
.faq-item.open .faq-arrow { transform: rotate(180deg); color: #eab308; }
.faq-answer {
  max-height: 0; overflow: hidden;
  transition: max-height 0.4s cubic-bezier(0.4,0,0.2,1);
}
.faq-answer p {
  padding: 0 24px 20px; font-size: 14px; color: #64748b;
  line-height: 1.7; margin: 0;
}

/* ===== SECTION HEADINGS (identique Home.vue) ===== */
.section-tag {
  display: inline-block; background: #fefce8; color: #a16207;
  padding: 6px 16px; border-radius: 100px; font-weight: 800;
  font-size: 12px; text-transform: uppercase; letter-spacing: 1.5px;
  border: 1px solid #fde68a;
}
.section-tag.light { background: rgba(255,255,255,0.15); color: white; border-color: rgba(255,255,255,0.3); }
.section-title { font-size: 44px; font-weight: 800; letter-spacing: -1.5px; color: #0f172a; }
.h-line { width: 60px; height: 4px; background: #eab308; margin: 20px auto 0; border-radius: 10px; }

/* ===== CTA FINAL (identique Home.vue) ===== */
.final-cta { padding: 60px 0; position: relative; z-index: 2; }
.cta-box {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
  padding: 80px 40px; border-radius: 32px;
  border: 1px solid rgba(234,179,8,0.2);
}
.cta-particles { position: absolute; inset: 0; overflow: hidden; }
.cta-p {
  position: absolute; width: 4px; height: 4px; background: #eab308;
  border-radius: 50%; opacity: 0; animation: cta-particle 4s ease-in-out infinite;
}
.cta-p:nth-child(1){left:10%;top:20%;animation-delay:0s}
.cta-p:nth-child(2){left:20%;top:80%;animation-delay:.5s}
.cta-p:nth-child(3){left:30%;top:40%;animation-delay:1s}
.cta-p:nth-child(4){left:40%;top:70%;animation-delay:1.5s}
.cta-p:nth-child(5){left:50%;top:10%;animation-delay:2s}
.cta-p:nth-child(6){left:60%;top:60%;animation-delay:2.5s}
.cta-p:nth-child(7){left:70%;top:30%;animation-delay:3s}
.cta-p:nth-child(8){left:80%;top:85%;animation-delay:3.5s}
.cta-p:nth-child(9){left:90%;top:50%;animation-delay:.8s}
.cta-p:nth-child(10){left:5%;top:60%;animation-delay:1.3s}
.cta-p:nth-child(11){left:95%;top:25%;animation-delay:1.8s}
.cta-p:nth-child(12){left:45%;top:90%;animation-delay:2.3s}
@keyframes cta-particle {
  0%,100%{opacity:0;transform:translateY(0) scale(0)}
  50%{opacity:1;transform:translateY(-30px) scale(1)}
}
.cta-circle-1,.cta-circle-2,.cta-circle-3 { position:absolute; border-radius:50%; border:1px solid rgba(234,179,8,0.15); }
.cta-circle-1 { width:400px;height:400px;top:-150px;right:-100px;animation:rotate-slow 20s linear infinite; }
.cta-circle-2 { width:250px;height:250px;bottom:-100px;left:5%;animation:rotate-slow 15s linear infinite reverse; }
.cta-circle-3 { width:150px;height:150px;top:20%;left:40%;animation:rotate-slow 10s linear infinite; }
@keyframes rotate-slow { to{transform:rotate(360deg)} }

/* ===== FOOTER (identique Home.vue) ===== */
.footer-dark { background: #0f172a; border-radius: 40px 40px 0 0; margin-top: 40px; position: relative; z-index: 2; }
.footer-links a, .footer-links .router-link-active {
  color: #94a3b8; text-decoration: none; transition: 0.2s; display: block;
  line-height: 2.5; font-weight: 600; font-size: 14px;
}
.footer-links a:hover { color: #eab308; padding-left: 6px; }
.footer-socials { display: flex; gap: 10px; }
.social-icon {
  width: 38px; height: 38px; border-radius: 10px; background: rgba(255,255,255,0.06);
  color: #94a3b8; font-size: 12px; font-weight: 800;
  display: flex; align-items: center; justify-content: center; text-decoration: none;
  transition: 0.2s; border: 1px solid rgba(255,255,255,0.08);
}
.social-icon:hover { background: #eab308; color: #0f172a; border-color: #eab308; }
.newsletter-input {
  background: rgba(255,255,255,0.06); border: 1px solid rgba(255,255,255,0.1);
  color: white; padding: 10px 16px; border-radius: 12px 0 0 12px;
  font-size: 14px; outline: none; width: calc(100% - 48px);
}
.newsletter-input::placeholder { color: #64748b; }
.newsletter-btn {
  width: 48px; background: #eab308; border: none; color: #0f172a;
  font-size: 20px; font-weight: bold; border-radius: 0 12px 12px 0; cursor: pointer; transition: 0.2s;
}
.newsletter-btn:hover { background: #d97706; }
.footer-newsletter { display: flex; }
.border-slate-700 { border-color: rgba(255,255,255,0.08) !important; }

/* ===== RESPONSIVE ===== */
@media (max-width: 1199px) {
  .pricing-grid { grid-template-columns: 1fr 1fr; }
  .pricing-card-featured { transform: none; }
  .pricing-card-featured:hover { transform: translateY(-8px); }
}
@media (max-width: 767px) {
  .pricing-grid { grid-template-columns: 1fr; max-width: 460px; margin: 0 auto; }
  .hero-title { font-size: 38px; letter-spacing: -2px; }
  .section-title { font-size: 30px; }
  .pricing-card-featured { transform: none; }
  .footer-dark { border-radius: 24px 24px 0 0; }
  .cta-box { padding: 50px 24px; }
  .comparison-table th, .comparison-table td { padding: 12px 14px; font-size: 13px; }
}
@media (max-width: 575px) {
  .pricing-hero { padding: 100px 0 40px; }
  .hero-title { font-size: 30px; }
  .billing-toggle { flex-wrap: wrap; justify-content: center; }
}
</style>