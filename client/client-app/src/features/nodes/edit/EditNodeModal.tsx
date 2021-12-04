import {Button, Form, Menu, Modal, Segment} from "semantic-ui-react";
import React, {useState} from "react";
import {OtNode} from "../../../app/models/otnode";

interface Props {
    node: OtNode
}

export default function EditNodeModal({node}: Props) {
    const generalMenu = 'general';
    const sshDetailsMenu = 'ssh details';

    const [activeItem, setActiveItem] = useState(generalMenu);
    const [open, setOpen] = React.useState(false);

    function getGeneralForm() {
        return <Form active={activeItem === generalMenu}>
            <Form.Input required placeholder='Title'/>
            <Form.Input placeholder='ExternalId'/>
            <Form.Input disabled={true}/>
            <br/>
            <Button floated='right' positive type='submit' content='Submit' onClick={() => setOpen(false)}/>
            <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
        </Form>;
    }

    function getSshForm() {
        return <Form active={activeItem === sshDetailsMenu}>
            <Form.Input required placeholder='Host'/>
            <Form.Input required placeholder='Username'/>
            <Form.Input required placeholder='Key' type='password'/>
            <br/>
            <Button floated='right' positive type='submit' content='Submit' onClick={() => setOpen(false)}/>
            <Button floated='left' type='button' content='Cancel' onClick={() => setOpen(false)}/>
        </Form>;
    }

    return (
        <Modal
            onClose={() => setOpen(false)}
            onOpen={() => setOpen(true)}
            open={open}
            trigger={<Button basic content='Edit' color='green'/>}
        >
            <Modal.Header>Edit {node.title}</Modal.Header>
            <Segment style={{border: 'none'}}>
            <Menu fluid={true} attached='top' tabular widths={4} >
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
            <Segment clearing attached='bottom' style={{border: 'none', paddingBottom: '50px'}}>
                {activeItem === generalMenu ? getGeneralForm() : getSshForm()}
            </Segment>
            </Segment>
        </Modal>
    )
}