import Vue from "vue";

const postFile = async function(file, uri) {
    let headers = makeHeaders(true);

    return post(file, uri, headers);
}

const post = async function(body, uri, headers = null) {
    if (headers == null) {
        headers = makeHeaders(false);
    }

    return new Promise((resolve, reject) => {
        Vue.prototype.$http.post(uri, body, {
            headers
        })
        .then((res) => {
            resolve(res);
        })
        .catch((err) => {
            reject(err);
        });
    });
}

function makeHeaders(isFile = false) {
    if (isFile) {
        return {
            "Content-Type": "multipart/form-data",
            "Authorization": "Bearer " + getToken()
        }
    } else {
        return {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + getToken()
        }
    }
}

function getToken() {
    return localStorage.getItem("red-giant-token");
}

export {
    post,
    postFile
}