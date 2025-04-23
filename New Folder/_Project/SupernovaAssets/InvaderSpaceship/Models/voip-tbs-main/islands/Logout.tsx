import { Button } from "../components/Button.tsx";
import ky from "ky";

export default function Logout() {
    return (
        <Button onClick={() => { ky.get("/api/logout") }}>Logout</Button>
    )
}
