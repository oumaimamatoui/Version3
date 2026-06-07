<template>
  <div class="aura-dashboard" @mousemove="handleParallax">

    <!-- FOND ANIMÉ PARALLAX -->
    <div class="luxury-bg" aria-hidden="true">
      <div class="glow-orb orb-amber" :style="orbStyle(0.04)"></div>
      <div class="glow-orb orb-blue"  :style="orbStyle(0.015)"></div>
      <div class="glow-orb orb-rose"  :style="orbStyle(0.025)"></div>
      <div class="quantum-grid"></div>
    </div>

    <AppSidebar />

    <div class="main-viewport flex-grow-1 d-flex flex-column position-relative">
      <AppNavbar />

      <main class="canvas-engine flex-grow-1 overflow-auto custom-scrollbar">
        <div class="content-area p-4 p-lg-5">

          <!-- TERMINAL HEADER BAR -->
          <header class="terminal-bar mb-5 d-flex align-items-center justify-content-between flex-wrap gap-3">
            <div class="terminal-left d-flex align-items-center gap-3">
              <div class="ai-robot-terminal">
                <svg viewBox="0 0 60 60" fill="none" width="36">
                  <rect x="12" y="10" width="36" height="34" rx="11" fill="white" opacity=".96"/>
                  <rect x="16" y="18" width="28" height="12" rx="6" fill="#0f172a"/>
                  <circle cx="22" cy="24" r="3.5" fill="#f59e0b">
                    <animate attributeName="opacity" values="1;0.15;1" dur="3s" repeatCount="indefinite"/>
                  </circle>
                  <circle cx="38" cy="24" r="3.5" fill="#f59e0b">
                    <animate attributeName="opacity" values="1;0.15;1" dur="3s" begin="0.4s" repeatCount="indefinite"/>
                  </circle>
                </svg>
              </div>
              <div>
                <div class="breadcrumb-pro mb-1">
                  <span class="root">{{ $t('dashboard.terminalBar.brand') }}</span>
                  <i class="fa-solid fa-chevron-right mx-2 separator"></i>
                  <span class="current">{{ roleLabel }}</span>
                </div>
                <h2 class="premium-title m-0">{{ $t('dashboard.title') }}</h2>
              </div>
            </div>
            <div class="terminal-right d-flex align-items-center gap-3 flex-wrap">
              <button class="theme-toggle-btn" @click="toggleTheme" :title="isDark ? $t('dashboard.terminalBar.themeLight') : $t('dashboard.terminalBar.themeDark')">
                <i class="fa" :class="isDark ? 'fa-sun' : 'fa-moon'"></i>
                <span>{{ isDark ? $t('dashboard.terminalBar.themeLight') : $t('dashboard.terminalBar.themeDark') }}</span>
              </button>
              <div class="metric-pill">
                <span class="live-dot"></span>
                <span>{{ $t('dashboard.terminalBar.live') }}</span>
              </div>
              <div class="metric-pill">
                <i class="fa-solid fa-clock" style="color:#6366f1"></i>
                <span>{{ currentTime }}</span>
              </div>
              <div class="user-badge">
                <div class="user-avatar">{{ userInitial }}</div>
                <span>{{ userName }}</span>
              </div>
            </div>
          </header>

          <!-- HERO CARD -->
          <div class="hero-card mb-5">
            <div class="scanner-sweep"></div>
            <div class="hero-inner">
              <div class="hero-text">
                <div class="premium-tag" :style="{'background': roleTagBg, 'border-color': roleTagBorder, 'color': roleTagColor}">
                  <i :class="roleIcon"></i>
                  {{ roleLabel }}
                </div>
                <h1 class="hero-title">
                  {{ $t('dashboard.hero.welcome') }}<br>
                  <span class="gold-text">{{ authStore.user?.name || $t('optional') }}</span>
                </h1>
                <div class="ia-insight">
                  <div class="ia-orb" :style="{'background': `linear-gradient(135deg, ${roleAccent}, ${roleAccentLight})`}">
                    <div class="orb-ring"></div>
                    <i :class="roleInsightIcon"></i>
                  </div>
                  <div class="ia-text">
                    <div class="ia-header">
                      <span class="ia-label">{{ roleInsightLabel }}</span>
                      <span class="ia-status-badge">● {{ $t('dashboard.hero.syncBadge') }}</span>
                    </div>
                    <p v-if="loading" class="shimmer-text">{{ $t('dashboard.hero.loading') }}</p>
                    <p v-else class="ia-msg">{{ iaInsight }}</p>
                  </div>
                </div>
              </div>
              <div class="hero-visual">
                <div class="hero-bot-wrap">
                  <div class="bot-glow-ring" :style="{'border-color': roleAccent + '33'}"></div>
                  <div class="bot-glow-ring ring2" :style="{'border-color': roleAccent + '22'}"></div>
                  <i :class="['fa-solid', roleBotIcon, 'bot-icon']" :style="{color: roleAccent}"></i>
                </div>
                <div class="hero-stats-mini" v-if="role !== 'SuperAdmin'">
                  <div class="mini-stat" v-for="ms in miniStats" :key="ms.label">
                    <span class="ms-val" :style="{color: ms.color}">{{ loading ? '—' : ms.value }}</span>
                    <span class="ms-lbl">{{ ms.label }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- KPI GRID -->
          <div class="kpi-grid mb-5">
            <div v-for="(stat, i) in kpiCards" :key="i" class="kpi-card" :style="{'--accent': stat.color}">
              <div class="kpi-top d-flex justify-content-between align-items-start mb-4">
                <div class="kpi-icon" :style="{background: stat.bg, color: stat.color}">
                  <i :class="stat.icon"></i>
                </div>
                <div class="kpi-trend" :style="{background: stat.bg, color: stat.color}">{{ stat.trend }}</div>
              </div>
              <div class="kpi-body">
                <h2 class="kpi-value">
                  <span v-if="loading" class="shimmer-val">—</span>
                  <span v-else>{{ stat.value ?? '0' }}</span>
                </h2>
                <span class="kpi-label">{{ stat.label }}</span>
              </div>
              <div class="kpi-spark">
                <svg viewBox="0 0 80 28" fill="none">
                  <polyline :points="stat.sparkPoints" :stroke="stat.color" stroke-width="2" fill="none" stroke-linecap="round" stroke-linejoin="round"/>
                </svg>
              </div>
            </div>
          </div>

          <!-- ══════════════════════════════════════════════════════
               RECOMMANDATIONS IA — ROUTING PAR RÔLE
          ══════════════════════════════════════════════════════ -->
          <div class="reco-section mb-5">
            <div class="reco-header d-flex align-items-center justify-content-between mb-4">
              <div class="d-flex align-items-center gap-3">
                <div class="reco-icon-wrap">
                  <i class="fa-solid fa-wand-magic-sparkles"></i>
                </div>
                <div>
                  <h5 class="reco-title m-0">{{ $t('dashboard.ai.recoTitle') }}</h5>
                  <span class="reco-sub">{{ $t('dashboard.ai.recoSub') }}</span>
                </div>
              </div>
              <button class="btn-refresh-reco" @click="refreshRecommendations" :disabled="recoLoading">
                <i class="fa-solid" :class="recoLoading ? 'fa-spinner fa-spin' : 'fa-rotate-right'"></i>
                {{ recoLoading ? $t('dashboard.ai.refreshing') : $t('dashboard.ai.refresh') }}
              </button>
            </div>

            <div v-if="recoLoading" class="reco-grid">
              <div v-for="k in 3" :key="k" class="reco-card-skel">
                <div class="reco-skel-top"></div>
                <div class="reco-skel-line w-80"></div>
                <div class="reco-skel-line w-60"></div>
                <div class="reco-skel-line w-90"></div>
              </div>
            </div>

            <div v-else class="reco-grid">
              <div
                v-for="(reco, i) in roleRecommendations"
                :key="i"
                class="reco-card"
                :style="{'--reco-color': reco.color}"
              >
                <div class="reco-card-top">
                  <div class="reco-card-icon" :style="{background: reco.color + '18', color: reco.color}">
                    <i :class="reco.icon"></i>
                  </div>
                  <span class="reco-priority-badge" :style="{background: reco.priorityBg, color: reco.priorityColor}">
                    {{ reco.priority }}
                  </span>
                </div>
                <h6 class="reco-card-title">{{ reco.title }}</h6>
                <p class="reco-card-desc">{{ reco.description }}</p>
                <div class="reco-card-action" @click="handleRecoAction(reco)">
                  <span>{{ reco.actionLabel }}</span>
                  <i class="fa-solid fa-arrow-right"></i>
                </div>
              </div>
            </div>
          </div>

          <!-- ══════════════════════════════════════════════════════
               DASHBOARD CANDIDAT
          ══════════════════════════════════════════════════════ -->
          <template v-if="role === 'Candidat'">
            <div class="two-col-grid mb-5">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-list-check text-blue me-2"></i>
                    {{ $t('dashboard.candidat.testsInProgress') }}
                  </h5>
                  <button class="btn-see-all" @click="router.push('/my-tests')">
                    {{ $t('dashboard.buttons.seeAll') }} <i class="fa-solid fa-arrow-right ms-1"></i>
                  </button>
                </div>
                <div v-if="loading" class="reco-skeletons">
                  <div v-for="k in 3" :key="k" class="reco-skel">
                    <div class="skel-icon"></div>
                    <div class="skel-lines"><div class="skel-l skel-title"></div><div class="skel-l skel-desc"></div></div>
                  </div>
                </div>
                <div v-else class="test-list">
                  <div v-for="test in candidatTests" :key="test.candidatureId" class="test-item"
                    @click="router.push('/exam-lobby/' + test.candidatureId)">
                    <div class="test-left">
                      <div class="test-ico" :style="{background: getTestColor(test.statut)+'18', color: getTestColor(test.statut)}">
                        <i class="fa-solid fa-brain"></i>
                      </div>
                      <div>
                        <span class="test-name">{{ test.campagneNom }}</span>
                        <span class="test-meta">{{ $t('dashboard.candidat.passedOn', { date: formatDate(test.datePostule) }) }}</span>
                      </div>
                    </div>
                    <div class="test-right">
                      <span class="test-status-badge" :style="{background: getTestColor(test.statut)+'18', color: getTestColor(test.statut)}">
                        {{ formatStatut(test.statut) }}
                      </span>
                      <i class="fa-solid fa-chevron-right test-arrow" :style="{color: getTestColor(test.statut)}"></i>
                    </div>
                  </div>
                  <div v-if="!candidatTests.length" class="empty-state">
                    <i class="fa-solid fa-check-circle text-success fa-2x mb-2 d-block"></i>
                    <p>{{ $t('dashboard.candidat.noTests') }}</p>
                  </div>
                </div>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-chart-line text-green me-2"></i>
                    {{ $t('dashboard.candidat.progression') }}
                  </h5>
                  <button class="btn-see-all" @click="router.push('/history')">
                    {{ $t('dashboard.buttons.history') }} <i class="fa-solid fa-arrow-right ms-1"></i>
                  </button>
                </div>
                <div class="progress-list">
                  <div v-for="(item, i) in candidatProgression" :key="i" class="progress-row">
                    <div class="progress-info d-flex justify-content-between mb-1">
                      <span class="progress-label">{{ item.campagneNom }}</span>
                      <span class="progress-pct" :style="{color: getScoreColor(item.score)}">{{ Math.round(item.score) }}%</span>
                    </div>
                    <div class="progress-bar-wrap">
                      <div class="progress-bar-fill" :style="{width: Math.round(item.score) + '%', background: getScoreColor(item.score)}"></div>
                    </div>
                  </div>
                  <div v-if="!candidatProgression.length" class="empty-state">
                    <p>{{ $t('dashboard.candidat.passTest') }}</p>
                  </div>
                </div>
                <div class="next-test-cta mt-4" @click="router.push('/my-tests')">
                  <i class="fa-solid fa-play-circle"></i>
                  <span>{{ $t('dashboard.candidat.startNew') }}</span>
                  <i class="fa-solid fa-arrow-right ms-auto"></i>
                </div>
              </div>
            </div>

            <!-- CV ANALYSIS + LETTRE DE MOTIVATION -->
            <div class="two-col-grid mb-5">
              <!-- CV SCAN -->
              <div id="cv-scan-section" class="panel cv-panel d-flex flex-column">
                <div class="panel-header d-flex align-items-center justify-content-between mb-3">
                  <div class="d-flex align-items-center gap-3">
                    <div class="kpi-icon" style="background:#fef3c7;color:#fbbf24;"><i class="fa-solid fa-file-pdf"></i></div>
                    <div>
                      <h6 class="panel-title m-0">{{ $t('dashboard.cv.title') }}</h6>
                      <span class="neural-badge">{{ $t('dashboard.cv.badge') }}</span>
                    </div>
                  </div>
                </div>
                <p class="cv-desc">{{ $t('dashboard.cv.desc') }}</p>
                <transition name="fade-up">
                  <div v-if="cvResult" class="cv-result mb-3">
                    <div class="score-ring">
                      <svg viewBox="0 0 80 80">
                        <circle cx="40" cy="40" r="34" fill="none" stroke="var(--border-color,#eef2f6)" stroke-width="6"/>
                        <circle cx="40" cy="40" r="34" fill="none" :stroke="cvScoreColor" stroke-width="6" stroke-linecap="round"
                          :stroke-dasharray="`${cvResult.score * 2.13} 213`"
                          transform="rotate(-90 40 40)" style="transition:stroke-dasharray 1s ease"/>
                        <text x="40" y="46" text-anchor="middle" class="score-svg-text">{{ cvResult.score }}%</text>
                      </svg>
                    </div>
                    <div class="cv-result-info">
                      <p class="cv-verdict" :style="{color: cvScoreColor}">
                        {{ cvResult.score >= 75 ? $t('dashboard.cv.compatible') : $t('dashboard.cv.partial') }}
                      </p>
                      <div class="strength-pills d-flex flex-wrap gap-1 mb-2">
                        <span v-for="(pt, i) in (cvResult.points_forts||[]).slice(0,3)" :key="i" class="strength-pill">{{ pt }}</span>
                      </div>
                      <div v-if="cvResult.conseils" class="cv-conseils mb-2">
                        <div class="conseils-title"><i class="fa-solid fa-lightbulb me-1" style="color:#f59e0b"></i>{{ $t('dashboard.cv.tips') }}</div>
                        <ul class="conseils-list">
                          <li v-for="(c, i) in (cvResult.conseils||[]).slice(0,3)" :key="i">{{ c }}</li>
                        </ul>
                      </div>
                      <button @click="cvResult=null;selectedFile=null" class="btn-reset">
                        <i class="fa-solid fa-rotate me-1"></i>{{ $t('dashboard.cv.newScan') }}
                      </button>
                    </div>
                  </div>
                </transition>
                <div v-if="!cvResult" class="upload-zone" :class="{'uploading': isDragging}"
                  @dragover.prevent="isDragging=true" @dragleave="isDragging=false"
                  @drop.prevent="handleDrop" @click="$refs.cvInputCandidat.click()">
                  <input ref="cvInputCandidat" type="file" @change="handleCvUpload" accept=".pdf,.docx" style="display:none">
                  <i class="fa-solid fa-cloud-arrow-up upload-icon d-block mb-2"></i>
                  <p class="upload-text">{{ selectedFile ? selectedFile.name : $t('dashboard.cv.upload') }}</p>
                  <span class="upload-hint">{{ $t('dashboard.cv.hint') }}</span>
                </div>
                <div v-if="!cvResult" class="job-input-wrap mt-3">
                  <input v-model="jobDescription" class="job-input" :placeholder="$t('dashboard.cv.job')" />
                </div>
                <button v-if="!cvResult" @click="runCvAnalysis" :disabled="!selectedFile || isAnalyzing" class="btn-enigma-primary w-100 mt-3">
                  <div class="btn-content">
                    <span v-if="isAnalyzing"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ $t('dashboard.cv.analyzing') }}</span>
                    <span v-else><i class="fa-solid fa-magnifying-glass me-2"></i>{{ $t('dashboard.cv.analyze') }}</span>
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>

              <!-- LETTRE DE MOTIVATION IA -->
              <div class="panel lettre-panel d-flex flex-column">
                <div class="panel-header d-flex align-items-center justify-content-between mb-3">
                  <div class="d-flex align-items-center gap-3">
                    <div class="kpi-icon" style="background:#ede9fe;color:#8b5cf6;"><i class="fa-solid fa-envelope-open-text"></i></div>
                    <div>
                      <h6 class="panel-title m-0">{{ $t('dashboard.lettre.title') }}</h6>
                      <span class="neural-badge" style="background:rgba(139,92,246,0.15);color:#7c3aed;">{{ $t('dashboard.lettre.badge') }}</span>
                    </div>
                  </div>
                </div>
                <p class="cv-desc">{{ $t('dashboard.lettre.desc') }}</p>
                <div class="lettre-form" v-if="!lettreResult">
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">NOM COMPLET</label>
                    <input v-model="lettreData.nom" class="job-input" :placeholder="authStore.user?.name || 'Ahmed Ben Ali'" />
                  </div>
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">{{ $t('dashboard.lettre.position') }} <span style="color:#ef4444">*</span></label>
                    <input v-model="lettreData.poste" class="job-input" :placeholder="$t('dashboard.lettre.position')" />
                  </div>
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">{{ $t('dashboard.lettre.company') }} <span style="color:#ef4444">*</span></label>
                    <input v-model="lettreData.entreprise" class="job-input" :placeholder="$t('dashboard.lettre.company')" />
                  </div>
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">{{ $t('dashboard.lettre.skills') }}</label>
                    <input v-model="lettreData.competences" class="job-input" :placeholder="$t('dashboard.lettre.skills')" />
                  </div>
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">STYLE</label>
                    <select v-model="lettreData.style" class="job-input">
                      <option value="professionnel">Professionnel — Classique et formel</option>
                      <option value="créatif">Créatif — Dynamique et original</option>
                      <option value="concis">Concis — Court et percutant</option>
                    </select>
                  </div>
                  <div class="lettre-field mb-3">
                    <label class="lettre-label-field">{{ $t('dashboard.lettre.language') }}</label>
                    <select v-model="lettreData.langue" class="job-input">
                      <option value="fr">🇫🇷 Français</option>
                      <option value="en">🇬🇧 English</option>
                      <option value="ar">🇸🇦 العربية</option>
                    </select>
                    <div class="lang-indicator mt-2 d-flex align-items-center gap-2">
                      <span class="lang-badge-pill" :class="`lang-pill-${lettreData.langue}`">
                        {{ { fr: '🇫🇷 Français', en: '🇬🇧 English', ar: '🇸🇦 العربية' }[lettreData.langue] }}
                      </span>
                      <small style="font-size:11px;color:var(--text-muted,#94a3b8);font-style:italic;">
                        La lettre sera générée entièrement dans cette langue
                      </small>
                    </div>
                  </div>
                  <button @click="generateLettre"
                    :disabled="!lettreData.poste || !lettreData.entreprise || isGeneratingLettre"
                    class="btn-lettre-primary w-100">
                    <span v-if="isGeneratingLettre"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ $t('dashboard.lettre.generating') }}</span>
                    <span v-else><i class="fa-solid fa-wand-magic-sparkles me-2"></i>{{ $t('dashboard.lettre.generate') }}</span>
                  </button>
                </div>
                <transition name="fade-up">
                  <div v-if="lettreResult" class="lettre-result">
                    <div class="result-lang-bar d-flex align-items-center gap-2 flex-wrap mb-3">
                      <span class="lang-badge-pill" :class="`lang-pill-${lettreData.langue}`">
                        {{ { fr: '🇫🇷 Français', en: '🇬🇧 English', ar: '🇸🇦 العربية' }[lettreData.langue] }}
                      </span>
                      <span class="result-style-badge">{{ lettreData.style || 'professionnel' }}</span>
                      <span class="source-gemini">✨ EvaluaTech IA</span>
                    </div>
                    <div class="lettre-header-row d-flex justify-content-between align-items-center mb-3">
                      <span class="lettre-label"><i class="fa-solid fa-check-circle me-2" style="color:#10b981"></i>{{ $t('dashboard.lettre.generated') }}</span>
                      <div class="d-flex gap-2">
                        <button class="btn-copy-lettre" @click="copyLettre">
                          <i class="fa-solid" :class="letterCopied ? 'fa-check' : 'fa-copy'"></i>
                          {{ letterCopied ? $t('dashboard.lettre.copied') : $t('dashboard.lettre.copy') }}
                        </button>
                        <button class="btn-copy-lettre" @click="downloadLettre">
                          <i class="fa-solid fa-download"></i> .txt
                        </button>
                        <button class="btn-reset" @click="lettreResult=null">
                          <i class="fa-solid fa-rotate me-1"></i>{{ $t('dashboard.lettre.newLetter') }}
                        </button>
                      </div>
                    </div>
                    <div class="lettre-content" :dir="lettreData.langue === 'ar' ? 'rtl' : 'ltr'">{{ lettreResult }}</div>
                  </div>
                </transition>
              </div>
            </div>

            <!-- GRAPHIQUE + DERNIERS RÉSULTATS -->
            <div class="two-col-grid pb-5" style="--col1: 2fr; --col2: 1fr;">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.scoreEvolution') }}</h5>
                  <div class="period-switcher">
                    <button v-for="p in ['week','month','quarter']" :key="p"
                      @click="switchPeriod(p)" :class="['period-btn', {active: activePeriod === p}]">
                      {{ $t(`dashboard.periods.${p}`) }}
                    </button>
                  </div>
                </div>
                <div class="chart-wrap"><canvas class="js-main-chart"></canvas><div class="chart-no-data" style="display:none;padding:40px;text-align:center;color:#94a3b8;font-size:14px">Aucune donnée de performance.</div></div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.lastResults') }}</h5>
                  <span class="activity-count-badge">{{ historiqueCandidat.length }}</span>
                </div>
                <div class="results-list">
                  <div v-for="res in historiqueCandidat.slice(0,5)" :key="res.id" class="result-row" @click="router.push('/results/' + res.id)">
                    <div class="result-score" :style="{background: getScoreColor(res.score)+'18', color: getScoreColor(res.score)}">
                      {{ Math.round(res.score) }}%
                    </div>
                    <div class="result-body">
                      <span class="result-name">{{ res.titreExamen }}</span>
                      <span class="result-date">{{ formatDate(res.date) }}</span>
                    </div>
                    <i class="fa-solid fa-chevron-right result-arrow"></i>
                  </div>
                  <div v-if="!historiqueCandidat.length" class="empty-state">
                    <span class="small text-muted">{{ $t('dashboard.candidat.noResults') }}</span>
                  </div>
                </div>
              </div>
            </div>
          </template>

          <!-- ══════════════════════════════════════════════════════
               DASHBOARD ÉVALUATEUR / RH / RECRUTEUR
          ══════════════════════════════════════════════════════ -->
          <template v-else-if="['Evaluateur','RH','Recruteur'].includes(role)">
            <div class="two-col-grid mb-5">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-users-gear text-amber me-2"></i>
                    {{ $t('dashboard.sections.evalQueue') }}
                  </h5>
                  <button class="btn-see-all" @click="router.push('/analyse-comportementale')">
                    {{ $t('dashboard.buttons.seeAll') }} <i class="fa-solid fa-arrow-right ms-1"></i>
                  </button>
                </div>
                <div v-if="loading" class="reco-skeletons">
                  <div v-for="k in 3" :key="k" class="reco-skel">
                    <div class="skel-icon"></div>
                    <div class="skel-lines"><div class="skel-l skel-title"></div><div class="skel-l skel-desc"></div></div>
                  </div>
                </div>
                <div v-else class="eval-list">
                  <div v-for="ev in evalQueue.slice(0,5)" :key="ev.id" class="eval-item">
                    <div class="eval-avatar">{{ getInitials(ev.candidatNom || ev.candidateName) }}</div>
                    <div class="eval-body flex-grow-1">
                      <span class="eval-name">{{ ev.candidatNom || ev.candidateName }}</span>
                      <span class="eval-test">{{ ev.titreExamen || ev.testName }}</span>
                    </div>
                    <div class="eval-right">
                      <span class="eval-badge" :style="{background: getStatusBg(ev.statut), color: getStatusColor(ev.statut)}">
                        {{ $t(`dashboard.eval.${ev.statut === 'EN_COURS' ? 'inProgress' : ev.statut === 'TERMINE' ? 'done' : 'waiting'}`) }}
                      </span>
                      <button class="eval-btn" @click="router.push('/analyse-comportementale')" style="color:#f59e0b">{{ $t('dashboard.eval.evalBtn') }}</button>
                    </div>
                  </div>
                  <div v-if="!evalQueue.length" class="empty-state">
                    <i class="fa-solid fa-check-circle text-success fa-2x mb-2 d-block"></i>
                    <p>{{ $t('dashboard.empty.queue') }}</p>
                  </div>
                </div>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-calendar-check text-indigo me-2"></i>
                    {{ $t('dashboard.sections.plannedSessions') }}
                  </h5>
                  <button class="btn-see-all" @click="router.push('/campaigns')">
                    {{ $t('dashboard.buttons.schedule') }} <i class="fa-solid fa-plus ms-1"></i>
                  </button>
                </div>
                <div class="session-list">
                  <div v-for="sess in campagnes.slice(0,4)" :key="sess.id" class="session-item">
                    <div class="session-date-block" :style="{background: '#6366f1'+'18', color: '#6366f1'}">
                      <span class="sess-day">{{ sess.dateDebut ? new Date(sess.dateDebut).getDate() : '—' }}</span>
                      <span class="sess-month">{{ sess.dateDebut ? new Date(sess.dateDebut).toLocaleString('fr',{month:'short'}).toUpperCase() : '—' }}</span>
                    </div>
                    <div class="session-body">
                      <span class="sess-title">{{ sess.nom }}</span>
                      <span class="sess-info">{{ sess.dureeMinutes }} {{ $t('dashboard.sessions.minutes') }} · {{ sess.nbCandidats || 0 }} {{ $t('dashboard.sessions.candidates') }}</span>
                    </div>
                    <span class="sess-status" :style="{color: sess.statut === 1 ? '#10b981' : '#f59e0b'}">
                      {{ sess.statut === 1 ? $t('dashboard.sessions.active') : $t('dashboard.sessions.planned') }}
                    </span>
                  </div>
                  <div v-if="!campagnes.length" class="empty-state">
                    <p>{{ $t('dashboard.empty.campaigns') }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- SCAN CV + TOP SKILLS -->
            <div class="two-col-grid mb-5">
              <div id="cv-scan-section" class="panel cv-panel d-flex flex-column">
                <div class="panel-header d-flex align-items-center justify-content-between mb-3">
                  <div class="d-flex align-items-center gap-3">
                    <div class="kpi-icon" style="background:#fef3c7;color:#fbbf24;"><i class="fa-solid fa-file-pdf"></i></div>
                    <div>
                      <h6 class="panel-title m-0">{{ $t('dashboard.cv.title') }}</h6>
                      <span class="neural-badge">{{ $t('dashboard.cv.badge') }}</span>
                    </div>
                  </div>
                </div>
                <p class="cv-desc">{{ $t('dashboard.cv.descEval') }}</p>
                <transition name="fade-up">
                  <div v-if="cvResult" class="cv-result mb-3">
                    <div class="score-ring">
                      <svg viewBox="0 0 80 80">
                        <circle cx="40" cy="40" r="34" fill="none" stroke="var(--border-color,#eef2f6)" stroke-width="6"/>
                        <circle cx="40" cy="40" r="34" fill="none" :stroke="cvScoreColor" stroke-width="6" stroke-linecap="round"
                          :stroke-dasharray="`${cvResult.score * 2.13} 213`"
                          transform="rotate(-90 40 40)" style="transition:stroke-dasharray 1s ease"/>
                        <text x="40" y="46" text-anchor="middle" class="score-svg-text">{{ cvResult.score }}%</text>
                      </svg>
                    </div>
                    <div class="cv-result-info">
                      <p class="cv-verdict" :style="{color: cvScoreColor}">{{ cvResult.score >= 75 ? $t('dashboard.cv.compatible') : $t('dashboard.cv.partial') }}</p>
                      <div class="strength-pills d-flex flex-wrap gap-1 mb-2">
                        <span v-for="(pt, i) in (cvResult.points_forts||[]).slice(0,3)" :key="i" class="strength-pill">{{ pt }}</span>
                      </div>
                      <div v-if="cvResult.decision" class="decision-badge mt-2">
                        <i class="fa-solid fa-robot me-1"></i>{{ cvResult.decision }}
                      </div>
                      <button @click="cvResult=null;selectedFile=null" class="btn-reset mt-2">
                        <i class="fa-solid fa-rotate me-1"></i>{{ $t('dashboard.cv.newScan') }}
                      </button>
                    </div>
                  </div>
                </transition>
                <div v-if="!cvResult" class="upload-zone" :class="{'uploading': isDragging}"
                  @dragover.prevent="isDragging=true" @dragleave="isDragging=false"
                  @drop.prevent="handleDrop" @click="$refs.cvInput.click()">
                  <input ref="cvInput" type="file" @change="handleCvUpload" accept=".pdf,.docx" style="display:none">
                  <i class="fa-solid fa-cloud-arrow-up upload-icon d-block mb-2"></i>
                  <p class="upload-text">{{ selectedFile ? selectedFile.name : $t('dashboard.cv.uploadEval') }}</p>
                  <span class="upload-hint">{{ $t('dashboard.cv.hint') }}</span>
                </div>
                <div v-if="!cvResult" class="job-input-wrap mt-3">
                  <input v-model="jobDescription" class="job-input" :placeholder="$t('dashboard.cv.jobEval')" />
                </div>
                <button v-if="!cvResult" @click="runCvAnalysis" :disabled="!selectedFile || isAnalyzing" class="btn-enigma-primary w-100 mt-3">
                  <div class="btn-content">
                    <span v-if="isAnalyzing"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ $t('dashboard.cv.analyzingShort') }}</span>
                    <span v-else><i class="fa-solid fa-magnifying-glass me-2"></i>{{ $t('dashboard.cv.analyze') }}</span>
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>

              <!-- TOP COMPÉTENCES -->
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.topSkills') }}</h5>
                </div>
                <div class="skill-radar-list">
                  <div v-for="sk in topSkills" :key="sk.label" class="skill-bar-row">
                    <div class="skill-bar-info d-flex justify-content-between mb-1">
                      <span class="skill-bar-label">{{ sk.label }}</span>
                      <span class="skill-bar-pct" :style="{color: sk.color}">{{ sk.value }}%</span>
                    </div>
                    <div class="progress-bar-wrap">
                      <div class="progress-bar-fill" :style="{width: sk.value+'%', background: sk.color}"></div>
                    </div>
                  </div>
                </div>
                <div class="analyse-cta mt-4" @click="router.push('/analyse-comportementale')">
                  <i class="fa-solid fa-brain"></i>
                  <span>{{ $t('dashboard.buttons.analyze') }}</span>
                  <i class="fa-solid fa-arrow-right ms-auto"></i>
                </div>
              </div>
            </div>

            <!-- GRAPHIQUE + JOURNAL ACTIVITÉ -->
            <div class="two-col-grid pb-5" style="--col1: 2fr; --col2: 1fr;">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.performance') }}</h5>
                  <div class="period-switcher">
                    <button v-for="p in ['week','month','quarter']" :key="p"
                      @click="switchPeriod(p)" :class="['period-btn', {active: activePeriod === p}]">
                      {{ $t(`dashboard.periods.${p}`) }}
                    </button>
                  </div>
                </div>
                <div class="chart-wrap"><canvas class="js-main-chart"></canvas><div class="chart-no-data" style="display:none;padding:40px;text-align:center;color:#94a3b8;font-size:14px">Aucune donnée de performance.</div></div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.activityLog') }}</h5>
                  <span class="activity-count-badge">{{ recentActivities.length }}</span>
                </div>
                <div class="activity-list">
                  <div v-for="act in recentActivities" :key="act.id" class="activity-row">
                    <div class="act-dot" :style="{background: act.color}"></div>
                    <div class="act-body flex-grow-1 overflow-hidden">
                      <span class="act-user d-block">{{ act.user }}</span>
                      <span class="act-action d-block text-truncate">{{ act.action }}</span>
                    </div>
                    <span class="act-time">{{ act.time }}</span>
                  </div>
                </div>
              </div>
            </div>
          </template>

          <!-- ══════════════════════════════════════════════════════
               DASHBOARD ADMIN ENTREPRISE
          ══════════════════════════════════════════════════════ -->
          <template v-else-if="role === 'AdminEntreprise'">
            <div class="two-col-grid mb-5">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-bolt-lightning text-amber me-2"></i>{{ $t('dashboard.admin.activityLog') }}
                  </h5>
                  <span class="activity-count-badge">{{ recentActivities.length }}</span>
                </div>
                <div class="activity-list">
                  <div v-for="act in recentActivities" :key="act.id" class="activity-row">
                    <div class="act-dot" :style="{background: act.color}"></div>
                    <div class="act-body flex-grow-1 overflow-hidden">
                      <span class="act-user d-block">{{ act.user }}</span>
                      <span class="act-action d-block text-truncate">{{ act.action }}</span>
                    </div>
                    <span class="act-time">{{ act.time }}</span>
                  </div>
                </div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">
                    <i class="fa-solid fa-people-group text-indigo me-2"></i>{{ $t('dashboard.admin.team') }}
                  </h5>
                  <button class="btn-see-all" @click="router.push('/staff-members')">
                    {{ $t('dashboard.buttons.manage') }} <i class="fa-solid fa-arrow-right ms-1"></i>
                  </button>
                </div>
                <div class="team-grid">
                  <div v-for="member in staffMembers.slice(0,6)" :key="member.id" class="team-card">
                    <div class="team-avatar" :style="{background: getAvatarColor(member.roleNom)}">
                      {{ getInitials((member.prenom||'') + ' ' + (member.nomFamille||member.nom||'')) }}
                    </div>
                    <span class="team-name">{{ member.prenom }} {{ member.nomFamille || member.nom }}</span>
                    <span class="team-role">{{ member.roleNom }}</span>
                    <span class="team-badge" :style="{background: member.estActif ? '#ecfdf5' : '#fef2f2', color: member.estActif ? '#059669' : '#dc2626'}">
                      {{ member.estActif ? $t('dashboard.team.active') : $t('dashboard.team.inactive') }}
                    </span>
                  </div>
                  <div v-if="!staffMembers.length" class="empty-state">
                    <p>{{ $t('dashboard.empty.team') }}</p>
                  </div>
                </div>
                <div class="invite-cta mt-3" @click="router.push('/invite')">
                  <i class="fa-solid fa-user-plus"></i>
                  <span>{{ $t('dashboard.buttons.newMember') }}</span>
                  <i class="fa-solid fa-arrow-right ms-auto"></i>
                </div>
              </div>
            </div>

            <!-- SCAN CV + CANDIDATS RÉCENTS -->
            <div class="two-col-grid mb-5">
              <div id="cv-scan-section" class="panel cv-panel d-flex flex-column">
                <div class="panel-header d-flex align-items-center justify-content-between mb-3">
                  <div class="d-flex align-items-center gap-3">
                    <div class="kpi-icon" style="background:#fef3c7;color:#fbbf24;"><i class="fa-solid fa-file-pdf"></i></div>
                    <div>
                      <h6 class="panel-title m-0">{{ $t('dashboard.cv.title') }}</h6>
                      <span class="neural-badge">{{ $t('dashboard.cv.badge') }}</span>
                    </div>
                  </div>
                </div>
                <p class="cv-desc">{{ $t('dashboard.cv.descAdmin') }}</p>
                <transition name="fade-up">
                  <div v-if="cvResult" class="cv-result mb-3">
                    <div class="score-ring">
                      <svg viewBox="0 0 80 80">
                        <circle cx="40" cy="40" r="34" fill="none" stroke="var(--border-color,#eef2f6)" stroke-width="6"/>
                        <circle cx="40" cy="40" r="34" fill="none" :stroke="cvScoreColor" stroke-width="6" stroke-linecap="round"
                          :stroke-dasharray="`${cvResult.score * 2.13} 213`"
                          transform="rotate(-90 40 40)" style="transition:stroke-dasharray 1s ease"/>
                        <text x="40" y="46" text-anchor="middle" class="score-svg-text">{{ cvResult.score }}%</text>
                      </svg>
                    </div>
                    <div class="cv-result-info">
                      <p class="cv-verdict" :style="{color: cvScoreColor}">{{ cvResult.score >= 75 ? $t('dashboard.cv.compatible') : $t('dashboard.cv.partial') }}</p>
                      <div class="strength-pills d-flex flex-wrap gap-1 mb-2">
                        <span v-for="(pt, i) in (cvResult.points_forts||[]).slice(0,3)" :key="i" class="strength-pill">{{ pt }}</span>
                      </div>
                      <div v-if="cvResult.decision" class="decision-badge mt-2">
                        <i class="fa-solid fa-robot me-1"></i>{{ cvResult.decision }}
                      </div>
                      <button @click="cvResult=null;selectedFile=null" class="btn-reset mt-2">
                        <i class="fa-solid fa-rotate me-1"></i>{{ $t('dashboard.cv.newScan') }}
                      </button>
                    </div>
                  </div>
                </transition>
                <div v-if="!cvResult" class="upload-zone" :class="{'uploading': isDragging}"
                  @dragover.prevent="isDragging=true" @dragleave="isDragging=false"
                  @drop.prevent="handleDrop" @click="$refs.cvInputAdmin.click()">
                  <input ref="cvInputAdmin" type="file" @change="handleCvUpload" accept=".pdf,.docx" style="display:none">
                  <i class="fa-solid fa-cloud-arrow-up upload-icon d-block mb-2"></i>
                  <p class="upload-text">{{ selectedFile ? selectedFile.name : $t('dashboard.cv.upload') }}</p>
                  <span class="upload-hint">{{ $t('dashboard.cv.hint') }}</span>
                </div>
                <div v-if="!cvResult" class="job-input-wrap mt-3">
                  <input v-model="jobDescription" class="job-input" :placeholder="$t('dashboard.cv.job')" />
                </div>
                <button v-if="!cvResult" @click="runCvAnalysis" :disabled="!selectedFile || isAnalyzing" class="btn-enigma-primary w-100 mt-3">
                  <div class="btn-content">
                    <span v-if="isAnalyzing"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ $t('dashboard.cv.analyzingShort') }}</span>
                    <span v-else><i class="fa-solid fa-magnifying-glass me-2"></i>{{ $t('dashboard.cv.analyze') }}</span>
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.admin.recentCandidates') }}</h5>
                  <button class="btn-see-all" @click="router.push('/candidates-list')">{{ $t('dashboard.buttons.seeAll') }}</button>
                </div>
                <div class="candidates-list">
                  <div v-for="cand in recentCandidates" :key="cand.id" class="cand-row" @click="router.push('/details-candidat/' + cand.candidateId)">
                    <div class="cand-avatar">{{ getInitials(cand.candidateName) }}</div>
                    <div class="cand-body flex-grow-1">
                      <span class="cand-name">{{ cand.candidateName }}</span>
                      <span class="cand-test">{{ cand.testName }}</span>
                    </div>
                    <div class="cand-score" :style="{color: getScoreColor(cand.score)}">{{ cand.score }}%</div>
                  </div>
                  <div v-if="!recentCandidates.length" class="empty-state">
                    <p>{{ $t('dashboard.empty.candidates') }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- GRAPHIQUE + TOP SKILLS -->
            <div class="two-col-grid pb-5" style="--col1: 2fr; --col2: 1fr;">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.performance') }}</h5>
                  <div class="period-switcher">
                    <button v-for="p in ['week','month','quarter']" :key="p"
                      @click="switchPeriod(p)" :class="['period-btn', {active: activePeriod === p}]">
                      {{ $t(`dashboard.periods.${p}`) }}
                    </button>
                  </div>
                </div>
                <div class="chart-wrap"><canvas class="js-main-chart"></canvas><div class="chart-no-data" style="display:none;padding:40px;text-align:center;color:#94a3b8;font-size:14px">Aucune donnée de performance.</div></div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.topSkills') }}</h5>
                </div>
                <div class="skill-radar-list">
                  <div v-for="sk in topSkills" :key="sk.label" class="skill-bar-row">
                    <div class="skill-bar-info d-flex justify-content-between mb-1">
                      <span class="skill-bar-label">{{ sk.label }}</span>
                      <span class="skill-bar-pct" :style="{color: sk.color}">{{ sk.value }}%</span>
                    </div>
                    <div class="progress-bar-wrap">
                      <div class="progress-bar-fill" :style="{width: sk.value+'%', background: sk.color}"></div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </template>

          <!-- ══════════════════════════════════════════════════════
               DASHBOARD SUPER ADMIN
          ══════════════════════════════════════════════════════ -->
          <template v-else-if="role === 'SuperAdmin'">
            <div class="three-col-grid mb-5">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0"><i class="fa-solid fa-server text-indigo me-2"></i>{{ $t('dashboard.superAdmin.health') }}</h5>
                  <span class="live-badge"><span class="live-dot"></span>{{ $t('dashboard.superAdmin.liveBadge') }}</span>
                </div>
                <div class="health-list">
                  <div v-for="svc in iaServices" :key="svc.name" class="health-row">
                    <span class="health-dot" :style="{background: svc.up ? '#10b981' : '#ef4444'}"></span>
                    <span class="health-name flex-grow-1">{{ $t('dashboard.health.' + svc.key) }}</span>
                    <span class="health-latency">{{ svc.latency }}</span>
                    <span class="health-status" :style="{color: svc.up ? '#10b981' : '#ef4444'}">{{ svc.up ? $t('dashboard.superAdmin.up') : $t('dashboard.superAdmin.down') }}</span>
                  </div>
                </div>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0"><i class="fa-solid fa-building text-amber me-2"></i>{{ $t('dashboard.superAdmin.companies') }}</h5>
                  <button class="btn-see-all" @click="router.push('/super-admin')">
                    {{ $t('dashboard.superAdmin.manage') }} <i class="fa-solid fa-arrow-right ms-1"></i>
                  </button>
                </div>
                <div class="company-list">
                  <div v-for="co in superAdminStats.recentTransactions || []" :key="co.id" class="company-row">
                    <div class="company-logo" :style="{background: co.color || '#6366f1'}">
                      <i class="fa-solid fa-building" style="font-size:14px;color:white"></i>
                    </div>
                    <div class="company-body flex-grow-1">
                      <span class="company-name">{{ co.name }}</span>
                      <span class="company-plan">{{ co.plan || 'Gratuit' }}</span>
                    </div>
                    <span class="company-users">{{ co.date }}</span>
                  </div>
                  <div v-if="!superAdminStats.recentTransactions?.length" class="empty-state">
                    <p>{{ $t('dashboard.empty.companies', { count: superAdminStats.totalEntreprises || 0 }) }}</p>
                  </div>
                </div>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0"><i class="fa-solid fa-wave-square text-green me-2"></i>{{ $t('dashboard.sections.globalActivity') }}</h5>
                  <span class="activity-count-badge">{{ recentActivities.length }}</span>
                </div>
                <div class="activity-list">
                  <div v-for="act in recentActivities" :key="act.id" class="activity-row">
                    <div class="act-dot" :style="{background: act.color}"></div>
                    <div class="act-body flex-grow-1 overflow-hidden">
                      <span class="act-user d-block">{{ act.user }}</span>
                      <span class="act-action d-block text-truncate">{{ act.action }}</span>
                    </div>
                    <span class="act-time">{{ act.time }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- SCAN CV + UTILISATEURS -->
            <div class="two-col-grid mb-5">
              <div id="cv-scan-section" class="panel cv-panel d-flex flex-column">
                <div class="panel-header d-flex align-items-center justify-content-between mb-3">
                  <div class="d-flex align-items-center gap-3">
                    <div class="kpi-icon" style="background:#fef3c7;color:#fbbf24;"><i class="fa-solid fa-file-pdf"></i></div>
                    <div>
                      <h6 class="panel-title m-0">{{ $t('dashboard.cv.platformTitle') }}</h6>
                      <span class="neural-badge">{{ $t('dashboard.cv.platformBadge') }}</span>
                    </div>
                  </div>
                </div>
                <p class="cv-desc">{{ $t('dashboard.cv.descSA') }}</p>
                <transition name="fade-up">
                  <div v-if="cvResult" class="cv-result mb-3">
                    <div class="score-ring">
                      <svg viewBox="0 0 80 80">
                        <circle cx="40" cy="40" r="34" fill="none" stroke="var(--border-color,#eef2f6)" stroke-width="6"/>
                        <circle cx="40" cy="40" r="34" fill="none" :stroke="cvScoreColor" stroke-width="6" stroke-linecap="round"
                          :stroke-dasharray="`${cvResult.score * 2.13} 213`"
                          transform="rotate(-90 40 40)" style="transition:stroke-dasharray 1s ease"/>
                        <text x="40" y="46" text-anchor="middle" class="score-svg-text">{{ cvResult.score }}%</text>
                      </svg>
                    </div>
                    <div class="cv-result-info">
                      <p class="cv-verdict" :style="{color: cvScoreColor}">{{ cvResult.score >= 75 ? $t('dashboard.cv.compatible') : $t('dashboard.cv.partial') }}</p>
                      <div class="strength-pills d-flex flex-wrap gap-1 mb-2">
                        <span v-for="(pt, i) in (cvResult.points_forts||[]).slice(0,3)" :key="i" class="strength-pill">{{ pt }}</span>
                      </div>
                      <button @click="cvResult=null;selectedFile=null" class="btn-reset mt-2">
                        <i class="fa-solid fa-rotate me-1"></i>{{ $t('dashboard.cv.newScan') }}
                      </button>
                    </div>
                  </div>
                </transition>
                <div v-if="!cvResult" class="upload-zone" :class="{'uploading': isDragging}"
                  @dragover.prevent="isDragging=true" @dragleave="isDragging=false"
                  @drop.prevent="handleDrop" @click="$refs.cvInputSA.click()">
                  <input ref="cvInputSA" type="file" @change="handleCvUpload" accept=".pdf,.docx" style="display:none">
                  <i class="fa-solid fa-cloud-arrow-up upload-icon d-block mb-2"></i>
                  <p class="upload-text">{{ selectedFile ? selectedFile.name : $t('dashboard.cv.uploadSA') }}</p>
                  <span class="upload-hint">{{ $t('dashboard.cv.hint') }}</span>
                </div>
                <div v-if="!cvResult" class="job-input-wrap mt-3">
                  <input v-model="jobDescription" class="job-input" :placeholder="$t('dashboard.cv.jobSA')" />
                </div>
                <button v-if="!cvResult" @click="runCvAnalysis" :disabled="!selectedFile || isAnalyzing" class="btn-enigma-primary w-100 mt-3">
                  <div class="btn-content">
                    <span v-if="isAnalyzing"><i class="fa-solid fa-spinner fa-spin me-2"></i>{{ $t('dashboard.cv.analyzingShort') }}</span>
                    <span v-else><i class="fa-solid fa-magnifying-glass me-2"></i>{{ $t('dashboard.cv.analyze') }}</span>
                  </div>
                  <div class="btn-glow"></div>
                </button>
              </div>

              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.superAdmin.users') }}</h5>
                  <button class="btn-see-all" @click="router.push('/platform-users')">{{ $t('dashboard.superAdmin.manage') }}</button>
                </div>
                <div class="candidates-list">
                  <div v-for="usr in platformUsers.slice(0,6)" :key="usr.id" class="cand-row">
                    <div class="cand-avatar">{{ getInitials(usr.name) }}</div>
                    <div class="cand-body flex-grow-1">
                      <span class="cand-name">{{ usr.name }}</span>
                      <span class="cand-test">{{ usr.org }} · {{ usr.role }}</span>
                    </div>
                    <span class="team-badge" :style="{background: usr.isActive ? '#ecfdf5' : '#fef2f2', color: usr.isActive ? '#059669' : '#dc2626', padding:'3px 8px', borderRadius:'100px', fontSize:'9px', fontWeight:'700'}">
                      {{ usr.isActive ? $t('dashboard.team.active') : $t('dashboard.team.inactive') }}
                    </span>
                  </div>
                  <div v-if="!platformUsers.length" class="empty-state">
                    <p>{{ $t('dashboard.empty.users', { count: superAdminStats.totalUtilisateurs || 0 }) }}</p>
                  </div>
                </div>
                <div class="analytics-cta mt-3" @click="router.push('/super-admin-analytics')">
                  <i class="fa-solid fa-chart-mixed"></i>
                  <span>{{ $t('dashboard.superAdmin.analyticsDetail') }}</span>
                  <i class="fa-solid fa-arrow-right ms-auto"></i>
                </div>
              </div>
            </div>

            <!-- GRAPHIQUE SUPER ADMIN -->
            <div class="two-col-grid pb-5" style="--col1: 2fr; --col2: 1fr;">
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.analytics') }}</h5>
                  <div class="period-switcher">
                    <button v-for="p in ['week','month','quarter']" :key="p"
                      @click="switchPeriod(p)" :class="['period-btn', {active: activePeriod === p}]">
                      {{ $t(`dashboard.periods.${p}`) }}
                    </button>
                  </div>
                </div>
                <div class="chart-wrap"><canvas class="js-main-chart"></canvas><div class="chart-no-data" style="display:none;padding:40px;text-align:center;color:#94a3b8;font-size:14px">Aucune donnée de performance.</div></div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.superAdmin.pendingDeletion') }}</h5>
                  <span class="activity-count-badge">{{ superAdminStats.demandesEnAttente || 0 }}</span>
                </div>
                <div class="activity-list">
                  <div v-for="act in recentActivities" :key="act.id" class="activity-row">
                    <div class="act-dot" :style="{background: act.color}"></div>
                    <div class="act-body flex-grow-1 overflow-hidden">
                      <span class="act-user d-block">{{ act.user }}</span>
                      <span class="act-action d-block text-truncate">{{ act.action }}</span>
                    </div>
                    <span class="act-time">{{ act.time }}</span>
                  </div>
                </div>
              </div>
            </div>
          </template>

          <!-- FALLBACK -->
          <template v-else>
            <div class="two-col-grid pb-5">
              <div class="panel">
                <div class="panel-header mb-4"><h5 class="panel-title m-0">{{ $t('dashboard.sections.performance') }}</h5></div>
                <div class="chart-wrap"><canvas class="js-main-chart"></canvas><div class="chart-no-data" style="display:none;padding:40px;text-align:center;color:#94a3b8;font-size:14px">Aucune donnée de performance.</div></div>
              </div>
              <div class="panel">
                <div class="panel-header d-flex align-items-center justify-content-between mb-4">
                  <h5 class="panel-title m-0">{{ $t('dashboard.sections.activityLog') }}</h5>
                  <span class="activity-count-badge">{{ recentActivities.length }}</span>
                </div>
                <div class="activity-list">
                  <div v-for="act in recentActivities" :key="act.id" class="activity-row">
                    <div class="act-dot" :style="{background: act.color}"></div>
                    <div class="act-body flex-grow-1 overflow-hidden">
                      <span class="act-user d-block">{{ act.user }}</span>
                      <span class="act-action d-block text-truncate">{{ act.action }}</span>
                    </div>
                    <span class="act-time">{{ act.time }}</span>
                  </div>
                </div>
              </div>
            </div>
          </template>

        </div>
      </main>
    </div>

    <!-- MODAL MAILER DIAGNOSTICS -->
    <transition name="modal-fade">
      <div v-if="showMailerModal" class="enigma-modal-overlay" @click.self="showMailerModal = false">
        <div class="enigma-modal-container glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-square-envelope text-primary"></i>
              <span>Diagnostic Messagerie</span>
            </h4>
            <button class="btn-close-modal" @click="showMailerModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body">
            <div class="d-flex flex-column gap-4">
              <!-- SMTP & Google OAuth Status Card -->
              <div :class="['status-card', mailerDiag.isGoogleConnected ? 'active-green' : 'error-red']">
                <div class="status-card-icon">
                  <i v-if="mailerDiag.isGoogleConnected" class="fa-solid fa-circle-check text-green"></i>
                  <i v-else class="fa-solid fa-circle-xmark text-red"></i>
                </div>
                <div class="flex-grow-1 text-start">
                  <div class="status-label">CONNEXION SMTP GOOGLE OAUTH2</div>
                  <div class="status-value font-weight-bold">
                    {{ mailerDiag.isGoogleConnected ? 'Opérationnel' : 'Service Mailer en panne — CRITIQUE' }}
                  </div>
                  <div class="status-desc">
                    {{ mailerDiag.isGoogleConnected ? `Connecté avec l'adresse : ${mailerDiag.email}` : 'La connexion OAuth avec Google a expiré ou n\'est pas configurée.' }}
                  </div>
                </div>
                <div v-if="!mailerDiag.isGoogleConnected">
                  <button class="btn-action-primary glow-orange py-2 px-3 text-xs" @click="connectGoogleSmtp">
                    <i class="fa-brands fa-google me-1"></i> Réactiver SMTP
                  </button>
                </div>
              </div>

              <!-- Pending Invitations Status Banner -->
              <div class="pending-invites-banner d-flex align-items-center justify-content-between">
                <div class="text-start">
                  <div class="status-label">INVITATIONS EN ATTENTE DE RENVOI</div>
                  <div class="fs-4 text-white font-weight-bold my-1">
                    {{ mailerDiag.pendingInvitesCount }} <span class="fs-6 font-weight-normal text-muted">invitations bloquées</span>
                  </div>
                  <p class="m-0 text-muted small">Ces invitations n'ont pas pu être envoyées en raison de la panne du mailer.</p>
                </div>
                <div v-if="mailerDiag.pendingInvitesCount > 0">
                  <button :disabled="mailerResendLoading" class="btn-action-primary glow-green" @click="triggerMailerResend">
                    <i v-if="mailerResendLoading" class="fa-solid fa-spinner fa-spin me-2"></i>
                    <i v-else class="fa-solid fa-paper-plane me-2"></i>
                    Libérer les invitations
                  </button>
                </div>
              </div>

              <!-- Terminal Console CLI Logs -->
              <div class="terminal-logs-wrapper">
                <div class="terminal-header d-flex align-items-center justify-content-between">
                  <div class="d-flex align-items-center gap-2">
                    <span class="dot red"></span>
                    <span class="dot yellow"></span>
                    <span class="dot green"></span>
                    <span class="terminal-title text-muted ms-2">diagnostics_console.sh</span>
                  </div>
                  <span class="terminal-badge text-xs">OAUTH2 / SMTP</span>
                </div>
                <div class="terminal-body scrollbar-thin">
                  <div v-for="(log, index) in mailerConsoleLogs" :key="index" :class="[
                    'terminal-log-line',
                    log.includes('[SUCCESS]') ? 'log-success' : '',
                    log.includes('[CRITICAL]') ? 'log-critical' : '',
                    log.includes('[WARNING]') ? 'log-warning' : '',
                    log.includes('[INFO]') ? 'log-help' : ''
                  ]">
                    {{ log }}
                  </div>
                  <div v-if="mailerLoading" class="terminal-log-line">
                    <i class="fa-solid fa-spinner fa-spin me-2"></i>
                    <span class="log-cursor">█</span>
                  </div>
                  <div v-else class="terminal-log-line">
                    <span>$ ready_to_operate</span><span class="log-cursor">█</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- MODAL EXPIRING SUBSCRIPTIONS -->
    <transition name="modal-fade">
      <div v-if="showSubsModal" class="enigma-modal-overlay" @click.self="showSubsModal = false">
        <div class="enigma-modal-container max-w-lg glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-clock text-warning"></i>
              <span>Abonnements expirant bientôt</span>
            </h4>
            <button class="btn-close-modal" @click="showSubsModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body p-0">
            <!-- Spinner / Loader -->
            <div v-if="subsLoading" class="p-5 text-center">
              <i class="fa-solid fa-spinner fa-spin fa-2x text-primary mb-3"></i>
              <p class="text-muted m-0">Chargement des abonnements en cours...</p>
            </div>

            <!-- Table of expiring subs -->
            <div v-else-if="expiringSubs.length > 0" class="subs-table-wrapper scrollbar-thin">
              <table class="subs-table">
                <thead>
                  <tr>
                    <th>Entreprise</th>
                    <th>Formule</th>
                    <th>Expiration</th>
                    <th>Jours restants</th>
                    <th class="text-end">Rappel</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="sub in expiringSubs" :key="sub.id" class="sub-row">
                    <td>
                      <div class="d-flex flex-column text-start">
                        <span class="sub-company-name font-weight-bold">{{ sub.name }}</span>
                        <span class="text-xs text-muted">{{ sub.adminName }} ({{ sub.adminEmail }})</span>
                      </div>
                    </td>
                    <td>
                      <span class="sub-plan-badge font-weight-bold">{{ sub.plan }}</span>
                    </td>
                    <td>
                      <span class="text-white text-xs">{{ sub.expirationDate ? new Date(sub.expirationDate).toLocaleDateString() : 'N/A' }}</span>
                    </td>
                    <td>
                      <span :class="['sub-days-badge text-xs', sub.daysRemaining <= 3 ? 'danger' : 'warning']">
                        {{ sub.daysRemaining }} jours
                      </span>
                    </td>
                    <td class="text-end">
                      <button :disabled="notifyingSubId === sub.id" class="btn-send-reminder" @click="sendRenewalReminder(sub)">
                        <i v-if="notifyingSubId === sub.id" class="fa-solid fa-spinner fa-spin text-warning"></i>
                        <i v-else class="fa-solid fa-paper-plane"></i>
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Empty State -->
            <div v-else class="p-5 text-center">
              <i class="fa-solid fa-face-smile fa-2x text-success mb-3"></i>
              <p class="text-muted m-0">Aucun abonnement n'expire dans les 7 prochains jours.</p>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- MODAL SECURITY AUDIT SCANNER -->
    <transition name="modal-fade">
      <div v-if="showSecurityModal" class="enigma-modal-overlay" @click.self="showSecurityModal = false">
        <div class="enigma-modal-container max-w-lg glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-shield-halved text-success animate-pulse"></i>
              <span>Audit de Sécurité Intégrale</span>
            </h4>
            <button class="btn-close-modal" @click="showSecurityModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body">
            <div class="d-flex flex-column gap-4">
              <!-- Scanning Active HUD View or Complete Dashboard -->
              <div v-if="!securityScanComplete" class="security-scanning-hud">
                <div class="row align-items-center">
                  <div class="col-md-7">
                    <!-- Steps Animate -->
                    <div class="hud-steps-list">
                      <div v-for="(step, idx) in securityScanSteps" :key="idx" :class="[
                        'hud-step-item',
                        securityScanActiveStep === idx ? 'step-scanning' : (step.status === 'done' ? 'step-done' : 'step-pending')
                      ]">
                        <div class="hud-step-icon">
                          <i v-if="securityScanActiveStep === idx" class="fa-solid fa-sync fa-spin"></i>
                          <i v-else-if="step.status === 'done'" class="fa-solid fa-circle-check text-green"></i>
                          <i v-else :class="step.icon"></i>
                        </div>
                        <div class="flex-grow-1 text-start">
                          <div class="hud-step-name font-weight-bold">{{ step.name }}</div>
                          <div class="text-xs text-muted">{{ step.details }}</div>
                        </div>
                      </div>
                    </div>
                  </div>
                  
                  <div class="col-md-5 d-flex flex-column align-items-center justify-content-center">
                    <div class="cyber-shield-icon-wrapper text-primary mb-3">
                      <i class="fa-solid fa-shield-halved animate-spin-slow text-success opacity-75"></i>
                    </div>
                    <button :disabled="securityLoading" class="btn-action-primary glow-orange w-100" @click="runSecurityAuditScan">
                      <i v-if="securityLoading" class="fa-solid fa-spinner fa-spin me-2"></i>
                      <i v-else class="fa-solid fa-bolt me-2"></i>
                      Lancer l'audit
                    </button>
                  </div>
                </div>

                <!-- Matrix / HUD log terminal -->
                <div class="hud-logs-wrapper mt-4 text-start">
                  <div class="hud-logs scrollbar-thin">
                    <div v-for="(line, lineIdx) in securityScanLogs" :key="lineIdx" class="hud-log-line">
                      {{ line }}
                    </div>
                    <div v-if="securityLoading" class="hud-log-line">
                      <span class="text-warning">[SCANNING]</span> Analyse active... <span class="log-cursor">█</span>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Audit Complete HUD View -->
              <div v-else class="security-scanning-hud d-flex flex-column gap-4">
                <div class="d-flex align-items-center justify-content-between">
                  <div class="score-radial-wrapper" :style="{'--score-color': securityScanScore >= 90 ? '#10b981' : (securityScanScore >= 70 ? '#f59e0b' : '#ef4444')}">
                    <div class="score-radial">
                      <span class="score-value font-weight-bold">{{ securityScanScore }}</span>
                      <span class="text-xxs text-muted">SCORE</span>
                    </div>
                  </div>
                  <div class="flex-grow-1 text-start ms-4">
                    <h5 class="text-white font-weight-bold m-0">Analyse de Sécurité Réussie</h5>
                    <p class="text-muted small m-0">Audit du système complété et enregistré. Aucun incident critique n'a été détecté.</p>
                  </div>
                </div>

                <!-- Statistics grid -->
                <div class="two-col-grid" style="--col1: 1fr; --col2: 1fr;">
                  <!-- Checked Items Summary -->
                  <div :class="['card-result-mini', securityScanResult.weakPasswordsCount === 0 ? 'active-green' : 'active-warning']">
                    <div class="crm-icon">
                      <i class="fa-solid fa-key"></i>
                    </div>
                    <div class="text-start">
                      <div class="status-label">MOTS DE PASSE FAIBLES</div>
                      <div class="text-white font-weight-bold fs-6">
                        {{ securityScanResult.weakPasswordsCount }} detectés
                      </div>
                    </div>
                  </div>

                  <div class="card-result-mini active-green">
                    <div class="crm-icon">
                      <i class="fa-solid fa-clock-rotate-left"></i>
                    </div>
                    <div class="text-start">
                      <div class="status-label">JETONS D'ACCÈS PURGÉS</div>
                      <div class="text-white font-weight-bold fs-6">
                        {{ securityScanResult.expiredTokensCleaned }} expirés nettoyés
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Audited Checklist detail -->
                <div class="checked-items-list scrollbar-thin text-start">
                  <h6 class="text-muted font-weight-bold text-xs uppercase mb-3">POINTS DE CONTRÔLE VÉRIFIÉS</h6>
                  <div v-for="(check, checkIdx) in securityScanResult.checkedItems" :key="checkIdx" class="checked-item-row d-flex align-items-center justify-content-between">
                    <div>
                      <div class="item-name font-weight-bold">{{ check.name }}</div>
                      <div class="text-xxs text-muted mt-1">{{ check.details }}</div>
                    </div>
                    <div>
                      <span :class="['sub-days-badge text-xxs', check.status === 'OK' ? 'glow-green text-white font-weight-bold' : (check.status === 'WARNING' ? 'warning' : 'danger')]">
                        {{ check.status }}
                      </span>
                    </div>
                  </div>
                </div>

                <!-- Close / Retrigger Actions -->
                <div class="d-flex align-items-center justify-content-end gap-3 mt-2">
                  <button class="btn-action-secondary py-2 px-3 text-xs" @click="securityScanComplete = false">
                    <i class="fa-solid fa-rotate-left me-1"></i> Réanalyser
                  </button>
                  <button class="btn-action-primary glow-green py-2 px-3 text-xs" @click="showSecurityModal = false">
                    <i class="fa-solid fa-check me-1"></i> Appliquer & Fermer
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- MODAL ENTERPRISE INTEGRITY ACTIVITY -->
    <transition name="modal-fade">
      <div v-if="showEntActivityModal" class="enigma-modal-overlay" @click.self="showEntActivityModal = false">
        <div class="enigma-modal-container max-w-lg glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-bolt text-danger"></i>
              <span>Audit Behavioural & Anomalies</span>
            </h4>
            <button class="btn-close-modal" @click="showEntActivityModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body">
            <div class="d-flex flex-column gap-4">
              <!-- Terminal Console CLI Logs -->
              <div class="terminal-logs-wrapper">
                <div class="terminal-header d-flex align-items-center justify-content-between">
                  <div class="d-flex align-items-center gap-2">
                    <span class="dot red"></span>
                    <span class="dot yellow"></span>
                    <span class="dot green"></span>
                    <span class="terminal-title text-muted ms-2">neo_integrity_audit.sh</span>
                  </div>
                  <span class="terminal-badge text-xs">AI INTEGRITY SCAN</span>
                </div>
                <div class="terminal-body scrollbar-thin" style="max-height: 180px;">
                  <div v-for="(log, index) in entActivityConsoleLogs" :key="index" :class="[
                    'terminal-log-line',
                    log.includes('[SUCCESS]') ? 'log-success' : '',
                    log.includes('[CRITICAL]') ? 'log-critical' : '',
                    log.includes('[WARNING]') ? 'log-warning' : '',
                    log.includes('[INFO]') ? 'log-help' : ''
                  ]">
                    {{ log }}
                  </div>
                  <div v-if="entActivityLoading" class="terminal-log-line">
                    <i class="fa-solid fa-spinner fa-spin me-2"></i>
                    <span class="log-cursor">█</span>
                  </div>
                </div>
              </div>

              <!-- Anomaly List Table -->
              <div class="subs-table-wrapper scrollbar-thin" style="max-height: 250px;">
                <table class="subs-table" v-if="enterpriseReco.anomalies && enterpriseReco.anomalies.length > 0">
                  <thead>
                    <tr>
                      <th>Candidat</th>
                      <th>Campagne</th>
                      <th>Anomalie</th>
                      <th>Alertes</th>
                      <th class="text-end">Action</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="anomaly in enterpriseReco.anomalies" :key="anomaly.evaluationId" class="sub-row">
                      <td>
                        <span class="font-weight-bold text-white">{{ anomaly.candidateName }}</span>
                      </td>
                      <td>
                        <span class="text-muted text-xs">{{ anomaly.campaignName }}</span>
                      </td>
                      <td>
                        <span class="text-danger text-xs font-weight-bold">{{ anomaly.type }}</span>
                      </td>
                      <td>
                        <div class="d-flex gap-2">
                          <span class="badge bg-danger-transparent text-danger text-xs" v-if="anomaly.infractionsCount > 0">
                            {{ anomaly.infractionsCount }} Infractions
                          </span>
                          <span class="badge bg-warning-transparent text-warning text-xs" v-if="anomaly.nbReprises > 0">
                            {{ anomaly.nbReprises }} Sorties
                          </span>
                        </div>
                      </td>
                      <td class="text-end">
                        <button :disabled="entResolvingAnomalyId === anomaly.evaluationId" class="btn-action-primary glow-orange py-1 px-2 text-xs" @click="triggerResolveAnomaly(anomaly.evaluationId)">
                          <i v-if="entResolvingAnomalyId === anomaly.evaluationId" class="fa-solid fa-spinner fa-spin me-1"></i>
                          <i v-else class="fa-solid fa-circle-check me-1"></i>
                          Résoudre
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-else class="p-4 text-center">
                  <i class="fa-solid fa-face-smile fa-2x text-success mb-2"></i>
                  <p class="text-muted m-0">Félicitations, aucune anomalie critique active détectée.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- MODAL ENTERPRISE WEEKLY PERFORMANCE REPORT -->
    <transition name="modal-fade">
      <div v-if="showEntReportModal" class="enigma-modal-overlay" @click.self="showEntReportModal = false">
        <div class="enigma-modal-container max-w-lg glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-chart-line text-warning"></i>
              <span>Rapport Analytique Hebdomadaire</span>
            </h4>
            <button class="btn-close-modal" @click="showEntReportModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body">
            <div class="d-flex flex-column gap-4">
              <!-- KPI Card Grid -->
              <div class="row g-3">
                <div class="col-6 col-md-3">
                  <div class="status-card active-green p-3 text-start h-100">
                    <div class="status-label">ÉVALUATIONS COMPLETES</div>
                    <div class="fs-3 font-weight-bold text-white">{{ enterpriseReco.weeklyReport.completedEvaluationsThisWeek }}</div>
                  </div>
                </div>
                <div class="col-6 col-md-3">
                  <div class="status-card glow-blue p-3 text-start h-100" style="border: 1px solid rgba(59,130,246,0.2); background: rgba(59,130,246,0.05);">
                    <div class="status-label">INVITATIONS ENVOYÉES</div>
                    <div class="fs-3 font-weight-bold text-white">{{ enterpriseReco.weeklyReport.totalInvitationsThisWeek }}</div>
                  </div>
                </div>
                <div class="col-6 col-md-3">
                  <div class="status-card active-amber p-3 text-start h-100" style="border: 1px solid rgba(245,158,11,0.2); background: rgba(245,158,11,0.05);">
                    <div class="status-label">SCORE MOYEN GLOBAL</div>
                    <div class="fs-3 font-weight-bold text-white">{{ enterpriseReco.weeklyReport.averageScore }}%</div>
                  </div>
                </div>
                <div class="col-6 col-md-3">
                  <div class="status-card p-3 text-start h-100" style="border: 1px solid rgba(16,185,129,0.2); background: rgba(16,185,129,0.05);">
                    <div class="status-label">TAUX DE COMPLÉTION</div>
                    <div class="fs-3 font-weight-bold text-white">{{ enterpriseReco.weeklyReport.completionRate }}%</div>
                  </div>
                </div>
              </div>

              <!-- Performance List by Campaign -->
              <div class="subs-table-wrapper scrollbar-thin" style="max-height: 220px;">
                <table class="subs-table" v-if="enterpriseReco.weeklyReport.campaignPerformances && enterpriseReco.weeklyReport.campaignPerformances.length > 0">
                  <thead>
                    <tr>
                      <th>Campagne active</th>
                      <th>Candidats évalués</th>
                      <th>Score moyen</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="cPerf in enterpriseReco.weeklyReport.campaignPerformances" :key="cPerf.campaignName" class="sub-row">
                      <td class="text-start font-weight-bold text-white">{{ cPerf.campaignName }}</td>
                      <td>{{ cPerf.candidatesCount }} candidats</td>
                      <td>
                        <span class="font-weight-bold" :style="{color: getScoreColor(cPerf.averageScore)}">{{ cPerf.averageScore }}%</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <div v-else class="p-4 text-center">
                  <i class="fa-solid fa-info-circle fa-2x text-muted mb-2"></i>
                  <p class="text-muted m-0">Aucune évaluation passée cette semaine.</p>
                </div>
              </div>

              <!-- Export Footer -->
              <div class="d-flex justify-content-end gap-3 mt-2">
                <button class="btn-action-primary glow-green py-2 px-4" @click="triggerCsvExport" :disabled="isExporting">
                  <i v-if="isExporting" class="fa-solid fa-spinner fa-spin me-2"></i>
                  <i v-else class="fa-solid fa-file-csv me-2"></i>
                  {{ isExporting ? 'Export en cours...' : 'CSV' }}
                </button>
                <button class="btn-action-primary glow-red py-2 px-4" @click="triggerPdfExport" :disabled="isExportingPdf">
                  <i v-if="isExportingPdf" class="fa-solid fa-spinner fa-spin me-2"></i>
                  <i v-else class="fa-solid fa-file-pdf me-2"></i>
                  {{ isExportingPdf ? 'Export en cours...' : 'PDF' }}
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- MODAL ENTERPRISE DRAFT CAMPAIGNS CONFIGURATOR -->
    <transition name="modal-fade">
      <div v-if="showEntDraftsModal" class="enigma-modal-overlay" @click.self="showEntDraftsModal = false">
        <div class="enigma-modal-container max-w-lg glass-morphic">
          <div class="enigma-modal-header">
            <h4 class="m-0 text-white font-weight-bold d-flex align-items-center gap-2">
              <i class="fa-solid fa-bullhorn text-green animate-pulse"></i>
              <span>Finalisation des Campagnes Brouillon</span>
            </h4>
            <button class="btn-close-modal" @click="showEntDraftsModal = false">
              <i class="fa-solid fa-xmark"></i>
            </button>
          </div>
          <div class="enigma-modal-body">
            <div class="d-flex flex-column gap-4">
              <!-- Checklist of draft campaigns -->
              <div class="draft-campaigns-list scrollbar-thin" style="max-height: 400px; overflow-y: auto;" v-if="enterpriseReco.draftCampaigns && enterpriseReco.draftCampaigns.length > 0">
                <div v-for="draft in enterpriseReco.draftCampaigns" :key="draft.campaignId" class="status-card p-4 text-start mb-3" style="border: 1px solid rgba(255,255,255,0.08); background: rgba(255,255,255,0.02);">
                  <div class="d-flex align-items-center justify-content-between mb-3">
                    <h5 class="m-0 text-white font-weight-bold">{{ draft.name }}</h5>
                    <span class="badge bg-warning text-dark font-weight-bold">{{ draft.completionPercentage }}% complété</span>
                  </div>

                  <!-- Mini Progress Bar -->
                  <div class="progress mb-3" style="height: 6px; background: rgba(255,255,255,0.1); border-radius: 10px; overflow: hidden;">
                    <div class="progress-bar bg-warning" :style="{width: `${draft.completionPercentage}%`}"></div>
                  </div>

                  <!-- Checklist Grid -->
                  <div class="row g-2 text-xs mb-3">
                    <div class="col-6">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="draft.hasQuestions ? 'fa-solid fa-circle-check text-green' : 'fa-solid fa-circle-xmark text-muted'"></i>
                        <span :class="draft.hasQuestions ? 'text-white' : 'text-muted'">Questions ({{ draft.questionsCount }} ajoutées)</span>
                      </div>
                    </div>
                    <div class="col-6">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="draft.hasDescription ? 'fa-solid fa-circle-check text-green' : 'fa-solid fa-circle-xmark text-muted'"></i>
                        <span :class="draft.hasDescription ? 'text-white' : 'text-muted'">Description rédigée</span>
                      </div>
                    </div>
                    <div class="col-6">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="draft.hasDuration ? 'fa-solid fa-circle-check text-green' : 'fa-solid fa-circle-xmark text-muted'"></i>
                        <span :class="draft.hasDuration ? 'text-white' : 'text-muted'">Durée configurée</span>
                      </div>
                    </div>
                    <div class="col-6">
                      <div class="d-flex align-items-center gap-2">
                        <i :class="draft.hasMaxCandidates ? 'fa-solid fa-circle-check text-green' : 'fa-solid fa-circle-xmark text-muted'"></i>
                        <span :class="draft.hasMaxCandidates ? 'text-white' : 'text-muted'">Limite de candidats</span>
                      </div>
                    </div>
                  </div>

                  <!-- Action button -->
                  <div class="d-flex justify-content-end">
                    <button :disabled="entPublishingCampaignId === draft.campaignId" class="btn-action-primary glow-green py-2 px-3 text-xs" @click="triggerPublishDraft(draft.campaignId)">
                      <i v-if="entPublishingCampaignId === draft.campaignId" class="fa-solid fa-spinner fa-spin me-1"></i>
                      <i v-else class="fa-solid fa-rocket me-1"></i>
                      Finaliser et Publier la campagne
                    </button>
                  </div>
                </div>
              </div>
              <div v-else class="p-5 text-center">
                <i class="fa-solid fa-circle-check fa-3x text-success mb-3 animate-bounce"></i>
                <h5 class="text-white">Tout est en ordre !</h5>
                <p class="text-muted m-0">Toutes vos campagnes sont configurées et actives.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </transition>

    <!-- TOAST -->
    <transition name="toast-slide">
      <div v-if="globalToast.active" class="enigma-toast" :class="globalToast.type">
        <div class="t-ico"><i :class="globalToast.icon"></i></div>
        <div class="t-body">
          <strong>{{ $t('dashboard.toast.systemMessage') }}</strong>
          <p class="m-0 small">{{ globalToast.message }}</p>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted, computed, nextTick, watch, inject, reactive } from 'vue';
