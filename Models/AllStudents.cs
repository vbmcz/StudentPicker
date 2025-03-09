using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StudentPicker.Models
{
	public class AllStudents
	{
		public enum FileDataTypes : int
		{
			StudentId = 0,
			StudentName = 1,
			StudentClassId = 2,
			StudentIsPresent = 3,
			StudentAskCooldown = 4,
			StudentInClassNumber = 5
		}

		public static readonly string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "students.txt");
		public static ObservableCollection<Student> Students { get; set; } = [];

		public static void LoadAllStudents()
		{
			string[] data = TryReadFile(dataFilePath);
			if (data == null)
				return;

			Students.Clear();


			foreach(string item in data)
			{
				var temp = item.Split(';');
				var isNum = int.TryParse(temp[(int)FileDataTypes.StudentId], out int id);

				if (isNum)
					Students.Add(new Student(id, 
						temp[(int)FileDataTypes.StudentName],
						temp[(int)FileDataTypes.StudentClassId],
						bool.Parse(temp[(int)FileDataTypes.StudentIsPresent]),
						int.Parse(temp[(int)FileDataTypes.StudentAskCooldown]),
						int.Parse(temp[(int)FileDataTypes.StudentInClassNumber])));	
			}
		}

		public static void LoadStudentsByClass(string classId)
		{
			string[] data = TryReadFile(dataFilePath);
			if (data == null)
				return;


			string[] temp = [];
			for (int i = 0; i < data.Length; i++)
			{
				temp = data[i].Split(';');
				var isNum = int.TryParse(temp[(int)FileDataTypes.StudentId], out int num);
				if (isNum && temp[(int)FileDataTypes.StudentClassId].ToLower().Equals(classId.ToLower()))
					Students.Add(new Student(num, 
						temp[(int)FileDataTypes.StudentName],
						temp[(int)FileDataTypes.StudentClassId],
						bool.Parse(temp[(int)FileDataTypes.StudentIsPresent]),
						int.Parse(temp[(int)FileDataTypes.StudentAskCooldown]),
						int.Parse(temp[(int)FileDataTypes.StudentInClassNumber])));	
			}
		}

		public static void WriteStudentsToFile(ObservableCollection<Student> students, string classId)
		{
			string[] data = TryReadFile(dataFilePath);
			if (data.Length == 0)
				return;
			

			List<Student> stdTemp = [];
			string[] dataTemp = [];

			for (int i = 0; i < data.Length; i++)
			{
				dataTemp = data[i].Split(';');
				if (dataTemp.Length == 1)
					break;
				if (dataTemp[2].ToLower().Equals(classId.ToLower()))
				{
					data[i] = "";
					stdTemp.Add(ConvertToStudent(string.Join("", dataTemp)));
				}
			}

			foreach (Student student in stdTemp)
			{
				foreach (Student student2 in students)
				{
					if (student2.Id == student.Id)
					{
						student.AskCooldown = student2.AskCooldown;
						student.IsPresent = student2.IsPresent;
					}
				}
			}

			File.Delete(dataFilePath);

			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == "")
					continue;
				File.AppendAllText(dataFilePath, data[i] + '\n');
			}

			foreach (Student student in stdTemp)
			{
				File.AppendAllText(dataFilePath, student.ToString());
			}
		}

		public static string[] TryReadFile(string filePath)
		{
			string[] data = [];
			try
			{
				data = File.ReadAllText(filePath).Split('\n');
			}
			catch (Exception)
			{
				return [];
			}
			return data;
		}

		public static Student ConvertToStudent(string arg)
		{
			string[] args = arg.Split("");
			bool temp = int.TryParse(args[0], out int id);
			if(temp)
				return new Student(id, args[1], args[2], bool.Parse(args[3].ToLower()), int.Parse(args[4]), int.Parse(args[5]));
			return null;
		}
	}
}
