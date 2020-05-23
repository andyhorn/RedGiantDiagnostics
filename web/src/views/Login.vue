<template>
<div>
    <b-dropdown right id="dropdown-form" text="Login" ref="loginDropdown">
        <b-dropdown-form @submit.prevent="onLogin" @keydown="() => {}">
            <b-form-group label="Email" label-for="email-address-input">
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
        async onLogin() {
            this.$refs.loginDropdown.hide();
            let loginData = { 
                email: this.email,
                password: this.password,
                rememberMe: this.rememberMe
            };
            console.log("Submitting login request with data:")
            console.log(loginData);
            this.clear();
            await this.$store.dispatch("login", loginData);
            console.log("login complete, fetching user...")
            this.$store.dispatch("fetchUser");
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