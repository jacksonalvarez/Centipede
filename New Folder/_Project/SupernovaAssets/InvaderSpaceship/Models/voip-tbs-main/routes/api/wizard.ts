import { Handlers } from "$fresh/server.ts";
import { CustomDB } from "$libs/database.ts";
import { getCookies } from "$std/http/cookie.ts";

export const handler: Handlers = {
  async POST(req) {
    const form = await req.formData();
    const cookies = getCookies(req.headers);

    const db = new CustomDB();

    if (!cookies.token) {
      const headers = new Headers();
      headers.set("location", "/login");
      return new Response(null, { status: 303, headers });
    }

    const session = db.sessions.getSession(cookies.token);

    if (!session) {
      const headers = new Headers();
      headers.set("location", "/login");
      return new Response(null, { status: 303, headers });
    }
    
    const agent = {
      user_id: session.user_id,
      agent_name: form.get("character-name") as string,
      business_name: form.get("business-name") as string,
      personality: form.get("personality") as string,
      directives: form.get("directives") as string,
      fallback_contact: form.get("fallback-name") as string,
      fallback_number: form.get("fallback-number") as string,
    }

    db.agents.deleteAgent(session.user_id);
    db.agents.insertAgent(agent);

    console.log(`Agent created: (uuid: ${session.user_id})`);
    console.table(agent);

    const headers = new Headers();
    headers.set("location", "/prompts");
    return (new Response(null, { status: 303, headers }));
  }
};