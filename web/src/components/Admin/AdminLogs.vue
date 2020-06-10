<template>
  <div class="container">
    <h1>Log Management</h1>
    <p>{{ logs.length }} logs currently saved to database.</p>
    <LogTable
      v-if="logs.length > 0 && users.length > 0"
      :logs="logs"
      @deleteLog="onDeleteLog"
      :users="users"
    />
    <div class="text-center" v-if="isLoading">
      <b-spinner label="Loading" />
    </div>
  </div>
</template>

<script>
import LogTable from "@/components/Profile/LogTable.vue";

export default {
  name: "AdminLogs",
  components: {
    LogTable
  },
  data() {
    return {
      logs: [],
      users: [],
      isLoading: true
    };
  },
  async mounted() {
    await this.fetchLogs();
    await this.fetchUsers();
    this.isLoading = false;
  },
  watch: {},
  methods: {
    async fetchUsers() {
      await this.$store.dispatch("fetchAllUsers");
      this.users = this.$store.state.userList;
    },
    async fetchLogs() {
      await this.$store.dispatch("fetchAllLogs");
      this.logs = this.$store.state.logList;
    },
    async onDeleteLog(id) {
      await this.$store.dispatch("deleteLogAdmin", id);
      this.logs = this.$store.state.logList;
    }
  }
};
</script>

<style scoped></style>
