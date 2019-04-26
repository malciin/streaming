import * as React from 'react';
import './Console.scss';

export interface ConsoleProps {
    barEnabled: boolean,
    title: string
    inputEnabled: boolean,
    consoleOutputBuffer: string[],
    handleConsoleOutput: (output: string) => void;
}


export default class Console extends React.Component<ConsoleProps> {

    private consoleOutputElement: HTMLElement

    constructor(props : ConsoleProps) {
        super(props);
        this.props

        this.pushOutput = this.pushOutput.bind(this);
        this.preventBackspaceFromNavigatingBack = this.preventBackspaceFromNavigatingBack.bind(this);
        this.state = {
            consoleOutputBuffer: []
        }
    }

    componentDidUpdate() {
        this.scroolToBottom();
    }

    scroolToBottom() {
        const scrollHeight = this.consoleOutputElement.scrollHeight;
        const height = this.consoleOutputElement.clientHeight;
        const maxScrollTop = scrollHeight - height;
        this.consoleOutputElement.scrollTop = maxScrollTop > 0 ? maxScrollTop : 0;
    }

    public pushOutput(output) {
        this.props.handleConsoleOutput(output);
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
                    this.pushOutput(consoleInput.textContent);
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
                this.props.barEnabled && <div className="console-bar">{this.props.title}</div>
            }
            <div className="console-buffer" ref={el => {this.consoleOutputElement = el;}}>
            {
                this.props.consoleOutputBuffer.map((cmd, i) => <div key={i}>{cmd}</div>)
            }
            </div>
            { 
                this.props.inputEnabled && <div tabIndex={0} className="console-input">~ <span id="console-input"></span></div>
            }
        </div>
    }
}