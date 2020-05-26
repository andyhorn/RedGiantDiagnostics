<template>
    <b-collapse :visible="isOpen" :id="id">
        <b-card>
            <ServerDetails 
                :detectedAddresses="detectedAddresses"
                :detectedMacs="detectedMacs"
                :hostAddress="data.hostAddress"
                :hostMac="data.hostMac"
                :isvPort="data.isvPort"
                :hostPort="data.hostPort"
            />
            <ProductsTable :productList="data.productLicenses" />
        </b-card>
    </b-collapse>
</template>

<script>
import ServerDetails from "./ServerDetails.vue";
import ProductsTable from "./ProductsTable.vue";

export default {
    name: "CardBody",
    components: {
        ServerDetails,
        ProductsTable
    },
    props: ["isOpen", "data", "id", "detectedAddresses", "detectedMacs"]
}
</script>

<style scoped>
.card-body {
    border-top: none;
}
</style>