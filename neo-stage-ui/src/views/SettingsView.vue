<template>
  <div class="settings-master-root" @mousemove="handleParallax">

    <!-- BACKGROUND -->
    <div class="cyber-engine-bg">
      <div class="bg-vignette"></div>
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-indigo" :style="orbStyle(0.015)"></div>
      <div class="glow-orb orb-rose" :style="orbStyle(0.025)"></div>
      <div class="quantum-grid"></div>
      <div class="mesh-overlay"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="content-area p-4 p-lg-5">

          <div v-if="!loading" class="animate__animated animate__fadeIn">

            <!-- HEADER -->
            <header class="page-header mb-5">
              <div class="header-left">
                <div class="breadcrumb-pro mb-2">
                  <span class="root">Administration</span>
                  <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                  <span class="current">{{ t('settings.terminal') }}</span>
                </div>
                <h2 class="premium-title">
                  {{ t('settings.title') }}
                  <span class="gradient-text">{{ t('settings.titleSpan') }}</span>
                </h2>
                <p class="brand-subtitle mt-1">{{ t('settings.subtitle', { role: roleDisplay }) }}</p>
              </div>
              <div class="header-right">
                <div class="header-live-chip">
                  <span class="live-dot"></span>
                  <span>SYSTÈME EN LIGNE</span>
                </div>
              </div>
            </header>

            <div class="row g-4">

              <!-- SIDEBAR NAV -->
              <div class="col-lg-3">
                <div class="nav-panel sticky-top" style="top: 20px; z-index: 10;">

                  <div class="nav-panel-header">
                    <div class="avatar-display">
                      <img :src="profileDisplayUrl" :alt="t('profile.avatarAlt')" class="avatar-img">
                      <div class="avatar-status-ring"></div>
                      <div class="avatar-glow"></div>
                    </div>
                    <div class="avatar-info ms-3">
                      <h6 class="fw-900 m-0">{{ userForm.prenom }} {{ userForm.nom }}</h6>
                      <span class="role-badge-inline">{{ roleDisplay }}</span>
                    </div>
                  </div>

                  <div class="settings-nav-matrix">
                    <button
                      v-for="tab in filteredTabs"
                      :key="tab.id"
                      @click="activeTab = tab.id"
                      :class="['nav-matrix-btn', { active: activeTab === tab.id }]"
                    >
                      <div class="icon-shell">
                        <i :class="tab.icon"></i>
                      </div>
                      <span>{{ t(`settings.tabs.${tab.id}`).toUpperCase() }}</span>
                      <i v-if="activeTab === tab.id" class="fa-solid fa-chevron-right ms-auto nav-arrow"></i>
                    </button>
                  </div>

                  <div class="nav-panel-footer">
                    <div class="join-date-widget">
                      <i class="fa-regular fa-calendar-check text-amber me-2"></i>
                      <span class="small text-muted">{{ t('profile.joinedSince') }} {{ userForm.joinDate }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- CONTENT PANEL -->
              <div class="col-lg-9">
                <div class="content-panel">

                  <!-- SECTION HEADER -->
                  <div class="section-title-bar mb-5">
                    <div class="section-icon-box">
                      <i :class="filteredTabs.find(t => t.id === activeTab)?.icon"></i>
                    </div>
                    <div>
                      <h5 class="fw-900 m-0">{{ t(`settings.tabs.${activeTab}`) }}</h5>
                      <p class="text-muted small m-0">{{ getSectionSubtitle(activeTab) }}</p>
                    </div>
                    <div class="ms-auto">
                      <span class="status-badge status-active">
                        <span class="status-dot"></span> ACTIF
                      </span>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : PROFIL
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'profile'" class="settings-section animate__animated animate__fadeIn">

                    <div class="profile-hero-card mb-5">
                      <div class="profile-hero-bg">
                        <div class="hero-particles">
                          <span v-for="i in 6" :key="i" class="particle" :style="`--d:${i * 60}deg;--r:${60 + i * 15}px`"></span>
                        </div>
                      </div>
                      <div class="profile-hero-content">
                        <div class="avatar-upload-zone" @click="triggerPhotoUpload">
                          <img :src="profileDisplayUrl" :alt="t('profile.avatarAlt')" class="avatar-large">
                          <div class="avatar-upload-overlay">
                            <i class="fa-solid fa-camera"></i>
                            <span>Modifier</span>
                          </div>
                          <input type="file" ref="photoInput" @change="handlePhotoChange" hidden accept="image/*">
                        </div>
                        <div class="profile-hero-data ms-4">
                          <h3 class="fw-900 m-0">{{ userForm.prenom }} {{ userForm.nom }}</h3>
                          <p class="text-muted mb-3">{{ userForm.email }}</p>
                          <div class="d-flex gap-2 flex-wrap">
                            <span class="hero-badge"><i class="fa-solid fa-shield-check me-1 text-amber"></i>{{ roleDisplay }}</span>
                            <span class="hero-badge"><i class="fa-regular fa-calendar me-1 text-indigo"></i>{{ userForm.joinDate }}</span>
                          </div>
                        </div>
                      </div>
                    </div>

                    <div class="row g-4">
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.firstName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-user input-icon"></i>
                            <input type="text" class="enigma-field" v-model="userForm.prenom" :placeholder="t('settings.labels.firstName')">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.lastName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-user input-icon"></i>
                            <input type="text" class="enigma-field" v-model="userForm.nom" :placeholder="t('settings.labels.lastName')">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.email').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-envelope input-icon"></i>
                            <input type="email" class="enigma-field" v-model="userForm.email" :placeholder="t('settings.labels.email')">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.bio').toUpperCase() }}</label>
                          <textarea class="enigma-field no-icon" v-model="userForm.bio" rows="4" :placeholder="t('settings.labels.bioPlaceholder')"></textarea>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : SÉCURITÉ
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'security'" class="settings-section animate__animated animate__fadeIn">

                    <div class="security-status-widget mb-5">
                      <div class="sec-status-left">
                        <div class="sec-icon-ring">
                          <i class="fa-solid fa-shield-halved"></i>
                          <div class="sec-icon-pulse"></div>
                        </div>
                        <div class="ms-4">
                          <h6 class="fw-900 m-0">Niveau de sécurité</h6>
                          <p class="text-muted small m-0">Protégez votre compte avec un mot de passe robuste</p>
                        </div>
                      </div>
                      <div class="sec-health-bar-wrap">
                        <div class="sec-health-label-top">BON</div>
                        <div class="sec-health-bar">
                          <div class="sec-health-fill" style="width: 75%">
                            <div class="sec-health-shimmer"></div>
                          </div>
                        </div>
                        <div class="sec-health-pct">75%</div>
                      </div>
                    </div>

                    <div class="row g-4">
                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.currentPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-lock input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.currentPassword" placeholder="••••••••">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.newPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-key input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.newPassword" placeholder="••••••••">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.confirmPassword').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-key input-icon"></i>
                            <input type="password" class="enigma-field" v-model="securityForm.confirmPassword" placeholder="••••••••">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12" v-if="securityForm.newPassword">
                        <div class="password-strength-analyzer">
                          <div class="psa-header">
                            <div class="psa-label">ROBUSTESSE DU MOT DE PASSE</div>
                            <span class="psa-text" :style="{ color: strengthColor }">{{ strengthLabel }}</span>
                          </div>
                          <div class="psa-bars">
                            <div
                              v-for="(_, i) in 5"
                              :key="i"
                              class="psa-bar"
                              :class="{ filled: i < passwordStrength }"
                              :style="i < passwordStrength ? { background: strengthColor, boxShadow: `0 0 8px ${strengthColor}55` } : {}"
                            ></div>
                          </div>
                          <div class="psa-criteria mt-3">
                            <div class="criterion" :class="{ met: securityForm.newPassword.length >= 8 }">
                              <i class="fa-solid" :class="securityForm.newPassword.length >= 8 ? 'fa-circle-check' : 'fa-circle'"></i>
                              <span>8 caractères minimum</span>
                            </div>
                            <div class="criterion" :class="{ met: /[A-Z]/.test(securityForm.newPassword) }">
                              <i class="fa-solid" :class="/[A-Z]/.test(securityForm.newPassword) ? 'fa-circle-check' : 'fa-circle'"></i>
                              <span>Majuscule</span>
                            </div>
                            <div class="criterion" :class="{ met: /[0-9]/.test(securityForm.newPassword) }">
                              <i class="fa-solid" :class="/[0-9]/.test(securityForm.newPassword) ? 'fa-circle-check' : 'fa-circle'"></i>
                              <span>Chiffre</span>
                            </div>
                            <div class="criterion" :class="{ met: /[^A-Za-z0-9]/.test(securityForm.newPassword) }">
                              <i class="fa-solid" :class="/[^A-Za-z0-9]/.test(securityForm.newPassword) ? 'fa-circle-check' : 'fa-circle'"></i>
                              <span>Caractère spécial</span>
                            </div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12 mt-2" v-if="authStore.role !== 'Candidat'">
                        <div class="protocol-row">
                          <div class="p-icon"><i class="fa-solid fa-mobile-screen-button"></i></div>
                          <div class="p-data">
                            <h6>Authentification 2FA</h6>
                            <p>Renforcez votre sécurité avec un second facteur d'authentification.</p>
                          </div>
                          <span class="badge-coming-soon">BIENTÔT</span>
                        </div>
                      </div>
                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : BRANDING
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'branding'" class="settings-section animate__animated animate__fadeIn">
                    <div class="row g-4">

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.companyName').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-building input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.companyName">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.matriculeFiscale').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-id-card input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.matriculeFiscale">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.domain').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-globe input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.domaine">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.industry').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-briefcase input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.secteur">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.website').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-link input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.siteWeb">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.zipCode').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-map-pin input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.codePostal">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.city').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-city input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.ville">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-md-6">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.country').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-flag input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.pays">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.address').toUpperCase() }}</label>
                          <div class="input-with-icon">
                            <i class="fa-solid fa-location-dot input-icon"></i>
                            <input type="text" class="enigma-field" v-model="brandForm.adresse">
                            <div class="field-shine"></div>
                          </div>
                        </div>
                      </div>

                      <div class="col-12">
                        <div class="enigma-input-wrap">
                          <label>{{ t('settings.labels.description').toUpperCase() }}</label>
                          <textarea class="enigma-field p-3" rows="4" style="height: auto;" v-model="brandForm.description"></textarea>
                        </div>
                      </div>

                    </div>
                  </div>

                  <!-- ══════════════════════════
                       SECTION : INTÉGRATIONS
                  ══════════════════════════ -->
                  <div v-if="activeTab === 'integrations'" class="settings-section animate__animated animate__fadeIn">

                    <!-- Stats Row -->
                    <div class="integrations-stats-row mb-5">
                      <div class="int-stat-card stat-connected">
                        <div class="int-stat-icon-wrap">
                          <div class="int-stat-icon" style="background: linear-gradient(135deg,#ecfdf5,#d1fae5); color: #059669;">
                            <i class="fa-solid fa-link"></i>
                          </div>
                        </div>
                        <div>
                          <div class="int-stat-value">{{ integrationStats.isGoogleConnected ? '1' : '0' }}</div>
                          <div class="int-stat-label">Connecté(s)</div>
                        </div>
                        <div class="stat-card-glow" style="--gc:#10b981;"></div>
                      </div>

                      <div class="int-stat-card stat-available">
                        <div class="int-stat-icon-wrap">
                          <div class="int-stat-icon" style="background: linear-gradient(135deg,#fffbeb,#fef3c7); color: #d97706;">
                            <i class="fa-solid fa-plug-circle-bolt"></i>
                          </div>
                        </div>
                        <div>
                          <div class="int-stat-value">1</div>
                          <div class="int-stat-label">Disponible(s)</div>
                        </div>
                        <div class="stat-card-glow" style="--gc:#f59e0b;"></div>
                      </div>

                      <div class="int-stat-card stat-oauth2">
                        <div class="int-stat-icon-wrap">
                          <div class="int-stat-icon oauth2-icon-shell">
                            <svg width="22" height="22" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                              <circle cx="12" cy="12" r="9.5" stroke="#0ea5e9" stroke-width="1.6"/>
                              <path d="M8.5 12C8.5 9.79 10.07 8 12 8C13.38 8 14.58 8.79 15.2 10H17.36C16.64 7.67 14.51 6 12 6C8.96 6 6.5 8.69 6.5 12C6.5 15.31 8.96 18 12 18C14.51 18 16.64 16.33 17.36 14H15.2C14.58 15.21 13.38 16 12 16C10.07 16 8.5 14.21 8.5 12Z" fill="#0ea5e9"/>
                              <path d="M15 12L18.5 9V11H21.5V13H18.5V15L15 12Z" fill="#0ea5e9"/>
                            </svg>
                          </div>
                        </div>
                        <div>
                          <div class="int-stat-value oauth2-value">OAuth 2.0</div>
                          <div class="int-stat-label">Protocole sécurisé</div>
                        </div>
                        <div class="oauth2-badge-corner">
                          <i class="fa-solid fa-lock"></i>
                        </div>
                        <div class="stat-card-glow" style="--gc:#0ea5e9;"></div>
                      </div>
                    </div>

                    <!-- Google Integration -->
                    <div class="integrations-grid">
                      <div class="integration-card" :class="{ connected: integrationStats.isGoogleConnected }">
                        <div class="integration-card-inner">
                          <div class="int-icon-shell">
                            <svg width="26" height="26" viewBox="0 0 24 24" fill="none">
                              <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                              <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                              <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                              <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
                            </svg>
                          </div>
                          <div class="int-data flex-grow-1">
                            <div class="d-flex align-items-center gap-2 mb-1 flex-wrap">
                              <h6 class="fw-900 m-0">{{ t('settings.labels.googleTitle') }}</h6>
                              <span v-if="integrationStats.isGoogleConnected" class="int-connected-badge">
                                <i class="fa-solid fa-circle-check me-1"></i>Connecté
                              </span>
                            </div>
                            <p class="text-muted small m-0 mb-2">{{ t('settings.labels.googleDesc') }}</p>
                            <div class="int-tags">
                              <span class="int-tag"><i class="fa-solid fa-envelope me-1"></i>Gmail</span>
                              
                            </div>
                            <span v-if="integrationStats.isGoogleConnected" class="int-email-chip mt-2 d-inline-flex">
                              <i class="fa-regular fa-envelope me-1"></i>{{ integrationStats.connectedEmail }}
                            </span>
                          </div>
                          <div class="int-actions">
                            <button v-if="!integrationStats.isGoogleConnected" @click="connectGmail" class="btn-enigma-primary py-2 px-4">
                              <div class="btn-content"><i class="fa-solid fa-link me-2"></i>{{ t('settings.actions.connect') }}</div>
                              <div class="btn-glow"></div>
                              <div class="btn-shimmer"></div>
                            </button>
                            <button v-else @click="disconnectGmail" class="btn-disconnect">
                              <i class="fa-solid fa-link-slash me-2"></i>{{ t('settings.actions.disconnect') }}
                            </button>
                          </div>
                        </div>
                        <div v-if="integrationStats.isGoogleConnected" class="int-active-bar"></div>
                        <div class="integration-card-shine"></div>
                      </div>
                    </div>

                    <!-- Security Note -->
                    <div class="security-note mt-4">
                      <div class="sec-note-icon">
                        <i class="fa-solid fa-shield-halved"></i>
                      </div>
                      <div>
                        <div class="sec-note-title">Connexion sécurisée</div>
                        <span class="sec-note-text">{{ t('settings.integrations.oauthNote') }}</span>
                      </div>
                    </div>
                  </div>

                  <!-- FOOTER ACTIONS -->
                  <div class="footer-actions mt-5 pt-4">
                    <button class="btn-ghost-action" @click="resetForm">
                      <i class="fa-solid fa-rotate-left me-2"></i>{{ t('settings.actions.cancel') }}
                    </button>
                    <button @click="saveChanges" class="btn-enigma-primary px-5 py-3" :disabled="saving">
                      <div class="btn-content">
                        <span v-if="saving"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ t('settings.actions.syncing') }}</span>
                        <span v-else>{{ t('settings.actions.save') }} <i class="fa-solid fa-cloud-arrow-up ms-2"></i></span>
                      </div>
                      <div class="btn-glow"></div>
                      <div class="btn-shimmer"></div>
                    </button>
                  </div>

                </div>
              </div>
            </div>
          </div>

          <!-- LOADING STATE -->
          <div v-else class="loading-state">
            <div class="spinner-pro-premium">
              <div class="spinner-ring ring-1"></div>
              <div class="spinner-ring ring-2"></div>
              <div class="spinner-core"></div>
            </div>
            <p class="mt-4 fw-800 text-muted text-uppercase small tracking-wider">{{ t('settings.actions.loadingCore') }}</p>
          </div>

        </div><!-- /content-area -->
      </main>
    </div><!-- /main-viewport -->

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico-wrap">
          <div class="t-ico"><i :class="globalToast.icon"></i></div>
        </div>
        <div class="t-body">
          <strong>SYSTEM MESSAGE</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
        <div class="t-progress"></div>
      </div>
    </transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, reactive } from 'vue';
