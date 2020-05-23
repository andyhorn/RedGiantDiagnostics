<template>
    <div class="container">
        <b-form @submit.prevent="onSubmit" @reset="onReset">
            <div class="row">
                <div class="col">
                    <label for="password-input">Password</label>
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <b-input id="password-input" type="password" required v-model="password" />
                </div>
            </div>
            <div class="row">
                <div class="col">
                    <label for="password-confirm-input">Confirm password</label>
                </div>
            </div>
            <div class="row">
                <div class="col-10">
                    <b-input id="password-confirm-input" type="password" required v-model="passwordConfirm" />
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
            password: "",
            passwordConfirm: ""
        }
    },
    computed: {
        submitDisabled() {
            return this.password == "" || !this.passwordsMatch;
        },
        passwordsMatch() {
            return this.password == this.passwordConfirm;
        }
    },
    methods: {
        onSubmit() {

        },
        onReset() {

        }
    }
}
</script>

<style scoped>

</style>