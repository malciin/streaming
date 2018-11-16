import React from 'react';
import Dropzone from 'react-dropzone';
import './uploadVideoForm.css';
import Button from '@material-ui/core/Button';
import { TextField } from '@material-ui/core';

class UploadVideoForm extends React.Component {
    constructor(props) {
        super(props);
        
        console.log(props.apiDefinition);
        this.state = {
            videoTitle: null,
            videoDescription: null,
            video: []
        }

        this.onDrop = this.onDrop.bind(this);
        this.onCancel = this.onCancel.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    uploadVideo(event) {
        event.preventDefault();
        
        var formData = new FormData();
        formData.append("Title", this.state.videoTitle);
        formData.append("Description", this.state.videoDescription);
        formData.append("File", this.state.video[0]);

        var xhr = new XMLHttpRequest();
        xhr.open("POST", this.props.apiDefinition.post);
        xhr.send(formData);
    }

    onDrop(files, rejected) {
        console.log(rejected);
        if (rejected.length > 0) {
            alert('Please upload file in .mp4 format!');
            return;
        }
        this.setState({
            video: files
        });
        console.log(files);
    }

    onCancel() {
        this.setState({
            video: []
        });
    }

    handleInputChange(event) {
        const target = event.target;
        const value = target.value;
        const name = target.name;
    
        this.setState({
          [name]: value
        });

        console.log(this.state);
    }
    
    render() {
        return (
            <div className="container">
                <form encType="multipart/form-data"></form>
                <TextField name="videoTitle" onChange={this.handleInputChange} label="Video title" fullWidth margin="normal"/>
                <TextField name="videoDescription" onChange={this.handleInputChange} label="Video description" fullWidth margin="normal"/>
                <Dropzone onDrop={this.onDrop} onFileDialogCancel={this.onCancel} className="dropzone" accept="video/mp4">
                    <div className="dropzone-description">
                        <div>
                            <p><i className="upload-icon icon-upload-cloud"></i></p>
                            <p>Upload file .mp4</p>
                        </div>
                    </div>
                    
                </Dropzone>
                <div className="justify-content-center align-items-center">
                    <Button variant="contained" onClick={this.uploadVideo}>Upload</Button>
                </div>
            </div>
        );
    }
}

export default UploadVideoForm;