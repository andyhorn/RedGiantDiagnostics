<template>
  <div class="container p-3">
    <h2>Editing {{ user.email }}</h2>
    <div class="border rounded p-3 my-3">
      <h3>Change User Email</h3>
      <EmailUpdateForm :currentEmail="user.email" @submit="onEmailSubmit" />
    </div>
    <div class="border rounded p-3 my-3">
      <h3>Change User Roles</h3>
      <UserRolesForm :userId="user.userId" @updated="refresh" />
    </div>
    <div class="border rounded p-3 my-3">
      <h3>Password Reset</h3>
      <PasswordUpdateForm
        :requiresCurrentPassword="false"
        @submit="onPasswordSubmit"
      />
    </div>
  </div>
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";
import UserRolesForm from "@/components/Admin/UserRolesForm.vue";
const toastService = require("@/services/toastService");
const userService = require("@/services/userService");

export default {
  name: "AdminUserEdit",
  props: ["user"],
  components: {
    EmailUpdateForm,
    PasswordUpdateForm,
    UserRolesForm
  },
  methods: {
    async onEmailSubmit(email) {
      let success = await userService.updateUserAdmin(this.user.userId, {
        email
      });
      if (success) {
        toastService.successToast("Saved", "User email saved successfully!");
        this.refresh();
      } else {
        toastService.errorToast(
          "Error",
          "There was an error saving your changes."
        );
      }
    },
    async onPasswordSubmit(data) {
      let success = await userService.setUserPasswordAdmin(this.user.userId, {
        newPassword: data.newPassword,
        confirmPassword: data.confirmPassword
      });

      if (success) {
        toastService.successToast(
          "Saved",
          "User password updated successfully!"
        );
      } else {
        toastService.errorToast(
          "Error",
          "There was an error updating the user's password."
        );
      }
    },
    refresh() {
      this.$emit("refreshUser", this.user.userId);
    }
  }
};
</script>

<style scoped></style>
