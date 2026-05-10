<template>
  <div class="bq-root" :class="{ 'dark-mode': isDark }" @mousemove="handleParallax">

    <!-- ══════════════════════ BACKGROUND ENGINE ══════════════════════ -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.035)"></div>
      <div class="glow-orb orb-slate" :style="orbStyle(0.02)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="bq-workspace p-4 p-lg-5">

          <!-- ══════════════════════ HEADER ══════════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3 bq-header">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Banque des Questions</span>
              </div>
              <h2 class="premium-title">
                Banque des <span class="gradient-text">Questions</span>
              </h2>
              <p class="brand-subtitle-v2 d-flex align-items-center gap-2 mt-2">
                <span class="live-dot-wrap">
                  <span class="live-dot"></span>
                  <span class="live-ring"></span>
                </span>
                Référentiel IA · Génération bilingue FR/EN · <strong>{{ questions.length }}</strong> actifs
              </p>
            </div>

            <div class="d-flex align-items-center gap-2 flex-wrap">
              <!-- DARK MODE TOGGLE -->
              <button class="btn-refresh-pro" @click="isDark = !isDark" :title="isDark ? 'Mode clair' : 'Mode sombre'">
                <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
              </button>

              <!-- SEARCH -->
              <div class="search-inline-box" :class="{ focused: searchFocused }">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input
                  v-model="searchQuery"
                  @focus="searchFocused = true"
                  @blur="searchFocused = false"
                  type="text"
                  placeholder="Rechercher..."
                  class="search-inline-input"
                >
                <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                  <i class="fa-solid fa-xmark"></i>
                </button>
              </div>

              <button class="btn-outline-pro" @click="showCatManager = true">
                <i class="fa-solid fa-sitemap me-2"></i>Catégories
              </button>

              <button class="btn-ai-glow" @click="showAIModal = true">
                <span class="btn-shine-layer"></span>
                <i class="fa-solid fa-wand-magic-sparkles me-2"></i>
                Générer par IA
                <span class="lang-badge-pill ms-2">FR/EN</span>
              </button>

              <button class="btn-enigma-primary shadow-premium" @click="openModal()">
                <div class="btn-content">
                  <i class="fa-solid fa-plus me-2"></i>Nouvelle question
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- ══════════════════════ KPI STATS ══════════════════════ -->
          <div class="row g-3 mb-5">
            <div class="col-xl col-md-4 col-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
                <div v-if="stat.trend" class="stat-trend ms-auto trend-up">
                  <i class="fa-solid fa-arrow-trend-up"></i>
                  <span>+{{ stat.trend }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ══════════════════════ TOOLBAR ══════════════════════ -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <!-- TYPE FILTERS -->
            <div class="d-flex align-items-center gap-2 flex-wrap">
              <div class="tabs-container">
                <div class="d-flex gap-2 p-1 rounded-4 shadow-sm border tabs-pill-wrap">
                  <button
                    class="nav-tab-btn-modern"
                    :class="{ active: activeFilter === -1 }"
                    @click="activeFilter = -1"
                  >
                    <i class="fa-solid fa-border-all me-1"></i>Tous
                    <span class="tab-count">{{ questions.length }}</span>
                  </button>
                  <button
                    v-for="t in typeDefinitions"
                    :key="t.val"
                    class="nav-tab-btn-modern"
                    :class="{ active: activeFilter === t.val }"
                    :style="activeFilter === t.val ? { '--tab-accent': t.color } : {}"
                    @click="activeFilter = t.val"
                  >
                    <i :class="t.icon + ' me-1'" :style="{ color: activeFilter === t.val ? t.color : '' }"></i>
                    {{ t.label }}
                    <span class="tab-count">{{ countByType(t.val) }}</span>
                  </button>
                </div>
              </div>
            </div>

            <div class="d-flex align-items-center gap-2 flex-wrap">
              <!-- CATEGORY SELECT -->
              <div class="sort-select-wrap">
                <i class="fa-solid fa-layer-group sort-ico"></i>
                <select v-model="selectedCat" class="sort-select-pro">
                  <option value="All">Toutes catégories</option>
                  <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                </select>
                <i class="fa-solid fa-chevron-down sort-arrow"></i>
              </div>

              <!-- LANG FILTER -->
              <div class="lang-cluster">
                <button :class="['lang-tab', { active: filterLang === 'all' }]" @click="filterLang = 'all'">
                  <i class="fa-solid fa-globe"></i> Tous
                  <span class="ltab-count">{{ questions.length }}</span>
                </button>
                <button :class="['lang-tab', { active: filterLang === 'fr' }]" @click="filterLang = 'fr'">
                  🇫🇷 FR <span class="ltab-count">{{ countByLang('fr') }}</span>
                </button>
                <button :class="['lang-tab', { active: filterLang === 'en' }]" @click="filterLang = 'en'">
                  🇬🇧 EN <span class="ltab-count">{{ countByLang('en') }}</span>
                </button>
              </div>

              <!-- VIEW TOGGLE -->
              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'grid' }]" @click="viewMode = 'grid'" title="Grille">
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'list' }]" @click="viewMode = 'list'" title="Liste">
                  <i class="fa-solid fa-list-ul"></i>
                </button>
              </div>
            </div>
          </div>

          <!-- ══════════════════════ LOADING STATE ══════════════════════ -->
          <div v-if="loading" class="empty-state-pro py-5 text-center">
            <div class="spinner-pro-premium"></div>
            <p class="state-label mt-3"><i class="fa-solid fa-satellite-dish fa-spin me-2"></i>Chargement...</p>
          </div>

          <!-- ══════════════════════ EMPTY STATE ══════════════════════ -->
          <div v-else-if="filteredQuestions.length === 0" class="empty-state-pro py-5 text-center">
            <div class="empty-graphic mb-4">
              <div class="empty-ring r1"></div>
              <div class="empty-ring r2"></div>
              <div class="empty-ring r3"></div>
              <div class="empty-core"><i class="fa-solid fa-database"></i></div>
            </div>
            <h5 class="fw-800 mb-2">Aucune question trouvée</h5>
            <p class="text-muted small">Modifiez vos filtres ou créez de nouvelles questions</p>
            <button class="btn-enigma-primary mt-3" @click="resetFilters">
              <div class="btn-content">
                <i class="fa-solid fa-rotate-left me-2"></i>Réinitialiser
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>

          <!-- ══════════════════════ GRID VIEW ══════════════════════ -->
          <transition-group
            v-else-if="viewMode === 'grid'"
            name="card-anim"
            tag="div"
            class="questions-grid"
          >
            <div
              v-for="(q, i) in filteredQuestions"
              :key="q.id"
              class="q-card campaign-card-modern"
              :style="{ '--card-delay': i * 0.04 + 's', '--type-color': getTypeInfo(q.type).color }"
            >
              <!-- TOP STRIPE -->
              <div class="card-type-stripe" :style="{ background: getTypeInfo(q.type).color }"></div>

              <!-- LANG BANNER -->
              <div class="card-lang-banner" :class="`lang-${resolveQuestionLang(q)}`">
                <span>{{ resolveQuestionLang(q) === 'en' ? '🇬🇧' : '🇫🇷' }}</span>
                <span class="lang-banner-name">{{ resolveQuestionLang(q) === 'en' ? 'English' : 'Français' }}</span>
              </div>

              <!-- CARD HEAD -->
              <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                <div class="card-cat-pill">
                  <i class="fa-solid fa-folder-open me-1"></i>
                  <span>{{ q.theme || 'Non classé' }}</span>
                </div>
                <div class="d-flex gap-2 align-items-center">
                  <button class="btn-icon-sm" @click="openModal(q)" title="Modifier">
                    <i class="fa-solid fa-pen-to-square"></i>
                  </button>
                  <button class="btn-icon-sm danger" @click="handleDelete(q.id)" title="Supprimer">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>

              <!-- TYPE BADGE -->
              <div class="type-badge-row" :style="{ '--badge-c': getTypeInfo(q.type).color }">
                <div class="type-badge-icon-box" :style="{ background: getTypeInfo(q.type).color + '18', border: '1px solid ' + getTypeInfo(q.type).color + '35' }">
                  <i :class="getTypeInfo(q.type).icon" :style="{ color: getTypeInfo(q.type).color }"></i>
                </div>
                <span class="type-badge-label" :style="{ color: getTypeInfo(q.type).color }">{{ getTypeInfo(q.type).label }}</span>
                <span class="type-badge-live" :style="{ background: getTypeInfo(q.type).color }"></span>
              </div>

              <!-- ÉNONCÉ -->
              <p class="card-enonce">{{ q.enonce }}</p>

              <!-- OPTIONS PREVIEW -->
              <div v-if="q.choix && q.choix.length > 0" class="card-options-preview">
                <div class="opts-preview-header">
                  <i class="fa-solid fa-list-check text-amber me-1"></i>
                  <span>{{ q.choix.length }} options</span>
                </div>
                <div class="opts-preview-list">
                  <div v-for="(opt, oi) in q.choix.slice(0, 3)" :key="oi" class="opt-preview-item">
                    <span class="opt-letter">{{ String.fromCharCode(65 + oi) }}</span>
                    <span class="opt-text">{{ opt }}</span>
                  </div>
                  <div v-if="q.choix.length > 3" class="opt-more">+{{ q.choix.length - 3 }} autres</div>
                </div>
              </div>

              <!-- CARD FOOTER -->
              <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3 border-top">
                <div class="level-indicator">
                  <span class="level-label-sm"><i class="fa-solid fa-signal me-1"></i>NIV.</span>
                  <div class="level-dots">
                    <span
                      v-for="d in 5" :key="d"
                      class="ldot"
                      :class="{ 'ldot-on': d <= q.points }"
                      :style="d <= q.points ? { background: getLevelColor(q.points), boxShadow: '0 0 4px ' + getLevelColor(q.points) } : {}"
                    ></span>
                  </div>
                  <span class="level-val" :style="{ color: getLevelColor(q.points) }">{{ q.points }}/5</span>
                </div>
                <span class="slot-badge" :style="{ background: getLevelColor(q.points) + '18', color: getLevelColor(q.points), border: '1px solid ' + getLevelColor(q.points) + '35' }">
                  <i class="fa-solid fa-gauge me-1"></i>Niv. {{ q.points }}
                </span>
              </div>
            </div>
          </transition-group>

          <!-- ══════════════════════ LIST VIEW ══════════════════════ -->
          <div v-else class="list-view-pro">
            <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
              <span style="width:110px" class="list-col-label">TYPE</span>
              <span style="width:80px"  class="list-col-label">LANG</span>
              <span class="flex-grow-1 list-col-label">QUESTION</span>
              <span style="width:150px" class="list-col-label">CATÉGORIE</span>
              <span style="width:80px"  class="list-col-label text-center">NIVEAU</span>
              <span style="width:80px"  class="list-col-label text-center">ACTIONS</span>
            </div>
            <transition-group name="row-anim" tag="div">
              <div v-for="(q, i) in filteredQuestions" :key="q.id" class="list-row-item d-flex align-items-center px-4 py-3 mb-2" :style="{ '--row-delay': i * 0.02 + 's' }">
                <div style="width:110px">
                  <span class="row-type-badge" :style="{ color: getTypeInfo(q.type).color, background: getTypeInfo(q.type).color + '12', borderColor: getTypeInfo(q.type).color + '30' }">
                    <i :class="getTypeInfo(q.type).icon + ' me-1'"></i>
                    <span class="d-none d-xl-inline">{{ getTypeInfo(q.type).label }}</span>
                  </span>
                </div>
                <div style="width:80px">
                  <span :class="['row-lang-chip', resolveQuestionLang(q) === 'en' ? 'lc-en' : 'lc-fr']">
                    {{ resolveQuestionLang(q) === 'en' ? '🇬🇧 EN' : '🇫🇷 FR' }}
                  </span>
                </div>
                <div class="flex-grow-1 overflow-hidden pe-3">
                  <p class="fw-800 small mb-0 text-truncate">{{ q.enonce }}</p>
                  <div class="d-flex gap-1 mt-1 flex-wrap">
                    <span class="meta-chip" v-if="q.choix && q.choix.length">
                      <i class="fa-solid fa-list-check me-1"></i>{{ q.choix.length }} opts
                    </span>
                  </div>
                </div>
                <div style="width:150px">
                  <span class="meta-chip"><i class="fa-solid fa-folder-open me-1"></i>{{ q.theme || 'Non classé' }}</span>
                </div>
                <div style="width:80px" class="text-center">
                  <span class="slot-badge" :style="{ background: getLevelColor(q.points) + '18', color: getLevelColor(q.points), border: '1px solid ' + getLevelColor(q.points) + '35' }">
                    {{ q.points }}/5
                  </span>
                </div>
                <div style="width:80px" class="d-flex gap-2 justify-content-center">
                  <button class="btn-icon-sm" @click="openModal(q)"><i class="fa-solid fa-pen-to-square"></i></button>
                  <button class="btn-icon-sm danger" @click="handleDelete(q.id)"><i class="fa-solid fa-trash-can"></i></button>
                </div>
              </div>
            </transition-group>
          </div>

        </div>
      </main>
    </div>

    <!-- ══════════════════════════════════════════════════════════════
         MODAL IA BILINGUE
    ══════════════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="showAIModal" class="quantum-vault-overlay" @click.self="showAIModal = false">
        <div class="quantum-vault-window modal-md">
          <!-- Corner decorators -->
          <div class="modal-corner tl"></div><div class="modal-corner tr"></div>
          <div class="modal-corner bl"></div><div class="modal-corner br"></div>

          <div class="qv-header">
            <div class="d-flex align-items-center gap-3 flex-grow-1">
              <div class="icon-box-v2 amber" style="position:relative">
                <div class="icon-pulse-ring"></div>
                <i class="fa-solid fa-wand-magic-sparkles"></i>
              </div>
              <div>
                <h5 class="fw-900 m-0">Génération <em class="text-amber">Bilingue</em> par IA</h5>
                <p class="small text-muted m-0"><i class="fa-solid fa-microchip me-1"></i>Moteur Gemini · Questions FR &amp; EN</p>
              </div>
            </div>
            <button class="btn-modal-close" @click="showAIModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="modal-body-scroll fancy-scroll p-4">
            <!-- LANGUE -->
            <div class="enigma-input-wrap mb-4">
              <label>LANGUE DE GÉNÉRATION</label>
              <div class="lang-cards-grid">
                <label v-for="l in langOptions" :key="l.val"
                  :class="['lang-card', { 'lang-card-active': aiForm.langue === l.val }]"
                  @click="aiForm.langue = l.val"
                >
                  <input type="radio" v-model="aiForm.langue" :value="l.val" style="display:none">
                  <span class="lc-flag">{{ l.flag }}</span>
                  <span class="lc-name">{{ l.name }}</span>
                  <span class="lc-desc">{{ l.desc }}</span>
                  <div class="lc-check"><i class="fa-solid fa-check"></i></div>
                </label>
              </div>
            </div>

            <!-- TYPE -->
            <div class="enigma-input-wrap mb-4">
              <label>FORMAT DE LA QUESTION</label>
              <div class="type-tiles-grid">
                <div
                  v-for="t in typeDefinitions"
                  :key="t.val"
                  :class="['type-tile', { 'type-tile-active': aiForm.type === t.val }]"
                  :style="aiForm.type === t.val ? { '--tile-c': t.color } : {}"
                  @click="aiForm.type = t.val"
                >
                  <div class="tile-icon-wrap" :style="{ color: t.color }">
                    <i :class="t.icon"></i>
                  </div>
                  <span class="tile-label">{{ t.label }}</span>
                  <div class="tile-check"><i class="fa-solid fa-check"></i></div>
                </div>
              </div>
            </div>

            <!-- CATÉGORIE + SOUS-THÈME -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>CATÉGORIE</label>
                  <div class="theme-select-wrapper">
                    <i class="fa-solid fa-folder theme-select-icon"></i>
                    <select v-model="aiForm.theme" class="enigma-field theme-select" @change="aiForm.sousTheme = ''">
                      <option value="">— Choisir —</option>
                      <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>SOUS-THÈME</label>
                  <div class="theme-select-wrapper" :class="{ 'disabled-wrapper': !aiForm.theme }">
                    <i class="fa-solid fa-tags theme-select-icon"></i>
                    <select v-model="aiForm.sousTheme" class="enigma-field theme-select" :disabled="!aiForm.theme">
                      <option value="">— Sélectionner —</option>
                      <option v-for="sub in aiDynamicSubCategories" :key="sub.id" :value="sub.nom">{{ sub.nom }}</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <!-- NOMBRE + DIFFICULTÉ -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>NOMBRE DE QUESTIONS</label>
                  <div class="number-stepper">
                    <button class="step-btn" @click="aiForm.n = Math.max(1, aiForm.n - 1)"><i class="fa-solid fa-minus"></i></button>
                    <input v-model.number="aiForm.n" type="number" min="1" max="20" class="step-input">
                    <button class="step-btn" @click="aiForm.n = Math.min(20, aiForm.n + 1)"><i class="fa-solid fa-plus"></i></button>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>DIFFICULTÉ</label>
                  <div class="d-flex gap-2">
                    <button
                      v-for="d in difficultyLevels"
                      :key="d.val"
                      class="diff-btn flex-grow-1"
                      :class="{ 'diff-btn-active': aiForm.difficulty === d.val }"
                      :style="aiForm.difficulty === d.val ? { background: d.color, borderColor: d.color, color: '#fff' } : {}"
                      @click="aiForm.difficulty = d.val"
                    >
                      <i :class="d.icon + ' me-1'"></i>{{ d.label }}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- PROGRESS -->
            <transition name="fade-up">
              <div v-if="isAILoading" class="ai-progress-box mb-4">
                <div class="ai-prog-track"><div class="ai-prog-fill" :style="{ width: aiProgress + '%' }"></div></div>
                <div class="ai-prog-text mt-2"><i class="fa-solid fa-circle-notch fa-spin me-2"></i>{{ aiStatusText }}</div>
              </div>
            </transition>

            <!-- APERÇU -->
            <transition name="fade-up">
              <div v-if="aiPreview.length > 0 && !isAILoading" class="ai-preview-box">
                <div class="preview-header">
                  <i class="fa-solid fa-eye me-2 text-amber"></i>
                  <span>{{ aiPreview.length }} QUESTIONS GÉNÉRÉES</span>
                  <span class="ms-2 row-lang-chip lc-fr">🇫🇷 {{ aiPreview.filter(q => resolvePreviewLang(q) === 'fr').length }}</span>
                  <span class="ms-1 row-lang-chip lc-en">🇬🇧 {{ aiPreview.filter(q => resolvePreviewLang(q) === 'en').length }}</span>
                  <button class="btn-clear-search ms-auto" @click="aiPreview = []"><i class="fa-solid fa-xmark"></i></button>
                </div>
                <div class="preview-list fancy-scroll">
                  <div v-for="(pq, pi) in aiPreview" :key="pi" class="preview-item">
                    <span class="preview-num">{{ pi + 1 }}</span>
                    <div class="preview-content">
                      <p class="preview-q">{{ pq.question }}</p>
                      <div v-if="pq.options && pq.options.length" class="d-flex flex-wrap gap-1 mt-1">
                        <span v-for="(opt, oi) in pq.options.slice(0, 4)" :key="oi" class="meta-chip">
                          {{ String.fromCharCode(65+oi) }}. {{ opt }}
                        </span>
                      </div>
                    </div>
                    <span :class="['row-lang-chip', resolvePreviewLang(pq) === 'en' ? 'lc-en' : 'lc-fr']">
                      {{ resolvePreviewLang(pq) === 'en' ? '🇬🇧' : '🇫🇷' }}
                    </span>
                  </div>
                </div>
              </div>
            </transition>
          </div>

          <div class="modal-footer-actions">
            <button class="btn-qv-cancel" @click="showAIModal = false"><i class="fa-solid fa-xmark me-2"></i>Annuler</button>
            <button class="btn-outline-pro" :disabled="isAILoading || !aiForm.theme || !aiForm.sousTheme" @click="previewAI">
              <i class="fa-solid fa-eye me-2"></i>Aperçu
            </button>
            <button class="btn-enigma-primary" :disabled="isAILoading || !aiForm.theme || !aiForm.sousTheme" @click="handleAIGeneration">
              <div class="btn-content">
                <i v-if="isAILoading" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                <i v-else class="fa-solid fa-wand-magic-sparkles me-2"></i>
                {{ isAILoading ? 'Génération...' : 'Lancer le moteur IA' }}
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- ══════════════════════════════════════════════════════════════
         MODAL CATÉGORIES
    ══════════════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="showCatManager" class="quantum-vault-overlay" @click.self="showCatManager = false">
        <div class="quantum-vault-window modal-md">
          <div class="modal-corner tl"></div><div class="modal-corner tr"></div>
          <div class="modal-corner bl"></div><div class="modal-corner br"></div>

          <div class="qv-header">
            <div class="d-flex align-items-center gap-3 flex-grow-1">
              <div class="icon-box-v2" style="background:linear-gradient(135deg,#7c3aed,#8b5cf6);color:#fff">
                <i class="fa-solid fa-sitemap"></i>
              </div>
              <div>
                <h5 class="fw-900 m-0">Gérer les <em class="text-amber">Catégories</em></h5>
                <p class="small text-muted m-0">Thèmes &amp; sous-thèmes du référentiel</p>
              </div>
            </div>
            <button class="btn-modal-close" @click="showCatManager = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="modal-body-scroll fancy-scroll p-4">
            <div class="d-flex gap-2 mb-4">
              <div class="search-inline-box flex-grow-1">
                <i class="fa-solid fa-folder-plus"></i>
                <input v-model="newCatName" @keyup.enter="addCategory" placeholder="Nom de la nouvelle catégorie..." class="search-inline-input">
              </div>
              <button class="btn-enigma-primary" @click="addCategory">
                <div class="btn-content"><i class="fa-solid fa-plus me-2"></i>Créer</div>
                <div class="btn-glow"></div>
              </button>
            </div>

            <div class="cats-grid">
              <transition-group name="card-anim">
                <div v-for="cat in categoriesList" :key="cat.id" class="cat-block">
                  <div class="cat-block-head">
                    <div class="d-flex align-items-center gap-2">
                      <i class="fa-solid fa-folder-open text-amber"></i>
                      <strong class="fw-800 small">{{ cat.nom }}</strong>
                      <span class="slot-badge" style="font-size:9px">{{ cat.sousCategories?.length || 0 }}</span>
                    </div>
                    <button class="btn-icon-sm danger" @click="removeCategory(cat.id)">
                      <i class="fa-solid fa-trash-can"></i>
                    </button>
                  </div>
                  <div class="sub-chips">
                    <div v-for="sub in cat.sousCategories" :key="sub.id" class="sub-chip">
                      <i class="fa-solid fa-tag text-amber" style="font-size:9px"></i>
                      <span>{{ sub.nom }}</span>
                      <button class="sub-chip-del" @click="removeSubCategory(sub.id)">
                        <i class="fa-solid fa-xmark"></i>
                      </button>
                    </div>
                  </div>
                  <div class="sub-add-row">
                    <input v-model="subCatInputs[cat.id]" @keyup.enter="handleSubAdd(cat.id)" placeholder="Ajouter un sous-thème..." class="sub-input">
                    <button class="sub-add-btn" @click="handleSubAdd(cat.id)"><i class="fa-solid fa-plus"></i></button>
                  </div>
                </div>
              </transition-group>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- ══════════════════════════════════════════════════════════════
         MODAL ÉDITION / CRÉATION
    ══════════════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="showModal" class="quantum-vault-overlay" @click.self="showModal = false">
        <div class="quantum-vault-window modal-md">
          <div class="modal-corner tl"></div><div class="modal-corner tr"></div>
          <div class="modal-corner bl"></div><div class="modal-corner br"></div>

          <div class="qv-header">
            <div class="d-flex align-items-center gap-3 flex-grow-1">
              <div class="icon-box-v2 amber">
                <i class="fa-solid fa-pen-ruler"></i>
              </div>
              <div>
                <h5 class="fw-900 m-0">
                  {{ isEdit ? 'Modifier' : 'Créer' }} une <em class="text-amber">Question</em>
                </h5>
                <p class="small text-muted m-0">{{ isEdit ? 'Mise à jour du référentiel' : 'Nouvelle entrée dans la banque' }}</p>
              </div>
            </div>
            <button class="btn-modal-close" @click="showModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="modal-body-scroll fancy-scroll p-4">
            <!-- TYPE -->
            <div class="enigma-input-wrap mb-4">
              <label>FORMAT DE LA QUESTION</label>
              <div class="type-tiles-grid">
                <div
                  v-for="t in typeDefinitions"
                  :key="t.val"
                  :class="['type-tile', { 'type-tile-active': form.type === t.val }]"
                  :style="form.type === t.val ? { '--tile-c': t.color } : {}"
                  @click="handleTypeChange(t.val)"
                >
                  <div class="tile-icon-wrap" :style="{ color: t.color }">
                    <i :class="t.icon"></i>
                  </div>
                  <span class="tile-label">{{ t.label }}</span>
                  <div class="tile-check"><i class="fa-solid fa-check"></i></div>
                </div>
              </div>
            </div>

            <!-- LANGUE -->
            <div class="enigma-input-wrap mb-4">
              <label>LANGUE</label>
              <div class="d-flex gap-2">
                <button :class="['lang-toggle-btn flex-grow-1', { active: form.langue === 'fr' }]" @click="form.langue = 'fr'">
                  🇫🇷 Français
                </button>
                <button :class="['lang-toggle-btn flex-grow-1', { active: form.langue === 'en' }]" @click="form.langue = 'en'">
                  🇬🇧 English
                </button>
              </div>
            </div>

            <!-- ÉNONCÉ -->
            <div class="enigma-input-wrap mb-4">
              <label>ÉNONCÉ *</label>
              <div style="position:relative">
                <textarea
                  v-model="form.enonce"
                  class="enigma-field"
                  rows="3"
                  :placeholder="form.langue === 'en' ? 'Enter the question...' : 'Saisir la problématique...'"
                ></textarea>
                <span class="char-counter">{{ form.enonce.length }}</span>
              </div>
            </div>

            <!-- CATÉGORIE + SOUS-THÈME -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>CATÉGORIE</label>
                  <div class="theme-select-wrapper">
                    <i class="fa-solid fa-folder theme-select-icon"></i>
                    <select v-model="form.theme" class="enigma-field theme-select">
                      <option value="">Sélectionner...</option>
                      <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>SOUS-THÈME</label>
                  <div class="theme-select-wrapper" :class="{ 'disabled-wrapper': !form.theme }">
                    <i class="fa-solid fa-tags theme-select-icon"></i>
                    <select v-model="form.sousTheme" class="enigma-field theme-select" :disabled="!form.theme">
                      <option value="">Aucun</option>
                      <option v-for="sub in dynamicSubCategories" :key="sub.id" :value="sub.nom">{{ sub.nom }}</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <!-- NIVEAU -->
            <div class="enigma-input-wrap mb-4">
              <label class="d-flex justify-content-between">
                <span>NIVEAU DE COMPLEXITÉ</span>
                <span :style="{ color: getLevelColor(form.points) }"><i class="fa-solid fa-signal me-1"></i>{{ form.points }} / 5</span>
              </label>
              <div class="admissibility-dashboard">
                <input type="range" min="1" max="5" step="1" v-model.number="form.points" class="enigma-range"
                  :style="{ '--rng-c': getLevelColor(form.points), '--rng-pct': ((form.points-1)/4*100) + '%' }">
                <div class="d-flex justify-content-between mt-2">
                  <span class="score-tier tier-low">Débutant</span>
                  <span class="score-tier tier-mid">Standard</span>
                  <span class="score-tier tier-high">Expert</span>
                </div>
              </div>
            </div>

            <!-- OPTIONS -->
            <div class="enigma-input-wrap mb-4" v-if="[0,1,2].includes(form.type)">
              <div class="d-flex justify-content-between align-items-center mb-3 pb-2" style="border-bottom:1px solid var(--bdr)">
                <label class="m-0">
                  <i :class="getTypeInfo(form.type).icon + ' me-2'" :style="{ color: getTypeInfo(form.type).color }"></i>
                  {{ form.langue === 'en' ? 'ANSWER OPTIONS' : 'OPTIONS DE RÉPONSE' }}
                </label>
                <button v-if="form.type !== 2" @click="addResponse" class="btn-bank-action-v2">
                  <i class="fa-solid fa-plus me-1"></i>Ajouter
                </button>
              </div>
              <div class="d-flex flex-column gap-2">
                <div v-for="(rep, idx) in form.reponses" :key="idx" class="asset-card-v8">
                  <div class="drag-node-handle opt-check-area">
                    <input v-if="form.type === 1" type="checkbox" v-model="rep.estCorrecte" class="asset-checkbox" style="accent-color:#f59e0b">
                    <input v-else type="radio" :name="`r-${form.id||'new'}`" :value="idx" v-model="correctRadioIndex" class="asset-checkbox" style="accent-color:#f59e0b">
                  </div>
                  <input v-model="rep.texte" class="opt-input" :placeholder="`Option ${idx+1}...`">
                  <button v-if="form.type !== 2" class="btn-remove-v8" @click="removeResponse(idx)">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>
              </div>
              <p class="opts-hint mt-2">
                <i class="fa-solid fa-shield-halved me-2 text-amber"></i>
                Les bonnes réponses sont gérées en interne et ne sont pas affichées aux candidats.
              </p>
            </div>

            <!-- CODE / TEXTE LIBRE -->
            <div class="enigma-input-wrap mb-4" v-if="[4,5,6].includes(form.type)">
              <label>
                <i :class="(form.type === 5 ? 'fa-solid fa-terminal' : 'fa-solid fa-pen-to-square') + ' me-2'" :style="{ color: getTypeInfo(form.type).color }"></i>
                {{ form.type === 5 ? 'CODE DE RÉFÉRENCE' : (form.langue === 'en' ? 'EXPECTED ANSWER' : 'RÉPONSE ATTENDUE') }}
              </label>
              <div class="code-box">
                <div class="code-titlebar">
                  <div class="code-dots"><span class="cd-red"></span><span class="cd-amber"></span><span class="cd-green"></span></div>
                  <span class="code-fname">
                    <i :class="getTypeInfo(form.type).icon + ' me-1'" :style="{ color: getTypeInfo(form.type).color }"></i>
                    {{ form.type === 5 ? 'solution.js' : 'answer.txt' }}
                  </span>
                </div>
                <textarea v-model="form.bonneReponse" class="code-area" :rows="form.type === 5 ? 10 : 5" :placeholder="getPlaceholder(form.type, form.langue)"></textarea>
              </div>
            </div>
          </div>

          <div class="modal-footer-actions">
            <button class="btn-qv-cancel" @click="showModal = false">
              <i class="fa-solid fa-xmark me-2"></i>Fermer
            </button>
            <button class="btn-enigma-primary" :disabled="isSaving" @click="save">
              <div class="btn-content">
                <i v-if="isSaving" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                <i v-else class="fa-solid fa-floppy-disk me-2"></i>
                {{ isSaving ? 'Enregistrement...' : 'Sauvegarder' }}
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>
        </div>
      </div>
    </transition>

    <!-- ══════════════════════ TOAST ══════════════════════ -->
    <transition name="toast-slide">
      <div v-if="toast.active" :class="['enigma-toast', `t-${toast.type}`]">
        <div class="t-ico"><i :class="toast.icon"></i></div>
        <div class="t-body">
          <strong>{{ toast.type === 'success' ? 'SUCCÈS' : toast.type === 'error' ? 'ERREUR' : 'INFO' }}</strong>
          <p class="m-0 small">{{ toast.message }}</p>
        </div>
        <button class="btn-clear-search ms-2" @click="toast.active = false"><i class="fa-solid fa-xmark"></i></button>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import api from '@/services/api';
