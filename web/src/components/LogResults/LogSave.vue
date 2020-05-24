<template>
    <b-dropdown right id="log-save-dropdown-form" :text="buttonText" ref="dropdown" class="m-2" size="sm">
        <b-dropdown-form @submit.prevent="onSave" @keydown.enter="$event.stopPropagation()">
            <b-form-group label="Title" label-for="log-title-input" @submit.stop.prevent>
                <b-form-input id="log-title-input" placeholder="Title" v-model="title" @keydown.enter="$event.stopPropagation()" />
            </b-form-group>
            <b-form-group label="Comments" label-for="log-notes-input">
                <b-form-textarea id="log-notes-input" v-model="comments" placeholder="Comments..." rows="3" max-rows="10"  @keydown.enter="$event.stopPropagation()"/>
            </b-form-group>
            <b-button type="submit" variant="primary" size="sm">{{ buttonText }} <b-icon-check /></b-button>
        </b-dropdown-form>
    </b-dropdown>
</template>

<script>
export default {
    name: 'LogSave',
    props: ["logId", "currentTitle", "currentComments"],
    data() {
        return {
            title: "",
            comments: "",
            dropdown: null
        }
    },
    computed: {
        buttonText() {
            return (!this.logId || this.logId === "") ? "Save" : "Update";
        }
    },
    methods: {
        onSave() {
            this.$emit("saveLog", { title: this.title, comments: this.comments });
            this.dropdown.hide();
        }
    },
    mounted() {
        this.title = this.currentTitle;
        this.comments = this.currentComments;
        this.dropdown = this.$refs.dropdown;
    }
}
</script>

<style scoped>
.b-dropdown-form {
    width: 25vw;
}
</style>