const routes = require('../config/routes');
const webService = require("./webService");

const getUserData = async function() {
    try {
        console.log("Retrieving user data...")
        let user = await webService.get(routes.getUserData);
        console.log(user)
        return user;
    }
    catch (err)
    {
        console.log("Error retrieving user data")
        console.log(err)
        return {};
    }
}

const getUserById = async function(id) {
    try {
        let user = await webService.get(`${routes.getUserById}/${id}`);
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