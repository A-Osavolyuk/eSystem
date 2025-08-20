function base64ToUint8Array(base64) {
    const binary = atob(base64.replace(/-/g, '+').replace(/_/g, '/'));
    const len = binary.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

async function attestate(optionsFromServer) {
    optionsFromServer.challenge = base64ToUint8Array(optionsFromServer.challenge);
    optionsFromServer.user.id = base64ToUint8Array(optionsFromServer.user.id);
    
    const credential = await navigator.credentials.create({
        publicKey: optionsFromServer
    });

    console.log("Credential:", credential);
}