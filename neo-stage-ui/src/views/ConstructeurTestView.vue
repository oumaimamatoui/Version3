<template>
  <div class="enigma-studio-root d-flex overflow-hidden" @mousemove="handleParallax">
    
    <!-- LAYER 0 : FONDATIONS CYBER-TECH -->
    <div class="cyber-engine-bg">
        <div class="bg-vignette"></div>
        <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
        <div class="glow-orb orb-blue" :style="orbStyle(0.015)"></div>
        <div class="quantum-grid"></div>
    </div>

    <!-- NAVIGATION LATERALE PRO -->
    <AppSidebar />

    <div class="main-orchestrator flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar" @scroll="handleScroll">
        
        <!-- HEADER DYNAMIQUE "FROSTED ARCHITECT" -->
        <header class="studio-header-v2" :class="{ 'is-docked': isScrolled }">
            <div class="header-content-inner p-4 p-xl-5 pb-4">
                <div class="d-flex justify-content-between align-items-center mb-5">

                    <div class="brand-logic-block animate__animated animate__fadeInLeft">
                        <div class="d-flex align-items-center gap-4">
                            <div class="ai-robot-terminal">
                                <svg viewBox="0 0 60 60" fill="none" xmlns="http://www.w3.org/2000/svg" width="56">
                                    <rect x="12" y="10" width="36" height="34" rx="11" fill="white" opacity=".96"/>
                                    <rect x="16" y="18" width="28" height="12" rx="6" fill="#0f172a"/>
                                    <circle cx="22" cy="24" r="3.5" fill="#f59e0b">
                                        <animate attributeName="opacity" values="1;0.15;1" dur="3s" repeatCount="indefinite"/>
                                    </circle>
                                    <circle cx="38" cy="24" r="3.5" fill="#f59e0b">
                                        <animate attributeName="opacity" values="1;0.15;1" dur="3s" begin="0.4s" repeatCount="indefinite"/>
                                    </circle>
                                    <rect x="18" y="36" width="24" height="3.5" rx="1.75" fill="white" opacity=".25"/>
                                </svg>
                            </div>
                            <div class="brand-text-logic">
                                <h1 class="main-title-v2">Evalua<span>Architect</span> <span class="v-badge">ULTIMATE v6.5</span></h1>
                                <p class="brand-subtitle-v2"><i class="fa-solid fa-code-branch me-2"></i> TERMINAL DE CONFIGURATION UML & DÉPLOIEMENT SQL</p>
                            </div>
                        </div>
                    </div>

                    <div class="global-actions-cluster d-flex gap-3 align-items-center animate__animated animate__fadeInRight">

                        <!-- DIAGNOSTIC HEALTH ENGINE -->
                        <div class="health-core-widget" v-tooltip="'Analyse de viabilité structurelle'">
                            <div class="health-ring-box">
                                <svg width="48" height="48" viewBox="0 0 48 48">
                                    <circle class="ring-track" cx="24" cy="24" r="19"></circle>
                                    <circle class="ring-fill" cx="24" cy="24" r="19" :style="healthRingStyle"></circle>
                                </svg>
                                <span class="health-percent">{{ architectureHealth }}%</span>
                            </div>
                            <div class="health-label-box">
                                <span class="h-main">SYSTÈME</span>
                                <span class="h-sub" :style="{ color: healthColor }">{{ healthStatusText }}</span>
                            </div>
                        </div>

                        <div class="action-divider"></div>

                        <div class="sync-indicator-v2" :class="isSaving ? 'is-syncing' : 'is-stable'">
                            <span class="status-icon"></span>
                            <span class="status-txt">{{ isSaving ? 'SYNCHRONISATION...' : 'TERMINAL PRÊT' }}</span>
                        </div>

                        <button @click="saveDraftAsynchronous" class="btn-enigma-outline">
                            <i class="fa-solid fa-database me-2"></i> BROUILLON
                        </button>
                        <button @click="publishToProduction" :disabled="!isReadyToPublish || isPublishing" class="btn-enigma-primary">
                            <div class="btn-content">
                                <i v-if="!isPublishing" class="fa-solid fa-rocket me-2"></i>
                                <span v-else class="spinner-border spinner-border-sm me-2"></span>
                                DÉPLOYER L'ARCHITECTURE
                            </div>
                            <div class="btn-glow"></div>
                        </button>
                    </div>
                </div>

                <!-- STEPPER V8 (PRECISION LINE) -->
                <nav class="stepper-precision-v8">
                    <div class="stepper-line-bg">
                        <div class="stepper-line-fill" :style="{ width: stepperProgress + '%' }"></div>
                    </div>
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
            
            <!-- COLONNE ÉDITEUR (8 COL) -->
            <div class="col-xl-8">
              
              <!-- SECTION 1 : CONFIGURATION STRUCTURELLE -->
              <section v-if="currentStep === 1" class="engine-pane animate__animated animate__fadeIn">
                  <div class="enigma-card p-5">
                      <div class="pane-header-v2 mb-5">
                          <div class="icon-box-v2 amber"><i class="fa-solid fa-layer-group"></i></div>
                          <div>
                              <h4 class="fw-900 m-0">Architecture Noyau</h4>
                              <p class="text-muted m-0">Définition des métadonnées et protocoles de validation.</p>
                          </div>
                      </div>

                      <div class="row g-5">
                          <div class="col-12">
                              <div class="enigma-input-wrap">
                                  <label>NOM DE L'INSTANCE D'ÉVALUATION *</label>
                                  <input type="text" v-model="studio.questionnaire.titre" placeholder="ex: Senior Fullstack Engineer - System Design Assessment" class="enigma-field">
                                  <span class="field-hint">Ce titre sera visible par tous les candidats ciblés.</span>
                              </div>
                          </div>
                          <div class="col-md-6">
                              <div class="enigma-input-wrap">
                                  <label>DOMAINE D'EXPERTISE</label>
                                  <div class="select-enigma-custom">
                                      <select v-model="studio.questionnaire.categorie" class="enigma-field">
                                          <option v-for="cat in categories" :key="cat" :value="cat">{{ cat }}</option>
                                      </select>
                                      <i class="fa-solid fa-chevron-down"></i>
                                  </div>
                              </div>
                          </div>
                          <div class="col-md-6">
                              <div class="enigma-input-wrap">
                                  <label>VÉLOCITÉ ESTIMÉE (MINUTAGE)</label>
                                  <div class="input-with-ico">
                                      <i class="fa-solid fa-stopwatch"></i>
                                      <input type="number" v-model.number="studio.questionnaire.duree" class="enigma-field has-ico">
                                  </div>
                              </div>
                          </div>
                          <div class="col-12 mt-5">
                              <div class="admissibility-dashboard">
                                  <div class="d-flex justify-content-between align-items-center mb-4">
                                      <div class="lbl-group">
                                          <label class="m-0 fw-900">SEUIL D'ADMISSIBILITÉ</label>
                                          <p class="m-0 small text-muted">Score minimal requis pour le statut "Validé".</p>
                                      </div>
                                      <div class="score-display-v2">{{ studio.questionnaire.scoreReussite }}%</div>
                                  </div>
                                  <div class="range-enigma-wrap">
                                      <input type="range" v-model="studio.questionnaire.scoreReussite" min="1" max="100" class="enigma-range">
                                      <div class="range-visual-track" :style="{ width: studio.questionnaire.scoreReussite + '%' }"></div>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>
              </section>

              <!-- SECTION 2 : PUITS DE QUESTIONS -->
              <section v-if="currentStep === 2" class="engine-pane animate__animated animate__fadeIn">
                  <div class="orchestrator-head-v2 mb-4 d-flex justify-content-between align-items-end">
                      <div class="oh-info">
                          <h5 class="fw-900 m-0">Composition Logicielle</h5>
                          <p class="m-0 small text-muted">Ordonnancement atomique des actifs ({{ studio.questions.length }})</p>
                      </div>
                      <div class="oh-actions d-flex gap-3">
                          <button @click="modals.iaPrompt = true" class="btn-ia-action-v2">
                              <i class="fa-solid fa-microchip me-2"></i> IA GENERATOR
                          </button>
                          <button @click="openDeepBankModal" class="btn-bank-action-v2">
                              <i class="fa-solid fa-vault me-2"></i> BANQUE D'ACTIFS
                          </button>
                      </div>
                  </div>

                  <!-- ETAT VIDE -->
                  <div v-if="studio.questions.length === 0" class="empty-assets-v2 p-5">
                      <div class="empty-icon-pulse mb-4">
                          <div class="pulse-ring"></div>
                          <i class="fa-solid fa-layer-group"></i>
                      </div>
                      <h4 class="fw-800">Le puit est vierge</h4>
                      <p class="text-muted">Commencez l'ingénierie en important des questions UML.</p>
                      <button @click="openDeepBankModal" class="btn-enigma-outline-amber mt-3">EXPLORER LA BANQUE</button>
                  </div>

                  <!-- LISTE DRAGGABLE -->
                  <div v-else class="assets-scroll-v8 custom-scrollbar">
                      <draggable v-model="studio.questions" item-key="id" handle=".drag-node-handle" ghost-class="asset-ghost-v8">
                          <template #item="{ element, index }">
                              <div class="asset-card-v8 animate__animated animate__fadeInUp" :style="{ animationDelay: (index * 0.05) + 's' }">
                                  <div class="drag-node-handle"><i class="fa-solid fa-grip-vertical"></i></div>
                                  <div class="asset-index-v8">0{{ index + 1 }}</div>
                                  <div class="asset-core-v8">
                                      <h6 class="asset-title-v8 text-truncate">{{ element.texte }}</h6>
                                      <div class="asset-tags-v8">
                                          <span class="t-pill weight">{{ element.poids }} PTS</span>
                                          <span class="t-pill tech">{{ element.categorie }}</span>
                                          <span class="t-pill status" :class="element.difficulty?.toLowerCase()">{{ element.difficulty }}</span>
                                      </div>
                                  </div>
                                  <div class="asset-ctrls-v8">
                                      <button @click="studio.questions.splice(index, 1)" class="btn-remove-v8" v-tooltip="'Retirer l\'actif'">
                                          <i class="fa-solid fa-xmark"></i>
                                      </button>
                                  </div>
                              </div>
                          </template>
                      </draggable>
                  </div>
              </section>

              <!-- SECTION 3 : POOL DE TALENTS -->
              <section v-if="currentStep === 3" class="engine-pane animate__animated animate__fadeIn">
                  <div class="talent-hub-search-v2 mb-5">
                      <div class="search-enigma-box">
                          <i class="fa-solid fa-magnifying-glass"></i>
                          <input type="text" v-model="searchCandQuery" placeholder="Filtrer par stack, nom ou e-mail professionnel..." class="enigma-search-field">
                      </div>
                      <div class="batch-counter-v2">
                          <span class="val">{{ selectedCandidatesIds.length }}</span>
                          <span class="lab">CANDIDATS</span>
                      </div>
                  </div>

                  <div class="talent-matrix-grid custom-scrollbar">
                      <div class="row g-4">
                          <div v-for="c in filteredCandidatePool" :key="c.id" class="col-md-6 col-lg-4">
                              <div :class="['talent-card-v8', { 'active': isSelectedCandidate(c.id) }]" @click="toggleCandidateTarget(c)">
                                  <div class="card-check-v8"><i class="fa-solid fa-check"></i></div>
                                  <div class="avatar-v8">{{ c.name[0] }}</div>
                                  <div class="data-v8">
                                      <div class="n">{{ c.name }}</div>
                                      <div class="e">{{ c.email }}</div>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>
              </section>

              <!-- SECTION 4 : PLANNING & PROTOCOLES -->
              <section v-if="currentStep === 4" class="engine-pane animate__animated animate__fadeIn">
                  <div class="enigma-card p-5">
                      <h5 class="fw-900 mb-5 text-navy"><i class="fa-solid fa-calendar-check text-amber me-3"></i>Planification de Campagne</h5>
                      <div class="row g-5">
                          <div class="col-md-6">
                              <div class="enigma-input-wrap">
                                  <label>OUVERTURE DU TERMINAL</label>
                                  <input type="datetime-local" v-model="studio.campagne.dateDebut" class="enigma-field">
                              </div>
                          </div>
                          <div class="col-md-6">
                              <div class="enigma-input-wrap">
                                  <label>FERMETURE DES ACCÈS</label>
                                  <input type="datetime-local" v-model="studio.campagne.dateFin" class="enigma-field">
                              </div>
                          </div>
                          <div class="col-12 mt-5">
                              <div class="protocol-security-matrix">
                                  <div class="protocol-row">
                                      <div class="p-icon"><i class="fa-solid fa-shield-halved"></i></div>
                                      <div class="p-data">
                                          <h6>Surveillance Anti-Cheat IA v2.0</h6>
                                          <p>Analyse des patterns de navigation et détection de fraude asynchrone.</p>
                                      </div>
                                      <div class="form-check form-switch enigma-switch">
                                          <input class="form-check-input" type="checkbox" v-model="studio.campagne.anticheat" checked>
                                      </div>
                                  </div>
                                  <div class="protocol-row">
                                      <div class="p-icon"><i class="fa-solid fa-eye"></i></div>
                                      <div class="p-data">
                                          <h6>Transparence RH & Feedback</h6>
                                          <p>Délivrer le rapport de performance au candidat dès la soumission finale.</p>
                                      </div>
                                      <div class="form-check form-switch enigma-switch">
                                          <input class="form-check-input" type="checkbox" v-model="studio.campagne.transparence" checked>
                                      </div>
                                  </div>
                              </div>
                          </div>
                      </div>
                  </div>
              </section>
            </div>

            <!-- PANNEAU DROIT : ANALYTICS HUD (4 COL) -->
            <div class="col-xl-4">
              <aside class="sidebar-analytics-engine">
                  <div class="analytics-hub-glass">
                      <div class="hub-header-v2">
                          <span class="hub-label">ARCHITECTURE DASHBOARD</span>
                          <h4 class="hub-title-v2 text-truncate">{{ studio.questionnaire.titre || 'UML SANS TITRE' }}</h4>
                          <div class="hub-status-box" :class="validationState.class">
                              <span class="pulse-dot"></span> {{ validationState.text }}
                          </div>
                      </div>

                      <div class="hub-body-v2 p-4">
                          <!-- BENTO KPI GRID -->
                          <div class="kpi-bento-grid">
                              <div class="bento-item">
                                  <span class="v">{{ studio.questions.length }}</span>
                                  <span class="l">ACTIFS</span>
                              </div>
                              <div class="bento-item highlight">
                                  <span class="v">{{ totalLogicPoints }}</span>
                                  <span class="l">POINTS</span>
                              </div>
                              <div class="bento-item">
                                  <span class="v">{{ selectedCandidatesIds.length }}</span>
                                  <span class="l">TARGETS</span>
                              </div>
                          </div>

                          <!-- STATS PROFONDES -->
                          <div class="deep-analytics-list mt-5">
                              <div class="d-row">
                                  <span class="k">Indice de Complexité</span>
                                  <div class="bolts-v8">
                                      <i v-for="s in 5" :key="s" class="fa-solid fa-bolt" :class="{ 'active': s <= complexCalculatedValue }"></i>
                                  </div>
                              </div>
                              <div class="d-row">
                                  <span class="k">Temps de Passage Est.</span>
                                  <span class="v">{{ estimatedTotalTime }} min</span>
                              </div>
                              <div class="d-row">
                                  <span class="k">Validité Structurelle</span>
                                  <span class="v text-amber fw-800">{{ architectureHealth }}%</span>
                              </div>
                          </div>

                          <!-- ACTIONS STEPPER -->
                          <div class="wizard-controls-v8 mt-5 pt-4">
                              <div class="d-flex gap-2">
                                  <button @click="prevStep" :disabled="currentStep === 1" class="btn-step-nav back">
                                      <i class="fa-solid fa-arrow-left"></i>
                                  </button>
                                  <button v-if="currentStep < 4" @click="nextStep" class="btn-step-nav next flex-grow-1">
                                      ÉTAPE SUIVANTE <i class="fa-solid fa-arrow-right ms-2"></i>
                                  </button>
                                  <button v-else @click="publishToProduction" :disabled="!isReadyToPublish" class="btn-step-deploy flex-grow-1">
                                      DÉPLOYER LA SESSION
                                  </button>
                              </div>
                              <p class="reset-link" @click="resetStudio">Réinitialiser l'architecture</p>
                          </div>
                      </div>
                  </div>

                  <!-- COACH IA TERMINAL -->
                  <div class="ia-coach-terminal mt-4">
                      <div class="robot-glow-container">
                          <svg viewBox="0 0 60 60" fill="none" xmlns="http://www.w3.org/2000/svg" width="60">
                              <rect x="12" y="10" width="36" height="34" rx="11" fill="white" opacity=".95"/>
                              <rect x="16" y="18" width="28" height="12" rx="6" fill="#1e293b"/>
                              <circle cx="22" cy="24" r="3" fill="#f59e0b">
                                  <animate attributeName="opacity" values="1;0.2;1" dur="2.5s" repeatCount="indefinite"/>
                              </circle>
                              <circle cx="38" cy="24" r="3" fill="#f59e0b">
                                  <animate attributeName="opacity" values="1;0.2;1" dur="2.5s" begin="0.5s" repeatCount="indefinite"/>
                              </circle>
                          </svg>
                      </div>
                      <div class="coach-text-v8">
                          <h6>Conseiller EvaluaArchitect</h6>
                          <p class="m-0 small">{{ coachAIAdvice }}</p>
                      </div>
                  </div>
              </aside>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- MODALE BANQUE D'ACTIFS -->
    <transition name="modal-quantum">
        <div v-if="modals.bank" class="quantum-vault-overlay" @click.self="closeDeepBank">
            <div class="quantum-vault-window animate__animated animate__zoomIn">
                
                <header class="qv-header">
                    <div class="qv-title">
                        <h2 class="m-0 fw-900">Bibliothèque d'Actifs Certifiés</h2>
                        <p class="m-0 text-muted small">Panier temporaire : {{ selectedItemsInBank.length }} questions en attente d'importation.</p>
                    </div>
                    <div class="qv-actions d-flex gap-3">
                        <button @click="closeDeepBank" class="btn-qv-cancel">ANNULER</button>
                        <button @click="confirmStudioSync" :disabled="selectedItemsInBank.length === 0" class="btn-qv-confirm">
                            IMPORTER LA SÉLECTION ({{ selectedItemsInBank.length }})
                        </button>
                    </div>
                </header>

                <div class="qv-layout">
                    
                    <aside class="qv-sidebar">
                        <div class="qv-search-area mb-5">
                            <label>RECHERCHE GLOBALE</label>
                            <div class="qv-search-wrap">
                                <i class="fa-solid fa-magnifying-glass"></i>
                                <input type="text" v-model="searchBankQuery" placeholder="SQL, React, Kafka...">
                            </div>
                        </div>
                        <div class="qv-filter-group mb-5">
                            <label>DIFFICULTÉ UML</label>
                            <div class="qv-pill-stack">
                                <button class="active">TOUS</button>
                                <button>JUNIOR</button>
                                <button>EXPERT</button>
                            </div>
                        </div>
                        <div class="qv-bank-stats mt-auto p-4 bg-light rounded-4">
                            <div class="d-flex justify-content-between mb-2"><span>Bibliothèque</span><strong>{{ bankGlobalReference.length }}</strong></div>
                            <div class="d-flex justify-content-between"><span>Dernière MAJ</span><strong>Live</strong></div>
                        </div>
                    </aside>

                    <section class="qv-list custom-scrollbar">
                        <div v-for="qRef in filteredBankReference" :key="qRef.id" 
                             @click="focusToInspectItem(qRef)"
                             :class="['qv-item-card', { 'active': currentInspectedBankItem?.id === qRef.id, 'checked': isCocheeInBank(qRef.id) }]">
                             <div class="qv-item-top">
                                 <span class="qv-badge" :class="qRef.difficulty?.toLowerCase()">{{ qRef.difficulty }}</span>
                                 <div class="qv-checkbox" @click.stop="toggleInBankPool(qRef)">
                                     <i :class="isCocheeInBank(qRef.id) ? 'fa-solid fa-check-circle' : 'fa-regular fa-circle'"></i>
                                 </div>
                             </div>
                             <h6 class="qv-item-text mt-3 text-truncate-2">{{ qRef.texte }}</h6>
                             <div class="qv-item-footer mt-4">
                                 <span class="pts"><strong>{{ qRef.poids }}</strong> PTS</span>
                                 <span class="time"><i class="fa-solid fa-clock me-1"></i> 4m</span>
                             </div>
                        </div>
                    </section>

                    <section class="qv-inspector">
                        <div v-if="currentInspectedBankItem" class="qv-inspector-content p-5 animate__animated animate__fadeIn">
                            <div class="qv-tabs mb-5">
                                <button @click="inspectorTab = 'edit'" :class="{ active: inspectorTab === 'edit' }">ÉDITION SOURCE</button>
                                <button @click="inspectorTab = 'preview'" :class="{ active: inspectorTab === 'preview' }">VUE CANDIDAT</button>
                            </div>

                            <div v-if="inspectorTab === 'edit'" class="qv-edit-pane">
                                <div class="qv-field-group mb-4">
                                    <label>ÉNONCÉ DE L'ACTIF</label>
                                    <textarea v-model="currentInspectedBankItem.texte" rows="4" class="enigma-field"></textarea>
                                </div>
                                <div class="qv-field-group mb-4">
                                    <label>STRUCTURE DES CHOIX & RÉPONSE CORRECTE</label>
                                    <div v-for="(val, kIdx) in currentInspectedBankItem.options" :key="kIdx" 
                                         :class="['qv-choice-row', { 'is-correct': currentInspectedBankItem.correctIndex === kIdx }]">
                                         <div class="qv-alpha" @click="currentInspectedBankItem.correctIndex = kIdx">
                                             {{ String.fromCharCode(65 + kIdx) }}
                                         </div>
                                         <input type="text" v-model="currentInspectedBankItem.options[kIdx]" class="qv-choice-input">
                                         <div class="qv-check-action" @click="currentInspectedBankItem.correctIndex = kIdx">
                                             <i class="fa-solid fa-check-circle"></i>
                                         </div>
                                    </div>
                                </div>
                                <div class="ia-justification-v8 p-4 rounded-4 bg-light border-start border-amber border-4">
                                    <h6 class="fw-900 mb-2"><i class="fa-solid fa-wand-magic-sparkles text-amber me-2"></i> JUSTIFICATION IA</h6>
                                    <textarea v-model="currentInspectedBankItem.explication" class="small text-muted bg-transparent border-0 w-100" rows="3"></textarea>
                                </div>
                            </div>

                            <div v-else class="qv-preview-pane">
                                <div class="device-mockup-v8">
                                    <div class="mockup-top">MODÉLISATION DÉPLOIEMENT</div>
                                    <div class="mockup-body p-4">
                                        <p class="mockup-q-txt">{{ currentInspectedBankItem.texte }}</p>
                                        <div class="mockup-options mt-4">
                                            <div v-for="opt in currentInspectedBankItem.options" :key="opt" class="mockup-opt-item">
                                                <div class="m-radio"></div> {{ opt }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div v-else class="qv-inspector-empty">
                             <i class="fa-solid fa-hand-pointer fa-4x mb-3"></i>
                             <p>Sélectionnez une question pour entrer en mode édition.</p>
                        </div>
                    </section>

                </div>
            </div>
        </div>
    </transition>

    <!-- NOTIFICATION SYSTEM -->
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
import { ref, reactive, computed, onMounted, watch } from 'vue';
import axios from 'axios';
import draggable from 'vuedraggable';

/* ============================================================
   I. CONFIGURATION & ÉTATS RÉACTIFS
   ============================================================ */

const API_ENDPOINT = "http://localhost:5172/api";

const currentStep = ref(1);
const inspectorTab = ref('edit'); 
const isSaving = ref(false);
const isPublishing = ref(false);
const isScrolled = ref(false);
const searchBankQuery = ref("");
const searchCandQuery = ref("");
const currentInspectedBankItem = ref(null);
const mousePos = reactive({ x: 0, y: 0 });

const categories = [
    "Backend Specialist (SQL/Node)", 
    "Frontend Architect (Vue 3)", 
    "Cybersecurity Infrastructure", 
    "DevOps Automation (K8s/AWS)", 
    "Data Analysis & AI Logic"
];

const studio = reactive({
    questionnaire: {
        id: '00000000-0000-0000-0000-000000000000',
        titre: '',
        categorie: categories[1],
        description: 'Session d\'évaluation certifiée UML v6.5.',
        duree: 45,
        scoreReussite: 70
    },
    campagne: {
        id: '00000000-0000-0000-0000-000000000000',
        dateDebut: '',
        dateFin: '',
        maxCandidats: 100,
        anticheat: true,
        transparence: true
    },
    questions: []
});

const steps = [
    { id: 1, label: 'Structure', sub: 'Paramètres SQL' },
    { id: 2, label: 'Ingénierie', sub: 'Puits d\'Actifs' },
    { id: 3, label: 'Target Pool', sub: 'Ciblage Profils' },
    { id: 4, label: 'Déploiement', sub: 'Matrix Planning' }
];

const bankGlobalReference = ref([]);
const candidateMasterPool = ref([]);
const selectedItemsInBank = ref([]);
const selectedCandidatesIds = ref([]);
const modals = reactive({ bank: false, iaPrompt: false });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });

