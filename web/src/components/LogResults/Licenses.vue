<template>
    <div class="container">
        <License v-for="license in licenses" :key="license.name" :data="license"/>
    </div>
</template>

<script>
import License from "./Licenses/License.vue";

export default {
    name: "Licenses",
    props: ["licenses"],
    components: {
        License
    }
}
</script>

<style scoped>

</style>