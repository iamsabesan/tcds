using Dawn.Utilities.i18N;
using Newtonsoft.Json;

namespace EntitiesDTO.News
{
    public class CreateNews
    {
        public string Title { get; set; } = "";
        public string NewsContent { get; set; } = "";
        public bool ShowOnStartUp { get; set; }
    }

    public class EditNews
    {
        public Guid NewsListID { get; set; }
        public string Title { get; set; } = "";
        public string NewsContent { get; set; } = "";
        public bool ShowOnStartUp { get; set; }
    }

    public class GetNewsList
    {
        public Guid NewsListID { get; set; }
        [JsonIgnore]
        public DateI18 DateI18 { get; set; } = new DateI18();
        public string Date { get { return DateI18; } set { DateI18 = value; } }
        public string Title { get; set; } = "";
        public string NewsContent { get; set; } = "";
        public bool ShowOnStartUp { get; set; }
    }

    public class NewsByFranchisee
    {
        public Guid NewsListID { get; set; }
        public string Title { get; set; } = "";
        public string NewsContent { get; set; } = "";
    }
}
