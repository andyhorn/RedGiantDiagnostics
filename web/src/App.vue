<template>
  <div id="app">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-brand href="#">Red Giant Diagnostics</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>

        <b-navbar-nav class="ml-auto">
          <b-nav-item to="/">Home</b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav v-if="isLoggedIn">
          <b-nav-item v-if="isAdmin" to="/admin">Manage</b-nav-item>
          <b-nav-item to="/profile">Profile</b-nav-item>
          <b-nav-item to="/logout">Logout</b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav else>
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
  data() {
    return {
      isLoggedIn: this.$store.getters.token,
      isAdmin: false
    }
  },
  mounted() {
    this.$store.watch((state) => {
      if (state.userId !== "" && Object.keys(state.user).length === 0) {
        this.$store.dispatch("fetch_user");
      }
      if (Object.keys(state.user).length > 0) {
        this.isAdmin = this.$store.getters.user.roles.includes("Admin");
      }
    })
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
