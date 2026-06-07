FROM node:20-alpine AS build
ARG VITE_API_URL
ARG VITE_AI_URL
ARG VITE_BASE_URL
ARG VITE_GOOGLE_CLIENT_ID
ARG VITE_AI_INTERNAL_API_KEY
ENV VITE_API_URL=$VITE_API_URL
ENV VITE_AI_URL=$VITE_AI_URL
ENV VITE_BASE_URL=$VITE_BASE_URL
ENV VITE_GOOGLE_CLIENT_ID=$VITE_GOOGLE_CLIENT_ID
ENV VITE_AI_INTERNAL_API_KEY=$VITE_AI_INTERNAL_API_KEY
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN ./node_modules/.bin/vite build

FROM nginx:alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]