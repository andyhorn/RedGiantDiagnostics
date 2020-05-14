import Vue from "vue";

const put = async function(uri, data) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.put(uri, data)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const del = async function(uri) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.delete(uri)
            .then((res) => resolve(res))
            .catch(err => reject(err));
    });
}

const get = async function(uri) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.get(uri)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const postFile = async function (uri, file) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.post(uri, file)
            .then(res => resolve(res))
            .catch(err => reject(err));
    });
}

const post = async function(body, uri) {
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.post(uri, body)
            .then((res) => {
                resolve(res);
            })
            .catch((err) => {
                reject(err);
            });
    });
}

export {
    get,
    post,
    postFile,
    put,
    del
}