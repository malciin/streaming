import React from 'react';
import Dropzone from 'react-dropzone';
import FileUploadCard from '../fileUploadCard/fileUploadCard';

export default class Mp4Upload extends React.Component {
    
    constructor(props) {
        super(props);

        this.state = {
            video: null
        }

        this.addFile = this.addFile.bind(this);
        this.removeFile = this.removeFile.bind(this);
    }

    addFile(files, rejected) {
        console.log(rejected);
        if (rejected.length > 0) {
            alert('Please upload file in .mp4 format!');
            return;
        }
        this.setState({
            video: files[0]
        }, function () {
            this.props.videoUploaded(this.state.video);
        });
    }

    removeFile() {
        this.setState({
            video: null
        });
    }
    
    render() {
        let child = null;

        if (this.state.video) {
            child = <FileUploadCard file={this.state.video} deleteFile={this.removeFile}/>;
        }
        else {
            child = 
            <Dropzone onDrop={this.addFile} onFileDialogCancel={this.removeFile} className="dropzone" accept="video/mp4">
                <div className="dropzone-description">
                    <div>
                        <p><i className="upload-icon icon-upload-cloud"></i></p>
                        <p>Upload file .mp4</p>
                    </div>
                </div>
            </Dropzone>;
        }
        return (
            <div className="mp4Upload">
                {child}
            </div>
        );
    }
}