import { useI18n } from 'vue-i18n';
import api from '@/services/api';
import { useAuthStore } from '@/stores/auth';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';

const { t } = useI18n();
const authStore = useAuthStore();

const loading = ref(true);
const saving = ref(false);
const role = ref('');
const activeTab = ref('profile');
const integrationStats = ref({ isGoogleConnected: false, connectedEmail: null });
const photoInput = ref(null);

const userForm = ref({ nom: '', prenom: '', email: '', photoUrl: '', joinDate: '' });
const securityForm = ref({ currentPassword: '', newPassword: '', confirmPassword: '' });
const brandForm = ref({
  companyName: '',
  color: '#eab308',
  domaine: '',
  secteur: '',
  siteWeb: '',
  ville: '',
  pays: '',
  codePostal: '',
  adresse: '',
  description: '',
  matriculeFiscale: ''
});

const mousePos = reactive({ x: 0, y: 0 });
const globalToast = reactive({ active: false, message: '', type: '', icon: '' });

const colorPresets = ['#eab308', '#f43f5e', '#6366f1', '#10b981', '#06b6d4', '#f97316', '#0f172a'];

const profileDisplayUrl = computed(() => {
  if (userForm.value.photoUrl) return `${import.meta.env.VITE_BASE_URL || 'http://localhost:5172'}${userForm.value.photoUrl}`;
  return `https://ui-avatars.com/api/?name=${userForm.value.prenom}+${userForm.value.nom}&background=0f172a&color=eab308&size=128`;
});

