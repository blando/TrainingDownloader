using System;
using System.Collections.Generic;

namespace TrainingDownloader.models.entities
{
    public partial class IsuVendorCourses
    {
        public int Id { get; set; }
        public string LwCourseId { get; set; }
        public string VendorCourseId { get; set; }
        public string TlCourseId { get; set; }
        public string VendorCourseName { get; set; }
        public string VendorCourseGroup { get; set; }
        public string TlCourseIdParallel { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string TlCourseType { get; set; }
        public string TlCourseTypeParallel { get; set; }
        public string DependsOn { get; set; }
        public byte? Source { get; set; }
    }
}