import axios from 'axios';
import Swal from 'sweetalert2';
import { useRouter } from 'vue-router';

const router = useRouter();

const AI_BASE = 'http://127.0.0.1:8000';

/* ═══ TYPES ═══ */
const typeDefinitions = [
  { val: 0, label: 'Choix unique',   icon: 'fa-solid fa-circle-dot',   color: '#3b82f6' },
  { val: 1, label: 'Choix multiple', icon: 'fa-solid fa-square-check', color: '#8b5cf6' },
  { val: 2, label: 'Vrai / Faux',    icon: 'fa-solid fa-toggle-on',    color: '#10b981' },
  { val: 4, label: 'Texte libre',    icon: 'fa-solid fa-robot',        color: '#f59e0b' },
  { val: 5, label: 'Code source',    icon: 'fa-solid fa-code',         color: '#06b6d4' },
  { val: 6, label: 'Projet',         icon: 'fa-solid fa-folder-open',  color: '#ef4444' },
];

const difficultyLevels = [
  { val: 1, label: 'Débutant',      color: '#10b981', icon: 'fa-solid fa-seedling' },
  { val: 2, label: 'Intermédiaire', color: '#f59e0b', icon: 'fa-solid fa-fire-flame-curved' },
  { val: 3, label: 'Avancé',        color: '#ef4444', icon: 'fa-solid fa-skull-crossbones' },
];

