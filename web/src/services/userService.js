const routes = require('../config/routes');
const webService = require("./webService");

const getUserData = function() {
    try {
        let user = webService.get(routes.getUserData);
        return user;
    }
    catch
    {
        return {};
    }
}

const getUserById = function(id) {
    try {
        let user = webService.get(`${routes.getUserById}/${id}`);
        return user;
    }
    catch 
    {
        return {};
    }
}

export {
    getUserById,
    getUserData
}