import LiveStreamMetadata from "../Models/LiveStreamMetadata";
import VideoMetadata from "../Models/VideoMetadata";
import * as moment from 'moment';
import Auth0User, { AppMetadata } from "../Models/Auth0User";

export default class Mapper {
    public static mapLiveStreamMetadata(jsonObject: any): LiveStreamMetadata {
        let result: LiveStreamMetadata = {
            liveStreamId: jsonObject['liveStreamId'],
            title: jsonObject['title'],
            userStarted: jsonObject['userStarted'],
            manifestUrl: jsonObject['manifestUrl'],
            started: moment(jsonObject['started'])
        };
        return result;
    }

    public static mapVideoMetadata(jsonObject: any): VideoMetadata {
        let result: VideoMetadata = {
            videoId: jsonObject["videoId"],
            title: jsonObject["title"],
            description: jsonObject["description"],
            createdDate: moment(jsonObject["createdDate"]),
            length: jsonObject["length"],
            ownerNickname: jsonObject["ownerNickname"],
            thumbnailUrl: jsonObject["thumbnailUrl"]
        };
        return result;
    }

    // Check sample response from https://auth0.com/docs/api/management/v2#!/Users/get_users to correct map
    public static mapAuth0User(jsonObject: any): Auth0User {
        let result: Auth0User = {
            appMetadata: {
                claims: jsonObject["app_metadata"]["claims"]
            },
            createdAt: moment(jsonObject["created_at"]),
            email: jsonObject["email"],
            emailVerified: jsonObject["email_verified"],
            givenName: jsonObject["given_name"],
            name: jsonObject["name"],
            nickname: jsonObject["nickname"],
            picture: jsonObject["picture"],
            userId: jsonObject["user_id"],
            username: jsonObject["username"]
        };
        return result;
    }
}