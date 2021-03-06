<template>
  <div>
    <b-pagination
      v-model="currentPage"
      :total-rows="logs ? logs.length : 0"
      :per-page="perPage"
      aria-controls="log-table"
      first-number
      last-number
      size="sm"
    />
    <b-table
      :items="logs"
      fixed
      :sort-by="sortBy"
      :sort-desc="sortDesc"
      :fields="fields"
      id="log-table"
      :busy="!logs"
      :per-page="perPage"
      sort-icon-left
      responsiveness="sm"
    >
      <template v-slot:table-busy>
        <div class="text-center text-danger my-2">
          <b-spinner class="align-middle"></b-spinner>
          <strong>Loading...</strong>
        </div>
      </template>
      <template v-slot:cell(ownerId)="data">{{ getOwnerName(data.item.ownerId) }}</template>
      <template v-slot:cell(uploadDate)="data">
        {{
        data.item.uploadDate && new Date(data.item.uploadDate).toLocaleString()
        }}
      </template>
      <template v-slot:cell(options)="data">
        <b-button variant="danger" size="sm" @click="onDelete(data.item.logId)">
          Delete
          <b-icon-x />
        </b-button>
      </template>
      <template v-slot:cell(logTitle)="data">
        <router-link :to="{ name: 'Log', params: { id: data.item.logId } }">
          {{
          data.item.logTitle
          }}
        </router-link>
      </template>
    </b-table>
  </div>
</template>

<script>
export default {
  name: "LogTable",
  props: ["logs", "users"],
  data() {
    return {
      fields: [
        {
          key: "logTitle",
          label: "Title",
          sortable: true,
          thStyle: { width: "20%" }
        },
        { key: "comments", label: "Comments" },
        {
          key: "uploadDate",
          label: "Upload Date",
          sortable: true,
          tdClass: "w-20"
        },
        { key: "options", display: "Options" }
      ],
      perPage: 10,
      currentPage: 1,
      sortBy: "uploadDate",
      sortDesc: true
    };
  },
  computed: {
    displayOwners() {
      return (
        this.$store.getters.isAdmin &&
        this.$route.name == "AdminLogs" &&
        this.users.length > 0
      );
    }
  },
  methods: {
    getOwnerName(id) {
      let owner = this.users.find(user => user.userId == id);
      return owner != null ? owner.email : "email not found";
    },
    onDelete(id) {
      if (confirm("Are you sure you want to delete this log?"))
        this.$emit("deleteLog", id);
    }
  },
  mounted() {
    if (this.displayOwners) {
      this.fields.splice(1, 0, {
        key: "ownerId",
        label: "Owner",
        sortable: true
      });
    }
  }
};
</script>

<style scoped></style>
