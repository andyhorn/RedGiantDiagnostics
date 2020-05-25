<template>
    <div>
        <EmailUpdateForm :currentEmail="user.email" @submit="onEmailSubmit"/>
        <UserRolesForm :userId="user.userId" @updated="refresh"/>
        <PasswordUpdateForm :requiresCurrentPassword="false" @submit="onPasswordSubmit" />
    </div>
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";
import UserRolesForm from "@/components/Admin/UserRolesForm.vue";
const toastService = require("@/services/toastService");
const userService = require("@/services/userService");

export default {
    name: 'AdminUserEdit',
    props: ['user'],
    components: {
        EmailUpdateForm,
        PasswordUpdateForm,
        UserRolesForm
    },
    methods: {
        async onEmailSubmit(email) {
            let success = await userService.updateUserAdmin(this.user.userId, { email });
            if (success) {
                toastService.successToast("Saved", "User email saved successfully!");
                await this.$store.dispatch("fetchAllUsers");
            } else {
                toastService.errorToast("Error", "There was an error saving your changes.");
            }
        },
        async onPasswordSubmit(data) {
            let success = await userService.setUserPasswordAdmin(this.user.userId, {
                    newPassword: data.newPassword,
                    confirmPassword: data.confirmPassword
                });

            if (success) {
                toastService.successToast("Saved", "User password updated successfully!");
            } else {
                toastService.errorToast("Error", "There was an error updating the user's password.");
            }
        },
        refresh() {
            this.$emit("refresh");
        }
    }
}
</script>

<style scoped>

</style>