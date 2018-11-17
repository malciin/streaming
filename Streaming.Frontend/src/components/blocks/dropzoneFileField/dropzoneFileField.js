import React from 'react';
import Dropzone from 'react-dropzone';
import './dropzoneFileField.scss'

export default class DropzoneFileField extends React.Component {
    constructor(props) {
        super(props);

        this.addFile = this.props.addFile.bind(this);
        this.removeFile = this.props.removeFile.bind(this);
    }

    render() {
        return (
            <Dropzone onDrop={this.addFile} onFileDialogCancel={this.removeFile} className="dropzone" accept={this.props.accept}>
                <div className="dropzone-description">
                    <div>
                        <p><i className="upload-icon icon-upload-cloud"></i></p>
                        <p>Click or drag and drop here a file*</p>
                        <p>* File must be in .mp4 format</p>
                    </div>
                </div>
            </Dropzone>
        );
    }
}