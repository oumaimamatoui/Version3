<template>
  <div class="chatbot-wrapper">
  
    <div @click="isOpen = !isOpen" class="chat-bubble shadow-lg">
      <i v-if="!isOpen" class="fa fa-robot fa-2x"></i>
      <i v-else class="fa fa-times fa-2x"></i>
    </div>


    <div v-if="isOpen" class="chat-window shadow-lg animate__animated animate__fadeInUp">
      <div class="chat-header bg-primary p-3 d-flex justify-content-between align-items-center">
        <h6 class="mb-0 text-white"><i class="fa fa-magic me-2"></i>Assistant NeoStage</h6>
        <span class="badge bg-success">Online</span>
      </div>
      
      <div class="chat-body p-3">
        <div v-for="(msg, index) in messages" :key="index" :class="['message mb-2', msg.role]">
          <div class="message-content p-2 rounded shadow-sm">
            {{ msg.text }}
          </div>
        </div>
      </div>

      <div class="chat-footer p-2 border-top border-dark">
        <div class="input-group">
          <input v-model="userInput" @keyup.enter="sendMessage" type="text" class="form-control bg-dark text-white border-dark" placeholder="Posez une question...">
          <button @click="sendMessage" class="btn btn-primary"><i class="fa fa-paper-plane"></i></button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';

const isOpen = ref(false);
const userInput = ref('');
const messages = ref([
  { role: 'ai', text: 'Bonjour ! Comment puis-je vous aider ?' }
]);

const sendMessage = () => {
  if (!userInput.value) return;
  messages.value.push({ role: 'user', text: userInput.value });
  userInput.value = '';
  setTimeout(() => {
    messages.value.push({ role: 'ai', text: 'Je suis une simulation d\'IA pour votre .' });
  }, 1000);
};
</script>

<style scoped>
.chatbot-wrapper { position: fixed; bottom: 30px; right: 30px; z-index: 9999; }
.chat-bubble {
  width: 60px; height: 60px; background: #eb1616; color: white;
  border-radius: 50%; display: flex; align-items: center; justify-content: center;
  cursor: pointer; box-shadow: 0 4px 15px rgba(235, 22, 22, 0.4);
}
.chat-window {
  position: absolute; bottom: 80px; right: 0; width: 300px; height: 400px;
  background: #191c24; border-radius: 10px; display: flex; flex-direction: column;
  border: 1px solid #333;
}
.chat-body { flex-grow: 1; overflow-y: auto; display: flex; flex-direction: column; }
.message.ai { align-self: flex-start; }
.message.ai .message-content { background: #2a2e3a; color: white; }
.message.user { align-self: flex-end; }
.message.user .message-content { background: #eb1616; color: white; }
.message-content { font-size: 0.8rem; }
</style>