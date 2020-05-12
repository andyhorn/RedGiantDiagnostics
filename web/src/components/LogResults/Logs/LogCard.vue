<template>
    <b-card>
        <b-card-body class="m-0 p-0">
            <div class="d-flex justify-content-between align-items-center">
                <b-card-title>{{ debugLog.filename }}</b-card-title>
                <b-button v-b-toggle="id" @click="toggleButton">{{ buttonTitle }}</b-button>
            </div>
            <b-collapse :id="id" class="mt-2">
                <b-list-group>
                    <b-list-group-item v-for="(line, index) in debugLog.lines" :key="index">
                        {{ line }}
                    </b-list-group-item>
                </b-list-group>
            </b-collapse>
        </b-card-body>
    </b-card>    
</template>

<script>
export default {
    name: 'LogCard',
    props: ['debugLog', 'id'],
    data() {
        return {
            buttonTitle: 'Open'
        }
    },
    methods: {
        toggleButton() {
            this.buttonTitle = this.buttonTitle === 'Open' ? 'Close' : 'Open';
        }
    }
}
</script>

<style scoped>
.card-title {
    font-size: 1.1rem;
    font-weight: 400;
    font-family:'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}
</style>