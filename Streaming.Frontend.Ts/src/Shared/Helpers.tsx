import * as moment from 'moment';

export default class Helpers {
    public static getHumanizedDateAgoDifference(pastDatetime: moment.Moment): string {
        var dateDifference = moment().diff(pastDatetime);
        return moment.duration(dateDifference).humanize();
    }
}