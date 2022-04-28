// COP 4365 – Spring 2022
//
// Homework #4: The Traffic Study
//
// Description: In this problem we are required to create a program with having emergency vehicle signals and have two signals be the same color at the same time. We used a timer in this program to change the signal lights
//              Then the program analyzes all the cars that come from the 4 directions and prints data on the average wait time, maximum number of cars and so on.
//
// File name: Homework2
//
// By: Amrutha Kota

//UPDATED!!!!


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

namespace Homework2
{
    public partial class TrafficLight : Form
    {
        public TrafficLight()
        {
            InitializeComponent();
        }
        //stop watch variable
        Stopwatch timer = new Stopwatch();

        TimeSpan ts;

        //Creating new objects
        Lighttraffic north_stoplight = new Lighttraffic();
        Lighttraffic south_stoplight = new Lighttraffic();
        Lighttraffic east_stoplight = new Lighttraffic();
        Lighttraffic west_stoplight = new Lighttraffic();


        // Creating a list for each direction

        List<Cars> NorthDir = new List<Cars>();
        List<Cars> SouthDir = new List<Cars>();
        List<Cars> EastDir = new List<Cars>();
        List<Cars> WestDir = new List<Cars>();

        // Variables
        static int count_North_Cars = 0;
        static int count_South_Cars = 0;
        static int count_East_Cars = 0;
        static int count_West_Cars = 0;

        //we will also create global queues for each direction, these will help us find out the maximum number of cars
        //in a line at one time
        List<Cars> WaitingNorth = new List<Cars>();
        List<Cars> WaitingSouth = new List<Cars>();
        List<Cars> WaitingEast = new List<Cars>();
        List<Cars> WaitingWest = new List<Cars>();

        //these variables will hold the maximum number of cars waiting in line in each direction
        static int max_North_Cars = 0;
        static int max_South_Cars = 0;
        static int max_East_Cars = 0;
        static int max_West_Cars = 0;

        //these lists will contain the list of all cars that passed from each direction
        List<Cars> cars_Left_North = new List<Cars>();
        List<Cars> cars_Left_South = new List<Cars>();
        List<Cars> cars_Left_East = new List<Cars>();
        List<Cars> cars_Left_West = new List<Cars>();


        //Method to know what will happen if the button is clicked on
        private void Run_button_Click(object sender, EventArgs e)
        {
            timer.Start();
            ts = timer.Elapsed;
            int seconds = ts.Seconds;
            Count.Text = seconds.ToString();

            north_stoplight.change_light("Green");
            south_stoplight.change_light("Red");
            east_stoplight.change_light("Red");
            west_stoplight.change_light("Red");

            //Printing the console output
            Console.WriteLine("\n{0} {1, 12} {2, 12} {3, 15} {4, 15}", "Current Time", "North Light", "South Light", "East Light", "West Light");
            Console.WriteLine("{0} {1, 12} {2, 12} {3, 15} {4, 15}", "____________", "___________", "____________", "___________", "__________");

            string [] text = File.ReadAllLines("HW #4 Data.txt");
            //MessageBox.Show(text);

            SeperateDirections(text);


            while (timer.Elapsed.Minutes < 4)
            {
                ts = timer.Elapsed;
                northlight();
                eastlight();
            }
        }

        //Method for the north light
        //Name: northlight();
        public void northlight()
        {

            Stopwatch timer1 = new Stopwatch();
            TimeSpan ts1 = timer1.Elapsed;
            timer1.Start();

            //Green light on and others are off
            Stop_North.BackColor = Color.Gray;
            yellow_North.BackColor = Color.Gray;
            Go_North.BackColor = Color.Lime;

            Refresh();

            north_stoplight.change_light("Green");
            //Printing output on console
            Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.TotalSeconds.ToString(), " ",
                north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());


