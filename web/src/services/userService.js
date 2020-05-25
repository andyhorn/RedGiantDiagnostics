const routes = require('../config/routes');
const webService = require("./webService");

const createNewUser = async function(data) {
    try {
        let result = await webService.post(data, routes.createNewUserUri);
        return result.data;
    } catch {
        return false;
    }
}

const setUserRoles = async function(userId, roles) {
    let uri = routes.setUserRolesUri;
    uri = uri.replace("{id}", userId);

    try {
        await webService.put(uri, { roles });
        return true;
    } catch {
        return false;
    }
}

const getAllUsers = async function() {
    try {
        let users = await webService.get(routes.getAllUsersUri);
        return users.data;
    } catch (err) {
        return null;
    }
}

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

const changeUserEmail = async function(id, newEmail) {
    let uri = routes.changeEmail;
    uri = uri.replace("{id}", id);
    try {
        await webService.put(uri, { email: newEmail });
        return { success: true, errors: null };
    } catch (err) {
        console.log(err)
        let errors = processErrors(err);
        return { success: false, errors };
    }
}

const updateUserAdmin = async function(userId, data) {
    let uri = routes.updateUserAdminUri;
    uri = uri.replace("{id}", userId);

    try {
        await webService.put(uri, data);
        return true;
    } catch {
        return false;
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

const setUserPasswordAdmin = async function(id, data) {
    let uri = routes.setUserPasswordAdminUri;
    uri = uri.replace("{id}", id);

    try {
        await webService.put(uri, data);
        return true;
    } catch {
        return false;
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
    changeUserPassword,
    changeUserEmail,
    getAllUsers,
    createNewUser,
    setUserRoles,
    updateUserAdmin,
    setUserPasswordAdmin
}