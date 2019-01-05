import React from 'react';
import DropzoneFileField from '../dropzoneFileField/dropzoneFileField';
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
            this.props.onChange({
                target: {
                    value: this.state.video,
                    name: this.props.name
                }
            });
        });
    }

    removeFile() {
        this.setState({
            video: null
        });

        this.props.onChange({
            target: {
                value: null,
                name: this.props.name
            }
        });
    }
    
    render() {
        let child = null;

        if (this.state.video) {
            child = <FileUploadCard file={this.state.video} deleteFile={this.removeFile}/>;
        }
        else {
            child = <DropzoneFileField addFile={this.addFile} removeFile={this.removeFile} className="dropzone" />;
        }
        return (
            <div className="mp4Upload">
                {child}
            </div>
        );
    }
}