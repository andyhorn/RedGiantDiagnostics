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
            <b-button type="submit" variant="primary" :disabled="!submitEnabled()">Save</b-button>
        </b-form>
    </div>
</template>

<script>
const userService = require("@/services/userService");

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
            this.submitEnabled();
        }
    },
    mounted() {
        this.fetchUserData();
    },
    methods: {
        async fetchUserData() {
            let userData = await userService.getUserByIdAdmin(this.userId);
            this.isAdmin = userData.roles.includes("Administrator");
            this.isUser = userData.roles.includes("User");
            this.currentAdminSetting = userData.roles.includes("Administrator");
            this.currentUserSetting = userData.roles.includes("User");
        },
        submitEnabled() {
            return (this.isUser != this.currentUserSetting) || (this.isAdmin != this.currentAdminSetting);
        }
    }
}
</script>

<style scoped>

</style>