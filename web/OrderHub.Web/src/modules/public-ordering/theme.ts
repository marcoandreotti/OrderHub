import type { PublicContext } from './types'

const defaults = {
  primary: '#4f46e5', secondary: '#0f766e',
  background: '#f8fafc', text: '#0f172a'
}
function hex(value: string) {
  if (!/^#[0-9a-f]{6}$/i.test(value)) return null
  return [1, 3, 5].map(index => Number.parseInt(value.slice(index, index + 2), 16) / 255)
}
function luminance(value: string) {
  const rgb = hex(value)
  if (!rgb) return null
  const linear = rgb.map(channel => channel <= 0.03928
    ? channel / 12.92 : ((channel + 0.055) / 1.055) ** 2.4)
  return 0.2126 * linear[0]! + 0.7152 * linear[1]! + 0.0722 * linear[2]!
}
export function contrast(first: string, second: string) {
  const a = luminance(first)
  const b = luminance(second)
  if (a === null || b === null) return 0
  return (Math.max(a, b) + 0.05) / (Math.min(a, b) + 0.05)
}
export function applyPublicTheme(value: PublicContext) {
  const background = hex(value.theme.backgroundColor) ? value.theme.backgroundColor : defaults.background
  const fallbackText = contrast(background, defaults.text) >= 4.5 ? defaults.text : '#ffffff'
  const text = contrast(background, value.theme.textColor) >= 4.5 ? value.theme.textColor : fallbackText
  const primary = contrast(value.theme.primaryColor, '#ffffff') >= 4.5 ? value.theme.primaryColor : defaults.primary
  const fallbackSecondary = contrast(defaults.secondary, background) >= 3 ? defaults.secondary : text
  const secondary = contrast(value.theme.secondaryColor, background) >= 3 ? value.theme.secondaryColor : fallbackSecondary
  const root = document.documentElement
  root.style.setProperty('--oh-color-primary', primary)
  root.style.setProperty('--oh-color-secondary', secondary)
  root.style.setProperty('--oh-color-background', background)
  root.style.setProperty('--oh-color-text', text)
  root.style.setProperty('--oh-font-family', /^[\w ,'-]+$/.test(value.theme.fontFamily)
    ? value.theme.fontFamily : 'system-ui, sans-serif')
}
