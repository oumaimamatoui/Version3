<template>
  <div class="chatbot-wrapper">
    <transition name="bubble-bounce">
      <div @click="toggleChat" class="chat-bubble" :class="{ 'active': isChatOpen }">
        <div class="bubble-ring"></div>
        <i v-if="!isChatOpen" class="fa fa-comments"></i>
        <i v-else class="fa fa-times"></i>
        <span v-if="unreadCount > 0 && !isChatOpen" class="unread-badge">{{ unreadCount }}</span>
      </div>
    </transition>

    <transition name="chat-slide">
      <!-- RTL s'applique UNIQUEMENT sur .chat-window via :dir="chatDir" -->
      <div v-if="isChatOpen" class="chat-window" :dir="chatDir">

        <div class="chat-header">
          <div class="header-left">
            <div class="bot-avatar">
              <i class="fa fa-robot"></i>
              <span class="avatar-pulse"></span>
            </div>
            <div class="header-info">
              <h6>NeoBot Assistant</h6>
              <div class="status-row">
                <span class="dot-online"></span>
                <small>{{ t('chatbot.online') }}</small>
              </div>
            </div>
          </div>
          <div class="header-actions">
            <button @click="toggleTheme" class="btn-action" :title="isDark ? t('theme.light') : t('theme.dark')">
              <i class="fa" :class="isDark ? 'fa-sun' : 'fa-moon'"></i>
            </button>
            <button @click="cycleLang" class="btn-action btn-lang" :title="t('lang.switch')">
              {{ langFlags[currentLang] }}
            </button>
            <button @click="clearChat" class="btn-action" :title="t('chatbot.clear')">
              <i class="fa fa-trash-alt"></i>
            </button>
            <button @click="toggleChat" class="btn-action btn-close-chat">
              <i class="fa fa-times"></i>
            </button>
          </div>
        </div>

        <div v-if="isChatLoading" class="loading-bar">
          <div class="loading-bar-fill"></div>
        </div>

        <div class="chat-body" ref="chatScroll">
          <div v-if="chatMessages.length <= 1 && startSuggestions.length > 0" class="start-suggestions">
            <p class="suggestions-label">{{ t('chatbot.suggestLabel') }}</p>
            <div class="suggestion-chips">
              <button v-for="(s, i) in startSuggestions" :key="i" @click="sendSuggestion(s)" class="chip">
                {{ s }}
              </button>
            </div>
          </div>

          <div v-for="(msg, index) in chatMessages" :key="index" :class="['chat-msg', msg.role]">
            <div v-if="msg.role === 'ai'" class="msg-avatar">
              <i class="fa fa-robot"></i>
            </div>
            <div class="msg-bubble">
              <div class="msg-content" v-html="formatMessage(msg.text)"></div>
              <span v-if="msg.source && msg.source !== 'gemini'" class="source-badge">
                {{ msg.source === 'cache' ? '⚡ Cache' : msg.source === 'intent' ? '🧠 Local' : '🤖 IA' }}
              </span>
              <div v-if="msg.role === 'ai'" class="msg-actions">
                <button @click="speak(msg.text, index)" class="msg-action-btn" :title="t('chatbot.speak')">
                  <i class="fa fa-volume-up"></i>
                </button>
                <button @click="copyText(msg.text)" class="msg-action-btn" :title="t('chatbot.copy')">
                  <i class="fa fa-copy"></i>
                </button>
              </div>
              <div v-if="msg.suggestions && msg.suggestions.length > 0" class="follow-suggestions">
                <button v-for="(s, si) in msg.suggestions" :key="si" @click="sendSuggestion(s)" class="follow-chip">
                  {{ s }}
                </button>
              </div>
              <span class="msg-time">{{ msg.time }}</span>
            </div>
          </div>

          <div v-if="isChatLoading" class="chat-msg ai typing-indicator-wrap">
            <div class="msg-avatar"><i class="fa fa-robot"></i></div>
            <div class="msg-bubble">
              <div class="msg-content typing-dots">
                <span></span><span></span><span></span>
              </div>
            </div>
          </div>
        </div>

        <div class="chat-footer">
          <div v-if="isListening" class="voice-active-bar">
            <i class="fa fa-microphone"></i>
            <span>{{ t('chatbot.listening') }}</span>
            <div class="voice-waves">
              <span v-for="i in 5" :key="i"></span>
            </div>
          </div>
          <div class="input-row">
            <button @click="toggleVoiceRecognition" class="btn-mic" :class="{ 'mic-active': isListening }" :title="t('chatbot.voice')">
              <i class="fa" :class="isListening ? 'fa-microphone-slash' : 'fa-microphone'"></i>
            </button>
            <div class="input-wrapper">
              <input
                v-model="chatInput"
                @keyup.enter="handleChat(false)"
                type="text"
                class="chat-input"
                :placeholder="t('chatbot.placeholder')"
                :disabled="isChatLoading"
                ref="chatInputRef"
              />
              <span v-if="chatInput.length > 0" class="char-count">{{ chatInput.length }}</span>
            </div>
            <button @click="handleChat(false)" class="btn-send" :disabled="!chatInput.trim() || isChatLoading">
              <i class="fa fa-paper-plane"></i>
            </button>
          </div>
        </div>

      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref, computed, watch, inject, nextTick } from 'vue';
