import * as React from "react";
import './FileUploadCard.scss';

interface FileUploadCardProps {
    file: File,
    onDeleteFile: (file: File) => void;
}

interface FileUploadCardState {
    file: File,
    uploading: boolean
}

class FileUploadCard extends React.Component<FileUploadCardProps, FileUploadCardState> {
    constructor(props) {
        super(props);
        
        this.state = {
            file: this.props.file,
            uploading: false
        }
    }

    printFriendlyBytes(size: number): string {
        let unit = "B";
        
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
                <div 
                    onClick={(any) => this.props.onDeleteFile(this.props.file)} 
                    className="icon icon-clickable"><span className="icon-trash"></span></div>       
            </div>
            </div>
            
        );
    }
}

export default FileUploadCard;