import React, {SyntheticEvent, useState} from "react";
import {OtNode} from "../../../app/models/otnode";
import {Button, Item, Segment} from "semantic-ui-react";
import NodeToggle from "./NodeToggle";
import {NodeRequest} from "../../../app/models/node-request";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

function NodeList() {
    const {nodeStore} = useStore();
    const {nodes, viewNode, submitting, handleDeleteNode} = nodeStore;
    const [target, setTarget] = useState('');
    
    function handleDelete(e: SyntheticEvent<HTMLButtonElement>, request: NodeRequest){
        setTarget(e.currentTarget.name);
        handleDeleteNode(request);
    }
    
    // TODO: cleanup componentson this
    return (
        <Segment>
            <Item.Group divided>
                {nodes.map(node => (
                    
                    <Item key={'listitem'+node.id}>
                        <Item.Content>
                            <Item.Header as='a' style={{display: 'flex', justifyContent: 'space-between'}}>
                                <div>{node.title}</div>
                                <NodeToggle node={node}/>
                            </Item.Header>
                            {getItemDescription(node)}
                            <Item.Extra>
                                <Button floated='right' content='More' color='blue' onClick={()=>viewNode(node.id)}/>
                                <Button name = {node.id} floated='right' content='Delete' color='red' onClick={(e)=>{
                                    var request : NodeRequest = {
                                        nodeId : node.id
                                    }
                                    handleDelete(e,request);
                                }} disabled={submitting && target===node.id} loading={submitting && target===node.id}/>
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

export default observer(NodeList)
