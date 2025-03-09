﻿using StudentPicker.Models;

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
			foreach(Student std in AllStudents.Students)
				if (std.InClassNumber > maxClassNumber )
					maxClassNumber = std.InClassNumber;

			luckyNumber = new Random().Next(maxClassNumber) + 1;

			LuckyNumberLabel.Text = $"Szczęśliwy numerek: {luckyNumber}";
			AllClasses.LoadClasses();
			ClassPicker.ItemsSource = AllClasses.Classes;
		}

		private async void DrawButtonClicked(object sender, EventArgs e)
		{
			StudentList.ItemsSource = null;
			if (ClassPicker.SelectedItem == null)
			{
				await DisplayAlert("Uwaga", "Wybierz klasę!", "OK");
				return;
			}

			string? classId = ClassPicker.SelectedItem.ToString();
			AllStudents.LoadStudentsByClass(classId);

			if(AllStudents.Students.Count <= 0)
			{
				await DisplayAlert("Uwaga", "Klasa nie posiada uczniów!", "OK");
				return;
			}

			int	maxClassNumber = AllStudents.Students.ElementAt(AllStudents.Students.Count - 1).InClassNumber;
			if(maxClassNumber <= 1)
			{

			}

			int rand = 0;
			Random random = new();
			rand = random.Next(maxClassNumber) + 1;
			Student selectedStudent = AllStudents.Students.First(s => s.InClassNumber == rand);

			if (selectedStudent.IsPresent == false || selectedStudent.InClassNumber == luckyNumber || selectedStudent.AskCooldown > 0)
				DrawButtonClicked(sender, e);


			StudentToBeAsked.Text = $"Uczeń do pytania: {selectedStudent.Name}";

			/*UPDATES THE COOLDOWN*/
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
				await DisplayAlert("Uwaga", "Wybierz klasę!", "OK");
				return;
			}
			StudentList.ItemsSource = AllStudents.Students;
		}

		private async void AddStudentClicked(object sender, EventArgs e)
		{
			if (StudentName.Text == null || ClassId.Text == null)
			{
				await DisplayAlert("Uwaga", "Podaj poprawne dane!", "OK");
				return;
			}
			string studentName = StudentName.Text.ToString(),
				classId = ClassId.Text.ToString();
			int lastId = 0, inClassNumber = 0;
			AllStudents.LoadAllStudents();
			try
			{
				lastId = AllStudents.Students.ElementAt(AllStudents.Students.Count - 1).Id + 1;
			}
			catch (Exception)
			{
				lastId = 1;
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
	}
}
