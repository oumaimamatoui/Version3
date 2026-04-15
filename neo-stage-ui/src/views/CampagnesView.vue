<template>
  <div class="enigma-master-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar" @scroll="handleScroll">

        <!-- ═══════════════════════════════════════════════════════
             SECTION A : DASHBOARD
        ═══════════════════════════════════════════════════════ -->
        <div v-if="!isStudioMode && activeView === 'dashboard'" class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Terminal de Campagnes</span>
              </div>
              <h2 class="premium-title">Architecture &amp; <span class="gradient-text">Déploiement</span></h2>
            </div>
            <div class="d-flex gap-3 flex-wrap align-items-center">
              <button class="btn-refresh-pro" @click="fetchInitialData" :disabled="loading" title="Rafraîchir">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': loading }"></i>
              </button>
              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: viewMode === 'grid' }]"      @click="viewMode = 'grid'"      title="Vue grille"><i class="fa-solid fa-table-cells-large"></i></button>
                <button :class="['btn-view-toggle', { active: viewMode === 'list' }]"      @click="viewMode = 'list'"      title="Vue liste"><i class="fa-solid fa-list-ul"></i></button>
                <button :class="['btn-view-toggle', { active: viewMode === 'analytics' }]" @click="viewMode = 'analytics'" title="Analytique"><i class="fa-solid fa-chart-simple"></i></button>
              </div>
              <button class="btn-enigma-primary shadow-premium" @click="enterStudioMode()">
                <div class="btn-content"><i class="fa-solid fa-plus me-2"></i> CRÉER UNE ARCHITECTURE</div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- KPI -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }"><i :class="stat.icon"></i></div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
                <div v-if="stat.trend" class="stat-trend ms-auto" :class="stat.trend > 0 ? 'trend-up' : 'trend-down'">
                  <i :class="stat.trend > 0 ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ Math.abs(stat.trend) }}%</span>
                </div>
              </div>
            </div>
          </div>

          <!-- ANALYTICS VIEW -->
          <div v-if="viewMode === 'analytics'" class="analytics-overview-panel mb-5 animate__animated animate__fadeIn">
            <div class="row g-4">
              <div class="col-lg-8">
                <div class="analytics-card-pro p-4">
                  <div class="d-flex justify-content-between align-items-center mb-4">
                    <h6 class="fw-800 m-0">Activité des Campagnes (7j)</h6>
                    <div class="d-flex gap-2 align-items-center">
                      <span class="legend-dot dot-amber"></span><span class="small text-muted">Déploiements</span>
                      <span class="legend-dot dot-indigo ms-3"></span><span class="small text-muted">Candidats</span>
                    </div>
                  </div>
                  <div class="bar-chart-v2">
                    <!-- FIX #1: activityData is a ref (generated once), not a computed with Math.random() -->
                    <div v-for="(bar, i) in activityData" :key="i" class="bar-col">
                      <div class="bar-wrap">
                        <div class="bar-fill bar-amber" :style="{ height: bar.deploy + '%' }"></div>
                        <div class="bar-fill bar-indigo" :style="{ height: bar.cand + '%' }"></div>
                      </div>
                      <span class="bar-label">{{ bar.label }}</span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-lg-4">
                <div class="analytics-card-pro p-4 h-100">
                  <h6 class="fw-800 mb-4">Distribution par Statut</h6>
                  <div class="donut-chart-container">
                    <svg viewBox="0 0 120 120" width="120">
                      <circle v-for="(seg, i) in donutSegments" :key="i"
                        cx="60" cy="60" r="45"
                        :stroke="seg.color" stroke-width="20" fill="none"
                        :stroke-dasharray="`${seg.dash} ${283 - seg.dash}`"
                        :stroke-dashoffset="seg.offset"
                        style="transition: stroke-dasharray 0.6s ease"/>
                      <text x="60" y="64" text-anchor="middle" class="donut-center-text">{{ campaigns.length }}</text>
                      <text x="60" y="75" text-anchor="middle" class="donut-sub-text">Total</text>
                    </svg>
                    <div class="donut-legend">
                      <div v-for="seg in donutSegments" :key="seg.label" class="donut-legend-item">
                        <span class="legend-dot-sm" :style="{ background: seg.color }"></span>
                        <span class="small">{{ seg.label }}</span>
                        <span class="ms-auto fw-800 small">{{ seg.count }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- FILTERS -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="tabs-container">
              <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border">
                <button v-for="tab in filterTabs" :key="tab.label"
                  class="nav-tab-btn-modern" :class="{ active: activeTab === tab.value }"
                  @click="activeTab = tab.value">
                  {{ tab.label }} <span class="tab-count">{{ tab.count }}</span>
                </button>
              </div>
            </div>
            <div class="d-flex gap-2">
              <div class="search-inline-box">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" v-model="dashboardSearch" placeholder="Rechercher..." class="search-inline-input">
                <button v-if="dashboardSearch" @click="dashboardSearch = ''" class="btn-clear-search"><i class="fa-solid fa-xmark"></i></button>
              </div>
              <select v-model="sortBy" class="sort-select-pro">
                <option value="date">Date</option>
                <option value="name">Nom</option>
                <option value="candidates">Candidats</option>
              </select>
            </div>
          </div>

          <!-- GRID VIEW -->
          <div v-if="viewMode !== 'list'" class="row g-4">
            <div v-if="loading" class="col-12 text-center py-5"><div class="spinner-pro-premium"></div></div>
            <div v-else-if="filteredCampaigns.length === 0" class="col-12">
              <div class="empty-state-pro py-5 text-center">
                <i class="fa-solid fa-layer-group fa-3x text-muted mb-3"></i>
                <h5 class="fw-800">Aucune campagne trouvée</h5>
                <p class="text-muted">Créez votre première architecture ou modifiez les filtres.</p>
                <button class="btn-enigma-primary mt-3" @click="enterStudioMode()">
                  <div class="btn-content"><i class="fa-solid fa-plus me-2"></i> Nouvelle Architecture</div>
                  <div class="btn-glow"></div>
                </button>
              </div>
            </div>
            <div v-else v-for="c in filteredCampaigns" :key="c.id" class="col-xl-4 col-md-6 animate__animated animate__fadeInUp">
              <div class="campaign-card-modern" @click="previewCampaign(c)">
                <div class="card-header-modern mb-3 d-flex justify-content-between align-items-start">
                  <span class="status-badge" :class="'status-' + c.statut">
                    <span class="status-dot"></span> {{ getStatusLabel(c.statut) }}
                  </span>
                  <div class="d-flex gap-2 align-items-center">
                    <button class="btn-icon-sm" @click.stop="duplicateCampaign(c)" title="Dupliquer"><i class="fa-regular fa-copy"></i></button>
                    <div class="dropdown">
                      <button class="btn-options-round" data-bs-toggle="dropdown" @click.stop><i class="fa-solid fa-ellipsis-vertical"></i></button>
                      <ul class="dropdown-menu border-0 shadow-premium p-2 rounded-4">
                        <li><button class="dropdown-item rounded-3" @click.stop="enterStudioMode(c)"><i class="fa-solid fa-pen-to-square me-2"></i>Editer</button></li>
                        <li><button class="dropdown-item rounded-3" @click.stop="exportCampaign(c)"><i class="fa-solid fa-file-export me-2"></i>Exporter JSON</button></li>
                        <li><button class="dropdown-item rounded-3" @click.stop="togglePinCampaign(c.id)"><i :class="['me-2', pinnedCampaigns.includes(c.id) ? 'fa-solid fa-thumbtack text-amber' : 'fa-regular fa-thumbtack']"></i>{{ pinnedCampaigns.includes(c.id) ? 'Désépingler' : 'Épingler' }}</button></li>
                        <li><hr class="dropdown-divider"></li>
                        <li><button class="dropdown-item rounded-3 text-danger" @click.stop="handleDelete(c.id)"><i class="fa-solid fa-trash-can me-2"></i>Supprimer</button></li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div v-if="pinnedCampaigns.includes(c.id)" class="pin-badge"><i class="fa-solid fa-thumbtack"></i> Épinglé</div>
                <h5 class="campaign-title-modern fw-800">{{ c.nom }}</h5>
                <div class="test-attachment-box mt-3 mb-3 p-3 bg-light rounded-4 d-flex align-items-center gap-3">
                  <div class="icon-file text-amber"><i class="fa-solid fa-file-code fa-lg"></i></div>
                  <div class="flex-grow-1 overflow-hidden">
                    <span class="text-overline d-block" style="font-size:0.6rem;font-weight:900;opacity:0.5;">STRUCTURE LIÉE</span>
                    <p class="m-0 text-truncate fw-bold small">{{ getQuestionnaireName(c.questionnaireId) }}</p>
                  </div>
                </div>
                <div class="progress-slim mb-3">
                  <div class="progress-fill" :style="{ width: getCampaignProgress(c) + '%', background: getProgressColor(getCampaignProgress(c)) }"></div>
                </div>
                <div class="card-footer-modern d-flex justify-content-between pt-3 border-top border-light">
                  <div class="meta-item small text-muted"><i class="fa-regular fa-calendar me-2"></i>{{ formatDate(c.dateDebut) }}</div>
                  <div class="meta-item small text-muted"><i class="fa-solid fa-user-group me-2"></i>{{ c.maxCandidats }} slots</div>
                </div>
              </div>
            </div>
          </div>

          <!-- LIST VIEW -->
          <div v-if="viewMode === 'list'" class="list-view-pro animate__animated animate__fadeIn">
            <div class="list-header-row d-flex align-items-center px-4 py-2 mb-2">
              <span style="width:200px" class="list-col-label">STATUT / NOM</span>
              <span class="flex-grow-1 list-col-label">STRUCTURE</span>
              <span style="width:120px" class="list-col-label">DATE</span>
              <span style="width:100px" class="list-col-label text-center">SLOTS</span>
              <span style="width:80px"  class="list-col-label text-center">ACTIONS</span>
            </div>
            <div v-if="loading" class="text-center py-4"><div class="spinner-pro-premium"></div></div>
            <div v-else-if="filteredCampaigns.length === 0" class="text-center py-4 text-muted">Aucune campagne trouvée.</div>
            <div v-else v-for="c in filteredCampaigns" :key="c.id" class="list-row-item d-flex align-items-center px-4 py-3 mb-2">
              <div style="width:200px">
                <span class="status-badge" :class="'status-' + c.statut"><span class="status-dot"></span> {{ getStatusLabel(c.statut) }}</span>
              </div>
              <div class="flex-grow-1">
                <div class="fw-800 small">{{ c.nom }}</div>
                <div class="text-muted" style="font-size:0.7rem">{{ getQuestionnaireName(c.questionnaireId) }}</div>
              </div>
              <div style="width:120px" class="small text-muted">{{ formatDate(c.dateDebut) }}</div>
              <div style="width:100px" class="text-center"><span class="slot-badge">{{ c.maxCandidats }}</span></div>
              <div style="width:80px" class="d-flex gap-2 justify-content-center">
                <button class="btn-icon-sm" @click="enterStudioMode(c)" title="Editer"><i class="fa-solid fa-pen-to-square"></i></button>
                <button class="btn-icon-sm danger" @click="handleDelete(c.id)" title="Supprimer"><i class="fa-solid fa-trash-can"></i></button>
              </div>
            </div>
          </div>
        </div>

        <!-- ═══════════════════════════════════════════════════════
             SECTION PREVIEW
        ═══════════════════════════════════════════════════════ -->
        <div v-if="activeView === 'preview' && previewData && !isStudioMode" class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">
          <header class="d-flex align-items-center gap-4 mb-5 flex-wrap">
            <button class="btn-back-to-dash" @click="activeView = 'dashboard'"><i class="fa-solid fa-arrow-left"></i></button>
            <div>
              <div class="breadcrumb-pro mb-1">
                <span class="root" @click="activeView = 'dashboard'" style="cursor:pointer">Campagnes</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Aperçu</span>
              </div>
              <h2 class="premium-title m-0">{{ previewData.nom }}</h2>
            </div>
            <div class="ms-auto d-flex gap-3">
              <button class="btn-outline-pro" @click="exportCampaign(previewData)"><i class="fa-solid fa-file-export me-2"></i>Export JSON</button>
              <button class="btn-enigma-primary" @click="enterStudioMode(previewData)">
                <div class="btn-content"><i class="fa-solid fa-pen-to-square me-2"></i>Éditer</div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>
          <div class="row g-4">
            <div class="col-lg-8">
              <div class="enigma-card p-5 mb-4">
                <h6 class="fw-900 mb-4 text-muted" style="font-size:0.65rem;letter-spacing:2px;">INFORMATIONS GÉNÉRALES</h6>
                <div class="row g-4">
                  <div class="col-md-6">
                    <div class="preview-field-box">
                      <span class="preview-field-label">STATUT</span>
                      <span class="status-badge mt-1" :class="'status-' + previewData.statut"><span class="status-dot"></span> {{ getStatusLabel(previewData.statut) }}</span>
                    </div>
                  </div>
                  <div class="col-md-6">
                    <div class="preview-field-box"><span class="preview-field-label">STRUCTURE LIÉE</span><span class="fw-800">{{ getQuestionnaireName(previewData.questionnaireId) }}</span></div>
                  </div>
                  <div class="col-md-6">
                    <div class="preview-field-box"><span class="preview-field-label">DATE D'OUVERTURE</span><span class="fw-800">{{ formatDate(previewData.dateDebut) }}</span></div>
                  </div>
                  <div class="col-md-6">
                    <div class="preview-field-box"><span class="preview-field-label">MAX CANDIDATS</span><span class="fw-800">{{ previewData.maxCandidats }}</span></div>
                  </div>
                </div>
              </div>
              <div class="enigma-card p-5">
                <h6 class="fw-900 mb-4 text-muted" style="font-size:0.65rem;letter-spacing:2px;">TIMELINE D'ACTIVITÉ</h6>
                <div class="activity-timeline">
                  <div class="timeline-item" v-for="event in previewTimeline" :key="event.id">
                    <div class="tl-dot" :style="{ background: event.color }"></div>
                    <div class="tl-content"><strong class="small">{{ event.title }}</strong><p class="text-muted m-0" style="font-size:0.75rem">{{ event.desc }}</p></div>
                    <span class="tl-time">{{ event.time }}</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="col-lg-4">
              <div class="enigma-card p-4 mb-4">
                <h6 class="fw-900 mb-3" style="font-size:0.65rem;letter-spacing:2px;color:#94a3b8;">TAUX DE COMPLÉTION</h6>
                <div class="big-progress-container">
                  <svg viewBox="0 0 120 120" width="120">
                    <circle cx="60" cy="60" r="50" fill="none" stroke="#eef2f6" stroke-width="10"/>
                    <circle cx="60" cy="60" r="50" fill="none" stroke="#f59e0b" stroke-width="10"
                      :stroke-dasharray="`${getCampaignProgress(previewData) * 3.14} 314`"
                      stroke-dashoffset="78.5" stroke-linecap="round"
                      style="transition:stroke-dasharray 0.8s ease"/>
                    <text x="60" y="65" text-anchor="middle" font-size="18" font-weight="900" fill="#0f172a">{{ getCampaignProgress(previewData) }}%</text>
                  </svg>
                </div>
              </div>
              <div class="enigma-card p-4">
                <h6 class="fw-900 mb-4" style="font-size:0.65rem;letter-spacing:2px;color:#94a3b8;">ACTIONS RAPIDES</h6>
                <div class="d-flex flex-column gap-2">
                  <button class="btn-quick-action" @click="changeStatus(previewData, 1)"><i class="fa-solid fa-play me-2 text-amber"></i>Activer la campagne</button>
                  <button class="btn-quick-action" @click="changeStatus(previewData, 2)"><i class="fa-solid fa-stop me-2 text-danger"></i>Terminer la campagne</button>
                  <button class="btn-quick-action" @click="duplicateCampaign(previewData)"><i class="fa-regular fa-copy me-2 text-indigo"></i>Dupliquer l'architecture</button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- ═══════════════════════════════════════════════════════
             SECTION B : STUDIO
        ═══════════════════════════════════════════════════════ -->
        <div v-if="isStudioMode" class="studio-view animate__animated animate__zoomIn animate__faster">
          <header class="studio-header-v2" :class="{ 'is-docked': isScrolled }">
            <div class="header-content-inner p-4 p-xl-5 pb-4">
              <div class="d-flex justify-content-between align-items-center mb-5 flex-wrap gap-3">
                <div class="d-flex align-items-center gap-4">
                  <button class="btn-back-to-dash" @click="exitStudio"><i class="fa-solid fa-arrow-left"></i></button>
                  <div class="ai-robot-terminal">
                    <svg viewBox="0 0 60 60" fill="none" width="40">
                      <rect x="12" y="10" width="36" height="34" rx="11" fill="white" opacity=".96"/>
                      <rect x="16" y="18" width="28" height="12" rx="6" fill="#0f172a"/>
                      <circle cx="22" cy="24" r="3.5" fill="#f59e0b"><animate attributeName="opacity" values="1;0.15;1" dur="3s" repeatCount="indefinite"/></circle>
                      <circle cx="38" cy="24" r="3.5" fill="#f59e0b"><animate attributeName="opacity" values="1;0.15;1" dur="3s" begin="0.4s" repeatCount="indefinite"/></circle>
                    </svg>
                  </div>
                  <div>
                    <h1 class="main-title-v2">Studio<span>Architect</span> <span class="v-badge">v6.5</span></h1>
                    <p class="brand-subtitle-v2">CONFIGURATION DE L'INSTANCE UML &amp; SQL</p>
                  </div>
                </div>
                <div class="global-actions-cluster d-flex gap-3 align-items-center flex-wrap">
                  <!-- FIX #2: autoSaveTimer is a ref, not window._autoSaveTimer -->
                  <div class="autosave-indicator" :class="{ 'saving': isSaving, 'saved': lastSaved }">
                    <i :class="isSaving ? 'fa-solid fa-spinner fa-spin' : 'fa-solid fa-cloud-arrow-up'"></i>
                    <span>{{ isSaving ? 'Sauvegarde...' : lastSaved ? 'Sauvegardé ' + lastSavedTime : 'Non sauvegardé' }}</span>
                  </div>
                  <div class="health-core-widget">
                    <div class="health-ring-box">
                      <svg width="48" height="48">
                        <circle class="ring-track" cx="24" cy="24" r="19"></circle>
                        <circle class="ring-fill" cx="24" cy="24" r="19" :style="healthRingStyle"></circle>
                      </svg>
                      <span class="health-percent">{{ architectureHealth }}%</span>
                    </div>
                    <div class="health-label-box">
                      <span class="h-main">VIABILITÉ</span>
                      <span class="h-sub" :style="{ color: healthColor }">{{ healthStatusText }}</span>
                    </div>
                  </div>
                  <button class="btn-shortcuts" @click="modals.shortcuts = true" title="Raccourcis"><i class="fa-solid fa-keyboard"></i></button>
                  <button @click="publishToProduction" :disabled="!isReadyToPublish || isPublishing" class="btn-enigma-primary">
                    <div class="btn-content">
                      <span v-if="isPublishing" class="spinner-border spinner-border-sm me-2"></span>
                      <i v-else class="fa-solid fa-rocket me-2"></i> DÉPLOYER
                    </div>
                    <div class="btn-glow"></div>
                  </button>
                </div>
              </div>

              <nav class="stepper-precision-v8">
                <div class="stepper-line-bg"><div class="stepper-line-fill" :style="{ width: stepperProgress + '%' }"></div></div>
                <div class="stepper-nodes-row">
                  <div v-for="step in steps" :key="step.id"
                    :class="['step-node-v8', { 'active': currentStep === step.id, 'completed': currentStep > step.id }]"
                    @click="jumpToStep(step.id)">
                    <div class="node-point">
                      <div class="point-inner">{{ currentStep > step.id ? '✓' : step.id }}</div>
                      <div class="node-label-floating">{{ step.label }}</div>
                    </div>
                  </div>
                </div>
              </nav>
            </div>
          </header>

          <div class="studio-workspace-inner p-4 p-xl-5 pt-0">
            <div class="row g-5">
              <div class="col-xl-8">

                <!-- ÉTAPE 1 -->
                <section v-if="currentStep === 1" class="engine-pane animate__animated animate__fadeIn">
                  <div class="enigma-card p-5">
                    <div class="pane-header-v2 mb-5">
                      <div class="icon-box-v2 amber"><i class="fa-solid fa-layer-group"></i></div>
                      <div><h4 class="fw-900 m-0">Architecture Noyau</h4><p class="text-muted m-0">Métadonnées de l'instance SQL.</p></div>
                    </div>
                    <div class="row g-5">
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>NOM DE L'INSTANCE *</label>
                          <input type="text" v-model="studio.questionnaire.titre" class="enigma-field" :class="{ 'field-error': showValidation && !studio.questionnaire.titre }" placeholder="Ex: Frontend Senior Audit 2025">
                          <span v-if="showValidation && !studio.questionnaire.titre" class="field-error-msg">Ce champ est requis</span>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>EXPERTISE</label>
                          <select v-model="studio.questionnaire.categorie" class="enigma-field">
                            <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                          </select>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>DURÉE (MIN)</label>
                          <input type="number" v-model.number="studio.questionnaire.duree" class="enigma-field" min="5" max="360">
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>DESCRIPTION (OPTIONNEL)</label>
                          <textarea v-model="studio.questionnaire.description" class="enigma-field" rows="3" placeholder="Décrivez l'objectif de cette session..."></textarea>
                        </div>
                      </div>
                      <div class="col-12 mt-4">
                        <div class="admissibility-dashboard">
                          <div class="d-flex justify-content-between">
                            <h6>SEUIL D'ADMISSIBILITÉ</h6>
                            <span class="fw-900">{{ studio.questionnaire.scoreReussite }}%</span>
                          </div>
                          <input type="range" v-model="studio.questionnaire.scoreReussite" min="1" max="100" class="enigma-range mt-3">
                          <div class="d-flex justify-content-between mt-2">
                            <span class="score-tier tier-low">Junior</span>
                            <span class="score-tier tier-mid">Standard</span>
                            <span class="score-tier tier-high">Expert</span>
                          </div>
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>TAGS</label>
                          <div class="tags-input-container enigma-field" style="height:auto;min-height:50px;display:flex;flex-wrap:wrap;gap:6px;align-items:center;">
                            <span v-for="(tag, i) in studio.questionnaire.tags" :key="i" class="tag-chip">
                              {{ tag }} <button @click="removeTag(i)" class="tag-remove"><i class="fa-solid fa-xmark"></i></button>
                            </span>
                            <input type="text" v-model="tagInput" @keydown.enter.prevent="addTag" @keydown.188.prevent="addTag" placeholder="Ajouter un tag..." class="tag-field-input">
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                </section>

                <!-- ÉTAPE 2 -->
                <section v-if="currentStep === 2" class="engine-pane animate__animated animate__fadeIn">
                  <div class="orchestrator-head-v2 mb-4 d-flex justify-content-between align-items-end">
                    <h5 class="fw-900 m-0">Composition ({{ studio.questions.length }})</h5>
                    <div class="d-flex gap-2">
    
                      <button @click="openDeepBankModal" class="btn-bank-action-v2"><i class="fa-solid fa-vault me-2"></i> BANQUE</button>
                    </div>
                  </div>
                  <div v-if="studio.questions.length > 0" class="bulk-actions-bar mb-3">
                    <label class="d-flex align-items-center gap-2 small fw-700" style="cursor:pointer">
                      <input type="checkbox" @change="toggleSelectAll" :checked="selectedQuestions.length === studio.questions.length && studio.questions.length > 0">
                      Tout sélectionner
                    </label>
                    <div v-if="selectedQuestions.length > 0" class="d-flex gap-2 ms-auto">
                      <span class="small text-muted">{{ selectedQuestions.length }} sélectionné(s)</span>
                      <button @click="removeSelectedQuestions" class="btn-icon-sm danger"><i class="fa-solid fa-trash-can"></i></button>
                    </div>
                    <div class="ms-auto" v-else>
                      <span class="small text-muted">Total: <strong>{{ totalLogicPoints }} pts</strong></span>
                    </div>
                  </div>
                  <div class="assets-scroll-v8 custom-scrollbar">
                    <draggable v-model="studio.questions" item-key="id" handle=".drag-node-handle" ghost-class="asset-ghost-v8">
                      <template #item="{ element, index }">
                        <div class="asset-card-v8" :class="{ 'selected': selectedQuestions.includes(element.id) }">
                          <input type="checkbox" class="asset-checkbox" :checked="selectedQuestions.includes(element.id)" @change="toggleSelectQuestion(element.id)">
                          <div class="drag-node-handle"><i class="fa-solid fa-grip-vertical"></i></div>
                          <div class="asset-index-v8">{{ String(index + 1).padStart(2, '0') }}</div>
                          <div class="asset-core-v8 flex-grow-1">
                            <h6 class="asset-title-v8 text-truncate">{{ element.texte }}</h6>
                            <div class="d-flex gap-2 mt-1">
                              <span class="t-pill weight">{{ element.poids }} PTS</span>
                              <span v-if="element.duree" class="t-pill time"><i class="fa-solid fa-clock me-1"></i> {{ element.duree }}s</span>
                              <span v-if="element.difficulty" class="t-pill" :class="'diff-' + element.difficulty?.toLowerCase()">{{ element.difficulty }}</span>
                            </div>
                          </div>
                          <button @click="editQuestion(element, index)" class="btn-icon-sm me-1"><i class="fa-solid fa-pen"></i></button>
                          <button @click="studio.questions.splice(index, 1)" class="btn-remove-v8"><i class="fa-solid fa-xmark"></i></button>
                        </div>
                      </template>
                    </draggable>
                    <div v-if="studio.questions.length === 0" class="empty-questions-hint text-center py-5">
                      <i class="fa-solid fa-vault fa-2x text-muted mb-3"></i>
                      <p class="text-muted small">Commencez par créer ou importer des questions depuis la banque d'actifs.</p>
                    </div>
                  </div>
                </section>

                <!-- ÉTAPE 3 -->
                <section v-if="currentStep === 3" class="engine-pane animate__animated animate__fadeIn">
                  <div class="talent-hub-search-v2 mb-4">
                    <div class="search-enigma-box"><i class="fa-solid fa-magnifying-glass"></i><input type="text" v-model="searchCandQuery" placeholder="Cibler un candidat..." class="enigma-search-field"></div>
                    <div class="batch-counter-v2"><span class="val">{{ selectedCandidatesIds.length }}</span><span class="lab">CANDIDATS</span></div>
                    <button v-if="selectedCandidatesIds.length < filteredCandidatePool.length" @click="selectAllCandidates" class="btn-outline-pro ms-2">Tout sélectionner</button>
                    <button v-else @click="selectedCandidatesIds = []" class="btn-outline-pro ms-2">Désélectionner</button>
                  </div>
                  <div class="d-flex gap-2 mb-4 flex-wrap">
                    <button :class="['filter-chip', { active: candidateFilter === '' }]"       @click="candidateFilter = ''">Tous</button>
                    <button :class="['filter-chip', { active: candidateFilter === 'new' }]"    @click="candidateFilter = 'new'">Nouveaux</button>
                    <button :class="['filter-chip', { active: candidateFilter === 'senior' }]" @click="candidateFilter = 'senior'">Seniors</button>
                  </div>
                  <div class="talent-matrix-grid custom-scrollbar">
                    <div class="row g-4">
                      <div v-for="c in filteredCandidatePool" :key="c.id" class="col-md-6 col-lg-4">
                        <div :class="['talent-card-v8', { 'active': isSelectedCandidate(c.id) }]" @click="toggleCandidateTarget(c)">
                          <div class="card-check-v8"><i class="fa-solid fa-check"></i></div>
                          <div class="avatar-v8">{{ c.name ? c.name[0] : '?' }}</div>
                          <div class="data-v8"><b>{{ c.name }}</b><p class="small text-muted m-0">{{ c.email }}</p></div>
                        </div>
                      </div>
                    </div>
                  </div>
                </section>

                <!-- ÉTAPE 4 -->
                <section v-if="currentStep === 4" class="engine-pane animate__animated animate__fadeIn">
                  <div class="enigma-card p-5">
                    <h5 class="fw-900 mb-5"><i class="fa-solid fa-calendar-check text-amber me-3"></i>Matrix Planning</h5>
                    <div class="row g-5">
                      <div class="col-md-4"><div class="enigma-input-wrap"><label>OUVERTURE TERMINAL</label><input type="datetime-local" v-model="studio.campagne.dateDebut" class="enigma-field"></div></div>
                      <div class="col-md-4"><div class="enigma-input-wrap"><label>DURÉE (MIN)</label><input type="number" v-model.number="studio.questionnaire.duree" class="enigma-field" min="1"></div></div>
                      <div class="col-md-4"><div class="enigma-input-wrap"><label>FERMETURE ACCÈS</label><input type="datetime-local" v-model="studio.campagne.dateFin" class="enigma-field"></div></div>
                      <div class="col-md-6"><div class="enigma-input-wrap"><label>MAX CANDIDATS</label><input type="number" v-model.number="studio.campagne.maxCandidats" class="enigma-field" min="1"></div></div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>FUSEAU HORAIRE</label>
                          <select v-model="studio.campagne.timezone" class="enigma-field">
                            <option value="Europe/Paris">Europe/Paris (UTC+1)</option>
                            <option value="UTC">UTC</option>
                            <option value="America/New_York">America/New_York (UTC-5)</option>
                            <option value="Asia/Dubai">Asia/Dubai (UTC+4)</option>
                          </select>
                        </div>
                      </div>
                      <div class="col-12 mt-5">
                        <div class="protocol-row border-top pt-4">
                          <div class="p-icon"><i class="fa-solid fa-shield-halved"></i></div>
                          <div class="p-data"><h6>Surveillance Anti-Cheat v2.0</h6><p>Analyse de fraude asynchrone.</p></div>
                          <div class="form-check form-switch enigma-switch"><input class="form-check-input" type="checkbox" v-model="studio.campagne.anticheat"></div>
                        </div>
                        <div class="protocol-row border-top pt-4 mt-4">
                          <div class="p-icon text-indigo"><i class="fa-solid fa-timer"></i></div>
                          <div class="p-data"><h6>Minuteur par Question</h6><p>Limite de temps individuelle par actif.</p></div>
                          <div class="form-check form-switch enigma-switch"><input class="form-check-input" type="checkbox" v-model="studio.campagne.timerPerQuestion"></div>
                        </div>
                        <div class="protocol-row border-top pt-4 mt-4">
                          <div class="p-icon text-success"><i class="fa-solid fa-paper-plane"></i></div>
                          <div class="p-data"><h6>Notifications Email Automatiques</h6><p>Envoi automatique aux candidats sélectionnés.</p></div>
                          <div class="form-check form-switch enigma-switch"><input class="form-check-input" type="checkbox" v-model="studio.campagne.sendNotifications"></div>
                        </div>
                      </div>
                    </div>
                  </div>
                </section>
              </div>

              <!-- HUD SIDEBAR -->
              <div class="col-xl-4">
                <aside class="sidebar-analytics-engine">
                  <div class="analytics-hub-glass">
                    <div class="hub-header-v2">
                      <span class="hub-label">DASHBOARD ARCHITECTE</span>
                      <h4 class="hub-title-v2">{{ studio.questionnaire.titre || 'UML SANS TITRE' }}</h4>
                      <div class="hub-status-box" :class="validationState.class"><span class="pulse-dot"></span> {{ validationState.text }}</div>
                    </div>
                    <div class="kpi-bento-grid mt-4">
                      <div class="bento-item"><span class="v">{{ studio.questions.length }}</span><span class="l">ACTIFS</span></div>
                      <div class="bento-item highlight"><span class="v">{{ totalLogicPoints }}</span><span class="l">POINTS</span></div>
                      <div class="bento-item"><span class="v">{{ studio.questionnaire.duree }}'</span><span class="l">DURÉE</span></div>
                      <div class="bento-item"><span class="v">{{ selectedCandidatesIds.length }}</span><span class="l">CIBLÉS</span></div>
                    </div>
                    <div class="readiness-checklist mt-4">
                      <div class="checklist-label mb-2">CHECKLIST DE DÉPLOIEMENT</div>
                      <div v-for="check in deployChecklist" :key="check.label" class="check-item" :class="{ 'passed': check.passed }">
                        <i :class="check.passed ? 'fa-solid fa-check-circle text-success' : 'fa-regular fa-circle text-muted'"></i>
                        <span>{{ check.label }}</span>
                      </div>
                    </div>
                    <div class="wizard-controls-v8 mt-5 pt-4">
                      <div class="d-flex gap-2">
                        <button @click="prevStep" :disabled="currentStep === 1" class="btn-step-nav back"><i class="fa-solid fa-arrow-left"></i></button>
                        <button v-if="currentStep < 4" @click="nextStep" class="btn-step-nav next flex-grow-1">ÉTAPE SUIVANTE</button>
                        <button v-else @click="publishToProduction" :disabled="!isReadyToPublish" class="btn-step-deploy flex-grow-1">DÉPLOYER</button>
                      </div>
                    </div>
                  </div>

                  <div class="ia-coach-terminal mt-4">
                    <div class="robot-glow-container"><i class="fa-solid fa-robot text-white"></i></div>
                    <div class="coach-text-v8"><h6>Coach EvaluaArchitect</h6><p class="m-0 small">{{ coachAIAdvice }}</p></div>
                  </div>

                  <div class="activity-feed-widget mt-4">
                    <div class="feed-header">ACTIVITÉ RÉCENTE</div>
                    <div v-for="act in studioActivityFeed" :key="act.id" class="feed-item">
                      <div class="feed-dot" :style="{ background: act.color }"></div>
                      <span class="feed-text small">{{ act.text }}</span>
                      <span class="feed-time">{{ act.time }}</span>
                    </div>
                  </div>
                </aside>
              </div>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- ═══════════════════════════════════════════════════════
         MODALE : BANQUE D'ACTIFS
    ═══════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="modals.bank" class="quantum-vault-overlay" @click.self="modals.bank = false">
        <div class="quantum-vault-window">
          <header class="qv-header">
            <div class="qv-title">
              <h2 class="m-0 fw-900">Bibliothèque d'Actifs Certifiés</h2>
              <p class="m-0 text-muted small">Panier temporaire : {{ selectedItemsInBank.length }} actifs.</p>
            </div>
            <div class="qv-actions d-flex gap-3">
              <button @click="modals.bank = false" class="btn-qv-cancel">ANNULER</button>
              <button @click="confirmStudioSync" :disabled="selectedItemsInBank.length === 0" class="btn-qv-confirm">IMPORTER ({{ selectedItemsInBank.length }})</button>
            </div>
          </header>
          <div class="qv-filter-tabs px-4 py-3 border-bottom d-flex gap-2 align-items-center flex-wrap">
            <button v-for="diff in ['Tous', 'EXPERT', 'MEDIUM', 'EASY']" :key="diff"
              :class="['qv-filter-btn', { active: bankDiffFilter === diff }]"
              @click="bankDiffFilter = diff">{{ diff }}</button>
            <div class="ms-auto small text-muted">{{ filteredBankReference.length }} actifs trouvés</div>
          </div>
          <div class="qv-layout">
            <aside class="qv-sidebar">
              <div class="qv-search-area mb-4">
                <label class="small fw-800 text-muted d-block mb-2">RECHERCHE</label>
                <input type="text" v-model="searchBankQuery" class="enigma-field" placeholder="SQL, Kafka...">
              </div>
              <div class="mb-4">
                <label class="small fw-800 text-muted d-block mb-2">CATÉGORIE</label>
                <div class="d-flex flex-column gap-1">
                  <button v-for="cat in ['Toutes', ...categories]" :key="cat"
                    :class="['qv-cat-btn', { active: bankCatFilter === cat }]"
                    @click="bankCatFilter = cat">{{ cat }}</button>
                </div>
              </div>
              <div class="qv-bank-stats mt-auto p-4 bg-light rounded-4">
                <span>Total Assets: </span><strong>{{ bankGlobalReference.length }}</strong>
              </div>
            </aside>
            <section class="qv-list custom-scrollbar">
              <div v-for="qRef in filteredBankReference" :key="qRef.id"
                @click="currentInspectedBankItem = qRef"
                :class="['qv-item-card', { 'active': currentInspectedBankItem?.id === qRef.id, 'checked': isCocheeInBank(qRef.id) }]">
                <div class="qv-item-top">
                  <span class="qv-badge" :class="qRef.difficulty?.toLowerCase()">{{ qRef.difficulty }}</span>
                  <div class="qv-checkbox" @click.stop="toggleInBankPool(qRef)">
                    <i :class="isCocheeInBank(qRef.id) ? 'fa-solid fa-check-circle' : 'fa-regular fa-circle'"></i>
                  </div>
                </div>
                <h6 class="qv-item-text mt-3">{{ qRef.texte }}</h6>
                <div class="qv-item-footer mt-4"><span><strong>{{ qRef.poids }}</strong> PTS</span></div>
              </div>
            </section>
            <section class="qv-inspector">
              <div v-if="currentInspectedBankItem" class="p-5 animate__animated animate__fadeIn">
                <h5 class="fw-900 mb-4">Aperçu Technique</h5>
                <div class="device-mockup-v8 p-4">
                  <p class="fw-bold">{{ currentInspectedBankItem.texte }}</p>
                  <hr>
                  <p class="small text-muted">{{ currentInspectedBankItem.explication }}</p>
                </div>
                <button @click="toggleInBankPool(currentInspectedBankItem)" class="btn-enigma-primary mt-4 w-100">
                  <div class="btn-content">
                    <i :class="isCocheeInBank(currentInspectedBankItem.id) ? 'fa-solid fa-check me-2' : 'fa-solid fa-plus me-2'"></i>
                    {{ isCocheeInBank(currentInspectedBankItem.id) ? 'Retiré du panier' : 'Ajouter au panier' }}
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>
              <div v-else class="p-5 text-center text-muted d-flex flex-column align-items-center justify-content-center h-100">
                <i class="fa-solid fa-hand-pointer fa-2x mb-3"></i>
                <p class="small">Cliquez sur un actif pour l'inspecter</p>
              </div>
            </section>
          </div>
        </div>
      </div>
    </transition>

    <!-- ═══════════════════════════════════════════════════════
         MODALE : QUICK ADD QUESTION
    ═══════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="modals.quickAdd" class="quantum-vault-overlay" @click.self="closeQuickAdd">
        <div class="quick-add-modal animate__animated animate__zoomIn animate__faster">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-900 m-0">{{ editingQuestion ? 'Modifier la Question' : 'Créer une Question' }}</h5>
            <button @click="closeQuickAdd" class="btn-icon-sm"><i class="fa-solid fa-xmark"></i></button>
          </div>
          <div class="row g-4">
            <div class="col-12">
              <div class="enigma-input-wrap"><label>ÉNONCÉ *</label>
                <textarea v-model="newQuestion.texte" class="enigma-field" rows="3" placeholder="Quelle est la différence entre..."></textarea>
              </div>
            </div>
            <div class="col-md-6">
              <div class="enigma-input-wrap"><label>POINTS</label>
                <input type="number" v-model.number="newQuestion.poids" class="enigma-field" min="1" max="100">
              </div>
            </div>
            <div class="col-md-6">
              <div class="enigma-input-wrap"><label>DURÉE (SECONDRES)</label>
                <input type="number" v-model.number="newQuestion.duree" class="enigma-field" placeholder="Ex: 60">
              </div>
            </div>
            <div class="col-md-6">
              <div class="enigma-input-wrap"><label>DIFFICULTÉ</label>
                <select v-model="newQuestion.difficulty" class="enigma-field">
                  <option value="EASY">Facile</option>
                  <option value="MEDIUM">Moyen</option>
                  <option value="EXPERT">Expert</option>
                </select>
              </div>
            </div>
            <div class="col-12">
              <div class="enigma-input-wrap"><label>EXPLICATION (OPTIONNEL)</label>
                <textarea v-model="newQuestion.explication" class="enigma-field" rows="2" placeholder="Réponse attendue ou justification..."></textarea>
              </div>
            </div>
            <div class="col-12 d-flex gap-3 justify-content-end mt-2">
              <button @click="closeQuickAdd" class="btn-qv-cancel">ANNULER</button>
              <button @click="saveQuestion" class="btn-qv-confirm">{{ editingQuestion ? 'MODIFIER' : 'AJOUTER' }}</button>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- ═══════════════════════════════════════════════════════
         MODALE : RACCOURCIS CLAVIER
    ═══════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="modals.shortcuts" class="quantum-vault-overlay" @click.self="modals.shortcuts = false">
        <div class="shortcuts-modal animate__animated animate__zoomIn animate__faster">
          <div class="d-flex justify-content-between align-items-center mb-4">
            <h5 class="fw-900 m-0"><i class="fa-solid fa-keyboard me-2 text-amber"></i>Raccourcis Clavier</h5>
            <button @click="modals.shortcuts = false" class="btn-icon-sm"><i class="fa-solid fa-xmark"></i></button>
          </div>
          <div class="shortcuts-grid">
            <div class="shortcut-item" v-for="s in keyboardShortcuts" :key="s.label">
              <div class="d-flex gap-2">
                <kbd v-for="k in s.keys" :key="k">{{ k }}</kbd>
              </div>
              <span class="small text-muted">{{ s.label }}</span>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- ═══════════════════════════════════════════════════════
         CONFIRM DIALOG
    ═══════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="confirmDialog.show" class="quantum-vault-overlay" style="z-index:9999" @click.self="confirmDialog.show = false">
        <div class="confirm-modal animate__animated animate__zoomIn animate__faster">
          <div class="confirm-icon mb-3"><i :class="confirmDialog.icon" class="fa-2x text-danger"></i></div>
          <h5 class="fw-900 mb-2">{{ confirmDialog.title }}</h5>
          <p class="text-muted small mb-4">{{ confirmDialog.message }}</p>
          <div class="d-flex gap-3 justify-content-center">
            <button @click="confirmDialog.show = false" class="btn-qv-cancel">ANNULER</button>
            <!-- FIX #5: Close dialog BEFORE running the confirm callback to avoid visual flash -->
            <button @click="runConfirmDialog" class="btn-confirm-danger">CONFIRMER</button>
          </div>
        </div>
      </div>
    </transition>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body"><strong>SYSTEM MESSAGE</strong><p class="m-0 small">{{ globalToast.message }}</p></div>
      </div>
    </transition>
  </div>
</template>
<script setup>
import { ref, reactive, computed, onMounted, onUnmounted, watch } from 'vue';
import api from '@/services/api';
import draggable from 'vuedraggable';

const API_ENDPOINT = '';

/* ─── ÉTAT GLOBAL ─────────────────────────────────────────────── */
const isStudioMode   = ref(false);
const loading        = ref(true);
const currentStep    = ref(1);
const isPublishing   = ref(false);
const isScrolled     = ref(false);
const isSaving       = ref(false);
const lastSaved      = ref(false);
const lastSavedTime  = ref('');
const showValidation = ref(false);
const activeView     = ref('dashboard');
const viewMode       = ref('grid');
const dashboardSearch = ref('');
const sortBy         = ref('date');
const activeTab      = ref('all');
const tagInput       = ref('');
const candidateFilter = ref('');
const bankDiffFilter = ref('Tous');
const bankCatFilter  = ref('Toutes');
const searchBankQuery = ref('');
const searchCandQuery = ref('');

const campaigns           = ref([]);
const questionnairesList  = ref([]);
const candidateMasterPool = ref([]);
const bankGlobalReference = ref([]);
const selectedItemsInBank = ref([]);
const selectedCandidatesIds = ref([]);
const pinnedCampaigns     = ref([]);
const selectedQuestions   = ref([]);
const previewData         = ref(null);
const currentInspectedBankItem = ref(null);
const editingQuestion     = ref(null);
const editingIndex        = ref(-1);
const studioActivityFeed  = ref([{ id: 1, text: 'Studio initialisé', color: '#10b981', time: 'À l\'instant' }]);

const activityData = ref(
  ['L', 'M', 'M', 'J', 'V', 'S', 'D'].map(d => ({
    label: d,
    deploy: Math.floor(Math.random() * 80 + 20),
    cand:   Math.floor(Math.random() * 60 + 10),
  }))
);

const mousePos = reactive({ x: 0, y: 0 });
const modals   = reactive({ bank: false, quickAdd: false, shortcuts: false });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _cb: null });

