﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using BLApi;
using BO;
using DO;
using DalApi;

namespace BL
{
    class BLImp : IBL
    {
        IDL dl = DLFactory.GetDL();

       

        //#region GetStationInLine
        ///// <summary>
        ///// A BO function that return a BO bus
        ///// </summary>
        ///// <param name = "id" ></ param >
        ///// < returns ></ returns >
        //public BO.StationInLine GetStationInLine(int stationCode, int lineId)
        //{
        //    DO.LineStation lineStationDO;
        //    BO.StationInLine stationinlineBO;
        //    try
        //    {
        //        lineStationDO = dl.GetLineStation(lineId, stationCode);
        //    }
        //    catch (DO.IncorrectInputException ex)
        //    {
        //        throw new BO.IncorrectInputException(ex.Message);
        //    }
            
        //    stationinlineBO = DeepCopyUtilities.CopyToStationInLine(lineStationDO);
        //    return stationinlineBO;
        //}
        //#endregion

        #region Bus

        #region busDoBoAdapter
        /// <summary>
        /// A function that copy details of bus from DO to BO
        /// </summary>
        /// <param name="busDO"></param>
        /// <returns></returns>
        BO.Bus busDoBoAdapter(DO.Bus busDO)
        {
            BO.Bus busBO = new BO.Bus();
            busDO.CopyPropertiesTo(busBO);
            return busBO;
        }
        #endregion

        #region GetBus
        /// <summary>
        /// A BO function that return a BO bus
        /// </summary>
        /// <param name = "id" ></ param >
        /// < returns ></ returns >
        public BO.Bus GetBus(int id)
        {
            DO.Bus busDO;
            try
            {
                busDO = dl.GetBus(id);
            }
            catch (DO.IncorrectLicenseNumberException ex)
            {
                throw new BO.IncorrectLicenseNumberException(ex.licenseNumber, ex.Message);
            }
            return busDoBoAdapter(busDO);
        }
        #endregion

        #region GetAllBuses
        /// <summary>
        /// A BO function that return all the buses 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.Bus> GetAllBuses()//פונקצית הרחבה
        {
            return from bus in dl.GetAllBuses()
                   select busDoBoAdapter(bus);
        }
        #endregion

        #region CountDigit - help function
        /// <summary>
        /// A function that count how many digits in the number
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int CountDigit(int num)
        {
            int counter = 0;
            while (num != 0)
            {
                num = num / 10;
                counter++;
            }
            return counter;
        }
        #endregion

        #region AddBus
        /// <summary>
        /// A BO function that add a bus to the list
        /// </summary>
        /// <param name="bus"></param>
        public void AddBus(BO.Bus bus)
        {
            try
            {
                DO.Bus busDOtemp = new DO.Bus();
                bus.CopyPropertiesTo(busDOtemp);
                bus.IsDeleted = false;//אם מוסיפיםם זה וודאי לא מחוק
                if (bus.DateBegin > DateTime.Now) //נבדוק האם התאריך תקין
                    throw new BO.IncorrectDateException(bus.DateBegin, "The date begin not valid");
                if (bus.TotalMileage < 0) // אם הקילומטראז פחות מ-0 אזי נשים 0 כברירת מחדל
                    bus.TotalMileage = 0;
                if (bus.FuelRemain < 0 || bus.FuelRemain > 1200) //אם הדלק לא תקין נמלא את המיכל עד 1200
                    bus.FuelRemain = 1200;

                int lengthLicenceNumber = CountDigit(bus.LicenseNumber); //נבדוק האם מספר הספרות בלוחית רישוי תואמת את שנת הייצור
                if (lengthLicenceNumber != 7 && lengthLicenceNumber != 8)
                    throw new BO.IncorrectLicenseNumberException(bus.LicenseNumber, "The license number isn't correct");
                if (((lengthLicenceNumber == 7 && bus.DateBegin.Year >= 2018) || (lengthLicenceNumber == 8 && bus.DateBegin.Year < 2018)))
                    throw new BO.IncorrectLicenseNumberOrDateException(bus.LicenseNumber, bus.DateBegin, "The license number or the date begin isn't correct");
                if (bus.LastTreatment > DateTime.Now || bus.LastTreatment < bus.DateBegin)
                    throw new BO.IncorrectDateException(bus.LastTreatment, "The date of last treatment is not valid");
                if (bus.KmBeforTreatment < 0 || bus.KmBeforTreatment > bus.TotalMileage)
                    throw new BO.IncorrectInputException("The kilometrage of last treatment is not valid");
                if(bus.KmBeforeFuel < 0 || bus.KmBeforeFuel > 1200)
                    throw new BO.IncorrectInputException("The kilometrage of last fuel is not valid");

                dl.AddBus(busDOtemp);
            }
            catch (DO.IncorrectLicenseNumberException ex)
            {
                throw new BO.IncorrectLicenseNumberException(ex.licenseNumber, ex.Message);
            }

        }
        #endregion

