import {Button, Dropdown} from "semantic-ui-react";
import React from "react";
import {OtNode} from "../../../app/models/otnode";

interface Props {
    selectedNodes: OtNode[]
}

export default function NodeBulkActions({selectedNodes}: Props) {
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