import {Button, Dropdown} from "semantic-ui-react";
import React from "react";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";


function NodeBulkActions() {
    const {nodeStore} = useStore();
    const {selectedNodesForBulk} = nodeStore;

    return (
        <Dropdown color='white' disabled={selectedNodesForBulk.length === 0} button text='Bulk Actions' style={{backgroundColor: 'black', color: 'white'}}>
            <Dropdown.Menu>
                <Dropdown.Item>Refresh</Dropdown.Item>
                <Dropdown.Item>Clean</Dropdown.Item>
                <Dropdown.Item>Backup</Dropdown.Item>
            </Dropdown.Menu>
        </Dropdown>
    )
}

export default observer(NodeBulkActions)
