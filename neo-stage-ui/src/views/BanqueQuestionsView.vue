<template>
  <div class="bq-root" @mousemove="handleParallax">

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
                <span class="root">{{ $t('rolesView.breadcrumb.admin') }}</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">{{ $t('sidebar.links.bank') }}</span>
              </div>
              <h2 class="premium-title">
                {{ $t('sidebar.links.bank').split(' ')[0] }}
                <span class="gradient-text">{{ $t('sidebar.links.bank').split(' ').slice(1).join(' ') }}</span>
              </h2>
              <p class="brand-subtitle-v2 d-flex align-items-center gap-2 mt-2">
                <span class="live-dot-wrap">
                  <span class="live-dot"></span>
                  <span class="live-ring"></span>
                </span>
                {{ $t('dashboard.kpis.talentsActifs') }} IA · {{ $t('sidebar.links.ai') }} FR/EN ·
                <strong>{{ questions.length }}</strong> actifs
              </p>
            </div>

            <div class="header-actions-group">
              <button class="btn-refresh-pro" @click="toggleGlobalTheme"
                :title="isGlobalDark ? $t('theme.light') : $t('theme.dark')">
                <i :class="isGlobalDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
              </button>

              <div class="search-inline-box" :class="{ focused: searchFocused }">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input v-model="searchQuery" @focus="searchFocused = true" @blur="searchFocused = false"
                  type="text" :placeholder="$t('search')" class="search-inline-input">
                <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search">
                  <i class="fa-solid fa-xmark"></i>
                </button>
              </div>

              <div class="action-buttons-wrap">
                <button class="btn-outline-pro" @click="showCatManager = true">
                  <i class="fa-solid fa-sitemap me-2"></i>{{ $t('campaigns.studio.step1.theme').split(' ')[0] }}s
                </button>
                <button class="btn-ai-glow" @click="showAIModal = true">
                  <span class="btn-shine-layer"></span>
                  <i class="fa-solid fa-wand-magic-sparkles me-2"></i>
                  <span>{{ $t('sidebar.links.ai') }}</span>
                  <span class="lang-badge-pill ms-2">FR/EN</span>
                </button>
                <button class="btn-enigma-primary shadow-premium" @click="openModal()">
                  <div class="btn-content">
                    <i class="fa-solid fa-plus me-2"></i>{{ $t('create') }}
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>
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
            <div class="d-flex align-items-center gap-2 flex-wrap">
              <div class="tabs-container">
                <div class="d-flex gap-2 p-1 rounded-4 shadow-sm border tabs-pill-wrap">
                  <button class="nav-tab-btn-modern" :class="{ active: activeFilter === -1 }" @click="activeFilter = -1">
                    <i class="fa-solid fa-border-all me-1"></i>{{ $t('all') }}
                    <span class="tab-count">{{ questions.length }}</span>
                  </button>
                  <button v-for="t in typeDefinitions" :key="t.val"
                    class="nav-tab-btn-modern"
                    :class="{ active: activeFilter === t.val }"
                    :style="activeFilter === t.val ? { '--tab-accent': t.color } : {}"
                    @click="activeFilter = t.val">
                    <i :class="t.icon + ' me-1'" :style="{ color: activeFilter === t.val ? t.color : '' }"></i>
                    {{ t.label }}
                    <span class="tab-count">{{ countByType(t.val) }}</span>
                  </button>
                </div>
              </div>
            </div>

            <div class="d-flex align-items-center gap-2 flex-wrap">
              <div class="sort-select-wrap">
                <i class="fa-solid fa-layer-group sort-ico"></i>
                <select v-model="selectedCat" class="sort-select-pro">
                  <option value="All">{{ $t('campaigns.studio.bank.allThemes') }}</option>
                  <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                </select>
                <i class="fa-solid fa-chevron-down sort-arrow"></i>
              </div>

              <div class="lang-cluster">
                <button :class="['lang-tab', { active: filterLang === 'all' }]" @click="filterLang = 'all'">
                  <i class="fa-solid fa-globe"></i> {{ $t('all') }}
                  <span class="ltab-count">{{ questions.length }}</span>
                </button>
                <button :class="['lang-tab', { active: filterLang === 'fr' }]" @click="filterLang = 'fr'">
                  🇫🇷 FR <span class="ltab-count">{{ countByLang('fr') }}</span>
                </button>
                <button :class="['lang-tab', { active: filterLang === 'en' }]" @click="filterLang = 'en'">
                  🇬🇧 EN <span class="ltab-count">{{ countByLang('en') }}</span>
                </button>
              </div>

              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'grid' }]"
                  @click="viewMode = 'grid'" :title="$t('view')">
                  <i class="fa-solid fa-table-cells-large"></i>
                </button>
                <button :class="['btn-view-toggle', { active: viewMode === 'list' }]"
                  @click="viewMode = 'list'" :title="$t('filter')">
                  <i class="fa-solid fa-list-ul"></i>
                </button>
              </div>

           
              <button
                :class="['btn-select-mode', { active: selectMode }]"
                @click="toggleSelectMode"
                :title="selectMode ? 'Quitter la sélection' : 'Sélectionner des questions'">
                <i :class="selectMode ? 'fa-solid fa-xmark' : 'fa-solid fa-check-double'"></i>
                <span class="d-none d-md-inline ms-1">{{ selectMode ? 'Annuler' : 'Sélectionner' }}</span>
              </button>
            </div>
          </div>

          <!-- ══════════════════════ BULK ACTION BAR ══════════════════════ -->
          <transition name="bulk-bar-anim">
            <div v-if="selectMode && selectedIds.size > 0" class="bulk-action-bar mb-4">
              <div class="bulk-info">
                <div class="bulk-count-badge">
                  <i class="fa-solid fa-check-circle me-1"></i>
                  {{ selectedIds.size }} sélectionnée{{ selectedIds.size > 1 ? 's' : '' }}
                </div>
                <span class="bulk-label">sur {{ filteredQuestions.length }} questions</span>
              </div>
              <div class="bulk-actions">
                <button class="bulk-btn bulk-select-all" @click="selectAll">
                  <i class="fa-solid fa-check-double me-1"></i>Tout sélectionner
                </button>
                <button class="bulk-btn bulk-deselect" @click="deselectAll">
                  <i class="fa-solid fa-square me-1"></i>Désélectionner
                </button>
                <div class="bulk-separator"></div>
                <button class="bulk-btn bulk-export" @click="exportSelectedCSV">
                  <i class="fa-solid fa-file-csv me-1"></i>Export CSV
                </button>
                <button class="bulk-btn bulk-export" @click="exportSelectedJSON">
                  <i class="fa-solid fa-file-code me-1"></i>Export JSON
                </button>
                <div class="bulk-separator"></div>
                <button class="bulk-btn bulk-delete" @click="bulkDelete">
                  <i class="fa-solid fa-trash-can me-1"></i>Supprimer ({{ selectedIds.size }})
                </button>
              </div>
            </div>
          </transition>

          <!-- Select All mini bar when select mode active but nothing selected -->
          <div v-if="selectMode && selectedIds.size === 0" class="select-hint-bar mb-3">
            <i class="fa-solid fa-hand-pointer me-2 text-amber"></i>
            Cliquez sur les cartes pour les sélectionner ·
            <button class="select-hint-btn ms-2" @click="selectAll">Tout sélectionner</button>
          </div>

          <!-- ══════════════════════ LOADING ══════════════════════ -->
          <div v-if="loading" class="empty-state-pro py-5 text-center">
            <div class="spinner-pro-premium"></div>
            <p class="state-label mt-3">
              <i class="fa-solid fa-satellite-dish fa-spin me-2"></i>{{ $t('loading') }}
            </p>
          </div>

          <!-- ══════════════════════ EMPTY ══════════════════════ -->
          <div v-else-if="filteredQuestions.length === 0" class="empty-state-pro py-5 text-center">
            <div class="empty-graphic mb-4">
              <div class="empty-ring r1"></div>
              <div class="empty-ring r2"></div>
              <div class="empty-ring r3"></div>
              <div class="empty-core"><i class="fa-solid fa-database"></i></div>
            </div>
            <h5 class="fw-800 mb-2">{{ $t('noData') }}</h5>
            <p class="text-muted small">{{ $t('campaigns.studio.bank.empty') }}</p>
            <button class="btn-enigma-primary mt-3" @click="resetFilters">
              <div class="btn-content">
                <i class="fa-solid fa-rotate-left me-2"></i>{{ $t('reset') }}
              </div>
              <div class="btn-glow"></div>
            </button>
          </div>

          <!-- ══════════════════════ GRID VIEW ══════════════════════ -->
          <transition-group v-else-if="viewMode === 'grid'" name="card-anim" tag="div" class="questions-grid">
            <div v-for="(q, i) in paginatedQuestions" :key="q.id"
              class="q-card campaign-card-modern"
              :class="{
                'card-selected': selectMode && selectedIds.has(q.id),
                'card-select-mode': selectMode
              }"
              :style="{ '--card-delay': i * 0.04 + 's', '--type-color': getTypeInfo(q.type).color }"
              @click="selectMode ? toggleSelect(q.id) : null">

           
              <div v-if="selectMode" class="select-overlay" @click.stop="toggleSelect(q.id)">
                <div :class="['select-checkbox', { checked: selectedIds.has(q.id) }]">
                  <i v-if="selectedIds.has(q.id)" class="fa-solid fa-check"></i>
                </div>
              </div>

              <div class="card-type-stripe" :style="{ background: getTypeInfo(q.type).color }"></div>

              <div class="card-lang-banner" :class="`lang-${resolveQuestionLang(q)}`">
                <span>{{ resolveQuestionLang(q) === 'en' ? '🇬🇧' : '🇫🇷' }}</span>
                <span class="lang-banner-name">{{ resolveQuestionLang(q) === 'en' ? 'English' : 'Français' }}</span>
              </div>

              <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                <div class="card-cat-pill" :class="{ 'cat-pill-unclassified': getDisplayTheme(q) === 'Non classé' }">
                  <i :class="getDisplayTheme(q) === 'Non classé' ? 'fa-solid fa-folder me-1' : 'fa-solid fa-folder-open me-1'"></i>
                  <span>{{ getDisplayTheme(q) }}</span>
                </div>
                <div v-if="!selectMode" class="d-flex gap-2 align-items-center">
                  <button class="btn-icon-sm" @click.stop="openModal(q)" :title="$t('edit')">
                    <i class="fa-solid fa-pen-to-square"></i>
                  </button>
                  <button class="btn-icon-sm danger" @click.stop="handleDelete(q.id)" :title="$t('delete')">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>

              <div class="type-badge-row" :style="{ '--badge-c': getTypeInfo(q.type).color }">
                <div class="type-badge-icon-box"
                  :style="{ background: getTypeInfo(q.type).color + '18', border: '1px solid ' + getTypeInfo(q.type).color + '35' }">
                  <i :class="getTypeInfo(q.type).icon" :style="{ color: getTypeInfo(q.type).color }"></i>
                </div>
                <span class="type-badge-label" :style="{ color: getTypeInfo(q.type).color }">
                  {{ getTypeInfo(q.type).label }}
                </span>
                <span class="type-badge-live" :style="{ background: getTypeInfo(q.type).color }"></span>
              </div>

              <p class="card-enonce">{{ q.enonce }}</p>

              <div v-if="q.choix && q.choix.length > 0" class="card-options-preview">
                <div class="opts-preview-header">
                  <i class="fa-solid fa-list-check text-amber me-1"></i>
                  <span>{{ q.choix.length }} {{ $t('campaigns.studio.bank.options').replace(':', '') }}</span>
                </div>
                <div class="opts-preview-list">
                  <div v-for="(opt, oi) in q.choix.slice(0, 3)" :key="oi" class="opt-preview-item">
                    <span class="opt-letter">{{ String.fromCharCode(65 + oi) }}</span>
                    <span class="opt-text">{{ opt }}</span>
                  </div>
                  <div v-if="q.choix.length > 3" class="opt-more">+{{ q.choix.length - 3 }} autres</div>
                </div>
              </div>

              <div class="card-footer-modern d-flex justify-content-between align-items-center pt-3 border-top">
                <div class="level-indicator">
                  <span class="level-label-sm"><i class="fa-solid fa-signal me-1"></i>NIV.</span>
                  <div class="level-dots">
                    <span v-for="d in 5" :key="d" class="ldot" :class="{ 'ldot-on': d <= q.points }"
                      :style="d <= q.points ? { background: getLevelColor(q.points), boxShadow: '0 0 4px ' + getLevelColor(q.points) } : {}">
                    </span>
                  </div>
                  <span class="level-val" :style="{ color: getLevelColor(q.points) }">{{ q.points }}/5</span>
                </div>
                <span class="slot-badge"
                  :style="{ background: getLevelColor(q.points) + '18', color: getLevelColor(q.points), border: '1px solid ' + getLevelColor(q.points) + '35' }">
                  <i class="fa-solid fa-gauge me-1"></i>Niv. {{ q.points }}
                </span>
              </div>
            </div>
          </transition-group>

          <!-- ══════════════════════ LIST VIEW ══════════════════════ -->
          <div v-else class="list-view-pro">
            <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
        
              <span v-if="selectMode" style="width:40px" class="list-col-label">
                <input type="checkbox" class="asset-checkbox"
                  :checked="selectedIds.size === filteredQuestions.length && filteredQuestions.length > 0"
                  @change="selectedIds.size === filteredQuestions.length ? deselectAll() : selectAll()"
                  style="accent-color:#f59e0b; cursor:pointer">
              </span>
              <span style="width:110px" class="list-col-label">TYPE</span>
              <span style="width:80px" class="list-col-label">LANG</span>
              <span class="flex-grow-1 list-col-label">QUESTION</span>
              <span style="width:150px" class="list-col-label">{{ $t('campaigns.studio.step1.theme') }}</span>
              <span style="width:80px" class="list-col-label text-center">NIV.</span>
              <span style="width:80px" class="list-col-label text-center">{{ $t('actions') }}</span>
            </div>
            <transition-group name="row-anim" tag="div">
              <div v-for="(q, i) in paginatedQuestions" :key="q.id"
                class="list-row-item d-flex align-items-center px-4 py-3 mb-2"
                :class="{ 'row-selected': selectMode && selectedIds.has(q.id) }"
                :style="{ '--row-delay': i * 0.02 + 's' }"
                @click="selectMode ? toggleSelect(q.id) : null">

             
                <div v-if="selectMode" style="width:40px" @click.stop="toggleSelect(q.id)">
                  <div :class="['select-checkbox-sm', { checked: selectedIds.has(q.id) }]">
                    <i v-if="selectedIds.has(q.id)" class="fa-solid fa-check"></i>
                  </div>
                </div>

                <div style="width:110px">
                  <span class="row-type-badge"
                    :style="{ color: getTypeInfo(q.type).color, background: getTypeInfo(q.type).color + '12', borderColor: getTypeInfo(q.type).color + '30' }">
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
                  <span class="meta-chip" :class="{ 'chip-unclassified': getDisplayTheme(q) === 'Non classé' }">
                    <i :class="getDisplayTheme(q) === 'Non classé' ? 'fa-solid fa-folder me-1' : 'fa-solid fa-folder-open me-1'"></i>
                    {{ getDisplayTheme(q) }}
                  </span>
                </div>
                <div style="width:80px" class="text-center">
                  <span class="slot-badge"
                    :style="{ background: getLevelColor(q.points) + '18', color: getLevelColor(q.points), border: '1px solid ' + getLevelColor(q.points) + '35' }">
                    {{ q.points }}/5
                  </span>
                </div>
                <div style="width:80px" class="d-flex gap-2 justify-content-center">
                  <template v-if="!selectMode">
                    <button class="btn-icon-sm" @click.stop="openModal(q)">
                      <i class="fa-solid fa-pen-to-square"></i>
                    </button>
                    <button class="btn-icon-sm danger" @click.stop="handleDelete(q.id)">
                      <i class="fa-solid fa-trash-can"></i>
                    </button>
                  </template>
                  <template v-else>
                    <div :class="['select-checkbox-sm', { checked: selectedIds.has(q.id) }]" @click.stop="toggleSelect(q.id)">
                      <i v-if="selectedIds.has(q.id)" class="fa-solid fa-check"></i>
                    </div>
                  </template>
                </div>
              </div>
            </transition-group>
          </div>

          <!-- ══════════════════════ PAGINATION ══════════════════════ -->
          <div v-if="totalPages > 1" class="bq-pagination-wrap mt-5 mb-4 animate__animated animate__fadeIn d-flex justify-content-center align-items-center position-relative w-100 p-3 shadow-sm" style="background: var(--surface); border: 1.5px solid var(--bdr); border-radius: 14px;">
            
            <div class="pagination-controls d-flex align-items-center gap-1 justify-content-center">
              <button 
                class="btn-pagination" 
                :disabled="currentPage === 1" 
                @click="setPage(1)"
                title="Première page"
              >
                <i class="fa-solid fa-angles-left"></i>
              </button>
              
              <button 
                class="btn-pagination" 
                :disabled="currentPage === 1" 
                @click="setPage(currentPage - 1)"
                title="Page précédente"
              >
                <i class="fa-solid fa-angle-left"></i>
              </button>
              
              <button 
                v-for="page in pageNumbers" 
                :key="page" 
                :class="['btn-pagination-num', { active: currentPage === page }]" 
                @click="setPage(page)"
              >
                {{ page }}
              </button>
              
              <button 
                class="btn-pagination" 
                :disabled="currentPage === totalPages" 
                @click="setPage(currentPage + 1)"
                title="Page suivante"
              >
                <i class="fa-solid fa-angle-right"></i>
              </button>
              
              <button 
                class="btn-pagination" 
                :disabled="currentPage === totalPages" 
                @click="setPage(totalPages)"
                title="Dernière page"
              >
                <i class="fa-solid fa-angles-right"></i>
              </button>
            </div>
            
            <div class="pagination-size-selector d-flex align-items-center gap-2 position-absolute" style="right: 1.5rem;">
              <span class="text-muted small fw-600">Par page :</span>
              <div class="sort-select-wrap pagination-size-select">
                <select v-model.number="itemsPerPage" class="sort-select-pro py-1 px-2 text-dark" style="min-width: 60px; height: 32px; border-radius: 8px;">
                  <option :value="12">12</option>
                  <option :value="24">24</option>
                  <option :value="48">48</option>
                  <option :value="96">96</option>
                </select>
                <i class="fa-solid fa-chevron-down sort-arrow text-dark" style="right: 8px; font-size: 10px;"></i>
              </div>
            </div>
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
          <div class="modal-corner tl"></div><div class="modal-corner tr"></div>
          <div class="modal-corner bl"></div><div class="modal-corner br"></div>

          <div class="qv-header">
            <div class="d-flex align-items-center gap-3 flex-grow-1">
              <div class="icon-box-v2 amber" style="position:relative">
                <div class="icon-pulse-ring"></div>
                <i class="fa-solid fa-wand-magic-sparkles"></i>
              </div>
              <div>
                <h5 class="fw-900 m-0">{{ $t('sidebar.links.ai') }} <em class="text-amber">Bilingue</em></h5>
                <p class="small text-muted m-0">
                  <i class="fa-solid fa-microchip me-1"></i>Moteur Gemini · Questions FR &amp; EN
                </p>
              </div>
            </div>
            <button class="btn-modal-close" @click="showAIModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="modal-body-scroll fancy-scroll p-4">
            <!-- LANGUE -->
            <div class="enigma-input-wrap mb-4">
              <label>{{ $t('lang.switch').toUpperCase() }}</label>
              <div class="lang-cards-grid">
                <label v-for="l in langOptions" :key="l.val"
                  :class="['lang-card', { 'lang-card-active': aiForm.langue === l.val }]"
                  @click="aiForm.langue = l.val">
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
              <label>{{ $t('campaigns.studio.quickAdd.type').toUpperCase() }}</label>
              <div class="type-tiles-grid">
                <div v-for="t in typeDefinitions" :key="t.val"
                  :class="['type-tile', { 'type-tile-active': aiForm.type === t.val }]"
                  :style="aiForm.type === t.val ? { '--tile-c': t.color } : {}"
                  @click="aiForm.type = t.val">
                  <div class="tile-icon-wrap" :style="{ color: t.color }">
                    <i :class="t.icon"></i>
                  </div>
                  <span class="tile-label">{{ t.label }}</span>
                  <div class="tile-check"><i class="fa-solid fa-check"></i></div>
                </div>
              </div>
            </div>

            <!-- ✅ CATÉGORIE + SOUS-THÈME — required with visual indicator -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label class="d-flex justify-content-between">
                    <span>{{ $t('campaigns.studio.step1.theme') }}</span>
                    <span class="required-badge">Requis ✦</span>
                  </label>
                  <div class="theme-select-wrapper" :class="{ 'field-required-glow': !aiForm.theme && aiAttempted }">
                    <i class="fa-solid fa-folder theme-select-icon"></i>
                    <select v-model="aiForm.theme" class="enigma-field theme-select"
                      @change="aiForm.sousTheme = ''">
                      <option value="">— Choisir un thème —</option>
                      <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">{{ cat.nom }}</option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label class="d-flex justify-content-between">
                    <span>{{ $t('campaigns.studio.step1.subTheme') }}</span>
                    <span class="required-badge">Requis ✦</span>
                  </label>
                  <div class="theme-select-wrapper" :class="{ 'disabled-wrapper': !aiForm.theme, 'field-required-glow': !aiForm.sousTheme && aiAttempted }">
                    <i class="fa-solid fa-tags theme-select-icon"></i>
                    <select v-model="aiForm.sousTheme" class="enigma-field theme-select"
                      :disabled="!aiForm.theme">
                      <option value="">— {{ $t('optional') }} —</option>
                      <option v-for="sub in aiDynamicSubCategories" :key="sub.id" :value="sub.nom">
                        {{ sub.nom }}
                      </option>
                    </select>
                  </div>
                  <!-- ✅ inline warning if no subcategories -->
                  <small v-if="aiForm.theme && aiDynamicSubCategories.length === 0" class="text-warning mt-1 d-block">
                    <i class="fa-solid fa-triangle-exclamation me-1"></i>
                    Aucun sous-thème — ajoutez-en dans Gestion Thèmes
                  </small>
                </div>
              </div>
            </div>

            <!-- ✅ THEME PREVIEW BADGE -->
            <transition name="fade-up">
              <div v-if="aiForm.theme && aiForm.sousTheme" class="theme-preview-badge mb-4">
                <i class="fa-solid fa-folder-open me-2 text-amber"></i>
                <span class="theme-preview-path">{{ aiForm.theme }}</span>
                <i class="fa-solid fa-chevron-right mx-2" style="font-size:9px;opacity:0.5"></i>
                <span class="theme-preview-sub">{{ aiForm.sousTheme }}</span>
                <span class="theme-preview-tick ms-2">
                  <i class="fa-solid fa-check"></i>
                </span>
              </div>
            </transition>

            <!-- NOMBRE + DIFFICULTÉ -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>NOMBRE DE QUESTIONS</label>
                  <div class="number-stepper">
                    <button class="step-btn" @click="aiForm.n = Math.max(1, aiForm.n - 1)">
                      <i class="fa-solid fa-minus"></i>
                    </button>
                    <input v-model.number="aiForm.n" type="number" min="1" max="100" class="step-input"
                      @input="aiForm.n = aiForm.n > 100 ? 100 : aiForm.n < 1 ? 1 : aiForm.n">
                    <button class="step-btn" @click="aiForm.n = Math.min(100, aiForm.n + 1)">
                      <i class="fa-solid fa-plus"></i>
                    </button>
                  </div>
                  <small :class="aiForm.n < 5 ? 'text-danger' : 'text-muted'">
                    {{ aiForm.n < 5 ? 'Minimum 5 questions requis' : 'Max 100 questions' }}
                  </small>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>{{ $t('campaigns.studio.quickAdd.difficulty').toUpperCase() }}</label>
                  <div class="d-flex gap-2">
                    <button v-for="d in difficultyLevels" :key="d.val"
                      class="diff-btn flex-grow-1"
                      :class="{ 'diff-btn-active': aiForm.difficulty === d.val }"
                      :style="aiForm.difficulty === d.val ? { background: d.color, borderColor: d.color, color: '#fff' } : {}"
                      @click="aiForm.difficulty = d.val">
                      <i :class="d.icon + ' me-1'"></i>{{ d.label }}
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- PROGRESS -->
            <transition name="fade-up">
              <div v-if="isAILoading" class="ai-progress-box mb-4">
                <div class="ai-prog-track">
                  <div class="ai-prog-fill" :style="{ width: aiProgress + '%' }"></div>
                </div>
                <div class="ai-prog-text mt-2">
                  <i class="fa-solid fa-circle-notch fa-spin me-2"></i>{{ aiStatusText }}
                </div>
              </div>
            </transition>

            <!-- APERÇU -->
            <transition name="fade-up">
              <div v-if="aiPreview.length > 0 && !isAILoading" class="ai-preview-box">
                <div class="preview-header">
                  <i class="fa-solid fa-eye me-2 text-amber"></i>
                  <span>{{ aiPreview.length }} QUESTIONS</span>
                  <span class="ms-2 row-lang-chip lc-fr">
                    🇫🇷 {{ aiPreview.filter(q => resolvePreviewLang(q) === 'fr').length }}
                  </span>
                  <span class="ms-1 row-lang-chip lc-en">
                    🇬🇧 {{ aiPreview.filter(q => resolvePreviewLang(q) === 'en').length }}
                  </span>
                
                  <span v-if="aiPreview[0]?.theme" class="ms-auto meta-chip">
                    <i class="fa-solid fa-folder-open me-1"></i>{{ aiPreview[0].theme }}
                    <i class="fa-solid fa-chevron-right mx-1" style="font-size:8px"></i>
                    {{ aiPreview[0].sousTheme }}
                  </span>
                  <button class="btn-clear-search ms-2" @click="aiPreview = []">
                    <i class="fa-solid fa-xmark"></i>
                  </button>
                </div>
                <div class="preview-list fancy-scroll">
                  <div v-for="(pq, pi) in aiPreview" :key="pi" class="preview-item">
                    <span class="preview-num">{{ pi + 1 }}</span>
                    <div class="preview-content">
                      <p class="preview-q">{{ pq.question }}</p>
                      <div v-if="pq.options && pq.options.length" class="d-flex flex-wrap gap-1 mt-1">
                        <span v-for="(opt, oi) in pq.options.slice(0, 4)" :key="oi" class="meta-chip">
                          {{ String.fromCharCode(65 + oi) }}. {{ opt }}
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
            <button class="btn-qv-cancel" @click="showAIModal = false">
              <i class="fa-solid fa-xmark me-2"></i>{{ $t('cancel') }}
            </button>
            <button class="btn-outline-pro"
              :disabled="isAILoading || aiForm.n < 5"
              @click="previewAI">
              <i class="fa-solid fa-eye me-2"></i>{{ $t('view') }}
            </button>
            <button class="btn-enigma-primary"
              :disabled="isAILoading || aiForm.n < 5"
              @click="handleAIGeneration">
              <div class="btn-content">
                <i v-if="isAILoading" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                <i v-else class="fa-solid fa-wand-magic-sparkles me-2"></i>
                {{ isAILoading ? $t('loading') : $t('sidebar.links.ai') }}
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
                <h5 class="fw-900 m-0">
                  {{ $t('create') }} <em class="text-amber">{{ $t('campaigns.studio.step1.theme') }}</em>
                </h5>
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
                <input v-model="newCatName" @keyup.enter="addCategory"
                  placeholder="Nom de la nouvelle catégorie..." class="search-inline-input">
              </div>
              <button class="btn-enigma-primary" @click="addCategory">
                <div class="btn-content"><i class="fa-solid fa-plus me-2"></i>{{ $t('create') }}</div>
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
                      <span class="slot-badge" style="font-size:9px">
                        {{ cat.sousCategories?.length || 0 }}
                      </span>
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
                    <input v-model="subCatInputs[cat.id]" @keyup.enter="handleSubAdd(cat.id)"
                      placeholder="Ajouter un sous-thème..." class="sub-input">
                    <button class="sub-add-btn" @click="handleSubAdd(cat.id)">
                      <i class="fa-solid fa-plus"></i>
                    </button>
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
                  {{ isEdit ? $t('edit') : $t('create') }} <em class="text-amber">Question</em>
                </h5>
                <p class="small text-muted m-0">
                  {{ isEdit ? 'Mise à jour du référentiel' : 'Nouvelle entrée dans la banque' }}
                </p>
              </div>
            </div>
            <button class="btn-modal-close" @click="showModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>

          <div class="modal-body-scroll fancy-scroll p-4">
            <!-- TYPE -->
            <div class="enigma-input-wrap mb-4">
              <label>{{ $t('campaigns.studio.quickAdd.type').toUpperCase() }}</label>
              <div class="type-tiles-grid">
                <div v-for="t in typeDefinitions" :key="t.val"
                  :class="['type-tile', { 'type-tile-active': form.type === t.val }]"
                  :style="form.type === t.val ? { '--tile-c': t.color } : {}"
                  @click="handleTypeChange(t.val)">
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
              <label>{{ $t('lang.switch').toUpperCase() }}</label>
              <div class="d-flex gap-2">
                <button :class="['lang-toggle-btn', 'flex-grow-1', { active: form.langue === 'fr' }]"
                  @click="form.langue = 'fr'">
                  🇫🇷 Français
                </button>
                <button :class="['lang-toggle-btn', 'flex-grow-1', { active: form.langue === 'en' }]"
                  @click="form.langue = 'en'">
                  🇬🇧 English
                </button>
              </div>
            </div>

            <!-- ÉNONCÉ -->
            <div class="enigma-input-wrap mb-4">
              <label>{{ $t('campaigns.studio.quickAdd.enonce') }}</label>
              <div style="position:relative">
                <textarea v-model="form.enonce" class="enigma-field" rows="3"
                  :placeholder="form.langue === 'en' ? 'Enter the question...' : 'Saisir la problématique...'">
                </textarea>
                <span class="char-counter">{{ form.enonce.length }}</span>
              </div>
            </div>

            <!-- CATÉGORIE + SOUS-THÈME -->
            <div class="row g-3 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>{{ $t('campaigns.studio.step1.theme') }}</label>
                  <div class="theme-select-wrapper">
                    <i class="fa-solid fa-folder theme-select-icon"></i>
                    <select v-model="form.theme" class="enigma-field theme-select"
                      @change="form.sousTheme = ''">
                      <option value="">{{ $t('optional') }}...</option>
                      <option v-for="cat in categoriesList" :key="cat.id" :value="cat.nom">
                        {{ cat.nom }}
                      </option>
                    </select>
                  </div>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>{{ $t('campaigns.studio.step1.subTheme') }}</label>
                  <div class="theme-select-wrapper" :class="{ 'disabled-wrapper': !form.theme }">
                    <i class="fa-solid fa-tags theme-select-icon"></i>
                    <select v-model="form.sousTheme" class="enigma-field theme-select"
                      :disabled="!form.theme">
                      <option value="">{{ $t('optional') }}</option>
                      <option v-for="sub in dynamicSubCategories" :key="sub.id" :value="sub.nom">
                        {{ sub.nom }}
                      </option>
                    </select>
                  </div>
                </div>
              </div>
            </div>

            <!-- NIVEAU -->
            <div class="enigma-input-wrap mb-4">
              <label class="d-flex justify-content-between">
                <span>{{ $t('campaigns.studio.quickAdd.difficulty').toUpperCase() }}</span>
                <span :style="{ color: getLevelColor(form.points) }">
                  <i class="fa-solid fa-signal me-1"></i>{{ form.points }} / 5
                </span>
              </label>
              <div class="admissibility-dashboard">
                <input type="range" min="1" max="5" step="1" v-model.number="form.points"
                  class="enigma-range"
                  :style="{ '--rng-c': getLevelColor(form.points), '--rng-pct': ((form.points - 1) / 4 * 100) + '%' }">
                <div class="d-flex justify-content-between mt-2">
                  <span class="score-tier tier-low">{{ $t('campaigns.studio.step1.tiers.low') }}</span>
                  <span class="score-tier tier-mid">{{ $t('campaigns.studio.step1.tiers.mid') }}</span>
                  <span class="score-tier tier-high">{{ $t('campaigns.studio.step1.tiers.high') }}</span>
                </div>
              </div>
            </div>

            <!-- OPTIONS QCU / QCM / VRAI-FAUX -->
            <div class="enigma-input-wrap mb-4" v-if="[0, 1, 2].includes(form.type)">
              <div class="d-flex justify-content-between align-items-center mb-3 pb-2"
                style="border-bottom:1px solid var(--bdr)">
                <label class="m-0">
                  <i :class="getTypeInfo(form.type).icon + ' me-2'"
                    :style="{ color: getTypeInfo(form.type).color }"></i>
                  {{ form.langue === 'en' ? 'ANSWER OPTIONS' : 'OPTIONS DE RÉPONSE' }}
                </label>
                <button v-if="form.type !== 2" @click="addResponse" class="btn-bank-action-v2">
                  <i class="fa-solid fa-plus me-1"></i>{{ $t('create') }}
                </button>
              </div>
              <div class="d-flex flex-column gap-2">
                <div v-for="(rep, idx) in form.reponses" :key="idx" class="asset-card-v8">
                  <div class="drag-node-handle opt-check-area">
                    <input v-if="form.type === 1" type="checkbox" v-model="rep.estCorrecte"
                      class="asset-checkbox" style="accent-color:#f59e0b">
                    <input v-else type="radio" :name="`r-${form.id || 'new'}`" :value="idx"
                      v-model="correctRadioIndex" class="asset-checkbox" style="accent-color:#f59e0b">
                  </div>
                  <input v-model="rep.texte" class="opt-input"
                    :placeholder="`${form.langue === 'en' ? 'Option' : 'Option'} ${idx + 1}...`">
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

            <!-- CODE / TEXTE LIBRE / PROJET -->
            <div class="enigma-input-wrap mb-4" v-if="[4, 5, 6].includes(form.type)">
              <label>
                <i :class="(form.type === 5 ? 'fa-solid fa-terminal' : 'fa-solid fa-pen-to-square') + ' me-2'"
                  :style="{ color: getTypeInfo(form.type).color }"></i>
                {{ form.type === 5
                  ? 'CODE DE RÉFÉRENCE'
                  : form.langue === 'en' ? 'EXPECTED ANSWER' : 'RÉPONSE ATTENDUE' }}
              </label>
              <div class="code-box">
                <div class="code-titlebar">
                  <div class="code-dots">
                    <span class="cd-red"></span>
                    <span class="cd-amber"></span>
                    <span class="cd-green"></span>
                  </div>
                  <span class="code-fname">
                    <i :class="getTypeInfo(form.type).icon + ' me-1'"
                      :style="{ color: getTypeInfo(form.type).color }"></i>
                    {{ form.type === 5 ? 'solution.js' : 'answer.txt' }}
                  </span>
                </div>
                <textarea v-model="form.bonneReponse" class="code-area"
                  :rows="form.type === 5 ? 10 : 5"
                  :placeholder="getPlaceholder(form.type, form.langue)">
                </textarea>
              </div>
            </div>
          </div>

          <div class="modal-footer-actions">
            <button class="btn-qv-cancel" @click="showModal = false">
              <i class="fa-solid fa-xmark me-2"></i>{{ $t('close') }}
            </button>
            <button class="btn-enigma-primary" :disabled="isSaving || !form.enonce.trim()" @click="save">
              <div class="btn-content">
                <i v-if="isSaving" class="fa-solid fa-circle-notch fa-spin me-2"></i>
                <i v-else class="fa-solid fa-floppy-disk me-2"></i>
                {{ isSaving ? $t('loading') : $t('save') }}
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
          <strong>{{
            toast.type === 'success' ? 'SUCCÈS' :
            toast.type === 'error'   ? 'ERREUR' : 'INFO'
          }}</strong>
          <p class="m-0 small">{{ toast.message }}</p>
        </div>
        <button class="btn-clear-search ms-2" @click="toast.active = false">
          <i class="fa-solid fa-xmark"></i>
        </button>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, watch } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import axios from 'axios';
