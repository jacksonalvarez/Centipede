import { Handlers } from "$fresh/server.ts";
import { deleteCookie, setCookie } from "$std/http/cookie.ts";
import * as bcrypt from "https://deno.land/x/bcrypt@v0.2.4/mod.ts";
import * as EmailClient from "npm:@azure/communication-email";
import { CustomDB } from "$libs/database.ts";

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();

    const url = new URL(req.url);
    const form = await req.formData();
    const headers = new Headers();
    headers.set("location", "/login");

    const hash = await bcrypt.hash(form.get("password") as string);
    const myUUID = crypto.randomUUID();
    const user = {
      email: form.get("email") as string,
      username: form.get("username") as string,
      password: hash as string,
      uuid: myUUID,
      verified: false,
      subscribed: false,
    };

    if (
      !user.email.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/) ||
      !user.username.match(/^[a-zA-Z0-9_]{3,16}$/)
    ) {
      setCookie(headers, {
        name: "auth",
        value: "invalid",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
      });
      headers.set("location", "/signup");
      return new Response(null, { status: 303, headers });
    }

    if (!form.has("email") || !form.has("username") || !form.has("password")) {
      setCookie(headers, {
        name: "auth",
        value: "infomissing",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
        path: "/",
        secure: true,
      });
      return new Response(null, { status: 303, headers });
    }

    const usercheck = db.users.getUserByUsername(user.username);
    const emailcheck = db.users.getUserByEmail(user.email);

    if (usercheck || emailcheck) {
      setCookie(headers, {
        name: "auth",
        value: "userexists",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
        path: "/",
        secure: true,
      });
      return new Response(null, { status: 303, headers });
    }

    console.log("Registering user!");
    console.table(user);

    db.users.insertUser(user);

    deleteCookie(headers, "auth", { path: "/", domain: url.hostname });

    db.close();

    const connectionString = `endpoint=${Deno.env.get("COMMUNICATION_SERVICES_ENDPOINT")};accessKey=${Deno.env.get("COMMUNICATION_SERVICES_ACCESS_KEY")}`;
    const emailClient = new EmailClient.EmailClient(connectionString);

    const message = {
      senderAddress: "donotreply@gatewaycorporate.org",
      content: {
        subject: "Verify your email",
        plainText:
          `Click this link to verify your email: https://${url.hostname}/api/verify?uuid=${myUUID}`,
      },
      recipients: {
        to: [
          {
            address: user.email,
            displayName: user.username,
          },
        ],
      },
    };

    const poller = await emailClient.beginSend(message);
    const response = await poller.pollUntilDone();

    console.log("Sending email...");
    console.table({id: response.id, status: response.status});

    return new Response(null, { status: 303, headers });
  },
};