        #region UpdateBus
        /// <summary>
        /// A BO function that update the bus
        /// </summary>
        /// <param name="bus"></param>

        public void UpdateBus(BO.Bus bus)
        {
            DO.Bus busDO;
            try
            {
                busDO = dl.GetBus(bus.LicenseNumber);
                bus.CopyPropertiesTo(busDO);
                dl.DeleteBus(bus.LicenseNumber);
                dl.AddBus(busDO);
            }

            catch (DO.IncorrectLicenseNumberException ex)
            {
                throw new BO.IncorrectLicenseNumberException(ex.licenseNumber, ex.Message);
            }

        }
        #endregion

        #region DeleteBus
        /// <summary>
        /// A BO function that delete bus 
        /// </summary>
        /// <param name="id"></param>
        public void DeleteBus(BO.Bus busBO)
        {
            try
            {
                dl.DeleteBus(busBO.LicenseNumber);
            }
            catch (DO.IncorrectLicenseNumberException ex)
            {
                throw new BO.IncorrectLicenseNumberException(ex.licenseNumber, ex.Message);
            }
        }
        #endregion





        #endregion

        #region Line
        #region lineDoBoAdapter
        /// <summary>
        /// A function that copy details from DO to BO
        /// </summary>
        /// <param name="lineDO"></param>
        /// <returns></returns>
        BO.Line lineDoBoAdapter(DO.Line lineDO)
        {
            BO.Line lineBO = new BO.Line();
            int lineId = lineDO.Id;

            List<BO.StationInLine> stations = new List<BO.StationInLine>();
            stations = (from stat in dl.GetAllLinesStationBy(stat => stat.LineId == lineId && stat.IsDeleted == false)
                                               let station = dl.GetStation(stat.StationCode)
                                               select station.CopyToStationInLine(stat)).ToList();
            stations = (stations.OrderBy(s => s.LineStationIndex)).ToList();
            foreach (BO.StationInLine s in stations)
            {
                if (s.LineStationIndex != stations[stations.Count - 1].LineStationIndex) // if this is not the end
                {
                    int sc1 = s.StationCode;//station code 1
                    int sc2 = stations[s.LineStationIndex+1].StationCode;//station code 2
                    DO.AdjacentStations adjStat = dl.GetAdjacentStations(sc1, sc2);
                    s.DistanceTo = adjStat.Distance;
                    s.TimeTo = adjStat.TravelTime;
                }
            }
            lineBO.ListOfStationsInLine = stations;
            //BO.Station sBO;
            //DO.Station sDO = dl.GetStation(lineDO.FirstStation);
            //lineBO.FirstStation = stationDoBoAdapter(sDO);
            //sDO = dl.GetStation(lineDO.LastStation);
            //lineBO.LastStation = stationDoBoAdapter(sDO);
            lineBO = lineDO.CopyToLine();

            return lineBO;
        }
        #endregion

        #region GetLine
        /// <summary>
        ///  A function that return a line
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BO.Line GetLine(int id)
        {

            DO.Line lineDO;
            try
            {
                lineDO = dl.GetLine(id);
            }

            catch (DO.IncorrectLineIDException ex)
            {
                throw new BO.IncorrectLineIDException(ex.ID, ex.Message);
            }

            return lineDoBoAdapter(lineDO);
        }
        #endregion

        #region GetAllLines
        /// <summary>
        /// A BO function that return all the lines
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.Line> GetAllLines()
        {
            return from line in dl.GetAllLines()
                   select lineDoBoAdapter(line);

        }
        #endregion

