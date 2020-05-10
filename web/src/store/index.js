import Vue from "vue";
import Vuex from "vuex";
const logService = require("../services/logService");

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
    retrieving_log(state) {
      state.status = "retrieving...";
      state.log = null;
    },
    log_retrieved(state, logData) {
      state.status = "log retrieved";
      state.log = logData;
    },
    retrieval_failure(state, err) {
      state.status = "failed to retrieve log";
      state.err = err;
    }
  },
  actions: {
    getLogById({ commit }, id) {
      return new Promise(() => {
        commit("retrieving_log");
        logService.getById(id)
          .then((log) => commit("retrieved", log))
          .catch((err) => commit("retrieval_failure", err));
      })
    }
  },
  getters: {
    log: (state) => state.log != null ? state.log : false
  },
  modules: {

  }
});
