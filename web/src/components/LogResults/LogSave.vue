<template>
    <b-dropdown right id="log-save-dropdown-form" :text="buttonText" ref="log-save-dropdown" class="m-2" size="sm">
        <b-dropdown-form @submit.prevent="onSave">
            <b-form-group label="Title" label-for="log-title-input" @submit.stop.prevent>
                <b-form-input id="log-title-input" placeholder="Title" v-model="title" />
            </b-form-group>
            <b-form-group label="Comments" label-for="log-notes-input">
                <b-form-textarea id="log-notes-input" v-model="comments" placeholder="Comments..." rows="3" max-rows="10" />
            </b-form-group>
            <b-button type="submit" variant="primary" size="sm">{{ buttonText }}</b-button>
        </b-dropdown-form>
    </b-dropdown>
</template>

<script>
export default {
    name: 'LogSave',
    props: ["logId"],
    data() {
        return {
            title: "",
            comments: ""
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
        }
    }
}
</script>

<style scoped>
.b-dropdown-form {
    width: 25vw;
}
</style>