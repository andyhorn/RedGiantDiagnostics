<template>
    <div class="container my-3">
        <b-form @submit.prevent="onSubmit" @keydown.enter="$event.stopPropagation()">
            <b-form-group
                label="User roles"
                label-for="user-roles-input"
                description="Select the user's active roles (must be an active user to be an Administrator)">
                <b-form-checkbox switch
                    id="user-admin-role-input"
                    v-model="isAdmin"
                    :value="true"
                    :unchecked-value="false">
                    Administrator
                </b-form-checkbox>
                <b-form-checkbox switch
                    id="user-user-role-input"
                    v-model="isUser"
                    :value="true"
                    :unchecked-value="false">
                    Active User
                </b-form-checkbox>
            </b-form-group>
            <b-button type="submit" variant="primary" :disabled="!submitEnabled">Save</b-button>
        </b-form>
    </div>
</template>

<script>
const userService = require("@/services/userService");
const toastService = require("@/services/toastService");

export default {
    name: 'UserRolesForm',
    props: ['userId'],
    data() {
        return {
            isAdmin: false,
            isUser: false,
            currentAdminSetting: false,
            currentUserSetting: false
        }
    },
    watch: {
        isUser: function() {
            if (!this.isUser)
                this.isAdmin = false;
        }
    },
    mounted() {
        this.fetchUserData();
    },
    computed: {
        submitEnabled() {
            return (this.isUser != this.currentUserSetting) || (this.isAdmin != this.currentAdminSetting);
        }
    },
    methods: {
        async fetchUserData() {
            let userData = await userService.getUserByIdAdmin(this.userId);
            this.isAdmin = userData.roles.includes("Administrator");
            this.isUser = userData.roles.includes("User");
            this.currentAdminSetting = userData.roles.includes("Administrator");
            this.currentUserSetting = userData.roles.includes("User");
        },
        async onSubmit() {
            if (!this.submitEnabled) return;

            let roles = [];
            if (this.isUser)
                roles.push("User");
            if (this.isAdmin)
                roles.push("Administrator");

            let success = await userService.setUserRoles(this.userId, roles);
            if (success) {
                toastService.successToast("Saved", "User roles have been updated!");
                this.fetchUserData();
                this.$emit("updated");
            } else {
                toastService.errorToast("Error", "There was an error saving the user roles.");
            }
        }
    }
}
</script>

<style scoped>

</style>