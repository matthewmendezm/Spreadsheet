all: spreadsheet_server.cpp
	g++ spreadsheet_server.cpp spreadsheet_graph.cpp -pthread