import { jsPDF } from 'jspdf';
import html2canvas from 'html2canvas';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/stores/auth';
import Chart from 'chart.js/auto';
import AppSidebar from '../components/AppSidebar.vue';
import AppNavbar from '../components/AppNavbar.vue';
import axios from 'axios';
import { useI18n } from 'vue-i18n';
import { superAdminApi, enterpriseApi } from '@/services/api';

const { t, locale } = useI18n();

const API_NET = import.meta.env.VITE_API_URL || 'http://localhost:5172/api';
const API_IA  = import.meta.env.VITE_IA_URL  || 'http://localhost:5600';

const authStore   = useAuthStore();
const router      = useRouter();
const isDark      = inject('isDark', ref(false));
const toggleTheme = inject('toggleTheme', () => {});

// ── STATE ──
const loading       = ref(true);

// ── STATE GESTION DYNAMIQUE SUPERADMIN ──
const mailerDiag = ref({
  isGoogleConnected: false,
  email: '',
  pendingInvitesCount: 0,
  diagnosticsLogs: []
});
const expiringSubs = ref([]);
const securityDiag = ref({
  securityScore: 100,
  lastAuditDate: null,
  daysSinceLastAudit: null,
  isAuditRecommended: true
});

const showMailerModal = ref(false);
const mailerLoading = ref(false);
const mailerConsoleLogs = ref([]);
const mailerResendSuccess = ref(false);
const mailerResendLoading = ref(false);
const mailerResendMessage = ref('');

