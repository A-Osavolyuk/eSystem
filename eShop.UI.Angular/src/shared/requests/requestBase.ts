import { v4 as uuidv4 } from 'uuid';

export class RequestBase {

    public requestId : string;
    public requestedAt : Date;

    constructor() {
        this.requestId = uuidv4();
        this.requestedAt = new Date();
    }
}