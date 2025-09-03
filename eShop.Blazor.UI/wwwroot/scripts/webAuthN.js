function base64ToUint8Array(base64) {
    const binary = atob(base64.replace(/-/g, '+').replace(/_/g, '/'));
    const len = binary.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

async function createKey(options)
{
    options.challenge = base64ToUint8Array(options.challenge);
    options.user.id = base64ToUint8Array(options.user.id);
    return await navigator.credentials.create({
        publicKey: options
    });
}

async function signIn(options)
{
    options.challenge = base64ToUint8Array(options.challenge);
    for (let cred of options.allowedCredentials) {
        cred.id = base64ToUint8Array(cred.id);
    }
    
    return await navigator.credentials.get({ 
        publicKey: options 
    });
}