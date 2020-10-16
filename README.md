# TCP-FSM
Simplistic TCP Finite State Machine program in C#
This is a sample of how the application works given the following input ["APP_PASSIVE_OPEN", "APP_SEND", "RCV_SYN_ACK"] =>  "ESTABLISHED":

Welcome to Simplistic TCP Finite State Machine program.
Please enter each event that you want to evaluate individually and press Enter.
Anytime enter Q or q to finish your capture.
APP_PASSIVE_OPEN
APP_SEND
RCV_SYN_ACK
q
Final State is: ESTABLISHED
********************************************
Press Enter to close the program.
********************************************

This is a sample of how the application works given the following input ["APP_ACTIVE_OPEN"] =>  "SYN_SENT":

Welcome to Simplistic TCP Finite State Machine program.
Please enter each event that you want to evaluate individually and press Enter.
Anytime enter Q or q to finish your capture.
APP_ACTIVE_OPEN
q
Final State is: SYN_SENT
********************************************
Press Enter to close the program.
********************************************

This is a sample of how the application works given the following input ["APP_ACTIVE_OPEN", "RCV_SYN_ACK", "APP_CLOSE", "RCV_FIN_ACK", "RCV_ACK"] =>  "ERROR":

Welcome to Simplistic TCP Finite State Machine program.
Please enter each event that you want to evaluate individually and press Enter.
Anytime enter Q or q to finish your capture.
APP_ACTIVE_OPEN
RCV_SYN_ACK
APP_CLOSE
RCV_FIN_ACK
RCV_ACK
q
Final State is: ERROR
********************************************
Press Enter to close the program.
********************************************
