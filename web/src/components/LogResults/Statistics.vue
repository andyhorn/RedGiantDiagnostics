<template>
  <div class="container">
    <StatisticsCard
      v-for="server in statistics"
      :key="server.serverName"
      :server="server"
      class="my-3"
    />
  </div>
</template>

<script>
import StatisticsCard from "./Statistics/StatisticsCard";

export default {
  name: "Statistics",
  props: ["statistics"],
  components: {
    StatisticsCard
  }
};
</script>

<style scoped></style>
