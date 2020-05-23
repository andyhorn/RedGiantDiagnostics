<template>
    <div class="container">
        <b-form @submit.prevent="onSubmit" @reset="onReset">
            <div class="row mt-3">
                <div class="col">
                    <label for="current-password-input">Current Password</label>
                </div>
            </div>
            <div class="row">
                <div class="col-10">
                    <b-input id="current-password-input" type="password" required v-model="currentPassword" />
                </div>
            </div>
            <div class="row mt-3">
                <div class="col">
                    <label for="new-password-input">New Password</label>
                </div>
            </div>
            <div class="row">
                <div class="col-10">
                    <b-input id="new-password-input" type="password" required v-model="newPassword" />
                </div>
            </div>
            <div class="row mt-3">
                <div class="col">
                    <label for="password-confirm-input">Confirm password</label>
                </div>
            </div>
            <div class="row">
                <div class="col-10">
                    <b-input id="password-confirm-input" type="password" required v-model="confirmNewPassword" />
                </div>
                <div class="col-2">
                    <b-button type="submit" variant="primary" :disabled="submitDisabled">Save</b-button>
                </div>
            </div>
        </b-form>
    </div>
</template>

<script>
export default {
    name: 'PasswordUpdateForm',
    data() {
        return {
            currentPassword: "",
            newPassword: "",
            confirmNewPassword: ""
        }
    },
    computed: {
        submitDisabled() {
            return this.currentPassword == "" || this.newPassword == "" || !this.passwordsMatch;
        },
        passwordsMatch() {
            return this.newPassword == this.confirmNewPassword;
        }
    },
    methods: {
        onSubmit() {
            if (!this.submitDisabed) {
                let data = {
                    currentPassword: this.currentPassword,
                    newPassword: this.newPassword,
                    confirmNewPassword: this.confirmNewPassword
                };

                this.$emit("submit", data);
            }
        },
        onReset() {
            this.currentPassword = "";
            this.newPassword = "";
            this.confirmNewPassword = "";
        }
    }
}
</script>

<style scoped>

</style>