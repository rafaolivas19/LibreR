for /f "delims=" %%a in ('git rev-parse HEAD') do @set VERSION=%%a
powershell -Command "(type C:\Users\ingen\Documents\Dev\LibreR\LibreR\Properties\AssemblyInfo.cs) -replace '\[assembly: AssemblyTitle\(\".*?\"\)\]', '[assembly: AssemblyTitle(\"%VERSION%\")]' | Out-File C:\Users\ingen\Documents\Dev\LibreR\LibreR\Properties\AssemblyInfo.cs"
pause