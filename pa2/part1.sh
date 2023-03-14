#!/usr/bin/env bash

# Your solution for part 1 goes here.

for file in "part1_start"/*.txt; do

	name=$(basename "$file" .txt_
	mkdir "$name"

	while read line || [ -n "$line"]; do
		property=$(echo "$line" | awk '{print $1}')
		value=$(echo "$line" | awk '{print $2}')

		echo "$value" > "$name/$property.txt"
	done < "$file"
done
