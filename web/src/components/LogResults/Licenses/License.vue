<template>
  <b-card>
    <CardHeader
      :name="data.name"
      @toggled="onToggled"
      :expiration="expiration()"
    />
    <CardBody
      :isOpen="isOpen"
      :data="data"
      :id="id"
      :detectedAddresses="detectedAddresses"
      :detectedMacs="detectedMacs"
    />
  </b-card>
</template>

<script>
import CardHeader from "./CardHeader.vue";
import CardBody from "./CardBody.vue";

export default {
  name: "License",
  props: ["data", "id", "detectedAddresses", "detectedMacs"],
  data() {
    return {
      isOpen: false
    };
  },
  components: {
    CardHeader,
    CardBody
  },
  methods: {
    onToggled() {
      this.isOpen = !this.isOpen;
    },
    expiration() {
      let product = this.data.productLicenses[0];
      if (product == null || product == undefined) {
        return "None found";
      } else {
        let date = product.expirationDate;
        let dateString = new Date(date).toLocaleDateString();
        return dateString;
      }
    }
  }
};
</script>

<style scoped>
.card {
  margin: 10px;
}
.card-body {
  padding: 0;
}
</style>
