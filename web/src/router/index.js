import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import LogResults from "../views/LogResults.vue";
import Profile from "@/views/Profile.vue";
import UserSettings from "@/views/UserSettings.vue";
import Admin from "@/views/Admin.vue";
import AdminAnalytics from "@/components/Admin/AdminAnalytics.vue";
import AdminUsers from "@/components/Admin/AdminUsers.vue";
import AdminLogs from "@/components/Admin/AdminLogs.vue";
import RegisterNewUser from "@/components/Admin/RegisterNewUser.vue";
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
  },
  {
    path: "/admin",
    name: "Admin",
    component: Admin,
    children: [
      {
        path: "",
        name: "AdminAnalytics",
        component: AdminAnalytics
      },
      {
        path: "/admin/users",
        name: "AdminUsers",
        component: AdminUsers
      },
      {
        path: "/admin/logs",
        name: "AdminLogs",
        component: AdminLogs
      },
      {
        path: "/admin/register",
        name: "RegisterNewUser",
        component: RegisterNewUser
      }
    ]
  }
];

const router = new VueRouter({
  mode: "history",
  routes
});

router.beforeEach((to, from, next) => {
  if (to.name === "Log" && to.params.id == null && store.state.log == null)
    next({ name: "Home" });
  else next();
});

export default router;
