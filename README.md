<div align="center">

# Kimi Pocket

<p><strong>Carry your Kimi Code quota in your pocket.</strong></p>
<p>An ultra-lightweight, cross-platform subscription and quota monitor for Kimi Code.<br/>Natively crafted for <strong>macOS Menubar HUD</strong> · <strong>Windows System Tray</strong> · <strong>Android App</strong> · <strong>Mobile PWA</strong>.</p>

<p>
  <a href="./README.md"><strong>English</strong></a> ·
  <a href="./README_CN.md">简体中文</a>
</p>

<p>
  <a href="https://github.com/yangguan-xue/Kimi-Pocket/releases/latest"><img src="https://img.shields.io/github/v/release/yangguan-xue/Kimi-Pocket?color=10B981&label=Release" alt="Release"/></a>
  <a href="https://github.com/yangguan-xue/Kimi-Pocket/actions"><img src="https://img.shields.io/github/actions/workflow/status/yangguan-xue/Kimi-Pocket/release.yml?branch=main&label=CI%20Build" alt="Build Status"/></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT"/></a>
  <img src="https://img.shields.io/badge/Platform-macOS%20%7C%20Windows%20%7C%20Android%20%7C%20PWA-262626" alt="Platforms"/>
  <img src="https://img.shields.io/badge/Privacy-Local--First%20%2F%20Zero--Relay-059669" alt="Zero Relay Security"/>
</p>

<p>
  <a href="#overview">Overview</a> •
  <a href="#key-features">Key Features</a> •
  <a href="#downloads">Downloads</a> •
  <a href="#architecture--privacy">Architecture</a> •
  <a href="#credential-configuration">Credentials</a> •
  <a href="#refresh-strategy">Refresh Strategy</a> •
  <a href="#development--build">Development</a> •
  <a href="#license">License</a>
</p>

</div>

---

## Overview

When developing intensively with Kimi Code (Moonshot AI), developers constantly need to monitor three critical quota metrics:
- **5-Hour Rolling Request Window** and its exact reset countdown.
- **Weekly Quota** consumption and renewal schedule.
- **Subscription Balance** package validity and expiration date.

Traditionally, checking these numbers requires interrupting your workflow, logging into the web dashboard, or opening browser DevTools to inspect network requests. **Kimi Pocket** brings these metrics directly into your desktop status bar, system tray, or mobile lock screen with instant visibility and zero mental friction.

---

## Key Features

### 🍏 macOS Native Menubar HUD
- **Zero-Click Hover HUD**: Hover over the menubar icon (>180ms debounce) to instantly expand an Obsidian Dark frosted-glass HUD; moving the cursor away smoothly dismisses it.
- **Ultra-Lightweight & Efficient**: Engineered with Swift 6 + SwiftUI + AppKit. Consumes 0% idle CPU and maintains a steady memory footprint of under 25MB.
- **Hardware-Level Security**: Credentials are encrypted directly inside macOS Keychain — never stored in plaintext config files.
- **Dynamic Status Indicator**: Status bar icon dynamically displays status badges and color-coded alert states.

### 🪟 Windows Native Taskbar Tray
- **Restrained Dark Aesthetic**: Built natively with .NET and WPF, featuring a high-contrast, fully opaque dark theme with system-native typography.
- **Seamless Hover Experience**: Zero-click tray expansion and auto-dismiss on cursor leave; full keyboard accessibility via `Win+B`, `Tab`, and `Esc`.
- **Self-Contained Deployment**: Ships with an embedded .NET runtime inside a single portable ZIP — extract and run with zero external runtime prerequisites.
- **Credential Protection**: Uses Windows Credential Manager for secure, encrypted token persistence.

### 📱 Android Native App & Mobile PWA
- **Responsive Dual-Mode UI**:
  - **Portrait Mode**: Three vertical segmented cards (4-segment total bar, 7-segment weekly bar, smooth rolling 5-hour bar with live countdown).
  - **Landscape Mode**: StandBy-inspired dashboard powered by precise SVG math (4-arc total ring, 7-arc weekly ring, continuous 5-hour ring).
- **Mobile-First UX**: One-tap clipboard token import, intelligent layout adjustment when the virtual keyboard appears, and immersive fullscreen display.
- **Offline Snapshot Resilience**: Retains and displays the last known good snapshot during network hiccups or temporary disconnections.

