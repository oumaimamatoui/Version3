<template>
  <div
    class="cert-root"
    @contextmenu.prevent
    @copy.prevent
    @cut.prevent
    @paste.prevent
    @selectstart.prevent
    @keydown.f12.prevent
    @keydown.ctrl.prevent
  >
    <!-- ══ LUXURY BACKGROUND ══ -->
    <div class="lux-bg">
      <div class="orb orb-1"></div>
      <div class="orb orb-2"></div>
      <div class="orb orb-3"></div>
      <div class="grid-dots"></div>
    </div>

    <!-- ══════════════════════════════════════
         OVERLAY ANTI-CHEAT — SORTIE DÉTECTÉE
    ══════════════════════════════════════ -->
    <Transition name="scale-fade">
      <div v-if="security.overlayVisible" class="anticheat-overlay">
        <div class="anticheat-card">
          <div class="ac-icon-ring">
            <i class="fa-solid fa-user-shield"></i>
          </div>
          <h2 class="ac-title">PROTOCOLE ANTI-CHEAT ACTIF</h2>
          <p class="ac-desc">
            Une sortie de fenêtre a été détectée. Chaque infraction réduit votre score d'intégrité.
          </p>
          <div class="ac-infraction-count">
            <span class="ac-count-val">{{ security.infractions }}</span>
            <span class="ac-count-lbl">infraction(s) enregistrée(s)</span>
          </div>
          <div class="ac-integrity-bar-wrap">
            <div class="ac-integrity-label">
              <span>Score d'intégrité</span>
              <span :class="integrityScore >= 70 ? 'text-ok' : 'text-warn'">{{ integrityScore }}%</span>
            </div>
            <div class="ac-integrity-track">
              <div
                class="ac-integrity-fill"
                :style="{ width: integrityScore + '%', background: integrityScore >= 70 ? '#10b981' : '#f43f5e' }"
              ></div>
            </div>
          </div>
          <button
            class="btn-resume"
            @click="resumeExam"
            :disabled="security.lockTimer > 0"
          >
            <span v-if="security.lockTimer > 0">
              <i class="fa-solid fa-lock me-2"></i>VERROUILLÉ ({{ security.lockTimer }}s)
            </span>
            <span v-else>
              <i class="fa-solid fa-play me-2"></i>REPRENDRE L'EXAMEN
            </span>
          </button>
          <p v-if="security.infractions >= 5" class="ac-warning-critical">
            <i class="fa-solid fa-triangle-exclamation me-1"></i>
            Attention : 5 infractions ou plus entraîne la clôture automatique de la session.
          </p>
        </div>
      </div>
    </Transition>

    <!-- ══════════════════════════════════════
         LOBBY — ACCUEIL CANDIDAT
    ══════════════════════════════════════ -->
    <div v-if="phase === 'lobby'" class="phase-center">
      <div class="lobby-card animate-in">

        <!-- BRAND -->
        <div class="brand-logo">
          <svg viewBox="0 0 48 48" width="52" height="52">
            <rect x="4" y="4" width="40" height="40" rx="14" fill="#0f172a"/>
            <rect x="10" y="16" width="28" height="10" rx="5" fill="white" opacity=".9"/>
            <circle cx="17" cy="21" r="3" fill="#f59e0b">
              <animate attributeName="opacity" values="1;0.2;1" dur="2.5s" repeatCount="indefinite"/>
            </circle>
            <circle cx="31" cy="21" r="3" fill="#f59e0b">
              <animate attributeName="opacity" values="1;0.2;1" dur="2.5s" begin="0.4s" repeatCount="indefinite"/>
            </circle>
            <rect x="18" y="30" width="12" height="3" rx="1.5" fill="#f59e0b"/>
          </svg>
          <div class="brand-text">
            <span class="brand-name">Evalua<span>Tech</span></span>
            <span class="brand-sub">PLATEFORME DE CERTIFICATION</span>
          </div>
        </div>

        <!-- BADGE SESSION -->
        <div class="session-badge">
          <span class="badge-dot"></span> SESSION SÉCURISÉE
        </div>

        <!-- INFOS EXAM -->
        <div v-if="examMeta" class="exam-info-box">
          <h3 class="exam-title">{{ examMeta.titre }}</h3>
          <div class="exam-meta-grid">
            <div class="meta-chip">
              <i class="fa-solid fa-list-check"></i>
              <span>{{ examMeta.totalQuestions }} question(s)</span>
            </div>
            <div class="meta-chip">
              <i class="fa-solid fa-hourglass-half"></i>
              <span>{{ examMeta.dureeMinutes }} min</span>
            </div>
            <div class="meta-chip">
              <i class="fa-solid fa-trophy"></i>
              <span>Seuil : {{ examMeta.scoreReussite }}%</span>
            </div>
            <div class="meta-chip">
              <i class="fa-solid fa-tag"></i>
              <span>{{ examMeta.theme || 'Général' }}</span>
            </div>
          </div>
        </div>

        <!-- FONCTIONNALITÉS ACTIVES -->
        <div class="features-list">
          <div class="feature-item" :class="{ 'feature-active': examMeta?.anticheatEnabled !== false }">
            <div class="feat-icon">
              <i class="fa-solid fa-shield-halved"></i>
            </div>
            <div class="feat-body">
              <span class="feat-title">Surveillance Anti-Cheat v2.0</span>
              <span class="feat-desc">Analyse de fraude asynchrone — sorties surveillées</span>
            </div>
            <div class="feat-status">
              <span v-if="examMeta?.anticheatEnabled !== false" class="feat-on">ACTIF</span>
              <span v-else class="feat-off">INACTIF</span>
            </div>
          </div>

          <div class="feature-item" :class="{ 'feature-active': examMeta?.questionsWithTimer > 0 }">
            <div class="feat-icon feat-blue">
              <i class="fa-solid fa-stopwatch"></i>
            </div>
            <div class="feat-body">
              <span class="feat-title">Minuteur par Question</span>
              <span class="feat-desc">Chronomètre individuel sur chaque actif</span>
            </div>
            <div class="feat-status">
              <span class="feat-counter-badge">
                {{ examMeta?.questionsWithTimer || 0 }}
                <small>minutée(s)</small>
              </span>
            </div>
          </div>

          <div class="feature-item" :class="{ 'feature-active': examMeta?.notificationsEnabled }">
            <div class="feat-icon feat-green">
              <i class="fa-solid fa-paper-plane"></i>
            </div>
            <div class="feat-body">
              <span class="feat-title">Notifications Email</span>
              <span class="feat-desc">Confirmation automatique envoyée après la session</span>
            </div>
            <div class="feat-status">
              <span v-if="examMeta?.notificationsEnabled" class="feat-on">ACTIF</span>
              <span v-else class="feat-off">—</span>
            </div>
          </div>
        </div>

        <!-- INSTRUCTIONS -->
        <div class="instructions-box">
          <div class="inst-row"><i class="fa-solid fa-eye"></i> L'onglet actif est surveillé en permanence</div>
          <div class="inst-row"><i class="fa-solid fa-lock"></i> Ne quittez pas la page pendant la session</div>
          <div class="inst-row"><i class="fa-solid fa-ban"></i> Copier/coller et clic droit désactivés</div>
          <div class="inst-row"><i class="fa-solid fa-clock-rotate-left"></i> Temps global limité — gérez votre rythme</div>
        </div>

        <!-- BOUTON LANCER -->
        <button
          @click="startExam"
          class="btn-launch"
          :disabled="loading"
        >
          <span v-if="loading" class="spin-inline"></span>
          <span v-else>
            <i class="fa-solid fa-rocket me-2"></i>LANCER LA SESSION
          </span>
        </button>

        <div v-if="loadError" class="load-error">
          <i class="fa-solid fa-triangle-exclamation me-2"></i>{{ loadError }}
        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════
         EXAMEN EN COURS
    ══════════════════════════════════════ -->
    <template v-else-if="phase === 'testing'">

      <!-- HEADER FIXE -->
      <header class="exam-header">
        <div class="hdr-brand">Evalua<span>Tech</span></div>
        <div class="hdr-center">
          <div class="global-timer" :class="{ 'timer-warn': timeLeft < 120, 'timer-danger': timeLeft < 60 }">
            <i class="fa-solid fa-clock"></i> {{ formatTime(timeLeft) }}
          </div>
        </div>
        <div class="hdr-right">
          <div
            class="integrity-pill"
            :class="integrityScore >= 70 ? 'integrity-ok' : 'integrity-warn'"
            title="Score d'intégrité Anti-Cheat"
          >
            <i class="fa-solid fa-shield-halved"></i>
            {{ integrityScore }}%
          </div>
          <button @click="confirmFinish" class="btn-end-exam">
            <i class="fa-solid fa-flag-checkered me-1"></i> TERMINER
          </button>
        </div>
      </header>

      <!-- BARRE PROGRESSION GLOBALE -->
      <div class="global-prog-bar">
        <div
          class="global-prog-fill"
          :style="{ width: ((currentIndex + 1) / questions.length) * 100 + '%' }"
        ></div>
      </div>

      <!-- NAVIGATEUR PILLS -->
      <div class="q-navigator">
        <button
          v-for="(q, i) in questions"
          :key="i"
          class="nav-pill"
          :class="{
            'np-active': i === currentIndex,
            'np-answered': answers[i]?.valeur,
            'np-timeout': q.isTimedOut
          }"
          @click="jumpTo(i)"
        >{{ i + 1 }}</button>
      </div>

      <!-- MINUTEUR INDIVIDUEL -->
      <div v-if="currentQ?.dureeSecondes > 0 && !currentQ?.isTimedOut" class="q-timer-strip">
        <div class="qt-meta">
          <span>TEMPS SEGMENT</span>
          <span class="qt-val" :class="{ 'qt-danger': qTimer < 15 }">{{ formatTime(qTimer) }}</span>
        </div>
        <div class="qt-track">
          <div class="qt-fill" :style="qTimerStyle"></div>
        </div>
      </div>

      <!-- CORPS QUESTION -->
      <div class="exam-body" :class="{ 'has-qtimer': currentQ?.dureeSecondes > 0 && !currentQ?.isTimedOut }">
        <Transition name="q-slide" mode="out-in">
          <div :key="currentIndex" class="question-card" :class="{ 'q-locked': currentQ?.isTimedOut }">

            <div class="q-meta-row">
              <span class="q-num">Q{{ currentIndex + 1 }} / {{ questions.length }}</span>
              <div class="q-badges">
                <span v-if="currentQ?.points" class="badge-pts">{{ currentQ.points }} pts</span>
                <span class="badge-type">{{ typeLabel(currentQ?.type) }}</span>
                <span v-if="currentQ?.theme" class="badge-theme">{{ currentQ.theme }}</span>
              </div>
            </div>

            <h2 class="q-text">{{ currentQ?.enonce }}</h2>

            <div class="q-interaction" :class="{ 'q-disabled': currentQ?.isTimedOut }">

              <!-- QCU / QCM -->
              <div v-if="isChoixType(currentQ?.type)" class="opts-list">
                <div
                  v-for="(opt, oi) in currentQ?.choix || []"
                  :key="oi"
                  class="opt-item"
                  :class="{ 'opt-selected': isSelected(oi) }"
                  @click="selectOpt(oi)"
                >
                  <div class="opt-letter">{{ String.fromCharCode(65 + oi) }}</div>
                  <div class="opt-text">{{ opt }}</div>
                  <div v-if="isSelected(oi)" class="opt-check"><i class="fa-solid fa-check"></i></div>
                </div>
              </div>

              <!-- VRAI / FAUX -->
              <div v-else-if="isVFType(currentQ?.type)" class="vf-zone">
                <button
                  class="vf-btn"
                  :class="{ 'vf-selected': answers[currentIndex]?.valeur === 'Vrai' }"
                  @click="selectVF('Vrai')"
                >
                  <i class="fa-solid fa-check fa-lg"></i> VRAI
                </button>
                <button
                  class="vf-btn vf-false"
                  :class="{ 'vf-selected': answers[currentIndex]?.valeur === 'Faux' }"
                  @click="selectVF('Faux')"
                >
                  <i class="fa-solid fa-xmark fa-lg"></i> FAUX
                </button>
              </div>

              <!-- TEXTE LIBRE -->
              <div v-else class="text-zone">
                <textarea
                  v-model="answers[currentIndex].valeur"
                  @blur="saveAnswer"
                  placeholder="Saisissez votre réponse ici..."
                  class="text-answer"
                ></textarea>
              </div>
            </div>

            <!-- BADGE TIMEOUT -->
            <div v-if="currentQ?.isTimedOut" class="timeout-badge">
              <i class="fa-solid fa-hourglass-end me-2"></i>Temps écoulé — passage automatique
            </div>

            <!-- NAVIGATION -->
            <footer class="q-footer">
              <button
                @click="prevQ"
                :disabled="currentIndex === 0"
                class="btn-nav"
              >
                <i class="fa-solid fa-arrow-left me-1"></i> PRÉCÉDENT
              </button>
              <div class="q-answered-info">
                <i class="fa-solid fa-circle-check me-1" style="color:#10b981"></i>
                {{ answeredCount }} / {{ questions.length }} répondues
              </div>
              <button
                v-if="currentIndex < questions.length - 1"
                @click="nextQ"
                class="btn-nav btn-nav-next"
              >
                SUIVANT <i class="fa-solid fa-arrow-right ms-1"></i>
              </button>
              <button
                v-else
                @click="confirmFinish"
                class="btn-nav btn-nav-finish"
              >
                <i class="fa-solid fa-flag-checkered me-1"></i> FINALISER
              </button>
            </footer>
          </div>
        </Transition>
      </div>
    </template>

    <!-- ══════════════════════════════════════
         RÉSULTATS FINAUX
    ══════════════════════════════════════ -->
    <div v-else-if="phase === 'results'" class="phase-center p-4">
      <div class="results-wrapper animate-in">

        <!-- HEADER RÉSULTATS -->
        <div class="results-hero">
          <div class="brand-logo-sm">Evalua<span>Tech</span></div>
          <div class="result-badge" :class="isPassed ? 'rb-pass' : 'rb-fail'">
            <i :class="isPassed ? 'fa-solid fa-award' : 'fa-solid fa-rotate'"></i>
            {{ isPassed ? 'SESSION VALIDÉE' : 'SESSION ÉCHOUÉE' }}
          </div>
        </div>

        <!-- GRILLE BENTO -->
        <div class="bento-grid">

          <!-- SCORE CERCLE -->
          <div class="bento-card bento-score">
            <span class="bento-lbl">SCORE FINAL</span>
            <div class="score-ring-wrap">
              <svg viewBox="0 0 140 140" width="200" height="200">
                <circle cx="70" cy="70" r="56" class="ring-bg"/>
                <circle
                  cx="70" cy="70" r="56"
                  class="ring-fill"
                  :stroke="isPassed ? '#10b981' : '#f43f5e'"
                  :style="ringStyle"
                />
                <text x="70" y="65" text-anchor="middle" class="ring-pct">{{ results.pourcentage }}%</text>
                <text x="70" y="82" text-anchor="middle" class="ring-pts">{{ results.scoreTotal }} pts</text>
              </svg>
            </div>
            <div class="result-status-pill" :class="isPassed ? 'pill-pass' : 'pill-fail'">
              <i :class="isPassed ? 'fa-solid fa-check me-1' : 'fa-solid fa-xmark me-1'"></i>
              {{ isPassed ? 'ADMIS' : 'ÉCHEC' }}
            </div>
            <span class="seuil-info">Seuil requis : {{ examMeta?.scoreReussite || 70 }}%</span>
          </div>

          <!-- MÉTRIQUES -->
          <div class="bento-card bento-metrics">
            <h4 class="bento-title">MÉTRIQUES DE SESSION</h4>

            <div class="metric-item">
              <div class="metric-info">
                <span><i class="fa-solid fa-star me-2" style="color:#f59e0b"></i>Score obtenu</span>
                <strong>{{ results.pourcentage }}%</strong>
              </div>
              <div class="metric-bar">
                <div class="mbar-fill mbar-amber" :style="{ width: results.pourcentage + '%' }"></div>
              </div>
            </div>

            <div class="metric-item">
              <div class="metric-info">
                <span><i class="fa-solid fa-shield-halved me-2" style="color:#6366f1"></i>Intégrité</span>
                <strong>{{ integrityScore }}%</strong>
              </div>
              <div class="metric-bar">
                <div class="mbar-fill mbar-indigo" :style="{ width: integrityScore + '%' }"></div>
              </div>
            </div>

            <div class="metric-item">
              <div class="metric-info">
                <span><i class="fa-solid fa-check-circle me-2" style="color:#10b981"></i>Correctes</span>
                <strong>{{ correctCount }} / {{ results.detailedCorrection?.length || 0 }}</strong>
              </div>
              <div class="metric-bar">
                <div
                  class="mbar-fill mbar-green"
                  :style="{ width: (correctCount / Math.max(results.detailedCorrection?.length || 1, 1)) * 100 + '%' }"
                ></div>
              </div>
            </div>

            <div class="metric-item">
              <div class="metric-info">
                <span><i class="fa-solid fa-clock me-2 text-muted"></i>Temps utilisé</span>
                <strong>{{ formatTime(timeUsed) }}</strong>
              </div>
              <div class="metric-bar">
                <div
                  class="mbar-fill mbar-gray"
                  :style="{ width: Math.min((timeUsed / ((examMeta?.dureeMinutes || 60) * 60)) * 100, 100) + '%' }"
                ></div>
              </div>
            </div>

            <!-- SCORE ANTI-CHEAT -->
            <div class="anticheat-result-box" :class="integrityScore >= 70 ? 'ac-result-ok' : 'ac-result-warn'">
              <i class="fa-solid fa-shield-halved me-2"></i>
              <div>
                <strong>Anti-Cheat v2.0</strong>
                <span>{{ security.infractions }} infraction(s) — Intégrité : {{ integrityScore }}%</span>
              </div>
            </div>

            <!-- RÉPARTITION -->
            <div class="repartition-row">
              <div class="rep-item rep-correct">
                <div class="rep-icon"><i class="fa-solid fa-check"></i></div>
                <span class="rep-val">{{ correctCount }}</span>
                <span class="rep-lbl">Correctes</span>
              </div>
              <div class="rep-item rep-incorrect">
                <div class="rep-icon"><i class="fa-solid fa-xmark"></i></div>
                <span class="rep-val">{{ incorrectCount }}</span>
                <span class="rep-lbl">Incorrectes</span>
              </div>
              <div class="rep-item rep-skipped">
                <div class="rep-icon"><i class="fa-solid fa-minus"></i></div>
                <span class="rep-val">{{ skippedCount }}</span>
                <span class="rep-lbl">Ignorées</span>
              </div>
            </div>

            <button @click="phase = 'review'" class="btn-voir-correction">
              <i class="fa-solid fa-magnifying-glass me-2"></i>VOIR LA CORRECTION
            </button>
          </div>

          <!-- ANALYSE PAR THÈME -->
          <div v-if="themeBreakdown.length > 0" class="bento-card bento-themes">
            <h4 class="bento-title">ANALYSE PAR THÈME</h4>
            <div v-for="th in themeBreakdown" :key="th.name" class="theme-row">
              <div class="theme-info">
                <span class="theme-name">{{ th.name }}</span>
                <span class="theme-score" :class="th.pct >= 70 ? 'text-ok' : 'text-warn'">
                  {{ th.correct }}/{{ th.total }} · {{ th.pct }}%
                </span>
              </div>
              <div class="theme-track">
                <div
                  class="theme-fill"
                  :style="{ width: th.pct + '%', background: th.pct >= 70 ? '#10b981' : '#f43f5e' }"
                ></div>
              </div>
            </div>
          </div>

          <!-- ANALYSE IA + COACH -->
          <div class="bento-card bento-coach">
            <div class="coach-avatar">
              <i class="fa-solid fa-robot"></i>
            </div>
            <h4 class="bento-title mt-3">ANALYSE IA</h4>
            <p class="coach-msg">{{ coachMessage }}</p>
            <div class="result-tags">
              <span v-for="tag in resultTags" :key="tag" class="result-tag">{{ tag }}</span>
            </div>
            <!-- NOTIFICATION EMAIL -->
            <div v-if="emailNotifSent" class="email-notif-sent">
              <i class="fa-solid fa-envelope-circle-check me-2"></i>
              Confirmation envoyée par email
            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- ══════════════════════════════════════
         CORRECTION DÉTAILLÉE
    ══════════════════════════════════════ -->
    <div v-else-if="phase === 'review'" class="review-portal animate-in">
      <nav class="review-nav">
        <div class="brand-sm">Evalua<span>Tech</span> <span class="review-label">| CORRECTION</span></div>
        <div class="review-nav-right">
          <span class="result-pill" :class="isPassed ? 'pill-pass' : 'pill-fail'">
            {{ results.pourcentage }}% — {{ isPassed ? 'ADMIS' : 'ÉCHEC' }}
          </span>
          <button @click="phase = 'results'" class="btn-back-review">
            <i class="fa-solid fa-arrow-left me-2"></i>RETOUR
          </button>
        </div>
      </nav>

      <!-- FILTRES -->
      <div class="review-filters">
        <button
          v-for="f in reviewFilterDefs"
          :key="f.val"
          class="rf-btn"
          :class="{ 'rf-active': reviewFilter === f.val }"
          @click="reviewFilter = f.val"
        >
          <span class="rf-dot" :class="'rfd-' + f.val"></span>
          {{ f.label }} ({{ f.count }})
        </button>
      </div>

      <div class="review-scroll">
        <div class="review-list">
          <div
            v-for="(item, idx) in filteredCorrection"
            :key="idx"
            class="correction-card"
            :class="item.isCorrect ? 'cc-correct' : (item.userAnswer ? 'cc-incorrect' : 'cc-skipped')"
          >
            <div class="cc-header">
              <div class="cc-header-left">
                <span class="cc-num">QUESTION {{ item.originalIndex + 1 }}</span>
                <span v-if="item.theme" class="cc-theme">{{ item.theme }}</span>
                <span v-if="item.points" class="cc-pts">{{ item.points }} pts</span>
              </div>
              <span class="cc-status" :class="item.isCorrect ? 'cc-s-correct' : (item.userAnswer ? 'cc-s-incorrect' : 'cc-s-skipped')">
                <i :class="item.isCorrect ? 'fa-solid fa-check me-1' : (item.userAnswer ? 'fa-solid fa-xmark me-1' : 'fa-solid fa-minus me-1')"></i>
                {{ item.isCorrect ? 'CORRECT' : (item.userAnswer ? 'INCORRECT' : 'IGNORÉ') }}
              </span>
            </div>

            <h3 class="cc-enonce">{{ item.enonce }}</h3>

            <!-- OPTIONS QCU/QCM -->
            <div v-if="item.options && item.options.length > 0" class="cc-opts">
              <div
                v-for="(opt, oi) in item.options"
                :key="oi"
                class="cc-opt"
                :class="{
                  'cco-correct': item.correctIndexes?.includes(oi),
                  'cco-user': item.userIndexes?.includes(oi) && !item.correctIndexes?.includes(oi),
                  'cco-user-correct': item.userIndexes?.includes(oi) && item.correctIndexes?.includes(oi)
                }"
              >
                <div class="cco-letter">{{ String.fromCharCode(65 + oi) }}</div>
                <div class="cco-text">{{ opt }}</div>
                <i v-if="item.correctIndexes?.includes(oi)" class="fa-solid fa-check" style="color:#10b981"></i>
                <i v-else-if="item.userIndexes?.includes(oi)" class="fa-solid fa-xmark" style="color:#f43f5e"></i>
              </div>
            </div>

            <!-- TEXTE -->
            <div v-else class="cc-text-compare">
              <div class="cctc-block" :class="item.isCorrect ? 'cctc-ok' : 'cctc-user'">
                <label>VOTRE RÉPONSE</label>
                <p>{{ item.userAnswer || 'AUCUNE RÉPONSE' }}</p>
              </div>
              <div v-if="!item.isCorrect" class="cctc-block cctc-ok">
                <label>RÉPONSE CORRECTE</label>
                <p>{{ item.correctAnswer }}</p>
              </div>
            </div>

            <!-- EXPLICATION -->
            <div v-if="item.explication" class="cc-explication">
              <div class="cc-exp-header">
                <i class="fa-solid fa-lightbulb me-2" style="color:#f59e0b"></i>
                EXPLICATION
              </div>
              <p class="cc-exp-text">{{ item.explication }}</p>
            </div>
          </div>

          <div v-if="filteredCorrection.length === 0" class="review-empty">
            <i class="fa-solid fa-check-double fa-2x mb-3" style="color:#10b981"></i>
            <p>Aucune question dans cette catégorie.</p>
          </div>
        </div>
      </div>
    </div>

    <!-- TOAST GLOBAL -->
    <Transition name="toast-pop">
      <div v-if="toast.active" class="global-toast" :class="'t-' + toast.type">
        <i :class="toast.icon"></i>
        <div class="toast-body">
          <strong>SYSTÈME</strong>
          <p>{{ toast.message }}</p>
        </div>
      </div>
    </Transition>

    <!-- CONFIRM DIALOG -->
    <Transition name="scale-fade">
      <div v-if="confirmDialog.show" class="confirm-overlay" @click.self="confirmDialog.show = false">
        <div class="confirm-card">
          <i :class="confirmDialog.icon" class="confirm-icon"></i>
          <h4>{{ confirmDialog.title }}</h4>
          <p>{{ confirmDialog.message }}</p>
          <div class="confirm-actions">
            <button @click="confirmDialog.show = false" class="btn-cancel">ANNULER</button>
            <button @click="runConfirm" class="btn-confirm-ok">CONFIRMER</button>
          </div>
        </div>
      </div>
    </Transition>
  </div>
