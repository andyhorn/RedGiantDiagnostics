import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import LogResults from "../views/LogResults.vue";

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

export default router;
