import { Handlers, PageProps } from "$fresh/server.ts";
import { renderMarkdown } from "$libs/markdown.ts";
import HomeIcon from "../../components/HomeIcon.tsx";

interface Data {
  blog: string;
}

export const handler: Handlers = {
  async GET(_req, ctx) {
    const content = await renderMarkdown();
    return ctx.render!({blog: content});
  }
}

export default function BlogPage({ data }: PageProps<Data>) {
  return (
    <div class="flex flex-col w-screen items-center justify-center">
      <HomeIcon size={128} />
      <div class="flex flex-col text-white text-center items-center justify-center w-[80vw] h-full" dangerouslySetInnerHTML={{__html: data.blog}}></div>
    </div>
  )
}