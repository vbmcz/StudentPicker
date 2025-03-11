using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPicker.Models
{
	public class AllClasses
	{
		public static ObservableCollection<string> Classes = [];

		public static void LoadClasses()
		{
			Classes.Clear();
			string[] data = [];
			try
			{
				data = File.ReadAllText(AllStudents.dataFilePath).Split('\n');
			}
			catch (Exception)
			{
				return;
			}

			for (int i = 0; i < data.Length; i++)
			{
				string[] temp = data[i].Split(';');
				if (temp.Length < 6)
					break;
				if (Classes.Contains(temp[2]))
					continue;
				Classes.Add(temp[2]);
			}
		}
	}
}
