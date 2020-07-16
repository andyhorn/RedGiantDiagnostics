# This file serves as a setup script that should install the application and all of its dependencies.
# Author:   Andy Horn
# Date:     July 15, 2020

#!/bin/bash
EXISTS=0

function exists {
    if ! command -v $! &> /dev/null
    then
        EXISTS=0
    else
        EXISTS=1
    fi
}

# First, check if Docker and Docker Compose are installed
exists docker
if [ $EXISTS == 0 ]; then
    apt-get install docker-ce -y
fi
echo Docker installed.

exists docker-compose
if [ $EXISTS == 0 ]; then
    apt-get install docker-compose -y
fi
echo Docker-Compose installed

# Check if NPM is installed
exists npm
if [ $EXISTS == 0 ]; then
    apt-get install npm -y
fi
echo NPM installed

exists vue
if [ $EXISTS == 0 ]; then
    npm install -g vuejs
fi
echo Vue.js installed

exists vue-cli-service
if [ $EXISTS == 0 ]; then
    npm install -g @vue/cli-service
fi
echo Vue-Cli-Service installed

cd ./web
npm install
