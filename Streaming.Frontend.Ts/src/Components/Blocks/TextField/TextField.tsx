import * as React from 'react';
import './TextField.scss'

interface TextFieldProps {
    label: string;
    name: string;
    onChange: (fieldName: string, value: string) => void;
}

export default class TextField extends React.Component<TextFieldProps> {
    constructor(props: TextFieldProps) {
        super(props);
    }

    render() {
        return (
            <div className="form-group">
                { this.props.label && <label>{this.props.label}</label>}
                <input onChange={(event: React.ChangeEvent<HTMLInputElement>) => 
                        this.props.onChange(this.props.name, event.currentTarget.value)} 
                    name={this.props.name} className="form-control" type="text"/>
            </div>
        );
    }
}