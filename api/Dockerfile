###################
### BUILD STAGE ###
###################

# Pull the DotNet Core SDK image
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-stage

# Set the working directory
WORKDIR /app

# Copy the dotnet project file
COPY *.csproj ./

# Run DotNet Restore
RUN dotnet restore

# Copy the application files
COPY . ./

# Build and publish the application
RUN dotnet publish -c Release -o out

########################
### PRODUCTION STAGE ###
########################

# Pull the production DotNet Core image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as runtime-stage

# Set the working directory
WORKDIR /app

# Copy the build artifacts
COPY --from=build-stage /app/out .

# Expose the port
EXPOSE 5000

# Set the Docker entrypoint
ENTRYPOINT [ "dotnet", "API.dll" ]