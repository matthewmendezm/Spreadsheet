// SpreadsheetServer.cpp 
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


int main(int argc, char const *argv[])
{
std::cout << "HERE";
  spreadsheet_server server;
  server.listen_for_connections();
  return 0;
}

spreadsheet_server::spreadsheet_server()
{

}

void spreadsheet_server::listen_for_connections()
{
  int sockfd, new_fd;  // listen on sock_fd, new connection on new_fd
  struct addrinfo hints, *servinfo, *p;
  struct sockaddr_storage their_addr; // connector's address information
  socklen_t sin_size;
  struct sigaction sa;
  int yes=1;
  char s[INET6_ADDRSTRLEN];
  int rv;
  memset(&hints, 0, sizeof hints);
  hints.ai_family = AF_INET;
  hints.ai_socktype = SOCK_STREAM;
  hints.ai_flags = AI_PASSIVE; // use my IP
  
  if ((rv = getaddrinfo(NULL, "2112", &hints, &servinfo)) != 0) {
    fprintf(stderr, "getaddrinfo: %s\n", gai_strerror(rv));
    return;
  }
  sockfd = socket(p->ai_family, p->ai_socktype, p->ai_protocol);
  bind(sockfd, p->ai_addr, p->ai_addrlen);

  freeaddrinfo(servinfo);
  
  listen(sockfd, NULL);

  while(true)
  {
    sin_size = sizeof their_addr;
    new_fd = accept(sockfd, (struct sockaddr *)&their_addr, &sin_size);
    if(new_fd != -1)
      std::cout << "CONNECTED TO SOMEONE" << std::endl;
  }
}

void spreadsheet_server::connect()
{

}

void spreadsheet_server::message_received()
{

}

void spreadsheet_server::send_message(std::string s)
{

}
