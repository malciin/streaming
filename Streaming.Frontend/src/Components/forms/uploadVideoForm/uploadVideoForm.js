import React from 'react';
import './uploadVideoForm.css';
import Mp4Upload from '../../blocks/mp4upload/mp4Upload';
import TextField from '../../blocks/textField/textField';
import ButtonField from '../../blocks/buttonField/buttonField';

class UploadVideoForm extends React.Component {
    constructor(props) {
        super(props);
        
        console.log(props.apiDefinition);
        this.state = {
            videoTitle: null,
            videoDescription: null,
            video: null
        }

        this.handleInputChange = this.handleInputChange.bind(this);
        this.videoUploaded = this.videoUploaded.bind(this);
    }

    videoUploaded(file) {
        this.setState({
            video: file
        }, function () {
            console.log(this.state.video)
        });
    }

    handleInputChange(event) {
        const target = event.target;
        const value = target.value;
        const name = target.name;

        this.setState({
          [name]: value
        });

        
    }
    
    render() {
        return (
            <div className="container">
                <form encType="multipart/form-data"></form>
                <TextField onChange={this.handleInputChange} label="Please enter a video title" name="videoTitle" />
                <TextField onChange={this.handleInputChange} label="Please enter a video description" name="videoDescription"/>
                <Mp4Upload onChange={this.handleInputChange} name="video" />
                { this.state.video && <ButtonField style={{marginTop: '5px'}} btnType="btn-primary" label="Upload" center /> }
            </div>
        );
    }
}

export default UploadVideoForm;