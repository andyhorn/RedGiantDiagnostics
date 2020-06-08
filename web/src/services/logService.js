import { get, postFile, put, del } from "./webService";
import {
    getLogById,
    postLog,
    putLog,
    deleteLog as deleteLogUri,
    currentUserLogs,
    getAllLogsUri,
    updateLogAdminUri,
    deleteLogAdminUri
} from "../config/routes";

const updateLogAdmin = async function (log) {
    let uri = updateLogAdminUri;
    uri = uri.replace("{id}", log.id);

    try {
        await put(uri, log);
        return true;
    } catch {
        return false
    }
}

const deleteLogAdmin = async function (id) {
    let uri = deleteLogAdminUri;
    uri = uri.replace("{id}", id);

    await del(uri);
}

const getById = async function (id) {
    let uri = `${getLogById}/${id}`;
    try {
        let log = await get(uri);
        return log.data;
    }
    catch {
        return null;
    }
}

const saveLog = async function (log) {
    let uri = `${postLog}`;
    try {
        let response = await postFile(uri, log);
        return response.data;
    }
    catch {
        return {};
    }
}

const updateLog = async function (log) {
    let uri = `${putLog}/${log.id}`;
    try {
        await put(uri, log);
        return log
    }
    catch {
        return "";
    }
}

const deleteLog = async function (id) {
    let uri = `${deleteLogUri}/${id}`;
    try {
        await del(uri);
        return true;
    }
    catch {
        return false;
    }
}

const getLogsForCurrentUser = async function () {
    let uri = currentUserLogs;
    try {
        let logs = await get(uri);
        return logs.data;
    } catch {
        return null;
    }
}

const getAllLogs = async function () {
    let uri = getAllLogsUri;
    try {
        let logs = await get(uri);
        return logs.data;
    } catch (err) {
        return null;
    }
}

export {
    getById,
    saveLog,
    updateLog,
    deleteLog,
    getLogsForCurrentUser,
    getAllLogs,
    updateLogAdmin,
    deleteLogAdmin
}