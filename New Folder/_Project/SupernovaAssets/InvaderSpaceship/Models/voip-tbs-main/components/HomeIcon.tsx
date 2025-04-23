interface Props {
  size?: number;
}

export default function HomeIcon({ size = 128 }: Props) {
  if (size == 0) {
    return (
      <a href="/" class="flex items-center space-x-2 h-full">
        <img
          src="/logo.png"
          class="h-full"
          alt="The Gateway Corporate Solutions logo"
        />
      </a>
    );
  }

  return (
    <a href="/" class="flex items-center space-x-2">
      <img
        src="/logo.png"
        width={size}
        height={size}
        alt="The Gateway Corporate Solutions logo"
      />
    </a>
  );
}
