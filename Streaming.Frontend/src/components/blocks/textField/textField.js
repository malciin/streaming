import React from 'react';

export default function TextField(props) {

    return (
        <div class="form-group">
        <input name={props.name} className="form-control" type="text"/>
        </div>
    );
}