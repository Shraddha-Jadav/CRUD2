//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CRUD2.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class user
    {
        public int userId { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public Nullable<System.DateTime> dob { get; set; }
        [Required]
        public Nullable<bool> gender { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string address { get; set; }
        [Required]
        public string hobbies { get; set; }
        [Required]
        public string profile_photo { get; set; }
        [Required]
        public string resume { get; set; }
    }

    public enum City
    {
        Ahemdabad,
        Junagadh,
        Rajkot,
        Surat
    }
}
