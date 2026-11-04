#!/usr/bin/env bash
set -e

# ==============================================================================
# Kimi Pocket macOS App 打包构建脚本
# ==============================================================================

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
MACOS_DIR="$ROOT_DIR/macos"
DIST_DIR="$ROOT_DIR/dist-macos"
APP_NAME="Kimi Pocket"
APP_BUNDLE="$DIST_DIR/$APP_NAME.app"

echo "=================================================="
echo "  🚀 开始构建 $APP_NAME for macOS (Release)       "
echo "=================================================="

# 1. 编译 Swift Release 二进制
echo "📦 [1/5] 正在通过 Swift Package Manager 编译 Release 二进制..."
cd "$MACOS_DIR"
swift build -c release

BIN_PATH="$MACOS_DIR/.build/release/KimiPocket"
if [ ! -f "$BIN_PATH" ]; then
    echo "❌ 未找到构建产物: $BIN_PATH"
    exit 1
fi

# 2. 创建标准 .app 结构
echo "📁 [2/5] 正在创建 macOS App Bundle 目录结构..."
rm -rf "$APP_BUNDLE"
mkdir -p "$APP_BUNDLE/Contents/MacOS"
mkdir -p "$APP_BUNDLE/Contents/Resources"

# 3. 复制二进制与 Info.plist
echo "📄 [3/5] 正在装配可执行文件与元数据配置文件..."
cp "$BIN_PATH" "$APP_BUNDLE/Contents/MacOS/KimiPocket"
chmod +x "$APP_BUNDLE/Contents/MacOS/KimiPocket"

cp "$MACOS_DIR/Resources/Info.plist" "$APP_BUNDLE/Contents/Info.plist"

# 4. 生成 PkgInfo 标记
echo "APPL????" > "$APP_BUNDLE/Contents/PkgInfo"

# 5. Ad-Hoc 本地代码签名 (避免 Gatekeeper 拦截)
echo "🔏 [4/5] 正在执行本地 Ad-Hoc 代码签名..."
codesign -s - --force --deep "$APP_BUNDLE" 2>/dev/null || true

# 6. 生成 DMG 镜像与 ZIP 安装包
echo "📦 [5/5] 正在打包 DMG 与 ZIP 发布分发包..."
ARCH=$(uname -m)
VERSION="0.2.0"
DMG_NAME="Kimi-Pocket-macos-${ARCH}-${VERSION}.dmg"
ZIP_NAME="Kimi-Pocket-macos-${ARCH}-${VERSION}.zip"

cd "$DIST_DIR"
rm -f "$DMG_NAME" "$ZIP_NAME" "${DMG_NAME}.sha256" "${ZIP_NAME}.sha256"

# 制作 DMG 磁盘映像
hdiutil create -volname "$APP_NAME" -srcfolder "$APP_NAME.app" -ov -format UDZO "$DMG_NAME" > /dev/null
shasum -a 256 "$DMG_NAME" | awk '{print $1}' > "${DMG_NAME}.sha256"

# 制作保留权限的 ZIP 便携包
ditto -c -k --sequesterRsrc --keepParent "$APP_NAME.app" "$ZIP_NAME"
shasum -a 256 "$ZIP_NAME" | awk '{print $1}' > "${ZIP_NAME}.sha256"

# 7. 完成汇总
echo "=================================================="
echo "  ✅ 构建成功！产物已就绪:                         "
echo "  👉 应用程序: $APP_BUNDLE"
echo "  👉 DMG 镜像: $DIST_DIR/$DMG_NAME"
echo "  👉 ZIP 压缩包: $DIST_DIR/$ZIP_NAME"
echo "=================================================="
echo "  💡 你可以直接双击运行，或将其拖入 /Applications 文件夹。"
echo "=================================================="