const allTabs = [
  { id: 'profile',      icon: 'fa-solid fa-user-gear',     roles: ['SuperAdmin','AdminEntreprise','Recruteur','Evaluateur','Candidat'] },
  { id: 'security',     icon: 'fa-solid fa-shield-halved', roles: ['SuperAdmin','AdminEntreprise','Recruteur','Evaluateur','Candidat'] },
  { id: 'branding',     icon: 'fa-solid fa-palette',       roles: ['AdminEntreprise'] },
  { id: 'integrations', icon: 'fa-solid fa-plug',          roles: ['AdminEntreprise','SuperAdmin'] },
];

const filteredTabs = computed(() => {
  const currentRole = (role.value || '').toLowerCase();
  return allTabs.filter(tab => {
    if (tab.id === 'profile' || tab.id === 'security') return true;
    return tab.roles.some(r => r.toLowerCase() === currentRole);
  });
});

const roleDisplay = computed(() => {
  if (!role.value) return t('roles.User');
  return t(`roles.${role.value}`, role.value);
});

const passwordStrength = computed(() => {
  const p = securityForm.value.newPassword;
  if (!p) return 0;
  let score = 0;
  if (p.length >= 8)  score++;
  if (p.length >= 12) score++;
  if (/[A-Z]/.test(p)) score++;
  if (/[0-9]/.test(p)) score++;
  if (/[^A-Za-z0-9]/.test(p)) score++;
  return score;
});

const strengthColor = computed(() => {
  return ['#f43f5e','#f97316','#eab308','#06b6d4','#10b981'][passwordStrength.value - 1] || '#f43f5e';
});

const strengthLabel = computed(() => {
  return ['Très faible','Faible','Moyen','Fort','Excellent'][passwordStrength.value - 1] || 'Très faible';
});

const getSectionSubtitle = (tab) => {
  const map = {
    profile:      'Gérez vos informations personnelles',
    security:     "Sécurisez l'accès à votre compte",
    branding:     "Personnalisez l'identité de votre entreprise",
    integrations: 'Connectez vos outils externes',
  };
  return map[tab] || '';
};

const fetchInitialData = async () => {
  loading.value = true;
  try {
    const resUser = await api.get('/Settings/me');
    userForm.value = resUser.data;
    if (resUser.data.photoUrl && authStore.user.photoUrl !== resUser.data.photoUrl) {
      authStore.user.photoUrl = resUser.data.photoUrl;
      localStorage.setItem('user', JSON.stringify(authStore.user));
    }

    const currentRole = role.value.toLowerCase();

    if (currentRole === 'adminentreprise') {
      const resBrand = await api.get('/Settings/branding');
      brandForm.value = resBrand.data;
    }

    if (currentRole === 'adminentreprise' || currentRole === 'superadmin') {
      const resDiag = await api.get('/Settings/mailer-diag');
      integrationStats.value.isGoogleConnected = resDiag.data.isGoogleConnected;
      integrationStats.value.connectedEmail = resDiag.data.email;
    }
  } catch (error) {
    console.error(error);
  } finally {
    loading.value = false;
  }
};

