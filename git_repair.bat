@echo off
setlocal enabledelayedexpansion

REM Define the URL for git cloning
set git_url=https://github.com/Team8Alpha/smo3ds

REM Clone the repository
git clone %git_url% smo3ds

REM Check if the clone was successful
if not errorlevel 1 (
    echo Git clone successful.

    REM Copy all files from the current folder to smo3ds folder, skipping existing files
    for /r %%i in (*) do (
        if not exist "smo3ds\%%~nxi" (
            copy "%%i" "smo3ds\%%~nxi"
        )
    )

    REM Delete remaining data from the root folder
    del /q *

    REM Copy all files from smo3ds folder to the root folder
    for /r "smo3ds" %%i in (*) do (
        copy "%%i" "%%~nxi"
    )

    REM Delete smo3ds folder and its contents
    rmdir /s /q "smo3ds"

    echo Cleanup completed.
) else (
    echo Git clone failed.
)

endlocal
