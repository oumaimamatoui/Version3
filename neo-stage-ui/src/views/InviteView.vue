<template>
  <div class="enigma-invite-root d-flex overflow-hidden" @mousemove="handleParallax">

    <!-- ═══════════════════ BACKGROUND ═══════════════════ -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-teal"  :style="orbStyle(0.02)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="p-4 p-lg-5">

          <!-- ═══════════════════ HEADER PREMIUM ═══════════════════ -->
          <header class="d-flex justify-content-between align-items-end mb-5 flex-wrap gap-3">
            <div>
              <div class="breadcrumb-pro mb-2">
                <span class="root">Administration</span>
                <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                <span class="current">Terminal d'Invitations</span>
              </div>
              <h2 class="premium-title">Studio de <span class="gradient-text">Déploiement</span></h2>
            </div>
            <div class="d-flex gap-3 flex-wrap align-items-center">
              <div class="system-live-badge">
                <span class="pulse-dot"></span> RÉSEAU OPTIMAL
              </div>
              <div class="view-toggle-cluster">
                <button :class="['btn-view-toggle', { active: activeView === 'send' }]"
                  @click="activeView = 'send'" title="Envoi">
                  <i class="fa-solid fa-paper-plane"></i>
                </button>
                <button :class="['btn-view-toggle', { active: activeView === 'logs' }]"
                  @click="activeView = 'logs'" title="Logs">
                  <i class="fa-solid fa-clock-rotate-left"></i>
                </button>
                <button :class="['btn-view-toggle', { active: activeView === 'recycle' }]"
                  @click="activeView = 'recycle'" title="Corbeille">
                  <i class="fa-solid fa-trash-can"></i>
                  <span v-if="recycleItems.length > 0" class="recycle-dot">{{ recycleItems.length }}</span>
                </button>
              </div>
              <div class="counter-badge-pro">
                <i class="fa-solid fa-paper-plane me-2"></i>
                <span class="fw-900">{{ recentInvites.length }}</span>
                <span class="badge-label">ENVOIS</span>
              </div>
            </div>
          </header>

          <!-- ═══════════════════ KPI STRIP ═══════════════════ -->
          <div class="row g-4 mb-5">
            <div class="col-xl-3 col-md-6" v-for="stat in kpiStats" :key="stat.label">
              <div class="stat-card-premium">
                <div class="stat-icon-wrapper" :style="{ background: stat.bg, color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-details ms-3">
                  <div class="stat-value">{{ stat.value }}</div>
                  <div class="stat-label">{{ stat.label }}</div>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════ VIEW : SEND ═══════════════════ -->
          <div v-if="activeView === 'send'" class="animate__animated animate__fadeIn">
            <div class="row g-4">

              <!-- FORMULAIRE PRINCIPAL -->
              <div class="col-lg-8">
                <div class="enigma-card studio-form-card p-5 position-relative overflow-visible">

                  <!-- Robot flottant -->
                  <div class="bot-anchor">
                    <div class="robot-float">
                      <svg class="master-bot" viewBox="0 0 200 200">
                        <circle cx="100" cy="30" r="8" fill="none" stroke="#f59e0b" stroke-width="1.5" class="signal-ping"/>
                        <rect x="50" y="55" width="100" height="95" rx="44" fill="white" stroke="#f1f5f9" stroke-width="1.5"/>
                        <rect x="62" y="75" width="76" height="44" rx="20" fill="#0f172a"/>
                        <circle cx="84" cy="97" r="5" fill="#f59e0b" class="led-blink"/>
                        <circle cx="116" cy="97" r="5" fill="#f59e0b" class="led-blink"/>
                        <path d="M86 118 Q100 126 114 118" stroke="#f59e0b" stroke-width="2" fill="none" stroke-linecap="round"/>
                        <line x1="100" y1="30" x2="100" y2="55" stroke="#0f172a" stroke-width="3" stroke-linecap="round"/>
                        <circle cx="100" cy="30" r="6" fill="#f59e0b" class="antenna-core"/>
                        <line x1="50" y1="80" x2="30" y2="70" stroke="#f1f5f9" stroke-width="3" stroke-linecap="round"/>
                        <circle cx="28" cy="69" r="5" fill="#f59e0b" opacity="0.6"/>
                        <line x1="150" y1="80" x2="170" y2="70" stroke="#f1f5f9" stroke-width="3" stroke-linecap="round"/>
                        <circle cx="172" cy="69" r="5" fill="#f59e0b" opacity="0.6"/>
                      </svg>
                    </div>
                  </div>

                  <!-- Steps indicator -->
                  <div class="steps-track mb-5">
                    <div class="step-item step-done">
                      <div class="step-bubble"><i class="fa-solid fa-check"></i></div>
                      <span>Campagne</span>
                    </div>
                    <div class="step-line"></div>
                    <div class="step-item step-active">
                      <div class="step-bubble">2</div>
                      <span>Talent</span>
                    </div>
                    <div class="step-line"></div>
                    <div class="step-item" :class="{ 'step-done': currentEmail && form.campagneId }">
                      <div class="step-bubble">{{ currentEmail && form.campagneId ? '✓' : '3' }}</div>
                      <span>Envoi</span>
                    </div>
                  </div>

                  <h4 class="form-section-title mb-5">
                    <i class="fa-solid fa-sliders me-2 text-amber"></i>
                    Configuration de l'accès
                  </h4>

                  <!-- Sélection campagne -->
                  <div class="enigma-input-wrap mb-4">
                    <label>FLUX D'ÉVALUATION CIBLE</label>
                    <div class="theme-select-wrapper">
                      <i class="fa-solid fa-rocket theme-select-icon"></i>
                      <select v-model="form.campagneId" class="enigma-field theme-select ps-5" :disabled="loadingCampagnes">
                        <option value="">{{ loadingCampagnes ? 'Synchronisation...' : 'Sélectionner une campagne' }}</option>
                        <option v-for="c in campagnes" :key="c.id" :value="c.id">{{ c.nom }}</option>
                      </select>
                    </div>
                    <p class="field-hint" v-if="form.campagneId">
                      <i class="fa-solid fa-circle-check me-1"></i> Campagne sélectionnée
                    </p>
                  </div>

                  <!-- Mode tabs -->
                  <div class="mode-tabs mb-4">
                    <button :class="['mode-tab', { active: sendMode === 'single' }]" @click="sendMode = 'single'">
                      <i class="fa-solid fa-at me-2"></i>Talent unique
                    </button>
                    <button :class="['mode-tab', { active: sendMode === 'bulk' }]" @click="sendMode = 'bulk'">
                      <i class="fa-solid fa-layer-group me-2"></i>Import CSV
                    </button>
                  </div>

                  <!-- Mode : single -->
                  <div v-if="sendMode === 'single'" class="enigma-input-wrap mb-5">
                    <label>IDENTIFIANT E-MAIL DU TALENT</label>
                    <div class="theme-select-wrapper">
                      <i class="fa-solid fa-at theme-select-icon"></i>
                      <input
                        type="email"
                        v-model="currentEmail"
                        placeholder="talent@entreprise.com"
                        class="enigma-field ps-5"
                        @keyup.enter="sendInvitation"
                      />
                    </div>
                  </div>

                  <!-- Mode : bulk CSV -->
                  <div v-if="sendMode === 'bulk'" class="mb-5">
                    <div class="enigma-input-wrap mb-3">
                      <label>IMPORT CSV (UN EMAIL PAR LIGNE)</label>
                      <textarea
                        v-model="bulkEmailsRaw"
                        class="enigma-field"
                        rows="5"
                        placeholder="alice@corp.com&#10;bob@startup.io&#10;carol@agency.fr"
                      ></textarea>
                    </div>
                    <div v-if="parsedBulkEmails.length > 0" class="bulk-preview-bar">
                      <i class="fa-solid fa-users me-2 text-amber"></i>
                      <span class="fw-800">{{ parsedBulkEmails.length }}</span> talent(s) détecté(s)
                      <div class="bulk-chips ms-3">
                        <span v-for="(e, i) in parsedBulkEmails.slice(0, 4)" :key="i" class="bulk-chip">{{ e }}</span>
                        <span v-if="parsedBulkEmails.length > 4" class="bulk-chip more">+{{ parsedBulkEmails.length - 4 }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- Footer formulaire -->
                  <div class="form-footer-pro">
                    <div class="security-cluster">
                      <div class="security-icon-box">
                        <i class="fa-solid fa-shield-halved"></i>
                      </div>
                      <div>
                        <div class="security-label">Protocole SHA-256</div>
                        <div class="security-sub">Lien chiffré · Validité 72h</div>
                      </div>
                    </div>
                    <button
                      @click="sendInvitation"
                      :disabled="isLoading || !form.campagneId || (!currentEmail && parsedBulkEmails.length === 0)"
                      class="btn-enigma-primary"
                    >
                      <div class="btn-content">
                        <template v-if="!isLoading">
                          <i class="fa-solid fa-paper-plane me-2"></i>
                          <span>DÉPLOYER</span>
                        </template>
                        <div v-else class="btn-dots-loader">
                          <span></span><span></span><span></span>
                        </div>
                      </div>
                      <div class="btn-glow"></div>
                    </button>
                  </div>
                </div>
              </div>

              <!-- SIDEBAR DROITE -->
              <div class="col-lg-4 d-flex flex-column gap-4">

                <!-- Info Card dark -->
                <div class="enigma-dark-card p-4">
                  <div class="info-card-top mb-3">
                    <div class="info-icon-wrap"><i class="fa-solid fa-wand-magic-sparkles"></i></div>
                    <h5 class="text-white fw-900 m-0">Guide de Flux</h5>
                  </div>
                  <p class="text-muted-light mb-0">Chaque invitation génère un environnement sécurisé unique. Le système Proctoring surveille les anomalies comportementales en temps réel.</p>
                  <div class="luxury-divider my-3"></div>
                  <div class="info-features">
                    <div class="info-feature-item" v-for="feat in infoFeatures" :key="feat.label">
                      <div class="feat-dot"></div>
                      <div>
                        <div class="feat-label">{{ feat.label }}</div>
                        <div class="feat-value">{{ feat.value }}</div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Stats rapides -->
                <div class="enigma-card p-4">
                  <div class="stats-title mb-3">ACTIVITÉ DU JOUR</div>
                  <div class="stats-bento-grid">
                    <div class="bento-stat-box">
                      <span class="bento-val">{{ recentInvites.length }}</span>
                      <span class="bento-lbl">Invitations</span>
                    </div>
                    <div class="bento-stat-box highlight">
                      <span class="bento-val">{{ campagnes.length }}</span>
                      <span class="bento-lbl">Campagnes</span>
                    </div>
                    <div class="bento-stat-full">
                      <i class="fa-solid fa-shield-halved me-2"></i>
                      <span>Système sécurisé · En ligne</span>
                    </div>
                  </div>
                </div>

                <!-- Email preview -->
                <div class="enigma-card overflow-hidden">
                  <div class="preview-header-bar">
                    <div class="preview-dots">
                      <span class="pd red"></span>
                      <span class="pd yellow"></span>
                      <span class="pd green"></span>
                    </div>
                    <span class="preview-label">Aperçu e-mail</span>
                  </div>
                  <div class="p-4">
                    <div class="preview-logo-row mb-3">
                      <div class="prev-logo-box">E</div>
                      <span class="prev-brand fw-900">EvaluaTech</span>
                    </div>
                    <div class="mock-line" style="width:70%"></div>
                    <div class="mock-line" style="width:90%"></div>
                    <div class="mock-line" style="width:55%"></div>
                    <div class="mock-btn-preview mt-3">
                      <i class="fa-solid fa-arrow-right me-1"></i> Accéder à l'évaluation
                    </div>
                    <div class="mock-line" style="width:40%; margin-top:12px; opacity:0.4"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════ VIEW : LOGS ═══════════════════ -->
          <div v-if="activeView === 'logs'" class="animate__animated animate__fadeIn">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
              <div>
                <h4 class="fw-900 m-0 section-title-dark">Logs de Transmission</h4>
                <p class="text-muted small m-0">Historique complet des signaux envoyés</p>
              </div>
              <div class="d-flex gap-2 align-items-center flex-wrap">
                <div class="search-inline-box">
                  <i class="fa-solid fa-magnifying-glass"></i>
                  <input type="text" v-model="logsSearch" placeholder="Filtrer..." class="search-inline-input">
                  <button v-if="logsSearch" @click="logsSearch = ''" class="btn-clear-search"><i class="fa-solid fa-xmark"></i></button>
                </div>
                <button class="btn-outline-pro" @click="exportLogs">
                  <i class="fa-solid fa-file-export me-2"></i>Exporter
                </button>
                <button class="btn-danger-outline" @click="clearLogsWithConfirm" :disabled="recentInvites.length === 0">
                  <i class="fa-solid fa-trash-can me-1"></i>Vider
                </button>
              </div>
            </div>

            <div class="enigma-card overflow-hidden">
              <div class="table-header-elite">
                <div class="d-flex align-items-center gap-2">
                  <div class="table-header-icon"><i class="fa-solid fa-clock-rotate-left"></i></div>
                  <span>LOGS DE TRANSMISSION RÉCENTS</span>
                </div>
                <div class="table-badge-count">{{ filteredLogs.length }} envois</div>
              </div>
              <div class="table-responsive">
                <table class="table elite-table mb-0">
                  <thead>
                    <tr>
                      <th>DESTINATAIRE</th>
                      <th>CAMPAGNE</th>
                      <th>STATUT</th>
                      <th class="text-end">HEURE</th>
                      <th class="text-center">ACTION</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, index) in paginatedLogs" :key="index">
                      <td>
                        <div class="email-cell">
                          <div class="email-avatar">{{ item.email.charAt(0).toUpperCase() }}</div>
                          <span class="fw-700">{{ item.email }}</span>
                        </div>
                      </td>
                      <td class="text-muted small">{{ item.campagne || '—' }}</td>
                      <td>
                        <span class="status-pill-elite">
                          <i class="fa-solid fa-check me-1"></i>EXPÉDIÉ
                        </span>
                      </td>
                      <td class="text-end text-muted small">{{ item.date }}</td>
                      <td class="text-center">
                        <button class="btn-icon-sm danger"
                          @click="deleteLog(logsCurrentPage * logsPerPage + index)"
                          title="Déplacer en corbeille">
                          <i class="fa-solid fa-trash-can"></i>
                        </button>
                      </td>
                    </tr>
                    <tr v-if="filteredLogs.length === 0">
                      <td colspan="5" class="empty-state-row">
                        <div class="empty-icon"><i class="fa-solid fa-satellite-dish"></i></div>
                        <p>Aucun signal envoyé pour le moment.</p>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>

              <!-- PAGINATION LOGS -->
              <div v-if="logsTotalPages > 1" class="pagination-bar">
                <div class="pagination-info">
                  <span>Affichage</span>
                  <strong>{{ logsCurrentPage * logsPerPage + 1 }}–{{ Math.min((logsCurrentPage + 1) * logsPerPage, filteredLogs.length) }}</strong>
                  <span>sur</span>
                  <strong>{{ filteredLogs.length }}</strong>
                  <span>entrées</span>
                </div>
                <div class="pagination-controls">
                  <button class="pg-btn" @click="logsCurrentPage = 0" :disabled="logsCurrentPage === 0" title="Première">
                    <i class="fa-solid fa-angles-left"></i>
                  </button>
                  <button class="pg-btn" @click="logsCurrentPage--" :disabled="logsCurrentPage === 0" title="Précédente">
                    <i class="fa-solid fa-angle-left"></i>
                  </button>
                  <div class="pg-numbers">
                    <button
                      v-for="p in logsPagesRange" :key="p"
                      :class="['pg-num', { active: p === logsCurrentPage, ellipsis: p === '...' }]"
                      @click="p !== '...' && (logsCurrentPage = p)"
                      :disabled="p === '...'">
                      {{ p === '...' ? '…' : p + 1 }}
                    </button>
                  </div>
                  <button class="pg-btn" @click="logsCurrentPage++" :disabled="logsCurrentPage >= logsTotalPages - 1" title="Suivante">
                    <i class="fa-solid fa-angle-right"></i>
                  </button>
                  <button class="pg-btn" @click="logsCurrentPage = logsTotalPages - 1" :disabled="logsCurrentPage >= logsTotalPages - 1" title="Dernière">
                    <i class="fa-solid fa-angles-right"></i>
                  </button>
                </div>
                <div class="pagination-size">
                  <span class="pg-size-label">Lignes</span>
                  <select v-model="logsPerPage" @change="logsCurrentPage = 0" class="pg-size-select">
                    <option :value="5">5</option>
                    <option :value="10">10</option>
                    <option :value="20">20</option>
                    <option :value="50">50</option>
                  </select>
                </div>
              </div>
            </div>
          </div>

          <!-- ═══════════════════ VIEW : CORBEILLE ═══════════════════ -->
          <div v-if="activeView === 'recycle'" class="animate__animated animate__fadeIn">
            <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
              <div>
                <h4 class="fw-900 m-0 d-flex align-items-center gap-2 section-title-dark">
                  <div class="recycle-header-icon"><i class="fa-solid fa-trash-can"></i></div>
                  Corbeille
                </h4>
                <p class="text-muted small m-0 mt-1">Éléments supprimés — restauration possible sous 30 jours</p>
              </div>
              <div class="d-flex gap-2">
                <button class="btn-outline-pro" @click="restoreAll" :disabled="recycleItems.length === 0">
                  <i class="fa-solid fa-rotate-left me-2"></i>Tout restaurer
                </button>
                <button class="btn-danger-outline" @click="purgeRecycle" :disabled="recycleItems.length === 0">
                  <i class="fa-solid fa-fire me-2"></i>Vider définitivement
                </button>
              </div>
            </div>

            <!-- Corbeille vide -->
            <div v-if="recycleItems.length === 0" class="recycle-empty-state">
              <div class="recycle-empty-icon">
                <i class="fa-solid fa-trash-can"></i>
              </div>
              <h5 class="fw-900 mt-4 mb-2 section-title-dark">Corbeille vide</h5>
              <p class="text-muted small">Les invitations supprimées apparaîtront ici pour récupération.</p>
              <button class="btn-enigma-primary mt-3" @click="activeView = 'logs'">
                <div class="btn-content"><i class="fa-solid fa-arrow-left me-2"></i>Retour aux logs</div>
                <div class="btn-glow"></div>
              </button>
            </div>

            <!-- Items corbeille -->
            <div v-else>
              <div class="recycle-info-bar mb-4">
                <i class="fa-solid fa-circle-info me-2 text-amber"></i>
                <span>{{ recycleItems.length }} élément(s) en attente de suppression définitive</span>
                <span class="ms-auto text-muted small">Restauration possible avant purge manuelle</span>
              </div>

              <div class="enigma-card overflow-hidden">
                <div class="table-header-elite">
                  <div class="d-flex align-items-center gap-2">
                    <div class="table-header-icon recycle-icon"><i class="fa-solid fa-trash-can"></i></div>
                    <span>ÉLÉMENTS SUPPRIMÉS</span>
                  </div>
                  <div class="table-badge-count danger-badge">{{ recycleItems.length }} éléments</div>
                </div>
                <div class="table-responsive">
                  <table class="table elite-table mb-0">
                    <thead>
                      <tr>
                        <th>DESTINATAIRE</th>
                        <th>CAMPAGNE</th>
                        <th>ENVOYÉ</th>
                        <th>SUPPRIMÉ</th>
                        <th class="text-center">ACTIONS</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(item, index) in paginatedRecycle" :key="index" class="recycle-row">
                        <td>
                          <div class="email-cell opacity-60">
                            <div class="email-avatar recycle-avatar">{{ item.email.charAt(0).toUpperCase() }}</div>
                            <div>
                              <span class="fw-700">{{ item.email }}</span>
                              <div class="deleted-badge">SUPPRIMÉ</div>
                            </div>
                          </div>
                        </td>
                        <td class="text-muted small">{{ item.campagne || '—' }}</td>
                        <td class="text-muted small">{{ item.date }}</td>
                        <td class="text-muted small">{{ item.deletedAt }}</td>
                        <td class="text-center">
                          <div class="d-flex gap-2 justify-content-center">
                            <button class="btn-restore"
                              @click="restoreItem(recycleCurrentPage * recyclePerPage + index)"
                              title="Restaurer">
                              <i class="fa-solid fa-rotate-left me-1"></i>Restaurer
                            </button>
                            <button class="btn-icon-sm danger"
                              @click="purgeItem(recycleCurrentPage * recyclePerPage + index)"
                              title="Supprimer définitivement">
                              <i class="fa-solid fa-xmark"></i>
                            </button>
                          </div>
                        </td>
                      </tr>
                    </tbody>
                  </table>
                </div>

                <!-- PAGINATION CORBEILLE -->
                <div v-if="recycleTotalPages > 1" class="pagination-bar">
                  <div class="pagination-info">
                    <span>Affichage</span>
                    <strong>{{ recycleCurrentPage * recyclePerPage + 1 }}–{{ Math.min((recycleCurrentPage + 1) * recyclePerPage, recycleItems.length) }}</strong>
                    <span>sur</span>
                    <strong>{{ recycleItems.length }}</strong>
                    <span>éléments</span>
                  </div>
                  <div class="pagination-controls">
                    <button class="pg-btn" @click="recycleCurrentPage = 0" :disabled="recycleCurrentPage === 0" title="Première">
                      <i class="fa-solid fa-angles-left"></i>
                    </button>
                    <button class="pg-btn" @click="recycleCurrentPage--" :disabled="recycleCurrentPage === 0" title="Précédente">
                      <i class="fa-solid fa-angle-left"></i>
                    </button>
                    <div class="pg-numbers">
                      <button
                        v-for="p in recyclePagesRange" :key="p"
                        :class="['pg-num', { active: p === recycleCurrentPage, ellipsis: p === '...' }]"
                        @click="p !== '...' && (recycleCurrentPage = p)"
                        :disabled="p === '...'">
                        {{ p === '...' ? '…' : p + 1 }}
                      </button>
                    </div>
                    <button class="pg-btn" @click="recycleCurrentPage++" :disabled="recycleCurrentPage >= recycleTotalPages - 1" title="Suivante">
                      <i class="fa-solid fa-angle-right"></i>
                    </button>
                    <button class="pg-btn" @click="recycleCurrentPage = recycleTotalPages - 1" :disabled="recycleCurrentPage >= recycleTotalPages - 1" title="Dernière">
                      <i class="fa-solid fa-angles-right"></i>
                    </button>
                  </div>
                  <div class="pagination-size">
                    <span class="pg-size-label">Lignes</span>
                    <select v-model="recyclePerPage" @change="recycleCurrentPage = 0" class="pg-size-select">
                      <option :value="5">5</option>
                      <option :value="10">10</option>
                      <option :value="20">20</option>
                      <option :value="50">50</option>
                    </select>
                  </div>
                </div>
              </div>
            </div>
          </div>

        </div>
      </main>
    </div>

    <!-- ═══════════════════ CONFIRM DIALOG ═══════════════════ -->
    <transition name="modal-quantum">
      <div v-if="confirmDialog.show" class="quantum-vault-overlay" @click.self="confirmDialog.show = false">
        <div class="confirm-modal animate__animated animate__zoomIn animate__faster">
          <div class="confirm-icon mb-3">
            <i :class="confirmDialog.icon" class="fa-2x text-danger"></i>
          </div>
          <h5 class="fw-900 mb-2 section-title-dark">{{ confirmDialog.title }}</h5>
          <p class="text-muted small mb-4">{{ confirmDialog.message }}</p>
          <div class="d-flex gap-3 justify-content-center">
            <button @click="confirmDialog.show = false" class="btn-qv-cancel">ANNULER</button>
            <button @click="runConfirm" class="btn-confirm-danger">CONFIRMER</button>
          </div>
        </div>
      </div>
    </transition>

    <!-- ═══════════════════ TOAST PREMIUM ═══════════════════ -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>SYSTEM MESSAGE</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, reactive, computed, watch, onMounted } from 'vue';
