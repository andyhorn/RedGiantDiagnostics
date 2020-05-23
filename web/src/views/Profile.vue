<template>
    <div class="container mt-5" v-if="currentUser != null">
        <div class="d-flex justify-content-between align-items-end">
            <h1>Welcome, {{ currentUser.email }}</h1>
            <router-link :to="{ name: 'UserSettings' }">Account Settings</router-link>
        </div>
        <p>{{ userLogs && userLogs.length }} available</p>
        <LogTable :logs="userLogs" />
    </div>
</template>

<script>
import LogTable from "@/components/Profile/LogTable";
export default {
    name: 'Profile',
    components: {
        LogTable
    },
    mounted() {
        this.$store.dispatch("fetchUserLogs");
        this.$store.dispatch("fetchUser");
    },
    computed: {
        currentUser() {
            return this.$store.state.user;
        },
        userLogs() {
            return this.$store.state.userLogs;
        }
    },
    methods: {
        async init() {
            await this.$store.dispatch("fetchUser");
            await this.$store.dispatch("fetchUserLogs");
        }
    }
}
</script>

<style scoped>

</style>