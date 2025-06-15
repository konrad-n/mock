# Multi-stage build for React with nginx
# Build stage
FROM node:20-alpine AS build

# Set build arguments for API URL
ARG VITE_API_URL=http://localhost:5000

WORKDIR /app

# Copy package files for better caching
COPY package*.json ./
COPY packages/shared/package*.json ./packages/shared/
COPY packages/web/package*.json ./packages/web/

# Install dependencies
RUN npm ci

# Copy source code
COPY . .

# Build the application
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

# Production stage with nginx
FROM nginx:alpine AS final

# Install curl for health checks
RUN apk add --no-cache curl

# Copy custom nginx configuration
COPY nginx/default.conf /etc/nginx/conf.d/default.conf 2>/dev/null || \
    echo 'server { listen 80; location / { root /usr/share/nginx/html; try_files $uri $uri/ /index.html; } location /health { access_log off; return 200 "healthy\n"; add_header Content-Type text/plain; } }' > /etc/nginx/conf.d/default.conf

# Copy built application from build stage
COPY --from=build /app/packages/web/dist /usr/share/nginx/html

# Create non-root user for security
RUN addgroup -g 1000 -S appuser && \
    adduser -u 1000 -S appuser -G appuser && \
    chown -R appuser:appuser /usr/share/nginx/html && \
    chown -R appuser:appuser /var/cache/nginx && \
    chown -R appuser:appuser /var/log/nginx && \
    touch /var/run/nginx.pid && \
    chown -R appuser:appuser /var/run/nginx.pid

# Update nginx config to run as non-root
RUN sed -i 's/user  nginx;/user  appuser;/g' /etc/nginx/nginx.conf

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Switch to non-root user
USER appuser

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]