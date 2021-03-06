<template>
  <div class="container">
    <h1 class="title">User Management</h1>
    <p class="subtitle">Select a user from the list below to edit their information.</p>
    <div class="text-center" v-if="isLoading">
      <b-spinner label="Loading" />
    </div>
    <b-table
      outlined
      head-variant="dark"
      v-if="!isLoading && users.length > 0"
      :items="users"
      :fields="tableFields"
      selectable
      select-mode="single"
      @row-selected="onRowSelected"
    >
      <template v-slot:cell(options)="data">
        <b-button
          v-if="!isCurrentUser(data.item.userId)"
          @click="onDelete(data.item.userId)"
          variant="danger"
          size="sm"
        >Delete</b-button>
      </template>
      <template v-slot:cell(roles)="data">{{ makeRoles(data.item.roles) }}</template>
    </b-table>
    <div v-if="selectedUser.length > 0">
      <div v-if="selectedUser[0].userId == $store.state.user.userId">
        <p>
          Please visit your
          <router-link :to="{ name: 'UserSettings' }">Account Settings</router-link>to make changes to your own account.
        </p>
      </div>
      <div v-else>
        <hr />
        <AdminUserEdit :user="selectedUser[0]" @refreshUser="onRefreshUser" />
      </div>
    </div>
  </div>
</template>

<script>
import AdminUserEdit from "@/components/Admin/AdminUserEdit.vue";
const userService = require("@/services/userService");
const toastService = require("@/services/toastService");

export default {
  name: "AdminUsers",
  components: {
    AdminUserEdit
  },
  data() {
    return {
      isLoading: true,
      users: [],
      selectedUser: [],
      tableFields: [{ key: "email", sortable: true }, "roles", "options"]
    };
  },
  mounted() {
    this.fetchUsers();
  },
  methods: {
    async fetchUsers() {
      await this.$store.dispatch("fetchAllUsers");
      this.users = this.$store.state.userList;
      this.isLoading = false;
    },
    onRowSelected(item) {
      this.selectedUser = item;
    },
    makeRoles(list) {
      let str = "";

      for (let i = 0; i < list.length; i++) {
        str += list[i];

        if (i < list.length - 1) {
          str += ", ";
        }
      }

      return str;
    },
    async onRefreshUser(userId) {
      let user = await userService.getUserByIdAdmin(userId);
      this.selectedUser = [user];
      this.$store.commit("updated_user_data", [userId, user]);
    },
    async onDelete(userId) {
      if (userId == this.$store.state.user.userId) return;

      if (
        confirm(
          "Are you sure you want to permanently delete this user? " +
            "You can also disable their User status to prevent them from logging in."
        )
      ) {
        let success = await userService.deleteUserAdmin(userId);
        if (success) {
          this.fetchUsers();
          toastService.successToast("Deleted", "The user has been deleted.");
        } else {
          toastService.errorToast(
            "Error",
            "There was an error that prevented this user from being deleted."
          );
        }
      }
    },
    isCurrentUser(id) {
      return id == this.$store.state.user.userId;
    }
  }
};
</script>

<style scoped></style>
