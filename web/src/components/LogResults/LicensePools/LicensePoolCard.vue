<template>
    <b-card>
        <b-card-body>
            <p class="card-title">ISV Server: {{ serverName }}</p>
            <LicensePoolTable v-if="shouldDisplay(licensePools)" :pools="licensePools" />
            <div v-else>
                <p>No licenses in use</p>
            </div>
        </b-card-body>
    </b-card>
</template>

<script>
import LicensePoolTable from "./LicensePoolTable.vue";

export default {
    name: 'LicensePoolCard',
    props: ['licensePools', 'serverName'],
    components: {
        LicensePoolTable
    },
    methods: {
        title() {
            return `ISV Server: <span class="name">${this.serverName}</span>`;
        },
        shouldDisplay(pools) {
            // If no pools are in the list, do not display
            if (pools.length === 0) return false;
            
            // If none of the pools have any licenses in use, do not display
            let display = false;
            for (let pool of pools) {
                if (pool.inUse > 0) {
                    display = true;
                    break;
                }
            }

            return display;
        }
    }
}
</script>

<style scoped>
.card-title {
    font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
</style>