import NodeStore from "./node-store";
import {createContext, useContext} from "react";

interface Store{
    nodeStore: NodeStore
}

export const store : Store ={
    nodeStore : new NodeStore()
}

export const StoreContext = createContext(store);

export function useStore(){
    return useContext(StoreContext);
}