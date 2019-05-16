import * as React from 'react';
import './TextField.scss'

interface TextFieldProps {
    label: string;
    name: string;
    multiline?: boolean,
    className?: string,
    onChange: (fieldName: string, value: string) => void;
}

export default class TextField extends React.Component<TextFieldProps> {
    constructor(props: TextFieldProps) {
        super(props);
    }

    render() {
        var handleChange = (event: any) => 
        this.props.onChange(this.props.name, event.currentTarget.value)

        var input;
        if (!this.props.multiline) {
            input = <input className={`input ${this.props.className}`} onChange={handleChange} 
                name={this.props.name} type="text"/>;
        }
            
        else {
            input = <textarea className={`input ${this.props.className}`} onChange={element => {
                element.currentTarget.style.height = "";
                element.currentTarget.style.height = element.currentTarget.scrollHeight + 10 + "px";
                handleChange(element);
            }} name={this.props.name}/>;
        }
        return (
            <div className="text-input">
                { this.props.label && <label>{this.props.label}</label>}
                { input }
            </div>
        );
    }
}