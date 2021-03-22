import React, { useState } from 'react';
import './advertisement-dashboard.scss';
import { Advertisement } from '../../app/models/Advertisement';
import AdvertisementList from './advertisement-list/advertisement-list';
import AdvertisementDetails from './advertisement-deatails/advertisement-details';

interface Props {
    advertisements: Advertisement[];
}

export default function AdvertisementDashboard({ advertisements }: Props) {
    const [selectedAdvetisement, setSelectedAdvertisement] = useState<Advertisement | undefined>(undefined);

    const handleAdSelect = (id: string) => {
        console.log('advertisement selected');
        setSelectedAdvertisement(advertisements.find(advertisement => advertisement.id === id));
    }

    return (
        <div className="dashboard">
            <div className="list">
                <AdvertisementList advertisements={advertisements}
                    onAddSelect={handleAdSelect} />
            </div>
            {selectedAdvetisement && <div className="details">
                <AdvertisementDetails advertisement={selectedAdvetisement} />
            </div>}
        </div>
    )
}