const langOptions = [
  { val: 'fr',   flag: '🇫🇷', name: 'Français', desc: 'Questions en français' },
  { val: 'en',   flag: '🇬🇧', name: 'English',  desc: 'Questions in English' },
  { val: 'both', flag: '🌐', name: 'Bilingue',  desc: 'FR + EN simultanément' },
];

/* ═══ STATE ═══ */
const questions      = ref([]);
const categoriesList = ref([]);
const loading        = ref(true);
const isSaving       = ref(false);
const isAILoading    = ref(false);
const showModal      = ref(false);
const showCatManager = ref(false);
const showAIModal    = ref(false);
const isEdit         = ref(false);
const isDark         = ref(false);
const searchQuery    = ref('');
const searchFocused  = ref(false);
const activeFilter   = ref(-1);
const selectedCat    = ref('All');
const filterLang     = ref('all');
const viewMode       = ref('grid');
const newCatName     = ref('');
const aiPreview      = ref([]);
const aiProgress     = ref(0);
const aiStatusText   = ref('Initialisation...');
const mousePos       = reactive({ x: 0, y: 0 });
const subCatInputs   = reactive({});

const toast  = reactive({ active: false, message: '', type: 'success', icon: '' });
const aiForm = reactive({ theme: '', sousTheme: '', n: 5, langue: 'fr', type: 0, difficulty: 2 });
const form   = reactive({
  id: '', enonce: '', type: 0, points: 1,
  theme: '', sousTheme: '', reponses: [], bonneReponse: '', langue: 'fr'
});

