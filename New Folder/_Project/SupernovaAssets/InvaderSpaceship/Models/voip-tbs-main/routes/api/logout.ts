import { Handlers } from "$fresh/server.ts";
import { deleteCookie, getCookies } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";

export const handler: Handlers = {
  GET(req) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);
    if (cookies.token) {
      db.sessions.deleteSession(cookies.token);
    }

    const url = new URL(req.url);
    const headers = new Headers(req.headers);
    deleteCookie(headers, "auth", { path: "/", domain: url.hostname });
    deleteCookie(headers, "token", { path: "/", domain: url.hostname });

    db.close();

    headers.set("location", "/");
    return new Response(null, { status: 302, headers });
  },
};
