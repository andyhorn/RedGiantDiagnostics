<template>
    <div class="container mt-5">
        <h1>Welcome, {{ currentUser.email }}</h1>
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