<template>
  <div class="container">
    <h1>Analytics</h1>
    <div class="row">
      <div class="col">
        <div class="box">
          <div class="d-flex flex-row align-items-center justify-content-end">
            <p class="py-0 px-2 m-0">Viewing Year:</p>
            <b-form-select class="w-25" v-model="selectedYear" :options="availableYears" />
          </div>
          <BarChart
            v-if="showUploadDates"
            title="Log Saves"
            chartId="1"
            :chartData="uploadChartData"
            :options="options"
          />
        </div>
      </div>
    </div>
    <div class="row my-3">
      <div class="col">
        <div class="box">
          <BarChart
            v-if="showErrorChart"
            title="Analysis Frequency"
            chartId="2"
            :chartData="errorChartData"
            :options="options"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { getLogAnalytics, getLogAnalysisTypes } from "@/services/logService.js";
import BarChart from "@/components/Charts/BarChart.vue";

export default {
  name: "AdminAnalytics",
  components: {
    BarChart
  },
  computed: {
    uploadChartData() {
      return this.allUploadChartData.length > 0
        ? this.allUploadChartData[
            this.availableYears.indexOf(this.selectedYear)
          ]
        : null;
    },
    showUploadDates() {
      return this.allUploadChartData.length > 0;
    },
    showErrorChart() {
      return this.errorChartData != null;
    }
  },
  data() {
    return {
      availableYears: [],
      selectedYear: 0,
      analytics: null,
      allUploadChartData: [],
      errorChartData: null,
      analysisTypes: [],
      options: {
        scales: {
          yAxes: [
            {
              ticks: {
                beginAtZero: true,
                stepSize: 1
              }
            }
          ]
        }
      }
    };
  },
  async mounted() {
    await this.fetch();
    this.processChartData();
  },
  methods: {
    async fetch() {
      this.analytics = await getLogAnalytics();
      this.analysisTypes = await getLogAnalysisTypes();
    },
    async processChartData() {
      this.allUploadChartData = this.getLogUploadDistribution();
      this.errorChartData = this.getLogErrorDistribution();
    },
    getLogErrorDistribution() {
      let axisLabels = [];

      let errorData = [];
      let warningData = [];
      let suggestionData = [];

      let colors = [
        "rgba(208, 0, 0, 0.6)",
        "rgba(255, 186, 8, 0.6)",
        "rgba(63, 136, 197, 0.6)"
      ];

      for (let i = 0; i < this.analysisTypes.length; i++) {
        let type = this.analysisTypes[i];
        let readableType = type.match(/[A-Z][a-z]+/g).join(" ");

        axisLabels.push(readableType);

        let errorList = Object.keys(this.analytics.errorFrequency).filter(
          item => item == type
        );
        let warningList = Object.keys(this.analytics.warningFrequency).filter(
          item => item == type
        );
        let suggestionList = Object.keys(
          this.analytics.suggestionFrequency
        ).filter(item => item == type);

        let errorCount = errorList != null ? errorList.length : 0;
        let warningCount = warningList != null ? warningList.length : 0;
        let suggestionCount =
          suggestionList != null ? suggestionList.length : 0;

        errorData.push(errorCount);
        warningData.push(warningCount);
        suggestionData.push(suggestionCount);
      }

      let chartData = this.makeChartData(
        axisLabels,
        ["Errors", "Warnings", "Suggestions"],
        [errorData, warningData, suggestionData],
        colors
      );

      return chartData;
    },
    getLogUploadDistribution() {
      let axisLabels = [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
      ];

      const newYear = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];

      let data = {};

      let colors = ["rgba(19, 111, 99, 0.8)"];

      for (let dateString of this.analytics.logSaveDates) {
        let month = new Date(dateString).getMonth();
        let year = new Date(dateString).getFullYear();

        if (!this.availableYears.includes(year)) {
          this.availableYears.push(year);
        }

        if (!(year in data)) {
          data[year] = newYear;
        }

        data[year][month] += 1;
      }

      this.availableYears.sort((a, b) => a < b);
      this.selectedYear = this.availableYears[this.availableYears.length - 1];

      let chartData = [];

      for (let year of Object.keys(data)) {
        let yearData = this.makeChartData(
          axisLabels,
          ["Number of Saves"],
          [data[year]],
          colors
        );

        chartData.push(yearData);
      }

      return chartData;
    },
    makeChartData(axisLabels, datasetLabels, datasets, colors) {
      let data = {
        labels: axisLabels,
        datasets: []
      };

      for (let i = 0; i < datasetLabels.length; i++) {
        let dataset = {
          label: datasetLabels[i],
          data: datasets[i],
          backgroundColor:
            colors != null && colors.length > i
              ? colors[i]
              : "rgba(0, 0, 0, 0.6)"
        };

        data.datasets.push(dataset);
      }

      return data;
    }
  }
};
</script>

<style scoped>
.box {
  margin: 10px 25px;
  padding: 30px 10px;
  border: 1px solid rgba(0, 0, 0, 0.5);
  border-radius: 5px;
}
</style>