



namespace Borg.Client.Areas.Backoffice.Domain
{
    public class ToDo
    {
        protected ToDo() { }

        public ToDo(string title, bool done = false)
        {
            Title = title;
            Done = done;
        }

        public void Complete()
        {
            if (!Done) Done = true;
        }
        public int Id { get; protected set; }
        public bool Done { get; protected set; } = false;
        public string Title { get; protected set; }
    }

    //public class ToDoConfig : EntityTypeConfiguration<ToDo>
    //{
    //    public ToDoConfig()
    //    {
    //        HasKey(x => x.Id);
    //    }
    //}
}
