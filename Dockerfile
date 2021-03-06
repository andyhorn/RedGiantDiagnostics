#########################
### BUILD THE VUE APP ###
#########################

# Pull the alpine linux image
FROM node as client-build-stage

# Install the vue-cli tool
RUN npm install @vue/cli-service

# Set the working directory
WORKDIR /app

# Copy the project package files
COPY ./web/package*.json ./

# Install the client dependencies
RUN npm install

# Copy the application files
COPY ./web ./

# Build the production app
RUN npm run build

################################
### BUILD THE DOTNET BACKEND ###
################################

# Pull the DotNet Core SDK
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS dotnet-build-stage

# Set the working directory
WORKDIR /app

# Copy the project file
COPY ./api/API.csproj ./

# Run the restore
RUN dotnet restore

# Copy the application files
COPY ./api ./

# Build the application
RUN dotnet publish "/app/API.csproj" -c Release -o out

################################
### BUILD THE PRODUCTION APP ###
################################

# Pull the production DotNet Core image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as runtime

# Set the working directory to the root web folder
WORKDIR /app/wwwroot

# Copy the Vue build artifacts
COPY --from=client-build-stage /app/dist .

# Set the working directory to the main app folder
WORKDIR /app

# Copy the DotNet build artifacts
COPY --from=dotnet-build-stage /app/out .

# Expose the main port
EXPOSE 80

ENTRYPOINT ["dotnet", "API.dll"]