/* ============================================================
   II. LOGIQUE MÉTIER & ANALYTICS
   ============================================================ */

const stepperProgress = computed(() => ((currentStep.value - 1) / (steps.length - 1)) * 100);
const totalLogicPoints = computed(() => studio.questions.reduce((a, b) => a + (Number(b.poids) || 0), 0));
const estimatedTotalTime = computed(() => studio.questions.length * 4);

const architectureHealth = computed(() => {
    let score = 0;
    if (studio.questionnaire.titre.length > 8) score += 20;
    if (studio.questions.length >= 4) score += 30;
    if (studio.questions.length >= 8) score += 20;
    if (selectedCandidatesIds.value.length > 0) score += 30;
    return score;
});

const healthStatusText = computed(() => {
    const h = architectureHealth.value;
    if (h > 80) return "STABLE";
    if (h > 40) return "CRITIQUE";
    return "VIERGE";
});

const healthColor = computed(() => {
    const h = architectureHealth.value;
    return h > 80 ? '#10b981' : h > 40 ? '#f59e0b' : '#ef4444';
});

const healthRingStyle = computed(() => {
    const circ = 2 * Math.PI * 19;
    const offset = circ - (architectureHealth.value / 100) * circ;
    return {
        strokeDasharray: `${circ} ${circ}`,
        strokeDashoffset: offset,
        stroke: healthColor.value
    };
});

