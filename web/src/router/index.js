import Vue from "vue";
import VueRouter from "vue-router";
import Home from "../views/Home.vue";
import LogResults from "../views/LogResults.vue";
import Profile from "@/views/Profile.vue";
import UserSettings from "@/views/UserSettings.vue";
import Admin from "@/views/Admin.vue";
import AdminUsers from "@/components/Admin/AdminUsers.vue";
import AdminLogs from "@/components/Admin/AdminLogs.vue";
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
        path: '/admin/users',
        name: 'AdminUsers',
        component: AdminUsers
      },
      {
        path: '/admin/logs',
        name: 'AdminLogs',
        component: AdminLogs
      }
    ]
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
