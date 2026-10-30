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
        obsidian: '#000000', // 纯黑 OLED True Black
        cardDark: '#0A0A0A', // 中性纯黑微浅，绝无偏蓝
        cardSubtle: '#121212',
        borderDark: 'rgba(255, 255, 255, 0.08)',
        emeraldGlow: '#10B981',
      },
      fontFamily: {
        mono: ['ui-monospace', 'SF Mono', 'JetBrains Mono', 'Roboto Mono', 'monospace'],
      }
    },
  },
  plugins: [],
}
