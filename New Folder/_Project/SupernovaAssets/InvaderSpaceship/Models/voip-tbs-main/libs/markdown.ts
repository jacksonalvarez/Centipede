export const renderMarkdown = async(page: string = "default") => {
  try {
    let content = await Deno.readTextFile(Deno.cwd() + `/routes/blog/${page}.md`);

    content = content.replace(/## (.*)/g, "<h2 class=\"text-3xl font-bold p-2\">$1</h2>");
    content = content.replace(/# (.*)/g, "<h1 class=\"text-5xl font-bold p-2\">$1</h1>");
    content = content.replace(/\/\/ (.*)/g, "<span>$1</span>");
    content = content.replace(/!\[(.*)\]\((.*) \"(.*)\"\)/g, "<img class=\"items-center justify-center p-2 shadow-md sm:max-w-screen md:max-w-[80vw] xl:max-w-[60vw]\" src=\"$2\" alt=\"$1\" title=\"$3\">");
    content = content.replace(/\[(.*)\]\((.*)\)/g, "<a href=\"$2\" class=\"text-blue-400\" style=\"display:inline;\">$1</a>");

    return content;
  } catch (_e) {
    return "404";
  }
}