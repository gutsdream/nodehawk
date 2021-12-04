import React, {Fragment, useEffect, useState} from 'react';
import axios from "axios";
import {Container} from "semantic-ui-react";
import {OtNode} from "../models/otnode";
import NavBar from "./NavBar";
import NodeDashboard from "../../features/nodes/dashboard/NodeDashboard";

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
                <NodeDashboard nodes={nodes}/>
            </Container>
        </Fragment>
    );
}

export default App;
