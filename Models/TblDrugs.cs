﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace WpfApp6.Models;

[Table("tblDrugs")]
[Index("DoseMg", "DrugName", Name = "NonClusteredIndex-20241023-105100", IsUnique = true)]
public partial class TblDrugs
{
    [Key]
    public int DrugsId { get; set; }

    [Column("Dose_MG", TypeName = "decimal(10, 4)")]
    public decimal DoseMg { get; set; }

    [Required]
    [StringLength(200)]
    public string DrugName { get; set; }

    [ForeignKey("DrugId")]
    [InverseProperty("Drug")]
    public virtual ICollection<TblTreatment> Treatment { get; set; } = new List<TblTreatment>();
}