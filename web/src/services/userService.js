const routes = require('../config/routes');
const webService = require("./webService");

const getUserData = async function() {
    try {
        let user = await webService.get(routes.getUserData);
        return user;
    }
    catch (err) {
        return {};
    }
}

const getUserById = async function(id) {
    try {
        let user = await webService.get(`${routes.getUserById}/${id}`);
        return user;
    }
    catch {
        return {};
    }
}

const changeUserPassword = async function(userId, currentPassword, newPassword, confirmNewPassword) {
    try {
        let uri = routes.changePassword;
        uri.replace("{id}", userId);
        await webService.post(uri, {
            currentPassword,
            newPassword,
            confirmNewPassword
        });
        return true;
    }
    catch {
        return false;
    }
}

export {
    getUserById,
    getUserData,
    changeUserPassword
}