import Swal from 'sweetalert2';
import { useRouter } from 'vue-router';

const router = useRouter();
const { t }  = useI18n();

const AI_BASE = 'http://127.0.0.1:8000';

// ══════════════════════════════════════════════════════════════
// CONSTANTES
// ══════════════════════════════════════════════════════════════
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
  { val: 'en',   flag: '🇬🇧', name: 'English',  desc: 'Questions in English'  },
  { val: 'both', flag: '🌐',  name: 'Bilingue',  desc: 'FR + EN simultanément' },
];

// ══════════════════════════════════════════════════════════════
// STATE
// ══════════════════════════════════════════════════════════════
const questions      = ref([]);
const categoriesList = ref([]);
const loading        = ref(true);
const isSaving       = ref(false);
const isAILoading    = ref(false);
const showModal      = ref(false);
const showCatManager = ref(false);
const showAIModal    = ref(false);
const isEdit         = ref(false);
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
const aiAttempted    = ref(false); // ✅ tracks if user tried to generate without theme

//  MULTI-SELECT STATE
const selectMode  = ref(false);
const selectedIds = ref(new Set());

const toast  = reactive({ active: false, message: '', type: 'success', icon: '' });
const aiForm = reactive({ theme: '', sousTheme: '', n: 5, langue: 'fr', type: 0, difficulty: 2 });
const form   = reactive({
  id: '', enonce: '', type: 0, points: 1,
  theme: '', sousTheme: '', reponses: [], bonneReponse: '', langue: 'fr'
});