/* ═══ LANGUAGE ═══ */
const resolveQuestionLang = (q) => {
  if (q.langue === 'en' || q.langue === 'fr') return q.langue;
  if (q.lang === 'en' || q.lang === 'fr') return q.lang;
  const t  = (q.enonce || '').toLowerCase();
  const en = ['what','which','how','when','where','why','is ','are ','the ','this ','that '].filter(k => t.includes(k)).length;
  const fr = ['quel','comment','pourquoi','les ','des ','est ','sont '].filter(k => t.includes(k)).length;
  return en > fr ? 'en' : 'fr';
};
const resolvePreviewLang = (pq) => {
  if (pq.langue === 'en' || pq.langue === 'fr') return pq.langue;
  if (pq.lang === 'en' || pq.lang === 'fr') return pq.lang;
  return 'fr';
};

/* ═══ COMPUTED ═══ */
const dynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === form.theme);
  return cat ? cat.sousCategories : [];
});
const aiDynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === aiForm.theme);
  return cat ? cat.sousCategories : [];
});
const correctRadioIndex = computed({
  get: () => form.reponses.findIndex(r => r.estCorrecte),
  set: (idx) => form.reponses.forEach((r, i) => { r.estCorrecte = (i === idx); })
});
const filteredQuestions = computed(() =>
  questions.value.filter(q => {
    const ms = !searchQuery.value || q.enonce?.toLowerCase().includes(searchQuery.value.toLowerCase());
    const mt = activeFilter.value === -1 || q.type === activeFilter.value;
    const mc = selectedCat.value === 'All' || q.theme === selectedCat.value;
    const ml = filterLang.value === 'all' || resolveQuestionLang(q) === filterLang.value;
    return ms && mt && mc && ml;
  })
);
const countByLang = (lang) => questions.value.filter(q => resolveQuestionLang(q) === lang).length;
const countByType = (val)  => questions.value.filter(q => q.type === val).length;

const kpiStats = computed(() => [
  { label: 'Total Questions', value: questions.value.length,                                     icon: 'fa-solid fa-database',      color: '#f59e0b', bg: '#fffbeb', trend: 8 },
  { label: 'Catégories',      value: categoriesList.value.length,                                icon: 'fa-solid fa-sitemap',        color: '#6366f1', bg: '#eef2ff' },
  { label: 'Niveau Expert',   value: questions.value.filter(x => x.points >= 4).length,          icon: 'fa-solid fa-bolt-lightning', color: '#ef4444', bg: '#fff1f2' },
  { label: 'En Français',     value: countByLang('fr'),                                          icon: 'fa-solid fa-flag',           color: '#10b981', bg: '#ecfdf5' },
  { label: 'En Anglais',      value: countByLang('en'),                                          icon: 'fa-solid fa-earth-americas', color: '#8b5cf6', bg: '#f5f3ff' },
]);

/* ═══ API ═══ */
const fetchData = async () => {
  loading.value = true;
  try {
    const [resQ, resC] = await Promise.all([api.get('/Questions'), api.get('/Categories')]);
    questions.value     = resQ.data;
    categoriesList.value = resC.data;
  } catch { showToast('Erreur de connexion', 'error'); }
  finally { loading.value = false; }
};

/* ═══ IA GÉNÉRATION ═══ */
const simulateProgress = () => {
  aiProgress.value = 0;
  const steps = [[15,'Connexion Gemini...'],[35,'Analyse thématique...'],[55,'Génération...'],[75,'Structuration...'],[92,'Sauvegarde...'],[100,'Terminé !']];
  let i = 0;
  const t = setInterval(() => {
    if (i < steps.length) { aiProgress.value = steps[i][0]; aiStatusText.value = steps[i][1]; i++; }
    else clearInterval(t);
  }, 400);
  return t;
};

const callAIAPI = async (lang) => {
  const fd = new FormData();
  fd.append('theme', aiForm.theme); fd.append('sousTheme', aiForm.sousTheme);
  fd.append('type', aiForm.type);   fd.append('n', aiForm.n); fd.append('langue', lang);
  try {
    const r = await axios.post(`${AI_BASE}/ia/generate-bilingual`, fd);
    return (r.data.questions || []).map(q => ({ ...q, lang, langue: lang }));
  } catch {
    const fd2 = new FormData();
    fd2.append('theme', aiForm.theme); fd2.append('sousTheme', aiForm.sousTheme);
    fd2.append('n', aiForm.n); fd2.append('langue', lang);
    const r2 = await axios.post(`${AI_BASE}/ia/generate-ultra`, fd2);
    return (r2.data.questions || []).map(q => ({ ...q, lang, langue: lang }));
  }
};

const previewAI = async () => {
  if (!aiForm.theme || !aiForm.sousTheme) { showToast('Choisissez une catégorie et sous-thème', 'error'); return; }
  isAILoading.value = true; aiPreview.value = [];
  const t = simulateProgress();
  try {
    let res = aiForm.langue === 'both'
      ? [...await callAIAPI('fr'), ...await callAIAPI('en')]
      : await callAIAPI(aiForm.langue);
    aiPreview.value = res;
    showToast(`${res.length} questions générées en aperçu`, 'success');
  } catch { showToast('Erreur prévisualisation', 'error'); }
  finally { clearInterval(t); isAILoading.value = false; aiProgress.value = 0; }
};

const handleAIGeneration = async () => {
  if (!aiForm.theme || !aiForm.sousTheme) { showToast('Choisissez une catégorie et sous-thème', 'error'); return; }

  // 🛡️ VÉRIFICATION QUOTA (Limite de 2/jour)
  try {
    await api.post('/Usage/validate-action');
  } catch (err) {
    if (err.response && err.response.status === 403) {
      showAIModal.value = false;
      const secondsLeft = err.response.data.retryAfterSeconds || 0;
      const h = Math.floor(secondsLeft / 3600);
      const m = Math.floor((secondsLeft % 3600) / 60);
      const s = secondsLeft % 60;
      const timeStr = h > 0 ? `${h}h ${m}m ${s}s` : `${m}m ${s}s`;

      Swal.fire({
        title: '<h2 style="font-size: 2.2rem; font-weight: 500; color: #1e293b; margin-top: 1.5rem;">Limite de génération atteinte</h2>',
        html: `
          <div style="padding: 1rem 2rem;">
            <p style="color: #64748b; font-size: 1.1rem; margin-bottom: 2rem;">Le plan Starter est limité à <b>5 générations</b>.</p>
            
            <div style="background-color: #fff1f2; border: 1px solid #fecaca; border-radius: 8px; padding: 1.5rem; margin-bottom: 2rem; display: flex; align-items: center; justify-content: center; gap: 15px;">
              <i class="fa-solid fa-rotate-left" style="color: #ef4444; font-size: 1.5rem;"></i>
              <span style="color: #be123c; font-size: 1.2rem; font-weight: 500;">Réessayez dans :</span>
              <span style="background-color: #ef4444; color: white; padding: 4px 12px; border-radius: 6px; font-weight: bold; font-size: 1.1rem;">${timeStr}</span>
            </div>

            <div style="background-color: #fffbeb; border: 1px solid #fde68a; border-radius: 8px; padding: 1.5rem; margin-bottom: 2rem;">
              <p style="color: #92400e; font-size: 1.05rem; margin: 0; line-height: 1.6;">
                Passez à <b style="color: #92400e;">EvaluaTech Go</b> pour supprimer ce délai et créer des questions illimitées.
              </p>
            </div>
          </div>
        `,
        showCancelButton: true,
        confirmButtonText: 'Passer à EvaluaTech Go',
        cancelButtonText: 'Plus tard',
        confirmButtonColor: '#eab308',
        cancelButtonColor: '#f1f5f9',
        background: '#fff',
        width: '600px',
        customClass: {
          popup: 'rounded-4 border-0 shadow-lg',
          confirmButton: 'btn-premium-confirm',
          cancelButton: 'btn-premium-cancel'
        },
        didOpen: () => {
          const confirmBtn = Swal.getConfirmButton();
          const cancelBtn = Swal.getCancelButton();
          if (confirmBtn) {
            confirmBtn.style.color = '#000';
            confirmBtn.style.fontWeight = 'bold';
            confirmBtn.style.padding = '12px 30px';
            confirmBtn.style.borderRadius = '8px';
            confirmBtn.style.fontSize = '1.1rem';
          }
          if (cancelBtn) {
            cancelBtn.style.color = '#475569';
            cancelBtn.style.fontWeight = '500';
            cancelBtn.style.padding = '12px 30px';
            cancelBtn.style.borderRadius = '8px';
            cancelBtn.style.fontSize = '1.1rem';
            cancelBtn.style.backgroundColor = '#f1f5f9';
            cancelBtn.style.border = 'none';
          }
        }
      }).then(res => { if (res.isConfirmed) router.push('/pricing'); });
      return;
    }
  }

  isAILoading.value = true;
  const t = simulateProgress();
  const token = localStorage.getItem('token');
  try {
    let all = aiForm.langue === 'both'
      ? [...await callAIAPI('fr'), ...await callAIAPI('en')]
      : await callAIAPI(aiForm.langue);
    for (const q of all) {
      await api.post('/Questions', {
        enonce: q.question, type: aiForm.type, points: aiForm.difficulty,
        theme: aiForm.theme, sousTheme: aiForm.sousTheme, langue: q.langue,
        choix: q.options || [], bonneReponse: q.options && q.answer !== undefined ? q.options[q.answer] : ''
      }, { headers: { Authorization: `Bearer ${token}` } });
    }
    showAIModal.value = false; aiPreview.value = [];
    await fetchData();
    const fr = all.filter(q => q.langue === 'fr').length;
    const en = all.filter(q => q.langue === 'en').length;
    showToast(`${all.length} questions sauvegardées (${fr} 🇫🇷 / ${en} 🇬🇧)`, 'success');
  } catch { showToast('Erreur moteur IA', 'error'); }
  finally { clearInterval(t); isAILoading.value = false; aiProgress.value = 0; }
};

