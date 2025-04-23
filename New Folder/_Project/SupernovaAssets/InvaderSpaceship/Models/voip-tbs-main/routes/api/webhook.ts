import { Handlers } from "$fresh/server.ts";
import twilio from "twilio";
import OpenAI from "openai";
import { CustomDB } from "$libs/database.ts";
import { ChatCompletionMessageParam } from "https://jsr.io/@openai/openai/4.82.0/resources/index.ts";

export const handler: Handlers = {
  async POST(req) {
    const db = new CustomDB();

    const url = new URL(req.url);
    const form = await req.formData();

    const api_key = db.twilioKeys.getTwilioKey(url.searchParams.get("key") as string);
    const caller_number = form.get("From") as string;

    if (!api_key) {
      return new Response("Invalid API key", { status: 401 });
    }

    const openaiClient = new OpenAI({apiKey: Deno.env.get("OPENAI_API_KEY") as string});

    const prompts = db.prompts.getPrompts(api_key.user_id);
    const agent = db.agents.getAgent(api_key.user_id);

    const inputs: ChatCompletionMessageParam[] = [
      { role: "system", content: "You are a helpful assistant." },
    ];

    if (agent) {
      inputs.push({ role: "system", content: `Your name is ${agent.agent_name}.` });
      inputs.push({ role: "system", content: `You are a representative for ${agent.business_name}.` });
      inputs.push({ role: "system", content: `Your personality is as follows: ${agent.personality}` });
      inputs.push({ role: "system", content: `Your directions are as follows: ${agent.directives}` });
      if (agent.fallback_contact && agent.fallback_number) {
        inputs.push({ role: "system", content: `If you are unable to assist a customer, direct them to contact ${agent.fallback_contact} at ${agent.fallback_number}.` });
      }
    }

    prompts.forEach((prompt) => {
      inputs.push({ role: "user", content: `EXAMPLE INPUT: "${prompt.input}"` });
      inputs.push({ role: "assistant", content: `EXAMPLE OUTPUT: "${prompt.output}"` });
    });

    const body = form.get("Body") as string;

    if (body) {
      inputs.push({ role: "user", content: body });
      db.messages.insertMessage({ user_id: api_key.user_id, message: body, number_to: api_key.number, number_from: caller_number });
      console.log(`Incoming: Text from ${caller_number}. Received by ${api_key.number}`);
      console.log(`Content: "${body}"`);
      if (body.toLocaleLowerCase() == "reset") {
        db.messages.deleteMessages(api_key.user_id, api_key.number, caller_number);
        inputs.push({ role: "system", content: "You are taking an inquiry. Introduce the business and ask the user what they need."});
        console.log("Resetting conversation");
      }
    } else {
      inputs.push({ role: "system", content: "You are taking an inquiry. Introduce the business and ask the user what they need."});
      console.log(`Incoming: Call from ${caller_number}. Received by ${api_key.number}`);
    }

    const messages = db.messages.getMessagesByNumbers(api_key.user_id, api_key.number, caller_number);

    messages.forEach((message) => {
      if (message.number_to === api_key.number) {
        inputs.push({ role: "user", content: message.message });
      } else {
        inputs.push({ role: "assistant", content: message.message });
      }
    });

    const completion = await openaiClient.chat.completions.create({
      model: "gpt-3.5-turbo",
      messages: inputs,
      max_tokens: 1000,
      temperature: 0.5,
    });

    const message = {
      body: completion.choices[0].message.content as string,
      from: api_key.number,
      to: caller_number,
    };

    const contact = db.contacts.getContactByNumber(api_key.user_id, caller_number);

    if (!contact) {
      db.contacts.insertContact({ user_id: api_key.user_id, number: caller_number });
    }

    db.messages.insertMessage({ user_id: api_key.user_id, message: message.body, number_to: caller_number, number_from: api_key.number });

    console.log(`Response: "${message.body}"`);

    db.close();

    if (body) {
      return new Response(message.body, { status: 200 });
    }

    const twilioClient = twilio(api_key.sid, api_key.auth_token);

    twilioClient.messages.create({
      body: message.body,
      to: caller_number,
      from: api_key.number
    });

    return new Response("Sorry we couldn't get to your call. Please leave a message.", { status: 200 });
  },
};
