import React, {useEffect, useState} from 'react';
import logo from './logo.svg';
import './App.css';
import axios from "axios";
import {Header, List} from "semantic-ui-react";

function App() {
  const [nodes, setNodes] = useState([]);
  
  useEffect(()=>{
    axios
        .get('http://localhost:5000/api/nodes')
        .then(x=>{
          setNodes(x.data);
        })
  }, [] );
  
  return (
    <div>
        <Header as='h2' icon='terminal' content='nodehawk'/>
        <List>
          {nodes.map((node: any) => (
              <List.Item key={node.id}>
                {node.title}
              </List.Item>
          ))}
        </List>
    </div>
  );
}

export default App;
