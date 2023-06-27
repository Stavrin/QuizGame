@echo off

:: Fetch and pull new commits
cd "c:/Kiosk\QuizGame"
git fetch
git pull

timeout /t 10

cd %USERPROFILE%\AppData\Local\Programs\Servez\
start "" /min servez.exe -- --port=1234 c:/Kiosk\QuizGame\Builds

timeout /t 5

taskkill /F /IM chrome.exe

start "" chrome "http://localhost:1234/" --disable-infobars --allow-file-access-from-files -incognito --disable-direct-write --kiosk
