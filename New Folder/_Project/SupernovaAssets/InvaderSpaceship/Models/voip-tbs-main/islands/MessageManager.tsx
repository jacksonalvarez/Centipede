import { useState } from "preact/hooks";
import { IMessage, IContact } from "$types/data.ts";
import Messages from "./Messages.tsx";
import Contacts from "./Contacts.tsx";

interface Props {
  messages: IMessage[];
  contacts: IContact[];
  apikey: string;
}

export default function MessageManager(props: Props) {
  const [messages, setMessages] = useState(props.messages);
  const [contacts, setContacts] = useState(props.contacts);
  const [selected, setSelected] = useState("");

  const filter = (number: string) => {
    return messages.filter((message) => message.number_from === number || message.number_to === number);
  }

  const sort = (messages: IMessage[]) => {
    return messages.sort((a, b) => a.unix_timestamp - b.unix_timestamp);
  }

  return (
    <div class="flex flex-row h-full w-full justify-start">
      <div>
        <p class="text-white text-center">Please set your Twilio webhook to:</p>
        <p class="text-white text-center">{`https://gatewaycorporate.org/api/webhook?key=${props.apikey}`}</p>
        <p class="text-red-500 text-center">Keep this key private. Do not share it with anyone.</p>
      </div>

      <Contacts contacts={contacts} hook={setSelected} />

      <div class="flex flex-col-reverse items-center justify-start fixed w-full h-full">

        <div class="flex flex-row items-center justify-start bg-darker w-[40vw] sm:w-[60vw] xl:w-[80vw] h-5vh fixed right-0 bottom-0">
          <span class="p-2 m-2 w-full bg-dark rounded-full">
            <p class="px-2 text-white text-lg">Send a message</p>
          </span>
        </div>

        <Messages messages={sort(filter(selected))} />

      </div>
    </div>
  )
}