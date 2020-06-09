<template>
  <div id="admin-container">
    <div id="left-nav">
      <AdminNavigation :activeSection="activeSection" />
    </div>
    <div id="admin-view" class="pt-5">
      <router-view />
    </div>
  </div>
</template>

<script>
import AdminNavigation from "@/components/Admin/AdminNavigation.vue";

export default {
  name: "Admin",
  components: {
    AdminNavigation
  },
  data() {
    return {};
  },
  computed: {
    activeSection() {
      if (this.$route.name == "AdminAnalytics") return "home";
      else if (this.$route.name == "AdminLogs") return "logs";
      else if (this.$route.name == "AdminUsers") return "users";
      else if (this.$route.name == "RegisterNewUser") return "register";
      else return "";
    }
  }
};
</script>

<style scoped>
#admin-container {
  width: 100vw;
  height: 100vh;
  position: fixed;
}
#left-nav {
  width: 13rem;
  min-width: 13rem;
  max-width: 13rem;
  height: 100vh;
  position: absolute;
  left: 0;
  background-color: #5e807f;
  /* border-right: 1px solid black; */
  box-shadow: 1px 0 5px black;
}
#admin-view {
  padding-left: 13rem;
  width: 100%;
  height: 100%;
  overflow-y: auto;
  padding-bottom: 5rem;
}
</style>
