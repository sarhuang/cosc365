#Name: Sarah Huang
#Date: 3/14/23
#Program: part2.sh
#Purpose: Process a CSV (comma-separated value) file with car statistics, producing a file called answer.txt that contains the make of the car with the highest horsepower and the actual horespower value 

#!/usr/bin/env bash

# Your solution for part 2 goes here.

max_horsepower_car=""
max_horsepower=0

#Separate each data piece into its own category following the header
while IFS=',' read -r make model mpg cylinders displacement horsepower weight acceleration model2 origin; do	
	#This check skips the header
	if [ "$horsepower" != "Horsepower" ]; then
		if [ $(echo "$horsepower > $max_horsepower" | bc) == 1 ]; then
			max_horsepower_car="$make"
			max_horsepower="$horsepower"
		fi
	fi
done < cars.csv

echo "$max_horsepower_car $max_horsepower" > answer.txt
