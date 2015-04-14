// SpreadsheetServer.cpp 
#include "spreadsheet_server.h"
void *get_in_addr(struct sockaddr *sa);
void sigchld_handler(int s);

int main(int argc, char const *argv[])
{
	std::cout << "Spreadsheet server created" << std::endl;
  	spreadsheet_server server;
  	server.listen_for_connections();

  	// mem clean up
  	return 0;
}

spreadsheet_server::spreadsheet_server()
{
	graph = new spreadsheet_graph();
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

    sa.sa_handler = sigchld_handler; // reap all dead processes
    sigemptyset(&sa.sa_mask);
    sa.sa_flags = SA_RESTART;
    if (sigaction(SIGCHLD, &sa, NULL) == -1) {
        perror("sigaction");
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

        if (!fork()) // this is the child process
        { 
            close(sockfd);
            listen_to_client(new_fd);   
        }
        close(new_fd);  // parent doesn't need this
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


void spreadsheet_server::connect()
{

}

// WE SHOULD RENAME THIS LISTEN_FOR_MESSAGE or something
void spreadsheet_server::listen_to_client(int socket)
{
	std::string temp = "";
	int last_index = 0;
	while(1)
	{
  		char msg[1];
  		recv(socket, msg, 1, 0);
  		if(temp == "END\n")
  			break;
  		if(msg[0] == '\n')
  		{
  			process_request(temp);
  			send_message(socket, temp);
  			temp = "";
  		}
  		else
  			temp += msg;
		 //if (send(socket, msg, sizeof(msg), 0) == -1)
  	 		//break;
	}		
  	close(socket);
  	exit(0);
}
void spreadsheet_server::process_request(std::string input)
{	
	return;
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
  			std::cout << cstr << std:: endl;
}
