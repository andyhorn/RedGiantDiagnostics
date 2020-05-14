import { get, postFile, put, del } from "./webService";
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
    console.log("Saving log at uri " + uri)
    try {
        let response = await postFile(uri, log);
        console.log("Save response:")
        console.log(response)
        return response.data;
    }
    catch (err) {
        console.log("Error saving log")
        console.log(err)
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