const categories = ['Backend Specialist', 'Frontend Architect', 'Cybersecurity', 'DevOps', 'AI Logic'];

const studio = reactive({
  questionnaire: { id: null, titre: '', categorie: categories[1], duree: 45, scoreReussite: 70, description: '', tags: [] },
  campagne:      { id: null, nom: '', dateDebut: '', dateFin: '', maxCandidats: 100, anticheat: true, timerPerQuestion: false, sendNotifications: false, timezone: 'Europe/Paris' },
  questions:     [],
});

const newQuestion = reactive({ texte: '', poids: 10, difficulty: 'MEDIUM', explication: '', duree: 0 });

const steps = [
  { id: 1, label: 'Structure' },
  { id: 2, label: 'Ingénierie' },
  { id: 3, label: 'Ciblage' },
  { id: 4, label: 'Matrix' },
];

const keyboardShortcuts = [
  { keys: ['Ctrl', 'S'],     label: 'Sauvegarder le brouillon' },
  { keys: ['Ctrl', 'Enter'], label: 'Étape suivante' },
  { keys: ['Ctrl', 'B'],     label: 'Ouvrir la banque d\'actifs' },
  { keys: ['Ctrl', 'N'],     label: 'Créer une question' },
  { keys: ['Esc'],           label: 'Fermer les modales' },
];

