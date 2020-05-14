<template>
    <b-tab :title="title" :class="{ active: activeSection == title }" @click="onClick"></b-tab>
</template>

<script>
export default {
    name: "Tab",
    props: ["title", "activeSection"],
    methods: {
        onClick() {
            this.$emit("clicked", this.title);
        }
    }
}
</script>