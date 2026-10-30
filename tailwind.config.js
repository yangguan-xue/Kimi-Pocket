/** @type {import('tailwindcss').Config} */
export default {
  darkMode: 'class',
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        obsidian: '#0B0C10',
        cardDark: '#12141A',
        cardSubtle: '#181B22',
        borderDark: 'rgba(255, 255, 255, 0.08)',
        cyanGlow: '#00E5FF',
        emeraldGlow: '#10B981',
      },
      fontFamily: {
        mono: ['ui-monospace', 'SF Mono', 'JetBrains Mono', 'Roboto Mono', 'monospace'],
      }
    },
  },
  plugins: [],
}
