import { IContact } from '$types/data.ts';
import Contact from '../components/Contact.tsx';

interface Props {
  contacts: IContact[];
  hook: (number: string) => void;
}

export default function Contacts(props: Props) {
  return (
    <div class="flex flex-col fixed left-0 h-full items-center justify-start">
      <div class="bg-darker border-r-darkest border-r-4 border-t-darker border-t-4 w-[60vw] sm:w-[40vw] xl:w-[20vw] h-full bg-opacity-50">
        {props.contacts.map((contact) => (
          <Contact key={contact.id} name={contact.name} number={contact.number} hook={props.hook}/>
        ))}
      </div>
    </div>
  )
}