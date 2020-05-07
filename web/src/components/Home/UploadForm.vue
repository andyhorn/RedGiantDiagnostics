<template>
  <div class="container">
    <b-form @submit.prevent="onSubmit">
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
      <b-button type="submit" variant="success">Go!</b-button>
    </b-form>
  </div>
</template>

<script>
import { PostFile } from "../../config/routes";
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
        let formData = new FormData();
        formData.append('file', this.file);
        this.$http.post(PostFile,
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            }
        )
        .then(value => {
            console.log(value);
        })
        .catch(err => {
            console.log(err);
        })
    }
  }
};
</script>

<style scoped>
</style>