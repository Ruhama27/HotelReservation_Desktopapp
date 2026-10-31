using System;
using System.Collections.Generic;

namespace HotelReservation
{
    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        public string GuestName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights => Math.Max(1, (CheckOut.Date - CheckIn.Date).Days);
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; } = "Paid";
        public List<string> Services { get; set; } = new List<string>();
    }

    public static class DataStore
    {
        // Category-specific room capacities (Managed by Admin)
        public static int StandardCapacity { get; set; } = 15;
        public static int SuitesCapacity { get; set; } = 15;
        public static int DeluxeCapacity { get; set; } = 10;
        public static int BeingCleanedCount { get; set; } = 4;

        public static int TotalRooms => StandardCapacity + SuitesCapacity + DeluxeCapacity;

        public static List<Reservation> Reservations { get; set; } = new List<Reservation>()
        {
            new Reservation
            {
                Id = "RES101",
                GuestName = "Ruhama Nigusu",
                PhoneNumber = "+251 91 123 4567",
                RoomType = "Standard Room",
                RoomNumber = "207",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                TotalPrice = 6400,
                PaymentMethod = "CBE",
                PaymentStatus = "Paid"
            },
            new Reservation
            {
                Id = "RES102",
                GuestName = "Kura",
                PhoneNumber = "+251 92 234 5678",
                RoomType = "Deluxe Room",
                RoomNumber = "117",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                TotalPrice = 4200,
                PaymentMethod = "Telebirr",
                PaymentStatus = "Pending"
            },
            new Reservation
            {
                Id = "RES103",
                GuestName = "Biniyam",
                PhoneNumber = "+251 93 345 6789",
                RoomType = "Executive Suite",
                RoomNumber = "90",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(3),
                TotalPrice = 21600,
                PaymentMethod = "CBE",
                PaymentStatus = "Pending"
            },
            new Reservation
            {
                Id = "RES104",
                GuestName = "Blen",
                PhoneNumber = "+251 94 456 7890",
                RoomType = "Standard Room",
                RoomNumber = "203",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                TotalPrice = 7300,
                PaymentMethod = "Others",
                PaymentStatus = "Paid"
            }
        };

        public static string AssignRandomRoomNumber(string roomType)
        {
            Random rnd = new Random();
            if (roomType.Contains("Suite")) return rnd.Next(301, 320).ToString();
            if (roomType.Contains("Deluxe")) return rnd.Next(201, 230).ToString();
            return rnd.Next(101, 150).ToString();
        }

        public static void AddFreeRooms(string roomType, int count)
        {
            if (roomType == "Standard") StandardCapacity += count;
            else if (roomType == "Suites") SuitesCapacity += count;
            else if (roomType == "Deluxe") DeluxeCapacity += count;
            else if (roomType == "Release Cleaned")
            {
                // Move cleaned rooms back into available capacity
                int release = Math.Min(BeingCleanedCount, count);
                BeingCleanedCount -= release;
                StandardCapacity += release;
            }
        }
    }
}