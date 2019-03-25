import React from 'react';
import Navbar from '../../components/navbar/Navbar';
import { AppContext } from '../../AppContext';
import UploadVideoForm from '../../components/forms/uploadVideoForm/uploadVideoForm';
import UploadingVideoStatus from '../../components/blocks/uploadingVideoStatus/uploadingVideoStatus';
import Console from '../../components/blocks/console/console';
import { Config } from '../../shared/config';
var signalR = require('@aspnet/signalr');

class UploadVideoPage extends React.Component{
    constructor(props) {
        super(props);
        this.uploadVideo = this.uploadVideo.bind(this);
        this.state = {
            uploading: false,
            progress: 0.0,
            output: ["- - - Upload Video Shell 0.22 - - -"]
        };

        this.consoleOutput = this.consoleOutput.bind(this);        
    }

    consoleOutput(line) {
        if (line.startsWith('frame')) {
            this.setState({
                output: [...this.state.output.slice(0, this.state.output.length - 1), line]
            })
        }
        else {
            this.setState({
                output: [...this.state.output, line]
            });
        }
    }

    async componentDidMount()
    {
        await this.context.auth.silentLogin();
        await this.context.auth.waitForAuth();
    }

    async uploadVideo(data) {
        this.videoInfo = {
            title: data.videoTitle,
            description: data.videoDescription,
            file: data.video
        }
        this.consoleOutput("Video uploading...");
        this.setState({
            uploading: true
        });
        try {
            this.connection = new signalR.HubConnectionBuilder().withUrl(`${Config.apiPath}/hub/processingInfo`, {
                accessTokenFactory: () => this.context.auth.idToken,
            }).build();

            this.connection.on("ProcessingInformation", function (msg) {
                this.consoleOutput(msg);
            }.bind(this));

            this.connection.start();
            
            await this.context.streamingApi.uploadVideo(data, progress => this.setState({
                progress: progress
            }));
            this.consoleOutput("Video successfully uploaded...");
            this.consoleOutput("Video will start processing!");
        } catch (exception) {
            this.consoleOutput(`Exception: ${exception.message}`);
        }
    }

    render() {
        return (
            <div>
                <Navbar />
                <div className="container">
                { !this.state.uploading && <UploadVideoForm submit={this.uploadVideo}/> }
                { this.state.uploading && 
                <UploadingVideoStatus video={this.videoInfo} progress={this.state.progress} output={this.state.output} /> }
                { this.state.uploading &&    
                    <Console settings = {{
                        barEnabled: true,
                        inputEnabled: false
                    }} model= {{
                        title: "Progress",
                        commands: this.state.output
                    }}/> }
                </div>
            </div>
        );
    }
}

UploadVideoPage.contextType = AppContext;
export default UploadVideoPage;