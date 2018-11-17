import React from 'react';
import './fileUploadCard.css'

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
        const { theme } = this.props;
        const {primary, secondary} = theme.palette.text;
        console.log(theme.palette);

        const iconStyle = {
            fontSize: '25px',
            padding: '5px',
            justifyContent: 'space-between',
            color: theme.palette.text.secondary,
            backgroundColor: theme.palette.background.paper
        };

        return (
            <div>
            <div className="upload-card flex-box" style={{backgroundColor: theme.palette.background.paper}}>
                <div className="flex-box">
                <div className="icon" style={{backgroundColor: theme.palette.background.default, color: theme.palette.text.secondary}}><span className="icon-video-alt"></span></div>
                <div className="card-content">
                    <div style={{ color: theme.palette.text.primary }}>{this.state.file.name}</div>
                    <div style={{ color: theme.palette.text.secondary }}>{this.printFriendlyBytes(this.state.file.size)}</div>
                </div>
                </div>
                <div onClick={this.deleteFile} className="icon icon-clickable" style={{backgroundColor: theme.palette.background.default, color: theme.palette.text.secondary}}><span className="icon-trash"></span></div>       
            </div>
            </div>
            
        );
    }
}

export default FileUploadCard;