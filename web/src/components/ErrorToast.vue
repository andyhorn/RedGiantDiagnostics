<template>
  <b-toast variant="danger" v-model="visible" @hidden="$emit('hidden')">
    <template v-slot:toast-title>
      <strong>{{ title }}</strong>
    </template>
    <p>{{ message }}</p>
    <ul>
      <li v-for="(error, index) in errorList" :key="index">
        {{ error[0] }}
      </li>
    </ul>
  </b-toast>
</template>

<script>
export default {
  name: "ErrorToast",
  props: ["title", "message", "errorList", "visible"]
};
</script>