        #region AddLine
        /// <summary>
        /// A function that add a line to the list
        /// </summary>
        /// <param name="line"></param>
        public void AddLine(BO.Line line)
        {
            try
            {
                DO.Line lineDOtemp = new DO.Line();
                lineDOtemp.CopyPropertiesTo(line);

                if (line.LineNumber < 0 || line.Fare < 0 || line.TravelTimeInThisLine == TimeSpan.Zero) //מספר הקו שלילי ולא תקין או מחיר הנסיעה שלילי או זמן הנסיעה 0
                    throw new BO.IncorrectInputException("The line number or fare of trabel time not valid");

                DO.Station sDO1 = dl.GetStation(line.FirstStation.Code);
                DO.Station sDO2 = dl.GetStation(line.LastStation.Code);

                if ((sDO1 == null) || (sDO2 == null))
                    throw new BO.IncorrectCodeStationException(line.FirstStation.Code, "This station code could not found");
                line.FirstStation = sDO1.CopyToStation();
                line.LastStation = sDO2.CopyToStation();

                //לעבור על רשימת התחנות ולשאול האם אני בתחנה מסוימת ובאותה תחנה להוסיף את הקו לרשימת הקווים
                List<BO.StationInLine> tempListStations = new List<StationInLine>();
                if (line.ListOfStationsInLine.Count() == 0)
                    throw new ArgumentNullException("The list of the line is empty");

                dl.AddLine(lineDOtemp);
                foreach (BO.StationInLine station in line.ListOfStationsInLine)
                    if (station.LineId == line.Id)
                        tempListStations.Add(station);
                BO.Station bs;
                BO.ShortLine sline = new ShortLine();
                for (int i = 0; i < tempListStations.Count(); i++) //נעבור על רשימת התחנות קו שיש בקו הזה
                {
                    foreach (DO.Station station in dl.GetAllStationes())//נעבור על כל רשימת התחנות הקיימות
                        if (station.Code == tempListStations[i].StationCode)
                        {
                            bs = GetStation(station.Code);
                            sline.Id = line.Id;
                            sline.IsDeleted = line.IsDeleted;
                            sline.LastStation = line.LastStation;
                            sline.LineNumber = line.LineNumber;

                            bs.ListOfLines.ToList().Add(sline); //עדכון הקו הרצוי ברשימת התחנות שהוא עובר בה
                        }
                }
            }
            catch (DO.IncorrectLineIDException ex)
            {
                throw new BO.IncorrectLineIDException(ex.ID, ex.Message);
            }
            
        }
        #endregion
        //////////////////////////////////////////



        //#region UpdateLine
        ///// <summary>
        ///// A function that update the line 
        ///// </summary>
        ///// <param name="line"></param>
        //public void UpdateLine(DO.Line line)
        //{
        //    DO.Line l = DataSource.ListLines.Find(p => p.Id == line.Id && p.IsDeleted);

        //    if (l != null)
        //    {
        //        DataSource.ListLines.Remove(l);
        //        DataSource.ListLines.Add(line.Clone());
        //    }
        //    else
        //        throw new Exception();

        //}
        //#endregion

        //#region UpdateLine
        ///// <summary>
        ///// method that knows to updt specific fields in Line
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="update"></param>
        //public void UpdateLine(int id, Action<DO.Line> update)
        //{
        //    DO.Line line = DataSource.ListLines.Find(p => p.Id == id && p.IsDeleted == false);
        //    if (line == null)
        //        throw new Exception();
        //    update(line);
        //}
        //#endregion

        //#region DeleteLine
        ///// <summary>
        ///// A function that delete line (mark the flag IsDeleted = true) 
        ///// </summary>
        ///// <param name="id"></param>
        //public void DeleteLine(int id)
        //{
        //    DO.Line l = DataSource.ListLines.Find(p => p.Id == id && p.IsDeleted);

        //    if (l != null)
        //    {
        //        //DataSource.ListLines.Remove(l);
        //        l.IsDeleted = true;
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion

        #endregion

        #region Station

        #region stationDoBoAdapter
        /// <summary>
        /// A function that copy details from DO to BO
        /// </summary>
        /// <param name="busDO"></param>
        /// <returns></returns>
        BO.Station stationDoBoAdapter(DO.Station stationDO)
        {
            BO.Station stationBO = new BO.Station();
            int stationCode = stationDO.Code;
            stationBO = stationDO.CopyToStation();


            stationBO.ListOfLines = (from stat in dl.GetAllLinesStationBy(stat => stat.StationCode == stationCode && stat.IsDeleted == false)
                                     let line = dl.GetLine(stat.LineId)
                                     select line.CopyToLineInStation(stat)).ToList();

            stationBO.ListOfStationsInLines = (from stat in dl.GetAllLinesStationBy(stat => stat.StationCode == stationCode && stat.IsDeleted == false)
                                               let station = dl.GetStation(stat.StationCode)
                                               select station.CopyToStationInLine(stat)).ToList();
            return stationBO;
        }
        #endregion

