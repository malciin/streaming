import * as moment from 'moment';

export default interface LiveStreamMetadata {
    liveStreamId: string,
    title: string,
    manifestUrl: string,
    started: moment.Moment,
    userStarted: string
}