</template>

<script setup>
import { ref, computed, reactive, onMounted, onUnmounted } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/services/api';

const route = useRoute();

/* ─── ÉTAT PRINCIPAL ─────────────────────────────────────────── */
const phase          = ref('lobby');  // lobby | testing | results | review
const questions      = ref([]);
const answers        = ref([]);
const currentIndex   = ref(0);
const timeLeft       = ref(0);
const timeUsed       = ref(0);
const qTimer         = ref(0);
const examMeta       = ref(null);
const loading        = ref(false);
const loadError      = ref('');
const reviewFilter   = ref('all');
const emailNotifSent = ref(false);

// ✅ FIX : Stocker l'evaluationId séparément (distinct du candidatureId)
const evaluationId = ref(null);

const results = ref({
  scoreTotal: 0,
  pourcentage: 0,
  detailedCorrection: [],
});

/* ─── SÉCURITÉ ANTI-CHEAT ────────────────────────────────────── */
const security = reactive({
  infractions: 0,
  overlayVisible: false,
  lockTimer: 0,
  active: false,
  _lockInterval: null,
});

/* ─── TIMERS ─────────────────────────────────────────────────── */
let globalInterval   = null;
let questionInterval = null;
let _startTime       = null;
let _toastTimer      = null;

/* ─── TOAST ──────────────────────────────────────────────────── */
const toast = reactive({ active: false, message: '', type: 'success', icon: '' });

