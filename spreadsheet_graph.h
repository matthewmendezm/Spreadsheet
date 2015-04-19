#ifndef SPREADSHEET_GRAPH_H
#define SPREADSHEET_GRAPH_H

#include <map>
#include <string>
#include <iostream>
#include <vector>
#include <boost/regex.hpp>
#include <cctype> // toupper

class spreadsheet_graph
{
	public:
		spreadsheet_graph();
		~spreadsheet_graph(){};
		bool add(std::string key, std::string value);
		std::map<std::string, std::string> get_cells();
		int size();

	private:
		std::map<std::string, std::string> cells;
		bool circular_dependency_check(std::string key, std::string value);
		std::vector<std::string> parse_formula(std::string value);
		bool circular_recursive(std::string check, std::string key);
};

#endif