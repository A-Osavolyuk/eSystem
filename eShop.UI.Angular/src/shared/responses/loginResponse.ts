import { UserDto } from "../dtos/userDto";

export class LoginResponse {
    constructor(
        public user : UserDto,
        public accessToken : string,
        public refreshToken : string,
        public message : string,
        public hasTwoFactorAuthentication : boolean
    ){}
}