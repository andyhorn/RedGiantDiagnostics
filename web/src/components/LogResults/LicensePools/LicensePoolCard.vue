<template>
    <b-card>
        <b-card-body>
            <b-card-title>ISV Server: <span class="name">{{ serverName }}</span></b-card-title>
            <LicensePoolTable v-if="licensePools.length > 0" :pools="licensePools" />
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
        }
    }
}
</script>

<style scoped>
.card-title {
    font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
.name {
    font-family: 'Courier New', Courier, monospace;
    font-weight: 400;
}
</style>