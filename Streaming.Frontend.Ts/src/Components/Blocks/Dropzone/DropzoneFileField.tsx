import * as React from 'react';
import Dropzone, { DropEvent } from 'react-dropzone';
import './DropzoneFileField.scss'

interface DropzoneFileFieldSettings {
    onDropFile?: (acceptedFiles: File[], rejectedFiles: File[]) => void;
    removeFiles?: () => void;
}

export default class DropzoneFileField extends React.Component<DropzoneFileFieldSettings> {
    constructor(props) {
        super(props);
    }

    render() {
        return <div className="dropzone"><Dropzone minSize={200}
            onDrop={(accepted: File[], rejected: File[], event: DropEvent) => this.props.onDropFile(accepted, rejected)} 
            onFileDialogCancel={this.props.removeFiles}>
                {({getRootProps, getInputProps}) => (
    <section>
      <div {...getRootProps()}>
        <input {...getInputProps()} />
        <div className="dropzone-description">
                    <div>
                        <p><i className="upload-icon icon-upload-cloud"></i></p>
                        <p>Click or drag and drop here a file*</p>
                        <p>* File must be in .mp4 format</p>
                    </div>
                </div>
      </div>
    </section>
  )}
        </Dropzone>
        </div>
    }
}