rem an attempt to get environment vars set up before building packetmap

call "C:\Program Files\Microsoft Visual Studio 8\VC\vcvarsall.bat" x86
echo Conveniently, VS80COMNTOOLS is now %VS80COMNTOOLS%
echo and PATH is %PATH%
echo so this should work...

msbuild TimeTube.sln
