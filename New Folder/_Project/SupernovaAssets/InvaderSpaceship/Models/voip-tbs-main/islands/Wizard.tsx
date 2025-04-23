import { useState } from "preact/hooks";
import { IAgent } from "$types/data.ts";

interface Props {
    info: IAgent | null;
}

export default function Wizard(props: Props) {
    const [isOpen, setIsOpen] = useState(false);
    const toggleSidebar = () => setIsOpen(!isOpen);

    return (
        <div>
            <div class={`bg-darker font-sans font-bold fixed z-20 top-0 bottom-0 left-0 transition-transform duration-300 ${isOpen ? "translate-x-0" : "-translate-x-full"}`}>
                <div class="sidebar p-2 xl:w-[20vw] overflow-y-auto text-center bg-darker">
                    <div class="text-white text-xl">
                        <div class="p-2.5 mt-1 flex items-center justify-between">
                            <h1 class="font-bold text-gray-200 text-xl ml-3">
                                Setup Wizard
                            </h1>
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke-width="1.5"
                                stroke="currentColor"
                                class="cursor-pointer hover:bg-red-500 size-10"
                                onClick={toggleSidebar}
                            >
                                <path
                                    stroke-linecap="round"
                                    stroke-linejoin="round"
                                    d="M6 18 18 6M6 6l12 12"
                                />
                            </svg>
                        </div>
                    </div>
                    <form action="/api/wizard" method="POST">
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <input
                                type="text"
                                name="character-name"
                                placeholder="Character Name"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none"
                                value={props.info ? props.info.agent_name : ""}
                                required
                            ></input>
                        </div>
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <input
                                type="text"
                                name="business-name"
                                placeholder="Business Name"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none"
                                value={props.info ? props.info.business_name : ""}
                                required
                            ></input>
                        </div>
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <textarea
                                name="personality"
                                placeholder="Personality Description"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-track]:bg-dark [&::-webkit-scrollbar-thumb]:bg-darker"
                                rows={6}
                                value={props.info? props.info.personality : ""}
                                required
                            ></textarea>
                        </div>
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <textarea
                                name="directives"
                                placeholder="Directives / Instructions"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-track]:bg-dark [&::-webkit-scrollbar-thumb]:bg-darker"
                                rows={8}
                                value={props.info ? props.info.directives : ""}
                                required
                            ></textarea>
                        </div>
                        <p class="mt-3 text-white">Optional Inputs</p>
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <input
                                type="text"
                                name="fallback-name"
                                placeholder="Emergency Contact Name"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none"
                                value={props.info && props.info.fallback_contact ? props.info.fallback_contact : ""}
                            ></input>
                        </div>
                        <div class="p-2.5 mt-3 flex items-center rounded-md px-2 duration-300 cursor-pointer bg-dark text-white">
                            <input
                                type="text"
                                name="fallback-number"
                                placeholder="Emergency Contact Number"
                                class="text-[15px] ml-2 w-full bg-transparent focus:outline-none resize-none"
                                value={props.info && props.info.fallback_number ? props.info.fallback_number : ""}
                            ></input>
                        </div>
                        <div class="flex fixed w-full bottom-0 items-center justify-center py-2 pl-2 pr-4 rounded-sm" title="⚠️ Checking this box will regenerate all custom prompts">
                            <input id="regenerate" type="checkbox" value="" name="regenerate"
                                class="w-4 h-4 text-blue-600 bg-gray-100 border-gray-300 rounded-sm focus:ring-blue-500 dark:focus:ring-blue-600 dark:ring-offset-gray-800 focus:ring-2 dark:bg-gray-700 dark:border-gray-600"
                            >
                            </input>
                            <label for="regenerate" class="w-full py-4 ms-2 text-sm text-left font-medium text-gray-900 dark:text-gray-300">
                                Regenerate Prompts
                            </label>
                            <button
                                type="submit"
                                class="p-2.5 mt-3 bg-blue-900 text-white rounded-md w-full duration-300 cursor-pointer hover:bg-blue-950"
                            >Submit</button>
                        </div>
                    </form>
                </div>
            </div>
            {!isOpen && (
                <button
                    onClick={toggleSidebar}
                    class="fixed top-[70px] md:top-[100px] xl:top-[150px] left-2 transform -translate-x-1/2 bg-darker border-y-2 border-r-2 border-darkest text-white p-2 rounded-r-md hover:bg-dark"
                >
                    &gt;
                </button>
            )}
        </div>
    );
}
