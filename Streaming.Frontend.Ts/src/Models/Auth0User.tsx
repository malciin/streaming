import * as moment from 'moment';

export default interface Auth0User {
    email: string,
    emailVerified: boolean,
    username: string,
    userId: string,
    createdAt: moment.Moment,
    appMetadata: AppMetadata,
    picture: string,
    name: string,
    nickname: string,
    givenName: string
}

export interface AppMetadata {
    claims: string[]
}