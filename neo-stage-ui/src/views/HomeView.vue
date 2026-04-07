<template>
  <div class="landing-page">
    
    <!-- 1. PRELOADER (Fixé) -->
    <div id="js-preloader" class="js-preloader" v-if="loading">
      <div class="preloader-inner">
        <div class="loader-circle"></div>
        <div class="loader-text">EVALUA<span>TECH</span></div>
      </div>
    </div>

    <!-- 2. NAVBAR (Glassmorphism) -->
    <header class="header-nav sticky-top">
      <div class="container">
        <nav class="main-navigation">
          <router-link to="/" class="brand">
            <div class="brand-logo">E</div>
            <span class="brand-name">Evalua<span class="text-amber">Tech</span></span>
          </router-link>
          
          <ul class="nav-links d-none d-lg-flex">
            <li><a href="#top" class="active">Accueil</a></li>
            <li><a href="#services">Services</a></li>
            <li><a href="#process">Processus</a></li>
            <li><router-link to="/pricing">Tarifs</router-link></li>
          </ul>

          <div class="nav-auth">
            <router-link to="/login" class="btn-text d-none d-sm-inline">Connexion</router-link>
            <router-link to="/login" class="btn-amber-pill shadow-amber">
              Essai Gratuit 
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="ms-1"><path d="M12 2L15.09 8.26L22 9.27L17 14.14L18.18 21.02L12 17.77L5.82 21.02L7 14.14L2 9.27L8.91 8.26L12 2Z"/></svg>
            </router-link>
          </div>
        </nav>
      </div>
    </header>

    <!-- 3. HERO SECTION -->
    <section id="top" class="hero-section">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-6 hero-text-wrapper">
            <div class="hero-badge">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="3" class="me-2"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path></svg>
              Plateforme IA · PFE 2026
            </div>
            <h1 class="hero-title">Révolutionnez vos <br> <span>évaluations avec l'IA</span></h1>
            <p class="hero-desc">
              EvaluaTech est une plateforme SaaS d'évaluation technique et comportementale. 
              Automatisez la création de tests et analysez les compétences en temps réel.
            </p>
            <div class="hero-btns mt-4 d-flex align-items-center gap-3">
              <router-link to="/login" class="btn-amber-lg shadow-amber">Démarrer maintenant</router-link>
              <a href="#services" class="btn-outline-custom">
                Découvrir 
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="ms-1"><path d="M5 12h14m-7-7l7 7-7 7"></path></svg>
              </a>
            </div>
          </div>
          <div class="col-lg-6 hero-img-wrapper mt-5 mt-lg-0">
            <div class="right-image position-relative">
              <div class="hero-mockup shadow-2xl animate-float">
                <div class="mock-header"><span class="m-dot red"></span><span class="m-dot yellow"></span><span class="m-dot green"></span></div>
                <div class="mock-body text-center">
                   <div class="score-circle">84.2<span>Score IA</span></div>
                   <div class="mock-line w-75"></div>
                   <div class="mock-line w-50 mx-auto"></div>
                </div>
              </div>
              <div class="hero-glow-effect"></div>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 4. STATS BAR -->
    <section class="stats-bar border-top border-bottom bg-light-soft py-4">
      <div class="container">
        <div class="row text-center g-4">
          <div class="col-6 col-md-3" v-for="(stat, i) in statsData" :key="i" :class="{'border-start d-none d-md-block': i > 1, 'border-start': i === 1}">
            <h3 class="fw-bold mb-0 text-slate-900">{{ stat.val }}</h3>
            <p class="text-slate-500 small mb-0 fw-bold text-uppercase ls-1">{{ stat.label }}</p>
          </div>
        </div>
      </div>
    </section>

    <!-- 5. SERVICES -->
    <section id="services" class="services-section py-5">
      <div class="container py-5">
        <div class="section-heading text-center mb-5">
          <span class="text-amber fw-bold text-uppercase small ls-2">Nos Fonctionnalités</span>
          <h2 class="fw-bold fs-1 mt-2">Pourquoi choisir <span class="text-amber">EvaluaTech</span> ?</h2>
          <div class="h-line"></div>
        </div>
        <div class="row g-4">
          <div class="col-lg-3 col-md-6" v-for="(s, i) in serviceList" :key="i">
            <div class="service-card h-100">
              <div class="icon-circle" v-html="s.svg"></div>
              <h4>{{ s.title }}</h4>
              <p>{{ s.desc }}</p>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- 6. SECTION MISE À JOUR : COMMENCER EST FACILE (STYLE PHOTO) -->
    <section id="process" class="process-section py-5 bg-white position-relative">
      <div class="container py-5">
        <div class="section-heading text-center mb-5">
          <h2 class="fw-bold fs-1 mt-2 text-slate-900">Commencer est facile</h2>
          <p class="text-muted">Trois étapes simples pour un meilleur recrutement</p>
        </div>
        
        <div class="position-relative">
          <!-- Flèches latérales -->
          <div class="nav-arrow left"><i class="fa fa-chevron-left"></i></div>
          <div class="nav-arrow right"><i class="fa fa-chevron-right"></i></div>

          <div class="row g-4 mt-5">
            <div class="col-lg-4 text-center" v-for="(step, i) in processSteps" :key="i">
              <div class="step-square-icon">{{ i + 1 }}</div>
              <h5 class="fw-bold mt-4 text-slate-800">{{ step.title }}</h5>
              <p class="text-muted small mt-3 px-4 line-height-relaxed">{{ step.desc }}</p>
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
            <span class="text-amber fw-bold small text-uppercase">À propos de nous</span>
            <h2 class="fw-bold mt-2 display-5">Digitaliser l'évaluation avec <span class="text-amber italic">précision</span></h2>
            <p class="mt-4 text-muted">Notre mission est de digitaliser le cycle de l'évaluation en offrant une traçabilité totale et une analyse objective des compétences.</p>
            <ul class="list-unstyled mt-4 text-slate-700 fw-medium">
              <li class="mb-3"><svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" class="me-2"><polyline points="20 6 9 17 4 12"></polyline></svg> Gain de temps sur la correction</li>
              <li class="mb-3"><svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" class="me-2"><polyline points="20 6 9 17 4 12"></polyline></svg> Bilan des compétences automatisé</li>
              <li class="mb-3"><svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#eab308" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" class="me-2"><polyline points="20 6 9 17 4 12"></polyline></svg> Protection contre la fraude (Proctoring)</li>
            </ul>
            <router-link to="/pricing" class="btn-amber mt-4">Découvrir nos solutions</router-link>
          </div>
          <div class="col-lg-6 text-center">
             <div class="about-img-box position-relative">
                <img src="https://images.unsplash.com/photo-1551288049-bbda38a10ad5?auto=format&fit=crop&q=80&w=800" alt="Stats IA" class="img-fluid rounded-4 shadow-lg">
                <div class="floating-badge shadow-lg animate-float-delayed">
                   <div class="d-flex align-items-center gap-3">
                      <div class="icon-bg-amber"><svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#fff" stroke-width="2"><path d="M12 2v20M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/></svg></div>
                      <div class="text-start">
                        <div class="fw-bold fs-5 text-dark">-40%</div>
                        <div class="small text-muted">Coûts de recrutement</div>
                      </div>
                   </div>
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
          <h2 class="fw-bold fs-1 mt-2">Ce qu'ils disent de nous</h2>
          <div class="h-line"></div>
        </div>
        <div class="row g-4">
          <div class="col-lg-4" v-for="(t, i) in testimonials" :key="i">
            <div class="testimonial-card shadow-sm h-100">
              <div class="d-flex text-amber mb-3">
                <svg v-for="s in 5" :key="s" width="18" height="18" viewBox="0 0 24 24" fill="currentColor"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
              </div>
              <p class="fst-italic text-slate-700">"{{ t.quote }}"</p>
              <div class="d-flex align-items-center mt-4">
                <div class="avatar-placeholder me-3">{{ t.name.charAt(0) }}</div>
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
        <div class="cta-box text-center shadow-2xl rounded-4 position-relative overflow-hidden">
          <div class="position-relative z-index-10">
            <h2 class="fw-bold text-white mb-3">Prêt à identifier vos futurs talents ?</h2>
            <p class="text-white-50 mb-4 max-w-600 mx-auto">Rejoignez plus de 500 entreprises qui utilisent l'IA d'EvaluaTech pour fiabiliser leurs recrutements techniques.</p>
            <router-link to="/login" class="btn btn-light btn-lg rounded-pill px-5 fw-bold text-slate-900 shadow">
              Démarrer l'essai gratuit
            </router-link>
          </div>
          <div class="cta-circle-1"></div>
          <div class="cta-circle-2"></div>
        </div>
      </div>
    </section>

    <!-- 10. FOOTER -->
    <footer class="footer-dark">
      <div class="container py-5">
        <div class="row gy-4">
          <div class="col-lg-4">
            <h3 class="text-white fw-bold mb-3">Evalua<span class="text-amber">Tech</span></h3>
            <p class="text-slate-400 small">Innovation Éducative & Professionnelle. Projet SaaS assisté par l'IA pour moderniser l'évaluation.</p>
          </div>
          <div class="col-lg-4 text-lg-center">
            <h5 class="text-white fw-bold mb-4">Navigation</h5>
            <ul class="list-unstyled footer-links">
              <li><a href="#top">Accueil</a></li>
              <li><a href="#services">Services</a></li>
              <li><router-link to="/pricing">Tarifs</router-link></li>
            </ul>
          </div>
          <div class="col-lg-4 text-lg-end">
            <h5 class="text-white fw-bold mb-4">Contact</h5>
            <p class="text-slate-400 small">contact@evaluatech.tn <br> Plateforme SaaS - PFE 2026</p>
          </div>
        </div>
        <div class="footer-bottom mt-5 pt-4 border-top border-slate-700 text-center">
          <p class="text-slate-500 small">© 2026 EvaluaTech IA. Tous droits réservés.</p>
        </div>
      </div>
    </footer>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';

