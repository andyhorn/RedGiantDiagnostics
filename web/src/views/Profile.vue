<template>
    <div class="container">
        <h1>Welcome, {{ currentUser.email }}</h1>
    </div>
</template>

<script>
export default {
    name: 'Profile',
    data() {
        return {

        }
    },
    computed: {
        currentUser() {
            return this.$store.getters.user;
        }
    }
}
</script>

<style scoped>

</style>