// ══════════════════════════════════════════════════════════════
//  MULTI-SELECT ACTIONS
// ══════════════════════════════════════════════════════════════
const toggleSelectMode = () => {
  selectMode.value = !selectMode.value;
  if (!selectMode.value) selectedIds.value = new Set();
};

const toggleSelect = (id) => {
  const set = new Set(selectedIds.value);
  if (set.has(id)) set.delete(id);
  else set.add(id);
  selectedIds.value = set;
};

const selectAll = () => {
  selectedIds.value = new Set(filteredQuestions.value.map(q => q.id));
};

const deselectAll = () => {
  selectedIds.value = new Set();
};

const bulkDelete = async () => {
  if (selectedIds.value.size === 0) return;
  const result = await Swal.fire({
    title: `Supprimer ${selectedIds.value.size} question${selectedIds.value.size > 1 ? 's' : ''} ?`,
    text: 'Cette action est irréversible.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Supprimer',
    cancelButtonText: t('cancel'),
    confirmButtonColor: '#ef4444',
    background: 'var(--surface, #fff)',
  });
  if (!result.isConfirmed) return;

  let successCount = 0;
  let errorCount = 0;
  for (const id of selectedIds.value) {
    try {
      await api.delete(`/Questions/${id}`);
      successCount++;
    } catch {
      errorCount++;
    }
  }
  await fetchData();
  selectedIds.value = new Set();
  selectMode.value = false;
  showToast(`${successCount} question${successCount > 1 ? 's' : ''} supprimée${successCount > 1 ? 's' : ''}${errorCount ? ` (${errorCount} erreur${errorCount > 1 ? 's' : ''})` : ''}`, successCount > 0 ? 'success' : 'error');
};

