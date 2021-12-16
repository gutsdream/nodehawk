import React from "react";
import {Grid} from "semantic-ui-react";
import NodeList from "./NodeList";
import NodeDetails from "../details/NodeDetails";
import CreateNodeModal from "../create/CreateNodeModal";
import NodeBulkActions from "./NodeBulkActions";
import BulkToggleButton from "./BulkToggleButton";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

function NodeDashboard() {
    const {nodeStore} = useStore();
    const {nodeUnderView} = nodeStore;

    return (<Grid>
        <Grid.Column width='10'>
            <CreateNodeModal/>
            <NodeBulkActions/>
            <BulkToggleButton/>
            <NodeList/>
        </Grid.Column>
        <Grid.Column width='6'>
            {nodeUnderView && <NodeDetails node={nodeUnderView}/>}
        </Grid.Column>
    </Grid>)
}

export default observer(NodeDashboard)