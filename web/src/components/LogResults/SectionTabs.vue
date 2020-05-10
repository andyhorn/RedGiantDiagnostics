<template>
    <b-tabs content-class="mt-3" fill>
        <Tab title="Results" :isActive="activeSection" @clicked="onClick" />
        <Tab title="Licenses" :isActive="activeSection" @clicked="onClick" />
        <Tab title="License Pools" :isActive="activeSection" @clicked="onClick" />
        <Tab title="Statistics" :isActive="activeSection" @clicked="onClick" />
        <Tab title="Logs" :isActive="activeSection" @clicked="onClick" />
        <Tab title="RLM Instances" :isActive="activeSection" @clicked="onClick" />
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
        },
        isActive(title) {
            return title == this.activeSection;
        }
    }
}
</script>