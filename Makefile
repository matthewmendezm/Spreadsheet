all: spreadsheet_server.cpp
	g++ spreadsheet_server.cpp spreadsheet_graph.cpp -pthread -o server.o

clean:
	rm *.bin; rm *.o

run:
	g++ spreadsheet_server.cpp spreadsheet_graph.cpp -pthread -o server.o; ./server.o