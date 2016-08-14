/**
 * Company: [redacted]
 * Date: May 2, 2016
 * Author: Emily Davis (emilysdavis@outlook.com)
 * Modified Date: August 13, 2016
 * Modified By: Emily Davis (emilysdavis@outlook.com)
 * Description: Technical test post-interview
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication
{
    /// <summary>
    /// A program written as a technical test for a software company
    /// </summary>
    class Program
    {
        static bool terminate = false;      // if true, can cause the program to terminate
        static bool noMoreWords = false;    // if true, can trigger a loop break in MOST_COMMON function - Idea from: http://stackoverflow.com/a/203289
        static int buffer = 100;            // Console.BufferWidth value
        static ushort backspacer = 0x0008;  // backspacer Unicode equivalent value for Code Page 850 - Original CP850 code: http://www.ascii-codes.com/cp850.html; Equivalent Unicode value resource: ftp://ftp.unicode.org/Public/MAPPINGS/VENDORS/MICSFT/WindowsBestFit/bestfit1252.txt

        bool chooserTest = false;           // if true, user should be in Chooser function
        bool sumTest = false;               // if true, user should be in SUM function
        bool contTest = false;              // if true, a function has finished its routine and the user can choose to run the function again or return to the Chooser function

        Thread ChooserThread;               // main thread which initially starts the Chooser function
        Thread ConsoleKeyListener;          // thread which listens for certain keypresses when the user is prompted for a single-character input

        /// <summary>
        /// ThreadStarter, a void method
        /// Creates, names, and starts main and key listener threads
        /// Idea from:
        /// http://stackoverflow.com/a/9979160
        /// </summary>
        void ThreadStarter()
        {
            // instantiate ChooserThread
            ChooserThread = new Thread(new ThreadStart(Chooser));

            // instantiate ConsoleKeyListener
            ConsoleKeyListener = new Thread(new ThreadStart(ListenerKeyboardEvent));

            // name ChooserThread
            ChooserThread.Name = "Chooser";

            // name ConsoleKeyListener
            ConsoleKeyListener.Name = "KeyListener";

            // start ChooserThread
            ChooserThread.Start();

            // start ConsoleKeyListener
            ConsoleKeyListener.Start();

        } // end ThreadStarter

        /// <summary>
        /// ListenerKeyboardEvent, a void method
        /// Utilizes a switch statement while listening for keypresses on a 
        /// single character user input so that the proper action can be taken 
        /// when certain keys are pressed
        /// Method based on:
        /// http://stackoverflow.com/a/9979160
        /// </summary>
        void ListenerKeyboardEvent()
        {
            do
            {
                switch (Console.ReadKey(true).Key)
                {
                    // if user presses Escape key, set terminate flag to 'true'
                    // so the program can terminate
                    case ConsoleKey.Escape:
                        terminate = true;
                        break;

                    // if user presses '1' key and is in the chooser screen, 
                    // clear console window and load SUM() function
                    case ConsoleKey.D1:
                        if (chooserTest)
                        {
                            Console.Clear();
                            SUM();
                        }
                        break;

                    // if user presses '2' key and is in the chooser screen, 
                    // clear console window and load MOST_COMMON() function
                    case ConsoleKey.D2:
                        if (chooserTest)
                        {
                            Console.Clear();
                            MOST_COMMON();
                        }
                        break;

                    // if "Continue?" is 'hit', user presses the 'Y' key, and 
                    // is not in the chooser screen, clear console window and 
                    // re-load same function based on if sumTest is 'true' or 
                    // not (sumTest is 'true' only in SUM() function)
                    case ConsoleKey.Y:
                        if (contTest)
                        {
                            Console.Clear();
                            if (sumTest)
                            {
                                SUM();
                            }
                            else
                            {
                                MOST_COMMON();
                            }
                        }
                        break;

                    // if "Continue?" is 'hit', user presses the 'N' key, and 
                    // is not in the chooser screen, clear console window and 
                    // return to chooser screen
                    case ConsoleKey.N:
                        if (contTest)
                        {
                            Console.Clear();
                            Chooser();
                        }
                        break;

                    // all other keypresses and combinations of keypresses do 
                    // nothing
                    default:
                        break;
                }

            } while (true);

        } // end ListenerKeyboardEvent

        /// <summary>
        /// Chooser, a void method
        /// Provides the user with three choices: to run the SUM function, to 
        /// run the MOST_COMMON function, or to exit the program. After 
        /// prompting the user for a selection, the Console.KeyAvailable 
        /// waits for a single key input, which gets checked against the known 
        /// keys in the ListenerKeyboardEvent method to perform the correct 
        /// action
        /// KeyAvailable loop from Examples at:
        /// https://msdn.microsoft.com/en-us/library/system.console.keyavailable(v=vs.110).aspx
        /// </summary>
        void Chooser()
        {
            chooserTest = true;     // user is in the Chooser method; gets reset every time the Chooser method is called
            sumTest = false;        // user is not in the SUM function; gets reset every time the Chooser method is called
            contTest = false;       // user is not being prompted to restart a function; gets reset every time the Chooser method is called

            Console.Title = "WELCOME TO THE MACHINE - CHOOSE WISELY";

            Console.WriteLine("***Welcome to The Machine***");
            Console.WriteLine("From here, you have three options:");
            Console.WriteLine("To use the SUM function, press 1.");
            Console.WriteLine("To use the MOST_COMMON function, press 2.");
            Console.WriteLine("To exit the program at any time, press the Escape (Esc) key.");
            Console.WriteLine("Please enter your selection:");

            while (!Console.KeyAvailable)
            {
                // restarts while loop every 360 milliseconds
                Thread.Sleep(360);
            }

        } // end Chooser

        /// <summary>
        /// Termination, a void method
        /// Waits for permission to terminate and kills all the threads
        /// Extracted ThreadSafe code from the main method on:
        /// http://stackoverflow.com/a/9979160
        /// </summary>
        void Termination()
        {
            while (true)
            {
                if (terminate)
                {
                    // sorry, had to throw in a little 80s geek humour in here
                    Console.WriteLine("END OF LINE");

                    ChooserThread.Abort();
                    ConsoleKeyListener.Abort();

                    Thread.Sleep(2000);
                    Thread.CurrentThread.Abort();

                    return;
                }
            }

        } // end Termination

        /// <summary>
        /// Chelsea, a string method
        /// effectively replaces Console.ReadLine() so that individual 
        /// keypresses can be read asynchronously and actions can be taken 
        /// based on any specific keys or combinations of keys during multi-
        /// character user input
        /// Keypress Handling Idea based on:
        /// http://stackoverflow.com/a/18902318
        /// and Examples on:
        /// https://msdn.microsoft.com/en-us/library/system.consolekeyinfo.keychar(v=vs.110).aspx
        /// </summary>
        /// <returns>Returns a string object to be validated</returns>
        string Chelsea()
        {
            ConsoleKeyInfo readKey;

            string result = String.Empty;
            StringBuilder build = new StringBuilder();

            do
            {
                readKey = Console.ReadKey(true);

                // ignore is ALT is pressed
                if ((readKey.Modifiers & ConsoleModifiers.Alt) == ConsoleModifiers.Alt)
                {
                    continue;
                }

                // ignore is CTRL is pressed, unless in combination with C
                // and only then if in the MOST_COMMON function because that 
                // is the only function that needs to read this key combination
                if ((readKey.Modifiers & ConsoleModifiers.Control) == ConsoleModifiers.Control)
                {
                    if (readKey.Key == ConsoleKey.C)
                    {
                        if (!sumTest && !chooserTest)
                        {
                            noMoreWords = true;

                            return Convert.ToChar(backspacer).ToString();
                        }
                    }
                    continue;
                }

                // ignore if KeyChar value is \u0000
                if (readKey.KeyChar == '\u0000')
                {
                    continue;
                }

                // ignore tab key
                if (readKey.Key == ConsoleKey.Tab)
                {
                    continue;
                }

                // handle backspace
                if (readKey.Key == ConsoleKey.Backspace)
                {
                    // are there any characters to erase?
                    if (build.Length >= 1)
                    {
                        // determine where we are in the console buffer
                        int curCol = Console.CursorLeft - 1;
                        int oldLength = build.Length;
                        int extraRows = oldLength / Console.BufferWidth;

                        result = build.Remove(oldLength - 1, 1).ToString();
                        Console.CursorLeft = 0;
                        Console.CursorTop = Console.CursorTop - extraRows;
                        Console.Write(result + new String(' ', oldLength - build.Length));
                        Console.CursorLeft = curCol;
                    }
                    continue;
                }

                // handle escape key
                if (readKey.Key == ConsoleKey.Escape)
                {
                    // for fancy formatting
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        Console.Write(Environment.NewLine);
                    }

                    // for thread termination, because the program is done now
                    terminate = true;

                    // indicator to help prevent subsequent text from being 
                    // displayed before all threads have been aborted
                    return null;
                }

                // handle key by adding it to input StringBuilder and setting 
                // the value of the resultant string
                Console.Write(readKey.KeyChar);
                build.Append(readKey.KeyChar);
                result = build.ToString();

            } while (readKey.Key != ConsoleKey.Enter);

            // trim leading and trailing whitespace for validation purposes
            result = result.Trim();

            // more fancy formatting
            if (!String.IsNullOrWhiteSpace(result))
            {
                Console.Write(Environment.NewLine);
            }

            // if, after trimming whitespace, the resultant string is empty, 
            // return String.Empty, otherwise return the resultant string
            return (result.Equals(String.Empty) ? String.Empty : result);

        } // end Chelsea

        /// <summary>
        /// SUM, a void function
        /// This function prompts the user for two positive integers: the first 
        /// (n) is the upper limit, the second (q) is the multiple. Using these 
        /// two validated inputs, the function prints out the sum of every qth 
        /// number between 1 and n
        /// KeyAvailable loop from Examples at:
        /// https://msdn.microsoft.com/en-us/library/system.console.keyavailable(v=vs.110).aspx
        /// </summary>
        void SUM()
        {
            chooserTest = false;            // user is no longer in the Chooser method; gets reset every time SUM function is called
            sumTest = true;                 // user is now in the SUM function; gets reset every time SUM function is called
            contTest = false;               // user is not being prompted to restart a function; gets reset every time the SUM function is called

            bool isNInt = true;             // n is assumed to be an integer until otherwise determined
            bool isNPos = true;             // n is assumed to be a positive integer until otherwise determined
            bool isQInt = true;             // q is assumed to be an integer until otherwise determined
            bool isQPos = true;             // q is assumed to be a positive integer until otherwise determined
            int n = 0;                      // n is set to 0 initially
            int q = 0;                      // q is set to 0 initially
            int sigma = 0;                  // sigma is set to 0 initially
            string input = String.Empty;    // input is initially an empty string
            string suffix = String.Empty;   // suffix is initially an empty string

            Console.Title = "WELCOME TO THE MACHINE - THE SUM FUNCTION";

            Console.WriteLine("***The SUM Function***");
            Console.WriteLine("You will be prompted for two positive integers.");
            Console.WriteLine("The first integer (n) will be the upper limit.");
            Console.WriteLine("The second integer (q) will be the multiple.");
            Console.WriteLine("This function will return the sum of every qth number between 1 and n.");
            Console.WriteLine("For example, given n = 10 and q = 3, the function will return 3 + 6 + 9 = 18");

            Console.WriteLine("Please enter a value for the first integer, n:");

            while (!Console.KeyAvailable)
            {
                // restarts while loop every 360 milliseconds
                Thread.Sleep(360);
            }

            // multi-character input is processed through the Chelsea handler
            input = Chelsea();

            // if Chelsea returned null, that means the Escape key was pressed 
            // so the program gets terminated
            if (input == null)
            {
                return;
            }

            // is n an integer? Let's try it!
            isNInt = int.TryParse(input, out n);

            if (isNInt)
            {
                // is n a POSITIVE integer?
                isNPos = (int.Parse(input) > 0);
            }

            // if Chelsea returned an empty string, or a string that was 
            // neither an integer or a positive integer, the user gets prompted 
            // until a valid input is provided and the program can continue
            while (input.Equals(String.Empty) || !isNInt || !isNPos)
            {
                Console.WriteLine("INVALID INPUT");
                Console.WriteLine("Must be an integer greater than 0.");
                Console.WriteLine("Please enter a value for the first integer, n:");

                while (!Console.KeyAvailable)
                {
                    // restarts while loop every 360 milliseconds
                    Thread.Sleep(360);
                }

                // multi-character input is processed through the Chelsea handler
                input = Chelsea();

                // if Chelsea returned null, that means the Escape key was pressed 
                // so the program gets terminated
                if (input == null)
                {
                    return;
                }

                // is n an integer? Let's try it!
                isNInt = int.TryParse(input, out n);

                if (isNInt)
                {
                    // is n a POSITIVE integer?
                    isNPos = (int.Parse(input) > 0);
                }
            }

            Console.WriteLine("Please enter a value for the second integer, q:");

            while (!Console.KeyAvailable)
            {
                // restarts while loop every 360 milliseconds
                Thread.Sleep(360);
            }

            // multi-character input is processed through the Chelsea handler
            input = Chelsea();

            // if Chelsea returned null, that means the Escape key was pressed 
            // so the program gets terminated
            if (input == null)
            {
                return;
            }

            // is q an integer? Let's try it!
            isQInt = int.TryParse(input, out q);

            if (isQInt)
            {
                // is q a POSITIVE integer?
                isQPos = (int.Parse(input) > 0);
            }

            // if Chelsea returned an empty string, or a string that was 
            // neither an integer or a positive integer, or q is larger than n,
            // the user gets prompted until a valid input is provided and the 
            // program can continue
            while (input.Equals(String.Empty) || !isQInt || !isQPos || (q > n))
            {
                Console.WriteLine("INVALID INPUT");
                Console.WriteLine("Must be an integer greater than 0, but smaller than n.");
                Console.WriteLine("Please enter a value for the second integer, q:");

                while (!Console.KeyAvailable)
                {
                    // restarts while loop every 360 milliseconds
                    Thread.Sleep(360);
                }

                // multi-character input is processed through the Chelsea handler
                input = Chelsea();

                // if Chelsea returned null, that means the Escape key was pressed 
                // so the program gets terminated
                if (input == null)
                {
                    return;
                }

                // is q an integer? Let's try it!
                isQInt = int.TryParse(input, out q);

                if (isQInt)
                {
                    // is q a POSITIVE integer?
                    isQPos = (int.Parse(input) > 0);
                }
            }

            if (q.ToString().EndsWith("1"))
            {
                suffix = "st"; // if the multiple integer ends with a one, 'st' will be appended
            }
            else if (q.ToString().EndsWith("2"))
            {
                suffix = "nd"; // if the multiple integer ends with a two, 'nd' will be appended
            }
            else if (q.ToString().EndsWith("3"))
            {
                suffix = "rd"; // if the multiple integer ends with a three, 'rd' will be appended
            }
            else
            {
                suffix = "th"; // if the multiple integer ends with anything else, 'th' will be appended
            }

            Console.WriteLine("The sum of every {0}{1} number between 1 and {2} is:", q, suffix, n);

            // add every qth number between 1 and n
            for (int i = 1; i <= n; i++)
            {
                int s = q * i;

                Console.Write(s);

                sigma += s;

                bool checkNext = ((q * (i + 1)) <= n);

                if (checkNext)
                {
                    Console.Write(" + ");
                }
                else
                {
                    Console.WriteLine(" = {0}", sigma);
                    break;
                }
            }

            // add an extra line for fanciness
            Console.WriteLine();

            contTest = true; // flag set because main function has run

            Console.WriteLine("Want to play again? (Y/N):");

            while (!Console.KeyAvailable)
            {
                // restarts while loop every 360 milliseconds
                Thread.Sleep(360);
            }

        } // end SUM

        /// <summary>
        /// MOST_COMMON, a void function
        /// </summary>
        void MOST_COMMON()
        {
            chooserTest = false;                        // user is no longer in the Chooser method; gets reset every time MOST_COMMON function is called
            sumTest = false;                            // user is no longer in the SUM function; gets reset every time MOST_COMMON function is called
            contTest = false;                           // user is not being prompted to restart a function; gets reset every time the SUM function is called
            noMoreWords = false;                        // noMoreWords is set to false; gets reset every time MOST_COMMON function is called

            List<string> words = new List<string>();    // new list of string objects is created
            string word = String.Empty;                 // word is initially an empty string
            bool treWord = false;                       // treWord is set to false because there are not yet three words in the List<string> words
            // TODO - manage possibility of more than one word being the most common word
            // bool twoSame = false;

            Console.Title = "WELCOME TO THE MACHINE - THE MOST_COMMON FUNCTION";

            Console.WriteLine("***The MOST_COMMON Function***");
            Console.WriteLine("You will be prompted for a series of words.");
            Console.WriteLine("Please make sure you enter at least one word more than once.");
            Console.WriteLine("Enter CTRL-C when you are done entering at least three words.");
            Console.WriteLine("This function will return the most common word from the list you provided.");
            Console.WriteLine("For example, given the words 'apple', 'banana', 'orange', 'orange', 'apple', 'kiwi', 'apple'");
            Console.WriteLine("the function will return 'apple'.");

            do
            {
                Console.WriteLine("Please enter a word:");

                while (!Console.KeyAvailable)
                {
                    // restarts while loop every 360 milliseconds
                    Thread.Sleep(360);
                }

                // multi-character input is processed through the Chelsea handler
                word = Chelsea();

                // if Chelsea returned null, that means the Escape key was pressed 
                // so the program gets terminated
                if (word == null)
                {
                    return;
                }

                // if Chelsea returned an empty string, or CTRL+C is pressed 
                // but there are not yet three words in the List<string> words,
                // the user gets prompted until a valid input is provided and the 
                // program can continue
                while ((word.Equals(String.Empty)) || ((noMoreWords) && (!treWord)))
                {
                    if (word.Equals(String.Empty))
                    {
                        Console.WriteLine("INVALID INPUT");
                        Console.WriteLine("Must be a word that contains at least one non-whitespace character.");
                    }
                    else if ((noMoreWords) && (!treWord))
                    {
                        Console.WriteLine("LIST TOO SHORT");
                        Console.WriteLine("The list must have at least three words.");

                        noMoreWords = false;
                    }

                    Console.WriteLine("Please enter a word:");

                    while (!Console.KeyAvailable)
                    {
                        // restarts while loop every 360 milliseconds
                        Thread.Sleep(360);
                    }

                    // multi-character input is processed through the Chelsea handler
                    word = Chelsea();

                    // if Chelsea returned null, that means the Escape key was pressed 
                    // so the program gets terminated
                    if (word == null)
                    {
                        return;
                    }
                }

                Console.WriteLine("Thank you.");

                // word is added to the list
                words.Add(word);

                if (words.Count == 3)
                {
                    treWord = true; // flag is set once there are three words in the list
                }

            } while (!noMoreWords);

            // determining which word comes up the most times in the list
            // LINQ code from http://stackoverflow.com/a/355977
            var most = words.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();

            Console.WriteLine("In this list of words:");

            foreach (string w in words)
            {
                Console.WriteLine(" - {0}", w); // print out each word in the list
            }

            Console.WriteLine("The most common word is:");
            Console.WriteLine(most);

            // add an extra line for fanciness
            Console.WriteLine();

            contTest = true; // flag set because main function has run

            Console.WriteLine("Want to play again? (Y/N):");

            while (!Console.KeyAvailable)
            {
                // restarts while loop every 360 milliseconds
                Thread.Sleep(360);
            }

        } // end MOST_COMMON

        /// <summary>
        /// Main
        /// 
        /// I think we know what this does by now
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // setting up Console.BufferWidth and Console.WindowWidth, used in 
            // HandleInput() method
            Console.BufferWidth = buffer;
            Console.WindowWidth = Console.BufferWidth;

            // prevents program from terminating if CTRL+C is pressed
            Console.TreatControlCAsInput = true;

            // create new instance of the Program class
            Program program = new Program();

            // call to ThreadStarter method
            program.ThreadStarter();

            // call to Termination loop
            program.Termination();

        } // end Main

    } // end Program

} // end ConsoleApplication
