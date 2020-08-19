#!/bin/bash

echo Script to compile a directory full of vulkan shaders...

rm *.spv

shopt -s nullglob;
for file in *.frag ; 
	do 
		[ -f "$file" ] || break
		echo ------------------;
		echo File Found: "$file".... compiling...;
		NAME="${file%%.*}";
		glslc $file -o $NAME.spv
		echo $NAME.spv compiled...
done

for file in *.vert ; 
	do 
		[ -f "$file" ] || break
		echo ------------------;
		echo File Found: "$file".... compiling...;
		NAME="${file%%.*}";
		glslc $file -o $NAME.spv
		echo $NAME.spv compiled...
done

echo .............. done!
