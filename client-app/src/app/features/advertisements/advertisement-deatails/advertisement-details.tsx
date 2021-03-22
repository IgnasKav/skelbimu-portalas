import React, { useState } from 'react';
import { Advertisement } from 'app/models/Advertisement';
import CloseIcon from 'app/icons/close-icon';
import css from './advertisement-details.module.scss';

interface Props {
    advertisement: Advertisement
    onAddClose: () => void;
}

export default function AdvertisementDetails({ advertisement, onAddClose }: Props) {
    return (
        <>
            <div className={css.title}>{advertisement.title}</div>
            <div className={css.iconWrapper}>
                <CloseIcon onClick={onAddClose} />
            </div>
        </>
    )
}