# Build Stage
FROM node:20-alpine as build-stage
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
# Vue-TSC compila Typescript e o Vite empacota.
RUN npm run build

# Production Stage (Nginx)
FROM nginx:stable-alpine as production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Expor Porta padrão para Web
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
