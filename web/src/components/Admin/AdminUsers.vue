<template>
    <div class="container">
        <h1 class="title">User Management</h1>
        <div class="d-flex justify-content-between">
            <p class="subtitle">Select a user from the list below to edit their information.</p>
            <router-link :to="{ name: 'RegisterNewUser' }">Create New User</router-link>
        </div>
        <b-table striped
            :items="users"
            :fields="tableFields"
            selectable
            select-mode="single"
            @row-selected="onRowSelected">
            <template v-slot:cell(options)="{ data }">
                <b-button @click="onDelete(data.userId)" variant="danger" size="sm">Delete</b-button>
            </template>
            <template v-slot:cell(roles)="data">
                {{ makeRoles(data.item.roles) }}
            </template>
        </b-table>
        <div v-if="selectedUser.length > 0">
            <div v-if="selectedUser[0].userId == $store.state.user.userId">
                <p>Please visit your <router-link :to="{ name: 'UserSettings' }">Account Settings</router-link>
                    to make changes to your own account.</p>
            </div>
            <div v-else>
                <AdminUserEdit :user="selectedUser[0]" @refresh="onRefresh" />
            </div>
        </div>
    </div>
</template>

<script>
import AdminUserEdit from "@/components/Admin/AdminUserEdit.vue";

export default {
    name: 'AdminUsers',
    components: {
        AdminUserEdit
    },
    data() {
        return {
            // users: [],
            selectedUser: [],
            tableFields: ['email', 'roles', 'options']
        }
    },
    mounted() {
        this.fetchUsers();
    },
    computed: {
        users() {
            return this.$store.state.userList;
        }
    },
    methods: {
        async fetchUsers() {
            await this.$store.dispatch("fetchAllUsers");
            // this.users = this.$store.state.userList;

            // return 0;
        },
        onRowSelected(item) {
            this.selectedUser = item;
        },
        makeRoles(list) {
            let str = "";

            for (let i = 0; i < list.length; i++) {
                str += list[i];

                if (i < list.length - 1) {
                    str += ", ";
                }
            }

            return str;
        },
        onRefresh() {
            this.fetchUsers();
        }
    }
}
</script>

<style scoped>

</style>