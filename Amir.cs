using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;


//
// COP 4365 - Spring 2022
//
// Homework #4: Traffic Study
//
// Description: This program is designed to simulate the working system of a 4-way stoplight: North, South, East, and West. This
//              utilizes the system library of System.Diagnostics to make use of a stopwatch, which allows the program to automatically
//              run without human intervention. This program is a Windows Forms application with a GUI that shows the stoplights and correspondent
//              color changes when the stoplights change. The GUI aslo contains a button that is handled by a method that triggers the whole system and
//              calls other helper methods to carry the system out. Another objective of the project is to analyze the number of cars coming to the intersection
//              from all the directions.
//
// File Name: Traffic Study
//
// By: Amir Aslamov
//
//


    //UPDATED!

namespace Smarter_Stoplight_Problem
{
    public partial class Form1 : Form
    {
        // Method Name: Form1
        // Description: this is a constructor that initializes the form application
        public Form1()
        {
            InitializeComponent();
        }

        //we create a stopwatch object that will determine the overall running time
        //of the program we will create a stopwatch object and a timespan object
        //outside to make them global
        Stopwatch stopwatch_overall = new Stopwatch();
        TimeSpan time_span_overall;

        //we also create 4 stoplight objects
        Stoplight north_stoplight = new Stoplight();
        Stoplight south_stoplight = new Stoplight();
        Stoplight east_stoplight = new Stoplight();
        Stoplight west_stoplight = new Stoplight();


        //We will create 4 seperate lists to hold the sequence for North, South, East, and West directions
        List<Car> CarsComingNorth = new List<Car>();
        List<Car> CarsComingSouth = new List<Car>();
        List<Car> CarsComingEast = new List<Car>();
        List<Car> CarsComingWest = new List<Car>();

        //we will also create global lists for each direction, these will help us find out the maximum number of cars
        //in a line at one time
        List<Car> CarsWaitingNorth = new List<Car>();
        List<Car> CarsWaitingSouth = new List<Car>();
        List<Car> CarsWaitingEast = new List<Car>();
        List<Car> CarsWaitingWest = new List<Car>();

        //these lists will contain the list of all cars that passed from each direction
        List<Car> CarsPassedNorth = new List<Car>();
        List<Car> CarsPassedSouth = new List<Car>();
        List<Car> CarsPassedEast = new List<Car>();
        List<Car> CarsPassedWest = new List<Car>();

        //these variables will hold the maximum number of cars waiting in line in each direction
        static int maxCarsNorth = 0;
        static int maxCarsSouth = 0;
        static int maxCarsEast = 0;
        static int maxCarsWest = 0;

        //these global variables will store the number of cars coming from each direction
        static int numCarsNorth = 0;
        static int numCarsSouth = 0;
        static int numCarsEast = 0;
        static int numCarsWest = 0;

