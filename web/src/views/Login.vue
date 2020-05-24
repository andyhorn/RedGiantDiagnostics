<template>
<div>
    <b-dropdown right id="dropdown-form" ref="loginDropdown" no-caret>
        <template v-slot:button-content>
            <b-icon icon="arrow-right" aria-hidden="true"/> Login
        </template>
        <b-dropdown-form @submit.prevent.stop="onLogin" v-on:keydown.enter="$event.stopPropagation()"> 
            <b-form-group label="Email" label-for="email-address-input">
                <b-form-input size="sm" v-model="email" id="email-address-input" type="email" placeholder="email@domain.com" @keydown.enter.stop />
            </b-form-group>
            <b-form-group label="Password" label-for="password-input">
                <b-form-input size="sm" v-model="password" id="password-input" type="password" placeholder="password" @keydown.enter.stop/>
            </b-form-group>
            <b-form-checkbox v-model="rememberMe" class="mb-3">Remember me</b-form-checkbox>
            <b-button class="float-right" variant="primary" type="submit">Login <b-icon-check /></b-button>
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
            errorMessage: "",
            dropdown: null
        }
    },
    mounted() {
        this.dropdown = this.$refs.loginDropdown;
    },
    methods: {
        async onLogin() {
            this.isError = false;
            this.errorMessage = "";

            let loginData = { 
                email: this.email,
                password: this.password,
                rememberMe: this.rememberMe
            };

            console.log("Logging in")
            try {
                await this.$store.dispatch("login", loginData);
                this.clear();
                this.dropdown.hide();
                await this.$store.dispatch("fetchUser", true);
            } catch {
                this.isError = true;
                this.errorMessage = "Login attempt failed.";
            }
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