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

#define PENDINGCONNECTIONS 5

class spreadsheet_server
{
	public:
	  spreadsheet_server();
	  void listen_for_connections();
	  void connect();
	  void listen_to_client(int socket);
	  void send_message(int socket, std::string s);

	  ~spreadsheet_server(){}

	private:
		spreadsheet_graph * graph;
		void process_request(int socket, std::string input);
		std::vector<std::string> parse_command(std::string input);

};

#endif
