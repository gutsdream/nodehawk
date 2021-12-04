import React from "react";
import {Button, Card, Icon} from "semantic-ui-react";
import {OtNode} from "../../../app/models/otnode";
import EditNodeModal from "../edit/EditNodeModal";

interface Props {
    node: OtNode
}

export default function NodeDetails({node}: Props) {
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
                        <Button basic content='Remove' color='red'/>
                        <Button as={EditNodeModal} node={node}/>
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