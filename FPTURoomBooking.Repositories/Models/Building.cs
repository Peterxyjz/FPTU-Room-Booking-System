using System;
using System.Collections.Generic;

namespace FPTURoomBooking.Repositories.Models;

public partial class Building
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Floors { get; set; }

    public string? Description { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
