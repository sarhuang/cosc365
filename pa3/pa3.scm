;Name: Sarah Huang
;Date:
;Program: pa3.scm
;Purpose: Write functions from scratch (some are built-in) in scheme


;;list-len
;take a single list as a parameter and compute evaluate the number of elements in the list
(define (list-len l)
	(if (null? l)
		0
		(+ 1 (list-len (cdr l)))
	)
)


;;inc-list
;take a positive integer n as its only parameter and evaluate to a list containing every integer from 1 to n 
(define (inc-list n)
  ;if n = 0, make an empty list. else, append elements 1 to n.
  (if (= n 0)
	  (list)
	  (append (inc-list (- n 1)) (list n))
  )
)	




(define (rev-list l)
    #f ;; TODO: return something other than FALSE
    )

(define (my-map f l)
    #f ;; TODO: return something other than FALSE
    )

(define (merge-sort l)
    ;; Split a list into two halves, returned in a pair. You may uncomment this.
    (define (split l)
        (define (split-rec pair)
            (let ((front (car pair)) (back (cdr pair)))
                (if (>= (length front) (length back))
                    pair
                    (split-rec (cons (append front (list (car back))) (cdr back))))))
        (split-rec (cons (list (car l)) (cdr l))))

    #f ;; TODO: return something other than FALSE
    )
