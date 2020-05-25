const axios = require("axios");

const baseURL = process.env.VUE_APP_BACKEND_URL || null;
const options = {
  headers: {
    'Content-Type': 'application/json'
  }
}

if (baseURL) options.baseURL = baseURL;

const http = axios.create(options);

const addAuthorization = function(token) {
  http.defaults.headers.common["Authorization"] = "Bearer " + token;
}

const removeAuthorization = function() {
  http.defaults.headers.common["Authorization"] = null;
}

// If a token is present on launch, add it to the headers
const token = localStorage.getItem("red-giant-token");
if (token) {
  addAuthorization(token);
}

export {
  http,
  addAuthorization,
  removeAuthorization
}