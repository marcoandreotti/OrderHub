## 1. Preference and theme foundation

- [x] 1.1 Implement the browser-local `system | light | dark` preference resolver, system-change handling, cross-tab synchronization, and pre-render initialization; verify missing or invalid storage resolves to the device preference.
- [x] 1.2 Add paired semantic light/dark tokens and initialize Quasar Dark from the same resolved mode; verify framework overlays and CSS surfaces use matching modes.

## 2. Public tenant theme compatibility

- [x] 2.1 Separate tenant brand inputs from neutral public surface tokens and adapt foregrounds for dark appearance; verify tenant brand variables remain scoped to Public and neutral text remains readable.

## 3. Shared control and surface integration

- [x] 3.1 Create the shared accessible appearance control with System, Claro, and Escuro choices; verify current selection is announced and all choices work by keyboard.
- [x] 3.2 Add the control to Public, Administration, Operations/KDS, and Platform headers and adapt remaining fixed neutral surfaces; verify all views change immediately without navigation or reload.

## 4. Documentation and integration verification

- [x] 4.1 Update the web Design System with the shared control and token behavior, then run frontend typecheck, build, and OpenSpec validation; verify all commands succeed.
