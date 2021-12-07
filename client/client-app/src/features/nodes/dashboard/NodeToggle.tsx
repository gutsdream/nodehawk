import {Checkbox} from "semantic-ui-react";
import React from "react";
import {OtNode} from "../../../app/models/otnode";
interface Props {
    node: OtNode
    selectNode: (id: string) => void;
    selectedNodes: string[]
}

export default function NodeToggle({node, selectNode, selectedNodes}: Props) {
    
    return (
        <Checkbox onClick={() => selectNode(node.id)} toggle checked={selectedNodes.find(x=>x === node.id) !== undefined}/>
    )
}