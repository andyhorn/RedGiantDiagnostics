<template>
    <div class="container">
        <div v-if="log">
            <h1>Log results!</h1>
        </div>
        <div v-else>
            <h1>Loading...</h1>
        </div>
    </div>    
</template>

<script>
export default {
    name: 'LogResults',
    data() {
        
    },
    async beforeMount() {
        if (this.$route.params.id) {
            await this.$store.getLogById(this.$route.params.id);
        }

        if (!this.$store.getters.log) {
            this.$router.push({ name: "Home" });
        }
    },
    created() {
        this.log = this.$store.getters.log;
    }
}
</script>

<style scoped>

</style>