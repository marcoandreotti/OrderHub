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

export function applyTenantTheme(theme: TenantTheme, surface: HTMLElement): void {
  surface.style.setProperty('--oh-brand-primary', theme.primary)
  surface.style.setProperty('--oh-brand-secondary', theme.secondary)
  surface.style.setProperty('--oh-brand-accent', theme.accent)
  surface.style.setProperty('--oh-surface-page', theme.background)
  surface.style.setProperty('--oh-surface-raised', theme.surface)
  surface.style.setProperty('--oh-text-primary', theme.text)
  surface.style.setProperty('--oh-border-radius', theme.borderRadius)
  surface.style.setProperty('--oh-font-family', theme.fontFamily)
}
