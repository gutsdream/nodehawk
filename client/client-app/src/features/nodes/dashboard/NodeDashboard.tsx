import React, {useState} from "react";
import {Button, Grid} from "semantic-ui-react";
import {OtNode} from "../../../app/models/otnode";
import NodeList from "./NodeList";
import NodeDetails from "../details/NodeDetails";
import CreateNodeModal from "../create/CreateNodeModal";
import NodeBulkActions from "./NodeBulkActions";
import {NodeGeneralDetails} from "../../../app/models/update-node-details";
import {CreateNodeRequest} from "../../../app/models/create-node";
import {NodeRequest} from "../../../app/models/node-request";

interface Props {
    nodes: OtNode[]
    node: OtNode | undefined
    selectedNodesForBulk : string[]
    selectNode : (id: string) => void;
    bulkToggle : () => void;
    viewNode : (id: string) => void;
    cancelViewNode : () => void;
    handleEditNode: (details : NodeGeneralDetails) => void;
    submitting : boolean;
    handleCreateNode: (request: CreateNodeRequest) => void;
    handleDeleteNode: (request: NodeRequest) => void;
}

export default function NodeDashboard({nodes, 
                                          node, 
                                          selectedNodesForBulk, 
                                          bulkToggle, 
                                          viewNode, 
                                          cancelViewNode, 
                                          selectNode, 
                                          handleEditNode, 
                                          handleCreateNode,
                                          handleDeleteNode,
                                          submitting}: Props) {
        return (<Grid>
        <Grid.Column width='10'>
            <CreateNodeModal handleCreateNode={handleCreateNode} submitting={submitting}/>
            <NodeBulkActions selectedNodes={selectedNodesForBulk}/>
            <Button onClick={()=>bulkToggle()} content='Select All' style={{backgroundColor: 'white', float:'right'}}/>
            <NodeList nodes={nodes} selectNode={selectNode} viewNode={viewNode} selectedNodes={selectedNodesForBulk} handleDeleteNode={handleDeleteNode} submitting={submitting}/>
        </Grid.Column>
        <Grid.Column width='6'>
            {node&&<NodeDetails node={node} cancelViewNode={cancelViewNode} handleEditNode={handleEditNode} submitting={submitting}/>}
        </Grid.Column>
    </Grid>)
}