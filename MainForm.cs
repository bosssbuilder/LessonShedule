using System;
using System.Drawing;
using System.Windows.Forms;

namespace LessonScheduleNew
{
    public class MainForm : Form
    {
        private readonly LessonService _service;
        private DataGridView _dataGridView;
        private Button _btnLoad;
        private Button _btnAdd;
        private Button _btnDelete;
        private Button _btnExecuteCommands;
        private Label _lblStatus;

        public MainForm()
        {
            _service = new LessonService();
            SetupUI();
            _service.DataChanged += (s, e) => RefreshGrid();
        }

        private void SetupUI()
        {
            Text = "Учебные занятия - Вариант 9";
            Size = new Size(900, 550);

            _dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false
            };

            _dataGridView.Columns.Add("Date", "Дата");
            _dataGridView.Columns.Add("Audience", "Аудитория");
            _dataGridView.Columns.Add("Teacher", "Преподаватель");
            _dataGridView.Columns.Add("Quality", "Качество"); // 🔥 ДОБАВИЛИ

            var topPanel = new Panel { Dock = DockStyle.Top, Height = 40 };

            _btnLoad = new Button
            {
                Text = "Загрузить из файла",
                Location = new Point(10, 8),
                Size = new Size(130, 28)
            };

            _btnAdd = new Button
            {
                Text = "Добавить",
                Location = new Point(150, 8),
                Size = new Size(100, 28)
            };

            _btnDelete = new Button
            {
                Text = "Удалить",
                Location = new Point(260, 8),
                Size = new Size(100, 28)
            };

            _btnExecuteCommands = new Button
            {
                Text = "Выполнить команды",
                Location = new Point(370, 8),
                Size = new Size(150, 28)
            };

            _btnLoad.Click += BtnLoad_Click;
            _btnAdd.Click += BtnAdd_Click;
            _btnDelete.Click += BtnDelete_Click;
            _btnExecuteCommands.Click += BtnExecuteCommands_Click;

            topPanel.Controls.AddRange(new Control[] { _btnLoad, _btnAdd, _btnDelete, _btnExecuteCommands });

            _lblStatus = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 25,
                Text = "Готов",
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.LightGray
            };

            Controls.Add(_dataGridView);
            Controls.Add(topPanel);
            Controls.Add(_lblStatus);
        }

        private void RefreshGrid()
        {
            _dataGridView.Rows.Clear();
            var lessons = _service.GetAll();

            foreach (var lesson in lessons)
            {
                _dataGridView.Rows.Add(
                    lesson.Date.ToString("yyyy.MM.dd"),
                    lesson.Audience,
                    lesson.Teacher,
                    lesson.Quality // 🔥 ДОБАВИЛИ
                );
            }

            _lblStatus.Text = $"Всего записей: {lessons.Count}";
        }

        private void BtnLoad_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Text files|*.txt|All files|*.*",
                Title = "Выберите файл с данными"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var (lessons, errors) = LessonParser.LoadFromFileWithLog(ofd.FileName);
                    _service.SetLessons(lessons);

                    if (errors.Count > 0)
                    {
                        string errorMsg = $"Загружено {lessons.Count} записей.\nПропущено строк с ошибками: {errors.Count}\n\nПервые 5 ошибок:\n";
                        for (int i = 0; i < Math.Min(5, errors.Count); i++)
                        {
                            errorMsg += $"Строка {errors[i].LineNumber}: {errors[i].ErrorMessage}\n   -> \"{errors[i].Line}\"\n";
                        }
                        MessageBox.Show(errorMsg, "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Загружено {lessons.Count} записей.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using AddLessonDialog dialog = new AddLessonDialog();
            if (dialog.ShowDialog() == DialogResult.OK && dialog.Lesson is not null)
            {
                _service.Add(dialog.Lesson);
                MessageBox.Show("Занятие добавлено", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите запись для удаления", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int index = _dataGridView.SelectedRows[0].Index;
                if (_service.RemoveAt(index))
                {
                    MessageBox.Show("Запись удалена", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnExecuteCommands_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Command files|*.txt|All files|*.*",
                Title = "Выберите файл с командами"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var parser = new CommandParser(_service);
                var log = parser.ExecuteCommands(ofd.FileName);
                RefreshGrid();

                string logMessage = string.Join(Environment.NewLine, log);
                MessageBox.Show(logMessage, "Результат выполнения команд", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataGridView?.Dispose();
                _btnLoad?.Dispose();
                _btnAdd?.Dispose();
                _btnDelete?.Dispose();
                _btnExecuteCommands?.Dispose();
                _lblStatus?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}