<template>
  <div class="container">
    <p>
      Number of instances detected:
      <span class="text-success" v-if="rlmInstances.length === 1">
        {{ rlmInstances.length }}
      </span>
      <span v-else class="text-danger">
        {{ rlmInstances.length }}
      </span>
    </p>
    <b-table :items="rlmInstances" :fields="fields" responsive small>
      <template v-slot:cell(alternativePorts)="data">
        {{ joinList(data.value) }}
      </template>
      <template v-slot:cell(isvServers)="data">
        {{ joinList(data.value) }}
      </template>
      <template v-slot:cell(isCurrentInstance)="data">
        {{ data.value ? "X" : "" }}
      </template>
    </b-table>
  </div>
</template>

<script>
export default {
  name: "RlmInstances",
  props: ["rlmInstances"],
  data() {
    return {
      fields: [
        "version",
        "workingDirectory",
        { key: "port", label: "Primary Port" },
        { key: "alternativePorts", label: "Alternate Ports" },
        "webPort",
        "isvServers",
        "isCurrentInstance"
      ],
      class: ""
    };
  },
  mounted() {
    this.setClass();
  },
  methods: {
    setClass() {
      this.class =
        this.rlmInstances.length === 1 ? "text-success" : "text-danger";
    },
    joinList(list) {
      let str = "";

      for (let i = 0; i < list.length; i++) {
        let value = list[i];

        if (value == "" || value == " ") continue;

        str += value;

        if (i != list.length - 1) {
          str += ", ";
        }
      }

      return str;
    }
  }
};
</script>

<style scoped>
p {
  font-size: 1.5rem;
}
</style>
