export default class ApiService {
    constructor(props) {
        this.authContext = props.auth;
        this.getVideos = this.getVideos.bind(this);
        this.waitForAuth = this.waitForAuth.bind(this);
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

    getVideos(filterObject, callback) {
        var headers = {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
        };
        
        if (this.authContext.getIdToken()) {
            headers['Authentication'] = `Bearer ${this.authContext.getIdToken()}`;
        }

        fetch(Config.apiPath + '/Video/Search', {
            method: 'POST',
            headers: headers,
            body: JSON.stringify({
                keywords: [],
                offset: 0,
                howMuch: 10
            })
        }).then(responsePromise => responsePromise.json())
        .then(callback);
    }

    getVideo(id, callback) {
        fetch(`${Config.apiPath}/Video/${id}`)
            .then(responsePromise => responsePromise.json())
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
            xhr.send(formData);
        }.bind(null, data));        
    }
}