import api from '@/services/api';
import AppSidebar from '@/components/AppSidebar.vue';
import AppNavbar  from '@/components/AppNavbar.vue';

/* ─── STATE ─────────────────────────────────────────────────────── */
const campagnes        = ref([]);
const currentEmail     = ref('');
const bulkEmailsRaw    = ref('');
const isLoading        = ref(false);
const loadingCampagnes = ref(false);
const recentInvites    = ref([]);
const recycleItems     = ref([]);
const logsSearch       = ref('');
const activeView       = ref('send');
const sendMode         = ref('single');
const form             = reactive({ campagneId: '' });
const mousePos         = reactive({ x: 0, y: 0 });

const globalToast   = reactive({ active: false, message: '', type: '', icon: '' });
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _cb: null });

/* ─── PAGINATION ─────────────────────────────────────────────────── */
const logsCurrentPage    = ref(0);
const logsPerPage        = ref(10);
const recycleCurrentPage = ref(0);
const recyclePerPage     = ref(10);

/* ─── COMPUTED ───────────────────────────────────────────────────── */
const parsedBulkEmails = computed(() =>
  bulkEmailsRaw.value
    .split(/[\n,;]+/)
    .map(e => e.trim().toLowerCase())
    .filter(e => e.includes('@'))
);

const filteredLogs = computed(() => {
  if (!logsSearch.value) return recentInvites.value;
  const q = logsSearch.value.toLowerCase();
  return recentInvites.value.filter(i =>
    i.email.toLowerCase().includes(q) ||
    (i.campagne || '').toLowerCase().includes(q)
  );
});

