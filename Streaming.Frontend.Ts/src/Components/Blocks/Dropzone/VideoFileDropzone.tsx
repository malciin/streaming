import * as React from 'react';
import DropzoneFileField from './DropzoneFileField';

interface VideoFileDropzoneProps {
    acceptedExtensions?: string[],
    acceptedFileTypes?: string[],
    onDropFile: (videoFile: File) => void;
}

interface VideoFileDropzoneState {
    video: File[]
}

export default class VideoFileDropzone extends React.Component<VideoFileDropzoneProps,VideoFileDropzoneState> {
    constructor(props) {
        super(props);

        this.state = {
            video: null
        }

        this.onDropFile = this.onDropFile.bind(this);
        this.onRemoveFiles = this.onRemoveFiles.bind(this);
    }

    onDropFile(acceptedFiles: File[], rejectedFiles: File[]) : void {
        if (acceptedFiles.length == 1) {
            let file = acceptedFiles[0];
            let fileExtension = '.' + file.name.split('.').pop();
            if(this.props.acceptedExtensions && !this.props.acceptedExtensions.includes(fileExtension)) {
                alert(`Please select file with ${this.props.acceptedExtensions} extensions!`);
                return;
            }
            if (this.props.acceptedFileTypes && !this.props.acceptedFileTypes.includes(file.type)) {
                alert(`Selected file is not a correct ${fileExtension} format!`)
                return;
            }
            this.props.onDropFile(file);
        }
        else {
            acceptedFiles.forEach(file => {
                rejectedFiles.push(file);
            });
        }
        console.log("Accepted files: ");
        console.log(acceptedFiles);
        console.log("Rejected files: ");
        console.log(rejectedFiles);
    }

    onRemoveFiles() : void {

    }
    
    render() {
        return <DropzoneFileField onDropFile={this.onDropFile} removeFiles={this.onRemoveFiles} >
                    <div>
                        <p><i className="upload-icon icon-upload-cloud"></i></p>
                        <p>Click or drag and drop here a file*</p>
                        <p>* File must be in .mp4 format</p>
                    </div>
            </DropzoneFileField>
    }
}