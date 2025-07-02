import { RequestBase } from "./requestBase";

export class LoginRequest extends RequestBase{
    constructor(public email?: string, public password? : string) {
        super();
    }
}