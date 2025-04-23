import { useState } from "preact/hooks";

interface Props {
  title: string;
  items: {
    name: string;
    action: () => void;
  }[];
}

export default function Dropdown(props: Props) {
  const [dropdownOpen, setDropdownOpen] = useState(false);

  return (
    <div class="relative flex flex-row-reverse w-[80vw] items-start">
      <button
        onClick={() => setDropdownOpen(!dropdownOpen)}
        class="relative right-0 z-10 block text-white bg-darker rounded-full px-6 py-3 text-sm focus:outline-none"
      >
        {props.title}
        <svg
          class="h-5 w-5 inline"
          fill="currentColor"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 20 20"
        >
          <path
            d="M5.293 7.293a1 1 0 011.414 0L10 
              10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
            clip-rule="evenodd"
          />
        </svg>
      </button>

      {dropdownOpen && (
        <div class="absolute right-0 top-10 mt-2 w-48 rounded-md shadow-xl z-20">
          {
            props.items.map((item) => {
              return (
                <button class="block w-full px-4 py-2 text-sm bg-dark hover:bg-darker border-2 border-darker text-white" onClick={item.action}>
                  {item.name}
                </button>
              );
            })
          }
        </div>
      )}
    </div>
  );
}
