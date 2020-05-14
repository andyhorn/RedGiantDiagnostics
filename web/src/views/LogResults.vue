<template>
    <div class="container">
        <div v-if="!!log">
            <div class="row d-flex justify-content-between">
                <h1>Results</h1>
                <LogSave v-if="isAuthenticated" :logId="log.id" @saveLog="onSaveLog" />
            </div>
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

import LogSave from "../components/LogResults/LogSave.vue";

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
        RlmInstances,
        LogSave
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
    async beforeMount() {
        if (this.$route.params.id) {
            await this.$store.getLogById(this.$route.params.id);
        }

        this.activeSection = this.sections[0];
    },
    // After the component has been created, save a reference to the log
    // data in this component
    computed: {
        log() {
            if (this.$store.getters.hasLog) {
                return this.$store.getters.log;
            } else {
                return {}
            }
        },
        isAuthenticated() {
             return this.$store && this.$store.getters.isAuthenticated;
        }
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
        },
        onSaveLog(data) {
            this.log.title = data.title;
            this.log.comments = data.comments;
            this.$store.dispatch("save_log", this.log);
        }
    }
}
</script>

<style scoped>

</style>