/* ═══ CATÉGORIES ═══ */
const addCategory = async () => {
  if (!newCatName.value.trim()) return;
  try {
    const res = await api.post('/Categories', { nom: newCatName.value });
    categoriesList.value.push(res.data); newCatName.value = '';
    showToast('Catégorie créée', 'success');
  } catch { showToast('Erreur création', 'error'); }
};
const handleSubAdd = async (catId) => {
  const val = subCatInputs[catId];
  if (!val?.trim()) return;
  try {
    const res = await api.post(`/Categories/${catId}/sub`, { nom: val.trim() });
    const cat = categoriesList.value.find(c => c.id === catId);
    cat?.sousCategories?.push(res.data);
    subCatInputs[catId] = '';
    showToast('Sous-thème ajouté', 'success');
  } catch { showToast('Erreur ajout', 'error'); }
};
const removeCategory = async (id) => {
  if (!confirm('Supprimer cette catégorie ?')) return;
  try {
    await api.delete(`/Categories/${id}`);
    categoriesList.value = categoriesList.value.filter(c => c.id !== id);
    showToast('Catégorie supprimée', 'info');
  } catch { showToast('Erreur suppression', 'error'); }
};
const removeSubCategory = async (subId) => {
  if (!confirm('Supprimer ce sous-thème ?')) return;
  try { await api.delete(`/Categories/sub/${subId}`); await fetchData(); showToast('Supprimé', 'info'); }
  catch { showToast('Erreur', 'error'); }
};

/* ═══ FORM ═══ */
const handleTypeChange = (newType) => {
  form.type = newType;
  if (newType === 2) form.reponses = [{ texte: form.langue === 'en' ? 'True' : 'Vrai', estCorrecte: true }, { texte: form.langue === 'en' ? 'False' : 'Faux', estCorrecte: false }];
  else if ([0,1].includes(newType)) { if (form.reponses.length < 2) form.reponses = [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }]; }
  else form.reponses = [];
};
const addResponse    = () => form.reponses.push({ texte: '', estCorrecte: false });
const removeResponse = (i) => form.reponses.splice(i, 1);

const openModal = (q = null) => {
  isEdit.value = !!q;
  if (q) {
    const clone = JSON.parse(JSON.stringify(q));
    Object.assign(form, clone);
    form.reponses = (q.choix || []).map(opt => ({ texte: opt, estCorrecte: (q.bonneReponse || '').split('|').includes(opt) }));
    form.langue   = resolveQuestionLang(q);
  } else {
    Object.assign(form, { id: '', enonce: '', type: 0, points: 1, theme: '', sousTheme: '', bonneReponse: '', langue: 'fr', reponses: [{ texte: '', estCorrecte: true }, { texte: '', estCorrecte: false }] });
  }
  showModal.value = true;
};

const save = async () => {
  if (!form.enonce.trim()) return;
  isSaving.value = true;
  try {
    let finalBR = '';
    if (form.type === 0 || form.type === 2) finalBR = form.reponses[correctRadioIndex.value]?.texte || '';
    else if (form.type === 1) finalBR = form.reponses.filter(r => r.estCorrecte).map(r => r.texte).join('|');
    else finalBR = form.bonneReponse;
    const payload = {
      enonce: form.enonce, type: form.type, points: form.points || 1,
      theme: form.theme, sousTheme: form.sousTheme, langue: form.langue,
      choix: form.reponses.map(r => r.texte).filter(t => t?.trim()),
      bonneReponse: finalBR
    };
    if (isEdit.value) await api.put(`/Questions/${form.id}`, payload);
    else              await api.post('/Questions', payload);
    showModal.value = false;
    await fetchData();
    showToast('Question enregistrée', 'success');
  } catch { showToast('Erreur sauvegarde', 'error'); }
  finally { isSaving.value = false; }
};

const handleDelete = async (id) => {
  if (!confirm('Supprimer cette question ?')) return;
  try { await api.delete(`/Questions/${id}`); await fetchData(); showToast('Question supprimée', 'info'); }
  catch { showToast('Erreur réseau', 'error'); }
};

/* ═══ UTILS ═══ */
const getTypeInfo    = (val) => typeDefinitions.find(t => t.val === val) || typeDefinitions[0];
const getLevelColor  = (p)  => p >= 4 ? '#ef4444' : p >= 3 ? '#f59e0b' : p >= 2 ? '#10b981' : '#3b82f6';
const getPlaceholder = (t, lang) => {
  if (t === 5) return lang === 'en' ? '// Enter reference code...' : '// Code de référence...';
  if (t === 4) return lang === 'en' ? 'Expected answer...' : 'Réponse attendue...';
  return lang === 'en' ? 'Answer...' : 'Réponse...';
};
const resetFilters = () => { searchQuery.value = ''; activeFilter.value = -1; selectedCat.value = 'All'; filterLang.value = 'all'; };

let _toastT = null;
const showToast = (message, type = 'success') => {
  clearTimeout(_toastT);
  toast.message = message; toast.type = type;
  toast.icon = type === 'success' ? 'fa-solid fa-circle-check' : type === 'error' ? 'fa-solid fa-circle-xmark' : 'fa-solid fa-circle-info';
  toast.active = true;
  _toastT = setTimeout(() => { toast.active = false; }, 3500);
};

const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(() => fetchData());
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;0,900;1,700&display=swap');

/* ══════════════════════════════════════════
   CSS CUSTOM PROPERTIES — Light Mode
══════════════════════════════════════════ */
.bq-root {
  --bg:          #f8fafc;
  --surface:     #ffffff;
  --surface2:    #f1f5f9;
  --bdr:         #eef2f6;
  --bdr2:        #cbd5e1;
  --text:        #0f172a;
  --text2:       #475569;
  --text3:       #94a3b8;
  --amber:       #f59e0b;
  --amber-light: #fffbeb;
  --amber-bdr:   #fde68a;
  --amber-dark:  #92400e;
  --shadow-sm:   0 1px 3px rgba(0,0,0,0.06), 0 4px 16px rgba(0,0,0,0.04);
  --shadow-md:   0 8px 30px rgba(0,0,0,0.08);
  --shadow-lg:   0 24px 60px rgba(0,0,0,0.1);
  --ease-spring: cubic-bezier(0.34, 1.56, 0.64, 1);
  --ease-out:    cubic-bezier(0.16, 1, 0.3, 1);

  font-family: 'Plus Jakarta Sans', sans-serif;
  background: var(--bg);
  min-height: 100vh;
  display: flex;
  position: relative;
  overflow-x: hidden;
  color: var(--text);
  transition: background 0.3s ease, color 0.3s ease;
}

*, *::before, *::after { box-sizing: border-box; }

/* ══════════════════════════════════════════
   DARK MODE — toutes les variables overridées
══════════════════════════════════════════ */
.bq-root.dark-mode {
  --bg:          #0d1117;
  --surface:     #161b22;
  --surface2:    #1c2128;
  --bdr:         rgba(255,255,255,0.08);
  --bdr2:        rgba(255,255,255,0.15);
  --text:        #f0f6fc;
  --text2:       #8b949e;
  --text3:       #6e7681;
  --amber-light: rgba(245,158,11,0.12);
  --amber-bdr:   rgba(245,158,11,0.3);
  --amber-dark:  #fbbf24;
  --shadow-sm:   0 1px 3px rgba(0,0,0,0.3), 0 4px 16px rgba(0,0,0,0.2);
  --shadow-md:   0 8px 30px rgba(0,0,0,0.4);
  --shadow-lg:   0 24px 60px rgba(0,0,0,0.5);
}

/* ── BACKGROUND ── */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.18;
}
.dark-mode .quantum-grid { opacity: 0.07; }
.glow-orb { position: absolute; width: 600px; height: 600px; filter: blur(120px); opacity: 0.12; border-radius: 50%; transition: transform 0.3s ease-out; }
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-slate { background: #6366f1; bottom: -200px; left: -100px; }
.dark-mode .glow-orb { opacity: 0.08; }

.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ── HEADER ── */
.bq-header { animation: slideDown 0.6s var(--ease-out) backwards; }
@keyframes slideDown { from { opacity: 0; transform: translateY(-24px); } to { opacity: 1; transform: none; } }

.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: var(--text3); }
.breadcrumb-pro .root { cursor: pointer; }
.breadcrumb-pro .root:hover { color: var(--amber); }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: var(--text); font-weight: 800; }
.premium-title { font-weight: 900; font-size: 2.2rem; letter-spacing: -1.5px; color: var(--text); margin: 0; line-height: 1.05; }
.gradient-text { background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent; font-style: italic; }
.brand-subtitle-v2 { font-size: 0.8rem; font-weight: 600; color: var(--text3); margin-top: 8px; }

/* LIVE DOT */
.live-dot-wrap { position: relative; display: inline-flex; align-items: center; justify-content: center; width: 18px; height: 18px; }
.live-dot { width: 8px; height: 8px; background: var(--amber); border-radius: 50%; }
.live-ring { position: absolute; inset: 0; border: 2px solid rgba(245,158,11,0.4); border-radius: 50%; animation: livePulse 2.2s ease-out infinite; }
@keyframes livePulse { 0% { transform: scale(0.5); opacity: 0.8; } 100% { transform: scale(1.6); opacity: 0; } }

