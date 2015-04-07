// SpreadsheetServer.h 
// Test Edit
#ifndef SPREADSHEET_SERVER_H
#define SPREADSHEET_SERVER_H

class spreadsheet_server
{
	public:
	  spreadsheet_server();
	  void listen();
	  void connect();
	  void message_received();
	  void send_message(std::string s);

	  ~spreadsheet_server();

	private:
};

#endif
