import React, { useEffect, useState } from 'react';
import './styles.css';
import axios from 'axios';
import { Advertisement } from 'app/models/Advertisement';
import AdvertisementDashboard from 'app/features/advertisements/advertisement-dashboard';
import Header from './Header/header';

function App() {
  const [advertisements, setAdvertisements] = useState<Advertisement[]>([]);

  useEffect(() => {
    axios.get<Advertisement[]>('http://localhost:5000/api/advertisements').then(response => {
      setAdvertisements(response.data);
    });
  }, [])

  return (
    <>
      <Header />
      <AdvertisementDashboard advertisements={advertisements} />
    </>
  );
}

export default App;
