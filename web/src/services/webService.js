import Vue from "vue";

const put = async function(uri, data) {
    // let headers = headers();
    return new Promise((resolve, reject) => {
        // Vue.prototype.$http.put(uri, data, { headers })
        Vue.prototype.$http.put(uri, data)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const del = async function(uri) {
    // let headers = headers();
    return new Promise((resolve, reject) => {
        // Vue.prototype.$http.delete(uri, { headers })
        Vue.prototype.$http.delete(uri)
            .then((res) => resolve(res))
            .catch(err => reject(err));
    });
}

const get = async function(uri) {
    // let headers = headers();
    console.log("Sending GET request to " + uri)
    return new Promise((resolve, reject) => {
        // Vue.prototype.$http.get(uri, { headers })
        Vue.prototype.$http.get(uri)
            .then((res) => resolve(res))
            .catch((err) => reject(err));
    });
}

const postFile = async function (file, uri) {
    // let headers = fileHeaders();

    // return post(file, uri, headers);
    return new Promise((resolve, reject) => {
        Vue.prototype.$http.post(uri, file, {
            headers: fileHeaders()
        })
        .then(res => resolve(res))
        .catch(err => reject(err));
    });
}

// const post = async function (body, uri, requestHeaders = null) {
const post = async function(body, uri) {
    // if (requestHeaders == null) {
    //     requestHeaders = headers();
    // }

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

function fileHeaders() {
    return {
        "Content-Type": "multipart/form-data"
    }
}

// function headers() {
//     return {
//         "Content-Type": "application/json"
//     }
// }

export {
    get,
    post,
    postFile,
    put,
    del
}