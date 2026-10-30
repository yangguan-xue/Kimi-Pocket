import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.yangguan.kimipocket',
  appName: 'Kimi Pocket',
  webDir: 'dist',
  backgroundColor: '#000000',
  plugins: {
    CapacitorHttp: {
      enabled: true, // 启用 Android 原生 HTTP 拦截，彻底无视浏览器 CORS 跨域限制
    },
  },
  android: {
    backgroundColor: '#000000',
    allowMixedContent: true,
  },
  server: {
    androidScheme: 'https',
  },
};

export default config;
