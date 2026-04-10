# Stage 1: Build Angular
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npx ng build --configuration production

# Stage 2: Serve with Nginx
FROM nginx:1.27-alpine AS runtime

COPY --from=build /app/dist/kudos-ui/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
