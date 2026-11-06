namespace EntitiesDTO.Subject
{
    public class CreateSubject
    {
        public string SubjectName { get; set; } = string.Empty;
        public List<GetMaterial> SubjectMaterial { get; set; } = new List<GetMaterial>();
    }

    public class Material
    {
        public Guid MaterialID { get; set; }
        public string MaterialName { get; set; } = string.Empty;
    }

    public class GetMaterial
    {
        public Guid MaterialID { get; set; }
    }

    public class SubjectResult
    {
        public Guid SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
    }
    public class MaterialResult
    {
        public Guid SubjectID { get; set; }
        public Guid MaterialID { get; set; }
    }
    public class GetSubjectList
    {
        public Guid SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<Material> Materials { get; set; } = new List<Material>();
    }

    public class SubjectList
    {
        public Guid SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public Guid MaterialID { get; set; }
    }

    public class EditSubject
    {
        public Guid SubjectID { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public List<GetMaterial> SubjectMaterial { get; set; } = new List<GetMaterial>();
    }

    public class Model
    {
        public List<GetMaterial> SubjectMaterial { get; set; } = new List<GetMaterial>();
    }

    public class AddSubVideoMap
    {
        public Guid SubjectID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class GetSubjectVideoList
    {
        public Guid SubVideoMappingID { get; set; }
        public Guid SubjectID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string VideoType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    public class DeleteVideo
    {
        public Guid SubVideoMappingID { get; set; }
    }
    public class GetCourse
    {
        public Guid CourseID { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class GetSubjectVideo
    {
        public Guid SubjectID { get; set; }
        public string SubVideoMappingID { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string SortOrder { get; set; } = string.Empty;
        public string VideoType { get; set; } = string.Empty;
        public string ContentUrl { get; set; } = string.Empty;
    }

    public class SubjectsOfCourse {
        public Guid SubjectID { get; set; }
        public  int Semester { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int TheoryMarks { get; set; } 
        public int PracticalMarks { get; set; } 
        public int TotalMarks { get; set; } 


    }
}
