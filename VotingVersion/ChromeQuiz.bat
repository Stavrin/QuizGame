@echo off
taskkill /F /IM chrome.exe

start "Chrome" chrome "https://v6p9d9t4.ssl.hwcdn.net/html/7775447/index.html" --kiosk --disable-infobars --enable-features=OverlayScrollbar --allow-file-access-from-files --incognito --kiosk --disable-direct-write

