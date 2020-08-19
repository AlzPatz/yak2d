rem Usage:
rem Attempts to compile all .frag and .vert files in current directory and recursive lower directories
rem must be passed path to GLSLC compiler
rem entry point of shaders if main()
rem .frag is fragment shader, .vert is vertex shader

@echo off
rem Don't want to set any global / environment variables
setlocal
cls
echo VULKAN Shader Compiler Batch Script
echo -----------------------------------
echo.

rem The below tests for a blank/non-existent first argument
rem the ~ removes any quotes
if "%~1"=="" (
	echo Please provide the filepath for the GLSLC.exe compiler as the first argument for the .bat file
	goto end
	) 

rem Remove the path and get the filename and extension

set fullpath=%~1
set filename=%~nx1

if "%filename%"=="" (
	echo Provided path "%~1" did not include filename...
	goto end
	)

rem echo Filename to use: "%filename%"

rem Check that the filename is GLSLC.exe

if not "%filename%"=="glslc.exe" (
		echo Provided compiler file was not glslc.exe
		goto end
	)

rem Check if executable for compiler even exists

if not exist %fullpath% (
	echo Provided file "%fullpath%" does not exist
	goto end
	)

rem Here we atleast have a compiler. Let's delete all existing .hlsl.bytes files
del *.spv

set fragmentCount=0
for /r %%f in (*.frag) do set /a fragmentCount+=1
set vertexCount=0
for /r %%v in (*.vert) do set /a vertexCount+=1

echo Directory (and sub directory) search found %vertexCount% potential vertex shader files and %fragmentCount% potential fragment shader files
echo.

if not %vertexCount%==0 (
	echo Vertex Shader Files:
	echo ---------------------
	for %%v in (*.vert) do echo %%v
	)

echo.

if not %fragmentCount%==0 (
	echo Fragment Shader Files:
	echo ---------------------
	for %%f in (*.frag) do echo %%f
	)

echo.

setlocal enabledelayedexpansion

if not %vertexCount%==0 (
	echo Attempting to compile Vertex Shaders...
	for %%v in (*.vert) do (
			echo Compiling: %%v
			set nv=%%v
			rem echo !nv!
			set nve=!nv:~0,-5!
			%fullpath% !nv! -o !nve!.spv
		)
	)

echo.

if not %fragmentCount%==0 (
	echo Attempting to compile Fragment Shaders...
	for %%f in (*.frag) do (
			echo Compiling: %%f
			set nf=%%f
			rem echo !nf!
			set nfe=!nf:~0,-5!
			%fullpath% !nf! -o !nfe!.spv
		)
	)

echo.
echo Complete...

goto skipfail
:end 
	echo Check input and try again...
:skipfail
	echo Exiting...
endlocal