<template>
    <div class="container my-5">
        <h1>Account Settings</h1>
        <div class="my-4 p-4 border rounded">
            <h2>Update Email Address</h2>
            <EmailUpdateForm v-if="user" :currentEmail="user.email" @submit="onEmailChange" />
        </div>
        <div class="my-4 p-4 border rounded">
            <h2>Update Password</h2>
            <PasswordUpdateForm v-if="user" 
                @submit="onPasswordChange" ref="passwordForm"
                :requiresCurrentPassword="true" />
        </div>
    </div>    
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";
import { changeUserPassword, changeUserEmail } from "@/services/userService.js";
const toastService = require("@/services/toastService");

export default {
    name: 'UserSettings',
    components: {
        EmailUpdateForm,
        PasswordUpdateForm
    },
    data() {
        return {
            user: {},
            title: "",
            errorToastMessage: "",
            errorToastList: [],
            errorVisible: false,
            passwordForm: null
        }
    },
    mounted() {
        this.fetchUser();
        this.passwordForm = this.$refs.passwordForm;
    },
    methods: {
        async fetchUser(force = false) {
            await this.$store.dispatch("fetchUser", force);

            // If the user data couldn't be loaded, display an error toast
            if (this.$store.state.user == null) {
                toastService.errorToast("Server Error", "Unable to load user data. Please refresh and try again.");
            } else {
                // Otherwise, store the user data
                this.user = this.$store.state.user;
            }
        },
        async onPasswordChange(passwordData) {
            let result = await changeUserPassword(
                this.user.userId,
                passwordData.currentPassword, 
                passwordData.newPassword, 
                passwordData.confirmPassword);
            
            if (result.success) {
                toastService.successToast("Password Saved", "Your password has been successfully saved.");
                this.passwordForm.onReset();
            } else {
                toastService.errorToast("Password Error", "Unable to change your password. " +
                "Make sure it meets the minimum requirements and try again.")
            }
        },
        async onEmailChange(email) {
            let result = await changeUserEmail(this.user.userId, email);

            if (result.success) {
                toastService.successToast("Email Updated", "Your email has been saved!");
                this.fetchUser(true);
            } else {
                toastService.errorToast("Email Update Failed", "Unable to change your email address. " +
                "Make sure this email is not already being used by another user and try again.");
            }
        },
    }
}
</script>

<style scoped>
h2 {
    font-size: 1.2rem;
}
</style>