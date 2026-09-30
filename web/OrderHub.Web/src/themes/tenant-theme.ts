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
  surface.style.setProperty('--oh-brand-primary', theme.primary)
  surface.style.setProperty('--oh-brand-secondary', theme.secondary)
  surface.style.setProperty('--oh-brand-accent', theme.accent)
  surface.style.setProperty('--oh-surface-page', theme.background)
  surface.style.setProperty('--oh-surface-raised', theme.surface)
  surface.style.setProperty('--oh-text-primary', theme.text)
  surface.style.setProperty('--oh-border-radius', theme.borderRadius)
  surface.style.setProperty('--oh-font-family', theme.fontFamily)
}
