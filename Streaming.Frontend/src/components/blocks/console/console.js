import React from 'react';
import './console.scss';

export default class Console extends React.Component {
    constructor(props) {
        super(props);

        this.command = this.command.bind(this);
        this.preventBackspaceFromNavigatingBack = this.preventBackspaceFromNavigatingBack.bind(this);
        this.command = this.command.bind(this);
        this.state = {
            commands: []
        }
    }

    componentDidUpdate() {
        this.scroolToBottom();
    }

    scroolToBottom() {
        const scrollHeight = this.el.scrollHeight;
        const height = this.el.clientHeight;
        const maxScrollTop = scrollHeight - height;
        this.el.scrollTop = maxScrollTop > 0 ? maxScrollTop : 0;
    }

    command(cmd) {
        this.setState({
            commands: [...this.state.commands, cmd]
        });
    }

    componentDidMount() {
        document.addEventListener('keydown', this.preventBackspaceFromNavigatingBack);
        this.scroolToBottom();
    }

    componentWillUnmount() {
        document.removeEventListener('keydown', this.preventBackspaceFromNavigatingBack);
    }

    preventBackspaceFromNavigatingBack(e) {
        if (e.which === 8 || e.which === 222) {
            e.preventDefault();
        }

        var consoleInput = document.getElementById('console-input');
        if (document.activeElement.className == 'console-input') {
            switch(e.key) {
                case 'Enter':
                    this.command(consoleInput.textContent);
                    consoleInput.textContent = "";
                case 'Backspace':
                    consoleInput.textContent = consoleInput.innerText.substring(0, consoleInput.innerText.length - 1);
                    break;
                default:
                    if (e.key.length == 1)
                    {
                        consoleInput.textContent += e.key;
                    }
            }
        }
    }

    render() {
        return <div className="console-container">
            {
                this.props.settings.barEnabled && <div className="console-bar">{this.props.model.title}</div>
            }
            <div className="console-buffer" ref={el => {this.el = el;}}>
            {
                this.props.model.commands.map((cmd, i) => <div key={i}>{cmd}</div>)
            }
            </div>
            { 
                this.props.settings.inputEnabled && <div tabIndex="0" className="console-input">~ <span id="console-input"></span></div>
            }
        </div>
    }
}