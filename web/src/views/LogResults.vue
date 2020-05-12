<template>
    <div class="container">
        <div v-if="!!log">
            <h1>Results</h1>
            <LogHeader :date="log.date" :rlmVersion="log.rlmVersion" :hostname="log.hostname" />
            <SectionTabs :sections="sections"
                :activeSection="activeSection" 
                @clicked="tabClicked"/>

            <Licenses v-if="activeSection == 'Licenses'" 
                :licenses="log.licenses" 
                :detectedAddresses="log.hostIpList"
                :detectedMacs="log.hostMacList"/>

            <LicensePools v-if="activeSection == 'License Pools'"
                :isvStatistics="log.isvStatistics" />

            <Statistics v-if="activeSection == 'Statistics'"
                :statistics="log.isvStatistics.concat(log.rlmStatistics)" />

            <Logs v-if="activeSection == 'Logs'"
                :debugLogs="debugLogs" />

            <RlmInstances v-if="activeSection == 'RLM Instances'"
                :rlmInstances="log.rlmInstances" />

            <ScrollToTop />
        </div>
        <div v-else>
            <h1>Loading...</h1>
        </div>
    </div>    
</template>

<script>
import LogHeader from "../components/LogResults/LogHeader.vue";
import SectionTabs from "../components/LogResults/SectionTabs.vue";
import ScrollToTop from "../components/ScrollToTop.vue";

import Licenses from "../components/LogResults/Licenses.vue";
import LicensePools from "../components/LogResults/LicensePools.vue";
import Statistics from "../components/LogResults/Statistics.vue";
import Logs from "../components/LogResults/Logs.vue";
import RlmInstances from "../components/LogResults/RlmInstances.vue";

export default {
    name: 'LogResults',
    components: {
        LogHeader,
        SectionTabs,
        Licenses,
        LicensePools,
        Statistics,
        Logs,
        ScrollToTop,
        RlmInstances
    },
    data() {
        return {
            sections: [
                "Results",
                "Licenses",
                "License Pools",
                "Statistics",
                "Logs",
                "RLM Instances"
            ],
            activeSection: "",
            debugLogs: []
        }
    },
    // Before creating the component, check if there is an ID parameter
    // in the route path; If there is, ask the Vuex store to retrieve
    // and save the log data before continuing. If there isn't an ID
    // parameter, check if there is a log currently saved in the Vuex
    // store; If there is, continue rendering the view - If not, return
    // the user to the main Home page.
    async beforeCreate() {
        if (this.$route.params.id) {
            await this.$store.getLogById(this.$route.params.id);
        }

        if (!this.$store.getters.log) {
            this.$router.push({ name: "Home" });
        }
    },
    beforeMount() {
        // After creating the component, but before mounting it, set
        // the active section to the first section in the list (Results).
        this.activeSection = this.sections[0];
    },
    // After the component has been created, save a reference to the log
    // data in this component
    created() {
        this.log = this.$store.getters.log;
    },
    mounted() {
        this.concatLogs();
    },
    methods: {
        // When a tab is clicked, set it as the active section
        tabClicked(sectionTitle) {
            this.activeSection = sectionTitle;
        },
        concatLogs() {
            this.debugLogs = [
                ...this.log.isvLogs,
                this.log.rlmLog
            ];
        }
    }
}
</script>

<style scoped>

</style>