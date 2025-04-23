import Navbar from "../islands/Navbar.tsx";
import { Handlers, PageProps } from "$fresh/server.ts";
import { setCookie, getCookies } from "$std/http/cookie.ts";
import { CustomDB } from "$libs/database.ts";
import { IContact, IMessage } from "$types/data.ts";
import { authenticateToken } from "$libs/authentication.ts";
import APIKeyManager from "../islands/APIKeyManager.tsx";
import MessageManager from "../islands/MessageManager.tsx";

interface Data {
  key: string;
  contacts: IContact[];
  messages: IMessage[];
}

export const handler: Handlers = {
  GET(req, ctx) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);
    const url = new URL(req.url);
    const headers = new Headers();

    if (!cookies.token) {
      db.close();
      headers.set("location", "/login");
      setCookie(headers, {
        name: "auth",
        value: "unauthorized",
        maxAge: 120,
        sameSite: "Lax",
        domain: url.hostname,
      });
      return new Response(null, { status: 303, headers });
    }

    const user = authenticateToken(cookies.token, headers, req);

    if (!user) {
      db.close();
      headers.set("location", "/login");
      return new Response(null, { status: 303, headers });
    }

    const contacts = db.contacts.getContacts(user.id);
    const messages = db.messages.getMessages(user.id);
    const api_key = db.twilioKeys.getTwilioKeyByUserID(user.id);

    const auth = api_key ? api_key.key : "-1";

    db.close();
    return ctx.render!({ key: auth, contacts: contacts, messages: messages });
  },
};

export default function MessagesPage({ data }: PageProps<Data>) {
  return (
    <div class="h-screen mx-auto bg-dark">
      <div class="max-w-screen-md z-40 mx-auto flex flex-col items-center justify-start space-y-2 relative top-16 md:top-20 xl:top-28">
        <Navbar />
        {data.key === "-1" ? (
          <APIKeyManager />
        ) : (
          <MessageManager messages={data.messages} contacts={data.contacts} apikey={data.key}/>
        )}
      </div>
    </div>
  );
}
