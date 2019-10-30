using System;
using System.Collections.Generic;
using FinchAPI;

namespace Project_FinchControl
{

    // **************************************************
    //
    // Title: Finch Control
    // Description: A Menu-Based application for operating the user's Finch Robot.
    // Application Type: Console
    // Author: Joseph Geranen
    // Dated Created: 2 October 2019
    // Last Modified: 16 October 2019
    //
    // **************************************************

    class Program
    {
        //
        //All user commands for user programming.
        //
        public enum FinchCommand
        {
            NONE,
            MOVEFORWARD,
            MOVEBACKWARD,
            STOPMOTORS,
            DELAY,
            TURNRIGHT,
            TURNLEFT,
            LEDON,
            LEDOFF,
            DONE,
        }


        static void Main(string[] args)
        {
            DisplayWelcomeScreen();

            DisplayMainMenu();
            DisplayClosingScreen();
        }
   
        #region USER PROGRAMMING
        static void DisplayUserProgramming(Finch finchRobot)
        {
            string menuChoice;
            bool quitApplication = false;
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;
            commandParameters.motorSpeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;
            List<FinchCommand> commands = new List<FinchCommand>();
            do
            {
                DisplayScreenHeader("Main Menu");

                //
                //get user's menu choice
                //
                Console.WriteLine("a) Set Command Parameters");
                Console.WriteLine("b) Add Commands");
                Console.WriteLine("c) View Commands");
                Console.WriteLine("d) Execute Commands");
                Console.WriteLine("q) Quit");
                Console.WriteLine("Enter Choice");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        commandParameters = DisplayGetCommandParameters();
                        break;

                    case "b":
                        DisplayGetFinchCommands(commands);
                        break;

                    case "c":
                        DisplayFinchCommands(commands);
                        break;

                    case "d":
                        DisplayExecuteFinchCommands(finchRobot, commands, commandParameters);
                        break;

                    case "q":

                        quitApplication = true;

                        break;

                    default:
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Please refer to the selections to their letter assignment. Ex: if you want to add commands, just type 'b'");
                        Console.ForegroundColor = ConsoleColor.White;
                        DisplayContinuePrompt();
                        break;
                }



            } while (!quitApplication);
        }
        static void DisplayExecuteFinchCommands (Finch finchRobot,
            List<FinchCommand> commands,
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters)
        {
            int motorSpeed = commandParameters.motorSpeed;
            int ledBrightness = commandParameters.ledBrightness;
            int waitMilliseconds = commandParameters.waitSeconds * 1000;
            DisplayScreenHeader("Execute Finch Commands");
            // info and pause
            Console.ReadKey();
            foreach (FinchCommand command in commands)
            {
                switch (command)
                {
                    case FinchCommand.NONE:
                        break;
                    case FinchCommand.MOVEFORWARD:
                        finchRobot.setMotors(motorSpeed,motorSpeed);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.MOVEBACKWARD:
                        finchRobot.setMotors(-motorSpeed, -motorSpeed);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.STOPMOTORS:
                        finchRobot.setMotors(0,0);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.DELAY:
                        finchRobot.wait(waitMilliseconds);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.TURNRIGHT:
                        finchRobot.setMotors(-motorSpeed, motorSpeed);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.TURNLEFT:
                        finchRobot.setMotors(motorSpeed, -motorSpeed);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.LEDON:
                        finchRobot.setLED(ledBrightness,ledBrightness,ledBrightness);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.LEDOFF:
                        finchRobot.setLED(0, 0, 0);
                        Console.WriteLine(command);
                        break;
                    case FinchCommand.DONE:
                        break;
                    default:
                        break;
                }
            }


        }
        static void DisplayFinchCommands (List<FinchCommand> commands)
        {
            DisplayScreenHeader("Finch Robot Commands:");
            foreach (FinchCommand command in commands)
            {
                Console.WriteLine(command);
            }

            DisplayContinuePrompt();
        }
        static void DisplayGetFinchCommands(List<FinchCommand> commands)
        {
            FinchCommand command = FinchCommand.NONE;
            string userResponse;
            DisplayScreenHeader("Finch Robot Commands");
            // info for user
            while (command != FinchCommand.DONE)
            {
                // todo - VALIDATE. ECHO, AND FEEDBACK.
                Console.WriteLine("Enter Command:");
                userResponse = Console.ReadLine().ToUpper();
                Enum.TryParse(userResponse, out command);
                if (command != FinchCommand.NONE)
                {
                    commands.Add(command);
                }

            }


            DisplayContinuePrompt();
        }

