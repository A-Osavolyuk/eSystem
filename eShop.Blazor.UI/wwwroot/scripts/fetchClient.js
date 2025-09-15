async function fetchApi(options){
    const result = await fetch(options.url, {
        method: options.method,
        headers: options.headers,
        body: options.body,
        credentials: options.credentials
    })
    
    const json = await result.json()
    console.log(json)
    
    return json
}