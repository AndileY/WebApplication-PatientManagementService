﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WpfApp6.Models;

[Table("tblDisease")]
public partial class TblDisease
{
    [Key]
    public int DiseaseId { get; set; }

    [Required]
    [StringLength(200)]
    public string DiseaseName { get; set; }

    [Required]
    public string DiseaseDescription { get; set; }
}