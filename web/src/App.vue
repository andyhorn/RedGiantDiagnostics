<template>
  <div id="app">
    <b-navbar toggleable="md" type="dark" variant="dark">
      <b-navbar-brand href="#">Red Giant Diagnostics</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>

        <b-navbar-nav class="ml-auto">
          <b-nav-item to="/"><b-icon-house-fill /> Home</b-nav-item>
        </b-navbar-nav>

        <b-navbar-nav v-if="isAuthenticated">
          <b-nav-item v-if="isAdmin" to="/admin"><b-icon-wrench /> Manage</b-nav-item>
          <b-nav-item to="/profile"><b-icon-person-fill /> Profile</b-nav-item>
          <b-nav-item @click="onLogout"><b-icon-reply-fill /> Logout</b-nav-item>
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
      return this.$store.state.isAuthenticated;
    },
    isAdmin () {
      return this.$store.getters.isAdmin;
    }
  },
  methods: {
    onLogout() {
      this.$store.dispatch("logout");
      if (this.$route.name != "Home" && this.$route.name != "Log")
        this.$router.push({ name: "Home" });
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
a {
  font-weight: 350;
  font-size: 1.1rem;
}
.text-subtle {
  font-weight: 300;
  font-size: .9rem;
}
.text-bold {
  font-weight: 600;
  font-size: 1.2rem;
}
a.navbar-brand {
  font-weight: 300;
}
</style>
