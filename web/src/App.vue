<template>
  <div id="app">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-brand href="#">Red Giant Diagnostics</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>

        <b-navbar-nav class="ml-auto">
          <b-nav-item to="/">Home</b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav v-if="isAuthenticated">
          <b-nav-item v-if="isAdmin" to="/admin">Manage</b-nav-item>
          <b-nav-item to="/profile">Profile</b-nav-item>
          <b-nav-item @click="onLogout">Logout</b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav v-if="!isAuthenticated">
          <Login />
        </b-navbar-nav>

      </b-collapse>
    </b-navbar>

    <router-view />
  </div>
</template>

<script>
import Login from "@/views/Login";

export default {
  name: 'App',
  components: {
    Login
  },
  computed: {
    isAuthenticated() {
      return this.$store.getters.isAuthenticated;
    },
    isAdmin() {
      if (this.$store.getters.user.roles)
        return this.$store.getters.user.roles.includes("Administrator");
      else return false;
    },
    hasLog() {
      if (this.$store.getters.log && Object.keys(this.$store.getters.log).length > 0)
        return true;
      else
        return false;
    }
  },
  methods: {
    onLogout() {
      this.$store.dispatch("logout");
    }
  }
}
</script>

<style>
p {
  font-family: 'Courier New', Courier, monospace;
  font-weight: 700;
}
h1, h2, h3, h4, h5, h6 {
  font-family: 'Roboto', 'Segoe UI', sans-serif;
  font-weight: 400;
}
</style>