// ✅ Export CSV
const exportSelectedCSV = () => {
  const selected = questions.value.filter(q => selectedIds.value.has(q.id));
  if (!selected.length) return;

  const headers = ['ID', 'Enoncé', 'Type', 'Langue', 'Thème', 'Sous-thème', 'Points', 'Choix', 'Bonne réponse'];
  const rows = selected.map(q => [
    q.id,
    `"${(q.enonce || '').replace(/"/g, '""')}"`,
    getTypeInfo(q.type).label,
    resolveQuestionLang(q).toUpperCase(),
    q.theme || 'Non classé',
    q.sousTheme || '',
    q.points || 1,
    `"${(q.choix || []).join(' | ')}"`,
    `"${(q.bonneReponse || '').replace(/"/g, '""')}"`
  ]);

  const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
  const blob = new Blob(['\uFEFF' + csv], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `questions_export_${new Date().toISOString().slice(0,10)}.csv`;
  a.click();
  URL.revokeObjectURL(url);
  showToast(`${selected.length} question${selected.length > 1 ? 's' : ''} exportée${selected.length > 1 ? 's' : ''} en CSV`, 'success');
};

// Export JSON
const exportSelectedJSON = () => {
  const selected = questions.value.filter(q => selectedIds.value.has(q.id));
  if (!selected.length) return;

  const blob = new Blob([JSON.stringify(selected, null, 2)], { type: 'application/json' });
  const url = URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = `questions_export_${new Date().toISOString().slice(0,10)}.json`;
  a.click();
  URL.revokeObjectURL(url);
  showToast(`${selected.length} question${selected.length > 1 ? 's' : ''} exportée${selected.length > 1 ? 's' : ''} en JSON`, 'success');
};

// ══════════════════════════════════════════════════════════════
// RÉSOLUTION LANGUE
// ══════════════════════════════════════════════════════════════
const resolveQuestionLang = (q) => {
  if (q.langue === 'en' || q.langue === 'fr') return q.langue;
  if (q.lang   === 'en' || q.lang   === 'fr') return q.lang;
  const tx = (q.enonce || '').toLowerCase();
  const enScore = ['what','which','how','when','where','why',' is ',' are ',' the '].filter(k => tx.includes(k)).length;
  const frScore = ['quel','quelle','comment','pourquoi',' les ',' des ',' est ',' sont '].filter(k => tx.includes(k)).length;
  return enScore > frScore ? 'en' : 'fr';
};

const resolvePreviewLang = (pq) => {
  if (pq.langue === 'en' || pq.langue === 'fr') return pq.langue;
  if (pq.lang   === 'en' || pq.lang   === 'fr') return pq.lang;
  return 'fr';
};

// ══════════════════════════════════════════════════════════════
// AFFICHAGE THÈME
// ══════════════════════════════════════════════════════════════
const getDisplayTheme = (q) => {
  const theme     = (q.theme     || '').trim();
  const sousTheme = (q.sousTheme || '').trim();
  if (theme && sousTheme) return `${theme} › ${sousTheme}`;
  if (theme)              return theme;
  if (sousTheme)          return sousTheme;
  return 'Non classé';
};

// ══════════════════════════════════════════════════════════════
// COMPUTED
// ══════════════════════════════════════════════════════════════
const dynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === form.theme);
  return cat?.sousCategories ?? [];
});

const aiDynamicSubCategories = computed(() => {
  const cat = categoriesList.value.find(c => c.nom === aiForm.theme);
  return cat?.sousCategories ?? [];
});

const correctRadioIndex = computed({
  get: () => {
    const idx = form.reponses.findIndex(r => r.estCorrecte);
    return idx >= 0 ? idx : 0;
  },
  set: (idx) => form.reponses.forEach((r, i) => { r.estCorrecte = (i === idx); })
});

const filteredQuestions = computed(() =>
  questions.value.filter(q => {
    const ms = !searchQuery.value ||
      q.enonce?.toLowerCase().includes(searchQuery.value.toLowerCase());
    const mt = activeFilter.value === -1 || q.type === activeFilter.value;
    const mc = selectedCat.value === 'All' ||
      (q.theme || '').toLowerCase() === selectedCat.value.toLowerCase();
    const ml = filterLang.value === 'all' || resolveQuestionLang(q) === filterLang.value;
    return ms && mt && mc && ml;
  })
);

const countByLang = (lang) => questions.value.filter(q => resolveQuestionLang(q) === lang).length;
const countByType = (val)  => questions.value.filter(q => q.type === val).length;

const kpiStats = computed(() => [
  {
    label: t('dashboard.kpis.totalTests'),
    value: questions.value.length,
    icon: 'fa-solid fa-database', color: '#f59e0b', bg: '#fffbeb', trend: 8
  },
  {
    label: t('dashboard.kpis.campaigns'),
    value: categoriesList.value.length,
    icon: 'fa-solid fa-sitemap', color: '#6366f1', bg: '#eef2ff'
  },
  {
    label: 'Difficiles (≥4)',
    value: questions.value.filter(x => x.points >= 4).length,
    icon: 'fa-solid fa-bolt-lightning', color: '#ef4444', bg: '#fff1f2'
  },
  {
    label: 'Français',
    value: countByLang('fr'),
    icon: 'fa-solid fa-flag', color: '#10b981', bg: '#ecfdf5'
  },
  {
    label: 'English',
    value: countByLang('en'),
    icon: 'fa-solid fa-earth-americas', color: '#8b5cf6', bg: '#f5f3ff'
  },
]);

// ══════════════════════════════════════════════════════════════
// API — FETCH
// ══════════════════════════════════════════════════════════════
const fetchData = async () => {
  loading.value = true;
  try {
    const [resQ, resC] = await Promise.all([
      api.get('/Questions'),
      api.get('/Categories')
    ]);
    questions.value      = resQ.data;
    categoriesList.value = resC.data;
  } catch (err) {
    console.error('[fetchData]', err);
    showToast(t('error'), 'error');
  } finally {
    loading.value = false;
  }
};

// ══════════════════════════════════════════════════════════════
// IA — PROGRESS SIMULÉ
// ══════════════════════════════════════════════════════════════
const simulateProgress = () => {
  aiProgress.value = 0;
  const steps = [
    [15,  'Connexion Gemini...'],
    [35,  'Analyse thématique...'],
    [55,  'Génération des questions...'],
    [75,  'Structuration...'],
    [92,  'Sauvegarde...'],
    [100, 'Terminé !']
  ];
  let i = 0;
  const timer = setInterval(() => {
    if (i < steps.length) {
      aiProgress.value   = steps[i][0];
      aiStatusText.value = steps[i][1];
      i++;
    } else {
      clearInterval(timer);
    }
  }, 400);
  return timer;
};

// ══════════════════════════════════════════════════════════════
//  IA — APPEL API — THÈME GARANTI DANS CHAQUE QUESTION
// ══════════════════════════════════════════════════════════════
const callAIAPI = async (lang) => {
  //  Snapshot immédiat des valeurs du formulaire
  const snapshotTheme     = (aiForm.theme     || '').trim();
  const snapshotSousTheme = (aiForm.sousTheme || '').trim();

  //  Validation préalable
  if (!snapshotTheme) {
    throw new Error('theme_required');
  }

  const fd = new FormData();
  fd.append('theme',     snapshotTheme);
  fd.append('sousTheme', snapshotSousTheme);
  fd.append('type',      aiForm.type);
  fd.append('n',         aiForm.n);
  fd.append('langue',    lang);

  const processQuestions = (rawList) => {
    return (rawList || []).map(q => ({
      ...q,
      langue:    lang,
      lang:      lang,
      // ✅ FIX PRINCIPAL : theme et sousTheme garantis depuis le snapshot
      theme:     snapshotTheme,
      sousTheme: snapshotSousTheme,
    }));
  };

  try {
    const r = await axios.post(`${AI_BASE}/ia/generate-bilingual`, fd);
    return processQuestions(r.data.questions);
  } catch {
    // Fallback
    const fd2 = new FormData();
    fd2.append('theme',     snapshotTheme);
    fd2.append('sousTheme', snapshotSousTheme);
    fd2.append('n',         aiForm.n);
    fd2.append('langue',    lang);
    const r2 = await axios.post(`${AI_BASE}/ia/generate-ultra`, fd2);
    return processQuestions(r2.data.questions);
  }
};

// ══════════════════════════════════════════════════════════════
// ✅ IA — VALIDATION THÈME / SOUS-THÈME
// ══════════════════════════════════════════════════════════════
const validateAIForm = () => {
  aiAttempted.value = true;

  if (!aiForm.theme) {
    showToast('⚠️ Veuillez sélectionner un thème', 'error');
    return false;
  }
  if (!aiForm.sousTheme) {
    // ✅ Si pas de sous-thème disponible, on accepte avec le thème seul
    if (aiDynamicSubCategories.value.length === 0) {
      // Auto-set sousTheme to theme value when no sub-categories exist
      aiForm.sousTheme = aiForm.theme;
      showToast(`ℹ️ Sous-thème auto-défini sur "${aiForm.theme}"`, 'info');
    } else {
      showToast('⚠️ Veuillez sélectionner un sous-thème', 'error');
      return false;
    }
  }
  if (aiForm.n < 5 || aiForm.n > 100) {
    showToast('Le nombre de questions doit être entre 5 et 100', 'error');
    return false;
  }
  return true;
};

// ══════════════════════════════════════════════════════════════
// IA — APERÇU
// ══════════════════════════════════════════════════════════════
const previewAI = async () => {
  if (!validateAIForm()) return;

  isAILoading.value = true;
  aiPreview.value   = [];
  const timer = simulateProgress();

  try {
    const res = aiForm.langue === 'both'
      ? [...await callAIAPI('fr'), ...await callAIAPI('en')]
      : await callAIAPI(aiForm.langue);

    aiPreview.value = res;
    showToast(`${res.length} questions générées en aperçu (${aiForm.theme} › ${aiForm.sousTheme})`, 'success');
  } catch (err) {
    if (err.message === 'theme_required') {
      showToast('⚠️ Thème requis avant de générer', 'error');
    } else {
      console.error('[previewAI]', err);
      showToast(t('error'), 'error');
    }
  } finally {
    clearInterval(timer);
    isAILoading.value = false;
    aiProgress.value  = 0;
  }
};