const loading = ref(true);

const statsData =[
  { val: "1.2M+", label: "Tests générés" },
  { val: "98%", label: "Taux de précision" },
  { val: "500+", label: "Entreprises clientes" },
  { val: "24/7", label: "Surveillance IA" }
];

const serviceList =[
  { 
    title: "Banque de Questions", 
    desc: "Bibliothèque structurée par thèmes et niveaux gérée par IA.", 
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"></path><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"></path></svg>` 
  },
  { 
    title: "Surveillance IA", 
    desc: "Système anti-triche intégré (Proctoring) pour garantir l'équité.", 
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M12 22s8-4 8-10V5l-8-3-8 3v7c0 6 8 10 8 10z"></path></svg>` 
  },
  { 
    title: "Rapports Détaillés", 
    desc: "Analyses automatiques et recommandations personnalisées.", 
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21.21 15.89A10 10 0 1 1 8 2.83"></path><path d="M22 12A10 10 0 0 0 12 2v10z"></path></svg>` 
  },
  { 
    title: "Multi-Entreprises", 
    desc: "Architecture SaaS permettant une gestion isolée sécurisée.", 
    svg: `<svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><rect x="3" y="3" width="7" height="7"></rect><rect x="14" y="3" width="7" height="7"></rect><rect x="14" y="14" width="7" height="7"></rect><rect x="3" y="14" width="7" height="7"></rect></svg>` 
  }
];

