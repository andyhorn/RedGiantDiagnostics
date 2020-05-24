<template>
    <div class="container">
        <h1>Log Management</h1>
        <p>{{ logs.length }} logs currently saved to database.</p>
        <LogTable v-if="logs.length > 0" :logs="logs" @deleteLog="onLogDelete" />
    </div>
</template>

<script>
import LogTable from "@/components/Profile/LogTable.vue";

export default {
    name: 'AdminLogs',
    components: {
        LogTable
    },
    data() {
        return {
            logs: []
        }
    },
    mounted() {
        this.fetchLogs();
    },
    methods: {
        async fetchLogs() {
            await this.$store.dispatch("fetchAllLogs");
            this.logs = this.$store.state.logList;
        }
    }
}
</script>

<style scoped>

</style>