<template>
    <div class="container mt-5">
        <h1>Account Settings</h1>
        <div class="my-4 p-4 border rounded">
            <h2>Update Email Address</h2>
            <EmailUpdateForm v-if="user" :currentEmail="user.email" @submit="onEmailChange" />
        </div>
        <div class="my-4 p-4 border rounded">
            <h2>Update Password</h2>
            <PasswordUpdateForm v-if="user" @submit="onPasswordChange" ref="passwordForm" />
        </div>
    </div>    
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";
import { changeUserPassword, changeUserEmail } from "@/services/userService.js";
import { makeToast } from "@/services/toastService.js";

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
                // this.showPasswordErrorToast("Error", "Unable to load user data", []);
                makeToast("Error", "Unable to load user data", []);
            } else {
                // Otherwise, store the user data
                this.user = this.$store.state.user;
            }
        },
        async onPasswordChange(passwordData) {
            let result = await changeUserPassword(
                passwordData.currentPassword, 
                passwordData.newPassword, 
                passwordData.confirmNewPassword);
            
            if (result.success) {
                this.$bvToast.toast("Your password has been successfully changed.", {
                    title: "Password saved!",
                    noCloseButton: true,
                    variant: "success"
                });
                this.passwordForm.onReset();
            } else {
                this.showPasswordErrorToast(result.errors);
            }
        },
        showPasswordErrorToast(errors) {
            let title = "Password Change Failed";

            let message = "There ";
            if (errors.length > 1) {
                message += `were ${errors.length} errors `;
            } else {
                message += "was an error ";
            }

            message += "with your password change request:";
            makeToast(title, message, errors, { variant: "danger" });
        },
        showEmailErrorToast(errors) {
            let title = "Email Change Failed";

            let message = "There ";
            if (errors.length > 1) {
                message += `were ${errors.length} errors `;
            } else {
                message += "was an error ";
            }
            message += "with your email change request";

            makeToast(title, message, errors, { variant: "danger" });
        },
        async onEmailChange(email) {
            let result = await changeUserEmail(this.user.userId, email);

            if (result.success) {
                this.$bvToast.toast("Your email has been successfully changed.", {
                    title: "Email updated!",
                    noCloseButton: true,
                    variant: "success"
                });
                this.fetchUser(true);
            } else {
                this.showEmailErrorToast(result.errors);
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