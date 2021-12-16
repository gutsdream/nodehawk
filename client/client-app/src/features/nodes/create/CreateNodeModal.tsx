import {Button, Form, Header, Modal, Segment} from "semantic-ui-react";
import React, {ChangeEvent, useState} from "react";
import {CreateNodeRequest} from "../../../app/models/create-node";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

// TODO: clean into comps
function CreateNodeModal(){
    const {nodeStore} = useStore();
    const{handleCreateNode, submitting} = nodeStore;
    const [open, setOpen] = React.useState(false);

    const initialGeneralDetails : CreateNodeRequest = {
        title: "",
        externalId: undefined,
        host: "",
        username: "",
        key: ""
    }

    const [nodeForm, setNodeForm] = useState(initialGeneralDetails);
    
    function handleSubmit() {
        handleCreateNode(nodeForm);
    }

    function handleInputChange(event: ChangeEvent<HTMLInputElement>) {
        const {name, value} = event.target;
        setNodeForm({...nodeForm, [name]: value});
    }

    return (
        <Modal
            onClose={() => setOpen(false)}
            onOpen={() => setOpen(true)}
            open={open}
            trigger={<Button content='Register Node' color='blue'/>}
            style={{display: 'flex', width:'50%'}}
        >
            <Modal.Header>Register Node</Modal.Header>
            <Segment clearing className='form-segment' style={{border: 'none', paddingBottom:'25px'}}>
                <Form onSubmit={handleSubmit} className='general-form' autoComplete='off'>
                    <Header as='h4'>General Details</Header>
                    <Form.Field required>
                        <label>Title</label>
                        <input placeholder='Title' value={nodeForm.title} name = 'title' onChange={handleInputChange}/>
                    </Form.Field>
                    <Form.Field>
                        <label>External Id <i>(same identifier as used on OT Hub)</i></label>
                        <input value={nodeForm.externalId} name = 'externalId' onChange={handleInputChange}/>
                    </Form.Field>
                    
                    <Header as='h4'>SSH Connection Details</Header>
                    <Form.Field required>
                        <label>Host</label>
                        <input placeholder='Host' value={nodeForm.host} name = 'host' onChange={handleInputChange}/>
                    </Form.Field>
                    <Form.Field required>
                        <label>Username</label>
                        <input placeholder='Username' value={nodeForm.username} name = 'username' onChange={handleInputChange}/>
                    </Form.Field>
                    <Form.Field required>
                        <label>Key</label>
                        <input placeholder='Key' type='password' value={nodeForm.key} name = 'key' onChange={handleInputChange}/>
                    </Form.Field>
                    <br/>
                    
                    <Button floated='right' positive type='submit' content='Submit' disabled={submitting} loading={submitting}/>
                    <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
                </Form>
            </Segment>
        </Modal>
    )
}

export default observer(CreateNodeModal)