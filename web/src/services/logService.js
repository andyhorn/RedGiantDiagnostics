import { get, post, put, del } from "./webService";
import { getLogById, postLog, putLog, deleteLog as deleteLogUri } from "../config/routes";

const getById = async function(id) {
    let uri = `${getLogById}/${id}`;
    try {
        let log = await get(uri);
        return log.data;
    }
    catch {
        return null;
    }
}

const saveLog = async function(log) {
    let uri = `${postLog}`;
    try {
        let response = await post(uri, log);
        return response.location;
    }
    catch {
        return {};
    }
}

const updateLog = async function(log) {
    let uri = `${putLog}/${log.id}`;
    try {
        await put(uri, log);
        return log
    }
    catch {
        return "";
    }
}

const deleteLog = async function(id) {
    let uri = `${deleteLogUri}/${id}`;
    try {
        await del(uri);
        return true;
    }
    catch {
        return false;
    }
}

export {
    getById,
    saveLog,
    updateLog,
    deleteLog
}