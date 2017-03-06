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
        [Required, MinLength(5)]
        public string Name { get; set; }
        }

    public class SDC
        {
        public int Id { get; set; }
        [Required, MinLength(5)]
        public string Title { get; set; }
        [Required]
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
        public string Complainant { get; set; }
        public string NIC { get; set; }
        public string Mobile { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<string> Documents { get; set; }
        }

    public class ComplaintFullView
        {
        public enum ComplaintStatus : int
            {
            InQueue = 1,
            UnderProcess = 2,
            Completed = 3,
            Closed = 4
            }

        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        public int Id { get; set; }
        public int ComplainantId { get; set; }
        public string Complainant { get; set; }
        public string ComplainantNIC { get; set; }
        public string ComplainantMobile { get; set; }
        public string ComplainantAddress { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        public string Category { get; set; }
        public string District { get; set; }
        public string SDC { get; set; }
        public string Description { get; set; }
        public DateTime Dated { get; set; }
        public ComplaintStatus Status { get; set; }
        public List<ViewDataUploadFilesResult> Documents { get; set; }
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
        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        [Display(Name= "Complainant")]
        public int ComplainantId { get; set; }
        [Required]
        [Display(Name= "Category")]
        public int CategoryId { get; set; }
        public int DistrictId { get; set; }
        public int SDCId { get; set; }
        [Required]
        public string Description { get; set; }
        [MinLength(5)]
        public string FullName { get; set; }
        [RegularExpression("^\\d{14}", ErrorMessage = "Enter valid NIC Number in the format (0000000000000)")]
        public string NIC { get; set; }
        [RegularExpression("^\\d{4}-\\d{7}", ErrorMessage="Enter valid Mobile Number in the format (0000-0000000)")]
        public string Mobile { get; set; }
        public string Address { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        public List<string> Documents { get; set; }
        }

    public class ComplainantList
        {
        public int Id { get; set; }
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

    public class Complainant
        {
        public enum CommunicationMedium : int
            {
            SMS = 1,
            Phone = 2
            }

        public int Id { get; set; }
        public string FullName { get; set; }
        public string NIC { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public CommunicationMedium ContactMedium { get; set; }
        }

    public class FilterIds
        {
        public int Id { get; set; }
        }

    //Properties starting with small letters because of its dependency on a library
    public class ViewDataUploadFilesResult
        {
        public string name { get; set; }
        public int size { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public string deleteUrl { get; set; }
        public string thumbnailUrl { get; set; }
        public string deleteType { get; set; }
        }
    }