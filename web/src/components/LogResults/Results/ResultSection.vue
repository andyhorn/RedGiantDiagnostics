<template>
    <div class="row col p-3 my-3 border rounded">
        <div v-if="list.length == 0">
            <h2 class="text-success"><b-icon-check /> No {{ title.toLowerCase() }} found</h2>
        </div>
        <div v-else>
            <h2 :class="titleClass">{{ list.length }} {{ title }} Found</h2>
            <ul>
                <li v-for="item in list" :key="item.message">
                    {{ item.message }}
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
export default {
    name: 'ResultSection',
    props: ['title', 'list', 'variant'],
    computed: {
        titleClass() {
            return this.variant != null ? `text-${this.variant}` : 'text-default';
        }
    }
}
</script>

<style scoped>
li {
    font-size: 1.2rem;
}
</style>