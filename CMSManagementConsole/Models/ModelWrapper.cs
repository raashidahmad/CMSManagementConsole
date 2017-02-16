using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CMSManagementConsole.Models
    {
    public class Category
        {
        public int Id { get; set; }
        public string Name { get; set; }
        }

    public class District
        {
        public int Id { get; set; }
        public string Name { get; set; }
        }

    public class Role
        {
        public string Id { get; set; }
        public string Name { get; set; }
        }

    public class SDC
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DistrictId { get; set; }
        }

    public class SDCView
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public int DistrictId { get; set; }
        public string District { get; set; }
        }

    public class ComplaintView
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }

        public int Id { get; set; }
        public int ComplainantId { get; set; }
        public string Complainant { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public int DistrictId { get; set; }
        public string District { get; set; }
        public string SDCId { get; set; }
        public string SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<string> Documents { get; set; }
        }

    public class NewComplaint
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }
        [Display(Name= "Complainant")]
        public int ComplainantId { get; set; }
        [Display(Name= "Category")]
        public int CategoryId { get; set; }
        [Display(Name= "SDC")]
        public int SDCId { get; set; }
        public string Description { get; set; }
        public List<string> Documents { get; set; }
        }

    public class ComplainantList
        {
        public string NIC { get; set; }
        public string FullName { get; set; }
        }

    public class UserView
        {
        public string Username { get; set; }
        public string FullName { get; set; }
        public int DistrictId { get; set; }
        public string District { get; set; }
        public int SDCId { get; set; }
        public string SDC { get; set; }
        public List<string> Roles { get; set; }
        }

    public class CreateUserBindingModel
        {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int SDCId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        }

    public class FilterIds
        {
        public int Id { get; set; }
        }
    }