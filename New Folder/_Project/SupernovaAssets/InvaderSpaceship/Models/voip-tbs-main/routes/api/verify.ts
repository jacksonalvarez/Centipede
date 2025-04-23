import { Handlers } from "$fresh/server.ts";
import { setCookie } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";
import { IUser } from "$types/data.ts";

export const handler: Handlers = {
  GET(req) {
    const db = new CustomDB();

    const url = new URL(req.url);
    const headers = new Headers();
    headers.set("location", "/");

    const uuid = url.searchParams.get("uuid");

    const user = db.users.getUserByUUID(uuid as string) as IUser;

    if (user) {
      db.prepare("UPDATE users SET verified = ? WHERE uuid = ?").run(
        true,
        uuid,
      );
    } else {
      setCookie(headers, {
        name: "auth",
        value: "unregistered",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
        path: "/",
        secure: true,
      });
    }

    console.log(`User verified: (uuid: ${user.id})`);

    db.close();

    return new Response(null, { status: 303, headers });
  },
};
