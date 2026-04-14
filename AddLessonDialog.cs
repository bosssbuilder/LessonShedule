using System;
using System.Drawing;
using System.Windows.Forms;

namespace LessonScheduleNew
{
    public partial class AddLessonDialog : Form
    {
        public Lesson Lesson;

        private TextBox _txtDate, _txtAudience, _txtTeacher;
        private Button _btnOk, _btnCancel;

        public AddLessonDialog()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Добавить занятие";
            this.Size = new Size(420, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblDate = new Label
            {
                Text = "Дата (ГГГГ.ММ.ДД):",
                Location = new Point(15, 25),
                Size = new Size(120, 23)
            };

            Label lblAudience = new Label
            {
                Text = "Аудитория:",
                Location = new Point(15, 55),
                Size = new Size(120, 23)
            };

            Label lblTeacher = new Label
            {
                Text = "Преподаватель:",
                Location = new Point(15, 85),
                Size = new Size(120, 23)
            };

            _txtDate = new TextBox { Location = new Point(150, 22), Size = new Size(230, 23) };
            _txtAudience = new TextBox { Location = new Point(150, 52), Size = new Size(230, 23) };
            _txtTeacher = new TextBox { Location = new Point(150, 82), Size = new Size(230, 23) };

            _txtDate.Text = DateTime.Now.ToString("yyyy.MM.dd");

            _btnOk = new Button
            {
                Text = "OK",
                Location = new Point(220, 130),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK
            };

            _btnCancel = new Button
            {
                Text = "Отмена",
                Location = new Point(305, 130),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            _btnOk.Click += BtnOk_Click;

            this.Controls.AddRange(new Control[] {
                lblDate, lblAudience, lblTeacher,
                _txtDate, _txtAudience, _txtTeacher,
                _btnOk, _btnCancel
            });
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime date = DateTime.ParseExact(_txtDate.Text, "yyyy.MM.dd", null);
                Lesson = new Lesson(date, _txtAudience.Text.Trim(), _txtTeacher.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}