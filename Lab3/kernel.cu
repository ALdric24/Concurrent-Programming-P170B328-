#include "cuda_runtime.h"
#include <cuda.h>
#include "device_launch_parameters.h"
#include <fstream>
#include "iostream"

//using namespace std;

typedef struct {
    char* name;
    int year;
    float grade;
} Student;

typedef struct {
    Student students[200];
    int numStudents;
} data_t;

//typedef struct {
//    char students[200];
//   
//} result;

char* file_buffer;
size_t file_size;
//__global__ void addResult(data_t data, data_t result);


void parseData(data_t& data, const char* file_buffer, size_t file_size) {

    data.numStudents = 200;
    char temp_name[256];
    int temp_year = 0;
    float temp_grade = 0.0;


    int i = 0;
    while (sscanf(file_buffer, "%[^,],%d,%f", temp_name, &temp_year, &temp_grade) == 3) {
        // Store the parsed data in the students array
        data.students[i].name = (char*)malloc(strlen(temp_name) + 1);
        strcpy(data.students[i].name, temp_name);
        data.students[i].year = temp_year;
        data.students[i].grade = temp_grade;

        printf("Name: %s, Score: %d, Score2: %f\n", data.students[i].name, data.students[i].year, data.students[i].grade);


        file_buffer = strchr(file_buffer, '\n') + 1;
        i++;
    }

    if (i < data.numStudents) {
        fprintf(stderr, "Error: failed to parse data from file buffer\n");
        return;
    }
}

data_t readfile(const char* textfile) {
    
    char* file_buffer;
    size_t file_size;
    FILE* file = fopen("data.txt", "r");
    fseek(file, 0, SEEK_END);
    file_size = ftell(file);
    rewind(file);
    file_buffer = static_cast<char*>(malloc(file_size));
    fread(file_buffer, 1, file_size, file);
    fclose(file);

    data_t data;
    data.numStudents = 200;
    parseData(data, file_buffer, file_size);

   
    free(file_buffer);

    return data;
}



void writeResultToFile(data_t result, const char* filename) {
	std::ofstream out_file(filename);
    if (out_file.is_open()) {
        for (int i = 0; i < result.numStudents; i++) {
            Student student = result.students[i];
            out_file << student.name << ", " << student.year << ", " << student.grade << std::endl;
        }
        out_file.close();
    }
}
__device__ bool checkNameFirstLetter(char letter) {

    return letter == 'C';
}
__global__ void addResult(data_t result) {
    int idx = blockIdx.x * blockDim.x + threadIdx.x;

    if (idx < result.numStudents) {
        Student student = result.students[idx];
        if (checkNameFirstLetter(student.name[0])) {
            // Convert name to uppercase
            int name_len = strlen(student.name);
            char* name = (char*)malloc(name_len + 1);
            for (int i = 0; i < name_len; i++) {
                name[i] = student.name[i];
                if (name[i] >= 'a' && name[i] <= 'z') {
                    name[i] = name[i] - 'a' + 'A';
                }
            }
            result.students[idx].name = name;
        }
        else {
            // Remove element from result data
            for (int i = idx; i < result.numStudents - 1; i++) {
                result.students[i] = result.students[i + 1];
            }
            result.numStudents = atomicAdd(&result.numStudents, -1);
        }
    }
}

//__global__ void addResult(data_t* data, data_t* result) {
//
//    int idx = blockIdx.x * blockDim.x + threadIdx.x;
//
//
//    if (idx < data->numStudents) {
//
//        Student student = data->students[idx];
//
//
//        if (checkNameFirstLetter(student.name[0])) {
//
//            int numStudents = atomicAdd(&result->numStudents, 1);
//
//            result->students[numStudents].name = student.name;
//            result->students[numStudents].year = student.year;
//            result->students[numStudents].grade = student.grade;
//        }
//    }
//}
int main() {
    // Read data from file and copy to device memory
    data_t h_data = readfile("data.txt");
    data_t* d_data;
    cudaMalloc(&d_data, sizeof(data_t));
    cudaMemcpy(d_data, &h_data, sizeof(data_t), cudaMemcpyHostToDevice);

    // Allocate and initialize result data on device memory
    data_t* d_result;
    cudaMalloc(&d_result, sizeof(data_t));
    data_t h_result = { 0 };
    cudaMemcpy(d_result, &h_data, sizeof(data_t), cudaMemcpyHostToDevice);

    // Launch kernel to process data and store result
    addResult <<<2, 32 >>> (*d_result);

    // Copy result data from device memory to host memory
    cudaMemcpy(&h_result, d_result, sizeof(data_t), cudaMemcpyDeviceToHost);

    // Print result data
    for (int i = 0; i < h_result.numStudents; i++) {
        Student student = h_result.students[i];
        std::cout << student.name << ", " << student.year << ", " << student.grade << std::endl;
    }

    // Write result data to file
    writeResultToFile(h_result, "result.txt");

    // Free device memory
    cudaFree(d_data);
    cudaFree(d_result);

    return 0;
}

//int main() {
//    data_t data = readfile("data.txt");
//    data_t result;
//    cudaMalloc(&result, sizeof(data_t));
//
//    addResult <<<2, 32>>> (&data, &result);
//
//    writeResultToFile(result, "result.txt");
//
//    cudaFree(result);
//    for (int i = 0; i < data.numStudents; i++) {
//        free(data.students[i].name);
//    }
//
//    return 0;
//}
/* data_t* result;
    cudaMalloc(&result, sizeof(data_t));
    cudaMemset(result, 0, sizeof(data_t));*/

    //addResult <<<2, 32>>> (data,result);

   /* data_t h_result;
    cudaMemcpy(&h_result, result, sizeof(data_t), cudaMemcpyDeviceToHost);*/
    /* for (int i = 0; i < h_result.numStudents; i++) {
           Student student = h_result.students[i];
           std::cout << student.name << ", " << student.year << ", " << student.grade << std::endl;
           free(student.name);
       }*/