const complexCalculatedValue = computed(() => {
    if(studio.questions.length === 0) return 0;
    const avg = totalLogicPoints.value / studio.questions.length;
    if (avg >= 16) return 5;
    if (avg >= 12) return 4;
    if (avg >= 8) return 3;
    return 2;
});

const coachAIAdvice = computed(() => {
    if (studio.questions.length === 0) return "Bonjour. Veuillez injecter au moins 5 actifs techniques pour valider la structure de base.";
    if (complexCalculatedValue.value >= 4 && studio.questionnaire.duree < 30) return "Alerte : Complexité trop élevée pour la durée définie. Envisagez +15 minutes.";
    if (architectureHealth.value < 60) return "Votre score de santé est sous le seuil optimal. Vérifiez les titres et le ciblage candidat.";
    return "Architecture certifiée ! Votre session est prête pour un déploiement SQL sécurisé.";
});

const filteredCandidatePool = computed(() => {
    const q = searchCandQuery.value.toLowerCase();
    return candidateMasterPool.value.filter(c => c.name.toLowerCase().includes(q) || c.email.toLowerCase().includes(q));
});

const filteredBankReference = computed(() => {
    const q = searchBankQuery.value.toLowerCase();
    return bankGlobalReference.value.filter(item => item.texte.toLowerCase().includes(q) || item.categorie.toLowerCase().includes(q));
});

