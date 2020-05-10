<template>
    <b-tab :title="title" :class="{ active: isActive == title }" @click="onClick"></b-tab>
</template>

<script>
export default {
    name: "Tab",
    props: ["title", "isActive"],
    methods: {
        onClick() {
            console.log("I was clicked!")
            this.$emit("clicked", this.title);
        }
    }
}
</script>