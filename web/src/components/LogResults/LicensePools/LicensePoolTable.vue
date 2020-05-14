<template>
    <table class="table table-hover container">
        <thead>
            <tr class="row">
                <th :class="productSpan()">Product</th>
                <th :class="numberSpan()">Total Seats</th>
                <th :class="numberSpan()">In Use</th>
                <th :class="holderSpan()">License Holders</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="pool in pools" :key="pool.product" class="row">
                <td :class="productSpan()">{{ name(pool.product) }}</td>
                <td :class="numberSpan()">{{ pool.totalSeats }}</td>
                <td :class="numberSpan()">{{ pool.inUse }}</td>
                <td :class="holderSpan()">
                    <b-list-group>
                        <b-list-group-item v-for="holder in pool.checkedOutTo" 
                            :key="holder"
                            class="m-0 p-1 text-center">
                            <p class="m-0 p-1">{{ holder }}</p>
                        </b-list-group-item>
                    </b-list-group>
                </td>
            </tr>
        </tbody>
    </table>
</template>

<script>
import formatSuiteName from "../../../helpers/formatSuiteName";
const COLUMN = "col-";
const PRODUCT_SPAN = 3;
const NUMBER_SPAN = 2;
const HOLDER_SPAN = 5;

export default {
    name: 'LicensePoolTable',
    props: ['pools'],
    methods: {
        productSpan() {
            return `${COLUMN}${PRODUCT_SPAN}`;
        },
        numberSpan() {
            return `${COLUMN}${NUMBER_SPAN}`;
        },
        holderSpan() {
            return `${COLUMN}${HOLDER_SPAN}`;
        },
        name(val) {
            return formatSuiteName(val);
        }
    }
}
</script>
