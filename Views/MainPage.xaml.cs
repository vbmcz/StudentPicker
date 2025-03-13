using StudentPicker.Models;
using CommunityToolkit.Maui.Views;
using StudentPicker.Views;

namespace StudentPicker
{
	public partial class MainPage : ContentPage
	{
		private int luckyNumber = 0;
		public MainPage()
		{
			InitializeComponent();
			AllStudents.LoadAllStudents();
			int maxClassNumber = 0;
			foreach (Student std in AllStudents.Students)
				if (std.InClassNumber > maxClassNumber)
					maxClassNumber = std.InClassNumber;

			luckyNumber = new Random().Next(maxClassNumber) + 1;

			LuckyNumberLabel.Text = $"Lucky number: {luckyNumber}";
			AllClasses.LoadClasses();
			ClassPicker.ItemsSource = AllClasses.Classes;
        }



		private async void DrawButtonClicked(object sender, EventArgs e)
		{
			StudentList.ItemsSource = null;
			if (ClassPicker.SelectedItem == null)
			{
				await DisplayAlert("Warning", "Pick class to draw from!", "OK");
				return;
			}

			string? classId = ClassPicker.SelectedItem.ToString();
			AllStudents.LoadStudentsByClass(classId);

			if(AllStudents.Students.Count <= 0)
			{
				await DisplayAlert("Warning", "No students found in this class!", "OK");
				return;
			}

			int maxClassNumber = AllStudents.Students.ElementAt(AllStudents.Students.Count - 1).InClassNumber;
			List<Student> students = [];
			/*TODO: DO SMTH WHEN LESS THAN 4 STUDENTS*/
			foreach(Student std in AllStudents.Students)
			{
				if (std.AskCooldown == 0 && std.IsPresent && std.InClassNumber != luckyNumber)
					students.Add(std);
			}


			int rand = 0;
			rand = new Random().Next(maxClassNumber) + 1;
			Student selectedStudent = AllStudents.Students.First(s => s.InClassNumber == rand);

			if (!selectedStudent.IsPresent)
			{
				await DisplayAlert("Warning", $"Drawn student: {selectedStudent.Name} isn't present!", "OK");
				return;
			}
			if (selectedStudent.InClassNumber == luckyNumber)
			{
				await DisplayAlert("Warning", $"Drawn student: {selectedStudent.Name} has the lucky number!", "OK");
				return;
			}
			if (selectedStudent.AskCooldown > 0)
			{
				await DisplayAlert("Warning", $"Drawn student: {selectedStudent.Name}, is out of the ask pool, remaining students: {selectedStudent.AskCooldown}", "OK");
				return;
			}

			StudentToBeAsked.Text = $"Drawn student: {selectedStudent.Name}";

			foreach (Student student in AllStudents.Students)
			{
				if(student.Id == selectedStudent.Id)
				{
					student.AskCooldown = 3;
					continue;
				}
				if (student.AskCooldown > 0)
					student.AskCooldown--;
			}

			AllStudents.WriteStudentsToFile(AllStudents.Students, classId);
		}

		private void ClassPickerChanged(object sender, EventArgs e)
		{
			var picker = (Picker)sender;
			int selectedIdx = picker.SelectedIndex;
			string classId = "";

			if (selectedIdx != -1)
				classId = (string)picker.SelectedItem;
			AllStudents.Students.Clear();
			AllStudents.LoadStudentsByClass(classId);
			StudentList.ItemsSource = null;
		}

		private async void ShowStudentsClicked(object sender, EventArgs e)
		{
			if (ClassPicker.SelectedItem == null)
			{
				await DisplayAlert("Warning", "Choose class!", "OK");
				return;
			}
			AllStudents.Students.Clear();
			AllStudents.LoadStudentsByClass(ClassPicker.SelectedItem.ToString());
			StudentList.ItemsSource = AllStudents.Students;
		}

		private async void AddStudentClicked(object sender, EventArgs e)
		{
			if (StudentName.Text == null || ClassId.Text == null)
			{
				await DisplayAlert("Warning", "Enter valid data!", "OK");
				return;
			}
			string studentName = StudentName.Text.ToString(),
				classId = ClassId.Text.ToString();
			int lastId = 0, inClassNumber = 0;
			AllStudents.LoadAllStudents();
			if(AllStudents.Students.Count == 0)
			{
				lastId = 1;
			}
			else
			{
				int max = 0;
				foreach (Student sdt in AllStudents.Students)
				{
					if (sdt.Id > max)
						max = sdt.Id;
					lastId = max + 1;
				}	
			}

			try
			{
				var listCount = AllStudents.Students.Where(s => s.ClassId == classId)
					.ToList().Count;
				inClassNumber = AllStudents.Students.Where(s => s.ClassId == classId)
					.ToList().ElementAt(listCount - 1).InClassNumber + 1;
			}
			catch (Exception)
			{
				inClassNumber = 1;
			}

			Student student = new(lastId, studentName, classId, true, 0, inClassNumber);
			AllStudents.Students.Add(student);
			File.AppendAllText(AllStudents.dataFilePath, student.ToString());
			AllStudents.Students.Clear();
			AllStudents.LoadStudentsByClass(classId);

			StudentList.ItemsSource = AllStudents.Students;
		}

		private void RefreshClassesClicked(object sender, EventArgs e)
		{
			AllClasses.LoadClasses();
			ClassPicker.ItemsSource = AllClasses.Classes;
		}

		private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			AllStudents.editStudent = e.SelectedItem as Student;

			this.ShowPopup(new EditPopup());
		}
	}
}
