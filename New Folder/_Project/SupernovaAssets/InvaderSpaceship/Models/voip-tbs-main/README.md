# Gateway Corporate (Running on Fresh)

### Usage

Make sure to install Deno: https://deno.land/manual/getting_started/installation

To start the webserver only:

```
deno task start
```

This will watch the project directory and restart as necessary.

Environment variables for Docker containers are set in .env.list

Azure environment variables (COMMUNICATION_SERVICES_ENDPOINT and COMMUNICATION_SERVICES_ACCESS_KEY) are required to be set before verification emails can be sent.

An OpenAI API key is required to be set before messages can be sent to end users (OPENAI_API_KEY)

To start all services (including nginx reverse proxy):

```
docker compose up --build
```