const validationState = computed(() => {
    if(!studio.questionnaire.titre) return { text: 'TITRE MANQUANT', class: 'cl-err' };
    if(studio.questions.length < 3) return { text: 'PUITS FAIBLE', class: 'cl-warn' };
    if(selectedCandidatesIds.value.length === 0 && currentStep.value > 2) return { text: 'ZÉRO CIBLE', class: 'cl-warn' };
    return { text: 'SYSTÈME VALIDE', class: 'cl-success' };
});

const isReadyToPublish = computed(() => {
    return studio.questionnaire.titre.length > 5 && studio.questions.length >= 2 && selectedCandidatesIds.value.length > 0;
});

/* ============================================================
   III. LIFECYCLE
   ============================================================ */

onMounted(async () => {
    try {
        const [respQ, respC] = await Promise.all([
            axios.get(`${API_ENDPOINT}/Questions`),
            axios.get(`${API_ENDPOINT}/Candidates`)
        ]);

        bankGlobalReference.value = respQ.data.map(item => ({
            ...item,
            poids: item.poids || 10,
            difficulty: ['JUNIOR', 'INTERMÉDIAIRE', 'EXPERT'][Math.floor(Math.random() * 3)],
            correctIndex: 1,
            options: ["Option Alpha Standard", "Patron Certifié UML", "Middleware SQL Proxy", "Alternative Asynchrone v4"],
            explication: "Cette réponse dénote une compréhension profonde des cycles de vie asynchrones dans un environnement multi-tenant Cloud."
        }));

        candidateMasterPool.value = respC.data;
        if(bankGlobalReference.value.length === 0) generateSystemBackupMocks();

        const draft = localStorage.getItem('enigma_studio_draft');
        if(draft) Object.assign(studio, JSON.parse(draft));

    } catch (error) {
        showPulseToast("API non joignable. Mode Architecte Local.", "warn", "fa-solid fa-plug-circle-xmark");
        generateSystemBackupMocks();
    }
});

watch(studio, (newVal) => {
    localStorage.setItem('enigma_studio_draft', JSON.stringify(newVal));
    isSaving.value = true;
    setTimeout(() => { isSaving.value = false; }, 1000);
}, { deep: true });

/* ============================================================
   IV. NAVIGATION
   ============================================================ */

const jumpToStep = (id) => { if(id <= currentStep.value + 1) currentStep.value = id; };
const nextStep = () => { currentStep.value++; window.scrollTo(0,0); };
const prevStep = () => { currentStep.value--; window.scrollTo(0,0); };
const handleScroll = (e) => { isScrolled.value = e.target.scrollTop > 100; };
const handleParallax = (e) => {
    mousePos.x = (e.clientX - window.innerWidth/2) / 20;
    mousePos.y = (e.clientY - window.innerHeight/2) / 20;
};
const orbStyle = (factor) => ({
    transform: `translate(${mousePos.x * factor * 10}px, ${mousePos.y * factor * 10}px)`
});
const resetStudio = () => {
    if(confirm("Action irréversible : Réinitialiser tout le schéma d'architecture ?")) {
        localStorage.removeItem('enigma_studio_draft');
        window.location.reload();
    }
};

/* ============================================================
   V. BANK OPERATIONS
   ============================================================ */

const openDeepBankModal = () => { modals.bank = true; document.body.style.overflow = 'hidden'; };
const closeDeepBank = () => { modals.bank = false; selectedItemsInBank.value = []; currentInspectedBankItem.value = null; document.body.style.overflow = 'auto'; };
const focusToInspectItem = (item) => { currentInspectedBankItem.value = item; };
const isCocheeInBank = (id) => selectedItemsInBank.value.some(q => q.id === id);
const toggleInBankPool = (q) => {
    const idx = selectedItemsInBank.value.findIndex(item => item.id === q.id);
    if(idx > -1) selectedItemsInBank.value.splice(idx, 1);
    else selectedItemsInBank.value.push(JSON.parse(JSON.stringify(q)));
};
const confirmStudioSync = () => {
    const dataClone = JSON.parse(JSON.stringify(selectedItemsInBank.value)).map(obj => ({
        ...obj,
        id: `enigma-ref-${Math.random().toString(16)}`
    }));
    studio.questions.push(...dataClone);
    showPulseToast(`${dataClone.length} actifs injectés avec succès.`, "success", "fa-solid fa-bolt-lightning");
    closeDeepBank();
};
const toggleCandidateTarget = (c) => {
    const idx = selectedCandidatesIds.value.indexOf(c.id);
    idx > -1 ? selectedCandidatesIds.value.splice(idx, 1) : selectedCandidatesIds.value.push(c.id);
};
const isSelectedCandidate = (id) => selectedCandidatesIds.value.includes(id);

/* ============================================================
   VI. DÉPLOIEMENT
   ============================================================ */

const publishToProduction = async () => {
    isPublishing.value = true;
    try {
        const respQuest = await axios.post(`${API_ENDPOINT}/Questionnaires`, studio.questionnaire);
        const masterId = respQuest.data.id;
        for (const qObj of studio.questions) {
            await axios.post(`${API_ENDPOINT}/Questions`, { ...qObj, questionnaireId: masterId });
        }
        const campSchema = {
            ...studio.campagne,
            nom: `ARCHITECTURE RH DEPLOYEE : ${studio.questionnaire.titre}`,
            questionnaireId: masterId,
            entrepriseId: "00000000-0000-0000-0000-000000000000"
        };
        const respCamp = await axios.post(`${API_ENDPOINT}/Campagnes`, campSchema);
        const campId = respCamp.data.id;
        for(let userId of selectedCandidatesIds.value) {
            await axios.post(`${API_ENDPOINT}/Invitations`, { candidateId: userId, campagneId: campId });
        }
        showPulseToast("DÉPLOIEMENT RÉUSSI SUR LE TERMINAL.", "success", "fa-solid fa-server");
        localStorage.removeItem('enigma_studio_draft');
        setTimeout(() => window.location.href = "/campaigns", 3000);
    } catch (err) {
        showPulseToast("ERREUR TRANSACTIONNELLE SQL.", "error", "fa-solid fa-database-slash");
    } finally {
        isPublishing.value = false;
    }
};

const showPulseToast = (msg, type, icon) => {
    globalToast.message = msg; globalToast.type = `t-${type}`; globalToast.icon = icon; globalToast.active = true;
    setTimeout(() => globalToast.active = false, 5500);
};

const generateSystemBackupMocks = () => {
    bankGlobalReference.value = Array.from({ length: 20 }, (v, k) => ({
        id: `mock-${k}`,
        texte: `Modélisation structurelle n°${k+1} : Diagnostique contextuel sur les variables runtime du cycle Docker context triggers ?`,
        categorie: categories[Math.floor(Math.random() * categories.length)],
        poids: 10 + k,
        difficulty: k > 14 ? 'EXPERT' : k > 8 ? 'INTERMÉDIAIRE' : 'JUNIOR',
        correctIndex: 1,
        options: ["Option Alpha", "Patron Certifié UML", "Middleware SQL Pattern", "Alternative Asynchrone v4"],
        explication: "Explication technique détaillée sur le flux messaging system cloud handler properties patterns contextuels v4."
    }));
};
</script>

<style scoped>
/* ============================================================
   DESIGN SYSTEM : ENIGMA OBSIDIAN — ÉDITION AMÉLIORÉE
   ============================================================ */

@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700;800;900&display=swap');

:root {
    --amber-core: #f59e0b;
    --amber-light: #fbbf24;
    --amber-glow: rgba(245, 158, 11, 0.35);
    --amber-soft: rgba(245, 158, 11, 0.08);
    --navy-core: #0f172a;
    --navy-mid: #1e293b;
    --slate-meta: #94a3b8;
    --slate-light: #cbd5e1;
    --border-soft: #e2e8f0;
    --border-xsoft: #f1f5f9;
    --enigma-glass: rgba(255, 255, 255, 0.96);
    --enigma-shadow: 0 24px 64px -16px rgba(0, 0, 0, 0.10);
    --enigma-shadow-lg: 0 40px 100px -20px rgba(0, 0, 0, 0.14);
    --radius-card: 40px;
    --radius-btn: 18px;
    --radius-field: 20px;
}

.enigma-studio-root {
    min-height: 100vh;
    background-color: #f8fafc;
    font-family: 'Plus Jakarta Sans', sans-serif;
    color: var(--navy-core);
}

