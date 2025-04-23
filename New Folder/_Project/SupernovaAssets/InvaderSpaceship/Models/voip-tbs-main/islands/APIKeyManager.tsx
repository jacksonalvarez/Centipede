import APIKeyForm from "./APIKeyForm.tsx"

export default function APIKeyManager() {
  return (
    <div class="w-full flex flex-col items-center justify-center">
      <p class="text-white text-center">You must generate an API key before proceeding.</p>
      <APIKeyForm />
    </div>
  )
}