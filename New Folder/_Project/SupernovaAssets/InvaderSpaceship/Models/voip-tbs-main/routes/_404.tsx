import { Head } from "$fresh/runtime.ts";
import HomeIcon from "../components/HomeIcon.tsx";

export default function Error404() {
  return (
    <>
      <Head>
        <title>404 - Page not found</title>
      </Head>
      <div class="px-4 py-2 mx-auto h-screen bg-dark">
        <div class="max-w-screen-md mx-auto flex flex-col items-center justify-center">
          <HomeIcon />
          <h1 class="text-4xl font-bold text-white">404 - Page not found</h1>
          <p class="my-4 text-white">
            The page you were looking for doesn't exist.
          </p>
          <a href="/" class="underline text-blue-400">Go back home</a>
        </div>
      </div>
    </>
  );
}
