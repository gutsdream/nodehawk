import React, {Fragment} from 'react';
import {Container, Menu} from "semantic-ui-react";

export default function NavBar() {
    return (
        <Container>
            <Menu inverted borderless fixed='top'>
                <Container style={{width:'90%'}} className='navbar-grid-container'>
                    <Menu.Item header>
                        <i className="terminal icon" style={{marginRight: '10px'}}/>
                        nodehawk
                    </Menu.Item>
                    <Menu.Item>
                        <i className="paper plane icon job-icon"/>
                    </Menu.Item>
                    <Menu.Item>
                        <i className="setting icon"/>
                    </Menu.Item>
                </Container>
            
            </Menu>
            <div style={{marginBottom: '120px'}}/>
        </Container>
    )
}