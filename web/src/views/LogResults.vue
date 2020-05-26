<template>
    <div class="container">
        <div v-if="isAuthenticated" class="d-flex flex-row justify-content-end align-items-center mt-3">
            <LogSave v-if="isAuthenticated" 
                :logId="log.id" 
                :currentTitle="log.title"
                :currentComments="log.comments"
                @saveLog="onSaveLog"
                @adminSave="onAdminSave" />
        </div>
        <div v-if="!!log" class="mb-5">
            <h1>Results</h1>
            <LogHeader :date="log.date" :rlmVersion="log.rlmVersion" :hostname="log.hostname" />
            <SectionTabs :sections="sections"
                :activeSection="activeSection" 
                @clicked="tabClicked"/>

            <AnalysisResults v-if="activeSection == 'Results'" :results="log.analysisResults" />

            <Licenses v-if="activeSection == 'Licenses'" 
                :licenses="log.licenses" 
                :detectedAddresses="log.hostIpList"
                :detectedMacs="log.hostMacList"/>

            <IsvServers v-if="activeSection == 'ISV Servers'" :isvServerList="log.rlmStatistics.servers" />

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
import AnalysisResults from "../components/LogResults/Results/AnalysisResults.vue";
import Licenses from "../components/LogResults/Licenses.vue";
import IsvServers from "../components/LogResults/IsvServers.vue";
import LicensePools from "../components/LogResults/LicensePools.vue";
import Statistics from "../components/LogResults/Statistics.vue";
import Logs from "../components/LogResults/Logs.vue";
import RlmInstances from "../components/LogResults/RlmInstances.vue";

import LogSave from "../components/LogResults/LogSave.vue";

export default {
    name: 'LogResults',
    components: {
        LogHeader,
        SectionTabs,
        AnalysisResults,
        Licenses,
        IsvServers,
        LicensePools,
        Statistics,
        Logs,
        ScrollToTop,
        RlmInstances,
        LogSave
    },
    data() {
        return {
            sections: [
                "Results",
                "Licenses",
                "ISV Servers",
                "License Pools",
                "Statistics",
                "Logs",
                "RLM Instances"
            ],
            activeSection: null
        }
    },
    // Before creating the component, check if there is an ID parameter
    // in the route path; If there is, ask the Vuex store to retrieve
    // and save the log data before continuing.
    async beforeMount() {
        if (this.$route.params.id) {
            await this.$store.dispatch("getLogById", this.$route.params.id);
        }
    },
    mounted() {
        this.activeSection = this.sections[0];
    },
    // After the component has been created, save a reference to the log
    // data in this component
    computed: {
        log() {
            return this.$store.state.log;
        },
        isAuthenticated() {
             return this.$store && this.$store.state.isAuthenticated;
        },
        debugLogs() {
            if (this.log != null) {
                return [...this.log.isvLogs, this.log.rlmLog];
            } else {
                return [];
            }
        }
    },
    methods: {
        // When a tab is clicked, set it as the active section
        tabClicked(sectionTitle) {
            this.activeSection = sectionTitle;
        },
        async onSaveLog(data) {
            this.log.title = data.title;
            this.log.comments = data.comments;
            await this.$store.dispatch("saveLog", this.log);
            await this.$store.dispatch("fetchUserLogs", true);
        },
        async onAdminSave(data) {
            this.log.title = data.title;
            this.log.comments = data.comments;
            this.log.ownerId = data.assignedUser.userId;
            await this.$store.dispatch("saveLogAdmin", this.log);
            await this.$store.dispatch("fetchAllLogs");
        }
    }
}
</script>

<style scoped>

</style>