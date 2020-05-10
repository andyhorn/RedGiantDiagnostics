import { get } from "./webService";
import { getLogById } from "../config/routes";

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

export {
    getById
}