import Vue from "vue";

const get = async function(uri) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.get(uri)
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
    postFile
}