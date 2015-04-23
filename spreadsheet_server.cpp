#include "spreadsheet_server.h"
void *get_in_addr(struct sockaddr *sa);
std::string int_to_string(int a);

// Global spreadsheet instance. Okay since there will only be one spreadsheet
// object created when the program is run.
spreadsheet_server * server = new spreadsheet_server();

int main(int argc, char const *argv[])
{
  // Simply kick off the server listening for connections (the servers main running state). 
  std::cout << "Spreadsheet server created" << std::endl;
  server->listen_for_connections();
  return 0;
}

/* 
 * Static method for thread initialization. Required by the pthread_create function.
 */
void *thread_init(void * arg)
{
  // The parameter will be an integer specifying the connected client's socket.
  int * socket;
  socket = (int *) arg;

  std::cout << "New thread created for socket " + int_to_string((*socket)) << std::endl;

  // Begin listening to messages from the new client on the new thread.
  server->listen_to_client(*socket);
}

/*
 * Helper method to convert an integer to a string.
 */
std::string int_to_string(int a)
{
  std::string result;
  std::stringstream out;
  out << a;
  return out.str();
}

/*
 * Constructor initializes all pointed to structures 
 * on the heap and opens the .bin file which holds state
 * of registered users and spreadsheet information.
 */
spreadsheet_server::spreadsheet_server()
{
  spreadsheets = new spreadsheet_map;
  spreadsheet_clients = new spreadsheet_client_map;
  socket_spreadsheet = new socket_spreadsheet_map;
  users = new std::vector<std::string>;
  (*users).push_back("sysadmin");
  open(); // Populate server with necessary state information.
}

/*
 * Destructor cleans up heap memory.
 */
spreadsheet_server::~spreadsheet_server()
{
  delete spreadsheets;
  delete spreadsheet_clients;
  delete socket_spreadsheet;
  delete users;
}

/*
 * Saves the registered user names and non-empty cells to file. 
 */
void spreadsheet_server::save()
{
  // create spreadsheet file data on disk
  std::ofstream file_out ("sprd_data.bin", std::ofstream::out | std::ofstream::trunc);

  // Iterate over users and add them to file in the format "user johnnyboy\n" 
 std::vector<std::string>::iterator it;
  for(it = users->begin(); it != users->end(); it++)
    if((*it) != "sysadmin")
      file_out << "user " + (*it) + "\n";

  // Iterate over the spreadsheets, add a header to file in the format "spreadsheet MangoSheet"
  // and then add all non-empty cells in that spreadsheet underneath it in the form "cell A1 worm". 
  spreadsheet_map::iterator iter;
  for(iter = spreadsheets->begin(); iter != spreadsheets->end(); iter++)
  {
    file_out << "spreadsheet " + (*iter).first + "\n";
    std::map<std::string, std::string> cells = (*iter).second->get_cells();
    std::map<std::string, std::string>::iterator cells_it;
    for(cells_it = cells.begin(); cells_it != cells.end(); cells_it++)
      file_out << "cell " + (*cells_it).first + " " + (*cells_it).second + "\n";
  }
  file_out.close();
}

/*
 * Populate the spreadsheet uers and spreadsheet graphs with data from sprd_data.bin.
 */
void spreadsheet_server::open()
{
  // Open the file 
  std::ifstream file_in ("sprd_data.bin", std::ifstream::in);
  std::string line = "";
  std::string current_sheet = "";
  char c = file_in.get();

  // When the file has remaining characters, keep going
  while(file_in.good())
  {
    // Iterate over each character, adding to "line" until
    // a new line comes along, then process the line.
    if(c != '\n')
      line += c;
    else
    {
      // Parse the line separated by spaces
      std::vector<std::string> v = parse_command(line.substr(0, line.length()));
      // Add the second string from all lines that start with "user" to users
      if(v.at(0) == "user")
        users->push_back(v.at(1));
      // Process "spreadsheet" line
      else if(v.at(0) == "spreadsheet")
      {
        // In this case, all of the cells associated with current_sheet have
        // been added, now clear the undo stack.
        if(current_sheet != "")
          (*spreadsheets)[current_sheet]->reset_undo();
        // Advance current sheet and instantiate a new graph.
        current_sheet = v.at(1);
        (*spreadsheets)[current_sheet] = new spreadsheet_graph();
      }
      // Add cell to the current_sheet
      else if(v.at(0) == "cell")
        (*spreadsheets)[current_sheet]->add(v.at(1), v.at(2));

      line = "";
    }
    c = file_in.get();
  }
  // Clear undo stack for the final sheet in the file
  if(current_sheet != "")
      (*spreadsheets)[current_sheet]->reset_undo();
  file_in.close();
}

/*
 * Kicks off the server listening and accepting connections.
 * When a connection is made, the a new thread is made to listen
 * for messages from the client over the socket.  
 */
