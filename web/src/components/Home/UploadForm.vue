<template>
  <div class="container">
    <b-form @submit.prevent="onSubmit">
      <b-form-group
        id="file-input-group"
        description="Select the log file to analyze"
      >
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
      <b-button type="submit" variant="success" id="submit-button">
        <b-icon icon="lightning-fill" /> Go!
      </b-button>
      <b-tooltip
        target="submit-button"
        ref="tooltip"
        :title="randomMessage()"
      ></b-tooltip>
    </b-form>
  </div>
</template>

<script>
import { PostFile } from "../../config/routes";
import { postFile } from "../../services/webService";

export default {
  name: "UploadForm",
  data() {
    return {
      file: null,
      messages: [
        "Magic!",
        "Show me what you got!",
        "Analyze this!",
        "Brilliant!",
        "Send it to space!",
        "THE RIG!"
      ]
    };
  },
  mounted() {
    this.$root.$on("bv::tooltip::show", event => {
      event.vueTarget.title = this.randomMessage();
    });
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
      formData.append("file", this.file);
      postFile(PostFile, formData)
        .then(value => {
          this.$store.commit("log_retrieved", value.data);
          this.$emit("parsed");
        })
        .catch(() => {});
    },
    randomMessage() {
      let index = Math.floor(Math.random() * this.messages.length);
      return this.messages[index];
    }
  }
};
</script>

<style scoped></style>
