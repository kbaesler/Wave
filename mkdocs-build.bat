:: -----------------------------------------------------------
:: 	Name: mkdocs-build.bat
:: 	Author: Kyle Baesler
:: 	Date: 2016-11-04
:: -----------------------------------------------------------
::	Description: Use this script to download the necessary
::				python scripts that build the mkdocs documentation
::				using the markdown files in the /docs directory.
:: -----------------------------------------------------------
::	To-do:	
:: -----------------------------------------------------------

@echo off
set URL=http:\\127.0.0.1:8000

:main
mkdocs build --clean

if not "%1" == "" (
	set URL=%1
)

if not "%2" == "" goto end

start "" %URL%
mkdocs serve

:end