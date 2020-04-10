### PRODUCTION MODE ###
#######################

###################
### BUILD STAGE ###
###################

# Pull the Node.js alpine image to build the Vue.js app
FROM node:lts-alpine as build-stage

# Set the working directory
WORKDIR /app

# Copy the project package files
COPY package*.json ./

# Install the app dependencies
RUN npm install

# Copy the application files
COPY . .

# Build the production server app
RUN npm run build


########################
### PRODUCTION STAGE ###
########################

# Pull the Nginx server Docker image
FROM nginx:alpine as production-stage

# Copy the build artifacts
COPY --from=build-stage /app/dist /usr/share/nginx/app

# Copy the Nginx Server configuration
ADD ./nginx/nginx.conf /etc/nginx/conf.d/default.conf