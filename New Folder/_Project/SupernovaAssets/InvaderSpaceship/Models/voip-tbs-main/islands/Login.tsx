import LoginButton from "../components/LoginButton.tsx";

export default function Login() {
    return (
        <div class="flex flex-col items-center justify-center w-96 h-96">
            <div class="flex flex-col items-center justify-center space-y-6 bg-darker text-white p-8 rounded-lg w-full">
                <h1 class="font-bold text-2xl">Sign in to your account</h1>
                <form class="space-y-6 w-full" action="/api/login" method="POST">
                    <div class="flex items-center bg-gray-600 text-white rounded-full p-3 w-full">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke-width="1.5"
                            stroke="currentColor"
                            class="w-6 h-6"
                        >
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z"
                            />
                        </svg>
                        <input
                            type="text"
                            name="username"
                            id="username"
                            class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                            placeholder="Username"
                        />
                    </div>

                    <div class="flex items-center bg-gray-600 text-white rounded-full p-3 w-full">
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            fill="none"
                            viewBox="0 0 24 24"
                            stroke-width="1.5"
                            stroke="currentColor"
                            class="w-6 h-6"
                        >
                            <path
                                stroke-linecap="round"
                                stroke-linejoin="round"
                                d="M16.5 10.5V6.75a4.5 4.5 0 1 0-9 0v3.75m-.75 11.25h10.5a2.25 2.25 0 0 0 2.25-2.25v-6.75a2.25 2.25 0 0 0-2.25-2.25H6.75a2.25 2.25 0 0 0-2.25 2.25v6.75a2.25 2.25 0 0 0 2.25 2.25Z"
                            />
                        </svg>
                        <input
                            type="password"
                            name="password"
                            id="password"
                            class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                            placeholder="Password"
                        />
                    </div>

                    <LoginButton />
                </form>
            </div>
        </div>
    );
}
