<template>
    <div id="admin-container">
        <div id="left-nav">
            <AdminNavigation :activeSection="activeSection" />
        </div>
        <div id="admin-view">
            <router-view />
        </div>
    </div>    
</template>

<script>
import AdminNavigation from "@/components/Admin/AdminNavigation.vue";

export default {
    name: 'Admin',
    components: {
        AdminNavigation
    },
    data() {
        return {

        }
    },
    computed: {
        activeSection() {
            if (this.$route.name == 'Admin')
                return 'home';
            else if (this.$route.name == 'AdminLogs')
                return 'logs';
            else if (this.$route.name == 'AdminUsers')
                return 'users';
            else
                return '';
        }
    }
}
</script>

<style scoped>
#admin-container {
    width: 100vw;
}
#left-nav {
    width: 13rem;
    min-width: 13rem;
    max-width: 13rem;
    height: 100vh;
    position: fixed;
    border-right: 1px solid grey;
}
#admin-view {
    padding-left: 13rem;
}
</style>