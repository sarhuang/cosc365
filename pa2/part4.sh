#Name: Sarah Huang
#Date: 3/14/23
#Program: part4.sh
#Purpose: Extract the sequence samples from each file and put them back in the correct order so that you can perform some analysis on the whole sequence

#!/usr/bin/env bash

# Your solution for part 4 goes here.

#Create combined sequence
#Use awk to print only the position followed by the sequence data on one line for each file
#Use sort to get each sample in the correct order
#Use cut to remove the sequence position, leaving only the sequence data
#Use tr to remove newlines so all data is on one line
for file in *.sample; do
  awk '/sequence position/ {pos=$3} /sequence data/ {print pos,$3}' "$file"
done | sort -n | cut -d " " -f 2- | tr -d "\n" > sequence.txt



#Pair count
#Use fold to split the sequence file such that each character has its own line
#Use awk to:
	#Use an associate array to count each occurrence of each base pair
	#Print the four base pairs ATCG followed by their count on separate lines
fold -w1 sequence.txt | awk '{ array[$1]++ } 
	END { print "A "array["A"]"\nT "array["T"]"\nC "array["C"]"\nG "array["G"] }' > pairs.txt



#Codon distribution
#Use fold to split the sequence file such that every three characters is on its own line
#Use awk to:
	#Use an associative array to count each occurrence of each codon
	#Keep a running total of the number of codons
	#Print each unique codon followed by its distribution in the file as a percentage
	#Sort the output by codon name
fold -w3 sequence.txt | awk '{
  if (length($1) == 3) {
    array[$1]++
    total++
  }
} END {
  for (codon in array) {
    printf "%s %.6f%%\n", codon, (array[codon]/total)*100
  }
}' | sort > codons.txt
