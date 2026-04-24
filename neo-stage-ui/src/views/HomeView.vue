<template>
  <div class="landing-page">

    <!-- 1. PRELOADER -->
    <div id="js-preloader" class="js-preloader" v-if="loading">
      <div class="preloader-inner">
        <div class="loader-ring">
          <div></div><div></div><div></div><div></div>
        </div>
        <div class="loader-brand">
          <svg width="36" height="36" viewBox="0 0 36 36" fill="none">
            <rect width="36" height="36" rx="10" fill="#eab308"/>
            <path d="M8 10h20M8 18h14M8 26h17" stroke="#fff" stroke-width="3" stroke-linecap="round"/>
            <circle cx="26" cy="26" r="4" fill="#0f172a"/>
          </svg>
          <span>EVALUA<b>TECH</b></span>
        </div>
      </div>
    </div>

    <!-- 2. NAVBAR -->
    <header class="header-nav sticky-top" :class="{ 'scrolled': scrolled }">
      <div class="container">
        <nav class="main-navigation">
          <a href="#top" class="brand">
            <div class="brand-logo-wrap">
              <svg width="38" height="38" viewBox="0 0 38 38" fill="none">
                <rect width="38" height="38" rx="11" fill="#0f172a"/>
                <path d="M9 11h20M9 19h15M9 27h18" stroke="#eab308" stroke-width="2.8" stroke-linecap="round"/>
                <circle cx="27" cy="27" r="4.5" fill="#eab308"/>
                <circle cx="27" cy="27" r="2" fill="#0f172a"/>
              </svg>
            </div>
            <span class="brand-name">Evalua<span class="text-amber">Tech</span></span>
          </a>

          <ul class="nav-links d-none d-lg-flex">
            <li><a href="#top" :class="{ active: activeSection === 'top' }">Accueil</a></li>
            <li><a href="#services" :class="{ active: activeSection === 'services' }">Services</a></li>
            <li><a href="#process" :class="{ active: activeSection === 'process' }">Processus</a></li>
            <li><a href="#about" :class="{ active: activeSection === 'about' }">À propos</a></li>
            <li><a href="#pricing">Tarifs</a></li>
          </ul>

          <div class="nav-auth">
            <a href="/login" class="btn-text d-none d-sm-inline">Connexion</a>
            <a href="/login" class="btn-amber-pill shadow-amber">
              Essai Gratuit
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="ms-1"><path d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"/></svg>
            </a>
          </div>
        </nav>
      </div>
    </header>

    <!-- 3. HERO SECTION -->
    <section id="top" class="hero-section">
      <div class="hero-bg-grid"></div>
      <div class="hero-orb hero-orb-1"></div>
      <div class="hero-orb hero-orb-2"></div>
      <div class="container position-relative">
        <div class="row align-items-center">
          <div class="col-lg-6 hero-text-wrapper" :class="{ 'hero-visible': heroVisible }">
            <div class="hero-badge">
              <span class="badge-dot"></span>
              Plateforme IA · PFE 2026
            </div>
            <h1 class="hero-title">
              Révolutionnez<br>
              vos <span class="hero-highlight">évaluations<br>avec l'IA</span>
            </h1>
            <p class="hero-desc">
              EvaluaTech est une plateforme SaaS d'évaluation technique et comportementale.
              Automatisez la création de tests et analysez les compétences en temps réel.
            </p>
            <div class="hero-btns mt-4 d-flex align-items-center gap-3">
              <a href="/login" class="btn-amber-lg shadow-amber">
                Démarrer maintenant
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
              </a>
              <a href="#services" class="btn-outline-custom">
                Découvrir
              </a>
            </div>
            <div class="hero-trust mt-5">
              <div class="trust-avatars">
                <div class="ta" style="background:#6366f1">A</div>
                <div class="ta" style="background:#8b5cf6">B</div>
                <div class="ta" style="background:#ec4899">C</div>
                <div class="ta" style="background:#14b8a6">D</div>
              </div>
              <span class="trust-text"><strong>500+</strong> entreprises nous font confiance</span>
            </div>
          </div>

          <div class="col-lg-6 hero-img-wrapper mt-5 mt-lg-0" :class="{ 'hero-visible': heroVisible }">
            <div class="right-image position-relative">
              <div class="hero-mockup animate-float">
                <div class="mock-header">
                  <span class="m-dot red"></span>
                  <span class="m-dot yellow"></span>
                  <span class="m-dot green"></span>
                  <span class="mock-title">Rapport IA – Candidat #042</span>
                </div>
                <div class="mock-body">
                  <div class="score-row">
                    <div class="score-circle">
                      <svg viewBox="0 0 120 120" class="score-svg">
                        <circle cx="60" cy="60" r="50" fill="none" stroke="#f1f5f9" stroke-width="12"/>
                        <circle cx="60" cy="60" r="50" fill="none" stroke="#eab308" stroke-width="12"
                          stroke-dasharray="264" stroke-dashoffset="38" stroke-linecap="round"
                          style="transform:rotate(-90deg);transform-origin:60px 60px"/>
                      </svg>
                      <div class="score-text">84.2<span>Score IA</span></div>
                    </div>
                    <div class="score-details">
                      <div class="skill-bar-item" v-for="(skill, i) in mockSkills" :key="i">
                        <div class="d-flex justify-content-between mb-1">
                          <small class="fw-bold text-slate-700">{{ skill.name }}</small>
                          <small class="text-amber fw-bold">{{ skill.val }}%</small>
                        </div>
                        <div class="skill-track">
                          <div class="skill-fill" :style="{ width: skill.val + '%', animationDelay: (i * 0.2) + 's' }"></div>
                        </div>
                      </div>
                    </div>
                  </div>
                  <div class="mock-tags mt-3">
                    <span class="mock-tag green">✓ React Expert</span>
                    <span class="mock-tag blue">✓ Node.js</span>
                    <span class="mock-tag amber">⚠ DevOps</span>
                  </div>
                </div>
              </div>
              <!-- Floating badges -->
              <div class="float-badge fb1 animate-float-slow">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2.5"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>
                <span>Proctoring Actif</span>
              </div>
              <div class="float-badge fb2 animate-float-delayed">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="#22c55e" stroke-width="2.5"><polyline points="20 6 9 17 4 12"/></svg>
                <span>+147 Tests aujourd'hui</span>
              </div>
              <div class="hero-glow-effect"></div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 4. STATS BAR -->
    <section class="stats-bar">
      <div class="container">
        <div class="stats-inner">
          <div class="stat-item" v-for="(stat, i) in statsData" :key="i">
            <div class="stat-icon" v-html="stat.icon"></div>
            <div>
              <h3 class="stat-val">{{ stat.val }}</h3>
              <p class="stat-label">{{ stat.label }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 5. SERVICES -->
    <section id="services" class="services-section py-5">
      <div class="container py-5">
        <div class="section-heading text-center mb-5">
          <span class="section-tag">Nos Fonctionnalités</span>
          <h2 class="section-title mt-3">Pourquoi choisir <span class="text-amber">EvaluaTech</span> ?</h2>
          <div class="h-line"></div>
        </div>
        <div class="row g-4">
          <div class="col-lg-3 col-md-6" v-for="(s, i) in serviceList" :key="i">
            <div class="service-card h-100" :style="{ animationDelay: (i * 0.1) + 's' }">
              <div class="icon-circle" v-html="s.svg"></div>
              <h4>{{ s.title }}</h4>
              <p>{{ s.desc }}</p>
              <div class="card-arrow">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M5 12h14m-7-7l7 7-7 7"/></svg>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 6. PROCESS / COMMENCER EST FACILE -->
    <section id="process" class="process-section py-5 bg-white">
      <div class="container py-5">
        <div class="section-heading text-center mb-2">
          <span class="section-tag">Comment ça marche</span>
          <h2 class="section-title mt-3 text-slate-900">Commencer est <span class="text-amber">facile</span></h2>
          <p class="text-muted mt-2">Suivez ces étapes simples pour un meilleur recrutement</p>
          <div class="h-line"></div>
        </div>

        <!-- Timeline connector line -->
        <div class="process-connector d-none d-lg-block"></div>

        <div class="row g-4 mt-4 justify-content-center">
          <div class="col-lg-4 col-md-6" v-for="(step, i) in processSteps" :key="i">
            <div class="process-card" :class="{ 'process-card-featured': step.featured }">
              <div class="step-number-wrap">
                <div class="step-number" :class="{ 'step-amber': step.featured }">{{ i + 1 }}</div>
                <div class="step-pulse" v-if="step.featured"></div>
              </div>
              <div class="process-icon" v-html="step.icon"></div>
              <h5 class="fw-bold mt-3 text-slate-800">{{ step.title }}</h5>
              <p class="text-muted small mt-2 line-height-relaxed">{{ step.desc }}</p>
              <div class="process-badge" v-if="step.badge">{{ step.badge }}</div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 7. ABOUT US -->
    <section id="about" class="about-section py-5 bg-light-soft">
      <div class="container">
        <div class="row align-items-center g-5 py-5">
          <div class="col-lg-6">
            <span class="section-tag">À propos de nous</span>
            <h2 class="fw-bold mt-3 display-5 text-slate-900">Digitaliser l'évaluation avec <span class="text-amber italic">précision</span></h2>
            <p class="mt-4 text-muted">Notre mission est de digitaliser le cycle de l'évaluation en offrant une traçabilité totale et une analyse objective des compétences.</p>
            <ul class="list-unstyled mt-4">
              <li class="about-li mb-3" v-for="(li, i) in aboutList" :key="i">
                <div class="about-check">
                  <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="3" stroke-linecap="round"><polyline points="20 6 9 17 4 12"/></svg>
                </div>
                <span class="text-slate-700 fw-medium">{{ li }}</span>
              </li>
            </ul>
            <a href="#pricing" class="btn-amber mt-4 d-inline-block">Découvrir nos solutions</a>
          </div>
          <div class="col-lg-6 text-center">
            <div class="about-img-box position-relative">
              <img src="https://cdn.dribbble.com/userupload/24665857/file/original-e4c52a6c2e61f45045a31c79b844a84e.gif" alt="Stats IA" class="img-fluid rounded-4 shadow-lg about-img">
              <div class="floating-badge shadow-lg animate-float-delayed">
                <div class="d-flex align-items-center gap-3">
                  <div class="icon-bg-amber">
                    <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="2"><path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>
                  </div>
                  <div class="text-start">
                    <div class="fw-bold fs-5 text-dark">-40%</div>
                    <div class="small text-muted">Coûts de recrutement</div>
                  </div>
                </div>
              </div>
              <div class="floating-badge-2 shadow-lg animate-float">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="#22c55e" stroke-width="2.5"><polyline points="22 7 13.5 15.5 8.5 10.5 2 17"/><polyline points="16 7 22 7 22 13"/></svg>
                <span class="ms-2 small fw-bold text-slate-700">+23% précision ce mois</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 8. TÉMOIGNAGES -->
    <section class="testimonials py-5">
      <div class="container py-5">
        <div class="section-heading text-center mb-5">
          <span class="section-tag">Témoignages</span>
          <h2 class="section-title mt-3">Ce qu'ils disent de nous</h2>
          <div class="h-line"></div>
        </div>
        <div class="row g-4">
          <div class="col-lg-4" v-for="(t, i) in testimonials" :key="i">
            <div class="testimonial-card h-100" :class="{ 'testimonial-featured': i === 1 }">
              <div class="d-flex text-amber mb-3 gap-1">
                <svg v-for="s in 5" :key="s" width="17" height="17" viewBox="0 0 24 24" fill="currentColor"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
              </div>
              <div class="quote-mark">"</div>
              <p class="fst-italic text-slate-700 mt-2">{{ t.quote }}</p>
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

    <!-- 9. CTA FINAL BANNER -->
    <section class="final-cta py-5">
      <div class="container">
        <div class="cta-box text-center position-relative overflow-hidden">
          <div class="cta-particles">
            <div class="cta-p" v-for="n in 12" :key="n"></div>
          </div>
          <div class="position-relative" style="z-index:10">
            <span class="section-tag light mb-3 d-inline-block">Prêt à commencer ?</span>
            <h2 class="fw-bold text-white mb-3 fs-1">Identifiez vos futurs talents<br>avec l'IA</h2>
            <p class="text-white-50 mb-4 max-w-600 mx-auto">Rejoignez plus de 500 entreprises qui utilisent EvaluaTech pour fiabiliser leurs recrutements techniques.</p>
            <div class="d-flex gap-3 justify-content-center flex-wrap">
              <a href="/login" class="btn btn-light btn-lg rounded-pill px-5 fw-bold text-slate-900">
                Démarrer l'essai gratuit
              </a>
              <a href="#about" class="btn btn-outline-light btn-lg rounded-pill px-5 fw-bold">
                En savoir plus
              </a>
            </div>
          </div>
          <div class="cta-circle-1"></div>
          <div class="cta-circle-2"></div>
          <div class="cta-circle-3"></div>
        </div>
      </div>
    </section>

    <!-- 10. FOOTER -->
    <footer class="footer-dark">
      <div class="container py-5">
        <div class="row gy-4">
          <div class="col-lg-4">
            <div class="d-flex align-items-center gap-2 mb-3">
              <svg width="36" height="36" viewBox="0 0 38 38" fill="none">
                <rect width="38" height="38" rx="11" fill="#eab308"/>
                <path d="M9 11h20M9 19h15M9 27h18" stroke="#0f172a" stroke-width="2.8" stroke-linecap="round"/>
                <circle cx="27" cy="27" r="4.5" fill="#0f172a"/>
                <circle cx="27" cy="27" r="2" fill="#eab308"/>
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
              <li><a href="#top">Accueil</a></li>
              <li><a href="#services">Services</a></li>
              <li><a href="#process">Processus</a></li>
              <li><a href="#about">À propos</a></li>
            </ul>
          </div>
          <div class="col-lg-2">
            <h5 class="text-white fw-bold mb-4">Produit</h5>
            <ul class="list-unstyled footer-links">
              <li><a href="#pricing">Tarifs</a></li>
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
import { ref, onMounted, onUnmounted } from 'vue';

const loading = ref(true);
const scrolled = ref(false);
const heroVisible = ref(false);
const activeSection = ref('top');

const mockSkills = [
  { name: 'Algorithmique', val: 92 },
  { name: 'Communication', val: 78 },
  { name: 'Problem Solving', val: 88 },
];

const statsData = [
  {
    val: "1.2M+", label: "Tests Générés",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 5H7a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V7a2 2 0 0 0-2-2h-2M9 5a2 2 0 0 0 2 2h2a2 2 0 0 0 2-2M9 5a2 2 0 0 1 2-2h2a2 2 0 0 1 2 2"/></svg>`
  },
  {
    val: "98%", label: "Taux de Précision",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>`
  },
  {
    val: "500+", label: "Entreprises Clientes",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/></svg>`
  },
  {
    val: "24/7", label: "Surveillance IA",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/></svg>`
  },
  {
    val: "-40%", label: "Coûts de Recrutement",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg>`
  },
  {
    val: "3x", label: "Gain de Vitesse",
    icon: `<svg width="22" height="22" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="22 7 13.5 15.5 8.5 10.5 2 17"/><polyline points="16 7 22 7 22 13"/></svg>`
  },
];

const serviceList = [
  {
    title: "Banque de Questions",
    desc: "Bibliothèque structurée par thèmes et niveaux gérée par IA.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/></svg>`
  },
  {
    title: "Surveillance IA",
    desc: "Système anti-triche intégré (Proctoring) pour garantir l'équité.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>`
  },
  {
    title: "Rapports Détaillés",
    desc: "Analyses automatiques et recommandations personnalisées.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21.21 15.89A10 10 0 1 1 8 2.83"/><path d="M22 12A10 10 0 0 0 12 2v10z"/></svg>`
  },
  {
    title: "Multi-Entreprises",
    desc: "Architecture SaaS permettant une gestion isolée sécurisée.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/><rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/></svg>`
  },
  {
    title: "Code Evaluation",
    desc: "Exécution de code en temps réel avec sandbox sécurisé pour tous les langages.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/></svg>`
  },
  {
    title: "Analytics Avancés",
    desc: "Tableaux de bord interactifs pour suivre l'évolution des compétences.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/><line x1="6" y1="20" x2="6" y2="14"/></svg>`
  },
  {
    title: "Intégration RH",
    desc: "Compatible avec vos outils RH existants via API et webhooks.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><circle cx="18" cy="18" r="3"/><circle cx="6" cy="6" r="3"/><path d="M13 6h3a2 2 0 0 1 2 2v7M11 18H8a2 2 0 0 1-2-2V9"/></svg>`
  },
  {
    title: "Tests Adaptatifs",
    desc: "IA qui ajuste la difficulté en temps réel selon les réponses du candidat.",
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>`
  },
];

const processSteps = [
  {
    title: "Créez votre évaluation",
    desc: "Définissez les exigences du rôle et laissez EvaliA générer des questions pertinentes, ou créez une évaluation personnalisée avec notre éditeur intuitif.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/></svg>`,
    badge: "IA Générative",
    featured: false
  },
  {
    title: "Configurez le proctoring",
    desc: "Activez la surveillance IA en un clic. Notre système détecte automatiquement les comportements suspects et garantit l'authenticité des résultats.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"/></svg>`,
    badge: "Anti-Triche",
    featured: true
  },
  {
    title: "Invitez les candidats",
    desc: "Envoyez les liens d'évaluation par e-mail. Les candidats complètent les tests selon leur horaire, depuis n'importe quel appareil, en toute simplicité.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><path d="M4 4h16c1.1 0 2 .9 2 2v12c0 1.1-.9 2-2 2H4c-1.1 0-2-.9-2-2V6c0-1.1.9-2 2-2z"/><polyline points="22,6 12,13 2,6"/></svg>`,
    badge: "Multi-Device",
    featured: false
  },
  {
    title: "Analysez en temps réel",
    desc: "Suivez la progression des candidats en direct. Visualisez les métriques clés et recevez des alertes instantanées dès la fin de chaque évaluation.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><polyline points="22 12 18 12 15 21 9 3 6 12 2 12"/></svg>`,
    badge: "Live Dashboard",
    featured: false
  },
  {
    title: "Comparez les profils",
    desc: "Notre IA rank automatiquement les candidats selon vos critères. Comparez les compétences côte à côte et identifiez vos meilleurs profils en un clin d'œil.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>`,
    badge: "IA Ranking",
    featured: false
  },
  {
    title: "Décidez en confiance",
    desc: "Obtenez des rapports de notation détaillés et des analyses prédictives. Prenez vos décisions de recrutement sur des données fiables et objectives.",
    icon: `<svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="2"><path d="M21.21 15.89A10 10 0 1 1 8 2.83"/><path d="M22 12A10 10 0 0 0 12 2v10z"/></svg>`,
    badge: "Rapport PDF",
    featured: false
  },
];

const aboutList = [
  "Gain de temps sur la correction automatique",
  "Bilan des compétences instantané et objectif",
  "Protection contre la fraude (Proctoring IA)",
  "Intégration facile avec vos outils RH",
  "Support 24/7 et onboarding personnalisé",
];

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

const handleScroll = () => {
  scrolled.value = window.scrollY > 50;
  // update active section
  const sections = ['top', 'services', 'process', 'about'];
  for (const id of sections.reverse()) {
    const el = document.getElementById(id);
    if (el && window.scrollY >= el.offsetTop - 120) {
      activeSection.value = id;
      break;
    }
  }
};

onMounted(() => {
  setTimeout(() => {
    loading.value = false;
    setTimeout(() => { heroVisible.value = true; }, 100);
  }, 1000);
  window.addEventListener('scroll', handleScroll);
});

onUnmounted(() => {
  window.removeEventListener('scroll', handleScroll);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;1,700&display=swap');

/* ===== RESET & BASE ===== */
*, *::before, *::after { box-sizing: border-box; }
.landing-page {
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  background: #ffffff;
  overflow-x: hidden;
}

/* ===== TOKENS ===== */
.text-amber { color: #eab308 !important; }
.italic { font-style: italic; }
.bg-light-soft { background-color: #f8fafc; }
.text-slate-400 { color: #94a3b8; }
.text-slate-500 { color: #64748b; }
.text-slate-700 { color: #334155; }
.text-slate-800 { color: #1e293b; }
.text-slate-900 { color: #0f172a; }
.shadow-amber { box-shadow: 0 8px 24px rgba(234,179,8,0.25) !important; }
.max-w-600 { max-width: 600px; }
.line-height-relaxed { line-height: 1.75; }

/* ===== PRELOADER ===== */
.js-preloader {
  position: fixed; inset: 0; background: #0f172a; z-index: 9999;
  display: flex; align-items: center; justify-content: center;
}
.preloader-inner { display: flex; flex-direction: column; align-items: center; gap: 24px; }
.loader-ring { display: inline-block; position: relative; width: 60px; height: 60px; }
.loader-ring div {
  box-sizing: border-box; display: block; position: absolute;
  width: 48px; height: 48px; margin: 6px;
  border: 5px solid transparent; border-top-color: #eab308;
  border-radius: 50%;
  animation: ring-spin 1.2s cubic-bezier(0.5,0,0.5,1) infinite;
}
.loader-ring div:nth-child(1) { animation-delay: -0.45s; }
.loader-ring div:nth-child(2) { animation-delay: -0.3s; border-top-color: rgba(234,179,8,0.5); }
.loader-ring div:nth-child(3) { animation-delay: -0.15s; border-top-color: rgba(234,179,8,0.25); }
@keyframes ring-spin { 0% { transform: rotate(0); } 100% { transform: rotate(360deg); } }
.loader-brand { display: flex; align-items: center; gap: 12px; color: white; font-size: 22px; font-weight: 800; letter-spacing: -0.5px; }
.loader-brand b { color: #eab308; }

/* ===== NAVBAR ===== */
.header-nav {
  background: rgba(255,255,255,0.9);
  backdrop-filter: blur(16px);
  border-bottom: 1px solid rgba(241,245,249,0.8);
  z-index: 1100;
  transition: box-shadow 0.3s;
}
.header-nav.scrolled { box-shadow: 0 4px 30px rgba(0,0,0,0.06); }
.main-navigation { height: 80px; display: flex; align-items: center; justify-content: space-between; }

.brand { text-decoration: none; display: flex; align-items: center; gap: 10px; }
.brand-logo-wrap { flex-shrink: 0; }
.brand-name { font-size: 22px; font-weight: 800; color: #0f172a; letter-spacing: -0.5px; }

.nav-links { list-style: none; margin: 0; padding: 0; display: flex; gap: 30px; }
.nav-links a { text-decoration: none; color: #64748b; font-weight: 700; font-size: 15px; transition: 0.2s; position: relative; }
.nav-links a::after { content: ''; position: absolute; bottom: -4px; left: 0; width: 0; height: 2px; background: #eab308; border-radius: 10px; transition: 0.3s; }
.nav-links a:hover::after, .nav-links a.active::after { width: 100%; }
.nav-links a:hover, .nav-links a.active { color: #eab308; }

.btn-text { text-decoration: none; color: #0f172a; font-weight: 700; font-size: 15px; transition: 0.2s; margin-right: 8px; }
.btn-text:hover { color: #eab308; }
.btn-amber-pill {
  background: #eab308; color: #fff; padding: 12px 26px; border-radius: 50px;
  text-decoration: none; font-weight: 800; font-size: 14px; display: inline-flex; align-items: center; transition: 0.3s;
}
.btn-amber-pill:hover { transform: translateY(-2px); box-shadow: 0 8px 20px rgba(234,179,8,0.35); color: #fff; }

/* ===== HERO ===== */
.hero-section {
  min-height: 100vh; display: flex; align-items: center;
  padding: 120px 0 80px;
  background: #ffffff;
  position: relative; overflow: hidden;
}
.hero-bg-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(234,179,8,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(234,179,8,0.04) 1px, transparent 1px);
  background-size: 60px 60px;
  pointer-events: none;
}
.hero-orb {
  position: absolute; border-radius: 50%;
  filter: blur(80px); pointer-events: none;
}
.hero-orb-1 { width: 500px; height: 500px; background: rgba(234,179,8,0.12); top: -150px; right: -100px; animation: orb-float 8s ease-in-out infinite; }
.hero-orb-2 { width: 300px; height: 300px; background: rgba(234,179,8,0.07); bottom: 0; left: -50px; animation: orb-float 10s ease-in-out infinite reverse; }
@keyframes orb-float { 0%,100%{transform:translate(0,0)} 50%{transform:translate(30px,-30px)} }

/* Hero text animation */
.hero-text-wrapper { opacity: 0; transform: translateY(40px); transition: opacity 0.8s ease, transform 0.8s ease; }
.hero-img-wrapper { opacity: 0; transform: translateY(40px); transition: opacity 0.8s ease 0.3s, transform 0.8s ease 0.3s; }
.hero-visible { opacity: 1 !important; transform: none !important; }

.hero-badge {
  display: inline-flex; align-items: center; gap: 8px;
  background: #fefce8; color: #a16207;
  padding: 8px 18px; border-radius: 100px; font-weight: 800; font-size: 13px;
  margin-bottom: 24px; border: 1px solid #fde68a;
}
.badge-dot { width: 8px; height: 8px; background: #eab308; border-radius: 50%; animation: pulse-dot 2s ease-in-out infinite; }
@keyframes pulse-dot { 0%,100%{transform:scale(1);opacity:1} 50%{transform:scale(1.5);opacity:0.7} }

.hero-title { font-size: 62px; font-weight: 800; letter-spacing: -3px; line-height: 1.05; color: #0f172a; }
.hero-highlight { color: #eab308; font-style: italic; position: relative; }
.hero-highlight::after {
  content: ''; position: absolute; bottom: -4px; left: 0; right: 0; height: 4px;
  background: linear-gradient(90deg, #eab308, transparent);
  border-radius: 10px;
}
.hero-desc { font-size: 18px; color: #475569; margin-top: 24px; line-height: 1.65; max-width: 520px; }

.btn-amber-lg, .btn-amber {
  background: #eab308; color: #fff; padding: 18px 40px; border-radius: 16px;
  text-decoration: none; font-weight: 800; transition: 0.3s; display: inline-flex; align-items: center; gap: 8px;
}
.btn-amber-lg:hover, .btn-amber:hover { transform: translateY(-4px); box-shadow: 0 12px 28px rgba(234,179,8,0.35); color: #fff; }
.btn-outline-custom {
  border: 2px solid #e2e8f0; color: #0f172a; padding: 18px 40px; border-radius: 16px;
  text-decoration: none; font-weight: 800; transition: 0.3s; display: inline-flex; align-items: center;
}
.btn-outline-custom:hover { background: #0f172a; color: #fff; border-color: #0f172a; }

.hero-trust { display: flex; align-items: center; gap: 14px; }
.trust-avatars { display: flex; }
.ta {
  width: 36px; height: 36px; border-radius: 50%; border: 3px solid white;
  display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 13px; color: white;
  margin-left: -8px;
}
.ta:first-child { margin-left: 0; }
.trust-text { font-size: 14px; color: #64748b; }
.trust-text strong { color: #0f172a; }

/* Hero mockup */
.hero-mockup {
  background: white; border-radius: 24px; border: 1px solid #e2e8f0;
  overflow: hidden; max-width: 480px; margin: 0 auto; position: relative; z-index: 5;
  box-shadow: 0 40px 80px rgba(0,0,0,0.08);
}
.mock-header {
  background: #f8fafc; padding: 14px 20px; border-bottom: 1px solid #f1f5f9;
  display: flex; align-items: center; gap: 6px;
}
.mock-title { font-size: 12px; font-weight: 700; color: #94a3b8; margin-left: 10px; }
.m-dot { width: 10px; height: 10px; border-radius: 50%; }
.m-dot.red { background: #ff5f56; } .m-dot.yellow { background: #ffbd2e; } .m-dot.green { background: #27c93f; }
.mock-body { padding: 30px; }
.score-row { display: flex; gap: 24px; align-items: center; }
.score-circle { position: relative; width: 120px; height: 120px; flex-shrink: 0; }
.score-svg { width: 120px; height: 120px; }
.score-text {
  position: absolute; inset: 0; display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  font-size: 26px; font-weight: 800; color: #0f172a;
}
.score-text span { font-size: 11px; color: #eab308; text-transform: uppercase; font-weight: 800; letter-spacing: 1px; }
.score-details { flex: 1; }
.skill-bar-item { margin-bottom: 12px; }
.skill-track { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.skill-fill {
  height: 100%; background: linear-gradient(90deg, #eab308, #f59e0b);
  border-radius: 10px; width: 0;
  animation: fill-bar 1.5s ease forwards;
  animation-delay: 1.2s;
}
@keyframes fill-bar { to { width: var(--w); } }
.mock-tags { display: flex; gap: 8px; flex-wrap: wrap; }
.mock-tag { padding: 4px 10px; border-radius: 100px; font-size: 11px; font-weight: 700; }
.mock-tag.green { background: #dcfce7; color: #16a34a; }
.mock-tag.blue { background: #dbeafe; color: #2563eb; }
.mock-tag.amber { background: #fef9c3; color: #a16207; }

/* Float badges */
.float-badge {
  position: absolute; background: white; border: 1px solid #f1f5f9;
  border-radius: 50px; padding: 10px 18px; display: flex; align-items: center;
  gap: 8px; font-size: 13px; font-weight: 700; color: #0f172a;
  box-shadow: 0 10px 30px rgba(0,0,0,0.08);
}
.fb1 { top: 10%; right: -10%; }
.fb2 { bottom: 10%; left: -5%; }
.hero-glow-effect {
  position: absolute; top: 40%; left: 50%; transform: translate(-30%, -30%);
  width: 400px; height: 400px; background: rgba(234,179,8,0.15);
  filter: blur(80px); z-index: 1; border-radius: 50%; pointer-events: none;
}

/* ===== STATS BAR ===== */
.stats-bar {
  background: #f8fafc;
  border-top: 1px solid #f1f5f9;
  border-bottom: 1px solid #f1f5f9;
  padding: 32px 0;
}
.stats-inner {
  display: grid; grid-template-columns: repeat(6, 1fr);
  gap: 0;
}
.stat-item {
  display: flex; align-items: center; gap: 14px;
  padding: 16px 24px;
  border-right: 1px solid #f1f5f9;
}
.stat-item:last-child { border-right: none; }
.stat-icon {
  width: 44px; height: 44px; flex-shrink: 0;
  background: #fefce8; color: #eab308;
  border-radius: 12px; display: flex; align-items: center; justify-content: center;
}
.stat-val { font-size: 22px; font-weight: 800; color: #0f172a; margin: 0; line-height: 1.2; }
.stat-label { font-size: 11px; font-weight: 700; color: #94a3b8; text-transform: uppercase; letter-spacing: 0.8px; margin: 2px 0 0; }

/* ===== SECTION HEADINGS ===== */
.section-tag {
  display: inline-block; background: #fefce8; color: #a16207;
  padding: 6px 16px; border-radius: 100px; font-weight: 800;
  font-size: 12px; text-transform: uppercase; letter-spacing: 1.5px;
  border: 1px solid #fde68a;
}
.section-tag.light { background: rgba(255,255,255,0.15); color: white; border-color: rgba(255,255,255,0.3); }
.section-title { font-size: 44px; font-weight: 800; letter-spacing: -1.5px; color: #0f172a; }
.h-line { width: 60px; height: 4px; background: #eab308; margin: 20px auto; border-radius: 10px; }

/* ===== SERVICES ===== */
.services-section { background: #fff; }
.service-card {
  padding: 36px 28px; background: #fff; border: 1px solid #f1f5f9;
  border-radius: 24px; transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  text-align: center; position: relative; overflow: hidden;
}
.service-card::before {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(135deg, rgba(234,179,8,0.04) 0%, transparent 60%);
  opacity: 0; transition: 0.4s;
}
.service-card:hover { transform: translateY(-10px); box-shadow: 0 24px 50px rgba(0,0,0,0.06); border-color: #fde68a; }
.service-card:hover::before { opacity: 1; }
.service-card:hover .card-arrow { opacity: 1; transform: translate(-50%, -50%) scale(1); }
.icon-circle {
  width: 64px; height: 64px; background: #fefce8; color: #eab308;
  border-radius: 18px; display: flex; align-items: center; justify-content: center;
  margin: 0 auto 24px; transition: 0.3s;
}
.service-card:hover .icon-circle { background: #eab308; color: white; transform: rotate(5deg) scale(1.05); }
.service-card h4 { font-size: 17px; font-weight: 800; color: #0f172a; margin-bottom: 12px; }
.service-card p { font-size: 14px; color: #64748b; line-height: 1.6; margin: 0; }
.card-arrow {
  position: absolute; bottom: 20px; right: 20px;
  width: 32px; height: 32px; background: #eab308; color: white;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  opacity: 0; transition: 0.3s;
}

/* ===== PROCESS ===== */
.process-section { background: #ffffff; }
.process-connector {
  position: absolute; top: 50%; left: 50%;
  transform: translate(-50%,0);
  width: 70%; height: 2px;
  background: linear-gradient(90deg, transparent, #fde68a, #eab308, #fde68a, transparent);
}
.process-card {
  padding: 36px 28px; border-radius: 24px;
  background: #fff; border: 1px solid #f1f5f9;
  transition: 0.4s; position: relative; text-align: center;
}
.process-card:hover { transform: translateY(-8px); box-shadow: 0 20px 50px rgba(0,0,0,0.06); border-color: #fde68a; }
.process-card-featured { border-color: #eab308; background: linear-gradient(135deg, #fffbeb, #fff); }
.process-card-featured:hover { box-shadow: 0 20px 50px rgba(234,179,8,0.15); }
.step-number-wrap { position: relative; display: inline-block; margin-bottom: 20px; }
.step-number {
  width: 52px; height: 52px; background: #f1f5f9; color: #94a3b8;
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 22px; border-radius: 14px; margin: 0 auto;
  transition: 0.3s;
}
.step-number.step-amber { background: #eab308; color: white; }
.process-card:hover .step-number { background: #eab308; color: white; }
.step-pulse {
  position: absolute; inset: -6px;
  border-radius: 20px; border: 2px solid rgba(234,179,8,0.4);
  animation: pulse-ring 2s ease-in-out infinite;
}
@keyframes pulse-ring { 0%,100%{opacity:1;transform:scale(1)} 50%{opacity:0.5;transform:scale(1.1)} }
.process-icon { margin-bottom: 8px; }
.process-card h5 { font-size: 18px; font-weight: 800; }
.process-card p { font-size: 14px; }
.process-badge {
  display: inline-block; background: #fefce8; color: #a16207; border: 1px solid #fde68a;
  padding: 4px 12px; border-radius: 100px; font-size: 11px; font-weight: 800;
  text-transform: uppercase; letter-spacing: 1px; margin-top: 16px;
}

/* ===== ABOUT ===== */
.about-img-box { padding-bottom: 40px; padding-left: 30px; }
.about-img { border-radius: 20px !important; }
.about-li { display: flex; align-items: center; gap: 12px; }
.about-check {
  width: 24px; height: 24px; background: #eab308; border-radius: 8px;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}
.floating-badge {
  position: absolute; bottom: -10px; left: 0; background: #fff;
  padding: 16px 20px; border-radius: 18px; border: 1px solid #f1f5f9; z-index: 10;
}
.floating-badge-2 {
  position: absolute; top: 20px; right: -10px; background: #fff;
  padding: 10px 16px; border-radius: 100px; border: 1px solid #f1f5f9;
  z-index: 10; display: flex; align-items: center;
}
.icon-bg-amber { width: 48px; height: 48px; background: #eab308; border-radius: 14px; display: flex; align-items: center; justify-content: center; }

/* ===== TESTIMONIALS ===== */
.testimonials { background: #f8fafc; }
.testimonial-card {
  padding: 36px 30px; background: #fff; border: 1px solid #f1f5f9;
  border-radius: 24px; transition: 0.3s; position: relative; overflow: hidden;
}
.testimonial-featured { border-color: #eab308; }
.testimonial-featured::before {
  content: ''; position: absolute; top: 0; left: 0; right: 0; height: 4px; background: #eab308;
}
.testimonial-card:hover { box-shadow: 0 20px 40px rgba(0,0,0,0.06); transform: translateY(-4px); }
.quote-mark { font-size: 72px; font-weight: 800; color: #fde68a; line-height: 0.5; height: 30px; overflow: visible; }
.avatar-placeholder { width: 48px; height: 48px; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 18px; color: white; }

/* ===== CTA FINAL ===== */
.final-cta { padding: 60px 0; }
.cta-box {
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%);
  padding: 80px 40px; border-radius: 32px;
  border: 1px solid rgba(234,179,8,0.2);
}
.cta-particles { position: absolute; inset: 0; overflow: hidden; }
.cta-p {
  position: absolute; width: 4px; height: 4px; background: #eab308;
  border-radius: 50%; opacity: 0;
  animation: cta-particle 4s ease-in-out infinite;
}
.cta-p:nth-child(1) { left: 10%; top: 20%; animation-delay: 0s; }
.cta-p:nth-child(2) { left: 20%; top: 80%; animation-delay: 0.5s; }
.cta-p:nth-child(3) { left: 30%; top: 40%; animation-delay: 1s; }
.cta-p:nth-child(4) { left: 40%; top: 70%; animation-delay: 1.5s; }
.cta-p:nth-child(5) { left: 50%; top: 10%; animation-delay: 2s; }
.cta-p:nth-child(6) { left: 60%; top: 60%; animation-delay: 2.5s; }
.cta-p:nth-child(7) { left: 70%; top: 30%; animation-delay: 3s; }
.cta-p:nth-child(8) { left: 80%; top: 85%; animation-delay: 3.5s; }
.cta-p:nth-child(9) { left: 90%; top: 50%; animation-delay: 0.8s; }
.cta-p:nth-child(10) { left: 5%; top: 60%; animation-delay: 1.3s; }
.cta-p:nth-child(11) { left: 95%; top: 25%; animation-delay: 1.8s; }
.cta-p:nth-child(12) { left: 45%; top: 90%; animation-delay: 2.3s; }
@keyframes cta-particle {
  0%,100%{opacity:0;transform:translateY(0) scale(0)}
  50%{opacity:1;transform:translateY(-30px) scale(1)}
}
.cta-circle-1,.cta-circle-2,.cta-circle-3 { position: absolute; border-radius: 50%; border: 1px solid rgba(234,179,8,0.15); }
.cta-circle-1 { width: 400px; height: 400px; top: -150px; right: -100px; animation: rotate-slow 20s linear infinite; }
.cta-circle-2 { width: 250px; height: 250px; bottom: -100px; left: 5%; animation: rotate-slow 15s linear infinite reverse; }
.cta-circle-3 { width: 150px; height: 150px; top: 20%; left: 40%; animation: rotate-slow 10s linear infinite; }
@keyframes rotate-slow { to { transform: rotate(360deg); } }

/* ===== FOOTER ===== */
.footer-dark { background: #0f172a; border-radius: 40px 40px 0 0; margin-top: 40px; }
.footer-links a { color: #94a3b8; text-decoration: none; transition: 0.2s; display: block; line-height: 2.5; font-weight: 600; font-size: 14px; }
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
  width: 48px; background: #eab308; border: none; color: white;
  font-size: 20px; font-weight: bold; border-radius: 0 12px 12px 0; cursor: pointer;
  transition: 0.2s;
}
.newsletter-btn:hover { background: #d97706; }
.newsletter-input, .newsletter-btn { vertical-align: middle; }
.footer-newsletter { display: flex; }

/* ===== ANIMATIONS ===== */
@keyframes float { 0%,100%{transform:translateY(0)} 50%{transform:translateY(-18px)} }
@keyframes float-slow { 0%,100%{transform:translateY(0)} 50%{transform:translateY(-12px)} }
.animate-float { animation: float 6s ease-in-out infinite; }
.animate-float-slow { animation: float-slow 8s ease-in-out infinite; }
.animate-float-delayed { animation: float 6s ease-in-out infinite; animation-delay: 2s; }

/* ===== RESPONSIVE ===== */
@media (max-width: 1199px) {
  .stats-inner { grid-template-columns: repeat(3, 1fr); }
  .stat-item:nth-child(3) { border-right: none; }
  .stat-item:nth-child(4) { border-top: 1px solid #f1f5f9; }
}
@media (max-width: 991px) {
  .hero-section { min-height: auto; padding: 100px 0 60px; }
  .hero-title { font-size: 40px; text-align: center; }
  .hero-desc { text-align: center; margin: 20px auto; }
  .hero-btns { justify-content: center; flex-direction: column; }
  .hero-badge { margin: 0 auto 20px; }
  .hero-trust { justify-content: center; }
  .section-title { font-size: 34px; }
  .floating-badge, .floating-badge-2 { display: none; }
  .footer-dark { border-radius: 24px 24px 0 0; }
  .stats-inner { grid-template-columns: repeat(2, 1fr); }
  .stat-item { border-right: none; border-bottom: 1px solid #f1f5f9; }
  .fb1, .fb2 { display: none; }
}
@media (max-width: 575px) {
  .hero-title { font-size: 32px; letter-spacing: -1.5px; }
  .stats-inner { grid-template-columns: 1fr 1fr; }
  .cta-box { padding: 50px 24px; }
}
</style>