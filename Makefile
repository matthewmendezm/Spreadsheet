all: spreadsheet_server.cpp
	g++ -std=c++0x spreadsheet_server.cpp spreadsheet_graph.cpp -lboost_regex