import React from 'react';
import './uploadingVideoStatus.scss';

export default class UploadingVideoStatus extends React.Component {
    // componentDidMount() {
    //     document.addEventListener('keydown', this.preventBackspaceFromNavigatingBack);
    // }

    // componentWillUnmount() {
    //     document.removeEventListener('keydown', this.preventBackspaceFromNavigatingBack);
    // }

    // preventBackspaceFromNavigatingBack(e) {
    //     if (e.which === 8) {
    //         e.preventDefault();
    //     }

    //     var consoleInput = document.getElementById('console-input');
    //     if (document.activeElement.className == 'console-input') {
    //         consoleInput.innerText += e.key;
    //     }
    // }

    printFriendlyBytes(size) {
        size = size/1024/1024;        
        return `${size.toFixed(2)}MB`
    }

    render() {
        return <div className="container">
            <div className="status-box">
                <div className="status-box-content">
                    <div className="flex-box main-upload-info">
                        <div className="progress" style= {{ width: `${this.props.progress}%`}}></div>
                        <div className="icon"><span className="icon-video"></span></div>
                        <div>
                            <div className="video-title">
                                <span className="text-dark-accent">Title:</span> {this.props.video.title}
                            </div>
                            <div className="video-description">
                                <span className="text-dark-accent">Description:</span> {this.props.video.description}
                            </div>

                            <div className="video-description">
                                <span className="text-dark-accent">Upload progress:</span>
                                <span className="consolata-font"> {parseFloat(this.props.progress).toFixed(1)}
                                % ({this.printFriendlyBytes(this.props.progress / 100 * this.props.video.file.size)}/{this.printFriendlyBytes(this.props.video.file.size)}) </span> 
                            </div>
                        </div>
                    </div>
                    <div className="upload-realtime-info">
                        Process information:
                        <div className="buffer">
                        { this.props.output.map((command, index) => <div key={index}>{command}</div>) }
                        </div>
                        {/* <div tabIndex="0" className="console-input">
                            ~ <span id="console-input"></span>_
                        </div> */}
                    </div>
                </div>
            </div>
        </div>
    }
}