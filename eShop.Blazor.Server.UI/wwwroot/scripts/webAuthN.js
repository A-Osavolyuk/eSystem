function base64ToUint8Array(base64) {
    const binary = atob(base64.replace(/-/g, '+').replace(/_/g, '/'));
    const len = binary.length;
    const bytes = new Uint8Array(len);
    for (let i = 0; i < len; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return bytes;
}

async function assert(options)
{
    options.challenge = base64ToUint8Array(options.challenge);
    options.user.id = base64ToUint8Array(options.user.id);
    try {
        const credential = await navigator.credentials.create({ publicKey: options });
        return { success: true,  data: credential }
    }
    catch (error) {
        return {
            success: false,
            error: {
                message: error?.message ?? "Unknown error",
                type: error?.name?? "UnknownError"
            }
        }
    }
}

async function authenticate(options)
{
    options.challenge = base64ToUint8Array(options.challenge);
    for (let cred of options.allowedCredentials) {
        cred.id = base64ToUint8Array(cred.id);
    }
    try {
        const credential = await navigator.credentials.get({ publicKey: options });
        return { success: true,  data: credential }
    }
    catch (error) {
        return {
            success: false,
            error: {
                message: error?.message ?? "Unknown error",
                type: error?.name?? "UnknownError"
            }
        }
    }
}