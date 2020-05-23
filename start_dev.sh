#!/bin/bash
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d database identity
cd ./web
VUE_APP_BACKEND_URL=http://localhost:5000/api/v2 npm run serve