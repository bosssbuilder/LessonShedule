using System;
using System.Drawing;
using System.Windows.Forms;

namespace LessonScheduleNew
{
    public class AddLessonDialog : Form
    {
        public Lesson? Lesson { get; private set; }

        private TextBox _txtDate;
        private TextBox _txtAudience;
        private TextBox _txtTeacher;
        private TextBox _txtQuality;
        private Button _btnOk;
        private Button _btnCancel;

        public AddLessonDialog()
        {
            _txtDate = new TextBox();
            _txtAudience = new TextBox();
            _txtTeacher = new TextBox();
            _btnOk = new Button();
            _btnCancel = new Button();

            SetupUI();
        }

        private void SetupUI()
        {
            Text = "Добавить занятие";
            Size = new Size(450, 280);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            Label lblDate = new Label
            {
                Text = "Дата (ГГГГ.ММ.ДД):",
                Location = new Point(15, 25),
                Size = new Size(130, 23)
            };

            Label lblAudience = new Label
            {
                Text = "Аудитория:",
                Location = new Point(15, 55),
                Size = new Size(130, 23)
            };

            Label lblTeacher = new Label
            {
                Text = "Преподаватель:",
                Location = new Point(15, 85),
                Size = new Size(130, 23)
            };

            Label lblQuality = new Label
            {
                Text = "Качество (отлично/хорошо):",
                Location = new Point(15, 115),
                Size = new Size(180, 23)
            };

            _txtDate.Location = new Point(200, 22);
            _txtDate.Size = new Size(200, 23);
            _txtDate.Text = DateTime.Now.ToString("yyyy.MM.dd");

            _txtAudience.Location = new Point(200, 52);
            _txtAudience.Size = new Size(200, 23);

            _txtTeacher.Location = new Point(200, 82);
            _txtTeacher.Size = new Size(200, 23);

            _txtQuality.Location = new Point(200, 112);
            _txtQuality.Size = new Size(200, 23);
            _txtQuality.Text = "отлично";

            _btnOk.Text = "OK";
            _btnOk.Location = new Point(230, 160);
            _btnOk.Size = new Size(75, 30);
            _btnOk.DialogResult = DialogResult.OK;
            _btnOk.Click += BtnOk_Click;

            _btnCancel.Text = "Отмена";
            _btnCancel.Location = new Point(315, 160);
            _btnCancel.Size = new Size(75, 30);
            _btnCancel.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[]
            {
                lblDate, lblAudience, lblTeacher, lblQuality,
                _txtDate, _txtAudience, _txtTeacher, _txtQuality,
                _btnOk, _btnCancel
            });
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            try
            {
                DateTime date = DateTime.ParseExact(_txtDate.Text, "yyyy.MM.dd", null);
                Lesson = new Lesson(date, _txtAudience.Text.Trim(), _txtTeacher.Text.Trim(), _txtQuality.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _txtDate?.Dispose();
                _txtAudience?.Dispose();
                _txtTeacher?.Dispose();
                _txtQuality?.Dispose();
                _btnOk?.Dispose();
                _btnCancel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}