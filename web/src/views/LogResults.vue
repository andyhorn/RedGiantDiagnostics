<template>
    <div class="container">
        <div v-if="!!log">
            <h1>Results</h1>
            <LogHeader :date="log.date" :rlmVersion="log.rlmVersion" :hostname="log.hostname" />
            <SectionTabs />
        </div>
        <div v-else>
            <h1>Loading...</h1>
        </div>
    </div>    
</template>

<script>
import LogHeader from "../components/LogResults/LogHeader.vue";
import SectionTabs from "../components/LogResults/SectionTabs.vue";

export default {
    name: 'LogResults',
    components: {
        LogHeader,
        SectionTabs
    },
    async beforeMount() {
        if (this.$route.params.id) {
            await this.$store.getLogById(this.$route.params.id);
        }

        if (!this.$store.getters.log) {
            this.$router.push({ name: "Home" });
        }
    },
    created() {
        this.log = this.$store.getters.log;
    }
}
</script>

<style scoped>

</style>