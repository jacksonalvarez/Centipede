import HomeIcon from "../components/HomeIcon.tsx";

export default function Navbar() {
  return (
    <nav class="bg-transparent text-white border-b bg-darker">
      <div class="flex flex-row fixed top-0 left-0 w-screen">
        <div class="mx-2 h-16 md:h-20 xl:h-28">
          <HomeIcon size={0} />
        </div>
        <div class="flex flex-row h-16 md:h-20 xl:h-28">
          <a
            href="/analytics"
            class="px-2 sm:p-4 md:px-6 lg:px-8 xl:px-12 py-3 w-[20vw] h-full text-lg sm:text-xl md:text-2xl xl:text-4xl border-r-4 border-b-4 outline-black border-r-darker border-b-darkest font-bold flex text-center items-center justify-center sm:justify-start text-white bg-darker bg-opacity-50 rounded hover:bg-opacity-75"
          >
            Analytics
          </a>
          <a
            href="/prompts"
            class="px-2 sm:p-4 md:px-6 lg:px-8 xl:px-12 py-3 w-[20vw] h-full text-lg sm:text-xl md:text-2xl xl:text-4xl border-r-4 border-b-4 border-r-darker border-b-darkest font-bold flex text-center items-center justify-center sm:justify-start text-white bg-darker bg-opacity-50 rounded hover:bg-opacity-75"
          >
            Prompts
          </a>
          <a
            href="/messages"
            class="px-2 sm:p-4 md:px-6 lg:px-8 xl:px-12 py-3 w-[20vw] h-full text-lg sm:text-xl md:text-2xl xl:text-4xl border-r-4 border-b-4 border-r-darker border-b-darkest font-bold flex text-center items-center justify-center sm:justify-start text-white bg-darker bg-opacity-50 rounded hover:bg-opacity-75"
          >
            Messages
          </a>
        </div>
        <div class="ml-auto">
          <a
            href="/api/logout"
            class="px-4 md:px-8 xl:px-12 py-3 text-lg flex fixed right-0 md:right-2 xl:right-4 top-0 md:top-2 xl:top-4 items-center border-2 border-solid border-red-600 text-red-600 font-bold bg-darker bg-opacity-45 rounded hover:bg-red-700"
          >
            Logout
          </a>
        </div>
      </div>
    </nav>
  );
}
