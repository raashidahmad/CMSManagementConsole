using System;
using System.Collections.Generic;
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
    }