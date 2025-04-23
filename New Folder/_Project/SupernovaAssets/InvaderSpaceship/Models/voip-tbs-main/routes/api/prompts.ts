import { Handlers } from "$fresh/server.ts";
import { CustomDB } from "$libs/database.ts";
import { getCookies } from "$std/http/cookie.ts";

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);
    const prompt = await req.json();

    if (!cookies.token) {
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    if (!session) {
      return new Response(null, { status: 401 });
    }

    db.prompts.insertPrompt({ ...prompt, user_id: session.user_id });

    console.log(`Blank prompt created: (uuid: ${session.user_id})`);

    db.close();

    return new Response(JSON.stringify({ id: db.prompts.getLastRowID() }), {
      status: 200,
    });
  },

  async DELETE(req) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);
    const prompt = await req.json();

    if (!cookies.token) {
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    if (!session) {
      return new Response(null, { status: 401 });
    }

    db.prompts.deletePrompt(prompt.id);

    console.log("Prompt deleted:");
    console.table(prompt);

    db.close();

    return new Response(null, { status: 200 });
  },

  async PUT(req) {
    const db = new CustomDB();

    const prompt = await req.json();
    const cookies = getCookies(req.headers);

    if (!cookies.token) {
      return new Response(null, { status: 401 });
    }

    const session = db.sessions.getSession(cookies.token);
    if (!session) {
      return new Response(null, { status: 401 });
    }

    db.prompts.updatePrompt({ ...prompt, user_id: session.user_id });

    console.log("Prompt updated:");
    console.table(prompt);

    db.close();

    return new Response(null, { status: 200 });
  },
};