void spreadsheet_server::listen_for_connections()
{
  int sockfd, new_fd;  // listen on sock_fd, new connection on new_fd
  struct addrinfo hints, *servinfo, *p;
  struct sockaddr_storage client_addr; // connector's address information
  socklen_t sin_size;
  struct sigaction sa;
  int yes=1;
  char s[INET6_ADDRSTRLEN];
  int rv;

  memset(&hints, 0, sizeof hints);
  hints.ai_family = AF_UNSPEC;
  hints.ai_socktype = SOCK_STREAM;
  hints.ai_flags = AI_PASSIVE; // use my IP

  if ((rv = getaddrinfo(NULL, "2112", &hints, &servinfo)) != 0) {
      fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(rv));
      return;
  }

  // loop through all the results and bind to the first we can
  for(p = servinfo; p != NULL; p = p->ai_next) {
      if ((sockfd = socket(p->ai_family, p->ai_socktype,
              p->ai_protocol)) == -1) {
          perror("server: socket");
          continue;
      }

      if (setsockopt(sockfd, SOL_SOCKET, SO_REUSEADDR, &yes,
              sizeof(int)) == -1) {
          perror("setsockopt");
          exit(1);
      }

      // Bind the server socket to listen to this IP and port
      if (bind(sockfd, p->ai_addr, p->ai_addrlen) == -1) {
          close(sockfd);
          perror("server: bind");
          continue;
      }
      break;
  }

  if (p == NULL)  {
      fprintf(stderr, "server: failed to bind\n");
      return;
  }

  freeaddrinfo(servinfo); // all done with this structure

  if (listen(sockfd, PENDINGCONNECTIONS) == -1) {
      perror("listen");
      exit(1);
  }

  printf("Waiting for connection from clients...\n");

   // main accept() loop //put while loop back HERE
  while(true)
  {
  	
    sin_size = sizeof client_addr;

    // Accept call is BLOCKING until a client connects
    new_fd = accept(sockfd, (struct sockaddr *)&client_addr, &sin_size);
    if (new_fd == -1) {
        perror("accept");
        continue;
    }

    inet_ntop(client_addr.ss_family, get_in_addr((struct sockaddr *)&client_addr), s, sizeof s);
    printf("server: got connection from %s\n", s);

    // create new thread to handle this connection on this socket.
    pthread_t new_thread;
    int ret;
    int * socket_ID = & new_fd;
    ret = pthread_create(&new_thread, NULL, &thread_init, socket_ID); 
    if(ret != 0) 
    { 
      printf("Error creating new pthread\n"); 
      exit(EXIT_FAILURE); 
    }
  }
  return;
}

/*
 * 
 */
void *get_in_addr(struct sockaddr *sa)
{
    if (sa->sa_family == AF_INET) {
        return &(((struct sockaddr_in*)sa)->sin_addr);
    }

    return &(((struct sockaddr_in6*)sa)->sin6_addr);
}

/*
 *
 */ 
void spreadsheet_server::listen_to_client(int socket)
{
  bool registered = false;
	std::string temp = "";
  int received;
	while(1)
	{
  		char msg[1];
  		received = recv(socket, msg, 1, 0);
      if(received < 1)
        break;

  		if(msg[0] == '\n')
  		{
  			process_request(socket, temp, &registered);
  			temp = "";
  		}
  		else
  			temp += msg[0];
	}		
  	close(socket);
    std::cout << "client disconnected" << std::endl;
    server->save();
    pthread_exit(NULL);
}

/*
 * Takes in the message sent from the client,  figures out what
 * the message is, and call the correct function associated with
 * that message.
 */
void spreadsheet_server::process_request(int socket, std::string input, bool * registered)
{	
  std::vector<std::string> v = parse_command(input);
  if(v[0] == "connect")
    process_connect(socket, v, registered);
  else if(v.at(0) == "register" && *registered)
    process_register(socket, v);
  else if(v.at(0) == "cell" && *registered)
    process_cell(socket, v, input);
  else if(v.at(0) == "undo")
    process_undo(socket, v);
  else // Junk recieved... send error 2
    send_message(socket, "error 2 " + input);
}

/*
 * As long as the specified user (index 1 of the vector) is a registered user, 
 * allow them to connect, open the specified spreadsheet (index 2) and associate
 * the user with the spreadsheet graph so they can do things to it.
 */
void spreadsheet_server::process_connect(int socket, std::vector<std::string> v, bool * registered)
{
  if(std::find((*users).begin(), (*users).end(), v.at(1)) == (*users).end())
    {
      std::cout << " Sent error 4 in connect" << std::endl;
      send_message(socket, "error 4 " + v.at(1));
      return;
    }  

    // This flag is used in other functions to make sure that 
    // a socket trying to make changes has been approved to 
    // make changes.
    *registered = true;

    // if not found, make new spreadv[0] == "connect"sheet
    if(spreadsheets->find(v.at(2)) == spreadsheets->end())
    {
      std::cout << "New spreadsheet made with name " + v.at(2) << std::endl;
      // Make a new spreadsheet, add it to servers current spreadsheets
      (*spreadsheets)[v.at(2)] = new spreadsheet_graph();
      std::vector<int> new_vector; 
      (*spreadsheet_clients)[v.at(2)] = new_vector;
    }

    // Add the new client's socket to the associated spreadsheet_clients list
    (*spreadsheet_clients)[v.at(2)].push_back(socket);
       
    // Add the socket to the spreadsheet_graph -> socket list.
    (*socket_spreadsheet)[socket] = v.at(2);

    // Echo to the client that they are connected
    send_message(socket, "connected " + int_to_string((*spreadsheets)[v.at(2)]->size()));
    
    // Send all non-empty cell information to the new socket.
    std::map<std::string, std::string> cells_to_send = (*spreadsheets)[v.at(2)]->get_cells();
    std::map<std::string, std::string>::iterator it;
    for(it = cells_to_send.begin(); it != cells_to_send.end(); it++)
    {
      std::cout << "sending cell " + it->first + " " + it->second << std::endl;
      send_message(socket, "cell " + it->first + " " + it->second);
    }
}