import { useAuthStore } from '@/stores/auth';
import aiService from '@/services/ai.service';
import MarkdownIt from 'markdown-it';
import DOMPurify from 'dompurify';
import hljs from 'highlight.js/lib/core';
import javascript from 'highlight.js/lib/languages/javascript';
import python from 'highlight.js/lib/languages/python';
import java from 'highlight.js/lib/languages/java';
import sql from 'highlight.js/lib/languages/sql';
import bash from 'highlight.js/lib/languages/bash';
import json from 'highlight.js/lib/languages/json';
import xml from 'highlight.js/lib/languages/xml';
import css from 'highlight.js/lib/languages/css';
import php from 'highlight.js/lib/languages/php';
import ruby from 'highlight.js/lib/languages/ruby';
import csharp from 'highlight.js/lib/languages/csharp';
hljs.registerLanguage('javascript', javascript);
hljs.registerLanguage('python', python);
hljs.registerLanguage('java', java);
hljs.registerLanguage('sql', sql);
hljs.registerLanguage('bash', bash);
hljs.registerLanguage('json', json);
hljs.registerLanguage('xml', xml);
hljs.registerLanguage('css', css);
hljs.registerLanguage('php', php);
hljs.registerLanguage('ruby', ruby);
hljs.registerLanguage('csharp', csharp);
const md = new MarkdownIt({
  html: false,
  linkify: true,
  typographer: true,
  highlight(str, lang) {
    if (lang && hljs.getLanguage(lang)) {
      try {
        return `<pre class="hljs"><code>${hljs.highlight(str, { language: lang }).value}</code></pre>`;
      } catch {}
    }
    return `<pre class="hljs"><code>${md.utils.escapeHtml(str)}</code></pre>`;
  }
});

const authStore = useAuthStore();

// Injections from App.vue
const isDark = inject('isDark');
const toggleTheme = inject('toggleTheme');
const currentLang = inject('currentLang');
const t = inject('t');
const cycleLang = inject('cycleLang');

// Chatbot constants
const langFlags  = { fr: '🇫🇷', en: '🇬🇧', ar: '🇸🇦' };
const langVoice  = { fr: 'fr-FR', en: 'en-US', ar: 'ar-SA' };
const langLocale = { fr: 'fr-FR', en: 'en-US', ar: 'ar-SA' };

const isChatOpen     = ref(false);
const chatInput      = ref('');
const isChatLoading  = ref(false);
const isListening    = ref(false);
const chatScroll     = ref(null);
const chatInputRef   = ref(null);
const chatMessages   = ref([]);
const unreadCount    = ref(0);
const startSuggestions   = ref([]);
const sessionId          = ref(`session_${Date.now()}`);
const speakingIndex      = ref(-1);

const DEFAULT_SUGGESTIONS = {
  fr: ['Créer un test IA', 'Analyser un CV', 'Voir mes résultats', 'Générer un rapport'],
  en: ['Create an AI test', 'Analyze a CV', 'View my results', 'Generate a report'],
  ar: ['إنشاء اختبار ذكاء اصطناعي', 'تحليل سيرة ذاتية', 'عرض نتائجي', 'إنشاء تقرير'],
};

const chatDir = computed(() => currentLang.value === 'ar' ? 'rtl' : 'ltr');

const greetingText = computed(() => {
  const name = authStore.user?.name;
  if (!name) return t('chatbot.welcome');
  const welcome = t('chatbot.welcome');
  return welcome.replace(/^(👋\s*)([^!\n]+)(!\s*)/, `$1$2 ${name}$3`);
});

