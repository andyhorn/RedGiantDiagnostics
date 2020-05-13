<template>
    <b-dropdown-form inline @submit.prevent="onLogin">
        <b-form-group 
            label="Email"
            label-for="email-address-input"
            class="m-2"
        >
            <b-input 
                id="email-address-input" 
                placeholder="email@domain.com" 
                v-model="email"
                type="email"
                size="sm"
                required
            />
        </b-form-group>

        <b-form-group
            label="Password"
            label-for="password-input"
            class="m-2"
        >
            <b-input 
                id="password-input" 
                placeholder="password" 
                v-model="password"
                type="password"
                size="sm"
                required
            />
        </b-form-group>

        <b-form-group 
            label="Remember Me"
            label-for="remember-me-checkbox"
            class="m-2"
        >
            <b-form-checkbox 
                id="remember-me-checkbox" 
                v-model="rememberMe"
                class="mx-2"
            />
        </b-form-group>

        <b-button type="submit" @click.prevent="onLogin">Login</b-button>
    </b-dropdown-form>   
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

        }
    }
}
</script>

<style scoped>

</style>