/*
 * Add the newly registered user to the list of registered users
 * if they are not already on the list. Otherwise send an error.
 */
void spreadsheet_server::process_register(int socket, std::vector<std::string> v)
{
  if(std::find((*users).begin(), (*users).end(), v.at(1)) == (*users).end())
    {
      (*users).push_back(v.at(1));
      std::cout << "registered new user " + v.at(1) << std::endl;
    }
    else
      send_message(socket, "error 4 " + v[1]);
}

/*
 * Update the spreadsheet according to the cell name and cell value
 * that are the first and second indices of the vector parameter. 
 * Updating the spreadsheet involves editing the underlying spreadsheet
 * structure and sending the cell update message to all clients associated
 * with the spreadsheet.
 */
void spreadsheet_server::process_cell(int socket, std::vector<std::string> v, std::string input)
{
  // get the spreadsheet name
  std::string sheet_name = (*socket_spreadsheet)[socket];
  if((*spreadsheets)[sheet_name]->add(v.at(1), v.at(2)))
  {
    //std::cout << "sending updated cell value to all clients " + v.at(1) + " " + v.at(2) << std::endl;
    
    // iterate through all of the clients and send new cell value
    for(socket_list::iterator iterator = (*spreadsheet_clients)[sheet_name].begin();
      iterator != (*spreadsheet_clients)[sheet_name].end(); iterator++)
        send_message(*iterator, input); 
  }
  // If add returns false, there was a circular dependency.
  else
    send_message(socket, "error 1 CD");
}

/*
 * Processes the undo command by updating the underlying "undo" and "spreadsheet"
 * structures, and sending the cell that needs to be set by the undo to all 
 * clients associated with the same spreadsheet as the client who undid.
 */
void spreadsheet_server::process_undo(int socket, std::vector<std::string> v)
{
  // Find the spreadsheet name associated with this socket
  std::string sheet_name = (*socket_spreadsheet)[socket];

  // Get the cell command that needs to be sent to all clients
  std::string undo = (*spreadsheets)[(*socket_spreadsheet)[socket]]->undo();

  // Iterate through all clients associated with the spreadsheet and 
  // send them the cell update message.
  for(socket_list::iterator iterator = (*spreadsheet_clients)[sheet_name].begin();
    iterator != (*spreadsheet_clients)[sheet_name].end(); iterator++)
      send_message(*iterator, undo);
}

/*
 * Send a specified string over the specified socket with 
 * a newline character appended to the end.
 */
void spreadsheet_server::send_message(int socket, std::string temp)
{
  // Allocate enough space in the char array for the string + \n
	char * cstr = new char[temp.length() + 1];
	std::strcpy (cstr, temp.c_str());
	cstr[temp.length()] = '\n';

	send(socket, cstr, (temp.length() + 1), 0);
}

/*
 * Splits a string depending on its spaces. If there are no spaces,
 * returns the same single string in the vector. If there is one space,
 * returns two strings separated by that space in the vector. If there 
 * are two spaces or more, returns the string split up into three strings
 * divided by the first and second spaces. All remaining spaces are ignored.
 */
std::vector<std::string> spreadsheet_server::parse_command(std::string input)
{
  int first_space   = 0;
  int second_space  = 0;
  std::vector<std::string> result;

  // find positions of the first 2 empty spaces to parse string
  for (int i = 0; i < input.length(); i++)
  {
    if (input[i] == ' ')
      if(first_space == 0)
        first_space = i;
      else
      {
        second_space = i;
        break;
      }
  }

  // If there were no spaces, simply put the string back
  // in the vector as one.
  if (!first_space && !second_space)
    result.push_back(input);

  // If there was only one space, put the two space-separated
  // strings into the vector
  else if(!second_space)
  { 
    std::string first_one = input.substr(0, first_space);
    std::string second_one = input.substr(first_space + 1, input.length() - first_space - 1);
    result.push_back(first_one);
    result.push_back(second_one);
  }
  // If there were two spaces, put the three first-and-second-space
  // separated strings into the vector.
  else if(first_space && second_space)
  {
    result.push_back(input.substr(0, first_space));
    // cell name
    result.push_back(input.substr(first_space + 1, second_space - first_space - 1));
    // contents
    result.push_back(input.substr(second_space + 1, input.length() - second_space - 1));
  }
  return result;
}