/* ─── PAGINATION COMPUTED ────────────────────────────────────────── */
const logsTotalPages = computed(() => Math.ceil(filteredLogs.value.length / logsPerPage.value));
const paginatedLogs  = computed(() => {
  const start = logsCurrentPage.value * logsPerPage.value;
  return filteredLogs.value.slice(start, start + logsPerPage.value);
});

const recycleTotalPages = computed(() => Math.ceil(recycleItems.value.length / recyclePerPage.value));
const paginatedRecycle  = computed(() => {
  const start = recycleCurrentPage.value * recyclePerPage.value;
  return recycleItems.value.slice(start, start + recyclePerPage.value);
});

const buildPagesRange = (current, total) => {
  if (total <= 7) return Array.from({ length: total }, (_, i) => i);
  const pages = [];
  if (current > 2)            { pages.push(0); if (current > 3) pages.push('...'); }
  for (let i = Math.max(0, current - 2); i <= Math.min(total - 1, current + 2); i++) pages.push(i);
  if (current < total - 3)    { if (current < total - 4) pages.push('...'); pages.push(total - 1); }
  return pages;
};
const logsPagesRange    = computed(() => buildPagesRange(logsCurrentPage.value, logsTotalPages.value));
const recyclePagesRange = computed(() => buildPagesRange(recycleCurrentPage.value, recycleTotalPages.value));