/* ── BACKGROUND ENGINE ─────────────────────────────────── */
.cyber-engine-bg {
    position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden;
}
.bg-vignette {
    position: absolute; inset: 0;
    background: radial-gradient(ellipse at 50% 0%, rgba(245,158,11,0.04) 0%, transparent 70%);
}
.glow-orb {
    position: absolute; border-radius: 50%;
    filter: blur(130px); opacity: 0.10; transition: transform 0.12s linear;
}
.orb-amber { width: 700px; height: 700px; background: var(--amber-core); top: -20%; left: -8%; }
.orb-blue  { width: 600px; height: 600px; background: #3b82f6; bottom: -12%; right: -6%; }
.quantum-grid {
    position: absolute; inset: 0;
    background-image:
        linear-gradient(rgba(148, 163, 184, 0.06) 1px, transparent 1px),
        linear-gradient(90deg, rgba(148, 163, 184, 0.06) 1px, transparent 1px);
    background-size: 56px 56px;
}

/* ── HEADER ────────────────────────────────────────────── */
.studio-header-v2 {
    transition: background 0.4s ease, border-color 0.4s ease, box-shadow 0.4s ease;
    position: sticky; top: 0; z-index: 1000;
}
.is-docked {
    background: rgba(255, 255, 255, 0.88);
    backdrop-filter: blur(28px) saturate(1.6);
    -webkit-backdrop-filter: blur(28px) saturate(1.6);
    border-bottom: 1px solid var(--border-xsoft);
    box-shadow: 0 4px 24px rgba(0,0,0,0.04);
}

/* ── ROBOT TERMINAL ICON ───────────────────────────────── */
.ai-robot-terminal {
    width: 58px; height: 58px;
    background: var(--navy-core);
    border-radius: 20px;
    display: flex; align-items: center; justify-content: center;
    flex-shrink: 0;
    position: relative; overflow: hidden;
    box-shadow: 0 8px 24px rgba(15,23,42,0.25);
}
.ai-robot-terminal::after {
    content: '';
    position: absolute; inset: 0;
    background: linear-gradient(135deg, rgba(245,158,11,0.2) 0%, transparent 55%);
    pointer-events: none;
}

/* ── BRAND TEXT ────────────────────────────────────────── */
.main-title-v2 {
    font-weight: 900;
    font-size: 2.1rem;
    letter-spacing: -1.5px;
    margin: 0; line-height: 1;
}
.main-title-v2 > span:first-child { color: var(--amber-core); }

.v-badge {
    font-size: 0.58rem;
    font-weight: 800;
    letter-spacing: 1.5px;
    border: 1.5px solid var(--amber-core);
    color: var(--amber-core);
    padding: 3px 9px;
    border-radius: 8px;
    vertical-align: middle;
    margin-left: 10px;
}
.brand-subtitle-v2 {
    font-size: 0.6rem; font-weight: 800;
    color: var(--slate-meta); letter-spacing: 2.5px; margin: 5px 0 0;
}

/* ── HEALTH WIDGET ─────────────────────────────────────── */
.health-core-widget {
    display: flex; align-items: center; gap: 12px;
    background: white;
    padding: 6px 16px 6px 8px;
    border-radius: 22px;
    border: 1px solid var(--border-soft);
    box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    cursor: default;
}
.health-ring-box {
    position: relative; width: 48px; height: 48px; flex-shrink: 0;
}
.health-ring-box svg { transform: rotate(-90deg); }
.ring-track { fill: none; stroke: var(--border-xsoft); stroke-width: 4; }
.ring-fill {
    fill: none; stroke-width: 4; stroke-linecap: round;
    transition: stroke-dashoffset 1s cubic-bezier(0.4, 0, 0.2, 1), stroke 0.5s ease;
}
.health-percent {
    position: absolute; inset: 0;
    display: flex; align-items: center; justify-content: center;
    font-size: 0.72rem; font-weight: 900; color: var(--navy-core);
}
.health-label-box { display: flex; flex-direction: column; line-height: 1.3; }
.h-main { font-size: 0.52rem; font-weight: 900; letter-spacing: 1.5px; color: var(--slate-meta); }
.h-sub  { font-size: 0.82rem; font-weight: 900; transition: color 0.4s; }

.action-divider { width: 1px; height: 34px; background: var(--border-soft); margin: 0 2px; }

/* ── SYNC INDICATOR ────────────────────────────────────── */
.sync-indicator-v2 {
    display: flex; align-items: center; gap: 8px;
    font-size: 0.62rem; font-weight: 800; letter-spacing: 1.5px; color: var(--slate-meta);
}
.is-stable .status-icon {
    width: 8px; height: 8px; border-radius: 50%; background: #10b981; display: inline-block;
    box-shadow: 0 0 0 3px rgba(16,185,129,0.18);
    animation: pulse-stable 2.5s ease-in-out infinite;
}
.is-syncing .status-icon {
    width: 8px; height: 8px; border-radius: 50%; background: var(--amber-core); display: inline-block;
    animation: pulse-syncing 0.8s ease-in-out infinite;
}
@keyframes pulse-stable {
    0%, 100% { box-shadow: 0 0 0 3px rgba(16,185,129,0.18); }
    50%       { box-shadow: 0 0 0 6px rgba(16,185,129,0.06); }
}
@keyframes pulse-syncing {
    0%, 100% { opacity: 1; } 50% { opacity: 0.35; }
}

/* ── BUTTONS ───────────────────────────────────────────── */
.btn-enigma-primary {
    background: var(--navy-core);
    color: white;
    border: none;
    padding: 13px 30px;
    border-radius: var(--radius-btn);
    font-weight: 800;
    font-size: 0.82rem;
    position: relative;
    overflow: hidden;
    transition: transform 0.3s cubic-bezier(0.34,1.56,0.64,1), box-shadow 0.3s ease;
    cursor: pointer;
    font-family: inherit;
}
.btn-enigma-primary .btn-content { position: relative; z-index: 2; display: flex; align-items: center; }
.btn-enigma-primary .btn-glow {
    position: absolute; inset: 0;
    background: linear-gradient(135deg, var(--amber-core), var(--amber-light));
    opacity: 0;
    transition: opacity 0.35s ease;
}
.btn-enigma-primary:hover { transform: translateY(-2px); box-shadow: 0 16px 36px var(--amber-glow); }
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary:hover .btn-content { color: var(--navy-core); }
.btn-enigma-primary:disabled { opacity: 0.45; cursor: not-allowed; transform: none; box-shadow: none; }

.btn-enigma-outline {
    background: white;
    border: 1.5px solid var(--border-soft);
    padding: 11px 24px;
    border-radius: var(--radius-btn);
    font-weight: 800;
    font-size: 0.82rem;
    color: var(--navy-core);
    transition: border-color 0.25s, color 0.25s, box-shadow 0.25s;
    cursor: pointer;
    font-family: inherit;
}
.btn-enigma-outline:hover {
    border-color: var(--amber-core);
    color: var(--amber-core);
    box-shadow: 0 4px 16px var(--amber-soft);
}

.btn-enigma-outline-amber {
    background: var(--amber-soft);
    border: 1.5px solid rgba(245,158,11,0.3);
    padding: 11px 24px;
    border-radius: var(--radius-btn);
    font-weight: 800;
    font-size: 0.82rem;
    color: var(--amber-core);
    transition: background 0.25s, border-color 0.25s;
    cursor: pointer;
    font-family: inherit;
}
.btn-enigma-outline-amber:hover { background: rgba(245,158,11,0.14); border-color: var(--amber-core); }

/* ── IA / BANK BUTTONS ─────────────────────────────────── */
.btn-ia-action-v2 {
    background: linear-gradient(135deg, #7c3aed, #a855f7);
    color: white; border: none;
    padding: 10px 20px; border-radius: 14px;
    font-weight: 800; font-size: 0.75rem;
    cursor: pointer; font-family: inherit;
    display: flex; align-items: center; gap: 8px;
    transition: transform 0.2s, box-shadow 0.2s;
}
.btn-ia-action-v2:hover { transform: translateY(-2px); box-shadow: 0 10px 24px rgba(124,58,237,0.3); }

.btn-bank-action-v2 {
    background: white; border: 1.5px solid var(--border-soft);
    padding: 10px 20px; border-radius: 14px;
    font-weight: 800; font-size: 0.75rem;
    cursor: pointer; font-family: inherit;
    display: flex; align-items: center; gap: 8px;
    color: var(--navy-core); transition: border-color 0.2s, color 0.2s;
}
.btn-bank-action-v2:hover { border-color: var(--amber-core); color: var(--amber-core); }

/* ── STEPPER ───────────────────────────────────────────── */
.stepper-precision-v8 {
    position: relative;
    max-width: 960px;
    margin-top: 2.5rem;
    padding-bottom: 2.5rem;
}
.stepper-line-bg {
    position: absolute; top: 11px; left: 0; right: 0;
    height: 2px; background: var(--border-soft); border-radius: 4px;
}
.stepper-line-fill {
    height: 100%;
    background: linear-gradient(90deg, var(--amber-core), var(--amber-light));
    border-radius: 4px;
    transition: width 0.7s cubic-bezier(0.4, 0, 0.2, 1);
}
.stepper-nodes-row {
    position: relative; display: flex; justify-content: space-between;
}
.step-node-v8 { cursor: pointer; }
.node-point {
    width: 22px; height: 22px;
    background: white; border: 2.5px solid var(--border-soft);
    border-radius: 50%;
    display: flex; align-items: center; justify-content: center;
    transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
    position: relative; z-index: 1;
}
.step-node-v8.active .node-point {
    border-color: var(--amber-core);
    background: var(--amber-core);
    transform: scale(1.3);
    box-shadow: 0 0 0 6px rgba(245,158,11,0.16), 0 4px 12px rgba(245,158,11,0.3);
}
.step-node-v8.completed .node-point {
    background: var(--amber-core);
    border-color: var(--amber-core);
}
.point-inner { font-size: 0; }
.step-node-v8.active .point-inner,
.step-node-v8.completed .point-inner {
    font-size: 9px; color: white; font-weight: 900;
    text-align: center; line-height: 17px;
}
.node-label-floating {
    position: absolute; top: 32px; left: 50%;
    transform: translateX(-50%);
    white-space: nowrap;
    font-size: 0.7rem; font-weight: 800;
    color: var(--slate-meta);
    transition: color 0.25s;
}
.step-node-v8.active .node-label-floating { color: var(--navy-core); }
.step-node-v8.completed .node-label-floating { color: #64748b; }

/* ── CARDS ─────────────────────────────────────────────── */
.enigma-card {
    background: var(--enigma-glass);
    backdrop-filter: blur(20px);
    -webkit-backdrop-filter: blur(20px);
    border-radius: var(--radius-card);
    border: 1px solid rgba(255,255,255,0.9);
    box-shadow: var(--enigma-shadow);
}
.pane-header-v2 { display: flex; align-items: center; gap: 20px; }
.icon-box-v2 {
    width: 56px; height: 56px; border-radius: 18px;
    display: flex; align-items: center; justify-content: center;
    font-size: 1.5rem; color: white;
    transform: rotate(-4deg);
    flex-shrink: 0;
}
.icon-box-v2.amber { background: linear-gradient(135deg, var(--amber-core), var(--amber-light)); }

/* ── INPUTS ────────────────────────────────────────────── */
.enigma-input-wrap label {
    font-size: 0.62rem; font-weight: 900; letter-spacing: 1.8px;
    color: var(--slate-meta); text-transform: uppercase;
    margin-bottom: 11px; display: block;
}
.enigma-field {
    width: 100%;
    background: #f8fafc;
    border: 2px solid var(--border-soft);
    padding: 16px 22px;
    border-radius: var(--radius-field);
    font-weight: 700;
    font-size: 0.9rem;
    color: var(--navy-core);
    outline: none;
    transition: border-color 0.25s, background 0.25s, box-shadow 0.25s;
    font-family: inherit;
}
.enigma-field:focus {
    border-color: var(--amber-core);
    background: white;
    box-shadow: 0 0 0 4px rgba(245,158,11,0.08), 0 8px 24px rgba(245,158,11,0.06);
}
.field-hint { font-size: 0.72rem; color: var(--slate-meta); margin-top: 7px; display: block; }

.select-enigma-custom { position: relative; }
.select-enigma-custom i {
    position: absolute; right: 18px; top: 50%; transform: translateY(-50%);
    font-size: 0.75rem; color: var(--slate-meta); pointer-events: none;
}
.input-with-ico { position: relative; }
.input-with-ico > i {
    position: absolute; left: 20px; top: 50%; transform: translateY(-50%);
    color: var(--slate-meta); font-size: 0.9rem;
}
.enigma-field.has-ico { padding-left: 48px; }

/* ── ADMISSIBILITY SLIDER ──────────────────────────────── */
.admissibility-dashboard {
    background: #f8fafc;
    border-radius: 28px;
    padding: 28px 32px;
    border: 1.5px solid var(--border-xsoft);
}
.score-display-v2 {
    font-size: 2rem; font-weight: 900; color: var(--amber-core);
    background: var(--amber-soft);
    padding: 6px 20px; border-radius: 16px;
    line-height: 1;
}
.range-enigma-wrap {
    position: relative; height: 8px;
    background: var(--border-soft); border-radius: 10px; overflow: hidden;
    margin-top: 16px;
}
.range-visual-track {
    height: 100%;
    background: linear-gradient(90deg, var(--amber-core), var(--amber-light));
    border-radius: 10px;
    transition: width 0.15s ease;
    pointer-events: none;
}
.enigma-range {
    width: 100%; opacity: 0;
    position: relative; z-index: 2;
    margin: -8px 0 0; cursor: pointer;
}

/* ── ASSET CARDS ───────────────────────────────────────── */
.asset-card-v8 {
    display: flex; align-items: center; gap: 16px;
    background: white;
    border: 1.5px solid var(--border-xsoft);
    border-radius: 22px; padding: 18px 22px;
    margin-bottom: 12px;
    transition: border-color 0.2s, transform 0.2s, box-shadow 0.2s;
}
.asset-card-v8:hover {
    border-color: rgba(245,158,11,0.4);
    transform: translateX(5px);
    box-shadow: 0 4px 20px rgba(245,158,11,0.08);
}
.drag-node-handle { color: var(--slate-light); cursor: grab; font-size: 0.9rem; flex-shrink: 0; }
.drag-node-handle:active { cursor: grabbing; }
.asset-index-v8 {
    font-size: 0.62rem; font-weight: 900; letter-spacing: 0.5px;
    color: var(--amber-core);
    background: var(--amber-soft);
    padding: 4px 10px; border-radius: 10px;
    white-space: nowrap; flex-shrink: 0;
}
.asset-core-v8 { flex: 1; min-width: 0; }
.asset-title-v8 { font-size: 0.88rem; font-weight: 700; margin: 0 0 8px; }
.asset-tags-v8 { display: flex; gap: 8px; flex-wrap: wrap; }
.t-pill {
    font-size: 0.6rem; font-weight: 800; letter-spacing: 0.5px;
    padding: 3px 10px; border-radius: 10px;
}
.t-pill.weight { background: #ecfdf5; color: #059669; }
.t-pill.tech    { background: #eff6ff; color: #1d4ed8; }
.t-pill.status  { background: #fef3c7; color: #92400e; }
.t-pill.expert  { background: #fce7f3; color: #9d174d; }
.btn-remove-v8 {
    width: 32px; height: 32px; border-radius: 10px;
    background: #fef2f2; border: none;
    color: #ef4444; cursor: pointer;
    display: flex; align-items: center; justify-content: center;
    font-size: 0.75rem; transition: background 0.2s;
    flex-shrink: 0;
}
.btn-remove-v8:hover { background: #fecaca; }
.asset-ghost-v8 { opacity: 0.35; background: #fffdf5; border-style: dashed; }

/* ── EMPTY STATE ───────────────────────────────────────── */
.empty-assets-v2 {
    text-align: center; border-radius: 36px;
    background: white; border: 2px dashed var(--border-soft);
}
.empty-icon-pulse {
    position: relative; display: inline-flex;
    align-items: center; justify-content: center;
    width: 80px; height: 80px;
    background: var(--amber-soft); border-radius: 28px;
    font-size: 2rem; color: var(--amber-core);
}
.pulse-ring {
    position: absolute; inset: -8px; border-radius: 36px;
    border: 2px solid rgba(245,158,11,0.25);
    animation: pulse-ring 2s ease-in-out infinite;
}
@keyframes pulse-ring {
    0%, 100% { transform: scale(1); opacity: 1; }
    50%       { transform: scale(1.08); opacity: 0.5; }
}

/* ── TALENT CARDS ──────────────────────────────────────── */
.talent-hub-search-v2 { display: flex; gap: 20px; align-items: center; }
.search-enigma-box {
    flex: 1; position: relative;
    background: white; border-radius: 20px; border: 1.5px solid var(--border-soft);
    overflow: hidden;
}
.search-enigma-box i {
    position: absolute; left: 18px; top: 50%;
    transform: translateY(-50%); color: var(--slate-meta); font-size: 0.9rem;
}
.enigma-search-field {
    width: 100%; background: transparent; border: none;
    padding: 14px 18px 14px 46px;
    font-weight: 600; font-size: 0.88rem;
    color: var(--navy-core); outline: none; font-family: inherit;
}
.batch-counter-v2 {
    display: flex; flex-direction: column; align-items: center;
    background: var(--navy-core); color: white;
    padding: 12px 22px; border-radius: 18px; line-height: 1.1;
}
.batch-counter-v2 .val { font-size: 1.6rem; font-weight: 900; }
.batch-counter-v2 .lab { font-size: 0.55rem; font-weight: 900; letter-spacing: 1.5px; opacity: 0.5; }

.talent-card-v8 {
    background: white; border-radius: 22px;
    border: 2px solid var(--border-soft);
    padding: 20px; cursor: pointer;
    transition: all 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);
    display: flex; flex-direction: column; align-items: center; gap: 10px;
    position: relative;
}
.talent-card-v8:hover { border-color: rgba(245,158,11,0.4); transform: translateY(-3px); }
.talent-card-v8.active { border-color: var(--amber-core); background: #fffdf5; transform: translateY(-3px); box-shadow: 0 12px 28px var(--amber-glow); }
.card-check-v8 {
    position: absolute; top: 14px; right: 14px;
    width: 22px; height: 22px; border-radius: 50%;
    background: var(--border-soft); color: transparent;
    display: flex; align-items: center; justify-content: center;
    font-size: 0.65rem; transition: all 0.2s;
}
.talent-card-v8.active .card-check-v8 { background: var(--amber-core); color: white; }
.avatar-v8 {
    width: 48px; height: 48px; border-radius: 18px;
    background: var(--navy-core); color: white;
    display: flex; align-items: center; justify-content: center;
    font-size: 1.2rem; font-weight: 900;
}
.data-v8 { text-align: center; }
.data-v8 .n { font-size: 0.88rem; font-weight: 800; }
.data-v8 .e { font-size: 0.72rem; color: var(--slate-meta); margin-top: 2px; }

/* ── PROTOCOL ROWS ─────────────────────────────────────── */
.protocol-security-matrix { display: flex; flex-direction: column; gap: 0; }
.protocol-row {
    display: flex; align-items: center; gap: 20px;
    padding: 24px 0; border-bottom: 1px solid var(--border-xsoft);
}
.protocol-row:last-child { border-bottom: none; }
.p-icon {
    width: 46px; height: 46px; border-radius: 16px;
    background: var(--amber-soft);
    display: flex; align-items: center; justify-content: center;
    font-size: 1.1rem; color: var(--amber-core); flex-shrink: 0;
}
.p-data { flex: 1; }
.p-data h6 { font-weight: 800; font-size: 0.9rem; margin: 0 0 4px; }
.p-data p  { font-size: 0.78rem; color: var(--slate-meta); margin: 0; }

/* ── ANALYTICS SIDEBAR ─────────────────────────────────── */
.analytics-hub-glass {
    background: var(--navy-core);
    color: white; border-radius: 44px;
    padding: 42px;
    position: relative; overflow: hidden;
    box-shadow: 0 24px 60px rgba(15,23,42,0.2);
}
.analytics-hub-glass::before {
    content: '';
    position: absolute; top: -90px; right: -90px;
    width: 240px; height: 240px; border-radius: 50%;
    background: rgba(245,158,11,0.07);
    pointer-events: none;
}
.analytics-hub-glass::after {
    content: '';
    position: absolute; bottom: -60px; left: -60px;
    width: 180px; height: 180px; border-radius: 50%;
    background: rgba(59,130,246,0.05);
    pointer-events: none;
}
.hub-header-v2 { position: relative; z-index: 1; }
.hub-label { font-size: 0.58rem; font-weight: 900; letter-spacing: 2.5px; color: rgba(255,255,255,0.3); }
.hub-title-v2 { font-size: 1.05rem; font-weight: 800; margin: 6px 0 16px; line-height: 1.25; }
.hub-status-box {
    display: inline-flex; align-items: center; gap: 8px;
    font-size: 0.62rem; font-weight: 800; letter-spacing: 1px;
    padding: 6px 14px; border-radius: 12px;
}
.hub-status-box.cl-success { background: rgba(16,185,129,0.14); color: #34d399; }
.hub-status-box.cl-warn    { background: rgba(245,158,11,0.14); color: #fbbf24; }
.hub-status-box.cl-err     { background: rgba(239,68,68,0.14); color: #f87171; }
.pulse-dot {
    width: 7px; height: 7px; border-radius: 50%;
    background: currentColor;
    animation: pulse-stable 2.5s ease-in-out infinite;
    display: inline-block;
}

/* ── KPI BENTO ─────────────────────────────────────────── */
.kpi-bento-grid { display: grid; grid-template-columns: repeat(3,1fr); gap: 12px; }
.bento-item {
    background: rgba(255,255,255,0.05);
    border-radius: 22px; padding: 22px 10px;
    text-align: center; border: 1px solid rgba(255,255,255,0.04);
    transition: background 0.2s;
}
.bento-item.highlight {
    background: rgba(245,158,11,0.12);
    border-color: rgba(245,158,11,0.2);
}
.bento-item .v { display: block; font-size: 2.1rem; font-weight: 900; line-height: 1; margin-bottom: 5px; }
.bento-item .l { font-size: 0.57rem; font-weight: 900; opacity: 0.38; text-transform: uppercase; letter-spacing: 1px; }

/* ── DEEP ANALYTICS ────────────────────────────────────── */
.deep-analytics-list .d-row {
    display: flex; justify-content: space-between; align-items: center;
    padding: 13px 0; border-bottom: 1px solid rgba(255,255,255,0.06);
    font-size: 0.82rem;
}
.deep-analytics-list .d-row:last-child { border-bottom: none; }
.deep-analytics-list .k { color: rgba(255,255,255,0.45); }
.deep-analytics-list .v { font-weight: 800; }
.bolts-v8 { display: flex; gap: 4px; }
.bolts-v8 .fa-bolt { font-size: 0.78rem; color: rgba(255,255,255,0.15); transition: color 0.2s; }
.bolts-v8 .fa-bolt.active { color: var(--amber-core); }
.text-amber { color: var(--amber-core) !important; }

/* ── WIZARD CONTROLS ───────────────────────────────────── */
.wizard-controls-v8 { border-top: 1px solid rgba(255,255,255,0.08); }
.btn-step-nav {
    height: 46px; border-radius: 15px; font-weight: 800; font-size: 0.8rem;
    cursor: pointer; font-family: inherit; transition: all 0.25s;
    display: flex; align-items: center; justify-content: center; gap: 8px;
}
.btn-step-nav.back {
    width: 46px; flex-shrink: 0;
    background: rgba(255,255,255,0.07);
    border: 1px solid rgba(255,255,255,0.1);
    color: white;
}
.btn-step-nav.back:hover { background: rgba(255,255,255,0.13); }
.btn-step-nav.back:disabled { opacity: 0.3; cursor: not-allowed; }
.btn-step-nav.next {
    background: var(--amber-core);
    border: none; color: var(--navy-core); letter-spacing: 0.5px;
}
.btn-step-nav.next:hover { background: var(--amber-light); transform: translateY(-1px); }
.btn-step-deploy {
    height: 46px; flex: 1; border-radius: 15px;
    background: linear-gradient(135deg, #10b981, #059669);
    border: none; color: white;
    font-weight: 800; font-size: 0.8rem;
    cursor: pointer; font-family: inherit;
    transition: opacity 0.2s, transform 0.2s;
}
.btn-step-deploy:hover { opacity: 0.88; transform: translateY(-1px); }
.btn-step-deploy:disabled { opacity: 0.4; cursor: not-allowed; transform: none; }
.reset-link {
    text-align: center; margin-top: 14px;
    font-size: 0.7rem; color: rgba(255,255,255,0.25);
    cursor: pointer; transition: color 0.2s;
    user-select: none;
}
.reset-link:hover { color: rgba(255,255,255,0.5); }

/* ── COACH IA ──────────────────────────────────────────── */
.ia-coach-terminal {
    background: white; border-radius: 30px; padding: 26px 30px;
    display: flex; align-items: center; gap: 20px;
    border: 1px solid var(--border-xsoft);
    box-shadow: 0 8px 28px rgba(0,0,0,0.04);
}
.robot-glow-container {
    flex-shrink: 0;
    background: var(--navy-core);
    border-radius: 18px; padding: 8px;
    animation: robotFloat 4s ease-in-out infinite;
    box-shadow: 0 8px 20px rgba(15,23,42,0.2);
}
@keyframes robotFloat {
    0%, 100% { transform: translateY(0); }
    50%       { transform: translateY(-9px); }
}
.coach-text-v8 h6 { font-size: 0.82rem; font-weight: 900; margin: 0 0 6px; color: var(--navy-core); }
.coach-text-v8 p  { font-size: 0.76rem; color: #64748b; line-height: 1.55; }

/* ── QUANTUM VAULT MODAL ───────────────────────────────── */
.quantum-vault-overlay {
    position: fixed; inset: 0;
    background: rgba(15, 23, 42, 0.45);
    backdrop-filter: blur(16px);
    -webkit-backdrop-filter: blur(16px);
    z-index: 5000; display: flex;
    align-items: center; justify-content: center; padding: 32px;
}
.quantum-vault-window {
    width: 95vw; max-width: 1400px; height: 90vh;
    background: white; border-radius: 48px;
    overflow: hidden; display: flex; flex-direction: column;
    box-shadow: 0 60px 120px rgba(0,0,0,0.22);
}
.qv-header {
    padding: 32px 48px; display: flex;
    justify-content: space-between; align-items: center;
    border-bottom: 1px solid var(--border-xsoft);
    flex-shrink: 0;
}
.qv-title h2 { font-size: 1.5rem; }
.qv-layout { flex-grow: 1; display: flex; overflow: hidden; }

.qv-sidebar {
    width: 320px; flex-shrink: 0;
    border-right: 1px solid var(--border-xsoft);
    padding: 36px; display: flex; flex-direction: column;
    overflow-y: auto;
}
.qv-search-area label { font-size: 0.6rem; font-weight: 900; letter-spacing: 1.5px; color: var(--slate-meta); display: block; margin-bottom: 10px; }
.qv-search-wrap {
    position: relative; background: #f8fafc;
    border-radius: 16px; border: 1.5px solid var(--border-soft);
}
.qv-search-wrap i { position: absolute; left: 14px; top: 50%; transform: translateY(-50%); color: var(--slate-meta); font-size: 0.85rem; }
.qv-search-wrap input {
    width: 100%; background: transparent; border: none;
    padding: 12px 16px 12px 40px;
    font-weight: 600; font-size: 0.85rem; outline: none; font-family: inherit;
}
.qv-filter-group label { font-size: 0.6rem; font-weight: 900; letter-spacing: 1.5px; color: var(--slate-meta); display: block; margin-bottom: 12px; }
.qv-pill-stack { display: flex; gap: 8px; flex-wrap: wrap; }
.qv-pill-stack button {
    background: #f1f5f9; border: none; padding: 7px 16px;
    border-radius: 12px; font-size: 0.7rem; font-weight: 800;
    color: var(--slate-meta); cursor: pointer; font-family: inherit; transition: all 0.2s;
}
.qv-pill-stack button.active { background: var(--navy-core); color: white; }
.qv-bank-stats { font-size: 0.82rem; }

.qv-list {
    flex-grow: 1; padding: 32px 28px;
    background: #fafbfc; overflow-y: auto;
    display: grid; grid-template-columns: repeat(auto-fill, minmax(260px, 1fr));
    gap: 16px; align-content: start;
}
.qv-item-card {
    background: white; border-radius: 26px;
    padding: 26px; cursor: pointer;
    border: 2px solid transparent;
    transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
    box-shadow: 0 2px 8px rgba(0,0,0,0.04);
    height: fit-content;
}
.qv-item-card:hover { border-color: rgba(245,158,11,0.35); transform: translateY(-2px); }
.qv-item-card.active { border-color: var(--amber-core); background: #fffdf5; transform: translateY(-2px) scale(1.01); }
.qv-item-card.checked { border-color: rgba(16,185,129,0.5); }
.qv-item-top { display: flex; justify-content: space-between; align-items: center; }
.qv-badge {
    font-size: 0.6rem; font-weight: 800; letter-spacing: 0.5px;
    padding: 4px 10px; border-radius: 10px;
}
.qv-badge.junior        { background: #f0fdf4; color: #166534; }
.qv-badge.intermédiaire { background: #fef3c7; color: #92400e; }
.qv-badge.expert        { background: #fce7f3; color: #9d174d; }
.qv-checkbox { cursor: pointer; font-size: 1.1rem; color: var(--slate-light); transition: color 0.2s; }
.qv-checkbox:hover { color: var(--amber-core); }
.fa-check-circle { color: #10b981 !important; }
.qv-item-text { font-size: 0.85rem; font-weight: 700; line-height: 1.45; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; }
.qv-item-footer { display: flex; justify-content: space-between; font-size: 0.75rem; }
.qv-item-footer .pts { font-weight: 700; color: var(--navy-core); }
.qv-item-footer .time { color: var(--slate-meta); }

.qv-inspector {
    width: 520px; flex-shrink: 0;
    border-left: 1px solid var(--border-xsoft);
    background: white; overflow-y: auto;
}
.qv-tabs {
    display: flex; gap: 4px;
    background: #f8fafc; border-radius: 16px; padding: 4px;
}
.qv-tabs button {
    flex: 1; background: transparent; border: none;
    padding: 10px 16px; border-radius: 12px;
    font-size: 0.7rem; font-weight: 800; letter-spacing: 0.5px;
    cursor: pointer; font-family: inherit; color: var(--slate-meta);
    transition: all 0.2s;
}
.qv-tabs button.active { background: white; color: var(--navy-core); box-shadow: 0 2px 8px rgba(0,0,0,0.06); }
.qv-field-group label { font-size: 0.6rem; font-weight: 900; letter-spacing: 1.5px; color: var(--slate-meta); display: block; margin-bottom: 10px; }
.qv-choice-row {
    display: flex; align-items: center; gap: 12px;
    padding: 12px; border-radius: 16px;
    border: 1.5px solid var(--border-soft);
    margin-bottom: 8px; transition: border-color 0.2s, background 0.2s;
    cursor: pointer;
}
.qv-choice-row.is-correct { border-color: #10b981; background: #f0fdf4; }
.qv-alpha {
    width: 30px; height: 30px; border-radius: 10px;
    background: var(--border-soft); flex-shrink: 0;
    display: flex; align-items: center; justify-content: center;
    font-weight: 900; font-size: 0.8rem; color: var(--navy-core);
    cursor: pointer;
}
.qv-choice-row.is-correct .qv-alpha { background: #10b981; color: white; }
.qv-choice-input {
    flex: 1; background: transparent; border: none;
    font-weight: 600; font-size: 0.85rem; outline: none; font-family: inherit;
}
.qv-check-action { color: var(--slate-light); cursor: pointer; font-size: 1rem; transition: color 0.2s; }
.qv-choice-row.is-correct .qv-check-action { color: #10b981; }
.ia-justification-v8 { border-left: 4px solid var(--amber-core) !important; border-radius: 0 16px 16px 0 !important; }
.ia-justification-v8 textarea { resize: none; line-height: 1.5; font-family: inherit; }
.border-amber { border-color: var(--amber-core) !important; }

.qv-inspector-empty {
    display: flex; flex-direction: column;
    align-items: center; justify-content: center;
    height: 100%; color: var(--slate-light); text-align: center; padding: 40px;
}

.device-mockup-v8 {
    background: #f8fafc; border-radius: 24px; overflow: hidden;
    border: 1.5px solid var(--border-soft);
}
.mockup-top {
    background: var(--navy-core); color: rgba(255,255,255,0.5);
    padding: 12px 22px; font-size: 0.62rem; font-weight: 900; letter-spacing: 2px;
}
.mockup-q-txt { font-weight: 700; font-size: 0.9rem; line-height: 1.55; }
.mockup-opt-item {
    display: flex; align-items: center; gap: 12px;
    padding: 12px 16px; background: white; border-radius: 14px;
    margin-bottom: 8px; font-size: 0.85rem; font-weight: 600;
    border: 1.5px solid var(--border-soft);
}
.m-radio {
    width: 16px; height: 16px; border-radius: 50%;
    border: 2px solid var(--border-soft); flex-shrink: 0;
}

.btn-qv-cancel {
    background: #f1f5f9; border: none; padding: 12px 24px;
    border-radius: 16px; font-weight: 800; font-size: 0.8rem;
    cursor: pointer; font-family: inherit; color: var(--navy-core);
    transition: background 0.2s;
}
.btn-qv-cancel:hover { background: #e2e8f0; }
.btn-qv-confirm {
    background: var(--navy-core); color: white; border: none;
    padding: 12px 28px; border-radius: 16px;
    font-weight: 800; font-size: 0.8rem;
    cursor: pointer; font-family: inherit;
    transition: background 0.2s, transform 0.2s;
}
.btn-qv-confirm:hover { background: #1e293b; transform: translateY(-1px); }
.btn-qv-confirm:disabled { opacity: 0.4; cursor: not-allowed; transform: none; }

/* ── TOAST ─────────────────────────────────────────────── */
.enigma-toast {
    position: fixed; bottom: 36px; right: 36px; z-index: 10000;
    background: var(--navy-core); color: white;
    padding: 22px 34px; border-radius: 28px;
    display: flex; align-items: center; gap: 20px;
    border-left: 5px solid var(--amber-core);
    box-shadow: 0 28px 60px rgba(0,0,0,0.22);
    min-width: 300px;
}
.t-ico { font-size: 1.2rem; color: var(--amber-core); flex-shrink: 0; }
.t-body strong { font-size: 0.6rem; font-weight: 900; letter-spacing: 2px; opacity: 0.45; display: block; margin-bottom: 3px; }
.t-body p { font-size: 0.85rem; font-weight: 700; }
.t-success { border-color: #10b981; }
.t-success .t-ico { color: #10b981; }
.t-error { border-color: #ef4444; }
.t-error .t-ico { color: #ef4444; }
.t-warn { border-color: var(--amber-core); }

/* ── SCROLLBAR ─────────────────────────────────────────── */
.custom-scrollbar::-webkit-scrollbar { width: 5px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: var(--border-soft); border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: var(--slate-light); }

/* ── TRANSITIONS ───────────────────────────────────────── */
.modal-quantum-enter-active { transition: opacity 0.3s ease; }
.modal-quantum-leave-active { transition: opacity 0.25s ease; }
.modal-quantum-enter-from,
.modal-quantum-leave-to { opacity: 0; }

.toast-slide-enter-active { animation: toastIn 0.4s cubic-bezier(0.22, 1, 0.36, 1); }
.toast-slide-leave-active { animation: toastOut 0.3s ease forwards; }
@keyframes toastIn { from { opacity: 0; transform: translateY(20px) scale(0.96); } to { opacity: 1; transform: translateY(0) scale(1); } }
@keyframes toastOut { from { opacity: 1; transform: translateY(0); } to { opacity: 0; transform: translateY(12px); } }

/* ── ENIGMA SWITCH ─────────────────────────────────────── */
.enigma-switch .form-check-input { width: 48px; height: 26px; cursor: pointer; }
.enigma-switch .form-check-input:checked { background-color: var(--amber-core); border-color: var(--amber-core); }
.enigma-switch .form-check-input:focus { box-shadow: 0 0 0 3px rgba(245,158,11,0.2); }

/* ── TEXT NAV ──────────────────────────────────────────── */
.text-navy { color: var(--navy-core) !important; }
.text-muted { color: var(--slate-meta) !important; }
.fw-900 { font-weight: 900 !important; }
.fw-800 { font-weight: 800 !important; }
</style>