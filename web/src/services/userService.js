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

const changeUserPassword = async function(currentPassword, newPassword, confirmNewPassword) {
    try {
        let uri = routes.changePassword;
        await webService.post({
            currentPassword,
            newPassword,
            confirmNewPassword
        }, uri);
        return { success: true, errors: null };
    }
    catch (err) {
        let errors = processErrors(err);
        return { success: false, errors };
    }
}

const processErrors = function(err) {
    let data = err.response.data;

    if (typeof(data) == "string") {
        return [data];
    } else if (Array.isArray(data)) {
        return data;
    } else if (typeof(data) == "object") {
        let errors = [];
        data = data.errors;
        for (let error of Object.keys(data)) {
            errors.push(data[error]);
        }

        return errors;
    } 
}

export {
    getUserById,
    getUserData,
    changeUserPassword
}