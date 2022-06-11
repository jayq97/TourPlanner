﻿using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.JsonClasses;
using TourPlanner.Models;

namespace TourPlanner.BusinessLayer
{
    public static class APIRequest
    {
        public static async Task<TourDistanceAndTime?> GetRequest(string from, string to, string transportType)
        {
            HttpClient client = new();

            var apikey = ConfigurationManager.AppSettings["mapquestapikey"];

            string transportTypeOnUrl;
            switch (transportType)
            {
                case "Car":
                    transportTypeOnUrl = "fastest"; 
                    break;
                case "Foot":
                    transportTypeOnUrl = "pedestrian"; 
                    break;
                case "Bicycle":
                    transportTypeOnUrl = "bicycle";
                    break;
                default:
                    transportTypeOnUrl = "fastest";

                    break;
            }

            HttpResponseMessage response = await client.GetAsync(
                $"http://www.mapquestapi.com/directions/v2/route?" +
                $"&key={apikey}" +
                $"&unit=k" +
                $"&routeType={transportTypeOnUrl}" +
                $"&from={from}" +
                $"&to={to}");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseBody);

            double distance = myDeserializedClass.route.distance;
            int time = myDeserializedClass.route.time;

            return new TourDistanceAndTime(distance, time);
        }
    }
}