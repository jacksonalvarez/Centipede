# Linking Your Number

## Connecting to Twilio

// One of the first things you will want to do after making an account with Gateway Corporate is to link your [Twilio](https://twilio.com) number. This will allow your virtual assistant to receive calls and texts, respond to end users, and set events in your calendar for you.
// Until you link your account on Gateway's [messages page](https://gatewaycorporate.org/messages), you will see a prompt appearing something like this.

![API Form](https://gatewaycorporate.org/blog/api-form.png "API Form")

// To complete the form, navigate to your [Twilio console](https://console.twilio.com) and copy the relevant information at the bottom of the page over to your Gateway Corporate account and click submit.

![Twilio Info](https://gatewaycorporate.org/blog/twilio-info.png "Twilio Info")

// After the form is submitted and the page refreshes, you will be granted an API webhook of the form https://gatewaycorporate.org/api/webhook?key=[webhook_key]. To use this webhook, copy and paste it into the configuration page for the Twilio number you previously entered. Make sure that the hook is used for both calls and messages for proper end user interaction.

![Twilio Config](https://gatewaycorporate.org/blog/twilio-config.png "Twilio Config")

// Your number is now set up for receiving voice calls! Further configuration may be required through Twilio for messaging in the U.S.