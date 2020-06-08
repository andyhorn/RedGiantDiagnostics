<template>
  <div class="mt-3 pt-3 container">
    <h3>Licensed Products</h3>
    <b-table
      v-if="!!products.length"
      :items="products"
      :fields="fields"
      class="my-4"
    />
  </div>
</template>

<script>
import formatSuiteName from "../../../helpers/formatSuiteName";

export default {
  name: "ProductsTable",
  props: ["productList"],
  data() {
    return {
      products: [],
      fields: [
        { key: "name", label: "Name", formatter: "formatName" },
        "seats",
        "render_only_seats"
      ]
    };
  },
  beforeMount() {
    this.processProducts();
  },
  methods: {
    processProducts() {
      for (let product of this.productList) {
        let suiteName = product.productName;

        if (suiteName.includes("-ro")) {
          let index = suiteName.indexOf("-ro");
          suiteName = suiteName.substring(0, index);
        }

        if (this.products.find(el => el.name === suiteName) == null) {
          let newProduct = {
            name: suiteName,
            seats: "0",
            render_only_seats: "0"
          };

          this.products.push(newProduct);
        }

        let item = this.products.find(el => el.name === suiteName);

        if (product.isRenderOnly) {
          item.render_only_seats = product.seats;
        } else {
          item.seats = product.seats;
        }
      }
    },
    formatName(suite) {
      return formatSuiteName(suite);
    }
  }
};
</script>

<style scoped>
h3 {
  font-size: 1.5rem;
}
</style>
