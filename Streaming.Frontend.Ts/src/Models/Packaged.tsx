export interface PackagedDetails {
    TotalCount: number,
    Count: number
}

export default interface Packaged<T> {
    Items: T[],
    Details: PackagedDetails
}