        static (int motorSpeed, int ledBrightness, int waitSeconds) DisplayGetCommandParameters()
        {
            (int motorSpeed, int ledBrightness, int waitSeconds) commandParameters;
            commandParameters.motorSpeed = 0;
            commandParameters.ledBrightness = 0;
            commandParameters.waitSeconds = 0;

            // todo - VALIDATE AND ECHO

            Console.WriteLine("Enter Motor Speed [1 - 255]");
            int.TryParse(Console.ReadLine(), out commandParameters.motorSpeed);

            Console.WriteLine("Enter LED Brightness [1 - 255]");
            int.TryParse(Console.ReadLine(), out commandParameters.ledBrightness);

            Console.WriteLine("Enter Wait in Seconds");
            int.TryParse(Console.ReadLine(), out commandParameters.waitSeconds);

            return commandParameters;
        }
        #endregion

        #region ALARM SYSTEM
        static bool MonitorCurrentTempLevel(Finch finchRobot, double threshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            double currentTempLevel;
            double seconds = 0;


            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentTempLevel = finchRobot.getTemperature();
                DisplayScreenHeader("Monitor Temperature Levels");
                Console.WriteLine($"Maximum Temperature Level: {threshold}");
                Console.WriteLine($"Current Temperature Level: {currentTempLevel}");
                if (currentTempLevel > threshold)
                {
                    thresholdExceeded = true;
                }
                finchRobot.wait(500);
                seconds += 0.5;
            }

