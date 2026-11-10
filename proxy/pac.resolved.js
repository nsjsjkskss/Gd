// iOS Wi-Fi -> Configure Proxy -> Automatic -> URL:
//   https://okmod.sbs/pac.resolved.js
//
// Only the FreeFire core ABHotUpdates CDN goes through the proxy.
// Login, version.ggwhitehawk.com, optional avatar CDN and everything else
// stay DIRECT.
//
// proxy.okmod.sbs:1299 is a DNS-only (grey cloud) record pointing at
// 217.217.255.156. Cloudflare orange-cloud does not proxy arbitrary TCP ports.
// The IP is kept as a fallback.

function FindProxyForURL(url, host) {
    if (host === "core-gmc.freefiremobile.com" ||
        host === "core-aw.freefiremobile.com" ||
        host === "core-bs.freefiremobile.com" ||
        host === "core-th.freefiremobile.com" ||
        host === "core-vn.freefiremobile.com" ||
        host === "core-sg.freefiremobile.com" ||
        host === "core-id.freefiremobile.com" ||
        (host.indexOf("core-") === 0 && host.indexOf(".freefiremobile.com") !== -1)) {
        return "PROXY proxy.okmod.sbs:1299; PROXY 217.217.255.156:1299; DIRECT";
    }
    return "DIRECT";
}
