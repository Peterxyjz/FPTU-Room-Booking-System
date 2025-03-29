using System;
using System.Collections.Generic;

namespace FPTURoomBooking.Repositories.Models;

public partial class RoomEquipment
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public int EquipmentId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? LastMaintenance { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
