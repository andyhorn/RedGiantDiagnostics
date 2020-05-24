import Vue from "vue";
import Vuex from "vuex";
import createPersistedState from "vuex-persistedstate";
const logService = require("../services/logService");
const authenticationService = require('../services/authenticationService');
const userService = require("../services/userService");
const http = require("@/config/axios_config");

Vue.use(Vuex);

const COOKIE_KEY = "X-RedGiant-Token-X";
const TOKEN_KEY = "red-giant-token";

const persistedStateOptions = {
  paths: ['log', 'user', 'userId', 'isAuthenticated']
}

const defaultState = function() {
  return {
    status: "",
    log: null,
    logId: null,
    user: null,
    userId: null,
    userLogs: null,
    error: null,
    token: null,
    isAuthenticated: false
  }
}

export default new Vuex.Store({
  state: defaultState(),
  mutations: {
    retrieving_log(state, id) {
      state.status = "retrieving...";
      state.logId = id;
      state.log = null;
    },
    saving_log(state) {
      state.status = "saving log...";
    },
    log_saved(state, log) {
      state.status = "log saved";
      state.log = log;
      state.logId = log.id;
      new Vue().$bvToast.toast("Log saved successfully!", {
        title: "Log saved",
        variant: "success",
        isStatus: true,
        autoHideDelay: 3000,
        toaster: "b-toaster-bottom-right"
      });
    },
    log_save_failure(state) {
      state.status = "log not saved";
      new Vue().$bvToast.toast("There was an error saving the log.", {
        title: "Log Save Error",
        variant: "danger",
        autoHideDelay: 3000,
        toaster: "b-toaster-top-center"
      });
    },
    log_retrieved(state, logData) {
      state.status = "log retrieved";
      state.log = logData;
    },
    retrieval_failure(state, err) {
      state.status = "failed to retrieve log";
      state.err = err;
      new Vue().$bvToast.toast("There was an error downloading this log file. Refresh and try again.", {
        title: "Error",
        variant: "warning",
        autoHideDelay: 3000
      });
    },
    authenticating(state) {
      state.status = "authenticating";
    },
    authentication_success(state, data) {
      state.status = "authenticated";
      state.token = data.token;
      state.userId = data.userId;
      state.isAuthenticated = true;
      localStorage.setItem(TOKEN_KEY, data.token);
      new Vue().$bvToast.toast("You have been logged in.", {
        title: "Logged In",
        variant: "success",
        isStatus: true,
        autoHideDelay: 3000,
        toaster: "b-toaster-bottom-right"
      });
    },
    authentication_failure(state, err) {
      state.status = "authentication failed";
      state.error = err;
      state.isAuthenticated = false;
      new Vue().$bvToast.toast("There was an error logging in. Check your credentials and try again.", {
        title: "Login Failed",
        variant: "danger",
        toaster: "b-toaster-top-center"
      });
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
      new Vue().$bvToast.toast("There was an error retrieving your user data. Refresh to try again.", {
        title: "Error",
        variant: "danger",
        toaster: "b-toaster-top-center"
      });
    },
    fetching_user_logs(state) {
      state.status = "retrieving user logs";
    },
    user_logs_retrieved(state, logs) {
      state.status = "logs retrieved";
      state.userLogs = logs;
    },
    logout() {
      this.replaceState(defaultState());
      localStorage.removeItem(TOKEN_KEY);
      Vue.prototype.$cookies.remove(COOKIE_KEY);
      new Vue().$bvToast.toast("You have been logged out.", {
        title: "Logout Successful",
        variant: "success",
        autoHideDelay: 3000,
        toaster: "b-toaster-top-full"
      });
    }
  },
  actions: {
    getLogById({ commit }, id) {
      return new Promise(() => {
        commit("retrieving_log", id);
        logService.getById(id)
          .then((log) => commit("log_retrieved", log))
          .catch((err) => commit("retrieval_failure", err));
      })
    },
    login({ commit }, data) {
      commit("authenticating");
      return new Promise((resolve, reject) => {
        authenticationService.login(data.email, data.password)
          .then((res) => {
            commit("authentication_success", res.data);
  
            http.addAuthorization(res.data.token);
  
            if (data.rememberMe) {
              Vue.prototype.$cookies.set()
            }

            return resolve();
          })
          .catch((err) => {
            commit("authentication_failure", err);
            return reject();
          });
      })
    },
    fetchUser({ commit }, force) {
      return new Promise((resolve, reject) => {
        if (force || (this.state.userId != "" && this.state.user == null)) {
          commit("fetching_user");
          userService.getUserData()
            .then((res) => {
              commit("fetch_success", res.data);
              return resolve();
            })
            .catch((err) => {
              commit("fetch_failure", err);
              return reject();
            });
        } else {
          return resolve();
        }
      })
    },
    async save_log({ commit }, log) {
      commit("saving_log");

      let logResponse = null;
      if (this.state.log.id) {
        // If the current log already has an ID, then this is an update request
        logResponse = await logService.updateLog(log);
      } else {
        // If the current log doesn't have an ID, then this is a new save request
        logResponse = await logService.saveLog(log);
      }

      if (logResponse) {
        commit("log_saved", logResponse);
      } else {
        commit("log_save_failure");
      }

    },
    async fetchUserLogs({ commit }, force = false) {
      if (!this.state.userLogs || force) {
        commit("fetching_user_logs");
        let logs = await logService.getLogsForCurrentUser();
        commit("user_logs_retrieved", logs);
      }
    },
    logout({ commit }) {
      commit("logout");
      http.removeAuthorization();
    }
  },
  getters: {
    isAdmin: state => {
      if (state.user) {
        return state.user.roles.includes("Administrator");
      } else {
        return false;
      }
    }
  },
  modules: {

  },
  plugins: [
    createPersistedState(persistedStateOptions)
  ]
});
