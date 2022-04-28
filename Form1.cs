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
        List<Car> NorthDirection = new List<Car>();
        List<Car> SouthDirection = new List<Car>();
        List<Car> EastDirection = new List<Car>();
        List<Car> WestDirection = new List<Car>();

        //these global variables will store the number of cars coming from each direction
        static int numCarsNorth = 0;
        static int numCarsSouth = 0;
        static int numCarsEast = 0;
        static int numCarsWest = 0;

        //we will also create global queues for each direction, these will help us find out the maximum number of cars
        //in a line at one time
        Queue<Car> QueueNorth = new Queue<Car>();
        Queue<Car> QueueSouth = new Queue<Car>();
        Queue<Car> QueueEast = new Queue<Car>();
        Queue<Car> QueueWest = new Queue<Car>();

        //these variables will hold the maximum number of cars waiting in line in each direction
        static int maxCarsNorth = 0;
        static int maxCarsSouth = 0;
        static int maxCarsEast = 0;
        static int maxCarsWest = 0;

        //these lists will contain the list of all cars that passed from each direction
        List<Car> CarsPassedNorth = new List<Car>();
        List<Car> CarsPassedSouth = new List<Car>();
        List<Car> CarsPassedEast = new List<Car>();
        List<Car> CarsPassedWest = new List<Car>();

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
                time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //To deal with the cars, our basic idea is: we keep running the north method and keep checking the global
            //timer against the first car in each direction list, and see if the global timer's seconds are equal to 
            //the arrival seconds of each car in each direction; we know that the direction list is already sorted, so
            //we check the first car in the list as it is the earliest

            //we can let go of all the cars in the queue in North
            while (QueueNorth.Count != 0)
            {
                Car temp = QueueNorth.Dequeue();
                //set the exit time of the car
                temp.SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds));
                // because the car passes through the intersection, we print its data
                Console.WriteLine("\nThe car has passed from North: " + "\nSequence Number: " + temp.GetSeqNum() + " | Arrival Time: " +
                     temp.GetArrTime() + " | Departure Time: " + temp.GetExitTime() + " | Wait Time: " + (temp.GetExitTime() - temp.GetArrTime()));
                //we now remove the first car from the list of cars in North direction
                NorthDirection.RemoveAt(0);
                //Add the car to the list of passed cars
                CarsPassedNorth.Add(temp);
            }

            //green for 9 seconds, yellow for 3 seconds, the rest is red, we technically need 
            //the timing for the green and yellow colors, and the red color is just a default color
            //after a total of 12 seconds
            //but after 6 seconds this function calls the function for the south stoplight cycle
            //we will change the north light color inside that south stoplight cycle function

            //these variables will help with adding the cars to the queues - we need them because 
            //the execution time of program is faster than seconds elapsed
            Car removedCarNorth = new Car();
            Car removedCarSouth = new Car();
            Car removedCarEast = new Car();
            Car removedCarWest = new Car();

            bool addedNorth = false;
            bool addedSouth = false;
            bool addedEast = false;
            bool addedWest = false;

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
                
                //check the global timer against the first car in north direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == NorthDirection[0].GetArrTime())
                {
                    if (QueueNorth.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueNorth.Dequeue();
                        if (grabbed.GetArrTime() != removedCarNorth.GetArrTime())
                        {
                            QueueNorth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueNorth.Enqueue(NorthDirection[0]);
                            //update the maximum number of cars in north direction if possible
                            maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                            //we can add this car to the list of passed cars from North, with the updated exit time
                            NorthDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds + 1));
                            CarsPassedNorth.Add(NorthDirection[0]);
                            //because currently the north stoplight is green, this car passes after 1 second
                            QueueNorth.Dequeue();
                            //because the car passes through the intersection, we print its data
                            Console.WriteLine("\nThe car has passed from North: " + "\nSequence Number: " + CarsPassedNorth[CarsPassedNorth.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                     CarsPassedNorth[CarsPassedNorth.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedNorth[CarsPassedNorth.Count - 1].GetExitTime() +
                                     " | Wait Time: " + (CarsPassedNorth[CarsPassedNorth.Count - 1].GetExitTime() - CarsPassedNorth[CarsPassedNorth.Count - 1].GetArrTime()));

                            removedCarNorth = NorthDirection[0];
                            //we now remove the first car from the list of cars in North direction
                            NorthDirection.RemoveAt(0);
                        }
                        QueueNorth.Enqueue(grabbed);
                    }
                    else if (!addedNorth)
                    {
                        addedNorth = true;
                        //we add the car to the queue
                        QueueNorth.Enqueue(NorthDirection[0]);
                        //update the maximum number of cars in north direction if possible
                        maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                        removedCarNorth = NorthDirection[0];
                        NorthDirection.RemoveAt(0);
                    }
                }
                //check the cars in south direction

                if (Convert.ToInt32(time_span_overall.TotalSeconds) == SouthDirection[0].GetArrTime())
                {
                    if (QueueSouth.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueSouth.Dequeue();
                        if (grabbed.GetArrTime() != removedCarSouth.GetArrTime())
                        {
                            QueueSouth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueSouth.Enqueue(SouthDirection[0]);
                            //update the maximum number of cars in south direction if possible
                            maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                            //because currently the south light is red, we do not let the car go
                            removedCarSouth = SouthDirection[0];
                            SouthDirection.RemoveAt(0);
                        }
                        QueueSouth.Enqueue(grabbed);
                    }
                    else if (!addedSouth)
                    {
                        addedSouth = true;
                        //we add the car to the queue
                        QueueSouth.Enqueue(SouthDirection[0]);
                        //update the maximum number of cars in south direction if possible
                        maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                        //because currently the south light is red, we do not let the car go
                        removedCarSouth = SouthDirection[0];
                        SouthDirection.RemoveAt(0);
                    }
                }
               
                //check the cars in east direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == EastDirection[0].GetArrTime())
                {
                    if (QueueEast.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueEast.Dequeue();
                        if (grabbed.GetArrTime() != removedCarEast.GetArrTime())
                        {
                            QueueEast.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueEast.Enqueue(EastDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                            //because currently the east light is red, we do not let the car go
                            removedCarEast = EastDirection[0];
                            EastDirection.RemoveAt(0);
                        }
                        QueueEast.Enqueue(grabbed);
                    }
                    else if (!addedEast)
                    {
                        addedEast = true;
                        //we add the car to the queue
                        QueueEast.Enqueue(EastDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                        //because currently the east light is red, we do not let the car go
                        removedCarEast = EastDirection[0];
                        EastDirection.RemoveAt(0);
                    }
                }
                //check the cars in west direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == WestDirection[0].GetArrTime())
                {
                    if (QueueWest.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueWest.Dequeue();
                        if (grabbed.GetArrTime() != removedCarWest.GetArrTime())
                        {
                            QueueWest.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueWest.Enqueue(WestDirection[0]);
                            //update the maximum number of cars in west direction if possible
                            maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                            //because currently the east light is red, we do not let the car go
                            removedCarWest = WestDirection[0];
                            WestDirection.RemoveAt(0);
                        }
                        QueueWest.Enqueue(grabbed);
                    }
                    else if (!addedWest)
                    {
                        addedWest = true;
                        //we add the car to the queue
                        QueueWest.Enqueue(WestDirection[0]);
                        //update the maximum number of cars in west direction if possible
                        maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                        //because currently the east light is red, we do not let the car go
                        removedCarWest = WestDirection[0];
                        WestDirection.RemoveAt(0);
                    }
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = time_span_overall.Seconds.ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_north = north_watch.Elapsed;
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
                time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //we can let go of all the cars in the queue in South
            while (QueueSouth.Count != 0)
            {
                Car temp = QueueSouth.Dequeue();
                //set exit time of the car
                temp.SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds) + 1);
                // because the car passes through the intersection, we print its data
                Console.WriteLine("\nThe car has passed from South: " + "\nSequence Number: " + temp.GetSeqNum() + " | Arrival Time: " +
                     temp.GetArrTime() + " | Departure Time: " + temp.GetExitTime() + " | Wait Time: " + (temp.GetExitTime() - temp.GetArrTime()));
                //we now remove the first car from the list of cars in North direction
                SouthDirection.RemoveAt(0);
                //Add the car to the list of passed cars
                CarsPassedSouth.Add(temp);
            }

            //because the stopwatch is slower than the execution time of the program, we need to make sure we don't repeatedly print 
            //the same output inisde the while loop, so we will create helper boolean variables to help us with printing the changes once
            bool north_light_changed_yellow = false;
            bool north_light_changed_red = false;
            bool south_light_changed = false;

            Car addedNorth = new Car();
            Car addedSouth = new Car();
            Car addedEast = new Car();
            Car addedWest = new Car();

            bool addedNorthCar = false;
            bool addedSouthCar = false;
            bool addedEastCar = false;
            bool addedWestCar = false;

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

                //check the global timer against the first car in north direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == NorthDirection[0].GetArrTime())
                {
                    if (QueueNorth.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueNorth.Dequeue();
                        if (grabbed.GetArrTime() != addedNorth.GetArrTime())
                        {
                            QueueNorth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueNorth.Enqueue(NorthDirection[0]);
                            //update the maximum number of cars in north direction if possible
                            maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                            //if the north stoplight is still green, we can let this car pass
                            addedNorth = NorthDirection[0];
                            NorthDirection.RemoveAt(0);
                            if (!north_light_changed_red && !north_light_changed_yellow)
                            {
                                //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                                Car grabbed_2 = QueueNorth.Dequeue();
                                if (grabbed_2.GetArrTime() != addedNorth.GetArrTime())
                                {
                                    QueueNorth.Enqueue(grabbed_2);
                                    //we can add this car to the list of passed cars from North, with the updated exit time
                                    NorthDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds) + 1);
                                    CarsPassedNorth.Add(NorthDirection[0]);
                                    //because currently the north stoplight is green, this car passes after 1 second
                                    QueueNorth.Dequeue();
                                    //because the car passes through the intersection, we print its data
                                    Console.WriteLine("\nThe car has passed from North: " + "\nSequence Number: " + CarsPassedNorth[CarsPassedNorth.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                         CarsPassedNorth[CarsPassedNorth.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedNorth[CarsPassedNorth.Count - 1].GetExitTime() +
                                         " | Wait Time: " + (CarsPassedNorth[CarsPassedNorth.Count - 1].GetExitTime() - CarsPassedNorth[CarsPassedNorth.Count - 1].GetArrTime()));
                                    addedNorth = NorthDirection[0];
                                    //we now remove the first car from the list of cars in North direction
                                    NorthDirection.RemoveAt(0);
                                }
                                QueueNorth.Enqueue(grabbed_2);
                            }
                        }
                        QueueNorth.Enqueue(grabbed);
                    }
                    else if (!addedNorthCar)
                    {
                        addedNorthCar = true;
                        //we add the car to the queue
                        QueueNorth.Enqueue(NorthDirection[0]);
                        //update the maximum number of cars in north direction if possible
                        maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                        addedNorth = NorthDirection[0];
                        NorthDirection.RemoveAt(0);
                    }
                }
                //check the cars in south direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == SouthDirection[0].GetArrTime())
                {
                    if (QueueSouth.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueSouth.Dequeue();
                        if (grabbed.GetArrTime() != addedSouth.GetArrTime())
                        {
                            QueueSouth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueSouth.Enqueue(SouthDirection[0]);
                            //update the maximum number of cars in south direction if possible
                            maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                            addedSouth = SouthDirection[0];
                            SouthDirection.RemoveAt(0);
                            //we check if the south stoplight is still green, if so we let the car pass through
                            if (!south_light_changed)
                            {
                                Car grabbed_2 = QueueSouth.Dequeue();
                                if (grabbed_2.GetArrTime() != addedSouth.GetArrTime())
                                {
                                    QueueSouth.Enqueue(grabbed_2);
                                    //we can add this car to the list of passed cars from South, with the updated exit time
                                    SouthDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.Seconds) + 1);
                                    CarsPassedSouth.Add(SouthDirection[0]);
                                    //because currently the south stoplight is green, this car passes after 1 second
                                    QueueSouth.Dequeue();
                                    //because the car passes through the intersection, we print its data
                                    Console.WriteLine("\nThe car has passed from South: " + "\nSequence Number: " + CarsPassedSouth[CarsPassedSouth.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                         CarsPassedSouth[CarsPassedSouth.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedSouth[CarsPassedSouth.Count - 1].GetExitTime() +
                                         " | Wait Time: " + (CarsPassedSouth[CarsPassedSouth.Count - 1].GetExitTime() - CarsPassedSouth[CarsPassedSouth.Count - 1].GetArrTime()));
                                    //we now remove the first car from the list of cars in South direction
                                    addedSouth = SouthDirection[0];
                                    SouthDirection.RemoveAt(0);
                                }
                                QueueSouth.Enqueue(grabbed_2);
                            }
                        }
                        QueueSouth.Enqueue(grabbed);
                    }
                    else if (!addedSouthCar)
                    {
                        addedSouthCar = true;
                        QueueSouth.Enqueue(SouthDirection[0]);
                        //update the maximum number of cars in south direction if possible
                        maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                        addedSouth = SouthDirection[0];
                        SouthDirection.RemoveAt(0);
                    }
                    
                }
                //check the cars in east direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == EastDirection[0].GetArrTime())
                {
                    if (QueueEast.Count != 0)
                    {
                        Car grabbed = QueueEast.Dequeue();
                        if (grabbed.GetArrTime() != addedEast.GetArrTime())
                        {
                            QueueEast.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueEast.Enqueue(EastDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                            //because currently the east light is red, we do not let the car go
                            addedEast = EastDirection[0];
                            EastDirection.RemoveAt(0);
                        }
                        QueueEast.Enqueue(grabbed);
                    }
                    else if (!addedEastCar)
                    {
                        addedEastCar = true;
                        //we add the car to the queue
                        QueueEast.Enqueue(EastDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                        //because currently the east light is red, we do not let the car go
                        addedEast = EastDirection[0];
                        EastDirection.RemoveAt(0);
                    }
                }
                //check the cars in west direction
                if (Convert.ToInt32(time_span_overall.Seconds) == WestDirection[0].GetArrTime())
                { 
                    if (QueueWest.Count != 0)
                    {
                        Car grabbed = QueueWest.Dequeue();
                        if (grabbed.GetArrTime() != addedWest.GetArrTime())
                        {
                            QueueWest.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueWest.Enqueue(WestDirection[0]);
                            //update the maximum number of cars in west direction if possible
                            maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                            //because currently the east light is red, we do not let the car go
                            addedWest = WestDirection[0];
                            WestDirection.RemoveAt(0);
                        }
                        QueueWest.Enqueue(grabbed);
                    }
                    else if (!addedWestCar)
                    {
                        addedWestCar = true;
                        //we add the car to the queue
                        QueueWest.Enqueue(WestDirection[0]);
                        //update the maximum number of cars in west direction if possible
                        maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                        //because currently the east light is red, we do not let the car go
                        addedWest = WestDirection[0];
                        WestDirection.RemoveAt(0);
                    }
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = time_span_overall.Seconds.ToString();
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(),
                            east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        south_light_changed = true;
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
                time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(), 
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //we can let go of all the cars in the queue in East
            while (QueueEast.Count != 0)
            {
                Car temp = QueueEast.Dequeue();
                //set the exit time of the car
                temp.SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds + 1));
                // because the car passes through the intersection, we print its data
                Console.WriteLine("\nThe car has passed from East: " + "\nSequence Number: " + temp.GetSeqNum() + " | Arrival Time: " +
                     temp.GetArrTime() + " | Departure Time: " + temp.GetExitTime() + " | Wait Time: " + (temp.GetExitTime() - temp.GetArrTime()));
                //we now remove the first car from the list of cars in North direction
                EastDirection.RemoveAt(0);
                //add the car to the list of passed cars
                CarsPassedEast.Add(temp);
            }

            Car addedEast = new Car();
            Car addedNorth = new Car();
            Car addedSouth = new Car();
            Car addedWest = new Car();

            bool addedCarEast = false;
            bool addedCarWest = false;
            bool addedCarNorth = false;
            bool addedCarSouth = false;

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

                //check the global timer against the first car in east direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == EastDirection[0].GetArrTime())
                {
                    if (QueueEast.Count != 0)
                    {
                        //we grab the last car from the queue and check if the arrival time of this car is the same as that of the car from NorthDirection
                        Car grabbed = QueueEast.Dequeue();
                        if (grabbed.GetArrTime() != addedEast.GetArrTime())
                        {
                            QueueEast.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueEast.Enqueue(EastDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                            //we can add this car to the list of passed cars from East, with the updated exit time
                            EastDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.Seconds) + 1);
                            CarsPassedEast.Add(EastDirection[0]);
                            //because currently the east stoplight is green, this car passes after 1 second
                            QueueEast.Dequeue();
                            //because the car passes through the intersection, we print its data
                            Console.WriteLine("\nThe car has passed from East: " + "\nSequence Number: " + CarsPassedEast[CarsPassedEast.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                     CarsPassedEast[CarsPassedEast.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedEast[CarsPassedEast.Count - 1].GetExitTime() +
                                     " | Wait Time: " + (CarsPassedEast[CarsPassedEast.Count - 1].GetExitTime() - CarsPassedEast[CarsPassedEast.Count - 1].GetArrTime()));
                            //we now remove the first car from the list of cars in East direction
                            addedEast = EastDirection[0];
                            EastDirection.RemoveAt(0);
                        }
                        QueueEast.Enqueue(grabbed);
                    }
                    else if (!addedCarEast)
                    {
                        addedCarEast = true;
                        //we add the car to the queue
                        QueueEast.Enqueue(EastDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                        //we can add this car to the list of passed cars from East, with the updated exit time
                        addedEast = EastDirection[0];
                        EastDirection.RemoveAt(0);
                    }
                }
                //check the cars in south direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == SouthDirection[0].GetArrTime())
                {
                    if (QueueSouth.Count != 0)
                    {
                        Car grabbed = QueueSouth.Dequeue();
                        if (grabbed.GetArrTime() != addedSouth.GetArrTime())
                        {
                            QueueSouth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueSouth.Enqueue(SouthDirection[0]);
                            //update the maximum number of cars in south direction if possible
                            maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                            //because currently the south light is red, we do not let the car go
                            addedSouth = SouthDirection[0];
                            SouthDirection.RemoveAt(0);
                        }
                        QueueSouth.Enqueue(grabbed);
                    } else if (!addedCarSouth)
                    {
                        addedCarSouth = true;
                        //we add the car to the queue
                        QueueSouth.Enqueue(SouthDirection[0]);
                        //update the maximum number of cars in south direction if possible
                        maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                        //because currently the south light is red, we do not let the car go
                        addedSouth = SouthDirection[0];
                        SouthDirection.RemoveAt(0);
                    }
                }
                //check the cars in north direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == NorthDirection[0].GetArrTime())
                {
                    if (QueueNorth.Count != 0)
                    {
                        Car grabbed = QueueNorth.Dequeue();
                        if (grabbed.GetArrTime() != addedNorth.GetArrTime())
                        {
                            QueueNorth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueNorth.Enqueue(NorthDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                            //because currently the east light is red, we do not let the car go
                            addedNorth = NorthDirection[0];
                            NorthDirection.RemoveAt(0);
                        }
                        QueueNorth.Enqueue(grabbed);
                    }
                    else if (!addedCarNorth)
                    {
                        addedCarNorth = true;
                        //we add the car to the queue
                        QueueNorth.Enqueue(NorthDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                        //because currently the east light is red, we do not let the car go
                        addedNorth = NorthDirection[0];
                        NorthDirection.RemoveAt(0);
                    }

                }
                //check the cars in west direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == WestDirection[0].GetArrTime())
                {

                    if (QueueWest.Count != 0)
                    {
                        Car grabbed = QueueWest.Dequeue();
                        if (grabbed.GetArrTime() != addedWest.GetArrTime())
                        {
                            QueueWest.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueWest.Enqueue(WestDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                            //because currently the east light is red, we do not let the car go
                            addedWest = WestDirection[0];
                            WestDirection.RemoveAt(0);
                        }
                        QueueWest.Enqueue(grabbed);
                    }
                    else if (!addedCarWest)
                    {
                        addedCarWest = true;
                        //we add the car to the queue
                        QueueWest.Enqueue(WestDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                        //because currently the east light is red, we do not let the car go
                        addedWest = WestDirection[0];
                        WestDirection.RemoveAt(0);
                    }

                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = time_span_overall.Seconds.ToString();
                Refresh();

                //we get the seconds elapsed in the timer for the north stoplight
                ts_east = east_watch.Elapsed;
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
                time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(), south_stoplight.GetColour(), 
                east_stoplight.GetColour(), west_stoplight.GetColour());

            //we can let go of all the cars in the queue in West
            while (QueueWest.Count != 0)
            {
                Car temp = QueueWest.Dequeue();
                //set exit time of the car
                temp.SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds + 1));
                // because the car passes through the intersection, we print its data
                Console.WriteLine("\nThe car has passed from West: " + "\nSequence Number: " + temp.GetSeqNum() + " | Arrival Time: " +
                     temp.GetArrTime() + " | Departure Time: " + temp.GetExitTime() + " | Wait Time: " + (temp.GetExitTime() - temp.GetArrTime()));
                //we now remove the first car from the list of cars in West direction
                WestDirection.RemoveAt(0);
                //Add the car to the list of passed cars
                CarsPassedWest.Add(temp);
            }

            //because the stopwatch is slower than the execution time of the program, we need to make sure we don't repeatedly print 
            //the same output inisde the while loop, so we will create helper boolean variables to help us with printing the changes once
            bool east_light_changed_yellow = false;
            bool east_light_changed_red = false;
            bool west_light_changed = false;

            Car addedEast = new Car();
            Car addedWest = new Car();
            Car addedNorth = new Car();
            Car addedSouth = new Car();

            bool addedCarEast = false;
            bool addedCarWest = false;
            bool addedCarNorth = false;
            bool addedCarSouth = false;

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

                //check the global timer against the first car in east direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == EastDirection[0].GetArrTime())
                {
                    if (QueueEast.Count != 0)
                    {
                        Car grabbed = QueueEast.Dequeue();
                        if (grabbed.GetArrTime() != addedEast.GetArrTime())
                        {
                            QueueEast.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueEast.Enqueue(EastDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                            addedEast = EastDirection[0];
                            EastDirection.RemoveAt(0);
                            //if the east stoplight is still green, we can let this car pass
                            if (!east_light_changed_red && !east_light_changed_yellow)
                            {
                                Car grabbed_2 = QueueEast.Dequeue();
                                if (grabbed_2.GetArrTime() != addedEast.GetArrTime())
                                {
                                    QueueEast.Enqueue(grabbed_2);
                                    //we can add this car to the list of passed cars from East, with the updated exit time
                                    EastDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds) + 1);
                                    CarsPassedEast.Add(EastDirection[0]);
                                    //because currently the east stoplight is green, this car passes after 1 second
                                    QueueEast.Dequeue();
                                    //because the car passes through the intersection, we print its data
                                    Console.WriteLine("\nThe car has passed from East: " + "\nSequence Number: " + CarsPassedEast[CarsPassedEast.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                              CarsPassedEast[CarsPassedEast.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedEast[CarsPassedEast.Count - 1].GetExitTime() +
                                              " | Wait Time: " + (CarsPassedEast[CarsPassedEast.Count - 1].GetExitTime() - CarsPassedEast[CarsPassedEast.Count - 1].GetArrTime()));
                                    //we now remove the first car from the list of cars in East direction
                                    addedEast = EastDirection[0];
                                    EastDirection.RemoveAt(0);
                                }
                                QueueEast.Enqueue(grabbed_2);
                            }
                        }
                        QueueEast.Enqueue(grabbed);
                    }
                    else if (!addedCarEast)
                    {
                        addedCarEast = true;
                        //we add the car to the queue
                        QueueEast.Enqueue(EastDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsEast = maxCarsEast < QueueEast.Count ? QueueEast.Count : maxCarsEast;
                        addedEast = EastDirection[0];
                        EastDirection.RemoveAt(0);
                    }                 
                }
                //check the cars in west direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == WestDirection[0].GetArrTime())
                {
                   if (QueueWest.Count != 0)
                   {
                        Car grabbed = QueueWest.Dequeue();
                        if (grabbed.GetArrTime() != addedWest.GetArrTime())
                        {
                            QueueWest.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueWest.Enqueue(WestDirection[0]);
                            //update the maximum number of cars in west direction if possible
                            maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                            addedWest = WestDirection[0];
                            WestDirection.RemoveAt(0);
                            //we check if the west stoplight is still green, if so we let the car pass through
                            if (!west_light_changed)
                            {
                                Car grabbed_2 = QueueWest.Dequeue();
                                if (grabbed_2.GetArrTime() != addedWest.GetArrTime())
                                {
                                    QueueWest.Enqueue(grabbed);
                                    //we can add this car to the list of passed cars from West, with the updated exit time
                                    WestDirection[0].SetExitTime(Convert.ToInt32(time_span_overall.TotalSeconds) + 1);
                                    CarsPassedWest.Add(WestDirection[0]);
                                    //because currently the west stoplight is green, this car passes after 1 second
                                    QueueWest.Dequeue();
                                    //because the car passes through the intersection, we print its data
                                    Console.WriteLine("\nThe car has passed from West: " + "\nSequence Number: " + CarsPassedWest[CarsPassedWest.Count - 1].GetSeqNum() + " | Arrival Time: " +
                                         CarsPassedWest[CarsPassedWest.Count - 1].GetArrTime() + " | Departure Time: " + CarsPassedWest[CarsPassedWest.Count - 1].GetExitTime() +
                                         " | Wait Time: " + (CarsPassedWest[CarsPassedWest.Count - 1].GetExitTime() - CarsPassedWest[CarsPassedWest.Count - 1].GetArrTime()));
                                    //we now remove the first car from the list of cars in West direction
                                    addedWest = WestDirection[0];
                                    WestDirection.RemoveAt(0);
                                }
                                QueueWest.Enqueue(grabbed_2);
                            }
                        }
                        QueueWest.Enqueue(grabbed);
                   }
                   else if (!addedCarWest)
                    {
                        addedCarWest = true;
                        //we add the car to the queue
                        QueueWest.Enqueue(WestDirection[0]);
                        //update the maximum number of cars in west direction if possible
                        maxCarsWest = maxCarsWest < QueueWest.Count ? QueueWest.Count : maxCarsWest;
                        //we check if the west stoplight is still green, if so we let the car pass through
                        addedWest = WestDirection[0];
                        WestDirection.RemoveAt(0);
                    }
                }
                //check the cars in north direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == NorthDirection[0].GetArrTime())
                {
                    if (QueueNorth.Count != 0)
                    {
                        Car grabbed = QueueNorth.Dequeue();
                        if (grabbed.GetArrTime() != addedNorth.GetArrTime())
                        {
                            QueueNorth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueNorth.Enqueue(NorthDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                            //because currently the east light is red, we do not let the car go
                            addedNorth = NorthDirection[0];
                            NorthDirection.RemoveAt(0);
                        }
                        QueueNorth.Enqueue(grabbed);
                    }
                    else if (!addedCarNorth)
                    {
                        addedCarNorth = true;
                        //we add the car to the queue
                        QueueNorth.Enqueue(NorthDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsNorth = maxCarsNorth < QueueNorth.Count ? QueueNorth.Count : maxCarsNorth;
                        //because currently the east light is red, we do not let the car go
                        addedNorth = NorthDirection[0];
                        NorthDirection.RemoveAt(0);
                    }
                }
                //check the cars in south direction
                if (Convert.ToInt32(time_span_overall.TotalSeconds) == SouthDirection[0].GetArrTime())
                {
                    if (QueueSouth.Count != 0)
                    {
                        Car grabbed = QueueSouth.Dequeue();
                        if (grabbed.GetArrTime() != addedSouth.GetArrTime())
                        {
                            QueueSouth.Enqueue(grabbed);
                            //we add the car to the queue
                            QueueSouth.Enqueue(SouthDirection[0]);
                            //update the maximum number of cars in east direction if possible
                            maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                            //because currently the east light is red, we do not let the car go
                            addedSouth = SouthDirection[0];
                            SouthDirection.RemoveAt(0);
                        }
                        QueueSouth.Enqueue(grabbed);
                    }
                    else if (!addedCarSouth)
                    {
                        addedCarSouth = true;
                        //we add the car to the queue
                        QueueSouth.Enqueue(SouthDirection[0]);
                        //update the maximum number of cars in east direction if possible
                        maxCarsSouth = maxCarsSouth < QueueSouth.Count ? QueueSouth.Count : maxCarsSouth;
                        //because currently the east light is red, we do not let the car go
                        addedSouth = SouthDirection[0];
                        SouthDirection.RemoveAt(0);
                    }
                }

                //we keep dynamically changing the text value of the timer label
                Timer_LBL.Text = time_span_overall.Seconds.ToString();
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(),
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(),
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
                            time_span_overall.Seconds.ToString(), " ", north_stoplight.GetColour(),
                            south_stoplight.GetColour(), east_stoplight.GetColour(), west_stoplight.GetColour());
                        //set the boolean
                        west_light_changed = true;
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
            numCarsNorth = NorthDirection.Count;
            numCarsSouth = SouthDirection.Count;
            numCarsEast = EastDirection.Count;
            numCarsWest = WestDirection.Count;

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
                PrintResults();
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
                        Car car_N = new Car(NorthDirection.Count+1, int.Parse(line.Substring(1)));
                        NorthDirection.Add(car_N);
                        //MessageBox.Show("NORTH CAR: " + car_N.GetSeqNum() + car_N.GetArrTime());
                        break;
                    case 'S':
                        Car car_S = new Car(SouthDirection.Count + 1, int.Parse(line.Substring(1)));
                        SouthDirection.Add(car_S);
                        //MessageBox.Show("SOUTH CAR: " + car_S.GetSeqNum() + car_S.GetArrTime());
                        break;
                    case 'E':
                        Car car_E = new Car(EastDirection.Count + 1, int.Parse(line.Substring(1)));
                        EastDirection.Add(car_E);
                        //MessageBox.Show("EAST CAR: " + car_E.GetSeqNum() + car_E.GetArrTime());
                        break;
                    case 'W':
                        Car car_W = new Car(WestDirection.Count + 1, int.Parse(line.Substring(1)));
                        WestDirection.Add(car_W);
                        //MessageBox.Show("WEST CAR: " + car_W.GetSeqNum() + car_W.GetArrTime());
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

            Console.WriteLine("The Maximum Size of Line of Cars that had to Wait to pass through: ");
            Console.WriteLine("North: " + maxCarsNorth + " | South: " + maxCarsSouth + " | East: " + maxCarsEast +
                " | West: " + maxCarsWest);

            Console.WriteLine("The Average Waiting Time Of Cars: ");

            int total_wait_time = 0;
            foreach(var car in CarsPassedNorth)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            foreach (var car in CarsPassedSouth)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            foreach (var car in CarsPassedEast)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }
            foreach (var car in CarsPassedWest)
            {
                total_wait_time += (car.GetExitTime() - car.GetArrTime());
            }

            int average_time = total_wait_time / (CarsPassedNorth.Count + CarsPassedSouth.Count + CarsPassedEast.Count + CarsPassedWest.Count);
            Console.Write(average_time);
            Console.WriteLine();
            Console.Read();
        }
    }
}
