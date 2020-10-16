using System;
using System.Collections.Generic;

namespace Juliet
{
   public enum ProcessState
   {
      CLOSED,
      LISTEN,
      SYN_RCVD,
      SYN_SENT,
      ESTABLISHED,
      FIN_WAIT_1,
      CLOSING,
      FIN_WAIT_2,
      TIME_WAIT,
      CLOSE_WAIT,
      LAST_ACK
   }

   public enum Command
   {
      APP_PASSIVE_OPEN,
      APP_ACTIVE_OPEN,
      RCV_SYN,
      APP_SEND,
      APP_CLOSE,
      RCV_ACK,
      RCV_SYN_ACK,
      RCV_FIN,
      RCV_FIN_ACK,
      APP_TIMEOUT
   }

   public class Process
   {
      class StateTransition
      {
         readonly ProcessState CurrentState;
         readonly Command Command;

         public StateTransition(ProcessState currentState, Command command)
         {
            CurrentState = currentState;
            Command = command;
         }

         public override int GetHashCode()
         {
            return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
         }

         public override bool Equals(object obj)
         {
            StateTransition other = obj as StateTransition;
            return other != null && this.CurrentState == other.CurrentState && this.Command == other.Command;
         }
      }

      Dictionary<StateTransition, ProcessState> transitions;
      public ProcessState CurrentState { get; private set; }

      public Process()
      {
         CurrentState = ProcessState.CLOSED;
         transitions = new Dictionary<StateTransition, ProcessState>
            {
               { new StateTransition(ProcessState.CLOSED, Command.APP_PASSIVE_OPEN), ProcessState.LISTEN },
               { new StateTransition(ProcessState.CLOSED, Command.APP_ACTIVE_OPEN), ProcessState.SYN_SENT },

               { new StateTransition(ProcessState.LISTEN, Command.RCV_SYN), ProcessState.SYN_RCVD },
               { new StateTransition(ProcessState.LISTEN, Command.APP_SEND), ProcessState.SYN_SENT },
               { new StateTransition(ProcessState.LISTEN, Command.APP_CLOSE), ProcessState.CLOSED },

               { new StateTransition(ProcessState.SYN_RCVD, Command.APP_CLOSE), ProcessState.FIN_WAIT_1 },
               { new StateTransition(ProcessState.SYN_RCVD, Command.RCV_ACK), ProcessState.ESTABLISHED },

               { new StateTransition(ProcessState.SYN_SENT, Command.RCV_SYN), ProcessState.SYN_RCVD },
               { new StateTransition(ProcessState.SYN_SENT, Command.RCV_SYN_ACK), ProcessState.ESTABLISHED },
               { new StateTransition(ProcessState.SYN_SENT, Command.APP_CLOSE), ProcessState.CLOSED },

               { new StateTransition(ProcessState.ESTABLISHED, Command.APP_CLOSE), ProcessState.FIN_WAIT_1 },
               { new StateTransition(ProcessState.ESTABLISHED, Command.RCV_FIN), ProcessState.CLOSE_WAIT },

               { new StateTransition(ProcessState.FIN_WAIT_1, Command.RCV_FIN), ProcessState.CLOSING },
               { new StateTransition(ProcessState.FIN_WAIT_1, Command.RCV_FIN_ACK), ProcessState.TIME_WAIT },
               { new StateTransition(ProcessState.FIN_WAIT_1, Command.RCV_ACK), ProcessState.FIN_WAIT_2 },

               { new StateTransition(ProcessState.CLOSING, Command.RCV_ACK), ProcessState.TIME_WAIT },

               { new StateTransition(ProcessState.FIN_WAIT_2, Command.RCV_FIN      ), ProcessState.TIME_WAIT },

               { new StateTransition(ProcessState.TIME_WAIT, Command.APP_TIMEOUT   ), ProcessState.CLOSED },

               { new StateTransition(ProcessState.CLOSE_WAIT, Command.APP_CLOSE    ), ProcessState.LAST_ACK },

               { new StateTransition(ProcessState.LAST_ACK, Command.RCV_ACK        ), ProcessState.CLOSED }
            };
      }

      public ProcessState GetNext(Command command)
      {
         StateTransition transition = new StateTransition(CurrentState, command);
         ProcessState nextState;
         if (!transitions.TryGetValue(transition, out nextState))
            throw new Exception("Invalid transition: " + CurrentState + " -> " + command);
         return nextState;
      }

      public ProcessState MoveNext(Command command)
      {
         CurrentState = GetNext(command);
         return CurrentState;
      }
   }


   public class Program
   {
      static void Main(string[] args)
      {
         string userResponse = "";
         List<Command> commands = new List<Command>();
         Console.WriteLine("Welcome to Simplistic TCP Finite State Machine program.");
         Console.WriteLine("Please enter each event that you want to evaluate individually and press Enter.");
         Console.ForegroundColor = ConsoleColor.Green;
         Console.WriteLine("Anytime enter Q or q to finish your capture.");
         while (userResponse.ToLower() != "q")
         {
            userResponse = Console.ReadLine();
            if (userResponse.ToLower() == "q")
               break;

            try
            {
               Command providedCommand = (Command)Enum.Parse(typeof(Command), userResponse, true);

               if (Enum.IsDefined(typeof(Command), providedCommand) | providedCommand.ToString().Contains(","))
                  commands.Add(providedCommand);
               else
                  Console.WriteLine("{0} is not an underlying value of the TCP Events enumeration.", providedCommand.ToString());
            }
            catch (ArgumentException)
            {
               Console.WriteLine("{0} is not a member of the TCP Events enumeration.", userResponse);
            }
         }

         try
         {

            Process p = new Process();
            ProcessState nextState = new ProcessState();

            foreach (Command command in commands)
            {
               nextState = p.MoveNext(command);
            }

            PrintProgramFinalization(p.CurrentState.ToString());

         }
         catch
         {
            PrintProgramFinalization("ERROR");
         }

      }

      private static void PrintProgramFinalization(string finalState)
      {

         Console.WriteLine("Final State is: " + finalState);
         Console.ForegroundColor = ConsoleColor.Red;
         Console.WriteLine("********************************************");
         Console.WriteLine("Press Enter to close the program.");
         Console.WriteLine("********************************************");
         Console.ReadLine();
      }
   }
}