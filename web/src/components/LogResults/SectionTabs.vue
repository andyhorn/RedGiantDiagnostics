<template>
    <b-tabs content-class="mt-3" fill>
        <Tab v-for="section in sections" 
            :key="section" 
            :title="section" 
            @clicked="onClick"
            :activeSection="activeSection"/>
    </b-tabs>
</template>

<script>
import Tab from "./SectionTabs/Tab.vue";

export default {
    name: "SectionTabs",
    props: ["sections"],
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