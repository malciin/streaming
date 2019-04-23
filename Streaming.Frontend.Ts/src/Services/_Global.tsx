export default class _Global {
    private dir: { [key: string] : any; }
    private static object : _Global;
    
    static getInstance() : _Global {
        if (!_Global.object) {
            _Global.object = new _Global();
        }
        return _Global.object;
    }

    public getVar(key: string) : any {
        return this.dir[key];
    }

    public setVal(key: string, value: any) {
        this.dir[key] = value;
    }
}