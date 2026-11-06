using EntitiesDTO.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesDTO.Course
{
    public class CourseTypeList
    {
        public string CourseTypeID { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
    }

    public class CreateNewCourse
    {
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DurationType { get; set; } = string.Empty;
        public int TotalSemester { get; set; }
        public decimal CourseFees { get; set; }
        public decimal ExamFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public string Eligibility { get; set; } = string.Empty;
        public List<CourseSubjectMap> SubjectMapping { get; set; } = new List<CourseSubjectMap>();
    }

    public class CourseSubjectMap
    {
        public Guid SubjectID { get; set; }
        public Guid MaterialID { get; set; }
        public int Semester { get; set; }
        public int TheoryMark { get; set; }
        public int PracticalMark { get; set; }
    }

    public class CourseList
    {
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int Semester { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string DurationType { get; set; } = string.Empty;
        public string CertificatePrintName { get; set; } = string.Empty;
        public decimal CourseFees { get; set; }
        public decimal ExamFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public decimal TotalFees { get; set; }
        //Added on 19-05-2025
        public string Eligibility { get; set; } = string.Empty;
        //Added on 19-05-2025
        public List<CourseSubjectMapList> SubjectListDetails = new List<CourseSubjectMapList>();
    }


    public class GetCourseList
    {
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string DurationType { get; set; } = string.Empty;
        public string CertificatePrintName { get; set; } = string.Empty;
        public decimal CourseFees { get; set; }
        public decimal ExamFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public decimal TotalFees { get; set; }
        public int Semester { get; set; }
        //Added on 19-05-2025
        public string Eligibility { get; set; } = string.Empty;
        //Added on 19-05-2025
        public List<CourseSubjectMapList> SubjectMapping { get; set; } = new List<CourseSubjectMapList>();
    }

    public class CourseSubjectMapList
    {
        public Guid CourseID { get; set; }
        public Guid SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public byte Semester { get; set; }
        public byte TheoryMarks { get; set; }
        public byte PracticalMarks { get; set; }
        public int TotalMarks { get; set; }
    }

    public class CourseInfo
    {
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
    }

    public class CourseBaseInfo
    {
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
    }

    public class CourseByID
    {
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public decimal CourseFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public decimal FeesToPaid { get; set; }
    }

    public class CourseMapping
    {
        public Guid TenantBranchID { get; set; }
        public List<Courses> Courses { get; set; } = new List<Courses>();
    }

    public class Courses
    {
        public Guid CourseID { get; set; }
    }

    public class CourseMappingList
    {
        public Guid TenantBranchID { get; set; }
        public string FranchiseeID { get; set; } = string.Empty;
        public string FranchiseeName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public List<GetCourseMappingList> Courses { get; set; } = new List<GetCourseMappingList>();
    }

    public class GetCourseMappingList
    {
        public Guid TenantBranchID { get; set; }
        public Guid CourseID { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public int Semester { get; set; }
    }

    public class CourseSummaryByAdmin
    {
        public int CourseTotal { get; set; }
        public int PrintCertificate { get; set; }
        public int PrintMarksheet { get; set; }
        public int DespatchCertificate { get; set; }
        public int DespatchMarksheet { get; set; }
    }

    public class CourseSummaryByFranchisee
    {
        public int Courses { get; set; }
        public int AlumnusStudent { get; set; }
        public int CurrentStudent { get; set; }
        public int DiscontinuedStudent { get; set; }
        public int CompletedBatch { get; set; }
        public int ActiveBatch { get; set; }
        public int CertficatePending { get; set; }
        public int MarksheetPending { get; set; }
    }

    public class MblCourse
    {
        public Guid CourseID { get; set; }
        public string CourseTypeName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public decimal CourseFees { get; set; }
        public int TotalSemesters { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Eligibility { get; set; } = string.Empty;
        public List<SubjectsOfCourse> Subjects { get; set; } = new List<SubjectsOfCourse>();
    }

    public class EditCourse {
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
        public string CourseType { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string DurationType { get; set; } = string.Empty;
        public int TotalSemester { get; set; }
        public decimal CourseFees { get; set; }
        public decimal ExamFees { get; set; }
        public decimal AdmissionFees { get; set; }
        public string Eligibility { get; set; } = string.Empty;
    }
}
