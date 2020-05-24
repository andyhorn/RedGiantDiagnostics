import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import LogResults from "../views/LogResults.vue";
import Profile from "@/views/Profile.vue";
import UserSettings from "@/views/UserSettings.vue";
import store from "../store/index";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "Home",
    component: Home
  },
  {
    path: "/log/:id?",
    name: "Log",
    component: LogResults
  },
  {
    path: "/profile",
    name: "Profile",
    component: Profile
  },
  {
    path: "/settings",
    name: "UserSettings",
    component: UserSettings
  }
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes
});

router.beforeEach((to, from, next) => {
  if (to.name === "Log" && to.params.id == null && store.state.log == null)
    next({ name: "Home" });
  else
    next();
})

export default router;
