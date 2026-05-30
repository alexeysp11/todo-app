using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Serilog;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Abstractions;

namespace TodoApp.WinForms.Views
{
    public partial class MainForm : Form
    {
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public MainForm(ILogger logger, ITaskService taskService, ICategoryService categoryService)
        {
            InitializeComponent();
            _logger = logger.ForContext<MainForm>();
            _taskService = taskService;
            _categoryService = categoryService;

            Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            _logger.Information("Loading the main interface form.");
            
            await Task.WhenAll(LoadCategoriesFilterAsync(), RefreshTaskListAsync());
        }

        private async Task RefreshTaskListAsync()
        {
            try
            {
                dgvTasks.Enabled = false;
                statusStrip.Text = "Loading tasks from database...";

                IEnumerable<TodoTaskEntity> tasks = await _taskService.GetAllTasksAsync();

                if (cmbFilterCategory.SelectedValue is int selectedCategoryId && selectedCategoryId > 0)
                {
                    tasks = tasks.Where(t => t.CategoryId == selectedCategoryId);
                }

                dgvTasks.DataSource = null;
                dgvTasks.DataSource = tasks.ToList();

                ConfigureGridColumns();
                statusStrip.Text = $"Total tasks: {dgvTasks.Rows.Count}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical error updating task list.");
                statusStrip.Text = "Error loading data.";
                MessageBox.Show("Error updating task list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dgvTasks.Enabled = true;
            }
        }

        private async Task LoadCategoriesFilterAsync()
        {
            try
            {
                List<CategoryEntity> categories = new List<CategoryEntity>
                {
                    new CategoryEntity { Id = 0, Name = "All Categories" }
                };

                IEnumerable<CategoryEntity> dbCategories = await _categoryService.GetAllCategoriesAsync();
                categories.AddRange(dbCategories);

                cmbFilterCategory.DataSource = categories;
                cmbFilterCategory.DisplayMember = "Name";
                cmbFilterCategory.ValueMember = "Id";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading category filter.");
                MessageBox.Show("Failed to load categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void CmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            await RefreshTaskListAsync();
        }

        private void ConfigureGridColumns()
        {
            if (dgvTasks.Columns.Count == 0) return;

            // Hide technical ID columns
            string[] technicalColumns = { "Id", "CategoryId", "PriorityId", "StatusId" };
            foreach (string col in technicalColumns)
            {
                if (dgvTasks.Columns[col] != null)
                    dgvTasks.Columns[col].Visible = false;
            }

            // Set human-readable headers
            if (dgvTasks.Columns["Name"] != null) dgvTasks.Columns["Name"].HeaderText = "Task name";
            if (dgvTasks.Columns["Description"] != null) dgvTasks.Columns["Description"].HeaderText = "Description";
            if (dgvTasks.Columns["CategoryName"] != null) dgvTasks.Columns["CategoryName"].HeaderText = "Category";
            if (dgvTasks.Columns["PriorityName"] != null) dgvTasks.Columns["PriorityName"].HeaderText = "Priority";
            if (dgvTasks.Columns["StatusName"] != null) dgvTasks.Columns["StatusName"].HeaderText = "Status";
            if (dgvTasks.Columns["DueDate"] != null) dgvTasks.Columns["DueDate"].HeaderText = "Due date";
            if (dgvTasks.Columns["CreatedAt"] != null) dgvTasks.Columns["CreatedAt"].HeaderText = "Created at";

            // Establish strict visible order via DisplayIndex (0-based)
            int currentOrder = 0;
            string[] visibleColumnsOrder = { "Name", "Description", "CategoryName", "PriorityName", "StatusName", "DueDate", "CreatedAt" };

            foreach (string columnName in visibleColumnsOrder)
            {
                if (dgvTasks.Columns[columnName] != null && dgvTasks.Columns[columnName].Visible)
                {
                    dgvTasks.Columns[columnName].DisplayIndex = currentOrder++;
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            // Checking if a row is selected in the grid
            if (dgvTasks.CurrentRow?.DataBoundItem is TodoTaskEntity selectedTask)
            {
                DialogResult result = MessageBox.Show($"Are you sure you want to delete task '{selectedTask.Name}'?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _logger.Information("User deletes task with ID: {TaskId}", selectedTask.Id);
                    await _taskService.DeleteTaskAsync(selectedTask.Id);
                    await RefreshTaskListAsync();
                }
            }
            else
            {
                MessageBox.Show("Please select a task to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
            {
                TaskEditForm editForm = scope.Resolve<TaskEditForm>();
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    await RefreshTaskListAsync();
                }
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvTasks.CurrentRow?.DataBoundItem is TodoTaskEntity selectedTask)
            {
                _logger.Information("Opening TaskEditForm in edit mode for Task ID: {TaskId}", selectedTask.Id);

                using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
                {
                    // Pass the selected task object as a typed parameter into the Autofac DI container
                    TaskEditForm editForm = scope.Resolve<TaskEditForm>(new TypedParameter(typeof(TodoTaskEntity), selectedTask));

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        await RefreshTaskListAsync();
                    }
                }
            }
            else
            {
                _logger.Warning("User tried to edit a task without selecting one.");
                MessageBox.Show("Please select a task to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnManageCategories_Click(object sender, EventArgs e)
        {
            _logger.Information("User opened Category Management window.");

            // Open the category management form using Autofac DI container
            using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
            {
                CategoryForm categoryForm = scope.Resolve<CategoryForm>();
                categoryForm.ShowDialog();
            }

            // Refresh the filters on MainForm because categories might have changed.
            _ = LoadCategoriesFilterAsync();
        }
    }
}
