import { Handlers } from "$fresh/server.ts";
import { deleteCookie, setCookie } from "$std/http/cookie.ts";
import * as bcrypt from "https://deno.land/x/bcrypt@v0.2.4/mod.ts";
import { CustomDB } from "$libs/database.ts";
import { IUser } from "$types/data.ts";

const authenticate = async (username: string, password: string) => {
  const db = new CustomDB();

  const usercheck = db.users.getUserByUsername(username);
  const emailcheck = db.users.getUserByEmail(username);

  if (usercheck) {
    const user = usercheck as IUser;
    if (await bcrypt.compare(password, user.password)) {
      return user;
    }
  } else if (emailcheck) {
    const user = emailcheck as IUser;
    if (await bcrypt.compare(password, user.password)) {
      return user;
    }
  }

  db.close();

  return null;
}

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();
    const url = new URL(req.url);
    const form = await req.formData();
    const headers = new Headers();
    headers.set("location", "/login");

    if (form.get("username") === "" && form.get("password") === "") {
      deleteCookie(headers, "auth", { path: "/", domain: url.hostname });
    } 

    const user = await authenticate(form.get("username") as string, form.get("password") as string);

    console.assert(Boolean(user), "Login attempted with invalid credentials");

    if (!user) {
      setCookie(headers, {
        name: "auth",
        value: "invalid",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
        path: "/",
        secure: true,
      });

      return new Response(null, { status: 303, headers });
    }

    setCookie (headers, {
      name: "auth",
      value: "valid",
      maxAge: 60 * 60 * 24 * 7,
      sameSite: "Lax",
      domain: url.hostname,
      path: "/",
      secure: true
    });

    console.log("User authenticated:");
    console.table({uuid: user!.id, username: user!.username, email: user!.email});

    const token = crypto.randomUUID();

    db.sessions.insertSession({token: token, user_id: user!.id, uuid: user!.uuid});

    setCookie(headers, {
      name: "token",
      value: token,
      maxAge: 60 * 60 * 24 * 7,
      sameSite: "Lax",
      domain: url.hostname,
      path: "/",
      secure: true
    });

    headers.set("location", "/analytics");

    return new Response(null, { status: 303, headers });
  },
};
