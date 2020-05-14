<template>
    <b-card-header class="p-2">
        <b-card-title>
        <div class="d-flex justify-content-between align-items-center">
            <h4>{{ name }}</h4>
            <b-button @click="toggleButtonTitle" 
                variant="info"
                class="m-0">
                {{ buttonTitle }}
            </b-button>
        </div>
        </b-card-title>
        <b-card-sub-title>
            <p :class="expirationClass(expiration)"
                class="m-0 p-0">
                Expiration Date: {{ expiration }}
            </p>
        </b-card-sub-title>
    </b-card-header>
</template>

<script>
export default {
    name: "CardHeader",
    props: ["name", "expiration"],
    data() {
        return {
            buttonTitle: "Open"
        }
    },
    methods: {
        toggleButtonTitle() {
            this.buttonTitle = this.buttonTitle === "Open"
                ? "Close"
                : "Open";
            
            this.$emit("toggled", this.buttonTitle, this.name);
        },
        expirationClass(value) {
            if (value == "None found") {
                return "text-danger";
            }

            let today = new Date();
            let cardDate = new Date(value);

            return today > cardDate 
                ? "text-danger"
                : "text-success";
        }
    }
}
</script>

<style scoped>

</style>