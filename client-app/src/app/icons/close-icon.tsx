import React, { useState } from "react";
import { HiXCircle, HiOutlineXCircle } from "react-icons/hi";
import css from './close-icon.module.scss';

interface Props {
    onClick: () => void;
}

export default function CloseIcon({ onClick }: Props) {
    const [isHovered, setHover] = useState<Boolean>(false);

    return (
        <>
            {isHovered ? <HiXCircle /> : <HiOutlineXCircle />}
            <div className={css.hoverArea} onClick={() => onClick()} onMouseEnter={() => setHover(true)} onMouseLeave={() => setHover(false)}></div>
        </>
    )
}