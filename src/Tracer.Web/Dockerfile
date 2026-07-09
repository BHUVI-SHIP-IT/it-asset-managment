# Stage 1: Build the Angular app
FROM node:22-alpine AS builder

WORKDIR /app

# Copy package files and install dependencies
COPY package*.json ./
RUN npm ci

# Copy the rest of the application code
COPY . .

# Build the Angular app in production mode
RUN npm run build --configuration=production

# Stage 2: Serve the app using Nginx
FROM nginx:alpine

# Copy the custom Nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copy the built assets from the builder stage
# For Angular 17+, the build output is usually under dist/<project-name>/browser
COPY --from=builder /app/dist/Tracer.Web/browser /usr/share/nginx/html

# Expose port 80
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
