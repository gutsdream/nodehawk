import {makeAutoObservable} from "mobx";
import {OtNode} from "../models/otnode";
import agent from "../api/agent";
import {NodeGeneralDetails} from "../models/update-node-details";
import {CreateNodeRequest} from "../models/create-node";
import {NodeRequest} from "../models/node-request";

export default class NodeStore {
    nodes: OtNode[] = [];
    selectedNodesForBulk: string[] = [];
    nodeUnderView: OtNode | null = null;
    loading = false;
    loadingInitial = false;
    submitting = false;

    constructor() {
        makeAutoObservable(this)
    }

    loadNodes = async () => {
        this.setLoadingInitial(true);

        try {
            this.nodes = await agent.Nodes.list();
            this.setLoadingInitial(false);
        } catch (error) {
            console.log(error);
            this.setLoadingInitial(false);
        }
    }

    setLoadingInitial = (state: boolean) => {
        this.loadingInitial = state;
    }

    viewNode = (id: string) => {
        this.nodeUnderView = this.nodes.find(x => x.id === id) ?? null;
    }

    cancelViewNode = () => {
        this.nodeUnderView = null;
    }

    selectNode = (id: string) => {
        if (this.selectedNodesForBulk.includes(id)) {
            // Deselect node
            this.selectedNodesForBulk = this.selectedNodesForBulk.filter(x => x !== id);
        } else {
            // Select node
            this.selectedNodesForBulk = this.selectedNodesForBulk.slice().concat(id);
        }
    }

    bulkToggle = () => {
        // If any nodes are selected deselect all, otherwise select all
        if (this.selectedNodesForBulk.length > 0) {
            this.selectedNodesForBulk = [];
        } else {
            this.selectedNodesForBulk = this.nodes.map(x => x.id);
        }
    }

    handleEditNode = (nodeGeneralDetails: NodeGeneralDetails) => {
        this.submitting = true;
        agent.Nodes.updateGeneral(nodeGeneralDetails).then(() => {
            let node = this.nodes.find(x => x.id === nodeGeneralDetails.nodeId);
            if (node !== undefined) {
                node.title = nodeGeneralDetails.title;
                node.externalId = nodeGeneralDetails.externalId;
                this.nodes = ([...this.nodes.filter(x => x.id !== nodeGeneralDetails.nodeId), node])
            }

            this.submitting = false;
        }).catch(() => {
            this.submitting = false;
        });
    }

    handleCreateNode = (createNodeRequest: CreateNodeRequest) => {
        this.submitting = true;
        agent.Nodes.create(createNodeRequest).then(() => {
            this.loadNodes().then(() => {
                this.submitting = false;
            })
        }).catch(() => {
            this.submitting = false;
        });
    }

    handleDeleteNode = (nodeRequest: NodeRequest) => {
        this.submitting = true;
        agent.Nodes.delete(nodeRequest).then(() => {
            this.nodes = (this.nodes.filter(x => x.id !== nodeRequest.nodeId))
            this.submitting = false;
        }).catch(() => {
            this.submitting = false;
        });
    }
}