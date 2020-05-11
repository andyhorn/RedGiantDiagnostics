<template>
    <div class="container">
        <License v-for="(license, index) in licenses" 
            :key="license.name" 
            :id="index.toString()" 
            :data="license"
            :detectedAddresses="detectedAddresses"
            :detectedMacs="detectedMacs"/>
    </div>
</template>

<script>
import License from "./Licenses/License.vue";
import getFileName from "../../helpers/getFileName";

export default {
    name: "Licenses",
    props: ["licenses", "detectedAddresses", "detectedMacs"],
    components: {
        License
    }
}
</script>

<style scoped>

</style>