import HomeIcon from "../components/HomeIcon.tsx";
import CreateAccount from "../islands/CreateAccount.tsx";
import { getCookies } from "$std/http/cookie.ts";
import { Handlers, PageProps } from "$fresh/server.ts";

interface Data {
  auth_state: string;
}

export const handler: Handlers = {
  GET(req, ctx) {
    const cookies = getCookies(req.headers);
    return ctx.render({ auth_state: cookies.auth });
  }
}

export default function SignupPage({ data }: PageProps<Data>) {
  return (
    <div class="h-screen bg-dark flex items-center justify-start">
      <div class="h-screen py-8 flex flex-col mx-auto items-center justify-start space-y-6">
        <HomeIcon size={128} />
        <h1 class="text-5xl font-bold text-white mb-8">Create an Account</h1>
        <CreateAccount />
        <SignupData data={data} />
      </div>
    </div>
  );
}

function SignupData({ data }: PageProps<Data>) {
  if (data.auth_state === "invalid") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">Invalid information submitted</p>
      </div>
    );
  }
  return null;
}