/* ─── RESET PAGES ON SEARCH ──────────────────────────────────────── */
watch(logsSearch, () => { logsCurrentPage.value = 0; });

const kpiStats = computed(() => [
  { label: 'Invitations Envoyées', value: recentInvites.value.length,   icon: 'fa-solid fa-paper-plane', color: '#f59e0b', bg: 'rgba(245,158,11,0.12)' },
  { label: 'Campagnes Actives',    value: campagnes.value.length,        icon: 'fa-solid fa-rocket',      color: '#6366f1', bg: 'rgba(99,102,241,0.12)' },
  { label: 'En Corbeille',         value: recycleItems.value.length,     icon: 'fa-solid fa-trash-can',   color: '#f43f5e', bg: 'rgba(244,63,94,0.12)'  },
  { label: 'Taux de Succès',       value: recentInvites.value.length ? '100%' : '—', icon: 'fa-solid fa-chart-line', color: '#10b981', bg: 'rgba(16,185,129,0.12)' },
]);

const infoFeatures = [
  { label: 'Chiffrement',    value: 'SHA-256 · TLS 1.3' },
  { label: 'Validité',       value: '72 heures' },
  { label: 'Proctoring',     value: 'Actif · Temps réel' },
];

/* ─── DATA ───────────────────────────────────────────────────────── */
onMounted(async () => {
  loadingCampagnes.value = true;
  try {
    const res = await api.get('/Invitations/campagnes');
    campagnes.value = res.data;
  } catch {
    campagnes.value = [];
  } finally {
    loadingCampagnes.value = false;
  }
});

/* ─── ACTIONS ────────────────────────────────────────────────────── */
const sendInvitation = async () => {
  const emails = sendMode.value === 'single'
    ? [currentEmail.value.toLowerCase().trim()]
    : parsedBulkEmails.value;

  if (!form.campagneId || emails.length === 0) return;
  isLoading.value = true;

  try {
    await api.post('/Invitations/invite-candidates', {
      campagneId: form.campagneId,
      emails
    });

    const campagneName = campagnes.value.find(c => c.id === form.campagneId)?.nom || '';
    const nowStr = new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' });

    emails.forEach(email => {
      recentInvites.value.unshift({ email, campagne: campagneName, date: nowStr });
    });

    showToast(`${emails.length} signal(s) transmis avec succès.`, 't-success', 'fa-solid fa-circle-check');
    currentEmail.value  = '';
    bulkEmailsRaw.value = '';
  } catch {
    showToast('Échec de la transmission du signal.', 't-error', 'fa-solid fa-circle-exclamation');
  } finally {
    isLoading.value = false;
  }
};

const deleteLog = (index) => {
  const item = recentInvites.value[index];
  recycleItems.value.unshift({
    ...item,
    deletedAt: new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' })
  });
  recentInvites.value.splice(index, 1);
  showToast('Élément déplacé vers la corbeille.', 't-warn', 'fa-solid fa-trash-can');
};

const clearLogsWithConfirm = () => {
  showConfirmDialog(
    'Vider les logs ?',
    'Tous les logs seront déplacés dans la corbeille.',
    'fa-solid fa-trash-can',
    () => {
      const nowStr = new Date().toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' });
      recentInvites.value.forEach(item => {
        recycleItems.value.unshift({ ...item, deletedAt: nowStr });
      });
      recentInvites.value = [];
      showToast('Logs déplacés en corbeille.', 't-warn', 'fa-solid fa-trash-can');
    }
  );
};

const restoreItem = (index) => {
  const item = { ...recycleItems.value[index] };
  delete item.deletedAt;
  recentInvites.value.unshift(item);
  recycleItems.value.splice(index, 1);
  showToast('Invitation restaurée avec succès.', 't-success', 'fa-solid fa-rotate-left');
};

const restoreAll = () => {
  recycleItems.value.forEach(item => {
    const restored = { ...item };
    delete restored.deletedAt;
    recentInvites.value.unshift(restored);
  });
  recycleItems.value = [];
  showToast('Tous les éléments ont été restaurés.', 't-success', 'fa-solid fa-rotate-left');
};

const purgeItem = (index) => {
  showConfirmDialog(
    'Suppression définitive ?',
    'Cette action est irréversible.',
    'fa-solid fa-fire',
    () => {
      recycleItems.value.splice(index, 1);
      showToast('Élément supprimé définitivement.', 't-error', 'fa-solid fa-xmark');
    }
  );
};

const purgeRecycle = () => {
  showConfirmDialog(
    'Vider la corbeille définitivement ?',
    'Toutes les données seront perdues. Cette action est irréversible.',
    'fa-solid fa-fire',
    () => {
      recycleItems.value = [];
      showToast('Corbeille vidée définitivement.', 't-error', 'fa-solid fa-fire');
    }
  );
};

