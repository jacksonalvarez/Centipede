import { IMessage } from "$types/data.ts";
import Message from "../components/Message.tsx";

interface Props {
  messages: IMessage[];
}

export default function Messages({ messages }: Props) {
  return (
    <div class="flex flex-col-reverse items-center justify-start fixed w-full h-full">
      <div class="flex flex-row items-center justify-start bg-darker w-[40vw] sm:w-[60vw] xl:w-[80vw] h-5vh fixed right-0 bottom-0">
        <span class="p-2 m-2 w-full bg-dark rounded-full">
          <p class="px-2 text-white text-lg">Send a message</p>
        </span>
      </div>

      {
        messages.map((message) => {
          return (
            <Message message={message} />
          )
        })
      }
    </div>
  )
}