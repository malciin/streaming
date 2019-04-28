import * as moment from "moment";

export default interface VideoMetadata {
    videoId: string,
    title: string,
    description: string,
    createdDate: moment.Moment,
    thumbnailUrl: string,
    length: number,
    ownerNickname: string
}