<template>
<div>
    <b-dropdown right id="dropdown-form" text="Login" ref="loginDropdown">
        <b-dropdown-form @submit.prevent.stop="onLogin" v-on:keydown.enter="$event.stopPropagation()"> 
            <b-form-group label="Email" label-for="email-address-input">
                <b-form-input size="sm" v-model="email" id="email-address-input" type="email" placeholder="email@domain.com" @keydown.enter.stop />
            </b-form-group>
            <b-form-group label="Password" label-for="password-input">
                <b-form-input size="sm" v-model="password" id="password-input" type="password" placeholder="password" @keydown.enter.stop/>
            </b-form-group>
            <b-form-checkbox v-model="rememberMe" class="mb-3">Remember me</b-form-checkbox>
            <b-button class="float-right" variant="primary" type="submit">Login</b-button>
            <p v-if="isError" class="text-danger">{{ errorMessage }}</p>
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
            rememberMe: false,
            isError: false,
            errorMessage: ""
        }
    },
    methods: {
        onLogin() {
            this.isError = false;
            this.errorMessage = "";

            let loginData = { 
                email: this.email,
                password: this.password,
                rememberMe: this.rememberMe
            };

            this.$store.dispatch("login", loginData)
                .then(() => {
                    this.clear();
                    this.$refs.loginDropdown.hide();
                    this.$store.dispatch("fetchUser");
                })
                .catch(() => {
                    // Login failed
                    this.isError = true;
                    this.errorMessage = "Login attempt failed.";
                });
        },
        clear() {
            this.email = null;
            this.password = null;
            this.rememberMe = false;
            this.isError = false;
            this.errorMessage = "";
        }
    }
}
</script>

<style scoped>
.b-dropdown-form {
    width: 25vw;
}
</style>