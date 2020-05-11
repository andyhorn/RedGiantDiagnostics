<template>
    <b-table v-if="!!products.length" :items="products" :fields="fields" class="my-4"/>
</template>

<script>
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
        }
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
            if (suite == "" || suite == null || suite == undefined) return;
            let name = suite.substring(0, suite.indexOf("suite"));
            name = `${name[0].toUpperCase()}${name.substring(1)} Suite`;
            return name;
        }
    }
}
</script>

<style scoped>

</style>