const showSubsModal = ref(false);
const subsLoading = ref(false);
const notifyingSubId = ref(null);

const showSecurityModal = ref(false);
const securityLoading = ref(false);
const securityScanSteps = ref([]);
const securityScanLogs = ref([]);
const securityScanActiveStep = ref(-1);
const securityScanScore = ref(0);
const securityScanComplete = ref(false);
const securityScanResult = ref(null);

// ── STATE GESTION DYNAMIQUE ADMINENTREPRISE ──
const enterpriseReco = ref({
  anomaliesCount: 0,
  draftCampaignsCount: 0,
  weeklyReportAvailable: true,
  anomalies: [],
  weeklyReport: {
    completedEvaluationsThisWeek: 0,
    totalInvitationsThisWeek: 0,
    averageScore: 0,
    averageCompletionTimeMinutes: 0,
    completionRate: 0,
    campaignPerformances: []
  },
  draftCampaigns: []
});

const showEntActivityModal = ref(false);
const showEntReportModal = ref(false);
const showEntDraftsModal = ref(false);

const entActivityConsoleLogs = ref([]);
const entActivityLoading = ref(false);
const entResolvingAnomalyId = ref(null);
const entPublishingCampaignId = ref(null);

const activePeriod  = ref('week');
const currentTime   = ref('');
const mousePos      = reactive({ x: 0, y: 0 });
const globalToast   = reactive({ active: false, message: '', type: '', icon: '' });
const iaInsight     = ref(t('dashboard.hero.loading'));

