/// <reference types="vitest" />
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import { VitePWA } from 'vite-plugin-pwa';

export default defineConfig({
  plugins: [
    react(),
    VitePWA({
      registerType: 'autoUpdate',
      includeAssets: ['favicon.ico', 'robots.txt', 'icons/*.png'],
      manifest: {
        name: 'Kimi Pocket',
        short_name: 'KimiPocket',
        description: 'Kimi Code 掌上订阅额度与用量监控',
        theme_color: '#0B0C10',
        background_color: '#0B0C10',
        display: 'standalone',
        orientation: 'any',
        icons: [
          {
            src: '/icons/icon-192.png',
            sizes: '192x192',
            type: 'image/png',
          },
          {
            src: '/icons/icon-512.png',
            sizes: '512x512',
            type: 'image/png',
          },
        ],
      },
    }),
  ],
  server: {
    port: 3000,
    host: true, // 允许局域网真机调试 (Android 手机直接访问 Mac IP:3000)
    proxy: {
      '/apiv2': {
        target: 'https://www.kimi.com',
        changeOrigin: true,
        secure: true,
        headers: {
          Referer: 'https://www.kimi.com/code/console',
          Origin: 'https://www.kimi.com',
        },
      },
    },
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: './src/test/setup.ts',
  },
});
