import React, { useEffect, useState } from 'react';
import './styles.css';
import axios from 'axios';
import { Header, List } from 'semantic-ui-react';

function App() {
  const [advertisements, setAdvertisements] = useState([]);

  useEffect(() => {
    axios.get('http://localhost:5000/api/advertisements').then(response => {
      setAdvertisements(response.data);
    });
  }, [])

  return (
    <div className="App">
      <Header as='h2' icon='users' content='Reactivities' />
      <List>
        {
          advertisements.map((advertisement: any) => (
            <li key={advertisement.id}>
              {advertisement.title}
            </li>
          ))
        }
      </List>
    </div>
  );
}

export default App;
