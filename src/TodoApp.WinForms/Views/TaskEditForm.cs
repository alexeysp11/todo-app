using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Abstractions;

namespace TodoApp.WinForms.Views
{
    public partial class TaskEditForm : Form
    {
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;
        private readonly IPriorityRepository _priorityRepository;
        private readonly ITaskStatusRepository _statusRepository;
        private readonly ILogger _logger;

        private readonly TodoTaskEntity _existingTask; // Null if creating a new task

        public TaskEditForm(
            ILogger logger,
            ITaskService taskService,
            ICategoryService categoryService,
            IPriorityRepository priorityRepository,
            ITaskStatusRepository statusRepository,
            TodoTaskEntity task = null)
        {
            InitializeComponent();

            _taskService = taskService;
            _categoryService = categoryService;
            _priorityRepository = priorityRepository;
            _statusRepository = statusRepository;
            _logger = logger.ForContext<TaskEditForm>();
            _existingTask = task;

            Load += TaskEditForm_Load;
            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => Close();
        }

        private async void TaskEditForm_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Information("Loading TaskEditForm UI.");
                ConfigureDatePicker();
                ConfigureComboBoxStyles();

                // Load drop-down reference data in parallel to optimize startup time
                await Task.WhenAll(
                    LoadCategoriesAsync(),
                    LoadPrioritiesAsync(),
                    LoadStatusesAsync()
                );

                // Populate UI fields based on operational mode
                if (_existingTask != null)
                {
                    Text = "TodoApp: Edit Task";
                    txtName.Text = _existingTask.Name;
                    txtDescription.Text = _existingTask.Description;
                    cmbCategory.SelectedValue = _existingTask.CategoryId ?? 0;
                    cmbPriority.SelectedValue = _existingTask.PriorityId;
                    cmbStatus.SelectedValue = _existingTask.StatusId;

                    if (_existingTask.DueDate.HasValue)
                        dtpDueDate.Value = _existingTask.DueDate.Value;
                }
                else
                {
                    Text = "TodoApp: Create New Task";
                    if (cmbStatus.DataSource is List<TaskStatusEntity> statusList)
                    {
                        var defaultStatus = statusList.FirstOrDefault(s => s.Name.Equals("New", StringComparison.OrdinalIgnoreCase));
                        if (defaultStatus != null) cmbStatus.SelectedValue = defaultStatus.Id;
                    }

                    // Hide status modification since a newborn task always starts as 'New'
                    cmbStatus.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Critical error during task creation/editing form startup.");
                MessageBox.Show("Failed to load required reference data form.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void ConfigureDatePicker()
        {
            dtpDueDate.Format = DateTimePickerFormat.Custom;
            dtpDueDate.CustomFormat = "dd.MM.yyyy";
        }

        private void ConfigureComboBoxStyles()
        {
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = new List<CategoryEntity> { new CategoryEntity { Id = 0, Name = "No Category" } };
                IEnumerable<CategoryEntity> dbCategories = await _categoryService.GetAllCategoriesAsync();
                categories.AddRange(dbCategories);

                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = nameof(CategoryEntity.Name);
                cmbCategory.ValueMember = nameof(CategoryEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to aggregate categories into combobox.");
                throw;
            }
        }

        private async Task LoadPrioritiesAsync()
        {
            try
            {
                IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync();
                cmbPriority.DataSource = priorities.ToList();
                cmbPriority.DisplayMember = nameof(PriorityEntity.Name);
                cmbPriority.ValueMember = nameof(PriorityEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load priorities data stream.");
                throw;
            }
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync();
                cmbStatus.DataSource = statuses.ToList();
                cmbStatus.DisplayMember = nameof(TaskStatusEntity.Name);
                cmbStatus.ValueMember = nameof(TaskStatusEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load task statuses infrastructure.");
                throw;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            string taskName = txtName.Text.Trim();

            // UI-Level Guard Validation: avoid hitting database endpoints for basic empty string errors
            if (string.IsNullOrWhiteSpace(taskName))
            {
                _logger.Warning("User attempted to save a task with an empty name field.");
                MessageBox.Show("Task name is required and cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            // Safe extraction pattern to prevent InvalidCastException if data isn't loaded completely
            if (!(cmbPriority.SelectedValue is int priorityId) || !(cmbStatus.SelectedValue is int statusId))
            {
                MessageBox.Show("Reference data is still loading. Please wait.", "System Busy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                TodoTaskEntity task = _existingTask ?? new TodoTaskEntity();
                task.Name = taskName;
                task.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
                task.CategoryId = cmbCategory.SelectedValue is int catId && catId == 0 ? (int?)null : (int?)cmbCategory.SelectedValue;
                task.PriorityId = priorityId;
                task.StatusId = statusId;
                task.DueDate = dtpDueDate.Value.Date;

                _logger.Information("Persisting task changes. Executing mode: {Mode}", _existingTask == null ? "Create" : "Update");

                if (_existingTask == null)
                {
                    await _taskService.CreateTaskAsync(task);
                }
                else
                {
                    await _taskService.UpdateTaskAsync(task);
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (ArgumentException ex)
            {
                _logger.Warning("Task operational parameters validation failure on BLL: {Message}", ex.Message);
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected exception intercept while executing save task action.");
                MessageBox.Show("An unexpected system error occurred while processing data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
