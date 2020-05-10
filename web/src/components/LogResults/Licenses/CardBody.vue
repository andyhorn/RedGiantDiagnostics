<template>
    <b-collapse :visible="isOpen" :id="id">
        <b-card>
            <ServerDetails 
                :detectedAddresses="detectedAddresses"
                :detectedMacs="detectedMacs"
                :hostAddress="data.hostAddress"
                :hostMac="data.hostMac"
                :isvPort="data.isvPort"
                :hostPort="data.hostPort"
            />
        </b-card>
    </b-collapse>
</template>

<script>
import ServerDetails from "./ServerDetails.vue";

export default {
    name: "CardBody",
    components: {
        ServerDetails
    },
    props: ["isOpen", "data", "id", "detectedAddresses", "detectedMacs"]
}
</script>

<style scoped>

</style>