        #region GetStation
        /// <summary>
        ///  A function that return a BO station
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public BO.Station GetStation(int code)
        {
            DO.Station stationDO;
            try
            {
                stationDO = dl.GetStation(code);
            }

            catch (DO.IncorrectCodeStationException ex)
            {
                throw new BO.IncorrectLineIDException(ex.stationCode, ex.Message);
            }

            return stationDoBoAdapter(stationDO);
        }
        #endregion

        //////////////////////////////////////////////////

        //#region GetAllStationes
        ///// <summary>
        ///// A function that return all the stations
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<BO.Station> GetAllStationes()
        //{
        //    return from station in dl.GetAllStationes()
        //           select busDoBoAdapter(station);
        //}
        //#endregion

        //#region GetAllStationesBy
        ///// <summary>
        ///// A function that returns the stations that have the special thing that the predicat do
        ///// </summary>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public IEnumerable<DO.Station> GetAllStationesBy(Predicate<DO.Station> predicate)
        //{
        //    return from station in DataSource.ListStations
        //           where predicate(station)
        //           select station.Clone();
        //}
        //#endregion


        //#region AddStation
        ///// <summary>
        ///// A function that add a station to the list
        ///// </summary>
        ///// <param name="station"></param>
        //public void AddStation(DO.Station station)
        //{
        //    if (DataSource.ListStations.FirstOrDefault(p => p.Code == station.Code && p.IsDeleted) != null)
        //        throw new Exception();
        //    DataSource.ListStations.Add(station.Clone());

        //}
        //#endregion

        //#region UpdateStation
        ///// <summary>
        ///// A function that update the station
        ///// </summary>
        ///// <param name="station"></param>
        //public void UpdateStation(DO.Station station)
        //{
        //    DO.Station b = DataSource.ListStations.Find(p => p.Code == station.Code && p.IsDeleted);

        //    if (b != null)
        //    {
        //        DataSource.ListStations.Remove(b);
        //        DataSource.ListStations.Add(station.Clone());
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion

        //#region UpdateStation
        ///// <summary>
        ///// method that knows to updt specific fields in station
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="update"></param>
        //public void UpdateStation(int id, Action<DO.Station> update)
        //{
        //    DO.Station station = DataSource.ListStations.Find(p => p.Code == id && p.IsDeleted == false);
        //    if (station == null)
        //        throw new Exception();
        //    update(station);
        //}
        //#endregion

        //#region DeleteStation
        ///// <summary>
        ///// A function that delete station (mark the flag IsDeleted = true) 
        ///// </summary>
        ///// <param name="code"></param>
        //public void DeleteStation(int code)
        //{
        //    DO.Station b = DataSource.ListStations.Find(p => p.Code == code && p.IsDeleted);

        //    if (b != null)
        //    {
        //        //DataSource.ListStations.Remove(b);
        //        b.IsDeleted = true;
        //    }
        //    else
        //        throw new Exception();
        //}
        //        #endregion
        //        #endregion

        //        #region Line

        //#region lineDoBoAdapter
        /// <summary>
        /// A function that copy details from DO to BO
        /// </summary>
        /// <param name = "lineDO" ></ param >
        /// < returns ></ returns >
        //BO.Line lineDoBoAdapter(DO.Line lineDO)
        //{
        //    BO.Line lineBO = new BO.Line();
        //    int lineId = lineBO.Id;
        //    lineDO.CopyPropertiesTo(lineBO);

        //    List<BO.StationInLine> stations = (from stat in dl.GetAllLinesStationBy(stat => stat.LineId == lineId && stat.IsDeleted == false)
        //                                       let station = dl.GetStation(stat.StationCode)
        //                                       select station.CopyToStationInLine(stat)).ToList();
        //    stations = (stations.OrderBy(s => s.LineStationIndex)).ToList();
        //    foreach (BO.StationInLine s in stations)
        //    {
        //        if (s.LineStationIndex != stations[stations.Count - 1].LineStationIndex) // if this is not the end
        //        {
        //            int sc1 = s.StationCode;//station code 1
        //            int sc2 = stations[s.LineStationIndex].StationCode;//station code 2
        //            DO.AdjacentStations adjStat = dl.GetAdjacentStations(sc1, sc2);
        //            s.DistanceTo = adjStat.Distance;
        //            s.TimeTo = adjStat.TravelTime;
        //        }
        //    }
        //    lineBO.ListOfStationsInLine = stations;
        //    return lineBO;
        //}
        //#endregion

