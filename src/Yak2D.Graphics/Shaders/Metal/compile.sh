#!/bin/bash

echo Script to compile a directory full of metal shaders...

rm *.metallib

shopt -s nullglob;
for file in *.metal ; 
	do 
		[ -f "$file" ] || break
		echo ------------------;
		echo File Found: "$file".... compiling...;
		NAME="${file%%.*}";
		sudo xcrun -sdk macosx metal -c "$file" -o $NAME.air  
		sudo xcrun -sdk macosx metallib $NAME.air -o $NAME.metallib
		rm $NAME.air
		echo $NAME.metallib compiled...
done

echo .............. done!

