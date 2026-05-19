import axios from 'axios';

const AI_BASE = import.meta.env.VITE_API_BASE || 'http://127.0.0.1:8000';

const aiApi = axios.create({
  baseURL: AI_BASE
});

export const aiService = {
  async getSuggestions(role, lang) {
    const response = await aiApi.get(`/ia/chat/suggestions`, {
      params: { role, lang }
    });
    return response.data;
  },

  async sendMessage(message, role, sessionId) {
    const fd = new FormData();
    fd.append('message', message);
    fd.append('role', role);
    fd.append('lang', 'auto');
    fd.append('session_id', sessionId);
    
    const response = await aiApi.post(`/ia/chat`, fd, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  },

  async resetSession(sessionId) {
    const fd = new FormData();
    fd.append('session_id', sessionId);
    
    const response = await aiApi.post(`/ia/chat/reset`, fd, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    return response.data;
  }
};

export default aiService;
