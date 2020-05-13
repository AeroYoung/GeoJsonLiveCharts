cd /d %~dp0
echo off
cls

for /f "delims=" %%i in ("%cd%") do set folder=%%~ni

set proj=%folder%.csproj

set Configuration=Release
set /p Configuration=Enter configuration or just ENTER for default [%Configuration%] : 

set version=1.0.0.1
set /p version=Enter version or just ENTER for default [%version%] : 
set nupkg=%folder%.%version%.nupkg

set ApiKey=apikey
set /p ApiKey=Enter the nuget api key:

nuget spec -Force
nuget pack %proj% -Prop Configuration=%Configuration%
nuget push %nupkg% %ApiKey% -Source https://api.nuget.org/v3/index.json

pause