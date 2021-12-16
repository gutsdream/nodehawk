import {Button, Dropdown} from "semantic-ui-react";
import React from "react";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";


function NodeBulkActions() {
    const {nodeStore} = useStore();
    const {selectedNodesForBulk} = nodeStore;

    return (
        <Button color='black'>
            <Dropdown item text='Bulk Actions'>
                <Dropdown.Menu>
                    <Dropdown.Item>Refresh</Dropdown.Item>
                    <Dropdown.Item>Clean</Dropdown.Item>
                    <Dropdown.Item>Backup</Dropdown.Item>
                </Dropdown.Menu>
            </Dropdown>
        </Button>
    )
}
export default observer(NodeBulkActions)
