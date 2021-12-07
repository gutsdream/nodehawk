import React, {Fragment, useEffect, useState} from 'react';
import {Container} from "semantic-ui-react";
import {OtNode} from "../models/otnode";
import NavBar from "./NavBar";
import NodeDashboard from "../../features/nodes/dashboard/NodeDashboard";
import agent from "../api/agent";
import LoadingComponent from "./LoadingComponent";
import {NodeGeneralDetails} from "../models/update-node-details";

function App() {
    const [nodes, setNodes] = useState<OtNode[]>([]);
    const [node, setNode] = useState<OtNode>();
    const [selectedNodesForBulk, setSelectedNodesForBulk] = useState<string[]>(new Array<string>());
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        agent.Nodes.list().then(x => {
            setNodes(x);
            setLoading(false);
        })
    }, []);

    function viewNode(id: string) {
        setNode(nodes.find(x => x.id === id));
    }

    function cancelViewNode() {
        setNode(undefined);
    }

    function selectNode(id: string) {
        if (selectedNodesForBulk.includes(id)) {
            var nodesForBulk = selectedNodesForBulk.filter(x => x != id);
            setSelectedNodesForBulk(nodesForBulk);
        } else {
            var nodesForBulk = selectedNodesForBulk.slice().concat(id);
            setSelectedNodesForBulk(nodesForBulk);
        }
    }

    function bulkToggle() {
        if (selectedNodesForBulk.length > 0) {
            setSelectedNodesForBulk(new Array<string>());
        } else {
            setSelectedNodesForBulk(nodes.map(x => x.id));
        }
    }

    function handleEditNode(nodeGeneralDetails: NodeGeneralDetails) {
        console.log(nodeGeneralDetails);
        setSubmitting(true);
        agent.Nodes.updateGeneral(nodeGeneralDetails).then(() => {
            let node = nodes.find(x => x.id === nodeGeneralDetails.nodeId);
            if (node !== undefined) {
                node.title = nodeGeneralDetails.title;
                node.externalId = nodeGeneralDetails.externalId;
                setNodes([...nodes.filter(x => x.id !== nodeGeneralDetails.nodeId), node])
            }

            setSubmitting(false);
        }).catch(()=>{
            setSubmitting(false);
        });
    }

    if (loading) return <LoadingComponent/>

    return (
        <Fragment>
            <NavBar/>
            <Container>
                <NodeDashboard nodes={nodes}
                               node={node}
                               selectedNodesForBulk={selectedNodesForBulk}
                               selectNode={selectNode}
                               bulkToggle={bulkToggle}
                               viewNode={viewNode}
                               cancelViewNode={cancelViewNode}
                               handleEditNode={handleEditNode} 
                               submitting={submitting}/>
            </Container>
        </Fragment>
    );
}

export default App;
