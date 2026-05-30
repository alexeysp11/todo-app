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
            _logger.Information("Loading TaskEditForm.");
            ConfigureDatePicker();
            ConfigureComboBoxStyles();

            // Load drop-down reference data in parallel to save time
            await Task.WhenAll(
                LoadCategoriesAsync(),
                LoadPrioritiesAsync(),
                LoadStatusesAsync()
            );

            // Populate fields if we are in Edit Mode
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

                // Set default status to 'New' (assumed Id = 1 based on our init.sql)
                cmbStatus.SelectedValue = 1;
                
                // Hide status choice during creation since a new task is always 'New'
                cmbStatus.Enabled = false;
            }
        }

        private void ConfigureDatePicker()
        {
            // Switch the format mode to Custom
            dtpDueDate.Format = DateTimePickerFormat.Custom;

            // Define the strict European/Russian date mask
            dtpDueDate.CustomFormat = "dd.MM.yyyy";
        }

        private void ConfigureComboBoxStyles()
        {
            // Lock keyboard text input for all reference dropdowns
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                List<CategoryEntity> categories = new List<CategoryEntity> { new CategoryEntity { Id = 0, Name = "No Category" } };
                IEnumerable<CategoryEntity> dbCategories = await _categoryService.GetAllCategoriesAsync();
                categories.AddRange(dbCategories);

                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load categories into combobox.");
            }
        }

        private async Task LoadPrioritiesAsync()
        {
            try
            {
                IEnumerable<PriorityEntity> priorities = await _priorityRepository.GetAllAsync();
                cmbPriority.DataSource = priorities.ToList();
                cmbPriority.DisplayMember = "Name";
                cmbPriority.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load priorities.");
            }
        }

        private async Task LoadStatusesAsync()
        {
            try
            {
                IEnumerable<TaskStatusEntity> statuses = await _statusRepository.GetAllAsync();
                cmbStatus.DataSource = statuses.ToList();
                cmbStatus.DisplayMember = "Name";
                cmbStatus.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load task statuses.");
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                TodoTaskEntity task = _existingTask ?? new TodoTaskEntity();
                task.Name = txtName.Text.Trim();
                task.Description = string.IsNullOrWhiteSpace(txtDescription.Text) ? null : txtDescription.Text.Trim();
                task.CategoryId = (int)cmbCategory.SelectedValue == 0 ? (int?)null : (int)cmbCategory.SelectedValue;
                task.PriorityId = (int)cmbPriority.SelectedValue;
                task.StatusId = (int)cmbStatus.SelectedValue;
                task.DueDate = dtpDueDate.Value.Date;

                _logger.Information("Saving task. Mode: {Mode}", _existingTask == null ? "Create" : "Update");

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
                _logger.Warning("Task validation failure: {Message}", ex.Message);
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error saving a task.");
                MessageBox.Show("An unexpected system error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
