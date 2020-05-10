<template>
    <b-tab :title="title" :class="{ active: isActive }" @click="onClick"></b-tab>
</template>

<script>
export default {
    name: "Tab",
    props: ["title", "isActive"],
    methods: {
        onClick() {
            this.$emit("clicked", this.title);
        }
    }
}
</script>