package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"os"
	"reflect"
	"sort"
	"sync"
)

const filePath string = "data/Data2.json"
const resultsFilePath string = "data/Result.txt"

const workersCount int = 4

//const bufferSize int = 5

func main() {
	// Reads json data and parses it to Student array type
	data := ReadData(filePath)
	Students := ParseJsonData(data) //here value becomes 0 when i debug
	n := len(Students)
	// Counters for synchronization
	group := sync.WaitGroup{} //used to sync the go routines
	// if wait dont exist it wont give results because it'll run in background and wont print results
	workers := sync.WaitGroup{}
	// Channels initialization
	main := make(chan Student)     //not threads form of transfer
	dataChan := make(chan Student) //channels is synched automatically
	filtered := make(chan Student) //connects functions with main function
	results := make(chan Student)
	// Starts data array and results array management goroutines
	group.Add(2)
	go DWRoutine(n, main, dataChan, &group)      //implements concurrency go runf function in a green thread
	go ResultsRoutine(filtered, results, &group) //& is like ref

	// Starts workers
	workers.Add(workersCount) //4 threads
	for i := 0; i < workersCount; i++ {
		go WorkerRoutine(dataChan, filtered, &workers) // we call 4 worker routine concurrently

	}

	// Sends data items one by one to the data management goroutine
	for _, Student := range Students {
		main <- Student
	}

	// Closes channels for stopping goroutines
	close(main)
	workers.Wait()
	close(filtered)

	// Retrieves filtered Students from result goroutine
	var Studentsfiltered_list []Student
	for Student := range results {
		Studentsfiltered_list = append(Studentsfiltered_list, Student)
	}
	group.Wait()

	WriteResultsToFile(Students, Studentsfiltered_list, resultsFilePath)

}

// ResultsRoutine Manages results array and send data to the main goroutine
func ResultsRoutine(filtered chan Student, resultsChan chan Student, group *sync.WaitGroup) {
	defer close(resultsChan)
	defer group.Done() //says process is done , defer is used so it executes last

	var results []Student
	openChannel := false

	for {
		var casearray []reflect.SelectCase

		if !openChannel {
			casearray = append(casearray, reflect.SelectCase{
				Dir:  reflect.SelectRecv,
				Chan: reflect.ValueOf(filtered),
			})
		} else {
			casearray = append(casearray, reflect.SelectCase{
				Dir:  reflect.SelectRecv,
				Chan: reflect.ValueOf(nil),
			})
		}

		if len(results) > 0 {
			casearray = append(casearray, reflect.SelectCase{
				Dir: reflect.SelectDefault,
			})
		}

		// Loop exit condition
		if !casearray[0].Chan.IsValid() && len(casearray) == 1 {
			return
		}

		chosen, item, open := reflect.Select(casearray)
		switch chosen {
		case 0:
			if !open {
				openChannel = true
			} else {
				results = append(results, item.Interface().(Student))
			}
		default:
			if openChannel {
				resultsChan <- results[0]
				results = results[1:]
			}
		}
	}
}

// WorkerRoutine Filters given data and sends it to the results goroutine
func WorkerRoutine(dataChan chan Student, filtered chan Student, group *sync.WaitGroup) {
	defer group.Done()
	for Student := range dataChan {
		//FILTER
		if Student.Name[0] == 'C' {
			filtered <- Student
		}

	}
}

// DWRoutine Manages given data from the main and sends it to the workers
func DWRoutine(n int, main chan Student, dataChan chan Student, group *sync.WaitGroup) {
	defer close(dataChan)
	defer group.Done()
	var data []Student

	channelopen := false
	for {
		// Activates and deactivates channels
		var casearray []reflect.SelectCase
		// If data array is full deactivates channel from the main
		if len(data) <= int(n/2) && !channelopen { //waits for struct to fill half then remove and send to other workers
			casearray = append(casearray, reflect.SelectCase{
				Dir:  reflect.SelectRecv,
				Chan: reflect.ValueOf(main),
			})
		} else {
			casearray = append(casearray, reflect.SelectCase{
				Dir:  reflect.SelectRecv,
				Chan: reflect.ValueOf(nil),
			})
		}

		// If there is data in the data array, opens channel to workers
		if len(data) > 0 {
			casearray = append(casearray, reflect.SelectCase{
				Dir: reflect.SelectDefault, //opens channels to workers
			})
		}
		// Loop exit condition
		if !casearray[0].Chan.IsValid() && len(casearray) == 1 {
			return
		}
		// Dynamic select for managing channels
		chosen, item, open := reflect.Select(casearray)
		switch chosen {
		case 0:
			if !open {
				channelopen = true
			} else {
				data = append(data, item.Interface().(Student))
			}
		default:
			dataChan <- data[0]
			data = data[1:] //removes
		}
	}
}

// Parse json data to Student array
func ParseJsonData(jsonData []byte) []Student {
	var Students []Student

	err := json.Unmarshal([]byte(jsonData), &Students)
	if err != nil {
		log.Fatal(err)
	}

	return Students
}

// Reads data in bytes from given file
func ReadData(filePath string) []byte {
	data, err := ioutil.ReadFile(filePath)
	if err != nil {
		log.Fatal(err)
	}

	return data
}

type Student struct {
	Name  string
	Year  int
	Grade float32
}

func (c *Student) toString() string {
	fmt.Printf("%s %d %f\n", c.Name, c.Year, c.Grade)
	return fmt.Sprintf(" %20v %20v %20v ", c.Name, c.Year, c.Grade)
}

func (c *Student) getNumber() int {

	return c.Year
}
func WriteResultsToFile(originalData []Student, results []Student, path string) {
	file, err := os.OpenFile(path, os.O_CREATE|os.O_WRONLY, 0644)
	if err != nil {
		fmt.Printf("Failed writing to file %s", err)
	}

	dataWriter := bufio.NewWriter(file)
	_, _ = dataWriter.WriteString(fmt.Sprintf("%55v\n", "Main Data"))
	_, _ = dataWriter.WriteString(fmt.Sprintf("Total: %v", len(originalData)))
	_, _ = dataWriter.WriteString(fmt.Sprintf(" %20v %20v %20v \n", "Name", "year",
		"grade"))

	sort.Slice(originalData, func(i, j int) bool {
		return originalData[i].Name < originalData[j].Name
	})

	for _, Student := range originalData {
		_, _ = dataWriter.WriteString(Student.toString() + "\n")
	}

	sort.Slice(results, func(i, j int) bool {
		return results[i].getNumber() < results[j].getNumber()
	})
	_, _ = dataWriter.WriteString(fmt.Sprintf("\n\n%48v\n", "Filtered Data"))
	_, _ = dataWriter.WriteString(fmt.Sprintf(" %20v %20v %20v \n", "Name", "year", "grade"))

	for _, Student := range results {
		_, _ = dataWriter.WriteString(Student.toString() + "\n")
	}
	_, _ = dataWriter.WriteString(fmt.Sprintf("Total: %v", len(results)))
	_ = dataWriter.Flush()
	_ = file.Close()

	_, _ = dataWriter.WriteString(fmt.Sprintf("\n\n%48v\n", "empty Results"))

	_, _ = dataWriter.WriteString(fmt.Sprintf(" %20v %20v %20v \n", "Name", "year", "grade"))

	for _, Student := range results {
		_, _ = dataWriter.WriteString(Student.toString() + "\n")
	}

	_ = dataWriter.Flush()
	_ = file.Close()

}
