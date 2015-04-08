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

#define PENDINGCONNECTIONS 5

void *get_in_addr(struct sockaddr *sa);
void sigchld_handler(int s);

int main(int argc, char const *argv[])
{
	std::cout << "HERE" << std::endl;
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

    sa.sa_handler = sigchld_handler; // reap all dead processes
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = SA_RESTART;
    if (sigaction(SIGCHLD, &sa, NULL) == -1) {
        perror("sigaction");
        exit(1);
    }

    printf("server: waiting for connections...\n");

     // main accept() loop //put while loop back HERE
        sin_size = sizeof their_addr;
        new_fd = accept(sockfd, (struct sockaddr *)&their_addr, &sin_size);
        if (new_fd == -1) {
            perror("accept");
            //continue;
            return;
        }

        inet_ntop(their_addr.ss_family,
            get_in_addr((struct sockaddr *)&their_addr),
            s, sizeof s);
        printf("server: got connection from %s\n", s);

        message_received(new_fd);
        //close(new_fd);  // parent doesn't need this

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


void spreadsheet_server::connect()
{

}

void spreadsheet_server::message_received(int socket)
{
	// char incoming_buffer[256];
	// char * buf_ptr = (char *) incoming_buffer;
	// int last_index = 0;
	// size_t to_read = 256;
	// while(to_read > 0)
	// {
	// 	ssize_t receive_size = recv(socket, buf_ptr, 256, 0);
	// 	if(receive_size <= 0)
	// 		return;

	// 	to_read 		-= receive_size;
	// 	buf_ptr += receive_size;
	// }

	if (send(socket, "Hello from server :)\r\n", 50, 0) == -1)
                perror("send");
    //close(socket);
    //exit(0);
}

void spreadsheet_server::send_message(std::string s)
{

}
