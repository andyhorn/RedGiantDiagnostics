<template>
    <div class="container">
        <h1 class="title">User Management</h1>
        <div class="d-flex justify-content-between">
            <p class="subtitle">Select a user from the list below to edit their information.</p>
            <router-link :to="{ name: 'RegisterNewUser' }">Create New User</router-link>
        </div>
        <b-table striped
            :items="users"
            selectable
            select-mode="single"
            @row-selected="onRowSelected"/>
        <div v-if="selectedUser.length > 0">
            <div v-if="selectedUser[0].userId == $store.state.user.userId">
                <p>Please visit your <router-link :to="{ name: 'UserSettings' }">Account Settings</router-link>
                    to make changes to your own account.</p>
            </div>
            <div v-else>
                <AdminUserEdit :user="selectedUser[0]" />
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'AdminUsers',
    data() {
        return {
            users: [],
            selectedUser: []
        }
    },
    mounted() {
        this.fetchUsers();
    },
    methods: {
        async fetchUsers() {
            await this.$store.dispatch("fetchAllUsers");
            this.users = this.$store.state.userList;

            return 0;
        },
        onRowSelected(item) {
            this.selectedUser = item;
        }
    }
}
</script>

<style scoped>

</style>