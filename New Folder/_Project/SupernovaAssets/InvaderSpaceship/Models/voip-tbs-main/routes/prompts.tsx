import Navbar from "../islands/Navbar.tsx";
import { Handlers, PageProps } from "$fresh/server.ts";
import { setCookie, getCookies } from "$std/http/cookie.ts";
import Wizard from "../islands/Wizard.tsx";
import { CustomDB } from "$libs/database.ts";
import { IPrompt, IAgent } from "$types/data.ts";
import { authenticateToken } from "$libs/authentication.ts";
import Prompts from "../islands/Prompts.tsx";
import Dropdown from "../islands/Dropdown.tsx";

interface Data {
  prompts: IPrompt[];
  wizard_info: IAgent;
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

    const data = db.prompts.getPrompts(user.id);
    const prompts = data.map((prompt) => {return {input: prompt.input, output: prompt.output, id: prompt.id}});
    const wizard_info = db.agents.getAgent(user.id);
    
    db.close();
    return ctx.render!({ prompts: prompts, wizard_info: wizard_info });
  },
};

export default function PromptsPage({ data }: PageProps<Data>) {
  return (
    <div class="h-screen px-4 py-8 mx-auto bg-dark">
      <div class="max-w-screen-md mx-auto flex flex-col items-center justify-start space-y-2 relative top-[8vh] md:top-[12vh] xl:top-[15vh]">
        <Navbar></Navbar>
        <Dropdown title="Files" items={[{name: "Add File", action: () => {}}, {name: "Link Calendar", action: () => {}}]}/>
        <Prompts prompts={data.prompts}/>
        <svg
          class="w-24 h-24 absolute flex top-[200px] right-[1800px]"
          xmlns="http://www.w3.org/2000/svg"
          fill="none"
          viewBox="0 0 24 24"
          stroke-width="1.5"
          stroke="currentColor"
        >
          <path
            stroke-linecap="round"
            stroke-linejoin="round"
            d="m16.862 4.487 1.687-1.688a1.875 1.875 0 1 1 2.652 2.652L6.832 19.82a4.5 4.5 0 0 1-1.897 1.13l-2.685.8.8-2.685a4.5 4.5 0 0 1 1.13-1.897L16.863 4.487Zm0 0L19.5 7.125"
          />
        </svg>
        <Wizard info={data.wizard_info}/>
      </div>
    </div>
  );
}
