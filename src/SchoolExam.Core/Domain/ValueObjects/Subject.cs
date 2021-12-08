namespace SchoolExam.Core.Domain.ValueObjects
{
    public class Subject
    {
        public string Name { get; set; }

        public Subject(string name)
        {
            Name = name;
        }

        public override bool Equals(object? obj)
        {
            return Name.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}