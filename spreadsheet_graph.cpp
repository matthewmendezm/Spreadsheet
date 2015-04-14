#include "spreadsheet_graph.h"
#include <iostream>

spreadsheet_graph::spreadsheet_graph()
{
	cells["hi"] = "dad";
	std::cout << "Constructor touched" << std::endl;
}

bool spreadsheet_graph::add(std::string key, std::string value)
{
	if(circular_dependency_check(key, value))
		return false;
	cells[key] = value;
	return true;
}

bool spreadsheet_graph::circular_dependency_check(std::string key, std::string value)
{
	return false;
}