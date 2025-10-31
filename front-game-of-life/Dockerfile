# Dockerfile
# Etapa 1: build
FROM node:18 AS build
WORKDIR /app

COPY package*.json ./
COPY . .

ARG VITE_API_URL
RUN echo "VITE_API_URL=$VITE_API_URL" > .env

RUN npm install
RUN npm run build

# Etapa 2: production with nginx
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
