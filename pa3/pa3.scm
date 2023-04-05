;Name: Sarah Huang
;Date: 4/4/23
;Program: pa3.scm
;Purpose: Write functions from scratch (some are built-in) in scheme


;;list-len
;take a single list as a parameter and compute evaluate the number of elements in the list
(define (list-len l)
	;if list length = 0, return 0. else, recursively add 1 and return the total number of elements
	(if (null? l)
		0
		(+ 1 (list-len (cdr l)))
	)
)


;;inc-list
;take a positive integer n as its only parameter and evaluate to a list containing every integer from 1 to n 
(define (inc-list n)
  ;if n = 0, make an empty list. else, recursively append elements 1 to n.
  (if (= n 0)
	  (list)
	  ;in order for append to work, they both need to be lists
	  (append (inc-list (- n 1)) (list n))
  )
)	


;;rev-list
;takes a list as its only parameter and evaluates to the reverse of the list
(define (rev-list l)
  (define (iterate origList reversedList)
	;returns the final reversed list whenever it goes through all the list elements
	(if (null? origList)
		reversedList	
		;recursively put the first element of the list in the front of the new reversed list
		;cdr l takes out the element that got added to the reversedList
		(iterate (cdr origList) (append (list (car origList)) reversedList))
	)
  )
  ;this is where it starts
  (iterate l (list))  
)


;;my-map
;take a function and a single list as parameters. It will apply the function to each element
;of the list and produce a new list containing the results of those function applications.
(define (my-map f l)
  (if (null? l)
	  (list)
	  (append (list (apply f (list (car l)))) (my-map f (cdr l)))
  )  
)


;;merge-sort
;takes a list as its only parameter and evaluates to the same list in sorted order
(define (merge-sort l)
    ;; Split a list into two halves, returned in a pair. You may uncomment this.
    (define (split l)
        (define (split-rec pair)
            (let ((front (car pair)) (back (cdr pair)))
                (if (>= (length front) (length back))
                    pair
                    (split-rec (cons (append front (list (car back))) (cdr back))))))
        (split-rec (cons (list (car l)) (cdr l))))

    ;;merge the two halves into a single sorted list.
    (define (merge list1 list2)
        ;if list1 is null, return list2
		(cond ((null? list1) 
		    list2
		)
		;if list2 is null, return list1
		((null? list2) 
		    list1
		)
		;if 1st element of list1 > 1st element of list2, add list2 1st element to result of merging list1 and remainder of list2
		((> (car list1) (car list2))
			(append (list (car list2)) (merge (cdr list2) list1))
		)
		;add 1st element of list1 to result of merging list2 and remainder of list1
		(else (append (list (car list1)) (merge (cdr list1) list2)))
		)
	)

    ;;recursively sort and merge the two halves of the list.
	(if (<= (length l) 1)
        l	;the one element list doesn't need to be merge sorted
    
		(let* ((halves (split l)) 
			(front (car halves)) 
			(back (cdr halves)))
            (merge (merge-sort front) (merge-sort back))
		)
	)
)
