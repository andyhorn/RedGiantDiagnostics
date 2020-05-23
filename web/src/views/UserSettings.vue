<template>
    <div class="container mt-5">
        <h1>Account Settings</h1>
        <div class="my-4 p-4 border rounded">
            <h2>Update Email Address</h2>
            <EmailUpdateForm :currentEmail="user.email" />
        </div>
        <div class="my-4 p-4 border rounded">
            <h2>Update Password</h2>
            <PasswordUpdateForm />
        </div>
    </div>    
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";

export default {
    name: 'UserSettings',
    components: {
        EmailUpdateForm,
        PasswordUpdateForm
    },
    data() {
        return {
            user: {}
        }
    },
    mounted() {
        this.fetchUser();
    },
    methods: {
        async fetchUser() {
            console.log("fetching user")
            if (this.$store.getters.user == null) {
                console.log("retrieving user from api")
                await this.$store.dispatch("fetchUser");
            }
            
            console.log("setting user data")
            console.log(this.$store.getters.user);
            this.user = this.$store.getters.user;
        }
    }
}
</script>

<style scoped>
h2 {
    font-size: 1.2rem;
}
</style>