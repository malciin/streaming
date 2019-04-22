import * as React from 'react';
import Dropzone, { DropEvent, DropzoneState } from 'react-dropzone';
import './dropzoneFileField.scss'

interface DropzoneFileFieldSettings {
    onDropFile: (acceptedFiles: File[], rejectedFiles: File[]) => void;
    removeFiles: () => void;
    dropzoneDescription: JSX.Element;
}

export default class DropzoneFileField extends React.Component<DropzoneFileFieldSettings> {
    constructor(props) {
        super(props);
    }

    render() {
        return <Dropzone 
            onDrop={(accepted: File[], rejected: [], event: any) => this.props.onDropFile(accepted, rejected)}
            onFileDialogCancel={this.props.removeFiles}
            children={(state: DropzoneState) => this.props.dropzoneDescription} />;
    }
}