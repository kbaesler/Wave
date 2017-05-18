:: -----------------------------------------------------------
:: 	Name: mkdocs-setup.bat
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

:main
pip install mkdocs --upgrade | more
pip install mkdocs-material --upgrade | more
pip install pygments --upgrade | more
pip install pymdown-extensions --upgrade
:end