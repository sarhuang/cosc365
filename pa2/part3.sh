#Name: Sarah Huang
#Date: 3/14/23
#Program: part3.sh
#Purpose: Use awk - Process a CSV (comma-separated value) file with car statistics, producing a file called answer.txt that contains the make of the car with the highest horsepower and the actual horespower value

#!/usr/bin/env bash

# Your solution for part 3 goes here.

awk -F',' 'NR > 1 && $6 > max_horsepower { max_horsepower_car = $1; max_horsepower = $6 }
		   END { printf "%s %.6f\n", max_horsepower_car, max_horsepower > "answer.txt" }' < cars.csv
