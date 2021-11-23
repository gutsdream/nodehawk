import React, {useEffect, useState} from 'react';
import logo from './logo.svg';
import './App.css';
import axios from "axios";

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
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <ul>
          {nodes.map((node: any) => (
              <li key={node.id}>
                {node.title}
              </li>
          ))}
        </ul>
      </header>
    </div>
  );
}

export default App;
