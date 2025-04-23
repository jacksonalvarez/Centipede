import { Handlers } from "$fresh/server.ts";
import { getCookies } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);

    if (!cookies.token) {
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    if (!session) {
      return new Response(null, { status: 401 });
    }

    const form = await req.formData();
    const key = {
      user_id: session.user_id,
      key: crypto.randomUUID(),
      number: form.get("phone-number") as string,
      auth_token: form.get("auth-token") as string,
      sid: form.get("sid") as string,
    };
    
    if (key.number == "" || key.auth_token == "" || key.sid == "") {
      return new Response(null, { status: 401 });
    }

    console.log(key);

    db.twilioKeys.deleteTwilioKeyByUUID(session.user_id);
    db.twilioKeys.insertTwilioKey(key);

    console.log("Key created:");
    console.table(key);

    const headers = new Headers();
    headers.set("location", "/messages");

    return new Response(null, { status: 303, headers });
  },
};