        //#region GetAllLines
        /// <summary>
        /// A function that return all the lines
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<BO.Line> GetAllLines()
        //{
        //    return from line in dl.GetAllLines()
        //           select lineDoBoAdapter(line);

        //}
        //#endregion


        #endregion

        #region User

        #region userDoBoAdapter
        /// <summary>
        /// A function that copy user's details from DO to BO
        /// </summary>
        /// <param name="busDO"></param>
        /// <returns></returns>
        BO.User userDoBoAdapter(DO.User userDO)
        {
            BO.User userBO = new BO.User();
            userDO.CopyPropertiesTo(userBO);
            return userBO;
        }
        #endregion

        #region GetUser
        /// <summary>
        /// A BO function that return a user
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BO.User GetUser(string name)
        {
            DO.User userDO;
            try
            {
                userDO = dl.GetUser(name);
            }

            catch (DO.IncorrectUserNameException ex)
            {
                throw new BO.IncorrectUserNameException(ex.UserName, ex.Message);
            }

            return userDoBoAdapter(userDO);
        }
        #endregion

        #region GetAllUsers
        /// <summary>
        /// A BO function that return all the users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.User> GetAllUsers()
        {
            {
                return from user in dl.GetAllUsers()
                       select userDoBoAdapter(user);
            }
        }
        #endregion

        #region AddUser
        /// <summary>
        /// A BO function that add a user to the list
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(BO.User userBO)

        {
            try
            {
                DO.User userDO = new DO.User();
                userBO.CopyPropertiesTo(userDO);
                userDO.IsDeleted = false;
                dl.AddUser(userDO);
            }
            catch (DO.IncorrectUserNameException ex)
            {
                throw new BO.IncorrectUserNameException(ex.UserName, ex.Message);
            }

        }
        #endregion

        #region Charge
        /// <summary>
        /// A function that charge the balance of the user
        /// </summary>
        /// <param name="balance"></param>
        /// <param name="user"></param>
        public void Charge(int balance, BO.User user)
        {
            if (user == null)
                throw new ArgumentNullException("User was null");
            else if ((user.UserProfile == BO.Profile.Youth) || (user.UserProfile == BO.Profile.Veteran) || (user.UserProfile == BO.Profile.ExtendedStudent))
            {
                user.Balance = user.Balance + balance * 1.5;
            }
            else if (user.UserProfile == BO.Profile.Normal)
            {
                user.Balance = user.Balance + balance * 1.25;
            }
            else if (user.UserProfile == BO.Profile.OrdinaryStudent)
            {
                user.Balance = user.Balance + balance * 1.333;
            }
        }
        #endregion

        #region UpdateUser
        /// <summary>
        /// A BO function that update the user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(BO.User user)
        {
            DO.User userDO;
            try
            {
                userDO = dl.GetUser(user.UserName);
                user.CopyPropertiesTo(userDO);
                dl.DeleteUser(user.UserName);
                dl.AddUser(userDO);
            }

            catch (DO.IncorrectUserNameException ex)
            {
                throw new BO.IncorrectUserNameException(ex.UserName, ex.Message);
            }
        }
        #endregion

        #region DeleteUser
        /// <summary>
        /// A Bo function that delete user 
        /// </summary>
        /// <param name="userName"></param>
        public void DeleteUser(BO.User userBO)
        {
            try
            {
                dl.DeleteUser(userBO.UserName);
            }
            catch (DO.IncorrectUserNameException ex)
            {
                throw new BO.IncorrectUserNameException(ex.UserName, ex.Message);
            }
        }
        #endregion

        #endregion 

        #region Trip

        #region tripDoBoAdapter
        /// <summary>
        /// A function that copy details from DO to BO
        /// </summary>
        /// <param name="tripDO"></param>
        /// <returns></returns>
        BO.Trip tripDoBoAdapter(DO.Trip tripDO)
        {
            BO.Trip tripBO = new BO.Trip();
            tripDO.CopyPropertiesTo(tripBO);
            return tripBO;
        }
        #endregion

