import { defineConfig } from '#q-app/wrappers'

export default defineConfig(() => ({
  boot: ['http'],
  css: ['app.scss'],
  build: {
    target: {
      browser: ['es2022'],
      node: 'node22'
    },
    vueRouterMode: 'history'
  },
  devServer: {
    open: false,
    port: 9000
  },
  framework: {
    config: {
      brand: {
        primary: '#4f46e5',
        secondary: '#0f766e',
        accent: '#f59e0b'
      }
    },
    plugins: []
  }
}))
