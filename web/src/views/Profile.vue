<template>
    <div class="container mt-5">
        <h1>Welcome, {{ currentUser.email }}</h1>
        <p>{{ userLogs && userLogs.length }} available</p>
    </div>
</template>

<script>
export default {
    name: 'Profile',
    data() {
        return {

        }
    },
    mounted() {
        this.$store.dispatch("fetchUserLogs");
    },
    computed: {
        currentUser() {
            return this.$store.getters.user;
        },
        userLogs() {
            return this.$store.getters.userLogs;
        }
    }
}
</script>

<style scoped>

</style>