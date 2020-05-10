<template>
    <b-collapse :visible="isOpen">

    </b-collapse>
</template>

<script>
export default {
    name: "CardBody",
    props: ["isOpen", "data"]
}
</script>