const formatMessage = (text) => {
  if (!text) return '';
  try {
    return DOMPurify.sanitize(md.render(text));
  } catch {
    return text.replace(/\n/g, '<br>');
  }
};

const now = () =>
  new Date().toLocaleTimeString(langLocale[currentLang.value] || 'fr-FR', {
    hour: '2-digit', minute: '2-digit'
  });

const MAX_CHAT_MSGS = 100;
const pushMsg = (msg) => {
  chatMessages.value.push(msg);
  if (chatMessages.value.length > MAX_CHAT_MSGS) chatMessages.value.shift();
};

const scrollToBottom = async () => {
  await nextTick();
  chatScroll.value?.scrollTo({ top: chatScroll.value.scrollHeight, behavior: 'smooth' });
};

const loadStartSuggestions = async () => {
  try {
    const role = authStore.role || 'Recruteur';
    const data = await aiService.getSuggestions(role, currentLang.value);
    startSuggestions.value = data.suggestions || DEFAULT_SUGGESTIONS[currentLang.value];
  } catch {
    startSuggestions.value = DEFAULT_SUGGESTIONS[currentLang.value] || DEFAULT_SUGGESTIONS.fr;
  }
};

const sendSuggestion = (text) => {
  chatInput.value = text;
  handleChat(false);
};

const toggleChat = async () => {
  isChatOpen.value = !isChatOpen.value;
  if (!isChatOpen.value) {
    window.speechSynthesis.cancel();
    speakingIndex.value = -1;
  }
  if (isChatOpen.value) {
    unreadCount.value = 0;
    if (chatMessages.value.length === 0) {
      pushMsg({ role: 'ai', text: greetingText.value, time: now(), suggestions: [] });
      await loadStartSuggestions();
    }
    await nextTick();
    chatInputRef.value?.focus();
    await scrollToBottom();
  }
};

const handleChat = async (isVocal = false) => {
  if (!chatInput.value.trim() || isChatLoading.value) return;
  const userText = chatInput.value.trim();
  pushMsg({ role: 'user', text: userText, time: now() });
  chatInput.value     = '';
  isChatLoading.value = true;
  await scrollToBottom();
  try {
    const role = authStore.role || 'Recruteur';
    const userName = authStore.user?.name || '';
    const data = await aiService.sendMessage(userText, role, sessionId.value, userName, '', '');
    const reply    = data.response || data.reply || t('chatbot.error');
    pushMsg({
      role: 'ai', text: reply, time: now(),
      suggestions: data.suggestions || [],
      source: data.source || ''
    });
    if (isVocal) speak(reply, chatMessages.value.length - 1);
    if (!isChatOpen.value) unreadCount.value++;
  } catch {
    pushMsg({ role: 'ai', text: t('chatbot.error'), time: now(), suggestions: [] });
  } finally {
    isChatLoading.value = false;
    await scrollToBottom();
  }
};

const clearChat = async () => {
  try {
    await aiService.resetSession(sessionId.value);
  } catch {}
  chatMessages.value = [];
  sessionId.value    = `session_${Date.now()}`;
  pushMsg({ role: 'ai', text: greetingText.value, time: now(), suggestions: [] });
  await loadStartSuggestions();
};

const speak = (text, index = -1) => {
  if (!text) return;
  if (index === speakingIndex.value) {
    window.speechSynthesis.cancel();
    speakingIndex.value = -1;
    return;
  }
  window.speechSynthesis.cancel();
  try {
    const clean = text.replace(/<[^>]*>/g, '').replace(/\*\*/g, '');
    const msg   = new SpeechSynthesisUtterance(clean);
    msg.lang    = langVoice[currentLang.value] || 'fr-FR';
    msg.rate    = 0.95;
    msg.onend   = () => { speakingIndex.value = -1; };
    msg.onerror = () => { speakingIndex.value = -1; };
    speakingIndex.value = index;
    window.speechSynthesis.speak(msg);
  } catch {
    speakingIndex.value = -1;
  }
};

const copyText = (text) => {
  const clean = text.replace(/<[^>]*>/g, '').replace(/\*\*/g, '');
  navigator.clipboard.writeText(clean).catch(() => {});
};