// ══════════════════════════════════════════════════════════════
// ✅ IA — GÉNÉRATION + SAUVEGARDE — THÈME GARANTI
// ══════════════════════════════════════════════════════════════
const handleAIGeneration = async () => {
  if (!validateAIForm()) return;

  // ✅ Snapshot des valeurs AVANT tout appel async
  const snapshotTheme      = (aiForm.theme     || '').trim();
  const snapshotSousTheme  = (aiForm.sousTheme || '').trim();
  const snapshotType       = aiForm.type;
  const snapshotDifficulty = aiForm.difficulty;

  // Vérification quota
  try {
    await api.post(`/Usage/validate-action?questionCount=${aiForm.n}`);
  } catch (err) {
    if (err.response?.status === 403) {
      if (err.response.data?.error === 'MAX_QUESTIONS_EXCEEDED') {
        showToast('Limite de 100 questions dépassée', 'error');
        return;
      }
      showAIModal.value = false;
      const secondsLeft = err.response.data?.retryAfterSeconds || 0;
      const h = Math.floor(secondsLeft / 3600);
      const m = Math.floor((secondsLeft % 3600) / 60);
      const s = secondsLeft % 60;
      const timeStr = h > 0 ? `${h}h ${m}m ${s}s` : `${m}m ${s}s`;

      Swal.fire({
        title: '<h2 style="font-size:2rem;font-weight:600;color:#1e293b;margin-top:1rem">Limite atteinte</h2>',
        html: `
          <div style="padding:1rem 2rem">
            <p style="color:#64748b;font-size:1.05rem;margin-bottom:1.5rem">
              Le plan Starter est limité à <b>3 générations</b> par 24h.
            </p>
            <div style="background:#fff1f2;border:1px solid #fecaca;border-radius:8px;
                        padding:1.2rem;margin-bottom:1.5rem;display:flex;
                        align-items:center;justify-content:center;gap:12px">
              <i class="fa-solid fa-rotate-left" style="color:#ef4444;font-size:1.3rem"></i>
              <span style="color:#be123c;font-size:1.1rem;font-weight:500">Réessayez dans :</span>
              <span style="background:#ef4444;color:white;padding:4px 12px;
                           border-radius:6px;font-weight:700">${timeStr}</span>
            </div>
            <div style="background:#fffbeb;border:1px solid #fde68a;border-radius:8px;padding:1.2rem">
              <p style="color:#92400e;margin:0;line-height:1.6">
                Passez à <b>EvaluaTech Go</b> pour des questions illimitées.
              </p>
            </div>
          </div>`,
        showCancelButton: true,
        confirmButtonText: 'Passer à EvaluaTech Go',
        cancelButtonText:  t('cancel'),
        confirmButtonColor: '#eab308',
        cancelButtonColor:  '#f1f5f9',
        background: '#fff',
        width: '580px',
        customClass: { popup: 'rounded-4 border-0 shadow-lg' },
        didOpen: () => {
          const c = Swal.getConfirmButton();
          const x = Swal.getCancelButton();
          if (c) Object.assign(c.style, { color:'#000', fontWeight:'700', padding:'12px 28px', borderRadius:'8px', fontSize:'1rem' });
          if (x) Object.assign(x.style, { color:'#475569', fontWeight:'500', padding:'12px 28px', borderRadius:'8px', fontSize:'1rem', backgroundColor:'#f1f5f9', border:'none' });
        }
      }).then(r => { if (r.isConfirmed) router.push('/pricing'); });
      return;
    }
  }

  isAILoading.value = true;
  const timer = simulateProgress();

  try {
    const all = aiForm.langue === 'both'
      ? [...await callAIAPI('fr'), ...await callAIAPI('en')]
      : await callAIAPI(aiForm.langue);

    // ✅ SAUVEGARDE : chaque question porte déjà theme + sousTheme depuis callAIAPI
    // On utilise le snapshot en fallback ultime de sécurité
    let savedCount = 0;
    for (const q of all) {
      // ✅ Triple fallback pour garantir que theme/sousTheme ne soient JAMAIS vides
      const themeToSave     = (q.theme     && q.theme.trim())     ? q.theme.trim()     : snapshotTheme;
      const sousThemeToSave = (q.sousTheme && q.sousTheme.trim()) ? q.sousTheme.trim() : snapshotSousTheme || snapshotTheme;

      try {
        await api.post('/Questions', {
          enonce:       q.question,
          type:         snapshotType,
          points:       snapshotDifficulty,
          theme:        themeToSave,        // ✅ JAMAIS vide
          sousTheme:    sousThemeToSave,    // ✅ JAMAIS vide
          langue:       q.langue,           // ✅ 'fr' ou 'en' forcé par callAIAPI
          choix:        q.options || [],
          bonneReponse: Array.isArray(q.options) && q.answer != null
            ? (q.options[q.answer] ?? '')
            : ''
        });
        savedCount++;
      } catch (saveErr) {
        console.error('[save question error]', saveErr, { themeToSave, sousThemeToSave });
      }
    }

    showAIModal.value = false;
    aiPreview.value   = [];
    aiAttempted.value = false;
    await fetchData();

    const fr = all.filter(q => q.langue === 'fr').length;
    const en = all.filter(q => q.langue === 'en').length;
    showToast(
      `✅ ${savedCount} questions sauvegardées sous "${snapshotTheme} › ${snapshotSousTheme}" (${fr} 🇫🇷 / ${en} 🇬🇧)`,
      'success'
    );
  } catch (err) {
    console.error('[handleAIGeneration]', err);
    if (err.message === 'theme_required') {
      showToast('⚠️ Thème requis avant de générer', 'error');
    } else {
      showToast(t('error'), 'error');
    }
  } finally {
    clearInterval(timer);
    isAILoading.value = false;
    aiProgress.value  = 0;
  }
};

// ══════════════════════════════════════════════════════════════
// CATÉGORIES
// ══════════════════════════════════════════════════════════════
const addCategory = async () => {
  if (!newCatName.value.trim()) return;
  try {
    const res = await api.post('/Categories', { nom: newCatName.value.trim() });
    categoriesList.value.push(res.data);
    newCatName.value = '';
    showToast(t('success'), 'success');
  } catch { showToast(t('error'), 'error'); }
};

const handleSubAdd = async (catId) => {
  const val = subCatInputs[catId];
  if (!val?.trim()) return;
  try {
    const res = await api.post(`/Categories/${catId}/sub`, { nom: val.trim() });
    const cat = categoriesList.value.find(c => c.id === catId);
    if (cat) {
      if (!cat.sousCategories) cat.sousCategories = [];
      cat.sousCategories.push(res.data);
    }
    subCatInputs[catId] = '';
    showToast(t('success'), 'success');
  } catch { showToast(t('error'), 'error'); }
};

const removeCategory = async (id) => {
  if (!confirm(t('confirm') + ' ?')) return;
  try {
    await api.delete(`/Categories/${id}`);
    categoriesList.value = categoriesList.value.filter(c => c.id !== id);
    showToast(t('delete'), 'info');
  } catch { showToast(t('error'), 'error'); }
};

const removeSubCategory = async (subId) => {
  if (!confirm(t('confirm') + ' ?')) return;
  try {
    await api.delete(`/Categories/sub/${subId}`);
    await fetchData();
    showToast(t('delete'), 'info');
  } catch { showToast(t('error'), 'error'); }
};

// ══════════════════════════════════════════════════════════════
// FORM — MODAL ÉDITION / CRÉATION
// ══════════════════════════════════════════════════════════════
const handleTypeChange = (newType) => {
  form.type = newType;
  if (newType === 2) {
    form.reponses = [
      { texte: form.langue === 'en' ? 'True'  : 'Vrai', estCorrecte: true  },
      { texte: form.langue === 'en' ? 'False' : 'Faux', estCorrecte: false }
    ];
  } else if ([0, 1].includes(newType)) {
    if (form.reponses.length < 2) {
      form.reponses = [
        { texte: '', estCorrecte: true  },
        { texte: '', estCorrecte: false }
      ];
    }
  } else {
    form.reponses = [];
  }
};

const addResponse    = () => form.reponses.push({ texte: '', estCorrecte: false });
const removeResponse = (i) => form.reponses.splice(i, 1);

const openModal = (q = null) => {
  isEdit.value = !!q;

  if (q) {
    Object.assign(form, {
      id:           q.id,
      enonce:       q.enonce       || '',
      type:         q.type         ?? 0,
      points:       q.points       ?? 1,
      theme:        q.theme        || '',
      sousTheme:    q.sousTheme    || '',
      bonneReponse: q.bonneReponse || '',
      langue:       resolveQuestionLang(q),
      reponses:     (q.choix || []).map(opt => ({
        texte:       opt,
        estCorrecte: (q.bonneReponse || '').split('|').includes(opt)
      }))
    });
  } else {
    Object.assign(form, {
      id: '', enonce: '', type: 0, points: 1,
      theme: '', sousTheme: '', bonneReponse: '',
      langue: 'fr',
      reponses: [
        { texte: '', estCorrecte: true  },
        { texte: '', estCorrecte: false }
      ]
    });
  }

  showModal.value = true;
};

const save = async () => {
  if (!form.enonce.trim()) return;
  isSaving.value = true;

  try {
    let finalBR = '';
    if (form.type === 0 || form.type === 2) {
      finalBR = form.reponses[correctRadioIndex.value]?.texte || '';
    } else if (form.type === 1) {
      finalBR = form.reponses.filter(r => r.estCorrecte).map(r => r.texte).join('|');
    } else {
      finalBR = form.bonneReponse;
    }

    const payload = {
      enonce:       form.enonce.trim(),
      type:         form.type,
      points:       form.points || 1,
      theme:        form.theme     || '',
      sousTheme:    form.sousTheme || '',
      langue:       form.langue,
      choix:        form.reponses.map(r => r.texte).filter(tx => tx?.trim()),
      bonneReponse: finalBR
    };

    if (isEdit.value) {
      await api.put(`/Questions/${form.id}`, payload);
    } else {
      await api.post('/Questions', payload);
    }

    showModal.value = false;
    await fetchData();
    showToast(t('success'), 'success');
  } catch (err) {
    console.error('[save]', err);
    showToast(t('error'), 'error');
  } finally {
    isSaving.value = false;
  }
};

const handleDelete = async (id) => {
  if (!confirm(t('confirm') + ' ?')) return;
  try {
    await api.delete(`/Questions/${id}`);
    await fetchData();
    showToast(t('delete'), 'info');
  } catch { showToast(t('error'), 'error'); }
};

// ══════════════════════════════════════════════════════════════
// UTILS
// ══════════════════════════════════════════════════════════════
const getTypeInfo = (val) =>
  typeDefinitions.find(t => t.val === val) ?? typeDefinitions[0];

const getLevelColor = (p) =>
  p >= 4 ? '#ef4444' : p >= 3 ? '#f59e0b' : p >= 2 ? '#10b981' : '#3b82f6';

const getPlaceholder = (tp, lang) => {
  if (tp === 5) return lang === 'en' ? '// Enter reference code...' : '// Code de référence...';
  if (tp === 4) return lang === 'en' ? 'Expected answer...' : 'Réponse attendue...';
  return lang === 'en' ? 'Answer...' : 'Réponse...';
};

const resetFilters = () => {
  searchQuery.value  = '';
  activeFilter.value = -1;
  selectedCat.value  = 'All';
  filterLang.value   = 'all';
};

let _toastT = null;
const showToast = (message, type = 'success') => {
  clearTimeout(_toastT);
  toast.message = message;
  toast.type    = type;
  toast.icon    = type === 'success' ? 'fa-solid fa-circle-check'
                : type === 'error'   ? 'fa-solid fa-circle-xmark'
                :                      'fa-solid fa-circle-info';
  toast.active  = true;
  _toastT = setTimeout(() => { toast.active = false; }, 3500);
};

