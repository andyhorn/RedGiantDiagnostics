import Vue from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import VueCookies from "vue-cookies";

import { http } from "./config/axios_config";

import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";
import { BootstrapVue, IconsPlugin } from "bootstrap-vue";

Vue.prototype.$http = http;
Vue.use(BootstrapVue);
Vue.use(IconsPlugin);
Vue.use(VueCookies);

Vue.$cookies.config("7d");

Vue.config.productionTip = false;

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