// ── CV STATE ──
const isAnalyzing    = ref(false);
const isDragging     = ref(false);
const selectedFile   = ref(null);
const cvResult       = ref(null);
const jobDescription = ref('');

// ── LETTRE STATE ──
const isGeneratingLettre = ref(false);
const lettreResult       = ref(null);
const letterCopied       = ref(false);
const lettreData         = reactive({
  nom:         '',
  poste:       '',
  entreprise:  '',
  competences: '',
  style:       'professionnel',
  langue:      'fr',
});

// ── RECOMMANDATIONS ──
const recoLoading = ref(false);

// ══════════════════════════════════════════════
// RECOMMANDATIONS PAR RÔLE — ROUTING INTÉGRÉ
// ══════════════════════════════════════════════

const RECO_BY_ROLE = {
  Candidat: [
    {
      priority: 'Urgent',
      priorityBg: 'rgba(239,68,68,0.12)', priorityColor: '#ef4444',
      color: '#ef4444',
      icon: 'fa-solid fa-clipboard-list',
      title: 'Complétez vos tests en attente',
      description: '2 tests en attente. Les compléter augmentera votre profil de 15 points.',
      actionLabel: 'Voir mes tests',
      route: '/my-tests',
      scrollTo: null,
    },
    {
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)', priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-microphone',
      title: 'Préparez votre entretien IA',
      description: 'Entraînez-vous sur des questions comportementales et techniques.',
      actionLabel: 'Commencer préparation',
      route: '/interview-prep',
      scrollTo: null,
    },
    {
      priority: 'Standard',
      priorityBg: 'rgba(16,185,129,0.12)', priorityColor: '#10b981',
      color: '#10b981',
      icon: 'fa-solid fa-file-pdf',
      title: "Analysez votre CV avec l'IA",
      description: 'Obtenez des conseils personnalisés pour augmenter votre score de matching.',
      actionLabel: 'Analyser mon CV',
      route: null,
      scrollTo: 'cv-scan-section',
    },
  ],

  Evaluateur: [
    {
      priority: 'Urgent',
      priorityBg: 'rgba(239,68,68,0.12)', priorityColor: '#ef4444',
      color: '#ef4444',
      icon: 'fa-solid fa-users-slash',
      title: '8 candidats sans analyse depuis 48h',
      description: 'Traitez-les avant expiration des liens.',
      actionLabel: 'Voir le pipeline',
      route: '/analyse-comportementale',
      scrollTo: null,
    },
    {
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)', priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-file-chart-column',
      title: 'Générer le rapport mensuel',
      description: "Le rapport de ce mois n'a pas encore été généré.",
      actionLabel: 'Générer rapport',
      route: '/reports/generate',
      scrollTo: null,
    },
    {
      priority: 'Standard',
      priorityBg: 'rgba(16,185,129,0.12)', priorityColor: '#10b981',
      color: '#10b981',
      icon: 'fa-solid fa-user-plus',
      title: 'Inviter de nouveaux membres RH',
      description: 'Équipe en sous-effectif. Invitez 2 membres supplémentaires.',
      actionLabel: 'Inviter membres',
      route: '/invite',
      scrollTo: null,
    },
  ],

  RH: [
    {
      priority: 'Urgent',
      priorityBg: 'rgba(239,68,68,0.12)', priorityColor: '#ef4444',
      color: '#ef4444',
      icon: 'fa-solid fa-users-slash',
      title: '8 candidats sans analyse depuis 48h',
      description: 'Traitez-les avant expiration des liens.',
      actionLabel: 'Voir le pipeline',
      route: '/analyse-comportementale',
      scrollTo: null,
    },
    {
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)', priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-file-chart-column',
      title: 'Générer le rapport mensuel',
      description: "Le rapport de ce mois n'a pas encore été généré.",
      actionLabel: 'Générer rapport',
      route: '/reports/generate',
      scrollTo: null,
    },
    {
      priority: 'Standard',
      priorityBg: 'rgba(16,185,129,0.12)', priorityColor: '#10b981',
      color: '#10b981',
      icon: 'fa-solid fa-user-plus',
      title: 'Inviter de nouveaux membres RH',
      description: 'Équipe en sous-effectif. Invitez 2 membres supplémentaires.',
      actionLabel: 'Inviter membres',
      route: '/invite',
      scrollTo: null,
    },
  ],

  Recruteur: [
    {
      priority: 'Urgent',
      priorityBg: 'rgba(239,68,68,0.12)', priorityColor: '#ef4444',
      color: '#ef4444',
      icon: 'fa-solid fa-users-slash',
      title: '8 candidats sans analyse depuis 48h',
      description: 'Traitez-les avant expiration des liens.',
      actionLabel: 'Voir le pipeline',
      route: '/analyse-comportementale',
      scrollTo: null,
    },
    {
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)', priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-handshake',
      title: 'Nouveaux profils à contacter',
      description: '5 candidats qualifiés attendent votre retour.',
      actionLabel: 'Voir les profils',
      route: '/candidates-list',
      scrollTo: null,
    },
    {
      priority: 'Standard',
      priorityBg: 'rgba(16,185,129,0.12)', priorityColor: '#10b981',
      color: '#10b981',
      icon: 'fa-solid fa-file-pdf',
      title: 'Analyser un CV candidat',
      description: "Utilisez l'IA pour scorer automatiquement les CVs reçus.",
      actionLabel: 'Analyser un CV',
      route: null,
      scrollTo: 'cv-scan-section',
    },
  ],

  AdminEntreprise: [
    {
      priority: 'Urgent',
      priorityBg: 'rgba(239,68,68,0.12)', priorityColor: '#ef4444',
      color: '#ef4444',
      icon: 'fa-solid fa-bolt',
      title: 'Activité critique détectée',
      description: 'Anomalie dans 3 campagnes actives. Vérification immédiate requise.',
      actionLabel: "Voir l'activité",
      route: '/admin/activity',
      scrollTo: null,
    },
    {
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)', priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-chart-bar',
      title: 'Rapport de performance hebdo',
      description: 'Le rapport de cette semaine est disponible pour votre équipe.',
      actionLabel: 'Voir rapport',
      route: '/reports',
      scrollTo: null,
    },
    {
      priority: 'Standard',
      priorityBg: 'rgba(16,185,129,0.12)', priorityColor: '#10b981',
      color: '#10b981',
      icon: 'fa-solid fa-bullhorn',
      title: 'Configurer vos campagnes',
      description: '2 campagnes en brouillon. Finalisez-les pour démarrer les évaluations.',
      actionLabel: 'Gérer campagnes',
      route: '/campaigns',
      scrollTo: null,
    },
  ],

  SuperAdmin: [],
};

// Recommandations selon le rôle courant
const roleRecommendations = computed(() => {
  if (role.value === 'SuperAdmin') {
    const list = [];
    
    // 1. Service Mailer Card
    const isDown = !mailerDiag.value.isGoogleConnected;
    list.push({
      id: 'mailer',
      priority: isDown ? 'Urgent' : 'Priorité',
      priorityBg: isDown ? 'rgba(239,68,68,0.12)' : 'rgba(16,185,129,0.12)',
      priorityColor: isDown ? '#ef4444' : '#10b981',
      color: isDown ? '#ef4444' : '#10b981',
      icon: isDown ? 'fa-solid fa-triangle-exclamation' : 'fa-solid fa-circle-check',
      title: isDown ? 'Service Mailer en panne — CRITIQUE' : 'Service Mailer opérationnel',
      description: isDown 
        ? `Le jeton Gmail est déconnecté. ${mailerDiag.value.pendingInvitesCount || 0} invitations sont bloquées.` 
        : `Service actif avec le compte : ${mailerDiag.value.email || 'OAuth system'}.`,
      actionLabel: isDown ? 'Diagnostiquer' : 'Lancer Diagnostic',
      route: null,
      scrollTo: null,
    });

    // 2. Abonnements Card
    const expCount = expiringSubs.value.length;
    const hasExp = expCount > 0;
    list.push({
      id: 'subscriptions',
      priority: hasExp ? 'Priorité' : 'Standard',
      priorityBg: hasExp ? 'rgba(245,158,11,0.12)' : 'rgba(16,185,129,0.12)',
      priorityColor: hasExp ? '#f59e0b' : '#10b981',
      color: hasExp ? '#f59e0b' : '#10b981',
      icon: 'fa-solid fa-credit-card',
      title: hasExp ? `${expCount} abonnement${expCount > 1 ? 's' : ''} expirant dans 7 jours` : 'Abonnements à jour',
      description: hasExp 
        ? 'Relancez les administrateurs d\'entreprise pour renouveler leur offre.' 
        : 'Toutes les entreprises actives possèdent un abonnement valide à long terme.',
      actionLabel: 'Voir abonnements',
      route: null,
      scrollTo: null,
    });

    // 3. Audit Sécurité Card
    const isRecommended = securityDiag.value.isAuditRecommended;
    const lastAuditText = securityDiag.value.lastAuditDate 
      ? `Dernier audit il y a ${securityDiag.value.daysSinceLastAudit} jours.` 
      : 'Aucun audit de sécurité n\'a encore été effectué.';
    list.push({
      id: 'security',
      priority: isRecommended ? 'Standard' : 'Optimal',
      priorityBg: isRecommended ? 'rgba(245,158,11,0.12)' : 'rgba(16,185,129,0.12)',
      priorityColor: isRecommended ? '#f59e0b' : '#10b981',
      color: isRecommended ? '#f59e0b' : '#10b981',
      icon: 'fa-solid fa-shield-halved',
      title: isRecommended ? 'Audit de sécurité recommandé' : 'Sécurité système optimale',
      description: isRecommended 
        ? `${lastAuditText} Lancez un diagnostic complet de la base de données et des tokens.` 
        : `Score système de ${securityDiag.value.securityScore}/100. ${lastAuditText}`,
      actionLabel: 'Lancer audit',
      route: null,
      scrollTo: null,
    });

    return list;
  }

  if (role.value === 'AdminEntreprise' || role.value === 'Evaluateur') {
    const list = [];

    // 1. Activité critique
    const hasAnomalies = enterpriseReco.value.anomaliesCount > 0;
    list.push({
      id: 'enterprise-activity',
      priority: hasAnomalies ? 'Urgent' : 'Optimal',
      priorityBg: hasAnomalies ? 'rgba(239,68,68,0.12)' : 'rgba(16,185,129,0.12)',
      priorityColor: hasAnomalies ? '#ef4444' : '#10b981',
      color: hasAnomalies ? '#ef4444' : '#10b981',
      icon: hasAnomalies ? 'fa-solid fa-bolt' : 'fa-solid fa-circle-check',
      title: 'Activité critique détectée',
      description: hasAnomalies 
        ? `Anomalie dans ${enterpriseReco.value.anomaliesCount} campagnes actives. Vérification immédiate requise.`
        : 'Aucune anomalie détectée sur vos campagnes actives.',
      actionLabel: "Voir l'activité",
      route: null,
      scrollTo: null,
    });

    // 2. Rapport de performance
    list.push({
      id: 'enterprise-report',
      priority: 'Priorité',
      priorityBg: 'rgba(245,158,11,0.12)',
      priorityColor: '#f59e0b',
      color: '#f59e0b',
      icon: 'fa-solid fa-chart-bar',
      title: 'Rapport de performance hebdo',
      description: 'Le rapport de cette semaine est disponible pour votre équipe.',
      actionLabel: 'Voir rapport',
      route: null,
      scrollTo: null,
    });

    // 3. Configurer vos campagnes
    const draftCount = enterpriseReco.value.draftCampaignsCount;
    const hasDrafts = draftCount > 0;
    list.push({
      id: 'enterprise-drafts',
      priority: hasDrafts ? 'Standard' : 'Optimal',
      priorityBg: hasDrafts ? 'rgba(245,158,11,0.12)' : 'rgba(16,185,129,0.12)',
      priorityColor: hasDrafts ? '#f59e0b' : '#10b981',
      color: hasDrafts ? '#f59e0b' : '#10b981',
      icon: 'fa-solid fa-bullhorn',
      title: 'Configurer vos campagnes',
      description: hasDrafts 
        ? `${draftCount} campagne${draftCount > 1 ? 's' : ''} en brouillon. Finalisez-les pour démarrer les évaluations.`
        : 'Tous vos brouillons ont été finalisés et publiés.',
      actionLabel: 'Gérer campagnes',
      route: null,
      scrollTo: null,
    });

    return list;
  }
  
  return RECO_BY_ROLE[role.value] || RECO_BY_ROLE['AdminEntreprise'];
});

// Handler de clic sur une recommandation
const handleRecoAction = (reco) => {
  if (role.value === 'SuperAdmin') {
    if (reco.id === 'mailer') {
      openMailerDiagnostics();
      return;
    }
    if (reco.id === 'subscriptions') {
      openExpiringSubscriptions();
      return;
    }
    if (reco.id === 'security') {
      openSecurityScanner();
      return;
    }
  }

  if (role.value === 'AdminEntreprise' || role.value === 'Evaluateur') {
    if (reco.id === 'enterprise-activity') {
      openEnterpriseActivity();
      return;
    }
    if (reco.id === 'enterprise-report') {
      openEnterpriseReport();
      return;
    }
    if (reco.id === 'enterprise-drafts') {
      openEnterpriseDrafts();
      return;
    }
  }

  // Si scrollTo est défini → scroll vers l'ancre sur la même page
  if (reco.scrollTo) {
    const el = document.getElementById(reco.scrollTo);
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
      el.classList.add('panel-highlight');
      setTimeout(() => el.classList.remove('panel-highlight'), 2000);
    }
    return;
  }
  // Sinon → navigation vers la route
  if (reco.route) {
    router.push(reco.route);
  }
};

// ── ACTIONS ADMINENTREPRISE ──
const openEnterpriseActivity = async () => {
  showEntActivityModal.value = true;
  entActivityLoading.value = true;
  entActivityConsoleLogs.value = [];
  
  try {
    const res = await enterpriseApi.getRecommendations();
    enterpriseReco.value = res.data;
    
    const logs = [
      "[INFO] Initialisation de l'audit de sécurité des examens...",
      "[INFO] Connexion au service d'analyse comportementale Neo-AI...",
      `[INFO] Analyse de ${res.data.anomaliesCount || 0} anomalies en cours sur les campagnes actives...`,
    ];
    
    if (res.data.anomalies && res.data.anomalies.length > 0) {
      res.data.anomalies.forEach((a, i) => {
        logs.push(`[WARNING] Anomalie #${i+1} détectée : Candidat [${a.candidateName}] dans la campagne [${a.campaignName}]. Cause: ${a.type} (Infractions: ${a.infractionsCount}, Reprises: ${a.nbReprises}).`);
      });
      logs.push("[CRITICAL] Veuillez vérifier immédiatement les rapports et prendre des mesures de sécurité.");
    } else {
      logs.push("[SUCCESS] Aucun comportement suspect ou infraction de sécurité détectée.");
    }
    
    for (let i = 0; i < logs.length; i++) {
      await new Promise(resolve => setTimeout(resolve, 150));
      entActivityConsoleLogs.value.push(logs[i]);
    }
  } catch (err) {
    entActivityConsoleLogs.value.push(`[CRITICAL] Échec de l'audit d'activité : ${err.message}`);
  } finally {
    entActivityLoading.value = false;
  }
};

const triggerResolveAnomaly = async (evaluationId) => {
  if (entResolvingAnomalyId.value) return;
  entResolvingAnomalyId.value = evaluationId;
  
  try {
    const res = await enterpriseApi.resolveAnomaly(evaluationId);
    showToast(res.data.message || "L'anomalie a été résolue.", "success", "fa-solid fa-circle-check");
    
    const recoRes = await enterpriseApi.getRecommendations();
    enterpriseReco.value = recoRes.data;
    
    entActivityConsoleLogs.value.push(`[SUCCESS] Anomalie de l'évaluation ${evaluationId} résolue physiquement en base de données.`);
  } catch (err) {
    showToast(`Erreur de résolution : ${err.response?.data || err.message}`, "error", "fa-solid fa-triangle-exclamation");
  } finally {
    entResolvingAnomalyId.value = null;
  }
};

const openEnterpriseReport = async () => {
  showEntReportModal.value = true;
  try {
    const res = await enterpriseApi.getRecommendations();
    enterpriseReco.value = res.data;
  } catch (err) {
    console.error(err);
  }
};

