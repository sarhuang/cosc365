#Name: Sarah Huang
#Date: 3/14/23
#Program: part1.sh
#Purpose: Given a directory with personal record files, decompose these record files into directories, where each property is now its own file
#Source for newline condition: https://unix.stackexchange.com/questions/482517/why-does-this-while-loop-not-recognize-the-last-line 


#!/usr/bin/env bash

# Your solution for part 1 goes here.

for file in *.txt; do

	name=$(basename "$file" .txt)
	mkdir "$name"

	#Extra condition since read will fail when reading a line with no newline
	while read line || [ -n "$line" ]; do
		property=$(echo "$line" | awk '{print $1}') #Read 1st column of properties (id, age, email)
		value=$(echo "$line" | awk '{print $2}') #Read 2nd column of values

		echo "$value" > "$name/$property.txt"
	done < "$file"
done
