<template>
  <div class="container mt-5">
    <UploadForm />
  </div>
</template>

<script>
// @ is an alias to /src
import UploadForm from "@/components/Home/UploadForm";

export default {
  name: "Home",
  components: {
    UploadForm
  }
};
</script>
