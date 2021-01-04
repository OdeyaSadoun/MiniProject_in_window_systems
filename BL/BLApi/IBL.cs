﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL;
using BO;

namespace BLApi
{
    public interface IBL
    {
        #region Bus
        IEnumerable<BO.Bus> GetAllBuses(); //V
        IEnumerable<BO.Bus> GetAllBusesBy(Predicate<BO.Bus> predicate);
        BO.Bus GetBus(int id);//V
        void AddBus(BO.Bus bus);//V
        void UpdateBus(BO.Bus bus);//V

        /// <summary>
        /// method that knows to updt specific fields in Bus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        void UpdateBus(int id, Action<BO.Bus> update);
        void DeleteBus(int id);//V
        #endregion

        #region BusOnTrip
        IEnumerable<BO.BusOnTrip> GetAllBusesOnTrip();
        IEnumerable<BO.BusOnTrip> GetAllBusesOnTripBy(Predicate<BO.BusOnTrip> predicate);
        BO.BusOnTrip GetBusOnTrip(int id);
        void AddBusOnTrip(BO.BusOnTrip bus);
        void UpdateBusOnTrip(BO.BusOnTrip bus);
        void UpdateBusOnTrip(int id, Action<BO.BusOnTrip> update); //method that knows to updt specific fields in BusOnTrip
        void DeleteBusOnTrip(int id);
        #endregion

        #region LineTrip
        IEnumerable<BO.LineTrip> GetAllLinesTrip();
        IEnumerable<BO.BusOnTrip> GetAllLinesTripBy(Predicate<BO.LineTrip> predicate);
        BO.LineTrip GetLineTrip(int id);
        void AddLineTrip(BO.LineTrip bus);
        void UpdateLineTrip(BO.LineTrip bus);
        void UpdateLineTrip(int id, Action<BO.LineTrip> update); //method that knows to updt specific fields in LineTrip
        void DeleteLineTrip(int id);
        #endregion

        #region Station
        IEnumerable<BO.Station> GetAllStationes();
        IEnumerable<BO.Station> GetAllStationesBy(Predicate<BO.Station> predicate);
        BO.Station GetStation(int id);
        void AddStation(BO.Station station);
        void UpdateStation(BO.Station station);
        void UpdateStation(int id, Action<BO.Station> update); //method that knows to updt specific fields in station
        void DeleteStation(int id);
        #endregion

        #region Line
        IEnumerable<BO.Line> GetAllLines();
        IEnumerable<BO.Line> GetAllLinesBy(Predicate<DO.Line> predicate);
        BO.Line GetLine(int id);
        void AddLine(BO.Line line);
        void UpdateLine(BO.Line line);
        void UpdateLine(int id, Action<BO.Line> update); //method that knows to updt specific fields in Line
        void DeleteLine(int id);
        #endregion

        #region LineStation
        IEnumerable<BO.LineStation> GetAllLinesStation();
        IEnumerable<BO.LineStation> GetAllLinesStationBy(Predicate<DO.LineStation> predicate);
        BO.LineStation GetLineStation(int id);
        void AddLineStation(BO.LineStation lineStation);
        void UpdateLineStation(BO.LineStation lineStation);
        void UpdateLineStation(int id, Action<BO.LineStation> update); //method that knows to updt specific fields in LineStation
        void DeleteLineStation(int id);
        #endregion

        #region User
        IEnumerable<BO.User> GetAllUsers();
        IEnumerable<BO.User> GetAllUsersBy(Predicate<BO.User> predicate);
        BO.User GetUser(int id);
        void AddUser(BO.User user);
        void UpdateUser(BO.User user);
        void UpdateUser(int id, Action<BO.User> update); //method that knows to updt specific fields in User
        void DeleteUser(int id);
        #endregion

        #region AdjacentStations
        IEnumerable<BO.AdjacentStations> GetAllAdjacentStations();
        IEnumerable<BO.AdjacentStations> GetAllAdjacentStationsBy(Predicate<BO.AdjacentStations> predicate);
        BO.AdjacentStations GetAdjacentStationsr(int id);
        void AddAdjacentStations(BO.AdjacentStations adjacentStations);
        void UpdateAdjacentStations(BO.AdjacentStations adjacentStations);
        void UpdateAdjacentStations(int id, Action<BO.AdjacentStations> update); //method that knows to updt specific fields in AdjacentStations
        void DeleteAdjacentStations(int id);
        #endregion

        #region Trip
        IEnumerable<BO.Trip> GetAllTrips();
        IEnumerable<BO.Trip> GetAllTripsBy(Predicate<BO.Trip> predicate);
        BO.Trip GetTrip(int id);
        void AddTrip(BO.Trip trip);
        void UpdateTrip(BO.Trip trip);
        void UpdateTrip(int id, Action<BO.Trip> update); //method that knows to updt specific fields in Trip
        void DeleteTrip(int id);
        #endregion
    }
}