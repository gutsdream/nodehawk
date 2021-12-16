import {Checkbox} from "semantic-ui-react";
import React from "react";
import {OtNode} from "../../../app/models/otnode";
import {useStore} from "../../../app/stores/store";
import {observer} from "mobx-react-lite";

interface Props {
    node: OtNode
}

function NodeToggle({node}: Props) {
    const {nodeStore} = useStore();
    const {selectNode, selectedNodesForBulk} = nodeStore;
    return (
        <Checkbox onClick={() => selectNode(node.id)} toggle checked={selectedNodesForBulk.find(x => x === node.id) !== undefined}/>
    )
}

export default observer(NodeToggle)