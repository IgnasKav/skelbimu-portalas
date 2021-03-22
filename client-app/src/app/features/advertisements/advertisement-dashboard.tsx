import React, { useState } from 'react';
import css from './advertisement-dashboard.module.scss';
import { Advertisement } from 'app/models/Advertisement';
import AdvertisementList from './advertisement-list/advertisement-list';
import AdvertisementDetails from './advertisement-deatails/advertisement-details';

interface Props {
    advertisements: Advertisement[];
}

export default function AdvertisementDashboard({ advertisements }: Props) {
    const [selectedAdvetisement, setSelectedAdvertisement] = useState<Advertisement | undefined>(undefined);

    const handleAddSelect = (id: string) => {
        setSelectedAdvertisement(advertisements.find(advertisement => advertisement.id === id));
    }

    const handleAddClose = () => setSelectedAdvertisement(undefined);

    return (
        <div className={css.dashboard}>
            <div className={css.list}>
                <AdvertisementList advertisements={advertisements}
                    onAddSelect={handleAddSelect} />
            </div>
            {selectedAdvetisement && <div className={css.details}>
                <AdvertisementDetails advertisement={selectedAdvetisement} onAddClose={handleAddClose} />
            </div>}
        </div>
    )
}