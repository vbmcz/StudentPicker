using CommunityToolkit.Maui.Views;
using StudentPicker.Models;

namespace StudentPicker.Views;

public partial class EditPopup : Popup
{
	public EditPopup()
	{
		InitializeComponent();
		NameEntry.Text = AllStudents.editStudent.Name;
		EditClassEntry.Text = AllStudents.editStudent.ClassId;
		IsPresentChckbx.IsChecked = AllStudents.editStudent.IsPresent;
	}
	private void EditStudent(object sender, EventArgs e)
	{
		string editName = NameEntry.Text.ToString(),
			editClass = EditClassEntry.Text.ToString();
		bool editIsPresent = IsPresentChckbx.IsChecked;

		AllStudents.editStudent.Name = editName;
		AllStudents.editStudent.ClassId = editClass;
		AllStudents.editStudent.IsPresent = editIsPresent;

		AllStudents.LoadAllStudents();
		File.Delete(AllStudents.dataFilePath);
		foreach(Student student in AllStudents.Students)
		{
			if(student.Id == AllStudents.editStudent.Id)
			{
				student.Name = editName;
				student.ClassId = editClass;
				student.IsPresent = editIsPresent;
			}
			File.AppendAllText(AllStudents.dataFilePath, student.ToString());
		}
		AllStudents.Students.Clear();
		AllStudents.LoadStudentsByClass(editClass);


		Close();
	}
}