const toggleVoiceRecognition = () => {
  const SR = window.SpeechRecognition || window.webkitSpeechRecognition;
  if (!SR) { alert('Votre navigateur ne supporte pas la reconnaissance vocale.'); return; }
  if (isListening.value) { isListening.value = false; return; }
  const recognition  = new SR();
  recognition.lang   = langVoice[currentLang.value] || 'fr-FR';
  recognition.onstart  = () => { isListening.value = true; };
  recognition.onend    = () => { isListening.value = false; };
  recognition.onerror  = () => { isListening.value = false; };
  recognition.onresult = (e) => {
    chatInput.value = e.results[0][0].transcript;
    handleChat(true);
  };
  recognition.start();
};

// Reacting to language changes
watch(currentLang, async () => {
  await loadStartSuggestions();
  if (chatMessages.value.length === 1 && chatMessages.value[0].role === 'ai') {
    chatMessages.value[0].text = greetingText.value;
  }
});

// Clear chat on user change (login/logout)
watch(() => authStore.user, (newUser, oldUser) => {
  if (oldUser && newUser?.email !== oldUser?.email) {
    clearChat();
  }
});
</script>

<style>
@import 'highlight.js/styles/github.css';
/* ════════════════════════════════════════════════════════════
   CHATBOT RTL ISOLÉ
   Le RTL s'applique UNIQUEMENT dans .chat-window[dir="rtl"]
   ════════════════════════════════════════════════════════════ */

.chatbot-wrapper {
  --bg-card: var(--surface);
  --bg-page: var(--surface2);
  --bg-input: var(--surface);
  --border-color: var(--bdr);
  --text-main: var(--text);
  --text-muted: var(--text2);
  --text-light: var(--text3);
  --primary: var(--amber);
  --primary-light: rgba(245, 158, 11, 0.1);
  --primary-dark: #d97706;
  --danger: #ef4444;
  --danger-bg: rgba(239, 68, 68, 0.1);
  --shadow-sm: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
  --shadow-lg: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  --shadow-primary: 0 4px 14px 0 rgba(245, 158, 11, 0.39);
  --radius-lg: 0.5rem;
  --radius-xl: 1rem;
  --transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1);
}

/* Chatbot wrapper : toujours positionné en bas à droite (LTR) */
.chatbot-wrapper {
  position: fixed;
  bottom: 28px;
  right: 28px;       /* ← toujours à droite, jamais changé */
  left: auto !important;
  z-index: 99999;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  direction: ltr;    /* ← forcer LTR sur le wrapper */
}

