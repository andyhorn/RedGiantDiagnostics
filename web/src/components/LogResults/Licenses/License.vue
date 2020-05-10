<template>
    <b-card>
        <CardHeader :name="data.name" @toggled="onToggled" />
        <CardBody :isOpen="isOpen" :data="data"/>
    </b-card>
</template>

<script>
import CardHeader from "./CardHeader.vue";
import CardBody from "./CardBody.vue";

export default {
    name: "License",
    props: ["data"],
    data() {
        return {
            isOpen: false
        }
    },
    components: {
        CardHeader,
        CardBody
    },
    methods: {
        onToggled() {
            this.isOpen = !this.isOpen;
        }
    }
}
</script>

<style scoped>
.card {
    margin: 10px;
}
.card-body {
    padding: 0;
}
</style>