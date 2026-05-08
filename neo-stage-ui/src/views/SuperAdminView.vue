<template>
  <div class="enigma-master-root d-flex overflow-hidden" :data-theme="isDark ? 'dark' : 'light'" @mousemove="handleParallax">

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

      <!-- THEME TOGGLE -->
      <button class="theme-toggle-btn" @click="isDark = !isDark" :title="isDark ? 'Mode Clair' : 'Mode Sombre'">
        <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'"></i>
      </button>

      <!-- LOADER -->
      <transition name="fade">
        <div v-if="isLoading" class="loader-overlay">
          <div class="tech-loader-container">
            <div class="spinner-pro-premium"></div>
            <div class="loader-text">SYNCHRONISATION NEURALE...</div>
            <div class="loader-sub">Chargement du Master Control Panel</div>
          </div>
        </div>
      </transition>

      <main v-if="!isLoading" class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">

        <!-- ═══════════════════════════════════════════════════════
             DASHBOARD VIEW
        ═══════════════════════════════════════════════════════ -->
        <div class="dashboard-view animate__animated animate__fadeIn p-4 p-lg-5">

          <!-- HEADER -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Master Control Panel</span>
              </div>
              <h2 class="premium-title">Master <span class="gradient-text">Control</span></h2>
              <p class="page-sub">Supervision en temps réel · <strong>{{ today }}</strong></p>
            </div>
            <div class="d-flex gap-3 align-items-center flex-wrap">
              <div class="system-status-pro">
                <span class="status-dot-pro pulse"></span>
                <span class="status-text-pro">SERVEURS IA : OPTIMUM</span>
              </div>
              <button class="btn-refresh-pro" @click="fetchData" :disabled="isRefreshing" title="Rafraîchir">
                <i class="fa-solid fa-rotate" :class="{ 'fa-spin': isRefreshing }"></i>
              </button>
              <button @click="showOrgModal = true" class="btn-enigma-primary shadow-premium">
                <div class="btn-content"><i class="fa-solid fa-plus-circle me-2"></i> ENREGISTRER UNE ENTREPRISE</div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </header>

          <!-- KPI CARDS -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in masterStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details">
                  <div class="stat-value">{{ stat.val }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
                <div class="stat-trend ms-auto" :class="stat.trendUp ? 'trend-up' : 'trend-down'">
                  <i :class="stat.trendUp ? 'fa-solid fa-arrow-trend-up' : 'fa-solid fa-arrow-trend-down'"></i>
                  <span>{{ stat.trend }}</span>
                </div>
              </div>
            </div>
          </div>

          <!-- FILTER BAR -->
          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
            <div class="tabs-container">
              <div class="d-flex gap-2 p-1 bg-white rounded-4 shadow-sm border">
                <button class="nav-tab-btn-modern" :class="{ active: statusFilter === '' }"  @click="statusFilter = ''">
                  Toutes <span class="tab-count">{{ orgs.length }}</span>
                </button>
                <button class="nav-tab-btn-modern" :class="{ active: statusFilter === '1' }" @click="statusFilter = '1'">
                  Actives <span class="tab-count">{{ orgs.filter(o=>o.estActif).length }}</span>
                </button>
                <button class="nav-tab-btn-modern" :class="{ active: statusFilter === '0' }" @click="statusFilter = '0'">
                  Inactives <span class="tab-count">{{ orgs.filter(o=>!o.estActif).length }}</span>
                </button>
              </div>
            </div>
            <div class="d-flex gap-2">
              <div class="search-inline-box">
                <i class="fa-solid fa-magnifying-glass"></i>
                <input type="text" v-model="searchQuery" placeholder="Rechercher entreprise, email..." class="search-inline-input">
                <button v-if="searchQuery" @click="searchQuery = ''" class="btn-clear-search"><i class="fa-solid fa-xmark"></i></button>
              </div>
              <select v-model="periodFilter" class="sort-select-pro">
                <option value="30">30 jours</option>
                <option value="7">7 jours</option>
                <option value="90">90 jours</option>
              </select>
            </div>
          </div>

          <!-- MAIN GRID -->
          <div class="row g-4 mb-5">

            <!-- DEMANDES TABLE -->
            <div class="col-lg-8">
              <div class="enigma-card p-0 overflow-hidden h-100">
                <div class="card-header-section d-flex justify-content-between align-items-center p-4">
                  <div class="d-flex align-items-center gap-3">
                    <div class="icon-box-v2 amber"><i class="fa-solid fa-id-card-clip"></i></div>
                    <div>
                      <h6 class="fw-800 m-0" style="font-size:0.85rem">Demandes d'adhésion</h6>
                      <p class="m-0 text-muted" style="font-size:0.7rem">Dossiers en attente de validation</p>
                    </div>
                  </div>
                  <span class="slot-badge-amber">{{ filteredPendingRequests.length }} dossiers</span>
                </div>
                <div class="table-responsive">
                  <table class="table-enigma m-0">
                    <thead>
                      <tr>
                        <th class="ps-4">Entreprise</th>
                        <th>Contact</th>
                        <th>Date</th>
                        <th class="text-end pe-4">Actions</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(req, idx) in filteredPendingRequests" :key="req.id"
                        class="table-row-enigma" :style="{ animationDelay: idx * 0.05 + 's' }">
                        <td class="ps-4">
                          <div class="d-flex align-items-center gap-3">
                            <div class="avatar-v8">{{ req.nomEntreprise?.[0] || '?' }}</div>
                            <div>
                              <div class="fw-800" style="font-size:0.85rem">{{ req.nomEntreprise }}</div>
                              <div class="text-muted" style="font-size:0.7rem">Matricule: {{ req.matriculeFiscale || '—' }}</div>
                            </div>
                          </div>
                        </td>
                        <td>
                          <div class="fw-700" style="font-size:0.85rem">{{ req.nomResponsable }}</div>
                          <div style="font-size:0.7rem;color:#6366f1;font-weight:700">{{ req.emailResponsable }}</div>
                        </td>
                        <td><span class="date-chip-pro">{{ formatDate(req.creeLe) }}</span></td>
                        <td class="text-end pe-4">
                          <div class="d-flex justify-content-end gap-2">
                            <button @click="handleApprove(req.id)" class="btn-action-pro approve" title="Approuver">
                              <i class="fa-solid fa-check"></i>
                            </button>
                            <button @click="handleReject(req.id)" class="btn-action-pro reject" title="Refuser">
                              <i class="fa-solid fa-xmark"></i>
                            </button>
                          </div>
                        </td>
                      </tr>
                      <tr v-if="filteredPendingRequests.length === 0">
                        <td colspan="4">
                          <div class="empty-state-pro py-5 text-center">
                            <i class="fa-solid fa-inbox fa-2x text-muted mb-3 d-block"></i>
                            <h6 class="fw-800 text-muted">Aucun dossier en attente</h6>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <!-- RIGHT COLUMN -->
            <div class="col-lg-4">
              <div class="d-flex flex-column gap-4 h-100">

                <!-- IA PERFORMANCE -->
                <div class="enigma-card p-4">
                  <div class="pane-header-v2 mb-4">
                    <div class="icon-box-v2" style="background:#eef2ff;color:#6366f1"><i class="fa-solid fa-microchip"></i></div>
                    <div><h6 class="fw-800 m-0" style="font-size:0.85rem">Performance IA</h6></div>
                  </div>
                  <div class="ia-donut-wrapper mb-3">
                    <svg viewBox="0 0 120 120" width="120">
                      <circle cx="60" cy="60" r="45" fill="none" stroke="#eef2f6" stroke-width="10"/>
                      <circle cx="60" cy="60" r="45" fill="none"
                        stroke="url(#iaGrad)" stroke-width="10" stroke-linecap="round"
                        :stroke-dasharray="`${iaPerformance.charge * 2.83} 283`"
                        stroke-dashoffset="70.75"
                        style="transition:stroke-dasharray 1s ease"/>
                      <defs>
                        <linearGradient id="iaGrad" x1="0%" y1="0%" x2="100%" y2="0%">
                          <stop offset="0%" stop-color="#f59e0b"/>
                          <stop offset="100%" stop-color="#fbbf24"/>
                        </linearGradient>
                      </defs>
                      <text x="60" y="57" text-anchor="middle" class="donut-center-text">{{ iaPerformance.charge }}%</text>
                      <text x="60" y="70" text-anchor="middle" class="donut-sub-text">CHARGE</text>
                    </svg>
                    <div class="ia-metrics-list">
                      <div class="ia-metric-item">
                        <span class="legend-dot dot-amber"></span>
                        <span class="small text-muted fw-700">Tokens LLM</span>
                        <span class="ms-auto fw-800 small">{{ iaPerformance.tokens }}</span>
                      </div>
                      <div class="ia-metric-item">
                        <span class="legend-dot dot-green"></span>
                        <span class="small text-muted fw-700">Réponse</span>
                        <span class="ms-auto fw-800 small">{{ iaPerformance.responseTime }}</span>
                      </div>
                      <div class="ia-metric-item">
                        <span class="legend-dot dot-indigo"></span>
                        <span class="small text-muted fw-700">Req/s</span>
                        <span class="ms-auto fw-800 small">{{ iaPerformance.requestsPerSecond }}/s</span>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- MODULE USAGE -->
                <div class="enigma-card p-4 flex-grow-1">
                  <div class="pane-header-v2 mb-4">
                    <div class="icon-box-v2" style="background:#fff1f2;color:#f43f5e"><i class="fa-solid fa-chart-bar"></i></div>
                    <div><h6 class="fw-800 m-0" style="font-size:0.85rem">Utilisation par module</h6></div>
                  </div>
                  <div v-for="(mod, idx) in moduleUsage" :key="mod.name" class="mb-3">
                    <div class="d-flex justify-content-between align-items-center mb-1">
                      <span class="small fw-700">{{ mod.name }}</span>
                      <span class="small fw-800">{{ mod.pct }}%</span>
                    </div>
                    <div class="progress-slim">
                      <div class="progress-fill" :style="{ width: mod.pct + '%', background: mod.color }"></div>
                    </div>
                  </div>
                </div>

              </div>
            </div>
          </div>

          <!-- ═══════════════════════════════════════════════════════
               REGISTRE DES ENTREPRISES — LIST VIEW STYLE
          ═══════════════════════════════════════════════════════ -->
          <div class="enigma-card p-0 overflow-hidden mb-5">

            <!-- Header -->
            <div class="card-header-section d-flex justify-content-between align-items-center p-4">
              <div class="d-flex align-items-center gap-3">
                <div class="icon-box-v2 amber"><i class="fa-solid fa-building"></i></div>
                <div>
                  <h6 class="fw-800 m-0" style="font-size:0.85rem">Registre des Entreprises</h6>
                  <p class="m-0 text-muted" style="font-size:0.7rem">
                    Vue recyclée · {{ filteredOrgs.length }} entrées · Scroll infini
                  </p>
                </div>
              </div>
              <div class="d-flex gap-2 align-items-center">
                <span class="recycle-badge-pro">RECYCLE VIEW</span>
                <div class="search-inline-box" style="max-width:200px">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  <input v-model="orgSearch" type="text" placeholder="Filtrer..." class="search-inline-input">
                </div>
              </div>
            </div>

            <!-- Column headers -->
            <div class="list-header-row d-flex align-items-center px-4 py-2">
              <span style="width:36px" class="list-col-label">#</span>
              <span style="width:48px"></span>
              <span class="flex-grow-1 list-col-label">ENTREPRISE / EMAIL</span>
              <span style="width:130px" class="list-col-label d-none d-lg-block">VILLE / INDUSTRIE</span>
              <span style="width:100px" class="list-col-label text-center">STATUT</span>
              <span style="width:90px"  class="list-col-label text-center">SCORE</span>
              <span style="width:100px" class="list-col-label text-end pe-2">ACTIONS</span>
            </div>

            <!-- Loader -->
            <div v-if="orgsLoading" class="text-center py-5">
              <div class="spinner-pro-premium"></div>
            </div>

            <!-- Recycle viewport -->
            <div v-else class="recycle-viewport-pro" ref="recycleViewport" @scroll="onRecycleScroll">
              <div :style="{ height: paddingTop + 'px' }"></div>

              <div v-for="(org, idx) in visibleOrgs" :key="org.id"
                class="list-row-item d-flex align-items-center px-4"
                :style="{ height: ROW_HEIGHT + 'px', animationDelay: (idx % 10) * 0.03 + 's' }">

                <div style="width:36px;flex-shrink:0">
                  <span class="rank-label">#{{ org._rank }}</span>
                </div>

                <div style="width:48px;flex-shrink:0">
                  <div class="avatar-v8" :style="{ background: org._color + '22', color: org._color }">
                    {{ org.nom?.[0] || '?' }}
                  </div>
                </div>

                <div class="flex-grow-1" style="min-width:0">
                  <div class="fw-800 text-truncate" style="font-size:0.85rem">{{ org.nom }}</div>
                  <div class="text-muted text-truncate" style="font-size:0.7rem">{{ org.email || org.emailAdmin || '—' }}</div>
                </div>

                <div class="d-none d-lg-flex" style="width:130px;flex-shrink:0;gap:6px;flex-wrap:wrap">
                  <span class="t-pill cat-pill">{{ org.ville || org.city || 'N/A' }}</span>
                  <span class="t-pill type-pill">{{ org.industrie || org.industry || 'Tech' }}</span>
                </div>

                <div style="width:100px;flex-shrink:0;text-align:center">
                  <span class="status-badge" :class="org.estActif ? 'status-1' : 'status-2'">
                    <span class="status-dot"></span>
                    {{ org.estActif ? 'Actif' : 'Inactif' }}
                  </span>
                </div>

                <div style="width:90px;flex-shrink:0;text-align:center">
                  <div class="score-ring-pro" :style="{ '--pct': org._score, '--col': org._color }">
                    <span class="score-ring-val">{{ org._score }}%</span>
                  </div>
                </div>

                <div style="width:100px;flex-shrink:0" class="d-flex justify-content-end gap-1 pe-2">
                  <button class="btn-icon-sm" title="Voir" @click="viewOrgDetails(org)">
                    <i class="fa-solid fa-eye"></i>
                  </button>
                  <button class="btn-icon-sm" title="Éditer" @click="editOrg(org)">
                    <i class="fa-solid fa-pen-to-square"></i>
                  </button>
                  <button class="btn-icon-sm danger" title="Supprimer" @click="deleteOrg(org.id)">
                    <i class="fa-solid fa-trash-can"></i>
                  </button>
                </div>
              </div>

              <div :style="{ height: paddingBottom + 'px' }"></div>

              <div v-if="isLoadingMore" class="text-center py-4">
                <div class="spinner-pro-premium" style="width:32px;height:32px;border-width:3px;margin:0 auto 8px"></div>
                <span class="small text-muted fw-700">Chargement...</span>
              </div>

              <div v-if="!isLoadingMore && filteredOrgs.length > 0 && scrolledToEnd" class="text-center py-3">
                <span class="small fw-700 text-muted">
                  <i class="fa-solid fa-check-circle text-success me-2"></i>
                  {{ filteredOrgs.length }} entreprises · Fin de liste
                </span>
              </div>

              <div v-if="!orgsLoading && filteredOrgs.length === 0" class="empty-state-pro py-5 text-center mx-4 my-3">
                <i class="fa-solid fa-inbox fa-2x text-muted mb-3 d-block"></i>
                <h6 class="fw-800 text-muted">Aucune entreprise trouvée</h6>
              </div>
            </div>

            <!-- Footer -->
            <div class="d-flex align-items-center justify-content-between p-3 border-top" style="background:#f8fafc">
              <div class="d-flex gap-4">
                <div class="recycle-stat-pro"><span class="rv">{{ filteredOrgs.length }}</span><span class="rl">Total</span></div>
                <div class="recycle-stat-pro"><span class="rv text-success">{{ orgs.filter(o=>o.estActif).length }}</span><span class="rl">Actives</span></div>
                <div class="recycle-stat-pro"><span class="rv text-danger">{{ orgs.filter(o=>!o.estActif).length }}</span><span class="rl">Inactives</span></div>
              </div>
              <span class="small text-muted fw-700">
                <i class="fa-solid fa-recycle text-amber me-2"></i>
                VIRTUAL SCROLL · {{ visibleOrgs.length }} rows rendered
              </span>
            </div>
          </div>

          <!-- TERMINAL LOGS -->
          <div class="analytics-hub-glass p-0 overflow-hidden position-relative">
            <div class="scanner-line"></div>
            <div class="terminal-titlebar px-4 py-3">
              <div class="d-flex gap-1 me-3">
                <div class="terminal-dot red"></div>
                <div class="terminal-dot amber-dot"></div>
                <div class="terminal-dot green-dot"></div>
              </div>
              <span class="small fw-800" style="color:rgba(255,255,255,0.3);letter-spacing:1px;font-size:0.65rem">
                MASTER_CONTROL // ADMIN_LOGS_REALTIME — v3.1.0
              </span>
              <div class="ms-auto d-flex align-items-center gap-2">
                <div style="width:6px;height:6px;background:#22c55e;border-radius:50%;animation:pulse 2s infinite"></div>
                <span style="font-size:0.6rem;font-weight:800;color:#22c55e;letter-spacing:1px">ONLINE</span>
              </div>
            </div>
            <div class="p-4">
              <div v-for="(line, i) in terminalLogs" :key="i" class="t-line-anim" :style="{ animationDelay: i * 0.05 + 's' }">
                <span class="t-time">{{ line.time }}</span>
                <span class="t-prompt" :class="line.type">›</span>
                <span class="t-text" v-html="line.text"></span>
              </div>
              <div class="d-flex align-items-center gap-2 mt-1">
                <span class="t-prompt green">›</span>
                <span style="font-family:'JetBrains Mono',monospace;font-size:0.85rem;color:#4f46e5;animation:blink-cursor 1s step-end infinite">█</span>
              </div>
            </div>
          </div>

        </div><!-- end dashboard-view -->
      </main>
    </div>

    <!-- ═══════════════════════════════════════════════════════
         MODALE : CRÉER ORGANISATION
    ═══════════════════════════════════════════════════════ -->
    <transition name="modal-quantum">
      <div v-if="showOrgModal" class="quantum-vault-overlay" @click.self="showOrgModal = false">
        <div class="quick-add-modal animate__animated animate__zoomIn animate__faster" style="max-width:840px;width:95%">

          <div class="d-flex justify-content-between align-items-center mb-5">
            <div class="d-flex align-items-center gap-3">
              <div class="icon-box-v2 amber"><i class="fa-solid fa-building"></i></div>
              <div>
                <h5 class="fw-900 m-0">Créer une organisation</h5>
                <p class="m-0 text-muted small">Enregistrement d'une nouvelle entreprise cliente</p>
              </div>
            </div>
            <button @click="showOrgModal = false" class="btn-icon-sm"><i class="fa-solid fa-xmark"></i></button>
          </div>

          <form @submit.prevent="handleCreateOrg">
            <!-- SECTION ORG -->
            <div class="modal-section-label mb-4">
              <i class="fa-solid fa-briefcase text-amber me-2"></i>
              Informations sur l'organisation
            </div>
            <div class="modal-alert-info mb-4">
              <i class="fa-solid fa-circle-info me-2"></i>
              Un mot de passe sécurisé sera automatiquement généré et envoyé par e-mail.
            </div>
            <div class="row g-4 mb-4">
              <div class="col-12">
                <div class="enigma-input-wrap">
                  <label>NOM DE L'ORGANISATION <span class="required-star">*</span></label>
                  <input type="text" v-model="newOrg.name" class="enigma-field" placeholder="Entrez le nom de l'organisation" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>DOMAINE</label>
                  <input type="text" v-model="newOrg.domain" class="enigma-field" placeholder="entreprise.com">
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>INDUSTRIE</label>
                  <select v-model="newOrg.industry" class="enigma-field">
                    <option value="" disabled selected>Sélectionnez...</option>
                    <option>Technologie</option>
                    <option>Finance</option>
                    <option>Santé</option>
                    <option>Éducation</option>
                    <option>Commerce</option>
                  </select>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>SITE WEB</label>
                  <input type="url" v-model="newOrg.website" class="enigma-field" placeholder="https://example.com">
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>VILLE</label>
                  <input type="text" v-model="newOrg.city" class="enigma-field" placeholder="Tunis, Paris...">
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>PAYS</label>
                  <input type="text" v-model="newOrg.country" class="enigma-field" placeholder="Tunisie, France...">
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>CODE POSTAL</label>
                  <input type="text" v-model="newOrg.zipCode" class="enigma-field" placeholder="1000">
                </div>
              </div>
              <div class="col-12">
                <div class="enigma-input-wrap">
                  <label>ADRESSE</label>
                  <input type="text" v-model="newOrg.address" class="enigma-field" placeholder="Rue, Numéro...">
                </div>
              </div>
              <div class="col-12">
                <div class="enigma-input-wrap">
                  <label>DESCRIPTION</label>
                  <textarea v-model="newOrg.description" class="enigma-field" rows="2" placeholder="Brève description..."></textarea>
                </div>
              </div>
            </div>

            <!-- SECTION ADMIN -->
            <div class="modal-section-label mb-4">
              <i class="fa-solid fa-circle-user me-2" style="color:#6366f1"></i>
              Informations sur l'administrateur
            </div>
            <div class="row g-4 mb-4">
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>PRÉNOM <span class="required-star">*</span></label>
                  <input type="text" v-model="newOrg.adminFirstName" class="enigma-field" placeholder="Prénom" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>NOM <span class="required-star">*</span></label>
                  <input type="text" v-model="newOrg.adminLastName" class="enigma-field" placeholder="Nom" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>EMAIL ADMIN <span class="required-star">*</span></label>
                  <input type="email" v-model="newOrg.adminEmail" class="enigma-field" placeholder="admin@entreprise.com" required>
                </div>
              </div>
              <div class="col-md-6">
                <div class="enigma-input-wrap">
                  <label>TÉLÉPHONE</label>
                  <div class="d-flex gap-2">
                    <select class="enigma-field" style="width:auto;min-width:100px">
                      <option>🇹🇳 +216</option>
                      <option>🇫🇷 +33</option>
                      <option>🇺🇸 +1</option>
                    </select>
                    <input type="tel" v-model="newOrg.adminPhone" class="enigma-field" placeholder="55 123 456">
                  </div>
                </div>
              </div>
            </div>

            <div class="d-flex justify-content-end gap-3 pt-3 border-top">
              <button type="button" @click="showOrgModal = false" class="btn-qv-cancel">ANNULER</button>
              <button type="submit" class="btn-enigma-primary" :disabled="isCreating">
                <div class="btn-content">
                  <span v-if="isCreating" class="spinner-border spinner-border-sm me-2"></span>
                  <i v-else class="fa-solid fa-check me-2"></i>
                  CRÉER L'ORGANISATION
                </div>
                <div class="btn-glow"></div>
              </button>
            </div>
          </form>
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
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar  from '@/components/AppNavbar.vue';
import { superAdminApi } from '@/services/api';
import axios from 'axios';

// ─── DARK MODE ─────────────────────────────────────────────────
const isDark = ref(false);

// ─── DATE ──────────────────────────────────────────────────────
const today = new Date().toLocaleDateString('fr-FR', {
  weekday: 'long', day: '2-digit', month: 'long', year: 'numeric'
});

// ─── ÉTAT ──────────────────────────────────────────────────────
const isLoading    = ref(true);
const isRefreshing = ref(false);
const orgsLoading  = ref(false);
const isCreating   = ref(false);
const showOrgModal = ref(false);
const searchQuery  = ref('');
const statusFilter = ref('');
const periodFilter = ref('30');
const orgSearch    = ref('');
const mousePos     = reactive({ x: 0, y: 0 });

// ─── IA PERFORMANCE ────────────────────────────────────────────
const iaPerformance = reactive({
  charge: 72, tokens: '1.2M', responseTime: '340ms', requestsPerSecond: 18
});
const moduleUsage = ref([
  { name: 'Analyse IA',        pct: 87, color: '#4f46e5' },
  { name: 'Questionnaires',    pct: 73, color: '#f59e0b' },
  { name: 'Notifications',     pct: 61, color: '#10b981' },
  { name: 'Rapports Export',   pct: 45, color: '#f43f5e' },
  { name: 'Anti-Cheat Engine', pct: 92, color: '#06b6d4' },
]);

// ─── STATS ─────────────────────────────────────────────────────
const masterStats = ref([
  { label: 'Entreprises',  val: '—', icon: 'fa-solid fa-building',            bg: '#eef2ff', color: '#4f46e5', trend: '+12%', trendUp: true  },
  { label: 'Sessions IA',  val: '—', icon: 'fa-solid fa-wand-magic-sparkles', bg: '#fff7ed', color: '#f97316', trend: '+24%', trendUp: true  },
  { label: 'Utilisateurs', val: '—', icon: 'fa-solid fa-users',               bg: '#ecfdf5', color: '#10b981', trend: '+5%',  trendUp: true  },
  { label: 'En attente',   val: '—', icon: 'fa-solid fa-shield-virus',        bg: '#fef2f2', color: '#ef4444', trend: '0%',   trendUp: false },
]);

// ─── DEMANDES ──────────────────────────────────────────────────
const pendingRequests = ref([]);
const filteredPendingRequests = computed(() => {
  const q = searchQuery.value.toLowerCase().trim();
  if (!q) return pendingRequests.value;
  return pendingRequests.value.filter(r =>
    r.nomEntreprise?.toLowerCase().includes(q) ||
    r.emailResponsable?.toLowerCase().includes(q) ||
    r.nomResponsable?.toLowerCase().includes(q) ||
    r.matriculeFiscale?.toLowerCase().includes(q)
  );
});

// ─── ORGANISATIONS ─────────────────────────────────────────────
const orgs = ref([]);
const ORG_COLORS = ['#f59e0b','#4f46e5','#10b981','#f43f5e','#06b6d4','#a855f7','#ec4899'];

const filteredOrgs = computed(() => {
  let list = orgs.value;
  const q  = orgSearch.value.toLowerCase();
  if (q) list = list.filter(o =>
    o.nom?.toLowerCase().includes(q) ||
    o.email?.toLowerCase().includes(q) ||
    o.industrie?.toLowerCase().includes(q) ||
    o.emailAdmin?.toLowerCase().includes(q)
  );
  if (statusFilter.value !== '')
    list = list.filter(o => String(Number(o.estActif)) === statusFilter.value);
  return list.map((o, i) => ({
    ...o,
    _rank:  i + 1,
    _color: ORG_COLORS[i % ORG_COLORS.length],
    _score: o._score || Math.floor(Math.random() * 30 + 65),
  }));
});

// ─── VIRTUAL SCROLL ────────────────────────────────────────────
const ROW_HEIGHT      = 68;
const BUFFER          = 5;
const recycleViewport = ref(null);
const scrollTop_      = ref(0);
const viewportHeight_ = ref(440);
const isLoadingMore   = ref(false);
const scrolledToEnd   = ref(false);
let   _loadedPages    = 1;

const startIdx_    = computed(() => Math.max(0, Math.floor(scrollTop_.value / ROW_HEIGHT) - BUFFER));
const endIdx_      = computed(() => Math.min(filteredOrgs.value.length, Math.ceil((scrollTop_.value + viewportHeight_.value) / ROW_HEIGHT) + BUFFER));
const visibleOrgs  = computed(() => filteredOrgs.value.slice(startIdx_.value, endIdx_.value));
const paddingTop   = computed(() => startIdx_.value * ROW_HEIGHT);
const paddingBottom= computed(() => Math.max(0, (filteredOrgs.value.length - endIdx_.value) * ROW_HEIGHT));

const onRecycleScroll = (e) => {
  scrollTop_.value = e.target.scrollTop;
  const { scrollHeight, scrollTop: st, clientHeight } = e.target;
  if (scrollHeight - st - clientHeight < 80 && !isLoadingMore.value && !scrolledToEnd.value) {
    loadMoreOrgs();
  }
};
const loadMoreOrgs = async () => {
  if (isLoadingMore.value || scrolledToEnd.value) return;
  isLoadingMore.value = true;
  try {
    _loadedPages++;
    const res     = await superAdminApi.getOrganizations({ page: _loadedPages, limit: 20 });
    const newData = res.data?.data || res.data || [];
    if (newData.length === 0) scrolledToEnd.value = true;
    else orgs.value.push(...newData);
  } catch { scrolledToEnd.value = true; }
  finally  { isLoadingMore.value = false; }
};

// ─── TERMINAL ──────────────────────────────────────────────────
const terminalLogs = ref([
  { time: '00:00:01', type: 'green', text: 'Master Control Panel initialisé <span class="t-ok">[ OK ]</span>' },
  { time: '00:00:03', type: 'blue',  text: 'Connexion API SuperAdmin <span class="t-hi">établie</span>' },
  { time: '00:00:05', type: 'amber', text: 'Chargement des organisations depuis l\'API...' },
  { time: '00:00:07', type: 'green', text: 'RecycleView Virtual Scroll <span class="t-ok">[ READY ]</span>' },
  { time: '00:00:09', type: 'green', text: 'Dashboard opérationnel <span class="t-ok">[ DEPLOYED ]</span>' },
]);
const addTerminalLog = (type, text) => {
  const time = new Date().toLocaleTimeString('fr-FR', { hour:'2-digit', minute:'2-digit', second:'2-digit' });
  terminalLogs.value.push({ time, type, text });
  if (terminalLogs.value.length > 20) terminalLogs.value.shift();
};

// ─── FORM ──────────────────────────────────────────────────────
const newOrg = reactive({
  name:'', domain:'', industry:'', website:'',
  city:'', country:'', zipCode:'', address:'',
  description:'', adminFirstName:'', adminLastName:'',
  adminEmail:'', adminPhone:''
});

// ─── TOAST ─────────────────────────────────────────────────────
const globalToast = reactive({ active:false, message:'', type:'', icon:'' });
let _toastTimer = null;
const showPulseToast = (msg, type='success', icon='fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message:msg, type:`t-${type}`, icon, active:true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

// ─── FETCH ─────────────────────────────────────────────────────
const fetchData = async () => {
  isRefreshing.value  = true;
  orgsLoading.value   = true;
  scrolledToEnd.value = false;
  _loadedPages        = 1;
  try {
    try {
      const r = await superAdminApi.getStats();
      const d = r.data;
      masterStats.value[0].val = d.totalEntreprises   ?? '—';
      masterStats.value[1].val = d.sessionsIARecentes  ?? '—';
      masterStats.value[2].val = d.totalUtilisateurs   ?? '—';
      masterStats.value[3].val = d.demandesEnAttente    ?? '—';
    } catch {}

    try {
      const r = await superAdminApi.getPendingRequests();
      pendingRequests.value    = r.data || [];
      masterStats.value[3].val = pendingRequests.value.length;
    } catch { pendingRequests.value = []; }

    try {
      const r  = await superAdminApi.getOrganizations({ page:1, limit:20 });
      orgs.value = r.data?.data || r.data || [];
      if (orgs.value.length < 20) scrolledToEnd.value = true;
      addTerminalLog('green', `<span class="t-hi">${orgs.value.length}</span> organisations chargées <span class="t-ok">[ OK ]</span>`);
    } catch {
      orgs.value = [];
      addTerminalLog('amber', 'Impossible de charger les organisations depuis l\'API');
    }

    try {
      const r = await axios.post('http://127.0.0.1:8000/ia/performance-report');
      if (r.data?.performance) Object.assign(iaPerformance, r.data.performance);
      if (r.data?.usage)       moduleUsage.value = r.data.usage;
    } catch {}

    addTerminalLog('green', `Refresh complet — <span class="t-ok">${pendingRequests.value.length} dossiers</span> en attente`);
  } catch(err) {
    addTerminalLog('amber', 'Erreur lors du chargement des données');
  } finally {
    isRefreshing.value = false;
    orgsLoading.value  = false;
    setTimeout(() => { isLoading.value = false; }, 800);
  }
};

// ─── ACTIONS ───────────────────────────────────────────────────
const handleApprove = async (id) => {
  try {
    await superAdminApi.approveRequest(id);
    showPulseToast('Entreprise activée avec succès !', 'success', 'fa-solid fa-check');
    pendingRequests.value = pendingRequests.value.filter(r => r.id !== id);
    masterStats.value[3].val = Math.max(0, (masterStats.value[3].val||1) - 1);
    addTerminalLog('green', `Dossier <span class="t-hi">#${id}</span> approuvé <span class="t-ok">[ VALIDATED ]</span>`);
  } catch {
    showPulseToast("Erreur lors de l'activation.", 'error', 'fa-solid fa-triangle-exclamation');
  }
};
const handleReject = async (id) => {
  if (!confirm('Voulez-vous vraiment refuser cette demande ?')) return;
  try {
    await superAdminApi.rejectRequest(id);
    showPulseToast('Demande refusée.', 'warn', 'fa-solid fa-ban');
    pendingRequests.value = pendingRequests.value.filter(r => r.id !== id);
    addTerminalLog('amber', `Dossier <span class="t-hi">#${id}</span> refusé`);
  } catch {
    showPulseToast('Erreur.', 'error', 'fa-solid fa-triangle-exclamation');
  }
};
const handleCreateOrg = async () => {
  isCreating.value = true;
  try {
    await superAdminApi.createOrg(newOrg);
    showPulseToast(`Organisation "${newOrg.name}" créée.`, 'success', 'fa-solid fa-building');
    showOrgModal.value = false;
    Object.keys(newOrg).forEach(k => newOrg[k] = '');
    addTerminalLog('green', `Nouvelle organisation créée <span class="t-ok">[ OK ]</span>`);
    fetchData();
  } catch {
    showPulseToast('Erreur lors de la création.', 'error', 'fa-solid fa-triangle-exclamation');
  } finally { isCreating.value = false; }
};
const viewOrgDetails = (org) => {
  showPulseToast(`Détails : ${org.nom}`, 'success', 'fa-solid fa-eye');
  addTerminalLog('blue', `Vue détails : <span class="t-hi">${org.nom}</span>`);
};
const editOrg = (org) => {
  showPulseToast(`Édition : ${org.nom}`, 'warn', 'fa-solid fa-pen-to-square');
};
const deleteOrg = async (id) => {
  if (!confirm('Supprimer cette organisation ?')) return;
  try {
    await superAdminApi.deleteOrg(id);
    orgs.value = orgs.value.filter(o => o.id !== id);
    showPulseToast('Organisation supprimée.', 'warn', 'fa-solid fa-trash-can');
    addTerminalLog('amber', `Organisation <span class="t-hi">#${id}</span> supprimée`);
  } catch {
    orgs.value = orgs.value.filter(o => o.id !== id);
    showPulseToast('Organisation retirée de la liste.', 'warn', 'fa-solid fa-trash-can');
  }
};

// ─── HELPERS ───────────────────────────────────────────────────
const formatDate     = (d) => d ? new Date(d).toLocaleDateString('fr-FR', { day:'numeric', month:'short' }) : '—';
const orbStyle       = (f) => ({ transform:`translate(${mousePos.x*f*10}px,${mousePos.y*f*10}px)` });
const handleParallax = (e) => { mousePos.x=(e.clientX-window.innerWidth/2)/20; mousePos.y=(e.clientY-window.innerHeight/2)/20; };
const handleKeyboard = (e) => { if (e.key==='Escape') showOrgModal.value = false; };

watch(recycleViewport, (el) => { if (el) viewportHeight_.value = el.clientHeight; });

onMounted(() => {
  fetchData();
  document.addEventListener('keydown', handleKeyboard);
  if (recycleViewport.value) viewportHeight_.value = recycleViewport.value.clientHeight;
});
onUnmounted(() => document.removeEventListener('keydown', handleKeyboard));
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800&family=JetBrains+Mono:wght@400;500;700&display=swap');

/* ─── ROOT ─────────────────────────────────────────────────── */
.enigma-master-root { min-height:100vh; background:#f8fafc; font-family:'Plus Jakarta Sans',sans-serif; color:#0f172a; }

/* ─── BACKGROUND ───────────────────────────────────────────── */
.cyber-engine-bg { position:fixed; inset:0; z-index:0; pointer-events:none; }
.quantum-grid { position:absolute; inset:0; background-image:radial-gradient(#cbd5e1 1px,transparent 1px); background-size:40px 40px; opacity:.2; }
.glow-orb { position:absolute; width:600px; height:600px; filter:blur(120px); opacity:.15; border-radius:50%; transition:transform .3s ease-out; }
.orb-amber { background:#f59e0b; top:-200px; right:-100px; }
.orb-blue  { background:#6366f1; bottom:-200px; left:-100px; }
.main-orchestrator { z-index:5; }
.canvas-engine { height:calc(100vh - 64px); }

/* ─── THEME TOGGLE ─────────────────────────────────────────── */
.theme-toggle-btn {
  position:fixed; top:80px; right:20px; z-index:200; width:42px; height:42px; border-radius:14px;
  background:white; border:1.5px solid #eef2f6; color:#64748b;
  cursor:pointer; font-size:16px; display:flex; align-items:center; justify-content:center; transition:.2s;
  box-shadow:0 4px 16px rgba(0,0,0,.06);
}
.theme-toggle-btn:hover { color:#f59e0b; border-color:#f59e0b; }

/* ─── LOADER ───────────────────────────────────────────────── */
.loader-overlay { position:fixed; inset:0; background:#f8fafc; z-index:9999; display:flex; flex-direction:column; align-items:center; justify-content:center; }
.tech-loader-container { text-align:center; }
.loader-text { font-weight:800; color:#0f172a; letter-spacing:2px; font-size:14px; font-family:'JetBrains Mono',monospace; margin-top:16px; }
.loader-sub  { font-size:11px; color:#94a3b8; margin-top:6px; font-family:'JetBrains Mono',monospace; }
.fade-leave-active { transition:opacity .3s; } .fade-leave-to { opacity:0; }

/* ─── HEADER ───────────────────────────────────────────────── */
.premium-title { font-weight:800; font-size:2.2rem; letter-spacing:-1px; }
.gradient-text { background:linear-gradient(135deg,#f59e0b 0%,#fbbf24 100%); -webkit-background-clip:text; -webkit-text-fill-color:transparent; }
.breadcrumb-pro { font-size:.72rem; font-weight:700; color:#94a3b8; }
.breadcrumb-pro .root { cursor:pointer; } .breadcrumb-pro .root:hover { color:#f59e0b; }
.breadcrumb-pro .separator { font-size:.55rem; opacity:.5; }
.breadcrumb-pro .current { color:#0f172a; font-weight:800; }
.page-sub { font-size:.75rem; color:#94a3b8; font-weight:600; margin-top:6px; }

.system-status-pro { display:flex; align-items:center; background:white; border:1.5px solid #eef2f6; border-radius:100px; padding:7px 16px; gap:8px; }
.status-dot-pro { width:7px; height:7px; background:#10b981; border-radius:50%; display:inline-block; }
.pulse { animation:statusPulse 2s infinite; }
@keyframes statusPulse { 0%{box-shadow:0 0 0 0 rgba(16,185,129,.6)}70%{box-shadow:0 0 0 8px rgba(16,185,129,0)}100%{box-shadow:0 0 0 0 rgba(16,185,129,0)} }
.status-text-pro { font-size:.65rem; font-weight:800; color:#64748b; letter-spacing:.8px; text-transform:uppercase; }

.btn-refresh-pro { width:44px; height:44px; background:white; border:1.5px solid #e2e8f0; border-radius:14px; color:#64748b; cursor:pointer; transition:.3s; display:flex; align-items:center; justify-content:center; }
.btn-refresh-pro:hover:not(:disabled) { background:#f8fafc; border-color:#f59e0b; color:#f59e0b; transform:rotate(180deg) scale(1.1); }
.shadow-premium { box-shadow:0 20px 60px rgba(0,0,0,.12)!important; }

/* ─── STAT CARDS ───────────────────────────────────────────── */
.stat-card-premium { background:white; border-radius:24px; padding:24px; display:flex; align-items:center; border:1px solid #eef2f6; transition:.2s; box-shadow:0 2px 8px rgba(0,0,0,.04); }
.stat-card-premium:hover { transform:translateY(-3px); box-shadow:0 10px 30px rgba(0,0,0,.06); }
.stat-icon-wrapper { width:56px; height:56px; border-radius:16px; display:flex; align-items:center; justify-content:center; font-size:1.4rem; flex-shrink:0; }
.stat-value { font-size:1.6rem; font-weight:800; line-height:1; }
.stat-label { font-size:.7rem; font-weight:700; color:#94a3b8; text-transform:uppercase; margin-top:4px; }
.stat-details { margin-left:16px; }
.stat-trend { display:flex; flex-direction:column; align-items:center; font-size:.65rem; font-weight:800; gap:2px; }
.trend-up { color:#10b981; } .trend-down { color:#f43f5e; }

/* ─── FILTERS ──────────────────────────────────────────────── */
.nav-tab-btn-modern { padding:8px 18px; border-radius:12px; border:none; background:transparent; font-weight:800; font-size:.8rem; color:#94a3b8; cursor:pointer; transition:.2s; font-family:inherit; }
.nav-tab-btn-modern.active { background:#0f172a; color:white; }
.tab-count { background:rgba(255,255,255,.2); padding:2px 7px; border-radius:8px; font-size:.65rem; margin-left:6px; }
.nav-tab-btn-modern:not(.active) .tab-count { background:#f1f5f9; color:#64748b; }
.search-inline-box { display:flex; align-items:center; background:white; border:1.5px solid #eef2f6; border-radius:14px; padding:0 14px; gap:10px; color:#94a3b8; }
.search-inline-input { border:none; outline:none; background:transparent; padding:10px 0; font-weight:700; font-size:.85rem; width:180px; font-family:inherit; }
.btn-clear-search { border:none; background:transparent; color:#94a3b8; padding:0; cursor:pointer; }
.sort-select-pro { border:1.5px solid #eef2f6; border-radius:14px; padding:10px 14px; font-weight:700; font-size:.8rem; background:white; outline:none; cursor:pointer; font-family:inherit; }

/* ─── CARD ENIGMA ──────────────────────────────────────────── */
.enigma-card { background:white; border-radius:32px; border:1px solid #eef2f6; box-shadow:0 2px 8px rgba(0,0,0,.03); }
.card-header-section { border-bottom:1px solid #eef2f6; }
.icon-box-v2 { width:48px; height:48px; border-radius:16px; display:flex; align-items:center; justify-content:center; font-size:1.1rem; flex-shrink:0; }
.icon-box-v2.amber { background:#fffbeb; color:#f59e0b; }
.pane-header-v2 { display:flex; align-items:center; gap:16px; }

/* ─── TABLE ────────────────────────────────────────────────── */
.table-enigma { width:100%; border-collapse:collapse; }
.table-enigma thead th { padding:12px 16px; font-size:.62rem; font-weight:900; text-transform:uppercase; letter-spacing:1px; color:#94a3b8; background:#f8fafc; border-bottom:1px solid #eef2f6; }
.table-row-enigma { border-bottom:1px solid #eef2f6; transition:background .15s; animation:entry-up .4s both; }
.table-row-enigma:hover { background:rgba(255,251,235,.5); }
.table-enigma td { padding:14px 16px; vertical-align:middle; }
.avatar-v8 { width:38px; height:38px; min-width:38px; border-radius:12px; background:linear-gradient(135deg,#f59e0b,#fbbf24); color:#0f172a; font-weight:900; font-size:.9rem; display:flex; align-items:center; justify-content:center; flex-shrink:0; }
.date-chip-pro { background:#f8fafc; border:1px solid #eef2f6; color:#64748b; font-size:.7rem; font-weight:800; padding:3px 10px; border-radius:8px; font-family:'JetBrains Mono',monospace; }
.slot-badge-amber { background:linear-gradient(135deg,#fffbeb,#fef3c7); color:#f59e0b; font-size:.7rem; font-weight:800; padding:5px 14px; border-radius:10px; border:1px solid #fde68a; font-family:'JetBrains Mono',monospace; }
.btn-action-pro { width:34px; height:34px; border-radius:10px; border:none; display:inline-flex; align-items:center; justify-content:center; cursor:pointer; transition:.15s; font-size:.75rem; }
.btn-action-pro.approve { background:#ecfdf5; color:#10b981; }
.btn-action-pro.approve:hover { background:#d1fae5; transform:scale(1.1); }
.btn-action-pro.reject  { background:#fef2f2; color:#ef4444; }
.btn-action-pro.reject:hover  { background:#fee2e2; transform:scale(1.1); }

/* ─── IA DONUT ─────────────────────────────────────────────── */
.ia-donut-wrapper { display:flex; align-items:center; gap:16px; }
.ia-metrics-list  { display:flex; flex-direction:column; gap:10px; flex:1; }
.ia-metric-item   { display:flex; align-items:center; gap:8px; }
.legend-dot { width:8px; height:8px; border-radius:50%; display:inline-block; flex-shrink:0; }
.dot-amber { background:#f59e0b; } .dot-green { background:#10b981; } .dot-indigo { background:#4f46e5; }
.donut-center-text { font-size:20px; font-weight:900; fill:#0f172a; font-family:'Plus Jakarta Sans',sans-serif; }
.donut-sub-text { font-size:7px; fill:#94a3b8; font-weight:800; letter-spacing:1px; }

/* ─── PROGRESS ─────────────────────────────────────────────── */
.progress-slim { height:5px; background:#f1f5f9; border-radius:10px; overflow:hidden; }
.progress-fill { height:100%; border-radius:10px; transition:width 1.2s cubic-bezier(.165,.84,.44,1); }

/* ─── REGISTRE ENTREPRISES ─────────────────────────────────── */
.recycle-badge-pro { font-size:.6rem; font-weight:800; color:#94a3b8; letter-spacing:1.5px; text-transform:uppercase; background:#f8fafc; border:1px solid #eef2f6; padding:4px 10px; border-radius:8px; }
.list-header-row { background:#f8fafc; border-bottom:1px solid #eef2f6; }
.list-col-label { font-size:.6rem; font-weight:900; color:#94a3b8; text-transform:uppercase; letter-spacing:1px; }
.recycle-viewport-pro { height:420px; overflow-y:auto; position:relative; scroll-behavior:smooth; }
.recycle-viewport-pro::-webkit-scrollbar { width:6px; }
.recycle-viewport-pro::-webkit-scrollbar-track { background:transparent; }
.recycle-viewport-pro::-webkit-scrollbar-thumb { background:#eef2f6; border-radius:10px; }
.recycle-viewport-pro::-webkit-scrollbar-thumb:hover { background:#fbbf24; }
.list-row-item { display:flex; align-items:center; gap:12px; border-bottom:1px solid #eef2f6; transition:background .15s,transform .15s; animation:entry-up .35s both; }
.list-row-item:hover { background:rgba(255,251,235,.6); transform:translateX(4px); }
@keyframes entry-up { from{opacity:0;transform:translateY(12px);}to{opacity:1;transform:translateY(0);} }
.rank-label { font-family:'JetBrains Mono',monospace; font-size:.7rem; font-weight:800; color:#94a3b8; }
.status-badge { padding:4px 12px; border-radius:10px; font-size:.62rem; font-weight:800; text-transform:uppercase; display:inline-flex; align-items:center; }
.status-1 { background:#ecfdf5; color:#10b981; }
.status-2 { background:#f1f5f9; color:#64748b; }
.status-dot { width:5px; height:5px; border-radius:50%; background:currentColor; margin-right:6px; }
.score-ring-pro { --pct:75; --col:#f59e0b; width:44px; height:44px; border-radius:50%; display:inline-flex; align-items:center; justify-content:center; background:conic-gradient(var(--col) calc(var(--pct)*1%),#eef2f6 0); box-shadow:inset 0 0 0 6px white; }
.score-ring-val { font-family:'JetBrains Mono',monospace; font-size:.62rem; font-weight:900; color:#0f172a; }
.recycle-stat-pro { display:flex; flex-direction:column; align-items:center; }
.recycle-stat-pro .rv { font-size:14px; font-weight:900; color:#0f172a; line-height:1; }
.recycle-stat-pro .rl { font-size:.55rem; font-weight:700; color:#94a3b8; text-transform:uppercase; letter-spacing:.8px; margin-top:2px; }

/* ─── PILLS ─────────────────────────────────────────────────── */
.t-pill { font-size:.6rem; font-weight:800; padding:2px 8px; border-radius:6px; }
.cat-pill  { background:#f0f9ff; color:#0284c7; }
.type-pill { background:#f0fdf4; color:#16a34a; }

/* ─── TERMINAL ─────────────────────────────────────────────── */
.analytics-hub-glass { background:linear-gradient(135deg,#080f1e 0%,#0f172a 50%,#130a2a 100%); border-radius:32px; border:1px solid rgba(255,255,255,.06); box-shadow:0 32px 64px rgba(15,23,42,.2); }
.scanner-line { position:absolute; top:0; left:0; width:100%; height:1px; background:linear-gradient(90deg,transparent,rgba(234,179,8,.6) 50%,transparent); animation:scan 6s linear infinite; border-radius:32px 32px 0 0; }
@keyframes scan { 0%{top:0%;opacity:0}5%{opacity:1}95%{opacity:.3}100%{top:100%;opacity:0} }
.terminal-titlebar { display:flex; align-items:center; border-bottom:1px solid rgba(255,255,255,.05); }
.terminal-dot { width:10px; height:10px; border-radius:50%; }
.terminal-dot.red{background:#ef4444;} .terminal-dot.amber-dot{background:#f59e0b;} .terminal-dot.green-dot{background:#22c55e;}
.t-line-anim { display:flex; align-items:baseline; gap:8px; animation:entry-up .4s both; margin-bottom:4px; }
.t-time  { font-family:'JetBrains Mono',monospace; font-size:.65rem; color:rgba(255,255,255,.18); min-width:68px; flex-shrink:0; }
.t-prompt { font-family:'JetBrains Mono',monospace; font-size:.85rem; font-weight:700; flex-shrink:0; }
.t-prompt.green{color:#22c55e;} .t-prompt.blue{color:#60a5fa;} .t-prompt.amber{color:#f59e0b;}
.t-text  { font-family:'JetBrains Mono',monospace; font-size:.72rem; color:rgba(255,255,255,.45); line-height:1.8; }
:deep(.t-hi){color:rgba(255,255,255,.85);font-weight:600;}
:deep(.t-ok){color:#22c55e;font-weight:700;}

/* ─── BUTTONS ──────────────────────────────────────────────── */
.btn-enigma-primary { background:#0f172a; color:white; border:none; padding:14px 28px; border-radius:18px; font-weight:800; position:relative; overflow:hidden; cursor:pointer; font-family:inherit; }
.btn-enigma-primary .btn-glow { position:absolute; inset:0; background:linear-gradient(135deg,#f59e0b,#fbbf24); opacity:0; transition:.3s; z-index:1; }
.btn-enigma-primary:hover .btn-glow { opacity:1; }
.btn-enigma-primary .btn-content { position:relative; z-index:2; display:flex; align-items:center; }
.btn-enigma-primary:hover .btn-content { color:#0f172a; }
.btn-enigma-primary:disabled { opacity:.4; cursor:not-allowed; }
.btn-icon-sm { width:32px; height:32px; border-radius:10px; border:1.5px solid #eef2f6; background:white; color:#64748b; cursor:pointer; transition:.2s; font-size:.75rem; display:flex; align-items:center; justify-content:center; }
.btn-icon-sm:hover { background:#f8fafc; color:#0f172a; }
.btn-icon-sm.danger:hover { background:#fff1f2; color:#f43f5e; border-color:#f43f5e; }

/* ─── MODAL ────────────────────────────────────────────────── */
.quantum-vault-overlay { position:fixed; inset:0; background:rgba(15,23,42,.6); backdrop-filter:blur(10px); z-index:2000; display:flex; align-items:flex-start; justify-content:center; padding:40px 12px; overflow-y:auto; }
.quick-add-modal { background:white; border-radius:32px; padding:40px; box-shadow:0 40px 100px rgba(0,0,0,.2); max-height:90vh; overflow-y:auto; }
.quick-add-modal::-webkit-scrollbar { width:6px; }
.quick-add-modal::-webkit-scrollbar-thumb { background:#eef2f6; border-radius:10px; }
.modal-section-label { font-size:.8rem; font-weight:800; color:#0f172a; border-bottom:1px solid #eef2f6; padding-bottom:12px; display:flex; align-items:center; margin-bottom:0; }
.modal-alert-info { background:#f0f9ff; color:#0369a1; border:1px solid #bae6fd; border-radius:12px; padding:12px 16px; font-size:.8rem; font-weight:600; display:flex; align-items:center; }
.enigma-input-wrap label { font-size:.6rem; font-weight:900; color:#94a3b8; letter-spacing:1px; margin-bottom:8px; display:block; }
.required-star { color:#f43f5e; }
.enigma-field { width:100%; padding:13px 18px; background:#f8fafc; border:2px solid #eef2f6; border-radius:14px; font-weight:700; outline:none; font-family:inherit; transition:.2s; font-size:.88rem; color:#0f172a; }
.enigma-field:focus { border-color:#f59e0b; background:white; }
textarea.enigma-field { resize:vertical; }
.btn-qv-cancel { background:#f1f5f9; color:#64748b; border:none; padding:12px 24px; border-radius:14px; font-weight:800; cursor:pointer; font-family:inherit; }

/* ─── TOAST ────────────────────────────────────────────────── */
.enigma-toast { position:fixed; bottom:30px; right:30px; background:#0f172a; color:white; padding:20px 30px; border-radius:20px; display:flex; align-items:center; gap:15px; z-index:3000; border-left:5px solid #f59e0b; box-shadow:0 20px 40px rgba(0,0,0,.2); }
.t-success{border-left-color:#10b981;} .t-error{border-left-color:#f43f5e;} .t-warn{border-left-color:#f59e0b;}
.t-ico { font-size:18px; }
.t-body strong { font-size:.6rem; font-weight:900; letter-spacing:1px; color:rgba(255,255,255,.5); }
.t-body .small { font-size:.75rem; color:rgba(255,255,255,.85); margin-top:2px; }
.toast-slide-enter-active { animation:slideIn .4s ease-out; }
.toast-slide-leave-active  { animation:slideIn .3s ease-in reverse; }
@keyframes slideIn { from{transform:translateX(120%);opacity:0;}to{transform:translateX(0);opacity:1;} }

/* ─── TRANSITIONS ──────────────────────────────────────────── */
.modal-quantum-enter-active { animation:zoomIn .25s ease-out; }
.modal-quantum-leave-active { animation:zoomIn .2s ease-in reverse; }
@keyframes zoomIn { from{opacity:0;transform:scale(.92);}to{opacity:1;transform:scale(1);} }

/* ─── SPINNER ──────────────────────────────────────────────── */
.spinner-pro-premium { width:50px; height:50px; border:4px solid #f1f5f9; border-top:4px solid #f59e0b; border-radius:50%; animation:spin 1s linear infinite; margin:40px auto; }
@keyframes spin { to{transform:rotate(360deg);} }

/* ─── MISC ─────────────────────────────────────────────────── */
.text-amber   { color:#f59e0b!important; }
.text-success { color:#10b981!important; }
.text-danger  { color:#f43f5e!important; }
.fw-700 { font-weight:700!important; }
.fw-800 { font-weight:800!important; }
.fw-900 { font-weight:900!important; }
.empty-state-pro { background:#f8fafc; border-radius:20px; border:1px dashed #e2e8f0; }

/* ─── ANIMATIONS ───────────────────────────────────────────── */
@keyframes blink-cursor { 0%,100%{opacity:1}50%{opacity:0} }
@keyframes pulse { 0%,100%{opacity:1}50%{opacity:.3} }

/* ══════════════════════════════════════════════════
   DARK MODE
══════════════════════════════════════════════════ */
[data-theme="dark"] .enigma-master-root { background:#0d1117; color:#f0f6fc; }
[data-theme="dark"] .canvas-engine { background:#0d1117; }
[data-theme="dark"] .premium-title { color:#f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color:#f0f6fc; }
[data-theme="dark"] .page-sub { color:#6e7681; }

[data-theme="dark"] .stat-card-premium { background:rgba(22,27,34,.9); border-color:rgba(255,255,255,.06); }
[data-theme="dark"] .stat-value { color:#f0f6fc; }

[data-theme="dark"] .enigma-card { background:#161b22; border-color:rgba(255,255,255,.07); }
[data-theme="dark"] .card-header-section { border-bottom-color:rgba(255,255,255,.07); }
[data-theme="dark"] .pane-header-v2 h6 { color:#f0f6fc; }

[data-theme="dark"] .system-status-pro { background:#161b22; border-color:rgba(255,255,255,.07); }
[data-theme="dark"] .btn-refresh-pro { background:#161b22; border-color:rgba(255,255,255,.07); color:#8b949e; }
[data-theme="dark"] .btn-refresh-pro:hover:not(:disabled) { background:#0d1117; }

[data-theme="dark"] .tabs-container .bg-white { background:#161b22!important; border-color:rgba(255,255,255,.07)!important; }
[data-theme="dark"] .nav-tab-btn-modern { color:#6e7681; }
[data-theme="dark"] .nav-tab-btn-modern.active { background:#0d1117; color:#f0f6fc; }
[data-theme="dark"] .nav-tab-btn-modern:not(.active) .tab-count { background:rgba(255,255,255,.05); color:#6e7681; }

[data-theme="dark"] .search-inline-box { background:rgba(255,255,255,.05); border-color:rgba(255,255,255,.07); }
[data-theme="dark"] .search-inline-input { color:#f0f6fc; }
[data-theme="dark"] .sort-select-pro { background:rgba(255,255,255,.05); border-color:rgba(255,255,255,.07); color:#f0f6fc; }

[data-theme="dark"] .table-enigma thead th { background:rgba(255,255,255,.02); border-bottom-color:rgba(255,255,255,.06); }
[data-theme="dark"] .table-row-enigma { border-bottom-color:rgba(255,255,255,.06); }
[data-theme="dark"] .table-row-enigma:hover { background:rgba(245,158,11,.05); }
[data-theme="dark"] .table-enigma td { color:#f0f6fc; }
[data-theme="dark"] .date-chip-pro { background:rgba(255,255,255,.05); border-color:rgba(255,255,255,.06); color:#8b949e; }
[data-theme="dark"] .slot-badge-amber { background:rgba(245,158,11,.1); border-color:rgba(245,158,11,.2); }

[data-theme="dark"] .donut-center-text { fill:#f0f6fc; }

[data-theme="dark"] .list-header-row { background:rgba(255,255,255,.02); border-bottom-color:rgba(255,255,255,.06); }
[data-theme="dark"] .list-row-item { border-bottom-color:rgba(255,255,255,.05); }
[data-theme="dark"] .list-row-item:hover { background:rgba(245,158,11,.05); }
[data-theme="dark"] .recycle-viewport-pro::-webkit-scrollbar-thumb { background:rgba(255,255,255,.07); }
[data-theme="dark"] .score-ring-pro { box-shadow:inset 0 0 0 6px #161b22; }
[data-theme="dark"] .score-ring-val { color:#f0f6fc; }
[data-theme="dark"] .recycle-stat-pro .rv { color:#f0f6fc; }

[data-theme="dark"] .enigma-field { background:rgba(255,255,255,.05); border-color:rgba(255,255,255,.08); color:#f0f6fc; }
[data-theme="dark"] .enigma-field:focus { border-color:#f59e0b; background:rgba(255,255,255,.08); }
[data-theme="dark"] .quick-add-modal { background:#161b22; }
[data-theme="dark"] .modal-section-label { color:#f0f6fc; border-bottom-color:rgba(255,255,255,.06); }
[data-theme="dark"] .btn-qv-cancel { background:rgba(255,255,255,.06); color:#8b949e; }

[data-theme="dark"] .btn-icon-sm { background:#161b22; border-color:rgba(255,255,255,.07); color:#8b949e; }
[data-theme="dark"] .btn-icon-sm:hover { background:rgba(255,255,255,.08); color:#f0f6fc; }

[data-theme="dark"] .empty-state-pro { background:rgba(255,255,255,.02); border-color:rgba(255,255,255,.06); }
</style>