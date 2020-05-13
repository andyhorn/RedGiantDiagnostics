<template>
<div>
    <b-dropdown right id="dropdown-form" text="Login">
        <b-dropdown-form @submit.prevent="onLogin">
            <b-form-group label="Email" label-for="email-address-input" @submit.stop.prevent>
                <b-form-input size="sm" id="email-address-input" type="email" placeholder="email@domain.com" />
            </b-form-group>
            <b-form-group label="Password" label-for="password-input">
                <b-form-input size="sm" id="password-input" type="password" placeholder="password" />
            </b-form-group>

            <b-form-checkbox class="mb-3">Remember me</b-form-checkbox>
            <b-button class="float-right" variant="primary" type="submit">Login</b-button>
        </b-dropdown-form>
    </b-dropdown>
</div>
</template>

<script>
export default {
    name: 'Login',
    data() {
        return {
            email: null,
            password: null,
            rememberMe: false
        }
    },
    methods: {
        onLogin() {

        }
    }
}
</script>

<style scoped>
.b-dropdown-form {
    width: 25vw;
}
</style>