/* ── BUTTONS ── */
.btn-refresh-pro {
  width: 44px; height: 44px; background: var(--surface);
  border: 1.5px solid var(--bdr); border-radius: 14px;
  color: var(--text2); font-size: 14px; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.2s; box-shadow: var(--shadow-sm);
}
.btn-refresh-pro:hover { background: var(--amber); color: #0f172a; border-color: var(--amber); transform: translateY(-2px); }

.search-inline-box {
  display: flex; align-items: center; gap: 10px;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 10px 14px;
  box-shadow: var(--shadow-sm); transition: all 0.25s;
  color: var(--text3);
}
.search-inline-box.focused { border-color: var(--amber); box-shadow: 0 0 0 4px rgba(245,158,11,0.1); }
.search-inline-input { border: none; background: none; outline: none; width: 170px; font-size: 13px; font-weight: 700; color: var(--text); font-family: inherit; }
.search-inline-input::placeholder { color: var(--text3); }
.btn-clear-search { border: none; background: none; color: var(--text3); cursor: pointer; padding: 0; font-size: 12px; transition: color 0.2s; }
.btn-clear-search:hover { color: #ef4444; }

.btn-outline-pro {
  display: flex; align-items: center;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 10px 18px;
  font-size: 13px; font-weight: 800; color: var(--text2);
  cursor: pointer; transition: all 0.22s; font-family: inherit;
  box-shadow: var(--shadow-sm);
}
.btn-outline-pro:hover { background: var(--text); color: var(--surface); transform: translateY(-2px); box-shadow: var(--shadow-md); }
.btn-outline-pro:disabled { opacity: 0.4; cursor: not-allowed; transform: none; }

/* AI BUTTON */
.btn-ai-glow {
  position: relative; overflow: hidden;
  display: flex; align-items: center;
  background: #0f172a; border: none; border-radius: 14px;
  padding: 10px 20px; font-size: 13px; font-weight: 800;
  color: #fff; cursor: pointer; font-family: inherit;
  box-shadow: 0 4px 20px rgba(15,23,42,0.2); transition: all 0.3s;
}
.btn-ai-glow:hover { transform: translateY(-3px); box-shadow: 0 12px 32px rgba(245,158,11,0.3); color: #0f172a; }
.btn-shine-layer { position: absolute; inset: 0; background: linear-gradient(135deg, var(--amber), #fbbf24); opacity: 0; transition: opacity 0.3s; z-index: 1; }
.btn-ai-glow:hover .btn-shine-layer { opacity: 1; }
.btn-ai-glow > *:not(.btn-shine-layer) { position: relative; z-index: 2; }
.lang-badge-pill { background: rgba(255,255,255,0.18); font-size: 9px; font-weight: 900; padding: 2px 8px; border-radius: 20px; letter-spacing: 0.5px; }

.btn-enigma-primary {
  background: #0f172a; color: #fff; border: none; padding: 10px 20px;
  border-radius: 14px; font-weight: 800; position: relative; overflow: hidden;
  cursor: pointer; font-family: inherit; transition: all 0.3s;
  display: flex; align-items: center;
  box-shadow: 0 4px 18px rgba(15,23,42,0.2);
}
.btn-enigma-primary .btn-glow { position: absolute; inset: 0; background: linear-gradient(135deg, var(--amber), #fbbf24); opacity: 0; transition: opacity 0.3s; }
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:hover { transform: translateY(-2px); box-shadow: 0 10px 28px rgba(245,158,11,0.3); }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; transform: none; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

/* ── STAT CARDS ── */
.stat-card-premium {
  background: var(--surface); border-radius: 24px; padding: 22px;
  display: flex; align-items: center; gap: 14px;
  border: 1.5px solid var(--bdr); transition: all 0.3s;
  box-shadow: var(--shadow-sm);
  animation: slideUp 0.5s var(--ease-out) backwards;
}
.stat-card-premium:hover { transform: translateY(-4px); box-shadow: var(--shadow-md); border-color: var(--amber-bdr); }
@keyframes slideUp { from { opacity: 0; transform: translateY(16px); } to { opacity: 1; transform: none; } }
.stat-icon-wrapper { width: 52px; height: 52px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.3rem; flex-shrink: 0; transition: transform 0.3s var(--ease-spring); }
.stat-card-premium:hover .stat-icon-wrapper { transform: scale(1.1) rotate(-5deg); }
.stat-value { font-size: 1.8rem; font-weight: 800; color: var(--text); display: block; line-height: 1; letter-spacing: -1px; }
.stat-label { font-size: 0.62rem; font-weight: 700; color: var(--text3); margin-top: 4px; display: block; text-transform: uppercase; letter-spacing: 0.8px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; gap: 2px; font-size: 0.6rem; font-weight: 800; padding: 6px 10px; border-radius: 10px; white-space: nowrap; }
.trend-up { color: #10b981; background: #ecfdf5; }

/* ── TOOLBAR ── */
.tabs-pill-wrap {
  background: var(--surface) !important;
  border-color: var(--bdr) !important;
}
.nav-tab-btn-modern {
  padding: 8px 14px; border-radius: 12px; border: none;
  background: transparent; font-weight: 800; font-size: 0.78rem;
  color: var(--text3); cursor: pointer; transition: all 0.2s; font-family: inherit;
  display: flex; align-items: center; gap: 6px;
}
.nav-tab-btn-modern:hover { background: var(--amber-light); color: var(--amber-dark); }
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.dark-mode .nav-tab-btn-modern.active { background: var(--amber); color: #0f172a; }
.tab-count { background: rgba(255,255,255,0.15); padding: 2px 7px; border-radius: 8px; font-size: 0.62rem; margin-left: 2px; }
.nav-tab-btn-modern:not(.active) .tab-count { background: var(--surface2); color: var(--text3); }

.sort-select-wrap {
  display: flex; align-items: center; gap: 8px;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 12px; padding: 9px 12px; transition: all 0.2s;
}
.sort-select-wrap:focus-within { border-color: var(--amber); }
.sort-ico { color: var(--text3); font-size: 11px; }
.sort-arrow { font-size: 9px; color: var(--text3); }
.sort-select-pro { border: none; background: none; outline: none; font-size: 11px; font-weight: 700; color: var(--text); font-family: inherit; cursor: pointer; }

.lang-cluster { display: flex; border: 1.5px solid var(--bdr); border-radius: 12px; overflow: hidden; background: var(--surface); }
.lang-tab { padding: 8px 12px; font-size: 11px; font-weight: 700; background: none; border: none; cursor: pointer; color: var(--text2); transition: all 0.2s; font-family: inherit; display: flex; align-items: center; gap: 5px; }
.lang-tab.active { background: var(--amber); color: #0f172a; }
.ltab-count { background: rgba(0,0,0,0.1); border-radius: 20px; padding: 1px 6px; font-size: 9px; font-weight: 900; }

.view-toggle-cluster { display: flex; background: var(--surface); border: 1.5px solid var(--bdr); border-radius: 12px; padding: 4px; gap: 3px; }
.btn-view-toggle { width: 38px; height: 36px; background: transparent; border: none; cursor: pointer; color: var(--text3); font-size: 13px; transition: all 0.2s; display: flex; align-items: center; justify-content: center; border-radius: 8px; }
.btn-view-toggle:hover { background: var(--surface2); color: var(--text); }
.btn-view-toggle.active { background: #0f172a; color: var(--amber); box-shadow: 0 4px 12px rgba(15,23,42,0.2); }
.dark-mode .btn-view-toggle.active { background: var(--amber); color: #0f172a; }

/* ── STATES ── */
.state-label { font-size: 11px; font-weight: 800; color: var(--text3); letter-spacing: 2px; display: flex; align-items: center; justify-content: center; gap: 8px; }
.spinner-pro-premium { width: 50px; height: 50px; border: 4px solid var(--bdr); border-top: 4px solid var(--amber); border-radius: 50%; animation: spin 1s linear infinite; margin: 40px auto 0; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state-pro { background: var(--surface); border-radius: 30px; padding: 40px; border: 1.5px dashed var(--bdr2); }

.empty-graphic { position: relative; width: 90px; height: 90px; margin: 0 auto; display: flex; align-items: center; justify-content: center; }
.empty-ring { position: absolute; inset: 0; border-radius: 50%; border: 1px solid var(--bdr); animation: haloSpin linear infinite; }
.r1 { animation-duration: 5s; } .r2 { inset: 12px; animation-duration: 8s; } .r3 { inset: 24px; animation-duration: 11s; }
@keyframes haloSpin { to { transform: rotate(360deg); } }
.empty-core { font-size: 30px; color: var(--amber); position: relative; z-index: 1; }

/* ── QUESTION GRID ── */
.questions-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 20px; }

.q-card.campaign-card-modern {
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 28px; overflow: hidden;
  display: flex; flex-direction: column;
  box-shadow: var(--shadow-sm); position: relative;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  animation: cardAppear 0.5s var(--ease-out) backwards;
  animation-delay: var(--card-delay, 0s);
  padding: 0;
}
.q-card.campaign-card-modern:hover {
  transform: translateY(-10px);
  border-color: var(--amber-bdr);
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08), 0 8px 30px rgba(245,158,11,0.12);
}
@keyframes cardAppear { from { opacity: 0; transform: translateY(20px) scale(0.97); } to { opacity: 1; transform: none; } }

.card-type-stripe { height: 3px; width: 100%; transition: height 0.3s; flex-shrink: 0; }
.q-card:hover .card-type-stripe { height: 4px; }

.card-lang-banner {
  display: flex; align-items: center; gap: 8px;
  padding: 6px 16px; font-size: 10px; font-weight: 800;
  letter-spacing: 0.5px; text-transform: uppercase;
}
.lang-fr { background: linear-gradient(90deg, rgba(254,226,226,0.8), transparent); color: #dc2626; border-bottom: 1px solid rgba(254,202,202,0.5); }
.lang-en { background: linear-gradient(90deg, rgba(239,246,255,0.8), transparent); color: #2563eb; border-bottom: 1px solid rgba(191,219,254,0.5); }
.dark-mode .lang-fr { background: linear-gradient(90deg, rgba(127,29,29,0.25), transparent); color: #fca5a5; border-bottom-color: rgba(127,29,29,0.3); }
.dark-mode .lang-en { background: linear-gradient(90deg, rgba(29,78,216,0.2), transparent); color: #93c5fd; border-bottom-color: rgba(29,78,216,0.25); }
.lang-banner-name { opacity: 0.8; }

.card-header-modern { display: flex; justify-content: space-between; align-items: center; padding: 12px 18px 0; }
.card-cat-pill {
  display: inline-flex; align-items: center; gap: 6px;
  background: var(--surface2); border: 1px solid var(--bdr);
  border-radius: 50px; padding: 5px 12px;
  font-size: 10px; font-weight: 700; color: var(--text2);
  max-width: 160px; overflow: hidden;
}
.card-cat-pill span { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid var(--bdr); background: var(--surface2);
  display: flex; align-items: center; justify-content: center;
  font-size: 11px; cursor: pointer; color: var(--text3); transition: all 0.2s;
}
.btn-icon-sm:hover { background: #3b82f6; color: #fff; border-color: #3b82f6; transform: scale(1.08); }
.btn-icon-sm.danger:hover { background: #ef4444; color: #fff; border-color: #ef4444; }

.type-badge-row {
  display: inline-flex; align-items: center; gap: 8px;
  padding: 6px 12px 6px 8px; margin: 10px 16px 10px;
  border: 1.5px solid var(--badge-c, var(--amber));
  border-left-width: 3px; border-radius: 10px;
  background: color-mix(in srgb, var(--badge-c, var(--amber)) 6%, var(--surface));
  width: fit-content; transition: transform 0.2s;
}
.q-card:hover .type-badge-row { transform: translateX(4px); }
.type-badge-icon-box { width: 26px; height: 26px; border-radius: 7px; display: flex; align-items: center; justify-content: center; font-size: 12px; flex-shrink: 0; }
.type-badge-label { font-size: 11px; font-weight: 800; }
.type-badge-live { width: 5px; height: 5px; border-radius: 50%; margin-left: auto; animation: dotBlink 2s ease-in-out infinite; }
@keyframes dotBlink { 0%,100% { opacity: 0.4; } 50% { opacity: 1; } }

.card-enonce {
  flex: 1; padding: 0 18px 14px;
  font-size: 14px; font-weight: 700; color: var(--text);
  line-height: 1.65; margin: 0;
  display: -webkit-box; -webkit-line-clamp: 3;
  -webkit-box-orient: vertical; overflow: hidden;
}

.card-options-preview { padding: 0 18px 14px; }
.opts-preview-header { display: flex; align-items: center; gap: 5px; font-size: 9px; font-weight: 800; color: var(--text3); text-transform: uppercase; letter-spacing: 0.8px; margin-bottom: 8px; }
.opts-preview-list { display: flex; flex-direction: column; gap: 4px; }
.opt-preview-item {
  display: flex; align-items: center; gap: 8px;
  background: var(--surface2); border: 1px solid var(--bdr);
  border-radius: 8px; padding: 5px 10px;
  font-size: 11px; font-weight: 600; color: var(--text2); transition: all 0.2s;
}
.opt-preview-item:hover { border-color: var(--amber-bdr); background: var(--amber-light); }
.opt-letter { width: 18px; height: 18px; border-radius: 5px; background: var(--bdr); color: var(--text3); font-size: 9px; font-weight: 900; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.opt-text { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.opt-more { font-size: 10px; color: var(--text3); font-style: italic; padding: 3px 8px; }

.card-footer-modern { padding: 10px 18px 16px; border-top-color: var(--bdr) !important; }
.level-indicator { display: flex; align-items: center; gap: 8px; }
.level-label-sm { font-size: 9px; font-weight: 800; color: var(--text3); letter-spacing: 0.8px; }
.level-dots { display: flex; gap: 4px; }
.ldot { width: 8px; height: 8px; border-radius: 50%; background: var(--bdr); transition: all 0.3s; }
.level-val { font-size: 12px; font-weight: 800; }
.slot-badge { padding: 4px 10px; border-radius: 20px; font-size: 10px; font-weight: 800; display: flex; align-items: center; }

/* ── LIST VIEW ── */
.list-view-pro { display: flex; flex-direction: column; }
.list-header-row { background: var(--surface2); border-radius: 14px; }
.list-col-label { font-size: 0.6rem; font-weight: 900; color: var(--text3); text-transform: uppercase; letter-spacing: 1px; }
.list-row-item {
  background: var(--surface); border-radius: 16px;
  border: 1.5px solid var(--bdr); transition: all 0.2s;
  animation: rowAppear 0.4s var(--ease-out) backwards;
  animation-delay: var(--row-delay, 0s);
}
.list-row-item:hover { border-color: var(--amber-bdr); transform: translateX(4px); box-shadow: 0 8px 24px rgba(245,158,11,0.1); }
@keyframes rowAppear { from { opacity: 0; transform: translateX(-16px); } to { opacity: 1; transform: none; } }

.row-type-badge { display: inline-flex; align-items: center; padding: 5px 10px; border-radius: 10px; border: 1px solid; font-size: 11px; font-weight: 700; flex-shrink: 0; }
.row-lang-chip { padding: 3px 9px; border-radius: 20px; font-size: 10px; font-weight: 800; }
.lc-fr { background: rgba(254,226,226,0.7); color: #dc2626; border: 1px solid rgba(254,202,202,0.7); }
.lc-en { background: rgba(239,246,255,0.7); color: #2563eb; border: 1px solid rgba(191,219,254,0.7); }
.dark-mode .lc-fr { background: rgba(127,29,29,0.3); color: #fca5a5; border-color: rgba(127,29,29,0.4); }
.dark-mode .lc-en { background: rgba(29,78,216,0.2); color: #93c5fd; border-color: rgba(29,78,216,0.3); }

.meta-chip {
  font-size: 10px; font-weight: 600; color: var(--text3);
  background: var(--surface2); padding: 2px 8px;
  border-radius: 50px; border: 1px solid var(--bdr);
  display: inline-flex; align-items: center;
}

/* ══════════════════════════════════════════
   MODALS
══════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; z-index: 1000;
  background: rgba(15,23,42,0.65);
  backdrop-filter: blur(16px) saturate(1.4);
  display: flex; align-items: center; justify-content: center; padding: 20px;
}
.quantum-vault-window {
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 32px; width: 100%; max-width: 860px;
  box-shadow: 0 40px 80px rgba(0,0,0,0.15); overflow: hidden; position: relative;
  display: flex; flex-direction: column; max-height: 90vh;
}
.modal-md { max-width: 760px; }

/* Corner decorators */
.modal-corner { position: absolute; width: 14px; height: 14px; pointer-events: none; z-index: 2; }
.tl { top: 8px; left: 8px;   border-top: 2px solid var(--amber); border-left: 2px solid var(--amber); border-radius: 4px 0 0 0; }
.tr { top: 8px; right: 8px;  border-top: 2px solid var(--amber); border-right: 2px solid var(--amber); border-radius: 0 4px 0 0; }
.bl { bottom: 8px; left: 8px;  border-bottom: 2px solid var(--amber); border-left: 2px solid var(--amber); border-radius: 0 0 0 4px; }
.br { bottom: 8px; right: 8px; border-bottom: 2px solid var(--amber); border-right: 2px solid var(--amber); border-radius: 0 0 4px 0; }

.qv-header {
  display: flex; align-items: center; gap: 16px;
  padding: 22px 28px; border-bottom: 1.5px solid var(--bdr);
  flex-shrink: 0;
}
.icon-box-v2 {
  width: 52px; height: 52px; border-radius: 16px; flex-shrink: 0;
  display: flex; align-items: center; justify-content: center; font-size: 22px; position: relative;
}
.icon-box-v2.amber { background: linear-gradient(135deg, var(--amber), #fbbf24); color: #0f172a; }
.icon-pulse-ring { position: absolute; inset: -4px; border-radius: 20px; border: 2px solid rgba(245,158,11,0.3); animation: iconPulse 2.5s ease infinite; }
@keyframes iconPulse { 0%,100% { transform: scale(1); opacity: 0.5; } 50% { transform: scale(1.15); opacity: 0; } }

.btn-modal-close {
  width: 38px; height: 38px; border-radius: 10px;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  color: var(--text3); font-size: 16px; cursor: pointer;
  display: flex; align-items: center; justify-content: center; transition: all 0.2s; flex-shrink: 0;
}
.btn-modal-close:hover { background: #fef2f2; color: #ef4444; border-color: #fca5a5; transform: rotate(90deg); }

.modal-body-scroll { overflow-y: auto; flex: 1; }
.modal-footer-actions {
  padding: 16px 28px; background: var(--surface2);
  border-top: 1.5px solid var(--bdr);
  display: flex; justify-content: flex-end; gap: 10px;
  flex-shrink: 0;
}

/* ══════════════════════════════════════════
   FORM ELEMENTS
══════════════════════════════════════════ */
.enigma-input-wrap label {
  display: flex; align-items: center; justify-content: space-between;
  font-size: 0.6rem; font-weight: 900; color: var(--text3);
  letter-spacing: 1.2px; text-transform: uppercase; margin-bottom: 10px;
}

.enigma-field {
  width: 100%; padding: 12px 16px;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 14px; font-weight: 600; outline: none;
  font-family: inherit; transition: all 0.2s; font-size: 13px; color: var(--text);
}
.enigma-field:focus { border-color: var(--amber); background: var(--surface); box-shadow: 0 0 0 4px rgba(245,158,11,0.1); }
.enigma-field:disabled { opacity: 0.4; cursor: not-allowed; }

.char-counter { position: absolute; bottom: 10px; right: 12px; font-size: 10px; color: var(--text3); font-weight: 600; pointer-events: none; }

/* LANG CARDS */
.lang-cards-grid { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 10px; }
.lang-card {
  background: var(--surface2); border: 2px solid var(--bdr);
  border-radius: 16px; padding: 16px 12px; text-align: center;
  cursor: pointer; transition: all 0.25s; position: relative;
}
.lang-card:hover { border-color: var(--amber-bdr); background: var(--surface); }
.lang-card-active { border-color: var(--amber); background: var(--amber-light); box-shadow: 0 4px 16px rgba(245,158,11,0.15); }
.lc-flag { font-size: 26px; margin-bottom: 6px; display: block; }
.lc-name { font-size: 13px; font-weight: 800; color: var(--text); margin-bottom: 3px; display: block; }
.lc-desc { font-size: 10px; color: var(--text3); }
.lc-check {
  position: absolute; top: 8px; right: 8px;
  width: 18px; height: 18px; border-radius: 50%;
  background: var(--amber); color: #0f172a; font-size: 9px;
  display: flex; align-items: center; justify-content: center;
  opacity: 0; transform: scale(0); transition: all 0.2s var(--ease-spring);
}
.lang-card-active .lc-check { opacity: 1; transform: scale(1); }

/* TYPE TILES */
.type-tiles-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(110px, 1fr)); gap: 8px; }
.type-tile {
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 14px 8px; text-align: center;
  cursor: pointer; transition: all 0.25s;
  display: flex; flex-direction: column; align-items: center; gap: 8px;
  position: relative; overflow: hidden;
}
.type-tile:hover { border-color: var(--amber-bdr); background: var(--amber-light); }
.type-tile-active { border-color: var(--tile-c, var(--amber)); background: var(--surface); box-shadow: 0 4px 16px rgba(245,158,11,0.1); }
.tile-icon-wrap {
  width: 38px; height: 38px; border-radius: 11px;
  display: flex; align-items: center; justify-content: center; font-size: 17px;
  background: color-mix(in srgb, currentColor 12%, transparent);
  transition: all 0.3s var(--ease-spring);
}
.type-tile:hover .tile-icon-wrap,
.type-tile-active .tile-icon-wrap { transform: scale(1.1); }
.tile-label { font-size: 10px; font-weight: 700; color: var(--text2); }
.tile-check {
  position: absolute; top: 6px; right: 6px;
  width: 16px; height: 16px; border-radius: 50%;
  background: var(--amber); color: #0f172a; font-size: 8px;
  display: flex; align-items: center; justify-content: center;
  opacity: 0; transform: scale(0); transition: all 0.2s var(--ease-spring);
}
.type-tile-active .tile-check { opacity: 1; transform: scale(1); }

/* SELECT WRAPPERS */
.theme-select-wrapper { position: relative; }
.theme-select-icon { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); color: var(--amber); font-size: 12px; z-index: 2; pointer-events: none; }
.theme-select { padding-left: 38px !important; appearance: none; -webkit-appearance: none; cursor: pointer; }
.disabled-wrapper .theme-select-icon { color: var(--text3); }
.disabled-wrapper { opacity: 0.5; pointer-events: none; }

/* LANG TOGGLE */
.lang-toggle-btn {
  padding: 10px; border-radius: 12px;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  font-size: 13px; font-weight: 700; cursor: pointer; color: var(--text2);
  transition: all 0.2s; font-family: inherit;
}
.lang-toggle-btn.active { background: var(--amber); color: #0f172a; border-color: var(--amber); font-weight: 800; }

/* RANGE SLIDER */
.admissibility-dashboard { background: var(--surface2); border-radius: 18px; padding: 20px; }
.enigma-range {
  width: 100%; height: 5px; appearance: none; cursor: pointer; outline: none;
  border-radius: 5px;
  background: linear-gradient(
    to right,
    var(--rng-c, var(--amber)) 0%,
    var(--rng-c, var(--amber)) var(--rng-pct, 0%),
    var(--bdr) var(--rng-pct, 0%)
  );
  transition: background 0.3s;
}
.enigma-range::-webkit-slider-thumb {
  appearance: none; width: 20px; height: 20px; border-radius: 50%;
  background: var(--surface); border: 3px solid var(--rng-c, var(--amber));
  box-shadow: 0 2px 8px rgba(0,0,0,0.15); cursor: pointer; transition: transform 0.2s;
}
.enigma-range::-webkit-slider-thumb:hover { transform: scale(1.2); }
.score-tier { font-size: 0.6rem; font-weight: 800; opacity: 0.5; }
.tier-low { color: #10b981; } .tier-mid { color: var(--amber); } .tier-high { color: #ef4444; }

/* STEPPER / NUMBER */
.number-stepper {
  display: flex; align-items: center;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 14px; overflow: hidden;
}
.step-btn {
  width: 44px; height: 46px; background: none; border: none;
  border-right: 1px solid var(--bdr); font-size: 13px;
  cursor: pointer; color: var(--text2); transition: all 0.2s;
  display: flex; align-items: center; justify-content: center;
}
.step-btn:last-child { border-right: none; border-left: 1px solid var(--bdr); }
.step-btn:hover { background: var(--amber); color: #0f172a; }
.step-input { flex: 1; border: none; background: none; outline: none; text-align: center; font-size: 16px; font-weight: 800; color: var(--text); font-family: inherit; }

/* DIFF BUTTONS */
.diff-btn {
  padding: 9px 6px; background: var(--surface2);
  border: 1.5px solid var(--bdr); border-radius: 12px;
  font-size: 11px; font-weight: 700; cursor: pointer;
  color: var(--text2); font-family: inherit;
  display: flex; align-items: center; justify-content: center; gap: 5px; transition: all 0.2s;
}

/* OPTIONS LIST */
.asset-card-v8 {
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 12px; padding: 10px 14px;
  display: flex; align-items: center; gap: 10px; transition: all 0.2s;
}
.asset-card-v8:hover { background: var(--surface); border-color: var(--bdr2); }
.asset-checkbox { width: 15px; height: 15px; cursor: pointer; }
.opt-check-area { flex-shrink: 0; }
.opt-input { flex: 1; border: none; background: none; outline: none; font-size: 13px; font-weight: 600; color: var(--text); font-family: inherit; }
.btn-remove-v8 { width: 26px; height: 26px; border-radius: 7px; border: none; background: none; color: var(--bdr2); cursor: pointer; font-size: 11px; transition: color 0.2s; }
.btn-remove-v8:hover { color: #ef4444; }
.drag-node-handle { cursor: default; }

.btn-bank-action-v2 {
  background: #0f172a; color: white; border: none;
  padding: 8px 14px; border-radius: 12px; font-weight: 800;
  font-size: 12px; cursor: pointer; font-family: inherit; transition: all 0.2s;
  display: flex; align-items: center;
}
.btn-bank-action-v2:hover { background: var(--amber); color: #0f172a; }

.opts-hint {
  display: flex; align-items: center; gap: 8px;
  font-size: 11px; color: var(--text3); font-weight: 600;
  background: var(--surface2); border: 1px dashed var(--bdr2);
  border-radius: 10px; padding: 8px 12px; margin-top: 8px;
}

/* CODE BOX */
.code-box { border-radius: 16px; overflow: hidden; border: 1.5px solid var(--bdr); }
.code-titlebar { background: #0f172a; padding: 10px 16px; display: flex; align-items: center; justify-content: space-between; }
.code-dots { display: flex; gap: 5px; }
.cd-red   { width: 10px; height: 10px; border-radius: 50%; background: #ef4444; }
.cd-amber { width: 10px; height: 10px; border-radius: 50%; background: var(--amber); }
.cd-green { width: 10px; height: 10px; border-radius: 50%; background: #22c55e; }
.code-fname { font-size: 10px; color: #4a6090; font-family: monospace; display: flex; align-items: center; gap: 7px; }
.code-area { width: 100%; background: #0d1829; border: none; padding: 16px 18px; color: #7dd3fc; font-family: monospace; font-size: 12.5px; resize: vertical; outline: none; line-height: 1.7; }
.code-area::placeholder { color: #2d4a6e; }

/* ── CATÉGORIES MODAL ── */
.cats-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.cat-block { background: var(--surface2); border: 1.5px solid var(--bdr); border-radius: 16px; padding: 16px; transition: all 0.25s; }
.cat-block:hover { border-color: var(--amber-bdr); }
.cat-block-head { display: flex; justify-content: space-between; align-items: center; margin-bottom: 12px; }
.sub-chips { display: flex; flex-wrap: wrap; gap: 6px; margin-bottom: 12px; }
.sub-chip { display: flex; align-items: center; gap: 6px; background: var(--surface); border: 1px solid var(--bdr); border-radius: 50px; padding: 4px 10px; font-size: 11px; font-weight: 600; color: var(--text2); }
.sub-chip-del { background: none; border: none; cursor: pointer; color: var(--text3); font-size: 10px; transition: color 0.15s; padding: 0; }
.sub-chip-del:hover { color: #ef4444; }
.sub-add-row { display: flex; gap: 7px; border-top: 1px dashed var(--bdr); padding-top: 10px; }
.sub-input {
  flex: 1; border: 1.5px solid var(--bdr); border-radius: 10px;
  padding: 7px 12px; font-size: 12px; font-weight: 600;
  outline: none; background: var(--surface); color: var(--text);
  transition: border-color 0.2s; font-family: inherit;
}
.sub-input:focus { border-color: var(--amber); }
.sub-add-btn { width: 32px; height: 32px; border-radius: 9px; background: var(--amber); color: #0f172a; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 13px; transition: all 0.2s; }
.sub-add-btn:hover { transform: scale(1.1); }

/* ── AI PROGRESS ── */
.ai-progress-box {
  background: var(--amber-light); border: 1.5px solid var(--amber-bdr);
  border-radius: 16px; padding: 18px; text-align: center;
}
.ai-prog-track { height: 4px; background: rgba(245,158,11,0.25); border-radius: 4px; overflow: hidden; }
.ai-prog-fill { height: 100%; background: linear-gradient(90deg, var(--amber), #fbbf24); border-radius: 4px; transition: width 0.4s var(--ease-out); min-width: 8px; }
.ai-prog-text { font-size: 11px; font-weight: 800; color: var(--amber-dark); display: flex; align-items: center; justify-content: center; gap: 8px; }

/* ── AI PREVIEW ── */
.ai-preview-box { background: var(--surface2); border: 1.5px solid var(--bdr); border-radius: 16px; overflow: hidden; }
.preview-header { display: flex; align-items: center; gap: 6px; padding: 10px 14px; background: var(--surface); border-bottom: 1px solid var(--bdr); font-size: 10px; font-weight: 800; color: var(--text3); letter-spacing: 1px; }
.preview-list { padding: 10px; display: flex; flex-direction: column; gap: 6px; max-height: 220px; overflow-y: auto; }
.preview-item { display: flex; align-items: flex-start; gap: 10px; background: var(--surface); border: 1px solid var(--bdr); border-radius: 10px; padding: 10px; }
.preview-num { width: 22px; height: 22px; border-radius: 7px; background: var(--amber-light); color: var(--amber-dark); font-size: 10px; font-weight: 800; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.preview-content { flex: 1; min-width: 0; }
.preview-q { font-size: 13px; font-weight: 700; color: var(--text); margin: 0 0 4px; }

/* ══════════════════════════════════════════
   MODAL BUTTONS
══════════════════════════════════════════ */
.btn-qv-cancel {
  background: var(--surface); color: var(--text2);
  border: 1.5px solid var(--bdr); border-radius: 12px;
  padding: 10px 22px; font-size: 13px; font-weight: 700;
  cursor: pointer; font-family: inherit; transition: all 0.2s;
  display: flex; align-items: center;
}
.btn-qv-cancel:hover { background: var(--surface2); }

/* ══════════════════════════════════════════
   TOAST
══════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 28px; right: 28px;
  background: #0f172a; border-radius: 20px;
  padding: 16px 20px; display: flex; align-items: center; gap: 12px;
  z-index: 9999; box-shadow: 0 20px 40px rgba(0,0,0,0.3);
  min-width: 280px; max-width: 380px; overflow: hidden;
  border-left: 4px solid var(--amber);
}
.t-success { border-left-color: var(--amber); }
.t-error   { border-left-color: #ef4444; }
.t-info    { border-left-color: #6366f1; }
.t-ico { font-size: 20px; color: white; flex-shrink: 0; }
.t-body strong { font-size: 9px; font-weight: 900; color: #94a3b8; letter-spacing: 1.2px; display: block; margin-bottom: 2px; }
.t-body p { font-size: 13px; font-weight: 700; color: white; }

/* ══════════════════════════════════════════
   SCROLLBARS
══════════════════════════════════════════ */
.fancy-scroll::-webkit-scrollbar { width: 4px; }
.fancy-scroll::-webkit-scrollbar-track { background: var(--surface2); border-radius: 4px; }
.fancy-scroll::-webkit-scrollbar-thumb { background: var(--amber-bdr); border-radius: 4px; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: var(--bdr); border-radius: 4px; }

/* ══════════════════════════════════════════
   TRANSITIONS
══════════════════════════════════════════ */
.modal-quantum-enter-active { animation: zoomModalIn 0.3s var(--ease-spring); }
.modal-quantum-leave-active { animation: zoomModalIn 0.2s ease-in reverse; }
@keyframes zoomModalIn { from { opacity: 0; transform: scale(0.9) translateY(20px); } to { opacity: 1; transform: none; } }

.card-anim-enter-active { transition: all 0.45s var(--ease-spring); transition-delay: var(--card-delay, 0s); }
.card-anim-leave-active { transition: all 0.3s ease; position: absolute; }
.card-anim-enter-from   { opacity: 0; transform: translateY(24px) scale(0.96); }
.card-anim-leave-to     { opacity: 0; transform: scale(0.94); }
.card-anim-move         { transition: all 0.4s ease; }

.row-anim-enter-active { transition: all 0.35s var(--ease-spring); transition-delay: var(--row-delay, 0s); }
.row-anim-leave-active { transition: all 0.2s ease; position: absolute; width: 100%; }
.row-anim-enter-from   { opacity: 0; transform: translateX(-20px); }
.row-anim-leave-to     { opacity: 0; transform: translateX(16px); }
.row-anim-move         { transition: all 0.3s ease; }

.fade-up-enter-active { transition: all 0.35s var(--ease-out); }
.fade-up-leave-active { transition: all 0.25s ease; }
.fade-up-enter-from   { opacity: 0; transform: translateY(12px); }
.fade-up-leave-to     { opacity: 0; transform: translateY(-6px); }

.toast-slide-enter-active { animation: toastIn 0.4s var(--ease-spring); }
.toast-slide-leave-active { animation: toastIn 0.25s ease reverse; }
@keyframes toastIn { from { transform: translateX(60px); opacity: 0; } to { transform: none; opacity: 1; } }

/* ══════════════════════════════════════════
   RESPONSIVE
══════════════════════════════════════════ */
@media (max-width: 1024px) { .bq-workspace { padding: 20px !important; } .premium-title { font-size: 1.8rem; } }
@media (max-width: 768px) {
  .bq-header { flex-direction: column !important; gap: 16px !important; align-items: flex-start !important; }
  .questions-grid { grid-template-columns: 1fr; }
  .lang-cards-grid { grid-template-columns: 1fr; }
  .type-tiles-grid { grid-template-columns: repeat(3, 1fr); }
  .cats-grid { grid-template-columns: 1fr; }
}
</style>