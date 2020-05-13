import Vue from "vue";

const put = async function(uri, data) {
    let headers = header();
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.put(uri, data, { headers })
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const del = async function(uri) {
    let headers = headers();
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.delete(uri, { headers })
            .then((res) => resolve(res))
            .catch(err => reject(err));
    });
}

const get = async function(uri) {
    let headers = headers();
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.get(uri, { headers })
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const postFile = async function (file, uri) {
    let headers = fileHeaders();

    return post(file, uri, headers);
}

const post = async function (body, uri, requestHeaders = null) {
    if (requestHeaders == null) {
        requestHeaders = headers();
    }

    return new Promise((resolve, reject) => {
        Vue.prototype.$http.post(uri, body, {
            headers: requestHeaders
        })
            .then((res) => {
                resolve(res);
            })
            .catch((err) => {
                reject(err);
            });
    });
}

function fileHeaders() {
    return {
        "Content-Type": "multipart/form-data"
    }
}

function headers() {
    return {
        "Content-Type": "application/json"
    }
}

export {
    get,
    post,
    postFile,
    put,
    del
}