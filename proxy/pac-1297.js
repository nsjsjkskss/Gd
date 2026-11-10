// iOS Wi-Fi -> Configure Proxy -> Automatic -> URL:
//   https://okmod.sbs/pac-1297.js
//
// Routes ONLY the FreeFire hotupdate hosts that the 1297 addon intercepts.
// Login, Apple, optional avatar CDN and everything else stay DIRECT.
// 1297 serves the OKMOD R11 build (patch.bin).

function FindProxyForURL(url, host) {
    if (host === "core-gmc.freefiremobile.com" ||
        host === "core-aw.freefiremobile.com" ||
        host === "core-bs.freefiremobile.com" ||
        host === "dl-core.cdn.freefiremobile.com") {
        return "PROXY proxy.okmod.sbs:1106; PROXY 217.217.255.156:1106; DIRECT";
    }
    return "DIRECT";
}
