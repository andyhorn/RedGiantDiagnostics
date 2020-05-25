<template>
    <div>
        <b-list-group>
            <b-list-group-item
                :active="isActive('home')"
                :to="{ name: 'Admin' }"
            >
                Home
            </b-list-group-item>
            <b-list-group-item 
                :active="isActive('logs')" 
                :to="{ name: 'AdminLogs' }"
            >
                Logs
            </b-list-group-item>
            <b-list-group-item 
                :active="isActive('users')" 
                :to="{ name: 'AdminUsers' }"
            >
                Users
            </b-list-group-item>
        </b-list-group>
    </div>
</template>

<script>
export default {
    name: 'AdminNavigation',
    props: ['activeSection'],
    methods: {
        isActive(name) {
            return name == this.activeSection;
        }
    }
}
</script>

<style scoped>
.list-group-item.active {
    border: none;
    background-color: red;
    color: white;
    border-radius: 0;
}
.list-group-item {
    border-bottom: 1px solid grey;
    border-radius: 0;
}
</style>