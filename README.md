# DesktopToggle

一键隐藏/显示 Windows 桌面图标，系统托盘操作，干净利落。

## 功能

- 左键点击托盘图标 → 切换桌面图标的显示和隐藏
- 右键托盘图标 → 弹出菜单，可切换或退出
- 退出时自动恢复桌面图标
- 不影响右键菜单的"显示桌面图标"选项

## 安装

1. 从 [Releases](../../releases) 下载 `DesktopToggle.exe`
2. 双击运行即可，图标出现在系统托盘

## 开机自启

将 `DesktopToggle.exe` 的快捷方式放入 `shell:startup` 即可：

```
Win+R → shell:startup → 粘贴快捷方式
```

## 技术

- 通过 Win32 API 操控桌面 SysListView32 窗口的可见性
- 纯 C# / .NET Framework 4.x，无需额外依赖
- 预渲染托盘图标，切换零延迟

## 许可证

MIT