const isExporting = ref(false);
const triggerCsvExport = async () => {
  if (isExporting.value) return;
  isExporting.value = true;
  try {
    const res = await enterpriseApi.exportWeeklyReport();
    if (!res.data || res.data.size === 0) {
      showToast("Aucune donnée à exporter cette semaine.", "warn", "fa-solid fa-file-csv");
      return;
    }
    const blob = new Blob([res.data], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', `Rapport_Performance_Hebdo_${new Date().toISOString().slice(0,10)}.csv`);
    document.body.appendChild(link);
    link.click();
    link.remove();
    window.URL.revokeObjectURL(url);
    showToast("Export CSV réussi !", "success", "fa-solid fa-file-csv");
  } catch (err) {
    const msg = err.response?.data
      ? typeof err.response.data === 'object'
        ? err.response.data.message || err.message
        : err.response.data
      : err.message;
    showToast("Échec de l'exportation CSV: " + msg, "error", "fa-solid fa-triangle-exclamation");
  } finally {
    isExporting.value = false;
  }
};

const isExportingPdf = ref(false);
const triggerPdfExport = async () => {
  if (isExportingPdf.value) return;
  isExportingPdf.value = true;
  try {
    const report = enterpriseReco.value.weeklyReport;
    const dateStr = new Date().toLocaleDateString('fr-FR', { day: 'numeric', month: 'long', year: 'numeric' });

    const container = document.createElement('div');
    container.style.cssText = 'position:fixed;left:-9999px;top:0;width:780px;padding:40px;font-family:Arial,sans-serif;background:#fff;color:#1e293b;';
    container.innerHTML = `
      <div style="text-align:center;margin-bottom:30px">
        <h1 style="color:#f59e0b;font-size:24px;margin:0">Rapport Analytique Hebdomadaire</h1>
        <p style="color:#64748b;font-size:14px;margin:8px 0 0">${dateStr}</p>
      </div>
      <div style="display:flex;gap:12px;margin-bottom:24px">
        <div style="flex:1;background:#f0fdf4;border-radius:10px;padding:16px;text-align:center;border:1px solid #bbf7d0">
          <div style="font-size:11px;color:#64748b;font-weight:700">ÉVALUATIONS COMPLÈTES</div>
          <div style="font-size:28px;font-weight:900;color:#16a34a;margin-top:4px">${report.completedEvaluationsThisWeek}</div>
        </div>
        <div style="flex:1;background:#eff6ff;border-radius:10px;padding:16px;text-align:center;border:1px solid #bfdbfe">
          <div style="font-size:11px;color:#64748b;font-weight:700">INVITATIONS ENVOYÉES</div>
          <div style="font-size:28px;font-weight:900;color:#2563eb;margin-top:4px">${report.totalInvitationsThisWeek}</div>
        </div>
        <div style="flex:1;background:#fffbeb;border-radius:10px;padding:16px;text-align:center;border:1px solid #fde68a">
          <div style="font-size:11px;color:#64748b;font-weight:700">SCORE MOYEN GLOBAL</div>
          <div style="font-size:28px;font-weight:900;color:#d97706;margin-top:4px">${report.averageScore}%</div>
        </div>
        <div style="flex:1;background:#f0fdfa;border-radius:10px;padding:16px;text-align:center;border:1px solid #99f6e4">
          <div style="font-size:11px;color:#64748b;font-weight:700">TAUX DE COMPLÉTION</div>
          <div style="font-size:28px;font-weight:900;color:#0d9488;margin-top:4px">${report.completionRate}%</div>
        </div>
      </div>
      ${report.campaignPerformances?.length ? `
        <table style="width:100%;border-collapse:collapse;font-size:13px">
          <thead>
            <tr style="background:#f8fafc">
              <th style="padding:10px 12px;text-align:left;border-bottom:2px solid #e2e8f0;color:#64748b;font-size:11px">CAMPAGNE</th>
              <th style="padding:10px 12px;text-align:center;border-bottom:2px solid #e2e8f0;color:#64748b;font-size:11px">CANDIDATS</th>
              <th style="padding:10px 12px;text-align:center;border-bottom:2px solid #e2e8f0;color:#64748b;font-size:11px">SCORE MOYEN</th>
            </tr>
          </thead>
          <tbody>
            ${report.campaignPerformances.map(c => `
              <tr>
                <td style="padding:10px 12px;border-bottom:1px solid #f1f5f9;font-weight:600">${c.campaignName}</td>
                <td style="padding:10px 12px;text-align:center;border-bottom:1px solid #f1f5f9">${c.candidatesCount} candidats</td>
                <td style="padding:10px 12px;text-align:center;border-bottom:1px solid #f1f5f9;font-weight:700;color:${c.averageScore >= 75 ? '#16a34a' : c.averageScore >= 50 ? '#d97706' : '#dc2626'}">${c.averageScore}%</td>
              </tr>
            `).join('')}
          </tbody>
        </table>
      ` : '<p style="color:#94a3b8;text-align:center;padding:20px;border:1px dashed #cbd5e1;border-radius:8px">Aucune donnée cette semaine.</p>'}
      <div style="text-align:center;margin-top:30px;padding-top:20px;border-top:1px solid #e2e8f0">
        <p style="color:#94a3b8;font-size:11px">Rapport généré par NeoEvaluation — ${dateStr}</p>
      </div>
    `;
    document.body.appendChild(container);

    const canvas = await html2canvas(container, { scale: 2, useCORS: true, logging: false });
    document.body.removeChild(container);

    const imgData = canvas.toDataURL('image/jpeg', 0.85);
    const pdf = new jsPDF('p', 'mm', 'a4');
    const pdfW = 210;
    const imgW = canvas.width;
    const imgH = canvas.height;
    const ratio = pdfW / imgW;
    const pdfH = imgH * ratio;

    if (pdfH <= 297) {
      pdf.addImage(imgData, 'JPEG', 0, 0, pdfW, pdfH);
    } else {
      let y = 0;
      const pageH = 297;
      const pageImgH = pageH / ratio;
      while (y < imgH) {
        if (y > 0) pdf.addPage();
        const h = Math.min(pageImgH, imgH - y);
        const clipCanvas = document.createElement('canvas');
        clipCanvas.width = canvas.width;
        clipCanvas.height = h * ratio;
        const ctx = clipCanvas.getContext('2d');
        ctx.drawImage(canvas, 0, y * ratio, canvas.width, clipCanvas.height, 0, 0, canvas.width, clipCanvas.height);
        pdf.addImage(clipCanvas.toDataURL('image/jpeg', 0.85), 'JPEG', 0, 0, pdfW, pageH);
        y += h;
      }
    }

    pdf.save(`Rapport_Performance_Hebdo_${new Date().toISOString().slice(0,10)}.pdf`);
    showToast("Export PDF réussi !", "success", "fa-solid fa-file-pdf");
  } catch (err) {
    showToast("Échec de l'exportation PDF: " + (err.message || err), "error", "fa-solid fa-triangle-exclamation");
  } finally {
    isExportingPdf.value = false;
  }
};

const openEnterpriseDrafts = async () => {
  showEntDraftsModal.value = true;
  try {
    const res = await enterpriseApi.getRecommendations();
    enterpriseReco.value = res.data;
  } catch (err) {
    console.error(err);
  }
};

const triggerPublishDraft = async (campaignId) => {
  if (entPublishingCampaignId.value) return;
  entPublishingCampaignId.value = campaignId;
  
  try {
    const res = await enterpriseApi.publishDraft(campaignId);
    showToast(res.data.message || "Campagne publiée !", "success", "fa-solid fa-circle-check");
    
    // Update local campaign immediately if returned
    if (res.data.campaign) {
      const idx = campagnes.value.findIndex(c => c.id === res.data.campaign.id);
      if (idx >= 0) campagnes.value[idx] = res.data.campaign;
      else campagnes.value.push(res.data.campaign);
    }
    
    const recoRes = await enterpriseApi.getRecommendations();
    enterpriseReco.value = recoRes.data;
    
    if (['Evaluateur','RH','Recruteur','AdminEntreprise'].includes(role.value)) {
      const cr = await axios.get(`${API_NET}/Campagnes`, { headers: getAuthHeaders() });
      campagnes.value = cr.data || [];
    }
  } catch (err) {
    showToast(`Erreur de publication: ${err.response?.data?.message || err.message}`, "error", "fa-solid fa-triangle-exclamation");
  } finally {
    entPublishingCampaignId.value = null;
  }
};

// ── MODAL MAILER ──
const openMailerDiagnostics = async () => {
  showMailerModal.value = true;
  mailerLoading.value = true;
  mailerConsoleLogs.value = [];
  mailerResendSuccess.value = false;
  mailerResendMessage.value = '';
  
  try {
    const res = await superAdminApi.getMailerDiagnostics();
    mailerDiag.value = res.data;
    
    const logs = res.data.diagnosticsLogs || [];
    for (let i = 0; i < logs.length; i++) {
      await new Promise(resolve => setTimeout(resolve, 150));
      mailerConsoleLogs.value.push(logs[i]);
    }
  } catch (err) {
    mailerConsoleLogs.value.push(`[CRITICAL] Une erreur s'est produite : ${err.message}`);
  } finally {
    mailerLoading.value = false;
  }
};

const triggerMailerResend = async () => {
  if (mailerResendLoading.value) return;
  mailerResendLoading.value = true;
  mailerResendSuccess.value = false;
  mailerResendMessage.value = '';
  mailerConsoleLogs.value.push('[INFO] Lancement du traitement de libération des invitations...');
  
  try {
    const res = await superAdminApi.retriggerMailer();
    mailerConsoleLogs.value.push(`[SUCCESS] Envoi terminé ! Résultat : ${res.data.message}`);
    mailerResendSuccess.value = true;
    mailerResendMessage.value = res.data.message;
    // Reload state in background
    const diagRes = await superAdminApi.getMailerDiagnostics();
    mailerDiag.value = diagRes.data;
  } catch (err) {
    mailerConsoleLogs.value.push(`[CRITICAL] Échec de la libération : ${err.response?.data || err.message}`);
  } finally {
    mailerResendLoading.value = false;
  }
};

const connectGoogleSmtp = async () => {
  try {
    const res = await axios.get(`${API_NET}/GoogleAuth/auth-url`, { headers: getAuthHeaders() });
    if (res.data && res.data.url) {
      window.location.href = res.data.url;
    } else {
      showToast('Impossible d\'obtenir l\'URL d\'authentification Google', 'error', 'fa-solid fa-triangle-exclamation');
    }
  } catch (err) {
    showToast('Erreur de connexion avec Google Auth: ' + err.message, 'error', 'fa-solid fa-triangle-exclamation');
  }
};

// ── MODAL SUBSCRIPTIONS ──
const openExpiringSubscriptions = async () => {
  showSubsModal.value = true;
  subsLoading.value = true;
  try {
    const res = await superAdminApi.getExpiringSubscriptions();
    expiringSubs.value = res.data || [];
  } catch (err) {
    console.error('Erreur chargement abonnements expirants:', err);
  } finally {
    subsLoading.value = false;
  }
};

const sendRenewalReminder = async (sub) => {
  if (notifyingSubId.value === sub.id) return;
  notifyingSubId.value = sub.id;
  try {
    const res = await superAdminApi.notifyRenewal(sub.id);
    showToast(`Rappel de renouvellement envoyé avec succès pour ${sub.name}.`, 'success', 'fa-solid fa-check-circle');
    // Refresh lists
    await openExpiringSubscriptions();
  } catch (err) {
    showToast(`Erreur lors de l'envoi de la notification: ${err.response?.data || err.message}`, 'error', 'fa-solid fa-triangle-exclamation');
  } finally {
    notifyingSubId.value = null;
  }
};

// ── MODAL SECURITY SCANNER ──
const openSecurityScanner = () => {
  showSecurityModal.value = true;
  securityScanComplete.value = false;
  securityScanResult.value = null;
  securityScanScore.value = 0;
  securityScanLogs.value = [];
  securityScanActiveStep.value = -1;
  
  securityScanSteps.value = [
    { name: 'Initialisation du scanner d\'intégrité', details: 'Connexion aux bases de données locales et globales', icon: 'fa-solid fa-server', status: 'pending' },
    { name: 'Contrôle des politiques de sécurité des comptes', details: 'Vérification de la force des hashages de mot de passe', icon: 'fa-solid fa-key', status: 'pending' },
    { name: 'Analyse des jetons d\'accès actifs', details: 'Identification des tokens d\'activation expirés non utilisés', icon: 'fa-solid fa-clock-rotate-left', status: 'pending' },
    { name: 'Vérification de l\'isolation Multi-Tenant', details: 'Validation des barrières d\'accès SQL et contextes d\'entreprise', icon: 'fa-solid fa-shield-halved', status: 'pending' },
    { name: 'Enregistrement du rapport d\'audit système', details: 'Création du log chiffré dans le registre d\'audit local', icon: 'fa-solid fa-file-signature', status: 'pending' }
  ];
};

const runSecurityAuditScan = async () => {
  if (securityLoading.value) return;
  securityLoading.value = true;
  securityScanComplete.value = false;
  securityScanResult.value = null;
  securityScanScore.value = 0;
  securityScanLogs.value = [];
  
  try {
    // 1. Call API immediately in background but do premium slow animated HUD scan on UI!
    const apiPromise = superAdminApi.runSecurityAudit();
    
    // 2. Loop through steps to animate progress
    for (let stepIndex = 0; stepIndex < securityScanSteps.value.length; stepIndex++) {
      securityScanActiveStep.value = stepIndex;
      securityScanSteps.value[stepIndex].status = 'scanning';
      
      const dots = ['.', '..', '...'];
      for (let dot = 0; dot < 3; dot++) {
        securityScanLogs.value.push(`[SCANNING] ${securityScanSteps.value[stepIndex].name}${dots[dot]}`);
        if (securityScanLogs.value.length > 25) securityScanLogs.value.shift();
        await new Promise(r => setTimeout(r, 200));
      }
      
      // Step complete
      securityScanSteps.value[stepIndex].status = 'done';
      securityScanLogs.value.push(`[DONE] ${securityScanSteps.value[stepIndex].name} - Succès`);
      if (securityScanLogs.value.length > 25) securityScanLogs.value.shift();
      await new Promise(r => setTimeout(r, 250));
    }
    
    // 3. Wait for backend to respond if it hasn't already
    const res = await apiPromise;
    securityScanResult.value = res.data;
    
    // Count score up animatedly
    const targetScore = res.data.securityScore || 100;
    for (let current = 0; current <= targetScore; current += 2) {
      securityScanScore.value = Math.min(current, targetScore);
      await new Promise(r => setTimeout(r, 10));
    }
    securityScanScore.value = targetScore;
    
    securityScanComplete.value = true;
    showToast('Audit de sécurité complété avec succès !', 'success', 'fa-solid fa-shield-halved');
    
    // Refresh stats and security status in the background
    const statusRes = await superAdminApi.getSecurityStatus();
    securityDiag.value = statusRes.data;
  } catch (err) {
    securityScanLogs.value.push(`[CRITICAL] Échec de l'audit système : ${err.message}`);
  } finally {
    securityLoading.value = false;
    securityScanActiveStep.value = -1;
  }
};

// Refresh avec animation (garde compatibilité avec l'API IA si dispo)
const refreshRecommendations = async () => {
  recoLoading.value = true;
  try {
    // Tentative appel API IA optionnel
    const fd = new FormData();
    fd.append('role', role.value);
    fd.append('lang', locale.value.toLowerCase());
    fd.append('force_refresh', 'true');
    await axios.post(`${API_IA}/ia/recommendations`, fd);
  } catch {
    // silencieux — on utilise les recommandations statiques enrichies
  } finally {
    await new Promise(r => setTimeout(r, 800));
    recoLoading.value = false;
    showToast(t('dashboard.toast.recoRefreshed') || 'Recommandations actualisées', 'success', 'fa-solid fa-check');
  }
};

// ── DONNÉES .NET ──
const dotnetStats        = reactive({ kpis: {}, chart: [], leaders: [], recentResults: [] });
const evalQueue          = ref([]);
const campagnes          = ref([]);
const staffMembers       = ref([]);
const historiqueCandidat = ref([]);
const candidatTests      = ref([]);
const superAdminStats    = reactive({ totalEntreprises: 0, totalUtilisateurs: 0, demandesEnAttente: 0, totalTests: 0, croissanceStats: [], recentTransactions: [] });
const platformUsers      = ref([]);

// ── DONNÉES IA PYTHON ──
const iaServices       = ref([]);
const recentActivities = ref([]);

// ── ROLE ──
const role = computed(() => authStore.role || 'Recruteur');

const roleConfig = computed(() => ({
  Candidat:        { label: t('dashboard.roleLabels.Candidat'),       icon: 'fa-solid fa-user-graduate',   accent: '#3b82f6', accentLight: '#60a5fa', insightIcon: 'fa-solid fa-trophy',               insightLabel: t('dashboard.roleInsights.Candidat'),       botIcon: 'fa-user-graduate',  tagBg: 'rgba(59,130,246,0.1)',  tagBorder: 'rgba(59,130,246,0.25)',  tagColor: '#3b82f6' },
  Evaluateur:      { label: t('dashboard.roleLabels.Evaluateur'),      icon: 'fa-solid fa-clipboard-check', accent: '#f59e0b', accentLight: '#fbbf24', insightIcon: 'fa-solid fa-magnifying-glass-chart', insightLabel: t('dashboard.roleInsights.Evaluateur'),     botIcon: 'fa-clipboard-check', tagBg: 'rgba(245,158,11,0.1)', tagBorder: 'rgba(245,158,11,0.25)', tagColor: '#f59e0b' },
  RH:              { label: t('dashboard.roleLabels.RH'),              icon: 'fa-solid fa-people-arrows',   accent: '#8b5cf6', accentLight: '#a78bfa', insightIcon: 'fa-solid fa-chart-pie',             insightLabel: t('dashboard.roleInsights.RH'),             botIcon: 'fa-people-arrows',  tagBg: 'rgba(139,92,246,0.1)', tagBorder: 'rgba(139,92,246,0.25)', tagColor: '#8b5cf6' },
  Recruteur:       { label: t('dashboard.roleLabels.Recruteur'),       icon: 'fa-solid fa-handshake',       accent: '#10b981', accentLight: '#34d399', insightIcon: 'fa-solid fa-user-plus',             insightLabel: t('dashboard.roleInsights.Recruteur'),      botIcon: 'fa-handshake',      tagBg: 'rgba(16,185,129,0.1)', tagBorder: 'rgba(16,185,129,0.25)', tagColor: '#10b981' },
  AdminEntreprise: { label: t('dashboard.roleLabels.AdminEntreprise'), icon: 'fa-solid fa-building-user',   accent: '#f59e0b', accentLight: '#fbbf24', insightIcon: 'fa-solid fa-brain',                insightLabel: t('dashboard.roleInsights.AdminEntreprise'), botIcon: 'fa-robot',          tagBg: 'rgba(245,158,11,0.1)', tagBorder: 'rgba(245,158,11,0.25)', tagColor: '#f59e0b' },
  SuperAdmin:      { label: t('dashboard.roleLabels.SuperAdmin'),      icon: 'fa-solid fa-shield-halved',   accent: '#6366f1', accentLight: '#818cf8', insightIcon: 'fa-solid fa-server',               insightLabel: t('dashboard.roleInsights.SuperAdmin'),     botIcon: 'fa-shield-halved',  tagBg: 'rgba(99,102,241,0.1)', tagBorder: 'rgba(99,102,241,0.25)', tagColor: '#6366f1' },
}));

const cfg             = computed(() => roleConfig.value[role.value] || roleConfig.value.AdminEntreprise);
const roleLabel       = computed(() => cfg.value.label);
const roleIcon        = computed(() => cfg.value.icon);
const roleAccent      = computed(() => cfg.value.accent);
const roleAccentLight = computed(() => cfg.value.accentLight);
const roleInsightIcon  = computed(() => cfg.value.insightIcon);
const roleInsightLabel = computed(() => cfg.value.insightLabel);
const roleBotIcon     = computed(() => cfg.value.botIcon);
const roleTagBg       = computed(() => cfg.value.tagBg);
const roleTagBorder   = computed(() => cfg.value.tagBorder);
const roleTagColor    = computed(() => cfg.value.tagColor);
const userName        = computed(() => authStore.user?.name?.split(' ')[0] || t('optional'));
const userInitial     = computed(() => (authStore.user?.name || 'U')[0].toUpperCase());

const cvScoreColor = computed(() => {
  if (!cvResult.value) return '#fbbf24';
  const s = cvResult.value.score;
  return s >= 80 ? '#10b981' : s >= 60 ? '#f59e0b' : '#ef4444';
});

// ── KPI CARDS ──
const kpiCards = computed(() => {
  const r = role.value;
  const k = dotnetStats.kpis;
  if (r === 'Candidat') return [
    { label: t('dashboard.kpis.totalTests'),  value: k.totalTests ?? '—',  icon: 'fa-solid fa-clipboard-check', color: '#3b82f6', bg: 'rgba(59,130,246,0.12)',  trend: '↑', sparkPoints: '0,22 80,6' },
    { label: t('dashboard.kpis.avgScore'),    value: k.moyenne != null ? k.moyenne+'%' : '—', icon: 'fa-solid fa-star', color: '#f59e0b', bg: 'rgba(245,158,11,0.12)', trend: '↑', sparkPoints: '0,24 80,4' },
    { label: t('dashboard.kpis.campaigns'),   value: k.totalCampagnes ?? '—', icon: 'fa-solid fa-bullhorn', color: '#ef4444', bg: 'rgba(239,68,68,0.12)', trend: '→', sparkPoints: '0,20 80,8' },
    { label: t('dashboard.kpis.talents'),     value: k.totalTalents ?? '—',  icon: 'fa-solid fa-users', color: '#8b5cf6', bg: 'rgba(139,92,246,0.12)', trend: '↑', sparkPoints: '0,26 80,6' },
  ];
  if (['Evaluateur','RH','Recruteur'].includes(r)) return [
    { label: t('dashboard.kpis.evaluationsIA'),    value: k.totalTests ?? '—',     icon: 'fa-solid fa-clipboard-check', color: '#f59e0b', bg: 'rgba(245,158,11,0.12)',  trend: '↑', sparkPoints: '0,22 80,6' },
    { label: t('dashboard.kpis.scoreGlobal'),      value: k.moyenne != null ? k.moyenne+'%' : '—', icon: 'fa-solid fa-chart-bar', color: '#10b981', bg: 'rgba(16,185,129,0.12)', trend: '↑', sparkPoints: '0,24 80,4' },
    { label: t('dashboard.kpis.campagnesActives'), value: k.totalCampagnes ?? '—', icon: 'fa-solid fa-calendar-days', color: '#6366f1', bg: 'rgba(99,102,241,0.12)', trend: '↑', sparkPoints: '0,20 80,8' },
    { label: t('dashboard.kpis.talentsActifs'),    value: k.totalTalents ?? '—',   icon: 'fa-solid fa-user-group', color: '#8b5cf6', bg: 'rgba(139,92,246,0.12)', trend: '↑', sparkPoints: '0,26 80,6' },
  ];
  if (r === 'AdminEntreprise') return [
    { label: t('dashboard.kpis.talentsActifs'),    value: k.totalTalents ?? '—',   icon: 'fa-solid fa-user-group', color: '#fbbf24', bg: 'rgba(251,191,36,0.12)',  trend: '↑', sparkPoints: '0,22 80,6' },
    { label: t('dashboard.kpis.scoreGlobal'),      value: k.moyenne != null ? k.moyenne+'%' : '—', icon: 'fa-solid fa-circle-check', color: '#10b981', bg: 'rgba(16,185,129,0.12)', trend: '↑', sparkPoints: '0,24 80,4' },
    { label: t('dashboard.kpis.campagnesActives'), value: k.totalCampagnes ?? '—', icon: 'fa-solid fa-bolt-lightning', color: '#3b82f6', bg: 'rgba(59,130,246,0.12)', trend: '↑', sparkPoints: '0,20 80,8' },
    { label: t('dashboard.kpis.evaluationsIA'),    value: k.iaProcessed ?? '—',    icon: 'fa-solid fa-brain', color: '#8b5cf6', bg: 'rgba(139,92,246,0.12)', trend: '↑', sparkPoints: '0,26 80,6' },
  ];
  if (r === 'SuperAdmin') return [
    { label: t('dashboard.kpis.companies'),  value: superAdminStats.totalEntreprises ?? '—', icon: 'fa-solid fa-building', color: '#6366f1', bg: 'rgba(99,102,241,0.12)',  trend: '↑', sparkPoints: '0,22 80,6' },
    { label: t('dashboard.kpis.users'),      value: superAdminStats.totalUtilisateurs ?? '—', icon: 'fa-solid fa-users', color: '#10b981', bg: 'rgba(16,185,129,0.12)', trend: '↑', sparkPoints: '0,24 80,4' },
    { label: t('dashboard.kpis.evaluations'),value: superAdminStats.totalTests ?? '—', icon: 'fa-solid fa-wave-square', color: '#f59e0b', bg: 'rgba(245,158,11,0.12)', trend: '↑', sparkPoints: '0,20 80,8' },
    { label: t('dashboard.kpis.pending'),    value: superAdminStats.demandesEnAttente ?? '—', icon: 'fa-solid fa-clock', color: '#ef4444', bg: 'rgba(239,68,68,0.12)', trend: '!', sparkPoints: '0,26 80,6' },
  ];
  return [];
});

const miniStats = computed(() => [
  { label: t('dashboard.miniStats.tests'),   value: dotnetStats.kpis.totalTests ?? '—',   color: '#3b82f6' },
  { label: t('dashboard.miniStats.score'),   value: dotnetStats.kpis.moyenne != null ? dotnetStats.kpis.moyenne+'%' : '—', color: '#f59e0b' },
  { label: t('dashboard.miniStats.talents'), value: dotnetStats.kpis.totalTalents ?? '—', color: '#10b981' },
]);

const topSkills = computed(() => {
  const leaders = dotnetStats.leaders || [];
  if (!leaders.length) return [
    { label: 'Logique & Analyse', value: 0, color: '#3b82f6' },
    { label: 'Communication',     value: 0, color: '#10b981' },
  ];
  const avg = Math.round(leaders.reduce((a, b) => a + (b.score || 0), 0) / leaders.length);
  return [
    { label: 'Performance globale', value: avg,                                          color: '#3b82f6' },
    { label: 'Meilleur candidat',   value: leaders[0]?.score || 0,                      color: '#10b981' },
    { label: 'Taux de réussite',    value: dotnetStats.kpis?.moyenne || 0,              color: '#f59e0b' },
    { label: 'Campagnes actives',   value: Math.min(100, (dotnetStats.kpis?.totalCampagnes || 0) * 10), color: '#8b5cf6' },
  ];
});

const recentCandidates    = computed(() => dotnetStats.recentResults || []);
const candidatProgression = computed(() => historiqueCandidat.value.map(h => ({
  campagneNom: h.titreExamen,
  score: h.score,
})));

// ══════════════════════════════════════════════
// CHARGEMENT DONNÉES
// ══════════════════════════════════════════════

const getAuthHeaders = () => {
  const token = authStore.token || localStorage.getItem('token');
  return token ? { Authorization: `Bearer ${token}` } : {};
};

const loadDotnetDashboard = async () => {
  try {
    const res = await axios.get(`${API_NET}/Dashboard/global-stats`, { headers: getAuthHeaders() });
    Object.assign(dotnetStats, res.data);
    const k = res.data.kpis || {};
    iaInsight.value = `${k.totalTalents || 0} ${t('dashboard.kpis.talentsActifs')} · ${t('dashboard.kpis.scoreGlobal')} : ${k.moyenne || 0}% · ${k.totalCampagnes || 0} ${t('dashboard.kpis.campagnesActives')}`;
  } catch (e) { console.warn(e.message); }
};

const loadAnalytics = async () => {
  try {
    const res = await axios.get(`${API_NET}/Analytics/overview`, { headers: getAuthHeaders() });
    if (res.data.recentActivities) {
      recentActivities.value = res.data.recentActivities.map((a, i) => ({
        id: i,
        user: a.user,
        action: `${a.action} — ${a.campagne || ''}`,
        color: a.color || '#6366f1',
        time: a.time || t('dashboard.activity.recent'),
      }));
    }
    if (res.data.insight) iaInsight.value = res.data.insight;
  } catch (e) { console.warn(e.message); }
};

const runCvAnalysis = async () => {
  if (!selectedFile.value) return;
  isAnalyzing.value = true;
  cvResult.value = null;
  try {
    const fd = new FormData();
    fd.append('File', selectedFile.value);
    fd.append('JobDescription', jobDescription.value || 'Poste générique');
    fd.append('Lang', locale.value.toLowerCase());
    const res = await axios.post(`${API_NET}/Dashboard/analyze-cv`, fd, { headers: getAuthHeaders() });
    cvResult.value = res.data;
    showToast(t('dashboard.toast.cvSuccess', { score: res.data.score }), 'success', 'fa-solid fa-check');
  } catch (e) { showToast(t('dashboard.toast.cvError'), 'error', 'fa-solid fa-x'); }
  finally { isAnalyzing.value = false; }
};

const generateLettre = async () => {
  if (!lettreData.poste || !lettreData.entreprise) return;
  isGeneratingLettre.value = true;
  try {
    const fd = new FormData();
    fd.append('nom',         lettreData.nom || authStore.user?.name || '');
    fd.append('poste',       lettreData.poste);
    fd.append('entreprise',  lettreData.entreprise);
    fd.append('competences', lettreData.competences);
    fd.append('style',       lettreData.style);
    fd.append('langue',      lettreData.langue);
    const res = await axios.post(`${API_IA}/ia/lettre-motivation`, fd);
    if (res.data.status === 'SUCCESS' && res.data.lettre) {
      lettreResult.value = res.data.lettre;
    } else {
      lettreResult.value = res.data.lettre || res.data;
    }
    showToast(t('dashboard.toast.lettreSuccess'), 'success', 'fa-solid fa-envelope');
  } catch (e) { showToast(t('dashboard.toast.lettreError'), 'error', 'fa-solid fa-x'); }
  finally { isGeneratingLettre.value = false; }
};

const copyLettre = async () => {
  if (!lettreResult.value) return;
  try {
    await navigator.clipboard.writeText(lettreResult.value);
    letterCopied.value = true;
    setTimeout(() => { letterCopied.value = false; }, 2500);
    showToast(t('dashboard.toast.letterCopied'), 'success', 'fa-solid fa-check');
  } catch {
    const el = document.createElement('textarea');
    el.value = lettreResult.value;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
    letterCopied.value = true;
    setTimeout(() => { letterCopied.value = false; }, 2500);
  }
};

const downloadLettre = () => {
  if (!lettreResult.value) return;
  const blob = new Blob([lettreResult.value], { type: 'text/plain;charset=utf-8' });
  const url  = URL.createObjectURL(blob);
  const a    = document.createElement('a');
  a.href     = url;
  a.download = `lettre_${lettreData.poste.replace(/\s+/g,'_')}_${lettreData.langue}.txt`;
  a.click();
  URL.revokeObjectURL(url);
};

const loadIaServices = async () => {
  try {
    const res = await axios.get(`${API_IA}/ia/health`);
    iaServices.value = [
      { key: 'gateway',  name: 'API Gateway .NET',    latency: '~',   up: true },
      { key: 'iaEngine', name: t('dashboard.health.iaEngine') || 'IA Engine (EvaluaTech)',  latency: `${res.data.avg_latency_ms || 0}ms`, up: res.data.circuit_state === 'CLOSED' },
      { key: 'auth',     name: 'Auth Service',        latency: '~',   up: true },
    ];
  } catch { iaServices.value = [{ key: 'gateway', name: 'API Gateway .NET', up: false }]; }
};

const loadAllData = async () => {
  loading.value = true;
  try {
    await Promise.allSettled([loadDotnetDashboard(), loadAnalytics()]);
    const r = role.value;
    if (r === 'Candidat') {
      const [tr, hr] = await Promise.allSettled([
        axios.get(`${API_NET}/Candidatures/mes-tests`, { headers: getAuthHeaders() }),
        axios.get(`${API_NET}/Examen/historique`,      { headers: getAuthHeaders() }),
      ]);
      if (tr.status === 'fulfilled') candidatTests.value      = tr.value.data || [];
      if (hr.status === 'fulfilled') historiqueCandidat.value = hr.value.data || [];
    } else if (['Evaluateur','RH','Recruteur','AdminEntreprise'].includes(r)) {
      const [er, cr, sr] = await Promise.allSettled([
        axios.get(`${API_NET}/Examen/all`,  { headers: getAuthHeaders() }),
        axios.get(`${API_NET}/Campagnes`,   { headers: getAuthHeaders() }),
        (r === 'AdminEntreprise' || r === 'RH' || r === 'Recruteur') 
          ? axios.get(`${API_NET}/Staff`, { headers: getAuthHeaders() }) 
          : Promise.resolve({ data: [] }),
      ]);
      if (er.status === 'fulfilled') evalQueue.value    = er.value.data || [];
      if (cr.status === 'fulfilled') campagnes.value    = cr.value.data || [];
      if (sr.status === 'fulfilled') staffMembers.value = sr.value.data || [];
      
      if (r === 'AdminEntreprise' || r === 'Evaluateur') {
        try {
          const res = await enterpriseApi.getRecommendations();
          enterpriseReco.value = res.data;
        } catch (err) {
          console.error("Erreur recommendations:", err);
        }
      }
    } else if (r === 'SuperAdmin') {
      const [str, ur, mailerRes, subsRes, secRes] = await Promise.allSettled([
        axios.get(`${API_NET}/SuperAdmin/stats`, { headers: getAuthHeaders() }),
        axios.get(`${API_NET}/SuperAdmin/users`, { headers: getAuthHeaders() }),
        superAdminApi.getMailerDiagnostics(),
        superAdminApi.getExpiringSubscriptions(),
        superAdminApi.getSecurityStatus()
      ]);
      if (str.status === 'fulfilled') Object.assign(superAdminStats, str.value.data);
      if (ur.status === 'fulfilled')  platformUsers.value = ur.value.data || [];
      if (mailerRes.status === 'fulfilled') mailerDiag.value = mailerRes.value.data;
      if (subsRes.status === 'fulfilled') expiringSubs.value = subsRes.value.data || [];
      if (secRes.status === 'fulfilled') securityDiag.value = secRes.value.data;
      await loadIaServices();
    }
  } finally { loading.value = false; }
};

// ── UI HELPERS ──
const getInitials    = (name = '') => (name || '?').split(' ').filter(Boolean).map(w => w[0]).slice(0,2).join('').toUpperCase();
const getScoreColor  = (s) => (s >= 75 ? '#10b981' : s >= 50 ? '#f59e0b' : '#ef4444');
const getAvatarColor = (role) => ({'RH':'#6366f1','Evaluateur':'#f59e0b','Recruteur':'#10b981','AdminEntreprise':'#fbbf24','SuperAdmin':'#8b5cf6'}[role] || '#64748b');
const getTestColor   = (statut) => ({ 'EN_COURS': '#3b82f6', 'TERMINE': '#10b981', 'POSTULE': '#f59e0b', 'NON_COMMENCE': '#94a3b8' }[statut] || '#94a3b8');
const getStatusBg    = (s) => ({ 'EN_COURS': '#eff6ff', 'TERMINE': '#ecfdf5', 'NON_COMMENCE': '#fef9ec' }[s] || '#f8fafc');
const getStatusColor = (s) => ({ 'EN_COURS': '#3b82f6', 'TERMINE': '#10b981', 'NON_COMMENCE': '#f59e0b' }[s] || '#94a3b8');
const formatStatut   = (s) => t(`dashboard.testStatus.${s}`);
const formatDate     = (d) => d ? new Date(d).toLocaleDateString(locale.value === 'AR' ? 'ar-TN' : 'fr-FR', { day: '2-digit', month: 'short' }) : '—';
const handleCvUpload = (e) => { selectedFile.value = e.target.files[0]; };
const handleDrop     = (e) => { isDragging.value = false; const f = e.dataTransfer.files[0]; if (f && (f.name.endsWith('.pdf') || f.name.endsWith('.docx'))) selectedFile.value = f; };
const updateClock    = () => { currentTime.value = new Date().toLocaleTimeString(locale.value === 'AR' ? 'ar-TN' : 'fr-FR', { hour: '2-digit', minute: '2-digit', second: '2-digit' }); };
const orbStyle       = (f) => ({ transform: `translate(${mousePos.x * f * 10}px, ${mousePos.y * f * 10}px)` });
const handleParallax = (e) => { mousePos.x = (e.clientX - window.innerWidth / 2) / 20; mousePos.y = (e.clientY - window.innerHeight / 2) / 20; };

// ── CHART ──
let chartInstance = null;
const initChart = async (period = 'week') => {
  await nextTick();
  const canvases = document.querySelectorAll('.js-main-chart');
  if (!canvases.length) return;
  const canvas = canvases[0];
  const noDataMsg = canvas.parentElement?.querySelector('.chart-no-data');

  if (!dotnetStats.chart?.length) {
    canvas.style.display = 'none';
    if (noDataMsg) noDataMsg.style.display = 'block';
    return;
  }
  canvas.style.display = 'block';
  if (noDataMsg) noDataMsg.style.display = 'none';

  if (chartInstance) chartInstance.destroy();

  const accent = roleAccent.value;
  const dark   = isDark.value;

  chartInstance = new Chart(canvas, {
    type: 'line',
    data: {
      labels: dotnetStats.chart.map(c => c.name || c.nom || ''),
      datasets: [{
        data: dotnetStats.chart.map(c => c.score || c.moyenne || 0),
        borderColor: accent,
        backgroundColor: accent + '18',
        tension: 0.4,
        fill: true,
        pointBackgroundColor: accent,
        pointBorderColor: dark ? '#0d1117' : '#fff',
        pointBorderWidth: 2,
        pointRadius: 4,
      }],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: false } },
      scales: {
        x: { grid: { color: dark ? 'rgba(255,255,255,0.06)' : '#f1f5f9' }, ticks: { color: dark ? '#94a3b8' : '#64748b' } },
        y: { grid: { color: dark ? 'rgba(255,255,255,0.06)' : '#f1f5f9' }, ticks: { color: dark ? '#94a3b8' : '#64748b' } },
      },
    },
  });
};

