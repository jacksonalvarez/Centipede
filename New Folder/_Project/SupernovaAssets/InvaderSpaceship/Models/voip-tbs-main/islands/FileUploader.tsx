import { h } from "preact";

export default function FileUploader() {
  return (
    <div class="flex items-center justify-center min-h-screen bg-gradient-to-br from-purple-500 to-orange-300">
      <div class="bg-white shadow-xl rounded-lg p-6 w-96">
        <div id="drop-area" class="border-2 border-dashed border-gray-400 rounded-lg p-6 text-center bg-gray-100 hover:bg-gray-200 transition">
          <p class="text-gray-600">Drag and drop supported files here</p>
          <p class="text-xs text-gray-500 mt-2">
            Supported: .txt, .doc, .docx, .pdf, .odt, .xls, .xlsx, .csv, .html, .xml, .xaml, .yml, .yaml, .json, .bson
          </p>
        </div>
        <div class="mt-4 flex flex-col items-center gap-2">
          <button class="bg-darker text-white px-4 py-2 rounded cursor-pointer hover:bg-dark transition">
            Choose File
          </button>
          <button class="bg-gray-300 text-gray-700 px-4 py-2 rounded cursor-not-allowed opacity-50">
            Paste URL
          </button>
        </div>
      </div>
    </div>
  );
}
