<template>
    <div class="container">
        <b-form @submit.prevent="onSubmit" @reset="reset">
            <div class="row">
                <div class="col">
                    <label for="email-input">Email address</label>
                </div>
            </div>
            <div class="row">
                <div class="col-10" :class="{ 'input-modified' : modified }">
                    <b-input id="email-input" type="email" v-model="emailAddress"
                        :class="{ 'input-modified': modified }" ref="emailInput" />
                </div>
                <div class="col-2">
                    <b-button type="submit" variant="primary" :disabled="!modified">Save</b-button>
                </div>
            </div>
        </b-form>
    </div>
</template>

<script>
export default {
    name: 'EmailUpdateForm',
    props: ['currentEmail'],
    data() {
        return {
            emailAddress: ""
        }
    },
    methods: {
        onSubmit() {

        },
        reset() {

        }
    },
    watch: {
        currentEmail: {
            immediate: true,
            handler: function() {
                this.emailAddress = this.currentEmail;
            }
        }
    },
    mounted() {
        this.$refs.emailInput = this.currentEmail;
    },
    computed: {
        modified() {
            return this.emailAddress != this.currentEmail;
        }
    }
}
</script>

<style scoped>
    input.input-modified {
        border: 2px solid orange;
    }
</style>