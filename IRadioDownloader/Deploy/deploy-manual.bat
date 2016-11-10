rmdir RadioOwl /S /Q
md RadioOwl
xcopy ..\bin\Debug\*.* RadioOwl 

@rem odstraneni problemu s 9 vs 10 ... mezery nahradim nulama
set zipname=RadioOwl_%DATE:~-4%%DATE:~-7,2%%DATE:~-10,2%-%TIME:~-11,2%%TIME:~-8,2%.7z
7z a -r -mx9 -x!*.vshost.* -x!*.xml -x!*.pdb -x!*.config -x!*.bat -x!*.7z %zipname: =0% *.*

rmdir RadioOwl /S /Q

      
    