/* Fenêtre chat : RTL appliqué en interne via :dir="chatDir" */
.chat-window {
  position: absolute;
  bottom: 80px;
  right: 0;          /* ← toujours à droite */
  left: auto !important;
  width: 370px;
  max-height: 585px;
  background: var(--bg-card);
  border-radius: var(--radius-xl);
  display: flex;
  flex-direction: column;
  border: 1px solid var(--border-color);
  overflow: hidden;
  box-shadow: var(--shadow-lg);
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

/* RTL interne au chat : alignement texte et bulles */
.chat-window[dir="rtl"] .chat-msg.user { flex-direction: row;         }
.chat-window[dir="rtl"] .chat-msg.ai   { flex-direction: row-reverse; }
.chat-window[dir="rtl"] .msg-bubble    { align-items: flex-end;       }
.chat-window[dir="rtl"] .header-actions { flex-direction: row-reverse; }
.chat-window[dir="rtl"] .chat-input    { text-align: right;           }
.chat-window[dir="rtl"] .footer-note   { flex-direction: row-reverse; }
.chat-window[dir="rtl"] .suggestions-label { text-align: right;       }
.chat-window[dir="rtl"] .suggestion-chips  { justify-content: flex-end; }
.chat-window[dir="rtl"] .follow-suggestions { justify-content: flex-end; }

/* ════════════════════════════════════════════════════════════
   CHATBOT — BULLE
   ════════════════════════════════════════════════════════════ */

.chat-bubble {
  width: 62px; height: 62px;
  background: var(--primary); color: #fff;
  border-radius: 50%;
  display: flex; align-items: center; justify-content: center;
  cursor: pointer; font-size: 1.35rem;
  box-shadow: var(--shadow-primary);
  transition: var(--transition); position: relative;
}
.chat-bubble:hover { transform: scale(1.08); }
.chat-bubble.active { background: var(--danger); box-shadow: 0 8px 28px rgba(239,68,68,0.45); }

.bubble-ring {
  position: absolute; width: 100%; height: 100%;
  border-radius: 50%; border: 2px solid var(--primary);
  animation: ring-pulse 2.5s ease-out infinite; pointer-events: none;
}
.chat-bubble.active .bubble-ring { border-color: var(--danger); animation: none; }

.unread-badge {
  position: absolute; top: -4px; right: -4px;
  background: var(--danger); color: #fff;
  border-radius: 50%; width: 20px; height: 20px;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.7rem; font-weight: 700; border: 2px solid var(--bg-page);
}

/* ════════════════════════════════════════════════════════════
   CHATBOT — HEADER
   ════════════════════════════════════════════════════════════ */

.chat-header {
  background: linear-gradient(135deg, #0f172a 0%, #1e3a5f 100%);
  padding: 14px 16px;
  display: flex; align-items: center; justify-content: space-between;
  flex-shrink: 0;
}

.header-left { display: flex; align-items: center; gap: 10px; }

.bot-avatar {
  width: 40px; height: 40px; border-radius: 12px;
  background: rgba(245,158,11,0.2); border: 1.5px solid rgba(245,158,11,0.4);
  display: flex; align-items: center; justify-content: center;
  color: var(--primary); font-size: 1.1rem; position: relative;
}
.avatar-pulse {
  position: absolute; top: -3px; right: -3px;
  width: 10px; height: 10px;
  background: #10b981; border-radius: 50%;
  border: 2px solid #0f172a;
  animation: dot-pulse 2s ease infinite;
}
.header-info h6    { margin: 0; color: #fff; font-size: 0.9rem; font-weight: 700; }
.status-row        { display: flex; align-items: center; gap: 5px; }
.status-row small  { color: #6ee7b7; font-size: 0.7rem; }
.dot-online        { width: 7px; height: 7px; background: #10b981; border-radius: 50%; animation: dot-pulse 2s ease infinite; }

.header-actions { display: flex; align-items: center; gap: 6px; }
.btn-action {
  width: 30px; height: 30px; border-radius: 8px;
  border: 1px solid rgba(255,255,255,0.15); background: rgba(255,255,255,0.08);
  color: rgba(255,255,255,0.8); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.75rem; transition: var(--transition);
}
.btn-action:hover     { background: rgba(255,255,255,0.18); color: #fff; }
.btn-lang             { font-size: 1rem; }
.btn-close-chat:hover { background: rgba(239,68,68,0.35) !important; }

/* ════════════════════════════════════════════════════════════
   CHATBOT — LOADING BAR
   ════════════════════════════════════════════════════════════ */

.loading-bar      { height: 2px; background: var(--border-color); flex-shrink: 0; overflow: hidden; }
.loading-bar-fill {
  height: 100%;
  background: linear-gradient(90deg, var(--primary), #f97316, var(--primary));
  background-size: 200% 100%;
  animation: loading-slide 1.2s linear infinite;
}

/* ════════════════════════════════════════════════════════════
   CHATBOT — BODY / MESSAGES
   ════════════════════════════════════════════════════════════ */

.chat-body {
  flex: 1; overflow-y: auto; padding: 16px 14px;
  background: var(--bg-page);
  display: flex; flex-direction: column; gap: 8px;
  transition: background-color 0.3s ease;
}

.start-suggestions  { margin-bottom: 6px; }
.suggestions-label  { font-size: 0.72rem; color: var(--text-muted); margin-bottom: 8px; font-weight: 600; text-transform: uppercase; letter-spacing: 0.05em; }
.suggestion-chips   { display: flex; flex-wrap: wrap; gap: 6px; }

.chip {
  padding: 5px 11px; border-radius: 20px;
  border: 1.5px solid var(--primary); background: transparent; color: var(--primary);
  font-size: 0.75rem; font-weight: 600; cursor: pointer; transition: var(--transition);
}
.chip:hover { background: var(--primary); color: #fff; transform: translateY(-1px); }

.chat-msg      { display: flex; gap: 8px; animation: msgFadeIn 0.3s ease; }
.chat-msg.user { flex-direction: row-reverse; }

.msg-avatar {
  width: 30px; height: 30px; border-radius: 9px;
  background: var(--primary-light); border: 1px solid rgba(245,158,11,0.25);
  display: flex; align-items: center; justify-content: center;
  color: var(--primary); font-size: 0.75rem; flex-shrink: 0; align-self: flex-end;
}

.msg-bubble  { display: flex; flex-direction: column; gap: 4px; max-width: 80%; }

.msg-content { padding: 9px 13px; border-radius: 14px; font-size: 0.85rem; line-height: 1.55; word-break: break-word; }
.chat-msg.ai .msg-content {
  background: var(--bg-card); color: var(--text-main);
  border-radius: 4px 14px 14px 14px; border: 1px solid var(--border-color); box-shadow: var(--shadow-sm);
}
.chat-msg.user .msg-content {
  background: linear-gradient(135deg, var(--primary), var(--primary-dark));
  color: #fff; border-radius: 14px 4px 14px 14px; box-shadow: var(--shadow-primary);
}

.source-badge {
  font-size: 0.65rem; color: var(--text-muted);
  padding: 1px 6px; background: var(--bg-page);
  border-radius: 6px; border: 1px solid var(--border-color); align-self: flex-start;
}

.msg-actions { display: flex; gap: 4px; opacity: 0; transition: opacity 0.2s; }
.chat-msg:hover .msg-actions { opacity: 1; }
.msg-action-btn {
  width: 22px; height: 22px; border-radius: 6px;
  border: 1px solid var(--border-color); background: var(--bg-card);
  color: var(--text-muted); font-size: 0.65rem; cursor: pointer;
  display: flex; align-items: center; justify-content: center; transition: var(--transition);
}
.msg-action-btn:hover { background: var(--primary); color: #fff; border-color: var(--primary); }

.follow-suggestions { display: flex; flex-wrap: wrap; gap: 5px; margin-top: 3px; }
.follow-chip {
  padding: 3px 9px; border-radius: 14px;
  border: 1px solid var(--border-color); background: var(--bg-page);
  color: var(--text-muted); font-size: 0.72rem; cursor: pointer; transition: var(--transition);
}
.follow-chip:hover { border-color: var(--primary); color: var(--primary); }

.msg-time { font-size: 0.62rem; color: var(--text-light); align-self: flex-end; }

.typing-dots { display: flex; align-items: center; gap: 4px; padding: 4px 2px !important; }
.typing-dots span { width: 7px; height: 7px; background: var(--text-muted); border-radius: 50%; animation: typing-bounce 1.4s ease infinite; }
.typing-dots span:nth-child(2) { animation-delay: 0.2s; }
.typing-dots span:nth-child(3) { animation-delay: 0.4s; }

/* ════════════════════════════════════════════════════════════
   CHATBOT — FOOTER / INPUT
   ════════════════════════════════════════════════════════════ */

.chat-footer {
  border-top: 1px solid var(--border-color); padding: 10px 12px 8px;
  background: var(--bg-card); flex-shrink: 0;
  transition: background-color 0.3s ease, border-color 0.3s ease;
}

.voice-active-bar {
  display: flex; align-items: center; gap: 8px;
  padding: 6px 10px; margin-bottom: 8px;
  background: var(--danger-bg); border: 1px solid rgba(239,68,68,0.2);
  border-radius: 8px; color: var(--danger); font-size: 0.78rem; font-weight: 600;
}
.voice-waves { display: flex; align-items: center; gap: 2px; margin-left: auto; }
.voice-waves span { width: 3px; border-radius: 3px; background: var(--danger); animation: voice-wave 0.7s ease infinite; }
.voice-waves span:nth-child(1) { height: 6px; }
.voice-waves span:nth-child(2) { height: 12px; animation-delay: 0.1s; }
.voice-waves span:nth-child(3) { height: 18px; animation-delay: 0.2s; }
.voice-waves span:nth-child(4) { height: 12px; animation-delay: 0.3s; }
.voice-waves span:nth-child(5) { height: 6px;  animation-delay: 0.4s; }

.input-row     { display: flex; align-items: center; gap: 7px; }
.input-wrapper { flex: 1; position: relative; }

.chat-input {
  width: 100%; padding: 9px 34px 9px 13px; border-radius: 12px;
  border: 1.5px solid var(--border-color);
  background: var(--bg-input) !important; color: var(--text-main) !important;
  font-size: 0.85rem; outline: none; transition: var(--transition);
}
.chat-input:focus        { border-color: var(--primary); box-shadow: 0 0 0 3px var(--primary-light); }
.chat-input::placeholder { color: var(--text-light) !important; }
.chat-input:disabled     { opacity: 0.6; cursor: not-allowed; }
.char-count { position: absolute; right: 10px; top: 50%; transform: translateY(-50%); font-size: 0.65rem; color: var(--text-light); }

.btn-mic {
  width: 38px; height: 38px; border-radius: 10px;
  border: 1.5px solid var(--border-color); background: var(--bg-input);
  color: var(--text-muted); cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; transition: var(--transition); flex-shrink: 0;
}
.btn-mic:hover { border-color: var(--primary); color: var(--primary); }
.mic-active    { background: var(--danger) !important; border-color: var(--danger) !important; color: #fff !important; animation: pulse-red 1.5s infinite; }

.btn-send {
  width: 38px; height: 38px; border-radius: 10px;
  border: none; background: var(--primary); color: #fff; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  font-size: 0.85rem; transition: var(--transition); flex-shrink: 0;
  box-shadow: var(--shadow-primary);
}
.btn-send:hover:not(:disabled) { background: var(--primary-dark); transform: scale(1.05); }
.btn-send:disabled             { opacity: 0.45; cursor: not-allowed; transform: none; }

.footer-note {
  margin-top: 7px; text-align: center; font-size: 0.64rem; color: var(--text-light);
  display: flex; align-items: center; justify-content: center; gap: 4px;
}

/* ════════════════════════════════════════════════════════════
   TRANSITIONS CHATBOT
   ════════════════════════════════════════════════════════════ */

.chat-slide-enter-active    { animation: chatSlideIn 0.32s cubic-bezier(0.34,1.56,0.64,1); }
.chat-slide-leave-active    { animation: chatSlideOut 0.22s ease-in forwards; }
.bubble-bounce-enter-active { animation: bubbleIn 0.4s cubic-bezier(0.34,1.56,0.64,1); }
.bubble-bounce-leave-active { animation: bubbleOut 0.2s ease forwards; }

/* ════════════════════════════════════════════════════════════
   KEYFRAMES
   ════════════════════════════════════════════════════════════ */

@keyframes ring-pulse    { 0% { transform: scale(1); opacity: 0.6; } 100% { transform: scale(1.7); opacity: 0; } }
@keyframes dot-pulse     { 0%, 100% { opacity: 1; } 50% { opacity: 0.4; } }
@keyframes loading-slide { 0% { background-position: 200% 0; } 100% { background-position: -200% 0; } }
@keyframes msgFadeIn     { from { opacity: 0; transform: translateY(8px); } to { opacity: 1; transform: translateY(0); } }
@keyframes typing-bounce { 0%, 60%, 100% { transform: translateY(0); } 30% { transform: translateY(-5px); } }
@keyframes pulse-red     { 0% { box-shadow: 0 0 0 0 rgba(239,68,68,0.4); } 100% { box-shadow: 0 0 0 10px rgba(239,68,68,0); } }
@keyframes voice-wave    { 0%, 100% { transform: scaleY(0.5); } 50% { transform: scaleY(1); } }
@keyframes chatSlideIn   { from { opacity: 0; transform: translateY(20px) scale(0.95); } to { opacity: 1; transform: translateY(0) scale(1); } }
@keyframes chatSlideOut  { from { opacity: 1; transform: translateY(0) scale(1); } to { opacity: 0; transform: translateY(16px) scale(0.95); } }
@keyframes bubbleIn      { from { transform: scale(0); opacity: 0; } to { transform: scale(1); opacity: 1; } }
@keyframes bubbleOut     { from { transform: scale(1); opacity: 1; } to { transform: scale(0); opacity: 0; } }

/* ════════════════════════════════════════════════════════════
   RESPONSIVE
   ════════════════════════════════════════════════════════════ */

@media (max-width: 768px) {
  .chat-window {
    width: calc(100vw - 20px);
    right: -14px; left: auto !important;
    height: 78vh; bottom: 78px; border-radius: var(--radius-lg);
  }
  .chatbot-wrapper { bottom: 16px; right: 16px; }
}

@media (max-width: 480px) {
  .chat-window {
    width: 100vw;
    right: -14px; left: auto !important;
    height: 85vh; bottom: 70px;
    border-radius: var(--radius-lg) var(--radius-lg) 0 0;
  }
  .chatbot-wrapper { bottom: 14px; right: 14px; }
  .chat-bubble     { width: 54px; height: 54px; font-size: 1.1rem; }
}
</style>