const saveChanges = async () => {
  saving.value = true;
  try {
    let endpoint = '';
    let payload = {};
    if (activeTab.value === 'profile') {
      endpoint = '/Settings/update-profile';
      payload = userForm.value;
    } else if (activeTab.value === 'security') {
      if (securityForm.value.newPassword !== securityForm.value.confirmPassword) {
        showToast(t('settings.alerts.passMismatch'), 'error', 'fa-solid fa-triangle-exclamation');
        saving.value = false;
        return;
      }
      endpoint = '/Settings/change-password';
      payload = securityForm.value;
    } else if (activeTab.value === 'branding') {
      endpoint = '/Settings/update-branding';
      payload = brandForm.value;
    }
    await api.post(endpoint, payload);
    showToast(t('settings.alerts.syncSuccess'), 'success', 'fa-solid fa-cloud-arrow-up');
  } catch (error) {
    showToast(error.response?.data?.message || t('profile.uploadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    saving.value = false;
  }
};

const handlePhotoChange = async (e) => {
  const file = e.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    saving.value = true;
    const res = await api.post('/Settings/upload-photo', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    userForm.value.photoUrl = res.data.photoUrl;
    authStore.user.photoUrl = res.data.photoUrl;
    localStorage.setItem('user', JSON.stringify(authStore.user));
    showToast(t('profile.uploadSuccess'), 'success', 'fa-solid fa-camera');
  } catch {
    showToast(t('profile.uploadError'), 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    saving.value = false;
  }
};

const disconnectGmail = async () => {
  if (!confirm(t('settings.alerts.disconnectConfirm'))) return;
  try {
    await api.post('/GoogleAuth/disconnect');
    integrationStats.value.isGoogleConnected = false;
    showToast(t('settings.alerts.disconnectSuccess'), 'success', 'fa-solid fa-link-slash');
  } catch {
    showToast(t('settings.alerts.authError'), 'error', 'fa-solid fa-triangle-exclamation');
  }
};

const connectGmail = async () => {
  try {
    const res = await api.get('/GoogleAuth/auth-url');
    window.location.href = res.data.url;
  } catch {
    showToast(t('settings.alerts.authError'), 'error', 'fa-solid fa-triangle-exclamation');
  }
};

let _toastTimer = null;
const showToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

const resetForm = () => fetchInitialData();
const triggerPhotoUpload = () => photoInput.value.click();

const orbStyle = (f) => ({
  transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)`
});
const handleParallax = (e) => {
  mousePos.x = (e.clientX - window.innerWidth / 2) / 20;
  mousePos.y = (e.clientY - window.innerHeight / 2) / 20;
};

onMounted(() => {
  role.value = authStore.role || localStorage.getItem('role') || 'Candidat';
  fetchInitialData();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800;900&family=JetBrains+Mono:wght@600;700&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.2/css/all.min.css');

/* ═══════════════════════════════════════════
   ROOT & LAYOUT
═══════════════════════════════════════════ */
.settings-master-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: var(--bg-page, #f8fafc);
  color: var(--text-main, #0f172a);
  position: relative;
  overflow-x: hidden;
  display: flex;
  transition: background-color 0.35s ease;
}
.main-viewport { z-index: 10; }
.canvas-engine { height: calc(100vh - 64px); }
.content-area  { position: relative; z-index: 20; }

.custom-scrollbar::-webkit-scrollbar       { width: 6px; }
.custom-scrollbar::-webkit-scrollbar-track { background: transparent; }
.custom-scrollbar::-webkit-scrollbar-thumb { background: rgba(148,163,184,0.35); border-radius: 10px; }
.custom-scrollbar::-webkit-scrollbar-thumb:hover { background: rgba(148,163,184,0.6); }

/* ═══════════════════════════════════════════
   BACKGROUND
═══════════════════════════════════════════ */
.cyber-engine-bg { position: fixed; inset: 0; z-index: 0; pointer-events: none; }

.quantum-grid {
  position: absolute; inset: 0;
  background-image:
    linear-gradient(rgba(148,163,184,0.08) 1px, transparent 1px),
    linear-gradient(90deg, rgba(148,163,184,0.08) 1px, transparent 1px);
  background-size: 48px 48px;
}
[data-theme="dark"] .quantum-grid {
  background-image:
    linear-gradient(rgba(255,255,255,0.04) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255,255,255,0.04) 1px, transparent 1px);
}

.mesh-overlay {
  position: absolute; inset: 0;
  background:
    radial-gradient(ellipse 80% 50% at 20% 10%, rgba(245,158,11,0.04) 0%, transparent 60%),
    radial-gradient(ellipse 60% 40% at 80% 90%, rgba(99,102,241,0.04) 0%, transparent 60%);
}

.glow-orb {
  position: absolute; border-radius: 50%;
  filter: blur(130px); opacity: 0.1;
  transition: transform 0.4s ease-out;
}
[data-theme="dark"] .glow-orb { opacity: 0.16; }
.orb-amber  { width: 700px; height: 700px; background: #fbbf24; top: -250px; right: -150px; }
.orb-indigo { width: 500px; height: 500px; background: #818cf8; bottom: -200px; left: -100px; }
.orb-rose   { width: 400px; height: 400px; background: #fb7185; opacity: 0.04; top: 40%; right: 20%; }
[data-theme="dark"] .orb-amber  { opacity: 0.1; }
[data-theme="dark"] .orb-indigo { opacity: 0.1; }
[data-theme="dark"] .orb-rose   { opacity: 0.07; }

/* ═══════════════════════════════════════════
   PAGE HEADER
═══════════════════════════════════════════ */
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  flex-wrap: wrap;
  gap: 16px;
}
.premium-title {
  font-weight: 900;
  font-size: clamp(1.4rem, 2.5vw, 2.1rem);
  letter-spacing: -1.5px;
  margin: 0;
  line-height: 1.1;
  color: var(--text-main, #0f172a);
  transition: color 0.3s;
}
.gradient-text {
  background: linear-gradient(135deg, #f59e0b 0%, #fbbf24 50%, #f59e0b 100%);
  background-size: 200% auto;
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  animation: textShimmer 4s linear infinite;
}
@keyframes textShimmer { to { background-position: 200% center; } }

.breadcrumb-pro { font-size: 0.7rem; font-weight: 700; color: #94a3b8; display: flex; align-items: center; }
.breadcrumb-pro .root:hover { color: #f59e0b; cursor: pointer; }
.breadcrumb-pro .separator { font-size: 0.5rem; opacity: 0.4; }
.breadcrumb-pro .current { color: #0f172a; font-weight: 800; }
[data-theme="dark"] .breadcrumb-pro .current { color: #e2e8f0; }
.brand-subtitle {
  color: #94a3b8;
  font-size: 0.68rem;
  font-weight: 800;
  letter-spacing: 1.5px;
  text-transform: uppercase;
}

.header-right { display: flex; align-items: flex-end; }
.header-live-chip {
  display: flex; align-items: center; gap: 8px;
  background: white; border: 1px solid #e2e8f0;
  border-radius: 50px; padding: 8px 16px;
  font-size: 0.6rem; font-weight: 800; color: #64748b;
  letter-spacing: 1px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.05);
}
[data-theme="dark"] .header-live-chip { background: #1e293b; border-color: rgba(255,255,255,0.08); color: #8b949e; }
.live-dot {
  width: 8px; height: 8px; border-radius: 50%; background: #10b981;
  box-shadow: 0 0 0 3px rgba(16,185,129,0.2);
  animation: livePulse 2s ease-in-out infinite;
}
@keyframes livePulse { 0%,100%{box-shadow:0 0 0 3px rgba(16,185,129,0.2)} 50%{box-shadow:0 0 0 6px rgba(16,185,129,0.08)} }

/* ═══════════════════════════════════════════
   NAV PANEL
═══════════════════════════════════════════ */
.nav-panel {
  background: rgba(255,255,255,0.85);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-radius: 28px;
  padding: 24px;
  border: 1px solid rgba(255,255,255,0.9);
  box-shadow: 0 1px 0 rgba(255,255,255,1) inset, 0 8px 32px rgba(15,23,42,0.08), 0 2px 8px rgba(15,23,42,0.04);
}
[data-theme="dark"] .nav-panel {
  background: rgba(22,27,34,0.85);
  border-color: rgba(255,255,255,0.06);
  box-shadow: 0 8px 32px rgba(0,0,0,0.3), 0 2px 8px rgba(0,0,0,0.2);
}

.nav-panel-header {
  display: flex; align-items: center;
  padding-bottom: 20px;
  border-bottom: 1px solid #f1f5f9;
  margin-bottom: 16px;
}
[data-theme="dark"] .nav-panel-header { border-bottom-color: rgba(255,255,255,0.06); }

.avatar-display { position: relative; width: 50px; height: 50px; flex-shrink: 0; }
.avatar-img {
  width: 50px; height: 50px; border-radius: 16px;
  object-fit: cover; border: 3px solid white;
  box-shadow: 0 4px 12px rgba(0,0,0,0.12);
  position: relative; z-index: 2;
  transition: transform 0.2s, box-shadow 0.2s;
}
.avatar-img:hover { transform: scale(1.05); box-shadow: 0 6px 18px rgba(0,0,0,0.18); }
.avatar-status-ring {
  position: absolute; bottom: -2px; right: -2px;
  width: 13px; height: 13px; border-radius: 50%;
  background: #10b981; border: 2px solid white; z-index: 3;
  animation: statusPulse 3s ease-in-out infinite;
}
@keyframes statusPulse { 0%,100%{box-shadow:0 0 0 0 rgba(16,185,129,0.4)} 50%{box-shadow:0 0 0 4px rgba(16,185,129,0)} }

.avatar-glow {
  position: absolute; inset: -4px; border-radius: 20px;
  background: linear-gradient(135deg, #f59e0b33, #6366f133);
  filter: blur(8px); z-index: 1;
  animation: glowRotate 4s linear infinite;
}
@keyframes glowRotate { to { filter: blur(8px) hue-rotate(360deg); } }

.avatar-info h6 { font-size: 0.88rem; color: #0f172a; }
[data-theme="dark"] .avatar-info h6 { color: #f0f6fc; }
.role-badge-inline {
  font-size: 0.58rem; font-weight: 900;
  background: linear-gradient(135deg, #fffbeb, #fef3c7);
  color: #b45309;
  padding: 3px 10px; border-radius: 8px;
  text-transform: uppercase; letter-spacing: 0.5px;
  border: 1px solid #fde68a;
}
[data-theme="dark"] .role-badge-inline { background: rgba(245,158,11,0.12); color: #fbbf24; border-color: rgba(245,158,11,0.2); }

.settings-nav-matrix { display: flex; flex-direction: column; gap: 5px; }
.nav-matrix-btn {
  display: flex; align-items: center; gap: 12px;
  padding: 12px 14px; background: transparent;
  border: none; border-radius: 16px;
  color: #64748b; font-weight: 800; font-size: 0.68rem;
  transition: all 0.2s cubic-bezier(0.4,0,0.2,1);
  cursor: pointer; font-family: inherit; width: 100%;
  text-align: left; letter-spacing: 0.3px;
  position: relative; overflow: hidden;
}
.nav-matrix-btn::before {
  content: ''; position: absolute; left: 0; top: 0; bottom: 0;
  width: 3px; border-radius: 0 3px 3px 0;
  background: #f59e0b; transform: scaleY(0); transition: transform 0.2s;
}
.nav-matrix-btn:hover { background: #f8fafc; color: #0f172a; transform: translateX(3px); }
.nav-matrix-btn:hover::before { transform: scaleY(0.5); }
[data-theme="dark"] .nav-matrix-btn:hover { background: rgba(255,255,255,0.05); color: #f0f6fc; }
.nav-matrix-btn.active {
  background: #0f172a; color: #fff;
  box-shadow: 0 8px 24px rgba(15,23,42,0.18);
  transform: translateX(0);
}
.nav-matrix-btn.active::before { transform: scaleY(1); background: #fbbf24; }
.nav-matrix-btn.active .icon-shell { background: rgba(255,255,255,0.1); color: #f59e0b; }
.nav-arrow { font-size: 0.55rem; opacity: 0.5; }

.icon-shell {
  width: 34px; height: 34px; background: #f1f5f9;
  border-radius: 10px; display: flex;
  align-items: center; justify-content: center;
  flex-shrink: 0; font-size: 0.82rem; transition: 0.2s;
}
[data-theme="dark"] .icon-shell { background: rgba(255,255,255,0.06); }
.nav-matrix-btn:hover .icon-shell { background: #e2e8f0; transform: scale(1.08) rotate(-2deg); }
[data-theme="dark"] .nav-matrix-btn:hover .icon-shell { background: rgba(255,255,255,0.1); }

.nav-panel-footer { border-top: 1px solid #f1f5f9; padding-top: 16px; margin-top: 16px; }
[data-theme="dark"] .nav-panel-footer { border-top-color: rgba(255,255,255,0.06); }
.join-date-widget { display: flex; align-items: center; font-size: 0.7rem; }
.text-amber { color: #f59e0b !important; }
.text-indigo { color: #6366f1 !important; }

/* ═══════════════════════════════════════════
   CONTENT PANEL
═══════════════════════════════════════════ */
.content-panel {
  background: rgba(255,255,255,0.88);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  border-radius: 28px;
  padding: 36px 40px;
  border: 1px solid rgba(255,255,255,0.9);
  position: relative;
  box-shadow: 0 1px 0 rgba(255,255,255,1) inset, 0 8px 32px rgba(15,23,42,0.08), 0 2px 8px rgba(15,23,42,0.04);
}
.content-panel::before {
  content: '';
  position: absolute; top: 0; left: 0; right: 0; height: 4px;
  background: linear-gradient(90deg, #f59e0b, #fbbf24, #f59e0b);
  background-size: 200% auto;
  border-radius: 28px 28px 0 0;
  animation: borderShimmer 3s linear infinite;
}
@keyframes borderShimmer { to { background-position: 200% center; } }
[data-theme="dark"] .content-panel {
  background: rgba(22,27,34,0.88);
  border-color: rgba(255,255,255,0.06);
  box-shadow: 0 8px 32px rgba(0,0,0,0.3), 0 2px 8px rgba(0,0,0,0.2);
}

.section-title-bar {
  display: flex; align-items: center; gap: 18px;
  padding-bottom: 24px;
  border-bottom: 1px solid #f1f5f9;
}
[data-theme="dark"] .section-title-bar { border-bottom-color: rgba(255,255,255,0.06); }
.section-icon-box {
  width: 50px; height: 50px;
  background: linear-gradient(135deg, #fffbeb, #fef3c7);
  color: #d97706; border-radius: 16px; display: flex;
  align-items: center; justify-content: center;
  font-size: 1.15rem; flex-shrink: 0;
  border: 1px solid #fde68a;
  box-shadow: 0 4px 14px rgba(245,158,11,0.15);
  position: relative;
}
.section-icon-box::after {
  content: ''; position: absolute; inset: 0; border-radius: 16px;
  background: linear-gradient(135deg, rgba(255,255,255,0.4), transparent);
}
[data-theme="dark"] .section-icon-box { background: rgba(245,158,11,0.1); border-color: rgba(245,158,11,0.2); }

.section-title-bar h5 { color: #0f172a; font-size: 0.98rem; }
[data-theme="dark"] .section-title-bar h5 { color: #f0f6fc; }

.status-badge {
  padding: 5px 14px; border-radius: 10px;
  font-size: 0.6rem; font-weight: 800;
  text-transform: uppercase;
  display: inline-flex; align-items: center;
}
.status-active {
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  color: #065f46; border: 1px solid #a7f3d0;
}
[data-theme="dark"] .status-active { background: rgba(16,185,129,0.12); color: #34d399; border-color: rgba(16,185,129,0.2); }
.status-dot {
  width: 6px; height: 6px; border-radius: 50%;
  background: currentColor; margin-right: 6px;
  animation: pulse 2s infinite;
}
@keyframes pulse { 0%,100%{opacity:1} 50%{opacity:0.3} }

/* ═══════════════════════════════════════════
   PROFILE HERO
═══════════════════════════════════════════ */
.profile-hero-card {
  border-radius: 20px; border: 1px solid #eef2f6;
  overflow: hidden; position: relative;
  box-shadow: 0 4px 24px rgba(0,0,0,0.07);
}
[data-theme="dark"] .profile-hero-card { border-color: rgba(255,255,255,0.06); }
.profile-hero-bg {
  height: 88px;
  background: linear-gradient(135deg, #0a0f1e 0%, #111827 30%, #0f172a 60%, #141022 100%);
  position: relative; overflow: hidden;
}
.profile-hero-bg::before {
  content: '';
  position: absolute; inset: 0;
  background:
    radial-gradient(ellipse 60% 80% at 20% 50%, rgba(245,158,11,0.12), transparent),
    radial-gradient(ellipse 40% 60% at 80% 30%, rgba(99,102,241,0.1), transparent);
}
.hero-particles { position: absolute; inset: 0; }
.particle {
  position: absolute; top: 50%; left: 50%;
  width: 2px; height: 2px; border-radius: 50%;
  background: rgba(245,158,11,0.6);
  transform: rotate(var(--d)) translateX(var(--r));
  animation: particleFloat 6s ease-in-out infinite;
  animation-delay: calc(var(--d) * 0.01s);
}
@keyframes particleFloat {
  0%,100%{opacity:0.3;transform:rotate(var(--d)) translateX(var(--r))}
  50%{opacity:0.8;transform:rotate(var(--d)) translateX(calc(var(--r) + 8px))}
}
.profile-hero-content {
  padding: 0 28px 24px;
  display: flex; align-items: flex-end;
  margin-top: -30px;
}
.avatar-upload-zone {
  position: relative; width: 88px; height: 88px;
  cursor: pointer; flex-shrink: 0;
  border-radius: 20px; overflow: hidden;
  border: 4px solid white;
  box-shadow: 0 8px 24px rgba(0,0,0,0.14);
  transition: transform 0.25s cubic-bezier(0.34,1.56,0.64,1), box-shadow 0.25s;
}
.avatar-upload-zone:hover { transform: scale(1.06) translateY(-2px); box-shadow: 0 14px 36px rgba(0,0,0,0.24); }
.avatar-large { width: 100%; height: 100%; object-fit: cover; }
.avatar-upload-overlay {
  position: absolute; inset: 0;
  background: rgba(15,23,42,0.78);
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  color: white; font-size: 0.68rem; font-weight: 800;
  opacity: 0; transition: 0.25s; gap: 5px;
}
.avatar-upload-zone:hover .avatar-upload-overlay { opacity: 1; }
.profile-hero-data h3 { color: #0f172a; font-size: 1.3rem; }
[data-theme="dark"] .profile-hero-data h3 { color: #f0f6fc; }
.hero-badge {
  font-size: 0.68rem; font-weight: 800;
  background: rgba(255,255,255,0.8);
  backdrop-filter: blur(10px);
  border: 1.5px solid rgba(226,232,240,0.8);
  padding: 4px 12px; border-radius: 10px; color: #64748b;
  transition: all 0.2s;
}
.hero-badge:hover { border-color: #cbd5e1; background: white; }
[data-theme="dark"] .hero-badge { background: rgba(255,255,255,0.06); border-color: rgba(255,255,255,0.1); color: #8b949e; }

/* ═══════════════════════════════════════════
   FORM INPUTS
═══════════════════════════════════════════ */
.enigma-input-wrap label {
  font-size: 0.6rem; font-weight: 900;
  color: #94a3b8; letter-spacing: 1px;
  margin-bottom: 8px; display: block;
  transition: color 0.2s;
}
.enigma-input-wrap:focus-within label { color: #d97706; }
[data-theme="dark"] .enigma-input-wrap:focus-within label { color: #fbbf24; }
.input-with-icon { position: relative; }
.input-icon {
  position: absolute; left: 16px; top: 50%;
  transform: translateY(-50%); color: #d1d5db;
  font-size: 0.78rem; z-index: 2; transition: all 0.25s;
}
.input-with-icon:focus-within .input-icon { color: #f59e0b; transform: translateY(-50%) scale(1.1); }
.enigma-field {
  width: 100%; padding: 14px 18px 14px 44px;
  background: rgba(248,250,252,0.8);
  border: 1.5px solid #e8ecf1;
  border-radius: 14px; outline: none;
  font-weight: 700; font-family: inherit;
  transition: all 0.25s cubic-bezier(0.4,0,0.2,1); font-size: 0.88rem;
  color: #0f172a;
}
.enigma-field.no-icon { padding-left: 18px; }
textarea.enigma-field { padding-left: 18px; resize: vertical; }
.enigma-field:hover { border-color: #d1d5db; background: rgba(241,245,249,0.9); }
.enigma-field:focus {
  border-color: #f59e0b; background: white;
  box-shadow: 0 0 0 4px rgba(245,158,11,0.08);
}
[data-theme="dark"] .enigma-field { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .enigma-field:hover { background: rgba(255,255,255,0.07); border-color: rgba(255,255,255,0.13); }
[data-theme="dark"] .enigma-field:focus { border-color: #f59e0b; background: rgba(255,255,255,0.08); }
[data-theme="dark"] .input-icon { color: rgba(255,255,255,0.18); }
[data-theme="dark"] .input-with-icon:focus-within .input-icon { color: #f59e0b; }

.field-shine {
  position: absolute; top: 0; left: -100%; right: 0; bottom: 0;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.4), transparent);
  pointer-events: none; border-radius: 14px;
  transition: left 0.4s ease;
}
.input-with-icon:focus-within .field-shine { left: 100%; }

/* ═══════════════════════════════════════════
   SECURITY
═══════════════════════════════════════════ */
.security-status-widget {
  background: linear-gradient(135deg, #fffbeb 0%, #fefce8 50%, #fffbeb 100%);
  border: 1.5px solid #fde68a; border-radius: 22px;
  padding: 24px 28px; display: flex; align-items: center;
  justify-content: space-between; gap: 20px; flex-wrap: wrap;
  position: relative; overflow: hidden;
  box-shadow: 0 4px 20px rgba(245,158,11,0.08);
}
.security-status-widget::before {
  content: ''; position: absolute; top: 0; right: 0;
  width: 200px; height: 100%;
  background: radial-gradient(ellipse at right center, rgba(245,158,11,0.08), transparent);
}
[data-theme="dark"] .security-status-widget { background: rgba(245,158,11,0.06); border-color: rgba(245,158,11,0.2); }

.sec-status-left { display: flex; align-items: center; }
.sec-icon-ring {
  width: 54px; height: 54px; position: relative;
  background: linear-gradient(145deg, #f59e0b, #d97706);
  border-radius: 16px; display: flex;
  align-items: center; justify-content: center;
  color: white; font-size: 1.25rem; flex-shrink: 0;
  box-shadow: 0 8px 20px rgba(245,158,11,0.28);
}
.sec-icon-ring::after {
  content: ''; position: absolute; inset: 0; border-radius: 16px;
  background: linear-gradient(135deg, rgba(255,255,255,0.3), transparent);
}
.sec-icon-pulse {
  position: absolute; inset: -4px; border-radius: 20px;
  border: 2px solid rgba(245,158,11,0.3);
  animation: secPulse 2s ease-in-out infinite;
}
@keyframes secPulse { 0%,100%{transform:scale(1);opacity:0.6} 50%{transform:scale(1.08);opacity:0} }

.sec-health-bar-wrap { display: flex; flex-direction: column; align-items: flex-end; gap: 6px; }
.sec-health-label-top { font-size: 0.65rem; font-weight: 900; color: #10b981; letter-spacing: 1.5px; }
.sec-health-bar {
  width: 140px; height: 8px; background: rgba(226,232,240,0.8);
  border-radius: 10px; overflow: hidden;
  box-shadow: inset 0 1px 3px rgba(0,0,0,0.08);
}
.sec-health-fill {
  height: 100%; position: relative; border-radius: 10px;
  background: linear-gradient(90deg, #10b981, #34d399, #6ee7b7);
  background-size: 200% auto;
  animation: healthFlow 2s linear infinite;
}
@keyframes healthFlow { to { background-position: 200% center; } }
.sec-health-shimmer {
  position: absolute; inset: 0; border-radius: 10px;
  background: linear-gradient(90deg, transparent 0%, rgba(255,255,255,0.5) 50%, transparent 100%);
  animation: shimmerMove 1.5s ease-in-out infinite;
}
@keyframes shimmerMove { from{transform:translateX(-100%)} to{transform:translateX(200%)} }
.sec-health-pct { font-size: 0.62rem; font-weight: 900; color: #94a3b8; }

.password-strength-analyzer {
  background: rgba(248,250,252,0.8); border-radius: 18px;
  padding: 20px 24px; border: 1.5px solid #eef2f6;
  transition: border-color 0.2s, box-shadow 0.2s;
}
.password-strength-analyzer:hover { border-color: #e2e8f0; box-shadow: 0 4px 14px rgba(0,0,0,0.04); }
[data-theme="dark"] .password-strength-analyzer { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }

.psa-header { display: flex; align-items: center; justify-content: space-between; margin-bottom: 12px; }
.psa-label { font-size: 0.55rem; font-weight: 900; color: #94a3b8; letter-spacing: 1.2px; }
.psa-bars { display: flex; gap: 6px; margin-bottom: 14px; }
.psa-bar {
  flex: 1; height: 6px; background: #e8ecf1;
  border-radius: 10px; transition: all 0.4s cubic-bezier(0.34,1.56,0.64,1);
}
.psa-bar.filled { transform: scaleY(1.3); }
.psa-text { font-size: 0.7rem; font-weight: 800; }

.psa-criteria { display: flex; gap: 12px; flex-wrap: wrap; }
.criterion {
  display: flex; align-items: center; gap: 6px;
  font-size: 0.7rem; font-weight: 700; color: #cbd5e1;
  transition: color 0.3s;
}
.criterion.met { color: #10b981; }
.criterion i { font-size: 0.7rem; }

.protocol-row {
  display: flex; align-items: center; gap: 16px;
  background: rgba(248,250,252,0.8); border-radius: 18px; padding: 20px 24px;
  border: 1.5px solid #eef2f6;
  position: relative; overflow: hidden;
}
.protocol-row::before {
  content: ''; position: absolute; left: 0; top: 0; bottom: 0;
  width: 3px; background: linear-gradient(180deg, #f59e0b, #fbbf24);
  border-radius: 0 2px 2px 0;
}
.protocol-row:hover { border-color: #e2e8f0; box-shadow: 0 4px 14px rgba(0,0,0,0.04); }
[data-theme="dark"] .protocol-row { background: rgba(255,255,255,0.03); border-color: rgba(255,255,255,0.06); }

.p-icon {
  width: 44px; height: 44px;
  background: linear-gradient(135deg, #fffbeb, #fef3c7);
  border-radius: 12px; display: flex;
  align-items: center; justify-content: center;
  font-size: 1rem; color: #d97706; flex-shrink: 0;
  border: 1px solid #fde68a;
  box-shadow: 0 4px 12px rgba(245,158,11,0.12);
}
[data-theme="dark"] .p-icon { background: rgba(245,158,11,0.1); border-color: rgba(245,158,11,0.15); }
.p-data { flex: 1; }
.p-data h6 { font-weight: 800; margin: 0; font-size: 0.88rem; color: #0f172a; }
[data-theme="dark"] .p-data h6 { color: #f0f6fc; }
.p-data p  { font-size: 0.72rem; color: #94a3b8; margin: 0; }
.badge-coming-soon {
  font-size: 0.55rem; font-weight: 900;
  background: linear-gradient(135deg, #f1f5f9, #e2e8f0);
  color: #94a3b8; padding: 5px 12px; border-radius: 8px;
  letter-spacing: 0.8px; white-space: nowrap; border: 1px solid #e2e8f0;
}
[data-theme="dark"] .badge-coming-soon { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }

/* ═══════════════════════════════════════════
   INTEGRATIONS
═══════════════════════════════════════════ */
.integrations-stats-row { display: grid; grid-template-columns: repeat(3, 1fr); gap: 14px; }

.int-stat-card {
  background: white; border: 1px solid #eef2f6; border-radius: 20px;
  padding: 18px 20px; display: flex; align-items: center; gap: 14px;
  transition: all 0.3s cubic-bezier(0.4,0,0.2,1);
  position: relative; overflow: hidden;
  box-shadow: 0 2px 10px rgba(0,0,0,0.04);
}
.int-stat-card:hover { transform: translateY(-4px); box-shadow: 0 10px 28px rgba(0,0,0,0.09); }
[data-theme="dark"] .int-stat-card { background: rgba(255,255,255,0.04); border-color: rgba(255,255,255,0.07); }

.stat-card-glow {
  position: absolute; bottom: -20px; right: -20px;
  width: 80px; height: 80px; border-radius: 50%;
  background: var(--gc, #f59e0b); opacity: 0.06; filter: blur(20px);
  transition: opacity 0.3s, transform 0.3s;
}
.int-stat-card:hover .stat-card-glow { opacity: 0.12; transform: scale(1.3); }

.int-stat-icon-wrap { position: relative; }
.int-stat-icon {
  width: 44px; height: 44px; border-radius: 14px;
  display: flex; align-items: center; justify-content: center;
  font-size: 1.05rem; flex-shrink: 0; position: relative;
}
.int-stat-icon::after {
  content: ''; position: absolute; inset: 0; border-radius: 14px;
  background: linear-gradient(135deg, rgba(255,255,255,0.5), transparent);
}
.int-stat-value { font-weight: 900; font-size: 1rem; color: #0f172a; }
[data-theme="dark"] .int-stat-value { color: #f0f6fc; }
.int-stat-label { font-size: 0.6rem; font-weight: 700; color: #94a3b8; letter-spacing: 0.5px; }

.stat-oauth2 {
  background: linear-gradient(135deg, #f0f9ff 0%, white 100%);
  border-color: rgba(14,165,233,0.2);
}
[data-theme="dark"] .stat-oauth2 { background: rgba(14,165,233,0.06); border-color: rgba(14,165,233,0.15); }
.oauth2-icon-shell { background: linear-gradient(135deg, #e0f2fe, #bae6fd) !important; }
.oauth2-value { color: #0284c7 !important; }
[data-theme="dark"] .oauth2-value { color: #38bdf8 !important; }
.oauth2-badge-corner {
  position: absolute; top: 12px; right: 12px;
  width: 22px; height: 22px; border-radius: 8px;
  background: rgba(14,165,233,0.12); color: #0ea5e9;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.6rem;
}

.integrations-grid { display: flex; flex-direction: column; gap: 14px; }
.integration-card {
  border: 1.5px solid #eef2f6; border-radius: 22px; overflow: hidden;
  background: white; transition: all 0.3s cubic-bezier(0.4,0,0.2,1);
  position: relative;
  box-shadow: 0 2px 10px rgba(0,0,0,0.04);
}
.integration-card::before {
  content: ''; position: absolute; inset: 0; border-radius: 22px;
  background: linear-gradient(135deg, rgba(255,255,255,0.6) 0%, transparent 60%);
  pointer-events: none; z-index: 1;
}
.integration-card:hover { border-color: #e2e8f0; box-shadow: 0 12px 40px rgba(0,0,0,0.1); transform: translateY(-3px); }
.integration-card.connected {
  border-color: rgba(16,185,129,0.25);
  background: linear-gradient(135deg, #f0fdf4 0%, white 60%);
  box-shadow: 0 4px 20px rgba(16,185,129,0.08);
}
[data-theme="dark"] .integration-card { background: #161b22; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .integration-card.connected { background: rgba(16,185,129,0.07); border-color: rgba(16,185,129,0.2); }

.integration-card-shine {
  position: absolute; top: 0; left: -100%; right: 0; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.15), transparent);
  transition: left 0.5s ease; z-index: 2; pointer-events: none;
  border-radius: 22px;
}
.integration-card:hover .integration-card-shine { left: 100%; }

.integration-card-inner {
  display: flex; align-items: center; gap: 20px;
  padding: 24px 28px; flex-wrap: wrap; position: relative; z-index: 3;
}
.int-icon-shell {
  width: 58px; height: 58px; background: white; border-radius: 18px;
  display: flex; align-items: center; justify-content: center; flex-shrink: 0;
  box-shadow: 0 4px 16px rgba(0,0,0,0.1), 0 1px 4px rgba(0,0,0,0.06);
  border: 1.5px solid #f1f5f9;
  transition: transform 0.25s cubic-bezier(0.34,1.56,0.64,1), box-shadow 0.25s;
}
.integration-card:hover .int-icon-shell { transform: scale(1.08) rotate(-3deg); box-shadow: 0 8px 24px rgba(0,0,0,0.14); }
[data-theme="dark"] .int-icon-shell { background: #0d1117; border-color: rgba(255,255,255,0.07); }

.int-connected-badge {
  font-size: 0.58px; font-weight: 900;
  background: linear-gradient(135deg, #ecfdf5, #d1fae5);
  color: #065f46; padding: 4px 12px; border-radius: 50px;
  border: 1px solid #a7f3d0; letter-spacing: 0.4px;
  font-size: 0.58rem;
}

.int-tags { display: flex; gap: 6px; flex-wrap: wrap; margin-top: 2px; }
.int-tag {
  font-size: 0.62rem; font-weight: 700; color: #64748b;
  background: #f8fafc; border: 1px solid #e8ecf1;
  padding: 3px 10px; border-radius: 8px; transition: all 0.2s;
}
.int-tag:hover { background: #f1f5f9; border-color: #d1d5db; color: #374151; }
[data-theme="dark"] .int-tag { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #8b949e; }

.int-email-chip {
  font-size: 0.72rem; font-weight: 700; background: white; color: #64748b;
  border: 1.5px solid #e8ecf1; padding: 5px 14px; border-radius: 10px;
  display: inline-flex; align-items: center;
  box-shadow: 0 2px 6px rgba(0,0,0,0.04);
}
[data-theme="dark"] .int-email-chip { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }

.int-active-bar {
  height: 3px;
  background: linear-gradient(90deg, #10b981, #34d399, #6ee7b7);
  background-size: 200% auto;
  animation: barFlow 2s linear infinite;
  box-shadow: 0 0 8px rgba(16,185,129,0.5);
}
@keyframes barFlow { to { background-position: 200% center; } }

.btn-disconnect {
  background: white; color: #e11d48; border: 1.5px solid #fecdd3;
  border-radius: 14px; padding: 10px 20px; font-weight: 800;
  font-size: 0.78rem; cursor: pointer; transition: all 0.22s; font-family: inherit;
}
.btn-disconnect:hover {
  background: #fff1f2; border-color: #f43f5e;
  box-shadow: 0 4px 14px rgba(244,63,94,0.18); transform: translateY(-1px);
}
[data-theme="dark"] .btn-disconnect { background: rgba(244,63,94,0.08); border-color: rgba(244,63,94,0.25); }

.security-note {
  background: linear-gradient(135deg, #fffbeb, #fef9ee);
  border: 1.5px solid #fde68a; border-radius: 18px;
  padding: 18px 22px; display: flex; align-items: flex-start; gap: 14px;
  box-shadow: 0 2px 10px rgba(245,158,11,0.07);
}
[data-theme="dark"] .security-note { background: rgba(245,158,11,0.07); border-color: rgba(245,158,11,0.2); }
.sec-note-icon {
  width: 36px; height: 36px; border-radius: 10px; flex-shrink: 0;
  background: linear-gradient(135deg, #fef3c7, #fde68a);
  display: flex; align-items: center; justify-content: center;
  color: #d97706; font-size: 0.9rem;
}
.sec-note-title { font-size: 0.7rem; font-weight: 900; color: #92400e; letter-spacing: 0.5px; margin-bottom: 2px; }
.sec-note-text { font-size: 0.76rem; color: #a16207; font-weight: 600; line-height: 1.5; }
[data-theme="dark"] .sec-note-title { color: #fde68a; }
[data-theme="dark"] .sec-note-text { color: #fbbf24; }

/* ═══════════════════════════════════════════
   BUTTONS
═══════════════════════════════════════════ */
.btn-enigma-primary {
  background: #0f172a; color: white;
  border: none; border-radius: 16px;
  font-weight: 800; position: relative;
  overflow: hidden; cursor: pointer;
  font-family: inherit; font-size: 0.82rem;
  transition: transform 0.2s cubic-bezier(0.34,1.56,0.64,1), box-shadow 0.2s;
  box-shadow: 0 4px 14px rgba(15,23,42,0.28), 0 1px 4px rgba(15,23,42,0.15);
}
.btn-enigma-primary:hover { transform: translateY(-2px); box-shadow: 0 8px 26px rgba(15,23,42,0.35), 0 2px 8px rgba(15,23,42,0.2); }
.btn-enigma-primary:active { transform: translateY(0); }
.btn-enigma-primary .btn-glow {
  position: absolute; inset: 0;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  opacity: 0; transition: 0.3s; z-index: 1;
}
.btn-enigma-primary:hover .btn-glow { opacity: 1; }
.btn-enigma-primary .btn-shimmer {
  position: absolute; top: 0; left: -100%; width: 60%; height: 100%;
  background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
  z-index: 3; transform: skewX(-15deg);
  transition: left 0.5s ease;
}
.btn-enigma-primary:hover .btn-shimmer { left: 150%; }
.btn-enigma-primary .btn-content {
  position: relative; z-index: 2;
  display: flex; align-items: center; justify-content: center;
}
.btn-enigma-primary:hover .btn-content { color: #0f172a; }
.btn-enigma-primary:disabled { opacity: 0.5; cursor: not-allowed; transform: none; box-shadow: none; }

.btn-ghost-action {
  background: transparent; border: 1.5px solid #e2e8f0;
  color: #64748b; font-weight: 800;
  font-size: 0.78rem; cursor: pointer;
  font-family: inherit; padding: 12px 20px;
  border-radius: 14px; transition: all 0.22s;
}
.btn-ghost-action:hover { background: #f8fafc; border-color: #cbd5e1; color: #0f172a; transform: translateY(-1px); }
[data-theme="dark"] .btn-ghost-action { border-color: rgba(255,255,255,0.1); color: #8b949e; }
[data-theme="dark"] .btn-ghost-action:hover { background: rgba(255,255,255,0.05); color: #f0f6fc; }

/* ═══════════════════════════════════════════
   FOOTER ACTIONS
═══════════════════════════════════════════ */
.footer-actions {
  display: flex; justify-content: space-between;
  align-items: center; border-top: 1px solid #f1f5f9;
  padding-top: 24px;
}
[data-theme="dark"] .footer-actions { border-top-color: rgba(255,255,255,0.06); }

/* ═══════════════════════════════════════════
   LOADING — Cinematic
═══════════════════════════════════════════ */
.loading-state {
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  min-height: 60vh;
}
.spinner-pro-premium { position: relative; width: 60px; height: 60px; }
.spinner-ring {
  position: absolute; inset: 0; border-radius: 50%; border: 3px solid transparent;
}
.ring-1 { border-top-color: #f59e0b; animation: spin1 1.2s linear infinite; }
.ring-2 { inset: 8px; border-right-color: #6366f1; animation: spin2 0.9s linear infinite reverse; }
.spinner-core {
  position: absolute; inset: 18px; border-radius: 50%;
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  animation: coreGlow 1.5s ease-in-out infinite alternate;
  box-shadow: 0 0 16px rgba(245,158,11,0.4);
}
@keyframes spin1 { to { transform: rotate(360deg); } }
@keyframes spin2 { to { transform: rotate(360deg); } }
@keyframes coreGlow { from{opacity:0.6;transform:scale(0.8)} to{opacity:1;transform:scale(1)} }
.tracking-wider { letter-spacing: 2px; }

/* ═══════════════════════════════════════════
   TOAST — Premium
═══════════════════════════════════════════ */
.enigma-toast {
  position: fixed; bottom: 30px; right: 30px;
  background: rgba(15,23,42,0.95); color: white;
  backdrop-filter: blur(20px); -webkit-backdrop-filter: blur(20px);
  padding: 18px 24px; border-radius: 20px;
  display: flex; align-items: center; gap: 14px;
  z-index: 3000;
  box-shadow: 0 20px 50px rgba(0,0,0,0.35), 0 4px 16px rgba(0,0,0,0.2);
  min-width: 290px; overflow: hidden;
  border: 1px solid rgba(255,255,255,0.08);
}
.t-ico-wrap {
  width: 36px; height: 36px; border-radius: 10px; flex-shrink: 0;
  background: rgba(255,255,255,0.08); display: flex; align-items: center; justify-content: center;
}
.t-ico { font-size: 1rem; }
.t-success .t-ico-wrap { background: rgba(16,185,129,0.2); color: #34d399; }
.t-error   .t-ico-wrap { background: rgba(244,63,94,0.2);  color: #fb7185; }
.t-warn    .t-ico-wrap { background: rgba(245,158,11,0.2); color: #fbbf24; }
.t-body strong { font-size: 0.62rem; letter-spacing: 1px; opacity: 0.4; display: block; margin-bottom: 3px; }
.t-body p { font-size: 0.82rem; line-height: 1.4; }
.t-progress {
  position: absolute; bottom: 0; left: 0; right: 0; height: 2px;
  background: rgba(245,158,11,0.6);
  animation: toastProgress 4s linear forwards;
}
.t-success .t-progress { background: rgba(16,185,129,0.7); }
.t-error   .t-progress { background: rgba(244,63,94,0.7); }
@keyframes toastProgress { from{width:100%} to{width:0%} }

.toast-slide-enter-active { animation: slideIn 0.4s cubic-bezier(0.34,1.56,0.64,1); }
.toast-slide-leave-active { animation: slideIn 0.25s cubic-bezier(0.4,0,0.2,1) reverse; }
@keyframes slideIn { from { transform: translateX(120%) scale(0.85); opacity: 0; } to { transform: translateX(0) scale(1); opacity: 1; } }

/* ═══════════════════════════════════════════
   UTILS
═══════════════════════════════════════════ */
.fw-900 { font-weight: 900 !important; }
.fw-800 { font-weight: 800 !important; }

/* ═══════════════════════════════════════════
   DARK MODE
═══════════════════════════════════════════ */
[data-theme="dark"] .settings-master-root { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .nav-panel { background: rgba(22,27,34,0.85); border-color: rgba(255,255,255,0.06); }

[data-theme="dark"] .nav-matrix-btn.active { background: #0d1117; color: #f0f6fc; }
[data-theme="dark"] .avatar-img { border-color: rgba(255,255,255,0.1); }
[data-theme="dark"] .content-panel { background: rgba(22,27,34,0.88); border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .premium-title { color: #f0f6fc; }
[data-theme="dark"] .profile-hero-data h3 { color: #f0f6fc; }
[data-theme="dark"] .enigma-field { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); color: #f0f6fc; }
[data-theme="dark"] .enigma-field:focus { border-color: #f59e0b; background: rgba(255,255,255,0.08); }
[data-theme="dark"] .int-icon-shell { background: #0d1117; border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .int-email-chip { background: rgba(255,255,255,0.05); border-color: rgba(255,255,255,0.08); }
[data-theme="dark"] .footer-actions { border-top-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .profile-hero-card { border-color: rgba(255,255,255,0.06); }
[data-theme="dark"] .header-live-chip { background: #1e293b; border-color: rgba(255,255,255,0.08); color: #8b949e; }
</style>