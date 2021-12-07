import axios, {AxiosResponse} from "axios";
import {OtNode} from "../models/otnode";
import {NodeGeneralDetails} from "../models/update-node-details";
import {NodeSshDetails} from "../models/update-node-ssh-details";
import {NodeRequest} from "../models/node-request";
import {CreateNodeRequest} from "../models/create-node";

const sleep = (delay: number) =>{
    return new Promise((resolve) => {
        setTimeout(resolve,delay);
    })
}

axios.interceptors.response.use(async response => {
    try {
        await sleep(1000);
        return response;
    } catch (error) {
        console.log(error);
        return Promise.reject(error);
    }
})

axios.defaults.baseURL = 'http://localhost:5000/api/';

const responseBody = <T> (response: AxiosResponse<T>) => response.data;

const requests = {
    get: <T> (url: string) => axios.get<T>(url).then(responseBody),
    post: <T> (url: string, body: {}) => axios.post<T>(url, body).then(responseBody),
    put: <T> (url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
    delete: <T> (url: string) => axios.delete<T>(url).then(responseBody),
}

const Nodes ={
    list: ()=> requests.get<OtNode[]>('nodes/GetNodes'),
    details: (id: string)=> requests.get<OtNode[]>(`nodes/GetNode/${id}`),
    create: (node: CreateNodeRequest)=> requests.post<CreateNodeRequest>('nodes/Create', node),
    updateGeneral: (details: NodeGeneralDetails)=> requests.post<void>('nodes/UpdateGeneralDetails', details),
    updateSsh: (details: NodeSshDetails)=> requests.post<void>('nodes/UpdateSshDetails', details),
    delete: (request: NodeRequest)=> requests.post<void>('nodes/Delete', request),
    refresh: (request: NodeRequest)=> requests.post<void>('nodes/CreateSnapshot', request),
    clean: (request: NodeRequest)=> requests.post<void>('nodes/Clean', request)
}

const Aws ={
    clean: (request: NodeRequest)=> requests.post<NodeRequest>('aws/BackupNode', request),
}

const agent ={
    Nodes
}

export default agent;