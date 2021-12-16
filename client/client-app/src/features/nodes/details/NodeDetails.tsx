import React from "react";
import {Button, Card} from "semantic-ui-react";
import {OtNode} from "../../../app/models/otnode";
import EditNodeModal from "../edit/EditNodeModal";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

interface Props {
    node: OtNode
}

function NodeDetails({node}: Props) {
    const {nodeStore} = useStore();
    const {cancelViewNode} = nodeStore;
    
    return (
        <Card fluid>
            <Card.Content>
                <Card.Header style={{display: 'flex', justifyContent: 'space-between'}}>
                    <div>{node.title}</div>
                    <Button as='a' 
                            href={'https://othub.origin-trail.network/nodes/dataholders/' + node.externalId} 
                            target='_blank' 
                            basic
                            content='View on OT Hub' 
                            color='blue' 
                            disabled={node.externalId === null}/>
                </Card.Header>
                <br/>
                <Card.Description>
                    {getItemDescription(node)}
                </Card.Description>
                <br/>
                <Card.Content extra>
                    <i>
                        <div>Node details last updated {node.minutesSinceLastSnapshot} minutes ago</div>
                    </i>
                    <br/>
                    <Button.Group widths='2'>
                        <Button basic onClick={cancelViewNode} content='Close' color='grey'/>
                        <Button as={EditNodeModal}/>
                    </Button.Group>
                </Card.Content>
            </Card.Content>
        </Card>
    )

    function getItemDescription(node: OtNode) {
        if (node.minutesSinceLastSnapshot == null) {
            return <>
                <div>Retrieving node details.</div>
            </>
        }
        return <>
            <div><b>Space taken on drive:</b> {node.spaceUsedPercentage}%</div>
            {getContainerStatusMsg(node.containerRunning)}
        </>;
    }

    function getContainerStatusMsg(containerRunning: boolean) {
        return containerRunning
            ? <>
                <div style={{color: 'green'}}>Container Online</div>
            </>
            : <>
                <div style={{color: 'red'}}>Container Offline</div>
            </>
    }
}

export default observer(NodeDetails);