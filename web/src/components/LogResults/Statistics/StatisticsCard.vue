<template>
    <b-card>
        <b-card-body>
            <p>Server: {{ server.serverName }}</p>
            <b-card-text>
                <StatisticsTable :server="server" />
            </b-card-text>
        </b-card-body>
    </b-card>
</template>

<script>
import StatisticsTable from "./StatisticsTable";

export default {
    name: 'StatisticsCard',
    props: ['server'],
    components: {
        StatisticsTable
    }
}
</script>

<style scoped>
.name {
    font-family: 'Courier New', Courier, monospace !important;
    font-weight: 500;
}
</style>