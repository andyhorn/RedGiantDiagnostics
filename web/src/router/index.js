import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import LogResults from "../views/LogResults.vue";
import store from "../store/index";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/log",
    name: "Log",
    component: LogResults
  },
  {
    path: "/log/:id",
    name: "LogById",
    component: LogResults
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

router.beforeEach((to, from, next) => {
  if (to.name === "Log" && to.params.id !== null
    && Object.keys(store.getters.log).length == 0)
    next({ name: "Home" });
  else
    next();
})

export default router;
