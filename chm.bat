:: -----------------------------------------------------------
:: 	Name: chm.bat
:: 	Author: Kyle Baesler
:: 	Date: 2016-11-04
:: -----------------------------------------------------------
::	Description: Use this script to build the CHM documentation
::				files.
:: -----------------------------------------------------------
::	To-do:	
:: -----------------------------------------------------------
@echo off

set MSBUILD=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBUILD
set NET35=net35.shfbproj
set NET45=net45.shfbproj

:main
if "%1" == "" goto help

if "%1" == "Help" (
	:help
	echo.Please use `chm.bat ^<version^>` where ^<version^>`
	echo.	3	to compile the .NET 3.5 version.
	echo.	4	to compile the .NET 4.5 version.
	echo.	7	to compile both the .NET 3.5 and 4.5 version.
)

:net35
if "%1" == "3" (
	%MSBUILD% %NET35%
	if errorlevel 1 exit /b 1
	echo.
	goto end
)

:net45
if "%1" == "4" (
	%MSBUILD% %NET35%
	if errorlevel 1 exit /b 1
	echo.
	goto end
)

:both
if "%1" == "7" (
	%MSBUILD% %NET35%
	%MSBUILD% %NET45%
	if errorlevel 1 exit /b 1
	echo.
	goto end
)

:end