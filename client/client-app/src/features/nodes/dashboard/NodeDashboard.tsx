import React, {useState} from "react";
import {Grid, Select} from "semantic-ui-react";
import {OtNode} from "../../../app/models/otnode";
import NodeList from "./NodeList";
import NodeDetails from "../details/NodeDetails";
import CreateNodeModal from "../create/CreateNodeModal";
import NodeBulkActions from "./NodeBulkActions";

interface Props {
    nodes: OtNode[]
}

export default function NodeDashboard({nodes}: Props) {
    const [node, setNode] = useState(null);
    const [selectedNodes, setSelectedNodes] = useState<OtNode[]>(new Array<OtNode>());

    function selectNode(node: OtNode) {
        if (selectedNodes.includes(node)) {
            console.log('unchecked');
            var nodes = selectedNodes.filter(x => x != node);
            setSelectedNodes(nodes);
        } else {
            console.log('checked');
            var nodes = selectedNodes.concat(node);
            setSelectedNodes(nodes);
        }
    }
    
    return (<Grid>
        <Grid.Column width='10'>
            <CreateNodeModal/>
            <NodeBulkActions selectedNodes={selectedNodes}/>
            <NodeList nodes={nodes} selectNode={selectNode}/>
        </Grid.Column>
        <Grid.Column width='6'>
            {nodes[0]&&<NodeDetails node={nodes[0]}/>}
        </Grid.Column>
    </Grid>)
}