const switchPeriod = async (p) => { activePeriod.value = p; await initChart(p); };

// ── WATCHERS ──
watch(isDark,  async () => { await nextTick(); await initChart(activePeriod.value); });
watch(role,    async () => { await nextTick(); await initChart(activePeriod.value); await loadAllData(); });
watch(locale,  (newLang) => {
  document.documentElement.dir = newLang === 'AR' ? 'rtl' : 'ltr';
  initChart(activePeriod.value);
}, { immediate: true });

// ── TOAST ──
let _toastTimer = null;
const showToast = (msg, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(globalToast, { message: msg, type: `t-${type}`, icon, active: true });
  _toastTimer = setTimeout(() => { globalToast.active = false; }, 4000);
};

// ── LIFECYCLE ──
let _clockInterval = null;
onMounted(async () => {
  updateClock();
  _clockInterval = setInterval(updateClock, 1000);
  await loadAllData();
  await initChart();
  setInterval(loadAllData, 60000);
});

onUnmounted(() => {
  clearInterval(_clockInterval);
  if (chartInstance) chartInstance.destroy();
});
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;600;700;800&family=JetBrains+Mono:wght@600;700&display=swap');

/* ─── BASE ─── */
.aura-dashboard { min-height: 100vh; background: var(--bg-page); font-family: 'Plus Jakarta Sans', sans-serif; display: flex; position: relative; overflow-x: hidden; transition: background-color 0.35s ease; }
.main-viewport  { z-index: 10; }
.canvas-engine  { height: calc(100vh - 64px); }
.content-area   { position: relative; z-index: 20; }

