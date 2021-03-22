import React from 'react';
import { Advertisement } from 'app/models/Advertisement';
import './advertisement-list.scss';

interface Props {
    advertisements: Advertisement[];
    onAddSelect: (id: string) => void;
}

export default function AdvertisementList({ advertisements, onAddSelect }: Props) {
    return (
        <>
            {advertisements.map(advertisement => (
                <div className='item' onClick={() => onAddSelect(advertisement.id)}>
                    <div className='left'>
                    </div>
                    <div className='right'>
                        <div className="top-section">
                            <div className='city'>{advertisement.city}</div>
                            <div className='title'>{advertisement.title}</div>
                            <hr className='separator' />
                        </div>
                        <div className='middle-section'>{advertisement.description}</div>
                        <div className='bottom-sectoin'>{advertisement.price} €</div>
                    </div>
                </div>
            ))}
        </>
    )
}