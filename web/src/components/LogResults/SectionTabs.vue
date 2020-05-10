<template>
    <b-tabs content-class="mt-3" fill>
        <Tab title="Results" :isActive="activeSection == this.title" @click="onClick" />
        <Tab title="Licenses" :isActive="activeSection == this.title" @click="onClick" />
        <Tab title="License Pools" :isActive="activeSection == this.title" @click="onClick" />
        <Tab title="Statistics" :isActive="activeSection == this.title" @click="onClick" />
        <Tab title="Logs" :isActive="activeSection == this.title" @click="onClick" />
        <Tab title="RLM Instances" :isActive="activeSection == this.title" @click="onClick" />
    </b-tabs>
</template>

<script>
import Tab from "./SectionTabs/Tab.vue";

export default {
    name: "SectionTabs",
    components: {
        Tab
    },
    data() {
        return {
            activeSection: "Results"
        }
    },
    methods: {
        onClick(title) {
            this.activeSection = title;
            this.$emit("clicked", title);
        }
    }
}
</script>