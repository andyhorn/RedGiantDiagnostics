const axios = require("axios");

const baseURL = process.env.VUE_APP_BACKEND_URL || null;
const options = {
  headers: {
    'Content-Type': 'application/json'
  }
}

if (baseURL) options.baseURL = baseURL;

const http = axios.create(options);

// if (process.env.NODE_ENV !== "production") {
//   http = axios.create({
//     baseURL: process.env.VUE_APP_BACKEND_URL,
//     headers: {
//       'Content-Type': 'application/json'
//     }
//   });
// } else {
//   http = axios.create();
// }

const addAuthorization = function(token) {
  http.defaults.headers.common["Authorization"] = token;
}

const removeAuthorization = function() {
  http.defaults.headers.common["Authorization"] = null;
}

export {
  http,
  addAuthorization,
  removeAuthorization
}