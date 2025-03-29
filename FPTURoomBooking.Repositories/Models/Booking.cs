using System;
using System.Collections.Generic;

namespace FPTURoomBooking.Repositories.Models;

public partial class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int UserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string Purpose { get; set; } = null!;

    public string Status { get; set; } = null!;

    public bool IsRecurring { get; set; }

    public string? RecurringPattern { get; set; }

    public DateTime? CheckInTime { get; set; }

    public DateTime? CheckOutTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