### ⚡ Core Engine & Smart Polling
- **Dynamic Heartbeat Polling**: Automatically adjusts poll intervals based on lowest remaining quota (60s Normal / 30s Warning / 15s Critical); pauses in the background to preserve battery and refreshes immediately when returning to the foreground.
- **Silent Background Token Rotation**: Supports optional `refresh_token` to automatically rotate expired `access_token` credentials without user interruption.
- **Zero-Relay Privacy**: Direct HTTPS communication between your device and Moonshot's official servers. No middleman proxies, no analytics, no user tracking.

---

## Downloads

Official multi-platform installation packages are published on GitHub Releases:

| Platform | Form Factor | Architecture & Requirements | Direct Download |
| :--- | :--- | :--- | :--- |
| 🍏 **macOS** | Native Menubar HUD (Hover Trigger) | Apple Silicon (M1/M2/M3/M4), macOS 14+ | [**Download DMG**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) · [Download Portable ZIP](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 🪟 **Windows** | Native System Tray Panel (Embedded Runtime) | Windows 11 x64 (Portable, No Install) | [**Download Portable ZIP**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 📱 **Android** | Native App (Landscape HUD / Portrait Cards) | Android 8.0+ | [**Download Android APK**](https://github.com/yangguan-xue/Kimi-Pocket/releases/latest) |
| 🌐 **Web / PWA** | Browser App / Add to Home Screen | Modern Mobile & Desktop Browsers | [Self-Host / Build from Source](#development--build) |

> Checksums (SHA256) and detailed release changelogs are available on the [GitHub Releases Page](https://github.com/yangguan-xue/Kimi-Pocket/releases).

---

## Architecture & Privacy

Kimi Pocket strictly adheres to **Local-First** and **Zero-Relay** principles:

```
┌────────────────────────────────────────────────────────┐
│             Client (macOS / Windows / Android)         │
│                                                        │
│  ┌──────────────────────────────────────────────────┐  │
│  │                    UI Layer                      │  │
│  │  - macOS: SwiftUI + AppKit                       │  │
│  │  - Windows: WPF + Win32 NotifyIcon               │  │
│  │  - Android / Web: React 19 + Tailwind CSS        │  │
│  └────────────────────────┬─────────────────────────┘  │
│                           │                            │
│                           ▼                            │
│  ┌──────────────────────────────────────────────────┐  │
│  │             Native Secure Key Storage            │  │
│  │  - macOS Keychain / Windows Credential Manager   │  │
│  │  - Android EncryptedStorage / LocalStorage       │  │
│  └────────────────────────┬─────────────────────────┘  │
└───────────────────────────┼────────────────────────────┘
                            │ Direct HTTPS Request (Bearer Token)
                            ▼
┌────────────────────────────────────────────────────────┐
│              Official Moonshot Kimi API                │
│                                                        │
│  • GET  /apiv2/usages        (Total, weekly, 5-hour)   │
│  • GET  /apiv2/subscription  (Package tier, validity) │
│  • POST /apiv2/token/refresh (Silent token rotation)   │
└────────────────────────────────────────────────────────┘
```

- **Tokens Stay on Device**: Authentication credentials are stored strictly within OS-level secure storage and only sent directly to official Moonshot endpoints.
- **Direct Connectivity**: Desktop clients utilize OS network stacks (Swift `URLSession` / .NET `HttpClient`) directly, bypassing browser CORS restrictions without intermediary proxy servers.

---

## Credential Configuration

Kimi Pocket requires your Kimi authorization tokens to fetch quota metrics. Follow these steps:

1. Log into your account on [Kimi Official Site](https://kimi.moonshot.cn/) in a desktop browser;
2. Press `F12` (or right-click → **Inspect**) to open Developer Tools, then navigate to the **Application** tab;
3. In the left panel, expand **Storage → Local Storage**, and click `https://kimi.moonshot.cn`;
4. Locate and copy the following keys:

| Field Name | Required | Description |
| :--- | :--- | :--- |
| `access_token` | **Yes** | Primary bearer token for quota and subscription APIs (JWT format) |
| `refresh_token` | **Recommended** | Used to automatically rotate and renew the `access_token` in the background |

5. Paste the copied values into Kimi Pocket's Settings panel and save.

> [!WARNING]
> Your tokens grant access to your account. Keep them secure and never share them in public screenshots or issue trackers.

---

## Refresh Strategy

The polling engine automatically regulates its heartbeat according to the **lowest remaining percentage** among all three quota categories:

| Quota Status | Polling Interval | Visual Theme / State | Trigger Threshold |
| :--- | :--- | :--- | :--- |
| **Normal** | 60 seconds | Emerald Green (`#10B981`) / Normal Icon | All quotas remaining > 30% |
| **Warning** | 30 seconds | Amber Yellow (`#F59E0B`) / Alert Badge | Any quota remaining ≤ 30% |
| **Critical** | 15 seconds | Crimson Red (`#EF4444`) / High Priority | Any quota remaining ≤ 10% |

- **Lifecycle Aware**: Polling automatically sleeps when the window minimizes or the screen turns off, saving battery. When foregrounded, an immediate refresh triggers.
- **Error Handling**: On HTTP 401 (token expired/invalid), polling halts gracefully with a clear notification; network drops retain the cached snapshot with an offline status label.

---

## Development & Build

### Clone Repository

```bash
git clone https://github.com/yangguan-xue/Kimi-Pocket.git
cd Kimi-Pocket
```

### 1. Web & Android (React 19 + Capacitor 7)

- **Prerequisites**: Node.js 20+, npm; Android builds require JDK 21 and Android SDK Platform 35.

```bash
# Install dependencies
npm ci

# Start local dev server (includes /apiv2 proxy)
npm run dev

# Run unit tests
npm test

# Build production Web / PWA assets
npm run build

# Sync assets to Capacitor native Android project
npm run build:android

# Compile Android Debug APK
cd android && ./gradlew assembleDebug
```
Output path: `android/app/build/outputs/apk/debug/app-debug.apk`.

### 2. macOS Menubar App (Swift 6 + SwiftUI)

- **Prerequisites**: macOS 14+, Xcode 16+ or standalone Command Line Tools (Swift 6.0+).

```bash
# Navigate to macOS project directory
cd macos

# Run unit tests
swift test

# Build and package DMG / ZIP distributions
bash ../scripts/build-macos.sh
```
Output path: `dist-macos/KimiPocket-v{VERSION}-macOS.dmg` and `.zip`.

### 3. Windows System Tray App (.NET 10 + WPF)

- **Prerequisites**: Windows 11 x64, .NET 10 SDK.

```powershell
# Run unit tests and formatting checks
dotnet test windows/KimiPocket.sln --configuration Release

# Build self-contained portable ZIP package (with embedded runtime)
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\build-windows.ps1
```
Output path: `dist-windows/Kimi-Pocket-windows-x64-{VERSION}.zip`.

---

## Project Structure

```text
kimi-pocket/
├── android/            # Capacitor native Android app and Gradle configs
├── macos/              # macOS native project (Swift 6 + SwiftUI + AppKit)
│   ├── Sources/App/    # Status item, hover HUD, and controllers
│   └── Sources/Core/   # Quota data models, Keychain storage, HTTP client
├── windows/            # Windows native project (.NET + WPF)
│   ├── src/            # Tray icon, WPF panel, and Credential Manager
│   └── tests/          # Unit tests and layout integration tests
├── src/                # Web / Android frontend source (React 19 + TS)
│   ├── components/     # Segmented card and 3-ring HUD components
│   ├── services/       # Official Moonshot API protocol contracts
│   └── hooks/          # Dynamic polling and state machine hooks
├── specs/              # Specification-Driven Development (SDD) design docs
├── scripts/            # Multi-platform packaging and CI automation scripts
└── tasks/              # Phased task tracking cards and implementation notes
```

---

## Engineering Standards

- **Specification-Driven Development (SDD)**: All architectural designs, schema alterations, and platform ports are driven strictly by contract specifications under `specs/`. No implementation precedes a specification.
- **Type Safety & Testing**: Strict TypeScript compilation (`strict: true`, zero `any`), comprehensive Vitest suites for core mathematical calculations, and native platform unit tests.
- **Conventional Commits**: Commit messages follow the Conventional Commits specification (`feat:`, `fix:`, `docs:`, `test:`, `refactor:`, `chore:`).

---

## Acknowledgements

- [nekocharm1207/kimi-island](https://github.com/nekocharm1207/kimi-island): Valuable protocol analysis and design inspiration.
- [Capacitor](https://capacitorjs.com/): Cross-platform native runtime for mobile.
- [Tailwind CSS](https://tailwindcss.com/) & [Lucide Icons](https://lucide.dev/): Design system and icon assets.

---

## License

This project is licensed under the [MIT License](LICENSE).
