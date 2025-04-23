import type { Handlers, PageProps } from "$fresh/server.ts"
import { getCookies } from "$std/http/cookie.ts";
import HomeIcon from "../components/HomeIcon.tsx";

interface Data {
  auth_state: string;
}

export const handler: Handlers = {
  GET(req, ctx) {
    const cookies = getCookies(req.headers);
    return ctx.render!({ auth_state: cookies.auth });
  },
}

export default function HomePage({ data }: PageProps<Data>) {
  return (
    <div class="h-screen mx-auto bg-dark">
      <div class="py-8 max-w-screen-md mx-auto flex flex-col items-center justify-start h-full">
        <HomeIcon size={128} />
        <h1 class="text-5xl font-bold text-white mt-6">
          Welcome to Gateway Corporate
        </h1>
        <h2 class="text-3xl font-bold text-white mt-4">
          AI call automation made simple.
        </h2>
        <h4 class="text-lg text-white mt-4">
          Read our <a href="/blog" class="text-blue-400 underline" style="display:inline;">blog</a> to learn more.
        </h4>
        <h4 class="text-lg text-white mt-4">
          <i>Schedule a demo at +1-314-970-4177</i>
        </h4>
        <LoginData data={data} />
      </div>
    </div>
  );
}


function LoginData({ data }: PageProps<Data>) {
  if (data.auth_state === "valid") {
    return (
      <div class="flex flex-col items-center justify-center text-center mt-4">
        <a href="/analytics">
          <button class="w-full py-3 px-6 rounded-full bg-blue-400 text-white hover:bg-red-600 mb-4">
            Go to Analytics
          </button>
        </a>
        <p class="text-white">You are already logged in.</p>
      </div>
    );
  } else {
    return (
      <div class="flex flex-col items-center justify-center text-center mt-4">
        <a href="/login">
          <button class="w-full py-3 px-6 rounded-full bg-red-500 text-white hover:bg-red-600 mb-4">
            Log in
          </button>
        </a>
        <p class="text-white">Don't have an account?</p>
        <a href="/signup" class="text-blue-400 underline">Create an account</a>
      </div>
    );
  }
}
