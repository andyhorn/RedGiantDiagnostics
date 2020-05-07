const axios = require("axios");

var http;

if (process.env.NODE_ENV !== "production") {
  http = axios.create({
    baseURL: process.env.VUE_APP_BACKEND_URL
  });
} else {
  http = axios;
}

export default http;
