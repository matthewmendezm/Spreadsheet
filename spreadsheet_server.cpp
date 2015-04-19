#include "spreadsheet_server.h"
void *get_in_addr(struct sockaddr *sa);

// global variable spreadsheet
spreadsheet_server * server = new spreadsheet_server();

int main(int argc, char const *argv[])
{
	std::cout << "Spreadsheet server created" << std::endl;
	server->listen_for_connections();
	return 0;
}

// Static method for thread initialization
void *thread_init(void * arg)
{
  int * socket;
  socket = (int *) arg;
  std::cout << "now in thread init" << std::endl;
  server->listen_to_client(*socket);
}

spreadsheet_server::spreadsheet_server()
{
  spreadsheets = new spreadsheet_map;
  spreadsheet_clients = new spreadsheet_client_map;
  socket_spreadsheet = new socket_spreadsheet_map;
  users = new std::vector<std::string>;
  (*users).push_back("sysadmin");
}

spreadsheet_server::~spreadsheet_server()
{
  delete spreadsheets;
  delete spreadsheet_clients;
  delete socket_spreadsheet;
  delete users;
}
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

void *get_in_addr(struct sockaddr *sa)
{
    if (sa->sa_family == AF_INET) {
        return &(((struct sockaddr_in*)sa)->sin_addr);
    }

    return &(((struct sockaddr_in6*)sa)->sin6_addr);
}

void sigchld_handler(int s)
{
    while(waitpid(-1, NULL, WNOHANG) > 0);
}

// WE SHOULD RENAME THIS LISTEN_FOR_MESSAGE or something
void spreadsheet_server::listen_to_client(int socket)
{
  bool registered = false;
	std::string temp = "";
  int received;
	while(1)
	{
  		char msg[1];
  		received = recv(socket, msg, 1, 0);
      if(!received)
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
    pthread_exit(NULL);
}
void print_stuff(std::vector<std::string> * collection)
{
  std::cout << "printing stuff" << std::endl;
  for (int i = 0; i < collection->size(); i++)
  {
    std::cout << collection->at(i) << ": size of this entry" << collection->at(i).length() << std::endl;
  }
}

void spreadsheet_server::process_request(int socket, std::string input, bool * registered)
{	
  std::vector<std::string> v = parse_command(input);
  std::cout << v.at(1) << std::endl;

  if(v[0] == "connect")
  {
    if(std::find((*users).begin(), (*users).end(), v.at(1)) == (*users).end())
    {
      send_message(socket, "error 4 " + v.at(1));
      return;
    }  

    *registered = true;

    // if not found, make new spreadv[0] == "connect"sheet
    if(spreadsheets->find(v.at(2)) == spreadsheets->end())
    {
      // Make a new spreadsheet, add it to servers current spreadsheets
        std::cout << input << std::endl;
      (*spreadsheets)[v.at(2)] = new spreadsheet_graph();

      std::vector<int> new_vector; 
      (*spreadsheet_clients)[v.at(2)] = new_vector;
    }

    // Add the new client's socket to the associated spreadsheet_clients list
    (*spreadsheet_clients)[v.at(2)].push_back(socket);
       
    (*socket_spreadsheet)[socket] = v.at(2);


    send_message(socket, "connected " + (*spreadsheets)[v.at(2)]->size());
    std::map<std::string, std::string> cells_to_send = (*spreadsheets)[v.at(2)]->get_cells();
    std::map<std::string, std::string>::iterator it;
    for(it = cells_to_send.begin(); it != cells_to_send.end(); it++)
      send_message(socket, "cell " + it->first + " " + it->second);

  
  }
  else if(v.at(0) == "register" && *registered)
  {
    if(std::find((*users).begin(), (*users).end(), v.at(1)) == (*users).end())
    {
      (*users).push_back(v.at(1));
      print_stuff(users);
    }
    else
      send_message(socket, "error 4 " + v[1]);
  }
  else if(v.at(0) == "cell" && *registered)
  {
    // get the spreadsheet name
    std::string sheet_name = (*socket_spreadsheet)[socket];
    if((*spreadsheets)[sheet_name]->add(v.at(1), v.at(2)))
    {
      // iterate through all of the clients and send new cell value
      for(socket_list::iterator iterator = (*spreadsheet_clients)[sheet_name].begin();
        iterator != (*spreadsheet_clients)[sheet_name].end(); iterator++)
          send_message(*iterator, input); 
    }
    else
      send_message(socket, "error 1 CD");
  }
  else if(v.at(0) == "undo")
  {
    std::string sheet_name = (*socket_spreadsheet)[socket];
    for(socket_list::iterator iterator = (*spreadsheet_clients)[sheet_name].begin();
      iterator != (*spreadsheet_clients)[sheet_name].end(); iterator++)
        send_message(*iterator, (*spreadsheets)[(*socket_spreadsheet)[socket]]->undo());
  }
  else
    send_message(socket, "error 2 " + input);

	// parse message
	// complete command
	// decide where to go from here
}

void spreadsheet_server::send_message(int socket, std::string temp)
{
	char * cstr = new char[temp.length() + 1];
	std::strcpy (cstr, temp.c_str());
	cstr[temp.length()] = '\n';
	send(socket, cstr, (temp.length() +1), 0);
}

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

  // Probably undo
  if (!first_space && !second_space)
    result.push_back(input);
  // Probably Register User
  else if(!second_space)
  { 
    std::string first_one = input.substr(0, first_space);
    std::string second_one = input.substr(first_space + 1, input.length() - first_space - 1);

    std::cout << first_one << std::endl;
    std::cout << first_one.length() << std::endl;
    std::cout << second_one << std::endl;
    std::cout << second_one.length() << std::endl;
    result.push_back(first_one);
    result.push_back(second_one);
  }
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
