import { useState } from "preact/hooks";
import ky from "ky";

interface Props {
  id: number;
  input: string;
  output: string;
  editable: boolean;
  hook: (id: number, input: string, output: string) => void;
}

export default function Prompt(props: Props) {
  const [deleted, setDeleted] = useState(false);
  const [editing, setEditing] = useState(false);
  const deletePrompt = () => { setDeleted(true); ky.delete("/api/prompts", {json: {id: props.id}}); };
  const editPrompt = () => { setEditing(true); };
  const savePrompt = () => {
    const prompt = document.querySelector(`#prompt-${props.id}`) as HTMLFormElement;
    props.input = (prompt.querySelector(`input[name="input"]`) as HTMLInputElement).value;
    props.output = (prompt.querySelector(`input[name="output"]`) as HTMLInputElement).value;
    setEditing(false);
    ky.put("/api/prompts", {json: {id: props.id, input: props.input, output: props.output}});
    props.hook(props.id, props.input, props.output);
  };

  if (deleted) {
    return null;
  }

  return (
    <form action="#" method="GET" target="hidden-iframe" class="prompt space-x-4 items-center justify-center flex flex-row" id={`prompt-${props.id}`}>
      <span class="px-4 py-3 md:h-16 xl:h-20 border-r-4 border-b-4 w-[35vw] border-r-darker border-b-darkest font-bold flex items-center bg-darker bg-opacity-50 rounded hover:bg-opacity-75">
        <input type="text" name="input" readOnly={!editing} value={props.input} class="bg-transparent text-white text-lg md:text-xl xl:text-2xl w-full" />
      </span>
      <span class="px-4 py-3 md:h-16 xl:h-20 border-r-4 border-b-4 w-[35vw] border-r-darker border-b-darkest font-bold flex items-center bg-darker bg-opacity-50 rounded hover:bg-opacity-75">
        <input type="text" name="output" readOnly={!editing} value={props.output} class="bg-transparent text-white text-lg md:text-xl xl:text-2xl w-full" />
      </span>
      <div class="flex flex-row space-x-2">
        <button onClick={editPrompt} class={"text-yellow-500 " + (editing ? "invisible" : "")}>
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="size-6">
            <path d="M21.731 2.269a2.625 2.625 0 0 0-3.712 0l-1.157 1.157 3.712 3.712 1.157-1.157a2.625 2.625 0 0 0 0-3.712ZM19.513 8.199l-3.712-3.712-12.15 12.15a5.25 5.25 0 0 0-1.32 2.214l-.8 2.685a.75.75 0 0 0 .933.933l2.685-.8a5.25 5.25 0 0 0 2.214-1.32L19.513 8.2Z" />
          </svg>
        </button>
        <button onClick={savePrompt} class={"text-green-500  " + (editing ? "" : "invisible")}>
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="size-6">
            <path fill-rule="evenodd" d="M7.502 6h7.128A3.375 3.375 0 0 1 18 9.375v9.375a3 3 0 0 0 3-3V6.108c0-1.505-1.125-2.811-2.664-2.94a48.972 48.972 0 0 0-.673-.05A3 3 0 0 0 15 1.5h-1.5a3 3 0 0 0-2.663 1.618c-.225.015-.45.032-.673.05C8.662 3.295 7.554 4.542 7.502 6ZM13.5 3A1.5 1.5 0 0 0 12 4.5h4.5A1.5 1.5 0 0 0 15 3h-1.5Z" clip-rule="evenodd" />
            <path fill-rule="evenodd" d="M3 9.375C3 8.339 3.84 7.5 4.875 7.5h9.75c1.036 0 1.875.84 1.875 1.875v11.25c0 1.035-.84 1.875-1.875 1.875h-9.75A1.875 1.875 0 0 1 3 20.625V9.375Zm9.586 4.594a.75.75 0 0 0-1.172-.938l-2.476 3.096-.908-.907a.75.75 0 0 0-1.06 1.06l1.5 1.5a.75.75 0 0 0 1.116-.062l3-3.75Z" clip-rule="evenodd" />
          </svg>
        </button>
        <button onClick={deletePrompt} class="text-red-500">
          <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" class="size-6">
            <path fill-rule="evenodd" d="M16.5 4.478v.227a48.816 48.816 0 0 1 3.878.512.75.75 0 1 1-.256 1.478l-.209-.035-1.005 13.07a3 3 0 0 1-2.991 2.77H8.084a3 3 0 0 1-2.991-2.77L4.087 6.66l-.209.035a.75.75 0 0 1-.256-1.478A48.567 48.567 0 0 1 7.5 4.705v-.227c0-1.564 1.213-2.9 2.816-2.951a52.662 52.662 0 0 1 3.369 0c1.603.051 2.815 1.387 2.815 2.951Zm-6.136-1.452a51.196 51.196 0 0 1 3.273 0C14.39 3.05 15 3.684 15 4.478v.113a49.488 49.488 0 0 0-6 0v-.113c0-.794.609-1.428 1.364-1.452Zm-.355 5.945a.75.75 0 1 0-1.5.058l.347 9a.75.75 0 1 0 1.499-.058l-.346-9Zm5.48.058a.75.75 0 1 0-1.498-.058l-.347 9a.75.75 0 0 0 1.5.058l.345-9Z" clip-rule="evenodd" />
          </svg>
        </button>
      </div>
    </form>
  );
}
