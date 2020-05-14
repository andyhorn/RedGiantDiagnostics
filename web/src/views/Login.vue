<template>
<div>
    <b-dropdown right id="dropdown-form" text="Login" ref="loginDropdown">
        <b-dropdown-form @submit.prevent="onLogin">
            <b-form-group label="Email" label-for="email-address-input" @submit.stop.prevent>
                <b-form-input size="sm" v-model="email" id="email-address-input" type="email" placeholder="email@domain.com" />
            </b-form-group>
            <b-form-group label="Password" label-for="password-input">
                <b-form-input size="sm" v-model="password" id="password-input" type="password" placeholder="password" />
            </b-form-group>
            <b-form-checkbox v-model="rememberMe" class="mb-3">Remember me</b-form-checkbox>
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
            email: "",
            password: "",
            rememberMe: false
        }
    },
    methods: {
        onLogin() {
            this.$refs.loginDropdown.hide();
            let loginData = { 
                email: this.email,
                password: this.password,
                rememberMe: this.rememberMe
            };
            this.clear();
            this.$store.dispatch("login", loginData);
        },
        clear() {
            this.email = null;
            this.password = null;
            this.rememberMe = false;
        }
    }
}
</script>

<style scoped>
.b-dropdown-form {
    width: 25vw;
}
</style>