<template>
    <div class="container">
        <h1 class="mb-3">Create a New User</h1>
        <b-form @submit.prevent="onSubmit" @reset="onReset">
            <b-form-group
                label="Email address"
                label-for="email-address-input"
                description="Enter the new user's email address">
                <b-input id="email-address-input" 
                    type="email" 
                    v-model="email" 
                    required 
                    @keydown.enter="$event.stopPropagation()" />
            </b-form-group>
            <b-form-group
                label="Password"
                label-for="password-input"
                :description="!passwordsMatch ? 'Passwords must match' : 'Enter a temporary password'">
                <b-input id="password-input" 
                    type="password" 
                    v-model="password" 
                    required 
                    :class="{ 'mismatch' : !passwordsMatch }"
                    @keydown.enter="$event.stopPropagation()" />
            </b-form-group>
            <b-form-group
                label="Confirm Password"
                label-for="confirm-password-input"
                :description="!passwordsMatch ? 'Passwords must match' : 'Confirm the temporary password'">
                <b-input id="confirm-password-input" 
                    type="password" 
                    v-model="confirmPassword" 
                    required 
                    :class="{ 'mismatch' : !passwordsMatch }"
                    @keydown.enter="$event.stopPropagation()" />
            </b-form-group>
            <b-form-group
                label="Roles"
                label-for="roles-checkbox-input">
                <b-form-checkbox
                    v-model="isAdmin"
                    value="true"
                    unchecked-value="false">
                    Is an Administrator
                </b-form-checkbox>
            </b-form-group>
            <div>
                <b-button type="submit" :disabled="isDisabled" variant="success">Submit</b-button>
                <b-button type="reset" variant="info" class="ml-3">Clear</b-button>
            </div>
        </b-form>
    </div>    
</template>

<script>
export default {
    name: 'RegisterNewUser',
    data() {
        return {
            email: "",
            password: "",
            confirmPassword: "",
            isAdmin: false
        }
    },
    methods: {
        onSubmit() {
            if (this.isDisabled) return;

        },
        onReset() {
            this.email = "";
            this.password = "";
            this.confirmPassword = "";
        }
    },
    computed: {
        isDisabled() {
            return this.email == ""
                || this.password == ""
                || this.password != this.confirmPassword;
        },
        passwordsMatch() {
            return this.password == this.confirmPassword;
        }
    }
}
</script>

<style scoped>
.mismatch {
    border: 1px solid red;
}
</style>