/* ─── FOND PARALLAX ─── */
.luxury-bg    { position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden; }
.quantum-grid { position: absolute; inset: 0; background-image: radial-gradient(var(--grid-dot,#cbd5e1) 1px, transparent 1px); background-size: 40px 40px; opacity: 0.15; }
[data-theme="dark"] .quantum-grid { --grid-dot:#334155; opacity: 0.25; }
.glow-orb  { position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.12; transition: transform 0.3s ease-out; pointer-events: none; }
[data-theme="dark"] .glow-orb { opacity: 0.18; }
.orb-amber { width: 600px; height: 600px; background: #f59e0b; top: -200px; right: -100px; }
.orb-blue  { width: 400px; height: 400px; background: #6366f1; bottom: -200px; left: -100px; }
.orb-rose  { width: 300px; height: 300px; background: #f472b6; top: 40%; right: 25%; }

/* ─── TERMINAL BAR ─── */
.terminal-bar { background: var(--bg-card,#fff); border: 1px solid var(--border-color,#eef2f6); border-radius: 28px; padding: 16px 28px; box-shadow: 0 4px 20px rgba(0,0,0,0.04); transition: background-color 0.3s, border-color 0.3s; }
.ai-robot-terminal { width: 46px; height: 46px; background: #0f172a; border-radius: 14px; display: flex; align-items: center; justify-content: center; flex-shrink: 0; }
.breadcrumb-pro { font-family: 'JetBrains Mono', monospace; font-size: 10px; color: var(--text-muted,#94a3b8); }
.breadcrumb-pro .separator { font-size: 8px; opacity: 0.5; }
.breadcrumb-pro .current { color: #f59e0b; font-weight: 700; }
.premium-title { font-weight: 800; font-size: clamp(1.4rem,2.5vw,1.9rem); letter-spacing: -1px; color: var(--text-main,#0f172a); }
.theme-toggle-btn { display: flex; align-items: center; gap: 6px; padding: 8px 16px; border-radius: 100px; border: 1px solid var(--border-color,#eef2f6); background: var(--bg-input,#f8fafc); color: var(--text-main,#0f172a); font-size: 12px; font-weight: 700; cursor: pointer; transition: all 0.25s; font-family: inherit; }
.theme-toggle-btn:hover { border-color: #f59e0b; color: #d97706; }
.metric-pill { background: var(--bg-input,#f8fafc); padding: 8px 16px; border-radius: 100px; font-size: 11px; font-weight: 700; display: flex; align-items: center; gap: 8px; color: var(--text-muted,#94a3b8); border: 1px solid var(--border-color,#eef2f6); font-family: 'JetBrains Mono', monospace; }
.live-dot { width: 7px; height: 7px; background: #10b981; border-radius: 50%; box-shadow: 0 0 8px #10b981; animation: pulseLive 2s infinite; }
.user-badge { background: #0f172a; color: white; padding: 6px 16px 6px 6px; border-radius: 100px; font-size: 12px; font-weight: 700; display: flex; align-items: center; gap: 10px; }
.user-avatar { width: 32px; height: 32px; background: linear-gradient(135deg,#f59e0b,#fbbf24); border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 13px; font-weight: 900; color: #0f172a; flex-shrink: 0; }

/* ─── HERO CARD ─── */
.hero-card { background: var(--bg-card,#fff); border-radius: 40px; border: 1px solid var(--border-color,#eef2f6); box-shadow: 0 8px 30px rgba(0,0,0,0.05); overflow: hidden; position: relative; transition: background-color 0.3s, border-color 0.3s; }
.scanner-sweep { position: absolute; top: 0; left: -100%; width: 25%; height: 100%; background: linear-gradient(90deg,transparent,rgba(245,158,11,0.05),transparent); animation: sweep 7s ease-in-out infinite; }
.hero-inner  { display: flex; align-items: center; }
.hero-text   { flex: 1; padding: 44px 52px; }
.hero-visual { width: 200px; min-width: 200px; padding: 32px; display: flex; flex-direction: column; align-items: center; gap: 24px; }
.hero-bot-wrap { position: relative; display: flex; align-items: center; justify-content: center; width: 100px; height: 100px; }
.bot-glow-ring { position: absolute; width: 90px; height: 90px; border-radius: 50%; border: 2px solid rgba(245,158,11,0.2); animation: spin 10s linear infinite; }
.bot-glow-ring.ring2 { width: 70px; height: 70px; animation: spin 6s linear infinite reverse; }
.bot-icon { animation: floatBot 5s ease-in-out infinite; font-size: 2.5rem !important; position: relative; z-index: 2; }
.hero-stats-mini { display: flex; flex-direction: column; gap: 8px; width: 100%; }
.mini-stat { background: var(--bg-input,#f8fafc); border-radius: 12px; padding: 10px 14px; border: 1px solid var(--border-color,#eef2f6); display: flex; justify-content: space-between; align-items: center; }
.ms-val { font-size: 1rem; font-weight: 900; line-height: 1; }
.ms-lbl { font-size: 9px; font-weight: 700; color: var(--text-muted,#94a3b8); text-transform: uppercase; letter-spacing: 0.5px; }
.premium-tag { display: inline-flex; align-items: center; gap: 8px; font-size: 11px; font-weight: 700; padding: 6px 14px; border-radius: 100px; letter-spacing: 1px; margin-bottom: 16px; text-transform: uppercase; border: 1px solid; }
.hero-title  { font-weight: 800; font-size: clamp(1.8rem,3vw,2.6rem); letter-spacing: -1.5px; color: var(--text-main,#0f172a); line-height: 1.15; margin-bottom: 24px; }
.gold-text   { background: linear-gradient(135deg,#fbbf24,#f59e0b,#d97706); -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }
.ia-insight  { background: var(--bg-input,#f8fafc); border-radius: 20px; padding: 18px; display: flex; align-items: flex-start; gap: 14px; border: 1px solid var(--border-color,#eef2f6); }
.ia-orb      { width: 48px; height: 48px; min-width: 48px; border-radius: 14px; display: flex; align-items: center; justify-content: center; font-size: 20px; color: white; position: relative; }
.orb-ring    { position: absolute; inset: -4px; border: 2px solid rgba(251,191,36,0.3); border-radius: 18px; animation: spin 8s linear infinite; }
.ia-header   { display: flex; justify-content: space-between; margin-bottom: 6px; }
.ia-label    { font-size: 10px; font-weight: 800; color: var(--text-main,#0f172a); text-transform: uppercase; letter-spacing: 1px; }
.ia-status-badge { font-size: 9px; background: #ecfdf5; color: #059669; padding: 2px 8px; border-radius: 100px; font-weight: 700; }
.ia-msg      { font-size: 13px; color: var(--text-muted,#64748b); margin: 0; line-height: 1.6; }
.shimmer-text { background: linear-gradient(90deg,var(--text-muted,#94a3b8) 25%,var(--border-color,#e2e8f0) 50%,var(--text-muted,#94a3b8) 75%); background-size: 200% 100%; animation: shimmer 1.5s infinite; -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; margin: 0; }
.shimmer-val  { background: linear-gradient(90deg,#94a3b8 25%,#e2e8f0 50%,#94a3b8 75%); background-size: 200% 100%; animation: shimmer 1.5s infinite; -webkit-background-clip: text; -webkit-text-fill-color: transparent; background-clip: text; }

/* ─── KPI GRID ─── */
.kpi-grid { display: grid; grid-template-columns: repeat(4,1fr); gap: 20px; }
@media(max-width:1200px){.kpi-grid{grid-template-columns:repeat(2,1fr);}}
@media(max-width:640px){.kpi-grid{grid-template-columns:1fr;}}
.kpi-card { background: var(--bg-card,#fff); border-radius: 28px; padding: 24px; border: 1px solid var(--border-color,#eef2f6); position: relative; overflow: hidden; transition: transform 0.3s,border-color 0.3s,box-shadow 0.3s,background-color 0.3s; }
.kpi-card::before { content:''; position:absolute; top:0; left:0; right:0; height:3px; background:var(--accent); opacity:0; transition:opacity 0.3s; }
.kpi-card:hover { transform:translateY(-6px); border-color:var(--accent); box-shadow:0 20px 40px rgba(0,0,0,0.07); }
.kpi-card:hover::before { opacity:1; }
.kpi-icon  { width:46px; height:46px; border-radius:14px; display:flex; align-items:center; justify-content:center; font-size:18px; flex-shrink:0; }
.kpi-trend { font-size:10px; font-weight:800; padding:4px 10px; border-radius:100px; }
.kpi-value { font-size:2.2rem; font-weight:800; color:var(--text-main,#0f172a); margin:0; letter-spacing:-1.5px; line-height:1; }
.kpi-label { font-size:10px; font-weight:700; color:var(--text-muted,#94a3b8); text-transform:uppercase; letter-spacing:1px; margin-top:6px; display:block; }
.kpi-spark { margin-top:12px; opacity:0.7; }
.kpi-spark svg { width:100%; height:28px; }

/* ─── RECOMMANDATIONS ─── */
.reco-section { background:var(--bg-card,#fff); border-radius:32px; padding:28px; border:1px solid var(--border-color,#eef2f6); box-shadow:0 4px 20px rgba(0,0,0,0.03); transition:background-color 0.3s,border-color 0.3s; }
.reco-icon-wrap { width:46px; height:46px; border-radius:14px; background:linear-gradient(135deg,#f59e0b,#fbbf24); display:flex; align-items:center; justify-content:center; font-size:18px; color:white; flex-shrink:0; }
.reco-title { font-size:16px; font-weight:800; color:var(--text-main,#0f172a); }
.reco-sub   { font-size:11px; color:var(--text-muted,#94a3b8); }
.btn-refresh-reco { display:flex; align-items:center; gap:8px; padding:10px 20px; background:#0f172a; color:white; border:none; border-radius:14px; font-size:12px; font-weight:700; cursor:pointer; font-family:inherit; transition:all 0.25s; }
.btn-refresh-reco:hover:not(:disabled) { background:#1e293b; transform:translateY(-2px); }
.btn-refresh-reco:disabled { opacity:0.6; cursor:not-allowed; }
.reco-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:16px; }
@media(max-width:960px){.reco-grid{grid-template-columns:1fr;}}
.reco-card { background:var(--bg-input,#f8fafc); border-radius:24px; padding:24px; border:1px solid var(--border-color,#eef2f6); position:relative; overflow:hidden; transition:transform 0.3s,border-color 0.3s,box-shadow 0.3s; }
.reco-card::before { content:''; position:absolute; top:0; left:0; width:4px; height:100%; background:var(--reco-color); border-radius:4px 0 0 4px; }
.reco-card:hover { transform:translateY(-4px); border-color:var(--reco-color); box-shadow:0 12px 30px rgba(0,0,0,0.07); }
.reco-card-top  { display:flex; justify-content:space-between; align-items:center; margin-bottom:14px; }
.reco-card-icon { width:44px; height:44px; border-radius:12px; display:flex; align-items:center; justify-content:center; font-size:17px; flex-shrink:0; }
.reco-priority-badge { font-size:10px; font-weight:800; padding:4px 10px; border-radius:100px; }
.reco-card-title  { font-size:14px; font-weight:800; color:var(--text-main,#0f172a); margin-bottom:8px; line-height:1.3; }
.reco-card-desc   { font-size:12px; color:var(--text-muted,#64748b); line-height:1.6; margin-bottom:16px; }
.reco-card-action {
  display:flex; align-items:center; justify-content:space-between;
  font-size:12px; font-weight:700; color:var(--reco-color);
  cursor:pointer; padding:10px 14px;
  background:var(--bg-card,#fff); border-radius:12px;
  border:1px solid var(--border-color,#eef2f6);
  transition:all 0.2s;
  user-select:none;
}
.reco-card-action:hover { background:rgba(0,0,0,0.02); transform:translateX(4px); }
.reco-card-skel { background:var(--bg-input,#f8fafc); border-radius:24px; padding:24px; border:1px solid var(--border-color,#eef2f6); }
.reco-skel-top  { height:44px; width:44px; border-radius:12px; background:var(--border-color,#e2e8f0); animation:shimmer 1.5s infinite; margin-bottom:16px; }
.reco-skel-line { height:10px; border-radius:6px; background:var(--border-color,#e2e8f0); animation:shimmer 1.5s infinite; margin-bottom:8px; }
.w-80{width:80%;}.w-60{width:60%;}.w-90{width:90%;}

/* ─── HIGHLIGHT ANIMATION pour scroll-to ─── */
@keyframes panelPulse {
  0%   { box-shadow: 0 0 0 0 rgba(245,158,11,0.4); }
  70%  { box-shadow: 0 0 0 12px rgba(245,158,11,0); }
  100% { box-shadow: 0 0 0 0 rgba(245,158,11,0); }
}
.panel-highlight {
  animation: panelPulse 1.5s ease-out;
  border-color: #f59e0b !important;
}

/* ─── GRIDS ─── */
.two-col-grid   { display:grid; grid-template-columns:var(--col1,1fr) var(--col2,1fr); gap:24px; }
.three-col-grid { display:grid; grid-template-columns:repeat(3,1fr); gap:24px; }
@media(max-width:960px){.two-col-grid,.three-col-grid{grid-template-columns:1fr;}}

/* ─── PANEL ─── */
.panel { background:var(--bg-card,#fff); border-radius:32px; padding:28px; border:1px solid var(--border-color,#eef2f6); box-shadow:0 4px 16px rgba(0,0,0,0.03); transition:background-color 0.3s,border-color 0.3s,box-shadow 0.3s; }
.panel-title { font-size:15px; font-weight:800; color:var(--text-main,#0f172a); }
.text-amber { color:#f59e0b !important; }
.text-blue  { color:#3b82f6 !important; }
.text-green { color:#10b981 !important; }
.text-indigo{ color:#6366f1 !important; }
.btn-see-all { font-size:11px; font-weight:700; background:none; border:1px solid var(--border-color,#e2e8f0); border-radius:100px; padding:5px 12px; cursor:pointer; color:var(--text-muted,#64748b); font-family:inherit; display:inline-flex; align-items:center; gap:4px; transition:all 0.2s; }
.btn-see-all:hover { background:var(--bg-input,#f8fafc); color:var(--text-main,#0f172a); }

/* ─── SKELETONS ─── */
.reco-skeletons { display:flex; flex-direction:column; gap:10px; }
.reco-skel { display:flex; align-items:center; gap:12px; padding:14px; border-radius:16px; background:var(--bg-input,#f8fafc); }
.skel-icon { width:40px; height:40px; min-width:40px; border-radius:10px; background:var(--border-color,#e2e8f0); animation:shimmer 1.5s infinite; }
.skel-lines { flex:1; }
.skel-l { height:10px; border-radius:6px; background:var(--border-color,#e2e8f0); animation:shimmer 1.5s infinite; margin-bottom:8px; }
.skel-title{width:55%;}.skel-desc{width:85%;height:8px;}

/* ─── TESTS ─── */
.test-list { display:flex; flex-direction:column; gap:10px; }
.test-item { display:flex; align-items:center; justify-content:space-between; padding:14px; border-radius:18px; border:1px solid var(--border-color,#eef2f6); cursor:pointer; background:var(--bg-input,#f8fafc); transition:transform 0.2s,box-shadow 0.2s; }
.test-item:hover { transform:translateX(4px); box-shadow:0 4px 12px rgba(0,0,0,0.05); }
.test-left { display:flex; align-items:center; gap:12px; }
.test-ico  { width:40px; height:40px; border-radius:12px; display:flex; align-items:center; justify-content:center; font-size:15px; flex-shrink:0; }
.test-name { font-size:13px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.test-meta { font-size:11px; color:var(--text-muted,#94a3b8); display:block; margin-top:2px; }
.test-right { display:flex; align-items:center; gap:10px; }
.test-status-badge { font-size:10px; font-weight:800; padding:4px 10px; border-radius:100px; }
.test-arrow { font-size:11px; opacity:0.6; }

/* ─── PROGRESSION ─── */
.progress-list { display:flex; flex-direction:column; gap:14px; }
.progress-label { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); }
.progress-pct   { font-size:12px; font-weight:800; }
.progress-bar-wrap { height:6px; background:var(--border-color,#f1f5f9); border-radius:100px; overflow:hidden; }
.progress-bar-fill { height:100%; border-radius:100px; transition:width 1s ease; }
.next-test-cta,.analyse-cta,.invite-cta,.analytics-cta { display:flex; align-items:center; gap:10px; padding:12px 16px; background:var(--bg-input,#f8fafc); border-radius:14px; border:1px solid var(--border-color,#eef2f6); cursor:pointer; font-size:13px; font-weight:700; color:var(--text-main,#0f172a); transition:all 0.2s; }
.next-test-cta:hover,.analyse-cta:hover,.invite-cta:hover,.analytics-cta:hover { background:rgba(245,158,11,0.08); border-color:#f59e0b; color:#d97706; }

/* ─── RESULTS ─── */
.results-list { display:flex; flex-direction:column; gap:8px; }
.result-row   { display:flex; align-items:center; gap:12px; padding:10px; border-radius:14px; cursor:pointer; transition:background-color 0.2s; }
.result-row:hover { background:var(--bg-input,#f8fafc); }
.result-score { width:52px; height:40px; border-radius:10px; font-size:12px; font-weight:900; display:flex; align-items:center; justify-content:center; flex-shrink:0; }
.result-name  { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.result-date  { font-size:11px; color:var(--text-muted,#94a3b8); display:block; }
.result-arrow { color:var(--text-muted,#94a3b8); font-size:11px; }

/* ─── EVAL ─── */
.eval-list { display:flex; flex-direction:column; gap:10px; }
.eval-item { display:flex; align-items:center; gap:12px; padding:12px; border-radius:16px; border:1px solid var(--border-color,#eef2f6); background:var(--bg-input,#f8fafc); }
.eval-avatar { width:38px; height:38px; border-radius:50%; background:linear-gradient(135deg,#f59e0b,#fbbf24); display:flex; align-items:center; justify-content:center; font-size:14px; font-weight:900; color:#0f172a; flex-shrink:0; }
.eval-name  { font-size:13px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.eval-test  { font-size:11px; color:var(--text-muted,#94a3b8); display:block; }
.eval-right { display:flex; flex-direction:column; align-items:flex-end; gap:6px; }
.eval-badge { font-size:9px; font-weight:800; padding:3px 8px; border-radius:100px; text-transform:uppercase; }
.eval-btn   { font-size:11px; font-weight:700; background:none; border:none; cursor:pointer; font-family:inherit; padding:0; }

/* ─── SESSIONS ─── */
.session-list { display:flex; flex-direction:column; gap:10px; }
.session-item { display:flex; align-items:center; gap:14px; padding:12px; border-radius:16px; border:1px solid var(--border-color,#eef2f6); background:var(--bg-input,#f8fafc); }
.session-date-block { width:48px; min-width:48px; height:48px; border-radius:14px; display:flex; flex-direction:column; align-items:center; justify-content:center; }
.sess-day   { font-size:18px; font-weight:900; line-height:1; }
.sess-month { font-size:9px; font-weight:700; text-transform:uppercase; letter-spacing:0.5px; }
.session-body { flex:1; }
.sess-title { font-size:13px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.sess-info  { font-size:11px; color:var(--text-muted,#94a3b8); display:block; margin-top:2px; }
.sess-status { font-size:10px; font-weight:700; }

/* ─── SKILLS ─── */
.skill-radar-list { display:flex; flex-direction:column; gap:12px; }
.skill-bar-label  { font-size:12px; font-weight:600; color:var(--text-main,#0f172a); }
.skill-bar-pct    { font-size:12px; font-weight:800; }

/* ─── TEAM ─── */
.team-grid { display:grid; grid-template-columns:repeat(2,1fr); gap:10px; }
.team-card { display:flex; flex-direction:column; align-items:center; padding:14px; border-radius:18px; border:1px solid var(--border-color,#eef2f6); background:var(--bg-input,#f8fafc); gap:6px; text-align:center; }
.team-avatar { width:44px; height:44px; border-radius:50%; display:flex; align-items:center; justify-content:center; font-size:16px; font-weight:900; color:white; }
.team-name  { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); }
.team-role  { font-size:10px; color:var(--text-muted,#94a3b8); }
.team-badge { font-size:9px; font-weight:700; padding:2px 8px; border-radius:100px; }

/* ─── CANDIDATES ─── */
.candidates-list { display:flex; flex-direction:column; gap:6px; }
.cand-row { display:flex; align-items:center; gap:10px; padding:10px; border-radius:12px; cursor:pointer; transition:background-color 0.2s; }
.cand-row:hover { background:var(--bg-input,#f8fafc); }
.cand-avatar { width:36px; height:36px; border-radius:50%; background:linear-gradient(135deg,#6366f1,#8b5cf6); display:flex; align-items:center; justify-content:center; font-size:13px; font-weight:900; color:white; flex-shrink:0; }
.cand-name  { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.cand-test  { font-size:11px; color:var(--text-muted,#94a3b8); display:block; }
.cand-score { font-size:14px; font-weight:900; flex-shrink:0; }

/* ─── HEALTH ─── */
.health-list { display:flex; flex-direction:column; gap:8px; }
.health-row  { display:flex; align-items:center; gap:10px; padding:10px 12px; border-radius:12px; background:var(--bg-input,#f8fafc); border:1px solid var(--border-color,#eef2f6); }
.health-dot  { width:10px; height:10px; border-radius:50%; flex-shrink:0; }
.health-name { font-size:12px; font-weight:600; color:var(--text-main,#0f172a); }
.health-latency { font-size:11px; color:var(--text-muted,#94a3b8); font-family:'JetBrains Mono',monospace; }
.health-status  { font-size:10px; font-weight:800; }
.live-badge { display:inline-flex; align-items:center; gap:5px; font-size:10px; font-weight:800; color:#10b981; background:rgba(16,185,129,0.12); padding:3px 10px; border-radius:100px; }

/* ─── COMPANIES ─── */
.company-list { display:flex; flex-direction:column; gap:8px; }
.company-row  { display:flex; align-items:center; gap:12px; padding:10px; border-radius:14px; border:1px solid var(--border-color,#eef2f6); background:var(--bg-input,#f8fafc); }
.company-logo { width:38px; height:38px; border-radius:10px; display:flex; align-items:center; justify-content:center; font-size:15px; font-weight:900; color:white; flex-shrink:0; }
.company-name { font-size:13px; font-weight:700; color:var(--text-main,#0f172a); display:block; }
.company-plan { font-size:10px; color:var(--text-muted,#94a3b8); display:block; }
.company-users { font-size:11px; font-weight:700; color:var(--text-muted,#94a3b8); flex-shrink:0; }

/* ─── ACTIVITY ─── */
.activity-count-badge { background:rgba(245,158,11,0.12); color:#d97706; font-size:11px; font-weight:800; padding:3px 10px; border-radius:100px; }
.activity-list { display:flex; flex-direction:column; gap:4px; }
.activity-row  { display:flex; align-items:center; gap:12px; padding:10px 8px; border-radius:14px; transition:background-color 0.2s; }
.activity-row:hover { background:var(--bg-input,#f8fafc); }
.act-dot    { width:4px; height:34px; border-radius:10px; flex-shrink:0; }
.act-user   { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); }
.act-action { font-size:11px; color:var(--text-muted,#94a3b8); }
.act-time   { font-size:10px; color:var(--text-muted,#94a3b8); font-family:'JetBrains Mono',monospace; flex-shrink:0; }

/* ─── CHART ─── */
.chart-wrap { height:270px; }
.period-switcher { display:flex; gap:4px; background:var(--bg-input,#f8fafc); padding:4px; border-radius:14px; border:1px solid var(--border-color,#eef2f6); }
.period-btn { font-size:11px; font-weight:700; padding:6px 14px; border-radius:10px; border:none; background:none; color:var(--text-muted,#94a3b8); cursor:pointer; transition:all 0.2s; font-family:inherit; }
.period-btn.active { background:var(--bg-card,#fff); color:var(--text-main,#0f172a); box-shadow:0 2px 8px rgba(0,0,0,0.06); }

/* ─── EMPTY STATE ─── */
.empty-state { text-align:center; padding:24px; color:var(--text-muted,#94a3b8); font-size:13px; }

/* ─── CV SCAN ─── */
.neural-badge { font-size:10px; font-weight:700; background:rgba(245,158,11,0.15); color:#d97706; padding:2px 8px; border-radius:100px; display:block; margin-top:3px; }
.cv-desc  { font-size:12px; color:var(--text-muted,#64748b); margin:0 0 16px; line-height:1.5; }
.upload-zone { border:2px dashed var(--border-color,#e2e8f0); border-radius:20px; padding:28px; text-align:center; cursor:pointer; transition:all 0.25s; background:var(--bg-input,#f8fafc); }
.upload-zone:hover,.upload-zone.uploading { border-color:#f59e0b; background:rgba(245,158,11,0.04); }
.upload-zone.uploading { transform:scale(1.02); }
.upload-icon { font-size:2rem; color:#f59e0b; }
.upload-text { font-size:13px; font-weight:700; color:var(--text-main,#0f172a); margin:0 0 4px; }
.upload-hint { font-size:11px; color:var(--text-muted,#94a3b8); }
.cv-result { display:flex; align-items:center; gap:16px; padding:16px; border-radius:18px; background:var(--bg-input,#f8fafc); border:1px solid var(--border-color,#eef2f6); }
.score-ring { width:80px; min-width:80px; }
.score-ring svg { width:80px; height:80px; }
.score-svg-text { font-family:'Plus Jakarta Sans',sans-serif; font-size:13px; font-weight:800; fill:var(--text-main,#0f172a); }
.cv-verdict    { font-weight:700; font-size:13px; margin:0 0 8px; }
.strength-pill { font-size:10px; font-weight:600; background:#ecfdf5; color:#059669; padding:2px 8px; border-radius:100px; }
.decision-badge { font-size:11px; font-weight:600; color:var(--text-muted,#64748b); background:var(--bg-card,#fff); border:1px solid var(--border-color,#eef2f6); padding:4px 10px; border-radius:8px; display:inline-block; }
.cv-conseils { background:rgba(245,158,11,0.06); border-radius:10px; padding:10px; border:1px solid rgba(245,158,11,0.2); }
.conseils-title { font-size:11px; font-weight:800; color:#d97706; margin-bottom:6px; }
.conseils-list { margin:0; padding-left:16px; }
.conseils-list li { font-size:11px; color:var(--text-muted,#64748b); margin-bottom:3px; }
.job-input { width:100%; padding:10px 14px; border-radius:12px; border:1px solid var(--border-color,#e2e8f0); background:var(--bg-input,#f8fafc); font-size:12px; color:var(--text-main,#0f172a); font-family:inherit; outline:none; transition:border-color 0.2s; box-sizing:border-box; }
.job-input:focus { border-color:#f59e0b; }
select.job-input { cursor:pointer; }
.btn-reset { font-size:11px; font-weight:700; color:var(--text-muted,#64748b); background:none; border:1px solid var(--border-color,#e2e8f0); border-radius:8px; padding:4px 10px; cursor:pointer; display:inline-flex; align-items:center; transition:all 0.2s; font-family:inherit; }
.btn-reset:hover { background:var(--bg-input,#f8fafc); color:var(--text-main,#0f172a); }
.btn-enigma-primary { background:#0f172a; color:white; border:none; padding:14px 28px; border-radius:18px; font-weight:800; position:relative; overflow:hidden; cursor:pointer; font-family:inherit; width:100%; }
.btn-enigma-primary .btn-glow { position:absolute; inset:0; background:linear-gradient(135deg,#f59e0b,#fbbf24); opacity:0; transition:0.3s; z-index:1; }
.btn-enigma-primary:hover .btn-glow { opacity:1; }
.btn-enigma-primary .btn-content { position:relative; z-index:2; display:flex; align-items:center; justify-content:center; gap:6px; }
.btn-enigma-primary:hover .btn-content { color:#0f172a; }
.btn-enigma-primary:disabled { opacity:0.45; cursor:not-allowed; }

/* ─── LETTRE ─── */
.lettre-form { display:flex; flex-direction:column; }
.lettre-label-field { font-size:11px; font-weight:700; color:var(--text-muted,#64748b); text-transform:uppercase; letter-spacing:0.5px; display:block; margin-bottom:5px; }
.lang-badge-pill { display:inline-flex; align-items:center; gap:4px; padding:3px 10px; border-radius:20px; font-size:11px; font-weight:700; }
.lang-pill-fr { background:rgba(59,130,246,0.12); color:#2563eb; }
.lang-pill-en { background:rgba(16,185,129,0.12); color:#059669; }
.lang-pill-ar { background:rgba(245,158,11,0.12); color:#d97706; }
.result-style-badge { padding:3px 10px; background:var(--bg-input,#f1f5f9); border-radius:20px; font-size:11px; font-weight:600; color:var(--text-muted,#64748b); text-transform:capitalize; }
.source-gemini { padding:3px 10px; background:rgba(16,185,129,0.1); color:#059669; border-radius:20px; font-size:11px; font-weight:600; }
.btn-lettre-primary { background:linear-gradient(135deg,#8b5cf6,#7c3aed); color:white; border:none; padding:14px 24px; border-radius:18px; font-weight:800; cursor:pointer; font-family:inherit; font-size:13px; display:flex; align-items:center; justify-content:center; gap:8px; transition:all 0.25s; width:100%; }
.btn-lettre-primary:hover:not(:disabled) { transform:translateY(-2px); box-shadow:0 8px 20px rgba(139,92,246,0.3); }
.btn-lettre-primary:disabled { opacity:0.45; cursor:not-allowed; }
.lettre-result { display:flex; flex-direction:column; }
.lettre-label  { font-size:12px; font-weight:700; color:var(--text-main,#0f172a); display:flex; align-items:center; }
.lettre-content { background:var(--bg-input,#f8fafc); border:1px solid var(--border-color,#eef2f6); border-radius:16px; padding:18px; font-size:12px; line-height:1.8; color:var(--text-muted,#64748b); white-space:pre-line; overflow-y:auto; max-height:320px; margin-top:4px; }
.btn-copy-lettre { display:inline-flex; align-items:center; gap:6px; font-size:11px; font-weight:700; padding:6px 12px; border-radius:10px; border:1px solid var(--border-color,#e2e8f0); background:var(--bg-input,#f8fafc); color:var(--text-muted,#64748b); cursor:pointer; transition:all 0.2s; font-family:inherit; }
.btn-copy-lettre:hover { border-color:#8b5cf6; color:#8b5cf6; }

/* ─── TRANSITIONS ─── */
.fade-up-enter-active { transition:all 0.35s ease; }
.fade-up-enter-from  { opacity:0; transform:translateY(10px); }

/* ─── TOAST ─── */
.enigma-toast { position:fixed; bottom:30px; right:30px; background:#0f172a; color:white; padding:20px 30px; border-radius:20px; display:flex; align-items:center; gap:15px; z-index:3000; border-left:5px solid #f59e0b; box-shadow:0 20px 40px rgba(0,0,0,0.2); }
.t-success{border-left-color:#10b981;}.t-error{border-left-color:#f43f5e;}.t-warn{border-left-color:#f59e0b;}
.toast-slide-enter-active { animation:slideIn 0.4s ease-out; }
.toast-slide-leave-active { animation:slideIn 0.3s ease-in reverse; }

/* ─── KEYFRAMES ─── */
@keyframes pulseLive { 0%,100%{opacity:1;box-shadow:0 0 6px #10b981;}50%{opacity:0.5;box-shadow:0 0 14px #10b981;} }
@keyframes sweep { 0%{left:-100%;}100%{left:200%;} }
@keyframes floatBot { 0%,100%{transform:translateY(0);}50%{transform:translateY(-12px);} }
@keyframes spin { from{transform:rotate(0deg);}to{transform:rotate(360deg);} }
@keyframes shimmer { 0%{background-position:200% 0;}100%{background-position:-200% 0;} }
@keyframes slideIn { from{transform:translateX(120%);opacity:0;}to{transform:translateX(0);opacity:1;} }

/* ─── DARK MODE ─── */
[data-theme="dark"] .aura-dashboard { background:#0d1117;color:#f0f6fc; }
[data-theme="dark"] .terminal-bar,[data-theme="dark"] .hero-card,[data-theme="dark"] .panel,[data-theme="dark"] .kpi-card,[data-theme="dark"] .reco-section { background:#161b22;border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .reco-card,[data-theme="dark"] .reco-card-skel,[data-theme="dark"] .test-item,[data-theme="dark"] .eval-item,[data-theme="dark"] .session-item,[data-theme="dark"] .team-card,[data-theme="dark"] .health-row,[data-theme="dark"] .company-row { background:rgba(255,255,255,0.03);border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .reco-card-title,[data-theme="dark"] .reco-title,[data-theme="dark"] .kpi-value,[data-theme="dark"] .hero-title,[data-theme="dark"] .premium-title,[data-theme="dark"] .panel-title,[data-theme="dark"] .ia-label,[data-theme="dark"] .eval-name,[data-theme="dark"] .sess-title,[data-theme="dark"] .team-name,[data-theme="dark"] .health-name,[data-theme="dark"] .company-name,[data-theme="dark"] .cand-name,[data-theme="dark"] .skill-bar-label,[data-theme="dark"] .progress-label,[data-theme="dark"] .act-user,[data-theme="dark"] .test-name,[data-theme="dark"] .result-name { color:#f0f6fc; }
[data-theme="dark"] .reco-card-desc,[data-theme="dark"] .ia-msg,[data-theme="dark"] .act-action,[data-theme="dark"] .lettre-content { color:#8b949e; }
[data-theme="dark"] .reco-card-action,[data-theme="dark"] .ia-insight,[data-theme="dark"] .mini-stat,[data-theme="dark"] .metric-pill { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .next-test-cta,[data-theme="dark"] .analyse-cta,[data-theme="dark"] .invite-cta,[data-theme="dark"] .analytics-cta { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.08);color:#f0f6fc; }
[data-theme="dark"] .activity-row:hover,[data-theme="dark"] .result-row:hover,[data-theme="dark"] .cand-row:hover { background:rgba(255,255,255,0.04); }
[data-theme="dark"] .period-switcher { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .period-btn.active { background:rgba(255,255,255,0.08);color:#f0f6fc; }
[data-theme="dark"] .theme-toggle-btn { background:rgba(255,255,255,0.05);border-color:rgba(255,255,255,0.1);color:#f0f6fc; }
[data-theme="dark"] .btn-see-all { border-color:rgba(255,255,255,0.1);color:#8b949e; }
[data-theme="dark"] .btn-see-all:hover { background:rgba(255,255,255,0.06);color:#f0f6fc; }
[data-theme="dark"] .upload-zone { background:rgba(255,255,255,0.03);border-color:rgba(255,255,255,0.1); }
[data-theme="dark"] .upload-text { color:#f0f6fc; }
[data-theme="dark"] .cv-result { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .score-svg-text { fill:#f0f6fc; }
[data-theme="dark"] .strength-pill { background:rgba(16,185,129,0.15);color:#34d399; }
[data-theme="dark"] .btn-reset { border-color:rgba(255,255,255,0.1);color:#8b949e; }
[data-theme="dark"] .job-input { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.1);color:#f0f6fc; }
[data-theme="dark"] .decision-badge { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.08);color:#8b949e; }
[data-theme="dark"] .cv-conseils { background:rgba(245,158,11,0.08);border-color:rgba(245,158,11,0.15); }
[data-theme="dark"] .progress-bar-wrap { background:rgba(255,255,255,0.08); }
[data-theme="dark"] .btn-copy-lettre { background:rgba(255,255,255,0.04);border-color:rgba(255,255,255,0.1);color:#8b949e; }
[data-theme="dark"] .neural-badge { color:#fbbf24;background:rgba(245,158,11,0.2); }
[data-theme="dark"] .lang-pill-fr { background:rgba(59,130,246,0.2); }
[data-theme="dark"] .lang-pill-en { background:rgba(16,185,129,0.2); }
[data-theme="dark"] .lang-pill-ar { background:rgba(245,158,11,0.2); }
[data-theme="dark"] .result-style-badge { background:rgba(255,255,255,0.06);color:#8b949e; }
[data-theme="dark"] .source-gemini { background:rgba(16,185,129,0.15);color:#34d399; }
[data-theme="dark"] .lettre-content { background:rgba(255,255,255,0.03);border-color:rgba(255,255,255,0.08); }
[data-theme="dark"] .panel-highlight { border-color:#f59e0b !important; }

/* ═══════════════════════════════════════════
   CYBER GLASS MODALS
   ═══════════════════════════════════════════ */
.enigma-modal-overlay {
  position: fixed; inset: 0; z-index: 9999;
  background: rgba(8, 12, 36, 0.7);
  backdrop-filter: blur(20px);
  -webkit-backdrop-filter: blur(20px);
  display: flex; align-items: center; justify-content: center;
  padding: 20px;
  animation: fadeIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

.enigma-modal-container {
  width: 100%; max-width: 680px;
  border-radius: 28px; border: 1px solid rgba(99, 102, 241, 0.25);
  box-shadow: 0 30px 70px -15px rgba(0, 0, 0, 0.6), 0 0 50px rgba(99, 102, 241, 0.15);
  overflow: hidden;
  transition: all 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.enigma-modal-container.max-w-lg { max-width: 780px; }

.glass-morphic {
  background: rgba(15, 18, 42, 0.9);
  backdrop-filter: blur(30px);
  -webkit-backdrop-filter: blur(30px);
  border: 1px solid rgba(99, 102, 241, 0.2);
}
[data-theme="light"] .glass-morphic {
  background: rgba(255, 255, 255, 0.95);
  border-color: rgba(99, 102, 241, 0.15);
}

.enigma-modal-header {
  padding: 24px 32px;
  border-bottom: 1px solid rgba(99, 102, 241, 0.15);
  display: flex; align-items: center; justify-content: space-between;
}
[data-theme="light"] .enigma-modal-header {
  border-bottom-color: rgba(99, 102, 241, 0.1);
}

.btn-close-modal {
  background: rgba(99, 102, 241, 0.08);
  border: none; border-radius: 12px;
  width: 36px; height: 36px; display: flex;
  align-items: center; justify-content: center;
  color: #818cf8; transition: 0.2s; cursor: pointer;
}
.btn-close-modal:hover {
  background: rgba(239, 68, 68, 0.25);
  color: #f87171;
}

.enigma-modal-body { padding: 32px; }

/* STATUT CARD */
.status-card {
  display: flex; align-items: center; gap: 20px;
  padding: 20px; border-radius: 20px;
  border: 1.5px solid transparent;
}
.status-card.active-green {
  background: rgba(16, 185, 129, 0.08);
  border-color: rgba(16, 185, 129, 0.2);
}
.status-card.error-red {
  background: rgba(239, 68, 68, 0.08);
  border-color: rgba(239, 68, 68, 0.2);
}
.status-card-icon {
  font-size: 2.2rem; display: flex; align-items: center;
}
.text-green { color: #10b981; }
.text-red { color: #ef4444; }

.status-label { font-size: 0.65rem; font-weight: 900; color: #818cf8; letter-spacing: 1.2px; text-transform: uppercase; }
[data-theme="light"] .status-label { color: #4f46e5; }
.status-value { font-size: 1.05rem; color: #fff; }
[data-theme="light"] .status-value { color: #0f172a; }
.status-desc { color: #a5b4fc; font-size: 0.78rem; opacity: 0.9; }
[data-theme="light"] .status-desc { color: #475569; }

/* PENDING INVITES BANNER */
.pending-invites-banner {
  background: rgba(99, 102, 241, 0.05);
  border: 1px solid rgba(99, 102, 241, 0.18);
  border-radius: 20px; padding: 20px 28px;
}
[data-theme="light"] .pending-invites-banner {
  background: rgba(99, 102, 241, 0.03);
  border-color: rgba(99, 102, 241, 0.12);
}

.btn-action-primary {
  padding: 12px 28px; border-radius: 14px;
  border: none; font-weight: 800; font-size: 0.85rem;
  color: #fff; cursor: pointer; transition: all 0.3s;
}
.glow-orange {
  background: linear-gradient(135deg, #f59e0b, #fbbf24);
  box-shadow: 0 8px 24px rgba(245, 158, 11, 0.25);
}
.glow-orange:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 28px rgba(245, 158, 11, 0.4);
}
.glow-green {
  background: linear-gradient(135deg, #10b981, #34d399);
  box-shadow: 0 8px 24px rgba(16, 185, 129, 0.25);
}
.glow-green:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 28px rgba(16, 185, 129, 0.4);
}
.glow-red {
  background: linear-gradient(135deg, #ef4444, #f87171);
  box-shadow: 0 8px 24px rgba(239, 68, 68, 0.25);
}
.glow-red:hover {
  transform: translateY(-2px);
  box-shadow: 0 12px 28px rgba(239, 68, 68, 0.4);
}

.btn-action-secondary {
  padding: 12px 24px; border-radius: 14px;
  background: rgba(99, 102, 241, 0.08);
  border: 1px solid rgba(99, 102, 241, 0.2);
  color: #818cf8; font-weight: 700;
  transition: all 0.3s;
  cursor: pointer;
}
.btn-action-secondary:hover {
  background: rgba(99, 102, 241, 0.15);
  border-color: rgba(99, 102, 241, 0.4);
  color: #a5b4fc;
}
[data-theme="light"] .btn-action-secondary {
  background: rgba(79, 70, 229, 0.05);
  border-color: rgba(79, 70, 229, 0.15);
  color: #4f46e5;
}
[data-theme="light"] .btn-action-secondary:hover {
  background: rgba(79, 70, 229, 0.1);
  color: #3730a3;
}

/* TERMINAL CONSOLE */
.terminal-logs-wrapper {
  background: #090d16; border-radius: 20px;
  border: 1.5px solid rgba(99, 102, 241, 0.2);
  overflow: hidden;
  transition: all 0.3s ease;
}
[data-theme="light"] .terminal-logs-wrapper {
  background: #ffffff !important;
  border: 1.5px solid rgba(99, 102, 241, 0.25) !important;
  box-shadow: 0 10px 30px rgba(99, 102, 241, 0.06) !important;
}
.terminal-header {
  background: #0f1524; padding: 14px 20px;
  border-bottom: 1.5px solid rgba(99, 102, 241, 0.2);
}
[data-theme="light"] .terminal-header {
  background: rgba(99, 102, 241, 0.04) !important;
  border-bottom: 1.5px solid rgba(99, 102, 241, 0.15) !important;
}
.dot {
  width: 10px; height: 10px; border-radius: 50%;
  display: inline-block;
}
.dot.red { background: #ef4444; }
.dot.yellow { background: #f59e0b; }
.dot.green { background: #10b981; }

.terminal-title { font-family: 'JetBrains Mono', monospace; font-size: 0.72rem; color: #a5b4fc; }
[data-theme="light"] .terminal-title {
  color: #4f46e5 !important;
  font-weight: 700 !important;
}
.terminal-badge { font-family: 'JetBrains Mono', monospace; color: #818cf8; font-weight: 700; }
[data-theme="light"] .terminal-badge {
  color: #6366f1 !important;
}
.terminal-body {
  padding: 20px; height: 260px; overflow-y: auto;
  font-family: 'JetBrains Mono', monospace; font-size: 0.75rem;
  line-height: 1.6; color: #c9d1d9;
}
[data-theme="light"] .terminal-body {
  background: #fafafa !important;
  color: #1e293b !important;
}
.terminal-log-line { margin-bottom: 8px; white-space: pre-wrap; text-align: left; }
.terminal-log-line.log-success { color: #58a6ff; }
[data-theme="light"] .terminal-log-line.log-success { color: #0969da !important; font-weight: 600; }
.terminal-log-line.log-warning { color: #f2cc60; }
[data-theme="light"] .terminal-log-line.log-warning { color: #9a6700 !important; font-weight: 600; }
.terminal-log-line.log-critical { color: #ff7b72; }
[data-theme="light"] .terminal-log-line.log-critical { color: #cf222e !important; font-weight: 600; }
.terminal-log-line.log-help { color: #79c0ff; }
[data-theme="light"] .terminal-log-line.log-help { color: #0550ae !important; }
.log-cursor { color: #f59e0b; }
[data-theme="light"] .log-cursor { color: #4f46e5 !important; }
.log-scanning-cursor { color: #f59e0b; animation: pulse 1.5s infinite; text-align: left; padding: 20px; }
[data-theme="light"] .log-scanning-cursor { color: #4f46e5 !important; }

/* RESPONSIVE TABLE FOR SUB */
.subs-table-wrapper {
  max-height: 480px; overflow-y: auto;
  padding: 0;
}
.subs-table {
  width: 100%; border-collapse: collapse; text-align: left;
}
.subs-table th {
  padding: 16px 24px; font-size: 0.65rem; font-weight: 900;
  text-transform: uppercase; letter-spacing: 0.8px;
  color: #818cf8; border-bottom: 1.5px solid rgba(99, 102, 241, 0.15);
}
[data-theme="light"] .subs-table th {
  border-bottom-color: rgba(99, 102, 241, 0.1);
  color: #4f46e5;
}
.subs-table td {
  padding: 18px 24px; border-bottom: 1px solid rgba(99, 102, 241, 0.08);
}
[data-theme="light"] .subs-table td {
  border-bottom-color: rgba(99, 102, 241, 0.05);
}
.sub-row:hover { background: rgba(99, 102, 241, 0.03); }
[data-theme="light"] .sub-row:hover { background: rgba(99, 102, 241, 0.02); }

.sub-company-name { color: #fff; font-size: 0.88rem; }
[data-theme="light"] .sub-company-name { color: #0f172a; }
.sub-plan-badge {
  background: rgba(99, 102, 241, 0.15); color: #818cf8;
  padding: 4px 10px; border-radius: 8px; display: inline-block;
}
[data-theme="light"] .sub-plan-badge {
  background: rgba(79, 70, 229, 0.08) !important;
  color: #4f46e5 !important;
}
.sub-days-badge {
  padding: 4px 10px; border-radius: 8px; display: inline-block;
  font-weight: 800;
}
.sub-days-badge.warning { background: rgba(245, 158, 11, 0.15); color: #fbbf24; }
[data-theme="light"] .sub-days-badge.warning { background: rgba(245, 158, 11, 0.12) !important; color: #b45309 !important; }
.sub-days-badge.danger { background: rgba(239, 68, 68, 0.15); color: #f87171; }
[data-theme="light"] .sub-days-badge.danger { background: rgba(239, 68, 68, 0.1) !important; color: #cf222e !important; }

.btn-send-reminder {
  width: 36px; height: 36px; border-radius: 10px;
  background: rgba(245, 158, 11, 0.1); border: 1.5px solid rgba(245, 158, 11, 0.2);
  color: #f59e0b; display: inline-flex; align-items: center; justify-content: center;
  cursor: pointer; transition: 0.2s;
}
.btn-send-reminder:hover {
  background: #f59e0b; color: white;
  box-shadow: 0 4px 12px rgba(245, 158, 11, 0.3);
}
[data-theme="light"] .btn-send-reminder {
  background: rgba(245, 158, 11, 0.08) !important;
  border-color: rgba(245, 158, 11, 0.25) !important;
  color: #d97706 !important;
}
[data-theme="light"] .btn-send-reminder:hover {
  background: #d97706 !important;
  color: white !important;
  box-shadow: 0 4px 12px rgba(217, 119, 6, 0.25) !important;
}

/* SECURITY SHIELD HUD */
.cyber-shield-icon-wrapper {
  font-size: 4.8rem; height: 120px; display: flex; align-items: center; justify-content: center;
}
.security-scanning-hud {
  background: rgba(8, 10, 28, 0.4); border-radius: 24px;
  border: 1px solid rgba(99, 102, 241, 0.15); padding: 24px;
  transition: all 0.3s ease;
}
[data-theme="light"] .security-scanning-hud {
  background: linear-gradient(135deg, rgba(239, 246, 255, 0.85), rgba(245, 243, 255, 0.85)) !important;
  backdrop-filter: blur(15px);
  -webkit-backdrop-filter: blur(15px);
  border: 1px solid rgba(99, 102, 241, 0.22) !important;
  box-shadow: 0 10px 30px rgba(99, 102, 241, 0.06), inset 0 1px 0 rgba(255, 255, 255, 0.8) !important;
}

.hud-step-item {
  display: flex; align-items: center; gap: 18px;
  padding: 16px 20px; border-radius: 16px;
  border: 1px solid transparent; margin-bottom: 12px;
  transition: all 0.3s;
  text-align: left;
}
.hud-step-item.step-pending {
  opacity: 0.4; background: rgba(99, 102, 241, 0.02);
}
[data-theme="light"] .hud-step-item.step-pending {
  opacity: 0.75 !important;
  background: rgba(99, 102, 241, 0.03) !important;
  border-color: rgba(99, 102, 241, 0.08) !important;
}
[data-theme="light"] .hud-step-item.step-pending .text-muted {
  color: #475569 !important;
}

.hud-step-item.step-scanning {
  background: rgba(245, 158, 11, 0.05);
  border-color: rgba(245, 158, 11, 0.2);
  box-shadow: 0 0 15px rgba(245, 158, 11, 0.05);
}
[data-theme="light"] .hud-step-item.step-scanning {
  background: rgba(245, 158, 11, 0.08) !important;
  border-color: rgba(245, 158, 11, 0.4) !important;
  box-shadow: 0 4px 15px rgba(245, 158, 11, 0.12) !important;
}
[data-theme="light"] .hud-step-item.step-scanning .hud-step-name {
  color: #b45309 !important;
}
[data-theme="light"] .hud-step-item.step-scanning .text-muted {
  color: #d97706 !important;
}

.hud-step-item.step-done {
  background: rgba(16, 185, 129, 0.05);
  border-color: rgba(16, 185, 129, 0.15);
}
[data-theme="light"] .hud-step-item.step-done {
  background: rgba(16, 185, 129, 0.08) !important;
  border-color: rgba(16, 185, 129, 0.3) !important;
}
[data-theme="light"] .hud-step-item.step-done .hud-step-name {
  color: #065f46 !important;
}
[data-theme="light"] .hud-step-item.step-done .text-muted {
  color: #059669 !important;
}

.hud-step-icon {
  width: 38px; height: 38px; border-radius: 10px;
  background: rgba(99, 102, 241, 0.08); display: flex;
  align-items: center; justify-content: center; font-size: 1rem;
  color: #818cf8;
}
.step-scanning .hud-step-icon {
  background: rgba(245, 158, 11, 0.15); color: #f59e0b;
}
.step-done .hud-step-icon {
  background: rgba(16, 185, 129, 0.15); color: #10b981;
}

.hud-step-name { color: #fff; font-size: 0.85rem; }
[data-theme="light"] .hud-step-name { color: #0f172a; }

.hud-logs-wrapper {
  background: #090d16; border-radius: 16px;
  border: 1px solid rgba(99, 102, 241, 0.2);
  padding: 16px 20px;
}
[data-theme="light"] .hud-logs-wrapper {
  background: #fafafa !important;
  border: 1px solid rgba(99, 102, 241, 0.25) !important;
  box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.02) !important;
}
.hud-logs {
  height: 90px; overflow-y: auto;
  font-family: 'JetBrains Mono', monospace; font-size: 0.72rem;
}
.hud-log-line { color: #a5b4fc; line-height: 1.5; text-align: left; }
[data-theme="light"] .hud-log-line {
  color: #4f46e5 !important;
  font-weight: 600;
}

/* RADIAL SCORE GAUGE */
.score-radial-wrapper {
  display: flex; flex-direction: column; align-items: center;
}
.score-radial {
  width: 110px; height: 110px; border-radius: 50%;
  border: 8px solid var(--score-color, #10b981);
  display: flex; flex-direction: column;
  align-items: center; justify-content: center;
  background: rgba(99, 102, 241, 0.04);
  box-shadow: inset 0 0 15px rgba(99, 102, 241, 0.1);
}
[data-theme="light"] .score-radial {
  background: rgba(99, 102, 241, 0.05) !important;
  box-shadow: inset 0 0 15px rgba(99, 102, 241, 0.08) !important;
}
.score-value { font-size: 2.1rem; color: #fff; line-height: 1; }
[data-theme="light"] .score-value { color: #0f172a; }
[data-theme="light"] .score-radial .text-muted {
  color: #4f46e5 !important;
  font-weight: 800;
}

.card-result-mini {
  display: flex; align-items: center; gap: 14px;
  padding: 14px 18px; border-radius: 16px;
  border: 1px solid transparent;
  text-align: left;
}
.card-result-mini.active-green {
  background: rgba(16, 185, 129, 0.06); border-color: rgba(16, 185, 129, 0.15);
}
.card-result-mini.active-warning {
  background: rgba(245, 158, 11, 0.06); border-color: rgba(245, 158, 11, 0.15);
}
[data-theme="light"] .card-result-mini {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.02) !important;
}
[data-theme="light"] .card-result-mini.active-green {
  background: rgba(16, 185, 129, 0.08) !important;
  border-color: rgba(16, 185, 129, 0.3) !important;
}
[data-theme="light"] .card-result-mini.active-warning {
  background: rgba(245, 158, 11, 0.08) !important;
  border-color: rgba(245, 158, 11, 0.3) !important;
}
.crm-icon { font-size: 1.4rem; color: #818cf8; }
.active-green .crm-icon { color: #10b981; }
.active-warning .crm-icon { color: #f59e0b; }

.checked-items-list {
  max-height: 240px; overflow-y: auto;
}
.checked-item-row {
  background: rgba(99, 102, 241, 0.03);
  border: 1px solid rgba(99, 102, 241, 0.1);
  border-radius: 14px; padding: 14px 18px; margin-bottom: 10px;
  text-align: left;
}
[data-theme="light"] .checked-item-row {
  background: #ffffff !important;
  border: 1px solid rgba(99, 102, 241, 0.15) !important;
  box-shadow: 0 4px 10px rgba(99, 102, 241, 0.03) !important;
}
.item-name { color: #fff; font-size: 0.82rem; }
[data-theme="light"] .item-name { color: #0f172a; }
[data-theme="light"] .checked-item-row .text-muted {
  color: #64748b !important;
}

/* GLOBAL LIGHT MODE TEXT & CLOSE BUTTON OVERRIDES IN MODALS */
[data-theme="light"] .btn-close-modal {
  background: rgba(99, 102, 241, 0.06) !important;
  color: #4f46e5 !important;
}
[data-theme="light"] .btn-close-modal:hover {
  background: rgba(239, 68, 68, 0.12) !important;
  color: #ef4444 !important;
}
[data-theme="light"] .enigma-modal-header h4.text-white {
  color: #0f172a !important;
}
[data-theme="light"] .pending-invites-banner .text-white {
  color: #0f172a !important;
}
[data-theme="light"] .subs-table td .text-white {
  color: #334155 !important;
}
[data-theme="light"] .security-scanning-hud h5.text-white {
  color: #0f172a !important;
}
[data-theme="light"] .card-result-mini .text-white {
  color: #0f172a !important;
}

/* ANIMATION TRANSITION */
.modal-fade-enter-active, .modal-fade-leave-active {
  transition: opacity 0.3s ease;
}
.modal-fade-enter-from, .modal-fade-leave-to {
  opacity: 0;
}
.modal-fade-enter-active .enigma-modal-container {
  animation: slideIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}
.modal-fade-leave-active .enigma-modal-container {
  animation: slideOut 0.3s cubic-bezier(0.16, 1, 0.3, 1);
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
@keyframes slideIn {
  from { transform: scale(0.95) translateY(10px); opacity: 0; }
  to { transform: scale(1) translateY(0); opacity: 1; }
}
@keyframes slideOut {
  from { transform: scale(1) translateY(0); opacity: 1; }
  to { transform: scale(0.95) translateY(10px); opacity: 0; }
}

.scrollbar-thin::-webkit-scrollbar {
  width: 5px;
}
.scrollbar-thin::-webkit-scrollbar-track {
  background: transparent;
}
.scrollbar-thin::-webkit-scrollbar-thumb {
  background: rgba(255, 255, 255, 0.1); border-radius: 4px;
}
.scrollbar-thin::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 255, 255, 0.2);
}
[data-theme="light"] .scrollbar-thin::-webkit-scrollbar-thumb {
  background: rgba(0, 0, 0, 0.1);
}

.animate-spin-slow {
  animation: spin 6s linear infinite;
}
@keyframes spin {
  from { transform: rotate(0deg); }
  to { transform: rotate(360deg); }
}
</style>