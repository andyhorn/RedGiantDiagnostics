<template>
    <div class="container">
        <LogCard v-for="(log, index) in debugLogs" :key="index" :debugLog="log" :id="index.toString()"/>
    </div>
</template>

<script>
import LogCard from "./Logs/LogCard.vue";

export default {
    name: 'Logs',
    props: ['debugLogs'],
    components: {
        LogCard
    }
}
</script>

<style scoped>

</style>