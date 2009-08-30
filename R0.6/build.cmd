@echo off

SET msbuild=C:\Windows\Microsoft.NET\Framework\v3.5\msbuild.exe
SET solution=ID3Tag.Net.sln

echo Building Debug Configuration ...
%msbuild% %solution% /t:Rebuild /p:Configuration=Debug /fileLogger /fileLoggerParameters:LogFile=build.debug.log
if errorlevel 1 goto error

echo Building Release Configuration ...
%msbuild% %solution% /t:Rebuild /p:Configuration=Release /fileLogger /fileLoggerParameters:LogFile=build.release.log
if errorlevel 1 goto error

rem NUnit C:\Program Files\NUnit 2.4.6\bin

echo Preparing Deployment Package ...
xcopy ID3TagUtility\bin\Release\ID3Tag.Net.dll bin\*.* /Y
xcopy ID3TagUtility\bin\Release\ID3Tag.Net.xml bin\*.* /Y
xcopy ID3TagUtility\bin\Release\ID3TagUtility.exe bin\*.* /Y

exit 0

:error
echo Build Failed!!!
pause
exit 1