        // Method Name: CycleNorth
        // Description: this method simulates the cycle of the colors of the north stoplight:
        // it creates a seperate stopwatch object and correspondingly changes the labels, color properties
        // of the picture boxes and the color attribute of the north stoplight object
        public void CycleNorth()
        {
            //we create a seperate timer for this stoplight
            Stopwatch north_watch = new Stopwatch();
            //we start the timer
            north_watch.Start();
            //we get the timspan of seconds elapsed
            TimeSpan ts_north = north_watch.Elapsed;

            //first the stoplight has to be green for 9 seconds
            //so we change the backcolor of the picture box
            North_Green_PX.BackColor = Color.Green;
            //the rest of the lights will turn to gray color
            North_Red_PX.BackColor = Color.Gray;
            North_Yellow_PX.BackColor = Color.Gray;

            //refresh the GUI
            Refresh();

            //we also change the color property of the north stopligh object
            north_stoplight.ChangeColor("Green");

            //we print the changes to the console
            Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //To deal with the cars, our basic idea is: we keep running the north method and keep checking the global
            //timer against the first car in each direction list, and see if the global timer's seconds are equal to 
            //the arrival seconds of each car in each direction; we know that the direction list is already sorted, so
            //we check the first car in the list as it is the earliest

            //first let's let all the cars waiting currently in the North direction because the stoplight is green at the moment
            //we take the cars from the start of the list as they are waiting in a queue
            //before we take cars from the queue of cars waitig, let's see if we have the maximum number of cars waiting in line
            maxCarsNorth = maxCarsNorth < CarsWaitingNorth.Count ? CarsWaitingNorth.Count : maxCarsNorth;
            while(CarsWaitingNorth.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                CarsWaitingNorth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                //print the info that this car hass passed the intersection
                Console.WriteLine("A car has passed from North: |Sequence Number: " + CarsWaitingNorth[0].GetSeqNum() + "|Arrival Time: " +
                    CarsWaitingNorth[0].GetArrTime() + "|Exit Time: " + CarsWaitingNorth[0].GetExitTime() + "|Wait Time: " + 
                    (CarsWaitingNorth[0].GetExitTime() - CarsWaitingNorth[0].GetArrTime()));
                //add this car to the list of already pased cars from North
                CarsPassedNorth.Add(CarsWaitingNorth[0]);
                //remove this car from the list of cars waiting in north direction
                CarsWaitingNorth.RemoveAt(0);
            }

            //green for 9 seconds, yellow for 3 seconds, the rest is red, we technically need 
            //the timing for the green and yellow colors, and the red color is just a default color
            //after a total of 12 seconds
            //but after 6 seconds this function calls the function for the south stoplight cycle
            //we will change the north light color inside that south stoplight cycle function
            while (ts_north.Seconds < 6)
            {
                //we keep getting the seconds elapsed from the overall timer
                time_span_overall = stopwatch_overall.Elapsed;
   
                //if 4 minutes have passed in the overall stopwatch, we terminate the program
                if (time_span_overall.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = ((int)Math.Round(time_span_overall.TotalSeconds)).ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_north = north_watch.Elapsed;

                //now let's check all the cars that come from each direction, because the list of cars is already sorted
                //based on the arrival times, it is enough to check the first car in the respective lists
                //becuase the North stoplight is currently green, instead of putting the cars to the queue of waiting cars,
                //we let them pass, the other cars from other directions will be put to the respective queues of waiting cars
                
                //check the car from North
                //first see if there are any cars left coming
                if (CarsComingNorth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingNorth[0].GetArrTime())
                    {
                        //check that this car has not already been passed - check sequence number because it must be unique
                        //this check is needed becuase the execution time of program is faster than the stopwatch timer
                        if (CarsPassedNorth.Count != 0)
                        {
                            if (CarsPassedNorth[CarsPassedNorth.Count - 1].GetSeqNum() != CarsComingNorth[0].GetSeqNum())
                            {
                                //set the exit time of the car
                                CarsComingNorth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from North: |Sequence Number: " + CarsComingNorth[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingNorth[0].GetArrTime() + "|Exit Time: " + CarsComingNorth[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingNorth[0].GetExitTime() - CarsComingNorth[0].GetArrTime()));
                                //add this car to the list of already pased cars from North
                                CarsPassedNorth.Add(CarsComingNorth[0]);
                                //remove this car from the list of cars coming from North
                                CarsComingNorth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //just add this car to the list of cars passed
                            //set its exit time
                            CarsComingNorth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                            //print the info that this car has passed the intersection
                            Console.WriteLine("A car has passed from North: |Sequence Number: " + CarsComingNorth[0].GetSeqNum() + "|Arrival Time: " +
                                CarsComingNorth[0].GetArrTime() + "|Exit Time: " + CarsComingNorth[0].GetExitTime() + "|Wait Time: " +
                                (CarsComingNorth[0].GetExitTime() - CarsComingNorth[0].GetArrTime()));
                            //add this car to the list of already pased cars from North
                            CarsPassedNorth.Add(CarsComingNorth[0]);
                            //remove this car from the list of cars coming from North
                            CarsComingNorth.RemoveAt(0);
                        }
                    }
                }
                //check the car from South
                //first see if there are any cars left coming
                if (CarsComingSouth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingSouth[0].GetArrTime())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (CarsWaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingSouth[CarsWaitingSouth.Count - 1].GetSeqNum() != CarsComingSouth[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingSouth.Add(CarsComingSouth[0]);
                                //remove this car from the list of cars coming from South
                                CarsComingSouth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingSouth.Add(CarsComingSouth[0]);
                            //remove this car from the list of cars coming from South
                            CarsComingSouth.RemoveAt(0);
                        }
                    }
                }
                //check the car from East
                //first see if there are any cars left coming
                if (CarsComingEast.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingEast[0].GetArrTime())
                    {
                        //becuase currently the East stoplight is red, we put the cars waiting in line
                        if (CarsWaitingEast.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingEast[CarsWaitingEast.Count - 1].GetSeqNum() != CarsComingEast[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingEast.Add(CarsComingEast[0]);
                                //remove this car from the list of cars coming from East
                                CarsComingEast.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingEast.Add(CarsComingEast[0]);
                            //remove this car from the list of cars coming from East
                            CarsComingEast.RemoveAt(0);
                        }
                    }
                }
                //check the car from West
                //first see if there are any cars left coming
                if (CarsComingWest.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingWest[0].GetArrTime())
                    {
                        //becuase currently the West stoplight is red, we put the cars waiting in line
                        if (CarsWaitingWest.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingWest[CarsWaitingWest.Count - 1].GetSeqNum() != CarsComingWest[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingWest.Add(CarsComingWest[0]);
                                //remove this car from the list of cars coming from West
                                CarsComingWest.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingWest.Add(CarsComingWest[0]);
                            //remove this car from the list of cars coming from West
                            CarsComingWest.RemoveAt(0);
                        }
                    }
                }
            }

            //we stop the watch for the north light
            north_watch.Stop();

            //if 6 seconds have passed, we trigger the south stoplight cycle
            CycleSouth();
        }


