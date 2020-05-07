<template>
  <div class="container">
    <b-form @submit:prevent="onSubmit" @reset="onReset">
      <b-form-group id="file-input-group" description="Select the log file to analyze">
        <h2>Upload a file</h2>
        <b-form-file
          v-model="file"
          :state="Boolean(file)"
          placeholder="Select a file..."
          no-drop
          accept=".txt"
          :file-name-formatter="fileNameFormatter"
        />
      </b-form-group>
    </b-form>
  </div>
</template>

<script>
export default {
  name: "UploadForm",
  data() {
    return {
      file: null
    };
  },
  methods: {
    fileNameFormatter() {
      if (this.file) {
        return this.file.name;
      } else {
        return null;
      }
    },
    onSubmit() {

    },
    onReset() {
        
    }
  }
};
</script>

<style scoped>
</style>