        #region GetTrip
        /// <summary>
        /// A function that return trip
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BO.Trip GetTrip(int id)
        {
            DO.Trip tripDO;
            try
            {
                tripDO = dl.GetTrip(id);
            }

            catch (DO.IncorrectTripIDException ex)
            {
                throw new BO.IncorrectTripIDException(ex.ID, ex.Message);
            }

            return tripDoBoAdapter(tripDO);
        }
        #endregion
        //#region GetAllTrips
        ///// <summary>
        ///// A function that return all the trips
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<DO.Trip> GetAllTrips()
        //{
        //    return from user in DataSource.ListTrips
        //           select user.Clone();
        //}
        //#endregion

        //#region GetAllTripsBy
        ///// <summary>
        ///// A function that returns the trip that have the special thing that the predicat do
        ///// </summary>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public IEnumerable<DO.Trip> GetAllTripsBy(Predicate<DO.Trip> predicate)
        //{
        //    return from trip in DataSource.ListTrips
        //           where predicate(trip)
        //           select trip.Clone();
        //}
        //#endregion


        //#region AddTrip
        ///// <summary>
        ///// A function that add trip to the list
        ///// </summary>
        ///// <param name="trip"></param>
        //public void AddTrip(DO.Trip trip)
        //{
        //    if (DataSource.ListTrips.FirstOrDefault(p => p.Id == trip.Id && p.IsDeleted) != null)
        //        throw new Exception();
        //    trip.Id = DO.Configuration.TripID;
        //    DataSource.ListTrips.Add(trip.Clone());
        //}
        //#endregion

        //#region UpdateTrip
        ///// <summary>
        /////  A function that update the trip
        ///// </summary>
        ///// <param name="trip"></param>
        //public void UpdateTrip(DO.Trip trip)
        //{
        //    DO.Trip u = DataSource.ListTrips.Find(p => p.Id == trip.Id && p.IsDeleted);

        //    if (u != null)
        //    {
        //        DataSource.ListTrips.Remove(u);
        //        DataSource.ListTrips.Add(trip.Clone());
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion

        //#region UpdateTrip
        ///// <summary>
        ///// method that knows to updt specific fields in Trip
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="update"></param>
        //public void UpdateTrip(int id, Action<DO.Trip> update) //method that knows to updt specific fields in Trip
        //{
        //    DO.Trip trip = DataSource.ListTrips.Find(p => p.Id == id && p.IsDeleted == false);
        //    if (trip == null)
        //        throw new Exception();
        //    update(trip);
        //}
        //#endregion

        //#region DeleteTrip
        ///// <summary>
        ///// A function that delete trip (mark the flag IsDeleted = true)
        ///// </summary>
        ///// <param name="id"></param>
        //public void DeleteTrip(int id)
        //{
        //    DO.Trip u = DataSource.ListTrips.Find(p => p.Id == id && p.IsDeleted);

        //    if (u != null)
        //    {
        //        //DataSource.ListTrips.Remove(u);
        //        u.IsDeleted = true;
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion
        #endregion


        ////////////////////////////////////////////////////////////   

        #region BusOnTrip

        //#region BusOnTripDoBoAdapter
        ///// <summary>
        ///// A function that copy details from DO to BO
        ///// </summary>
        ///// <param name="busDO"></param>
        ///// <returns></returns>
        //BO.BusOnTrip BusOnTripDoBoAdapter(DO.BusOnTrip BusOnTripDO)
        //{
        //    BO.BusOnTrip BusOnTripBO = new BO.BusOnTrip();
        //    BusOnTripDO.CopyPropertiesTo(BusOnTripBO);
        //    return BusOnTripBO;
        //}
        //#endregion 

        //#region GetBusOnTrip
        ///// <summary>
        /////  A function that return a bus on trip
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="licenseNumber"></param>
        ///// <returns></returns>
        //public DO.BusOnTrip GetBusOnTrip(int id, int licenseNumber)
        //{
        //    DO.BusOnTrip b = DataSource.ListBusesOnTrip.Find(p => p.Id == id && p.LicenseNumber == licenseNumber && p.IsDeleted);

        //    if (b != null)
        //        return b.Clone();
        //    else

        //        throw new Exception();
        //}
        //#endregion