const autoSaveTimer = ref(null);

/* ─── DATA FETCHING ───────────────────────────────────────────── */
const fetchInitialData = async () => {
  loading.value = true;
  try {
    const [resCamp, resQuest, resCand, resBank] = await Promise.all([
      api.get(`/Campagnes`),
      api.get(`/Questionnaires`),
      api.get(`/Candidates`),
      api.get(`/Questions`),
    ]);
    campaigns.value           = resCamp.data;
    questionnairesList.value  = resQuest.data;
    candidateMasterPool.value = resCand.data;
    bankGlobalReference.value = resBank.data.map(q => ({ 
      ...q, 
      difficulty: q.difficulty || 'EXPERT', 
      explication: q.bonneReponse || 'Justification UML standard.' 
    }));
  } catch (err) {
    console.warn("API Offline, utilisation des mocks");
    showPulseToast('Mode local activé (API Offline).', 'warn', 'fa-solid fa-plug-circle-xmark');
    generateMocks();
  } finally {
    loading.value = false;
  }
};

/* ─── DASHBOARD LOGIC ─────────────────────────────────────────── */
const filterTabs = computed(() => [
  { label: 'Tout',     value: 'all', count: campaigns.value.length },
  { label: 'Actives',  value: 1,     count: campaigns.value.filter(c => c.statut === 1).length },
  { label: 'Terminées',value: 2,     count: campaigns.value.filter(c => c.statut === 2).length },
]);

