export interface TenantTheme {
  primary: string
  secondary: string
  accent: string
  background: string
  surface: string
  text: string
  borderRadius: string
  fontFamily: string
}

export const defaultTenantTheme: TenantTheme = {
  primary: '#4f46e5',
  secondary: '#0f766e',
  accent: '#f59e0b',
  background: '#f8fafc',
  surface: '#ffffff',
  text: '#0f172a',
  borderRadius: '12px',
  fontFamily: "Inter, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
}

export function applyTenantTheme(theme: TenantTheme): void {
  const root = document.documentElement
  Object.entries(theme).forEach(([key, value]) => {
    const cssName = key.replace(/[A-Z]/g, character => `-${character.toLowerCase()}`)
    root.style.setProperty(`--oh-${cssName.startsWith('color-') ? cssName : `color-${cssName}`}`, value)
  })
  root.style.setProperty('--oh-border-radius', theme.borderRadius)
  root.style.setProperty('--oh-font-family', theme.fontFamily)
}
