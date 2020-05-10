<template>
    <b-card-header class="p-2">
        <div class="d-flex justify-content-between align-items-center">
            <h4>{{ name }}</h4>
            <b-button @click="toggleButtonTitle" 
                variant="info"
                class="m-0"
            >
                {{ buttonTitle }}
            </b-button>
        </div>
    </b-card-header>
</template>

<script>
export default {
    name: "CardHeader",
    props: ["name"],
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
        }
    }
}
</script>