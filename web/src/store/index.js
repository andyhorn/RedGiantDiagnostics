import Vue from "vue";
import Vuex from "vuex";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    status: "",
    log: null,
    user: null,
    error: "",
    token: localStorage.getItem("red-giant-token") || ""
  },
  mutations: {
    view_log(state, logData) {
      state.status = "received log";
      state.log = logData;
    }
  },
  actions: {

  },
  getters: {
    log: (state) => state.log != null ? state.log : false
  },
  modules: {

  }
});