/* ─── CONFIRM ────────────────────────────────────────────────── */
const confirmDialog = reactive({ show: false, title: '', message: '', icon: '', _cb: null });

/* ─── COMPUTED ───────────────────────────────────────────────── */
const currentQ = computed(() => questions.value[currentIndex.value]);

const integrityScore = computed(() => Math.max(0, 100 - security.infractions * 10));

const isPassed = computed(() =>
  results.value.pourcentage >= (examMeta.value?.scoreReussite || 70)
);

const answeredCount = computed(() =>
  answers.value.filter(a => a?.valeur && a.valeur !== '').length
);

const correctCount = computed(() =>
  results.value.detailedCorrection?.filter(q => q.isCorrect).length || 0
);
const incorrectCount = computed(() =>
  results.value.detailedCorrection?.filter(q => !q.isCorrect && q.userAnswer).length || 0
);
const skippedCount = computed(() =>
  results.value.detailedCorrection?.filter(q => !q.userAnswer).length || 0
);

const ringStyle = computed(() => {
  const circ = 2 * Math.PI * 56;
  const fill = (results.value.pourcentage / 100) * circ;
  return {
    strokeDasharray: `${fill} ${circ}`,
    strokeDashoffset: circ * 0.25,
    transition: 'stroke-dasharray 1.5s ease-out',
  };
});

const qTimerStyle = computed(() => {
  const dur = currentQ.value?.dureeSecondes || 1;
  const pct = (qTimer.value / dur) * 100;
  return {
    width: `${pct}%`,
    background: pct < 25 ? '#f43f5e' : pct < 50 ? '#f59e0b' : '#10b981',
    transition: 'width 1s linear',
  };
});

const themeBreakdown = computed(() => {
  const map = {};
  (results.value.detailedCorrection || []).forEach(q => {
    const t = q.theme || 'Général';
    if (!map[t]) map[t] = { name: t, total: 0, correct: 0 };
    map[t].total++;
    if (q.isCorrect) map[t].correct++;
  });
  return Object.values(map).map(t => ({
    ...t,
    pct: Math.round((t.correct / t.total) * 100),
  }));
});

const coachMessage = computed(() => {
  const pct = results.value.pourcentage;
  if (pct >= 90) return "Excellente performance ! Maîtrise experte démontrée sur l'ensemble des modules. Votre profil est hautement qualifié.";
  if (pct >= 70) return "Bonne performance. Session validée avec succès. Quelques axes d'amélioration identifiés sur les questions avancées.";
  if (pct >= 50) return "Performance intermédiaire. Des lacunes détectées sur certains modules. Une révision ciblée est recommandée.";
  return "Performance insuffisante. Une préparation approfondie sur les fondamentaux est nécessaire avant de retenter.";
});

const resultTags = computed(() => {
  const tags = [];
  if (integrityScore.value === 100) tags.push('Intégrité parfaite');
  if (results.value.pourcentage >= 90) tags.push('Expert certifié');
  else if (results.value.pourcentage >= 70) tags.push('Standard validé');
  if (correctCount.value === (results.value.detailedCorrection?.length || 0) && correctCount.value > 0) tags.push('Score parfait');
  if (skippedCount.value === 0) tags.push('Aucune question ignorée');
  return tags;
});

const reviewFilterDefs = computed(() => [
  { val: 'all',       label: 'Toutes',      count: results.value.detailedCorrection?.length || 0 },
  { val: 'correct',   label: 'Correctes',   count: correctCount.value },
  { val: 'incorrect', label: 'Incorrectes', count: incorrectCount.value },
  { val: 'skipped',   label: 'Ignorées',    count: skippedCount.value },
]);

const filteredCorrection = computed(() => {
  const list = (results.value.detailedCorrection || []).map((item, i) => ({
    ...item,
    originalIndex: i,
  }));
  if (reviewFilter.value === 'correct')   return list.filter(q => q.isCorrect);
  if (reviewFilter.value === 'incorrect') return list.filter(q => !q.isCorrect && q.userAnswer);
  if (reviewFilter.value === 'skipped')   return list.filter(q => !q.userAnswer);
  return list;
});