const orbStyle = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`
});

const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

const isGlobalDark = ref(
  document.documentElement.getAttribute('data-theme') === 'dark'
);

const toggleGlobalTheme = () => {
  const next = isGlobalDark.value ? 'light' : 'dark';
  isGlobalDark.value = !isGlobalDark.value;
  document.documentElement.setAttribute('data-theme', next);
  localStorage.setItem('theme_preferé_evalua', next);
};

// ══════════════════════════════════════════════════════════════
// PAGINATION LOGIC
// ══════════════════════════════════════════════════════════════
const currentPage = ref(1);
const itemsPerPage = ref(12);

const paginatedQuestions = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredQuestions.value.slice(start, end);
});

const totalPages = computed(() => {
  return Math.ceil(filteredQuestions.value.length / itemsPerPage.value);
});

const pageNumbers = computed(() => {
  const pages = [];
  const maxVisiblePages = 5;
  let start = Math.max(1, currentPage.value - Math.floor(maxVisiblePages / 2));
  let end = Math.min(totalPages.value, start + maxVisiblePages - 1);

  if (end - start + 1 < maxVisiblePages) {
    start = Math.max(1, end - maxVisiblePages + 1);
  }

  for (let i = start; i <= end; i++) {
    pages.push(i);
  }
  return pages;
});

const setPage = (page) => {
  if (page < 1 || page > totalPages.value) return;
  currentPage.value = page;
  
  // Smooth scroll back to top of container on page change
  const container = document.querySelector('.canvas-engine');
  if (container) {
    container.scrollTo({ top: 0, behavior: 'smooth' });
  }
};

watch([searchQuery, activeFilter, selectedCat, filterLang, itemsPerPage], () => {
  currentPage.value = 1;
});

onMounted(() => fetchData());
</script>

<style scoped>
/* ════════════════════════════════════════
   CSS CUSTOM PROPERTIES — Light Mode
════════════════════════════════════════ */
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:ital,wght@0,400;0,500;0,600;0,700;0,800;0,900;1,700&display=swap');

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

/* ════════════════════════════════════════
   DARK MODE
════════════════════════════════════════ */
[data-theme="dark"] .bq-root {
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

/* ════════════════════════════════════════
   BACKGROUND
════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.18; transition: opacity 0.3s;
}
[data-theme="dark"] .quantum-grid { opacity: 0.06; }
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.12; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-slate { background: #6366f1; bottom: -200px; left: -100px; }
[data-theme="dark"] .glow-orb { opacity: 0.07; }

.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ════════════════════════════════════════
   HEADER
════════════════════════════════════════ */
.bq-header { animation: slideDown 0.6s var(--ease-out) backwards; }
@keyframes slideDown {
  from { opacity: 0; transform: translateY(-24px); }
  to   { opacity: 1; transform: none; }
}
.breadcrumb-pro {
  font-size: 0.72rem; font-weight: 700; color: var(--text3);
  display: flex; align-items: center;
  padding: 6px 12px; background: var(--surface);
  border: 1px solid var(--bdr); border-radius: 50px;
  width: fit-content; gap: 4px;
}
.breadcrumb-pro .root { cursor: pointer; transition: color 0.2s; }
.breadcrumb-pro .root:hover { color: var(--amber); }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: var(--amber); font-weight: 800; }
.premium-title {
  font-weight: 900; font-size: 2.4rem; letter-spacing: -1.5px;
  color: var(--text); margin: 0; line-height: 1.05;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 50%, #f97316 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
  font-style: italic;
}
.brand-subtitle-v2 { font-size: 0.8rem; font-weight: 600; color: var(--text3); margin-top: 8px; }
.live-dot-wrap { position: relative; display: inline-flex; align-items: center; justify-content: center; width: 18px; height: 18px; }
.live-dot  { width: 8px; height: 8px; background: var(--amber); border-radius: 50%; }
.live-ring { position: absolute; inset: 0; border: 2px solid rgba(245,158,11,0.4); border-radius: 50%; animation: livePulse 2.2s ease-out infinite; }
@keyframes livePulse { 0% { transform: scale(0.5); opacity: 0.8; } 100% { transform: scale(1.6); opacity: 0; } }

/* ════════════════════════════════════════
   BUTTONS
════════════════════════════════════════ */
.header-actions-group  { display: flex; align-items: center; gap: 15px; flex-wrap: wrap; }
.action-buttons-wrap   { display: flex; align-items: center; gap: 10px; }

.btn-refresh-pro {
  width: 44px; height: 44px; background: var(--surface);
  border: 1.5px solid var(--bdr); border-radius: 14px;
  color: var(--text2); font-size: 14px; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  transition: all 0.25s var(--ease-spring); box-shadow: var(--shadow-sm);
}
.btn-refresh-pro:hover { background: var(--amber); color: #0f172a; border-color: var(--amber); transform: translateY(-2px) rotate(15deg); }

.search-inline-box {
  display: flex; align-items: center; gap: 10px;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 10px 14px;
  box-shadow: var(--shadow-sm); transition: all 0.25s; color: var(--text3);
}
.search-inline-box.focused { border-color: var(--amber); box-shadow: 0 0 0 4px rgba(245,158,11,0.1); color: var(--amber); }
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

.btn-ai-glow {
  position: relative; overflow: hidden;
  display: flex; align-items: center;
  background: #0f172a; border: none; border-radius: 14px;
  padding: 10px 20px; font-size: 13px; font-weight: 800;
  color: #fff; cursor: pointer; font-family: inherit;
  box-shadow: 0 4px 20px rgba(15,23,42,0.2); transition: all 0.3s;
}
.btn-ai-glow:hover { transform: translateY(-3px); box-shadow: 0 12px 32px rgba(245,158,11,0.35); color: #0f172a; }
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

/* ✅ SELECT MODE BUTTON */
.btn-select-mode {
  display: flex; align-items: center;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 10px 14px;
  font-size: 13px; font-weight: 800; color: var(--text2);
  cursor: pointer; transition: all 0.22s; font-family: inherit;
  box-shadow: var(--shadow-sm);
}
.btn-select-mode:hover { border-color: var(--amber-bdr); color: var(--amber-dark); background: var(--amber-light); }
.btn-select-mode.active { background: #0f172a; color: white; border-color: #0f172a; }
[data-theme="dark"] .btn-select-mode.active { background: var(--amber); color: #0f172a; border-color: var(--amber); }

/* ════════════════════════════════════════
   ✅ BULK ACTION BAR
════════════════════════════════════════ */
.bulk-action-bar {
  display: flex; align-items: center; gap: 12px; flex-wrap: wrap;
  background: var(--surface); border: 2px solid var(--amber-bdr);
  border-radius: 20px; padding: 14px 20px;
  box-shadow: 0 4px 20px rgba(245,158,11,0.12);
  animation: slideDown 0.3s var(--ease-out) backwards;
}
.bulk-info { display: flex; align-items: center; gap: 10px; }
.bulk-count-badge {
  background: var(--amber); color: #0f172a;
  padding: 6px 14px; border-radius: 50px;
  font-size: 12px; font-weight: 900; letter-spacing: 0.3px;
}
.bulk-label { font-size: 12px; font-weight: 600; color: var(--text3); }
.bulk-actions { display: flex; align-items: center; gap: 8px; flex-wrap: wrap; margin-left: auto; }
.bulk-separator { width: 1px; height: 24px; background: var(--bdr); }
.bulk-btn {
  display: flex; align-items: center;
  padding: 7px 14px; border-radius: 10px;
  font-size: 12px; font-weight: 800; cursor: pointer;
  border: 1.5px solid var(--bdr); background: var(--surface2);
  color: var(--text2); transition: all 0.2s; font-family: inherit;
}
.bulk-btn:hover { transform: translateY(-1px); }
.bulk-select-all:hover { background: #eff6ff; color: #2563eb; border-color: #bfdbfe; }
.bulk-deselect:hover { background: var(--surface); color: var(--text); }
.bulk-export:hover { background: #ecfdf5; color: #059669; border-color: #a7f3d0; }
.bulk-delete { color: #ef4444; border-color: #fecaca; }
.bulk-delete:hover { background: #fff1f2; border-color: #fca5a5; color: #dc2626; }

.select-hint-bar {
  background: var(--amber-light); border: 1px dashed var(--amber-bdr);
  border-radius: 14px; padding: 10px 16px;
  font-size: 12px; font-weight: 700; color: var(--amber-dark);
  display: flex; align-items: center; flex-wrap: wrap;
}
.select-hint-btn {
  background: var(--amber); color: #0f172a; border: none;
  padding: 4px 12px; border-radius: 8px; font-size: 11px;
  font-weight: 800; cursor: pointer; font-family: inherit;
  transition: all 0.2s;
}
.select-hint-btn:hover { transform: scale(1.05); }

/* ════════════════════════════════════════
   ✅ CARD SELECTION STYLES
════════════════════════════════════════ */
.card-select-mode { cursor: pointer !important; }
.card-select-mode:hover { border-color: var(--amber-bdr) !important; }

.card-selected {
  border-color: var(--amber) !important;
  box-shadow: 0 0 0 3px rgba(245,158,11,0.2), var(--shadow-md) !important;
  transform: translateY(-4px) scale(1.005) !important;
}

.select-overlay {
  position: absolute; top: 12px; right: 52px; z-index: 10;
}
.select-checkbox {
  width: 24px; height: 24px; border-radius: 8px;
  border: 2px solid var(--bdr2); background: var(--surface);
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; transition: all 0.2s var(--ease-spring);
  font-size: 11px; color: white;
}
.select-checkbox:hover { border-color: var(--amber); }
.select-checkbox.checked { background: var(--amber); border-color: var(--amber); color: #0f172a; font-weight: 900; }

.select-checkbox-sm {
  width: 20px; height: 20px; border-radius: 6px;
  border: 2px solid var(--bdr2); background: var(--surface);
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; transition: all 0.2s; font-size: 10px; color: white;
}
.select-checkbox-sm:hover { border-color: var(--amber); }
.select-checkbox-sm.checked { background: var(--amber); border-color: var(--amber); color: #0f172a; font-weight: 900; }

.row-selected { background: var(--amber-light) !important; border-color: var(--amber-bdr) !important; }

/* ✅ Non classé visual indicator */
.cat-pill-unclassified {
  background: var(--surface2) !important;
  border-color: var(--bdr2) !important;
  color: var(--text3) !important;
  opacity: 0.7;
  font-style: italic;
}
.chip-unclassified { color: var(--text3) !important; font-style: italic; }

/* ✅ Required badge */
.required-badge {
  font-size: 9px; font-weight: 900; color: #ef4444;
  background: #fee2e2; padding: 2px 8px; border-radius: 20px;
}

/* ✅ Field required glow */
.field-required-glow .enigma-field {
  border-color: #ef4444 !important;
  box-shadow: 0 0 0 3px rgba(239,68,68,0.1) !important;
}

/* ✅ Theme preview badge */
.theme-preview-badge {
  display: flex; align-items: center;
  background: linear-gradient(135deg, var(--amber-light), rgba(245,158,11,0.05));
  border: 1.5px solid var(--amber-bdr); border-radius: 14px;
  padding: 10px 16px; font-size: 13px;
}
.theme-preview-path { font-weight: 800; color: var(--amber-dark); }
.theme-preview-sub { font-weight: 700; color: var(--text2); }
.theme-preview-tick {
  width: 20px; height: 20px; border-radius: 50%;
  background: #10b981; color: white; font-size: 9px;
  display: flex; align-items: center; justify-content: center;
}

/* ════════════════════════════════════════
   STAT CARDS
════════════════════════════════════════ */
.stat-card-premium {
  background: var(--surface); border-radius: 24px; padding: 22px;
  display: flex; align-items: center; gap: 14px;
  border: 1.5px solid var(--bdr); transition: all 0.3s var(--ease-out);
  box-shadow: var(--shadow-sm); animation: slideUp 0.5s var(--ease-out) backwards;
  position: relative; overflow: hidden;
}
.stat-card-premium::before {
  content: ''; position: absolute; inset: 0;
  background: linear-gradient(135deg, transparent 60%, rgba(245,158,11,0.04));
  opacity: 0; transition: opacity 0.3s;
}
.stat-card-premium:hover::before { opacity: 1; }
.stat-card-premium:hover { transform: translateY(-5px); box-shadow: var(--shadow-md); border-color: var(--amber-bdr); }
@keyframes slideUp { from { opacity: 0; transform: translateY(16px); } to { opacity: 1; transform: none; } }
.stat-icon-wrapper {
  width: 52px; height: 52px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.3rem; flex-shrink: 0; transition: transform 0.3s var(--ease-spring);
}
.stat-card-premium:hover .stat-icon-wrapper { transform: scale(1.12) rotate(-6deg); }
.stat-value { font-size: 1.9rem; font-weight: 900; color: var(--text); display: block; line-height: 1; letter-spacing: -1.5px; }
.stat-label { font-size: 0.62rem; font-weight: 700; color: var(--text3); margin-top: 4px; display: block; text-transform: uppercase; letter-spacing: 0.8px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; gap: 2px; font-size: 0.6rem; font-weight: 800; padding: 6px 10px; border-radius: 10px; }
.trend-up { color: #10b981; background: #ecfdf5; }

/* ════════════════════════════════════════
   TOOLBAR
════════════════════════════════════════ */
.tabs-pill-wrap { background: var(--surface) !important; border-color: var(--bdr) !important; }
.nav-tab-btn-modern {
  padding: 8px 14px; border-radius: 12px; border: none;
  background: transparent; font-weight: 800; font-size: 0.78rem;
  color: var(--text3); cursor: pointer; transition: all 0.2s; font-family: inherit;
  display: flex; align-items: center; gap: 6px;
}
.nav-tab-btn-modern:hover { background: var(--amber-light); color: var(--amber-dark); }
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
[data-theme="dark"] .nav-tab-btn-modern.active { background: var(--amber); color: #0f172a; }
.tab-count { background: rgba(255,255,255,0.15); padding: 2px 7px; border-radius: 8px; font-size: 0.62rem; margin-left: 2px; }
.nav-tab-btn-modern:not(.active) .tab-count { background: var(--surface2); color: var(--text3); }

.sort-select-wrap {
  display: flex; align-items: center; gap: 8px;
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 12px; padding: 9px 12px; transition: all 0.2s;
}
.sort-select-wrap:focus-within { border-color: var(--amber); box-shadow: 0 0 0 3px rgba(245,158,11,0.1); }
.sort-ico   { color: var(--text3); font-size: 11px; }
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
[data-theme="dark"] .btn-view-toggle.active { background: var(--amber); color: #0f172a; }

/* ════════════════════════════════════════
   STATES
════════════════════════════════════════ */
.state-label { font-size: 11px; font-weight: 800; color: var(--text3); letter-spacing: 2px; display: flex; align-items: center; justify-content: center; gap: 8px; }
.spinner-pro-premium { width: 50px; height: 50px; border: 4px solid var(--bdr); border-top: 4px solid var(--amber); border-radius: 50%; animation: spin 1s linear infinite; margin: 40px auto 0; }
@keyframes spin { to { transform: rotate(360deg); } }
.empty-state-pro { background: var(--surface); border-radius: 30px; padding: 40px; border: 1.5px dashed var(--bdr2); }
.empty-graphic { position: relative; width: 90px; height: 90px; margin: 0 auto; display: flex; align-items: center; justify-content: center; }
.empty-ring { position: absolute; inset: 0; border-radius: 50%; border: 1px solid var(--bdr); animation: haloSpin linear infinite; }
.r1 { animation-duration: 5s; } .r2 { inset: 12px; animation-duration: 8s; } .r3 { inset: 24px; animation-duration: 11s; }
@keyframes haloSpin { to { transform: rotate(360deg); } }
.empty-core { font-size: 30px; color: var(--amber); position: relative; z-index: 1; }

/* ════════════════════════════════════════
   GRID VIEW
════════════════════════════════════════ */
.questions-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(320px, 1fr)); gap: 20px; }
.q-card.campaign-card-modern {
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 28px; overflow: hidden; display: flex; flex-direction: column;
  box-shadow: var(--shadow-sm); position: relative;
  transition: all 0.35s cubic-bezier(0.4, 0, 0.2, 1);
  animation: cardAppear 0.5s var(--ease-out) backwards;
  animation-delay: var(--card-delay, 0s); padding: 0;
}
.q-card.campaign-card-modern::after {
  content: ''; position: absolute; inset: 0; border-radius: 28px;
  background: linear-gradient(135deg, rgba(245,158,11,0.04), transparent 60%);
  opacity: 0; transition: opacity 0.3s; pointer-events: none;
}
.q-card.campaign-card-modern:hover::after { opacity: 1; }
.q-card.campaign-card-modern:hover {
  transform: translateY(-10px) scale(1.01); border-color: var(--amber-bdr);
  box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08), 0 8px 30px rgba(245,158,11,0.15);
}
@keyframes cardAppear { from { opacity: 0; transform: translateY(20px) scale(0.97); } to { opacity: 1; transform: none; } }

.card-type-stripe { height: 3px; width: 100%; transition: height 0.3s; flex-shrink: 0; }
.q-card:hover .card-type-stripe { height: 5px; }

.card-lang-banner {
  display: flex; align-items: center; gap: 8px;
  padding: 6px 16px; font-size: 10px; font-weight: 800;
  letter-spacing: 0.5px; text-transform: uppercase;
}
.lang-fr { background: linear-gradient(90deg, rgba(254,226,226,0.8), transparent); color: #dc2626; border-bottom: 1px solid rgba(254,202,202,0.5); }
.lang-en { background: linear-gradient(90deg, rgba(239,246,255,0.8), transparent); color: #2563eb; border-bottom: 1px solid rgba(191,219,254,0.5); }
[data-theme="dark"] .lang-fr { background: linear-gradient(90deg, rgba(127,29,29,0.25), transparent); color: #fca5a5; border-bottom-color: rgba(127,29,29,0.3); }
[data-theme="dark"] .lang-en { background: linear-gradient(90deg, rgba(29,78,216,0.2), transparent); color: #93c5fd; border-bottom-color: rgba(29,78,216,0.25); }
.lang-banner-name { opacity: 0.8; }

.card-header-modern { display: flex; justify-content: space-between; align-items: center; padding: 12px 18px 0; }
.card-cat-pill {
  display: inline-flex; align-items: center; gap: 6px;
  background: var(--surface2); border: 1px solid var(--bdr);
  border-radius: 50px; padding: 5px 12px;
  font-size: 10px; font-weight: 700; color: var(--text2);
  max-width: 180px; overflow: hidden; transition: all 0.2s;
}
.card-cat-pill:hover { border-color: var(--amber-bdr); background: var(--amber-light); color: var(--amber-dark); }
.card-cat-pill span { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid var(--bdr); background: var(--surface2);
  display: flex; align-items: center; justify-content: center;
  font-size: 11px; cursor: pointer; color: var(--text3);
  transition: all 0.2s var(--ease-spring);
}
.btn-icon-sm:hover { background: #3b82f6; color: #fff; border-color: #3b82f6; transform: scale(1.1) rotate(-5deg); }
.btn-icon-sm.danger:hover { background: #ef4444; color: #fff; border-color: #ef4444; transform: scale(1.1); }

.type-badge-row {
  display: inline-flex; align-items: center; gap: 8px;
  padding: 6px 12px 6px 8px; margin: 10px 16px;
  border: 1.5px solid var(--badge-c, var(--amber));
  border-left-width: 3px; border-radius: 10px;
  background: color-mix(in srgb, var(--badge-c, var(--amber)) 6%, var(--surface));
  width: fit-content; transition: transform 0.25s var(--ease-spring);
}
.q-card:hover .type-badge-row { transform: translateX(6px); }
.type-badge-icon-box { width: 26px; height: 26px; border-radius: 7px; display: flex; align-items: center; justify-content: center; font-size: 12px; flex-shrink: 0; }
.type-badge-label { font-size: 11px; font-weight: 800; }
.type-badge-live { width: 5px; height: 5px; border-radius: 50%; margin-left: auto; animation: dotBlink 2s ease-in-out infinite; }
@keyframes dotBlink { 0%,100% { opacity: 0.4; } 50% { opacity: 1; } }

.card-enonce {
  flex: 1; padding: 0 18px 14px;
  font-size: 14px; font-weight: 700; color: var(--text);
  line-height: 1.65; margin: 0;
  display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden;
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
.opt-letter {
  width: 18px; height: 18px; border-radius: 5px; background: var(--bdr);
  color: var(--text3); font-size: 9px; font-weight: 900;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
}
.opt-text { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
.opt-more { font-size: 10px; color: var(--text3); font-style: italic; padding: 3px 8px; }

.card-footer-modern { padding: 10px 18px 16px; border-top-color: var(--bdr) !important; }
.level-indicator { display: flex; align-items: center; gap: 8px; }
.level-label-sm { font-size: 9px; font-weight: 800; color: var(--text3); letter-spacing: 0.8px; }
.level-dots { display: flex; gap: 4px; }
.ldot { width: 8px; height: 8px; border-radius: 50%; background: var(--bdr); transition: all 0.3s; }
.level-val { font-size: 12px; font-weight: 800; }
.slot-badge { padding: 4px 10px; border-radius: 20px; font-size: 10px; font-weight: 800; display: flex; align-items: center; }

/* ════════════════════════════════════════
   LIST VIEW
════════════════════════════════════════ */
.list-view-pro { display: flex; flex-direction: column; }
.list-header-row { background: var(--surface2); border-radius: 14px; border: 1px solid var(--bdr); }
.list-col-label { font-size: 0.6rem; font-weight: 900; color: var(--text3); text-transform: uppercase; letter-spacing: 1px; }
.list-row-item {
  background: var(--surface); border-radius: 16px;
  border: 1.5px solid var(--bdr); transition: all 0.25s var(--ease-out);
  animation: rowAppear 0.4s var(--ease-out) backwards;
  animation-delay: var(--row-delay, 0s);
}
.list-row-item:hover {
  border-color: var(--amber-bdr); transform: translateX(6px);
  box-shadow: 0 8px 24px rgba(245,158,11,0.08), -4px 0 0 var(--amber);
}
@keyframes rowAppear { from { opacity: 0; transform: translateX(-16px); } to { opacity: 1; transform: none; } }

.row-type-badge { display: inline-flex; align-items: center; padding: 5px 10px; border-radius: 10px; border: 1px solid; font-size: 11px; font-weight: 700; flex-shrink: 0; }
.row-lang-chip { padding: 3px 9px; border-radius: 20px; font-size: 10px; font-weight: 800; }
.lc-fr { background: rgba(254,226,226,0.7); color: #dc2626; border: 1px solid rgba(254,202,202,0.7); }
.lc-en { background: rgba(239,246,255,0.7); color: #2563eb; border: 1px solid rgba(191,219,254,0.7); }
[data-theme="dark"] .lc-fr { background: rgba(127,29,29,0.3); color: #fca5a5; border-color: rgba(127,29,29,0.4); }
[data-theme="dark"] .lc-en { background: rgba(29,78,216,0.2); color: #93c5fd; border-color: rgba(29,78,216,0.3); }

.meta-chip {
  font-size: 10px; font-weight: 600; color: var(--text3);
  background: var(--surface2); padding: 2px 8px;
  border-radius: 50px; border: 1px solid var(--bdr);
  display: inline-flex; align-items: center;
  max-width: 140px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;
}

/* ════════════════════════════════════════
   MODALS
════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; z-index: 1000;
  background: rgba(15,23,42,0.7);
  backdrop-filter: blur(20px) saturate(1.6);
  display: flex; align-items: center; justify-content: center; padding: 20px;
}
.quantum-vault-window {
  background: var(--surface); border: 1.5px solid var(--bdr);
  border-radius: 32px; width: 100%; max-width: 860px;
  box-shadow: 0 40px 80px rgba(0,0,0,0.2); overflow: hidden; position: relative;
  display: flex; flex-direction: column; max-height: 90vh;
}
.modal-md { max-width: 760px; }
.modal-corner { position: absolute; width: 16px; height: 16px; pointer-events: none; z-index: 2; }
.tl { top: 10px; left: 10px;  border-top: 2.5px solid var(--amber); border-left: 2.5px solid var(--amber); border-radius: 5px 0 0 0; }
.tr { top: 10px; right: 10px; border-top: 2.5px solid var(--amber); border-right: 2.5px solid var(--amber); border-radius: 0 5px 0 0; }
.bl { bottom: 10px; left: 10px;  border-bottom: 2.5px solid var(--amber); border-left: 2.5px solid var(--amber); border-radius: 0 0 0 5px; }
.br { bottom: 10px; right: 10px; border-bottom: 2.5px solid var(--amber); border-right: 2.5px solid var(--amber); border-radius: 0 0 5px 0; }

.qv-header {
  display: flex; align-items: center; gap: 16px;
  padding: 22px 28px; border-bottom: 1.5px solid var(--bdr); flex-shrink: 0;
  background: linear-gradient(135deg, var(--surface), var(--surface2));
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
  display: flex; align-items: center; justify-content: center;
  transition: all 0.25s var(--ease-spring); flex-shrink: 0;
}
.btn-modal-close:hover { background: #fef2f2; color: #ef4444; border-color: #fca5a5; transform: rotate(90deg) scale(1.1); }

.modal-body-scroll { overflow-y: auto; flex: 1; }
.modal-footer-actions {
  padding: 16px 28px;
  background: linear-gradient(to top, var(--surface2), var(--surface));
  border-top: 1.5px solid var(--bdr);
  display: flex; justify-content: flex-end; gap: 10px; flex-shrink: 0;
}

/* ════════════════════════════════════════
   FORM ELEMENTS
════════════════════════════════════════ */
.enigma-input-wrap label {
  display: flex; align-items: center; justify-content: space-between;
  font-size: 0.6rem; font-weight: 900; color: var(--text3);
  letter-spacing: 1.2px; text-transform: uppercase; margin-bottom: 10px;
}
.enigma-field {
  width: 100%; padding: 12px 16px;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 14px; font-weight: 600; outline: none;
  font-family: inherit; transition: all 0.25s; font-size: 13px; color: var(--text);
}
.enigma-field:focus { border-color: var(--amber); background: var(--surface); box-shadow: 0 0 0 4px rgba(245,158,11,0.1); }
.enigma-field:disabled { opacity: 0.4; cursor: not-allowed; }
.char-counter { position: absolute; bottom: 10px; right: 12px; font-size: 10px; color: var(--text3); font-weight: 600; pointer-events: none; }

.lang-cards-grid { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 10px; }
.lang-card {
  background: var(--surface2); border: 2px solid var(--bdr);
  border-radius: 16px; padding: 16px 12px; text-align: center;
  cursor: pointer; transition: all 0.25s var(--ease-spring); position: relative;
}
.lang-card:hover { border-color: var(--amber-bdr); background: var(--surface); transform: translateY(-2px); }
.lang-card-active { border-color: var(--amber); background: var(--amber-light); box-shadow: 0 4px 16px rgba(245,158,11,0.2); transform: translateY(-2px); }
.lc-flag { font-size: 26px; margin-bottom: 6px; display: block; }
.lc-name { font-size: 13px; font-weight: 800; color: var(--text); margin-bottom: 3px; display: block; }
.lc-desc { font-size: 10px; color: var(--text3); }
.lc-check {
  position: absolute; top: 8px; right: 8px;
  width: 18px; height: 18px; border-radius: 50%;
  background: var(--amber); color: #0f172a; font-size: 9px;
  display: flex; align-items: center; justify-content: center;
  opacity: 0; transform: scale(0); transition: all 0.25s var(--ease-spring);
}
.lang-card-active .lc-check { opacity: 1; transform: scale(1); }

.type-tiles-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(110px, 1fr)); gap: 8px; }
.type-tile {
  background: var(--surface2); border: 1.5px solid var(--bdr);
  border-radius: 14px; padding: 14px 8px; text-align: center;
  cursor: pointer; transition: all 0.25s var(--ease-spring);
  display: flex; flex-direction: column; align-items: center; gap: 8px;
  position: relative; overflow: hidden;
}
.type-tile:hover { border-color: var(--amber-bdr); background: var(--amber-light); transform: translateY(-2px); }
.type-tile-active { border-color: var(--tile-c, var(--amber)); background: var(--surface); box-shadow: 0 4px 16px rgba(245,158,11,0.12); transform: translateY(-2px); }
.tile-icon-wrap {
  width: 38px; height: 38px; border-radius: 11px;
  display: flex; align-items: center; justify-content: center; font-size: 17px;
  background: color-mix(in srgb, currentColor 12%, transparent);
  transition: all 0.3s var(--ease-spring);
}
.type-tile:hover .tile-icon-wrap,
.type-tile-active .tile-icon-wrap { transform: scale(1.12) rotate(-5deg); }
.tile-label { font-size: 10px; font-weight: 700; color: var(--text2); }
.tile-check {
  position: absolute; top: 6px; right: 6px;
  width: 16px; height: 16px; border-radius: 50%;
  background: var(--amber); color: #0f172a; font-size: 8px;
  display: flex; align-items: center; justify-content: center;
  opacity: 0; transform: scale(0); transition: all 0.25s var(--ease-spring);
}
.type-tile-active .tile-check { opacity: 1; transform: scale(1); }

.theme-select-wrapper { position: relative; }
.theme-select-icon { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); color: var(--amber); font-size: 12px; z-index: 2; pointer-events: none; }
.theme-select { padding-left: 38px !important; appearance: none; -webkit-appearance: none; cursor: pointer; }
.disabled-wrapper .theme-select-icon { color: var(--text3); }
.disabled-wrapper { opacity: 0.5; pointer-events: none; }

.lang-toggle-btn {
  padding: 10px; border-radius: 12px;
  background: var(--surface2); border: 1.5px solid var(--bdr);
  font-size: 13px; font-weight: 700; cursor: pointer; color: var(--text2);
  transition: all 0.2s var(--ease-spring); font-family: inherit;
}
.lang-toggle-btn.active { background: var(--amber); color: #0f172a; border-color: var(--amber); font-weight: 800; transform: translateY(-1px); }

.admissibility-dashboard { background: var(--surface2); border-radius: 18px; padding: 20px; border: 1px solid var(--bdr); }
.enigma-range {
  width: 100%; height: 6px; appearance: none; cursor: pointer; outline: none;
  border-radius: 6px;
  background: linear-gradient(to right,
    var(--rng-c, var(--amber)) 0%,
    var(--rng-c, var(--amber)) var(--rng-pct, 0%),
    var(--bdr) var(--rng-pct, 0%));
  transition: background 0.3s;
}
.enigma-range::-webkit-slider-thumb {
  appearance: none; width: 22px; height: 22px; border-radius: 50%;
  background: var(--surface); border: 3px solid var(--rng-c, var(--amber));
  box-shadow: 0 2px 10px rgba(0,0,0,0.15); cursor: pointer; transition: transform 0.2s var(--ease-spring);
}
.enigma-range::-webkit-slider-thumb:hover { transform: scale(1.25); }
.score-tier { font-size: 0.6rem; font-weight: 800; opacity: 0.5; }
.tier-low { color: #10b981; } .tier-mid { color: var(--amber); } .tier-high { color: #ef4444; }

.number-stepper {
  display: flex; align-items: center;
  background: var(--surface2); border: 1.5px solid var(--bdr); border-radius: 14px; overflow: hidden;
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

.diff-btn {
  padding: 9px 6px; background: var(--surface2);
  border: 1.5px solid var(--bdr); border-radius: 12px;
  font-size: 11px; font-weight: 700; cursor: pointer;
  color: var(--text2); font-family: inherit;
  display: flex; align-items: center; justify-content: center; gap: 5px; transition: all 0.2s;
}
.diff-btn:hover { transform: translateY(-1px); }

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

.btn-bank-action-v2 {
  background: #0f172a; color: white; border: none;
  padding: 8px 14px; border-radius: 12px; font-weight: 800;
  font-size: 12px; cursor: pointer; font-family: inherit; transition: all 0.2s;
  display: flex; align-items: center;
}
.btn-bank-action-v2:hover { background: var(--amber); color: #0f172a; transform: translateY(-1px); }

.opts-hint {
  display: flex; align-items: center; gap: 8px;
  font-size: 11px; color: var(--text3); font-weight: 600;
  background: var(--surface2); border: 1px dashed var(--bdr2);
  border-radius: 10px; padding: 8px 12px; margin-top: 8px;
}

.code-box { border-radius: 16px; overflow: hidden; border: 1.5px solid var(--bdr); box-shadow: var(--shadow-sm); }
.code-titlebar { background: #0f172a; padding: 10px 16px; display: flex; align-items: center; justify-content: space-between; }
.code-dots { display: flex; gap: 5px; }
.cd-red   { width: 10px; height: 10px; border-radius: 50%; background: #ef4444; cursor: pointer; }
.cd-amber { width: 10px; height: 10px; border-radius: 50%; background: var(--amber); }
.cd-green { width: 10px; height: 10px; border-radius: 50%; background: #22c55e; }
.code-fname { font-size: 10px; color: #4a6090; font-family: monospace; display: flex; align-items: center; gap: 7px; }
.code-area { width: 100%; background: #0d1829; border: none; padding: 16px 18px; color: #7dd3fc; font-family: 'Fira Code', monospace; font-size: 12.5px; resize: vertical; outline: none; line-height: 1.7; }
.code-area::placeholder { color: #2d4a6e; }

/* ════════════════════════════════════════
   CATÉGORIES
════════════════════════════════════════ */
.cats-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.cat-block { background: var(--surface2); border: 1.5px solid var(--bdr); border-radius: 16px; padding: 16px; transition: all 0.25s; }
.cat-block:hover { border-color: var(--amber-bdr); transform: translateY(-2px); box-shadow: var(--shadow-sm); }
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
.sub-add-btn { width: 32px; height: 32px; border-radius: 9px; background: var(--amber); color: #0f172a; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 13px; transition: all 0.2s var(--ease-spring); }
.sub-add-btn:hover { transform: scale(1.12) rotate(10deg); }

/* ════════════════════════════════════════
   AI PROGRESS
════════════════════════════════════════ */
.ai-progress-box { background: var(--amber-light); border: 1.5px solid var(--amber-bdr); border-radius: 16px; padding: 18px; text-align: center; }
.ai-prog-track { height: 5px; background: rgba(245,158,11,0.2); border-radius: 5px; overflow: hidden; }
.ai-prog-fill { height: 100%; background: linear-gradient(90deg, var(--amber), #fbbf24, #f97316); border-radius: 5px; transition: width 0.4s var(--ease-out); min-width: 8px; background-size: 200% 100%; animation: shimmer 1.5s infinite; }
@keyframes shimmer { 0% { background-position: -200% 0; } 100% { background-position: 200% 0; } }
.ai-prog-text { font-size: 11px; font-weight: 800; color: var(--amber-dark); display: flex; align-items: center; justify-content: center; gap: 8px; }

/* ════════════════════════════════════════
   AI PREVIEW
════════════════════════════════════════ */
.ai-preview-box { background: var(--surface2); border: 1.5px solid var(--bdr); border-radius: 16px; overflow: hidden; }
.preview-header { display: flex; align-items: center; gap: 6px; padding: 10px 14px; background: var(--surface); border-bottom: 1px solid var(--bdr); font-size: 10px; font-weight: 800; color: var(--text3); letter-spacing: 1px; flex-wrap: wrap; }
.preview-list { padding: 10px; display: flex; flex-direction: column; gap: 6px; max-height: 220px; overflow-y: auto; }
.preview-item { display: flex; align-items: flex-start; gap: 10px; background: var(--surface); border: 1px solid var(--bdr); border-radius: 10px; padding: 10px; transition: border-color 0.2s; }
.preview-item:hover { border-color: var(--amber-bdr); }
.preview-num { width: 22px; height: 22px; border-radius: 7px; background: var(--amber-light); color: var(--amber-dark); font-size: 10px; font-weight: 800; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.preview-content { flex: 1; min-width: 0; }
.preview-q { font-size: 13px; font-weight: 700; color: var(--text); margin: 0 0 4px; }

/* ════════════════════════════════════════
   MODAL BUTTONS
════════════════════════════════════════ */
.btn-qv-cancel {
  background: var(--surface); color: var(--text2);
  border: 1.5px solid var(--bdr); border-radius: 12px;
  padding: 10px 22px; font-size: 13px; font-weight: 700;
  cursor: pointer; font-family: inherit; transition: all 0.2s;
  display: flex; align-items: center;
}
.btn-qv-cancel:hover { background: var(--surface2); transform: translateY(-1px); }

/* ════════════════════════════════════════
   TOAST
════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 28px; right: 28px;
  background: #0f172a; border-radius: 20px;
  padding: 16px 20px; display: flex; align-items: center; gap: 12px;
  z-index: 9999; box-shadow: 0 20px 40px rgba(0,0,0,0.3), 0 0 0 1px rgba(255,255,255,0.05);
  min-width: 280px; max-width: 420px; overflow: hidden;
  border-left: 4px solid var(--amber);
}
.enigma-toast::before { content: ''; position: absolute; inset: 0; background: linear-gradient(135deg, rgba(245,158,11,0.08), transparent); pointer-events: none; }
.t-success { border-left-color: var(--amber); }
.t-error   { border-left-color: #ef4444; }
.t-info    { border-left-color: #6366f1; }
.t-ico { font-size: 20px; color: white; flex-shrink: 0; position: relative; z-index: 1; }
.t-body { position: relative; z-index: 1; flex: 1; }
.t-body strong { font-size: 9px; font-weight: 900; color: #94a3b8; letter-spacing: 1.2px; display: block; margin-bottom: 2px; }
.t-body p { font-size: 12px; font-weight: 700; color: white; word-break: break-word; }

/* ════════════════════════════════════════
   SCROLLBARS
════════════════════════════════════════ */
.fancy-scroll::-webkit-scrollbar { width: 4px; }
.fancy-scroll::-webkit-scrollbar-track { background: var(--surface2); border-radius: 4px; }
.fancy-scroll::-webkit-scrollbar-thumb { background: var(--amber-bdr); border-radius: 4px; }
.custom-scrollbar::-webkit-scrollbar { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: var(--bdr); border-radius: 4px; }

/* ════════════════════════════════════════
   TRANSITIONS
════════════════════════════════════════ */
.modal-quantum-enter-active { animation: zoomModalIn 0.35s var(--ease-spring); }
.modal-quantum-leave-active { animation: zoomModalIn 0.2s ease-in reverse; }
@keyframes zoomModalIn { from { opacity: 0; transform: scale(0.88) translateY(24px); } to { opacity: 1; transform: none; } }

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

.bulk-bar-anim-enter-active { animation: slideDown 0.35s var(--ease-spring); }
.bulk-bar-anim-leave-active { animation: slideDown 0.2s ease reverse; }

.toast-slide-enter-active { animation: toastIn 0.4s var(--ease-spring); }
.toast-slide-leave-active { animation: toastIn 0.25s ease reverse; }
@keyframes toastIn { from { transform: translateX(60px); opacity: 0; } to { transform: none; opacity: 1; } }

/* ════════════════════════════════════════
   UTILITIES
════════════════════════════════════════ */
.text-amber { color: var(--amber) !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
.text-warning { color: #f59e0b !important; }

/* ════════════════════════════════════════
   RESPONSIVE
════════════════════════════════════════ */
@media (max-width: 1024px) { .bq-workspace { padding: 20px !important; } .premium-title { font-size: 1.8rem; } }
@media (max-width: 768px) {
  .bq-header { flex-direction: column !important; gap: 16px !important; align-items: flex-start !important; }
  .questions-grid { grid-template-columns: 1fr; }
  .lang-cards-grid { grid-template-columns: 1fr; }
  .type-tiles-grid { grid-template-columns: repeat(3, 1fr); }
  .cats-grid { grid-template-columns: 1fr; }
  .premium-title { font-size: 1.6rem; }
  .search-inline-input { width: 130px; }
  .bulk-action-bar { flex-direction: column; align-items: flex-start; }
  .bulk-actions { margin-left: 0; }
}
@media (max-width: 480px) {
  .modal-footer-actions { flex-direction: column; }
  .modal-footer-actions button { width: 100%; justify-content: center; }
}

/* ════════════════════════════════════════
   PAGINATION
════════════════════════════════════════ */
.bq-pagination-wrap {
  background: var(--surface);
  border: 1px solid var(--bdr);
  border-radius: 20px;
  padding: 16px 24px;
  box-shadow: var(--shadow-sm);
  display: flex;
  justify-content: space-between;
  align-items: center;
  transition: all 0.3s ease;
  z-index: 10;
}
.btn-pagination {
  width: 38px;
  height: 38px;
  border-radius: 12px;
  border: 1px solid var(--bdr);
  background: var(--surface2);
  color: var(--text2);
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  transition: all 0.2s var(--ease-spring);
}
.btn-pagination:hover:not(:disabled) {
  border-color: var(--amber);
  color: var(--amber-dark);
  background: var(--amber-light);
  transform: translateY(-2px);
}
.btn-pagination:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
.btn-pagination-num {
  height: 38px;
  min-width: 38px;
  padding: 0 8px;
  border-radius: 12px;
  border: 1px solid var(--bdr);
  background: var(--surface);
  color: var(--text);
  font-weight: 700;
  font-size: 13px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s var(--ease-spring);
}
.btn-pagination-num:hover {
  border-color: var(--amber);
  color: var(--amber-dark);
  background: var(--amber-light);
  transform: translateY(-2px);
}
.btn-pagination-num.active {
  background: var(--amber);
  color: #0f172a;
  border-color: var(--amber);
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
}
.pagination-size-select {
  width: 90px;
}
</style>