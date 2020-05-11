<template>
    <b-button :class="isShown()" @click.prevent="onClick" variant="primary">Top</b-button>    
</template>

<script>
const TRIGGER = 400;
export default {
    name: 'ScrollToTop',
    data() {
        return {
            position: 0
        }
    },
    methods: {
        onClick() {
            window.scrollTo(0, 0);
        },
        isShown() {
            return this.position > TRIGGER ? 'show' : 'hide';
        },
        handleScroll() {
            this.position = document.documentElement.scrollTop;
        }
    },
    created() {
        window.addEventListener('scroll', this.handleScroll);
    },
    destroyed() {
        window.removeEventListener('scroll', this.handleScroll);
    }
}
</script>

<style scoped>
button {
    position: fixed;
    bottom: 50px;
    right: 10px;
}
.hide {
    visibility: hidden;
}
.show {
    visibility: visible;
}
</style>