/* ─── HELPERS TYPE ───────────────────────────────────────────── */
// ✅ FIX : Helpers centralisés pour éviter les erreurs de comparaison de types
const isChoixType = (type) => [0, 1, '0', '1', 'QCU', 'QCM'].includes(type);
const isVFType    = (type) => type === 2 || type === '2' || type === 'VF' || type === 'VRAI_FAUX' || type === 'VRAIFAUX';

/* ─── CHARGEMENT LOBBY ───────────────────────────────────────── */
onMounted(async () => {
  // ✅ FIX : Utiliser candidatureId de façon cohérente
  const candidatureId = getCandidatureId();
  try {
    const res = await api.get(`/Examen/info/${candidatureId}`);
    const d = res.data;
    const questionsWithTimer = d.questions?.filter(q => q.dureeSecondes > 0).length || 0;
    examMeta.value = {
      titre:               d.titre || 'Examen',
      totalQuestions:      d.totalQuestions || '—',
      dureeMinutes:        d.dureeMinutes || Math.floor((d.tempsLimite || 3600) / 60),
      scoreReussite:       d.scoreReussite || 70,
      theme:               d.theme || '',
      anticheatEnabled:    d.anticheatEnabled !== false,
      notificationsEnabled: d.sendNotifications || false,
      questionsWithTimer,
    };
  } catch {
    examMeta.value = {
      titre: "Session d'examen",
      totalQuestions: '—',
      dureeMinutes: 60,
      scoreReussite: 70,
      theme: '',
      anticheatEnabled: true,
      notificationsEnabled: false,
      questionsWithTimer: 0,
    };
  }
});

onUnmounted(() => {
  clearInterval(globalInterval);
  clearInterval(questionInterval);
  disableSecurity();
});

/* ─── DÉMARRAGE EXAMEN ───────────────────────────────────────── */
const startExam = async () => {
  loading.value  = true;
  loadError.value = '';
  try {
    const candidatureId = getCandidatureId();
    const res = await api.get(`/Examen/setup/${candidatureId}`);
    const d = res.data;

    // ✅ FIX : Stocker l'evaluationId retourné par le backend
    evaluationId.value = d.evaluationId;

    const questionsWithTimer = (d.questions || []).filter(q => (q.dureeSecondes || 0) > 0).length;
    examMeta.value = {
      titre:               d.questionnaire?.titre || d.titre || 'Examen',
      totalQuestions:      d.questions?.length || 0,
      dureeMinutes:        d.tempsLimite ? Math.floor(d.tempsLimite / 60) : d.dureeMinutes || 60,
      scoreReussite:       d.scoreReussite || 70,
      theme:               d.questionnaire?.theme || d.theme || '',
      anticheatEnabled:    d.anticheatEnabled !== false,
      notificationsEnabled: d.sendNotifications || false,
      questionsWithTimer,
    };

    questions.value = (d.questions || []).map(q => ({
      ...q,
      isTimedOut: false,
      // ✅ FIX : Normaliser le tableau choix — accepter choix ou options
      choix: Array.isArray(q.choix) ? q.choix : (Array.isArray(q.options) ? q.options : []),
      points: q.points || q.poids || 1,
    }));

    timeLeft.value = d.tempsLimite || examMeta.value.dureeMinutes * 60;
    // ✅ FIX : Initialiser toutes les réponses avec questionId correct
    answers.value  = questions.value.map(q => ({ questionId: q.id, valeur: '' }));
    _startTime     = Date.now();

    phase.value = 'testing';

    try { document.documentElement.requestFullscreen(); } catch {}

    if (examMeta.value.anticheatEnabled) {
      enableSecurity();
    }

    startGlobalClock();
    startQClock();

  } catch (err) {
    loadError.value = err?.response?.data?.message || 'Erreur de chargement. Veuillez réessayer.';
  } finally {
    loading.value = false;
  }
};

/* ─── ANTI-CHEAT ─────────────────────────────────────────────── */
const enableSecurity = () => {
  security.active = true;
  window.addEventListener('blur', onBlur);
  document.addEventListener('visibilitychange', onVisibility);
  window.addEventListener('beforeunload', onBeforeUnload);
};

const disableSecurity = () => {
  security.active = false;
  window.removeEventListener('blur', onBlur);
  document.removeEventListener('visibilitychange', onVisibility);
  window.removeEventListener('beforeunload', onBeforeUnload);
};

const onBlur = () => {
  if (phase.value !== 'testing' || security.overlayVisible) return;
  triggerBreach();
};

const onVisibility = () => {
  if (document.hidden && phase.value === 'testing') triggerBreach();
};

const onBeforeUnload = (e) => {
  if (phase.value === 'testing') { e.preventDefault(); e.returnValue = ''; }
};

const triggerBreach = () => {
  if (phase.value !== 'testing' || security.overlayVisible) return;
  security.infractions++;

  if (security.infractions >= 5) {
    showToast('5 infractions : session clôturée automatiquement.', 'error', 'fa-solid fa-ban');
    setTimeout(() => finishExam(), 2000);
    return;
  }

  security.overlayVisible = true;
  security.lockTimer = 5;

  clearInterval(security._lockInterval);
  security._lockInterval = setInterval(() => {
    security.lockTimer--;
    if (security.lockTimer <= 0) {
      clearInterval(security._lockInterval);
    }
  }, 1000);

  showToast(`Infraction anti-cheat détectée (${security.infractions}/5)`, 'warn', 'fa-solid fa-shield-halved');
};

const resumeExam = () => {
  if (security.lockTimer > 0) return;
  security.overlayVisible = false;
  try { document.documentElement.requestFullscreen(); } catch {}
};

/* ─── MINUTEURS ──────────────────────────────────────────────── */
const startGlobalClock = () => {
  clearInterval(globalInterval);
  globalInterval = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      clearInterval(globalInterval);
      showToast('Temps global écoulé — soumission automatique.', 'warn', 'fa-solid fa-hourglass-end');
      finishExam();
    }
  }, 1000);
};

const startQClock = () => {
  clearInterval(questionInterval);
  const dur = currentQ.value?.dureeSecondes || 0;
  // ✅ FIX : Ne pas lancer le timer si déjà expiré ou si durée = 0
  if (dur > 0 && !currentQ.value?.isTimedOut) {
    qTimer.value = dur;
    questionInterval = setInterval(() => {
      if (qTimer.value > 0) {
        qTimer.value--;
      } else {
        clearInterval(questionInterval);
        handleQTimeout();
      }
    }, 1000);
  } else {
    qTimer.value = 0;
  }
};

const handleQTimeout = () => {
  if (currentQ.value) currentQ.value.isTimedOut = true;
  saveAnswer();
  showToast('Temps écoulé sur ce segment.', 'warn', 'fa-solid fa-hourglass-end');
  setTimeout(() => {
    if (currentIndex.value < questions.value.length - 1) nextQ();
  }, 1500);
};

/* ─── NAVIGATION QUESTIONS ───────────────────────────────────── */
const nextQ = () => {
  if (currentIndex.value < questions.value.length - 1) {
    currentIndex.value++;
    startQClock();
    window.scrollTo(0, 0);
  }
};
const prevQ = () => {
  if (currentIndex.value > 0) {
    currentIndex.value--;
    startQClock();
    window.scrollTo(0, 0);
  }
};
// ✅ FIX : Permettre la navigation libre même sur les questions timeout
const jumpTo = (i) => {
  currentIndex.value = i;
  startQClock();
};

/* ─── SÉLECTION RÉPONSES ─────────────────────────────────────── */
const selectOpt = (idx) => {
  if (currentQ.value?.isTimedOut) return;
  const ans = answers.value[currentIndex.value];
  const isQCM = currentQ.value?.type === 1 || currentQ.value?.type === 'QCM';

  if (isQCM) {
    let list = ans.valeur ? ans.valeur.split(';') : [];
    const key = idx.toString();
    list.includes(key) ? (list = list.filter(x => x !== key)) : list.push(key);
    ans.valeur = list.join(';');
  } else {
    ans.valeur = idx.toString();
  }
  saveAnswer();
};

const selectVF = (val) => {
  if (currentQ.value?.isTimedOut) return;
  answers.value[currentIndex.value].valeur = val;
  saveAnswer();
};

const isSelected = (idx) =>
  (answers.value[currentIndex.value]?.valeur || '').split(';').includes(idx.toString());

/* ─── SAUVEGARDE RÉPONSE ─────────────────────────────────────── */
const saveAnswer = async () => {
  // ✅ FIX : Vérifier que evaluationId est disponible avant d'appeler l'API
  if (!evaluationId.value) return;
  const ans = answers.value[currentIndex.value];
  if (!ans) return;
  try {
    await api.post('/Examen/save-response', {
      evaluationId: evaluationId.value,  // ✅ FIX : Utiliser evaluationId stocké, pas candidatureId
      questionId:   currentQ.value?.id,
      valeur:       ans.valeur || '',
    });
  } catch { /* silencieux */ }
};

/* ─── FIN EXAMEN ─────────────────────────────────────────────── */
const confirmFinish = () => {
  const unanswered = questions.value.length - answeredCount.value;
  showConfirmDialog(
    'Terminer la session ?',
    unanswered > 0
      ? `${unanswered} question(s) sans réponse. Confirmer la soumission ?`
      : "Valider toutes vos réponses et clôturer l'examen ?",
    'fa-solid fa-flag-checkered',
    finishExam
  );
};

