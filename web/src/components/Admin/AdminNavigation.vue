<template>
  <div class="m-0 p-0 text-light">
    <h2 class="my-3">Administration Portal</h2>
    <b-list-group flush class="m-0 p-0">
      <b-list-group-item :active="isActive('home')" :to="{ name: 'Admin' }">
        <div>Home</div>
      </b-list-group-item>
      <b-list-group-item :active="isActive('logs')" :to="{ name: 'AdminLogs' }">
        <div>Logs</div>
      </b-list-group-item>
      <b-list-group-item
        :active="isActive('users')"
        :to="{ name: 'AdminUsers' }"
      >
        <div>Users</div>
      </b-list-group-item>
      <b-list-group-item
        :active="isActive('register')"
        :to="{ name: 'RegisterNewUser' }"
      >
        <div>Registration</div>
      </b-list-group-item>
    </b-list-group>
  </div>
</template>

<script>
export default {
  name: "AdminNavigation",
  props: ["activeSection"],
  methods: {
    isActive(name) {
      return name == this.activeSection;
    }
  }
};
</script>

<style scoped>
h2 {
  font-size: 1.2rem;
  text-align: center;
  text-decoration: underline;
}
.list-group {
  padding: 1px;
  border: none;
}
.list-group-item.active {
  background-color: #efefef;
  border-radius: 0;
  color: black;
  border: none;
  font-weight: 450;
}
.list-group-item {
  transition: background-color 500ms;
  border-radius: 0;
  border: none;
  background-color: rgba(0, 0, 0, 0);
  color: #eee;
}

.list-group-item > div {
  width: 0;
  margin: 0;
  padding: 0;
  transition: width 500ms;
  text-align: right;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
}

.list-group-item.active > div {
  width: 100%;
}
</style>
