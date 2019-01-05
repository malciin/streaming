import React from 'react';
import './fileUploadCard.scss'

class FileUploadCard extends React.Component {

    constructor(props) {
        super(props);
        this.state = {
            file: this.props.file,
            uploading: false
        }

        this.deleteFile = this.props.deleteFile.bind(this);
    }

    printFriendlyBytes(size) {
        var unit = "B";
        
        if (size > 1024)
        {
            size = size / 1024;
            unit = "KB";
        }
        if (size > 1024)
        {
            size = size / 1024;
            unit = "MB";
        }
        return `${size.toFixed(2)} ${unit}`
    }
    
    render() {

        return (
            <div>
            <div className="upload-card flex-box text-primary-color">
                
                <div className="flex-box">
                <div className="icon"><span className="icon-video"></span></div>
                <div className="card-content">
                    <div className="text-primary-color">{this.state.file.name}</div>
                    <div className="text-secondary-color">{this.printFriendlyBytes(this.state.file.size)}</div>
                </div>
                </div>
                <div onClick={this.deleteFile} className="icon icon-clickable"><span className="icon-trash"></span></div>       
            </div>
            <div className="progress">
                <div className="progress-bar" role="progressbar" style={{width: '0%'}} aria-valuenow="50" aria-valuemin="0" aria-valuemax="100"></div>
            </div>
            </div>
            
        );
    }
}

export default FileUploadCard;