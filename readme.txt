Server Notes::

To compile and run spreadsheet server:

After zip extraction to a folder, in the command line type 'make run'. This compiles and runs the program and listens to port 2112 by default. If you would like to run the server on a different port, you can specify with a command line argument. 'make clean' will remove all .bin and .o files to have a clean server.

Run 'make' and then './server.o [port_num]' to run the server on a different port! 

To stop server type 'kill' in the console while the server is running. This saves the spreadsheets and terminates the server.