        // Method Name: CycleSouth
        // Description: this method simulates the cycle of the colors of the south stoplight:
        // it creates a seperate stopwatch object and correspondingly changes the labels, color properties
        // of the picture boxes and the color attribute of the south stoplight object
        public void CycleSouth()
        {
            //we create a seperate timer for this stoplight
            Stopwatch south_watch = new Stopwatch();
            //we start the timer
            south_watch.Start();
            //we get the timspan of seconds elapsed
            TimeSpan ts_south = south_watch.Elapsed;

            //first the stoplight has to be green for 9 seconds
            //so we change the backcolor of the picture box
            South_Green_PX.BackColor = Color.Green;
            //the rest of the lights will turn to gray color
            South_Red_PX.BackColor = Color.Gray;
            South_Yellow_PX.BackColor = Color.Gray;

            //refresh the GUI
            Refresh();
            //we also change the color property of the south stopligh object
            south_stoplight.ChangeColor("Green");

            //we print the changes to the console
            Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //because the stopwatch is slower than the execution time of the program, we need to make sure we don't repeatedly print 
            //the same output inisde the while loop, so we will create helper boolean variables to help us with printing the changes once
            bool north_light_changed_yellow = false;
            bool north_light_changed_red = false;
            bool south_light_changed = false;


            //first let's let all the cars waiting currently in the South direction because the stoplight is green at the moment
            //we take the cars from the start of the list as they are waiting in a queue
            //before we take cars from the queue of cars waitig, let's see if we have the maximum number of cars waiting in line
            maxCarsSouth = maxCarsSouth < CarsWaitingSouth.Count ? CarsWaitingSouth.Count : maxCarsSouth;
            while (CarsWaitingSouth.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                CarsWaitingSouth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                //print the info that this car hass passed the intersection
                Console.WriteLine("A car has passed from South: |Sequence Number: " + CarsWaitingSouth[0].GetSeqNum() + "|Arrival Time: " +
                    CarsWaitingSouth[0].GetArrTime() + "|Exit Time: " + CarsWaitingSouth[0].GetExitTime() + "|Wait Time: " +
                    (CarsWaitingSouth[0].GetExitTime() - CarsWaitingSouth[0].GetArrTime()));
                //add this car to the list of already pased cars from South
                CarsPassedSouth.Add(CarsWaitingSouth[0]);
                //remove this car from the list of cars waiting in north direction
                CarsWaitingSouth.RemoveAt(0);
            }

            //green for 9 seconds, yellow for 3 seconds, the rest is red, we technically need 
            //the timing for the green and yellow colors, and the red color is just a default color
            //after a total of 12 seconds
            while (ts_south.Seconds < 12)
            {
                //we keep getting the seconds elapsed from the overall timer
                time_span_overall = stopwatch_overall.Elapsed;
                //if 4 minutes have passed in the overall stopwatch, we terminate the program
                if (time_span_overall.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = ((int)Math.Round(time_span_overall.TotalSeconds)).ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_south = south_watch.Elapsed;

                //if the timer for the south stoplight is 2, it means 3 seconds have passed, which means
                //a total of 9 seconds have passed for the north stoplight stopwatch: we change the color to yellow
                if (ts_south.Seconds == 3)
                {
                    if(!north_light_changed_yellow)
                    {
                        north_stoplight.ChangeColor("Yellow");
                        //we turn on the yellow light
                        North_Yellow_PX.BackColor = Color.Yellow;
                        //the rest of the lights will turn to gray color
                        North_Red_PX.BackColor = Color.Gray;
                        North_Green_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ",
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                            east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        north_light_changed_yellow = true;
                    }
                }
                //else if it is 5, 6 seconds have passed - total of 12 seconds for the north stoplight stopwatch: 
                //we change north light color to red
                else if (ts_south.Seconds == 6)
                {                  
                    if (!north_light_changed_red)
                    {
                        //we leave the stoplight at the default color of red
                        north_stoplight.ChangeColor("Red");
                        North_Red_PX.BackColor = Color.Red;
                        //we turn the rest of the light to the gray color
                        North_Green_PX.BackColor = Color.Gray;
                        North_Yellow_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                            east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        north_light_changed_red = true;
                    }
                }
                //if 9 seconds have passed, we change the stoplight color to yellow
                else if (ts_south.Seconds == 9)
                {                
                    if (!south_light_changed)
                    {
                        south_stoplight.ChangeColor("Yellow");
                        //we turn on the yellow light
                        South_Yellow_PX.BackColor = Color.Yellow;
                        //we turn the rest of the colors to gray color
                        South_Red_PX.BackColor = Color.Gray;
                        South_Green_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ",
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                            east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        south_light_changed = true;
                    }
                }

                //let's take care of cars coming from all directions
                //check cars from North
                if (CarsComingNorth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingNorth[0].GetArrTime())
                    {
                        //if the North stoplight is still green we try to pass the car
                        if (north_light_changed_red == false && north_light_changed_yellow == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (CarsPassedNorth.Count != 0)
                            {
                                if (CarsPassedNorth[CarsPassedNorth.Count - 1].GetSeqNum() != CarsComingNorth[0].GetSeqNum())
                                {
                                    //set the exit time of the car
                                    CarsComingNorth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car has passed from North: |Sequence Number: " + CarsComingNorth[0].GetSeqNum() + "|Arrival Time: " +
                                        CarsComingNorth[0].GetArrTime() + "|Exit Time: " + CarsComingNorth[0].GetExitTime() + "|Wait Time: " +
                                        (CarsComingNorth[0].GetExitTime() - CarsComingNorth[0].GetArrTime()));
                                    //add this car to the list of already pased cars from North
                                    CarsPassedNorth.Add(CarsComingNorth[0]);
                                    //remove this car from the list of cars coming from North
                                    CarsComingNorth.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                CarsComingNorth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from North: |Sequence Number: " + CarsComingNorth[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingNorth[0].GetArrTime() + "|Exit Time: " + CarsComingNorth[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingNorth[0].GetExitTime() - CarsComingNorth[0].GetArrTime()));
                                //add this car to the list of already pased cars from North
                                CarsPassedNorth.Add(CarsComingNorth[0]);
                                //remove this car from the list of cars coming from North
                                CarsComingNorth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the North stoplight is no longer green, we add the car to the waiting list
                            if (CarsWaitingNorth.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (CarsWaitingNorth[CarsWaitingNorth.Count - 1].GetSeqNum() != CarsComingNorth[0].GetSeqNum())
                                {
                                    //add to the waiting list
                                    CarsWaitingNorth.Add(CarsComingNorth[0]);
                                    //remove this car from the list of cars coming from North
                                    CarsComingNorth.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                CarsWaitingNorth.Add(CarsComingNorth[0]);
                                //remove this car from the list of cars coming from North
                                CarsComingNorth.RemoveAt(0);
                            }
                        }
                    }
                }
                //check cars from South
                if (CarsComingSouth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingSouth[0].GetArrTime())
                    {
                        //if the South stoplight is still green we try to pass the car
                        if (south_light_changed == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (CarsPassedSouth.Count != 0)
                            {
                                if (CarsPassedSouth[CarsPassedSouth.Count - 1].GetSeqNum() != CarsComingSouth[0].GetSeqNum())
                                {
                                    //set the exit time of the car
                                    CarsComingSouth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car has passed from South: |Sequence Number: " + CarsComingSouth[0].GetSeqNum() + "|Arrival Time: " +
                                        CarsComingSouth[0].GetArrTime() + "|Exit Time: " + CarsComingSouth[0].GetExitTime() + "|Wait Time: " +
                                        (CarsComingSouth[0].GetExitTime() - CarsComingSouth[0].GetArrTime()));
                                    //add this car to the list of already pased cars from South
                                    CarsPassedSouth.Add(CarsComingSouth[0]);
                                    //remove this car from the list of cars coming from South
                                    CarsComingSouth.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                CarsComingSouth[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from South: |Sequence Number: " + CarsComingSouth[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingSouth[0].GetArrTime() + "|Exit Time: " + CarsComingSouth[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingSouth[0].GetExitTime() - CarsComingSouth[0].GetArrTime()));
                                //add this car to the list of already pased cars from South
                                CarsPassedSouth.Add(CarsComingSouth[0]);
                                //remove this car from the list of cars coming from South
                                CarsComingSouth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the South stoplight is no longer green, we add the car to the waiting list
                            if (CarsWaitingSouth.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (CarsWaitingSouth[CarsWaitingSouth.Count - 1].GetSeqNum() != CarsComingSouth[0].GetSeqNum())
                                {
                                    //add to the waiting list
                                    CarsWaitingSouth.Add(CarsComingSouth[0]);
                                    //remove this car from the list of cars coming from South
                                    CarsComingSouth.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                CarsWaitingSouth.Add(CarsComingSouth[0]);
                                //remove this car from the list of cars coming from South
                                CarsComingSouth.RemoveAt(0);
                            }
                        }
                    }
                }
                //check the car from East
                //first see if there are any cars left coming
                if (CarsComingEast.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingEast[0].GetArrTime())
                    {
                        //becuase currently the East stoplight is red, we put the cars waiting in line
                        if (CarsWaitingEast.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingEast[CarsWaitingEast.Count - 1].GetSeqNum() != CarsComingEast[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingEast.Add(CarsComingEast[0]);
                                //remove this car from the list of cars coming from East
                                CarsComingEast.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingEast.Add(CarsComingEast[0]);
                            //remove this car from the list of cars coming from East
                            CarsComingEast.RemoveAt(0);
                        }
                    }
                }
                //check the car from West
                //first see if there are any cars left coming
                if (CarsComingWest.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingWest[0].GetArrTime())
                    {
                        //becuase currently the West stoplight is red, we put the cars waiting in line
                        if (CarsWaitingWest.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingWest[CarsWaitingWest.Count - 1].GetSeqNum() != CarsComingWest[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingWest.Add(CarsComingWest[0]);
                                //remove this car from the list of cars coming from West
                                CarsComingWest.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingWest.Add(CarsComingWest[0]);
                            //remove this car from the list of cars coming from West
                            CarsComingWest.RemoveAt(0);
                        }
                    }
                }
            }

            //we leave the stoplight at the default color of red
            south_stoplight.ChangeColor("Red");
            South_Red_PX.BackColor = Color.Red;
            //we turn the rest of the light to gray color
            South_Green_PX.BackColor = Color.Gray;
            South_Yellow_PX.BackColor = Color.Gray;
            Refresh();

            //stop the stopwatch for the south light
            south_watch.Stop();
        }


        // Method Name: CycleEast
        // Description: this method simulates the cycle of the colors of the east stoplight:
        // it creates a seperate stopwatch object and correspondingly changes the labels, color properties
        // of the picture boxes and the color attribute of the south stoplight object
        public void CycleEast()
        {
            //we create a seperate timer for this stoplight
            Stopwatch east_watch = new Stopwatch();
            //we start the timer
            east_watch.Start();
            //we get the timspan of seconds elapsed
            TimeSpan ts_east = east_watch.Elapsed;

            //first the stoplight has to be green for 9 seconds
            //so we change the backcolor of the picture box
            East_Green_PX.BackColor = Color.Green;
            //the rest of the colors of the stoplight have to be gray
            East_Red_PX.BackColor = Color.Gray;
            East_Yellow_PX.BackColor = Color.Gray;

            //refresh the GUI
            Refresh();

            //we also change the color property of the south stopligh object
            east_stoplight.ChangeColor("Green");

            //we print the changes to the console
            Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(), 
                east_stoplight.GetColour(), west_stoplight.GetColour());


            //first let's let all the cars waiting currently in the East direction because the stoplight is green at the moment
            //we take the cars from the start of the list as they are waiting in a queue
            //before we take cars from the queue of cars waitig, let's see if we have the maximum number of cars waiting in line
            maxCarsEast = maxCarsEast < CarsWaitingEast.Count ? CarsWaitingEast.Count : maxCarsEast;
            while (CarsWaitingEast.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                CarsWaitingEast[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                //print the info that this car hass passed the intersection
                Console.WriteLine("A car has passed from East: |Sequence Number: " + CarsWaitingEast[0].GetSeqNum() + "|Arrival Time: " +
                    CarsWaitingEast[0].GetArrTime() + "|Exit Time: " + CarsWaitingEast[0].GetExitTime() + "|Wait Time: " +
                    (CarsWaitingEast[0].GetExitTime() - CarsWaitingEast[0].GetArrTime()));
                //add this car to the list of already pased cars from East
                CarsPassedEast.Add(CarsWaitingEast[0]);
                //remove this car from the list of cars waiting in north direction
                CarsWaitingEast.RemoveAt(0);
            }

            //green for 9 seconds, yellow for 3 seconds, the rest is red, we technically need 
            //the timing for the green and yellow colors, and the red color is just a default color
            //after a total of 12 seconds
            //but after 6 seconds this function calls the function for the west stoplight cycle
            //we will change the east light color inside that west stoplight cycle function
            while (ts_east.Seconds < 6)
            {
                //we keep getting the seconds elapsed from the overall timer
                time_span_overall = stopwatch_overall.Elapsed;
                //if 4 minutes have passed in the overall stopwatch, we terminate the program
                if (time_span_overall.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }
                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = ((int)Math.Round(time_span_overall.TotalSeconds)).ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_east = east_watch.Elapsed;


                //now let's check all the cars that come from each direction, because the list of cars is already sorted
                //based on the arrival times, it is enough to check the first car in the respective lists
                //becuase the East stoplight is currently green, instead of putting the cars to the queue of waiting cars,
                //we let them pass, the other cars from other directions will be put to the respective queues of waiting cars

                //check the car from East
                //first see if there are any cars left coming
                if (CarsComingEast.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingEast[0].GetArrTime())
                    {
                        //check that this car has not already been passed - check sequence number because it must be unique
                        //this check is needed becuase the execution time of program is faster than the stopwatch timer
                        if (CarsPassedEast.Count != 0)
                        {
                            if (CarsPassedEast[CarsPassedEast.Count - 1].GetSeqNum() != CarsComingEast[0].GetSeqNum())
                            {
                                //set the exit time of the car
                                CarsComingEast[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from East: |Sequence Number: " + CarsComingEast[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingEast[0].GetArrTime() + " Exit Time: " + CarsComingEast[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingEast[0].GetExitTime() - CarsComingEast[0].GetArrTime()));
                                //add this car to the list of already pased cars from East
                                CarsPassedEast.Add(CarsComingEast[0]);
                                //remove this car from the list of cars coming from East
                                CarsComingEast.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //just add this car to the list of cars passed
                            //set the exit time of the car
                            CarsComingEast[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                            //print the info that this car has passed the intersection
                            Console.WriteLine("A car has passed from East: |Sequence Number: " + CarsComingEast[0].GetSeqNum() + "|Arrival Time: " +
                                CarsComingEast[0].GetArrTime() + "|Exit Time: " + CarsComingEast[0].GetExitTime() + "|Wait Time: " +
                                (CarsComingEast[0].GetExitTime() - CarsComingEast[0].GetArrTime()));
                            //add this car to the list of already pased cars from East
                            CarsPassedEast.Add(CarsComingEast[0]);
                            //remove this car from the list of cars coming from East
                            CarsComingEast.RemoveAt(0);
                        }
                    }
                }
                //check the car from South
                //first see if there are any cars left coming
                if (CarsComingSouth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingSouth[0].GetArrTime())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (CarsWaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingSouth[CarsWaitingSouth.Count - 1].GetSeqNum() != CarsComingSouth[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingSouth.Add(CarsComingSouth[0]);
                                //remove this car from the list of cars coming from South
                                CarsComingSouth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingSouth.Add(CarsComingSouth[0]);
                            //remove this car from the list of cars coming from South
                            CarsComingSouth.RemoveAt(0);
                        }
                    }
                }
                //check the car from North
                //first see if there are any cars left coming
                if (CarsComingNorth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingNorth[0].GetArrTime())
                    {
                        //becuase currently the North stoplight is red, we put the cars waiting in line
                        if (CarsWaitingNorth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingNorth[CarsWaitingNorth.Count - 1].GetSeqNum() != CarsComingNorth[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingNorth.Add(CarsComingNorth[0]);
                                //remove this car from the list of cars coming from North
                                CarsComingNorth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingNorth.Add(CarsComingNorth[0]);
                            //remove this car from the list of cars coming from North
                            CarsComingNorth.RemoveAt(0);
                        }
                    }
                }
                //check the car from West
                //first see if there are any cars left coming
                if (CarsComingWest.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingWest[0].GetArrTime())
                    {
                        //becuase currently the West stoplight is red, we put the cars waiting in line
                        if (CarsWaitingWest.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingWest[CarsWaitingWest.Count - 1].GetSeqNum() != CarsComingWest[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingWest.Add(CarsComingWest[0]);
                                //remove this car from the list of cars coming from West
                                CarsComingWest.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingWest.Add(CarsComingWest[0]);
                            //remove this car from the list of cars coming from West
                            CarsComingWest.RemoveAt(0);
                        }
                    }
                }
            }

            //we stop the stopwatch for the east light
            east_watch.Stop();
            //if 6 seconds have passed, we trigger the west stoplight cycle
            CycleWest();           
        }


        // Method Name: CycleWest
        // Description: this method simulates the cycle of the colors of the west stoplight:
        // it creates a seperate stopwatch object and correspondingly changes the labels, color properties
        // of the picture boxes and the color attribute of the south stoplight object
        public void CycleWest()
        {
            //we create a seperate timer for this stoplight
            Stopwatch west_watch = new Stopwatch();
            //we start the timer
            west_watch.Start();
            //we get the timspan of seconds elapsed
            TimeSpan ts_west = west_watch.Elapsed;

            //first the stoplight has to be green for 9 seconds
            //so we change the backcolor of the picture box
            West_Green_PX.BackColor = Color.Green;
            //the rest of the colors of the stoplight have to be gray color
            West_Red_PX.BackColor = Color.Gray;
            West_Yellow_PX.BackColor = Color.Gray;

            //refresh the GUI
            Refresh();

            //we also change the color property of the south stopligh object
            west_stoplight.ChangeColor("Green");

            //we print the changes to the console
            Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(), south_stoplight.GetColour(), 
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //first let's let all the cars waiting currently in the West direction because the stoplight is green at the moment
            //we take the cars from the start of the list as they are waiting in a queue
            //before we take cars from the queue of cars waitig, let's see if we have the maximum number of cars waiting in line
            maxCarsWest = maxCarsWest < CarsWaitingWest.Count ? CarsWaitingWest.Count : maxCarsWest;
            while (CarsWaitingWest.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                CarsWaitingWest[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                //print the info that this car has passed the intersection
                Console.WriteLine("A car has passed from West: |Sequence Number: " + CarsWaitingWest[0].GetSeqNum() + "|Arrival Time: " +
                    CarsWaitingWest[0].GetArrTime() + "|Exit Time: " + CarsWaitingWest[0].GetExitTime() + "|Wait Time: " +
                    (CarsWaitingWest[0].GetExitTime() - CarsWaitingWest[0].GetArrTime()));
                //add this car to the list of already pased cars from West
                CarsPassedWest.Add(CarsWaitingWest[0]);
                //remove this car from the list of cars waiting in north direction
                CarsWaitingWest.RemoveAt(0);
            }

            //because the stopwatch is slower than the execution time of the program, we need to make sure we don't repeatedly print 
            //the same output inisde the while loop, so we will create helper boolean variables to help us with printing the changes once
            bool east_light_changed_yellow = false;
            bool east_light_changed_red = false;
            bool west_light_changed = false;

            //green for 9 seconds, yellow for 3 seconds, the rest is red, we technically need 
            //the timing for the green and yellow colors, and the red color is just a default color
            //after a total of 12 seconds
            while (ts_west.Seconds < 12)
            {
                //we keep getting the seconds elapsed from the overall timer
                time_span_overall = stopwatch_overall.Elapsed;
                //if 1 minute has passed in the overall stopwatch, we terminate the program
                if (time_span_overall.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = ((int)Math.Round(time_span_overall.TotalSeconds)).ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_west = west_watch.Elapsed;

                //if the timer for the west stoplight is 2, it means 3 seconds have passed, which means
                //a total of 9 seconds have passed for the east stoplight stopwatch: we change the color to yellow
                if (ts_west.Seconds == 3)
                {              
                    if (!east_light_changed_yellow)
                    {
                        east_stoplight.ChangeColor("Yellow");
                        //we change turn the yellow light
                        East_Yellow_PX.BackColor = Color.Yellow;
                        //the rest of the colors will be gray
                        East_Green_PX.BackColor = Color.Gray;
                        East_Red_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(),
                            south_stoplight.GetColour(), east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        east_light_changed_yellow = true;
                    }
                }
                //else if it is 5, 6 seconds have passed - total of 12 seconds for the east stoplight stopwatch: 
                //we change east light color to red
                else if (ts_west.Seconds == 6)
                {
                    if (!east_light_changed_red)
                    {
                        //we leave the stoplight at the default color of red
                        east_stoplight.ChangeColor("Red");
                        East_Red_PX.BackColor = Color.Red;
                        //the rest turn to gray
                        East_Yellow_PX.BackColor = Color.Gray;
                        East_Green_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ",
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(),
                            south_stoplight.GetColour(), east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        east_light_changed_red = true;
                    }
                }
                //if 9 seconds have passed, we change the stoplight color to yellow
                else if (ts_west.Seconds == 9)
                {
                    if (!west_light_changed)
                    {
                        west_stoplight.ChangeColor("Yellow");
                        //we turn on the yellow light 
                        West_Yellow_PX.BackColor = Color.Yellow;
                        //the rest of the lights will turn to gray
                        West_Red_PX.BackColor = Color.Gray;
                        West_Green_PX.BackColor = Color.Gray;
                        Refresh();

                        //we print the changes to the console
                        Console.WriteLine("{0, 3} {1, -2} {2, 9} {3, -13} {4, -14} {5, -16} {6, -13}", " ", 
                            (int)Math.Round(time_span_overall.TotalSeconds), " ", north_stoplight.GetColour(),
                            south_stoplight.GetColour(), east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        west_light_changed = true;
                    }
                }
                //let's take care of cars coming from all directions
                //check cars from East
                if (CarsComingEast.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingEast[0].GetArrTime())
                    {
                        //if the East stoplight is still green we try to pass the car
                        if (east_light_changed_red == false && east_light_changed_yellow == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (CarsPassedEast.Count != 0)
                            {
                                if (CarsPassedEast[CarsPassedEast.Count - 1].GetSeqNum() != CarsComingEast[0].GetSeqNum())
                                {
                                    //set the exit time of the car
                                    CarsComingEast[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car has passed from East: |Sequence Number: " + CarsComingEast[0].GetSeqNum() + "|Arrival Time: " +
                                        CarsComingEast[0].GetArrTime() + "|Exit Time: " + CarsComingEast[0].GetExitTime() + "|Wait Time: " +
                                        (CarsComingEast[0].GetExitTime() - CarsComingEast[0].GetArrTime()));
                                    //add this car to the list of already pased cars from East
                                    CarsPassedEast.Add(CarsComingEast[0]);
                                    //remove this car from the list of cars coming from East
                                    CarsComingEast.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                CarsComingEast[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from East: |Sequence Number: " + CarsComingEast[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingEast[0].GetArrTime() + "|Exit Time: " + CarsComingEast[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingEast[0].GetExitTime() - CarsComingEast[0].GetArrTime()));
                                //add this car to the list of already pased cars from East
                                CarsPassedEast.Add(CarsComingEast[0]);
                                //remove this car from the list of cars coming from East
                                CarsComingEast.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the East stoplight is no longer green, we add the car to the waiting list
                            if (CarsWaitingEast.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (CarsWaitingEast[CarsWaitingEast.Count - 1].GetSeqNum() != CarsComingEast[0].GetSeqNum())
                                {
                                    //add to the waiting list
                                    CarsWaitingEast.Add(CarsComingEast[0]);
                                    //remove this car from the list of cars coming from East
                                    CarsComingEast.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                CarsWaitingEast.Add(CarsComingEast[0]);
                                //remove this car from the list of cars coming from East
                                CarsComingEast.RemoveAt(0);
                            }
                        }
                    }
                }
                //check cars from West
                if (CarsComingWest.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingWest[0].GetArrTime())
                    {
                        //if the West stoplight is still green we try to pass the car
                        if (west_light_changed == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (CarsPassedWest.Count != 0)
                            {
                                if (CarsPassedWest[CarsPassedWest.Count - 1].GetSeqNum() != CarsComingWest[0].GetSeqNum())
                                {
                                    //set the exit time of the car
                                    CarsComingWest[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car has passed from West: |Sequence Number: " + CarsComingWest[0].GetSeqNum() + "|Arrival Time: " +
                                        CarsComingWest[0].GetArrTime() + "|Exit Time: " + CarsComingWest[0].GetExitTime() + "|Wait Time: " +
                                        (CarsComingWest[0].GetExitTime() - CarsComingWest[0].GetArrTime()));
                                    //add this car to the list of already pased cars from West
                                    CarsPassedWest.Add(CarsComingWest[0]);
                                    //remove this car from the list of cars coming from West
                                    CarsComingWest.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                CarsComingWest[0].SetExitTime((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car has passed from West: |Sequence Number: " + CarsComingWest[0].GetSeqNum() + "|Arrival Time: " +
                                    CarsComingWest[0].GetArrTime() + "|Exit Time: " + CarsComingWest[0].GetExitTime() + "|Wait Time: " +
                                    (CarsComingWest[0].GetExitTime() - CarsComingWest[0].GetArrTime()));
                                //add this car to the list of already pased cars from West
                                CarsPassedWest.Add(CarsComingWest[0]);
                                //remove this car from the list of cars coming from West
                                CarsComingWest.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the West stoplight is no longer green, we add the car to the waiting list
                            if (CarsWaitingWest.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (CarsWaitingWest[CarsWaitingWest.Count - 1].GetSeqNum() != CarsComingWest[0].GetSeqNum())
                                {
                                    //add to the waiting list
                                    CarsWaitingWest.Add(CarsComingWest[0]);
                                    //remove this car from the list of cars coming from West
                                    CarsComingWest.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                CarsWaitingWest.Add(CarsComingWest[0]);
                                //remove this car from the list of cars coming from West
                                CarsComingWest.RemoveAt(0);
                            }
                        }
                    }
                }
                //check the car from North
                //first see if there are any cars left coming
                if (CarsComingNorth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingNorth[0].GetArrTime())
                    {
                        //becuase currently the North stoplight is red, we put the cars waiting in line
                        if (CarsWaitingNorth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingNorth[CarsWaitingNorth.Count - 1].GetSeqNum() != CarsComingNorth[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingNorth.Add(CarsComingNorth[0]);
                                //remove this car from the list of cars coming from North
                                CarsComingNorth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingNorth.Add(CarsComingNorth[0]);
                            //remove this car from the list of cars coming from North
                            CarsComingNorth.RemoveAt(0);
                        }
                    }
                }
                //check the car from South
                //first see if there are any cars left coming
                if (CarsComingSouth.Count != 0)
                {
                    if ((int)Math.Round(stopwatch_overall.Elapsed.TotalSeconds) == CarsComingSouth[0].GetArrTime())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (CarsWaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (CarsWaitingSouth[CarsWaitingSouth.Count - 1].GetSeqNum() != CarsComingSouth[0].GetSeqNum())
                            {
                                //add to the waiting list
                                CarsWaitingSouth.Add(CarsComingSouth[0]);
                                //remove this car from the list of cars coming from South
                                CarsComingSouth.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            CarsWaitingSouth.Add(CarsComingSouth[0]);
                            //remove this car from the list of cars coming from South
                            CarsComingWest.RemoveAt(0);
                        }
                    }
                }
            }

            //we leave the stoplight at the default color of red
            west_stoplight.ChangeColor("Red");
            West_Red_PX.BackColor = Color.Red;
            //the rest of the lights till turn to gray color
            West_Green_PX.BackColor = Color.Gray;
            West_Yellow_PX.BackColor = Color.Gray;
            Refresh();

            //we stop the stopwatch for the west light
            west_watch.Stop();
        }


        // Method Name: Start_BTN_Click
        // Description: this is an event handler method for the start button, when the user clicks on it
        // this method starts off the system, causes the stoplights to change colors and calls other helper functions
        // to cause the progression of the cycles inside the system
        private void Start_BTN_Click(object sender, EventArgs e)
        {
            //we change the text value of the status label to display the system is on
            Status_LBL.Text = "On Regular Cycle";

            //we trigger the global timer
            stopwatch_overall.Start();
            time_span_overall = stopwatch_overall.Elapsed;

            //we know that initially the north light's color is green, while that of the rest is red
            north_stoplight.ChangeColor("Green");
            south_stoplight.ChangeColor("Red");
            west_stoplight.ChangeColor("Red");
            east_stoplight.ChangeColor("Red");

            //we print the initial setup
            Console.WriteLine("\n\n{0} {1, 12} {2, 12} {3, 15} {4, 15}", "Current Time", "North Light", "South Light", 
                "East Light", "West Light");
            Console.WriteLine("{0} {1, 12} {1, 12} {2, 15} {3, 15}", "____________", "___________", "___________", 
                "__________", "__________");

            //The idea of the program:
            //CYCLE:
            //NORTH starts GREEN
            //After 6 seconds => SOUTH turns GREEN
            //After SOUTH turns RED => EAST turns GREEN
            //After 6 seconds => WEST turns GREEN
            //After WEST turns RED => NORTH starts GREEN

            //we will read the data from the file and put the data into an array
            string[] data = File.ReadAllLines("HW #4 Data.txt");
            //now we call the helper method to seperate the cars into distinct lists
            SeperateDirections(data);

            //once we got the cars into the direction lists, we can assign the number of cars coming from
            //each direction - it will just be the count of each list
            numCarsNorth = CarsComingNorth.Count;
            numCarsSouth = CarsComingSouth.Count;
            numCarsEast = CarsComingEast.Count;
            numCarsWest = CarsComingWest.Count;

            //we will keep running the system while 60 seconds are not passed
            //so we keep running until 4 minutes has not elapsed
            while (time_span_overall.Minutes < 4)
            {
                time_span_overall = stopwatch_overall.Elapsed;
                //we change the text value of the timer label to display the seconds elapsed dynamically
                Timer_LBL.Text = time_span_overall.Seconds.ToString();
                //refresh the GUI
                Refresh();
                //the cycle starts from the north stoplight
                //the north stoplight, after 6 seconds, triggers the south stoplight
                //once the control returns to this function after the 2 calls of the helper
                //functions, we know that the south stoplight finished its cycle, so we start
                //the east stoplight cycle, which in turn triggers the west stoplight after 6 seconds
                CycleNorth();
                CycleEast();
            }   
          
            //we stop the global stopwatch of the overall program
            stopwatch_overall.Stop();
            //PrintResults();
            VerifyCarsPrint_2();
        }

        // A helper method to accept an array of strings and filter them into seperate direction lists
        public void SeperateDirections(string[] data)
        {
            //we will loop through the list of strings and determine what direction and what arrival times are
            //at each iteration we will create a new Car object and depending on the direction put it into the list
            foreach (var line in data)
            {
                //check the first letter for the direction of the car
                switch(line[0])
                {
                    case 'N':
                        Car car_N = new Car(CarsComingNorth.Count+1, int.Parse(line.Substring(1)));
                        CarsComingNorth.Add(car_N);
                        break;
                    case 'S':
                        Car car_S = new Car(CarsComingSouth.Count + 1, int.Parse(line.Substring(1)));
                        CarsComingSouth.Add(car_S);
                        break;
                    case 'E':
                        Car car_E = new Car(CarsComingEast.Count + 1, int.Parse(line.Substring(1)));
                        CarsComingEast.Add(car_E);
                        break;
                    case 'W':
                        Car car_W = new Car(CarsComingWest.Count + 1, int.Parse(line.Substring(1)));
                        CarsComingWest.Add(car_W);
                        break;
                    default:
                        MessageBox.Show("Error! The car's direction is not identified!");
                        break;
                }
            }
        }

        //helper method to print the results
        public void PrintResults()
        {
            Console.WriteLine("\n\nNumber of Cars That Came From Each Direction: ");
            Console.WriteLine("North: " + CarsPassedNorth.Count + " | South: " + CarsPassedSouth.Count +
                " | East: " + CarsPassedEast.Count + " | West: " + CarsPassedWest.Count);

            Console.WriteLine("\nThe Maximum Size of Line of Cars that had to Wait to pass through: ");
            Console.WriteLine("North: " + maxCarsNorth + " | South: " + maxCarsSouth + " | East: " + maxCarsEast +
                " | West: " + maxCarsWest);

            //average waiting time is the total wait time of all cars coming from each diriction divided by the number
            //of cars that passed from respective directions
            Console.WriteLine("\nThe Average Waiting Time Of Cars: ");
            int total_wait_time = 0;
            foreach(var car in CarsPassedNorth)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            int avrge_north = total_wait_time / CarsPassedNorth.Count;

            total_wait_time = 0;
            foreach (var car in CarsPassedSouth)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            int avrge_south = total_wait_time / CarsPassedSouth.Count;

            total_wait_time = 0;
            foreach (var car in CarsPassedEast)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            int avrge_east = total_wait_time / CarsPassedEast.Count;

            total_wait_time = 0;
            foreach (var car in CarsPassedWest)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            int avrge_west = total_wait_time / CarsPassedWest.Count;
            Console.WriteLine("North: " + avrge_north + " | South: " + avrge_south + " | East: " +
                " | West: " + avrge_east);

            Console.Read();
        }

        //helper method to print all the cars coming and passing from each direction
        public void VerifyCarsPrint()
        {
            Console.WriteLine("CARS IN NORTH: ");
            for (int i = 0; i < CarsPassedNorth.Count; i++)
            {
                Console.WriteLine(CarsPassedNorth[i].GetSeqNum() + "-" + CarsPassedNorth[i].GetArrTime() +
                    "-" + CarsPassedNorth[i].GetExitTime());
            }
            Console.WriteLine("CARS IN SOUTH: ");
            for (int i = 0; i < CarsPassedSouth.Count; i++)
            {
                Console.WriteLine(CarsPassedSouth[i].GetSeqNum() + "-" + CarsPassedSouth[i].GetArrTime() + 
                    "-" + CarsPassedSouth[i].GetExitTime());
            }
            Console.WriteLine("CARS IN EAST: ");
            for (int i = 0; i < CarsPassedEast.Count; i++)
            {
                Console.WriteLine(CarsPassedEast[i].GetSeqNum() + "-" + CarsPassedEast[i].GetArrTime() +
                    "-" + CarsPassedEast[i].GetExitTime());
            }
            Console.WriteLine("CARS IN WEST: ");
            for (int i = 0; i < CarsPassedWest.Count; i++)
            {
                Console.WriteLine(CarsPassedWest[i].GetSeqNum() + "-" + CarsPassedWest[i].GetArrTime() +
                    "-" + CarsPassedWest[i].GetExitTime());
            }
        }
        public void VerifyCarsPrint_2()
        {
            Console.WriteLine("CARS IN NORTH: ");
            for (int i = 0; i < CarsComingNorth.Count; i++)
            {
                Console.WriteLine(CarsComingNorth[i].GetSeqNum() + "-" + CarsComingNorth[i].GetArrTime());
            }
            Console.WriteLine("CARS IN SOUTH: ");
            for (int i = 0; i < CarsComingSouth.Count; i++)
            {
                Console.WriteLine(CarsComingSouth[i].GetSeqNum() + "-" + CarsComingSouth[i].GetArrTime());
            }
            Console.WriteLine("CARS IN EAST: ");
            for (int i = 0; i < CarsComingEast.Count; i++)
            {
                Console.WriteLine(CarsComingEast[i].GetSeqNum() + "-" + CarsComingEast[i].GetArrTime());
            }
            Console.WriteLine("CARS IN WEST: ");
            for (int i = 0; i < CarsComingWest.Count; i++)
            {
                Console.WriteLine(CarsComingWest[i].GetSeqNum() + "-" + CarsComingWest[i].GetArrTime());
            }
        }
    }
}
