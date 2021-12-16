import {Button, Checkbox} from "semantic-ui-react";
import React from "react";
import {OtNode} from "../../../app/models/otnode";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

function BulkToggleButton() {
    const {nodeStore} = useStore();
    const {bulkToggle} = nodeStore;

    return (
        <Button onClick={() => bulkToggle()} content='Select All' style={{backgroundColor: 'white', float: 'right'}}/>
    )
}

export default observer(BulkToggleButton)