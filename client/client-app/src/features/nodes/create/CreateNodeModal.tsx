import {Button, Form, Header, Menu, Modal, Segment} from "semantic-ui-react";
import React, {useState} from "react";
import {OtNode} from "../../../app/models/otnode";

export default function CreateNodeModal() {
    const [open, setOpen] = React.useState(false);

    return (
        <Modal
            onClose={() => setOpen(false)}
            onOpen={() => setOpen(true)}
            open={open}
            trigger={<Button content='Register Node' color='blue'/>}
            style={{display: 'flex', width:'50%'}}
        >
            <Modal.Header>Register Node</Modal.Header>
            <Segment clearing className='form-segment' style={{border: 'none'}}>
                <Form style={{ width:'100%'}}>
                    <Header as='h4'>General Details</Header>
                    <Form.Field required>
                        <label>Title</label>
                        <input placeholder='Title'/>
                    </Form.Field>
                    <Form.Field>
                        <label>External Id <i>(same identifier as used on OT Hub)</i></label>
                        <input/>
                    </Form.Field>
                    
                    <Header as='h4'>SSH Connection Details</Header>
                    <Form.Field required>
                        <label>Host</label>
                        <input placeholder='Host'/>
                    </Form.Field>
                    <Form.Field required>
                        <label>Username</label>
                        <input placeholder='Username'/>
                    </Form.Field>
                    <Form.Field required>
                        <label>Key</label>
                        <input placeholder='Key' type='password'/>
                    </Form.Field>
                    <br/>
                    
                    <Button floated='right' positive type='submit' content='Submit' onClick={() => setOpen(false)}/>
                    <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
                </Form>
            </Segment>
        </Modal>
    )
}