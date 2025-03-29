using System;
using System.Collections.Generic;

namespace FPTURoomBooking.Repositories.Models;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Capacity { get; set; }

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int BuildingId { get; set; }

    public int Floor { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Building Building { get; set; } = null!;

    public virtual ICollection<RoomEquipment> RoomEquipments { get; set; } = new List<RoomEquipment>();
}
