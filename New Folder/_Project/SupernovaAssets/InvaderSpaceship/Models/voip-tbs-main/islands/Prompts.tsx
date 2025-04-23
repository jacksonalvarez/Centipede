import Prompt from "../components/Prompt.tsx";
import { useState } from "preact/hooks";
import { Optional, IPrompt } from "$types/data.ts";
import ky from "ky";

interface Props {
  prompts: Optional<IPrompt, "user_id">[];
}

export default function Prompts(props: Props) {
  const [prompts, setPrompts] = useState(props.prompts);
  const addPrompt = () => {
    const r = ky.post("/api/prompts", { json: { input: "", output: "" } });
    r.then((res) => {
      if (res.ok) {
        res.json().then((data) => {
          setPrompts((prevPrompts) => {
            return [...prevPrompts, { input: "", output: "", id: data.id }];
          });
        });
        return;
      }
      throw new Error("Error adding prompt");
    });
  };

  const savePrompt = (id: number, input: string, output: string) => {
    setPrompts((prevPrompts) => {
      return prevPrompts.map((p) => {
        if (p.id === id) {
          p.input = input;
          p.output = output;
        }
        return p;
      });
    });
  };

  return (
    <div class="flex flex-col items-center justify-center space-y-2">
      <iframe name="hidden-iframe" id="hidden-iframe" class="absolute invisible"></iframe>
      <div class="flex flex-row justify-center space-x-4">
        <a class="px-2 sm:px-8 py-3 w-[40vw] md:h-20 xl:h-28 text-xl md:text-2xl xl:text-4xl border-r-4 border-b-4 border-r-darker border-b-darkest font-bold flex text-center items-center justify-center sm:justify-start text-white bg-darker bg-opacity-50 rounded hover:bg-opacity-75">
          Input
        </a>
        <a
          href="/prompts"
          class="px-2 sm:px-8 py-3 w-[40vw] md:h-20 xl:h-28 text-xl md:text-2xl xl:text-4xl border-r-4 border-b-4 border-r-darker border-b-darkest font-bold flex text-center items-center justify-center sm:justify-start text-white bg-darker bg-opacity-50 rounded hover:bg-opacity-75"
        >
          Output
        </a>
      </div>
      <div class="max-h-[50vh] overflow-y-scroll space-y-2 px-4 [&::-webkit-scrollbar]:w-2 [&::-webkit-scrollbar-track]:bg-dark [&::-webkit-scrollbar-thumb]:bg-darker">
        {
          prompts.map((prompt) => { return <Prompt id={prompt.id} input={prompt.input} output={prompt.output} editable={false} hook={savePrompt} />; })
        }
      </div>
      <button
        class="size-10 border-r-4 border-b-4 border-r-darker border-b-darkest bg-darker items-center justify-center font-bold text-xl md:text-2xl text-white"
        onClick={addPrompt}
      >
        +
      </button>
    </div>
  )
}