const processSteps =[
  { title: "Créez votre évaluation", desc: "Définissez les exigences du rôle et laissez EvaliA générer des questions pertinentes, ou créez une évaluation personnalisée." },
  { title: "Invitez les candidats", desc: "Envoyez les liens d'évaluation par e-mail. Les candidats complètent les tests selon leur horaire, depuis n'importe quel appareil." },
  { title: "Examinez et décidez", desc: "Obtenez des rapports de notation détaillés et des analyses. Comparez les réponses et prenez des décisions confiantes." }
];

const testimonials =[
  { quote: "Depuis qu'on utilise EvaluaTech, le temps de screening technique a été divisé par trois. L'IA génère des questions qu'on n'aurait pas imaginées.", name: "Ahmed B.", role: "CTO @ TechMakers" },
  { quote: "La fonction de Proctoring nous rassure sur l'authenticité des résultats, surtout pour le recrutement de développeurs en full remote.", name: "Sarah Y.", role: "HR Manager @ Innovate" },
  { quote: "Les rapports détaillés permettent de comprendre non seulement si le code fonctionne, mais aussi la logique derrière. Un outil indispensable.", name: "Karim D.", role: "Lead Developer" }
];

onMounted(() => {
  setTimeout(() => { loading.value = false; }, 800);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700;800&display=swap');

/* --- GLOBAL --- */
.landing-page {
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: #0f172a;
  background-color: #ffffff;
  overflow-x: hidden;
}

.text-amber { color: #eab308 !important; }
.bg-light-soft { background-color: #f8fafc; }
.bg-slate-900 { background-color: #0f172a !important; }
.text-slate-400 { color: #94a3b8; }
.text-slate-500 { color: #64748b; }
.text-slate-700 { color: #334155; }
.text-slate-900 { color: #0f172a; }

.shadow-2xl { box-shadow: 0 40px 100px rgba(0,0,0,0.08); }
.ls-1 { letter-spacing: 1px; }
.ls-2 { letter-spacing: 2px; }
.z-index-10 { z-index: 10; position: relative; }
.max-w-600 { max-width: 600px; }

/* --- PRELOADER --- */
.js-preloader {
  position: fixed; inset: 0; background: #fff; z-index: 9999;
  display: flex; align-items: center; justify-content: center;
}
.loader-circle { width: 40px; height: 40px; border: 4px solid #f1f5f9; border-top: 4px solid #eab308; border-radius: 50%; animation: spin 1s linear infinite; }
.loader-text { margin-left: 15px; font-weight: 800; font-size: 20px; }
.loader-text span { color: #eab308; }
@keyframes spin { to { transform: rotate(360deg); } }

/* --- NAVBAR --- */
.header-nav {
  background: rgba(255,255,255,0.85);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid #f1f5f9;
  z-index: 1100;
}
.main-navigation { height: 80px; display: flex; align-items: center; justify-content: space-between; }
.brand { text-decoration: none; display: flex; align-items: center; gap: 10px; }
.brand-logo { width: 36px; height: 36px; background: #0f172a; color: white; border-radius: 10px; display: flex; align-items: center; justify-content: center; font-weight: 800; }
.brand-name { font-size: 24px; font-weight: 800; color: #0f172a; letter-spacing: -1px; }

.nav-links { list-style: none; margin: 0; padding: 0; display: flex; gap: 35px; }
.nav-links a { text-decoration: none; color: #64748b; font-weight: 700; font-size: 15px; transition: 0.2s; }
.nav-links a:hover, .nav-links a.active { color: #eab308; }

.btn-text { text-decoration: none; color: #0f172a; font-weight: 700; font-size: 15px; transition: 0.2s; }
.btn-text:hover { color: #eab308; }

.btn-amber-pill {
  background: #eab308; color: #fff; padding: 12px 28px; border-radius: 50px;
  text-decoration: none; font-weight: 800; display: inline-flex; align-items: center; transition: 0.3s;
}
.btn-amber-pill:hover { transform: translateY(-3px); box-shadow: 0 10px 25px rgba(234, 179, 8, 0.3); }

/* --- HERO --- */
.hero-section { 
  padding: 160px 0 100px; 
  background: radial-gradient(circle at 90% 10%, rgba(234, 179, 8, 0.05) 0%, #fff 60%);
  position: relative; overflow: hidden;
}
.hero-badge {
  display: inline-flex; align-items: center; background: #fefce8; color: #a16207;
  padding: 10px 20px; border-radius: 100px; font-weight: 800; font-size: 13px; margin-bottom: 25px;
}
.hero-title { font-size: 64px; font-weight: 800; letter-spacing: -3px; line-height: 1.1; color: #0f172a; }
.hero-title span { color: #eab308; font-style: italic; }
.hero-desc { font-size: 19px; color: #475569; margin-top: 25px; line-height: 1.6; max-width: 550px; }

.btn-amber-lg, .btn-amber {
  background: #eab308; color: #fff; padding: 18px 40px; border-radius: 16px;
  text-decoration: none; font-weight: 800; transition: 0.3s; display: inline-block;
}
.btn-amber-lg:hover, .btn-amber:hover { transform: translateY(-5px); box-shadow: 0 15px 30px rgba(234, 179, 8, 0.3); }

.btn-outline-custom {
  border: 2px solid #0f172a; color: #0f172a; padding: 18px 40px; border-radius: 16px;
  text-decoration: none; font-weight: 800; transition: 0.3s; display: inline-flex; align-items: center;
}
.btn-outline-custom:hover { background: #0f172a; color: #fff; }

.hero-mockup {
  background: white; border-radius: 24px; border: 1px solid #e2e8f0; 
  overflow: hidden; max-width: 450px; margin: 0 auto; position: relative; z-index: 5;
}
.mock-header { background: #f8fafc; padding: 15px 20px; border-bottom: 1px solid #f1f5f9; }
.m-dot { width: 10px; height: 10px; border-radius: 50%; display: inline-block; margin-right: 4px; }
.m-dot.red { background: #ff5f56; } .m-dot.yellow { background: #ffbd2e; } .m-dot.green { background: #27c93f; }
.mock-body { padding: 40px; }
.score-circle {
  width: 160px; height: 160px; border: 12px solid #eab308; border-radius: 50%;
  margin: 0 auto 30px; display: flex; flex-direction: column; justify-content: center;
  font-size: 40px; font-weight: 800; color: #0f172a;
}
.score-circle span { font-size: 14px; color: #eab308; text-transform: uppercase; font-weight: 800; letter-spacing: 1px; }
.mock-line { height: 10px; background: #f1f5f9; border-radius: 10px; margin-bottom: 15px; }
.hero-glow-effect { position: absolute; top: 50%; left: 50%; transform: translate(-30%, -30%); width: 500px; height: 500px; background: rgba(234, 179, 8, 0.2); filter: blur(80px); z-index: 1; border-radius: 50%; }

/* --- SERVICES --- */
.services-section { background: #fff; }
.h-line { width: 60px; height: 4px; background: #eab308; margin: 25px auto; border-radius: 10px; }
.service-card {
  padding: 40px 30px; background: #fff; border: 1px solid #f1f5f9;
  border-radius: 30px; transition: 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275); text-align: center;
}
.service-card:hover { transform: translateY(-12px); box-shadow: 0 30px 60px rgba(0,0,0,0.05); border-color: #eab308; }
.icon-circle {
  width: 64px; height: 64px; background: #fefce8; color: #eab308;
  border-radius: 16px; display: flex; align-items: center; justify-content: center; margin: 0 auto 30px;
}

/* --- SECTION COMMENCER EST FACILE (NEW STYLE) --- */
.step-square-icon {
  width: 50px; height: 50px;
  background-color: #f59e0b; /* Jaune exact photo */
  color: white; font-weight: 800; font-size: 20px;
  display: flex; align-items: center; justify-content: center;
  border-radius: 12px; margin: 0 auto;
}
.line-height-relaxed { line-height: 1.7; }
.nav-arrow {
  position: absolute; top: 50%; transform: translateY(-50%);
  width: 40px; height: 40px; background: #6b7280; color: white;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  cursor: pointer; z-index: 5;
}
.nav-arrow.left { left: 0; }
.nav-arrow.right { right: 0; }

/* --- ABOUT --- */
.floating-badge {
  position: absolute; bottom: -30px; left: -30px; background: #fff;
  padding: 20px; border-radius: 20px; border: 1px solid #f1f5f9; z-index: 10;
}
.icon-bg-amber { width: 50px; height: 50px; background: #eab308; border-radius: 15px; display: flex; align-items: center; justify-content: center; }

/* --- TESTIMONIALS --- */
.testimonials { background: #fff; }
.testimonial-card {
  padding: 40px 30px; background: #fff; border: 1px solid #f1f5f9;
  border-radius: 24px; transition: 0.3s;
}
.testimonial-card:hover { box-shadow: 0 20px 40px rgba(0,0,0,0.05) !important; }
.avatar-placeholder { width: 50px; height: 50px; background: #fefce8; color: #eab308; font-weight: 800; font-size: 20px; display: flex; align-items: center; justify-content: center; border-radius: 50%; }

/* --- FINAL CTA --- */
.cta-box {
  background: linear-gradient(135deg, #eab308 0%, #d97706 100%);
  padding: 80px 40px;
}
.cta-circle-1 { position: absolute; width: 300px; height: 300px; background: rgba(255,255,255,0.1); border-radius: 50%; top: -100px; right: -50px; }
.cta-circle-2 { position: absolute; width: 200px; height: 200px; background: rgba(255,255,255,0.1); border-radius: 50%; bottom: -100px; left: 10%; }

/* --- FOOTER --- */
.footer-dark { background: #0f172a; padding: 80px 0 30px; border-radius: 60px 60px 0 0; }
.footer-links a { color: #94a3b8; text-decoration: none; transition: 0.2s; line-height: 2.5; font-weight: 600; }
.footer-links a:hover { color: #eab308; padding-left: 5px; }

/* --- ANIMATIONS --- */
@keyframes float { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(-20px); } }
.animate-float { animation: float 6s ease-in-out infinite; }
.animate-float-delayed { animation: float 6s ease-in-out infinite; animation-delay: 2s; }

@media (max-width: 991px) {
  .hero-title { font-size: 42px; text-align: center; }
  .hero-desc { text-align: center; margin: 25px auto; }
  .hero-btns { justify-content: center; flex-direction: column; }
  .btn-outline-custom { margin-left: 0 !important; margin-top: 15px; }
  .hero-img-wrapper { margin-top: 80px; }
  .floating-badge { display: none; }
  .footer-dark { border-radius: 30px 30px 0 0; }
  .nav-arrow { display: none; }
}
</style>