            max_North_Cars = max_North_Cars < WaitingNorth.Count ? WaitingNorth.Count : max_North_Cars;
            while (WaitingNorth.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                WaitingNorth[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                Console.WriteLine("A car passed from North! \nSequence Number: " + WaitingNorth[0].GetNumber() + " Arrival Time: " +
                    WaitingNorth[0].GetArrival() + " Exit Time: " + WaitingNorth[0].GetDepart() + " Wait Time: " +
                    (WaitingNorth[0].GetDepart() - WaitingNorth[0].GetArrival()));
                //add this car to the list of already pased cars from North
                cars_Left_North.Add(WaitingNorth[0]);
                //remove this car from the list of cars waiting in North 
                WaitingNorth.RemoveAt(0);
            }

            while (ts1.Seconds < 6)
            {
                Count.Text = timer.Elapsed.Seconds.ToString();
                ts1 = timer1.Elapsed;
                Refresh();

                //Restart timer after 1 minute(Restart the sequence)
                if (timer.Elapsed.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

                //check North cars
                if (NorthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == NorthDir[0].GetArrival())
                    {
                        //check that this car has not already been passed - check sequence number because it must be unique
                        //this check is needed becuase the execution time of program is faster than the stopwatch timer
                        if (cars_Left_North.Count != 0)
                        {
                            if (cars_Left_North[0].GetNumber() != cars_Left_North[0].GetNumber())
                            {
                                //set the exit time of the car
                                NorthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from North! \nSequence Number: " + NorthDir[0].GetNumber() + " Arrival Time: " +
                                    NorthDir[0].GetArrival() + " Exit Time: " + NorthDir[0].GetDepart() + " Wait Time: " +
                                        (NorthDir[0].GetDepart() - NorthDir[0].GetArrival()));
                                //add this car to the list of already pased cars from North
                                cars_Left_North.Add(NorthDir[0]);
                                //remove this car from the list of cars coming from North
                                NorthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //just add this car to the list of cars passed
                            //set its exit time
                            NorthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                            //print the info that this car has passed the intersection
                            Console.WriteLine("A car passed from North! \nSequence Number: " + NorthDir[0].GetNumber() + " Arrival Time: " +
                                 NorthDir[0].GetArrival() + " Exit Time: " + NorthDir[0].GetDepart() + " Wait Time: " +
                                    (NorthDir[0].GetDepart() - NorthDir[0].GetArrival()));
                            //add this car to the list of already pased cars from North
                            cars_Left_North.Add(NorthDir[0]);
                            //remove this car from the list of cars coming from North
                            NorthDir.RemoveAt(0);
                        }
                    }
                }
                //check South cars
                if (SouthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == SouthDir[0].GetArrival())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (WaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingSouth[WaitingSouth.Count - 1].GetNumber() != SouthDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingSouth.Add(SouthDir[0]);
                                //remove this car from the list of cars coming from South
                                SouthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingSouth.Add(SouthDir[0]);
                            //remove this car from the list of cars coming from South
                            SouthDir.RemoveAt(0);
                        }
                    }
                }
                //check East cars
                if (EastDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == EastDir[0].GetArrival())
                    {
                        //becuase currently the East stoplight is red, we put the cars waiting in line
                        if (WaitingEast.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingEast[WaitingEast.Count - 1].GetNumber() != EastDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingEast.Add(EastDir[0]);
                                //remove this car from the list of cars coming from East
                                EastDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            WaitingEast.Add(EastDir[0]);
                            EastDir.RemoveAt(0);
                        }
                    }
                }
                //check West cars
                if (WestDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == WestDir[0].GetArrival())
                    {
                        if (WaitingWest.Count != 0)
                        {
                            if (WaitingWest[WaitingWest.Count - 1].GetNumber() != WestDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingWest.Add(WestDir[0]);
                                WestDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingWest.Add(WestDir[0]);
                            WestDir.RemoveAt(0);
                        }
                    }
                }
            }

                southlight();
        }

        //Method name: southLight();
        //Method shows the operation of south light
        public void southlight()
        {

            Stopwatch timer1 = new Stopwatch();
            timer1.Start();
            TimeSpan ts1 = timer1.Elapsed;

            //turning on the south light
            south_stoplight.change_light("Green");
            Go_South.BackColor = Color.Lime;
            Stop_South.BackColor = Color.Gray;
            yellow_South.BackColor = Color.Gray;
            Refresh();



            south_stoplight.change_light("Green");
            //Printing output on console
            Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                 north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());

            max_South_Cars = max_South_Cars < WaitingSouth.Count ? WaitingSouth.Count : max_South_Cars;
            while (WaitingSouth.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                WaitingSouth[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                Console.WriteLine("A car passed from South! \nSequence Number: " + WaitingSouth[0].GetNumber() + " Arrival Time: " +
                    WaitingSouth[0].GetArrival() + " Exit Time: " + WaitingSouth[0].GetDepart() + " Wait Time: " +
                    (WaitingSouth[0].GetDepart() - WaitingSouth[0].GetArrival()));
                //add this car to the list of already pased cars from South
                cars_Left_South.Add(WaitingNorth[0]);
                //remove this car from the list of cars waiting in South
                WaitingSouth.RemoveAt(0);
            }

            bool north_yellow_printed = false;
            bool north_red_printed = false;
            bool south_change = false;

            while (ts1.Seconds < 12)
            {

                if (timer.Elapsed.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

               
                //Make the north light yellow when ts1 is//Make the east light yellow after 3 seconds i.e. 9 seconds after the green turns on 3 seconds i.e. 9 seconds after the green turns on
                if (ts1.Seconds == 3)
                {
                    //Printing north yellow on the console
                    if (!north_yellow_printed)
                    {
                        north_stoplight.change_light("Yellow");
                        yellow_North.BackColor = Color.Yellow;
                        Go_North.BackColor = Color.Gray;
                        Stop_North.BackColor = Color.Gray;
                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                            north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());
                        Refresh();

                        north_yellow_printed = true;
                    }
                }
                //Make the north light red when 6 seconds i.e. 12 seconds after the green turns on
                else if (ts1.Seconds == 6)
                {
                    //Printing the north signal red on the console
                    if (!north_red_printed)
                    {
                        north_stoplight.change_light("Red");
                        Stop_North.BackColor = Color.Red;
                        yellow_North.BackColor = Color.Gray;
                        Go_North.BackColor = Color.Gray;
                        Refresh();
                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                    north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());

                        north_red_printed = true;
                    }
                }
                else if (ts1.Seconds == 9)
                {
                    if (!south_change)
                    {
                        south_change = true;
                        yellow_South.BackColor = Color.Yellow;
                        Go_South.BackColor = Color.Gray;
                        Stop_South.BackColor = Color.Gray;
                        south_stoplight.change_light("Yellow");
                        Refresh();

                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                        north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());
                    }
                }
                //let's take care of cars coming from all directions
                //check cars from North
                if (NorthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == NorthDir[0].GetArrival())
                    {
                        //if the North stoplight is still green we try to pass the car
                        if (north_yellow_printed == false && north_red_printed == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (cars_Left_North.Count != 0)
                            {
                                if (cars_Left_North[0].GetNumber() != NorthDir[0].GetNumber())
                                {
                                    //set the exit time of the car
                                    NorthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car passed from North! \nSequence Number: " + NorthDir[0].GetNumber() + " Arrival Time: " +
                                    NorthDir[0].GetArrival() + " Exit Time: " + NorthDir[0].GetDepart() + " Wait Time: " +
                                            (NorthDir[0].GetDepart() - NorthDir[0].GetArrival()));
                                    //add this car to the list of already pased cars from North
                                    cars_Left_North.Add(NorthDir[0]);
                                    //remove this car from the list of cars coming from North
                                    NorthDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                NorthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from North! \nSequence Number: " + NorthDir[0].GetNumber() + " Arrival Time: " +
                                    NorthDir[0].GetArrival() + " Exit Time: " + NorthDir[0].GetDepart() + " Wait Time: " +
                                            (NorthDir[0].GetDepart() - NorthDir[0].GetArrival()));
                                //add this car to the list of already pased cars from North
                                cars_Left_North.Add(NorthDir[0]);
                                //remove this car from the list of cars coming from North
                                NorthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the North stoplight is no longer green, we add the car to the waiting list
                            if (WaitingNorth.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (WaitingNorth[WaitingNorth.Count - 1].GetNumber() != NorthDir[0].GetNumber())
                                {
                                    //add to the waiting list
                                    WaitingNorth.Add(NorthDir[0]);
                                    //remove this car from the list of cars coming from North
                                    NorthDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                WaitingNorth.Add(NorthDir[0]);
                                //remove this car from the list of cars coming from North
                                NorthDir.RemoveAt(0);
                            }
                        }
                        
                    }
                }

                //check cars from South
                if (SouthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == SouthDir[0].GetArrival())
                    {
                        //if the South stoplight is still green we try to pass the car
                        if (south_change == false)
                        {
  
                            if (cars_Left_South.Count != 0)
                            {
                                if (cars_Left_South[0].GetNumber() != SouthDir[0].GetNumber())
                                {
                                    //set the exit time of the car
                                    SouthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car passed from South! \nSequence Number: " + SouthDir[0].GetNumber() + " Arrival Time: " +
                                    SouthDir[0].GetArrival() + " Exit Time: " + SouthDir[0].GetDepart() + " Wait Time: " +
                                            (SouthDir[0].GetDepart() - SouthDir[0].GetArrival()));
                                    //add this car to the list of already pased cars from South
                                    cars_Left_South.Add(SouthDir[0]);
                                    //remove this car from the list of cars coming from South
                                    SouthDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                SouthDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from South! \nSequence Number: " + SouthDir[0].GetNumber() + " Arrival Time: " +
                                    SouthDir[0].GetArrival() + " Exit Time: " + SouthDir[0].GetDepart() + " Wait Time: " +
                                            (SouthDir[0].GetDepart() - SouthDir[0].GetArrival()));
                                //add this car to the list of already pased cars from South
                                cars_Left_South.Add(SouthDir[0]);
                                //remove this car from the list of cars coming from South
                                SouthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the South stoplight is no longer green, we add the car to the waiting list
                            if (WaitingSouth.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (WaitingSouth[WaitingSouth.Count - 1].GetNumber() != SouthDir[0].GetNumber())
                                {
                                    //add to the waiting list
                                    WaitingSouth.Add(SouthDir[0]);
                                    //remove this car from the list of cars coming from South
                                    SouthDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                WaitingSouth.Add(SouthDir[0]);
                                //remove this car from the list of cars coming from South
                                SouthDir.RemoveAt(0);
                            }
                        }
                    }
                }

                //check East cars
                if (EastDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == EastDir[0].GetArrival())
                    {
                        //becuase currently the East stoplight is red, we put the cars waiting in line
                        if (WaitingEast.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingEast[WaitingEast.Count - 1].GetNumber() != EastDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingEast.Add(EastDir[0]);
                                //remove this car from the list of cars coming from East
                                EastDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingEast.Add(EastDir[0]);
                            //remove this car from the list of cars coming from East
                            EastDir.RemoveAt(0);
                        }
                    }
                }
                //check the car from West
                //first see if there are any cars left coming
                if (WestDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == WestDir[0].GetArrival())
                    {
                        //becuase currently the West stoplight is red, we put the cars waiting in line
                        if (WaitingWest.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingWest[WaitingWest.Count - 1].GetNumber() != WestDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingWest.Add(WestDir[0]);
                                //remove this car from the list of cars coming from West
                                WestDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingWest.Add(WestDir[0]);
                            //remove this car from the list of cars coming from West
                            WestDir.RemoveAt(0);
                        }
                    }
                }
            

            Count.Text = timer.Elapsed.Seconds.ToString();
                ts1 = timer1.Elapsed;
                Refresh();
            }

               
                

             
                
                
                    south_stoplight.change_light("Red");
                    Stop_South.BackColor = Color.Red;
                    Go_South.BackColor = Color.Gray;
                    yellow_South.BackColor = Color.Gray;
                    
                    Refresh();
                
            
        }

        //Method name: eastLight();
        //Method shows the operation of east light
        public void eastlight()
        {

            Stopwatch timer1 = new Stopwatch();
            TimeSpan ts1 = timer1.Elapsed;
            timer1.Start();

            Stop_East.BackColor = Color.Gray;
            yellow_East.BackColor = Color.Gray;
            Go_East.BackColor = Color.Lime;

            Refresh();

            east_stoplight.change_light("Green");
            //Printing output on console
            Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());

            max_East_Cars = max_East_Cars < WaitingEast.Count ? WaitingEast.Count : max_East_Cars;
            while (WaitingEast.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                WaitingEast[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                Console.WriteLine("A car passed from East! \nSequence Number: " + WaitingEast[0].GetNumber() + " Arrival Time: " +
                    WaitingEast[0].GetArrival() + " Exit Time: " + WaitingEast[0].GetDepart() + " Wait Time: " +
                    (WaitingEast[0].GetDepart() - WaitingEast[0].GetArrival()));
                //add this car to the list of already pased cars from East
                cars_Left_East.Add(WaitingEast[0]);
                //remove this car from the list of cars waiting in East
                WaitingEast.RemoveAt(0);
            }


            while (ts1.Seconds < 6)
            {
                //Reset timer after 60 seconds
                if (timer.Elapsed.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }

     
                if (EastDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == EastDir[0].GetArrival())
                    {
                        //check that this car has not already been passed - check sequence number because it must be unique
                        //this check is needed becuase the execution time of program is faster than the stopwatch timer
                        if (cars_Left_East.Count != 0)
                        {
                            if (cars_Left_East[0].GetNumber() != EastDir[0].GetNumber())
                            {
                                //set the exit time of the car
                                EastDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from East! \nSequence Number: " + EastDir[0].GetNumber() + " Arrival Time: " +
                                   EastDir[0].GetArrival() + " Exit Time: " + EastDir[0].GetDepart() + " Wait Time: " +
                                           (EastDir[0].GetDepart() - EastDir[0].GetArrival()));
                                //add this car to the list of already pased cars from East
                                cars_Left_East.Add(EastDir[0]);
                                //remove this car from the list of cars coming from East
                                EastDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //just add this car to the list of cars passed
                            //set the exit time of the car
                            EastDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                            //print the info that this car has passed the intersection
                            Console.WriteLine("A car passed from East! \nSequence Number: " + EastDir[0].GetNumber() + " Arrival Time: " +
                                   EastDir[0].GetArrival() + " Exit Time: " + EastDir[0].GetDepart() + " Wait Time: " +
                                           (EastDir[0].GetDepart() - EastDir[0].GetArrival()));
                            //add this car to the list of already pased cars from East
                            cars_Left_East.Add(EastDir[0]);
                            //remove this car from the list of cars coming from East
                            EastDir.RemoveAt(0);
                        }
                    }
                }

                if (SouthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == SouthDir[0].GetArrival())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (WaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingSouth[WaitingSouth.Count - 1].GetNumber() != SouthDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingSouth.Add(SouthDir[0]);
                                //remove this car from the list of cars coming from South
                                SouthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingSouth.Add(SouthDir[0]);
                            //remove this car from the list of cars coming from South
                            SouthDir.RemoveAt(0);
                        }
                    }
                }
                if (NorthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == NorthDir[0].GetArrival())
                    {
                        //becuase currently the North stoplight is red, we put the cars waiting in line
                        if (WaitingNorth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingNorth[WaitingNorth.Count - 1].GetNumber() != NorthDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingNorth.Add(NorthDir[0]);
                                //remove this car from the list of cars coming from North
                                NorthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingNorth.Add(NorthDir[0]);
                            //remove this car from the list of cars coming from North
                            NorthDir.RemoveAt(0);
                        }
                    }
                }

                if (WestDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == WestDir[0].GetArrival())
                    {
                        //becuase currently the West stoplight is red, we put the cars waiting in line
                        if (WaitingWest.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingWest[WaitingWest.Count - 1].GetNumber() != WestDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingWest.Add(WestDir[0]);
                                //remove this car from the list of cars coming from West
                                WestDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingWest.Add(WestDir[0]);
                            //remove this car from the list of cars coming from West
                            WestDir.RemoveAt(0);
                        }
                    }
                }

                Count.Text = timer.Elapsed.Seconds.ToString();
                ts1 = timer1.Elapsed;
                Refresh();
            }

            westlight();
        }

        //Method name: westLight();
        //Method shows the operation of west light
        public void westlight()
        {

            Stopwatch timer1 = new Stopwatch();
            TimeSpan ts1 = timer1.Elapsed;
            timer1.Start();

            Stop_West.BackColor = Color.Gray;
            yellow_West.BackColor = Color.Gray;
            Go_West.BackColor = Color.Lime;

            Refresh();

            west_stoplight.change_light("Green");

            //Printing the outputs on the console
            Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());


            max_West_Cars = max_West_Cars < WaitingWest.Count ? WaitingWest.Count : max_West_Cars;
            while (WaitingWest.Count != 0)
            {
                //set the car's exit time, which is current time plus 1
                WaitingWest[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                Console.WriteLine("A car passed from West! \nSequence Number: " + WaitingWest[0].GetNumber() + " Arrival Time: " +
                    WaitingWest[0].GetArrival() + " Exit Time: " + WaitingSouth[0].GetDepart() + " Wait Time: " +
                    (WaitingSouth[0].GetDepart() - WaitingSouth[0].GetArrival()));
                //add this car to the list of already pased cars from West
                cars_Left_West.Add(WaitingWest[0]);
                //remove this car from the list of cars waiting in West
                WaitingWest.RemoveAt(0);
            }

            //Calling a variable to prints the east light yellow on console
            bool east_yellow_printed = false;
            //Check if the variable prints the east light as red on the console
            bool east_red_printed = false;
            bool west_changed = false;

            while (ts1.Seconds < 12)
            {
                //Restart timer after 60 seconds
                if (timer.Elapsed.Minutes == 4)
                {
                    PrintResults();
                    Environment.Exit(0);
                }


                //Make the east light yellow after 3 seconds i.e. 9 seconds after the green turns on
                if (ts1.Seconds == 3)
                {
                    //Printing the east signal yellow on the console
                    if (!east_yellow_printed)
                    {
                        east_stoplight.change_light("Yellow");
                        yellow_East.BackColor = Color.Yellow;
                        Go_East.BackColor = Color.Gray;
                        Stop_East.BackColor = Color.Gray;
                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                    north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());
                        Refresh();

                        east_yellow_printed = true;
                    }
                }
                //Make the east light red after 6 seconds i.e. 12 seconds after the green turns on
                else if (ts1.Seconds == 6)
                {
                    //Printing the east signal red on the console
                    if (!east_red_printed)
                    {
                        east_stoplight.change_light("Red");
                        Stop_East.BackColor = Color.Red;
                        yellow_East.BackColor = Color.Gray;
                        Go_East.BackColor = Color.Gray;
                        Refresh();
                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                    north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());
                        east_red_printed = true;
                    }
                }
                else if (ts1.Seconds == 9)
                {
                    if (!west_changed)
                    {
                        //Changing west light to yellow
                        west_stoplight.change_light("Yellow");
                        Go_West.BackColor = Color.Gray;
                        yellow_West.BackColor = Color.Yellow;
                        Stop_West.BackColor = Color.Gray;

                        //Printing output on console
                        Console.WriteLine("{0,3} {1,-2} {2, 9} {3,-13} {4, -14} {5, -16} {6, -13}", " ", timer.Elapsed.Seconds.ToString(), " ",
                        north_stoplight.get_lightColor(), south_stoplight.get_lightColor(), east_stoplight.get_lightColor(), west_stoplight.get_lightColor());

                        Refresh();
                    }
                }

                if (EastDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == EastDir[0].GetArrival())
                    {
                        //if the East stoplight is still green we try to pass the car
                        if (east_red_printed == false && east_yellow_printed == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (cars_Left_East.Count != 0)
                            {
                                if (cars_Left_East[0].GetNumber() != cars_Left_East[0].GetNumber())
                                {
                                    //set the exit time of the car
                                    EastDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car passed from East! \nSequence Number: " + EastDir[0].GetNumber() + " Arrival Time: " +
                                    EastDir[0].GetArrival() + " Exit Time: " + EastDir[0].GetDepart() + " Wait Time: " +
                                            (EastDir[0].GetDepart() - EastDir[0].GetArrival()));
                                    //add this car to the list of already pased cars from East
                                    cars_Left_East.Add(EastDir[0]);
                                    //remove this car from the list of cars coming from East
                                    EastDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                EastDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from East! \nSequence Number: " + EastDir[0].GetNumber() + " Arrival Time: " +
                                    EastDir[0].GetArrival() + " Exit Time: " + EastDir[0].GetDepart() + " Wait Time: " +
                                            (EastDir[0].GetDepart() - EastDir[0].GetArrival()));
                                //add this car to the list of already pased cars from East
                                cars_Left_East.Add(EastDir[0]);
                                //remove this car from the list of cars coming from East
                                EastDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the East stoplight is no longer green, we add the car to the waiting list
                            if (WaitingEast.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (WaitingEast[WaitingEast.Count - 1].GetNumber() != EastDir[0].GetNumber())
                                {
                                    //add to the waiting list
                                    WaitingEast.Add(EastDir[0]);
                                    //remove this car from the list of cars coming from East
                                    EastDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                WaitingEast.Add(EastDir[0]);
                                //remove this car from the list of cars coming from East
                                EastDir.RemoveAt(0);
                            }
                        }
                    }
                }
                if (WestDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == WestDir[0].GetArrival())
                    {
                        //if the West stoplight is still green we try to pass the car
                        if (west_changed == false)
                        {
                            //check that this car has not already been passed - check sequence number because it must be unique
                            //this check is needed becuase the execution time of program is faster than the stopwatch timer
                            if (cars_Left_West.Count != 0)
                            {
                                if (cars_Left_West[0].GetNumber() != WestDir[0].GetNumber())
                                {
                                    //set the exit time of the car
                                    WestDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                    //print the info that this car has passed the intersection
                                    Console.WriteLine("A car passed from West! \nSequence Number: " + WestDir[0].GetNumber() + " Arrival Time: " +
                                     WestDir[0].GetArrival() + " Exit Time: " + WestDir[0].GetDepart() + " Wait Time: " +
                                             (WestDir[0].GetDepart() - WestDir[0].GetArrival()));
                                    //add this car to the list of already pased cars from West
                                    cars_Left_West.Add(WestDir[0]);
                                    //remove this car from the list of cars coming from West
                                    WestDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //just add this car to the list of cars passed
                                //set its exit time
                                WestDir[0].SetDepart((int)Math.Round(timer.Elapsed.TotalSeconds) + 1);
                                //print the info that this car has passed the intersection
                                Console.WriteLine("A car passed from West! \nSequence Number: " + WestDir[0].GetNumber() + " Arrival Time: " +
                                     WestDir[0].GetArrival() + " Exit Time: " + WestDir[0].GetDepart() + " Wait Time: " +
                                             (WestDir[0].GetDepart() - WestDir[0].GetArrival()));
                                //add this car to the list of already pased cars from West
                                cars_Left_West.Add(WestDir[0]);
                                //remove this car from the list of cars coming from West
                                WestDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //if the West stoplight is no longer green, we add the car to the waiting list
                            if (WaitingWest.Count != 0)
                            {
                                //make sure that this car is not already added to the waiting list
                                if (WaitingWest[WaitingWest.Count - 1].GetNumber() != WestDir[0].GetNumber())
                                {
                                    //add to the waiting list
                                    WaitingWest.Add(WestDir[0]);
                                    //remove this car from the list of cars coming from West
                                    WestDir.RemoveAt(0);
                                }
                            }
                            else
                            {
                                //add to the waiting list
                                WaitingWest.Add(WestDir[0]);
                                //remove this car from the list of cars coming from West
                                WestDir.RemoveAt(0);
                            }
                        }
                    }
                }

                if (NorthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == NorthDir[0].GetArrival())
                    {
                        //becuase currently the North stoplight is red, we put the cars waiting in line
                        if (WaitingNorth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingNorth[WaitingNorth.Count - 1].GetNumber() != NorthDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingNorth.Add(NorthDir[0]);
                                //remove this car from the list of cars coming from North
                                NorthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingNorth.Add(NorthDir[0]);
                            //remove this car from the list of cars coming from North
                            NorthDir.RemoveAt(0);
                        }
                    }
                }
                if (SouthDir.Count != 0)
                {
                    if ((int)Math.Round(timer.Elapsed.TotalSeconds) == SouthDir[0].GetArrival())
                    {
                        //becuase currently the South stoplight is red, we put the cars waiting in line
                        if (WaitingSouth.Count != 0)
                        {
                            //make sure that this car is not already added to the waiting list
                            if (WaitingSouth[WaitingSouth.Count - 1].GetNumber() != SouthDir[0].GetNumber())
                            {
                                //add to the waiting list
                                WaitingSouth.Add(SouthDir[0]);
                                //remove this car from the list of cars coming from South
                                SouthDir.RemoveAt(0);
                            }
                        }
                        else
                        {
                            //add to the waiting list
                            WaitingSouth.Add(SouthDir[0]);
                            //remove this car from the list of cars coming from South
                            WestDir.RemoveAt(0);
                        }
                    }
                }

                Count.Text = timer.Elapsed.Seconds.ToString();
                ts1 = timer1.Elapsed;
                Refresh();
            }

                    //changing west light to red
                    west_stoplight.change_light("Red");
                    Go_West.BackColor = Color.Gray;
                    yellow_West.BackColor = Color.Gray;
                    Stop_West.BackColor = Color.Red;
                    
                    Refresh();
                
            
        }

        public void SeperateDirections(string[] text)
        {
            //we will loop through the list of strings and determine what direction and what arrival times are
            //at each iteration we will create a new Car object and depending on the direction put it into the list
            foreach (var line in text)
            {
                if (line[0] == 'N')
                {
                    Cars north_Car = new Cars(NorthDir.Count + 1, int.Parse(line.Substring(1)));
                    NorthDir.Add(north_Car);
                }

                else if (line[0] == 'S')
                {
                    Cars south_Car = new Cars(SouthDir.Count + 1, int.Parse(line.Substring(1)));
                    SouthDir.Add(south_Car);
                }

                else if (line[0] == 'E')
                {
                    Cars east_Car = new Cars(EastDir.Count + 1, int.Parse(line.Substring(1)));
                    EastDir.Add(east_Car);
                }

                else if (line[0] == 'W')
                {
                    Cars west_Car = new Cars(WestDir.Count + 1, int.Parse(line.Substring(1)));
                    WestDir.Add(west_Car);
                }

            }
        }

        //helper method to print the results
        public void PrintResults()
        {
            Console.WriteLine("\nNumber of cars that came from each direction: ");
            Console.WriteLine("North: " + cars_Left_North.Count + "  South: " + cars_Left_South.Count +
                "  East: " + cars_Left_East.Count + "  West: " + cars_Left_West.Count);

            Console.WriteLine("\nThe maximum line of cars that had to wait: ");
            Console.WriteLine("North: " + max_North_Cars + "  South: " + max_South_Cars + "  East: " + max_East_Cars +
                "  West: " + max_East_Cars);

            //average waiting time is the total wait time of all cars coming from each diriction divided by the number
            //of cars that passed from respective directions
            Console.WriteLine("\nThe average waiting time Of Cars: ");
            int total_wait_time = 0;
            foreach (var car in cars_Left_North)
            {
                total_wait_time += (car.GetDepart() - car.GetArrival());
            }
            int avrge_north = total_wait_time / cars_Left_North.Count;

            total_wait_time = 0;
            foreach (var car in cars_Left_South)
            {
                total_wait_time += (car.GetDepart() - car.GetArrival());
            }
            int avrge_south = total_wait_time / cars_Left_South.Count;

            total_wait_time = 0;
            foreach (var car in cars_Left_East)
            {
                total_wait_time += (car.GetDepart() - car.GetArrival());
            }
            int avrge_east = total_wait_time / cars_Left_East.Count;

            total_wait_time = 0;
            foreach (var car in cars_Left_West)
            {
                total_wait_time += (car.GetDepart() - car.GetArrival());
            }
            int avrge_west = total_wait_time / cars_Left_West.Count;
            Console.WriteLine("North: " + avrge_north + "  South: " + avrge_south + "  East: " +
                "  West: " + avrge_east);

            Console.Read();
        }
    }
}


 
