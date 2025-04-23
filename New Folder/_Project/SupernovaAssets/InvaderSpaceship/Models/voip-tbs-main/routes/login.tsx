import type { Handlers, PageProps } from "$fresh/server.ts";
import { getCookies } from "$std/http/cookie.ts";
import HomeIcon from "../components/HomeIcon.tsx";
import Login from "../islands/Login.tsx";
import { CustomDB } from "$libs/database.ts";

interface Data {
  auth_state: string;
}

export const handler: Handlers = {
  GET(req, ctx) {
    const db = new CustomDB();

    const cookies = getCookies(req.headers);

    if (!cookies.token) {
      db.close();
      return ctx.render!({ auth_state: cookies.auth });
    }

    const session = db.sessions.getSession(cookies.token);

    if (!session) {
      db.close();
      return ctx.render!({ auth_state: cookies.auth });
    }

    const headers = new Headers();
    headers.set("location", "/analytics");
    return new Response(null, { status: 303, headers });
  },
};

export default function LoginPage({ data }: PageProps<Data>) {
  return (
    <div class="h-screen mx-auto bg-dark">
      <div class="max-w-screen-md py-8 mx-auto flex flex-col items-center justify-start h-full">
        <HomeIcon size={128} />
        <h1 class="text-5xl font-bold text-white mt-6">
          Welcome to Gateway Corporate
        </h1>
        <Login />
        <p class="mt-4">
          <LoginData data={data} />
        </p>
      </div>
    </div>
  );
}

function LoginData({ data }: PageProps<Data>) {
  if (data.auth_state === "infomissing") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">Please fill in all fields</p>
        <a href="/signup" class="text-blue-400 underline">Create an account</a>
      </div>
    );
  }

  if (data.auth_state === "userexists") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">User already exists</p>
        <a href="/signup" class="text-blue-400 underline">Create an account</a>
      </div>
    );
  }

  if (data.auth_state === "expired") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">Your session has expired</p>
      </div>
    );
  }

  if (data.auth_state === "unauthorized") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">You are not authorized to view this page</p>
      </div>
    );
  }

  if (data.auth_state === "invalid") {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-red-500">Invalid username or password</p>
        <p class="text-white">
          Would you like to try{" "}
          <a class="text-blue-400 underline" href="/reset-password">
            resetting your password?
          </a>
        </p>
        <a href="/signup" class="text-blue-400 underline">Create an account</a>
      </div>
    );
  } else {
    return (
      <div class="flex flex-col items-center justify-center text-center">
        <p class="text-white">
          {data.auth_state === "valid"
            ? "You are logged in"
            : "Don't have an account?"}
        </p>
        <a href="/signup" class="text-blue-400 underline">
          {data.auth_state === "valid" ? "" : "Create an account"}
        </a>
      </div>
    );
  }
}
