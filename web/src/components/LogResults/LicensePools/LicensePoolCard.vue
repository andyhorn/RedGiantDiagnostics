<template>
    <b-card :title="title()">
        <LicensePoolTable :pools="licensePools" />
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
            return `ISV Server: ${this.serverName}`;
        }
    }
}
</script>