            return thresholdExceeded;
        }

        static bool MonitorCurrentLightLevel(Finch finchRobot, double threshold, int maxSeconds)
        {
            bool thresholdExceeded = false;
            int currentLightLevel;
            double seconds = 0;


            while (!thresholdExceeded && seconds <= maxSeconds)
            {
                currentLightLevel = finchRobot.getLeftLightSensor();
                DisplayScreenHeader("Monitor Light Levels");
                Console.WriteLine($"Maximum Light Level: {threshold}");
                Console.WriteLine($"Current Light Level: {currentLightLevel}");
                if (currentLightLevel > threshold)
                {
                    thresholdExceeded = true;
                }
                finchRobot.wait(500);
                seconds += 0.5;
            }

            return thresholdExceeded;
        }

        private static void DisplayAlarmSystem(Finch finchRobot)
        {
            string alarmType;
            int maxSeconds;
            double threshold;
            bool thresholdExceeded;
            DisplayScreenHeader("Alarm System");
            finchRobot.setLED(0, 255, 0);

            alarmType = DisplayGetAlarmType();
            maxSeconds = DisplayGetMaxSeconds();
            threshold = DisplayGetThreshold(finchRobot, alarmType);
            // pause and prompt user


            if (alarmType == "light")
            {
                thresholdExceeded = MonitorCurrentLightLevel(finchRobot, threshold, maxSeconds);
            }
            else
            {
                thresholdExceeded = MonitorCurrentTempLevel(finchRobot, threshold, maxSeconds);
            }



            if (thresholdExceeded)
            {
                if (alarmType == "light")
                {
                    Console.WriteLine("Max Light Level Exceeded.");
                    finchRobot.setLED(255, 0, 0);
                    PlayNote(finchRobot, 1000, 1000);
                }
                else
                {
                    Console.WriteLine("Max Temperature Level Exceeded.");
                    finchRobot.setLED(255, 0, 0);
                    PlayNote(finchRobot, 1000, 1000);
                }
            }
            else
            {
                Console.WriteLine("Maximum Time Exceeded.");
                finchRobot.setLED(255, 255, 255);
                PlayNote(finchRobot, 500, 1000);
            }

            DisplayContinuePrompt();
        }

        static double DisplayGetThreshold(Finch finchRobot, string alarmType)
        {
            double threshold = 0;
            bool validResponse = false;
            DisplayScreenHeader("Threshold Acquisition");

            switch (alarmType)
            {
                case "light":

                    do
                    {
                        Console.Write($"Current Light Level: {finchRobot.getLeftLightSensor()}");
                        Console.WriteLine();
                        Console.Write("Enter max light level.[0-255]");
                        if (double.TryParse(Console.ReadLine(), out threshold))
                        {
                            if (0 <= threshold && threshold <= 255)
                            {
                                validResponse = true;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Please input a valid number. Must be between 0 and 255.");
                        }


                    } while (!validResponse);

                    break;

                case "temperature":

                    Console.Write($"Current Temperature Level: {finchRobot.getTemperature()}");
                    Console.WriteLine();
                    Console.Write("Enter max Temperature level.[Celsius]");
                    do
                    {
                        if (double.TryParse(Console.ReadLine(), out threshold))
                        {
                            validResponse = true;
                        }
                        else
                        {
                            Console.WriteLine("Please input a valid number.");
                        }
                    } while (!validResponse);

                    break;

                default:
                    throw new FormatException();
                    break;
            }

            DisplayContinuePrompt();
            return threshold;
        }

        static int DisplayGetMaxSeconds()
        {
            bool validResponse = false;
            int maxSeconds;

            do
            {
                Console.WriteLine("Enter the Max number of seconds");

                if (int.TryParse(Console.ReadLine(), out maxSeconds))
                {
                    validResponse = true;
                }
                else
                {
                    Console.WriteLine("Please input a valid number.");
                }
            } while (!validResponse);

            return maxSeconds;
        }

        static string DisplayGetAlarmType()
        {

            bool validResponse = false;
            string alarmType;

            do
            {
                Console.WriteLine("Enter Alarm Type [light or temperature]");

                switch (alarmType = Console.ReadLine())
                {

                    case "light":
                        Console.WriteLine("You have chosen: light.");
                        validResponse = true;
                        break;
                    case "temperature":
                        Console.WriteLine("You have chosen: temperature.");
                        validResponse = true;
                        break;

                    default:
                        Console.WriteLine("Please input a valid alarm type.");
                        break;
                }
            } while (!validResponse);


            return alarmType;

        }

        #endregion

        #region DATA RECORDER
        private static void DisplayDataRecorder(Finch finchRobot)
        {
            double dataPointFrequency;
            int numberOfDataPoints;
            DisplayScreenHeader("Data Recorder");
            // information time!
            dataPointFrequency = DisplayGetDataPointFrequency();
            // the number given in frequency is in seconds. multiply by 1000 later.
            numberOfDataPoints = DisplayGetDataPointQuantity();
            double[] temperatures = new double[numberOfDataPoints];
            DisplayGetData(finchRobot, numberOfDataPoints, dataPointFrequency, temperatures);
            DisplayData(temperatures);





            DisplayContinuePrompt();
        }

        static void DisplayData(double[] temperatures)
        {
            string userResponse;
            bool validResponse = false;
            //Console.WriteLine($"Temperature {index+1}: {(((temperatures[index]) *9/5) +32)} °F"); the code to convert to farenheit
            //Console.WriteLine($"Temperature {index + 1}: {temperatures[index]} °C"); the code for celsius
            do
            {
                Console.WriteLine("Would you like your final readings to be displayed in farenheit or celsius?");
                userResponse = Console.ReadLine().ToLower();
                if (userResponse == "celsius")
                {

                    DisplayScreenHeader("Temperatures");

                    for (int index = 0; index < temperatures.Length; index++)
                    {
                        Console.WriteLine($"Temperature {index + 1}: {temperatures[index]} °C");
                    }



                    validResponse = true;
                }
                else if (userResponse == "farenheit")
                {
                    DisplayScreenHeader("Temperatures");

                    for (int index = 0; index < temperatures.Length; index++)
                    {
                        Console.WriteLine($"Temperature {index + 1}: {(((temperatures[index]) * 9 / 5) + 32)} °F");
                    }



                    validResponse = true;
                }
                else
                {
                    Console.WriteLine("Please use the full unit name. If it still doesnt work, check spelling.");
                }



            } while (!validResponse);




            DisplayContinuePrompt();


        }

        static void DisplayGetData(
            Finch finchRobot,
            int numberOfDataPoints,
            double dataPointFrequency,
            double[] temperatures)
        {
            DisplayScreenHeader("Get Temperatures");

            // give the user info and a prompt
            const double MILLISECONDS_PER_SECOND = 1000;
            for (int index = 0; index < numberOfDataPoints; index++)
            {
                temperatures[index] = finchRobot.getTemperature();
                int milliseconds = (int)(dataPointFrequency * MILLISECONDS_PER_SECOND);
                finchRobot.wait(milliseconds);


                Console.WriteLine($"Temperature {index + 1}: {temperatures[index]}");
            }




            DisplayContinuePrompt();
        }


        static int DisplayGetDataPointQuantity()
        {
            int dataPointQuantity;
            bool validResponse = false;

            Console.WriteLine("What is the quantity of readings you want?");
            do
            {
                if (int.TryParse(Console.ReadLine(), out dataPointQuantity))
                {
                    validResponse = true;
                    Console.WriteLine($"You desire {dataPointQuantity} readings.");
                }
                else
                {
                    Console.WriteLine("Please insert a valid number.");
                }
                DisplayContinuePrompt();
            } while (!validResponse);


            return dataPointQuantity;
        }

        static double DisplayGetDataPointFrequency()
        {
            double dataPointFrequency;
            bool validResponse = false;

            Console.WriteLine("What is the frequency you desire in seconds?");
            do
            {
                if (double.TryParse(Console.ReadLine(), out dataPointFrequency))
                {
                    validResponse = true;
                    Console.WriteLine($"Your frequency in seconds is a reading every {dataPointFrequency} second.");
                }
                else
                {
                    Console.WriteLine("Please insert a valid number.");
                }
                DisplayContinuePrompt();
            } while (!validResponse);


            return dataPointFrequency;
        }
        #endregion

        #region TALENT SHOW
        static void DisplayTalentShow(Finch finchRobot)
        {
            DisplayScreenHeader("~{The Talent Show!}~");
            Console.WriteLine("The Finch is ready to perform.");
            DisplayContinuePrompt();
            int userPitch;
            int userLength;
            bool validResponse;
            string userResponse;
            for (int i = 0; i < 255; i++)
            {
                finchRobot.setLED(i, i, i);
            }
            finchRobot.setLED(0, 0, 0);
            for (int i = 0; i < 255; i++)
            {
                finchRobot.setLED(0, i, 0);
            }
            finchRobot.setLED(0, 0, 0);
            for (int i = 0; i < 255; i++)
            {
                finchRobot.setLED(0, 0, i);
            }
            finchRobot.setLED(0, 0, 0);
            for (int i = 0; i < 255; i++)
            {
                finchRobot.setLED(i, 0, 0);
            }
            finchRobot.setLED(0, 0, 0);

            do
            {
                Console.WriteLine("I want to play a note! What pitch do you want in hertz?");
                userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out userPitch))
                {
                    validResponse = true;
                }
                else
                {
                    validResponse = false;
                    Console.WriteLine();
                    Console.WriteLine("Please use numbers only.");
                    Console.WriteLine();
                }


            } while (!validResponse);
            do
            {
                Console.WriteLine("And it's length in milliseconds?");
                userResponse = Console.ReadLine();
                if (int.TryParse(userResponse, out userLength))
                {
                    validResponse = true;
                }
                else
                {
                    validResponse = false;
                    Console.WriteLine();
                    Console.WriteLine("I said, NUMBERS ONLY!");
                    Console.WriteLine();
                }

            } while (!validResponse);

            Console.WriteLine("Here's your note!");
            PlayNote(finchRobot, userPitch, userLength);

            DisplayContinuePrompt();

            Console.WriteLine("Watch me go!");
            finchRobot.setMotors(255, 255);
            finchRobot.wait(2500);
            finchRobot.setMotors(-55, -55);
            finchRobot.wait(2500);
            finchRobot.setMotors(255, -255);
            finchRobot.wait(1000);
            finchRobot.setMotors(-55, 55);
            finchRobot.wait(3000);
            finchRobot.setMotors(0, 0);


            DisplayContinuePrompt();
        }
        #endregion

        #region  FINCH MANAGEMENT
        /// <summary>
        /// plays a note with an input frequency and length.
        /// 
        /// </summary>
        static void PlayNote(Finch finchRobot, int frequency, int length)
        {
            finchRobot.noteOn(frequency);
            finchRobot.wait(length);
            finchRobot.noteOff();

        }

        static void DisplayDisconnectFinch(Finch finchRobot)
        {
            DisplayScreenHeader("Disconnect Finch Robot");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ready to Disconnect. Confirm.");
            Console.ForegroundColor = ConsoleColor.White;
            DisplayContinuePrompt();
            finchRobot.disConnect();
            Console.WriteLine();
            Console.WriteLine("Finch robot is now disconnected.");



            DisplayContinuePrompt();
        }

        static bool DisplayConnectFinchRobot(Finch finchRobot)
        {
            bool finchConnected = false;
            DisplayScreenHeader("Connect Finch Robot");
            Console.WriteLine("Ready to connect. Please ensure the cable is connected between the computer and robot before continuing.");
            DisplayContinuePrompt();

            finchConnected = finchRobot.connect();

            if (finchConnected)
            {
                Console.WriteLine("Finch has been successfully connected.");
                for (int i = 0; i < 3; i++)
                {
                    finchRobot.setLED(0, 255, 0);
                    finchRobot.noteOn(432);
                    finchRobot.wait(660);
                    finchRobot.noteOff();
                    finchRobot.setLED(0, 0, 0);
                    finchRobot.wait(300);

                }
            }
            else
            {
                Console.WriteLine("Unable to connect.");
                finchRobot.setLED(255, 0, 0);
                finchRobot.noteOn(100);
                finchRobot.wait(1000);
                finchRobot.noteOff();
                finchRobot.setLED(0, 0, 0);
            }

            DisplayContinuePrompt();
            return finchConnected;
        }


        #endregion

        #region USER INTERFACE
 static void DisplayMainMenu()
        {
            //
            //instantiate a finch object
            //


            Finch finchRobot = new Finch();
            bool finchRobotConnected = false;
            bool quitApplication = false;
            string menuChoice;

            do
            {
                DisplayScreenHeader("Main Menu");

                //
                //get user's menu choice
                //
                Console.WriteLine("a) Connect finch robot (REQUIRED TO OPERATE)");
                Console.WriteLine("b) Talent Show");
                Console.WriteLine("c) Data Recorder");
                Console.WriteLine("d) Alarm System");
                Console.WriteLine("e) User Programming");
                Console.WriteLine("f) Disconnect Finch Robot");
                Console.WriteLine("q) Quit");
                Console.WriteLine("Enter Choice");
                menuChoice = Console.ReadLine().ToLower();

                switch (menuChoice)
                {
                    case "a":
                        finchRobotConnected = DisplayConnectFinchRobot(finchRobot);
                        break;

                    case "b":
                        if (finchRobotConnected)
                        {
                            DisplayTalentShow(finchRobot);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Please ensure you are connected.");
                            DisplayContinuePrompt();
                        }
                        break;

                    case "c":
                        if (finchRobotConnected)
                        {
                            DisplayDataRecorder(finchRobot);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Please ensure you are connected.");
                            DisplayContinuePrompt();
                        }

                        break;

                    case "d":
                        if (finchRobotConnected)
                        {
                            DisplayAlarmSystem(finchRobot);
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Please ensure you are connected.");
                            DisplayContinuePrompt();
                        }
                        break;

                    case "e":
                        DisplayUserProgramming(finchRobot);
                        break;

                    case "f":

                        DisplayDisconnectFinch(finchRobot);
                        finchRobotConnected = false;
                        break;

                    case "q":
                        if (finchRobotConnected == false)
                        {
                            quitApplication = true;
                        }
                        else
                        {
                            Console.WriteLine("Please ensure your finch has been disconnected.");
                            DisplayContinuePrompt();
                        }

                        break;

                    default:
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Please refer to the selections to their letter assignment. Ex: if you want talent show, just type 'b'");
                        Console.ForegroundColor = ConsoleColor.White;
                        DisplayContinuePrompt();
                        break;
                }



            } while (!quitApplication);

        }

        /// <summary>
        /// display welcome screen
        /// </summary>
        static void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tFinch Control");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display closing screen
        /// </summary>
        static void DisplayClosingScreen()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\tThank you for using Finch Control!");
            Console.WriteLine();

            DisplayContinuePrompt();
        }

        /// <summary>
        /// display continue prompt
        /// </summary>
        static void DisplayContinuePrompt()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// display screen header
        /// </summary>
        static void DisplayScreenHeader(string headerText)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t" + headerText);
            Console.WriteLine();
        }

        #endregion
    }
}