const filteredCampaigns = computed(() => {
  let list = [...campaigns.value];
  if (activeTab.value !== 'all') list = list.filter(c => c.statut === Number(activeTab.value));
  if (dashboardSearch.value) list = list.filter(c => c.nom.toLowerCase().includes(dashboardSearch.value.toLowerCase()));
  list.sort((a, b) => {
    if (pinnedCampaigns.value.includes(a.id) && !pinnedCampaigns.value.includes(b.id)) return -1;
    if (!pinnedCampaigns.value.includes(a.id) && pinnedCampaigns.value.includes(b.id)) return 1;
    if (sortBy.value === 'name')       return a.nom.localeCompare(b.nom);
    if (sortBy.value === 'candidates') return (b.maxCandidats || 0) - (a.maxCandidats || 0);
    return new Date(b.dateDebut || 0) - new Date(a.dateDebut || 0);
  });
  return list;
});

const kpiStats = computed(() => [
  { label: 'Sessions',  value: campaigns.value.length,           icon: 'fa-solid fa-layer-group', color: '#f59e0b', bg: '#fffbeb', trend: 12 },
  { label: 'Candidats', value: candidateMasterPool.value.length, icon: 'fa-solid fa-user-tie',    color: '#6366f1', bg: '#eef2ff', trend: 8  },
  { label: 'Questions', value: bankGlobalReference.value.length, icon: 'fa-solid fa-database',    color: '#10b981', bg: '#ecfdf5', trend: -2 },
  { label: 'Santé Moy.',value: '94%',                            icon: 'fa-solid fa-chart-line',  color: '#f43f5e', bg: '#fff1f2', trend: 5  },
]);

