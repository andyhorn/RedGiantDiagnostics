<template>
    <div class="container">
        <LicensePoolCard v-for="isv in isvStatistics"
            :key="isv.serverName"
            :licensePools="isv.licensePools"
            :serverName="isv.serverName"
            class="my-3" />
    </div>
</template>

<script>
import LicensePoolCard from "./LicensePools/LicensePoolCard.vue";

export default {
    name: 'LicensePools',
    props: ['isvStatistics'],
    components: {
        LicensePoolCard
    }
}
</script>

<style scoped>

</style>