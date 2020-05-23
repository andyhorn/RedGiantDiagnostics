<template>
    <div class="container mt-5">
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
    },
    computed: {
        currentUser() {
            return this.$store.getters.user || {};
        },
        userLogs() {
            return this.$store.getters.userLogs || [];
        }
    }
}
</script>

<style scoped>

</style>