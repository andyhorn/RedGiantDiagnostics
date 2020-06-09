<template>
  <div>
    <h2 class="text-center">{{ title }}</h2>
    <canvas ref="canvas" />
  </div>
</template>

<script>
import { Bar } from "vue-chartjs";

export default {
  extends: Bar,
  name: "BarChart",
  props: ["chartData", "options", "title"],
  data() {
    return {
      type: "bar"
    };
  },
  mounted() {
    this.renderChart(this.chartData, this.options);
  },
  watch: {
    chartData: {
      deep: true,
      handler: function() {
        this.renderChart(this.chartData, this.options);
      }
    }
  }
};
</script>