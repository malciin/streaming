import { Config } from "./shared/config";

export default class ApiService {
    constructor(props) {
        this.authContext = props.authContext;
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

    async getVideos(filterObject, callback) {
        this.waitForAuth(function(filterObject) {
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
        }.bind(this, filterObject));
    }
}