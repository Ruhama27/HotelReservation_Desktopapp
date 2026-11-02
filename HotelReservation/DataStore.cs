using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelReservation
{
    public enum RoomStatus
    {
        Available,
        Occupied,
        Cleaning
    }

    public class Room
    {
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public decimal NightlyRate { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;
    }

    public class Reservation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
        public string GuestName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights => Math.Max(1, (CheckOut.Date - CheckIn.Date).Days);
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionRef { get; set; } = "N/A";
        public string PaymentStatus { get; set; } = "Pending Verification"; // "Paid (Verified)", "Pending Verification", "Rejected (Unpaid)"
        public string StayStatus { get; set; } = "Active"; // "Active", "Departed"
    }

    public class HotelService
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public static class DataStore
    {
        public static List<Room> Rooms { get; set; } = new List<Room>()
        {
            new Room { RoomNumber = "101", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Available },
            new Room { RoomNumber = "102", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Available },
            new Room { RoomNumber = "103", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Occupied },
            new Room { RoomNumber = "104", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Available },
            new Room { RoomNumber = "105", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Cleaning },
            new Room { RoomNumber = "106", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Available },
            new Room { RoomNumber = "107", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Occupied },
            new Room { RoomNumber = "108", RoomType = "Standard Room", NightlyRate = 3200, Status = RoomStatus.Available },

            new Room { RoomNumber = "201", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Available },
            new Room { RoomNumber = "202", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Occupied },
            new Room { RoomNumber = "203", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Available },
            new Room { RoomNumber = "204", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Cleaning },
            new Room { RoomNumber = "205", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Available },
            new Room { RoomNumber = "206", RoomType = "Deluxe Room", NightlyRate = 4200, Status = RoomStatus.Available },

            new Room { RoomNumber = "301", RoomType = "Family Suite", NightlyRate = 7200, Status = RoomStatus.Available },
            new Room { RoomNumber = "302", RoomType = "Family Suite", NightlyRate = 7200, Status = RoomStatus.Occupied },
            new Room { RoomNumber = "303", RoomType = "Family Suite", NightlyRate = 7200, Status = RoomStatus.Available },
            new Room { RoomNumber = "304", RoomType = "Executive Suite", NightlyRate = 7200, Status = RoomStatus.Available },
            new Room { RoomNumber = "305", RoomType = "Executive Suite", NightlyRate = 7200, Status = RoomStatus.Cleaning },
            new Room { RoomNumber = "306", RoomType = "Executive Suite", NightlyRate = 7200, Status = RoomStatus.Available }
        };

        public static List<HotelService> Services { get; set; } = new List<HotelService>()
        {
            new HotelService { Name = "Restaurant meals", Price = 900m },
            new HotelService { Name = "Laundry", Price = 500m },
            new HotelService { Name = "Spa", Price = 1200m },
            new HotelService { Name = "Airport pickup", Price = 1000m },
            new HotelService { Name = "Gym", Price = 300m }
        };

        public static List<Reservation> Reservations { get; set; } = new List<Reservation>()
        {
            new Reservation
            {
                Id = "RES101",
                GuestName = "Ruhama Nigusu",
                PhoneNumber = "+251 91 123 4567",
                RoomType = "Standard Room",
                RoomNumber = "103",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(2),
                TotalPrice = 6400,
                PaymentMethod = "CBE",
                TransactionRef = "FT26305X89",
                PaymentStatus = "Paid (Verified)",
                StayStatus = "Active"
            },
            new Reservation
            {
                Id = "RES102",
                GuestName = "Kura",
                PhoneNumber = "+251 92 234 5678",
                RoomType = "Deluxe Room",
                RoomNumber = "202",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(1),
                TotalPrice = 4200,
                PaymentMethod = "Telebirr",
                TransactionRef = "TB8923410",
                PaymentStatus = "Pending Verification",
                StayStatus = "Active"
            },
            new Reservation
            {
                Id = "RES103",
                GuestName = "Biniyam",
                PhoneNumber = "+251 93 345 6789",
                RoomType = "Family Suite",
                RoomNumber = "302",
                CheckIn = DateTime.Today,
                CheckOut = DateTime.Today.AddDays(3),
                TotalPrice = 21600,
                PaymentMethod = "CBE",
                TransactionRef = "FT26309Y11",
                PaymentStatus = "Pending Verification",
                StayStatus = "Active"
            }
        };

        public static int TotalRooms => Rooms.Count;

        public static Room AllocateAvailableRoom(string requestedType)
        {
            var match = Rooms.FirstOrDefault(r => r.Status == RoomStatus.Available &&
                        (r.RoomType.IndexOf(requestedType, StringComparison.OrdinalIgnoreCase) >= 0 ||
                         requestedType.IndexOf(r.RoomType, StringComparison.OrdinalIgnoreCase) >= 0));

            if (match == null) match = Rooms.FirstOrDefault(r => r.Status == RoomStatus.Available);
            if (match != null) match.Status = RoomStatus.Occupied;
            return match;
        }

        public static void CheckOutGuest(string roomNumber)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room != null) room.Status = RoomStatus.Cleaning;

            var reservation = Reservations.FirstOrDefault(r => r.RoomNumber == roomNumber && r.StayStatus == "Active");
            if (reservation != null) reservation.StayStatus = "Departed";
        }

        public static void MakeRoomAvailable(string roomNumber)
        {
            var room = Rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
            if (room != null) room.Status = RoomStatus.Available;
        }
    }
}