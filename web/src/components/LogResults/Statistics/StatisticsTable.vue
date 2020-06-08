<template>
  <table class="table container">
    <thead>
      <tr class="row bg-info text-light">
        <th class="col"></th>
        <th class="col text-center">Since Start</th>
        <th class="col text-center">Since Midnight</th>
        <th class="col text-center">Recent</th>
      </tr>
    </thead>
    <tbody>
      <tr class="row">
        <td class="col">Start Time</td>
        <td
          v-for="(time, index) in server.startTimes"
          :key="index"
          class="col text-center"
        >
          {{ new Date(time).toLocaleString() }}
        </td>
      </tr>
      <tr class="row">
        <td class="col">Messages</td>
        <td
          v-for="(count, index) in server.messages"
          :key="index"
          class="col text-center"
        >
          {{ count }}
        </td>
      </tr>
      <tr class="row">
        <td class="col">Connections</td>
        <td
          v-for="(count, index) in server.connections"
          :key="index"
          class="col text-center"
        >
          {{ count }}
        </td>
      </tr>
      <tr v-if="!!server.checkouts" class="row">
        <td class="col">Checkouts</td>
        <td
          v-for="(count, index) in server.checkouts"
          :key="index"
          class="col text-center"
        >
          {{ count }}
        </td>
      </tr>
      <tr v-if="!!server.denials" class="row">
        <td class="col">Denials</td>
        <td
          v-for="(count, index) in server.denials"
          :key="index"
          class="col text-center"
        >
          {{ count }}
        </td>
      </tr>
      <tr v-if="!!server.licenseRemovals" class="row">
        <td class="col">License Removals</td>
        <td
          v-for="(count, index) in server.licenseRemovals"
          :key="index"
          class="col text-center"
        >
          {{ count }}
        </td>
      </tr>
    </tbody>
  </table>
</template>

<script>
export default {
  name: "StatisticsTable",
  props: ["server"]
};
</script>

<style scoped>
th,
td {
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-weight: 400;
}
</style>
