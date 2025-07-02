export class ResponseDto {
    constructor(
        public resultMessage? : string,
        public errorMessage? : string,
        public isSucceeded? : boolean,
        public errors? : string[],
        public result? : object,
    ){}
}