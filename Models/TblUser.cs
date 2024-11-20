﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using IndexAttribute = Microsoft.EntityFrameworkCore.IndexAttribute;

namespace WpfApp6.Models;

[Table("tblUser")]
[Index("UserEmail", Name = "NonClusteredIndex-20240919-153638", IsUnique = true)]
public partial class TblUser
{
    [Key]
    public int UserId { get; set; }

    public byte UserType { get; set; }

    [Required]
    [StringLength(200)]
    [Unicode(false)]
    public string UserEmail { get; set; }

    [Required]
    [StringLength(64)]
    [Unicode(false)]
    public string UserPw { get; set; }

    [Required]
    [Column("RegisterIP")]
    [StringLength(16)]
    [Unicode(false)]
    public string RegisterIp { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime RegisterDate { get; set; }

    [Required]
    [StringLength(40)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(40)]
    public string LastName { get; set; }

    [Required]
    [StringLength(36)]
    public string SaltOfPw { get; set; }

    [ForeignKey("UserType")]
    [InverseProperty("TblUser")]
    public virtual TblUserType UserTypeNavigation { get; set; }
}