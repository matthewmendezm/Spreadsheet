#include "spreadsheet_graph.h"
spreadsheet_graph::spreadsheet_graph()
{

}

int spreadsheet_graph::size()
{
	return cells.size();
}

std::map<std::string, std::string> spreadsheet_graph::get_cells()
{
	return cells;
}

bool spreadsheet_graph::add(std::string key, std::string value)
{
	//std::cout << "ADD: Key " << key << " Value " << value << std::endl;
	for (int i = 0; i < key.length(); i++)
		key[i] = toupper(key[i]);
	if(value[0] == '=')
	{
		// normalize formula
		for (int i = 0; i < value.length(); i++)
			value[i] = toupper(value[i]);

		// check for circular dependency
		if(circular_dependency_check(key, value))
			return false;
	}

	if(cells.find(key) == cells.end())
	{
		std::pair<std::string, std::string> temp(key, "");
		undo_stack.push(temp);
	}
	else
	{
		std::pair<std::string, std::string> temp(key, cells[key]);
		undo_stack.push(temp);
	}

	cells[key] = value;
	return true;
}
// Driver method -- needed since value is not in the table yet
bool spreadsheet_graph::circular_dependency_check(std::string key, std::string value)
{
	std::vector<std::string> dependents = parse_formula(value);
	for (int i = 0; i < dependents.size(); i++)
	{
		if(key == dependents[i] || circular_recursive(key, dependents[i]))
			return true;
	}
	return false;
}

// Recursive part -- to check values stored in the table
bool spreadsheet_graph::circular_recursive(std::string check, std::string current)
{
	std::vector<std::string> dependents = parse_formula(cells[current]);
	for (int i = 0; i < dependents.size(); i++)
	{
		if(check == dependents[i] || circular_recursive(check, dependents[i]))
			return true;
	}
	return false;
}

std::vector<std::string> spreadsheet_graph::parse_formula(std::string value)
{
	std::vector<std::string> result;
	// check every character in the string
	for (int i = 0; i < value.length(); i++)
	{
		// range A - Z
		if (value[i] <= 90 && value[i] >= 65)
		{
			std::string str = value.substr(i, 2);
			if(i < value.length() - 1)
			{
				i++; i++;
				// range 0 - 9
				if(value[i] <= 57 && value[i] >= 48)
					str += value[i];
			}
			result.push_back(str);
		}
	}
	return result;
}

std::string spreadsheet_graph::undo()
{
	if(undo_stack.size() == 0)
		return "error 0 nothing to undo";
	std::string key = undo_stack.top().first;
	std::string value = undo_stack.top().second;
	undo_stack.pop();
	
	cells[key] = value;

	std::string result = "cell " + key + " " + value;
	return result;
}

void spreadsheet_graph::reset_undo()
{
	while(undo_stack.size() > 0)
		undo_stack.pop();
}

