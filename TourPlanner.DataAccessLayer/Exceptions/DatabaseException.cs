﻿using System;

namespace TourPlanner.DataAccessLayer.Exceptions
{
    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message)
        {
        }
    }
}
