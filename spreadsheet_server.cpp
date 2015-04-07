// SpreadsheetServer.cpp 
#include <iostream>
#include <string>
#include <sys/socket.h>
#include <sys/types.h>
#include "spreadsheet_server.h"

int main(int argc, char const *argv[])
{
	listen();
	return 0;
}

spreadsheet_server::spreadsheet_server()
{

}

void spreadsheet_server::listen()
{
	while(true)
	{
		connect();
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