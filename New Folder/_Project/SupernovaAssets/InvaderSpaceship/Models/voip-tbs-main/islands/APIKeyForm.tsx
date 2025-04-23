export default function APIKeyForm() {
    return (
      <div class="fixed top-0 left-0 h-screen w-screen bg-darker bg-opacity-50 z-10">
        <form
          class="fixed inset-x-[30vw] inset-y-[30vh] bg-darker rounded-xl text-white flex flex-col items-center justify-center p-4 z-20"
          action="/api/register-key"
          method="POST"
        >
          <h1 class="text-red-500 text-3xl font-bold mb-6">Please register for an API key</h1>
          <div class="w-full max-w-2xl space-y-4">
            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
              <input
                type="text"
                name="sid"
                id="sid"
                class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                placeholder="Twilio SID"
                required
              />
            </div>
            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
              <input
                type="password"
                name="auth-token"
                id="auth-token"
                class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                placeholder="Twilio Auth Token"
                required
              />
            </div>
            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
              <input
                type="text"
                name="phone-number"
                id="phone-number"
                class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                placeholder="Phone Number"
                required
              />
            </div>
            <div class="flex items-center justify-center">
              <button class="bg-red-500 text-white px-4 py-2 rounded-full font-bold" type="submit">Submit</button>
            </div>
          </div>
        </form>
      </div>
    );
  }
  