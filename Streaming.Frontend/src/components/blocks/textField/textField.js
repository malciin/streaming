import React from 'react';
import './textField.scss'

export default function TextField(props) {

    return (
        <div className="form-group">
            { props.label && <label>{props.label}</label>}
            <input onChange={props.onChange} name={props.name} className="form-control" type="text"/>
        </div>
    );
}