const donutSegments = computed(() => {
  const colors  = ['#f59e0b', '#6366f1', '#10b981', '#f43f5e', '#06b6d4'];
  const labels  = ['Planifié', 'Active', 'Terminée'];
  const total   = campaigns.value.length || 1;
  const circ    = 283; 
  const startOffset = circ / 4; 
  const grouped = { 0:0, 1:0, 2:0 };
  campaigns.value.forEach(c => { grouped[c.statut] = (grouped[c.statut] || 0) + 1; });

  let cumulative = 0;
  return Object.entries(grouped).map(([s, count], i) => {
    const dash = (count / total) * circ;
    const offset = startOffset - cumulative;
    cumulative += dash;
    return { label: labels[s] || 'Autre', count, color: colors[i % colors.length], dash, offset };
  });
});

/* ─── STUDIO CORE ─────────────────────────────────────────────── */
const enterStudioMode = (campaign = null) => {
  if (campaign) {
    Object.assign(studio.campagne, campaign);
    studio.questionnaire.id    = campaign.questionnaireId;
    studio.questionnaire.titre = getQuestionnaireName(campaign.questionnaireId);
    api.get(`/Questions/ByQuestionnaire/${campaign.questionnaireId}`)
      .then(res => { studio.questions = res.data; })
      .catch(() => { studio.questions = []; });
  } else {
    Object.assign(studio.questionnaire, { id: null, titre: '', categorie: categories[1], duree: 45, scoreReussite: 70, description: '', tags: [] });
    Object.assign(studio.campagne,      { id: null, nom: '', dateDebut: '', dateFin: '', maxCandidats: 100, anticheat: true, timerPerQuestion: false, sendNotifications: false, timezone: 'Europe/Paris' });
    studio.questions = [];
    selectedCandidatesIds.value = [];
    selectedQuestions.value = [];
  }
  currentStep.value  = 1;
  isStudioMode.value = true;
  showValidation.value = false;
  lastSaved.value    = false;
  studioActivityFeed.value = [{ id: Date.now(), text: 'Studio initialisé', color: '#10b981', time: 'À l\'instant' }];
};

const exitStudio = () => {
  if (studio.questionnaire.titre || studio.questions.length > 0) {
    showConfirmDialog(
      'Quitter le Studio ?',
      'Les modifications non déployées seront perdues.',
      'fa-solid fa-triangle-exclamation',
      () => { isStudioMode.value = false; }
    );
  } else {
    isStudioMode.value = false;
  }
};

watch(
  [() => studio.questionnaire.titre, () => studio.questions.length],
  () => {
    if (!isStudioMode.value) return;
    isSaving.value = true;
    clearTimeout(autoSaveTimer.value);
    autoSaveTimer.value = setTimeout(() => {
      try {
        localStorage.setItem('studio_draft', JSON.stringify({ questionnaire: studio.questionnaire, questions: studio.questions }));
        lastSaved.value    = true;
        lastSavedTime.value = new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' });
      } catch (e) {}
      isSaving.value = false;
    }, 1200);
  }
);

// Calcul automatique de la date de fin en fonction de la durée
watch(
  [() => studio.campagne.dateDebut, () => studio.questionnaire.duree],
  ([newDate, newDuree]) => {
    if (newDate && newDuree) {
      const date = new Date(newDate);
      date.setMinutes(date.getMinutes() + parseInt(newDuree));
      
      // Formater en YYYY-MM-DDTHH:mm pour l'input datetime-local
      const y = date.getFullYear();
      const m = String(date.getMonth() + 1).padStart(2, '0');
      const d = String(date.getDate()).padStart(2, '0');
      const h = String(date.getHours()).padStart(2, '0');
      const min = String(date.getMinutes()).padStart(2, '0');
      
      studio.campagne.dateFin = `${y}-${m}-${d}T${h}:${min}`;
    }
  }
);

/* ─── STUDIO COMPUTED ─────────────────────────────────────────── */
const architectureHealth = computed(() => {
  let score = 0;
  if (studio.questionnaire.titre.length > 5)   score += 25;
  if (studio.questions.length >= 3)             score += 35;
  if (selectedCandidatesIds.value.length > 0)   score += 25;
  if (studio.campagne.dateDebut)                score += 15;
  return score;
});
const healthStatusText = computed(() => architectureHealth.value > 80 ? 'STABLE' : architectureHealth.value > 50 ? 'EN COURS' : 'CRITIQUE');
const healthColor      = computed(() => architectureHealth.value > 80 ? '#10b981' : architectureHealth.value > 50 ? '#f59e0b' : '#f43f5e');
const healthRingStyle  = computed(() => {
  const circ = 2 * Math.PI * 19;
  return { strokeDasharray: `${circ} ${circ}`, strokeDashoffset: circ - (architectureHealth.value / 100) * circ, stroke: healthColor.value };
});
const stepperProgress    = computed(() => ((currentStep.value - 1) / (steps.length - 1)) * 100);
const totalLogicPoints   = computed(() => studio.questions.reduce((a, b) => a + (Number(b.poids) || 0), 0));
const isReadyToPublish   = computed(() => architectureHealth.value >= 70);
const coachAIAdvice      = computed(() => {
  if (!studio.questionnaire.titre)             return 'Commencez par nommer votre instance UML.';
  if (studio.questions.length < 3)             return 'Ajoutez au moins 3 actifs pour stabiliser la structure.';
  if (selectedCandidatesIds.value.length === 0) return 'Ciblez des profils pour permettre le déploiement.';
  if (!studio.campagne.dateDebut)              return 'Définissez les dates d\'ouverture du terminal.';
  return 'Architecture certifiée prête pour le déploiement SQL.';
});
const validationState = computed(() => {
  if (!studio.questionnaire.titre) return { text: 'TITRE MANQUANT', class: 'cl-err' };
  if (studio.questions.length < 3) return { text: 'PUITS FAIBLE',   class: 'cl-warn' };
  return { text: 'SYSTÈME VALIDE', class: 'cl-success' };
});
const deployChecklist = computed(() => [
  { label: 'Titre défini',        passed: studio.questionnaire.titre.length > 2 },
  { label: 'Minimum 3 questions', passed: studio.questions.length >= 3 },
  { label: 'Candidats ciblés',    passed: selectedCandidatesIds.value.length > 0 },
  { label: 'Date d\'ouverture',   passed: !!studio.campagne.dateDebut },
]);

/* ─── BANK & CANDIDATES ───────────────────────────────────────── */
const filteredBankReference = computed(() => {
  let list = bankGlobalReference.value;
  if (searchBankQuery.value) list = list.filter(q => q.texte?.toLowerCase().includes(searchBankQuery.value.toLowerCase()));
  if (bankDiffFilter.value !== 'Tous') list = list.filter(q => q.difficulty === bankDiffFilter.value);
  if (bankCatFilter.value !== 'Toutes') list = list.filter(q => q.categorie === bankCatFilter.value);
  return list;
});
const filteredCandidatePool = computed(() =>
  candidateMasterPool.value.filter(c => c.name?.toLowerCase().includes(searchCandQuery.value.toLowerCase()))
);

const openDeepBankModal  = () => { modals.bank = true; selectedItemsInBank.value = []; };
const isCocheeInBank     = (id) => selectedItemsInBank.value.some(q => q.id === id);
const toggleInBankPool   = (q) => {
  const idx = selectedItemsInBank.value.findIndex(item => item.id === q.id);
  idx > -1 ? selectedItemsInBank.value.splice(idx, 1) : selectedItemsInBank.value.push(JSON.parse(JSON.stringify(q)));
};
const confirmStudioSync  = () => {
  const added = selectedItemsInBank.value.map(q => ({ 
    texte: q.texte, 
    poids: q.poids, 
    difficulty: q.difficulty, 
    explication: q.explication,
    id: `temp-${Math.random()}` 
  }));
  studio.questions.push(...added);
  modals.bank = false;
  addFeedItem(`${added.length} actif(s) importé(s)`, '#f59e0b');
  showPulseToast('Actifs synchronisés', 'success', 'fa-solid fa-bolt');
};

const toggleCandidateTarget = (c) => {
  const idx = selectedCandidatesIds.value.indexOf(c.id);
  idx > -1 ? selectedCandidatesIds.value.splice(idx, 1) : selectedCandidatesIds.value.push(c.id);
};
const isSelectedCandidate = (id) => selectedCandidatesIds.value.includes(id);
const selectAllCandidates = () => { selectedCandidatesIds.value = filteredCandidatePool.value.map(c => c.id); };

/* ─── STEPPER NAV ─────────────────────────────────────────────── */
const jumpToStep = (id) => { if (id <= currentStep.value + 1) currentStep.value = id; };
const nextStep   = () => {
  if (currentStep.value === 1 && !studio.questionnaire.titre) { showValidation.value = true; return; }
  currentStep.value++;
  window.scrollTo(0, 0);
};
const prevStep = () => { currentStep.value--; window.scrollTo(0, 0); };