        //#region GetAllBusesOnTrip
        ///// <summary>
        ///// A function that return all the buses on trip
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<DO.BusOnTrip> GetAllBusesOnTrip()
        //{
        //    return from bus in DataSource.ListBusesOnTrip
        //           select bus.Clone();
        //}
        //#endregion

        //#region GetAllBusesOnTripBy
        ///// <summary>
        /////  A function that returns the buses on trip that have the special thing that the predicat do
        ///// </summary>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public IEnumerable<DO.BusOnTrip> GetAllBusesOnTripBy(Predicate<DO.BusOnTrip> predicate)
        //{
        //    return from bus in DataSource.ListBusesOnTrip
        //           where predicate(bus)
        //           select bus.Clone();
        //}
        //#endregion

        //#region AddBusOnTrip
        ///// <summary>
        ///// A function that add a bus on trip to the list
        ///// </summary>
        ///// <param name="bus"></param>
        //public void AddBusOnTrip(DO.BusOnTrip bus)
        //{

        //    if (DataSource.ListBusesOnTrip.FirstOrDefault(p => p.Id == bus.Id && p.LicenseNumber == bus.LicenseNumber && p.IsDeleted) != null)
        //        throw new Exception();
        //    bus.Id = DO.Configuration.BusOnTripID++;//המספר הרץ
        //    DataSource.ListBusesOnTrip.Add(bus.Clone());
        //}
        //#endregion

        //#region UpdateBusOnTrip
        ///// <summary>
        ///// A function that update the bus on trip
        ///// </summary>
        ///// <param name="bus"></param>
        //public void UpdateBusOnTrip(DO.BusOnTrip bus)
        //{
        //    DO.BusOnTrip b = DataSource.ListBusesOnTrip.Find(p => p.Id == bus.Id && p.LicenseNumber == bus.LicenseNumber && p.IsDeleted);

        //    if (b != null)
        //    {
        //        DataSource.ListBusesOnTrip.Remove(b);
        //        DataSource.ListBusesOnTrip.Add(bus.Clone());
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion UpdateBusOnTrip

        //#region UpdateBusOnTrip

        ///// <summary>
        ///// method that knows to updt specific fields in BusOnTrip
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="update"></param>
        //public void UpdateBusOnTrip(int id, int licenseNumber, Action<DO.BusOnTrip> update)
        //{
        //    DO.BusOnTrip b = DataSource.ListBusesOnTrip.Find(p => p.Id == id && p.LicenseNumber == licenseNumber && p.IsDeleted == false);
        //    if (b == null)
        //        throw new Exception();
        //    update(b);
        //}
        //#endregion

        //#region DeleteBusOnTrip
        ///// <summary>
        ///// A function that delete bus on trip (mark the flag IsDeleted = true) 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="licenseNumber"></param>
        //public void DeleteBusOnTrip(int id, int licenseNumber)
        //{
        //    DO.BusOnTrip b = DataSource.ListBusesOnTrip.Find(p => p.Id == id && p.LicenseNumber == licenseNumber && p.IsDeleted);

        //    if (b != null)
        //    {
        //        //DataSource.ListBusesOnTrip.Remove(b);
        //        b.IsDeleted = true;
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion
        #endregion

        #region LineTrip

        #region lineTripDoBoAdapter
        /// <summary>
        /// A function that copy details from DO to BO
        /// </summary>
        /// <param name="busDO"></param>
        /// <returns></returns>
        BO.LineTrip lineTripDoBoAdapter(DO.LineTrip lineTripDO)
        {
            BO.LineTrip lineTripBO = new BO.LineTrip();
            int tripId = lineTripBO.Id;
            int lineId = lineTripBO.LineId;

            lineTripDO.CopyPropertiesTo(lineTripBO);

            List<BO.StationInLine> stations = (from stat in dl.GetAllLinesStationBy(stat => stat.LineId == lineId && stat.IsDeleted == false)
                                               let station = dl.GetStation(stat.StationCode)
                                               select station.CopyToStationInLine(stat)).ToList();
            stations = (stations.OrderBy(s => s.LineStationIndex)).ToList();
            //foreach (BO.StationInLine s in stations)
            //{
            //    if (s.LineStationIndex != stations[stations.Count - 1].LineStationIndex) // if this is not the end
            //    {
            //        int sc1 = s.StationCode;//station code 1
            //        int sc2 = stations[s.LineStationIndex].StationCode;//station code 2
            //        DO.AdjacentStations adjStat = dl.GetAdjacentStations(sc1, sc2);
            //        s.DistanceTo = adjStat.Distance;
            //        s.TimeTo = adjStat.TravelTime;
            //    }
            //}
            lineTripBO.ListOfStationsInLine = stations;
            return lineTripBO;

        }
        #endregion

