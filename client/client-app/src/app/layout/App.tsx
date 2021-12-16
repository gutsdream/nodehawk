import React, {Fragment, useEffect} from 'react';
import {Container} from "semantic-ui-react";
import NodeDashboard from "../../features/nodes/dashboard/NodeDashboard";
import LoadingComponent from "./LoadingComponent";
import {useStore} from "../stores/store";
import {observer} from "mobx-react-lite";
import NavBar from "./NavBar";

function App() {
    const {nodeStore} = useStore();

    useEffect(() => {
        nodeStore.loadNodes();
    }, [nodeStore]);

    if (nodeStore.loadingInitial) return <LoadingComponent/>

    return (
        <Fragment>
            <NavBar/>
            <Container>
                <NodeDashboard/>
            </Container>
        </Fragment>
    );
}

export default observer(App);
