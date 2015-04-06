// SpreadsheetServer.h 
#ifndef SPREADSHEET_SERVER_H
#define SPREADSHEET_SERVER_H

#include <string>

class spreadsheet_server
{
public:
  spreadsheet_server();
  listen();
  connect();
  message_received();
  send_message(std::string);

  ~huge_number();

private:
}



# endif
