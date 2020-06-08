<template>
  <div class="container">
    <ResultSection title="Errors" :list="errors" variant="danger" />
    <ResultSection title="Warnings" :list="warnings" variant="warning" />
    <ResultSection title="Suggestions" :list="suggestions" variant="info" />
  </div>
</template>

<script>
import ResultSection from "./ResultSection";

export default {
  name: "AnalysisResults",
  props: ["results"],
  components: {
    ResultSection
  },
  data() {
    return {
      suggestions: [],
      warnings: [],
      errors: []
    };
  },
  methods: {
    extractSuggestions() {
      this.suggestions = this.results.filter(r => r.resultLevel == 0);
    },
    extractWarnings() {
      this.warnings = this.results.filter(r => r.resultLevel == 1);
    },
    extractErrors() {
      this.errors = this.results.filter(r => r.resultLevel == 2);
    }
  },
  mounted() {
    this.extractSuggestions();
    this.extractWarnings();
    this.extractErrors();
  }
};
</script>

<style scoped></style>
