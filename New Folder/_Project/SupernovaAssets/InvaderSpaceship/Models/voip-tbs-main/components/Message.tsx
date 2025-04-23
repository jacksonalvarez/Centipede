import { IMessage } from "$types/data.ts";

interface Props {
  message: IMessage;
}

export default function Message({ message }: Props) {
  return (
    <div class="flex flex-row items-center justify-start w-[40vw] bg-dark odd:bg-darker sm:w-[60vw] xl:w-[80vw] h-5vh fixed right-0 bottom-0" id={message.id.toString()}>
      <span class="p-2 m-2 w-full rounded-full">
        <p class="px-2 text-white text-lg">{message.message}</p>
      </span>
    </div>
  )
}