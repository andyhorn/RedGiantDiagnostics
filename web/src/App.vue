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
          <b-nav-item to="/login">Login</b-nav-item>
        </b-navbar-nav>

      </b-collapse>
    </b-navbar>

    <router-view />
  </div>
</template>

<script>
export default {
  name: 'App',
  data() {
    return {
      isLoggedIn: false,
      isAdmin: false
    }
  }
}
</script>

<style>

</style>