const finishExam = async () => {
  clearInterval(globalInterval);
  clearInterval(questionInterval);
  disableSecurity();
  timeUsed.value = _startTime ? Math.floor((Date.now() - _startTime) / 1000) : 0;

  // ✅ FIX : Utiliser evaluationId (et non candidatureId) pour terminer/results
  const evalId = evaluationId.value;
  if (!evalId) {
    computeLocalResults();
    return;
  }

  try {
    await api.post(`/Examen/terminer/${evalId}`);
    const res = await api.get(`/Examen/results/${evalId}`);
    const raw = res.data;

    // Enrichissement correction
    const correction = (raw.detailedCorrection || []).map((item, idx) => {
      const q = questions.value[idx];
      const choix = q?.choix || [];
      const userRaw = item.userAnswer || '';

      // ✅ FIX : Détecter si la réponse est par index (numérique) ou par texte
      let userIndexes = [];
      const parts = userRaw.split(';').map(s => s.trim()).filter(Boolean);
      const allNumeric = parts.every(p => !isNaN(p));
      if (allNumeric && parts.length > 0) {
        userIndexes = parts.map(Number).filter(n => !isNaN(n) && n >= 0 && n < choix.length);
      }

      // ✅ FIX : Calculer les index corrects depuis le texte BonneReponse
      let correctIndexes = [];
      const correctRaw = item.correctAnswer || '';
      if (choix.length > 0 && correctRaw) {
        const correctTexts = correctRaw.split('|').map(s => s.trim().toLowerCase());
        correctIndexes = choix
          .map((c, i) => correctTexts.includes(c.trim().toLowerCase()) ? i : -1)
          .filter(i => i !== -1);
      }

      return {
        ...item,
        options:        choix,
        userIndexes,
        correctIndexes,
        theme:          item.theme || q?.theme || 'Général',
        points:         item.points || q?.points || 1,
        explication:    item.explication || q?.explication || '',
      };
    });

    results.value = { ...raw, detailedCorrection: correction };
    phase.value = 'results';

    // ── ENVOI NOTIFICATION EMAIL ──
    if (examMeta.value?.notificationsEnabled) {
      try {
        await api.post('/Examen/notify-result', {
          evaluationId: evalId,
          pourcentage:  results.value.pourcentage,
          passed:       isPassed.value,
          integrityScore: integrityScore.value,
        });
        emailNotifSent.value = true;
        showToast('Confirmation envoyée par email.', 'success', 'fa-solid fa-envelope-circle-check');
      } catch { /* silencieux */ }
    }

    try { if (document.fullscreenElement) document.exitFullscreen(); } catch {}

  } catch {
    computeLocalResults();
  }
};

/* ─── CALCUL LOCAL (fallback si API indisponible) ────────────── */
const computeLocalResults = () => {
  let totalPts = 0;
  let maxPts   = 0;

  const correction = questions.value.map((q, idx) => {
    const ans    = answers.value[idx]?.valeur || '';
    const choix  = q.choix || [];
    const bonneR = q.bonneReponse || '';
    const pts    = q.points || 1;
    maxPts      += pts;

    const correctTexts = bonneR.split('|').map(s => s.trim().toLowerCase()).filter(Boolean);
    let isCorrect = false;

    if (choix.length > 0) {
      const userIdxs    = ans.split(';').map(Number).filter(n => !isNaN(n));
      const correctIdxs = choix.map((c, i) => correctTexts.includes(c.trim().toLowerCase()) ? i : -1).filter(i => i !== -1);
      isCorrect = JSON.stringify(userIdxs.sort()) === JSON.stringify(correctIdxs.sort());
    } else {
      isCorrect = ans.trim().toLowerCase() === bonneR.trim().toLowerCase();
    }

    if (isCorrect) totalPts += pts;

    const userAnswer = choix.length > 0
      ? ans.split(';').map(Number).filter(n => !isNaN(n) && choix[n]).map(n => choix[n]).join(', ')
      : ans;

    return {
      enonce:         q.enonce || q.texte || '',
      userAnswer,
      correctAnswer:  bonneR,
      isCorrect,
      options:        choix,
      userIndexes:    ans.split(';').map(Number).filter(n => !isNaN(n)),
      correctIndexes: choix.map((c, i) => correctTexts.includes(c.trim().toLowerCase()) ? i : -1).filter(i => i !== -1),
      theme:          q.theme || 'Général',
      points:         pts,
      explication:    q.explication || bonneR,
    };
  });

  const pourcentage = maxPts > 0 ? Math.round((totalPts / maxPts) * 100) : 0;
  results.value = { scoreTotal: totalPts, pourcentage, detailedCorrection: correction };
  phase.value = 'results';
  try { if (document.fullscreenElement) document.exitFullscreen(); } catch {}
};

/* ─── HELPERS ────────────────────────────────────────────────── */
// ✅ FIX : Nom unifié getCandidatureId (utilisé partout sauf finishExam qui utilise evaluationId)
const getCandidatureId = () => route.params.id || route.params.candidatureId || route.params.campaignId;

const formatTime = (s) => {
  if (!s || s < 0) return '0:00';
  const m = Math.floor(s / 60);
  const sec = (s % 60).toString().padStart(2, '0');
  return `${m}:${sec}`;
};

const typeLabel = (type) => {
  const map = { 
    0: 'QCU', '0': 'QCU', QCU: 'QCU', 
    1: 'QCM', '1': 'QCM', QCM: 'QCM', 
    2: 'VRAI/FAUX', '2': 'VRAI/FAUX', VRAI_FAUX: 'VRAI/FAUX', VRAIFAUX: 'VRAI/FAUX', VF: 'VRAI/FAUX', 
    4: 'TEXTE', '4': 'TEXTE', TEXTE: 'TEXTE', 
    5: 'CODE', '5': 'CODE', CODE: 'CODE' 
  };
  return map[type] ?? 'QCU';
};

const showToast = (message, type = 'success', icon = 'fa-solid fa-check') => {
  clearTimeout(_toastTimer);
  Object.assign(toast, { message, type, icon, active: true });
  _toastTimer = setTimeout(() => { toast.active = false; }, 4500);
};

const showConfirmDialog = (title, message, icon, cb) => {
  Object.assign(confirmDialog, { title, message, icon, _cb: cb, show: true });
};
const runConfirm = () => {
  confirmDialog.show = false;
  if (confirmDialog._cb) confirmDialog._cb();
};
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;600;700;800;900&display=swap');
@import url('https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css');

/* ══ RESET & ROOT ══ */
*, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

.cert-root {
  font-family: 'Plus Jakarta Sans', sans-serif;
  min-height: 100vh;
  background: #fdfdfd;
  color: #0f172a;
  overflow-x: hidden;
  position: relative;
  user-select: none;
}

