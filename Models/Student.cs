namespace StudentPicker.Models
{
	public class Student(int id, string name, string classId, bool isPresent, int askCooldown, int inClassNumber)
	{
		public int Id { get; set; } = id;
		public string Name { get; set; } = name;
		public string ClassId { get; set; } = classId;
		public bool IsPresent { get; set; } = isPresent;
		public int AskCooldown { get; set; } = askCooldown;
		public int InClassNumber { get; set; } = inClassNumber;

		override public string ToString() => $"{Id};{Name};{ClassId};{IsPresent};{AskCooldown};{InClassNumber}\n";
	}
}
