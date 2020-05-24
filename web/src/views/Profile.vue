<template>
    <div class="container mt-5" v-if="currentUser != null">
        <div class="d-flex flex-column">
            <h1>Your saved logs</h1>
            <router-link :to="{ name: 'UserSettings' }" class="text-subtle"><b-icon-tools /> Account Settings</router-link>
            <p class="mt-3">{{ userLogs && userLogs.length }} logs available</p>
        </div>
        <LogTable :logs="userLogs" @deleteLog="onLogDelete"/>
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
        this.init();
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
        },
        async onLogDelete(id) {
            await this.$store.dispatch("deleteLogById", id);
            await this.$store.dispatch("fetchUserLogs", true);
        }
    }
}
</script>

<style scoped>

</style>