import React from 'react';
import './uploadVideoForm.css';
import Mp4Upload from '../../blocks/mp4upload/mp4Upload';
import TextField from '../../blocks/textField/textField';

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

        console.log(this.state);
    }
    
    render() {
        return (
            <div className="container">
                <form encType="multipart/form-data"></form>
                <TextField />
                <TextField />
                <Mp4Upload videoUploaded={this.videoUploaded} />
                
            </div>
        );
    }
}

export default UploadVideoForm;