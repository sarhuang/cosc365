/* Name: Sarah Huang
 * Date: 5/ /23
 * Program: concp.go
 * Purpose: Use Go's concurrency tools to copy multiple files simultaneously
 */


package main
import (
    "fmt"
	"io"
	"os"
	"path/filepath"
	"sync"
)


func cp(name, destination string, waitGroup *sync.WaitGroup, errorChannel chan<-error){
    defer waitGroup.Done();

	fileInfo, err := os.Stat(name)
	if(err != nil){
		errorChannel<-err;
		return;
	}
	if(!fileInfo.Mode().IsRegular()){
		errorChannel<-fmt.Errorf("'%s' is an invalid file\n", name);
		return;
	}

	srcFile, err := os.Open(name);
	if(err != nil){
		errorChannel<-err;
		return;
	}
	defer srcFile.Close();

	newFile, err := os.Create(filepath.Join(destination, filepath.Base(name)));
	if(err != nil){
		errorChannel<-err;
		return;
	}
	defer newFile.Close();

	_, err = io.Copy(newFile, srcFile);
	if(err != nil){
		errorChannel<-err;
		return;
	}
}



//go build hello.go
// run ./hello
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


	errorChannel := make(chan error)

	var waitGroup sync.WaitGroup;

	for _, name := range os.Args[1:len(os.Args)-1]{
		waitGroup.Add(1);
		go cp(name, destination, &waitGroup, errorChannel);
	}

	doneChannel := make(chan struct{});
	go func(){
		waitGroup.Wait();
		close(doneChannel);
	}()

	select{
		case <-doneChannel:
			fmt.Printf("All files copied\n");
		case err := <-errorChannel:
			fmt.Printf("Error during copy: '%s'\n", err);
	}

	close(errorChannel);
}
