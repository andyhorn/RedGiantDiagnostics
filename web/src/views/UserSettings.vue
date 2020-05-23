<template>
    <div class="container mt-5">
        <ErrorToast :title="errorToastTitle" :message="errorToastMessage" :errorList="errorToastList" :visible="errorVisible" @hidden="onErrorHide" />
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
import ErrorToast from "@/components/ErrorToast.vue";
import { changeUserPassword } from "@/services/userService.js";

export default {
    name: 'UserSettings',
    components: {
        EmailUpdateForm,
        PasswordUpdateForm,
        ErrorToast
    },
    data() {
        return {
            user: {},
            errorToastTitle: "",
            errorToastMessage: "",
            errorToastList: [],
            errorVisible: false
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
            let result = await changeUserPassword(
                passwordData.currentPassword, 
                passwordData.newPassword, 
                passwordData.confirmNewPassword);
            
            if (result.success) {
                this.$bvToast.toast("Your password has been successfully changed.", {
                    title: "Password saved!",
                    noCloseButton: true,
                    variant: 'success'
                });
                this.$refs.passwordForm.onReset();
            } else {
                let errorToastTitle = "Password Change Failed";

                let message = "There ";
                if (Object.keys(result.errors) > 1) {
                    message += `were ${result.errors.length} errors `;
                } else {
                    message += "was an error ";
                }

                message += "with your password change request:";

                this.makeErrorToast(errorToastTitle, message, result.errors);
            }
        },
        createElement(type, classes, content) {
            const node = this.$createElement(
                type, { class: [...classes] },
                content
            );
            return node;
        },
        makeErrorToast(title, message, errors) {
            const messageNode = this.createElement('p', [], message);
            const titleNode = this.createElement('strong', [], title);
            let errorListItems = [];
            for (let error of errors) {
                const listItem = this.createElement('li', [], error)
                errorListItems.push(listItem);
            }
            const errorList = this.createElement('ul', [], errorListItems);
            const bodyNode = this.createElement('div', [], [messageNode, errorList])
            this.$bvToast.toast(bodyNode, {
                title: titleNode,
                variant: 'danger'
            });
        },
        onEmailChange() {

        },
        onErrorHide() {
            console.log("toast hidden")
            this.errorToastTitle = "",
            this.errorToastMessage = "",
            this.errorToastList = [],
            this.errorVisible = false
        }
    }
}
</script>

<style scoped>
h2 {
    font-size: 1.2rem;
}
</style>