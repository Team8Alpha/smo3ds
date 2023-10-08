@echo off
WHERE magick
if errorlevel 1 echo Please install ImageMagick & pause>nul & exit
set animformat=gif
:: If you have ImageMagick version 7.1.10-20 or above, you may uncomment the lines below. As of 11/27/2022, the latest windows binary release is 7.1.0, so unless you compiled from source you shouldn't uncomment the lines. The lines are commented out because versions below 7.1.10-20 will incorrectly and randomly speed up or slow down animations converted to apng.
::WHERE ffmpeg
::if errorlevel 1 echo If you have ffmpeg, please put it in the system path.
::choice /m "use animated png instead of gif (requires ffmpeg)"
::goto option-%errorlevel%
:::option-1
::echo animated png selected
::set animformat="apng"
::goto continue
:::option-2
::echo gif selected
:::continue
FOR %%i IN (*.bmp) DO (
	echo "%%~nxi"
	magick identify "%%~nxi" | FIND "bmp["
	IF errorlevel 1 (
		echo not an animated webp
		magick "%%~nxi" "%%~ni.png"
	) ELSE (
		echo animated webp
		magick "%%~nxi" "%%~ni.%animformat%"
	)
)
del *.webp