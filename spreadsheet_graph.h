#ifndef SPREADSHEET_GRAPH_H
#define SPREADSHEET_GRAPH_H

#include <map>
#include <string>

class spreadsheet_graph
{
	public:
		spreadsheet_graph();
		~spreadsheet_graph(){};
		bool add(std::string key, std::string value);
	private:
		std::map<std::string, std::string> cells;
		bool circular_dependency_check(std::string key, std::string value);
};

#endif