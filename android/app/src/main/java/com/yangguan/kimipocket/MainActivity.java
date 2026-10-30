package com.yangguan.kimipocket;

import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.view.ActionMode;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.WebSettings;
import android.webkit.WebView;
import androidx.core.view.WindowCompat;
import androidx.core.view.WindowInsetsCompat;
import androidx.core.view.WindowInsetsControllerCompat;
import com.getcapacitor.BridgeActivity;

public class MainActivity extends BridgeActivity {
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        applyImmersiveFullScreen();
        configureWebViewDarkSettings();
    }

    @Override
    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        if (hasFocus) {
            applyImmersiveFullScreen();
        }
    }

    private void configureWebViewDarkSettings() {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.Q) {
            try {
                WebView webView = getBridge().getWebView();
                if (webView != null) {
                    // 禁止 Chromium 引擎二次强制反色，防止输入框长按弹出的 Floating ActionMode 变成全屏白色反色蒙层
                    webView.getSettings().setForceDark(WebSettings.FORCE_DARK_OFF);
                }
            } catch (Exception ignored) {}
        }
    }

    private void applyImmersiveFullScreen() {
        Window window = getWindow();
        if (window == null) return;

        // 窗口纯黑，彻底杜绝切口漏白
        window.getDecorView().setBackgroundColor(Color.BLACK);

        // 允许延伸进摄像头挖孔区域，横屏竖屏均占满全屏
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.P) {
            WindowManager.LayoutParams lp = window.getAttributes();
            lp.layoutInDisplayCutoutMode = WindowManager.LayoutParams.LAYOUT_IN_DISPLAY_CUTOUT_MODE_SHORT_EDGES;
            window.setAttributes(lp);
        }

        // 绝对全屏：隐藏顶部信号栏/状态栏与底部虚拟按键，启用手势轻扫临时浮动显示
        WindowCompat.setDecorFitsSystemWindows(window, false);
        WindowInsetsControllerCompat controller = WindowCompat.getInsetsController(window, window.getDecorView());
        if (controller != null) {
            controller.hide(WindowInsetsCompat.Type.systemBars());
            controller.setSystemBarsBehavior(WindowInsetsControllerCompat.BEHAVIOR_SHOW_TRANSIENT_BARS_BY_SWIPE);
        }
    }

    @Override
    public ActionMode onWindowStartingActionMode(ActionMode.Callback callback, int type) {
        try {
            return super.onWindowStartingActionMode(callback, type);
        } catch (Exception e) {
            return null;
        }
    }
}
