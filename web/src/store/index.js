import Vue from "vue";
import Vuex from "vuex";
const logService = require("../services/logService");
const authenticationService = require('../services/authenticationService');
const userService = require("../services/userService");

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    status: "",
    log: {},
    user: {},
    userId: "",
    error: "",
    token: localStorage.getItem("red-giant-token") || "",
    isAuthenticated: false
  },
  mutations: {
    retrieving_log(state) {
      state.status = "retrieving...";
      state.log = null;
    },
    saving_log(state) {
      state.status = "saving log...";
    },
    log_saved(state) {
      state.status = "log saved";
    },
    log_save_failure(state) {
      state.status = "log not saved";
    },
    log_retrieved(state, logData) {
      state.status = "log retrieved";
      state.log = logData;
    },
    retrieval_failure(state, err) {
      state.status = "failed to retrieve log";
      state.err = err;
    },
    authenticating(state) {
      state.status = "authenticating";
    },
    authentication_success(state, data) {
      state.status = "authenticated";
      state.token = data.token;
      state.userId = data.userId;
      state.isAuthenticated = true;
    },
    authentication_failure(state, err) {
      state.status = "authentication failed";
      state.error = err;
      state.isAuthenticated = false;
    },
    fetching_user(state) {
      state.status = "fetching user data";
    },
    fetch_success(state, user) {
      state.status = "user data fetched";
      state.user = user;
    },
    fetch_failure(state, err) {
      state.status = "failed to fetch user data";
      state.error = err;
    },
    logout(state) {
      state.status = "";
      state.user = {};
      state.userId = "";
      state.error = "";
      state.token = "";
      localStorage.removeItem("red-giant-token");
      state.isAuthenticated = false;
    }
  },
  actions: {
    get_log_by_id({ commit }, id) {
      return new Promise(() => {
        commit("retrieving_log");
        logService.getById(id)
          .then((log) => commit("retrieved", log))
          .catch((err) => commit("retrieval_failure", err));
      })
    },
    login({ commit }, data) {
      commit("authenticating");
      authenticationService.login(data.email, data.password)
        .then((res) => {
          console.log("authentication successful")
          console.log(res)
          commit("authentication_success", res.data);

          Vue.prototype.$http.defaults.headers.common["Authorization"] = res.data.token;

          if (data.rememberMe) {
            localStorage.setItem("red-giant-token", res.data.token);
          }

          this.dispatch("fetch_user");
        })
        .catch((err) => {
          console.log("authentication failed")
          console.log(err)
          commit("authentication_failure", err);
        });
    },
    fetch_user({ commit }) {
      if (this.state.userId != "") {
        commit("fetching_user");
        // userService.getUserById(this.state.userId)
        userService.getUserData()
          .then((res) => {
            commit("fetch_success", res.data);
          })
          .catch((err) => {
            commit("fetch_failure", err);
          });
      }
    },
    async save_log({ commit }, log) {
      commit("saving_log");

      let logId = "";
      let success = false;
      
      if (this.state.log.id) {
        success = await logService.updateLog(log) !== "";
        logId = log.id;
      } else {
        logId = await logService.saveLog(log);
        success = logId !== "";
      }

      if (success) {
        commit("log_saved");
        this.dispatch("get_log_by_id", logId);
      } else {
        commit("log_save_failure");
      }

    },
    logout({ commit }) {
      commit("logout");
    }
  },
  getters: {
    log: (state) => state.log || false,
    user: (state) => state.user,
    userId: state => state.userId,
    token: state => state.token,
    isAuthenticated: state => state.isAuthenticated
  },
  modules: {

  }
});