const exportLogs = () => {
  const data = { logs: recentInvites.value, exportedAt: new Date().toISOString() };
  const blob = new Blob([JSON.stringify(data, null, 2)], { type: 'application/json' });
  const url  = URL.createObjectURL(blob);
  const a    = document.createElement('a');
  a.href = url; a.download = `invitations-logs-${Date.now()}.json`; a.click();
  URL.revokeObjectURL(url);
};

/* ─── UTILS ──────────────────────────────────────────────────────── */
const showConfirmDialog = (title, message, icon, cb) => {
  Object.assign(confirmDialog, { title, message, icon, _cb: cb, show: true });
};
const runConfirm = () => {
  confirmDialog.show = false;
  if (confirmDialog._cb) confirmDialog._cb();
};

let _toastTimer = null;
const showToast = (msg, type = 't-success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth  / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;700;800;900&display=swap');

/* ════════════════════════════════════════════════════════
   BASE
════════════════════════════════════════════════════════ */
.enigma-invite-root {
  min-height: 100vh;
  background: var(--bg-page);
  font-family: 'Plus Jakarta Sans', sans-serif;
  color: var(--text-main);
}

/* ════════════════════════════════════════════════════════
   BACKGROUND
════════════════════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }
.quantum-grid {
  position: absolute; inset: 0;
  background-image: radial-gradient(var(--border-color) 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.25;
}
.glow-orb {
  position: absolute; width: 600px; height: 600px;
  filter: blur(120px); opacity: 0.1; border-radius: 50%;
  transition: transform 0.3s ease-out;
}
.orb-amber { background: #f59e0b; top: -200px; right: -100px; }
.orb-teal  { background: #2dd4bf; bottom: -200px; left: -100px; }
.main-orchestrator { z-index: 5; }
.canvas-engine { height: calc(100vh - 64px); }

/* ════════════════════════════════════════════════════════
   HEADER
════════════════════════════════════════════════════════ */
.premium-title {
  font-weight: 900; font-size: 2.2rem; letter-spacing: -1px; margin: 0;
  color: var(--text-main);
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 100%);
  -webkit-background-clip: text; -webkit-text-fill-color: transparent;
}
.breadcrumb-pro { font-size: 0.72rem; font-weight: 700; color: var(--text-muted); }
.breadcrumb-pro .root { cursor: pointer; }
.breadcrumb-pro .root:hover { color: #f59e0b; }
.breadcrumb-pro .separator { font-size: 0.55rem; opacity: 0.5; }
.breadcrumb-pro .current { color: var(--text-main); font-weight: 800; }

.system-live-badge {
  background: var(--bg-card);
  border: 1px solid var(--border-color);
  padding: 10px 18px; border-radius: 50px;
  font-size: 11px; font-weight: 800; color: var(--success);
  display: flex; align-items: center;
  box-shadow: var(--shadow-sm);
}
.counter-badge-pro {
  background: #0f172a; color: #f59e0b; padding: 10px 18px;
  border-radius: 50px; font-size: 11px; font-weight: 800;
  display: flex; align-items: center; gap: 4px;
}
.badge-label { font-size: 9px; font-weight: 700; opacity: 0.6; letter-spacing: 1px; }
.pulse-dot {
  width: 8px; height: 8px; background: var(--success);
  border-radius: 50%; display: inline-block; margin-right: 8px;
  animation: pulse-anim 2s infinite;
}
@keyframes pulse-anim {
  0%   { box-shadow: 0 0 0 0 rgba(16,185,129,0.7); }
  70%  { box-shadow: 0 0 0 8px rgba(16,185,129,0); }
  100% { box-shadow: 0 0 0 0 rgba(16,185,129,0); }
}

/* VIEW TOGGLE */
.view-toggle-cluster {
  display: flex;
  background: var(--bg-card);
  border: 1.5px solid var(--border-color);
  border-radius: 16px; padding: 4px; gap: 4px;
  box-shadow: var(--shadow-sm);
}
.btn-view-toggle {
  width: 38px; height: 38px; border-radius: 12px; border: none;
  background: transparent; color: var(--text-muted); transition: 0.3s;
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; position: relative;
}
.btn-view-toggle:hover { background: var(--bg-hover); color: var(--text-main); }
.btn-view-toggle.active { background: #0f172a; color: #f59e0b; box-shadow: 0 4px 12px rgba(15,23,42,0.25); }
.recycle-dot {
  position: absolute; top: 2px; right: 2px;
  width: 14px; height: 14px; border-radius: 50%;
  background: var(--danger); color: #fff;
  font-size: 8px; font-weight: 900;
  display: flex; align-items: center; justify-content: center;
}

/* ════════════════════════════════════════════════════════
   KPI CARDS
════════════════════════════════════════════════════════ */
.stat-card-premium {
  background: var(--bg-card);
  border-radius: 24px; padding: 20px;
  display: flex; align-items: center;
  border: 1px solid var(--border-color);
  transition: var(--transition);
  box-shadow: var(--shadow-sm);
}
.stat-card-premium:hover { transform: translateY(-3px); box-shadow: var(--shadow-md); }
.stat-icon-wrapper {
  width: 52px; height: 52px; border-radius: 16px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.3rem; flex-shrink: 0;
}
.stat-value { font-size: 1.5rem; font-weight: 900; line-height: 1; color: var(--text-main); }
.stat-label { font-size: 0.68rem; font-weight: 700; color: var(--text-muted); text-transform: uppercase; margin-top: 4px; }

/* ════════════════════════════════════════════════════════
   ENIGMA CARDS
════════════════════════════════════════════════════════ */
.enigma-card {
  background: var(--bg-card);
  border-radius: 32px;
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-sm);
}
.studio-form-card { overflow: visible; }

/* ════════════════════════════════════════════════════════
   ROBOT FLOTTANT
════════════════════════════════════════════════════════ */
.bot-anchor { position: absolute; top: -80px; right: 44px; }
.robot-float { animation: floatBot 4s ease-in-out infinite; }
.master-bot { width: 140px; filter: drop-shadow(0 20px 40px rgba(0,0,0,0.1)); }
@keyframes floatBot {
  0%, 100% { transform: translateY(0) rotate(-1deg); }
  50%       { transform: translateY(-12px) rotate(1deg); }
}
.signal-ping { animation: ping-anim 3s ease-out infinite; transform-origin: center; }
@keyframes ping-anim { 0% { r: 6; opacity: 0.9; } 100% { r: 44; opacity: 0; } }
.led-blink { animation: blink-anim 4s ease-in-out infinite; }
@keyframes blink-anim { 0%,88%,100% { transform: scaleY(1); } 94% { transform: scaleY(0.1); } }

/* ════════════════════════════════════════════════════════
   STEPS
════════════════════════════════════════════════════════ */
.steps-track { display: flex; align-items: center; }
.step-item { display: flex; flex-direction: column; align-items: center; gap: 6px; }
.step-bubble {
  width: 36px; height: 36px; border-radius: 12px;
  background: var(--bg-hover); color: var(--text-muted);
  display: flex; align-items: center; justify-content: center;
  font-weight: 800; font-size: 14px; transition: var(--transition);
}
.step-item span { font-size: 10px; font-weight: 700; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.5px; }
.step-done .step-bubble   { background: var(--success-bg); color: var(--success); }
.step-active .step-bubble { background: #f59e0b; color: #0f172a; box-shadow: 0 6px 16px rgba(245,158,11,0.35); }
.step-active span         { color: var(--text-main); font-weight: 900; }
.step-line { flex: 1; height: 2px; background: var(--border-color); margin: 0 8px; margin-bottom: 22px; }

/* ════════════════════════════════════════════════════════
   FORM
════════════════════════════════════════════════════════ */
.form-section-title { font-weight: 900; font-size: 1.15rem; color: var(--text-main); }
.text-amber { color: #f59e0b !important; }

.enigma-input-wrap label {
  display: block; font-size: 10px; font-weight: 900;
  color: var(--text-muted); margin-bottom: 10px;
  text-transform: uppercase; letter-spacing: 0.8px;
}
.theme-select-wrapper { position: relative; }
.theme-select-icon {
  position: absolute; left: 18px; top: 50%; transform: translateY(-50%);
  color: #f59e0b; font-size: 15px; z-index: 2; pointer-events: none;
}
.enigma-field {
  width: 100%; padding: 15px 20px; border-radius: 16px;
  border: 2px solid var(--border-color);
  background: var(--bg-input);
  font-size: 14px; font-weight: 600; color: var(--text-main);
  transition: var(--transition); appearance: none; font-family: inherit;
}
.enigma-field:focus {
  outline: none; border-color: #f59e0b;
  background: var(--bg-card);
  box-shadow: 0 0 0 4px rgba(245,158,11,0.12);
}
.enigma-field::placeholder { color: var(--text-light); }
.field-hint { font-size: 11px; color: var(--success); font-weight: 700; margin: 8px 0 0 4px; }

/* MODE TABS */
.mode-tabs { display: flex; gap: 8px; }
.mode-tab {
  flex: 1; padding: 10px 16px; border-radius: 14px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-card); font-size: 12px; font-weight: 800;
  color: var(--text-muted); cursor: pointer; transition: var(--transition); font-family: inherit;
}
.mode-tab:hover { border-color: #f59e0b; color: var(--text-main); }
.mode-tab.active { background: #0f172a; color: #f59e0b; border-color: #0f172a; }

/* BULK PREVIEW */
.bulk-preview-bar {
  background: rgba(245,158,11,0.08);
  border: 1.5px solid rgba(245,158,11,0.25);
  border-radius: 14px; padding: 12px 16px;
  display: flex; align-items: center;
  font-size: 13px; font-weight: 600; flex-wrap: wrap; gap: 8px;
  color: var(--text-main);
}
.bulk-chips { display: flex; gap: 6px; flex-wrap: wrap; }
.bulk-chip { background: #f59e0b; color: #0f172a; font-size: 10px; font-weight: 800; padding: 3px 10px; border-radius: 8px; }
.bulk-chip.more { background: #0f172a; color: white; }

/* FORM FOOTER */
.form-footer-pro {
  display: flex; justify-content: space-between; align-items: center;
  padding-top: 28px; border-top: 1px solid var(--border-color);
  gap: 16px; flex-wrap: wrap;
}
.security-cluster { display: flex; align-items: center; gap: 14px; }
.security-icon-box {
  width: 42px; height: 42px;
  background: var(--bg-hover); border: 1px solid var(--border-color);
  border-radius: 14px; display: flex; align-items: center; justify-content: center;
  color: #f59e0b; font-size: 16px;
}
.security-label { font-size: 13px; font-weight: 800; color: var(--text-main); }
.security-sub   { font-size: 11px; color: var(--text-muted); margin-top: 2px; }

/* PRIMARY BUTTON */
.btn-enigma-primary {
  background: #0f172a; color: white; border: none;
  padding: 14px 28px; border-radius: 18px; font-weight: 800;
  position: relative; overflow: hidden; cursor: pointer; font-family: inherit;
  transition: var(--transition);
}
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2;
  display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.4; cursor: not-allowed; }

.btn-dots-loader { display: flex; gap: 5px; align-items: center; padding: 0 8px; }
.btn-dots-loader span {
  width: 6px; height: 6px; background: white; border-radius: 50%;
  animation: dots 1.2s ease-in-out infinite;
}
.btn-dots-loader span:nth-child(2) { animation-delay: 0.2s; }
.btn-dots-loader span:nth-child(3) { animation-delay: 0.4s; }
@keyframes dots { 0%,80%,100% { transform: scale(0.6); opacity: 0.4; } 40% { transform: scale(1); opacity: 1; } }

/* ════════════════════════════════════════════════════════
   SIDEBAR CARDS
════════════════════════════════════════════════════════ */
.enigma-dark-card {
  background: #0f172a; border-radius: 28px; padding: 32px;
  position: relative; overflow: hidden;
  box-shadow: 0 16px 40px rgba(15,23,42,0.25);
}
.enigma-dark-card::before {
  content: ''; position: absolute; top: -60px; right: -60px;
  width: 200px; height: 200px; background: #f59e0b; opacity: 0.06; border-radius: 50%;
}
.info-card-top { display: flex; align-items: center; gap: 14px; }
.info-icon-wrap {
  width: 44px; height: 44px; background: rgba(245,158,11,0.15); color: #f59e0b;
  border-radius: 14px; display: flex; align-items: center; justify-content: center;
  font-size: 18px; flex-shrink: 0;
}
.text-muted-light { color: #64748b; font-size: 13px; line-height: 1.7; }
.luxury-divider   { height: 1px; background: rgba(255,255,255,0.08); }
.info-features    { display: flex; flex-direction: column; gap: 14px; }
.info-feature-item { display: flex; align-items: flex-start; gap: 14px; }
.feat-dot  { width: 8px; height: 8px; background: #f59e0b; border-radius: 50%; margin-top: 4px; flex-shrink: 0; }
.feat-label { font-size: 10px; font-weight: 800; color: #475569; text-transform: uppercase; letter-spacing: 0.5px; }
.feat-value { font-size: 13px; font-weight: 700; color: #e2e8f0; margin-top: 2px; }

.stats-title { font-size: 10px; font-weight: 800; color: var(--text-muted); letter-spacing: 2px; }
.stats-bento-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.bento-stat-box {
  background: var(--bg-hover); border-radius: 16px; padding: 16px 12px;
  text-align: center; border: 1px solid var(--border-color);
}
.bento-stat-box.highlight { background: rgba(245,158,11,0.08); border-color: rgba(245,158,11,0.2); }
.bento-val { font-size: 28px; font-weight: 900; color: var(--text-main); display: block; line-height: 1; }
.bento-lbl { font-size: 9px; font-weight: 700; color: var(--text-muted); text-transform: uppercase; margin-top: 4px; display: block; }
.bento-stat-full {
  grid-column: 1/-1; background: #0f172a; color: #f59e0b;
  border-radius: 14px; padding: 12px 16px; text-align: center;
  font-size: 11px; font-weight: 800;
  display: flex; align-items: center; justify-content: center;
}

/* EMAIL PREVIEW */
.preview-header-bar {
  background: var(--bg-hover); padding: 12px 16px;
  border-bottom: 1px solid var(--border-color);
  display: flex; align-items: center; gap: 10px;
}
.preview-dots { display: flex; gap: 6px; }
.pd { width: 10px; height: 10px; border-radius: 50%; }
.pd.red    { background: #ff5f56; }
.pd.yellow { background: #ffbd2e; }
.pd.green  { background: #27c93f; }
.preview-label { font-size: 11px; font-weight: 700; color: var(--text-muted); }
.preview-logo-row { display: flex; align-items: center; gap: 8px; }
.prev-logo-box {
  width: 28px; height: 28px; background: #f59e0b; color: #0f172a;
  border-radius: 8px; display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 13px;
}
.prev-brand { font-size: 13px; color: var(--text-main); }
.mock-line { height: 6px; background: var(--border-color); border-radius: 4px; margin-bottom: 10px; }
.mock-btn-preview {
  background: #0f172a; color: #f59e0b; text-align: center;
  font-size: 11px; font-weight: 800; padding: 12px 16px; border-radius: 12px;
}

/* ════════════════════════════════════════════════════════
   TABLE SHARED
════════════════════════════════════════════════════════ */
.table-header-elite {
  display: flex; justify-content: space-between; align-items: center;
  background: var(--bg-hover); padding: 18px 24px;
  border-bottom: 1px solid var(--border-color);
  font-size: 11px; font-weight: 800; color: var(--text-muted); letter-spacing: 1.5px;
}
.table-header-icon {
  width: 28px; height: 28px; background: #0f172a; color: #f59e0b;
  border-radius: 8px; display: flex; align-items: center; justify-content: center; font-size: 12px;
}
.table-header-icon.recycle-icon { background: var(--danger); }
.table-badge-count {
  background: rgba(245,158,11,0.12); color: #d97706;
  padding: 4px 12px; border-radius: 100px; font-size: 11px; font-weight: 800;
}
.table-badge-count.danger-badge { background: var(--danger-bg); color: var(--danger); }

.elite-table thead th {
  border: none; font-size: 10px; font-weight: 800; color: var(--text-muted);
  letter-spacing: 1px; padding: 14px 24px; text-transform: uppercase;
  background: transparent;
}
.elite-table td {
  padding: 16px 24px; border-top: 1px solid var(--border-color);
  font-size: 13px; vertical-align: middle; color: var(--text-main);
}
.elite-table tbody tr { transition: background 0.2s; }
.elite-table tbody tr:hover { background: var(--bg-hover); }

.email-cell  { display: flex; align-items: center; gap: 10px; }
.email-avatar {
  width: 32px; height: 32px; background: #0f172a; color: #f59e0b;
  border-radius: 10px; display: flex; align-items: center; justify-content: center;
  font-size: 13px; font-weight: 800; flex-shrink: 0;
}
.email-avatar.recycle-avatar { background: var(--bg-hover); color: var(--text-muted); }
.status-pill-elite {
  background: rgba(245,158,11,0.12); color: #d97706;
  padding: 5px 12px; border-radius: 8px; font-weight: 800; font-size: 10px; letter-spacing: 0.5px;
}
.empty-state-row { text-align: center; padding: 48px 0 !important; }
.empty-icon { font-size: 32px; color: var(--text-light); margin-bottom: 12px; }
.empty-state-row p { color: var(--text-muted); font-size: 13px; margin: 0; }

/* BUTTONS TABLE */
.btn-icon-sm {
  width: 32px; height: 32px; border-radius: 10px;
  border: 1.5px solid var(--border-color); background: var(--bg-card);
  color: var(--text-muted); cursor: pointer; transition: var(--transition);
  font-size: 0.75rem; display: flex; align-items: center; justify-content: center;
}
.btn-icon-sm.danger:hover { background: var(--danger-bg); color: var(--danger); border-color: var(--danger); }

.btn-outline-pro {
  background: var(--bg-card); color: var(--text-main);
  border: 1.5px solid var(--border-color);
  padding: 10px 18px; border-radius: 14px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; transition: var(--transition); font-family: inherit;
}
.btn-outline-pro:hover:not(:disabled) { border-color: var(--text-main); }
.btn-outline-pro:disabled { opacity: 0.4; cursor: not-allowed; }

.btn-danger-outline {
  background: var(--bg-card); color: var(--danger);
  border: 1.5px solid var(--danger-bg);
  padding: 10px 18px; border-radius: 14px; font-weight: 800; font-size: 0.8rem;
  cursor: pointer; transition: var(--transition); font-family: inherit;
}
.btn-danger-outline:hover:not(:disabled) { background: var(--danger-bg); border-color: var(--danger); }
.btn-danger-outline:disabled { opacity: 0.4; cursor: not-allowed; }

/* SEARCH */
.search-inline-box {
  display: flex; align-items: center;
  background: var(--bg-card); border: 1.5px solid var(--border-color);
  border-radius: 14px; padding: 0 14px; gap: 10px; color: var(--text-muted);
}
.search-inline-input {
  border: none; outline: none; background: transparent;
  padding: 10px 0; font-weight: 700; font-size: 0.85rem;
  width: 160px; font-family: inherit; color: var(--text-main);
}
.btn-clear-search { border: none; background: transparent; color: var(--text-muted); padding: 0; cursor: pointer; }

/* SECTION TITLE (texte adapté dark) */
.section-title-dark { color: var(--text-main); }

/* ════════════════════════════════════════════════════════
   RECYCLE VIEW
════════════════════════════════════════════════════════ */
.recycle-header-icon {
  width: 36px; height: 36px; background: var(--danger-bg); color: var(--danger);
  border-radius: 12px; display: flex; align-items: center; justify-content: center; font-size: 14px;
}
.recycle-info-bar {
  background: rgba(245,158,11,0.08); border: 1.5px solid rgba(245,158,11,0.25);
  border-radius: 14px; padding: 14px 20px;
  display: flex; align-items: center;
  font-size: 13px; font-weight: 600; flex-wrap: wrap; gap: 8px;
  color: var(--text-main);
}
.recycle-empty-state {
  background: var(--bg-card); border-radius: 32px; padding: 80px 40px;
  text-align: center; border: 2px dashed var(--border-color);
  display: flex; flex-direction: column; align-items: center;
}
.recycle-empty-icon {
  width: 80px; height: 80px; border-radius: 50%;
  background: var(--bg-hover); color: var(--text-light);
  display: flex; align-items: center; justify-content: center; font-size: 2rem;
}
.recycle-row { opacity: 0.85; }
.recycle-row:hover { opacity: 1; }
.opacity-60 { opacity: 0.7; }
.deleted-badge {
  font-size: 9px; font-weight: 900; background: var(--danger-bg); color: var(--danger);
  padding: 2px 6px; border-radius: 4px; margin-top: 2px; display: inline-block;
}
.btn-restore {
  background: var(--success-bg); color: var(--success);
  border: 1.5px solid rgba(16,185,129,0.3);
  padding: 6px 14px; border-radius: 10px; font-size: 11px; font-weight: 800;
  cursor: pointer; transition: var(--transition); font-family: inherit; white-space: nowrap;
}
.btn-restore:hover { background: rgba(16,185,129,0.2); border-color: var(--success); }

/* ════════════════════════════════════════════════════════
   CONFIRM MODAL
════════════════════════════════════════════════════════ */
.quantum-vault-overlay {
  position: fixed; inset: 0; background: var(--bg-overlay);
  backdrop-filter: blur(10px); z-index: 2000;
  display: flex; align-items: center; justify-content: center;
}
.confirm-modal {
  background: var(--bg-card); border-radius: 32px; padding: 40px;
  width: 420px; max-width: 95vw; text-align: center;
  box-shadow: var(--shadow-lg); border: 1px solid var(--border-color);
}
.confirm-icon { display: flex; justify-content: center; }
.btn-qv-cancel {
  background: var(--bg-hover); color: var(--text-muted); border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
  transition: var(--transition);
}
.btn-qv-cancel:hover { background: var(--border-color); }
.btn-confirm-danger {
  background: var(--danger); color: white; border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
  transition: var(--transition);
}
.btn-confirm-danger:hover { opacity: 0.9; }

/* ════════════════════════════════════════════════════════
   TOAST
════════════════════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: #0f172a; color: white; padding: 20px 30px; border-radius: 20px;
  display: flex; align-items: center; gap: 15px; z-index: 3000;
  border-left: 5px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.25);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: var(--danger); }
.t-warn    { border-left-color: #f59e0b; }
.t-ico { font-size: 1.3rem; }
.t-body strong { font-size: 0.7rem; letter-spacing: 1px; opacity: 0.6; }
.t-body p { color: #94a3b8; font-size: 0.82rem; }

/* ════════════════════════════════════════════════════════
   TRANSITIONS
════════════════════════════════════════════════════════ */
.toast-slide-enter-active { animation: slideIn 0.4s ease-out; }
.toast-slide-leave-active  { animation: slideIn 0.3s ease-in reverse; }
@keyframes slideIn { from { transform: translateX(120%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
.modal-quantum-enter-active { animation: zoomIn 0.25s ease-out; }
.modal-quantum-leave-active  { animation: zoomIn 0.2s ease-in reverse; }
@keyframes zoomIn { from { opacity: 0; transform: scale(0.92); } to { opacity: 1; transform: scale(1); } }

/* ════════════════════════════════════════════════════════
   PAGINATION — identique à CampagneView
════════════════════════════════════════════════════════ */
.pagination-bar {
  display: flex; align-items: center; justify-content: space-between;
  padding: 16px 24px; border-top: 1px solid var(--border-color);
  flex-wrap: wrap; gap: 12px; background: var(--bg-hover);
}
.pagination-info {
  display: flex; align-items: center; gap: 6px;
  font-size: 12px; color: var(--text-muted); font-weight: 600;
}
.pagination-info strong { color: var(--text-main); font-weight: 900; }
.pagination-controls { display: flex; align-items: center; gap: 4px; }
.pg-btn {
  width: 34px; height: 34px; border-radius: 10px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-card); color: var(--text-muted);
  cursor: pointer; font-size: 12px;
  display: flex; align-items: center; justify-content: center;
  transition: var(--transition); font-family: inherit;
}
.pg-btn:hover:not(:disabled) { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
.pg-btn:disabled { opacity: 0.35; cursor: not-allowed; }
.pg-numbers { display: flex; align-items: center; gap: 3px; margin: 0 4px; }
.pg-num {
  min-width: 34px; height: 34px; padding: 0 6px;
  border-radius: 10px; border: 1.5px solid transparent;
  background: transparent; color: var(--text-muted);
  cursor: pointer; font-size: 12px; font-weight: 800;
  display: flex; align-items: center; justify-content: center;
  transition: var(--transition); font-family: inherit;
}
.pg-num:hover:not(:disabled):not(.ellipsis) { background: var(--bg-hover); border-color: var(--border-color); color: var(--text-main); }
.pg-num.active { background: #0f172a; color: #f59e0b; border-color: #0f172a; box-shadow: 0 4px 12px rgba(15,23,42,0.18); }
.pg-num.ellipsis { cursor: default; opacity: 0.5; }
.pagination-size { display: flex; align-items: center; gap: 8px; }
.pg-size-label { font-size: 11px; font-weight: 800; color: var(--text-muted); letter-spacing: 0.5px; }
.pg-size-select {
  padding: 6px 10px; border-radius: 10px; border: 1.5px solid var(--border-color);
  background: var(--bg-card); font-size: 12px; font-weight: 800; color: var(--text-main);
  cursor: pointer; outline: none; font-family: inherit;
}
.pg-size-select:focus { border-color: #f59e0b; }

/* ════════════════════════════════════════════════════════
   MISC
════════════════════════════════════════════════════════ */
.fw-700 { font-weight: 700 !important; }
.fw-900 { font-weight: 900 !important; }
.text-danger { color: var(--danger) !important; }

/* ════════════════════════════════════════════════════════
   DARK MODE OVERRIDES
   (reprend exactement le pattern de CampagneView)
════════════════════════════════════════════════════════ */
[data-theme="dark"] .enigma-invite-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .premium-title      { color: #f0f6fc; }
[data-theme="dark"] .section-title-dark { color: #f0f6fc; }
[data-theme="dark"] .form-section-title { color: #f0f6fc; }
[data-theme="dark"] .breadcrumb-pro .current { color: #f0f6fc; }

/* Cards */
[data-theme="dark"] .enigma-card { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .stat-card-premium { background: rgba(22,27,34,0.7); border-color: rgba(255,255,255,0.05); }
[data-theme="dark"] .stat-value { color: #f0f6fc; }

/* Formulaires */
[data-theme="dark"] .enigma-field {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.1);
  color: #f0f6fc;
}
[data-theme="dark"] .enigma-field:focus {
  border-color: #d97706;
  background: rgba(255,255,255,0.08);
}
[data-theme="dark"] .enigma-field::placeholder { color: #4b5563; }

/* Mode tabs */
[data-theme="dark"] .mode-tab {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.1);
  color: #94a3b8;
}
[data-theme="dark"] .mode-tab.active { background: #0f172a; color: #f59e0b; border-color: #0f172a; }
[data-theme="dark"] .mode-tab:hover  { border-color: #d97706; color: #f0f6fc; }

/* Toggle cluster */
[data-theme="dark"] .view-toggle-cluster {
  background: #161b22;
  border-color: rgba(255,255,255,0.08);
}
[data-theme="dark"] .btn-view-toggle:hover { background: rgba(255,255,255,0.08); color: #f0f6fc; }

/* System live badge */
[data-theme="dark"] .system-live-badge {
  background: rgba(255,255,255,0.05);
  border-color: rgba(255,255,255,0.08);
  color: #34d399;
}

/* Stats bento */
[data-theme="dark"] .bento-stat-box { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .bento-val      { color: #f0f6fc; }

/* Preview bars */
[data-theme="dark"] .preview-header-bar { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .prev-brand         { color: #f0f6fc; }
[data-theme="dark"] .mock-line          { background: rgba(255,255,255,0.08); }

/* Tables */
[data-theme="dark"] .table-header-elite { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .elite-table td     { border-top-color: rgba(255,255,255,0.05); color: #f0f6fc; }
[data-theme="dark"] .elite-table tbody tr:hover { background: rgba(255,255,255,0.04); }
[data-theme="dark"] .elite-table thead th { color: #8b949e; }

/* Security cluster */
[data-theme="dark"] .security-icon-box { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .security-label    { color: #f0f6fc; }

/* Form footer */
[data-theme="dark"] .form-footer-pro { border-top-color: rgba(255,255,255,0.08); }

/* Steps */
[data-theme="dark"] .step-bubble { background: rgba(255,255,255,0.08); }
[data-theme="dark"] .step-line   { background: rgba(255,255,255,0.1); }

/* Buttons */
[data-theme="dark"] .btn-icon-sm      { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .btn-outline-pro  { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }
[data-theme="dark"] .btn-outline-pro:hover:not(:disabled) { border-color: #d97706; background: rgba(217,119,6,0.1); }
[data-theme="dark"] .btn-danger-outline { background: rgba(255,255,255,0.05); border-color: rgba(248,113,113,0.2); color: #f87171; }
[data-theme="dark"] .btn-danger-outline:hover:not(:disabled) { background: rgba(248,113,113,0.1); }

/* Search */
[data-theme="dark"] .search-inline-box { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .search-inline-input { color: #f0f6fc; }

/* Bulk preview */
[data-theme="dark"] .bulk-preview-bar { background: rgba(245,158,11,0.08); border-color: rgba(245,158,11,0.2); color: #f0f6fc; }

/* Recycle */
[data-theme="dark"] .recycle-empty-state { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .recycle-info-bar    { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.15); color: #f0f6fc; }

/* Confirm modal */
[data-theme="dark"] .confirm-modal { background: #161b22; border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .btn-qv-cancel { background: rgba(255,255,255,0.08); color: #94a3b8; }
[data-theme="dark"] .btn-qv-cancel:hover { background: rgba(255,255,255,0.12); }

/* Pagination */
[data-theme="dark"] .pagination-bar  { background: rgba(255,255,255,0.02); border-top-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .pagination-info { color: #64748b; }
[data-theme="dark"] .pagination-info strong { color: #f0f6fc; }
[data-theme="dark"] .pg-btn { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #94a3b8; }
[data-theme="dark"] .pg-btn:hover:not(:disabled) { background: #f59e0b; border-color: #f59e0b; color: #0f172a; }
[data-theme="dark"] .pg-num { color: #64748b; }
[data-theme="dark"] .pg-num:hover:not(:disabled):not(.ellipsis) { background: rgba(255,255,255,0.08); border-color: rgba(255,255,255,0.15); color: #f0f6fc; }
[data-theme="dark"] .pg-num.active { background: #f59e0b; color: #0f172a; border-color: #f59e0b; }
[data-theme="dark"] .pg-size-select { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.1); color: #f0f6fc; }

/* ════════════════════════════════════════════════════════
   RESPONSIVE
════════════════════════════════════════════════════════ */
@media (max-width: 991px) {
  .studio-form-card { padding: 48px 24px 32px; }
  .bot-anchor  { top: -60px; right: 16px; }
  .master-bot  { width: 110px; }
  .form-footer-pro { flex-direction: column; align-items: flex-start; }
}
@media (max-width: 768px) {
  .premium-title { font-size: 1.6rem; }
  .pagination-bar { flex-direction: column; align-items: flex-start; gap: 10px; }
  .pagination-controls { flex-wrap: wrap; }
}
@media (max-width: 576px) {
  .enigma-toast { left: 16px; right: 16px; bottom: 16px; }
  .stats-bento-grid { grid-template-columns: 1fr; }
  .view-toggle-cluster { gap: 2px; padding: 3px; }
}
</style>