<template>
    <div class="container mt-5">
        <h1>Account Settings</h1>
        <div class="my-4 p-4 border rounded">
            <h2>Update Email Address</h2>
            <EmailUpdateForm :currentEmail="user.email" @submit="onEmailChange" />
        </div>
        <div class="my-4 p-4 border rounded">
            <h2>Update Password</h2>
            <PasswordUpdateForm @submit="onPasswordChange" ref="passwordForm" />
        </div>
    </div>    
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";
import { changeUserPassword } from "@/services/userService.js";

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
        },
        async onPasswordChange(passwordData) {
            let success = await changeUserPassword(
                this.user.id, passwordData.currentPassword, 
                passwordData.newPassword, passwordData.confirmNewPassword);
            
            if (success) {
                this.$bvToast.toast("Your password has been successfully changed.", {
                    title: "Password saved!",
                    noCloseButton: true,
                    variant: 'success'
                });
                this.$refs.passwordForm.onReset();
            } else {
                this.$bvToast.toast("There was an error changing your password, double check everything and try again.", {
                    title: "Password not saved",
                    noCloseButton: true,
                    variant: 'danger'
                });
            }
        },
        onEmailChange() {

        }
    }
}
</script>

<style scoped>
h2 {
    font-size: 1.2rem;
}
</style>