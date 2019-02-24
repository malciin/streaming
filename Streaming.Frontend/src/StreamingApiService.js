import { Config } from "./shared/config";

export default class ApiService {
    constructor(props) {
        this.authContext = props.auth;
        this.waitForAuth = this.waitForAuth.bind(this);
        this.addBearerTokenHeader = this.addBearerTokenHeader.bind(this);

        this.getVideos = this.getVideos.bind(this);
        this.getVideo = this.getVideo.bind(this);
        this.uploadVideo = this.uploadVideo.bind(this);
    }

    // TODO: Propably not the correct way for waiting to silent authentication
    // Waiting for authentication
    waitForAuth(callback) {
        if (this.authContext.pendingSilentLogin)
        {
            setTimeout(this.waitForAuth.bind(null, callback), 10);
        }
        else
        {
            callback();
        }
    }

    addBearerTokenHeader(headers) {
        if (!headers)
            headers = {};

        if (this.authContext.getIdToken()) {
            headers['Authorization'] = `Bearer ${this.authContext.getIdToken()}`;
        }

        return headers;
    }

    getVideos(filterObject, callback) {
        fetch(Config.apiPath + '/Video/Search', {
            method: 'POST',
            headers: this.addBearerTokenHeader({
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            }),
            body: JSON.stringify({
                keywords: [],
                offset: 0,
                howMuch: 10
            })
        }).then(responsePromise => responsePromise.json())
        .then(callback);
    }

    getVideo(id, callback) {
        fetch(`${Config.apiPath}/Video/${id}`, {
            headers: this.addBearerTokenHeader({
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            })
        }).then(responsePromise => responsePromise.json())
          .then(callback);
    }

    uploadVideo(data, callback) {
        this.waitForAuth(function(data) {
            var formData = new FormData();
            formData.append("Title", data.videoTitle);
            formData.append("Description", data.videoDescription);
            formData.append("File", data.video);
            
            var xhr = new XMLHttpRequest();
            if (callback)
                xhr.onreadystatechange = callback;
            xhr.open("POST", `${Config.apiPath}/Video`);
            
            if (this.authContext.getIdToken())
                xhr.setRequestHeader('Authorization', `Bearer ${this.authContext.getIdToken()}`);
            
            xhr.send(formData);
        }.bind(this, data));        
    }
}