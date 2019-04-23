import * as React from 'react';

interface ButtonFieldProps {
    label: string,
    buttonType: string,
    center: boolean,
    onClick: () => void;
}

export default class ButtonField extends React.Component<ButtonFieldProps> {
    render() {
        return(
            <div className={this.props.center && 'text-center'}>
            <button onClick={this.props.onClick} type="button" className={`btn ${this.props.buttonType}`}>{this.props.label}</button>
            </div>
        );
    }
}