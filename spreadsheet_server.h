// SpreadsheetServer.h 
// Test Edit
#ifndef SPREADSHEET_SERVER_H
#define SPREADSHEET_SERVER_H

#include <string>
#include <iostream>
#include "spreadsheet_server.h"
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h>
#include <arpa/inet.h>
#include <sys/wait.h>
#include <signal.h>
#include <cstring>
#include <vector>
#include "spreadsheet_graph.h"
#include <map>
#include <unistd.h>
#include <pthread.h>
#include <stdlib.h>
#include <sstream>

#define PENDINGCONNECTIONS 5

typedef std::vector<int>                            socket_list;
typedef std::map<std::string, spreadsheet_graph*>   spreadsheet_map;
typedef std::map<std::string, socket_list >    		spreadsheet_client_map;
typedef std::map<int, std::string>                  socket_spreadsheet_map;

class spreadsheet_server
{
	public:
	  spreadsheet_server();
	  void listen_for_connections();
	  void listen_to_client(int socket);
	  void send_message(int socket, std::string s);
	  ~spreadsheet_server();

	private:
		void process_request(int socket, std::string input, bool * registered);
		void process_connect(int socket, std::vector<std::string> v, bool * registered);
		void process_register(int socket, std::vector<std::string> v);
		void process_cell(int socket, std::vector<std::string> v, std::string input);
		void process_undo(int socket, std::vector<std::string> v);

		std::vector<std::string> parse_command(std::string input);
		spreadsheet_map * spreadsheets;
    	spreadsheet_client_map * spreadsheet_clients;
    	socket_spreadsheet_map * socket_spreadsheet;
    	std::vector<std::string> * users;
};

#endif
