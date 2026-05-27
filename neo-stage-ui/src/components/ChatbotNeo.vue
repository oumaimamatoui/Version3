<template>
  <div class="chatbot-wrapper">
    <!-- Bubble -->
    <div @click="isOpen = !isOpen" class="chat-bubble shadow-lg">
      <i v-if="!isOpen" class="fa fa-robot fa-2x"></i>
      <i v-else class="fa fa-times fa-2x"></i>
    </div>

    <!-- Window -->
    <div v-if="isOpen" class="chat-window shadow-lg animate__animated animate__fadeInUp">
      <div class="chat-header bg-primary p-3 d-flex justify-content-between align-items-center">
        <h6 class="mb-0 text-white"><i class="fa fa-magic me-2"></i>Assistant NeoStage</h6>
        <span class="badge bg-success">En ligne</span>
      </div>
      
      <div class="chat-body p-3" ref="chatBody">
        <div v-for="(msg, index) in messages" :key="index" :class="['message mb-2', msg.role]">
          <div class="message-content p-2 rounded shadow-sm">
            {{ msg.text }}
            <!-- Petit bouton pour réécouter si besoin -->
            <i v-if="msg.role === 'ai'" @click="speakText(msg.text)" class="fa fa-volume-up ms-2 listen-icon"></i>
          </div>
        </div>
        <div v-if="isLoading" class="message ai">
           <div class="message-content p-2 rounded shadow-sm italic text-muted">Réflexion...</div>
        </div>
      </div>

      <div class="chat-footer p-2 border-top border-dark">
        <div class="input-group">
          <!-- BOUTON MICRO -->
          <button 
            @click="toggleListen" 
            class="btn btn-outline-secondary" 
            :class="{'is-listening': isListening}"
            title="Parler au micro"
          >
            <i class="fa" :class="isListening ? 'fa-microphone-slash' : 'fa-microphone'"></i>
          </button>

          <input 
            v-model="userInput" 
            @keyup.enter="sendMessage()" 
            type="text" 
            class="form-control bg-dark text-white border-dark" 
            placeholder="Posez une question..."
            :disabled="isLoading"
          >
          
          <button @click="sendMessage()" class="btn btn-primary" :disabled="isLoading">
            <i class="fa fa-paper-plane"></i>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, nextTick } from 'vue';

const isOpen = ref(false);
const userInput = ref('');
const isLoading = ref(false);
const isListening = ref(false); // État du micro
const chatBody = ref(null);
const messages = ref([
  { role: 'ai', text: 'Bonjour ! Je suis NeoStage, comment puis-je vous aider aujourd\'hui ?' }
]);

// --- FONCTION VOCALE : PARLER (Text-to-Speech) ---
const speakText = (text) => {
  if ('speechSynthesis' in window) {
    // Annuler les lectures en cours
    window.speechSynthesis.cancel();
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = 'fr-FR';
    utterance.rate = 1.1; // Vitesse légèrement plus rapide
    window.speechSynthesis.speak(utterance);
  }
};

// --- FONCTION VOCALE : ÉCOUTER (Speech-to-Text) ---
const toggleListen = () => {
  const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
  
  if (!SpeechRecognition) {
    alert("Désolé, votre navigateur ne supporte pas la reconnaissance vocale.");
    return;
  }

  const recognition = new SpeechRecognition();
  recognition.lang = 'fr-FR';
  recognition.continuous = false;
  recognition.interimResults = false;

  recognition.onstart = () => {
    isListening.value = true;
  };

  recognition.onresult = (event) => {
    const transcript = event.results[0][0].transcript;
    userInput.value = transcript;
    sendMessage(true); // Envoyer automatiquement après dictée
  };

  recognition.onerror = () => {
    isListening.value = false;
  };

  recognition.onend = () => {
    isListening.value = false;
  };

  recognition.start();
};

const scrollToBottom = async () => {
  await nextTick();
  if (chatBody.value) {
    chatBody.value.scrollTop = chatBody.value.scrollHeight;
  }
};

const sendMessage = async (isVoice = false) => {
  if (!userInput.value.trim() || isLoading.value) return;

  const userText = userInput.value;
  messages.value.push({ role: 'user', text: userText });
  userInput.value = '';
  isLoading.value = true;
  await scrollToBottom();

  try {
    const formData = new FormData();
    formData.append('message', userText);

    const response = await fetch('http://127.0.0.1:8000/ia/chat', {
      method: 'POST',
      body: formData
    });

    const data = await response.json();

    if (data.reply) {
      messages.value.push({ role: 'ai', text: data.reply });
      // Si l'utilisateur a utilisé le micro, on lui répond à la voix
      if (isVoice) {
        speakText(data.reply);
      }
    } else {
      messages.value.push({ role: 'ai', text: "Désolé, j'ai rencontré une erreur." });
    }
  } catch (error) {
    messages.value.push({ role: 'ai', text: "Serveur indisponible." });
  } finally {
    isLoading.value = false;
    await scrollToBottom();
  }
};
</script>

<style scoped>
.chatbot-wrapper { position: fixed; bottom: 30px; right: 30px; z-index: 9999; font-family: 'Segoe UI', sans-serif; }
.chat-bubble {
  width: 60px; height: 60px; background: #eb1616; color: white;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  cursor: pointer; box-shadow: 0 4px 15px rgba(235, 22, 22, 0.4); transition: transform 0.3s;
}
.chat-bubble:hover { transform: scale(1.1); }
.chat-window {
  position: absolute; bottom: 80px; right: 0; width: 320px; height: 450px;
  background: #191c24; border-radius: 10px; display: flex; flex-direction: column;
  border: 1px solid #333; overflow: hidden;
}
.chat-body { flex-grow: 1; overflow-y: auto; display: flex; flex-direction: column; scroll-behavior: smooth; }
.message.ai { align-self: flex-start; max-width: 85%; }
.message.ai .message-content { background: #2a2e3a; color: white; border-bottom-left-radius: 2px; }
.message.user { align-self: flex-end; max-width: 85%; }
.message.user .message-content { background: #eb1616; color: white; border-bottom-right-radius: 2px; }
.message-content { font-size: 0.85rem; position: relative; }

/* Icône pour réécouter */
.listen-icon { cursor: pointer; opacity: 0.6; font-size: 0.7rem; }
.listen-icon:hover { opacity: 1; color: #eb1616; }

/* Animation micro activé */
.is-listening {
  background-color: #eb1616 !important;
  color: white !important;
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.05); }
  100% { transform: scale(1); }
}

.italic { font-style: italic; }
</style>