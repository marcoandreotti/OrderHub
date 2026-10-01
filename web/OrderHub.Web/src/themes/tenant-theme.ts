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
  primary: '#f97316',
  secondary: '#1f2937',
  accent: '#ea580c',
  background: '#f8fafc',
  surface: '#ffffff',
  text: '#374151',
  borderRadius: '12px',
  fontFamily: "Inter, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
}

export function applyTenantTheme(theme: TenantTheme, surface: HTMLElement): void {
  surface.style.setProperty('--oh-public-brand-primary', theme.primary)
  surface.style.setProperty('--oh-public-brand-primary-hover', theme.accent)
  surface.style.setProperty('--oh-public-brand-secondary', theme.secondary)
  surface.style.setProperty('--oh-public-theme-background', theme.background)
  surface.style.setProperty('--oh-public-theme-surface', theme.surface)
  surface.style.setProperty('--oh-public-theme-text', theme.text)
  surface.style.setProperty('--oh-public-border-radius', theme.borderRadius)
  surface.style.setProperty('--oh-public-font-family', theme.fontFamily)
}
