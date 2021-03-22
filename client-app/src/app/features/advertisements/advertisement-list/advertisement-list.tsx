import React from 'react';
import { Advertisement } from 'app/models/Advertisement';
import css from './advertisement-list.module.scss';

interface Props {
    advertisements: Advertisement[];
    onAddSelect: (id: string) => void;
}

export default function AdvertisementList({ advertisements, onAddSelect }: Props) {
    return (
        <>
            {advertisements.map(advertisement => (
                <div className={css.item} onClick={() => onAddSelect(advertisement.id)}>
                    <div className={css.left}>
                    </div>
                    <div className={css.right}>
                        <div>
                            <div className={css.city}>{advertisement.city}</div>
                            <div className={css.title}>{advertisement.title}</div>
                            <hr className={css.separator} />
                        </div>
                        <div className={css.middleSection}>{advertisement.description}</div>
                        <div className={css.bottomSection}>{advertisement.price} €</div>
                    </div>
                </div>
            ))}
        </>
    )
}