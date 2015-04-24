/* 
 * Spreadsheet graph was made in collaboration for the Spreadsheet project
 * for CS 3505 Spring 2015
 *
 * Team Name: The Mangos
 *
 * Names:     Scott Wells
 *            Taylor Morris
 *            Matthew Mendez
 *            Jace Bleazard
 */

#include "spreadsheet_graph.h"

// Empty constructor & default constructor is fine.
spreadsheet_graph::spreadsheet_graph(){}

/* 
 * Returns how many cells in the graph are non-empty
 */
int spreadsheet_graph::size()
{
  return cells.size();
}

/*
 * Returns the cell -> value map that belongs to this graph (spreadsheet).
 * Only non-empty cells are included in the map.
 */
std::map<std::string, std::string> spreadsheet_graph::get_cells()
{
  return cells;
}

/*
 * Adds a specified value to a cell. The "key" is the cell name
 * and the "value" is the value to be put in the cell. Circlular
 * dependency checks are made to make sure the add is valid. Also,
 * necessary "undo" information is added to the undo stack.
 */
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

  // If the current value of the cell (previous to adding) is empty,
  // then an empty string as associated with the cell on the undo
  // stack. Otherwise, the current cell value is associated with
  // the cell on the undo stack.
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

  // If there were no circuluar dependencies, it is safe to add to the graph.
  cells[key] = value;
  return true;
}

/*
 * Driver method -- needed since value is not in the table yet
 */
bool spreadsheet_graph::circular_dependency_check(std::string key, std::string value)
{
  // Iterate over each cell that this cell will be dependent on. 
  std::vector<std::string> dependents = parse_formula(value);
  
  // This for loop is the "driver" for every necessary path to take recursively.
  for (int i = 0; i < dependents.size(); i++)
  {
    // If the current dependent is the same as the key, or if the key comes up somewhere
    // down the recursive path starting at current dependent, there is a circular dependency.
    if(key == dependents[i] || circular_recursive(key, dependents[i]))
      return true;
  }
  // If the function makes it here, there is no circular dependency.
  return false;
}

/*
 * Recursive method to check values already stored in the table. Kicked off by
 * the circular_dependency_check driver funtion.
 */
bool spreadsheet_graph::circular_recursive(std::string check, std::string current)
{
  // Iterate over each cell "check" is dependent on.
  std::vector<std::string> dependents = parse_formula(cells[current]);
  for (int i = 0; i < dependents.size(); i++)
    {
      // If "check" comes up in its direct or recursive dependencies, there
      // is a circular dependency.
      if(check == dependents[i] || circular_recursive(check, dependents[i]))
	return true;
    }
  return false;
}

/*
 * Returns the cell names from a formula value. e.g., if the formula was 
 * "=A5 - 3 + b6", this function would return a vector with the string values
 * "A5" and "b6".
 */
std::vector<std::string> spreadsheet_graph::parse_formula(std::string value)
{
  std::vector<std::string> result;
  // check every character in the string
  for (int i = 0; i < value.length(); i++)
    {
      // range A - Z
      if (value[i] <= 90 && value[i] >= 65)
	{
	  // Since there is guaranteed to be a number after we can assume
	  // that there is, and create a substring of the letter + first number.
	  std::string str = value.substr(i, 2);

	  // Check if there is another number after the first number. If there is
	  // the cell has a double digit e.g. "A67".
	  if(i < value.length() - 1)
	    {
	      i++; i++; // Advanced one spot passsed the substring we just made above.
	      // range 0 - 9
	      if(value[i] <= 57 && value[i] >= 48)
		str += value[i];
	    }
	  result.push_back(str);
	}
    }
  return result;
}

/*
 * Returns the state of the cell that was last changed before
 * it was changed. Also updates the graph to its previous state
 * and pops the underlying undo stack so that the next "undo" is on top.
 */
std::string spreadsheet_graph::undo()
{
  if(undo_stack.size() == 0)
    return "error 0 nothing to undo";

  // The information about the last changed cell is on top of the
  // stack. The stack consists of state pairs<key, value>.
  std::string key = undo_stack.top().first;
  std::string value = undo_stack.top().second;
  undo_stack.pop();
	
  // Update the graph.
  cells[key] = value;

  std::string result = "cell " + key + " " + value;
  return result;
}

/*
 * This function clears the undo stack.
 */
void spreadsheet_graph::reset_undo()
{
  while(undo_stack.size() > 0)
    undo_stack.pop();
}