/* ══ BACKGROUND ══ */
.lux-bg {
  position: fixed; inset: 0; z-index: 0; pointer-events: none; overflow: hidden;
}
.orb {
  position: absolute; border-radius: 50%; filter: blur(120px); opacity: 0.18;
  animation: orb-drift 12s ease-in-out infinite alternate;
}
.orb-1 { width: 700px; height: 700px; background: #fbbf24; top: -200px; right: -150px; }
.orb-2 { width: 500px; height: 500px; background: #6366f1; bottom: -100px; left: -100px; animation-delay: 4s; }
.orb-3 { width: 300px; height: 300px; background: #f9a8d4; top: 50%; left: 50%; transform: translate(-50%,-50%); opacity: 0.1; animation-delay: 8s; }
.grid-dots {
  position: absolute; inset: 0;
  background-image: radial-gradient(#cbd5e1 1px, transparent 1px);
  background-size: 40px 40px; opacity: 0.18;
}
@keyframes orb-drift {
  from { transform: translate(0, 0) scale(1); }
  to   { transform: translate(30px, 20px) scale(1.05); }
}

/* ══ CENTRAGE PHASES ══ */
.phase-center {
  position: relative; z-index: 10; min-height: 100vh;
  display: flex; align-items: center; justify-content: center; padding: 24px;
}
.animate-in { animation: fadeInUp 0.6s cubic-bezier(0.4,0,0.2,1) both; }
@keyframes fadeInUp { from { opacity: 0; transform: translateY(30px); } to { opacity: 1; transform: translateY(0); } }

/* ══ LOBBY CARD ══ */
.lobby-card {
  background: rgba(255,255,255,0.92);
  backdrop-filter: blur(24px);
  border: 1px solid rgba(255,255,255,0.9);
  border-radius: 44px;
  padding: 56px 48px;
  max-width: 620px;
  width: 100%;
  box-shadow: 0 32px 80px rgba(0,0,0,0.06), 0 0 0 1px rgba(245,158,11,0.05);
}

/* BRAND */
.brand-logo { display: flex; align-items: center; gap: 16px; margin-bottom: 20px; }
.brand-name { font-size: 2.4rem; font-weight: 900; letter-spacing: -1.5px; line-height: 1; }
.brand-name span { color: #f59e0b; }
.brand-sub { font-size: 0.55rem; font-weight: 900; letter-spacing: 2px; color: #94a3b8; display: block; margin-top: 4px; }

/* BADGE SESSION */
.session-badge {
  display: inline-flex; align-items: center; gap: 8px;
  background: linear-gradient(135deg, #0f172a, #1e293b);
  color: #f59e0b; padding: 6px 18px; border-radius: 12px;
  font-size: 0.62rem; font-weight: 900; letter-spacing: 2px;
  margin-bottom: 28px;
}
.badge-dot {
  width: 6px; height: 6px; background: #f59e0b; border-radius: 50%;
  animation: pulse-dot 2s infinite;
}
@keyframes pulse-dot { 0%,100% { opacity:1; transform:scale(1); } 50% { opacity:0.4; transform:scale(0.7); } }

/* EXAM INFO */
.exam-info-box {
  background: #f8fafc; border-radius: 22px; padding: 24px; margin-bottom: 24px;
  border: 1px solid #eef2f6;
}
.exam-title { font-size: 1.1rem; font-weight: 800; color: #0f172a; text-align: center; margin-bottom: 18px; }
.exam-meta-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.meta-chip {
  display: flex; align-items: center; gap: 10px; font-size: 0.8rem;
  font-weight: 700; color: #64748b; padding: 8px 12px;
  background: white; border-radius: 12px; border: 1px solid #eef2f6;
}
.meta-chip i { color: #f59e0b; font-size: 0.8rem; }

/* FEATURES LIST */
.features-list { display: flex; flex-direction: column; gap: 10px; margin-bottom: 24px; }
.feature-item {
  display: flex; align-items: center; gap: 14px;
  background: #f8fafc; border: 1.5px solid #eef2f6;
  border-radius: 18px; padding: 14px 18px; transition: 0.2s;
}
.feature-item.feature-active { border-color: rgba(245,158,11,0.3); background: rgba(255,251,235,0.6); }
.feat-icon {
  width: 40px; height: 40px; border-radius: 12px;
  background: #fffbeb; color: #f59e0b;
  display: flex; align-items: center; justify-content: center;
  font-size: 1rem; flex-shrink: 0;
}
.feat-icon.feat-blue { background: #eff6ff; color: #3b82f6; }
.feat-icon.feat-green { background: #ecfdf5; color: #10b981; }
.feat-body { flex: 1; }
.feat-title { font-size: 0.88rem; font-weight: 800; color: #0f172a; display: block; }
.feat-desc { font-size: 0.72rem; color: #94a3b8; font-weight: 600; display: block; }
.feat-status { flex-shrink: 0; }
.feat-on {
  background: #ecfdf5; color: #10b981; font-size: 0.6rem;
  font-weight: 900; padding: 4px 10px; border-radius: 8px; letter-spacing: 0.5px;
}
.feat-off {
  background: #f1f5f9; color: #94a3b8; font-size: 0.6rem;
  font-weight: 900; padding: 4px 10px; border-radius: 8px;
}
.feat-counter-badge {
  background: #eff6ff; color: #3b82f6; font-size: 0.72rem; font-weight: 900;
  padding: 4px 12px; border-radius: 10px; display: flex; align-items: center; gap: 4px;
}
.feat-counter-badge small { font-size: 0.6rem; opacity: 0.7; }

/* INSTRUCTIONS */
.instructions-box { display: flex; flex-direction: column; gap: 8px; margin-bottom: 28px; }
.inst-row {
  display: flex; align-items: center; gap: 12px;
  padding: 10px 16px; background: #f8fafc;
  border-radius: 12px; font-size: 0.8rem; font-weight: 700; color: #475569;
}
.inst-row i { color: #f59e0b; width: 16px; text-align: center; }

/* BOUTON LANCER */
.btn-launch {
  width: 100%; padding: 18px; background: #0f172a; color: white;
  border: none; border-radius: 20px; font-size: 1.05rem; font-weight: 900;
  cursor: pointer; transition: 0.3s; font-family: inherit;
  display: flex; align-items: center; justify-content: center; gap: 8px;
}
.btn-launch:hover:not(:disabled) {
  background: #f59e0b; color: #0f172a;
  transform: translateY(-2px); box-shadow: 0 12px 32px rgba(245,158,11,0.25);
}
.btn-launch:disabled { opacity: 0.5; cursor: not-allowed; }
.spin-inline {
  width: 20px; height: 20px; border: 3px solid rgba(255,255,255,0.3);
  border-top-color: white; border-radius: 50%; animation: spin 0.8s linear infinite;
}
@keyframes spin { to { transform: rotate(360deg); } }

.load-error {
  margin-top: 14px; background: #fff1f2; color: #f43f5e;
  padding: 12px 18px; border-radius: 14px; font-size: 0.8rem; font-weight: 700;
}

/* ══ ANTI-CHEAT OVERLAY ══ */
.anticheat-overlay {
  position: fixed; inset: 0; background: rgba(15,23,42,0.97);
  backdrop-filter: blur(16px); z-index: 9999;
  display: flex; align-items: center; justify-content: center;
}
.anticheat-card {
  background: white; padding: 56px 48px; border-radius: 40px;
  text-align: center; max-width: 520px; width: 90%;
  box-shadow: 0 40px 80px rgba(0,0,0,0.4);
}
.ac-icon-ring {
  width: 80px; height: 80px; border-radius: 50%;
  background: linear-gradient(135deg, #fff1f2, #fecaca);
  border: 3px solid #fca5a5;
  display: flex; align-items: center; justify-content: center;
  font-size: 2rem; color: #ef4444; margin: 0 auto 20px;
  animation: ring-pulse 2s infinite;
}
@keyframes ring-pulse {
  0%,100% { box-shadow: 0 0 0 0 rgba(239,68,68,0.3); }
  50% { box-shadow: 0 0 0 16px rgba(239,68,68,0); }
}
.ac-title { font-size: 1.25rem; font-weight: 900; color: #0f172a; margin-bottom: 12px; }
.ac-desc { color: #64748b; font-size: 0.88rem; font-weight: 600; line-height: 1.6; margin-bottom: 20px; }
.ac-infraction-count {
  background: #fff1f2; border: 1.5px solid #fca5a5;
  border-radius: 14px; padding: 12px 24px;
  display: inline-flex; align-items: baseline; gap: 8px; margin-bottom: 20px;
}
.ac-count-val { font-size: 2rem; font-weight: 900; color: #f43f5e; line-height: 1; }
.ac-count-lbl { font-size: 0.75rem; font-weight: 700; color: #f43f5e; }
.ac-integrity-bar-wrap { margin-bottom: 24px; }
.ac-integrity-label {
  display: flex; justify-content: space-between;
  font-size: 0.75rem; font-weight: 800; color: #64748b; margin-bottom: 8px;
}
.text-ok  { color: #10b981 !important; }
.text-warn { color: #f43f5e !important; }
.ac-integrity-track { height: 8px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.ac-integrity-fill { height: 100%; border-radius: 10px; transition: width 0.6s ease; }
.btn-resume {
  width: 100%; padding: 16px; background: #0f172a; color: white;
  border: none; border-radius: 18px; font-weight: 900; font-size: 1rem;
  cursor: pointer; font-family: inherit; transition: 0.3s;
}
.btn-resume:not(:disabled):hover { background: #f59e0b; color: #0f172a; }
.btn-resume:disabled { opacity: 0.4; cursor: not-allowed; }
.ac-warning-critical {
  margin-top: 16px; font-size: 0.8rem; font-weight: 700; color: #f43f5e;
  background: #fff1f2; padding: 10px 16px; border-radius: 12px;
}

/* ══ EXAM HEADER ══ */
.exam-header {
  position: fixed; top: 0; left: 0; right: 0; height: 68px;
  z-index: 100; background: rgba(255,255,255,0.95);
  backdrop-filter: blur(12px);
  border-bottom: 1px solid #eef2f6;
  box-shadow: 0 4px 20px rgba(0,0,0,0.04);
  display: flex; align-items: center; justify-content: space-between;
  padding: 0 30px;
}
.hdr-brand { font-size: 1.4rem; font-weight: 900; color: #0f172a; }
.hdr-brand span { color: #f59e0b; }
.global-timer {
  font-size: 1.5rem; font-weight: 900; color: #0f172a;
  display: flex; align-items: center; gap: 8px;
  font-variant-numeric: tabular-nums;
}
.timer-warn   { color: #f59e0b !important; }
.timer-danger { color: #f43f5e !important; animation: timer-blink 1s infinite; }
@keyframes timer-blink { 0%,100% { opacity:1; } 50% { opacity:0.5; } }
.hdr-right { display: flex; align-items: center; gap: 16px; }
.integrity-pill {
  display: flex; align-items: center; gap: 6px;
  padding: 6px 14px; border-radius: 10px;
  font-size: 0.72rem; font-weight: 900;
}
.integrity-ok   { background: #ecfdf5; color: #10b981; }
.integrity-warn { background: #fff1f2; color: #f43f5e; }
.btn-end-exam {
  background: #0f172a; color: white; border: none;
  padding: 10px 20px; border-radius: 12px;
  font-weight: 800; cursor: pointer; font-family: inherit;
  font-size: 0.82rem; transition: 0.2s;
}
.btn-end-exam:hover { background: #f43f5e; }

/* BARRE PROGRESSION */
.global-prog-bar {
  position: fixed; top: 68px; left: 0; right: 0; height: 3px;
  background: #eef2f6; z-index: 99;
}
.global-prog-fill {
  height: 100%;
  background: linear-gradient(90deg, #f59e0b, #fbbf24);
  transition: width 0.5s ease;
}

/* NAVIGATOR */
.q-navigator {
  position: fixed; top: 71px; left: 0; right: 0; z-index: 98;
  background: rgba(255,255,255,0.96); backdrop-filter: blur(8px);
  padding: 10px 30px; display: flex; flex-wrap: wrap; gap: 6px;
  border-bottom: 1px solid #eef2f6;
}
.nav-pill {
  width: 32px; height: 32px; border-radius: 10px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.7rem; font-weight: 800; cursor: pointer; transition: 0.2s;
  background: #f8fafc; color: #94a3b8; border: 1.5px solid #eef2f6;
}
.nav-pill.np-active   { background: #0f172a; color: white; border-color: #0f172a; }
.nav-pill.np-answered { background: #fffbeb; color: #f59e0b; border-color: #fde68a; }
.nav-pill.np-timeout  { background: #fff1f2; color: #f43f5e; border-color: #fecaca; }

/* Q TIMER STRIP */
.q-timer-strip {
  position: fixed; top: 121px; left: 0; right: 0; z-index: 97;
  background: rgba(255,255,255,0.97); padding: 10px 30px;
  border-bottom: 1px solid #eef2f6;
}
.qt-meta {
  display: flex; justify-content: space-between;
  font-size: 0.62rem; font-weight: 900; color: #64748b;
  margin-bottom: 6px; letter-spacing: 0.5px;
}
.qt-val   { color: #10b981; }
.qt-danger { color: #f43f5e !important; animation: timer-blink 1s infinite; }
.qt-track { height: 5px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.qt-fill  { height: 100%; border-radius: 10px; }

/* EXAM BODY */
.exam-body {
  position: relative; z-index: 10;
  padding-top: 175px;
  display: flex; justify-content: center; padding-bottom: 60px;
  padding-left: 20px; padding-right: 20px;
}
/* ✅ FIX : Padding extra si la barre timer individuelle est affichée */
.exam-body.has-qtimer { padding-top: 230px; }

/* QUESTION CARD */
.question-card {
  width: 100%; max-width: 860px;
  background: white; border-radius: 32px;
  padding: 40px; border: 1px solid #eef2f6;
  box-shadow: 0 20px 40px rgba(0,0,0,0.04);
  margin-top: 20px;
}
.question-card.q-locked { opacity: 0.6; filter: grayscale(0.4); }

.q-meta-row {
  display: flex; justify-content: space-between; align-items: center;
  margin-bottom: 22px;
}
.q-num { font-size: 0.62rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; }
.q-badges { display: flex; align-items: center; gap: 8px; }
.badge-pts   { background: #fffbeb; color: #f59e0b; font-size: 0.62rem; font-weight: 900; padding: 3px 10px; border-radius: 8px; }
.badge-type  { background: #f1f5f9; color: #64748b; font-size: 0.62rem; font-weight: 900; padding: 4px 12px; border-radius: 50px; }
.badge-theme { background: #ecfdf5; color: #10b981; font-size: 0.62rem; font-weight: 900; padding: 3px 10px; border-radius: 8px; }

.q-text {
  font-size: 1.75rem; font-weight: 800; line-height: 1.35;
  color: #0f172a; margin-bottom: 32px;
}
.q-disabled { pointer-events: none; opacity: 0.7; }

/* OPTIONS */
.opts-list { display: flex; flex-direction: column; gap: 12px; }
.opt-item {
  background: #f8fafc; border: 2px solid #eef2f6;
  border-radius: 18px; padding: 18px 22px;
  display: flex; align-items: center; gap: 16px;
  cursor: pointer; transition: 0.25s cubic-bezier(0.4,0,0.2,1);
}
.opt-item:hover {
  border-color: #f59e0b; background: white;
  transform: translateY(-2px); box-shadow: 0 8px 20px rgba(245,158,11,0.08);
}
.opt-item.opt-selected {
  border-color: #f59e0b; background: #fffbeb;
  transform: translateY(-2px); box-shadow: 0 8px 20px rgba(245,158,11,0.12);
}
.opt-letter {
  width: 40px; height: 40px; min-width: 40px;
  background: white; border: 2px solid #eef2f6; border-radius: 12px;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; color: #94a3b8; font-size: 0.82rem;
}
.opt-selected .opt-letter { background: #f59e0b; border-color: #f59e0b; color: white; }
.opt-text  { flex: 1; font-weight: 700; font-size: 0.95rem; color: #0f172a; line-height: 1.4; }
.opt-check { color: #f59e0b; font-size: 0.9rem; }

/* VRAI/FAUX */
.vf-zone { display: flex; gap: 20px; }
.vf-btn {
  flex: 1; padding: 28px; background: #f8fafc;
  border: 2px solid #eef2f6; border-radius: 20px;
  display: flex; align-items: center; justify-content: center; gap: 12px;
  font-size: 1.1rem; font-weight: 900; cursor: pointer; transition: 0.25s;
  color: #64748b; font-family: inherit;
}
.vf-btn:hover { transform: translateY(-3px); }
.vf-btn.vf-selected       { background: #fffbeb; border-color: #f59e0b; color: #f59e0b; }
.vf-btn.vf-false.vf-selected { background: #fff1f2; border-color: #f43f5e; color: #f43f5e; }
.vf-btn:not(.vf-false):hover { border-color: #10b981; color: #10b981; }
.vf-btn.vf-false:hover     { border-color: #f43f5e; color: #f43f5e; }

/* TEXTE LIBRE */
.text-answer {
  width: 100%; min-height: 180px; padding: 18px;
  background: #f8fafc; border: 2px solid #eef2f6;
  border-radius: 18px; font-family: inherit;
  font-size: 0.95rem; font-weight: 600; color: #0f172a;
  resize: vertical; outline: none; transition: 0.2s;
}
.text-answer:focus { border-color: #f59e0b; background: white; }

/* TIMEOUT */
.timeout-badge {
  background: #fff1f2; color: #f43f5e;
  border: 1px solid #fecaca; border-radius: 12px;
  padding: 10px 16px; font-size: 0.78rem; font-weight: 700;
  margin-top: 20px; text-align: center;
}

/* FOOTER NAVIGATION */
.q-footer {
  display: flex; align-items: center; justify-content: space-between;
  margin-top: 36px; padding-top: 24px;
  border-top: 1px solid #f1f5f9; gap: 12px;
}
.q-answered-info { font-size: 0.7rem; font-weight: 800; color: #94a3b8; text-align: center; }
.btn-nav {
  padding: 12px 24px; border-radius: 14px;
  border: 2px solid #e2e8f0; background: white;
  font-weight: 800; cursor: pointer; font-family: inherit;
  font-size: 0.82rem; transition: 0.2s; color: #64748b;
  display: flex; align-items: center; gap: 6px;
}
.btn-nav:disabled { opacity: 0.3; cursor: not-allowed; }
.btn-nav:not(:disabled):hover { border-color: #0f172a; color: #0f172a; }
.btn-nav-next { background: #0f172a; color: white; border-color: #0f172a; }
.btn-nav-next:hover { background: #f59e0b !important; border-color: #f59e0b !important; color: #0f172a !important; }
.btn-nav-finish { background: #10b981; color: white; border-color: #10b981; }
.btn-nav-finish:hover { background: #059669 !important; border-color: #059669 !important; }

/* ══ RÉSULTATS ══ */
.results-wrapper { max-width: 1100px; width: 100%; position: relative; z-index: 10; }
.results-hero { text-align: center; margin-bottom: 32px; }
.brand-logo-sm { font-size: 2.2rem; font-weight: 900; letter-spacing: -1.5px; margin-bottom: 16px; display: block; }
.brand-logo-sm span { color: #f59e0b; }
.result-badge {
  display: inline-flex; align-items: center; gap: 10px;
  padding: 10px 24px; border-radius: 16px;
  font-size: 0.72rem; font-weight: 900; letter-spacing: 2px;
}
.rb-pass { background: #ecfdf5; color: #10b981; border: 2px solid #6ee7b7; }
.rb-fail { background: #fff1f2; color: #f43f5e; border: 2px solid #fca5a5; }

/* BENTO GRID */
.bento-grid {
  display: grid; grid-template-columns: 1fr 1.6fr;
  grid-template-rows: auto auto; gap: 20px;
}
.bento-card {
  background: white; border-radius: 32px; padding: 36px;
  box-shadow: 0 20px 40px rgba(0,0,0,0.04); border: 1px solid #eef2f6;
}
.bento-lbl {
  font-size: 0.58rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 2px; text-transform: uppercase; display: block; margin-bottom: 20px;
}
.bento-title {
  font-size: 0.6rem; font-weight: 900; color: #94a3b8;
  letter-spacing: 2px; text-transform: uppercase; margin: 0 0 20px;
}
.bento-score  { display: flex; flex-direction: column; align-items: center; text-align: center; }
.bento-themes { grid-column: 1 / -1; }

/* SCORE RING */
.score-ring-wrap { margin: 10px auto 20px; }
.ring-bg   { fill: none; stroke: #f1f5f9; stroke-width: 10; }
.ring-fill { fill: none; stroke-width: 10; stroke-linecap: round; }
.ring-pct  { font-size: 22px; font-weight: 900; fill: #0f172a; }
.ring-pts  { font-size: 9px; fill: #94a3b8; font-weight: 700; }
.result-status-pill {
  padding: 10px 28px; border-radius: 14px; font-size: 0.78rem; font-weight: 900;
  display: inline-flex; align-items: center; gap: 6px;
}
.pill-pass { background: #ecfdf5; color: #10b981; }
.pill-fail { background: #fff1f2; color: #f43f5e; }
.seuil-info { font-size: 0.68rem; color: #94a3b8; font-weight: 700; margin-top: 12px; display: block; }

/* MÉTRIQUES */
.metric-item { margin-bottom: 16px; }
.metric-info {
  display: flex; justify-content: space-between;
  font-size: 0.8rem; font-weight: 700; color: #64748b; margin-bottom: 6px;
}
.metric-bar { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.mbar-fill  { height: 100%; border-radius: 10px; transition: width 1s ease; }
.mbar-amber  { background: linear-gradient(90deg, #f59e0b, #fbbf24); }
.mbar-indigo { background: linear-gradient(90deg, #6366f1, #818cf8); }
.mbar-green  { background: linear-gradient(90deg, #10b981, #34d399); }
.mbar-gray   { background: linear-gradient(90deg, #94a3b8, #cbd5e1); }

/* ANTI-CHEAT RÉSULTAT */
.anticheat-result-box {
  display: flex; align-items: center; gap: 12px;
  border-radius: 14px; padding: 12px 18px; margin: 16px 0; font-size: 0.8rem;
}
.ac-result-ok   { background: #ecfdf5; color: #10b981; border: 1px solid #6ee7b7; }
.ac-result-warn { background: #fff1f2; color: #f43f5e; border: 1px solid #fca5a5; }
.anticheat-result-box div { display: flex; flex-direction: column; gap: 2px; }
.anticheat-result-box strong { font-size: 0.78rem; font-weight: 900; }
.anticheat-result-box span  { font-size: 0.7rem; font-weight: 600; }

/* RÉPARTITION */
.repartition-row { display: flex; gap: 12px; margin: 16px 0; }
.rep-item {
  flex: 1; border-radius: 16px; padding: 16px;
  text-align: center; display: flex; flex-direction: column; align-items: center; gap: 4px;
}
.rep-correct  { background: #ecfdf5; }
.rep-incorrect { background: #fff1f2; }
.rep-skipped  { background: #f8fafc; }
.rep-icon { font-size: 1rem; margin-bottom: 4px; }
.rep-correct .rep-icon   { color: #10b981; }
.rep-incorrect .rep-icon { color: #f43f5e; }
.rep-skipped .rep-icon   { color: #94a3b8; }
.rep-val { font-size: 1.5rem; font-weight: 900; color: #0f172a; line-height: 1; }
.rep-lbl { font-size: 0.58rem; font-weight: 900; color: #94a3b8; letter-spacing: 0.5px; }

.btn-voir-correction {
  width: 100%; padding: 14px; border-radius: 14px;
  background: white; border: 2px solid #0f172a; color: #0f172a;
  font-weight: 900; cursor: pointer; font-family: inherit;
  font-size: 0.82rem; display: flex; align-items: center; justify-content: center;
  transition: 0.2s; margin-top: 4px;
}
.btn-voir-correction:hover { background: #0f172a; color: white; }

/* THÈMES */
.theme-row { margin-bottom: 14px; }
.theme-info {
  display: flex; justify-content: space-between;
  font-size: 0.8rem; font-weight: 700; margin-bottom: 6px;
}
.theme-name  { color: #0f172a; }
.theme-score { font-weight: 800; font-size: 0.75rem; }
.theme-track { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.theme-fill  { height: 100%; border-radius: 10px; transition: width 1s ease 0.3s; }

/* COACH */
.bento-coach { display: flex; flex-direction: column; align-items: center; text-align: center; }
.coach-avatar {
  width: 60px; height: 60px; background: #0f172a;
  border-radius: 20px; display: flex; align-items: center;
  justify-content: center; font-size: 1.5rem; color: #f59e0b;
}
.coach-msg   { font-size: 0.88rem; color: #64748b; line-height: 1.6; font-weight: 600; margin-bottom: 16px; }
.result-tags { display: flex; flex-wrap: wrap; gap: 8px; justify-content: center; margin-bottom: 16px; }
.result-tag  {
  background: #fffbeb; color: #f59e0b; font-size: 0.65rem;
  font-weight: 900; padding: 5px 12px; border-radius: 8px;
}
.email-notif-sent {
  background: #ecfdf5; color: #10b981; border: 1px solid #6ee7b7;
  border-radius: 12px; padding: 10px 18px; font-size: 0.78rem; font-weight: 700;
  display: flex; align-items: center;
}

/* ══ CORRECTION ══ */
.review-portal { position: relative; z-index: 10; min-height: 100vh; background: #f8fafc; }
.review-nav {
  position: sticky; top: 0; z-index: 100;
  background: rgba(255,255,255,0.97); backdrop-filter: blur(12px);
  padding: 0 30px; height: 64px;
  display: flex; align-items: center; justify-content: space-between;
  border-bottom: 1px solid #eef2f6; box-shadow: 0 4px 20px rgba(0,0,0,0.04);
}
.brand-sm { font-size: 1.3rem; font-weight: 900; color: #0f172a; }
.brand-sm span:first-child { color: #f59e0b; }
.review-label { font-size: 0.7rem; font-weight: 700; color: #94a3b8; margin-left: 6px; }
.review-nav-right { display: flex; align-items: center; gap: 14px; }
.result-pill { padding: 5px 14px; border-radius: 10px; font-size: 0.7rem; font-weight: 900; }
.btn-back-review {
  background: #0f172a; color: white; border: none;
  padding: 10px 18px; border-radius: 12px;
  font-weight: 800; cursor: pointer; font-family: inherit; font-size: 0.8rem;
  display: flex; align-items: center; gap: 6px;
}
.btn-back-review:hover { background: #f59e0b; color: #0f172a; }

/* FILTRES CORRECTION */
.review-filters {
  position: sticky; top: 64px; z-index: 99;
  background: white; border-bottom: 1px solid #eef2f6;
  padding: 12px 30px; display: flex; gap: 8px; flex-wrap: wrap;
}
.rf-btn {
  display: flex; align-items: center; gap: 8px;
  padding: 7px 16px; border-radius: 10px;
  border: 1.5px solid #eef2f6; background: white;
  font-size: 0.72rem; font-weight: 800;
  cursor: pointer; font-family: inherit; transition: 0.2s; color: #64748b;
}
.rf-btn:hover  { border-color: #0f172a; color: #0f172a; }
.rf-btn.rf-active { background: #0f172a; color: white; border-color: #0f172a; }
.rf-dot { width: 7px; height: 7px; border-radius: 50%; }
.rfd-all       { background: #94a3b8; }
.rfd-correct   { background: #10b981; }
.rfd-incorrect { background: #f43f5e; }
.rfd-skipped   { background: #f59e0b; }

.review-scroll { padding: 30px 20px 60px; }
.review-list { max-width: 860px; margin: 0 auto; display: flex; flex-direction: column; gap: 20px; }
.review-empty { text-align: center; padding: 60px 20px; color: #94a3b8; }

.correction-card {
  background: white; border-radius: 28px; border: 2px solid #eef2f6; overflow: hidden;
}
.cc-correct   { border-color: #6ee7b7; }
.cc-incorrect { border-color: #fca5a5; }
.cc-skipped   { border-color: #fde68a; }

.cc-header {
  display: flex; justify-content: space-between; align-items: center;
  padding: 16px 24px; background: #f8fafc; border-bottom: 1px solid #eef2f6;
}
.cc-header-left { display: flex; align-items: center; gap: 10px; }
.cc-num   { font-size: 0.6rem; font-weight: 900; color: #94a3b8; letter-spacing: 1px; }
.cc-theme { background: #fffbeb; color: #f59e0b; font-size: 0.6rem; font-weight: 900; padding: 3px 8px; border-radius: 6px; }
.cc-pts   { background: #f1f5f9; color: #64748b; font-size: 0.6rem; font-weight: 900; padding: 3px 8px; border-radius: 6px; }
.cc-status {
  font-size: 0.62rem; font-weight: 900; padding: 5px 12px;
  border-radius: 8px; display: flex; align-items: center;
}
.cc-s-correct   { background: #ecfdf5; color: #10b981; }
.cc-s-incorrect { background: #fff1f2; color: #f43f5e; }
.cc-s-skipped   { background: #fffbeb; color: #f59e0b; }

.cc-enonce {
  font-size: 1.05rem; font-weight: 800; color: #0f172a;
  padding: 22px 24px 14px; margin: 0; line-height: 1.4;
}

/* OPTIONS CORRECTION */
.cc-opts { padding: 0 24px 20px; display: flex; flex-direction: column; gap: 10px; }
.cc-opt {
  display: flex; align-items: center; gap: 12px;
  padding: 12px 16px; border-radius: 14px;
  border: 1.5px solid #eef2f6; background: #f8fafc; transition: 0.2s;
}
.cc-opt.cco-correct      { border-color: #6ee7b7; background: #f0fdf4; }
.cc-opt.cco-user         { border-color: #fca5a5; background: #fff1f2; }
.cc-opt.cco-user-correct { border-color: #6ee7b7; background: #ecfdf5; }
.cco-letter {
  width: 30px; height: 30px; min-width: 30px; border-radius: 9px;
  background: white; border: 1.5px solid #eef2f6;
  display: flex; align-items: center; justify-content: center;
  font-weight: 900; font-size: 0.72rem; color: #94a3b8;
}
.cco-correct .cco-letter { background: #10b981; border-color: #10b981; color: white; }
.cco-user    .cco-letter { background: #f43f5e; border-color: #f43f5e; color: white; }
.cco-text { flex: 1; font-weight: 700; font-size: 0.85rem; color: #0f172a; }

/* TEXTE CORRECTION */
.cc-text-compare { display: grid; grid-template-columns: 1fr 1fr; gap: 12px; padding: 0 24px 20px; }
.cctc-block { border-radius: 14px; padding: 16px; border: 1.5px solid #eef2f6; }
.cctc-block label { font-size: 0.55rem; font-weight: 900; color: #94a3b8; letter-spacing: 1.5px; display: block; margin-bottom: 8px; }
.cctc-block p { font-size: 0.85rem; font-weight: 700; color: #0f172a; white-space: pre-wrap; margin: 0; }
.cctc-ok   { background: #f0fdf4; border-color: #6ee7b7; }
.cctc-user { background: #fff1f2; border-color: #fca5a5; }

/* EXPLICATION */
.cc-explication { margin: 0 24px 24px; background: #fffbeb; border: 1px solid #fde68a; border-radius: 16px; padding: 16px 20px; }
.cc-exp-header {
  display: flex; align-items: center; font-size: 0.62rem;
  font-weight: 900; color: #92400e; letter-spacing: 1px; margin-bottom: 8px;
}
.cc-exp-text { font-size: 0.85rem; color: #78350f; font-weight: 600; line-height: 1.6; margin: 0; }

/* ══ TOAST ══ */
.global-toast {
  position: fixed; bottom: 28px; right: 28px;
  background: #0f172a; color: white;
  padding: 18px 24px; border-radius: 18px;
  display: flex; align-items: center; gap: 14px;
  z-index: 3000; border-left: 4px solid #f59e0b;
  box-shadow: 0 20px 40px rgba(0,0,0,0.2);
}
.t-success { border-left-color: #10b981; }
.t-error   { border-left-color: #f43f5e; }
.t-warn    { border-left-color: #f59e0b; }
.toast-body strong { font-size: 0.6rem; letter-spacing: 1px; opacity: 0.5; display: block; }
.toast-body p { font-size: 0.82rem; font-weight: 700; margin: 0; }

/* ══ CONFIRM ══ */
.confirm-overlay {
  position: fixed; inset: 0; background: rgba(15,23,42,0.5);
  backdrop-filter: blur(8px); z-index: 8000;
  display: flex; align-items: center; justify-content: center;
}
.confirm-card {
  background: white; padding: 48px 40px;
  border-radius: 32px; text-align: center; max-width: 420px; width: 90%;
  box-shadow: 0 40px 80px rgba(0,0,0,0.15);
}
.confirm-icon { font-size: 2.5rem; color: #f59e0b; margin-bottom: 16px; display: block; }
.confirm-card h4 { font-weight: 900; margin-bottom: 10px; }
.confirm-card p  { color: #64748b; font-size: 0.88rem; font-weight: 600; margin-bottom: 28px; }
.confirm-actions { display: flex; gap: 14px; justify-content: center; }
.btn-cancel {
  background: #f1f5f9; color: #64748b; border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit;
}
.btn-confirm-ok {
  background: #0f172a; color: white; border: none;
  padding: 12px 24px; border-radius: 14px; font-weight: 800;
  cursor: pointer; font-family: inherit; transition: 0.2s;
}
.btn-confirm-ok:hover { background: #f43f5e; }

/* ══ TRANSITIONS ══ */
.scale-fade-enter-active { animation: scaleIn 0.3s ease-out; }
.scale-fade-leave-active  { animation: scaleIn 0.2s ease-in reverse; }
@keyframes scaleIn { from { opacity:0; transform:scale(0.95); } to { opacity:1; transform:scale(1); } }

.toast-pop-enter-active { animation: toastSlide 0.4s cubic-bezier(0.4,0,0.2,1) both; }
.toast-pop-leave-active  { animation: toastSlide 0.3s ease-in reverse; }
@keyframes toastSlide { from { transform:translateX(120%); opacity:0; } to { transform:translateX(0); opacity:1; } }

.q-slide-enter-active { transition: all 0.3s ease-out; }
.q-slide-leave-active { transition: all 0.2s ease-in; }
.q-slide-enter-from   { opacity:0; transform:translateX(28px); }
.q-slide-leave-to     { opacity:0; transform:translateX(-28px); }

/* ══ RESPONSIVE ══ */
@media (max-width: 768px) {
  .lobby-card { padding: 36px 24px; border-radius: 32px; }
  .exam-meta-grid { grid-template-columns: 1fr; }
  .brand-name { font-size: 2rem; }
  .exam-header { padding: 0 16px; }
  .global-timer { font-size: 1.2rem; }
  .q-navigator { padding: 8px 16px; }
  .exam-body { padding-top: 175px; padding-left: 12px; padding-right: 12px; }
  .exam-body.has-qtimer { padding-top: 230px; }
  .question-card { padding: 24px 18px; }
  .q-text { font-size: 1.3rem; }
  .vf-zone { flex-direction: column; }
  .bento-grid { grid-template-columns: 1fr; }
  .bento-themes { grid-column: unset; }
  .cc-text-compare { grid-template-columns: 1fr; }
  .review-filters { padding: 10px 16px; }
}
</style>
