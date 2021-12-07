import React from "react";
import {OtNode} from "../../../app/models/otnode";
import {Button, Checkbox, Item, Segment} from "semantic-ui-react";
import NodeToggle from "./NodeToggle";
import {NodeRequest} from "../../../app/models/node-request";

interface Props {
    nodes: OtNode[]
    selectedNodes: string[]
    selectNode : (id: string) => void;
    viewNode : (id: string) => void
    handleDeleteNode: (request: NodeRequest) => void;
    submitting : boolean;
}

export default function NodeList({nodes, selectNode, viewNode, selectedNodes, handleDeleteNode, submitting}: Props) {
    return (
        <Segment>
            <Item.Group divided>
                {nodes.map(node => (
                    
                    <Item key={'listitem'+node.id}>
                        <Item.Content>
                            <Item.Header as='a' style={{display: 'flex', justifyContent: 'space-between'}}>
                                <div>{node.title}</div>
                                <NodeToggle node={node} selectNode={selectNode} selectedNodes={selectedNodes}/>
                            </Item.Header>
                            {getItemDescription(node)}
                            <Item.Extra>
                                <Button floated='right' content='More' color='blue' onClick={()=>viewNode(node.id)}/>
                                <Button floated='right' content='Delete' color='red' onClick={()=>{
                                    var request : NodeRequest = {
                                        nodeId : node.id
                                    }
                                    handleDeleteNode(request);
                                }} disabled={submitting} loading={submitting}/>
                                <div>Node details last updated {node.minutesSinceLastSnapshot === 0 || node.minutesSinceLastSnapshot === 1
                                    ? 'just now' 
                                    : node.minutesSinceLastSnapshot + ' minutes ago'}</div>
                            </Item.Extra>
                        </Item.Content>
                    </Item>
                ))}
            </Item.Group>
        </Segment>
    )

    function getItemDescription(node: OtNode) {
        if (node.minutesSinceLastSnapshot == null) {
            return <>
                <div>Retrieving node details.</div>
            </>
        }
        return <>
            <div><b>Space taken on drive:</b> {node.spaceUsedPercentage}%</div>
            <div style={{color: node.containerRunning ? 'green' : 'red'}}>Container {node.containerRunning ? "Online" : " Offline"}</div>
        </>;
    }
}