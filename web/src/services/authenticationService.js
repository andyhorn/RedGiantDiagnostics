const routes = require('../config/routes')
const http = require('../services/webService')

const login = function(email, password) {
    return new Promise((resolve, reject) => {
        http.post({ Email: email, Password: password }, routes.login)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

export {
    login
}