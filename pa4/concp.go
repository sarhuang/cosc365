/* Name: Sarah Huang
 * Date: 5/6/23
 * Program: concp.go
 * Purpose: Use Go's concurrency tools to copy multiple files simultaneously
 
 * How to run Go program:
 * go build concp.go
 * ./concp _ _ _
 */


package main
import (
    "fmt"
	"io"
	"os"
	"path/filepath"
	"sync"
)

/*Creates a copy of the given file using Read() and Write()
 * name = name of source file will be copied
 * destination = destination path where the file should be copied
 * waitGroup = pointer to sync.WaitGroup to track completion of goroutine
 * errorChannel = channel reports any errors
 */
func cp(name, destination string, errorChannel chan<-error, waitGroup *sync.WaitGroup){
    //Ensure to decrement the wait group counter when function is done
	defer waitGroup.Done();

	//Error check - source file doesn't exist
	fileInfo, err := os.Stat(name)
	if(os.IsNotExist(err)){
		errorChannel <- err;
		return;
	}
	//Error check - source file isn't a regular file
	if(!fileInfo.Mode().IsRegular()){
		errorChannel <- fmt.Errorf("'%s' is an invalid file\n", name);
		return;
	}
	//Open source file (hopefully successfully)
	srcFile, err := os.Open(name);
	if(err != nil){
		errorChannel <- err;
		return;
	}
	defer srcFile.Close(); //source file will close at end


	//Create new file
	newFile, err := os.Create(filepath.Join(destination, filepath.Base(name)));
	if(err != nil){
		errorChannel <- err;
		return;
	}
	defer newFile.Close(); //newly created file will close at end


	//Read and write to make new file
	buffer := make([]byte, 1024);
	for true{
		bytesRead, err := srcFile.Read(buffer);
		//Error check - read isn't successful
		if(err != nil && err != io.EOF){
			errorChannel <- err;
			return;
		}
		//Break out of loop once done reading/writing file
		if(bytesRead == 0){
			break;
		}

		//The _ varible signifies we don't need a return value
		//Write only however many bytes were read
		_, err = newFile.Write(buffer[:bytesRead]);
		//Error check - create isn't successful
		if(err != nil){
			errorChannel <- err;
			return;
		}
	}
}



func main(){
	//Error check - minimum # of arguments to run program
	if(len(os.Args) < 3){
		fmt.Printf("Usage: concp file1 ... destination_directory\n");
		os.Exit(1);
	}

	//Error check - directory must exist beforehand
	destination := os.Args[len(os.Args)-1];
	fileInfo, err := os.Stat(destination);
	if(err != nil || !fileInfo.IsDir()){
		fmt.Printf("Invalid destination directory\n");
		os.Exit(1);
	}


	/*
	* errorChannel = Channel for reporting errors
	* watiGroup = Waits for all copying goroutines to complete
	* doneChannel = Channel for waiting for all the copying goroutines to finish
	*/
	errorChannel := make(chan error);
	var waitGroup sync.WaitGroup;
	//Use one goroutine per file to do the concurrent copying
	for _, name := range os.Args[1:len(os.Args)-1]{
		waitGroup.Add(1);
		go cp(name, destination, errorChannel, &waitGroup);
	}

	//Another channel dedicated for waiting allows main thread to also handle errors
	wgDoneChannel := make(chan bool);
	go func(){
		waitGroup.Wait();
		close(wgDoneChannel);
	}()

	//Listening to errors or the WaitGroup to complete
	select{
		case err := <-errorChannel:
			fmt.Printf("%s\n", err);
		case <- wgDoneChannel:
			fmt.Printf("All files successfully copied\n");
	}
	close(errorChannel);
}
