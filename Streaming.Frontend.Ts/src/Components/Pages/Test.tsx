import * as React from 'react';
import TextField from '../Blocks/TextField/TextField';

export default class Test extends React.Component {
    onChange(value: any) {
        console.log(value);
    }

    render() {
        return <TextField label="label" name="name" onChange={this.onChange} />
    }
}