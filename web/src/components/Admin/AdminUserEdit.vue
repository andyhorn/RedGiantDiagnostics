<template>
    <div>
        <EmailUpdateForm />
        <PasswordUpdateForm />
    </div>
</template>

<script>
import EmailUpdateForm from "@/components/UserSettings/EmailUpdateForm.vue";
import PasswordUpdateForm from "@/components/UserSettings/PasswordUpdateForm.vue";

export default {
    name: 'AdminUserEdit',
    props: ['user'],
    components: {
        EmailUpdateForm,
        PasswordUpdateForm
    }
}
</script>

<style scoped>

</style>