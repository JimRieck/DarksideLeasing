﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Darkside.LeasingCalc.Data.Models;

public partial class DailyMilageDetail
{
    public Guid Id { get; set; }

    public DateTime? MilageDate { get; set; }

    public int? ExpectedMileDriven { get; set; }

    public int? MilesDriven { get; set; }

    public int? ExpectedMilage { get; set; }

    public int? ActualMileage { get; set; }

    public int? MilageDifference { get; set; }

    public Guid CarLeaseId { get; set; }

    public bool IsDeleted { get; set; }

    public string CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }
}