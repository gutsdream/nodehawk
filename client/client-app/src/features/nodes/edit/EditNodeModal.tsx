import {Button, Form, FormProps, Menu, Modal, Segment} from "semantic-ui-react";
import React, {ChangeEvent, FormEvent, useState} from "react";
import {OtNode} from "../../../app/models/otnode";
import {NodeGeneralDetails} from "../../../app/models/update-node-details";
import {NodeSshDetails} from "../../../app/models/update-node-ssh-details";
import agent from "../../../app/api/agent";

interface Props {
    node: OtNode
    handleEditNode: (details : NodeGeneralDetails)=> void;
    submitting : boolean;
}

export default function EditNodeModal({node, handleEditNode, submitting}: Props) {
    const generalMenu = 'general';
    const sshDetailsMenu = 'ssh details';

    const [activeItem, setActiveItem] = useState(generalMenu);
    const [open, setOpen] = React.useState(false);

    const initialGeneralDetails : NodeGeneralDetails = {
        nodeId: node.id,
        title: node.title,
        externalId: node.externalId
    }

    const [nodeGeneralDetails, setNodeGeneralDetails] = useState(initialGeneralDetails);

    const initialSshDetails : NodeSshDetails = {
        nodeId: node.id,
        host: "",
        username: "",
        key: ""
    }

    const [nodeSshDetails, setNodeSshDetails] = useState(initialSshDetails);
    const [sshSubmitting, setSshSubmitting] = useState(false);
    function sshIsSubmitting(value: boolean){
        setSshSubmitting(value);
    }

    return (
        <Modal
            onClose={() => setOpen(false)}
            onOpen={() => setOpen(true)}
            open={open}
            trigger={<Button basic content='Edit' color='green'/>}
            style={{display: 'flex', width: '50%!'}}
        >
            <Modal.Header>Edit {node.title}</Modal.Header>
            <Segment clearing style={{border: 'none'}}>
                <Menu fluid={true} attached='top' tabular widths={4} style={{width: '50%!important'}}>
                    <Menu.Item name={generalMenu}
                               active={activeItem === generalMenu}
                               onClick={() => {
                                   setActiveItem(generalMenu);
                               }}>
                    </Menu.Item>
                    <Menu.Item name={sshDetailsMenu}
                               active={activeItem === sshDetailsMenu}
                               onClick={() => {
                                   setActiveItem(sshDetailsMenu)
                               }}>
                    </Menu.Item>
                </Menu>
                <Segment clearing className='form-segment' attached='bottom' style={{border: 'none'}}>
                    {activeItem === generalMenu ? getGeneralForm() : getSshForm()}
                </Segment>
            </Segment>
        </Modal>
    )

    function getGeneralForm() {
        function handleSubmit() {
            handleEditNode(nodeGeneralDetails);
        }

        function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
            const {name, value} = event.target;
            setNodeGeneralDetails({...nodeGeneralDetails, [name]: value});
        }

        return <Form onSubmit={handleSubmit} className='general-form' autoComplete='off'>
            <Form.Field required>
                <label>Title</label>
                <input placeholder='Title' value={nodeGeneralDetails.title} name='title' onChange={handleInputChange}/>
            </Form.Field>
            <Form.Field>
                <label>External Id <i>(same identifier as used on OT Hub)</i></label>
                <input value={nodeGeneralDetails.externalId} name='externalId' onChange={handleInputChange}/>
            </Form.Field>
            <br/>
            <Button floated='right' positive type='submit' content='Submit' disabled={submitting} loading={submitting}/>
            <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
        </Form>;
    }

    function getSshForm() {
        function handleSubmit() {
            sshIsSubmitting(true);
            agent.Nodes.updateSsh(nodeSshDetails).then(()=>{
                sshIsSubmitting(false);
            }).catch(()=>{
                sshIsSubmitting(false);
            });
        }

        function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
            const {name, value} = event.target;
            setNodeSshDetails({...nodeSshDetails, [name]: value});
        }

        return <Form onSubmit={handleSubmit} className='general-form' autoComplete='off'>
            <Form.Field required>
                <label>Host</label>
                <input placeholder='Host' value={nodeSshDetails.host} name='host' onChange={handleInputChange}/>
            </Form.Field>
            <Form.Field required>
                <label>Username</label>
                <input placeholder='Username' value={nodeSshDetails.username} name='username' onChange={handleInputChange}/>
            </Form.Field>
            <Form.Field required>
                <label>Key</label>
                <input placeholder='Key' type='password' value={nodeSshDetails.key} name='key' onChange={handleInputChange}/>
            </Form.Field>
            <br/>
            <Button floated='right' positive type='submit' content='Submit' disabled={sshSubmitting} loading={sshSubmitting}/>
            <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
        </Form>;
    }
}