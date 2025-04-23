import { Button } from "../components/Button.tsx";

export default function CreateAccount() {
    return (
        <div class="h-screen px-4 py-2 mx-auto">
            <div class="max-w-screen-md mx-auto flex flex-col items-center justify-center">
                <div class="flex flex-col items-center justify-center w-96">
                    <div class="flex flex-col items-center justify-center space-y-6 bg-darker text-white p-8 rounded-lg w-full">
                        <form class="space-y-6 w-full" action="/api/signup" method="POST">
                            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
                                <input
                                    type="email"
                                    name="email"
                                    id="email"
                                    class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                                    placeholder="Email"
                                    required
                                    onInput={() => {
                                        const email = document.querySelector("input[name='email']") as HTMLInputElement;
                                        if (!email.value.match(/^[^\s@]+@[^\s@]+\.[^\s@]+$/) || email.value.length > 254) {
                                            email.setCustomValidity("Not a valid email");
                                        } else {
                                            email.setCustomValidity("");
                                        }
                                    }}
                                />
                            </div>

                            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
                                <input
                                    type="text"
                                    name="username"
                                    id="username"
                                    class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                                    placeholder="Username"
                                    required
                                    onInput={() => {
                                        const username = document.querySelector("input[name='username']") as HTMLInputElement;
                                        if (!username.value.match(/^[a-zA-Z0-9_]{3,16}$/)) {
                                            username.setCustomValidity("Usernames must be between 3 and 16 characters long and contain only letters, numbers, and underscores");
                                        } else {
                                            username.setCustomValidity("");
                                        }
                                    }}
                                />
                            </div>
                            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
                                <input
                                    type="password"
                                    name="password"
                                    id="password"
                                    class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                                    placeholder="Password"
                                    required
                                    onInput={() => {
                                        const password = document.querySelector("input[name='password']") as HTMLInputElement;
                                        if (!password.value.match(/^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}/)) {
                                            password.setCustomValidity("Passwords must be at least 8 characters long and contain at least one letter, one number, and one special character");
                                        } else {
                                            password.setCustomValidity("");
                                        }
                                    }}
                                />
                            </div>
                            <div class="flex items-center bg-dark text-white rounded-full p-3 w-full">
                                <input
                                    type="password"
                                    name="password-check"
                                    id="password-check"
                                    class="bg-transparent flex-1 text-white placeholder-gray-400 focus:outline-none pl-3"
                                    placeholder="Confirm Password"
                                    required
                                    onInput={() => {
                                        const password = document.querySelector("input[name='password']") as HTMLInputElement;
                                        const passwordCheck = document.querySelector("input[name='password-check']") as HTMLInputElement;
                                        if (password.value !== passwordCheck.value) {
                                            passwordCheck.setCustomValidity("Passwords do not match");
                                        } else {
                                            passwordCheck.setCustomValidity("");
                                        }
                                    }}
                                />
                            </div>
                            <div class="w-full">
                                <Button className="w-full py-3 rounded-full bg-red-500 text-white hover:bg-red-600">
                                    Sign Up
                                </Button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}
