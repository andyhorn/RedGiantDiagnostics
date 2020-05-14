<template>
  <div class="container mt-5">
    <UploadForm @parsed="onParsed"/>
  </div>
</template>

<script>
// @ is an alias to /src
import UploadForm from "@/components/Home/UploadForm";

export default {
  name: "Home",
  components: {
    UploadForm
  },
  methods: {
    onParsed() {
      this.$router.push({ name: "Log" })
    }
  }
};
</script>
