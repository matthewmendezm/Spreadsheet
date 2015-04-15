#include "spreadsheet_graph.h"

spreadsheet_graph::spreadsheet_graph()
{
	cells["hi"] = "dad";
	std::cout << "Constructor touched" << std::endl;
}

bool spreadsheet_graph::add(std::string key, std::string value)
{
	std::cout << "ADD: Key " << key << " Value " << value << std::endl;
	if(circular_dependency_check(key, value))
		return false;
	cells[key] = value;
	return true;
}

bool spreadsheet_graph::circular_dependency_check(std::string key, std::string value)
{
	std::vector<std::string> dependents = parse_formula(value);
	return false;
}

std::vector<std::string> spreadsheet_graph::parse_formula(std::string value)
{
	std::cout << "now in parse formula" << std::endl;
	boost::regex expression("^[(A-Z)|(a-z)][1-9]?[1-9]$");
	boost::sregex_iterator it(value.begin(), value.end(), expression);
    boost::sregex_iterator end;

    for (; it != end; ++it) 
    {
        std::cout << it->str() << "\n";
	}

	std::vector<std::string> result;
	return result;
}

