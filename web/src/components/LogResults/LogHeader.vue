<template>
    <div class="row my-3">
        <div class="col">
            <h2>Log date</h2>
            <p>{{ new Date(this.date).toLocaleString() }}</p>
        </div>
        <div class="col">
            <h2>RLM Version</h2>
            <p>{{ this.rlmVersion }}</p>
        </div>
        <div class="col">
            <h2>Hostname</h2>
            <p>{{ this.hostname }}</p>
        </div>
    </div>
</template>

<script>
export default {
    name: 'LogHeader',
    props: ['date', 'rlmVersion', 'hostname']
}
</script>

<style scoped>
p {
    margin: 0;
    padding: 0;
}
</style>