/* ─── PUBLISH TRANSACTION (CORRECTED) ─────────────────────────── */
const publishToProduction = async () => {
  if (!isReadyToPublish.value) return;
  isPublishing.value = true;
  try {
    // 1. Création du Questionnaire
    const qResp = await api.post(`/Questionnaires`, {
      Titre: studio.questionnaire.titre,
      Description: studio.questionnaire.description || "",
      DureeMinutes: studio.questionnaire.duree || 60,
      ScoreReussite: studio.questionnaire.scoreReussite || 70
    });
    const qId = qResp.data.id;

    // 2. Création des Questions (On retire les IDs temporaires pour laisser le Guid se générer)
    await Promise.all(
      studio.questions.map(q => {
        const payload = {
          Enonce: q.texte || "Question sans titre",
          Points: Number(q.poids) || 1,
          DureeSecondes: q.duree > 0 ? q.duree : null,
          Type: 0, 
          Niveau: 1, 
          BonneReponse: q.explication || "",
          QuestionnaireId: qId,
          Choix: [],
          Prerequis: []
        };
        return api.post(`/Questions`, payload);
      })
    );

    // 3. Création de la Campagne
    await api.post(`/Campagnes`, {
      Nom: studio.campagne.nom || `ARCHITECTE : ${studio.questionnaire.titre}`,
      Description: studio.questionnaire.description || "",
      QuestionnaireId: qId,
      DateDebut: new Date(studio.campagne.dateDebut).toISOString(),
      DateFin: new Date(studio.campagne.dateFin).toISOString(),
      MaxCandidats: studio.campagne.maxCandidats,
      ScorePassage: studio.questionnaire.scoreReussite,
      DureeMinutes: studio.questionnaire.duree,
      Statut: 1,
      SelectedCandidatesIds: selectedCandidatesIds.value 
    });

    showPulseToast('DÉPLOIEMENT RÉUSSI', 'success', 'fa-solid fa-server');
    isStudioMode.value = false;
    fetchInitialData();
  } catch (err) {
    console.error("Erreur détaillée:", err.response?.data);
    showPulseToast('ERREUR DE DÉPLOIEMENT (400)', 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    isPublishing.value = false;
  }
};

/* ─── CRUD & UTILS ────────────────────────────────────────────── */
const handleDelete = (id) => {
  showConfirmDialog(
    'Supprimer cette session ?',
    'Cette action est irréversible.',
    'fa-solid fa-trash-can',
    async () => {
      try {
        await api.delete(`/Campagnes/${id}`);
        showPulseToast('Session supprimée.', 'warn', 'fa-solid fa-trash-can');
        fetchInitialData();
      } catch {
        campaigns.value = campaigns.value.filter(c => c.id !== id);
      }
    }
  );
};

const duplicateCampaign = (c) => {
  const dup = { ...c, id: crypto.randomUUID(), nom: c.nom + ' (Copie)', statut: 0 };
  campaigns.value.push(dup);
  showPulseToast('Architecture dupliquée.', 'success', 'fa-regular fa-copy');
};

const exportCampaign = (c) => {
  const data = { campaign: c, exportedAt: new Date().toISOString() };
  const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
  const url  = URL.createObjectURL(blob);
  const a    = document.createElement('a');
  a.href = url; a.download = `campaign-${c.id}.json`; a.click();
  URL.revokeObjectURL(url);
};

const togglePinCampaign = (id) => {
  const idx = pinnedCampaigns.value.indexOf(id);
  idx > -1 ? pinnedCampaigns.value.splice(idx, 1) : pinnedCampaigns.value.push(id);
};

const previewCampaign = (c) => { previewData.value = c; activeView.value = 'preview'; };
const changeStatus    = (c, s) => { c.statut = s; showPulseToast('Statut mis à jour.', 'success', 'fa-solid fa-check'); };
const getCampaignProgress = (c) => ({ 0: 10, 1: 60, 2: 100 }[c.statut] ?? 0);
const getProgressColor    = (p) => p >= 100 ? '#10b981' : p >= 50 ? '#f59e0b' : '#6366f1';

const previewTimeline = computed(() => [
  { id: 1, title: 'Campagne créée', desc: 'Architecte initialisé.', color: '#6366f1', time: formatDate(previewData.value?.dateDebut) },
  { id: 2, title: 'Candidats notifiés', desc: 'Invitations envoyées.', color: '#f59e0b', time: 'J+1' },
  { id: 3, title: 'Session ouverte', desc: 'Accès activé.', color: '#10b981', time: formatDate(previewData.value?.dateDebut) },
]);

/* ─── MODAL ACTIONS ───────────────────────────────────────────── */
const editQuestion = (q, idx) => {
  editingQuestion.value = q;
  editingIndex.value    = idx;
  Object.assign(newQuestion, { 
    texte: q.texte, 
    poids: q.poids, 
    difficulty: q.difficulty || 'MEDIUM', 
    explication: q.explication || '',
    duree: q.duree || 0
  });
  modals.quickAdd = true;
};

const saveQuestion = () => {
  if (!newQuestion.texte.trim()) return;
  if (editingQuestion.value !== null) {
    Object.assign(studio.questions[editingIndex.value], { ...newQuestion });
  } else {
    studio.questions.push({ ...newQuestion, id: `custom-${Date.now()}` });
  }
  showPulseToast('Question enregistrée.', 'success', 'fa-solid fa-check');
  closeQuickAdd();
};

const closeQuickAdd = () => {
  modals.quickAdd = false; editingQuestion.value = null;
  Object.assign(newQuestion, { texte: '', poids: 10, difficulty: 'MEDIUM', explication: '', duree: 0 });
};

const toggleSelectQuestion  = (id) => {
  const idx = selectedQuestions.value.indexOf(id);
  idx > -1 ? selectedQuestions.value.splice(idx, 1) : selectedQuestions.value.push(id);
};
const toggleSelectAll = (e) => {
  selectedQuestions.value = e.target.checked ? studio.questions.map(q => q.id) : [];
};
const removeSelectedQuestions = () => {
  studio.questions = studio.questions.filter(q => !selectedQuestions.value.includes(q.id));
  selectedQuestions.value = [];
};

const addTag = () => {
  const t = tagInput.value.trim().replace(',', '');
  if (t && !studio.questionnaire.tags.includes(t)) studio.questionnaire.tags.push(t);
  tagInput.value = '';
};
const removeTag = (i) => studio.questionnaire.tags.splice(i, 1);

const addFeedItem = (text, color) => {
  studioActivityFeed.value.unshift({ id: Date.now(), text, color, time: new Date().toLocaleTimeString() });
  if (studioActivityFeed.value.length > 5) studioActivityFeed.value.pop();
};

const showConfirmDialog = (title, message, icon, cb) => {
  Object.assign(confirmDialog, { title, message, icon, _cb: cb, show: true });
};
const runConfirmDialog = () => {
  confirmDialog.show = false;
  if (confirmDialog._cb) confirmDialog._cb();
};

/* ─── HELPERS ─────────────────────────────────────────────────── */
const getQuestionnaireName = (id)  => questionnairesList.value.find(q => q.id === id)?.titre || 'UML Inconnu';
const getStatusLabel       = (s)   => (['Planifié', 'Active', 'Terminée'][s] ?? 'Brouillon');
const formatDate           = (d)   => d ? new Date(d).toLocaleDateString('fr-FR') : '∞';

let _toastTimer = null;
const showPulseToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

const orbStyle      = (f)  => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => { mousePos.x = (e.clientX - window.innerWidth / 2) / 20; mousePos.y = (e.clientY - window.innerHeight / 2) / 20; };
const handleScroll   = (e) => { isScrolled.value = e.target.scrollTop > 50; };

const handleKeyboard = (e) => {
  if (e.key === 'Escape') { Object.keys(modals).forEach(k => modals[k] = false); confirmDialog.show = false; }
  if (e.ctrlKey && e.key === 'b' && isStudioMode.value) { e.preventDefault(); openDeepBankModal(); }
  if (e.ctrlKey && e.key === 'Enter' && isStudioMode.value) { e.preventDefault(); nextStep(); }
};

const generateMocks = () => {
  campaigns.value = [{ id: '1', nom: 'Frontend Senior Audit', questionnaireId: '10', statut: 1, maxCandidats: 25, dateDebut: new Date().toISOString() }];
  questionnairesList.value = [{ id: '10', titre: 'Vue.js Architecture Test' }];
  candidateMasterPool.value = [{ id: '1', name: 'Alice Durand', email: 'alice@tech.io' }];
  bankGlobalReference.value = [{ id: '101', texte: "Expliquez l'Event Loop Node.js", poids: 15, difficulty: 'EXPERT' }];
};

onMounted(() => {
  fetchInitialData();
  document.addEventListener('keydown', handleKeyboard);
});
onUnmounted(() => {
  document.removeEventListener('keydown', handleKeyboard);
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&display=swap');

.enigma-master-root { min-height: 100vh; background: #f8fafc; font-family: 'Plus Jakarta Sans', sans-serif; color: #0f172a; }

/* BACKGROUND */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid { position: absolute; inset: 0; background-image: radial-gradient(#cbd5e1 1px, transparent 1px); background-size: 40px 40px; opacity: 0.2; }
.glow-orb { position: absolute; width: 600px; height: 600px; filter: blur(120px); opacity: 0.15; border-radius: 50%; transition: transform 0.3s ease-out; }
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { background: #6366f1; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* DASHBOARD */
.premium-title { font-weight: 800; font-size: 2.2rem; letter-spacing: -1px; }
.gradient-text { background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%); -webkit-background-clip: text; -webkit-text-fill-color: transparent; }
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: #94a3b8; }
.breadcrumb-pro .root { cursor: pointer; } .breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }

/* STAT CARDS */
.stat-card-premium { background: white; border-radius: 24px; padding: 24px; display: flex; align-items: center; border: 1px solid #eef2f6; transition: 0.2s; }
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: 0 10px 30px rgba(0,0,0,0.06); }
.stat-icon-wrapper { width: 56px; height: 56px; border-radius: 16px; display: flex; align-items: center; justify-content: center; font-size: 1.4rem; flex-shrink: 0; }
.stat-value { font-size: 1.6rem; font-weight: 800; line-height: 1; }
.stat-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; text-transform: uppercase; margin-top: 4px; }
.stat-trend { display: flex; flex-direction: column; align-items: center; font-size: 0.65rem; font-weight: 800; gap: 2px; }
.trend-up { color: #10b981; } .trend-down { color: #f43f5e; }

/* VIEW TOGGLE */
.view-toggle-cluster { display: flex; background: white; border: 1px solid #eef2f6; border-radius: 14px; padding: 4px; gap: 2px; }
.btn-view-toggle { width: 36px; height: 36px; border-radius: 10px; border: none; background: transparent; color: #94a3b8; transition: 0.2s; font-size: 0.85rem; cursor: pointer; }
.btn-view-toggle.active { background: #0f172a; color: white; }
.btn-refresh-pro { width: 44px; height: 44px; border-radius: 14px; border: 1.5px solid #eef2f6; background: white; cursor: pointer; color: #64748b; transition: 0.2s; }
.btn-refresh-pro:hover { background: #f8fafc; }

/* CAMPAIGN CARDS */
.campaign-card-modern { background: white; border-radius: 30px; padding: 28px; border: 1px solid #eef2f6; transition: 0.3s cubic-bezier(0.4,0,0.2,1); cursor: pointer; height: 100%; }
.campaign-card-modern:hover { transform: translateY(-10px); border-color: #f59e0b; box-shadow: 0 25px 50px -12px rgba(0,0,0,0.08); }
.campaign-title-modern { font-size: 1rem; color: #0f172a; margin-bottom: 0; }
.status-badge { padding: 5px 12px; border-radius: 10px; font-size: 0.65rem; font-weight: 800; text-transform: uppercase; display: inline-flex; align-items: center; }
.status-0 { background: #f0f9ff; color: #6366f1; } .status-1 { background: #ecfdf5; color: #10b981; } .status-2 { background: #f1f5f9; color: #64748b; }
.status-dot { width: 6px; height: 6px; border-radius: 50%; background: currentColor; margin-right: 6px; }
.pin-badge { font-size: 0.6rem; font-weight: 800; color: #f59e0b; background: #fffbeb; padding: 2px 8px; border-radius: 8px; margin-bottom: 8px; display: inline-block; }
.progress-slim { height: 4px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-fill { height: 100%; border-radius: 10px; transition: width 0.6s ease; }
.btn-options-round { width: 32px; height: 32px; border-radius: 10px; border: 1.5px solid #eef2f6; background: white; cursor: pointer; font-size: 0.8rem; color: #94a3b8; }
.shadow-premium { box-shadow: 0 20px 60px rgba(0,0,0,0.12) !important; }

/* ANALYTICS */
.analytics-card-pro { background: white; border-radius: 24px; border: 1px solid #eef2f6; }
.bar-chart-v2 { display: flex; align-items: flex-end; gap: 8px; height: 120px; }
.bar-col { display: flex; flex-direction: column; align-items: center; gap: 4px; flex: 1; }
.bar-wrap { display: flex; gap: 3px; align-items: flex-end; height: 100%; width: 100%; justify-content: center; }
.bar-fill { width: 12px; border-radius: 6px 6px 0 0; transition: height 0.8s ease; min-height: 4px; }
.bar-amber { background: #f59e0b; } .bar-indigo { background: #6366f1; }
.bar-label { font-size: 0.6rem; font-weight: 800; color: #94a3b8; }
.legend-dot { width: 8px; height: 8px; border-radius: 50%; display: inline-block; }
.dot-amber { background: #f59e0b; } .dot-indigo { background: #6366f1; }
.donut-chart-container { display: flex; align-items: center; gap: 20px; }
.donut-legend { display: flex; flex-direction: column; gap: 8px; flex: 1; }
.donut-legend-item { display: flex; align-items: center; gap: 8px; }
.legend-dot-sm { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.donut-center-text { font-size: 22px; font-weight: 900; fill: #0f172a; }
.donut-sub-text { font-size: 8px; fill: #94a3b8; font-weight: 700; }

/* TABS */
.nav-tab-btn-modern { padding: 8px 18px; border-radius: 12px; border: none; background: transparent; font-weight: 800; font-size: 0.8rem; color: #94a3b8; cursor: pointer; transition: 0.2s; }
.nav-tab-btn-modern.active { background: #0f172a; color: white; }
.tab-count { background: rgba(255,255,255,0.2); padding: 2px 7px; border-radius: 8px; font-size: 0.65rem; margin-left: 6px; }
.nav-tab-btn-modern:not(.active) .tab-count { background: #f1f5f9; color: #64748b; }

/* SEARCH + SORT */
.search-inline-box { display: flex; align-items: center; background: white; border: 1.5px solid #eef2f6; border-radius: 14px; padding: 0 14px; gap: 10px; color: #94a3b8; }
.search-inline-input { border: none; outline: none; background: transparent; padding: 10px 0; font-weight: 700; font-size: 0.85rem; width: 180px; font-family: inherit; }
.btn-clear-search { border: none; background: transparent; color: #94a3b8; padding: 0; cursor: pointer; }
.sort-select-pro { border: 1.5px solid #eef2f6; border-radius: 14px; padding: 10px 14px; font-weight: 700; font-size: 0.8rem; background: white; outline: none; cursor: pointer; font-family: inherit; }

/* LIST VIEW */
.list-header-row { background: #f8fafc; border-radius: 14px; }
.list-col-label { font-size: 0.6rem; font-weight: 900; color: #94a3b8; text-transform: uppercase; letter-spacing: 1px; }
.list-row-item { background: white; border-radius: 16px; border: 1px solid #eef2f6; transition: 0.2s; }
.list-row-item:hover { border-color: #f59e0b; }
.slot-badge { background: #fffbeb; color: #f59e0b; font-weight: 800; font-size: 0.75rem; padding: 3px 10px; border-radius: 8px; }

/* EMPTY STATE */
.empty-state-pro { background: white; border-radius: 30px; padding: 40px; border: 1px dashed #e2e8f0; }

/* PREVIEW */
.preview-field-box { display: flex; flex-direction: column; gap: 6px; }
.preview-field-label { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; text-transform: uppercase; }
.big-progress-container { display: flex; justify-content: center; }
.btn-quick-action { width: 100%; background: #f8fafc; border: 1.5px solid #eef2f6; border-radius: 14px; padding: 12px 16px; text-align: left; font-weight: 700; font-size: 0.82rem; cursor: pointer; transition: 0.2s; font-family: inherit; }
.btn-quick-action:hover { border-color: #f59e0b; background: #fffbeb; }
.activity-timeline { display: flex; flex-direction: column; }
.timeline-item { display: flex; align-items: flex-start; gap: 16px; padding: 16px 0; border-bottom: 1px solid #f1f5f9; }
.tl-dot { width: 10px; height: 10px; border-radius: 50%; flex-shrink: 0; margin-top: 5px; }
.tl-content { flex: 1; }
.tl-time { font-size: 0.65rem; color: #94a3b8; font-weight: 700; white-space: nowrap; }

/* STUDIO HEADER */
.studio-header-v2 { position: sticky; top: 0; z-index: 100; background: rgba(255,255,255,0.92); backdrop-filter: blur(12px); border-bottom: 1px solid #eef2f6; transition: box-shadow 0.3s; }
.studio-header-v2.is-docked { box-shadow: 0 8px 30px rgba(0,0,0,0.06); }
.main-title-v2 { font-weight: 900; font-size: 1.8rem; margin: 0; }
.main-title-v2 span { color: #f59e0b; }
.v-badge { font-size: 0.7rem; background: #fffbeb; color: #f59e0b; padding: 3px 10px; border-radius: 8px; font-weight: 800; vertical-align: middle; }
.brand-subtitle-v2 { font-size: 0.6rem; font-weight: 800; color: #94a3b8; letter-spacing: 1px; margin: 0; }
.btn-back-to-dash { width: 44px; height: 44px; border-radius: 14px; border: 1.5px solid #e2e8f0; background: white; cursor: pointer; color: #64748b; transition: 0.2s; }
.btn-back-to-dash:hover { background: #0f172a; color: white; border-color: #0f172a; }

/* HEADER ACTIONS PRO */
.btn-refresh-pro {
  width: 44px;
  height: 44px;
  background: white;
  border: 1.5px solid #e2e8f0;
  border-radius: 14px;
  color: #64748b;
  cursor: pointer;
  transition: 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 6px rgba(0,0,0,0.02);
}
.btn-refresh-pro:hover:not(:disabled) {
  background: #f8fafc;
  border-color: #f59e0b;
  color: #f59e0b;
  transform: rotate(180deg) scale(1.1);
  box-shadow: 0 8px 15px rgba(245, 158, 11, 0.1);
}

.view-toggle-cluster {
  display: flex;
  background: white;
  padding: 4px;
  border: 1.5px solid #e2e8f0;
  border-radius: 16px;
  gap: 4px;
  box-shadow: 0 4px 6px rgba(0,0,0,0.02);
}

.btn-view-toggle {
  width: 38px;
  height: 38px;
  border-radius: 12px;
  border: none;
  background: transparent;
  color: #94a3b8;
  cursor: pointer;
  transition: 0.3s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-view-toggle:hover {
  background: #f8fafc;
  color: #0f172a;
}

.btn-view-toggle.active {
  background: #0f172a;
  color: #f59e0b;
  box-shadow: 0 4px 12px rgba(15, 23, 42, 0.2);
}
.ai-robot-terminal { width: 50px; height: 50px; background: #0f172a; border-radius: 15px; display: flex; align-items: center; justify-content: center; }

/* AUTOSAVE */
.autosave-indicator { display: flex; align-items: center; gap: 8px; font-size: 0.7rem; font-weight: 700; color: #94a3b8; background: #f8fafc; padding: 8px 14px; border-radius: 12px; border: 1px solid #eef2f6; }
.autosave-indicator.saved { color: #10b981; } .autosave-indicator.saving { color: #f59e0b; }

/* HEALTH WIDGET */
.health-core-widget { display: flex; align-items: center; gap: 12px; background: #f8fafc; padding: 10px 16px; border-radius: 16px; border: 1px solid #eef2f6; }
.health-ring-box { position: relative; display: flex; align-items: center; justify-content: center; }
.health-ring-box svg { overflow: visible; }
.ring-track { fill: none; stroke: #eef2f6; stroke-width: 3; }
.ring-fill  { fill: none; stroke-width: 3; stroke-linecap: round; transform: rotate(-90deg); transform-origin: 50% 50%; transition: stroke-dasharray 0.8s ease, stroke 0.5s ease; }
.health-percent { position: absolute; font-size: 0.65rem; font-weight: 900; color: #0f172a; }
.health-label-box .h-main { font-size: 0.55rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; display: block; }
.health-label-box .h-sub  { font-size: 0.7rem; font-weight: 800; }
.btn-shortcuts { width: 40px; height: 40px; border-radius: 12px; border: 1.5px solid #eef2f6; background: white; color: #94a3b8; cursor: pointer; transition: 0.2s; }
.btn-shortcuts:hover { background: #0f172a; color: white; border-color: #0f172a; }

/* STEPPER */
.stepper-precision-v8 { position: relative; margin-top: 30px; padding-bottom: 10px; }
.stepper-line-bg { height: 4px; background: #eef2f6; border-radius: 10px; position: absolute; top: 20px; left: 22px; right: 22px; }
.stepper-line-fill { height: 100%; background: linear-gradient(90deg, #f59e0b, #fbbf24); transition: width 0.6s ease; border-radius: 10px; }
.stepper-nodes-row { display: flex; justify-content: space-between; position: relative; }
.step-node-v8 { display: flex; flex-direction: column; align-items: center; cursor: pointer; }
.node-point { width: 44px; height: 44px; background: white; border: 4px solid #eef2f6; border-radius: 50%; display: flex; align-items: center; justify-content: center; z-index: 2; font-weight: 900; font-size: 0.85rem; color: #94a3b8; position: relative; transition: 0.3s; }
.node-label-floating { position: absolute; top: 52px; font-size: 0.6rem; font-weight: 800; color: #94a3b8; white-space: nowrap; }
.step-node-v8.active .node-point { border-color: #f59e0b; color: #f59e0b; transform: scale(1.1); box-shadow: 0 0 0 4px rgba(245,158,11,0.15); }
.step-node-v8.active .node-label-floating { color: #f59e0b; }
.step-node-v8.completed .node-point { background: #f59e0b; border-color: #f59e0b; color: white; }

/* FORMS */
.enigma-card { background: white; border-radius: 32px; border: 1px solid #eef2f6; }
.enigma-field { width: 100%; padding: 15px 20px; background: #f8fafc; border: 2px solid #f59e0b; border-radius: 16px; font-weight: 700; outline: none; font-family: inherit; transition: border-color 0.2s; font-size: 0.9rem; color: #0f172a; }
.enigma-field:focus { border-color: #f59e0b; background: white; }
.enigma-field.field-error { border-color: #f43f5e !important; }
.field-error-msg { font-size: 0.7rem; color: #f43f5e; font-weight: 700; margin-top: 4px; display: block; }
.enigma-input-wrap label { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; margin-bottom: 8px; display: block; }
.enigma-range { accent-color: #f59e0b; width: 100%; }
.admissibility-dashboard { background: #f8fafc; border-radius: 20px; padding: 24px; }
.score-tier { font-size: 0.6rem; font-weight: 800; opacity: 0.5; }
.tier-low { color: #10b981; } .tier-mid { color: #f59e0b; } .tier-high { color: #f43f5e; }
.pane-header-v2 { display: flex; align-items: center; gap: 20px; }
.icon-box-v2 { width: 52px; height: 52px; border-radius: 18px; display: flex; align-items: center; justify-content: center; font-size: 1.3rem; flex-shrink: 0; }
.icon-box-v2.amber { background: #fffbeb; color: #f59e0b; }

/* TAGS */
.tags-input-container { cursor: text; }
.tag-chip { background: #fffbeb; color: #f59e0b; font-size: 0.7rem; font-weight: 800; padding: 4px 10px; border-radius: 8px; display: flex; align-items: center; gap: 6px; }
.tag-remove { border: none; background: transparent; color: #f59e0b; padding: 0; cursor: pointer; font-size: 0.7rem; line-height: 1; }
.tag-field-input { border: none; outline: none; background: transparent; font-weight: 700; font-size: 0.85rem; flex: 1; min-width: 100px; font-family: inherit; }

/* PROTOCOL ROWS */
.protocol-row { display: flex; align-items: center; gap: 20px; }
.p-icon { width: 44px; height: 44px; background: #f8fafc; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 1.1rem; color: #f59e0b; flex-shrink: 0; }
.p-data { flex: 1; } .p-data h6 { font-weight: 800; margin: 0; font-size: 0.9rem; } .p-data p { font-size: 0.75rem; color: #94a3b8; margin: 0; }
.enigma-switch .form-check-input { width: 44px; height: 24px; cursor: pointer; }
.enigma-switch .form-check-input:checked { background-color: #f59e0b; border-color: #f59e0b; }

/* ASSET CARDS (Step 2) */
.assets-scroll-v8 { max-height: 500px; overflow-y: auto; padding-right: 4px; }
.asset-card-v8 { background: white; border: 1.5px solid #eef2f6; border-radius: 20px; padding: 15px 20px; display: flex; align-items: center; gap: 15px; margin-bottom: 12px; transition: 0.2s; }
.asset-card-v8:hover { border-color: #f59e0b; transform: translateX(4px); }
.asset-card-v8.selected { border-color: #6366f1; background: #f5f3ff; }
.asset-checkbox { width: 16px; height: 16px; accent-color: #6366f1; flex-shrink: 0; cursor: pointer; }
.drag-node-handle { color: #cbd5e1; cursor: grab; font-size: 0.85rem; flex-shrink: 0; }
.drag-node-handle:active { cursor: grabbing; }
.asset-index-v8 { font-size: 0.6rem; font-weight: 900; background: #fffbeb; color: #f59e0b; padding: 4px 8px; border-radius: 8px; flex-shrink: 0; }
.asset-title-v8 { font-size: 0.88rem; font-weight: 700; margin: 0; }
.asset-ghost-v8 { opacity: 0.4; background: #fffbeb; }
.btn-remove-v8 { width: 28px; height: 28px; border-radius: 8px; border: none; background: #f1f5f9; color: #94a3b8; cursor: pointer; flex-shrink: 0; font-size: 0.75rem; transition: 0.2s; }
.btn-remove-v8:hover { background: #fee2e2; color: #f43f5e; }
.t-pill { font-size: 0.6rem; font-weight: 800; padding: 2px 8px; border-radius: 6px; }
.t-pill.weight { background: #fffbeb; color: #f59e0b; }
.diff-expert { background: #fff1f2; color: #f43f5e; } .diff-medium { background: #fffbeb; color: #f59e0b; } .diff-easy { background: #ecfdf5; color: #10b981; }
.bulk-actions-bar { background: #f8fafc; border-radius: 14px; padding: 10px 16px; display: flex; align-items: center; gap: 10px; }
.btn-bank-action-v2 { background: #0f172a; color: white; border: none; padding: 10px 20px; border-radius: 14px; font-weight: 800; font-size: 0.78rem; cursor: pointer; }
.empty-questions-hint { background: #f8fafc; border-radius: 16px; }

/* CANDIDATES */
.talent-hub-search-v2 { display: flex; align-items: center; gap: 16px; flex-wrap: wrap; }
.search-enigma-box { display: flex; align-items: center; gap: 10px; background: white; border: 1.5px solid #eef2f6; border-radius: 14px; padding: 0 16px; flex: 1; min-width: 200px; color: #94a3b8; }
.enigma-search-field { border: none; outline: none; background: transparent; padding: 12px 0; font-weight: 700; font-family: inherit; flex: 1; }
.batch-counter-v2 { background: #0f172a; color: white; padding: 10px 18px; border-radius: 14px; display: flex; flex-direction: column; align-items: center; }
.batch-counter-v2 .val { font-size: 1.2rem; font-weight: 900; line-height: 1; }
.batch-counter-v2 .lab { font-size: 0.5rem; font-weight: 800; opacity: 0.5; letter-spacing: 1px; }
.filter-chip { padding: 6px 14px; border-radius: 10px; border: 1.5px solid #eef2f6; background: white; font-size: 0.7rem; font-weight: 800; cursor: pointer; transition: 0.2s; font-family: inherit; }
.filter-chip.active { background: #0f172a; color: white; border-color: #0f172a; }
.talent-matrix-grid { max-height: 420px; overflow-y: auto; }
.talent-card-v8 { background: white; border: 2px solid #eef2f6; border-radius: 20px; padding: 18px; display: flex; align-items: center; gap: 14px; cursor: pointer; transition: 0.2s; position: relative; }
.talent-card-v8:hover { border-color: #f59e0b; }
.talent-card-v8.active { border-color: #10b981; background: #f0fdf4; }
.card-check-v8 { position: absolute; top: 12px; right: 12px; width: 20px; height: 20px; border-radius: 50%; background: #10b981; color: white; font-size: 0.6rem; display: flex; align-items: center; justify-content: center; opacity: 0; transition: opacity 0.2s; }
.talent-card-v8.active .card-check-v8 { opacity: 1; }
.avatar-v8 { width: 44px; height: 44px; border-radius: 14px; background: linear-gradient(135deg, #f59e0b, #fbbf24); color: #0f172a; font-weight: 900; font-size: 1.1rem; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.data-v8 b { font-size: 0.9rem; } .data-v8 p { font-size: 0.72rem; }

/* ANALYTICS HUD */
.analytics-hub-glass { background: #0f172a; color: white; border-radius: 40px; padding: 40px; }
.hub-label { font-size: 0.55rem; font-weight: 900; opacity: 0.4; letter-spacing: 2px; text-transform: uppercase; }
.hub-title-v2 { font-size: 1.1rem; font-weight: 900; margin-top: 8px; margin-bottom: 4px; word-break: break-word; }
.hub-status-box { font-size: 0.6rem; font-weight: 800; padding: 6px 12px; border-radius: 10px; display: inline-flex; align-items: center; gap: 8px; margin-top: 10px; }
.cl-success { background: rgba(16,185,129,0.1); color: #10b981; }
.cl-warn    { background: rgba(245,158,11,0.1);  color: #f59e0b; }
.cl-err     { background: rgba(244,63,94,0.1);   color: #f43f5e; }
.pulse-dot  { width: 6px; height: 6px; background: currentColor; border-radius: 50%; animation: pulse 2s infinite; }
@keyframes pulse { 0%,100% { opacity: 1; } 50% { opacity: 0.3; } }
.kpi-bento-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; }
.bento-item { background: rgba(255,255,255,0.05); padding: 18px; border-radius: 18px; text-align: center; }
.bento-item.highlight { background: rgba(245,158,11,0.15); }
.bento-item .v { font-size: 1.8rem; font-weight: 800; display: block; line-height: 1; }
.bento-item .l { font-size: 0.55rem; opacity: 0.5; text-transform: uppercase; letter-spacing: 1px; margin-top: 4px; display: block; }
.readiness-checklist { background: rgba(255,255,255,0.05); border-radius: 16px; padding: 16px; }
.checklist-label { font-size: 0.55rem; font-weight: 900; opacity: 0.4; letter-spacing: 1px; }
.check-item { display: flex; align-items: center; gap: 10px; font-size: 0.75rem; padding: 6px 0; opacity: 0.4; transition: 0.3s; }
.check-item.passed { opacity: 1; }
.wizard-controls-v8 { border-top: 1px solid rgba(255,255,255,0.08); }
.btn-step-nav { padding: 12px 18px; border-radius: 16px; font-weight: 800; border: 1.5px solid rgba(255,255,255,0.1); background: rgba(255,255,255,0.06); color: white; cursor: pointer; font-family: inherit; transition: 0.2s; }
.btn-step-nav:disabled { opacity: 0.3; cursor: not-allowed; }
.btn-step-nav.next { background: #f59e0b; border-color: #f59e0b; color: #0f172a; }
.btn-step-deploy { padding: 12px; border-radius: 16px; font-weight: 800; background: #10b981; border: none; color: white; cursor: pointer; font-family: inherit; }
.ia-coach-terminal { background: #0f172a; border-radius: 24px; padding: 24px; display: flex; gap: 16px; align-items: flex-start; }
.robot-glow-container { width: 44px; height: 44px; border-radius: 14px; background: rgba(245,158,11,0.2); display: flex; align-items: center; justify-content: center; font-size: 1.1rem; flex-shrink: 0; }
.coach-text-v8 h6 { color: white; font-weight: 800; margin-bottom: 4px; font-size: 0.85rem; }
.coach-text-v8 p { color: #94a3b8; font-size: 0.75rem; }
.activity-feed-widget { background: white; border-radius: 24px; padding: 20px; border: 1px solid #eef2f6; }
.feed-header { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; margin-bottom: 12px; }
.feed-item { display: flex; align-items: center; gap: 10px; padding: 6px 0; border-bottom: 1px solid #f8fafc; }
.feed-dot { width: 6px; height: 6px; border-radius: 50%; flex-shrink: 0; }
.feed-text { flex: 1; font-size: 0.72rem; font-weight: 600; }
.feed-time { font-size: 0.6rem; color: #94a3b8; }

/* BUTTONS */
.btn-enigma-primary { background: #0f172a; color: white; border: none; padding: 14px 28px; border-radius: 18px; font-weight: 800; position: relative; overflow: hidden; cursor: pointer; font-family: inherit; }
.btn-enigma-primary .btn-glow { position: absolute; inset: 0; background: linear-gradient(135deg, #f59e0b, #fbbf24); opacity: 0; transition: 0.3s; z-index: 1; }
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }
.btn-outline-pro { background: white; color: #0f172a; border: 1.5px solid #eef2f6; padding: 10px 18px; border-radius: 14px; font-weight: 800; font-size: 0.8rem; cursor: pointer; transition: 0.2s; font-family: inherit; }
.btn-outline-pro:hover { border-color: #0f172a; }
.btn-icon-sm { width: 32px; height: 32px; border-radius: 10px; border: 1.5px solid #eef2f6; background: white; color: #64748b; cursor: pointer; transition: 0.2s; font-size: 0.75rem; display: flex; align-items: center; justify-content: center; }
.btn-icon-sm:hover { background: #f8fafc; color: #0f172a; }
.btn-icon-sm.danger:hover { background: #fff1f2; color: #f43f5e; border-color: #f43f5e; }

/* VAULT MODAL */
.quantum-vault-overlay { position: fixed; inset: 0; background: rgba(15,23,42,0.6); backdrop-filter: blur(10px); z-index: 2000; display: flex; align-items: center; justify-content: center; }
.quantum-vault-window { width: 90vw; height: 85vh; background: white; border-radius: 40px; display: flex; flex-direction: column; overflow: hidden; box-shadow: 0 40px 100px rgba(0,0,0,0.2); }
.qv-header { padding: 28px 40px; border-bottom: 1px solid #eef2f6; display: flex; justify-content: space-between; align-items: center; flex-shrink: 0; }
.qv-filter-tabs { background: #f8fafc; flex-shrink: 0; }
.qv-filter-btn { padding: 6px 14px; border-radius: 10px; border: 1.5px solid transparent; background: transparent; font-size: 0.7rem; font-weight: 800; cursor: pointer; font-family: inherit; }
.qv-filter-btn.active { background: #0f172a; color: white; }
.qv-layout { display: flex; flex-grow: 1; overflow: hidden; }
.qv-sidebar { width: 240px; padding: 24px; border-right: 1px solid #eef2f6; display: flex; flex-direction: column; overflow-y: auto; flex-shrink: 0; }
.qv-cat-btn { padding: 6px 12px; border-radius: 10px; border: none; background: transparent; font-size: 0.75rem; font-weight: 700; cursor: pointer; text-align: left; color: #64748b; font-family: inherit; transition: 0.2s; }
.qv-cat-btn.active { background: #fffbeb; color: #f59e0b; font-weight: 800; }
.qv-bank-stats { margin-top: auto; }
.qv-list { flex-grow: 1; padding: 24px; background: #f8fafc; overflow-y: auto; display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 14px; align-content: start; }
.qv-item-card { background: white; padding: 18px; border-radius: 18px; cursor: pointer; border: 2px solid transparent; transition: 0.2s; }
.qv-item-card:hover { border-color: #e2e8f0; }
.qv-item-card.active { border-color: #f59e0b; }
.qv-item-card.checked { background: #f0fdf4; border-color: #10b981; }
.qv-item-top { display: flex; justify-content: space-between; align-items: center; }
.qv-badge { font-size: 0.6rem; font-weight: 900; padding: 3px 10px; border-radius: 8px; text-transform: uppercase; }
.qv-badge.expert { background: #fff1f2; color: #f43f5e; } .qv-badge.medium { background: #fffbeb; color: #f59e0b; } .qv-badge.easy { background: #ecfdf5; color: #10b981; }
.qv-checkbox { color: #94a3b8; cursor: pointer; transition: color 0.2s; font-size: 1.1rem; }
.qv-item-card.checked .qv-checkbox { color: #10b981; }
.qv-item-text { font-size: 0.82rem; font-weight: 700; color: #0f172a; line-height: 1.4; }
.qv-item-footer { font-size: 0.75rem; color: #94a3b8; }
.qv-inspector { width: 280px; border-left: 1px solid #eef2f6; overflow-y: auto; display: flex; flex-direction: column; flex-shrink: 0; }
.device-mockup-v8 { background: #f8fafc; border-radius: 20px; }
.btn-qv-cancel  { background: #f1f5f9; color: #64748b; border: none; padding: 12px 24px; border-radius: 14px; font-weight: 800; cursor: pointer; font-family: inherit; }
.btn-qv-confirm { background: #0f172a; color: white; border: none; padding: 12px 24px; border-radius: 14px; font-weight: 800; cursor: pointer; font-family: inherit; }
.btn-qv-confirm:disabled { opacity: 0.4; cursor: not-allowed; }
.quick-add-modal { background: white; border-radius: 32px; padding: 40px; width: 550px; max-width: 95vw; box-shadow: 0 40px 80px rgba(0,0,0,0.15); }
.shortcuts-modal { background: white; border-radius: 32px; padding: 40px; width: 480px; max-width: 95vw; box-shadow: 0 40px 80px rgba(0,0,0,0.15); }
.shortcuts-grid { display: flex; flex-direction: column; gap: 10px; }
.shortcut-item { display: flex; align-items: center; justify-content: space-between; padding: 12px 16px; background: #f8fafc; border-radius: 14px; }
kbd { background: #0f172a; color: white; padding: 4px 10px; border-radius: 8px; font-size: 0.7rem; font-weight: 800; font-family: monospace; }
.confirm-modal { background: white; border-radius: 32px; padding: 40px; width: 420px; max-width: 95vw; text-align: center; box-shadow: 0 40px 80px rgba(0,0,0,0.15); }
.btn-confirm-danger { background: #f43f5e; color: white; border: none; padding: 12px 24px; border-radius: 14px; font-weight: 800; cursor: pointer; font-family: inherit; }

/* TOAST */
.enigma-toast { position: fixed; bottom: 30px; right: 30px; background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px; display: flex; align-items: center; gap: 15px; z-index: 3000; border-left: 5px solid #f59e0b; box-shadow: 0 20px 40px rgba(0,0,0,0.2); }
.t-success { border-left-color: #10b981; } .t-error { border-left-color: #f43f5e; } .t-warn { border-left-color: #f59e0b; }
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }

/* TRANSITIONS */
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.92); } to { opacity: 1; transform: scale(1); } }

/* SPINNER */
.spinner-pro-premium { width: 50px; height: 50px; border: 4px solid #f1f5f9; border-top: 4px solid #f59e0b; border-radius: 50%; animation: spin 1s linear infinite; margin: 40px auto; }
@keyframes spin { to { transform: rotate(360deg); } }

/* SCROLLBAR */


/* MISC */
.text-amber   { color: #f59e0b !important; }
.text-indigo  { color: #6366f1 !important; }
.text-success { color: #10b981 !important; }
.text-danger  { color: #f43f5e !important; }
.fw-800 { font-weight: 800 !important; }
.fw-900 { font-weight: 900 !important; }
</style>