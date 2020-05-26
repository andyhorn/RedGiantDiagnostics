<template>
    <div class="container">
        <b-card title="ISV Servers">
            <b-card-body>
                <b-table :items="isvServerList" :fields="fields">
                    <template v-slot:cell(isRunning)="data">
                        {{ data.item.isRunning ? "Yes" : "No" }}
                    </template>
                </b-table>
            </b-card-body>
        </b-card>
    </div>
</template>

<script>
export default {
    name: 'IsvServers',
    props: ['isvServerList'],
    data() {
        return {
            fields: [
                { key: 'name', label: 'ISV Server' },
                { key: 'isRunning', label: 'Running?' },
                { key: 'port', label: 'Assigned Port' },
                'restarts'
            ]
        }
    }
}
</script>