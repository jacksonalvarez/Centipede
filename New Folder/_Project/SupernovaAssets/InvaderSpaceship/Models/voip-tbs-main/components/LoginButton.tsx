import { Button } from "./Button.tsx";

export default function LoginButton() {
  return (
    <div class="w-full">
      <Button className="w-full py-3 rounded-full bg-red-500 text-white hover:bg-red-600">
        Log in
      </Button>
    </div>
  );
}