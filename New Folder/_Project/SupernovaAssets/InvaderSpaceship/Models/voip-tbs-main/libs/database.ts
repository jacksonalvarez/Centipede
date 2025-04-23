import { Database } from "jsr:@db/sqlite";
import { UserHandler } from "$libs/database/user.ts";
import { SessionHandler } from "$libs/database/session.ts";
import { PromptHandler } from "$libs/database/prompt.ts";
import { MessageHandler } from "$libs/database/message.ts";
import { ContactHandler } from "$libs/database/contact.ts";
import { TwilioApiKeyHandler } from "$libs/database/twilioapikey.ts";
import { AgentHandler } from "$libs/database/agent.ts";
import { CalendarEventHandler } from "$libs/database/calendarevent.ts";

export class CustomDB {
  private dbPath: string;
  private db: Database;
  public users: UserHandler;
  public sessions: SessionHandler;
  public prompts: PromptHandler;  
  public messages: MessageHandler;
  public contacts: ContactHandler;
  public twilioKeys: TwilioApiKeyHandler;
  public agents: AgentHandler;
  public calendarEvents: CalendarEventHandler;

  constructor() {
    this.dbPath = "main.sqlite";
    this.db = new Database(this.dbPath);
    this.users = new UserHandler(this.db);
    this.sessions = new SessionHandler(this.db);
    this.prompts = new PromptHandler(this.db);
    this.messages = new MessageHandler(this.db);
    this.contacts = new ContactHandler(this.db);
    this.twilioKeys = new TwilioApiKeyHandler(this.db);
    this.agents = new AgentHandler(this.db);
    this.calendarEvents = new CalendarEventHandler(this.db);
  }

  public close(): void {
    this.db.close();
  }
}
