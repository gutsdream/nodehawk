import React, {Fragment, useEffect, useState} from 'react';
import axios from "axios";
import {Container, Header, List} from "semantic-ui-react";
import {OtNode} from "../models/node";
import NavBar from "./NavBar";

function App() {
    const [nodes, setNodes] = useState<OtNode[]>([]);

    useEffect(() => {
        axios.get<OtNode[]>('http://localhost:5000/api/nodes/GetNodes').then(x => {
            setNodes(x.data);
        })
    }, []);

    return (
        <Fragment>
            <NavBar/>
            <Container>
                <List>
                    {nodes.map(node => (
                        <List.Item key={node.id}>
                            {node.title}
                        </List.Item>
                    ))}
                </List>
            </Container>
        </Fragment>
    );
}

export default App;
