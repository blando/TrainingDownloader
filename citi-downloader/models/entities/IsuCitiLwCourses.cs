using System;
using System.Collections.Generic;

namespace CitiDownloader.models.entities
{
    public partial class IsuCitiLwCourses
    {
        public int Id { get; set; }
        public string LwCourseId { get; set; }
        public string CitiCourseId { get; set; }
        public string TlCourseId { get; set; }
        public string CitiCourseName { get; set; }
        public string CitiCourseGroup { get; set; }
        public string TlCourseIdParallel { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public string TlCourseType { get; set; }
        public string TlCourseTypeParallel { get; set; }
        public string DependsOn { get; set; }
        public byte? Source { get; set; }
    }
}
