import React from 'react';

export default function ButtonField(props) {
    return(
        <div style={props.style} className={props.center && 'text-center'}>
        <button onClick={props.onClick} type="button" className={`btn ${props.btnType}`}>{props.label}</button>
        </div>
    );
}