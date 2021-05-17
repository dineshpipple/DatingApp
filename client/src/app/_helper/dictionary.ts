export class Dictionary {
    items = {};
    constructor() {
      this.items = {};
    }
    public has(key) {
      return key in this.items;
    }
    public set(key,value) {
        debugger;
      this.items[key] = value;
    }
    public get(key) {
      return this.items[key];
    }
    public delete(key) {
      if( this.has(key) ){
        delete this.items[key]
        return true;
      }
      return false;
    }
  }