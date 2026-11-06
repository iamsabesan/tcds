using Dawn.Utilities.i18N;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDTO.Batch
{
    public class CreateBatch
    {
        public Guid CourseID { get; set; }
        public string BatchType { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public string BatchTrack { get; set; } = string.Empty;
    }

    public class BatchList
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public Guid CourseID { get; set; }
        public string BatchType { get; set; } = string.Empty;
        public string Timing { get; set; } = string.Empty;
        public string BatchTrack { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class NewBatchList
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
        public string Timing { get; set; } = string.Empty;
        public string BatchTrack { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string ShortName { get; set; } = string.Empty;
    }

    public class BatchByID
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
        public string Timing { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
    }

    public class BatchInfo
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public Guid CourseID { get; set; }
        public Guid FacultyID { get; set; }
        public string Timing { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
    }

    public class NewBatchInfo
    {
        public Guid CourseBatchID { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string Timing { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseTypeName { get; set; } = string.Empty;
    }

    public class BatchByCourseID
    {
        public string BatchNumber { get; set; } = string.Empty;
        public Guid CourseBatchID { get; set; }
        public string Timing { get; set; } = string.Empty;
        public Guid FacultyID { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string BatchType { get; set; } = string.Empty;
    }
    public class BatchMappingID
    {
        public Guid StudentBatchMapping { get; set; }
        public Guid ExamIndentTransID { get; set; }
    }
}
