<template>
  <div class="container">
    <h4>Assigned to Server</h4>
    <div class="row text-center">
      <div class="col-3">
        <p class="underline">IP/Hostname</p>
        <p
          :class="
            detectedAddresses.indexOf(hostAddress) !== -1
              ? 'text-success'
              : 'text-danger'
          "
        >
          {{ dataOrNone(hostAddress) }}
        </p>
      </div>
      <div class="col-3">
        <p class="underline">MAC Address</p>
        <p
          :class="
            detectedMacs.indexOf(hostMac) !== -1
              ? 'text-success'
              : 'text-danger'
          "
        >
          {{ dataOrNone(hostMac) }}
        </p>
      </div>
      <div class="col-3">
        <p class="underline">ISV Port</p>
        <p>{{ isvPort != "" ? isvPort : "Dynamic" }}</p>
      </div>
      <div class="col-3">
        <p class="underline">Primary Comm. Port</p>
        <p>{{ dataOrNone(hostPort) }}</p>
      </div>
    </div>
    <h4>Detected Server</h4>
    <div class="row text-center">
      <div class="col-3">
        <p class="underline">IP/Hostname</p>
        <p
          v-for="address in detectedAddresses"
          :key="address"
          :class="{ 'text-success': address === hostAddress }"
        >
          {{ address }}
        </p>
      </div>
      <div class="col-3">
        <p class="underline">MAC Address</p>
        <p
          v-for="mac in detectedMacs"
          :key="mac"
          :class="{ 'text-success': mac === hostMac }"
        >
          {{ mac }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "ServerDetails",
  props: [
    "detectedAddresses",
    "detectedMacs",
    "hostAddress",
    "hostMac",
    "isvPort",
    "hostPort"
  ],
  methods: {
    dataOrNone(str) {
      return str !== "" ? str : "None";
    }
  }
};
</script>

<style scoped>
p {
  padding: 0;
  margin: 0;
}
.underline {
  text-decoration: underline;
}
</style>