        #region GetLineTrip
        /// <summary>
        /// A function that return a line trip
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lineId"></param>
        /// <returns></returns>
        public BO.LineTrip GetLineTrip(int id, int lineId)
        {
            DO.LineTrip lineTripDO;
            try
            {
                lineTripDO = dl.GetLineTrip(id,lineId);
            }

            catch (DO.IncorrectInputException ex)
            {
                throw new BO.IncorrectInputException(ex.Message);
            }

            return lineTripDoBoAdapter(lineTripDO);
        }
        #endregion

        #region GetAllLinesTrip
        /// <summary>
        /// A BO function that return all the lines trip
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.LineTrip> GetAllLinesTrip()
        {
            return from linetrip in dl.GetAllLinesTrip()
                   select lineTripDoBoAdapter(linetrip);
        }
        #endregion

        ////////////////////////////////////////////


        //#region GetAllLinesTripBy
        ///// <summary>
        ///// A function that returns the lines trip that have the special thing that the predicat do
        ///// </summary>
        ///// <param name="predicate"></param>
        ///// <returns></returns>
        //public IEnumerable<DO.LineTrip> GetAllLinesTripBy(Predicate<DO.LineTrip> predicate)
        //{
        //    return from trip in DataSource.ListLinesTrip
        //           where predicate(trip)
        //           select trip.Clone();
        //}
        //#endregion

        //#region AddLineTrip
        ///// <summary>
        ///// A function that add a line trip to the list
        ///// </summary>
        ///// <param name="lt"></param>
        //public void AddLineTrip(BO.LineTrip lt)
        //{
        //    DO.LineTrip linetripDOtemp;
        //    try
        //    {
        //        linetripDOtemp = dl.GetLineTrip(lt.Id,lt.LineId);
        //    }
        //    catch (DO.IncorrectLicenseNumberException ex)
        //    {
        //        throw new BO.IncorrectLicenseNumberException(ex.licenseNumber, ex.Message);
        //    }
        //    if (DataSource.ListLinesTrip.FirstOrDefault(p => p.Id == lt.Id && p.LineId == lt.LineId && p.IsDeleted) != null)
        //        throw new Exception();
        //    DataSource.ListLinesTrip.Add(lt.Clone());
        //}
        //#endregion

        //#region UpdateLineTrip
        ///// <summary>
        ///// A function that update the line trip
        ///// </summary>
        ///// <param name="bus"></param>
        //public void UpdateLineTrip(DO.LineTrip bus)
        //{

        //    DO.LineTrip b = DataSource.ListLinesTrip.Find(p => p.Id == bus.Id && p.LineId == bus.LineId && p.IsDeleted);

        //    if (b != null)
        //    {
        //        DataSource.ListLinesTrip.Remove(b);
        //        DataSource.ListLinesTrip.Add(bus.Clone());
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion

        //#region UpdateLineTrip
        ///// <summary>
        ///// method that knows to updt specific fields in LineTrip
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="lineId"></param>
        ///// <param name="update"></param>
        //public void UpdateLineTrip(int id, int lineId, Action<DO.LineTrip> update)
        //{

        //    DO.LineTrip b = DataSource.ListLinesTrip.Find(p => p.Id == id && p.LineId == lineId && p.IsDeleted == false);
        //    if (b == null)
        //        throw new Exception();
        //    update(b);
        //}
        //#endregion

        //#region DeleteLineTrip
        ///// <summary>
        ///// A function that delete line trip (mark the flag IsDeleted = true) 
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="lineId"></param>
        //public void DeleteLineTrip(int id, int lineId)
        //{
        //    DO.LineTrip b = DataSource.ListLinesTrip.Find(p => p.Id == id && p.LineId == lineId && p.IsDeleted);

        //    if (b != null)
        //    {
        //        //DataSource.ListLinesTrip.Remove(b);
        //        b.IsDeleted = true;
        //    }
        //    else
        //        throw new Exception();
        //}
        //#endregion
        #endregion



    }
}


